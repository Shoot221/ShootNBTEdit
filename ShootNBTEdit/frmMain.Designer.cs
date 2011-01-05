namespace ShootNBTEdit
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblSelectedType = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripSplitButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.reloadWorld = new System.Windows.Forms.ToolStripButton();
            this.bulletsList = new System.Windows.Forms.ImageList(this.components);
            this.mainTreeView = new System.Windows.Forms.TreeView();
            this.facesList = new System.Windows.Forms.ImageList(this.components);
            this.spltContainer = new System.Windows.Forms.SplitContainer();
            this.btnSaveVal = new System.Windows.Forms.Button();
            this.txtBoxVal = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltContainer)).BeginInit();
            this.spltContainer.Panel1.SuspendLayout();
            this.spltContainer.Panel2.SuspendLayout();
            this.spltContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblSelectedType,
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 390);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(634, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblSelectedType
            // 
            this.lblSelectedType.Name = "lblSelectedType";
            this.lblSelectedType.Size = new System.Drawing.Size(0, 17);
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(57, 17);
            this.lblStatus.Text = "Waiting...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSave,
            this.reloadWorld});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(634, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::ShootNBTEdit.Properties.Resources.folder_open;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(32, 22);
            this.btnOpen.Text = "Open";
            this.btnOpen.ToolTipText = "Open (CTRL + O)";
            this.btnOpen.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnOpen_DropDownItemClicked);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Enabled = false;
            this.btnSave.Image = global::ShootNBTEdit.Properties.Resources.disk_return_black;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save (CTRL + S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // reloadWorld
            // 
            this.reloadWorld.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reloadWorld.Enabled = false;
            this.reloadWorld.Image = global::ShootNBTEdit.Properties.Resources.arrow_circle;
            this.reloadWorld.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadWorld.Name = "reloadWorld";
            this.reloadWorld.Size = new System.Drawing.Size(23, 22);
            this.reloadWorld.Text = "Reload world";
            this.reloadWorld.Click += new System.EventHandler(this.reloadWorld_Click);
            // 
            // bulletsList
            // 
            this.bulletsList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("bulletsList.ImageStream")));
            this.bulletsList.TransparentColor = System.Drawing.Color.Transparent;
            this.bulletsList.Images.SetKeyName(0, "bullet_yellow.png");
            this.bulletsList.Images.SetKeyName(1, "bullet_red.png");
            this.bulletsList.Images.SetKeyName(2, "bullet_pink.png");
            this.bulletsList.Images.SetKeyName(3, "bullet_purple.png");
            this.bulletsList.Images.SetKeyName(4, "bullet_green.png");
            this.bulletsList.Images.SetKeyName(5, "bullet_orange.png");
            this.bulletsList.Images.SetKeyName(6, "bullet_blue.png");
            this.bulletsList.Images.SetKeyName(7, "bullet_black.png");
            // 
            // mainTreeView
            // 
            this.mainTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainTreeView.Cursor = System.Windows.Forms.Cursors.Default;
            this.mainTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTreeView.HideSelection = false;
            this.mainTreeView.ImageIndex = 0;
            this.mainTreeView.ImageList = this.facesList;
            this.mainTreeView.Location = new System.Drawing.Point(0, 0);
            this.mainTreeView.Name = "mainTreeView";
            this.mainTreeView.SelectedImageIndex = 8;
            this.mainTreeView.Size = new System.Drawing.Size(322, 365);
            this.mainTreeView.TabIndex = 3;
            this.mainTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.mainTreeView_NodeMouseClick);
            // 
            // facesList
            // 
            this.facesList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("facesList.ImageStream")));
            this.facesList.TransparentColor = System.Drawing.Color.Transparent;
            this.facesList.Images.SetKeyName(0, "bullet_black.png");
            this.facesList.Images.SetKeyName(1, "bullet_blue.png");
            this.facesList.Images.SetKeyName(2, "bullet_green.png");
            this.facesList.Images.SetKeyName(3, "bullet_orange.png");
            this.facesList.Images.SetKeyName(4, "bullet_pink.png");
            this.facesList.Images.SetKeyName(5, "bullet_purple.png");
            this.facesList.Images.SetKeyName(6, "bullet_red.png");
            this.facesList.Images.SetKeyName(7, "bullet_star.png");
            this.facesList.Images.SetKeyName(8, "bullet_white.png");
            this.facesList.Images.SetKeyName(9, "bullet_yellow.png");
            this.facesList.Images.SetKeyName(10, "Arrow.png");
            this.facesList.Images.SetKeyName(11, "Boat.png");
            this.facesList.Images.SetKeyName(12, "Chest.png");
            this.facesList.Images.SetKeyName(13, "ChickenFace.png");
            this.facesList.Images.SetKeyName(14, "CowFace.png");
            this.facesList.Images.SetKeyName(15, "CreeperFace.png");
            this.facesList.Images.SetKeyName(16, "disk-return-black.png");
            this.facesList.Images.SetKeyName(17, "Egg.png");
            this.facesList.Images.SetKeyName(18, "FallingSand.png");
            this.facesList.Images.SetKeyName(19, "folder.png");
            this.facesList.Images.SetKeyName(20, "folder-open.png");
            this.facesList.Images.SetKeyName(21, "Furnace.png");
            this.facesList.Images.SetKeyName(22, "GhastFace.png");
            this.facesList.Images.SetKeyName(23, "HumanFace.png");
            this.facesList.Images.SetKeyName(24, "Minecart.png");
            this.facesList.Images.SetKeyName(25, "Mobspawner.png");
            this.facesList.Images.SetKeyName(26, "Paintingit.png");
            this.facesList.Images.SetKeyName(27, "PigFace.png");
            this.facesList.Images.SetKeyName(28, "PrimedTnt.png");
            this.facesList.Images.SetKeyName(29, "SheepFace.png");
            this.facesList.Images.SetKeyName(30, "Sign.png");
            this.facesList.Images.SetKeyName(31, "SkeletonFace.png");
            this.facesList.Images.SetKeyName(32, "SlimeFace.png");
            this.facesList.Images.SetKeyName(33, "Snowball.png");
            this.facesList.Images.SetKeyName(34, "Spider+SkeletonFace.png");
            this.facesList.Images.SetKeyName(35, "SpiderFace.png");
            this.facesList.Images.SetKeyName(36, "ZombieFace.png");
            this.facesList.Images.SetKeyName(37, "ZombiePigmanFace.png");
            this.facesList.Images.SetKeyName(38, "chunk.png");
            this.facesList.Images.SetKeyName(39, "Diamondpickaxe.png");
            this.facesList.Images.SetKeyName(40, "leveldat.png");
            this.facesList.Images.SetKeyName(41, "EmptyPixel.png");
            // 
            // spltContainer
            // 
            this.spltContainer.Cursor = System.Windows.Forms.Cursors.Default;
            this.spltContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltContainer.Location = new System.Drawing.Point(0, 25);
            this.spltContainer.Name = "spltContainer";
            // 
            // spltContainer.Panel1
            // 
            this.spltContainer.Panel1.Controls.Add(this.mainTreeView);
            // 
            // spltContainer.Panel2
            // 
            this.spltContainer.Panel2.Controls.Add(this.btnSaveVal);
            this.spltContainer.Panel2.Controls.Add(this.txtBoxVal);
            this.spltContainer.Panel2MinSize = 60;
            this.spltContainer.Size = new System.Drawing.Size(634, 365);
            this.spltContainer.SplitterDistance = 322;
            this.spltContainer.SplitterWidth = 6;
            this.spltContainer.TabIndex = 5;
            // 
            // btnSaveVal
            // 
            this.btnSaveVal.Location = new System.Drawing.Point(44, 215);
            this.btnSaveVal.Name = "btnSaveVal";
            this.btnSaveVal.Size = new System.Drawing.Size(218, 23);
            this.btnSaveVal.TabIndex = 6;
            this.btnSaveVal.Text = "Save Value";
            this.btnSaveVal.UseVisualStyleBackColor = true;
            this.btnSaveVal.Visible = false;
            this.btnSaveVal.Click += new System.EventHandler(this.btnSaveVal_Click);
            // 
            // txtBoxVal
            // 
            this.txtBoxVal.BackColor = System.Drawing.Color.White;
            this.txtBoxVal.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtBoxVal.Location = new System.Drawing.Point(0, 0);
            this.txtBoxVal.Multiline = true;
            this.txtBoxVal.Name = "txtBoxVal";
            this.txtBoxVal.Size = new System.Drawing.Size(306, 194);
            this.txtBoxVal.TabIndex = 5;
            this.txtBoxVal.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 412);
            this.Controls.Add(this.spltContainer);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "ShootNBTEdit";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.spltContainer.Panel1.ResumeLayout(false);
            this.spltContainer.Panel2.ResumeLayout(false);
            this.spltContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltContainer)).EndInit();
            this.spltContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSplitButton btnOpen;
        private System.Windows.Forms.ImageList bulletsList;
        public System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.TreeView mainTreeView;
        private System.Windows.Forms.ImageList facesList;
        private System.Windows.Forms.ToolStripStatusLabel lblSelectedType;
        private System.Windows.Forms.ToolStripButton reloadWorld;
        private System.Windows.Forms.TextBox txtBoxVal;
        private System.Windows.Forms.Button btnSaveVal;
        private System.Windows.Forms.SplitContainer spltContainer;
    }
}

