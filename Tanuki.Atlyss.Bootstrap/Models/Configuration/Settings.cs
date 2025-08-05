using BepInEx.Configuration;

namespace Tanuki.Atlyss.Bootstrap.Models.Configuration;

internal class Settings(ConfigFile ConfigFile)
{
    private const string Section = "Settings";
    public ConfigEntry<string> Language = ConfigFile.Bind(Section, "Language", "default");
}