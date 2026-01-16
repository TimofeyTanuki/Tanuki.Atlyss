using System;

namespace Tanuki.Atlyss.Game.Extensions;

public static class Player
{
    public enum ENicknameType
    {
        Default,
        Global,
        Any
    }

    extension(global::Player)
    {
        public static global::Player? GetByAutoRecognition(
            string input,
            ENicknameType nicknameType = ENicknameType.Any,
            bool nicknameStrictLength = false,
            StringComparison nicknameStrictComparsion = StringComparison.InvariantCultureIgnoreCase
        ) =>
            uint.TryParse(input, out uint netId) ? Providers.Player.Instance.GetByNetID(netId) :
            ulong.TryParse(input, out ulong steamId) ? Providers.Player.Instance.GetBySteamId(steamId) :
            GetByNickname(input, nicknameType, nicknameStrictLength, nicknameStrictComparsion);

        public static global::Player? GetByNickname(
            string nickname,
            ENicknameType nicknameType = ENicknameType.Any,
            bool strictLength = false,
            StringComparison stringComparsion = StringComparison.InvariantCultureIgnoreCase
        ) =>
            nicknameType switch
            {
                ENicknameType.Default => Providers.Player.Instance.GetByDefaultNickname(nickname, strictLength, stringComparsion),
                ENicknameType.Global => Providers.Player.Instance.GetByGlobalNickname(nickname, strictLength, stringComparsion),
                ENicknameType.Any => Providers.Player.Instance.GetByAnyNickname(nickname, strictLength, stringComparsion),
                _ => null,
            };
    }
}