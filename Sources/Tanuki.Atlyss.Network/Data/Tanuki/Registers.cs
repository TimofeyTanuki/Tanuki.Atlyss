namespace Tanuki.Atlyss.Network.Data.Tanuki;

public sealed class Registers
{
    internal Network.Registers.Packets packets = null!;

    public Network.Registers.Packets Packets => packets;

    internal Registers() { }
}
