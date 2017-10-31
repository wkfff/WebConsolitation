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
    public partial class FO_0035_0004_v : CustomReportPage
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

            Label3.Text = string.Format("Прогноз расходования средств областного бюджета на {0}&nbsp;{1}&nbsp;{2}&nbsp;года, тыс.руб.",
                            day,
                            CRHelper.RusMonthGenitive(monthNum),
                            yearNum);


            Label1.Text = string.Format("данные на {0} {1} {2} года", day, CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0004_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);
           
            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender,LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                e.Layout.Bands[0].Columns[0].Width = 190;
                e.Layout.Bands[0].Columns[1].Width = 110;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);

                if (i == 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 3;
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }
                else
                {
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.Padding.Left = 1;
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }
            }
        }

        #endregion
    }
}
