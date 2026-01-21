using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core;

internal sealed class Configuration
{
    public static Configuration Instance = null!;

    public Data.Configuration.Translations Language = null!;
    public Data.Configuration.Commands Commands = null!;
    public Data.Configuration.Network Network = null!;

    private Configuration() { }

    public static void Initialize() => Instance ??= new();

    public void Load(ConfigFile ConfigFile)
    {
        Language = new Data.Configuration.Translations(ConfigFile);
        Commands = new Data.Configuration.Commands(ConfigFile);
        Network = new Data.Configuration.Network(ConfigFile);
    }
}
