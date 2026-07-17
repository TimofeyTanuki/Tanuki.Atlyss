using System;

namespace Tanuki.Atlyss.Game.Types.Player;

[Flags]
public enum ENicknameType
{
    Default = 1 << 0,
    Global = 1 << 1,
    Any = Default | Global
}
