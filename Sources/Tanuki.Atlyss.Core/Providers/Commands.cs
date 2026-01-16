using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Providers;

public sealed class Commands
{
    private readonly Registers.Commands commandRegistry;
    private readonly ConcurrentDictionary<Type, Func<ICommand>> factories = [];

    internal Commands(Registers.Commands commandRegistry) => this.commandRegistry = commandRegistry;

    public ICommand Create(Type commandType)
    {
        if (!commandRegistry.Entries.ContainsKey(commandType))
            throw new ArgumentException($"Type {commandType.FullName} not found in the command pluginRegistry.");

        Func<ICommand> factory = factories.GetOrAdd(commandType, CreateFactory);

        return factory();
    }

    private static Func<ICommand> CreateFactory(Type type)
    {
        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException($"{type.FullName} must have a parameterless constructor.");

        Expression expression = Expression.New(constructor);
        Expression<Func<ICommand>> lambda = Expression.Lambda<Func<ICommand>>(expression);
        return lambda.Compile();
    }
}
