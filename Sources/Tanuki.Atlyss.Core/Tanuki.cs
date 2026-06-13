using BepInEx.Logging;
using System;

namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    internal static Tanuki instance = null!;
    private static Action? onInitialized;

    internal Types.Tanuki.Registers registers = null!;
    internal Types.Tanuki.Managers managers = null!;
    internal Types.Tanuki.Providers providers = null!;
    internal Types.Tanuki.Routers routers = null!;
    internal Types.Tanuki.Services services = null!;

    public static Tanuki Instance => instance;

    public Types.Tanuki.Registers Registers => registers;
    public Types.Tanuki.Managers Managers => managers;
    public Types.Tanuki.Providers Providers => providers;
    public Types.Tanuki.Routers Routers => routers;
    public Types.Tanuki.Services Services => services;

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
            commands = new(),
            settings = new(),
            commandCallerPolicies = new()
        };

        Types.Tanuki.Registers registers = new()
        {
            commands = new(manualLogSource, providers.commandCallerPolicies, providers.settings.CommandSection),
            plugins = new()
        };

        Types.Tanuki.Routers routers = new()
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

        Types.Tanuki.Managers managers = new()
        {
            plugin = new(manualLogSource, registers.plugins),
            chat = new(routers.commands),
            hotkey = new(BepInEx.UnityInput.Current)
        };

        Types.Tanuki.Services services = new()
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