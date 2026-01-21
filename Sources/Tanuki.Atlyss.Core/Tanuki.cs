namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    internal static Tanuki instance = null!;
    public static Tanuki Instance => instance;

    internal Data.Tanuki.Registers registers = null!;
    internal Data.Tanuki.Managers managers = null!;
    internal Data.Tanuki.Providers providers = null!;
    internal Data.Tanuki.Routers routers = null!;

    public Data.Tanuki.Registers Registers => registers;
    public Data.Tanuki.Managers Managers => managers;
    public Data.Tanuki.Providers Providers => providers;
    public Data.Tanuki.Routers Routers => routers;

    internal Tanuki() { }
}