using BepInEx;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Plugins;

public class Manager
{
    public delegate void BeforePluginsReload();
    public event BeforePluginsReload? OnBeforePluginsReload;

    public delegate void AfterPluginsReload();
    public event AfterPluginsReload? OnAfterPluginsReload;

    public delegate void BeforePluginsLoad();
    public event BeforePluginsLoad? OnBeforePluginsLoad;

    public delegate void AfterPluginsLoad();
    public event AfterPluginsLoad? OnAfterPluginsLoad;

    public readonly HashSet<IPlugin> Plugins = [];
    internal void LoadPlugins()
    {
        if (Plugins.Count > 0)
            return;

        BaseUnityPlugin BaseUnityPlugin;
        Type Type = typeof(IPlugin);
        foreach (PluginInfo PluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            BaseUnityPlugin = PluginInfo.Instance;
            if (BaseUnityPlugin is null)
                continue;

            if (!Type.IsAssignableFrom(BaseUnityPlugin.GetType()))
                continue;

            Plugins.Add((IPlugin)BaseUnityPlugin);
        }

        OnBeforePluginsLoad?.Invoke();

        foreach (IPlugin Plugin in Plugins)
            Plugin.LoadPlugin();

        OnAfterPluginsLoad?.Invoke();
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