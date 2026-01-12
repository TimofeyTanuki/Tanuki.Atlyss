using System.Collections.Generic;

namespace Tanuki.Atlyss.API.Collections;

public class Translation
{
    public Dictionary<string, string> Translations = null!;

    public string this[string Key]
    {
        get => Translations[Key];
        set => Translations[Key] = value;
    }

    public string Translate(string Key, params object[] Placeholder)
    {
        if (!Translations.TryGetValue(Key, out string Value))
            return $"{{{Key}}}";

        if (Placeholder.Length > 0)
            return string.Format(Value, Placeholder);

        return Value;
    }
}