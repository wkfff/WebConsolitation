using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using Dundas.Maps.WebControl;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;

using Infragistics.UltraChart.Core.Layers;

using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.FO.FO_00021._0003
{
    public partial class _default : CustomReportPage
    {
        string page_title = "Расходы бюджета по состоянию на {0} {1} года";
        private CustomParam Budget_Level;
        private CustomParam currentDate;
        private CustomParam Year;
        private CustomParam Month;
        private CustomParam halfYear;
        private CustomParam quartYear;
        private double AvgParc = 0;
        private string Grid1h1 = "КОСГУ";
        private string Grid1h2 = "Назначено, рублей";
        private string Grid1h3 = "Исполнено, рублей";
        private string Grid1h4 = "Исполнено (%)";
        private string Grid1h5 = "Исполнено в текущем месяце, рублей";
        private string Grid2_title = "Расходы по РзПр";
        private string Grid1_title = "Расходы по КОСГУ";
        Dictionary<string, int> LevelsLoad()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            try
            {
                string[] s = RegionSettingsHelper.Instance.GetPropertyValue("ParamBudgetLevels").Split(';');
                for (int i = 0; i < s.Length; i++)
                {
                    d.Add(s[i], 0);
                }
            }
            catch
            {
            }
            return d;
        }
        Dictionary<string, int> PeriodLoad(string query)
        {
            Dictionary<string, int> d = GenDistonary(query);
            return d;
        }
        private int DotCount(string s)
        {
            int k = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '.')
                {
                    k += 1;
                }
            }
            return k;
        }
        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "dfd", dt);
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));


            int m = 0;
            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                m = DotCount(cs.Axes[1].Positions[i].Members[0].UniqueName);
                if (m == 3)
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);

                }

                if (m == 6)
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
                }


            }

            return d;
        }
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            base.Page_PreLoad(sender, e);
            Budget_Level = UserParams.CustomParam("Budget_Level");
            currentDate = UserParams.CustomParam("currentDate");
            Year = UserParams.CustomParam("Year");
            Month = UserParams.CustomParam("Month");
            quartYear = UserParams.CustomParam("quartYear");
            halfYear = UserParams.CustomParam("halfYear");
            if ((BN == "IE") || (BN == "FIREFOX"))
            {
                Grid1.Width = 1222;
                Grid2.Width = 1222;
            }
            else
            {
                Grid1.Width = 1222;
                Grid2.Width = 1222;
            }
            BudgetLevel.Width = 300;
            Period.Width = 150;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = false;
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
           // try
           // {
                base.Page_Load(sender, e);
                //RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");
                if (!Page.IsPostBack)
                {
                    BudgetLevel.Title = "Уровень бюджета";
                    BudgetLevel.FillDictionaryValues(LevelsLoad());
                    Period.FillDictionaryValues(PeriodLoad("lastDate"));
                    Period.PanelHeaderTitle = "Период";
                    Period.ShowSelectedValue = false;
                    Period.SelectLastNode();
                }
                
                
                Period.Title = "Период " + Period.SelectedNode.Parent.Text+" года";
                Budget_Level.Value = BudgetLevel.SelectedValue;
                int num = CRHelper.MonthNum(Period.SelectedValue);
                Year.Value = Period.SelectedNode.Parent.Text;
                Month.Value = Period.SelectedValue;
                halfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(num));
                quartYear.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(num));
                Label1.Text = String.Format(page_title, Period.SelectedValue.ToLower(), Year.Value);
                Label2.Text=Grid2_title;
                Label3.Text = Grid1_title;
                Page.Title = String.Format(page_title, Period.SelectedValue.ToLower(), Year.Value);
                Grid1.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                Grid2.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                AvgParc = (1 / 12.0)* CRHelper.MonthNum(Period.SelectedValue);
                Grid1.DataBind();
                Grid2.DataBind();
                Grid2.Rows.Add();
                for (int i = 0; i < Grid2.Columns.Count; i++)
                {
                    Grid2.Rows[Grid2.Rows.Count - 1].Cells[i].Text = dt1.Rows[dt1.Rows.Count - 1].ItemArray[i].ToString();
                }
                Grid1.Rows.Add(Grid2.Rows[Grid2.Rows.Count - 1]);
                Grid1.Rows[Grid1.Rows.Count - 1].Cells[0].Style.Font.Bold = 1 == 1;
                Grid1.Height = 23 * Grid1.Rows.Count;
                Grid2.Height = 24 * Grid2.Rows.Count;
                float m = 0;
                m = float.Parse(Grid1.Rows[Grid1.Rows.Count-1].Cells[4].Text);
                if (AvgParc > m)
                {
                    Grid1.Rows[Grid1.Rows.Count - 1].Cells[4].Style.BackgroundImage = "~/images/ballRedBB.png";
                    Grid1.Rows[Grid1.Rows.Count - 1].Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    double m1 = Math.Round(AvgParc * 100, 2);
                    Grid1.Rows[Grid1.Rows.Count - 1].Cells[4].Title = "Не соблюдается условие равномерности (" + m1.ToString() + "%)";
                }
                if (AvgParc < m)
                {
                    Grid1.Rows[Grid1.Rows.Count - 1].Cells[4].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    Grid1.Rows[Grid1.Rows.Count - 1].Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    double m1 = Math.Round(AvgParc * 100, 2);
                    Grid1.Rows[Grid1.Rows.Count - 1].Cells[4].Title = "Соблюдается условие равномерности (" + m1.ToString() + "%)";
                }
                

          //  }
          // catch { }
        }

        protected void Grid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid1"), "dfd", dt);
            Grid1.DataSource = dt;

        }
        DataTable dt1 = new DataTable();
        protected void Grid2_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid2"), "dfd", dt);
            Grid2.DataSource = dt;
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid2Itog"), "dfd", dt1);

        }

        protected void Grid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[4].Text != " ")
            {
                double m = 0;
                
                m = double.Parse(e.Row.Cells[4].Text);

                if (AvgParc > m)
                {
                    e.Row.Cells[4].Style.BackgroundImage = "~/images/ballRedBB.png";
                    e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    double m1 = Math.Round(AvgParc*100, 2);
                    e.Row.Cells[4].Title = "Не соблюдается условие равномерности (" + m1.ToString() + "%)";
                }
                if (AvgParc < m)
                {
                    e.Row.Cells[4].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    double m1 = Math.Round(AvgParc*100, 2);
                    e.Row.Cells[4].Title = "Соблюдается условие равномерности (" + m1.ToString() + "%)";
                }
            }
            if (e.Row.Cells[1].Text == "Группа")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            if (e.Row.Cells[1].Text == "Статья")
            {
                e.Row.Cells[0].Style.Font.Underline = true;
            }
        }

        protected void Grid2_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[4].Value!=null)
            {
                float m = 0;
                m = float.Parse(e.Row.Cells[4].Text);
                if (AvgParc > m)
                {
                    e.Row.Cells[4].Style.BackgroundImage = "~/images/ballRedBB.png";
                    e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    double m1 = Math.Round(AvgParc * 100, 2);
                    e.Row.Cells[4].Title = "Не соблюдается условие равномерности (" + m1.ToString() + "%)";
                }
                if (AvgParc < m)
                {
                    e.Row.Cells[4].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    double m1 = Math.Round(AvgParc * 100, 2);
                    e.Row.Cells[4].Title = "Соблюдается условие равномерности (" + m1.ToString() + "%)";
                }
            }
        }

        protected void Grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if ((BN == "IE") || (BN == "FIREFOX"))
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.455);
            }
            else
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.42);
            }
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.12);
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;

            }
            e.Layout.Bands[0].Columns[1].Hidden = true;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "###,##0.00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "###,##0.00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "###,##0.00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "#0.00 %");
            e.Layout.Bands[0].Columns[0].Header.Caption = Grid1h1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].Header.Caption = Grid1h2;
            e.Layout.Bands[0].Columns[3].Header.Caption = Grid1h3;
            e.Layout.Bands[0].Columns[4].Header.Caption = Grid1h4;
            e.Layout.Bands[0].Columns[5].Header.Caption = Grid1h5;
           
        }

        protected void Grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if ((BN == "IE") || (BN == "FIREFOX"))
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.455);
            }
            else
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.42);
            }
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.12);
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;

            }
            e.Layout.Bands[0].Columns[1].Hidden = true;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "###,##0.00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "###,##0.00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "###,##0.00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "#0.00 %");
            e.Layout.Bands[0].Columns[0].Header.Caption = "РзПр";
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].Header.Caption = Grid1h2;
            e.Layout.Bands[0].Columns[3].Header.Caption = Grid1h3;
            e.Layout.Bands[0].Columns[4].Header.Caption = Grid1h4;
            e.Layout.Bands[0].Columns[5].Header.Caption = Grid1h5;
        }
        #region Экспорт Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {


        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            string formatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Rows[0].Cells[4].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[0].Height = 650;
            for (int i = 1; i <= Grid1.Rows.Count; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 800;
            }
            e.CurrentWorksheet.Columns[0].Width = 700 * 37;
            for (int i = 1; i < Grid1.Columns.Count; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 110 * 37;
            }
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#0.00 %";

            e.CurrentWorksheet = e.Workbook.Worksheets[1];
            e.CurrentWorksheet.Rows[0].Cells[4].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[0].Height = 650;
            for (int i = 1; i <= Grid2.Rows.Count; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 800;
            }
            e.CurrentWorksheet.Columns[0].Width = 700 * 37;
            for (int i = 1; i < Grid2.Columns.Count; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 110 * 37;
            }
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#0.00 %";

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица1");
            Worksheet sheet2 = workbook.Worksheets.Add("Таблица2");
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 1;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(Grid1, sheet1);
            UltraGridExporter1.ExcelExporter.Export(Grid2,sheet2);
        }

        #endregion
    }
}
