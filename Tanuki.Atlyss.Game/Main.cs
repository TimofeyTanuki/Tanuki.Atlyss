using HarmonyLib;
using System.Reflection;

namespace Tanuki.Atlyss.Game;

public class Main
{
    public const string HarmonyID = "Tanuki.Atlyss.Game";
    public static bool Patched { get; private set; }
    public static void Patch()
    {
        if (Patched)
            return;

        Patched = true;
        Harmony h = new(HarmonyID);
        h.patch
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}