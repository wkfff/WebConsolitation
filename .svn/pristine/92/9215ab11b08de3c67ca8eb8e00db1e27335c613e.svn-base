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
    public partial class FO_0002_0007_h : CustomReportPage
    {
        private DataTable dt;

        #region Параметры запроса

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
        // численность населения
        private CustomParam populationMeasure;
        // год для численности населения
        private CustomParam populationMeasureYear;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            #region Инициализация параметров запроса

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
            if (populationMeasure == null)
            {
                populationMeasure = UserParams.CustomParam("population_measure");
            }
            if (populationMeasureYear == null)
            {
                populationMeasureYear = UserParams.CustomParam("population_measure_year");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0007_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning ? (yearNum + 1).ToString() : yearNum.ToString();
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            UserParams.PopulationCube.Value = RegionSettingsHelper.Instance.PopulationCube;
            UserParams.PopulationFilter.Value = RegionSettingsHelper.Instance.PopulationFilter;
            UserParams.PopulationPeriodDimension.Value = RegionSettingsHelper.Instance.PopulationPeriodDimension;
            UserParams.PopulationValueDivider.Value = RegionSettingsHelper.Instance.PopulationValueDivider;

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

            TextBox1.Text = string.Format("Расходы бюджетов, процент исполнения плана по расходам");
            TextBox2.Text = string.Format("и бюджетные расходы на душу населения (тыс.руб./чел.)");
            TextBox3.Text = string.Format("за {0} {1} {2} года по {3}.",
                monthNum,
                monthStr,
                yearNum,
                RegionSettingsHelper.Instance.ShortName);
                                    
            TextBox5.Text = string.Format("Расходы млн.руб. = расходы бюджетов за {0} {1} {2} года", 
                                        monthNum,
                                        monthStr,
                                        yearNum);

            TextBox7.Text = "Насел. тыс.чел. = численность постоянного населения";

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
                    if (words[0].Contains("партизанский"))
                    {
                        return words[0].Replace("занский", "-занский");
                    }
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
            string query = DataProvider.GetQueryText("FO_0002_0007_h");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("Городской округ", "ГО");
                    row[0] = row[0].ToString().Replace("городской округ", "ГО"); 
                    row[0] = row[0].ToString().Replace("муниципальное образование", "МО");
                    row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                    row[0] = row[0].ToString().Replace("Муниципальный район", "МР");
                    row[0] = row[0].ToString().Replace("г.", "город ");
                    row[0] = row[0].ToString().Replace("район", "р&#8209н");
                    row[0] = row[0].ToString().Replace("п.", "");

                    row[0] = row[0].ToString().Replace("Новокуйбышевск", "Новокуйбы-шевск");
                    row[0] = row[0].ToString().Replace("ГО \"Смирныховский\"", "ГО \"Смирныхов-ский\"");
                    row[0] = row[0].ToString().Replace("ГО \"Александровск-Сахалинский р&#8209н\"", "ГО \"Александ-ровск-Сахалинский р&#8209н\"");

                    row[0] = CropString(row[0].ToString());
                }
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / 1000000;
                }
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) * 100;
                }
                if (row[5] != DBNull.Value)
                {
                    row[5] = Convert.ToDouble(row[5]);
                }
                if (row[6] != DBNull.Value)
                {
                    row[6] = Convert.ToDouble(row[6]) / 1000;
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

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 7)
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

                AddColumnHeader(e.Layout, "Бюджеты", 0, 0, 1);
                AddColumnHeader(e.Layout, "Расходы", 0, 1, 1);
                AddColumnHeader(e.Layout, "Исполнено", 0, 2, 2);
                AddColumnHeader(e.Layout, "Насел.", 0, 4, 1);
                AddColumnHeader(e.Layout, "Бюдж.расх. на душу населения", 0, 5, 2);

                e.Layout.Bands[0].Columns[4].Hidden = true;
                e.Layout.Bands[0].Columns[8].Hidden = true;

                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.Height = 5;

                e.Layout.Bands[0].Columns[3].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[7].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[1].Header.Caption = "млн. руб.";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[2].Header.Caption = "%";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[3].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 3;
                e.Layout.Bands[0].Columns[5].Header.Caption = "тыс.чел.";
                e.Layout.Bands[0].Columns[5].Header.RowLayoutColumnInfo.OriginX = 5;
                e.Layout.Bands[0].Columns[6].Header.Caption = "тыс.р./ чел.";
                e.Layout.Bands[0].Columns[6].Header.RowLayoutColumnInfo.OriginX = 6;
                e.Layout.Bands[0].Columns[7].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[7].Header.RowLayoutColumnInfo.OriginX = 7;
                e.Layout.Bands[0].Columns[8].Header.RowLayoutColumnInfo.OriginX = 8;

                for (int i = 1; i <= 7; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");

                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Height = 5;
                    e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");

                e.Layout.Bands[0].Columns[0].Width = 113;
                e.Layout.Bands[0].Columns[1].Width = 81;
                e.Layout.Bands[0].Columns[2].Width = 53;
                e.Layout.Bands[0].Columns[3].Width = 52;
                e.Layout.Bands[0].Columns[5].Width = 66;
                e.Layout.Bands[0].Columns[6].Width = 52;
                e.Layout.Bands[0].Columns[7].Width = 51;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 3 || i == 7);

                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                e.Row.Cells[i].Style.Padding.Top = 7;

                if (i == 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 3;
                }
                else
                {
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }

                if (i == 0 || i == 1 || i == 3 || i == 5)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }

                if (i == e.Row.Cells.Count - 2)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }

                if (e.Row.Index == 2 || e.Row.Index == dt.Rows.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }

                if (rank)
                {
                    string img = string.Empty;
                    if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                    {
                        img = "~/images/starYellow.png";
                    }
                    else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value) &&
                             Convert.ToInt32(e.Row.Cells[i].Value) != 0)
                    {
                        img = "~/images/starGray.png";
                    }

                    e.Row.Cells[i].Style.VerticalAlign = VerticalAlign.Middle;
                    e.Row.Cells[i].Style.Padding.Top = 1;
                    e.Row.Cells[i].Style.Padding.Bottom = 1;
                    e.Row.Cells[i].Style.BackgroundImage = img;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
                }
            }
        }

        #endregion
    }
}
