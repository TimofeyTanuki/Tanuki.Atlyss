using Mirror;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.Game.Utilities.Player;

namespace Tanuki.Atlyss.Game.Providers;

public sealed class Player
{
    public static Player Instance { get; internal set; } = null!;

    public readonly Dictionary<uint, global::Player> players = [];
    private readonly SortedSet<uint> initializingPlayers = [];

    public static event Action<global::Player>? OnPlayerAdded;
    public static event Action<global::Player>? OnPlayerRemoved;
    public static event Action<global::Player>? OnPlayerInitialized;

    public IReadOnlyDictionary<uint, global::Player> Players => players;

    private Player()
    {
        Patches.NetworkBehaviour.OnStartClient.OnPostfix += OnNetworkBehaviourStartClientPostfix;
        Patches.NetworkBehaviour.OnStopClient.OnPrefix += OnNetworkBehaviourStopClientPostfix;
        Patches.AtlyssNetworkManager.OnStopClient.OnPrefix += OnAtlyssNetworkManagerStopClientPrefix;
        Patches.Player.DeserializeSyncVars.OnPostfix += OnPlayerDeserializeSyncVarsPostfix;
    }

    public static void Initialize() => Instance ??= new();

    private void AddInitializingPlayer(global::Player player)
    {
        if (player.isLocalPlayer || player._isHostPlayer)
        {
            OnPlayerInitialized?.Invoke(player);
            return;
        }

        initializingPlayers.Add(player.netId);
    }

    private void HandleInitializingPlayer(global::Player player)
    {
        if (player._currentGameCondition == GameCondition.LOADING_GAME)
            return;

        if (!initializingPlayers.Remove(player.netId))
            return;

        OnPlayerInitialized?.Invoke(player);
    }

    private void OnPlayerDeserializeSyncVarsPostfix(global::Player player, NetworkReader networkReader, bool initialState, int originalPosition)
    {
        if (initializingPlayers.Count == 0)
            return;

        HandleInitializingPlayer(player);
    }

    private void OnAtlyssNetworkManagerStopClientPrefix()
    {
        initializingPlayers.Clear();
        players.Clear();
    }

    private void OnNetworkBehaviourStartClientPostfix(NetworkBehaviour instance)
    {
        if (instance is not global::Player player)
            return;

        if (!instance)
            return;

        if (string.IsNullOrEmpty(player._steamID))
            return;

        if (players.ContainsKey(player.netId))
        {
            players[player.netId] = player;
            return;
        }

        players.Add(player.netId, player);
        AddInitializingPlayer(player);

        OnPlayerAdded?.Invoke(player);
    }

    private void OnNetworkBehaviourStopClientPostfix(NetworkBehaviour instance)
    {
        if (instance is not global::Player player)
            return;

        if (!players.ContainsKey(instance.netId))
            return;

        players.Remove(instance.netId);
        initializingPlayers.Remove(instance.netId);

        OnPlayerRemoved?.Invoke(player);
    }

    public global::Player? GetByNetID(uint netId)
    {
        foreach (global::Player Player in players.Values)
            if (Player.netId == netId)
                return Player;

        return null;
    }

    public global::Player? GetBySteamId(ulong steamId)
    {
        string match = steamId.ToString();

        foreach (global::Player player in players.Values)
            if (player._steamID == match)
                return player;

        return null;
    }

    public global::Player? GetByDefaultNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (global::Player Player in players.Values)
            if (Nickname.Match(Player._nickname, nickname, strictLength, stringComparsion))
                return Player;

        return null;
    }

    public global::Player? GetByGlobalNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (global::Player Player in players.Values)
            if (Nickname.Match(Player._globalNickname, nickname, strictLength, stringComparsion))
                return Player;

        return null;
    }

    public global::Player? GetByAnyNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (global::Player Player in players.Values)
            if (Nickname.Match(Player._nickname, nickname, strictLength, stringComparsion) ||
                Nickname.Match(Player._globalNickname, nickname, strictLength, stringComparsion))
                return Player;

        return null;
    }
}
