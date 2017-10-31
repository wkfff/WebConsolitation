using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Krista.FM.Common.TaskParamEditors
{
    partial class MembersTree
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MembersTree));
            this.TVImageList = new System.Windows.Forms.ImageList(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.PrevCheckedButton = new System.Windows.Forms.Button();
            this.ButtonsImageList = new System.Windows.Forms.ImageList(this.components);
            this.NextCheckedButton = new System.Windows.Forms.Button();
            this.ExpandCheckedButton = new System.Windows.Forms.Button();
            this.CollapseButton = new System.Windows.Forms.Button();
            this.ExpandButton = new System.Windows.Forms.Button();
            this.InvertButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemNone = new System.Windows.Forms.ToolStripMenuItem();
            this.itemChildren = new System.Windows.Forms.ToolStripMenuItem();
            this.itemDescendants = new System.Windows.Forms.ToolStripMenuItem();
            this.itemExclude = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.itemSelectChildren = new System.Windows.Forms.ToolStripMenuItem();
            this.itemSelectDescendants = new System.Windows.Forms.ToolStripMenuItem();
            this.itemDeselectChildren = new System.Windows.Forms.ToolStripMenuItem();
            this.itemDeselectDescendants = new System.Windows.Forms.ToolStripMenuItem();
            this.pnLevels = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.levelMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.itemEnable = new System.Windows.Forms.ToolStripMenuItem();
            this.itemForce = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.itemCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.itemUncheck = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.treeMenu.SuspendLayout();
            this.pnLevels.SuspendLayout();
            this.levelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TVImageList
            // 
            this.TVImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TVImageList.ImageStream")));
            this.TVImageList.TransparentColor = System.Drawing.Color.Magenta;
            this.TVImageList.Images.SetKeyName(0, "Blue.bmp");
            this.TVImageList.Images.SetKeyName(1, "SimpleBlue.bmp");
            this.TVImageList.Images.SetKeyName(2, "Green.bmp");
            this.TVImageList.Images.SetKeyName(3, "SimpleGreen.bmp");
            this.TVImageList.Images.SetKeyName(4, "Red.bmp");
            this.TVImageList.Images.SetKeyName(5, "SimpleRed.bmp");
            this.TVImageList.Images.SetKeyName(6, "Grey.bmp");
            this.TVImageList.Images.SetKeyName(7, "SimpleGrey.bmp");
            this.TVImageList.Images.SetKeyName(8, "SimpleWhite.bmp");
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panelButtons);
            this.panel3.Controls.Add(this.treeView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 20);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(480, 340);
            this.panel3.TabIndex = 4;
            // 
            // panelButtons
            // 
            this.panelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelButtons.Controls.Add(this.PrevCheckedButton);
            this.panelButtons.Controls.Add(this.NextCheckedButton);
            this.panelButtons.Controls.Add(this.ExpandCheckedButton);
            this.panelButtons.Controls.Add(this.CollapseButton);
            this.panelButtons.Controls.Add(this.ExpandButton);
            this.panelButtons.Controls.Add(this.InvertButton);
            this.panelButtons.Controls.Add(this.DeselectAllButton);
            this.panelButtons.Controls.Add(this.SelectAllButton);
            this.panelButtons.Location = new System.Drawing.Point(444, 0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(32, 337);
            this.panelButtons.TabIndex = 2;
            // 
            // PrevCheckedButton
            // 
            this.PrevCheckedButton.ImageKey = "PrevChecked.bmp";
            this.PrevCheckedButton.ImageList = this.ButtonsImageList;
            this.PrevCheckedButton.Location = new System.Drawing.Point(4, 160);
            this.PrevCheckedButton.Name = "PrevCheckedButton";
            this.PrevCheckedButton.Size = new System.Drawing.Size(24, 24);
            this.PrevCheckedButton.TabIndex = 7;
            this.PrevCheckedButton.UseVisualStyleBackColor = true;
            this.PrevCheckedButton.Click += new System.EventHandler(this.PrevCheckedButton_Click);
            this.PrevCheckedButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // ButtonsImageList
            // 
            this.ButtonsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonsImageList.ImageStream")));
            this.ButtonsImageList.TransparentColor = System.Drawing.Color.White;
            this.ButtonsImageList.Images.SetKeyName(0, "Plus.bmp");
            this.ButtonsImageList.Images.SetKeyName(1, "Minus.bmp");
            this.ButtonsImageList.Images.SetKeyName(2, "Star.bmp");
            this.ButtonsImageList.Images.SetKeyName(3, "Expand.bmp");
            this.ButtonsImageList.Images.SetKeyName(4, "Collapse.bmp");
            this.ButtonsImageList.Images.SetKeyName(5, "ExpandChecked.bmp");
            this.ButtonsImageList.Images.SetKeyName(6, "NextChecked.bmp");
            this.ButtonsImageList.Images.SetKeyName(7, "PrevChecked.bmp");
            // 
            // NextCheckedButton
            // 
            this.NextCheckedButton.ImageKey = "NextChecked.bmp";
            this.NextCheckedButton.ImageList = this.ButtonsImageList;
            this.NextCheckedButton.Location = new System.Drawing.Point(4, 186);
            this.NextCheckedButton.Name = "NextCheckedButton";
            this.NextCheckedButton.Size = new System.Drawing.Size(24, 24);
            this.NextCheckedButton.TabIndex = 6;
            this.NextCheckedButton.UseVisualStyleBackColor = true;
            this.NextCheckedButton.Click += new System.EventHandler(this.NextCheckedButton_Click);
            this.NextCheckedButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // ExpandCheckedButton
            // 
            this.ExpandCheckedButton.ImageKey = "ExpandChecked.bmp";
            this.ExpandCheckedButton.ImageList = this.ButtonsImageList;
            this.ExpandCheckedButton.Location = new System.Drawing.Point(4, 134);
            this.ExpandCheckedButton.Name = "ExpandCheckedButton";
            this.ExpandCheckedButton.Size = new System.Drawing.Size(24, 24);
            this.ExpandCheckedButton.TabIndex = 5;
            this.ExpandCheckedButton.UseVisualStyleBackColor = true;
            this.ExpandCheckedButton.Click += new System.EventHandler(this.ExpandCheckedButton_Click);
            this.ExpandCheckedButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // CollapseButton
            // 
            this.CollapseButton.ImageKey = "Collapse.bmp";
            this.CollapseButton.ImageList = this.ButtonsImageList;
            this.CollapseButton.Location = new System.Drawing.Point(4, 108);
            this.CollapseButton.Name = "CollapseButton";
            this.CollapseButton.Size = new System.Drawing.Size(24, 24);
            this.CollapseButton.TabIndex = 4;
            this.CollapseButton.UseVisualStyleBackColor = true;
            this.CollapseButton.Click += new System.EventHandler(this.CollapseButton_Click);
            this.CollapseButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // ExpandButton
            // 
            this.ExpandButton.ImageKey = "Expand.bmp";
            this.ExpandButton.ImageList = this.ButtonsImageList;
            this.ExpandButton.Location = new System.Drawing.Point(4, 82);
            this.ExpandButton.Name = "ExpandButton";
            this.ExpandButton.Size = new System.Drawing.Size(24, 24);
            this.ExpandButton.TabIndex = 3;
            this.ExpandButton.UseVisualStyleBackColor = true;
            this.ExpandButton.Click += new System.EventHandler(this.ExpandButton_Click);
            this.ExpandButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // InvertButton
            // 
            this.InvertButton.ImageKey = "Star.bmp";
            this.InvertButton.ImageList = this.ButtonsImageList;
            this.InvertButton.Location = new System.Drawing.Point(4, 56);
            this.InvertButton.Name = "InvertButton";
            this.InvertButton.Size = new System.Drawing.Size(24, 24);
            this.InvertButton.TabIndex = 2;
            this.InvertButton.UseVisualStyleBackColor = true;
            this.InvertButton.Click += new System.EventHandler(this.InvertButton_Click);
            this.InvertButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.ImageKey = "Minus.bmp";
            this.DeselectAllButton.ImageList = this.ButtonsImageList;
            this.DeselectAllButton.Location = new System.Drawing.Point(4, 30);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(24, 24);
            this.DeselectAllButton.TabIndex = 1;
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            this.DeselectAllButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.SelectAllButton.ImageKey = "Plus.bmp";
            this.SelectAllButton.ImageList = this.ButtonsImageList;
            this.SelectAllButton.Location = new System.Drawing.Point(4, 4);
            this.SelectAllButton.Margin = new System.Windows.Forms.Padding(0);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(24, 24);
            this.SelectAllButton.TabIndex = 0;
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            this.SelectAllButton.Enter += new System.EventHandler(this.ButtonEnterHandler);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CheckBoxes = true;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.TVImageList;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(436, 334);
            this.treeView1.TabIndex = 1;
            this.treeView1.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCheck);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // treeMenu
            // 
            this.treeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemNone,
            this.itemChildren,
            this.itemDescendants,
            this.itemExclude,
            this.toolStripSeparator2,
            this.itemSelectChildren,
            this.itemSelectDescendants,
            this.itemDeselectChildren,
            this.itemDeselectDescendants});
            this.treeMenu.Name = "treeMenu";
            this.treeMenu.Size = new System.Drawing.Size(351, 186);
            this.treeMenu.Opening += new System.ComponentModel.CancelEventHandler(this.treeMenu_Opening);
            this.treeMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.treeMenu_ItemClicked);
            // 
            // itemNone
            // 
            this.itemNone.Name = "itemNone";
            this.itemNone.Size = new System.Drawing.Size(350, 22);
            this.itemNone.Text = "Не влияет на подчиненные элементы";
            // 
            // itemChildren
            // 
            this.itemChildren.Name = "itemChildren";
            this.itemChildren.Size = new System.Drawing.Size(350, 22);
            this.itemChildren.Text = "Автоматически включает дочерние элементы";
            // 
            // itemDescendants
            // 
            this.itemDescendants.Name = "itemDescendants";
            this.itemDescendants.Size = new System.Drawing.Size(350, 22);
            this.itemDescendants.Text = "Автоматически включает все подчиненные элементы";
            // 
            // itemExclude
            // 
            this.itemExclude.Name = "itemExclude";
            this.itemExclude.Size = new System.Drawing.Size(350, 22);
            this.itemExclude.Text = "Элемент всегда игнорируется";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(347, 6);
            // 
            // itemSelectChildren
            // 
            this.itemSelectChildren.Name = "itemSelectChildren";
            this.itemSelectChildren.Size = new System.Drawing.Size(350, 22);
            this.itemSelectChildren.Text = "Выделить дочерние элементы";
            // 
            // itemSelectDescendants
            // 
            this.itemSelectDescendants.Name = "itemSelectDescendants";
            this.itemSelectDescendants.Size = new System.Drawing.Size(350, 22);
            this.itemSelectDescendants.Text = "Выделить все подчиненные элементы";
            // 
            // itemDeselectChildren
            // 
            this.itemDeselectChildren.Name = "itemDeselectChildren";
            this.itemDeselectChildren.Size = new System.Drawing.Size(350, 22);
            this.itemDeselectChildren.Text = "Снять выделение с дочерних элементов";
            // 
            // itemDeselectDescendants
            // 
            this.itemDeselectDescendants.Name = "itemDeselectDescendants";
            this.itemDeselectDescendants.Size = new System.Drawing.Size(350, 22);
            this.itemDeselectDescendants.Text = "Снять выделение со всех подчиненных элементов";
            // 
            // pnLevels
            // 
            this.pnLevels.Controls.Add(this.linkLabel1);
            this.pnLevels.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnLevels.Location = new System.Drawing.Point(0, 0);
            this.pnLevels.MinimumSize = new System.Drawing.Size(0, 20);
            this.pnLevels.Name = "pnLevels";
            this.pnLevels.Size = new System.Drawing.Size(480, 20);
            this.pnLevels.TabIndex = 5;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkLabel1.Location = new System.Drawing.Point(380, 4);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(94, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Показать уровни";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // levelMenu
            // 
            this.levelMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemDisable,
            this.itemEnable,
            this.itemForce,
            this.toolStripSeparator1,
            this.itemCheck,
            this.itemUncheck});
            this.levelMenu.Name = "levelMenu";
            this.levelMenu.ShowCheckMargin = true;
            this.levelMenu.ShowImageMargin = false;
            this.levelMenu.Size = new System.Drawing.Size(301, 120);
            this.levelMenu.Opening += new System.ComponentModel.CancelEventHandler(this.levelMenu_Opening);
            this.levelMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.levelMenu_ItemClicked);
            // 
            // itemDisable
            // 
            this.itemDisable.Name = "itemDisable";
            this.itemDisable.Size = new System.Drawing.Size(300, 22);
            this.itemDisable.Text = "Исключить уровень";
            // 
            // itemEnable
            // 
            this.itemEnable.Name = "itemEnable";
            this.itemEnable.Size = new System.Drawing.Size(300, 22);
            this.itemEnable.Text = "Включить уровень";
            // 
            // itemForce
            // 
            this.itemForce.Name = "itemForce";
            this.itemForce.Size = new System.Drawing.Size(300, 22);
            this.itemForce.Text = "Включить уровень со всеми элементами";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(297, 6);
            // 
            // itemCheck
            // 
            this.itemCheck.Name = "itemCheck";
            this.itemCheck.Size = new System.Drawing.Size(300, 22);
            this.itemCheck.Text = "Выделить все элементы уровня";
            // 
            // itemUncheck
            // 
            this.itemUncheck.Name = "itemUncheck";
            this.itemUncheck.Size = new System.Drawing.Size(300, 22);
            this.itemUncheck.Text = "Снять выделение со всех элементов уровня";
            // 
            // MembersTree
            // 
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pnLevels);
            this.MinimumSize = new System.Drawing.Size(480, 360);
            this.Name = "MembersTree";
            this.Size = new System.Drawing.Size(480, 360);
            this.panel3.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.treeMenu.ResumeLayout(false);
            this.pnLevels.ResumeLayout(false);
            this.pnLevels.PerformLayout();
            this.levelMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageList TVImageList;
        private Panel panel3;
        private Panel panelButtons;
        private Button PrevCheckedButton;
        private Button NextCheckedButton;
        private Button ExpandCheckedButton;
        private Button CollapseButton;
        private Button ExpandButton;
        private Button InvertButton;
        private Button DeselectAllButton;
        private Button SelectAllButton;
        private TreeView treeView1;
        private Panel pnLevels;
        private LinkLabel linkLabel1;
        private ContextMenuStrip levelMenu;
        private ToolStripMenuItem itemDisable;
        private ToolStripMenuItem itemEnable;
        private ToolStripMenuItem itemForce;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem itemCheck;
        private ToolStripMenuItem itemUncheck;
        private ContextMenuStrip treeMenu;
        private ToolStripMenuItem itemNone;
        private ToolStripMenuItem itemChildren;
        private ToolStripMenuItem itemDescendants;
        private ToolStripMenuItem itemExclude;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem itemSelectChildren;
        private ToolStripMenuItem itemSelectDescendants;
        private ToolStripMenuItem itemDeselectChildren;
        private ToolStripMenuItem itemDeselectDescendants;
        private ImageList ButtonsImageList;
    }
}
