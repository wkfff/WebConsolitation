using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0034 : CustomReportPage
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
            string query = DataProvider.GetQueryText("FO_0035_0034_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);            

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            ReviewGrid.Width = Unit.Empty;
            ReviewGrid.Height = Unit.Empty;
            ReviewGrid.DisplayLayout.NoDataMessage = "Нет данных";
            ReviewGrid.DataBind();

            lbDescription.Text = String.Format("Остатки средств из федерального бюджета на едином счете областного бюджета по состоянию на&nbsp;<b><span class='DigitsValuePopup'>{0:dd.MM.yyyy}</span></b>, тыс.руб.",
                date, UserParams.Grbs.Value);

            DataTable dtPerson = new DataTable();
            query = DataProvider.GetQueryText("person");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "name", dtPerson);

            HyperLinkSite.NavigateUrl = dtPerson.Rows[0][2].ToString();
            HyperLinkSite.Text = dtPerson.Rows[0][2].ToString();
            lbFIO.Text = dtPerson.Rows[0][3].ToString();
            lbPhone.Text = dtPerson.Rows[0][4].ToString();
            HyperLinkMail.NavigateUrl = "mailto:" + dtPerson.Rows[0][5];
            HyperLinkMail.Text = dtPerson.Rows[0][5].ToString();

            Image1.ImageUrl = String.Format("../../../images/novosibPhotos/{0}.jpg", HttpContext.Current.Session["CurrentGrbsID"]);
        }
        

        #region Обработчики грида

        protected void ReviewGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0034_ReviewGrid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtGrid);

            dtGrid.AcceptChanges();
            ReviewGrid.DataSource = dtGrid;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 50;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Код цели", date);

            e.Layout.Bands[0].Columns[2].Width = 130;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Поступило из федерального бюджета", date);

            e.Layout.Bands[0].Columns[3].Width = 120;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            e.Layout.Bands[0].Columns[3].Header.Caption = String.Format("Перечислено по данным АС «Бюджет»", date);

            e.Layout.Bands[0].Columns[4].Width = 120;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            e.Layout.Bands[0].Columns[4].Header.Caption = String.Format("Остатки средств на едином счете бюджета", date);

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell("Подпрограмма (объект)");

            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
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
