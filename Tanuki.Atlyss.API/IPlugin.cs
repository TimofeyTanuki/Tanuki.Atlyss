using Tanuki.Atlyss.API.Collections;

namespace Tanuki.Atlyss.API;

public interface IPlugin
{
    string Name { get; }
    EPluginState State { get; }
    Translation Translation { get; }

    void LoadPlugin();
    void UnloadPlugin(EPluginState PluginState);
}