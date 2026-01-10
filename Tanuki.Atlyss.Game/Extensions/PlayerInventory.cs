namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerInventory
{
    extension(global::PlayerInventory PlayerInventory)
    {
        public ItemData GetItem(int Slot, bool IsEquipped, ItemType ItemType)
        {
            foreach (ItemData ItemData in PlayerInventory._heldItems)
            {
                if (ItemData._slotNumber != Slot ||
                    ItemData._isEquipped != IsEquipped)
                    continue;

                ScriptableItem ScriptableItem = GameManager._current.Locate_Item(ItemData._itemName);
                if (!ScriptableItem)
                    continue;

                if (ScriptableItem._itemType != ItemType)
                    continue;

                return ItemData;
            }

            return null;
        }
    }
}