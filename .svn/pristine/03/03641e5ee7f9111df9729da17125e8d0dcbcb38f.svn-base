using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.Dashboards
{
    public partial class IndexPage : CustomReportPage
    {
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                Title = ConfigurationManager.AppSettings["SiteName"];
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                PresentationText.Text = ConfigurationManager.AppSettings["SitePresentation"] != null
                                       ? ConfigurationManager.AppSettings["SitePresentation"]
                                       : String.Empty;

                if (Session["IsWebAdministrator"] != null &&
                    (bool)Session["IsWebAdministrator"] &&
                    metrikaContainer != null)
                {
                    metrikaContainer.Visible = true;
                }

                GenerateIndexControls();

                GenerateNewsControls();

                GenerateHotReportControls();

                GenerateContactInformationControls();

                GenerateDocumentsControls();
            }
            catch (Exception ex)
            {
                CRHelper.SaveToErrorLog(CRHelper.GetExceptionInfo(ex));
            }
        }
        
        private void GenerateIndexControls()
        {
            DataTable reports = ConfigurationManager.AppSettings["ShowAllReports"] != null &&
                                ConfigurationManager.AppSettings["ShowAllReports"].ToString().ToLower() == "true"
                                    ? AllReports
                                    : AllowedReports;
            if (reports == null)
            {
                return;
            }

            HtmlGenericControl control = new HtmlGenericControl("div");
            control.Controls.Add(IndexPageHelper.GetReportTable(reports, TemplateTypes.Web));

            if (AllowedReportsIPhone != null &&
                AllowedReportsIPhone.Rows.Count > 0)
            {
                control.Controls.Add(GetIPhoneIndexPageLink());
            }

            Control container = Page.LoadControl("~/Components/ContainerPanel.ascx");
            ((ContainerPanel)container).AddContent(control);
            ((ContainerPanel)container).AddHeader("Аналитические отчеты");
            ((ContainerPanel)container).AddHeaderImage("../images/Reports.png");

            ReportsPlaceHolder.Controls.Add(container);
        }

        private static HtmlTable GetIPhoneIndexPageLink()
        {
            HtmlTable htmlTable = new HtmlTable();
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();
            HyperLink link = new HyperLink();
            link.NavigateUrl = link.ResolveUrl("~/reports/iphone/index.aspx");
            link.Text = "Отчеты смартфонов и планшетов";
            link.CssClass = "ReportTitle";
            cell.Controls.Add(link);
            row.Cells.Add(cell);
            htmlTable.Rows.Add(row);
            return htmlTable;
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

        private void GenerateHotReportControls()
        {
            DataSet hotReports = HotReportsHelper.LoadHotReportsStore();
            if (hotReports == null ||
                hotReports.Tables["Reports"] == null)
            {
                return;
            }
            else
            {
                Control hotReport = HotReportsHelper.GetCurrentHotReport(hotReports);
                if (hotReport != null)
                {
                    HotReportsPlaceHolder.Controls.Add(hotReport);
                }
            }
        }

        private void GenerateContactInformationControls()
        {
            Control contactInformation = ContactInformationHelper.Instance.GetContactInformationControl();
            if (contactInformation != null)
                ContactInformationPlaceHolder.Controls.Add(contactInformation);
        }

        private void GenerateDocumentsControls()
        {
            bool openDocuments = ConfigurationManager.AppSettings["OpenDocuments"] != null
                                   && ConfigurationManager.AppSettings["OpenDocuments"].ToString().ToLower() == "true";
            if (openDocuments || !LoginedAsGuest())
            {
                DataTable documents = DocumentsHelper.LoadDocumentsStore();
                if (documents != null)
                {
                    DocumentsPlaceHolder.Controls.Add(DocumentsHelper.GetDocumentsPanel(documents));
                }
            }
        }
    }
}
