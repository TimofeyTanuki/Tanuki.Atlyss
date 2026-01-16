using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.PlayerVisual;

[HarmonyPatch(typeof(global::PlayerVisual), nameof(global::PlayerVisual.Apply_NetworkedCharacterDisplay), MethodType.Normal)]
public sealed class Apply_NetworkedCharacterDisplay
{
    public delegate void PrefixHandler(global::PlayerVisual instance, ref bool runOriginal);

    private static PrefixHandler? onPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<Apply_NetworkedCharacterDisplay>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix]
    private static bool Prefix(global::PlayerVisual __instance)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;
        onPrefix.Invoke(__instance, ref runOriginal);
        return runOriginal;
    }
}