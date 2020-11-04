using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RPG.Inventories;
using RPG.Core.UI.Dragging;

namespace RPG.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;

        // STATE
        int index;
        InventoryItem item;
        Inventory inventory;

        // PUBLIC

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetAmountInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int amount)
        {
            inventory.AddItemToSlot(index, item, amount);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetAmount()
        {
            return inventory.GetAmountInSlot(index);
        }

        public void RemoveItems(int amount)
        {
            inventory.RemoveFromSlot(index, amount);
        }
    }
}