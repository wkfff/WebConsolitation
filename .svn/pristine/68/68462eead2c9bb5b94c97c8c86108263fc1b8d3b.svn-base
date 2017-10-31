using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0005
{
    public partial class DefaultOKVD : Dashboards.CustomReportPage
    {
        private DataTable dt = new DataTable();
        private int headerHeight = 200;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            MainIndexRef.HRef = Convert.ToString(Session["MainIndexRef"]);

            int dirtyWidth = (int)Session["width_size"] - 45;
            int dirtyHeight = (int)Session["height_size"] - 40;

            uwGrid.Width = dirtyWidth;
            uwGrid.Height = dirtyHeight - headerHeight;

            WebPanel.Width = 1025;
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
            WebPanel.Height = 290;
        }

        protected void uwGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("tableOKVD");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ОКВЭД", dt);

            uwGrid.DataSource = dt;
        }

        protected void uwGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].Width = 340;
            e.Layout.Bands[0].Columns[1].Width = 100;
            e.Layout.Bands[0].Columns[2].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = 120;
                e.Layout.Bands[0].Columns[i].HeaderText += (i <= 5) ? ", руб." : ", %";
            }
        }

        protected void uwGrid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            for (int i = 6; i <= 8; i++)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    double dValue = double.Parse(e.Row.Cells[i].Value.ToString());
                    e.Row.Cells[i].Value = dValue * 100;
                }
            }

            string level = e.Row.Cells[2].Value.ToString();
            switch (level)
            {
                case "(All)":
                    {
                        e.Row.Style.Font.Size = 10;
                        e.Row.Style.Font.Bold = true;
                        e.Row.Style.Font.Italic = false;
                        e.Row.Style.Font.Underline = true;
                        break;
                    }
                case "Раздел":
                    {
                        e.Row.Style.Font.Size = 10;
                        e.Row.Style.Font.Bold = true;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Подраздел":
                    {
                        e.Row.Style.Font.Size = 10;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Класс":
                    {
                        e.Row.Style.Font.Size = 8;  
                        e.Row.Style.Font.Bold = true;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Подкласс":
                    {
                        e.Row.Style.Font.Size = 8; 
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Группа":
                    {
                        e.Row.Style.Font.Size = 8; 
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = true;
                        break;
                    }
                case "Подгруппа":
                    {
                        e.Row.Style.Font.Size = 7; 
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
                case "Вид":
                    {
                        e.Row.Style.Font.Size = 7;
                        e.Row.Style.Font.Bold = false;
                        e.Row.Style.Font.Italic = false;
                        break;
                    }
            }
        }

        protected void SubmitButton_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            uwGrid.DataBind();

            //SetExpandWidthGrid(uwGrid);
            SetExpandHeightGrid(uwGrid);
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
