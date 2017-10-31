using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
    public partial class DisintRulesUI : BaseViewObj
    {
        // текущие нормативы, с которыми работаем
        internal NormativesKind currentNormatives = NormativesKind.Unknown;

        private DataTable dtNormatives;

        private const string VALUE_POSTFIX = "_Value";
        private const string RESULT_POSTFIX = "_Result";
        private const string RESULT_VALUE_POSTFIX = "_ResultValue";
        private const string REF_VALUE_POSTFIX = "_RefValue";

        private const string valueMask = "nnn.nn";
        private const string negativeValueMask = "-nnn.nn";

        private IEntity normativeObj = null;

        #region свойства, связанные с тултипами

        private UltraGridCell _lastCellUnderMouse = null;

        private Timer _toolTipTimer = null;

        public Timer ToolTipTimer
        {
            get
            {
                if (_toolTipTimer == null)
                {
                    _toolTipTimer = new Timer();
                    _toolTipTimer.Interval = 500;
                    _toolTipTimer.Tick += new EventHandler(toolTipTimer_Tick);
                }
                return _toolTipTimer;
            }
        }

        private Infragistics.Win.ToolTip toolTipValue = null;

        public Infragistics.Win.ToolTip ToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new Infragistics.Win.ToolTip(drv);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        #endregion


        private ColumnFilter yearFilter;

        private bool TrySaveNormatives()
        {
            if (currentNormatives != NormativesKind.AllNormatives)
            {
                if (dtNormatives.GetChanges() != null)
                {
                    if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        newGrid_OnSaveChanges(drv.newGrid);
                        return true;
                    }

                    newGrid_OnCancelChanges(drv.newGrid);
                }
            }
            return false;
        }

        /// <summary>
        /// загрузка нового норматива
        /// </summary>
        internal void LoadData()
        {
            switch (moduleName)
            {
                case NormativesKeys.NormativesBK:
                    currentNormatives = NormativesKind.NormativesBK;
                    normativeObj = Workplace.ActiveScheme.FactTables[GuidConsts.f_Norm_BK];
                    break;
                case NormativesKeys.NormativesRegionRF:
                    currentNormatives = NormativesKind.NormativesRegionRF;
                    normativeObj = Workplace.ActiveScheme.FactTables[GuidConsts.f_Norm_Region];
                    break;
                case NormativesKeys.NormativesMR:
                    currentNormatives = NormativesKind.NormativesMR;
                    normativeObj = Workplace.ActiveScheme.FactTables[GuidConsts.f_Norm_MR];
                    break;
                case NormativesKeys.VarNormativesMR:
                    currentNormatives = NormativesKind.VarNormativesMR;
                    normativeObj = Workplace.ActiveScheme.FactTables[GuidConsts.f_Norm_VariedMR];
                    break;
                case NormativesKeys.VarNormativesRegionRF:
                    currentNormatives = NormativesKind.VarNormativesRegionRF;
                    normativeObj = Workplace.ActiveScheme.FactTables[GuidConsts.f_Norm_VariedRegion];
                    break;
                case NormativesKeys.AllNormatives:
                    currentNormatives = NormativesKind.AllNormatives;
                    drv.sp.Panel2Collapsed = true;
                    drv.sp.Panel1Collapsed = false;
                    return;
            }

            string workplaceCaption = GetNormativeRusName(currentNormatives);

            if (currentNormatives == NormativesKind.AllNormatives)
            {
                drv.sp.Panel2Collapsed = true;
                drv.sp.Panel1Collapsed = false;
            }
            else
            {
                drv.sp.Panel2Collapsed = false;
                drv.sp.Panel1Collapsed = true;
            }

            ViewCtrl.Text = workplaceCaption;
            drv.newGrid.SaveLoadFileName = workplaceCaption;
            Workplace.OperationObj.Text = "Получение данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                dtNormatives = disintRulesModule.GetNormatives(currentNormatives);
                drv.newGrid.DataSource = dtNormatives;
                if (yearFilter != null)
                {
                    foreach (FilterCondition o in yearFilter.FilterConditions)
                    {
                        drv.newGrid._ugData.DisplayLayout.Bands[0].ColumnFilters["RefYearDayUNV_Lookup"].FilterConditions.Add(o);
                    }
                }
                if (drv.newGrid.ugData.Rows.Count > 0)
                    drv.newGrid.ugData.Rows[0].Activate();

                if (drn.refreshNormatives.Contains(drn.ultraExplorerBar1.CheckedItem.Key))
                {
                    drn.refreshNormatives.Remove(drn.ultraExplorerBar1.CheckedItem.Key);
                    drv.newGrid.BurnRefreshDataButton(false);
                }

            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        private void UpdateToolBar1()
        {
            UltraToolbar tb = drv.newGrid.utmMain.Toolbars["utbColumns"];

            ButtonTool btnCalculateNormative = null;
            if (!drv.newGrid.utmMain.Tools.Exists("btnCalculateNormative"))
            {
                btnCalculateNormative = new ButtonTool("btnCalculateNormative");
                btnCalculateNormative.SharedProps.ToolTipText = "Рассчитать нормативы отчислений";
                btnCalculateNormative.SharedProps.AppearancesSmall.Appearance.Image = drv.ilImages.Images[4];
                drv.newGrid.utmMain.Tools.Add(btnCalculateNormative);
                tb.Tools.AddTool("btnCalculateNormative");
            }
            else
                btnCalculateNormative = (ButtonTool)drv.newGrid.utmMain.Tools["btnCalculateNormative"];
            btnCalculateNormative.SharedProps.Visible = currentNormatives != NormativesKind.AllNormatives;

            // кнопка с проверкой нормативов
            ButtonTool btnCheckDisinRules = null;
            if (!drv.newGrid.utmMain.Tools.Exists("btnCheckDisinRules"))
            {
                btnCheckDisinRules = new ButtonTool("btnCheckDisinRules");
                btnCheckDisinRules.SharedProps.ToolTipText = "Выполнить проверку нормативов отчислений";
                btnCheckDisinRules.SharedProps.AppearancesSmall.Appearance.Image = drv.ilImages.Images[3];
                drv.newGrid.utmMain.Tools.Add(btnCheckDisinRules);
                tb.Tools.AddTool("btnCheckDisinRules");
                btnCheckDisinRules.SharedProps.Visible = true;
            }
            else
                btnCheckDisinRules = (ButtonTool)drv.newGrid.utmMain.Tools["btnCheckDisinRules"];

            drv.newGrid.utmMain.Tools["CopyRow"].SharedProps.Visible = true;
            drv.newGrid.utmMain.Tools["PasteRow"].SharedProps.Visible = true;

            if (currentNormatives == NormativesKind.NormativesBK)
            {
                ButtonTool btnCopyNorm = null;
                if (!drv.newGrid.utmMain.Tools.Exists("btnCopyNorm"))
                {
                    btnCopyNorm = new ButtonTool("btnCopyNorm");
                    btnCopyNorm.SharedProps.ToolTipText = "Перенос нормативов за год";
                    btnCopyNorm.SharedProps.AppearancesSmall.Appearance.Image = drv.ilImages.Images[8];
                    drv.newGrid.utmMain.Tools.Add(btnCopyNorm);
                    tb.Tools.AddTool("btnCopyNorm");
                    btnCopyNorm.SharedProps.Visible = true;
                }
                else
                    btnCopyNorm = (ButtonTool) drv.newGrid.utmMain.Tools["btnCopyNorm"];
            }
        }

        /// <summary>
        /// настройки для грида
        /// </summary>
        private void SetupNewRulesGrid()
        {
            if (drn == null)
                drn = DisintRulesNavigation.Instance;

            drv.newGrid.OnSaveChanges += new SaveChanges(newGrid_OnSaveChanges);
            drv.newGrid.OnCancelChanges += new DataWorking(newGrid_OnCancelChanges);
            drv.newGrid.OnClearCurrentTable += new DataWorking(newGrid_OnClearCurrentTable);
            drv.newGrid.OnGetGridColumnsState += new GetGridColumnsState(newGrid_OnGetGridColumnsState);
            drv.newGrid.OnGridInitializeLayout += new GridInitializeLayout(newGrid_OnGridInitializeLayout);
            drv.newGrid.OnClickCellButton += new ClickCellButton(newGrid_OnClickCellButton);
            drv.newGrid.OnAfterRowInsert += new AfterRowInsert(newGrid_OnAfterRowInsert);
            drv.newGrid.OnInitializeRow += new InitializeRow(newGrid_OnInitializeRow);
            drv.newGrid.OnBeforeRowDeactivate += new BeforeRowDeactivate(newGrid_OnBeforeRowDeactivate);
            drv.newGrid.OnBeforeCellDeactivate += new BeforeCellDeactivate(newGrid_OnBeforeCellDeactivate);
            drv.newGrid.OnRefreshData += new RefreshData(newGrid_OnRefreshData);
            drv.newGrid.OnSaveToXML += new SaveLoadXML(newGrid_OnSaveToXML);
            drv.newGrid.OnLoadFromXML += new SaveLoadXML(newGrid_OnLoadFromXML);
            drv.newGrid.OnLoadFromExcel += new SaveLoadXML(newGrid_OnLoadFromExcel);
            drv.newGrid.OnSaveToExcel += new SaveLoadXML(newGrid_OnSaveToExcel);
            drv.newGrid.OnAftertImportFromXML += new AftertImportFromXML(newGrid_OnAftertImportFromXML);
            // лукапы
            drv.newGrid.OnGetLookupValue += new GetLookupValueDelegate(newGrid_OnGetLookupValue);
            drv.newGrid.OnCheckLookupValue += new CheckLookupValueDelegate(newGrid_OnCheckLookupValue);
            drv.newGrid.OnMouseEnterGridElement += new MouseEnterElement(newGrid_OnMouseEnterGridElement);
            drv.newGrid.OnMouseLeaveGridElement += new MouseLeaveElement(newGrid_OnMouseLeaveGridElement);

            drv.newGrid.ToolClick += new ToolBarToolsClick(newGrid_ToolClick);

            drv.newGrid.OnCopyRow += new RefreshData(newGrid_OnCopyRow);
            drv.newGrid.OnPasteRow += new RefreshData(newGrid_OnPasteRow);

            FillCash();
            drv.newGrid.StateRowEnable = true;

            LoadData();

            UpdateToolBar1();
            
        }

        public bool GetPeriod(string[] columnValues, ref object[] values)
        {
            // получаем нужный классификатор
            IEntity periodEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(fx_Date_YearDayUNV);
            PeriodUI periodUI = new PeriodUI(periodEntity);
            periodUI.Workplace = Workplace;
            periodUI.RestoreDataSet = false;
            periodUI.Initialize();
            periodUI.InitModalCls(-1);

            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            modalClsForm.AttachCls(periodUI);
            ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
            // ...загружаем данные
            periodUI.RefreshAttachedData();

            if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
            {
                int clsID = modalClsForm.AttachedCls.GetSelectedID();
                modalClsForm.AttachedCls.GetColumnsValues(columnValues, ref values);
                // если ничего не выбрали - считаем что функция завершилась неудачно
                if (clsID == -10)
                    return false;
                return true;
            }
            return false;
        }


        private List<Dictionary<string, object>> copyRows = new List<Dictionary<string, object>>();

        bool newGrid_OnCopyRow(object sender)
        {
            copyRows.Clear();
            if (drv.newGrid.ugData.Selected.Rows.Count == 0 && drv.newGrid.ugData.ActiveRow != null)
                drv.newGrid.ugData.ActiveRow.Selected = true;
            foreach (UltraGridRow row in drv.newGrid.ugData.Selected.Rows)
            {
                Dictionary<string, object> copyRow = new Dictionary<string, object>();
                foreach (UltraGridColumn column in drv.newGrid.ugData.DisplayLayout.Bands[0].Columns)
                {
                    if (column.Key != "ID")
                        copyRow.Add(column.Key, row.Cells[column.Key].Value);
                }
                copyRows.Add(copyRow);
            }
            return true;
        }

        private bool isPasteRows = false;

        bool newGrid_OnPasteRow(object sender)
        {
            isPasteRows = true;
            foreach (Dictionary<string, object> copyRow in copyRows)
            {
                UltraGridRow newRow = drv.newGrid.ugData.DisplayLayout.Bands[0].AddNew();
                drv.newGrid.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                try
                {
                    foreach (KeyValuePair<string, object> kvp in copyRow)
                    {
                        if (newRow.Cells.Exists(kvp.Key))
                            newRow.Cells[kvp.Key].Value = kvp.Value;
                    }
                    SetGeneratorValues(newRow);
                }
                finally
                {
                    drv.newGrid.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                }
                newRow.Update();
            }
            isPasteRows = false;
            return false;
        }


        #region события грида

        #region лукапы


        bool newGrid_OnCheckLookupValue(object sender, string lookupName, object value)
        {
            return true;
        }

        string newGrid_OnGetLookupValue(object sender, string lookupName, bool needFoolValue, object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            DataRow row = null;
            int refID = Convert.ToInt32(value);
            switch (lookupName)
            {
                case "KD":
                    if (kdLookupCash.ContainsKey(refID))
                        return kdLookupCash[refID];
                    row = GetKDData(refID);
                    if (row != null)
                        return string.Format("{0}({1})", row["CodeStr"], row["ID"]);
                    break;
                case "region":
                    if (regionsLookupCash.ContainsKey(refID))
                        return regionsLookupCash[refID];
                    row = GetRegionData(refID);
                    if (row != null)
                        return string.Format("{0}({1})", row["Name"], row["ID"]);
                    break;
            }

            return Convert.ToString(value);
        }

        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // ячейка не задана?
                if (_lastCellUnderMouse == null)
                    return;

                // ячейка не пуста?
                if ((_lastCellUnderMouse.Value == null) ||
                    (_lastCellUnderMouse.Value == DBNull.Value) ||
                    (Convert.ToString(_lastCellUnderMouse.Value) == String.Empty))
                    return;

                CellUIElement uiElem = (CellUIElement)_lastCellUnderMouse.GetUIElement();
                // если каким то образм получили пустой элемент, выходим
                if (uiElem == null) return;

                #region Лукапы
                string sourceColumnName = string.Empty;
                // если ячейка служит для лукапа, то найдем значение, которое будем отображатьпод нею в тултипе или хинте
                if (drv.newGrid.ColumnIsLookup(_lastCellUnderMouse.Column.Key))
                {
                    sourceColumnName = UltraGridEx.GetSourceColumnName(_lastCellUnderMouse.Column.Key);
                    if (sourceColumnName.ToUpper() != "REFKD" && sourceColumnName.ToUpper() != "REFREGIONS")
                        return;
                    object value = _lastCellUnderMouse.Row.Cells[sourceColumnName].Value;
                    if ((value == null) || (value == DBNull.Value))
                        return;
                    string lookupText = GetReferenceAttributeRenaming(Convert.ToInt32(value), sourceColumnName);
                    ShowCellToolTip(uiElem, lookupText);
                    return;
                }
                #endregion

                #region Разыменовка ссылок
                // является ли 
                string attrName = _lastCellUnderMouse.Column.Key;

                GridColumnsStates states = newGrid_OnGetGridColumnsState(drv.newGrid);
                // колонка для аттрибута была инициализирована?
                if (!states.ContainsKey(attrName))
                    return;
                // аттрибут ссылочный?
                if (!states[attrName].IsReference)
                    return;

                // предполагаем что в ячейке находится некое ID и пытаемся его получить
                int refKD = Convert.ToInt32(_lastCellUnderMouse.Value);
                ShowCellToolTip(uiElem, GetReferenceAttributeRenaming(refKD, sourceColumnName));
                return;

                #endregion
            }
            finally
            {
                ToolTipTimer.Stop();
            }
        }

        private void ShowCellToolTip(CellUIElement cellUIElem, string toolTipText)
        {
            if (toolTipText == string.Empty)
                return;
            ToolTip.ToolTipText = toolTipText;
            Point tooltipPos = new Point(cellUIElem.ClipRect.Left, cellUIElem.ClipRect.Bottom);
            ToolTip.Show(drv.newGrid.ugData.PointToScreen(tooltipPos));
        }

        void newGrid_OnMouseLeaveGridElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            // если элемент - ячейка, прячем тултип
            if (e.Element is CellUIElement)
            {
                ToolTip.Hide();
                ToolTipTimer.Stop();
                _lastCellUnderMouse = null;
            }
        }

        void newGrid_OnMouseEnterGridElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (e.Element is CellUIElement)
            {
                // прячем предыдущий тултип
                ToolTip.Hide();
                // получаем ячейку которая соответсвует элементу
                UltraGridCell cell = (UltraGridCell)e.Element.GetContext(typeof(UltraGridCell));
                // если мышь передвинута на другую ячейку - перезапускаем таймер и запоминаем ее
                if (cell != _lastCellUnderMouse)
                {
                    _lastCellUnderMouse = cell;
                    ToolTipTimer.Stop();
                    ToolTipTimer.Start();
                }
            }
        }

        /// <summary>
        /// разыменовка для поля КД
        /// </summary>
        /// <param name="refKD"></param>
        /// <returns></returns>
        private string GetReferenceAttributeRenaming(int refID, string columnName)
        {
            string res = "Код не найден";
            DataRow row = null;
            switch (columnName.ToUpper())
            {
                case "REFKD":
                    row = GetKDData(refID);
                    if (row != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ID: " + Convert.ToString(row["ID"]));
                        sb.AppendLine("КД: " + Convert.ToString(row["CodeStr"]));
                        sb.AppendLine("Наименование: " + Convert.ToString(row["Name"]));
                        res = sb.ToString();
                    }
                    break;
                case "REFREGIONS":
                    row = GetRegionData(refID);
                    if (row != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ID: " + Convert.ToString(row["ID"]));
                        sb.AppendLine("Наименование: " + Convert.ToString(row["Name"]));
                        res = sb.ToString();
                    }
                    break;
            }

            return res;
        }


        private Dictionary<int, string> kdLookupCash = new Dictionary<int, string>();
        private Dictionary<int, string> regionsLookupCash = new Dictionary<int, string>();

        private void FillCash()
        {
            string kdCashQuery = "select ID, CodeStr, Name from d_KD_Analysis";
            string regionsCashQuery = "select ID, Name from d_Regions_Analysis";
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt = (DataTable)db.ExecQuery(kdCashQuery, QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                {
                    kdLookupCash.Add(Convert.ToInt32(row["ID"]), string.Format("{0}({1})", row["CodeStr"], row["ID"]));
                }
                dt = (DataTable)db.ExecQuery(regionsCashQuery, QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                {
                    regionsLookupCash.Add(Convert.ToInt32(row["ID"]), string.Format("{0}({1})", row["Name"], row["ID"]));
                }
            }
        }

        private DataRow GetKDData(int refKD)
        {
            IDatabase db = null;
            try
            {
                db = Workplace.ActiveScheme.SchemeDWH.DB;
                string query = string.Format("select ID, CodeStr, Name from d_KD_Analysis where ID = ?");
                IDbDataParameter param = new System.Data.OleDb.OleDbParameter("ID", refKD);//db.CreateParameter("ID", refKD);
                DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, param);
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                return null;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        private DataRow GetRegionData(int refRegion)
        {
            IDatabase db = null;
            try
            {
                db = Workplace.ActiveScheme.SchemeDWH.DB;
                string query = string.Format("select ID, Name from d_Regions_Analysis where ID = ?");
                IDbDataParameter param = new System.Data.OleDb.OleDbParameter("ID", refRegion); //db.CreateParameter("ID", refRegion);
                DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, param);
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                return null;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        #endregion

        #region экспорт импорт

        bool newGrid_OnSaveToExcel(object sender)
        {
            DataSet tmpDataSet = GetDataExportedDataSet();//GetDataSetForExport();
            ExcelExportImportHelper.ExportDataToExcel(tmpDataSet, cashedColumnsSettings[currentNormatives.ToString()], drv.newGrid.SaveLoadFileName, this.Workplace, false, true);
            return true;
        }

        bool newGrid_OnLoadFromExcel(object sender)
        {
            // создаем датасет со всеми таблицами, которые нужны для импорта нормативов
            DataSet tmpDataSet = new DataSet();
            DataTable tmpNormatives = dtNormatives.Clone();
            tmpNormatives.TableName = currentNormatives.ToString();
            tmpDataSet.Tables.Add(tmpNormatives);
            // добавляем таблицы для записей классификаторов и источников
            // классификатор КД анализ
            DataTable dt = CreateDataSourceTable();
            dt.TableName = "KDSources";
            tmpDataSet.Tables.Add(dt);
            tmpDataSet.Tables.Add(CreateNormativesClassifierTable("KD", false));
            // классификатор районы анализ
            dt = CreateDataSourceTable();
            dt.TableName = "RegionsSources";
            tmpDataSet.Tables.Add(dt);
            tmpDataSet.Tables.Add(CreateNormativesClassifierTable("Regions", true));

            string fileName = drv.newGrid.SaveLoadFileName;
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, false, ref fileName))
            {
                this.Workplace.OperationObj.Text = "Загрузка данных";
                this.Workplace.OperationObj.StartOperation();
                try
                {
                    if (ExcelExportImportHelper.ImportFromExcel(tmpDataSet, string.Empty, string.Empty,
                        cashedColumnsSettings[currentNormatives.ToString()], false, false,
                        fileName, this.Workplace, string.Empty))
                    {
                        if (tmpDataSet.Tables[0].Rows.Count == 0)
                        {
                            this.Workplace.OperationObj.StopOperation();
                            MessageBox.Show("Импортируемый документ Excel имеет неверный формат", "Импорт данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }

                        Dictionary<int, int> KDIds = disintRulesModule.GetNewClassifiersRef(tmpDataSet.Tables["KDSources"], tmpDataSet.Tables["KD"], d_KD_Analysis);
                        Dictionary<int, int> regionsIds = disintRulesModule.GetNewClassifiersRef(tmpDataSet.Tables["RegionsSources"], tmpDataSet.Tables["Regions"], d_Regions_Analysis);

                        foreach (DataRow row in tmpDataSet.Tables[0].Rows)
                        {
                            row["ID"] = dtNormatives.Rows.Count;
                            // меняем ссылки на новые
                            row["RefKD"] = KDIds[Convert.ToInt32(row["RefKD"])];
                            if (row.Table.Columns.Contains("RefRegions"))
                                row["RefRegions"] = regionsIds[Convert.ToInt32(row["RefRegions"])];

                            dtNormatives.Rows.Add(row.ItemArray);
                        }
                    }
                }
                finally
                {
                    this.Workplace.OperationObj.StopOperation();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// сохранение данных в XML
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        bool newGrid_OnSaveToXML(object sender)
        {
            // сохраняем данные
            DataSet tmpDataSet = GetDataExportedDataSet();
            ExportImportHelper.SaveToXML(tmpDataSet, drv.newGrid.SaveLoadFileName);
            return true;
        }


        bool newGrid_OnLoadFromXML(object sender)
        {
            // создаем датасет со всеми таблицами, которые нужны для импорта нормативов
            DataSet tmpDataSet = new DataSet();
            DataTable tmpNormatives = dtNormatives.Clone();
            tmpNormatives.TableName = currentNormatives.ToString();
            tmpDataSet.Tables.Add(tmpNormatives);
            // добавляем таблицы для записей классификаторов и источников
            // классификатор КД анализ
            DataTable dt = CreateDataSourceTable();
            dt.TableName = "KDSources";
            tmpDataSet.Tables.Add(dt);
            tmpDataSet.Tables.Add(CreateNormativesClassifierTable("KD", false));
            // классификатор районы анализ
            dt = CreateDataSourceTable();
            dt.TableName = "RegionsSources";
            tmpDataSet.Tables.Add(dt);
            tmpDataSet.Tables.Add(CreateNormativesClassifierTable("Regions", true));

            string fileName = drv.newGrid.SaveLoadFileName;
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xml, false, ref fileName))
            {
                // проверим, соответствует ли загружаемая XML те
                XmlDocument xmlData = new XmlDocument();
                xmlData.Load(fileName);
                //doc.Get
                XmlNodeList nodes = xmlData.SelectNodes(string.Format("//{0}", currentNormatives.ToString()));
                if (nodes.Count == 0)
                {
                    MessageBox.Show("Импортируемая XML имеет неверный формат", "Импорт данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (ExportImportHelper.LoadFromXML(tmpDataSet, Workplace, fileName))
                {
                    Workplace.OperationObj.Text = "Загрузка данных";
                    Workplace.OperationObj.StartOperation();
                    try
                    {
                        Dictionary<int, int> KDIds = disintRulesModule.GetNewClassifiersRef(tmpDataSet.Tables["KDSources"], tmpDataSet.Tables["KD"], d_KD_Analysis);
                        Dictionary<int, int> regionsIds = disintRulesModule.GetNewClassifiersRef(tmpDataSet.Tables["RegionsSources"], tmpDataSet.Tables["Regions"], d_Regions_Analysis);

                        foreach (DataRow row in tmpDataSet.Tables[0].Rows)
                        {
                            row["ID"] = dtNormatives.Rows.Count;
                            // меняем ссылки на новые
                            row["RefKD"] = KDIds[Convert.ToInt32(row["RefKD"])];
                            if (row.Table.Columns.Contains("RefRegions"))
                            {
                                if (row["RefRegions"] != null && row["RefRegions"] != DBNull.Value)
                                {
                                    int refRegions = Convert.ToInt32(row["RefRegions"]);
                                    if (refRegions >= 0)
                                        row["RefRegions"] = regionsIds[refRegions];
                                }
                                else
                                    row["RefRegions"] = -1;
                            }
                            dtNormatives.Rows.Add(row.ItemArray);
                        }
                    }
                    finally
                    {
                        Workplace.OperationObj.StopOperation();
                    }
                    return true;
                }
            }
            return false;
        }


        void newGrid_OnAftertImportFromXML(object sender, int RowsCountBeforeImport)
        {
            Workplace.OperationObj.Text = "Обработка данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                foreach (DataRow row in dtNormatives.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        for (int i = 1; i <= 17; i++)
                        {
                            string valueColumnName = string.Format("{0}{1}", i, VALUE_POSTFIX);
                            if (dtNormatives.Columns.Contains(valueColumnName))
                                if (row[valueColumnName] == DBNull.Value)
                                    row[valueColumnName] = 0;
                        }
                    }
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }


        void newGrid_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show("Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                try
                {
                    normativeObj.DeleteData(string.Empty);
                    foreach (DataRow row in dtNormatives.Rows)
                    {
                        if (row.RowState == DataRowState.Added)
                            row.AcceptChanges();
                        row.Delete();
                    }
                    dtNormatives.AcceptChanges();
                    BurnNormative(currentNormatives, false);
                    BurnCalculateNormatives(currentNormatives, true);
                    if (currentNormatives == NormativesKind.VarNormativesRegionRF || currentNormatives == NormativesKind.VarNormativesMR)
                        BurnCalculateNormative(false);
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
            }
        }


        void newGrid_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "btnCalculateNormative":
                    Workplace.OperationObj.Text = "Обработка данных";
                    Workplace.OperationObj.StartOperation();
                    try
                    {
                        CalculateAllNormatives();
                        BurnCalculateNormative(false);
                        BurnCalculateNormatives(currentNormatives, false);
                        BurnNormative(currentNormatives, false);
                    }
                    finally
                    {
                        Workplace.OperationObj.StopOperation();
                    }
                    break;
                case "btnCheckDisinRules":
                    if (!UpdateLastChanges())
                        return;

                    List<string> errorList = new List<string>();
                    CheckNormatives(errorList, dtNormatives, currentNormatives, true);

                    if (errorList.Count > 0)
                    {
                        if (MessageBox.Show("В результате проверки данных были обнаружены ошибки. Сохранить протокол проверки?", "Проверка данных",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            CreateErrorsProtocol(errorList);
                        errorList.Clear();
                    }
                    else
                    {
                        MessageBox.Show("В результате проверки данных ошибок обнаружено не было.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
                case "btnCopyNorm":
                    int oldYear = DateTime.Now.Year;
                    int newYear = DateTime.Now.Year;
                    bool transfertToYear = false;
                    if (FormYearDataTransfert.ShowRequestForm(Workplace.WindowHandle, ref oldYear, ref newYear, ref transfertToYear))
                    {
                        Workplace.OperationObj.Text = "Обработка данных";
                        Workplace.OperationObj.StartOperation();
                        int[] rowsCopyCount = new int[0];
                        try
                        {
                            if (transfertToYear)
                            {
                                rowsCopyCount = disintRulesModule.DataTransfert(oldYear, newYear);
                            }
                            else
                            {
                                Dictionary<int, int> rowsParams = new Dictionary<int, int>();
                                if (drv.newGrid.ugData.Selected.Rows.Count == 0 && drv.newGrid.ugData.ActiveRow != null)
                                    drv.newGrid.ugData.ActiveRow.Selected = true;
                                if (drv.newGrid.ugData.Selected.Rows.Count == 0)
                                {
                                    MessageBox.Show(Workplace.WindowHandle, "Не выбрано ни одной записи", "Результаты копирования");
                                    return;
                                }
                                foreach (UltraGridRow row in drv.newGrid.ugData.Selected.Rows)
                                {
                                    rowsParams.Add(Convert.ToInt32(row.Cells["RefKD"].Value),
                                                   Convert.ToInt32(row.Cells["RefYearDayUNV"].Value));
                                }
                                if (rowsParams.Count > 0)
                                {
                                    rowsCopyCount = disintRulesModule.DataTransfert(newYear, rowsParams);
                                }
                            }
                        }
                        finally
                        {
                            Workplace.OperationObj.StopOperation();
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(
                            string.Format("Копирование записей прошло успешно. Всего скопировано записей {0}, из них:", rowsCopyCount[0]));
                        sb.AppendLine(string.Format("  из них по бюджетному кодексу {0} записей", rowsCopyCount[1]));
                        sb.AppendLine(string.Format("  из них субъекта РФ {0} записей", rowsCopyCount[2]));
                        sb.AppendLine(string.Format("  из них муниципального района {0} записей", rowsCopyCount[3]));
                        sb.AppendLine(string.Format("  из них дифференцированные субъекта РФ {0} записей", rowsCopyCount[4]));
                        sb.AppendLine(string.Format("  из них дифференцированные муниципального района {0} записей", rowsCopyCount[5]));
                        if (rowsCopyCount[0] > 0)
                        {
                            sb.AppendLine();
                            sb.AppendLine("Для отображения результатов копирования обновите данные");
                            drv.newGrid.BurnRefreshDataButton(true);
                        }
                        MessageBox.Show(Workplace.WindowHandle, sb.ToString(), "Результаты копирования");
                    }
                    break;
            }
        }


        /// <summary>
        /// сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        bool newGrid_OnSaveChanges(object sender)
        {
            return SaveData(true, (UltraGridEx)sender);
        }

        #region Обновление данных
        /// <summary>
        /// обновление данных
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        bool newGrid_OnRefreshData(object sender)
        {
            RefreshData();

            return true;
        }

        internal void RefreshData()
        {
            Workplace.OperationObj.Text = "Обновление данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                dtNormatives = disintRulesModule.GetNormatives(currentNormatives);
                drv.newGrid.DataSource = dtNormatives;
                if (drv.newGrid.ugData.Rows.Count > 0)
                {
                    drv.newGrid.ugData.Rows[0].Activate();
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }

            if (drn.refreshNormatives.Contains(drn.ultraExplorerBar1.CheckedItem.Key))
            {
                drn.refreshNormatives.Remove(drn.ultraExplorerBar1.CheckedItem.Key);
                drv.newGrid.BurnRefreshDataButton(false);
            }
        }
        #endregion

        #endregion

        void newGrid_OnBeforeCellDeactivate(object sender, CancelEventArgs e)
        {
            // при смене активной ячейки производим вычисления для записи
            UltraGridCell activeCell = drv.newGrid.ugData.ActiveCell;
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(drv.newGrid.ugData);
            string cellKey = UltraGridEx.GetSourceColumnName(activeCell.Column.Key);

            if (string.Compare(cellKey, "RefYearDayUNV", true) == 0)
            {
                if (activeCell.Value != null && activeCell.Value != DBNull.Value)
                {
                    int yearDay = Convert.ToInt16(activeCell.Value);
                    if (yearDay < 1000)
                    {
                        MessageBox.Show("Значение поля не соответствует маске", "Нормативы отчислений",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                        return;
                    }
                }
            }

            if (cellKey == "RefKD" || cellKey == "RefYearDayUNV" || cellKey == "RefRegions")
            {
                DataRow row = dtNormatives.Select(string.Format("ID = {0}", activeRow.Cells["ID"].Value))[0];
                if (row.HasVersion(DataRowVersion.Original))
                {
                    if (Convert.ToInt32(row[cellKey, DataRowVersion.Original]) != Convert.ToInt32(row[cellKey]))
                        GetFilledValues(activeRow);
                }
                else
                    GetFilledValues(activeRow);
            }

            if (activeCell.IsDataCell)
            {
                cellKey = activeCell.Column.Key;
                // если значение удалили и перешли на другую ячейку, ставим в текущей ноль
                if (activeCell.Value == DBNull.Value && cellKey.Contains(VALUE_POSTFIX))
                    activeCell.Value = 0;

                decimal cellValue = Convert.ToDecimal(activeCell.Value);


                decimal roundCellValue = Math.Round(cellValue, 2);
                if (cellValue != roundCellValue)
                    activeCell.Value = roundCellValue;
                if (activeCell.Column.Key == "6" + VALUE_POSTFIX)
                {
                    // для сельского и городского поселений ставим такие же значения, как и у поселения
                    activeRow.Cells["16" + VALUE_POSTFIX].Value = activeCell.Value;
                    activeRow.Cells["17" + VALUE_POSTFIX].Value = activeCell.Value;
                }
            }
            // расчеты для записи
            CalculateRow(activeRow);
        }

        void newGrid_OnBeforeRowDeactivate(object sender, CancelEventArgs e)
        {

        }

        /// <summary>
        /// установка флага корректности для записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newGrid_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            if (row.Cells["ID"].Value == DBNull.Value)
                return;
            // получаем запись в таблице, кторая соответствует текущей в гриде
            int id = Convert.ToInt32(row.Cells["ID"].Value);
            DataRow[] rows = dtNormatives.Select(string.Format("ID = {0}", id));
            if (rows.Length > 0)
            {
                DataRow dataRow = rows[0];
                // проверим на дубликаты
                if (dataRow.RowState == DataRowState.Added)
                {
                    object refKD = dataRow["RefKD"];
                    object refYearDayUNV = dataRow["RefYearDayUNV"];
                    object refRegions = null;
                    DataRow[] selectedRows = null;
                    if (currentNormatives == NormativesKind.VarNormativesMR || currentNormatives == NormativesKind.VarNormativesRegionRF)
                    {
                        refRegions = dataRow["RefRegions"];
                        if (refKD != DBNull.Value && refYearDayUNV != DBNull.Value && refRegions != DBNull.Value)
                        {
                            selectedRows = dtNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions = {2} and ID <> {3}",
                                refKD, refYearDayUNV, refRegions, dataRow["ID"]));
                            if (selectedRows.Length > 0)
                            {
                                // текущая запись есть дубликат
                                row.Cells["clmnValideNormative"].Appearance.Image = drv.ilImages.Images[7];
                                row.Cells["clmnValideNormative"].ToolTipText = string.Format("Запись является дубликатом записи с ID = {0}", selectedRows[0]["ID"]);
                            }
                        }
                    }
                    else
                    {
                        if (refKD != DBNull.Value && refYearDayUNV != DBNull.Value)
                        {
                            selectedRows = dtNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and ID <> {2}",
                                refKD, refYearDayUNV, dataRow["ID"]));
                            if (selectedRows.Length > 0)
                            {
                                // текущая запись есть дубликат
                                row.Cells["clmnValideNormative"].Appearance.Image = drv.ilImages.Images[7];
                                row.Cells["clmnValideNormative"].ToolTipText = string.Format("Запись является дубликатом записи с ID = {0}", selectedRows[0]["ID"]);
                            }
                        }
                    }
                }

                if (dataRow.RowState != DataRowState.Deleted && dataRow.RowState != DataRowState.Added)
                {
                    StringBuilder errors = new StringBuilder();
                    if (!CheckRowWithoutCalculate(dataRow, ref errors))
                    {
                        // в качестве хинта получаем сообщения об ошибках
                        row.Cells["clmnValideNormative"].Appearance.Image = drv.ilImages.Images[5];
                        row.Cells["clmnValideNormative"].ToolTipText = errors.ToString();//"Некорректный норматив отчислений";
                    }
                    else
                    {
                        row.Cells["clmnValideNormative"].Appearance.Image = drv.ilImages.Images[6];
                        row.Cells["clmnValideNormative"].ToolTipText = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// обработчик добавления записей
        /// </summary> 
        /// <param name="sender"></param>
        /// <param name="row"></param>
        void newGrid_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            // после добавления записей заполним все поля, которые не берутся из других нормативов нулями
            drv.newGrid.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                row.Cells["ID"].Value = dtNormatives.Rows.Count + 1;
                row.Update();
                if (!isPasteRows)
                {
                    SetGeneratorValues(row);
                    CalculateRow(row);
                }
            }
            finally
            {
                drv.newGrid.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        private void SetGeneratorValues(UltraGridRow row)
        {
            for (int i = 1; i <= 17; i++)
            {
                string cellValueKey = string.Format("{0}{1}", i, VALUE_POSTFIX);
                string cellResultValueKey = string.Format("{0}{1}", i, RESULT_VALUE_POSTFIX);

                if (row.Cells.Exists(cellValueKey) && row.Cells[cellValueKey].Value is DBNull)
                    row.Cells[cellValueKey].Value = 0;
                if (row.Cells.Exists(cellResultValueKey) && row.Cells[cellValueKey].Value is DBNull)
                    row.Cells[cellResultValueKey].Value = 0;
                if (row.Cells.Exists(string.Format("{0}", i)))
                    row.Cells[string.Format("{0}", i)].Value = normativeObj.GetGeneratorNextValue;
            }
        }


        private int GetFilledYear()
        {
            foreach (DataRow row in dtNormatives.Rows)
            {
                if (row["RefYearDayUNV"] != DBNull.Value)
                    return Convert.ToInt32(row["RefYearDayUNV"]);
            }
            return 0;
        }

        private const string d_KD_Analysis = "2553274b-4cee-4d20-a9a6-eef173465d8b";
        private const string fx_Date_YearDayUNV = "b4612528-0e51-4e6b-8891-64c22611816b";
        private const string d_Regions_Analysis = "383f887a-3ebb-4dba-8abb-560b5777436f";

        /// <summary>
        /// выбор справочников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newGrid_OnClickCellButton(object sender, CellEventArgs e)
        {
            IModalClsManager cls = null;
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(drv.newGrid.ugData);

            object oldRefKD = activeRow.Cells["RefKD"].Value;
            decimal oldRefYearDayUNV = 
                (activeRow.Cells["RefYearDayUNV"].Value is DBNull || activeRow.Cells["RefYearDayUNV"].Value == null) ?
                0 : Convert.ToDecimal(activeRow.Cells["RefYearDayUNV"].Value);
            object oldRefRegion = null;
            if (activeRow.Band.Columns.Exists("RefRegions"))
                oldRefRegion = activeRow.Cells["RefRegions"].Value;

            object refKD = null;
            object refRegion = null;

            int dataSourceYear = 0;
            if (activeRow.Cells["RefYearDayUNV"].Value != DBNull.Value)
                dataSourceYear = Convert.ToInt32(activeRow.Cells["RefYearDayUNV"].Value);
            if (dataSourceYear == 0)
                dataSourceYear = GetFilledYear();
            // проверяем, изменились ли при вызове справочников значения в полях, если да, то перезаполняем поля
            bool fillValues = false;
            switch (UltraGridEx.GetSourceColumnName(e.Cell.Column.Key))
            {
                case "RefKD":
                    cls = Workplace.ClsManager;
                    refKD = -1;
                    if (cls.ShowClsModal(d_KD_Analysis, -1, -1, dataSourceYear, ref refKD))
                        if (refKD != oldRefKD)
                        {
                            activeRow.Cells["RefKD"].Value = refKD;
                            fillValues = true;
                        }
                    break;
                case "RefYearDayUNV":
                    object[] values = new object[1];
                    if (GetPeriod(new string[] { "ID" }, ref values))
                    {
                        decimal refYearDayUNV = Convert.ToDecimal(values[0])/10000;
                        if (oldRefYearDayUNV != refYearDayUNV)
                        {
                            activeRow.Cells["RefYearDayUNV"].Value = refYearDayUNV;
                            fillValues = true;
                        }
                    }
                    break;
                case "RefRegions":
                    cls = Workplace.ClsManager;
                    refRegion = -1;
                    if (cls.ShowClsModal(d_Regions_Analysis, -1, -1, dataSourceYear, ref refRegion))
                        if (refRegion != oldRefRegion)
                        {
                            activeRow.Cells["RefRegions"].Value = refRegion;
                            fillValues = true;
                        }
                    break;
            }
            // в случае если все поля с ссылками введены и они отличаются от тех, что были раньше, то подтягиваем все значения
            if (fillValues)
                // для для некоторых полей подтягиваем значения из других нормативов
                GetFilledValues(activeRow);
        }


        private void GetFilledValues(UltraGridRow row)
        {
            if (row.Cells["ID"].Value == DBNull.Value)
                return;
            DataRow dataRow = dtNormatives.Select(string.Format("ID = {0}", row.Cells["ID"].Value))[0];
            GetFilledValues(dataRow);
        }


        private void GetFilledValues(DataRow row)
        {
            if (row["RefKD"] != DBNull.Value && row["RefYearDayUNV"] != DBNull.Value)
            {
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]) * 10000;
                switch (currentNormatives)
                {
                    case NormativesKind.NormativesRegionRF:
                        // если изменились какие то параметры у родительской записи, то поменяем 
                        FillCell(2, refKD, refYearDayUNV, row, "2" + VALUE_POSTFIX);
                        FillCell(16, refKD, refYearDayUNV, row, "16" + VALUE_POSTFIX);
                        FillCell(17, refKD, refYearDayUNV, row, "17" + VALUE_POSTFIX);
                        FillCell(6, refKD, refYearDayUNV, row, "6" + VALUE_POSTFIX);
                        FillCell(5, refKD, refYearDayUNV, row, "5" + VALUE_POSTFIX);
                        FillCell(15, refKD, refYearDayUNV, row, "15" + VALUE_POSTFIX);
                        break;
                    case NormativesKind.NormativesMR:
                        // ищем подходящую запись и значение
                        FillCell(4, refKD, refYearDayUNV, row, "4" + VALUE_POSTFIX);
                        FillCell(16, refKD, refYearDayUNV, row, "16" + VALUE_POSTFIX);
                        FillCell(17, refKD, refYearDayUNV, row, "17" + VALUE_POSTFIX);
                        FillCell(6, refKD, refYearDayUNV, row, "6" + VALUE_POSTFIX);
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        FillCell(2, refKD, refYearDayUNV, row, "2" + VALUE_POSTFIX);
                        FillCell(15, refKD, refYearDayUNV, row, "15" + REF_VALUE_POSTFIX);
                        FillCell(5, refKD, refYearDayUNV, row, "5" + REF_VALUE_POSTFIX);
                        FillCell(15, refKD, refYearDayUNV, row, "15" + REF_VALUE_POSTFIX);
                        FillCell(6, refKD, refYearDayUNV, row, "6" + REF_VALUE_POSTFIX);
                        FillCell(16, refKD, refYearDayUNV, row, "16" + REF_VALUE_POSTFIX);
                        FillCell(17, refKD, refYearDayUNV, row, "17" + REF_VALUE_POSTFIX);
                        break;
                    case NormativesKind.VarNormativesMR:
                        FillCell(4, refKD, refYearDayUNV, row, "4" + VALUE_POSTFIX);
                        FillCell(6, refKD, refYearDayUNV, row, "6" + REF_VALUE_POSTFIX);
                        FillCell(16, refKD, refYearDayUNV, row, "16" + REF_VALUE_POSTFIX);
                        FillCell(17, refKD, refYearDayUNV, row, "17" + REF_VALUE_POSTFIX);
                        break;
                }
            }
        }

        private bool FillCell(int cellNumber, int refKD, int refYearDayUNV, DataRow row, string cellName)
        {

            decimal value = Convert.ToDecimal(disintRulesModule.GetConsRegionBudget(currentNormatives, refKD, refYearDayUNV, cellNumber));
            value = value * 100;
            if (value < 0)
                value = 0;

            if (row[cellName] == DBNull.Value)
            {
                row[cellName] = value;
                return true;
            }

            decimal cellValue = Convert.ToDecimal(row[cellName]);
            if (value != cellValue)
            {
                row[cellName] = value;
                return true;
            }
            return false;
        }


        void newGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!drv.newGrid._ugData.DisplayLayout.Bands[0].Columns.Exists("clmnValideNormative"))
            {
                UltraGridColumn valideColumn = drv.newGrid._ugData.DisplayLayout.Bands[0].Columns.Add("clmnValideNormative");
                valideColumn.Header.Caption = string.Empty;
                valideColumn.Header.VisiblePosition = 1;
                UltraGridHelper.SetLikelyImageColumnsStyle(valideColumn, -1);
            }
        }


        #region настройка колонок грида

        private Dictionary<string, GridColumnsStates> cashedColumnsSettings = new Dictionary<string, GridColumnsStates>();

        GridColumnsStates newGrid_OnGetGridColumnsState(object sender)
        {
            if (cashedColumnsSettings.ContainsKey(currentNormatives.ToString()))
                return cashedColumnsSettings[currentNormatives.ToString()];

            if (currentNormatives == NormativesKind.VarNormativesMR || currentNormatives == NormativesKind.VarNormativesRegionRF)
                return GetVarColumnsSettings();
            int valueCoumnsVisibleIndex = 8;
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsHiden = true;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new GridColumnState();
            state.ColumnName = "RefKD";
            state.ColumnCaption = "КД";
            state.ColumnPosition = 1;
            state.Tag = "KD";
            state.IsLookUp = true;
            state.IsReference = true;
            state.ColumnWidth = 160;
            states.Add("RefKD", state);

            state = new GridColumnState();
            state.ColumnName = "RefYearDayUNV";
            state.ColumnCaption = "Период";
            state.ColumnPosition = 2;
            state.IsLookUp = true;
            state.Mask = "9999";
            state.Tag = "a.Norm.Region.RefYearDayUNV";
            states.Add("RefYearDayUNV", state);

            state = new GridColumnState();
            state.ColumnName = "RefRegions";
            state.ColumnCaption = string.Empty;
            state.IsHiden = true;
            states.Add("RefRegions", state);

            state = new GridColumnState();
            state.ColumnName = "1";
            state.IsHiden = true;
            states.Add("1", state);

            state = new GridColumnState();
            state.ColumnName = "1_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.ColumnCaption = "% федеральный бюджет";
            state.Mask = negativeValueMask;
            state.DefaultValue = 0;
            state.IsSystem = true;
            states.Add("1_Value", state);

            state = new GridColumnState();
            state.ColumnName = "2";
            state.IsHiden = true;
            states.Add("2", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "2_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = negativeValueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% конс. бюджет субъекта";
            state.IsSystem = true;
            states.Add("2_Value", state);

            state = new GridColumnState();
            state.ColumnName = "3";
            state.IsHiden = true;
            states.Add("3", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "3_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет субъекта";
            if (currentNormatives == NormativesKind.NormativesRegionRF)
            {
                state.IsSystem = true;
                state.Mask = negativeValueMask;
            }
            states.Add("3_Value", state);

            state = new GridColumnState();
            state.ColumnName = "14";
            state.IsHiden = true;
            states.Add("14", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "14_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% конс. бюджет МО";
            state.IsSystem = true;
            states.Add("14_Value", state);

            state = new GridColumnState();
            state.ColumnName = "15";
            state.IsHiden = true;
            states.Add("15", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "15_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет ГО";
            states.Add("15_Value", state);

            state = new GridColumnState();
            state.ColumnName = "4";
            state.IsHiden = true;
            states.Add("4", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "4_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% конс. бюджет МР";
            state.IsSystem = true;
            states.Add("4_Value", state);

            state = new GridColumnState();
            state.ColumnName = "5";
            state.IsHiden = true;
            states.Add("5", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "5_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет района";
            if (currentNormatives == NormativesKind.NormativesMR)
            {
                state.IsSystem = true;
                state.Mask = negativeValueMask;
            }
            states.Add("5_Value", state);

            state = new GridColumnState();
            state.ColumnName = "6";
            state.IsHiden = true;
            states.Add("6", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "6_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет поселения";
            states.Add("6_Value", state);

            state = new GridColumnState();
            state.ColumnName = "16";
            state.IsHiden = true;
            states.Add("16", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "16_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет городского поселения";
            states.Add("16_Value", state);

            state = new GridColumnState();
            state.ColumnName = "17";
            state.IsHiden = true;
            states.Add("17", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "17_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет сельского поселения";
            states.Add("17_Value", state);

            state = new GridColumnState();
            state.ColumnName = "7";
            state.IsHiden = true;
            states.Add("7", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "7_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% внебюджетные фонды";
            //state.IsSystem = true;
            states.Add("7_Value", state);

            state = new GridColumnState();
            state.ColumnName = "8";
            state.IsHiden = true;
            states.Add("8", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "8_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% пенсионный фонд";
            states.Add("8_Value", state);

            state = new GridColumnState();
            state.ColumnName = "9";
            state.IsHiden = true;
            states.Add("9", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "9_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% фонд социального страхования";
            states.Add("9_Value", state);

            state = new GridColumnState();
            state.ColumnName = "10";
            state.IsHiden = true;
            states.Add("10", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "10_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% фед. фонд ОМС";
            states.Add("10_Value", state);

            state = new GridColumnState();
            state.ColumnName = "11";
            state.IsHiden = true;
            states.Add("11", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "11_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% террит фонды ОМС";
            states.Add("11_Value", state);

            state = new GridColumnState();
            state.ColumnName = "12";
            state.IsHiden = true;
            states.Add("12", state);

            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "12_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% на счет УФК Смоленск";
            states.Add("12_Value", state);

            state = new GridColumnState();
            state.ColumnName = "13";
            state.IsHiden = true;
            states.Add("13", state);
            valueCoumnsVisibleIndex += 1;

            state = new GridColumnState();
            state.ColumnName = "13_Value";
            state.ColumnPosition = valueCoumnsVisibleIndex;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% в областной бюджет Тюменской обл";
            states.Add("13_Value", state);

            cashedColumnsSettings.Add(currentNormatives.ToString(), states);

            return states;
        }


        private GridColumnsStates GetVarColumnsSettings()
        {
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsHiden = true;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new GridColumnState();
            state.ColumnName = "RefRegions";
            state.ColumnCaption = "Район";
            state.ColumnPosition = 1;
            state.Tag = "region";
            state.IsLookUp = true;
            state.ColumnWidth = 150;
            states.Add("RefRegions", state);

            state = new GridColumnState();
            state.ColumnName = "RefKD";
            state.ColumnCaption = "КД";
            state.ColumnPosition = 2;
            state.Tag = "KD";
            state.IsLookUp = true;
            state.IsReference = true;
            state.ColumnWidth = 160;
            state.Tag = "KD";
            states.Add("RefKD", state);

            state = new GridColumnState();
            state.ColumnName = "RefYearDayUNV";
            state.ColumnCaption = "Год";
            state.ColumnPosition = 3;
            state.IsLookUp = true;
            states.Add("RefYearDayUNV", state);

            state = new GridColumnState();
            state.ColumnName = "2";
            state.IsHiden = true;
            states.Add("2", state);

            state = new GridColumnState();
            state.ColumnName = "2_Value";
            state.ColumnPosition = 10;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% конс. бюджет субъекта";
            state.IsSystem = true;
            states.Add("2_Value", state);

            state = new GridColumnState();
            state.ColumnName = "3";
            state.IsHiden = true;
            states.Add("3", state);

            state = new GridColumnState();
            state.ColumnName = "3_Value";
            state.ColumnPosition = 11;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет субъекта";
            if (currentNormatives == NormativesKind.VarNormativesRegionRF)
            {
                state.IsSystem = true;
                state.Mask = negativeValueMask;
            }
            states.Add("3_Value", state);

            state = new GridColumnState();
            state.ColumnName = "14";
            state.IsHiden = true;
            states.Add("14", state);

            state = new GridColumnState();
            state.ColumnName = "14_Value";
            state.ColumnPosition = 12;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% конс. бюджет МО";
            state.IsSystem = true;
            states.Add("14_Value", state);

            #region

            state = new GridColumnState();
            state.ColumnName = "15" + RESULT_POSTFIX;
            state.IsHiden = true;
            states.Add("15" + RESULT_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "15" + RESULT_VALUE_POSTFIX;
            state.ColumnPosition = 13;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет ГО итоговый";
            state.IsSystem = true;
            states.Add("15" + RESULT_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "15" + REF_VALUE_POSTFIX;
            state.ColumnPosition = 14;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет ГО общий";
            state.IsSystem = true;
            states.Add("15" + REF_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "15";
            state.IsHiden = true;
            states.Add("15", state);

            state = new GridColumnState();
            state.ColumnName = "15_Value";
            state.ColumnPosition = 15;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет ГО дополнит.";
            states.Add("15_Value", state);

            #endregion

            state = new GridColumnState();
            state.ColumnName = "4";
            state.IsHiden = true;
            states.Add("4", state);

            state = new GridColumnState();
            state.ColumnName = "4_Value";
            state.ColumnPosition = 16;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% конс. бюджет МР";
            state.IsSystem = true;
            states.Add("4_Value", state);

            #region

            state = new GridColumnState();
            state.ColumnName = "5" + RESULT_POSTFIX;
            state.IsHiden = true;
            states.Add("5" + RESULT_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "5" + RESULT_VALUE_POSTFIX;
            state.ColumnPosition = 17;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет района итоговый";
            state.IsSystem = true;
            states.Add("5" + RESULT_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "5" + REF_VALUE_POSTFIX;
            state.ColumnPosition = 18;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет района общий";
            state.IsSystem = true;
            states.Add("5" + REF_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "5";
            state.IsHiden = true;
            states.Add("5", state);

            state = new GridColumnState();
            state.ColumnName = "5_Value";
            state.ColumnPosition = 19;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            if (currentNormatives == NormativesKind.VarNormativesRegionRF)
                state.ColumnCaption = "% бюджет района дополнит.";
            else
                state.ColumnCaption = "% бюджет района";
            if (currentNormatives == NormativesKind.VarNormativesMR)
            {
                state.IsSystem = true;
                state.Mask = negativeValueMask;
            }
            states.Add("5_Value", state);

            #endregion

            #region

            state = new GridColumnState();
            state.ColumnName = "6" + RESULT_POSTFIX;
            state.IsHiden = true;
            states.Add("6" + RESULT_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "6" + RESULT_VALUE_POSTFIX;
            state.ColumnPosition = 20;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет поселения итоговый";
            state.IsSystem = true;
            states.Add("6" + RESULT_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "6" + REF_VALUE_POSTFIX;
            state.ColumnPosition = 21;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет поселения общий";
            state.IsSystem = true;
            states.Add("6" + REF_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "6";
            state.IsHiden = true;
            states.Add("6", state);

            state = new GridColumnState();
            state.ColumnName = "6_Value";
            state.ColumnPosition = 22;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет поселения дополнит.";
            states.Add("6_Value", state);

            #endregion

            #region

            state = new GridColumnState();
            state.ColumnName = "16" + RESULT_POSTFIX;
            state.IsHiden = true;
            states.Add("16" + RESULT_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "16" + RESULT_VALUE_POSTFIX;
            state.ColumnPosition = 23;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет городского поселения итоговый";
            state.IsSystem = true;
            states.Add("16" + RESULT_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "16" + REF_VALUE_POSTFIX;
            state.ColumnPosition = 24;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет городского поселения общий";
            state.IsSystem = true;
            states.Add("16" + REF_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "16";
            state.IsHiden = true;
            states.Add("16", state);

            state = new GridColumnState();
            state.ColumnName = "16_Value";
            state.ColumnPosition = 25;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет городского поселения дополнит.";
            states.Add("16_Value", state);

            #endregion

            #region

            state = new GridColumnState();
            state.ColumnName = "17" + RESULT_POSTFIX;
            state.IsHiden = true;
            states.Add("17" + RESULT_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "17" + RESULT_VALUE_POSTFIX;
            state.ColumnPosition = 26;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет сельского поселения итоговый";
            state.IsSystem = true;
            states.Add("17" + RESULT_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "17" + REF_VALUE_POSTFIX;
            state.ColumnPosition = 27;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет сельского поселения общий";
            state.IsSystem = true;
            states.Add("17" + REF_VALUE_POSTFIX, state);

            state = new GridColumnState();
            state.ColumnName = "17";
            state.IsHiden = true;
            states.Add("17", state);

            state = new GridColumnState();
            state.ColumnName = "17_Value";
            state.ColumnPosition = 28;
            state.Mask = valueMask;
            state.DefaultValue = 0;
            state.ColumnCaption = "% бюджет сельского поселения дополнит.";
            states.Add("17_Value", state);

            #endregion

            cashedColumnsSettings.Add(currentNormatives.ToString(), states);

            return states;
        }


        #endregion


        void newGrid_OnCancelChanges(object sender)
        {
            dtNormatives.RejectChanges();
            CanDeactivate = true;
        }


        private bool UpdateLastChanges()
        {
            if (dtNormatives.Rows.Count == 0)
                return false;
            if (drv.newGrid.ugData.ActiveRow != null)
            {
                UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(drv.newGrid.ugData);
                activeRow.Cells[0].Activate();
                activeRow.Update();
            }
            else
                drv.newGrid.ugData.Rows[0].Activate();
            return true;
        }


        private bool SaveData(bool showWarnings, UltraGridEx grid)
        {
            // сохранение изменений по нормативам
            if (!UpdateLastChanges())
                return true;

            Workplace.OperationObj.Text = "Сохранение данных";
            try
            {
                drv.newGrid.ugData.ActiveRow.Update();
                List<string> errorList = new List<string>();

                if (!CheckNormativesData(dtNormatives))
                    return false;

                if (!CheckNormatives(errorList, dtNormatives, currentNormatives, false))
                {
                    MessageBox.Show("В результате проверки данных были обнаружены записи - дубликаты. Данные невозможно сохранить", "Проверка данных",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                // если все поля заполненв, сохраним изменения
                else
                {
                    Workplace.OperationObj.StartOperation();
                    DataTable changes = dtNormatives.GetChanges();
                    if (changes != null)
                    {
                        if (disintRulesModule.ApplyChanges(currentNormatives, changes))
                            RefreshData();
                    }
                    // если в определенных нормативах произошли изменения, покажем, что нужно пересчитать другие нормативы
                    BurnCalculateNormatives(currentNormatives, true);
                    //применим изменения на клиенте
                    dtNormatives.AcceptChanges();
                    drv.newGrid.DataSource = dtNormatives;
                }
                // если резльтаты проверки показали на какие то ошибки, то выведем их в некий отчет
                if (errorList.Count > 0 && showWarnings)
                {
                    Workplace.OperationObj.StopOperation();
                    if (MessageBox.Show("В результате проверки данных были обнаружены ошибки. Сохранить протокол проверки?", "Проверка данных",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        CreateErrorsProtocol(errorList);
                    errorList.Clear();
                }
                return true;
            }
            catch (Exception e)
            {
                Workplace.OperationObj.StopOperation();
                throw new Exception(e.Message);
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        /// <summary>
        /// подсвечивает нормативы в окне навигации
        /// </summary>
        /// <param name="normative"></param>
        /// <param name="isBurn"></param>
        private void BurnCalculateNormatives(NormativesKind normative, bool isBurn)
        {
            // в зависимости от того, какой норматив был изменен, подсвечиваем разное нормативы на пересчет
            switch (normative)
            {
                case NormativesKind.NormativesBK:
                    // ваще везде
                    bool burnCalculate = BurnNormative(NormativesKind.NormativesMR, isBurn);
                    burnCalculate = BurnNormative(NormativesKind.NormativesRegionRF, isBurn) || burnCalculate;
                    burnCalculate = BurnNormative(NormativesKind.VarNormativesMR, isBurn) || burnCalculate;
                    burnCalculate = BurnNormative(NormativesKind.VarNormativesRegionRF, isBurn) || burnCalculate;
                    BurnCalculateNormative(burnCalculate);
                    break;
                case NormativesKind.NormativesMR:
                    // токо в одном месте
                    burnCalculate = BurnNormative(NormativesKind.VarNormativesRegionRF, isBurn);
                    burnCalculate = BurnNormative(NormativesKind.VarNormativesMR, isBurn) || burnCalculate;
                    BurnCalculateNormative(burnCalculate);
                    break;
                case NormativesKind.NormativesRegionRF:
                    // почти везде
                    burnCalculate = BurnNormative(NormativesKind.VarNormativesMR, isBurn);
                    burnCalculate = BurnNormative(NormativesKind.NormativesMR, isBurn) || burnCalculate;
                    burnCalculate = BurnNormative(NormativesKind.VarNormativesRegionRF, isBurn) || burnCalculate;
                    BurnCalculateNormative(burnCalculate);
                    break;
            }
        }


        /// <summary>
        /// зажигаем строчку в навигации с нормативом
        /// </summary>
        private bool BurnNormative(NormativesKind normative, bool isBurn)
        {
            // получаем пункт, который нужно будет подсветить
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem item = null;
            switch (normative)
            {
                case NormativesKind.NormativesMR:
                    item = drn.ultraExplorerBar1.Groups[0].Items["97D945A2-75C7-4301-AFEF-20CE62F30F48"];
                    break;
                case NormativesKind.NormativesRegionRF:

                    item = drn.ultraExplorerBar1.Groups[0].Items["D357705E-5594-4f2a-8E78-0872DDEC54E7"];
                    break;
                case NormativesKind.VarNormativesMR:

                    item = drn.ultraExplorerBar1.Groups[0].Items["334845A1-FC38-43da-9AB8-AB94AA79FBFE"];
                    break;
                case NormativesKind.VarNormativesRegionRF:

                    item = drn.ultraExplorerBar1.Groups[0].Items["61C6D67E-66E5-47c4-BB8E-3B5478975D96"];
                    break;
            }
            if (item != null)
            {
                if (isBurn && PresentNormatives(normative))
                {
                    // если данные по нормативу присутствуют и его нужно пересчитать, зажигаем напоминание об этом
                    item.Settings.AppearancesSmall.Appearance.BackColor = Color.FromKnownColor(KnownColor.White);
                    item.Settings.AppearancesSmall.Appearance.BackColor2 = Color.FromKnownColor(KnownColor.Chartreuse);
                    item.Settings.AppearancesSmall.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
                    if (!drn.refreshNormatives.Contains(item.Key))
                        drn.refreshNormatives.Add(item.Key);
                    return true;
                }

                // в противном случае, даже если напоминание было, убираем его
                item.Settings.AppearancesSmall.Appearance.BackColor = Color.Empty;
                item.Settings.AppearancesSmall.Appearance.BackColor2 = Color.Empty;
                item.Settings.AppearancesSmall.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            }
            return false;
        }

        internal void BurnGridRefreshData(bool burnRefresh)
        {
            drv.newGrid.BurnRefreshDataButton(burnRefresh);
        }

        #region зажигаем огоньки на кнопках
        /*
        /// <summary>
        /// подсвечивает кнопку рассчета нормативов на тулбаре
        /// </summary>
        /// <param name="item"></param>
        private void BurnCalculateNormative(Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem item)
        {
            bool fire = item.Settings.AppearancesSmall.Appearance.BackColor == Color.FromKnownColor(KnownColor.White) &&
                item.Settings.AppearancesSmall.Appearance.BackColor2 == Color.FromKnownColor(KnownColor.Chartreuse);
            BurnCalculateNormative(fire);
        }*/

        /// <summary>
        /// подсвечивает кнопку рассчета нормативов на тулбаре
        /// </summary>
        /// <param name="fireItem"></param>
        private void BurnCalculateNormative(bool fireItem)
        {
            UltraToolbar tb = drv.newGrid.utmMain.Toolbars["utbColumns"];
            if (fireItem)
            {
                tb.Tools["btnCalculateNormative"].SharedProps.AppearancesSmall.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
                tb.Tools["btnCalculateNormative"].SharedProps.AppearancesSmall.Appearance.BackColor2 = Color.FromKnownColor(KnownColor.Chartreuse);
                tb.Tools["btnCalculateNormative"].SharedProps.AppearancesSmall.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            }
            else
            {
                tb.Tools["btnCalculateNormative"].SharedProps.AppearancesSmall.Appearance.BackColor = Color.Empty;
                tb.Tools["btnCalculateNormative"].SharedProps.AppearancesSmall.Appearance.BackColor2 = Color.Empty;
                tb.Tools["btnCalculateNormative"].SharedProps.AppearancesSmall.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            }
        }


        #endregion


        private bool PresentNormatives(NormativesKind normative)
        {
            return GetNormativeRowsCount(normative) != 0;
        }


        public int GetNormativeRowsCount(NormativesKind normatives)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string countQuery = "select count(ID) from {0}";
                switch (normatives)
                {
                    case NormativesKind.NormativesBK:
                        countQuery = string.Format(countQuery, "F_NORM_BK");
                        break;
                    case NormativesKind.NormativesMR:
                        countQuery = string.Format(countQuery, "F_NORM_MR");
                        break;
                    case NormativesKind.NormativesRegionRF:
                        countQuery = string.Format(countQuery, "F_NORM_REGION");
                        break;
                    case NormativesKind.VarNormativesMR:
                        countQuery = string.Format(countQuery, "F_NORM_VARIEDMR");
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        countQuery = string.Format(countQuery, "F_NORM_VARIEDREGION");
                        break;
                }
                int count = Convert.ToInt32(db.ExecQuery(countQuery, QueryResultTypes.Scalar));
                return count;
            }
        }


        private static string GetNormativeRusName(NormativesKind normative)
        {
            switch (normative)
            {
                case NormativesKind.AllNormatives:
                    return "Все нормативы отчислений";
                case NormativesKind.NormativesBK:
                    return "Нормативы отчислений по бюджетному кодексу";
                case NormativesKind.NormativesMR:
                    return "Нормативы отчислений муниципального района";
                case NormativesKind.NormativesRegionRF:
                    return "Нормативы отчислений субъекта РФ";
                case NormativesKind.VarNormativesMR:
                    return "Дифференцированные нормативы отчислений муниципального района";
                case NormativesKind.VarNormativesRegionRF:
                    return "Дифференцированные нормативы отчислений субъекта РФ";
            }
            return string.Empty;
        }

        /// <summary>
        /// код КД по ссылке на него
        /// </summary>
        /// <param name="refKD"></param>
        /// <returns></returns>
        private string GetKDFromRef(int refKD)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string selectQuery = string.Format("select CodeStr from d_KD_Analysis where ID = {0}", refKD);
                object KD = db.ExecQuery(selectQuery, QueryResultTypes.Scalar);
                if (KD != null)
                    return Convert.ToString(KD);
                return string.Empty;
            }
        }

        /// <summary>
        /// наименование района по ссылке на него
        /// </summary>
        /// <param name="refRegions"></param>
        /// <returns></returns>
        private string GetRegionFromRef(int refRegions)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string selectQuery = string.Format("select Name from d_Regions_Analysis where ID = {0}", refRegions);
                object region = db.ExecQuery(selectQuery, QueryResultTypes.Scalar);
                if (region != null)
                    return Convert.ToString(region);
                return string.Empty;
            }
        }

        /// <summary>
        /// информация о записи
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetRowInfo(DataRow row)
        {
            int refKD = Convert.ToInt32(row["RefKD"]);
            int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
            int refRegions = -1;
            if (row.Table.Columns.Contains("RefRegions"))
                refRegions = Convert.ToInt32(row["RefRegions"]);

            if (refRegions >= 0)
                return string.Format("Запись с ID = {0} (район = {1}, КД = {2}, год = {3})", row["ID"], GetRegionFromRef(refRegions), GetKDFromRef(refKD), refYearDayUNV.ToString().Substring(0, 4));

            return string.Format("Запись с ID = {0} (КД = {1}, год = {2})", row["ID"], GetKDFromRef(refKD), refYearDayUNV.ToString().Substring(0, 4));
        }


        private DataSet GetDataSetForExport()
        {
            DataSet tmpDataSet = new DataSet();
            // получаем ID записей, которые видны в гриде...
            List<int> visibleRowsIds = UltraGridHelper.GetVisibleRowsIds(drv.newGrid.ugData);
            // получаем фильтр на такие записи
            string[] filter = new string[visibleRowsIds.Count];
            for (int i = 0; i <= visibleRowsIds.Count - 1; i++)
            {
                filter[i] = string.Format("ID = {0}", visibleRowsIds[i]);
            }
            DataRow[] rows = dtNormatives.Select(string.Join(" or ", filter));

            tmpDataSet.Tables.Add(dtNormatives.Clone());
            for (int i = 0; i <= 17; i++)
            {
                string columnName = string.Format("{0}", i);
                if (tmpDataSet.Tables[0].Columns.Contains(columnName))
                    tmpDataSet.Tables[0].Columns.Remove(columnName);
            }

            foreach (DataRow row in rows)
            {
                DataRow addRow = tmpDataSet.Tables[0].NewRow();
                foreach (DataColumn column in tmpDataSet.Tables[0].Columns)
                {
                    addRow[column] = row[column.ColumnName];
                }
                tmpDataSet.Tables[0].Rows.Add(addRow);
            }

            return tmpDataSet;
        }

        DataTable GetXMLDataTable(DataTable normativeTable)
        {
            // получаем копию таблицы с нормативами, удаляем все колонки, в которых нет данных
            DataTable dtExportedNormative = normativeTable.Clone();
            dtExportedNormative.TableName = currentNormatives.ToString();
            for (int i = 0; i <= 17; i++)
            {
                string columnName = string.Format("{0}", i);
                if (dtExportedNormative.Columns.Contains(columnName))
                    dtExportedNormative.Columns.Remove(columnName);
            }
            return dtExportedNormative;
        }

        private DataSet GetDataExportedDataSet()
        {
            DataSet tmpDataSet = new DataSet();
            string filterString = string.Empty;
            List<string> filterConditiion = new List<string>();
            // получим значения фильтров для поля за год
            foreach (FilterCondition filter in drv.newGrid._ugData.DisplayLayout.Bands[0].ColumnFilters["RefYearDayUNV_Lookup"].FilterConditions)
            {
                if (filter.ComparisionOperator == FilterComparisionOperator.Equals)
                {
                    filterConditiion.Add(string.Format("{0} = {1}", "RefYearDayUNV", filter.CompareValue));
                }
            }
            if (filterConditiion.Count > 0)
            {
                filterString = string.Join(" or ", filterConditiion.ToArray());
            }

            DataRow[] rows = dtNormatives.Select(filterString);

            // получаем копию таблицы с нормативами, удаляем все колонки, в которых нет данных
            DataTable dtExportedNormative = GetXMLDataTable(dtNormatives);
            IDatabase db = null;
            DataTable dtKD = CreateNormativesClassifierTable("KD", false);
            DataTable dtRegions = CreateNormativesClassifierTable("Regions", true);

            try
            {
                db = Workplace.ActiveScheme.SchemeDWH.DB;
                // добавляем записи в новую таблицу для экспорта
                // по всем записям собираем источники
                List<int> kdDataSourcesID = new List<int>();
                List<int> regionsSourcesID = new List<int>();

                foreach (DataRow row in rows)
                {
                    DataRow addRow = dtExportedNormative.NewRow();
                    foreach (DataColumn column in dtExportedNormative.Columns)
                    {
                        addRow[column] = row[column.ColumnName];
                    }
                    // получим данные по классификатору КД и районы
                    int refKD = Convert.ToInt32(addRow["RefKD"]);
                    DataRow rowKD = GetKDRow(refKD, kdDataSourcesID, db);
                    if (dtKD.Select(string.Format("ID = {0}", rowKD["ID"])).Length == 0)
                        dtKD.Rows.Add(rowKD.ItemArray);
                    // если есть данные по районам, то получим их тоже
                    if (dtExportedNormative.Columns.Contains("RefRegions"))
                    {
                        int refRegions = Convert.ToInt32(addRow["RefRegions"]);
                        DataRow rowRegions = GetRegionsRow(refRegions, regionsSourcesID, db);
                        if ((rowRegions != null) && (dtRegions.Select(string.Format("ID = {0}", rowRegions["ID"])).Length == 0))
                            dtRegions.Rows.Add(rowRegions.ItemArray);
                    }

                    dtExportedNormative.Rows.Add(addRow);
                }

                DataTable dtKDSources = disintRulesModule.GetSourcesTable(kdDataSourcesID);
                dtKDSources.TableName = "KDSources";
                DataTable dtRegionsSources = disintRulesModule.GetSourcesTable(regionsSourcesID);
                dtRegionsSources.TableName = "RegionsSources";

                tmpDataSet.Tables.Add(dtExportedNormative);
                tmpDataSet.Tables.Add(dtKDSources);
                tmpDataSet.Tables.Add(dtKD);
                tmpDataSet.Tables.Add(dtRegionsSources);
                tmpDataSet.Tables.Add(dtRegions);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return tmpDataSet;
        }

        private static DataTable CreateNormativesClassifierTable(string tableName, bool intCode)
        {
            DataTable dt = new DataTable(tableName);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("SourceID", typeof(int));
            if (intCode)
                dt.Columns.Add("Code", typeof(int));
            else
                dt.Columns.Add("CodeStr", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            return dt;
        }

        private static DataRow GetKDRow(int refKD, List<int> kdSourcesID, IDatabase db)
        {
            string selectQuery = string.Format("select ID, SourceID, CodeStr, Name from d_KD_Analysis where ID = {0}", refKD);
            DataTable dt = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable);
            int sourceID = Convert.ToInt32(dt.Rows[0]["SourceID"]);
            if (!kdSourcesID.Contains(sourceID))
                kdSourcesID.Add(sourceID);
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            return null;
        }

        private static DataRow GetRegionsRow(int refRegions, List<int> regionSourcesID, IDatabase db)
        {
            if (refRegions < 0)
                return null;
            string selectQuery = string.Format("select ID, SourceID, Code, Name from d_Regions_Analysis where ID = {0}", refRegions);
            DataTable dt = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable);
            int sourceID = Convert.ToInt32(dt.Rows[0]["SourceID"]);
            if (!regionSourcesID.Contains(sourceID))
                regionSourcesID.Add(sourceID);
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            return null;
        }

        private static DataTable CreateDataSourceTable()
        {
            DataTable dtSource = new DataTable();
            dtSource.Columns.Add("id", typeof(Int32));
            dtSource.Columns.Add("suppplierCode", typeof(String));
            dtSource.Columns.Add("dataCode", typeof(String));
            dtSource.Columns.Add("dataName", typeof(String));
            dtSource.Columns.Add("kindsOfParams", typeof(String));
            dtSource.Columns.Add("name", typeof(String));
            dtSource.Columns.Add("year", typeof(Int32));
            dtSource.Columns.Add("month", typeof(Int32));
            dtSource.Columns.Add("variant", typeof(String));
            dtSource.Columns.Add("quarter", typeof(Int32));
            dtSource.Columns.Add("territory", typeof(String));
            dtSource.Columns.Add("dataSourceNameValue", typeof(String));
            return dtSource;
        }

        #endregion
    }
}
