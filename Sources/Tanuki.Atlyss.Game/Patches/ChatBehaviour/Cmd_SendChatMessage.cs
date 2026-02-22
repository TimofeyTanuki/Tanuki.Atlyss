using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ChatBehaviour;

[HarmonyPatch(typeof(global::ChatBehaviour), nameof(global::ChatBehaviour.Cmd_SendChatMessage), MethodType.Normal)]
public sealed class Cmd_SendChatMessage
{
    public delegate void PrefixHandler(string message, global::ChatBehaviour.ChatChannel chatChannel, ref bool runOriginal);

    private static PrefixHandler? onPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Cmd_SendChatMessage>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix]
    private static bool Prefix(string _message, global::ChatBehaviour.ChatChannel _chatChannel)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;
        onPrefix.Invoke(_message, _chatChannel, ref runOriginal);
        return runOriginal;
    }
}
