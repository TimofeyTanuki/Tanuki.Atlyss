namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Registers
{
    public Core.Registers.Commands commands = null!;
    public Core.Registers.Plugins plugins = null!;

    public Core.Registers.Commands Commands => commands;
    public Core.Registers.Plugins Plugins => plugins;

    internal Registers() { }
}
