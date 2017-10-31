using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Oil_0001_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
 
        private string query;
        private string shortFO;
        private DateTime date;
        
        private CustomParam selectedFO;
        private CustomParam selectedOil;
        private CustomParam lastDay;
        private DateTime lastDate;
        private DateTime currentDate;

        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        public bool AllOilKinds
        {
            get { return ComboOil.SelectedValue == "Все виды"; }
        }
        
      /// <summary>
        /// Выбраны ли все федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.BrowserSizeAdapting = false;
            GridBrick.Height = Unit.Empty;
            GridBrick.Width = Unit.Empty;
            GridBrick.RedNegativeColoring = false;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion
            
            #region Инициализация параматров
           
            selectedFO = UserParams.CustomParam("selected_fo");
            selectedOil = UserParams.CustomParam("selected_oil");
            lastDay = UserParams.CustomParam("last_day");
            
            #endregion

            ReportExcelExporter1.Visible = false;
            ReportPDFExporter1.Visible = false;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                query = DataProvider.GetQueryText("Oil_0001_0001_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                date = new DateTime(Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4]));

                CustomCalendar1.WebCalendar.SelectedDate = date;

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState("Центральный федеральный округ", true);

                Dictionary<string, int> typeOil = new Dictionary<string, int>();
                typeOil.Add("Все виды", 0);
                typeOil.Add("Бензин марки Аи-92", 0);
                typeOil.Add("Бензин марки Аи-95", 0);
                typeOil.Add("Дизельное топливо", 0);

                ComboOil.Title = "Вид нефтепродуктов";
                ComboOil.Width = 300;
                ComboOil.MultiSelect = false;
                ComboOil.FillDictionaryValues(typeOil);
                ComboOil.SetСheckedState("Все виды", true);
            }

            Page.Title = "Цены на нефтепродукты (ФАС)";
            Label1.Visible = false;
            Label1.Text = Page.Title;
            Label2.Text = "Розничные цены на нефтепродукты по субъектам РФ и компаниям по данным Федеральной антимонопольной службы";

            shortFO = RFSelected ? "РФ" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue);

            selectedFO.Value = RFSelected ? " " : string.Format(".[{0}]", ComboFO.SelectedValue);

            switch (ComboOil.SelectedIndex)
            {
                case 0:
                    {
                        selectedOil.Value = "[Все виды топлива]";
                        break;
                    }
                case 1:
                    {
                        selectedOil.Value = "[Организации].[Реестр продукции сопоставимый].[Вся продукуция].[Бензин АИ-92]";
                        break;
                    }
                case 2:
                    {
                        selectedOil.Value = "[Организации].[Реестр продукции сопоставимый].[Вся продукуция].[Бензин АИ-95]";
                        break;
                    }
                case 3:
                    {
                        selectedOil.Value = "[Организации].[Реестр продукции сопоставимый].[Вся продукуция].[Дизельное топливо]";
                        break;
                    }
            }


            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, CustomCalendar1.WebCalendar.SelectedDate, 5);
            UserParams.PeriodYear.Value = CustomCalendar1.WebCalendar.SelectedDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(UserParams.PeriodYear.Value) - 1).ToString();

            currentDate = CustomCalendar1.WebCalendar.SelectedDate;
            lastDate = new DateTime(currentDate.Year - 1, 12, 30);

            query = DataProvider.GetQueryText("Oil_0001_0001_lastDate");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            lastDay.Value = string.Format("[Период].[День].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}].[{4}]",
                                          dtDate.Rows[0][0], dtDate.Rows[0][1], dtDate.Rows[0][2], dtDate.Rows[0][3],
                                          dtDate.Rows[0][4]);

            InitializeTable1();

            if (AllOilKinds)
            {
                UltraChart1.ReportDate = currentDate;
                UltraChart2.ReportDate = currentDate;
                UltraChart3.ReportDate = currentDate;

                UltraChart1.LastDate = lastDate;
                UltraChart2.LastDate = lastDate;
                UltraChart3.LastDate = lastDate;

                UltraChart1.OilName = "Бензин марки АИ-92";
                UltraChart2.OilName = "Бензин марки АИ-95";
                UltraChart3.OilName = "Дизельное топливо";

                UltraChart1.SelectedOil = "[Организации].[Реестр продукции сопоставимый].[Вся продукуция].[Бензин АИ-92]";
                UltraChart2.SelectedOil = "[Организации].[Реестр продукции сопоставимый].[Вся продукуция].[Бензин АИ-95]";
                UltraChart3.SelectedOil = "[Организации].[Реестр продукции сопоставимый].[Вся продукуция].[Дизельное топливо]";

                UltraChart1.SelectedFO = selectedFO.Value;
                UltraChart2.SelectedFO = selectedFO.Value;
                UltraChart3.SelectedFO = selectedFO.Value;

                UltraChart1.ShortFO = shortFO;
                UltraChart2.ShortFO = shortFO;
                UltraChart3.ShortFO = shortFO;

                UltraChart1.Visible = true;
                UltraChart2.Visible = true;
                UltraChart3.Visible = true;
            }
            else
            {
                UltraChart1.ReportDate = currentDate;
                UltraChart1.LastDate = lastDate;
                UltraChart1.OilName = ComboOil.SelectedValue;
                UltraChart1.SelectedOil = selectedOil.Value;
                UltraChart1.SelectedFO = selectedFO.Value;
                UltraChart1.ShortFO = shortFO;

                UltraChart1.Visible = true;
                UltraChart2.Visible = false;
                UltraChart3.Visible = false;
            }
        }

       #region Настройка грида

       private DataTable dt;

       private void InitializeTable1()
       {
           dt = new DataTable();
           string query = DataProvider.GetQueryText("OIL_0001_0001_Grid");
           DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

           if (dt.Columns.Count > 0)
           {
               dt.Columns.RemoveAt(0);
           }

           PaddingRule rule = new PaddingRule(0, "Уровень", 20);
           GridBrick.AddIndicatorRule(rule);

           GrowRateRule growRateRule = new GrowRateRule("Темп прироста, %");
           growRateRule.IncreaseImg = "~/images/ArrowRedUpIPad.png";
           growRateRule.DecreaseImg = "~/images/ArrowGreenDownIPad.png";
           growRateRule.Limit = 0;
           GridBrick.AddIndicatorRule(growRateRule);

           GridBrick.DataTable = dt;
       }

       private void Grid_InitializeRow(object sender, RowEventArgs e)
       {
           double limit = GetRowValue(e.Row, 10);
           double value = GetRowValue(e.Row, 1);

           setStar(e.Row, 1, 6, 7);
           setStar(e.Row, 3, 9, 8);

           e.Row.Cells[1].Style.Font.Bold = true;

           if (e.Row.Cells[5].Value.ToString() == "-1")
           {
               e.Row.Cells[0].ColSpan = 5;
               e.Row.Cells[0].Style.Font.Bold = true;
               e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
               e.Row.Cells[0].Style.Font.Size = 16;
           }
           else if (e.Row.Cells[5].Value.ToString() == "0" && !e.Row.Cells[0].ToString().Contains("Среднее"))
           {
               e.Row.Cells[0].Style.Font.Bold = true;
               e.Row.Cells[1].ColSpan = 4;
               e.Row.Style.BorderDetails.WidthTop = 3;

               if (limit != Double.MinValue && limit > 0)
               {
                   e.Row.Cells[1].Value = String.Format("порог Минэнерго России 31.12.2011:&nbsp;<b>{0:N2}</b>&nbsp;руб.", limit);
                   e.Row.Cells[1].Style.Font.Bold = false;
               }
               else
               {
                   e.Row.Cells[1].Value = String.Empty;
               }
           }
           else if (value != Double.MinValue && limit != Double.MinValue && value > limit && limit != 0)
           {
               e.Row.Cells[1].Value = String.Format("<img src='../../images/ballRedBB.png' style='margin-bottom:-4px'>&nbsp;{0:N2}", value);
           }
       }

       private static void setStar(UltraGridRow row, int valueIndex, int bestIndex, int worseIndex)
       {
           double value = GetRowValue(row, valueIndex);
           double bestValue = GetRowValue(row, bestIndex);
           double worseValue = GetRowValue(row, worseIndex);

           if (value != Double.MinValue && value == bestValue)
           {
               row.Cells[valueIndex].Style.BackgroundImage = "~/images/starGrayBB.png";
               row.Cells[valueIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 10% center; padding-left: 2px";
           }

           if (value != Double.MinValue && value == worseValue)
           {
               row.Cells[valueIndex].Style.BackgroundImage = "~/images/starYellowBB.png";
               row.Cells[valueIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 10% center; padding-left: 2px";
           }
       }

       private static double GetRowValue(UltraGridRow e, int index)
       {
           if (e.Cells.Count > index)
           {
               if (e.Cells[index].Value != null && e.Cells[index].Value.ToString() != String.Empty)
               {
                   return Convert.ToDouble(e.Cells[index].Value);
               }
           }

           return Double.MinValue;
       }

       void Grid_InitializeLayout(object sender, LayoutEventArgs e)
       {
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

           e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
           e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

           e.Layout.Bands[0].Columns[5].Hidden = true;
           e.Layout.Bands[0].Columns[6].Hidden = true;
           e.Layout.Bands[0].Columns[7].Hidden = true;
           e.Layout.Bands[0].Columns[8].Hidden = true;
           e.Layout.Bands[0].Columns[9].Hidden = true;
           e.Layout.Bands[0].Columns[10].Hidden = true;

           GridHeaderLayout headerLayout1 = GridBrick.GridHeaderLayout;
           headerLayout1.AddCell("");
           headerLayout1.AddCell(string.Format("Розничная цена на {0:dd.MM.yyyy}", currentDate));
           headerLayout1.AddCell(string.Format("Розничная цена на {0:dd.MM.yyyy} ", lastDate));
           headerLayout1.AddCell("Абс.откл., руб.");
           headerLayout1.AddCell("Темп прироста, %");

           headerLayout1.ApplyHeaderInfo();

           e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(340);
           e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(130);
           e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(130);
           e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(130);
           e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(112);

           e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 12;
           e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 12;
           e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 12;
           e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 12;
           e.Layout.Bands[0].Columns[4].CellStyle.Font.Size = 12;
       }

       #endregion

        #region Экспорт

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
//            ReportExcelExporter1.WorksheetTitle = Label1.Text;
//            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
//
//            Workbook workbook = new Workbook();
//            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
//            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
//            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
//
//            ReportExcelExporter1.HeaderCellHeight = 20;
//            ReportExcelExporter1.GridColumnWidthScale = 1.5;
//            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
//            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 3);
        }

        #endregion
      
        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
//            ReportPDFExporter1.PageTitle = Label1.Text;
//            ReportPDFExporter1.PageSubTitle = Label2.Text;
//
//            Report report = new Report();
//            ISection section1 = report.AddSection();
//            ISection section2 = report.AddSection();
//            ISection section3 = report.AddSection();
//
//            ReportPDFExporter1.HeaderCellHeight = 50;
//            ReportPDFExporter1.Export(headerLayout, section1);
//            UltraChart1.Width = 1000;
//            ReportPDFExporter1.Export(UltraChart1,chart1ElementCaption.Text, section2);
        }

        #endregion

        #endregion
    }
}
