using BepInEx.Logging;
using System;

namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    internal static Tanuki instance = null!;
    private static Action? onInitialized;

    internal Data.Tanuki.Registers registers = null!;
    internal Data.Tanuki.Managers managers = null!;
    internal Data.Tanuki.Providers providers = null!;
    internal Data.Tanuki.Routers routers = null!;
    internal Data.Tanuki.Services services = null!;

    public static Tanuki Instance => instance;

    public Data.Tanuki.Registers Registers => registers;
    public Data.Tanuki.Managers Managers => managers;
    public Data.Tanuki.Providers Providers => providers;
    public Data.Tanuki.Routers Routers => routers;
    public Data.Tanuki.Services Services => services;

    public static event Action OnInitialized
    {
        add { onInitialized += value; }
        remove { onInitialized -= value; }
    }

    internal Tanuki() { }

    internal static void Initialize(Game.Tanuki tanukiGame, Network.Tanuki tanukiNetwork, ManualLogSource manualLogSource)
    {
        if (instance is not null)
            return;

        tanukiNetwork.Providers.Steam.CreateCallbacks();

        Data.Tanuki.Providers providers = new()
        {
            commands = new(),
            settings = new(),
            commandCallerPolicies = new()
        };

        Data.Tanuki.Registers registers = new()
        {
            commands = new(manualLogSource, providers.commandCallerPolicies, providers.settings.CommandSection),
            plugins = new()
        };

        Data.Tanuki.Routers routers = new()
        {
            commands = new(
                tanukiNetwork.Registers.Packets,
                tanukiNetwork.Managers.Packets,
                new(['"', '\"', '`']),
                providers.settings.CommandSection,
                registers.commands,
                providers.commands,
                tanukiNetwork.Providers.SteamLobby,
                tanukiNetwork.Routers.Packet,
                tanukiGame.Providers.Player
            )
        };

        Data.Tanuki.Managers managers = new()
        {
            plugins = new(manualLogSource, registers.plugins),
            chat = new(routers.commands)
        };

        Data.Tanuki.Services services = new()
        {
            tanukiServer = new(
                tanukiNetwork,
                routers.commands,
                providers.settings,
                tanukiNetwork.Routers.Packet,
                tanukiGame.Providers.Player
            )
        };

        instance = new()
        {
            managers = managers,
            providers = providers,
            registers = registers,
            routers = routers,
            services = services
        };

        onInitialized?.Invoke();
        onInitialized = null;
    }
}