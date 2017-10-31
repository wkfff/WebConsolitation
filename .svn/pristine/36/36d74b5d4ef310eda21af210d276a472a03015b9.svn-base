using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win;
using Infragistics.Win.UltraWinChart;
using Infragistics.Win.UltraWinDock;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Client.Common.Forms;
using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    public delegate void DockPanelControlEventHandler();

    public partial class DockPanelControl : UserControl
    {
        #region Поля

        private DockSettings dockSettings;

        private string styleSheet;
        private MainForm _mainForm;

        #endregion

        #region Свойства    

        public MainForm MainForm
        {
            get { return _mainForm; }
            set { _mainForm = value; }
        }

        public UltraDockManager udManager
        {
            get { return ultraDockManager; }
        }

        public string StyleSheet
        {
            get 
            { 
                return styleSheet; 
            }
            set 
            {
                styleSheet = value;
                
                if (this.udManager.DockAreas.Count > 0)
                {
                    foreach (DockableControlPane pane in this.udManager.ControlPanes)
                    {
                        CustomReportElement elem = ((CustomReportElement)pane.Control);
                        if ((elem != null) && (elem.ElementType == ReportElementType.eTable))
                        {
                            ((TableReportElement)elem).SetStyle(styleSheet);
                        }
                    }
                }
            }
        }

        #endregion

        #region события

        //при деактивации элемента
        static private DockPanelControlEventHandler paneDeactivated = null;

        [Category("Internal events")]
        [Description("Вызывается при деактивации элемента отчета")]
        public event DockPanelControlEventHandler PaneDeactivated
        {
            add { paneDeactivated += value; }
            remove { paneDeactivated -= value; }
        }

        #endregion

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public DockPanelControl()
        {
            InitializeComponent();
            dockSettings = new DockSettings();

            ultraDockManager.DefaultPaneSettings.AllowClose = DefaultableBoolean.False;
            ultraDockManager.DefaultPaneSettings.AllowFloating = DefaultableBoolean.False;

        }

        /// <summary>
        ///  Сохранение параметров DockManager
        /// </summary>
        public bool SaveDockSettings(string fileName)
        {
            return dockSettings.Save(udManager, fileName);
        }

        /// <summary>
        ///  Восстановление параметров DockManager
        /// </summary>
        public void LoadDockSettings(string fileName)
        {
            dockSettings.Load(udManager, this, fileName);
            
            // Если у DockManager'a нет областей
            if (udManager.DockAreas.Count == 0)
            {
                // то добавляем нулевую область
                Guid areaId = new Guid("00000000-0000-0000-0000-000000000000");
                DockAreaPane newDockAreaPane = NewArea(DockedLocation.DockedLeft, areaId, new Point(0, 0), new Size(100, 100),
                                                       ChildPaneStyle.VerticalSplit);
                newDockAreaPane.Key = newDockAreaPane.GetHashCode().ToString();
                udManager.DockAreas.Add(newDockAreaPane);
            }
        }

        /// <summary>
        ///  Сброс параметров DockManager
        /// </summary>
        public void ResetDockSettings()
        {
            udManager.PinAll();

            int k = udManager.ControlPanes.Count;
            for (int i = 0; i < k; i++)
            {
                DockableControlPane dcPane = udManager.ControlPanes[0];
                if (dcPane.Control != null)
                {
                    dcPane.Control.Dispose();
                }
                udManager.ControlPanes.Remove(dcPane);
            }

            udManager.DockAreas.Clear();
            dockSettings.ResetSettings();
        }


        /// <summary>
        /// Получение окна с активным элементом
        /// </summary>
        /// <returns>окно с активным элементом, если есть такой элемент, иначе - null</returns>
        public DockableControlPane GetActivePane()
        {
            if (this.udManager.ControlPanes.Count == 0)
            {
                return null;
            }

            CustomReportElement elem = null;

            foreach (DockableControlPane pane in this.udManager.ControlPanes)
            {
                elem = ((CustomReportElement)pane.Control);
                if (elem.IsActive)
                {
                    return pane;
                }
            }

            return null;
        }

        /// <summary>
        /// Вернет имя которого еще нет в коллекции элементов
        /// </summary>
        /// <param name="originalName"></param>
        /// <returns></returns>
        public string GetCloneElementName(string originalName)
        {
            int i = 2;
            string result = originalName;
            bool isExistName = true;

            //узнаем может оригинальное имя - это копия, и уже содержит число в скобках
            this.InitializationOrignalName(ref originalName, ref i);
            
            //Теперь будем искать имя которого еще нет
            while (isExistName)
            {
                result = originalName.Trim() + string.Format(" ({0})", i.ToString());
                isExistName = false;

                foreach (DockableControlPane pane in this.udManager.ControlPanes)
                {
                    CustomReportElement elem = ((CustomReportElement)pane.Control);
                    if (result == elem.Title)
                    {
                        isExistName = true;
                        break;
                    }
                }
                i++;
            }
            return result;
        }

        private void InitializationOrignalName(ref string originalName, ref int i)
        {
            int braketStart = originalName.LastIndexOf('(') + 1;
            int braketEnd = originalName.LastIndexOf(')');
            if ((braketStart > 1) && (braketEnd == originalName.Length - 1))
            {
                string inBraketValue = originalName.Substring(braketStart,
                    braketEnd - braketStart);
                int copyIndex;
                if (int.TryParse(inBraketValue, out copyIndex))
                {
                    originalName = originalName.Remove(braketStart - 1);
                    i = copyIndex + 1;
                }
            }
        }

        /// <summary>
        /// Обработчик деактивации элемента
        /// </summary>
        static private void OnPaneDeactivated()
        {
            if (paneDeactivated != null)
            {
                paneDeactivated();
            }
        }

        #region Динамическое создание объектов

        /// <summary>
        /// Создание области
        /// </summary>
        /// <param name="dockLocation">Расположение</param>
        /// <param name="dockAreaId">Id области</param>
        /// <param name="locationPoint">положение верхнего левого угла области</param>
        /// <param name="size">размер области</param>
        /// <param name="childPaneStyle">стиль области</param>
        /// <returns>созданная область</returns>
        private static DockAreaPane NewArea(DockedLocation dockLocation, Guid dockAreaId, Point locationPoint, Size size, ChildPaneStyle childPaneStyle)
        {
            DockAreaPane newDockAreaPane = new DockAreaPane(dockLocation, dockAreaId);
            newDockAreaPane.FloatingLocation = locationPoint;
            newDockAreaPane.Size = new Size(size.Width - 6, size.Height - 25);
            newDockAreaPane.ChildPaneStyle = childPaneStyle;
            newDockAreaPane.Settings.AllowMinimize = Infragistics.Win.DefaultableBoolean.True;
            return newDockAreaPane;
        }

        /// <summary>
        /// Создание докабельного окна
        /// </summary>
        /// <param name="paneId">Id окна</param>
        /// <param name="floatParentId">Id родителя плавающего окна </param>
        /// <param name="dockParentId">Id родителя докабельного окна</param>
        /// <param name="size">размер окна</param>
        /// <param name="minimized">свернуто ли окно</param>
        /// <param name="pinned">пришпилено ли окно</param>
        /// <param name="caption">заголовок окна</param>
        /// <returns>созданное окно</returns>
        private static DockableControlPane NewControlPane(Guid paneId, Guid floatParentId, Guid dockParentId, Size size, bool minimized, bool pinned)
        {
            DockableControlPane newDockableControlPane = new DockableControlPane(paneId,
                                                                                 floatParentId,
                                                                                 0,
                                                                                 dockParentId,
                                                                                 0);
            newDockableControlPane.Size = size;
            newDockableControlPane.Minimized = minimized;
            newDockableControlPane.Pinned = pinned;
            newDockableControlPane.Key = newDockableControlPane.GetHashCode().ToString();
            return newDockableControlPane;
        }

        /// <summary>
        /// Создание группового окна
        /// </summary>
        /// <param name="paneId">Id панели</param>
        /// <param name="floatParentId">Id родителя плавающего окна </param>
        /// <param name="dockParentId">Id родителя докабельного окна</param>
        /// <param name="style">стиль области</param>
        /// <param name="size">размер окна</param>
        /// <param name="minimized">свернуто ли окно</param>
        /// <returns>созданное окно</returns>
        private static DockableGroupPane NewGroupPane(Guid paneId, Guid floatParentId, Guid dockParentId, ChildPaneStyle style, Size size, bool minimized)
        {
            DockableGroupPane newDockableGroupPane = new DockableGroupPane(paneId,
                                                                           floatParentId,
                                                                           0,
                                                                           dockParentId,
                                                                           0);
            newDockableGroupPane.ChildPaneStyle = style;
            newDockableGroupPane.Size = size;
            newDockableGroupPane.Minimized = minimized;
            return newDockableGroupPane;
        }

        /// <summary>
        ///  Создание элемента отчета
        /// </summary>
        /// <param name="cubeName">куб</param>
        /// <returns>созданный элемент</returns>
        private CustomReportElement NewReportElement(DockSettings dockSettings, DockableControlPane parent, 
            string cubeName, ReportElementType reportElementType, string styleSheet, XmlNode reportElementNode)
        {
            CustomReportElement newElement = null;
            switch (reportElementType)
            {
                case ReportElementType.eTable:
                    newElement = new TableReportElement(this.MainForm, styleSheet);
                    break;
                case ReportElementType.eChart:
                    newElement = new ChartReportElement(this.MainForm, this.MainForm.IsCompositeChart);
                    break;
                case ReportElementType.eMap:
                    newElement = new MapReportElement(this.MainForm);
                    break;
                case ReportElementType.eGauge:
                    newElement = new GaugeReportElement(this.MainForm);
                    break;
                case ReportElementType.eMultiGauge:
                    newElement = new MultipleGaugeReportElement(this.MainForm);
                    break;
            }
            
            if (newElement == null)
            {
                return newElement;
            }
            
            newElement.PivotData.CubeName = cubeName;

            parent.Control = newElement;
            parent.Control.Size = parent.Size;

            //Если dockSettings == null, значит элемент только создан, версия свеженькая
            //во всех остальных случаях берем свойство из dockSettings
            string reportVersion = Consts.applicationVersion; 
            if (dockSettings != null) 
                reportVersion = dockSettings.version.Number ?? string.Empty;
            //Если версия отчета старей приложения, будем прогонять мета данные через конвертор
            if (CommonUtils.GetVersionRelation(reportVersion, Consts.applicationVersion) == VersionRelation.Ancient)
            {
                Convertor convertor = new Convertor();
                convertor.Update(ref reportElementNode, reportVersion);
            }

            newElement.Load(reportElementNode, true);
            //После загрузки элемента сбросим и сохраним его первоночальное сосоятние
            this.MainForm.UndoRedoManager.AddEvent(newElement, this.MainForm.IsCompositeChart ?
                UndoRedoEventType.AppearanceChange : UndoRedoEventType.DataChange, true);

            newElement.Deactivated += new ReportElementEventHandler(OnPaneDeactivated);

            return newElement;
        }

        #endregion

        #region Добавление объектов на DockManager

        /// <summary>
        /// Добавление окна в нулевую область DockManager (параметры по умолчанию)
        /// <param name="cubeDef">куб</param>
        /// </summary>
        public DockableControlPane AddDockControlPane(string cubeName, ReportElementType reportElementType)
        {
            // Генерация ключей
            Guid dockControlPaneId = Guid.NewGuid();
            Guid floatParentId = Guid.NewGuid();
            Guid dockParentId = new Guid("00000000-0000-0000-0000-000000000000");

            DockableControlPane newDockableControlPane = NewControlPane(dockControlPaneId, floatParentId,
                dockParentId, new Size(100, 100), false, true);
           
            SuspendLayout();

            // Если у DockManager'a нет областей
            if (udManager.DockAreas.Count == 0)
            {
                // то добавляем нулевую область
                Guid areaId = new Guid("00000000-0000-0000-0000-000000000000");
                DockAreaPane newDockAreaPane = NewArea(DockedLocation.DockedLeft, areaId, new Point(0, 0), new Size(100, 100),
                                                       ChildPaneStyle.VerticalSplit);
                newDockAreaPane.Key = newDockAreaPane.GetHashCode().ToString();
                udManager.DockAreas.Add(newDockAreaPane);
            }
            udManager.DockAreas[0].Panes.Insert(newDockableControlPane, 0);
            if (newDockableControlPane.DockedState == DockedState.Floating)
            {
                newDockableControlPane.Dock(DockedSide.Left);
            }
            

            ResumeLayout(false);
            PerformLayout();

            NewReportElement(null, newDockableControlPane, cubeName, reportElementType, this.StyleSheet, null);

            //активируем добавленную панель
            newDockableControlPane.Activate();

            return newDockableControlPane;
        }

        /// <summary>
        /// Добавление только докабельного окна на DockManager
        /// </summary>
        /// <param name="parent">родитель окна</param>
        /// <param name="cubeDefName">имя куба</param>
        /// <param name="paneId">Id окна</param>
        /// <param name="floatParentId">Id родителя плавающего окна</param>
        /// <param name="dockParentId">Id родителя докабельного окна</param>
        /// <param name="state">cостояние окна</param>
        /// <param name="size">размер окна</param>
        /// <param name="closed">скрыто ли оно</param>
        /// <param name="pinned">пришпилено ли окно</param>
        /// <param name="caption">заголовок окна</param>
        /// <param name="mdxQuery">mdx-запрос</param>
        public void AddDockableControlPane(object parent, DockSettings dockSettings, Guid paneId, Guid floatParentId, Guid dockParentId,
            Size size, bool closed, bool pinned, XmlNode reportElementNode,
            ReportElementType reportElementType)
        {
            DockableControlPane newDockableControlPane = NewControlPane(paneId, floatParentId, dockParentId, size, closed, pinned);

            if (parent is DockAreaPane)
            {
                ((DockAreaPane)parent).Panes.Add(newDockableControlPane);
            }
            else if (parent is DockableGroupPane)
            {
                ((DockableGroupPane)parent).Panes.Add(newDockableControlPane);
            }
            if (newDockableControlPane.DockedState == DockedState.Floating)
            {
                newDockableControlPane.Dock(DockedSide.Left);
            }


            try
            {
                NewReportElement(dockSettings, newDockableControlPane, string.Empty, reportElementType, this.StyleSheet,
                    reportElementNode);
            }
            catch (Exception e)
            {
                if (e is AdomdException)
                {
                    if (AdomdExceptionHandler.ProcessOK((AdomdException)e))
                    {
                        AdomdExceptionHandler.IsRepeatedProcess = true;
                        AddDockableControlPane(parent, dockSettings, paneId, floatParentId, dockParentId, new Size(300, 400),
                            closed, pinned, reportElementNode, reportElementType);
                        AdomdExceptionHandler.IsRepeatedProcess = false;
                        return;
                    }
                }

                Common.CommonUtils.ProcessException(e);
            }
        }

        /// <summary>
        /// Добавление группового окна на DockManager
        /// </summary>
        /// <param name="parent">родитель окна</param>
        /// <param name="paneId">Id окна</param>
        /// <param name="floatParentId">Id родителя плавающего окна</param>
        /// <param name="dockParentId">Id родителя докабельного окна</param>
        /// <param name="state">cостояние окна</param>
        /// <param name="style">cтиль группировки потомков</param>
        /// <param name="size">размер докабельного окна</param>
        /// <param name="closed">скрыто ли оно</param>
        /// <returns>добавленное окно</returns> 
        public DockableGroupPane AddDockableGroupPane(object parent, Guid paneId, Guid floatParentId, Guid dockParentId, DockedState state, ChildPaneStyle style, Size size, bool closed)
        {
            DockableGroupPane newDockableGroupPane = NewGroupPane(paneId, floatParentId, dockParentId, style, size, closed);

            if (parent is DockAreaPane)
            {
                ((DockAreaPane)parent).Panes.Add(newDockableGroupPane);
            }
            else if (parent is DockableGroupPane)
            {
                ((DockableGroupPane)parent).Panes.Add(newDockableGroupPane);
            }

            return newDockableGroupPane;
        }

        /// <summary>
        /// Добавление только области (без окна) на DockManager
        /// </summary>
        /// <param name="dockAreaId">Id области</param>
        /// <param name="dockLocation">ориентация области</param>
        /// <param name="locationPoint">положение левого верхнего угла области</param>
        /// <param name="size">размер области</param>
        /// <param name="childPaneStyle">cтиль группировки потомков</param>
        /// <returns>добавленная область</returns>
        public DockAreaPane AddDockAreaPane(Guid dockAreaId, DockedLocation dockLocation, Point locationPoint, Size size, ChildPaneStyle childPaneStyle)
        {
            DockAreaPane newDockAreaPane = NewArea(dockLocation, dockAreaId, locationPoint, size, childPaneStyle);
            udManager.DockAreas.Add(newDockAreaPane);

            return newDockAreaPane;
        }

        #endregion

        #region Обработчики

        /// <summary>
        /// Обработка нажатия на кнопку окна
        /// </summary>
        private void ultraDockManager_BeforePaneButtonClick(object sender, CancelablePaneButtonEventArgs e)
        {
            if (e.Button == PaneButton.Close)
            {
                if (((DockableControlPane)e.Pane).Control != null)
                {
                    ((DockableControlPane)e.Pane).Control.Dispose();
                }
                udManager.ControlPanes.Remove((DockableControlPane)e.Pane);
            }
        }

        private void ultraDockManager_AfterDockChange(object sender, PaneEventArgs e)
        {
            try
            {
                //Изменения в положении элементов в историю попадать не должны
                //поэтому временно приостанавливаем запись событий
                this.MainForm.UndoRedoManager.IsRecordHistory = false;
                this.MainForm.Saved = false;
            }
            finally
            {
                this.MainForm.UndoRedoManager.IsRecordHistory = true;
            }
        }

        #endregion
    }
}
