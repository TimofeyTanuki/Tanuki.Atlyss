namespace Tanuki.Atlyss.Core;

public class Tanuki
{
    public static Tanuki Instance = null!;

    public readonly Settings Settings = null!;
    public readonly Managers.CommandsLegacy CommandsLegacy = null!;
    public readonly Managers.Commands Commands;
    public readonly Managers.Plugins Plugins = null!;

    internal Tanuki()
    {
        Settings = new();
        CommandsLegacy = new();
        Commands = new();
        Plugins = new();
    }
}