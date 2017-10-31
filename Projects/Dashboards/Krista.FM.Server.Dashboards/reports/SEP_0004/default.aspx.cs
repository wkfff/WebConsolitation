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

namespace Krista.FM.Server.Dashboards.reports.SEP_0004
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Оценка социально-экономического развития территории в разрезе муниципальных образований";
        private string page_sub_title = "Ежегодная оценка социально-экономического развития территории по состоянию на {0}";
        private string chart_title = "Распределение муниципальных образований по стабильности социально-экономического положения в {0} году по отношению к предыдущему периоду";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam prevPeriod { get { return (UserParams.CustomParam("prevPeriod")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
        private GridHeaderLayout headerLayout;
        private bool emptyReport = false;
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
            Year.Width = 200;
            ComboPok.Width = 400;
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            Chart.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            Chart.Height = 750;
            G.Height = Unit.Empty;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "Оценка социально-экономического развития территории (по муниципальным образованиям)";
            CrossLink1.NavigateUrl = "~/reports/SEP_0003/default.aspx";
        } 
        

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension + ".[Новосибирская область]";
            
            if (!Page.IsPostBack)
            {
                Year.FillDictionaryValues(YearsLoad("years"));
                Year.Title = "Период";
                Year.ParentSelect = true;
                Year.SetСheckedState(Year.GetRootNodesName(0), true);
                Year.SelectedNode.Hidden = true;
                Year.ParentSelect = false;
                Year.SelectLastNode();
                ComboPok.Title = "Показатель";
                ComboPok.FillDictionaryValues(PokLoad("Pok_list"));
                ComboPok.ParentSelect = true;
                ComboPok.SetСheckedState(ComboPok.GetRootNodesName(0), true);
            }
            
            int n = CRHelper.MonthNum(Year.SelectedValue.Split(' ')[0]);
            selectedPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "]";
            if (ComboPok.SelectedNode.Level == 2)
            {
                selectedPok.Value = "[СЭР__Показатели МО].[СЭР__Показатели МО].[Все показатели].[" + ComboPok.SelectedValue.Remove(ComboPok.SelectedValue.LastIndexOf('(')-1) + "]";
            }
            else
            {
                selectedPok.Value = "[СЭР__Показатели МО].[СЭР__Показатели МО].[Все показатели].[" + ComboPok.SelectedValue + "]";
            }
            PageSubTitle.Text = String.Format(page_sub_title, Year.SelectedValue.ToLower());
            PageTitle.Text = page_title;
            Page.Title = page_title;
            ChartTitle.Text = String.Format(chart_title,Year.SelectedValue.Split(' ')[0]);
            if (Year.SelectedNode.Index!=0)
            {
                prevPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + Year.SelectedNode.PrevNode.Text.Split(' ')[0] + "]";
                n = CRHelper.MonthNum(Year.SelectedNode.PrevNode.Text.Split(' ')[0]);
            }
            else
            {
                //prevPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + Year.SelectedNode.Parent.PrevNode.Text.Split(' ')[0] + "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(n).ToString() + "].[Квартал " + CRHelper.QuarterNumByMonthNum(n).ToString() + "].[" + Year.SelectedNode.Parent.PrevNode.Nodes[Year.SelectedNode.Parent.PrevNode.Nodes.Count - 1].Text.Split(' ')[0] + "]";
                n = CRHelper.MonthNum(Year.SelectedNode.Parent.PrevNode.Nodes[Year.SelectedNode.Parent.PrevNode.Nodes.Count - 1].Text.Split(' ')[0]);
            }
            
            headerLayout = new GridHeaderLayout(G);
            Chart.DataBind();
            
            
            if (emptyReport)
            {
                LabelEmpty.Visible = true;
                Table.Visible = false;
                Table1.Visible = false;
                headerLayout.childCells.Clear();
                G.DataSource = null;
                G.DisplayLayout.NoDataMessage = "Нет данных";
            }
            else
            {
                LabelEmpty.Visible = false;
                Table.Visible = true;
                Table1.Visible = true;
            }
            G.DataBind();
            if (!emptyReport)
            {
                calculateRank(G, 1, 3);
                calculateRank(G, 4, 6);
                FormDynText();
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

        protected void FormDynText()
        {
            string pokName = "";
            if (ComboPok.SelectedNode.Level == 2)
            {
                pokName = "показателя " + ComboPok.SelectedValue.ToLower();
            }
            else
            {
                pokName = ComboPok.SelectedValue.Replace("Интегральный показатель", "интегрального показателя");
            }
            string s = "В <b>{0}</b> по сравнению с предыдущим отчетным периодом наблюдается резкое изменение значения <b>{1}</b> в следующих муниципальных образованиях:<br>";
            string s1 = "&nbsp;<img src='../../images/ballRedBB.png'><b> ухудшение: </b>";
            string s2 = "&nbsp;<img src='../../images/ballGreenBB.png'><b> улучшение: </b>";
            string s3 = "<b>{0}</b> (снизился с  <b>{1}</b> до <b>{2}</b> {3}), ";
            string s4 = "<b>{0}</b> (увеличился с  <b>{1}</b> до <b>{2}</b> {3}), ";
            string bad="";
            string good=""; 
            DynamicText.Text = String.Format(s, Year.SelectedValue.Split(' ')[0] + " году",pokName);
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][1].ToString().Contains("-") && !dtChart.Rows[i][2].ToString().Contains("-") && GetNumber(dtChart.Rows[i][2].ToString())!=0)
                {
                    bad += String.Format(s3, dtChart.Rows[i][0].ToString().Split('<')[0], Math.Round((GetNumber(dtChart.Rows[i][2].ToString()) + 1),2), Math.Round((GetNumber(dtChart.Rows[i][1].ToString()) + 1), 2), "&nbsp;<img src='../../images/arrowRedDownBB.png'>");
                }
                if (!dtChart.Rows[i][1].ToString().Contains("-") && dtChart.Rows[i][2].ToString().Contains("-") && GetNumber(dtChart.Rows[i][1].ToString())!=0)
                {
                    good += String.Format(s4, dtChart.Rows[i][0].ToString().Split('<')[0], Math.Round( (GetNumber(dtChart.Rows[i][2].ToString()) + 1),2), Math.Round((GetNumber(dtChart.Rows[i][1].ToString()) + 1), 2), "&nbsp;<img src='../../images/arrowGreenUpBB.png'>");
                }
            }
            if (bad == "" && good == "")
            {
                Table1.Visible = false;
            }
            else
            {
                Table1.Visible = true;
                if (bad != "")
                {
                    DynamicText.Text += s1 + bad.Remove(bad.LastIndexOf(','), 2) + ";<br>";
                }
                if (good != "")
                {
                    DynamicText.Text += s2 + good.Remove(good.LastIndexOf(','), 2) + ".";
                }
                if (DynamicText.Text == s)
                {
                    DynamicText.Text = "Нет данных";
                }
            }
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 1; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption + " год", 0);
            }
            return d;
        }

        Dictionary<string, int> PokLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Pok_list"), "Показатель", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Интегральный показатель", 0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][1].ToString() == "100")
                {
                    d.Add("Интегральный показатель социальной сферы", 1);
                }
                else
                {
                    if (dt.Rows[i][1].ToString() == "200")
                    {
                        d.Add("Интегральный показатель экономической сферы", 1);
                    }
                    else
                    {
                        if (dt.Rows[i][1].ToString() == "300")
                        {
                            d.Add("Интегральный показатель финансовой сферы", 1);
                        }
                        else
                        {
                            d.Add(dt.Rows[i][0].ToString()+" (индикатор)", 2);
                        }
                    }
                }
            }
            return d;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Columns.Clear();
            G.Bands.Clear();
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Территория", dt);
            if (emptyReport)
            {
                G.DataSource = null;
                G.DisplayLayout.NoDataMessage = "Нет данных";
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][1] == DBNull.Value && dt.Rows[i][4] == DBNull.Value)
                    {
                        dt.Rows.Remove(dt.Rows[i]);
                        i -= 1;
                    }
                }
                G.DataSource = dt;
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
            }

            if (m!=0)
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 0; i < Grid.Rows.Count; i ++)
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
                    
                    for (int j =0; j < Grid.Rows.Count; j ++)
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
                double max = GetNumber(Grid.Rows[0].Cells[colNumber].Text);
                for (int j = 0; j < Grid.Rows.Count; j ++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[j].Cells[colNumber].Value) == true && Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) < max)
                        {
                            max = GetNumber(Grid.Rows[j].Cells[colNumber].Text);
                        }
                    }
                }
                for (int j = 0; j < Grid.Rows.Count; j ++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[j].Cells[colNumber].Value) == true && Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) == max)
                        {
                            Grid.Rows[j].Cells[colRankNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                            Grid.Rows[j].Cells[colRankNumber].Title = "Самое низкое значение";
                            Grid.Rows[j].Cells[colRankNumber].Style.CustomRules = style;
                        }
                    }
                }
            }
           
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double colWidth = 0;
            if (IsSmallResolution)
            {
                colWidth = 0.13;
            }
            else
            {
                colWidth = 0.1;
            }
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.25);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;


            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "##0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "##0");
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.08);
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.08);
            headerLayout.AddCell("Территория");
            GridHeaderCell headerCell = null;
            if (Year.SelectedNode.Index != 0)
            {
                headerCell = headerLayout.AddCell(Year.SelectedNode.PrevNode.Text);
            }
            else
            {
                headerCell = headerLayout.AddCell(Year.SelectedNode.Parent.PrevNode.Nodes[Year.SelectedNode.Parent.PrevNode.Nodes.Count - 1].Text);
            }

            headerCell.AddCell(ComboPok.SelectedValue);
            headerCell.AddCell("Изменение по сравнению с предыдущим годом");
            headerCell.AddCell("Ранг");
            
            
            headerCell = headerLayout.AddCell(Year.SelectedValue);
            headerCell.AddCell(ComboPok.SelectedValue);
            headerCell.AddCell("Изменение по сравнению с предыдущим годом");
            headerCell.AddCell("Ранг");
            headerLayout.ApplyHeaderInfo();
        
        }

        DataTable dtChart;
        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Территория", dt);
            if (dt.Columns.Count < 3)
            {
                emptyReport = true;
                Chart.DataSource = null;
            }
            else
            {
                emptyReport = false;
                double minY = Convert.ToDouble(dt.Rows[0][2]), maxY = Convert.ToDouble(dt.Rows[0][2]), minX = Convert.ToDouble(dt.Rows[0][1]), maxX = Convert.ToDouble(dt.Rows[0][1]);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (GetNumber(dt.Rows[i][1].ToString()) == 0 || GetNumber(dt.Rows[i][2].ToString()) == 0)
                    {
                        dt.Rows.Remove(dt.Rows[i]);
                        i--;
                    }
                    
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (GetNumber(dt.Rows[i][2].ToString()) < minY)
                    {
                        minY = GetNumber(dt.Rows[i][2].ToString());
                    }
                    if (GetNumber(dt.Rows[i][2].ToString()) > maxY)
                    {
                        maxY = GetNumber(dt.Rows[i][2].ToString());
                    }
                    if (GetNumber(dt.Rows[i][1].ToString()) > maxX)
                    {
                        maxX = GetNumber(dt.Rows[i][1].ToString());
                    }
                    if (GetNumber(dt.Rows[i][1].ToString()) < minX)
                    {
                        minX = GetNumber(dt.Rows[i][1].ToString());
                    }
                }
                Chart.DataSource = dt;
                Chart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                Chart.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                Chart.Axis.X.RangeMax = maxX + 0.1;
                Chart.Axis.X.RangeMin = minX - 0.1;
                Chart.Axis.Y.RangeMax = maxY + 0.1;
                Chart.Axis.Y.RangeMin = minY - 0.1;
                dtChart = dt;
            }
        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();
            if (!emptyReport)
            {
                Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
                Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
                ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
                ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
                ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
                ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;

                ReportExcelExporter1.TitleStartRow = 3;
                ReportExcelExporter1.SheetColumnCount = 15;
                ReportExcelExporter1.Export(headerLayout, sheet1, 6);

                ReportExcelExporter1.WorksheetTitle = String.Empty;
                ReportExcelExporter1.WorksheetSubTitle = String.Empty;
                ReportExcelExporter1.Export(Chart, ChartTitle.Text, sheet2, 3);
            }
            else
            {
                Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
                ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
                ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
                ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
                ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
                ReportExcelExporter1.TitleStartRow = 3;
                ReportExcelExporter1.SheetColumnCount = 15;
                ReportExcelExporter1.Export(headerLayout, sheet1, 6);
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
            if (!emptyReport)
            {
                e.Workbook.Worksheets["Диаграмма"].MergedCellsRegions.Clear();
                e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            if (!emptyReport)
            {
                ReportPDFExporter1.PageSubTitle = PageSubTitle.Text + "\n" + DynamicText.Text.Replace("<br>", "\n") + "\n";
                ReportPDFExporter1.HeaderCellHeight = 30;
                Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

                Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
                Infragistics.Documents.Reports.Report.Section.ISection section2 = report.AddSection();
                ReportPDFExporter1.HeaderCellHeight = 60;

                ReportPDFExporter1.Export(headerLayout, section1);
                section2.PageSize = new Infragistics.Documents.Reports.Report.PageSize(1000, 900);
                Image img = UltraGridExporter.GetImageFromChart(Chart);

                Infragistics.Documents.Reports.Report.Text.IText title = section2.AddText();
                Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
                title.Style.Font = font;
                title.AddContent(ChartTitle.Text);
                section2.AddImage(img);
            }
            else
            {
                ReportPDFExporter1.PageSubTitle = PageSubTitle.Text + "\nНет данных";
                ReportPDFExporter1.HeaderCellHeight = 30;
                Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();
                Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
                ReportPDFExporter1.HeaderCellHeight = 60;
                ReportPDFExporter1.Export(headerLayout, section1);
            }
            
        }
        #endregion

        protected void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            string pokName = "";
            if (ComboPok.SelectedNode.Level == 2)
            {
                pokName = "показателя " + ComboPok.SelectedValue.ToLower();
                
            }
            else
            {
                pokName = ComboPok.SelectedValue.Replace("Интегральный показатель", "интегрального показателя");
                
            }
            if (pokName.Length > 30)
            {
                pokName = pokName.Insert(pokName.IndexOf(' ', 30) + 1, "<br>");
            }
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.PointSet)
                {
                    Infragistics.UltraChart.Core.Primitives.PointSet pointSet = (Infragistics.UltraChart.Core.Primitives.PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        double temp = 0;
                        if (GetNumber(dtChart.Rows[point.Row][2].ToString()) != 0)
                        {
                            temp = (((GetNumber(dtChart.Rows[point.Row][1].ToString())+1) / (GetNumber(dtChart.Rows[point.Row][2].ToString())+1))-1)*100;
                        }
                        point.DataPoint.Label = dtChart.Rows[point.Row][0].ToString() + " изменение <br>"+pokName+"<br>в " + Year.SelectedValue.Split(' ')[0] + " году составляет " + String.Format("{0:0.##}",(GetNumber(dtChart.Rows[point.Row][1].ToString()))) + "<br>в " + UserComboBox.getLastBlock(prevPeriod.Value)+ " году составляет " + String.Format("{0:0.##}",(GetNumber(dtChart.Rows[point.Row][2].ToString())));
                    }
                }
            }

            Text text1 = new Text();
            text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9,FontStyle.Bold);
            text1.PE.Fill = Color.Black;
            text1.bounds = new Rectangle(20, 10,100, 30);
            text1.SetTextString("Ухудшение");
            e.SceneGraph.Add(text1);

            text1 = new Text();
            text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            text1.PE.Fill = Color.Black;
            text1.bounds = new Rectangle(1020, 10, 200, 30);
            text1.SetTextString("Стабильный рост");
            e.SceneGraph.Add(text1);

            text1 = new Text();
            text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            text1.PE.Fill = Color.Black;
            text1.bounds = new Rectangle(20, 545, 200, 30);
            text1.SetTextString("Стабильное ухудшение");
            e.SceneGraph.Add(text1);

            text1 = new Text();
            text1.labelStyle.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            text1.PE.Fill = Color.Black;
            text1.bounds = new Rectangle(1020, 545, 100, 30);
            text1.SetTextString("Улучшение");
            e.SceneGraph.Add(text1);
        }
    }
}