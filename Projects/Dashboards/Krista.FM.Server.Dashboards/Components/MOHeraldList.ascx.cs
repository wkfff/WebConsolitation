using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class MOHeraldList : UserControl
    {
        private string regionName = "Krasnodar";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadMoXml()
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/MO.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "MOList")
                {
                    foreach (XmlNode subjectNode in rootNode.ChildNodes)
                    {
                        if (subjectNode.Attributes != null && subjectNode.Attributes["name"].Value == regionName)
                        {
                            foreach (XmlNode moNode in subjectNode.ChildNodes)
                            {
                                if (moNode.Attributes != null && moNode.Attributes["id"] != null && moNode.Attributes["name"] != null)
                                {
                                    string name = moNode.Attributes["name"].Value;
                                    string id = moNode.Attributes["id"].Value;
                                    AddMoTable(name, id);

                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddMoTable(string moName, string moId)
        {
            Table table = new Table();
            TableRow row = new TableRow();

            TableCell imageCell = new TableCell();
            imageCell.Text = String.Format(
                    @"<a href='reports/FO_0002_0001/Default.aspx?paramlist=filter={0}' title='{0}'>
                         <img border='0' style='float: left; padding: 2px; padding-right: 10px' alt='{0}' src='App_Themes\Krasnodar\MoHeralds\{1}.png' />
                      </a>", moName, moId);
            row.Cells.Add(imageCell);

            TableCell descriptionCell = new TableCell();
            descriptionCell.CssClass = "ReportDescription";
            row.Cells.Add(descriptionCell);
            descriptionCell.Text = String.Format(
                    @"<a href='reports/FO_0002_0001/Default.aspx?paramlist=filter={0}' title='{0}'>{0}</a>", moName);
            table.Rows.Add(row);
            
            MoControlList.Controls.Add(table);
        }
    }
}