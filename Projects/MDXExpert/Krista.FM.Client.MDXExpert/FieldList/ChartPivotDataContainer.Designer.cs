namespace Krista.FM.Client.MDXExpert.FieldList
{
    partial class ChartPivotDataContainer
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
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint2 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinTree.Override _override2 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint3 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinTree.Override _override3 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint1 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            this.dropZonesContainer = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.gbRowsArea = new Infragistics.Win.Misc.UltraGroupBox();
            this.aRows = new AxisArea(this.components);
            this.gbColumnsArea = new Infragistics.Win.Misc.UltraGroupBox();
            this.aCols = new AxisArea(this.components);
            this.gbFiltersArea = new Infragistics.Win.Misc.UltraGroupBox();
            this.aFilters = new AxisArea(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dropZonesContainer)).BeginInit();
            this.dropZonesContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbRowsArea)).BeginInit();
            this.gbRowsArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbColumnsArea)).BeginInit();
            this.gbColumnsArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFiltersArea)).BeginInit();
            this.gbFiltersArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aFilters)).BeginInit();
            this.SuspendLayout();
            // 
            // dropZonesContainer
            // 
            this.dropZonesContainer.Controls.Add(this.gbFiltersArea);
            this.dropZonesContainer.Controls.Add(this.gbRowsArea);
            this.dropZonesContainer.Controls.Add(this.gbColumnsArea);
            this.dropZonesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropZonesContainer.ExpandToFitHeight = true;
            this.dropZonesContainer.ExpandToFitWidth = true;
            this.dropZonesContainer.Location = new System.Drawing.Point(0, 0);
            this.dropZonesContainer.Name = "dropZonesContainer";
            this.dropZonesContainer.Padding = new System.Windows.Forms.Padding(5);
            this.dropZonesContainer.Size = new System.Drawing.Size(352, 360);
            this.dropZonesContainer.TabIndex = 20;
            // 
            // gbRowsArea
            // 
            this.gbRowsArea.Controls.Add(this.aRows);
            gridBagConstraint2.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint2.OriginX = 0;
            gridBagConstraint2.OriginY = 2;
            this.dropZonesContainer.SetGridBagConstraint(this.gbRowsArea, gridBagConstraint2);
            this.gbRowsArea.Location = new System.Drawing.Point(5, 238);
            this.gbRowsArea.Name = "gbRowsArea";
            this.dropZonesContainer.SetPreferredSize(this.gbRowsArea, new System.Drawing.Size(181, 104));
            this.gbRowsArea.Size = new System.Drawing.Size(342, 117);
            this.gbRowsArea.TabIndex = 21;
            this.gbRowsArea.Text = " ";
            this.gbRowsArea.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // aRows
            // 
            this.aRows.AllowDrop = true;
            this.aRows.AxisType = Krista.FM.Client.MDXExpert.Data.AxisType.atRows;
            this.aRows.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded1;
            this.aRows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aRows.Location = new System.Drawing.Point(3, 16);
            this.aRows.Name = "aRows";
            _override2.NodeDoubleClickAction = Infragistics.Win.UltraWinTree.NodeDoubleClickAction.None;
            _override2.SelectionType = Infragistics.Win.UltraWinTree.SelectType.SingleAutoDrag;
            this.aRows.Override = _override2;
            this.aRows.ShowRootLines = false;
            this.aRows.Size = new System.Drawing.Size(336, 98);
            this.aRows.TabIndex = 0;
            // 
            // gbColumnsArea
            // 
            this.gbColumnsArea.Controls.Add(this.aCols);
            gridBagConstraint3.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint3.OriginX = 0;
            gridBagConstraint3.OriginY = 1;
            this.dropZonesContainer.SetGridBagConstraint(this.gbColumnsArea, gridBagConstraint3);
            this.gbColumnsArea.Location = new System.Drawing.Point(5, 122);
            this.gbColumnsArea.Name = "gbColumnsArea";
            this.dropZonesContainer.SetPreferredSize(this.gbColumnsArea, new System.Drawing.Size(171, 104));
            this.gbColumnsArea.Size = new System.Drawing.Size(342, 116);
            this.gbColumnsArea.TabIndex = 19;
            this.gbColumnsArea.Text = " ";
            this.gbColumnsArea.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // aCols
            // 
            this.aCols.AllowDrop = true;
            this.aCols.AxisType = Krista.FM.Client.MDXExpert.Data.AxisType.atColumns;
            this.aCols.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded1;
            this.aCols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aCols.Location = new System.Drawing.Point(3, 16);
            this.aCols.Name = "aCols";
            _override3.NodeDoubleClickAction = Infragistics.Win.UltraWinTree.NodeDoubleClickAction.None;
            _override3.SelectionType = Infragistics.Win.UltraWinTree.SelectType.SingleAutoDrag;
            this.aCols.Override = _override3;
            this.aCols.ShowRootLines = false;
            this.aCols.Size = new System.Drawing.Size(336, 97);
            this.aCols.TabIndex = 0;
            // 
            // gbFiltersArea
            // 
            this.gbFiltersArea.Controls.Add(this.aFilters);
            gridBagConstraint1.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint1.OriginX = 0;
            gridBagConstraint1.OriginY = 0;
            this.dropZonesContainer.SetGridBagConstraint(this.gbFiltersArea, gridBagConstraint1);
            this.gbFiltersArea.Location = new System.Drawing.Point(5, 5);
            this.gbFiltersArea.Name = "gbFiltersArea";
            this.dropZonesContainer.SetPreferredSize(this.gbFiltersArea, new System.Drawing.Size(181, 105));
            this.gbFiltersArea.Size = new System.Drawing.Size(342, 117);
            this.gbFiltersArea.TabIndex = 22;
            this.gbFiltersArea.Text = " ";
            this.gbFiltersArea.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // aFilters
            // 
            this.aFilters.AllowDrop = true;
            this.aFilters.AxisType = Krista.FM.Client.MDXExpert.Data.AxisType.atFilters;
            this.aFilters.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded1;
            this.aFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aFilters.Location = new System.Drawing.Point(3, 16);
            this.aFilters.Name = "aFilters";
            _override1.NodeDoubleClickAction = Infragistics.Win.UltraWinTree.NodeDoubleClickAction.None;
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.SingleAutoDrag;
            this.aFilters.Override = _override1;
            this.aFilters.ShowRootLines = false;
            this.aFilters.Size = new System.Drawing.Size(336, 98);
            this.aFilters.TabIndex = 1;
            // 
            // ChartPivotDataContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dropZonesContainer);
            this.Name = "ChartPivotDataContainer";
            this.Size = new System.Drawing.Size(352, 360);
            ((System.ComponentModel.ISupportInitialize)(this.dropZonesContainer)).EndInit();
            this.dropZonesContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbRowsArea)).EndInit();
            this.gbRowsArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.aRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbColumnsArea)).EndInit();
            this.gbColumnsArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.aCols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFiltersArea)).EndInit();
            this.gbFiltersArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.aFilters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGridBagLayoutPanel dropZonesContainer;
        private Infragistics.Win.Misc.UltraGroupBox gbRowsArea;
        private Infragistics.Win.Misc.UltraGroupBox gbColumnsArea;
        private AxisArea aRows;
        private AxisArea aCols;
        private Infragistics.Win.Misc.UltraGroupBox gbFiltersArea;
        private AxisArea aFilters;
    }
}
