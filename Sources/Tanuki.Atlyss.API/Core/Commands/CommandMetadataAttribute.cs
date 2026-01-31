using System;

namespace Tanuki.Atlyss.API.Core.Commands;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CommandMetadataAttribute : Attribute
{
    private readonly EExecutionSide executionSide;
    public EExecutionSide ExecutionSide => executionSide;

    private readonly Type? callerPolicyType;
    public Type? CallerPolicy => callerPolicyType;

    public CommandMetadataAttribute(EExecutionSide executionSide, Type? callerPolicyType = null)
    {
        this.executionSide = executionSide;

        if (callerPolicyType is not null)
        {
            if (!typeof(ICallerPolicy).IsAssignableFrom(callerPolicyType))
                throw new ArgumentException($"{callerPolicyType!.FullName} must implement {nameof(ICallerPolicy)}");

            this.callerPolicyType = callerPolicyType;
        }
    }
}
