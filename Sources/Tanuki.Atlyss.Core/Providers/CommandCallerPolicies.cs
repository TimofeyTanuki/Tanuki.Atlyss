using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Providers;

public sealed class CommandCallerPolicies
{
    private static readonly Dictionary<Type, ICallerPolicy> callerPolicies = [];

    internal CommandCallerPolicies() { }

    public ICallerPolicy GetOrCreate(Type type)
    {
        if (callerPolicies.TryGetValue(type, out ICallerPolicy callerPolicy))
            return callerPolicy;

        callerPolicy = (ICallerPolicy)Activator.CreateInstance(type);

        callerPolicies[type] = callerPolicy;

        return callerPolicy;
    }

    public ICallerPolicy GetOrCreate<T>() => GetOrCreate(typeof(T));
}
