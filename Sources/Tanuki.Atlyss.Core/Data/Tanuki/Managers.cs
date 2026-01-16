namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Managers
{
    internal Core.Managers.Plugins plugins = null!;
    internal Core.Managers.Settings settings = null!;
    internal Core.Managers.Chat chat = null!;

    public Core.Managers.Plugins Plugins => plugins;
    public Core.Managers.Settings Settings => settings;
    public Core.Managers.Chat Chat => chat;

    internal Managers() { }
}
