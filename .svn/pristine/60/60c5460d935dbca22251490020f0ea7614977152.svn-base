using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SE_0001_0003 : CustomReportPage
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

            UltraWebGrid2.DataBinding += new EventHandler(UltraWebGrid2_DataBinding);
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid2_InitializeLayout);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid2_InitializeRow);
            UltraWebGrid2.DataBind();
            UltraWebGrid2.Width = Unit.Empty;
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

        void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricksHelper.SetConditionCorner(e, 1, 100);
            iPadBricksHelper.SetConditionCorner(e, 3, 100);
            iPadBricksHelper.SetConditionCorner(e, 5, 100);
            iPadBricksHelper.SetConditionCorner(e, 7, 100);
            iPadBricksHelper.SetConditionCorner(e, 2, 100);
            iPadBricksHelper.SetConditionCorner(e, 4, 100);
            iPadBricksHelper.SetConditionCorner(e, 6, 100);
            iPadBricksHelper.SetConditionCorner(e, 8, 100);

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

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricksHelper.SetConditionCorner(e, 1, 100);
            iPadBricksHelper.SetConditionCorner(e, 4, 100);
            iPadBricksHelper.SetConditionCorner(e, 7, 100);
            iPadBricksHelper.SetConditionCorner(e, 10, 100);

            string style =
                "background-repeat: no-repeat; background-position: 7px center; padding-left: 2px; padding-right: 5px";
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

            e.Layout.CellSpacingDefault = 0;
            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[8].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[11].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[9].CellStyle.BorderDetails.WidthLeft = borderWidth;

            e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[5].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[8].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[11].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

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

            e.Layout.Bands[0].Columns[2].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].Columns[4].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);

            e.Layout.Bands[0].Columns[8].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].Columns[10].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
        }

        private int borderWidth = 3;

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtIndex = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0003_table1_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIndex);

            UltraWebGrid1.DataSource = dtIndex;
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Collection<int> columnsX = new Collection<int>();
            Collection<int> columnsHeights = new Collection<int>();
            Collection<string> columnsValues = new Collection<string>();
            Collection<string> regions = new Collection<string>();
            int columnWidth = 0;
            int axisZero = 0;

            regions.Add("РФ");
            regions.Add(RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("region").Value));
            regions.Add(RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("state_area").Value));

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (axisZero == 0)
                        {
                            axisZero = box.rect.Y + box.rect.Height;
                        }
                        columnsX.Add(box.rect.X);
                        columnsHeights.Add(box.rect.Height);
                        columnWidth = box.rect.Width;
                        if (box.Value != null)
                        {
                            columnsValues.Add(box.Value.ToString());
                        }
                    }
                }
            }

            for (int i = 0; i < columnsValues.Count; i++)
            {
                double value;
                if (double.TryParse(columnsValues[i].ToString(), out value))
                {
                    Text text = new Text();
                    text.labelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
                    text.PE.Fill = Color.White;

                    int yPos = value > 0 ? axisZero - columnsHeights[i] - 20 : axisZero + columnsHeights[i] + 20;

                    text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 20);
                    text.SetTextString(value.ToString("N1") + "%");
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);

                    text = new Text();
                    text.labelStyle.Font = new Font("Arial", 12, FontStyle.Bold);

                    text.PE.Fill = Color.White;
                    yPos = value > 0 ? axisZero - columnsHeights[i] - 40 : axisZero + columnsHeights[i];
                    text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 20);
                    text.SetTextString(regions[i]);
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);
                }
            }
        }

        void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 172;
            e.Layout.Bands[0].Columns[1].Width = 73;
            e.Layout.Bands[0].Columns[2].Width = 73;
            e.Layout.Bands[0].Columns[3].Width = 73;
            e.Layout.Bands[0].Columns[4].Width = 73;
            e.Layout.Bands[0].Columns[5].Width = 73;
            e.Layout.Bands[0].Columns[6].Width = 73;
            e.Layout.Bands[0].Columns[7].Width = 73;
            e.Layout.Bands[0].Columns[8].Width = 73;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209); 
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[3].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[5].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[7].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[8].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[7].CellStyle.BorderDetails.WidthLeft = borderWidth;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 3;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 2;
                }
            }

            string period = monthNum == 1 ? "Январь" : String.Format("Январь- {0}", CRHelper.RusMonth(monthNum));

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N1");

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, String.Format("{0} {1}", period, year - 1), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, String.Format("{0} {1}", period, year), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, String.Format("{0} {1}", period, year - 1), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, String.Format("{0} {1}", period, year), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, String.Format("{0} {1}", period, year - 1), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, String.Format("{0} {1}", period, year), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, String.Format("{0} {1}", period, year - 1), String.Empty);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, String.Format("{0} {1}", period, year), String.Empty);

            e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[3].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[4].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[5].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[6].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[7].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[8].Header.Style.Font.Size = FontUnit.Parse("14px");

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Индекс промышленного производства";

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.RowLayoutColumnInfo.SpanY = 2;
            ch.Style.Font.Size = FontUnit.Parse("16px");

            ch.Style.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].HeaderLayout.Add(ch);
            
            ch = new ColumnHeader(true);
            ch.Caption = "Добыча полезных ископаемых"; ;
            ch.RowLayoutColumnInfo.OriginY = 1;
            ch.RowLayoutColumnInfo.OriginX = 3;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Font.Size = FontUnit.Parse("16px");
            ch.Style.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Обрабатывающие производства";
            ch.RowLayoutColumnInfo.OriginY = 1;
            ch.RowLayoutColumnInfo.OriginX = 5;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Производство и распределение электроэнергии, газа и воды";
            ch.RowLayoutColumnInfo.OriginY = 1;
            ch.RowLayoutColumnInfo.OriginX = 7;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "По видам деятельности";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 3;
            ch.RowLayoutColumnInfo.SpanX = 6;
            ch.Style.Font.Size = FontUnit.Parse("16px");
            ch.Style.BorderDetails.ColorBottom = Color.FromArgb(85, 85, 85);
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }

        void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            DataTable dtIndex = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0003_table2_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIndex);

            UltraWebGrid2.DataSource = dtIndex;
        }

    }
}
