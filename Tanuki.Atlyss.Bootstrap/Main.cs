using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Commands;

namespace Tanuki.Atlyss.Bootstrap;

[BepInPlugin("9c00d52e-10b8-413f-9ee4-bfde81762442", "Tanuki.Atlyss.Bootstrap", "1.0.0.0")]
[BepInProcess("ATLYSS.exe")]
internal class Main : BaseUnityPlugin
{
    private List<IPlugin> Plugins;

    public void Awake()
    {
        Assembly Assembly = Assembly.GetExecutingAssembly();
        Plugins = [];

        BaseUnityPlugin BaseUnityPlugin;
        IPlugin Plugin;
        foreach (PluginInfo PluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            BaseUnityPlugin = PluginInfo.Instance;
            if (BaseUnityPlugin is null)
                continue;

            Logger.LogInfo($"Loaded: {BaseUnityPlugin.GetType()}");
            if (!typeof(IPlugin).IsAssignableFrom(BaseUnityPlugin.GetType()))
                continue;
            Logger.LogInfo($"OK");

            Plugin = (IPlugin)BaseUnityPlugin;
            Plugins.Add(Plugin);
            try
            {

                Plugin.LoadPlugin();
            }
             catch (Exception ex)
            {
                Logger.LogError(ex);
                continue;
            }

            List<ICommand> cmds = [];
            foreach (Type Type in Plugin.GetType().Assembly.GetTypes())
            {
                if (Type.IsAbstract || Type.IsInterface)
                    continue;

                if (!typeof(ICommand).IsAssignableFrom(Type))
                    continue;

                Logger.LogInfo($"Command {Type.Name}");

                cmds.Add((ICommand)Type);
            }
        }
        Logger.LogInfo($"Loaded: {Plugins.Count}");
        Task.Run(Debug);
    }


    private async void Debug()
    {
        while (true)
        {
            await Task.Delay(1500);

            Logger.LogInfo($"Task.Run() {Plugins.Count}\n");

            foreach (IPlugin plugin in Plugins)
            {
                Logger.LogInfo($"{plugin is null}");
            }
        }
    }
}