using System.IO;

namespace Tanuki.Atlyss.Core.Utilities.Plugins;

internal class ConfigurationDirectory
{
    public static string? Find(string rootDirectory, string pluginName)
    {
        string[] preferredDirectories = Directory.GetDirectories(rootDirectory, pluginName, SearchOption.AllDirectories);
        return preferredDirectories.Length > 0 ? preferredDirectories[0] : null;
    }

    public static string Get(string rootDirectory, string pluginName) =>
        Find(rootDirectory, pluginName) ?? Path.Combine(rootDirectory, pluginName);
}
