namespace Tanuki.Atlyss.Network.Data.Tanuki;

public sealed class Providers
{
    internal Network.Providers.Steam steam = null!;
    internal Network.Providers.SteamLobby steamLobby = null!;
    internal Network.Providers.Packets packet = null!;

    public Network.Providers.Steam Steam => steam;
    public Network.Providers.SteamLobby SteamLobby => steamLobby;
    public Network.Providers.Packets Packet => packet;

    internal Providers() { }
}
