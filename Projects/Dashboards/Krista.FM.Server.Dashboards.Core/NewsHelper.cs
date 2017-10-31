using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.Core
{
    public static class NewsHelper
    {
        public static DataTable LoadNewsStore()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/WebNewsStore.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["WebNewsStore"];
        }

        public static HtmlTable GetNewsHtmlTable(DataTable newsStore)
        {
            HtmlTable htmlTable = new HtmlTable();

            foreach (DataRow row in newsStore.Rows)
            {
                AddNewsRow(htmlTable, row);
            }

            return htmlTable;
        }

        public static HtmlTable GetLastNewsHtmlTable(DataTable newsStore)
        {
            HtmlTable htmlTable = new HtmlTable();
            DataRow[] rows = newsStore.Select("", "ID DESC");

            for (int i = 0; i < rows.Length && i < 5; i++)
            {
                AddNewsRow(htmlTable, rows[i]);
            }
            AddArchiveLink(htmlTable);
            return htmlTable;
        }

        private static void AddArchiveLink(HtmlTable htmlTable)
        {
            HtmlTableRow htmlRow = new HtmlTableRow();
            HtmlTableCell htmlCell = new HtmlTableCell();

            htmlRow.Attributes.Add("margin-top", "15px");
            htmlCell.Attributes.Add("align", "right");

            HyperLink archiveLink = new HyperLink();
            archiveLink.SkinID = "HyperLink";
            archiveLink.NavigateUrl = "~/WebNews.aspx";
            archiveLink.Text = "Aрхив новостей";

            htmlCell.Controls.Add(archiveLink);
            htmlRow.Cells.Add(htmlCell);
            htmlTable.Rows.Add(htmlRow);
        }

        private static void AddNewsRow(HtmlTable htmlTable, DataRow row)
        {
            HtmlTableRow htmlRow = new HtmlTableRow();
            HtmlTableCell htmlCell = new HtmlTableCell();
            htmlRow.Attributes.Add("margin-top", "15px");

            Label title = new Label();
            title.Text = String.Format("{0}", row["title"]);
            title.CssClass = "ReportTitle";
            htmlCell.Controls.Add(title);

            Label date = new Label();
            date.Text = String.Format("{0}",
                                      Convert.ToDateTime(row["CreateDate"]).ToShortDateString()/*,
					Convert.ToDateTime(row["CreateDate"]).ToShortTimeString()*/
                );
            date.CssClass = "ReportCode";
            htmlCell.Controls.Add(date);

            htmlRow.Cells.Add(htmlCell);
            htmlTable.Rows.Add(htmlRow);

            htmlRow = new HtmlTableRow();
            htmlCell = new HtmlTableCell();

            Label shortBody = new Label();
            shortBody.Text = String.Format("{0}", row["ShortBody"]);
            shortBody.CssClass = "ReportDescription";
            htmlCell.Controls.Add(shortBody);


            htmlRow.Cells.Add(htmlCell);
            htmlTable.Rows.Add(htmlRow);
        }
    }
}
