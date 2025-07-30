using BepInEx;
using BepInEx.Logging;

namespace Tanuki.Atlyss.ExamplePlugin;

[BepInPlugin("653a2c21-7d84-4fbb-94bd-c30fac5a45e3", "Tanuki.Atlyss.ExamplePlugin", "1.0.0.0")]
[BepInProcess("ATLYSS.exe")]
public class Main : Core.Plugin
{
    internal static Main Instance;
    internal new ManualLogSource Logger;
    public void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        base.Logger.LogInfo("Awake()");
    }
    protected override void Load()
    {
        base.Logger.LogInfo("Load()");

        base.Logger.LogInfo($"Translate: {Translate("Debug")}");
    }
    protected override void Unload()
    {
        base.Logger.LogInfo("Unload()");
    }
}