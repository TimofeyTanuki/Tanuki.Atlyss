using Steamworks;
using System;

namespace Tanuki.Atlyss.Network.Providers;

public sealed class SteamLobby
{
    private readonly Steam steamProvider;

    public Action<CSteamID>? OnLobbyChanged;

    private CSteamID steamId;
    private CSteamID ownerSteamId;

    public CSteamID SteamId => steamId;
    public CSteamID OwnerSteamId => ownerSteamId;

    internal SteamLobby(Steam steamProvider)
    {
        this.steamProvider = steamProvider;
        this.steamProvider.OnLobbyEnter += SteamProvider_OnLobbyEnter;
        Game.Patches.SteamLobby.Reset_LobbyQueueParams.OnPostfix += Reset_LobbyQueueParams_OnPostfix;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPostfix += OnStopClient_OnPostfix;
    }

    private void UpdateLobby(CSteamID newLobby)
    {
        if (ownerSteamId.Equals(newLobby))
            return;

        ownerSteamId = newLobby;
        steamId = ownerSteamId.Equals(CSteamID.Nil) ? CSteamID.Nil : SteamMatchmaking.GetLobbyOwner(ownerSteamId);

        OnLobbyChanged?.Invoke(ownerSteamId);
    }

    private void OnStopClient_OnPostfix() => UpdateLobby(CSteamID.Nil);

    private void SteamProvider_OnLobbyEnter(LobbyEnter_t lobbyEnter) => UpdateLobby(new(lobbyEnter.m_ulSteamIDLobby));

    private void Reset_LobbyQueueParams_OnPostfix()
    {
        Console.WriteLine("Reset_LobbyQueueParams_OnPostfix");
        UpdateLobby(CSteamID.Nil);
    }
}
