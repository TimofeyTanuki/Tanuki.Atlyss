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

    public static Configuration CreateFromType(Type type, string restrictedPrefix)
    {
        Configuration configuration = new()
        {
            names = [Utilities.Commands.Name.Normalize(type.FullName, restrictedPrefix)]
        };

        return configuration;
    }

    public static Configuration CreateFromType<T>(string restrictedPrefix) => CreateFromType(typeof(T), restrictedPrefix);
}
