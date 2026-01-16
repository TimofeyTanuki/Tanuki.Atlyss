namespace Tanuki.Atlyss.Core.Data.Settings;

public sealed class Commands
{
    public const string CLIENTPREFIX_DEFAULT = "/";
    public const byte CLIENTPREFIX_MAXLENGTH = 16;
    public const string SERVERPREFIX_DEFAULT = "/";
    public const byte SERVERPREFIX_MAXLENGTH = 16;

    public string ClientPrefix { get; internal set; } = null!;
    public string ServerPrefix { get; internal set; } = null!;

    public string[] Prefixes { get; internal set; } = null!;
}
