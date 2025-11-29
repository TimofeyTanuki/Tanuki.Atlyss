using System.Collections.Generic;
using System.Reflection;

namespace Tanuki.Atlyss.Game.Fields;

public static class GameManager
{

    private static Dictionary<string, ScriptableItem> _CachedScriptableItems;
    public static Dictionary<string, ScriptableItem> CachedScriptableItems
    {
        get
        {
            _CachedScriptableItems ??= (Dictionary<string, ScriptableItem>)global::GameManager._current.GetType().GetField("_cachedScriptableItems", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::GameManager._current);
            return _CachedScriptableItems;
        }
    }

    private static Dictionary<string, ScriptablePlayerRace> _CachedScriptableRaces;
    public static Dictionary<string, ScriptablePlayerRace> CachedScriptableRaces
    {
        get
        {
            _CachedScriptableRaces ??= (Dictionary<string, ScriptablePlayerRace>)global::GameManager._current.GetType().GetField("_cachedScriptableRaces", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::GameManager._current);
            return _CachedScriptableRaces;
        }
    }

    private static Dictionary<string, ScriptableMapData> _CachedScriptableMapDatas;
    public static Dictionary<string, ScriptableMapData> CachedScriptableMapDatas
    {
        get
        {
            _CachedScriptableMapDatas ??= (Dictionary<string, ScriptableMapData>)global::GameManager._current.GetType().GetField("_cachedScriptableMapDatas", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::GameManager._current);
            return _CachedScriptableMapDatas;
        }
    }
}