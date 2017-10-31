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
    public partial class FO_0033_0001_H : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string query = DataProvider.GetQueryText("FO_0033_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            DataTable dt = new DataTable();
            UserParams.PeriodMonthFO.Value = string.Format("[Период].[Месяц Бюджет].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}].PrevMember",
                    dtDate.Rows[0][0], dtDate.Rows[0][1], dtDate.Rows[0][2], dtDate.Rows[0][3]);
            UserParams.PeriodDayFO.Value = string.Format("{0}", dtDate.Rows[0][5]);
            UserParams.PeriodYQM_Quarter.Value =
                string.Format("[Период].[Год Квартал Месяц].[Данные всех периодов].[{0}]", dtDate.Rows[0][0]);
            query = DataProvider.GetQueryText("FO_0033_0001");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            int gaudeEndValue = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dt.Rows[0][4]) * 100 / 50)) * 50;
            gaudeEndValue = gaudeEndValue < 100 ? 100 : gaudeEndValue;

            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Axis.SetEndValue(gaudeEndValue);
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].Value = Convert.ToDouble(dt.Rows[0][4]) * 100;
                        
            DataColumn col = new DataColumn("Name", typeof(string));
            dtSource.Columns.Add(col);
            col = new DataColumn("Value", typeof(string));
            dtSource.Columns.Add(col);
            for (int i = 0; i < 7; i++)
            {
                DataRow row = dtSource.NewRow();
                dtSource.Rows.Add(row);
            }
            
            Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            LabelDate.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            lbExecuted.Text = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][4]) * 100).ToString("N"));
            char percentGroupSeparator = CultureInfo.CurrentCulture.NumberFormat.PercentGroupSeparator[0];
            dtSource.Rows[0][1] = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][0])).ToString("N").Replace(percentGroupSeparator, '\''));
            dtSource.Rows[1][1] = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][1])).ToString("N").Replace(percentGroupSeparator, '\''));
            dtSource.Rows[2][1] = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][2])).ToString("N").Replace(percentGroupSeparator, '\''));
            dtSource.Rows[3][1] = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][3])).ToString("N").Replace(percentGroupSeparator, '\''));
            dtSource.Rows[4][1] = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][5])).ToString("N").Replace(percentGroupSeparator, '\''));
            dtSource.Rows[5][1] = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][6])).ToString("N").Replace(percentGroupSeparator, '\''));
            dtSource.Rows[6][1] = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][7])).ToString("P").Replace(percentGroupSeparator, '\''));

            dtSource.Rows[0][0] = String.Format("План на {0}", CRHelper.RusMonth(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            dtSource.Rows[1][0] = String.Format("Всего за {0}", CRHelper.RusMonth(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            dtSource.Rows[2][0] = String.Format("В том числе {0} {1}", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            dtSource.Rows[3][0] = String.Format("Остатки на {0}", CRHelper.RusMonth(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            dtSource.Rows[4][0] = String.Format("План на {0} год", dtDate.Rows[0][0]);
            dtSource.Rows[5][0] = String.Format("Всего за {0} год", dtDate.Rows[0][0]);
            dtSource.Rows[6][0] = String.Format("Исполнено за {0} год", dtDate.Rows[0][0]);

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
            UltraWebGrid.DisplayLayout.GroupByBox.Hidden = true;

            if (UltraWebGrid.DisplayLayout.Bands.Count == 0)
                return;

            if (UltraWebGrid.DisplayLayout.Bands[0].Columns.Count == 2)
            {
                UltraWebGrid.DisplayLayout.Bands[0].HeaderLayout.Clear();

                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Wrap = true;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.Wrap = true;
                
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.BackColor = Color.Black;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.BackColor = Color.Black;
                
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Font.Name = "Arial";
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.Font.Name = "Arial";
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.Font.Size = FontUnit.Parse("16px");
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.ForeColor = Color.White;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].Width = 188;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].Width = 121;
            }
        }

        protected void Grid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                cell.Style.BorderColor = Color.FromArgb(50, 50, 50);
            }
            if (e.Row.Index == 3)
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.BorderDetails.WidthBottom = 3;
                }
            }
        }
    }
}
