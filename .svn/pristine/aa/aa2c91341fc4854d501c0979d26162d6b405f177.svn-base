using System;
using System.Data;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0014_3
{
    public partial class Default : GadgetControlBase, IHotReport
    {
        private DataTable dtChart2;
        private CustomParam bkkuDate;

        public int Width
        {
            get { return CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2) + 100; }
        }

        public int Height
        {
            get { return CRHelper.GetChartWidth(CustomReportConst.minScreenHeight / 2); }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2);
                UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
                
                // CRHelper.FillCustomColorModel(UltraChart1, 17, false);
                UltraChart2.ColorModel.ModelStyle = ColorModels.PureRandom;

                UltraChart2.ChartType = ChartType.PieChart;
                UltraChart2.Border.Thickness = 0;
                //            UltraChart1.PieChart.ColumnIndex = 1;
                UltraChart2.PieChart.OthersCategoryPercent = 0;
                UltraChart2.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N2>%";
                UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\nфакт <DATA_VALUE:N2> тыс.руб.\nдоля <PERCENT_VALUE:N2>%";
                UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

                UltraChart2.PieChart.RadiusFactor = 80;
                UltraChart2.PieChart.StartAngle = 20;

                UltraChart2.Legend.Visible = false;
                UltraChart2.Legend.Location = LegendLocation.Bottom;
                UltraChart2.Legend.SpanPercentage = 32;
                UltraChart2.Legend.Margins.Top = 0;

                // CRHelper.FillCustomColorModel(UltraChart1, 17, false);
                UltraChart2.ColorModel.Skin.ApplyRowWise = true;

                string query = DataProvider.GetQueryText("FK_0001_0014_3_date", Server.MapPath("~/reports/FK_0001_0014_3/"));

                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();

                Page.Title = "Основные показатели консолидированных бюджетов субъектов РФ";

                int year = endYear;
                if (String.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    UserParams.StateArea.Value = "Ярославская область";
                }
                UserParams.Region.Value = RegionsNamingHelper.GetFoBySubject(UserParams.StateArea.Value);
                DateTime date = new DateTime(year, CRHelper.MonthNum(UserParams.PeriodMonth.Value), 1);
                UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, date, 4);
                UserParams.Subject.Value =
                    string.Format("[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);

                bkkuDate = UserParams.CustomParam("bkkuDate");

                if (date.Year == 2010)
                {
                    date.AddYears(-1);
                }

                bkkuDate.Value = CRHelper.PeriodMemberUName(String.Empty, date, 4);
            }

            UltraChart2.DataBind();
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0014_3_chart2", Server.MapPath("~/reports/FK_0001_0014_3/"));
            dtChart2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);
            foreach (DataRow row in dtChart2.Rows)
            {
                row[0] = row[0].ToString().Replace("br", "\n");
            }
            UltraChart2.DataSource = dtChart2;
        }


        public override string Title
        {
            get { return String.Format("Cтруктура расходной части бюджета {0}", RegionsNamingHelper.ShortName(UserParams.StateArea.Value)); }
            set {}
        }
    }
}
