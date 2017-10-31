using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
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
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0001
{
    public partial class DefaultAllocation : CustomReportPage
    {
        #region Поля
        private const string regionsSerieNameSO = "МО Сахалинской области";
        private const string KDSerieNameSO = "Доходные источники консолидированного бюджета Сахалинской области";
        private const string OKVDSerieNameSO = "ОКВЭД консолидированного бюджета Сахалинской области";
        private const string regionsSerieName = "Территории";
        //private const string regionsSerieName = "МО";
        private const string KDSerieName = "Доходные источники";
        //private const string KDSerieName =   "Доходные источники консолидированного бюджета";
        private const string OKVDSerieName = "ОКВЭД";
        //private const string OKVDSerieName = "ОКВЭД консолидированного бюджета";
        private int firstYear = 2000;
        private int endYear = 2011;

        private double avgArrears;
        private int counterAvg = 0;
        private double avgArrearsIncrease;
        private bool arrearsDown = false;
        private bool fns28nSplitting;

        private DateTime currentDate;

        #endregion

        #region Параметры запроса

        // уровень МР и ГО
        private CustomParam regionsLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Visible = false;
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 150);

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.ScatterChart.IconSize = SymbolIconSize.Medium;

            UltraChart.Border.Thickness = 0;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> \n  недоимка: <DATA_VALUE_X:N3> тыс.руб.\n  прирост: <DATA_VALUE_Y:N3> тыс.руб.";

            UltraChart.Axis.X.Extent = 60;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Extent = 70;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.TitleLeft.Visible = true;
            //UltraChart.TitleLeft.Text = "      Прирост недоимки, тыс.руб.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "Сумма недоимки, тыс.руб.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 12);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.SpanPercentage = 12;
            if (RegionSettingsHelper.Instance.Name == "Сахалинская область")
            {
                UltraChart.Legend.Location = LegendLocation.Top;
                UltraChart.Legend.Font = new Font("Verdana", 10);
                UltraChart.Legend.Margins.Left = 70;
            }
            else
            {
                UltraChart.Legend.Location = LegendLocation.Right;
                UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value * 0.75);
            }
            UltraChart.Legend.DataAssociation = ChartTypeData.ScatterData;


            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;

            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            KDLink.Visible = true;
            KDLink.Text = "По&nbsp;доходным&nbsp;источникам";
            KDLink.NavigateUrl = GetReportFullName("ФНС_0001_0001_KD");

            OKVDLink.Visible = true;
            OKVDLink.Text = "По&nbsp;ОКВЭД";
            OKVDLink.NavigateUrl = "~/reports/FNS_0001_0001/DefaultOKVD.aspx";

            RegionsLink.Visible = true;
            RegionsLink.Text = "См.&nbsp;также&nbsp;по&nbsp;муницип.районам&nbsp;и&nbsp;гор.округам";
            RegionsLink.NavigateUrl = GetReportFullName("ФНС_0001_0001");

            SettlementLink.Visible = true;
            SettlementLink.Text = "По&nbsp;поселениям";
            SettlementLink.NavigateUrl = "~/reports/FNS_0001_0001/DefaultSettlement.aspx";

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.FNS28nSplitting);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.FNSOKVEDGovernment.Value = RegionSettingsHelper.Instance.FNSOKVEDGovernment;
            UserParams.FNSOKVEDHousehold.Value = RegionSettingsHelper.Instance.FNSOKVEDHousehold;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                currentDate = fns28nSplitting ? CubeInfoHelper.Fns28nSplitInfo.LastDate : CubeInfoHelper.Fns28nNonSplitInfo.LastDate;

                endYear = currentDate.Year;

                UserParams.PeriodYear.Value = currentDate.Year.ToString();
                UserParams.PeriodMonth.Value = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month));

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            Page.Title = "Распределение по приросту недоимки";
            PageTitle.Text = Page.Title;

            UserParams.PeriodLastYear.Value = String.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", currentDate.Year - 1);
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodYear.Value = String.Format("[{0}].[{1}].[{2}].[{3}]", currentDate.Year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(currentDate.Month));
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            PageSubTitle.Text = string.Format("за {0} {1} {2} года", currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year);

            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        private static double AVG(double d1, double d2)
        {
            return (d1 + d2) / 2;
        }

        private static bool CheckArrearsDown(DataTable dt, int columnIndex)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[columnIndex] != DBNull.Value && row[columnIndex].ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(row[columnIndex]);
                    if (value < 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {

            string regionsSerieName_ = string.Empty;
            string KDSerieName_ = string.Empty;
            string OKVDSerieName_ = string.Empty;
            string queryName = fns28nSplitting ? "FNS_0001_0001_allocation_regionsChart_split" : "FNS_0001_0001_allocation_regionsChart";
            string query = DataProvider.GetQueryText(queryName);
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (RegionSettingsHelper.Instance.Name == "Сахалинская область")
            {

                regionsSerieName_ = regionsSerieNameSO;
                KDSerieName_ = KDSerieNameSO;
                OKVDSerieName_ = OKVDSerieNameSO;
            }
            else
            {
                regionsSerieName_ = regionsSerieName;
                KDSerieName_ = KDSerieName;
                OKVDSerieName_ = OKVDSerieName;
            }
            if (dtChart.Rows.Count != 0)
            {
                foreach (DataRow r in dtChart.Rows)
                {
                    if (r[0] != DBNull.Value)
                    {
                        string value = r[0].ToString();
                        r[0] = value.Replace("\"", "'");
                    }
                }

                DataRow row = dtChart.Rows[dtChart.Rows.Count - 1];
                avgArrears = AVG(avgArrears, Convert.ToDouble(row[1]));
                avgArrearsIncrease = AVG(avgArrearsIncrease, Convert.ToDouble(row[2]));
                dtChart.Rows.Remove(row);
            }
            arrearsDown = arrearsDown || CheckArrearsDown(dtChart, 2);
            XYSeries series = CRHelper.GetXYSeries(1, 2, dtChart);
            series.Label = regionsSerieName_;
            UltraChart.Series.Add(series);

            queryName = fns28nSplitting ? "FNS_0001_0001_allocation_KDChart_split" : "FNS_0001_0001_allocation_KDChart";
            query = DataProvider.GetQueryText(queryName);
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count != 0)
            {
                DataRow row = dtChart.Rows[dtChart.Rows.Count - 1];
                avgArrears = AVG(avgArrears, Convert.ToDouble(row[1]));
                avgArrearsIncrease = AVG(avgArrearsIncrease, Convert.ToDouble(row[2]));
                dtChart.Rows.Remove(row);
            }
            arrearsDown = arrearsDown || CheckArrearsDown(dtChart, 2);
            series = CRHelper.GetXYSeries(1, 2, dtChart);

            series.Label = KDSerieName_;
            UltraChart.Series.Add(series);

            queryName = fns28nSplitting ? "FNS_0001_0001_allocation_OKVDChart_split" : "FNS_0001_0001_allocation_OKVDChart";
            query = DataProvider.GetQueryText(queryName);
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count != 0)
            {
                DataRow row = dtChart.Rows[dtChart.Rows.Count - 1];
                avgArrears = AVG(avgArrears, Convert.ToDouble(row[1]));
 
                avgArrearsIncrease = AVG(avgArrearsIncrease, Convert.ToDouble(row[2]));
                dtChart.Rows.Remove(row);
            }
            
            arrearsDown = arrearsDown || CheckArrearsDown(dtChart, 2);
            series = CRHelper.GetXYSeries(1, 2, dtChart);
            series.Label = OKVDSerieName_;
            UltraChart.Series.Add(series);

            if (arrearsDown)
            {
                UltraChart.TitleLeft.Text = " Прирост / снижение недоимки, тыс.руб.";
            }
            else
            {
                UltraChart.TitleLeft.Text = "      Прирост недоимки, тыс.руб.";
            }
        }

        private void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (UltraChart.Series.Count != 0 && UltraChart.Series[0].GetPoints().Count == 0)
            {
                return;
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int)xAxis.MapMinimum;
            int yMin = (int)yAxis.MapMinimum;
            int xMax = (int)xAxis.MapMaximum;
            int yMax = (int)yAxis.MapMaximum;


            int avgY = (int)yAxis.Map(avgArrearsIncrease);
            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dot;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;

            line.p1 = new Point(xMin, avgY);
            line.p2 = new Point(xMax, avgY);
            e.SceneGraph.Add(line);

            int avgX = (int)xAxis.Map(avgArrears);
            line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dot;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(avgX, yMin);
            line.p2 = new Point(avgX, yMax);
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(3 * xMax / 4, yMin + 55, 100, 20);
            text.SetTextString("Большая недоимка");
            e.SceneGraph.Add(text);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(xMin - 85, (yMin - yMax) / 16, 20, 150);
            text.SetTextString("Большой прирост недоимки");
            LabelStyle style = new LabelStyle();
            style.Orientation = TextOrientation.VerticalLeftFacing;
            text.SetLabelStyle(style);
            e.SceneGraph.Add(text);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(xMax / 4, yMin + 55, 120, 20);
            text.SetTextString("Небольшая недоимка");
            e.SceneGraph.Add(text);

            bool containsZero = false;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Symbol)
                {
                    Symbol icon = primitive as Symbol;
                    if (icon.Path == "Legend")
                    {
                        Primitive prevPrimitive = e.SceneGraph[i - 1];
                        if (prevPrimitive is Text)
                        {
                            string legendText = ((Text)prevPrimitive).GetTextString();
                            icon.icon = GetIconType(legendText);
                            icon.iconSize = SymbolIconSize.Medium;
                        }
                    }
                    else if (icon.Series != null)
                    {
                        icon.icon = GetIconType(icon.Series.Label);
                    }
                }

                if (arrearsDown && primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text label = (Text)primitive;
                    if (!containsZero && label.GetTextString() != null && label.GetTextString() == "0")
                    {
                        label.SetTextString(string.Empty);
                        containsZero = true;
                    }
                }
            }

            if (arrearsDown)
            {
                int zeroValue = (int)yAxis.Map(0);
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.Gray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(xMin, zeroValue);
                line.p2 = new Point(xMax, zeroValue);
                e.SceneGraph.Add(line);

                text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMax - 60, zeroValue - 20, 60, 20);
                text.SetTextString("Прирост");
                style = new LabelStyle();
                style.Orientation = TextOrientation.Horizontal;
                text.SetLabelStyle(style);
                e.SceneGraph.Add(text);

                text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMax - 60, zeroValue, 60, 20);
                text.SetTextString("Снижение");
                style = new LabelStyle();
                style.Orientation = TextOrientation.Horizontal;
                text.SetLabelStyle(style);
                e.SceneGraph.Add(text);

                text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMin - 12, zeroValue - 4, 10, 10);
                text.SetTextString("0");
                style = new LabelStyle();
                style.Font = UltraChart.Axis.Y.Labels.Font;
                style.FontColor = Color.Gainsboro;
                style.Orientation = TextOrientation.Horizontal;
                text.SetLabelStyle(style);
                e.SceneGraph.Add(text);
            }
            else
            {
                text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMin - 85, 6 * (yMin - yMax) / 8, 20, 160);
                text.SetTextString("Небольшой прирост недоимки");
                style = new LabelStyle();
                style.Orientation = TextOrientation.VerticalLeftFacing;
                text.SetLabelStyle(style);
                e.SceneGraph.Add(text);
            }
        }

        private static SymbolIcon GetIconType(string seriesName)
        {
            SymbolIcon iconType;
            if (RegionSettingsHelper.Instance.Name == "Сахалинская область")
            {
                switch (seriesName)
                {
                    case regionsSerieNameSO:
                        {
                            iconType = SymbolIcon.Circle;
                            break;
                        }
                    case KDSerieNameSO:
                        {
                            iconType = SymbolIcon.Square;
                            break;
                        }
                    case OKVDSerieNameSO:
                        {
                            iconType = SymbolIcon.Diamond;
                            break;
                        }
                    default:
                        {
                            iconType = SymbolIcon.Random;
                            break;
                        }
                }
            }
            else
            {
                switch (seriesName)
                {
                    case regionsSerieName:
                        {
                            iconType = SymbolIcon.Circle;
                            break;
                        }
                    case KDSerieName:
                        {
                            iconType = SymbolIcon.Square;
                            break;
                        }
                    case OKVDSerieName:
                        {
                            iconType = SymbolIcon.Diamond;
                            break;
                        }
                    default:
                        {
                            iconType = SymbolIcon.Random;
                            break;
                        }
                }
            }
            return iconType;
        }

        #endregion
        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Диаграмма");
            sheet1.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet1.Rows[1].Cells[0].Value = PageSubTitle.Text;

            UltraGridExporter.ChartExcelExport(sheet1.Rows[3].Cells[0], UltraChart);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 3;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1, 100, 0);
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
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.86));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
