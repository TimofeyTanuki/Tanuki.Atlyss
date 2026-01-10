using Mirror;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game.Managers;

public class Player
{
    public static Player Instance;
    private readonly Patching.Patcher Patcher;
    public readonly Dictionary<uint, global::Player> Players = [];
    private readonly SortedSet<uint> InitializingPlayers = [];

    public delegate void PlayerAdded(global::Player Player);
    public static event PlayerAdded OnPlayerAdded;

    public delegate void PlayerRemoved(global::Player Player);
    public static event PlayerRemoved OnPlayerRemoved;

    public delegate void PlayerInitialized(global::Player Player);
    public static event PlayerInitialized OnPlayerInitialized;

    private Player()
    {
        Patcher = new();
        Patcher.Use(
            typeof(Patches.NetworkBehaviour.OnStartClient_Postfix),
            typeof(Patches.NetworkBehaviour.OnStopClient_Prefix),
            typeof(Patches.AtlyssNetworkManager.OnStopClient_Prefix),
            typeof(Patches.Player.DeserializeSyncVars)
        );

        Patches.NetworkBehaviour.OnStartClient_Postfix.OnInvoke += OnStartClient;
        Patches.NetworkBehaviour.OnStopClient_Prefix.OnInvoke += OnStopClient;
        Patches.AtlyssNetworkManager.OnStopClient_Prefix.OnInvoke += OnStopAtlyssNetworkManager;
    }

    private void AddInitializingPlayer(global::Player Player)
    {
        if (Player.isLocalPlayer || Player._isHostPlayer)
        {
            OnPlayerInitialized?.Invoke(Player);
            return;
        }

        if (InitializingPlayers.Count == 0)
            Patches.Player.DeserializeSyncVars.OnAfter += DeserializeSyncVars_OnAfter;

        InitializingPlayers.Add(Player.netId);
    }

    private void HandleInitializingPlayer(global::Player Player)
    {
        if (Player._currentGameCondition == GameCondition.LOADING_GAME)
            return;

        if (!InitializingPlayers.Remove(Player.netId))
            return;

        if (InitializingPlayers.Count == 0)
            Patches.Player.DeserializeSyncVars.OnAfter -= DeserializeSyncVars_OnAfter;

        OnPlayerInitialized?.Invoke(Player);
    }

    private void DeserializeSyncVars_OnAfter(global::Player Player, NetworkReader NetworkReader, bool InitialState, bool Cancelled) =>
        HandleInitializingPlayer(Player);

    public static void Initialize() => Instance ??= new();

    private void OnStopAtlyssNetworkManager()
    {
        Patches.Player.DeserializeSyncVars.OnAfter -= DeserializeSyncVars_OnAfter;

        InitializingPlayers.Clear();
        Players.Clear();
    }

    private void OnStartClient(NetworkBehaviour NetworkBehaviour)
    {
        if (NetworkBehaviour is not global::Player Player)
            return;

        if (!NetworkBehaviour)
            return;

        if (string.IsNullOrEmpty(Player._steamID))
            return;

        if (Players.ContainsKey(Player.netId))
        {
            Players[Player.netId] = Player;
            return;
        }

        Players.Add(Player.netId, Player);
        OnPlayerAdded?.Invoke(Player);

        AddInitializingPlayer(Player);
    }

    private void OnStopClient(NetworkBehaviour NetworkBehaviour)
    {
        if (NetworkBehaviour is not global::Player Player)
            return;

        if (!Players.ContainsKey(NetworkBehaviour.netId))
            return;

        Players.Remove(NetworkBehaviour.netId);
        OnPlayerRemoved?.Invoke(Player);

        InitializingPlayers.Remove(NetworkBehaviour.netId);
    }
}