using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0003_0001 : CustomReportPage
    {

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            IT_0002_0001_IncomesGrid1.DescriptionText = "Доходы филиала по " + UserParams.Region.Value +
                                                        "<br/>составляют&nbsp;<span style='color: White'><b>{0:N0}</b></span>&nbsp;тыс.руб.";
            IT_0002_0001_IncomesGrid2.DescriptionText = "Расходы филиала по " + UserParams.Region.Value +
                                                        "<br/>составляют&nbsp;<span style='color: White'><b>{0:N0}</b></span>&nbsp;тыс.руб.";
        }
    }
}
