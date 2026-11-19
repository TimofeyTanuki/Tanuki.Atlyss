using System;
using System.Collections.Generic;
using System.IO;
using Tanuki.Atlyss.Shared.Extensions;

namespace Tanuki.Atlyss.Core.Providers;

public sealed class Settings
{
    private readonly Types.Settings.Translations translationSection = new();
    private readonly Types.Settings.Commands commandSection = new();
    private readonly Types.Settings.Network networkSection = new();

    public Types.Settings.Translations TranslationSection => translationSection;
    public Types.Settings.Commands CommandSection => commandSection;
    public Types.Settings.Network NetworkSection => networkSection;

    internal Settings() { }

    private void RefreshLanguageSection()
    {
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

            if (!translationSection.preferredLanguageOrder.TryAdd(language, priority))
                continue;

            languages.Add(language);

            if (priority == byte.MaxValue)
                break;

            priority++;
        }

        translationSection.PreferredLanguages = [.. languages];
    }

    private void RefreshCommandsSection()
    {
        commandSection.prefix = Configuration.Instance.Commands.Prefix.Value;

        if (commandSection.Prefix.Length == 0 ||
            commandSection.Prefix.Length > Types.Settings.Commands.PREFIX_MAX_LENGTH)
        {
            commandSection.prefix = Types.Settings.Commands.DEFAULT_PREFIX;
            Configuration.Instance.Commands.Prefix.Value = commandSection.Prefix;
        }
    }

    private void RefreshNetworkSection()
    {
        networkSection.windowRateLimiter.window = Configuration.Instance.Network.WindowRateLimiter_Window.Value;
        networkSection.windowRateLimiter.bandwidth = Configuration.Instance.Network.WindowRateLimiter_Bandwidth.Value;
        networkSection.preventLobbyOwnerRateLimiting = Configuration.Instance.Network.PreventLobbyOwnerRateLimiting.Value;
        networkSection.steamNetworkMessagePollerBuffer = Configuration.Instance.Network.SteamNetworkMessagePollerBuffer.Value;
    }

    internal void Refresh()
    {
        RefreshCommandsSection();
        RefreshLanguageSection();
        RefreshNetworkSection();
    }
}