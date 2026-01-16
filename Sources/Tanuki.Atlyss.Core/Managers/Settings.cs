using System;
using System.Collections.Generic;
using System.IO;
using Tanuki.Atlyss.Core.Extensions;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Settings
{
    private readonly Data.Tanuki.Settings settings;

    internal Settings(Data.Tanuki.Settings settings) => this.settings = settings;

    private void RefreshLanguageSection()
    {
        Data.Settings.Translations section = settings.translations;

        HashSet<char> invalidCharacters = [.. Path.GetInvalidFileNameChars()];
        List<string> languages = [];

        byte priority = 0;
        foreach (string rawString in Configuration.Instance.Language.PreferredLanguages.Value.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            Span<char> span = rawString
                .ToCharArray()
                .AsSpan();

            for (int position = 0; position < span.Length; position++)
                span[position] = char.ToLower(span[position]);

            span = span.RemoveCharacters(invalidCharacters);

            int start = 0;
            while (start < span.Length && span[start] == ' ')
                start++;

            int end = span.Length;
            while (end > start && (span[end - 1] == ' ' || span[end - 1] == '.'))
                end--;

            span = span[start..end];

            if (span.Length == 0 || (span.Length == 1 && span[0] == '.'))
                continue;

            string language = span.ToString();

            if (!section.preferredLanguageOrder.TryAdd(language, priority))
                continue;

            languages.Add(language);

            if (priority == byte.MaxValue)
                break;

            priority++;
        }

        section.PreferredLanguages = [.. languages];
    }

    private void RefreshCommandsSection()
    {
        Data.Settings.Commands section = settings.commands;
        section.ClientPrefix = Configuration.Instance.Commands.ClientPrefix.Value;
        section.ServerPrefix = Configuration.Instance.Commands.ServerPrefix.Value;

        if (section.ClientPrefix.Length == 0 ||
            section.ClientPrefix.Length > Data.Settings.Commands.CLIENTPREFIX_MAXLENGTH)
        {
            section.ClientPrefix = Data.Settings.Commands.CLIENTPREFIX_DEFAULT;
            Configuration.Instance.Commands.ClientPrefix.Value = section.ClientPrefix;
        }

        if (section.ServerPrefix.Length == 0 ||
            section.ServerPrefix.Length > Data.Settings.Commands.SERVERPREFIX_MAXLENGTH)
        {
            section.ServerPrefix = Data.Settings.Commands.SERVERPREFIX_DEFAULT;
            Configuration.Instance.Commands.ServerPrefix.Value = section.ServerPrefix;
        }

        settings.commands.Prefixes = [Configuration.Instance.Commands.ClientPrefix.Value, Configuration.Instance.Commands.ServerPrefix.Value];
    }

    internal void Refresh()
    {
        RefreshCommandsSection();
        RefreshLanguageSection();
    }
}