using System;
using Tanuki.Atlyss.API.Tanuki.Plugins;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Plugins
{
    private readonly Registers.Plugins pluginRegistry;

    public event Action? OnBeforePluginsLoad;

    internal Plugins(Registers.Plugins pluginRegistry) =>
        this.pluginRegistry = pluginRegistry;

    internal void Refresh()
    {
        foreach (BepInEx.PluginInfo pluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            if (!pluginInfo.Instance)
                continue;

            Type pluginType = pluginInfo.Instance.GetType();

            if (pluginInfo.Instance is not IPlugin plugin)
                continue;

            pluginRegistry.RegisterPlugin(pluginType, plugin);
        }
    }

    public void UnloadPlugins()
    {
        foreach (IPlugin plugin in pluginRegistry.PluginInterfaces.Values)
            UnloadPlugin(plugin);
    }

    public void LoadPlugins()
    {
        OnBeforePluginsLoad?.Invoke();

        foreach (IPlugin plugin in pluginRegistry.PluginInterfaces.Values)
            LoadPlugin(plugin);
    }

    public void ReloadPlugins()
    {
        UnloadPlugins();
        LoadPlugins();
    }

    public void UnloadPlugin(IPlugin plugin)
    {
        if (plugin.State != EState.Loaded)
            return;

        try
        {
            plugin.UnloadPlugin(EState.Unloaded);
        }
        catch (Exception exception)
        {

            Main.Instance.ManualLogSource.LogError($"Failed to unload plugin {plugin.Name} ({plugin.GetType().Assembly.GetName().Name}).\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
        }
    }

    public void LoadPlugin(IPlugin plugin)
    {
        if (plugin.State == EState.Loaded)
            return;

        try
        {
            plugin.LoadPlugin();
        }
        catch (Exception exception)
        {
            Main.Instance.ManualLogSource.LogError($"Failed to load plugin {plugin.Name} ({plugin.GetType().Assembly.GetName().Name}).\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
        }
    }

    public void ReloadPlugin(IPlugin plugin)
    {
        UnloadPlugin(plugin);
        LoadPlugin(plugin);
    }
}
