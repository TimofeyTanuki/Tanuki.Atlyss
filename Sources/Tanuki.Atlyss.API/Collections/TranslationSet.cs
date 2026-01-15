using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Tanuki.Atlyss.API.Collections;

public class TranslationSet : IDictionary<string, string>
{
    protected Dictionary<string, string> Translations;

    protected TranslationSet() : this(EqualityComparer<string>.Default) { }

    protected TranslationSet(IEqualityComparer<string> EqualityComparer) =>
        Translations = new(EqualityComparer);

    protected TranslationSet(IDictionary<string, string> Translations) :
        this(Translations, Translations is Dictionary<string, string> Dictionary ? Dictionary.Comparer : EqualityComparer<string>.Default)
    { }

    protected TranslationSet(IDictionary<string, string> Translations, IEqualityComparer<string> EqualityComparer) =>
        this.Translations = new(Translations, EqualityComparer);

    public virtual ICollection<string> Keys => Translations.Keys;
    public virtual ICollection<string> Values => Translations.Values;
    public virtual int Count => Translations.Count;
    public virtual bool IsReadOnly => false;

    public virtual void Add(string Key, string Value) => Translations.Add(Key, Value);

    public virtual void Add(KeyValuePair<string, string> Item) => Translations.Add(Item.Key, Item.Value);

    public virtual void Clear() => Translations.Clear();

    public virtual bool Contains(KeyValuePair<string, string> Item) =>
        Translations.TryGetValue(Item.Key, out string Value) && Value == Item.Value;

    public virtual bool ContainsKey(string Key) => Translations.ContainsKey(Key);

    public virtual void CopyTo(KeyValuePair<string, string>[] Array, int ArrayIndex)
    {
        if (Array is null)
            throw new ArgumentNullException(nameof(Array));

        if (ArrayIndex < 0 || ArrayIndex >= Array.Length)
            throw new ArgumentOutOfRangeException(nameof(ArrayIndex));

        if (Array.Length - ArrayIndex < Translations.Count)
            throw new ArgumentException("The destination array is too small.");

        foreach (KeyValuePair<string, string> Item in Translations)
            Array[ArrayIndex++] = Item;
    }

    public virtual IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Translations.GetEnumerator();

    public virtual bool Remove(string Key) => Translations.Remove(Key);

    public virtual bool Remove(KeyValuePair<string, string> Item)
    {
        if (Translations.TryGetValue(Item.Key, out string Value) && Value == Item.Value)
            return Translations.Remove(Item.Key);

        return false;
    }

    public virtual bool TryGetValue(string Key, out string Value) => Translations.TryGetValue(Key, out Value);

    IEnumerator IEnumerable.GetEnumerator() => Translations.GetEnumerator();

    public virtual string this[string Key]
    {
        get => Translations[Key];
        set => Translations[Key] = value;
    }

    public virtual string Translate(string Key, params object[] Placeholder) => Translate(CultureInfo.InvariantCulture, Key, Placeholder);

    public virtual string Translate(IFormatProvider FormatProvider, string Key, params object[] Placeholder)
    {
        if (!Translations.TryGetValue(Key, out string Value))
            return $"{{{Key}}}";

        if (Placeholder.Length > 0)
        {
            try
            {
                Value = string.Format(FormatProvider, Value, Placeholder);
            }
            catch (Exception Exception)
            {
                Value = $"{{{Key}:{Exception.GetType().Name}}}";
            }
        }

        return Value;
    }
}
