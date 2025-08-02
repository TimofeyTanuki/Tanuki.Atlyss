using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.API;

public interface IPlugin
{
    string Name { get; }
    EState State { get; }

    void LoadPlugin();
    void UnloadPlugin(EState PluginState);
}