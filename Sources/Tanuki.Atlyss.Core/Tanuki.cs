namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    internal static Tanuki instance = null!;

    internal Data.Tanuki.Settings settings = null!;
    internal Data.Tanuki.Registers registers = null!;
    internal Data.Tanuki.Managers managers = null!;
    internal Data.Tanuki.Providers providers = null!;

    public static Tanuki Instance => instance;
    internal Data.Tanuki.Settings Settings => settings;
    internal Data.Tanuki.Registers Registers => registers;
    internal Data.Tanuki.Managers Managers => managers;
    internal Data.Tanuki.Providers Providers => providers;

    internal Tanuki() { }
}