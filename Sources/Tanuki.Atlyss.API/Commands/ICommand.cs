namespace Tanuki.Atlyss.API.Commands;

public interface ICommand
{
    bool Execute(string[] Arguments);
}