using System;
using System.Windows.Forms;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinToolbars;
using System.Drawing;
using System.Xml;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.BrowseAdapters.ChartReportAdapters.ChartEditors;
using Krista.FM.Client.MDXExpert.BrowseAdapters.MapReportAdapters.MapEditors;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;


namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Ключи элементов в панели инструментов, относящихся к таблице
    /// </summary>
    public class TableToolbar
    {
        #region Ключи
        //вкладок
        //вкладки строк таблицы
        public const string rowTabKey = "TableRowTab";
        //вкладки колонок таблицы
        public const string columnTabKey = "TableColumnTab";
        //вкладки мер таблицы
        public const string measureTabKey = "TableMeasureTab";
        //вкладка Элемент
        private const string elementTabKey = "ElementTab";

        //групп
        //поведение
        public const string behaviorGroupKey = "BehaviorGroup";
        //элемент сток
        public const string selectedRowElementGroupKey = "SelectedObjectGroup";
        //элемент столбцов
        public const string selectedColumnElementGroupKey = "SelectedObjectGroup";
        //мера
        public const string selectedMeasureGroupKey = "SelectedObjectGroup";
        //вычислимые меры
        public const string customMeasureGroupKey = "CustomMeasureGroup";
        //управление данными
        public const string dataManageGroupKey = "DataManageGroup";
        //создание элементов по таблице
        public const string elementsByTableGroupKey = "ElementsByTableGroup";
        //функции анализа
        public const string analysisFunctionsGroupKey = "AnalysisFunctionsGroup";


        //инструментов
        //общие для всех элементов таблицы
        public const string axisGrandTotalVisibleKey = "AxisGrandTotalVisible";
        public const string axisHideEmptyPositionsKey = "AxisHideEmptyPositions";
        public const string axisPropertiesDisplayTypeKey = "AxisPropertiesDisplayType";
        public const string captionStyleGalleryKey = "TableCaptionStyleGallery";
        public const string cellStyleGalleryKey = "TableCellStyleGallery";
        public const string memberPropertiesEditorContainerKey = "MemberPropertiesContainer";
        public const string axisSortTypeKey = "AxisSortType";

        public const string createChartByTableKey = "CreateChartByTable";
        public const string createMapByTableKey = "CreateMapByTable";

        //меры
        public const string totalStyleGalleryKey = "TableTotalStyleGallery";
        public const string measureEditorContainerKey = "MeasureEditorContainer";
        public const string addCalcTotalKey = "AddCalcTotal";
        public const string deleteCalcTotalKey = "DeleteCalcTotal";
        public const string measureFormulaKey = "MeasureFormula";
        public const string colorRulesKey = "ColorRules";

        //строки
        public const string autoSizeRowsKey = "AutoSizeRows";
        public const string calcAverageKey = "CalcAverage";
        public const string calcMedianKey = "CalcMedian";
        public const string calcTopCountKey = "CalcTopCount";
        public const string calcBottomCountKey = "CalcBottomCount";
        #endregion

        #region контролы таблицы
        //вкладки таблицы
        /// <summary>
        /// Вкладка со свойствами строк
        /// </summary>
        private RibbonTab rowTab;
        /// <summary>
        /// Вкладка со свойствами колонок
        /// </summary>
        private RibbonTab columnTab;
        /// <summary>
        /// Вкладка со свойствами мер
        /// </summary>
        private RibbonTab measureTab;
        /// <summary>
        /// Вкладка элемент
        /// </summary>
        private RibbonTab elementTab;

        //группы
        /// <summary>
        /// Поведение сторк
        /// </summary>
        private RibbonGroup rowBehaviorGroup;
        /// <summary>
        /// Выделенный элемент строк
        /// </summary>
        private RibbonGroup selectedRowElementGroup;
        /// <summary>
        /// Управление данными строк
        /// </summary>
        private RibbonGroup rowDataManageGroup;
        /// <summary>
        /// Поведение столбцов
        /// </summary>
        private RibbonGroup columnBehaviorGroup;
        /// <summary>
        /// Выделенный элемент столбцов
        /// </summary>
        private RibbonGroup selectedColumnElementGroup;
        /// <summary>
        /// Управление данными столбцов
        /// </summary>
        private RibbonGroup columnDataManageGroup;
        /// <summary>
        /// Выделенная мера
        /// </summary>
        private RibbonGroup selectedMeasureGroup;
        /// <summary>
        /// Пользовательсие меры
        /// </summary>
        private RibbonGroup customMeasureGroup;
        /// <summary>
        /// Создание элементов по таблице
        /// </summary>
        private RibbonGroup elementsByTableGroup;
        /// <summary>
        /// Функции анализа
        /// </summary>
        private RibbonGroup analysisFunctionsGroup;

        //строки
        /// <summary>
        /// Автоматический расчет высоты ячейки у строк
        /// </summary>
        private StateButtonTool autoSizeRows;
        /// <summary>
        /// Расчет среднего
        /// </summary>
        private ButtonTool calcAverage;
        /// <summary>
        /// Расчет медианы
        /// </summary>
        private ButtonTool calcMedian;
        /// <summary>
        /// Расчет k-первых
        /// </summary>
        private ButtonTool calcTopCount;
        /// <summary>
        /// Расчет k-последних
        /// </summary>
        private ButtonTool calcBottomCount;

        //общие
        /// <summary>
        /// Показывать главный итог оси
        /// </summary>
        private StateButtonTool axisGrandTotalVisible;
        /// <summary>
        /// Скрывать пустые элементы оси
        /// </summary>
        private StateButtonTool axisHideEmptyPositions;
        /// <summary>
        /// Место отображения свойств элементов оси
        /// </summary>
        private ComboBoxTool axisPropertiesDisplayType;
        /// <summary>
        /// Сортировка элементов оси
        /// </summary>
        private ComboBoxTool axisSortType;
        /// <summary>
        /// Галерея стилей для заголовков
        /// </summary>
        private PopupGalleryTool captionStyleGallery;
        /// <summary>
        /// Галерея стилей для ячеек
        /// </summary>
        private PopupGalleryTool cellStyleGallery;
        /// <summary>
        /// Создание диаграммы по таблице
        /// </summary>
        private ButtonTool createChartByTable;
        /// <summary>
        /// Создание карты по таблице
        /// </summary>
        private ButtonTool createMapByTable;

        //меры
        /// <summary>
        /// Галерея стилей итогов у мер
        /// </summary>
        private PopupGalleryTool totalStyleGallery;
        /// <summary>
        /// Контейнер, для редактирования меры
        /// </summary>
        private ControlContainerTool measureEditorContainer;
        /// <summary>
        /// Контрол, редактирующий выделенную меру
        /// </summary>
        private MeasureEditor measureEditor;
        /// <summary>
        /// Контейнер, для редактирования свойств элемента
        /// </summary>
        private PopupControlContainerTool memberPropertiesContainer;
        /// <summary>
        /// Редактор свойств элементов
        /// </summary>
        private MemberPropertiesControl memberPropertiesEditor;
        /// <summary>
        /// Добавить вычисляемый показатель
        /// </summary>
        private ButtonTool addCalcTotal;
        /// <summary>
        /// Удалить вычисляемый показатель
        /// </summary>
        private ButtonTool deleteCalcTotal;
        /// <summary>
        /// Редактировать формулу вычислимой меры
        /// </summary>
        private ButtonTool measureFormula;
        /// <summary>
        /// Условная раскраска
        /// </summary>
        private ButtonTool colorRules;

        #endregion

        //Переменный под хранение уникальных имен, последних not Null выделенных объектов
        private string lastNotNullSelectedLevel = string.Empty;
        private string lastNotNullSelectedMeasure = string.Empty;

        private TableReportElement _activeTableElement;
        private ToolbarsManage _toolbarsManager;
        private ActiveGalleryType _activeGallery;
        //стиль элемента таблицы до редактирования
        private CellStyle befoEditStyle = null;
        //Если true - то события контролов не выполняются 
        private bool isMayHook = false;

        public TableToolbar(ToolbarsManage toolbarManager)
        {
            this.ToolbarsManager = toolbarManager;
        }

        public void Initialize()
        {
            //Устанавливаем ссылки на контролы таблицы
            this.SetLinkOnTools();
            //Устанавливаем заголовкам вкладок русские имена
            this.SetRusTabCaption();
            //Устанавливаем контролам таблицы, требуемые обработчики
            this.SetEventHandlers();
            //Скроем все группы, которые отображают свойства выделенного объекта
            this.RefreshSelectedGroups(null);
        }

        /// <summary>
        /// Устанавливаем ссылки на контролы таблицы
        /// </summary>
        private void SetLinkOnTools()
        {
            //Вкладки
            //Строки таблицы
            this.rowTab = this.Toolbars.Ribbon.Tabs[rowTabKey];
            //Колонки таблицы
            this.columnTab = this.Toolbars.Ribbon.Tabs[columnTabKey];
            //Меры таблицы
            this.measureTab = this.Toolbars.Ribbon.Tabs[measureTabKey];
            //Элемент
            this.elementTab = this.Toolbars.Ribbon.Tabs[elementTabKey];
            
            //Группы
            this.rowBehaviorGroup = this.rowTab.Groups[behaviorGroupKey];
            this.selectedRowElementGroup = this.rowTab.Groups[selectedRowElementGroupKey];
            this.rowDataManageGroup = this.rowTab.Groups[dataManageGroupKey];
            this.columnBehaviorGroup = this.columnTab.Groups[behaviorGroupKey];
            this.selectedColumnElementGroup = this.columnTab.Groups[selectedColumnElementGroupKey];
            this.columnDataManageGroup = this.columnTab.Groups[dataManageGroupKey];
            this.customMeasureGroup = this.measureTab.Groups[customMeasureGroupKey];
            this.selectedMeasureGroup = this.measureTab.Groups[selectedMeasureGroupKey];
            this.elementsByTableGroup = this.elementTab.Groups[elementsByTableGroupKey];
            this.analysisFunctionsGroup = this.rowTab.Groups[analysisFunctionsGroupKey];

            //Инструменты
            //создание элементов по таблице
            this.createChartByTable = (ButtonTool)this.Toolbars.Tools[createChartByTableKey];
            this.createMapByTable = (ButtonTool)this.Toolbars.Tools[createMapByTableKey];

            //строки
            this.autoSizeRows = (StateButtonTool)this.Toolbars.Tools[autoSizeRowsKey];
            this.calcAverage = (ButtonTool)this.Toolbars.Tools[calcAverageKey];
            this.calcMedian = (ButtonTool)this.Toolbars.Tools[calcMedianKey];
            this.calcTopCount = (ButtonTool)this.Toolbars.Tools[calcTopCountKey];
            this.calcBottomCount = (ButtonTool)this.Toolbars.Tools[calcBottomCountKey];
            //общие
            this.axisGrandTotalVisible = (StateButtonTool)this.Toolbars.Tools[axisGrandTotalVisibleKey];
            this.axisHideEmptyPositions = (StateButtonTool)this.Toolbars.Tools[axisHideEmptyPositionsKey];
            this.axisPropertiesDisplayType = (ComboBoxTool)this.Toolbars.Tools[axisPropertiesDisplayTypeKey];
            this.captionStyleGallery = (PopupGalleryTool)this.Toolbars.Tools[captionStyleGalleryKey];
            this.cellStyleGallery = (PopupGalleryTool)this.Toolbars.Tools[cellStyleGalleryKey];
            this.axisSortType = (ComboBoxTool)this.Toolbars.Tools[axisSortTypeKey];
            this.memberPropertiesContainer = (PopupControlContainerTool)this.Toolbars.Tools[memberPropertiesEditorContainerKey];
            this.memberPropertiesEditor = (MemberPropertiesControl)this.memberPropertiesContainer.Control;
            //меры
            this.totalStyleGallery = (PopupGalleryTool)this.Toolbars.Tools[totalStyleGalleryKey];
            this.measureEditorContainer = (ControlContainerTool)this.Toolbars.Tools[measureEditorContainerKey];
            this.measureEditor = (MeasureEditor)this.measureEditorContainer.Control;
            this.addCalcTotal = (ButtonTool)this.Toolbars.Tools[addCalcTotalKey];
            this.deleteCalcTotal = (ButtonTool)this.Toolbars.Tools[deleteCalcTotalKey];
            this.measureFormula = (ButtonTool)this.Toolbars.Tools[measureFormulaKey];
            this.colorRules = (ButtonTool)this.Toolbars.Tools[colorRulesKey];
        }

        /// <summary>
        /// Установить обработчики событий
        /// </summary>
        private void SetEventHandlers()
        {
            //закладки
            this.Toolbars.AfterRibbonTabSelected += new RibbonTabEventHandler(Toolbars_AfterRibbonTabSelected);
            this.Toolbars.AfterRibbonTabDropDown += new RibbonTabEventHandler(Toolbars_AfterRibbonTabDropDown);

            //строки
            this.autoSizeRows.ToolClick += new ToolClickEventHandler(autoSizeRows_ToolClick);
            this.calcAverage.ToolClick += new ToolClickEventHandler(calcAverage_ToolClick);
            this.calcMedian.ToolClick += new ToolClickEventHandler(calcMedian_ToolClick);
            this.calcTopCount.ToolClick += new ToolClickEventHandler(calcTopCount_ToolClick);
            this.calcBottomCount.ToolClick += new ToolClickEventHandler(calcBottomCount_ToolClick);


            //общие
            this.createChartByTable.ToolClick += new ToolClickEventHandler(createChartByTable_ToolClick);
            this.createMapByTable.ToolClick += new ToolClickEventHandler(createMapByTable_ToolClick);

            this.axisGrandTotalVisible.ToolClick += new ToolClickEventHandler(axisGrandTotalVisible_ToolClick);
            this.axisHideEmptyPositions.ToolClick += new ToolClickEventHandler(axisHideEmptyPositions_ToolClick);
            this.axisPropertiesDisplayType.ToolValueChanged += new ToolEventHandler(axisPropertiesDisplayType_ToolValueChanged);
            this.captionStyleGallery.GalleryToolActiveItemChange += new GalleryToolItemEventHandler(captionStyleGallery_GalleryToolActiveItemChange);
            this.captionStyleGallery.GalleryToolItemClick += new GalleryToolItemEventHandler(captionStyleGallery_GalleryToolItemClick);
            this.cellStyleGallery.GalleryToolActiveItemChange += new GalleryToolItemEventHandler(cellStyleGallery_GalleryToolActiveItemChange);
            this.cellStyleGallery.GalleryToolItemClick += new GalleryToolItemEventHandler(cellStyleGallery_GalleryToolItemClick);
            this.memberPropertiesEditor.VisibleChanged += new EventHandler(memberPropertiesEditor_VisibleChanged);
            this.memberPropertiesContainer.BeforeToolDropdown += new BeforeToolDropdownEventHandler(memberPropertiesContainer_BeforeToolDropdown);
            this.axisSortType.ToolValueChanged += new ToolEventHandler(axisSortType_ToolValueChanged);
            //меры
            this.totalStyleGallery.GalleryToolActiveItemChange += new GalleryToolItemEventHandler(totalStyleGallery_GalleryToolActiveItemChange);
            this.totalStyleGallery.GalleryToolItemClick += new GalleryToolItemEventHandler(totalStyleGallery_GalleryToolItemClick);
            this.measureEditor.ExitEditMode += new EventHandler(measureEditor_ExitEditMode);
            this.addCalcTotal.ToolClick += new ToolClickEventHandler(addCalcTotal_ToolClick);
            this.deleteCalcTotal.ToolClick += new ToolClickEventHandler(deleteCalcTotal_ToolClick);
            this.measureFormula.ToolClick += new ToolClickEventHandler(measureFormula_ToolClick);
            this.colorRules.ToolClick += new ToolClickEventHandler(colorRules_ToolClick);
        }



        /// <summary>
        /// Устанавливаем заголовкам вкладок русские имена, непонятно почему, но они очень 
        /// часто меняются на ключи
        /// </summary>
        private void SetRusTabCaption()
        {
            this.rowTab.Caption = "Строки";
            this.columnTab.Caption = "Столбцы";
            this.measureTab.Caption = "Меры";
        }

        /// <summary>
        /// Обновляем значения контролов в соотвествии с данными активной таблицы
        /// </summary>
        /// <param name="activeElement"></param>
        public void RefreshTabsTools(TableReportElement activeElement)
        {
            //Удалим обратоботчики у старого активного элемента отчета
            this.RemoveHandlers(this.ActiveTableElement);

            this.ActiveTableElement = activeElement;

            //Добавим обработчики номвому элементу отчета
            this.AddHandlers(this.ActiveTableElement);

            this.DoRefreshTabsTools();
        }

        /// <summary>
        /// Обнавляем значение контролов на выделенной вкладке таблицы
        /// </summary>
        /// <param name="activeElement"></param>
        private void DoRefreshTabsTools()
        {
            if ((this.ActiveTableElement != null) && (this.ActiveTableElement.PivotData != null)
                && (this.ActiveTableElement.GridUserInterface != null))
            {
                try
                {
                    this.isMayHook = true;

                    switch (this.SelectedTab)
                    {
                        //строки
                        case SelectedTabType.Row:
                            this.RefreshRowTab(this.ActiveTableElement);
                            break;
                        //колонки
                        case SelectedTabType.Column:
                            this.RefreshColumnTab(this.ActiveTableElement);
                            break;
                        //меры
                        case SelectedTabType.Measure:
                            this.RefreshMeasureTab(this.ActiveTableElement);
                            break;
                    }
                }
                finally
                {
                    this.isMayHook = false;
                }
            }
        }

        #region Обновление групп
        private void RefreshGroups(TableReportElement activeElement)
        {
            //если у элемента пользовательский MDX, отключаем видимость некоторых групп
            bool isCustomMdx = ((activeElement != null) && activeElement.PivotData.IsCustomMDX);
            this.rowBehaviorGroup.Visible = !isCustomMdx;
            this.columnBehaviorGroup.Visible = !isCustomMdx;
            this.customMeasureGroup.Visible = !isCustomMdx;

            this.RefreshSelectedGroups(activeElement);
        }

        /// <summary>
        /// Обновляем группы зависящие от выделенного объекта
        /// </summary>
        /// <param name="tableReportElement"></param>
        private void RefreshSelectedGroups(TableReportElement activeElement)
        {
            this.SetSelectedGroupsVisible(activeElement);

            if (this.selectedMeasureGroup.Visible)
                this.measureEditor.EditingMeasure = this.GetNotNullSelectedMeasure(activeElement);

            if ((this.selectedColumnElementGroup.Visible) || (this.selectedRowElementGroup.Visible))
                this.InitializeMemberPropertiesEditor(activeElement);
        }

        /// <summary>
        /// Устанавливает видимость зависящим от выделенного объекта группам
        /// </summary>
        /// <param name="selectedObject"></param>
        private void SetSelectedGroupsVisible(TableReportElement activeElement)
        {
            PivotField field = this.GetNotNullSelectedLevel(activeElement);
            PivotTotal measure = this.GetNotNullSelectedMeasure(activeElement);
            bool isCustomMDX = (activeElement != null && activeElement.PivotData.IsCustomMDX);

            this.selectedRowElementGroup.Visible = !isCustomMDX && (field != null)
                && (field.ParentPivotData.RowAxis.GetPivotObject(field.UniqueName) != null);

            this.selectedColumnElementGroup.Visible = !isCustomMDX && (field != null)
                && (field.ParentPivotData.ColumnAxis.GetPivotObject(field.UniqueName) != null);

            this.selectedMeasureGroup.Visible = (measure != null);
            if (!this.selectedMeasureGroup.Visible)
                this.measureEditor.Visible = this.selectedMeasureGroup.Visible;

            if (this.selectedRowElementGroup.Visible)
                this.selectedRowElementGroup.Caption = "Уровень: " + field.Caption;

            if (this.selectedColumnElementGroup.Visible)
                this.selectedColumnElementGroup.Caption = "Уровень: " + field.Caption;

            if (this.selectedMeasureGroup.Visible)
                this.selectedMeasureGroup.Caption = "Мера: " + measure.Caption;

            this.SetSelectedButtonsVisible(activeElement);
        }

        /// <summary>
        /// Устанавливает видимость зависящих от выделенного объекта кнопок
        /// </summary>
        /// <param name="tableReportElement"></param>
        private void SetSelectedButtonsVisible(TableReportElement tableReportElement)
        {
            PivotTotal measure = this.GetNotNullSelectedMeasure(tableReportElement);
            this.deleteCalcTotal.SharedProps.Visible = ((measure != null) && (measure.IsCustomTotal));
            this.measureFormula.SharedProps.Visible = ((measure != null) && (measure.IsCustomTotal));
        }

        /// <summary>
        /// Устанавливает нужные обработчик отчету
        /// </summary>
        /// <param name="tableReportElement"></param>
        private void AddHandlers(TableReportElement tableReportElement)
        {
            if (tableReportElement != null)
            {
                tableReportElement.PivotData.SelectionChanged += new PivotDataEventHandler(PivotData_SelectionChanged);
                tableReportElement.PivotData.DataChanged += new PivotDataEventHandler(PivotData_DataChanged);
            }
        }

        /// <summary>
        /// Удаляет обработчики у отчета
        /// </summary>
        /// <param name="tableReportElement"></param>
        private void RemoveHandlers(TableReportElement tableReportElement)
        {
            if (tableReportElement != null)
            {
                tableReportElement.PivotData.SelectionChanged -= PivotData_SelectionChanged;
                tableReportElement.PivotData.DataChanged -= PivotData_DataChanged;
            }
        }

        void PivotData_DataChanged()
        {
            this.RefreshSelectedGroups(this.ActiveTableElement);
        }

        void PivotData_SelectionChanged()
        {
            this.RefreshSelectedGroups(this.ActiveTableElement);
        }
        #endregion

        /// <summary>
        /// Обновляет вкладку со свойствами строк
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshRowTab(TableReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshGroups(activeElement);
            this.autoSizeRows.Checked = activeElement.GridUserInterface.AutoSizeRows;
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Row);

            //Поменяем иконки галереям, т.к. они зависят от типа редактируемого объекта
            this.captionStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 65;
            this.cellStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 70;
            this.captionStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 34;
            this.cellStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 35;
        }

        /// <summary>
        /// Обновляет вкладку со свойствами колонок
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshColumnTab(TableReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshGroups(activeElement);
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Column);

            //Поменяем иконки галереям, т.к. они зависят от типа редактируемого объекта
            this.captionStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 66;
            this.cellStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 69;
            this.captionStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 32;
            this.cellStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 33;
        }

        /// <summary>
        /// Обновление вкладки со свойствами мер
        /// </summary>
        /// <param name="tableReportElement"></param>
        private void RefreshMeasureTab(TableReportElement tableReportElement)
        {
            this.RefreshGroups(tableReportElement);

            //Поменяем иконки галереям, т.к. они зависят от типа редактируемого объекта
            this.captionStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 71;
            this.cellStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 63;
            this.captionStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 36;
            this.cellStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 45;
        }

        /// <summary>
        /// Обновляет значения общих свойств осей
        /// </summary>
        /// <param name="activeElement"></param>
        /// <param name="tabType"></param>
        private void RefreshAxisCommonProperties(TableReportElement activeElement, SelectedTabType tabType)
        {
            if (activeElement == null)
                return;

            PivotAxis currentAxis = (tabType == SelectedTabType.Row) ? activeElement.PivotData.RowAxis : 
                activeElement.PivotData.ColumnAxis;
            this.axisGrandTotalVisible.Checked = currentAxis.GrandTotalVisible;
            this.axisHideEmptyPositions.Checked = currentAxis.HideEmptyPositions;
            this.axisPropertiesDisplayType.SelectedIndex = (int)currentAxis.PropertiesDisplayType;
            this.axisSortType.SelectedIndex = (int) currentAxis.SortType;
        }

        /// <summary>
        /// Вернет тип выделенной вкладки
        /// </summary>
        /// <returns></returns>
        private SelectedTabType GetSelectedTab()
        {
            SelectedTabType result = SelectedTabType.None;
            if (this.Toolbars.Ribbon.SelectedTab == null)
                return result;

            switch (this.Toolbars.Ribbon.SelectedTab.Key)
            {
                //строки
                case rowTabKey:
                    result = SelectedTabType.Row;
                    break;
                //колонки
                case columnTabKey:
                    result = SelectedTabType.Column;
                    break;
                //меры
                case measureTabKey:
                    result = SelectedTabType.Measure;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Выделит вкладку указанного типа
        /// </summary>
        /// <param name="tabType"></param>
        private void SetSelectedTab(SelectedTabType tabType)
        {
            switch (tabType)
            {
                case SelectedTabType.Column:
                        this.Toolbars.Ribbon.SelectedTab = this.columnTab;
                        break;
                case SelectedTabType.Row:
                        this.Toolbars.Ribbon.SelectedTab = this.rowTab;
                        break;
                case SelectedTabType.Measure:
                        this.Toolbars.Ribbon.SelectedTab = this.measureTab;
                        break;
            }
        }

        /// <summary>
        /// Инициализирует панель инструментов используя данные сохранненые в XML
        /// </summary>
        public void Load(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return;
            try
            {
                ToolbarUtils.LoadStyleGallery(this.captionStyleGallery, xmlNode.SelectSingleNode(ToolbarUtils.cellStylesNodeName));
                ToolbarUtils.LoadStyleGallery(this.cellStyleGallery, xmlNode.SelectSingleNode(ToolbarUtils.cellStylesNodeName));
                ToolbarUtils.LoadStyleGallery(this.totalStyleGallery, xmlNode.SelectSingleNode(ToolbarUtils.cellStylesNodeName));
                this.LoadTabProperties(xmlNode.SelectSingleNode(ToolbarUtils.tabsNodeName));
            }
            catch
            {
                FormException.ShowErrorForm(new Exception("При загрузке данных в панель инструментов таблицы, произошла ошибка"));
            }
        }

        /// <summary>
        /// Загрузить свойства вкладок
        /// </summary>
        private void LoadTabProperties(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return;
            //цвет фона вкладки свойств
            Color propertiesTabBackColor = ToolbarUtils.GetTabColor(xmlNode, ToolbarUtils.propertiesTabNodeName);
            if (propertiesTabBackColor != Color.Empty)
            {
                //Редактор меры
                this.measureEditor.BackColor = propertiesTabBackColor;
            }
        }

        /// <summary>
        /// Должно вызываться после изменения любых свойств элемента отчета
        /// </summary>
        private void AfterEdited()
        {
            this.MainForm.PropertyGrid.Refresh();
            this.MainForm.Saved = false;
        }

        /// <summary>
        /// Создание элемента по текущей таблице
        /// </summary>
        /// <param name="elemType">тип создаваемого элемента</param>
        private void CreateElementByTable(ReportElementType elemType)
        {
            if (this.ActiveTableElement == null)
                return;

            DockableControlPane cp = null;
            
            switch(elemType)
            {
                case ReportElementType.eMap:
                    MapSyncForm msForm = new MapSyncForm();
                    if (msForm.ShowDialog() == DialogResult.OK)
                    {
                        // Добавление панели в DockManager
                        cp = this.MainForm.DockPanelControl.AddDockControlPane(this.ActiveTableElement.PivotData.CubeName, elemType);

                        MapReportElement mapElement = (MapReportElement)cp.Control;
                        mapElement.SelectTemplateName();

                        mapElement.Synchronization.ObjectsInRows = msForm.ObjectsInRows;
                        mapElement.Synchronization.BoundTo = this.ActiveTableElement.UniqueName;
                        mapElement.Synchronize(true);
                        if (!msForm.IsSyncronize)
                        {
                            mapElement.Synchronization.BoundTo = "";
                            this.MainForm.FieldListEditor.InitEditor(mapElement);
                        }

                    }
                    break;
                case ReportElementType.eChart:
                    ChartSyncForm csForm = new ChartSyncForm();
                    if (csForm.ShowDialog() == DialogResult.OK)
                    {
                        // Добавление панели в DockManager
                        cp = this.MainForm.DockPanelControl.AddDockControlPane(this.ActiveTableElement.PivotData.CubeName, elemType);
                        
                        ChartReportElement chartElement = (ChartReportElement)cp.Control;

                        chartElement.Synchronization.MeasureInRows = csForm.MeasureInRows;
                        chartElement.Synchronization.BoundTo = this.ActiveTableElement.UniqueName;
                        chartElement.Synchronize(true);
                        if (!csForm.IsSyncronize)
                        {
                            chartElement.Synchronization.BoundTo = "";
                            this.MainForm.FieldListEditor.InitEditor(chartElement);
                        }

                    }
                    break;
            }
        }


        #region Обработчики событий

        //Клик по вкладке
        void Toolbars_AfterRibbonTabSelected(object sender, RibbonTabEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.DoRefreshTabsTools();
        }

        void Toolbars_AfterRibbonTabDropDown(object sender, RibbonTabEventArgs e)
        {
            if (this.isMayHook)
                return;

            //Обновим динамические свойства
            this.RefreshSelectedGroups(this.ActiveTableElement);
        }

        //Строки
        void autoSizeRows_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveTableElement)
            {
                this.ActiveTableInterface.AutoSizeRows = this.autoSizeRows.Checked;
                this.AfterEdited();
            }
        }

        void calcAverage_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveTableElement)
            {
                AverageSettings aSettings = this.ActiveTableElement.PivotData.AverageSettings;
                AverageSettingsForm aForm = new AverageSettingsForm(aSettings);

                if (aForm.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveTableElement.PivotData.AverageSettings = aForm.AverageSettings;
                    this.ActiveTableElement.PivotData.DoDataChanged();
                }

                this.AfterEdited();
            }
        }

        void calcMedian_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveTableElement)
            {
                MedianSettings mSettings = this.ActiveTableElement.PivotData.MedianSettings;
                MedianSettingsForm mForm = new MedianSettingsForm(mSettings);

                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveTableElement.PivotData.MedianSettings = mForm.MedianSettings;
                    this.ActiveTableElement.PivotData.DoDataChanged();
                }

                this.AfterEdited();
            }
        }

        void calcTopCount_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveTableElement)
            {
                TopCountSettings mSettings = this.ActiveTableElement.PivotData.TopCountSettings;
                TopCountSettingsForm mForm = new TopCountSettingsForm(mSettings);

                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveTableElement.PivotData.TopCountSettings = mForm.TopCountSettings;
                    this.ActiveTableElement.PivotData.DoDataChanged();
                }

                this.AfterEdited();
            }
        }

        void calcBottomCount_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveTableElement)
            {
                BottomCountSettings bcSettings = this.ActiveTableElement.PivotData.BottomCountSettings;
                BottomCountSettingsForm bcForm = new BottomCountSettingsForm(bcSettings);

                if (bcForm.ShowDialog() == DialogResult.OK)
                {
                    this.ActiveTableElement.PivotData.BottomCountSettings = bcForm.BottomCountSettings;
                    this.ActiveTableElement.PivotData.DoDataChanged();
                }

                this.AfterEdited();
            }
        }


        //Общие
        void axisGrandTotalVisible_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveTableElement)
            {
                switch (this.SelectedTab)
                {
                    case SelectedTabType.Row:
                        {
                            this.ActiveTableElement.PivotData.RowAxis.GrandTotalVisible = this.axisGrandTotalVisible.Checked;
                            break;
                        }
                    case SelectedTabType.Column:
                        {
                            this.ActiveTableElement.PivotData.ColumnAxis.GrandTotalVisible = this.axisGrandTotalVisible.Checked;
                            break;
                        }
                }
                this.AfterEdited();
            }
        }

        void axisHideEmptyPositions_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveTableElement)
            {
                switch (this.SelectedTab)
                {
                    case SelectedTabType.Row:
                        {
                            this.ActiveTableElement.PivotData.RowAxis.HideEmptyPositions = this.axisHideEmptyPositions.Checked;
                            break;
                        }
                    case SelectedTabType.Column:
                        {
                            this.ActiveTableElement.PivotData.ColumnAxis.HideEmptyPositions = this.axisHideEmptyPositions.Checked;
                            break;
                        }
                }
                this.AfterEdited();
            }
        }

        void axisPropertiesDisplayType_ToolValueChanged(object sender, ToolEventArgs e)
        {
            if (this.isMayHook)
                return;

            switch (this.SelectedTab)
            {
                case SelectedTabType.Row:
                    {
                        this.EditRowPropertiesDisplayType();
                        break;
                    }
                case SelectedTabType.Column:
                    {
                        this.EditColumnPropertiesDisplayType();
                        break;
                    }
            }
        }

        void axisSortType_ToolValueChanged(object sender, ToolEventArgs e)
        {
            if (this.isMayHook)
                return;

            switch (this.SelectedTab)
            {
                case SelectedTabType.Row:
                    {
                        this.EditRowSortType();
                        break;
                    }
                case SelectedTabType.Column:
                    {
                        this.EditColumnSortType();
                        break;
                    }
            }
        }


        void captionStyleGallery_GalleryToolActiveItemChange(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.ActiveGallery = ActiveGalleryType.CaptionStyle;
            this.GalleryToolActiveItemChange(e);
        }

        void captionStyleGallery_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.GalleryToolItemClick(e);
        }

        void cellStyleGallery_GalleryToolActiveItemChange(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.ActiveGallery = ActiveGalleryType.CellStyle;
            this.GalleryToolActiveItemChange(e);
        }

        void cellStyleGallery_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.GalleryToolItemClick(e);
        }

        void totalStyleGallery_GalleryToolActiveItemChange(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.ActiveGallery = ActiveGalleryType.TotalCellStyle;
            this.GalleryToolActiveItemChange(e);
        }

        void totalStyleGallery_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.GalleryToolItemClick(e);
        }

        void measureEditor_ExitEditMode(object sender, EventArgs e)
        {
            if (this.isMayHook)
                return;

            this.AfterEdited();
        }

        void memberPropertiesEditor_VisibleChanged(object sender, EventArgs e)
        {
            //если редактор свойств элемента не виден, то закрываем контейре отображающий этот редактор
            if (!this.memberPropertiesEditor.Visible)
                this.memberPropertiesContainer.ClosePopup();
        }

        void memberPropertiesContainer_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            //Перед показом свойств элементов, проинициализируем их
            this.InitializeMemberPropertiesEditor(this.ActiveTableElement);
        }

        void addCalcTotal_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if ((this.ActiveTableElement != null) && (this.ActiveTableElement.PivotData != null))
                this.ActiveTableElement.PivotData.TotalAxis.AddCalcTotal();
        }

        void deleteCalcTotal_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if ((this.ActiveTableElement != null) && (this.ActiveTableElement.PivotData != null))
            {
                PivotTotal measure = this.GetNotNullSelectedMeasure(this.ActiveTableElement);
                if ((measure != null) && (measure.IsCustomTotal))
                {
                    this.ActiveTableElement.PivotData.TotalAxis.DeleteCalcTotal(measure.UniqueName);
                    this.ActiveTableElement.PivotData.SelectedObject = null;
                    SetSelectedButtonsVisible(this.ActiveTableElement);
                }
            }
        }

        void measureFormula_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if ((this.ActiveTableElement != null) && (this.ActiveTableElement.PivotData != null))
            {
                PivotTotal measure = this.GetNotNullSelectedMeasure(this.ActiveTableElement);
                if ((measure != null) && (measure.IsCustomTotal))
                    this.ActiveTableElement.PivotData.TotalAxis.EditCalcTotal(measure);
            }
        }

        void colorRules_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.ActiveTableInterface != null)
            {
                ColorRuleCollectionForm crcForm = new ColorRuleCollectionForm(this.ActiveTableInterface.ColorRules);
                crcForm.ShowDialog();
            }
        }

        void createChartByTable_ToolClick(object sender, ToolClickEventArgs e)
        {
            CreateElementByTable(ReportElementType.eChart);
        }

        void createMapByTable_ToolClick(object sender, ToolClickEventArgs e)
        {
            CreateElementByTable(ReportElementType.eMap);
        }



        #endregion 

        #region Вспомогательные методы для установки правильных значений контролам

        /// <summary>
        /// Редактирует тип отображения свойств элементов у строк 
        /// </summary>
        private void EditRowPropertiesDisplayType()
        {
            if (this.IsExistActiveTableElement)
            {
                MemberPropertiesDisplayType newType = (MemberPropertiesDisplayType)this.axisPropertiesDisplayType.SelectedIndex;
                if (newType != this.ActiveTableElement.PivotData.RowAxis.PropertiesDisplayType)
                {
                    this.ActiveTableElement.PivotData.RowAxis.PropertiesDisplayType = newType;
                    this.AfterEdited();
                }
            }
        }

        /// <summary>
        /// Редактирует тип отображения свойств элементов у колонок 
        /// </summary>
        private void EditColumnPropertiesDisplayType()
        {
            if (this.IsExistActiveTableElement)
            {
                MemberPropertiesDisplayType newType = (MemberPropertiesDisplayType)this.axisPropertiesDisplayType.SelectedIndex;
                if (newType != this.ActiveTableElement.PivotData.ColumnAxis.PropertiesDisplayType)
                {
                    this.ActiveTableElement.PivotData.ColumnAxis.PropertiesDisplayType = newType;
                    this.AfterEdited();
                }
            }
        }


        /// <summary>
        /// Редактирует тип сортировки элементов у строк 
        /// </summary>
        private void EditRowSortType()
        {
            if (this.IsExistActiveTableElement)
            {
                SortType newType = (SortType)this.axisSortType.SelectedIndex;
                if (newType != this.ActiveTableElement.PivotData.RowAxis.SortType)
                {
                    this.ActiveTableElement.PivotData.RowAxis.SortType = newType;
                    this.AfterEdited();
                }
            }
        }

        /// <summary>
        /// Редактирует тип сортировки элементов у колонок 
        /// </summary>
        private void EditColumnSortType()
        {
            if (this.IsExistActiveTableElement)
            {
                SortType newType = (SortType)this.axisSortType.SelectedIndex;
                if (newType != this.ActiveTableElement.PivotData.ColumnAxis.SortType)
                {
                    this.ActiveTableElement.PivotData.ColumnAxis.SortType = newType;
                    this.AfterEdited();
                }
            }
        }

        /// <summary>
        /// Получить стиль редактируемого элемента таблицы
        /// </summary>
        /// <returns></returns>
        private CellStyle GetTableEditElementStyle()
        {
            CellStyle result = null;
            if (this.IsExistActiveTableElement)
            {
                switch (this.ActiveGallery)
                {
                    //галерея стилей заголовков
                    case ActiveGalleryType.CaptionStyle:
                        {
                            switch (this.SelectedTab)
                            {
                                case SelectedTabType.Column: 
                                    result = this.ActiveTableInterface.ColumnCaptionsStyle;
                                    break;
                                case SelectedTabType.Comment:
                                    result = this.ActiveTableInterface.CommentStyle;
                                    break;
                                case SelectedTabType.Measure:
                                    result = this.ActiveTableInterface.MeasureCaptionsStyle;
                                    break;
                                case SelectedTabType.Row:
                                    result = this.ActiveTableInterface.RowCaptionsStyle;
                                    break;
                            }
                            break;
                        }
                    //галерея стилей ячеек
                    case ActiveGalleryType.CellStyle:
                        {
                            switch (this.SelectedTab)
                            {
                                case SelectedTabType.Column:
                                    result = this.ActiveTableInterface.ColumnAxisStyle;
                                    break;
                                case SelectedTabType.Comment:
                                    result = this.ActiveTableInterface.CommentStyle;
                                    break;
                                case SelectedTabType.Measure:
                                    result = this.ActiveTableInterface.DataAreaStyle;
                                    break;
                                case SelectedTabType.Row:
                                    result = this.ActiveTableInterface.RowAxisStyle;
                                    break;
                            }
                            break;
                        }
                    //галерея стилей ячеек итогов
                    case ActiveGalleryType.TotalCellStyle:
                        {
                            switch (this.SelectedTab)
                            {
                                case SelectedTabType.Measure:
                                    result = this.ActiveTableInterface.DataTotalsAreaStyle;
                                    break;
                            }
                            break;
                        }
                }
            }
            return result;
        }

        /// <summary>
        /// Установить стиль редактируемому элементу таблицы
        /// </summary>
        /// <param name="style"></param>
        private void SetTableEditElementStyle(CellStyle style)
        {
            if (style == null)
                return;
            if (this.IsExistActiveTableElement)
            {
                switch (this.ActiveGallery)
                {
                    //галерея стилей заголовков
                    case ActiveGalleryType.CaptionStyle:
                        {
                            switch (this.SelectedTab)
                            {
                                case SelectedTabType.Column:
                                    this.ActiveTableInterface.ColumnCaptionsStyle = style;
                                    break;
                                case SelectedTabType.Comment:
                                    this.ActiveTableInterface.CommentStyle = style;
                                    break;
                                case SelectedTabType.Measure:
                                    this.ActiveTableInterface.MeasureCaptionsStyle = style;
                                    break;
                                case SelectedTabType.Row:
                                    this.ActiveTableInterface.RowCaptionsStyle = style;
                                    break;
                            }
                            break;
                        }
                    //галерея стилей ячеек
                    case ActiveGalleryType.CellStyle:
                        {
                            switch (this.SelectedTab)
                            {
                                case SelectedTabType.Column:
                                    this.ActiveTableInterface.ColumnAxisStyle = style;
                                    break;
                                case SelectedTabType.Comment:
                                    this.ActiveTableInterface.CommentStyle = style;
                                    break;
                                case SelectedTabType.Measure:
                                    this.ActiveTableInterface.DataAreaStyle = style;
                                    break;
                                case SelectedTabType.Row:
                                    this.ActiveTableInterface.RowAxisStyle = style;
                                    break;
                            }
                            break;
                        }
                    //галерея стилей ячеек итогов
                    case ActiveGalleryType.TotalCellStyle:
                        {
                            switch (this.SelectedTab)
                            {
                                case SelectedTabType.Measure:
                                    this.ActiveTableInterface.DataTotalsAreaStyle = style;
                                    break;
                            }
                            break;
                        }
                }
            }
        }

        private void GalleryToolActiveItemChange(GalleryToolItemEventArgs e)
        {
            if (e.Item != null)
            {
                //если активация элемента происходи впервые, запомним стиль 
                if (this.befoEditStyle == null)
                    this.befoEditStyle = this.GetTableEditElementStyle();
                //установим новый стиль
                this.SetTableEditElementStyle((CellStyle)e.Item.Tag);
            }
            else
            {
                //сюда попадаем при деактивации галереии стилей, если ничего не выбрали,
                //значит в befoEditStyle должен сохраниться стиль запомненый перед
                //началом выбора его и выставляем, если выбор все даки был сделан
                //(кликали по элементам) то ничего не происходит, т.к. это значение очищалось
                this.SetTableEditElementStyle(this.befoEditStyle);
                this.befoEditStyle = null;
            }
        }

        private void GalleryToolItemClick(GalleryToolItemEventArgs e)
        {
            this.befoEditStyle = null;
            this.AfterEdited();
        }

        private void InitializeMemberPropertiesEditor(TableReportElement reportElement)
        {
            if (reportElement == null)
                return;

            //Получаем выделенный уровень
            PivotField level = this.GetNotNullSelectedLevel(reportElement);

            if (level != null)
            {
                this.memberPropertiesEditor.MemberProperties = level.MemberProperties;
                this.memberPropertiesContainer.SharedProps.Enabled = 
                    level.MemberProperties.AllProperties.Count > 0;
            }
        }

        /// <summary>
        /// Получаем выделенную меру, если е нет, то пытаемся получить lastNotNullSelectedMeasure
        /// </summary>
        /// <param name="reportElement"></param>
        /// <returns></returns>
        private PivotTotal GetNotNullSelectedMeasure(TableReportElement reportElement)
        {
            PivotTotal result = null;
            if (reportElement == null)
                return result;

            //Если выделен уровень, значит делать здесь нечего, т.к. может быть выделен только один объект
            if (reportElement.PivotData.SelectedField != null)
            {
                this.lastNotNullSelectedMeasure = string.Empty;
                return result;
            }

            //Получаем выделенную меру
            result = reportElement.PivotData.SelectedMeasure;

            //Если ее нет, пытаемся получить lastNotNullSelectedMeasure
            if (result == null)
            {
                result = reportElement.PivotData.TotalAxis.GetPivotTotal(this.lastNotNullSelectedMeasure);
            }

            if (result != null)
            {
                //сохраним последние не нулевую меру
                this.lastNotNullSelectedMeasure = result.UniqueName;
            }

            return result;
        }

        /// <summary>
        /// Получаем выделенный уровень, если его нет, то пытаемся получить lastNotNullSelectedLevel
        /// </summary>
        /// <param name="reportElement"></param>
        /// <returns></returns>
        private PivotField GetNotNullSelectedLevel(TableReportElement reportElement)
        {
            PivotField result = null;
            if (reportElement == null)
                return result;

            //Если выделена мера, значит делать здесь нечего, т.к. может быть выделен только один объект
            if (reportElement.PivotData.SelectedMeasure != null)
            {
                this.lastNotNullSelectedLevel = string.Empty;
                return result;
            }

            //Получаем выделенный уровень
            result = reportElement.PivotData.SelectedField;

            //Если его нет, пытаемся получить lastNotNullSelectedLevel
            if (result == null)
            {
                result = reportElement.PivotData.GetPivotField(this.lastNotNullSelectedLevel);
            }

            if (result != null)
            {
                //сохраним последние не нулевой уровень
                this.lastNotNullSelectedLevel = result.UniqueName;
            }

            return result;
        }

        #endregion

        #region Свойства
        public ToolbarsManage ToolbarsManager
        {
            get { return _toolbarsManager; }
            set { _toolbarsManager = value; }
        }

        /// <summary>
        /// Ссылка на главную форму
        /// </summary>
        public MainForm MainForm
        {
            get { return this.ToolbarsManager.MainForm; }
            set { this.ToolbarsManager.MainForm = value; }
        }

        /// <summary>
        /// Лента
        /// </summary>
        public UltraToolbarsManager Toolbars
        {
            get { return this.ToolbarsManager.Toolbars; }
            set { this.ToolbarsManager.Toolbars = value; }
        }

        /// <summary>
        /// Интерфейс активной таблицы
        /// </summary>
        public IGridUserInterface ActiveTableInterface
        {
            get
            {
                return this.IsExistActiveTableElement ? this.ActiveTableElement.GridUserInterface : null;
            }
        }

        /// <summary>
        /// Существует ли активный элемент
        /// </summary>
        private bool IsExistActiveTableElement
        {
            get { return this.ActiveTableElement != null; }
        }

        /// <summary>
        /// Активный элемент с таблицей, по которому инициализирован тулбар
        /// </summary>
        public TableReportElement ActiveTableElement
        {
            get { return _activeTableElement; }
            set { _activeTableElement = value; }
        }

        /// <summary>
        /// Выделенная вкладка таблицы
        /// </summary>
        private SelectedTabType SelectedTab
        {
            get { return this.GetSelectedTab(); }
            set { this.SetSelectedTab(value); }
        }

        /// <summary>
        /// Тип активной галереи
        /// </summary>
        private ActiveGalleryType ActiveGallery
        {
            get
            {
                return _activeGallery;
            }
            set
            {
                this._activeGallery = value;
            }
        }
        #endregion

        #region Дополнительные классы

        /// <summary>
        /// Тип владки таблицы
        /// </summary>
        enum SelectedTabType
        {
            Row,
            Column,
            Comment,
            Measure,
            None
        }

        /// <summary>
        /// Тип активной галерии
        /// </summary>
        enum ActiveGalleryType
        {
            //стилей заголовка
            CaptionStyle,
            //стилей ячейки
            CellStyle,
            //стилей ячейки итога
            TotalCellStyle,
            //пусто
            None
        }
        #endregion
    }
}
