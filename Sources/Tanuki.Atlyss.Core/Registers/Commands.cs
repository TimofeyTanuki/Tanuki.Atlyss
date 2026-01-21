using BepInEx.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Registers;

public sealed class Commands
{
    private readonly ManualLogSource manualLogSource;
    private readonly Dictionary<string, Type> nameMap = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<ulong, Type> hashMap = [];
    private readonly Dictionary<Type, Data.Commands.Descriptor> descriptors = [];
    private readonly Dictionary<Assembly, HashSet<Type>> assemblyCommands = [];
    private readonly Data.Settings.Commands commandSettings;

    public Action<Type>? OnCommandRegistered;
    public Action<Type>? OnCommandDeregistered;

    /// <summary>
    /// Provides a lookup of a command <see cref="Type"/> by its active names.
    /// </summary>
    public IReadOnlyDictionary<string, Type> NameMap => nameMap;

    /// <summary>
    /// Provides a lookup of <see cref="Serialization.Commands.Configuration"/> objects by their corresponding command <see cref="Type"/>.
    /// </summary>
    public IReadOnlyDictionary<Type, Data.Commands.Descriptor> Descriptors => descriptors;

    /// <summary>
    /// Provides a lookup of commands grouped by their <see cref="Assembly"/>.
    /// </summary>
    /// <remarks>
    /// Modifying the <see cref="HashSet{T}"/> values isn't recommended, as they're managed by the registry.
    /// </remarks>
    public IReadOnlyDictionary<Assembly, HashSet<Type>> AssemblyCommands => assemblyCommands;
    public IReadOnlyDictionary<ulong, Type> HashMap => hashMap;

    internal Commands(ManualLogSource manualLogSource, Data.Settings.Commands settings)
    {
        this.manualLogSource = manualLogSource;
        this.commandSettings = settings;
    }

    public Dictionary<string, Serialization.Commands.Configuration>? TryReadConfiguration(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Serialization.Commands.Configuration>>(File.ReadAllText(path)) ?? [];
            }
            catch (Exception exception)
            {
                manualLogSource.LogError($"Unable to read the configuration file \"{path}\".\nException:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
                return null;
            }
        }

        return [];
    }

    public void TrySaveConfiguration(IReadOnlyDictionary<string, Serialization.Commands.Configuration> configuration, string file)
    {
        try
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(configuration, Formatting.Indented));
        }
        catch (Exception exception)
        {
            manualLogSource.LogError($"Failed to save the updated configuration file \"{file}\".\nException:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            return;
        }
    }

    private uint ProcessExistingConfiguration(Dictionary<string, Type> pluginCommands, Dictionary<string, Serialization.Commands.Configuration> commandConfigurations)
    {
        string[] prefixes = [commandSettings.ClientPrefix, commandSettings.ServerPrefix];

        uint changes = 0;

        foreach (string commandConfigurationKey in commandConfigurations.Keys.ToArray())
        {
            if (!pluginCommands.TryGetValue(commandConfigurationKey, out Type command))
                continue;

            pluginCommands.Remove(commandConfigurationKey);

            Serialization.Commands.Configuration configuration = commandConfigurations[commandConfigurationKey];

            if (configuration is null)
            {
                manualLogSource.LogInfo($"Command configuration entry for {command.FullName} has been restored.");

                configuration = Serialization.Commands.Configuration.CreateFromType(command, commandSettings.Prefixes);
                commandConfigurations[commandConfigurationKey] = configuration;
                changes++;
            }
            else
            {
                if (configuration.names is null)
                {
                    configuration.names = [];
                    changes++;

                    continue;
                }

                for (int nameIndex = configuration.names.Count - 1; nameIndex >= 0; nameIndex--)
                {
                    manualLogSource.LogInfo(string.Join(", ", nameMap.Keys));
                    string? name = configuration.names[nameIndex];
                    string normalizedName = Utilities.Commands.Name.Normalize(name, commandSettings.Prefixes);

                    if (string.IsNullOrEmpty(normalizedName))
                    {
                        manualLogSource.LogInfo($"Removed empty command name from {command.FullName}.");

                        configuration.names.RemoveAt(nameIndex);
                        changes++;

                        continue;
                    }

                    if (nameMap.TryGetValue(normalizedName, out Type existingCommand))
                    {
                        if (command == existingCommand)
                        {
                            configuration.names.RemoveAt(nameIndex);
                            changes++;
                        }
                        else
                            manualLogSource.LogWarning($"Command name \"{name}\" of {command.FullName} is already used by {existingCommand.FullName}.");

                        continue;
                    }

                    if (name != normalizedName)
                    {
                        manualLogSource.LogInfo($"Command {command.FullName} name normalized: {name} -> {normalizedName}.");

                        configuration.names[nameIndex] = normalizedName;
                        changes++;
                    }

                    nameMap.Add(normalizedName, command);
                }

                descriptors[command].configuration = configuration;
            }

            OnCommandRegistered?.Invoke(command);
        }

        foreach (KeyValuePair<string, Type> pluginCommand in pluginCommands)
        {
            manualLogSource.LogInfo($"Command configuration entry for {pluginCommand.Key} has been created.");

            Serialization.Commands.Configuration newCommandConfiguration = Serialization.Commands.Configuration.CreateFromType(pluginCommand.Value, commandSettings.Prefixes);
            commandConfigurations.Add(pluginCommand.Key, newCommandConfiguration);

            descriptors[pluginCommand.Value].configuration = newCommandConfiguration;

            changes++;
        }

        return changes;
    }

    private void ProcessConfigurationFile(Dictionary<string, Type> pluginCommands, string configurationFile)
    {
        Dictionary<string, Serialization.Commands.Configuration>? commandConfiguration = TryReadConfiguration(configurationFile);

        if (commandConfiguration is null)
            return;

        uint Changes = ProcessExistingConfiguration(pluginCommands, commandConfiguration);

        if (Changes == 0)
            return;

        TrySaveConfiguration(commandConfiguration, configurationFile);
    }

    public bool RegisterCommand(Type command, ulong hash)
    {
        Assembly assembly = command.Assembly;

        if (hashMap.ContainsKey(hash))
        {
            manualLogSource.LogWarning($"Failed to register command {command.FullName} because its Hash {hash} is already in use.");
            return false;
        }

        if (!this.assemblyCommands.TryGetValue(assembly, out HashSet<Type> assemblyCommands))
        {
            assemblyCommands = [];
            this.assemblyCommands[assembly] = assemblyCommands;
        }

        if (assemblyCommands.Contains(command))
            return false;

        assemblyCommands.Add(command);

        hashMap[hash] = command;
        descriptors[command] = new(hash, null);

        OnCommandRegistered?.Invoke(command);

        return true;
    }

    public void RegisterCommand<T>(ulong hash) => RegisterCommand(typeof(T), hash);

    public void DeregisterCommand(Type сommandType)
    {
        if (!descriptors.TryGetValue(сommandType, out Data.Commands.Descriptor entry))
            return;

        Serialization.Commands.Configuration? configuration = entry.configuration;

        if (configuration is not null)
            foreach (string Name in configuration.names)
                nameMap.Remove(Name);

        descriptors.Remove(сommandType);
        hashMap.Remove(entry.Hash);

        OnCommandDeregistered?.Invoke(сommandType);
    }

    public void DeregisterCommand<T>() => DeregisterCommand(typeof(T));

    public void RegisterAssembly(Assembly assembly, string? configurationFile)
    {
        Dictionary<string, Type> pluginCommands = [];

        Type[] assemblyTypes;
        try
        {
            assemblyTypes = assembly.GetTypes();
        }
        catch (Exception exception)
        {
            manualLogSource.LogError($"Failed to retrieve types from assembly {assembly.GetName().Name}\nException:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            return;
        }

        Type commandInterfaceType = typeof(ICommand);

        foreach (Type type in assemblyTypes)
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            if (!commandInterfaceType.IsAssignableFrom(type))
                continue;

            if (descriptors.ContainsKey(type))
                continue;

            if (!RegisterCommand(type, Utilities.Commands.Hash.Generate(type)))
                continue;

            pluginCommands.Add(type.FullName, type);
        }

        if (pluginCommands.Count == 0)
            return;

        if (!string.IsNullOrEmpty(configurationFile))
            ProcessConfigurationFile(pluginCommands, configurationFile);
    }

    public void DeregisterAssembly(Assembly assembly)
    {
        if (!this.assemblyCommands.TryGetValue(assembly, out HashSet<Type> assemblyCommands))
            return;

        foreach (Type command in assemblyCommands)
            DeregisterCommand(command);

        this.assemblyCommands.Remove(assembly);
    }
}
