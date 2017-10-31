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
    /// ����� ��������� � ������ ������������, ����������� � ���������
    /// </summary>
    public class ChartToolBar
    {
        #region �����
        //�������
        //������� ������� ���������
        public const string propertiesTabKey = "ChartPropertiesTab";
        //������� ����� ���������
        public const string chartRowTabKey = "ChartRowTab";
        //������� ��������� ���������
        public const string chartColumnTabKey = "ChartColumnTab";

        //�����
        //������ � �������� ��������
        public const string typesGalleryGroupKey = "ChartTypesGroup";
        //�������� 3D ���������
        public const string chart3DTransformGroupKey = "3DChartGroup";
        //�������� ����������� ��������
        public const string structuralChartGroupKey = "StructuralChartGroup";
        //�������� ���� ���������
        public const string chartAxisGroupKey = "ChartAxisGroup";
        //����� �������� ���������
        public const string chartCommonGroupKey = "ChartCommonGroup";
        //���������� �������
        public const string dataManageGroupKey = "DataManageGroup";

        //������������
        //������� ��������
        public const string typesGalleryKey = "ChartTypesGallery";
        //��������� ��� ��������, �������������� 3D ��� ���������
        public const string chart3DTransformContainerKey = "3DTransform";
        //��������� ��� ��������, �������������� �������
        public const string chartLegendEditorContainerKey = "ChartLegendEditorContainer";
        //��������� ��� ��������, �������������� �������� ����������� ��������
        public const string structuralChartEditorContainerKey = "StructuralChartEditorContainer";
        //�������� ����
        public const string chartAxisEditorContainerKey = "ChartAxisEditorContainer";
        //������� ������
        public const string chartTextKey = "ChartText";
        //��� ���������
        public const string axesSettingsKey = "AxesSettings";
        //������ ��������� �������� ������
        public const string chartTextCollection1Key = "ChartTextCollection1";
        //������ ��������� �������� ������
        public const string chartTextCollection2Key = "ChartTextCollection2";
        //���������� ��������� ���
        public const string axisSortTypeKey = "AxisSortType";
        //���������� �� ������ ���������
        public const string isSortByNameKey = "IsSortByName";
        //�������� ������� ���������
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
        //��� ��������� ����� ������� ������ ����� ������� ��������
        private string befoEditChartType = string.Empty;
        //���� true - �� ������� ��������� �� ����������� 
        private bool isMayHook = false;

        #region ����������� ���������
        /// <summary>
        /// ������� �� ����������
        /// </summary>
        private RibbonTab chartPropertiesTab;
        /// <summary>
        /// ������� ����
        /// </summary>
        private RibbonTab chartRowTab;
        /// <summary>
        /// ������� ���������
        /// </summary>
        private RibbonTab chartColumnTab;

        /// <summary>
        /// ������ ������� 3D ��������
        /// </summary>
        private RibbonGroup chart3DTransformGroup;
        /// <summary>
        /// ������ ������� ����������� ��������
        /// </summary>
        private RibbonGroup structuralChartGroup;
        /// <summary>
        /// ������ ������� ���� ���������
        /// </summary>
        private RibbonGroup chartAxisGroup;
        /// <summary>
        /// ������ ����� ��������
        /// </summary>
        private RibbonGroup chartTypesGroup;
        /// <summary>
        /// ������ ����� ������� ��������
        /// </summary>
        private RibbonGroup chartCommonGroup;
        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        private RibbonGroup rowDataManageGroup;
        /// <summary>
        /// ���������� ������� ��������
        /// </summary>
        private RibbonGroup columnDataManageGroup;

        /// <summary>
        /// ������� �������
        /// </summary>
        private PopupGalleryTool chartGallery;
        /// <summary>
        /// ��������� ��� ��������� 3D ���������
        /// </summary>
        private ControlContainerTool chart3DTransformContainer;
        /// <summary>
        /// �������� 3D ��������
        /// </summary>
        private Chart3DTransform chart3DTransform;
        /// <summary>
        /// ��������� ��� ��������� ������� ���������
        /// </summary>
        private ControlContainerTool chartLegendEditorContainer;
        /// <summary>
        /// �������� ������� ���������
        /// </summary>
        private ChartLegendEditor chartLegendEditor;
        /// <summary>
        /// ��������� ��� ��������� ������� ����������� ��������
        /// </summary>
        private ControlContainerTool structuralChartEditorContainer;
        /// <summary>
        /// �������� ������� ����������� ��������
        /// </summary>
        private StructuralChartEditor structuralChartEditor;
        /// <summary>
        /// ��������� ��� ��������� ���� ���������
        /// </summary>
        private ControlContainerTool chartAxisEditorContainer;
        /// <summary>
        /// �������� ���� ���������
        /// </summary>
        private ChartAxisEditor chartAxisEditor;
        /// <summary>
        /// ������� ������
        /// </summary>
        private PopupGalleryTool chartText;
        /// <summary>
        /// ��� ���������
        /// </summary>
        private ButtonTool axesSettings;
        /// <summary>
        /// ��������� �������� ������ 1
        /// </summary>
        private ButtonTool chartTextCollection1;
        /// <summary>
        /// ��������� �������� ������ 2
        /// </summary>
        private ButtonTool chartTextCollection2;
        /// <summary>
        /// ���������� �� ������
        /// </summary>
        private StateButtonTool isSortByName;
        /// <summary>
        /// �������� �������
        /// </summary>
        private StateButtonTool reverseOrder;
        /// <summary>
        /// ���������� ��������� ���
        /// </summary>
        private ComboBoxTool axisSortType;



        #endregion

        public ChartToolBar(ToolbarsManage toolbarManager)
        {
            this.ToolbarsManager = toolbarManager;
        }

        public void Initialize()
        {
            //������������� ������ �� �������� ���������
            this.SetLinkOnTools();
            this.InitializeChartGalleryTool();
            //������������� ��������� ���������, ��������� �����������
            this.SetEventHandlers();
            //������������� ���������� ������� ������� �����
            this.SetRusTabCaption();
        }

        /// <summary>
        /// ������������� ������ �� �������� ���������
        /// </summary>
        private void SetLinkOnTools()
        {
            //�������
            this.chartPropertiesTab = this.Toolbars.Ribbon.Tabs[propertiesTabKey];
            this.chartRowTab = this.Toolbars.Ribbon.Tabs[chartRowTabKey];
            this.chartColumnTab = this.Toolbars.Ribbon.Tabs[chartColumnTabKey];
            //������
            this.chart3DTransformGroup = this.chartPropertiesTab.Groups[chart3DTransformGroupKey];
            this.structuralChartGroup = this.chartPropertiesTab.Groups[structuralChartGroupKey];
            this.chartAxisGroup = this.chartPropertiesTab.Groups[chartAxisGroupKey];
            this.chartTypesGroup = this.chartPropertiesTab.Groups[typesGalleryGroupKey];
            this.chartCommonGroup = this.chartPropertiesTab.Groups[chartCommonGroupKey];
            this.rowDataManageGroup = this.chartRowTab.Groups[dataManageGroupKey];
            this.columnDataManageGroup = this.chartColumnTab.Groups[dataManageGroupKey];

            //��������
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
        /// ���������� ����������� �������
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
        /// ������������� ���������� ������� ������� �����, ��������� ������, �� ��� ����� 
        /// ����� �������� �� �����
        /// </summary>
        private void SetRusTabCaption()
        {
            this.chartPropertiesTab.Caption = "��������";
            this.chartRowTab.Caption = "����";
            this.chartColumnTab.Caption = "���������";
        }

        /// <summary>
        /// ������ ��� ���������� �������
        /// </summary>
        /// <returns></returns>
        private SelectedTabType GetSelectedTab()
        {
            SelectedTabType result = SelectedTabType.None;
            if (this.Toolbars.Ribbon.SelectedTab == null)
                return result;

            switch (this.Toolbars.Ribbon.SelectedTab.Key)
            {
                //������
                case chartRowTabKey:
                    result = SelectedTabType.Row;
                    break;
                //�������
                case chartColumnTabKey:
                    result = SelectedTabType.Column;
                    break;
            }
            return result;
        }


        /// <summary>
        /// ��������� �������� ��������� �� �������� ���������
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
        /// ����������� �������� �� ������� �������� ���������
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
        /// ��������� ������� �� ���������� �����
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshRowTab(ChartReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Row);
        }

        /// <summary>
        /// ��������� ������� �� ���������� ���������
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshColumnTab(ChartReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Column);
        }

        /// <summary>
        /// ��������� �������� ����� ������� ����
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
        /// ������������� ��������� ������ � ����������� �� ���� ���������, ��� �� �������������� ��������
        /// ����������� �� ���
        /// </summary>
        /// <param name="activeChart"></param>
        private void Refresh3DTransformGroup(UltraChart activeChart)
        {
            if (activeChart == null)
                return;

            //���������� ������ ������ ��� 3D ��������
            if (Common.InfragisticsUtils.Is3DChart(activeChart))
            {
                this.chart3DTransformGroup.Visible = true;
                this.chart3DTransform.Chart = activeChart;
            }
            else
                this.chart3DTransformGroup.Visible = false;
        }

        /// <summary>
        /// ������������� ��������� ������, ������ ��� ����������� ��������, � �������������� 
        /// ������� �� ���
        /// </summary>
        /// <param name="activeChart"></param>
        private void RefreshStructuralChartGroup(UltraChart activeChart)
        {
            if (activeChart == null)
                return;

            //���������� ������ ������ ��� ������������� ��������
            if (Common.InfragisticsUtils.IsStructuralChart(activeChart))
            {
                this.structuralChartGroup.Visible = true;
                this.structuralChartEditor.Chart = activeChart;
            }
            else
                this.structuralChartGroup.Visible = false;
        }

        /// <summary>
        /// ������������� ��������� ������, ������ ��� �������� c �����, � �������������� 
        /// ������� �� ���
        /// </summary>
        /// <param name="activeChart"></param>
        private void RefreshAxisGroup(UltraChart activeChart)
        {
            if (activeChart == null)
                return;

            //���������� ������ ������ ��� �������� ������� ���, 
            //BubbleChart3D � PoinChart3D - ���� ��� � �����, �� �� ��������� �� ������� �������
            //������ ����� ���� Infragistics, ���� ��� ��� ���� ��������
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
            // ���� �������� ������ ����� ������� ��� ����������� ��������
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
        /// ��������� ��������� ������ ������������, ����������� � XML
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
                FormException.ShowErrorForm(new Exception("��� �������� ������ � ������ ������������ ���������, ��������� ������"));
            }
        }

        /// <summary>
        /// ��������� �������� �������
        /// </summary>
        private void LoadTabProperties(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return;
            //���� ���� ������� �������
            Color propertiesTabBackColor = ToolbarUtils.GetTabColor(xmlNode, ToolbarUtils.propertiesTabNodeName);
            if (propertiesTabBackColor != Color.Empty)
            {
                //�������� 3D ��������
                this.chart3DTransform.BackColor = propertiesTabBackColor;
                //�������� �������
                this.chartLegendEditor.BackColor = propertiesTabBackColor;
                //�������� ������������� ��������
                this.structuralChartEditor.BackColor = propertiesTabBackColor;
                //�������� ���� ���������
                this.chartAxisEditor.BackColor = propertiesTabBackColor;
            }
        }

        /// <summary>
        /// ������ ���������� ����� ��������� ����� ������� �������� ������
        /// </summary>
        private void AfterEdited()
        {
            this.MainForm.PropertyGrid.Refresh();
            this.MainForm.Saved = false;
        }

        #region �����������
        void Toolbars_AfterRibbonTabSelected(object sender, RibbonTabEventArgs e)
        {
            RefreshTabsTools(this.ActiveChartElement);
        }

        /// <summary>
        /// ������� �� ������� � �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChartGallery_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            ChartType newChartType = (ChartType)Enum.Parse(typeof(ChartType), this.GetActiveChartType());
            // ���� ��������� �������� �������� � ����� ��� �����������
            if (MainForm.IsCompositeChildChart(this.ActiveChartElement.UniqueName) &&
                !CompositeChartUtils.IsCompositeCompatibleType(newChartType))
            {
                ChartType curChartType = (ChartType)Enum.Parse(typeof(ChartType), this.befoEditChartType);
                
                string elementText = MainForm.GetReportElementText(this.ActiveChartElement.UniqueName);
                string curChartTypeText = ChartTypeConverter.GetLocalizedChartType(curChartType);
                string newChartTypeText = ChartTypeConverter.GetLocalizedChartType(newChartType);
                string msg = string.Format("��������� \"{0}\" ������������ � �������� ���� ����������� ���������. " +
                                           "��� \"{1}\" �������� ������������� ��� ����������� ���������. ������� ���� \"{0}({2})\" �� ����������� ���������?",
                                           elementText, newChartTypeText, curChartTypeText);
                if (MessageBox.Show(msg, "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // ��������������� �������� ���
                    this.SetActiveChartType(this.befoEditChartType);
                    this.befoEditChartType = string.Empty;
                    return;
                }
                else
                {
                    // ������� �������������
                    msg = string.Format("�� ������������� ������ ������� ���� \"{0}({1})\" �� ����������� ���������?",
                        elementText, curChartTypeText);
                    if (MessageBox.Show(msg, "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {                    
                        // ��������������� �������� ���
                        this.SetActiveChartType(this.befoEditChartType);
                        this.befoEditChartType = string.Empty;
                        return;
                    }
                    else
                    {
                        // ������� ��� ����, ���� ������� ��������
                        MainForm.RemoveCompositeChildKey(this.ActiveChartElement.UniqueName);
                    }
                }
            }
            
            //�.�. ����� ���������� � ������� �������� ��������, ������ ��� ��������� ������� 
            //������� ���� ������ ��� ��� ����������� � ��������, ����� ���� �������� ��������
            //befoEditChartType ��� ����, ��� �� ��� ������� ��������� ����� ����������� �������
            this.befoEditChartType = string.Empty;
            this.RefreshCommonTab(this.ActiveChartElement);
            this.AfterEdited();
        }

        /// <summary>
        /// ��������� ���������� �������� � �������
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
                //������� �������� �������� ������ ��� ������� ���� ���������
                ChartTextCollection[] chartTexts = Common.InfragisticsUtils.GetChartTextCollection(this.ActiveChart);
                //���� �������� ����, �� �� ����� ����������
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
                        this.chartTextCollection1.SharedProps.Caption = "�������";
                    }
                    if (this.ActiveChart.ChartType == ChartType.ScatterLineChart)
                    {
                        this.chartTextCollection1.SharedProps.Caption = "�����";
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
                //�� ���� ����� �������� ��� �� ������������ ������� ���� �� "����������"
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

        #region ��������������� ������ ��� ��������� ���������� �������� ���������
        /// <summary>
        /// ���� � ��������� � �������� �������� ������ ���������� ���� ���� ������� 
        /// ������� � ������ ������������ ������, ������ ������� �������
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
        /// ���������� �� � �������� ���� ���� ������� ������� � ������ ������������ ������
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
        /// ���������� � ��������� �������� �������� � ������ ������������ ������
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
        /// ������ ��������� � �������� ��������, �������� � ������ ������������ ������
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
        /// ���������� ��� ��������� ��������� �������� ������
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
        /// �������� ��� ��������� � ��������� �������� ������
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
        /// ��������� ���� ���������� � ������� ��������
        /// </summary>
        /// <param name="e"></param>
        private void ActivateChartType(GalleryToolItemEventArgs e)
        {
            if (e.Item != null)
            {
                               
                //���� ��������� �������� ��������� �������, �������� �������������� ���
                //���������
                if (this.befoEditChartType == string.Empty)
                {
                    this.befoEditChartType = this.GetActiveChartType();
                }

                //��������� ����� ��� ���������
                this.SetActiveChartType(e.Item.Key);
            }
            else
            {
                //���� �������� ��� ����������� �������� ��������, ���� ������ �� �������,
                //������ � befoEditChartType ������ ����������� ��� ��������� ���������� �����
                //������� ������ ��� � ����������, ���� ����� ��� ���� ��� ������
                //(������� �� ���������) �� ������ �� ����������, �.�. ��� �������� ���������
                this.SetActiveChartType(this.befoEditChartType);
                this.befoEditChartType = string.Empty;
            }
        }

        /// <summary>
        /// ����������� ��� ���������� ��������� � ����� 
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
        /// ����������� ��� ���������� ��������� � ������� 
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
        /// ������������ ������ ��������
        /// </summary>
        /// <param name="groupName">���� ��������</param>
        /// <returns>������� ��������</returns>
        private string GetLocalizedChartGroup(string groupName)
        {
            switch (groupName)
            {
                case "Area":
                    return "� ���������";
                case "Bar":
                    return "����������";
                case "Bubble":
                    return "�����������";
                case "Candle":
                    return "��������";
                case "Column":
                    return "�����������";
                case "Doughnut":
                    return "���������";
                case "Heat":
                    return "�����������";
                case "Line":
                    return "������";
                case "Misc":
                    return "������";
                case "Pie":
                    return "��������";
                case "Pyramid":
                    return "�������������";
                case "Radar":
                    return "�����������";
                case "Point":
                    return "��������";
                default:
                    return groupName;

            }
        }


        #endregion 

        #region ��������
        public ToolbarsManage ToolbarsManager
        {
            get { return _toolbarsManager; }
            set { _toolbarsManager = value; }
        }

        /// <summary>
        /// ������ �� ������� �����
        /// </summary>
        public MainForm MainForm
        {
            get { return this.ToolbarsManager.MainForm; }
            set { this.ToolbarsManager.MainForm = value; }
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        public UltraChart ActiveChart
        {
            get { return this.IsExistActiveChartElement ? this.ActiveChartElement.Chart : null; }
        }

        /// <summary>
        /// ���������� �� �������� ������� ������ � ����������
        /// </summary>
        public bool IsExistActiveChartElement
        {
            get { return this.ActiveChartElement != null; }
        }

        /// <summary>
        /// �������� ������� ������ � ���������� �� �������� ��������������� ������
        /// </summary>
        public ChartReportElement ActiveChartElement
        {
            get { return _activeChartElement; }
            set { _activeChartElement = value; }
        }

        /// <summary>
        /// �����
        /// </summary>
        public UltraToolbarsManager Toolbars
        {
            get { return this.ToolbarsManager.Toolbars; }
            set { this.ToolbarsManager.Toolbars = value; }
        }
        #endregion

        #region �������������� ������

        /// <summary>
        /// ��� ������ ���������
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
