using System.Collections.Generic;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Contexts.Commands;

internal sealed class Context : IContext
{
    public ICaller Caller { get; set; } = null!;
    public IReadOnlyList<string> Arguments { get; set; } = null!;
}
