using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ChatBehaviour;


[HarmonyPatch(typeof(global::ChatBehaviour), nameof(global::ChatBehaviour.Send_ChatMessage), MethodType.Normal)]
public static class Send_ChatMessage_Prefix
{
    public delegate void EventHandler(string Message, ref bool ShouldAllow);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static bool Prefix(string _message)
    {
        if (string.IsNullOrEmpty(_message))
            return true;

        bool ShouldAllow = true;
        OnInvoke?.Invoke(_message, ref ShouldAllow);

        if (ShouldAllow)
            return true;

        global::ChatBehaviour._current._chatAssets._chatInput.text = string.Empty;
        global::ChatBehaviour._current.Display_Chat(false);

        return false;
    }
#pragma warning restore IDE0051
}