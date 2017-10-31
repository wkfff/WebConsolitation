using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Shared.Styles;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Graphics = System.Drawing.Graphics;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using System.Drawing.Imaging;
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebNavigator;
using System.Globalization;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0004new0.EO_0004LeninRegionNew2
{
    public partial class _default : CustomReportPage
    {
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam curYear { get { return (UserParams.CustomParam("curYear")); } }
        private CustomParam tableArea { get { return (UserParams.CustomParam("tableArea")); } }
        private CustomParam chartArea { get { return (UserParams.CustomParam("chartArea")); } }
        private CustomParam chartMeasure { get { return (UserParams.CustomParam("chartMeasure")); } }
        private CustomParam dataBase { get { return (UserParams.CustomParam("dataBase")); } }
        private CustomParam pokPrefix { get { return (UserParams.CustomParam("pokPrefix")); } }
        private string page_title = "Оценка качества жизни населения (в разрезе муниципальных образований)";
        string page_title2 = "Оценка качества жизни населения по основным направлениям: демография, образование, здравоохранение, социальная защита, культура и отдых, жилищные условия, экономика в разрезе муниципальных образований, {0} год";
        private string grid_title = "Интегральный показатель уровня жизни населения и его составляющие";
        private string grid_title1 = "Индикатор сферы «{0}» и его составляющие";
        private string chart1_title = "Распределение муниципальных образований по интегральному показателю";
        private string chart2_title = "Распределение муниципальных образований по индикатору сферы «{0}»";
        private string chart3_title = "Распределение муниципальных образований по показателю «{0}»";
        private Dictionary<string, int> widths = new Dictionary<string, int>();
        private Dictionary<string, int> kindNumbers = new Dictionary<string, int>();
        private DataTable gridMarks;
        private int screen_width { get { return (int)Session["width_size"]; } }
        private string mapFolderName;
        DataTable dtMap;
        private GridHeaderLayout headerLayout;
        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
        }
        string BN = "IE";

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            Grid.Width = (int)((screen_width) - 3);
            Grid.Height = Unit.Empty;
            Chart1.Width = (int)((screen_width) - 40);
            DundasMap.Width = (int)((screen_width) - 40);
            DundasMap.Height = 800;
            ComboArea.Width = 450;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            if (!Page.IsPostBack)
            {
                ComboArea.Title = "Выберите показатель";
                ComboArea.FillDictionaryValues(AreasLoad("Areas"));
                ComboArea.SetСheckedState(ComboArea.GetRootNodesName(0), true);

                ComboYear.Title = "Год";
                ComboYear.FillDictionaryValues(YearsLoad("Years"));
                ComboYear.SelectLastNode();
                Label3.Text = chart1_title;
            }
            mapFolderName = "Субъекты/ХМАОLev";
            curYear.Value = ComboYear.SelectedValue;
            if (ComboArea.GetRootNodesName(0) == ComboArea.SelectedValue)
            {
                tableArea.Value = "[КЖН__Ленинградская область МО].[КЖН__Ленинградская область МО].[Интегральный показатель]";
                Label2.Text = grid_title;
            }
            else
            {
                tableArea.Value = "[КЖН__Ленинградская область МО].[КЖН__Ленинградская область МО].[Интегральный показатель].[" + ComboArea.SelectedValue + "]";
                Label2.Text = String.Format(grid_title1, ComboArea.SelectedValue);
            }

            Label1.Text = page_title;
            Label5.Text = String.Format(page_title2, ComboYear.SelectedValue);
            headerLayout = new GridHeaderLayout(Grid)   ;
            Grid.DataBind();
            
            if (Grid.DataSource != null)
            {
                calculateRank();
                SetSfereparam();
                Chart1.DataBind();
                Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                Grid.Width = 1200;
                Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                Grid.DisplayLayout.CellClickActionDefault = CellClickAction.NotSet;
                SetMapSettings();
            } 
            HidenEmptyColumn(Grid);
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Года", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                d.Add(dt.Columns[i].ColumnName, 0);
            }
            return d;
        }

        Dictionary<string, int> AreasLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(cs.Axes[1].Positions[0].Members[0].Caption, 0);
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                if ((cs.Axes[1].Positions[i].Members[0].LevelDepth == 1))
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
                }
            }
            return d;
        }

        Dictionary<string, int> AreasLoadChart(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(cs.Axes[1].Positions[0].Members[0].Caption, 0);
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                if ((cs.Axes[1].Positions[i].Members[0].LevelDepth == 1))
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
                }
                if ((cs.Axes[1].Positions[i].Members[0].LevelDepth == 2))
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 2);
                }
            }
            return d;
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


        #region Обработчики грида

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            Page.Session.Remove("Pokazatel");
            Page.Session.Add("Pokazatel", ComboArea.SelectedValue);
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            gridMarks = new DataTable();
            if (ComboArea.SelectedValue == ComboArea.GetRootNodesName(0))
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), " ", dt);
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid_Prop"), " ", gridMarks);
            }
            else
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid1"), " ", dt);
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid_Prop1"), " ", gridMarks);
            }

            if (dt.Rows.Count < 1)
            {
                Grid.DataSource = null;
            }
            else
            {
                if (ComboArea.SelectedValue == ComboArea.GetRootNodesName(0))
                {

                    dt1.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
                    for (int i = 2; i < dt.Columns.Count; i += 2)
                    {
                        dt1.Columns.Add(dt.Columns[i].ColumnName.Split(';')[0], dt.Columns[i].DataType);
                    }
                }
                else
                {
                    DataTable dtEdIsm = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid_Prop_ED"), " ", dtEdIsm);
                    int l = 2;
                    dt1.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
                    dt1.Columns.Add(dt.Columns[2].ColumnName.Split(';')[0], dt.Columns[2].DataType);
                    for (int i = 4; i < dt.Columns.Count; i += 2)
                    {
                        dt1.Columns.Add(dt.Columns[i].ColumnName.Split(';')[0] + ", " + dtEdIsm.Rows[0].ItemArray[l].ToString().ToLower(), dt.Columns[i].DataType);
                        l += 1;
                    }

                }
                object[] o = new object[dt1.Columns.Count];
                object[] o1 = new object[dt1.Columns.Count];
                object[] o2 = new object[dt1.Columns.Count];
                object[] o3 = new object[dt1.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    o[0] = dt.Rows[i].ItemArray[0].ToString();
                    o1[0] = "";
                    o2[0] = "";
                    o3[0] = "Ранг по ХМАО";
                    int k = 1;
                    for (int j = 2; j < dt.Columns.Count; j += 2)
                    {

                        o[k] = dt.Rows[i].ItemArray[j];
                        if (dt.Rows[i].ItemArray[j - 1].ToString() == "")
                        {
                            o1[k] = 0.1234501234;
                        }
                        else
                        {
                            o1[k] = GetNumber(dt.Rows[i].ItemArray[j].ToString()) - GetNumber(dt.Rows[i].ItemArray[j - 1].ToString());
                        }
                        if (dt.Rows[i].ItemArray[j - 1].ToString() == "")
                        {
                            o2[k] = 0.1234501234;
                        }
                        else
                        {
                            if (GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()) == 0)
                            {
                                o2[k] = 0;
                            }
                            else
                            {
                                o2[k] = ((GetNumber(dt.Rows[i].ItemArray[j].ToString()) / GetNumber(dt.Rows[i].ItemArray[j - 1].ToString())) - 1) * 100;
                            }
                        }
                        o3[k] = 0;

                        k += 1;
                    }
                    dt1.Rows.Add(o);
                    dt1.Rows.Add(o1);
                    dt1.Rows.Add(o2);
                    dt1.Rows.Add(o3);
                }
                Grid.DataSource = dt1;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.125);
            e.Layout.Bands[0].Columns[0].CellStyle.BackColor = Color.White;
            GridHeaderCell header = headerLayout.AddCell("Муниципальное образование");
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.097);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                header = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
            }

            e.Layout.Bands[0].Columns[0].Header.Caption = "Муниципальное образование";
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        int gridMarksIndex = 0;
        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if ((e.Row.Index + 1) % 4 == 0)
            {
                e.Row.Cells[0].Style.BorderDetails.WidthTop = 0;
                e.Row.Cells[0].Style.CustomRules = "text-align:right;margin-right:5px;";
                for (int i = 1; i < Grid.Columns.Count; i++)
                {
                    if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) == 0.1234501234)
                    {
                        Grid.Rows[e.Row.Index - 3].Cells[i].Text = String.Format("{0:0.00}", Convert.ToDouble(Grid.Rows[e.Row.Index - 3].Cells[i].Text));
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.WidthBottom = 0;
                        Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.WidthBottom = 0;
                        Grid.Rows[e.Row.Index - 3].Cells[i].Style.BorderDetails.WidthBottom = 0;
                        Grid.Rows[e.Row.Index - 1].Cells[i].Text = "";
                        Grid.Rows[e.Row.Index - 2].Cells[i].Text = "";
                    }
                    else
                    {
                        int mark = 0;
                        if (int.TryParse(gridMarks.Rows[gridMarksIndex].ItemArray[i].ToString(), out mark))
                        {

                        }
                        else
                        { mark = 0; }
                        if (mark == 0)
                        {
                            if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.CssClass = "ArrowDownRed";
                            }
                            if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.CssClass = "ArrowUpGreen";
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.CssClass = "ArrowUpRed";
                            }
                            if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.CssClass = "ArrowDownGreen";

                            }
                        }

                        Grid.Rows[e.Row.Index - 1].Cells[i].Text = String.Format("{0:0.00%}", Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Text) / 100);
                        Grid.Rows[e.Row.Index - 2].Cells[i].Text = String.Format("{0:0.00}", Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[i].Text));
                        Grid.Rows[e.Row.Index - 3].Cells[i].Text = String.Format("{0:0.00}", Convert.ToDouble(Grid.Rows[e.Row.Index - 3].Cells[i].Text));
                        Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Темп прироста к " + (int.Parse(ComboYear.SelectedValue) - 1).ToString();
                        Grid.Rows[e.Row.Index - 2].Cells[i].Title = "Прирост к " + (int.Parse(ComboYear.SelectedValue) - 1).ToString();
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.WidthBottom = 0;
                        Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.WidthBottom = 0;
                        Grid.Rows[e.Row.Index - 3].Cells[i].Style.BorderDetails.WidthBottom = 0;
                    }
                }
                Grid.Rows[e.Row.Index - 3].Cells[0].Style.BorderDetails.WidthBottom = 0;
                Grid.Rows[e.Row.Index - 3].Cells[0].RowSpan = 3;
                gridMarksIndex += 1;
            }

        }

        protected void calculateRank()
        {

            for (int l = 1; l < Grid.Columns.Count; l++)
            {
                int m = 0;
                for (int i = 0; i < Grid.Rows.Count; i += 4)//подсчет ненулевых значений столбца
                {
                    if (GetNumber(Grid.Rows[i].Cells[l].Value.ToString()) != 0)
                    {
                        m += 1;
                    }
                    else
                    {
                        Grid.Rows[i + 3].Cells[l].Text = "";
                    }
                }
                if (m == 0)
                {
                    for (int j = 0; j < Grid.Rows.Count; j += 4)
                    {
                        if (Convert.ToDouble(Grid.Rows[j].Cells[l].Value) == 0)
                        {
                            Grid.Rows[j].Cells[l].Text = "";
                        }
                    }
                }
                else
                {
                    double[] rank = new double[m];
                    m = 0;
                    for (int i = 0; i < Grid.Rows.Count; i += 4)
                    {
                        if (GetNumber(Grid.Rows[i].Cells[l].Value.ToString()) != 0)
                        {
                            rank[m] = GetNumber(Grid.Rows[i].Cells[l].Value.ToString());
                            m += 1;
                        }
                    }
                    int mark = 0;
                    Array.Sort(rank);
                    m = 1;

                    if (GetNumber(gridMarks.Rows[0].ItemArray[l].ToString()) == 0)
                    {
                        m = 1;
                    }
                    else
                    {
                        m = rank.Length;
                    }
                    for (int i = rank.Length - 1; i >= 0; i--)
                    {
                        for (int j = 0; j < Grid.Rows.Count; j += 4)
                        {
                            if (rank[i] == Convert.ToDouble(Grid.Rows[j].Cells[l].Text))
                            {
                                if (Convert.ToDouble(Grid.Rows[j + 3].Cells[l].Value) == 0)
                                {
                                    Grid.Rows[j + 3].Cells[l].Text = m.ToString();

                                    if ((m) == 1)
                                    {
                                        Grid.Rows[j + 3].Cells[l].Style.BackgroundImage = "~/images/starYellowBB.png";
                                        Grid.Rows[j + 3].Cells[l].Style.CustomRules = "background-repeat: no-repeat;background-position: left";
                                    }
                                }
                            }
                        }
                        if (GetNumber(gridMarks.Rows[0].ItemArray[l].ToString()) == 0)
                        {
                            m += 1;
                        }
                        else
                        { m -= 1; }
                    }
                    int max = 0;
                    for (int j = 0; j < Grid.Rows.Count; j += 4)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[l].Text) == 0)
                        {
                            Grid.Rows[j].Cells[l].Text = "";
                        }
                        if (GetNumber(Grid.Rows[j + 3].Cells[l].Text) > max)
                        {
                            max = Convert.ToInt32(Grid.Rows[j + 3].Cells[l].Value);
                        }
                    }
                    for (int j = 0; j < Grid.Rows.Count; j += 4)
                    {
                        if (GetNumber(Grid.Rows[j + 3].Cells[l].Text) == max)
                        {
                            Grid.Rows[j + 3].Cells[l].Style.BackgroundImage = "~/images/starGrayBB.png";
                            Grid.Rows[j + 3].Cells[l].Style.CustomRules = "background-repeat: no-repeat;background-position: left";
                        }
                    }
                }
            }

        }

        void HidenEmptyColumn(UltraWebGrid G)
        {
            int colCount = G.Columns.Count;
            for (int i = 0; i < G.Columns.Count; i++)
            {
                bool empty = 1 == 1;
                for (int j = 0; j < G.Rows.Count; j++)
                {
                    if (!string.IsNullOrEmpty(G.Rows[j].Cells[i].Text))
                    {
                        empty = 1 == 2;
                    }
                }
                if (empty)
                {
                    G.Columns.Remove(G.Columns[i]);
                    i -= 1;
                }              
            }
        }

        #endregion

        #region Обработчики диаграммы

        int medianIndex = 0;
        DataTable medianDT;
        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Дата", dt);

            if (dt.Rows.Count > 0)
            {
                if (chartArea.Value.Split('.').Length == 3)
                {
                    Label3.Text = chart1_title;
                }
                if (chartArea.Value.Split('.').Length == 4)
                {
                    Label3.Text = String.Format(chart2_title, UserComboBox.getLastBlock(chartArea.Value));
                }
                if (chartArea.Value.Split('.').Length == 5)
                {
                    Label3.Text = String.Format(chart3_title, UserComboBox.getLastBlock(chartArea.Value)) + ", " + dt.Rows[0].ItemArray[2].ToString().ToLower();
                }

                if ((dt.Rows[0].ItemArray[2].ToString() != "") && (dt.Rows[0].ItemArray[2].ToString() != "Доля"))
                {
                    Chart1.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:0.00></b>, " + dt.Rows[0].ItemArray[2].ToString().ToLower();
                }
                else
                {
                    Chart1.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:0.00></b>";
                }
                dt.Columns.Remove(dt.Columns[2]);
                double avgValue = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace(" муниципальный район", " р-н");
                        row[0] = row[0].ToString().Replace("Город ", "Г. ");
                    }
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    avgValue += double.Parse(dt.Rows[i].ItemArray[1].ToString());
                }
                medianDT = new DataTable();
                medianDT = dt.Clone();
                avgValue = avgValue / dt.Rows.Count;
                double medianValue = MedianValue(dt, 1);
                int medianIndex = MedianIndex(dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    medianDT.ImportRow(dt.Rows[i]);
                    double value;
                    Double.TryParse(dt.Rows[i][1].ToString(), out value);
                    double nextValue;
                    Double.TryParse(dt.Rows[i + 1][1].ToString(), out nextValue);
                    if (((value <= avgValue) && (nextValue > avgValue)) && (i == medianIndex))
                    {
                        if (medianValue > avgValue)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                            row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dt, 1);
                            medianDT.Rows.Add(row);
                        }
                        else
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dt, 1);
                            medianDT.Rows.Add(row);
                            row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                        }
                    }
                    else
                    {
                        if ((value <= avgValue) && (nextValue > avgValue))
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                        }

                        if (i == medianIndex)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dt, 1);
                            medianDT.Rows.Add(row);
                        }
                    }
                }
                medianDT.ImportRow(dt.Rows[dt.Rows.Count - 1]);
                Chart1.DataSource = medianDT;
                double minValue = Convert.ToDouble(dt.Rows[0][1]), maxValue = Convert.ToDouble(dt.Rows[0][1]);
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    double value = Convert.ToDouble(dt.Rows[i][1]);
                    minValue = value < minValue ? value : minValue;
                    maxValue = value > maxValue ? value : maxValue;
                }
                Chart1.Axis.Y.RangeType = AxisRangeType.Custom;
                Chart1.Axis.Y.RangeMax = maxValue * 1.1;
                Chart1.Axis.Y.RangeMin = minValue * 0.9;
            }
            else
            {
                Chart1.DataSource = null;
            }
        }

        protected void Chart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        protected void Chart1_FillSceneGraph1(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if ((box.DataPoint.Label == "Среднее") || (box.DataPoint.Label == "Медиана"))
                        {
                            PaintElement pe = new PaintElement();
                            pe.Fill = Color.Yellow;
                            pe.FillOpacity = 255;
                            pe.Stroke = Color.Black;
                            box.PE = pe;
                        }
                    }
                }
            }
        }

        private static double MedianValue(DataTable dt, int medianValueColumn)
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

        private static bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }

        private static int MedianIndex(int length)
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

        #endregion

        #region Обработчики карты

        bool integral = 1 == 2;
        public void SetMapSettings()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");
            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;
            DundasMap.RenderType = RenderType.InteractiveImage;
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Left;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Left;
            DundasMap.Viewport.EnablePanning = true;

            // добавляем легенду
            Legend legend = new Legend("CostLegend");


            bool integral = 1 == 2;

            if (chartArea.Value.Split('.').Length == 3)
            {
                legend.Title = "Интегральный показатель";
                integral = 1 == 1;
            }
            if (chartArea.Value.Split('.').Length == 4)
            {
                legend.Title = "Интегральный показатель в сфере \n" + UserComboBox.getLastBlock(chartArea.Value);
                integral = 1 == 1;
            }
            if (chartArea.Value.Split('.').Length == 5)
            {
                legend.Title = UserComboBox.getLastBlock(chartArea.Value);
            }

            legend.AutoFitMinFontSize = 7;

            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.TitleFont = new System.Drawing.Font("MS Sans Serif", 8, FontStyle.Regular);
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            #region rule
            if (!integral)
            {
                // добавляем правила раскраски
                ShapeRule rule = new ShapeRule();
                rule.Name = "CostRule";
                rule.Category = String.Empty;
                rule.ShapeField = "Cost";
                rule.DataGrouping = DataGrouping.Optimal;
                rule.ColorCount = 5;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = Color.Red;
                rule.MiddleColor = Color.Yellow;
                rule.ToColor = Color.Green;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInColorSwatch = false;
                rule.ShowInLegend = "CostLegend";
                //rule.LegendText = string.Format("#FROMVALUE{{{0}}} - #TOVALUE{{{0}}}");
                rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                DundasMap.ShapeRules.Add(rule);
            }
            else
            {

                LegendItem item = new LegendItem();
                item.Text = String.Format("1.00 - 3.00");
                item.Color = GetColor(1.5);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("0.75 - 1.00");
                item.Color = GetColor(0.85);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("0.50 - 0.75");
                item.Color = GetColor(0.55);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("0.25 - 0.50");
                item.Color = GetColor(0.35);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("0.00 - 0.25");
                item.Color = GetColor(0.15);
                DundasMap.Legends["CostLegend"].Items.Add(item);

            }
            #endregion
            dtMap = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Map"), "df", dtMap);
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            FillMapData();
        }

        public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
        {
            hasCallout = false;
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape).ToLower() == patternValue.ToLower())
                {
                    shapeList.Add(shape);
                    if (IsCalloutTownShape(shape))
                    {
                        hasCallout = true;
                    }
                }
            }

            return shapeList;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Clear();
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }

            return shapeName;
        }

        public void FillMapData()
        {
            bool hasCallout;

            string valueSeparator = IsMozilla ? ". " : "\n";
            string shapeHint = "";
            if (chartArea.Value.Split('.').Length == 3)
            {
                shapeHint = "{0}{3}" + valueSeparator + "Интегральный показатель: {1:N2}" + valueSeparator + "Ранг: {2}"; ;
            }
            if (chartArea.Value.Split('.').Length == 4)
            {
                shapeHint = "{0}{3}" + valueSeparator + "Интегральный показатель: {1:N2}" + valueSeparator + "Ранг: {2}"; ;
            }
            if (chartArea.Value.Split('.').Length == 5)
            {
                shapeHint = "{0}{3}" + valueSeparator + UserComboBox.getLastBlock(chartArea.Value) + ": {1:N2} " + Chart1.Tooltips.FormatString.Split(',')[Chart1.Tooltips.FormatString.Split(',').Length - 1] + valueSeparator + "Ранг: {2}"; ;
            }

            string unit;

            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = String.Format("{0}", GetShapeName(shape).Replace(" ", "\n"));
            }

            double[] rank = new double[dtMap.Rows.Count];
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                rank[i] = GetNumber(dtMap.Rows[i].ItemArray[1].ToString());
            }
            Array.Sort(rank);
            int rankValue = 1;
            if (GetNumber(dtMap.Rows[0].ItemArray[2].ToString()) == 0)
            {
                rankValue = 1;
            }
            else
            {
                rankValue = rank.Length;
            }
            for (int i = rank.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < dtMap.Rows.Count; j++)
                {
                    if (rank[i] == GetNumber(dtMap.Rows[j].ItemArray[1].ToString()))
                    {
                        string s = CRHelper.MapShapeType.CalloutTowns.ToString();
                        string subject = dtMap.Rows[j].ItemArray[0].ToString().Replace(" муниципальный район", String.Empty).Replace("Город ", "г.");
                        double value = Convert.ToDouble(dtMap.Rows[j].ItemArray[1].ToString());
                        ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
                        foreach (Shape shape in shapeList)
                        {
                            shape.Visible = true;
                            if (shape.ToolTip == "")
                            {
                                string shapeName = GetShapeName(shape);
                                if (IsCalloutTownShape(shape))
                                {
                                    if (DundasMap.Legends["CostLegend"].Title.Contains("Интегральный показатель"))
                                    {

                                        shape.Color = GetColor(Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                                    }
                                    shape["Name"] = subject;
                                    shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);

                                    shape.ToolTip = String.Format(shapeHint, subject, String.Format("{0:0.00}", GetNumber(dtMap.Rows[j].ItemArray[1].ToString())), rankValue, String.Empty);
                                    shape.TextVisibility = TextVisibility.Shown;
                                    shape.Text = String.Format("{0}\n{1:N2}", shapeName.Replace(" ", "\n"), value);
                                    shape.TextVisibility = TextVisibility.Shown;
                                    shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;


                                }
                                else
                                {
                                    if (DundasMap.Legends["CostLegend"].Title.Contains("Интегральный показатель"))
                                    {

                                        shape.Color = GetColor(Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                                    }
                                    if (!hasCallout)
                                    {
                                        shape["Name"] = subject;
                                        shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                                        shape.ToolTip = String.Format(shapeHint, subject, String.Format("{0:0.00}", GetNumber(dtMap.Rows[j].ItemArray[1].ToString())), rankValue, " р-н");
                                        shape.TextVisibility = TextVisibility.Shown;
                                        shape.Text = String.Format("{0} р-н\n{1:N2}", shapeName.Replace(" ", "\n"), value);
                                    }
                                }
                            }

                        }
                    }

                }
                if (GetNumber(dtMap.Rows[0].ItemArray[2].ToString()) == 0)
                {
                    rankValue += 1;
                }
                else
                {
                    rankValue -= 1;
                }

            }
        }

        Color GetColor(Double val)
        {
            if (val < 0.25)
            {
                return Color.Red;
            }

            if (val < 0.5)
            {

                return Color.OrangeRed;
            }
            if (val < 0.75)
            {
                return Color.Orange;
            }
            if (val < 1)
            {
                return Color.Yellow;
            }
            return Color.Green;

        }

        #endregion

        #region Радиобаттоны

        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("AreasChart"), "dfdf", dt);
            if (PlaceHolder1.Controls.Count != 0)
            {
                RadioButton r1 = (RadioButton)(PlaceHolder1.Controls[0]);
                if (r1.Text != ComboArea.SelectedValue)
                {
                    PlaceHolder1.Controls.Clear();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Random r = new Random();
                        ra = ra++;
                        RadioButton rb = new RadioButton();
                        rb.Style.Add("font-size", "10pt");

                        rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                        rb.Style.Add("font-family", "Verdana");
                        PlaceHolder1.Controls.Add(rb);
                        Label l = new Label();
                        l.Text = "<br>";
                        PlaceHolder1.Controls.Add(l);
                        rb.Text = dt.Rows[i].ItemArray[0].ToString();
                        rb.GroupName = "sfere" + ra.ToString();
                        rb.ValidationGroup = rb.GroupName;
                        rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                        rb.AutoPostBack = 1 == 1;
                        rb.Checked = 1 == 2;
                    }
                    RadioButton r2 = (RadioButton)(PlaceHolder1.Controls[0]);
                    r2.Checked = true;
                    chartArea.Value = tableArea.Value;
                    chartMeasure.Value = "[Measures].[Оценка]";
                }
            }
            else
            {
                PlaceHolder1.Controls.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Random r = new Random();
                    ra = ra++;
                    RadioButton rb = new RadioButton();
                    rb.Style.Add("font-size", "10pt");

                    rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                    rb.Style.Add("font-family", "Verdana");
                    PlaceHolder1.Controls.Add(rb);
                    Label l = new Label();
                    l.Text = "<br>";
                    PlaceHolder1.Controls.Add(l);
                    rb.Text = dt.Rows[i].ItemArray[0].ToString();
                    rb.GroupName = "sfere" + ra.ToString();
                    rb.ValidationGroup = rb.GroupName;
                    rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                    rb.AutoPostBack = 1 == 1;
                    rb.Checked = 1 == 2;
                }
                RadioButton r2 = (RadioButton)(PlaceHolder1.Controls[0]);
                r2.Checked = true;
                chartArea.Value = tableArea.Value;
                chartMeasure.Value = "[Measures].[Оценка]";

            }
        }
        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)(sender);
            rb.Checked = 1 == 1;
            if (rb.Text == ComboArea.SelectedValue)
            {
                chartArea.Value = tableArea.Value;
                chartMeasure.Value = "[Measures].[Оценка]";
            }
            else
            {
                if (ComboArea.SelectedValue == "Интегральный показатель")
                {
                    chartArea.Value = tableArea.Value + ".[" + rb.Text + "]";
                    chartMeasure.Value = "[Measures].[Оценка]";

                }
                else
                {
                    chartArea.Value = tableArea.Value + ".[" + rb.Text + "]";
                    chartMeasure.Value = "[Measures].[Значение]";

                }
            }
            try
            {
                Chart1.DataBind();
                SetMapSettings();
            }
            catch { }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label5.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            int width = int.Parse(Chart1.Width.Value.ToString());
            Chart1.Width = 900;
            ReportExcelExporter1.Export(Chart1, Label3.Text, sheet2, 2);

            int fr = sheet2.MergedCellsRegions[0].FirstRow;
            int lr = sheet2.MergedCellsRegions[0].LastRow;
            int fc = sheet2.MergedCellsRegions[0].FirstColumn;
            int lc = sheet2.MergedCellsRegions[0].LastColumn;
            sheet2.MergedCellsRegions.Clear();
            sheet2.MergedCellsRegions.Add(fr, fc, lr, lc+25);
            Chart1.Width = width;
            ReportExcelExporter.MapExcelExport(sheet3.Rows[3].Cells[0], DundasMap);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 800;
            e.Workbook.Worksheets["Таблица"].Rows[6].Height = 2700;
            
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                Grid.Rows[i].Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
                Grid.Rows[i].Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                if (Grid.Rows[i].Cells[0].Text == "Ранг по ХМАО")
                {
                    Grid.Rows[i].Cells[0].Style.BorderDetails.ColorBottom = Color.LightGray;
                }
            }
            ReportPDFExporter1.HeaderCellHeight = 85;
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label5.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            int width = int.Parse(Chart1.Width.Value.ToString());
            Chart1.Width = 1000;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(Chart1, Label3.Text, section2);
            Chart1.Width = width;
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.6));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            section3.AddImage(img);
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
        }

        #endregion
    }
}
