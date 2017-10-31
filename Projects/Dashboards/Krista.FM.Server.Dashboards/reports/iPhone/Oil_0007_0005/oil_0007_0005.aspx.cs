using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0007_0005 : CustomReportPage
    {
        private DateTime currentDate;
        private DateTime lastDate;
        private DateTime firstDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InitializeTable1();
        }

        #region Таблица1
        private DataTable dt;

        private void InitializeTable1()
        {
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            string query;

            if (HttpContext.Current.Session["CurrentOilID"].ToString() != "5")
            {
                DataTable dtDateFederal = new DataTable();
                query = DataProvider.GetQueryText("Oil_0007_0005_incomes_federal_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDateFederal);

                currentDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[2][1].ToString(), 3);
                lastDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[1][1].ToString(), 3);
                firstDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[0][1].ToString(), 3);

                UserParams.PeriodCurrentDate.Value = dtDateFederal.Rows[2][1].ToString();
                UserParams.PeriodLastDate.Value = dtDateFederal.Rows[1][1].ToString();
                UserParams.PeriodFirstYear.Value = dtDateFederal.Rows[0][1].ToString();

                DataTable dtFederal = new DataTable();
                query = DataProvider.GetQueryText("Oil_0007_0005_table2");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtFederal);

                dtFederal.Columns.RemoveAt(0);
                dtFederal.AcceptChanges();

                UltraWebGrid1.DataSource = dtFederal;
                UltraWebGrid1.DataBind();
            }
            else
            {
                UltraWebGrid1.Visible = false;
            }

            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0007_0005_incomes_regional_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);
            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);
            firstDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDate.Rows[0][1].ToString();

            dt = new DataTable();
            query = DataProvider.GetQueryText("Oil_0007_0005_table1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dt);

            dt.Columns.RemoveAt(0);
            dt.AcceptChanges();

            UltraWebGrid2.DataSource = dt;
            UltraWebGrid2.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 2, 0, false);
            iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 4, 0, false);

            iPadBricks.iPadBricks.iPadBricksHelper.SetRankImage(e, 7, 8, false, "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px; padding-right: 10px", 1, "~/images/min.png", "~/images/max.png");

            if (e.Row.Index < 1 || e.Row.Cells[0].Value.ToString().ToLower().Contains("федера"))
            {
                e.Row.Style.Font.Bold = true;
            }
            
            if (!e.Row.Cells[0].Value.ToString().ToLower().Contains("федера") && !e.Row.Cells[0].Value.ToString().ToLower().Contains("хмао"))
            {                    
                CustomParams.MakeMoParams(e.Row.Cells[0].Value.ToString().Replace(" муниципальный район", "").Replace("Город ", "г."), "name");
                e.Row.Cells[0].Value = String.Format("<a href='webcommand?showReport=OIL_0007_0002_MO={1}'>{0}</a>", e.Row.Cells[0].Value.ToString().Replace(" муниципальный район", " МР").Replace("Город ", "г. "), HttpContext.Current.Session["CurrentMoID"]);
            }
            

            e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace(" муниципальный район", " МР").Replace("Город ", "г. ");
        }

        private GridHeaderLayout headerLayout1;
        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            headerLayout1 = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout1.AddCell("");
            headerLayout1.AddCell(String.Format("Средняя розничная цена на {0:dd.MM.yyyy}, руб.", currentDate));

            GridHeaderCell headerCell = headerLayout1.AddCell(String.Format("Изменение к {0:dd.MM.yyyy}", lastDate));
            headerCell.AddCell("Абс. откл, руб.");
            headerCell.AddCell("Темп прироста");

            headerCell = headerLayout1.AddCell(String.Format("Изменение к {0:dd.MM.yyyy}", firstDate));
            headerCell.AddCell("Абс. откл, руб.");
            headerCell.AddCell("Темп прироста");

            headerLayout1.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Width = 240;
            e.Layout.Bands[0].Columns[1].Width = 110;
            e.Layout.Bands[0].Columns[2].Width = 100;
            e.Layout.Bands[0].Columns[3].Width = 100;
            e.Layout.Bands[0].Columns[4].Width = 100;
            e.Layout.Bands[0].Columns[5].Width = 100;

            e.Layout.Bands[0].Columns[6].Hidden = true;

            e.Layout.Bands[0].Columns[1].HeaderStyle.Font.Size = 11;
        }

        #endregion

    }
}
