using System;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Resources.Appearance;
using System.Xml;
using System.Drawing;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Ключи элементов в панэли инструментов, относящихся к диаграмме
    /// </summary>
    public class ChartToolBar
    {
        #region Ключи
        //вкладок
        //вкладки свойств диаграммы
        public const string propertiesTabKey = "ChartPropertiesTab";
        //вкладки рядов диаграммы
        public const string chartRowTabKey = "ChartRowTab";
        //вкладки категорий диаграммы
        public const string chartColumnTabKey = "ChartColumnTab";

        //групп
        //группа с галереей диаграмм
        public const string typesGalleryGroupKey = "ChartTypesGroup";
        //свойства 3D диаграммы
        public const string chart3DTransformGroupKey = "3DChartGroup";
        //свойства структурных диаграмм
        public const string structuralChartGroupKey = "StructuralChartGroup";
        //свойства осей диаграммы
        public const string chartAxisGroupKey = "ChartAxisGroup";
        //общие свойства диаграммы
        public const string chartCommonGroupKey = "ChartCommonGroup";
        //управление данными
        public const string dataManageGroupKey = "DataManageGroup";

        //инструментов
        //галерея диаграмм
        public const string typesGalleryKey = "ChartTypesGallery";
        //контейнер для контрола, редактирующего 3D вид диаграммы
        public const string chart3DTransformContainerKey = "3DTransform";
        //контейнер для контрола, редактирующего легенду
        public const string chartLegendEditorContainerKey = "ChartLegendEditorContainer";
        //контейнер для контрола, редактируещего свойства структурных диаграмм
        public const string structuralChartEditorContainerKey = "StructuralChartEditorContainer";
        //редактор осей
        public const string chartAxisEditorContainerKey = "ChartAxisEditorContainer";
        //подписи данных
        public const string chartTextKey = "ChartText";
        //Оси диаграммы
        public const string axesSettingsKey = "AxesSettings";
        //первая коллекция подписей данных
        public const string chartTextCollection1Key = "ChartTextCollection1";
        //вторая коллекция подписей данных
        public const string chartTextCollection2Key = "ChartTextCollection2";
        //сортировка элементов оси
        public const string axisSortTypeKey = "AxisSortType";
        //сортировка по именам элементов
        public const string isSortByNameKey = "IsSortByName";
        //обратный порядок элементов
        public const string reverseOrderKey = "ReverseOrder";


        #endregion

        #region Chart arrays

        private static readonly ChartType[] ChartGroupsArea = new ChartType[] {
            ChartType.AreaChart3D,
            ChartType.AreaChart,
            ChartType.SplineAreaChart3D,
            ChartType.SplineAreaChart,
            ChartType.StackAreaChart,
            ChartType.StackSplineAreaChart,
            ChartType.StepAreaChart
		};

        private static readonly ChartType[] ChartGroupsBar = new ChartType[] {
            ChartType.BarChart3D,
            ChartType.BarChart,
            ChartType.CylinderBarChart3D,
            ChartType.CylinderStackBarChart3D,
            ChartType.Stack3DBarChart,
            ChartType.StackBarChart,
            ChartType.GanttChart,
		};

        private static readonly ChartType[] ChartGroupsMisc = new ChartType[] {
            ChartType.BoxChart,
            //ChartType.Composite,
            ChartType.FunnelChart3D,
            ChartType.FunnelChart,
            ChartType.ProbabilityChart,
            ChartType.PolarChart,
            ChartType.ParetoChart,
            ChartType.ConeChart3D
		};

        private static readonly ChartType[] ChartGroupsBubble = new ChartType[] {
            ChartType.BubbleChart3D,
            ChartType.BubbleChart
		};

        private static readonly ChartType[] ChartGroupsCandle = new ChartType[] {
            ChartType.CandleChart
		};

        private static readonly ChartType[] ChartGroupsColumn = new ChartType[] {
            ChartType.ColumnChart3D,
            ChartType.ColumnChart,
            ChartType.ColumnLineChart,
            ChartType.CylinderColumnChart3D,
            ChartType.CylinderStackColumnChart3D,
            ChartType.Stack3DColumnChart,
            ChartType.StackColumnChart,
            ChartType.HistogramChart
		};

        private static readonly ChartType[] ChartGroupsLine = new ChartType[] {
            ChartType.LineChart3D,
            ChartType.LineChart,
            ChartType.ScatterLineChart,
            ChartType.SplineChart3D,
            ChartType.SplineChart,
            ChartType.StackLineChart,
            ChartType.StackSplineChart,
            ChartType.StepLineChart
		};

        private static readonly ChartType[] ChartGroupsDoughnut = new ChartType[] {
            ChartType.DoughnutChart3D,
            ChartType.DoughnutChart
		};

        private static readonly ChartType[] ChartGroupsHeat = new ChartType[] {
            ChartType.HeatMapChart3D,
            ChartType.HeatMapChart,
            ChartType.TreeMapChart
		};

        private static readonly ChartType[] ChartGroupsPie = new ChartType[] {
            ChartType.PieChart3D,
            ChartType.PieChart
		};

        private static readonly ChartType[] ChartGroupsRadar = new ChartType[] {
            ChartType.RadarChart
		};

        private static readonly ChartType[] ChartGroupsPyramid = new ChartType[] {
            ChartType.PyramidChart3D,
            ChartType.PyramidChart
		};

        private static readonly ChartType[] ChartGroupsPoint = new ChartType[] {
            ChartType.PointChart3D,
            ChartType.ScatterChart
		};

        #endregion // Chart arrays

        private ChartReportElement _activeChartElement;
        private ToolbarsManage _toolbarsManager;
        //тип диаграммы перед началом выбора через Галерею Диаграмм
        private string befoEditChartType = string.Empty;
        //Если true - то события контролов не выполняются 
        private bool isMayHook = false;

        #region инструменты диаграммы
        /// <summary>
        /// вкладка со свойствами
        /// </summary>
        private RibbonTab chartPropertiesTab;
        /// <summary>
        /// вкладка Ряды
        /// </summary>
        private RibbonTab chartRowTab;
        /// <summary>
        /// вкладка Категории
        /// </summary>
        private RibbonTab chartColumnTab;

        /// <summary>
        /// группа свойств 3D диаграмм
        /// </summary>
        private RibbonGroup chart3DTransformGroup;
        /// <summary>
        /// группа свойств структурных диаграмм
        /// </summary>
        private RibbonGroup structuralChartGroup;
        /// <summary>
        /// группа свойств осей диаграммы
        /// </summary>
        private RibbonGroup chartAxisGroup;
        /// <summary>
        /// группа типов диаграмм
        /// </summary>
        private RibbonGroup chartTypesGroup;
        /// <summary>
        /// группа общих свойств диаграмм
        /// </summary>
        private RibbonGroup chartCommonGroup;
        /// <summary>
        /// управление данными рядов
        /// </summary>
        private RibbonGroup rowDataManageGroup;
        /// <summary>
        /// управление данными столбцов
        /// </summary>
        private RibbonGroup columnDataManageGroup;

        /// <summary>
        /// галерея диаграм
        /// </summary>
        private PopupGalleryTool chartGallery;
        /// <summary>
        /// контейнер для редактора 3D диаграммы
        /// </summary>
        private ControlContainerTool chart3DTransformContainer;
        /// <summary>
        /// редактор 3D диаграмм
        /// </summary>
        private Chart3DTransform chart3DTransform;
        /// <summary>
        /// контейнер для редактора легенды диаграммы
        /// </summary>
        private ControlContainerTool chartLegendEditorContainer;
        /// <summary>
        /// редактор легенды диаграммы
        /// </summary>
        private ChartLegendEditor chartLegendEditor;
        /// <summary>
        /// контейнер для редактора свойств структурных диаграмм
        /// </summary>
        private ControlContainerTool structuralChartEditorContainer;
        /// <summary>
        /// редактор свойств структурных диаграмм
        /// </summary>
        private StructuralChartEditor structuralChartEditor;
        /// <summary>
        /// контейнер для редактора осей диаграммы
        /// </summary>
        private ControlContainerTool chartAxisEditorContainer;
        /// <summary>
        /// редактор осей диаграммы
        /// </summary>
        private ChartAxisEditor chartAxisEditor;
        /// <summary>
        /// подписи данных
        /// </summary>
        private PopupGalleryTool chartText;
        /// <summary>
        /// оси координат
        /// </summary>
        private ButtonTool axesSettings;
        /// <summary>
        /// коллекция подписей данных 1
        /// </summary>
        private ButtonTool chartTextCollection1;
        /// <summary>
        /// коллекция подписей данных 2
        /// </summary>
        private ButtonTool chartTextCollection2;
        /// <summary>
        /// Сортировка по именам
        /// </summary>
        private StateButtonTool isSortByName;
        /// <summary>
        /// Обратный порядок
        /// </summary>
        private StateButtonTool reverseOrder;
        /// <summary>
        /// Сортировка элементов оси
        /// </summary>
        private ComboBoxTool axisSortType;



        #endregion

        public ChartToolBar(ToolbarsManage toolbarManager)
        {
            this.ToolbarsManager = toolbarManager;
        }

        public void Initialize()
        {
            //Устанавливаем ссылки на контролы диаграммы
            this.SetLinkOnTools();
            this.InitializeChartGalleryTool();
            //Устанавливаем контролам диаграммы, требуемые обработчики
            this.SetEventHandlers();
            //Устанавливаем заголовкам вкладок русские имена
            this.SetRusTabCaption();
        }

        /// <summary>
        /// Устанавливаем ссылки на контролы диаграммы
        /// </summary>
        private void SetLinkOnTools()
        {
            //Вкладки
            this.chartPropertiesTab = this.Toolbars.Ribbon.Tabs[propertiesTabKey];
            this.chartRowTab = this.Toolbars.Ribbon.Tabs[chartRowTabKey];
            this.chartColumnTab = this.Toolbars.Ribbon.Tabs[chartColumnTabKey];
            //Группы
            this.chart3DTransformGroup = this.chartPropertiesTab.Groups[chart3DTransformGroupKey];
            this.structuralChartGroup = this.chartPropertiesTab.Groups[structuralChartGroupKey];
            this.chartAxisGroup = this.chartPropertiesTab.Groups[chartAxisGroupKey];
            this.chartTypesGroup = this.chartPropertiesTab.Groups[typesGalleryGroupKey];
            this.chartCommonGroup = this.chartPropertiesTab.Groups[chartCommonGroupKey];
            this.rowDataManageGroup = this.chartRowTab.Groups[dataManageGroupKey];
            this.columnDataManageGroup = this.chartColumnTab.Groups[dataManageGroupKey];

            //Контролы
            this.chartGallery = (PopupGalleryTool)this.Toolbars.Tools[typesGalleryKey];
            this.chart3DTransformContainer = (ControlContainerTool)this.Toolbars.Tools[chart3DTransformContainerKey];
            this.chart3DTransform = (Chart3DTransform)this.chart3DTransformContainer.Control;
            this.chartLegendEditorContainer = (ControlContainerTool)this.Toolbars.Tools[chartLegendEditorContainerKey];
            this.chartLegendEditor = (ChartLegendEditor)this.chartLegendEditorContainer.Control;
            this.structuralChartEditorContainer = (ControlContainerTool)this.Toolbars.Tools[structuralChartEditorContainerKey];
            this.structuralChartEditor = (StructuralChartEditor)this.structuralChartEditorContainer.Control;
            this.chartAxisEditorContainer = (ControlContainerTool)this.Toolbars.Tools[chartAxisEditorContainerKey];
            this.chartAxisEditor = (ChartAxisEditor)this.chartAxisEditorContainer.Control;
            this.chartText = (PopupGalleryTool)this.Toolbars.Tools[chartTextKey];
            this.axesSettings = (ButtonTool)this.Toolbars.Tools[axesSettingsKey];
            this.chartTextCollection1 = (ButtonTool)this.Toolbars.Tools[chartTextCollection1Key];
            this.chartTextCollection2 = (ButtonTool)this.Toolbars.Tools[chartTextCollection2Key];
            this.isSortByName = (StateButtonTool)this.Toolbars.Tools[isSortByNameKey];
            this.reverseOrder = (StateButtonTool)this.Toolbars.Tools[reverseOrderKey];
            this.axisSortType = (ComboBoxTool)this.Toolbars.Tools[axisSortTypeKey];

        }


        /// <summary>
        /// Установить обработчики событий
        /// </summary>
        private void SetEventHandlers()
        {
            this.chartGallery.GalleryToolItemClick += new GalleryToolItemEventHandler(ChartGallery_GalleryToolItemClick);
            this.chartGallery.GalleryToolActiveItemChange += new GalleryToolItemEventHandler(ChartGallery_GalleryToolActiveItemChange);
            this.chart3DTransform.ExitEditMode += new EventHandler(chartTransform3D_ExitEditMode);
            this.chartLegendEditor.ExitEditMode += new EventHandler(chartLegendEditor_ExitEditMode);
            this.structuralChartEditor.ExitEditMode += new EventHandler(structuralChartEditor_ExitEditMode);
            this.chartAxisEditor.ExitEditMode += new EventHandler(chartAxisEditor_ExitEditMode);
            this.chartText.ToolClick += new ToolClickEventHandler(chartText_ToolClick);
            this.axesSettings.ToolClick += new ToolClickEventHandler(axesSettings_ToolClick);
            this.chartText.BeforeToolDropdown += new BeforeToolDropdownEventHandler(chartText_BeforeToolDropdown);
            this.chartTextCollection1.ToolClick += new ToolClickEventHandler(chartTextCollection1_ToolClick);
            this.chartTextCollection2.ToolClick += new ToolClickEventHandler(chartTextCollection2_ToolClick);
            this.isSortByName.ToolClick += new ToolClickEventHandler(isSortByName_ToolClick);
            this.reverseOrder.ToolClick += new ToolClickEventHandler(reverseOrder_ToolClick);
            this.axisSortType.ToolValueChanged += new ToolEventHandler(axisSortType_ToolValueChanged);

            this.Toolbars.AfterRibbonTabSelected += new RibbonTabEventHandler(Toolbars_AfterRibbonTabSelected);

        }

        /// <summary>
        /// Устанавливаем заголовкам вкладок русские имена, непонятно почему, но они очень 
        /// часто меняются на ключи
        /// </summary>
        private void SetRusTabCaption()
        {
            this.chartPropertiesTab.Caption = "Свойства";
            this.chartRowTab.Caption = "Ряды";
            this.chartColumnTab.Caption = "Категории";
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
                case chartRowTabKey:
                    result = SelectedTabType.Row;
                    break;
                //колонки
                case chartColumnTabKey:
                    result = SelectedTabType.Column;
                    break;
            }
            return result;
        }


        /// <summary>
        /// Обнавляем значение контролов на вкладках диаграммы
        /// </summary>
        /// <param name="activeElement"></param>
        public void RefreshTabsTools(ChartReportElement activeElement)
        {
            this.ActiveChartElement = activeElement;
            if ((this.ActiveChartElement != null) && (this.ActiveChartElement.PivotData != null)
                && (this.ActiveChartElement.Chart != null))
            {
                try
                {
                    this.isMayHook = true;
                    this.RefreshCommonTab(this.ActiveChartElement);
                    UltraChart activeChart = activeElement.Chart;
                    if (activeChart.ChartType == ChartType.Composite)
                    {
                        this.chartRowTab.Visible = false;
                        this.chartColumnTab.Visible = false;
                    }
                    else
                    {
                        this.chartRowTab.Visible = true;
                        this.chartColumnTab.Visible = true;
                        switch (GetSelectedTab())
                        {
                            case SelectedTabType.Column:
                                RefreshColumnTab(activeElement);
                                break;
                            case SelectedTabType.Row:
                                RefreshRowTab(activeElement);
                                break;
                        }
                    }
                }
                finally
                {
                    this.isMayHook = false;
                }
            }
        }

        /// <summary>
        /// Обновляются контролы на вкладке Свойства диаграммы
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshCommonTab(ChartReportElement activeElement)
        {
            UltraChart activeChart = activeElement.Chart;
            this.chartLegendEditor.Chart = activeChart;
            this.Refresh3DTransformGroup(activeChart);
            this.RefreshStructuralChartGroup(activeChart);
            this.RefreshAxisGroup(activeChart);
            this.RefreshChartTextControl(activeChart);
            this.RefreshAxesSettingsControl(activeChart);
            if (activeChart.ChartType == ChartType.Composite)
            {
                this.chartTypesGroup.Visible = false;
            }
            else
            {
                this.chartTypesGroup.Visible = true;
            }
        }

        /// <summary>
        /// Обновляет вкладку со свойствами рядов
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshRowTab(ChartReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Row);
        }

        /// <summary>
        /// Обновляет вкладку со свойствами категорий
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshColumnTab(ChartReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Column);
        }

        /// <summary>
        /// Обновляет значения общих свойств осей
        /// </summary>
        /// <param name="activeElement"></param>
        /// <param name="tabType"></param>
        private void RefreshAxisCommonProperties(ChartReportElement activeElement, SelectedTabType tabType)
        {
            if (activeElement == null)
                return;

            PivotAxis currentAxis = (tabType == SelectedTabType.Row) ? activeElement.PivotData.RowAxis :
                activeElement.PivotData.ColumnAxis;
            this.isSortByName.Checked = currentAxis.SortByName;
            this.reverseOrder.Checked = currentAxis.ReverseOrder;
            this.axisSortType.SelectedIndex = (int)currentAxis.SortType;
        }

        /// <summary>
        /// Устанавливает видимость группы в зависимости от типа диаграммы, так же инициализирует контролы
        /// находящиеся на ней
        /// </summary>
        /// <param name="activeChart"></param>
        private void Refresh3DTransformGroup(UltraChart activeChart)
        {
            if (activeChart == null)
                return;

            //Отображаем группу только для 3D диаграмм
            if (Common.InfragisticsUtils.Is3DChart(activeChart))
            {
                this.chart3DTransformGroup.Visible = true;
                this.chart3DTransform.Chart = activeChart;
            }
            else
                this.chart3DTransformGroup.Visible = false;
        }

        /// <summary>
        /// Устанавливает видимость группы, только для структурных диаграмм, и инициализирует 
        /// контрол на ней
        /// </summary>
        /// <param name="activeChart"></param>
        private void RefreshStructuralChartGroup(UltraChart activeChart)
        {
            if (activeChart == null)
                return;

            //Отображаем группу только для структурныхых диаграмм
            if (Common.InfragisticsUtils.IsStructuralChart(activeChart))
            {
                this.structuralChartGroup.Visible = true;
                this.structuralChartEditor.Chart = activeChart;
            }
            else
                this.structuralChartGroup.Visible = false;
        }

        /// <summary>
        /// Устанавливает видимость группы, только для диаграмм c осями, и инициализирует 
        /// контрол на ней
        /// </summary>
        /// <param name="activeChart"></param>
        private void RefreshAxisGroup(UltraChart activeChart)
        {
            if (activeChart == null)
                return;

            //Отображаем группу только для диаграмм имеющих оси, 
            //BubbleChart3D и PoinChart3D - хоть оси и имеют, но не реагируют на уставку свойств
            //скорей всего глюк Infragistics, пока для них тоже скрываем
            if (Common.InfragisticsUtils.IsAvaibleAxis(activeChart) && 
                (activeChart.ChartType != ChartType.BubbleChart3D) && 
                (activeChart.ChartType != ChartType.PointChart3D))
            {
                if (activeChart.ChartType == ChartType.Composite)
                {
                    if (MainForm.FieldListEditor.CompositeChartEditor.SelectedLayer != -1)
                    {
                        this.chartAxisGroup.Visible = true;
                        this.chartAxisEditor.SelectedCompositeLayer = MainForm.FieldListEditor.CompositeChartEditor.SelectedLayer;
                        this.chartAxisEditor.Chart = activeChart;
                    }
                    else
                    {
                        this.chartAxisGroup.Visible = false;
                    }
                }
                else
                {
                    this.chartAxisGroup.Visible = true;
                    this.chartAxisEditor.Chart = activeChart;
                }
            }
            else
            {
                this.chartAxisGroup.Visible = false;
            }
        }

        private void RefreshChartTextControl(UltraChart activeChart)
        {
            if (activeChart == null)
                return;
            if (Common.InfragisticsUtils.IsAvaibleChartText(this.ActiveChart))
            {
                this.chartText.SharedProps.Enabled = true;
                this.chartText.Checked = this.GetCheckedChartText(this.ActiveChart);
            }
            else
            {
                this.chartText.SharedProps.Enabled = false;
            }
            // пока скрываем группу общих свойств для композитных диаграмм
            this.chartCommonGroup.Visible = !(activeChart.ChartType == ChartType.Composite);
        }

        private void RefreshAxesSettingsControl(UltraChart activeChart)
        {
            if (activeChart == null)
                return;
            if (Common.InfragisticsUtils.IsAvaibleAxesSettings(this.ActiveChart))
            {
                this.axesSettings.SharedProps.Enabled = true;
            }
            else
            {
                this.axesSettings.SharedProps.Enabled = false;
            }
        }

        /// <summary>
        /// Загружаем настройки панели инструментов, сохранненый в XML
        /// </summary>
        /// <param name="xmlNode"></param>
        public void Load(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return;
            try
            {
                this.LoadTabProperties(xmlNode.SelectSingleNode(ToolbarUtils.tabsNodeName));
            }
            catch
            {
                FormException.ShowErrorForm(new Exception("При загрузке данных в панель инструментов диаграммы, произошла ошибка"));
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
                //Редактор 3D диаграмм
                this.chart3DTransform.BackColor = propertiesTabBackColor;
                //Редактор легенды
                this.chartLegendEditor.BackColor = propertiesTabBackColor;
                //Редактор структурныхых диаграмм
                this.structuralChartEditor.BackColor = propertiesTabBackColor;
                //Редактор осей диаграммы
                this.chartAxisEditor.BackColor = propertiesTabBackColor;
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

        #region Обработчики
        void Toolbars_AfterRibbonTabSelected(object sender, RibbonTabEventArgs e)
        {
            RefreshTabsTools(this.ActiveChartElement);
        }

        /// <summary>
        /// Нажатие на элемент в галерее
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChartGallery_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            ChartType newChartType = (ChartType)Enum.Parse(typeof(ChartType), this.GetActiveChartType());
            // если диаграмма является дочерней и новый тип несовместим
            if (MainForm.IsCompositeChildChart(this.ActiveChartElement.UniqueName) &&
                !CompositeChartUtils.IsCompositeCompatibleType(newChartType))
            {
                ChartType curChartType = (ChartType)Enum.Parse(typeof(ChartType), this.befoEditChartType);
                
                string elementText = MainForm.GetReportElementText(this.ActiveChartElement.UniqueName);
                string curChartTypeText = ChartTypeConverter.GetLocalizedChartType(curChartType);
                string newChartTypeText = ChartTypeConverter.GetLocalizedChartType(newChartType);
                string msg = string.Format("Диаграмма \"{0}\" используется в качестве слоя композитной диаграммы. " +
                                           "Тип \"{1}\" является несовместимым для композитной диаграммы. Удалить слой \"{0}({2})\" из композитной диаграммы?",
                                           elementText, newChartTypeText, curChartTypeText);
                if (MessageBox.Show(msg, "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // восстанавливаем исходный тип
                    this.SetActiveChartType(this.befoEditChartType);
                    this.befoEditChartType = string.Empty;
                    return;
                }
                else
                {
                    // выводим подтверждение
                    msg = string.Format("Вы действительно хотите удалить слой \"{0}({1})\" из композитной диаграммы?",
                        elementText, curChartTypeText);
                    if (MessageBox.Show(msg, "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {                    
                        // восстанавливаем исходный тип
                        this.SetActiveChartType(this.befoEditChartType);
                        this.befoEditChartType = string.Empty;
                        return;
                    }
                    else
                    {
                        // удаляем все слои, куда входила дочерняя
                        MainForm.RemoveCompositeChildKey(this.ActiveChartElement.UniqueName);
                    }
                }
            }
            
            //т.к. выбор происходил с помощью галереии диаграмм, значит тип диаграммы который 
            //выбрали этим кликом уже был активирован и присвоен, здесь лишь остается очистить
            //befoEditChartType для того, что бы тип остался выбранным после деактивации галерии
            this.befoEditChartType = string.Empty;
            this.RefreshCommonTab(this.ActiveChartElement);
            this.AfterEdited();
        }

        /// <summary>
        /// Активация выбранного элемента в галерее
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChartGallery_GalleryToolActiveItemChange(object sender, GalleryToolItemEventArgs e)
        {
            bool f = false;
            if (this.isMayHook)
                return;
            try
            {
                if (e.Item != null)
                {
                    ChartType chartType = (ChartType) Enum.Parse(typeof (ChartType), e.Item.Key);
                    string oldChartType = ActiveChart.ChartType.ToString();
                    if ((chartType == ChartType.HeatMapChart) &&
                        !InfragisticsUtils.CheckChangeToHeatmapChart(ActiveChart, chartType))
                    {
//                        Common.CommonUtils.ProcessException(new Exception(
//                                "Index was out of range. Must be non-negative and less than the size of the collection."), true);
//                        //chartGallery.ClosePopup();
                        //FormException.ShowErrorForm(
                        //    new Exception(
                        //        "Index was out of range. Must be non-negative and less than the size of the collection."));
                        
                        //SetActiveChartType(oldChartType);
                        //this.befoEditChartType = string.Empty;
                        //return;
                        f = true;
                    }
                }
            }
            catch
            {
                f = true;
                //return;
            }
            if (!f)
            {
                this.ActivateChartType(e);
            }
        }

        void chartTransform3D_ExitEditMode(object sender, EventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement)
            {
                this.AfterEdited();
            } 
        }

        void chartLegendEditor_ExitEditMode(object sender, EventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement)
            {
                this.AfterEdited();
            } 
        }

        void structuralChartEditor_ExitEditMode(object sender, EventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement)
            {
                this.AfterEdited();
            } 
        }

        void chartAxisEditor_ExitEditMode(object sender, EventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement)
            {
                this.AfterEdited();
            } 
        }

        void chartText_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement)
            {
                //получим колекции подписей данных для данного вида диаграммы
                ChartTextCollection[] chartTexts = Common.InfragisticsUtils.GetChartTextCollection(this.ActiveChart);
                //если колекция одна, мы ее сразу показываем
                if (chartTexts.GetValue(1) == null)
                {
                    e.Cancel = true;
                    CustomChartTextCollectionEditorForm temp = 
                        new CustomChartTextCollectionEditorForm(chartTexts[0], null);
                    this.AfterEdited();
                }
                else
                {
                    if (this.ActiveChart.ChartType == ChartType.ColumnLineChart)
                    {
                        this.chartTextCollection1.SharedProps.Caption = "Колонок";
                    }
                    if (this.ActiveChart.ChartType == ChartType.ScatterLineChart)
                    {
                        this.chartTextCollection1.SharedProps.Caption = "Точек";
                    }
                    this.chartTextCollection1.Tag = chartTexts[0];
                    this.chartTextCollection2.Tag = chartTexts[1];
                }
            }
        }

        void chartText_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement)
            {
                ChartTextCollection[] chartTexts =
                    Common.InfragisticsUtils.GetChartTextCollection(this.ActiveChart);

                foreach (ChartTextCollection item in chartTexts)
                {
                    if (this.chartText.Checked)
                        this.ShowChartTextItem(item);
                    else
                        this.HideChartTextItem(item);
                }
                //на этих типах диаграмм что бы отобразились подписи надо их "пошевелить"
                if (this.ActiveChart.ChartType == ChartType.FunnelChart ||
                    this.ActiveChart.ChartType == ChartType.PyramidChart)
                {
                    this.ActiveChart.DataBind();
                }
                this.AfterEdited();
            }
        }

        void axesSettings_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement)
            {
                AxesSettingsForm axesFrm = new AxesSettingsForm(this.ActiveChart);
                axesFrm.ShowDialog();

                if(this.MainForm.PropertyGrid.SelectedObject is ChartReportElementBrowseAdapter)
                {
                    ChartReportElementBrowseAdapter chartElement =
                        (ChartReportElementBrowseAdapter) this.MainForm.PropertyGrid.SelectedObject;
                    
                    if (chartElement.AxiesBrowse.XAxisBrowse != null)
                        chartElement.AxiesBrowse.XAxisBrowse.AxisLabelsBrowse.RefreshFormat();
                    if (chartElement.AxiesBrowse.YAxisBrowse != null)
                        chartElement.AxiesBrowse.YAxisBrowse.AxisLabelsBrowse.RefreshFormat();
                    if (chartElement.AxiesBrowse.ZAxisBrowse != null)
                        chartElement.AxiesBrowse.ZAxisBrowse.AxisLabelsBrowse.RefreshFormat();
                    if (chartElement.AxiesBrowse.X2AxisBrowse != null)
                        chartElement.AxiesBrowse.X2AxisBrowse.AxisLabelsBrowse.RefreshFormat();
                    if (chartElement.AxiesBrowse.Y2AxisBrowse != null)
                        chartElement.AxiesBrowse.Y2AxisBrowse.AxisLabelsBrowse.RefreshFormat();
                }

                this.AfterEdited();
            }
        }


        void chartTextCollection1_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement && (this.chartTextCollection1.Tag != null))
            {
                ChartTextCollection chartText = (ChartTextCollection)this.chartTextCollection1.Tag;
                CustomChartTextCollectionEditorForm temp = new CustomChartTextCollectionEditorForm(chartText, null);
                this.AfterEdited();
            }
        }

        void chartTextCollection2_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveChartElement && (this.chartTextCollection2.Tag != null))
            {
                ChartTextCollection chartText = (ChartTextCollection)this.chartTextCollection2.Tag;
                CustomChartTextCollectionEditorForm temp = new CustomChartTextCollectionEditorForm(chartText, null);
                this.AfterEdited();
            }
        }

        void axisSortType_ToolValueChanged(object sender, ToolEventArgs e)
        {
            if (this.isMayHook)
                return;

            switch (GetSelectedTab())
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

        void isSortByName_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveChartElement)
            {
                switch (GetSelectedTab())
                {
                    case SelectedTabType.Row:
                        {
                            this.ActiveChartElement.PivotData.RowAxis.SortByName = this.isSortByName.Checked;
                            break;
                        }
                    case SelectedTabType.Column:
                        {
                            this.ActiveChartElement.PivotData.ColumnAxis.SortByName = this.isSortByName.Checked;
                            break;
                        }
                }
                this.AfterEdited();
            }
        }

        void reverseOrder_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            if (this.IsExistActiveChartElement)
            {
                switch (GetSelectedTab())
                {
                    case SelectedTabType.Row:
                        {
                            this.ActiveChartElement.PivotData.RowAxis.ReverseOrder = this.reverseOrder.Checked;
                            break;
                        }
                    case SelectedTabType.Column:
                        {
                            this.ActiveChartElement.PivotData.ColumnAxis.ReverseOrder = this.reverseOrder.Checked;
                            break;
                        }
                }
                this.AfterEdited();
            }
        }

        #endregion

        #region Вспомогательные методы для установки правильных значений контролам
        /// <summary>
        /// Если у диаграммы в колкеции подписей данных существует хоть один видимый 
        /// элемент с полным отображением данных, значит контрол включен
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private bool GetCheckedChartText(UltraChart chart)
        {
            bool result = false;
            if (chart != null)
            {
                ChartTextCollection[] chartTexts = Common.InfragisticsUtils.GetChartTextCollection(chart);
                result = this.IsExistsVisibleChartTextItem(chartTexts[0]);
                if (!result)
                    result = this.IsExistsVisibleChartTextItem(chartTexts[1]);
            }
            return result;
        }

        /// <summary>
        /// Существует ли в колекции хоть один видимый элемент с полным отображением данных
        /// </summary>
        /// <param name="chartText"></param>
        /// <returns></returns>
        private bool IsExistsVisibleChartTextItem(ChartTextCollection chartText)
        {
            if (chartText != null)
            {
                foreach (ChartTextAppearance item in chartText)
                {
                    if ((item.Visible == true) && (item.Row == -2) && (item.Column == -2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Показывает в коллекции подписей элементы с полным отображением данных
        /// </summary>
        /// <param name="chartText"></param>
        private void ShowChartTextItem(ChartTextCollection chartText)
        {
            if (chartText != null)
            {
                bool isExistItem = false;
                foreach (ChartTextAppearance item in chartText)
                {
                    if ((item.Row == -2) && (item.Column == -2))
                    {
                        item.Visible = true;
                        isExistItem = true;
                    }
                }
                if (!isExistItem)
                {
                    ChartTextAppearance item = new ChartTextAppearance();
                    item.Row = -2;
                    item.Column = -2;
                    item.Visible = true;
                    chartText.Add(item);
                }
            }
        }

        /// <summary>
        /// Делает невидимым в колекции подписей, элементы с полным отображением данных
        /// </summary>
        /// <param name="chartText"></param>
        private void HideChartTextItem(ChartTextCollection chartText)
        {
            if (chartText != null)
            {
                foreach (ChartTextAppearance item in chartText)
                {
                    if ((item.Visible == true) && (item.Row == -2) && (item.Column == -2))
                    {
                        item.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Установить тип диаграммы активному элементу отчета
        /// </summary>
        /// <param name="sType"></param>
        private void SetActiveChartType(string sType)
        {
            if (sType == string.Empty)
            {
                return;
            }

            if (this.IsExistActiveChartElement)
            {
                ChartType chartType = (ChartType) Enum.Parse(typeof (ChartType), sType);
                InfragisticsUtils.ChangeChartType(this.ActiveChart, chartType);
                ChartFormatBrowseClass.DoFormatStringChange();
                this.ActiveChartElement.Invalidate(true);
            }
        }

        /// <summary>
        /// Получить тип диаграммы у активного элемента отчета
        /// </summary>
        /// <returns></returns>
        private string GetActiveChartType()
        {
            string result = string.Empty;
            if (this.IsExistActiveChartElement)
                result = this.ActiveChart.ChartType.ToString();
            return result;
        }

        /// <summary>
        /// Активация типа выбранного в галерее диаграмм
        /// </summary>
        /// <param name="e"></param>
        private void ActivateChartType(GalleryToolItemEventArgs e)
        {
            if (e.Item != null)
            {
                               
                //если активация элемента происходи впервые, запомним действительный тип
                //диаграммы
                if (this.befoEditChartType == string.Empty)
                {
                    this.befoEditChartType = this.GetActiveChartType();
                }

                //установим новый тип диаграммы
                this.SetActiveChartType(e.Item.Key);
            }
            else
            {
                //сюда попадаем при деактивации галереии диаграмм, если ничего не выбрали,
                //значит в befoEditChartType должен сохраниться тип диаграммы запомненый перед
                //началом выбора его и выставляем, если выбор все даки был сделан
                //(кликали по элементам) то ничего не происходит, т.к. это значение очищалось
                this.SetActiveChartType(this.befoEditChartType);
                this.befoEditChartType = string.Empty;
            }
        }

        /// <summary>
        /// Редактирует тип сортировки элементов у строк 
        /// </summary>
        private void EditRowSortType()
        {
            if (this.IsExistActiveChartElement)
            {
                SortType newType = (SortType)this.axisSortType.SelectedIndex;
                if (newType != this.ActiveChartElement.PivotData.RowAxis.SortType)
                {
                    this.ActiveChartElement.PivotData.RowAxis.SortType = newType;
                    this.AfterEdited();
                }
            }
        }

        /// <summary>
        /// Редактирует тип сортировки элементов у колонок 
        /// </summary>
        private void EditColumnSortType()
        {
            if (this.IsExistActiveChartElement)
            {
                SortType newType = (SortType)this.axisSortType.SelectedIndex;
                if (newType != this.ActiveChartElement.PivotData.ColumnAxis.SortType)
                {
                    this.ActiveChartElement.PivotData.ColumnAxis.SortType = newType;
                    this.AfterEdited();
                }
            }
        }

        #endregion

        #region CreateChartGalleryTool
        /// <summary>
        /// Helper method to create a popup gallery tool with chart related images.
        /// </summary>
        /// <returns>The popup gallery tool that was created</returns>
        private void InitializeChartGalleryTool()
        {
            // we'll use the first image for the tool. the large image will be used 
            // for a popup gallery tool when its shown as a large tool in the ribbon
            // which should occur when there is not enough room to show the minimum
            // number of preview items
            this.chartGallery.SharedProps.AppearancesLarge.Appearance.Image = ToolbarsManage.largeImageIndexFirstChart;

            string[] chartNames = Enum.GetNames(typeof(ChartType));
            // create the items for the gallery
            for (int i = 0; i < chartNames.Length; i++)
            {
                string name = chartNames[i];
                GalleryToolItem item = this.chartGallery.Items.Add(name);

                ChartType chartType = (ChartType)Enum.Parse(typeof(ChartType), name);
                item.ToolTipText = ChartTypeConverter.GetLocalizedChartType(chartType);

                item.Settings.Appearance.Image = i + ToolbarsManage.largeImageIndexFirstChart;
            }

            ChartType[][] groups = new ChartType[][] { 
						ChartGroupsArea, ChartGroupsBar, ChartGroupsBubble, 
						ChartGroupsCandle, ChartGroupsColumn, ChartGroupsDoughnut, 
						ChartGroupsHeat, ChartGroupsLine, ChartGroupsMisc, ChartGroupsPie, 
						ChartGroupsPyramid, ChartGroupsRadar, ChartGroupsPoint};
            string[] groupNames = new string[] {
						"Area", "Bar", "Bubble",
						"Candle", "Column", "Doughnut",
						"Heat", "Line", "Misc", "Pie",
						"Pyramid", "Radar", "Point"
			};

            // popup gallery tools have the ability to display the items
            // in the dropdown within groups.
            for (int i = 0; i < groups.Length; i++)
            {
                ChartType[] chartGroup = groups[i];
                string groupName = groupNames[i];

                // create the group
                GalleryToolItemGroup group = this.chartGallery.Groups.Add(groupName);
                group.Text = GetLocalizedChartGroup(groupName);

                // add all the keys of the items from the gallery tool's
                // Items collection that should be displayed within the group
                // note: items can be displayed in multiple groups
                for (int j = 0; j < chartGroup.Length; j++)
                {
                    string chartName = chartGroup[j].ToString();

                    group.Items.Add(chartName);
                }

                // popup gallery tools also have the ability to display a filter
                // bar at the top of the dropdown that can be used to filter the 
                // list of groups displayed in the dropdown.
                // in this exameple, we're createing a filter for each group
                GalleryToolItemGroupFilter filter = this.chartGallery.GroupFilters.Add(groupName);
                filter.Groups.Add(groupName);
            }

            // create a filter that will show all the groups
            GalleryToolItemGroupFilter filterAll = this.chartGallery.GroupFilters.Add("All");
            filterAll.Groups.AddRange(groupNames);

            // and make that the default/selected filter
            this.chartGallery.SelectedGroupFilterKey = "All";
        }

        /// <summary>
        /// Руссификация группы диаграмм
        /// </summary>
        /// <param name="groupName">англ название</param>
        /// <returns>русское название</returns>
        private string GetLocalizedChartGroup(string groupName)
        {
            switch (groupName)
            {
                case "Area":
                    return "С областями";
                case "Bar":
                    return "Линейчатая";
                case "Bubble":
                    return "Пузырьковая";
                case "Candle":
                    return "Биржевая";
                case "Column":
                    return "Гистограмма";
                case "Doughnut":
                    return "Кольцевая";
                case "Heat":
                    return "Поверхность";
                case "Line":
                    return "График";
                case "Misc":
                    return "Разные";
                case "Pie":
                    return "Круговая";
                case "Pyramid":
                    return "Пирамидальная";
                case "Radar":
                    return "Лепестковая";
                case "Point":
                    return "Точечная";
                default:
                    return groupName;

            }
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
        /// Активная диаграмма
        /// </summary>
        public UltraChart ActiveChart
        {
            get { return this.IsExistActiveChartElement ? this.ActiveChartElement.Chart : null; }
        }

        /// <summary>
        /// Существует ли активный элемент отчета с диаграммой
        /// </summary>
        public bool IsExistActiveChartElement
        {
            get { return this.ActiveChartElement != null; }
        }

        /// <summary>
        /// Активный элемент отчета с диаграммой по которому инициализирован тулбар
        /// </summary>
        public ChartReportElement ActiveChartElement
        {
            get { return _activeChartElement; }
            set { _activeChartElement = value; }
        }

        /// <summary>
        /// Лента
        /// </summary>
        public UltraToolbarsManager Toolbars
        {
            get { return this.ToolbarsManager.Toolbars; }
            set { this.ToolbarsManager.Toolbars = value; }
        }
        #endregion

        #region Дополнительные классы

        /// <summary>
        /// Тип владки диаграммы
        /// </summary>
        enum SelectedTabType
        {
            Row,
            Column,
            Common,
            None
        }

        #endregion

    }
}
