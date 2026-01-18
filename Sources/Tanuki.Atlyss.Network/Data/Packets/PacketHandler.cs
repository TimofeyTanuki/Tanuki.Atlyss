using System.Collections.Generic;

namespace Tanuki.Atlyss.Network.Data.Packets;

public delegate void Handler(IReadOnlyCollection<byte> data);
