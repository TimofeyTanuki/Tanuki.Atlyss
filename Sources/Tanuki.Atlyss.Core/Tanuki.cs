using BepInEx.Logging;
using System;

namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    private static Tanuki instance = null!;
    private static Action? onInitialized;

    private Types.Tanuki.Registers registers = null!;
    private Types.Tanuki.Managers managers = null!;
    private Types.Tanuki.Providers providers = null!;
    private Types.Tanuki.Routers routers = null!;
    private Types.Tanuki.Services services = null!;

    public static Tanuki Instance => instance;

    public Types.Tanuki.Registers Registers
    {
        get => registers;
        internal set => registers = value;
    }
    public Types.Tanuki.Managers Managers
    {
        get => managers;
        internal set => managers = value;
    }
    public Types.Tanuki.Providers Providers
    {
        get => providers;
        internal set => providers = value;
    }
    public Types.Tanuki.Routers Routers
    {
        get => routers;
        internal set => routers = value;
    }
    public Types.Tanuki.Services Services
    {
        get => services;
        internal set => services = value;
    }

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

        Types.Tanuki.Providers providers = new()
        {
            Commands = new(),
            Settings = new(),
            CommandCallerPolicies = new()
        };

        Types.Tanuki.Registers registers = new()
        {
            Commands = new(manualLogSource, providers.CommandCallerPolicies, providers.Settings.CommandSection),
            Plugins = new()
        };

        Types.Tanuki.Routers routers = new()
        {
            Commands = new(
                tanukiNetwork.Registers.Packets,
                tanukiNetwork.Managers.Packets,
                new(['"', '\"', '`']),
                providers.Settings.CommandSection,
                registers.Commands,
                providers.Commands,
                tanukiNetwork.Providers.SteamLobby,
                tanukiNetwork.Routers.Packet,
                tanukiGame.Providers.Player
            )
        };

        Types.Tanuki.Managers managers = new()
        {
            Plugin = new(manualLogSource, registers.Plugins),
            Chat = new(routers.Commands),
            Hotkey = new()
        };

        Types.Tanuki.Services services = new()
        {
            TanukiServer = new(
                tanukiNetwork,
                routers.Commands,
                providers.Settings,
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