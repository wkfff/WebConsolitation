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

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0001_HMAO
{
    public partial class DefaultAllocation : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private const string regionsSerieName = "Территории";
        private const string KDSerieName = "Доходные источники";
        private const string OKVDSerieName = "ОКВЭД";
        private int firstYear = 2000;
        private int endYear = 2011;
        private double avgArrears;
        private double avgArrearsIncrease;
        private bool arrearsDown = false;
        private bool fns28nSplitting;

        // недоимка именительный падеж
        private string nomStr;
        private string genStr;

        #endregion

        #region Параметры запроса

        // уровень МР и ГО
        private CustomParam regionsLevel;
        private CustomParam index;
        private CustomParam taxPayer;

        #endregion

        private static MemberAttributesDigest indexDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 150);

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }

            index = UserParams.CustomParam("index");
            taxPayer = UserParams.CustomParam("tax_payer");
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
           // UltraChart.TitleLeft.Text = "Прирост недоимки, тыс.руб.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart.TitleBottom.Visible = true;
          //  UltraChart.TitleBottom.Text = "Сумма недоимки, неурегулированной задолженности, тыс. руб.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 12);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.SpanPercentage = 12;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.DataAssociation = ChartTypeData.ScatterData;
            UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value * 0.75);

            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;

            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            KDLink.Visible = true;
            KDLink.Text = "По&nbsp;доходным&nbsp;источникам";
            KDLink.NavigateUrl = "~/reports/FNS_0001_0001_HMAO/DefaultKD.aspx";

            OKVDLink.Visible = true;
            OKVDLink.Text = "По&nbsp;ОКВЭД";
            OKVDLink.NavigateUrl = "~/reports/FNS_0001_0001_HMAO/DefaultOKVD.aspx";

            RegionsLink.Visible = true;
            RegionsLink.Text = "См.&nbsp;также&nbsp;по&nbsp;муницип.районам&nbsp;и&nbsp;гор.округам";
            RegionsLink.NavigateUrl = "~/reports/FNS_0001_0001_HMAO/DefaultRegions.aspx";

            SettlementLink.Visible = true;
            SettlementLink.Text = "По&nbsp;поселениям";
            SettlementLink.NavigateUrl = "~/reports/FNS_0001_0001_HMAO/DefaultSettlement.aspx";

            ComparableLink.Visible = true;
            ComparableLink.Text = "Недоимка&nbsp;в&nbsp;сопоставимых&nbsp;условиях";
            ComparableLink.NavigateUrl = "~/reports/FNS_0008_0002/Default.aspx";

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExportButton.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            bool splittingSwitchEnable = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("SplittingSwitchEnable"));
            if (splittingSwitchEnable)
            {
                SplittingSwitch.Visible = true;
                fns28nSplitting = SplittingSwitch.SelectedIndex == 1;
            }
            else
            {
                SplittingSwitch.Visible = false;
                fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.FNS28nSplitting);
            }


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

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string queryName = fns28nSplitting ? "FNS_0001_0001_HMAO_date_split" : "FNS_0001_0001_HMAO_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();

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

                indexDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0001_0001_Digest");
                ComboIndex.Title = "Показатели";
                ComboIndex.Width = 400;
                ComboIndex.MultiSelect = false;
                ComboIndex.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indexDigest.UniqueNames, indexDigest.MemberLevels));
                ComboIndex.SetСheckedState("Недоимка, неурегулиров. задолж. Всего", true);

            }
            string nameIndex = indexDigest.GetFullName(ComboIndex.SelectedValue);
            Page.Title = string.Format("Распределение по приросту {0}", nameIndex);
            PageTitle.Text = Page.Title;

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);

            int month = ComboMonth.SelectedIndex + 1;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            index.Value = indexDigest.GetMemberUniqueName(ComboIndex.SelectedValue);
            switch (ComboIndex.SelectedIndex)
            {
                case 0:
                    {
                        taxPayer.Value = "[ФНС_28н_Коды налогоплательщиков].[Основные налогоплательщики ]";
                        nomStr = "недоимка, неурегулированная задолженность";
                        genStr = "недоимки, неурегулированной задолженности";
                        break;
                    }
                case 1:
                    {
                        taxPayer.Value = "[ФНС_28н_Коды налогоплательщиков].[Основные налогоплательщики ]";
                        nomStr = "недоимка";
                        genStr = "недоимки";
                        break;
                    }
                case 2:
                    {
                        taxPayer.Value = "[ФНС_28н_Коды налогоплательщиков].[Основные налогоплательщики ]";
                        nomStr = "неурегулированная задолженность";
                        genStr = "неурегулированной задолженности";
                        break;
                    }
                case 3:
                    {
                        taxPayer.Value = " [ФНС_28н_Коды налогоплательщиков].[Все налогоплательщики]";
                        nomStr = "недоимка, неурегулированная задолженность";
                        genStr = "недоимки, неурегулированной задолженности";
                        break;
                    }
                case 4:
                    {
                        taxPayer.Value = " [ФНС_28н_Коды налогоплательщиков].[Все налогоплательщики]";
                        nomStr = "недоимка";
                        genStr = "недоимки";
                        break;
                    }
                case 5:
                    {
                        taxPayer.Value = " [ФНС_28н_Коды налогоплательщиков].[Все налогоплательщики]";
                        nomStr = "неурегулированная задолженность";
                        genStr = "неурегулированной задолженности";
                        break;
                    }
            }



            PageSubTitle.Text = string.Format("за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year);

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
            string queryName = fns28nSplitting ? "FNS_0001_0001_HMAO_allocation_regionsChart_split" : "FNS_0001_0001_HMAO_allocation_regionsChart";
            string query = DataProvider.GetQueryText(queryName);
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

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
            series.Label = regionsSerieName;
            UltraChart.Series.Add(series);

            queryName = fns28nSplitting ? "FNS_0001_0001_HMAO_allocation_KDChart_split" : "FNS_0001_0001_HMAO_allocation_KDChart";
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
            series.Label = KDSerieName;
            UltraChart.Series.Add(series);

            queryName = fns28nSplitting ? "FNS_0001_0001_HMAO_allocation_OKVDChart_split" : "FNS_0001_0001_HMAO_allocation_OKVDChart";
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
            series.Label = OKVDSerieName;
            UltraChart.Series.Add(series);

            if (arrearsDown)
            {
                UltraChart.TitleLeft.Text = string.Format("Прирост/снижение {0}, тыс.руб.", genStr);
            }
            else
            {
                UltraChart.TitleLeft.Text =string.Format( "      Прирост {0}, тыс.руб.", genStr);
            }

            UltraChart.TitleBottom.Text = string.Format("Сумма {0}, тыс. руб.", genStr);
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

            int avgY = (int)yAxis.Map(avgArrears);
            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dot;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(xMin, avgY);
            line.p2 = new Point(xMax, avgY);
            e.SceneGraph.Add(line);

            int avgX = (int)yAxis.Map(avgArrearsIncrease);
            line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dot;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(avgX, yMin);
            line.p2 = new Point(avgX, yMax);
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(xMax / 4 + 400, yMin + 55, 300, 20);
            text.SetTextString(string.Format("Большая {0}", nomStr));
            e.SceneGraph.Add(text);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(xMin - 85, (yMin - yMax) / 16, 20, 330);
            text.SetTextString(string.Format("Большой прирост {0}", genStr));
            LabelStyle style = new LabelStyle();
            style.Orientation = TextOrientation.VerticalLeftFacing;
            text.SetLabelStyle(style);
            e.SceneGraph.Add(text);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(xMax / 4 - 100, yMin + 55, 300, 20);
            text.SetTextString(string.Format("Небольшая {0}", nomStr));
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
                    Text label = (Text) primitive;
                    if (!containsZero && label.GetTextString() != null && label.GetTextString() == "0")
                    {
                        label.SetTextString(string.Empty);
                        containsZero = true;
                    }
                }
            }

            if (arrearsDown)
            {
                int zeroValue = (int) yAxis.Map(0);
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
                text.SetTextString(string.Format("Небольшой прирост {0}",genStr ));
                style = new LabelStyle();
                style.Orientation = TextOrientation.VerticalLeftFacing;
                text.SetLabelStyle(style);
                e.SceneGraph.Add(text);
            }
        }

        private static SymbolIcon GetIconType(string seriesName)
        {
            SymbolIcon iconType;
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
            return iconType;
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
