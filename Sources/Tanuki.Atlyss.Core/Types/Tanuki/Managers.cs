namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Managers
{
    private Core.Managers.Plugin plugin = null!;
    private Core.Managers.Chat chat = null!;
    private Core.Managers.Hotkey hotkey = null!;

    public Core.Managers.Plugin Plugin
    {
        get => plugin;
        internal set => plugin = value;
    }
    public Core.Managers.Chat Chat
    {
        get => chat;
        internal set => chat = value;
    }
    public Core.Managers.Hotkey Hotkey
    {
        get => hotkey;
        internal set => hotkey = value;
    }

    internal Managers() { }
}
