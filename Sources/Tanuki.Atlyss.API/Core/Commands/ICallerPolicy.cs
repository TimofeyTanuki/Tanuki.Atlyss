namespace Tanuki.Atlyss.API.Core.Commands;

public interface ICallerPolicy
{
    public bool IsAllowed(ICaller caller);
}
