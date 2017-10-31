using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Image=System.Web.UI.WebControls.Image;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class UFK_0017_0001_en_V : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            string query = DataProvider.GetQueryText("UFK_0017_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("UFK_0017_0001_V");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            dtGrid = ReverseRowsDataTable(dt);
           
            MakeRestTable();

            Label.Text = string.Format("data on {0}, {1} {2}", CRHelper.EnMonth(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][4], dtDate.Rows[0][0]);
            LabelDate.Text =
                string.Format("update on {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }
                
        private DataTable dtGrid = new DataTable();
        private DataTable dtDate = new DataTable();

        private DataTable ReverseRowsDataTable(DataTable dt)
        {
            DataTable resDt = dt.Clone();
            for (int i = dt.Rows.Count; i > 0; i--)
            {
                resDt.ImportRow(dt.Rows[i - 1]);
            }
            return resDt;
        }

     
        private void MakeRestTable()
        {
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.CssClass = "HtmlTableHeader";
            cell.RowSpan = 2;
            Label label = new Label();
            label.Text = "Date";
            cell.Width = 35;
            cell.Height = 40;
            cell.Controls.Add(label);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "HtmlTableHeader";
            label = new Label();
            label.Text = "Budget funds balance";
            cell.Controls.Add(label);
            cell.ColumnSpan = 3;
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);
            restsTable.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "HtmlTableHeader";
            label = new Label();
            label.Text = "Balance total,<br>thous. rub";
            label.CssClass = "DigitsValueSmall";
            cell.Width = 43;
            cell.Height = 40;
            cell.Controls.Add(label);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "HtmlTableHeader";
            label = new Label();
            label.Text = "Sum change,<br>thous. rub";
            label.CssClass = "DigitsValueSmall";
            cell.Controls.Add(label);
            cell.ColumnSpan = 2;
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);
            restsTable.Rows.Add(row);

            string month = string.Empty;

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                if (dtGrid.Rows[i][4].ToString() != month)
                {
                    month = dtGrid.Rows[i][4].ToString();
                    row = new TableRow();
                    cell = new TableCell();
                    cell.CssClass = "HtmlTable";
                    label = new Label();
                    label.Text = CRHelper.EnMonth(CRHelper.MonthNum(dtGrid.Rows[i][4].ToString()));
                    label.CssClass = "TableFont";
                    cell.ColumnSpan = 4;
                    cell.Controls.Add(label);
                    cell.Style["border-top"] = "#323232 3px solid";
                    cell.VerticalAlign = VerticalAlign.Middle;
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    row.Cells.Add(cell);
                    restsTable.Rows.Add(row);
                }

                row = new TableRow();
                cell = new TableCell();
                cell.CssClass = "HtmlTable";
                label = new Label();
                label.Text = dtGrid.Rows[i][0].ToString();
                label.CssClass = "TableFont";
                cell.Width = 40;
                cell.Controls.Add(label);
                cell.VerticalAlign = VerticalAlign.Middle;
                cell.HorizontalAlign = HorizontalAlign.Center;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.CssClass = "HtmlTable";
                label = new Label();
                
                label.Text = string.Format("{0:N2}", dtGrid.Rows[i][1]).Replace(",", ".");
                label.CssClass = "TableFont";
                cell.Width = 90;
                cell.Controls.Add(label);
                cell.VerticalAlign = VerticalAlign.Middle;
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.CssClass = "HtmlTable";
                double value;
                if (Double.TryParse(dtGrid.Rows[i][2].ToString(), out value))
                {
                    Image image = new Image();
                    image.ImageUrl = value > 0
                                         ? "~/images/ArrowUpGreen.png"
                                         : "~/images/ArrowDownRed.png";
                    cell.Controls.Add(image);
                }
                cell.Style["border-right-style"] = "none";
                cell.Width = 25;
                cell.VerticalAlign = VerticalAlign.Middle;
                cell.HorizontalAlign = HorizontalAlign.Right;

                row.Cells.Add(cell);
                cell = new TableCell();
                
                label = new Label();
                label.Text = string.Format("{0:N2}<br>", dtGrid.Rows[i][2]).Replace(",", ".");
                label.CssClass = "InformationText";
                cell.Controls.Add(label);
                
                label = new Label();
                label.Text = string.Format("{0:P2}", dtGrid.Rows[i][3]).Replace(",", ".");
                label.CssClass = "InformationText";
                cell.Controls.Add(label);
                cell.Style["border-left-style"] = "none";
                cell.Style["padding-right"] = "5px";
                cell.VerticalAlign = VerticalAlign.Middle;
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);

                restsTable.Rows.Add(row);
            }
        }

    }
}
