using System;

namespace Tanuki.Atlyss.Game.Extensions;

public static class Player
{
    private static global::Player[] GetPlayers() =>
        UnityEngine.Object.FindObjectsOfType<global::Player>();

    public enum NicknameType
    {
        Default,
        Global,
        Any
    }

    private static bool NicknameMatches(string Nickname, string Match, bool StrictLength, StringComparison StringComparsion)
    {
        if (Nickname.IndexOf(Match, StringComparsion) < 0)
            return false;

        if (StrictLength &&
            Match.Length != Nickname.Length)
            return false;

        return true;
    }

    extension(global::Player)
    {
        public static global::Player GetByNetID(uint NetID)
        {
            foreach (global::Player Player in GetPlayers())
                if (Player.netId == NetID)
                    return Player;

            return null;
        }

        public static global::Player GetBySteamID(ulong SteamID)
        {
            string Match = SteamID.ToString();

            foreach (global::Player Player in GetPlayers())
                if (Player._steamID == Match)
                    return Player;

            return null;
        }

        public static global::Player GetByNickname(string Nickname, NicknameType NicknameType = NicknameType.Any, bool StrictLength = false, StringComparison StringComparsion = StringComparison.InvariantCultureIgnoreCase)
        {
            switch (NicknameType)
            {
                case NicknameType.Default:
                    return GetByDefaultNickname(Nickname, StrictLength, StringComparsion);
                case NicknameType.Global:
                    return GetByGlobalNickname(Nickname, StrictLength, StringComparsion);
                case NicknameType.Any:
                    return GetByAnyNickname(Nickname, StrictLength, StringComparsion);
                default:
                    break;
            }

            return null;
        }

        public static global::Player GetByDefaultNickname(string Nickname, bool StrictLength, StringComparison StringComparsion)
        {
            foreach (global::Player Player in GetPlayers())
                if (NicknameMatches(Player._nickname, Nickname, StrictLength, StringComparsion))
                    return Player;

            return null;
        }

        public static global::Player GetByGlobalNickname(string Nickname, bool StrictLength, StringComparison StringComparsion)
        {
            foreach (global::Player Player in GetPlayers())
                if (NicknameMatches(Player._globalNickname, Nickname, StrictLength, StringComparsion))
                    return Player;

            return null;
        }

        public static global::Player GetByAnyNickname(string Nickname, bool StrictLength, StringComparison StringComparsion)
        {
            foreach (global::Player Player in GetPlayers())
                if (NicknameMatches(Player._nickname, Nickname, StrictLength, StringComparsion) ||
                    NicknameMatches(Player._globalNickname, Nickname, StrictLength, StringComparsion))
                    return Player;

            return null;
        }
    }
}