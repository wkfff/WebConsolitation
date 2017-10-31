using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.GlobalConstsUI
{
    public partial class GlobalConstsViewObj : BaseViewObj
    {

        private GlobalConstsView vo;

        private DataTable constTable = new DataTable();

        private IGlobalConstsCollection constCollection;

        private GlobalConstsTypes currentConstsType;

        private const string tableName = "GlobalConsts";

        private bool canGoToOthersConsts = true;

        private bool _activeUIElementIsRow = false;

        // нужны картинки всякие...

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.consts_161.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.consts_161; }
        }
        
        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.consts_241; }
        }
        
        protected override void SetViewCtrl()
        {
            fViewCtrl = new GlobalConstsView();
            vo = (GlobalConstsView)fViewCtrl;
        }

        public GlobalConstsViewObj(string key)
            : base(key)
        {
            currentConstsType = GetGlobalConstsTypesFromName(key);

            Caption = GlobalConstsNavigation.GetCaptionFromGlobalConstsTypes(currentConstsType);
        }

        public override void Initialize()
        {
            base.Initialize();

            vo.Text = Caption;

            vo.uexConsts.OnGetGridColumnsState += new GetGridColumnsState(uexConsts_OnGetGridColumnsState);
            vo.uexConsts.OnSaveChanges += new SaveChanges(uexConsts_OnSaveChanges);
            vo.uexConsts.OnCancelChanges += new DataWorking(uexConsts_OnCancelChanges);
            vo.uexConsts.OnAfterRowInsert += new AfterRowInsert(uexConsts_OnAfterRowInsert);
            vo.uexConsts.OnRefreshData += new RefreshData(uexConsts_OnRefreshData);
            vo.uexConsts.OnGridInitializeLayout += new GridInitializeLayout(uexConsts_OnGridInitializeLayout);

            vo.uexConsts.OnLoadFromXML += new SaveLoadXML(uexConsts_OnLoadFromXML);
            vo.uexConsts.OnLoadFromExcel += new SaveLoadXML(uexConsts_OnLoadFromExcel);
            vo.uexConsts.OnSaveToXML += new SaveLoadXML(uexConsts_OnSaveToXML);
            vo.uexConsts.OnSaveToExcel += new SaveLoadXML(uexConsts_OnSaveToExcel);

            vo.uexConsts.OnMouseEnterGridElement += new MouseEnterElement(uexConsts_OnMouseEnterGridElement);
            vo.uexConsts.OnMouseLeaveGridElement += new MouseLeaveElement(uexConsts_OnMouseLeaveGridElement);

            vo.uexConsts.OnClearCurrentTable += new DataWorking(uexConsts_OnClearCurrentTable);

            IGlobalConstsManager constManager = Workplace.ActiveScheme.GlobalConstsManager;

            vo.uexConsts.SingleBandLevelName = "Константа";

            constCollection = constManager.Consts;

            vo.cmsAudit.ItemClicked += new ToolStripItemClickedEventHandler(cmsAudit_ItemClicked);
            vo.uexConsts.ugData.MouseClick += new MouseEventHandler(ugData_MouseClick);
        }

        void ugData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_activeUIElementIsRow)
                {
                    vo.cmsAudit.Show(vo.uexConsts.ugData.PointToScreen(e.Location));
                }
            }
        }

        void cmsAudit_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "аудитToolStripMenuItem")
            {
                vo.cmsAudit.Hide();
				FormAudit.ShowAudit(this.Workplace, "GlobalConsts", ((BaseViewObj)WorkplaceSingleton.Workplace.ActiveContent).Caption,
                        UltraGridHelper.GetActiveID(vo.uexConsts.ugData), AuditShowObjects.RowObject);
            }
        }

        void uexConsts_OnMouseLeaveGridElement(object sender, UIElementEventArgs e)
        {
            
        }

        void uexConsts_OnMouseEnterGridElement(object sender, UIElementEventArgs e)
        {
            UltraGrid ug = (UltraGrid)sender;

            _activeUIElementIsRow = e.Element is RowSelectorUIElement;

            if (_activeUIElementIsRow)
            {
                if (!ug.Focused)
                    ug.Focus();
                return;
            }
        }
        
        void ugData_CellChange(object sender, CellEventArgs e)
        {

           // e.Cell.

        }


        void uebNavi_ActiveItemChanging(object sender, Infragistics.Win.UltraWinExplorerBar.CancelableItemEventArgs e)
        {
            if (this.constTable.GetChanges() != null)
            {
                if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    uexConsts_OnSaveChanges(null);
                }
                else
                {
                    uexConsts_OnCancelChanges(null);
                    canGoToOthersConsts = true;
                }
            }

            e.Cancel = !canGoToOthersConsts;

        }
        
        /// <summary>
        /// обновление данных
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        bool uexConsts_OnRefreshData(object sender)
        {
            constCollection.GetData(currentConstsType, ref constTable);
            vo.uexConsts.DataSource = constTable;

            //ColumnFilter filter = vo.uexConsts.ugData.DisplayLayout.Bands[0].ColumnFilters["CONSTTYPE"];
            //filter.FilterConditions.Add(FilterComparisionOperator.Equals, currentConstsType);

            return true;
        }

        /// <summary>
        /// очистка текущей таблицы
        /// </summary>
        /// <param name="sender"></param>
        void uexConsts_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show("Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                vo.uexConsts.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                try
                {
                    constTable.RejectChanges();
                    
                    foreach (UltraGridRow row in vo.uexConsts.ugData.Rows)
                    {
                        if (!row.IsFilteredOut)
                            row.Delete(false);
                    }
                    uexConsts_OnSaveChanges(null);
                    vo.uexConsts.BurnChangesDataButtons(false);
                }
                finally
                {
                    vo.uexConsts.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                }
            }
        }

        #region импорт констант

        private DataSet CreateDataSet(bool getData)
        {
            DataSet ds = new DataSet("GlobalConstsDataSet");
            DataTable dt = new DataTable("GlobalConstsDataTable");
            foreach(DataColumn column in constTable.Columns)
            {
                dt.Columns.Add(column.ColumnName.ToUpper());
            }

            if (getData)
                DataTableHelper.CopyDataTable(constTable, ref dt);

            ds.Tables.Add(dt);

            return ds;
        }

        bool uexConsts_OnSaveToExcel(object sender)
        {
            string fileName = String.Format("{0} константы", Caption);
            DataSet ds = CreateDataSet(true);
            ExcelExportImportHelper.ExportDataToExcel(ds, uexConsts_OnGetGridColumnsState(null), fileName, this.Workplace, false, true);
            return true;
        }

        bool uexConsts_OnSaveToXML(object sender)
        {
            string fileName = String.Format("{0} константы", Caption);
            DataSet ds = CreateDataSet(true);
            ExportImportHelper.SaveToXML(ds, fileName);
            return true;
        }

        bool uexConsts_OnLoadFromExcel(object sender)
        {
            string fileName = String.Format("{0} константы", Caption);
            
            bool returnValue = false;
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, false, ref fileName))
            {
                DataSet ds = CreateDataSet(false);
                returnValue = ExcelExportImportHelper.ImportFromExcel(ds, string.Empty, string.Empty, uexConsts_OnGetGridColumnsState(null), false, true, fileName, this.Workplace, tableName);
                CopyImportedTable(ds.Tables[0]);
            }
            return returnValue;
        }

        bool uexConsts_OnLoadFromXML(object sender)
        {
            string fileName = String.Format("{0} константы", Caption);
            bool returnValue = false;
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xml, false, ref fileName))
            {
                DataSet ds = CreateDataSet(false);
                returnValue = ExportImportHelper.LoadFromXML(ds, string.Empty, string.Empty, false, tableName, this.Workplace, fileName);
                CopyImportedTable(ds.Tables[0]);
            }
            return returnValue;
        }

        private void CopyImportedTable(DataTable importedTable)
        {
            foreach (DataRow row in importedTable.Rows)
            {
                row["CONSTTYPE"] = (int)currentConstsType;
                if (currentConstsType == GlobalConstsTypes.Custom)
                    if (constCollection.ContainsKey(row["NAME"].ToString()))
                        row["NAME"] = String.Format("Const_{0}", row["ID"]);
            }
            DataTableHelper.CopyDataTable(importedTable, ref constTable);
        }

        #endregion

        void uexConsts_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // добавляем список с выбором категории константы
            //if (currentConstsType != GlobalConstsTypes.Configuration)
            {
                ValueList list = null;
                if (!vo.uexConsts.ugData.DisplayLayout.ValueLists.Exists("ConstCategory"))
                {
                    list = vo.uexConsts.ugData.DisplayLayout.ValueLists.Add("ConstCategory");
                    ValueListItem item = list.ValueListItems.Add("item0");
                    item.DisplayText = "Администрирование (0)";
                    item.DataValue = 0;

                    item = list.ValueListItems.Add("item1");
                    item.DisplayText = "Мониторинг, анализ, прогноз и планирование (1)";
                    item.DataValue = 1;

                    item = list.ValueListItems.Add("item2");
                    item.DisplayText = "Региональные настройки (2)";
                    item.DataValue = 2;
                }
                else
                    list = vo.uexConsts.ugData.DisplayLayout.ValueLists["ConstCategory"];

                vo.uexConsts.ugData.DisplayLayout.Bands[0].Columns["CONSTCATEGORY"].ValueList = list;
                vo.uexConsts.ugData.DisplayLayout.Bands[0].Columns["CONSTCATEGORY"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

                // добавляем список с выбором типа константы
                if (!vo.uexConsts.ugData.DisplayLayout.ValueLists.Exists("ConstValueType"))
                {
                    list = vo.uexConsts.ugData.DisplayLayout.ValueLists.Add("ConstValueType");

                    ValueListItem item = list.ValueListItems.Add("item0");
                    item.DisplayText = "Целый (0)";
                    item.DataValue = 0;

                    item = list.ValueListItems.Add("item1");
                    item.DisplayText = "Вещественный (1)";
                    item.DataValue = 1;

                    item = list.ValueListItems.Add("item2");
                    item.DisplayText = "Символьный (2)";
                    item.DataValue = 2;

                    item = list.ValueListItems.Add("item3");
                    item.DisplayText = "Строковый (3)";
                    item.DataValue = 3;

                    item = list.ValueListItems.Add("item4");
                    item.DisplayText = "Логический (4)";
                    item.DataValue = 4;

                    item = list.ValueListItems.Add("item5");
                    item.DisplayText = "Дата (5)";
                    item.DataValue = 5;

                    item = list.ValueListItems.Add("item6");
                    item.DisplayText = "Дата и время (6)";
                    item.DataValue = 6;
                }
                else
                    list = vo.uexConsts.ugData.DisplayLayout.ValueLists["ConstValueType"];

                vo.uexConsts.ugData.DisplayLayout.Bands[0].Columns["CONSTVALUETYPE"].ValueList = list;
                vo.uexConsts.ugData.DisplayLayout.Bands[0].Columns["CONSTVALUETYPE"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            }
        }

        /// <summary>
        /// добавление некоторых параметров в запись
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="row"></param>
        void uexConsts_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            row.Cells["CONSTTYPE"].Value = (int)currentConstsType;
            // сразу поменяем имя константы на уникальное
            // для вновь добавленных констант получим новое значение ID
            int id = this.constCollection.GetGeneratorValue();
            row.Cells["ID"].Value = id;
            string tmpConstName = String.Format("Const_{0}", id);
            row.Cells["NAME"].Value = tmpConstName;
        }


        /// <summary>
        /// сохранение данных с проверками...
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        bool uexConsts_OnSaveChanges(object sender)
        {
            bool returnValue = false;
            StringBuilder errors = new StringBuilder();
            // проверка на заполненность полей
            if (!CheckFillRows(ref errors))
            {
                MessageBox.Show(errors.ToString(), "Сохранение данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                canGoToOthersConsts = false;
                return false;
            }

            if (!CheckIdentifiers(ref errors))
            {
                MessageBox.Show(errors.ToString(), "Сохранение данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                canGoToOthersConsts = false;
                return false;
            }

            // проверка на уникальность константы
            if (!CheckUniqueConstNames(ref errors))
            {
                MessageBox.Show(errors.ToString(), "Сохранение данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                canGoToOthersConsts = false;
                return false;
            }
            // проверка на совпадение значения константы типу значения
            if (!CheckConstsValues(ref errors))
            {
                MessageBox.Show(errors.ToString(), "Сохранение данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                canGoToOthersConsts = false;
                return false;
            }

            DataTable addConsts = constTable.GetChanges(DataRowState.Added);
            DataTable modifyConsts = constTable.GetChanges(DataRowState.Modified);
            DataTable deleteConsts = constTable.GetChanges(DataRowState.Deleted);

            try
            {
                // все значения заполнены, можно добавлять, изменять и удалять не боясь, что что то пойдет не так

                IGlobalConst globalConst = null;
                // добавляем добавленные константы
                if (addConsts != null)
                {
                    foreach (DataRow row in addConsts.Rows)
                    {
                        globalConst = constCollection.AddNew(Convert.ToInt32(row["ID"]), row["NAME"].ToString());
                        globalConst.Caption = row["CAPTION"].ToString();
                        globalConst.Description = row["DESCRIPTION"].ToString();
                        globalConst.Value = row["VALUE"];
                        globalConst.ConstValueType = (DataAttributeTypes)Convert.ToInt32(row["CONSTVALUETYPE"]);
                        globalConst.ConstCactegory = (GlobalConstCategories)Convert.ToInt32(row["CONSTCATEGORY"]);
                        globalConst.ConstType = (GlobalConstsTypes)Convert.ToInt32(row["CONSTTYPE"]);
                    }
                }
                // изменяем измененные константы
                if (modifyConsts != null)
                    foreach (DataRow row in modifyConsts.Rows)
                    {
                        globalConst = constCollection[row["NAME", DataRowVersion.Original].ToString()];
                        globalConst.Caption = row["CAPTION"].ToString();
                        globalConst.Description = row["DESCRIPTION"].ToString();
                        globalConst.Value = row["VALUE"];
                        globalConst.ConstValueType = (DataAttributeTypes)Convert.ToInt32(row["CONSTVALUETYPE"]);
                        globalConst.ConstCactegory = (GlobalConstCategories)Convert.ToInt32(row["CONSTCATEGORY"]);
                        globalConst.ConstType = (GlobalConstsTypes)Convert.ToInt32(row["CONSTTYPE"]);
                        globalConst.Name = row["NAME"].ToString();
                    }
                // удаляем удаленные константы
                if (deleteConsts != null)
                    foreach (DataRow row in deleteConsts.Rows)
                    {
                        string deleteName = row["NAME", DataRowVersion.Original].ToString();
                        constCollection.Remove(deleteName);
                    }
                // сохраняем изменения
                if ((addConsts != null) || (modifyConsts != null) || (deleteConsts != null))
                {
                    constCollection.ApplyChanges();
                    constTable.AcceptChanges();
                }
                returnValue = true;
                canGoToOthersConsts = true;
            }
            catch
            {
                throw;
            }
            finally
            {

            }
            return returnValue;
        }

        void uexConsts_OnCancelChanges(object sender)
        {
            constTable.RejectChanges();
            constCollection.CancelChanges();
            canGoToOthersConsts = true;
        }


        private Dictionary<string, GridColumnsStates> cashedColumnsSettings = new Dictionary<string, GridColumnsStates>();

        /// <summary>
        /// инициализация грида. Установка заголовков для всех колонок
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        GridColumnsStates uexConsts_OnGetGridColumnsState(object sender)
        {

            if (cashedColumnsSettings.ContainsKey(currentConstsType.ToString()))
            {
                // если да - возвращаем копию
                return cashedColumnsSettings[currentConstsType.ToString()];
            }

            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            state.IsSystem = true;
            states.Add("ID", state);


            state = new GridColumnState();
            if (currentConstsType == GlobalConstsTypes.Custom)
                state.IsHiden = true;
            state.ColumnCaption = "Идентификатор";
            state.ColumnName = "NAME";
            state.ColumnWidth = 150;
            if (!this.Workplace.IsDeveloperMode && (currentConstsType == GlobalConstsTypes.General || currentConstsType == GlobalConstsTypes.Configuration))
                state.IsSystem = true;
            states.Add("NAME", state);

            state = new GridColumnState();
            state.ColumnCaption = "Наименование";
            state.ColumnName = "CAPTION";
            state.ColumnWidth = 150;
            if (!this.Workplace.IsDeveloperMode && (currentConstsType == GlobalConstsTypes.General || currentConstsType == GlobalConstsTypes.Configuration))
                state.IsSystem = true;
            states.Add("CAPTION", state);

            state = new GridColumnState();
            state.ColumnCaption = "Описание";
            state.ColumnName = "DESCRIPTION";
            state.ColumnWidth = 150;
            if (!this.Workplace.IsDeveloperMode && (currentConstsType == GlobalConstsTypes.General || currentConstsType == GlobalConstsTypes.Configuration))
                state.IsSystem = true;
            states.Add("DESCRIPTION", state);

            state = new GridColumnState();
            state.ColumnCaption = "Значение константы";
            state.ColumnName = "VALUE";
            state.ColumnWidth = 150;
            if (!this.Workplace.IsDeveloperMode && (currentConstsType == GlobalConstsTypes.Configuration))
                state.IsSystem = true;
            states.Add("VALUE", state);

            state = new GridColumnState();
            state.ColumnCaption = "Тип значения константы";
            state.ColumnName = "CONSTVALUETYPE";
            state.ColumnWidth = 120;
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            if (!this.Workplace.IsDeveloperMode && (currentConstsType == GlobalConstsTypes.General || currentConstsType == GlobalConstsTypes.Configuration))
                state.IsSystem = true;
            states.Add("CONSTVALUETYPE", state);

            state = new GridColumnState();
            state.ColumnCaption = "Категория константы";
            state.ColumnName = "CONSTCATEGORY";
            state.ColumnWidth = 160;
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            if (!this.Workplace.IsDeveloperMode && (currentConstsType == GlobalConstsTypes.General || currentConstsType == GlobalConstsTypes.Configuration))
                state.IsSystem = true;
            states.Add("CONSTCATEGORY", state);

            state = new GridColumnState();
            //state.ColumnCaption = "Тип константы";
            state.ColumnCaption = string.Empty;
            state.ColumnName = "CONSTTYPE"; 
            state.ColumnWidth = 100;
            state.IsHiden = true;
            states.Add("CONSTTYPE", state);

            cashedColumnsSettings.Add(currentConstsType.ToString(), states);

            return states;
        }

        private enum GridState { ReadOnly, EditOnly, ReadWrite };

        private void SetGridState(GridState gridState)
        {
            switch (gridState)
            {
                case GridState.ReadOnly:
                    vo.uexConsts.AllowAddNewRecords = false;
                    vo.uexConsts.AllowClearTable = false;
                    vo.uexConsts.AllowDeleteRows = false;
                    vo.uexConsts.AllowImportFromXML = false;
                    vo.uexConsts.AllowEditRows = false;
                    break;
                case GridState.EditOnly:
                    vo.uexConsts.AllowAddNewRecords = false;
                    vo.uexConsts.AllowClearTable = false;
                    vo.uexConsts.AllowDeleteRows = false;
                    vo.uexConsts.AllowImportFromXML = false;
                    vo.uexConsts.AllowEditRows = true;
                    break;
                case GridState.ReadWrite:
                    vo.uexConsts.AllowAddNewRecords = true;
                    vo.uexConsts.AllowClearTable = true;
                    vo.uexConsts.AllowDeleteRows = true;
                    vo.uexConsts.AllowImportFromXML = true;
                    vo.uexConsts.AllowEditRows = true;
                    break;
            }
        }

        internal void LoadData()
        {
            switch (currentConstsType)
            {
                case GlobalConstsTypes.Configuration:
                    if (Workplace.IsDeveloperMode)
                        SetGridState(GridState.ReadWrite);
                    else
                        SetGridState(GridState.ReadOnly);
                    Workplace.ViewObjectCaption = "Конфигурационные константы (0)";
                    break;
                case GlobalConstsTypes.General:
                    if (Workplace.IsDeveloperMode)
                        SetGridState(GridState.ReadWrite);
                    else
                        SetGridState(GridState.EditOnly);
                    Workplace.ViewObjectCaption = "Настраиваемые константы (1)";
                    break;
                case GlobalConstsTypes.Custom:
                    SetGridState(GridState.ReadWrite);
                    Workplace.ViewObjectCaption = "Пользовательские константы (2)";
                    break;
            }
            if (WorkplaceSingleton.Workplace.ActiveContent != null)
			    vo.uexConsts.SaveLoadFileName = ((BaseViewObj)WorkplaceSingleton.Workplace.ActiveContent).Caption;
            constCollection.GetData(currentConstsType, ref constTable);

            vo.uexConsts.DataSource = constTable;
        }

        /// <summary>
        /// проверка на допустимость значений всех записей
        /// </summary>
        /// <returns></returns>
        private bool CheckConstsValues(ref StringBuilder errors)
        {
            foreach (DataRow row in constTable.Rows)
            {
                if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                    try
                    {
                        switch ((DataAttributeTypes)Convert.ToInt32(row["CONSTVALUETYPE"]))
                        {
                            case DataAttributeTypes.dtInteger:
                                Convert.ToInt64(row["VALUE"]);
                                break;
                            case DataAttributeTypes.dtDouble:
                                Convert.ToDouble(row["VALUE"]);
                                break;
                            case DataAttributeTypes.dtChar:
                                Convert.ToChar(row["VALUE"]);
                                break;
                            case DataAttributeTypes.dtString:
                                Convert.ToString(row["VALUE"]);
                                break;
                            case DataAttributeTypes.dtBoolean:
                                if ((Convert.ToInt32(row["VALUE"]) != 0) &&
                                    (Convert.ToInt32(row["VALUE"]) != 1))
                                    //Convert.ToBoolean(row["VALUE"]);
                                    throw new Exception();
                                break;
                            case DataAttributeTypes.dtDate:
                            case DataAttributeTypes.dtDateTime:
                                Convert.ToDateTime(row["VALUE"]);
                                break;
                        }
                    }
                    catch
                    {
                        //errors.AppendLine(String.Format("Константа '{0}' имеет значение, не соответствующее формату", row["CAPTION"]));
                        errors.AppendLine(String.Format("Ошибка при сохранении записи ID = {0} '{1}': Константа имеет значение, не соответствующее формату",
                                row["ID"], row["CAPTION"]));
                    }
            }
            return (errors.Length == 0);
        }

        
        private bool CheckIdentifiers(ref StringBuilder errors)
        {

            List<string> correctSympols = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                correctSympols.Add(c.ToString());
            }
            for (int i = 0; i <= 9; i++)
            {
                correctSympols.Add(i.ToString());
            }
            correctSympols.Add("_");

            foreach (DataRow row in constTable.Rows)
            {
                if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                {
                    string name = row["NAME"].ToString();
                    foreach(char symbol in name)
                    {
                        if (!correctSympols.Contains(symbol.ToString().ToUpper()))
                        {
                            //errors.AppendLine(String.Format("Идентификатор константы  c ID = {0} содержит некорректные символы", row["ID"]));
                            errors.AppendLine(String.Format("Ошибка при сохранении записи ID = {0} '{1}': Идентификатор '{2}' константы содержит недопустимые символы (разрешены английские буквы, цифры и символ подчеркивания)",
                                row["ID"], row["CAPTION"], row["NAME"]));
                            break;
                        }
                    }
                }
            }
            correctSympols.Clear();
            return (errors.Length == 0);
        }
        

        /// <summary>
        /// проверяет, заполнены ли все поля в записях
        /// </summary>
        /// <returns></returns>
        private bool CheckFillRows(ref StringBuilder errors)
        {
            foreach (DataRow row in constTable.Rows)
            {
                if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                { 
                    foreach (DataColumn column in constTable.Columns)
                    {
                        if ((column.ColumnName != "ID") && (row[column.ColumnName] == null || row[column.ColumnName] == DBNull.Value))
                        {
                            errors.AppendLine(String.Format("Ошибка при сохранении записи ID = {0} '{1}': Поле '{2}' константы не заполнено",
                                row["ID"], row["CAPTION"], vo.uexConsts.ugData.DisplayLayout.Bands[0].Columns[column.ColumnName].Header.Caption));
                            //errors.AppendLine(String.Format("Поле '{0}' константы  c ID = {1} не заполнено",
                                //vo.uexConsts.ugData.DisplayLayout.Bands[0].Columns[column.ColumnName].Header.Caption, row["ID"]));
                        }
                    }
                }
            }
            return (errors.Length == 0);
        }

        private bool CheckUniqueConstNames(ref StringBuilder errors)
        {
            foreach (DataRow row in constTable.Rows)
            {
                if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                {
                    string name = row["NAME"].ToString();
                    string caption = row["CAPTION"].ToString();
                    // проверка на уникальность идентификаторов
                    if (this.constCollection.ContainsKey(name))
                    {
                        int constID = this.constCollection[name].ID;
                        if (constID != Convert.ToInt32(row["ID"]))
                            errors.AppendLine(String.Format("Ошибка при сохранении записи ID = {0} '{1}': Константа с идентификатором '{2}' уже существует в коллекции", row["ID"], row["CAPTION"], row["NAME"]));
                        //errors.AppendLine(String.Format("Константа с идентификатором '{0}' уже существует в коллекции", row["NAME"]));
                    }
                    // проверка на уникальность русских наименований
                    int id = -1;
                    if (this.constCollection.ContainCaption(caption, ref id))
                    {
                        if (id != Convert.ToInt32(row["ID"]))
                            errors.AppendLine(String.Format("Ошибка при сохранении записи ID = {0} '{1}': Константа с наименованием '{1}' уже существует в коллекции", row["ID"], row["CAPTION"]));
                            //errors.AppendLine(String.Format("Константа с наименованием '{0}' уже существует в коллекции", row["CAPTION"]));
                    }
                }
            }
            return (errors.Length == 0);
        }

        public override void InternalFinalize()
        {
            uexConsts_OnCancelChanges(null);
        }

        internal static GlobalConstsTypes GetGlobalConstsTypesFromName(string name)
        {
            switch (name)
            {
                case GlobalConstsKeys.General:
                    return GlobalConstsTypes.General;
                case GlobalConstsKeys.Configuration:
                    return GlobalConstsTypes.Configuration;
                case GlobalConstsKeys.Custom:
                    return GlobalConstsTypes.Custom;
                default:
                    throw new ArgumentException(String.Format("Неизвестный элемент перечисленяи: \"{0}\"", name));
            }
        }

        private void LinqTest()
        {
            List<int> list = new List<int>();
            list.AddRange(new int[] { 1, 2, 3, 4 });
            foreach (var str in list.Select(s => s.ToString()))
            {
                MessageBox.Show(str);
            }
        }
    }
}
