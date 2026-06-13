namespace Tanuki.Atlyss.Network.Types.Tanuki;

public sealed class Registers
{
    internal Network.Registers.Packets packets = null!;

    public Network.Registers.Packets Packets => packets;

    internal Registers() { }
}
