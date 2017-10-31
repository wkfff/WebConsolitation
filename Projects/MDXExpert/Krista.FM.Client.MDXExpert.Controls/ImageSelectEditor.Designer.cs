namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class ImageSelectControl
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
            this.container = new Infragistics.Win.Misc.UltraGroupBox();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.btClear = new Infragistics.Win.Misc.UltraButton();
            this.btSelect = new Infragistics.Win.Misc.UltraButton();
            this.pbImage = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.container)).BeginInit();
            this.container.SuspendLayout();
            this.SuspendLayout();
            // 
            // container
            // 
            this.container.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.container.Controls.Add(this.btOK);
            this.container.Controls.Add(this.btClear);
            this.container.Controls.Add(this.btSelect);
            this.container.Controls.Add(this.pbImage);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(0, 0);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(246, 217);
            this.container.TabIndex = 0;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(166, 185);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(64, 23);
            this.btOK.TabIndex = 3;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btClear
            // 
            this.btClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btClear.Location = new System.Drawing.Point(83, 185);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(64, 23);
            this.btClear.TabIndex = 2;
            this.btClear.Text = "Очистить";
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btSelect
            // 
            this.btSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSelect.Location = new System.Drawing.Point(13, 185);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(64, 23);
            this.btSelect.TabIndex = 1;
            this.btSelect.Text = "Обзор...";
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // pbImage
            // 
            this.pbImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbImage.BorderShadowColor = System.Drawing.Color.Empty;
            this.pbImage.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.pbImage.Location = new System.Drawing.Point(14, 14);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(216, 165);
            this.pbImage.TabIndex = 0;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Файлы рисунков(*.bmp, *.gif, *.jpg, *.jpeg)|*.bmp;*.gif;*.jpg;*.jpeg|Все файлы (*" +
                ".*)|*.*";
            // 
            // ImageSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.container);
            this.MinimumSize = new System.Drawing.Size(240, 150);
            this.Name = "ImageSelectControl";
            this.Size = new System.Drawing.Size(246, 217);
            ((System.ComponentModel.ISupportInitialize)(this.container)).EndInit();
            this.container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox container;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraButton btClear;
        private Infragistics.Win.Misc.UltraButton btSelect;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox pbImage;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}
