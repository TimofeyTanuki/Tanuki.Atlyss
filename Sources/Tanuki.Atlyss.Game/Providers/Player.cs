using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.Game.Data.Player;
using Tanuki.Atlyss.Game.Utilities.Player;

namespace Tanuki.Atlyss.Game.Providers;

public sealed class Player
{
    private readonly Dictionary<uint, Entry> playerEntries = [];
    private readonly Dictionary<ulong, uint> steamIdMap = [];
    private readonly HashSet<uint> loadingPlayers = [];

    private bool playerObservationState = false;
    private bool lastNetworkServerState = false;

    /// <summary>
    /// Invoked once when a new network player starts on the client.
    /// </summary>
    public static event Action<global::Player>? OnPlayerAdded;

    /// <summary>
    /// Invoked once when a network player's game state changes to <see cref="GameCondition.IN_GAME"/>.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item>May not be invoked if a new network player doesn't load.</item>
    /// <item>Called for all players, including those initialized on the server before the client.</item>
    /// </list>
    /// </remarks>
    public static event Action<global::Player>? OnPlayerLoaded;

    /// <summary>
    /// Invoked once when a network player stops on the client.
    /// </summary>
    public static event Action<global::Player>? OnPlayerRemoved;

    /// <summary>
    /// Provides a lookup of current player entries by <see cref="NetworkBehaviour.netId"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="Entry.SteamId"/> is available once <see cref="OnPlayerLoaded"/> is invoked.
    /// </remarks>
    public IReadOnlyDictionary<uint, Entry> PlayerEntries => playerEntries;

    /// <summary>
    /// Provides a lookup of player <see cref="NetworkBehaviour.netId"/> by their <see cref="CSteamID.m_SteamID"/>.
    /// </summary>
    /// <remarks>
    /// A player can be found here once <see cref="OnPlayerLoaded"/> is invoked.
    /// </remarks>
    public IReadOnlyDictionary<ulong, uint> SteamIdMap => steamIdMap;

    /// <summary>
    /// Provides a collection of players that are currently expected to load and trigger <see cref="OnPlayerLoaded"/>.
    /// </summary>
    public IReadOnlyCollection<uint> LoadingPlayers => loadingPlayers;

    internal Player()
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

        if (playerEntries.ContainsKey(netId))
            return;

        playerEntries.Add(netId, new(player));

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

        if (!playerEntries.TryGetValue(netId, out Entry playerEntry))
            return;

        playerEntries.Remove(netId);
        steamIdMap.Remove(playerEntry.steamId.m_SteamID);

        FinalizePlayerInitialization(netId);

        OnPlayerRemoved?.Invoke(player);
    }

    /// <summary>
    /// Finds a player by their <see cref="NetworkBehaviour.netId"/>.
    /// </summary>
    /// <returns><see cref="global::Player"/> if found; otherwise, <see langword="null"/>.</returns>
    public global::Player? FindByNetId(uint netId)
    {
        if (playerEntries.TryGetValue(netId, out Entry playerEntry))
            return playerEntry.Player;

        return null;
    }

    /// <summary>
    /// Finds a player by their <see cref="CSteamID.m_SteamID"/>.
    /// </summary>
    /// <returns><see cref="global::Player"/> if found; otherwise, <see langword="null"/>.</returns>
    public global::Player? FindBySteamId(ulong steamId)
    {
        if (steamIdMap.TryGetValue(steamId, out uint netId) &&
            playerEntries.TryGetValue(netId, out Entry playerEntry))
            return playerEntry.Player;

        return null;
    }

    /// <summary>
    /// Finds a player by their <see cref="CSteamID"/>.
    /// </summary>
    /// <returns><see cref="global::Player"/> if found; otherwise, <see langword="null"/>.</returns>
    public global::Player? FindBySteamId(CSteamID steamId) => FindBySteamId(steamId.m_SteamID);

    /// <summary>
    /// Finds a player by their <see cref="global::Player._nickname"/>.
    /// </summary>
    /// <param name="nickname">
    /// The nickname to search for.
    /// </param>
    /// <param name="strictLength">
    /// Whether to match the length strictly.
    /// </param>
    /// <param name="stringComparsion">
    /// <see cref="StringComparison"/> method to use.
    /// </param>
    /// <returns><see cref="global::Player"/> if found; otherwise, <see langword="null"/>.</returns>
    public global::Player? FindByDefaultNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (Entry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.Player;

            if (Nickname.Match(player._nickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }

    /// <summary>
    /// Finds a player by their <see cref="global::Player._globalNickname"/>.
    /// </summary>
    /// <param name="nickname">
    /// The nickname to search for.
    /// </param>
    /// <param name="strictLength">
    /// Whether to match the length strictly.
    /// </param>
    /// <param name="stringComparsion">
    /// <see cref="StringComparison"/> method to use.
    /// </param>
    /// <returns><see cref="global::Player"/> if found; otherwise, <see langword="null"/>.</returns>
    public global::Player? FindByGlobalNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (Entry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.Player;

            if (Nickname.Match(player._globalNickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }

    /// <summary>
    /// Finds a player by their <see cref="global::Player._nickname"/> or <see cref="global::Player._globalNickname"/>.
    /// </summary>
    /// <param name="nickname">
    /// The nickname to search for.
    /// </param>
    /// <param name="strictLength">
    /// Whether to match the length strictly.
    /// </param>
    /// <param name="stringComparsion">
    /// <see cref="StringComparison"/> method to use.
    /// </param>
    /// <returns><see cref="global::Player"/> if found; otherwise, <see langword="null"/>.</returns>
    public global::Player? FindByAnyNickname(string nickname, bool strictLength, StringComparison stringComparsion)
    {
        foreach (Entry playerEntry in playerEntries.Values)
        {
            global::Player player = playerEntry.Player;

            if (Nickname.Match(player._nickname, nickname, strictLength, stringComparsion) ||
                Nickname.Match(player._globalNickname, nickname, strictLength, stringComparsion))
                return player;
        }

        return null;
    }

    /// <summary>
    /// Finds a player automatically by input, which can be a <see cref="NetworkBehaviour.netId"/>, <see cref="CSteamID.m_SteamID"/> or nickname.
    /// </summary>
    /// <remarks>
    /// Search order:
    /// <list type="number">
    /// <item><see cref="NetworkBehaviour.netId"/></item>
    /// <item><see cref="CSteamID.m_SteamID"/></item>
    /// <item>Nickname according to <paramref name="nicknameType"/></item>
    /// </list>
    /// </remarks>
    /// <param name="input">
    /// The input string to search by.
    /// </param>
    /// <param name="nicknameType">
    /// Which <see cref="ENicknameType"/> to search.
    /// </param>
    /// <param name="nicknameStrictLength">
    /// Whether to match the nickname length strictly.
    /// </param>
    /// <param name="nicknameStrictComparsion">
    /// <see cref="StringComparison"/> method to use for nickname.
    /// </param>
    /// <returns><see cref="global::Player"/> if found; otherwise, <see langword="null"/>.</returns>
    public global::Player? FindByAutoRecognition(
        string input,
        ENicknameType nicknameType = ENicknameType.Any,
        bool nicknameStrictLength = false,
        StringComparison nicknameStrictComparsion = StringComparison.InvariantCultureIgnoreCase
    )
    {
        global::Player? player = null;

        if (uint.TryParse(input, out uint netId))
            player = FindByNetId(netId);
        else if (ulong.TryParse(input, out ulong steamId))
            player = FindBySteamId(steamId);

        if (player is not null)
            return player;

        return FindByNickname(input, nicknameType, nicknameStrictLength, nicknameStrictComparsion);
    }

    /// <summary>
    /// Finds a player by nickname according to the specified <see cref="ENicknameType"/>.
    /// </summary>
    /// <param name="nickname">
    /// The nickname to search for.
    /// </param>
    /// <param name="nicknameType">
    /// Which <see cref="ENicknameType"/> to search.
    /// </param>
    /// <param name="strictLength">
    /// Whether to match the length strictly.
    /// </param>
    /// <param name="stringComparsion">
    /// <see cref="StringComparison"/> method to use.
    /// </param>
    public global::Player? FindByNickname(
        string nickname,
        ENicknameType nicknameType = ENicknameType.Any,
        bool strictLength = false,
        StringComparison stringComparsion = StringComparison.InvariantCultureIgnoreCase
    ) =>
        nicknameType switch
        {
            ENicknameType.Default => FindByDefaultNickname(nickname, strictLength, stringComparsion),
            ENicknameType.Global => FindByGlobalNickname(nickname, strictLength, stringComparsion),
            ENicknameType.Any => FindByAnyNickname(nickname, strictLength, stringComparsion),
            _ => null,
        };
}
