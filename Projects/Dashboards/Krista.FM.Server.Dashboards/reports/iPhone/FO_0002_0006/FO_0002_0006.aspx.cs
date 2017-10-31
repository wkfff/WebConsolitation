using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0006 : CustomReportPage
    {
        #region Параметры запроса

        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;
        // расходы Итого
        private CustomParam outcomesTotal;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;

            #region Инициализация параметров запроса

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
            outcomesTotal = UserParams.CustomParam("outcomes_total");

            #endregion

            #region Настройка диаграммы

            UltraChart.Width = 310;
            UltraChart.Height = 340;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Axis.Y.Extent = 100;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;

            UltraChart.Axis.X.Visible = false;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 75;
            UltraChart.Legend.Margins.Bottom = 0;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE_ITEM:P2>";

            #endregion

            DataTable dtDate = new DataTable();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0006_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.FOFKRCulture.Value = RegionSettingsHelper.Instance.FOFKRCulture;
            UserParams.FOFKRHelthCare.Value = RegionSettingsHelper.Instance.FOFKRHelthCare;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName == "Собственный бюджет" 
                ? "Собств. бюджет" 
                : "Бюджет субъекта";

            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            query = DataProvider.GetQueryText("FO_0002_0006");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dt);

            foreach(DataColumn column in dt.Columns)
            {
                column.ColumnName = GetShortRzPrName(column.ColumnName);
            }
            
            UltraChart.DataSource = dt;
            UltraChart.DataBind();
        }

        private static string GetShortRzPrName(string fullName)
        {
            string shortName = fullName;

            switch (fullName)
            {
                case "ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ":
                    {
                        return "Общегосуд.вопросы";
                    }
                case "НАЦИОНАЛЬНАЯ ОБОРОНА":
                    {
                        return "Национальная оборона";
                    }
                case "НАЦИОНАЛЬНАЯ БЕЗОПАСНОСТЬ И ПРАВООХРАНИТЕЛЬНАЯ ДЕЯТЕЛЬНОСТЬ":
                    {
                        return "Нац.безопасность и правоохранит.деят.";
                    }
                case "НАЦИОНАЛЬНАЯ ЭКОНОМИКА":
                    {
                        return "Национальная экономика";
                    }
                case "ЖИЛИЩНО-КОММУНАЛЬНОЕ ХОЗЯЙСТВО":
                    {
                        return "ЖКХ";
                    }
                case "ОХРАНА ОКРУЖАЮЩЕЙ СРЕДЫ":
                    {
                        return "Охрана окруж.среды";
                    }
                case "ОБРАЗОВАНИЕ":
                    {
                        return "Образование";
                    }
                case "КУЛЬТУРА, КИНЕМАТОГРАФИЯ":
                    {
                        return "Культура и кинематография";
                    }
                case "КУЛЬТУРА, КИНЕМАТОГРАФИЯ, СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ":
                    {
                        return "Культура,  кинематография, СМИ";
                    }
                case "СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ":
                    {
                        return "СМИ";
                    }
                case "ЗДРАВООХРАНЕНИЕ":
                    {
                        return "Здравоохранение";
                    }
                case "ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ":
                    {
                        return "Здрав., физ.культура и спорт";
                    }
                case "ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ":
                    {
                        return "Физическая культура и спорт";
                    }
                case "СОЦИАЛЬНАЯ ПОЛИТИКА":
                    {
                        return "Социальная политика";
                    }
                case "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ":
                    {
                        return "Межбюджетные трансферты";
                    }
                case "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ ОБЩЕГО ХАРАКТЕРА БЮДЖЕТАМ СУБЪЕКТОВ РОССИЙСКОЙ ФЕДЕРАЦИИ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ":
                    {
                        return "МБТ бюджетам суб.РФ и МО";
                    }
                case "ОБСЛУЖИВАНИЕ ГОСУДАРСТВЕННОГО И МУНИЦИПАЛЬНОГО ДОЛГА":
                    {
                        return "Обслуж.гос.и мун.долга";
                    }
            }
            return shortName;
        }
    }
}
