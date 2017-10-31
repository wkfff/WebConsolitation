using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Core
{
    public class DocumentsHelper
    {
        public static DataTable LoadDocumentsStore()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/DocumentsStore.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["DocumentsStore"];
        }

        public static Control GetDocumentsPanel(DataTable documentsStore)
        {
            Control container = new Page().LoadControl("~/Components/ContainerPanel.ascx");
            ((IContainerPanel)container).AddContent(GetDocumentsHtmlTable(documentsStore));
            ((IContainerPanel)container).AddHeader("Документы");
          //  ((IContainerPanel)container).AddHeaderImage("../images/WebNews.png");
            
            return container;
        }

        public static HtmlTable GetDocumentsHtmlTable(DataTable documentsStore)
        {
            HtmlTable htmlTable = new HtmlTable();

            foreach (DataRow row in documentsStore.Rows)
            {
                AddDocumentRow(htmlTable, row);
            }

            return htmlTable;
        }
        
        private static void AddDocumentRow(HtmlTable htmlTable, DataRow row)
        {
            HtmlTableRow htmlRow = new HtmlTableRow();
            HtmlTableCell htmlCell = new HtmlTableCell();
            htmlRow.Attributes.Add("margin-top", "15px");

            HyperLink link = new HyperLink();
            link.NavigateUrl = link.ResolveUrl("~/" + row["FileLink"]);
            link.Text = row["FileTitle"].ToString();
            link.CssClass = "ReportTitle";
            htmlCell.Controls.Add(link);
            
            Label date = new Label();
            date.Text = String.Format("{0}",
                                      Convert.ToDateTime(row["CreateDate"]).ToShortDateString());
            date.CssClass = "ReportCode";
            htmlCell.Controls.Add(date);

            htmlRow.Cells.Add(htmlCell);
            htmlTable.Rows.Add(htmlRow);

            htmlRow = new HtmlTableRow();
            htmlCell = new HtmlTableCell();

            Label description = new Label();
            description.Text = String.Format("{0}", row["Description"]);
            description.CssClass = "ReportDescription";
            htmlCell.Controls.Add(description);

            htmlRow.Cells.Add(htmlCell);
            htmlTable.Rows.Add(htmlRow);
        }
    }
}
