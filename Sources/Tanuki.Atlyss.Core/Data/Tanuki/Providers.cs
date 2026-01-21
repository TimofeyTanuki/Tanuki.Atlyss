namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Providers
{
    internal Core.Providers.Commands commands = null!;
    internal Core.Providers.Settings settings = null!;

    public Core.Providers.Commands Commands => commands;
    public Core.Providers.Settings Settings => settings;

    internal Providers() { }
}
