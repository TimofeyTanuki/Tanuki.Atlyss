namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Managers
{
    public Core.Managers.Plugins Plugins { get; internal set; } = null!;
    public Core.Managers.Commands Commands { get; internal set; } = null!;
    public Core.Managers.Settings Settings { get; internal set; } = null!;
    public Core.Managers.Chat Chat { get; internal set; } = null!;

    internal Managers() { }
}
