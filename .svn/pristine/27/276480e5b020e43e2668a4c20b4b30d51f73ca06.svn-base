using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;


namespace Krista.FM.Server.Dashboards.reports.FO_0007_0001
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private int firstYear = 2010;
        private int endYear = 2011;

        private GridHeaderLayout headerLayout;
        
        private static MemberAttributesDigest kosguDigest;

        private CustomParam area;
        private CustomParam measures;
      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -10);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #region Инициализация параметров

            area = UserParams.CustomParam("area");
            measures = UserParams.CustomParam("measures");

            #endregion
        }
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0007_0001_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                
                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(endYear.ToString(),true);

                kosguDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0007_0001_Budget");
                ComboBudget.Title = "Бюджет";
                ComboBudget.Visible = true;
                ComboBudget.Width = 450;
                ComboBudget.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(kosguDigest.UniqueNames,kosguDigest.MemberLevels));
                ComboBudget.SetСheckedState("",true);
            }

            Page.Title = string.Format("Анализ динамики расходов в разрезе КОСГУ: {0}", ComboBudget.SelectedValue);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("Данные за {0} - {1} годы, тыс.руб.", Convert.ToInt32(ComboYear.SelectedValue)-2,ComboYear.SelectedValue);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            area.Value = kosguDigest.GetMemberUniqueName(ComboBudget.SelectedValue);
            measures.Value = CheckBox1.Checked ? "[Measures].[Факт]" : "[Measures].[Факт_за период]";
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
        }

        #region Обработчик грида

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
           string query = DataProvider.GetQueryText("FO_0007_0001_Grid");
           dtGrid = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,"Наименование государственного органа",dtGrid);
           if (dtGrid.Rows.Count > 0)
            {
              dtGrid.AcceptChanges();
              UltraWebGrid1.DataSource = dtGrid;
            }
          
        }

      
        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            string[] caption;

            headerLayout.AddCell("КОСГУ");
            headerLayout.AddCell("Код");

            for (int i=2; i<e.Layout.Bands[0].Columns.Count-1; i+=5)
            {
                caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                GridHeaderCell cell = headerLayout.AddCell(string.Format("{0}", caption));
                cell.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue)-2));
                cell.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell.AddCell("Темп роста");
                cell.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue)));
                cell.AddCell("Темп роста");
            }

            headerLayout.ApplyHeaderInfo();

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(70);
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(50);

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i+=5)
            {
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+1], "N2");
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+2], "P2");
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+3], "N2");
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+4], "P2");
            }
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            e.Row.Cells[i].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }

           for (int i=4; i<e.Row.Cells.Count; i+=5)
           {
             e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
               
             if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
             {
                 if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                 {
                     e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                     e.Row.Cells[i].Title = "Снижение к прошлому году";
                 }
                 else if (100*Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                 {
                     e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                     e.Row.Cells[i].Title = "Прирост к прошлому году";
                 }

                 e.Row.Cells[i].Style.CustomRules =
                     "background-repeat: no-repeat; background-position: left center; margin: 2px";
             }

             if (e.Row.Cells[i+2].Value != null && e.Row.Cells[i+2].Value.ToString() != string.Empty)
             {
                 if (100 * Convert.ToDouble(e.Row.Cells[i+2].Value) < 100)
                 {
                     e.Row.Cells[i+2].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                     e.Row.Cells[i + 2].Title = "Снижение к прошлому году";
                 }
                 else if (100 * Convert.ToDouble(e.Row.Cells[i+2].Value) >= 100)
                 {
                     e.Row.Cells[i+2].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                     e.Row.Cells[i + 2].Title = "Прирост к прошлому году";
                 }

                 e.Row.Cells[i+2].Style.CustomRules =
                     "background-repeat: no-repeat; background-position: left center; margin: 2px";
             }

           }

            string level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
            
            if (level=="1")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            
        }

        #endregion 
  

        #region экспорт

      
        #region экспорт в Excel

       
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в Pdf

        
        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
           
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
         

        }

        #endregion

      #endregion


       }
}