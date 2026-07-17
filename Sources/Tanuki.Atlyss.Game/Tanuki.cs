using System;

namespace Tanuki.Atlyss.Game;

/// <summary>
/// This module is initialized automatically from the core.
/// </summary>
/// <remarks>
/// Manually calling <see cref="Initialize"/> isn't recommended.
/// </remarks>
public sealed class Tanuki
{
    private static Tanuki instance = null!;
    private static Action? onInitialized;

    private Types.Tanuki.Providers providers = null!;

    public static Tanuki Instance => instance;

    public Types.Tanuki.Providers Providers => providers;

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
                Player = new()
            }
        };

        onInitialized?.Invoke();
        onInitialized = null;
    }
}
