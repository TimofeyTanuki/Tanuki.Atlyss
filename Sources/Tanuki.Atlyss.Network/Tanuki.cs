using BepInEx.Logging;
using System;
using UnityEngine;

namespace Tanuki.Atlyss.Network;

public sealed class Tanuki
{
    public const int PACKET_SIGNATURE_SIZE = sizeof(ulong);
    public const int PACKET_MAX_SIZE = 4096;
    public const int PACKET_DATA_MAX_SIZE = PACKET_MAX_SIZE - PACKET_SIGNATURE_SIZE;

    private static Tanuki instance = null!;
    private static Action? onInitialized;

    internal GameObject gameObject = null!;
    internal ManualLogSource manualLogSource = null!;
    internal Types.Tanuki.Registers registers = null!;
    internal Types.Tanuki.Providers providers = null!;
    internal Types.Tanuki.Managers managers = null!;
    internal Types.Tanuki.Services services = null!;
    internal Types.Tanuki.Routers routers = null!;

    public static Tanuki Instance => instance;

    public GameObject GameObject => gameObject;
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

    public static void Initialize()
    {
        if (instance is not null)
            return;

        string moduleName = "Tanuki.Atlyss.Network";

        ManualLogSource manualLogSource = new(moduleName);
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
            rateLimiter = new()
        };

        Types.Tanuki.Routers routers = new()
        {
            packet = new(manualLogSource, registers.packets, services.packetProcessor, providers.steamLobby)
        };

        GameObject gameObject = new(moduleName);
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
        Components.SteamNetworkMessagePoller steamNetworkMessagePoller = gameObject.AddComponent<Components.SteamNetworkMessagePoller>();

        Types.Tanuki.Managers managers = new()
        {
            packets = new(manualLogSource, registers.packets),
            network = new(
                manualLogSource,
                providers.steam,
                providers.steamLobby,
                steamNetworkMessagePoller,
                registers.packets,
                services.rateLimiter,
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
            routers = routers,
            gameObject = gameObject
        };

        onInitialized?.Invoke();
        onInitialized = null;
    }
}
