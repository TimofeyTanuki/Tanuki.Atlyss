using System.Collections.Generic;

namespace Tanuki.Atlyss.API.Tanuki.Commands;

public interface IContext
{
    public ICaller Caller { get; }
    public IReadOnlyList<string> Arguments { get; }
}
