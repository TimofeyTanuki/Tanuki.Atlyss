using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Commands;
using Tanuki.Atlyss.Core.Commands;

namespace Tanuki.Atlyss.Bootstrap.Commands;

public class Help : ICommand
{
    private class PluginEntry(string Name)
    {
        public string Name = Name;
        public List<CommandConfiguration> Active = [];
        public SortedSet<string> Inactive = [];
    }
    public void Execute(string[] Arguments)
    {
        SortedDictionary<string, PluginEntry> Groups = [];

        if (Arguments.Length > 0)
        {
            Arguments = [.. Arguments.Select(x => x.ToLower())];

            foreach (IPlugin Plugin in Core.Tanuki.Instance.Plugins.Plugins)
            {
                if (!Arguments.Any(x => Plugin.Name.ToLower().Contains(x)))
                    continue;

                Groups.Add(Plugin.GetType().Assembly.GetName().Name, new(Plugin.Name));
                break;
            }

            if (Groups.Count == 0)
            {
                ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Help.PluginsNotFound"));
                return;
            }
        }
        else
            foreach (IPlugin Plugin in Core.Tanuki.Instance.Plugins.Plugins)
                Groups.Add(Plugin.GetType().Assembly.GetName().Name, new(Plugin.Name));

        CollectCommands(ref Groups);

        StringBuilder StringBuilder = new();
        foreach (KeyValuePair<string, PluginEntry> PluginEntry in Groups)
        {
            StringBuilder.AppendLine(Main.Instance.Translate("Commands.Help.Header", PluginEntry.Value.Name));

            if (PluginEntry.Value.Active.Count > 0)
                foreach (CommandConfiguration Active in PluginEntry.Value.Active.OrderBy(x => x.Names[0]))
                    StringBuilder.AppendLine(
                        Main.Instance.Translate(
                            "Commands.Help.Active.Entry",
                            Active.Names[0],
                            string.IsNullOrEmpty(Active.Syntax) ?
                                string.Empty
                                :
                                Main.Instance.Translate("Commands.Help.Active.Entry.Syntax", Active.Syntax),
                            string.IsNullOrEmpty(Active.Help) ?
                                string.Empty
                                :
                                Main.Instance.Translate("Commands.Help.Active.Entry.Help", Active.Help),
                            Active.Names.Count > 1 ?
                                Main.Instance.Translate(
                                    "Commands.Help.Active.Entry.Aliases",
                                    string.Join(
                                        Main.Instance.Translate("Commands.Help.Active.Entry.Alias.Separator"),
                                        Active.Names
                                            .Skip(1)
                                            .Select(x => Main.Instance.Translate("Commands.Help.Active.Entry.Alias", x))
                                    )
                                )
                                :
                                string.Empty
                        )
                    );

            if (PluginEntry.Value.Inactive.Count > 0)
                StringBuilder.AppendLine(Main.Instance.Translate("Commands.Help.Inactive", string.Join(Main.Instance.Translate("Commands.Help.Inactive.Separator"), PluginEntry.Value.Inactive)));

            if (PluginEntry.Value.Active.Count == 0 && PluginEntry.Value.Inactive.Count == 0)
                StringBuilder.Append(Main.Instance.Translate("Commands.Help.NoEntries"));
        }

        ChatBehaviour._current.New_ChatMessage(StringBuilder.ToString());
    }
    private void CollectCommands(ref SortedDictionary<string, PluginEntry> Groups)
    {
        string Assembly;

        foreach (KeyValuePair<ICommand, CommandConfiguration> KeyValuePair in Core.Tanuki.Instance.Commands.Commands)
        {
            Assembly = KeyValuePair.Key.GetType().Assembly.GetName().Name;
            if (!Groups.TryGetValue(Assembly, out PluginEntry Entry))
                continue;


            if (KeyValuePair.Value.Names is null)
            {
                Entry.Inactive.Add(KeyValuePair.Key.GetType().Name);
                continue;
            }

            Entry.Active.Add(KeyValuePair.Value);
        }
    }
}