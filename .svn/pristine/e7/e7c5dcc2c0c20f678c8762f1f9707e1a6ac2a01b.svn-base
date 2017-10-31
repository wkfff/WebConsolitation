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
    public partial class FO_0002_0005_v : CustomReportPage
    {
        private DataTable dt;

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
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

            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
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
            string query = DataProvider.GetQueryText("FO_0002_0005_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
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

            Label3.Text = string.Format("Процент исполнения плана по доходам и среднедушевые доходы (тыс.руб./чел.) за {0}&nbsp;{1}&nbsp;{2}&nbsp;года по {3}.",
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
            string query = DataProvider.GetQueryText("FO_0002_0005_v");
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
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("п.", "");

                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("Новокуйбышевск", "Новокуйбы-шевск");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("ГО \"Смирныховский\"", "ГО \"Смирныхов-ский\"");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("ГО \"Александровск-Сахалинский р&#8209н\"", "ГО \"Александ-ровск-Сахалинский р&#8209н\"");

                    dt.Rows[i][0] = CropString(dt.Rows[i][0].ToString());
                }
                
                if (dt.Rows[i][1] != DBNull.Value)
                {
                    dt.Rows[i][1] = Convert.ToDouble(dt.Rows[i][1]) * 100;
                }

                if (dt.Rows[i][4] != DBNull.Value)
                {
                    dt.Rows[i][4] = Convert.ToDouble(dt.Rows[i][4]) / 1000;
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

            if (e.Layout.Bands[0].Columns.Count > 6)
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
                AddColumnHeader(e.Layout, "Исполнено", 0, 1, 2);
                AddColumnHeader(e.Layout, "Среднедуш. доходы", 0, 3, 2);

                e.Layout.Bands[0].Columns[3].Hidden = true;
                e.Layout.Bands[0].Columns[6].Hidden = true;

                e.Layout.Bands[0].Columns[1].Header.Caption = "%";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[2].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[4].Header.Caption = "тыс.р./ чел.";
                e.Layout.Bands[0].Columns[4].Header.RowLayoutColumnInfo.OriginX = 4;
                e.Layout.Bands[0].Columns[5].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[5].Header.RowLayoutColumnInfo.OriginX = 5;

                e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[4].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].HeaderStyle.Height = 5;
                for (int i = 1; i <= 6; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                }

                e.Layout.Bands[0].Columns[0].Width = 113;
                e.Layout.Bands[0].Columns[1].Width = 47;
                e.Layout.Bands[0].Columns[2].Width = 49;
                e.Layout.Bands[0].Columns[4].Width = 51;
                e.Layout.Bands[0].Columns[5].Width = 49;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 2 || i == 5);

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
