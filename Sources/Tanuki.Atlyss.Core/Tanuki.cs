using Tanuki.Atlyss.Core.Managers;

namespace Tanuki.Atlyss.Core;

public sealed class Tanuki
{
    public static Tanuki Instance { get; internal set; } = null!;

    public readonly Data.Tanuki.Settings Settings;
    public readonly Data.Tanuki.Registers Registers;
    public readonly Data.Tanuki.Managers Managers;

    internal Tanuki()
    {
        Settings = new()
        {
            Translations = new(),
            Commands = new()
        };

        Registers = new()
        {
            Plugins = new(),
            Commands = new(Settings.Commands)
        };

        Managers = new()
        {
            Settings = new(Settings),
            Plugins = new(Registers.Plugins),
            Commands = new(Registers.Commands, Settings.Commands)
        };

        Managers.Chat = new(Managers.Commands);
    }
}