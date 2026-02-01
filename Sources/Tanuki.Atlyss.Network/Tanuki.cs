using BepInEx.Logging;
using UnityEngine;

namespace Tanuki.Atlyss.Network;

public sealed class Tanuki
{
    public const int PACKET_SIGNATURE_SIZE = sizeof(ulong);
    public const int PACKET_MAX_SIZE = 4096;
    public const int PACKET_DATA_MAX_SIZE = PACKET_MAX_SIZE - PACKET_SIGNATURE_SIZE;

    private static Tanuki instance = null!;
    public static Tanuki Instance => instance;

    internal GameObject gameObject = null!;
    internal ManualLogSource manualLogSource = null!;
    internal Data.Tanuki.Registers registers = null!;
    internal Data.Tanuki.Providers providers = null!;
    internal Data.Tanuki.Managers managers = null!;
    internal Data.Tanuki.Services services = null!;
    internal Data.Tanuki.Routers routers = null!;

    public GameObject GameObject => gameObject;
    public Data.Tanuki.Registers Registers => registers;
    public Data.Tanuki.Providers Providers => providers;
    public Data.Tanuki.Managers Managers => managers;
    public Data.Tanuki.Services Services => services;
    public Data.Tanuki.Routers Routers => routers;

    private Tanuki() { }

    public static void Initialize()
    {
        if (instance is not null)
            return;

        ManualLogSource manualLogSource = new("Tanuki.Atlyss.Network");
        BepInEx.Logging.Logger.Sources.Add(manualLogSource);

        Data.Tanuki.Registers registers = new()
        {
            packets = new(manualLogSource)
        };

        Providers.Steam steamProvider = new();

        Data.Tanuki.Providers providers = new()
        {
            steam = steamProvider,
            steamLobby = new(steamProvider),
            packet = new()
        };

        Data.Tanuki.Services services = new()
        {
            packetProcessor = new(providers.packet),
            rateLimiter = new()
        };

        Data.Tanuki.Routers routers = new()
        {
            packet = new(manualLogSource, registers.packets, services.packetProcessor, providers.steamLobby)
        };

        GameObject gameObject = new();
        Components.SteamNetworkMessagePoller steamNetworkMessagePoller = gameObject.AddComponent<Components.SteamNetworkMessagePoller>();
        Object.DontDestroyOnLoad(gameObject);

        Data.Tanuki.Managers managers = new()
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
    }
}
