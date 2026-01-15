namespace Tanuki.Atlyss.API.Commands;

public interface ICommand
{
    public EAllowedCaller AllowedCaller { get; }
    public EExecutionSide ExecutionSide { get; }

    public bool Execute(ICaller Caller, Context Context);
}