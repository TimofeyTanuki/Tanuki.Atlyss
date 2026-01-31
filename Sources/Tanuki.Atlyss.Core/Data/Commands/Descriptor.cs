using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Data.Commands;

public sealed class Descriptor
{
    internal Serialization.Commands.Configuration? configuration;
    internal EExecutionSide executionSide;
    internal ICallerPolicy callerPolicy;

    public readonly ulong Hash;
    public Serialization.Commands.Configuration? Configuration => configuration;

    internal Descriptor(ulong hash, EExecutionSide executionSide, ICallerPolicy callerPolicy, Serialization.Commands.Configuration? configuration)
    {
        Hash = hash;
        this.executionSide = executionSide;
        this.callerPolicy = callerPolicy;
        this.configuration = configuration;
    }
}
