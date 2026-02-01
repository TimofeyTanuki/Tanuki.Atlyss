namespace Tanuki.Atlyss.Core.Data.Tanuki;

public sealed class Services
{
    public Core.Services.TanukiServer tanukiServer = null!;
    public Core.Services.TanukiServer TanukiServer => tanukiServer;

    internal Services() { }
}
