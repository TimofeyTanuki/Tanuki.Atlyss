using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Caller;

internal class Free : ICallerPolicy
{
    public bool IsAllowed(ICaller caller) => true;
}
