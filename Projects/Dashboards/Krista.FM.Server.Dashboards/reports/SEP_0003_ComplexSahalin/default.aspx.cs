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

namespace Krista.FM.Server.Dashboards.reports.SEP_0003_ComplexSahalin
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Частные индикаторы уровня и динамики развития муниципальных образований по сферам";
        private string page_sub_title = "Расчет {0}, {1}, {2}";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
        private CustomParam weightKoef { get { return (UserParams.CustomParam("weightKoef")); } }
        private CustomParam selectedIndicator { get { return (UserParams.CustomParam("selectedIndicator")); } }
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
            Year.Width = 250;
            ComboPok.Width = 500;
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            G.Height = Unit.Empty;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override  void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack) 
            {
                Year.FillDictionaryValues(YearsLoad("years"));
                Year.Title = "Период";
                Year.ParentSelect = true;
                Year.SelectLastNode();
                ComboPok.Title = "Показатель";
                ComboPok.ParentSelect = true;
                ComboPok.FillDictionaryValues(PokLoad("pok"));
            }
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            PageTitle.Text = page_title;
            Page.Title = page_title;
            if (ComboPok.SelectedNode.Level != 0)
            {
                if (ComboPok.SelectedNode.Parent.Text == "Частный индикатор уровня развития")
                {
                    if (Year.SelectedNode.Level == 0)
                    {
                        PageSubTitle.Text = String.Format(page_sub_title, "частных индикаторов уровня развития", ComboPok.SelectedNode.Text.Split(',')[0].ToLower(), Year.SelectedValue.ToLower());
                    }
                    else
                    {
                        PageSubTitle.Text = String.Format(page_sub_title, "частных индикаторов уровня развития", ComboPok.SelectedNode.Text.Split(',')[0].ToLower(), Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года");
                    }
                    selectedIndicator.Value = "Индикатор уровня";
                }
                else
                {
                    if (Year.SelectedNode.Level == 0)
                    {
                        PageSubTitle.Text = String.Format(page_sub_title, "частных индикаторов динамики развития", ComboPok.SelectedNode.Text.Split(',')[0].ToLower(), Year.SelectedValue.ToLower());
                    }
                    else
                    {
                        PageSubTitle.Text = String.Format(page_sub_title, "частных индикаторов динамики развития", ComboPok.SelectedNode.Text.Split(',')[0].ToLower(), Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года");
                    }
                    selectedIndicator.Value = "Индикатор динамики";
                }
                selectedPok.Value = ComboPok.SelectedNode.Text.Split(',')[0];
            }
            else
            {
                if (ComboPok.SelectedValue == "Частный индикатор оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления")
                {
                    selectedPok.Value = "Оценка населением ситуации в ключевых сферах деятельности органов местного самоуправления";
                    selectedIndicator.Value = "Индикатор уровня";
                }
                else 
                {
                    selectedPok.Value = "Социальная сфера";
                    if (ComboPok.SelectedNode.Text == "Частный индикатор уровня развития")
                    {
                        selectedIndicator.Value = "Индикатор уровня";
                    }
                    else
                    {
                        selectedIndicator.Value = "Индикатор динамики";
                    }
                }
            }
            if (Year.SelectedNode.Level == 0)
            {
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "].[Полугодие 2].[Квартал 4]";
                weightKoef.Value = "Годовой";
            }
            else
            {
                int half_num = CRHelper.HalfYearNumByQuarterNum(int.Parse(Year.SelectedValue.Split(' ')[1]));
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие " + half_num.ToString() + "].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
                weightKoef.Value = "Квартальный";
            }
            
            headerLayout = new GridHeaderLayout(G);
            G.DataBind();
            if (G.DataSource == null)
            {
                EmptyReport.Visible = true;
                TableGrid.Visible = false;
            }
            else
            {
                EmptyReport.Visible = false;
                TableGrid.Visible = true;
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

            d.Add("Частный индикатор уровня развития", 0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString() + ", " + "частный индикатор уровня развития",1);
            }

            d.Add("Частный индикатор динамики развития", 0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString() + ", " + "частный индикатор динамики развития", 1);
            }
            d.Add("Частный индикатор оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления", 0);
            return d;
        }
        DataTable dtGrid;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Columns.Clear();
            G.Bands.Clear();
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Территория", dtGrid);
            if (dtGrid.Rows.Count > 1)
            {
                for (int i = 1; i < dtGrid.Columns.Count; i++)
                {
                    dtGrid.Columns[i].ColumnName = dtGrid.Columns[i].ColumnName + ";" + "Весовой коэффициент " + dtGrid.Rows[0][i].ToString();
                }
                dtGrid.Rows.Remove(dtGrid.Rows[0]);
                G.DataSource = dtGrid;
            }
            else 
            {
                G.DataSource = null;
            }
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[0];
            e.Row.Cells[0].Style.BackColor = Color.White;
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
            }
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double width = 0;
            if (IsSmallResolution)
            {
                width = 0.27;
            }
            else
            {
                width = 0.1;
            }
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
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
            }

            if (dtGrid.Rows.Count > 1) 
            {
                headerLayout.AddCell("Территория");
                GridHeaderCell headerCell = null;
                GridHeaderCell headerCell1 = null;
                if (ComboPok.SelectedNode.Level != 0)
                {
                    headerCell = headerLayout.AddCell(ComboPok.SelectedNode.Parent.Text + ", " + ComboPok.SelectedNode.Text.Split(',')[0]);
                }
                else
                {
                    if (ComboPok.SelectedValue != "Частный индикатор оценки населением ситуации в ключевых сферах деятельности органов местного самоуправления")
                    {
                        headerCell = headerLayout.AddCell(ComboPok.SelectedNode.Text + ", Социальная сфера");
                    }
                    else
                    {
                        headerCell = headerLayout.AddCell(ComboPok.SelectedNode.Text);
                    }
                }
                
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    headerCell1 = headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[0]);
                    headerCell1.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[1]);
                }
            }
            headerLayout.ApplyHeaderInfo();
        } 

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();

            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.TitleStartRow = 0;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
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
            ReportPDFExporter1.HeaderCellHeight = 80;

            ReportPDFExporter1.Export(headerLayout, section1);
        }
        #endregion
    }
}