using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Data.Commands;

public sealed class Descriptor
{
    internal Serialization.Commands.Configuration? configuration;
    internal EExecutionType executionType;
    internal ICallerPolicy callerPolicy;

    public readonly ulong Hash;
    public Serialization.Commands.Configuration? Configuration => configuration;

    internal Descriptor(ulong hash, EExecutionType executionType, ICallerPolicy callerPolicy, Serialization.Commands.Configuration? configuration)
    {
        Hash = hash;
        this.executionType = executionType;
        this.callerPolicy = callerPolicy;
        this.configuration = configuration;
    }
}
