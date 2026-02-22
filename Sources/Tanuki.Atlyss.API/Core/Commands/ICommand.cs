namespace Tanuki.Atlyss.API.Core.Commands;

/// <summary>
/// Command interface.
/// </summary>
/// <remarks>
/// Commands are stateless, which means that each time they're executed, a new instance is created.
/// </remarks>
public interface ICommand
{
    public void Execute(IContext context);
}
