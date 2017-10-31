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
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraChart.Core.Layers;
namespace Krista.FM.Server.Dashboards.EO_0004new.EO_0004_LeninRegionNew1.Report.reports.HMAO_ARC._0005
{
    public partial class _default : CustomReportPage
    {
        string page_title = "Мониторинг выбросов загрязняющих веществ в атмосферный воздух (по состоянию на выбранную дату)";
        string page_sub_title = "Данные ежегодного мониторинга выбросов загрязняющих веществ в атмосферный воздух в {0} году";
        private string style = "";
        private string edIsm = "";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedYear { get { return (UserParams.CustomParam("selectedYear")); } }
        private GridHeaderLayout headerLayout;
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

            //Grid.Height = Unit.Empty;
            Grid.Height = 400;
            ComboYear.Width = 200;
            Grid.Width = CRHelper.GetScreenWidth - 45;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Выберите год";
                ComboYear.FillDictionaryValues(YearsLoad("years"));
                ComboYear.SelectLastNode();
            }
            selectedYear.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[" + ComboYear.SelectedValue + "]";
            Label2.Text = String.Format(page_sub_title, ComboYear.SelectedValue);
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed; 
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "МР ГО", dt);
            if (dt.Rows[0].ItemArray[1].ToString() == "Тысяча тонн")
            {
                edIsm = "тыс. тонн";
            }
            else
            {
                edIsm = dt.Rows[0].ItemArray[1].ToString().ToLower();
            }

            dt.Rows.Remove(dt.Rows[0]);
            if (dt.Rows.Count < 1)
            {
                Grid.DataSource = null;
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i += 4)
                {
                    if (dt.Columns[1].ColumnName == "Всего выброшено в атмосферу загрязняющих веществ" && dt.Columns[4].ColumnName == "Разрешенный выброс по предприятиям" && Convert.ToDouble(dt.Rows[i][3].ToString()) != 0)
                    {
                        dt.Rows[i + 3][1] = Convert.ToDouble(dt.Rows[i][1].ToString()) / Convert.ToDouble(dt.Rows[i][3].ToString());
                    }
                    if (dt.Columns[dt.Columns.Count - 1].ColumnName == "Утилизировано загрязняющих веществ" && dt.Columns[dt.Columns.Count - 2].ColumnName == "Уловлено и обезврежено загрязняющих веществ" && Convert.ToDouble(dt.Rows[i][dt.Columns.Count - 2].ToString()) != 0)
                    {
                        dt.Rows[i + 3][dt.Columns.Count - 1] = Convert.ToDouble(dt.Rows[i][dt.Columns.Count - 1].ToString()) / Convert.ToDouble(dt.Rows[i][dt.Columns.Count - 2].ToString());
                    }
                    if (dt.Columns[dt.Columns.Count - 2].ColumnName == "Уловлено и обезврежено загрязняющих веществ" && dt.Columns[5].ColumnName == "Количество загрязняющих веществ, отходящих от всех стационарных источников выделения" && Convert.ToDouble(dt.Rows[i][5].ToString()) != 0)
                    {
                        dt.Rows[i + 3][dt.Columns.Count - 2] = Convert.ToDouble(dt.Rows[i][dt.Columns.Count - 2].ToString()) / Convert.ToDouble(dt.Rows[i][5].ToString());
                    }
                    if (dt.Columns[2].ColumnName == "Всего выброшено в атмосферу загрязняющих веществ в прошлом году")
                    {
                        dt.Rows[i + 1][2] = DBNull.Value;
                        dt.Rows[i + 2][2] = DBNull.Value;
                        dt.Rows[i + 3][2] = DBNull.Value;
                    }

                }
                Grid.DataSource = dt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double colWidth = 0,col1Width=0;
            if (IsSmallResolution)
            {
                colWidth = 0.12;
                col1Width = 0.15;
            }
            else
            {
                colWidth = 0.08;
                col1Width = 0.1;
            }
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * col1Width);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                //e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key+", "+edIsm;
                
            }
            GridHeaderCell headerCell = null;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (e.Layout.Bands[0].Columns[i].Header.Caption == "Всего выброшено в атмосферу загрязняющих веществ")
                {
                    headerCell = headerLayout.AddCell("Всего выброшено в атмосферу загрязняющих веществ, " + edIsm);
                    headerCell.AddCell("За отчетный год");
                    headerCell.AddCell("За предыдущий год");
                    i++;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key.Replace("Уменьшение, увеличение","Превышение") + ", " + edIsm;
                    headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                }
            }
            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position:15px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 40px"; }

            if (e.Row.Cells[0].Text.EndsWith("Темп прироста"))
            {

                e.Row.Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = Grid.Rows[e.Row.Index - 1].Cells[0].Text.Split(';')[0].Replace("муниципальный район","МР").Replace("Город","г.");
                Grid.Rows[e.Row.Index - 2].Cells[0].Text ="";

                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                e.Row.Cells[0].Style.BackColor = Color.White;

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                    e.Row.Cells[i].Style.BackColor = Color.White;
                    if (ComboYear.SelectedIndex != 0)
                    {
                        if (Math.Round(Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value), 3) < 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Снижение относительно " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }
                        if (Math.Round(Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value), 3) > 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Рост относительно " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }


                        if (Grid.Rows[e.Row.Index - 1].Cells[i].Value != null)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Value = String.Format("{0:N3}", Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value));
                        }
                        if (Grid.Rows[e.Row.Index - 2].Cells[i].Value != null)
                        {
                            Grid.Rows[e.Row.Index - 2].Cells[i].Value = String.Format("{0:N3}", Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[i].Value));
                        }
                        if ((e.Row.Cells[i].Value != null))
                        {
                            e.Row.Cells[i].Title = "Темп прироста к " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " году";
                            e.Row.Cells[i].Value = String.Format("{0:P3}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }
                }
                if (Grid.Columns[4].Header.Caption == "Уменьшение, увеличение выбросов загрязняющих веществ в отчетном году по сравнению с разрешенным выбросом, "+edIsm)
                {
                    if (Math.Round(Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[4].Value), 3) < 0)
                    {
                        Grid.Rows[e.Row.Index - 2].Cells[4].Title = "Снижение по сравнению с разрешенным выбросом";
                        Grid.Rows[e.Row.Index - 2].Cells[4].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        Grid.Rows[e.Row.Index - 2].Cells[4].Style.CustomRules = style;
                    }
                    if (Math.Round(Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[4].Value), 3) > 0)
                    {
                        Grid.Rows[e.Row.Index - 2].Cells[4].Title = "Рост по сравнению с разрешенным выбросом";
                        Grid.Rows[e.Row.Index - 2].Cells[4].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        Grid.Rows[e.Row.Index - 2].Cells[4].Style.CustomRules = style;
                    }
                }
            }
            else
            {
                if (e.Row.Cells[0].Text.EndsWith("Доля"))
                {
                    e.Row.Cells[0].Text = "";
                    Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    e.Row.Cells[0].Style.BackColor = Color.White;
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                        e.Row.Cells[i].Style.BackColor = Color.White;
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                        if ((e.Row.Cells[i].Value != null))
                        {
                            e.Row.Cells[i].Value = String.Format("{0:P3}", Convert.ToDouble(e.Row.Cells[i].Value));
                            if (Grid.Columns[i].Header.Caption == "За отчетный год")
                            {
                                e.Row.Cells[i].Title = "Доля от разрешенного выброса";
                            }
                            if (Grid.Columns[i].Header.Caption == "Утилизировано загрязняющих веществ, " + edIsm)
                            {
                                e.Row.Cells[i].Title = "Доля от уловленных и обезвреженных веществ";
                            }
                            if (Grid.Columns[i].Header.Caption == "Уловлено и обезврежено загрязняющих веществ, " + edIsm)
                            {
                                e.Row.Cells[i].Title = "Доля от количества загрязняющих веществ";
                            }
                        }
                    }
                }
            }
            if (e.Row.Cells[0].Text.Contains(";"))
            {
                //e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[0];
                //  e.Row.Cells[0].Style.Font.Bold = true;
                //  e.Row.Cells[0].Style.Font.Size = 8;
                //e.Row.Cells[0].Style.Font.Name = "Verdana";
            }
        }
        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
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
            ReportPDFExporter1.HeaderCellHeight = 80;
            ReportPDFExporter1.Export(headerLayout, section1);
        }
        #endregion
    }
}