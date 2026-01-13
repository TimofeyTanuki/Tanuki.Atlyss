using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core.Models.Configuration;

internal class Settings(ref ConfigFile ConfigFile)
{
    private const string Section = "General";

    public ConfigEntry<string> Language = ConfigFile.Bind(Section, "Language", "default");
}
