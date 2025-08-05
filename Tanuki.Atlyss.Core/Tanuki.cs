namespace Tanuki.Atlyss.Core;

public class Tanuki
{
    public static Tanuki Instance;

    public Settings Settings = new();
    public Commands.Manager Commands = new();
    public Plugins.Manager Plugins = new();
    private bool Loaded = false;
    private Tanuki() { }
    public static void Initialize()
    {
        if (Instance is not null)
            return;

        Instance = new();

        Game.Fields.GameManager.Initialize();

        Game.Main.Initialize();
        Game.Main.Instance.Patch(typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix));

        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Instance.Commands.OnSendMessage;
    }
    public void Load()
    {
        if (Loaded)
            return;

        Loaded = true;
        Plugins.LoadPlugins();
    }
    public void Reload()
    {
        Commands.RemoveAllCommands();
        Plugins.ReloadPlugins();
    }
}