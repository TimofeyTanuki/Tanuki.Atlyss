namespace Tanuki.Atlyss.ExamplePlugin.Commands;

internal class Example : API.Commands.ICommand
{
    public void Execute(string[] Arguments)
    {
        Main.Instance.Logger.LogInfo($"Execute([{string.Join(", ", Arguments)}])");
    }
}