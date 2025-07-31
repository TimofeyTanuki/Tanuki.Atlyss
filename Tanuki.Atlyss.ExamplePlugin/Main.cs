using BepInEx;

namespace Tanuki.Atlyss.ExamplePlugin;

[BepInPlugin("653a2c21-7d84-4fbb-94bd-c30fac5a45e3", "Tanuki.Atlyss.ExamplePlugin", "1.0.0.0")]
[BepInProcess("ATLYSS.exe")]
public class Main : Core.Plugins.Plugin
{
    internal static Main Instance;
    public void Awake()
    {
        Instance = this;
        Logger.LogInfo("Awake()");

        // You can change Settings here
        // Settings.Language = "your-language-from-cfg"; // By default, the variable of Tanuki.Atlyss.Bootloader configuration is set.
    }
    protected override void Load()
    {
        Logger.LogInfo("Load()");

        Logger.LogInfo($"Translation test: {Translate("Debug")}");
    }
    protected override void Unload()
    {
        Logger.LogInfo("Unload()");
    }
}