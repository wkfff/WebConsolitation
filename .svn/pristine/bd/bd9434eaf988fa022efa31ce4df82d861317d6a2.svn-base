using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0028 : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private DataTable populationDT;
        private DateTime lastDate;

        // выбранный период
        private CustomParam lastPeriod;
        // выбранный множитель рублей
        private CustomParam rubMultiplier;
        //Имя региона для выборки численности
        private CustomParam populationRegionName;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            lastPeriod = UserParams.CustomParam("last_period");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            populationRegionName = UserParams.CustomParam("population_region_name");

            #endregion

            lastDate = CubeInfoHelper.MonthReportIncomesInfo.LastDate;
            lastPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", lastDate, 4);
            UserParams.PeriodYear.Value = lastDate.Year.ToString();

            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;
            rubMultiplier.Value = "1000";
            populationRegionName.Value = "[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[Сибирский федеральный округ].[Омская область]";

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            lbPageTitle.Text = String.Format("Основные показатели исполнения {1} Омской области по состоянию на&nbsp;<span style='color:white;font-weight:bold'>{0:dd.MM.yyyy}</span>&nbsp;г., тыс.руб.",
                lastDate.AddMonths(1), CustomParam.CustomParamFactory("budget_level").Value == "Конс.бюджет субъекта" ? "консолидированного бюджета" : "собственного бюджета");

            OutcomesGrid.Bands.Clear();
            OutcomesGrid.DataBind();
        }

        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0028_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0028_grid_population");
                populationDT = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", populationDT);

                dtGrid = MergeDataTables(dtGrid, populationDT);

                OutcomesGrid.DataSource = dtGrid;
            }
        }

        private static DataTable MergeDataTables(DataTable dt1, DataTable dt2)
        {
            DataTable newDT = dt1.Copy();
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = newDT.NewRow();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    newRow[i] = row[i];
                }
                newDT.Rows.Add(newRow);
            }
            newDT.AcceptChanges();

            return newDT;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;
            
            e.Layout.Bands[0].Columns[0].Width = 210;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 130;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");

            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");

            e.Layout.Bands[0].Columns[3].Width = 140;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P1");

            e.Layout.Bands[0].Columns[4].Width = 140;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P1");

            e.Layout.Bands[0].Columns[5].Hidden = true;

            GridHeaderLayout headerLayout = new GridHeaderLayout(e.Layout.Grid);
            headerLayout.AddCell("Наименование показателя");
            headerLayout.AddCell(String.Format("Уточненный план на {0} год", lastDate.Year));
            headerLayout.AddCell(String.Format("Исполнение с начала года на {0:dd.MM.yyy} г.", lastDate.AddMonths(1)));
            headerLayout.AddCell(String.Format("Темп роста уточненного плана на {0} г. к исполнению {1} г.", lastDate.Year, lastDate.Year - 1));
            headerLayout.AddCell(String.Format("Темп роста исполнения с начала года на {0:dd.MM.yyy} г.", lastDate.AddMonths(1)));
            headerLayout.ApplyHeaderInfo();
        }

        private static bool IsInvertIndication(string indicatorName)
        {
            switch (indicatorName)
            {
                case "услуги связи (КОСГУ 221)":
                case "транспортные услуги (КОСГУ 222)":
                case "арендная плата за пользование имуществом (КОСГУ 224)":
                case "2.3 Расходы на прочие нужды":
                case "Работы, услуги по содержанию имущества (КОСГУ 225)":
                case "Прочие работы и услуги (КОСГУ 226)":
                case "Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)":
                case
                    "Безвозмездные перечисления организациям, за исключением государственных и муниципальных предприятий (КОСГУ 242)"
                    :
                case "Прочие расходы (КОСГУ 290)":
                case "3 Расходы":
                case "3.2 Другие расходы (за искл. групп 1, 2 и 3.1)":
                case "ИТОГО РАСХОДОВ":
                case "Итого расходов без учета доходов за счет безвозмездных поступлений":
                case "Расходы за исключением группы 3":
                case
                    "Превышение расходов (за исключением группы 3) над доходами (без учета безвозмездных перечислений на капитальные вложения)"
                    :
                case "3.2 Кредиты из федерального бюджета":
                case "3.3 Кредиты коммерческих банко":
                case "Привлечение кредитов":
                case "3.4 Иные источники":
                case "ИТОГО ИСТОЧНИКОВ ФИНАНСИРОВАНИЯ":
                case
                    "Превышение расходов (за исключением группы 3) над доходами (без учета безвозмездных перечислений на капитальные вложения) с учетом источников финансирования дефицита бюджета"
                    :
                case "Превышение расходов над доходами с учетом источников финансирования дефицита бюджета":
                case "IV Просроченная кредиторская задолженность - всего":
                case "По заработной плате":
                case "По начислениям на оплату труда":
                case "По оплате коммунальных услуг":
                case "По обеспечению мер социальной поддержки отдельных категорий граждан":
                case "По расходам  на обязательное медицинское страхование неработающего населения":
                case "5.1 Объем государственного долга":
                case "5.2 Численность населения, тыс.чел.":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        private static bool IsGroupRow(string rowName)
        {
            rowName = rowName.TrimEnd(' ');
            return (rowName == "I. Доходы" || rowName == "II. Расходы" ||
                    rowName == "III Источники финансирования дефицита бюджета" || rowName == "V.Справочно." ||
                    rowName.Contains("из них"));
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString().Replace("_", String.Empty);
                e.Row.Cells[0].Value = indicatorName;
            }

            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));

            string level = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex] != null)
            {
                level = e.Row.Cells[levelColumnIndex].ToString();
            }

            e.Row.Style.Padding.Right = 5;

            if (IsGroupRow(indicatorName))
            {
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count - 1;
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnCaption = e.Row.Band.Columns[i].Header.Caption.ToLower();

                bool rate = columnCaption.Contains("темп роста");

                switch (level)
                {
                    case "0":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 14;
                            break;
                        }
                    case "1":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Italic = true;
                            e.Row.Cells[i].Style.Font.Size = 12;
                            break;
                        }
                    case "2":
                        {
                            e.Row.Cells[i].Style.Font.Bold = false;
                            e.Row.Cells[i].Style.Font.Italic = true;
                            e.Row.Cells[i].Style.Font.Size = 12;
                            break;
                        }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue > 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                       ? "~/images/arrowRedUpBB.png"
                                                                       : "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Наблюдается рост исполнения относительно предыдущего года";
                        }
                        else if (currentValue < 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                       ? "~/images/arrowGreenDownBB.png"
                                                                       : "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Наблюдается снижение исполнения относительно предыдущего года";
                        }
                    }
                    e.Row.Cells[i].Style.Padding.Left = 10;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px;";
                }
            }
        }
    }

    #endregion
}
