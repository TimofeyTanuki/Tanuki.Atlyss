namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    public static Tanuki Instance = null!;

    public readonly Managers.Settings Settings = null!;
    //public readonly Managers.CommandsLegacy CommandsLegacy = null!;
    public readonly Managers.Commands Commands;
    public readonly Managers.Plugins Plugins = null!;
    public readonly Managers.Chat Chat = null!;

    internal Tanuki()
    {
        Settings = new();
        //CommandsLegacy = new();
        Commands = new();
        Plugins = new();
        Chat = new();
    }
}