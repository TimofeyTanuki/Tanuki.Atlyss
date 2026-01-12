using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Commands;

[Serializable]
public class CommandConfiguration
{
    [JsonProperty("Names")]
    public List<string> Names = null!;

    [JsonProperty("Help")]
    public string Help = null!;

    [JsonProperty("Syntax")]
    public string Syntax = null!;
}