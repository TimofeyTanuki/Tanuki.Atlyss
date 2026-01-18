using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Caller;

internal class Player : ICallerPolicy
{
    public bool IsAllowed(ICaller caller) => caller is Data.Commands.Callers.Player;
}
