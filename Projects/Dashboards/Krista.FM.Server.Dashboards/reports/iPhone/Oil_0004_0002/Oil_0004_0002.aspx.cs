using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Primitive = Infragistics.UltraChart.Core.Primitives.Primitive;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0004_0002 : CustomReportPage
    {
        private DateTime currentDateFederal;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDateFederal = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0004_0002_incomes_federal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDateFederal);

            DateTime currentDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[2][1].ToString(), 3);
            DateTime lastDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[1][1].ToString(), 3);
            DateTime firstDate = new DateTime(currentDate.Year - 1, 12, 30);//CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[0][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDateFederal.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDateFederal.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", firstDate, 5);//dtDateFederal.Rows[0][1].ToString();

            UltraChartOIL_0004_0002_Chart2.AppearanceColor = Color.Gold;
            UltraChartOIL_0004_0002_Chart3.AppearanceColor = Color.Pink;
            UltraChartOIL_0004_0002_Chart4.AppearanceColor = Color.Red;

            UltraChartOIL_0004_0002_Chart2.ReportDate = currentDate;
            UltraChartOIL_0004_0002_Chart3.ReportDate = currentDate;
            UltraChartOIL_0004_0002_Chart4.ReportDate = currentDate;

            UltraChartOIL_0004_0002_Chart2.LastDate = lastDate;
            UltraChartOIL_0004_0002_Chart3.LastDate = lastDate;
            UltraChartOIL_0004_0002_Chart4.LastDate = lastDate;

            UltraChartOIL_0004_0002_Chart2.FirstDate = firstDate;
            UltraChartOIL_0004_0002_Chart3.FirstDate = firstDate;
            UltraChartOIL_0004_0002_Chart4.FirstDate = firstDate;

            UltraChartOIL_0004_0002_Chart2.TaxName = "Бензин марки АИ-92";
            UltraChartOIL_0004_0002_Chart3.TaxName = "Бензин марки АИ-95";
            UltraChartOIL_0004_0002_Chart4.TaxName = "Дизельное топливо";

            UltraChartOIL_0004_0002_Chart2.OilId = "2";
            UltraChartOIL_0004_0002_Chart3.OilId = "3";
            UltraChartOIL_0004_0002_Chart4.OilId = "4";
        }
    }
}
