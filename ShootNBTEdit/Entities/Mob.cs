using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public abstract class IMob : IEntity
    {
        public Tag AttackTime, DeathTime, HurtTime;
        public Tag Health;

        // Mob, Monster, Creeper, Skeleton, Spider, Giant, Zombie, Slime, 
        // PigZombie, Ghast, Pig, Sheep, Cow, Chicken
        public IMob(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void  Load()
        {
            Health = Data["Health"];
 	        AttackTime = Data["AttackTime"];
            DeathTime = Data["DeathTime"];
            HurtTime = Data["HurtTime"];
        }   
    }

    public class Mob : IMob
    {
        public Mob(Tag data, Chunk curCh) : base(data, curCh) { }
        public override void Load() { }
    }

    public class Pig : IMob // WHY ISN'T THIS PIG IN A CART!!!!
    {
        public Tag Saddle;

        public Pig(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Saddle = Data["Saddle"];
            base.Load();
        }
    }

    public class Sheep : IMob
    {
        public Tag Sheared;

        public Sheep(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Sheared = Data["Sheared"]; // Naked sheep make me happy :)
            base.Load();
        }
    }

    public class Slime : IMob
    {
        public Tag Size;

        public Slime(Tag data, Chunk curCh)
            : base(data, curCh)
        {
            Load();
        }

        public override void Load()
        {
            Size = Data["Size"];
            base.Load();
        }
    }
}
