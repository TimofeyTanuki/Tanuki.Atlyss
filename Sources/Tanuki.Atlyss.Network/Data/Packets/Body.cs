namespace Tanuki.Atlyss.Network.Data.Packets;

public abstract class Body
{
    public abstract byte[] Serialize();
    public abstract void Deserialize(byte[] Data);
}
