using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ChatBehaviour;

[HarmonyPatch(typeof(global::ChatBehaviour), nameof(global::ChatBehaviour.Send_ChatMessage), MethodType.Normal)]
public sealed class Send_ChatMessage
{
    public delegate void PrefixHandler(string message, ref bool runOriginal);

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

    [HarmonyPrefix]
    private static bool Prefix(string _message)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;
        onPrefix.Invoke(_message, ref runOriginal);
        return runOriginal;
    }
}