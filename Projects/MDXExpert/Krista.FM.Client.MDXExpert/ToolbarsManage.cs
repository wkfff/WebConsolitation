using System;
using System.Xml;
using System.Collections.Generic;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Client.MDXExpert.CommonClass;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Класс создан для управлением Лентой
    /// </summary>
    public class ToolbarsManage
    {
        private bool _isMayHook = false;
        //ссылка на главную форму
        private MainForm _mainForm;
        private UltraToolbarsManager _toolbars;

        private ChartToolBar _chartToolbar;
        private TableToolbar _tableToolbar;
        private MapToolbar _mapToolbar;
        private ReportElementToolbar _elementToolbar;

        #region Константы панели инструментов
        /// <summary>
        /// индекс изображения, первой диаграммы в ilLarge
        /// </summary>
        public const int largeImageIndexFirstChart = 0;
        
        //Имена узлов в XML
        /// <summary>
        /// Корень свойства панели инструментов
        /// </summary>
        public const string toolbarNodeName = "MDXToolbar";
        /// <summary>
        /// Узел со свойствами панели инструментов таблицы
        /// </summary>
        public const string tableToolbarNodeName = "table";
        /// <summary>
        /// Узел со свойствами панели инструментов диаграммы
        /// </summary>
        public const string chartToolbarNodeName = "chart";
        /// <summary>
        /// Узел со свойствами панели инструментов карты
        /// </summary>
        public const string mapToolbarNodeName = "map";
        /// <summary>
        /// Узел со свойствами панели инструментов элемента отчета
        /// </summary>
        public const string elementToolbarNodeName = "reportElement";
        #endregion

        #region Ключи
        //вкадка отчета
        private const string reportTabKey = "ReportTab";
        //вкладка элемента
        private const string elementTabKey = "ElementTab";

        //групп
        //создание элементов по таблице
        public const string elementsByTableGroupKey = "ElementsByTableGroup";


        //кнопок
        private const string addStandardChartKey = "AddStandardChart";
        private const string addCompositeChartKey = "AddCompositeChart";
        private const string addCustomChartKey = "AddCustomChart";
        private const string addChartKey = "AddChart";
        private const string addStandartTableKey = "AddStandartTable";
        private const string addCustomTableKey = "AddCustomTable";
        private const string addTableKey = "AddTable";
        private const string addStandartMapKey = "AddStandartMap";
        private const string addCustomMapKey = "AddCustomMap";
        private const string addMapKey = "AddMap";
        private const string addGaugeKey = "AddGauge";
        private const string addStandartGaugeKey = "AddStandartGauge";
        private const string addMultiGaugeKey = "AddMultiGauge";
        private const string saveReportKey = "SaveReport";
        private const string saveReportAsKey = "SaveReportAs";
        private const string openReportKey = "OpenReport";
        private const string newReportKey = "NewReport";
        private const string colorSchemeListKey = "ColorSchemeList";
        private const string recentListKey = "RecentList";
        private const string structureToolKey = "StructureTool";
        private const string propertyToolKey = "PropertyTool";
        private const string exportToExcelKey = "ExportToExcel";
        private const string applicationAboutKey = "About";
        public const string removeElementKey = "RemoveElement";
        private const string cloneElementKey = "CloneElement";
        private const string unDoKey = "UnDo";
        private const string reDoKey = "ReDo";
        private const string connectionKey = "Connection";
        private const string connectionRefreshKey = "ConnectionRefresh";
        private const string isHideTotalsByDefaultKey = "IsHideTotalsByDefault";


        #endregion

        #region контролы главного меню
        /// <summary>
        /// вкладка отчета
        /// </summary>
        private RibbonTab reportTab;
        /// <summary>
        /// Вкладка со свойствами элемента
        /// </summary>
        private RibbonTab elementTab;
        
        //группы
        /// <summary>
        /// Создание элементов по таблице
        /// </summary>
        private RibbonGroup elementsByTableGroup;


        //кнопоки
        /// <summary>
        /// Добавить стандартную диаграмму
        /// </summary>
        private ButtonTool addStandardChart;
        /// <summary>
        /// Добавить композитную диаграмму
        /// </summary>
        private ButtonTool addCompositeChart;
        /// <summary>
        /// Добавить пользовательскую диаграмму
        /// </summary>
        private ButtonTool addCustomChart;
        /// <summary>
        /// Добавить диаграмму
        /// </summary>
        private PopupGalleryTool addChart;
        /// <summary>
        /// Добавить стандартную таблицу
        /// </summary>
        private ButtonTool addStandardTable;
        /// <summary>
        /// Добавить пользовательскую таблицу
        /// </summary>
        private ButtonTool addCustomTable;
        /// <summary>
        /// Добавить таблицу
        /// </summary>
        private PopupGalleryTool addTable;
        /// <summary>
        /// Добавить карту
        /// </summary>
        private PopupGalleryTool addMap;
        /// <summary>
        /// Добавить стандартную карту
        /// </summary>
        private ButtonTool addStandardMap;
        /// <summary>
        /// Добавить пользовательскую карту
        /// </summary>
        private ButtonTool addCustomMap;
        /// <summary>
        /// Добавить индикатор
        /// </summary>
        private PopupGalleryTool addGauge;
        /// <summary>
        /// Добавить стандартный индикатор
        /// </summary>
        private ButtonTool addStandardGauge;
        /// <summary>
        /// Добавить множественный индикатор
        /// </summary>
        private ButtonTool addMultiGauge;
        /// <summary>
        /// Удалить текущий элемент
        /// </summary>
        private ButtonTool removeElement;
        /// <summary>
        /// Сохранить отчет
        /// </summary>
        private ButtonTool saveReport;
        /// <summary>
        /// Сохранить очте как
        /// </summary>
        private ButtonTool saveReportAs;
        /// <summary>
        /// Открыть отчет
        /// </summary>
        private ButtonTool openReport;
        /// <summary>
        /// закрыть отчет
        /// </summary>
        private ButtonTool newReport;       
        /// <summary>
        /// Показать/Cкрыть панель структуры
        /// </summary>
        public StateButtonTool structureTool;
        /// <summary>
        /// Показать/Cкрыть панель свойств
        /// </summary>
        public StateButtonTool propertyTool;
        /// <summary>
        /// Список цветовых схем
        /// </summary>
        private ListTool colorSchemeList;
        /// <summary>
        /// Список последних использованных файлов
        /// </summary>
        private ListTool recentList;
        /// <summary>
        /// Экспорт в Excel
        /// </summary>
        private ButtonTool exportToExcel;
        /// <summary>
        /// О программе
        /// </summary>
        private ButtonTool applicationAbout;
        /// <summary>
        /// Копировать элемент
        /// </summary>
        private ButtonTool cloneElement;
        /// <summary>
        /// Подключение
        /// </summary>
        private ButtonTool connection;
        /// <summary>
        /// Обновить подключение
        /// </summary>
        private ButtonTool connectionRefresh;
        /// <summary>
        /// Отменить действие
        /// </summary>
        private ButtonTool undo;
        /// <summary>
        /// Вернуть действие
        /// </summary>
        private ButtonTool redo;
        /// <summary>
        /// Показать итоги по умолчанию
        /// </summary>
        public StateButtonTool isHideTotalsByDefault;

        #endregion

        //заголовок выделенной вкладки
        private string selectedTabKey = string.Empty;

        //ключи контекстных вкладок для таблицы
        private readonly string[] tableContextualTabKeys = new String[] { "contextualTableProperties" };
        //ключи контекстных вкладок для диаграммы
        private readonly string[] chartContextualTabKeys = new String[] { "contextualChartProperties" };
        //ключи контекстных вкладок для карты
        private readonly string[] mapContextualTabKeys = new String[] { "contextualMapProperties" };

        public ToolbarsManage(MainForm mainForm, UltraToolbarsManager toolbarsManager)
        {
            this.MainForm = mainForm;
            this.Toolbars = toolbarsManager;
            this.TableToolBar = new TableToolbar(this);
            this.ChartToolBar = new ChartToolBar(this);
            this.MapToolBar = new MapToolbar(this);

            this.ElementToolbar = new ReportElementToolbar(this);
        }

        /// <summary>
        /// Инициалазация
        /// </summary>
        public void Initialize()
        {
            //Устанавливаем ссылки на контролы главного меню
            this.SetLinkOnTools();
            //Устанавливаем заголовкам вкладок русские имена
            this.SetRusTabCaption();
            //Устанавливаем контролам главного меню, требуемые обработчики
            this.SetEventHandlers();

            this.TableToolBar.Initialize();
            this.ChartToolBar.Initialize();
            this.MapToolBar.Initialize();
            this.ElementToolbar.Initialize();
            //Сделаем все контекстные вкладки невидимыми
            this.RefreshContextualTabs(null);
            this.SetStateControl(null);
        }

        /// <summary>
        /// Некоторые настройки панели инструментов храняться в XML, загрузим их...
        /// </summary>
        /// <param name="docPath">путь к документу, содержащего настройки</param>
        public void Load(string docPath)
        {
            XmlDocument dom = new XmlDocument();
            try
            {
                dom.Load(docPath);
                XmlNode toolbarNode = dom.SelectSingleNode("styleLibrary//" + toolbarNodeName);
                if (toolbarNode != null)
                {
                    this.TableToolBar.Load(toolbarNode.SelectSingleNode(tableToolbarNodeName));
                    this.ChartToolBar.Load(toolbarNode.SelectSingleNode(chartToolbarNodeName));
                    this.MapToolBar.Load(toolbarNode.SelectSingleNode(mapToolbarNodeName));
                    this.ElementToolbar.Load(toolbarNode.SelectSingleNode(elementToolbarNodeName));
                }
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref dom);
            }
        }

        /// <summary>
        /// Устанавливаем ссылки на контролы таблицы
        /// </summary>
        private void SetLinkOnTools()
        {
            //Вкладки
            //главная
            this.reportTab = this.Toolbars.Ribbon.Tabs[reportTabKey];
            //элемент
            this.elementTab = this.Toolbars.Ribbon.Tabs[elementTabKey];

            //группы
            this.elementsByTableGroup = this.elementTab.Groups[elementsByTableGroupKey];

            //Контролы
            this.addStandardChart = (ButtonTool)this.Toolbars.Tools[addStandardChartKey];
            this.addCompositeChart = (ButtonTool)this.Toolbars.Tools[addCompositeChartKey];
            this.addCustomChart = (ButtonTool)this.Toolbars.Tools[addCustomChartKey];
            this.addChart = (PopupGalleryTool)this.Toolbars.Tools[addChartKey];
            this.addStandardTable = (ButtonTool)this.Toolbars.Tools[addStandartTableKey];
            this.addCustomTable = (ButtonTool)this.Toolbars.Tools[addCustomTableKey];
            this.addTable = (PopupGalleryTool)this.Toolbars.Tools[addTableKey];
            this.addStandardMap = (ButtonTool)this.Toolbars.Tools[addStandartMapKey];
            this.addCustomMap = (ButtonTool)this.Toolbars.Tools[addCustomMapKey];
            this.addMap = (PopupGalleryTool)this.Toolbars.Tools[addMapKey];
            this.addGauge = (PopupGalleryTool)this.Toolbars.Tools[addGaugeKey];
            this.addStandardGauge = (ButtonTool)this.Toolbars.Tools[addStandartGaugeKey];
            this.addMultiGauge = (ButtonTool)this.Toolbars.Tools[addMultiGaugeKey];
            this.removeElement = (ButtonTool)this.Toolbars.Tools[removeElementKey];
            this.saveReport = (ButtonTool)this.Toolbars.Tools[saveReportKey];
            this.saveReportAs = (ButtonTool)this.Toolbars.Tools[saveReportAsKey];
            this.openReport = (ButtonTool)this.Toolbars.Tools[openReportKey];
            this.newReport = (ButtonTool)this.Toolbars.Tools[newReportKey];
            this.colorSchemeList = (ListTool)this.Toolbars.Tools[colorSchemeListKey];
            this.recentList = (ListTool)this.Toolbars.Tools[recentListKey];
            this.structureTool = (StateButtonTool)this.Toolbars.Tools[structureToolKey];
            this.propertyTool = (StateButtonTool)this.Toolbars.Tools[propertyToolKey];
            this.exportToExcel = (ButtonTool)this.Toolbars.Tools[exportToExcelKey];
            this.applicationAbout = (ButtonTool)this.Toolbars.Tools[applicationAboutKey];
            this.cloneElement = (ButtonTool)this.Toolbars.Tools[cloneElementKey];
            this.connection = (ButtonTool)this.Toolbars.Tools[connectionKey];
            this.connectionRefresh = (ButtonTool)this.Toolbars.Tools[connectionRefreshKey];
            this.undo = (ButtonTool)this.Toolbars.Tools[unDoKey];
            this.redo = (ButtonTool)this.Toolbars.Tools[reDoKey];
            this.isHideTotalsByDefault = (StateButtonTool)this.Toolbars.Tools[isHideTotalsByDefaultKey];

        }

        /// <summary>
        /// Установить обработчики событий
        /// </summary>
        private void SetEventHandlers()
        {
            //Контролы
            this.addStandardChart.ToolClick += new ToolClickEventHandler(addStandardChart_ToolClick);
            this.addCompositeChart.ToolClick += new ToolClickEventHandler(addCompositeChart_ToolClick);
            this.addCustomChart.ToolClick += new ToolClickEventHandler(addCustomChart_ToolClick);
            this.addChart.ToolClick += new ToolClickEventHandler(addStandardChart_ToolClick);
            this.addStandardTable.ToolClick += new ToolClickEventHandler(addStandardTable_ToolClick);
            this.addCustomTable.ToolClick += new ToolClickEventHandler(addCustomTable_ToolClick);
            this.addTable.ToolClick += new ToolClickEventHandler(addStandardTable_ToolClick);
            this.addStandardMap.ToolClick += new ToolClickEventHandler(addStandardMap_ToolClick);
            this.addCustomMap.ToolClick += new ToolClickEventHandler(addCustomMap_ToolClick);
            this.addMap.ToolClick += new ToolClickEventHandler(addStandardMap_ToolClick);
            this.addGauge.ToolClick += new ToolClickEventHandler(addStandardGauge_ToolClick);
            this.addStandardGauge.ToolClick += new ToolClickEventHandler(addStandardGauge_ToolClick);
            this.addMultiGauge.ToolClick += new ToolClickEventHandler(addMultiGauge_ToolClick);
            this.removeElement.ToolClick += new ToolClickEventHandler(removeElement_ToolClick);
            this.saveReport.ToolClick += new ToolClickEventHandler(saveReport_ToolClick);
            this.saveReportAs.ToolClick += new ToolClickEventHandler(saveReportAs_ToolClick);
            this.openReport.ToolClick += new ToolClickEventHandler(openReport_ToolClick);
            this.newReport.ToolClick += new ToolClickEventHandler(newReport_ToolClick);
            this.colorSchemeList.ToolClick += new ToolClickEventHandler(colorSchemeList_ToolClick);
            this.recentList.ToolClick += new ToolClickEventHandler(recentList_ToolClick);
            this.structureTool.ToolClick += new ToolClickEventHandler(structureTool_ToolClick);
            this.propertyTool.ToolClick += new ToolClickEventHandler(propertyTool_ToolClick);
            this.exportToExcel.ToolClick += new ToolClickEventHandler(exportToExcel_ToolClick);
            this.applicationAbout.ToolClick += new ToolClickEventHandler(applicationAbout_ToolClick);
            this.cloneElement.ToolClick += new ToolClickEventHandler(cloneElement_ToolClick);
            this.connection.ToolClick += new ToolClickEventHandler(connection_ToolClick);
            this.connectionRefresh.ToolClick += new ToolClickEventHandler(connectionRefresh_ToolClick);
            this.undo.ToolClick += new ToolClickEventHandler(unDo_ToolClick);
            this.redo.ToolClick += new ToolClickEventHandler(reDo_ToolClick);
            this.isHideTotalsByDefault.ToolClick += new ToolClickEventHandler(isHideTotalsByDefault_ToolClick);

            //Обработчики для своевременого рефреша ленты
            this.MainForm.UndoRedoManager.NewEvent += new EventHandler(UndoRedoManager_NewEvent);
            this.MainForm.UndoRedoManager.UndoChanged += new EventHandler(UndoRedoManager_UndoChanged);
            this.MainForm.UndoRedoManager.RedoChanged += new EventHandler(UndoRedoManager_RedoChanged);
            this.MainForm.UndoRedoManager.ClearedEvents += new EventHandler(UndoRedoManager_ClearedEvents);
        }

        /// <summary>
        /// Устанавливаем заголовкам вкладок русские имена, непонятно почему, но они очень 
        /// часто меняются на ключи
        /// </summary>
        private void SetRusTabCaption()
        {
            this.reportTab.Caption = "Отчет";
        }


        #region Обработчики событий

        void addStandardChart_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddChart();
        }

        void addCompositeChart_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddChart(ReportElementSubType.Composite);
        }

        void addCustomChart_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddChart(ReportElementSubType.CustomMDX);
        }

        void addStandardTable_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddTable();
        }

        void addCustomTable_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddTable(ReportElementSubType.CustomMDX);
        }

        void addStandardMap_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddMap(ReportElementSubType.Standart);
        }

        void addStandardGauge_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddGauge(ReportElementSubType.CustomData);
        }

        void addMultiGauge_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddMultiGauge(ReportElementSubType.Standart);
        }


        void addCustomMap_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.AddMap(ReportElementSubType.CustomData);
        }

        void removeElement_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.RemoveActiveElement();
            this.ElementToolbar.InitReportElementList();
        }

        void cloneElement_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.CloneReportElement(this.MainForm.ActiveElement);
            this.ElementToolbar.InitReportElementList();

        }

        void connection_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.SelectConnection();
        }

        void connectionRefresh_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.RefreshConnection();
        }

        void saveReport_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.SaveReport(false, this.MainForm.ReportFileName);
        }

        void saveReportAs_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.SaveReport(true, this.MainForm.ReportFileName);
        }

        void openReport_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.MainForm.OpenReport("");
        }

        void newReport_ToolClick(object sender, ToolClickEventArgs e)
        {
            //Все что требуется для создания нового отчета, это закрыть старый
            this.MainForm.CloseReport();
        }

        void colorSchemeList_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            ListToolItem item = ((ListTool)e.Tool).SelectedItem;
            if (item != null)
            {
                this.MainForm.StyleSheet = item.Key;
            }
        }

        void recentList_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            ListToolItem item = ((ListTool)e.Tool).SelectedItem;
            if (item != null)
            {
                this.MainForm.OpenMRUReport(item.Key);
            }
        }

        void structureTool_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            this.MainForm.StructureToolVisible = !this.structureTool.Checked;            
        }

        void isHideTotalsByDefault_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            MainForm.IsHideTotalsByDefault = this.isHideTotalsByDefault.Checked;
        }

        void propertyTool_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            this.MainForm.PropertyToolVisible = !this.propertyTool.Checked;
        }

        void exportToExcel_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            this.MainForm.ExportForm.ShowDialog(false);
        }

        void applicationAbout_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            this.MainForm.AboutForm.Show(this.MainForm);
            this.MainForm.AboutForm.Visible = true;
        }

        void unDo_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            this.MainForm.UndoRedoManager.Undo();
        }

        void reDo_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.IsMayHook)
                return;

            this.MainForm.UndoRedoManager.Redo();
        }

        //Обработчики для своевременого обновления ленты
        void UndoRedoManager_NewEvent(object sender, EventArgs e)
        {
            this.RefreshUndoRedoButton();
        }

        void UndoRedoManager_UndoChanged(object sender, EventArgs e)
        {
            this.RefreshUndoRedoButton();
        }

        void UndoRedoManager_RedoChanged(object sender, EventArgs e)
        {
            this.RefreshUndoRedoButton();
        }

        void UndoRedoManager_ClearedEvents(object sender, EventArgs e)
        {
            this.RefreshUndoRedoButton();
        }

        #endregion

        /// <summary>
        /// Делает видимыми закладки, исходя из типа элемента отчета
        /// </summary>
        /// <param name="elementType"></param>
        private void SetVisibilityContextualTabs(ReportElementType reportElementType)
        {
            List<string> visibleKeyList = new List<string>();
            switch (reportElementType)
            {
                case ReportElementType.eTable:
                    {
                        visibleKeyList.InsertRange(0, tableContextualTabKeys);
                        break;
                    }
                case ReportElementType.eChart:
                    {
                        visibleKeyList.InsertRange(0, chartContextualTabKeys);
                        break;
                    }
                case ReportElementType.eMap:
                    {
                        visibleKeyList.InsertRange(0, mapContextualTabKeys);
                        break;
                    }
            }
            
            foreach (ContextualTabGroup contextualTab in this.Toolbars.Ribbon.ContextualTabGroups)
            {
                contextualTab.Visible = visibleKeyList.Contains(contextualTab.Key);
            }
        }


        private void SetVisibilityGroup(ReportElementType elemType)
        {
            this.elementsByTableGroup.Visible = false;
            switch(elemType)
            {
                case ReportElementType.eTable:
                    this.elementsByTableGroup.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Отображаем и обновляем вид контекстно зависимых вкладок и значения контролов на них
        /// </summary>
        /// <param name="activeElement"></param>
        public void RefreshTabs(CustomReportElement activeElement)
        {
            //запоминаем имя заголовка вкладки, после инициализации выставим вкладку, 
            //с таким же заголовком
            this._isMayHook = true;
            try
            {
                this.selectedTabKey = (this.Toolbars.Ribbon.SelectedTab != null) ?
                    this.Toolbars.Ribbon.SelectedTab.Key : string.Empty;
                    
                this.RefreshContextualTabs(activeElement);

                this.SetActiveTab(selectedTabKey);
                this.SetStateControl(activeElement);
            }
            finally
            {
                this._isMayHook = false;
            }
        }

        /// <summary>
        /// Т.к. состояние этих кнопок обновляются чаще других, выне в отдельную процедуру.
        /// </summary>
        public void RefreshUndoRedoButton()
        {
            this.undo.SharedProps.Enabled = this.MainForm.UndoRedoManager.IsMakeUndo;
            this.redo.SharedProps.Enabled = this.MainForm.UndoRedoManager.IsMakeRedo;
        }

        /// <summary>
        /// Устанавливет включенность контролов
        /// </summary>
        /// <param name="activeElement"></param>
        private void SetStateControl(CustomReportElement activeElement)
        {
            this.saveReport.SharedProps.Enabled = (activeElement != null);
            this.saveReportAs.SharedProps.Enabled = (activeElement != null);
            this.removeElement.SharedProps.Enabled = (activeElement != null);
            //запрещаем клонирование композитной диаграммы
            bool isCompositeChart = ((activeElement is ChartReportElement) &&
                                     (((ChartReportElement) activeElement).IsCompositeChart));
            this.cloneElement.SharedProps.Enabled = ((activeElement != null)&&(!isCompositeChart));
            this.exportToExcel.SharedProps.Enabled = (activeElement != null);
            this.RefreshUndoRedoButton();

            this.IsMayHook = true;
            this.isHideTotalsByDefault.Checked = MainForm.IsHideTotalsByDefault;
            this.IsMayHook = false;
        }

        /// <summary>
        /// По имени заголовка вкладки, делает ее выделенной
        /// </summary>
        /// <param name="tabKey"></param>
        private void SetActiveTab(string tabKey)
        {
            if (tabKey == string.Empty)
                return;

            foreach (RibbonTab ribbonTab in this.Toolbars.Ribbon.Tabs)
            {
                if ((ribbonTab.Visible) && (ribbonTab.Key == tabKey))
                {
                    this.Toolbars.Ribbon.SelectedTab = ribbonTab;
                    return;
                }
            }
        }

        /// <summary>
        /// Отображаем и обновляем в зависимости от типа активного элемента, контекстно зависимые вкладки
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshContextualTabs(CustomReportElement activeElement)
        {
            if (activeElement == null)
            {
                //Если активного элемента нет, то и закладок НЕТ
                foreach (ContextualTabGroup contextualTab in this.Toolbars.Ribbon.ContextualTabGroups)
                {
                    contextualTab.Visible = false;
                }
                this.elementTab.Visible = false;
            }
            else
            {
                //вкладка элемента отчета видна всегда, вне зависимости от типа елемента
                this.elementTab.Visible = true;
                //установим видимость контекстных вкладок
                this.SetVisibilityContextualTabs(activeElement.ElementType);
                //установим видимость групп
                this.SetVisibilityGroup(activeElement.ElementType);
                //инициализируем инструменты распологающиеся на вкладках
                this.RefreshContextualTabsToos(activeElement);
            }
        }

        /// <summary>
        /// Обновляет значение контролов на контекстных закладках
        /// </summary>
        /// <param name="reportElementType"></param>
        private void RefreshContextualTabsToos(CustomReportElement activeElement)
        {
            this.ElementToolbar.RefreshTabsTools(activeElement);
            switch (activeElement.ElementType)
            {
                case ReportElementType.eTable:
                    {
                        this.TableToolBar.RefreshTabsTools((TableReportElement)activeElement);
                        break;
                    }
                case ReportElementType.eChart:
                    {
                        this.ChartToolBar.RefreshTabsTools((ChartReportElement)activeElement);
                        break;
                    }
                case ReportElementType.eMap:
                    {
                        this.MapToolBar.RefreshTabsTools((MapReportElement)activeElement);
                        break;
                    }
            }
        }

        /// <summary>
        /// Панель инструментов диаграмы
        /// </summary>
        public ChartToolBar ChartToolBar
        {
            get { return _chartToolbar; }
            set { _chartToolbar = value; }
        }

        /// <summary>
        /// Панель инструментов таблицы
        /// </summary>
        public TableToolbar TableToolBar
        {
            get { return _tableToolbar; }
            set { _tableToolbar = value; }
        }

        /// <summary>
        /// Панель инструментов карты
        /// </summary>
        public MapToolbar MapToolBar
        {
            get { return _mapToolbar; }
            set { _mapToolbar = value; }
        }


        /// <summary>
        /// Панель инструментов элемента отчета
        /// </summary>
        internal ReportElementToolbar ElementToolbar
        {
            get { return _elementToolbar; }
            set { _elementToolbar = value; }
        }

        /// <summary>
        /// Ссылка на главную форму
        /// </summary>
        public MainForm MainForm
        {
            get { return _mainForm; }
            set { _mainForm = value; }
        }

        /// <summary>
        /// Лента
        /// </summary>
        public UltraToolbarsManager Toolbars
        {
            get { return _toolbars; }
            set { _toolbars = value; }
        }

        /// <summary>
        /// Заголовок ленты
        /// </summary>
        public string RibbonCaption
        {
            get { return this.Toolbars.Ribbon.Caption; }
            set { this.Toolbars.Ribbon.Caption = value; }
        }

        /// <summary>
        /// Если true, обработчики событий выполняться не должны
        /// </summary>
        public bool IsMayHook
        {
            get { return _isMayHook; }
            set { _isMayHook = value; }
        }
    }
}
