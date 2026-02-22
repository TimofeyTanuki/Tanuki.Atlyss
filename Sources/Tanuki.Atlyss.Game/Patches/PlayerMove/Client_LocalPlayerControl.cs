using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.PlayerMove;

[HarmonyPatch(typeof(global::PlayerMove), "Client_LocalPlayerControl", MethodType.Normal)]
public sealed class Client_LocalPlayerControl
{
    public delegate void PrefixHandler(global::PlayerMove instance, ref bool runOriginal);

    private static PrefixHandler? onPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Client_LocalPlayerControl>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix, SuppressMessage("CodeQuality", "IDE0051")]
    private static bool Prefix(global::PlayerMove __instance)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;
        onPrefix?.Invoke(__instance, ref runOriginal);
        return runOriginal;
    }
}
