namespace Tanuki.Atlyss.Core;

public class Tanuki
{
    public static Tanuki Instance;

    public Settings Settings;
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
            Plugins = new(),
            Settings = new()
            {
                Language = "default"
            }
        };

        Game.Fields.GameManager.Initialize();

        Game.Main.Initialize();
        Game.Main.Instance.Patch(typeof(Game.Events.ChatBehaviour.Send_ChatMessage_Prefix));

        Game.Events.ChatBehaviour.Send_ChatMessage_Prefix.OnInvoke += Instance.Commands.OnSendMessage;
    }
    public void Load()
        => Plugins.LoadPlugins();
    public void Reload()
    {
        Commands.ClearCommands();
        Plugins.ReloadPlugins();
    }
}