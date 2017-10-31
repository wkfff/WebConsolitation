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

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class iPadElementHeader : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string multitouchIcon = String.Empty;
            if (multitouch)
            {
                multitouchIcon = "<img src='../../../images/detail.png'>";
                detalizationIconDiv.InnerHtml = String.Format("<a href='webcommand?showPinchReport={2}_{1}&width=690&height=530&fitByHorizontal=true'>{0}</a>", multitouchIcon, HttpContext.Current.Session["CurrentSubjectID"], detalizationReportId);
            }            

            if (!String.IsNullOrEmpty(multitouchReport))
            {
                multitouchIcon = "<img src='../../../images/detail.png'>";
                detalizationIconDiv.InnerHtml = String.Format("<a href='webcommand?showPinchReport={1}'>{0}</a>", multitouchIcon, multitouchReport);
            }

            if (!String.IsNullOrEmpty(hrefReport))
            {
                multitouchIcon = "<img src='../../../images/detail.png'>";
                detalizationIconDiv.InnerHtml = String.Format("<a href='webcommand?showReport={1}'>{0}</a>", multitouchIcon, hrefReport);
            }
        }

        public string Text
        {
            get { return elementCaption.Text; }
            set { elementCaption.Text = value; }
        }

        private string detalizationReportId; 
        public string DetalizationReportId
        {
            get { return detalizationReportId; }
            set { detalizationReportId = value; }
        }

        private bool multitouch = false;

        public bool Multitouch
        {
            get { return multitouch; }
            set { multitouch = value; }
        }

        private string multitouchReport = String.Empty;

        public string MultitouchReport
        {
            get { return multitouchReport; }
            set { multitouchReport = value; }
        }

        private string hrefReport = String.Empty;

        public string HrefReport
        {
            get { return hrefReport; }
            set { hrefReport = value; }
        }

        public string Width
        {
            get
            {
                if (headerTable.Style["width"] != null)
                {
                    return headerTable.Style["width"];
                }
                else
                {
                    return "0px";
                }
            }
            set
            {
                headerTable.Style.Add("width", value);
            }
        }
    }
}