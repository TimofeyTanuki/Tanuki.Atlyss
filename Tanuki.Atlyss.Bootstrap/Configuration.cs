using BepInEx.Configuration;
using Tanuki.Atlyss.Bootstrap.Models.Configuration;

namespace Tanuki.Atlyss.Bootstrap;

internal class Configuration
{
    public static Configuration Instance;

    private Configuration() { }
    public static void Initialize()
    {
        Instance ??= new();
    }

    public Settings Settings;

    public void Load(ConfigFile ConfigFile) =>
        Settings = new Settings(ConfigFile);
}