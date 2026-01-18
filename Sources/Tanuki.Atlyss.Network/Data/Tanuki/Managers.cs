namespace Tanuki.Atlyss.Network.Data.Tanuki;

public sealed class Managers
{
    internal Network.Managers.Packets packets = null!;

    public Network.Managers.Packets Packets => packets;

    internal Managers() { }
}
