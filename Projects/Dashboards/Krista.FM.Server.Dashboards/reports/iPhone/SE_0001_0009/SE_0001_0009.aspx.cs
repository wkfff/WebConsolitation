using System;
using System.Data;
using System.IO;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SE_0001_0009 : CustomReportPage
    {
        private int year = 2010;
        private int monthNum = 8;
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/SE_0001_0009/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/SE_0001_0009/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"SE_0002_0009_FO={0}\" bounds=\"x=0;y=0;width=760;height=950\" openMode=\"loaded\"/></touchElements>", HttpContext.Current.Session["CurrentFOID"]));

            IPadElementHeader1.MultitouchReport = String.Format("SE_0002_0009_FO={0}", HttpContext.Current.Session["CurrentFOID"]);
            IPadElementHeader2.MultitouchReport = String.Format("SE_0002_0009_FO={0}", HttpContext.Current.Session["CurrentFOID"]);
            IPadElementHeader3.MultitouchReport = String.Format("SE_0002_0009_FO={0}", HttpContext.Current.Session["CurrentFOID"]);

            UserParams.Region.Value = RegionsNamingHelper.FullName(UserParams.Region.Value);

            InitializeDate();
            
            UltraChartSE_0001_0009_Chart1.DescriptionVisible = false;

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

            string period = monthNum == 1 ? "€нварь" : String.Format("€нварь-{0}", CRHelper.RusMonth(monthNum));
            
            Label1.Text =
                String.Format(
                    "«а&nbsp;<span class='DigitsValue'>{0} {1} года в {7}</span>&nbsp;отгружено товаров собственного производства, выполнено работ и услуг собственным силами на сумму&nbsp;<span class='DigitsValue'>{2:N2} млн.руб.</span><br/>ѕо сравнению с аналогичным периодом предыдущего года&nbsp;<span class='DigitsValue'>{3}</span>&nbsp;{4}&nbsp;{5}&nbsp;<span class='DigitsValue'>&nbsp;{6:P1}</span>",
                    period, year, dtLoaded.Rows[0][1], grown, constitute, img,
                    dtLoaded.Rows[0][3], RegionsNamingHelper.ShortName(UserParams.Region.Value));
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
            query = DataProvider.GetQueryText("SE_0001_0009_index");
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

            string period = monthNum == 1 ? "€нварь" : String.Format("€нварь-{0}", CRHelper.RusMonth(monthNum));

            lbDescription.Text =
                    String.Format(
                        "«а&nbsp;<span class='DigitsValue'>{0} {1}</span>&nbsp;года&nbsp;<span class='DigitsValue'>{3}</span>&nbsp;индекса промышленного производства {4}<br/>{5}<span class='DigitsValue'>&nbsp;{2:N1}%</span> &nbsp; по сравнению с аналогичным периодом прошлого года.<br/>",
                        period, year, dtIncomes.Rows[0][1], grown, constitute, img);

            DataTable dtLosers = new DataTable();
            query = DataProvider.GetQueryText("SE_0001_0009_index_grown_leaders");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtLosers);

            lbGrownLeaders.Text = "<img src=\"../../../images/starYellow.png\">&nbsp;Ћидеры по приросту:<br/>&nbsp;&nbsp;";

            

            for (int i = 0; i < dtLosers.Rows.Count; i++)
            {
                if (!(dtLosers.Rows[i][1].ToString().Contains("-")))
                {
                    lbGrownLeaders.Text += String.Format("{0}&nbsp;<span class='DigitsValue'>{1:N1}%</span><br/>&nbsp;&nbsp;", dtLosers.Rows[i][0], dtLosers.Rows[i][1]);
                    
                }
            }


            lbGrownLosers.Text = "<img src=\"../../../images/starGray.png\">&nbsp;Ќаибольшее снижение:<br/>&nbsp;&nbsp;";

            bool isLoosers = false;
            for (int i = dtLosers.Rows.Count - 1; i >= 0; i--)
            {
                if (dtLosers.Rows[i][1].ToString().Contains("-"))
                {
                    isLoosers = true;
                    lbGrownLosers.Text += String.Format("{0}&nbsp;<span class='DigitsValue'>{1:N1}%</span><br/>&nbsp;&nbsp;", dtLosers.Rows[i][0], dtLosers.Rows[i][1]);
                }
            }
            lbGrownLosers.Visible = isLoosers;
        }
    }
}
