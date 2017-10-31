namespace Krista.FM.Client.MDXExpert.FieldList
{
    partial class MapPivotDataContainer
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
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint1 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint2 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint3 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint4 = new Infragistics.Win.Layout.GridBagConstraint();
            this.dropZonesContainer = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.gbValuesArea = new Infragistics.Win.Misc.UltraGroupBox();
            this.gbRowsArea = new Infragistics.Win.Misc.UltraGroupBox();
            this.gbColumnsArea = new Infragistics.Win.Misc.UltraGroupBox();
            this.gbFiltersArea = new Infragistics.Win.Misc.UltraGroupBox();
            this.aFilters = new AxisArea(this.components);
            this.aCols = new AxisArea(this.components);
            this.aRows = new AxisArea(this.components);
            this.aTotals = new AxisArea(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dropZonesContainer)).BeginInit();
            this.dropZonesContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbValuesArea)).BeginInit();
            this.gbValuesArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbRowsArea)).BeginInit();
            this.gbRowsArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbColumnsArea)).BeginInit();
            this.gbColumnsArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbFiltersArea)).BeginInit();
            this.gbFiltersArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aFilters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTotals)).BeginInit();
            this.SuspendLayout();
            // 
            // dropZonesContainer
            // 
            this.dropZonesContainer.Controls.Add(this.gbValuesArea);
            this.dropZonesContainer.Controls.Add(this.gbRowsArea);
            this.dropZonesContainer.Controls.Add(this.gbColumnsArea);
            this.dropZonesContainer.Controls.Add(this.gbFiltersArea);
            this.dropZonesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropZonesContainer.ExpandToFitHeight = true;
            this.dropZonesContainer.ExpandToFitWidth = true;
            this.dropZonesContainer.Location = new System.Drawing.Point(0, 0);
            this.dropZonesContainer.Name = "dropZonesContainer";
            this.dropZonesContainer.Padding = new System.Windows.Forms.Padding(5);
            this.dropZonesContainer.Size = new System.Drawing.Size(394, 350);
            this.dropZonesContainer.TabIndex = 19;
            // 
            // gbValuesArea
            // 
            this.gbValuesArea.Controls.Add(this.aTotals);
            gridBagConstraint1.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint1.OriginX = 1;
            gridBagConstraint1.OriginY = 1;
            this.dropZonesContainer.SetGridBagConstraint(this.gbValuesArea, gridBagConstraint1);
            this.gbValuesArea.Location = new System.Drawing.Point(203, 175);
            this.gbValuesArea.Name = "gbValuesArea";
            this.dropZonesContainer.SetPreferredSize(this.gbValuesArea, new System.Drawing.Size(200, 110));
            this.gbValuesArea.Size = new System.Drawing.Size(186, 170);
            this.gbValuesArea.TabIndex = 21;
            this.gbValuesArea.Text = " ";
            this.gbValuesArea.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // gbRowsArea
            // 
            this.gbRowsArea.Controls.Add(this.aRows);
            gridBagConstraint2.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint2.OriginX = 0;
            gridBagConstraint2.OriginY = 1;
            this.dropZonesContainer.SetGridBagConstraint(this.gbRowsArea, gridBagConstraint2);
            this.gbRowsArea.Location = new System.Drawing.Point(5, 175);
            this.gbRowsArea.Name = "gbRowsArea";
            this.dropZonesContainer.SetPreferredSize(this.gbRowsArea, new System.Drawing.Size(200, 110));
            this.gbRowsArea.Size = new System.Drawing.Size(198, 170);
            this.gbRowsArea.TabIndex = 20;
            this.gbRowsArea.Text = " ";
            this.gbRowsArea.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // gbColumnsArea
            // 
            this.gbColumnsArea.Controls.Add(this.aCols);
            gridBagConstraint3.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint3.OriginX = 1;
            gridBagConstraint3.OriginY = 0;
            this.dropZonesContainer.SetGridBagConstraint(this.gbColumnsArea, gridBagConstraint3);
            this.gbColumnsArea.Location = new System.Drawing.Point(203, 5);
            this.gbColumnsArea.Name = "gbColumnsArea";
            this.dropZonesContainer.SetPreferredSize(this.gbColumnsArea, new System.Drawing.Size(200, 110));
            this.gbColumnsArea.Size = new System.Drawing.Size(186, 170);
            this.gbColumnsArea.TabIndex = 19;
            this.gbColumnsArea.Text = " ";
            this.gbColumnsArea.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // gbFiltersArea
            // 
            this.gbFiltersArea.Controls.Add(this.aFilters);
            gridBagConstraint4.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint4.OriginX = 0;
            gridBagConstraint4.OriginY = 0;
            this.dropZonesContainer.SetGridBagConstraint(this.gbFiltersArea, gridBagConstraint4);
            this.gbFiltersArea.Location = new System.Drawing.Point(5, 5);
            this.gbFiltersArea.Name = "gbFiltersArea";
            this.dropZonesContainer.SetPreferredSize(this.gbFiltersArea, new System.Drawing.Size(212, 77));
            this.gbFiltersArea.Size = new System.Drawing.Size(198, 170);
            this.gbFiltersArea.TabIndex = 18;
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
            this.aFilters.ShowRootLines = false;
            this.aFilters.Size = new System.Drawing.Size(192, 151);
            this.aFilters.TabIndex = 0;
            // 
            // aCols
            // 
            this.aCols.AllowDrop = true;
            this.aCols.AxisType = Krista.FM.Client.MDXExpert.Data.AxisType.atColumns;
            this.aCols.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded1;
            this.aCols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aCols.Location = new System.Drawing.Point(3, 16);
            this.aCols.Name = "aCols";
            this.aCols.Size = new System.Drawing.Size(180, 151);
            this.aCols.TabIndex = 0;
            // 
            // aRows
            // 
            this.aRows.AllowDrop = true;
            this.aRows.AxisType = Krista.FM.Client.MDXExpert.Data.AxisType.atRows;
            this.aRows.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded1;
            this.aRows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aRows.Location = new System.Drawing.Point(3, 16);
            this.aRows.Name = "aRows";
            this.aRows.Size = new System.Drawing.Size(192, 151);
            this.aRows.TabIndex = 0;
            // 
            // aTotals
            // 
            this.aTotals.AllowDrop = true;
            this.aTotals.AxisType = Krista.FM.Client.MDXExpert.Data.AxisType.atTotals;
            this.aTotals.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded1;
            this.aTotals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aTotals.Location = new System.Drawing.Point(3, 16);
            this.aTotals.Name = "aTotals";
            this.aTotals.Size = new System.Drawing.Size(180, 151);
            this.aTotals.TabIndex = 0;
            // 
            // TablePivotDataContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dropZonesContainer);
            this.Name = "TablePivotDataContainer";
            this.Size = new System.Drawing.Size(394, 350);
            ((System.ComponentModel.ISupportInitialize)(this.dropZonesContainer)).EndInit();
            this.dropZonesContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbValuesArea)).EndInit();
            this.gbValuesArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbRowsArea)).EndInit();
            this.gbRowsArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbColumnsArea)).EndInit();
            this.gbColumnsArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbFiltersArea)).EndInit();
            this.gbFiltersArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.aFilters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTotals)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGridBagLayoutPanel dropZonesContainer;
        private Infragistics.Win.Misc.UltraGroupBox gbValuesArea;
        private Infragistics.Win.Misc.UltraGroupBox gbRowsArea;
        private Infragistics.Win.Misc.UltraGroupBox gbColumnsArea;
        private Infragistics.Win.Misc.UltraGroupBox gbFiltersArea;
        private AxisArea aCols;
        private AxisArea aFilters;
        private AxisArea aTotals;
        private AxisArea aRows;
    }
}
