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
    public partial class FO_0035_0024 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        private string tableName = "ГРБС";

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0024_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            DateTime proceededDate = DataProvidersFactory.PrimaryMASDataProvider.GetCubeLastProcessedDate("ФО_Реестр платежных поручений");

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";
            OutcomesGrid.DataBind();
            
            lbDescription.Text = String.Format("Реестр платежных поручений областного бюджета Новосибирской области в разрезе ГРБС<br/>по состоянию на&nbsp;<b><span class='DigitsValue'>{0:HH:mm dd.MM.yyyy}</span></b>, тыс.руб.", proceededDate);           
        }


        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0024_OutcomesGridGrbs");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            foreach (DataRow dataRow in dtGrid.Rows)
            {
                string outcomesId = CustomParams.GetGrbsIdByName(dataRow[0].ToString());

                if (outcomesId != String.Empty)
                {
                    dataRow[0] =
                        String.Format(
                            "<a href='webcommand?showPopoverReport=FO_0035_0026_grbs={0}&width=690&height=650&fitByHorizontal=true'>{1}</a>",
                            outcomesId, dataRow[0]);
                }
            }
            dtGrid.AcceptChanges();
            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 220;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 125;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 110;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");

            e.Layout.Bands[0].Columns[3].Width = 165;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].Columns[4].Width = 125;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");

            e.Layout.Bands[0].Columns[5].Hidden = true;

            if (e.Layout.Bands[0].Columns.Count > 6)
            {
                e.Layout.Bands[0].Columns[6].Hidden = true;
            }

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell(String.Format("Наименование<br/>{0}", tableName));
            GridHeaderCell cell = headerLayout.AddCell("П/п на рассмотрении ФО");
            cell.AddCell("Сумма");
            cell.AddCell("% от общей суммы");
            headerLayout.AddCell("П/п проверены ФО (еще не санкционированы)");
            headerLayout.AddCell("П/п отправленные в УФК (банк)");

            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString() == "Всего")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }           
            else if (e.Row.Cells.Count > 6 &&
                    e.Row.Cells[6].Value.ToString() == "Подстатья")
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.ForeColor = Color.FromArgb(192, 192, 192);
                }
            }
            else
            {
                SetRankImage(e, 5, 2);
            }
        }

        #endregion

        public static void SetRankImage(RowEventArgs e, int rankCellIndex, int imageCellIndex)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                string img = String.Empty;

                if (value < 4)
                {
                    img = "~/images/StarYellow.png";
                }

                e.Row.Cells[imageCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[imageCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px;";
            }
        }
    }
}
