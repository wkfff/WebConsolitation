using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image=System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.DashboardNotepadFin.Dashboard.reports.DashboardNotepadFin
{
    public partial class FO_0002_0007_Gadget : GadgetControlBase
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private int endYear;
        private int monthNum;

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
            UltraChart2.ChartType = ChartType.ColumnChart;

            UltraChart2.Height = 320;

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Right;
            UltraChart2.Legend.SpanPercentage = 6;
            UltraChart2.Legend.Margins.Bottom = Convert.ToInt32(UltraChart2.Height.Value / 2);
            UltraChart2.Legend.Font = new Font("Verdana", 8);

            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Width = 1200;

            //UltraChart2.Legend.Margins.Right = (int)UltraChart2.Width.Value / 4 * 3;
            UltraChart2.Axis.X.Labels.Visible = false;

            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;

            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            //behavior.ClipText = false;
            //behavior.HideText = false;
            behavior.Trimming = StringTrimming.None;

            UltraChart2.Axis.X.Labels.SeriesLabels.Layout.BehaviorCollection.Add(behavior);

            UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Extent = 140;
            UltraChart2.Axis.Y.Extent = 50;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "млн.руб.";
            UltraChart2.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = UltraChart2.Axis.Y.Extent + 10;
            UltraChart2.TitleLeft.Extent = 50;
            
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.";
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;

            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);

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
            string month = dtDate.Rows[0][3].ToString();


            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            outcomesFKRTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            monthNum = CRHelper.MonthNum(month);
            int yearNum = endYear;

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.PeriodMonth.Value = month;

            UltraChart2.DataBind();

            topLabel.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), endYear);

            HyperLink1.Text = "Расходы";
            HyperLink2.Text = "Исполнение расходов";
            HyperLink1.NavigateUrl = "~/reports/FO_0002_0006/DefaultDetail.aspx";
            HyperLink2.NavigateUrl = "~/reports/FO_0002_0007/DefaultCompare.aspx";
        }

        private void UltraChart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive p = e.SceneGraph[i];
                if (p is Text)
                {
                    Text text = (Text)p;
                    if (text.Path != null && text.Path.Contains("Title.Grid.X"))
                    {
                        text.bounds.X = text.bounds.X - text.bounds.Width/2;
                        text.bounds.Width = text.bounds.Width*2;
                    }
                }
            }
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
            get { return "Исполнение расходов"; }
        }

        public override string Title
        {
            get { return "Исполнение расходов"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FO_0002_0006/DefaultDetail.aspx"; }
        }
        #endregion
    }
}