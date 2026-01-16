namespace Tanuki.Atlyss.Core.Data.Commands;

public sealed class RegistryEntry
{
    internal ulong hash;
    internal Serialization.Commands.Configuration? configuration;

    public ulong Hash => hash;
    public Serialization.Commands.Configuration? Configuration => configuration;

    internal RegistryEntry(ulong hash, Serialization.Commands.Configuration? configuration)
    {
        this.hash = hash;
        this.configuration = configuration;
    }
}
