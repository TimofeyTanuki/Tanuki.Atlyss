using System.Collections.Generic;
using Tanuki.Atlyss.API.Collections;
using Tanuki.Atlyss.API.Core.Commands;
using Tanuki.Atlyss.API.Core.Plugins;

namespace Tanuki.Atlyss.Core.Commands;

[CommandMetadata(EExecutionSide.Client, typeof(Policies.Commands.Caller.Player))]
internal sealed class Reload : ICommand
{
    private static readonly Registers.Plugins pluginRegistry;
    private static readonly Managers.Plugin pluginManager;
    private static readonly Managers.Chat chatManager;
    private static readonly TranslationSet translationSet;

    static Reload()
    {
        pluginRegistry = Tanuki.Instance.Registers.Plugins;
        pluginManager = Tanuki.Instance.Managers.Plugin;
        chatManager = Tanuki.Instance.Managers.Chat;
        translationSet = Main.Instance.TranslationSet;
    }

    public void Execute(IContext context)
    {
        IReadOnlyList<string> arguments = context.Arguments;

        if (arguments.Count == 0)
        {
            chatManager.AddMessage(translationSet.Translate("Commands.Reload.Full"));
            pluginManager.ReloadPlugins();
            return;
        }

        List<IPlugin> plugins = [];
        List<string> pluginNames = [];

        foreach (IPlugin plugin in pluginRegistry.PluginInterfaces.Values)
        {
            string pluginName = plugin.Name;
            bool skip = true;

            foreach (string Argument in arguments)
            {
                if (pluginName.IndexOf(Argument, System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                skip = false;
                break;
            }

            if (skip)
                continue;

            plugins.Add(plugin);
            pluginNames.Add(pluginName);
        }

        if (plugins.Count == 0)
        {
            chatManager.AddMessage(translationSet.Translate("Commands.Reload.PluginsNotFound"));
            return;
        }

        chatManager.AddMessage(
            translationSet.Translate(
                "Commands.Reload.Plugins",
                string.Join(
                    translationSet.Translate("Commands.Reload.Plugins.Separator"),
                    pluginNames
                )
            )
        );

        foreach (IPlugin plugin in plugins)
            pluginManager.ReloadPlugin(plugin);

        return;
    }
}
