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
            uint.TryParse(input, out uint netId) ? Providers.Player.Instance.FindByNetId(netId) :
            ulong.TryParse(input, out ulong steamId) ? Providers.Player.Instance.FindBySteamId(steamId) :
            GetByNickname(input, nicknameType, nicknameStrictLength, nicknameStrictComparsion);

        public static global::Player? GetByNickname(
            string nickname,
            ENicknameType nicknameType = ENicknameType.Any,
            bool strictLength = false,
            StringComparison stringComparsion = StringComparison.InvariantCultureIgnoreCase
        ) =>
            nicknameType switch
            {
                ENicknameType.Default => Providers.Player.Instance.FindByDefaultNickname(nickname, strictLength, stringComparsion),
                ENicknameType.Global => Providers.Player.Instance.FindByGlobalNickname(nickname, strictLength, stringComparsion),
                ENicknameType.Any => Providers.Player.Instance.FindByAnyNickname(nickname, strictLength, stringComparsion),
                _ => null,
            };
    }
}