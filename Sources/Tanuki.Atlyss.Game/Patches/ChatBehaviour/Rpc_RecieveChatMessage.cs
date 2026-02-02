using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.ChatBehaviour;

[HarmonyPatch(typeof(global::ChatBehaviour), "Rpc_RecieveChatMessage", MethodType.Normal)]
public sealed class Rpc_RecieveChatMessage
{
    public delegate void PrefixHandler(global::ChatBehaviour instance, string message, bool isEmoteMessage, global::ChatBehaviour.ChatChannel chatChannel, ref bool runOriginal);

    private static PrefixHandler? onPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<Rpc_RecieveChatMessage>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix, SuppressMessage("CodeQuality", "IDE0051")]
    private static bool Prefix(global::ChatBehaviour __instance, string message, bool _isEmoteMessage, global::ChatBehaviour.ChatChannel _chatChannel)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;
        onPrefix.Invoke(__instance, message, _isEmoteMessage, _chatChannel, ref runOriginal);
        return runOriginal;
    }
}
