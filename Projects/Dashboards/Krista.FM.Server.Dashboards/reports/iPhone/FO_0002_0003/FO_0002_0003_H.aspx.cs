using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0003_H : CustomReportPage
    {
        private DataTable dt;

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // группа КД
        private CustomParam kdGroupName;
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
            if (kdGroupName == null)
            {
                kdGroupName = UserParams.CustomParam("kd_group_name");
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

            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");

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

            TextBox1.Text = String.Format("Динамика по налоговым доходам по конс.бюджету {0} ", RegionSettingsHelper.Instance.ShortName);
            TextBox2.Text = string.Format("за {0}-{1} год и темп роста к аналогичному периоду", UserParams.PeriodLastYear.Value, UserParams.PeriodYear.Value);
            TextBox3.Text = string.Format("прошлого года:");
            TextBox4.Text = string.Format("Динамика по НДФЛ по конс.бюджету {0}",
                       RegionSettingsHelper.Instance.ShortName);
            TextBox5.Text = string.Format("за {0}-{1} год и темп роста к аналогичному периоду", UserParams.PeriodLastYear.Value,
                       UserParams.PeriodYear.Value);
            TextBox18.Text = string.Format("прошлого года:");

            TextBox6.Text = "Динамика по налогу на прибыль организаций по конс.бюджету";
            TextBox7.Text = string.Format("{2} за {0}-{1} год и темп роста к аналогичному",
                       UserParams.PeriodLastYear.Value,
                       UserParams.PeriodYear.Value,
                       RegionSettingsHelper.Instance.ShortName);
            TextBox8.Text = string.Format("периоду прошлого года:");

            TextBox15.Text = string.Format("Динамика общей суммы доходов по конс.бюджету {0}", RegionSettingsHelper.Instance.ShortName);
            TextBox16.Text = string.Format("за {0}-{1} год и темп роста к аналогичному периоду",
                       UserParams.PeriodLastYear.Value,
                       UserParams.PeriodYear.Value);
            TextBox17.Text = string.Format("прошлого года:");

            TextBox9.Text =  "За месяц млн.руб. = поступления за месяц в конс.бюджет субъекта";
            TextBox10.Text = "Назначено млн.руб. = план поступлений на год";
            TextBox11.Text = "Исполнено млн.руб. = поступления нарастающим итогом с начала года";
            TextBox12.Text = "Исполн % = процент выполнения назначений (плана)";
            TextBox13.Text = "Темп роста % = темп роста исполнения нарастающим итогом";
            TextBox14.Text = "к аналогичному периоду предыдущего года";

            UltraWebGrid1.DataBind();
            UltraWebGrid2.DataBind();
            UltraWebGrid3.DataBind();
            UltraWebGrid4.DataBind();
        }

        #region Обработчики грида

        private void FormatDt()
        {
            List<int> inserts = new List<int>();

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                DataRow row = dt.Rows[j];

                if (row[0].ToString() == "Январь")
                {
                    inserts.Add(j + inserts.Count);
                }

                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    if (i < 5 && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                    else
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) * 100;
                        }
                    }
                }
            }

            for (int i = 0; i < inserts.Count; i++)
            {
                DataRow r = dt.NewRow();
                r[0] = dt.Rows[inserts[i]].ItemArray[1].ToString();
                dt.Rows.InsertAt(r, inserts[i]);
            }
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            kdGroupName.Value = "Налоговые доходы ";
            string query = DataProvider.GetQueryText("FO_0002_0003_H");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);
            
            FormatDt();

            UltraWebGrid1.DataSource = dt;
        }


        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            kdGroupName.Value = "НДФЛ ";
            string query = DataProvider.GetQueryText("FO_0002_0003_H");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            FormatDt();

            UltraWebGrid2.DataSource = dt;
        }

        protected void UltraWebGrid3_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            kdGroupName.Value = "Налог на прибыль организаций ";
            string query = DataProvider.GetQueryText("FO_0002_0003_H");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            FormatDt();

            UltraWebGrid3.DataSource = dt;
        }

        protected void UltraWebGrid4_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            kdGroupName.Value = "Доходы ВСЕГО ";
            string query = DataProvider.GetQueryText("FO_0002_0003_H");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            FormatDt();

            UltraWebGrid4.DataSource = dt;
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

            if (e.Layout.Bands[0].Columns.Count > 6)
            {
                e.Layout.Bands[0].Columns[1].Hidden = true;

                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[2].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[3].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[4].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[5].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[6].CellStyle.Wrap = false;

                e.Layout.Bands[0].Columns[0].Header.Caption = "Месяц";
                e.Layout.Bands[0].Columns[2].Header.Caption = "За месяц млн.руб.";
                e.Layout.Bands[0].Columns[3].Header.Caption = "Назначено млн.руб.";
                e.Layout.Bands[0].Columns[4].Header.Caption = "Исполнено млн.руб.";
                e.Layout.Bands[0].Columns[5].Header.Caption = "Исполн %";
                e.Layout.Bands[0].Columns[6].Header.Caption = "Темп роста %";

                e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[3].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[4].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[5].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[6].Header.Style.Font.Size = FontUnit.Parse("14px");

                e.Layout.Bands[0].Columns[0].Width = 85;
                e.Layout.Bands[0].Columns[2].Width = 85;
                e.Layout.Bands[0].Columns[3].Width = 95;
                e.Layout.Bands[0].Columns[4].Width = 95;
                e.Layout.Bands[0].Columns[5].Width = 60;
                e.Layout.Bands[0].Columns[6].Width = 56;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N2");

                UltraGridRow row = new UltraGridRow();
                UltraGridCell cell = new UltraGridCell();
                cell.Value = 2007;
                row.Cells.Add(cell);
                row.Height = 20;
                e.Layout.Rows.Insert(1, row);
            }
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[3].Style.ForeColor = Color.FromArgb(209, 209, 209);
                e.Row.Cells[5].Style.ForeColor = Color.FromArgb(209, 209, 209);
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.Transparent;
                
                e.Row.Style.Wrap = false;

                decimal res;
                if (decimal.TryParse(e.Row.Cells[0].Value.ToString(), out res) && e.Row.Index != 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.FromArgb(50, 50, 50);
                    e.Row.Cells[i].Style.BorderDetails.ColorBottom = Color.Transparent;
                    e.Row.Cells[i].Style.ForeColor = Color.White;
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                else
                {
                    if (e.Row.Index == 0)
                    {
                        e.Row.Cells[i].Style.BorderDetails.ColorBottom = Color.Transparent;
                        e.Row.Cells[i].Style.ForeColor = Color.White;
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }

                if (e.Row.Index == dt.Rows.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
            }

        #endregion
        }
    }
}
