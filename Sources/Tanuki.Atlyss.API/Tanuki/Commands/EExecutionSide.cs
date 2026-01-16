using System;

namespace Tanuki.Atlyss.API.Tanuki.Commands;

[Flags]
public enum EExecutionSide
{
    Player = 0,
    MainPlayer = 1 << 0,
    Console = 2 << 1
}
