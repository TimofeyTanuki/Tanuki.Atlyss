using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core.Models.Configuration;

internal class Settings(ref ConfigFile ConfigFile)
{
    private const string Section = "Settings";

    public ConfigEntry<string> PreferredLanguages = ConfigFile.Bind(Section, "PreferredLanguages", "neutral, english, russian", "List of preferred languages by priority.");
}
