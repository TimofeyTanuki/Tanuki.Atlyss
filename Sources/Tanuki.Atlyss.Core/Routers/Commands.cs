using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Routers;

public sealed class Commands
{
    private readonly Parsers.Commands commandParser;
    private readonly Data.Settings.Commands commandSettings;
    private readonly Registers.Commands commandRegistry;
    private readonly Providers.Commands commandProvider;

    internal Commands(
        Parsers.Commands commandParser,
        Data.Settings.Commands commandSettings,
        Registers.Commands commandRegistry,
        Providers.Commands commandProvider)
    {
        this.commandParser = commandParser;
        this.commandSettings = commandSettings;
        this.commandRegistry = commandRegistry;
        this.commandProvider = commandProvider;
    }

    public bool RouteCommand(string input)
    {
        IReadOnlyDictionary<string, Type> nameMap = commandRegistry.NameMap;

        ICommand? command;
        Main.Instance.ManualLogSource.LogInfo($"Name map size: {nameMap.Count}");

        if (commandParser.TryParse(commandSettings.ClientPrefix, input, out string? commandName, out IReadOnlyList<string>? commandArguments, nameMap))
        {
            Main.Instance.ManualLogSource.LogInfo($"Executing: {commandName} [{string.Join(",", commandArguments)}]");
            command = commandProvider.Create(nameMap[commandName]);


            //test only
            command.ClientCallback(
                new Contexts.Commands.Context()
                {
                    Caller = new Data.Commands.Callers.Player()
                    {
                        player = Player._mainPlayer
                    },
                    Arguments = commandArguments
                }
            );


            //if (command.ExecutionType == EExecutionType.Local)
            //commands.NameMap[commandName]

            return true;
        }
        // commandSettings.ServerPrefix must be replaced with synced string (main tanuki atlyss packet to identify friendly servers)
        if (commandParser.TryParse(commandSettings.ServerPrefix, input, out commandName, out commandArguments))
        {
            // send to server
            Main.Instance.ManualLogSource.LogInfo($"Send to server: {commandName} [{string.Join(",", commandArguments)}]");
        }

        return false;
    }

    public void HandleIncomingPacket()
    {

    }
}
