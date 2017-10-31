using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0003_Horizontal : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            DataTable dtPerson = LoadPersons();

            DataRow[] persons = dtPerson.Select("territory='Амурская область'");

            DataRow person = persons[0];

            lbDirector.Text = person["personPost"].ToString();
            HyperLinkSite.NavigateUrl = person["siteRef"].ToString();
            HyperLinkSite.Text = person["siteRef"].ToString();
            lbFIO.Text = person["personName"].ToString();
            lbPhone.Text = person["phone"].ToString();
            HyperLinkMail.NavigateUrl = "mailto:" + person["mail"].ToString();
            HyperLinkMail.Text = person["mail"].ToString();

            Image1.ImageUrl = String.Format("../../../images/finPersons/{0}", person["photo"]);


        }

        private DataTable LoadPersons()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/iphone/FO_0003_0003_Horizontal/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["table"];
        }
    }
}
