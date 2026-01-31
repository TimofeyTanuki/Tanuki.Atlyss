using BepInEx.Logging;
using Steamworks;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Routers;

public sealed class Commands
{
    private readonly ManualLogSource manualLogSource;
    private readonly Parsers.Commands commandParser;
    private readonly Data.Settings.Commands commandSettings;
    private readonly Registers.Commands commandRegistry;
    private readonly Providers.Commands commandProvider;

    public string? ServerPrefix;

    internal Commands(
        ManualLogSource manualLogSource,
        Parsers.Commands commandParser,
        Data.Settings.Commands commandSettings,
        Registers.Commands commandRegistry,
        Providers.Commands commandProvider)
    {
        this.manualLogSource = manualLogSource;
        this.commandParser = commandParser;
        this.commandSettings = commandSettings;
        this.commandRegistry = commandRegistry;
        this.commandProvider = commandProvider;
    }

    public bool HandleCommandClient(string input)
    {
        IReadOnlyDictionary<string, Type> nameMap = commandRegistry.NameMap;

        if (commandParser.TryParse(commandSettings.ClientPrefix, input, out string? commandName, out IReadOnlyList<string>? commandArguments, nameMap))
        {
            manualLogSource.LogInfo($"Executing: {commandName} [{string.Join(",", commandArguments)}]");
            Type commandType = nameMap[commandName];
            Data.Commands.Descriptor commandDescriptor = commandRegistry.Descriptors[commandType];

            if (commandDescriptor.executionSide == EExecutionSide.Server && !AtlyssNetworkManager._current.isNetworkActive)
            {
                manualLogSource.LogInfo($"(Remote command) Send to server: {commandName} [{string.Join(",", commandArguments)}]");

                Main.Instance.Network.Routers.Packet.SendPacketToUser(
                    Main.Instance.Network.Providers.SteamLobby.OwnerSteamId,
                    new Packets.Commands.Request()
                    {
                        Hash = commandDescriptor.Hash,
                        Arguments = commandArguments
                    },
                    out EResult _
                );
            }
            else
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

            return true;
        }

        if (Player._mainPlayer._isHostPlayer || string.IsNullOrEmpty(ServerPrefix))
            return false;

        if (commandParser.TryParse(ServerPrefix, input, out commandName, out commandArguments))
        {
            manualLogSource.LogInfo($"(Unknown command) Send to server: {commandName} [{string.Join(",", commandArguments)}]");

            Main.Instance.Network.Routers.Packet.SendPacketToUser(
                Main.Instance.Network.Providers.SteamLobby.OwnerSteamId,
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

    public void HandleIncomingPacket(CSteamID sender, Packets.Commands.Request packet)
    {
        Player? player = Game.Providers.Player.Instance.GetBySteamId(sender);

        if (!player)
            return;

        Type? commandType = null;

        if (packet.Hash.HasValue)
            commandRegistry.HashMap.TryGetValue(packet.Hash.Value, out commandType);
        else if (!string.IsNullOrEmpty(packet.Name))
            commandRegistry.NameMap.TryGetValue(packet.Name, out commandType);

        if (commandType is null)
            return; // send "command not found" packet?

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
                    Caller = new Data.Commands.Callers.Player(Player._mainPlayer),
                    Arguments = Arguments
                }
            );
        }

        return true;
    }
}
