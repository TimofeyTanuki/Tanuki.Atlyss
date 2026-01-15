using System;
using System.Collections.Generic;
using System.IO;
using Tanuki.Atlyss.Core.Extensions;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Settings
{
    public string[] PreferredLanguages { get; internal set; } = null!;
    internal readonly Dictionary<string, byte> _PreferredLanguageOrder = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, byte> PreferredLanguageOrder => _PreferredLanguageOrder;

    internal Settings() { }

    private void RefreshPreferredLanguages()
    {
        _PreferredLanguageOrder.Clear();

        HashSet<char> InvalidCharacters = [.. Path.GetInvalidFileNameChars()];
        List<string> Languages = [];

        byte Priority = 0;
        foreach (string RawString in Configuration.Instance.General.PreferredLanguages.Value.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            Span<char> Span = RawString
                .ToCharArray()
                .AsSpan();

            for (int Position = 0; Position < Span.Length; Position++)
                Span[Position] = char.ToLowerInvariant(Span[Position]);

            Span = Span.RemoveCharacters(InvalidCharacters);

            int Start = 0;
            while (Start < Span.Length && Span[Start] == ' ')
                Start++;

            int End = Span.Length;
            while (End > Start && (Span[End - 1] == ' ' || Span[End - 1] == '.'))
                End--;

            Span = Span[Start..End];

            if (Span.Length == 0 || (Span.Length == 1 && Span[0] == '.'))
                continue;

            string Language = Span.ToString();

            if (!_PreferredLanguageOrder.TryAdd(Language, Priority))
                continue;

            Languages.Add(Language);

            if (Priority == byte.MaxValue)
                break;

            Priority++;
        }

        PreferredLanguages = [.. Languages];
    }

    public void Refresh()
    {
        RefreshPreferredLanguages();
    }
}