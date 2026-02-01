using System;
using System.Collections.Generic;
using System.IO;
using Tanuki.Atlyss.Shared.Extensions;

namespace Tanuki.Atlyss.Core.Providers;

public sealed class Settings
{
    private readonly Data.Settings.Translations translationSection = new();
    private readonly Data.Settings.Commands commandSection = new();
    private readonly Data.Settings.Network networkSection = new();

    public Data.Settings.Translations TranslationSection => translationSection;
    public Data.Settings.Commands CommandSection => commandSection;
    public Data.Settings.Network NetworkSection => networkSection;

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
        commandSection.clientPrefix = Configuration.Instance.Commands.ClientPrefix.Value;
        commandSection.serverPrefix = Configuration.Instance.Commands.ServerPrefix.Value;

        if (commandSection.ClientPrefix.Length == 0 ||
            commandSection.ClientPrefix.Length > Data.Settings.Commands.PREFIX_MAX_LENGTH)
        {
            commandSection.clientPrefix = Data.Settings.Commands.CLIENT_PREFIX_DEFAULT;
            Configuration.Instance.Commands.ClientPrefix.Value = commandSection.ClientPrefix;
        }

        if (commandSection.ServerPrefix.Length == 0 ||
            commandSection.ServerPrefix.Length > Data.Settings.Commands.PREFIX_MAX_LENGTH)
        {
            commandSection.serverPrefix = Data.Settings.Commands.SERVER_PREFIX_DEFAULT;
            Configuration.Instance.Commands.ServerPrefix.Value = commandSection.ServerPrefix;
        }

        commandSection.prefixes = [Configuration.Instance.Commands.ClientPrefix.Value, Configuration.Instance.Commands.ServerPrefix.Value];
    }

    private void RefreshNetworkSection()
    {
        networkSection.mainSteamMessageChannel = Configuration.Instance.Network.MainSteamMessageChannel.Value;

        if (Configuration.Instance.Network.RateLimiterWindow.Value < 0)
            Configuration.Instance.Network.RateLimiterWindow.Value = 0;

        networkSection.rateLimiterWindow = Configuration.Instance.Network.RateLimiterWindow.Value;

        if (Configuration.Instance.Network.RateLimiterBandwidth.Value < 0)
            Configuration.Instance.Network.RateLimiterBandwidth.Value = 0;

        networkSection.rateLimiterBandwidth = Configuration.Instance.Network.RateLimiterBandwidth.Value;

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