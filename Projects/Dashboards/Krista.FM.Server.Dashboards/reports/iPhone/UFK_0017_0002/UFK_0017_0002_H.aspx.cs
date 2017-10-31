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
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image=System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class UFK_0017_0002_V : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            string query = DataProvider.GetQueryText("UFK_0017_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFO.Value = dtDate.Rows[0][5].ToString();

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("UFK_0017_0002_V");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Трансферт", dtGrid);

            ConvertToThousands(dtGrid, 1);

            ultraWebGrid.DataBind();

            Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            LabelDate.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }
                
        private DataTable dtGrid = new DataTable();
        private DataTable dtDate = new DataTable();

        private void ConvertToThousands(DataTable dt, int colNum)
        {
            double value = 0;
            foreach(DataRow row in dt.Rows)
            {
                if(double.TryParse(row[colNum].ToString(), out value))
                {
                    row[colNum] = value/1000;
                }

            }
        }

        protected void ultraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[3].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[0].Width = 300;
            e.Layout.Bands[0].Columns[1].Width = 108;
            e.Layout.Bands[0].Columns[2].Width = 60;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Остаток";
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
        }

        protected void ultraWebGrid_DataBinding(object sender, EventArgs e)
        {
            ultraWebGrid.DataSource = dtGrid;
        }

        protected void ultraWebGrid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            int group = 0;
            if (int.TryParse(e.Row.Cells[3].Value.ToString(), out group))
            {
                switch(group)
                {
                    case (1):
                        {
                            e.Row.Cells[1].Style.BackgroundImage = "~/images/green.png";
                            break;
                        }
                    case (2):
                        {
                            e.Row.Cells[1].Style.BackgroundImage = "~/images/yellow.png";
                            break;
                        }
                    case (3):
                        {
                            e.Row.Cells[1].Style.BackgroundImage = "~/images/red.png";
                            break;
                        }
                }
                
            }
            e.Row.Cells[1].Style.CustomRules = "background-repeat: no-repeat; background-position: left; margin-left: 5px";
        }

    }
}
