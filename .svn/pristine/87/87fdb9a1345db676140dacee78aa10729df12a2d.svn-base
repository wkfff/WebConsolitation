using System;
using System.IO;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class FST_0003_0001 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DateTime currentDate = CubeInfoHelper.FstTariffsAndRegulationsInfo.LastDate;
            DateTime lastDate = new DateTime(currentDate.Year - 1, 12, 1);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]", currentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]", lastDate, 4);

            UltraChart1.ReportCode = "FST_0003_0001";
            UltraChart2.ReportCode = "FST_0003_0001";
            UltraChart3.ReportCode = "FST_0003_0001";
            UltraChart4.ReportCode = "FST_0003_0001";
            UltraChart5.ReportCode = "FST_0003_0001";
            UltraChart6.ReportCode = "FST_0003_0001";
            UltraChart7.ReportCode = "FST_0003_0001";

            UltraChart1.DetalizationReportCode = "FST_0003_0002_01";
            UltraChart2.DetalizationReportCode = "FST_0003_0002_02";
            UltraChart3.DetalizationReportCode = "FST_0003_0002_03";
            UltraChart4.DetalizationReportCode = "FST_0003_0002_04";
            UltraChart5.DetalizationReportCode = "FST_0003_0002_05";
            UltraChart6.DetalizationReportCode = "FST_0003_0002_06";
            UltraChart7.DetalizationReportCode = "FST_0003_0002_07";

            UltraChart1.ReportDate = currentDate;
            UltraChart2.ReportDate = currentDate;
            UltraChart3.ReportDate = currentDate;
            UltraChart4.ReportDate = currentDate;
            UltraChart5.ReportDate = currentDate;
            UltraChart6.ReportDate = currentDate;
            UltraChart7.ReportDate = currentDate;

            UltraChart1.LastDate = lastDate;
            UltraChart2.LastDate = lastDate;
            UltraChart3.LastDate = lastDate; 
            UltraChart4.LastDate = lastDate;
            UltraChart5.LastDate = lastDate;
            UltraChart6.LastDate = lastDate;
            UltraChart7.LastDate = lastDate;

            UltraChart1.ServiceName = "Водоснабжение";
            UltraChart2.ServiceName = "Водоотведение";
            UltraChart3.ServiceName = "Горячее водоснабжение";
            UltraChart4.ServiceName = "Электроснабжение";
            UltraChart5.ServiceName = "Отопление";
            UltraChart6.ServiceName = "Газоснабжение";
            UltraChart7.ServiceName = "Поставки твёрдого топлива при наличии печного отопления";

            UltraChart1.SubjectId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
            UltraChart2.SubjectId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
            UltraChart3.SubjectId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
            UltraChart4.SubjectId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
            UltraChart5.SubjectId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
            UltraChart6.SubjectId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
            UltraChart7.SubjectId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
        }

        protected override void Page_LoadComplete(object sender, EventArgs e)
        {
            base.Page_LoadComplete(sender, e);

            int control1Height = UltraChart1.ControlHeight;
            int control2Height = UltraChart2.ControlHeight;
            int control3Height = UltraChart3.ControlHeight;
            int control4Height = UltraChart4.ControlHeight;
            int control5Height = UltraChart5.ControlHeight;
            int control6Height = UltraChart6.ControlHeight;
            int control7Height = UltraChart7.ControlHeight;

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/FST_0003_0002/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/FST_0003_0002/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements>" +
    "<element id=\"FST_0003_0002_01_{7}\" bounds=\"x=0;y=0;width=768;height={0}\" openMode=\"\"/>" +
    "<element id=\"FST_0003_0002_02_{7}\" bounds=\"x=0;y={0};width=768;height={1}\" openMode=\"\"/>" +
    "<element id=\"FST_0003_0002_03_{7}\" bounds=\"x=0;y={1};width=768;height={2}\" openMode=\"\"/>" +
    "<element id=\"FST_0003_0002_04_{7}\" bounds=\"x=0;y={2};width=768;height={3}\" openMode=\"\"/>" +
    "<element id=\"FST_0003_0002_05_{7}\" bounds=\"x=0;y={3};width=768;height={4}\" openMode=\"\"/>" +
    "<element id=\"FST_0003_0002_06_{7}\" bounds=\"x=0;y={4};width=768;height={5}\" openMode=\"\"/>" +
    "<element id=\"FST_0003_0002_07_{7}\" bounds=\"x=0;y={5};width=768;height={6}\" openMode=\"\"/></touchElements>",
                    control1Height, control2Height, control3Height, control4Height, control5Height, control6Height, control7Height, HttpContext.Current.Session["CurrentSubjectID"]));
        }
    }
}
