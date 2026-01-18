using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Policies.Commands.Caller;

internal class Authorized : ICallerPolicy
{
    public bool IsAllowed(ICaller caller)
    {
        if (caller is Data.Commands.Callers.Console)
            return true;

        if (caller is not Data.Commands.Callers.Player player)
            return false;

        // check permission for player.player
        System.Console.WriteLine($"Check permission for {player.player._nickname}");

        return true;
    }
}
