using Tanuki.Atlyss.API.Commands;

namespace Tanuki.Atlyss.Bootstrap.Commands;

public class Reload : ICommand
{
    public void Execute(string[] Arguments)
    {
        Core.Tanuki.Instance.Plugins.Reload();
    }
}