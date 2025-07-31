namespace Tanuki.Atlyss.Core;

public class Tanuki
{
    public static Tanuki Instance { get; internal set; }

    public Commands.Manager Commands;
    public Plugins.Manager Plugins;

    private Tanuki() { }
    public static void Initialize()
    {
        if (Instance is not null)
            return;


        Instance = new()
        {
            Commands = new(),
            Plugins = new()
        };

        Game.Main.Patch();

        Game.Events.ChatBehaviour.Send_ChatMessage.Before += Instance.Commands.OnSendMessage;
    }
    public void Reload()
    {
        Plugins.Reload();
    }
}