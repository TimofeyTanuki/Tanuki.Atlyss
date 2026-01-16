namespace Tanuki.Atlyss.API.Tanuki.Commands;

public interface ICommand
{
    public ICallerPolicy CallerPolicy { get; }
    public IExecutionPolicy ExecutionPolicy { get; }

    public bool Execute(IContext context);
}
