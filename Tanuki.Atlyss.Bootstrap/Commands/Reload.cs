using System.Collections.Generic;
using System.Linq;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Commands;

namespace Tanuki.Atlyss.Bootstrap.Commands;

public class Reload : ICommand
{
    public bool Execute(string[] Arguments)
    {
        if (Arguments.Length == 0)
        {
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.Full"));
            Core.Tanuki.Instance.Plugins.ReloadPlugins();
            return false;
        }

        Arguments = [.. Arguments.Select(x => x.ToLower())];

        List<IPlugin> Plugins = [];

        string PluginName;
        foreach (IPlugin Plugin in Core.Tanuki.Instance.Plugins.Plugins)
        {
            foreach (string Argument in Arguments)
            {
                PluginName = Plugin.Name.ToLower();
                if (!PluginName.Contains(Argument))
                    continue;

                Plugins.Add(Plugin);
            }
        }

        if (Plugins.Count > 0)
        {
            ChatBehaviour._current.New_ChatMessage(
                Main.Instance.Translate(
                    "Commands.Reload.Plugins",
                    string.Join(
                        Main.Instance.Translate("Commands.Reload.Plugins.Separator"),
                        Plugins.Select(x => x.Name)
                    )
                )
            );

            for (int i = 0; i < Plugins.Count; i++)
                Core.Tanuki.Instance.Plugins.ReloadPlugin(Plugins[i]);

            return false;
        }

        ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.PluginsNotFound"));

        return false;
    }
}