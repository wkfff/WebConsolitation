using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

using Infragistics.Win.UltraWinGrid.ExcelExport;
using Krista.FM.Client.Reports;
using System.IO;
using Krista.FM.Client.Reports.Common.Commands;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI
{
    public partial class ConsBudgetForecastUI : BaseViewObj
    {
        internal ConsBudgetForecastView vo;

        internal ConsBudgetForecastService consBudgetForecastService;

        internal Dictionary<string , string> regionNames;

        internal DataSet dsData;

        private int variantYear;

        private int nosplitVariant;

        private int nosplitVariantType;
        // вариант, на который записываются данные при расщеплении
        private int splitVariant;
        // источник данных
        private int sourceId;
        // порядковые номера кодов дохода для сортировки
        private Dictionary<string, int> codeIndexes;
        // год, на который получаем данные по указанному источнику
        private int dataYear;

        public ConsBudgetForecastUI(string key)
            : base(key)
        {
            Caption = "Доходы";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new ConsBudgetForecastView();
            fViewCtrl.ViewContent = this;
            nosplitVariant = 0;
        }

        public override void Initialize()
        {
            base.Initialize();

            vo = (ConsBudgetForecastView) fViewCtrl;
            vo.ugData.InitializeLayout += ugData_InitializeLayout;
            vo.tbManager.ToolClick += tbManager_ToolClick;
            vo.ugData.InitializeRow += ugData_InitializeRow;
            vo.ugData.AfterCellUpdate += ugData_AfterCellUpdate;
            vo.ugData.BeforeCellUpdate += ugData_BeforeCellUpdate;
            vo.ugData.AfterCellActivate += ugData_AfterCellActivate;
            vo.ugData.MouseClick += ugData_MouseClick;
            vo.ugData.PreviewKeyDown += ugData_PreviewKeyDown;
            vo.ugData.KeyUp += ugData_KeyUp;

            consBudgetForecastService = new ConsBudgetForecastService(Workplace.ActiveScheme);
            splitVariant = -1;
            vo.tbManager.Tools["RefreshData"].SharedProps.Enabled = false;

            vo.tbManager.Tools["NosplitVariantName"].SharedProps.Caption = "Вариант не выбран";
            vo.tbManager.Tools["SplitVariantName"].SharedProps.Caption = "Вариант для расщепления не выбран";

            UltraToolbar tb = vo.tbManager.Toolbars["Year"];
            ComboBoxTool cb = new ComboBoxTool("Years");
            cb.DropDownStyle = DropDownStyle.DropDownList;
            cb.SharedProps.Width = 150;
            vo.tbManager.Tools.AddRange(new ToolBase[] { cb });
            tb.Tools.AddTool("Years");

            vo.tbManager.AfterToolCloseup += tbManager_AfterToolCloseup;
            vo.ugData.DisplayLayout.GroupByBox.Hidden = true;
            SetEnableOperations();
        }

        void ugData_KeyUp(object sender, KeyEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            switch (e.KeyValue)
            {
                case 37:// стрелка влево
                case 38:// стрелка вверх
                case 39:// стрелка вправо
                case 40:// стрелка вниз
                    grid.PerformAction(UltraGridAction.EnterEditMode);
                    break;
            }
        }

        void ugData_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            if (grid.ActiveCell == null)
                return;
            switch (e.KeyValue)
            {
                case 37:// стрелка влево
                    EmbeddableEditorBase editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    break;
                case 38:// стрелка вверх
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    break;
                case 39:// стрелка вправо
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    break;
                case 40:// стрелка вниз
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    break;
            }
        }

        void tbManager_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
       {
            switch (e.Tool.Key)
            {
                case "Years":
                    int newDataYear = Convert.ToInt32(((ComboBoxTool)e.Tool).Value);
                    if (newDataYear != dataYear)
                    {
                        dataYear = Convert.ToInt32(((ComboBoxTool) e.Tool).Value);
                        if (SplitVariant > 0)
                            SetSplitData(false);
                        else
                            SetNosplitData(variantYear);
                    }
                    break;
            }
        }

        private void FillYears(int year)
        {
            ((ComboBoxTool)vo.tbManager.Tools["Years"]).ValueList.ValueListItems.Clear();
            ((ComboBoxTool)vo.tbManager.Tools["Years"]).ValueList.ValueListItems.Add(year);
            ((ComboBoxTool)vo.tbManager.Tools["Years"]).ValueList.ValueListItems.Add(year + 1);
            ((ComboBoxTool)vo.tbManager.Tools["Years"]).ValueList.ValueListItems.Add(year + 2);
            ((ComboBoxTool)vo.tbManager.Tools["Years"]).SelectedIndex = 0;
        }

        #region свойства

        /// <summary>
        /// вариант, по которому мы получаем нерасщепленные данные из таблицы без расщепления
        /// </summary>
        internal int NosplitVariant
        {
            get { return nosplitVariant; }
            private set { nosplitVariant = value; }
        }

        /// <summary>
        /// вариант, по которому мы получаем и расщепляем данные из таблицы с расщеплением
        /// </summary>
        internal int SplitVariant
        {
            get { return splitVariant; }
            private set { splitVariant = value; }
        }

        /// <summary>
        /// текущий источник данных, зависит от года варианта без расщепления
        /// </summary>
        internal int SourceId
        {
            get { return sourceId; }
            private set { sourceId = value; }
        }

        /// <summary>
        /// год, по которому мы выбираем и расщепляем данные
        /// </summary>
        internal int DataYear
        {
            get { return dataYear; }
            private set { dataYear = value; }
        }

        internal int NosplitVariantType
        {
            get { return nosplitVariantType; }
            private set { nosplitVariantType = value; }
        }

        #endregion

        /// <summary>
        /// изменение года варианта 
        /// </summary>
        /// <param name="newYear"></param>
        private void ChangeYear(int newYear)
        {
            if (variantYear != newYear)
            {
                sourceId = consBudgetForecastService.GetSourceId(newYear);
                codeIndexes = consBudgetForecastService.GetKdCodeIndexes(sourceId);
                variantYear = newYear;
                dataYear = newYear;
                FillYears(newYear);
            }
        }

        private UltraGridRow activeMenuRow;

        void ugData_MouseClick(object sender, MouseEventArgs e)
        {
            // показываем меню для добавления новых записей
            vo.newRowMenu.Items.Clear();
            if (e.Button == MouseButtons.Right)
            {
                object activeCell = GetGridItemFromPos(vo.ugData, e.X, e.Y, typeof (UltraGridCell));
                if (activeCell != null)
                {
                    UltraGridRow activeRow = (UltraGridRow)GetGridItemFromPos(vo.ugData, e.X, e.Y, typeof(UltraGridRow));
                    if (Convert.ToBoolean(activeRow.Cells["ParentRow"].Value))
                        return;
                    ToolStripMenuItem item = new ToolStripMenuItem("Добавить");
                    item.DropDownItemClicked += newRowMenu_ItemClicked;
                    vo.newRowMenu.Items.Add(item);
                    string key = ((UltraGridCell) activeCell).Column.Key;
                    if (string.Compare(key, "Name", true) == 0 || string.Compare(key, "Code", true) == 0 ||
                        string.Compare(key, "IndexStr", true) == 0)
                    {
                        
                        string code = activeRow.Cells["Code"].Value.ToString();
                        DataRow[] rows = dsData.Tables[0].Select(string.Format("Code = '{0}' and Index = 0", code));
                        if (rows.Length == 0)
                        {
                            ToolStripItem toolStripItem = item.DropDownItems.Add("Прогноз");
                            toolStripItem.Tag = 0;
                        }
                        rows = dsData.Tables[0].Select(string.Format("Code = '{0}' and Index = 1", code));
                        if (rows.Length == 0)
                        {
                            ToolStripItem toolStripItem = item.DropDownItems.Add("Реструктуризация");
                            toolStripItem.Tag = 1;
                        }
                        rows = dsData.Tables[0].Select(string.Format("Code = '{0}' and Index = 2", code));
                        if (rows.Length == 0)
                        {
                            ToolStripItem toolStripItem = item.DropDownItems.Add("Недоимка");
                            toolStripItem.Tag = 2;
                        }
                        rows = dsData.Tables[0].Select(string.Format("Code = '{0}' and Index = 3", code));
                        if (rows.Length == 0)
                        {
                            ToolStripItem toolStripItem = item.DropDownItems.Add("Доначисленные");
                            toolStripItem.Tag = 3;
                        }
                    }
                    else
                        return;
                    if (vo.newRowMenu.Items.Count > 0)
                    {
                        activeMenuRow = activeRow;
                        vo.newRowMenu.Show(vo.ugData.PointToScreen(e.Location));
                    }
                }
            }
        }

        void newRowMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string code = activeMenuRow.Cells["Code"].Value.ToString();
            int sortValue = codeIndexes[code] + (int)(ValuesIndex)e.ClickedItem.Tag;
            ValuesIndex index = (ValuesIndex)e.ClickedItem.Tag;
            AddNewRow(index, sortValue, activeMenuRow.Cells["Name"].Value, code,
                activeMenuRow.Cells["RefKD"].Value, activeMenuRow.Cells["kdParentId"].Value, false);
            SortData();
        }

        void ugData_AfterCellActivate(object sender, EventArgs e)
        {
            string activeColumnName = vo.ugData.ActiveCell.Column.Key;
            if (activeColumnName.Contains("Result"))
            {
                vo.ugData.ActiveCell.Activation = Activation.NoEdit;
                return;
            }
        }

        private object oldValue;

        void ugData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            oldValue = e.Cell.Value;
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            string cellKey = e.Cell.Column.Key;
            int id = Convert.ToInt32(e.Cell.Row.Cells["ID"].Value);
            int parentCodeId = -1;
            int.TryParse(e.Cell.Row.Cells["kdParentId"].Value.ToString(), out parentCodeId);
            ValuesIndex index = (ValuesIndex)e.Cell.Row.Cells["Index"].Value;
            string code = e.Cell.Row.Cells["Code"].Value.ToString();
            // настраиваем автовычисление итогов
            UltraGridRow resultRow = UltraGridHelper.FindRow(vo.ugData, new string[] { "Code", "Index" },
                new object[] { code, (int)ValuesIndex.Result }, new Type[] { typeof(string), typeof(int) });
            UltraGridRow masterRow = parentCodeId == 0 ? null :
                UltraGridHelper.FindRow(vo.ugData, new string[] { "RefKD", "Index" },
                new object[] { parentCodeId, index }, new Type[] { typeof(int), typeof(int) });

            DataRow currentRow = GetActiveDataRow(id);

            decimal currentValue;
            decimal originalValue;
            decimal.TryParse(e.Cell.Value.ToString(), out currentValue);
            decimal.TryParse(oldValue.ToString(), out originalValue);
            // вычисляем итоги и вычисляем значения у вышестоящих родительских записей.
            // Не вычисляем итоги у родительских.
            if (index != ValuesIndex.Result)
            {
                // итоги считаем только для расщепленных данных
                if (resultRow != null && (index == ValuesIndex.BK || index == ValuesIndex.RF || index == ValuesIndex.MR ||
                    index == ValuesIndex.DifRF || index == ValuesIndex.DifMR))
                {
                    if (resultRow.Cells[cellKey].Value == DBNull.Value)
                    {
                        resultRow.Cells[cellKey].Value = currentValue;
                    }
                    else
                    {
                        resultRow.Cells[cellKey].Value = Convert.ToDecimal(resultRow.Cells[cellKey].Value) -
                                                         originalValue + currentValue;
                    }
                    resultRow.Update();
                }
                // вычисляем значение в родительской записи
                if (masterRow != null)
                {
                    if (masterRow.Cells[cellKey].Value == DBNull.Value)
                    {
                        masterRow.Cells[cellKey].Value = currentValue;
                    }
                    else
                    {
                        masterRow.Cells[cellKey].Value = Convert.ToDecimal(masterRow.Cells[cellKey].Value) - originalValue +
                                                         currentValue;
                    }
                    masterRow.Update();
                }

                CalculateConsMR(e.Cell.Row, cellKey, index);
            }
            SetResultSum(currentRow);
            BurnChangeDataButtons(true);
        }

        void ugData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (Convert.ToBoolean(e.Row.Cells["IsResultRow"].Value))
            {
                e.Row.Activation = Activation.ActivateOnly;
                e.Row.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            // добавляем дополнительные условия для невозмодности редактирования данных
            ValuesIndex index = (ValuesIndex)e.Row.Cells["Index"].Value;
            if (index == ValuesIndex.Result)
            {
                e.Row.Cells["Code"].Appearance.FontData.Bold = DefaultableBoolean.True;
                e.Row.Cells["Name"].Appearance.FontData.Bold = DefaultableBoolean.True;
                e.Row.Cells["IndexStr"].Appearance.FontData.Bold = DefaultableBoolean.True;
                if (dsData.Tables[0].Select(string.Format("Index = {0} and RefKD = {1}", (int)ValuesIndex.AllNormatives, e.Row.Cells["RefKD"].Value)).Length > 0)
                {
                    e.Row.Cells["IndexStr"].Value = "Сумма поступлений";
                }
            }

            foreach (DataColumn column in dsData.Tables[0].Columns)
            {
                if (index == ValuesIndex.NormBK || index == ValuesIndex.NormDifMR || index == ValuesIndex.NormDifRF || index == ValuesIndex.NormMR || index == ValuesIndex.NormRF)
                {
                    e.Row.Cells[column.ColumnName].Activation = Activation.ActivateOnly;
                    e.Row.Cells[column.ColumnName].Appearance.BackColor = Color.PaleGoldenrod;
                }

                if (index == ValuesIndex.BK || index == ValuesIndex.RF || index == ValuesIndex.MR || index == ValuesIndex.DifRF || index == ValuesIndex.DifRF)
                {
                    e.Row.Cells[column.ColumnName].Activation = Activation.ActivateOnly;
                    e.Row.Cells[column.ColumnName].Appearance.BackColor = Color.LightGoldenrodYellow;
                }
                if ((index == ValuesIndex.Forecast || index == ValuesIndex.Arrears || index == ValuesIndex.Priorcharge || index == ValuesIndex.Restructuring) && 
                    (column.ColumnName.Contains("_5") || column.ColumnName.Contains("_6")))
                {
                    e.Row.Cells[column.ColumnName].Activation = Activation.ActivateOnly;
                    e.Row.Cells[column.ColumnName].Appearance.BackColor = Color.LightGoldenrodYellow;
                }
                if (index == ValuesIndex.Result)
                    e.Row.Cells[column.ColumnName].Appearance.BackColor = Color.Khaki;
            }
        }

        void tbManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key.ToLower())
            {
                case "refresh":
                    // не выбран ни один из вариантов, ничего не делаем
                    if (nosplitVariant <= 0 && splitVariant <= 0)
                        return;
                    SetSplitData(false);
                    break;
                case "clearsplitvariant":
                    splitVariant = -1;
                    vo.tbManager.Tools["SelectSplitVariant"].SharedProps.Caption = "Вариант с расщеплением по уровням бюджета не выбран";
                    break;
                case "savechanges":
                    SaveData();
                    BurnChangeDataButtons(false);
                    BurnSplitDataButton(true);
                    break;
                case "cancelchanges":
                    dsData.RejectChanges();
                    BurnChangeDataButtons(false);
                    break;
                case "refreshdata":
                    if (NosplitVariant <= 0)
                    {
                        MessageBox.Show("Необходимо выбрать вариант проекта доходов в контингенте не выбран", "Получение данных",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //обрабатываем данные таблицы без расщепления, плюс к этому расщепляем данные
                    SetSplitData(true);
                    vo.tbManager.Tools["SelectSplitVariant"].SharedProps.Caption = GetVariantCaption(SplitVariant);
                    BurnSplitDataButton(false);
                    break;
                case "createreport":
                    if (dsData != null)
                    {
                        string reportName = string.Format("Доходы на {0}", DateTime.Now.ToShortDateString());
                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.Title = "Выберите путь для сохранения отчета";
                        dlg.Filter = "Excel документы *.xls|*.xls";
                        dlg.FileName = reportName;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataTable[] dtData = new DataTable[3];
                            dtData[0] = DataTableUtils.FilterDataSet(dsData.Tables[0], "index <> 14");
                            dtData[0] = DataTableUtils.SortDataSet(dtData[0], "id asc");
                            foreach (DataRow drData in dtData[0].Rows)
                            {
                                ValuesIndex index = (ValuesIndex)Convert.ToInt32(drData["Index"]);
                                string newValue = GetIndexStr(index);
                                if (newValue != String.Empty) drData["IndexStr"] = GetIndexStr(index);
                                if (index == ValuesIndex.Result)
                                {
                                    if (dsData.Tables[0].Select(string.Format("Index = 14 and RefKD = {0}", drData["RefKD"])).Length > 0)
                                    {
                                        drData["IndexStr"] = "Сумма поступлений";
                                    }
                                }
                            }
                            dtData[1] = consBudgetForecastService.GetRegions(sourceId);
                            dtData[2] = new DataTable();
                            dtData[2].Columns.Add("ReportYear", typeof(int));
                            dtData[2].Columns.Add("ReportPath", typeof(string));
                            dtData[2].Rows.Add(dataYear, dlg.FileName);
                            var report = new SimpleExcelReport(
                                Workplace.ActiveScheme,
                                "ReportBudgetForecast",
                                reportName);
                            report.SetReportFolder(Path.GetDirectoryName(dlg.FileName));
                            report.CreateReport(dtData);
                        }
                    }
                    break;
                case "addnewcode":
                    // добавление записей по выбранным кодам доходов
                    DataTable dt = new DataTable();
                    if (GetPlaningKDCodes(ref dt))
                        AddCodes(dt);
                    break;
                case "selectnosplitvariant":
                    // выбор варианта для получения данных
                    string variantCaption = string.Empty;
                    int variantType = 0;
                    int year = 0;
                    int newVariant = 0;
                    if (ChooseVariant(ObjectKeys.d_Variant_PlanIncomes, null, ref variantCaption, ref variantType, ref year, ref newVariant))
                    {
                        NosplitVariant = newVariant;
                        NosplitVariantType = variantType;
                        vo.tbManager.Tools["selectNosplitVariant"].SharedProps.Caption = variantCaption;
                        SetNosplitData(year);
                    }
                    break;
                case "selectsplitvariant":
                    //если не выбран вариант для получения данных, ругаемся и ничего не делаем
                    if (nosplitVariant <= 0)
                    {
                        MessageBox.Show("Необходимо выбрать вариант для получения данных", "Получение данных",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    // выбор варианта для расщепления
                    year = 0;
                    variantCaption = string.Empty;
                    newVariant = 0;
                    variantType = 0;
                    if (ChooseVariant(ObjectKeys.d_Variant_PlanIncomes, dataYear, ref variantCaption, ref variantType, ref year, ref newVariant))
                    {
                        SplitVariant = newVariant;
                        vo.tbManager.Tools["SelectSplitVariant"].SharedProps.Caption = variantCaption;
                        SetSplitData(false);
                    }
                    break;
            }
            SetEnableOperations();
        }

        private void SetEnableOperations()
        {
            vo.tbManager.Tools["RefreshData"].SharedProps.Enabled = NosplitVariant > 0;
            vo.tbManager.Tools["AddNewCode"].SharedProps.Enabled = NosplitVariant > 0;
            if (NosplitVariant > 0)
                FillProtocols();
        }

        void ugData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.UseFixedHeaders = true;
            // настраиваем объединение записей по коду и наименованию КД
            e.Layout.Bands[0].Columns["Code"].MergedCellStyle = MergedCellStyle.Always;
            e.Layout.Bands[0].Columns["Code"].MergedCellContentArea = MergedCellContentArea.VirtualRect;
            e.Layout.Bands[0].Columns["Code"].Width = 150;
            e.Layout.Bands[0].Columns["Code"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
            e.Layout.Bands[0].Columns["Name"].MergedCellStyle = MergedCellStyle.Always;
            e.Layout.Bands[0].Columns["Name"].MergedCellContentArea = MergedCellContentArea.VirtualRect;
            e.Layout.Bands[0].Columns["Name"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
            e.Layout.Bands[0].Columns["Name"].MergedCellAppearance.TextVAlign = VAlign.Top;
            e.Layout.Bands[0].Columns["Name"].Width = 350;
            e.Layout.Bands[0].Columns["Name"].CellMultiLine = DefaultableBoolean.False;
            
            e.Layout.Bands[0].Groups.Add("Code", "Классификация доходов");
            e.Layout.Bands[0].Groups.Add("Name", "Наименование доходов");
            e.Layout.Bands[0].Groups.Add("IndexStr", "Показатель");
            e.Layout.Bands[0].Columns["IndexStr"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
            
            e.Layout.Bands[0].Columns["IndexStr"].Width = 150;
            e.Layout.Bands[0].Columns["RefKD"].Hidden = true;
            e.Layout.Bands[0].Columns["Index"].Hidden = true;

            // называем по русски все поля. Плюс добавляем поля, которые будут объединять остальные по признаку района
            foreach (KeyValuePair<string , string> kvp in regionNames)
            {
                e.Layout.Bands[0].Groups.Add(kvp.Key, kvp.Value);
            }
            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.AllowRowFiltering = DefaultableBoolean.False;
                column.SortIndicator = SortIndicator.Disabled; 
                if (column.Key.Contains("_"))
                {
                    column.CellMultiLine = DefaultableBoolean.False;
                    string refRegion = column.Key.Split('_')[0];
                    column.Group = e.Layout.Bands[0].Groups[refRegion];
                    column.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                    column.CellAppearance.TextHAlign = HAlign.Right;
                    column.PadChar = '_';
                    column.MaskInput = "-nnn,nnn,nnn,nnn.n";
                }
                else
                {
                    if (!column.Hidden)
                    {
                        if (e.Layout.Bands[0].Groups.Exists(column.Key))
                            column.Group = e.Layout.Bands[0].Groups[column.Key];
                        else
                            column.Hidden = true;
                        column.CellActivation = Activation.NoEdit;
                    }
                }
                if (column.Key.Contains("Result"))
                {
                    column.CellActivation = Activation.ActivateOnly;
                    column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                }
            }
            e.Layout.Bands[0].Columns["ID"].SortIndicator = SortIndicator.Ascending;

            e.Layout.Bands[0].GroupHeaderLines = 3;
            e.Layout.Bands[0].ColHeaderLines = 3;

            e.Layout.Bands[0].Columns["Code"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Name"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["IndexStr"].Header.Fixed = true;

            e.Layout.Bands[0].Groups["Code"].Header.Fixed = true;
            e.Layout.Bands[0].Groups["Name"].Header.Fixed = true;
            e.Layout.Bands[0].Groups["IndexStr"].Header.Fixed = true;

            e.Layout.Override.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Override.AllowGroupMoving = AllowGroupMoving.NotAllowed;
            e.Layout.GroupByBox.Hidden = true;

            e.Layout.Bands[0].ColumnFilters["Index"].FilterConditions.Add(FilterComparisionOperator.NotEquals,
                                                                          (int) ValuesIndex.AllNormatives);
        }

        public override void InitializeData()
        {
            LoadData();
        }

        internal void LoadData()
        {
            
        }

        private DataRow GetActiveDataRow(int id)
        {
            return dsData.Tables[0].Select(string.Format("ID = {0}", id))[0];
        }

        private void CalculateResults(DataRow currentRow, DataRow resultRow, DataRow parentRow, string columnKey)
        {
            decimal currentValue = 0;
            decimal orginalValue = 0;

            Decimal.TryParse(currentRow[columnKey, DataRowVersion.Original].ToString(), out orginalValue);
            Decimal.TryParse(currentRow[columnKey].ToString(), out currentValue);
            // пересчет локального итога
            if (resultRow.IsNull(columnKey))
            {
                if (currentValue != 0)
                    resultRow[columnKey] = currentValue;
            }
            else
            {
                resultRow[columnKey] = Convert.ToDecimal(resultRow[columnKey]) - orginalValue + currentValue;
            }
            // песесчет итога записи верхнего уровня, если она есть
            if (parentRow == null)
                return;
            if (parentRow.IsNull(columnKey))
            {
                if (currentValue != 0)
                    parentRow[columnKey] = currentValue;
            }
            else
            {
                parentRow[columnKey] = Convert.ToDecimal(resultRow[columnKey]) - orginalValue + currentValue;
            }
        }

        #region работа с данными грида (сохранение, отмена изменений)

        private bool SaveData()
        {
            if (vo.ugData.ActiveRow != null)
                vo.ugData.ActiveRow.Update();
            DataTable dtChanges = dsData.Tables[0].GetChanges();
            // разбираем данные по отдельным записям и сохраняем изменения в базе
            if (dtChanges == null)
                return true;

            string noSplitTableName = Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.f_D_FOPlanInc).FullDBName;
            string splitTableName = Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.f_D_FOPlanIncDivide).FullDBName;

            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                foreach (DataRow row in dtChanges.Rows)
                {
                    bool isResult = Convert.ToBoolean(row["IsResultRow"]);
                    if (isResult)
                        continue;

                    foreach (DataColumn dataColumn in dtChanges.Columns)
                    {
                        if (dataColumn.DataType != typeof(Decimal))
                            continue;

                        if (dataColumn.ColumnName.Contains("Result"))
                            continue;

                        ValuesIndex valueIndex = (ValuesIndex) row["Index"];
                        // итоги для расщепленных данных не зописываем
                        if ((valueIndex == ValuesIndex.BK || valueIndex == ValuesIndex.MR || valueIndex == ValuesIndex.RF ||
                            valueIndex == ValuesIndex.DifMR || valueIndex == ValuesIndex.DifRF) && dataColumn.ColumnName.Contains("_4"))
                            continue;

                        if ((valueIndex == ValuesIndex.Forecast || valueIndex == ValuesIndex.Arrears || valueIndex == ValuesIndex.Priorcharge ||
                            valueIndex == ValuesIndex.Restructuring) && 
                            (dataColumn.ColumnName.Contains("_5") || dataColumn.ColumnName.Contains("_6")))
                            continue;

                        object refRegion = dataColumn.ColumnName.Split('_')[0];

                        object originalValue = row.RowState == DataRowState.Added ? DBNull.Value : row[dataColumn, DataRowVersion.Original];
                        object currentValue = row[dataColumn];
                        decimal original;
                        decimal current;
                        if (Decimal.TryParse(originalValue.ToString(), out original) &&
                            Decimal.TryParse(currentValue.ToString(), out current) && (original != current))
                        {
                            // обновляем запись
                            UpdateRow(db, row, refRegion, noSplitTableName, splitTableName, currentValue);
                        }
                        if (originalValue == DBNull.Value && currentValue != DBNull.Value)
                        {
                            // добавляем запись
                            if (HasRowInTable(row, dataColumn))
                                UpdateRow(db, row, refRegion, noSplitTableName, splitTableName, currentValue);
                            else
                                InsertNewRow(db, row, refRegion, noSplitTableName, splitTableName, currentValue);

                        }
                        if (originalValue != DBNull.Value && currentValue == DBNull.Value)
                        {
                            UpdateRow(db, row, refRegion, noSplitTableName, splitTableName, currentValue);
                            // тоже обновляем запись
                            //DeleteRow(db, refKd, refRegion, index, refVariant, refNewVariant, noSplitData.FullDBName,
                            //          splitData.FullDBName);
                        }
                    }
                    DataRow dataRow =
                        dsData.Tables[0].Select(string.Format("RefKD = {0} and Index = {1}", row["RefKD"], row["Index"]))
                            [0];
                    dataRow.AcceptChanges();
                }
            }
            dsData.AcceptChanges();
            return true;
        }

        private void UpdateRow(IDatabase db, DataRow updateRow, object refRegions,
            string noSplitDataName, string splitDataName, object value)
        {
            decimal saveValue;
            Decimal.TryParse(value.ToString(), out saveValue);
            saveValue = saveValue*1000;

            ValuesIndex index = (ValuesIndex)updateRow["Index"];
            const string insertQuery =
                @"update {0} set {1} = ? where SourceID = ? and RefVariant = ? and RefKD = ? and RefRegions = ? and RefYearDayUNV like ?";
            DbParameterDescriptor[] queryParams = new DbParameterDescriptor[6];
            string tableName = string.Empty;
            queryParams[0] = new DbParameterDescriptor("p0", saveValue);
            queryParams[1] = new DbParameterDescriptor("p1", updateRow["SourceID"]);
            queryParams[2] = new DbParameterDescriptor("p2", updateRow["RefVariant"]);
            queryParams[3] = new DbParameterDescriptor("p3", updateRow["RefKD"]);
            queryParams[4] = new DbParameterDescriptor("p4", refRegions);
            queryParams[5] = new DbParameterDescriptor("p5", string.Format("{0}____", dataYear));
            switch (index)
            {
                case ValuesIndex.Forecast:
                case ValuesIndex.Restructuring:
                case ValuesIndex.Arrears:
                case ValuesIndex.Priorcharge:
                    tableName = noSplitDataName;
                    break;
                case ValuesIndex.BK:
                case ValuesIndex.RF:
                case ValuesIndex.MR:
                case ValuesIndex.DifRF:
                case ValuesIndex.DifMR:
                    index = ValuesIndex.Forecast;
                    tableName = splitDataName;
                    break;
            }
            
            db.ExecQuery(string.Format(insertQuery, tableName, index), QueryResultTypes.NonQuery, queryParams);
        }

        private bool InsertNewRow(IDatabase db, DataRow insertRow, object refRegions, 
            string noSplitDataName, string splitDataName, object value)
        {
            decimal saveValue;
            Decimal.TryParse(value.ToString(), out saveValue);
            if (saveValue == 0)
                return false;
            saveValue = saveValue*1000;

            ValuesIndex index = (ValuesIndex)insertRow["Index"];
            const string insertQuery =
                @"insert into {0} (SourceID, TaskID, {1}, RefVariant, RefKD, RefRegions,
                RefYearDayUNV, RefKVSR, RefFODepartments, RefTaxObjects, RefOrganizations) 
                values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            DbParameterDescriptor[] queryParams = new DbParameterDescriptor[11];
            string tableName = string.Empty;
            queryParams[0] = new DbParameterDescriptor("p0", insertRow["SourceID"]);
            queryParams[1] = new DbParameterDescriptor("p1", -1);
            queryParams[2] = new DbParameterDescriptor("p2", saveValue);
            queryParams[3] = new DbParameterDescriptor("p3", insertRow["RefVariant"]);
            queryParams[4] = new DbParameterDescriptor("p4", insertRow["RefKD"]);
            queryParams[5] = new DbParameterDescriptor("p5", refRegions);
            queryParams[6] = new DbParameterDescriptor("p6", dataYear * 10000 + 1);
            queryParams[7] = new DbParameterDescriptor("p7", -1);
            queryParams[8] = new DbParameterDescriptor("p8", -1);
            queryParams[9] = new DbParameterDescriptor("p9", -1);
            queryParams[10] = new DbParameterDescriptor("p10", -1);
            switch (index)
            {
                case ValuesIndex.Forecast:
                case ValuesIndex.Restructuring:
                case ValuesIndex.Arrears:
                case ValuesIndex.Priorcharge:
                    tableName = noSplitDataName;
                    break;
                case ValuesIndex.BK:
                case ValuesIndex.RF:
                case ValuesIndex.MR:
                case ValuesIndex.DifRF:
                case ValuesIndex.DifMR:
                    index = ValuesIndex.Forecast;
                    tableName = splitDataName;
                    break;
            }
            db.ExecQuery(string.Format(insertQuery, tableName, index), QueryResultTypes.NonQuery, queryParams);
            return true;
        }

        private void DeleteRow(IDatabase db, object refKd, object refRegions, string index, 
            int variant, int newVariant, string noSplitDataName, string splitDataName)
        {
            switch (index)
            {
                case "Forecast":
                case "Restructuring":
                case "Arrears":
                case "Priorcharge":
                    db.ExecQuery(
                        string.Format("delete from {0} where RefVariant = ? and RefKD = ? and RefRegions = ?", noSplitDataName),
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("p0", variant), 
                        new DbParameterDescriptor("p1", refKd),
                        new DbParameterDescriptor("p2", refRegions));
                    break;
                default:
                    db.ExecQuery(
                        string.Format("delete from {0} where RefVariant = ? and RefKD = ? and RefRegions = ?", splitDataName),
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("p0", newVariant),
                        new DbParameterDescriptor("p1", refKd),
                        new DbParameterDescriptor("p2", refRegions));
                    break;
            }
        }

        /// <summary>
        /// проверка, есть ли запись в таблице фактов
        /// </summary>
        /// <param name="row"></param>
        /// <param name="regionColumn"></param>
        /// <returns></returns>
        private bool HasRowInTable(DataRow row, object regionColumn)
        {
            DataRow[] rows = dsData.Tables[0].Select(
                string.Format("RefKD = {0} and RefVariant = {1} and Index <> {2}",
                              row["RefKD"], row["RefVariant"], row["Index"]));
            foreach (DataRow r in rows)
            {
                if (r.RowState == DataRowState.Added)
                    continue;

                if (r[regionColumn.ToString(), DataRowVersion.Original] != DBNull.Value)
                    return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// получение результирующей суммы для строки
        /// </summary>
        /// <param name="row"></param>
        private void SetResultSum(DataRow row)
        {
            decimal mrSum = 0;
            decimal goSum = 0;
            decimal result = 0;
            decimal rSum = 0;
            decimal sSum = 0;

            foreach (DataColumn column in dsData.Tables[0].Columns)
            {
                if (column.ColumnName.Contains("4"))
                {
                    if (!row.IsNull(column))
                        mrSum += Convert.ToDecimal(row[column]);
                }
                if (column.ColumnName.Contains("15"))
                {
                    if (!row.IsNull(column))
                        goSum += Convert.ToDecimal(row[column]);
                }
                if (column.ColumnName.Contains("3"))
                {
                    if (!row.IsNull(column))
                        result += Convert.ToDecimal(row[column]);
                }
                // суммы района
                if (column.ColumnName.Contains("_5"))
                {
                    if (!row.IsNull(column))
                        rSum += Convert.ToDecimal(row[column]);
                }
                // суммы по поселениям
                if (column.ColumnName.Contains("_6"))
                {
                    if (!row.IsNull(column))
                        sSum += Convert.ToDecimal(row[column]);
                }
            }

            row["ResultMR_ResultMR"] = rSum == 0 ? DBNull.Value : (object)rSum;
            row["ResultMR_ResultMRSettlement"] = sSum == 0 ? DBNull.Value : (object)sSum;
            row["ResultMR_ResultMRKB"] = mrSum == 0 ? DBNull.Value : (object)mrSum;
            row["ResultGO_ResultGO"] = goSum == 0 ? DBNull.Value : (object)goSum;
            result = result + goSum + mrSum;
            row["Result_Result"] = result == 0 ? DBNull.Value : (object)result;
        }

        private void CalculateConsMR(UltraGridRow gridRow, string columnName, ValuesIndex index)
        {
            if (!columnName.Contains("_"))
                return;
            string region = columnName.Split('_')[0];
            string budLevel = columnName.Split('_')[1];
            if ((budLevel == "5" || budLevel == "6") && 
                (index == ValuesIndex.BK || index == ValuesIndex.RF || index == ValuesIndex.MR || index == ValuesIndex.DifRF || index == ValuesIndex.DifMR))
            {
                decimal value1 = 0;
                decimal value2 = 0;
                bool isValue1 = Decimal.TryParse(gridRow.Cells[string.Format("{0}_{1}", region, 5)].Value.ToString(), out value1);
                bool isValue2 = Decimal.TryParse(gridRow.Cells[string.Format("{0}_{1}", region, 6)].Value.ToString(), out value2);
                if (isValue1 && isValue2)
                    gridRow.Cells[string.Format("{0}_{1}", region, 4)].Value = value1 + value2;
                else
                    if (isValue1)
                        gridRow.Cells[string.Format("{0}_{1}", region, 4)].Value = value1;
                    else
                        if (isValue2)
                            gridRow.Cells[string.Format("{0}_{1}", region, 4)].Value = value2;
                        else
                            gridRow.Cells[string.Format("{0}_{1}", region, 4)].Value = DBNull.Value;
            }
        }

        private void RefreshData()
        {
            vo.ugData.DataSource = null;
            vo.ugData.DataSource = dsData;
        }

        private void BurnChangeDataButtons(bool burn)
        {
            InfragisticsHelper.BurnTool(vo.tbManager.Tools["SaveChanges"], burn);
            InfragisticsHelper.BurnTool(vo.tbManager.Tools["CancelChanges"], burn);
        }

        private void BurnSplitDataButton(bool burn)
        {
            InfragisticsHelper.BurnTool(vo.tbManager.Tools["RefreshData"], burn);
        }

        /// <summary>
        /// Получить UltraGridRow по экранным координатам. Опрашиваются и потомки.
        /// </summary>
        private object GetGridItemFromPos(UltraGrid grid, int X, int Y, Type type)
        {
            Point pt = new Point(X, Y);
            UIElement elem = grid.DisplayLayout.UIElement.ElementFromPoint(pt);
            return GetRowFromElement(elem, type);
        }

        /// <summary>
        /// Получить UltraGridRow от UIElement. Опрашиваются и потомки.
        /// </summary>
        /// <param name="elem">элемент</param>
        /// <returns>строка</returns>
        private object GetRowFromElement(UIElement elem, Type type)
        {
            object gridObject;
            gridObject = elem.GetContext(type, true);
            return gridObject;
        }

        private void SortData()
        {
            vo.ugData.DisplayLayout.Bands[0].Columns["ID"].SortIndicator = SortIndicator.Ascending;
        }

        private string GetIndexStr(ValuesIndex index)
        {
            switch (index)
            {
                case ValuesIndex.Forecast:
                    return "Прогноз";
                case ValuesIndex.Restructuring:
                    return "Реструктуризация";
                case ValuesIndex.Arrears:
                    return "Недоимка";
                case ValuesIndex.Priorcharge:
                    return "Доначисленные";
                case ValuesIndex.BK:
                    return "Отчисления по БК";
                case ValuesIndex.RF:
                    return "Отчисления по СБ";
                case ValuesIndex.MR:
                    return "Отчисления по МР";
                case ValuesIndex.DifRF:
                    return "Отчисления по Диф.СБ";
                case ValuesIndex.DifMR:
                    return "Отчисления по Диф.МР";
                case ValuesIndex.Result:
                    return "Всего отчисления";
            }
            return string.Empty;
        }

        #region выбор справочника

        private bool GetPlaningKDCodes(ref DataTable dtPlaningKD)
        {
            // получаем нужный классификатор
            IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.d_KD_PlanIncomes);
            // создаем объект просмотра классификаторов нужного типа
            DataClsUI planingKdUI = new PlaningKDUI(entity, variantYear);
            planingKdUI.Workplace = Workplace;
            planingKdUI.RestoreDataSet = false;
            planingKdUI.Initialize();
            planingKdUI.InitModalCls(-1);

            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            modalClsForm.AttachCls(planingKdUI);
            ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
            // ...загружаем данные
            planingKdUI.RefreshAttachedData();
            foreach (UltraGridBand band in planingKdUI.UltraGridExComponent.ugData.DisplayLayout.Bands)
            {
                band.Columns["CodeStr_Remasked"].SortIndicator = SortIndicator.Ascending;
            }

            if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
            {
                if (planingKdUI.UltraGridExComponent.ugData.ActiveRow != null)
                    planingKdUI.UltraGridExComponent.ugData.ActiveRow.Update();
                dtPlaningKD = modalClsForm.AttachedCls.GetClsDataSet().Tables[0];
                return true;
            }
            return false;
        }

        private bool ChooseVariant(string clsKey, object year, ref string variantCaption, ref int variantType, ref int variantYear, ref int variantId)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IClassifier cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.Classifiers[clsKey];
            BaseClsUI clsUI = new DataClsUI(cls);
            if (year != null)
                clsUI.AdditionalFilter = string.Format(" and RefYear = {0}", year);
            clsUI.Workplace = ConsBudgetForecastNavigation.Instance.Workplace;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            clsUI.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUI);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["Code"].SortIndicator =
                SortIndicator.Ascending;
            if (tmpClsForm.ShowDialog() == DialogResult.OK)
            {
                variantCaption = clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["NAME"].Value.ToString();
                variantType = Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["RefVarD"].Value);
                variantYear = Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["RefYear"].Value);
                variantId = Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// наименование варианта проекта доходов по id
        /// </summary>
        /// <param name="variantId"></param>
        /// <returns></returns>
        private string GetVariantCaption(int variantId)
        {
            IEntity variantEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.d_Variant_PlanIncomes);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return db.ExecQuery(string.Format("select Name from {0} where id = ?", variantEntity.FullDBName),
                             QueryResultTypes.Scalar, new DbParameterDescriptor("p0", variantId)).ToString();
            }
        }

        #endregion

        #region добавление новых строк в грид

        private void AddCodes(DataTable dtCodes)
        {
            vo.ugData.BeginUpdate();
            try
            {
                // смотрим те коды, которые у нас были выбраны
                foreach (DataRow selectRow in dtCodes.Select("SelectCode = true"))
                {
                    selectRow["AddRow"] = true;
                    SetParentRows(selectRow);
                }
                foreach (DataRow selectRow in dtCodes.Select("AddRow = true"))
                {
                    AddCodeRows(selectRow);
                }
            }
            finally
            {
                vo.ugData.EndUpdate();
                SortData();
            }
        }

        private void SetParentRows(DataRow row)
        {
            if (row.IsNull("ParentID"))
                return;
            DataRow parentRow = row.Table.Select(string.Format("ID = {0}", row["ParentID"]))[0];
            parentRow["AddRow"] = true;
            parentRow["IsParent"] = true;
            SetParentRows(parentRow);
        }

        /// <summary>
        /// добавляем записи 
        /// </summary>
        /// <param name="codeRow"></param>
        private void AddCodeRows(DataRow codeRow)
        {
            string codeStr = string.IsNullOrEmpty(codeRow["CodeStr"].ToString()) ? codeRow["ID"].ToString() : codeRow["CodeStr"].ToString();
            object refKD = codeRow["ID"];

            // добавляем записи с мерами
            DataRow[] rows = dsData.Tables[0].Select(string.Format("refKD = {0} and Index = {1}", refKD, (int)ValuesIndex.Forecast));
            if (rows.Length == 0)
            {
                int sortValue = codeIndexes[codeStr] + (int) ValuesIndex.Forecast;
                AddNewRow(ValuesIndex.Forecast, sortValue, codeRow["Name"], codeRow["CodeStr"], codeRow["ID"],
                          codeRow["ParentId"], codeRow["IsParent"]);
            }
            rows = dsData.Tables[0].Select(string.Format("refKD = {0} and Index = {1}", refKD, (int)ValuesIndex.Arrears));
            if (rows.Length == 0)
            {
                int sortValue = codeIndexes[codeStr] + (int)ValuesIndex.Arrears;
                AddNewRow(ValuesIndex.Arrears, sortValue, codeRow["Name"], codeRow["CodeStr"], codeRow["ID"],
                          codeRow["ParentId"], codeRow["IsParent"]);
            }
            rows = dsData.Tables[0].Select(string.Format("refKD = {0} and Index = {1}", refKD, (int)ValuesIndex.Priorcharge));
            if (rows.Length == 0)
            {
                int sortValue = codeIndexes[codeStr] + (int)ValuesIndex.Priorcharge;
                AddNewRow(ValuesIndex.Priorcharge, sortValue, codeRow["Name"], codeRow["CodeStr"], codeRow["ID"],
                          codeRow["ParentId"], codeRow["IsParent"]);
            }
            rows = dsData.Tables[0].Select(string.Format("refKD = {0} and Index = {1}", refKD, (int)ValuesIndex.Restructuring));
            if (rows.Length == 0)
            {
                int sortValue = codeIndexes[codeStr] + (int)ValuesIndex.Restructuring;
                AddNewRow(ValuesIndex.Restructuring, sortValue, codeRow["Name"], codeRow["CodeStr"], codeRow["ID"],
                          codeRow["ParentId"], codeRow["IsParent"]);
            }
        }

        /// <summary>
        /// добавление новой записи
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sortValue"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="refKd"></param>
        /// <param name="refParentKd"></param>
        private void AddNewRow(ValuesIndex index, int sortValue, object name, object code, object refKd, object refParentKd, object isParent)
        {
            vo.ugData.EventManager.AllEventsEnabled = false;
            UltraGridRow newRow = vo.ugData.DisplayLayout.Bands[0].AddNew();
            newRow.Cells["ID"].Value = sortValue;
            newRow.Cells["Index"].Value = (int)index;
            newRow.Cells["IndexStr"].Value = GetIndexStr(index);
            newRow.Cells["Code"].Value = code;
            newRow.Cells["Name"].Value = name;
            newRow.Cells["RefKD"].Value = refKd;
            newRow.Cells["RefVariant"].Value = nosplitVariant;
            newRow.Cells["SourceID"].Value = sourceId;
            newRow.Cells["kdParentId"].Value = refParentKd;
            newRow.Cells["IsResultRow"].Value = isParent;
            newRow.Cells["ParentRow"].Value = isParent;
            vo.ugData.EventManager.AllEventsEnabled = true;
            newRow.Update();
        }

        #endregion

        #region загрузка данных из таблиц

        private void SetNosplitData(int year)
        {
            Workplace.OperationObj.Text = "Получение и обработка данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                ChangeYear(year);
                regionNames = consBudgetForecastService.GetRegionNames(sourceId);
                dsData = consBudgetForecastService.GetNosplitData(NosplitVariant, NosplitVariantType, dataYear, sourceId);
                vo.ugData.DataSource = null;
                vo.ugData.DataSource = dsData;
                Workplace.OperationObj.StopOperation();
                BurnChangeDataButtons(false);
                MessageBox.Show(
                    "Получение и обработка данных по консолидированному бюджету успешно завершена",
                    "Консолидированный бюджет",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        private void SetSplitData(bool splitData)
        {
            vo.ugData.DataSource = null;
            Workplace.OperationObj.Text = "Получение и обработка данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                regionNames = consBudgetForecastService.GetRegionNames(SourceId);
                dsData = consBudgetForecastService.GetSplittedData(NosplitVariant, NosplitVariantType, DataYear, SourceId, splitData, ref splitVariant);
                vo.ugData.DataSource = null;
                vo.ugData.DataSource = dsData;
                Workplace.OperationObj.StopOperation(); 
                BurnChangeDataButtons(false);
                MessageBox.Show(
                    "Получение и обработка данных по консолидированному бюджету успешно завершена",
                    "Консолидированный бюджет",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        #endregion
    }
}
