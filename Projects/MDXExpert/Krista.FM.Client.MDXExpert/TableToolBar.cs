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
    /// ����� ��������� � ������ ������������, ����������� � �������
    /// </summary>
    public class TableToolbar
    {
        #region �����
        //�������
        //������� ����� �������
        public const string rowTabKey = "TableRowTab";
        //������� ������� �������
        public const string columnTabKey = "TableColumnTab";
        //������� ��� �������
        public const string measureTabKey = "TableMeasureTab";
        //������� �������
        private const string elementTabKey = "ElementTab";

        //�����
        //���������
        public const string behaviorGroupKey = "BehaviorGroup";
        //������� ����
        public const string selectedRowElementGroupKey = "SelectedObjectGroup";
        //������� ��������
        public const string selectedColumnElementGroupKey = "SelectedObjectGroup";
        //����
        public const string selectedMeasureGroupKey = "SelectedObjectGroup";
        //���������� ����
        public const string customMeasureGroupKey = "CustomMeasureGroup";
        //���������� �������
        public const string dataManageGroupKey = "DataManageGroup";
        //�������� ��������� �� �������
        public const string elementsByTableGroupKey = "ElementsByTableGroup";
        //������� �������
        public const string analysisFunctionsGroupKey = "AnalysisFunctionsGroup";


        //������������
        //����� ��� ���� ��������� �������
        public const string axisGrandTotalVisibleKey = "AxisGrandTotalVisible";
        public const string axisHideEmptyPositionsKey = "AxisHideEmptyPositions";
        public const string axisPropertiesDisplayTypeKey = "AxisPropertiesDisplayType";
        public const string captionStyleGalleryKey = "TableCaptionStyleGallery";
        public const string cellStyleGalleryKey = "TableCellStyleGallery";
        public const string memberPropertiesEditorContainerKey = "MemberPropertiesContainer";
        public const string axisSortTypeKey = "AxisSortType";

        public const string createChartByTableKey = "CreateChartByTable";
        public const string createMapByTableKey = "CreateMapByTable";

        //����
        public const string totalStyleGalleryKey = "TableTotalStyleGallery";
        public const string measureEditorContainerKey = "MeasureEditorContainer";
        public const string addCalcTotalKey = "AddCalcTotal";
        public const string deleteCalcTotalKey = "DeleteCalcTotal";
        public const string measureFormulaKey = "MeasureFormula";
        public const string colorRulesKey = "ColorRules";

        //������
        public const string autoSizeRowsKey = "AutoSizeRows";
        public const string calcAverageKey = "CalcAverage";
        public const string calcMedianKey = "CalcMedian";
        public const string calcTopCountKey = "CalcTopCount";
        public const string calcBottomCountKey = "CalcBottomCount";
        #endregion

        #region �������� �������
        //������� �������
        /// <summary>
        /// ������� �� ���������� �����
        /// </summary>
        private RibbonTab rowTab;
        /// <summary>
        /// ������� �� ���������� �������
        /// </summary>
        private RibbonTab columnTab;
        /// <summary>
        /// ������� �� ���������� ���
        /// </summary>
        private RibbonTab measureTab;
        /// <summary>
        /// ������� �������
        /// </summary>
        private RibbonTab elementTab;

        //������
        /// <summary>
        /// ��������� �����
        /// </summary>
        private RibbonGroup rowBehaviorGroup;
        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        private RibbonGroup selectedRowElementGroup;
        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        private RibbonGroup rowDataManageGroup;
        /// <summary>
        /// ��������� ��������
        /// </summary>
        private RibbonGroup columnBehaviorGroup;
        /// <summary>
        /// ���������� ������� ��������
        /// </summary>
        private RibbonGroup selectedColumnElementGroup;
        /// <summary>
        /// ���������� ������� ��������
        /// </summary>
        private RibbonGroup columnDataManageGroup;
        /// <summary>
        /// ���������� ����
        /// </summary>
        private RibbonGroup selectedMeasureGroup;
        /// <summary>
        /// ��������������� ����
        /// </summary>
        private RibbonGroup customMeasureGroup;
        /// <summary>
        /// �������� ��������� �� �������
        /// </summary>
        private RibbonGroup elementsByTableGroup;
        /// <summary>
        /// ������� �������
        /// </summary>
        private RibbonGroup analysisFunctionsGroup;

        //������
        /// <summary>
        /// �������������� ������ ������ ������ � �����
        /// </summary>
        private StateButtonTool autoSizeRows;
        /// <summary>
        /// ������ ��������
        /// </summary>
        private ButtonTool calcAverage;
        /// <summary>
        /// ������ �������
        /// </summary>
        private ButtonTool calcMedian;
        /// <summary>
        /// ������ k-������
        /// </summary>
        private ButtonTool calcTopCount;
        /// <summary>
        /// ������ k-���������
        /// </summary>
        private ButtonTool calcBottomCount;

        //�����
        /// <summary>
        /// ���������� ������� ���� ���
        /// </summary>
        private StateButtonTool axisGrandTotalVisible;
        /// <summary>
        /// �������� ������ �������� ���
        /// </summary>
        private StateButtonTool axisHideEmptyPositions;
        /// <summary>
        /// ����� ����������� ������� ��������� ���
        /// </summary>
        private ComboBoxTool axisPropertiesDisplayType;
        /// <summary>
        /// ���������� ��������� ���
        /// </summary>
        private ComboBoxTool axisSortType;
        /// <summary>
        /// ������� ������ ��� ����������
        /// </summary>
        private PopupGalleryTool captionStyleGallery;
        /// <summary>
        /// ������� ������ ��� �����
        /// </summary>
        private PopupGalleryTool cellStyleGallery;
        /// <summary>
        /// �������� ��������� �� �������
        /// </summary>
        private ButtonTool createChartByTable;
        /// <summary>
        /// �������� ����� �� �������
        /// </summary>
        private ButtonTool createMapByTable;

        //����
        /// <summary>
        /// ������� ������ ������ � ���
        /// </summary>
        private PopupGalleryTool totalStyleGallery;
        /// <summary>
        /// ���������, ��� �������������� ����
        /// </summary>
        private ControlContainerTool measureEditorContainer;
        /// <summary>
        /// �������, ������������� ���������� ����
        /// </summary>
        private MeasureEditor measureEditor;
        /// <summary>
        /// ���������, ��� �������������� ������� ��������
        /// </summary>
        private PopupControlContainerTool memberPropertiesContainer;
        /// <summary>
        /// �������� ������� ���������
        /// </summary>
        private MemberPropertiesControl memberPropertiesEditor;
        /// <summary>
        /// �������� ����������� ����������
        /// </summary>
        private ButtonTool addCalcTotal;
        /// <summary>
        /// ������� ����������� ����������
        /// </summary>
        private ButtonTool deleteCalcTotal;
        /// <summary>
        /// ������������� ������� ���������� ����
        /// </summary>
        private ButtonTool measureFormula;
        /// <summary>
        /// �������� ���������
        /// </summary>
        private ButtonTool colorRules;

        #endregion

        //���������� ��� �������� ���������� ����, ��������� not Null ���������� ��������
        private string lastNotNullSelectedLevel = string.Empty;
        private string lastNotNullSelectedMeasure = string.Empty;

        private TableReportElement _activeTableElement;
        private ToolbarsManage _toolbarsManager;
        private ActiveGalleryType _activeGallery;
        //����� �������� ������� �� ��������������
        private CellStyle befoEditStyle = null;
        //���� true - �� ������� ��������� �� ����������� 
        private bool isMayHook = false;

        public TableToolbar(ToolbarsManage toolbarManager)
        {
            this.ToolbarsManager = toolbarManager;
        }

        public void Initialize()
        {
            //������������� ������ �� �������� �������
            this.SetLinkOnTools();
            //������������� ���������� ������� ������� �����
            this.SetRusTabCaption();
            //������������� ��������� �������, ��������� �����������
            this.SetEventHandlers();
            //������ ��� ������, ������� ���������� �������� ����������� �������
            this.RefreshSelectedGroups(null);
        }

        /// <summary>
        /// ������������� ������ �� �������� �������
        /// </summary>
        private void SetLinkOnTools()
        {
            //�������
            //������ �������
            this.rowTab = this.Toolbars.Ribbon.Tabs[rowTabKey];
            //������� �������
            this.columnTab = this.Toolbars.Ribbon.Tabs[columnTabKey];
            //���� �������
            this.measureTab = this.Toolbars.Ribbon.Tabs[measureTabKey];
            //�������
            this.elementTab = this.Toolbars.Ribbon.Tabs[elementTabKey];
            
            //������
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

            //�����������
            //�������� ��������� �� �������
            this.createChartByTable = (ButtonTool)this.Toolbars.Tools[createChartByTableKey];
            this.createMapByTable = (ButtonTool)this.Toolbars.Tools[createMapByTableKey];

            //������
            this.autoSizeRows = (StateButtonTool)this.Toolbars.Tools[autoSizeRowsKey];
            this.calcAverage = (ButtonTool)this.Toolbars.Tools[calcAverageKey];
            this.calcMedian = (ButtonTool)this.Toolbars.Tools[calcMedianKey];
            this.calcTopCount = (ButtonTool)this.Toolbars.Tools[calcTopCountKey];
            this.calcBottomCount = (ButtonTool)this.Toolbars.Tools[calcBottomCountKey];
            //�����
            this.axisGrandTotalVisible = (StateButtonTool)this.Toolbars.Tools[axisGrandTotalVisibleKey];
            this.axisHideEmptyPositions = (StateButtonTool)this.Toolbars.Tools[axisHideEmptyPositionsKey];
            this.axisPropertiesDisplayType = (ComboBoxTool)this.Toolbars.Tools[axisPropertiesDisplayTypeKey];
            this.captionStyleGallery = (PopupGalleryTool)this.Toolbars.Tools[captionStyleGalleryKey];
            this.cellStyleGallery = (PopupGalleryTool)this.Toolbars.Tools[cellStyleGalleryKey];
            this.axisSortType = (ComboBoxTool)this.Toolbars.Tools[axisSortTypeKey];
            this.memberPropertiesContainer = (PopupControlContainerTool)this.Toolbars.Tools[memberPropertiesEditorContainerKey];
            this.memberPropertiesEditor = (MemberPropertiesControl)this.memberPropertiesContainer.Control;
            //����
            this.totalStyleGallery = (PopupGalleryTool)this.Toolbars.Tools[totalStyleGalleryKey];
            this.measureEditorContainer = (ControlContainerTool)this.Toolbars.Tools[measureEditorContainerKey];
            this.measureEditor = (MeasureEditor)this.measureEditorContainer.Control;
            this.addCalcTotal = (ButtonTool)this.Toolbars.Tools[addCalcTotalKey];
            this.deleteCalcTotal = (ButtonTool)this.Toolbars.Tools[deleteCalcTotalKey];
            this.measureFormula = (ButtonTool)this.Toolbars.Tools[measureFormulaKey];
            this.colorRules = (ButtonTool)this.Toolbars.Tools[colorRulesKey];
        }

        /// <summary>
        /// ���������� ����������� �������
        /// </summary>
        private void SetEventHandlers()
        {
            //��������
            this.Toolbars.AfterRibbonTabSelected += new RibbonTabEventHandler(Toolbars_AfterRibbonTabSelected);
            this.Toolbars.AfterRibbonTabDropDown += new RibbonTabEventHandler(Toolbars_AfterRibbonTabDropDown);

            //������
            this.autoSizeRows.ToolClick += new ToolClickEventHandler(autoSizeRows_ToolClick);
            this.calcAverage.ToolClick += new ToolClickEventHandler(calcAverage_ToolClick);
            this.calcMedian.ToolClick += new ToolClickEventHandler(calcMedian_ToolClick);
            this.calcTopCount.ToolClick += new ToolClickEventHandler(calcTopCount_ToolClick);
            this.calcBottomCount.ToolClick += new ToolClickEventHandler(calcBottomCount_ToolClick);


            //�����
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
            //����
            this.totalStyleGallery.GalleryToolActiveItemChange += new GalleryToolItemEventHandler(totalStyleGallery_GalleryToolActiveItemChange);
            this.totalStyleGallery.GalleryToolItemClick += new GalleryToolItemEventHandler(totalStyleGallery_GalleryToolItemClick);
            this.measureEditor.ExitEditMode += new EventHandler(measureEditor_ExitEditMode);
            this.addCalcTotal.ToolClick += new ToolClickEventHandler(addCalcTotal_ToolClick);
            this.deleteCalcTotal.ToolClick += new ToolClickEventHandler(deleteCalcTotal_ToolClick);
            this.measureFormula.ToolClick += new ToolClickEventHandler(measureFormula_ToolClick);
            this.colorRules.ToolClick += new ToolClickEventHandler(colorRules_ToolClick);
        }



        /// <summary>
        /// ������������� ���������� ������� ������� �����, ��������� ������, �� ��� ����� 
        /// ����� �������� �� �����
        /// </summary>
        private void SetRusTabCaption()
        {
            this.rowTab.Caption = "������";
            this.columnTab.Caption = "�������";
            this.measureTab.Caption = "����";
        }

        /// <summary>
        /// ��������� �������� ��������� � ����������� � ������� �������� �������
        /// </summary>
        /// <param name="activeElement"></param>
        public void RefreshTabsTools(TableReportElement activeElement)
        {
            //������ ������������� � ������� ��������� �������� ������
            this.RemoveHandlers(this.ActiveTableElement);

            this.ActiveTableElement = activeElement;

            //������� ����������� ������� �������� ������
            this.AddHandlers(this.ActiveTableElement);

            this.DoRefreshTabsTools();
        }

        /// <summary>
        /// ��������� �������� ��������� �� ���������� ������� �������
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
                        //������
                        case SelectedTabType.Row:
                            this.RefreshRowTab(this.ActiveTableElement);
                            break;
                        //�������
                        case SelectedTabType.Column:
                            this.RefreshColumnTab(this.ActiveTableElement);
                            break;
                        //����
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

        #region ���������� �����
        private void RefreshGroups(TableReportElement activeElement)
        {
            //���� � �������� ���������������� MDX, ��������� ��������� ��������� �����
            bool isCustomMdx = ((activeElement != null) && activeElement.PivotData.IsCustomMDX);
            this.rowBehaviorGroup.Visible = !isCustomMdx;
            this.columnBehaviorGroup.Visible = !isCustomMdx;
            this.customMeasureGroup.Visible = !isCustomMdx;

            this.RefreshSelectedGroups(activeElement);
        }

        /// <summary>
        /// ��������� ������ ��������� �� ����������� �������
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
        /// ������������� ��������� ��������� �� ����������� ������� �������
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
                this.selectedRowElementGroup.Caption = "�������: " + field.Caption;

            if (this.selectedColumnElementGroup.Visible)
                this.selectedColumnElementGroup.Caption = "�������: " + field.Caption;

            if (this.selectedMeasureGroup.Visible)
                this.selectedMeasureGroup.Caption = "����: " + measure.Caption;

            this.SetSelectedButtonsVisible(activeElement);
        }

        /// <summary>
        /// ������������� ��������� ��������� �� ����������� ������� ������
        /// </summary>
        /// <param name="tableReportElement"></param>
        private void SetSelectedButtonsVisible(TableReportElement tableReportElement)
        {
            PivotTotal measure = this.GetNotNullSelectedMeasure(tableReportElement);
            this.deleteCalcTotal.SharedProps.Visible = ((measure != null) && (measure.IsCustomTotal));
            this.measureFormula.SharedProps.Visible = ((measure != null) && (measure.IsCustomTotal));
        }

        /// <summary>
        /// ������������� ������ ���������� ������
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
        /// ������� ����������� � ������
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
        /// ��������� ������� �� ���������� �����
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshRowTab(TableReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshGroups(activeElement);
            this.autoSizeRows.Checked = activeElement.GridUserInterface.AutoSizeRows;
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Row);

            //�������� ������ ��������, �.�. ��� ������� �� ���� �������������� �������
            this.captionStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 65;
            this.cellStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 70;
            this.captionStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 34;
            this.cellStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 35;
        }

        /// <summary>
        /// ��������� ������� �� ���������� �������
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshColumnTab(TableReportElement activeElement)
        {
            if (activeElement == null)
                return;
            this.RefreshGroups(activeElement);
            this.RefreshAxisCommonProperties(activeElement, SelectedTabType.Column);

            //�������� ������ ��������, �.�. ��� ������� �� ���� �������������� �������
            this.captionStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 66;
            this.cellStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 69;
            this.captionStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 32;
            this.cellStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 33;
        }

        /// <summary>
        /// ���������� ������� �� ���������� ���
        /// </summary>
        /// <param name="tableReportElement"></param>
        private void RefreshMeasureTab(TableReportElement tableReportElement)
        {
            this.RefreshGroups(tableReportElement);

            //�������� ������ ��������, �.�. ��� ������� �� ���� �������������� �������
            this.captionStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 71;
            this.cellStyleGallery.SharedProps.AppearancesLarge.Appearance.Image = 63;
            this.captionStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 36;
            this.cellStyleGallery.SharedProps.AppearancesSmall.Appearance.Image = 45;
        }

        /// <summary>
        /// ��������� �������� ����� ������� ����
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
                case rowTabKey:
                    result = SelectedTabType.Row;
                    break;
                //�������
                case columnTabKey:
                    result = SelectedTabType.Column;
                    break;
                //����
                case measureTabKey:
                    result = SelectedTabType.Measure;
                    break;
            }
            return result;
        }

        /// <summary>
        /// ������� ������� ���������� ����
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
        /// �������������� ������ ������������ ��������� ������ ����������� � XML
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
                FormException.ShowErrorForm(new Exception("��� �������� ������ � ������ ������������ �������, ��������� ������"));
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
                //�������� ����
                this.measureEditor.BackColor = propertiesTabBackColor;
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

        /// <summary>
        /// �������� �������� �� ������� �������
        /// </summary>
        /// <param name="elemType">��� ������������ ��������</param>
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
                        // ���������� ������ � DockManager
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
                        // ���������� ������ � DockManager
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


        #region ����������� �������

        //���� �� �������
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

            //������� ������������ ��������
            this.RefreshSelectedGroups(this.ActiveTableElement);
        }

        //������
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


        //�����
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
            //���� �������� ������� �������� �� �����, �� ��������� �������� ������������ ���� ��������
            if (!this.memberPropertiesEditor.Visible)
                this.memberPropertiesContainer.ClosePopup();
        }

        void memberPropertiesContainer_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            //����� ������� ������� ���������, ����������������� ��
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

        #region ��������������� ������ ��� ��������� ���������� �������� ���������

        /// <summary>
        /// ����������� ��� ����������� ������� ��������� � ����� 
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
        /// ����������� ��� ����������� ������� ��������� � ������� 
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
        /// ����������� ��� ���������� ��������� � ����� 
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
        /// ����������� ��� ���������� ��������� � ������� 
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
        /// �������� ����� �������������� �������� �������
        /// </summary>
        /// <returns></returns>
        private CellStyle GetTableEditElementStyle()
        {
            CellStyle result = null;
            if (this.IsExistActiveTableElement)
            {
                switch (this.ActiveGallery)
                {
                    //������� ������ ����������
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
                    //������� ������ �����
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
                    //������� ������ ����� ������
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
        /// ���������� ����� �������������� �������� �������
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
                    //������� ������ ����������
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
                    //������� ������ �����
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
                    //������� ������ ����� ������
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
                //���� ��������� �������� ��������� �������, �������� ����� 
                if (this.befoEditStyle == null)
                    this.befoEditStyle = this.GetTableEditElementStyle();
                //��������� ����� �����
                this.SetTableEditElementStyle((CellStyle)e.Item.Tag);
            }
            else
            {
                //���� �������� ��� ����������� �������� ������, ���� ������ �� �������,
                //������ � befoEditStyle ������ ����������� ����� ���������� �����
                //������� ������ ��� � ����������, ���� ����� ��� ���� ��� ������
                //(������� �� ���������) �� ������ �� ����������, �.�. ��� �������� ���������
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

            //�������� ���������� �������
            PivotField level = this.GetNotNullSelectedLevel(reportElement);

            if (level != null)
            {
                this.memberPropertiesEditor.MemberProperties = level.MemberProperties;
                this.memberPropertiesContainer.SharedProps.Enabled = 
                    level.MemberProperties.AllProperties.Count > 0;
            }
        }

        /// <summary>
        /// �������� ���������� ����, ���� � ���, �� �������� �������� lastNotNullSelectedMeasure
        /// </summary>
        /// <param name="reportElement"></param>
        /// <returns></returns>
        private PivotTotal GetNotNullSelectedMeasure(TableReportElement reportElement)
        {
            PivotTotal result = null;
            if (reportElement == null)
                return result;

            //���� ������� �������, ������ ������ ����� ������, �.�. ����� ���� ������� ������ ���� ������
            if (reportElement.PivotData.SelectedField != null)
            {
                this.lastNotNullSelectedMeasure = string.Empty;
                return result;
            }

            //�������� ���������� ����
            result = reportElement.PivotData.SelectedMeasure;

            //���� �� ���, �������� �������� lastNotNullSelectedMeasure
            if (result == null)
            {
                result = reportElement.PivotData.TotalAxis.GetPivotTotal(this.lastNotNullSelectedMeasure);
            }

            if (result != null)
            {
                //�������� ��������� �� ������� ����
                this.lastNotNullSelectedMeasure = result.UniqueName;
            }

            return result;
        }

        /// <summary>
        /// �������� ���������� �������, ���� ��� ���, �� �������� �������� lastNotNullSelectedLevel
        /// </summary>
        /// <param name="reportElement"></param>
        /// <returns></returns>
        private PivotField GetNotNullSelectedLevel(TableReportElement reportElement)
        {
            PivotField result = null;
            if (reportElement == null)
                return result;

            //���� �������� ����, ������ ������ ����� ������, �.�. ����� ���� ������� ������ ���� ������
            if (reportElement.PivotData.SelectedMeasure != null)
            {
                this.lastNotNullSelectedLevel = string.Empty;
                return result;
            }

            //�������� ���������� �������
            result = reportElement.PivotData.SelectedField;

            //���� ��� ���, �������� �������� lastNotNullSelectedLevel
            if (result == null)
            {
                result = reportElement.PivotData.GetPivotField(this.lastNotNullSelectedLevel);
            }

            if (result != null)
            {
                //�������� ��������� �� ������� �������
                this.lastNotNullSelectedLevel = result.UniqueName;
            }

            return result;
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
        /// �����
        /// </summary>
        public UltraToolbarsManager Toolbars
        {
            get { return this.ToolbarsManager.Toolbars; }
            set { this.ToolbarsManager.Toolbars = value; }
        }

        /// <summary>
        /// ��������� �������� �������
        /// </summary>
        public IGridUserInterface ActiveTableInterface
        {
            get
            {
                return this.IsExistActiveTableElement ? this.ActiveTableElement.GridUserInterface : null;
            }
        }

        /// <summary>
        /// ���������� �� �������� �������
        /// </summary>
        private bool IsExistActiveTableElement
        {
            get { return this.ActiveTableElement != null; }
        }

        /// <summary>
        /// �������� ������� � ��������, �� �������� ��������������� ������
        /// </summary>
        public TableReportElement ActiveTableElement
        {
            get { return _activeTableElement; }
            set { _activeTableElement = value; }
        }

        /// <summary>
        /// ���������� ������� �������
        /// </summary>
        private SelectedTabType SelectedTab
        {
            get { return this.GetSelectedTab(); }
            set { this.SetSelectedTab(value); }
        }

        /// <summary>
        /// ��� �������� �������
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

        #region �������������� ������

        /// <summary>
        /// ��� ������ �������
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
        /// ��� �������� �������
        /// </summary>
        enum ActiveGalleryType
        {
            //������ ���������
            CaptionStyle,
            //������ ������
            CellStyle,
            //������ ������ �����
            TotalCellStyle,
            //�����
            None
        }
        #endregion
    }
}
