using System;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class FST_0003_0002_07 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DateTime currentDate = CubeInfoHelper.FstTariffsAndRegulationsInfo.LastDate;
            DateTime lastDate = new DateTime(currentDate.Year - 1, 12, 1);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]", currentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]", lastDate, 4);

            UltraGrid.ReportDate = currentDate;
            UltraGrid.LastDate = lastDate;
            UltraGrid.ReportCode = "FST_0003_0002_07";
            UltraGrid.ServiceName = "Поставки твёрдого топлива при наличии печного отопления";
        }
    }
}
