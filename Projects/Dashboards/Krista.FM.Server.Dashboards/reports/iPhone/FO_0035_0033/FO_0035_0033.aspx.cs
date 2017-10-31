using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0033 : CustomReportPage
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
            string query = DataProvider.GetQueryText("FO_0035_0033_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();

            lbDescription.Text = String.Format("Остатки средств, поступивших из федерального бюджета на едином счете областного бюджета в разрезе ГРБС по состоянию на&nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, тыс.руб.", date);
        }


        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0033_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            foreach (DataRow dataRow in dtGrid.Rows)
            {
                string outcomesId = CustomParams.GetGrbsIdByName(dataRow[0].ToString());

                if (outcomesId != String.Empty)
                {
                    dataRow[0] =
                        String.Format(
                            "<a href='webcommand?showPopoverReport=FO_0035_0034_grbs={0}&width=690&height=650&fitByHorizontal=true'>{1}</a>",
                            outcomesId, dataRow[0]);
                }
            }

            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 280;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 160;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Поступило из федерального бюджета", date);

            e.Layout.Bands[0].Columns[2].Width = 160;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Перечислено по данным АС «Бюджет»", date);

            e.Layout.Bands[0].Columns[3].Width = 160;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            e.Layout.Bands[0].Columns[3].Header.Caption = String.Format("Остатки средств на едином счете бюджета", date);

            headerLayout = new GridHeaderLayout(e.Layout.Grid);
                      
            headerLayout.AddCell("ГРБС");            

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
        }

        #endregion     
    }
}
