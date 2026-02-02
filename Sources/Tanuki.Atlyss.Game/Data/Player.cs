using Steamworks;

namespace Tanuki.Atlyss.Game.Data;

public sealed class PlayerEntry(Player player)
{
    internal CSteamID steamId = CSteamID.Nil;
    private readonly Player player = player;

    public CSteamID SteamId => steamId;
    public Player Player => player;
}
