using System.Collections.Generic;
using System.Linq;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Commands;

namespace Tanuki.Atlyss.Bootstrap.Commands;

public class Reload : ICommand
{
    public void Execute(string[] Arguments)
    {
        if (Arguments.Length == 0)
        {
            ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.Full"));
            Core.Tanuki.Instance.Plugins.Reload();
            return;
        }

        Arguments = [.. Arguments.Select(x => x.ToLower())];

        List<IPlugin> Plugins = [];

        foreach (IPlugin Plugin in Core.Tanuki.Instance.Plugins.Plugins)
        {
            if (!Arguments.Any(x => Plugin.Name.ToLower().Contains(x)))
                continue;

            Plugins.Add(Plugin);
        }

        if (Plugins.Count > 0)
        {
            ChatBehaviour._current.New_ChatMessage(
                Main.Instance.Translate(
                    "Commands.Reload.Plugins",
                    string.Join(
                        Main.Instance.Translate("Commands.Reload.Plugins.Separator"),
                        Plugins.Select(x => x.Name)
                    )
                )
            );
            Plugins.ForEach(Core.Tanuki.Instance.Plugins.Reload);
            return;
        }

        ChatBehaviour._current.New_ChatMessage(Main.Instance.Translate("Commands.Reload.PluginsNotFound"));
    }
}