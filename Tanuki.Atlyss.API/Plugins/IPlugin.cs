using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.API;

public interface IPlugin
{
    public delegate void Load();
    public event Load OnLoad;

    public delegate void Loaded();
    public event Loaded OnLoaded;

    public delegate void Unload();
    public event Unload OnUnload;

    public delegate void Unloaded();
    public event Unloaded OnUnloaded;

    string Name { get; }
    EState State { get; }

    void LoadPlugin();
    void UnloadPlugin(EState PluginState);
}