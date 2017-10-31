namespace Krista.FM.Client.MDXExpert
{
    partial class MapTemplateBrowser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapTemplateBrowser));
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btClear = new Infragistics.Win.Misc.UltraButton();
            this.btCancel = new Infragistics.Win.Misc.UltraButton();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.tvMaps = new Infragistics.Win.UltraWinTree.UltraTree();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tvMaps)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.ultraGroupBox1.Controls.Add(this.btClear);
            this.ultraGroupBox1.Controls.Add(this.btCancel);
            this.ultraGroupBox1.Controls.Add(this.btOK);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 186);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(227, 40);
            this.ultraGroupBox1.TabIndex = 0;
            // 
            // btClear
            // 
            this.btClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btClear.Location = new System.Drawing.Point(14, 9);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(65, 23);
            this.btClear.TabIndex = 2;
            this.btClear.Text = "Очистить";
            this.btClear.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.Location = new System.Drawing.Point(85, 9);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(65, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Отмена";
            this.btCancel.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Enabled = false;
            this.btOK.Location = new System.Drawing.Point(156, 9);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(65, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // tvMaps
            // 
            this.tvMaps.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.tvMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvMaps.HideSelection = false;
            this.tvMaps.ImageList = this.imageList;
            this.tvMaps.Location = new System.Drawing.Point(0, 0);
            this.tvMaps.Name = "tvMaps";
            this.tvMaps.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
            this.tvMaps.Size = new System.Drawing.Size(227, 186);
            this.tvMaps.TabIndex = 1;
            this.tvMaps.AfterExpand += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.tvMaps_AfterExpand);
            this.tvMaps.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvMaps_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Folder.bmp");
            this.imageList.Images.SetKeyName(1, "template.bmp");
            // 
            // MapTemplateBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvMaps);
            this.Controls.Add(this.ultraGroupBox1);
            this.Name = "MapTemplateBrowser";
            this.Size = new System.Drawing.Size(227, 226);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tvMaps)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.UltraWinTree.UltraTree tvMaps;
        private System.Windows.Forms.ImageList imageList;
        private Infragistics.Win.Misc.UltraButton btClear;
        private Infragistics.Win.Misc.UltraButton btCancel;
    }
}
