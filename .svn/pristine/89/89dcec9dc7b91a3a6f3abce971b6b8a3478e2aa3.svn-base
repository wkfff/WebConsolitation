using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    public partial class MapSeriesCollectionEditorForm : Form
    {
        #region Поля

        private MapSerieCollection series;
        private List<SymbolRule> symbolRules;
        bool isUpdating;

        private string serieName;
        private string measureName;

        private MapSeriesDrawFilterClass mapSeriesDrawFilter = new MapSeriesDrawFilterClass();

        private ContextMenu contextMenu;

        #endregion

        #region Свойства

        public string SerieName
        {
            get { return serieName; }
            set
            {
                serieName = value;
                //lSerieName.Text = string.Format("Серия: {0}", value);
            }
        }

        public string MeasureName
        {
            get { return measureName; }
            set
            {
                measureName = value;
               // lMeasureName.Text = string.Format("Мера: {0}", value);
            }
        }

        #endregion 

        public MapSeriesCollectionEditorForm(MapSerieCollection series)
        {
            InitializeComponent();
            mapSeriesDrawFilter.Invalidate += new EventHandler(this.MapSeriesDrawFilter_Invalidate);
            tvSeries.DrawFilter = mapSeriesDrawFilter;
            this.series = series;
            
            ReBindGridKeys();
            CreateContextMenu();

            InitEditor();
        }

        public void InitEditor()
        {
            tvSeries.Nodes.Clear();

            this.symbolRules = series.SymbolRules;
            if (this.series.Element.DataSourceType == DataSourceType.Cube)
            {
                btSeriesEdit.Enabled = false;
                btDataClear.Enabled = false;
                btRefreshAppearance.Enabled = false;
            }
            else
            {
                btSeriesEdit.Enabled = true;
                btDataClear.Enabled = true;
                btRefreshAppearance.Enabled = true;
            }

            foreach (MapSerie serie in series.Items)
            {
                UltraTreeNode serieNode = tvSeries.Nodes.Add(serie.Name);
                serieNode.Tag = serie;
                serieNode.Override.NodeAppearance.Image = 1;
                serieNode.Text = serie.Name;
                foreach(SerieRule serieRule in serie.SerieRules)
                {
                    UltraTreeNode measureNode = serieNode.Nodes.Add(serie.Name + " " + serieRule.Name);
                    measureNode.Tag = serieRule;
                    measureNode.Text = serieRule.Name;
                    measureNode.Override.NodeAppearance.Image = 0;
                }
            }
            tvSeries.ExpandAll();

            isUpdating = true;
            cbSymbolPropSize.Checked = this.series.IsProportionalSymbolSize;
            isUpdating = false;

            if (tvSeries.Nodes.Count > 0)
            {
                isUpdating = true;
                
                if (tvSeries.Nodes[0].Nodes.Count > 0)
                {
                    tvSeries.Nodes[0].Nodes[0].Selected = true;
                }
                else
                {
                    tvSeries.Nodes[0].Selected = true;
                }

                isUpdating = false;

                if ((tvSeries.SelectedNodes.Count > 0) && (tvSeries.SelectedNodes[0].Tag is SerieRule))
                {
                    SetRule(tvSeries.SelectedNodes[0].Parent.Text, tvSeries.SelectedNodes[0].Text, cbFillMap.Checked);
                }
            }
            else
            {
                gSerieData.DataSource = null;
                gbSerieData.Text = "Данные";
                propertyGrid.SelectedObject = null;
            }

        }

        private void SetGridReadOnly()
        {
            foreach (UltraGridColumn column in gSerieData.DisplayLayout.Bands[0].Columns)
            {
                if ((this.series.Element.DataSourceType == DataSourceType.Cube)||
                    (column.Key == Consts.objectsColumn) || (column.Key == Consts.objCodeColumn)
                    || (column.Key == Consts.mapObjectShortName))
                    column.CellActivation = Activation.NoEdit;
            }
        }

        private static void ApplyTemplate(SymbolRule rule, SymbolRuleBrowseAdapter template)
        {
            foreach (PredefinedSymbol symbol in rule.PredefinedSymbols)
            {
                symbol.BorderColor = template.BorderColor;
                symbol.BorderStyle = template.BorderStyle;
                symbol.BorderWidth = template.BorderWidth;
                symbol.Color = template.Color;
                symbol.Font = template.Font;
                symbol.GradientType = template.GradientType;
                symbol.HatchStyle = template.HatchStyle;
                // symbol.Height = template.Height;
               /* symbol.Image = template.Image;
                symbol.ImageResizeMode = template.ImageResizeMode;
                symbol.ImageTransColor = template.ImageTransColor;*/
                symbol.MarkerStyle = template.MarkerStyle;
                symbol.SecondaryColor = template.SecondaryColor;
                symbol.ShadowOffset = template.ShadowOffset;
                symbol.Text = template.TextAppearance.Text;
                symbol.TextColor = template.TextAppearance.Color;
                symbol.TextAlignment = template.TextAppearance.Alignment;
                symbol.TextShadowOffset = template.TextAppearance.ShadowOffset;
                symbol.ToolTip = template.ToolTip;
                //symbol.Width = template.Width;
            }
        }

        private SymbolRule GetSymbolRule(string category, string valueName)
        {
            foreach (SymbolRule rule in this.symbolRules)
            {
                if ((rule.SymbolField == MapHelper.CorrectFieldName(valueName)) && (rule.Category == category))
                {
                    return rule;
                }
            }
            return null;
        }

        private void SetRule(string serieName, string valueName, bool isFillMap)
        {
            if (isUpdating)
            {
                return;
            }

            propertyGrid.SelectedObject = null;

            MapSerie mapSerie = series[serieName];
            if (mapSerie == null)
            {
                return;
            }

            if (valueName == "")
            {
                propertyGrid.SelectedObject = mapSerie.ShowCharts ? new PieChartRuleBrowseAdapter(mapSerie) : null;
                return;
            }

            
            SerieRule serieRule = mapSerie.GetSerieRule(valueName);
            if(serieRule == null)
            {
                return;
            }

            bool isNeedRefresh = serieRule.IsFillMap != isFillMap;
            serieRule.IsFillMap = isFillMap;

            if (mapSerie.ShowCharts)
            {

                if (isFillMap)
                {

                    if (serieRule.RuleAppearance == null)
                    {
                        return;
                    }
                    propertyGrid.SelectedObject = new ShapeRuleBrowseAdapter((ShapeRule) serieRule.RuleAppearance, propertyGrid, series.Element,
                                                   serieRule);
                    return;
                }
                if (isNeedRefresh)
                {
                    this.series.Element.RefreshMapAppearance();
                    mapSerie = series[serieName];
                    serieRule = mapSerie.GetSerieRule(valueName);
                }
                propertyGrid.SelectedObject = new PieRuleBrowseAdapter(mapSerie, serieRule);
                return;
            }

            if (serieRule.RuleAppearance == null)
            {
                return;
            }

            propertyGrid.SelectedObject = isFillMap
                                              ? (object)new ShapeRuleBrowseAdapter((ShapeRule)serieRule.RuleAppearance, propertyGrid, series.Element, serieRule)
                                              :
                                                  new SymbolRuleBrowseAdapter((SymbolRule)serieRule.RuleAppearance, series.Element, serieRule);
        }

        #region грид с данными

        private void SetGridColumnSort()
        {
            if (gSerieData.DisplayLayout.Bands[0].Columns.Count > 0)
            {
                gSerieData.DisplayLayout.Bands[0].Columns[1].SortIndicator = SortIndicator.Ascending;
            }
        }

        private void SetGridTextAlign()
        {
            foreach (UltraGridColumn column  in gSerieData.DisplayLayout.Bands[0].Columns)
            {
                if ((column.Key == Consts.objectsColumn) || (column.Key == Consts.mapObjectShortName))
                    continue;
                column.CellAppearance.TextHAlign = HAlign.Right;
            }
        }

        private void ReBindGridKeys()
        {
            for (int i = 0; i < gSerieData.KeyActionMappings.Count; i++)
            {
                UltraGridAction actionCode = gSerieData.KeyActionMappings[i].ActionCode;
                if ((actionCode == UltraGridAction.BelowCell) ||
                    (actionCode == UltraGridAction.AboveCell) ||
                    (actionCode == UltraGridAction.PrevCell) ||
                    (actionCode == UltraGridAction.NextCell))
                {
                    gSerieData.KeyActionMappings.Remove(gSerieData.KeyActionMappings[i]);
                    i--;
                }
            }
            gSerieData.KeyActionMappings.Add(new GridKeyActionMapping(Keys.Down, UltraGridAction.BelowCell, UltraGridState.LastRowInGrid, UltraGridState.Cell, SpecialKeys.Alt, 0));
            gSerieData.KeyActionMappings.Add(new GridKeyActionMapping(Keys.Enter, UltraGridAction.BelowCell, UltraGridState.LastRowInGrid, UltraGridState.Cell, SpecialKeys.Alt, 0));
            gSerieData.KeyActionMappings.Add(new GridKeyActionMapping(Keys.Up, UltraGridAction.AboveCell, UltraGridState.FirstRowInGrid, UltraGridState.Cell, SpecialKeys.Alt, 0));
            gSerieData.KeyActionMappings.Add(new GridKeyActionMapping(Keys.Left, UltraGridAction.PrevCell, UltraGridState.CellFirst, UltraGridState.Cell, SpecialKeys.Alt, 0));
            gSerieData.KeyActionMappings.Add(new GridKeyActionMapping(Keys.Right, UltraGridAction.NextCell, UltraGridState.CellLast, UltraGridState.Cell, SpecialKeys.Alt, 0));
        }

        private void gSerieData_Error(object sender, ErrorEventArgs e)
        {
            if (e.ErrorType == ErrorType.MultiCellOperation)
            {
                e.Cancel = true;
                if (!e.MultiCellOperationErrorInfo.CanContinueWithRemainingCells)
                    CommonUtils.ProcessException(new Exception(e.ErrorText));
            }
        }

        private void gSerieData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            gSerieData.PerformAction(UltraGridAction.EnterEditMode);
        }

        private void CreateContextMenu()
        {
            this.contextMenu = new ContextMenu();

            MenuItem cutMenuItem = this.contextMenu.MenuItems.Add("Вырезать");
            cutMenuItem.Shortcut = Shortcut.CtrlX;
            cutMenuItem.ShowShortcut = true;
            cutMenuItem.Index = 0;
            cutMenuItem.Click += new EventHandler(cutMenuItem_Click);

            MenuItem copyMenuItem = this.contextMenu.MenuItems.Add("Копировать");
            copyMenuItem.Shortcut = Shortcut.CtrlC;
            copyMenuItem.ShowShortcut = true;
            copyMenuItem.Index = 1;
            copyMenuItem.Click += new EventHandler(copyMenuItem_Click);

            MenuItem pasteMenuItem = this.contextMenu.MenuItems.Add("Вставить");
            pasteMenuItem.Shortcut = Shortcut.CtrlV;
            pasteMenuItem.ShowShortcut = true;
            pasteMenuItem.Index = 2;
            pasteMenuItem.Click += new EventHandler(pasteMenuItem_Click);

            MenuItem delMenuItem = this.contextMenu.MenuItems.Add("Удалить");
            delMenuItem.Shortcut = Shortcut.Del;
            delMenuItem.ShowShortcut = true;
            delMenuItem.Index = 3;
            delMenuItem.Click += new EventHandler(delMenuItem_Click);
        }

        private void cutMenuItem_Click(object sender, EventArgs e)
        {
            gSerieData.PerformAction(UltraGridAction.Cut);
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            gSerieData.PerformAction(UltraGridAction.Copy);
        }

        private void pasteMenuItem_Click(object sender, EventArgs e)
        {
            gSerieData.PerformAction(UltraGridAction.Paste);
        }

        private void delMenuItem_Click(object sender, EventArgs e)
        {
            gSerieData.PerformAction(UltraGridAction.DeleteCells);
        }

        private void gSerieData_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button != System.Windows.Forms.MouseButtons.Right) || (gSerieData.Selected.Cells.Count == 0))
            {
                return;
            }

            this.contextMenu.Show(gSerieData, e.Location);
        }

        private void gSerieData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            this.series.Element.MainForm.Saved = false;
        }

        private void gSerieData_KeyPress(object sender, KeyPressEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;
            if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
            {
                if (char.IsControl(e.KeyChar) == false)
                {
                    grid.PerformAction(UltraGridAction.EnterEditMode);
                    if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                    {
                        EmbeddableEditorBase editor = this.gSerieData.ActiveCell.EditorResolved;
                        if (editor.SupportsSelectableText)
                        {
                            editor.SelectionStart = 0;
                            editor.SelectionLength = editor.TextLength;

                            if (editor is EditorWithMask)
                            {
                                editor.SelectedText = string.Empty;
                            }
                            else
                            {
                                editor.SelectedText = new string(e.KeyChar, 1);
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region Обработчики

        private void cbFillMap_CheckedChanged(object sender, EventArgs e)
        {
            if(isUpdating)
            {
                return;
            }

            if (this.MeasureName != "")
            {
                bool isFillMap = cbFillMap.Checked;

                MapSerie mapSerie = series[serieName];
                mapSerie.Series.Element.Map.ColorSwatchPanel.Title = this.MeasureName;

                SetRule(this.SerieName, this.MeasureName, isFillMap);
                RefreshNodes();
            }
            this.series.Element.RefreshMapAppearance();
            this.series.Element.MainForm.Saved = false;
        }

        private void tvSeries_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections.Count > 0)
                ShowRulePropertiesFromNode(e.NewSelections[0]);
        }

        private void ShowRulePropertiesFromNode(UltraTreeNode node)
        {
            mapSeriesDrawFilter.SelectHightLightNode = node;

            MapSerie serie = null;
            if (tvSeries.SelectedNodes[0].Tag is MapSerie)
            {
                serie = (MapSerie)tvSeries.SelectedNodes[0].Tag;
                this.SerieName = tvSeries.SelectedNodes[0].Text;
                this.MeasureName = "";
                cbFillMap.Enabled = false;
            }
            else
                if (tvSeries.SelectedNodes[0].Tag is SerieRule)
                {
                    SerieRule serieRule = (SerieRule)tvSeries.SelectedNodes[0].Tag;
                    serie = (MapSerie)tvSeries.SelectedNodes[0].Parent.Tag;
                    this.SerieName = tvSeries.SelectedNodes[0].Parent.Text;
                    this.MeasureName = tvSeries.SelectedNodes[0].Text;
                    cbFillMap.Enabled = true;
                    isUpdating = true;
                    cbFillMap.Checked = serieRule.IsFillMap;
                    isUpdating = false;
                }


            gbSerieData.Text = string.Format("Данные серии \"{0}\"", this.SerieName);
            if (serie != null)
            {
                gSerieData.DataSource = serie.Table;
                isUpdating = true;
                cbShowPieCharts.Checked = serie.ShowCharts;
                isUpdating = false;
            }

            SetGridReadOnly();

            SetGridColumnSort();
            SetGridTextAlign();

         //   cbFillMap.Checked = ((series.ShapeRules.Count > 0) && (series.ShapeRules[0].Name == this.SerieName) &&
         //       (series.ShapeRules[0].ShapeField == MapHelper.CorrectFieldName(this.MeasureName)));
         //   cbFillMap.Checked = ((series.Element.Map.ShapeRules.Count > 0) && (series.Element.Map.ShapeRules[0].Category == this.SerieName) &&
         //       (series.ShapeRules[0].ShapeField == MapHelper.CorrectFieldName(this.MeasureName)));

            SetRule(this.SerieName, this.MeasureName, cbFillMap.Checked);

        }

        private void MapSeriesDrawFilter_Invalidate(object sender, System.EventArgs e)
        {
            tvSeries.Invalidate();
        }
       
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!cbFillMap.Checked)
            {
                SymbolRule rule = null;
                if (MeasureName != "")
                {
                    rule = GetSymbolRule(SerieName, this.MeasureName);
                }

                if (rule != null)
                {
                    if(propertyGrid.SelectedObject is SymbolRuleBrowseAdapter)
                    {
                        ApplyTemplate(rule, (SymbolRuleBrowseAdapter) propertyGrid.SelectedObject);
                    }
                }
            }
            propertyGrid.Refresh();
            this.series.Element.RefreshMapAppearance();
            this.series.Element.MainForm.Saved = false;
        }

        private void cbShowPieCharts_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdating)
            {
                return;
            }

            if (cbShowPieCharts.Checked)
            {
                isUpdating = true;
                //cbFillMap.Checked = false;
                isUpdating = false;
            }

            MapSerie mapSerie = series[serieName];
            if (mapSerie == null)
            {
                return;
            }

            mapSerie.ShowCharts = cbShowPieCharts.Checked;

            foreach (SerieRule rule in mapSerie.SerieRules)
            {
                if (!mapSerie.ShowCharts)
                {
                    SetRule(mapSerie.Name, rule.Name, (rule.Name == this.MeasureName) ? cbFillMap.Checked : false);
                }
                else
                {
                    if ((cbFillMap.Checked) && (rule.Name == this.MeasureName))
                    {
                        SetRule(mapSerie.Name, rule.Name, true);
                    }
                }
            }

            series.Element.RefreshMapAppearance();
            RefreshNodes();
            ShowRulePropertiesFromNode(tvSeries.SelectedNodes[0]);
            this.series.Element.MainForm.Saved = false;
        }

        private void RefreshNodes()
        {
            foreach (UltraTreeNode serieNode in tvSeries.Nodes)
            {
                MapSerie serie = series[serieNode.Text];
                serieNode.Tag = serie;
                foreach(UltraTreeNode ruleNode in serieNode.Nodes)
                {
                    ruleNode.Tag = serie.GetSerieRule(ruleNode.Text);
                }
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            series.Element.InitSerieNamesByDataset();
            series.Element.RefreshMapAppearance();
            Close();
        }

        private void btRefreshAppearance_Click(object sender, EventArgs e)
        {
            series.Element.InitSerieNamesByDataset();
            series.Element.RefreshMapAppearance();
        }

        private void btSeriesEdit_Click(object sender, EventArgs e)
        {
            List<string> serieNames = new List<string>();
            foreach(MapSerie serie in this.series.Items)
                serieNames.Add(serie.Name);

            List<string> measureNames = new List<string>();
            if (this.series.Items.Count > 0)
            {
                foreach (SerieRule serieRule in this.series.Items[0].SerieRules)
                    measureNames.Add(serieRule.Name);
            }

            SerieStructureForm ssf = new SerieStructureForm(serieNames, measureNames);
            if (ssf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataSet ds = this.series.Element.SourceDS;
                if (ds == null)
                    ds = new DataSet();

                //List<string> objectNames = this.series.Element.GetObjectNames();
                List<string[]> objectNames = this.series.Element.GetObjectCodesWithNames();


                //удаляем лишние таблицы и столбцы
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (!ssf.SerieNames.Contains(ds.Tables[i].TableName))
                    {
                        ds.Tables.Remove(ds.Tables[i]);
                        i--;
                        continue;
                    }
                    for (int k = 0; k < ds.Tables[i].Columns.Count; k++)
                    {
                        if ((ds.Tables[i].Columns[k].ColumnName == Consts.objCodeColumn)||
                            (ds.Tables[i].Columns[k].ColumnName == Consts.objectsColumn)||
                            (ds.Tables[i].Columns[k].ColumnName == Consts.mapObjectShortName))
                            continue;

                        if (!ssf.MeasureNames.Contains(ds.Tables[i].Columns[k].ColumnName))
                        {
                            ds.Tables[i].Columns.Remove(ds.Tables[i].Columns[k]);
                            k--;
                        }
                    }
                }

                //добавляем новые таблицы и столбцы
                foreach(string serieName in ssf.SerieNames)
                {
                    DataTable table = ds.Tables[serieName];
                    if (table != null)
                    {
                        foreach(string measureName in ssf.MeasureNames)
                        {
                            if (table.Columns.Contains(measureName))
                                continue;

                            table.Columns.Add(measureName, typeof (Double));
                        }

                    }
                    else
                    {
                        ds.Tables.Add(serieName);
                        ds.Tables[serieName].Columns.Add(Consts.objCodeColumn, typeof(String));
                        ds.Tables[serieName].Columns.Add(Consts.objectsColumn, typeof(String));
                        ds.Tables[serieName].Columns.Add(Consts.mapObjectShortName, typeof(String));

                        foreach(string measureName in ssf.MeasureNames)
                            ds.Tables[serieName].Columns.Add(measureName, typeof(Double));

                        for (int j = 0; j < objectNames.Count; j++)
                        {
                            ds.Tables[serieName].Rows.Add();
                            ds.Tables[serieName].Rows[j][Consts.objCodeColumn] = objectNames[j][0];
                            ds.Tables[serieName].Rows[j][Consts.objectsColumn] = objectNames[j][1];
                            ds.Tables[serieName].Rows[j][Consts.mapObjectShortName] = objectNames[j][2];
                        }
                    }
                }
                this.series.Element.AddFields(ssf.MeasureNames);

                this.series.Element.SourceDS = ds;
                this.series.Element.InitMapSerieNames(ssf.SerieNames, this.series.Element.GetObjectNames(), ssf.MeasureNames);
                this.series.Element.RefreshMapAppearance();
                InitEditor();
                this.series.Element.MainForm.Saved = false;
                
            }
        }


        private void btDataClear_Click(object sender, EventArgs e)
        {
            DataSet ds = this.series.Element.SourceDS;
            if (ds == null)
                return;

            foreach(DataTable table in ds.Tables)
                foreach(DataRow row in table.Rows)
                    foreach(DataColumn column in table.Columns)
                    {
                        if ((column.ColumnName == Consts.objCodeColumn)||
                            (column.ColumnName == Consts.objectsColumn)||
                            (column.ColumnName == Consts.mapObjectShortName))
                            continue;

                        row[column] = DBNull.Value;
                    }


            this.series.Element.RefreshMapAppearance();
        }
        
        private void cbSymbolPropSize_CheckedChanged(object sender, EventArgs e)
        {
            if (!isUpdating)
            {
                this.series.IsProportionalSymbolSize = cbSymbolPropSize.Checked;
                this.propertyGrid.Refresh();
                this.series.Element.MainForm.Saved = false;
            }
        }

        #endregion

    }
}