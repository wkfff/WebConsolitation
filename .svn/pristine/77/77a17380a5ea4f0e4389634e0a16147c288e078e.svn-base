using System;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FNS_0001_0001_rsoa : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string connectionString = RegionSettingsHelper.GetValue("Alaniya", "ConnectionString");
            DataProvidersFactory.SetCustomPrimaryMASDataProvider(connectionString);
            Server.Transfer("~/reports/iphone/FNS_0001_0001.aspx");
        }
    }
}
