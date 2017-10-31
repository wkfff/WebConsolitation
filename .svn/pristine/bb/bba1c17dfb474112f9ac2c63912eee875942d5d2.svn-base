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
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0033_0001_V : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string query = DataProvider.GetQueryText("FO_0033_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            DataTable dt = new DataTable();
            UserParams.PeriodDayFO.Value = string.Format("{0}", dtDate.Rows[0][5]);
            query = DataProvider.GetQueryText("FO_0033_0001_V");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Группа", dt);
            Label1.Text = string.Format("Остатки = остатки за {0}, тыс. руб.", dtDate.Rows[0][3].ToString().ToLower());
            Label2.Text = string.Format("% исп = процент исполнения за {0}", dtDate.Rows[0][3].ToString().ToLower());
            
            Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            LabelDate.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            foreach (DataRow row in dt.Rows)
            {
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) * 100;
                }
            }

            dtSource = dt;
            UltraWebGrid.DataBind();
        }

        private DataTable dtSource = new DataTable();
        private DataTable dtDate = new DataTable();

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            UltraWebGrid.DataSource = dtSource;
        }

        protected void Grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count == 3)
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = "Группа";
                e.Layout.Bands[0].Columns[1].Header.Caption = "Остатки";
                e.Layout.Bands[0].Columns[2].Header.Caption = "% исп";

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

                e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = FontUnit.Parse("18px");
                e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;

                e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = FontUnit.Parse("18px");
                e.Layout.Bands[0].Columns[2].CellStyle.Font.Bold = true;

                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].Width = 160;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].Width = 90;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[2].Width = 60;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            }
        }

        protected void Grid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                cell.Style.BorderColor = Color.FromArgb(50, 50, 50);
            }

            string backgroundImage = string.Empty;
            string customRules = string.Empty;

            if (Convert.ToDouble(e.Row.Cells[2].Value) > 100)
            {
                backgroundImage = "~/images/CornerRed.gif";
                customRules = "background-repeat: no-repeat; background-position: right top";
            }
            else{
                if (Convert.ToDouble(e.Row.Cells[2].Value) < 100)
                {
                    backgroundImage = "~/images/CornerGreen.gif";
                    customRules = "background-repeat: no-repeat; background-position: right top";
                }
                else
                {
                    if (Convert.ToDouble(e.Row.Cells[1].Value) == 0)
                    {
                        backgroundImage = "~/images/CornerGray.gif";
                        customRules = "background-repeat: no-repeat; background-position: right top";
                    }
                }
            }

            e.Row.Cells[1].Style.BackgroundImage = backgroundImage;
            e.Row.Cells[1].Style.CustomRules = customRules;
            e.Row.Cells[2].Style.BackgroundImage = backgroundImage;
            e.Row.Cells[2].Style.CustomRules = customRules;
        }
    }
}
