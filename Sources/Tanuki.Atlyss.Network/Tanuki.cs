using System;
using System.Collections.Generic;
using System.Text;

namespace Tanuki.Atlyss.Network;

public sealed class Tanuki
{
    public static Tanuki Instance = null!;

    public readonly Managers.Steam Steam;
    public readonly Managers.Network Network;

    internal Tanuki()
    {
        Steam = new();
        Network = new();
    }

    public static void Initialize() => Instance ??= new();
}