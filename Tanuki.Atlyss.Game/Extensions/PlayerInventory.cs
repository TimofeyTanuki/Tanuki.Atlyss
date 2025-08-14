namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerInventory
{
    public static ItemData GetItem(this global::PlayerInventory PlayerInventory, int Slot, bool IsEquipped, ItemType ItemType)
    {
        ScriptableItem ScriptableItem;
        foreach (ItemData ItemData in PlayerInventory._heldItems)
        {
            if (ItemData._slotNumber != Slot)
                continue;

            if (ItemData._isEquipped != IsEquipped)
                continue;

            ScriptableItem = GameManager._current.Locate_Item(ItemData._itemName);
            if (ScriptableItem is null)
                continue;

            if (ScriptableItem._itemType != ItemType)
                continue;

            return ItemData;
        }
        return null;
    }
}