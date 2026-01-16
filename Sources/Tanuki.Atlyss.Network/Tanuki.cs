namespace Tanuki.Atlyss.Network;

public sealed class Tanuki
{
    public static Tanuki Instance = null!;

    public readonly Managers.Steam Steam;
    public readonly Managers.Lobby Lobby;
    public readonly Managers.Network Network;

    internal Tanuki()
    {
        Steam = new();
        Lobby = new();
        Network = new();
    }

    public static void Initialize() => Instance ??= new();
}