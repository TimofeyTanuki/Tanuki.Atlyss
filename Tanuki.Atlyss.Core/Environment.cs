namespace Tanuki.Atlyss.Core;

public static class Environment
{
    public static readonly string PluginTranslationsFileFormat = "translations.json";
    public static readonly string PluginTranslationsFileTemplate = "{0}.{1}";
    public static readonly string PluginCommandsFileFormat = "commands.json";
    public static readonly string PluginCommandsFileTemplate = "{0}.{1}";

    public static string FormatPluginTranslationsFile(string Language) => string.Format(PluginTranslationsFileTemplate, Language, PluginTranslationsFileFormat);
    public static string FormatPluginCommandsFile(string Language) => string.Format(PluginCommandsFileTemplate, Language, PluginCommandsFileFormat);
}