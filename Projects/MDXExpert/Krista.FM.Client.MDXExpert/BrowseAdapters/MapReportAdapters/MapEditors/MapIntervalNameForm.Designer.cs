namespace Krista.FM.Client.MDXExpert
{
    partial class MapIntervalNameForm
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
            this.lvIntervals = new Infragistics.Win.UltraWinListView.UltraListView();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.lvIntervals)).BeginInit();
            this.SuspendLayout();
            // 
            // lvIntervals
            // 
            this.lvIntervals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvIntervals.ItemSettings.HideSelection = false;
            this.lvIntervals.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvIntervals.Location = new System.Drawing.Point(12, 25);
            this.lvIntervals.Name = "lvIntervals";
            this.lvIntervals.Size = new System.Drawing.Size(217, 229);
            this.lvIntervals.TabIndex = 0;
            this.lvIntervals.Text = "lvIntervals";
            this.lvIntervals.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.lvIntervals.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.lvIntervals.ViewSettingsList.MultiColumn = false;
            this.lvIntervals.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvIntervals_ItemSelectionChanged);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(247, 25);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(229, 229);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(402, 267);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(15, 8);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(66, 14);
            this.ultraLabel1.TabIndex = 3;
            this.ultraLabel1.Text = "Интервалы:";
            // 
            // MapIntervalNameForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 302);
            this.ControlBox = false;
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.lvIntervals);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapIntervalNameForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Имена интервалов";
            ((System.ComponentModel.ISupportInitialize)(this.lvIntervals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView lvIntervals;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
    }
}