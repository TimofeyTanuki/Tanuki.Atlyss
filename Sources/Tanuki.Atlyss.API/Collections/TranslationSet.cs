using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Tanuki.Atlyss.API.Collections;

public class TranslationSet : IDictionary<string, string>
{
    protected Dictionary<string, string> translations;

    protected TranslationSet() : this(EqualityComparer<string>.Default) { }

    protected TranslationSet(IEqualityComparer<string> equalityComparer) =>
        translations = new(equalityComparer);

    protected TranslationSet(IDictionary<string, string> translations) :
        this(translations, translations is Dictionary<string, string> dictionary ? dictionary.Comparer : EqualityComparer<string>.Default)
    { }

    protected TranslationSet(IDictionary<string, string> translations, IEqualityComparer<string> equalityComparer) =>
        this.translations = new(translations, equalityComparer);

    public virtual ICollection<string> Keys => translations.Keys;
    public virtual ICollection<string> Values => translations.Values;
    public virtual int Count => translations.Count;
    public virtual bool IsReadOnly => false;

    public virtual void Add(string key, string value) => translations.Add(key, value);

    public virtual void Add(KeyValuePair<string, string> item) => translations.Add(item.Key, item.Value);

    public virtual void Clear() => translations.Clear();

    public virtual bool Contains(KeyValuePair<string, string> item) =>
        translations.TryGetValue(item.Key, out string value) && value == item.Value;

    public virtual bool ContainsKey(string key) => translations.ContainsKey(key);

    public virtual void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(Array));

        if (arrayIndex < 0 || arrayIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        if (array.Length - arrayIndex < translations.Count)
            throw new ArgumentException("The destination array is too small.");

        foreach (KeyValuePair<string, string> Item in translations)
            array[arrayIndex++] = Item;
    }

    public virtual IEnumerator<KeyValuePair<string, string>> GetEnumerator() => translations.GetEnumerator();

    public virtual bool Remove(string key) => translations.Remove(key);

    public virtual bool Remove(KeyValuePair<string, string> item)
    {
        if (translations.TryGetValue(item.Key, out string value) && value == item.Value)
            return translations.Remove(item.Key);

        return false;
    }

    public virtual bool TryGetValue(string key, out string value) => translations.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => translations.GetEnumerator();

    public virtual string this[string key]
    {
        get => translations[key];
        set => translations[key] = value;
    }

    public virtual string Translate(string key, params object[] placeholder) => Translate(CultureInfo.InvariantCulture, key, placeholder);

    public virtual string Translate(IFormatProvider formatProvider, string key, params object[] placeholder)
    {
        if (!translations.TryGetValue(key, out string value))
            return $"{{{key}}}";

        if (placeholder.Length > 0)
            value = string.Format(formatProvider, value, placeholder);

        return value;
    }
}
