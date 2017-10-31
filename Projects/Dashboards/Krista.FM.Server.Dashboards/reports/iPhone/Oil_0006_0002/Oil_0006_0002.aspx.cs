using System;
using System.IO;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class OIL_0006_0002 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DateTime currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "oil_0006_0002_lastDate");
            DateTime lastDate = new DateTime(currentDate.Year - 1, 12, 30);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[День]", currentDate, 5);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период].[День]", lastDate, 5);

            UltraChart1.ReportCode = "oil_0006_0002";
            UltraChart2.ReportCode = "oil_0006_0002";
            UltraChart3.ReportCode = "oil_0006_0002";

            UltraChart1.ReportDate = currentDate;
            UltraChart2.ReportDate = currentDate;
            UltraChart3.ReportDate = currentDate;

            UltraChart1.LastDate = lastDate;
            UltraChart2.LastDate = lastDate;
            UltraChart3.LastDate = lastDate;

            UltraChart1.OilName = "Бензин марки АИ-92";
            UltraChart2.OilName = "Бензин марки АИ-95";
            UltraChart3.OilName = "Дизельное топливо";

            UltraChart1.OilId = "2";
            UltraChart2.OilId = "3";
            UltraChart3.OilId = "4";

            UltraChart1.ReportPrefix = "";
            UltraChart2.ReportPrefix = "_02";
            UltraChart3.ReportPrefix = "_03";

            UltraChart1.FoId = HttpContext.Current.Session["CurrentFOID"].ToString();
            UltraChart2.FoId = HttpContext.Current.Session["CurrentFOID"].ToString();
            UltraChart3.FoId = HttpContext.Current.Session["CurrentFOID"].ToString();
        }

        protected override void Page_LoadComplete(object sender, EventArgs e)
        {
            base.Page_LoadComplete(sender, e);

            int control1Height = UltraChart1.ControlHeight;
            int control2Height = UltraChart2.ControlHeight;
            int control3Height = UltraChart3.ControlHeight;

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/oil_0006_0002/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/oil_0006_0002/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements>" +
    "<element id=\"oil_0006_0004_fo={3}\" bounds=\"x=0;y=0;width=768;height={0}\" openMode=\"\"/>" +
    "<element id=\"oil_0006_0004_02_fo={3}\" bounds=\"x=0;y={0};width=768;height={1}\" openMode=\"\"/>" +
    "<element id=\"oil_0006_0004_03_fo={3}\" bounds=\"x=0;y={1};width=768;height={2}\" openMode=\"\"/></touchElements>",
    control1Height, control2Height, control3Height, HttpContext.Current.Session["CurrentFOID"]));
        }
    }
}
