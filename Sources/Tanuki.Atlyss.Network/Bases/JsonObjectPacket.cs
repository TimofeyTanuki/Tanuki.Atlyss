using Newtonsoft.Json;
using System;
using System.Text;
using Tanuki.Atlyss.API.Network.Packets;

namespace Tanuki.Atlyss.Network.Bases;

/// <summary>
/// A packet based on the JSON format. The object and its fields must be serializable using <see cref="JsonConvert.SerializeObject(object?)"/>.
/// </summary>
/// <remarks>
/// This is a packet for lazy people who don't want to deal with manual serialization.
/// </remarks>
public abstract class JsonObjectPacket<T> : Packet where T : JsonObjectPacket<T>
{
    private static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.None,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };

    private byte[]? serialized;
    private int serializedLength;

    private void SerializeData()
    {
        serialized ??= Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((T)this, jsonSerializerSettings));
        serializedLength = serialized.Length;
    }

    public override int GetMaxSize()
    {
        SerializeData();

        return serializedLength;
    }

    public override int Serialize(Span<byte> buffer)
    {
        SerializeData();

        if (serialized is null)
            return 0;

        serialized.CopyTo(buffer);
        serialized = null;

        return serializedLength;
    }

    public override void Deserialize(ReadOnlySpan<byte> data)
    {
        serialized = null;
        JsonConvert.PopulateObject(Encoding.UTF8.GetString(data), this, jsonSerializerSettings);
    }
}
