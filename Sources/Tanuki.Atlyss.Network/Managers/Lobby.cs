using Steamworks;

namespace Tanuki.Atlyss.Network.Managers;

public class Lobby
{
    private CSteamID
        _LobbySteamID,
        _OwnerSteamID;

    public CSteamID LobbySteamID => _LobbySteamID;
    public CSteamID OwnerSteamID => _OwnerSteamID;

    internal Lobby()
    {
        Steam Steam = Tanuki.Instance.Steam;

        Steam.OnLobbyEntered += OnLobbyEntered;
        Steam.OnLobbyDataUpdate += OnLobbyDataUpdate;
    }

    private void RefreshLobbyOwner() =>
        _OwnerSteamID = SteamMatchmaking.GetLobbyOwner(_LobbySteamID);

    private void OnLobbyDataUpdate(LobbyDataUpdate_t LobbyDataUpdate) =>
        RefreshLobbyOwner();

    private void OnLobbyEntered(LobbyEnter_t LobbyEnter)
    {
        _LobbySteamID = new(LobbyEnter.m_ulSteamIDLobby);
        RefreshLobbyOwner();
    }
}