using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Components;
using Infragistics.UltraChart.Resources.Appearance;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0018
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private CustomParam kdincomes;
        private CustomParam adm;
        private GridHeaderLayout headerLayout;
        private DateTime curdate;
        private CustomParam day;
        private static MemberAttributesDigest admDigest;
        private static MemberAttributesDigest kdDigest;
        private CustomParam LastMonth;
        private CustomParam LastQuart;
        private CustomParam LastHalf;
        private CustomParam LastYear;
        private CustomParam pastmonth;
        private CustomParam kd;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 230);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7 );

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            if (kdincomes == null)
            {
                kdincomes = UserParams.CustomParam("kdincomes");
            }
            if (adm == null)
            {
                adm = UserParams.CustomParam("adm");
            }
            if (day == null)
            {
                day = UserParams.CustomParam("day");
            }
            if (kd == null)
            {
                kd = UserParams.CustomParam("kd");
            }
            if (pastmonth == null)
            {
                pastmonth = UserParams.CustomParam("pastmonth");
            }
            if (LastYear == null)
            {
                LastYear = UserParams.CustomParam("LastYear");
            }
            if (LastHalf == null)
            {
                LastHalf = UserParams.CustomParam("LastHalf");
            }
            if (LastQuart == null)
            {
                LastQuart = UserParams.CustomParam("LastQuart");
            }
            if (LastMonth == null)
            {
                LastMonth = UserParams.CustomParam("LastMonth");
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0018_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();
                day.Value = dtDate.Rows[0][4].ToString();
                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0]),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                           Convert.ToInt32(dtDate.Rows[0][4]));
                CustomCalendar1.WebCalendar.SelectedDate = date;

            }
            if (!Page.IsPostBack)
            {
                ComboAdm.Width = 400;
                ComboAdm.Title = "Администраторы";
                ComboAdm.MultiSelect = true;
                ComboAdm.ParentSelect = true;
                admDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0001_0018_adm");
                ComboAdm.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(admDigest.UniqueNames, admDigest.MemberLevels));
                ComboAdm.SetСheckedState("Все администраторы ", true);

                ComboKD.Width = 400;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.TooltipVisibility = TooltipVisibilityMode.Shown;
                kdDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0001_0018_KD");
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(kdDigest.UniqueNames, kdDigest.MemberLevels));
                ComboKD.SetСheckedState("НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ", true);
            }
            int yearNum = Convert.ToInt32(CustomCalendar1.WebCalendar.SelectedDate.Year);
            Page.Title = string.Format("Анализ доходов областного бюджета в разрезе администраторов: {0}", ComboKD.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Информация приводится по разделу 1 бюджетной классификации доходов по состоянию на {0:dd.MM.yyyy}г. (включительно), тыс.руб.",
                 CustomCalendar1.WebCalendar.SelectedDate);
            curdate = CustomCalendar1.WebCalendar.SelectedDate;
            DateTime lastdate = CustomCalendar1.WebCalendar.SelectedDate;
            CustomCalendar1.WebCalendar.Layout.DropDownYearsNumber = 0;
            pastmonth.Value = String.Format("{0}", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            int monthNum = CRHelper.MonthNum(CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodMonth.Value = String.Format("{0}", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodYear.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Year.ToString());
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodEndYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            day.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Day.ToString());
            lastdate = lastdate.AddMonths(-1);
            LastMonth.Value = CRHelper.RusMonth(lastdate.Month);
            LastHalf.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(lastdate.Month));
            LastQuart.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(lastdate.Month));
            LastYear.Value = lastdate.Year.ToString();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            string patternValue = UserParams.StateArea.Value;
            kd.Value = kdDigest.UniqueNames[ComboKD.SelectedValue];
            Collection<string> selectedValues = ComboAdm.SelectedValues;
            if (selectedValues.Count > 0)
            {


                string admin = String.Empty;
                if ((selectedValues[0] == "Все администраторы ") && (selectedValues.Count == 1))
                {
                    adm.Value = "[Все Администраторы]";
                }
                else
                {
                    for (int i = 0; i < selectedValues.Count; i++)
                    {
                        string admins = selectedValues[i];
                        admin +=
                            string.Format(
                                "[Администратор].[Сопоставим].[Все администраторы].[{0}], ",
                                admins);

                    }
                    admin = admin.Remove(admin.Length - 2, 1);
                    adm.Value = string.Format("{0}", admin);
                }
            }
            else
            {
                adm.Value = "{}";
            }
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            if (!Page.IsPostBack)
            {
                int defaultRowIndex = 1;
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
                ActiveGridRow(row);
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = string.Empty;
            if (ComboKD.SelectedValue == "НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ")
            {
                query = DataProvider.GetQueryText("FO_0001_0018_grid");
            }
            else
            {
                query = DataProvider.GetQueryText("FO_0001_0018_grid_light");
            }
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "КД", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                dtGrid.Rows.RemoveAt(0);
               
            }
            dtGrid.AcceptChanges();
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {

            ActiveGridRow(e.Row);
        }
        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {

            
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].Columns.RemoveAt(0);
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            string Date1 = string.Format("{0:MM.yyyy}",
                     curdate);
            string Date2 = string.Format("{0:dd.MM.yyyy}",
                     curdate);
            string Date3 = string.Format("{0:dd.MM.yyyy}",
                     curdate.AddYears(-1));
            headerLayout.AddCell("Доходы", "").AddCell("1"); ;
            headerLayout.AddCell("Код", "").AddCell("2"); ;
            string header = string.Empty;
            header = string.Format("{0} мес.", curdate.Month - 1);
            if (curdate.Month == 1)
            {
                headerLayout.AddCell(String.Format("Исполнено", header, curdate.Year), String.Format("Исполнено", header, curdate.Year)).AddCell("3");
                headerLayout.AddCell(String.Format("Исполнено", header, curdate.Year), String.Format("Исполнено", header, curdate.Year)).AddCell("4");
            }
            else
            {
                headerLayout.AddCell(String.Format("Исполнено за {0} {1} г. (по данным ежемесячной отчетности)", header, curdate.Year), String.Format("Исполнено за {0} {1} г. (нарастающим итогом по данным ежемесячной отчетности)", header, curdate.Year)).AddCell("3");
                headerLayout.AddCell(String.Format("Исполнено за {0} {1} г. (по данным АС «Бюджет»)", header, curdate.Year), String.Format("Исполнено за {0} {1} г. (нарастающим итогом по данным АС «Бюджет»)", header, curdate.Year)).AddCell("4");
            }
            headerLayout.AddCell(String.Format("Исполнено за месяц на {0} г.", Date2), String.Format("Исполнено за месяц на {0} г.", Date2)).AddCell("5");
            headerLayout.AddCell(String.Format("Исполнено за год на {0} г.", Date2), String.Format("Исполнено за год на {0} г. (нарастающим итогом)", Date2)).AddCell("6=3(4)+5");
            headerLayout.AddCell(String.Format("Исполнено прошлый год (по данным АС «Бюджет»)"), String.Format("Исполнено за прошлый год всего (по данным АС «Бюджет»)")).AddCell("7");
            headerLayout.AddCell(String.Format("Исполнено за год на {0} г.", Date3), String.Format("Исполнено за год на {0} г. (нарастающим итогом)", Date3)).AddCell("8"); 
            headerLayout.AddCell("Темп роста", "Темп роста (снижения) сумм фактических поступлений текущего года к аналогичному периоду прошлого года").AddCell("9=6/8"); ;
            headerLayout.ApplyHeaderInfo();
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0) ?
                                                                         HorizontalAlign.Left :
                                                                         HorizontalAlign.Right;
                double width;
                switch (i)
                {
                    case 0:
                        {
                            width = 270;
                            break;
                        }
                    case 1:
                        {
                            width = 124;
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "0");
                            e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                            e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                            break;
                        }
                    case 8:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                            width = 104;
                            break;
                        }
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    default:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            width = 104;
                            e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width);
            }
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(95);

        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if ((i == e.Row.Cells.Count - 2)  && e.Row.Cells[i].Value != DBNull.Value)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";
                    }
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100 && Convert.ToDouble(e.Row.Cells[i].Value) != 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                    }
                }
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
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
                if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "(All)":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Подстатья":
                            {
                                fontSize = 8;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "Статья":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                    if (e.Row.Cells[0].Value.ToString().Contains("за исключением"))
                    {
                        e.Row.Cells[i].Style.Font.Italic = true;
                    }
                    if (e.Row.Cells[0].Value.ToString().Contains("ИТОГО ДОХОДОВ"))
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Экспорт

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }
        #endregion

    }
}
