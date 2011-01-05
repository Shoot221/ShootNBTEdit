using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public class Player : IMob
    {
        public Tag InvItems;
        public List<InventoryItem> InventoryItems;

        public Tag Score;

        public Tag Dimension;

        public Position PlayerPos;

        public ChunkPosition PlayerChunkPos;

        public Chunk PlayerChunk;

        public Player(Tag data)
            : base(data, null)
        {
            InventoryItems = new List<InventoryItem>();
            Load();
        }

        public override void Load()
        {
            InventoryItems = new List<InventoryItem>();
            InventoryItem temp;
            InvItems = Data["Inventory"];
            foreach (Tag comp in InvItems)
            {
                temp = new InventoryItem(comp, false);
                InventoryItems.Add(temp);
            }
            Score = Data["Score"];
            Dimension = Data["Dimension"];
            PlayerPos = new Position((double)X.Value, (double)Y.Value, (double)Z.Value);
            PlayerChunkPos = PlayerPos.ToChunkCoordinates();
        }
    }
}
