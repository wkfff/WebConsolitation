using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

using System.Linq;

namespace Krista.FM.Client.Components
{
    //[Designer("Infragistics.Win.UltraWinGrid.Design.UltraGridDesigner")]
    public partial class UltraGridEx : UserControl, Infragistics.Win.IUIElementCreationFilter
    {
        public const string StateColumnName = "pic";
        public const string REMASKED_COLUMN_POSTFIX = "_Remasked";
        public const string LOOKUP_COLUMN_POSTFIX = "_Lookup";
        public const string DocumentExtension_Column_Poctfix = "_DocumentExtension";

        public static string GetSourceColumnName(string columnName)
        {
            return columnName.Replace(REMASKED_COLUMN_POSTFIX, String.Empty).
                              Replace(LOOKUP_COLUMN_POSTFIX, String.Empty).
                              Replace(DocumentExtension_Column_Poctfix, String.Empty);
        }

        public UltraGridEx()
        {
            InitializeComponent();
            InDebugMode = false;
            _ugData.BeforeRowExpanded += new CancelableRowEventHandler(BeforeRowExpanded);
            _ugData.AfterRowCollapsed += new RowEventHandler(_ugData_AfterRowCollapsed);
            // создаем объекты работы с колонками в гриде
            ClsColumns = new StateButtonsManager(((PopupMenuTool)_utmMain.Tools["ColumnsVisible"]));
            // добавим картинки в список, потом их там будем использовать
            il.Images.Add(Properties.Resources.Edited);
            il.Images.Add(Properties.Resources.Deleted);
            il.Images.Add(Properties.Resources.Inserted);
            Groups = new Dictionary<string, string>();
        }

        #region Свойства группы Internal Controls
        /// <summary>
        /// Самая нижняя панель, на случай внедрения куда-нибудь в рантайме
        /// </summary>
        [Category("Internal controls")]
        [Description("Самая нижняя панель")]
        public Panel pnTemplate
        {
            get { return _pnTemplate; }
        }

        /// <summary>
        /// ToolbarsManager для настройки собственных тулбаров
        /// </summary>
        [Category("Internal controls")]
        [Description("Объект для манипуляции с тулбарами")]
        public UltraToolbarsManager utmMain
        {
            get { return _utmMain; }
        }

        /// <summary>
        /// ToolbarsManager для настройки собственных тулбаров
        /// </summary>
        [Category("Internal controls")]
        [Description("Объект с календарем")]
        public MonthCalendar Calendar
        {
            get { return monthCalendar1; }
        }

        /// <summary>
        /// Грид с данными
        /// </summary>
        [Category("Internal controls")]
        [Description("Грид")]
        public UltraGrid ugData
        {
            get { return _ugData; }
        }
        #endregion

        #region установки состояния компонента

        private bool _stateRowEnable;
        /// <summary>
        /// доступность колонки состояния записи в гриде
        /// </summary>
        [Category("Object state")]
        [Description("Показывает, создавать или нет в гриде колонку, в которой отображается состояние записи")]
        [ReadOnly(false)]
        [DefaultValue(true)]
        public bool StateRowEnable
        {
            get { return _stateRowEnable; }
            set
            {
                _stateRowEnable = value;
                SetRowStateColumnVisible(_stateRowEnable);
            }
        }

        private bool _isReadOnly;
        /// <summary>
        /// Режим работы (доступность или нет для редактирования)
        /// </summary>
        [Category("Object state")]
        [Description("Режим работы (доступен/недоступен для редактирования)")]
        [ReadOnly(false)]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                SetComponentSettings();
            }
        }

        private bool _AllowAddNewRecords = true;
        /// <summary>
        /// устанавливает, можно ли добавлять новые записи в источник
        /// </summary>
        [Category("Object state")]
        [Description("Режим работы (доступно/недоступно добавлять записи вручную)")]
        [ReadOnly(false)]
        [DefaultValue(false)]
        public bool AllowAddNewRecords
        {
            get { return _AllowAddNewRecords; }
            set
            {
                _AllowAddNewRecords = value;
                SetAllowEditRows();
                SetSaveCancelDataButtons();
                SetAddButtonsVisible();
            }
        }

        private bool _allowClearTable = true;
        /// <summary>
        /// устанавливает, можно ли очищать все данные из грида
        /// </summary>
        [Category("Object state")]
        [Description("Режим работы (доступно/недоступно очищение данных в гриде)")]
        [ReadOnly(false)]
        [DefaultValue(false)]
        public bool AllowClearTable
        {
            get { return _allowClearTable; }
            set
            {
                _allowClearTable = value;
                SetClearTableButtonVisible();
            }
        }

        private bool _allowDeleteRows = true;
        /// <summary>
        /// устанавливает, можно ли удалять записи из грида
        /// </summary>
        [Category("Object state")]
        [Description("Режим работы (доступно/недоступно удаление записей из грида)")]
        [ReadOnly(false)]
        [DefaultValue(true)]
        public bool AllowDeleteRows
        {
            get { return _allowDeleteRows; }
            set
            {
                _allowDeleteRows = value;
                SetAllowDeleteRows();
                SetSaveCancelDataButtons();
            }
        }

        private bool _allowEditRows = true;
        /// <summary>
        /// устанавливает, можно ли удалять записи из грида
        /// </summary>
        [Category("Object state")]
        [Description("Режим работы (доступно/недоступно редактирование записей в грида)")]
        [ReadOnly(false)]
        [DefaultValue(true)]
        public bool AllowEditRows
        {
            get { return _allowEditRows; }
            set
            {
                _allowEditRows = value;
                SetAllowEditRows();
                SetSaveCancelDataButtons();
            }
        }

        private bool _allowImportFromXML = true;
        /// <summary>
        /// устанавливает, можно ли удалять записи из грида
        /// </summary>
        [Category("Object state")]
        [Description("Режим работы (доступно/недоступно редактирование записей в грида)")]
        [ReadOnly(false)]
        [DefaultValue(true)]
        public bool AllowImportFromXML
        {
            get { return _allowImportFromXML; }
            set
            {
                _allowImportFromXML = value;
                //SetLoadMenuVisible();
                LoadMenuVisible = value;
                SetSaveCancelDataButtons();
            }
        }

        void SetAllowEditRows()
        {
            SetActiveRowState();
        }

        /// <summary>
        /// установка возможности удаления записей
        /// </summary>
        void SetAllowDeleteRows()
        {
            if (IsReadOnly)
            {
                ugData.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
                _utmMain.Tools["DeleteSelectedRows"].SharedProps.Enabled = false;
                _utmMain.Tools["DeleteSelectedRows"].SharedProps.ToolTipText = string.Empty;
                return;
            }

            ugData.DisplayLayout.Override.AllowDelete = AllowDeleteRows ? DefaultableBoolean.True : DefaultableBoolean.False;
            _utmMain.Tools["DeleteSelectedRows"].SharedProps.Enabled = AllowDeleteRows;
            _utmMain.Tools["DeleteSelectedRows"].SharedProps.ToolTipText = AllowDeleteRows ?
                "Удалить запись" :
                string.Empty;
        }

        /// <summary>
        /// установка видимости колонки состояния строки грида
        /// </summary>
        /// <param name="isVisible"></param>
        private void SetRowStateColumnVisible(bool isVisible)
        {
            ugData.BeginUpdate();
            foreach (UltraGridBand band in ugData.DisplayLayout.Bands)
            {
                if (isVisible)
                    AppendStateColumn(band.Columns);
                else
                    if (band.Columns.Exists(StateColumnName))
                        band.Columns.Remove(StateColumnName);
            }
            ugData.EndUpdate();
        }


        // если компонент ридонли, то для него все действия недоступны...
        // если нет, то ставим в состояние, которое было выбрано в зависимости от отдельных состояний
        public void SetComponentSettings()
        {
            SetAllowDeleteRows();
            SetAddButtonsVisible();
            SetAllowEditRows();
            SetSaveCancelDataButtons();
            SetClearTableButtonVisible();
            SetLoadMenuVisible();
            if (_isReadOnly)
            {
                utmMain.Tools["PasteRow"].SharedProps.Enabled = false;
            }
            else
            {
                // ставим что можно делать то, что настроено отдельными свойствами
                _ugData.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
                if (copyRows != null)
                    utmMain.Tools["PasteRow"].SharedProps.Enabled = copyRows.Count > 0;
            }
        }

        /// <summary>
        /// установка параметров меню тулбара для видимости колонок 
        /// </summary>
        /// <param name="gridColumns"></param>
        /// <param name="hierarchyPresent"></param>
		/// <param name="relationName"></param>
        private void SetColumnsButtons(ColumnsCollection gridColumns,
			bool hierarchyPresent, string relationName)
        {
            // составляем список колонок согласно расположению на гриде, а не по порядку в коллекции
            List<UltraGridColumn> columns = new List<UltraGridColumn>();
            for (int i = 0; i <= gridColumns.Count - 1; i++)
            {
                foreach (UltraGridColumn column in gridColumns)
                {
                    if (column.Header.VisiblePosition == i)
                        columns.Add(column);
                }
            }

            foreach (UltraGridColumn column in columns)
            {
                ColumnType clmnType = GetColumnType(column.Key);
                bool btnVisible = !String.IsNullOrEmpty(column.Header.Caption);
                if (hierarchyPresent)
                    btnVisible = btnVisible && column.Header.Caption != relationName;
                ClsColumns.AddButton(
                        column.Key,
                        column.Header.Caption,
                        !column.Hidden,
                        btnVisible,
                        clmnType
                );
            }
        }

        /// <summary>
        /// установка параметров кнопок сохранения и отмены изменений
        /// </summary>
        void SetSaveCancelDataButtons()
        {
            if (IsReadOnly)
            {
                utmMain.Tools["SaveChange"].SharedProps.Enabled = false;
                utmMain.Tools["SaveChange"].SharedProps.ToolTipText = string.Empty;

                utmMain.Tools["CancelChange"].SharedProps.Enabled = false;
                utmMain.Tools["CancelChange"].SharedProps.ToolTipText = string.Empty;
                return;
            }

            // если разрешено одно из этих действий, то изменения можно сохранять и отменять
            if (AllowImportFromXML || AllowDeleteRows || AllowAddNewRecords || AllowEditRows)
            {
                utmMain.Tools["SaveChange"].SharedProps.Enabled = true;
                utmMain.Tools["SaveChange"].SharedProps.ToolTipText = "Сохранить изменения";

                utmMain.Tools["CancelChange"].SharedProps.Enabled = true;
                utmMain.Tools["CancelChange"].SharedProps.ToolTipText = "Отменить изменения";
            }
        }

        /// <summary>
        /// установка видимости кнопок добавления записи в гриде
        /// </summary>
        void SetAddButtonsVisible()
        {
            if (IsReadOnly)
            {
                _utmMain.Tools["btnVisibleAddButtons"].SharedProps.Enabled = false;
                _utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;
                ugData.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
                ugData.DisplayLayout.AddNewBox.Hidden = true;
                utmMain.Tools["CopyRow"].SharedProps.Enabled = false;
                return;
            }

            utmMain.Tools["btnVisibleAddButtons"].SharedProps.Enabled = AllowAddNewRecords;
            utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = AllowAddNewRecords;
            ((StateButtonTool)_utmMain.Tools["btnVisibleAddButtons"]).Checked = AllowAddNewRecords;
            ugData.DisplayLayout.Override.AllowAddNew = AllowAddNewRecords ? AllowAddNew.Yes : AllowAddNew.No;
            ugData.DisplayLayout.AddNewBox.Hidden = !AllowAddNewRecords;
        }

        /// <summary>
        /// установка возможности загрузки данных
        /// </summary>
        void SetLoadMenuVisible()
        {
            if (IsReadOnly)
            {
                LoadMenuVisible = false;
                return;
            }
            LoadMenuVisible = AllowImportFromXML && AllowAddNewRecords;
        }

        /// <summary>
        /// установка возможности очистки всех данных таблицы
        /// </summary>
        private void SetClearTableButtonVisible()
        {
            if (IsReadOnly)
            {
                _utmMain.Tools["ClearCurrentTable"].SharedProps.Enabled = false;
                return;
            }
            _utmMain.Tools["ClearCurrentTable"].SharedProps.Enabled = AllowClearTable;
        }

        private bool saveMenuVisible;
        public bool SaveMenuVisible
        {
            get { return saveMenuVisible; }
            set
            {
                saveMenuVisible = value;
                _utmMain.Tools["menuSave"].SharedProps.Enabled = saveMenuVisible;
            }
        }

        private bool loadMenuVisible;
        public bool LoadMenuVisible
        {
            get { return loadMenuVisible; }
            set
            {
                loadMenuVisible = value;
                _utmMain.Tools["menuLoad"].SharedProps.Enabled = loadMenuVisible;
            }
        }

        public enum ComponentStates { unknown, readonlyState, editableState, allowAddState }
        /// <summary>
        /// устанавливает компонент в определенное состояние
        /// </summary>
        /// <param name="state"></param>
        public void SetComponentToState(ComponentStates state)
        {
            // устанавливает набор свойств у грида
            switch (state)
            {
                case ComponentStates.allowAddState:
                    _isReadOnly = false;
                    AllowAddNewRecords = true;
                    AllowDeleteRows = true;
                    AllowEditRows = true;
                    AllowClearTable = true;
                    StateRowEnable = true;
                    break;
                case ComponentStates.editableState:
                    _isReadOnly = false;
                    AllowAddNewRecords = false;
                    AllowDeleteRows = false;
                    AllowEditRows = true;
                    AllowClearTable = false;
                    AllowImportFromXML = false;
                    StateRowEnable = true;
                    break;
                case ComponentStates.readonlyState:
                    _isReadOnly = true;
                    break;
            }

            foreach (UltraGridBand band in this.ugData.DisplayLayout.Bands)
            {
                switch (state)
                {
                    case ComponentStates.allowAddState:
                        band.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                        band.Layout.Override.AllowAddNew = AllowAddNew.Yes;
                        band.Layout.Override.AllowDelete = DefaultableBoolean.True;
                        break;
                    case ComponentStates.editableState:
                        band.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                        band.Layout.Override.AllowAddNew = AllowAddNew.No;
                        band.Layout.Override.AllowDelete = DefaultableBoolean.False;
                        break;
                    case ComponentStates.readonlyState:
                        band.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                        band.Layout.Override.AllowAddNew = AllowAddNew.No;
                        band.Layout.Override.AllowDelete = DefaultableBoolean.False;
                        break;
                }
            }
        }

        #endregion

        #region установка состояния отдельных записей в гриде

        public enum RowEditorState { ReadonlyState, EditableState }

        public RowEditorState CurrentRowState
        {
            get
            {
                if (_ugData.ActiveRow == null)
                    return RowEditorState.ReadonlyState;
                switch (_ugData.ActiveRow.Activation)
                {
                    case Activation.AllowEdit:
                        return RowEditorState.EditableState;
                    default:
                        return RowEditorState.ReadonlyState;
                }
            }
        }

        /// <summary>
        /// устанавливает строку в определенное состояние
        /// </summary>
        /// <param name="row"></param>
        /// <param name="rowState"></param>
        private void SetRowToState(UltraGridRow row, RowEditorState rowState)
        {
            //CurrentRowState = rowState;
            row = UltraGridHelper.GetRowCells(row);
            // установим состояние записи
            if (row.Activation == Activation.Disabled || row.Activation == Activation.NoEdit)
                return;
            switch (rowState)
            {
                case RowEditorState.EditableState:
                    row.Activation = Activation.AllowEdit;
                    break;
                case RowEditorState.ReadonlyState:
                    row.Activation = Activation.ActivateOnly;
                    break;
            }
            // установим различные системные ячейки
            if (GridColumnsStates == null)
                return;
            foreach (GridColumnState curState in GridColumnsStates.Values)
            {
                if (row.Cells.Contains(curState.ColumnName))
                {
                    if (curState.IsSystem || curState.IsReadOnly)
                        row.Cells[curState.ColumnName].Activation = Activation.ActivateOnly;
                }
            }
        }

        /// <summary>
        /// установка состояния активной записи в зависимости от настроек грида
        /// </summary>
        private void SetActiveRowState()
        {
            if (ugData.ActiveRow != null)
                SetRowToState(ugData.ActiveRow);
        }

        /// <summary>
        /// установка состояния записи в зависимости от настроек грида
        /// </summary>
        /// <param name="row"></param>
        private void SetRowToState(UltraGridRow row)
        {
            row = UltraGridHelper.GetRowCells(row);
            if (IsReadOnly)
            {
                SetRowToState(row, RowEditorState.ReadonlyState);
                return;
            }

            if (AllowEditRows)
            {
                SetRowToState(row, RowEditorState.EditableState);
                return;
            }

            if (AllowAddNewRecords)
            {
                if (StateRowEnable && row.Cells[StateColumnName].Tag != null &&
                    (DataRowState)row.Cells[StateColumnName].Tag == DataRowState.Added)
                    SetRowToState(row, RowEditorState.EditableState);
                else
                    SetRowToState(row, RowEditorState.ReadonlyState);
            }
            else
                SetRowToState(row, RowEditorState.ReadonlyState);
        }

        #endregion

        #region иерархия

        private HierarchyInfo hi;
        /// <summary>
        /// параметры иерархии в гриде
        /// </summary>
        public HierarchyInfo HierarchyInfo
        {
            get
            {
                if (hi == null && _onGetHierarchyInfo != null)
                    hi = _onGetHierarchyInfo(this);
                return hi;
            }
            private set { hi = value; }
        }

        // иерархия доступна если задан обработчик для получения пармаетров иерархии
        // и заданы имена колонок для построения иерархии
        private bool HierarchyEnabled
        {
            get
            {
                if ((_dataSourceType != typeof(DataSet)) || (_onGetHierarchyInfo == null))
                    return false;

                if (HierarchyInfo == null)
                    return false;

                return ((!String.IsNullOrEmpty(HierarchyInfo.ParentClmnName)) &&
                        (!String.IsNullOrEmpty(HierarchyInfo.ParentRefClmnName)));
            }
        }

        // Метод отвечающий за запрос и формирование параметров иерерхии
        private static void CheckHierarchyParams(HierarchyInfo hi)
        {
            // если иерархия недоступна - выходим
            if (hi == null)
                return;

            // проверяем количество уровней иерархии
            if (hi.LevelsCount > hi.MaxHierarchyLevelsCount)
                throw new Exception("Количество уровней иерархии не может превышать " + hi.MaxHierarchyLevelsCount);
            if (hi.LevelsCount <= 0)
                throw new Exception("Количество уровней иерархии должно быть больше нуля");

            // если имена уровней не заданы - задаем имена по умолчанию
            if (hi.LevelsNames == null)
            {
                hi.LevelsNames = new string[hi.LevelsCount];
                for (int i = 0; i < hi.LevelsCount; i++)
                    hi.LevelsNames[i] = String.Format("Уровень {0}", i + 1);
            }
        }

        private string _singleBandLevelName = "Добавить запись...";
        [Description("Для иерархического грида наименование первого уровня в плоском виде")]
        public string SingleBandLevelName
        {
            get { return _singleBandLevelName; }
            set { _singleBandLevelName = value; }
        }

        /// <summary>
        ///  Установка фильтра на грид для построения иерархии
        /// </summary>
        public void SetHierarchyFilter(FilterConditionAction action)
        {
            if (!HierarchyEnabled)
                return;

            // для источников данных с несколькими таблицами ничего не делаем
            if ((_dataSourceType == typeof(DataSet)) && (((DataSet)_dataSource).Tables.Count > 1))
                return;

            UltraGridBand band = _ugData.DisplayLayout.Bands[0];
            ColumnFilter filter = band.ColumnFilters[HierarchyInfo.ParentRefClmnName];
            // если грид настроен на плоское отображение данных, то единственное действие это очистка фильтра
            if (_ugData.DisplayLayout.ViewStyle == ViewStyle.SingleBand)
            {
                filter.FilterConditions.Clear();
                return;
            }
            _ugData.BeginUpdate();
            try
            {
                switch (action)
                {
                    case FilterConditionAction.Set:
                        filter.FilterConditions.Add(FilterComparisionOperator.Equals, null);
                        break;
                    case FilterConditionAction.Refresh:
                        if (filter.FilterConditions.Count > 0)
                            filter.FilterConditions.Clear();
                        filter.FilterConditions.Add(FilterComparisionOperator.Equals, null);
                        break;
                    case FilterConditionAction.Clear:
                        filter.FilterConditions.Clear();
                        break;
                }
            }
            finally
            {
                _ugData.EndUpdate();
            }
        }

        /// <summary>
        /// устанавливает названия на кнопки добавления записей в разных режимах грида (плоский и иерархический)
        /// </summary>
        private void SetAddCaptionButtons(HierarchyInfo hi)
        {
            if (HierarchyEnabled)
            {
                StateButtonTool refParentColumnButton = (StateButtonTool)_utmMain.Tools[hi.ParentRefClmnName];
                switch (hi.CurViewState)
                {
                    case ViewState.Flat:
                        SetHierarchyFilter(FilterConditionAction.Clear);
                        _ugData.DisplayLayout.Bands[0].AddButtonCaption = hi.FlatLevelName;
                        refParentColumnButton.SharedProps.Enabled = true;
                        refParentColumnButton.Checked = true;
                        break;
                    case ViewState.Hierarchy:
                        SetHierarchyFilter(FilterConditionAction.Refresh);
                        foreach (UltraGridBand band in _ugData.DisplayLayout.Bands)
                        {
                            band.AddButtonCaption = hi.LevelsNames[band.Index];
                        }
                        refParentColumnButton.Checked = false;
                        refParentColumnButton.SharedProps.Enabled = false;
                        break;
                }
            }
            else
            {
                _ugData.DisplayLayout.Bands[0].AddButtonCaption = hi != null ?
                    hi.FlatLevelName :
                    SingleBandLevelName;
            }
        }

        private void BeforeRowExpanded(object sender, CancelableRowEventArgs e)
        {
            // если это событие произошло, то иерархия уже доступна, но на всякий случай проверим
            if (!HierarchyEnabled)
                return;
            // если у строки есть потомки - ничего делать не нужно (данные были загружены ранее)
            if (e.Row.HasChild(false))
                return;
            // если режим не страничный - тоже ничего делать не надо
            if (HierarchyInfo.loadMode != LoadMode.OnParentExpand)
                return;
            // иначе - иниицируем событие запроса порции данных
            int parentID = Convert.ToInt32(e.Row.Cells["ID"].Value);
            if (_onNeedLoadChildRows == null)
                throw new Exception("Внутренняя ошибка: необходимо определить обработчик 'OnNeedLoadChildRows'");

            _onNeedLoadChildRows(this, parentID);
        }

        void _ugData_AfterRowCollapsed(object sender, RowEventArgs e)
        {
            // если это событие произошло, то иерархия уже доступна, но на всякий случай проверим
            if (!HierarchyEnabled)
                return;
            // если режим не страничный - тоже ничего делать не надо
            if (HierarchyInfo.loadMode != LoadMode.OnParentExpand)
                return;

            // удаляем из источника данных записи, если они не были изменены или помечены на удаление
            BindingSource bs = (BindingSource)((UltraGrid)sender).DataSource;
            DataView dv = (DataView)bs.DataSource;
            int parentID = Convert.ToInt32(e.Row.Cells["ID"].Value);
            // получаем список всех узлов, которые можно удалить из источника данных
            // удаляем, оставляя те, в которых есть редактируемые данные
            DeleteRowsCollapsedRows(dv.Table, parentID, HierarchyInfo.ParentRefClmnName);
        }

        /// <summary>
        /// удаляем схлопнутые записи из источника данных
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowID"></param>
        /// <param name="refColumnName"></param>
        private void DeleteRowsCollapsedRows(DataTable table, int rowID, string refColumnName)
        {
            DataRow[] rows = table.Select(string.Format("{0} = {1}", refColumnName, rowID));
            bool canDelete = true;
            foreach (DataRow row in rows)
            {
                canDelete = canDelete && row.RowState == DataRowState.Unchanged;
                if (!canDelete)
                    break;
            }
            if (canDelete)
            {
                table.BeginLoadData();
                foreach (DataRow row in rows)
                {
                    row.Delete();
                    row.AcceptChanges();
                }
                table.EndLoadData();
            }
        }

        #endregion

        #region Тулбары и вспомогательные свойства
        private string _SaveLoadFileName = string.Empty;
        [Description("Название файла, которое будет устанавливаться по умолчанию при сохранениях и загрузках")]
        [DefaultValue(true)]
        public string SaveLoadFileName
        {
            get { return _SaveLoadFileName; }
            set
            {
                _SaveLoadFileName = value;
            }
        }

        [Category("Public variables")]
        private bool _ExportImportToolbarVisible = true;
        [Description("Показывать тулбар с импортом/экспортом")]
        [DefaultValue(true)]
        public bool ExportImportToolbarVisible
        {
            get { return _ExportImportToolbarVisible; }
            set
            {
                _ExportImportToolbarVisible = value;
                _utmMain.Toolbars["utbImportExport"].Visible = _ExportImportToolbarVisible;
            }
        }

        [Category("Public variables")]
        private bool _MainToolbarVisible = true;
        [Description("Показывать тулбар с сохранением, отменой и обновлением")]
        [DefaultValue(true)]
        public bool MainToolbarVisible
        {
            get { return _MainToolbarVisible; }
            set
            {
                _MainToolbarVisible = value;
                _utmMain.Toolbars["utbMain"].Visible = _MainToolbarVisible;
            }
        }

        [Category("Public variables")]
        private bool _ColumnsToolbarVisible = true;
        [Description("Показывать тулбар с видимостью колонок и видом грида")]
        [DefaultValue(true)]
        public bool ColumnsToolbarVisible
        {
            get { return _ColumnsToolbarVisible; }
            set
            {
                _ColumnsToolbarVisible = value;
                _utmMain.Toolbars["utbColumns"].Visible = _ColumnsToolbarVisible;
            }
        }

        private StateButtonsManager ClsColumns = null;

        private int _RecordsCount;
        [Description("Количество записей в гриде")]
        public int RecordsCount
        {
            get { return _RecordsCount; }
        }


        string _sortColumnName = string.Empty;
        [Description("Наименование колонки, по которой сортируются записи в гриде")]
        public string sortColumnName
        {
            get { return _sortColumnName; }
            set { _sortColumnName = value; }
        }

        private ColumnType GetColumnType(string clmnName)
        {
            ColumnType tp = ColumnType.Standart;
            if (GridColumnsStates != null)
            {
                string sourceColumnName = GetSourceColumnName(clmnName);
                if (GridColumnsStates.ContainsKey(sourceColumnName))
                    tp = GridColumnsStates[sourceColumnName].ColumnType;
            }
            return tp;
        }

        string _caption = string.Empty;
        [Description("Заголовок грида")]
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        #endregion

        #region Установка источника данных для грида
        // объект-источник данных
        private object _dataSource;
        // тип объекта-источника данных
        private Type _dataSourceType;

        private const DataViewRowState RowStateFilter =
            DataViewRowState.Added | DataViewRowState.CurrentRows | DataViewRowState.Deleted |
            DataViewRowState.ModifiedCurrent | DataViewRowState.Unchanged;

        private static Type CheckDataSourceType(object obj)
        {
            Type objType = obj.GetType();
            if (!((objType == typeof(DataSet)) ||
               (objType == typeof(DataTable)) ||
               (objType == typeof(DataView)) ||
               (objType == typeof(UltraDataSource))))
                throw new Exception(String.Format("Тип '{0}' не является допустимым", objType));
            return objType;
        }

        [Category("Public controls")]
        [Description("Источник данных")]
        [ReadOnly(false)]
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        public object DataSource
        {
            get { return _dataSource; }
            set
            {
                Groups.Clear();
                SetGridDataSource(value, true);
            }
        }

        public void SetGridDataSource(object gridDataSource, bool refreshGridParams)
        {
            //перед установкой источника сбрасываем параметры грида
            if (refreshGridParams)
            {
                _ugData.DataSource = null;
                HierarchyInfo = null;
                gridColumnsStates = null;
            }
            else
            {
                _ugData.DataSource = null;
            }
            if (gridDataSource == null)
            {
                _dataSource = gridDataSource;
                return;
            }

            // определяем тип источника данных
            _dataSourceType = CheckDataSourceType(gridDataSource);
            // запоминаем указатель на источник во внутренней переменной
            _dataSource = gridDataSource;
            // сбрасываем настройки
            BurnChangesDataButtons(false);
            BurnRefreshDataButton(false);
            utmMain.Tools["CopyRow"].SharedProps.Enabled = false;
            ugData.ResetLayouts();
            // в зависимости от типа источника данных создаем DataView
            object dataSource = _dataSource;
            DataSet ds = null;
            DataView dv = null;
            if (_dataSourceType == typeof(DataSet))
            {
                // для нескольких таблиц создадим условия видимости записей для каждой таблицы
                ds = (DataSet)_dataSource;

                DataViewManager dvManager = new DataViewManager(ds);
                foreach (DataViewSetting dvs in dvManager.DataViewSettings)
                    dvs.RowStateFilter = RowStateFilter;
                dv = dvManager.CreateDataView(ds.Tables[0]);

                _RecordsCount = ds.Tables[0].Rows.Count;
            }
            else if (_dataSourceType == typeof(DataTable))
            {
                DataTable dt = (DataTable)_dataSource;
                dv = new DataView(dt, string.Empty, string.Empty, RowStateFilter);
                _RecordsCount = dt.Rows.Count;
            }
            else if (_dataSourceType == typeof(DataView))
            {
                dv = (DataView)_dataSource;
                _RecordsCount = dv.Count;
            }
            #region Настройка иерархии и связанной с ней элементов управления
            // эта переменная нам потребуется за пределами блока настройки иерархии
            // настраиваем возможность переключения вида грида
            _utmMain.EventManager.SetEnabled(Infragistics.Win.UltraWinToolbars.EventGroups.AllEvents, false);
            try
            {
                StateButtonTool hierarchyBtn = (StateButtonTool)_utmMain.Tools["ShowHierarchy"];
                bool enabled = HierarchyEnabled;
                hierarchyBtn.SharedProps.Enabled = enabled;
                hierarchyBtn.SharedProps.ToolTipText = string.Empty;
                hierarchyBtn.SharedProps.Caption = string.Empty;
                if (HierarchyEnabled)
                {
                    // провереяем параметры иерархии
                    CheckHierarchyParams(HierarchyInfo);
                    // при первом запуске показываем в иерархическом виде
                    if (HierarchyInfo.CurViewState == ViewState.NotDefined)
                        HierarchyInfo.CurViewState = ViewState.Hierarchy;
                    //ds.Relations.Clear();
                    switch (HierarchyInfo.CurViewState)
                    {
                        case ViewState.Hierarchy:
                            if (ds.Relations.Count == 0)
                            {
                                DataRelation hr = _onGetHierarchyRelation(this);
                                ds.Relations.Add(hr);
                            }
                            _ugData.DisplayLayout.MaxBandDepth = HierarchyInfo.LevelsCount;
                            hierarchyBtn.Checked = true;
                            hierarchyBtn.SharedProps.ToolTipText = "Не отображать иерархию";
                            break;
                        case ViewState.Flat:
                            _ugData.DisplayLayout.MaxBandDepth = 1;
                            hierarchyBtn.Checked = false;
                            hierarchyBtn.SharedProps.ToolTipText = "Отображать иерархию";
                            break;
                        default:
                            throw new Exception("Внутренняя ошибка: не инициализировано текущее состояние иерархии");
                    }
                }
                else
                {
                    hierarchyBtn.Checked = false;
                }
            }
            finally
            {
                _utmMain.EventManager.SetEnabled(Infragistics.Win.UltraWinToolbars.EventGroups.AllEvents, true);
            }
            #endregion
            // по невыясненным причинам, при работе с BindingSource, в новых компонентах не активируются
            // кнопочки добавления записей ниже 2 уровня.

            if (dv != null)
            {
                // ****
                // Внимание! Без вспомогательного объекта BindingSource
                // Грид аццки тормозит при работе с иерархическими данными
                // подробнее см.
                // http://devcenter.infragistics.com/Support/KnowledgeBaseArticle.aspx?ArticleID=9280
                // Как я понял, этот новый биндингсоурс должен заменить какой-то левый, который
                // создается по-умолчанию
                // ***
                BindingSource bs = new BindingSource();
                bs.DataSource = dv;
                dataSource = bs;
            }

            // синхронизация с CurrencyManager обязательно нужна в слечае использования внешнего
            // BindingSource, иначе - лучше отключить
            _ugData.SyncWithCurrencyManager = dataSource is BindingSource;
            // назначаем гриду Datasource
            _ugData.DataSource = dataSource;//dv;
            maxHeight = new int[_ugData.DisplayLayout.Bands.Count];
            // инициализация грида для формирования серверного фильтра
            InitServerFilter();
            SetComponentSettings();

			// настраиваем тулбар управления видимостью колонок 
            SetVisibleColumnsButtons();

        	SetAddCaptionButtons(HierarchyInfo);
            // сортируем записи по указанной колонке
            if (sortColumnName != string.Empty)
                foreach (UltraGridBand band in ugData.DisplayLayout.Bands)
                {
                    band.Columns[sortColumnName].SortIndicator = SortIndicator.Ascending;
                }
        }

		/// <summary>
		/// Настраивает тулбар управления видимостью колонок.
		/// </summary>
		/// <param name="ds"></param>
		public void SetVisibleColumnsButtons()
		{
			// настраиваем тулбар управления видимостью колонок 
			ClsColumns.ClearButtons();

			bool hierarchyPresent =
				(_dataSourceType == typeof(DataSet)) && (((DataSet)_dataSource).Relations.Count > 0);

			// выводим кнопки для скрытия колонок в том же порядке, что и сами колонки.
			SetColumnsButtons(
				ugData.DisplayLayout.Bands[0].Columns, 
				hierarchyPresent,
				hierarchyPresent ? ((DataSet)_dataSource).Relations[0].RelationName : String.Empty);

			ClsColumns.AttachButtons();
		}
		
		#endregion

        /// <summary>
        /// проверяем, находится ли грид в режиме группировки
        /// </summary>
        /// <returns></returns>
        public bool GridInGroupByMode(bool showMessage)
        {
            bool inGroupByMode = false;
            foreach (UltraGridBand band in _ugData.DisplayLayout.Bands)
            {
                foreach (UltraGridColumn clmn in band.SortedColumns)
                {
                    if (clmn.IsGroupByColumn)
                    {
                        inGroupByMode = true;
                        break;
                    }
                }
                if (inGroupByMode)
                    break;
            }
            if (inGroupByMode && showMessage)
                MessageBox.Show("Для работы с данными необходимо выйти из режима группировки", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return inGroupByMode;
        }

        /// <summary>
        /// показывает, можно ли сохранять данные или нет
        /// </summary>
        private bool isSaveChanges;

        bool refreshData = true;
        /// <summary>
        /// Обновление данных грида
        /// </summary>
        private void RefreshData()
        {
            SaveServerFilter();
            if (_onGetHierarchyInfo != null)
            {
                if (HierarchyInfo.LevelsCount > 1)
                    HierarchyInfo.CurViewState = ViewState.Hierarchy;
            }
            bool isRefresh = true;
            if (ugData.ActiveRow != null)
                ugData.ActiveRow.Update();
            // вызываем основной обработчик
            if (_onRefreshData != null)
                isRefresh = _onRefreshData(this);
			// восстанавливаем настройки фильтров
            if (isRefresh)
            {
                RestoreServerFilter();
                // гасим кнопки
                BurnChangesDataButtons(false);
                BurnRefreshDataButton(false);
                SetColumnsType();
                //SetComponentSettings();
                if (ServerFilterEnabled)
                    RefreshFilterHeight();
            }
        }

        private void SaveChanges()
        {
            isSaveChanges = true;
            // 
            if (ugData.ActiveCell != null)
                if (ugData.ActiveCell.IsInEditMode)
                    ugData.ActiveCell = ugData.ActiveRow.Cells[0];
            if (!isSaveChanges) return;

            if (_ugData.ActiveRow != null)
                _ugData.ActiveRow.Update();

            if (_onSaveChanges != null)
                isSaveChanges = _onSaveChanges(this);
            // только если данные нормально сохранились, то удаляем все картинки
            if (isSaveChanges)
            {
                RejectImages();
                BurnChangesDataButtons(false);
            }
        }

        /// <summary>
        /// Очищает текущие данные грида из базы
        /// </summary>
        private void ClearCurrentData()
        {
            if (_onClearCurrentTable != null)
                _onClearCurrentTable(this);
        }

        // показывает, нужно ли менять текущее отображение иерархии
        private bool isChangeHierarchyView = true;

        /// <summary>
        /// Ставит фильтр на колонку по определенному значению
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="compareValue"></param>
        public void SetFilter(string columnName, object compareValue)
        {
            ugData.DisplayLayout.Bands[0].ColumnFilters[columnName].FilterConditions.Clear();
            if (compareValue != null)
                ugData.DisplayLayout.Bands[0].ColumnFilters[columnName].FilterConditions.Add(FilterComparisionOperator.Equals, compareValue);
        }

        /// <summary>
        /// устанавливает у всех записей значок "Добавлена"
        /// </summary>
        public void SetAllRowsToAddMode()
        {
            foreach (UltraGridRow row in ugData.Rows)
                SetRowAppearance(row, LocalRowState.Added);
        }

        List<string> RemaskedColumnsNames = new List<string>();
        List<int> RemaskedColumnsDigitCount = new List<int>();

        List<string> LookupColumnsNames = new List<string>();

        // содержит в себе соотношение между отображаемой колонкой в гриде и колонкой, из которой данные попадают в базу
        private Dictionary<string, string> gridColumns = new Dictionary<string, string>();
        /// <summary>
        /// получаем колонку, которая отображается в гриде
        /// </summary>
        /// <param name="dataColumnName"></param>
        /// <returns></returns>
        public string GetGridColumnName(string dataColumnName)
        {
            dataColumnName = dataColumnName.ToLower();
            if (gridColumns.ContainsKey(dataColumnName))
                return gridColumns[dataColumnName];
            return dataColumnName;
        }

        public bool ColumnIsLookup(string columnName)
        {
            return LookupColumnsNames.Contains(columnName);
        }

        public enum ColumnType { Standart, System, Service, unknown };

        // установка маски на колонку
        private void CheckColumnMask(GridColumnState curState, UltraGridColumn clmn)
        {
            // если колонка неуказана или лукап - ничего не делаем
            if ((curState.IsLookUp) ||
                (String.IsNullOrEmpty(curState.Mask)))
                return;

            UltraGridColumn maskedColumn = clmn;
            // для строковых полей, у которых ограничение на количество вводимых символов
            // ставим ограничение по количеству этих символов
            if (curState.Mask.Contains("a") && !curState.Mask.Contains("n") &&
                !curState.Mask.Contains("9") && !curState.Mask.Contains("#"))
            {
                maskedColumn.MaxLength = curState.Mask.Length;
                return;
            }

            // определяем, нужна ли фиктивная колонка для маски
            bool isRemaskedColumn =
                // она не должна быть календарем
                (!curState.CalendarColumn) &&
                // должна содержать точку либо пробел
                (curState.Mask.Contains(".") || curState.Mask.Contains(" ")) &&
                // и один из признаков числовой маски
                (curState.Mask.Contains("9") || curState.Mask.Contains("#") || curState.Mask.Contains("0"));

            // запоминаем колонку на которую надо установить маску

            // если колонка требует создания фиктивной...
            if (isRemaskedColumn)
            {
                // формируем имя
                string remaskedName = String.Concat(curState.ColumnName, REMASKED_COLUMN_POSTFIX);
                // смотрим - была ли она обработана ранее (в родительских bands)
                if (!RemaskedColumnsNames.Contains(remaskedName))
                {
                    // если нет - определяем кол-во разрядов
                    int maskLength = 0;
                    foreach (char ch in curState.Mask)
                        if (ch == '#' || ch == '9' || ch == '0')
                            maskLength++;
                    // и добавляем в обе вспомогательные коллекции
                    RemaskedColumnsNames.Add(remaskedName);
                    RemaskedColumnsDigitCount.Add(maskLength);
                }
                maskedColumn = UltraGridHelper.CreateDummyColumn(clmn, remaskedName);
                if (!gridColumns.ContainsKey(clmn.Key.ToLower()))
                    gridColumns.Add(clmn.Key.ToLower(), remaskedName);
            }

            // теперь устанавливаем общие параметры для всех колонок с маской
            maskedColumn.MaskInput = curState.Mask;
            maskedColumn.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            maskedColumn.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            maskedColumn.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
            maskedColumn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            // для всех колонок кроме календаря - создаем редактор 
            // (для каледнаря какой-то свой редактор у Ромы есть)
            // ...редактор с календарем
            if (!curState.CalendarColumn)
                maskedColumn.Editor = new Infragistics.Win.EditorWithMask();
            // для вещественных чисел - правое выравнивание
            if (curState.Mask.Contains("n.n"))
                maskedColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            // для целых с минусом - отключаем отображение символов маски
            else if (curState.Mask.Contains("-n"))
                maskedColumn.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
        }

        /// <summary>
        /// инициализирует слой отображения грида
        /// </summary>
        public void ReinitializeLayout()
        {
            InitializeLayoutEventArgs arg = new InitializeLayoutEventArgs(_ugData.DisplayLayout);
            _ugData_InitializeLayout(_ugData, arg);
        }

        public GridColumnsStates CurrentStates
        {
            get { return GridColumnsStates; }
        }

        /// <summary>
        /// устанавливает свойства колонок
        /// </summary>
        private void SetColumnsType()
        {
            foreach (UltraGridBand band in ugData.DisplayLayout.Bands)
            {
                // если не указан список настроек - ничего далее не делаем
                if (GridColumnsStates == null) break;
                // иначе - применяем настройки к каждой из колонок
                foreach (GridColumnState curState in GridColumnsStates.Values)
                {
                    // если такой колонки нет - пропускаем описание 
                    if (!band.Columns.Exists(curState.ColumnName))
                        continue;

                    // иначе - устанавдиваем параметры
                    UltraGridColumn column = band.Columns[curState.ColumnName];
                    // если включен режим серверного фильтра - в основном гриде фильтрацию отключаем
                    if (ServerFilterEnabled)
                        column.AllowRowFiltering = DefaultableBoolean.False;
                    // ставим русское название в заголовок
                    column.Header.Caption = curState.ColumnCaption;

                    // если колонка помечена только для чтения, ставим невозможность ее редактирования
                    if (curState.IsSystem || curState.IsReadOnly)
                    {
                        column.CellActivation = Activation.ActivateOnly;
                        if (curState.IsSystem)
                            column.CellAppearance.BackColor = System.Drawing.Color.Gainsboro;
                    }
                    // для nullable колонок ставим серый фон
                    if (curState.IsNullable &&
                        !curState.IsSystem &&
                        !curState.IsReadOnly)
                    {
                        column.CellAppearance.BackColor = System.Drawing.SystemColors.Info;
                    }
                }
            }
        }

        // содержит настройки про колонки грида
        private GridColumnsStates gridColumnsStates = null;
        internal GridColumnsStates GridColumnsStates
        {
            get
            {
                if (gridColumnsStates == null && _onGetGridColumnsState != null)
                    gridColumnsStates = _onGetGridColumnsState(this);
                return gridColumnsStates;
            }
        }


        private bool LookupsEventsDefined()
        {
            return ((_onGetLookupValue != null) && (_onCheckLookupValue != null));
        }

        /// <summary>
        /// Указывает, разрешены или запрещены группировки полей в гриде
        /// </summary>
        public bool EnableGroups
        {
            get; set;
        }

        /// <summary>
        /// список группировок в гриде с первой колонкой 
        /// </summary>
        public Dictionary<string, string > Groups
        {
            get; private set;
        }

        /// <summary>
        /// Инициализация отображения грида
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ugData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // сперва установим различные разрешения и установки, какие были...
            RemaskedColumnsNames.Clear();
            RemaskedColumnsDigitCount.Clear();
            LookupColumnsNames.Clear();
            SetEditors();
            e.Layout.ScrollStyle = ScrollStyle.Immediate;
            // показываем кнопку со скрытием фильтра
            StateButtonTool filterBtn = (StateButtonTool)utmMain.Tools["ShowFilter"];
            filterBtn.SharedProps.Visible = ServerFilterEnabled;

            if (GridColumnsStates != null)
            {
                foreach (var columnState in
                    GridColumnsStates.Values.Where(w => w.FirstInGroup).Where(columnState => !Groups.ContainsKey(columnState.GroupName)))
                {
                    Groups.Add(columnState.GroupName, columnState.ColumnName);
                }
            }

            #region Непосредственно параметры колонок
            // ставим для колонок грида параметры
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                // в соответствии с настройками устанавливаем колоку для картинок 
                if (StateRowEnable)
                    AppendStateColumn(band.Columns);
                else
                    if (band.Columns.Contains(StateColumnName))
                        band.Columns.Remove(StateColumnName);
                // если не указан список настроек - ничего далее не делаем
                if (GridColumnsStates == null) break;
                // иначе - применяем настройки к каждой из колонок
                foreach (GridColumnState curState in GridColumnsStates.Values)
                {
                    // если такой колонки нет - пропускаем описание 
                    if (!band.Columns.Exists(curState.ColumnName))
                        continue;

                    // иначе - устанавдиваем параметры
                    UltraGridColumn column = band.Columns[curState.ColumnName];
                    // если включен режим серверного фильтра - в основном гриде фильтрацию отключаем
                    if (ServerFilterEnabled)
                        column.AllowRowFiltering = DefaultableBoolean.False;
                    // ставим русское название в заголовок
                    column.Header.Caption = curState.ColumnCaption;
                    // позиция в гриде
                    if (curState.ColumnPosition > 0)
                        column.Header.VisiblePosition = curState.ColumnPosition;
                    // если колонка помечена только для чтения, ставим невозможность ее редактирования
                    if (curState.IsSystem || curState.IsReadOnly)
                    {
                        column.CellActivation = Activation.ActivateOnly;
                        if (curState.IsSystem)
                            column.CellAppearance.BackColor = Color.Gainsboro;
                    }
                    // для nullable колонок ставим серый фон
                    if (curState.IsNullable &&
                        !curState.IsSystem &&
                        !curState.IsReadOnly)
                    {
                        column.CellAppearance.BackColor = SystemColors.Info;
                    }
                    // ставим колонке определенный размер
                    SetColumnWdth(column, curState);

                    // если колонка помечена как скрытая, то скрываем ее
                    column.Hidden = curState.IsHiden;

                    // если указана какой то стиль колонки, то применим его
                    column.Style = curState.ColumnStyle;

                    if (column.Style == Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList)
                    {
                        column.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    }

                    // если колонка - календарь
                    if (curState.CalendarColumn)
                    {
                        //column.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                        column.Editor = EditorWithMaskForCalendarColumns;
                    }
                    // если колонка не календарь (???) но таки лукап
                    if ((!curState.CalendarColumn) && (curState.IsLookUp))
                    {
                        UltraGridColumn tmpColumn = column;
                        if (LookupsEventsDefined())
                        {
                            string lookupColumnName = tmpColumn.Key + LOOKUP_COLUMN_POSTFIX;
                            if (!gridColumns.ContainsKey(tmpColumn.Key.ToLower()))
                                gridColumns.Add(tmpColumn.Key.ToLower(), lookupColumnName);
                            tmpColumn = UltraGridHelper.CreateDummyColumn(tmpColumn, lookupColumnName);
                            if (!LookupColumnsNames.Contains(lookupColumnName))
                                LookupColumnsNames.Add(lookupColumnName);
                        }
                        tmpColumn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                        UltraGridHelper.SetLikelyEditButtonColumnsStyle(tmpColumn, 0);
                        tmpColumn.AllowRowFiltering = ServerFilterEnabled ? DefaultableBoolean.False : DefaultableBoolean.True;
                        tmpColumn.CellButtonAppearance.Image = ilSmall.Images[0];
                        // пишем в TAG название исходного объекта-лукапа
                        tmpColumn.Tag = curState.Tag;
                        // Если у столбца CellMultiline стоит False, то пропадают некоторые яяейки в гриде
                        //tmpColumn.MaskInput = curState.Mask;
                    }

                    // для полей-ссылок приделываем кнопку
                    if (curState.IsReference && !curState.CalendarColumn)
                    {
                        column.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                        UltraGridHelper.SetLikelyEditButtonColumnsStyle(column, 0);
                        column.AllowRowFiltering = ServerFilterEnabled ? DefaultableBoolean.False : DefaultableBoolean.True;
                        column.CellButtonAppearance.Image = ilSmall.Images[0];
                    }

                    // если нужно - ставим маску
                    CheckColumnMask(curState, column);

                    column.CellMultiLine =
                        ((column.DataType == typeof(String)) && (curState.isWrapWord)) ? DefaultableBoolean.True : DefaultableBoolean.False;

                    if (column.DataType == typeof(int) || column.DataType == typeof(decimal) ||
                        column.DataType == typeof(double) || column.DataType == typeof(long))
                    {
                        column.FilterOperatorDropDownItems =
                            FilterOperatorDropDownItems.Equals |
                            FilterOperatorDropDownItems.NotEquals |
                            FilterOperatorDropDownItems.LessThan |
                            FilterOperatorDropDownItems.LessThanOrEqualTo |
                            FilterOperatorDropDownItems.GreaterThan |
                            FilterOperatorDropDownItems.GreaterThanOrEqualTo |
                            FilterOperatorDropDownItems.Like;
                    }
                }
            }

            #endregion

            // вызываем внешний обработчик
            if (_OnGridInitializeLayout != null)
                _OnGridInitializeLayout(sender, e);

            ugData.DisplayLayout.Override.WrapHeaderText = DefaultableBoolean.False;
        }

        void SetColumnWdth(UltraGridColumn column, GridColumnState curState)
        {
            switch (column.Header.Caption)
            {
                case "ID":
                    column.Width = 80;
                    break;
                case "":
                    column.Width = 20;
                    break;
                default:
                    if (curState.ColumnWidth > 260)
                        column.Width = 260;
                    else
                        if (curState.ColumnWidth >= 190)
                            column.Width = 190;
                        else
                            if (curState.ColumnWidth <= 20)
                                column.Width = 130;
                            else
                                column.Width = curState.ColumnWidth;
                    break;
            }
        }

        /// <summary>
        /// добавляем в грид колонку для отображения состояния записи
        /// </summary>
        /// <param name="columns"></param>
        private void AppendStateColumn(ColumnsCollection columns)
        {
            if (!columns.Exists(StateColumnName))
            {
                UltraGridColumn clmn = columns.Add(StateColumnName, "");
                clmn.Width = 20;
                clmn.MaxWidth = 20;
                clmn.MinWidth = 20;
                clmn.Header.VisiblePosition = 0;
                clmn.CellAppearance.BackColor = System.Drawing.Color.Gainsboro;
                clmn.CellActivation = Activation.NoEdit;
                clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
                clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
                clmn.FilterOperandStyle = FilterOperandStyle.None;
                clmn.FilterOperatorLocation = FilterOperatorLocation.Hidden;
                clmn.FilterOperatorDropDownItems = FilterOperatorDropDownItems.None;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.Header.Fixed = true;
            }
            else
            {
                columns[StateColumnName].Header.VisiblePosition = 0;
                columns[StateColumnName].CellActivation = Activation.NoEdit;
            }
        }

        // состояния записей грида для отрисовки картинок
        public enum LocalRowState { Added, Deleted, Modified, Unchanged };

        public enum FilterConditionAction { Set, Refresh, Clear };

        // объекты хранения изменений
        private List<UltraGridRow> ChangedRows = new List<UltraGridRow>();
        private Dictionary<int, List<ChangedUltraGridRow>> changes = new Dictionary<int, List<ChangedUltraGridRow>>();

        /// <summary>
        /// Удаляем все картинки из грида
        /// </summary>
        private void RejectImages()
        {
            if (!StateRowEnable)
                return;
            _ugData.BeginUpdate();
            _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                for (int i = 0; i <= ChangedRows.Count - 1; i++)
                {
                    UltraGridRow row = ChangedRows[i];
                    if (row.Cells.Count > 0)
                    {
                        UltraGridCell cell = row.Cells[StateColumnName];
                        cell.ToolTipText = string.Empty;
                        cell.Appearance.ResetImageBackground();
                        cell.Value = null;
                        row.Update();
                    }
                }
                ChangedRows.Clear();
                changes.Clear();
            }
            finally
            {
                _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                _ugData.EndUpdate();
            }
        }


        public void ClearAllStateImages()
        {
            UltraGridCell cell = null;
            foreach (UltraGridRow row in ugData.Rows)
            {
                if (row.Cells.Exists(StateColumnName))
                {
                    cell = row.Cells[StateColumnName];
                    if (cell.Appearance.ImageBackground != null)
                    {
                        cell.ToolTipText = string.Empty;
                        cell.Appearance.ResetImageBackground();
                        cell.Value = null;
                        row.Update();
                    }
                }
            }
        }

        /// <summary>
        /// Установка картинки на измененную стоку
        /// </summary>
        /// <param name="row"></param>
        /// <param name="rowState"></param>
        public void SetRowAppearance(UltraGridRow row, LocalRowState rowState)
        {
            if (row == null) return;
            // пытаемся найти колонку индикации состояния строки
            while (row.Cells == null)
                row = row.ChildBands[0].Rows[0];
            if (!StateRowEnable)
                return;
            UltraGridCell cellPict = row.Cells[StateColumnName];

            // если состояние строки не изменилось, или уже было изменено ранее - ничего делать не нужно
            if (cellPict.Appearance.ImageBackground != null && rowState != LocalRowState.Unchanged) return;
            bool isImage = true;
            // устанавалием картинку состояния
            cellPict.Appearance.ResetImageBackground();
            switch (rowState)
            {
                case LocalRowState.Added:
                    SetRowStateImage(row, DataRowState.Added);
                    break;
                case LocalRowState.Deleted:
                    SetRowStateImage(row, DataRowState.Deleted);
                    break;
                case LocalRowState.Modified:
                    SetRowStateImage(row, DataRowState.Modified);
                    break;
                case LocalRowState.Unchanged:
                    SetRowStateImage(row, DataRowState.Unchanged);
                    isImage = false;
                    break;
            }
            // добавим запись в коллекцию измененных, удаленных или добавленных записей
            if (isImage)
            {
                object id = row.Cells["ID"].Value;
                if (id != null && id != DBNull.Value)
                {
                    if (HierarchyEnabled)
                    {
                        ChangedUltraGridRow changedRow = new ChangedUltraGridRow(row);
                        if (changes.ContainsKey(changedRow.ParentID))
                            changes[changedRow.ParentID].Add(changedRow);
                        else
                        {
                            changes.Add(changedRow.ParentID, new List<ChangedUltraGridRow>());
                            changes[changedRow.ParentID].Add(changedRow);
                        }
                    }
                    ChangedRows.Add(row);
                }
            }
        }

        private void SetRowStateImage(UltraGridRow row, DataRowState rowState)
        {
            UltraGridCell cellPict = row.Cells[StateColumnName];

            _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                cellPict.Appearance.ResetImageBackground();
                switch (rowState)
                {
                    case DataRowState.Added:
                        cellPict.ToolTipText = "Добавлено";
                        cellPict.Appearance.ImageBackground = il.Images[2];
                        row.Tag = DataRowState.Added;
                        break;
                    case DataRowState.Deleted:
                        cellPict.ToolTipText = "Удалено";
                        cellPict.Appearance.ImageBackground = il.Images[1];
                        row.Tag = DataRowState.Deleted;
                        break;
                    case DataRowState.Modified:
                        cellPict.ToolTipText = "Изменено";
                        cellPict.Appearance.ImageBackground = il.Images[0];
                        row.Tag = DataRowState.Modified;
                        break;
                    case DataRowState.Unchanged:
                        cellPict.Appearance.ResetImageBackground();
                        cellPict.ToolTipText = string.Empty;
                        row.Tag = DataRowState.Unchanged;
                        break;
                }
            }
            finally
            {
                // обновляем строку в гриде (на экране)
                _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        // Удаление строк
        private void _ugData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (IsReadOnly) return;
            if (GridInGroupByMode(true))
            {
                e.Cancel = true;
                return;
            }
            if (_OnBeforeRowsDelete != null)
                _OnBeforeRowsDelete(this, e);
            if (e.Cancel)
                return;
            e.DisplayPromptMsg = false;
            DataTable dtSource = DataSource is DataTable ? (DataTable)DataSource : ((DataSet)DataSource).Tables[0];
            foreach (UltraGridRow row in e.Rows)
            {
                SetRowAppearance(row, LocalRowState.Deleted);
                // удаляем руками запись третьего уровня
                if (row.Band.Index == 2)
                {
                    object id = row.Cells["ID"].Value;
                    DataRow[] deletedRows = dtSource.Select(string.Format("ID = {0}", id));
                    if (deletedRows.Length > 0)
                    {
                        DataRow deletedRow = deletedRows[0];
                        if (deletedRow.RowState != DataRowState.Deleted)
                            deletedRow.Delete();
                    }
                }
            }
            if (_OnAfterRowsDelete != null)
                _OnAfterRowsDelete(this, e);
            if (HierarchyEnabled)
            {
                if (HierarchyInfo.CurViewState == ViewState.Hierarchy)
                    SetHierarchyFilter(FilterConditionAction.Refresh);
                else
                    SetHierarchyFilter(FilterConditionAction.Clear);
            }
            BurnChangesDataButtons(true);
        }

        /// <summary>
        /// добавление записи по кнопке Insert 
        /// на тот же уровень, что и активная запись
        /// </summary>
        /// <returns></returns>
        private void InsertRow()
        {
            if (IsReadOnly || !AllowAddNewRecords)
                return;
            if (GridInGroupByMode(true))
                return;

            UltraGridRow row = ugData.ActiveRow;
            int bandIndex = -1;

            if (row != null)
            {
                if (row.ParentRow != null)
                    bandIndex = row.ParentRow.Band.Index;
            }
            ActivateFirstEditableCell(ugData.DisplayLayout.Bands[bandIndex + 1].AddNew());
        }

        /// <summary>
        /// добавление записи по кнопке Insert 
        /// на уровень ниже активной записи
        /// </summary>
        /// <returns></returns>
        private void InsertChildRow()
        {
            if (!AllowAddNewRecords)
                return;
            if (GridInGroupByMode(true))
                return;

            UltraGridRow row = ugData.ActiveRow;
            int bandIndex = row.Band.Index;
            // если уровень родительской записи не самый последний, то добавим запись, иначе ничего делать не будем
            if (bandIndex < ugData.DisplayLayout.MaxBandDepth - 1)
            {
                if (row.ChildBands != null)
                {
                    ActivateFirstEditableCell(row.ChildBands[0].Band.AddNew());
                }
            }
        }

        /// <summary>
        /// активирует первую видимую ячейку в строке, которую можно редактировать
        /// </summary>
        /// <param name="row"></param>
        private void ActivateFirstEditableCell(UltraGridRow row)
        {
            for (int i = 0; i <= row.Cells.Count - 1; i++)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Column.Header.VisiblePosition == i && !cell.Column.Hidden
                        && cell.Column.CellActivation == Activation.AllowEdit)
                    {
                        cell.Activate();
                        ((UltraGrid)row.Band.Layout.Grid).PerformAction(UltraGridAction.EnterEditMode);
                        return;
                    }
                }
            }
        }


        private void _ugData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            //if (e.ParentRow != null)
            //    e.ParentRow.Activate();
        }


        /// <summary>
        /// действия, выполняемые после добавления записей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ugData_AfterRowInsert(object sender, RowEventArgs e)
        {
            UltraGridRow row = e.Row;
            // по какой то причине запись, определяемая гридом как добавленная таковой не является. 
            // А является ею запись, в данный момент активная. Получим ее вместо той, которую грид считает добавленной 
            if (row.Cells.Exists("ID"))
                if (row.Cells["ID"].Value.ToString() != string.Empty)
                {
                    UltraGridRow activeRow = ((UltraGrid)sender).ActiveRow;
                    if (string.IsNullOrEmpty(activeRow.Cells["ID"].Value.ToString()))
                        row = activeRow;
                    else
                    {
                        if (ugData.DisplayLayout.Bands.Count > 1)
                            activeRow.Band.AddNew();
                        return;
                    }
                }

            if (_OnAfterRowInsert != null)
                _OnAfterRowInsert(sender, row);
            _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                row.Update();
                // если можно добавлять записи, то их можно и редактировать...
                SetRowAppearance(row, LocalRowState.Added);
                SetRowToState(row);
                row.Update();
            }
            finally
            {
                _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                row.Refresh(RefreshRow.FireInitializeRow);
            }
            BurnChangesDataButtons(true);
        }

        // Изменение данных в строке
        private void _ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            // если данные не изменены - выходим            
            if (!e.Cell.DataChanged)
                return;

            UltraGridColumn column = e.Cell.Column;
            int columnWidth = column.Width;
            column.PerformAutoResize();
            column.Width = columnWidth;

            BurnChangesDataButtons(true);
            if (!e.Cell.Row.IsAddRow)
                SetRowAppearance(e.Cell.Row, LocalRowState.Modified);
            UltraGridCell cell = e.Cell;
            if (ugData.ActiveCell == null)
                return;
            if (cell != null)
            {
                if (RemaskedColumnsNames.Contains(ugData.ActiveCell.Column.Key))
                {
                    _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                    try
                    {
                        string dataClmnName = GetSourceColumnName(ugData.ActiveCell.Column.Key);
                        decimal val = 0;
                        if (cell.Row.Cells[dataClmnName].Column.DataType != typeof(string))
                        {
                            string valueStr = cell.Value.ToString().Replace(".", string.Empty);
                            valueStr = valueStr.Replace(" ", string.Empty);
                            if (valueStr != string.Empty)
                            {
                                val = Convert.ToDecimal(valueStr);
                                cell.Row.Cells[dataClmnName].Value = val;
                            }
                            else
                                cell.Row.Cells[dataClmnName].Value = DBNull.Value;
                        }
                        else
                            cell.Row.Cells[dataClmnName].Value = cell.Value;
                    }
                    finally
                    {
                        _ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    }

                }
            }
        }

        #region Реализация Drag&Drop

        private void _ugData_DragDrop(object sender, DragEventArgs e)
        {
            if (_OnDragDrop != null)
                _OnDragDrop(this, e);
        }

        private void _ugData_DragEnter(object sender, DragEventArgs e)
        {
            if (_OnDragEnter != null)
                _OnDragEnter(this, e);
        }

        private void _ugData_DragLeave(object sender, EventArgs e)
        {
            if (_OnDragLeave != null)
                _OnDragLeave(this, e);
        }

        private void _ugData_DragOver(object sender, DragEventArgs e)
        {
            if (_OnDragOver != null)
                _OnDragOver(this, e);
        }

        private void _ugData_SelectionDrag(object sender, CancelEventArgs e)
        {
            if (_OnSelectionDrag != null)
                _OnSelectionDrag(this, e);
        }
        #endregion

        private void _ugData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (_OnInitializeRow != null)
                _OnInitializeRow(sender, e);
            UltraGridRow row = e.Row;

            if ((RemaskedColumnsNames.Count == 0) && (LookupColumnsNames.Count == 0))
                return;

            #region Фиктивные колонки для масок
            // обработчик дополнительных колонок с маской
            // записываем данные из нормальных колонок записи в фиктивные с простановкой слева нулей, там где нужно
            for (int i = 0; i <= RemaskedColumnsNames.Count - 1; i++)
            {
                string columnName = GetSourceColumnName(RemaskedColumnsNames[i]);
                string strValue = string.Empty;
                if (e.Row.Cells[columnName].Value != null)
                    strValue = e.Row.Cells[columnName].Value.ToString();
                if (strValue != string.Empty)
                    e.Row.Cells[RemaskedColumnsNames[i]].Value = strValue.PadLeft(RemaskedColumnsDigitCount[i], '0');
            }
            #endregion

            #region Фиктивные колонки для лукапов
            foreach (string lookupColumnName in LookupColumnsNames)
            {
                string sourceColumnName = GetSourceColumnName(lookupColumnName);
                string lookupObjectName = (string)row.Band.Columns[lookupColumnName].Tag;
                // если колонка присутствует в коллекции - обработчик тоже должен быть назначен
                row.Cells[lookupColumnName].Value =
                    _onGetLookupValue(this, lookupObjectName, false,
                    row.Cells[sourceColumnName].Value);
                // почему то у таких колонок значение стоит false
                if (row.Cells[lookupColumnName].Column.Header.Tag == null)
                    row.Cells[lookupColumnName].Column.CellMultiLine = row.Cells[sourceColumnName].Column.CellMultiLine;
                else
                {
                    CheckState checkState = (CheckState) row.Cells[lookupColumnName].Column.Header.Tag;
                    row.Cells[lookupColumnName].Column.CellMultiLine = checkState == CheckState.Checked ?
                        DefaultableBoolean.True :
                        DefaultableBoolean.False;
                }
            }
            // если виден редактор - синхронизируем его значение
            if (LookupColumnsNames.Count != 0)
            {
                if ((lastActiveCellWithLookupEditor != null) &&
                    (umeLookupValue.Visible) &&
                    lastActiveCellWithLookupEditor.Row == row)
                {
                    string sourceColumnName = lastActiveCellWithLookupEditor.Column.Key.Replace(LOOKUP_COLUMN_POSTFIX, String.Empty);
                    umeLookupValue.Value = row.Cells[sourceColumnName].Value;
                }
            }
            #endregion
        }

        // метод нажатия кнопки на редакторе лукапов - транслируется в метод нажатия кнопки на ячейке
        private void umeLookupValue_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            // если кнока нажата, а активной ячейки для редактора нет - произошел глюк
            // (подвисание редактора) - ничего не делаем
            if (lastActiveCellWithLookupEditor == null)
                return;
            // если не определен обработчик нажатия кнопки в ячейке - тоже ничего не делаем
            if (_OnClickCellButton == null)
                return;
            // перенаправляем в метод нажатия кнопки на ячейке
            CellEventArgs args = new CellEventArgs(lastActiveCellWithLookupEditor);
            _OnClickCellButton(this.ugData, args);
            // берем из ячейки измененное значения и пишем в редактор
            //string sourceColumnName = lastActiveCellWithLookupEditor.Column.Key.Replace(LOOKUP_COLUMN_POSTFIX, String.Empty);
            //object newValue = lastActiveCellWithLookupEditor.Row.Cells[sourceColumnName].Value;
            //UltraMaskedEdit ed = (UltraMaskedEdit)sender;
            //if (ed.Value != newValue)
            //    ed.Value = newValue;
        }

        private void _ugData_ClickCellButton(object sender, CellEventArgs e)
        {
            if (_OnClickCellButton != null)
                _OnClickCellButton(this, e);
        }

        private void _ugData_BeforeRowDeactivate(object sender, CancelEventArgs e)
        {
            utmMain.Tools["CopyRow"].SharedProps.Enabled = false;
            if (_OnBeforeRowDeactivate != null)
                _OnBeforeRowDeactivate(sender, e);
        }

        // для различных манипуляций с нажатием кнопок грида и выводом хинтов
        private void _ugData_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (_OnMouseEnterGridElement != null)
                _OnMouseEnterGridElement(sender, e);
            if (e.Element is CheckBoxUIElement)
            {
                CheckBoxUIElement checkBox = (CheckBoxUIElement)e.Element;
                if (checkBox != null)
                {
                    if (checkBox.Parent is HeaderUIElement)
                    {
                        Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader =
                        (Infragistics.Win.UltraWinGrid.ColumnHeader)
                        checkBox.GetAncestor(typeof(HeaderUIElement)).GetContext(
                            typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                        if (aColumnHeader.Column.DataType == typeof(bool))
                        {
                            if (checkBox.CheckState == CheckState.Unchecked)
                                ToolTip.ToolTipText = "Применить для всех";
                            else
                                ToolTip.ToolTipText = "Отменить для всех";
                        }
                        else if (aColumnHeader.Column.DataType == typeof (string))
                        {
                            ToolTip.ToolTipText = "Перенос по словам";
                        }

                        Point tooltipPos = new Point(e.Element.ClipRect.Left, e.Element.ClipRect.Bottom);
                        tooltipPos.Y = tooltipPos.Y + checkBox.Rect.Height + 2;
                        ToolTip.Show(ugData.PointToScreen(tooltipPos));
                    }
                }
            }
            if (e.Element is ImageAndTextButtonUIElement)
            {
                ImageAndTextButtonUIElement button = (ImageAndTextButtonUIElement)e.Element;
                if (button.Parent is HeaderUIElement)
                {
                    HeaderBase header = ((HeaderUIElement) button.Parent).Header;
                    if (header.Group == null)
                    {
                        ToolTip.ToolTipText = "Установка значения поля как у родителя";
                        Point tooltipPos = new Point(e.Element.ClipRect.Left, e.Element.ClipRect.Bottom);
                        tooltipPos.Y = tooltipPos.Y + button.Rect.Height + 2;
                        ToolTip.Show(ugData.PointToScreen(tooltipPos));
                    }
                }
            }
        }

        private void _ugData_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (_OnMouseLeaveGridElement != null)
                _OnMouseLeaveGridElement(sender, e);
            if (e.Element is CheckBoxUIElement)
            {
                ToolTip.Hide();
            }
            if (e.Element is ImageAndTextButtonUIElement)
                ToolTip.Hide();
        }

        public void ClearAllRowsImages()
        {
            RejectImages();
        }

        /// <summary>
        /// ищет в колеекции записей запись с указанным ID и ставит знак изменения данных
        /// </summary>
        /// <param name="ID">ID записи</param>
        /// <param name="rows">коллекция записей</param>
        /// <param name="rowState">состояние, в котором будут находится записи</param>
        public void SetRowToStateByID(int ID, RowsCollection rows, LocalRowState rowState)
        {
            foreach (UltraGridRow row in rows)
            {
                if (Convert.ToInt32(row.Cells["ID"].Value) == ID)
                {
                    SetRowAppearance(row, rowState);
                    row.Activate();
                    break;
                }
            }
        }

        private void _ugData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            if (_OnBeforeCellActivate != null)
                _OnBeforeCellActivate(sender, e);
        }

        private void _ugData_AfterRowActivate(object sender, EventArgs e)
        {
            utmMain.Tools["CopyRow"].SharedProps.Enabled = AllowAddNewRecords && !IsReadOnly;

            if (_OnAfterRowActivate != null)
                _OnAfterRowActivate(sender, e);
            // установим запись в определенное состояние... Зависит от настроек компонента в плане редактирования
            SetRowToState(ugData.ActiveRow);
            CheckCopyRowButtonVisible();
        }

        private void _ugData_CellDataError(object sender, CellDataErrorEventArgs e)
        {
            e.RaiseErrorEvent = false;
            if (_OnGridCellError != null)
                _OnGridCellError(sender, e);

            isSaveChanges = false;

        }

        #region работа с календарем выбора даты
        // создание редактора с календарем
        EditorWithMask EditorWithMaskForCalendarColumns = new Infragistics.Win.EditorWithMask();

        Infragistics.Win.UltraWinEditors.DropDownEditorButton editButton = new DropDownEditorButton("CalendarButton");

        private void SetEditors()
        {
            editButton.Enabled = true;
            editButton.Control = this.monthCalendar1;
            editButton.RightAlignDropDown = DefaultableBoolean.True;
            if (!EditorWithMaskForCalendarColumns.ButtonsRight.Exists("CalendarButton"))
                EditorWithMaskForCalendarColumns.ButtonsRight.Add(editButton);
            editButton.BeforeDropDown += new BeforeEditorButtonDropDownEventHandler(editButton_BeforeDropDown);

        }

        [Category("Public variables")]
        private DateTime _minCalendarDate;
        [Description("Минимальная дата для календаря")]
        public DateTime MinCalendarDate
        {
            get { return _minCalendarDate; }
            set { _minCalendarDate = value; }
        }

        [Category("Public variables")]
        private DateTime _maxCalendarDate;
        [Description("Максимальная дата для календаря")]
        public DateTime MaxCalendarDate
        {
            get { return _maxCalendarDate; }
            set { _maxCalendarDate = value; }
        }

        void editButton_BeforeDropDown(object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            if (_dropDownCalendar != null)
            {
                _dropDownCalendar(this);

                monthCalendar1.MaxDate = _maxCalendarDate.AddYears(10);
                monthCalendar1.MinDate = _minCalendarDate.AddYears(-10);

                DateTime cuurentDate = DateTime.Now;
                if (_maxCalendarDate.Year != cuurentDate.Year)
                {
                    cuurentDate = _maxCalendarDate.AddMonths(1 - _maxCalendarDate.Month);
                    cuurentDate = cuurentDate.AddDays(1 - cuurentDate.Day);
                    monthCalendar1.SelectionStart = cuurentDate;
                    monthCalendar1.SelectionEnd = cuurentDate;
                }

                monthCalendar1.MaxDate = _maxCalendarDate;
                monthCalendar1.MinDate = _minCalendarDate;
            }
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (_OnDataSelect != null)
                _OnDataSelect(sender, e);
            editButton.CloseUp();
        }


        private void monthCalendar1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !(e.Control && e.Alt && e.Shift))
            {
                DateRangeEventArgs eventArg = new DateRangeEventArgs(monthCalendar1.SelectionStart, monthCalendar1.SelectionEnd);
                if (_OnDataSelect != null)
                    _OnDataSelect(sender, eventArg);
                editButton.CloseUp();
            }

            if (e.KeyCode == Keys.Escape && !(e.Control && e.Alt && e.Shift))
                editButton.CloseUp();
        }

        #endregion

        private void _ugData_BeforeCellDeactivate(object sender, CancelEventArgs e)
        {
            if (_OnBeforeCellDeactivate != null)
                _OnBeforeCellDeactivate(sender, e);
        }

        public void BurnChangesDataButtons(bool burn)
        {
            InfragisticsHelper.BurnTool(utmMain.Tools["SaveChange"], burn);
            InfragisticsHelper.BurnTool(utmMain.Tools["CancelChange"], burn);
        }

        public void BurnRefreshDataButton(bool burn)
        {
            InfragisticsHelper.BurnTool(utmMain.Tools["Refresh"], burn);
        }

        private void _ugData_CellChange(object sender, CellEventArgs e)
        {
            BurnChangesDataButtons(true);
            if (_onCellChange != null)
                _onCellChange(sender, e);
        }

        private void EnterExitEditMode()
        {
            if (ugData.ActiveCell == null)
                return;
            if (ugData.ActiveCell.IsInEditMode)
                ugData.PerformAction(UltraGridAction.ExitEditMode);
            else
                ugData.PerformAction(UltraGridAction.EnterEditMode);
        }


        private void EnterEditMode()
        {
            UltraGridCell cell = ugData.ActiveCell;
            if (cell == null)
                return;
            if (cell.IsInEditMode)
            {
                if (cell.ValueListResolved != null && cell.ValueListResolved.IsDroppedDown)
                {
                    // для вызванного списка данных через клавиатуру закроем этот список
                    cell.ValueListResolved.CloseUp();
                    cell.Activate();
                    ugData.PerformAction(UltraGridAction.ExitEditMode);
                }
                return;
            }

            ugData.PerformAction(UltraGridAction.EnterEditMode);
        }

        private void _ugData_Leave(object sender, EventArgs e)
        {
            // любое значение, которое введено не по маске при потере фокуса грида считаем пустым
            if (ugData.ActiveCell != null)
            {
                if (ugData.ActiveCell.Column.Editor != null && ugData.ActiveCell.IsInEditMode)
                    if (!ugData.ActiveCell.Column.Editor.IsValid)
                        ugData.ActiveCell.CancelUpdate();
            }
        }

        private void _ugData_Error(object sender, Infragistics.Win.UltraWinGrid.ErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void _ugData_BeforeRowRegionScroll(object sender, BeforeRowRegionScrollEventArgs e)
        {
        }

        private void UltraGridEx_Load(object sender, EventArgs e)
        {
            ugData.CreationFilter = this;
        }

        public enum fileExtensions { unknown, xml, xls };

        /// <summary>
        /// создает файловый диалог для записи/чтения файлов XML или Excel
        /// </summary>
        /// <param name="startFileName"></param>
        /// <param name="fileExt"></param>
        /// <param name="toSave"></param>
        /// <param name="finishFileName"></param>
        /// <returns></returns>
        public static bool GetFileName(string startFileName, fileExtensions fileExt, bool toSave, ref string finishFileName)
        {
            FileDialog dlg = toSave ? (FileDialog)new SaveFileDialog() : (FileDialog)new OpenFileDialog();
            string fileExtension = fileExt.ToString();
            fileExtension = fileExtension.Replace(".", string.Empty);
            string extension = Path.GetExtension(startFileName).ToUpper();
            if (string.Compare(fileExtension, extension) != 0)
                startFileName = startFileName + "." + fileExtension;

            string upperFileExtension = fileExtension.ToUpper();
            switch (upperFileExtension)
            {
                case "XLS":
                    dlg.Filter = "Excel документы *.xls|*.xls";
                    break;
                case "XML":
                    dlg.Filter = "XML документы *.xml|*.xml";
                    break;
                default: dlg.Filter = string.Format("Произвольные документы *.{0}|*.{0}", fileExtension);
                    break;
            }

            dlg.FileName = GetCorrectFileName(startFileName);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                finishFileName = dlg.FileName;
                dlg.Dispose();
                return true;
            }

            dlg.Dispose();
            return false;
        }

        private static string GetCorrectFileName(string fileName)
        {
            string correctFileName = fileName;
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char chr in invalidChars)
            {
                if (correctFileName.IndexOf(chr) != -1)
                    correctFileName = correctFileName.Replace(chr, '_');
            }
            return correctFileName;
        }


        private void _ugData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EnterEditMode();
        }

        private void UltraGridEx_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void _ugData_AfterPerformAction(object sender, AfterUltraGridPerformActionEventArgs e)
        {

        }
    }
}