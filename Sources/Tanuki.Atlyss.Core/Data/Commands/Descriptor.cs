namespace Tanuki.Atlyss.Core.Data.Commands;

public sealed class Descriptor
{
    public readonly ulong Hash;
    internal Serialization.Commands.Configuration? configuration;

    public Serialization.Commands.Configuration? Configuration => configuration;

    internal Descriptor(ulong hash, Serialization.Commands.Configuration? configuration)
    {
        Hash = hash;
        this.configuration = configuration;
    }
}
