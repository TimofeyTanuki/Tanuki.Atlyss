using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Tanuki.Atlyss.API.Collections;

namespace Tanuki.Atlyss.Core.Models;

public class JsonTranslationSet : TranslationSet
{
    /// <exception cref="Exception">
    /// Any exception that may occur during <see cref="File.ReadAllText(string)"/> or <see cref="JsonConvert.DeserializeObject{T}(string)"/>.
    /// </exception>
    public void LoadFromFile(string Path)
    {
        this.Translations.Clear();

        Dictionary<string, string> Translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path)) ?? [];

        this.Translations = Translations;
    }

    /// <exception cref="Exception">
    /// Any exception that may occur during <see cref="JsonConvert.SerializeObject(object?)"/> or <see cref="File.WriteAllText(string, string)"/>.
    /// </exception>
    public void SaveToFile(string Path) =>
        File.WriteAllText(Path, JsonConvert.SerializeObject(Translations));
}