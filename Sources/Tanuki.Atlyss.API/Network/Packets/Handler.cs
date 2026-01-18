using System.Collections.Generic;

namespace Tanuki.Atlyss.API.Network.Packets;

public delegate void Handler(IReadOnlyCollection<byte> data);
