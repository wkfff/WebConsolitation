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

namespace Krista.FM.Server.Dashboards.reports.SEP_0003
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
            if (RegionSettings.Instance.Id == "HMAO")
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

            C2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2.0 - 10 - 5);
            C3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2.0 - 10);
            C3.DeploymentScenario.FilePath = "../../TemporaryImages";
            C3.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png";
            Year.Width = 200;
            Regions.Width = 350;
            C2.Height = 500;
            C3.Height = 500;

            Panel.AddLinkedRequestTrigger(G);
            Panel.AddRefreshTarget(C2);
            Panel.AddRefreshTarget(C3);
            C3.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PieChart;
            C3.PieChart.OthersCategoryPercent = 1;
            C3.Legend.Visible = 1 == 1;
            C3.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
            C3.Legend.SpanPercentage = 50;
            C3.PieChart.OthersCategoryPercent = 0.001;
            C3.PieChart.OthersCategoryText = "Прочие";
            L_PageText.Height = 323;

            C1.Width = CRHelper.GetChartWidth(2 * CustomReportConst.minScreenWidth / 3.0 - 40 - 5);
            L_PageText.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 3.0 + 25);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "Оценка социально-экономического развития территории в разрезе муниципальных образований";
            CrossLink1.NavigateUrl = "~/reports/SEP_0004/default.aspx";
        }
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension+".[Новосибирская область]";
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
            if ((Year.SelectedIndex == 0) && (Regions.SelectedIndex == Regions.GetRootNodesCount()-1))
            {
                EmptyLabel.Text = "Нет данных<br>";
                EmptyLabel.Visible = true;
                Table.Visible = false;
            }
            else
            {
                EmptyLabel.Visible = false;
                Table.Visible = true;
                int n = CRHelper.MonthNum(Year.SelectedValue.Split(' ')[0]);
                if (selectedPeriod.Value.EndsWith(Year.SelectedNode.Text.Split(' ')[0] + "]"))
                { 
                    refreshFlag = false;
                }
                else
                {
                    refreshFlag = true;
                }
                int monthNum = CRHelper.MonthNum(Year.SelectedValue.Split(' ')[0]);
                selectedPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "]";
                //  RegionSelect.Value =RegionSettingsHelper.Instance.RegionBaseDimension+ ".[" + Regions.SelectedValue + "]";
                RegionSelect.Value = Regions.SelectedValue;
                headerLayout = new GridHeaderLayout(G);
                G.DataBind();
                if (refreshFlag || !Page.IsPostBack)
                {
                    areaSelect.Value = " [Показатели социальной сферы]";
                    PokSelect.Value = "[СЭР__Показатели МО].[СЭР__Показатели МО].[Все показатели].[" + G.Rows[1].Cells[0].Text.Split('<')[0].Remove(G.Rows[1].Cells[0].Text.Split('<')[0].LastIndexOf(',')) + "]";
                    G.Rows[1].Activated = true;
                    G.Rows[1].Selected = true;
                    G.Rows[1].Activate();
                    
                    Label10.Text = String.Format(chart3_title, "социальной", Year.SelectedValue.Split(' ')[0]);
                    Label2.Text = chart1_title;
                }
                Label1.Text = grid_title;
                C1.DataBind();
                C3.DataBind();
                C2.DataBind();
                if (refreshFlag || !Page.IsPostBack)
                {
                    Label5.Text = String.Format(chart2_title, G.Rows[1].Cells[0].Text.Split('<')[0].Remove(G.Rows[1].Cells[0].Text.Split('<')[0].LastIndexOf(',')), EdIsm);
                }
            }
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs;
            if (RegionSettings.Instance.Id == "HMAO")
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
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption + " год", 0);
            }
            return d;
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs;
            if (RegionSettings.Instance.Id == "HMAO")
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
            DataTable dt=new DataTable();
            DataTable resDt = new DataTable();
            if (RegionSettings.Instance.Id == "HMAO")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатель", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатель", dt);
            }
            for (int i = 0; i < dt.Columns.Count-2; i++)
            {
                resDt.Columns.Add(dt.Columns[i].ColumnName,dt.Columns[i].DataType);
            }
            object[] o =new object[resDt.Columns.Count];
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.7);
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
          //  headerLayout.AddCell("Отклонение от значения по субъекту");
         //   headerLayout.AddCell("Отклонение от значения по РФ");
            headerLayout.AddCell("Индикатор");

            headerLayout.ApplyHeaderInfo();
        }

        protected void С2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (RegionSettings.Instance.Id == "HMAO")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), "Chart2", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), "Chart2", dt);
            }
            if (dt.Rows.Count > 0)
            {
                EdIsm = dt.Rows[0][1].ToString().ToLower();
                dt.Columns.Remove(dt.Columns[1]);
                C2.DataSource = dt;
            }
            else
            {
                C2.DataSource = null;
            }
            C2.Tooltips.FormatString =Regions.SelectedValue + ", <b><DATA_VALUE:0.##></b> " + EdIsm;
        }

        protected void C3_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            if (RegionSettings.Instance.Id == "HMAO")
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
                C3.Legend.SpanPercentage=dt.Rows.Count*4+4;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Contains("\""))
                    {
                        dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("\"", "\'");
                    }
                    if (dt.Rows[i][0].ToString().Length > 115)
                    {
                        C3.Legend.SpanPercentage += 4;
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
                    Label10.Text = String.Format(chart3_title, "социальной",  Year.SelectedValue.Split(' ')[0]);
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
                C2.DataBind();
                C3.DataBind();

                Label5.Text = String.Format(chart2_title, e.Row.Cells[0].Text.Split('<')[0].Remove(e.Row.Cells[0].Text.Split('<')[0].LastIndexOf(',')), EdIsm);
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
                if (RegionSettings.Instance.Id == "HMAO")
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
                    if (RegionSettings.Instance.Id == "HMAO")
                    {
                        DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("dynamicText"), "chart", dtDynamicText);
                    }
                    else
                    {
                        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("dynamicText"), "chart", dtDynamicText);
                    }
                    int colNum = dtDynamicText.Columns.Count-1;
                    if (Year.SelectedNode.Index == 0 || ((Year.SelectedIndex==1) && (Regions.SelectedIndex==Regions.GetRootNodesCount()-1)))
                    {
                        L_PageText.Text = "В настроящий момент данные отсутствуют";
                        string s1 = "В <b>{0}</b> <b>интегральный показатель оценки СЭР МО</b> составил <b>{1}</b>";
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
                    string s1 = "В <b>{0}</b> <b>интегральный показатель оценки СЭР МО</b> составил <b>{1}</b> (прирост к предыдущему году составляет <b>{2}</b>)";
                    string s2 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки социальной сферы</b> равен <b>{0}</b> (прирост к предыдущему году составляет <b>{1}</b>);        <br>весовой коэффициент в оценке СЭР – <b>{2}</b>;";
                    string s3 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки экономической сферы</b> равен <b>{0}</b> (прирост к предыдущему году составляет <b>{1}</b>);        <br>весовой коэффициент в оценке СЭР – <b>{2}</b>;";
                    string s4 = "        &nbsp;&nbsp;&nbsp;<br>- <b>интегральный показатель оценки финансовой сферы</b> равен <b>{0}</b> (прирост к предыдущему году составляет <b>{1}</b>);        <br>весовой коэффициент в оценке СЭР – <b>{2}</b>;";
                        double m = 0;
                        if (GetNumber(dtDynamicText.Rows[0].ItemArray[colNum - 1].ToString()) != 0)
                        {
                            m = GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[0].ItemArray[colNum - 1].ToString());
                            if (m > 0)
                            {
                                L_PageText.Text = String.Format(s1,   Year.SelectedValue.Split(' ')[0] +" году ", String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())),String.Format("{0:0.##}",m));
                            }
                            else
                            {
                                if (m < 0)
                                {
                                    L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ",String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m));
                                }
                                else
                                {
                                    L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ", String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m), String.Empty);
                                }
                            }
                        }
                        else
                        {
                            L_PageText.Text = String.Format(s1, Year.SelectedValue.Split(' ')[0] + " году ", String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[0].ItemArray[colNum].ToString())), "0%", " ");
                        }

                        if (GetNumber(dtDynamicText.Rows[1].ItemArray[colNum].ToString()) != 0)
                        {
                            if (GetNumber(dtDynamicText.Rows[1].ItemArray[colNum - 1].ToString()) != 0)
                            {
                                m = GetNumber(dtDynamicText.Rows[1].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[1].ItemArray[colNum - 1].ToString());
                                if (m > 0)
                                {
                                    L_PageText.Text += String.Format(s2, String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), String.Format("{0:0.##}", m), SocKoef);
                                }
                                else
                                {
                                    if (m < 0)
                                    {
                                        L_PageText.Text += String.Format(s2, String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), String.Format("{0:0.##}", m), SocKoef);
                                    }
                                    else
                                    {
                                        L_PageText.Text += String.Format(s2,String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), String.Empty, String.Format("{0:0.##}", m), SocKoef);
                                    }
                                }
                            }
                            else
                            {
                                L_PageText.Text += String.Format(s2, String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[1].ItemArray[colNum])), "0", SocKoef);
                            }
                        }

                        if (GetNumber(dtDynamicText.Rows[2].ItemArray[colNum].ToString()) != 0)
                        {
                            if (GetNumber(dtDynamicText.Rows[2].ItemArray[colNum - 1].ToString()) != 0)
                            {
                                m = GetNumber(dtDynamicText.Rows[2].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[2].ItemArray[colNum - 1].ToString());
                                if (m > 0)
                                {
                                    L_PageText.Text += String.Format(s3, String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])), String.Format("{0:0.##}", m), EconKoef);
                                }
                                else
                                {
                                    if (m < 0)
                                    {
                                        L_PageText.Text += String.Format(s3, String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])), String.Format("{0:0.##}", m), EconKoef);
                                    }
                                    else
                                    {
                                        L_PageText.Text += String.Format(s3, String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])), String.Empty, String.Format("{0:0.##}", m), EconKoef);
                                    }
                                }
                            }
                            else
                            {
                                L_PageText.Text += String.Format(s3, String.Format("{0:0.##}",Convert.ToDouble(dtDynamicText.Rows[2].ItemArray[colNum])), "0", EconKoef);
                            }
                        }
                        if (GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString()) != 0)
                        {
                            if (GetNumber(dtDynamicText.Rows[3].ItemArray[colNum - 1].ToString()) != 0)
                            {
                                m = GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString()) - GetNumber(dtDynamicText.Rows[3].ItemArray[colNum - 1].ToString());

                                if (m > 0)
                                {
                                    L_PageText.Text += String.Format(s4,String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m), FinKoef);
                                }
                                else
                                {
                                    if (m < 0)
                                    {
                                        L_PageText.Text += String.Format(s4, String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), String.Format("{0:0.##}", m), FinKoef);
                                    }
                                    else
                                    {
                                        L_PageText.Text += String.Format(s4,String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), String.Empty, String.Format("{0:0.##}", m), FinKoef);
                                    }
                                }
                            }
                            else
                            {
                                L_PageText.Text += String.Format(s4, String.Format("{0:0.##}",GetNumber(dtDynamicText.Rows[3].ItemArray[colNum].ToString())), "0", FinKoef);
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
                        if (e.Row.Cells[2].Value==null)
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
            e.Workbook.Worksheets["Диаграмма2"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма3"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма1"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма2"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма3"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Hederglobal.Text;
            ReportPDFExporter1.PageSubTitle =PageSubTitle.Text;
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

                            if (CaptionLegend.GetTextString().Length > 100)
                            {
                                flag = true;
                                CaptionLegend.SetTextString(CaptionLegend.GetTextString().Insert(CaptionLegend.GetTextString().IndexOf(' ', 90) + 1, "\n"));
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


    }
}