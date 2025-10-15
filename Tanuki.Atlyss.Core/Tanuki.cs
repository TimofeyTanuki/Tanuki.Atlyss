﻿using BepInEx.Logging;

namespace Tanuki.Atlyss.Core;

public class Tanuki
{
    public static Tanuki Instance;

    internal readonly ManualLogSource ManualLogSource = Logger.CreateLogSource("Tanuki.Atlyss.Core");
    public readonly Settings Settings;
    public readonly Commands.Manager Commands;
    public readonly Plugins.Manager Plugins;
    private bool Loaded = false;

    private Tanuki()
    {
        Settings = new();
        Commands = new();
        Plugins = new();
    }

    public static void Initialize()
    {
        if (Instance is not null)
            return;

        Game.Fields.GameManager.Initialize();
        Patching.Core.Initialize();
        Instance = new();
    }
    public void Load()
    {
        if (Loaded)
            return;

        Loaded = true;
        Plugins.LoadPlugins();
    }
    public void Reload()
    {
        Patching.Core.Instance.UnpatchAll();
        Commands.RemoveAllCommands();
        Plugins.ReloadPlugins();
    }
}