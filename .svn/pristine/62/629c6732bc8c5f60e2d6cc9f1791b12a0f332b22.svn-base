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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0001_XMAO
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        DataTable dtDate = new DataTable();
        private string query;
        private int firstYear = 2008;
        private int endYear;
        private string month; 
       
        private GridHeaderLayout headerLayout;

      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 1.16);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/1.45);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

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
                dtDate = new DataTable();
                query = DataProvider.GetQueryText("FO_0001_0005_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string quarter = dtDate.Rows[0][2].ToString();
                
                if (quarter != "Квартал 4")
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]) - 1;
                }
                else
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
         
                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(endYear.ToString(), true);
                
            }


            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0001_0005_data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            month = dtDate.Rows[0][3].ToString();

            Page.Title = "Среднемесячная заработная плата работников органов государственной власти и органов местного самоуправления Ханты-Мансийского автономного округа-Югры";
            PageTitle.Text = Page.Title;
            DateTime currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(month), 1);
            PageSubTitle.Text = string.Format("{0} по состоянию на  {1:dd.MM.yyyy} г.", DebtKindButtonList.SelectedIndex == 0 ? "Органы государственной власти" : "Органы местного самоуправления" , currentDate);

            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
       
           
        }

        #region Обработчик грида

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            if (DebtKindButtonList.SelectedIndex == 0) // Форма_14
            {
                string query = DataProvider.GetQueryText("FO_0001_0005_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "ГРБС",
                                                                                 dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                   
                  

                    dtGrid.AcceptChanges();
                    UltraWebGrid1.DataSource = dtGrid;
                }
            }
            else // Форма_14 МО
            {
                string query = DataProvider.GetQueryText("FO_0001_0005_Grid_MO");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "МО",
                                                                                 dtGrid);
                if (dtGrid.Rows.Count>0)
                {
                    UltraWebGrid1.DataSource = dtGrid;
                }

            }
        }

      
        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(450);

            if (DebtKindButtonList.SelectedIndex == 0)
            {
                headerLayout.AddCell("Наименование государственного органа", "", 2);

              
                GridHeaderCell cell4 = headerLayout.AddCell("Среднемесячная начисленная заработная плата работников государственного органа, тыс.руб./чел.");
                cell4.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell4.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell4.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell5 = headerLayout.AddCell("Среднемесячная начисленная заработная плата лиц, замещающих должности государственной гражданской службы, тыс.руб./чел.");
                cell5.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell5.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell5.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

            }
            else
            {
                headerLayout.AddCell("Наименование муниципального образования", "", 2);

                GridHeaderCell cell4 = headerLayout.AddCell("Среднемесячная начисленная заработная плата работников органа местного самоуправления, избирательной комиссии МО, тыс.руб./чел.");
                cell4.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell4.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell4.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell5 = headerLayout.AddCell("Среднемесячная начисленная заработная плата лиц, замещающих должности муниципальной службы, тыс.руб./чел.");
                cell5.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell5.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell5.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

            }
            
            headerLayout.ApplyHeaderInfo();

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(85);
            }
            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i+=3 )
            {
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P1");
               e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(85);
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

                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;

                if (e.Row.Cells[0].Value.ToString() == "Итого")
                {
                    
                    e.Row.Cells[0].Style.Font.Bold = true;
                }

              
                if (i == 3 || i==6 )
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому отчетному году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому отчетному году";
                        }

                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }
              
        }

        #endregion 
  

        #region экспорт

      
        #region экспорт в Excel

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            
        }
        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 5; j < rowsCount + 5; j++)
                {
                   e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Font.Height = 200;
                }
            }
        }

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