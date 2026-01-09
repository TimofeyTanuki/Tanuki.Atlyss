using BepInEx;
using BepInEx.Logging;
using System.Linq;
using Tanuki.Atlyss.API;

namespace Tanuki.Atlyss.ExamplePlugin;

[BepInPlugin("653a2c21-7d84-4fbb-94bd-c30fac5a45e3", "Tanuki.Atlyss.ExamplePlugin", "1.0.0")]
[BepInDependency("9c00d52e-10b8-413f-9ee4-bfde81762442", BepInDependency.DependencyFlags.HardDependency)]
public class Main : Core.Plugins.Plugin
{
    internal static Main Instance;
    internal ManualLogSource ManualLogSource;
    internal Patching.Patcher Patcher;

    internal void Awake()
    {
        Instance = this;
        ManualLogSource = Logger;
        ManualLogSource.LogInfo("Awake()");
        Patcher = new();

        Core.Tanuki.Instance.Plugins.OnBeforePluginsLoad += Plugins_OnBeforePluginsLoad;
    }

    private void Plugins_OnBeforePluginsLoad()
    {
        IPlugin OtherTanukiPlugin = Core.Tanuki.Instance.Plugins.Plugins.Where(x => x.Name == "OtherPluginName").First();
        if (OtherTanukiPlugin is null)
            return;

        OtherTanukiPlugin.OnLoaded += OtherTanukiPlugin_OnLoaded;
    }

    private void OtherTanukiPlugin_OnLoaded()
    {
        // Actions after other plugin is loaded
    }

    protected override void Load()
    {
        /*
         * Application of necessary patches.
         * They are applied once.
         */
        Patcher.Use(
            typeof(Game.Patches.ChatBehaviour.Send_ChatMessage_Prefix),
            typeof(Game.Patches.LoadSceneManager.DeserializeSyncVars_Postfix)
        );

        ManualLogSource.LogInfo("Load()");

        /*
         * Translations file path:
         * BepInEx/config/{Plugin directory}/{Current framework language}.translation.properties
         */
        ManualLogSource.LogInfo($"Translation: {Translate("Example")}");

        // Subscribe to events of used patches.
        Game.Patches.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Send_ChatMessage_Prefix_OnInvoke;
    }
    protected override void Unload()
    {
        ManualLogSource.LogInfo("Unload()");

        /*
         * Unsubscribe from events.
         * This is necessary for the plugin to work correctly after reloading.
         * Command: /reload [plugin]
         */
        Game.Patches.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke -= Send_ChatMessage_Prefix_OnInvoke;



        /*
         * Remove all patches used by this plugin (patcher).
         * If a patch is used by other plugins, it will not be removed.
         */
        Patcher.UnuseAll();

        /*
         * ...
         * or
         * manual removal of patches.
         */
        /*
        Patcher.Unuse(
            typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix),
            typeof(Game.Events.LoadSceneManager.Init_LoadScreenDisable_Postfix)
        );
        */

        /*
         * Removing patches is not performative.
         * It causes all patches (except those that have been removed) to be repatched.
         */
    }

    private void Send_ChatMessage_Prefix_OnInvoke(string Message, ref bool ShouldAllow)
    {
        ManualLogSource.LogInfo("Send_ChatMessage_Prefix_OnInvoke");

        // Cancel sending a message if it contains a specific word.
        if (Message.Contains("example"))
            ShouldAllow = false;
    }
}