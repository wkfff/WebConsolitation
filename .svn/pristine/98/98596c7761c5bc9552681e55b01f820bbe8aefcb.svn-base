using System;
using System.Web.UI;

using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class ContainerPanel : UserControl, IContainerPanel
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(HeaderImage.Src))
            {
                HeaderImage.Visible = false;
            }
        }

        public void AddHeader(string text)
        {
            HeaderDiv.InnerHtml = text + "&nbsp;";
        }

        public void AddContent(Control control)
        {
            ContentPlaceHolder.Controls.Add(control);
        }

        public void AddHeaderImage(string source)
        {
            HeaderImage.Src = source;
            HeaderImage.Visible = true;
        }
    }
}