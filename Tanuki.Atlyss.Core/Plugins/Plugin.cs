using BepInEx;
using System;
using System.IO;
using System.Reflection;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Collections;
using Tanuki.Atlyss.API.Plugins;

namespace Tanuki.Atlyss.Core.Plugins;

public class Plugin : BaseUnityPlugin, IPlugin
{
    public string Name { get; private set; }
    private EState _State = EState.Unloaded;
    public EState State => _State;
    protected Assembly Assembly;
    protected readonly string Directory;
    public Translation Translation;
    public Plugin()
    {
        Assembly = GetType().Assembly;
        Name = Assembly.GetName().Name;
        Directory = Path.Combine(Paths.ConfigPath, Name);

        Translation = new();

        if (!System.IO.Directory.Exists(Directory))
            System.IO.Directory.CreateDirectory(Directory);
    }
    public virtual void LoadPlugin()
    {
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
        string Path = System.IO.Path.Combine(Directory, string.Format(Environment.PluginTranslationFileTemplate, Core.Tanuki.Instance.Settings.Language, Environment.PluginTranslationFileFormat));

        bool Exists = File.Exists(Path);
        if (!Exists)
        {
            foreach (string File in System.IO.Directory.GetFiles(Directory))
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

                Translation.Translations[Line.Substring(0, SplitIndex)] = Line.Substring(SplitIndex + 1);
            }
        }
    }
    public string Translate(string Key, params object[] Placeholder) =>
        Translation.Translate(Key, Placeholder);
}