using Newtonsoft.Json;
using System;
using System.Text;
using Tanuki.Atlyss.API.Network.Packets;

namespace Tanuki.Atlyss.Network.Bases;

/*
 * JOPa
 */
public abstract class JsonObjectPacket<T> : Packet
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

    public T? Data;

    private void SerializeData()
    {
        serialized ??= Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Data, jsonSerializerSettings));
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
        Data = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data), jsonSerializerSettings);
    }
}
