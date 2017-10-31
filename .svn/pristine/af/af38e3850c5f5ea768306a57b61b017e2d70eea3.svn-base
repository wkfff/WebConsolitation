using System;
using System.Data;
using System.IO;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SE_0001_0002 : CustomReportPage
    {
        private int year = 2000;
        private int monthNum = 1;

        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/SE_0001_0002/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/SE_0001_0002/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"SE_0001_0007_{0}\" bounds=\"x=0;y=0;width=768;height=160\" openMode=\"loaded\"/><element id=\"SE_0001_0003_{0}\" bounds=\"x=0;y=160;width=380;height=390\" openMode=\"outcomes\"/><element id=\"SE_0001_0004_{0}\" bounds=\"x=380;y=160;width=380;height=390\" openMode=\"rests\"/><element id=\"SE_0001_0005_{0}\" bounds=\"x=0;y=550;width=380;height=390\" openMode=\"\"/><element id=\"SE_0001_0006_{0}\" bounds=\"x=380;y=550;width=380;height=390\" openMode=\"\"/></touchElements>", HttpContext.Current.Session["CurrentSubjectID"]));
            base.Page_Load(sender, e);

            InitializeDate();

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }
            HeraldImageContainer.InnerHtml =
                String.Format(
                    "<a href ='{1}' <img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/Heralds/{0}.png\"></a>",
                    HttpContext.Current.Session["CurrentSubjectID"], HttpContext.Current.Session["CurrentSiteRef"]);

            DataTable dtLoaded = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0002_loaded");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtLoaded);

            string grown = "прирост";
            string constitute = "составил";
            string img = "<img src='../../../images/arrowGreenUpIPad.png'";
            if (dtLoaded.Rows[0][1].ToString().Contains("-"))
            {
                grown = "снижение";
                constitute = "составило";
                img = "<img src='../../../images/arrowRedDownIPad.png'>";
            }

            string rankDescription =
                String.Format(
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Ранг {0}&nbsp;<span class='DigitsValue'>{2:N0}</span>&nbsp;{1}&nbsp;РФ&nbsp;<span class='DigitsValue'>{4:N0}</span>&nbsp;{3}",
                    RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("region").Value),
                    GetRankImgage(dtLoaded, 0, "Ранг по ФО", "Худший ранг ФО"), dtLoaded.Rows[0]["Ранг по ФО"],
                    GetRankImgage(dtLoaded, 0, "Ранг по РФ", "Худший ранг РФ"), dtLoaded.Rows[0]["Ранг по РФ"]);

            string partDescription = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Доля в {0}&nbsp;<span class='DigitsValue'>{1:P2}</span>&nbsp;в РФ&nbsp;<span class='DigitsValue'>{2:P2}</span>", RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("region").Value), dtLoaded.Rows[0][8], dtLoaded.Rows[0][9]);

            string period = monthNum == 1 ? "январь" : String.Format("январь-{0}", CRHelper.RusMonth(monthNum));

            Label1.Text =
                String.Format(
                    "За&nbsp;<span class='DigitsValue'>{0} {1} года в {8}</span>&nbsp;отгружено товаров собственного<br/>производства, выполнено работ и услуг собственным силами<br/>на сумму&nbsp;<span class='DigitsValue'>{2:N2} млн.руб.</span><br/>{3}{9}<br/>К аналогичному периоду предыдущего года&nbsp;<span class='DigitsValue'>{4}</span>&nbsp;{5}&nbsp;{6}&nbsp;<span class='DigitsValue'>&nbsp;{7:P1}</span>",
                    period, year, dtLoaded.Rows[0][1], rankDescription, grown, constitute, img,
                    dtLoaded.Rows[0][3],
                    RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("state_area").Value), partDescription);
        }

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0]);
            reportDate = new DateTime(year, monthNum, 1);

            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 4);
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 1);
            UserParams.PeriodEndYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-2), 1);
        }

        private static string GetRankImgage(DataTable dt, int row, string column, string worseColumn)
        {
            string img1 = String.Empty;
            if (dt.Rows[row][column] != DBNull.Value &&
                Convert.ToInt32(dt.Rows[row][column]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[row][column] != DBNull.Value &&
                     dt.Rows[row][worseColumn] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[row][column]) ==
                     Convert.ToInt32(dt.Rows[row][worseColumn]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }
            return img1;
        }
       
    }
}
