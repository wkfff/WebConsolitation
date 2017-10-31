using System;
using Dundas.Maps.WinControl;
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
    /// ����� ��������� � ������ ������������, ����������� � �����
    /// </summary>
    public class MapToolbar
    {
        #region �����
        //�������
        //������� ������� �����
        public const string propertiesTabKey = "MapPropertiesTab";

        //�����
        //����� �������� �����
        public const string mapCommonGroupKey = "MapCommonGroup";

        //������������
        //����� ������� �����
        public const string mapTemplateKey = "MapTemplate";


        #endregion

        private MapReportElement _activeMapElement;
        private ToolbarsManage _toolbarsManager;

        //���� true - �� ������� ��������� �� ����������� 
        private bool isMayHook = false;

        #region ����������� �����
        /// <summary>
        /// ������� �� ����������
        /// </summary>
        private RibbonTab mapPropertiesTab;

        /// <summary>
        /// ������ ����� ������� ��������
        /// </summary>
        private RibbonGroup mapCommonGroup;

        /// <summary>
        /// ������ �����
        /// </summary>
        private ButtonTool mapTemplate;

        #endregion

        public MapToolbar(ToolbarsManage toolbarManager)
        {
            this.ToolbarsManager = toolbarManager;
        }

        public void Initialize()
        {
            //������������� ������ �� �������� �����
            this.SetLinkOnTools();
            //������������� ��������� ���������, ��������� �����������
            this.SetEventHandlers();
            //������������� ���������� ������� ������� �����
            this.SetRusTabCaption();
        }

        /// <summary>
        /// ������������� ������ �� �������� �����
        /// </summary>
        private void SetLinkOnTools()
        {
            //�������
            this.mapPropertiesTab = this.Toolbars.Ribbon.Tabs[propertiesTabKey];
            //������
            this.mapCommonGroup = this.mapPropertiesTab.Groups[mapCommonGroupKey];

            //��������
            this.mapTemplate = (ButtonTool)this.Toolbars.Tools[mapTemplateKey];

        }


        /// <summary>
        /// ���������� ����������� �������
        /// </summary>
        private void SetEventHandlers()
        {
            this.mapTemplate.ToolClick += new ToolClickEventHandler(mapTemplate_ToolClick);

            this.Toolbars.AfterRibbonTabSelected += new RibbonTabEventHandler(Toolbars_AfterRibbonTabSelected);

        }

        /// <summary>
        /// ������������� ���������� ������� ������� �����, ��������� ������, �� ��� ����� 
        /// ����� �������� �� �����
        /// </summary>
        private void SetRusTabCaption()
        {
            this.mapPropertiesTab.Caption = "��������";
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
                case propertiesTabKey:
                    result = SelectedTabType.Common;
                    break;
            }
            return result;
        }


        /// <summary>
        /// ��������� �������� ��������� �� �������� �����
        /// </summary>
        /// <param name="activeElement"></param>
        public void RefreshTabsTools(MapReportElement activeElement)
        {
            this.ActiveMapElement = activeElement;
            if ((this.ActiveMapElement != null) && (this.ActiveMapElement.PivotData != null)
                && (this.ActiveMapElement.Map != null))
            {
                try
                {
                    this.isMayHook = true;
                    this.RefreshCommonTab(this.ActiveMapElement);
                }
                finally
                {
                    this.isMayHook = false;
                }
            }
        }

        /// <summary>
        /// ����������� �������� �� ������� �������� �����
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshCommonTab(MapReportElement activeElement)
        {
            MapControl activeChart = activeElement.Map;
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
            RefreshTabsTools(this.ActiveMapElement);
        }

        void mapTemplate_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            if (this.IsExistActiveMapElement)
            {
                this.ActiveMapElement.SelectTemplateName();
                this.AfterEdited();
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
        /// �������� �����
        /// </summary>
        public MapControl ActiveMap
        {
            get { return this.IsExistActiveMapElement ? this.ActiveMapElement.Map : null; }
        }

        /// <summary>
        /// ���������� �� �������� ������� ������ � ����������
        /// </summary>
        public bool IsExistActiveMapElement
        {
            get { return this.ActiveMapElement != null; }
        }

        /// <summary>
        /// �������� ������� ������ � ������ �� �������� ��������������� ������
        /// </summary>
        public MapReportElement ActiveMapElement
        {
            get { return _activeMapElement; }
            set { _activeMapElement = value; }
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
