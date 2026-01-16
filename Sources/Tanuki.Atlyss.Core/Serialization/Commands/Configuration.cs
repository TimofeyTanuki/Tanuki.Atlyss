using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Serialization.Commands;

[Serializable]
public sealed class Configuration
{
    [JsonProperty("Names")]
    public List<string> names = null!;

    [JsonProperty("Help")]
    public string? help;

    [JsonProperty("Syntax")]
    public string? syntax;

    public static Configuration CreateFromType(Type type, IReadOnlyList<string> prefixRestrictions)
    {
        Configuration configuration = new()
        {
            names = [Utilities.Commands.Name.Normalize(type.FullName, prefixRestrictions)]
        };

        return configuration;
    }

    public static Configuration CreateFromType<T>(IReadOnlyList<string> prefixRestrictions) => CreateFromType(typeof(T), prefixRestrictions);
}
