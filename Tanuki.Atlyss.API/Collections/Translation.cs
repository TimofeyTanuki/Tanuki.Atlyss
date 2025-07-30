using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tanuki.Atlyss.API.Collections;

public class Translation
{
    public Dictionary<string, string> Translations;
    public string this[string Key]
    {
        get => Translations[Key];
        set => Translations[Key] = value;
    }
    public string Translate(string Key, params object[] Placeholder)
    {
        if (!Translations.TryGetValue(Key, out string Value))
            return $"{{{Key}}}";

        return string.Format(Value, Placeholder);
    }
}