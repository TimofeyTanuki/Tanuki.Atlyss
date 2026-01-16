using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Execution;

internal class Player : IExecutionPolicy
{
    bool IExecutionPolicy.CanExecute(EExecutionSide executionSide) =>
        executionSide.HasFlag(EExecutionSide.MainPlayer | EExecutionSide.Player);
}
