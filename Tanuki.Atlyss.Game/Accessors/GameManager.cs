using HarmonyLib;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game.Accessors;

public static class GameManager
{
    public static readonly AccessTools.FieldRef<global::GameManager, Dictionary<string, ScriptableItem>> _cachedScriptableItems;
    public static readonly AccessTools.FieldRef<global::GameManager, Dictionary<string, ScriptablePlayerRace>> _cachedScriptableRaces;
    public static readonly AccessTools.FieldRef<global::GameManager, Dictionary<string, ScriptableMapData>> _cachedScriptableMapDatas;
    public static readonly AccessTools.FieldRef<global::GameManager, Dictionary<string, ScriptableQuest>> _cachedScriptableQuests;

    static GameManager()
    {
        _cachedScriptableItems = AccessTools.FieldRefAccess<global::GameManager, Dictionary<string, ScriptableItem>>("_cachedScriptableItems");
        _cachedScriptableRaces = AccessTools.FieldRefAccess<global::GameManager, Dictionary<string, ScriptablePlayerRace>>("_cachedScriptableRaces");
        _cachedScriptableMapDatas = AccessTools.FieldRefAccess<global::GameManager, Dictionary<string, ScriptableMapData>>("_cachedScriptableMapDatas");
        _cachedScriptableQuests = AccessTools.FieldRefAccess<global::GameManager, Dictionary<string, ScriptableQuest>>("_cachedScriptableQuests");
    }
}