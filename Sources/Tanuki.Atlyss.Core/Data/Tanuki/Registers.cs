namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Registers
{
    public Core.Registers.Commands Commands { get; internal set; } = null!;
    public Core.Registers.Plugins Plugins { get; internal set; } = null!;

    internal Registers() { }
}
