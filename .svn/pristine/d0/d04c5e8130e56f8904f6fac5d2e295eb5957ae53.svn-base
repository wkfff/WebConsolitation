using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SE_0002_0009 : CustomReportPage
    {
        private int year = 2010;
        private int monthNum = 8;
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            UserParams.Region.Value = RegionsNamingHelper.FullName(UserParams.Region.Value.Replace("УФО", "УрФО"));
            InitializeDate();

            DataTable dtLoaded = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0002_loaded");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtLoaded);

            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataBind();
            UltraWebGrid1.Width = Unit.Empty;
        }

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0]);
            reportDate = new DateTime(year, monthNum, 1);

            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 4);
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 1);
            UserParams.PeriodEndYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-2), 1);

            DataTable dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("SE_0002_0009_index");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricksHelper.SetConditionArrow(e, 9);
            iPadBricksHelper.SetConditionCorner(e, 1, 100);

            string style =
                "background-repeat: no-repeat; background-position: 7px center; padding-left: 2px; padding-right: 5px";
            iPadBricksHelper.SetRankImage(e, 2, 3, true, style);
            iPadBricksHelper.SetRankImage(e, 5, 6, true, style);

            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("федер"))
            {
                e.Row.Style.BorderDetails.WidthTop = borderWidth;
                if (e.Row.Cells[0].Value.ToString().ToLower().Contains("округ"))
                {

                }
                else
                {
                    e.Row.Cells[0].Value =
                        String.Format(
                            "<a style='color: White' href='webcommand?showReport=indexPage_SE_0001_0008'>{0}</a>", e.Row.Cells[0].Value);
                }
            }
            else
            {
                e.Row.Cells[0].Value = String.Format("<a style='color: #d2d2d2' href='webcommand?showReport=se_0001_0002_{1}'>{0}</a>", e.Row.Cells[0].Value, CustomParams.GetSubjectIdByName(e.Row.Cells[0].Value.ToString()));
            }

            if (e.Row.Index < 2)
            {
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 200;
            e.Layout.Bands[0].Columns[1].Width = 150;
            e.Layout.Bands[0].Columns[2].Width = 65;
            e.Layout.Bands[0].Columns[4].Width = 183;
            e.Layout.Bands[0].Columns[5].Width = 65;
            e.Layout.Bands[0].Columns[7].Width = 80;

            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, String.Format("Индекс промышленного производства за январь- {0} {1}", CRHelper.RusMonth(monthNum), year - 1), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, String.Format("Ранг в {0}", RegionsNamingHelper.ShortName(UserParams.Region.Value)), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, String.Format("Объем отгруженных товаров, выполненных работ и услуг за январь- {0} {1}, млн.руб.", CRHelper.RusMonth(monthNum), year), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, String.Format("Ранг в {0}", RegionsNamingHelper.ShortName(UserParams.Region.Value)), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, String.Format("Доля в {0}, %", RegionsNamingHelper.ShortName(UserParams.Region.Value)), String.Empty);


            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[7].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[7].CellStyle.BorderDetails.WidthLeft = borderWidth;

            e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[5].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[7].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
        }

        private int borderWidth = 3;

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtIndex = new DataTable();
            string query = DataProvider.GetQueryText("SE_0002_0009_table1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIndex);

            UltraWebGrid1.DataSource = dtIndex;
        }
    }
}
