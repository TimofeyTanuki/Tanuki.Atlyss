namespace Tanuki.Atlyss.API.Tanuki.Commands;

public interface ICommand
{
    public EExecutionType ExecutionType { get; }
    public ICallerPolicy CallerPolicy { get; }

    public void ClientCallback(IContext context) { }

    public void ServerCallback(IContext context) { }
}
