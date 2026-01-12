using HarmonyLib;

namespace Tanuki.Atlyss.Game.Accessors;

public static class GameWorldManager
{
    public static readonly AccessTools.FieldRef<global::GameWorldManager, float> _currentDayNightCycleBuffer;

    static GameWorldManager() =>
        _currentDayNightCycleBuffer = AccessTools.FieldRefAccess<global::GameWorldManager, float>("_currentDayNightCycleBuffer");
}