using System;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class FST_0002_0002_05 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DateTime currentDate = new DateTime(2012, 1, 1);
            DateTime lastDate = new DateTime(2011, 11, 1);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__��� ������� �����].[������__��� ������� �����]", currentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[������__��� ������� �����].[������__��� ������� �����]", lastDate, 4);

            UltraGrid.ReportDate = currentDate;
            UltraGrid.LastDate = lastDate;
            UltraGrid.ReportCode = "FST_0002_0002_05";
            UltraGrid.ServiceName = "���������";
        }
    }
}
