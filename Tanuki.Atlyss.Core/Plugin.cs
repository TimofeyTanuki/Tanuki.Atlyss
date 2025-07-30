using BepInEx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tanuki.Atlyss.API;
using Tanuki.Atlyss.API.Collections;

namespace Tanuki.Atlyss.Core;

public class Plugin : BaseUnityPlugin, IPlugin
{
    public Assembly Assembly { get; private set; }
    public string Name { get; private set; }
    public string Directory { get; private set; }
    private EPluginState _State = EPluginState.Unloaded;
    public EPluginState State => _State;
    public PluginSettings Settings { get; private set; }

    public Translation Translation { get; internal set; }
    public Plugin()
    {
        Assembly = GetType().Assembly;
        Name = Assembly.GetName().Name;
        Directory = Path.Combine(Paths.PluginPath, Name);
        Settings = new()
        {
            Language = Core.Settings.Language
        };

        Translation = new();

        if (!System.IO.Directory.Exists(Directory))
            System.IO.Directory.CreateDirectory(Directory);

        /*
        Settings = new()
        {
            Language = null
        };
        */
    }
    public virtual void LoadPlugin()
    {
        LoadTranslation();
        try
        {
            Load();
        }
        catch (Exception Exception)
        {
            Logger.LogError(Exception);
            UnloadPlugin(EPluginState.Failure);
        }
    }
    public virtual void UnloadPlugin(EPluginState PluginState)
    {
        Unload();
        _State = PluginState;
    }
    public void ReloadPlugin()
    {
        UnloadPlugin(EPluginState.Unloaded);
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
        string Path = System.IO.Path.Combine(Directory, string.Format(Environment.PluginTranslationFileTemplate, Settings.Language, Environment.PluginTranslationFileFormat));
        bool Exists = File.Exists(Path);
        if (!Exists)
        {
            foreach (string File in System.IO.Directory.GetFiles(Directory))
            {
                Logger.LogInfo(File);
                if (!File.Contains(Environment.PluginTranslationFileFormat))
                    continue;

                Path = File;
                Exists = true;
                break;
            }
        }

        if (Exists)
            Translation.Translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path));

        Translation.Translations ??= [];
    }
}