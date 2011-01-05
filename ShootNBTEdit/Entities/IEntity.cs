using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public abstract class IEntity
    {
        // This File
        public Tag Data;

        public Chunk CurChunk;

        // Entities' ID
        public Tag EntityID;

        // 3 TAG_Doubles describing the current X,Y,Z position of the entity.
        public Tag Position;
        public Tag X, Y, Z;

        // 3 TAG_Doubles describing the current dX,dY,dZ velocity of the entity. 
        //(Note: Falling into the Void  appears to set this to ridiculously high speeds. 
        // 0,0,0 is no motion.) 
        public Tag Motion;
        public Tag dX, dY, dZ;

        //Two TAG_Floats representing rotation in degrees. 
        public Tag Rotation;
        public Tag Yaw, Pitch;

        // Distance the entity has fallen. 
        // Larger values cause more damage when the entity lands. 
        public Tag FallDistance;

        // Number of ticks until the fire is put out. Negative 
        // values reflect how long the entity can stand in fire before burning. 
        public Tag Fire;

        // How much air the entity has, in ticks. Fills to a maximum of 200 in air, 
        // giving up to 20 seconds submerged. Decreases while underwater. 
        // If 0 while underwater, the entity loses 1 health per second. 
        public Tag Air;

        // 1 if the entity is touching the ground. 
        public Tag OnGround;

        public IEntity(Tag data, Chunk ch)
        {
            CurChunk = ch;
            Data = data;
            load();
        }

        private void load()
        {
            if (Data.Name != "Player")
                EntityID = Data["id"];
            Position = Data["Pos"];
                X = Position[0];
                Y = Position[1];
                Z = Position[2];
            Motion = Data["Motion"];
                dX = Motion[0];
                dY = Motion[1];
                dZ = Motion[2];
            Rotation = Data["Rotation"];
                Yaw = Rotation[0];
                Pitch = Rotation[1];

            FallDistance = Data["FallDistance"];
            Fire = Data["Fire"];
            Air = Data["Air"];
            OnGround = Data["OnGround"];
        }

        public abstract void Load();

        public override string ToString()
        {
            if (Data.Name != "Player")
                return EntityID.ToString();
            else
                return "Player";
        }
    }

    public class Entity : IEntity
    {
        public Entity(Tag data, Chunk curChun) : base(data, curChun) { }
        public override void Load() { }
    }
}
