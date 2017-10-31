using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.DashboardNotepadFin
{
    public partial class FO_0002_0011_Gadget : GadgetControlBase, IHotReport
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart1;
        private int firstYear = 2000;
        private int endYear = 2011;

        private int monthNum;
        private int yearNum;

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
                    row[0] = DataDictionariesHelper.GetShortKDName(row[0].ToString().TrimEnd(' '));
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


        public int Width
        {
            get { return 350; }
        }

        public int Height
        {
            get { return 300; }
        }

       
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

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

            UltraChart1.Width = 580;
            UltraChart1.Height = 300;

            UltraChart1.ChartType = ChartType.DoughnutChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart1.DoughnutChart.ShowConcentricLegend = false;
            UltraChart1.DoughnutChart.Concentric = true;
            UltraChart1.DoughnutChart.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Left;
            UltraChart1.Legend.SpanPercentage = 40;
            UltraChart1.Legend.Margins.Top = 0;
            UltraChart1.Legend.Font = new Font("Verdana", 8);

            CRHelper.FillCustomColorModel(UltraChart1, 17, false);
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;

            CalloutAnnotation planAnnotation = new CalloutAnnotation();
            planAnnotation.Text = "План";
            planAnnotation.Width = 40;
            planAnnotation.Height = 20;
            planAnnotation.TextStyle.Font = new Font("Verdana", 10);
            planAnnotation.Location.Type = LocationType.Percentage;
            planAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;
            planAnnotation.Location.LocationX = UltraChart1.Legend.SpanPercentage + (100 - UltraChart1.Legend.SpanPercentage) / 2;
            planAnnotation.Location.LocationY = 71;

            CalloutAnnotation factAnnotation = new CalloutAnnotation();
            factAnnotation.Text = "Факт";
            factAnnotation.Width = 40;
            factAnnotation.Height = 20;
            factAnnotation.TextStyle.Font = new Font("Verdana", 10);
            factAnnotation.Location.Type = LocationType.Percentage;
            factAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;
            factAnnotation.Location.LocationX = UltraChart1.Legend.SpanPercentage + (100 - UltraChart1.Legend.SpanPercentage) / 2;
            factAnnotation.Location.LocationY = 14;

            UltraChart1.Annotations.Add(planAnnotation);
            UltraChart1.Annotations.Add(factAnnotation);
            UltraChart1.Annotations.Visible = true;
           
            #endregion

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
                        
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0011_01_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            
            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = "[ТипДокумента].[СКИФ].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";

            budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Бюджет субъекта]";

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            Label1.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();

            UltraChart1.DataBind();
        }


        #region IWebPart Members

        public override string Description
        {
            get { return "Структура собственных доходов"; }
        }

        public override string Title
        {
            get { return "Структура собственных доходов"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FO_0002_0011_01/"; }
        }

        #endregion

    }
}