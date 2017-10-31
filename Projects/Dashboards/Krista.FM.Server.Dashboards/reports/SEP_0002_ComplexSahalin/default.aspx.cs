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

namespace Krista.FM.Server.Dashboards.reports.SEP_0002_ComplexSahalin
{
    public partial class _default : CustomReportPage
    { 
        private string page_title = "Исходные показатели для расчета  комплексной оценки социально-экономического развития в разрезе муниципальных образований";
        private string page_sub_title = "Исходные показатели для расчета комплексной оценки социально-экономического развития муниципальных образований, {0}";
        private string chart_caption = "{0} в разрезе муниципальных образований, {1}";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam prevPeriod { get { return (UserParams.CustomParam("prevPeriod")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
        string pokName = "";
        private string mapFolderName = "";
        private string edIsm = "";
        private bool reverse = true;
        int[] rankColumns;
        
        private GridHeaderLayout headerLayout;
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

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Year.Width = 250;
            ComboPok.Width = 400;
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            DundasMap.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190) * 3 / 2; ;
            G.Height = Unit.Empty;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
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
                ComboPok.Title = "Показатель";
                ComboPok.FillDictionaryValues(PokLoad("pok"));
            }
            mapFolderName = "Субъекты/Сахалин";
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            PageTitle.Text = page_title;
            Page.Title = page_title;
           
            
            
            if (Year.SelectedNode.Level == 0) 
            {
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "].[Полугодие 2].[Квартал 4]";
                prevPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + (int.Parse(Year.SelectedNode.Text.Split(' ')[0]) - 1).ToString() + "].[Полугодие 2].[Квартал 4]";
                PageSubTitle.Text = String.Format(page_sub_title, Year.SelectedValue.ToLower());
                Label1.Text = String.Format(chart_caption, ComboPok.SelectedValue, Year.SelectedValue.ToLower());
                Label2.Text = String.Format(chart_caption, ComboPok.SelectedValue, Year.SelectedValue.ToLower());
            }
            else
            {
                int half_num = CRHelper.HalfYearNumByQuarterNum(int.Parse(Year.SelectedValue.Split(' ')[1]));
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие " + half_num.ToString() + "].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
                prevPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + (int.Parse(Year.SelectedNode.Parent.Text.Split(' ')[0]) - 1).ToString() + "].[Полугодие " + half_num.ToString() + "].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
                PageSubTitle.Text = String.Format(page_sub_title, Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года");
                Label1.Text = String.Format(chart_caption, ComboPok.SelectedValue, Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года");
                Label2.Text = String.Format(chart_caption, ComboPok.SelectedValue, Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года");
            }
            selectedPok.Value = "[Показатели__Комплексная оценка].[Показатели__Комплексная оценка].[Все].[Комплексная оценка СЭР_Сахалин].[Исходные показатели].[" + ComboPok.SelectedNode.Parent.Text + "].[" + ComboPok.SelectedNode.Text+"]";
            
            
            headerLayout = new GridHeaderLayout(G);
            Chart1.DataBind();
            G.DataBind();
            if (dtGrid.Columns.Count > 1 ) 
            {
                if (dtGrid.Columns.Count>3)
                {

                    if (ComboPok.SelectedNode.Parent.Text == "Оценка населением ситуации в ключевых сферах деятельности органов местного самоуправления")
                    { 
                        for (int i = 0; i < rankColumns.Length; i++)
                        {
                            if (G.Columns[rankColumns[i] - 2].Key.StartsWith("Частный"))
                            {
                                calculateRank(G, rankColumns[i] - 2, rankColumns[i]);
                            }
                            else
                            {
                                calculateRank(G, rankColumns[i] - 1, rankColumns[i]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < rankColumns.Length; i++)
                        {
                            if (G.Columns[rankColumns[i] - 2].Key.StartsWith("Индикатор"))
                            {
                                calculateRank(G, rankColumns[i] - 2, rankColumns[i]);
                            }
                            else
                            {
                                calculateRank(G, rankColumns[i] - 1, rankColumns[i]);
                            }
                        }
                    }
                }
                

                SetMapSettings();
                EmptyReport.Visible = false;
                TableGrid.Visible = true;
                TableChart.Visible = true;
                TableMap.Visible = true;

                string prevYear = prevPeriod.Value.Split('.')[3].Replace("[", String.Empty).Replace("]", String.Empty);
               /* if (!CheckBox1.Checked)
                {
                    for (int i = 1; i < G.Columns.Count; i++)
                    {
                        if (G.Columns[i].Header.Caption.Contains(prevYear))
                        {
                            G.Columns[i].Hidden = true;
                            
                         //   G.Columns[i].Header.Dispose();
                        }
                    }
                }*/
            }
            else
            {
                EmptyReport.Visible = true;
                TableGrid.Visible = false;
                TableChart.Visible = false;
                TableMap.Visible = false;
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

        Dictionary<string, int> PokLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "pok", dt);

            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(dt.Rows[0][2].ToString(), 0);
            d.Add(dt.Rows[0][0].ToString(), 1);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][2].ToString() != dt.Rows[i - 1][2].ToString())
                {
                    d.Add(dt.Rows[i][2].ToString(), 0);
                }
                d.Add(dt.Rows[i][0].ToString(),1);
            }
            return d;
        }
        DataTable dtGrid;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            int col = 0;
            G.Columns.Clear();
            G.Bands.Clear();
            dtGrid = new DataTable();
            if (ComboPok.SelectedNode.Parent.Text == "Оценка населением ситуации в ключевых сферах деятельности органов местного самоуправления")
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Территория", dtGrid);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Территория", dtGrid);
            }
            if (dtGrid.Rows.Count > 0) 
            {
                DataTable resDt = new DataTable();
                resDt.Columns.Add(dtGrid.Columns[0].ColumnName, dtGrid.Columns[0].DataType);
                for (int i = 1; i < dtGrid.Columns.Count; i++)
                {
                    resDt.Columns.Add(dtGrid.Columns[i].ColumnName+";"+dtGrid.Rows[0][i].ToString(), typeof(double));
                }
                object[] o = new object[resDt.Columns.Count];
                for (int i = 1; i < dtGrid.Rows.Count; i++)
                {
                    o = new object[resDt.Columns.Count];
                    o[0]=dtGrid.Rows[i][0].ToString().Split(';')[0];
                    for (int j = 1; j < dtGrid.Rows[i].ItemArray.Length; j++)
                    {
                        if (dtGrid.Rows[i][j] != DBNull.Value)
                        {
                            o[j] = Convert.ToDouble(dtGrid.Rows[i][j]);
                        }
                    }
                    resDt.Rows.Add(o);
                }
                if (!CheckBox1.Checked)
                {
                    string prevYear = prevPeriod.Value.Split('.')[3].Replace("[", String.Empty).Replace("]", String.Empty);
                    for (int i = 3; i < resDt.Columns.Count; i++)
                    {
                        if (resDt.Columns[i].ColumnName.Contains(prevYear))
                        {
                            resDt.Columns.Remove(resDt.Columns[i]);
                            i -= 1;
                        }
                    } 
                }
                for (int i = 1; i < resDt.Columns.Count; i++)
                {
                    if (resDt.Columns[i].ColumnName.StartsWith("Ранг"))
                    {
                        col += 1;
                    }
                }
                rankColumns = new int[col];
                col = 0;
                for (int i = 1; i < resDt.Columns.Count; i++)
                {
                    if (resDt.Columns[i].ColumnName.StartsWith("Ранг"))
                    {
                        rankColumns[col] = i;
                        col += 1;
                    }
                }
                G.DataSource = resDt;
            }
            else
            {
                G.DataSource = null;
            }
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double width = 0;
            if (IsSmallResolution)
            {
                width = 0.12;
            }
            else
            {
                width = 0.09;
            } 
            if (dtGrid.Columns.Count>1)
            { 
                headerLayout.childCells.Clear();
                e.Layout.AllowSortingDefault = AllowSorting.No;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
                e.Layout.HeaderStyleDefault.Wrap = true; headerLayout.childCells.Clear();
                e.Layout.AllowSortingDefault = AllowSorting.No;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.15);
                e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * width);
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                    e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                }

                if (dtGrid.Rows.Count > 1)
                {
                    headerLayout.AddCell("Территория");
                    GridHeaderCell headerCell = null;
                    headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Key.Split(';')[0]+", "+edIsm.ToLower());
                    if (Year.SelectedNode.Level == 0)
                    {
                        headerCell.AddCell(e.Layout.Bands[0].Columns[1].Key.Split(';')[2].Split(' ')[2] + " год");
                    }
                    else
                    {
                        headerCell.AddCell(e.Layout.Bands[0].Columns[1].Key.Split(';')[1] + " " + (int.Parse(Year.SelectedNode.Parent.Text.Split(' ')[0]) - 1).ToString() + " года");
                    }
                    for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                    {
                        if (e.Layout.Bands[0].Columns[i - 1].Key.Split(';')[0] != e.Layout.Bands[0].Columns[i].Key.Split(';')[0])
                        {
                            if (e.Layout.Bands[0].Columns[i].Key.StartsWith("Ранг"))
                            {
                                headerCell = headerLayout.AddCell("Ранг");
                            }
                            else
                            {
                                headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[0]);
                            }
                        }
                        if (Year.SelectedNode.Level == 0)
                        {
                            headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[2].Split(' ')[2]+" год");
                        }
                        else
                        {
                            headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[2]);
                        }
                    }
                }
                headerLayout.ApplyHeaderInfo();
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

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Территория", dt);
            if (dt.Rows.Count > 1)
            {
                pokName = ComboPok.SelectedValue;
                if (pokName.Length > 80)
                {
                    pokName = pokName.Insert(pokName.IndexOf(' ', 60) + 1, "<br>");
                    if (pokName.Length > 121)
                    {
                        pokName = pokName.Insert(pokName.IndexOf(' ', 120) + 1, "<br>");
                        if (pokName.Length > 361)
                        {
                            pokName = pokName.Insert(pokName.IndexOf(' ', 360) + 1, "<br>");
                        }
                    } 
                }
                resDt.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
                resDt.Columns.Add(pokName, typeof(double));
                edIsm = dt.Rows[0][1].ToString();

                object[] o = new object[resDt.Columns.Count];
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    o[0] = dt.Rows[i][0].ToString().Split(';')[0].Replace('\"', '\'');
                    o[1] = GetNumber(dt.Rows[i][1].ToString());
                    resDt.Rows.Add(o);
                }
                Chart1.DataSource = resDt;
                if (ComboPok.SelectedValue.StartsWith("Объём отгруженных товаров"))
                {
                    Chart1.Tooltips.FormatString = "<ITEM_LABEL><SERIES_LABEL><br><b><DATA_VALUE:##0.##></b>, " + edIsm.ToLower();
                }
                else
                {
                    Chart1.Tooltips.FormatString = "<ITEM_LABEL><br><SERIES_LABEL><br><b><DATA_VALUE:##0.##></b>, " + edIsm.ToLower();
                }
                Chart1.TitleLeft.Text = edIsm;
            }
            else
            {
                Chart1.DataSource = null;
            }
        } 

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.BackColor = Color.White;
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
                if (G.Columns[i].Key.StartsWith("Ранг"))
                {
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N0}", GetNumber(e.Row.Cells[i].Text));
                    }
                }
                else
                { 
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}", Convert.ToDouble(e.Row.Cells[i].Value));
                    }
                }

            }
        }

        #region Обработчики карты
        DataTable dtMap;
        bool smallLegend = false;
        bool revPok = false;
        double maxMap = 0;
        double minMap = 0;
        double step = 0;
        public void SetMapSettings()
        {
 
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            //DundasMap.Viewport.ViewCenter.X += 3;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 90;
            DundasMap.ColorSwatchPanel.Visible = false;

            DundasMap.Legends.Clear();
            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
       //     legend.Dock = PanelDockStyle.Right;
            legend.DockAlignment = DockAlignment.Far;
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
            legend.Title = pokName.Replace("<br>","\n")+" ("+edIsm.ToLower()+")\nв разрезе муниципальных образований, "+Year.SelectedValue.ToLower();
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);
            DundasMap.ShapeRules.Clear();

            dtMap = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Map"), "Карта", dtMap);
            if (dtMap.Rows[0][1].ToString() == "0")
            {
                reverse = false;
            }
            else
            {
                reverse = true;
            }
            dtMap.Rows.Remove(dtMap.Rows[0]);
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                dtMap.Rows[i][0] = dtMap.Rows[i][0].ToString().Split(';')[0];
            }
            double[] values = new double[dtMap.Rows.Count];
            maxMap = Convert.ToDouble(dtMap.Rows[0].ItemArray[1]);
            minMap = Convert.ToDouble(dtMap.Rows[0].ItemArray[1]);
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                values[i] = GetNumber(dtMap.Rows[i].ItemArray[1].ToString());
                if (maxMap < Convert.ToDouble(dtMap.Rows[i].ItemArray[1]))
                {
                    maxMap = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
                }
                if (minMap > Convert.ToDouble(dtMap.Rows[i].ItemArray[1]))
                {
                    minMap = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
                }
            }
            step = (maxMap - minMap) / 3;
            LegendItem item = new LegendItem();
            item.Text = String.Format(String.Format("{0:##0.00}", minMap) + " - " + String.Format("{0:##0.00}", (minMap + step)));
            if (reverse)
            {
                item.Color = Color.Green;
            }
            else
            {
                item.Color = Color.Red;
            }
            DundasMap.Legends["CostLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = String.Format(String.Format("{0:##0.00}", (minMap + step)) + " - " + String.Format("{0:##0.00}", (minMap + 2 * step)));
            item.Color = Color.Yellow;
            DundasMap.Legends["CostLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = String.Format(String.Format("{0:##0.00}", (minMap + 2 * step)) + " - " + String.Format("{0:##0.00}", maxMap));
            if (reverse)
            {
                item.Color = Color.Red;
            }
            else
            {
                item.Color = Color.Green;
            }
            DundasMap.Legends["CostLegend"].Items.Add(item);

            // добавляем поля
            DundasMap.Shapes.Clear();

            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");

            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

            // добавляем правила расстановки символов

            // звезды для легенды


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

            string valueSeparator = IsMozilla ? ". " : "\n";
            bool hasCallout;
            
            string shapeHint = "";

            
            shapeHint ="{0}"+valueSeparator +ComboPok.SelectedValue+", {1}, "+edIsm.ToLower();
            
            for (int j = 0; j < dtMap.Rows.Count; j++)
            {

                string subject = dtMap.Rows[j].ItemArray[0].ToString().Replace("Город","г.");

                double value = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                string name = dtMap.Rows[j].ItemArray[2].ToString().Replace("р-н",String.Empty);



                ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
          //      Label1.Text = shapeList.Count.ToString();
                foreach (Shape shape in shapeList)
                {
                     
                    shape.Visible = true; 
                    shape["Name"] = subject;

                    shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                    shape.ToolTip = String.Format(shapeHint, subject, String.Format("{0:##0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])));

                    shape.TextAlignment = System.Drawing.ContentAlignment.TopCenter;

                    shape.Text = name + "\n" + String.Format("{0:##0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                    shape.Name = subject;
                    shape.TextVisibility = TextVisibility.Shown;
                    
                    if (value <= (minMap + step))
                    {
                        if (reverse)
                        {
                            shape.Color = Color.Green;
                            
                        }
                        else
                        {
                            shape.Color = Color.Red;
                        }
                    }
                    else
                    {
                        if (value > (minMap + step) && value <= (minMap + 2 * step))
                        {
                            shape.Color = Color.Yellow;
                        }
                        else
                        {
                            if (value > (minMap + 2 * step) && value <= maxMap)
                            {
                                if (reverse)
                                {
                                    shape.Color = Color.Red; 
                                }
                                else
                                {
                                    
                                    shape.Color = Color.Green;
                                }
                            }
                        }
                    }
                    SetShapeTextOffset(shape);
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

        private static string GetNormalName(string p)
        {
            return p.Replace(" район", "").Replace(" городской округ", "").Replace("поселок ", "").Replace("\"", "").Replace("городской округ ", "").Replace("г.", "").Replace("г. ", "").Replace(" го", "").Replace(" мр", "").Replace(" ", "");
        }

        
        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();

            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Infragistics.Documents.Excel.Worksheet sheet3 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;
             
            ReportExcelExporter1.TitleStartRow = 0;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            Chart1.Width = 900;
            Chart1.Height = 700;
            DundasMap.Width = 1000;
           // DundasMap.Height = 700;
            ReportExcelExporter1.Export(Chart1,Label1.Text, sheet2, 3);

            ReportExcelExporter1.Export(DundasMap, Label2.Text, sheet3, 3);
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
            
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section2 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section3 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 85;
            Chart1.Width = 900;
            Chart1.Height = 700;
            DundasMap.Width = 1100;
            DundasMap.Height = 700;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(Chart1,Label1.Text, section2);
            ReportPDFExporter1.Export(DundasMap,Label2.Text,  section3);

        }
        #endregion 

        protected void Chart1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        protected void Chart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                     Infragistics.UltraChart.Core.Primitives.Text text = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                     if (text.GetTextString()=="Городской округ \'Александровск-Сахалинский район\'")
                     {
                         text.SetTextString("Городской округ\n\'Александровск-Сахалинский район\'");
                         text.bounds.X -= 10;
                         text.bounds.Width += 15;
                     }
                }
            }
        }
    }
}