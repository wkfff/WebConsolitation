using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0013_H : CustomReportPage
    {
        private DataTable dt;

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // расходы Итого
        private CustomParam outcomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного бюджета
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // текущий уровень бюджета
        private CustomParam currentBudgetLevel;

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
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }

            if (currentBudgetLevel == null)
            {
                currentBudgetLevel = UserParams.CustomParam("current_budget_level");
            }

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0013_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            
            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "месяц";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "месяца";
                        break;
                    }
                default:
                    {
                        monthStr = "месяцев";
                        break;
                    }
            }

            TextBox1.Text = "Исполнение бюджета в разрезе отдельных показателей";
            TextBox2.Text = string.Format("за {0} {1} {2} года по {3}.",
                                        monthNum,
                                        monthStr,
                                        yearNum,
                                        RegionSettingsHelper.Instance.ShortName);

            if (monthNum == 12)
            {
                monthNum = 1;
                yearNum++;
            }
            else
            {
                monthNum++;
            }

            Label1.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            Label2.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);

            ConsosidateBudgetLabel.Text = "Консолидированный бюджет";
            OwnBudgetLabel.Text = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            LocalBudgetLabel.Text = "Местные бюджеты";

            UltraWebGrid1.DataBind();
            UltraWebGrid2.DataBind();
            UltraWebGrid3.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            currentBudgetLevel.Value = "Консолидированный бюджет субъекта";
            string query = DataProvider.GetQueryText("FO_0002_0013_h");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            UltraWebGrid1.DataSource = dt;
        }


        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            currentBudgetLevel.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            string query = DataProvider.GetQueryText("FO_0002_0013_h");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            UltraWebGrid2.DataSource = dt;
        }

        protected void UltraWebGrid3_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            currentBudgetLevel.Value = "Местные бюджеты";
            string query = DataProvider.GetQueryText("FO_0002_0013_h");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            UltraWebGrid3.DataSource = dt;
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 4)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[2].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[3].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[4].CellStyle.Wrap = false;

                e.Layout.Bands[0].Columns[0].Header.Caption = "Показатели";
                e.Layout.Bands[0].Columns[1].Header.Caption = "План, млн.руб.";
                e.Layout.Bands[0].Columns[2].Header.Caption = "Факт, млн.руб.";
                e.Layout.Bands[0].Columns[3].Header.Caption = "% исп.";
                e.Layout.Bands[0].Columns[4].Header.Caption = "Темп роста %";

                e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[3].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[4].Header.Style.Font.Size = FontUnit.Parse("14px");

                e.Layout.Bands[0].Columns[0].Width = 122;
                e.Layout.Bands[0].Columns[1].Width = 89;
                e.Layout.Bands[0].Columns[2].Width = 89;
                e.Layout.Bands[0].Columns[3].Width = 80;
                e.Layout.Bands[0].Columns[4].Width = 80;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            }
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
//                e.Row.Cells[3].Style.ForeColor = Color.FromArgb(209, 209, 209);
//                e.Row.Cells[5].Style.ForeColor = Color.FromArgb(209, 209, 209);
//                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
//                e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.Transparent;
//                
//                e.Row.Style.Wrap = false;
//
//                decimal res;
//                if (decimal.TryParse(e.Row.Cells[0].Value.ToString(), out res) && e.Row.Index != 0)
//                {
//                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
//                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.FromArgb(50, 50, 50);
//                    e.Row.Cells[i].Style.BorderDetails.ColorBottom = Color.Transparent;
//                    e.Row.Cells[i].Style.ForeColor = Color.White;
//                    e.Row.Cells[i].Style.Font.Bold = true;
//                }
//                else
//                {
//                    if (e.Row.Index == 0)
//                    {
//                        e.Row.Cells[i].Style.BorderDetails.ColorBottom = Color.Transparent;
//                        e.Row.Cells[i].Style.ForeColor = Color.White;
//                        e.Row.Cells[i].Style.Font.Bold = true;
//                    }
//                }
//
//                if (e.Row.Index == dt.Rows.Count - 1)
//                {
//                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
//                }
            }
        }
        #endregion
    }
}
