using System.Collections.Generic;
using Tanuki.Atlyss.API.Commands;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Commands;

public class Reload : ICommand
{
    public EAllowedCaller AllowedCaller => EAllowedCaller.Player;
    public EExecutionSide ExecutionSide => EExecutionSide.Client;

    public bool Execute(Context Context)
    {
        string[] Arguments = Context.Arguments;

        if (Arguments.Length == 0)
        {
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.Full"));
            Tanuki.Instance.Plugins.ReloadPlugins();
            return false;
        }

        List<IPlugin> Plugins = [];
        List<string> PluginNames = [];

        foreach (IPlugin Plugin in Tanuki.Instance.Plugins.PluginEntries.Values)
        {
            string PluginName = Plugin.Name;
            bool Skip = true;

            foreach (string Argument in Arguments)
            {
                if (PluginName.IndexOf(Argument, System.StringComparison.InvariantCultureIgnoreCase) < 0)
                    continue;

                Skip = false;
                break;
            }

            if (Skip)
                continue;

            Plugins.Add(Plugin);
            PluginNames.Add(PluginName);
        }

        if (Plugins.Count == 0)
        {
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.PluginsNotFound"));
            return false;
        }

        ChatBehaviour._current.New_ChatMessage(
            Main.Instance.Translate(
                "Commands.Reload.Plugins",
                string.Join(
                    Main.Instance.Translate("Commands.Reload.Plugins.Separator"),
                    PluginNames
                )
            )
        );

        foreach (IPlugin Plugin in Plugins)
            Tanuki.Instance.Plugins.ReloadPlugin(Plugin);

        return false;
    }
}
