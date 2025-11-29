using BepInEx;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Collections;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Plugins;

public class Plugin : BaseUnityPlugin, IPlugin
{
    public string Name { get; private set; }
    private EState _State = EState.Unloaded;
    public EState State => _State;
    protected readonly string ConfigurationDirectory;
    public Translation Translation;
    public Plugin()
    {
        Name = GetType().Assembly.GetName().Name;
        ConfigurationDirectory = Path.Combine(Paths.ConfigPath, Name);
        Translation = new();
    }
    public virtual void LoadPlugin()
    {
        if (!Directory.Exists(ConfigurationDirectory))
            Directory.CreateDirectory(ConfigurationDirectory);

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
    }
    public virtual void UnloadPlugin(EState PluginState)
    {
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
    }
    protected virtual void Load() { }
    protected virtual void Unload() { }
    private void LoadTranslation()
    {
        string Path = System.IO.Path.Combine(ConfigurationDirectory, string.Format(Environment.PluginTranslationFileTemplate, Tanuki.Instance.Settings.Language, Environment.PluginTranslationFileFormat));

        bool Exists = File.Exists(Path);
        if (!Exists)
        {
            foreach (string File in System.IO.Directory.GetFiles(ConfigurationDirectory))
            {
                if (!File.Contains(Environment.PluginTranslationFileFormat))
                    continue;

                Path = File;
                Exists = true;
                break;
            }
        }

        Translation.Translations.Clear();

        if (Exists)
        {
            using FileStream FileStream = File.OpenRead(Path);
            using StreamReader StreamReader = new(FileStream);

            string Line;
            int SplitIndex = 0;
            while ((Line = StreamReader.ReadLine()) != null)
            {
                if (Line.StartsWith("#"))
                    continue;

                SplitIndex = Line.IndexOf('=');

                if (SplitIndex <= 0)
                    continue;

                Translation.Translations[Line.Substring(0, SplitIndex)] = Regex.Unescape(Line.Substring(SplitIndex + 1));
            }
        }
    }
    public string Translate(string Key, params object[] Placeholder) =>
        Translation.Translate(Key, Placeholder);
}