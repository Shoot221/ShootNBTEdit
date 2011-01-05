using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public class InventoryItem
    {
        // This File
        public Tag Data;

        // Item ID
        public Tag ID;

        // The amount of wear each item has suffered. 0 means undamaged. 
        // When the Damage exceeds the item's durability, it breaks and disappears. 
        // Only tools and armor accumulate damage normally. 
        public Tag Damage;

        // Number of items stacked in this slot.
        // Range is 1-255 and 127+ are not displayed in game
        public Tag Count;

        // Which slot this item is in
        // Only when !itemEntity
        public Tag Slot;
        public bool itemEntity;

        public InventoryItem(Tag item, bool itemEntity)
        {
            Data = item;
            this.itemEntity = itemEntity;
            Load();
        }

        public void Load()
        {
            ID = Data["id"];
            Damage = Data["Damage"];

            Count = Data["Count"];
            if(!itemEntity)
                Slot = Data["Slot"];
        }
    }
}
