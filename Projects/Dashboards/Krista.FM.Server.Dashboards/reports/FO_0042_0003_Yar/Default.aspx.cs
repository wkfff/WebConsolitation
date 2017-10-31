using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image=System.Drawing.Image;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0003_Yar
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChartAVG;
        private DataTable dtChart;
        private int firstYear = 2009;
        private int endYear = 2011;

        private double avgValue;
        private static MemberAttributesDigest grbsDigest;

        #endregion

        #region Параметры запроса

        // выбранный год
        private CustomParam selectedYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;

            #region Инициализация параметров запроса

            selectedYear = UserParams.CustomParam("selected_year");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 60;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X2.Visible = true;
            UltraChart.Axis.X2.Labels.Visible = false;
            UltraChart.Axis.X2.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X2.LineColor = Color.Transparent;
            UltraChart.Axis.X2.Extent = 20;

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart.ColorModel.ColorBegin = Color.Green;
            UltraChart.ColorModel.ColorEnd = Color.Red;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N1>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            //UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\nРейтинг: <DATA_VALUE:N1>";

            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;бальной&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_Yar/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Сводная&nbsp;оценка&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0002_Yar/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Анализ&nbsp;оценок&nbsp;ГРБС";
            CrossLink3.NavigateUrl = "~/reports/FO_0042_0004_Yar/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0003_Yar_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0001_Yar_adminstratorDescription");
            }

            Page.Title = String.Format("Рейтинг главных распорядителей средств областного бюджета Ярославской области по результатам оценки качества финансового менеджмента");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = String.Format("по итогам {0} года", ComboYear.SelectedValue);

            selectedYear.Value = ComboYear.SelectedValue;

            AVGChartDataBind();
            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0003_Yar_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            UltraChart.Series.Clear();
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                series.Label = dtChart.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }
        }

        protected void AVGChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0042_0003_Yar_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);

            avgValue = GetDoubleDTValue(dtChartAVG, "Средняя оценка");
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }
        
        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.bounds.Width = 30;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8);
                    axisText.labelStyle.WrapText = true;

                    axisText.SetTextString(grbsDigest.GetShortName(axisText.GetTextString()));
                }
                else if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string shortName = box.DataPoint.Label;
                        string fullName = grbsDigest.GetFullName(shortName);
                        string code = grbsDigest.GetMemberType(fullName);
                        box.DataPoint.Label = String.Format("{0} {1}", code, RemoveBadSymbols(fullName));
                    }
                }
            }
            
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            double lineStart = xAxis.MapMinimum;
            double lineLength = xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new Point((int)lineStart + (int)lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.labelStyle.Font = new Font("Verdana", 8);
            text.bounds = new Rectangle((int)lineLength - (int)textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя оценка: {0:N1}", avgValue));
            e.SceneGraph.Add(text);
        }

        private static string RemoveBadSymbols(string str)
        {
            str = str.Replace("\"", "'");
            str = str.Replace(@"
", "'");
            return str;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;

            UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[2].Cells[0], UltraChart);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid);
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
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            UltraChart.Width = Unit.Pixel((int) (CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
