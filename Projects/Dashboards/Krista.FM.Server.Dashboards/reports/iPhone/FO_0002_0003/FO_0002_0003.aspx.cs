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
    public partial class FO_0002_0003 : CustomReportPage
    {
        DataTable dt = new DataTable();

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
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
            
            #region Инициализация параметров запроса

            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
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

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0003_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));

            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = CRHelper.ToUpperFirstSymbol(RegionSettingsHelper.Instance.IncomesKD30000000000000000);

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


            Label1.Text = string.Format("Темп роста фактических доходов за {0}&nbsp;{3}&nbsp;{1}&nbsp;года к аналогичному периоду предыдущего года по {2}",
                                        monthNum,
                                        yearNum,
                                        RegionSettingsHelper.Instance.ShortName,
                                        monthStr);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt);

            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[0].ToString().TrimEnd(' ');
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    if (row[i] != DBNull.Value && row[i].ToString() != string.Empty)
                    {
                        row[i] = Convert.ToDouble(row[i])*100;
                    }
                }
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 3)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = "Доля в дох.";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 1;
                ch.RowLayoutColumnInfo.SpanX = 1;
                ch.Style.Font.Size = FontUnit.Parse("14px");
                ch.Style.Padding.Top = 1;
                ch.Style.Padding.Bottom = 1;
                ch.Style.Height = 5;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
                
                ch = new ColumnHeader(true);
                ch.Caption = "Темп роста % <br />к прошлому";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Font.Size = FontUnit.Parse("14px");
                ch.Style.Padding.Top = 1;
                ch.Style.Padding.Bottom = 1;
                ch.Style.Height = 5;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
                
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Header.Style.Padding.Top = 1;

                e.Layout.Bands[0].Columns[1].Header.Caption = "%";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[2].Header.Caption = "году";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[3].Header.Caption = "месяцу";
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 3;

                e.Layout.Bands[0].HeaderStyle.Height = 5;
                for (int i = 1; i <= 3; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");

                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                    e.Layout.Bands[0].Columns[0].CellStyle.Wrap = false;
                }

                e.Layout.Bands[0].Columns[0].Width = 105;
                e.Layout.Bands[0].Columns[1].Width = 69;
                e.Layout.Bands[0].Columns[2].Width = 69;
                e.Layout.Bands[0].Columns[3].Width = 69;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool cornerEvenness = (i == 2 || i == 3);

                if (cornerEvenness && e.Row.Cells[i] != null && e.Row.Cells[i].Value != null)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top";
                    }
                    else
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top";
                    }
                }

                int index = e.Row.Index;
                if (i == 0 && (index == 1 || index == 2 || index == 3))
                {
                    if (index != 2)
                    {
                        e.Row.Cells[i].Value = e.Row.Cells[i].Value.ToString().ToLower();
                    }
                    e.Row.Cells[i].Style.Padding.Left = 20;
                }
                else
                {
                    switch (i)
                    {
                        case 2 :
                        case 3:
                            {
                                e.Row.Cells[i].Style.Padding.Right = 8;
                                break;
                            }
                        default :
                            {
                                e.Row.Cells[i].Style.Padding.Right = 4;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }
                e.Row.Cells[i].Style.Padding.Top = 1;
                e.Row.Cells[i].Style.Padding.Bottom = 1;
            }
        }

        #endregion
    }
}
