using Steamworks;

namespace Tanuki.Atlyss.Game.Data;

public sealed class PlayerEntry(Player player)
{
    internal CSteamID steamId = CSteamID.NotInitYetGS;
    internal Player player = player;

    public CSteamID SteamId => steamId;
    public Player Player => player;
}
