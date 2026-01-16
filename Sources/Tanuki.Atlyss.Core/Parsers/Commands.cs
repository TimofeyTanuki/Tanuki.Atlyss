using System;
using System.Collections.Generic;
using System.Text;

namespace Tanuki.Atlyss.Core.Parsers;


public class Commands(IReadOnlyList<char> quoteCharacters)
{
    private readonly IReadOnlyList<char> quoteCharacters = quoteCharacters ?? throw new ArgumentNullException(nameof(quoteCharacters));


    public bool TryParse<T>(
        string prefix,
        string input,
        IReadOnlyDictionary<string, T>? commandNameMap,
        out string commandName,
        out IReadOnlyList<string> commandArguments)
    {
        commandName = null!;
        commandArguments = null!;
        Console.WriteLine("ST1");
        if (string.IsNullOrEmpty(input))
            return false;

        Console.WriteLine("ST2");
        if (!input.StartsWith(prefix))
            return false;

        Console.WriteLine("ST3");
        int length = input.Length;
        int index = 0;
        Console.WriteLine("ST34");

        while (index < length && !char.IsWhiteSpace(input[index])) index++;
        commandName = input[prefix.Length..index];

        Console.WriteLine($"ST6 {commandName}");
        if (commandNameMap is not null && !commandNameMap.ContainsKey(commandName))
            return false;
        Console.WriteLine("ST");

        var parsedArguments = new List<string>();

        while (index < length && char.IsWhiteSpace(input[index])) index++;

        var argument = new StringBuilder();

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
