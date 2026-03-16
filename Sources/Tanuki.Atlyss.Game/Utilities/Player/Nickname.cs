using System;

namespace Tanuki.Atlyss.Game.Utilities.Player;

public static class Nickname
{
    private static bool TryNullMatch(string? nickname, string? match, out bool result)
    {
        if (nickname == null || match == null)
        {
            result = nickname == match;
            return true;
        }

        result = default;
        return false;
    }

    public static bool SimilarMatch(string? nickname, string? match, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        if (TryNullMatch(nickname, match, out bool result))
            return result;

        return nickname!.IndexOf(match, stringComparison) >= 0;
    }

    public static bool ExactMatch(string? nickname, string? match, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        if (TryNullMatch(nickname, match, out bool result))
            return result;

        return string.Equals(nickname, match, stringComparison);
    }

    public static bool Match(string? nickname,
        string? match,
        bool exactMatch = false,
        StringComparison stringComparison = StringComparison.OrdinalIgnoreCase
    ) =>
        exactMatch ? ExactMatch(nickname, match, stringComparison) : SimilarMatch(nickname, match, stringComparison);
}
