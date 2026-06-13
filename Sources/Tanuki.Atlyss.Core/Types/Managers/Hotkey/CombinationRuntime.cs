namespace Tanuki.Atlyss.Core.Types.Managers.Hotkey;

internal readonly struct CombinationRuntime(KeyCondition keyCondition, int[] combinationIds)
{
    public readonly KeyCondition KeyCondition = keyCondition;
    public readonly int[] CombinationIds = combinationIds;
}