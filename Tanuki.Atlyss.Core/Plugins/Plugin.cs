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
    public Assembly Assembly { get; private set; }
    public string Name { get; private set; }
    public string Directory { get; private set; }
    private EState _State = EState.Unloaded;
    public EState State => _State;
    public Settings Settings { get; set; }
    public Translation Translation;
    public Plugin()
    {
        Assembly = GetType().Assembly;
        Name = Assembly.GetName().Name;
        Directory = Path.Combine(Paths.ConfigPath, Name);
        Settings = new Settings();
        Translation = new();

        if (!System.IO.Directory.Exists(Directory))
            System.IO.Directory.CreateDirectory(Directory);
    }
    public virtual void LoadPlugin()
    {
        if (string.IsNullOrEmpty(Settings.Language))
            Settings.Language = Core.Settings.Language;

        Settings.Translation = Path.Combine(Directory, string.Format(Environment.PluginTranslationFileTemplate, Settings.Language, Environment.PluginTranslationFileFormat));

        LoadTranslation();

        Settings.Command = Path.Combine(Directory, string.Format(Environment.PluginCommandFileTemplate, Settings.Language, Environment.PluginCommandFileFormat));
        Tanuki.Instance.Commands.RegisterCommands(this);

        try
        {
            Load();
        }
        catch (Exception Exception)
        {
            Logger.LogError(Exception);
            UnloadPlugin(EState.Failure);
        }
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
            UnloadPlugin(EState.Failure);
        }
        _State = PluginState;
    }
    public void ReloadPlugin()
    {
        UnloadPlugin(EState.Unloaded);
        LoadPlugin();
    }
    protected virtual void Load()
    {

    }
    protected virtual void Unload()
    {

    }
    public string Translate(string Key, params object[] Placeholder) => Translation.Translate(Key, Placeholder);
    private void LoadTranslation()
    {
        bool Exists = File.Exists(Settings.Translation);
        if (!Exists)
        {
            foreach (string File in System.IO.Directory.GetFiles(Directory))
            {
                if (!File.Contains(Environment.PluginTranslationFileFormat))
                    continue;

                Settings.Translation = File;
                Exists = true;
                break;
            }
        }

        Translation.Translations.Clear();

        if (Exists)
        {
            using FileStream FileStream = File.OpenRead(Settings.Translation);
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
}