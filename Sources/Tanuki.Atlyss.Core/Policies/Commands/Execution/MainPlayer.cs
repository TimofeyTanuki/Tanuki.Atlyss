using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Execution;

internal class MainPlayer : IExecutionPolicy
{
    bool IExecutionPolicy.CanExecute(EExecutionSide executionSide) =>
        executionSide is EExecutionSide.MainPlayer;
}
