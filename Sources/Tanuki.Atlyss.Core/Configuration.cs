using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core;

internal class Configuration
{
    public static Configuration Instance = null!;
    public Models.Configuration.Settings General = null!;

    private Configuration() { }

    public static void Initialize() =>
        Instance ??= new();

    public void Load(ConfigFile ConfigFile) =>
        General = new Models.Configuration.Settings(ref ConfigFile);
}
