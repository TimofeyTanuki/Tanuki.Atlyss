namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Managers
{
    internal Core.Managers.Plugin plugin = null!;
    internal Core.Managers.Chat chat = null!;
    internal Core.Managers.Hotkey hotkey = null!;

    public Core.Managers.Plugin Plugin => plugin;
    public Core.Managers.Chat Chat => chat;
    public Core.Managers.Hotkey Hotkey => hotkey;

    internal Managers() { }
}
