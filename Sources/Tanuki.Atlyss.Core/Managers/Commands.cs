using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Tanuki.Commands;
using Tanuki.Atlyss.Core.Contexts.Commands;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Commands
{
    private readonly Parsers.Commands parser = new(['"', '\'', '`']);
    private readonly Registers.Commands register;
    private readonly Data.Settings.Commands settings;

    private Func<Type, ICommand> CommandFactory => type => (ICommand)Activator.CreateInstance(type);

    internal Commands(Registers.Commands register, Data.Settings.Commands settings)
    {
        this.register = register;
        this.settings = settings;
    }

    private void ProcessCommand(Type commandType, ICaller caller, EExecutionSide executionSide, IReadOnlyList<string> arguments)
    {
        Main.Instance.ManualLogSource.LogInfo("ProcessCommand");

        ICommand command;

        try
        {
            command = CommandFactory(commandType);
        }
        catch (Exception Exception)
        {
            Main.Instance.ManualLogSource.LogError($"Failed to create an instance of the command {commandType.FullName}.\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
            return;
        }

        if (!command.CallerPolicy.IsAllowed(caller))
            return;

        if (!command.ExecutionPolicy.CanExecute(executionSide))
            return;

        try
        {
            command.Execute(
                new Context()
                {
                    Caller = caller,
                    Arguments = arguments
                }
            );
        }
        catch (Exception exception)
        {
            // log
        }

        if (command is IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception Exception)
            {
                // log
            }
        }
    }

    public bool ProcessCommand(ICaller caller, string input)
    {
        Main.Instance.ManualLogSource.LogInfo($"Message: {input}");
        bool result = parser.TryParse(settings.ClientPrefix, input, register.CommandNameMap, out string commandName, out IReadOnlyList<string> commandArguments);

        Console.WriteLine($"Arguments: [{string.Join(", ", commandArguments ?? [])}]");

        Main.Instance.ManualLogSource.LogInfo($"Command? {result}");

        if (result)
        {
            Type command = register.CommandNameMap[commandName];



            ProcessCommand(command, caller, EExecutionSide.MainPlayer, commandArguments);
        }

        return result;
    }
}
