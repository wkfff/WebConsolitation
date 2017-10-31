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
    public partial class SE_0001_0005 : CustomReportPage
    {
        private int year = 2010;
        private int monthNum = 8;
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InitializeDate();

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }

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
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricksHelper.SetConditionCorner(e, 1, 100);
            iPadBricksHelper.SetConditionCorner(e, 4, 100);
            iPadBricksHelper.SetConditionCorner(e, 7, 100);
            iPadBricksHelper.SetConditionCorner(e, 10, 100);

            string style = "background-repeat: no-repeat; background-position: 7px center; padding-left: 2px; padding-right: 5px";
            iPadBricksHelper.SetRankImage(e, 2, 3, true, style);
            iPadBricksHelper.SetRankImage(e, 5, 6, true, style);
            iPadBricksHelper.SetRankImage(e, 8, 9, true, style);
            iPadBricksHelper.SetRankImage(e, 11, 12, true, style);

            if (e.Row.Index == 2)
            {
                e.Row.Style.BorderDetails.WidthBottom = borderWidth;
            }
            if (e.Row.Index == 3)
            {
                e.Row.Style.BorderDetails.WidthTop = borderWidth;
            }
            if (e.Row.Index < 3)
            {
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }

            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("федер"))
            {
                e.Row.Style.BorderDetails.WidthTop = borderWidth;
                if (e.Row.Cells[0].Value.ToString().ToLower().Contains("округ"))
                {
                    e.Row.Cells[0].Value =
                        String.Format(
                            "<a style='color: White' href='webcommand?showReport=se_0001_0009_FO={1}'>{0}</a>",
                            e.Row.Cells[0].Value, CustomParams.GetFOIdByName(e.Row.Cells[0].Value.ToString()));
                }
                else
                {
                    e.Row.Cells[0].Value =
                        String.Format(
                            "<a style='color: White' href='webcommand?showReport=indexPage_SE_0001_0008'>{0}</a>", e.Row.Cells[0].Value);
                }
            }
            else if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Value = String.Format("<a style='color: #d2d2d2' href='webcommand?showReport=se_0001_0002_{1}'>{0}</a>", e.Row.Cells[0].Value, CustomParams.GetSubjectIdByName(e.Row.Cells[0].Value.ToString()));
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 172;
            e.Layout.Bands[0].Columns[1].Width = 88;
            e.Layout.Bands[0].Columns[2].Width = 58;
            e.Layout.Bands[0].Columns[4].Width = 88;
            e.Layout.Bands[0].Columns[5].Width = 58;
            e.Layout.Bands[0].Columns[7].Width = 88;
            e.Layout.Bands[0].Columns[8].Width = 58;
            e.Layout.Bands[0].Columns[10].Width = 88;
            e.Layout.Bands[0].Columns[11].Width = 58;

            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[9].Hidden = true;
            e.Layout.Bands[0].Columns[12].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            string period = monthNum == 1 ? "Январь" : String.Format("Январь- {0}", CRHelper.RusMonth(monthNum));

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Январь- декабрь", String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Ранг", String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, String.Format("{0}", period), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Ранг", String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Январь- декабрь", String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Ранг", String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10, String.Format("{0}", period), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11, "Ранг", String.Empty);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N1");

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[8].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[11].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[9].CellStyle.BorderDetails.WidthLeft = borderWidth;

            e.Layout.Bands[0].Columns[2].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].Columns[4].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);

            e.Layout.Bands[0].Columns[8].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].Columns[10].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = (year - 2).ToString();

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 2;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = (year - 1).ToString();
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 3;
            ch.RowLayoutColumnInfo.SpanX = 4;
            //ch.Style.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
            //ch.Style.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = year.ToString();
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 7;
            ch.RowLayoutColumnInfo.SpanX = 2;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            e.Layout.Bands[0].Columns[3].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].Columns[4].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);

            e.Layout.Bands[0].Columns[9].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].Columns[10].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
        }

        private int borderWidth = 3;

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtIndex = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0005_table1_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIndex);

            UltraWebGrid1.DataSource = dtIndex;
        }
    }
}
