using BepInEx;
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
    private static readonly HashSet<char> Quotes = ['"', '\'', '`'];
    public readonly Dictionary<string, ICommand> Aliases = [];
    public readonly Dictionary<ICommand, CommandConfiguration> Commands = [];

    public delegate void CommandRegistered(ICommand Command);
    public event CommandRegistered OnCommandRegistered;

    public delegate void CommandDeregistered(ICommand Command);
    public event CommandDeregistered OnCommandDeregistered;

    public delegate void CommandExecuted(ICommand Command, string[] Arguments, bool Success, ref bool ShouldContinue);
    public event CommandExecuted OnCommandExecuted;

    public void RegisterCommands(IPlugin Plugin)
    {
        Assembly Assembly = Plugin.GetType().Assembly;
        string[] Directories = System.IO.Directory.GetDirectories(Paths.ConfigPath, Plugin.Name, SearchOption.AllDirectories);
        string Directory = Directories.Length > 0 ? Directories[0] : System.IO.Path.Combine(Paths.ConfigPath, Plugin.Name);

        if (!System.IO.Directory.Exists(Directory))
            System.IO.Directory.CreateDirectory(Directory);

        bool UpdateFile = false;
        Dictionary<string, CommandConfiguration> CommandConfigurations = null;

        // Search for commands in the assembly
        List<ICommand> PluginCommands = [];
        foreach (Type Type in Assembly.GetTypes())
        {
            if (Type.IsAbstract || Type.IsInterface)
                continue;

            if (!typeof(ICommand).IsAssignableFrom(Type))
                continue;

            ICommand Command;
            try
            {
                Command = (ICommand)Activator.CreateInstance(Type);
            }
            catch (Exception Exception)
            {
                Tanuki.Instance.ManualLogSource.LogError(Exception);
                continue;
            }
            PluginCommands.Add(Command);
        }

        string Path = System.IO.Path.Combine(Directory, Environment.FormatPluginCommandsFile(Tanuki.Instance.Settings.Language));

        bool Exists = File.Exists(Path);
        if (!Exists)
        {
            foreach (string File in System.IO.Directory.GetFiles(Directory, Environment.FormatPluginCommandsFile("*")))
            {
                if (!File.Contains(Environment.PluginCommandsFileFormat))
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

            void ProcessAliases()
            {
                if (CommandConfiguration.Value.Names is null)
                    return;

                if (CommandConfiguration.Value.Names.Count == 0)
                {
                    CommandConfiguration.Value.Names = null;
                    return;
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

            ProcessAliases();
            OnCommandRegistered?.Invoke(Command);
        }

        if (UpdateFile)
            File.WriteAllText(Path, JsonConvert.SerializeObject(CommandConfigurations, Formatting.Indented));
    }

    private List<ICommand> FindRegisteredCommands(Assembly Assembly)
    {
        List<ICommand> Commands = [];

        foreach (ICommand Command in this.Commands.Keys)
        {
            if (Command.GetType().Assembly != Assembly)
                continue;

            Commands.Add(Command);
        }

        return Commands;
    }

    public void DeregisterCommands(IPlugin Plugin)
    {
        foreach (ICommand Command in FindRegisteredCommands(Plugin.GetType().Assembly))
            DeregisterCommand(Command);
    }

    private void DeregisterCommand(ICommand Command)
    {
        if (!Commands.TryGetValue(Command, out CommandConfiguration CommandConfiguration))
            return;

        if (CommandConfiguration.Names is not null)
            foreach (string Name in CommandConfiguration.Names)
                Aliases.Remove(Name);

        Type Type = Command.GetType();
        if (typeof(IDisposable).IsAssignableFrom(Type))
        {
            try
            {
                ((IDisposable)Command).Dispose();
            }
            catch (Exception Exception)
            {
                Tanuki.Instance.ManualLogSource.LogError(Exception);
            }
        }

        Commands.Remove(Command);
        OnCommandDeregistered?.Invoke(Command);
    }

    public void RemoveAllCommands()
    {
        foreach (ICommand Command in Commands.Keys)
            DeregisterCommand(Command);
    }

    public void OnSendMessage(string Message, ref bool ShouldAllow)
    {
        if (!Message.StartsWith("/"))
            return;

        int ArgumentsIndex = Message.IndexOf(' ');
        if (ArgumentsIndex < 0)
            ArgumentsIndex = Message.Length;

        string CommandName = Message.Substring(1, ArgumentsIndex - 1).ToLower();

        if (!Aliases.TryGetValue(CommandName, out ICommand Command))
            return;

        List<string> ArgumentsList = [];
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
                        ArgumentsList.Add(Argument.ToString());
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
                        ArgumentsList.Add(Argument.ToString());
                        Argument.Clear();
                    }
                }
                else if (Quotes.Contains(Character))
                {
                    if (Argument.Length > 0)
                    {
                        ArgumentsList.Add(Argument.ToString());
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
            ArgumentsList.Add(Argument.ToString());

        ShouldAllow = false;

        string[] Arguments = [.. ArgumentsList];

        bool Success = true;
        try
        {
            ShouldAllow = Command.Execute(Arguments);
        }
        catch (Exception Exception)
        {
            Success = false;
            Tanuki.Instance.ManualLogSource.LogError(Exception);
        }

        OnCommandExecuted?.Invoke(Command, Arguments, Success, ref ShouldAllow);
    }
}