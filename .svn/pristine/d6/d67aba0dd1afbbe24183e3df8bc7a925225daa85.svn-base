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
    public partial class FO_0002_0013_v : CustomReportPage
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
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

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
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");

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
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

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

            Label3.Text = string.Format("Процент исполнения плана и темп роста за {0}&nbsp;{1}&nbsp;{2}&nbsp;года по {3}.",
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
            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        private static string CropString(string source)
        {
            string[] words = source.Split(' ');
            if (words.Length > 0)
            {
                if (words[0].Length > 12 && !words[0].Contains("-"))
                {
                    if (words[0].Contains("ниговский"))
                    {
                        return words[0].Replace("ниговский", "-ниговский");
                    }
                    if (words[0].Contains("шицкий"))
                    {
                        return words[0].Replace("шицкий", "-шицкий");
                    }
                    if (words[0].Contains("ский"))
                    {
                        return words[0].Replace("ский", "-ский");
                    }
                }

            }
            return source;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0013_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dt);
           
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0] != DBNull.Value)
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("Городской округ", "ГО");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("городской округ", "ГО"); 
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("муниципальное образование", "МО");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("муниципальный район", "МР");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("Муниципальный район", "МР");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("г.", "город ");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("район", "р&#8209н");

                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("Новокуйбышевск", "Новокуйбы-шевск");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("ГО \"Смирныховский\"", "ГО \"Смирныхов-ский\"");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("ГО \"Александровск-Сахалинский р&#8209н\"", "ГО \"Александ-ровск-Сахалинский р&#8209н\"");

                    dt.Rows[i][0] = CropString(dt.Rows[i][0].ToString());
                }
            }

            UltraWebGrid.DataSource = dt;
        }

        private static void AddColumnHeader(UltraGridLayout layout, string caption, int originX, int originY, int spanX)
        {
            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = caption;
            ch.RowLayoutColumnInfo.OriginY = originX;
            ch.RowLayoutColumnInfo.OriginX = originY;
            ch.RowLayoutColumnInfo.SpanX = spanX;
            ch.Style.Padding.Top = 1;
            ch.Style.Padding.Bottom = 1;
            ch.Style.Height = 5;
            ch.Style.VerticalAlign = VerticalAlign.Middle;
            layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void UltraWebGrid_InitializeLayout(object sender,LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 4)
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

                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.Height = 5;

                AddColumnHeader(e.Layout, "Бюджет", 0, 0, 1);
                AddColumnHeader(e.Layout, "Доходы", 0, 1, 2);
                AddColumnHeader(e.Layout, "Расходы", 0, 3, 2);

                e.Layout.Bands[0].Columns[1].Header.Caption = "исп. %";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[2].Header.Caption = "темп роста %";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[3].Header.Caption = "исп. %";
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 3;
                e.Layout.Bands[0].Columns[4].Header.Caption = "темп роста %";
                e.Layout.Bands[0].Columns[4].Header.RowLayoutColumnInfo.OriginX = 4;

                e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[4].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].HeaderStyle.Height = 5;
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                }

                e.Layout.Bands[0].Columns[0].Width = 113;
                e.Layout.Bands[0].Columns[1].Width = 49;
                e.Layout.Bands[0].Columns[2].Width = 49;
                e.Layout.Bands[0].Columns[3].Width = 49;
                e.Layout.Bands[0].Columns[4].Width = 49;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                
                if (i == 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 3;
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }
                else
                {
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }

                if (i == 0 || i == 2 || i == dt.Rows.Count - 2)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }

                if (e.Row.Index == 2 || e.Row.Index == dt.Rows.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
            }
        }

        #endregion
    }
}
