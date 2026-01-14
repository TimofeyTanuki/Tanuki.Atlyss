using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tanuki.Atlyss.API.Commands;
using Tanuki.Atlyss.API.Plugins;
using Tanuki.Atlyss.Core.Models;

namespace Tanuki.Atlyss.Core.Commands;

public class Help : ICommand
{
    public EAllowedCaller AllowedCaller => EAllowedCaller.Player;
    public EExecutionSide ExecutionSide => EExecutionSide.Client;

    public bool Execute(Context Context)
    {
        string[] Arguments = Context.Arguments;

        StringBuilder
            Message = new(),
            MessageSection = new();

        string AdditionalNamesSeparator = Main.Instance.Translate("Commands.Help.Active.Entry.AdditionalNames.Separator");

        List<string> AdditionalNames = [];

        foreach (KeyValuePair<Type, IPlugin> Plugin in Tanuki.Instance.Plugins.PluginEntries)
        {
            Assembly Assembly = Plugin.Key.Assembly;

            if (!Tanuki.Instance.Commands.AssemblyCommands.TryGetValue(Assembly, out HashSet<Type> CommandTypes))
                continue;

            bool SkipPlugin = Arguments.Length != 0;

            foreach (string Argument in Arguments)
            {
                if (Plugin.Value.Name.IndexOf(Argument, StringComparison.InvariantCultureIgnoreCase) < 0)
                    continue;

                SkipPlugin = false;
                break;
            }

            if (SkipPlugin)
                continue;

            MessageSection.Clear();

            ushort InactiveCommandsCount = BuildMessageSection(MessageSection, CommandTypes, AdditionalNames, AdditionalNamesSeparator);

            if (MessageSection.Length == 0 &&
                InactiveCommandsCount == 0)
                continue;

            Message.Append(Main.Instance.Translate("Commands.Help.Header", Plugin.Value.Name));

            Message.Append(MessageSection);

            if (InactiveCommandsCount > 0)
                Message.Append(Main.Instance.Translate("Commands.Help.Inactive", InactiveCommandsCount));
        }

        if (Message.Length == 0)
        {
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Help.PluginsNotFound"));
            return false;
        }

        ChatBehaviour._current.New_ChatMessage(Message.ToString());

        return false;
    }

    private static ushort BuildMessageSection(StringBuilder StringBuilder, IEnumerable<Type> CommandTypes, List<string> AdditionalNames, string AdditionalNamesSeparator)
    {
        ushort InactiveCommandsCount = 0;

        foreach (Type CommandType in CommandTypes)
        {
            CommandConfigurationItem CommandConfiguration = Tanuki.Instance.Commands.CommandEntries[CommandType].Configuration;

            if (CommandConfiguration.Names.Count == 0)
            {
                InactiveCommandsCount++;
                continue;
            }

            AdditionalNames.Clear();

            int CommandNamesCount = CommandConfiguration.Names.Count;
            for (int i = 1; i < CommandNamesCount; i++)
                AdditionalNames.Add(Main.Instance.Translate("Commands.Help.Active.Entry.AdditionalNames.Item", CommandConfiguration.Names[i]));

            string CommandSyntax =
                string.IsNullOrEmpty(CommandConfiguration.Syntax) ?
                string.Empty : Main.Instance.Translate("Commands.Help.Active.Entry.Syntax", CommandConfiguration.Syntax);

            string CommandHelp =
                string.IsNullOrEmpty(CommandConfiguration.Help) ?
                string.Empty : Main.Instance.Translate("Commands.Help.Active.Entry.Help", CommandConfiguration.Help);

            string CommandAdditionalNames =
                CommandNamesCount > 1 ?
                Main.Instance.Translate("Commands.Help.Active.Entry.AdditionalNames", string.Join(AdditionalNamesSeparator, AdditionalNames)) : string.Empty;

            StringBuilder.Append(
                Main.Instance.Translate(
                    "Commands.Help.Active.Entry",
                    CommandConfiguration.Names[0],
                    CommandSyntax,
                    CommandAdditionalNames,
                    CommandHelp
                )
            );
        }

        return InactiveCommandsCount;
    }
}
