using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Caller;

public sealed class Console : ICallerPolicy
{
    public bool IsAllowed(ICaller caller) => caller is Data.Commands.Callers.Console;
}
