namespace Tanuki.Atlyss.Core;

public class Tanuki
{
    public static Tanuki Instance;

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

        Game.Fields.GameManager.Initialize();

        Game.Main.Initialize();
        Game.Main.Instance.Patch(typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix));

        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Instance.Commands.OnSendMessage;
    }
    public void Reload()
    {
        Plugins.Reload();
    }
}