using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0004
{
    public partial class DefaultAllocation_budget : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 80;
            UltraChart.Axis.Y.Extent = 30;

            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<DATA_VALUE:P2>";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            AxisLabelLayoutAppearance layout = UltraChart.Axis.X.Labels.SeriesLabels.Layout;

            layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.ClipText = false;
            behavior.Enabled = true;
            behavior.Trimming = StringTrimming.None;
            behavior.UseOnlyToPreventCollisions = false;
            layout.BehaviorCollection.Add(behavior);

            #endregion

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.ExcelExportButton.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 140);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0004_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                Dictionary<string, string> KDDictionary = new Dictionary<string, string>();
                KDDictionary.Add("Налоговые доходы", "[КД].[Сопоставимый].[Налоговые доходы]");
                KDDictionary.Add("Неналоговые доходы", "[КД].[Сопоставимый].[Неналоговые доходы]");
                KDDictionary.Add("Доходы от предпринимательской деятельности", "[КД].[Сопоставимый].[Доходы от предпринимательской деятельности]");
                KDDictionary.Add("Налоговые и неналоговые доходы_", "[КД].[Сопоставимый].[Налоговые и неналоговые доходы_]");
                KDDictionary.Add("Доходы ВСЕГО", "[КД].[Сопоставимый].[Доходы ВСЕГО]");
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillKDNames(KDDictionary));
                ComboKD.SetСheckedState("Доходы ВСЕГО", true);

                ComboFO.Title = "ФO";
                ComboFO.Width = 310;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState("Все федеральные округа", true);
            }

            UserParams.Filter.Value = (ComboFO.SelectedIndex != 0)
                ? string.Format("and [Территории].[Сопоставимый].CurrentMember.Parent is [Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]",
                    ComboFO.SelectedValue)
                : " ";

            Page.Title = string.Format("Распределение субъектов {0} по темпу роста доходов", ComboFO.SelectedIndex == 0 ? "РФ" : RegionsNamingHelper.ShortName(UserParams.Region.Value));
            Label1.Text = Page.Title;
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = string.Format("{3} за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboKD.SelectedValue);

            string monthValue = ComboMonth.SelectedValue;
            string yearValue = ComboYear.SelectedValue;
            string kdValue = KDKindsDictionary[ComboKD.SelectedValue];

            if (!Page.IsPostBack || !UserParams.PeriodYear.ValueIs(yearValue) || !UserParams.PeriodMonth.ValueIs(monthValue) || !UserParams.KDGroup.ValueIs(kdValue))
            {
                int year = Convert.ToInt32(ComboYear.SelectedValue);

                UserParams.PeriodYear.Value = year.ToString();
                UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
                UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
                UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
                UserParams.KDGroup.Value = kdValue;
            }

            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_allocation_budget");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart);

            double avgValue = 0;

            if (dtChart.Rows.Count != 0 && dtChart.Rows[dtChart.Rows.Count - 1][0] != DBNull.Value)
            {
                DataRow row = dtChart.Rows[dtChart.Rows.Count - 1];
                if (row[0].ToString() == "Среднее")
                {
                    avgValue = Convert.ToDouble(row[1]);
                    dtChart.Rows.Remove(row);
                }
            }


            int medianIndex = MedianIndex(dtChart.Rows.Count);

            RegionsNamingHelper.ReplaceRegionNames(dtChart, 0);

            DataTable medianDT = dtChart.Clone();
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                medianDT.ImportRow(dtChart.Rows[i]);
                if (i == medianIndex)
                {
                    DataRow row = medianDT.NewRow();
                    row[0] = "Медиана";
                    row[1] = MedianValue(dtChart, "Темп роста");
                    medianDT.Rows.Add(row);
                }

                double value;
                Double.TryParse(dtChart.Rows[i][1].ToString(), out value);
                if (value >= avgValue && i != dtChart.Rows.Count - 1)
                {
                    Double.TryParse(dtChart.Rows[i + 1][1].ToString(), out value);
                    if (value < avgValue)
                    {
                        DataRow row = medianDT.NewRow();
                        row[0] = "Среднее";
                        row[1] = avgValue;
                        medianDT.Rows.Add(row);
                    }
                }
            }

            UltraChart.DataSource = medianDT;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Value != null && box.Value.ToString() != string.Empty)
                        {
                            double value = Convert.ToDouble(box.Value);
                            box.PE.Fill = (value >= 1) ? Color.Green : Color.Red;
                            box.PE.FillStopColor = (value >= 1) ? Color.DarkGreen : Color.DarkRed;
                        }

                        if (box.Series != null)
                        {
                            box.Series.Label = RegionsNamingHelper.FullName(box.Series.Label);

                            if (box.Series.Label == "Среднее" || box.Series.Label == "Медиана")
                            {
                                box.PE.Fill = Color.Yellow;
                                box.PE.FillStopColor = Color.Orange;
                            }
                        }
                    }
                }

                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    if (text.GetTextString() == "Среднее" || text.GetTextString() == "Медиана")
                    {
                        LabelStyle boldStyle = text.GetLabelStyle();
                        boldStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
                        boldStyle.FontColor = Color.Black;
                        text.SetLabelStyle(boldStyle);
                    }
                }
            }

//            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
//            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

//            if (xAxis == null || yAxis == null)
//                return;
//
//            int xMin = (int)xAxis.MapMinimum;
//            int yMin = (int)yAxis.MapMinimum;
//            int xMax = (int)xAxis.MapMaximum;
//            int yMax = (int)yAxis.MapMaximum;
//
//            int avg = (int)yAxis.Map(avgValue);
//            Line line = new Line();
//            line.lineStyle.DrawStyle = LineDrawStyle.Dot;
//            line.PE.Stroke = Color.DarkGray;
//            line.PE.StrokeWidth = 2;
//            line.p1 = new Point(xMin, avg);
//            line.p2 = new Point(xMax, avg);
//            e.SceneGraph.Add(line);
//
//            int textWidth = 200;
//            int textHeight = 10;
//            Text text = new Text();
//            text.PE.Fill = Color.Black;
//            text.bounds = new Rectangle(xMax - textWidth, ((int)yAxis.Map(avgValue)) - textHeight, xMax, textHeight);
//            text.SetTextString(string.Format("Среднее значение: {0:P2}", avgValue));
//            e.SceneGraph.Add(text);
//
//            int median = (int)yAxis.Map(medianValue);
//            line = new Line();
//            line.lineStyle.DrawStyle = LineDrawStyle.Dot;
//            line.PE.Stroke = Color.DarkGray;
//            line.PE.StrokeWidth = 2;
//            line.p1 = new Point(xMin, median);
//            line.p2 = new Point(xMax, median);
//            e.SceneGraph.Add(line);
//
//            textWidth = 150;
//            textHeight = 10;
//            text = new Text();
//            text.PE.Fill = Color.Black;
//            text.bounds = new Rectangle(xMax - textWidth, ((int)yAxis.Map(medianValue)) - textHeight, xMax, textHeight);
//            text.SetTextString(string.Format("Медиана: {0:P2}", medianValue));
//            e.SceneGraph.Add(text);
        }

        #endregion

        #region Расчет медианы

        private bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }

        private int MedianIndex(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            if (Even(length))
            {
                return length / 2 - 1;
            }
            else
            {
                return (length + 1) / 2 - 1;
            }
        }

        private double MedianValue(DataTable dt, string medianValueColumn)
        {
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            if (Even(dt.Rows.Count))
            {
                double value1;
                double value2;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value1);
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count) + 1][medianValueColumn].ToString(),
                        out value2);
                return (value1 + value2) / 2;
            }
            else
            {
                double value;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value);
                return value;
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid());
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
