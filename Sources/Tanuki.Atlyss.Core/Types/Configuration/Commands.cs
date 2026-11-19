using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core.Types.Configuration;

internal sealed class Commands(ConfigFile configFile)
{
    private const string SECTION_NAME = "Commands";

    public ConfigEntry<string> Prefix = configFile.Bind(SECTION_NAME, "Prefix", Settings.Commands.DEFAULT_PREFIX, "Command prefix.");
}
