using NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public class Chunk
    {
        private Tag chunkFile;
        public Tag ChunkFile { get { return chunkFile; } }

        private String chunkFilePath;
        public String ChunkFilePath { get { return chunkFilePath; } }

        private Boolean modified = false;
        public Boolean Modified { get { return modified; } set { modified = value; } }

        private Boolean loaded = false;
        public Boolean Loaded { get { return loaded; } set { loaded = value; } }

        private Boolean hasPlayer = false;
        public Boolean HasPlayer { get { return hasPlayer; } set { hasPlayer = value; } }

        #region Tags hell :)
        // Chunk Data (this File)
        public Tag Level;

        // 32768 bytes of block IDs defining the terrain. 8 bits per block.
        public Tag Blocks;

        // 16384 bytes of block data additionally defining parts of the terrain. 
        // 4 bits per block. 
        public Tag Data;

        // 16384 bytes recording the amount of sun or moonlight hitting each block. 
        // 4 bits per block. Makes day/night transitions smoother compared to 
        // recomputing per level change. 
        public Tag SkyLight;

        // 16384 bytes recording the amount of block-emitted 
        // light in each block. 4 bits per block. Makes load times faster 
        // compared to recomputing at load time. 
        public Tag BlockLight;

        // 256 bytes of heightmap data. 16 x 16. 
        // Each byte records the lowest level in each column where the light from 
        // the sky is at full strength. Speeds computing of the SkyLight. 

        // Note: This array's indexes are ordered Z,X whereas the other array 
        // indexes are ordered X,Z,Y. 
        public Tag HeightMap;

        // Each TAG_Compound in this list defines an entity in the chunk
        public Tag EntityList;

        public List<Mob> Mobs;
        public List<Pig> Pigs;
        public List<Sheep> Sheeps;
        public List<Slime> Slimes;

        public List<Item> EItems;
        public List<Throwables> Throws;
        public List<Painting> Paintings;

        public List<Entity> Entities;
        public List<Minecart> Minecarts;

        public List<IgnitedTNT> IgnitedTNTs;
        public List<FallingSand> FallingSands;

        // Each TAG_Compound in this list defines a tile entity in the chunk.
        public Tag TileEntitiesList;
        public List<Furnace> Furnaces;
        public List<MobSpawner> MobSpawners;
        public List<Sign> Signs;
        public List<Chest> Chests;

        // Tick when the chunk was last saved. 
        public Tag LastUpdate;

        // X position of the chunk. Should match the file name. 
        public Tag xPos;

        // Z position of the chunk. Should match the file name. 
        public Tag zPos;

        // 1 or 0 (true/false) indicate whether the terrain in this chunk
        // was populated with special things. 
        // (Ores, special blocks, trees, dungeons, flowers, waterfalls, etc.) 
        public Tag TerrainPopulated;
        #endregion

        public Chunk(Tag dat, String pathToFile)
        {
            chunkFile = dat;
            chunkFilePath = pathToFile;
            Entities = new List<Entity>();
            Mobs = new List<Mob>();
            Pigs = new List<Pig>();
            Sheeps = new List<Sheep>();
            Slimes = new List<Slime>();

            EItems = new List<Item>();
            Throws = new List<Throwables>();
            Paintings = new List<Painting>();

            Minecarts = new List<Minecart>();

            IgnitedTNTs = new List<IgnitedTNT>();
            FallingSands = new List<FallingSand>();

            Furnaces = new List<Furnace>();
            MobSpawners = new List<MobSpawner>();
            Signs = new List<Sign>();
            Chests = new List<Chest>();
        }

        public void LoadChunk()
        {
            chunkFile = Tag.Load(chunkFilePath);

            Level = chunkFile["Level"];

            Blocks = Level["Blocks"];
            Data = Level["Data"];
            SkyLight = Level["SkyLight"];
            BlockLight = Level["BlockLight"];
            HeightMap = Level["HeightMap"];

            EntityList = Level["Entities"];
            #region Populating Lists hell
            Tag entiTemp;
            foreach (Tag enti in EntityList)
            {
                entiTemp = enti["id"];
                switch (entiTemp.ToString())
                {
                    case "Mob":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Monster":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Creeper":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Skeleton":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Spider":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Giant":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Zombie":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "PigZombie":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Ghast":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Cow":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Chicken":
                        Mobs.Add(new Mob(enti, this));
                        break;
                    case "Pig":
                        Pigs.Add(new Pig(enti, this));
                        break;
                    case "Sheep":
                        Sheeps.Add(new Sheep(enti, this));
                        break;
                    case "Slime":
                        Slimes.Add(new Slime(enti, this));
                        break;
                    case "Item":
                        EItems.Add(new Item(enti, this));
                        break;
                    case "Arrow":
                        Throws.Add(new Throwables(enti, this));
                        break;
                    case "Snowball":
                        Throws.Add(new Throwables(enti, this));
                        break;
                    case "Egg":
                        Throws.Add(new Throwables(enti, this));
                        break;
                    case "Painting":
                        Paintings.Add(new Painting(enti, this));
                        break;
                    case "Minecart":
                        Minecarts.Add(new Minecart(enti, this));
                        break;
                    case "PrimedTnt":
                        IgnitedTNTs.Add(new IgnitedTNT(enti, this));
                        break;
                    case "FallingSand":
                        FallingSands.Add(new FallingSand(enti, this));
                        break;
                    default:
                        Entities.Add(new Entity(enti, this));
                        break;
                }
            }
            #endregion 
            TileEntitiesList = Level["TileEntities"];
            #region Populating lists hell
            Tag tileEntiTemp;
            foreach (Tag enti in TileEntitiesList)
            {
                tileEntiTemp = enti["id"];
                switch ((string)tileEntiTemp.Value)
                {
                    case "Furnace":
                        Furnaces.Add(new Furnace(enti, this));
                        break;
                    case "Sign":
                        Signs.Add(new Sign(enti, this));
                        break;
                    case "MobSpawner":
                        MobSpawners.Add(new MobSpawner(enti, this));
                        break;
                    case "Chest":
                        Chests.Add(new Chest(enti, this));
                        break;
                }
            }
            #endregion

            LastUpdate = Level["LastUpdate"];

            xPos = Level["xPos"];
            zPos = Level["zPos"];

            TerrainPopulated = Level["TerrainPopulated"];

            loaded = true;
        }

        public void SaveChunk()
        {
            chunkFile.Save(chunkFilePath);
            modified = false;
        }

        public void Dispose()
        {
            Entities = null;
            Mobs = null;
            Pigs = null;
            Sheeps = null;
            Slimes = null;

            EItems = null;
            Throws = null;
            Paintings = null;

            Minecarts = null;

            IgnitedTNTs = null;
            FallingSands = null;

            Furnaces = null;
            MobSpawners = null;
            Signs = null;
            Chests = null;
            chunkFile = null;
        }
    }
}
