using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Infragistics.Excel;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Reflection;

namespace Krista.FM.Client.Components
{
    public partial class UltraGridEx : UserControl, Infragistics.Win.IUIElementCreationFilter
    {
        EventViewer eventViewer = null;

        #region Обработчик тулбара

        /// <summary>
        /// Обработчик нажатия на кнопки в тулбаре
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _utmMain_ToolClick(object sender, ToolClickEventArgs e)
        {
            // если нажатие произошло по горячей клавише, а сама кнопка не видна, выходим
            if (!e.Tool.SharedProps.Visible) return;
            CloseLookupEditor();
            string action = e.Tool.Key;
            PerfomAction(action);
        }

        /// <summary>
        /// Действия, выполняемые на тулбаре
        /// </summary>
        /// <param name="actionKey">ключ операции. совпадает с ключом кнопки на тулбаре</param>
        /// <returns></returns>
        public bool PerfomAction(string actionKey)
        {
            // если кнопки с указанным ключом - действием нет, выходим
            if (!utmMain.Tools.Exists(actionKey))
                return false;
            // практически все действия не работают в режиме группировки по какой либо колонке
            if ((actionKey == "SaveChange") ||
                 (actionKey == "ImportFromXML") ||
                 (actionKey == "ShowHierarchy") ||
                 actionKey == "CopyRow" ||
                 actionKey == "PasteRow" ||
                 actionKey == "DeleteSelectedRows")
                if (GridInGroupByMode(true))
                    return false;
            
            ToolBase tool = utmMain.Tools[actionKey];
            switch (actionKey)
            {
                case "Refresh":
                    // для таблиц фактов сохраняем настройки фильтра колонок
                    RefreshData();
                    // очистим коллекцию изменений
                    //ChangedRows.Clear();
                    break;
                case "SaveChange":
                    isSaveChanges = true;
                    // 
                    if (ugData.ActiveCell != null)
                        if (ugData.ActiveCell.IsInEditMode)
                            ugData.ActiveCell = ugData.ActiveRow.Cells[0];
                    if (!isSaveChanges) break;

                    if (_ugData.ActiveRow != null)
                        _ugData.ActiveRow.Update();

                    if (_onSaveChanges != null)
                        isSaveChanges = _onSaveChanges(this);

                    if (isSaveChanges)
                    {
                        // только если данные нормально сохранились, то удаляем все картинки
                        RejectImages();
                        // и зажигаем кнопки
                        BurnChangesDataButtons(false);
                    }
                    SetActiveRowState();
                    break;
                case "CancelChange":
                    _ugData.PerformAction(UltraGridAction.ExitEditMode);

                    if (_onCancelChanges != null)
                        _onCancelChanges(this);

                    if (_ugData.ActiveCell != null)
                        _ugData.ActiveCell.CancelUpdate();
                    if (_ugData.ActiveRow != null)
                        _ugData.ActiveRow.Refresh(RefreshRow.FireInitializeRow, false);

                    RejectImages();

                    if (HierarchyEnabled)
                    {
                        if (HierarchyInfo.CurViewState == ViewState.Hierarchy)
                            SetHierarchyFilter(FilterConditionAction.Refresh);
                        else
                            SetHierarchyFilter(FilterConditionAction.Clear);
                    }
                    BurnChangesDataButtons(false);
                    break;
                case "ClearCurrentTable":
                    ClearCurrentData();
                    ClearInnerCollections();
                    break;
                // обрабатываем выгрузку данных в XML содержимого источника данных, работает только для DataSet
                case "ExportToXML":
                    if (_OnSaveToXML != null)
                        _OnSaveToXML(this);
                    break;
                case "SaveDataSetXML":
                    if (_OnSaveToDataSetXML != null)
                        _OnSaveToDataSetXML(this);
                    break;
                // обрабатываем загрузку данных из XML, работает только для DataSet
                // для убыстрения процесса если грид находится в иерархическом виде переводим его в плоский вид
                case "ImportFromXML":
                    //if (_dataSourceType == typeof(DataSet))
                    {
                        Debug.Print("Import from XML started");
                        // количество записей в таблице
                        int gridRowsCount = 0;
                        // количество записей в гриде до выгрузки из XML
                        int dataTableRowCount = 0;
                        if (_ugData.Rows.Count > 0)
                            gridRowsCount = _ugData.Rows.Count;
                        if (_dataSource is DataSet)
                        {
                            if (((DataSet)_dataSource).Tables[0].Rows.Count > 0)
                                dataTableRowCount = ((DataSet)_dataSource).Tables[0].Rows.Count;
                        }
                        else
                        {
                            if (((DataTable)_dataSource).Rows.Count > 0)
                                dataTableRowCount = ((DataTable)_dataSource).Rows.Count;
                        }

                        bool isAddRows = false;
                        // вызов загрузки данных

                        if (_OnLoadFromXML != null)
                        {
                            Debug.Print("Call method ExportImportHelper.LoadFromXml");
                            isAddRows = _OnLoadFromXML(this);
                        }

                        // если данные были загружены, выполняем над ними какие то действия
                        if (isAddRows)
                        {
                            Debug.Print("Load data successfully");
                            // переведем грид и датсет в плоский режиm

                            refreshData = false;
                            if (((StateButtonTool)_utmMain.Tools["ShowHierarchy"]).SharedProps.Enabled)
                            {
                                Debug.Print("Turn grid to flat display mode");
                                ((StateButtonTool)_utmMain.Tools["ShowHierarchy"]).Checked = false;
                            }
                            refreshData = true;

                            ugData.BeginUpdate();
                            // ставим знак добавления на все добавленные записи

                            // выполняем возможные действия над данными после их загрузки

                            if (_OnAftertImportFromXML != null)
                            {
                                Debug.Print("Call processing data from XML");
                                _OnAftertImportFromXML(this, dataTableRowCount);
                            }

                            foreach (UltraGridBand band in ugData.DisplayLayout.Bands)
                            {
                                band.Columns["ID"].SortIndicator = SortIndicator.Ascending;
                            }

                            // ставим значки добавленных записей
                            Debug.Print("Set 'add' sign to added rows in grid");
                            for (int i = gridRowsCount; i <= _ugData.Rows.Count - 1; i++)
                            {
                                SetRowAppearance(_ugData.Rows[i], LocalRowState.Added);
                            }
                            if (sortColumnName != string.Empty)
                                foreach (UltraGridBand band in ugData.DisplayLayout.Bands)
                                {
                                    band.Columns[sortColumnName].SortIndicator = SortIndicator.Ascending;
                                }
                            ugData.PerformAction(UltraGridAction.ExitEditMode);
                            ugData.EndUpdate();
                            BurnChangesDataButtons(true);

                            if (HierarchyEnabled)
                                SetHierarchyFilter(FilterConditionAction.Clear);
                        }
                        Debug.Print("Import from XML finished successfully");
                    }
                    break;
                case "SaveToExcel":
                    // сохраняем в екселе содержимое грида, работает с любым видом грида и источника данных
                    string fileName = _SaveLoadFileName != string.Empty ?
                         _SaveLoadFileName + "_отчет" :
                        "noname_отчет";
                    fileName = GetCorrectFileName(fileName + ".xls");
                    var path = Application.StartupPath + @"\Reports";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    fileName = path + @"\" + fileName;
                    var excelExpoter = new UltraGridExcelExporter();
                    excelExpoter.FileLimitBehaviour = FileLimitBehaviour.TruncateData;
                    excelExpoter.Export(_ugData, fileName);
                    excelExpoter.Dispose();
                    Process.Start(fileName);
                    /*
                    if (GetFileName(fileName, fileExtensions.xls, true, ref fileName))
                    { 
                        var excelExpoter = new UltraGridExcelExporter();
                        excelExpoter.FileLimitBehaviour = FileLimitBehaviour.TruncateData;
                        excelExpoter.Export(_ugData, fileName);
                        excelExpoter.Dispose();
                        Process.Start(fileName);
                    }*/
                    break;
                // импорт из Excelя
                case "excelImport":
                    if (_onLoadFromExcel != null)
                    {
                        // количество записей в таблице
                        int gridRowsCount = 0;
                        // количество записей в гриде до выгрузки из XML
                        int dataTableRowCount = 0;
                        if (_ugData.Rows.Count > 0)
                            gridRowsCount = _ugData.Rows.Count;

                        if (_dataSource is DataSet)
                        {
                            if (((DataSet)_dataSource).Tables[0].Rows.Count > 0)
                                dataTableRowCount = ((DataSet)_dataSource).Tables[0].Rows.Count;
                        }
                        else
                            if (((DataTable)_dataSource).Rows.Count > 0)
                                dataTableRowCount = ((DataTable)_dataSource).Rows.Count;

                        if (_onLoadFromExcel(this))
                        {
                            if (_OnAftertImportFromXML != null)
                                _OnAftertImportFromXML(this, dataTableRowCount);

                            // ставим значки добавленных записей
                            for (int i = gridRowsCount; i <= _ugData.Rows.Count - 1; i++)
                            {
                                SetRowAppearance(_ugData.Rows[i], LocalRowState.Added);
                            }

                            foreach (UltraGridBand band in ugData.DisplayLayout.Bands)
                            {
                                band.Columns["ID"].SortIndicator = SortIndicator.Ascending;
                            }

                            if (sortColumnName != string.Empty)
                                foreach (UltraGridBand band in ugData.DisplayLayout.Bands)
                                {
                                    band.Columns[sortColumnName].SortIndicator = SortIndicator.Ascending;
                                }
                            ugData.PerformAction(UltraGridAction.ExitEditMode);
                            ugData.EndUpdate();
                            BurnChangesDataButtons(true);
                        }
                    }
                    break;
                // экспорт в Excel
                case "excelExport":
                    if (_onSaveToExcel != null)
                        _onSaveToExcel(this);
                    break;
                // обработка изменения вида грида (плоский - иерархический)
                case "ShowHierarchy":
                    // если иерархия не будет устанавливаться, то выходим
                    if (isChangeHierarchyView)
                    {
                        if (_beforeHierarcyChange != null)
                            _beforeHierarcyChange(this);

                        ButtonTool btnSearchText = (ButtonTool)utmMain.Tools["btnSearchText"];
                        isChangeHierarchyView = false;
                        // переключаем текущий вид
                        if (HierarchyInfo.CurViewState == ViewState.Hierarchy)
                        {
                            HierarchyInfo.CurViewState = ViewState.Flat;
                            // если классификатор был большой, то грузим его полностью
                            if (HierarchyInfo.loadMode == LoadMode.OnParentExpand)
                            {
                                HierarchyInfo.loadMode = LoadMode.AllRows;
                                if (refreshData)
                                {
                                    if (_onRefreshData != null)
                                        _onRefreshData(this);
                                }
                                else
                                    SetGridDataSource(_dataSource, false);
                                HierarchyInfo.loadMode = LoadMode.OnParentExpand;
                            }
                            else
                                SetGridDataSource(_dataSource, false);
                            ((StateButtonTool)tool).Checked = false;
                            btnSearchText.SharedProps.Visible = true;
                        }
                        else
                        {
                            HierarchyInfo.CurViewState = ViewState.Hierarchy;
                            SetGridDataSource(_dataSource, false);
                            btnSearchText.SharedProps.Visible = false;
                        }
                        if (_OnChangeHierarchyView != null)
                            _OnChangeHierarchyView(this, HierarchyInfo.CurViewState == ViewState.Hierarchy);
                        
                        isChangeHierarchyView = true;

                        if (_afterHierarcyChange != null)
                            _afterHierarcyChange(this);

                        // обновляем фильтр
                        SetAddCaptionButtons(HierarchyInfo);

                        if (HierarchyInfo.CurViewState == ViewState.Flat)
                        {
                            if (!string.IsNullOrEmpty(HierarchyInfo.ParentRefClmnName))
                            {
                                SetHierarchyFilter(FilterConditionAction.Clear);
                                ugData.DisplayLayout.Bands[0].Columns[HierarchyInfo.ParentRefClmnName].Hidden = false;
                            }
                        }
                        else
                        {
                            SetHierarchyFilter(FilterConditionAction.Refresh);
                            foreach(UltraGridBand band in ugData.DisplayLayout.Bands)
                            {
                                band.Columns[HierarchyInfo.ParentRefClmnName].Hidden = true;
                            }
                        }

                        #region Восстановление значков у всех измененнных записей при смене вида грида

                        foreach (KeyValuePair<int, List<ChangedUltraGridRow>> kvp in changes)
                        {
                            if (HierarchyInfo.CurViewState == ViewState.Hierarchy)
                            {
                                UltraGridRow parentRow = null;
                                if (kvp.Key != -1)
                                    parentRow = UltraGridHelper.FindGridRow(ugData, "ID", kvp.Key.ToString());

                                foreach (ChangedUltraGridRow changedRow in kvp.Value)
                                {
                                    UltraGridRow row = changedRow.FindRow(this, parentRow);
                                    if (row != null)
                                    {
                                        SetRowStateImage(row, changedRow.RowState);
                                        ChangedRows.Add(row);
                                    }
                                }
                            }
                            else
                            {
                                foreach (ChangedUltraGridRow changedRow in kvp.Value)
                                {
                                    UltraGridRow row = changedRow.FindRow(this, null);
                                    {
                                        if (row != null)
                                        {
                                            SetRowStateImage(row, changedRow.RowState);
                                            ChangedRows.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                        if (changes.Count > 0)
                        {
                            BurnChangesDataButtons(true);
                        }

                        #endregion
                    }

                    break;

                case "btnVisibleAddButtons":
                    if (IsReadOnly || !AllowAddNewRecords)
                        return false;
                    StateButtonTool btn = (StateButtonTool)utmMain.Tools["btnVisibleAddButtons"];
                    ugData.DisplayLayout.AddNewBox.Hidden = !btn.Checked;
                    btn.SharedProps.ToolTipText = btn.Checked ?
                        "Скрыть кнопки добавления записей" :
                        "Показать кнопки добавления записей";
                    break;

                case "DeleteSelectedRows":
                    if (_onDeleteDataSource != null)
                    {
                        _onDeleteDataSource(this);
                        return true;
                    }
                    if (ugData.Rows.Count <= 0)
                        return false;
                    ugData.PerformAction(UltraGridAction.DeleteRows);
                    break;
                case "FirstRow":
                    UltraGridHelper.MoveTo(ugData, SiblingRow.First);
                    break;
                case "PrevRow":
                    UltraGridHelper.MoveTo(ugData, SiblingRow.Previous);
                    break;
                case "NextRow":
                    UltraGridHelper.MoveTo(ugData, SiblingRow.Next);
                    break;
                case "LastRow":
                    UltraGridHelper.MoveTo(ugData, SiblingRow.Last);
                    break;
                // устанавливаем видимость всех колонок, кнопки которых находятся в меню
                case "SystemMenu":
                case "ServiceMenu":
                    PopupMenuTool menu = (PopupMenuTool)tool;
                    foreach (ToolBase btn1 in menu.Tools)
                    {
                        ((StateButtonTool)btn1).Checked = menu.Checked;
                    }
                    break;
                case "ShowGroupBy":
                    if (!_ugData.DisplayLayout.GroupByBox.Hidden)
                    {
                        if (GridInGroupByMode(false))
                        {
                            if (MessageBox.Show("Грид находится в режиме группировки. Группировка будет очищена", "Скрытие Области группировки", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                _ugData.DisplayLayout.GroupByBox.Hidden = true;
                                ((StateButtonTool)tool).SharedProps.ToolTipText = "Показать область группировки";
                                _ugData.DisplayLayout.Bands[0].SortedColumns.Clear();
                            }
                        }
                        else
                        {
                            _ugData.DisplayLayout.GroupByBox.Hidden = true;
                            ((StateButtonTool)tool).SharedProps.ToolTipText = "Показать область группировки";
                        }
                    }
                    else
                    {
                        _ugData.DisplayLayout.GroupByBox.Hidden = false;
                        ((StateButtonTool)tool).SharedProps.ToolTipText = "Скрыть область группировки";
                    }
                    break;
                case "ShowFilter":
                    if (!ServerFilterEnabled) 
                        return false;
                    if (ugFilter.Visible)
                    {
                        if (ServerFilterPresent())
                        {
                            if (MessageBox.Show("Имеется настроенный фильтр. Фильтр будет очищен", "Скрытие фильтра", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                ResetServerFilter();
                                InitServerFilter();
                                ugFilter.Visible = false;
                                ((StateButtonTool)tool).SharedProps.ToolTipText = "Пока зать фильтр";
                                BurnRefreshDataButton(true);
                            }
                        }
                        else
                        {
                            ugFilter.Visible = false;
                            ((StateButtonTool)tool).SharedProps.ToolTipText = "Показать фильтр";
                        }
                    }
                    else
                    {
                        ugFilter.Visible = true;
                        ((StateButtonTool)tool).SharedProps.ToolTipText = "Скрыть фильтр";
                    }
                    break;
                case "CopyRow":
                    CopyRow();
                    break;
                case "PasteRow":
                    if (ugData.ActiveCell != null && ugData.ActiveCell.IsInEditMode)
                        return false;
                    PasteRow();
                    break;
                case "EnterExitEditMode":
                    EnterExitEditMode();
                    break;
                case "btnSearchText":
                    SearchText();
                    break;
                case "ClearFilter":
                    ClearChildrenFilters(ugData.Rows);
                    break;
                default:
                    // кнопка установки видимости колонок
                    if (tool is StateButtonTool)
                    {
                        // не даем скрывать колонку если включен режим серверного фильтра и на этой колонке есть фильтр
                        if (!CanHideColumn(tool.Key))
                        {
                            MessageBox.Show("Колонка входит в фильтр выборки данных", "Невозможно скрыть колонку", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            // включаем галочку на кнопке обратно
                            _utmMain.EventManager.SetEnabled(Infragistics.Win.UltraWinToolbars.EventGroups.AllEvents, false);
                            ((StateButtonTool)tool).Checked = !((StateButtonTool)tool).Checked;
                            _utmMain.EventManager.SetEnabled(Infragistics.Win.UltraWinToolbars.EventGroups.AllEvents, true);
                            break;
                        }
                        // для основного грида
                        // получаем настройки для колонок
                        GridColumnsStates states = GridColumnsStates;
                        string sourceColumnName = GetSourceColumnName(tool.Key);
                        if (states != null)
                        {
                            if (!states.ContainsKey(sourceColumnName))
                            {
                                if (_toolClick != null)
                                    _toolClick(this, new ToolClickEventArgs(tool, new ListToolItem(tool.Key)));
                                break;
                            }
                            GridColumnState state = states[sourceColumnName];
                            state.IsHiden = !((StateButtonTool)tool).Checked;
                            foreach (UltraGridBand band in _ugData.DisplayLayout.Bands)
                            {
                                if (band.Columns.Exists(tool.Key))
                                {
                                    band.Columns[tool.Key].Hidden = state.IsHiden;
                                    if (_onAfterColumnHideShow != null)
                                        _onAfterColumnHideShow(this, state.ColumnName, state.IsHiden);
                                }
                            }
                        }
                        // для фильтра если он есть
                        if (ServerFilterEnabled)
                        {
                            foreach (UltraGridBand band in ugFilter.DisplayLayout.Bands)
                            {
                                if (band.Columns.Exists(GetSourceColumnName(tool.Key)))
                                {
                                    UltraGridColumn clmn = band.Columns[GetSourceColumnName(tool.Key)];
                                    clmn.Hidden = !((StateButtonTool)tool).Checked;
                                }
                            }
                        }
                    }
                    else
                        if (_toolClick != null)
                            _toolClick(this, new ToolClickEventArgs(tool, new ListToolItem(tool.Key)));
                    break;
            }
            CheckCopyRowButtonVisible();
            return true;
        }

        #endregion

        private void ClearInnerCollections()
        {
            ChangedRows.Clear();
            changes.Clear();
        }

        #region копирование записей

        // структуры для хранения информации по копируемой записи
        //private Dictionary<string, object> copyRowValues;
        private List<Dictionary<string, object>> copyRows;
        private int copyRowLevel;

        /// <summary>
        /// событие на копирование записи
        /// </summary>
        private RefreshData _onCopyRow = null;
        public event RefreshData OnCopyRow
        {
            add { _onCopyRow += value; }
            remove { _onCopyRow -= value; }
        }

        // событие на вставку записи
        private RefreshData _onPasteRow = null;
        public event RefreshData OnPasteRow
        {
            add { _onPasteRow += value; }
            remove { _onPasteRow -= value; }
        }

        private void CheckCopyRowButtonVisible()
        {
            utmMain.Tools["CopyRow"].SharedProps.Enabled = AllowAddNewRecords && ugData.ActiveRow != null && !IsReadOnly;
        }

        /// <summary>
        /// копирование активной записи
        /// </summary>
        private void CopyRow()
        {
            bool isCopyRow = true;

            if (_onCopyRow != null)
            {
                isCopyRow = _onCopyRow(_ugData);
                utmMain.Tools["PasteRow"].SharedProps.Enabled = true;
            }
            if (!isCopyRow) return;

            if (_ugData.ActiveRow == null)
                return;

            if (this.ugData.Selected.Rows.Count == 0)
                _ugData.ActiveRow.Selected = true;
            copyRows = new List<Dictionary<string, object>>();
            foreach (UltraGridRow row in this.ugData.Selected.Rows)
            {
                Dictionary<string, object> copyRowValues = new Dictionary<string, object>();
                UltraGridRow tmpRow = UltraGridHelper.GetRowCells(row);
                copyRowLevel = tmpRow.Band.Index;
                foreach (UltraGridCell cell in tmpRow.Cells)
                {
                    if (cell.Column.Key != "ID")
                        copyRowValues.Add(cell.Column.Key, cell.Value);
                }
                copyRows.Add(copyRowValues);
            }
            utmMain.Tools["PasteRow"].SharedProps.Enabled = true;
        }

        /// <summary>
        /// вставка скопированной записи
        /// </summary>
        private void PasteRow()
        {
            bool isPasteRow = true;
            if (_onPasteRow != null)
            {
                isPasteRow = _onPasteRow(_ugData);
            }
            if (!isPasteRow) return;

            GridColumnsStates states = GridColumnsStates;
            //ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                foreach (Dictionary<string, object> copyRowValues in copyRows)
                {
                    UltraGridRow row = _ugData.DisplayLayout.Bands[copyRowLevel].AddNew();
                    foreach (UltraGridCell cell in row.Cells)
                    {
                        if (states.ContainsKey(cell.Column.Key) && states[GetSourceColumnName(cell.Column.Key)].ColumnType == ColumnType.Standart)
                                cell.Value = copyRowValues[cell.Column.Key];
                    }
                    row.Update();
                }
            }
            finally
            {
                //ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        #endregion

        #region очистка фильтров

        private void ClearChildrenFilters(Infragistics.Win.UltraWinGrid.RowsCollection RowsCollection)
        {
            // clear the filter for this rows collection 
            RowsCollection.ColumnFilters.ClearAllFilters();

            // восстановим фильтр на иерархию
            if (HierarchyEnabled)
            {
                if (HierarchyInfo.CurViewState == ViewState.Hierarchy)
                    SetHierarchyFilter(FilterConditionAction.Refresh);
                else
                    SetHierarchyFilter(FilterConditionAction.Clear);
            }

            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in RowsCollection)
            {
                if (!row.IsFilteredOut && row.HasChild())
                    // this line assumes that you only have one child band per parent
                    // this code would need to be modified if you have have multiple child bands
                    //' per parent band
                    // ClearChildrenFilters(row.ChildBands(0).Rows)

                    // if you have multiple child bands then you would need something like this
                    //                 
                    foreach (Infragistics.Win.UltraWinGrid.UltraGridChildBand band in row.ChildBands)
                        ClearChildrenFilters(band.Rows);
            }
        }        

        #endregion

        private bool inDebugMode = false;
        public bool InDebugMode
        {
            get { return inDebugMode; }
            set
            {
                inDebugMode = value;
                if (inDebugMode)
                {
                    eventViewer = EventViewer.StartEventTracking(ugData);
                }
                else
                {
                    if (eventViewer != null)
                    {
                        eventViewer.StopEventTracking();
                        eventViewer.Dispose();
                    }
                }
            }
        }
    }
}
