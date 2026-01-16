using System.Collections.Generic;
using Tanuki.Atlyss.API.Tanuki.Commands;
using Tanuki.Atlyss.API.Tanuki.Plugins;

namespace Tanuki.Atlyss.Core.Commands;

public sealed class Reload : ICommand
{
    private static readonly ICallerPolicy callerPolicy = new Policies.Commands.Caller.Player();
    private static readonly EExecutionType executionType = EExecutionType.Local;

    private readonly Registers.Plugins pluginRegistry = Tanuki.Instance.registers.Plugins;
    private readonly Managers.Plugins pluginManager = Tanuki.Instance.managers.Plugins;

    public ICallerPolicy CallerPolicy => callerPolicy;
    public EExecutionType ExecutionType => executionType;

    public void ClientCallback(IContext context)
    {
        IReadOnlyList<string> arguments = context.Arguments;

        if (arguments.Count == 0)
        {
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.Full"));
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
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.PluginsNotFound"));
            return;
        }

        ChatBehaviour._current.New_ChatMessage(
            Main.Instance.Translate(
                "Commands.Reload.Plugins",
                string.Join(
                    Main.Instance.Translate("Commands.Reload.Plugins.Separator"),
                    pluginNames
                )
            )
        );

        foreach (IPlugin plugin in plugins)
            pluginManager.ReloadPlugin(plugin);

        return;
    }
}
