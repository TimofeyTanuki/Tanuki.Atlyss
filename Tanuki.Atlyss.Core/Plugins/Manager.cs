using BepInEx;
using System.Collections.Generic;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Plugins;

public class Manager
{
    public readonly HashSet<IPlugin> Plugins = [];
    public void LoadPlugins()
    {
        BaseUnityPlugin BaseUnityPlugin;
        IPlugin Plugin;
        foreach (PluginInfo PluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            BaseUnityPlugin = PluginInfo.Instance;
            if (BaseUnityPlugin is null)
                continue;

            if (!typeof(IPlugin).IsAssignableFrom(BaseUnityPlugin.GetType()))
                continue;

            Plugin = (IPlugin)BaseUnityPlugin;

            if (Plugins.Add(Plugin))
                Plugin.LoadPlugin();
        }
    }
    public void ReloadPlugins()
    {
        foreach (IPlugin Plugin in Plugins)
            ReloadPlugin(Plugin);

        /*
        Console.WriteLine($"Plugins (x{Plugins.Count}):\n{string.Join(", ", Plugins.Select(x => x.Name))}");
        Console.WriteLine($"Commands (x{Tanuki.Instance.Commands.Commands.Count}):\n{string.Join(", ", Tanuki.Instance.Commands.Commands.Keys.Select(x => x.GetType().FullName))}");
        Console.WriteLine($"Aliases (x{Tanuki.Instance.Commands.Aliases.Count}):\n{string.Join(", ", Tanuki.Instance.Commands.Aliases.Keys)}");
        */
    }
    public void ReloadPlugin(IPlugin Plugin)
    {
        if (Plugin.State != EState.Unloaded)
            Plugin.UnloadPlugin(EState.Unloaded);

        Plugin.LoadPlugin();
    }
}