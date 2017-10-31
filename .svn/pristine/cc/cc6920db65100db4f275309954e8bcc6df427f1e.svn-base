using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;

namespace Krista.FM.Server.Dashboards.iPhone.IT_0002_0001.reports.iPhone.FO_0003_0003
{
    public partial class FO_0003_0003_Person : UserControl
    {
        private DataRow person;

        public DataRow Person
        {
            get { return person; }
            set { person = value; }
        }

        private string personNum;

        public string PersonNum
        {
            get { return personNum; }
            set { personNum = value; }
        }

        private string stateAreaId;

        public string StateAreaId
        {
            get { return stateAreaId; }
            set { stateAreaId = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            lbDepfinName.Text = "<b>" + person["depFinName"].ToString() + "</b>";

            HyperLinkSite.NavigateUrl = person["siteRef"].ToString();
            HyperLinkSite.Text = person["siteRefName"].ToString();

            lbDirector.Text = person["personPost"].ToString();
            lbFIO.Text = "<b>" + person["personName"].ToString() + "</b>";
            lbPhone.Text = "тел.: " + person["phone"].ToString();

            LabelMail.Text = String.Empty;

            string[] mails = person["mail"].ToString().Split(',');
            foreach (string mail in mails)
            {
                LabelMail.Text += String.Format("<a href='mailto:{0}'>{0}</a>,<br/>", mail);
            }
            LabelMail.Text = LabelMail.Text.TrimEnd('>').TrimEnd('/').TrimEnd('r').TrimEnd('b').TrimEnd('<').TrimEnd(',');
            if (person["appleId"] != DBNull.Value &&
                person["appleId"].ToString() != "")
            {
                HyperLinkFaceTime.NavigateUrl = "facetime://" + person["appleId"].ToString();
                HyperLinkFaceTime.Text = person["appleId"].ToString();
            }
            else
            {
                Label1.Visible = false;
                HyperLinkFaceTime.Visible = false;
            }

            if (System.IO.File.Exists(Server.MapPath(String.Format("~/images/finPersons/{0}_{1}.png", stateAreaId, personNum))))
            {
                Image1.ImageUrl = String.Format("../images/finPersons/{0}_{1}.png", stateAreaId, personNum);
            }
            else
            {
                Image1.ImageUrl = String.Format("../images/finPersons/noavatar.png", stateAreaId, personNum);
            }
        }
    }
}