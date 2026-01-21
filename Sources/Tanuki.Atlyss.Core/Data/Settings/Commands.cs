namespace Tanuki.Atlyss.Core.Data.Settings;

public sealed class Commands
{
    public const string CLIENTPREFIX_DEFAULT = "/";
    public const byte CLIENTPREFIX_MAXLENGTH = 16;
    public const string SERVERPREFIX_DEFAULT = "/";
    public const byte SERVERPREFIX_MAXLENGTH = 16;

    internal string clientPrefix = null!;
    internal string serverPrefix = null!;
    internal string[] prefixes = null!;

    public string ClientPrefix => clientPrefix;
    public string ServerPrefix => serverPrefix;
    public string[] Prefixes => prefixes;
}
