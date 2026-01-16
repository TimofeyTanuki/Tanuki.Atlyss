using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core.Data.Configuration;

internal sealed class Language(ConfigFile configFile)
{
    private const string SECTION_NAME = "translations";

    public ConfigEntry<string> PreferredLanguages = configFile.Bind(SECTION_NAME, "PreferredLanguages", "neutral, english, russian", "List of preferred languages in order of priority.");
}
