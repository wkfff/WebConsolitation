using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.Text.RegularExpressions;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using System.Collections;
using System.ComponentModel;
using System.Xml;
using System.IO;
using AxisType=Krista.FM.Client.MDXExpert.Data.AxisType;

namespace Krista.FM.Client.MDXExpert 
{
    public class ChartReportElement : CustomReportElement, ICompositeLegendParams
    {
        private UltraChart chart;
        private DataTable _sourceDT;
        private static Dictionary<string, string> invalidDataMessages = MakeInvalidDataMessages();
        //Разделитель элементов сейрий
        private DataSeriesSeparator rowsSeparator = DataSeriesSeparator.eComma;
        //Разделитель элементов категорий
        private DataSeriesSeparator columnsSeparator = DataSeriesSeparator.eComma;
        //Элемент обновляется из базы
        private bool _isUpdatable = true;
        //Существуют ли отрицательные данные в диаграмме
        private bool _isExistsNegativeValue;
        // Контекстное меню
        private ContextMenuStrip chartContextMenu;
        // Номер слоя для PieChart/DoughnutChart диаграмм
        private int idSlice;
        private string labelSlice;
        /// <summary>
        /// в поле храним индексы кортежей с которых надо брать данные столбцам (категориям) 
        /// </summary>
        List<int> columnPosNum = new List<int>();
        /// <summary>
        /// в поле храним индексы кортежей с которых надо брать данные строкам (сериям) 
        /// </summary>
        List<int> rowPosNum = new List<int>();
        
        // является ли диаграмма композитной
        private bool isComposite = false;
        // положение композитной диаграммы
        private LegendLocation compositeLegendLocation = LegendLocation.Right;
        // размер композитной диаграммы
        private int compositeLegendExtent = 40;
        // список ключей дочерних диаграмм (для композитной)
        private List<string> childChartList = new List<string>();
        // список ключей композитных диаграмм (для некомпозитной)
        private List<string> parentChartList = new List<string>();
        // список осей композитной диаграммы
        private List<AxisItem> axies = new List<AxisItem>();
        // в поле храним все три Д свойства диаграммы, нужно для отслеживания
        // их пользователем
        private string view3DAppearanceString;

        private bool compositeAxiesCorrection = true;

        private ChartSynchronization synchronization;

        /// <summary>
        /// Корректировать ли оси композитных диаграмм
        /// </summary>
        public bool CompositeAxiesCorrection
        {
            get { return compositeAxiesCorrection; }
            set { compositeAxiesCorrection = value; }
        }

        public ChartReportElement(MainForm mainForm, bool isComposite)
            : base(mainForm, ReportElementType.eChart)
        {
            this.chart = new UltraChart();
            this.isComposite = isComposite;
            this.view3DAppearanceString = string.Empty;

            //Приостановим отрисовку панели (без этого при инициализации свойств диаграмы возникают исключения)
            this.ElementPlace.SuspendLayout();
            this.ElementPlace.AutoScroll = true;
            //this.ElementPlace.Controls.Add(this.chart);

            this.chart.Parent = this.ElementPlace;
            this.chart.Dock = DockStyle.Fill;

            if (isComposite)
            {
                Common.InfragisticsUtils.ChangeChartType(chart, ChartType.Composite);
            }
            else
            {
                Common.InfragisticsUtils.ChangeChartType(chart, ChartType.ColumnChart);
            }
            ChartFormatBrowseClass.DoFormatStringChange();
            this.chart.EmptyChartText = "";
            this.chart.Border.Thickness = 0;
            this.chart.Data.ZeroAligned = true;

            this.chart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(chart_InvalidDataReceived);
            this.chart.SizeChanged += new EventHandler(chart_SizeChanged);
            this.chart.DataItemOver += new Infragistics.UltraChart.Shared.Events.DataItemOverEventHandler(chart_DataItemOver);
            this.chart.DataItemOut += new Infragistics.UltraChart.Shared.Events.DataItemOutEventHandler(chart_DataItemOut);
            this.chart.ChartDrawItem += new ChartDrawItemEventHandler(chart_ChartDrawItem);
            this.chart.InterpolateValues += new InterpolateValuesEventHandler(chart_InterpolateValues);
            this.chart.MouseEnter += new EventHandler(chart_MouseEnter);
            this.chart.MouseLeave += new EventHandler(chart_MouseLeave);
            this.CreateContextMenu();
            this.SetCustomFormats();
            this.SetDefaultValue();

            //Восстановим отрисовку
            this.ElementPlace.ResumeLayout();

            this.PivotData.ColumnAxis.Caption = "Категории";
            this.PivotData.RowAxis.Caption = "Ряды";

            this.ElementType = ReportElementType.eChart;
            this.PivotData.DataChanged += new PivotDataEventHandler(OnPivotDataChange);
            this.PivotData.StructureChanged += new PivotDataEventHandler(PivotData_StructureChanged);
            this.PivotData.ElementsOrderChanged += new PivotDataChangeOrderEventHandler(PivotData_ElemOrderChanged);
            this.PivotData.ElementsSortChanged += new PivotDataChangeSortEventHandler(PivotData_ElemSortChanged);
            this.PivotData.AppearanceChanged += new PivotDataAppChangeEventHandler(PivotData_AppearanceChanged);

            this.synchronization = new ChartSynchronization(this);

        }


        private void CreateContextMenu()
        {
            // Создание контекстного меню
            chartContextMenu = new ContextMenuStrip();
            chartContextMenu.Items.Add("Отсоединить слой");
            chartContextMenu.Items.Add("Присоединить слой");
            //chartContextMenu.Items.Add("Отсоединить все слои");
            //chartContextMenu.Items.Add("Присоединить все слои");
            chartContextMenu.ItemClicked += new ToolStripItemClickedEventHandler(chartContextMenu_ItemClicked);
        }

        private void SetDefaultValue()
        {
            // Установка значения по умолчанию для свойства OthersCategoryText
            this.chart.PieChart.OthersCategoryText = "Прочие";
            this.chart.DoughnutChart.OthersCategoryText = "Прочие";
            this.chart.PyramidChart.OthersCategoryText = "Прочие";
            this.chart.PyramidChart3D.OthersCategoryText = "Прочие";
            this.chart.FunnelChart.OthersCategoryText = "Прочие";
            this.chart.FunnelChart3D.OthersCategoryText = "Прочие";
            this.chart.ConeChart3D.OthersCategoryText = "Прочие";
            
            // Делаем второстепенные оси невидимыми по умолчанию
            this.chart.Axis.X2.Visible = false;
            this.chart.Axis.Y2.Visible = false;
            this.chart.Axis.Z2.Visible = false;
            // В 2008 Infragisitics ось Z поумолчанию отключена, включаем ее
            this.chart.Axis.Z.Visible = true;

            // устанавливаем автоматические отступы для меток рядов и категорий
            this.chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Z.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.X2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Y2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Z2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Z.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.X2.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            this.chart.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            this.chart.Legend.MoreIndicatorText = "еще…";

            if (chart.CompositeChart.Legends.Count == 0)
            {
                // добавляем легенду
                CompositeLegend legend = new CompositeLegend();
                legend.Bounds = new Rectangle(75, 0, 100, 100);
                legend.BoundsMeasureType = MeasureType.Percentage;
                legend.PE.Fill = Color.FloralWhite;
                legend.PE.FillOpacity = 150;
                legend.Border.Color = Color.Navy;
                legend.Visible = false;
                chart.CompositeChart.Legends.Add(legend);
            }
        }

        public override System.Xml.XmlNode Save()
        {
            XmlNode result = base.Save();

            //Размер диаграммы
            SizeConverter sizeConverter = new SizeConverter();
            string elementSize = sizeConverter.ConvertToString(this.Chart.Size);
            XmlHelper.SetAttribute(result, Common.Consts.elementSize, elementSize);

            XmlHelper.SetAttribute(result, Common.Consts.elementDock, this.Chart.Dock.ToString());
            XmlHelper.SetAttribute(result, Common.Consts.isUpdatable, this.IsUpdatable.ToString());
            XmlHelper.SetAttribute(result, Common.Consts.compositeLegendLocation, this.compositeLegendLocation.ToString());
            XmlHelper.SetAttribute(result, Common.Consts.compositeLegendExtent, this.compositeLegendExtent.ToString());
            XmlHelper.AddChildNode(result, Consts.synchronization,
                                        new string[2] { Consts.boundTo, this.Synchronization.BoundTo },
                                        new string[2] { Consts.measureInRows, this.Synchronization.MeasureInRows.ToString() });


            this.SaveSourceDT(XmlHelper.AddChildNode(result, Common.Consts.sourceDT));
            this.SavePreset(XmlHelper.AddChildNode(result, Common.Consts.presets));

            return result;
        }

        public override void Load(System.Xml.XmlNode reportElement, bool isForceDataUpdate)
        {
            base.Load(reportElement, isForceDataUpdate);

            if (reportElement == null)
                return;

            this.childChartList.Clear();
            this.parentChartList.Clear();

            this.LoadPreset(reportElement.SelectSingleNode(Common.Consts.presets));
            this.isComposite = this.Chart.ChartType == ChartType.Composite;

            if (!this.isComposite)
            {
                this.IsUpdatable = XmlHelper.GetBoolAttrValue(reportElement, Common.Consts.isUpdatable, true);
                this.LoadSourceDT(reportElement.SelectSingleNode(Common.Consts.sourceDT));
            }


            if (this.isComposite)
            {
                compositeLegendLocation = (LegendLocation)Enum.Parse(typeof(LegendLocation),
                XmlHelper.GetStringAttrValue(reportElement, Common.Consts.compositeLegendLocation, "Left"));
                compositeLegendExtent = XmlHelper.GetIntAttrValue(reportElement, Common.Consts.compositeLegendExtent, 0);
                SetSpanCompositeChart(compositeLegendLocation, compositeLegendExtent);
            }

            this.Chart.Dock = (DockStyle)Enum.Parse(typeof(DockStyle),
                XmlHelper.GetStringAttrValue(reportElement, Common.Consts.elementDock, "Fill"));
            //Размер диаграммы
            string elementSize = XmlHelper.GetStringAttrValue(reportElement, Common.Consts.elementSize,
                string.Empty);
            if (elementSize != string.Empty)
            {
                SizeConverter sizeConverter = new SizeConverter();
                this.Chart.Size = (Size)sizeConverter.ConvertFromString(elementSize);
            }
            XmlNode syncNode = reportElement.SelectSingleNode(Consts.synchronization);
            if(syncNode != null)
            {
                this.Synchronization.BoundTo = XmlHelper.GetStringAttrValue(syncNode, Consts.boundTo, "");
                this.Synchronization.MeasureInRows = XmlHelper.GetBoolAttrValue(syncNode, Consts.measureInRows, true);
            }

            if (!this.IsShowErrorMessage)
                this.Chart.Show();

            if (this.isComposite)
            {
                // восстанавливаем дочерние диаграммы по ключам слоев композитной
                List<string> layerKeys = new List<string>();
                foreach (ChartLayerAppearance layer in this.Chart.CompositeChart.ChartLayers)
                {
                    layerKeys.Add(layer.Key);
                }

                this.CompositeAxiesCorrection = false;
                foreach (string key in layerKeys)
                {
                    AddChildChart(key);
                }
                this.CompositeAxiesCorrection = true;
            }
            else
            {
                DockStyle dockStyle = Chart.Dock;
                Chart.Dock = DockStyle.None;

                //пивот дата
                if (!GetSyncronizedPivotData(isForceDataUpdate))
                    this.PivotData.Load(reportElement.SelectSingleNode(Common.Consts.pivotData), isForceDataUpdate);

                Chart.Dock = dockStyle;
            }
            this.Chart.Refresh();

        }

        private void SaveSourceDT(XmlNode sourceDTNode)
        {
            if ((this.SourceDT != null) && !this.IsUpdatable)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    this.SourceDT.TableName = "Таблица";
                    this.SourceDT.WriteXml(stream, XmlWriteMode.WriteSchema);
                    stream.Flush();

                    stream.Position = 0;
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        XmlHelper.AppendCDataSection(sourceDTNode, streamReader.ReadToEnd());
                    }
                }
            }
            else
            {
                XmlHelper.AppendCDataSection(sourceDTNode, string.Empty);
            }
        }

        private void LoadSourceDT(XmlNode sourceDTNode)
        {
            if ((sourceDTNode == null) || (sourceDTNode.FirstChild.Value == string.Empty))
                return;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(sourceDTNode.FirstChild.Value);
                    streamWriter.Flush();

                    stream.Position = 0;
                    DataTable dataTable = new DataTable();
                    dataTable.ReadXml(stream);
                    this.SourceDT = dataTable;
                }
            }
        }

        private void SavePreset(XmlNode presetNode)
        {
            using (StringWriter strWriter = new StringWriter())
            {
                this.Chart.SavePreset(strWriter, "UltraChartPreset", string.Empty, 
                    Infragistics.Win.UltraWinChart.WinChartHelper.PresetType.All);
                XmlHelper.AppendCDataSection(presetNode, strWriter.ToString());
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
                    this.Chart.LoadPreset(stringReader, true);
                }
            }
        }

        /// <summary>
        /// Устанавление позиции и размера легенды композитной диаграммы
        /// </summary>
        /// <param name="location">позиция</param>
        /// <param name="span">размер</param>
        private void SetSpanCompositeChart(LegendLocation location, int span)
        {
            if (this.Chart.CompositeChart.Legends.Count == 0)
            {
                return;
            }

            switch (location)
            {
                case LegendLocation.Bottom:
                    {
                        this.Chart.CompositeChart.Legends[0].Bounds = new Rectangle(0, 100 - span, 100, 100);

                        break;
                    }
                case LegendLocation.Left:
                    {
                        this.Chart.CompositeChart.Legends[0].Bounds = new Rectangle(0, 0, span, 100);
                        break;
                    }
                case LegendLocation.Right:
                    {
                        this.Chart.CompositeChart.Legends[0].Bounds = new Rectangle(100 - span, 0, 100, 100);
                        break;
                    }
                case LegendLocation.Top:
                    {
                        this.Chart.CompositeChart.Legends[0].Bounds = new Rectangle(0, 0, 100, span);
                        break;
                    }
            }
        }

        /// <summary>
        /// Синхронизация по структуре данных
        /// </summary>
        /// <param name="pivotData">структура</param>
        /// <param name="refreshFieldList">обновлять список полей или нет</param>
        /// <param name="silentMode">true - если не хотим обновлять данные</param>
        public void Synchronize(PivotData pivotData, bool refreshFieldList, bool silentMode)
        {
            bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
            this.PivotData.IsDeferDataUpdating = true;

            this.PivotData.Clear();

            this.PivotData.CubeName = pivotData.CubeName;

            foreach (FieldSet fs in pivotData.FilterAxis.FieldSets)
                this.PivotData.FilterAxis.FieldSets.CopyFieldSet(fs, AxisType.atFilters);

            foreach (FieldSet fs in pivotData.ColumnAxis.FieldSets)
            {
                FieldSet newFS = this.PivotData.ColumnAxis.FieldSets.CopyFieldSet(fs, AxisType.atColumns); 
                newFS.IsVisibleTotals = false;
            }

            //Отключаем для диаграммы отображение общих итогов(задача #20382)
            //this.PivotData.ColumnAxis.GrandTotalVisible = pivotData.ColumnAxis.GrandTotalVisible;
            this.PivotData.ColumnAxis.GrandTotalVisible = false;

            foreach (FieldSet fs in pivotData.RowAxis.FieldSets)
            {
                FieldSet newFS = this.PivotData.RowAxis.FieldSets.CopyFieldSet(fs, AxisType.atRows);
                newFS.IsVisibleTotals = false;
            }

            //Отключаем для диаграммы отображение общих итогов(задача #20382)
            //this.PivotData.RowAxis.GrandTotalVisible = pivotData.RowAxis.GrandTotalVisible; 
            this.PivotData.RowAxis.GrandTotalVisible = false;

            pivotData.TotalAxis.RefreshMemberNames();
            if (pivotData.TotalAxis.FieldSets.Count > 0)
            {
                if (this.Synchronization.MeasureInRows)
                {
                    this.PivotData.RowAxis.FieldSets.CopyFieldSet(pivotData.TotalAxis.FieldSets[0], AxisType.atRows);
                }
                else
                {
                    this.PivotData.ColumnAxis.FieldSets.CopyFieldSet(pivotData.TotalAxis.FieldSets[0], AxisType.atColumns);
                }
            }

            foreach (PivotTotal total in pivotData.TotalAxis.Totals)
                this.PivotData.TotalAxis.CopyTotal(total);


            if (refreshFieldList)
            {
                this.MainForm.FieldListEditor.PivotData = this.PivotData;
                this.MainForm.FieldListEditor.InitEditor(this);
            }

            this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;

            if (!silentMode)
                this.RefreshData();

            //обновляем композитные диаграммы, построенные на основе этой
            List<ChartReportElement> chartElements = this.MainForm.GetCompositeParentCharts(this.UniqueName);
            foreach (ChartReportElement element in chartElements)
            {
                element.RefreshCompositeChart();
            }
        }

        public void Synchronize()
        {
            PivotData pivotData = null;
            TableReportElement tableElement = null;

            if (!String.IsNullOrEmpty(this.Synchronization.BoundTo))
            {
                tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                if(tableElement != null)
                {
                    pivotData = tableElement.PivotData;
                }
            }
            if (pivotData == null)
            {
                this.MainForm.UndoRedoManager.AddEvent(this, UndoRedoEventType.DataChange);
                return;
            }

            Synchronize(pivotData, true, false);
            if (tableElement != null)
                this.MainForm.UndoRedoManager.AddEvent(tableElement, UndoRedoEventType.DataChange);

        }

        /// <summary>
        /// Синхронизация с возможностью обхода отложенного обновления данных
        /// </summary>
        /// <param name="ignoreDeferDataUpdating"></param>
        public void Synchronize(bool forceDataUpdating)
        {
            if (forceDataUpdating)
            {
                bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
                this.PivotData.IsDeferDataUpdating = false;
                Synchronize();
                this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;
            }
            else
            {
                Synchronize();
            }
        }



        #region Обработчики событий

        // Отсоединение/присоединения слоя PieChart/DoughnutChart диаграмм
        private void chartContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            PieChartAppearance pieChartAppearance;

            if (chart.ChartType == ChartType.PieChart || chart.ChartType == ChartType.PieChart3D)
            {
                pieChartAppearance = chart.PieChart;
            }
            else
            {
                pieChartAppearance = chart.DoughnutChart;
            }

            switch (e.ClickedItem.Text)
            {
                case "Отсоединить слой":
                    {
                        if (labelSlice == pieChartAppearance.OthersCategoryText)
                        {
                            pieChartAppearance.BreakOthersSlice = true;
                        }
                        else
                        {
                            pieChartAppearance.BreakSlice(idSlice, true);
                        }
                        break;
                    }
                case "Присоединить слой":
                    {
                        if (labelSlice == pieChartAppearance.OthersCategoryText)
                        {
                            pieChartAppearance.BreakOthersSlice = false;
                        }
                        else
                        {
                            pieChartAppearance.BreakSlice(idSlice, false);
                        }
                        break;
                    }
                /*case "Присоединить все слои":
                    {
                        pieChartAppearance.BreakSliceReset();
                        pieChartAppearance.BreakAllSlices = false;
                        pieChartAppearance.BreakOthersSlice = false;
                        break;
                    }
                case "Отсоединить все слои":
                    {
                        pieChartAppearance.BreakAllSlices = true;
                        pieChartAppearance.BreakOthersSlice = true;
                        break;
                    }*/
            }

            chart.Refresh();
        }

        private void chart_DataItemOut(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {
            chart.ContextMenuStrip = null;
        }

        private void chart_DataItemOver(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {
            if (chart.ChartType == ChartType.PieChart || (chart.ChartType == ChartType.PieChart3D && !chart.PieChart.Concentric) ||
                chart.ChartType == ChartType.DoughnutChart || chart.ChartType == ChartType.DoughnutChart3D && !chart.DoughnutChart.Concentric)
            {
                chart.ContextMenuStrip = chartContextMenu;
                // Номер ряда == номер слоя
                idSlice = e.DataRow;
                labelSlice = e.RowLabel;

                // Получаем список рядов, помещенных в слой "Прочие"
                /*ArrayList othersList;
                if (Common.InfragistisUtils.Is3DChart(chart))
                {
                    othersList = ((Pie3DLayer)chart.Layer["Default"]).GetOthersListValue();
                }
                else
                {
                    othersList = ((PieLayer)chart.Layer["Default"]).GetOthersListValue();
                }
                isOtherSlice = othersList.Contains(Convert.ToDouble(idSlice));*/
            }
            else
            {
                chart.ContextMenuStrip = null; 
            }
        }

        private void chart_SizeChanged(object sender, EventArgs e)
        {
            //при растягивании диаграммы, когда область диаграммы становится < 0 вылазит AV, 
            //поетому нада запрещать задавать диаграмме такой размер
            if ((Chart.Layer["Default"] != null)&&(((ChartLayer)Chart.Layer["Default"]).OuterBound.Width < 0))
            {
                this.Chart.Width -= ((ChartLayer)Chart.Layer["Default"]).OuterBound.Width;
            }
            this.MainForm.PropertyGrid.Refresh();
        }

        private void chart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((chart.Legend.Location == LegendLocation.Top) || (chart.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = chart.Width - 20;
                }
                else
                {
                    widthLegendLabel = ((int)chart.Legend.SpanPercentage * chart.Width / 100) - 20;
                }

                widthLegendLabel -= chart.Legend.Margins.Left + chart.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }

        }

        private void chart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
            for (int i = 0; i < e.NullValueIndices.Length; i++)
            {
                e.NullValueIndices[i] = 0;
            }
        }

        void chart_MouseEnter(object sender, EventArgs e)
        {
            this.view3DAppearanceString = this.GetView3DAppearanceString(this.Chart.Transform3D);
        }

        void chart_MouseLeave(object sender, EventArgs e)
        {
            if ((this.view3DAppearanceString != string.Empty) &&
                (this.view3DAppearanceString != this.GetView3DAppearanceString(this.Chart.Transform3D)))
            {
                this.MainForm.Saved = false;
                this.view3DAppearanceString = string.Empty;
            }
        }

        private string GetView3DAppearanceString(View3DAppearance view3DAppearance)
        {
            string result = string.Empty;
            if (view3DAppearance != null)
            {
                result += view3DAppearance.EdgeSize.ToString();
                result += view3DAppearance.Light.ToString();
                result += view3DAppearance.Outline.ToString();
                result += view3DAppearance.Perspective.ToString();
                result += view3DAppearance.Scale.ToString();
                result += view3DAppearance.XRotation.ToString();
                result += view3DAppearance.YRotation.ToString();
                result += view3DAppearance.ZRotation.ToString();
            }
            return result;
        }

        #endregion

        #region  Руссификация сообщений диаграммы
        private void chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            if (e.Text != "")
            {
                string locMessage = GetLocalizeInvalidDataMessage(e.Text);
                if (locMessage != "")
                {
                    e.Text = locMessage;
                }
            }
        }

        /// <summary>
        /// Руссификация требования к диаграмме
        /// </summary>
        /// <param name="message">исходный текст требования</param>
        /// <returns>руссифицирование требование</returns>
        private string GetLocalizeInvalidDataMessage(string message)
        {
            string locMessage = message;

            if (Chart.ChartType == ChartType.PointChart3D)
            {
                message = "You must include one row and three numeric columns";
            }
            if (Chart.ChartType == ChartType.BubbleChart3D)
            {
                message = "You must include one row and four numeric columns";
            }

            foreach (string pattern in invalidDataMessages.Keys)
            {
                Regex regExp = new Regex(pattern);
                if (regExp.Match(message).Success)
                {
                    invalidDataMessages.TryGetValue(pattern, out locMessage);
                    return locMessage;
                }
            }

            return locMessage;
        }

        private static Dictionary<string, string> MakeInvalidDataMessages()
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();

            messages.Add("PieChart.ColumnIndex must be a numeric column", 
                         "Круговая диаграмма должна содержать не менее одной категории.");
            messages.Add("Stacked Bar Chart Error: You must have at least one row and one numeric column AND data should be either all positive or negative",
                         "Диаграмма должна содержать не менее одного ряда и одной категории. Все данные для каждого ряда и категории должны иметь либо положительные, либо отрицательные значения.");
            messages.Add("Stacked Column 3D Chart Error: You must have at least one row and one numeric column AND data should be either all positive or negative",
             "Диаграмма должна содержать не менее одного ряда и одной категории. Все данные для каждого ряда и категории должны иметь либо положительные, либо отрицательные значения.");
            messages.Add("Hierarchical Chart Error: There must be at least one row and one numeric column, and data must be all positive values.",
             "Диаграмма должна содержать не менее одного ряда и одной категории. Все данные должны иметь положительные значения.");
            messages.Add("There must be at least one row and one numeric column, and data should be all positive or negative.",
             "Диаграмма должна содержать не менее одного ряда и одной категории. Все данные для каждого ряда и категории должны иметь либо положительные, либо отрицательные значения.");
            messages.Add("You must include one row and three numeric columns",
                         "Диаграмма должна содержать не менее одного ряда и трех категорий.");
            messages.Add("You must include one row and four numeric columns",
                         "Диаграмма должна содержать не менее одного ряда и четырех категорий.");
            messages.Add("You must include at least one row and one numeric column",
                         "Диаграмма должна содержать не менее одного ряда и одной категории.");
            messages.Add("You must include at least one row and two numeric columns in Scatter Chart Appearance -ColumnX and ColumnY", 
                         "Диаграмма должна содержать не менее одного ряда и двух категорий. ");
            messages.Add("You must have at least two rows and two numeric column",
                         "Диаграмма должна содержать не менее двух рядов и двух категорий.");
            messages.Add("You must have at least one row and six columns, one date type and five numeric columns", 
                         "Диаграмма должна содержать не менее одного ряда и шести категорий, одна типа \"Дата\" и пять числовых.");
            messages.Add("You must include at least one row and two columns. First column as DateTime and second column as numeric values.", 
                         "Диаграмма должна содержать не менее одного ряда и двух категорий. Первая категория должна иметь тип \"Дата\\Время\", а вторая - числовое значение.");
            messages.Add("You must provide at least one item with a start and end time.", 
                         "Диаграмма должна содержать хотя бы один элемент с начальным и конечным временем.");
            messages.Add("You must include at least one row and two numeric columns matching settings for PolarChart.ColumnX and PolarChart.ColumnY",
                         "Диаграмма должна содержать не менее одного ряда и двух категорий.");
            messages.Add("To create a composite chart:",
                         "Чтобы создать композитную диаграмму: перетащите элементы из поля \"Доступные диаграммы отчета\" в поле \"Слои композитной диаграммы\".");
            messages.Add("At least 5 numeric columns are required.", 
                         "Диаграмма должна содержать не менее пяти категорий.");
            messages.Add("Chart Data error - please check the data for validity for this chart type.", 
                         "Ошибка данных диаграммы - пожалуйста, проверьте правильность данных для этого типа диаграмм.");
            messages.Add("You must provide at least one row and one numeric column.",
                         "Диаграмма должна содержать не менее одного ряда и одной категории.");
            messages.Add("when using the Series collection as a data source, at least one NumericSeries must be present.",
                         "Когда в качестве источника данных используется коллекция серий, должна быть представлена хотя бы одна числовая серия.");
            messages.Add("NumericAxisType = Logarithmic is not valid when UltraChart.Data.ZeroAligned = True.",
                         "При выравнивании по нулю невозможен логарифмический тип оси.");
            messages.Add("There must be at least one row and one numeric column.",
             "Диаграмма должна содержать не менее одного ряда и одной категории.");
            messages.Add("You must have at least one row and one numeric column",
             "Диаграмма должна содержать не менее одного ряда и одной категории.");
            return messages;
        }
        #endregion

        #region Установка пользовательских форматов

                    /// <summary>
        /// Получение DateTime из Hashtable
        /// </summary>
        /// <param name="dataValue">hashtable</param>
        /// <returns>значение в формате DateTime</returns>
        private static DateTime DoubleToDateTime(double dataValue)
        {
            DateTime dateTime;

            if (!DateTime.TryParse(dataValue.ToString(), out dateTime))
            {
                long uValue;
                if (long.TryParse(dataValue.ToString(), out uValue))
                {
                    dateTime = DateTime.FromBinary(uValue);
                }
            }

            return dateTime;
        }

        /// <summary>
        /// Установка пользовательских форматов
        /// </summary>
        private void SetCustomFormats()
        {
            Hashtable LabelHashTable = new Hashtable();
            LabelHashTable.Add("GENERAL", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("EXP", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("CRR", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("THS_CRR", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("THS_CRR_ND", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("MLN_CRR", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("MLN_CRR_ND", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("MLRD_CRR", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("MLRD_CRR_ND", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("PERCENT", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("PERCENT2", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("NUM", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("THS_NUM", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("MLN_NUM", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("MLRD_NUM", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("SHORT_DATE", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("LONG_DATE", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("SHORT_TIME", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("LONG_TIME", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("DATE_TIME", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("YESNO", new ChartLabelRenderer(this.chart));
            LabelHashTable.Add("TRUEFALSE", new ChartLabelRenderer(this.chart));

            this.chart.LabelHash = LabelHashTable;
        }

        #endregion

        /// <summary>
        /// Получение структуры из таблицы, если с ней синхронизирована карта
        /// </summary>
        /// <returns>true - если структура получена</returns>
        private bool GetSyncronizedPivotData(bool isForceDataUpdate)
        {
            if (this.Synchronization.BoundTo != String.Empty)
            {
                TableReportElement tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                if (tableElement != null)
                {
                    bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
                    this.PivotData.IsDeferDataUpdating = true;
                    Synchronize(tableElement.PivotData, false, !isForceDataUpdate);
                    this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;
                    return true;
                }

            }
            return false;
        }


        public void OnPivotDataChange()
        {
            GetSyncronizedPivotData(false);
            //Получаем данные с сервера
            RefreshData();
        }

        private void PivotData_AppearanceChanged(bool isNeedRecalculateGrid)
        {
            if (this.IsUpdatable)
            {
                if (this.CLS == null)
                {
                    RefreshData();
                }
                else
                {
                    this.InitialByCellSet();
                }
            }
        }

        void PivotData_StructureChanged()
        {
            //Обновляем диаграмму по ранее построенным данным
            if (this.IsUpdatable)
                this.InitialByCellSet();
        }

        private void PivotData_ElemOrderChanged(PivotAxis axis)
        {
            if (this.IsUpdatable)
            {
                if (this.CLS == null)
                {
                    RefreshData();
                }
                else
                {
                    this.InitialByCellSet();
                }
            }
            else
            {
                this.SourceDT = ChangeOrder(this.SourceDT, axis.AxisType);
            }
        }

        private void PivotData_ElemSortChanged(PivotAxis axis)
        {
            if (this.IsUpdatable)
            {
                if ((this.CLS == null)||(!axis.SortByName))
                {
                    RefreshData();
                }
                else
                {
                    this.InitialByCellSet();
                }
            }
            else
            {
                this.SourceDT = CheckElemOrder(this.SourceDT, axis.AxisType);
                if (axis.ReverseOrder)
                    this.SourceDT = ChangeOrder(this.SourceDT, axis.AxisType);
            }
        }


        private string GetSeparatorStr(DataSeriesSeparator sp)
        {
            switch (sp)
            {
                case DataSeriesSeparator.eComma: return ", ";
                case DataSeriesSeparator.eDotComma: return "; ";
                case DataSeriesSeparator.eNewLine: return "\n";
                case DataSeriesSeparator.eSpace: return " ";
                case DataSeriesSeparator.eStick: return " | ";
            }            
            return "; ";
        }

        protected override void RefreshData()
        {
            if (this.IsUpdatable)
            {
                base.RefreshData();
            }
        }

        /// <summary>
        /// Построение диаграммы по MDX-запросу
        /// </summary>
        protected override CellSet SetMDXQuery(string mdxQuery)
        {
            CellSet cls = null;
            if (this.IsCompositeChart)
                return cls;

            if (!IsUpdatable)
            {
                this.SetChartDataSource(this.SourceDT);
                return cls;
            }

            try
            {
                cls = base.SetMDXQuery(mdxQuery);
                this.InitialByCellSet(cls);
            }
            catch (Exception e)
            {
                chart.Data.ResetDataSource();
                chart.EmptyChartText = "";
                this.InitialByCellSet();
                Common.CommonUtils.ProcessException(e);
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
            DataTable dt = new DataTable();
            this.PopulateDataTableFromCellset(cls, ref dt);
            this.SourceDT = dt;  
        }


        public override void SetElementVisible(bool value)
        {
            if (this.chart.Visible != value)
            {
                this.chart.Visible = value;
                Application.DoEvents();
            }
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
        /// Перекачивает данные из селсета в стандартный датасет,
        /// который понимает диаграмма
        /// </summary>
        /// <param name="cls">Селлсет с данными запроса (входной формат)</param>
        /// <param name="dt">Наполняемый датасет (выходной формат)</param>
        private void PopulateDataTableFromCellset(CellSet cls, ref DataTable dt)
        {
            if (cls != null)
            {
                if (!this.PivotData.CheckConnection())
                    return;

                //Если есть серии, создаем столбец для их названий
                if (SeriesExist(cls))
                {
                    DataColumn dataColumn = dt.Columns.Add();
                    dataColumn.DataType = System.Type.GetType("System.String");
                    dataColumn.Caption = "Series Name";
                    dataColumn.ColumnName = "Series Name";
                }
                List<Member> collapsedMembers = GetSyncTableCollapsedMembers();

                PopulateColumnsFromCellset(cls, dt, collapsedMembers);
                PopulateSeriesFromCellset(cls, dt, collapsedMembers);                
                PopulateValuesFromCellset(cls, dt);

                dt = CheckElemOrder(cls, dt);
            }
        }
        
        private bool UseDimInLabels(Data.Axis ax, string hierarchy)
        {
            try
            {
                FieldSet fs = null;
                if ((ax.FieldSets.Count == 0) && (ax.AxisType == AxisType.atColumns))
                {
                    fs = ax.ParentPivotData.RowAxis.FieldSets.GetFieldSetByName(hierarchy);
                }
                else
                {
                    fs = ax.FieldSets.GetFieldSetByName(hierarchy);
                }
                return (fs != null) ? fs.UsedInChartLabels : false;
            }
            catch 
            {
                return false;                
            }
        }

        /// <summary>
        /// Нужно ли строки выбирать как столбцы?
        /// </summary>
        private bool RowsGoesAsColumns()
        {
            ///1) Это может понадобиться только тогда, когда в запросе должна быть ось строк,
            /// а ось столбцов отсутствует. Ситуация для MDX не допустимая.
            /// Поэтому, что бы хоть что-то выводить эти самые строки будем запихивать в 
            /// отсутствующие столбцы.
            /// 2) Делать это будем только для диаграммы, поскольку для таблицы в этой ситуации
            /// создается фиктивная мера-пустышка. Поэтому в таблице ось столбцов есть всегда
            return (this.PivotData.ColumnAxis.FieldSets.Count == 0);
        }

        private bool IsExistsGrandTotal(AxisType axisType)
        {
            //Если в запросе строки распологаются в оси колонок, то существование 
            //главного итога будем смотреть у строк
            if (this.RowsGoesAsColumns())
                axisType = AxisType.atRows;

            switch (axisType)
            {
                case AxisType.atColumns: return this.PivotData.ColumnAxis.GrandTotalExists;
                case AxisType.atRows: return this.PivotData.RowAxis.GrandTotalExists;
            }
            return false;
        }

        private bool IncludeInLableParentMember(AxisType axisType)
        {
            //Если в запросе строки распологаются в оси колонок, то существование 
            //главного итога будем смотреть у строк
            if (this.RowsGoesAsColumns())
                axisType = AxisType.atRows;

            switch (axisType)
            {
                case AxisType.atColumns: return this.PivotData.ColumnAxis.IncludeInChartLabelParentMember;
                case AxisType.atRows: return this.PivotData.RowAxis.IncludeInChartLabelParentMember;
            }
            return false;
        }

        /// <summary>
        /// Перекачивает названия серий из селсета в датасет
        /// </summary>
        private void PopulateSeriesFromCellset(CellSet cls, DataTable dt, List<Member> collapsedMembers)
        {
            if (SeriesExist(cls))
            {
                string seriesName;
                int counter;
                int positionCount = cls.Axes[1].Positions.Count;
                DimensInfo dimensionsInfo = new DimensInfo(this.PivotData);
                //вычислим номера последних уровней
                int[] lastLevelNumbers = this.GetLastLevelNumbers(cls.Axes[1].Positions);
                //найдем UN всех листовых элементов
                List<string> leafMemberUN = this.GetLeafMemberUN(cls.Axes[1].Positions, lastLevelNumbers);
                this.rowPosNum.Clear();

                //добавим схлопнутые элементы к листовым
                foreach (Member mbr in collapsedMembers)
                {
                    leafMemberUN.Add(mbr.UniqueName);
                }

                for (int i = 0; i < cls.Axes[1].Positions.Count; i++)
                {
                    Position pos = cls.Axes[1].Positions[i];
                    counter = 0;
                    seriesName = string.Empty;
                    dimensionsInfo.AddInfo(pos);
                    bool isAppendPos = false;
                    bool isHideMember = false;

                    //Если это общий итог, то и название соответствующее
                    if ((pos.Ordinal == positionCount - 1) && this.IsExistsGrandTotal(AxisType.atRows))
                        seriesName = Common.Consts.grandTotalCaption;
                    else
                    {
                        if (this.IsAppendPosition(pos, leafMemberUN))
                        {
                            isAppendPos = true;

                            foreach (Member mem in pos.Members)
                            {
                                if (IsHideMember(mem, collapsedMembers) || IsExceptedMember(mem, PivotData.RowAxis))
                                {
                                    seriesName = string.Empty;
                                    isHideMember = true;
                                    break;
                                }

                                if (UseDimInLabels(PivotData.RowAxis, mem.ParentLevel.ParentHierarchy.UniqueName))
                                {
                                    if (seriesName != string.Empty)
                                    {
                                        seriesName += GetSeparatorStr(rowsSeparator);
                                    }

                                    if (this.IncludeInLableParentMember(AxisType.atRows))
                                        seriesName += dimensionsInfo.GetAllLastMemberCaption(mem, GetSeparatorStr(rowsSeparator));
                                    else
                                    {
                                        PivotTotal total = this.PivotData.TotalAxis.GetTotalByName(mem.UniqueName);
                                        //seriesName += (total != null) ? total.Caption : mem.Caption;
                                        seriesName += (total != null) ? total.Caption : CommonUtils.GetMemberCaptionWithoutID(mem);

                                    }
                                }
                                counter++;
                            }
                        }
                    }

                    //if (String.IsNullOrEmpty(seriesName))
                    //    seriesName = GetRowName(dt.Rows, seriesName);

                    if (((seriesName != string.Empty) || (isAppendPos)) && (!isHideMember))
                    {
                        this.rowPosNum.Add(pos.Ordinal);
                        dt.Rows.Add(seriesName);
                    }
                }


            }
        }

        /// <summary>
        /// В датасете диаграммы по селсету создает столбцы (категории)
        /// Данными не заполняет, только пустые столбцы
        /// </summary>
        private void PopulateColumnsFromCellset(CellSet cls, DataTable dt, List<Member>collapsedMembers)
        {
            DataColumn dataColumn;
            int counter;            
            if (cls.OlapInfo.AxesInfo.Axes.Count > 0)
            {
                //будем помечать ячейки которые не при каком раскладе не должны отображаться 
                //в диаграмме
                const string hideCell = "hideCell";
                string columnName;
                int positionCount = cls.Axes[0].Positions.Count;
                DimensInfo dimensionsInfo = new DimensInfo(this.PivotData);
                //вычислим номера последних уровней
                int[] lastLevelNumbers = this.GetLastLevelNumbers(cls.Axes[0].Positions);
                //найдем UN всех листовых элементов
                List<string> leafMemberUN = this.GetLeafMemberUN(cls.Axes[0].Positions, lastLevelNumbers);
                this.columnPosNum.Clear();

                //пометим схлопнутые элементы как листовые
                foreach(Member mbr in collapsedMembers)
                {
                    leafMemberUN.Add(mbr.UniqueName);
                }


                for (int i = 0; i < cls.Axes[0].Positions.Count; i++)
                {
                    Position pos = cls.Axes[0].Positions[i];

                    counter = 0;
                    columnName = string.Empty;
                    dimensionsInfo.AddInfo(pos);

                    //Если это общий итог, то и название соответствующее
                    if ((pos.Ordinal == positionCount - 1) && this.IsExistsGrandTotal(AxisType.atColumns))
                        columnName = Common.Consts.grandTotalCaption;
                    else
                    {
                        if (this.IsAppendPosition(pos, leafMemberUN))
                        {
                            foreach (Member mem in pos.Members)
                            {
                                if (IsHideMember(mem, collapsedMembers) || IsExceptedMember(mem, PivotData.ColumnAxis))
                                {
                                    columnName = hideCell;
                                    break;
                                }

                                if (UseDimInLabels(PivotData.ColumnAxis, mem.ParentLevel.ParentHierarchy.UniqueName))
                                {
                                    if (columnName != string.Empty)
                                    {
                                        columnName += GetSeparatorStr(columnsSeparator);
                                    }

                                    if (this.IncludeInLableParentMember(AxisType.atColumns))
                                        columnName += dimensionsInfo.GetAllLastMemberCaption(mem,
                                                                                             GetSeparatorStr(
                                                                                                 columnsSeparator));
                                    else
                                    {
                                        PivotTotal total = this.PivotData.TotalAxis.GetTotalByName(mem.UniqueName);
                                        //columnName += (total != null) ? total.Caption : mem.Caption;
                                        columnName += (total != null) ? total.Caption : CommonUtils.GetMemberCaptionWithoutID(mem);

                                    }
                                }
                                counter++;
                            }
                        }
                        else
                            columnName = hideCell;
                    }

                    if (columnName != hideCell)
                    {
                        this.columnPosNum.Add(pos.Ordinal);
                        dataColumn = dt.Columns.Add();
                        dataColumn.DataType = typeof (Decimal);
                        dataColumn.Caption = this.GetColumnName(dt.Columns, columnName);
                        if (dataColumn.Caption != string.Empty)
                            dataColumn.ColumnName = dataColumn.Caption;
                    }
                }
            }
        }

        /// <summary>
        /// Получение схлопнутых элементов таблицы, по которой строится диаграмма
        /// </summary>
        /// <returns></returns>
        private List<Member> GetSyncTableCollapsedMembers()
        {
            string boundTo = this.Synchronization.BoundTo;
            TableReportElement tableElement = this.MainForm.FindTableReportElement(boundTo);
            ExpertGrid expertGrid = null;
            List<Member> collapsedMembers = new List<Member>();
            if (tableElement != null)
            {
                expertGrid = tableElement.ExpertGrid;
                collapsedMembers = expertGrid.Row.AllCollapsedMembers();
                collapsedMembers.AddRange(expertGrid.Column.AllCollapsedMembers());
            }
            return collapsedMembers;
        }

        /// <summary>
        /// Проверяем есть ли у мембера схлопнутые предки 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="collapsedMembers"></param>
        /// <returns></returns>
        private bool IsHideMember(Member member, List<Member> collapsedMembers)
        {
            foreach(Member mbr in collapsedMembers)
            {
                if(IsAncestor(mbr, member))
                {
                    return true;
                }
            }
            return false;
        }
            
        /// <summary>
        /// Отключен ли элемент в редакторе элементов измерения
        /// </summary>
        /// <param name="member"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        private bool IsExceptedMember(Member member, Data.PivotAxis axis)
        {
            foreach(FieldSet fs in axis.FieldSets)
            {
                if(fs.ExceptedMembers.Contains(member.UniqueName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Если первый элемент является предком второго, вернет true
        /// </summary>
        /// <param name="ancestor">предок</param>
        /// <param name="descendant">предпологаемый потомок</param>
        /// <returns></returns>
        private bool IsAncestor(Member ancestor, Member descendant)
        {
            if (ancestor.UniqueName == descendant.UniqueName)
                return false;

            while ((descendant.Parent != null) && (ancestor.LevelDepth <= descendant.Parent.LevelDepth))
            {
                if (ancestor.UniqueName == descendant.Parent.UniqueName)
                    return true;
                descendant = descendant.Parent;
            }
            return false;
        }

        /// <summary>
        /// Проверяет на соответствие видимости нелистовых элементов (итогов) в каждом измерении оси
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="leafMemberUN"></param>
        /// <returns></returns>
        private bool IsAppendPosition(Position pos, List<string> leafMemberUN)
        {
            foreach (Member mem in pos.Members)
            {
                FieldSet fieldSet = this.PivotData.GetFieldSet(mem.ParentLevel.ParentHierarchy.UniqueName);
                if ((fieldSet != null) && !fieldSet.IsVisibleTotals)
                {
                    //если надо показывать только листовые элементы, проверим содержиться ли данный 
                    //элемент в списке таковых
                    if (!leafMemberUN.Contains(mem.UniqueName))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Получаем номера последних включенных в выборку уровней  у каждого измерения в оси
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        private int[] GetLastLevelNumbers(PositionCollection pc)
        {
            if ((pc != null) && (pc.Count > 0))
            {
                //получаем количество измерений
                int[] result = new int[pc[0].Members.Count];
                foreach (Position pos in pc)
                {
                    for (int i = 0; i < pos.Members.Count; i++)
                    {
                        Member mem = pos.Members[i];
                        result[i] = Math.Max(result[i], mem.ParentLevel.LevelNumber);
                    }
                }
                return result;
            }
            return new int[0];
        }

        /// <summary>
        /// Получить UN листовых элементов
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        private List<string> GetLeafMemberUN(PositionCollection pc, int[] lastLevelNumbers)
        {
            List<string> result = new List<string>();

            if ((pc != null) && (pc.Count > 0))
            {
                foreach (Position pos in pc)
                {
                    for (int i = 0; i < pos.Members.Count; i++)
                    {
                        Member mem = pos.Members[i];
                        if ((mem.ChildCount == 0) || (mem.ParentLevel.LevelNumber == lastLevelNumbers[i]))
                            result.Add(mem.UniqueName);
                    }
                }
            }
            return result;
        }

        private string GetColumnName(DataColumnCollection columns, string columnName)
        {
            if (String.IsNullOrEmpty(columnName))
            {
                columnName = "Категория";
            }

            string result = columnName;
            if (columns == null)
                return result;
            
            bool isNameExist = false;


            int i = 1;
            do
            {
                isNameExist = (columns.IndexOf(result) != -1);
                if (isNameExist)
                {
                    result = string.Format("{0} ({1})", columnName, i.ToString());
                    i++;
                }
            }
            while (isNameExist);
            return result;
        }

        private string GetRowName(DataRowCollection rows, string rowName)
        {
            if (String.IsNullOrEmpty(rowName))
            {
                rowName = "Ряд";
            }

            string result = rowName;
            if (rows == null)
                return result;

            bool isNameExist = false;


            int i = 1;
            do
            {
                isNameExist = false;
                foreach(DataRow row in rows)
                {
                    if ((row.ItemArray.Length > 0) && (row[0] != DBNull.Value))
                    {
                        if (result == (string) row[0])
                        {
                            isNameExist = true;
                            break;
                        }
                    }
                }
                
                if (isNameExist)
                {
                    result = string.Format("{0} ({1})", rowName, i.ToString());
                    i++;
                }
            }
            while (isNameExist);
            return result;
        }


        private object GetCellValue(CellCollection cells, int index1, int index2)
        {
            try
            {
                return cells[index1, index2].Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        private object GetCellValue(CellCollection cells, int index)
        {
            try
            {
                return cells[index].Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Перекачивает значения из селсета в датасет диаграммы. Ранее в columnPosNum, rowPosNum 
        /// сохранили номера кортежей из которых будем брать данные
        /// </summary>
        private void PopulateValuesFromCellset(CellSet cls, DataTable dt)
        {
            object[] values;

            if (SeriesExist(cls))
            {
                for (int r = 0; r < this.rowPosNum.Count; r++)
                {
                    values = dt.Rows[r].ItemArray;
                    for (int c = 0; c < this.columnPosNum.Count; c++)
                    {
                        values[c + 1] = this.GetCellValue(cls.Cells, this.columnPosNum[c], this.rowPosNum[r]);
                        
                        //если вдруг попалось строковое значение - записываем вместо него null
                        if ((values[c + 1] != null)&&(values[c + 1].GetType() == typeof(string)))
                            values[c + 1] = null;
                    }

                    try
                    {
                        dt.Rows[r].ItemArray = values;
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                if (cls.Cells.Count > 0)
                {
                    values = new object[this.columnPosNum.Count];
                    for (int c = 0; c < values.Length; c++)
                    {
                        values[c] = this.GetCellValue(cls.Cells, this.columnPosNum[c]);
                    }

                    #warning Нет обработки. см камент ниже...
                    //При занесении строки в датасет может возникнуть исключение, 
                    //если в фильтрах была задействована мера у которой не числовые
                    //значения (есть вычислимые меры со строковыми значениями).
                    //Идет исключение из-за того, что в дата-тэйбле проставлен
                    //числовой тип. Сейчас просто затраено, что делать пока не понятно.                    
                    try
                    {
                        dt.Rows.Add(values);
                    }
                    catch
                    {
                    }
                }
            }
        }


        private DataTable CheckElemOrder(CellSet cls, DataTable dt)
        {
            if (cls.OlapInfo.AxesInfo.Axes.Count > 0)
            {
                bool sortByName = RowsGoesAsColumns()
                                        ? this.PivotData.RowAxis.SortByName
                                        : this.PivotData.ColumnAxis.SortByName;
                if (sortByName)
                {
                    SortType sortType = RowsGoesAsColumns()
                                        ? this.PivotData.RowAxis.SortType
                                        : this.PivotData.ColumnAxis.SortType;
                    if ((sortType == SortType.ASC)||(sortType == SortType.BASC))
                    {
                        dt = SortColumns(dt, true, "ASC");
                    }
                    if ((sortType == SortType.DESC) || (sortType == SortType.BDESC))
                    {
                        dt = SortColumns(dt, true, "DESC");
                    }
                }

                bool reverseOrder = RowsGoesAsColumns()
                                        ? this.PivotData.RowAxis.ReverseOrder
                                        : this.PivotData.ColumnAxis.ReverseOrder;
                if (reverseOrder)
                {
                    dt = ChangeOrder(dt, AxisType.atColumns);
                }
            }

            if (SeriesExist(cls))
            {
                if (this.PivotData.RowAxis.SortByName)
                {
                    if (dt.Columns.Count > 0)
                    {
                        if ((this.PivotData.RowAxis.SortType == SortType.ASC) || (this.PivotData.RowAxis.SortType == SortType.BASC))
                        {
                            dt = SortTableByColumn(dt, dt.Columns[0].ColumnName, "ASC");
                        }
                        if ((this.PivotData.RowAxis.SortType == SortType.DESC) || (this.PivotData.RowAxis.SortType == SortType.BDESC))
                        {
                            dt = SortTableByColumn(dt, dt.Columns[0].ColumnName, "DESC");
                        }
                        
                    }
                }

                if (this.PivotData.RowAxis.ReverseOrder)
                {
                    dt = ChangeOrder(dt, AxisType.atRows);
                }
            }
            return dt;
        }

        /// <summary>
        /// Прошерстим DataTable в поисках отрицательного или пустого значения
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool GetNegativeOrEmptyValueExists(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    if (item != null)
                    {
                        decimal dValue;
                        if (decimal.TryParse(item.ToString(), out dValue))
                        {
                            if (dValue < 0)
                                return true;
                        }
                        if (item.ToString() == string.Empty)
                            return true;
                    }
                }
            }

            return false;
        }

        private void SetChartDataSource(DataTable dt)
        {
            this.IsNotValidData = this.GetNegativeOrEmptyValueExists(dt);
            this.Chart.DataSource = dt;
        }

        /// <summary>
        /// Получить изображение для печати, если у диаграммы стоит признак растягивать, 
        /// значит уместим изображение в указаные границы, если его не стоит, значит получаем полное 
        /// изображение элемента (вероятно оно будет напечатано на нескольиких страницах)
        /// </summary>
        /// <param name="imageBounds"></param>
        /// <returns></returns>
        public override Bitmap GetPrintableImage(Rectangle pageBounds)
        {
            return (this.Chart.Dock == DockStyle.Fill) ? this.GetBitmap(pageBounds) : this.GetBitmap();
        }

        /// <summary>
        /// Получить полное изображение элемента
        /// </summary>
        /// <returns></returns>
        public override Bitmap GetBitmap()
        {
            Rectangle fullElementBounds = this.ClientRectangle;
            fullElementBounds.Width -= this.ElementPlace.Width;
            fullElementBounds.Height -= this.ElementPlace.Height;

            fullElementBounds.Width += this.Chart.Size.Width;
            fullElementBounds.Height += this.Chart.Size.Height;

            fullElementBounds.Width = Math.Max(this.ClientRectangle.Width, fullElementBounds.Width);
            fullElementBounds.Height = Math.Max(this.ClientRectangle.Height, fullElementBounds.Height);
            return base.GetBitmap(fullElementBounds);
        }

        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Разделитель элементов серий
        /// </summary>
        public DataSeriesSeparator RowsSeparator
        {
            get { return rowsSeparator; }
            set
            {
                rowsSeparator = value;
                OnPivotDataChange();
            }
        }

        /// <summary>
        /// Разделитель элементов категорий
        /// </summary>
        public DataSeriesSeparator ColumnsSeparator
        {
            get { return columnsSeparator; }
            set
            {
                columnsSeparator = value;
                OnPivotDataChange();
            }
        }

        /// <summary>
        /// Т.к. этот элемент отчета не содержит данного интерфейса, возвращаем null
        /// </summary>
        public override IGridUserInterface GridUserInterface
        {
            get { return null; }
        }

        public UltraChart Chart
        {
            get { return chart; }
        }

        /// <summary>
        /// Признак что данные берутся из базы
        /// </summary>
        public bool IsUpdatable
        {
            get { return _isUpdatable; }
            set 
            {
                if (_isUpdatable != value)
                {
                    _isUpdatable = value;
                    this.Invalidate(true);
                }
            }
        }

        public DataTable SourceDT
        {
            get { return _sourceDT; }
            set 
            { 
                _sourceDT = value;
                this.SetChartDataSource(value);

                // устанавливаем метки для TreeMapChart
                this.UpdateTreeMapLabels(value);
            }
        }

        public override bool IsShowErrorMessage
        {
            get 
            {
                return !this.isComposite && (base.IsCubeNotFond && this.IsUpdatable)
                    || (this.IsNotValidData && this.LogTypeEnable &&
                    (this.Chart.Axis.Y.NumericAxisType == NumericAxisType.Logarithmic 
                    || this.Chart.Axis.X.NumericAxisType == NumericAxisType.Logarithmic
                    || this.Chart.Axis.X2.NumericAxisType == NumericAxisType.Logarithmic
                    || this.Chart.Axis.Y2.NumericAxisType == NumericAxisType.Logarithmic)); 
            }
        }

        /// <summary>
        /// Если для усновленного типа диаграммы пременим логарифмический тип оси, вернет  true
        /// </summary>
        public bool LogTypeEnable
        {
            get
            {
                string chartType = this.Chart.ChartType.ToString();
                return (chartType == "SplineAreaChart") || (chartType == "StackAreaChart") 
                    || (chartType == "StackSplineAreaChart") || (chartType == "StepAreaChart") 
                    || (chartType == "BarChart") || (chartType == "StackBarChart") 
                    || (chartType == "GanttChart") || (chartType == "BubbleChart")
                    || (chartType == "CandleChart") || (chartType == "ColumnChart")
                    || (chartType == "AreaChart") || (chartType == "HeatMapChart")
                    || (chartType == "ColumnLineChart") || (chartType == "StackColumnChart")
                    || (chartType == "LineChart") || (chartType == "ScatterLineChart")
                    || (chartType == "SplineChart") || (chartType == "StackLineChart")
                    || (chartType == "StackSplineChart") || (chartType == "StepLineChart")
                    || (chartType == "PolarChart") || (chartType == "ParetoChart")
                    || (chartType == "RadarChart") || (chartType == "BoxChart")
                    || (chartType == "ScatterChart") || (chartType == "ProbabilityChart");
            } 
        }

        public bool IsNotValidData
        {
            get { return _isExistsNegativeValue; }
            set 
            {
                base.ErrorMessageText = "При логарифмическом типе оси у отметок, не должно быть отрицательных или пустых данных. Поменяйте тип оси на линейный, или удалите из данных отрицательные и пустые значения.";
                if (value != _isExistsNegativeValue)
                {
                    _isExistsNegativeValue = value;
                    this.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Является ли диаграмма композитной
        /// </summary>
        public bool IsCompositeChart
        {
            get { return isComposite; }
            set { isComposite = value; }
        }

        /// <summary>
        /// Проверка индекса категории
        /// </summary>
        /// <param name="dataTable">таблица данных</param>
        /// <param name="value">индекс категории</param>
        /// <returns>корректный индекс</returns>
        public static int CheckColumnIndexValue(DataTable dataTable, int value)
        {
            if (dataTable == null || value < 0)
            {
                return 0;
            }
            if (value > dataTable.Columns.Count)
            {
                return dataTable.Columns.Count;
            }
            return value;
        }

        /// <summary>
        /// Получение имени категории
        /// </summary>
        /// <param name="dataTable">таблица данных</param>
        /// <param name="index">индекс категории</param>
        /// <returns>имя категории</returns>
        public static string GetColumnName(DataTable dataTable, int index)
        {
            if ((dataTable == null) || (dataTable.Columns.Count == 0))
            {
                return string.Empty;
            }
            return dataTable.Columns[index].Caption;
        }

        /// <summary>
        /// Сортировка таблицы
        /// </summary>
        /// <param name="table">таблица, которую сортируем</param>
        /// <param name="columnName">имя столбца, по которому сортируем</param>
        /// <param name="sortType">тип сортировки - ASC или DESC</param>
        /// <returns>отсортированная таблица</returns>
        public static DataTable SortTableByColumn(DataTable table, string columnName, string sortType)
        {
            DataView view = table.DefaultView;
            view.Sort = String.Format("{0} {1}", columnName, sortType);
            return view.ToTable();
        }

        /// <summary>
        /// Сортировка столбцов таблицы(порядок следования столбцов)
        /// </summary>
        /// <param name="table">таблица</param>
        /// <param name="sortType">тип сортировки - ASC или DESC</param>
        /// <returns>отсортированная таблица</returns>
        public static DataTable SortColumns(DataTable table, bool sortByName, string sortType)
        {
            List<string> colNames = new List<string>();

            if (sortByName)
            {

                for (int i = 1; i < table.Columns.Count; i++)
                {
                    colNames.Add(table.Columns[i].ColumnName);
                }
                colNames.Sort(StringComparer.Ordinal);

            }
            else
            {
                List<decimal> values = new List<decimal>();

                if (table.Rows.Count > 0)
                {
                    for (int i = 1; i < table.Columns.Count; i++)
                    {
                        if (table.Rows[0][i] != DBNull.Value)
                            values.Add((Decimal) table.Rows[0][i]);
                    }
                }
                values.Sort();
                for (int i = 0; i < values.Count; i++)
                {
                    for (int k = 1; k < table.Columns.Count; k++)
                    {
                        if ((decimal) table.Rows[0][k] == values[i])
                            colNames.Add(table.Columns[k].ColumnName);
                        //table.Columns[k].SetOrdinal(i + 1);
                    }

                }
            }

            if (sortType == "ASC")
            {
                for (int i = 0; i < colNames.Count; i++)
                {
                    table.Columns[colNames[i]].SetOrdinal(i + 1);
                }
            }
            if (sortType == "DESC")
            {
                for (int i = colNames.Count - 1; i > -1; i--)
                {
                    table.Columns[colNames[i]].SetOrdinal(colNames.Count - i);
                }
            }

            return table;
        }

        /// <summary>
        /// Меняет порядок следования строк или столбцов в таблице на обратный
        /// </summary>
        /// <param name="table">таблица</param>
        /// <param name="axisType">"rows" или "columns"</param>
        /// <returns></returns>
        public static DataTable ChangeOrder(DataTable table, AxisType axisType)
        {
            if (axisType == AxisType.atRows)
            {
                DataTable result = new DataTable();
                foreach (DataColumn column in table.Columns)
                {
                    result.Columns.Add(column.ColumnName, column.DataType);
                }
                for(int i = table.Rows.Count - 1; i > -1; i--)
                {
                    result.Rows.Add(table.Rows[i].ItemArray);
                }
                return result;
            }

            if(axisType == AxisType.atColumns)
            {
                List<string> colNames = new List<string>();
                for(int i = 1; i < table.Columns.Count; i++)
                {
                    colNames.Add(table.Columns[i].ColumnName);
                }
                for (int i = colNames.Count - 1; i > -1; i--)
                {
                    table.Columns[colNames[i]].SetOrdinal(colNames.Count - i);
                }

            }
            return table;
        }

        public static DataTable ChangeSort(DataTable table, AxisType axisType)
        {
            if (axisType == AxisType.atRows)
            {

                DataTable result = new DataTable();
                foreach (DataColumn column in table.Columns)
                {
                    result.Columns.Add(column.ColumnName, column.DataType);
                }
                for (int i = table.Rows.Count - 1; i > -1; i--)
                {
                    result.Rows.Add(table.Rows[i].ItemArray);
                }
                return result;
            }

            if (axisType == AxisType.atColumns)
            {
                List<string> colNames = new List<string>();
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    colNames.Add(table.Columns[i].ColumnName);
                }
                for (int i = colNames.Count - 1; i > -1; i--)
                {
                    table.Columns[colNames[i]].SetOrdinal(colNames.Count - i);
                }

            }
            return table;
        }

        private DataTable CheckElemOrder(DataTable dt, AxisType axisType)
        {
            if (axisType == AxisType.atColumns)
            {
                bool sortByName = this.PivotData.ColumnAxis.SortByName;
                SortType sortType = this.PivotData.ColumnAxis.SortType;
                if ((sortType == SortType.ASC) || (sortType == SortType.BASC))
                {
                    dt = SortColumns(dt, sortByName, "ASC");
                }
                if ((sortType == SortType.DESC) || (sortType == SortType.BDESC))
                {
                    dt = SortColumns(dt, sortByName, "DESC");
                }

            }

            if (axisType == AxisType.atRows)
            {
                if (dt.Columns.Count > 0)
                {
                    if (this.PivotData.RowAxis.SortType != SortType.None)
                    {
                        int columnNumber = this.PivotData.RowAxis.SortByName ? 0 : 1;

                        if ((this.PivotData.RowAxis.SortType == SortType.ASC) ||
                            (this.PivotData.RowAxis.SortType == SortType.BASC))
                        {
                            dt = SortTableByColumn(dt, dt.Columns[columnNumber].ColumnName, "ASC");
                        }
                        if ((this.PivotData.RowAxis.SortType == SortType.DESC) ||
                            (this.PivotData.RowAxis.SortType == SortType.BDESC))
                        {
                            dt = SortTableByColumn(dt, dt.Columns[columnNumber].ColumnName, "DESC");
                        }
                    }
                }
            }
            return dt;
        }


        /// <summary>
        /// Обновление меток TreeMapChart
        /// </summary>
        /// <param name="dataTable">таблица данных</param>
        public void UpdateTreeMapLabels(DataTable dataTable)
        {
            if (dataTable == null)
            {
                return;
            }

            int sizeIndex = CheckColumnIndexValue(dataTable, chart.TreeMapChart.SizeValueIndex);
            chart.TreeMapChart.SizeValueLabel = GetColumnName(dataTable, sizeIndex);

            int colorIndex = CheckColumnIndexValue(dataTable, chart.TreeMapChart.ColorValueIndex);
            chart.TreeMapChart.ColorValueLabel = GetColumnName(dataTable, colorIndex);
        }

        #region Вспомогательные классы

        private class ChartLabelRenderer : IRenderLabel
        {
            private UltraChart _chart;

            public ChartLabelRenderer(UltraChart chart) :base()
            {
                this._chart = chart;
            }

            private string GetDataValueType(ref string renderFormat)
            {
                string result = "";

                int beginValueType = renderFormat.IndexOf("[");
                if (beginValueType > -1)
                {
                    int endValueType = renderFormat.IndexOf("]");
                    if (endValueType > beginValueType)
                    {
                        result = renderFormat.Substring(beginValueType, endValueType - beginValueType + 1);
                        renderFormat = renderFormat.Remove(beginValueType, endValueType - beginValueType + 1);
                    }
                }

                switch (result)
                {
                    case "[MIN]":
                        result = "DATA_VALUE_MIN";
                        break;
                    case "[MAX]":
                        result = "DATA_VALUE_MAX";
                        break;
                    case "[Q1]":
                        result = "DATA_VALUE_Q1";
                        break;
                    case "[Q2]":
                        result = "DATA_VALUE_Q2";
                        break;
                    case "[Q3]":
                        result = "DATA_VALUE_Q3";
                        break;
                    case "[X]":
                        result = "DATA_VALUE_X";
                        break;
                    case "[Y]":
                        result = "DATA_VALUE_Y";
                        break;
                    case "[TOPLEFT]":
                        result = "DATA_VALUE_TOPLEFT";
                        break;
                    case "[TOPRIGHT]":
                        result = "DATA_VALUE_TOPRIGHT";
                        break;
                    case "[BOTTOMLEFT]":
                        result = "DATA_VALUE_BOTTOMLEFT";
                        break;
                    case "[BOTTOMRIGHT]":
                        result = "DATA_VALUE_BOTTOMRIGHT";
                        break;
                    case "[ITEM]":
                        result = "DATA_VALUE_ITEM";
                        break;
                    case "[RADIUS]":
                        result = "DATA_VALUE_RADIUS";
                        break;
                    case "[DEGREES]":
                        result = "DEGREES_VALUE";
                        break;
                    case "[RADIANS]":
                        result = "RADIANS_VALUE";
                        break;
                    case "":
                        result = "DATA_VALUE";
                        break;
                }


                return result;
            }

            //Получение процентного значения для диаграммы с накоплением
            private string GetStackChartPercentValue(Hashtable context)
            {
                string columnName = (string)context["ITEM_LABEL"];
                DataTable dt = (DataTable)this._chart.DataSource;


                
                /*
                
                if (context["DATA_ROW"] == null)
                    return "test";

                int rowIndex = (int) context["DATA_ROW"];
                double sum = 0;
                for(int i = 1; i < dt.Columns.Count; i++)
                {
                    sum += (double)dt.Rows[rowIndex][i];
                }
                */
                object sum = Convert.ChangeType(dt.Compute(string.Format("sum([{0}])", columnName), null), typeof(double));
                
                
                
                double totalValue = sum != null ? ((double)sum) : 0;

                if (context["DATA_VALUE_ITEM"] != null)
                {
                    double dataValueItem = (double)context["DATA_VALUE_ITEM"];
                    double percentage = (((dataValueItem > 0) && (totalValue > 0)) ? dataValueItem / totalValue : 0);
                    return (String.Format("{0}{1}({2:0.0%})", dataValueItem, Environment.NewLine, percentage));
                }

                return ((double)context["DATA_VALUE_ITEM"]).ToString();
            }

            public string ToString(Hashtable context)
            {
                string format = (string)context["RENDER_FORMAT"];

                string dataValueType = GetDataValueType(ref format);

                double dataValue = Double.NaN;

                try
                {
                    if (context[dataValueType] == null)
                    {
                        if (context["DATA_VALUE"] == null)
                        {
                            dataValueType = "DATA_VALUE_ITEM";
                        }
                        else
                        {
                            dataValueType = "DATA_VALUE";
                        }
                    }

                    if (context[dataValueType] is double)
                    {
                        dataValue = (double)context[dataValueType];
                    }

                    
                    if (context[dataValueType] is decimal)
                    {
                        decimal value = (decimal)context[dataValueType];
                        dataValue = (double)value;
                    }
                    
                }
                catch
                {
                    return "";
                }

                string key = (string)context["RENDER_KEYWORD"];
                DateTime dateTime;

                if (dataValue != Double.NaN)
                {
                    switch (key)
                    {
                        case "GENERAL":
                            return dataValue.ToString("g");
                       /* case "PERCENT":
                            if ((this._chart.ChartType == ChartType.StackBarChart) ||
                                (this._chart.ChartType == ChartType.StackColumnChart))
                            {
                                return GetStackChartPercentValue(context);
                            }
                            break;*/
                        case "PERCENT2":
                            dataValue *= 100;
                            break;
                        case "SHORT_DATE":
                            dateTime = DoubleToDateTime(dataValue);
                            return dateTime.ToShortDateString();

                        case "LONG_DATE":
                            dateTime = DoubleToDateTime(dataValue);
                            return dateTime.ToLongDateString();

                        case "SHORT_TIME":
                            dateTime = DoubleToDateTime(dataValue);
                            return dateTime.ToShortTimeString();

                        case "LONG_TIME":
                            dateTime = DoubleToDateTime(dataValue);
                            return dateTime.ToLongTimeString();

                        case "DATE_TIME":
                            dateTime = DoubleToDateTime(dataValue);
                            return dateTime.ToString();

                        case "YESNO":
                            return dataValue.ToString("\"Да\";\"Да\";\"Нет\"");
                        case "TRUEFALSE":
                            return dataValue.ToString("\"Истина\";\"Истина\";\"Ложь\"");
                    }

                    return dataValue.ToString(format);
                }
                return string.Empty;
            }
        }

        private class DimensInfo
        {
            private List<DimInfo> _dimensionsInfo;
            private PivotData pivotData;

            public DimensInfo(PivotData pData)
            {
                this.pivotData = pData;
                this.DimensionsInfo = new List<DimInfo>();
            }

            public PivotData PivotData
            {
                get { return this.pivotData; }
            }

            public void AddInfo(Position pos)
            {
                if (pos == null)
                    return;

                foreach (Member mem in pos.Members)
                {
                    string dimUN = mem.ParentLevel.ParentHierarchy.UniqueName;
                    int levelNumber = mem.ParentLevel.LevelNumber;
                    bool isAllMember = IsAllMember(mem);

                    PivotTotal total = this.PivotData.TotalAxis.GetTotalByName(mem.UniqueName);
                    //string caption = (total != null) ? total.Caption : mem.Caption;
                    string caption = (total != null) ? total.Caption : CommonUtils.GetMemberCaptionWithoutID(mem);


                    this.SetLastMemberCaption(dimUN, levelNumber, caption, isAllMember);
                }
            }

            private bool IsAllMember(Member mbr)
            {
                return (mbr.ParentLevel.Name == "(All)");
            }

            private void SetLastMemberCaption(string dimUN, int levelNumber, string memberCaption, bool isAllMember)
            {
                foreach (DimInfo dimInfo in this.DimensionsInfo)
                {
                    if (dimInfo.UniqueName == dimUN)
                    {
                        dimInfo.SetLastMemberCaption(levelNumber, memberCaption, isAllMember);
                        return;
                    }
                }
                //Если нет информации о данном измерениии, добавим его
                DimInfo newDimInfo = new DimInfo(dimUN);
                newDimInfo.SetLastMemberCaption(levelNumber, memberCaption, isAllMember);
                this.DimensionsInfo.Add(newDimInfo);
            }

            /// <summary>
            /// Вернет имена всех родительских элементов
            /// </summary>
            /// <param name="mem"></param>
            /// <param name="separator"></param>
            /// <returns></returns>
            public string GetAllLastMemberCaption(Member mem, string separator)
            {
                string result = string.Empty;
                string[] captionArray = this.GetAllLastMemberCaption(mem.ParentLevel.ParentHierarchy.UniqueName,
                    mem.ParentLevel.LevelNumber);

                for (int i = 0; i < captionArray.Length; i++)
                {
                    if (i != 0)
                        result += separator;
                    result += captionArray[i];
                }
                return result;
            }

            public string[] GetAllLastMemberCaption(Member mem)
            {
                return this.GetAllLastMemberCaption(mem.ParentLevel.ParentHierarchy.UniqueName,
                    mem.ParentLevel.LevelNumber);
            }

            private string[] GetAllLastMemberCaption(string dimUN, int levelNumber)
            {
                foreach (DimInfo dimInfo in this.DimensionsInfo)
                {
                    if (dimInfo.UniqueName == dimUN)
                    {
                        return dimInfo.GetAllLastMemberCaption(levelNumber);
                    }
                }
                return new string[0];
            }

            public List<DimInfo> DimensionsInfo
            {
                get { return _dimensionsInfo; }
                set { _dimensionsInfo = value; }
            }
        }

        private class DimInfo
        {
            private string _uniqueName;
            private List<LevInfo> _levelsInfo;

            public DimInfo(string uniqueName)
            {
                this.UniqueName = uniqueName;
                this.LevelsInfo = new List<LevInfo>();
            }

            public void SetLastMemberCaption(int levelNumber, string memberCaption, bool isAllMember)
            {
                foreach (LevInfo levInfo in this.LevelsInfo)
                {
                    if (levInfo.LevelNumber == levelNumber)
                    {
                        levInfo.LastMemberCaption = memberCaption;
                        levInfo.IsAllLevel = isAllMember;
                        return;
                    }
                }
                //Если не нашли уровень с таким номером, создадим его
                this.LevelsInfo.Add(new LevInfo(levelNumber, memberCaption, isAllMember));
                //После добавления информации об новом уровне, упорядочим их по возростанию
                this.OrderLevels();
            }

            private void OrderLevels()
            {
                //сортируем уровни по возростанию
                for (int i = 1; i < this.LevelsInfo.Count; i++)
                {
                    int levelNumber = this.LevelsInfo[i].LevelNumber;
                    int k = i - 1;
                    while ((k >= 0) && (levelNumber < this.LevelsInfo[k].LevelNumber))
                    {
                        k--;
                    }
                    k++;
                    if ((k >= 0) && (k != i))
                    {
                        this.LevelsInfo.Insert(k, this.LevelsInfo[i]);
                        this.LevelsInfo.RemoveAt(i + 1);
                    }
                }
            }

            public string GetLastMemberCaption(int levelNumber)
            {
                foreach (LevInfo levInfo in this.LevelsInfo)
                {
                    if (levInfo.LevelNumber == levelNumber)
                    {
                        return levInfo.LastMemberCaption;
                    }
                }
                return string.Empty;
            }

            public string[] GetAllLastMemberCaption(int levelNumber)
            {
                List<string> result = new List<string>();

                foreach (LevInfo levInfo in this.LevelsInfo)
                {
                    if (levInfo.LevelNumber <= levelNumber)
                    {
                        if ((!levInfo.IsAllLevel) || (levelNumber == 0))
                            result.Add(levInfo.LastMemberCaption);
                    }
                }
                return result.ToArray();
            }

            public List<LevInfo> LevelsInfo
            {
                get { return _levelsInfo; }
                set { _levelsInfo = value; }
            }

            public string UniqueName
            {
                get { return _uniqueName; }
                set { _uniqueName = value; }
            }
        }

        private class LevInfo
        {
            private int _levelNumber;
            private string _lastMemberCaption;
            private bool _isAllLevel;

            public LevInfo(int levelNumber, string lastMemberCaption, bool isAllLevel)
            {
                this.LevelNumber = levelNumber;
                this.LastMemberCaption = lastMemberCaption;
                this.IsAllLevel = isAllLevel;
            }

            public int LevelNumber
            {
                get { return _levelNumber; }
                set { _levelNumber = value; }
            }

            public string LastMemberCaption
            {
                get { return _lastMemberCaption; }
                set { _lastMemberCaption = value; }
            }

            public bool IsAllLevel
            {
                get { return _isAllLevel; }
                set { _isAllLevel = value; }
            }
        }

        

        #endregion

        #region ICompositeLegendParams Members

        LegendLocation ICompositeLegendParams.CompositeLegendLocation
        {
            get { return compositeLegendLocation; }
            set
            {
                compositeLegendLocation = value;
                SetSpanCompositeChart(compositeLegendLocation, compositeLegendExtent);
            }
        }

        public int CompositeLegendExtent
        {
            get { return compositeLegendExtent; }
            set
            {
                compositeLegendExtent = value;
                SetSpanCompositeChart(compositeLegendLocation, compositeLegendExtent);
            }
        }

        public ChartSynchronization Synchronization
        {
            get { return synchronization; }
            set { synchronization = value; }
        }

        #endregion

        #region Работа с композитной диаграммой

        #region Работа со списком дочерних диаграмм

        /// <summary>
        /// Добавление дочерней диаграммы
        /// </summary>
        /// <param name="key">ключ дочерней диаграммы</param>
        public void AddChildChart(string key)
        {
            if (key != string.Empty && !childChartList.Contains(key))
            {
                childChartList.Add(key);
            }
            RefreshCompositeChart();
        }

        /// <summary>
        /// Удаление дочерней диаграммы
        /// </summary>
        /// <param name="key">ключ дочерней диаграммы</param>
        public void RemoveChildChart(string key)
        {
            if (key != string.Empty && childChartList.Contains(key))
            {
                childChartList.Remove(key);
            }
            RefreshCompositeChart();
        }

        /// <summary>
        /// Вставка дочерней диаграммы
        /// </summary>
        /// <param name="key">ключ дочерней диаграммы</param>
        /// <param name="position">позиция для вставки</param>
        public void InsertChildChart(string key, int position)
        {
            if (key != string.Empty && !childChartList.Contains(key))
            {
                childChartList.Insert(position, key);
            } 
            RefreshCompositeChart();
        }

        /// <summary>
        /// Содержит ли уже дочернюю диаграмму с таким ключом
        /// </summary>
        /// <param name="key">ключ дочерней диаграммы</param>
        public bool ContainsChildChart(string key)
        {
            return (key != string.Empty && childChartList.Contains(key));
        }

        /// <summary>
        /// Получить ключ дочерней диаграммы
        /// </summary>
        /// <param name="index">ключ дочерней диаграммы</param>
        public string GetChildChartKey(int index)
        {
            if (index >= 0 && index < childChartList.Count)
            {
                return childChartList[index];
            }
            return string.Empty;
        }

        #endregion

        #region Работа со списком родительских диаграмм

        /// <summary>
        /// Добавление дочерней диаграммы
        /// </summary>
        /// <param name="key">ключ дочерней диаграммы</param>
        public void AddParentChart(string key)
        {
            if (key != string.Empty && !parentChartList.Contains(key))
            {
                parentChartList.Add(key);
            }
        }

        /// <summary>
        /// Удаление дочерней диаграммы
        /// </summary>
        /// <param name="key">ключ дочерней диаграммы</param>
        public void RemoveParentChart(string key)
        {
            if (key != string.Empty && parentChartList.Contains(key))
            {
                parentChartList.Remove(key);
            }
        }

        #endregion

        /// <summary>
        /// Обновление композитной диаграммы
        /// </summary>
        public void RefreshCompositeChart()
        {
            // запоминаем настройки легенды
            CompositeLegend tempLegend = null;
            if (chart.CompositeChart.Legends.Count > 0)
            {
               tempLegend = chart.CompositeChart.Legends[0];
            }

            axies = new List<AxisItem>();
            // запоминаем настройки осей
            axies.Clear();
            if (chart.CompositeChart.ChartAreas.Count > 0)
            {
                for (int i = 0; i < chart.CompositeChart.ChartAreas[0].Axes.Count; i++)
                {
                    axies.Add(chart.CompositeChart.ChartAreas[0].Axes[i]);
                }
            }
            // сбрасываем все настрйки
            ResetCompositeChart();

            // добавляем дочерние диаграммы
            for (int i = 0; i < childChartList.Count; i++)
            {
                string key = childChartList[i];
                ChartReportElement element = MainForm.FindChartReportElement(key);
                if (element != null)
                {
                    UltraChart childChart = element.chart;
                    AddCompositeLayer(childChart, i,  key);
                    element.AddParentChart(UniqueName);
                }
            }

            // восстанавливаем настройки осей
            if (chart.CompositeChart.ChartAreas.Count > 0)
            {
                if (chart.CompositeChart.ChartAreas[0].Axes.Count != 0)
                {
                    for (int i = 0; i < axies.Count; i++)
                    {
                        if (i < chart.CompositeChart.ChartAreas[0].Axes.Count)
                        {
                            chart.CompositeChart.ChartAreas[0].Axes[i].Extent = axies[i].Extent;
                        }
                    }
                }
                else
                {
                    // при загрузке отчета невсегда дочерние диаграммы грузятся до композитных,
                    // поэтому осей в композитной в этом месте может не быть - 
                    // значит добавим их вручную
                    for (int i = 0; i < axies.Count; i++)
                    {
                        chart.CompositeChart.ChartAreas[0].Axes.Add(axies[i]);
                    }
                }
            }
            // корректируем настройки осей
            if (CompositeAxiesCorrection)
            {
                SetAxisExtents();
            }

            // восстанавливаем настройки легенды
            RestoreCompositeLegend(tempLegend);

            chart.InvalidateLayers();
        }

        /// <summary>
        /// Обновление родительских диаграмм при изменении настроек дочерней
        /// </summary>
        public void RefreshParentCharts()
        {
            foreach (string parentKey in parentChartList)
            {
                if (parentKey != string.Empty)
                {
                    ChartReportElement parentElement = MainForm.FindChartReportElement(parentKey);
                    if (parentElement != null)
                    {
                        parentElement.RefreshCompositeChart();
                    }
                }
            }
        }

        /// <summary>
        /// Сброс настроек композитной диаграммы
        /// </summary>
        public void ResetCompositeChart()
        {
            chart.CompositeChart.ResetChartAreas();
            chart.CompositeChart.ResetChartLayers();
            chart.CompositeChart.ResetLegends();
            chart.CompositeChart.ResetSeries();
            chart.CompositeChart.Reset();

            // Добавляем область
            if (chart.CompositeChart.ChartAreas.Count == 0)
            {
                ChartArea area = new ChartArea();
                chart.CompositeChart.ChartAreas.Add(area);
            }

            // добавляем легенду
            if (chart.CompositeChart.Legends.Count == 0)
            {
                CompositeLegend legend = new CompositeLegend();
                legend.Bounds = new Rectangle(75, 0, 100, 100);
                legend.BoundsMeasureType = MeasureType.Percentage;
                legend.PE.Fill = Color.FloralWhite;
                legend.PE.FillOpacity = 150;
                legend.Border.Color = Color.Navy;
                legend.Border.Thickness = 1;
                legend.Visible = false;
                chart.CompositeChart.Legends.Add(legend);
            }
        }

        /// <summary>
        /// Сброс настроек композитной диаграммы (легенда не сбрасывается)
        /// </summary>
        public void ResetCompositeLayers()
        {
            chart.CompositeChart.ResetChartAreas();
            chart.CompositeChart.ResetChartLayers();
            chart.CompositeChart.ResetSeries();

            // Добавляем область
            if (chart.CompositeChart.ChartAreas.Count == 0)
            {
                ChartArea area = new ChartArea();
                chart.CompositeChart.ChartAreas.Add(area);
            }
        }

        /// <summary>
        /// Добавление слоя диаграммы в композитную диаграмму
        /// </summary>
        /// <param name="childChart">добавляемая диаграмма</param>
        /// <param name="index">номер слоя</param>
        /// <param name="elementKey">ключ элемента</param>
        public void AddCompositeLayer(UltraChart childChart, int index, string elementKey)
        {
            if (childChart.DataSource == null)
            {
                return;
            }

            if (chart.CompositeChart.ChartAreas.Count == 0)
            {
                return;
            }

            // добавляем оси на область
            AxisItem axisX;
            AxisItem axisY;
            AxisItem axisY2;
            if (chart.Axis.X.Visible)
            {
                // по умолчанию из двух видимых берем ось X
                axisX = CompositeChartUtils.GetAxisX(childChart.Axis.X);
            }
            else
            {
                axisX = CompositeChartUtils.GetAxisX2(childChart.Axis.X2);
            }
            axisY = CompositeChartUtils.GetAxisY(childChart.Axis.Y);
            axisY2 = CompositeChartUtils.GetAxisY2(childChart.Axis.Y2);
            chart.CompositeChart.ChartAreas[0].Axes.Add(axisX);
            chart.CompositeChart.ChartAreas[0].Axes.Add(axisY);
            chart.CompositeChart.ChartAreas[0].Axes.Add(axisY2);

            // если есть метка серий, то идем с 1 и считаем метку нулевой колонки меткой рядов
            DataTable chartDT = ((DataTable)(childChart.DataSource));
            string beginSerieName = (chartDT.Columns.Count > 0) ? chartDT.Columns[0].ColumnName : string.Empty;
            int beginIndex;
            if (CompositeChartUtils.IsNumericSeries(childChart.ChartType) && beginSerieName == "Series Name")
            {
                beginIndex = 1;
                if (!CompositeChartUtils.IsInvertAxisHorizontalAlignment(childChart.ChartType))
                {
                    axisY.Labels.HorizontalAlign = StringAlignment.Near;
                }
            }
            else
            {
                if (childChart.ChartType == ChartType.BarChart || childChart.ChartType == ChartType.StackBarChart)
                {
                    axisY.Labels.SeriesLabels.Format = AxisSeriesLabelFormat.None;
                    axisY2.Labels.SeriesLabels.Format = AxisSeriesLabelFormat.None;
                }
                else
                {
                    if (childChart.ChartType == ChartType.ColumnChart || childChart.ChartType == ChartType.StackColumnChart)
                    {
                        axisX.Labels.SeriesLabels.Format = AxisSeriesLabelFormat.None;
                    }
                    else
                    {
                        axisX.Labels.SeriesLabels.Format = AxisSeriesLabelFormat.SeriesLabel;
                        axisY.Labels.SeriesLabels.Format = AxisSeriesLabelFormat.SeriesLabel;
                    }
                }

                beginIndex = 0;
                // для этих типов инвертируем горизонтальное выравнивание метки на оси Y
                if (CompositeChartUtils.IsInvertAxisHorizontalAlignment(childChart.ChartType) && childChart.ChartType != ChartType.BarChart)
                {
                    axisY.Labels.HorizontalAlign = CompositeChartUtils.InvertAlignment(axisY.Labels.HorizontalAlign);
                }
                else
                {
                    if (childChart.ChartType != ChartType.BarChart)
                    {
                        axisY.Labels.HorizontalAlign = StringAlignment.Near;
                    }
                    else
                    {
                        axisY.Labels.HorizontalAlign = StringAlignment.Far;
                    }
                }
            }

            // Инвертируем для этих типов вертикальное выравнивание для меток осей Y и Y2
            if (childChart.ChartType != ChartType.BarChart)
            {
                axisY.Labels.VerticalAlign = CompositeChartUtils.InvertAlignment(axisY.Labels.VerticalAlign);
                axisY2.Labels.VerticalAlign = CompositeChartUtils.InvertAlignment(axisY2.Labels.VerticalAlign);
            }

            // добавляем слой с сериями            
            ChartLayerAppearance layer = CompositeChartUtils.GetLayerAppearance(childChart, elementKey);
            for (int i = beginIndex; i <= ((DataTable)childChart.DataSource).Columns.Count - 1; i++)
            {
                SeriesBase series = CompositeChartUtils.GetSeries(i, "Series", childChart, childChart.DataSource);
                if (series != null)
                {
                    chart.CompositeChart.Series.Add(series);
                    layer.Series.Add(series);
                }
            }
            layer.ChartArea = chart.CompositeChart.ChartAreas[0];

            // устанавливаем оси
            layer.AxisX = axisX;
            layer.AxisY = axisY;
            layer.AxisY2 = axisY2;

            // почему-то свойство инвентировано для композитных диаграмм
            layer.SwapRowsAndColumns = !childChart.Data.SwapRowsAndColumns;

            // добавляем легенду слоя
            if (chart.CompositeChart.Legends.Count > 0)
            {
                chart.CompositeChart.Legends[0].ChartLayers.Add(layer);
            }

            chart.CompositeChart.ChartLayers.Add(layer);
        }

        /// <summary>
        /// Восстановление параметров легенды композитной диаграммы по имеющимся слоям
        /// </summary>
        /// <param name="legend">легенда</param>
        public void RestoreCompositeLegend(CompositeLegend legend)
        {
            if (chart.CompositeChart.Legends.Count > 0)
            {
                chart.CompositeChart.Legends[0] = legend;
            }
            else
            {
                chart.CompositeChart.Legends.Add(legend);
            }

            if (chart.CompositeChart.Legends.Count > 0)
            {
                chart.CompositeChart.Legends[0].ChartLayerList = string.Empty;
                chart.CompositeChart.Legends[0].ChartLayers.Clear();
                for (int i = 0; i < chart.CompositeChart.ChartLayers.Count; i++)
                {
                    chart.CompositeChart.Legends[0].ChartLayers.Add(chart.CompositeChart.ChartLayers[i]);
                    //chart.CompositeChart.Legends[0].ChartLayerList += chart.CompositeChart.ChartLayers[i].Key + "|";
                }
            }
            chart.InvalidateLayers();
        }

        /// <summary>
        /// Автоматическое установление отступов осей с учетом настроенных ранее
        /// </summary>
        public void SetAxisExtents()
        {
            int extentsCount = axies.Count;
            int axisCount = 0;
            if (chart.CompositeChart.ChartAreas.Count > 0)
            {
                axisCount = chart.CompositeChart.ChartAreas[0].Axes.Count;
            }
            else
            {
                return;
            }

            int extentX = 0;
            int extentX2 = 0;
            int extentY = 0;
            int extentY2 = 0;

            // проходим с обратной стороны, т.к. оси слоев идут также 3-2-1
            for (int i = axisCount - 1; i >= 0; i--)
            {
                AxisItem axis = chart.CompositeChart.ChartAreas[0].Axes[i];
                int extent = 0;

                if (axis.Visible)
                {
                    if (i >= extentsCount)
                    {
                        // ось не была настроена - выставляем отступ автоматически в 80
                        switch (axis.axisNumber)
                        {
                            case AxisNumber.X_Axis:
                                {
                                    extentX += 80;
                                    extent = extentX;
                                    break;
                                }
                            case AxisNumber.X2_Axis:
                                {
                                    extentX2 += 80;
                                    extent = extentX2;
                                    break;
                                }
                            case AxisNumber.Y_Axis:
                                {
                                    extentY += 80;
                                    extent = extentY;
                                    break;
                                }
                            case AxisNumber.Y2_Axis:
                                {
                                    extentY2 += 80;
                                    extent = extentY2;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        // иначе учитываем настроенный ранее отступ
                        switch (axis.axisNumber)
                        {
                            case AxisNumber.X_Axis:
                                {
                                    extent += extentX + axies[i].Extent;
                                    break;
                                }
                            case AxisNumber.X2_Axis:
                                {
                                    extent += extentX2 + axies[i].Extent;
                                    break;
                                }
                            case AxisNumber.Y_Axis:
                                {
                                    extent += extentY + axies[i].Extent;
                                    break;
                                }
                            case AxisNumber.Y2_Axis:
                                {
                                    extent += extentY2 + axies[i].Extent;
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    // для невидимых уставливаем в 0
                    extent = 0;
                }

                // значение отступа должно быть в этих пределах
                if (extent >= 0 && extent <= 500)
                {
                    axis.Extent = extent;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Синхронизация диаграммы с таблицей
    /// </summary>
    public class ChartSynchronization
    {
        private string boundTo;
        private bool measureInRows = true;
        private ChartReportElement chartElement;

        public string BoundTo
        {
            get { return boundTo; }
            set
            {
                SetBoundTo(value);
                //boundTo = value;
            }
        }

        public bool MeasureInRows
        {
            get { return measureInRows; }
            set { measureInRows = value; }
        }

        public ChartReportElement ChartElement
        {
            get { return chartElement; }
            set { chartElement = value; }
        }

        public ChartSynchronization(ChartReportElement chartElement)
        {
            this.chartElement = chartElement;
        }

        private void SetBoundTo(string key)
        {
            if (key == this.BoundTo)
                return;

            //удаляем у таблицы ссылку на диаграмму
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                TableReportElement tableElement = this.ChartElement.MainForm.FindTableReportElement(this.BoundTo);
                if (tableElement != null)
                    tableElement.AnchoredElements.Remove(this.ChartElement.UniqueName);
            }

            //добавляем для таблицы ссылку на диаграмму
            if (!String.IsNullOrEmpty(key))
            {
                TableReportElement tableElement = this.ChartElement.MainForm.FindTableReportElement(key);
                if (tableElement != null)
                {
                    tableElement.AnchoredElements.Add(this.ChartElement.UniqueName);
                }
            }

            this.boundTo = key;
            /*this.ChartElement.MainForm.FieldListEditor.PivotData = this.ChartElement.PivotData;
            this.ChartElement.MainForm.FieldListEditor.InitEditor(this.ChartElement);*/
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                return this.ChartElement.MainForm.GetReportElementText(this.BoundTo);
            }
            else
            {
                return "";
            }
        }
    }

    public enum DataSeriesSeparator
    {
        [Description("Перенос строки")]
        eNewLine,
        [Description("Пробел")]
        eSpace,
        [Description(",")]
        eComma,
        [Description(";")]
        eDotComma,
        [Description(" | ")]
        eStick
    }
}
