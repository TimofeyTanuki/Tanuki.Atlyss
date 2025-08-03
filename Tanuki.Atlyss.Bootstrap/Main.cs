using BepInEx;
using BepInEx.Configuration;
using Tanuki.Atlyss.Core.Plugins;

namespace Tanuki.Atlyss.Bootstrap;

[BepInPlugin("9c00d52e-10b8-413f-9ee4-bfde81762442", "Tanuki.Atlyss.Bootstrap", "1.0.0")]
[BepInProcess("ATLYSS.exe")]
public class Main : Plugin
{
    internal static Main Instance;
    public Main() =>
        Core.Tanuki.Initialize();
    internal void Awake()
    {
        Instance = this;

        ConfigEntry<string> ConfigEntry = Config.Bind("Settings", "Language", "default", "Default language");
        Core.Tanuki.Instance.Settings.Language = ConfigEntry.Value;
    }
    internal void Start() =>
        Core.Tanuki.Instance.Reload();
}