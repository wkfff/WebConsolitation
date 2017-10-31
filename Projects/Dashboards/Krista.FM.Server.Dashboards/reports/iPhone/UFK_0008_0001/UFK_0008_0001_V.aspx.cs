using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class UFK_0008_0001_V : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("UFK_0008_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodCurrentDate.Value = string.Format("[{0}].[{1}].[{2}].[{3}]",
                                                               dtDate.Rows[0][1],
                                                               dtDate.Rows[0][2],
                                                               dtDate.Rows[0][3],
                                                               dtDate.Rows[0][4]);

            UltraWebGrid.DataBind();
            ChartDataBind();
        }

        private void ChartDataBind()
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("UFK_0008_0001_v_chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / 1000000;
                }
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2])/1000000;
                }
                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3])/1000000;
                }
            }

            UltraChart1.DataSource = ReverseRowsDataTable(dt);
            UltraChart1.DataBind();

            UltraChart1.Height = 800;
            UltraChart1.ChartType = ChartType.BarChart;
            UltraChart1.Axis.X.Extent = 60;
            UltraChart1.Axis.Y.Extent = 15;
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Axis.Y.Labels.SeriesLabels.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Axis.Y.Labels.SeriesLabels.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Axis.X.LineColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Axis.Y.LineColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Axis.X.Labels.ItemFormatString += " млн.";
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.OrientationAngle = 45;
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Layout.Padding = 20;
            UltraChart1.Axis.X.Labels.VerticalAlign = StringAlignment.Near;

            UltraChart1.Axis.X2.Labels.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Axis.X2.LineColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Axis.X2.Labels.ItemFormatString += " млн.";
            UltraChart1.Axis.X2.Labels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X2.Labels.OrientationAngle = 45;
            UltraChart1.Axis.X2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X2.Labels.Layout.Padding = -20;
            UltraChart1.Axis.X2.Labels.VerticalAlign = StringAlignment.Far;
            UltraChart1.Axis.X2.Labels.HorizontalAlign = StringAlignment.Far;
            UltraChart1.Axis.X2.Extent = 32;
            UltraChart1.Axis.X2.Visible = true;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N3> млн.руб.";

            UltraChart1.Legend.FormatString = "";

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 7;
            UltraChart1.Legend.BackgroundColor = Color.DimGray;
            UltraChart1.Legend.AlphaLevel = 50;
            UltraChart1.Legend.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart1.Legend.BorderColor = Color.Black;

            UltraChart1.BackColor = Color.Black;
        }

        /// <summary>
        /// Инвертирует следование строк в таблице
        /// </summary>
        /// <param name="dt">входная таблица</param>
        /// <returns>выходная таблица</returns>
        private static DataTable ReverseRowsDataTable(DataTable dt)
        {
            DataTable resDt = new DataTable();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn column = new DataColumn(dt.Columns[i].Caption, dt.Columns[i].DataType);
                resDt.Columns.Add(column);
            }

            for (int i = dt.Rows.Count; i > 0; i--)
            {
                DataRow row = resDt.NewRow();
                row.ItemArray = dt.Rows[i - 1].ItemArray;
                resDt.Rows.Add(row);
            }

            return resDt;
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("UFK_0008_0001_v_table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt);
            DataColumn col = new DataColumn("Image");
            dt.Columns.Add(col);

            foreach(DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1])*100;
                }
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2])*100;
                }
            }

            UltraWebGrid.DataSource = dt.DefaultView;

            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText("UFK_0008_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            Label4.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            Label2.Text = String.Format("План % = запланированный темп роста доходов {0}г. к {1}г.", year, year - 1);
            DateTime dateCurrentYear = new DateTime(year, CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4]));
            DateTime dateLastYear = new DateTime(year - 1, CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4]));
            Label3.Text = String.Format("Факт % = фактический темп роста доходов {0:dd.MM.yyyy}г. к {1:dd.MM.yyyy}г.", dateCurrentYear, dateLastYear);

            UltraWebGrid.DisplayLayout.AllowSortingDefault = AllowSorting.OnClient;
            UltraWebGrid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.SortMulti;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid.DisplayLayout.GroupByBox.Hidden = true;

            if (UltraWebGrid.DisplayLayout.Bands.Count == 0)
                return;

            if (UltraWebGrid.DisplayLayout.Bands[0].Columns.Count > 3)
            {
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].Header.Caption = "План %";
                UltraWebGrid.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Факт %";
                UltraWebGrid.DisplayLayout.Bands[0].Columns[3].Header.Caption = string.Empty;

                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);  
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[2].CellStyle.Font.Bold = true;

                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].Width = 145;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].Width = 70;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[2].Width = 70;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[3].Width = 22;

                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Padding.Left = 5;

                CRHelper.FormatNumberColumn(UltraWebGrid.DisplayLayout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(UltraWebGrid.DisplayLayout.Bands[0].Columns[2], "N2");
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (Convert.ToDouble(e.Row.Cells[2].Value) < Convert.ToDouble(e.Row.Cells[1].Value))
            {
                e.Row.Cells[3].Style.BackgroundImage = "~/images/red.png";
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: center";
            }
            else if (Convert.ToDouble(e.Row.Cells[2].Value) > Convert.ToDouble(e.Row.Cells[1].Value))
            {
                e.Row.Cells[3].Style.BackgroundImage = "~/images/green.png";
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: center";
            }
            foreach(UltraGridCell cell in e.Row.Cells)
            {
                cell.Style.BorderColor = Color.FromArgb(50, 50, 50);
            }
            e.Row.Cells[3].Style.BorderDetails.StyleLeft = BorderStyle.None;
            e.Row.Cells[2].Style.BorderDetails.StyleRight = BorderStyle.None;
        }

        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int x0 = 40;
            int y0 = 20;

            Text text = new Text();
            text.PE.Fill = Color.FromArgb(209, 209, 209);
            text.bounds = new Rectangle(x0, y0, x0+40, y0+10);
            text.SetTextString("Факт"); 
            e.SceneGraph.Add(text);

            text = new Text();
            text.PE.Fill = Color.FromArgb(209, 209, 209);
            text.bounds = new Rectangle(x0 + 60, y0, x0 + 90, y0 + 10);
            text.SetTextString("План");
            e.SceneGraph.Add(text);

            text = new Text();
            text.PE.Fill = Color.FromArgb(209, 209, 209);
            text.bounds = new Rectangle(x0 + 115, y0, x0 + 140, y0 + 10);
            text.SetTextString("Факт прошлого года");
            e.SceneGraph.Add(text);


            Box box = new Box(new Rectangle(x0 - 15, y0 + 6, 15, 15));
            box.PE.Fill = Color.FromArgb(220, 186, 23);
            e.SceneGraph.Add(box);

            box = new Box(new Rectangle(x0 + 45, y0 + 6, 15, 15));
            box.PE.Fill = Color.FromArgb(1, 92, 151);
            e.SceneGraph.Add(box);

            box = new Box(new Rectangle(x0 + 100, y0 + 6, 15, 15));
            box.PE.Fill = Color.FromArgb(70, 119, 5);
            e.SceneGraph.Add(box);
        }
    }
}
