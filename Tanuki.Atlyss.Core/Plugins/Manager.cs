using BepInEx;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Plugins;

public class Manager
{
    public delegate void BeforePluginsReload();
    public event BeforePluginsReload OnBeforePluginsReload;

    public delegate void AfterPluginsReload();
    public event AfterPluginsReload OnAfterPluginsReload;

    public readonly HashSet<IPlugin> Plugins = [];
    internal void LoadPlugins()
    {
        BaseUnityPlugin BaseUnityPlugin;
        IPlugin Plugin;
        Type Type = typeof(IPlugin);
        foreach (PluginInfo PluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            BaseUnityPlugin = PluginInfo.Instance;
            if (BaseUnityPlugin is null)
                continue;

            if (!Type.IsAssignableFrom(BaseUnityPlugin.GetType()))
                continue;

            Plugin = (IPlugin)BaseUnityPlugin;

            if (Plugins.Add(Plugin))
                Plugin.LoadPlugin();
        }
    }
    public void ReloadPlugins()
    {
        OnBeforePluginsReload?.Invoke();

        foreach (IPlugin Plugin in Plugins)
            ReloadPlugin(Plugin);

        OnAfterPluginsReload?.Invoke();
    }
    public void ReloadPlugin(IPlugin Plugin)
    {
        if (Plugin.State != EState.Unloaded)
            Plugin.UnloadPlugin(EState.Unloaded);

        Plugin.LoadPlugin();
    }
}