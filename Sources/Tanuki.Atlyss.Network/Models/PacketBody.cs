namespace Tanuki.Atlyss.Network.Models;

public abstract class PacketBody
{
    public abstract byte[] Serialize();
    public abstract void Deserialize(byte[] Data);
}