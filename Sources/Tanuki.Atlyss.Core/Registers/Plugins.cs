using System;
using System.Collections.Generic;
using System.Reflection;
using Tanuki.Atlyss.API.Tanuki.Plugins;

namespace Tanuki.Atlyss.Core.Registers;

public sealed class Plugins
{
    private readonly Dictionary<Type, IPlugin> pluginInterfaces = [];
    private readonly Dictionary<Assembly, HashSet<Type>> assemblyPlugins = [];

    public Action<Type>? OnPluginRegistered;
    public Action<Type>? OnPluginDeregistered;

    /// <summary>
    /// Provides a lookup of <see cref="IPlugin"/> by their <see cref="Type"/>.
    /// </summary>
    public IReadOnlyDictionary<Type, IPlugin> PluginInterfaces => pluginInterfaces;

    /// <summary>
    /// Provides a <see cref="HashSet{T}"/> of plugins by their <see cref="Assembly"/>.
    /// </summary>
    /// <remarks>
    /// Modifying <see cref="HashSet{T}"/> values isn't recommended, as they're managed by register.
    /// </remarks>
    public IReadOnlyDictionary<Assembly, HashSet<Type>> AssemblyPlugins => assemblyPlugins;

    internal Plugins() { }

    internal void Refresh()
    {
        foreach (BepInEx.PluginInfo pluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            if (!pluginInfo.Instance)
                continue;

            Type pluginType = pluginInfo.Instance.GetType();

            if (pluginInfo.Instance is not IPlugin plugin)
                continue;

            RegisterPlugin(pluginType, plugin);
        }
    }

    public void RegisterPlugin(Type pluginType, IPlugin plugin)
    {
        if (pluginInterfaces.ContainsKey(pluginType))
            return;

        pluginInterfaces.Add(pluginType, plugin);

        Assembly PluginAssembly = pluginType.Assembly;

        if (!assemblyPlugins.TryGetValue(PluginAssembly, out HashSet<Type> AssemblyPlugins))
        {
            AssemblyPlugins = [];
            assemblyPlugins[PluginAssembly] = AssemblyPlugins;
        }

        AssemblyPlugins.Add(pluginType);

        OnPluginRegistered?.Invoke(pluginType);
    }

    public void DeregisterPlugin(Type pluginType)
    {
        if (!pluginInterfaces.Remove(pluginType))
            return;

        assemblyPlugins[pluginType.Assembly].Remove(pluginType);

        OnPluginDeregistered?.Invoke(pluginType);
    }
}
