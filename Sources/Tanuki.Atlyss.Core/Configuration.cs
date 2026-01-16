using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core;

internal sealed class Configuration
{
    public static Configuration Instance = null!;

    public Data.Configuration.Language Language = null!;
    public Data.Configuration.Commands Commands = null!;

    private Configuration() { }

    public static void Initialize() => Instance ??= new();

    public void Load(ConfigFile ConfigFile)
    {
        Language = new Data.Configuration.Language(ConfigFile);
        Commands = new Data.Configuration.Commands(ConfigFile);
    }
}
