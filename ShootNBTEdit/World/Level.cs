using NBT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public class Level
    {
        private Stopwatch sWatch;

        private Tag levelFile;
        public Tag LevelFile { get { return levelFile; } }

        private String levelFilePath;
        public String LevelFilePath { get { return levelFilePath; } }

        private Boolean modified = false;
        public Boolean Modified { get { return modified; } set { modified = value; } }

        private Boolean loaded = false;
        public Boolean Loaded { get { return loaded; } set { loaded = value; } }

        #region Tags hell :)

        // Level data (this file)
        public Tag Data;

        // Stores the current "time of day" in ticks. There are 20 ticks per real-life second
        // and 24000 ticks per Minecraft day, making the day length 20 minutes. 
        // 0 appears to be sunrise, 12000 sunset and 24000 sunrise again. 
        public Tag Time;

        // Stores the Unix time stamp (in milliseconds) when the player saved the game. 
        public Tag LastPlayed;

        public Player PPlayer;

        // X coordinate of the player's spawn position. Default is 0. 
        public Tag SpawnX;

        // Y coordinate of the player's spawn position. Default is 64. 
        public Tag SpawnY;

        // Z coordinate of the player's spawn position. Default is 0.
        public Tag SpawnZ;

        // Estimated size of the entire world in bytes. 
        public Tag SizeOnDisk;

        // Random number providing the Random Seed for the terrain. 
        public Tag RandomSeed;

        #endregion

        public Level(string filePath)
        {
            levelFilePath = filePath;
            levelFile = Tag.Load(filePath);
            sWatch = new Stopwatch();
            loadFile();
        }

        private void loadFile()
        {
            sWatch.Start();

            Data = levelFile["Data"];

            Time = Data["Time"];
            LastPlayed = Data["LastPlayed"];

            PPlayer = new Player(Data["Player"]);

            SpawnX = Data["SpawnX"];
            SpawnY = Data["SpawnY"];
            SpawnZ = Data["SpawnZ"];

            SizeOnDisk = Data["SizeOnDisk"];
            RandomSeed = Data["RandomSeed"];

            sWatch.Stop();
            Program.MainForm.lblStatus.Text = "Done loading in " + sWatch.ElapsedMilliseconds + "ms!";
            sWatch.Reset();

            loaded = true;
        }

        public void Save()
        {
            levelFile.Save(levelFilePath);
            modified = false;
        }
    }
}
