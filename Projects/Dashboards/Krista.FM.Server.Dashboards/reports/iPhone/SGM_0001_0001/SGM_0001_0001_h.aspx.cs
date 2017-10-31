using System;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0001_0001_h : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "÷ентральный федеральный округ";
                UserParams.StateArea.Value = "ярославска€ область";
            }
        }
    }

}
