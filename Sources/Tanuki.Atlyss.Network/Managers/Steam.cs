using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Tanuki.Atlyss.Network.Managers;

public sealed class Steam
{
    public event Action<SteamRelayNetworkStatus_t>? OnSteamRelayNetworkStatus;
    public event Action<SteamNetworkingMessagesSessionRequest_t>? OnSteamNetworkingMessagesSessionRequest;
    public event Action<GameLobbyJoinRequested_t>? OnGameLobbyJoinRequested;
    public event Action<LobbyEnter_t>? OnLobbyEntered;
    public event Action<LobbyChatMsg_t>? OnLobbyChatMsg;
    public event Action<LobbyDataUpdate_t>? OnLobbyDataUpdate;

    private static Callback<SteamRelayNetworkStatus_t> _SteamRelayNetworkStatusCallback = null!;
    private static Callback<SteamNetworkingMessagesSessionRequest_t> _SteamNetworkingMessagesSessionRequest = null!;
    private static Callback<GameLobbyJoinRequested_t> _GameLobbyJoinRequested = null!;
    private static Callback<LobbyEnter_t> _LobbyEnter = null!;
    private static Callback<LobbyChatMsg_t> _LobbyChatMsg = null!;
    private static Callback<LobbyDataUpdate_t> _LobbyDataUpdate = null!;

    private bool CallbacksStatus = false;

    internal Steam() { }

    public void CreateCallbacks()
    {
        if (!SteamManager.Initialized && !CallbacksStatus)
            return;

        _SteamRelayNetworkStatusCallback = Callback<SteamRelayNetworkStatus_t>.Create(SteamRelayNetworkStatusCallback);
        _SteamNetworkingMessagesSessionRequest = Callback<SteamNetworkingMessagesSessionRequest_t>.Create(SteamNetworkingMessagesSessionRequestCallback);
        _GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(JoinRequestCallback));
        _LobbyEnter = Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(LobbyEnteredCallback));
        _LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(LobbyChatMessageCallback);
        _LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(LobbyDataUpdateCallback);

        CallbacksStatus = true;
    }

    private void LobbyDataUpdateCallback(LobbyDataUpdate_t LobbyDataUpdate) =>
        OnLobbyDataUpdate?.Invoke(LobbyDataUpdate);

    public void DisposeCallbacks()
    {
        if (!CallbacksStatus)
            return;

        _SteamRelayNetworkStatusCallback.Dispose();
        _SteamNetworkingMessagesSessionRequest.Dispose();
        _GameLobbyJoinRequested.Dispose();
        _LobbyEnter.Dispose();
        _LobbyChatMsg.Dispose();
        _LobbyDataUpdate.Dispose();

        CallbacksStatus = false;
    }

    private void SteamRelayNetworkStatusCallback(SteamRelayNetworkStatus_t SteamRelayNetworkStatus) =>
        OnSteamRelayNetworkStatus?.Invoke(SteamRelayNetworkStatus);

    private void LobbyEnteredCallback(LobbyEnter_t LobbyEnter) =>
        OnLobbyEntered?.Invoke(LobbyEnter);

    private void JoinRequestCallback(GameLobbyJoinRequested_t GameLobbyJoinRequested) =>
        OnGameLobbyJoinRequested?.Invoke(GameLobbyJoinRequested);

    private void LobbyChatMessageCallback(LobbyChatMsg_t LobbyChatMsg) =>
        OnLobbyChatMsg?.Invoke(LobbyChatMsg);

    private void SteamNetworkingMessagesSessionRequestCallback(SteamNetworkingMessagesSessionRequest_t SteamNetworkingMessagesSessionRequest) =>
        OnSteamNetworkingMessagesSessionRequest?.Invoke(SteamNetworkingMessagesSessionRequest);
}