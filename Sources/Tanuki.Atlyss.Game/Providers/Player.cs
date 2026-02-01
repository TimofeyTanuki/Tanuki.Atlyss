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
    private readonly SortedSet<uint> initializingPlayers = [];

    public static event Action<global::Player>? OnPlayerAdded;
    public static event Action<global::Player>? OnPlayerRemoved;
    public static event Action<global::Player>? OnPlayerInitialized;

    public IReadOnlyDictionary<uint, PlayerEntry> PlayerEntries => playerEntries;

    private Player()
    {
        Patches.NetworkBehaviour.OnStartClient.OnPostfix += OnNetworkBehaviourStartClientPostfix;
        Patches.NetworkBehaviour.OnStopClient.OnPrefix += OnNetworkBehaviourStopClientPostfix;
        Patches.AtlyssNetworkManager.OnStopClient.OnPrefix += OnAtlyssNetworkManagerStopClientPrefix;
        Patches.Player.DeserializeSyncVars.OnPostfix += OnPlayerDeserializeSyncVarsPostfix;
    }

    public static void Initialize() => Instance ??= new();

    private void BeginPlayerInitialization(global::Player player)
    {
        /*
        if (player.)
        {
            FinalizePlayerInitialization(player);
            return;
        }
        */
        initializingPlayers.Add(player.netId);
    }

    private void HandlePlayerInitialization(global::Player player)
    {
        if (player._currentGameCondition == GameCondition.LOADING_GAME)
            return;

        if (!initializingPlayers.Remove(player.netId))
            return;

        FinalizePlayerInitialization(player);
    }

    private void FinalizePlayerInitialization(global::Player player)
    {
        Console.WriteLine($"FinalizePlayerInitialization");
        if (ulong.TryParse(player._steamID, out ulong steamId))
            playerEntries[player.netId].steamId = new(steamId);

        Console.WriteLine($"NET: {player.netId} - {playerEntries[player.netId].steamId} - {player._nickname}");

        OnPlayerInitialized?.Invoke(player);
    }

    private void OnPlayerDeserializeSyncVarsPostfix(global::Player player, NetworkReader networkReader, bool initialState, int originalPosition)
    {
        if (initializingPlayers.Count == 0)
            return;

        HandlePlayerInitialization(player);
    }

    private void OnAtlyssNetworkManagerStopClientPrefix()
    {
        initializingPlayers.Clear();
        playerEntries.Clear();
    }

    private void OnNetworkBehaviourStartClientPostfix(NetworkBehaviour instance)
    {
        if (instance is not global::Player player)
            return;

        if (!instance)
            return;

        if (string.IsNullOrEmpty(player._steamID))
            return;

        if (playerEntries.ContainsKey(player.netId))
        {
            playerEntries[player.netId].player = player;
            return;
        }

        playerEntries.Add(player.netId, new(player));

        BeginPlayerInitialization(player);

        OnPlayerAdded?.Invoke(player);
    }

    private void OnNetworkBehaviourStopClientPostfix(NetworkBehaviour instance)
    {
        if (instance is not global::Player player)
            return;

        if (!playerEntries.ContainsKey(instance.netId))
            return;

        playerEntries.Remove(instance.netId);
        initializingPlayers.Remove(instance.netId);

        OnPlayerRemoved?.Invoke(player);
    }

    public global::Player? FindByNetID(uint netId)
    {
        if (playerEntries.TryGetValue(netId, out PlayerEntry playerEntry))
            return playerEntry.player;

        return null;
    }

    public global::Player? FindBySteamId(ulong steamId)
    {
        foreach (PlayerEntry playerEntry in playerEntries.Values)
            if (playerEntry.steamId.m_SteamID == steamId)
                return playerEntry.player;

        return null;
    }

    public global::Player? FindBySteamId(CSteamID steamId) => FindBySteamId(steamId.m_SteamID);

    public global::Player? FindByDefaultNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (PlayerEntry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.player;

            if (Nickname.Match(player._nickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }

    public global::Player? FindByGlobalNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (PlayerEntry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.player;

            if (Nickname.Match(player._globalNickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }

    public global::Player? FindByAnyNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (PlayerEntry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.player;

            if (Nickname.Match(player._nickname, nickname, strictLength, stringComparsion) ||
                Nickname.Match(player._globalNickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }
}
