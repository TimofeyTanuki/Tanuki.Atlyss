using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Providers;

public sealed class Commands
{
    private static readonly ConcurrentDictionary<Type, Func<ICommand>> factories = [];

    internal Commands() { }

    public ICommand Create(Type commandType)
    {
        Func<ICommand> factory = factories.GetOrAdd(commandType, CreateFactory);
        return factory();
    }

    private static Func<ICommand> CreateFactory(Type type)
    {
        Expression expression = Expression.New(type);
        Expression<Func<ICommand>> lambda = Expression.Lambda<Func<ICommand>>(expression);
        return lambda.Compile();
    }
}
