namespace Tanuki.Atlyss.ExamplePlugin.Commands;

internal class Example : API.Commands.ICommand
{
    public bool Execute(string[] Arguments)
    {
        ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Example"));
        Player._mainPlayer._pVisual.Cmd_VanitySparkleEffect();

        return false;
    }
}