using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0005_0001Gub
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        private int firstYear = 2005;
        private int endYear = 2011;
        private int currentYear;
        private int currentQuarter;

        private GridHeaderLayout headerLayout;

        #region Параметры запроса

        // Выбранный вид дохода
        private CustomParam selectedKD;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 220);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Инициализация параметров запроса

            if (selectedKD == null)
            {
                selectedKD = UserParams.CustomParam("selected_kd");
            }

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0005_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter =dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                Collection<string> quarters = new Collection<string>();
                quarters.Add("3 месяца");
                quarters.Add("6 месяцев");
                quarters.Add("9 месяцев");
                quarters.Add("год");

                ComboQuarter.Title = "Отчетный период";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillValues(quarters);
                ComboQuarter.SetСheckedState(GetParamName(baseQuarter), true);

                Collection<string> incomes = new Collection<string>();
                incomes.Add("Налог на прибыль организаций ");
                incomes.Add("Налог на доходы физических лиц ");
                incomes.Add("Налог на имущество организаций ");

                ComboKD.Visible = true;
                ComboKD.Title = "Вид дохода";
                ComboKD.Width = 350;
                ComboKD.MultiSelect = false;
                ComboKD.FillValues(incomes);
                ComboKD.SetСheckedState("Налог на прибыль организаций", true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            currentQuarter = ComboQuarter.SelectedIndex + 1;

            Page.Title = "Поступление по основным видам налоговых доходов в местный бюджет в разрезе видов деятельности";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Поступление {3} за {0} в сравнении с аналогичными периодами {2} - {1} годов",
                GetQuarterCaption(currentQuarter, currentYear), currentYear - 1, currentYear - 2,
                ComboKD.SelectedValue.Replace("Налог", "налога").TrimEnd(' '));

            UserParams.PeriodYear.Value = currentYear.ToString();
            UserParams.PeriodLastYear.Value = (currentYear - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(currentQuarter));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", currentQuarter);
            UserParams.PeriodMonth.Value = GetLastQuarterMonth(UserParams.PeriodQuater.Value);

            selectedKD.Value = ComboKD.SelectedValue;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        private static string GetParamName(string quarterName)
        {
            switch (quarterName)
            {
                case "Квартал 1":
                    {
                        return "3 месяца";
                    }
                case "Квартал 2":
                    {
                        return "6 месяцев";
                    }
                case "Квартал 3":
                    {
                        return "9 месяцев";
                    }
                case "Квартал 4":
                    {
                        return "12 месяцев";
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        private static string GetLastQuarterMonth(string quarterName)
        {
            switch (quarterName)
            {
                case "Квартал 1":
                    {
                        return "Март";
                    }
                case "Квартал 2":
                    {
                        return "Июнь";
                    }
                case "Квартал 3":
                    {
                        return "Сентябрь";
                    }
                case "Квартал 4":
                    {
                        return "Декабрь";
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        private static string GetQuarterCaption(int quarterIndex, int year)
        {
            switch (quarterIndex)
            {
                case 1:
                    {
                        return String.Format("3 месяца {0} года", year);
                    }
                case 2:
                    {
                        return String.Format("6 месяцев {0} года", year);
                    }
                case 3:
                    {
                        return String.Format("9 месяцев {0} года", year);
                    }
                case 4:
                    {
                        return String.Format("{0} год", year);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        private static string GetQuarterHint(int quarterIndex, int year)
        {
            switch (quarterIndex)
            {
                case 1:
                    {
                        return String.Format("за 1-ый квартал {0} года", year);
                    }
                case 2:
                    {
                        return String.Format("за 1-ое полугодие {0} года", year);
                    }
                case 3:
                    {
                        return String.Format("с начала {0} года по сентябрь {0} года", year);
                    }
                case 4:
                    {
                        return String.Format("за {0} год", year);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0005_0001_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Виды экономической деятельности", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(100);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "");

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = (i == 3 || i == 5 || i == 6 || i == 8 || i == 9) ? "P1" : "N1";
                int widthColumn = formatString == "P1" ? 80 : 130;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            int year = currentYear - 2;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption, "Разделы общероссийского классификатора видов экономической деятельности");
            headerLayout.AddCell("Код раздела", "Код раздела общероссийского классификатора видов экономической деятельности");
            headerLayout.AddCell(
                String.Format("За {0}, тыс.руб.", GetQuarterCaption(currentQuarter, year)),
                String.Format("Фактическое поступление {0}, в тыс. руб", GetQuarterHint(currentQuarter, year)));
            headerLayout.AddCell("Удел.вес, %", "Удельный вес поступлений по данному виду экономической деятельности к общей сумме поступлений данного налога.");

            for (int i = 4; i < e.Layout.Bands[0].Columns.Count; i = i + 3)
            {
                year++;
                headerLayout.AddCell(
                    String.Format("За {0}, тыс.руб.", GetQuarterCaption(currentQuarter, year)),
                    String.Format("Фактическое поступление {0}, в тыс. руб", GetQuarterHint(currentQuarter, year)));
                headerLayout.AddCell("Удел.вес, %", "Удельный вес поступлений по данному виду экономической деятельности к общей сумме поступлений данного налога.");
                headerLayout.AddCell(String.Format("Темп роста {0} к {1},%", year, year - 1), "Темп роста поступлений к аналогичному периоду предыдущего года");
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool percent = (i == 5 || i == 8);
                bool rate = (i == 6 || i == 9);

                if (percent)
                {
                    int prevIndex = (i == 5) ? i - 2 : i - 3;
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[prevIndex].Value != null && e.Row.Cells[prevIndex].Value.ToString() != string.Empty)
                    {
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);
                        double prevValue = Convert.ToDouble(e.Row.Cells[prevIndex].Value);

                        if (currentValue > prevValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Наблюдается рост удельного веса относительно предыдущего года";
                        }
                        else if (currentValue < prevValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Наблюдается снижение удельного веса относительно предыдущего года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double value = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (value > 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Рост относительно предыдущего года";
                        }
                        else if (value < 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение относительно предыдущего года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() == "Все коды ОКВЭД")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

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
        }

        #endregion

        #region Экспорт

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.Export(headerLayout);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion
    }
}
