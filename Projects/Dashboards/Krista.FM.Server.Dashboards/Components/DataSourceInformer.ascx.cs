using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class DataSourceInformer : UserControl
    {
        private string title = "Источник данных";
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

        private string Description = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            string sources = "Источник данных: ";
            string path = ConvertToLocalReportURL(Request.Url.LocalPath);
            DataSet dataSources = LoadDataSourcesStore();
            if (dataSources == null)
            {
                this.Visible = false;
                return;
            }
            DataTable dependents = dataSources.Tables["Dependents"];
            DataTable dataSource = dataSources.Tables["DataSource"];

            DataRow[] rows = dependents.Select(String.Format("ReportID='{0}'", path));
            if (rows.Length == 0)
            {
                this.Visible = false;
                return;
            }
            else
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    DataRow[] sourceRows = dataSource.Select(String.Format("ID='{0}'", rows[i]["DataSourceID"]));

                    sources += String.Format("{0}, ", sourceRows[0]["Name"]);
                    Description += String.Format("<span class=\"sectionTitle\" style=\"color: black\">&nbsp;&nbsp;&nbsp;{0}</span> {1}<br/><br/>", sourceRows[0]["Name"], sourceRows[0]["Description"]);
                    Description = String.Format("<span class=\"sectionBody\" style=\"color: black\">{0}</span>", Description);
                }
            }
            sources = sources.Trim(' ').Trim(',');
			sources = String.Format("{0} <span class='help_sp' onclick='showSource();' title='Источники данных' style=\"color: #1572b4; text-decoration: underline\">&gt;&gt;&gt;</span>", sources);
            lbSources.Text = sources;
        }

        private string ConvertToLocalReportURL(string currentReport)
        {
            int trimNum = Request.ApplicationPath.Length == 1 ? 1 : Request.ApplicationPath.Length + 1;
            return currentReport.Remove(0, trimNum);
        }

        private static DataSet LoadDataSourcesStore()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/DataSources.xml");
            if (File.Exists(filePath))
            {
                DataSet dataSources = new DataSet();
                dataSources.ReadXml(filePath, XmlReadMode.Auto);

                return dataSources;
            }
            return null;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

			double width = CRHelper.GetScreenWidth;
			if (width > CustomReportConst.minScreenWidth * 1.25)
			{
				width = CustomReportConst.minScreenWidth * 1.25;
			}
			if (width < CustomReportConst.minScreenWidth * 0.75)
			{
				width = CustomReportConst.minScreenWidth * 0.75;
			}
			int widthPx = Convert.ToInt32(width * 0.45);

			double height = CRHelper.GetScreenHeight;
			if (height > CustomReportConst.minScreenHeight * 1.25)
			{
				height = CustomReportConst.minScreenHeight * 1.25;
			}
			if (height < CustomReportConst.minScreenHeight * 0.75)
			{
				height = CustomReportConst.minScreenHeight * 0.75;
			}
			int heightPx = Convert.ToInt32(height * 0.35);

            StringBuilder scriptString = new StringBuilder();
            scriptString.Append("<script type='text/javascript' language='JavaScript'>\n");
            // определяем положение курсора
			scriptString.AppendFormat("var mCur;\nvar dataWidth = {0};\n", widthPx);
            scriptString.Append(
				@"    
                    document.onmousedown = function(e){mCur = mousePageX(e)};               
                    function mousePageX(e)
                    {
                      if (!e) e = window.event;

                      if (e.pageX)
                      {
                        x = e.pageX;
                      }
                      else if (e.clientX)
                      {
                        x = e.clientX + (document.documentElement.scrollLeft || document.body.scrollLeft) - document.documentElement.clientLeft;                        
                      }
                      if (x > (pageWidth / 2))
                      {
                        x = x - dataWidth + 15;
                      }
                      else
                      {
                        x = x - 30;
                      }
                      return x;   
                        
                    }");
			scriptString.AppendFormat("var wait_source = '<div class=\"helpTable\" style=\"width:{0}px; height:{1}px; margin-top: -{1}px; *margin-top: -{2}px; margin-left: mCurPlacepx;\">", widthPx, heightPx, heightPx+20);
			scriptString.AppendFormat("<div class=\"helpHeader\" style=\"cursor:pointer;\" onclick=\"showSource();\"><div class=\"helpHeaderRight\"/></div><span>&nbsp;{0}</span></div>", title);
			scriptString.Append("<div class=\"helpFrame\"><div class=\"helpFrameTop\"></div>");
			scriptString.AppendFormat("<div style=\"width: {1}px; height: {2}px; border-left: solid 1px #f3d19f;border-bottom: solid 1px #f3d19f;border-right: solid 1px #f3d19f; background-color: White; padding: 5px; overflow: auto; cursor: auto; \">{0}</div>", Description, widthPx - 22, heightPx - 49);
			scriptString.Append("</div></div>';");
            scriptString.Append(
                @"
	                function showSource ()
	                {                               
		                var div = document.getElementById('sourceDiv');
                        var overlay = document.getElementById('overlay1');
		                if (!div) 
                        {
			                div = document.createElement('div');
			                div.className = 'help';
			                div.setAttribute('id', 'sourceDiv');
		                }
		                if (div.style.display == '' || div.style.display == 'none') 
                        {
			                div.innerHTML = wait_source.replace('mCurPlace', mCur);			                
		                }
		                else if (div.style.display == 'block') 
                        {
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