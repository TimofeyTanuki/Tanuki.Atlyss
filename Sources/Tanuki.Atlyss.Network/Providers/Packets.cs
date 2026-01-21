using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Tanuki.Atlyss.API.Network.Packets;

namespace Tanuki.Atlyss.Network.Providers;

public sealed class Packets
{
    private static readonly ConcurrentDictionary<Type, Func<Packet>> factories = [];

    public Packet Create(Type type)
    {
        Func<Packet> factory = factories.GetOrAdd(type, CreateFactory);
        return factory();
    }

    public TPacket Create<TPacket>()
        where TPacket : Packet =>
        (TPacket)Create(typeof(TPacket));

    private Func<Packet> CreateFactory(Type type)
    {
        Expression expression = Expression.New(type);
        Expression<Func<Packet>> lambda = Expression.Lambda<Func<Packet>>(expression);
        return lambda.Compile();
    }
}
