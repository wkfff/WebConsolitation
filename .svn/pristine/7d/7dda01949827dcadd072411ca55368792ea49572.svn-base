using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0066
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private GridHeaderLayout headerLayout;
        #endregion
        private CustomParam filter_period;
        private CustomParam filter_period_analog;
        #region Параметры запроса


        #endregion

        private double rubMultiplier;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 180);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (filter_period == null)
            {
                filter_period = UserParams.CustomParam("filter_period");
            }
            if (filter_period_analog == null)
            {
                filter_period_analog = UserParams.CustomParam("filter_period_analog");
            }
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            // Получаем последнюю дату
            string query = DataProvider.GetQueryText("FO_0002_0066_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(
                query, dtDate);
            if (!Page.IsPostBack)
            {
                CustomCalendar1.Visible = true;

                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                       Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                // Инициализируем календарь
                CustomCalendar1.WebCalendar.SelectedDate = date;
            }




            UserParams.PeriodYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", CustomCalendar1.WebCalendar.SelectedDate, 1);
            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", CustomCalendar1.WebCalendar.SelectedDate, 4);
            #region выбор фильтра для периода
            DateTime January = new DateTime(Convert.ToInt32(dtDate.Rows[0][0].ToString()), 1, 1);
            DateTime May = new DateTime(Convert.ToInt32(dtDate.Rows[0][0].ToString()), 5, 31);
            string param = string.Empty;
            string paramAnalog = string.Empty;
            int Year;
            if (!Check1.Checked)
            {
                param = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]",
                                                   CustomCalendar1.WebCalendar.SelectedDate, 5);

                if ((CustomCalendar1.WebCalendar.SelectedDate > January) &&
                    (CustomCalendar1.WebCalendar.SelectedDate < May))
                {
                    Year = CustomCalendar1.WebCalendar.SelectedDate.Year;
                    filter_period.Value = string.Format(
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь].[1]:[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 4].[Декабрь].[31]," +
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{1}].[Полугодие 1].[Квартал 1].[Январь].[1]:{2}",
                            CustomCalendar1.WebCalendar.SelectedDate.Year - 1,
                            CustomCalendar1.WebCalendar.SelectedDate.Year,
                            param);
                }
                else
                {
                    Year = CustomCalendar1.WebCalendar.SelectedDate.Year + 1;
                    filter_period.Value = string.Format(
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь].[1]:{1}",
                            CustomCalendar1.WebCalendar.SelectedDate.Year, param);
                }
            }
            else
            {
                param = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]",
                                                      CustomCalendar1.WebCalendar.SelectedDate.AddMonths(-1), 4);
                paramAnalog = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]",
                                                      CustomCalendar1.WebCalendar.SelectedDate, 5);
                if ((CustomCalendar1.WebCalendar.SelectedDate > January) &&
                    (CustomCalendar1.WebCalendar.SelectedDate < May))
                {
                    Year = CustomCalendar1.WebCalendar.SelectedDate.Year;
                    filter_period_analog.Value = string.Format(
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь].[1]:[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 4].[Декабрь].[31]," +
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{1}].[Полугодие 1].[Квартал 1].[Январь].[1]:{2}",
                            CustomCalendar1.WebCalendar.SelectedDate.Year - 1,
                            CustomCalendar1.WebCalendar.SelectedDate.Year,
                            paramAnalog);
                    filter_period.Value = string.Format(
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь]:[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 4].[Декабрь]," +
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{1}].[Полугодие 1].[Квартал 1].[Январь]:{2}",
                            CustomCalendar1.WebCalendar.SelectedDate.Year - 1,
                            CustomCalendar1.WebCalendar.SelectedDate.Year,
                            param);
                }
                else
                {
                    Year = CustomCalendar1.WebCalendar.SelectedDate.Year + 1;
                    filter_period_analog.Value = string.Format(
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь].[1]:{1}",
                            CustomCalendar1.WebCalendar.SelectedDate.Year, paramAnalog);
                    filter_period.Value = string.Format(
                            "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь]:{1}",
                            CustomCalendar1.WebCalendar.SelectedDate.Year, param);
                }
            }
            #endregion
            Page.Title = string.Format("Выделение денежных средств на подготовку к отопительному сезону {0}-{1}г.г. по состоянию на {2:dd.MM.yyyy}, {3}",
                Year - 1,
                Year,
                CustomCalendar1.WebCalendar.SelectedDate,
                RubMiltiplierButtonList.SelectedValue);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("(с 01.06.{0})", Year - 1);
            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            UserParams.KDLevel.Value = rubMultiplier.ToString();
            UltraWebGrid.Bands.Clear();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = string.Empty;
            if (!Check1.Checked)
            {
                query = DataProvider.GetQueryText("FO_0002_0066_grid");
            }
            else
            {
                query = DataProvider.GetQueryText("FO_0002_0066_grid_detail");
            }
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                dtGrid.Columns.RemoveAt(0);

                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    if (dtGrid.Rows[i][1].ToString().Contains("Итого по районам"))
                    {
                        dtGrid.Rows[i][1] = "ВСЕГО ПО РАЙОНАМ";
                    }
                    if (dtGrid.Rows[i][1].ToString().Contains("Итого по городам"))
                    {
                        dtGrid.Rows[i][1] = "ВСЕГО ПО ГОРОДАМ";
                    }
                    if (dtGrid.Rows[i][1].ToString().Contains("Итого"))
                    {
                        dtGrid.Rows[i][1] = "ИТОГО ПО РАЙ-И-ГОР.";
                    }
                }
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(45);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(245);
            string formatString = IsThsRubSelected ? "N0" : "N2";
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
            }

            headerLayout.AddCell("№");
            headerLayout.AddCell("РАЙОНЫ И ГОРОДА");

            if (!Check1.Checked)
            {
                GridHeaderCell cell =
                    headerLayout.AddCell(CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
                int j = 2;
                bool flag = false;
                string[] captionStr;
                while (!flag)
                {
                    captionStr = e.Layout.Bands[0].Columns[j].Header.Caption.Split(';');
                    if (captionStr[0].ToLower() !=
                        CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month).ToLower())
                    {
                        cell.AddCell(captionStr[0] + "." + string.Format(
                            "{0:00}",
                            CustomCalendar1.WebCalendar.SelectedDate.Month));
                    }
                    if (captionStr[0].ToLower() ==
                        CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month).ToLower())
                    {
                        cell.AddCell("Итого </br>" + captionStr[0]);
                        flag = true;
                    }
                    j++;
                }
                headerLayout.AddCell(string.Format("Всего отопительный сезон</br> {0}-{1}гг.",
                                                   CustomCalendar1.WebCalendar.SelectedDate.Year - 1,
                                                   CustomCalendar1.WebCalendar.SelectedDate.Year));
                headerLayout.AddCell("в том числе уголь");
                string[] header = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Header.Caption.Split(';');
                if (header[1].Contains("кредиты"))
                {
                    headerLayout.AddCell("из них кредиты");
                }
            }
            else
            {
                headerLayout.AddCell(string.Format("Итого </br> {0}-{1}", "Июнь", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month - 1)));
                GridHeaderCell cell =
                    headerLayout.AddCell(CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
                int j = 3;
                bool flag = false;
                while (!flag)
                {
                    string[] captionStr = e.Layout.Bands[0].Columns[j].Header.Caption.Split(';');
                    if (captionStr[0].ToLower() !=
                        CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month).ToLower())
                    {
                        cell.AddCell(captionStr[0] + "." + string.Format(
                            "{0:00}",
                            CustomCalendar1.WebCalendar.SelectedDate.Month));
                    }
                    if (captionStr[0].ToLower() ==
                        CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month).ToLower())
                    {
                        cell.AddCell("Итого </br>" + captionStr[0]);
                        flag = true;
                    }
                    j++;
                }
                headerLayout.AddCell(string.Format("Всего </br> {0}-{1}", "Июнь", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month)));
                string[] header = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Header.Caption.Split(';');
                if (header[1].Contains("кредиты"))
                {
                    headerLayout.AddCell("в том числе кредиты");
                }
            }
            headerLayout.ApplyHeaderInfo();
        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[1].Value != null &&
                    e.Row.Cells[1].Value.ToString().Contains("ВСЕГО ПО РАЙОНАМ"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                if ((e.Row.Cells[1].Value != null) && (e.Row.Cells[1].Value.ToString().Contains("ВСЕГО ПО ГОРОДАМ")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                if ((e.Row.Cells[1].Value != null) && (e.Row.Cells[1].Value.ToString().Contains("ИТОГО ПО РАЙ-И-ГОР.")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                if (e.Row.Cells[0].Value != null)
                {
                    string[] code = e.Row.Cells[0].Value.ToString().Split("_");
                    e.Row.Cells[0].Value = code[0];
                }
            }
        }
        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(headerLayout, PageSubTitle.Text, section1);
        }

        #endregion
    }
}
