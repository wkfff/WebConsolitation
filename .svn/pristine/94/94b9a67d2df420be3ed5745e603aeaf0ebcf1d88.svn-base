using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.reports.DashboardNotepadFin;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.DashboardNotepadFin
{
    public partial class FO_0002_0004_Gadget : GadgetControlBase
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid1 = new DataTable();
        private DataTable dtGrid2 = new DataTable();
        private int firstYear = 2003;
        private int endYear = 2011;
        private string descendantsLevel = string.Empty;
        private string incomesListKind = string.Empty;

        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;

        #region Параметры запроса

        // мера Исполнено
        private CustomParam factMeasure;
        // мера Темп роста
        private CustomParam rateMeasure;
        // группа КД
        private CustomParam kdGroupName;
        // список выбранных годов для таблицы
        private CustomParam yearGridDescendants;
        // список выбранных годов для диаграммы
        private CustomParam yearChartDescendants;
        // выбранный район
        private CustomParam selectedRegion;
        // доходы Итого
        private CustomParam incomesTotal;

        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        #endregion



        public bool IncomesFullList
        {
            get { return incomesListKind == "FullList"; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Инициализация параметров запроса

            if (factMeasure == null)
            {
                factMeasure = UserParams.CustomParam("fact_measure");
            }
            if (rateMeasure == null)
            {
                rateMeasure = UserParams.CustomParam("rate_measure");
            }
            if (kdGroupName == null)
            {
                kdGroupName = UserParams.CustomParam("kd_group_name");
            }
            if (yearGridDescendants == null)
            {
                yearGridDescendants = UserParams.CustomParam("year_grid_descendants");
            }
            if (yearChartDescendants == null)
            {
                yearChartDescendants = UserParams.CustomParam("year_chart_descendants");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }

            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (consolidateDocumentSKIFType == null)
            {
                consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            }
            if (regionDocumentSKIFType == null)
            {
                regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Axis.X.Extent = 50;
            UltraChart.Axis.Y.Extent = 50;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            //            UltraChart.Axis.Y2.Extent = 50;
            //            UltraChart.Axis.Y2.Visible = true;
            //            UltraChart.Axis.Y2.Labels.Visible = false;
            //            UltraChart.Axis.Y2.LineThickness = 0;

            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            CRHelper.FillCustomColorModel(UltraChart, 10, false);
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 6;
            UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value / 2);

            UltraChart.TitleLeft.Text = "Млн.руб.";
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Extent = 30;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Visible = true;

            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <SERIES_LABEL>г.\n<DATA_VALUE:N3> млн.руб.";

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 5;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

            EmptyAppearance emptyAppearance = new EmptyAppearance();
            emptyAppearance.EnablePoint = true;
            emptyAppearance.EnablePE = true;
            emptyAppearance.EnableLineStyle = true;
            emptyAppearance.PointStyle.Icon = SymbolIcon.Circle;
            emptyAppearance.PointStyle.IconSize = SymbolIconSize.Large;
            emptyAppearance.LineStyle.MidPointAnchors = true;
            UltraChart.SplineChart.EmptyStyles.Add(emptyAppearance);

            UltraChart.Data.ZeroAligned = true;
            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;

            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion
            
            UltraChart.Width = 1200;
            UltraChart.Height = 320;

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
           
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            firstYear = 2008;
            incomesListKind = RegionSettingsHelper.Instance.GetPropertyValue("IncomesListKind");
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0004_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            
            descendantsLevel = "Месяц";

            Collection<string> selectedValues = new Collection<string>();
            selectedValues.Add((endYear - 2).ToString());
            selectedValues.Add((endYear - 1).ToString());
            selectedValues.Add(endYear.ToString());
            if (selectedValues.Count > 0)
            {
                string chartDescendants = string.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    // для таблицы всегда выбираем месяцы

                    chartDescendants += string.Format("Descendants ({1}.[Данные всех периодов].[{0}],{1}.[{2}], SELF) + ",
                        year, UserParams.PeriodDimension.Value, descendantsLevel);
                }

                chartDescendants = chartDescendants.Remove(chartDescendants.Length - 3, 2);
                yearChartDescendants.Value = string.Format("{1}{0}{2}", chartDescendants, '{', '}');
            }
            else
            {
                yearChartDescendants.Value = "{}";
            }

            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            kdGroupName.Value = "НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ";
            factMeasure.Value = "Факт_за период";
            rateMeasure.Value = "Темп роста к аналогичному периоду предыдущего года_За период";
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            UltraChart.DataBind();

            Label1.Text = String.Format("динамика за {0}-{1} гг.", firstYear, endYear);
        }

        #region Обработчики грида



        private static string GetChartQuarterStr(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return string.Format("{0} квартал", quarterNumber);
            }
            else
            {
                return period;
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0004_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                DataTable newDtChart = new DataTable();
                DataColumn column = new DataColumn("Год", typeof(string));
                newDtChart.Columns.Add(column);

                for (int i = 1; i < 13; i++)
                {
                    DataColumn monthColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), typeof(double));
                    newDtChart.Columns.Add(monthColumn);
                }

                int year = 0;
                DataRow currRow = null;
                foreach (DataRow row in dtChart.Rows)
                {
                    int currYear = 0;
                    string period = string.Empty;
                    double measureValue = double.MinValue;
                    if (row[0] != DBNull.Value)
                    {
                        period = row[0].ToString();
                        period = GetChartQuarterStr(period);
                    }
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        currYear = Convert.ToInt32(row[1]);
                    }
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        measureValue = Convert.ToDouble(row[2]) / 1000000;
                    }

                    // добавляем новый год
                    if (year != currYear)
                    {
                        year = currYear;
                        DataRow newRow = newDtChart.NewRow();
                        newRow[0] = year;
                        newDtChart.Rows.Add(newRow);

                        currRow = newRow;
                    }

                    if (currRow != null && newDtChart.Columns.Contains(period) && measureValue != double.MinValue)
                    {
                        currRow[period] = measureValue;
                    }
                }

                UltraChart.DataSource = newDtChart;
            }
        }

        #endregion

        #region IWebPart Members

        public override string Description
        {
            get { return "Динамика поступлений собственных доходов"; }
        }

        public override string Title
        {
            get { return "Динамика поступлений собственных доходов"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FO_0002_0004/DefaultDetail.aspx"; }
        }

        #endregion
    }
}