using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0042_0003 : CustomReportPage
    {
        // выбранный период
        private CustomParam selectedPeriod;

        protected override void Page_Load(object sender, EventArgs e)
        {
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            #region Инициализация параметров запроса
            
            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0042_0003_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            string quarter = dtDate.Rows[0][2].ToString();

            UserParams.PeriodYear.Value = endYear.ToString();
            UserParams.PeriodHalfYear.Value = dtDate.Rows[0][1].ToString();
            UserParams.PeriodQuater.Value = quarter;

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            GRBSGrid.Width = Unit.Empty;
            GRBSGrid.Height = Unit.Empty;
            GRBSGrid.DisplayLayout.NoDataMessage = "Нет данных";

            GRBSGrid.Bands.Clear();
            GRBSGrid.DataBind();

            GRBSGrid.Columns.RemoveAt(1);
        }

        #region Обработчики грида

        private GridHeaderLayout headerLayout;

        protected void GRBSGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0003_HMAO_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtGrid);
            
            foreach (DataRow dataRow in dtGrid.Rows)
            {
                string name = dataRow[0].ToString();
                string shortName =
                    dataRow[0].ToString().Replace("Ханты-Мансийского автономного округа – Югры", "ХМАО-Югры").Replace(
                        "Ханты-Мансийскому автономному округу – Югре", "ХМАО-Югре");
                dataRow[0] = String.Format("<a href='webcommand?showPopoverReport=fo_0042_0002_GRBS={1}&width=735&height=850&fitByHorizontal=true'>{0}</a>", shortName, CustomParams.GetGrbsIdByName(name));
            }
           ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 438;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 58;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");

            e.Layout.Bands[0].Columns[2].Width = 110;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");

            e.Layout.Bands[0].Columns[3].Width = 110;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");

            e.Layout.Bands[0].Columns[4].Width = 75;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");

            e.Layout.Bands[0].Columns[5].Hidden = true;            

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell("Наименование ГРБС");
            headerLayout.AddCell("Код ГРБС");
            headerLayout.AddCell("Итоговая оценка");
            headerLayout.AddCell("Отклонение от среднего");
            headerLayout.AddCell("Ранг");

            headerLayout.ApplyHeaderInfo();
        }

        protected void GRBSGrid_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricks.iPadBricks.iPadBricksHelper.SetRankImage(e, 4, 5, true, "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px; padding-right: 3px");
            iPadBricks.iPadBricks.iPadBricksHelper.SetConditionCorner(e, 3);
        }

        #endregion
       

    }
}

