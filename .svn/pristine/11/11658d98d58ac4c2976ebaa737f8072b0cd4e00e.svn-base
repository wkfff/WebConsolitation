using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class iPad_0001_0002 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraChart.Width = 745;
            UltraChart.Height = 350;
            UltraChart.ChartType = ChartType.AreaChart;

            UltraChart.DataBinding += new EventHandler(UltraChart_DataBinding);

            UltraChart.Data.ZeroAligned = true;
            UltraChart.AreaChart.NullHandling = NullHandling.DontPlot;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart.Axis.X.Extent = 80;
            UltraChart.Axis.Y.Extent = 40;

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N3> руб.";

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 22;
            UltraChart.Legend.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.Legend.Font = new Font("Verdana", 10);

            UltraChart.Axis.Y.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart.Axis.X.Labels.Visible = true;

            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;

            UltraChart.AreaChart.LineAppearances.Add(lineAppearance);

            UltraWebGrid.Width = Unit.Empty;

            ImageBasket.ImageUrl = "~/Images/Basket.png";
            ImagePurse.ImageUrl = "~/Images/Purse.png";

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }

            HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}' <img style='margin-left: -30px; margin-right: 10px; height: 65px' src=\"../../../images/Heralds/{0}.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"], HttpContext.Current.Session["CurrentSiteRef"]);

            UltraChart.DataBind();
            UltraWebGrid.DataBind();
            LabelsDataBind();
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet) primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                    }
                }
            } 
        }

        private void LabelsDataBind()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0002_date_goods_cost");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodYear.Value = dtDate.Rows[0][4].ToString();
            DateTime reportDate = CRHelper.PeriodDayFoDate(dtDate.Rows[0][4].ToString());
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", reportDate.AddMonths(-1), 4);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0002_labels");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            lbReportDate.Text =
                String.Format("В {0} {1} года", CRHelper.RusMonthPrepositional(reportDate.Month), reportDate.Year);
            
            lbFixedSetCostValue.Text = String.Format("{0:N2}", dt.Rows[2][1]);

            lbFixedSetFoRankDescription.Text = String.Format("ранг {0}&nbsp;", UserParams.ShortRegion.Value);
            lbFixedSetFoRankValue.Text = String.Format("{0:N0}{1}", dt.Rows[0][1], GetStarImage(dt, 0, 1));
            lbFixedSetRfRankDescription.Text = String.Format("&nbsp;ранг&nbsp;РФ&nbsp;");
            lbFixedSetRfRankValue.Text = String.Format("&nbsp;{0:N0}&nbsp;{1}", dt.Rows[1][1], GetStarImage(dt, 1, 1));
            lbFixedSetFoRankValue.Style.Add("line-height", "40px");

            string grown = "не изменилась";

            if (Convert.ToDouble(dt.Rows[5][1]) > 0)
            {
                grown = "выросла&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;на";
            }
            else if (Convert.ToDouble(dt.Rows[5][1]) < 0)
            {
                grown = "снизилась&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;на";
            }
            else
            {
                Label10.Visible = false;
                Label9.Visible = false;
            }

            Label8.Text =
                String.Format("По сравнению с предыдущим месяцем<br/>стоимость фиксированного набора<br/>{0}&nbsp;", grown);
            Label9.Text = String.Format("{0:N1}", dt.Rows[5][1]);

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0002_date_life_level");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodYear.Value = dtDate.Rows[0][4].ToString();
            reportDate = CRHelper.PeriodDayFoDate(dtDate.Rows[0][4].ToString());

            dt = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0002_labels");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            Label7.Text = String.Format("В {0} {1} года", CRHelper.RusMonthPrepositional(reportDate.Month), reportDate.Year);

            if (dt.Rows[2][2] != DBNull.Value &&
                dt.Rows[2][2].ToString() != String.Empty)
            {
                Label3.Text = "Среднедушевые денежные доходы составили&nbsp;";
                Label5.Text = "&nbsp;руб.";
                lbAverageIncomesValue.Text = String.Format("{0:N2}", dt.Rows[2][2]);
                lbAverageIncomesFoRankDescription.Text = String.Format("ранг {0}&nbsp;", UserParams.ShortRegion.Value);
                lbAverageIncomesFoRankValue.Text = String.Format("{0:N0}{1}", dt.Rows[0][2], GetStarImage(dt, 0, 2));
                lbAverageIncomesRfRankDescription.Text = String.Format("&nbsp;ранг РФ&nbsp;");
                lbAverageIncomesRfRankValue.Text =
                    String.Format("{0:N0}&nbsp;{1}", dt.Rows[1][2], GetStarImage(dt, 1, 2));
                lbAverageIncomesFoRankValue.Style.Add("line-height", "40px");
                Label6.Text = "Соотношение&nbsp;среднедушевых&nbsp;доходов<br/>и&nbsp;стоимости&nbsp;фиксированного&nbsp;набора<br/>составило&nbsp;";
            }
            else
            {
                Label3.Text = "Информация по среднедушевым денежным доходам отсутствует";
            }

            if (dt.Rows[2][2] != DBNull.Value && dt.Rows[2][1] != DBNull.Value)
            {
                lbCorrelation.Text = (Convert.ToDouble(dt.Rows[2][2])/Convert.ToDouble(dt.Rows[2][1])).ToString("N2");
            }
        }

        private static string GetStarImage(DataTable dt, int row, int column)
        {
            string img = String.Empty;
            if (dt.Rows[row][column] != DBNull.Value && Convert.ToInt32(dt.Rows[row][column]) == 1)
            {
                img = string.Format(" <img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[row][column] != DBNull.Value && dt.Rows[row + 3][column] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[row][column]) == Convert.ToInt32(dt.Rows[row + 3][column]))
            {
                img = string.Format(" <img src=\"../../../images/starGray.png\">");
            }
            return img;
        }

        private void UltraChart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0002_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            string year = String.Empty;

            for (int i = 1; i < dt.Columns.Count; i++ )
            {
                if (dt.Rows[0][i].ToString() == year)
                {
                    dt.Columns[i].ColumnName = String.Format("{0}", dt.Columns[i].ColumnName);
                }
                else
                {
                    dt.Columns[i].ColumnName = String.Format("{0} - {1}", dt.Rows[0][i], dt.Columns[i].ColumnName);
                    year = dt.Rows[0][i].ToString();
                }
                
            }
            dt.Rows.RemoveAt(0);
            dt.AcceptChanges();
            UltraChart.DataSource = dt;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0002_date_goods_index");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodYear.Value = dtDate.Rows[0][4].ToString();

            Label1.Text =
                String.Format("Индексы цен в {0} {1} г. (в % к декабрю прошлого года)",
                              CRHelper.RusMonthPrepositional(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0002_table");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);
            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Width = 507;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 80;
            e.Layout.Bands[0].Columns[3].Width = 80;

            e.Layout.Bands[0].Columns[1].Header.Caption = RegionsNamingHelper.ShortName(UserParams.StateArea.Value);
            e.Layout.Bands[0].Columns[2].Header.Caption = RegionsNamingHelper.ShortName(UserParams.Region.Value);
            e.Layout.Bands[0].Columns[3].Header.Caption = "РФ";
            
            for (int i = 1; i <= e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = false;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            SetConditionArrow(e, 1);
            SetConditionArrow(e, 2);
            SetConditionArrow(e, 3);
        }

        private static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value > 100)
                {
                    img = "~/images/arrowRedUpBB.png";

                }
                else
                {
                    img = "~/images/arrowGreenDownBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
        }
    }
}
