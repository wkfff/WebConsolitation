using System;
using System.Text;
using System.Web.UI;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class PopupInformer : UserControl
    {
        private string title = "Справка";
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        private string helpPageUrl = "help.html";
        public string HelpPageUrl
        {
            get
            {
                return helpPageUrl;
            }
            set
            {
                helpPageUrl = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PrintVersion"] != null && (bool)Session["PrintVersion"])
            {
                this.Visible = false;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        	
			double width = CRHelper.GetScreenWidth;
			if (width > CustomReportConst.minScreenWidth*1.25)
			{
				width = CustomReportConst.minScreenWidth*1.25;
			}
			if (width < CustomReportConst.minScreenWidth*0.75)
			{
				width = CustomReportConst.minScreenWidth*0.75;
			}
        	int widthPx = Convert.ToInt32(width*0.5);

        	double height = CRHelper.GetScreenHeight;
			if (height > CustomReportConst.minScreenHeight*1.25)
			{
				height = CustomReportConst.minScreenHeight*1.25;
			}
			if (height < CustomReportConst.minScreenHeight*0.75)
			{
				height = CustomReportConst.minScreenHeight*0.75;
			}
        	int heightPx = Convert.ToInt32(height*0.5);

            StringBuilder scriptString = new StringBuilder();
            scriptString.Append("<script type='text/javascript' language='JavaScript'>\n");
			scriptString.AppendFormat("var wait_help = '<div class=\"helpTable\" style=\"width:{0}px; height:{1}px;\">", widthPx, heightPx);
			scriptString.AppendFormat("<div class=\"helpHeader\"><div class=\"helpHeaderLeft\"></div><div class=\"helpHeaderRight\"/></div>&nbsp;{0}</div>", title);
			scriptString.Append("<div class=\"helpFrame\"><div class=\"helpFrameTop\"></div>");
			scriptString.AppendFormat("<iframe src=\"{0}\" frameborder=\"no\" width=\"{1}\" height=\"{2}\" style=\"border-left: solid 1px #f3d19f;border-bottom: solid 1px #f3d19f;border-right: solid 1px #f3d19f;\"></iframe>", helpPageUrl, widthPx-12, heightPx-39);
			scriptString.Append("</div></div>';");
            scriptString.Append(
                @"
	                function help (obj)
	                {
		                var div = document.getElementById('help');
                        var overlay = document.getElementById('overlay');
		                if (!div) 
                        {
			                div = document.createElement('div');
			                div.className = 'help';
			                div.setAttribute('id', 'help');
		                }
		                if (div.style.display == '' || div.style.display == 'none') 
                        {
			                div.innerHTML = wait_help;
			                obj.appendChild(div);
		                }
		                else if (div.style.display == 'block') {
			                div.style.display = 'none';
                			overlay.style.display = 'none';
			                return;
		                }
		                div.style.display = 'block';
                        overlay.style.display = 'block';
	                }
	                </script>");
            writer.Write(scriptString.ToString());
        }
    }
}