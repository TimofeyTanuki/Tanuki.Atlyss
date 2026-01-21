using Steamworks;

namespace Tanuki.Atlyss.Network.Providers;

public sealed class SteamLobby
{
    private readonly Steam steamProvider;

    private CSteamID lobbyOwner;
    private CSteamID lobby;

    public CSteamID LobbyOwner => lobbyOwner;
    public CSteamID Lobby => lobby;

    internal SteamLobby(Steam steamProvider)
    {
        this.steamProvider = steamProvider;
        this.steamProvider.OnLobbyEnter += SteamProvider_OnLobbyEnter;
        Game.Patches.SteamLobby.Reset_LobbyQueueParams.OnPostfix += Reset_LobbyQueueParams_OnPostfix;
    }

    private void SteamProvider_OnLobbyEnter(LobbyEnter_t lobbyEnter)
    {
        lobby = new(lobbyEnter.m_ulSteamIDLobby);
        lobbyOwner = SteamMatchmaking.GetLobbyOwner(lobby);
    }

    private void Reset_LobbyQueueParams_OnPostfix()
    {
        lobby = lobbyOwner = CSteamID.Nil;
    }
}
