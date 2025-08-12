using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Commands;

namespace Tanuki.Atlyss.Core.Commands;

public class Manager
{
    public readonly Dictionary<string, ICommand> Aliases = [];
    public readonly Dictionary<ICommand, CommandConfiguration> Commands = [];

    /*
     * Perhaps this should be optimized, but I don't want to deal with it.
     * It is used only once during loading, so it does not impact overall performance.
     */
    public void RegisterCommands(IPlugin Plugin)
    {
        bool UpdateFile = false;
        Dictionary<string, CommandConfiguration> CommandConfigurations = null;

        Assembly Assembly = Plugin.GetType().Assembly;

        // Search for commands in the assembly
        List<ICommand> PluginCommands = [];
        foreach (Type Type in Assembly.GetTypes())
        {
            if (Type.IsAbstract || Type.IsInterface)
                continue;

            if (!typeof(ICommand).IsAssignableFrom(Type))
                continue;

            PluginCommands.Add((ICommand)Activator.CreateInstance(Type));
        }

        string Directory = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, Assembly.GetName().Name);
        string Path = System.IO.Path.Combine(Directory, string.Format(Environment.PluginCommandFileTemplate, Tanuki.Instance.Settings.Language, Environment.PluginCommandFileFormat));

        bool Exists = File.Exists(Path);
        if (!Exists)
        {
            foreach (string File in System.IO.Directory.GetFiles(Directory))
            {
                if (!File.Contains(Environment.PluginCommandFileFormat))
                    continue;

                Path = File;
                Exists = true;
                break;
            }
        }

        if (Exists)
            CommandConfigurations = JsonConvert.DeserializeObject<Dictionary<string, CommandConfiguration>>(File.ReadAllText(Path));

        CommandConfigurations ??= [];

        // Removing commands that are not present in the assembly from the configuration.
        List<string> PluginUnknownCommands = [];
        foreach (string CommandName in CommandConfigurations.Keys)
        {
            if (PluginCommands.Any(x => x.GetType().Name == CommandName))
                continue;

            PluginUnknownCommands.Add(CommandName);
            UpdateFile = true;
        }
        PluginUnknownCommands.ForEach(x => CommandConfigurations.Remove(x));

        // Add to the configuration with commands from the assembly.
        if (CommandConfigurations.Count != PluginCommands.Count)
        {
            foreach (ICommand Command in PluginCommands)
            {
                string CommandName = Command.GetType().Name;
                if (CommandConfigurations.ContainsKey(CommandName))
                    continue;

                CommandConfigurations.Add(
                    CommandName,
                    new()
                    {
                        Names = [CommandName.ToLower()]
                    }
                );
            }

            UpdateFile = true;
        }

        // Validation of configuration, filling in command aliases.
        foreach (KeyValuePair<string, CommandConfiguration> CommandConfiguration in CommandConfigurations)
        {
            ICommand Command = PluginCommands.Where(x => x.GetType().Name == CommandConfiguration.Key).First();
            Commands[Command] = CommandConfiguration.Value;

            if (CommandConfiguration.Value.Names is null)
                continue;

            if (CommandConfiguration.Value.Names.Count == 0)
            {
                CommandConfiguration.Value.Names = null;
                continue;
            }

            for (int i = CommandConfiguration.Value.Names.Count - 1; i >= 0; i--)
            {
                string CommandName = CommandConfiguration.Value.Names[i].ToLower();

                if (CommandName.Length == 0 || CommandName.Contains(' ') || Aliases.ContainsKey(CommandName))
                {
                    CommandConfiguration.Value.Names.RemoveAt(i);
                    UpdateFile = true;
                    continue;
                }

                Aliases.Add(CommandName, Command);
            }

            if (CommandConfiguration.Value.Names.Count == 0)
                CommandConfiguration.Value.Names = null;
        }

        if (UpdateFile)
            File.WriteAllText(Path, JsonConvert.SerializeObject(CommandConfigurations, Formatting.Indented));
    }
    public void DeregisterCommands(IPlugin Plugin)
    {
        Assembly Assembly = Plugin.GetType().Assembly;
        ICommand[] Commands = [.. this.Commands.Keys.Where(x => x.GetType().Assembly == Assembly)];
        for (ushort i = 0; i < Commands.Length; i++)
            RemoveCommand(Commands[i]);
    }
    private void RemoveCommand(ICommand Command)
    {
        if (!Commands.TryGetValue(Command, out CommandConfiguration CommandConfiguration))
            return;

        CommandConfiguration.Names?.ForEach(x => Aliases.Remove(x));
        Type Type = Command.GetType();
        if (typeof(IDisposable).IsAssignableFrom(Type))
            ((IDisposable)Command).Dispose();

        Commands.Remove(Command);
    }
    public void RemoveAllCommands()
    {
        foreach (ICommand Command in Commands.Keys)
            RemoveCommand(Command);
    }
    private static readonly HashSet<char> Quotes = ['"', '\'', '`'];
    public void OnSendMessage(string Message, ref bool ShouldAllow)
    {
        if (!Message.StartsWith("/"))
            return;

        int ArgumentsIndex = Message.IndexOf(' ');
        if (ArgumentsIndex < 0)
            ArgumentsIndex = Message.Length;

        string CommandName = Message.Substring(0, ArgumentsIndex).TrimStart('/').ToLower();

        if (!Aliases.TryGetValue(CommandName, out ICommand Command))
            return;

        List<string> Arguments = [];
        StringBuilder Argument = new();
        char? ArgumentOpenQuote = null;
        bool ArgumentQuoteEscaped = false;

        char Character;
        for (ushort i = (ushort)ArgumentsIndex; i < Message.Length; i++)
        {
            Character = Message[i];
            if (ArgumentOpenQuote.HasValue)
            {
                if (ArgumentQuoteEscaped)
                {
                    if (Character == '\\' || Quotes.Contains(Character))
                        Argument.Append(Character);
                    else
                    {
                        Argument.Append('\\');
                        Argument.Append(Character);
                    }
                    ArgumentQuoteEscaped = false;
                }
                else
                {
                    if (Character == '\\')
                        ArgumentQuoteEscaped = true;
                    else if (Character == ArgumentOpenQuote)
                    {
                        Arguments.Add(Argument.ToString());
                        Argument.Clear();
                        ArgumentOpenQuote = null;
                    }
                    else
                        Argument.Append(Character);
                }
            }
            else
            {
                if (Character == ' ')
                {
                    if (Argument.Length > 0)
                    {
                        Arguments.Add(Argument.ToString());
                        Argument.Clear();
                    }
                }
                else if (Quotes.Contains(Character))
                {
                    if (Argument.Length > 0)
                    {
                        Arguments.Add(Argument.ToString());
                        Argument.Clear();
                    }
                    ArgumentOpenQuote = Character;
                }
                else
                    Argument.Append(Character);
            }
        }

        if (ArgumentQuoteEscaped)
            Argument.Append('\\');

        if (Argument.Length > 0)
            Arguments.Add(Argument.ToString());

        ShouldAllow = false;

        try
        {
            ShouldAllow = Command.Execute([.. Arguments]);
        }
        catch (Exception Exception)
        {
            Tanuki.Instance.ManualLogSource.LogError(Exception);
        }
    }
}