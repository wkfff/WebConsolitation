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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0038
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private string query;
        private int firstYear = 2007;
        private int endYear = 2011;
        private string month;
       
        private GridHeaderLayout headerLayout;
        private DateTime currentDate;
 

        #region Параметры запроса

       private CustomParam selectedMOValue;
       
        #endregion

        private static MemberAttributesDigest moDigest;
        

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);


            #region Инициализация параметров запроса

            selectedMOValue = UserParams.CustomParam("selected_mo_value");
           

            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                query = DataProvider.GetQueryText("FO_0002_0038_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                moDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "MOList");
                ComboMO.Title = "Муниципальное образование";
                ComboMO.Width = 400;
                ComboMO.MultiSelect = false;
                ComboMO.ParentSelect = true;
                ComboMO.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(moDigest.UniqueNames, moDigest.MemberLevels));
                ComboMO.SetСheckedState("", true);
                
            }
 
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            selectedMOValue.Value = moDigest.GetMemberUniqueName(ComboMO.SelectedValue);
            
            Page.Title = "Анализ бюджета муниципального образования";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{0}, данные за {1} - {2} гг., тыс.руб.",ComboMO.SelectedValue, Convert.ToInt32(ComboYear.SelectedValue)-1, ComboYear.SelectedValue);
            
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedIndex == 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0038_grid2");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
            }
            else
            {
                query = DataProvider.GetQueryText("FO_0002_0038_grid1");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
            }
            if (dtGrid.Rows.Count > 0)
            {
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(220);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 1; i < columnCount; i++ )
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }

            for (int i = 3; i < columnCount; i += 3)
             {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
             }
             CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[columnCount], "P2");
             CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "P2");

            headerLayout.AddCell("Наименование показателя");
            GridHeaderCell cell1 = headerLayout.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue)-1));
            cell1.AddCell("План", "Уточненные годовые назначения",2);
            cell1.AddCell("Факт", "Фактическое исполнение за год",2);
            cell1.AddCell("% исполнения", "Процент исполнения годовых назначений",2);

            GridHeaderCell cell2 = headerLayout.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue)));
            cell2.AddCell("План", "Уточненные годовые назначения",2);
            cell2.AddCell("Факт", "Фактическое исполнение за год",2);
            cell2.AddCell("% исполнения", "Процент исполнения годовых назначений",2);
            GridHeaderCell cell3 = headerLayout.AddCell(string.Format("Отклонение текущего года от предыдущего"));
            GridHeaderCell cell31 = cell3.AddCell("в тыс. руб.");
            cell31.AddCell("План", string.Format("Отклонение годовых назначений на {0} год от годовых назначений на {1} год", ComboYear.SelectedValue ,Convert.ToInt32(ComboYear.SelectedValue)-1));
            cell31.AddCell("Факт", string.Format("Отклонение фактического исполнения за {0} год от фактического исполнения за {1} год", ComboYear.SelectedValue, Convert.ToInt32(ComboYear.SelectedValue) - 1));
            GridHeaderCell cell32 = cell3.AddCell("в %");
            cell32.AddCell("План", string.Format("Темп роста годовых назначений на {0} год по отношению к {1} году",ComboYear.SelectedValue, Convert.ToInt32(ComboYear.SelectedValue) - 1));
            cell32.AddCell("Факт", string.Format("Темп роста фактического исполнения за {0} год по отношению к {1} году", ComboYear.SelectedValue, Convert.ToInt32(ComboYear.SelectedValue) - 1));
          
            headerLayout.ApplyHeaderInfo();
        }

        private static bool IsInvertIndication(string indicatorName) // доходы
        {
            switch (indicatorName)
            {

                case "ДОХОДЫ":
                case "СОБСТВЕННЫЕ ДОХОДЫ":
                case "Налоговые доходы ":
                case "НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ":
                case "СТРАХОВЫЕ ВЗНОСЫ НА ОБЯЗАТЕЛЬНОЕ СОЦИАЛЬНОЕ СТРАХОВАНИЕ":
                case "НАЛОГИ НА ТОВАРЫ (РАБОТЫ, УСЛУГИ), РЕАЛИЗУЕМЫЕ НА ТЕРРИТОРИИ РОССИЙСКОЙ ФЕДЕРАЦИИ":
                case "НАЛОГИ НА ТОВАРЫ, ВВОЗИМЫЕ НА ТЕРРИТОРИЮ РОССИЙСКОЙ ФЕДЕРАЦИИ":
                case "НАЛОГИ НА СОВОКУПНЫЙ ДОХОД":
                case "НАЛОГИ НА ИМУЩЕСТВО":
                case "НАЛОГИ, СБОРЫ И РЕГУЛЯРНЫЕ ПЛАТЕЖИ ЗА ПОЛЬЗОВАНИЕ ПРИРОДНЫМИ РЕСУРСАМИ":
                case "ГОСУДАРСТВЕННАЯ ПОШЛИНА":
                case "ЗАДОЛЖЕННОСТЬ И ПЕРЕРАСЧЕТЫ ПО ОТМЕНЕННЫМ НАЛОГАМ, СБОРАМ И ИНЫМ ОБЯЗАТЕЛЬНЫМ ПЛАТЕЖАМ":
                case "Неналоговые доходы ( в целом) ":
                case "Доходы от предпринимательской и другой приносящей доход деятельности":
                case "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ":
                case "Субсидии":
                case "Cубвенции":
                case "Иные МБТ":
                case "Прочие безвозмездные поступления":
                case "ВСЕГО ДОХОДОВ БЮДЖЕТА":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }


       protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
       {
           string indicatorName = String.Empty;

           if (e.Row.Cells[0].Value != null)
           {
               indicatorName = e.Row.Cells[0].Value.ToString();
           }
           bool isInvertIndication = IsInvertIndication(indicatorName);

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
                if (RadioButtonList1.SelectedIndex == 0)
                {
                    if (i == 4 || i == 5)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {

                            double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);
                            double lastValue = Convert.ToDouble(e.Row.Cells[i - 3].Value);
                            if (currentValue > lastValue)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                           ? "~/images/arrowGreenUpBB.png"
                                                                           : "~/images/arrowRedUpBB.png";
                                e.Row.Cells[i].Title = "Рост по сравнению с предыдущим годом";
                            }
                            else if (currentValue < lastValue)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                           ? "~/images/arrowRedDownBB.png"
                                                                           : "~/images/arrowGreenDownBB.png";
                                e.Row.Cells[i].Title = "Снижение по сравнению с предыдущим годом";
                            }

                        }
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }

          if (e.Row.Cells[0] != null && e.Row.Cells[0].ToString() != string.Empty)
          {
              if (e.Row.Cells[0].Value.ToString() == "ИСТОЧНИКИ ФИНАНСИРОВАНИЯ ДЕФИЦИТА БЮДЖЕТА" || e.Row.Cells[0].Value.ToString() == "РАСХОДЫ")
              {
                  for (i = 0; i < e.Row.Cells.Count; i++)
                  {
                      e.Row.Cells[i].Style.BorderDetails.WidthTop = 2;
                      e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.Black;
                  }
                  e.Row.Cells[0].ColSpan = e.Row.Cells.Count;
                 
              }

              if (e.Row.Cells[e.Row.Cells.Count-1].ToString() == "1")
              {
                  e.Row.Cells[0].Style.Font.Bold = true;
              }

              
          }
          if (e.Row.Cells[0].Value != null)
          {
             if (e.Row.Cells[0].Value.ToString() == "ИСТОЧНИКИ ФИНАНСИРОВАНИЯ ДЕФИЦИТА БЮДЖЕТА" ||
                  e.Row.Cells[0].Value.ToString() == "РАСХОДЫ"
                  || e.Row.Cells[0].Value.ToString() == "ДОХОДЫ")
              {

                  e.Row.Cells[0].ColSpan = UltraWebGrid.Columns.Count - 2;

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

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}