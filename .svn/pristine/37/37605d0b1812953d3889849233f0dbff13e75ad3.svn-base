using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;
using System.Web.UI.WebControls;
using System;
using Infragistics.WebUI.UltraWebGrid;
using System.Data;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class EO_0002_0002_v : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private string multiplierCaption = "млн.руб.";

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            #region Настройка грида

            GRBSGridBrick.Height = Unit.Empty;
            GRBSGridBrick.Width = Unit.Empty;
            GRBSGridBrick.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);
            GRBSGridBrick.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0006_lastDate");

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 4);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            lbInfo.Text = String.Format(@"Целевые показатели по состоянию на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;года",
                currentDate.AddMonths(1));
            GridDataBind();
        }

       
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0002_0002_grid_v");
            gridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);
            
            if (gridDt.Rows.Count > 0)
            {
                GRBSGridBrick.DataSource = gridDt;
                GRBSGridBrick.DataBind();
            }
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString() == "Показатели непосредственных результатов" ||
                e.Row.Cells[0].Value.ToString() == "Показатели конечных результатов")
            {
                e.Row.Cells[0].ColSpan = 5;
                e.Row.Style.Font.Bold = true;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }
       

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int columnCount = e.Layout.Bands[0].Columns.Count;

            if (columnCount == 0)
            {
                return;
            }

            for (int i = 1; i < columnCount; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 10;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ###.###");
            }

            //for (int i = 2; i < columnCount; i++)
            //{
            //    e.Layout.Bands[0].Columns[i].CellStyle.VerticalAlign = VerticalAlign.Middle;
            //}

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            ////e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = 270;
            e.Layout.Bands[0].Columns[1].Width = 120;
            e.Layout.Bands[0].Columns[2].Width = 120;
            e.Layout.Bands[0].Columns[3].Width = 120;
            e.Layout.Bands[0].Columns[4].Width = 120;

            //e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 14;
            //e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;

            GridHeaderLayout headerLayout = new GridHeaderLayout(GRBSGridBrick);

            headerLayout.AddCell("Наименование показателя");
            headerLayout.AddCell(String.Format("Базовый показатель на начало реализации программы", currentDate));
            headerLayout.AddCell(String.Format("По состоянию на {0:dd.MM.yyyy}", currentDate.AddMonths(1)));
            GridHeaderCell cell = headerLayout.AddCell(String.Format("Значение на {0:yyyy} год", currentDate));
            cell.AddCell(String.Format("Факт", currentDate));
            cell.AddCell(String.Format("План", currentDate));            

            headerLayout.ApplyHeaderInfo();
        }

      
    }
}
