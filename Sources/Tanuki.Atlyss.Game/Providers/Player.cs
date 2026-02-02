using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.Game.Data;
using Tanuki.Atlyss.Game.Utilities.Player;

namespace Tanuki.Atlyss.Game.Providers;

public sealed class Player
{
    public static Player Instance { get; internal set; } = null!;

    public readonly Dictionary<uint, PlayerEntry> playerEntries = [];
    private readonly Dictionary<ulong, uint> steamIdMap = [];

    private readonly HashSet<uint> loadingPlayers = [];

    private bool playerObservationState = false;
    private bool lastNetworkServerState = false;

    public static event Action<global::Player>? OnPlayerAdded;
    public static event Action<global::Player>? OnPlayerLoaded;
    public static event Action<global::Player>? OnPlayerRemoved;

    public IReadOnlyDictionary<uint, PlayerEntry> Players => playerEntries;
    public IReadOnlyDictionary<ulong, uint> SteamIdMap => steamIdMap;
    public IReadOnlyCollection<uint> LoadingPlayers => loadingPlayers;

    private Player()
    {
        Patches.AtlyssNetworkManager.OnStopClient.OnPrefix += OnAtlyssNetworkManagerStop;
        Patches.NetworkBehaviour.OnStartClient.OnPostfix += OnNetworkBehaviourStartClient;
        Patches.NetworkBehaviour.OnStopClient.OnPrefix += OnNetworkBehaviourStopClient;
    }

    private void BeginPlayerInitialization(uint netId)
    {
        loadingPlayers.Add(netId);
        StartPlayerObservations();
    }

    private void FinalizePlayerInitialization(uint netId)
    {
        loadingPlayers.Remove(netId);
        FinalizePlayerObservations();
    }

    private void StartPlayerObservations()
    {
        if (playerObservationState)
            return;

        playerObservationState = true;
        lastNetworkServerState = NetworkServer.active;

        if (lastNetworkServerState)
        {
            Patches.Player.Network_steamID_Setter.OnPostfix += OnPlayerSteamIdChange;
            Patches.Player.OnGameConditionChange.OnPostfix += OnPlayerGameConditionChange;
        }
        else
            Patches.Player.DeserializeSyncVars.OnPostfix += OnPlayerDeserializeSyncVars;
    }

    private void FinalizePlayerObservations()
    {
        if (!playerObservationState || loadingPlayers.Count > 0)
            return;

        playerObservationState = false;

        if (lastNetworkServerState)
        {
            Patches.Player.Network_steamID_Setter.OnPostfix -= OnPlayerSteamIdChange;
            Patches.Player.OnGameConditionChange.OnPostfix -= OnPlayerGameConditionChange;
        }
        else
            Patches.Player.DeserializeSyncVars.OnPostfix -= OnPlayerDeserializeSyncVars;
    }

    private void OnPlayerSteamIdChange(global::Player player) =>
        HandleLoadingPlayer(player);

    private void OnPlayerGameConditionChange(global::Player player, GameCondition oldCondition, GameCondition newCondition) =>
        HandleLoadingPlayer(player);

    private void OnPlayerDeserializeSyncVars(global::Player player, NetworkReader networkReader, bool initialState, int originalPosition) =>
        HandleLoadingPlayer(player);

    private void HandleLoadingPlayer(global::Player player)
    {
        if (player._currentGameCondition != GameCondition.IN_GAME)
            return;

        uint netId = player.netId;

        if (!loadingPlayers.Contains(netId))
            return;

        if (!ulong.TryParse(player._steamID, out ulong steamId))
            return;

        FinalizePlayerInitialization(netId);

        playerEntries[netId].steamId = new CSteamID(steamId);
        steamIdMap[steamId] = netId;

        OnPlayerLoaded?.Invoke(player);
    }

    public static void Initialize() => Instance ??= new();

    private void OnAtlyssNetworkManagerStop()
    {
        loadingPlayers.Clear();
        playerEntries.Clear();
        steamIdMap.Clear();

        FinalizePlayerObservations();
    }

    private void OnNetworkBehaviourStartClient(NetworkBehaviour instance)
    {
        if (instance is not global::Player player)
            return;

        if (!player)
            return;

        uint netId = player.netId;
        PlayerEntry playerEntry = new(player);

        playerEntries.Add(netId, playerEntry);

        OnPlayerAdded?.Invoke(player);

        BeginPlayerInitialization(netId);

        if (player._isHostPlayer || player.isLocalPlayer)
            HandleLoadingPlayer(player);
    }

    private void OnNetworkBehaviourStopClient(NetworkBehaviour instance)
    {
        if (instance is not global::Player player)
            return;

        uint netId = instance.netId;

        if (!playerEntries.TryGetValue(netId, out PlayerEntry playerEntry))
            return;

        playerEntries.Remove(netId);
        steamIdMap.Remove(playerEntry.steamId.m_SteamID);

        FinalizePlayerInitialization(netId);

        OnPlayerRemoved?.Invoke(player);
    }

    public global::Player? FindByNetId(uint netId)
    {
        if (playerEntries.TryGetValue(netId, out PlayerEntry playerEntry))
            return playerEntry.Player;

        return null;
    }

    public global::Player? FindBySteamId(ulong steamId)
    {
        if (steamIdMap.TryGetValue(steamId, out uint netId) &&
            playerEntries.TryGetValue(netId, out PlayerEntry playerEntry))
            return playerEntry.Player;

        return null;
    }

    public global::Player? FindBySteamId(CSteamID steamId) => FindBySteamId(steamId.m_SteamID);

    public global::Player? FindByDefaultNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (PlayerEntry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.Player;

            if (Nickname.Match(player._nickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }

    public global::Player? FindByGlobalNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (PlayerEntry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.Player;

            if (Nickname.Match(player._globalNickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }

    public global::Player? FindByAnyNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (PlayerEntry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.Player;

            if (Nickname.Match(player._nickname, nickname, strictLength, stringComparsion) ||
                Nickname.Match(player._globalNickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }
}
