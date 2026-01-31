using System;

namespace Tanuki.Atlyss.API.Core.Commands;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CommandMetadataAttribute : Attribute
{
    private readonly EExecutionType executionType;
    public EExecutionType ExecutionType => executionType;

    private readonly Type? callerPolicyType;
    public Type? CallerPolicy => callerPolicyType;

    public CommandMetadataAttribute(EExecutionType executionType, Type? callerPolicyType = null)
    {
        this.executionType = executionType;

        if (callerPolicyType is not null)
        {
            if (!typeof(ICallerPolicy).IsAssignableFrom(callerPolicyType))
                throw new ArgumentException($"{callerPolicyType!.FullName} must implement {nameof(ICallerPolicy)}");

            this.callerPolicyType = callerPolicyType;
        }
    }
}
