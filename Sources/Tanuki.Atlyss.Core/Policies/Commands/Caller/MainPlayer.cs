using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Caller;

public class MainPlayer : ICallerPolicy
{
    public bool IsAllowed(ICaller caller)
    {
        if (caller is not Data.Commands.Callers.Player player)
            return false;

        return player.player == global::Player._mainPlayer;
    }
}
