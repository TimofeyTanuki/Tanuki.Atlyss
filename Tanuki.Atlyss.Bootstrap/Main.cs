using BepInEx;
using BepInEx.Configuration;
using Tanuki.Atlyss.Core.Plugins;

namespace Tanuki.Atlyss.Bootstrap;

[BepInPlugin("9c00d52e-10b8-413f-9ee4-bfde81762442", "Tanuki.Atlyss.Bootstrap", "1.0.0")]
[BepInProcess("ATLYSS.exe")]
public class Main : Plugin
{
    public Main() =>
        Core.Tanuki.Initialize();
    internal void Awake()
    {
        ConfigEntry<string> ConfigEntry = Config.Bind("Settings", "Language", "default", "Default language");
        Core.Settings.Language = ConfigEntry.Value;
    }
    internal void Start() =>
        Core.Tanuki.Instance.Reload();
}