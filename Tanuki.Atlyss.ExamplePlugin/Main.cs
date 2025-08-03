using BepInEx;

namespace Tanuki.Atlyss.ExamplePlugin;

[BepInPlugin("653a2c21-7d84-4fbb-94bd-c30fac5a45e3", "Tanuki.Atlyss.ExamplePlugin", "1.0.0")]
[BepInProcess("ATLYSS.exe")]
public class Main : Core.Plugins.Plugin
{
    internal static Main Instance;
    internal void Awake()
    {
        Instance = this;
        Logger.LogInfo("Awake()");

        /*
         * Application of necessary patches.
         * They are applied once.
         */
        Game.Main.Instance.Patch(
            typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix),
            typeof(Game.Events.LoadSceneManager.Init_LoadScreenDisable_Postfix)
        );
    }

    protected override void Load()
    {
        Logger.LogInfo("Load()");

        /*
         * Translations file path:
         * BepInEx/config/{Plugin directory}/{Current framework language}.translation.properties
         */
        Logger.LogInfo($"Translation: {Translate("Example")}");

        // Subscribe to events of used patches.
        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Send_ChatMessage_Prefix_OnInvoke;
        Game.Events.LoadSceneManager.Init_LoadScreenDisable_Postfix.OnInvoke += Init_LoadScreenDisable_Postfix_OnInvoke;
    }
    protected override void Unload()
    {
        Logger.LogInfo("Unload()");

        /*
         * Unsubscribe from events.
         * This is necessary for the plugin to work correctly after reloading.
         * Command: /reload [plugin]
         */
        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke -= Send_ChatMessage_Prefix_OnInvoke;
        Game.Events.LoadSceneManager.Init_LoadScreenDisable_Postfix.OnInvoke -= Init_LoadScreenDisable_Postfix_OnInvoke;
    }

    private void Send_ChatMessage_Prefix_OnInvoke(string Message, ref bool ShouldAllow)
    {
        Logger.LogInfo("Send_ChatMessage_Prefix_OnInvoke");

        // Cancel sending a message if it contains a specific word.
        if (Message.Contains("example"))
            ShouldAllow = false;
    }
    private void Init_LoadScreenDisable_Postfix_OnInvoke() =>
        Logger.LogInfo("Init_LoadScreenDisable_Postfix_OnInvoke");
}