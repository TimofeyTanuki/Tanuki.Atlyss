using BepInEx;
using System;
using System.IO;
using Tanuki.Atlyss.API.Core.Plugins;
using Tanuki.Atlyss.Core.Serialization.Translation;

namespace Tanuki.Atlyss.Core.Bases;

public abstract class Plugin : BaseUnityPlugin, IPlugin
{
    public virtual string Name { get; protected set; }
    private EState _State = EState.Unloaded;
    public EState State => _State;

    public event Action? OnLoad;
    public event Action? OnLoaded;
    public event Action? OnUnload;
    public event Action? OnUnloaded;

    protected string configurationDirectory = null!;
    public JsonTranslationSet translationSet;

    protected Plugin()
    {
        Name = GetType().Assembly.GetName().Name;
        translationSet = [];
    }

    private void ProcessTranslationSet()
    {
        translationSet.Clear();

        string path = Utilities.Translations.LanguageFileSelector.GetPreferredFile(
            Tanuki.Instance.Settings.Translations.PreferredLanguageOrder,
            configurationDirectory,
            ".translations.json"
        );

        if (!File.Exists(path))
            return;

        try
        {
            translationSet.LoadFromFile(path);
        }
        catch (Exception exception)
        {
            Logger.LogError($"Failed to load translation set from file \"{path}\".\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
        }
    }

    protected virtual void Load() { }

    protected virtual void Unload() { }

    public virtual void LoadPlugin()
    {
        OnLoad?.Invoke();

        if (configurationDirectory is null)
        {
            configurationDirectory = Utilities.Plugins.ConfigurationDirectory.Get(Paths.ConfigPath, Name);

            if (!Directory.Exists(configurationDirectory))
                Directory.CreateDirectory(configurationDirectory);
        }

        ProcessTranslationSet();

        Tanuki.Instance.Registers.Commands.DeregisterAssembly(GetType().Assembly);

        Tanuki.Instance.Registers.Commands.RegisterAssembly(
            GetType().Assembly,
            Utilities.Translations.LanguageFileSelector.GetPreferredFile(
                Tanuki.Instance.settings.translations.PreferredLanguageOrder,
                configurationDirectory,
                ".commands.json"
            )
        );

        try
        {
            Load();
        }
        catch (Exception exception)
        {
            Logger.LogError($"An exception occurred while loading the plugin.\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            UnloadPlugin(EState.Failure);
            return;
        }

        _State = EState.Loaded;

        OnLoaded?.Invoke();
    }

    public virtual void UnloadPlugin(EState state)
    {
        OnUnload?.Invoke();

        Tanuki.Instance.Registers.Commands.DeregisterAssembly(GetType().Assembly);

        try
        {
            Unload();
        }
        catch (Exception exception)
        {
            Logger.LogError($"An exception occurred while unloading the plugin.\nException message:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            _State = EState.Failure;
            return;
        }

        _State = state;

        OnUnloaded?.Invoke();
    }

    public virtual string Translate(string key, params object[] placeholder) =>
        translationSet.Translate(key, placeholder);
}
