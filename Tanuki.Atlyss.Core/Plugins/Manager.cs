using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using Tanuki.Atlyss.API;

namespace Tanuki.Atlyss.Core.Plugins;

public class Manager
{
    public List<IPlugin> Plugins { get; internal set; } = [];
    public void Reload()
    {
        Plugins.ForEach(x => x.UnloadPlugin(EPluginState.Unloaded));
        Plugins.Clear();
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

            Plugins.Add(Plugin);
            Plugin.LoadPlugin();
        }

        Console.WriteLine($"Plugins (x{Plugins.Count}):\n{string.Join(", ", Plugins.Select(x => x.Name))}");
        Console.WriteLine($"Commands (x{Tanuki.Instance.Commands.Commands.Count}):\n{string.Join(", ", Tanuki.Instance.Commands.Commands.Keys.Select(x => x.GetType().FullName))}");
        Console.WriteLine($"Aliases (x{Tanuki.Instance.Commands.Aliases.Count}):\n{string.Join(", ", Tanuki.Instance.Commands.Aliases.Keys)}");
    }
}