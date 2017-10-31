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
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0003
{
    public partial class Default : Dashboards.CustomReportPage
    {
        private DataTable dt = new DataTable();
        private int headerHeight = 200;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyWidth = (int)Session["width_size"] - 45;
            int dirtyHeight = (int)Session["height_size"] - 40;

            uwGrid.Width = dirtyWidth;
            uwGrid.Height = dirtyHeight - headerHeight;

            WebPanel.Width = 1020;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                uwGrid.DataBind();
                SetExpandHeightGrid(uwGrid);
                //SetExpandWidthGrid(uwGrid);
            }

            WebPanel.Expanded = false;
            WebPanel.Height = 340;
        }

        protected void uwGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Районы", dt);

            uwGrid.DataSource = dt;
        }

        protected void uwGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].Width = 150;
            e.Layout.Bands[0].Columns[1].Width = 55;
            e.Layout.Bands[0].Columns[3].Width = 140;
            e.Layout.Bands[0].Columns[3].HeaderText += ", руб.";

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[2].Hidden = true;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
        }

        protected void SubmitButton_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            uwGrid.DataBind();

            //SetExpandWidthGrid(uwGrid);
            SetExpandHeightGrid(uwGrid);
        }

        protected void uwGrid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            string level = e.Row.Cells[2].Value.ToString();
            switch (level)
            {
                case "(All)":
                    {
                        e.Row.Style.Font.Size = 10;
                        e.Row.Style.Font.Bold = true;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Районы":
                    {
                        e.Row.Style.Font.Size = 8;
                        e.Row.Style.Font.Bold = true;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Поселения":
                    {
                        e.Row.Style.Font.Size = 8;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
             }
        }

        // Устанавление максимально возможной высоты грида
        private static void SetExpandHeightGrid(UltraWebGrid grid)
        {
            if (grid == null)
            {
                return;
            }

            if (grid.DataSource == null)
            {
                return;
            }

            double gridHeight = grid.Bands[0].HeaderStyle.Height.Value;

            if (grid.Rows.Count == 0)
            {
                grid.Height = (int)gridHeight + 10;
            }
            else
            {
                foreach (UltraGridRow row in grid.Rows)
                {
                    gridHeight += row.Height.Value;
                }

                grid.Height = (int)gridHeight;
            }
        }

        // Устанавление максимально возможной ширины грида
        private static void SetExpandWidthGrid(UltraWebGrid grid)
        {
            if (grid == null)
            {
                return;
            }

            if (grid.DataSource == null)
            {
                return;
            }

            double gridWidth = 50;
            if (grid.Columns.Count != 0)
            {
                foreach (UltraGridColumn column in grid.Columns)
                {
                    if (!column.Hidden)
                    {
                        gridWidth += column.Width.Value;
                    }
                }

                grid.Width = (int)gridWidth;
            }
            else
            {
                grid.Width = (int)gridWidth + 10;
            }
        }
    }
}
