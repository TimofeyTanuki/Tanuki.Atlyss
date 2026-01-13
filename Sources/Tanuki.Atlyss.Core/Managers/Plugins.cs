using BepInEx;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Managers;

public class Plugins
{
    private readonly Dictionary<Type, IPlugin> _PluginEntries = [];
    public IReadOnlyDictionary<Type, IPlugin> PluginEntries => _PluginEntries;

    public delegate void BeforePluginsLoad();
    public event BeforePluginsLoad? OnBeforePluginsLoad;

    internal void Refresh()
    {
        foreach (PluginInfo PluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            if (!PluginInfo.Instance)
                continue;

            Type PluginType = PluginInfo.Instance.GetType();

            if (_PluginEntries.ContainsKey(PluginType))
                continue;

            if (PluginInfo.Instance is not IPlugin Plugin)
                continue;

            _PluginEntries.Add(PluginType, Plugin);
        }
    }

    public void UnloadPlugins()
    {
        foreach (IPlugin Plugin in _PluginEntries.Values)
            UnloadPlugin(Plugin);
    }

    public void LoadPlugins()
    {
        OnBeforePluginsLoad?.Invoke();

        foreach (IPlugin Plugin in _PluginEntries.Values)
            LoadPlugin(Plugin);
    }

    public void ReloadPlugins()
    {
        UnloadPlugins();
        LoadPlugins();
    }

    public void UnloadPlugin(IPlugin Plugin)
    {
        if (Plugin.State != EState.Loaded)
            return;

        try
        {
            Plugin.UnloadPlugin(EState.Unloaded);
        }
        catch (Exception Exception)
        {
            Main.Instance.ManualLogSource.LogError($"Failed to unload plugin {Plugin.Name} ({Plugin.GetType().Assembly.GetName().Name}).\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
        }
    }

    public void LoadPlugin(IPlugin Plugin)
    {
        if (Plugin.State == EState.Loaded)
            return;

        try
        {
            Plugin.LoadPlugin();
        }
        catch (Exception Exception)
        {
            Main.Instance.ManualLogSource.LogError($"Failed to load plugin {Plugin.Name} ({Plugin.GetType().Assembly.GetName().Name}).\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
        }
    }

    public void ReloadPlugin(IPlugin Plugin)
    {
        UnloadPlugin(Plugin);
        LoadPlugin(Plugin);
    }
}