using Steamworks;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Routers;

public sealed class Commands
{
    private readonly Network.Managers.Packets packetManager;
    private readonly Parsers.Commands commandParser;
    private readonly Types.Settings.Commands commandSettings;
    private readonly Registers.Commands commandRegistry;
    private readonly Providers.Commands commandProvider;
    private readonly Network.Providers.SteamLobby steamLobbyProvider;
    private readonly Network.Routers.Packets packetRouter;

    private readonly Game.Providers.Player playerProvider;

    public bool serverSupportsRequests;

    internal Commands(
        Network.Registers.Packets packetRegistry,
        Network.Managers.Packets packetManager,
        Parsers.Commands commandParser,
        Types.Settings.Commands commandSettings,
        Registers.Commands commandRegistry,
        Providers.Commands commandProvider,
        Network.Providers.SteamLobby steamLobbyProvider,
        Network.Routers.Packets packetRouter,
        Game.Providers.Player playerProvider)
    {
        this.packetManager = packetManager;
        this.commandParser = commandParser;
        this.commandSettings = commandSettings;
        this.commandRegistry = commandRegistry;
        this.commandProvider = commandProvider;
        this.steamLobbyProvider = steamLobbyProvider;
        this.packetRouter = packetRouter;
        this.playerProvider = playerProvider;

        packetRegistry.Register<Packets.Commands.Request>();
        packetManager.AddHandler<Packets.Commands.Request>(RequestReceived);

        Game.Patches.Player.OnStartAuthority.OnPostfix += OnPlayerStartAuthority;
    }

    public void Refresh() =>
        CheckServerRuntime();

    private void CheckServerRuntime()
    {
        bool isHost = false;

        if (Player._mainPlayer)
            isHost = Player._mainPlayer._isHostPlayer;

        packetManager.ChangeMuteState<Packets.Commands.Request>(!isHost);
    }

    private void OnPlayerStartAuthority(Player player) =>
        CheckServerRuntime();

    public bool HandleCommandClient(string input)
    {
        IReadOnlyDictionary<string, Type> nameMap = commandRegistry.NameMap;

        if (commandParser.TryParse(commandSettings.Prefix, input, out string? commandName, out IReadOnlyList<string>? commandArguments, nameMap))
        {
            Type commandType = nameMap[commandName];
            Types.Commands.Descriptor commandDescriptor = commandRegistry.Descriptors[commandType];

            if (commandDescriptor.executionSide == EExecutionSide.Client || Player._mainPlayer._isHostPlayer)
            {
                ICaller caller = new Types.Commands.Callers.Player(Player._mainPlayer);

                if (commandDescriptor.callerPolicy.IsAllowed(caller))
                {
                    ICommand command = commandProvider.Create(commandType);
                    command.Execute(
                        new Contexts.Commands.Context
                        {
                            Caller = caller,
                            Arguments = commandArguments
                        }
                    );
                }
            }
            else if (serverSupportsRequests)
            {
                packetRouter.SendPacketToUser(
                    steamLobbyProvider.OwnerSteamId,
                    new Packets.Commands.Request { Hash = commandDescriptor.Hash, Arguments = commandArguments },
                    out EResult _
                );
            }

            return true;
        }

        return false;
    }

    public bool HandleCommandServer(Player player, string input)
    {
        IReadOnlyDictionary<string, Type> nameMap = commandRegistry.NameMap;

        if (!commandParser.TryParse(commandSettings.Prefix, input, out string? commandName, out IReadOnlyList<string>? commandArguments, nameMap))
            return false;

        Type commandType = nameMap[commandName];
        return HandleCommandServer(player, commandType, commandArguments);
    }

    public void RequestReceived(CSteamID sender, Packets.Commands.Request packet)
    {
        Player? player = playerProvider.FindBySteamId(sender);

        if (player is null)
            return;

        Type? commandType = null;

        if (packet.Hash.HasValue)
            commandRegistry.HashMap.TryGetValue(packet.Hash.Value, out commandType);
        else if (!string.IsNullOrEmpty(packet.Name))
            commandRegistry.NameMap.TryGetValue(packet.Name, out commandType);

        if (commandType is null)
            return;

        HandleCommandServer(player, commandType, packet.Arguments);
    }

    public bool HandleCommandServer(Player player, Type commandType, IReadOnlyList<string> arguments)
    {
        ICaller caller = new Types.Commands.Callers.Player(player);
        Types.Commands.Descriptor commandDescriptor = commandRegistry.Descriptors[commandType];

        if (commandDescriptor.executionSide != EExecutionSide.Server)
            return false;

        if (commandDescriptor.callerPolicy.IsAllowed(caller))
        {
            ICommand command = commandProvider.Create(commandType);
            command.Execute(
                new Contexts.Commands.Context
                {
                    Caller = caller,
                    Arguments = arguments
                }
            );
        }

        return true;
    }
}
