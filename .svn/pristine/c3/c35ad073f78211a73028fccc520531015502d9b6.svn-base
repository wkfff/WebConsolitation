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
    public partial class FO_0002_0006_h : CustomReportPage
    {
        #region Параметры запроса

        // расходы Итого
        private CustomParam outcomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;

            #region Инициализация параметров запроса

            if (outcomesTotal == null)
            {
                outcomesTotal = UserParams.CustomParam("outcomes_total");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0006_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
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
            TextBox1.Text = string.Format("Структура расходов консолидированного бюджета {0}", RegionSettingsHelper.Instance.ShortName);
            TextBox8.Text = string.Format("за {0} {1} {2} года", monthNum, monthStr, yearNum);
            TextBox2.Text = string.Format("Исп = исполнено за {0} {1} {2} года, млн. руб.", monthNum, monthStr, yearNum);
            TextBox3.Text = string.Format("% расходов = доля расхода в общей сумме расходов");
            TextBox4.Text = string.Format("% исп = процент исполнения годовых назначений");

            if (monthNum == 12)
            {
                monthNum = 1;
                yearNum++;
            }
            else
            {
                monthNum++;
            }

            Label2.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label1.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0006_h");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Расходы", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / 1000000;
                }

                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) * 100;
                }

                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) * 100;
                }
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 3)
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = "РзПр";
                e.Layout.Bands[0].Columns[1].Header.Caption = "Исп";
                e.Layout.Bands[0].Columns[2].Header.Caption = "Доля расходов %";
                e.Layout.Bands[0].Columns[3].Header.Caption = "% исп";
                
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].Width = 230;
                e.Layout.Bands[0].Columns[1].Width = 66;
                e.Layout.Bands[0].Columns[2].Width = 75;
                e.Layout.Bands[0].Columns[3].Width = 100;
                e.Layout.Bands[0].Columns[4].Hidden = true;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[4].Value != null && e.Row.Cells[4].Value.ToString() == "Раздел")
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    UltraGridCell cell = e.Row.Cells[i];
                    if (!(e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString().ToLower() == "расходы бюджета - итого"))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        #endregion
    }
}
