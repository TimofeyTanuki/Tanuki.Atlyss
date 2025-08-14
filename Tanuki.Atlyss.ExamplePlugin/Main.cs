using BepInEx;
using BepInEx.Logging;

namespace Tanuki.Atlyss.ExamplePlugin;

[BepInPlugin("653a2c21-7d84-4fbb-94bd-c30fac5a45e3", "Tanuki.Atlyss.ExamplePlugin", "1.0.0")]
[BepInDependency("9c00d52e-10b8-413f-9ee4-bfde81762442", BepInDependency.DependencyFlags.HardDependency)]
public class Main : Core.Plugins.Plugin
{
    internal static Main Instance;
    internal ManualLogSource ManualLogSource;
    internal void Awake()
    {
        Instance = this;
        ManualLogSource = Logger;
        ManualLogSource.LogInfo("Awake()");
    }

    protected override void Load()
    {
        /*
         * Application of necessary patches.
         * They are applied once.
         */
        Game.Main.Instance.Patch(
            typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix),
            typeof(Game.Events.LoadSceneManager.Init_LoadScreenDisable_Postfix)
        );

        ManualLogSource.LogInfo("Load()");

        /*
         * Translations file path:
         * BepInEx/config/{Plugin directory}/{Current framework language}.translation.properties
         */
        ManualLogSource.LogInfo($"Translation: {Translate("Example")}");

        // Subscribe to events of used patches.
        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Send_ChatMessage_Prefix_OnInvoke;
    }
    protected override void Unload()
    {
        ManualLogSource.LogInfo("Unload()");

        /*
         * Unsubscribe from events.
         * This is necessary for the plugin to work correctly after reloading.
         * Command: /reload [plugin]
         */
        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke -= Send_ChatMessage_Prefix_OnInvoke;
    }

    private void Send_ChatMessage_Prefix_OnInvoke(string Message, ref bool ShouldAllow)
    {
        ManualLogSource.LogInfo("Send_ChatMessage_Prefix_OnInvoke");

        // Cancel sending a message if it contains a specific word.
        if (Message.Contains("example"))
            ShouldAllow = false;
    }
}