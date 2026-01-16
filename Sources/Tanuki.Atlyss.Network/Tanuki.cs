namespace Tanuki.Atlyss.Network;

public sealed class Tanuki
{
    private static Tanuki instance = null!;

    public static Tanuki Instance => instance;

    public readonly Managers.Steam Steam;
    public readonly Managers.Lobby Lobby;
    public readonly Managers.Network Network;

    internal Tanuki()
    {
        Steam = new();
        Lobby = new();
        Network = new();
    }

    public static void Initialize() => instance ??= new();
}