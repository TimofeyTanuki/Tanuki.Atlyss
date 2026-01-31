using System.Collections.Generic;
using Tanuki.Atlyss.API.Core.Commands;
using Tanuki.Atlyss.API.Core.Plugins;

namespace Tanuki.Atlyss.Core.Commands;

[CommandMetadata(EExecutionSide.Client, typeof(Policies.Commands.Caller.Player))]
public sealed class Reload : ICommand
{
    private readonly Registers.Plugins pluginRegistry = Tanuki.Instance.registers.Plugins;
    private readonly Managers.Plugins pluginManager = Tanuki.Instance.managers.Plugins;
    private readonly Main main = Main.Instance;

    public void Execute(IContext context)
    {
        IReadOnlyList<string> arguments = context.Arguments;

        if (arguments.Count == 0)
        {
            ChatBehaviour._current.New_ChatMessage(main.Translate("Commands.Reload.Full"));
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
            ChatBehaviour._current.New_ChatMessage(main.Translate("Commands.Reload.PluginsNotFound"));
            return;
        }

        ChatBehaviour._current.New_ChatMessage(
            main.Translate(
                "Commands.Reload.Plugins",
                string.Join(
                    main.Translate("Commands.Reload.Plugins.Separator"),
                    pluginNames
                )
            )
        );

        foreach (IPlugin plugin in plugins)
            pluginManager.ReloadPlugin(plugin);

        return;
    }
}
