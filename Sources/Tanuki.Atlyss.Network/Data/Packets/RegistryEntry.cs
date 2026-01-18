using System.Collections.Generic;
using Tanuki.Atlyss.API.Network.Compression;

namespace Tanuki.Atlyss.Network.Data.Packets;

public sealed class RegistryEntry(ulong signature)
{
    internal ulong Signature = signature;
    internal ICompressionProvider? CompressionProvider;
    internal List<API.Network.Packets.Handler> PacketHandlers = [];
}
