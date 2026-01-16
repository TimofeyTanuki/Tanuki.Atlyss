namespace Tanuki.Atlyss.API.Tanuki.Commands;

public interface IExecutionPolicy
{
    public bool CanExecute(EExecutionSide executionSide);
}
