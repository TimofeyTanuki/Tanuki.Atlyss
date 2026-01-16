using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Data.Settings;

public sealed class Translations
{
    internal readonly Dictionary<string, byte> preferredLanguageOrder = new(StringComparer.OrdinalIgnoreCase);

    public string[] PreferredLanguages { get; internal set; } = null!;
    public IReadOnlyDictionary<string, byte> PreferredLanguageOrder => preferredLanguageOrder;
}
