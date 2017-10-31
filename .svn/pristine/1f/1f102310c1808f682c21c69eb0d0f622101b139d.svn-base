using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.DashboardNotepadFin
{
    public partial class FO_0002_0003_Gadget : GadgetControlBase
    {
         #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private int endYear = 2011;
        private string month;
        
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
        // Расходы ФКР Всего
        private CustomParam outcomesFKRTotal;

        #endregion

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
            if (outcomesFKRTotal == null)
            {
                outcomesFKRTotal = UserParams.CustomParam("outcomes_fkr_total");
            }
           
            #endregion

            #region Настройка диаграммы

            UltraChart2.Width = 620;
            UltraChart2.Height = 350;

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Font = new Font("Verdana", 8);
            UltraChart2.Legend.Location = LegendLocation.Left;
            UltraChart2.Legend.SpanPercentage = 50;
            UltraChart2.Legend.Margins.Top = 0;

            //UltraChart2.TitleTop.Visible = true;
            //UltraChart2.TitleTop.Text = "Факт";
            //UltraChart2.TitleTop.HorizontalAlign = StringAlignment.Center;
            //UltraChart2.TitleTop.Font = new Font("Verdana", 10, FontStyle.Bold);
            //UltraChart2.TitleTop.Extent = 20;

            UltraChart2.ChartType = ChartType.PieChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.PieChart.ColumnIndex = 2;
            UltraChart2.PieChart.Labels.Font = new Font("Verdana", 8);
            UltraChart2.PieChart.OthersCategoryPercent = 1;
            UltraChart2.PieChart.OthersCategoryText = "Прочие";
            UltraChart2.PieChart.RadiusFactor = 80;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\nфакт <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";
            CRHelper.FillCustomColorModel(UltraChart2, 17, false);
            UltraChart2.ColorModel.Skin.ApplyRowWise = true;
            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.FOFKRCulture.Value = RegionSettingsHelper.Instance.FOFKRCulture;
            UserParams.FOFKRHelthCare.Value = RegionSettingsHelper.Instance.FOFKRHelthCare;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0008_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            month = dtDate.Rows[0][3].ToString();
            
            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            outcomesFKRTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            
            int monthNum = CRHelper.MonthNum(month);
            int yearNum = endYear;
            topLabel.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.PeriodMonth.Value = month;

            UltraChart2.DataBind();
        }
               

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = Session["rzprType"].ToString() == "rzpr" ? DataProvider.GetQueryText("FO_0002_0008_chart_FKR") : DataProvider.GetQueryText("FO_0002_0008_chart_KOSGU");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
                        
            foreach (DataRow row in dtChart.Rows)
            {
                if (Session["rzprType"].ToString() != "rzpr" && row[0] != DBNull.Value)
                {
                    row[0] = DataDictionariesHelper.GetShortKOSGUName(row[0].ToString());
                }

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1 || i == 2)
                         && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            ((UltraChart)sender).DataSource = dtChart;
        }

        #endregion

        #region IWebPart Members

        public override string Description
        {
            get { return "Структура расходов"; }
        }

        public override string Title
        {
            get { return "Структура расходов"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FO_0002_0008/Default.aspx"; }
        }

        #endregion
    }
}