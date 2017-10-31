using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.UFK_0017_0001
{
    public partial class Default_noTarget : CustomReportPage
    {
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyWidth = (int)Session["width_size"] - 40;
            int dirtyHeight = (int)Session["height_size"] - 300;

            UltraWebGrid.Width = dirtyWidth;
            UltraChart1.Width = dirtyWidth;

            UltraWebGrid.Height = (int)(dirtyHeight * 0.4);
            UltraChart1.Height = (int)(dirtyHeight * 0.5);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)                         
            {                
                comboYear.SelectedIndex = 10;
                comboMonth.SelectedIndex = 7;
            }

            string pValue = string.Format("[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                comboYear.SelectedRow.Cells[0].Text,
                CRHelper.HalfYearNumByMonthNum(comboMonth.SelectedIndex + 1),
                CRHelper.QuarterNumByMonthNum(comboMonth.SelectedIndex + 1),
                comboMonth.SelectedRow.Cells[0].Text);
                        
                lbTitle.Text = string.Format("Движение свободного остатка средств бюджета за {0} {1}г.",
                    comboMonth.SelectedRow.Cells[0].Text.ToLower(), comboYear.SelectedRow.Cells[0].Text);
                UserParams.PeriodMonth.Value = pValue;
                UserParams.PeriodYear.Value = string.Format("[Период].[День].[Данные всех периодов].[{0}]", 
                    comboYear.SelectedRow.Cells[0].Text);
                UltraWebGrid.DataBind();
                UltraChart1.DataBind();
                SetNote();            
        }

        DataTable dtTable = new DataTable();
        DataTable dtChart = new DataTable();
        DataTable dtNote = new DataTable();

        private void SetNote()
        {
            string query = DataProvider.GetQueryText("noteNoTarget");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtNote);
            foreach (DataRow item in dtNote.Rows)
            {
                double cellValue;
                for (int i = 0; i < dtNote.Columns.Count; i++)
                {
                    if (double.TryParse(item[i].ToString(), out cellValue))
                    {
                        item[i] = cellValue / 1000;
                    }
                }
            }
            lbAvg.Text = string.Format("Средний остаток средств в {0} году: <b>{1:N2}</b> тыс. руб.", 
                comboYear.SelectedRow.Cells[0].Text, dtNote.Rows[0][0]);
            lbMax.Text = string.Format("Максимальный остаток средств в {0} году: <b>{1:N2}</b> тыс. руб.",
                comboYear.SelectedRow.Cells[0].Text, dtNote.Rows[0][2]);
            lbMin.Text = string.Format("Минимальный остаток средств в {0} году: <b>{1:N2}</b> тыс. руб.", 
                comboYear.SelectedRow.Cells[0].Text, dtNote.Rows[0][1]);
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("chartNoTarget");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart);
            foreach (DataRow item in dtChart.Rows)
            {
                double cellValue;
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    if (double.TryParse(item[i].ToString(), out cellValue))
                    {
                        item[i] = cellValue / 1000;
                    }
                }
            } 
            UltraChart1.DataSource = dtChart;
            UltraChart1.Data.SwapRowsAndColumns = true;
        }       

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("tableNoTarget");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "День", dtTable);
            foreach (DataRow item in dtTable.Rows)
            {
                double cellValue;
                for (int i = 1; i < dtTable.Columns.Count; i++)
                {
                    if (double.TryParse(item[i].ToString(), out cellValue))
                    {
                        item[i] = cellValue / 1000;
                    }
                }
            }
           // dtTable.Rows[0][6] = dtTable.Rows[0][6] == DBNull.Value ? 38542.17 : Convert.ToDouble(dtTable.Rows[0][6]);
            UltraWebGrid.DataSource = dtTable;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            if (e.Layout.Bands.Count == 0)
                return;
            if (e.Layout.Bands[0].Columns.Count == 0)
                return;
            if (Page.IsPostBack)
                return;

            UltraGridColumn col = e.Layout.Bands[0].Columns[0];
            col.CellStyle.Wrap = true;
            col.Width = 50;
            col.Header.Style.Wrap = true;
            col.CellStyle.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                col = e.Layout.Bands[0].Columns[i];
                col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                col.Width = 110;
                col.Header.Style.Wrap = true;
                CRHelper.FormatNumberColumn(col, "N2");
                col.Header.Caption = string.Format("{0}, тыс. руб.", col.Header.Caption);
            }

            col = e.Layout.Bands[0].Columns[10];
            col.Width = 120;

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.Header.RowLayoutColumnInfo.OriginY = 1;
            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Остаток";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 3;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Выплаты";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 4;
            ch.RowLayoutColumnInfo.SpanX = 3;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Поступления";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 7;
            ch.RowLayoutColumnInfo.SpanX = e.Layout.Bands[0].Columns.Count - 7;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            int textWidht = 200;
            int textHeight = 10;
            int lineLength = 50;

            double normativeValue = Convert.ToDouble(dtNote.Rows[0].ItemArray[0]);
            
            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(normativeValue)) - textHeight, textWidht, textHeight);
            text.SetTextString("Средний");
            e.SceneGraph.Add(text);

            Line line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(normativeValue));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(normativeValue));
            e.SceneGraph.Add(line);

            normativeValue = Convert.ToDouble(dtNote.Rows[0].ItemArray[1]);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(normativeValue)) - textHeight, textWidht, textHeight);
            text.SetTextString("Минимум");
            e.SceneGraph.Add(text);

            line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(normativeValue));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(normativeValue));
            e.SceneGraph.Add(line);

            normativeValue = Convert.ToDouble(dtNote.Rows[0].ItemArray[2]);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(normativeValue)) - textHeight, textWidht, textHeight);
            text.SetTextString("Максимум");
            e.SceneGraph.Add(text);

            line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(normativeValue));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(normativeValue));
            e.SceneGraph.Add(line);
        }

    /*    protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[6].Column.Format = "N2";
        }      */
    }
}
