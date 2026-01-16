namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Settings
{
    public Data.Settings.Translations Translations { get; internal set; } = null!;
    public Data.Settings.Commands Commands { get; internal set; } = null!;

    internal Settings() { }
}
