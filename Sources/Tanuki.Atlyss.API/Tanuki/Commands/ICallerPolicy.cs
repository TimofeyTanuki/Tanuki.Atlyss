namespace Tanuki.Atlyss.API.Tanuki.Commands;

public interface ICallerPolicy
{
    public bool IsAllowed(ICaller caller);
}
