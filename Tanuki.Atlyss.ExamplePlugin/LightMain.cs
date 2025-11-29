using BepInEx;
using BepInEx.Logging;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.ExamplePlugin;

/*
 * If you want your own clean plugin (without built-in translation, but with commands), you can implement only the interface
 */
[BepInPlugin("653a2c21-7d84-4fbb-94bd-c30fac5a45e3", "Tanuki.Atlyss.ExamplePlugin", "1.0.0")]
[BepInDependency("9c00d52e-10b8-413f-9ee4-bfde81762442", BepInDependency.DependencyFlags.HardDependency)]
public class LightMain : BaseUnityPlugin, IPlugin
{
    internal static LightMain Instance;
    internal ManualLogSource ManualLogSource;

    public string Name => "Tanuki.Atlyss.ExamplePlugin";
    public event IPlugin.Load OnLoad;
    public event IPlugin.Loaded OnLoaded;
    public event IPlugin.Unload OnUnload;
    public event IPlugin.Unloaded OnUnloaded;
    public EState State => EState.Unloaded;

    internal void Awake()
    {
        Instance = this;
        ManualLogSource = Logger;
    }
    public void LoadPlugin()
    {
        OnLoad?.Invoke();
        // On load

        OnLoaded?.Invoke();
    }
    public void UnloadPlugin(EState PluginState)
    {
        OnUnload?.Invoke();
        // On unload

        OnUnloaded?.Invoke();
    }
}