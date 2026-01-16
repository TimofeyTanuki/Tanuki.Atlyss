namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Settings
{
    public Data.Settings.Translations translations = null!;
    public Data.Settings.Commands commands = null!;

    public Data.Settings.Translations Translations => translations;
    public Data.Settings.Commands Commands => commands;

    internal Settings() { }
}
