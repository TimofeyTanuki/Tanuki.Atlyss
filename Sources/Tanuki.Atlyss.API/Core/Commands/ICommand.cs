namespace Tanuki.Atlyss.API.Core.Commands;

public interface ICommand
{
    public void Execute(IContext context) { }
}
