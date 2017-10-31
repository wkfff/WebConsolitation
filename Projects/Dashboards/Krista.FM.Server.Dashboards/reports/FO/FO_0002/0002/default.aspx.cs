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

namespace Krista.FM.Server.Dashboards.reports.FO.FO_0002._00020
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Доходы бюджета по состоянию на {0} {1} года";
        private string h1 = "КД";
        private string h2 = "Назначено, рублей";
        private string h3 = "Исполнено, рублей";
        private string h4 = "Исполнено (%)";
        private string h5 = "Исполнено в текущем месяце, рублей";
        string ls = "";
        string ls2 = "";
        private CustomParam Period;
        private CustomParam BudgetLevel;
        private CustomParam currentDate;
        private CustomParam Year;
        private CustomParam Month;
        private CustomParam halfYear;
        private CustomParam quartYear;
        private double AvgParc=0;
        string BN = "IE";
        protected override void  Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender,e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            Period = UserParams.CustomParam("Period");
            BudgetLevel = UserParams.CustomParam("Budget_Level");
            currentDate = UserParams.CustomParam("currentDate");
            Year = UserParams.CustomParam("Year");
            Month = UserParams.CustomParam("Month");
            quartYear = UserParams.CustomParam("quartYear");
            halfYear = UserParams.CustomParam("halfYear");
            LevelsCombo.Width = 300;
            PeriodCombo.Width = 150;
            UltraGrid.Width = 1210;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = false;
        
        }
        protected  override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender,e);
            try
            {
                if (!Page.IsPostBack)
                {
                    LevelsCombo.FillDictionaryValues(LevelsLoad());
                    LevelsCombo.Title = "Уровень бюджета";
                    Dictionary<string, int> d = GenDistonary("lastDate");
                    PeriodCombo.FillDictionaryValues(d);
                    PeriodCombo.SelectLastNode();
                    PeriodCombo.ShowSelectedValue = false;
                    PeriodCombo.PanelHeaderTitle = "Период";

                }
                BudgetLevel.Value = LevelsCombo.SelectedValue;
                AvgParc = (1 / 12.0) * CRHelper.MonthNum(PeriodCombo.SelectedValue);
                currentDate.Value = PeriodCombo.SelectedValuesString;
                int num = CRHelper.MonthNum(PeriodCombo.SelectedValue);
                Year.Value = PeriodCombo.SelectedNode.Parent.Text;
                Month.Value = PeriodCombo.SelectedValue;
                halfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(num));
                quartYear.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(num));
                Label1.Text = String.Format(page_title, PeriodCombo.SelectedValue.ToLower(), PeriodCombo.SelectedNode.Parent.Text);
                UltraGrid.DataBind();


                UltraGrid.Height = 27 * UltraGrid.Rows.Count;
                UltraGrid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                Page.Title = String.Format(page_title, PeriodCombo.SelectedValue.ToLower(), PeriodCombo.SelectedNode.Parent.Text);
            }
            catch { }
        }

        Dictionary<string, int> LevelsLoad()
        {

            Dictionary<string, int> d = new Dictionary<string, int>();
            try
            {
                string[] s = RegionSettingsHelper.Instance.GetPropertyValue("ParamBudgetLevels").Split(';');
                for (int i = 0; i < s.Length; i++)
                {
                    d.Add(s[i],0);
                }
            }
            catch 
            { 
            }
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
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql),"dfd",dt);
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));


            int m = 0;
            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                m = DotCount(cs.Axes[1].Positions[i].Members[0].UniqueName);
                if (m==3)
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption,0);

                }

                if (m == 6)
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
                }


            }

            return d;
        }
        protected void UltraGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "ds", dt);
            UltraGrid.DataSource = dt;

        }

        protected void UltraGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.45);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.42);
            }
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.43);
            }
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1==1;
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
            e.Layout.Bands[0].Columns[0].Header.Caption = h1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].Header.Caption = h2;
            e.Layout.Bands[0].Columns[3].Header.Caption = h3;
            e.Layout.Bands[0].Columns[4].Header.Caption = h4;
            e.Layout.Bands[0].Columns[5].Header.Caption = h5;
            
        }

        protected void UltraGrid_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[4].Text != " ")
                {
                    double m = 0;
                    m = double.Parse(e.Row.Cells[4].Text);
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
                        e.Row.Cells[4].Title = "Соблюдается условие равномерности ("+m1.ToString()+"%)";
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
            catch
            { }
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
            for (int i = 1; i <= UltraGrid.Rows.Count; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 800;
            }
            e.CurrentWorksheet.Columns[0].Width = 700 * 37;
            for (int i = 1; i < UltraGrid.Columns.Count; i++)
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
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 1;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraGrid, sheet1);
            
        }

        #endregion
    }
}
