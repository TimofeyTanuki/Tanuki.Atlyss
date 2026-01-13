using System;

namespace Tanuki.Atlyss.Network;

public class Main
{
    public static void Initialize()
    {
        Console.WriteLine("Tanuki.Atlyss.Network initialization...");
        Managers.Network.Initialize();
    }
}