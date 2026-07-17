using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core;

internal sealed class Configuration
{
    public static Configuration Instance = null!;

    public Types.Configuration.Translations Language = null!;
    public Types.Configuration.Commands Commands = null!;
    public Types.Configuration.Network Network = null!;

    private Configuration() { }

    public static void Initialize() => Instance ??= new();

    public void Load(ConfigFile ConfigFile)
    {
        Language = new Types.Configuration.Translations(ConfigFile);
        Commands = new Types.Configuration.Commands(ConfigFile);
        Network = new Types.Configuration.Network(ConfigFile);
    }
}
