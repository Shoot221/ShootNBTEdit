using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public class Minecart : IEntity
    {
        //The type of the cart: 0 - empty, 1 - with a chest, 2 - with a furnace. 
        public Tag Type;

        // 1
        public Tag InvItems;
        public List<InventoryItem> InventoryItems;

        // 2
        public Tag PushX, PushZ;
        public Tag Fuel;
        // Fueled, Regular, Storage
        public Minecart(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            InventoryItems = new List<InventoryItem>();
            Load();
        }

        public override void Load()
        {
            Type = Data["Type"];            
            if ((byte)Type.Value == 1)
            {
                InvItems = Data["InvItems"];
                foreach (Tag comp in InvItems)
                {
                    InventoryItems.Add(new InventoryItem(comp, false));
                }
            }
            if ((byte)Type == 2)
            {
                PushX = Data["PushX"];
                PushZ = Data["PushZ"];
                Fuel = Data["Fuel"];
            }
        }
    }
}
