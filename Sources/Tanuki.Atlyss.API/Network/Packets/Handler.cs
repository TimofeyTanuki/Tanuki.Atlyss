using Steamworks;

namespace Tanuki.Atlyss.API.Network.Packets;

public delegate void Handler<T>(CSteamID sender, T packet);
