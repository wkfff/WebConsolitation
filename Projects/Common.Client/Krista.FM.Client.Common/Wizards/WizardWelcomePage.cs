using System;
using System.ComponentModel;
using System.Drawing;
using Krista.FM.Common.RegistryUtils;
using Krista.FM.Common.Services;
using Microsoft.Win32;

namespace Krista.FM.Client.Common.Wizards
{
    [ToolboxItem(false)]
    [Designer("Krista.FM.Client.Design.WizardWelcomePageDesigner, Krista.FM.Client.Design, Version=2.4.1.*, Culture=neutral, PublicKeyToken=null")]
    public class WizardWelcomePage : Krista.FM.Client.Common.Wizards.WizardPageBase
    {
        #region Form controls

        private System.Windows.Forms.CheckBox chkDontShow;
        private System.Windows.Forms.Label lblDescription2;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Label lblWizardName;
        private System.Windows.Forms.PictureBox imgSquareBox;
        private System.Windows.Forms.PictureBox imgWatermarkBackground;
        private System.ComponentModel.IContainer components = null;

        #endregion

        #region Class Properties

        protected override System.Drawing.Size DefaultSize
        {
            get { return new Size(498, 328); }
        }

        [Browsable(true)]
        [Category("Wizard Page")]
        [Description("Is checkbox \"Don't show...\" checked.")]
        [DefaultValue(false)]
        public bool DontShow
        {
            get { return chkDontShow.Checked; }
            set
            {
                if (value != chkDontShow.Checked)
                {
                    chkDontShow.Checked = value;
                    OnDontShowChanged();
                    OnChanged();
                }
            }
        }

        [Browsable(true)]
        [Category("Wizard Page")]
        [Description(
            "Gets/Sets wizard page second description Text. This description used only by welocme and final pages")]
        public string Description2
        {
            get
            {
                // if control not intialized yet
                if (lblDescription2 == null) return string.Empty;

                return lblDescription2.Text;
            }
            set
            {
                // skip text set if control not created yet
                if (lblDescription2 == null) return;

                if (value != lblDescription2.Text)
                {
                    lblDescription2.Text = value;
                    OnDescription2Changed();
                }
            }
        }

        #endregion

        #region Class Initialize/Finilize methods

        public WizardWelcomePage()
        {
            InitializeComponent();

            this.Size = new Size(498, 328);
            this.Name = "wizWelcomePage";
            base.WelcomePage = true;

            base.Title = lblWizardName.Text;
            base.Description = lblDescription.Text;
            base.HeaderImage = imgWatermarkBackground.Image;

            this.imgWatermarkBackground.Size = new Size(164, 328);
            this.imgWatermarkBackground.Image = Resources.ru.WatermarkBackground;// ResourceService.GetBitmap("WatermarkBackground");
            this.imgSquareBox.Size = new Size(61, 61);

            lblDescription2.SizeChanged += new EventHandler(OnLabelSizeChanged);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.chkDontShow = new System.Windows.Forms.CheckBox();
			this.lblDescription2 = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblHint = new System.Windows.Forms.Label();
			this.lblWizardName = new System.Windows.Forms.Label();
			this.imgSquareBox = new System.Windows.Forms.PictureBox();
			this.imgWatermarkBackground = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.imgSquareBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgWatermarkBackground)).BeginInit();
			this.SuspendLayout();
			// 
			// chkDontShow
			// 
			this.chkDontShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.chkDontShow.BackColor = System.Drawing.Color.Transparent;
			this.chkDontShow.Location = new System.Drawing.Point(200, 904);
			this.chkDontShow.Name = "chkDontShow";
			this.chkDontShow.Size = new System.Drawing.Size(1150, 17);
			this.chkDontShow.TabIndex = 9;
			this.chkDontShow.Text = "&Не показывать страницу приветствия в следующий раз";
			this.chkDontShow.UseVisualStyleBackColor = false;
			// 
			// lblDescription2
			// 
			this.lblDescription2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblDescription2.BackColor = System.Drawing.Color.Transparent;
			this.lblDescription2.Location = new System.Drawing.Point(174, 160);
			this.lblDescription2.Name = "lblDescription2";
			this.lblDescription2.Size = new System.Drawing.Size(775, 127);
			this.lblDescription2.TabIndex = 7;
			this.lblDescription2.Text = "some more description...";
			// 
			// lblDescription
			// 
			this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblDescription.BackColor = System.Drawing.Color.Transparent;
			this.lblDescription.Location = new System.Drawing.Point(174, 88);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(775, 64);
			this.lblDescription.TabIndex = 6;
			this.lblDescription.Text = "This wizard will guide your throw process of ...";
			// 
			// lblHint
			// 
			this.lblHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblHint.BackColor = System.Drawing.Color.Transparent;
			this.lblHint.Location = new System.Drawing.Point(200, 880);
			this.lblHint.Name = "lblHint";
			this.lblHint.Size = new System.Drawing.Size(1150, 16);
			this.lblHint.TabIndex = 8;
			this.lblHint.Text = "Для продолжения нажмите Далее";
			this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblWizardName
			// 
			this.lblWizardName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblWizardName.BackColor = System.Drawing.Color.Transparent;
			this.lblWizardName.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblWizardName.Location = new System.Drawing.Point(174, 12);
			this.lblWizardName.Name = "lblWizardName";
			this.lblWizardName.Size = new System.Drawing.Size(775, 36);
			this.lblWizardName.TabIndex = 5;
			this.lblWizardName.Text = "Welcome to ...";
			// 
			// imgSquareBox
			// 
			this.imgSquareBox.BackColor = System.Drawing.SystemColors.Control;
			this.imgSquareBox.Image = global::Krista.FM.Client.Common.Properties.Resources.SquareBox;
			this.imgSquareBox.Location = new System.Drawing.Point(86, 16);
			this.imgSquareBox.Name = "imgSquareBox";
			this.imgSquareBox.Size = new System.Drawing.Size(61, 61);
			this.imgSquareBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.imgSquareBox.TabIndex = 15;
			this.imgSquareBox.TabStop = false;
			// 
			// imgWatermarkBackground
			// 
			this.imgWatermarkBackground.Dock = System.Windows.Forms.DockStyle.Left;
			this.imgWatermarkBackground.Location = new System.Drawing.Point(0, 0);
			this.imgWatermarkBackground.Name = "imgWatermarkBackground";
			this.imgWatermarkBackground.Size = new System.Drawing.Size(164, 628);
			this.imgWatermarkBackground.TabIndex = 14;
			this.imgWatermarkBackground.TabStop = false;
			// 
			// WizardWelcomePage
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.imgSquareBox);
			this.Controls.Add(this.imgWatermarkBackground);
			this.Controls.Add(this.chkDontShow);
			this.Controls.Add(this.lblDescription2);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.lblHint);
			this.Controls.Add(this.lblWizardName);
			this.Name = "WizardWelcomePage";
			this.Size = new System.Drawing.Size(968, 628);
			((System.ComponentModel.ISupportInitialize)(this.imgSquareBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgWatermarkBackground)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        #region Class helper methods

        private void OnLabelSizeChanged(object sender, EventArgs e)
        {
            OnResize(e);
        }

        #endregion

        #region Class Overrides

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            
            if (Description2 != null && Description2.Length > 0)
            {
                Bitmap bmp = new Bitmap(lblDescription2.Width, lblDescription2.Height);
                Graphics g = Graphics.FromImage(bmp);
                //this.CreateGraphics();

                StringFormat format = new StringFormat();
                format.Trimming = StringTrimming.Word;

                SizeF size = g.MeasureString(Description2, lblDescription2.Font, lblDescription2.Width, format);

                lblDescription2.Height = (int) size.Height + 16;
                lblHint.Top = lblDescription2.Bottom + 8;

                g.Dispose();
                bmp.Dispose();
            }
        }

        protected override void OnImageListChanged()
        {
            if (base.ImageList != null)
            {
                if (base.ImageIndex >= 0 && ImageIndex < base.ImageList.Images.Count)
                {
                    imgSquareBox.Image = base.ImageList.Images[base.ImageIndex];
                }
            }
        }

        protected override void OnImageIndexChanged()
        {
            if (base.ImageList != null)
            {
                if (base.ImageIndex >= 0 && ImageIndex < base.ImageList.Images.Count)
                {
                    imgSquareBox.Image = base.ImageList.Images[base.ImageIndex];
                }
            }
        }

        protected override void OnHeaderImageChanged()
        {
            //!!!imgWatermarkBackground.Image = base.HeaderImage;
        }

        protected override void OnTitleChanged()
        {
            lblWizardName.Text = base.Title;
        }

        protected override void OnDescriptionChanged()
        {
            lblDescription.Text = base.Description;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            LoadSettings();
            OnImageListChanged();
            OnHeaderImageChanged();
            base.OnLoad(e);
        }

        protected virtual void OnDescription2Changed()
        {
            OnResize(EventArgs.Empty);
        }

        protected virtual void OnDontShowChanged()
        {
            SaveSettings();
        }

        /// <summary>
        /// Сохраняем настройки в реестре.
        /// </summary>
        private void SaveSettings()
        {
            object form = this.ParentForm;
            if (form != null)
            {
                RegistryKey rk = Utils.BuildRegistryKey(Registry.CurrentUser, form.GetType().FullName);
                rk.SetValue("DontShowWizardWelcomePage", chkDontShow.Checked);
            }
        }

        /// <summary>
        /// Считываем настройки из реестра.
        /// </summary>
        private void LoadSettings()
        {
            object form = this.ParentForm;
            if (form != null)
            {
                RegistryKey rk = Utils.BuildRegistryKey(Registry.CurrentUser, form.GetType().FullName);

                if (rk.ValueCount == 0)
                    return;

                DontShow = Convert.ToBoolean(rk.GetValue("DontShowWizardWelcomePage"));
            }
        }

        #endregion
    }
}