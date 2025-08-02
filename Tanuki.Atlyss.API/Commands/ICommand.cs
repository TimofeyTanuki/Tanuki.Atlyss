namespace Tanuki.Atlyss.API.Commands;

public interface ICommand
{
    void Execute(string[] Arguments);
}