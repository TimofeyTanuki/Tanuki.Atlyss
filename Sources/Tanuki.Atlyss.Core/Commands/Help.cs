using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tanuki.Atlyss.API.Collections;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Commands;

[CommandMetadata(EExecutionSide.Client, typeof(Policies.Commands.Caller.Player))]
internal sealed class Help : ICommand
{
    private readonly Registers.Plugins pluginRegistry;
    private readonly Registers.Commands commandRegistry;
    private readonly Managers.Chat chatManager;
    private readonly TranslationSet translationSet;

    public Help()
    {
        pluginRegistry = Tanuki.Instance.Registers.Plugins;
        commandRegistry = Tanuki.Instance.Registers.Commands;
        chatManager = Tanuki.Instance.Managers.Chat;
        translationSet = Main.Instance.translationSet;
    }

    public void Execute(IContext context)
    {
        IReadOnlyList<string> arguments = context.Arguments;

        StringBuilder
            message = new(),
            messageSection = new();

        string additionalNamesSeparator = translationSet.Translate("Commands.Help.Active.Entry.AdditionalNames.Separator");

        List<string> additionalNames = [];

        foreach (KeyValuePair<Assembly, HashSet<Type>> assemblyCommands in commandRegistry.AssemblyCommands)
        {
            Assembly assembly = assemblyCommands.Key;

            string sectionName =
                pluginRegistry.AssemblyPlugins.TryGetValue(assembly, out HashSet<Type> AssemblyPlugins) ?
                pluginRegistry.PluginInterfaces[AssemblyPlugins.First()].Name : assembly.GetName().Name;

            bool skipCommand = arguments.Count != 0;

            foreach (string argument in arguments)
            {
                if (sectionName.IndexOf(argument, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                skipCommand = false;
                break;
            }

            if (skipCommand)
                continue;

            messageSection.Clear();

            ushort inactiveCommands = 0;

            foreach (Type command in assemblyCommands.Value)
            {
                if (!commandRegistry.Descriptors.TryGetValue(command, out Data.Commands.Descriptor registryEntry))
                    continue;

                Serialization.Commands.Configuration? configuration = registryEntry.Configuration;

                if (configuration is null)
                    continue;

                List<string> names = configuration.names;
                int namesCount = names.Count;

                if (namesCount == 0)
                {
                    inactiveCommands++;
                    continue;
                }

                additionalNames.Clear();

                for (int index = 1; index < namesCount; index++)
                    additionalNames.Add(translationSet.Translate("Commands.Help.Active.Entry.AdditionalNames.Item", configuration.names[index]));

                string commandSyntax =
                    string.IsNullOrEmpty(configuration.syntax) ?
                    string.Empty : translationSet.Translate("Commands.Help.Active.Entry.Syntax", configuration.syntax);

                string commandHelp =
                    string.IsNullOrEmpty(configuration.help) ?
                    string.Empty : translationSet.Translate("Commands.Help.Active.Entry.Help", configuration.help);

                string commandAdditionalNames =
                    namesCount > 1 ?
                    translationSet.Translate("Commands.Help.Active.Entry.AdditionalNames", string.Join(additionalNamesSeparator, additionalNames)) : string.Empty;

                messageSection.Append(
                    translationSet.Translate(
                        "Commands.Help.Active.Entry",
                        names[0],
                        commandSyntax,
                        commandAdditionalNames,
                        commandHelp
                    )
                );
            }

            if (messageSection.Length == 0 && inactiveCommands == 0)
                continue;

            message.Append(translationSet.Translate("Commands.Help.Header", sectionName));

            message.Append(messageSection);

            if (inactiveCommands > 0)
                message.Append(translationSet.Translate("Commands.Help.Inactive", inactiveCommands));
        }

        if (message.Length == 0)
        {
            chatManager.SendClientMessage(translationSet.Translate("Commands.Help.PluginsNotFound"));
            return;
        }

        chatManager.SendClientMessage(message.ToString());
    }
}
