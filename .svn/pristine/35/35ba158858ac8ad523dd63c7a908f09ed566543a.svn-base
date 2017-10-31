using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Microsoft.AnalysisServices.AdomdClient;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Shared.Styles;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Components;
using System.Collections.ObjectModel;
using System.Text;
using Infragistics.WebUI.UltraWebChart;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0007_Novosib
{
    public partial class Default : CustomReportPage
    {
        string page_title = "Начисленная среднемесячная заработная плата в расчете на одного работника предприятий и организаций по видам экономической деятельности (ежемесячный)";
        string page_sub_title = "Данные ежемесячного мониторинга начисленной среднемесячной заработной платы в расчете на одного работника предприятий и организаций {1} по видам экономической деятельности за {0}";
        private string chart_title = "Распределение среднесписочной численности работников предприятий и организаций и начисленной среднемесячной заработной платы в расчете на одного работника предприятий и организаций за {0}";
        private CustomParam SelectedPeriod { get { return (UserParams.CustomParam("SelectedPeriod")); } }
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam comparePeriod { get { return (UserParams.CustomParam("comparePeriod")); } }
        private GridHeaderLayout headerLayout;
        private string style = "";
        private string SelectedMonth = "";
        private int offsetX = 0;
        private int startOffsetX = 0;
        private double average = 0; 
        private Dictionary<string, int> widths = new Dictionary<string, int>();
        private Dictionary<string, int> kindNumbers = new Dictionary<string, int>();
        private int widthAll = 0;
        private string chart_tooltips = "{0}\nЗаработная плата - {1} руб.\nСреднесписочная чис-ть - {2} чел.";
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
            Label1.Text = page_title;
            Page.Title = page_title;
            UltraWebGrid.Width = CRHelper.GetScreenWidth - 60;
            UltraWebGrid.Height = Unit.Empty;
            Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ##0.##>";
            ComboYear.Width = 350;
            ComboCompareYear.Width = 200;
            Chart.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            Chart.Height = 700;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart;
            Chart.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png";
            Chart.DeploymentScenario.FilePath = "../../TemporaryImages";
            Chart.Border.Color = Color.Transparent;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {

            base.Page_Load(sender, e);
                baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                if (!Page.IsPostBack)
                {
                    ComboYear.Title = "Отчётный период";
                    ComboYear.FillDictionaryValues(YearsLoad("years"));
                    ComboYear.SelectLastNode();
                    ComboCompareYear.Title = "Год для сравнения";
                }
                if (ComboYear.SelectedValue.Contains("-"))
                {
                    if (UserComboBox.getLastBlock(SelectedPeriod.Value).ToLower() != ComboYear.SelectedValue.Split(' ')[0].Remove(0, ComboYear.SelectedValue.Split(' ')[0].IndexOf('-') + 1).ToLower())
                    {
                        ComboCompareYear.FillDictionaryValues(CompareYearsLoad("compare_years"));
                        ComboCompareYear.SelectLastNode();
                    }
                } 
                else
                {
                    if (UserComboBox.getLastBlock(SelectedPeriod.Value).ToLower() != ComboYear.SelectedValue.Split(' ')[0].ToLower())
                    {
                        ComboCompareYear.FillDictionaryValues(CompareYearsLoad("compare_years"));
                        ComboCompareYear.SelectLastNode();
                    }
                }
                int n = 0;
                if (ComboYear.SelectedValue.Split(' ')[0]!="январь")
                {
                    SelectedMonth = ComboYear.SelectedValue.Split(' ')[0].Remove(0, ComboYear.SelectedValue.Split(' ')[0].IndexOf('-')+1);
                }
                else
                {
                    SelectedMonth = ComboYear.SelectedValue.Split(' ')[0];
                }
                
                n = CRHelper.MonthNum(SelectedMonth);
                
                SelectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + ComboYear.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(n).ToString() + "].[Квартал " + CRHelper.QuarterNumByMonthNum(n) + "].[" + SelectedMonth + "]";
                comparePeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + ComboCompareYear.SelectedValue.Split(' ')[0] + "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(n).ToString() + "].[Квартал " + CRHelper.QuarterNumByMonthNum(n) + "].[" + SelectedMonth + "]";
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.DataBind();
                Chart.DataBind();
                ChartTitle.Text = String.Format(chart_title, ComboYear.SelectedValue.ToLower());
                Label2.Text = String.Format(page_sub_title, ComboYear.SelectedValue.ToLower(),RegionSettingsHelper.Instance.RegionNameGenitive);
        
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Года", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(dt.Rows[0][2].ToString() + " год", 0);
            if (dt.Rows[0][0].ToString() != "Январь")
            {
                d.Add("январь - "+dt.Rows[0][0].ToString().ToLower() + " " + dt.Rows[0][2].ToString() + " года", 1);
            }
            else
            {
                d.Add(dt.Rows[0][0].ToString().ToLower() + " " + dt.Rows[0][2].ToString() + " года", 1);
            }
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][2].ToString() != dt.Rows[i - 1][2].ToString())
                {
                    d.Add(dt.Rows[i][2].ToString() + " год", 0);
                }
                if (dt.Rows[i][0].ToString() != "Январь")
                {
                    d.Add("январь-" + dt.Rows[i][0].ToString().ToLower() + " " + dt.Rows[i][2].ToString() + " года", 1);
                }
                else
                {
                    d.Add(dt.Rows[i][0].ToString().ToLower() + " " + dt.Rows[i][2].ToString() + " года", 1);
                }
            }
              
            return d;
        }

        Dictionary<string, int> CompareYearsLoad(string sql)
        {
            try
            {
                ComboCompareYear.ClearNodes();
            }
            catch { }
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                if (cs.Axes[1].Positions[i].Members[0].Caption != ComboYear.SelectedNode.Parent.Text.Split(' ')[0])
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
                }
                else
                {
                    return d;
                }
            }
            return d;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            UltraWebGrid.Columns.Clear();
            UltraWebGrid.Bands.Clear();
            
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Виды экономической деятельности", dt);
            if (dt.Rows.Count > 2)
            {
                dt.Rows.Remove(dt.Rows[0]);
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0].ToString().Split(';')[1] == " Значение не указано")
                    {
                        row[0] = row[0].ToString().Split(';')[0];
                    }
                    else
                    {
                        row[0] = row[0].ToString().Split(';')[1].Remove(0,1);
                    }
                }
                UltraWebGrid.DataSource = dt;
                
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }
        DataTable ChartTable;
        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Chart", dt);
            if (dt.Rows.Count < 3)
            {
                Chart.DataSource = null;
                ChartTable = null;
            }
            else
            {
                ChartTable = dt.Copy();
                average = double.Parse(dt.Rows[2][1].ToString());
                dt.Rows.Remove(dt.Rows[1]);
                dt.Rows.Remove(dt.Rows[1]);
                Chart.DataSource = dt;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            headerLayout.childCells.Clear();
            e.Layout.NoDataMessage = "Нет данных"; 
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.4);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {

                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.11);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");

            }
            GridHeaderCell headerCell = null;
            if (ComboCompareYear.GetRootNodesCount() != 0)
            {
                headerLayout.AddCell("Виды экономической деятельности");
                headerCell = headerLayout.AddCell(ComboYear.SelectedValue);
                headerCell.AddCell("Рублей");
                headerCell.AddCell("в % к соответствующему периоду "+ComboCompareYear.SelectedValue+" года");
                headerCell.AddCell("в % к среднеобластному уровню");
            }
            else
            {
                headerLayout.AddCell("Виды экономической деятельности");
                headerCell = headerLayout.AddCell(ComboYear.SelectedValue);
                headerCell.AddCell("Рублей");
                headerCell.AddCell("в % к среднеобластному уровню");
            }
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
           
                if (IsSmallResolution)
                { style = "background-repeat: no-repeat;background-position:2px"; }
                else
                { style = "background-repeat: no-repeat;background-position:2px"; }



                if (ComboCompareYear.GetRootNodesCount() != 0)
                {
                    if (e.Row.Cells[2].Value != null)
                    {
                        if (double.Parse(e.Row.Cells[2].Text) > 100 && e.Row.Cells[0].Text != "В том числе по видам деятельности:")
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[2].Style.CustomRules = style;
                        }
                        if (double.Parse(e.Row.Cells[2].Text) < 100 && e.Row.Cells[0].Text != "В том числе по видам деятельности:")
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[2].Style.CustomRules = style;
                        }
                        if (double.Parse(e.Row.Cells[2].Text) == 100 && e.Row.Cells[0].Text != "В том числе по видам деятельности:")
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballYellowBB.png";
                            e.Row.Cells[2].Style.CustomRules = style;
                        }
                    }
                    if (e.Row.Cells[3].Value!=null)
                    {
                        if (double.Parse(e.Row.Cells[3].Text) > 100 && e.Row.Index > 1)
                        {
                            e.Row.Cells[3].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[3].Title = "Выше среднеобластного уровня на " + (Math.Round(double.Parse(e.Row.Cells[3].Text) - 100, 1)).ToString() + "%";
                            e.Row.Cells[3].Style.CustomRules = style;
                        }
                        if (double.Parse(e.Row.Cells[3].Text) < 100 && e.Row.Index > 1)
                        {
                            e.Row.Cells[3].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[3].Title = "Ниже среднеобластного уровня на " + (Math.Round(100 - double.Parse(e.Row.Cells[3].Text), 1)).ToString() + "%";
                            e.Row.Cells[3].Style.CustomRules = style;
                        }
                        if (double.Parse(e.Row.Cells[3].Text) == 100 && e.Row.Index > 1)
                        {
                            e.Row.Cells[3].Style.BackgroundImage = "~/images/ballYellowBB.png";
                            e.Row.Cells[3].Title = "Равен среднеобластному уровню";
                            e.Row.Cells[3].Style.CustomRules = style;
                        }
                    }
                }
                else
                {
                    if (e.Row.Cells[2].Value!=null)
                    {
                        if (double.Parse(e.Row.Cells[2].Text) > 100 && e.Row.Index > 1)
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[2].Title = "Выше среднеобластного уровня на " + (Math.Round(double.Parse(e.Row.Cells[2].Text) - 100, 1)).ToString() + "%";
                            e.Row.Cells[2].Style.CustomRules = style;
                        }
                        if (double.Parse(e.Row.Cells[2].Text) < 100 && e.Row.Index > 1)
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[2].Title = "Ниже среднеобластного уровня на " + (Math.Round(100 - double.Parse(e.Row.Cells[2].Text), 1)).ToString() + "%";
                            e.Row.Cells[2].Style.CustomRules = style;
                        }
                        if (double.Parse(e.Row.Cells[2].Text) == 100 && e.Row.Index > 1)
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[2].Title = "Равен среднеобластному уровню";
                            e.Row.Cells[2].Style.CustomRules = style;
                        }
                    }
                }
                if (e.Row.Index != 0)
                {
                    e.Row.Cells[0].Style.CustomRules = "padding-left:15px";
                    if (e.Row.Cells[0].Text == "В том числе по видам деятельности:")
                    {
                        e.Row.Cells[0].ColSpan = e.Row.Cells.Count;
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].Text = "";
                        }
                    }
                }
            
        }

        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (ChartTable != null)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            widthAll += box.rect.Width - 2;
                        }
                    }
                }
                offsetX = 0;
                int j = 1;
                int labelCount = 1;
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            if (offsetX == 0)
                            {
                                offsetX = box.rect.X + 10;
                                startOffsetX = box.rect.X;
                            }
                            box.rect.X = offsetX;
                            double width = 0;
                            if (double.Parse(ChartTable.Rows[1].ItemArray[j].ToString()) < 1000)
                            {
                                width = double.Parse(ChartTable.Rows[1].ItemArray[j].ToString()) * 0.0091;
                            }
                            else
                            {
                                width = double.Parse(ChartTable.Rows[1].ItemArray[j].ToString()) * 0.0012;
                            }
                            widths.Add(ChartTable.Columns[j].ColumnName, (int)width);
                            box.rect.Width = (int)width;
                            box.DataPoint.Label = String.Format(chart_tooltips, ChartTable.Columns[j].ColumnName, String.Format("{0:# ##0.00}", double.Parse(ChartTable.Rows[0][j].ToString())), String.Format("{0:# ##0.00}", double.Parse(ChartTable.Rows[1][j].ToString())));
                            j += 1;
                            offsetX += box.rect.Width + 2;

                        }
                    }

                }
                offsetX = 0;
                int counter = 1;
                int counter2 = 1;

                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                    {

                        Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                        if (widths.ContainsKey(te.GetTextString()))
                        {

                            if (offsetX == 0)
                            {
                                offsetX = startOffsetX;
                            }
                            LabelStyle labStyle = new LabelStyle();
                            labStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                            te.SetLabelStyle(labStyle);
                            te.bounds.X = offsetX - 8 + widths[te.GetTextString()] / 2 + 14;
                            te.bounds.Y = 360;
                            offsetX += widths[te.GetTextString()] + 2;
                            te.bounds.Width = 20;
                            te.SetTextString(counter.ToString());
                            counter += 1;
                        }
                        else
                        {
                            if (te.GetTextString() == "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                            {
                                string s = ChartTable.Columns[counter2].ColumnName + " (" + String.Format("{0:# ##0.00}", double.Parse(ChartTable.Rows[0][counter2].ToString())) + " руб.)";
                                s = counter2.ToString() + ". " + s;
                                te.SetTextString(s);
                                counter2 += 1;

                            }
                            else
                            {
                                if (te.Path.Contains("Y"))
                                {
                                    te.SetTextString(String.Format("{0:### ##0.##}",double.Parse(te.GetTextString())));
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
                double urfoAverage;
                if (double.TryParse(average.ToString(), out urfoAverage))
                {
                    int fmY = (int)yAxis.Map(average);
                    Line line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Dot;
                    line.PE.Stroke = Color.Gray;
                    line.PE.StrokeWidth = 2;
                    line.p1 = new Point(xMin, fmY);
                    line.p2 = new Point(xMax, fmY);
                    e.SceneGraph.Add(line);

                    Text text = new Text();
                    text.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                    text.PE.Fill = Color.Black;
                    text.bounds = new Rectangle(xMin + 5, fmY - 15, 780, 15);
                    text.SetTextString("Средняя з/п по "+RegionSettingsHelper.Instance.RegionNameGenitive+": " + String.Format("{0:# ##0.00}", average) + " руб.");//String.Format(dtChart12.Rows[0].ItemArray[0].ToString(),""));
                    e.SceneGraph.Add(text);
                }
            }
        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();


            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;

            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            ReportExcelExporter1.Export(Chart, ChartTitle.Text, sheet2, 3);
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
            e.Workbook.Worksheets["Диаграмма"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
        }
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            
            ReportPDFExporter1.HeaderCellHeight = 60;

            UltraWebGrid.Width = 1024;
            ReportPDFExporter1.Export(headerLayout, section1);

            section2.PageSize = new Infragistics.Documents.Reports.Report.PageSize(950, 600);
            Infragistics.Documents.Reports.Report.Text.IText text = section2.AddText(); ;
            text.AddContent(ChartTitle.Text);
            text.Style.Font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16) ;
            text.Style.Font.Bold = true;
            Infragistics.Documents.Reports.Graphics.Image img =  UltraGridExporter.GetImageFromChart(Chart);
            section2.AddImage(img);
        //    ReportPDFExporter1.Export(Chart, ChartTitle.Text,section2);

            
        }
        #endregion

        protected void Chart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }
    }
}