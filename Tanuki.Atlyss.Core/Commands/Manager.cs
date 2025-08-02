using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Commands;
using Tanuki.Atlyss.Core.Plugins;

namespace Tanuki.Atlyss.Core.Commands;

public class Manager
{
    public Dictionary<string, ICommand> Aliases = [];
    public Dictionary<ICommand, CommandConfiguration> Commands = [];
    public void RegisterCommands(Plugin Plugin)
    {
        bool UpdateFile = false;
        Dictionary<string, CommandConfiguration> CommandConfigurations = null;
        ICommand Command;

        // Обнаружение всех команд в сборке плагина.
        List<ICommand> PluginCommands = [];
        foreach (Type Type in Plugin.Assembly.GetTypes())
        {
            if (Type.IsAbstract || Type.IsInterface)
                continue;

            if (!typeof(ICommand).IsAssignableFrom(Type))
                continue;

            Command = (ICommand)Activator.CreateInstance(Type);
            PluginCommands.Add(Command);
        }

        if (File.Exists(Plugin.Settings.Command))
            CommandConfigurations = JsonConvert.DeserializeObject<Dictionary<string, CommandConfiguration>>(File.ReadAllText(Plugin.Settings.Command));
        else
            UpdateFile = true;

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
            File.WriteAllText(Plugin.Settings.Command, JsonConvert.SerializeObject(CommandConfigurations, Formatting.Indented));
    }
    public void DeregisterCommands(Plugin Plugin)
    {
        // Удаление зарегистрированных псевдонимов команд
        List<string> RemovedAliases = [];
        foreach (KeyValuePair<string, ICommand> Alias in Aliases)
        {
            if (Alias.Value.GetType().Assembly != Plugin.Assembly)
                continue;

            RemovedAliases.Add(Alias.Key);
        }
        RemovedAliases.ForEach(x => Aliases.Remove(x));

        // Удаление команд
        List<ICommand> RemovedCommands = [];
        foreach (ICommand Command in Commands.Keys)
        {
            if (Command.GetType().Assembly != Plugin.Assembly)
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