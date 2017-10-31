namespace Krista.FM.Client.MDXExpert
{
    partial class AxesSettingsForm
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
            this.lvAxes = new Infragistics.Win.UltraWinListView.UltraListView();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.lProperty = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.lvAxes)).BeginInit();
            this.SuspendLayout();
            // 
            // lvAxes
            // 
            this.lvAxes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvAxes.ItemSettings.HideSelection = false;
            this.lvAxes.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvAxes.Location = new System.Drawing.Point(12, 22);
            this.lvAxes.Name = "lvAxes";
            this.lvAxes.Size = new System.Drawing.Size(208, 344);
            this.lvAxes.TabIndex = 0;
            this.lvAxes.Text = "lvAxes";
            this.lvAxes.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.lvAxes.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.lvAxes.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvAxes_ItemSelectionChanged);
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(557, 380);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "OK";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(249, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(383, 344);
            this.propertyGrid.TabIndex = 3;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // lProperty
            // 
            this.lProperty.Location = new System.Drawing.Point(249, 6);
            this.lProperty.Name = "lProperty";
            this.lProperty.Size = new System.Drawing.Size(142, 17);
            this.lProperty.TabIndex = 4;
            this.lProperty.Text = "Свойства:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Location = new System.Drawing.Point(12, 6);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(180, 13);
            this.ultraLabel2.TabIndex = 5;
            this.ultraLabel2.Text = "Оси:";
            // 
            // AxesSettingsForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 414);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.lProperty);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.lvAxes);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AxesSettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Оси координат";
            ((System.ComponentModel.ISupportInitialize)(this.lvAxes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView lvAxes;
        private Infragistics.Win.Misc.UltraButton btOK;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private Infragistics.Win.Misc.UltraLabel lProperty;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
    }
}