namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Routers
{
    private Core.Routers.Commands commands = null!;

    public Core.Routers.Commands Commands
    {
        get => commands;
        internal set => commands = value;
    }

    internal Routers() { }
}
