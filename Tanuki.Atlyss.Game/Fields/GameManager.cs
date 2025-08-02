using System.Collections.Generic;
using System.Reflection;

namespace Tanuki.Atlyss.Game.Fields;

public class GameManager
{
    public static GameManager Instance;
    private GameManager() { }
    public static void Initialize() =>
        Instance ??= new();

    private Dictionary<string, ScriptableItem> _CachedScriptableItems;
    public Dictionary<string, ScriptableItem> CachedScriptableItems
    {
        get
        {
            _CachedScriptableItems ??= (Dictionary<string, ScriptableItem>)global::GameManager._current.GetType().GetField("_cachedScriptableItems", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::GameManager._current);
            return _CachedScriptableItems;
        }
    }

    private Dictionary<string, ScriptablePlayerRace> _CachedScriptableRaces;
    public Dictionary<string, ScriptablePlayerRace> CachedScriptableRaces
    {
        get
        {
            _CachedScriptableRaces ??= (Dictionary<string, ScriptablePlayerRace>)global::GameManager._current.GetType().GetField("_cachedScriptableRaces", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::GameManager._current);
            return _CachedScriptableRaces;
        }
    }

    private Dictionary<string, ScriptableMapData> _CachedScriptableMapDatas;
    public Dictionary<string, ScriptableMapData> CachedScriptableMapDatas
    {
        get
        {
            _CachedScriptableMapDatas ??= (Dictionary<string, ScriptableMapData>)global::GameManager._current.GetType().GetField("_cachedScriptableMapDatas", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::GameManager._current);
            return _CachedScriptableMapDatas;
        }
    }
}