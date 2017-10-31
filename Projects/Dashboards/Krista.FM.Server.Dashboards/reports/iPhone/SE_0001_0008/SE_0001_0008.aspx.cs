using System;
using System.Data;
using System.IO;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SE_0001_0008 : CustomReportPage
    {
        private int year = 2010;
        private int monthNum = 8;
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/SE_0001_0008/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/SE_0001_0008/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"SE_0002_0008\" bounds=\"x=0;y=0;width=760;height=950\" openMode=\"loaded\"/></touchElements>", HttpContext.Current.Session["CurrentSubjectID"]));

            IPadElementHeader1.MultitouchReport = String.Format("SE_0002_0008", HttpContext.Current.Session["CurrentSubjectID"]);
            IPadElementHeader2.MultitouchReport = String.Format("SE_0002_0008", HttpContext.Current.Session["CurrentSubjectID"]);
            IPadElementHeader3.MultitouchReport = String.Format("SE_0002_0008", HttpContext.Current.Session["CurrentSubjectID"]);

            InitializeDate();

            UltraChartSE_0001_0008_Chart1.DescriptionVisible = false;

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

            string period = monthNum == 1 ? "январь" : String.Format("январь-{0}", CRHelper.RusMonth(monthNum));
            
            Label1.Text =
                String.Format(
                    "За&nbsp;<span class='DigitsValue'>{0} {1} года в Российской федерации</span>&nbsp;отгружено товаров собственного производства, выполнено работ и услуг собственным силами на сумму&nbsp;<span class='DigitsValue'>{2:N2} млн.руб.</span><br/>По сравнению с аналогичным периодом предыдущего года&nbsp;<span class='DigitsValue'>{3}</span>&nbsp;{4}&nbsp;{5}&nbsp;<span class='DigitsValue'>&nbsp;{6:P1}</span>",
                   period, year, dtLoaded.Rows[0][1], grown, constitute, img,
                    dtLoaded.Rows[0][3]);
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

            DataTable dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("SE_0001_0008_index");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            string grown = "прирост";
            string constitute = "составил";
            string img = "<img src='../../../images/arrowGreenUpIPad.png'";
            
            if (dtIncomes.Rows[0][1].ToString().Contains("-"))
            {
                grown = "снижение";
                constitute = "составило";
                img = "<img src='../../../images/arrowRedDownIPad.png'>";
            }

            string period = monthNum == 1 ? "январь" : String.Format("январь-{0}", CRHelper.RusMonth(monthNum));

            lbDescription.Text =
                    String.Format(
                        "За&nbsp;<span class='DigitsValue'>{0} {1}</span>&nbsp;года&nbsp;<span class='DigitsValue'>{3}</span>&nbsp;индекса промышленного производства {4}<br/>{5}<span class='DigitsValue'>&nbsp;{2:N1}%</span> &nbsp; по сравнению с аналогичным периодом прошлого года.<br/>",
                        period, year, dtIncomes.Rows[0][1], grown, constitute, img);

            DataTable dtLosers = new DataTable();
            query = DataProvider.GetQueryText("SE_0001_0008_index_grown_leaders");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtLosers);

            lbGrownLeaders.Text = String.Format("<img src=\"../../../images/starYellow.png\">&nbsp;Лидеры по приросту:<br/>&nbsp;&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:N1}%</span><br/>&nbsp;&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:N1}%</span><br/>&nbsp;&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:N1}%</span>", dtLosers.Rows[0][0], dtLosers.Rows[0][1], dtLosers.Rows[1][0], dtLosers.Rows[1][1], dtLosers.Rows[2][0], dtLosers.Rows[2][1]);
            lbGrownLosers.Text = String.Format("<img src=\"../../../images/starGray.png\">&nbsp;Наибольшее снижение:<br/>&nbsp;&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:N1}%</span><br/>&nbsp;&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:N1}%</span><br/>&nbsp;&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:N1}%</span>", dtLosers.Rows[5][0], dtLosers.Rows[5][1], dtLosers.Rows[4][0], dtLosers.Rows[4][1], dtLosers.Rows[3][0], dtLosers.Rows[3][1]);
        }
    }
}
