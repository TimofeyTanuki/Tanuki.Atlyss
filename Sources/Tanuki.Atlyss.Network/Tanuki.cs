using BepInEx.Logging;

namespace Tanuki.Atlyss.Network;

public sealed class Tanuki
{
    private static Tanuki instance = null!;
    public static Tanuki Instance => instance;

    internal ManualLogSource manualLogSource = null!;
    internal Data.Tanuki.Registers registers = null!;
    internal Data.Tanuki.Providers providers = null!;
    internal Data.Tanuki.Managers managers = null!;

    public Data.Tanuki.Registers Registers => registers;
    public Data.Tanuki.Providers Providers => providers;
    public Data.Tanuki.Managers Managers => managers;

    private Tanuki() { }

    public static void Initialize()
    {
        if (instance is not null)
            return;

        ManualLogSource manualLogSource = new("Tanuki.Atlyss.Network");

        Data.Tanuki.Registers registers = new()
        {
            packets = new(manualLogSource)
        };

        Data.Tanuki.Managers managers = new()
        {
            packets = new(manualLogSource, registers.packets)
        };

        Data.Tanuki.Providers providers = new()
        {
            steam = new()
        };

        instance = new()
        {
            manualLogSource = manualLogSource,
            managers = managers,
            providers = providers
        };
    }
}
