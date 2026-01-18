namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    internal static Tanuki instance = null!;
    public static Tanuki Instance => instance;

    internal Data.Tanuki.Settings settings = null!;
    internal Data.Tanuki.Registers registers = null!;
    internal Data.Tanuki.Managers managers = null!;
    internal Data.Tanuki.Providers providers = null!;

    public Data.Tanuki.Settings Settings => settings;
    public Data.Tanuki.Registers Registers => registers;
    public Data.Tanuki.Managers Managers => managers;
    public Data.Tanuki.Providers Providers => providers;

    internal Tanuki() { }
}