using Steamworks;
using System;

namespace Tanuki.Atlyss.Network.Providers;

public sealed class Steam
{
    public event Action<SteamRelayNetworkStatus_t>? OnSteamRelayNetworkStatus;
    public event Action<SteamNetworkingMessagesSessionRequest_t>? OnSteamNetworkingMessagesSessionRequest;
    public event Action<GameLobbyJoinRequested_t>? OnGameLobbyJoinRequested;
    public event Action<LobbyEnter_t>? OnLobbyEntered;
    public event Action<LobbyChatMsg_t>? OnLobbyChatMsg;
    public event Action<LobbyDataUpdate_t>? OnLobbyDataUpdate;

    private Callback<SteamRelayNetworkStatus_t> steamRelayNetworkStatusCallback = null!;
    private Callback<SteamNetworkingMessagesSessionRequest_t> steamNetworkingMessagesSessionRequest = null!;
    private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested = null!;
    private Callback<LobbyEnter_t> lobbyEnter = null!;
    private Callback<LobbyChatMsg_t> lobbyChatMsg = null!;
    private Callback<LobbyDataUpdate_t> lobbyDataUpdate = null!;

    private bool initialized = false;

    internal Steam() { }

    public void Initialize()
    {
        if (!SteamManager.Initialized && !initialized)
            return;

        steamRelayNetworkStatusCallback = Callback<SteamRelayNetworkStatus_t>.Create(SteamRelayNetworkStatus);
        steamNetworkingMessagesSessionRequest = Callback<SteamNetworkingMessagesSessionRequest_t>.Create(SteamNetworkingMessagesSessionRequest);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(JoinRequest));
        lobbyEnter = Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(LobbyEntered));
        lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(LobbyChatMessage);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(LobbyDataUpdateCallback);

        initialized = true;
    }

    private void LobbyDataUpdateCallback(LobbyDataUpdate_t LobbyDataUpdate) =>
        OnLobbyDataUpdate?.Invoke(LobbyDataUpdate);

    public void DisposeCallbacks()
    {
        if (!initialized)
            return;

        steamRelayNetworkStatusCallback.Dispose();
        steamNetworkingMessagesSessionRequest.Dispose();
        gameLobbyJoinRequested.Dispose();
        lobbyEnter.Dispose();
        lobbyChatMsg.Dispose();
        lobbyDataUpdate.Dispose();

        initialized = false;
    }

    private void SteamRelayNetworkStatus(SteamRelayNetworkStatus_t steamRelayNetworkStatus) =>
        OnSteamRelayNetworkStatus?.Invoke(steamRelayNetworkStatus);

    private void LobbyEntered(LobbyEnter_t lobbyEnter) =>
        OnLobbyEntered?.Invoke(lobbyEnter);

    private void JoinRequest(GameLobbyJoinRequested_t gameLobbyJoinRequested) =>
        OnGameLobbyJoinRequested?.Invoke(gameLobbyJoinRequested);

    private void LobbyChatMessage(LobbyChatMsg_t lobbyChatMsg) =>
        OnLobbyChatMsg?.Invoke(lobbyChatMsg);

    private void SteamNetworkingMessagesSessionRequest(SteamNetworkingMessagesSessionRequest_t steamNetworkingMessagesSessionRequest) =>
        OnSteamNetworkingMessagesSessionRequest?.Invoke(steamNetworkingMessagesSessionRequest);
}
