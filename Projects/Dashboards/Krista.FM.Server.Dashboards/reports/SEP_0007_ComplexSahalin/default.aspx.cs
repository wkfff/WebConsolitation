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
using Dundas.Maps.WebControl;

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.SEP_0007_ComplexSahalin
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Итоги комплексной оценки социально-экономического развития муниципальных образований";
        private string page_sub_title = "Оценка социально-экономического развития и оценка населением ситуации в ключевых сферах деятельности органов местного самоуправления в разрезе муниципальных образований";
        private string chart_caption = "Распределение муниципальных образований по итогам комплексной оценки социально-экономического развития, {0}";
        private string map_caption = "Результаты интегральной оценки населением ситуации в ключевых сферах деятельности ОМСУ";
        private string grid_caption = "Комплексная оценка социально-экономического развития, {0}";
        private string grid_caption2 = "Распределение муниципальных образований по итогам комплексной оценки социально-экономического развития";
        private string grid_caption3 = "Показатели комплексной оценки социально-экономического развития, {0}, {1}";
        private CustomParam weightKoef { get { return (UserParams.CustomParam("weightKoef", true)); } }
       // private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion", true)); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod", true)); } }
        private CustomParam selectedRegion { get { return (UserParams.CustomParam("selectedRegion", true)); } }
        private CustomParam selectedMeasure { get { return (UserParams.CustomParam("selectedMeasure", true)); } }
        private CustomParam selectedArea { get { return (UserParams.CustomParam("selectedArea", true)); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok",true)); } }
        private string legendTitle = "Результаты интегральной оценки населением\nситуации в ключевых сферах деятельности ОМСУ";
        private string date = "";
        private string mapFolderName = "";
        private GridHeaderLayout headerLayout;
        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;
        int[] rankColumns;
        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.Towns.ToString();
        }
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;
        private bool onWall;
        private bool blackStyle;
        System.Drawing.Font font8;
        System.Drawing.Font font7;
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
          /*  blackStyle = false;
            if (Session["blackStyle"] != null)
            {
                blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            }

            CRHelper.SetPageTheme(this, blackStyle ? "MinfinBlackStyle" : "Minfin");*/
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e); 
            Year.Width = 250;
            ComboRegion.Width = 400;

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            pageWidth = onWall ? int.Parse(RegionSettingsHelper.Instance.GetPropertyValue("WallResolutionWidth")) : (int)Session["width_size"];
            pageHeight = onWall ? int.Parse(RegionSettingsHelper.Instance.GetPropertyValue("WallResolutionHeight")) : (int)Session["height_size"];

            G.Width = CRHelper.GetGridWidth(pageWidth- 50);
            G.Height = Unit.Empty;
            G2.Width = CRHelper.GetGridWidth(pageWidth - 50);
            G2.Height = Unit.Empty;
            G3.Width = CRHelper.GetGridWidth(pageWidth - 50);
            G3.Height = Unit.Empty;
            Chart1.Width = CRHelper.GetGridWidth(pageWidth - 50);
            Chart1.Height = CRHelper.GetGridHeight(pageHeight - 300) ;
            DundasMap.Width = CRHelper.GetGridWidth(pageWidth - 50);
            DundasMap.Height = CRHelper.GetGridHeight(pageHeight - 50);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #region Bigresolution
            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            primitiveSizeMultiplier = onWall ? 4 : 1;

            pageWidth = onWall ? pageWidth : (int)Session["width_size"];

            pageHeight = onWall ? pageHeight : (int)Session["height_size"];

            widthMultiplier = 1;
            if (Session["width_size"] != null && (int)Session["width_size"] != 0)
            {
                widthMultiplier = onWall ? 1.08 * pageWidth / (int)Session["width_size"] : 1;
            }

            Color fontColor = Color.Black;
            //blackStyle ? Color.White : Color.Black;
            font8 = new System.Drawing.Font("Verdana", Convert.ToInt32(8 * fontSizeMultiplier));
            font7 = new System.Drawing.Font("Verdana", Convert.ToInt32(7 * fontSizeMultiplier));


            G2.DisplayLayout.HeaderStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            G2.DisplayLayout.HeaderStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            G2.DisplayLayout.HeaderStyleDefault.BorderWidth = blackStyle ? 1 : onWall ? 3 : 1;

            G2.DisplayLayout.RowStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            G2.DisplayLayout.RowStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            G2.DisplayLayout.RowStyleDefault.BorderWidth = 1;
           
            PageTitle.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            PageSubTitle.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);
            GridCaption2.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);


            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            //    LabelBottomGrid.Text = String.Format("Индикатор {0} / {1} - рост/снижение показателя относительно прошлого / позапрошлого года", greenGradientBar, redGradientBar);

            string bestStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string worseStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            //    LabelBottomGrid0.Text = String.Format("{0} - лучший ранг&nbsp;&nbsp;&nbsp;{1} - худший ранг", bestStar, worseStar);

            if (onWall)
            {
                ComprehensiveDiv.Style.Add("width", pageWidth.ToString() + "px");
                ComprehensiveDiv.Style.Add("height", pageHeight.ToString() + "px");
                PageTitle.Font.Size = Convert.ToInt32(12.5 * fontSizeMultiplier);
                PageSubTitle.Font.Size = Convert.ToInt32(11.5 * fontSizeMultiplier);
            }
            else
            {
                PageTitle.Font.Size = Convert.ToInt32(12.5);
                PageSubTitle.Font.Size = Convert.ToInt32(11.3);
            }

            if (onWall && Page.Master is IMasterPage)
            {
                ((IMasterPage)Page.Master).SetHeaderVisible(false);
            }

            PopupInformer1.Visible = !onWall;
            Year.Visible = !onWall;
            ComboRegion.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
            WallLink.Visible = !onWall;
            ReportExcelExporter1.Visible = !onWall;
            ReportPDFExporter1.Visible = !onWall;
            TableGrid.Visible = !onWall;
            TableGrid2.Visible = !onWall;
            TableChart.Visible = !onWall;
            TableMap.Visible = !onWall;

            #endregion

            CrossLink1.NavigateUrl = "~/reports/SEP_0001_ComplexSahalin/default.aspx";
            CrossLink1.Text = "Показатели&nbsp;для&nbsp;расчета&nbsp;комплексной&nbsp;оценки&nbsp;социально-экономического&nbsp;развития&nbsp;муниципальных&nbsp;образований";
        } 
         

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                Year.FillDictionaryValues(YearsLoad("years"));
                Year.Title = "Период";
                Year.ParentSelect = true;
                Year.SelectLastNode();

                ComboRegion.FillDictionaryValues(RegionLoad("regions"));
                ComboRegion.Title = "Территория";
            }

            mapFolderName = "Субъекты/Сахалин";
          //  baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            PageTitle.Text = page_title;
            Page.Title = page_title;
            
            if (Year.SelectedNode.Level == 0)
            {
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "].[Полугодие 2].[Квартал 4]";
                weightKoef.Value = "Годовой";
                date = selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[') + " год";
                
            }
            else
            {
                int half_num = CRHelper.HalfYearNumByQuarterNum(int.Parse(Year.SelectedValue.Split(' ')[1]));
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие " + half_num.ToString() + "].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
                weightKoef.Value = "Квартальный";
                date = UserComboBox.getLastBlock(selectedPeriod.Value).Split(' ')[1] + " квартал " + selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[') + " год";
            }

            if (ComboRegion.SelectedIndex == 0)
            {
                selectedRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Дальневосточный федеральный округ].[Сахалинская область].[Сахалинская обл.].children";
                TableGrid2.Visible = false;
                if (weightKoef.Value == "Годовой")
                {
                    PageSubTitle.Text = page_sub_title + ", " + selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[') + " год";
                } 
                else
                {
                    PageSubTitle.Text = page_sub_title + ", " + UserComboBox.getLastBlock(selectedPeriod.Value).Split(' ')[1] + " квартал " + selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[') + " год";
                }
            }
            else 
            {
                selectedRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Дальневосточный федеральный округ].[Сахалинская область].[Сахалинская обл.].[" + ComboRegion.SelectedValue + "]";
                TableGrid2.Visible = true;
                if (weightKoef.Value == "Годовой")
                {
                    PageSubTitle.Text = page_sub_title + ", " + UserComboBox.getLastBlock(selectedRegion.Value) + ", " + selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[') + " год";
                }
                else
                {
                    PageSubTitle.Text = page_sub_title + ", " + UserComboBox.getLastBlock(selectedRegion.Value) + ", " + UserComboBox.getLastBlock(selectedPeriod.Value).Split(' ')[1] + " квартал " + selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[')+" год";
                }
                headerLayout2 = new GridHeaderLayout(G3);
            }
            
            headerLayout = new GridHeaderLayout(G);
            headerLayout1 = new GridHeaderLayout(G2);
            
            G.DataBind();
            if (G.DataSource != null)
            {
                
                calculateRank(G, 1, 2);
                calculateRank(G, 5, 6);
                formGrid(G);
                G.Columns.Remove(G.Columns[1]);
                G.Columns.Remove(G.Columns[4]);
                for (int i = 0; i < G.Columns.Count; i++)
                {
                    headerLayout.AddCell(G.Columns[i].Key);
                } 
                headerLayout.ApplyHeaderInfo();
                G.Columns[5].CellStyle.Font.Size = 18;
                G.Columns[2].CellStyle.Font.Size = 18;
                if (ComboRegion.SelectedIndex != 0)
                {
                    for (int i = 0; i < G.Rows.Count; i++)
                    {
                        if (G.Rows[i].Cells[0].Text != UserComboBox.getLastBlock(selectedRegion.Value))
                        {
                            G.Rows[i].Hidden = true;
                        }
                    }
                    G3.DataBind();
                }
                G2.DataBind();
                G2.Rows[0].Cells[G2.Rows[0].Cells.Count - 1].Style.BackColor = Color.Yellow;
                G2.Rows[G2.Rows.Count - 1].Cells[1].Style.BackColor = Color.Gray;
                Chart1.DataBind();
                SetMapSettings();

                GridCaption.Text = String.Format(grid_caption, date);
                GridCaption2.Text = grid_caption2;
                Label1.Text = String.Format(chart_caption, date);
                Label2.Text = map_caption + ", " + date;
                GridCaption3.Text = String.Format(grid_caption3, ComboRegion.SelectedValue, date);
                if (!onWall)
                {
                    EmptyReport.Visible = false;
                    TableGrid.Visible = true;
                    TableGrid1.Visible = true;
                    TableChart.Visible = true;
                    TableMap.Visible = true;
                }
                WallLink.Text = "Для&nbsp;видеостены";
                
                WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());//String.Format("default.aspx?paramlist=onWall=true");
            //    WallLink.NavigateUrl = HttpContext.Current.Request.Url.AbsoluteUri + "?paramlist="+"onWall=true";
            }
            else
            {
                if (!onWall)
                {
                    EmptyReport.Visible = true;
                    TableGrid.Visible = false;
                    TableGrid1.Visible = false;
                    TableGrid2.Visible = false;
                    TableChart.Visible = false;
                    TableMap.Visible = false;
                }
            }
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "years", dt);

            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(dt.Rows[0][2].ToString() + " год", 0);
            d.Add(dt.Rows[0][0].ToString() + " " + dt.Rows[0][2].ToString() + " года", 1);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][2].ToString() != dt.Rows[i - 1][2].ToString())
                {
                    d.Add(dt.Rows[i][2].ToString() + " год", 0);
                }
                d.Add(dt.Rows[i][0].ToString() + " " + dt.Rows[i][2].ToString() + " года", 1);
            }
            return d;
        }

        Dictionary<string, int> RegionLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Region", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все территории", 0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }

        DataTable dtGrid;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Columns.Clear();
            G.Bands.Clear();
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Территория", dtGrid);
            if (dtGrid.Rows.Count < 1)
            {
                G.DataSource = null;
            }
            else
            {
                G.DataSource = dtGrid;
            }
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double width = 0;
            if (IsSmallResolution)
            {
                width = 0.2;
            }
            else
            {
                width = 0.1;
            }
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * (0.2));
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            //   headerLayout.AddCell("Территория");
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Key = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Key.TrimEnd('1');
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * (width));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                //     headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
                //    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            //   headerLayout.ApplyHeaderInfo();
        }

        protected void G1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("R/RP", typeof(string));
            dt.Columns.Add("-", typeof(string));
            dt.Columns.Add("±", typeof(string));
            dt.Columns.Add("+", typeof(string));

            object[] o = new object[dt.Columns.Count];
            o[0] = "A";
            dt.Rows.Add(o);
            o[0] = "B";
            dt.Rows.Add(o);
            o[0] = "C";
            dt.Rows.Add(o);
            for (int i = 0; i < G.Rows.Count; i++)
            {
                string regionName = "";
                if (ComboRegion.SelectedValue == G.Rows[i].Cells[0].Text)
                {
                    regionName = "<span style='color:red;'>"+G.Rows[i].Cells[0].Text+"</span>";
                }
                else
                {
                    regionName = G.Rows[i].Cells[0].Text;
                }
                if (G.Rows[i].Cells[2].Text == "A")
                {
                    switch (G.Rows[i].Cells[5].Text)
                    {
                        case "-": dt.Rows[0][1] += regionName + "<br>"; break;
                        case "±": dt.Rows[0][2] += regionName + "<br>"; break;
                        case "+": dt.Rows[0][3] += regionName + "<br>"; break;
                    }
                }
                if (G.Rows[i].Cells[2].Text == "B")
                {
                    switch (G.Rows[i].Cells[5].Text)
                    {
                        case "-": dt.Rows[1][1] += regionName + "<br>"; break;
                        case "±": dt.Rows[1][2] += regionName + "<br>"; break;
                        case "+": dt.Rows[1][3] += regionName + "<br>"; break;
                    }
                }
                if (G.Rows[i].Cells[2].Text == "C")
                {
                    switch (G.Rows[i].Cells[5].Text)
                    {
                        case "-": dt.Rows[2][1] += regionName + "<br>"; break;
                        case "±": dt.Rows[2][2] += regionName + "<br>"; break;
                        case "+": dt.Rows[2][3] += regionName + "<br>"; break;
                    }
                }
            }
            G2.DataSource = dt;

        }

        protected void G2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            headerLayout1.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true; 
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * (onWall ? 0.2 : 0.1));
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            headerLayout1.AddCell(" ");
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            { 
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * (onWall ? 0.8 : 0.25));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                headerLayout1.AddCell(e.Layout.Bands[0].Columns[i].Key);
                //    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            headerLayout1.ApplyHeaderInfo();
        }

        protected void G2_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.BackColor = Color.White;
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
                
                if (e.Row.Cells[i].Value!=null)
                {
                    e.Row.Cells[i].Text = e.Row.Cells[i].Text.Remove(e.Row.Cells[i].Text.LastIndexOf('<'));
                }
            } 
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
            }
           /* if (e.Row.Cells[1].Value != null)
            {
                e.Row.Cells[1].Value = String.Format("{0:N0}", GetNumber(e.Row.Cells[1].ToString()));
            }
            if (e.Row.Cells[4].Value != null)
            {
                e.Row.Cells[4].Value = String.Format("{0:N0}", GetNumber(e.Row.Cells[4].ToString()));
            }*/
        }


        DataTable dtChart;
        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "Территория", dt);
            dtChart.Columns.Add("Территория", typeof(string));
            dtChart.Columns.Add(dtGrid.Columns[1].ColumnName, typeof(double));
            dtChart.Columns.Add(dtGrid.Columns[4].ColumnName, typeof(double));
            object[] o = new object[dtChart.Columns.Count];
            for (int i = 0; i < G.Rows.Count; i++)
            {
                o[0]=G.Rows[i].Cells[0].Text.Replace("\"", "\'");
                o[1] = -GetNumber(G.Rows[i].Cells[1].Text);
                o[2] = -GetNumber(G.Rows[i].Cells[4].Text);
                dtChart.Rows.Add(o);
            }
            Chart1.DataSource = dtChart;
        }

        #region Обработчики карты
        DataTable dtMap;
        bool smallLegend = false;
        bool revPok = false;
        public void SetMapSettings()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = !onWall;
            DundasMap.NavigationPanel.Visible = !onWall;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 95;
            DundasMap.Viewport.BackColor = blackStyle ? Color.Black : Color.White;
            DundasMap.ColorSwatchPanel.Visible = false;

            DundasMap.Legends.Clear();
            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.Dock = PanelDockStyle.Right;
            legend.DockAlignment = DockAlignment.Far;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 8 * fontSizeMultiplier, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = legendTitle; 
            legend.TitleFont = new System.Drawing.Font("Verdana", 9 * fontSizeMultiplier, FontStyle.Regular);
            DundasMap.Legends.Add(legend);
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();

            dtMap = new DataTable();
            dtMap.Columns.Add(dtGrid.Columns[0].ColumnName, dtGrid.Columns[0].DataType);
            dtMap.Columns.Add(dtGrid.Columns[4].ColumnName, dtGrid.Columns[4].DataType);
            object[] o = new object[dtMap.Columns.Count];
            for (int i = 0; i < G.Rows.Count; i++)
            {
                o[0] = G.Rows[i].Cells[0].Value;
                o[1] = G.Rows[i].Cells[4].Value;
                dtMap.Rows.Add(o); 
            }

            double[] values = new double[dtMap.Rows.Count];
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                values[i] = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
            }
            // Array.Sort(values);



            smallLegend = true;
            LegendItem item = new LegendItem();
            item.Text = String.Format("1-6 место +");
            
            item.Color = Color.Green;
            DundasMap.Legends["CostLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = String.Format("7-13 место ±");
            item.Color = Color.Yellow;
            DundasMap.Legends["CostLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = String.Format("14-19 место -");
            item.Color = Color.Red;
            DundasMap.Legends["CostLegend"].Items.Add(item);

            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");
            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

            // AddMapLayer(DundasMap, mapFolderName, "Водные", CRHelper.MapShapeType.WaterObjects);
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.CalloutTowns);
            FillMapData();

        }

        public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
        {
            hasCallout = false;
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                //CRHelper.SaveToErrorLog(GetNormalName(GetShapeName(shape).ToLower()) + "|" + GetNormalName(patternValue.ToLower()));
                if (GetNormalName(GetShapeName(shape).ToLower()) == GetNormalName(patternValue.ToLower()))
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

        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }

            return shapeName;
        }

        private static string GetNormalName(string p)
        {
            return p.Replace(" район", "").Replace(" городской округ", "").Replace("поселок ", "").Replace("\"", "").Replace("городской округ ", "").Replace("г.", "").Replace("г. ", "").Replace(" го", "").Replace(" мр", "").Replace(" ", "");
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", mapFolder, layerFileName));
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

        public void FillMapData()
        {
            bool hasCallout;

            string shapeHint = "";

            string valueSeparator = IsMozilla ? ". " : "\n";
            shapeHint = "{0} ранг" + " {1}";


            Dictionary<string, int> colTown = new Dictionary<string, int>();

            for (int j = 0; j < dtMap.Rows.Count; j++)
            {

                string subject = dtMap.Rows[j].ItemArray[0].ToString();
                subject = subject.Replace("Городской округ", "").Replace("\"", String.Empty).Replace("Город", "г.").Replace(" городской округ", "").Replace(" район",String.Empty);
                double value = Convert.ToDouble(dtMap.Rows[j].ItemArray[1].ToString());
                ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
                //      Label1.Text = shapeList.Count.ToString();
                foreach (Shape shape in shapeList)
                {
                    shape.Visible = true;
                    shape["Name"] = subject;
                    shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                    shape.ToolTip = String.Format(shapeHint, subject.Replace("\"", "\'"), String.Format("{0:##0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])));
                    shape.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                    string buf = "";
                    if (GetNumber(G.Rows[j].Cells[4].Text) <= 6)
                    {
                        buf += "+";
                    }
                    else
                    {
                        if (GetNumber(G.Rows[j].Cells[4].Text) > 6 && GetNumber(G.Rows[j].Cells[4].Text) < 14)
                        {
                            buf += "±";
                        }
                        else
                        {
                            if (GetNumber(G.Rows[j].Cells[4].Text) >= 14 && GetNumber(G.Rows[j].Cells[4].Text) <= 19)
                            {
                                buf += "-";
                            }
                        }
                    }
                    shape.Text = subject + "\n" + buf;
                    shape.Name = subject;
                    SetShapeTextOffset(shape);
                    shape.TextVisibility = TextVisibility.Shown; 
                    if (Convert.ToDouble(G.Rows[j].Cells[4].Value) <= 6)
                    {
                         shape.Color = Color.Green;
                    }
                    else
                    {
                        if (Convert.ToDouble(G.Rows[j].Cells[4].Value) > 6 && Convert.ToDouble(G.Rows[j].Cells[4].Value) <= 13)
                        {
                            shape.Color = Color.Yellow;
                        }
                        else
                        {
                            if (Convert.ToDouble(G.Rows[j].Cells[4].Value) > 13)
                            {
                                shape.Color = Color.Red; 
                            }
                        }
                    }
                }
            }
        }

        void SetShapeTextOffset(Shape Shape)
        {
            string[] Offsets = new string[4] {
                "Углегорский","Холмский","Анивский","Невельский"
            };

            string ShapeName = Shape.Name;
            foreach (string s in Offsets)
            {
                if (ShapeName.Contains(s))
                {
                    //Shape.Offset.X = -10;
                    Shape.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;

                    if (ShapeName.Contains("Анивский"))
                    {
                        Shape.Text += "        `";
                    }

                    return;
                }
            }
            //Shape.Offset.X = 10;
            Shape.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;

            //SetShapeTextOffset
            //shape.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;

        }

        #endregion

        protected void G3_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataTable resDt = new DataTable();
            if (Detalization.Checked == true)
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2_2"), "Показатель", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2_1"), "Показатель", dt);
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt1.Columns.Add(dt.Columns[i].ColumnName, typeof(string));
            }
            object[] o = new object[dt1.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Rows[i].ItemArray.Length; j++)
                {
                    o[j] = dt.Rows[i].ItemArray[j].ToString();
                }
                dt1.Rows.Add(o);
            }
            for (int i = 1; i < dt1.Columns.Count; i += 2)
            {
                calculateRankDataTable(dt1, i, i + 1);
            }
            dt1.Rows.Remove(dt1.Rows[0]);
            resDt.Columns.Add("Показатель", typeof(string));
            resDt.Columns.Add("Значение",typeof(string));
            resDt.Columns.Add("Место", typeof(string)); 
            o = new object[resDt.Columns.Count];
            int n = 0;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                if (dt1.Rows[i][0].ToString() == ComboRegion.SelectedValue)
                {
                    n = i;
                }
            }
            for (int i = 1; i < dt1.Columns.Count; i += 2)
            {
                if (GetNumber(dt1.Rows[n][i].ToString()) != 0)
                {
                    o[0] = dt1.Columns[i].ColumnName;
                    o[1] = dt1.Rows[n][i];
                    o[2] = dt1.Rows[n][i + 1];
                    resDt.Rows.Add(o);
                }
            }
                G3.DataSource = resDt;
        }

        protected void G3_InitializeLayout(object sender, LayoutEventArgs e)
        {
            headerLayout2.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * (onWall ? 0.25: 0.6));
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            headerLayout2.AddCell("Показатель");
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * (onWall ? 0.7 : 0.1));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                headerLayout2.AddCell(e.Layout.Bands[0].Columns[i].Key);
                
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            headerLayout2.ApplyHeaderInfo();
            
        }

        protected void G3_InitializeRow(object sender, RowEventArgs e)
        {
            string style = "";
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position: 5px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 30px"; }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
                
            }
            if (e.Row.Cells[1].Value != null)
            {
                e.Row.Cells[1].Value = String.Format("{0:##0.00}", GetNumber(e.Row.Cells[1].Text));
            }
            e.Row.Cells[2].Style.HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;
            try
            {
                if (e.Row.Cells[2].Text.Contains("В"))
                {
                    
                    e.Row.Cells[2].Style.BackgroundImage = "~/images/starYellowBB.png";
                    e.Row.Cells[2].Title = "Самое высокое значение";
                    e.Row.Cells[2].Style.CustomRules = style;
                    e.Row.Cells[2].Text = e.Row.Cells[2].Text.Replace("В", String.Empty);
                }
                if (e.Row.Cells[2].Text.Contains("Н"))
                {
                    e.Row.Cells[2].Style.BackgroundImage = "~/images/starGrayBB.png";
                    e.Row.Cells[2].Title = "Самое низкое значение";
                    e.Row.Cells[2].Style.CustomRules = style;
                    e.Row.Cells[2].Text = e.Row.Cells[2].Text.Replace("Н", String.Empty);
                }
            }
            catch { }
            if (Detalization.Checked != true)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            else
            {
                if (e.Row.Cells[0].Text.StartsWith("Интегр") || e.Row.Cells[0].Text.StartsWith("Сводная") || e.Row.Cells[0].Text.StartsWith("Итог"))
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                }
            }
        }

        protected void Chart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.PointSet)
                {
                    Infragistics.UltraChart.Core.Primitives.PointSet pointSet = (Infragistics.UltraChart.Core.Primitives.PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        string buf = "";
                        if (Math.Abs(GetNumber(dtChart.Rows[point.Row][1].ToString())) <= 6)
                        {
                            buf += "(A,";
                        }
                        else
                        {
                            if (Math.Abs(GetNumber(dtChart.Rows[point.Row][1].ToString())) > 6 && Math.Abs(GetNumber(dtChart.Rows[point.Row][1].ToString())) < 14)
                            {
                                buf += "(B,";
                            }
                            else
                            {
                                if (Math.Abs(GetNumber(dtChart.Rows[point.Row][1].ToString())) >= 14 && Math.Abs(GetNumber(dtChart.Rows[point.Row][1].ToString())) <= 19)
                                {
                                    buf += "(C,";
                                }
                            }
                        }

                        if (Math.Abs(GetNumber(dtChart.Rows[point.Row][2].ToString())) <= 6)
                        {
                            buf += "+)";
                        }
                        else
                        {
                            if (Math.Abs(GetNumber(dtChart.Rows[point.Row][2].ToString())) > 6 && Math.Abs(GetNumber(dtChart.Rows[point.Row][2].ToString())) < 14)
                            {
                                buf += "±)";
                            }
                            else
                            {
                                if (Math.Abs(GetNumber(dtChart.Rows[point.Row][2].ToString())) >= 14 && Math.Abs(GetNumber(dtChart.Rows[point.Row][2].ToString())) <= 19)
                                {
                                    buf += "-)";
                                }
                            }
                        }
                        point.DataPoint.Label = dtChart.Rows[point.Row][0].ToString() + " " + buf;
                    }
                }
                else
                {
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                    {
                        Infragistics.UltraChart.Core.Primitives.Text text = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                        if (text.Path=="Border.Title.Grid.X")
                        {
                            if (text.GetTextString() == "-3")
                            {
                                Text text1 = new Text();
                                text1.labelStyle.Font = new System.Drawing.Font("Verdana", 10, FontStyle.Bold);
                                text1.PE.Fill = Color.Black;
                                text1.bounds = new Rectangle(text.bounds.X, text.bounds.Y+10, 20, 20);
                                text1.SetTextString("+");
                                e.SceneGraph.Add(text1);


                            }
                            if (text.GetTextString() == "-10")
                            {
                                Text text1 = new Text();
                                text1.labelStyle.Font = new System.Drawing.Font("Verdana", 10, FontStyle.Bold);
                                text1.PE.Fill = Color.Black;
                                text1.bounds = new Rectangle(text.bounds.X, text.bounds.Y + 10, 20, 20);
                                text1.SetTextString("±");
                                e.SceneGraph.Add(text1);
                            }
                            if (text.GetTextString() == "-17")
                            {
                                Text text1 = new Text();
                                text1.labelStyle.Font = new System.Drawing.Font("Verdana", 10, FontStyle.Bold);
                                text1.PE.Fill = Color.Black;
                                text1.bounds = new Rectangle(text.bounds.X, text.bounds.Y + 10, 20, 20);
                                text1.SetTextString("-");
                                e.SceneGraph.Add(text1);
                            }
                            if (text.GetTextString() == "-1")
                            {
                                Text text1 = new Text();
                                text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9);
                                text1.PE.Fill = Color.Black;
                                text1.bounds = new Rectangle(text.bounds.X + 20, text.bounds.Y +15, 45, 20);
                                text1.SetTextString("Место");
                                e.SceneGraph.Add(text1);
                            }
                            text.SetTextString(text.GetTextString().Replace("-", String.Empty));
                        }
                        else
                        {
                            if (text.Path == "Border.Title.Grid.Y")
                            {
                                if (text.GetTextString() == "-3")
                                {
                                    Text text1 = new Text();
                                    text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
                                    text1.PE.Fill = Color.Black;
                                    text1.bounds = new Rectangle(text.bounds.X, text.bounds.Y, 20, 20);
                                    text1.SetTextString("A");
                                    e.SceneGraph.Add(text1);
                                }
                                if (text.GetTextString() == "-10")
                                {
                                    Text text1 = new Text();
                                    text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
                                    text1.PE.Fill = Color.Black;
                                    text1.bounds = new Rectangle(text.bounds.X, text.bounds.Y, 20, 20);
                                    text1.SetTextString("B");
                                    e.SceneGraph.Add(text1);
                                }
                                if (text.GetTextString() == "-17")
                                {
                                    Text text1 = new Text();
                                    text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
                                    text1.PE.Fill = Color.Black;
                                    text1.bounds = new Rectangle(text.bounds.X, text.bounds.Y, 20, 20);
                                    text1.SetTextString("C");
                                    e.SceneGraph.Add(text1);
                                }
                                if (text.GetTextString() == "-2")
                                {
                                    Text text1 = new Text();
                                    text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9);
                                    text1.labelStyle.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
                                    text1.PE.Fill = Color.Black;
                                    text1.bounds = new Rectangle(text.bounds.X+20, text.bounds.Y - 40, 20, 45);
                                    text1.SetTextString("Место");
                                    e.SceneGraph.Add(text1);
                                }
                                text.SetTextString(text.GetTextString().Replace("-", String.Empty));
                            }
                            else
                            {
                                if (text.GetTextString() == "Городской округ \'Александровск-Сахалинский район\'")
                                {
                                    text.SetTextString("Городской округ\n\'Александровск-Сахалинский\' район");
                                    text.bounds.Y -= 7;
                                }
                            }
                        }
                    }
                }
            }
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int)xAxis.MapMinimum;
            int xMax = (int)xAxis.MapMaximum;
            int yMax = (int)yAxis.MapMaximum;
            int yMin = (int)yAxis.MapMinimum;

            int fmY = (int)yAxis.Map(0);

            int fmx = (int)xAxis.Map(-6.5);
            Line line = new Line();
            line.lineStyle.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dash;
            line.PE.Stroke = Color.Red;
            line.PE.StrokeWidth = 1;
            line.p1 = new Point(fmx, yMin);
            line.p2 = new Point(fmx, yMax);
            e.SceneGraph.Add(line);

            fmx = (int)xAxis.Map(-13.5);
            line = new Line();
            line.lineStyle.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dash;
            line.PE.Stroke = Color.Red;
            line.PE.StrokeWidth = 1;
            line.p1 = new Point(fmx, yMin);
            line.p2 = new Point(fmx, yMax);
            e.SceneGraph.Add(line);

            fmY = (int)yAxis.Map(-6.5);
            line = new Line();
            line.lineStyle.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dash;
            line.PE.Stroke = Color.Red;
            line.PE.StrokeWidth = 1;
            line.p1 = new Point(xMin, fmY);
            line.p2 = new Point(xMax, fmY);
            e.SceneGraph.Add(line);

            fmY = (int)yAxis.Map(-13.5);
            line = new Line();
            line.lineStyle.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dash;
            line.PE.Stroke = Color.Red;
            line.PE.StrokeWidth = 1;
            line.p1 = new Point(xMin, fmY);
            line.p2 = new Point(xMax, fmY);
            e.SceneGraph.Add(line);
            
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

        #region Экспорт в Excel
        private void TransformGrid()
        {
            for (int i = 0; i < headerLayout1.Grid.Rows.Count; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    try
                    {
                        if (headerLayout1.Grid.Rows[i].Cells[j].Text != "")
                        {
                            headerLayout1.Grid.Rows[i].Cells[j].Text = headerLayout1.Grid.Rows[i].Cells[j].Text.Replace("</span>", String.Empty).Replace("<span style='color:red;'>", String.Empty).Replace("<br>", ", ");

                        }
                    }
                    catch { }
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();

            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица1");
            Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Таблица2");
            Infragistics.Documents.Excel.Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма");
            Infragistics.Documents.Excel.Worksheet sheet4 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.TitleStartRow = 0;
            ReportExcelExporter1.SheetColumnCount = 15;
            TransformGrid();
            ReportExcelExporter1.Export(headerLayout, GridCaption.Text, sheet1, 4);
            if (ComboRegion.SelectedIndex != 0)
            {
                ReportExcelExporter1.TitleStartRow = 9;
                for (int i = 0; i < headerLayout.Grid.Rows.Count; i++)
                {
                    headerLayout.Grid.Rows[i].Hidden = false;
                }
                ReportExcelExporter1.Export(headerLayout, GridCaption.Text, sheet1, 12);
            }
            ReportExcelExporter1.TitleStartRow = 0;
            Chart1.Width = 900;
            DundasMap.Width = 1000; 

            ReportExcelExporter1.Export(headerLayout1,GridCaption2.Text, sheet2, 3);

            ReportExcelExporter1.Export(Chart1, Label1.Text, sheet3, 3);

            ReportExcelExporter1.Export(DundasMap, Label2.Text, sheet4, 3);

            if (ComboRegion.SelectedIndex != 0) 
            {
                Infragistics.Documents.Excel.Worksheet sheet5 = workbook.Worksheets.Add("Таблица3");
                ReportExcelExporter1.Export(headerLayout2, GridCaption3.Text, sheet5, 3);
            }

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
            e.Workbook.Worksheets["Диаграмма"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Карта"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Таблица1"].Rows[1].Height = 500;
            e.Workbook.Worksheets["Таблица1"].Rows[0].Height = 700;

        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            PageTitle.Font.Size = 9;
            PageSubTitle.Font.Size = 8;
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section2 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section3 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section4 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 60;
            Chart1.Width = 900;
            DundasMap.Width = 800;
            DundasMap.Height = 700;
            TransformGrid();
            ReportPDFExporter1.Export(headerLayout,GridCaption.Text, section1);
            ReportPDFExporter1.Export(headerLayout1,GridCaption2.Text, section2);
            ReportPDFExporter1.Export(Chart1, Label1.Text, section3);
            ReportPDFExporter1.Export(DundasMap, Label2.Text, section4);
            if (ComboRegion.SelectedIndex != 0)
            {
                Infragistics.Documents.Reports.Report.Section.ISection section5 = report.AddSection();
                ReportPDFExporter1.Export(headerLayout2, GridCaption3.Text, section5);
            }
        }
        #endregion

        protected void formGrid(UltraWebGrid Grid)
        {
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                if (GetNumber(Grid.Rows[i].Cells[2].Text) >= 1 && GetNumber(Grid.Rows[i].Cells[2].Text) <= 6)
                {
                    Grid.Rows[i].Cells[3].Text = "A";
                    Grid.Rows[i].Cells[4].Text = "высокая";
                }
                else
                {
                    if (GetNumber(Grid.Rows[i].Cells[2].Text) >= 7 && GetNumber(Grid.Rows[i].Cells[2].Text) <= 13)
                    {
                        Grid.Rows[i].Cells[3].Text = "B";
                        Grid.Rows[i].Cells[4].Text = "средняя";
                    }
                    else
                    {
                        if (GetNumber(Grid.Rows[i].Cells[2].Text) >= 14 && GetNumber(Grid.Rows[i].Cells[2].Text) <= 19)
                        {
                            Grid.Rows[i].Cells[3].Text = "C";
                            Grid.Rows[i].Cells[4].Text = "низкая";
                        }
                    }
                }

                if (GetNumber(Grid.Rows[i].Cells[6].Text) >= 1 && GetNumber(Grid.Rows[i].Cells[6].Text) <= 6)
                {
                    Grid.Rows[i].Cells[7].Text = "+";
                    Grid.Rows[i].Cells[8].Text = "высокая";
                }
                else
                {
                    if (GetNumber(Grid.Rows[i].Cells[6].Text) >= 7 && GetNumber(Grid.Rows[i].Cells[6].Text) <= 13)
                    {
                        Grid.Rows[i].Cells[7].Text = "±";
                        Grid.Rows[i].Cells[8].Text = "средняя";
                    }
                    else
                    {
                        if (GetNumber(Grid.Rows[i].Cells[6].Text) >= 14 && GetNumber(Grid.Rows[i].Cells[6].Text) <= 19)
                        {
                            Grid.Rows[i].Cells[7].Text = "-";
                            Grid.Rows[i].Cells[8].Text = "низкая";
                        }
                    }
                }
            }
        }

        protected void calculateRank(UltraWebGrid Grid, int colNumber, int colRankNumber)
        {
            string style = "";
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position: 15px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 50px"; }
            int m = 0;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                {
                    m += 1;

                }
                Grid.Rows[i].Cells[colRankNumber].Text = "0";
            }

            if (m != 0)
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 0; i < Grid.Rows.Count; i++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                    {
                        rank[m] = Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value);
                        m += 1;
                        Grid.Rows[i].Cells[colRankNumber].Text = "0";
                    }
                    else
                    {
                        Grid.Rows[i].Cells[colRankNumber].Text = String.Empty;
                    }

                }
                Array.Sort(rank);

                m = 1;
                for (int i = rank.Length - 1; i >= 0; i--)
                {

                    for (int j = 0; j < Grid.Rows.Count; j++)
                    {
                        if (rank[i] == GetNumber(Grid.Rows[j].Cells[colNumber].Text))
                        {
                            if (Grid.Rows[j].Cells[colRankNumber].Text == "0")
                            {
                                Grid.Rows[j].Cells[colRankNumber].Text = String.Format("{0:0}", m);
                                if ((m) == 1)
                                {

                                    Grid.Rows[j].Cells[colRankNumber].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    Grid.Rows[j].Cells[colRankNumber].Title = "Самое высокое значение";
                                    Grid.Rows[j].Cells[colRankNumber].Style.CustomRules = style;
                                }
                                if (m == rank.Length)
                                {

                                    Grid.Rows[j].Cells[colRankNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    Grid.Rows[j].Cells[colRankNumber].Title = "Самое низкое значение";
                                    Grid.Rows[j].Cells[colRankNumber].Style.CustomRules = style;
                                }
                            }
                        }
                    }
                    if (i != 0)
                    {
                        if (rank[i] != rank[i - 1])
                        {
                            m += 1;
                        }
                    }
                    else
                    { m += 1; }

                }
                double max = GetNumber(Grid.Rows[0].Cells[colRankNumber].Text);
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[j].Cells[colNumber].Value) == true && Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colRankNumber].Text) > max)
                        {
                            max = GetNumber(Grid.Rows[j].Cells[colRankNumber].Text);
                        }
                    }
                }
                for (int j = 0; j < Grid.Rows.Count; j++)
                {

                    if (GetNumber(Grid.Rows[j].Cells[colRankNumber].Text) == max)
                    {
                        Grid.Rows[j].Cells[colRankNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                        Grid.Rows[j].Cells[colRankNumber].Title = "Самое низкое значение";
                        Grid.Rows[j].Cells[colRankNumber].Style.CustomRules = style;
                    }

                }
            }

        }

        protected void calculateRankDataTable(DataTable Grid, int colNumber, int colRankNumber)
        {
            bool reverseFlag = false ;
            if (Grid.Rows[0][colNumber].ToString() !="-1")
            {
                reverseFlag = false;
            }
            else
            {
                reverseFlag = true;
            }
            int m = 0;
            for (int i = 1; i < Grid.Rows.Count; i++)
            {
                if (GetNumber(Grid.Rows[i][colNumber].ToString()) != 0)
                {
                    m += 1;
                    Grid.Rows[i][colRankNumber] = 0;
                }
                else
                {
                    Grid.Rows[i][colRankNumber] = DBNull.Value;
                }
                
            }

            if (m != 0)
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 1; i < Grid.Rows.Count; i++)
                {
                    if (GetNumber(Grid.Rows[i][colNumber].ToString()) != 0)
                    {
                        rank[m] = GetNumber(Grid.Rows[i][colNumber].ToString());
                        m += 1;
                        Grid.Rows[i][colRankNumber] = 0;
                    }
                    else
                    {
                        Grid.Rows[i][colRankNumber] = DBNull.Value;
                    }

                }
                Array.Sort(rank);
                if (reverseFlag)
                {
                    m = rank.Length;
                }
                else
                {
                    m = 1;
                }
                for (int i = rank.Length - 1; i >= 0; i--)
                {

                    for (int j = 1; j < Grid.Rows.Count; j++)
                    {
                        if (rank[i] == GetNumber(Grid.Rows[j][colNumber].ToString()))
                        {
                            if (Grid.Rows[j][colRankNumber].ToString() == "0")
                            {
                                Grid.Rows[j][colRankNumber] =  m;
                                if ((m) == 1)
                                {
                                    Grid.Rows[j][colRankNumber] += "В";
                                }
                                if (m == rank.Length)
                                {
                                    Grid.Rows[j][colRankNumber] += "Н";
                                }

                            }
                        }
                    }
                    if (i != 0)
                    {
                        if (rank[i] != rank[i - 1])
                        {
                            if (reverseFlag)
                            {
                                m -= 1;
                            }
                            else
                            {
                                m += 1;
                            }
                        }
                    }
                    else
                    {
                        if (reverseFlag)
                        {
                            m -= 1;
                        }
                        else
                        {
                            m += 1;
                        }
                    }

                }

              /*  double max = GetNumber(Grid.Rows[1][colRankNumber].ToString());
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                    if (GetNumber(Grid.Rows[j][colNumber].ToString()) != 0 && Grid.Rows[j][colNumber]!= DBNull.Value)
                    {
                        if (reverseFlag)
                        {
                            if (GetNumber(Grid.Rows[j][colRankNumber].ToString()) < max)
                            {
                                max = GetNumber(Grid.Rows[j][colRankNumber].ToString());
                            }
                        }
                        else
                        {
                            if (GetNumber(Grid.Rows[j][colRankNumber].ToString()) > max)
                            {
                                max = GetNumber(Grid.Rows[j][colRankNumber].ToString());
                            }
                        }
                    }
                }
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                    if (GetNumber(Grid.Rows[j][colRankNumber].ToString()) == max)
                    {
                        Grid.Rows[j][colRankNumber] += "Н";
                    }

                }*/
            }

        }

    }
}