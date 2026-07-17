using Steamworks;

namespace Tanuki.Atlyss.Game.Types.Player;

public sealed class Entry(global::Player player)
{
    private CSteamID steamId = CSteamID.Nil;
    private readonly global::Player player = player;

    public CSteamID SteamId
    {
        get => steamId;
        internal set => steamId = value;
    }
    public global::Player Player => player;
}
