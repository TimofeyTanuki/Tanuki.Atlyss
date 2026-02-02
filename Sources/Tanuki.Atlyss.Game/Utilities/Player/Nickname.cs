using System;

namespace Tanuki.Atlyss.Game.Utilities.Player;

public static class Nickname
{
    public static bool Match(string nickname, string match, bool strictLength, StringComparison stringComparsion)
    {
        if (nickname.IndexOf(match, stringComparsion) < 0)
            return false;

        if (strictLength && match.Length != nickname.Length)
            return false;

        return true;
    }
}
