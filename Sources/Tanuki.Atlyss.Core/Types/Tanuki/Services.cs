namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Services
{
    private Core.Services.ServerHello serverHello = null!;

    public Core.Services.ServerHello ServerHello
    {
        get => serverHello;
        internal set => serverHello = value;
    }

    internal Services() { }
}
