using System;

namespace Tanuki.Atlyss.Game;

public sealed class Tanuki
{
    public static Action? OnInitialized;

    private static Tanuki instance = null!;
    public static Tanuki Instance => instance;

    public Data.Tanuki.Managers managers = null!;
    public Data.Tanuki.Providers providers = null!;

    public Data.Tanuki.Managers Managers => managers;
    public Data.Tanuki.Providers Providers => providers;

    private Tanuki() { }

    public static void Initialize()
    {
        if (instance is not null)
            return;

        instance = new()
        {
            managers = new()
            {
                patches = new()
            },

            providers = new()
            {
                player = new()
            }
        };

        OnInitialized?.Invoke();
    }
}
