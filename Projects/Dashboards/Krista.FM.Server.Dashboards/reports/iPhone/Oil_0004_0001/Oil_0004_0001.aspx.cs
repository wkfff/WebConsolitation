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
    public partial class Oil_0004_0001 : CustomReportPage
    {
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
            UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", firstDate, 5);

            OIL_0004_0001_Text2.AppearanceColor = Color.Gold;
            OIL_0004_0001_Text3.AppearanceColor = Color.Pink;
            OIL_0004_0001_Text4.AppearanceColor = Color.Red;
            
            OIL_0004_0001_Text2.ReportDate = currentDate;
            OIL_0004_0001_Text3.ReportDate = currentDate;
            OIL_0004_0001_Text4.ReportDate = currentDate;
            
            OIL_0004_0001_Text2.LastDate = lastDate;
            OIL_0004_0001_Text3.LastDate = lastDate;
            OIL_0004_0001_Text4.LastDate = lastDate;
           
            OIL_0004_0001_Text2.FirstDate = firstDate;
            OIL_0004_0001_Text3.FirstDate = firstDate;
            OIL_0004_0001_Text4.FirstDate = firstDate;
            
            OIL_0004_0001_Text2.TaxName = "Бензин марки АИ-92";
            OIL_0004_0001_Text3.TaxName = "Бензин марки АИ-95";
            OIL_0004_0001_Text4.TaxName = "Дизельное топливо";
            
            OIL_0004_0001_Text2.OilId = "2";
            OIL_0004_0001_Text3.OilId = "3";
            OIL_0004_0001_Text4.OilId = "4";
        }
    }
}
