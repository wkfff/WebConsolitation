using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.Xml;
using System.Drawing;
using Krista.FM.Client.MDXExpert.CommonClass;
using Krista.FM.Client.MDXExpert.Controls;
using System.Data;
using Axis = Microsoft.AnalysisServices.AdomdClient.Axis;

namespace Krista.FM.Client.MDXExpert
{
    public class TableReportElement: CustomReportElement
    {
        //Грид для отображения данных
        private Grid.ExpertGrid expertGrid;
        
        //Класс для управления постраничным выводом таблицы
        private TablePager _tablePager;
        
        //Вклчен режим многостраничного отображения
        private bool pagingModeEnable;

        //список уникальных имен элементов, привязанных к таблице.
        private List<string> anchoredElements = new List<string>();

        public TableReportElement(MainForm mainForm, string styleName)
            : base(mainForm, ReportElementType.eTable)
        {
            this.expertGrid = new Grid.ExpertGrid(this.PivotData);
            SetStyle(styleName);
            this.expertGrid.Parent = this.ElementPlace;
            this.expertGrid.Dock = DockStyle.Fill;
            this.expertGrid.RecalculatedGrid += new EventHandler(expertGrid_RecalculatedGrid);
            this.expertGrid.DropButtonClick += new ExpertGrid.DropButtonClickHandler(expertGrid_DropButtonClick);
            this.expertGrid.SortClick += new ExpertGrid.SortClickHandler(expertGrid_SortClick);
            this.expertGrid.ObjectSelected += new ExpertGrid.ObjectSelectedHandler(expertGrid_ObjectSelected);
            this.expertGrid.GridSizeChanged += new EventHandler(expertGrid_GridSizeChanged);
            this.expertGrid.DrillThrough += new ExpertGrid.DrillThroughHandler(expertGrid_DrillThrough);
            this.expertGrid.ExpandedMember += new ExpertGrid.ExpandedMemberHandler(expertGrid_ExpandMember);
            this.expertGrid.ColorRulesChanged += new EventHandler(expertGrid_ColorRulesChanged);
            this.expertGrid.ScaleChanged += new EventHandler(expertGrid_ScaleChanged);
            this.ElementType = ReportElementType.eTable;

            this.PivotData.ColumnAxis.Caption = "Столбцы";
            this.PivotData.RowAxis.Caption = "Строки";

            this.PivotData.TotalAxis.Caption = "Меры";

            TablePager = new TablePager();
            PadingModeEnable = false;
            TablePager.Parent = this;
            TablePager.Dock = DockStyle.Top;
            TablePager.PageChanged += new TablePagerChangeEventHandler(OnPageChangedByUser);
            TablePager.PageSizeChanged += new PageSizeChangeEventHandler(TablePager_PageSizeChanged);

            this.PivotData.DataChanged += new PivotDataEventHandler(OnPivotDataChange);
            this.PivotData.StructureChanged += new PivotDataEventHandler(PivotData_StructureChanged);
        }

        void expertGrid_ColorRulesChanged(object sender, EventArgs e)
        {
            this.MainForm.Saved = false;
        }

        void expertGrid_ScaleChanged(object sender, EventArgs e)
        {
            if ((this.PivotData.SelectionType == SelectionType.GeneralArea)||
                ((this.PivotData.SelectionType == SelectionType.SingleObject) && (String.IsNullOrEmpty(this.PivotData.Selection))))
                this.MainForm.PropertyGrid.Refresh();
        }


        public override void Load(XmlNode reportElement, bool isForceDataUpdate)
        {
            base.Load(reportElement, isForceDataUpdate);

            if (reportElement == null)
                return;

            //максимальное количество строк на странице
            this.TablePager.SetPageSizeWithoutRefresh(XmlHelper.GetIntAttrValue(reportElement,
                Common.Consts.tablePageSize, TablePager.DefaultPageSize));
            this.TablePager.SetPageNumberWithoutRefresh(XmlHelper.GetIntAttrValue(reportElement,
                Common.Consts.tablePageNumber, 1));
            this.AnchoredElements = XmlHelper.GetStringListFromXmlNode(reportElement.SelectSingleNode(Common.Consts.anchoredElems));
            this.AnchoredElements = this.AnchoredElements.Distinct().ToList();


            //появилось много свойств выставлять которые надо как до, так и после получения данных
            this.LoadPreset(reportElement.SelectSingleNode(Common.Consts.presets));
            //пивот дата
            this.PivotData.Load(reportElement.SelectSingleNode(Common.Consts.pivotData), isForceDataUpdate);
            //загрузка свойств таблицы
            this.LoadPreset(reportElement.SelectSingleNode(Common.Consts.presets));
            

            //this.PivotData.DoStructureChanged();
            this.PivotData.DoForceDataChanged();

        }

        public override XmlNode Save()
        {
            XmlNode result = base.Save();
            
            //максимальное количество строк на странице
            XmlHelper.SetAttribute(result, Common.Consts.tablePageSize, this.TablePager.PageSize.ToString());
            XmlHelper.SetAttribute(result, Common.Consts.tablePageNumber, this.TablePager.CurrentPageNumber.ToString());

            this.AnchoredElements = this.AnchoredElements.Distinct().ToList();
            for(int i = 0; i < this.AnchoredElements.Count; i++)
            {
                if (this.MainForm.FindReportElement(this.AnchoredElements[i]) == null)
                {
                    this.AnchoredElements.RemoveAt(i);
                    i--;
                }
            }

            XmlHelper.AddStringListNode(result, Common.Consts.anchoredElems, this.AnchoredElements);
            
            this.SavePreset(XmlHelper.AddChildNode(result, Common.Consts.presets));
            XmlNode node;
            
            return result;
        }

        private void SavePreset(XmlNode presetNode)
        {
            XmlHelper.AppendCDataSection(presetNode, this.ExpertGrid.SavePropertys().InnerXml);
        }

        private void ExceptLoadPreset(XmlNode presetNode)
        {
            if (presetNode == null)
                return;

            string reportElementPreset = presetNode.FirstChild.Value;

            if (reportElementPreset != string.Empty)
            {
                using (StringReader stringReader = new StringReader(reportElementPreset))
                {
                    XmlDocument dom = new XmlDocument();
                    try
                    {
                        dom.LoadXml(reportElementPreset);
                        XmlNode gridPropertys = dom.SelectSingleNode("gridPropertys");
                        this.ExpertGrid.FilterCaptions.Load(gridPropertys.SelectSingleNode("filtersCaptions"), false);
                    }
                    finally
                    {
                        XmlHelper.ClearDomDocument(ref dom);
                    }
                }
            }
        }

        private void LoadPreset(XmlNode presetNode)
        {
            if (presetNode == null)
                return;

            string reportElementPreset = presetNode.FirstChild.Value;

            if (reportElementPreset != string.Empty)
            {
                using (StringReader stringReader = new StringReader(reportElementPreset))
                {
                    XmlDocument dom = new XmlDocument();
                    try
                    {
                        dom.LoadXml(reportElementPreset);
                        this.ExpertGrid.LoadPropertys(dom);
                    }
                    finally
                    {
                        XmlHelper.ClearDomDocument(ref dom);
                    }
                }
            }
        }

        private void OnPivotDataChange()
        {
            if (CheckConnection())
                this.RefreshData();
            
            //обновляем пивот-дату для привязанных элементов
            foreach(string key in this.AnchoredElements)
            {
                ChartReportElement chartElement = this.MainForm.FindChartReportElement(key);
                if (chartElement != null)
                {
                    chartElement.Synchronize(this.PivotData, false, false);
                }
                else
                {
                    MapReportElement mapElement = this.MainForm.FindMapReportElement(key);
                    if (mapElement != null)
                    {
                        mapElement.Synchronize(this.PivotData, false, false);
                    }

                    else
                    {
                        MultipleGaugeReportElement gaugeElement = this.MainForm.FindMultiGaugeReportElement(key);
                        if (gaugeElement != null)
                        {
                            gaugeElement.Synchronize(this.PivotData, false, false);
                        }

                    }
                }
            }
        }

        void PivotData_StructureChanged()
        {
            this.InitialByCellSet();
            this.MainForm.RefreshUserInterface(this);
        }

        private void OnPageChangedByUser()
        {
            if (PadingModeEnable)
            {
                this.RefreshData(TablePager.LowNumberOnPage, TablePager.PageSize);
            }            
        }

        void TablePager_PageSizeChanged()
        {
            if (!this.PivotData.IsDeferDataUpdating)
            {
                //Выбрали максимальное количество запрашиваемых строк. 
                //Общее количество строк получать не будем, т.к. оно уже известно
                PadingModeEnable = (TablePager.RecordCount > TablePager.PageSize);
                if (this.PadingModeEnable)
                    this.RefreshData(TablePager.LowNumberOnPage, TablePager.PageSize);
                else
                    base.RefreshData();
            }
        }

        /// <summary>
        /// Настройка пэйджинга и соответствующее обновление данных
        /// </summary>
        protected override void RefreshData()
        {
            if (!base.CheckConnection())
                return;
            

            //Если включена опция динамической загрузки данных или таблица строиться по пользовательскому запросу, 
            //то постраничный режим нам не пригодится
            this.TablePager.RecordCount = this.PivotData.DynamicLoadData || this.PivotData.IsCustomMDX ?
                0 : this.RowsCount();
            this.PadingModeEnable = (TablePager.RecordCount > TablePager.PageSize);
            //В некоторых ситуациях таблице надо знать, в каком режиме она работает
            this.ExpertGrid.IsPaddingModeEnabled = this.PadingModeEnable;
            
            
            if (PadingModeEnable)
            {
                if (this.TablePager.CurrentPageNumber < 1)
                    this.TablePager.CurrentPageNumber = 1;
                else
                    if (this.TablePager.CurrentPageNumber > this.TablePager.PageCount)
                        this.TablePager.CurrentPageNumber = this.TablePager.PageCount;
                    else
                    {
                        this.TablePager.SetControlState();
                        //если сработало хотябы одно из предыдущих условий, таблица обновления не требует
                        //т.к. обновление происходит при выставлении номера страницы
                        base.RefreshData(TablePager.LowNumberOnPage, TablePager.PageSize);
                    }
            }
            else
            {
                //если таблица вмещается на одну страницу, используем базовое обновленние
                base.RefreshData();
            }
            
        }

        /// <returns>Вернет общее кол-во строк в таблице</returns>
        private int RowsCount()
        {
            MDXQueryBuilder mdxQueryBuilder = new MDXQueryBuilder();
            mdxQueryBuilder.ElementType = ReportElementType.eTable;
            string countQuery = mdxQueryBuilder.BuildMDXQueryForRecordCount(this.PivotData);

            if (string.IsNullOrEmpty(countQuery))
                return 0;

            try
            {
                this.MainForm.Operation.StartOperation();
                this.MainForm.Operation.Text = "Получение общего кол-ва строк";
                CellSet cls = this.MainForm.MdxCommand.Execute(countQuery, PivotData.AdomdConn);
                return ((int)cls.Cells[0].Value);
            }
            catch
            {
                #warning Исключения не обрабатываются!
                return 0;
            }
            finally
            {
                this.MainForm.Operation.StopOperation();
            }
        }



        void expertGrid_RecalculatedGrid(object sender, EventArgs e)
        {
            this.MainForm.Saved = false;
        }

        void expertGrid_DropButtonClick(object sender, string hierarchyUN)
        {
            this.PivotData.ShowMemberList(hierarchyUN);
        }

        void expertGrid_SortClick(object sender, string uniqueName, SortType sortType, string sortedTupleUN)
        {
            this.PivotData.ChangeObjectSort(uniqueName, sortType, sortedTupleUN);
        }

        void expertGrid_ObjectSelected(SelectionType selectionType, string objectUN)
        {
            this.PivotData.SetSelection(selectionType, objectUN);
            UpdateAnchoredGauge();
        }

        void expertGrid_GridSizeChanged(object sender, EventArgs e)
        {
            this.MainForm.UndoRedoManager.AddEvent(this, UndoRedoEventType.AppearanceChange);
        }

        void expertGrid_DrillThrough(string measureUN, string rowCellUN, bool rowCellIsTotal,
            string columnCellUN, bool columnCellIsTotal, string actionName)
        {
            DrillThroughForm drillForm = new DrillThroughForm(this.MainForm, this.PivotData);
            try
            {
                drillForm.Operation = this.MainForm.Operation;
                drillForm.ShowDrillThroughData(measureUN, rowCellUN, rowCellIsTotal, columnCellUN, columnCellIsTotal, actionName);
            }
            finally
            {
            }
        }

        void expertGrid_ExpandMember(string dimemsionUN, string levelUN, bool state)
        {
            if (state)
                this.PivotData.ExpandNextLevel(dimemsionUN, levelUN);

            RefreshSyncCharts();
        }

        /// <summary>
        /// обновление диаграмм, синхронизированных с таблицей
        /// </summary>
        public void RefreshSyncCharts()
        {
            foreach (string elemName in this.AnchoredElements)
            {
                ChartReportElement chartElement = this.MainForm.FindChartReportElement(elemName);
                if (chartElement != null)
                {
                    chartElement.InitialByCellSet();
                }
                else
                {
                    MapReportElement mapElement = this.MainForm.FindMapReportElement(elemName);
                    if (mapElement != null)
                    {
                        mapElement.InitialByCellSet();
                    }
                    else
                    {
                        MultipleGaugeReportElement multiGaugeElement = this.MainForm.FindMultiGaugeReportElement(elemName);
                        if (multiGaugeElement != null)
                        {
                            multiGaugeElement.InitialByCellSet();
                        }

                    }
                }
            }
        }

        protected override CellSet SetMDXQuery(string mdxQuery)
        {
            CellSet cls = null;
            try
            {
                cls = base.SetMDXQuery(mdxQuery);
                this.InitialByCellSet(cls);
            }
            catch (Exception exc)
            {
                this.InitialByCellSet();
                Common.CommonUtils.ProcessException(exc);
            }
            return cls;
        }

        /// <summary>
        /// Именно в этом методе происходит инициализация элемента отчета по CellSet-у 
        /// </summary>
        /// <param name="cls"></param>
        public override void InitialByCellSet(CellSet cls)
        {
            base.InitialByCellSet(cls);
            //Если передвигаемся вперед по страницам, будем показывать начало таблицы
            if (this.PadingModeEnable && (this.TablePager.Direction == DirectionMode.Up))
                expertGrid.VScrollBar.Value = 0;
            expertGrid.InitializeGrid(cls);
        }

        public override void SetElementVisible(bool value)
        {
            if (this.ExpertGrid.Visible != value)
                this.ExpertGrid.Visible = value;
        }

        public void SetStyle(string styleName)
        {
            if (styleName != "")
            {
                this.expertGrid.LoadPropertys(styleName);
            }
        }

        /// <summary>
        /// Получить изображение печатаемой области таблицы
        /// </summary>
        /// <param name="pageBounds"></param>
        /// <returns></returns>
        public override Bitmap GetPrintableImage(Rectangle pageBounds)
        {
            return this.GetBitmap();
        }

        /// <summary>
        /// Получить изображение всей области таблицы (!!!пока возвращается только видимая)
        /// </summary>
        /// <returns></returns>
        public override Bitmap GetBitmap()
        {
            return base.GetBitmap(this.ClientRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (expertGrid != null))
            {
                expertGrid.ClearAll();
                expertGrid.Dispose();
                expertGrid = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Пользовательский интерфейс грида
        /// </summary>
        public override IGridUserInterface GridUserInterface
        {
            get { return expertGrid; }
        }

        public ExpertGrid ExpertGrid
        {
            get
            {
                return expertGrid;
            }
        }

        public TablePager TablePager
        {
            get { return _tablePager; }
            set { _tablePager = value; }
        }

        /// <summary>
        /// Включен режим многостраничного отображения
        /// </summary>
        public bool PadingModeEnable
        {
            get { return pagingModeEnable; }
            set
            {
                pagingModeEnable = value;
                TablePager.Visible = value;
            }
        }

        /// <summary>
        /// Показывать ли сообщение с ошибкой
        /// </summary>
        public override bool IsShowErrorMessage
        {
            get { return base.IsCubeNotFond; }
        }

        public List<string> AnchoredElements
        {
            get { return anchoredElements; }
            set { anchoredElements = value; }
        }

        
        /// <summary>
        /// Есть серии?
        /// </summary>
        private bool SeriesExist(CellSet cls)
        {
            if (cls != null)
            {
                return (cls.OlapInfo.AxesInfo.Axes.Count > 1);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// в поле храним индексы кортежей с которых надо брать данные столбцам (категориям) 
        /// </summary>
        List<int> columnPosNum = new List<int>();

        /// <summary>
        /// в поле храним индексы кортежей с которых надо брать данные строкам (сериям) 
        /// </summary>
        List<int> rowPosNum = new List<int>();

        private bool IsMeasure(Member mbr)
        {
            return mbr.LevelName == "[Measures].[MeasuresLevel]";
        }


        /// <summary>
        /// Запоминаем номера позиций на осях целлсета, из которых будем учитывать данные для индикатора
        /// Нужно учитывать показатель, и уровень элементов который выбран
        /// </summary>
        private void GetFilteredDataPositions(out double currentValue, bool isCurrentColumnValues)
        {
            currentValue = 0;

            if (!this.PivotData.CheckConnection())
            {
                return;
            }

            List<string> levels = new List<string>();
            List<string> members = new List<string>();

            CellSet cls = this.CLS;
            string measureName = "";

            try
            {
                if ((this.ExpertGrid.SelectedCells.CurrentCell != null) &&
                    (this.ExpertGrid.SelectedCells.CurrentCell is MeasureCell))
                {
                    MeasureCell mc = (MeasureCell) this.ExpertGrid.SelectedCells.CurrentCell;
                    measureName = mc.MeasureData.MeasureCaption.UniqueName;
                    if ((mc.Value != null) && (mc.Value.Value != null))
                    {
                        //currentValue = (double) mc.Value.Value;
                        Double.TryParse(mc.Value.Value.ToString(), out currentValue);
                    }

                    if (mc.RowCell != null)
                    {
                        DimensionCell dc = mc.RowCell;
                        Member mbr = null;

                        while (dc != null)
                        {
                            if (dc.ClsMember != null)
                            {

                                if (((mbr != null)&&(mbr.UniqueName != dc.ClsMember.UniqueName)) || (mbr == null))
                                {
                                    mbr = dc.ClsMember;

                                    if (mbr != null)
                                        levels.Add(mbr.ParentLevel.UniqueName);
                                }

                                if (mbr != null)
                                    mbr = mbr.Parent;
                            }
                            dc = dc.Parent;
                        }
                    }

                    if (mc.CaptionsSection.ColumnCell != null)
                    {
                        DimensionCell dc = mc.CaptionsSection.ColumnCell;
                        Member mbr = null;

                        while (dc != null)
                        {
                            if (dc.ClsMember != null)
                            {

                                if (((mbr != null) && (mbr.UniqueName != dc.ClsMember.UniqueName)) || (mbr == null))
                                {
                                    mbr = dc.ClsMember;

                                    if (mbr != null)
                                    {
                                        levels.Add(mbr.ParentLevel.UniqueName);
                                        members.Add(mbr.UniqueName);
                                    }
                                }

                                if (mbr != null)
                                    mbr = mbr.Parent;
                            }
                            dc = dc.Parent;
                        }
                    }
                }
                else
                {
                    return;
                }

                this.rowPosNum.Clear();
                this.columnPosNum.Clear();


                for (int k = 0; k < cls.Axes.Count; k++)
                {
                    for (int i = 0; i < cls.Axes[k].Positions.Count; i++)
                    {
                        Position pos = cls.Axes[k].Positions[i];
                        bool canAppendPos = true;

                        if ((isCurrentColumnValues) && (k == 0))
                        {
                            foreach (Member mem in pos.Members)
                            {
                                if ((IsMeasure(mem) && (mem.UniqueName != measureName)) ||
                                    (!IsMeasure(mem) && (!members.Contains(mem.UniqueName))))
                                {
                                    canAppendPos = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (Member mem in pos.Members)
                            {
                                if ((IsMeasure(mem) && (mem.UniqueName != measureName)) ||
                                    (!IsMeasure(mem) && (!levels.Contains(mem.ParentLevel.UniqueName))))
                                {
                                    canAppendPos = false;
                                    break;
                                }
                            }
                        }

                        if (canAppendPos)
                        {
                            if (k == 0)
                            {
                                this.columnPosNum.Add(pos.Ordinal);
                            }
                            else if (k == 1)
                            {
                                this.rowPosNum.Add(pos.Ordinal);
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                if (AdomdExceptionHandler.IsAdomdException(exc))
                {
                    if (AdomdExceptionHandler.ProcessOK(exc))
                    {
                        AdomdExceptionHandler.IsRepeatedProcess = true;
                        GetFilteredDataPositions(out currentValue, isCurrentColumnValues);
                        AdomdExceptionHandler.IsRepeatedProcess = false;
                        return;
                    }
                }

                Common.CommonUtils.ProcessException(exc);


            }
        }


        public void GetLimitValuesFromCellSet(out double minValue, out double maxValue)
        {
            CellSet cls = this.CLS;
            minValue = 0;
            maxValue = 0;
            object value = null;

            if (cls.Axes.Count > 1)
            {
                for (int r = 0; r < this.rowPosNum.Count; r++)
                {
                    for (int c = 0; c < this.columnPosNum.Count; c++)
                    {
                        value = this.GetCellValue(cls.Cells, this.columnPosNum[c], this.rowPosNum[r]);
                        if (value != null)
                        {
                            if ((double)value > maxValue)
                                maxValue = (double)value;
                            if ((double)value < minValue)
                                minValue = (double)value;
                        }
                    }
                }
            }
            else
            {
                if (cls.Cells.Count > 0)
                {
                    for (int c = 0; c < this.columnPosNum.Count; c++)
                    {
                        value = this.GetCellValue(cls.Cells, this.columnPosNum[c]);
                        if (value != null)
                        {
                            if ((double)value > maxValue)
                                maxValue = (double)value;
                            if ((double)value < minValue)
                                minValue = (double)value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получение значения из целлсета по 2м координатам
        /// </summary>
        /// <param name="cells">ячейки</param>
        /// <param name="index1">столбцы</param>
        /// <param name="index2">строки</param>
        /// <returns>значение</returns>
        private object GetCellValue(CellCollection cells, int index1, int index2)
        {
            try
            {
                return Double.Parse(cells[index1, index2].Value.ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Получение значения из целлсета по одной координате
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private object GetCellValue(CellCollection cells, int index)
        {
            try
            {
                return Double.Parse(cells[index].Value.ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Обновление индикаторов привязанных к таблице
        /// </summary>
        public void UpdateAnchoredGauge()
        {
            foreach (string key in this.AnchoredElements)
            {
                GaugeReportElement gaugeElement = this.MainForm.FindGaugeReportElement(key);
                if (gaugeElement != null)
                {
                    UpdateGaugeValues(gaugeElement);
                }
            }

        }

        /// <summary>
        /// Проверяет значение на корректность (должно быть числовым и не равно бесконечности)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsCorrectValue(double value)
        {
            return (!double.IsInfinity(value) && !double.IsNaN(value));
        }

        private void UpdateGaugeValues(GaugeReportElement gaugeElement)
        {
            double currentValue, startValue, endValue;
            GetFilteredDataPositions(out currentValue, gaugeElement.Synchronization.IsCurrentColumnValues);
            GetLimitValuesFromCellSet(out startValue, out endValue);

            if (!IsCorrectValue(startValue) || !IsCorrectValue(endValue) || !IsCorrectValue(currentValue))
                return;

            if ((currentValue != null )&&(startValue != null )&&(endValue != null))
            {
                if ((startValue <= endValue)&&(currentValue >= startValue)&&(currentValue <= endValue))
                {
                    if (!gaugeElement.AutoTickmarkCalculation)
                    {
                        gaugeElement.CalcTickMarkInterval(ref startValue, ref endValue, 10);
                    }

                    gaugeElement.IsNeedRefresh = false;

                    gaugeElement.SetValues(startValue, endValue, currentValue);

                    if (!gaugeElement.AutoTickmarkCalculation)
                    {
                        gaugeElement.TickmarkInterval = gaugeElement.TickmarkInterval;
                    }
                    gaugeElement.IsNeedRefresh = true;
                    gaugeElement.Gauge.Refresh();

                }
            }

        }

    }
}
