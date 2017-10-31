using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0004_0002
{
    public partial class DefaultEvennessChart : CustomReportPage
    {
        private DataTable dtChart = new DataTable();
        private DataTable etalonDT = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2008;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 23);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 2 - 120);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 23);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 2 - 120);

            #region Настройка диаграмм

            UltraChart1.ChartType = ChartType.StackBarChart;
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.StackChart.StackStyle = StackStyle.Complete;
            UltraChart2.ChartType = ChartType.BarChart;

            UltraChart1.Axis.Y.Extent = 150;
            UltraChart2.Axis.Y.Extent = 150;

            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            UltraChart2.Axis.X2.Visible = true;
            UltraChart2.Axis.X2.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            UltraChart2.Axis.X2.Extent = 50;

            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
            UltraChart1.Axis.Y.Labels.Visible = false;

            UltraChart2.Axis.Y.Labels.SeriesLabels.Visible = false;
            UltraChart2.Axis.Y.Labels.Visible = true;

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
            UltraChart1.Legend.SpanPercentage = 2;
            
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1.Axis.X2.Visible = true;
            UltraChart1.Axis.X2.Labels.Visible = false;
            UltraChart1.Axis.X2.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.X2.LineThickness = 0;
            UltraChart1.Axis.X2.Extent = 18;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;

            PaintElement pe = new PaintElement();
            pe.ElementType = PaintElementType.Gradient;
            pe.Fill = Color.Green;
            pe.FillStopColor = Color.ForestGreen;
            pe.FillGradientStyle = GradientStyle.Horizontal;
            pe.FillOpacity = 150;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);

            pe = new PaintElement();
            pe.ElementType = PaintElementType.Gradient;
            pe.Fill = Color.SkyBlue;
            pe.FillStopColor = Color.RoyalBlue;
            pe.FillGradientStyle = GradientStyle.Horizontal;
            pe.FillOpacity = 150;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);

            pe = new PaintElement();
            pe.ElementType = PaintElementType.Gradient;
            pe.Fill = Color.Yellow;
            pe.FillStopColor = Color.Gold;
            pe.FillGradientStyle = GradientStyle.Horizontal;
            pe.FillOpacity = 150;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);

            pe = new PaintElement();
            pe.ElementType = PaintElementType.Gradient;
            pe.Fill = Color.Red;
            pe.FillStopColor = Color.Maroon;
            pe.FillGradientStyle = GradientStyle.Horizontal;
            pe.FillOpacity = 150;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = false;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "Динамика&nbsp;исполнения&nbsp;расходов&nbsp;ГРБС";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0004_0004/DefaultDynamic.aspx";
            CrossLink2.Text = "Оценка&nbsp;равномерности&nbsp;расходов&nbsp;ГРБС";
            CrossLink2.NavigateUrl = "~/reports/MFRF_0004_0002/DefaultEvenness.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }

            Label1.Text = "Диаграмма равномерности расходов ГРБС";
            Page.Title = Label1.Text;
            Label2.Text = string.Format("за {0} год", ComboYear.SelectedValue);

            lbSubject1.Text = "Поквартальное исполнение расходов ГРБС";
            lbSubject2.Text = "Оценка равномерности расходов ГРБС";

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }
         
        #region Обработчики диаграмм

        private DataTable ReverseDataTable(DataTable dt)
        {
            DataTable revDT = dt.Clone();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                revDT.ImportRow(dt.Rows[i]);
            }

            return revDT;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = (((UltraChart)sender) == UltraChart1) ? DataProvider.GetQueryText("MFRF_0004_0002_chart1") : DataProvider.GetQueryText("MFRF_0004_0002_chart2");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "ГРБС", dtChart);

            if (((UltraChart)sender) == UltraChart2)
            {
                dtChart = ReverseDataTable(dtChart);

                etalonDT = dtChart.Copy();

                foreach (DataRow row in dtChart.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            double value = Convert.ToDouble(row[i]);
                            value =  value * 100;

                            if (value > 120)
                            {
                                row[i] = 120;
                            }
                            else
                            {
                                row[i] = value;
                            }
                        }
                    }
                }
            }

            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                ((UltraChart) sender).Series.Add(series);
            }
        }

        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int)xAxis.MapMinimum;
            int yMin = (int)yAxis.MapMinimum;
            int xMax = (int)xAxis.MapMaximum;
            int yMax = (int)yAxis.MapMaximum;

            int percent20 = (int)xAxis.Map(20);
            int percent50 = (int)xAxis.Map(50);
            int percent100 = (int)xAxis.Map(100);

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Solid;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(percent20, yMin);
            line.p2 = new Point(percent20, yMax);
            e.SceneGraph.Add(line);

            int textWidth = 40;
            int textHeight = 13;
            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(percent20 - textWidth / 2, yMax - textHeight - 40, textWidth, textHeight);
            text.SetTextString("№123н");
            e.SceneGraph.Add(text);

            line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(percent50, yMin);
            line.p2 = new Point(percent50, yMax);
            e.SceneGraph.Add(line);

            textWidth = 30;
            textHeight = 13;
            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(percent50 - textWidth / 2, yMax - textHeight - 40, textWidth, textHeight);
            text.SetTextString("№34н");
            e.SceneGraph.Add(text);

            line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Solid;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(percent100, yMin);
            line.p2 = new Point(percent100, yMax);
            e.SceneGraph.Add(line);

            textWidth = 110;
            textHeight = 13;
            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(percent100 - textWidth / 2, yMax - textHeight - 40, textWidth, textHeight);
            text.SetTextString("Предельный уровень");
            e.SceneGraph.Add(text);

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        double value = Convert.ToDouble(box.Value);
                        if (value < 20)
                        {
                            box.DataPoint.Label = string.Format("{0}\n{1:N2}%\nСоответствие критерию оценки (Приказ 123н)", 
                                DataDictionariesHelper.GetFullGRBSName(box.DataPoint.Label).Replace("\"", "'"), box.Value);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else
                        {
                            if (value > 100)
                            {
                                double etalonValue = 0;
                                if (etalonDT.Rows[box.Row][1] != DBNull.Value && etalonDT.Rows[box.Row][1].ToString() != string.Empty)
                                {
                                    etalonValue = 100 * Convert.ToDouble(etalonDT.Rows[box.Column][1]);
                                }

                                box.DataPoint.Label = string.Format("{0}\n{1:N2}%\nНесоответствие критерию оценки (Приказ 123н)",
                                     DataDictionariesHelper.GetFullGRBSName(box.DataPoint.Label).Replace("\"", "'"), etalonValue);
                                box.PE.ElementType = PaintElementType.Gradient;
                                box.PE.FillGradientStyle = GradientStyle.Horizontal;
                                box.PE.Fill = Color.Red;
                                box.PE.FillStopColor = Color.Maroon;
                            }
                            else
                            {


                                box.DataPoint.Label = string.Format("{0}\n{1:N2}%\nВ разрешенном интервале (Приказ 123н)",
                                    DataDictionariesHelper.GetFullGRBSName(box.DataPoint.Label).Replace("\"", "'"), box.Value);
                                box.PE.ElementType = PaintElementType.Gradient;
                                box.PE.FillGradientStyle = GradientStyle.Horizontal;
                                box.PE.Fill = Color.Yellow;
                                box.PE.FillStopColor = Color.Gold;
                            }
                        }
                    }
                }
            }
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.Series != null)
                    {
                        box.Series.Label = DataDictionariesHelper.GetFullGRBSName(box.Series.Label).Replace("\"", "'");
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.PdfExporter.TargetPaperSize = new PageSize(1250, 900);
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
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            IText title = cell.AddText();
            title.Alignment = TextAlignment.Center;
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSubject1.Text);

            UltraChart2.Width = 600;
            Infragistics.Documents.Reports.Graphics.Image img1 = UltraGridExporter.GetImageFromChart(UltraChart1);
            cell.AddImage(img1);

            cell = row.AddCell();
            title = cell.AddText();
            title.Alignment = TextAlignment.Center;
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSubject2.Text);

            UltraChart2.Width = 600;
            Infragistics.Documents.Reports.Graphics.Image img2 = UltraGridExporter.GetImageFromChart(UltraChart2);
            cell.AddImage(img2);
        }

        #endregion
    }
}