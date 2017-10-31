using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0013 : CustomReportPage
    {
        private DataTable dt;
        private DataTable newDt;

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // расходы Итого
        private CustomParam outcomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            #region Инициализация параметров запроса

            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (outcomesTotal == null)
            {
                outcomesTotal = UserParams.CustomParam("outcomes_total");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0013_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;

            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            Label1.Text = string.Format("Конс.бюджет {0} за {1} {2} {3}г",
                                        RegionSettingsHelper.Instance.ShortName,
                                        monthNum,
                                        CRHelper.RusManyMonthGenitive(monthNum),
                                        yearNum);
            
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            // создаем новую таблицу
            newDt = new DataTable();
            newDt.Columns.Add("column1", typeof (string));
            newDt.Columns.Add("column2", typeof (string));

            string query = DataProvider.GetQueryText("FO_0002_0013");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            // Доходы Всего
            double incomesExecute = GetDoubleDTValue(dt, "ДОХОДЫ ВСЕГО ; % выполнения");
            double incomesRate = GetDoubleDTValue(dt, "ДОХОДЫ ВСЕГО ; Темп роста");
            string incomesRateStr = incomesRate != double.MinValue ? 
                string.Format(",темп роста <span style=\"color:white;\">{0:P2}</span> ", incomesRate) : string.Empty;
            string incomesRateArrow = incomesRate == double.MinValue ? string.Empty :
                 incomesRate > 1
                 ? "<img src=\"../../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\">"
                 : "<img src=\"../../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\">";

            // Группы доходов
            double incomesGroup1Execute = GetDoubleDTValue(dt, "Налоговые и неналоговые доходы ; % выполнения");
            double incomesGroup2Execute = GetDoubleDTValue(dt, "Безвозмездные поступления ; % выполнения");
            
            // Расходы Всего
            double outcomesExecute = GetDoubleDTValue(dt, "РАСХОДЫ ВСЕГО ; % выполнения");
            double outcomesRate = GetDoubleDTValue(dt, "РАСХОДЫ ВСЕГО ; Темп роста");
            string outcomesRateStr = outcomesRate != double.MinValue ?
                    string.Format(",темп роста <span style=\"color:white;\">{0:P2}</span> ", outcomesRate) : string.Empty;
            string outcomesRateArrow = outcomesRate == double.MinValue ? string.Empty :
                 outcomesRate > 1
                 ? "<img src=\"../../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\">"
                 : "<img src=\"../../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\">";

            // Группы расходов
            double outcomesGroup1Execute = GetDoubleDTValue(dt, "Оплата труда ; % выполнения");
            double outcomesGroup2Execute = GetDoubleDTValue(dt, "Капитальные вложения ; % выполнения");
            double outcomesGroup3Execute = GetDoubleDTValue(dt, "Материальные затраты ; % выполнения");


            // Дефицит/профицит
            double deficitFact = GetDoubleDTValue(dt, "ДЕФИЦИТ/ПРОФИЦИТ; % выполнения");

            DataRow row1 = newDt.NewRow();
            row1[0] = incomesExecute != double.MinValue ?
                       string.Format("Доходы: <br/>&nbsp;&nbsp;&nbsp;исп. <span style=\"color:white;\">{0:P2}</span>{1} {2}",
                                      incomesExecute, incomesRateStr, incomesRateArrow) : string.Empty;
            row1[1] = DBNull.Value;
            newDt.Rows.Add(row1);

            DataRow row2 = newDt.NewRow();
            row2[0] = "в&nbsp;т.ч.&nbsp;налог. и неналог.:";
            row2[1] = incomesGroup1Execute != double.MinValue ? string.Format("<span style=\"color:white;\">{0:P2}</span>", incomesGroup1Execute) : string.Empty;
            newDt.Rows.Add(row2);

            DataRow row3 = newDt.NewRow();
            row3[0] = "<span style=\"padding-left:40px;\">безвозмездные:</span>";
            row3[1] = incomesGroup2Execute != double.MinValue ? string.Format("<span style=\"color:white;\">{0:P2}</span>", incomesGroup2Execute) : string.Empty;
            newDt.Rows.Add(row3);

            DataRow row4 = newDt.NewRow();
            row4[0] = outcomesExecute != double.MinValue ?
                       string.Format("Расходы: <br/>&nbsp;&nbsp;&nbsp;исп. <span style=\"color:white;\">{0:P2}</span>{1} {2}",
                       outcomesExecute, outcomesRateStr, outcomesRateArrow) : string.Empty;
            row4[1] = DBNull.Value;
            newDt.Rows.Add(row4);

            DataRow row5 = newDt.NewRow();
            row5[0] = "в&nbsp;т.ч.&nbsp;оплата труда:";
            row5[1] = outcomesGroup1Execute != double.MinValue ? string.Format("<span style=\"color:white;\">{0:P2}</span>", outcomesGroup1Execute) : string.Empty;
            newDt.Rows.Add(row5);

            DataRow row6 = newDt.NewRow();
            row6[0] = "<span style=\"padding-left:40px;\">кап.вложения:</span>";
            row6[1] = outcomesGroup2Execute != double.MinValue ? string.Format("<span style=\"color:white;\">{0:P2}</span>", outcomesGroup2Execute) : string.Empty;
            newDt.Rows.Add(row6);

            DataRow row7 = newDt.NewRow();
            row7[0] = "<span style=\"padding-left:40px;\">мат.затраты:</span>";
            row7[1] = outcomesGroup3Execute != double.MinValue ? string.Format("<span style=\"color:white;\">{0:P2}</span>", outcomesGroup3Execute) : string.Empty;
            newDt.Rows.Add(row7);

            DataRow row8 = newDt.NewRow();
            row8[0] = deficitFact != double.MinValue ? string.Format("Дефицит/профицит: <span style=\"color:white;\">{0:N3}</span> млн.руб.", deficitFact) : string.Empty;
            row8[1] = DBNull.Value;
            newDt.Rows.Add(row8);

            UltraWebGrid.DataSource = newDt;
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, double.MinValue);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.ColHeadersVisibleDefault = ShowMarginInfo.No;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].Width = 208;
                e.Layout.Bands[0].Columns[1].Width = 100;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "P2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool spanRow = (e.Row.Index == 0 || e.Row.Index == 3 || e.Row.Index == 7);

                if (spanRow)
                {
                    e.Row.Cells[0].ColSpan = 2;
                    e.Row.Cells[0].Style.BorderDetails.WidthTop = 3;
                }

                if (e.Row.Index == 7)
                {
                    e.Row.Cells[0].Style.BorderDetails.WidthBottom = 3;
                }
            }
        }

        #endregion
    }
}
