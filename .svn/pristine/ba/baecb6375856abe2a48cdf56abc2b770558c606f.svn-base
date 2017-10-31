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

namespace Krista.FM.Server.Dashboards.reports.HMAO_ARC._0001
{
    public partial class _default : CustomReportPage
    {
        string page_title = "Заболеваемость населения";
        string page_sub_title = "Ежегодный мониторинг заболеваемости населения, {0}, {1} год";
        string chart1_title = "Структура заболеваемости населения (впервые выявленный диагноз, {1}) в {0} году";
        string chart2_title = "Динамика заболеваемости населения (впервые выявленный диагноз, {0}) по классу:";
        string chart2_title1 = "«{0}»";
        string grid_title = "Число случаев заболеваний (впервые установленный диагноз, {1}) в {0} году";
        string edIsm = "";
        private CustomParam SelectedYear;
        private CustomParam currentWay;
        private GridHeaderLayout headerLayout;
        int[] offset; 
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
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            base.Page_PreLoad(sender, e);
            SelectedYear = UserParams.CustomParam("year");
            currentWay = UserParams.CustomParam("currentWay");
            ComboYear.Width = 200;
            RefreshPanel.AddLinkedRequestTrigger(Grid);
            RefreshPanel.AddRefreshTarget(Chart2);
            RefreshPanel.AddRefreshTarget(Label6);
            Grid.Height = Unit.Empty;
            if (BN != "APPLEMAC-SAFARI")
            {
                if (BN == "FIREFOX")
                {
                    Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
                    Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
                    Chart2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
                }
                else
                {
                    Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);

                    Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
                    Chart2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
                }
            }
            else 
            {
                Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
                Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
                Chart2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            }

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Выберите год";
                ComboYear.FillDictionaryValues(YearsLoad("years"));
                ComboYear.SelectLastNode();
            }
            SelectedYear.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].["+ComboYear.SelectedValue+"]";
            Chart1.DataBind();
            headerLayout = new GridHeaderLayout(Grid);
            
            Grid.DataBind();
            Grid.Rows[0].Selected = 1 == 1;
            Grid.Rows[0].Activated = 1 == 1;
            currentWay.Value = Grid.Rows[0].Cells[0].Text;
            Grid.Rows[0].Activate();
            Chart2.DataBind();
            Label1.Text = page_title;
            Page.Title = Label1.Text;
            Label2.Text =String.Format(page_sub_title,RegionSettingsHelper.Instance.Name,ComboYear.SelectedValue);
            Label3.Text = String.Format(chart1_title, ComboYear.SelectedValue, edIsm);
            Label4.Text =String.Format(chart2_title,edIsm);
            Label6.Text = String.Format(chart2_title1,currentWay.Value);
            Label5.Text = String.Format(grid_title,ComboYear.SelectedValue,edIsm);
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "chart1", dt);

            if (dt.Rows.Count < 1)
            {
                edIsm = "заболеваний на 1000 человек населения";
                Chart1.DataSource = null;
            }
            else
            {
                edIsm = dt.Rows[0][2].ToString().ToLower();

                dt.Columns.Remove(dt.Columns[2]);
                Chart1.DataSource = dt;
                Chart1.Tooltips.FormatString = "<ITEM_LABEL> <b><DATA_VALUE:0></b> " + edIsm;
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
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
           /*
                DataTable dt = new DataTable();
                DataTable resDt = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "chart1", dt);
                resDt.Columns.Add("Класс заболеваний", dt.Columns[0].DataType);
                resDt.Columns.Add("Всего", dt.Columns[1].DataType);
                resDt.Columns.Add("Прирост", dt.Columns[1].DataType);
                resDt.Columns.Add("Темп прироста", dt.Columns[1].DataType);
                resDt.Columns.Add("Доля в общем объеме заболеваний", dt.Columns[1].DataType);
                object[] o = new object[resDt.Columns.Count];
                double allPeriod = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    allPeriod +=GetNumber(dt.Rows[i].ItemArray[2].ToString());
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    o[0] = dt.Rows[i].ItemArray[0];
                    o[1] = GetNumber(dt.Rows[i].ItemArray[2].ToString());
                    double m = 0;
                    o[2] = GetNumber(dt.Rows[i].ItemArray[2].ToString()) - GetNumber(dt.Rows[i].ItemArray[1].ToString());


                    if (GetNumber(dt.Rows[i].ItemArray[1].ToString())!=0)
                        {
                            o[3] = (GetNumber(dt.Rows[i].ItemArray[2].ToString()) / GetNumber(dt.Rows[i].ItemArray[1].ToString())) - 1;
                        }
                        else
                        {
                            o[3] = 0;
                        }
                   

                    o[4] = (GetNumber(o[1].ToString())) / allPeriod;
                    resDt.Rows.Add(o);
                }
                Grid.DataSource = resDt;
          */

            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "chart1", dt);
            resDt.Columns.Add("Класс заболеваний", dt.Columns[0].DataType);
            resDt.Columns.Add("Всего", dt.Columns[1].DataType);
            resDt.Columns.Add("Прирост", dt.Columns[1].DataType);
            resDt.Columns.Add("Темп прироста", dt.Columns[1].DataType);
            resDt.Columns.Add("Доля в общем объеме заболеваний", dt.Columns[1].DataType);
            object[] o = new object[resDt.Columns.Count];
            double allPeriod = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                allPeriod += GetNumber(dt.Rows[i].ItemArray[1].ToString());
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                o[0] = dt.Rows[i].ItemArray[0];
                o[1] = dt.Rows[i].ItemArray[1];
                o[2] = dt.Rows[i].ItemArray[2];
                o[3] = dt.Rows[i].ItemArray[3];
                o[4] = (GetNumber(o[1].ToString())) / allPeriod;
                resDt.Rows.Add(o);
            }
            Grid.DataSource = resDt;
        }

        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "chart2", dt);
            if (dt.Rows.Count < 1)
            {
                Chart2.DataSource = null;
            }
            else
            {
                Chart2.DataSource = dt;
            }
            
            offset = new int[dt.Rows.Count];
        }
        void UltraWebGrid_SetSelectedFoodparams(UltraGridRow GridRow)
        {
            if (GridRow.Cells[0].Value.ToString().Contains("]"))
            {
                string s = GridRow.Cells[0].Value.ToString();
                currentWay.Value = s.Insert(s.IndexOf(']'), "]");
            }
            else
            {
                currentWay.Value = GridRow.Cells[0].Value.ToString();
            }
        }
        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            UltraWebGrid_SetSelectedFoodparams(e.Row);
            Label6.Text = String.Format(chart2_title1, e.Row.Cells[0].Value.ToString());
            Chart2.DataBind();
            e.Row.Activate();
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[1].SortingAlgorithm = SortingAlgorithm.InsertionSort;
            e.Layout.Bands[0].Columns[0].SortingAlgorithm = SortingAlgorithm.InsertionSort;
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.51);
            }
            else 
            {
                if (BN == "FIREFOX")
                {
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.45);
                }
                else 
                {
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.52);
                }
            }
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (BN == "FIREFOX")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.09);
                }
                else 
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.1);
                }
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:10px;";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            headerLayout.childCells.Clear();
            GridHeaderCell header = headerLayout.AddCell("Класс заболеваний");
            
            header = headerLayout.AddCell("Всего");
            header = headerLayout.AddCell("Прирост");
            header = headerLayout.AddCell("Темп роста");
            header = headerLayout.AddCell("Доля в общем объеме заболеваний");
            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[3].Text != "")
            {
                if (GetNumber(e.Row.Cells[3].Text) > 0)
                {
                    e.Row.Cells[3].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                    e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; margin-left: 20px";
                }
                if (GetNumber(e.Row.Cells[3].Text) < 0)
                {
                    e.Row.Cells[3].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                    e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; margin-left: 20px";
                }
            }
            if (ComboYear.SelectedIndex==0)
            {
                e.Row.Cells[1].Text = String.Format("{0:0.#0}", GetNumber(e.Row.Cells[1].Text));
                e.Row.Cells[2].Text = "";
                e.Row.Cells[3].Text = "";
                e.Row.Cells[4].Text = String.Format("{0:0.##%}", GetNumber(e.Row.Cells[4].Text));
            }
        }

        protected void Chart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int boxCol = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            offset[boxCol] = box.rect.Height;
                            boxCol += 1;
                        }
                    }
            }
            int texCol = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text)
                {
                    Text tx = (Text)primitive;
                    string s=tx.GetTextString();
                    if (s[0] == '_')
                    {
                        if (texCol <= (boxCol - 1))
                        {
                            tx.bounds.Y += offset[texCol] / 2;
                            tx.bounds.X -= 20;
                            LabelStyle ls = new LabelStyle();
                            ls.Font = new System.Drawing.Font("Arial", 10,FontStyle.Bold);
                            tx.SetLabelStyle(ls);
                            texCol += 1;
                            tx.SetTextString(s.Remove(0,1));
                        }
                    }
                }
            }
        }
        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
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
            Chart2.Width = 900;
            ReportExcelExporter1.Export(Chart1, Label3.Text, sheet2, 2);
            ReportExcelExporter1.Export(Chart2, Label4.Text, sheet3, 3);
            Chart1.Width = width;
            Chart2.Width = width;
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
            e.Workbook.Worksheets["Таблица"].Rows[7].Cells[2].CellFormat.Alignment = HorizontalCellAlignment.Right;
            e.Workbook.Worksheets["Таблица"].Rows[7].Cells[3].CellFormat.Alignment = HorizontalCellAlignment.Right;
            e.Workbook.Worksheets["Таблица"].Rows[7].Cells[4].CellFormat.Alignment = HorizontalCellAlignment.Right;
            e.Workbook.Worksheets["Диаграмма 1"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма 2"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма 1"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[2].Cells[0].Value = Label6.Text;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0].Value = Label4.Text;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0].CellFormat.Font.Name = "Verdana";
            e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0].CellFormat.Font.Height = e.Workbook.Worksheets["Диаграмма 2"].Rows[2].Cells[0].CellFormat.Font.Height;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text+"\n"+Label3.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            int width = int.Parse(Chart1.Width.Value.ToString());
            Chart1.Width = 1000;
            Chart2.Width = 1000;
            ReportPDFExporter1.Export(Chart1,section1);
            ReportPDFExporter1.Export(headerLayout,Label5.Text, section2);
            ReportPDFExporter1.Export(Chart2,Label4.Text+"\n"+Label6.Text, section3);
            Chart1.Width = width;
            Chart2.Width = width;
        }

        #endregion

        protected void Chart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        protected void Chart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }



    }
}
