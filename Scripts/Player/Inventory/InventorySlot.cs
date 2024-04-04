using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Scripts.PlayerController.Inventory
{
    public class InventorySlot
    {
        public ItemState.ItemStateData item;

        public InventorySlot()
        {
            item = null;
        }


        public Sprite GetIcon()
        {
            return item.item_type.inventory_icon;
        }

        public SO_Item GetItemType()
        {
            return item.item_type;
        }

        public bool SlotActive()
        {
            return item != null;
        }

        public void SetItem(ItemState.ItemStateData new_item)
        {
            item = new_item;
        }

        public bool Compare_Item(ItemState.ItemStateData comparison_item)
        {
            if (item == null)
                return false;

            if (comparison_item == null)
                return false;


            if (comparison_item.item_type.id == item.item_type.id)
            {
                return true;
            }

            return false;
        }

        public void DeleteItem()
        {
            item = null;
        }
    }
}
