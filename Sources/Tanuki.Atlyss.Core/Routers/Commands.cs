using Steamworks;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Routers;

public sealed class Commands
{
    private readonly Network.Managers.Packets packetManager;
    private readonly Parsers.Commands commandParser;
    private readonly Data.Settings.Commands commandSettings;
    private readonly Registers.Commands commandRegistry;
    private readonly Providers.Commands commandProvider;
    private readonly Network.Providers.SteamLobby steamLobbyProvider;
    private readonly Network.Routers.Packets packetRouter;

    public string? ServerPrefix;

    internal Commands(
        Network.Registers.Packets packetRegistry,
        Network.Managers.Packets packetManager,
        Parsers.Commands commandParser,
        Data.Settings.Commands commandSettings,
        Registers.Commands commandRegistry,
        Providers.Commands commandProvider,
        Network.Providers.SteamLobby steamLobbyProvider,
        Network.Routers.Packets packetRouter)
    {
        this.packetManager = packetManager;
        this.commandParser = commandParser;
        this.commandSettings = commandSettings;
        this.commandRegistry = commandRegistry;
        this.commandProvider = commandProvider;
        this.steamLobbyProvider = steamLobbyProvider;
        this.packetRouter = packetRouter;

        packetRegistry.Register<Packets.Commands.Request>();
        packetRegistry.Register<Packets.Commands.NotFoundResponse>();
        packetManager.AddHandler<Packets.Commands.Request>(RequestReceived);
        packetManager.AddHandler<Packets.Commands.NotFoundResponse>(NotFoundReceived);

        Game.Patches.Player.OnStartAuthority.OnPostfix += OnStartAuthority;
    }

    private void NotFoundReceived(CSteamID sender, Packets.Commands.NotFoundResponse packet)
    {
        ChatBehaviour._current.New_ChatMessage("command not found, lol");
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

    private void OnStartAuthority(Player player) =>
        CheckServerRuntime();

    public bool HandleCommandClient(string input)
    {
        IReadOnlyDictionary<string, Type> nameMap = commandRegistry.NameMap;

        if (commandParser.TryParse(commandSettings.ClientPrefix, input, out string? commandName, out IReadOnlyList<string>? commandArguments, nameMap))
        {
            Type commandType = nameMap[commandName];
            Data.Commands.Descriptor commandDescriptor = commandRegistry.Descriptors[commandType];

            if (commandDescriptor.executionSide == EExecutionSide.Client || Player._mainPlayer._isHostPlayer)
            {
                ICaller caller = new Data.Commands.Callers.Player(Player._mainPlayer);

                if (commandDescriptor.callerPolicy.IsAllowed(caller))
                {
                    ICommand command = commandProvider.Create(commandType);

                    command.Execute(
                        new Contexts.Commands.Context()
                        {
                            Caller = caller,
                            Arguments = commandArguments
                        }
                    );
                }
            }
            else
            {
                packetRouter.SendPacketToUser(
                    steamLobbyProvider.OwnerSteamId,
                    new Packets.Commands.Request()
                    {
                        Hash = commandDescriptor.Hash,
                        Arguments = commandArguments
                    },
                    out EResult _
                );
            }

            return true;
        }

        if (Player._mainPlayer._isHostPlayer || string.IsNullOrEmpty(ServerPrefix))
            return false;

        if (commandParser.TryParse(ServerPrefix, input, out commandName, out commandArguments))
        {
            packetRouter.SendPacketToUser(
                steamLobbyProvider.OwnerSteamId,
                new Packets.Commands.Request()
                {
                    Name = commandName,
                    Arguments = commandArguments
                },
                out EResult _
            );

            return true;
        }

        return false;
    }

    public bool HandleCommandServer(Player player, string input)
    {
        IReadOnlyDictionary<string, Type> nameMap = commandRegistry.NameMap;

        if (commandParser.TryParse(commandSettings.serverPrefix, input, out string? commandName, out IReadOnlyList<string>? commandArguments, nameMap))
        {
            Type commandType = nameMap[commandName];

            return HandleCommandServer(player, commandType, commandArguments);
        }

        return false;
    }

    public void RequestReceived(CSteamID sender, Packets.Commands.Request packet)
    {
        Player? player = Game.Providers.Player.Instance.FindBySteamId(sender);

        if (!player)
            return;

        Type? commandType = null;

        if (packet.Hash.HasValue)
            commandRegistry.HashMap.TryGetValue(packet.Hash.Value, out commandType);
        else if (!string.IsNullOrEmpty(packet.Name))
            commandRegistry.NameMap.TryGetValue(packet.Name, out commandType);

        if (commandType is null)
        {

            packetRouter.SendPacketToUser(
                sender,
                new Packets.Commands.NotFoundResponse(),
                out EResult _
            );
            return;
        }

        HandleCommandServer(player!, commandType, packet.Arguments);
    }

    public bool HandleCommandServer(Player player, Type commandType, IReadOnlyList<string> Arguments)
    {
        ICaller caller = new Data.Commands.Callers.Player(player);
        Data.Commands.Descriptor commandDescriptor = commandRegistry.Descriptors[commandType];

        if (commandDescriptor.executionSide != EExecutionSide.Server)
            return false;

        if (commandDescriptor.callerPolicy.IsAllowed(caller))
        {
            ICommand command = commandProvider.Create(commandType);

            command.Execute(
                new Contexts.Commands.Context()
                {
                    Caller = new Data.Commands.Callers.Player(player),
                    Arguments = Arguments
                }
            );
        }

        return true;
    }
}
