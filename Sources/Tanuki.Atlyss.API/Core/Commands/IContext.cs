using System.Collections.Generic;

namespace Tanuki.Atlyss.API.Core.Commands;

public interface IContext
{
    public ICaller Caller { get; }
    public IReadOnlyList<string> Arguments { get; }
}
