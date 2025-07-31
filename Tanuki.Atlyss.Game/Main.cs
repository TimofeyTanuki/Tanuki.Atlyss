using HarmonyLib;
using System.Reflection;

namespace Tanuki.Atlyss.Game;

public class Main
{
    public static bool Patched { get; private set; }
    public static void Patch()
    {
        if (Patched)
            return;

        Patched = true;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}