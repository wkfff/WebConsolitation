using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0005_Horizontal_h : CustomReportPage
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
            scriptBlock.InnerHtml = String.Format("<SCRIPT type='text/javascript'>{1}<!--{1}window.location = '{0}'{1}//-->{1}</SCRIPT>", HttpContext.Current.Session["CurrentSiteRef"], Environment.NewLine);
            //Page.ClientScript.Register(GetType(), "redirectScript", String.Format("window.location = '{0}'", HttpContext.Current.Session["CurrentSiteRef"]));
                     //   "EmergencySubmit", "getSize(); document.forms[0].submit();", true);
         //  Response.Write(String.Format("<script type='text/javascript'><!--{1}window.location = '{0}'{1}//--></script>", HttpContext.Current.Session["CurrentSiteRef"], Environment.NewLine));
         //   Response.End();
          //  Response.Redirect((string)HttpContext.Current.Session["CurrentSiteRef"], true);
           // InitializeIncomes();
           // InitializeOutcomes();
           // InitializeFonds();
           // InitializeBkku();
        }
    }

}
