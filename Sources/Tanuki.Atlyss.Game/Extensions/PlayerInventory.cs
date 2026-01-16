namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerInventory
{
    extension(global::PlayerInventory instance)
    {
        public ItemData? GetItem(int slot, bool isEquipped, ItemType itemType)
        {
            foreach (ItemData itemData in instance._heldItems)
            {
                if (itemData._slotNumber != slot ||
                    itemData._isEquipped != isEquipped)
                    continue;

                ScriptableItem scriptableItem = GameManager._current.Locate_Item(itemData._itemName);

                if (!scriptableItem)
                    continue;

                if (scriptableItem._itemType != itemType)
                    continue;

                return itemData;
            }

            return null;
        }
    }
}
