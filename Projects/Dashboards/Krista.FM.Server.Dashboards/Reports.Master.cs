using System;
using System.Configuration;
using System.Web.UI;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards
{
    public partial class ReportsMasterPage : MasterPage, IMasterPage
    {
        public Header SiteHeader
        {
            get
            {
                return Header1;
            }
        }

        private string twitterButtonText = "<a href=\"http://twitter.com/share\" class=\"twitter-share-button\" data-count=\"none\">Tweet</a><script type=\"text/javascript\" src=\"http://platform.twitter.com/widgets.js\"></script>";

        protected string currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = System.Web.HttpContext.Current.Session[CustomReportConst.currentUserKeyName].ToString();
            bool showTweetButton = ConfigurationManager.AppSettings["showTweetButton"] != null
                                   && ConfigurationManager.AppSettings["showTweetButton"].ToString().ToLower() == "true";

            if (Session["Embedded"] != null && (bool)Session["Embedded"])
            {
                this.Header1.Visible = false;
                showTweetButton = false;
            }
            string path = Request.Url.LocalPath.ToLower();
            

            if (!path.Contains("/index.aspx") && showTweetButton)
            {
                this.twitterButton.InnerHtml = twitterButtonText;
            }
            else
            {
                this.twitterButton.Visible = false;
            }
        }

        public void SetTwitterButtonText(string text)
        {
            twitterButtonText = text;
        }

        public void SetHeaderVisible(bool visible)
        {
            SiteHeader.Visible = visible;
        }
    }
}
