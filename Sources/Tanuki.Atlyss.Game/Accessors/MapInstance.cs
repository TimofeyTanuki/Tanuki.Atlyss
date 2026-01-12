using HarmonyLib;

namespace Tanuki.Atlyss.Game.Accessors;

public static class MapInstance
{
    public static readonly AccessTools.FieldRef<global::MapInstance, MapInstanceVisuals> _mapVisuals;
    public static readonly AccessTools.FieldRef<global::MapInstance, float> _currentWeatherIntervalBuffer;

    static MapInstance()
    {
        _currentWeatherIntervalBuffer = AccessTools.FieldRefAccess<global::MapInstance, float>("_currentWeatherIntervalBuffer");
        _mapVisuals = AccessTools.FieldRefAccess<global::MapInstance, MapInstanceVisuals>("_mapVisuals");
    }
}