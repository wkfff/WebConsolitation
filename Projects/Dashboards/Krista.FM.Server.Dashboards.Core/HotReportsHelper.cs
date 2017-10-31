using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Krista.FM.Server.Dashboards.Common;
using Image = System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.Core
{
    public interface IHotReport
    {
        int Width { get;}
    }

    public class HotReportsHelper
    {
        public static DataSet LoadHotReportsStore()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/HotReportsStore.xml");
            DataSet hotReports = new DataSet();
            hotReports.ReadXml(filePath, XmlReadMode.Auto);

            return hotReports;
        }

        /// <summary>
        /// Возвращает контрол с отчетом для отображения на главной странице.
        /// </summary>
        /// <param name="hotReports"></param>
        /// <returns></returns>
        public static Control GetCurrentHotReport(DataSet hotReports)
        {
            Page page = new Page();
            Control container = page.LoadControl("~/Components/ContainerPanel.ascx");
            // Ищем hot отчет
            DataRow[] rows = hotReports.Tables["Reports"].Select("ReportType = 'hot'");
            if (rows.Length > 0)
            {
                Panel panel = GetReportPanel(rows[0]);
                panel.Controls.Add(GetAllReportsLinkContainer());
                ((IContainerPanel) container).AddContent(panel);
                ((IContainerPanel) container).AddHeader(rows[0]["Title"].ToString());
                ((IContainerPanel) container).AddHeaderImage("../images/HotReport.png");
                return container;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает контрол с отчетом по идентификатору отчета.
        /// </summary>
        /// <param name="hotReports"></param>
        /// <param name="reportID"></param>
        /// <returns></returns>
        public static Control GetHotReport(DataSet hotReports, string reportID)
        {
            DataRow row = GetReportRow(hotReports, reportID);
            return GetReportPanel(row);
        }

        /// <summary>
        /// Возвращает контрол со всеми отчетами.
        /// </summary>
        /// <param name="hotReports"></param>
        /// <returns></returns>
        public static Control GetAllReports(DataSet hotReports)
        {
            Panel mainPanel = new Panel();
            Page page = new Page();
            foreach (DataRow row in hotReports.Tables["Reports"].Rows)
            {
                Control container = page.LoadControl("~/Components/ContainerPanel.ascx");
                Panel panel = GetReportPanel(row);
                AddGetCodeLabel(panel, row["ReportId"].ToString());
                ((IContainerPanel)container).AddContent(panel);
                ((IContainerPanel)container).AddHeader(row["Title"].ToString());
                mainPanel.Controls.Add(container);
            }
            return mainPanel;
        }

        public static Control GetEmbeddedPreview(DataSet hotReports, string reportID)
        {
            HtmlGenericControl container = new HtmlGenericControl("div");
            container.Style.Add("overflow", "visible");
            HtmlGenericControl previewDiv = new HtmlGenericControl("div");
            previewDiv.Style.Add("overflow", "visible");
            previewDiv.Style.Add("clear", "left");
            previewDiv.InnerHtml = GetSnippetText(reportID);
            container.Controls.Add(previewDiv);
            container.Controls.Add(GetCodeSnippet(reportID));
            return container;
        }

        private static Control GetCodeSnippet(string reportID)
        {
            HtmlGenericControl textArea = new HtmlGenericControl("textarea");
            textArea.Attributes.Add("rows", "8");
            textArea.Attributes.Add("cols", "50");
            textArea.Attributes.Add("readonly", "readonly");
            textArea.Attributes.Add("name", "snippetArea");
            string snipsetText = GetSnippetText(reportID);
            textArea.InnerText = snipsetText;
            textArea.EnableViewState = false;
            return textArea;
        }

        private static string GetSnippetText(string reportID)
        {
            string style = "style=\"width:600px; height: 460px; \"";
            return String.Format(
                        "<iframe frameborder=\"0\" scrolling=\"no\" {0} src='{1}?embedded=yes&hotReport={2}&paramlist=subjectId={3}'</iframe>",
                        style, GetHotReportPageAbsoluteUrl(), reportID, "87");
        }

        private static string GetHotReportPageAbsoluteUrl()
        {
            return String.Format("http://{0}{1}",
                HttpContext.Current.Request.Url.Authority,
                HttpContext.Current.Request.ApplicationPath).Trim('/') + "/" +
                "HotReports.aspx";
        }

        private static void AddGetCodeLabel(Panel panel, string reportID)
        {
            HtmlGenericControl linkDiv = new HtmlGenericControl("div");
            linkDiv.Style.Add("text-align", "right");
            linkDiv.Style.Add("width", panel.Width + "px");
            HyperLink getCode = new HyperLink();
            getCode.SkinID = "HyperLink";
            getCode.Text = "Код для встраивания";
            getCode.NavigateUrl = "~/EmbeddedReports.aspx?hotReport=" + reportID;
            linkDiv.Controls.Add(getCode);
            panel.Controls.Add(linkDiv);
        }

        private static Panel GetReportPanel(DataRow row)
        {
            Page page = new Page();
            Panel panel = new Panel();

            panel.Style.Add("margin-top", "5px");

            if (row["ReportType"].ToString() == "hot")
            {

            }
            else if (row["ReportType"].ToString() == "budgetMain")
            {
                HtmlGenericControl nobr = new HtmlGenericControl("nobr");
                panel.Style.Add("vertical-align", "top");
                Image img = new Image();
                img.ImageUrl = "~/images/sait.png";
                img.Style.Add("margin", "0 5px -2px 0");
                nobr.Controls.Add(img);
                HyperLink iMonitoring = GetIMonitoringRef();
                iMonitoring.ForeColor = Color.FromArgb(102, 102, 102);
                iMonitoring.Style.Add("font", "Arial");
                iMonitoring.Style.Add("font-size", "22px");
                iMonitoring.Style.Add("text-decoration", "none");
                iMonitoring.Style.Add("font-weight", "lighter");
                iMonitoring.Style.Add("line-height", "24px");
                iMonitoring.SkinID = "HyperLink";
                iMonitoring.Attributes.Add("target", "_blank");
                iMonitoring.Style.Add("margin-left", "5px");
                nobr.Controls.Add(iMonitoring);
                panel.Style.Add("margin-top", "-15px");
                panel.Controls.Add(nobr);
            }
            else if (row["ReportType"].ToString() == "budget")
            {
                panel.Style.Add("margin-top", "-25px");
            }
            else
            {
                HyperLink iMonitoring = GetIMonitoringRef();
                iMonitoring.SkinID = "HyperLink";
                panel.Controls.Add(iMonitoring);
            }

            Control ctrl = page.LoadControl(row["ReportPath"].ToString());
            panel.Width = ((IHotReport)ctrl).Width;
            panel.Controls.Add(ctrl);
            return panel;
        }

        private static HyperLink GetIMonitoringRef()
        {
            HyperLink iMonitoring = new HyperLink();
            iMonitoring.Attributes.Add("target", "_blank");
            string navigateUrl =
                String.Format("http://{0}{1}", HttpContext.Current.Request.Url.Authority,
                              HttpContext.Current.Request.ApplicationPath).Trim('/') + "/";
            iMonitoring.NavigateUrl = navigateUrl;
            iMonitoring.Text = ConfigurationManager.AppSettings["SiteName"] != null
                                   ? ConfigurationManager.AppSettings["SiteName"]
                                   : "iМониторинг";
            return iMonitoring;
        }

        private static DataRow GetReportRow(DataSet hotReports, string reportID)
        {
            DataTable reports = hotReports.Tables["Reports"];
            string filter = String.Format("ReportID='{0}'", reportID);
            DataRow[] rows = reports.Select(filter);
            return rows[0];
        }

        private static HtmlGenericControl GetAllReportsLinkContainer()
        {
            HtmlGenericControl linkDiv = new HtmlGenericControl("div");
            linkDiv.Style.Add("text-align", "right");
            HyperLink link = GetAllReportsLink();
            linkDiv.Controls.Add(link);
            return linkDiv;
        }

        private static HyperLink GetAllReportsLink()
        {
            HyperLink link = new HyperLink();
            link.Text = "Встраиваемые отчеты";
            link.SkinID = "HyperLink";
            link.NavigateUrl = "~/HotReports.aspx";
            return link;
        }
    }
}