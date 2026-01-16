using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Commands;

public sealed class Help : ICommand
{
    private readonly Registers.Plugins pluginRegistry = Tanuki.Instance.Registers.Plugins;
    private readonly Registers.Commands commandRegistry = Tanuki.Instance.Registers.Commands;

    public ICallerPolicy CallerPolicy => new Policies.Commands.Caller.MainPlayer();
    public IExecutionPolicy ExecutionPolicy => new Policies.Commands.Execution.Player();

    public bool Execute(IContext context)
    {
        IReadOnlyList<string> arguments = context.Arguments;

        StringBuilder
            message = new(),
            messageSection = new();

        string additionalNamesSeparator = Main.Instance.Translate("Commands.Help.Active.Entry.AdditionalNames.Separator");

        List<string> additionalNames = [];

        foreach (KeyValuePair<Assembly, HashSet<Type>> assemblyCommands in commandRegistry.AssemblyCommands)
        {
            Assembly assembly = assemblyCommands.Key;

            string sectionName =
                pluginRegistry.AssemblyPlugins.TryGetValue(assembly, out HashSet<Type> AssemblyPlugins) ?
                pluginRegistry.PluginInterfaces[AssemblyPlugins.First()].Name : assembly.GetName().Name;

            bool skip = arguments.Count != 0;

            foreach (string argument in arguments)
            {
                if (sectionName.IndexOf(argument, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                skip = false;
                break;
            }

            if (skip)
                continue;

            messageSection.Clear();

            ushort inactiveCommands = BuildMessageSection(messageSection, assemblyCommands.Value, additionalNames, additionalNamesSeparator);

            if (messageSection.Length == 0 &&
                inactiveCommands == 0)
                continue;

            message.Append(Main.Instance.Translate("Commands.Help.Header", sectionName));

            message.Append(messageSection);

            if (inactiveCommands > 0)
                message.Append(Main.Instance.Translate("Commands.Help.Inactive", inactiveCommands));
        }

        if (message.Length == 0)
        {
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Help.PluginsNotFound"));
            return false;
        }

        ChatBehaviour._current.New_ChatMessage(message.ToString());

        return false;
    }

    private ushort BuildMessageSection(StringBuilder stringBuilder, IEnumerable<Type> commands, List<string> additionalNames, string additionalNamesSeparator)
    {
        ushort inactiveCommands = 0;

        foreach (Type command in commands)
        {
            Serialization.Commands.Configuration configuration = commandRegistry.CommandConfigurations[command];

            if (configuration.names.Count == 0)
            {
                inactiveCommands++;
                continue;
            }

            additionalNames.Clear();

            int commandNamesCount = configuration.names.Count;
            for (int index = 1; index < commandNamesCount; index++)
                additionalNames.Add(Main.Instance.Translate("Commands.Help.Active.Entry.AdditionalNames.Item", configuration.names[index]));

            string commandSyntax =
                string.IsNullOrEmpty(configuration.syntax) ?
                string.Empty : Main.Instance.Translate("Commands.Help.Active.Entry.Syntax", configuration.syntax);

            string commandHelp =
                string.IsNullOrEmpty(configuration.help) ?
                string.Empty : Main.Instance.Translate("Commands.Help.Active.Entry.Help", configuration.help);

            string commandAdditionalNames =
                commandNamesCount > 1 ?
                Main.Instance.Translate("Commands.Help.Active.Entry.AdditionalNames", string.Join(additionalNamesSeparator, additionalNames)) : string.Empty;

            stringBuilder.Append(
                Main.Instance.Translate(
                    "Commands.Help.Active.Entry",
                    configuration.names[0],
                    commandSyntax,
                    commandAdditionalNames,
                    commandHelp
                )
            );
        }

        return inactiveCommands;
    }
}
