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
    /// Ключи элементов в панэли инструментов, относящихся к карте
    /// </summary>
    public class MapToolbar
    {
        #region Ключи
        //вкладок
        //вкладки свойств карты
        public const string propertiesTabKey = "MapPropertiesTab";

        //групп
        //общие свойства карты
        public const string mapCommonGroupKey = "MapCommonGroup";

        //инструментов
        //Выбор шаблона карты
        public const string mapTemplateKey = "MapTemplate";


        #endregion

        private MapReportElement _activeMapElement;
        private ToolbarsManage _toolbarsManager;

        //Если true - то события контролов не выполняются 
        private bool isMayHook = false;

        #region инструменты карты
        /// <summary>
        /// вкладка со свойствами
        /// </summary>
        private RibbonTab mapPropertiesTab;

        /// <summary>
        /// группа общих свойств диаграмм
        /// </summary>
        private RibbonGroup mapCommonGroup;

        /// <summary>
        /// Шаблон карты
        /// </summary>
        private ButtonTool mapTemplate;

        #endregion

        public MapToolbar(ToolbarsManage toolbarManager)
        {
            this.ToolbarsManager = toolbarManager;
        }

        public void Initialize()
        {
            //Устанавливаем ссылки на контролы карты
            this.SetLinkOnTools();
            //Устанавливаем контролам диаграммы, требуемые обработчики
            this.SetEventHandlers();
            //Устанавливаем заголовкам вкладок русские имена
            this.SetRusTabCaption();
        }

        /// <summary>
        /// Устанавливаем ссылки на контролы карты
        /// </summary>
        private void SetLinkOnTools()
        {
            //Вкладки
            this.mapPropertiesTab = this.Toolbars.Ribbon.Tabs[propertiesTabKey];
            //Группы
            this.mapCommonGroup = this.mapPropertiesTab.Groups[mapCommonGroupKey];

            //Контролы
            this.mapTemplate = (ButtonTool)this.Toolbars.Tools[mapTemplateKey];

        }


        /// <summary>
        /// Установить обработчики событий
        /// </summary>
        private void SetEventHandlers()
        {
            this.mapTemplate.ToolClick += new ToolClickEventHandler(mapTemplate_ToolClick);

            this.Toolbars.AfterRibbonTabSelected += new RibbonTabEventHandler(Toolbars_AfterRibbonTabSelected);

        }

        /// <summary>
        /// Устанавливаем заголовкам вкладок русские имена, непонятно почему, но они очень 
        /// часто меняются на ключи
        /// </summary>
        private void SetRusTabCaption()
        {
            this.mapPropertiesTab.Caption = "Свойства";
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
                case propertiesTabKey:
                    result = SelectedTabType.Common;
                    break;
            }
            return result;
        }


        /// <summary>
        /// Обнавляем значение контролов на вкладках карты
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
        /// Обновляются контролы на вкладке Свойства карты
        /// </summary>
        /// <param name="activeElement"></param>
        private void RefreshCommonTab(MapReportElement activeElement)
        {
            MapControl activeChart = activeElement.Map;
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
        /// Активная карта
        /// </summary>
        public MapControl ActiveMap
        {
            get { return this.IsExistActiveMapElement ? this.ActiveMapElement.Map : null; }
        }

        /// <summary>
        /// Существует ли активный элемент отчета с диаграммой
        /// </summary>
        public bool IsExistActiveMapElement
        {
            get { return this.ActiveMapElement != null; }
        }

        /// <summary>
        /// Активный элемент отчета с картой по которому инициализирован тулбар
        /// </summary>
        public MapReportElement ActiveMapElement
        {
            get { return _activeMapElement; }
            set { _activeMapElement = value; }
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
