using BepInEx;
using System.Reflection;

namespace Tanuki.Atlyss.Example;

/// <summary>
/// This an example of a plugin with a soft dependency and manual command registration.
/// </summary>
[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
[BepInDependency(Core.PluginInfo.GUID, BepInDependency.DependencyFlags.SoftDependency)]
public sealed class Main_SoftDependency : BaseUnityPlugin
{
    private const string TANUKI_ATLYSS_CORE_GUID = "9c00d52e10b8413f9ee4bfde81762442";

    internal static Main Instance = null!;

    private void Start()
    {
        Logger.LogDebug(nameof(Start));

        if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(TANUKI_ATLYSS_CORE_GUID))
        {
            Game.Patches.ChatBehaviour.Send_ChatMessage.OnPrefix += OnChatMessagePrefix;

            Core.Tanuki.OnInitialized += OnTanukiCoreInitialized;
        }
    }

    private void OnTanukiCoreInitialized()
    {
        Core.Tanuki tanukiCore = Core.Tanuki.Instance;
        Assembly assembly = GetType().Assembly;

        string commandConfigurationFile = System.IO.Path.Combine(Paths.ConfigPath, assembly.FullName, "commands.json");

        tanukiCore.Registers.Commands.RegisterAssembly(assembly, commandConfigurationFile);
    }

    private void OnChatMessagePrefix(string message, ref bool runOriginal)
    {
        Logger.LogDebug($"{nameof(message)}\n  message: {message}");
    }
}
