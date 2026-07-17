namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Providers
{
    private Core.Providers.Commands commands = null!;
    private Core.Providers.Settings settings = null!;
    private Core.Providers.CommandCallerPolicies commandCallerPolicies = null!;

    public Core.Providers.Commands Commands
    {
        get => commands;
        internal set => commands = value;
    }
    public Core.Providers.Settings Settings
    {
        get => settings;
        internal set => settings = value;
    }
    public Core.Providers.CommandCallerPolicies CommandCallerPolicies
    {
        get => commandCallerPolicies;
        internal set => commandCallerPolicies = value;
    }

    internal Providers() { }
}
