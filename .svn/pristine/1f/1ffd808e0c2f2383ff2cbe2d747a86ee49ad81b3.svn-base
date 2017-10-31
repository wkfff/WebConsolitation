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
    public partial class Oil_0003_0001 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0003_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            DateTime currentDate = CRHelper.PeriodDayFoDate(dtDate.Rows[0][1].ToString());


        }
    }
}
