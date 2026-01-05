using BepInEx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Collections;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Plugins;

public class Plugin : BaseUnityPlugin, IPlugin
{
    public string Name { get; private set; }
    private EState _State = EState.Unloaded;
    public EState State => _State;

    public event IPlugin.Load OnLoad;
    public event IPlugin.Loaded OnLoaded;
    public event IPlugin.Unload OnUnload;
    public event IPlugin.Unloaded OnUnloaded;

    protected readonly string ConfigurationDirectory;
    public Translation Translation;

    public Plugin()
    {
        Name = GetType().Assembly.GetName().Name;
        string[] Directories = Directory.GetDirectories(Paths.ConfigPath, Name, SearchOption.AllDirectories);
        ConfigurationDirectory = Directories.Length > 0 ? Directories[0] : Path.Combine(Paths.ConfigPath, Name);

        Translation = new();
    }

    public virtual void LoadPlugin()
    {
        OnLoad?.Invoke();

        Tanuki.Instance.Commands.RegisterCommands(this);
        LoadTranslation();

        try
        {
            Load();
        }
        catch (Exception Exception)
        {
            Logger.LogError(Exception);
            UnloadPlugin(EState.Failure);
            return;
        }

        _State = EState.Loaded;
        OnLoaded?.Invoke();
    }

    public virtual void UnloadPlugin(EState PluginState)
    {
        OnUnload?.Invoke();
        Tanuki.Instance.Commands.DeregisterCommands(this);

        try
        {
            Unload();
        }
        catch (Exception Exception)
        {
            Logger.LogError(Exception);
            _State = EState.Failure;
            return;
        }

        _State = PluginState;
        OnUnloaded?.Invoke();
    }

    protected virtual void Load() { }

    protected virtual void Unload() { }

    private void LoadTranslation()
    {
        if (!Directory.Exists(ConfigurationDirectory))
            Directory.CreateDirectory(ConfigurationDirectory);

        string Path = System.IO.Path.Combine(ConfigurationDirectory, Environment.FormatPluginTranslationsFile(Tanuki.Instance.Settings.Language));

        bool Exists = File.Exists(Path);
        if (!Exists)
        {
            string[] Files = Directory.GetFiles(ConfigurationDirectory, Environment.FormatPluginTranslationsFile("*"));
            if (Files.Length > 0)
            {
                Path = Files[0];
                Exists = true;
            }
        }

        if (Exists)
        {
            Translation.Translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path));
            if (Translation.Translations is not null)
                return;
        }

        Translation.Translations = [];
    }

    public string Translate(string Key, params object[] Placeholder) =>
        Translation.Translate(Key, Placeholder);
}