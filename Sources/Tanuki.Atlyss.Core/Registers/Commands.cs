using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tanuki.Atlyss.API.Tanuki.Commands;

namespace Tanuki.Atlyss.Core.Registers;

public sealed class Commands
{
    private readonly Dictionary<string, Type> commandNameMap = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<ulong, Type> commandHashMap = [];
    private readonly Dictionary<Type, Serialization.Commands.Configuration> commandConfigurations = [];
    private readonly Dictionary<Assembly, HashSet<Type>> assemblyCommands = [];
    private readonly Data.Settings.Commands settings;

    public Action<Type>? OnCommandRegistered;
    public Action<Type>? OnCommandDeregistered;

    /// <summary>
    /// Provides a lookup of a command <see cref="Type"/> by its active names.
    /// </summary>
    public IReadOnlyDictionary<string, Type> CommandNameMap => commandNameMap;

    /// <summary>
    /// Provides a lookup of <see cref="Serialization.Commands.Configuration"/> objects by their corresponding command <see cref="Type"/>.
    /// </summary>
    /// <remarks>
    /// Modifying these configurations isn't recommended, as they're managed by the registry.
    /// </remarks>
    public IReadOnlyDictionary<Type, Serialization.Commands.Configuration> CommandConfigurations => commandConfigurations;

    /// <summary>
    /// Provides a lookup of commands grouped by their <see cref="Assembly"/>.
    /// </summary>
    /// <remarks>
    /// Modifying the <see cref="HashSet{T}"/> values isn't recommended, as they're managed by the registry.
    /// </remarks>
    public IReadOnlyDictionary<Assembly, HashSet<Type>> AssemblyCommands => assemblyCommands;

    internal Commands(Data.Settings.Commands settings) => this.settings = settings;

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
                Main.Instance.ManualLogSource.LogError($"Unable to read the configuration file \"{path}\".\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
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
            Main.Instance.ManualLogSource.LogError($"Failed to save the updated configuration file \"{file}\".\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            return;
        }
    }

    private uint ProcessExistingConfiguration(Dictionary<string, Type> pluginCommands, Dictionary<string, Serialization.Commands.Configuration> commandConfigurations)
    {
        string[] prefixes = [settings.ClientPrefix, settings.ServerPrefix];

        uint changes = 0;

        foreach (string commandConfigurationKey in commandConfigurations.Keys.ToArray())
        {
            if (!pluginCommands.TryGetValue(commandConfigurationKey, out Type command))
                continue;

            pluginCommands.Remove(commandConfigurationKey);

            Serialization.Commands.Configuration configuration = commandConfigurations[commandConfigurationKey];

            if (configuration is null)
            {
                Main.Instance.ManualLogSource.LogInfo($"Command configuration entry for {command.FullName} has been restored.");

                configuration = Serialization.Commands.Configuration.CreateFromType(command, settings.Prefixes);
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
                    string? name = configuration.names[nameIndex];
                    string normalizedName = Utilities.Commands.Name.Normalize(name, settings.Prefixes);

                    if (string.IsNullOrEmpty(normalizedName))
                    {
                        Main.Instance.ManualLogSource.LogInfo($"Removed empty command name from {command.FullName}.");

                        configuration.names.RemoveAt(nameIndex);

                        changes++;

                        continue;
                    }

                    if (name != normalizedName)
                    {
                        Main.Instance.ManualLogSource.LogInfo($"Command {command.FullName} name normalized: {name} -> {normalizedName}.");

                        configuration.names[nameIndex] = normalizedName;

                        changes++;
                    }

                    if (commandNameMap.TryGetValue(name, out Type existingCommand))
                    {
                        Main.Instance.ManualLogSource.LogWarning($"Command name \"{name}\" of {command.FullName} is already used by {existingCommand.FullName}.");
                        continue;
                    }

                    commandNameMap.Add(name, command);
                }

                this.commandConfigurations[command] = configuration;
            }

            OnCommandRegistered?.Invoke(command);
        }

        foreach (KeyValuePair<string, Type> pluginCommand in pluginCommands)
        {
            Main.Instance.ManualLogSource.LogInfo($"Command configuration entry for {pluginCommand.Key} has been created.");

            Serialization.Commands.Configuration commandConfiguration = Serialization.Commands.Configuration.CreateFromType(pluginCommand.Value, settings.Prefixes);
            commandConfigurations.Add(pluginCommand.Key, commandConfiguration);

            this.commandConfigurations[pluginCommand.Value] = commandConfiguration;

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

        if (commandHashMap.ContainsKey(hash))
        {
            Main.Instance.ManualLogSource.LogWarning($"Failed to register command {command.FullName} because its hash {hash} is already in use.");
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

        commandHashMap[hash] = command;
        commandConfigurations[command] = default!;

        OnCommandRegistered?.Invoke(command);

        return true;
    }

    public void RegisterCommand<T>(ulong hash) => RegisterCommand(typeof(T), hash);

    public void DeregisterCommand(Type сommandType)
    {
        if (!commandConfigurations.TryGetValue(сommandType, out Serialization.Commands.Configuration configuration))
            return;

        if (configuration is not null)
            foreach (string Name in configuration.names)
                commandNameMap.Remove(Name);

        commandConfigurations.Remove(сommandType);

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
            Main.Instance.ManualLogSource.LogError($"Failed to retrieve types from assembly {assembly.GetName().Name}\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            return;
        }

        Type commandInterfaceType = typeof(ICommand);

        foreach (Type type in assemblyTypes)
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            if (!commandInterfaceType.IsAssignableFrom(type))
                continue;

            if (commandConfigurations.ContainsKey(type))
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
