using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Tanuki.Atlyss.API.Collections;

namespace Tanuki.Atlyss.Core.Serialization.Translation;

public class JsonTranslationSet : TranslationSet
{
    /// <exception cref="Exception">
    /// Any exception that may occur during <see cref="File.ReadAllText(string)"/> or <see cref="JsonConvert.DeserializeObject{T}(string)"/>.
    /// </exception>
    public void LoadFromFile(string path)
    {
        base.translations.Clear();
        Dictionary<string, string> translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path)) ?? [];
        base.translations = translations;
    }

    /// <exception cref="Exception">
    /// Any exception that may occur during <see cref="JsonConvert.SerializeObject(object?)"/> or <see cref="File.WriteAllText(string, string)"/>.
    /// </exception>
    public void SaveToFile(string path) =>
        File.WriteAllText(path, JsonConvert.SerializeObject(translations));
}
