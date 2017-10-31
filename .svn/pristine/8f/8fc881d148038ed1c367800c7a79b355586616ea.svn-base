using System;
using System.Collections.Generic;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0033_02
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private string query;
        private int day;
        private int firstYear = 2006;
        private int endYear = 2011;
        private string quarter;
        private string month;
        private int currentMonth;
        private int nColumn;
        private int tempCh;
        private int tempZn;
        private GridHeaderLayout headerLayout;
        private DateTime currentDate;
        private int selectedQuarterIndex;

        #region Параметры запроса

        private CustomParam selectedProgrammValue;
        private CustomParam selectedAdminValue;
        private CustomParam selectedRzPrValue;
        private CustomParam selectedKosguValue;
        private CustomParam measures;
        private CustomParam tempMeasuresCh;
        private CustomParam tempMeasuresZn;
        
        #endregion

        private static MemberAttributesDigest programmDigest;
        private static MemberAttributesDigest adminDigest;
        private static MemberAttributesDigest rzprDigest;
        private static MemberAttributesDigest kosguDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.8);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Исполнение&nbsp;по&nbsp;ГРБС";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0017_03/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Исполнение&nbsp;по&nbsp;КОСГУ";
            CrossLink2.NavigateUrl = "~/reports/FO_0002_0033_03/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Исполнение&nbsp;по&nbsp;РзПр";
            CrossLink3.NavigateUrl = "~/reports/FO_0002_0033_01/Default.aspx";

            #region Инициализация параметров запроса

            selectedAdminValue = UserParams.CustomParam("selected_admin_value");
            selectedProgrammValue = UserParams.CustomParam("selected_programm_value");
            selectedRzPrValue = UserParams.CustomParam("selected_rzpr_value");
            selectedKosguValue = UserParams.CustomParam("selected_kosgu_value");
            measures = UserParams.CustomParam("measures");
            tempMeasuresCh = UserParams.CustomParam("temp_ch");
            tempMeasuresZn = UserParams.CustomParam("temp_zn");

            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                query = DataProvider.GetQueryText("FO_0002_0033_02_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 150;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.SetСheckedState(quarter, true);

                programmDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0033_02_PurposeProgramm_Digest");
                ComboProgram.Title = "ОЦП";
                ComboProgram.Width = 500;
                ComboProgram.MultiSelect = true;
                ComboProgram.ParentSelect = true;
                ComboProgram.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(programmDigest.UniqueNames, programmDigest.MemberLevels));
                ComboProgram.SetСheckedState("Целевые программы", true);

                adminDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0033_02_Admin_Digest");
                ComboAdmin.Title = "ГРБС";
                ComboAdmin.Width = 400;
                ComboAdmin.MultiSelect = false;
                ComboProgram.ParentSelect = true;
                ComboAdmin.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(adminDigest.UniqueNames, adminDigest.MemberLevels));
                ComboAdmin.SetСheckedState("Все ГРБС", true);

                rzprDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0033_02_AllRzPr");
                ComboRzPr.Title = "КБК";
                ComboRzPr.Width = 500;
                ComboRzPr.MultiSelect = true;
                ComboRzPr.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboRzPr.ParentSelect = true;
                ComboRzPr.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(rzprDigest.UniqueNames, rzprDigest.MemberLevels));
                ComboRzPr.SetСheckedState("Все КБК", true);

                kosguDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0033_02_KOSGU_Digest");
                ComboKOSGU.Title = "КОСГУ ";
                ComboKOSGU.Width = 400;
                ComboKOSGU.MultiSelect = false;
                ComboKOSGU.ParentSelect = true;
                ComboKOSGU.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(kosguDigest.UniqueNames, kosguDigest.MemberLevels));
                ComboKOSGU.SetСheckedState("Все коды ЭКР", true);
            }
 
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0033_02_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            day = Convert.ToInt32(dtDate.Rows[0][4]);
            quarter = dtDate.Rows[0][2].ToString();
            month = dtDate.Rows[0][3].ToString();
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            if (ComboQuarter.SelectedValue == quarter && ComboYear.SelectedValue == endYear.ToString())
            {
                currentMonth = CRHelper.MonthNum(month);
            }
            else
            {
                currentMonth = 3 * selectedQuarterIndex;
            }
            currentDate = new DateTime(yearNum, currentMonth, (ComboQuarter.SelectedValue == quarter && Convert.ToInt32(ComboYear.SelectedValue) == endYear) ? day : CRHelper.QuarterLastDay(selectedQuarterIndex));


            Page.Title = "Динамика исполнения областного бюджета по областным и долгосрочным целевым программам";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Динамика исполнения областного бюджета по состоянию на {0:dd.MM.yyyy}, {1}, {2}, {3}, {4}, тыс. руб.", currentDate, ComboProgram.SelectedValue, ComboAdmin.SelectedValue, ComboRzPr.SelectedValue, ComboKOSGU.SelectedValue);

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            
            

            if (ComboProgram.SelectedValues.Count > 0)
            {
                string program = string.Empty;
                for (int i=0; i<ComboProgram.SelectedValues.Count; i++)
                {
                    program += (ComboProgram.SelectedValues[i] =="Целевые программы") ? "[Список цп]," : string.Format("{0},", programmDigest.GetMemberUniqueName(ComboProgram.SelectedValues[i]));
                }

                selectedProgrammValue.Value = program.TrimEnd(',');
            }
            else
            {
                selectedProgrammValue.Value = programmDigest.GetMemberUniqueName("( 5220300 )\"Газификация Новосибирской области на 2007-2011 годы\"");
            }

            if (ComboRzPr.SelectedValues.Count > 0 )
            {
                string bill = string.Empty;
                for (int i = 0; i < ComboRzPr.SelectedValues.Count; i++)
                {
                    
                    if (ComboRzPr.SelectedValues[i] == "Все КБК")
                    {

                        bill += string.Format(" Descendants ( [Расходы__Базовый].[Расходы__Базовый].[Все], [Расходы__Базовый].[Расходы__Базовый].[Расходы уровень 6], SELF ) +") ;
                    }
                    else
                    {
                        bill += string.Format(" Descendants ( {0}, [Расходы__Базовый].[Расходы__Базовый].[Расходы уровень 6], SELF ) +", rzprDigest.GetMemberUniqueName(ComboRzPr.SelectedValues[i]));
                    }
                     
                   
                }

               selectedRzPrValue.Value = bill.TrimEnd('+');
            }
            else
            {
                selectedRzPrValue.Value = rzprDigest.GetMemberUniqueName("( 100 ) Общегосударственные вопросы");
            }

            selectedAdminValue.Value = (ComboAdmin.SelectedIndex == 0) ? "[Администраторы]" : adminDigest.GetMemberUniqueName(ComboAdmin.SelectedValue);
            selectedKosguValue.Value = kosguDigest.GetMemberUniqueName(ComboKOSGU.SelectedValue);
          
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }


        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            int index = DebtKindButtonList1.SelectedIndex;

            switch (index)
            {
                case 0:   // утвержденный план на год
                    {
                        measures.Value = "[Measures].[План, утвержденный законодательно ]";
                        nColumn = 6;
                        break;
                    }
                case 1:   // уточненный план на год
                    {
                        measures.Value = "[Measures].[Уточненный план на год ]";
                        nColumn = 7;
                        break;
                    }
                case 2:   // уточненный план на конец квартала
                    {
                        measures.Value = "[Measures].[Уточненный план на квартал ]";
                        nColumn = 8;
                        break;
                    }
            }

            index = DebtKindButtonList2.SelectedIndex;
            switch (index)
            {
                case 0:   // утвержденный план на год
                    {
                        tempMeasuresCh.Value = "[Measures].[План, утвержденный законодательно ]";
                        tempMeasuresZn.Value = "[Measures].[План, утвержденный законодательно на аналогичный период предыдущего года ]";
                        tempCh = 6;
                        tempZn = 12;
                        break;
                    }
                case 1:   // уточненный план на год
                    {
                        tempMeasuresCh.Value = "[Measures].[Уточненный план на год ]";
                        tempMeasuresZn.Value = "[Measures].[Уточненный план на предыдущий год ]";
                        tempCh = 7;
                        tempZn = 13;
                        break;
                    }
                case 2:   // уточненный план на конец квартала
                    {
                        tempMeasuresCh.Value = "[Measures].[Уточненный план на квартал ]";
                        tempMeasuresZn.Value = "[Measures].[Уточненный план на аналогичный квартал предыдущего года ]";
                        tempCh = 8;
                        tempZn = 14;
                        break;
                    }
                case 3:   // кассовое исполнение
                    {
                        tempMeasuresCh.Value = "[Measures].[Кассовое исполнение на дату ]";
                        tempMeasuresZn.Value = "[Measures].[Кассовое исполнение на аналогичную дату предыдущего года ]";
                        tempCh = 9;
                        tempZn = 15;
                        break;
                    }
            }




            string query = DataProvider.GetQueryText("FO_0002_0033_02_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                for (int rowNum = 0; rowNum < dtGrid.Rows.Count; rowNum++ )
                {
                    string[] caption = dtGrid.Rows[rowNum][0].ToString().Split(';');
                    dtGrid.Rows[rowNum][0] = caption[0];
                 }
                
                DataRow row = dtGrid.NewRow();
                row[0] = "Итого";
                row[1] = DBNull.Value;
                row[2] = DBNull.Value;
                row[3] = DBNull.Value;
                row[4] = DBNull.Value;
                row[5] = DBNull.Value;
              
                for (int colNum = 6; colNum < 10; colNum++)
                {
                    double sum = 0;
                    for (int rowNum = 0; rowNum < dtGrid.Rows.Count; rowNum++)
                    {
                        if (dtGrid.Rows[rowNum][colNum] != DBNull.Value && dtGrid.Rows[rowNum][colNum].ToString() != string.Empty)
                        { 
                            if (dtGrid.Rows[rowNum][dtGrid.Columns.Count-1].ToString() == "1")
                            {
                                sum = sum + Convert.ToDouble(dtGrid.Rows[rowNum][colNum]);
                            }
                        }
                    }

                    if (sum != 0)
                    {
                        row[colNum] = sum;
                    }
                    else
                    {
                        row[colNum] = DBNull.Value;
                    }
                }

                if (row[nColumn] != DBNull.Value && row[nColumn].ToString()!=string.Empty)
                {
                    row[10] = Convert.ToDouble(row[nColumn]);
                }
                if (row[10] != DBNull.Value && row[10].ToString() != string.Empty && row[9] != DBNull.Value && row[9].ToString() != string.Empty)
                {
                    row[10] = Convert.ToDouble(row[10]) - Convert.ToDouble(row[9]);
                }
                else if (row[10] == DBNull.Value && row[10].ToString() == string.Empty && row[9] != DBNull.Value && row[9].ToString() != string.Empty )
                {
                    row[10] = -Convert.ToDouble(row[9]);
                }

                if (row[9] != DBNull.Value && row[nColumn] != DBNull.Value && row[9].ToString()!= string.Empty && row[nColumn].ToString() != string.Empty)
                {
                    row[11] = Convert.ToDouble(row[9]) / Convert.ToDouble(row[nColumn]);
                }

                for (int colNum = 12; colNum < 16; colNum++)
                {
                    double sum = 0;
                     for (int rowNum = 0; rowNum<dtGrid.Rows.Count; rowNum++)
                     {
                         if (dtGrid.Rows[rowNum][colNum] != DBNull.Value && dtGrid.Rows[rowNum][colNum].ToString() != string.Empty)
                         {
                             if (dtGrid.Rows[rowNum][dtGrid.Columns.Count - 1].ToString() == "1")
                             {
                                 sum = sum + Convert.ToDouble(dtGrid.Rows[rowNum][colNum]);
                             }
                         }
                     }

                    if (sum != 0)
                    {
                        row[colNum] = sum ;
                    }
                    else
                    {
                        row[colNum] = DBNull.Value;
                    }
                }

                if (row[tempCh] != DBNull.Value && row[tempZn] != DBNull.Value && row[tempCh].ToString() != string.Empty && row[tempZn].ToString() != string.Empty)
                {
                    row[16] = Convert.ToDouble(row[tempCh]) / Convert.ToDouble(row[tempZn]);
                }

                dtGrid.Rows.Add(row);

                dtGrid.AcceptChanges();
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
            
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count-1;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            
            for (int i = 1; i < columnCount; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;

                string formatString = GetColumnFormat(columnCaption);
                int widthColumn = GetColumnWidth(columnCaption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            int indexRB1 = DebtKindButtonList1.SelectedIndex;
            int indexRB2 = DebtKindButtonList2.SelectedIndex;

            headerLayout.AddCell("Наименование");
            headerLayout.AddCell("ГРБС", "Код главного распорядителя бюджетных средств");
            headerLayout.AddCell("РзПр", "Код раздела/подраздела расходов");
            headerLayout.AddCell("ЦСР", "Код целевой статьи расходов");
            headerLayout.AddCell("ВР", "Код вида расходов");
            headerLayout.AddCell("КОСГУ", "Код классификации операций сектора государственного управления");
            headerLayout.AddCell("План, утвержденный законодательно", "План, утвержденный Законом НСО «Об областном бюджете НСО» на год");
            headerLayout.AddCell("Уточненный план на год", "Уточненный план на год, включая все уведомления по текущий квартал");
            headerLayout.AddCell("Уточненный план на квартал", "Уточненный план на квартал");
            headerLayout.AddCell(string.Format("Кассовое исполнение на {0:dd.MM.yyyy}", currentDate), string.Format("Кассовое исполнение на {0:dd.MM.yyyy}", currentDate));
            headerLayout.AddCell("Остаток ассигнований", indexRB1 == 0 ? "Остаток плана, утвержденного законом НСО «Об областном бюджете» на год»" : indexRB1 == 1 ? "Остаток уточненного плана на год" : "Остаток уточненного плана на конец квартала");
            headerLayout.AddCell("% исполнения", indexRB1 == 0 ? "Процент исполнения плана, утвержденного законом НСО «Об областном бюджете» на год»" : indexRB1 == 1 ? "Процент исполнения уточненного плана на год" : "Процент исполнения уточненного плана на конец квартала");
            headerLayout.AddCell("План, утвержденный законодательно на аналогичный период предыдущего года", "План, утвержденный Законом НСО «Об областном бюджете НСО» на предыдущий год");
            headerLayout.AddCell("Уточненный план на предыдущий год", "Уточненный план на предыдущий год, включая все уведомления по текущий квартал");
            headerLayout.AddCell("Уточненный план на аналогичный квартал предыдущего года", "Уточненный план на аналогичный квартал предыдущего года");

            int lastYear = Convert.ToInt32(ComboYear.SelectedValue) - 1;
            int finDay = CRHelper.MonthLastDay(currentMonth);
            currentDate = new DateTime(lastYear, currentMonth, finDay);

            headerLayout.AddCell(string.Format("Кассовое исполнение на {0:dd.MM.yyyy}", currentDate), string.Format("Кассовое исполнение на {0:dd.MM.yyyy}", currentDate));
            headerLayout.AddCell(indexRB2 == 0 ? "Темп роста утвержденного плана на год" : indexRB2 == 1 ? "Темп роста уточненного плана на год" : indexRB2 == 2 ? "Темп роста уточненного плана на конец квартала" : "Темп роста кассового исполнения", indexRB2 == 0 ? "Темп роста плана, утвержденного законодательно к предыдущему году" : indexRB2 == 1 ? "Темп роста уточненного плана на год к предыдущему году" : indexRB2 == 2 ? " Темп роста уточненного плана на конец квартала к плану на аналогичный период предыдущего года" : "Темп роста кассового исполнения к исполнению за аналогичный период предыдущего года");
            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("%") || columnName.ToLower().Contains("темп роста"))
            {
                return "P2";
            }
            if (columnName.Contains("РзПр"))
            {
                return "00 00";
            }
            if (columnName.Contains("ЦСР"))
            {
                return "000 00 00";
            }
            if (columnName.Contains("ВР") || columnName.Contains("ГРБС"))
            {
                return "000";
            }
            
            if ( columnName.Contains("КОСГУ"))
            {
                return "";
            }

            return "N2";
        }

        private static int GetColumnWidth(string columnName)
        {
            if (columnName.ToLower().Contains("%") || columnName.ToLower().Contains("темп роста"))
            {
                return 100;
            }
            if (columnName.Contains("РзПр") || columnName.Contains("ЦСР") || columnName.Contains("ВР") || columnName.Contains("ГРБС") || columnName.Contains("КОСГУ"))
            {
                return 55;
            }
            return 100;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
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

            if (e.Row.Cells[0].Value.ToString() == "Итого")
            {
                e.Row.Style.Font.Bold = true;
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