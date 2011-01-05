using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public class IgnitedTNT : IEntity
    {
        public Tag Fuse;
        public IgnitedTNT(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Fuse = Data["Fuse"];
        }
    }

    public class FallingSand : IEntity
    {
        public Tag Tile;
        public FallingSand(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Tile = Data["Tile"];
        }
    }
}
