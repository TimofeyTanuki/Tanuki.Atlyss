using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Caller;

internal class Console : ICallerPolicy
{
    public bool IsAllowed(ICaller caller) => caller is Data.Commands.Callers.Console;
}
