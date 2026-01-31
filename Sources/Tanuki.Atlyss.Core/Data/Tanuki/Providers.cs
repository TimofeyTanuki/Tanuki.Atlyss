namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Providers
{
    internal Core.Providers.Commands commands = null!;
    internal Core.Providers.Settings settings = null!;
    internal Core.Providers.CommandCallerPolicies commandCallerPolicies = null!;

    public Core.Providers.Commands Commands => commands;
    public Core.Providers.Settings Settings => settings;
    public Core.Providers.CommandCallerPolicies CommandCallerPolicies => commandCallerPolicies;

    internal Providers() { }
}
