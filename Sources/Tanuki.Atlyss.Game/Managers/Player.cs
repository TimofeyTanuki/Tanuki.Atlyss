using Mirror;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game.Managers;

public class Player
{
    public static Player Instance = null!;
    public readonly Dictionary<uint, global::Player> Players = [];
    private readonly SortedSet<uint> InitializingPlayers = [];

    public delegate void PlayerAdded(global::Player Player);
    public static event PlayerAdded? OnPlayerAdded;

    public delegate void PlayerRemoved(global::Player Player);
    public static event PlayerRemoved? OnPlayerRemoved;

    public delegate void PlayerInitialized(global::Player Player);
    public static event PlayerInitialized? OnPlayerInitialized;

    private Player()
    {
        Game.Patches.NetworkBehaviour.OnStartClient.OnPostfix += OnNetworkBehaviourStartClientPostfix;
        Game.Patches.NetworkBehaviour.OnStopClient.OnPrefix += OnNetworkBehaviourStopClientPostfix;
        Game.Patches.AtlyssNetworkManager.OnStopClient.OnPrefix += OnAtlyssNetworkManagerStopClientPrefix;
        Game.Patches.Player.DeserializeSyncVars.OnPostfix += OnPlayerDeserializeSyncVarsPostfix;
    }

    private void AddInitializingPlayer(global::Player Player)
    {
        if (Player.isLocalPlayer || Player._isHostPlayer)
        {
            OnPlayerInitialized?.Invoke(Player);
            return;
        }

        InitializingPlayers.Add(Player.netId);
    }

    private void HandleInitializingPlayer(global::Player Player)
    {
        if (Player._currentGameCondition == GameCondition.LOADING_GAME)
            return;

        if (!InitializingPlayers.Remove(Player.netId))
            return;

        OnPlayerInitialized?.Invoke(Player);
    }

    private void OnPlayerDeserializeSyncVarsPostfix(global::Player Player, NetworkReader NetworkReader, bool InitialState, int InitialPosition)
    {
        if (InitializingPlayers.Count == 0)
            return;

        HandleInitializingPlayer(Player);
    }

    public static void Initialize() => Instance ??= new();

    private void OnAtlyssNetworkManagerStopClientPrefix()
    {
        InitializingPlayers.Clear();
        Players.Clear();
    }

    private void OnNetworkBehaviourStartClientPostfix(NetworkBehaviour NetworkBehaviour)
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

    private void OnNetworkBehaviourStopClientPostfix(NetworkBehaviour NetworkBehaviour)
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