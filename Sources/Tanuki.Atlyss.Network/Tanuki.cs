using BepInEx.Logging;
using System;
using UnityEngine;

namespace Tanuki.Atlyss.Network;

/// <summary>
/// This module is initialized automatically from the core.
/// </summary>
/// <remarks>
/// Manually calling <see cref="Initialize"/> isn't recommended.
/// </remarks>
public sealed class Tanuki
{
    public const string MODULE_NAME = "Tanuki.Atlyss.Network";

    public const int STEAM_NETWORKING_MESSAGE_CHANNEL = 130523;

    public const int PACKET_SIGNATURE_SIZE = sizeof(ulong);
    public const int PACKET_MAX_SIZE = 4096;
    public const int PACKET_DATA_MAX_SIZE = PACKET_MAX_SIZE - PACKET_SIGNATURE_SIZE;

    private static Tanuki instance = null!;
    private static Action? onInitialized;

    internal GameObject gameObject = null!;
    internal ManualLogSource manualLogSource = null!;

    private Types.Tanuki.Registers registers = null!;
    private Types.Tanuki.Providers providers = null!;
    private Types.Tanuki.Managers managers = null!;
    private Types.Tanuki.Services services = null!;
    private Types.Tanuki.Routers routers = null!;

    public static Tanuki Instance => instance;

    public Types.Tanuki.Registers Registers => registers;
    public Types.Tanuki.Providers Providers => providers;
    public Types.Tanuki.Managers Managers => managers;
    public Types.Tanuki.Services Services => services;
    public Types.Tanuki.Routers Routers => routers;

    public static event Action OnInitialized
    {
        add { onInitialized += value; }
        remove { onInitialized -= value; }
    }

    private Tanuki() { }

    public static void Initialize(GameObject owner)
    {
        if (!owner)
            return;

        if (instance is not null)
            return;

        ManualLogSource manualLogSource = new(MODULE_NAME);
        BepInEx.Logging.Logger.Sources.Add(manualLogSource);

        Types.Tanuki.Registers registers = new()
        {
            packets = new(manualLogSource)
        };

        Providers.Steam steamProvider = new();

        Types.Tanuki.Providers providers = new()
        {
            steam = steamProvider,
            steamLobby = new(steamProvider),
            packet = new()
        };

        Types.Tanuki.Services services = new()
        {
            packetProcessor = new(providers.packet),
            rateLimiter = null
        };

        Types.Tanuki.Routers routers = new()
        {
            packet = new(manualLogSource, registers.packets, services.packetProcessor, providers.steamLobby)
        };

        Components.SteamNetworkingMessagePoller steamNetworkMessagePoller = owner.AddComponent<Components.SteamNetworkingMessagePoller>();
        steamNetworkMessagePoller.MessageChannel = STEAM_NETWORKING_MESSAGE_CHANNEL;

        Types.Tanuki.Managers managers = new()
        {
            packets = new(manualLogSource, registers.packets),
            network = new(
                manualLogSource,
                providers.steam,
                providers.steamLobby,
                steamNetworkMessagePoller,
                registers.packets,
                services,
                routers.packet
            )
        };

        instance = new()
        {
            manualLogSource = manualLogSource,
            managers = managers,
            providers = providers,
            registers = registers,
            services = services,
            routers = routers
        };

        onInitialized?.Invoke();
        onInitialized = null;
    }
}
