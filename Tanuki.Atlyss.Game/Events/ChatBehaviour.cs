using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events;

public static class ChatBehaviour
{
    [HarmonyPatch(typeof(global::ChatBehaviour), "Send_ChatMessage", MethodType.Normal)]
    public static class Send_ChatMessage
    {
        public delegate void EventHandler(string Message, ref bool ShouldAllow);
        public static event EventHandler Before;

        internal static bool Prefix(string _message)
        {
            if (string.IsNullOrEmpty(_message))
                return true;

            bool ShouldAllow = true;
            Before?.Invoke(_message, ref ShouldAllow);

            if (ShouldAllow)
                return true;

            global::ChatBehaviour._current._chatAssets._chatInput.text = string.Empty;
            global::ChatBehaviour._current.Display_Chat(false);

            return false;
        }
    }
}