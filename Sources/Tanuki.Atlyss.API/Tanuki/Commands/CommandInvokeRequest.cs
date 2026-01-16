using System.Collections.Generic;

namespace Tanuki.Atlyss.API.Tanuki.Commands;

public record CommandInvokeRequest
{
    public ulong? Hash { get; }
    public string? Name { get; }

    public IReadOnlyList<string> Arguments { get; } = [];
}
