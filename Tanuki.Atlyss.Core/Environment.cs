namespace Tanuki.Atlyss.Core;

public static class Environment
{
    public static readonly string
        PluginTranslationsFileFormat = "translations.json",
        PluginTranslationsFileTemplate = "{0}.{1}",
        PluginCommandsFileFormat = "commands.json",
        PluginCommandsFileTemplate = "{0}.{1}";

    public static string FormatPluginTranslationsFile(string Language) => string.Format(PluginTranslationsFileTemplate, Language, PluginTranslationsFileFormat);

    public static string FormatPluginCommandsFile(string Language) => string.Format(PluginCommandsFileTemplate, Language, PluginCommandsFileFormat);
}