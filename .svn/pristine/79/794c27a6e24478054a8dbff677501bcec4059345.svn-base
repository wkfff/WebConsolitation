using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0005
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private int firstYear = 2011;
        private int endYear = 2011;

        private int currentYear;

        private static MemberAttributesDigest grbsDigest;
        private static MemberAttributesDigest indicatorDigest;

        #endregion

        public bool IndicatorPercentFormat 
        {
            get { return ComboIndicator.SelectedValue == "Оценка качества финансового менеджмента"; }
        }

        public string IndicatorFormat
        {
            get { return IndicatorPercentFormat ? "<DATA_VALUE:N2>%" : "<DATA_VALUE:N2>"; }
        }

        public bool ValueSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        #region Параметры запроса

        // выбранный показатель
        private CustomParam selectedIndicator;
        // выбранная мера
        private CustomParam selectedMeasure;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedIndicator = UserParams.CustomParam("selected_indicator");
            selectedMeasure = UserParams.CustomParam("selected_maasure");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Анализ&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0003/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0005_grbsDigest");
            indicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0005_indicatorDigest");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0005_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = 500;
                ComboIndicator.MultiSelect = false;
                ComboIndicator.ParentSelect = false;
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indicatorDigest.UniqueNames, indicatorDigest.MemberLevels));
                ComboIndicator.SetСheckedState("Оценка качества финансового менеджмента", true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = String.Format("Динамика результатов мониторинга и оценки качества финансового менеждмента, осуществляемого главными распорядителями средств областного бюджета ({0})", ComboIndicator.SelectedValue);
            PageTitle.Text = Page.Title; 
            PageSubTitle.Text = String.Format("по итогам {0} года", currentYear);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            selectedIndicator.Value = indicatorDigest.GetMemberUniqueName(ComboIndicator.SelectedValue);
            selectedMeasure.Value = ValueSelected ? "Значение" : "Оценка показателя";

            SetChartAppearance();
            ChartDataBind();
        }

        #region Обработчики диаграммы

        private void SetChartAppearance()
        {
            #region Настройка диаграммы

            UltraChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            UltraChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight - 100);
            UltraChartBrick.Chart.FillSceneGraph += new FillSceneGraphEventHandler(Chart_FillSceneGraph);

            UltraChartBrick.XAxisLabelFormat = "N2";
            UltraChartBrick.DataFormatString = "N2";
            UltraChartBrick.TooltipFormatString = String.Format("<ITEM_LABEL> г.\n<SERIES_LABEL>\n{0}", IndicatorFormat);
            UltraChartBrick.ColorModel = ChartColorModel.DefaultFixedColors;
            UltraChartBrick.TitleTop = "";
            UltraChartBrick.XAxisExtent = 40;
            UltraChartBrick.YAxisExtent = 150;
            UltraChartBrick.SeriesLabelWrap = true;
            UltraChartBrick.ZeroAligned = false;

            UltraChartBrick.Chart.Axis.Y.Labels.Visible = false;

            UltraChartBrick.X2AxisExtent = 40;
            UltraChartBrick.X2AxisVisible = true;

            UltraChartBrick.Y2AxisExtent = 30;
            UltraChartBrick.Y2AxisVisible = true;
            UltraChartBrick.Chart.Axis.Y2.Labels.Visible = false;
            UltraChartBrick.Chart.Axis.Y2.LineThickness = 0;

            UltraChartBrick.Legend.Visible = true;
            UltraChartBrick.Legend.Location = LegendLocation.Top;
            UltraChartBrick.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChartBrick.Width.Value / 3);
            UltraChartBrick.Legend.Font = new Font("Verdana", 8);

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Center;
            appearance.HorizontalAlign = StringAlignment.Far;
            appearance.ItemFormatString = IndicatorPercentFormat ? "<DATA_VALUE:N2>%" : "<DATA_VALUE:N2>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChartBrick.Chart.BarChart.ChartText.Add(appearance);

            #endregion
        }

        private void ChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0042_0005_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                string compareValue = String.Empty;
                bool allEqual = true;

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        if (compareValue == String.Empty)
                        {
                            compareValue = row[1].ToString();
                        }

                        allEqual = allEqual && (row[1].ToString() == compareValue);
                    }
                }

                //UltraChartBrick.ZeroAligned = allEqual;
                
                int chartHeight = dtChart.Rows.Count * 50 + 200;

                UltraChartBrick.Height = chartHeight;
                UltraChartBrick.Legend.SpanPercentage = 60 * 100 / chartHeight;

                UltraChartBrick.DataTable = dtChart;
                UltraChartBrick.DataBind();
            }
        }

        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text axisText = (Text)primitive;
                    axisText.SetTextString(grbsDigest.GetShortName(axisText.GetTextString()));
                }
                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    if (text.GetTextString().Contains("DATA_VALUE"))
                    {
                        text.SetTextString(String.Empty);
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            UltraChartBrick.Chart.Width = Convert.ToInt32(UltraChartBrick.Chart.Width.Value * 0.8);
            ReportExcelExporter1.Export(UltraChartBrick.Chart, String.Empty, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PdfExporter.TargetPaperSize = new PageSize(1500, 900);

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section = report.AddSection();
            section.PageMargins.Top = 35;
            section.PageMargins.Left = 30;

            MemoryStream imageStream = new MemoryStream();
            UltraChartBrick.Chart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 1));
            UltraChartBrick.Chart.SaveTo(imageStream, ImageFormat.Png);
            System.Drawing.Image image = new Bitmap(imageStream, true);

            ReportPDFExporter1.Export(image, String.Empty, section);
        }

        #endregion
    }
}