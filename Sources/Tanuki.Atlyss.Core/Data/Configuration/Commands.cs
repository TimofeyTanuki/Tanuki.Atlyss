using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core.Data.Configuration;

internal sealed class Commands(ConfigFile configFile)
{
    private const string SECTION_NAME = "Commands";

    public ConfigEntry<string> ClientPrefix = configFile.Bind(SECTION_NAME, "ClientPrefix", Settings.Commands.CLIENTPREFIX_DEFAULT, "Local command prefix.");
    public ConfigEntry<string> ServerPrefix = configFile.Bind(SECTION_NAME, "ServerPrefix", Settings.Commands.SERVERPREFIX_DEFAULT, "Command prefix for server clients with missing commands.");
}