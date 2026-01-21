namespace Tanuki.Atlyss.Network.Data.Tanuki;

public sealed class Managers
{
    internal Network.Managers.Packets packets = null!;
    internal Network.Managers.Network network = null!;

    public Network.Managers.Packets Packets => packets;
    public Network.Managers.Network Network => network;

    internal Managers() { }
}
