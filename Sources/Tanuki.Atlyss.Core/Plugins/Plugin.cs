using BepInEx;
using System;
using System.IO;
using Tanuki.Atlyss.API.Plugins;
using Tanuki.Atlyss.Core.Models;

namespace Tanuki.Atlyss.Core.Plugins;

public abstract class Plugin : BaseUnityPlugin, IPlugin
{
    public virtual string Name { get; protected set; }
    private EState _State = EState.Unloaded;
    public EState State => _State;

    protected string ConfigurationDirectory = null!;

    public JsonTranslationSet Translation;

    public event Action? OnLoad;
    public event Action? OnLoaded;
    public event Action? OnUnload;
    public event Action? OnUnloaded;

    protected Plugin()
    {
        Name = GetType().Assembly.GetName().Name;
        Translation = [];
    }

    private void ProcessTranslationSet()
    {
        Translation.Clear();

        string Path = Helpers.LanguageFileSelector.GetPreferredFile(Tanuki.Instance.Settings.PreferredLanguageOrder, ConfigurationDirectory, ".translations.json");

        if (!File.Exists(Path))
            return;

        try
        {
            Translation.LoadFromFile(Path);
        }
        catch (Exception Exception)
        {
            Logger.LogError($"Failed to load translation set from file \"{Path}\".\nException message:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
        }
    }

    private void PrepareConfigurationDirectory()
    {
        if (string.IsNullOrEmpty(ConfigurationDirectory))
        {
            string[] SuitableDirectories = Directory.GetDirectories(Paths.ConfigPath, Name, SearchOption.AllDirectories);
            ConfigurationDirectory = SuitableDirectories.Length > 0 ? SuitableDirectories[0] : Path.Combine(Paths.ConfigPath, Name);
        }

        if (!Directory.Exists(ConfigurationDirectory))
            Directory.CreateDirectory(ConfigurationDirectory);
    }

    protected virtual void Load() { }

    protected virtual void Unload() { }

    public virtual void LoadPlugin()
    {
        OnLoad?.Invoke();

        PrepareConfigurationDirectory();

        ProcessTranslationSet();

        Tanuki.Instance.Commands.RegisterAssembly(GetType().Assembly, Helpers.LanguageFileSelector.GetPreferredFile(Tanuki.Instance.Settings.PreferredLanguageOrder, ConfigurationDirectory, ".commands.json"));

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
        Tanuki.Instance.Commands.DeregisterAssembly(GetType().Assembly);

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

    public virtual string Translate(string Key, params object[] Placeholder) =>
        Translation.Translate(Key, Placeholder);
}