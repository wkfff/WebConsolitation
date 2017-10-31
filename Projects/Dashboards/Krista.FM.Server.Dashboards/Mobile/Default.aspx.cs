using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards
{
    public partial class Mobile : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            PresentationText.Text = ConfigurationManager.AppSettings["SitePresentation"] != null
                                       ? ConfigurationManager.AppSettings["SitePresentation"].Replace("App_Themes", "../app_themes")
                                       : String.Empty;

            GenerateNewsControls();

            GenerateContactInformationControls();
        }

        private void GenerateNewsControls()
        {
            DataTable news = NewsHelper.LoadNewsStore();

            if (news == null)
                return;

            Control container = Page.LoadControl("~/Components/ContainerPanel.ascx");
            ((ContainerPanel)container).AddContent(NewsHelper.GetLastNewsHtmlTable(news));
            ((ContainerPanel)container).AddHeader("Новости");
            ((ContainerPanel)container).AddHeaderImage("../images/WebNews.png");
            WebNewsPlaceHolder.Controls.Add(container);
        }

        private void GenerateContactInformationControls()
        {
            Control contactInformation = ContactInformationHelper.Instance.GetContactInformationControl();
            if (contactInformation != null)
                ContactInformationPlaceHolder.Controls.Add(contactInformation);
        }
    }
}
