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
    public partial class FO_0032_0001_V : CustomReportPage
    {
        private DataTable dt = new DataTable();
        private DataTable dtDate = new DataTable();
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string query = DataProvider.GetQueryText("FO_0032_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            dt = new DataTable();

            UserParams.PeriodDayFO.Value = string.Format("{0}", dtDate.Rows[0][5]);
            UserParams.PeriodYear.Value =
                string.Format("[Период].[Год Квартал Месяц].[Данные всех периодов].[{0}]", dtDate.Rows[0][0]);
            UserParams.PeriodMonth.Value =
                string.Format("[Период].[Год Квартал Месяц].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]",
                dtDate.Rows[0][0], dtDate.Rows[0][1], dtDate.Rows[0][2], dtDate.Rows[0][3]);

            query = DataProvider.GetQueryText("FO_0032_0001_V");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);
             
            Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            LabelDate.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) * 100;
                }

                if (row[5] != DBNull.Value)
                {
                    row[5] = Convert.ToDouble(row[5]) * 100;
                }

                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) / 1000;
                }

                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) / 1000;
                }
                if (row[4] != DBNull.Value)
                {
                    row[4] = Convert.ToDouble(row[4]) / 1000;
                }
            }

            UltraWebGrid.DataBind();
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            UltraWebGrid.DataSource = dt;
        }

        protected void Grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            UltraWebGrid.DisplayLayout.GroupByBox.Hidden = true;

            if (UltraWebGrid.DisplayLayout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Header.Caption = "Группа";
            e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("% исп {0}", dtDate.Rows[0][3].ToString().ToLower());
            e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("План {0}", dtDate.Rows[0][3].ToString().ToLower());
            e.Layout.Bands[0].Columns[3].Header.Caption = string.Format("Факт {0}", dtDate.Rows[0][3].ToString().ToLower());
            e.Layout.Bands[0].Columns[4].Header.Caption = string.Format("{0} {1}", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            e.Layout.Bands[0].Columns[5].Header.Caption = string.Format("% исп {0}", dtDate.Rows[0][0]);
            e.Layout.Bands[0].Columns[6].Hidden = true;

            UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Font.Name = "Arial";
            UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            
            UltraWebGrid.DisplayLayout.Bands[0].Columns[0].Width = 155;
            UltraWebGrid.DisplayLayout.Bands[0].Columns[1].Width = 63;
            UltraWebGrid.DisplayLayout.Bands[0].Columns[2].Width = 83;
            UltraWebGrid.DisplayLayout.Bands[0].Columns[3].Width = 83;
            UltraWebGrid.DisplayLayout.Bands[0].Columns[4].Width = 83;
            UltraWebGrid.DisplayLayout.Bands[0].Columns[5].Width = 63;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[6].Value.ToString() == "(All)" || e.Row.Cells[6].Value.ToString() == "Уровень02")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }
        }
    }
}
