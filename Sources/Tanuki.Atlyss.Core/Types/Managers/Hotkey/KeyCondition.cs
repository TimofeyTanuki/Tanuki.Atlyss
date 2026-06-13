using System;
using UnityEngine;

namespace Tanuki.Atlyss.Core.Types.Managers.Hotkey;

public readonly struct KeyCondition(KeyCode keyCode, EKeyState keyState) : IEquatable<KeyCondition>, IComparable<KeyCondition>
{
    public readonly KeyCode Code = keyCode;
    public readonly EKeyState State = keyState;

    public int CompareTo(KeyCondition other)
    {
        int keyCodeComparison = Code.CompareTo(other.Code);

        if (keyCodeComparison != 0)
            return keyCodeComparison;

        return State.CompareTo(other.State);
    }

    public bool Equals(KeyCondition other) => Code == other.Code && State == other.State;

    public override bool Equals(object obj) => obj is KeyCondition other && Equals(other);

    public override int GetHashCode() => ((int)Code << 2) | (int)State;
}
