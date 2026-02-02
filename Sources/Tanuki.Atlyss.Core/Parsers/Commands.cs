using System;
using System.Collections.Generic;
using System.Text;

namespace Tanuki.Atlyss.Core.Parsers;


public class Commands(IReadOnlyList<char> quoteCharacters)
{
    private readonly IReadOnlyList<char> quoteCharacters = quoteCharacters ?? throw new ArgumentNullException(nameof(quoteCharacters));

    public bool TryParse(
        string prefix,
        string input,
        out string commandName,
        out IReadOnlyList<string> commandArguments,
        IReadOnlyDictionary<string, Type>? commandNameMap = null)
    {
        commandName = null!;
        commandArguments = null!;

        if (string.IsNullOrEmpty(input))
            return false;

        if (!input.StartsWith(prefix, StringComparison.Ordinal))
            return false;

        int length = input.Length;

        if (length > prefix.Length &&
            input.AsSpan(prefix.Length).StartsWith(prefix, StringComparison.Ordinal))
            return false;

        int index = 0;

        while (index < length && !char.IsWhiteSpace(input[index])) index++;
        commandName = input[prefix.Length..index];

        if (commandName.Length == 0)
            return false;

        if (commandNameMap is not null && !commandNameMap.ContainsKey(commandName))
            return false;

        List<string> parsedArguments = new();

        while (index < length && char.IsWhiteSpace(input[index])) index++;

        StringBuilder argument = new();

        while (index < length)
        {
            argument.Clear();
            char? quoteChar = null;

            char character = input[index];
            for (int quoteIndex = 0; quoteIndex < quoteCharacters.Count; quoteIndex++)
            {
                if (character == quoteCharacters[quoteIndex])
                {
                    quoteChar = character;
                    index++;
                    break;
                }
            }

            bool escaped = false;
            while (index < length)
            {
                character = input[index];

                if (escaped)
                {
                    argument.Append(character);
                    escaped = false;
                }
                else if (character == '\\')
                {
                    escaped = true;
                }
                else if (quoteChar.HasValue && character == quoteChar.Value)
                {
                    index++;
                    break;
                }
                else if (!quoteChar.HasValue && char.IsWhiteSpace(character))
                {
                    break;
                }
                else
                {
                    argument.Append(character);
                }

                index++;
            }

            parsedArguments.Add(argument.ToString());

            while (index < length && char.IsWhiteSpace(input[index])) index++;
        }

        commandArguments = parsedArguments.AsReadOnly();
        return true;
    }
}
