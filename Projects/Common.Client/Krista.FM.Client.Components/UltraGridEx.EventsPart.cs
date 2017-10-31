using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

namespace Krista.FM.Client.Components
{
    #region Делегаты
    public delegate HierarchyInfo GetHierarchyInfo(object sender);
    public delegate DataRelation GetHierarchyRelation(object sender);
    public delegate void NeedLoadChildRows(object sender, int parentID);

    // лукапы 
    public delegate string GetLookupValueDelegate(object sender, string lookupName, bool needFoolValue, object value);
    public delegate bool CheckLookupValueDelegate(object sender, string lookupName, object value);

    // для вызова справочника из серверного фильтра
    public delegate bool GetHandbookValue(object sender, string columnName, ref object handbookValue);
    // список колонок которые могут входить в фильтр
    public delegate void GetServerFilterCustomDialogColumnsList(object sender, ValueList valueList, string columnName);

    public delegate GridColumnsStates GetGridColumnsState(object sender);
    public delegate bool RefreshData(object sender);
    public delegate void DataWorking(object sender);
    public delegate bool SaveChanges(object sender);
    public delegate void AfterRowInsert(object sender, UltraGridRow row);
    public delegate void AfterRowsDelete(object sender, BeforeRowsDeletedEventArgs e);
    public delegate void BeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e);
    // делегаты для реализации Drag&Drop грида компонента
    public delegate void SelectionDrag(object sender, CancelEventArgs e);
    public delegate void MainMouseEvents(object sender, DragEventArgs e);
    public delegate void DragLeave(object sender, EventArgs e);
    // делегат для инициализации записей в гриде
    public delegate void InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e);
    public delegate void ClickCellButton(object sender, CellEventArgs e);
    public delegate void MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e);
    public delegate void MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e);
    public delegate void BeforeRowDeactivate(object sender, CancelEventArgs e);
    public delegate void BeforeCellDeactivate(object sender, CancelEventArgs e);
    public delegate void AftertImportFromXML(object sender, int RowsCountBeforeImport);
    public delegate void BeforeCellActivate(object sender, CancelableCellEventArgs e);
    public delegate void ChangeHierarchyView(object sender, bool inHierarchy);
    public delegate bool SaveLoadXML(object sender);
    public delegate void AfterRowActivate(object sender, EventArgs e);
    public delegate void DataSelect(object sender, DateRangeEventArgs e);
    public delegate void DropDownCalendar(object sender);
    public delegate void ToolBarToolsClick(object sender, ToolClickEventArgs e);
    public delegate void GridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e);

    public delegate void GridCellError(object sender, CellDataErrorEventArgs e);

    public delegate void CreateUIElement(object sender, Infragistics.Win.UIElement parent);
    public delegate void UIElementClick(object sender, UIElementEventArgs e);

    public delegate void ColumnHideShow(object sender, string columnName, bool isColumnHide);

    public delegate void DeleteDataSource(object sender);

    public delegate void CellChange(object sender, CellEventArgs e);

    public delegate void HierarchyChange(object sender);

    #endregion

    public partial class UltraGridEx : UserControl, Infragistics.Win.IUIElementCreationFilter
    {

        #region События
        private NeedLoadChildRows _onNeedLoadChildRows = null;
        public event NeedLoadChildRows OnNeedLoadChildRows
        {
            add { _onNeedLoadChildRows += value; }
            remove { _onNeedLoadChildRows -= value; }
        }

        private GetHierarchyInfo _onGetHierarchyInfo;
        public event GetHierarchyInfo OnGetHierarchyInfo
        {
            add { _onGetHierarchyInfo += value; }
            remove { _onGetHierarchyInfo -= value; }
        }

        private GetHierarchyRelation _onGetHierarchyRelation = null;
        [Category("Internal variables")]
        [Description("Отношение (relation) для иерархии")]
        public event GetHierarchyRelation OnGetHierarchyRelation
        {
            add { _onGetHierarchyRelation += value; }
            remove { _onGetHierarchyRelation -= value; }
        }

        private GetGridColumnsState _onGetGridColumnsState = null;
        [Category("Internal events")]
        [Description("Вызывается при загрузке данных о заголовках колонок")]
        public event GetGridColumnsState OnGetGridColumnsState
        {
            add { _onGetGridColumnsState += value; }
            remove { _onGetGridColumnsState -= value; }
        }

        private AfterRowActivate _OnAfterRowActivate = null;
        [Category("Internal events")]
        [Description("Вызывается при переходе на другую стоку в гриде")]
        public event AfterRowActivate OnAfterRowActivate
        {
            add { _OnAfterRowActivate += value; }
            remove { _OnAfterRowActivate -= value; }
        }

        private GridInitializeLayout _OnGridInitializeLayout = null;
        [Category("Internal events")]
        [Description("Инициализация внешнего вида грида")]
        public event GridInitializeLayout OnGridInitializeLayout
        {
            add { _OnGridInitializeLayout += value; }
            remove { _OnGridInitializeLayout -= value; }
        }

        private RefreshData _onRefreshData = null;
        /// <summary>
        /// Обновление данных
        /// </summary>
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Обновить данные'")]
        public event RefreshData OnRefreshData
        {
            add { _onRefreshData += value; }
            remove { _onRefreshData -= value; }
        }

        private SaveChanges _onSaveChanges = null;
        /// <summary>
        /// Сохранение изменений
        /// </summary>
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Сохранить изменения'")]
        public event SaveChanges OnSaveChanges
        {
            add { _onSaveChanges += value; }
            remove { _onSaveChanges -= value; }
        }

        private DataWorking _onCancelChanges = null;
        /// <summary>
        /// Отмена изменений
        /// </summary>
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Отменить изменения'")]
        public event DataWorking OnCancelChanges
        {
            add { _onCancelChanges += value; }
            remove { _onCancelChanges -= value; }
        }

        private DataWorking _onClearCurrentTable = null;
        /// <summary>
        /// Очистка текущей таблицы
        /// </summary>
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Отменить изменения'")]
        public event DataWorking OnClearCurrentTable
        {
            add { _onClearCurrentTable += value; }
            remove { _onClearCurrentTable -= value; }
        }

        private SaveLoadXML _OnSaveToXML = null;
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Сохранить в XML'")]
        public event SaveLoadXML OnSaveToXML
        {
            add { _OnSaveToXML += value; }
            remove { _OnSaveToXML -= value; }
        }

        private SaveLoadXML _OnSaveToDataSetXML = null;
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Сохранить в XML'")]
        public event SaveLoadXML OnSaveToDataSetXML
        {
            add { _OnSaveToDataSetXML += value; }
            remove { _OnSaveToDataSetXML -= value; }
        }

        private SaveLoadXML _OnLoadFromXML = null;
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Загрузить из XML'")]
        public event SaveLoadXML OnLoadFromXML
        {
            add { _OnLoadFromXML += value; }
            remove { _OnLoadFromXML -= value; }
        }

        private SaveLoadXML _onLoadFromExcel = null;
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Загрузить из Excel'")]
        public event SaveLoadXML OnLoadFromExcel
        {
            add { _onLoadFromExcel += value; }
            remove { _onLoadFromExcel -= value; }
        }

        private SaveLoadXML _onSaveToExcel = null;
        [Category("Internal events")]
        [Description("Вызывается при нажатии на кнопку 'Сохранить в Excel'")]
        public event SaveLoadXML OnSaveToExcel
        {
            add { _onSaveToExcel += value; }
            remove { _onSaveToExcel -= value; }
        }

        ToolBarToolsClick _toolClick = null;
        [Category("Internal events")]
        [Description("Вызывается при клике мышкой на элемент тулбара, добавленный пользователем")]
        public event ToolBarToolsClick ToolClick
        {
            add { _toolClick += value; }
            remove { _toolClick -= value; }
        }

        private ChangeHierarchyView _OnChangeHierarchyView = null;
        [Category("Internal events")]
        [Description("Вызывается при смене вида грида")]
        public event ChangeHierarchyView OnChangeHierarchyView
        {
            add { _OnChangeHierarchyView += value; }
            remove { _OnChangeHierarchyView -= value; }
        }

        private AftertImportFromXML _OnAftertImportFromXML = null;
        [Category("Internal events")]
        [Description("Вызывается после импорта данных из XML для дальнейшей обработки данных")]
        public event AftertImportFromXML OnAftertImportFromXML
        {
            add { _OnAftertImportFromXML += value; }
            remove { _OnAftertImportFromXML -= value; }
        }

        private BeforeRowsDelete _OnBeforeRowsDelete = null;
        [Category("Internal events")]
        [Description("Вызывается перед удаления строк")]
        public event BeforeRowsDelete OnBeforeRowsDelete
        {
            add { _OnBeforeRowsDelete += value; }
            remove { _OnBeforeRowsDelete -= value; }
        }

        // после добавления записи проставляем картинку
        private AfterRowInsert _OnAfterRowInsert = null;
        [Category("Internal events")]
        [Description("Вызывается после добавления строк")]
        public event AfterRowInsert OnAfterRowInsert
        {
            add { _OnAfterRowInsert += value; }
            remove { _OnAfterRowInsert -= value; }
        }
        // удаление строк 
        private AfterRowsDelete _OnAfterRowsDelete = null;
        [Category("Internal events")]
        [Description("Вызывается после удаления строк")]
        public event AfterRowsDelete OnAfterRowsDelete
        {
            add { _OnAfterRowsDelete += value; }
            remove { _OnAfterRowsDelete -= value; }
        }
        private MainMouseEvents _OnDragEnter = null;
        [Category("Drag&Drop events")]
        [Description("Обработчик входа в элемент грида при Drag&Drop")]
        public event MainMouseEvents GridDragEnter
        {
            add { _OnDragEnter += value; }
            remove { _OnDragEnter -= value; }
        }

        private MainMouseEvents _OnDragDrop = null;
        [Category("Drag&Drop events")]
        [Description("Основная процедура Drag&Drop'а")]
        public event MainMouseEvents GridDragDrop
        {
            add { _OnDragDrop += value; }
            remove { _OnDragDrop -= value; }
        }

        private DragLeave _OnDragLeave = null;
        [Category("Drag&Drop events")]
        [Description("Выход из Drag&Drop'а")]
        public event DragLeave GridDragLeave
        {
            add { _OnDragLeave += value; }
            remove { _OnDragLeave -= value; }
        }

        private MainMouseEvents _OnDragOver = null;
        [Category("Drag&Drop events")]
        [Description("Перемещение над элементами грида")]
        public event MainMouseEvents GridDragOver
        {
            add { _OnDragOver += value; }
            remove { _OnDragOver -= value; }
        }

        private SelectionDrag _OnSelectionDrag = null;
        [Category("Drag&Drop events")]
        [Description("Запускает Drag&Drop для одной записи")]
        public event SelectionDrag GridSelectionDrag
        {
            add { _OnSelectionDrag += value; }
            remove { _OnSelectionDrag -= value; }
        }

        private InitializeRow _OnInitializeRow = null;
        [Category("Internal events")]
        [Description("Инициализация каждой записи")]
        public event InitializeRow OnInitializeRow
        {
            add { _OnInitializeRow += value; }
            remove { _OnInitializeRow -= value; }
        }

        private ClickCellButton _OnClickCellButton = null;
        [Category("Internal events")]
        [Description("Вызывается при нажатии кнопки в ячейке грида")]
        public event ClickCellButton OnClickCellButton
        {
            add { _OnClickCellButton += value; }
            remove { _OnClickCellButton -= value; }
        }

        private BeforeRowDeactivate _OnBeforeRowDeactivate = null;
        [Category("Internal events")]
        [Description("Вызывается перед снятия активности со строки грида")]
        public event BeforeRowDeactivate OnBeforeRowDeactivate
        {
            add { _OnBeforeRowDeactivate += value; }
            remove { _OnBeforeRowDeactivate -= value; }
        }

        // событие для различных манипуляций с нажатием кнопок грида и выводом хинтов
        private MouseEnterElement _OnMouseEnterGridElement = null;
        [Category("Internal events")]
        [Description("Вызывается при нахождении курсора над каким либо элементом грида")]
        public event MouseEnterElement OnMouseEnterGridElement
        {
            add { _OnMouseEnterGridElement += value; }
            remove { _OnMouseEnterGridElement -= value; }
        }

        private MouseLeaveElement _OnMouseLeaveGridElement = null;
        [Category("Internal events")]
        [Description("Вызывается при уходя курсора с какого либо элемента грида")]
        public event MouseLeaveElement OnMouseLeaveGridElement
        {
            add { _OnMouseLeaveGridElement += value; }
            remove { _OnMouseLeaveGridElement -= value; }
        }

        // События на перемещение между ячейками
        private BeforeCellActivate _OnBeforeCellActivate = null;
        public event BeforeCellActivate OnBeforeCellActivate
        {
            add { _OnBeforeCellActivate += value; }
            remove { _OnBeforeCellActivate -= value; }
        }

        private GridCellError _OnGridCellError = null;
        /// <summary>
        /// событие на ввод данных не по маске или не верного типа
        /// </summary>
        public event GridCellError OnGridCellError
        {
            add { _OnGridCellError += value; }
            remove { _OnGridCellError -= value; }
        }

        private BeforeRowFilterDropDownPopulateEventHandler _onBeforeRowFilterDropDownPopulateEventHandler = null;

        public event BeforeRowFilterDropDownPopulateEventHandler OnBeforeRowFilterDropDownPopulateEventHandler
        {
            add { _onBeforeRowFilterDropDownPopulateEventHandler += value; }
            remove { _onBeforeRowFilterDropDownPopulateEventHandler -= value; }
        }

        DropDownCalendar _dropDownCalendar = null;

        [Category("Internal events")]
        [Description("Вызывается при вызове календаря")]
        public event DropDownCalendar OnDropDownCalendar
        {
            add { _dropDownCalendar += value; }
            remove { _dropDownCalendar -= value; }
        }

        DataSelect _OnDataSelect = null;
        [Category("Internal events")]
        [Description("Вызывается при выборе даты из календаря")]
        public event DataSelect OnDataSelect
        {
            add { _OnDataSelect += value; }
            remove { _OnDataSelect -= value; }
        }

        private BeforeCellDeactivate _OnBeforeCellDeactivate = null;
        public event BeforeCellDeactivate OnBeforeCellDeactivate
        {
            add { _OnBeforeCellDeactivate += value; }
            remove { _OnBeforeCellDeactivate -= value; }
        }

        CreateUIElement _createUIElement = null;

        [Category("Internal events")]
        [Description("Вызывается при создании визуальных элементов грида")]
        public event CreateUIElement OnCreateUIElement
        {
            add { _createUIElement += value; }
            remove { _createUIElement -= value; }
        }

        UIElementClick _UIElementClick = null;

        [Category("Internal events")]
        [Description("Вызывается при клике на элементе")]
        public event UIElementClick OnUIElementClick
        {
            add { _UIElementClick += value; }
            remove { _UIElementClick -= value; }
        }

        private GetLookupValueDelegate _onGetLookupValue = null;
        public event GetLookupValueDelegate OnGetLookupValue
        {
            add { _onGetLookupValue += value; }
            remove { _onGetLookupValue -= value; }
        }

        private CheckLookupValueDelegate _onCheckLookupValue = null;
        public event CheckLookupValueDelegate OnCheckLookupValue
        {
            add { _onCheckLookupValue += value; }
            remove { _onCheckLookupValue -= value; }
        }

        private ColumnHideShow _onAfterColumnHideShow = null;
        public event ColumnHideShow OnAfterColumnHideShow
        {
            add { _onAfterColumnHideShow += value; }
            remove { _onAfterColumnHideShow -= value; }
        }

        private DeleteDataSource _onDeleteDataSource;
        public event DeleteDataSource OnDeleteDataSource
        {
            add { _onDeleteDataSource += value; }
            remove { _onDeleteDataSource -= value; }
        }

        private CellChange _onCellChange;
        public event CellChange OnCellChange
        {
            add { _onCellChange += value; }
            remove { _onCellChange -= value; }
        }

        private HierarchyChange _beforeHierarcyChange;
        public event HierarchyChange OnBeforeHierarchyChange
        {
            add { _beforeHierarcyChange += value; }
            remove { _beforeHierarcyChange -= value; }
        }

        private HierarchyChange _afterHierarcyChange;
        public event HierarchyChange OnAfterHierarchyChange
        {
            add { _afterHierarcyChange += value; }
            remove { _afterHierarcyChange -= value; }
        }

        #endregion
    }
}