using System;

namespace Tanuki.Atlyss.ExamplePlugin.Commands;

internal class DisposableExample : API.Commands.ICommand, IDisposable
{
    private bool Subscribed = false;
    public bool Execute(string[] Arguments)
    {
        if (Subscribed)
            return false;

        Player._mainPlayer._pVisual.Cmd_VanitySparkleEffect();

        Subscribed = true;
        Game.Events.LoadSceneManager.Init_LoadScreenDisable_Postfix.OnInvoke += Init_LoadScreenDisable_Postfix_OnInvoke;

        return false;
    }
    private void Init_LoadScreenDisable_Postfix_OnInvoke() =>
        Main.Instance.ManualLogSource.LogInfo("Init_LoadScreenDisable_Postfix_OnInvoke();");

    /*
     * When the plugin is reloaded, a new instance of the command is created.
     * Make sure you are not creating memory leaks.
     * Use the System.IDisposable interface.
     */
    public void Dispose() =>
        Game.Events.LoadSceneManager.Init_LoadScreenDisable_Postfix.OnInvoke -= Init_LoadScreenDisable_Postfix_OnInvoke;
}