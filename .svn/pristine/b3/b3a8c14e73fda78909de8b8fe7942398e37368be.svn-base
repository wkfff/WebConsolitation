using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0037
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private string query;
        private int firstYear = 2009;
        private int endYear = 2011;
        private string month;
       
        private GridHeaderLayout headerLayout;
        private DateTime currentDate;
 

        #region Параметры запроса

       private CustomParam selectedRzPrValue;
       
        #endregion

        private static MemberAttributesDigest rzprDigest;
        

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);


            #region Инициализация параметров запроса

            selectedRzPrValue = UserParams.CustomParam("selected_rzpr_value");
            
            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                query = DataProvider.GetQueryText("FO_0002_0037_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

                rzprDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "RzPrList");
                ComboRzPr.Title = "Раздел/подраздел";
                ComboRzPr.Width = 600;
                ComboRzPr.MultiSelect = false;
                ComboRzPr.ParentSelect = true;
                ComboRzPr.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(rzprDigest.UniqueNames, rzprDigest.MemberLevels));
                ComboRzPr.SetСheckedState("ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ", true);
                
            }
 
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            month = ComboMonth.SelectedValue;
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)));
            UserParams.PeriodMonth.Value = month;
            selectedRzPrValue.Value = rzprDigest.GetMemberUniqueName(ComboRzPr.SelectedValue);
            
            currentDate = new DateTime(year, CRHelper.MonthNum(month), 1);

            Page.Title = "Анализ отдельных расходов муниципальных образований";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{1}, данные на {0:dd.MM.yyyy}, тыс. руб.", currentDate, ComboRzPr.SelectedValue);
            

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedIndex == 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0037_grid1");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
            }
            else
            {
                query = DataProvider.GetQueryText("FO_0002_0037_grid2");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

                if (dtGrid.Rows.Count>0)
                {
                    for (int rowNum=0; rowNum<dtGrid.Rows.Count; rowNum++)
                    {
                        if (dtGrid.Rows[rowNum][0].ToString().Contains("ДАННЫЕ"))
                        {
                            dtGrid.Rows[rowNum][0] = string.Format("Собственный бюджет района");
                        }
                    }
                }

            }

            if (dtGrid.Rows.Count > 0)
            {
                for (int rowNum = 0; rowNum < dtGrid.Rows.Count; rowNum ++ )
                {
                    if (dtGrid.Rows[rowNum][1] != DBNull.Value && dtGrid.Rows[rowNum][1].ToString() != string.Empty)
                    {
                        dtGrid.Rows[rowNum][1] = Convert.ToDouble(dtGrid.Rows[rowNum][1])/1000;
                    }

                    if (dtGrid.Rows[rowNum][2] != DBNull.Value && dtGrid.Rows[rowNum][2].ToString() != string.Empty)
                    {
                        dtGrid.Rows[rowNum][2] = Convert.ToDouble(dtGrid.Rows[rowNum][2]) / 1000;
                    }
                }
                
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (((UltraWebGrid)sender).Rows.Count <= 10)
            {
                ((UltraWebGrid)sender).Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(280);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            int columnCount = e.Layout.Bands[0].Columns.Count-1;
            
            for (int i = 1; i < columnCount-1; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(130);

            headerLayout.AddCell("Муниципальные образования");
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue)-1, CRHelper.MonthNum(ComboMonth.SelectedValue),1);
            headerLayout.AddCell(string.Format("Исполнено на {0:dd.MM.yyyy} г.", currentDate),"Исполнено за аналогичный период предыдущего года");
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);
            headerLayout.AddCell(string.Format("Исполнено на {0:dd.MM.yyyy} г.", currentDate), "Исполнение по расходам нарастающим итогом с начала года");
            headerLayout.AddCell("Темп роста", "Темп роста к аналогичному периоду предыдущего года");
            headerLayout.AddCell("Доля, %", "Доля в общей сумме расходов данного вида");
            headerLayout.AddCell("Доля в прошлом году, %", "Доля в общей сумме расходов данного вида в прошлом году");
          
            headerLayout.ApplyHeaderInfo();
        }

       protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
       {
           int i;
            for (i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }

            i = 3;
            if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            {
                if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                {
                    e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                    e.Row.Cells[i].Title = string.Format("Сокращение по отношению к предыдущему году");
                }
                else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                {
                    e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                    e.Row.Cells[i].Title = string.Format("Рост по отношению к предыдущему году");
                }

                e.Row.Cells[i].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }


           if (e.Row.Cells[0]!= null && e.Row.Cells[0].ToString() != string.Empty)
           {
               if (e.Row.Cells[0].Value.ToString() == "Местные бюджеты Новосибирской области" || e.Row.Cells[0].Value.ToString() == "Городские округа" || e.Row.Cells[0].Value.ToString() == "Муниципальные районы") 
               {
                   e.Row.Cells[0].Style.Font.Bold = true;
               }

               if (RadioButtonList1.SelectedIndex == 1)
               {
                   if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].ToString() != string.Empty)
                   {
                       if (e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() == "3")
                       {
                           e.Row.Cells[0].Style.Font.Bold = true;
                       }
                   }
               }
           }

       }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
       }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}