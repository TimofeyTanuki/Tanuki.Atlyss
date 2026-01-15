using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tanuki.Atlyss.Network.Models;

internal class PacketHeader(CSteamID Sender, uint TargetNetID = 0)
{
    public readonly CSteamID Sender = Sender;
    public readonly uint TargetNetID = TargetNetID;
}