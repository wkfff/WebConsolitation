using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.UltraChart.Core;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Shared;
using Infragistics.UltraChart.Core.Layers;
namespace Krista.FM.Server.Dashboards.reports.SEP_0003_Sakhalin
{
    public partial class _default : CustomReportPage
    {
        private CustomParam baseRegion;
        private CustomParam selectedPeriod;
        private CustomParam YearSelect;
        private CustomParam RegionSelect;
        private CustomParam PokSelect;
        private CustomParam areaSelect;

        private string SocKoef = "";
        private string EconKoef = "";
        private string FinKoef = "";
        private string EdIsm = "";
        private CustomParam member;
        private GridHeaderLayout headerLayout;
        string Economic_Prefix = "[Показатели].[Методики Сопоставимый].[Раздел].[Оценка СЭР МО].[Показатели социальной сферы]";
        string Social_Prefix = "[Показатели].[Методики Сопоставимый].[Раздел].[Оценка СЭР МО].[Показатели экономической сферы]";
        string Date_Prefix = ",[Период].[Год Квартал Месяц].[Год].[{0}].lag({1})";
        string Date_Prefix2 = ",[Период].[Год Квартал Месяц].[Год].[{0}].lag({1})";
        string LBC = "{0}";

        private string grid_title = "Показатели социально-экономического развития территории";
        private string chart1_title = "Динамика интегральных показателей оценки социально-экономического развития";
        private string chart2_title = "Динамика показателя «{0}», {1}";
        private string chart3_title = "Структура интегрального показателя оценки {0} сферы в {1} году";
        private string page_title = "Оценка социально-экономического развития территории (по муниципальным образованиям)";
        private string page_sub_title = "Ежегодная оценка социально-экономического положения территории по состоянию на {0}, {1}";
        private string LParam;
        private bool refreshFlag = false;
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            if (RegionSettings.Instance.Id == "Sakhalin")
            {
                DataProvidersFactory.SpareMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.SpareMASDataProvider.GetCellset(s), dt, "sadad");
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            }
            return dt;
        }
        string[] l_Ar;
        string[] ldAr;

        Dictionary<string, int> ForParam(string query)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();


            DataTable dt = GetDSForChart(query);
            l_Ar = new string[dt.Rows.Count];
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                try
                {

                    d.Add(dt.Rows[i][0].ToString(), 0);
                    l_Ar[i] = dt.Rows[i][0].ToString();
                }
                catch { }
            }


            LParam = dt.Rows[dt.Rows.Count - 1][0].ToString();
            return d;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Page.UICulture = "af-ZA";
            Page.Title = "Оценка социально-экономического развития территории (для  муниципального образования)";
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            G.Height = 300;

            YearSelect = UserParams.CustomParam("YearSelect");
            baseRegion = UserParams.CustomParam("baseRegion");
            PokSelect = UserParams.CustomParam("pokSelect");
            areaSelect = UserParams.CustomParam("areaSelect");
            selectedPeriod = UserParams.CustomParam("selectedPeriod");

            RegionSelect = UserParams.CustomParam("regionSelect");

            member = UserParams.CustomParam("member");

            C2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth *0.45 - 10 - 5);
            C3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth *0.55 - 10);
            C3.DeploymentScenario.FilePath = "../../TemporaryImages";
            C3.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png";
            Year.Width = 200;
            Regions.Width = 350;
            C2.Height = 600;
            C3.Height = 600;

            Panel.AddLinkedRequestTrigger(G);
            Panel.AddRefreshTarget(C2);
            Panel.AddRefreshTarget(C3);
            C3.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PieChart;
            C3.PieChart.OthersCategoryPercent = 1;
            C3.Legend.Visible = 1 == 1;
            C3.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
            C3.Legend.SpanPercentage = 25;
            C3.PieChart.OthersCategoryPercent = 0.001;
            C3.PieChart.OthersCategoryText = "Прочие";
            C3.PieChart.RadiusFactor = 100;
            L_PageText.Height = 323; 

            C1.Width = CRHelper.GetChartWidth(2 * CustomReportConst.minScreenWidth / 3.0 - 40 - 5);
            L_PageText.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 3.0 + 25);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "Оценка&nbsp;социально-экономического&nbsp;развития&nbsp;территории<br>&nbsp;в&nbsp;разрезе&nbsp;муниципальных&nbsp;образований";
            CrossLink1.NavigateUrl = "~/reports/SEP_0004_Sakhalin/default.aspx";
        }
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {

                Year.FillDictionaryValues(YearsLoad("years"));
                Year.Title = "Период";
                Year.SelectLastNode();
                Regions.Title = "Территория";
                Regions.FillDictionaryValues(RegionsLoad("regions"));
            }

            Hederglobal.Text = page_title;
            PageSubTitle.Text = String.Format(page_sub_title, Year.SelectedValue.ToLower(), Regions.SelectedValue);
            /*if ((Year.SelectedIndex == 0))
            {
                EmptyLabel.Text = "Нет данных<br>";
                EmptyLabel.Visible = true;
                Table.Visible = false;
            }
            else
            {*/
                EmptyLabel.Visible = false;
                Table.Visible = true;
                int n = CRHelper.MonthNum(Year.SelectedValue.Split(' ')[0]);
                if (!selectedPeriod.Value.EndsWith(Year.SelectedNode.Text.Split(' ')[0] + "]") || !Page.IsPostBack)
                {
                    refreshFlag = true;
                }
                else
                {
                    refreshFlag = false;
                }
                int monthNum = CRHelper.MonthNum(Year.SelectedValue.Split(' ')[0]);
                selectedPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "]";
                RegionSelect.Value = Regions.SelectedValue;
                headerLayout = new GridHeaderLayout(G);
                G.DataBind();
                if (refreshFlag)
                {
                    areaSelect.Value = " [Показатели социальной сферы]";
                    PokSelect.Value = "[СЭР__Показатели МО].[СЭР__Показатели МО].[Все показатели].[" + G.Rows[1].Cells[0].Text.Split('<')[0].Remove(G.Rows[1].Cells[0].Text.Split('<')[0].LastIndexOf(',')) + "]";
                    G.Rows[1].Activated = true;
                    G.Rows[1].Selected = true;
                    G.Rows[1].Activate();
                }
                Label1.Text = grid_title;
                C1.DataBind();
                C3.DataBind();
                SetupChart();
                if (refreshFlag)
                {
                    Label5.Text = String.Format(chart2_title, G.Rows[1].Cells[0].Text.Split('<')[0].Remove(G.Rows[1].Cells[0].Text.Split('<')[0].LastIndexOf(',')), G.Rows[1].Cells[0].Text.Split('<')[0].Split(',')[1]);
                    Label10.Text = String.Format(chart3_title, "социальной", Year.SelectedValue.Split(' ')[0]);
                    Label2.Text = chart1_title;
                }
           // }
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs;
            if (RegionSettings.Instance.Id == "Sakhalin")
            {
                cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            }
            else
            {
                cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            }
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 1; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption + " год", 0);
            }
            return d;
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs;
            if (RegionSettings.Instance.Id == "Sakhalin")
            {
                cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            }
            else
            {
                cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            }
            Dictionary<string, int> d = new Dictionary<string, int>();

            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }


        int Econom_index = 11, Social_index = 0;

        System.Decimal SumSoc = 0;
        System.Decimal SumEconom = 0;


        System.Decimal SocMassa = 0;
        System.Decimal EconomMassa = 0;


        DataTable dtRes = new DataTable();
        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            if (RegionSettings.Instance.Id == "Sakhalin")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатель", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатель", dt);
            }
            for (int i = 0; i < dt.Columns.Count - 2; i++)
            {
                resDt.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }
            object[] o = new object[resDt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                o[0] = dt.Rows[i][0].ToString() + ", " + dt.Rows[i][dt.Rows[i].ItemArray.Length - 1].ToString().ToLower() + "<br>Весовой коэффициент: " + dt.Rows[i][dt.Rows[i].ItemArray.Length - 2].ToString().Replace("+", String.Empty);
                for (int j = 1; j < dt.Columns.Count - 2; j++)
                {
                    o[j] = dt.Rows[i][j];
                }
                resDt.Rows.Add(o);
            }
            G.DataSource = resDt;
        }

        protected void post_Grid(UltraWebGrid G)
        {
            try
            {
                G.Rows[Econom_index].Cells[1].ColSpan = 4;
                G.Rows[Social_index].Cells[1].ColSpan = 4;
            }
            catch { }
            for (int i = 0; i < G.Columns.Count; i++)
            {
                G.Rows[Econom_index].Cells[i].Style.Font.Bold = 1 == 1;
                G.Rows[Social_index].Cells[i].Style.Font.Bold = 1 == 1;
            }
            G.Rows[Econom_index].Cells[1].Text = G.Rows[Econom_index].Cells[1].Text.Split('<')[0];
            G.Rows[Social_index].Cells[1].Text = G.Rows[Social_index].Cells[1].Text.Split('<')[0];
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double colWidth = 0;
            if (IsSmallResolution)
            {
                colWidth = 0.11;
            }
            else
            {
                colWidth = 0.09;
            }
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            //   e.Layout.Bands[0].Columns[2].Hidden = true;
            //   e.Layout.Bands[0].Columns[3].Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.6);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;


            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }

            headerLayout.AddCell("Показатель");
            headerLayout.AddCell("Значение по МО");
              headerLayout.AddCell("Отклонение от значения по субъекту");
            //   headerLayout.AddCell("Отклонение от значения по РФ");
            headerLayout.AddCell("Индикатор");

            headerLayout.ApplyHeaderInfo();
        }

        DataTable dtStackColumnChart;

        private void SetupChart()
        {
            C2.ChartType = ChartType.Composite;
            C2.TitleLeft.Visible = true;
            C2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            C2.TitleLeft.Margins.Bottom = C2.Axis.X.Extent;
            C2.TitleLeft.Font = new System.Drawing.Font("Verdana", 10);

            C2.Legend.MoreIndicatorText = " ";
            C2.Legend.SpanPercentage = 30;
            C2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            Color color1 = Color.LimeGreen;
            C2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            ChartArea area = new ChartArea();
            area.Border.Thickness = 0;
            C2.CompositeChart.ChartAreas.Add(area);
            AxisItem axisX = new AxisItem();
            axisX.OrientationType = AxisNumber.X_Axis;
            axisX.SetLabelAxisType = SetLabelAxisType.GroupBySeries;
            axisX.DataType = AxisDataType.String;
            axisX.LineThickness = 1;
            axisX.Extent = 80;
            axisX.Key = "X";

            AxisItem axisX2 = new AxisItem();
            axisX2.OrientationType = AxisNumber.X_Axis;
            axisX2.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            axisX2.DataType = AxisDataType.String;
            axisX2.Visible = false;
            axisX2.Labels.Visible = false;
            axisX2.Key = "X2";

            dtStackColumnChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), "d", dtStackColumnChart);
            double max = double.Parse(dtStackColumnChart.Rows[0][1].ToString());
            double min = double.Parse(dtStackColumnChart.Rows[0][1].ToString());
            for (int i = 0; i < dtStackColumnChart.Rows.Count; i++)
            {
                if (double.Parse(dtStackColumnChart.Rows[i][1].ToString()) > max)
                {
                    max = double.Parse(dtStackColumnChart.Rows[i][1].ToString());
                }
                if (double.Parse(dtStackColumnChart.Rows[i][2].ToString()) > max)
                {
                    max = double.Parse(dtStackColumnChart.Rows[i][2].ToString());
                }

                if (double.Parse(dtStackColumnChart.Rows[i][1].ToString()) < min)
                {
                    min = double.Parse(dtStackColumnChart.Rows[i][1].ToString());
                }
                if (double.Parse(dtStackColumnChart.Rows[i][2].ToString()) < min)
                {
                    min = double.Parse(dtStackColumnChart.Rows[i][2].ToString());
                }
            }


            AxisItem axisY = new AxisItem();
            axisY.OrientationType = AxisNumber.Y_Axis;
            axisY.DataType = AxisDataType.Numeric;
            axisY.LineThickness = 1;
            axisY.Extent = 60;
            axisY.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            axisY.Labels.HorizontalAlign = StringAlignment.Far;
            axisY.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            axisY.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            axisY.Labels.Layout.Padding = 5;
            axisY.TickmarkStyle = AxisTickStyle.Smart;
           /* axisY.RangeType = AxisRangeType.Custom;
            axisY.RangeMax = max*1.1;
            axisY.RangeMin =0; 
            axisY.MinorGridLines.Visible = false;
            axisY.Labels.SeriesLabels.Visible = false;*/

            AxisItem hiddenAxisX2 = new AxisItem();
            hiddenAxisX2.OrientationType = AxisNumber.X2_Axis;
            hiddenAxisX2.Extent = 20;
            hiddenAxisX2.Labels.Visible = false;
            hiddenAxisX2.LineThickness = 0;
            hiddenAxisX2.Margin.Near.Value = 10;
            hiddenAxisX2.Margin.Far.Value = 10;
            hiddenAxisX2.Visible = true;

            area.Axes.Clear();
            area.Axes.Add(axisX);
            area.Axes.Add(axisX2);
            area.Axes.Add(axisY);
            area.Axes.Add(hiddenAxisX2);

            ChartLayerAppearance layer1 = new ChartLayerAppearance();
            ChartLayerAppearance layer2 = new ChartLayerAppearance();
            ChartLayerAppearance layer3 = new ChartLayerAppearance();

            layer1.ChartType = ChartType.ColumnChart;
            ((ColumnChartAppearance)layer1.ChartTypeAppearance).ColumnSpacing = 1;
            ((ColumnChartAppearance)layer1.ChartTypeAppearance).SeriesSpacing = 0;
            layer1.SwapRowsAndColumns = true;

            layer2.ChartType = ChartType.LineChart;
            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;

            lineAppearance.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            lineAppearance.IconAppearance.PE.Fill = Color.Blue;
            ((LineChartAppearance)layer2.ChartTypeAppearance).LineAppearances.Add(lineAppearance);

            layer3.ChartType = ChartType.LineChart;
            LineAppearance emptylineAppearance = new LineAppearance();

            emptylineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            emptylineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            emptylineAppearance.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            emptylineAppearance.IconAppearance.PE.Fill = Color.Red;

            emptylineAppearance.Thickness = 3;
            ((LineChartAppearance)layer3.ChartTypeAppearance).LineAppearances.Add(emptylineAppearance);


           
            dtStackColumnChart.Columns[2].ColumnName = "Сахалинская область";
           
            for (int i = 1; i < 2; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtStackColumnChart);
                C2.CompositeChart.Series.Add(series);
                layer1.Series.Add(series);
            }
            for (int i = 2; i < 3; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtStackColumnChart);
                C2.CompositeChart.Series.Add(series);
                layer2.Series.Add(series);
            }


            layer1.ChartArea = area;
            layer1.AxisX = axisX;
            layer1.AxisY = axisY;
            layer1.LegendItem = LegendItemType.Point;

            layer2.ChartArea = area;
            layer2.AxisX = axisX2;
            
            layer2.AxisY = axisY;
            layer2.LegendItem = LegendItemType.Series;

            C2.CompositeChart.ChartLayers.Clear();
            C2.CompositeChart.ChartLayers.Add(layer1);
            C2.CompositeChart.ChartLayers.Add(layer2);

            CompositeLegend compositeLegend = new CompositeLegend();
            compositeLegend.ChartLayers.Clear();
            compositeLegend.ChartLayers.Add(layer2);
            compositeLegend.ChartLayers.Add(layer1);
            compositeLegend.PE.ElementType = PaintElementType.SolidFill;
            compositeLegend.PE.Fill = Color.FloralWhite;
            compositeLegend.BoundsMeasureType = MeasureType.Percentage;
            compositeLegend.Bounds = new Rectangle(2, 88, 98, 12);
            compositeLegend.LabelStyle.Font = new System.Drawing.Font("Verdana", 10);
            C2.CompositeChart.Legends.Clear();
            C2.CompositeChart.Legends.Add(compositeLegend);

            C2.Tooltips.FormatString = "<ITEM_LABEL>";
        }


       

        protected void C3_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            if (RegionSettings.Instance.Id == "Sakhalin")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart3"), "chart3", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart3"), "chart3", dt);
            }

            if (dt.Rows.Count > 0)
            {
                C3.DataSource = dt;
                C3.Tooltips.FormatString = "<ITEM_LABEL> - <b><DATA_VALUE:0.##></b>";
                C3.Legend.SpanPercentage = dt.Rows.Count * 4 + 2;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Contains("\""))
                    {
                        dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("\"", "\'");
                    }
                    if (dt.Rows[i][0].ToString().Length > 140)
                    {
                        //C3.Legend.SpanPercentage += 2;
                    }
                }
            }
            else
            {
                C3.DataSource = null;
            }
        }
        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Style.Font.Bold != true)
            {  

                PokSelect.Value = "[СЭР__Показатели МО].[СЭР__Показатели МО].[Все показатели].[" + e.Row.Cells[0].Text.Split('<')[0].Remove(e.Row.Cells[0].Text.Split('<')[0].LastIndexOf(',')) + "]";
                if (e.Row.Cells[e.Row.Cells.Count - 1].Text[0] == '1')
                {
                    areaSelect.Value = " [Показатели социальной сферы]";
                    Label10.Text = String.Format(chart3_title, "социальной", Year.SelectedValue.Split(' ')[0]);
                }
                if (e.Row.Cells[e.Row.Cells.Count - 1].Text[0] == '2')
                {
                    areaSelect.Value = "[Показатели экономической сферы]";
                    Label10.Text = String.Format(chart3_title, "экономической", Year.SelectedValue.Split(' ')[0]);
                }
                if (e.Row.Cells[e.Row.Cells.Count - 1].Text[0] == '3')
                {
                    areaSelect.Value = "[Показатели финансовой сферы]";
                    Label10.Text = String.Format(chart3_title, "финансовой", Year.SelectedValue.Split(' ')[0]);
                }
                SetupChart();
                C3.DataBind();
                
                Label5.Text = String.Format(chart2_title, e.Row.Cells[0].Text.Split('<')[0].Remove(e.Row.Cells[0].Text.Split('<')[0].LastIndexOf(',')), e.Row.Cells[0].Text.Split('<')[0].Split(',')[e.Row.Cells[0].Text.Split('<')[0].Split(',').Length - 1]);
            }
            else
            {
                if (e.Row.Cells[0].Text.StartsWith("Социальная"))
                {
                    areaSelect.Value = " [Показатели социальной сферы]";
                    Label10.Text = String.Format(chart3_title, "социальной", Year.SelectedValue.Split(' ')[0]);
                }
                if (e.Row.Cells[0].Text.StartsWith("Экономическая"))
                {
                    areaSelect.Value = "[Показатели экономической сферы]";
                    Label10.Text = String.Format(chart3_title, "экономической", Year.SelectedValue.Split(' ')[0]);
                }
                if (e.Row.Cells[0].Text.StartsWith("Финансовая"))
                {

                    areaSelect.Value = "[Показатели финансовой сферы]";
                    Label10.Text = String.Format(chart3_title, "финансовой", Year.SelectedValue.Split(' ')[0]);
                }
                C3.DataBind();


            }
        }

        protected void C1_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            if (RegionSettings.Instance.Id == "Sakhalin")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "chart", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "chart", dt);
            }
            C1.DataSource = dt;
            if (dt.Rows.Count > 0)
            {
                DataTable dtDynamicText = new DataTable();
                if (RegionSettings.Instance.Id == "Sakhalin")
                {
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("dynamicText"), "chart", dtDynamicText);
                }
                else
                {
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("dynamicText"), "chart", dtDynamicText);
                }
                int colNum = dtDynamicText.Columns.Count - 1;
                if (Year.SelectedNode.Index == 0 || ((Year.SelectedIndex == 1) && (Regions.SelectedIndex == Regions.GetRootNodesCount() - 1)))
                {
                    L_PageText.Text = "В настроящий момент данные отсутствуют";
                    string s1 = "В <b>{0}</b> <b>интегральный показатель оценки СЭР МО</b> составил <b>{1}</b><br>";
                    string s2 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки социальной сферы</b> равен <b>{0}</b>;        <br>весовой коэффициент в оценке СЭР – <b>{1}</b>;";
                    string s3 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки экономической сферы</b> равен <b>{0}</b>;        <br>весовой коэффициент в оценке СЭР – <b>{1}</b>;";
                    string s4 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки финансовой сферы</b> равен <b>{0}</b>;        <br>весовой коэффициент в оценке СЭР – <b>{1}</b>;";
                    L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ", String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())));
                    if (GetNumber(dtDynamicText.Rows[1].ItemArray[colNum].ToString()) != 0)
                    {
                        L_PageText.Text += String.Format(s2, String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[1].ItemArray[colNum].ToString())), SocKoef);
                    }
                    if (GetNumber(dtDynamicText.Rows[2].ItemArray[colNum].ToString()) != 0)
                    {
                        L_PageText.Text += String.Format(s3, String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[2].ItemArray[colNum].ToString())), EconKoef);
                    }
                    if (GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString()) != 0)
                    {
                        L_PageText.Text += String.Format(s4, String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), FinKoef);
                    }
                }
                else
                {
                    string s1 = "В <b>{0}</b> <b>интегральный показатель оценки СЭР МО</b> составил <b>{1}</b> (прирост к предыдущему году составляет <b>{2}</b>)<br>";
                    string s2 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки социальной сферы</b> равен <b>{0}</b> (прирост к предыдущему году составляет <b>{1}</b>);        <br>весовой коэффициент в оценке СЭР – <b>{2}</b>;";
                    string s3 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки экономической сферы</b> равен <b>{0}</b> (прирост к предыдущему году составляет <b>{1}</b>);        <br>весовой коэффициент в оценке СЭР – <b>{2}</b>;";
                    string s4 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки финансовой сферы</b> равен <b>{0}</b> (прирост к предыдущему году составляет <b>{1}</b>);        <br>весовой коэффициент в оценке СЭР – <b>{2}</b>;";
                    double m = 0;
                    if (GetNumber(dtDynamicText.Rows[0].ItemArray[colNum - 1].ToString()) != 0)
                    {
                        m = GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[0].ItemArray[colNum - 1].ToString());
                        if (m > 0)
                        {
                            L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ", String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m));
                        }
                        else
                        {
                            if (m < 0)
                            {
                                L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ", String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m));
                            }
                            else
                            {
                                L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ", String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m), String.Empty);
                            }
                        }
                    }
                    else
                    {
                        L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ", String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())), "0%", " ");
                    }

                    if (GetNumber(dtDynamicText.Rows[1].ItemArray[colNum].ToString()) != 0)
                    {
                        if (GetNumber(dtDynamicText.Rows[1].ItemArray[colNum - 1].ToString()) != 0)
                        {
                            m = GetNumber(dtDynamicText.Rows[1].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[1].ItemArray[colNum - 1].ToString());
                           
                            if (m > 0)
                            {
                                L_PageText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), String.Format("{0:0.##}", m), SocKoef);
                            }
                            else
                            {
                                if (m < 0)
                                {
                                    L_PageText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), String.Format("{0:0.##}", m), SocKoef);
                                }
                                else
                                {
                                    L_PageText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), "0", SocKoef);
                                }
                            }
                        }
                        else
                        {
                            L_PageText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), "0", SocKoef);
                        }
                    }

                    if (GetNumber(dtDynamicText.Rows[2].ItemArray[colNum].ToString()) != 0)
                    {
                        if (GetNumber(dtDynamicText.Rows[2].ItemArray[colNum - 1].ToString()) != 0)
                        {
                            m = GetNumber(dtDynamicText.Rows[2].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[2].ItemArray[colNum - 1].ToString());
                            if (m > 0)
                            {
                                L_PageText.Text += String.Format(s3, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])), String.Format("{0:0.##}", m), EconKoef);
                            }
                            else
                            { 
                                if (m < 0)
                                {
                                    L_PageText.Text += String.Format(s3, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])), String.Format("{0:0.##}", m), EconKoef);
                                }
                                else
                                {
                                    L_PageText.Text += String.Format(s3, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])),  "0", EconKoef);
                                }
                            }
                        }
                        else
                        {
                            L_PageText.Text += String.Format(s3, String.Format("{0:0.##}", Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])), "0", EconKoef);
                        }
                    }
                    if (GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString()) != 0)
                    {
                        if (GetNumber(dtDynamicText.Rows[3].ItemArray[colNum - 1].ToString()) != 0)
                        {
                            m = GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[3].ItemArray[colNum - 1].ToString());

                            if (m > 0)
                            {
                                L_PageText.Text += String.Format(s4, String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m), FinKoef);
                            }
                            else
                            {
                                if (m < 0)
                                {
                                    L_PageText.Text += String.Format(s4, String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m), FinKoef);
                                }
                                else
                                {
                                    L_PageText.Text += String.Format(s4, String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), String.Empty, String.Format("{0:0.##}", m), FinKoef);
                                }
                            }
                        }
                        else
                        {
                            L_PageText.Text += String.Format(s4, String.Format("{0:0.##}", GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), "0", FinKoef);
                        }
                    }
                }
                L_PageText.Text = L_PageText.Text.Remove(L_PageText.Text.LastIndexOf(';')) + ".";
            }
            else
            {
                C1.DataSource = null;
                L_PageText.Text = "Нет данных";
            }

        }

        protected void C1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        protected double GetNumber(string s)
        {
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    return double.Parse(s);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
            }
            if (e.Row.Cells[e.Row.Cells.Count - 1].Text == "100")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count - 2;
                SocKoef = e.Row.Cells[0].Text.Split(':')[1];
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split('<')[0].Remove(e.Row.Cells[0].Text.Split('<')[0].LastIndexOf(','));
            }
            else
            {
                if (e.Row.Cells[e.Row.Cells.Count - 1].Text == "200")
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                    e.Row.Cells[0].ColSpan = e.Row.Cells.Count - 2;
                    EconKoef = e.Row.Cells[0].Text.Split(':')[1];
                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split('<')[0].Remove(e.Row.Cells[0].Text.Split('<')[0].LastIndexOf(','));
                }
                else
                {
                    if (e.Row.Cells[e.Row.Cells.Count - 1].Text == "300")
                    {
                        e.Row.Cells[0].Style.Font.Bold = true;
                        e.Row.Cells[0].ColSpan = e.Row.Cells.Count - 2;
                        FinKoef = e.Row.Cells[0].Text.Split(':')[1];
                        e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split('<')[0].Remove(e.Row.Cells[0].Text.Split('<')[0].LastIndexOf(','));
                    }
                    else
                    {
                        if (e.Row.Cells[2].Value == null)
                        {
                            e.Row.Cells[2].Text = "-";
                        }
                    }
                }
            }

        }

        protected void C1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {



            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int)xAxis.MapMinimum;
            int xMax = (int)xAxis.MapMaximum;
            //if (double.TryParse(dtChartAverage.Rows[0][0].ToString(), out urfoAverage))
            {
                int fmY = (int)yAxis.Map(1);
                Line line = new Line();
                line.lineStyle.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;//LineDrawStyle.Dot;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(xMin, fmY);
                line.p2 = new Point(xMax, fmY);
                e.SceneGraph.Add(line);

                Text text = new Text();
                text.labelStyle.Font = new System.Drawing.Font("Verdana", 8, FontStyle.Bold);
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMin + 3, fmY - 14, 780, 15);
                text.SetTextString("Уровень стабильности");
                e.SceneGraph.Add(text);
            }
        }

        protected void TransformGridForPDF()
        {
            for (int i = 0; i < G.Rows.Count; i++)
            {
                if (G.Rows[i].Cells[0].Text.Contains("<br>"))
                {
                    G.Rows[i].Cells[0].Text = G.Rows[i].Cells[0].Text.Replace("<br>", "\n");
                }
                else
                {
                    if (G.Rows[i].Cells[0].Style.Font.Bold != true)
                    {
                        G.Rows[i].Cells[0].Text = G.Rows[i].Cells[0].Text.Insert(G.Rows[i].Cells[0].Text.IndexOf("Весовой"), "\n");
                    }
                }
            }

        }

        protected void TransformGridForXLS()
        {
            for (int i = 0; i < G.Rows.Count; i++)
            {
                if (G.Rows[i].Cells[0].Text.Contains("\n"))
                {
                    G.Rows[i].Cells[0].Text = G.Rows[i].Cells[0].Text.Replace("\n", " ");
                }
                if (G.Rows[i].Cells[0].Text.Contains("<br>"))
                {
                    G.Rows[i].Cells[0].Text = G.Rows[i].Cells[0].Text.Replace("<br>", " ");
                }
            }

        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();


            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма1");
            Infragistics.Documents.Excel.Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма2");
            Infragistics.Documents.Excel.Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма3");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;

            ReportExcelExporter1.TitleStartRow = 3;
            TransformGridForXLS();
            G.Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.4);
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            ReportExcelExporter1.Export(C1, Label2.Text, sheet2, 3);
            ReportExcelExporter1.Export(C2, Label5.Text, sheet3, 3);
            ReportExcelExporter1.Export(C3, Label10.Text, sheet4, 3);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 450; ;
            e.Workbook.Worksheets["Диаграмма1"].MergedCellsRegions.Clear();
           // e.Workbook.Worksheets["Диаграмма2"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма3"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма1"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма2"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма2"].Rows[2].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Distributed;
            e.Workbook.Worksheets["Диаграмма2"].Rows[2].Height = 1500;
            e.Workbook.Worksheets["Диаграмма3"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Hederglobal.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();
            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section2 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section3 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section4 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 60;

            TransformGridForPDF();
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(C1, Label2.Text, section2);
            ReportPDFExporter1.Export(C2, Label5.Text, section3);
            ReportPDFExporter1.Export(C3, Label10.Text, section4);
        }
        #endregion

        protected void C3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            SceneGraph scene = e.SceneGraph;

            bool LegendMainBox = false;
            bool flag = false;
            #region Легенда
            int Top = 0;

            for (int i = 0; i < scene.Count; i++)
            {
                Primitive p = scene[i];

                if (p.Path == "Border.Title.Legend")
                {
                    if (p is Infragistics.UltraChart.Core.Primitives.Box)
                    {
                        if (LegendMainBox)
                        {
                            Text CaptionLegend = (Text)scene[i + 1];

                            if (CaptionLegend.GetTextString().Length > 110)
                            {
                                flag = true;
                                CaptionLegend.SetTextString(CaptionLegend.GetTextString().Insert(CaptionLegend.GetTextString().IndexOf(' ', 100) + 1, "\n"));
                            }
                            CaptionLegend.bounds.Y += Top;
                            Box b = (Box)p;
                            b.rect.Y += Top;
                            b.rect.Height = b.rect.Width;
                            if (flag)
                            {
                                Top += 4;
                            }
                        }
                        LegendMainBox = true;

                    }
                }
            }
            #endregion
        }


        bool firstLegendIconChanged = false;
        protected void C2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            string edIsm = Label5.Text.Split('<')[0].Split(',')[Label5.Text.Split('<')[0].Split(',').Length - 1];
            IAdvanceAxis xAxis = (IAdvanceAxis)C2.CompositeChart.ChartLayers[0].ChartLayer.Grid["X"];

            if (xAxis == null)
                return;

            double xMin = xAxis.MapMinimum;
            double axisStep = (xAxis.Map(1) - xAxis.Map(0));


            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline poly = (Polyline)primitive;

                    if (poly.Series != null && (poly.Series.Label == "Сахалинская область"))
                    {
                        if (poly.Series.Label == "Сахалинская область")
                        {
                            poly.PE.StrokeWidth = 2;
                            poly.PE.ElementType = PaintElementType.SolidFill;
                            poly.PE.Fill = Color.Blue;
                            foreach (DataPoint point in poly.points)
                            {
                                point.DataPoint.Label = "Сахалинская область, <b>" + String.Format("{0:0.##}", GetNumber(dtStackColumnChart.Rows[point.Column][2].ToString())) + "</b>, " + edIsm;
                            }
                        }

                        double offsetX = axisStep / 2;

                        for (int j = 0; j < poly.points.Length; j++)
                        {
                            DataPoint point = poly.points[j];
                            point.point = new Point((int)xMin + (int)offsetX, point.point.Y);
                            offsetX += axisStep;
                        }

                    }
                    else if (poly.Path != null && poly.Path.ToLower().Contains("legend"))
                    {
                        if (!firstLegendIconChanged)
                        {
                            poly.PE.Fill = Color.Blue;
                            Ellipse icon = GetCircleIcon(poly, Color.Blue, 3);
                            e.SceneGraph.Add(icon);
                        }
                        firstLegendIconChanged = true;

                    }
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;

                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = Regions.SelectedValue.Replace("\"","'") + ", <b>" + String.Format("{0:0.##}", GetNumber(dtStackColumnChart.Rows[box.Row][1].ToString())) + "</b>," + edIsm;
                    }
                }
            }
        }
        private static Ellipse GetCircleIcon(Polyline polyline, Color color, int radius)
        {
            Point center = new Point(polyline.points[0].point.X + (polyline.points[2].point.X - polyline.points[0].point.X) / 2, polyline.points[0].point.Y);
            Ellipse circle = new Ellipse(center, radius);
            circle.PE.ElementType = PaintElementType.SolidFill;
            circle.PE.Fill = color;

            return circle;
        }


    }
}