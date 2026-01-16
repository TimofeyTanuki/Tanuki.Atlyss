using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Managers;

[Obsolete("ligmaballs")]
public class CommandsLegacy
{
    private static readonly HashSet<char> Quotes = ['"', '\'', '`'];

    public void OnSendMessage(string Message, ref bool ShouldAllow)
    {
        /*
        Main.Instance.ManualLogSource.LogDebug($"OnSendMessage: {Message}");

        if (!Message.StartsWith("/"))
            return;

        int ArgumentsIndex = Message.IndexOf(' ');
        if (ArgumentsIndex < 0)
            ArgumentsIndex = Message.Length;

        string CommandName = Message[1..ArgumentsIndex].ToLower();

        Dictionary<string, Type> names = (Dictionary<string, Type>)typeof(Commands).GetField("_CommandMap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Tanuki.Instance.CommandRegistry);

        if (!names.TryGetValue(CommandName, out Type Type))
            return;

        List<string> ArgumentsList = [];
        StringBuilder Argument = new();
        char? ArgumentOpenQuote = null;
        bool ArgumentQuoteEscaped = false;

        char Character;
        for (ushort i = (ushort)ArgumentsIndex; i < Message.Length; i++)
        {
            Character = Message[i];
            if (ArgumentOpenQuote.HasValue)
            {
                if (ArgumentQuoteEscaped)
                {
                    if (Character == '\\' || Quotes.Contains(Character))
                        Argument.Append(Character);
                    else
                    {
                        Argument.Append('\\');
                        Argument.Append(Character);
                    }
                    ArgumentQuoteEscaped = false;
                }
                else
                {
                    if (Character == '\\')
                        ArgumentQuoteEscaped = true;
                    else if (Character == ArgumentOpenQuote)
                    {
                        ArgumentsList.Add(Argument.ToString());
                        Argument.Clear();
                        ArgumentOpenQuote = null;
                    }
                    else
                        Argument.Append(Character);
                }
            }
            else
            {
                if (Character == ' ')
                {
                    if (Argument.Length > 0)
                    {
                        ArgumentsList.Add(Argument.ToString());
                        Argument.Clear();
                    }
                }
                else if (Quotes.Contains(Character))
                {
                    if (Argument.Length > 0)
                    {
                        ArgumentsList.Add(Argument.ToString());
                        Argument.Clear();
                    }
                    ArgumentOpenQuote = Character;
                }
                else
                    Argument.Append(Character);
            }
        }

        if (ArgumentQuoteEscaped)
            Argument.Append('\\');

        if (Argument.Length > 0)
            ArgumentsList.Add(Argument.ToString());

        ShouldAllow = false;

        string[] Arguments = [.. ArgumentsList];

        // Search local command then send to server if Tanuki.Network on server

        Dictionary<Type, Entry> Commands = (Dictionary<Type, Entry>)typeof(Commands).GetField("_CommandEntries", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Tanuki.Instance.CommandRegistry);
        ICommand Command = Commands[Type].command;

        try
        {
            ShouldAllow = Command.Execute(new() { Arguments = Arguments });

            ChatBehaviour._current._chatAssets._chatInput.text = string.Empty;
            ChatBehaviour._current.Display_Chat(false);
        }
        catch (Exception Exception)
        {
            Main.Instance.ManualLogSource.LogError($"Error executing command \"{Command.GetType().FullName}\"\nException:\n{Exception}");
        }
        */
    }
}