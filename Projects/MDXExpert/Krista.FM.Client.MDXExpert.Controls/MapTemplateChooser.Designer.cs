namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class MapTemplateChooser
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapTemplateChooser));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.teRepository = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.tvMaps = new Infragistics.Win.UltraWinTree.UltraTree();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.btCancel = new Infragistics.Win.Misc.UltraButton();
            this.lTemplates = new Infragistics.Win.Misc.UltraLabel();
            this.lRepository = new Infragistics.Win.Misc.UltraLabel();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.teRepository)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvMaps)).BeginInit();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // teRepository
            // 
            this.teRepository.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BorderColor = System.Drawing.Color.Black;
            this.teRepository.Appearance = appearance2;
            this.teRepository.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            editorButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Borderless;
            editorButton1.Text = "...";
            this.teRepository.ButtonsRight.Add(editorButton1);
            this.teRepository.Location = new System.Drawing.Point(12, 91);
            this.teRepository.Name = "teRepository";
            this.teRepository.Size = new System.Drawing.Size(404, 19);
            this.teRepository.TabIndex = 0;
            this.teRepository.UseAppStyling = false;
            this.teRepository.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.teRepository_EditorButtonClick);
            // 
            // tvMaps
            // 
            this.tvMaps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMaps.HideSelection = false;
            this.tvMaps.ImageList = this.imageList;
            this.tvMaps.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tvMaps.Location = new System.Drawing.Point(12, 141);
            this.tvMaps.Name = "tvMaps";
            this.tvMaps.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            this.tvMaps.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
            this.tvMaps.Size = new System.Drawing.Size(404, 166);
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
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(262, 313);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ultraPanel1
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            this.ultraPanel1.Appearance = appearance1;
            this.ultraPanel1.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.ultraLabel2);
            this.ultraPanel1.ClientArea.Controls.Add(this.ultraPictureBox1);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(428, 60);
            this.ultraPanel1.TabIndex = 8;
            this.ultraPanel1.UseAppStyling = false;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ultraLabel2.Location = new System.Drawing.Point(11, 11);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(138, 14);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "Выберите шаблон карты.";
            // 
            // ultraPictureBox1
            // 
            this.ultraPictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
            this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
            this.ultraPictureBox1.Location = new System.Drawing.Point(370, 2);
            this.ultraPictureBox1.Name = "ultraPictureBox1";
            this.ultraPictureBox1.Size = new System.Drawing.Size(50, 43);
            this.ultraPictureBox1.TabIndex = 0;
            this.ultraPictureBox1.UseAppStyling = false;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(343, 313);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 9;
            this.btCancel.Text = "Отмена";
            // 
            // lTemplates
            // 
            this.lTemplates.AutoSize = true;
            this.lTemplates.Location = new System.Drawing.Point(12, 125);
            this.lTemplates.Name = "lTemplates";
            this.lTemplates.Size = new System.Drawing.Size(57, 14);
            this.lTemplates.TabIndex = 10;
            this.lTemplates.Text = "Шаблоны:";
            // 
            // lRepository
            // 
            this.lRepository.AutoSize = true;
            this.lRepository.Location = new System.Drawing.Point(12, 75);
            this.lRepository.Name = "lRepository";
            this.lRepository.Size = new System.Drawing.Size(156, 14);
            this.lRepository.TabIndex = 11;
            this.lRepository.Text = "Расположение репозитория:";
            // 
            // MapTemplateChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 341);
            this.ControlBox = false;
            this.Controls.Add(this.lRepository);
            this.Controls.Add(this.lTemplates);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.ultraPanel1);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.tvMaps);
            this.Controls.Add(this.teRepository);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapTemplateChooser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Выбор шаблона карты";
            ((System.ComponentModel.ISupportInitialize)(this.teRepository)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvMaps)).EndInit();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ClientArea.PerformLayout();
            this.ultraPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor teRepository;
        private Infragistics.Win.UltraWinTree.UltraTree tvMaps;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.Misc.UltraButton btCancel;
        private Infragistics.Win.Misc.UltraLabel lTemplates;
        private Infragistics.Win.Misc.UltraLabel lRepository;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ImageList imageList;
    }
}