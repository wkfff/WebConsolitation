namespace Krista.FM.Client.MDXExpert
{
    partial class MapSeriesCollectionEditorForm
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapSeriesCollectionEditorForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbProperties = new Infragistics.Win.Misc.UltraGroupBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.ultraPanel4 = new Infragistics.Win.Misc.UltraPanel();
            this.cbSymbolPropSize = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbFillMap = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbShowPieCharts = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraPanel5 = new Infragistics.Win.Misc.UltraPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gbSerieData = new Infragistics.Win.Misc.UltraGroupBox();
            this.btDataClear = new Infragistics.Win.Misc.UltraButton();
            this.gSerieData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            this.btRefreshAppearance = new Infragistics.Win.Misc.UltraButton();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.gbSeries = new Infragistics.Win.Misc.UltraGroupBox();
            this.tvSeries = new Infragistics.Win.UltraWinTree.UltraTree();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.btSeriesEdit = new Infragistics.Win.Misc.UltraButton();
            this.ultraPanel3 = new Infragistics.Win.Misc.UltraPanel();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbProperties)).BeginInit();
            this.gbProperties.SuspendLayout();
            this.ultraPanel4.ClientArea.SuspendLayout();
            this.ultraPanel4.SuspendLayout();
            this.ultraPanel5.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbSerieData)).BeginInit();
            this.gbSerieData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gSerieData)).BeginInit();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbSeries)).BeginInit();
            this.gbSeries.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tvSeries)).BeginInit();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            this.ultraPanel3.ClientArea.SuspendLayout();
            this.ultraPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.gbProperties);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.ultraPanel3);
            this.panel1.Location = new System.Drawing.Point(12, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 522);
            this.panel1.TabIndex = 13;
            // 
            // gbProperties
            // 
            this.gbProperties.Controls.Add(this.propertyGrid);
            this.gbProperties.Controls.Add(this.ultraPanel4);
            this.gbProperties.Controls.Add(this.ultraPanel5);
            this.gbProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbProperties.Location = new System.Drawing.Point(393, 0);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(375, 492);
            this.gbProperties.TabIndex = 15;
            this.gbProperties.Text = "Свойства";
            this.gbProperties.UseAppStyling = false;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(3, 84);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(369, 405);
            this.propertyGrid.TabIndex = 27;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // ultraPanel4
            // 
            // 
            // ultraPanel4.ClientArea
            // 
            this.ultraPanel4.ClientArea.Controls.Add(this.cbSymbolPropSize);
            this.ultraPanel4.ClientArea.Controls.Add(this.cbFillMap);
            this.ultraPanel4.ClientArea.Controls.Add(this.cbShowPieCharts);
            this.ultraPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel4.Location = new System.Drawing.Point(3, 16);
            this.ultraPanel4.Name = "ultraPanel4";
            this.ultraPanel4.Size = new System.Drawing.Size(369, 68);
            this.ultraPanel4.TabIndex = 26;
            this.ultraPanel4.Text = "ultraPanel4";
            this.ultraPanel4.UseAppStyling = false;
            // 
            // cbSymbolPropSize
            // 
            this.cbSymbolPropSize.Location = new System.Drawing.Point(5, 45);
            this.cbSymbolPropSize.Name = "cbSymbolPropSize";
            this.cbSymbolPropSize.Size = new System.Drawing.Size(361, 20);
            this.cbSymbolPropSize.TabIndex = 16;
            this.cbSymbolPropSize.Text = "Пропорциональный размер значков";
            this.cbSymbolPropSize.CheckedChanged += new System.EventHandler(this.cbSymbolPropSize_CheckedChanged);
            // 
            // cbFillMap
            // 
            this.cbFillMap.Location = new System.Drawing.Point(5, 24);
            this.cbFillMap.Name = "cbFillMap";
            this.cbFillMap.Size = new System.Drawing.Size(215, 20);
            this.cbFillMap.TabIndex = 15;
            this.cbFillMap.Text = "Заливка";
            this.cbFillMap.CheckedChanged += new System.EventHandler(this.cbFillMap_CheckedChanged);
            // 
            // cbShowPieCharts
            // 
            this.cbShowPieCharts.Location = new System.Drawing.Point(5, 3);
            this.cbShowPieCharts.Name = "cbShowPieCharts";
            this.cbShowPieCharts.Size = new System.Drawing.Size(285, 20);
            this.cbShowPieCharts.TabIndex = 14;
            this.cbShowPieCharts.Text = "Данные в виде секторных диаграмм";
            this.cbShowPieCharts.CheckedChanged += new System.EventHandler(this.cbShowPieCharts_CheckedChanged);
            // 
            // ultraPanel5
            // 
            this.ultraPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel5.Location = new System.Drawing.Point(3, 16);
            this.ultraPanel5.Name = "ultraPanel5";
            this.ultraPanel5.Size = new System.Drawing.Size(369, 473);
            this.ultraPanel5.TabIndex = 17;
            this.ultraPanel5.Text = "ultraPanel5";
            this.ultraPanel5.UseAppStyling = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(388, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 492);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.gbSerieData);
            this.panel3.Controls.Add(this.splitter2);
            this.panel3.Controls.Add(this.gbSeries);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(388, 492);
            this.panel3.TabIndex = 14;
            // 
            // gbSerieData
            // 
            this.gbSerieData.Controls.Add(this.btDataClear);
            this.gbSerieData.Controls.Add(this.gSerieData);
            this.gbSerieData.Controls.Add(this.ultraPanel2);
            this.gbSerieData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSerieData.Location = new System.Drawing.Point(0, 164);
            this.gbSerieData.Name = "gbSerieData";
            this.gbSerieData.Size = new System.Drawing.Size(388, 328);
            this.gbSerieData.TabIndex = 22;
            this.gbSerieData.Text = "Данные";
            this.gbSerieData.UseAppStyling = false;
            // 
            // btDataClear
            // 
            this.btDataClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btDataClear.Location = new System.Drawing.Point(287, 299);
            this.btDataClear.Name = "btDataClear";
            this.btDataClear.Size = new System.Drawing.Size(95, 23);
            this.btDataClear.TabIndex = 23;
            this.btDataClear.Text = "Очистить";
            this.btDataClear.Click += new System.EventHandler(this.btDataClear_Click);
            // 
            // gSerieData
            // 
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            this.gSerieData.DisplayLayout.Override.ActiveRowAppearance = appearance3;
            this.gSerieData.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.WithinBand;
            this.gSerieData.DisplayLayout.Override.AllowMultiCellOperations = ((Infragistics.Win.UltraWinGrid.AllowMultiCellOperation)((((((((Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy | Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.CopyWithHeaders)
                        | Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Cut)
                        | Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Delete)
                        | Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Paste)
                        | Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Undo)
                        | Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Redo)
                        | Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Reserved)));
            this.gSerieData.DisplayLayout.Override.AllowRowLayoutColMoving = Infragistics.Win.Layout.GridBagLayoutAllowMoving.AllowAll;
            this.gSerieData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            this.gSerieData.DisplayLayout.Override.RowLayoutCellNavigationVertical = Infragistics.Win.UltraWinGrid.RowLayoutCellNavigation.Adjacent;
            this.gSerieData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.gSerieData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gSerieData.Location = new System.Drawing.Point(3, 16);
            this.gSerieData.Name = "gSerieData";
            this.gSerieData.Size = new System.Drawing.Size(382, 279);
            this.gSerieData.TabIndex = 24;
            this.gSerieData.Error += new Infragistics.Win.UltraWinGrid.ErrorEventHandler(this.gSerieData_Error);
            this.gSerieData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gSerieData_AfterCellUpdate);
            this.gSerieData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gSerieData_MouseClick);
            this.gSerieData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gSerieData_KeyPress);
            this.gSerieData.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.gSerieData_DoubleClickCell);
            // 
            // ultraPanel2
            // 
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.btRefreshAppearance);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraPanel2.Location = new System.Drawing.Point(3, 295);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(382, 30);
            this.ultraPanel2.TabIndex = 24;
            this.ultraPanel2.Text = "ultraPanel2";
            this.ultraPanel2.UseAppStyling = false;
            // 
            // btRefreshAppearance
            // 
            this.btRefreshAppearance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btRefreshAppearance.Location = new System.Drawing.Point(183, 4);
            this.btRefreshAppearance.Name = "btRefreshAppearance";
            this.btRefreshAppearance.Size = new System.Drawing.Size(95, 23);
            this.btRefreshAppearance.TabIndex = 24;
            this.btRefreshAppearance.Text = "Обновить";
            this.btRefreshAppearance.Click += new System.EventHandler(this.btRefreshAppearance_Click);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 161);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(388, 3);
            this.splitter2.TabIndex = 21;
            this.splitter2.TabStop = false;
            // 
            // gbSeries
            // 
            this.gbSeries.Controls.Add(this.tvSeries);
            this.gbSeries.Controls.Add(this.ultraPanel1);
            this.gbSeries.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbSeries.Location = new System.Drawing.Point(0, 0);
            this.gbSeries.Name = "gbSeries";
            this.gbSeries.Size = new System.Drawing.Size(388, 161);
            this.gbSeries.TabIndex = 23;
            this.gbSeries.Text = "Серии";
            this.gbSeries.UseAppStyling = false;
            // 
            // tvSeries
            // 
            appearance2.BorderColor = System.Drawing.Color.Gray;
            this.tvSeries.Appearance = appearance2;
            this.tvSeries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSeries.ImageList = this.imageList;
            this.tvSeries.Location = new System.Drawing.Point(3, 16);
            this.tvSeries.Name = "tvSeries";
            appearance1.ForeColor = System.Drawing.Color.White;
            _override1.SelectedNodeAppearance = appearance1;
            this.tvSeries.Override = _override1;
            this.tvSeries.Size = new System.Drawing.Size(382, 112);
            this.tvSeries.TabIndex = 21;
            this.tvSeries.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvSeries_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "measure.bmp");
            this.imageList.Images.SetKeyName(1, "serie.bmp");
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.btSeriesEdit);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraPanel1.Location = new System.Drawing.Point(3, 128);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(382, 30);
            this.ultraPanel1.TabIndex = 23;
            this.ultraPanel1.Text = "ultraPanel1";
            this.ultraPanel1.UseAppStyling = false;
            // 
            // btSeriesEdit
            // 
            this.btSeriesEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSeriesEdit.Location = new System.Drawing.Point(284, 4);
            this.btSeriesEdit.Name = "btSeriesEdit";
            this.btSeriesEdit.Size = new System.Drawing.Size(95, 23);
            this.btSeriesEdit.TabIndex = 23;
            this.btSeriesEdit.Text = "Редактировать";
            this.btSeriesEdit.Click += new System.EventHandler(this.btSeriesEdit_Click);
            // 
            // ultraPanel3
            // 
            // 
            // ultraPanel3.ClientArea
            // 
            this.ultraPanel3.ClientArea.Controls.Add(this.btOK);
            this.ultraPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraPanel3.Location = new System.Drawing.Point(0, 492);
            this.ultraPanel3.Name = "ultraPanel3";
            this.ultraPanel3.Size = new System.Drawing.Size(768, 30);
            this.ultraPanel3.TabIndex = 16;
            this.ultraPanel3.Text = "ultraPanel3";
            this.ultraPanel3.UseAppStyling = false;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Location = new System.Drawing.Point(670, 4);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(95, 23);
            this.btOK.TabIndex = 24;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // MapSeriesCollectionEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 532);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapSeriesCollectionEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Серии";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbProperties)).EndInit();
            this.gbProperties.ResumeLayout(false);
            this.ultraPanel4.ClientArea.ResumeLayout(false);
            this.ultraPanel4.ResumeLayout(false);
            this.ultraPanel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbSerieData)).EndInit();
            this.gbSerieData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gSerieData)).EndInit();
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbSeries)).EndInit();
            this.gbSeries.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tvSeries)).EndInit();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            this.ultraPanel3.ClientArea.ResumeLayout(false);
            this.ultraPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ImageList imageList;
        private Infragistics.Win.Misc.UltraGroupBox gbSerieData;
        private Infragistics.Win.Misc.UltraButton btDataClear;
        private Infragistics.Win.UltraWinGrid.UltraGrid gSerieData;
        private Infragistics.Win.Misc.UltraGroupBox gbSeries;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraButton btSeriesEdit;
        private Infragistics.Win.UltraWinTree.UltraTree tvSeries;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;
        private Infragistics.Win.Misc.UltraGroupBox gbProperties;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private Infragistics.Win.Misc.UltraPanel ultraPanel4;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbFillMap;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbShowPieCharts;
        private Infragistics.Win.Misc.UltraButton btRefreshAppearance;
        private Infragistics.Win.Misc.UltraPanel ultraPanel3;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraPanel ultraPanel5;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbSymbolPropSize;
    }
}