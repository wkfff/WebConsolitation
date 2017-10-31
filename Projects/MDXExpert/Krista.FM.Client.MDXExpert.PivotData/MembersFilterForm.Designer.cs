namespace Krista.FM.Client.MDXExpert.Data
{
    partial class MembersFilterForm
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
            this.lvFilters = new Infragistics.Win.UltraWinListView.UltraListView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.btCancel = new Infragistics.Win.Misc.UltraButton();
            this.btDelete = new Infragistics.Win.Misc.UltraButton();
            this.btAdd = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.lvFilters)).BeginInit();
            this.SuspendLayout();
            // 
            // lvFilters
            // 
            this.lvFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvFilters.ItemSettings.HideSelection = false;
            this.lvFilters.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvFilters.Location = new System.Drawing.Point(12, 12);
            this.lvFilters.Name = "lvFilters";
            this.lvFilters.Size = new System.Drawing.Size(224, 341);
            this.lvFilters.TabIndex = 0;
            this.lvFilters.Text = "lvFilters";
            this.lvFilters.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.lvFilters.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.lvFilters.ViewSettingsList.MultiColumn = false;
            this.lvFilters.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvFilters_ItemSelectionChanged);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(258, 12);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(318, 341);
            this.propertyGrid1.TabIndex = 1;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(501, 359);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "OK";
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(420, 359);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "Отмена";
            // 
            // btDelete
            // 
            this.btDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btDelete.Location = new System.Drawing.Point(93, 359);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(75, 23);
            this.btDelete.TabIndex = 4;
            this.btDelete.Text = "Удалить";
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btAdd.Location = new System.Drawing.Point(12, 359);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(75, 23);
            this.btAdd.TabIndex = 5;
            this.btAdd.Text = "Добавить";
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // MembersFilterForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(588, 394);
            this.ControlBox = false;
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.lvFilters);
            this.Name = "MembersFilterForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Фильтры";
            ((System.ComponentModel.ISupportInitialize)(this.lvFilters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView lvFilters;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraButton btCancel;
        private Infragistics.Win.Misc.UltraButton btDelete;
        private Infragistics.Win.Misc.UltraButton btAdd;
    }
}