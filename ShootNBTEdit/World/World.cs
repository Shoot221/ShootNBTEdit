using NBT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ShootNBTEdit
{
    public class World
    {
        public Hashtable Chunks;
        private Stopwatch sWatch;

        public Level Lvl;
        public string Path;
        public string WorldName;

        public World(string folder, string NEM)
        {
            WorldName = NEM;
            Path = folder;
            Chunks = new Hashtable();
            sWatch = new Stopwatch();
        }

        public void DisposeAll()
        {
            foreach (Chunk chun in Chunks)
            {
                chun.Dispose();
                Chunks.Remove(chun);
            }
            Chunks.Clear();
            Chunks = null;
            Lvl = null;
        }

        public void SaveChunks()
        {
            sWatch.Start();
            foreach (Chunk chun in Chunks.Values)
            {
                if (chun.Modified)
                    chun.SaveChunk();
            }
            if (Lvl.Modified)
                Lvl.Save();
            sWatch.Stop();
            Program.MainForm.lblStatus.Text = "Done saving in " + sWatch.ElapsedMilliseconds + "ms!";
            sWatch.Reset();
        }

        public bool UnsavedChunks()
        {
            foreach (Chunk chun in Chunks.Values)
                if (chun.Modified)
                {
                    return true;
                }
            return false;
        }

        public override string ToString()
        {
            return WorldName;
        }
    }
}