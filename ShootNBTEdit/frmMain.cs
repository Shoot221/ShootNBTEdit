using NBT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace ShootNBTEdit
{
    public partial class frmMain : Form
    {
        private string savesFolder;
        private World world;

        private Chunk selectedChunk;
        private Stopwatch sWatch = new Stopwatch();

        FileSystemWatcher watcher = new FileSystemWatcher();

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Random rand = new Random();
            spltContainer.Panel2Collapsed = true;
            savesFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft\\saves");

            watcher.Filter = "session.lock";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);

            #region Populate drop down box
            string[] dirs = Directory.GetDirectories(savesFolder, "World?", SearchOption.TopDirectoryOnly);
            List<int> taken = new List<int>(9);
            foreach (string str in dirs)
            {
                string number = str.Remove(0, str.Length - 1);
                int imgNum = rand.Next(0, 8);
                while (taken.Contains(imgNum))
                    imgNum = rand.Next(0, 8);
                taken.Add(imgNum);
                btnOpen.DropDown.Items.Add("World" + number, bulletsList.Images[imgNum]);
                btnOpen.DropDown.Items[btnOpen.DropDown.Items.Count - 1].Tag = str;
            }
            #endregion
        }

        private void btnOpen_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            LoadWorld((string)e.ClickedItem.Tag, e.ClickedItem.Text);
        }

        private void LoadWorld(string path, string name)
        {
            if (world == null || !world.UnsavedChunks())
            {
                iLoadWorld(path, name);
            }
            else
            {
                DialogResult resu = MessageBox.Show("There were unsaved items, are you sure you want to change without saving?", "Unsaved Chunks!", MessageBoxButtons.YesNoCancel);
                if (resu == System.Windows.Forms.DialogResult.Yes)
                {
                    iLoadWorld(path, name);
                }
                else
                {
                    return;
                }
            }
        }

        private void iLoadWorld(string path, string name)
        {
            watcher.EnableRaisingEvents = false;
            btnSave.Enabled = true;
            reloadWorld.Enabled = true;

            byte[] mem = new byte[8];
            MemoryStream lStream = new MemoryStream(mem);
            BinaryWriter lWriter = new BinaryWriter(lStream);
            long unixTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            lWriter.Write(unixTime);
            Array.Reverse(mem, 0, mem.Length);
            File.WriteAllBytes(path + "//session.lock", mem);
            lStream.Close();
            lWriter.Close();

            watcher.Path = path;
            watcher.EnableRaisingEvents = true;

            world = new World(path, name);
            this.Text = "ShootNBTEdit - " + name;
            PopTreeview deDel = new PopTreeview(PopulateTreeview);
            deDel();
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            MessageBox.Show("The current save was loaded by another application.", "Warning:", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            world.DisposeAll();
            mainTreeView.Nodes.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            world.SaveChunks();
        }

        private delegate void dClearTView();
        private void ClearTView()
        {
            mainTreeView.Nodes.Clear();
            mainTreeView.Refresh();
        }

        private delegate void dAddTNode(TreeNode node, TreeNodeCollection parent);
        private void AddTNode(TreeNode node, TreeNodeCollection parent)
        {
            parent.Add(node);
        }

        private delegate void PopTreeview();
        private void PopulateTreeview()
        {
            sWatch.Start();
            dAddTNode dATN = new dAddTNode(AddTNode);
            dClearTView dCTV = new dClearTView(ClearTView);

            dCTV.Invoke();
            TreeNode fatherTemp, childTemp, grandKid;
            Chunk chunkTemp;
            Tag tagTemp;
            string name, x, z, chunkFilePath;
            string[] coords;
            world.Lvl = new Level(Path.Combine(world.Path, "level.dat"));
            foreach (string dir in Directory.GetDirectories(world.Path))
            {
                name = dir.Replace(world.Path + "\\", string.Empty);
                x = FromBase36(name).ToString();
                fatherTemp = new TreeNode(name + " - X: " + x);
                fatherTemp.ImageIndex = 19;
                fatherTemp.SelectedImageIndex = 20;
                fatherTemp.Name = "p." + x;
                dATN.Invoke(fatherTemp, mainTreeView.Nodes);
                Application.DoEvents();
                //mainTreeView.Nodes.Add(fatherTemp);

                foreach (string dir2 in Directory.GetDirectories(dir))
                {
                    name = dir2.Replace(dir + "\\", string.Empty);
                    z = FromBase36(name).ToString();
                    childTemp = new TreeNode(name + " - Z: " + z);
                    childTemp.ImageIndex = 19;
                    childTemp.SelectedImageIndex = 20;
                    childTemp.Name = "s." + z;
                    dATN.Invoke(childTemp, fatherTemp.Nodes);

                    foreach (string files in Directory.GetFiles(dir2))
                    {
                        tagTemp = NBT.Tag.Load(files);
                        chunkTemp = new Chunk(tagTemp, files);
                        world.Chunks.Add(files, chunkTemp);
                        name = files.Replace(dir2 + "\\", string.Empty);
                        chunkFilePath = files.Replace(world.Path + "\\", "");

                        if(chunkFilePath == world.Lvl.PPlayer.PlayerChunkPos.ChunkPath)
                            chunkTemp.HasPlayer = true;

                        coords = name.Split('.');

                        x = FromBase36(coords[1]).ToString();
                        z = FromBase36(coords[2]).ToString();

                        grandKid = new TreeNode(name + "  - X: " + x + ", Z: " + z);
                        grandKid.ImageIndex = 38;
                        grandKid.SelectedImageIndex = 38;
                        if (chunkFilePath.ToLower() == world.Lvl.PPlayer.PlayerChunkPos.ChunkPath.ToLower())
                        {
                            grandKid.ImageIndex = 23;
                            grandKid.SelectedImageIndex = 23;

                            childTemp.ImageIndex = 23;
                            childTemp.SelectedImageIndex = 23;

                            fatherTemp.ImageIndex = 23;
                            fatherTemp.SelectedImageIndex = 23;
                        }
                        grandKid.Name = Path.GetFileNameWithoutExtension(name);
                        grandKid.Tag = world.Chunks[files];
                        dATN.Invoke(grandKid, childTemp.Nodes);
                        //childTemp.Nodes.Add(grandKid);
                    }
                }
            }

            fatherTemp = new TreeNode();
            fatherTemp.Text = "Level Data";
            fatherTemp.Name = "ldata.";
            fatherTemp.Tag = world.Lvl;
            fatherTemp.ImageKey = "leveldat.png";
            fatherTemp.SelectedImageKey = "leveldat.png";
            mainTreeView.Nodes.Add(fatherTemp);

            sWatch.Stop();
            lblStatus.Text = "Done loading " + world.Chunks.Count + " chunks in " + sWatch.ElapsedMilliseconds + "ms!";
            sWatch.Reset();
        }

        private void PopulateChilds(Chunk ch, TreeNode father)
        {
            TreeNode temp, temp2, temp4, temp5, temp6;
            string temp1 = father.Name.ToString();
            string temp3 = father.Text;
            if (father.Nodes.Count == 0)
            {
                if (temp1.StartsWith("c."))
                {
                    #region Chunk info
                    temp = new TreeNode();
                    temp.Text = "Blocks";
                    temp.Tag = ch.Blocks;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Data";
                    temp.Tag = ch.Data;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "SkyLight";
                    temp.Tag = ch.SkyLight;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "BlockLight";
                    temp.Tag = ch.BlockLight;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "HeightMap";
                    temp.Tag = ch.HeightMap;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Entities";
                    temp.Name = "e.";
                    temp.Tag = ch;
                    father.Nodes.Add(temp);
                    temp2 = temp;
                    #region Entities
                    foreach (Entity enti in ch.Entities)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageKey = temp.Text + "Face";
                        temp.Tag = ch.Entities[ch.Entities.IndexOf(enti)];
                        temp2.Nodes.Add(temp);
                    }

                    foreach (Mob enti in ch.Mobs)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageKey = temp.Text + "Face";
                        temp.Tag = ch.Mobs[ch.Mobs.IndexOf(enti)];
                        temp2.Nodes.Add(temp);
                    }

                    foreach (Pig enti in ch.Pigs)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 27;
                        temp.SelectedImageIndex = 27;
                        temp.Tag = ch.Pigs[ch.Pigs.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }
                    foreach (Slime enti in ch.Slimes)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 32;
                        temp.SelectedImageIndex = 32;
                        temp.Tag = ch.Slimes[ch.Slimes.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }
                    foreach (Item enti in ch.EItems)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 39;
                        temp.SelectedImageIndex = 39;
                        temp.Tag = ch.EItems[ch.EItems.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }
                    foreach (Throwables enti in ch.Throws)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 33;
                        temp.SelectedImageIndex = 33;
                        temp.Tag = ch.Throws[ch.Throws.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }
                    foreach (Painting enti in ch.Paintings)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 26;
                        temp.SelectedImageIndex = 26;
                        temp.Tag = ch.Paintings[ch.Paintings.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }
                    foreach (Minecart enti in ch.Minecarts)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 24;
                        temp.SelectedImageIndex = 24;
                        temp.Tag = ch.Minecarts[ch.Minecarts.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }
                    foreach (IgnitedTNT enti in ch.IgnitedTNTs)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 28;
                        temp.SelectedImageIndex = 28;
                        temp.Tag = ch.IgnitedTNTs[ch.IgnitedTNTs.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }
                    foreach (FallingSand enti in ch.FallingSands)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 18;
                        temp.SelectedImageIndex = 18;
                        temp.Tag = ch.FallingSands[ch.FallingSands.IndexOf(enti)];
                        temp2.Nodes.Add(temp);

                    }

                    foreach (Sheep enti in ch.Sheeps)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.Tag = ch.Sheeps.IndexOf(enti);
                        temp.ImageKey = "SheepFace";
                        temp2.Nodes.Add(temp);

                    }
                    #endregion

                    temp = new TreeNode();
                    temp.Text = "Tile Entities";
                    temp.Name = "e.t.";
                    temp.Tag = ch;
                    father.Nodes.Add(temp);
                    temp2 = temp;
                    #region Tile Entities
                    foreach (MobSpawner enti in ch.MobSpawners)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 25;
                        temp.SelectedImageIndex = 25;
                        temp.Tag = ch.MobSpawners[ch.MobSpawners.IndexOf(enti)];
                        temp2.Nodes.Add(temp);
                    }
                    foreach (Sign enti in ch.Signs)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 30;
                        temp.SelectedImageIndex = 30;
                        temp.Tag = ch.Signs[ch.Signs.IndexOf(enti)];
                        temp2.Nodes.Add(temp);
                    }
                    foreach (Chest enti in ch.Chests)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 12;
                        temp.SelectedImageIndex = 12;
                        temp.Tag = ch.Chests[ch.Chests.IndexOf(enti)];
                        temp2.Nodes.Add(temp);
                    }
                    // Furnaces
                    foreach (Furnace enti in ch.Furnaces)
                    {
                        temp = new TreeNode();
                        temp.Text = enti.EntityID.ToString();
                        temp.ImageIndex = 21;
                        temp.SelectedImageIndex = 21;
                        temp.Tag = ch.Furnaces[ch.Furnaces.IndexOf(enti)];
                        temp2.Nodes.Add(temp);
                    }
                    #endregion

                    temp = new TreeNode();
                    temp.Text = "LastUpdate";
                    temp.Tag = ch.LastUpdate;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "xPos";
                    temp.Tag = ch.xPos;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "zPos";
                    temp.Tag = ch.zPos;
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "TerrainPopulated";
                    temp.Tag = ch.TerrainPopulated;
                    father.Nodes.Add(temp);
                    #endregion
                }
                else if (temp1.Equals("ldata."))
                {
                    #region Level.dat
                    temp = new TreeNode();
                    temp.Text = "Time";
                    temp.Tag = world.Lvl.Time;
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "LastPlayed";
                    temp.Tag = world.Lvl.LastPlayed;
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Player";
                    temp.Tag = world.Lvl.PPlayer;
                    temp.ImageKey = "HumanFace.png";
                    temp.SelectedImageKey = "HumanFace.png";
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);
                    temp6 = temp;

                    temp2 = new TreeNode();
                    temp2.Text = "Inventory";
                    temp2.Tag = world.Lvl.PPlayer.InventoryItems;
                    temp.Name = "levdata.";
                    temp.Nodes.Add(temp2);

                    foreach (InventoryItem inv in (List<InventoryItem>)temp2.Tag)
                    {

                        temp4 = new TreeNode();
                        temp4.Text = inv.ID.ToString();
                        temp4.Tag = inv;
                        temp2.Nodes.Add(temp4);
                        temp.Name = "levdata.";
                        temp5 = temp4;

                        temp = new TreeNode();
                        temp.Text = "ID";
                        temp.Tag = inv.ID;
                        temp.Name = "levdata.";
                        temp5.Nodes.Add(temp);

                        temp = new TreeNode();
                        temp.Text = "Damage";
                        temp.Tag = inv.Damage;
                        temp.Name = "levdata.";
                        temp5.Nodes.Add(temp);

                        temp = new TreeNode();
                        temp.Text = "Count";
                        temp.Tag = inv.Count;
                        temp.Name = "levdata.";
                        temp5.Nodes.Add(temp);

                        temp = new TreeNode();
                        temp.Text = "Slot";
                        temp.Tag = inv.Slot;
                        temp.Name = "levdata.";
                        temp5.Nodes.Add(temp);
                    }

                    temp2 = new TreeNode();
                    temp2.Text = "Score";
                    temp2.Tag = world.Lvl.PPlayer.Score;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp2);

                    temp2 = new TreeNode();
                    temp2.Text = "Dimension";
                    temp2.Tag = world.Lvl.PPlayer.Dimension;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp2);

                    temp = new TreeNode();
                    temp.Text = "Pos";
                    temp.Tag = world.Lvl.PPlayer.Position;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp);
                    temp2 = temp;

                    temp = new TreeNode();
                    temp.Text = "x";
                    temp.Tag = world.Lvl.PPlayer.X;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);
                    temp = new TreeNode();
                    temp.Text = "y";
                    temp.Tag = world.Lvl.PPlayer.Y;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);
                    temp = new TreeNode();
                    temp.Text = "z";
                    temp.Tag = world.Lvl.PPlayer.Z;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Motion";
                    temp.Tag = world.Lvl.PPlayer.Position;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp);
                    temp2 = temp;

                    temp = new TreeNode();
                    temp.Text = "dX";
                    temp.Tag = world.Lvl.PPlayer.dX;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);
                    temp = new TreeNode();
                    temp.Text = "dY";
                    temp.Tag = world.Lvl.PPlayer.dY;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);
                    temp = new TreeNode();
                    temp.Text = "dZ";
                    temp.Tag = world.Lvl.PPlayer.dZ;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Rotation";
                    temp.Tag = world.Lvl.PPlayer.Rotation;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp);
                    temp2 = temp;

                    temp = new TreeNode();
                    temp.Text = "Yaw";
                    temp.Tag = world.Lvl.PPlayer.Yaw;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);
                    temp = new TreeNode();
                    temp.Text = "Pitch";
                    temp.Tag = world.Lvl.PPlayer.Pitch;
                    temp.Name = "levdata.";
                    temp2.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "FallDistance";
                    temp.Tag = world.Lvl.PPlayer.FallDistance;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Fire";
                    temp.Tag = world.Lvl.PPlayer.Fire;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Air";
                    temp.Tag = world.Lvl.PPlayer.Air;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "OnGround";
                    temp.Tag = world.Lvl.PPlayer.OnGround;
                    temp.Name = "levdata.";
                    temp6.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "SpawnX";
                    temp.Tag = world.Lvl.SpawnX;
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "SpawnY";
                    temp.Tag = world.Lvl.SpawnY;
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "SpawnZ";
                    temp.Tag = world.Lvl.SpawnZ;
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "SizeOnDisk";
                    temp.Tag = world.Lvl.SizeOnDisk;
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "RandomSeed";
                    temp.Tag = world.Lvl.RandomSeed;
                    temp.Name = "levdata.";
                    father.Nodes.Add(temp);
                    #endregion
                }
                else
                {
                    switch (temp3)
                    {
                        #region Entities
                        #region Mobs
                        case "Mob":
                            AddEntityMob(father, ch);
                            break;
                        case "Monster":
                            AddEntityMob(father, ch);
                            break;
                        case "Creeper":
                            AddEntityMob(father, ch);
                            break;
                        case "Skeleton":
                            AddEntityMob(father, ch);
                            break;
                        case "Spider":
                            AddEntityMob(father, ch);
                            break;
                        case "Giant":
                            AddEntityMob(father, ch);
                            break;
                        case "Zombie":
                            AddEntityMob(father, ch);
                            break;
                        case "PigZombie":
                            AddEntityMob(father, ch);
                            break;
                        case "Ghast":
                            AddEntityMob(father, ch);
                            break;
                        case "Cow":
                            AddEntityMob(father, ch);
                            break;
                        case "Chicken":
                            AddEntityMob(father, ch);
                            break;
                        case "Pig":
                            AddEntityPig(father, ch);
                            break;
                        case "Sheep":
                            AddEntitySheep(father, ch);
                            break;
                        case "Slime":
                            AddEntitySlime(father, ch);
                            break;
                        #endregion
                        #region Items
                        case "Item":
                            AddEntityItem(father, ch);
                            break;
                        case "Arrow":
                            AddEntityThrowable(father, ch);
                            break;
                        case "Snowball":
                            AddEntityThrowable(father, ch);
                            break;
                        case "Egg":
                            AddEntityThrowable(father, ch);
                            break;
                        case "Painting":
                            AddEntityPainting(father, ch);
                            break;
                        #endregion
                        #region Vehicles
                        case "Minecart":
                            AddEntityMinecart(father, ch);
                            break;
                        #endregion
                        #region Dynamic tiles
                        case "PrimedTnt":
                            AddEntityIgnitedTNT(father, ch);
                            break;
                        case "FallingSand":
                            AddEntityFallingSand(father, ch);
                            break;
                        #endregion
                        #endregion
                        #region Tile Entities
                        case "Furnace":
                            AddTileEntityFurnace(ch, father);
                            break;
                        case "Sign":
                            AddTileEntitySign(ch, father);
                            break;
                        case "MobSpawner":
                            AddTileEntityMobSpawner(ch, father);
                            break;
                        case "Chest":
                            AddTileEntityChest(ch, father);
                            break;
                        #endregion
                        default:
                            AddEntity(father, ch);
                            break;
                    }
                }
            }
            mainTreeView.Refresh();
        }
        private TreeNode GetNode(string name)
        {
            foreach (TreeNode node in mainTreeView.Nodes)
            {
                if (node.Name == name)
                    return node;
            }
            return null;
        }
        private void mainTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string temp1 = e.Node.Name;
            string temp2;
            Chunk ch;
            TagType curTag;
            spltContainer.Panel2Collapsed = true;
            if (temp1.StartsWith("c."))
            {
                ch = (Chunk)e.Node.Tag;
                if (!ch.Loaded)
                {
                    ch.LoadChunk();
                }
                PopulateChilds(ch, e.Node);
                lblSelectedType.Text = "Chunk - " + e.Node.Text;
                selectedChunk = ch;
                return;
            }
            else if (temp1.StartsWith("p.") || temp1.StartsWith("s."))
            {
                if (temp1.StartsWith("p."))
                {
                    lblSelectedType.Text = "Primary folder";
                }
                else if (temp1.StartsWith("s."))
                {
                    lblSelectedType.Text = "Secondary folder";
                }
                PopulateChilds(null, e.Node);
                return;
            }
            else if (temp1.StartsWith("ldata."))
            {
                PopulateChilds(null, e.Node);
                return;
            }
            else if (!e.Node.Parent.Name.StartsWith("e."))
            {
                temp1 = e.Node.Tag.GetType().ToString();
                temp2 = ((Tag)e.Node.Tag).Type.ToString();
                if (temp1.Contains("ShootNBTEdit") || temp2.Contains("Byte_Array") || temp2.Contains("List") || temp2.Contains("Compound") || temp1.Contains("Player") && e.Node.Parent != null)
                    if (!e.Node.Parent.Name.StartsWith("e."))
                        return;
                spltContainer.Panel2Collapsed = false;
                curTag = ((Tag)e.Node.Tag).Type;
                lblSelectedType.Text = "Selected type: " + curTag.ToString();
                switch (curTag)
                {
                    case TagType.Byte:
                        txtBoxVal.Visible = true;
                        btnSaveVal.Visible = true;
                        txtBoxVal.Tag = e.Node.Tag;
		                txtBoxVal.Text = ((Tag)e.Node.Tag).Value.ToString();
                        break;
                    case TagType.Double:

                        txtBoxVal.Visible = true;
                        btnSaveVal.Visible = true;
                        txtBoxVal.Tag = e.Node.Tag;
		                txtBoxVal.Text = ((Tag)e.Node.Tag).Value.ToString();
                        break;
                    case TagType.Float:

                        txtBoxVal.Visible = true;
                        btnSaveVal.Visible = true;
                        txtBoxVal.Tag = e.Node.Tag;
                        txtBoxVal.Text = ((Tag)e.Node.Tag).Value.ToString();
                        break;
                    case TagType.Int:
                        txtBoxVal.Visible = true;
                        btnSaveVal.Visible = true;
                        txtBoxVal.Tag = e.Node.Tag;
                        txtBoxVal.Text = ((Tag)e.Node.Tag).Value.ToString();
                        break;
                    case TagType.Long:
                        txtBoxVal.Visible = true;
                        btnSaveVal.Visible = true;
                        txtBoxVal.Tag = e.Node.Tag;
                        txtBoxVal.Text = ((Tag)e.Node.Tag).Value.ToString();
                        break;
                    case TagType.Short:
                        txtBoxVal.Visible = true;
                        btnSaveVal.Visible = true;
                        txtBoxVal.Tag = e.Node.Tag;
                        txtBoxVal.Text = ((Tag)e.Node.Tag).Value.ToString();
                        break;
                    case TagType.String:
                        txtBoxVal.Visible = true;
                        btnSaveVal.Visible = true;
                        txtBoxVal.Tag = e.Node.Tag;
                        txtBoxVal.Text = ((Tag)e.Node.Tag).Value.ToString();
                        break;
                    default:
                        break;
                }
            }
            if (!temp1.Equals("p.") && !temp1.Equals("s.") && !temp1.StartsWith("c.") && e.Node.Parent != null)
            {
                if (e.Node.Parent.Name.StartsWith("e."))
                {
                    if (e.Node.Parent.Name == "e.")
                        ch = ((IEntity)e.Node.Tag).CurChunk;
                    else if (e.Node.Parent.Name == "e.t.")
                        ch = ((TileEntity)e.Node.Tag).CurChunk;
                    else
                        ch = ((IEntity)e.Node.Tag).CurChunk;

                    if (!ch.Loaded)
                    {
                        ch.LoadChunk();
                    }
                    PopulateChilds(ch, e.Node);
                }
            }
        }

        #region Add entities/tile entities
        #region Add entity different
        private void AddEntityPig(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, true, false, false, false, false, false, false); }
        private void AddEntitySheep(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, true, false, false, false, false, false, false, false); }
        private void AddEntitySlime(TreeNode father, Chunk ch) { AddEntity(father, ch, false, true, false, false, false, false, false, false, false, false); }
        private void AddEntityThrowable(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, false, true, false, false, false, false, false); }
        private void AddEntityItem(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, false, false, true, false, false, false, false); }
        private void AddEntityPainting(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, false, false, false, true, false, false, false); }
        private void AddEntityMinecart(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, false, false, false, false, true, false, false); }
        private void AddEntityIgnitedTNT(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, false, false, false, false, false, true, false); }
        private void AddEntityFallingSand(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, false, false, false, false, false, false, true); }
        private void AddEntity(TreeNode father, Chunk ch) { AddEntity(father, ch, false, false, false, false, false, false, false, false, false, false); }
        private void AddEntityMob(TreeNode father, Chunk ch) { AddEntity(father, ch, true, false, false, false, false, false, false, false, false, false); }
        #endregion
        private void AddEntity(TreeNode father, Chunk ch, bool mob, bool slime, bool sheep, bool pig, bool throwable, bool item,
            bool painting, bool minecart, bool ignitedTnt, bool fallingSand)
        {
            TreeNode temp, temp2, temp11;
            Tag temp3;
            Tag temp4, temp5;
            Tag temp7;

            if (!mob && !slime && !sheep && !pig && !throwable && !item && !painting && !minecart && !ignitedTnt && !fallingSand)
            {
                #region Regular boring Entities
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((Entity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((Entity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((Entity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((Entity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((Entity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((Entity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((Entity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((Entity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((Entity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((Entity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((Entity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((Entity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((Entity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((Entity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((Entity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((Entity)father.Tag).OnGround;
                father.Nodes.Add(temp);
                #endregion
            }

            if (mob)
            {
                #region Mobby mob
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "AttackTime";
                temp.Tag = ((IMob)father.Tag).AttackTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "DeathTime";
                temp.Tag = ((IMob)father.Tag).DeathTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "HurtTime";
                temp.Tag = ((IMob)father.Tag).HurtTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Health";
                temp.Tag = ((IMob)father.Tag).Health;
                father.Nodes.Add(temp);
                #endregion
            }
            else if (pig)
            {
                #region Stupid pig.
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "AttackTime";
                temp.Tag = ((IMob)father.Tag).AttackTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "DeathTime";
                temp.Tag = ((IMob)father.Tag).DeathTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "HurtTime";
                temp.Tag = ((IMob)father.Tag).HurtTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Health";
                temp.Tag = ((IMob)father.Tag).Health;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Saddle";
                temp.Tag = ((Pig)father.Tag).Saddle;
                father.Nodes.Add(temp);
                #endregion
            }
            else if (sheep)
            {
                #region Naked sheep ftw.
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "AttackTime";
                temp.Tag = ((IMob)father.Tag).AttackTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "DeathTime";
                temp.Tag = ((IMob)father.Tag).DeathTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "HurtTime";
                temp.Tag = ((IMob)father.Tag).HurtTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Health";
                temp.Tag = ((IMob)father.Tag).Health;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Sheared";
                temp.Tag = ((Sheep)father.Tag).Sheared;
                father.Nodes.Add(temp);
                #endregion
            }
            else if (slime)
            {
                #region Bouncy bouncy bouncy
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "AttackTime";
                temp.Tag = ((IMob)father.Tag).AttackTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "DeathTime";
                temp.Tag = ((IMob)father.Tag).DeathTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "HurtTime";
                temp.Tag = ((IMob)father.Tag).HurtTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Health";
                temp.Tag = ((IMob)father.Tag).Health;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Size";
                temp.Tag = ((Slime)father.Tag).Size;
                father.Nodes.Add(temp);
                #endregion
            }
            else if (item)
            {
                #region Basic Item
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Health";
                temp.Tag = ((Item)father.Tag).Health;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Age";
                temp.Tag = ((Item)father.Tag).Age;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Item";
                temp.Tag = ((Item)father.Tag).Items.Data;
                father.Nodes.Add(temp);
                temp2 = temp;
                temp3 = (Tag)temp2.Tag;

                temp4 = (Tag)temp3["id"];
                temp5 = (Tag)temp3["Damage"];
                temp7 = (Tag)temp3["Count"];
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = temp4;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Damage";
                temp.Tag = temp5;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Count";
                temp.Tag = temp7;
                temp2.Nodes.Add(temp);
                #endregion
            }
            else if (throwable)
            {
                #region Throwablebles
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Health";
                temp.Tag = ((Item)father.Tag).Health;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Age";
                temp.Tag = ((Item)father.Tag).Age;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Item";
                temp.Tag = ((Item)father.Tag).Items.Data;
                father.Nodes.Add(temp);
                temp2 = temp;
                temp3 = (Tag)temp2.Tag;

                temp4 = (Tag)temp3["id"];
                temp5 = (Tag)temp3["Damage"];
                temp7 = (Tag)temp3["Count"];
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = temp4;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Damage";
                temp.Tag = temp5;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Count";
                temp.Tag = temp7;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "xTile";
                temp.Tag = ((Throwables)father.Tag).xTile;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "yTile";
                temp.Tag = ((Throwables)father.Tag).yTile;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "zTile";
                temp.Tag = ((Throwables)father.Tag).zTile;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "inTile";
                temp.Tag = ((Throwables)father.Tag).inTile;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Shake";
                temp.Tag = ((Throwables)father.Tag).Shake;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "inGround";
                temp.Tag = ((Throwables)father.Tag).inGround;
                father.Nodes.Add(temp);
                #endregion
            }
            else if (painting)
            {
                #region Paintingingins
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Health";
                temp.Tag = ((Item)father.Tag).Health;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Age";
                temp.Tag = ((Item)father.Tag).Age;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Item";
                temp.Tag = ((Item)father.Tag).Items.Data;
                father.Nodes.Add(temp);
                temp2 = temp;
                temp3 = (Tag)temp2.Tag;

                temp4 = (Tag)temp3["id"];
                temp5 = (Tag)temp3["Damage"];
                temp7 = (Tag)temp3["Count"];
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = temp4;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Damage";
                temp.Tag = temp5;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Count";
                temp.Tag = temp7;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "TileX";
                temp.Tag = ((Painting)father.Tag).TileX;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "TileY";
                temp.Tag = ((Painting)father.Tag).TileY;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "TileZ";
                temp.Tag = ((Painting)father.Tag).TileZ;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motive(Name)";
                temp.Tag = ((Painting)father.Tag).Name;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Dir";
                temp.Tag = ((Painting)father.Tag).Direction;
                father.Nodes.Add(temp);
                #endregion
            }

            else if (minecart)
            {
                #region Minecarts w/o Pigs... FFUUUUU
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Type";
                temp.Tag = ((Minecart)father.Tag).Type;
                father.Nodes.Add(temp);

                switch ((byte)((Tag)temp.Tag).Value)
                {
                    case 1:
                        temp = new TreeNode();
                        temp.Text = "PushX";
                        temp.Tag = ((Minecart)father.Tag).PushX;
                        father.Nodes.Add(temp);

                        temp = new TreeNode();
                        temp.Text = "PushZ";
                        temp.Tag = ((Minecart)father.Tag).PushZ;
                        father.Nodes.Add(temp);

                        temp = new TreeNode();
                        temp.Text = "Fuel";
                        temp.Tag = ((Minecart)father.Tag).Fuel;
                        father.Nodes.Add(temp);
                        break;
                    case 2:
                        temp = new TreeNode();
                        temp.Text = "Items";
                        temp.Tag = ((Minecart)father.Tag).InventoryItems;
                        father.Nodes.Add(temp);
                        temp2 = temp;

                        foreach (InventoryItem inv in (List<InventoryItem>)temp2.Tag)
                        {
                            temp = new TreeNode();
                            temp.Text = inv.ID.ToString();
                            temp.Tag = inv.ID;
                            temp2.Nodes.Add(temp);
                            temp11 = temp;

                            temp = new TreeNode();
                            temp.Text = "ID";
                            temp.Tag = inv.ID;
                            temp11.Nodes.Add(temp);

                            temp = new TreeNode();
                            temp.Text = "Damage";
                            temp.Tag = inv.Damage;
                            temp11.Nodes.Add(temp);

                            temp = new TreeNode();
                            temp.Text = "Count";
                            temp.Tag = inv.Count;
                            temp11.Nodes.Add(temp);

                            temp = new TreeNode();
                            temp.Text = "Slot";
                            temp.Tag = inv.Slot;
                            temp11.Nodes.Add(temp);
                        }
                        break;
                }

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((Minecart)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((Minecart)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((Minecart)father.Tag).OnGround;
                father.Nodes.Add(temp);


                #endregion
            }
            else if (ignitedTnt)
            {
                #region OHHSHIIITT TNT!
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fuse";
                temp.Tag = ((IgnitedTNT)father.Tag).Fuse;
                father.Nodes.Add(temp);
                #endregion
            }
            else if (fallingSand)
            {
                #region It's dark in here.
                temp = new TreeNode();
                temp.Text = "ID";
                temp.Tag = ((IEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Pos";
                temp.Tag = ((IEntity)father.Tag).Position;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((IEntity)father.Tag).X;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((IEntity)father.Tag).Y;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((IEntity)father.Tag).Z;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Motion";
                temp.Tag = ((IEntity)father.Tag).Motion;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "dX";
                temp.Tag = ((IEntity)father.Tag).dX;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dY";
                temp.Tag = ((IEntity)father.Tag).dY;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "dZ";
                temp.Tag = ((IEntity)father.Tag).dZ;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Rotation";
                temp.Tag = ((IEntity)father.Tag).Rotation;
                father.Nodes.Add(temp);
                temp2 = temp;

                temp = new TreeNode();
                temp.Text = "Yaw";
                temp.Tag = ((IEntity)father.Tag).Yaw;
                temp2.Nodes.Add(temp);
                temp = new TreeNode();
                temp.Text = "Pitch";
                temp.Tag = ((IEntity)father.Tag).Pitch;
                temp2.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "FallDistance";
                temp.Tag = ((IEntity)father.Tag).FallDistance;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Fire";
                temp.Tag = ((IEntity)father.Tag).Fire;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Air";
                temp.Tag = ((IEntity)father.Tag).Air;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "OnGround";
                temp.Tag = ((IEntity)father.Tag).OnGround;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Tile";
                temp.Tag = ((FallingSand)father.Tag).Tile;
                father.Nodes.Add(temp);
                #endregion
            }
        }

        private void AddTileEntityFurnace(Chunk ch, TreeNode parent) { AddTileEntity(ch, parent, true, false, false, false); }
        private void AddTileEntitySign(Chunk ch, TreeNode parent) { AddTileEntity(ch, parent, false, true, false, false); }
        private void AddTileEntityMobSpawner(Chunk ch, TreeNode parent) { AddTileEntity(ch, parent, false, false, true, false); }
        private void AddTileEntityChest(Chunk ch, TreeNode parent) { AddTileEntity(ch, parent, false, false, false, true); }
        private void AddTileEntity(Chunk ch, TreeNode father, bool furnace, bool sign, bool mobspawner, bool chest)
        {
            TreeNode temp, temp2, temp11;
            if (furnace)
            {
                #region Pizza Maker
                temp = new TreeNode();
                temp.Text = "id";
                temp.Tag = ((TileEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((TileEntity)father.Tag).X;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((TileEntity)father.Tag).Y;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((TileEntity)father.Tag).Z;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "BurnTime";
                temp.Tag = ((Furnace)father.Tag).BurnTime;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "CookTime";
                temp.Tag = ((Furnace)father.Tag).CookTime;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Items";
                temp.Tag = ((Furnace)father.Tag).InventoryItems;
                father.Nodes.Add(temp);
                temp2 = temp;

                foreach (InventoryItem inv in (List<InventoryItem>)temp2.Tag)
                {
                    temp = new TreeNode();
                    temp.Text = inv.ID.ToString();
                    temp.Tag = inv.ID;
                    temp2.Nodes.Add(temp);
                    temp11 = temp;

                    temp = new TreeNode();
                    temp.Text = "ID";
                    temp.Tag = inv.ID;
                    temp11.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Damage";
                    temp.Tag = inv.Damage;
                    temp11.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Count";
                    temp.Tag = inv.Count;
                    temp11.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Slot";
                    temp.Tag = inv.Slot;
                    temp11.Nodes.Add(temp);
                }

                #endregion
            }

            if (sign)
            {
                #region Sign
                temp = new TreeNode();
                temp.Text = "id";
                temp.Tag = ((TileEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((TileEntity)father.Tag).X;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((TileEntity)father.Tag).Y;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((TileEntity)father.Tag).Z;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "Text1";
                temp.Tag = ((Sign)father.Tag).Text1;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "Text2";
                temp.Tag = ((Sign)father.Tag).Text2;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "Text3";
                temp.Tag = ((Sign)father.Tag).Text3;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Text4";
                temp.Tag = ((Sign)father.Tag).Text4;
                father.Nodes.Add(temp);
                #endregion
            }

            if (mobspawner)
            {
                #region Dungeon!
                temp = new TreeNode();
                temp.Text = "id";
                temp.Tag = ((TileEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((TileEntity)father.Tag).X;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((TileEntity)father.Tag).Y;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((TileEntity)father.Tag).Z;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "EntityID";
                temp.Tag = ((MobSpawner)father.Tag).ToSpawnID;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "Delay";
                temp.Tag = ((MobSpawner)father.Tag).Delay;
                father.Nodes.Add(temp);
                #endregion
            }

            if (chest)
            {
                #region Boobs.

                temp = new TreeNode();
                temp.Text = "id";
                temp.Tag = ((TileEntity)father.Tag).EntityID;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "x";
                temp.Tag = ((TileEntity)father.Tag).X;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "y";
                temp.Tag = ((TileEntity)father.Tag).Y;
                father.Nodes.Add(temp);


                temp = new TreeNode();
                temp.Text = "z";
                temp.Tag = ((TileEntity)father.Tag).Z;
                father.Nodes.Add(temp);

                temp = new TreeNode();
                temp.Text = "Items";
                temp.Tag = ((Chest)father.Tag).InventoryItems;
                father.Nodes.Add(temp);
                temp2 = temp;

                foreach (InventoryItem inv in (List<InventoryItem>)temp2.Tag)
                {

                    temp = new TreeNode();
                    temp.Text = inv.ID.ToString();
                    temp.Tag = inv.ID;
                    temp2.Nodes.Add(temp);
                    temp11 = temp;

                    temp = new TreeNode();
                    temp.Text = "ID";
                    temp.Tag = inv.ID;
                    temp11.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Damage";
                    temp.Tag = inv.Damage;
                    temp11.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Count";
                    temp.Tag = inv.Count;
                    temp11.Nodes.Add(temp);

                    temp = new TreeNode();
                    temp.Text = "Slot";
                    temp.Tag = inv.Slot;
                    temp11.Nodes.Add(temp);
                }
                #endregion
            }
        }
        #endregion

        #region Base36 tools
        private static long FromBase36(string base36String)
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyz";

            bool negative = base36String.StartsWith("-");

            string inputChars = base36String.ToLowerInvariant();

            long output = 0;
            int position = 0;
            for (int i = inputChars.Length - 1; i >= (negative ? 1 : 0); i--)
            {
                char c = inputChars[i];
                output += chars.IndexOf(c) * (long)Math.Pow(36, position);
                position++;
            }

            return (negative ? output * -1 : output);
        }

        private static string ToBase36(long i)
        {
            if (0 <= i && i < 10)
                return i.ToString();

            List<char> result = new List<char>();

            var remainder = Math.Abs(i);

            while (remainder != 0)
            {
                var digit = remainder % 36;
                remainder /= 36;

                if (digit < 10)
                    result.Add(digit.ToString()[0]);
                else
                    result.Add((char)(digit - 10 + 'A'));
            }

            if (i < 0)
                result.Add('-');

            result.Reverse();

            return new string(result.ToArray());
        }
        #endregion



        private void reloadWorld_Click(object sender, EventArgs e)
        {
            LoadWorld(world.Path, world.ToString());
        }

        #region Editing
        private void btnSaveVal_Click(object sender, EventArgs e)
        {
            TagType curTag = ((Tag)txtBoxVal.Tag).Type;
            NBT.Tag daddyTag;
            string name;
            switch (curTag)
            {
                case TagType.Byte:
                    Tag nbtProp = (Tag)txtBoxVal.Tag;
                    name = nbtProp.Name;
                    try
                    {
                        byte val = byte.Parse(txtBoxVal.Text);
                        daddyTag = nbtProp.Parent;
                        nbtProp.Parent[name].Remove();
                        daddyTag.Add(name, val);
                        txtBoxVal.Tag = nbtProp = daddyTag[name];
                        
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Value must be a byte!", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case TagType.Double:
                    Tag nbtProp1 = (Tag)txtBoxVal.Tag;
                    name = nbtProp1.Name;
                    try
                    {
                        double val = double.Parse(txtBoxVal.Text);
                        daddyTag = nbtProp1.Parent;
                        nbtProp1.Parent[name].Remove();
                        daddyTag.Add(name, val);
                        nbtProp1 = daddyTag[name];
                        txtBoxVal.Tag = nbtProp1;
                        
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Value must be a number!", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case TagType.Float:
                    Tag nbtProp2 = (Tag)txtBoxVal.Tag;
                    name = nbtProp2.Name;
                    try
                    {
                        float val = float.Parse(txtBoxVal.Text);
                        daddyTag = nbtProp2.Parent;
                        nbtProp2.Parent[name].Remove();
                        daddyTag.Add(name, val);
                        nbtProp2 = daddyTag[name];
                        txtBoxVal.Tag = nbtProp2;
                        
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Value must be a number!", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case TagType.Int:
                    Tag nbtProp3 = (Tag)txtBoxVal.Tag;
                    name = nbtProp3.Name;
                    try
                    {
                        int val = int.Parse(txtBoxVal.Text);
                        daddyTag = nbtProp3.Parent;
                        nbtProp3.Parent[name].Remove();
                        daddyTag.Add(name, val);
                        nbtProp3 = daddyTag[name];
                        txtBoxVal.Tag = nbtProp3;
                        
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Value must be a number!", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case TagType.Long:
                    Tag nbtProp4 = (Tag)txtBoxVal.Tag;
                    name = nbtProp4.Name;
                    try
                    {
                        long val = long.Parse(txtBoxVal.Text);
                        daddyTag = nbtProp4.Parent;
                        nbtProp4.Parent[name].Remove();
                        daddyTag.Add(name, val);
                        nbtProp4 = daddyTag[name];
                        txtBoxVal.Tag = nbtProp4;
                        
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Value must be a number!", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case TagType.Short:
                    Tag nbtProp5 = (Tag)txtBoxVal.Tag;
                    name = nbtProp5.Name;
                    try
                    {
                        short val = short.Parse(txtBoxVal.Text);
                        daddyTag = nbtProp5.Parent;
                        nbtProp5.Parent[name].Remove();
                        daddyTag.Add(name, val);
                        nbtProp5 = daddyTag[name];
                        txtBoxVal.Tag = nbtProp5;
                        
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Value must be a number!", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case TagType.String:
                    Tag nbtProp6 = (Tag)txtBoxVal.Tag;
                    name = nbtProp6.Name;
                    try
                    {
                        string val = txtBoxVal.Text;
                        daddyTag = nbtProp6.Parent;
                        nbtProp6.Parent[name].Remove();
                        daddyTag.Add(name, val);
                        nbtProp6 = daddyTag[name];
                        txtBoxVal.Tag = nbtProp6;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("An error has been encountered.", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                default:
                    break;
            }
            if (mainTreeView.SelectedNode.Name == "levdata.")
                world.Lvl.Modified = true;
            else
                selectedChunk.Modified = true;
        }
        #endregion
    }
}