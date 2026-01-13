using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Models;

[Serializable]
public class CommandConfigurationItem
{
    [JsonProperty("Names")]
    public List<string> Names = null!;

    [JsonProperty("Help")]
    public string? Help;

    [JsonProperty("Syntax")]
    public string? Syntax;
}