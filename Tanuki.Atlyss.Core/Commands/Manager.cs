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
    public void RegisterCommands(IPlugin Plugin)
    {
        bool UpdateFile = false;
        Dictionary<string, CommandConfiguration> CommandConfigurations = null;
        ICommand Command;

        Assembly Assembly = Plugin.GetType().Assembly;

        // Обнаружение всех команд в сборке плагина.
        List<ICommand> PluginCommands = [];
        foreach (Type Type in Assembly.GetTypes())
        {
            if (Type.IsAbstract || Type.IsInterface)
                continue;

            if (!typeof(ICommand).IsAssignableFrom(Type))
                continue;

            Command = (ICommand)Activator.CreateInstance(Type);
            PluginCommands.Add(Command);
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

        // Удаление из конфигурации команд, которые отсутствуют в сборке плагина.
        List<string> PluginUnknownCommands = [];
        foreach (string CommandName in CommandConfigurations.Keys)
        {
            if (PluginCommands.Any(x => x.GetType().Name == CommandName))
                continue;

            PluginUnknownCommands.Add(CommandName);
            UpdateFile = true;
        }
        PluginUnknownCommands.ForEach(x => CommandConfigurations.Remove(x));

        // Дополнение конфигурации командами из сборки плагина.
        if (CommandConfigurations.Count != PluginCommands.Count)
        {
            string CommandName;
            foreach (ICommand CommandX in PluginCommands)
            {
                CommandName = CommandX.GetType().Name;
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

        // Валидация конфигурации.
        foreach (KeyValuePair<string, CommandConfiguration> CommandConfiguration in CommandConfigurations)
        {
            Command = PluginCommands.Where(x => x.GetType().Name == CommandConfiguration.Key).First();
            Commands[Command] = CommandConfiguration.Value;

            if (CommandConfiguration.Value.Names is null)
                continue;

            if (CommandConfiguration.Value.Names.Count == 0)
            {
                CommandConfiguration.Value.Names = null;
                continue;
            }

            string CommandName;
            for (int i = CommandConfiguration.Value.Names.Count - 1; i >= 0; i--)
            {
                CommandName = CommandConfiguration.Value.Names[i].ToLower();

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

        // Удаление зарегистрированных псевдонимов команд
        List<string> RemovedAliases = [];
        foreach (KeyValuePair<string, ICommand> Alias in Aliases)
        {
            if (Alias.Value.GetType().Assembly != Assembly)
                continue;

            RemovedAliases.Add(Alias.Key);
        }
        RemovedAliases.ForEach(x => Aliases.Remove(x));

        // Удаление команд
        List<ICommand> RemovedCommands = [];
        foreach (ICommand Command in Commands.Keys)
        {
            if (Command.GetType().Assembly != Assembly)
                continue;

            RemovedCommands.Add(Command);
        }
        RemovedCommands.ForEach(x => Commands.Remove(x));
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

        ShouldAllow = false;

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

        Command.Execute([.. Arguments]);
    }
}