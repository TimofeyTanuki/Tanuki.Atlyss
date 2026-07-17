namespace Tanuki.Atlyss.Game.Types.Tanuki;

public sealed class Providers
{
    private Game.Providers.Player player = null!;

    public Game.Providers.Player Player
    {
        get => player;
        internal set => player = value;
    }

    internal Providers() { }
}
