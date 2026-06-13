using Steamworks;

namespace Tanuki.Atlyss.Game.Types.Player;

public sealed class Entry(global::Player player)
{
    internal CSteamID steamId = CSteamID.Nil;
    private readonly global::Player player = player;

    public CSteamID SteamId => steamId;
    public global::Player Player => player;
}
