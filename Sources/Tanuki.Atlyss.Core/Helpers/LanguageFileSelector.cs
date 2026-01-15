using System.Collections.Generic;
using System.IO;

namespace Tanuki.Atlyss.Core.Helpers;

public static class LanguageFileSelector
{
    public const string DEFAULT_LANGUAGE = "neutral";

    public static string? FindPreferredFile(IReadOnlyDictionary<string, byte> PreferredLanguages, string Directory, string? Extension, SearchOption SearchOption = SearchOption.AllDirectories)
    {
        if (!System.IO.Directory.Exists(Directory))
            throw new DirectoryNotFoundException($"Directory not found: {Directory}");

        int ExtensionLength;
        if (string.IsNullOrEmpty(Extension))
            ExtensionLength = 0;
        else
        {
            if (!Extension.StartsWith('.'))
                Extension = $".{Extension}";

            ExtensionLength = Extension.Length;
        }

        string SearchPattern = string.IsNullOrEmpty(Extension) ? "*" : $"*{Extension}";

        string? Result = null;
        byte BestPriority = byte.MaxValue;

        foreach (string File in System.IO.Directory.EnumerateFiles(Directory, SearchPattern, SearchOption))
        {
            string FileName = Path.GetFileName(File);

            if (FileName.Length <= ExtensionLength)
                continue;

            string FileLanguage = Extension is null ? FileName : FileName[..^ExtensionLength];

            if (!PreferredLanguages.TryGetValue(FileLanguage, out byte Priority))
                continue;

            if (BestPriority < Priority)
                continue;

            BestPriority = Priority;
            Result = File;

            if (BestPriority == 0)
                break;
        }

        return Result;
    }

    public static string GetPreferredFile(IReadOnlyDictionary<string, byte> PreferredLanguages, string Directory, string? Extension, SearchOption SearchOption = SearchOption.AllDirectories) =>
        FindPreferredFile(PreferredLanguages, Directory, Extension, SearchOption) ?? Path.Combine(Directory, $"{DEFAULT_LANGUAGE}{Extension}");
}
