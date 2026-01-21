using Steamworks;
using System;

namespace Tanuki.Atlyss.Network.Providers;

public sealed class Steam
{
    private bool callbackCreated = false;

    public event Action<SteamRelayNetworkStatus_t>? OnSteamRelayNetworkStatus;
    public event Action<SteamNetworkingMessagesSessionRequest_t>? OnSteamNetworkingMessagesSessionRequest;
    public event Action<GameLobbyJoinRequested_t>? OnGameLobbyJoinRequested;
    public event Action<LobbyEnter_t>? OnLobbyEnter;
    public event Action<LobbyChatMsg_t>? OnLobbyChatMsg;
    public event Action<LobbyDataUpdate_t>? OnLobbyDataUpdate;
    public event Action<LobbyChatUpdate_t>? OnLobbyChatUpdate;

    private static Callback<SteamRelayNetworkStatus_t> steamRelayNetworkStatus = null!;
    private static Callback<SteamNetworkingMessagesSessionRequest_t> steamNetworkingMessagesSessionRequest = null!;
    private static Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested = null!;
    private static Callback<LobbyEnter_t> lobbyEnter = null!;
    private static Callback<LobbyChatMsg_t> lobbyChatMsg = null!;
    private static Callback<LobbyDataUpdate_t> lobbyDataUpdate = null!;
    private static Callback<LobbyChatUpdate_t> lobbyChatUpdate = null!;

    internal Steam() { }

    private void SteamRelayNetworkStatus(SteamRelayNetworkStatus_t steamRelayNetworkStatus) =>
        OnSteamRelayNetworkStatus?.Invoke(steamRelayNetworkStatus);

    private void SteamNetworkingMessagesSessionRequest(SteamNetworkingMessagesSessionRequest_t steamNetworkingMessagesSessionRequest) =>
        OnSteamNetworkingMessagesSessionRequest?.Invoke(steamNetworkingMessagesSessionRequest);

    private void GameLobbyJoinRequested(GameLobbyJoinRequested_t gameLobbyJoinRequested) =>
        OnGameLobbyJoinRequested?.Invoke(gameLobbyJoinRequested);

    private void LobbyEntered(LobbyEnter_t lobbyEnter) =>
        OnLobbyEnter?.Invoke(lobbyEnter);

    private void LobbyChatMessage(LobbyChatMsg_t lobbyChatMsg) =>
        OnLobbyChatMsg?.Invoke(lobbyChatMsg);

    private void LobbyDataUpdate(LobbyDataUpdate_t LobbyDataUpdate) =>
        OnLobbyDataUpdate?.Invoke(LobbyDataUpdate);

    private void LobbyChatUpdate(LobbyChatUpdate_t LobbyChatUpdate) =>
        OnLobbyChatUpdate?.Invoke(LobbyChatUpdate);

    public void CreateCallbacks()
    {
        if (callbackCreated)
            return;

        steamRelayNetworkStatus = Callback<SteamRelayNetworkStatus_t>.Create(SteamRelayNetworkStatus);
        steamNetworkingMessagesSessionRequest = Callback<SteamNetworkingMessagesSessionRequest_t>.Create(SteamNetworkingMessagesSessionRequest);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(GameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(LobbyEntered);
        lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(LobbyChatMessage);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(LobbyDataUpdate);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(LobbyChatUpdate);

        callbackCreated = true;
    }

    public void DisposeCallbacks()
    {
        if (!callbackCreated)
            return;

        steamRelayNetworkStatus.Dispose();
        steamNetworkingMessagesSessionRequest.Dispose();
        gameLobbyJoinRequested.Dispose();
        lobbyEnter.Dispose();
        lobbyChatMsg.Dispose();
        lobbyDataUpdate.Dispose();
        lobbyChatUpdate.Dispose();

        callbackCreated = false;
    }
}
