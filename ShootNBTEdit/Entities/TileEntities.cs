using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public abstract class TileEntity
    {
        public Tag Data;

        public Tag EntityID;
        public Tag X, Y, Z;

        public Chunk CurChunk;

        public TileEntity(Tag data, Chunk curCh)
        {
            CurChunk = curCh;
            Data = data;
            load();
        }

        private void load()
        {
            X = Data["x"];
            Y = Data["y"];
            Z = Data["z"];

            EntityID = Data["id"];
        }
        public abstract void Load();
    }

    public class Furnace : TileEntity
    {
        public Tag BurnTime, CookTime;

        public Tag InvItems;
        public List<InventoryItem> InventoryItems;

        public Furnace(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            InventoryItems = new List<InventoryItem>();
            Load();
        }

        public override void Load()
        {
            BurnTime = Data["BurnTime"];
            CookTime = Data["CookTime"];
            InvItems = Data["Items"];
            foreach (Tag comp in InvItems)
            {
                InventoryItems.Add(new InventoryItem(comp, false));
            }
        }
    }

    public class Sign : TileEntity
    {
        public Tag Text1, Text2, Text3, Text4;

        public Sign(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Text1 = Data["Text1"];
            Text2 = Data["Text2"];
            Text3 = Data["Text3"];
            Text4 = Data["Text4"];
        }
    }

    public class MobSpawner : TileEntity
    {
        public Tag ToSpawnID;
        public Tag Delay;

        public MobSpawner(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            ToSpawnID = Data["EntityId"];
            Delay = Data["Delay"];
        }
    }

    public class Chest : TileEntity
    {
        public Tag InvItems;
        public List<InventoryItem> InventoryItems;

        public Chest(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            InventoryItems = new List<InventoryItem>();
            Load();
        }

        public override void Load()
        {
            InvItems = Data["Items"];
            foreach (Tag comp in InvItems)
            {
                InventoryItems.Add(new InventoryItem(comp, false));
            }
        }
    }
}
