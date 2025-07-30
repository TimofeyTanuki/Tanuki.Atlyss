namespace Tanuki.Atlyss.API.Commands;

public interface ICommand
{
    public void Execute(string[] Arguments);
}