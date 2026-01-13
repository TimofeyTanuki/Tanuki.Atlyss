using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tanuki.Atlyss.API.Commands;
using Tanuki.Atlyss.Core.Models;

namespace Tanuki.Atlyss.Core.Managers;

public class Commands
{
    public delegate void CommandInstanceCreated(Type Type);
    public event CommandInstanceCreated? OnCommandInstanceCreated;

    public delegate void CommandEntryCreated(Type Type);
    public event CommandEntryCreated? OnCommandEntryCreated;

    public delegate void CommandEntryRemoved(Type Type);
    public event CommandEntryRemoved? OnCommandEntryRemoved;

    private readonly Dictionary<string, Type> _CommandNames = [];
    private readonly Dictionary<Type, CommandEntry> _CommandEntries = [];
    private readonly Dictionary<Assembly, HashSet<Type>> _AssemblyCommands = [];

    /// <summary>
    /// Provides a lookup of a command <see cref="Type"/> by their active names.
    /// </summary>
    /// <remarks>
    /// Moodifying its values isn't recommended, as they're managed by <see cref="Commands"/>
    /// </remarks>
    public IReadOnlyDictionary<string, Type> CommandNames => _CommandNames;

    /// <summary>
    /// Provides a lookup of <see cref="CommandEntry"/> by their command <see cref="Type"/>.
    /// </summary>
    /// <remarks>
    /// Moodifying its values isn't recommended, as they're managed by <see cref="Commands"/>
    /// </remarks>
    public IReadOnlyDictionary<Type, CommandEntry> CommandEntries => _CommandEntries;

    /// <summary>
    /// Provides the command grouped by their <see cref="Assembly"/>
    /// </summary>
    /// <remarks>
    /// Moodifying its values isn't recommended, as they're managed by <see cref="Commands"/>
    /// </remarks>
    public IReadOnlyDictionary<Assembly, HashSet<Type>> AssemblyCommands => _AssemblyCommands;

    private readonly Func<Type, ICommand> CommandFactory = Type => (ICommand)Activator.CreateInstance(Type);

    internal Commands() { }

    private Dictionary<string, CommandConfigurationItem>? TryReadConfiguration(string ConfigurationFile)
    {
        if (File.Exists(ConfigurationFile))
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, CommandConfigurationItem>>(File.ReadAllText(ConfigurationFile)) ?? [];
            }
            catch (Exception Exception)
            {
                Main.Instance.ManualLogSource.LogError($"Unable to read the configuration file \"{ConfigurationFile}\".\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
                return null;
            }
        }

        return [];
    }

    private void TrySaveConfiguration(Dictionary<string, CommandConfigurationItem> CommandConfiguration, string ConfigurationFile)
    {
        try
        {
            File.WriteAllText(ConfigurationFile, JsonConvert.SerializeObject(CommandConfiguration, Formatting.Indented));
        }
        catch (Exception Exception)
        {
            Main.Instance.ManualLogSource.LogMessage($"Failed to save the updated configuration file \"{ConfigurationFile}\".\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
            return;
        }
    }

    private uint UpdateCommand(Dictionary<string, Type> PluginCommandTypes, Dictionary<string, CommandConfigurationItem> CommandConfigurations)
    {
        uint Changes = 0;

        foreach (string CommandConfigurationKey in CommandConfigurations.Keys.ToArray())
        {
            if (!PluginCommandTypes.TryGetValue(CommandConfigurationKey, out Type CommandType))
                continue;

            PluginCommandTypes.Remove(CommandConfigurationKey);

            CommandConfigurationItem CommandConfiguration = CommandConfigurations[CommandConfigurationKey];

            if (CommandConfiguration is null)
            {
                Main.Instance.ManualLogSource.LogMessage($"Command entry for {CommandType.FullName} has been restored.");

                CommandConfiguration = CreateCommandConfiguration(CommandType);
                CommandConfigurations[CommandConfigurationKey] = CommandConfiguration;

                Changes++;
            }
            else
            {
                if (CommandConfiguration.Names is null)
                {
                    CommandConfiguration.Names = [];

                    Changes++;

                    continue;
                }

                for (int CommandNameIndex = CommandConfiguration.Names.Count - 1; CommandNameIndex >= 0; CommandNameIndex--)
                {
                    string? CommandName = CommandConfiguration.Names[CommandNameIndex];
                    string NormalizedCommandName = NormalizeCommandName(CommandName);

                    if (string.IsNullOrEmpty(NormalizedCommandName))
                    {
                        Main.Instance.ManualLogSource.LogMessage($"Removed empty command name from {CommandType.FullName}");

                        CommandConfiguration.Names.RemoveAt(CommandNameIndex);

                        Changes++;

                        continue;
                    }

                    if (CommandName != NormalizedCommandName)
                    {
                        Main.Instance.ManualLogSource.LogMessage($"Command {CommandType.FullName} name normalized: {CommandName} -> {NormalizedCommandName}.");

                        CommandConfiguration.Names[CommandNameIndex] = NormalizedCommandName;

                        Changes++;
                    }

                    if (_CommandNames.TryGetValue(CommandName, out Type ConflictType))
                    {
                        Main.Instance.ManualLogSource.LogWarning($"Command name \"{CommandName}\" of {CommandType.FullName} is already used by {ConflictType.FullName}.");
                        continue;
                    }

                    _CommandNames.Add(CommandName, CommandType);
                }

                UpdateCommandEntryConfiguration(CommandType, CommandConfiguration);
            }

            OnCommandEntryCreated?.Invoke(CommandType);
        }

        foreach (KeyValuePair<string, Type> NewPluginCommandType in PluginCommandTypes)
        {
            Main.Instance.ManualLogSource.LogMessage($"Command entry for {NewPluginCommandType.Key} has been created.");

            CommandConfigurationItem CommandConfiguration = CreateCommandConfiguration(NewPluginCommandType.Value);
            CommandConfigurations.Add(NewPluginCommandType.Key, CommandConfiguration);

            UpdateCommandEntryConfiguration(NewPluginCommandType.Value, CommandConfiguration);

            Changes++;
        }

        return Changes;
    }

    private void UpdateCommandEntryConfiguration(Type CommandType, CommandConfigurationItem CommandConfiguration)
    {
        if (!_CommandEntries.TryGetValue(CommandType, out CommandEntry CommandEntry))
            return;

        CommandEntry.Configuration = CommandConfiguration;
    }

    public void RegisterAssembly(Assembly Assembly, string ConfigurationFile)
    {
        Dictionary<string, Type> PluginCommandTypes = [];

        Type[] AssemblyTypes;
        try
        {
            AssemblyTypes = Assembly.GetTypes();
        }
        catch (Exception Exception)
        {
            Main.Instance.ManualLogSource.LogError($"Failed to retrieve types from assembly {Assembly.GetName().Name}\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
            return;
        }

        Type InterfaceType = typeof(ICommand);

        foreach (Type AssemblyType in AssemblyTypes)
        {
            if (AssemblyType.IsAbstract || AssemblyType.IsInterface)
                continue;

            if (!InterfaceType.IsAssignableFrom(AssemblyType))
                continue;

            if (_CommandEntries.ContainsKey(AssemblyType))
                continue;

            ICommand Command;

            try
            {
                Command = CommandFactory(AssemblyType);
            }
            catch (Exception Exception)
            {
                Main.Instance.ManualLogSource.LogError($"Failed to obtain the interface for the command {AssemblyType.FullName}.\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
                continue;
            }

            if (!_AssemblyCommands.TryGetValue(Assembly, out HashSet<Type> AssemblyCommands))
            {
                AssemblyCommands = [];
                _AssemblyCommands[Assembly] = AssemblyCommands;
            }

            AssemblyCommands.Add(AssemblyType);
            PluginCommandTypes.Add(AssemblyType.FullName, AssemblyType);

            // TODO: ADD HASH CODE FOR NETWORKING

            CommandEntry CommandEntry = new(Command);

            _CommandEntries.Add(
                AssemblyType,
                CommandEntry
            );

            OnCommandInstanceCreated?.Invoke(AssemblyType);
        }

        if (PluginCommandTypes.Count == 0)
            return;

        Dictionary<string, CommandConfigurationItem>? CommandConfiguration = TryReadConfiguration(ConfigurationFile);

        if (CommandConfiguration is null)
            return;

        uint Changes = UpdateCommand(PluginCommandTypes, CommandConfiguration);

        if (Changes == 0)
            return;

        TrySaveConfiguration(CommandConfiguration, ConfigurationFile);


    }

    public void DeregisterCommand(Type CommandType)
    {
        if (!_CommandEntries.TryGetValue(CommandType, out CommandEntry CommandEntry))
            return;

        if (CommandEntry.Configuration is not null)
            foreach (string Name in CommandEntry.Configuration.Names)
                _CommandNames.Remove(Name);

        try
        {
            CommandEntry.Dispose();
        }
        catch (Exception Exception)
        {
            Main.Instance.ManualLogSource.LogError($"Failed to dispose the command {CommandType.FullName}.\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
        }

        _CommandEntries.Remove(CommandType);

        OnCommandEntryRemoved?.Invoke(CommandType);
    }

    public void DeregisterAssembly(Assembly Assembly)
    {
        if (!_AssemblyCommands.TryGetValue(Assembly, out HashSet<Type> AssemblyCommands))
            return;

        foreach (Type AssemblyCommand in AssemblyCommands)
            DeregisterCommand(AssemblyCommand);

        _AssemblyCommands.Remove(Assembly);
    }

    private CommandConfigurationItem CreateCommandConfiguration(Type CommandType)
    {
        CommandConfigurationItem CommandConfiguration = new()
        {
            Names = [NormalizeCommandName(CommandType.FullName)]
        };

        return CommandConfiguration;
    }

    public static string NormalizeCommandName(string? CommandName)
    {
        if (string.IsNullOrEmpty(CommandName))
            return string.Empty;

        int CommandNameLength = CommandName.Length;
        char[] Characters = new char[CommandNameLength];
        int CharacterPosition = 0;

        for (int Index = 0; Index < CommandNameLength; Index++)
        {
            char Character = CommandName[Index];

            if (Character == ' ')
                continue;

            Character = char.ToLowerInvariant(Character);
            Characters[CharacterPosition++] = Character;
        }

        return new string(Characters, 0, CharacterPosition);
    }
}