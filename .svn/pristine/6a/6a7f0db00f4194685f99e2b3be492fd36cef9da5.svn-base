using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPhone.IT_0002_0001.reports.iPhone.FO_0003_0003;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0003 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            DataTable dtPerson = LoadPersons();

            DataRow[] persons = dtPerson.Select(String.Format("territory = '{0}'", UserParams.StateArea.Value));

            for (int i = 0; i < persons.Length; i++ )
            {
                FO_0003_0003_Person person = (FO_0003_0003_Person)Page.LoadControl("~/iPadBricks/FO_0003_0003_Person.ascx");
                person.Person = persons[i];
                person.PersonNum = (i + 1).ToString();
                person.StateAreaId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
                PlaceHolder1.Controls.Add(person);
            }
        }

        private DataTable LoadPersons()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/iphone/fo_0003_0003/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["table"];
        }
    }
}
