using BepInEx.Configuration;
using Tanuki.Atlyss.Bootstrap.Models.Configuration;

namespace Tanuki.Atlyss.Bootstrap;

internal class Configuration
{
    public static Configuration Instance;
    public Settings Settings;

    private Configuration() { }

    public static void Initialize() =>
        Instance ??= new();

    public void Load(ConfigFile ConfigFile) =>
        Settings = new Settings(ref ConfigFile);
}