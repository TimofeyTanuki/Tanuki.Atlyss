namespace Tanuki.Atlyss.Core.Data.Settings;

public sealed class Commands
{
    public const string CLIENT_PREFIX_DEFAULT = "/";
    public const byte CLIENT_PREFIX_MAX_LENGTH = 4;
    public const string SERVER_PREFIX_DEFAULT = "/";
    public const byte SERVER_PREFIX_MAX_LENGTH = 4;

    internal string clientPrefix = null!;
    internal string serverPrefix = null!;
    internal string[] prefixes = null!;

    public string ClientPrefix => clientPrefix;
    public string ServerPrefix => serverPrefix;
    public string[] Prefixes => prefixes;
}
