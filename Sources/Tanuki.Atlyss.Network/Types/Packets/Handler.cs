using System.Collections.Generic;

namespace Tanuki.Atlyss.Network.Types.Packets;

public delegate void Handler(IReadOnlyCollection<byte> data);
