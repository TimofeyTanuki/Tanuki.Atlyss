using System;

namespace Tanuki.Atlyss.API.Plugins;

public interface IPlugin
{
    public event Action? OnLoad;
    public event Action? OnLoaded;
    public event Action? OnUnload;
    public event Action? OnUnloaded;

    string Name { get; }
    EState State { get; }

    void LoadPlugin();

    void UnloadPlugin(EState PluginState);
}