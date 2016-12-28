using System;

namespace ClipBoard
{
    partial class MainForm
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Saved", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Frequent", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Recent", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listView = new System.Windows.Forms.ListView();
            this.indexColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.countCoppiedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.countPastedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.labelClipBoardManager = new System.Windows.Forms.Label();
            this.linkLabelGitHub = new System.Windows.Forms.LinkLabel();
            this.labelMinimize = new System.Windows.Forms.Label();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.indexColumnHeader,
            this.countCoppiedColumnHeader,
            this.countPastedColumnHeader,
            this.textColumnHeader});
            this.listView.ForeColor = System.Drawing.Color.Black;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            listViewGroup1.Header = "Saved";
            listViewGroup1.Name = "savedListViewGroup";
            listViewGroup2.Header = "Frequent";
            listViewGroup2.Name = "frequentListViewGroup";
            listViewGroup3.Header = "Recent";
            listViewGroup3.Name = "recentListViewGroup";
            this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.Location = new System.Drawing.Point(9, 46);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Scrollable = false;
            this.listView.Size = new System.Drawing.Size(265, 276);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            this.listView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseClick);
            // 
            // indexColumnHeader
            // 
            this.indexColumnHeader.Text = "Index";
            this.indexColumnHeader.Width = 20;
            // 
            // countCoppiedColumnHeader
            // 
            this.countCoppiedColumnHeader.Text = "Copy Count";
            this.countCoppiedColumnHeader.Width = 0;
            // 
            // countPastedColumnHeader
            // 
            this.countPastedColumnHeader.Text = "Paste Count";
            this.countPastedColumnHeader.Width = 0;
            // 
            // textColumnHeader
            // 
            this.textColumnHeader.Text = "Content";
            this.textColumnHeader.Width = 242;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(118, 48);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "ClipBoard Manager";
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // labelClipBoardManager
            // 
            this.labelClipBoardManager.Font = new System.Drawing.Font("Trebuchet MS", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClipBoardManager.ForeColor = System.Drawing.Color.White;
            this.labelClipBoardManager.Location = new System.Drawing.Point(3, 9);
            this.labelClipBoardManager.Name = "labelClipBoardManager";
            this.labelClipBoardManager.Size = new System.Drawing.Size(269, 34);
            this.labelClipBoardManager.TabIndex = 1;
            this.labelClipBoardManager.Text = "ClipBoard Manager";
            this.labelClipBoardManager.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabelGitHub
            // 
            this.linkLabelGitHub.AutoSize = true;
            this.linkLabelGitHub.LinkColor = System.Drawing.Color.White;
            this.linkLabelGitHub.Location = new System.Drawing.Point(43, 335);
            this.linkLabelGitHub.Name = "linkLabelGitHub";
            this.linkLabelGitHub.Size = new System.Drawing.Size(184, 13);
            this.linkLabelGitHub.TabIndex = 2;
            this.linkLabelGitHub.TabStop = true;
            this.linkLabelGitHub.Text = "https://github.com/ralphite/ClipBoard";
            this.linkLabelGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGitHub_LinkClicked);
            // 
            // labelMinimize
            // 
            this.labelMinimize.AutoSize = true;
            this.labelMinimize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinimize.Location = new System.Drawing.Point(256, -1);
            this.labelMinimize.Name = "labelMinimize";
            this.labelMinimize.Size = new System.Drawing.Size(18, 26);
            this.labelMinimize.TabIndex = 3;
            this.labelMinimize.Text = "-";
            this.labelMinimize.Click += new System.EventHandler(this.labelMinimize_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(284, 347);
            this.Controls.Add(this.labelMinimize);
            this.Controls.Add(this.linkLabelGitHub);
            this.Controls.Add(this.labelClipBoardManager);
            this.Controls.Add(this.listView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Opacity = 0.96D;
            this.Text = "ClipBoard Manager";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader indexColumnHeader;
        private System.Windows.Forms.ColumnHeader textColumnHeader;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ColumnHeader countCoppiedColumnHeader;
        private System.Windows.Forms.ColumnHeader countPastedColumnHeader;
        private System.Windows.Forms.Label labelClipBoardManager;
        private System.Windows.Forms.LinkLabel linkLabelGitHub;
        private System.Windows.Forms.Label labelMinimize;
    }
}

