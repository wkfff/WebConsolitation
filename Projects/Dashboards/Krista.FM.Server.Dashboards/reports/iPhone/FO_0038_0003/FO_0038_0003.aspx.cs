using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0038_0003 : CustomReportPage
    {
        private int monthNum;
        private int yearNum;

        private GridHeaderLayout headerLayout;
        private string bujetName;


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0011_01_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddMonths(-1), 4);
            Label1.Text = String.Format("Структура расходов бюджета ВологО<br/>за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);       

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (documentType == null)
            {
                documentType = UserParams.CustomParam("document_type");
            }
            if (consolidateLevel == null)
            {
                consolidateLevel = UserParams.CustomParam("consolidate_level");
            }
            if (budgetSKIFLevel == null)
            {
                budgetSKIFLevel = UserParams.CustomParam("budget_SKIF_level");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart1.Width = 310;
            UltraChart1.Height = 340;

            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.PieChart.OthersCategoryText = "Прочие";
            UltraChart1.PieChart.Labels.Visible = true;
            UltraChart1.PieChart.Labels.LeaderLinesVisible = true;
            UltraChart1.PieChart.Labels.FontColor = Color.White;
            UltraChart1.PieChart.Labels.Font = new Font("Verdana", 10);
            UltraChart1.PieChart.Labels.LeaderLineColor = Color.White;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";
            UltraChart1.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N2>%";
            UltraChart1.PieChart.RadiusFactor = 60;
            UltraChart1.PieChart.StartAngle = 60;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

            UltraChart1.Legend.Visible = false;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i < 9; i++)
            {
                UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(GetColorChart(i), 150));
            }

            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

            #endregion

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = "[ТипДокумента].[СКИФ].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";

            budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Бюджет субъекта]";

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            
            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();

            UltraChart1.DataBind();
        }

        private static Color GetColorChart(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.LawnGreen;
                    }
                case 2:
                    {
                        return Color.Magenta;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.Cyan;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
            }
            return Color.White;
        }

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart1.Legend.Location == LegendLocation.Top) || (UltraChart1.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart1.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = (UltraChart1.Legend.SpanPercentage * (int)UltraChart1.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart1;
        private int firstYear = 2000;
        private int endYear = 2011;

        #endregion

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;
        // Тип документа
        private CustomParam documentType;
        // Консолидированный уровень
        private CustomParam consolidateLevel;
        // Уровень бюджета СКИФ
        private CustomParam budgetSKIFLevel;

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_01_chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            foreach (DataRow row in dtChart1.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().TrimEnd(' ').Replace(' ', '\n');
                }
            }

            UltraChart1.DataSource = dtChart1;
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Wedge)
                {
                    Wedge wedge = (Wedge)primitive;
                    if (wedge.DataPoint != null)
                    {
                        string shortName = wedge.DataPoint.Label;
                        string fullName = DataDictionariesHelper.GetFullKDName(shortName);
                        string name = shortName == fullName ? fullName : string.Format("{0} ({1}) ", fullName, shortName);
                        wedge.DataPoint.Label = string.Format("{0}\n{1}", name, wedge.Column == 1 ? "план" : "факт");
                    }
                }
            }
        }

        #endregion

    }
}