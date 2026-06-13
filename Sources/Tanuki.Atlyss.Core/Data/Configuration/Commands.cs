using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core.Data.Configuration;

internal sealed class Commands(ConfigFile configFile)
{
    private const string SECTION_NAME = "Commands";

    public ConfigEntry<string> ClientPrefix = configFile.Bind(SECTION_NAME, "ClientPrefix", Types.Settings.Commands.CLIENT_PREFIX_DEFAULT, "Local command prefix.");
    public ConfigEntry<string> ServerPrefix = configFile.Bind(SECTION_NAME, "ServerPrefix", Types.Settings.Commands.SERVER_PREFIX_DEFAULT, "Command prefix for server clients with missing commands.");
}
