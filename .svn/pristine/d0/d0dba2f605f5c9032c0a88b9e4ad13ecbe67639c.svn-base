using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0042_0002 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0042_0002_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            int quarterNum = Convert.ToInt32(dtDate.Rows[0][2].ToString().Replace("Квартал ", string.Empty));
            string quarter = dtDate.Rows[0][2].ToString();

            date = new DateTime(endYear, CRHelper.QuarterLastMonth(quarterNum), 1);
            
            UserParams.PeriodYear.Value = endYear.ToString();
            UserParams.PeriodHalfYear.Value = dtDate.Rows[0][1].ToString();
            UserParams.PeriodQuater.Value = quarter;

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            GRBSGrid.Width = 730;
            GRBSGrid.Height = Unit.Empty;
            GRBSGrid.DisplayLayout.NoDataMessage = "Нет данных";
            
            GRBSGrid.DataBind();
        }

        

        #region Обработчики грида

        protected void GRBSGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0002_GRBSGrid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtGrid);
            foreach (DataRow dataRow in dtGrid.Rows)
            {
                dataRow[0] = dataRow[0].ToString().Replace("Ханты-Мансийского автономного округа – Югры", "ХМАО-Югры").Replace("Ханты-Мансийскому автономному округу – Югре", "ХМАО-Югре");
            }

            GRBSGrid.DataSource = dtGrid;
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 600;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 115;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell("Наименование показателя");
            headerLayout.AddCell("Оценка показателя");

            headerLayout.ApplyHeaderInfo();
        }

        protected void GRBSGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].Value != null &&
                e.Row.Cells[1].Value.ToString() != String.Empty)
            {
                double value = Convert.ToDouble(e.Row.Cells[1].Value.ToString());
                if (value == -100500)
                {
                    e.Row.Cells[1].Value = "Не применим к оценке";
                    e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Left;
                }
            }
        }

        #endregion
    }
}
