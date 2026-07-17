namespace Tanuki.Atlyss.Core.Types.Tanuki;

public sealed class Services
{
    private Core.Services.TanukiServer tanukiServer = null!;

    public Core.Services.TanukiServer TanukiServer
    {
        get => tanukiServer;
        internal set => tanukiServer = value;
    }

    internal Services() { }
}
