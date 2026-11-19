namespace Tanuki.Atlyss.Core.Types.Settings;

public sealed class Commands
{
    public const string DEFAULT_PREFIX = "/";
    public const byte PREFIX_MAX_LENGTH = 4;

    internal string prefix = null!;

    public string Prefix => prefix;
}
