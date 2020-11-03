using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour, ISaveable
    {
        public struct InventorySlot
        {
            public InventoryItem item;
            public int amount;
        }

        // CONFIG DATA
        [Tooltip("Allowed size")]
        [SerializeField] int inventorySize = 16;

        // STATE
        public InventorySlot[] slots;

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action inventoryUpdated;
        public int number;

        

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }

        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        /// <summary>
        /// How many slots are in the inventory?
        /// </summary>
        public int GetSize()
        {
            return slots.Length;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(InventoryItem item, int amount)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }
            slots[i].item = item;
            slots[i].amount = amount;

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }

        public int GetAmountInSlot(int index)
        {
            return slots[index].amount;
        }

        /// <summary>
        /// Remove the item from the given slot.
        /// </summary>
        public void RemoveFromSlot(int slot, int amount)
        {
            slots[slot].amount -= amount;
            if(slots[slot].amount < 0)
            {
                slots[slot].amount = 0;
                slots[slot].item = null;
            }

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item, int amount)
        {
            if (slots[slot].item != null)
            {
                return AddToFirstEmptySlot(item, amount); ;
            }

            int indexOfStackSlot = FindStack(item);
            if(indexOfStackSlot >= 0)
            {
                slot = indexOfStackSlot;
            }
            slots[slot].item = item;
            slots[slot].amount += amount;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }

        // PRIVATE

        private void Awake()
        {
            slots = new InventorySlot[inventorySize];
        }

        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            int indexOfStackSlot = FindStack(item);
            if(indexOfStackSlot < 0)
            {
                // Não existe esse item no inventatio, ache um slor vazio
                indexOfStackSlot = FindEmptySlot();
            }
            return indexOfStackSlot;
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindStack(InventoryItem item)
        {
            // Se esse item nao for empilhavel retorne -1
            if (!item.IsStackable())
            {
                return -1;
            }

            // Procura se eu já tenho esse item para empilhar
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    return i;
                }
            }
            return -1;
        }

        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int amount;
        }

        object ISaveable.GetStates()
        {
            var inventorySlotRecord = new InventorySlotRecord[inventorySize];
            for (int i = 0; i < inventorySize; i++)
            {
                if (slots[i].item != null)
                {
                    inventorySlotRecord[i].itemID = slots[i].item.GetItemID();
                    inventorySlotRecord[i].amount = slots[i].amount;
                }
            }
            return inventorySlotRecord;
        }

        void ISaveable.RestoreState(object state)
        {
            var inventorySlotRecord = (InventorySlotRecord[])state;
            for (int i = 0; i < inventorySize; i++)
            {
                slots[i].item = InventoryItem.GetFromID(inventorySlotRecord[i].itemID);
                slots[i].amount = inventorySlotRecord[i].amount;
            }
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }
    }
}