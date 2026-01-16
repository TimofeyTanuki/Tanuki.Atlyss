using System.Collections.Generic;
using System.IO;

namespace Tanuki.Atlyss.Core.Utilities.Translations;

public static class LanguageFileSelector
{
    public const string DEFAULT_LANGUAGE = "neutral";

    public static string? FindPreferredFile(IReadOnlyDictionary<string, byte> preferredLanguages, string directory, string? extension, SearchOption searchOption = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Directory not found: {directory}");

        int extensionLength;
        if (string.IsNullOrEmpty(extension))
            extensionLength = 0;
        else
        {
            if (!extension.StartsWith('.'))
                extension = $".{extension}";

            extensionLength = extension.Length;
        }

        string searchPattern = string.IsNullOrEmpty(extension) ? "*" : $"*{extension}";

        string? result = null;
        byte bestPriority = byte.MaxValue;

        foreach (string file in Directory.EnumerateFiles(directory, searchPattern, searchOption))
        {
            string fileName = Path.GetFileName(file);

            if (fileName.Length <= extensionLength)
                continue;

            string fileLanguage = extension is null ? fileName : fileName[..^extensionLength];

            if (!preferredLanguages.TryGetValue(fileLanguage, out byte priority))
                continue;

            if (bestPriority < priority)
                continue;

            bestPriority = priority;
            result = file;

            if (bestPriority == 0)
                break;
        }

        return result;
    }

    public static string GetPreferredFile(IReadOnlyDictionary<string, byte> preferredLanguages, string directory, string? extension, SearchOption searchOption = SearchOption.AllDirectories) =>
        FindPreferredFile(preferredLanguages, directory, extension, searchOption) ?? Path.Combine(directory, $"{DEFAULT_LANGUAGE}{extension}");
}
