using System;

namespace Tanuki.Atlyss.Game;

public sealed class Tanuki
{
    private static Tanuki instance = null!;
    private static Action? onInitialized;

    public Data.Tanuki.Providers providers = null!;

    public static Tanuki Instance => instance;

    public Data.Tanuki.Providers Providers => providers;

    public static event Action OnInitialized
    {
        add { onInitialized += value; }
        remove { onInitialized -= value; }
    }

    private Tanuki() { }

    public static void Initialize()
    {
        if (instance is not null)
            return;

        instance = new()
        {
            providers = new()
            {
                player = new()
            }
        };

        onInitialized?.Invoke();
        onInitialized = null;
    }
}
