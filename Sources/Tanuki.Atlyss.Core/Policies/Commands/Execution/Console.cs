using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Execution;

internal class Console : IExecutionPolicy
{
    public bool CanExecute(EExecutionSide executionSide) =>
        executionSide is EExecutionSide.Console;
}
