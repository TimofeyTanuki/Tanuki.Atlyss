using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.ChatBehaviour;

[HarmonyPatch(typeof(global::ChatBehaviour), "UserCode_Cmd_SendChatMessage__String__ChatChannel", MethodType.Normal)]
public sealed class UserCode_Cmd_SendChatMessage__String__ChatChannel
{
    public delegate void PrefixHandler(global::ChatBehaviour instance, string message, ref bool runOriginal);

    private static PrefixHandler? onPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<Send_ChatMessage>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix, SuppressMessage("CodeQuality", "IDE0051")]
    private static bool Prefix(global::ChatBehaviour __instance, string _message, global::ChatBehaviour.ChatChannel _chatChannel)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;
        onPrefix.Invoke(__instance, _message, ref runOriginal);
        return runOriginal;
    }
}