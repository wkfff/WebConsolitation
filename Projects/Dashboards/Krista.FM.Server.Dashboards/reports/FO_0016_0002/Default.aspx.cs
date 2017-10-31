using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtGauges;
        private DataTable dtDate;

        private int firstYear = 2000;
        private int endYear = 2011;
        private const int gaugeWidth = 450;
        private int gaugeRowCount = 2;

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double pageWidth = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth).Value;

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }

            #endregion

            // число гэйджей в строке
            gaugeRowCount = Convert.ToInt32(pageWidth) / gaugeWidth;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter = dtDate.Rows[0][2].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodQuater.Value = baseQuarter;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 170;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.SetСheckedState(baseQuarter, true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "Территории";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillSettlements(RegionsNamingHelper.LocalSettlementTypes, false));
                ComboRegion.SetСheckedState("Баганский район", true);
            }

            Page.Title = "Результаты мониторинга БК и КУ в разрезе показателей";
            Label1.Text = string.Format("{0} ({1})", Page.Title, ComboRegion.SelectedValue);

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            Label2.Text = string.Format("за {0} квартал {1} года", quarterNum, yearNum);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", quarterNum);
            selectedRegion.Value = RegionsNamingHelper.LocalSettlementUniqueNames[ComboRegion.SelectedValue];

            GaugesDataBind();
        }

        /// <summary>
        /// Получение непустого значения в виде строки из таблицы
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns>значение в виде строки, либо пустая строка</returns>
        private static string GetNonDBNullValue(object value)
        {
            return (value != DBNull.Value) ? value.ToString() : string.Empty;
        }

        private void GaugesDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0016_0002_indiacators");
            dtGauges = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Индикатор", dtGauges);

            GaugesTD.Controls.Clear();
            // создаем таблицу для гэйджев
            Table table = new Table();
            TableRow currentRow = new TableRow();

            for (int i = 0; i < dtGauges.Rows.Count; i++)
            {
                if (i % gaugeRowCount == 0)
                {
                    // переходим на новую строку
                    currentRow = new TableRow();
                    table.Rows.Add(currentRow);
                }

                // создаем ячейку в текущей строке
                TableCell cell = new TableCell();
                currentRow.Cells.Add(cell);

                DataRow row = dtGauges.Rows[i];
                Control indControl = Page.LoadControl("~/components/GaugeIndicator.ascx");
                GaugeIndicator gauge = (GaugeIndicator)indControl;
                gauge.GaugeImageURL = string.Format("../../TemporaryImages/gaugeIndicator_{0}_#SEQNUM(500).png", i);
                gauge.Width = gaugeWidth.ToString();
                gauge.Height = "300";

                gauge.FactValue = GetNonDBNullValue(row["Значение"]);

                int rank = row["Ранг по значению"] != DBNull.Value ? Convert.ToInt32(row["Ранг по значению"]) : 0;
                int badRank = row["Худший ранг по значению"] != DBNull.Value ? Convert.ToInt32(row["Худший ранг по значению"]) : -1;
                string imgRank = string.Empty;
                if (rank == 1)
                {
                    imgRank = "<img src='../../images/starYellowBB.png'>";
                }
                else if (rank == badRank)
                {
                    imgRank = "<img src='../../images/starGrayBB.png'>";
                }

                gauge.RankValue = string.Format("{0}&nbsp;{1}", (rank == 0) ? string.Empty : rank.ToString(), imgRank);
                gauge.MinValue = GetNonDBNullValue(row["Минимальное значение по районам"]);
                gauge.MaxValue = GetNonDBNullValue(row["Максимальное значение по районам"]);

                gauge.Title = GetNonDBNullValue(row["Индикатор"]);
                gauge.Name = GetNonDBNullValue(row["Наименование"]);
                gauge.Content = GetNonDBNullValue(row["Содержание"]);
                gauge.Formula = GetNonDBNullValue(row["Формула"]);
                gauge.NormalValue = string.Format("{0}{1}", GetNonDBNullValue(row["Условия проверки"]), GetNonDBNullValue(row["Пороговое значение"]));

                if (row["Значение"] != DBNull.Value)
                {
                    double indValue = Convert.ToDouble(row["Значение"]);
                    double limitValue = row["Пороговое значение"] != DBNull.Value
                                            ? Convert.ToInt32(row["Пороговое значение"])
                                            : 0;

                    bool isFailure = row["Количество нарушений"] != DBNull.Value ? (Convert.ToInt32(row["Количество нарушений"]) == 1) : false;
                    gauge.BEStartColor = isFailure ? Color.Red : Color.LimeGreen;
                    gauge.BEEndColor = isFailure ? Color.Maroon : Color.DarkGreen;
                    gauge.ToolTip = indValue.ToString();

                    gauge.LimitValue = limitValue;
                    gauge.MarkerValue = indValue;
                    double minIndValue = row["Минимальное значение"] != DBNull.Value ? Convert.ToDouble(row["Минимальное значение"]) : 0;
                    double maxIndValue = row["Максимальное значение"] != DBNull.Value ? Convert.ToDouble(row["Максимальное значение"]) : 0;
                    gauge.AxisMin = Math.Min(Math.Min(minIndValue, indValue - 0.1), limitValue);
                    gauge.AxisMax = Math.Max(Math.Max(maxIndValue, indValue + 0.1), limitValue);
                    gauge.AxisTickInterval = (gauge.AxisMax - gauge.AxisMin) / 3;
                }
                else
                {
                    gauge.SetVisibleScales(false);
                    gauge.BEStartColor = Color.Transparent;
                    gauge.BEEndColor = Color.Transparent;
                    gauge.ToolTip = string.Empty;
                }

                cell.Controls.Add(gauge);
            }
            GaugesTD.Controls.Add(table);
        }
    }
}
