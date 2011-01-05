using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public abstract class IItem : IEntity
    {
        public Tag Health, Age;
        public InventoryItem Items;
        // Item, Arrow, Snowball, Egg, Painting
        public IItem(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Health = Data["Health"];
            Age = Data["Age"];
            Items = new InventoryItem(Data["Item"], false);
        }
    }

    public class Item : IItem
    {
        public Item(Tag data, Chunk curCh) : base(data, curCh) { }
        public override void Load() { }
    }

    public class Throwables : IItem 
    {
        public Tag xTile, yTile, zTile;
        public Tag inTile, Shake, inGround;

        public Throwables(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            inTile = Data["inTile"];
            Shake = Data["Shake"];
            inGround = Data["inGround"];

            xTile = Data["xTile"];
            yTile = Data["yTile"];
            zTile = Data["zTile"];
            base.Load();
        }
    }

    public class Painting : IItem
    {
        public Tag Direction;
        public Tag Name;
        public Tag TileX, TileY, TileZ;

        public Painting(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Direction = Data["Dir"];
            TileX = Data["xTile"];
            TileY = Data["yTile"];
            TileZ = Data["zTile"];
            Name = Data["Motive"];
            base.Load();
        }
    }
}
