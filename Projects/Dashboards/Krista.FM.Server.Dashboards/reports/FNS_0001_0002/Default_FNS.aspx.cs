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
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0002
{
    public partial class Default_FNS : Dashboards.CustomReportPage
    {
        private DataTable dt = new DataTable();
        private int headerHeight = 200;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyWidth = (int)Session["width_size"] - 40; 
            int dirtyHeight = (int)Session["height_size"] - 40;

            //uwGrid.DisplayLayout.ScrollBarView = ScrollBarView.Vertical;
            uwGrid.Width = dirtyWidth;
            uwGrid.Height = dirtyHeight - headerHeight;

            WebPanel.Width = 910;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                uwGrid.DataBind();
                //SetExpandHeightGrid(uwGrid);
                //SetExpandWidthGrid(uwGrid);
            }

            WebPanel.Expanded = false;
            Period.Height = 250;
            Region.Height = 250;
            Indicator.Height = 250;
            WebPanel.Height = 290;

            //SetExpandHeightGrid(uwGrid);
        }

        protected void uwGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходный источник", dt);

            uwGrid.DataSource = dt;
        }

        protected void uwGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            if (uwGrid.Columns.Count == 0)
            {
                return;
            }
            uwGrid.Columns[0].Width = 250;
            uwGrid.Columns[0].CellStyle.Wrap = true;
            uwGrid.Columns[2].Hidden = true;
            CRHelper.FormatNumberColumn(uwGrid.Columns[1], "N0");

            for (int i = 3; i < uwGrid.Columns.Count; i++)
            {
                switch (i)
                {
                    case 4:
                        {
                            uwGrid.Columns[i].Width = 100;
                            break;
                        }
                    case 10:
                        {
                            uwGrid.Columns[i].Width = 90;
                            break;
                        }
                    default:
                        {
                            uwGrid.Columns[i].Width = 95;
                            break;
                        }
                }
                CRHelper.FormatNumberColumn(uwGrid.Columns[i], "N2");
                uwGrid.Columns[i].HeaderText += ", руб.";
            }
        }

        protected void SubmitButton_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            uwGrid.DataBind();
            //SetExpandHeightGrid(uwGrid);
            //SetExpandWidthGrid(uwGrid);
        }

        protected void uwGrid_InitializeRow(object sender, RowEventArgs e)
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
                case "Группа":
                    {
                        e.Row.Style.Font.Size = 10;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Подгруппа":
                    {
                        e.Row.Style.Font.Size = 10;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = true;
                        break;
                    }
                case "Статья":
                    {
                        e.Row.Style.Font.Size = 8;
                        e.Row.Style.Font.Bold = true;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Подстатья":
                    {
                        e.Row.Style.Font.Size = 8;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Элемент подстатьи":
                    {
                        e.Row.Style.Font.Size = 8;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = true;
                        break;
                    }
                case "Элемент":
                    {
                        e.Row.Style.Font.Size = 6;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
            }
        }

        // Установление максимально возможной высоты грида
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

            double gridHeight = grid.DisplayLayout.Bands[0].HeaderStyle.Height.Value;

            if (grid.DisplayLayout.Rows.Count == 0)
            {
                grid.Height = (int)gridHeight + 10;
            }
            else
            {
                foreach (UltraGridRow row in grid.DisplayLayout.Rows)
                {
                    gridHeight += row.Height.Value;
                }

                grid.Height = (int)gridHeight;
            }
        }

        // Установление максимально возможной ширины грида
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

            double gridWidth = 25;
            if (grid.Columns.Count != 0)
            {
                foreach (UltraGridColumn column in grid.Columns)
                {
                    gridWidth += column.Width.Value;
                }

                grid.Width = (int)gridWidth;
            }
            else
            {
                grid.Width = (int)gridWidth;
            }
        }  


        protected void uwGrid_DataBound(object sender, EventArgs e)
        {
            SetExpandHeightGrid(uwGrid);
            //SetExpandWidthGrid(uwGrid);
        }
    }
}
