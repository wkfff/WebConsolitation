using System;
using System.IO;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0009_0001 : CustomReportPage
    {
        string subjectId = "0";

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DateTime previousDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "Oil_0009_0001_lastDate", false, 0);
            DateTime currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "Oil_0009_0001_lastDate", false, 1);
            DateTime lastYearDate = new DateTime(currentDate.Year - 1, 12, 30);

            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", previousDate, 5);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", currentDate, 5);
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", lastYearDate, 5);

            UltraChart1.ReportCode = "Oil_0009_0001";
            UltraChart2.ReportCode = "Oil_0009_0001";
            UltraChart3.ReportCode = "Oil_0009_0001";
            UltraChart4.ReportCode = "Oil_0009_0001";
            UltraChart5.ReportCode = "Oil_0009_0001";

            UltraChart1.ReportDate = currentDate;
            UltraChart2.ReportDate = currentDate;
            UltraChart3.ReportDate = currentDate;
            UltraChart4.ReportDate = currentDate;
            UltraChart5.ReportDate = currentDate;

            UltraChart1.PrevDate = previousDate;
            UltraChart2.PrevDate = previousDate;
            UltraChart3.PrevDate = previousDate;
            UltraChart4.PrevDate = previousDate;
            UltraChart5.PrevDate = previousDate;

            UltraChart1.LastYearDate = lastYearDate;
            UltraChart2.LastYearDate = lastYearDate;
            UltraChart3.LastYearDate = lastYearDate;
            UltraChart4.LastYearDate = lastYearDate;
            UltraChart5.LastYearDate = lastYearDate;

            UltraChart1.OilName = "Бензин марки АИ-80";
            UltraChart2.OilName = "Бензин марки АИ-92";
            UltraChart3.OilName = "Бензин марки АИ-95";
            UltraChart4.OilName = "Дизельное топливо";
            UltraChart5.OilName = "Бензин марки АИ-98";

            UltraChart1.OilId = "1";
            UltraChart2.OilId = "2";
            UltraChart3.OilId = "3";
            UltraChart4.OilId = "4";
            UltraChart5.OilId = "5";

            subjectId = "0";
            if (HttpContext.Current.Session["CurrentMOID"] != null)
            {
                subjectId = HttpContext.Current.Session["CurrentMOID"].ToString();
            }

            UltraChart1.SubjectId = subjectId;
            UltraChart2.SubjectId = subjectId;
            UltraChart3.SubjectId = subjectId;
            UltraChart4.SubjectId = subjectId;
            UltraChart5.SubjectId = subjectId;

            UltraChart1.MultiTouchReportCode = String.Format("Oil_0009_0002_mo={0}", subjectId);
            UltraChart2.MultiTouchReportCode = String.Format("Oil_0009_0002_02_mo={0}", subjectId);
            UltraChart3.MultiTouchReportCode = String.Format("Oil_0009_0002_03_mo={0}", subjectId);
            UltraChart4.MultiTouchReportCode = String.Format("Oil_0009_0002_04_mo={0}", subjectId);
            UltraChart5.MultiTouchReportCode = String.Format("Oil_0009_0002_05_mo={0}", subjectId);
        }
        
        protected override void Page_LoadComplete(object sender, EventArgs e)
        {
            base.Page_LoadComplete(sender, e);

            int control1Height = UltraChart1.ControlHeight;
            int control2Height = UltraChart2.ControlHeight;
            int control3Height = UltraChart3.ControlHeight;
            int control4Height = UltraChart4.ControlHeight;
            int control5Height = UltraChart5.ControlHeight;

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/Oil_0009_0001/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/Oil_0009_0001/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements>" +
    "<element id=\"oil_0009_0002_mo={0}\" bounds=\"x=0;y=0;width=768;height={1}\" openMode=\"\"/>" +
    "<element id=\"Oil_0009_0002_02_mo={0}\" bounds=\"x=0;y={1};width=768;height={2}\" openMode=\"\"/>" +
    "<element id=\"Oil_0009_0002_03_mo={0}\" bounds=\"x=0;y={2};width=768;height={3}\" openMode=\"\"/>" +
    "<element id=\"Oil_0009_0002_05_mo={0}\" bounds=\"x=0;y={3};width=768;height={4}\" openMode=\"\"/>" +
    "<element id=\"Oil_0009_0002_04_mo={0}\" bounds=\"x=0;y={4};width=768;height={5}\" openMode=\"\"/></touchElements>",
         subjectId, control1Height, control2Height, control3Height, control5Height, control4Height));
        }
    }
}
