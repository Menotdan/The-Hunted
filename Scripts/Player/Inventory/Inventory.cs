using Assets._Scripts.PlayerController.Inventory;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.PlayerController.Inventory
{
    public class Inventory
    {
        public int slots_count = 15;
        public Action<int> inventory_update;

        List<InventorySlot> slots;

        public Inventory()
        {
            slots = new List<InventorySlot>();

            // Initialize the inventory as blank.
            for (int i = 0; i < slots_count; i++)
            {
                slots.Add(new InventorySlot()); // Slot defaults to null for the ItemState
            }
        }

        public InventorySlot GetSlot(int slot)
        {
            return slots[slot];
        }

        public bool Add_New_Item(ItemState item_input, bool stack = true)
        {
            ItemState.ItemStateData item = new ItemState.ItemStateData();
            item.item_type = item_input.GetItemType();
            item.stack_amount = item_input.GetStackAmount(); // Copy item for magic.

            if (stack)
            {
                foreach (InventorySlot slot in slots)
                {
                    if (slot.Compare_Item(item))
                    {
                        slot.item.stack_amount += item.stack_amount;
                        if (inventory_update != null)
                            inventory_update(slots.IndexOf(slot));

                        return true;
                    }
                }
            }

            foreach (InventorySlot slot in slots)
            {
                if (!slot.SlotActive())
                {
                    slot.SetItem(item);
                    if (inventory_update != null)
                        inventory_update(slots.IndexOf(slot));

                    return true;
                }
            }

            return false;
        }

        public void PrintInventory()
        {
            int slot_num = 1;
            foreach (InventorySlot slot in slots)
            {
                if (!slot.SlotActive())
                {
                    Debug.Log($"[{slot_num}] Empty");
                } else
                {
                    Debug.Log($"[{slot_num}] '{slot.GetItemType().name}', {slot.item.stack_amount} in slot.");
                }

                slot_num++;
            }
        }
    }
}
