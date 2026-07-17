namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Registers
{
    private Core.Registers.Commands commands = null!;
    private Core.Registers.Plugins plugins = null!;

    public Core.Registers.Commands Commands
    {
        get => commands;
        internal set => commands = value;
    }
    public Core.Registers.Plugins Plugins
    {
        get => plugins;
        internal set => plugins = value;
    }

    internal Registers() { }
}
