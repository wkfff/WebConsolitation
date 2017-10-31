using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0004_H : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0004_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);
            int day = Convert.ToInt32(dtDate.Rows[0][4]);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.PeriodDayFO.Value = day.ToString();

            TextBox1.Text = string.Format("Фактически произведенные расходы на {0} {1} {2} года",
                                        day,
                                        CRHelper.RusMonthGenitive(monthNum),
                                        yearNum);


            Label1.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            Label2.Text = string.Format("данные на {0} {1} {2} года", day, CRHelper.RusMonthGenitive(monthNum), yearNum);

            UltraWebGrid1.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0004_h");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Виды расходов", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0] != DBNull.Value)
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("Первоочередные", "Первоочеред-ные");
                }
            }

            UltraWebGrid1.DataSource = dt;
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 5)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[2].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[3].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[4].CellStyle.Wrap = false;
                e.Layout.Bands[0].Columns[5].CellStyle.Wrap = false;

                e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[3].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[4].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[5].Header.Style.Font.Size = FontUnit.Parse("14px");

                e.Layout.Bands[0].Columns[0].Width = 110;
                e.Layout.Bands[0].Columns[1].Width = 70;
                e.Layout.Bands[0].Columns[2].Width = 70;
                e.Layout.Bands[0].Columns[3].Width = 70;
                e.Layout.Bands[0].Columns[4].Width = 70;
                e.Layout.Bands[0].Columns[5].Width = 80;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            }
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 3;
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString().ToLower().Contains("итого"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }
        #endregion
    }
}
