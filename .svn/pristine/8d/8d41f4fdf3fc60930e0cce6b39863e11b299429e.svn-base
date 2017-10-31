using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core.Layers;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0001_0004_FFPR
{
    public partial class Default : CustomReportPage
    {
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int currentWidth = (int)Session["width_size"] - 50;
            UltraWebGridFFPR.Width = (int)(currentWidth + 10);
            UltraChartFFPR1.Width = (int)(currentWidth * 0.5);
            UltraChartFFPR2.Width = (int)(currentWidth * 0.5);           

            int currentHeight = (int)Session["height_size"] - 280;
            UltraWebGridFFPR.Height = (int)(currentHeight * 0.5);
            UltraChartFFPR1.Height = (int)(currentHeight * 0.5);
            UltraChartFFPR2.Height = (int)(currentHeight * 0.5);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            curStateAreaNameFFPR = string.Empty;
            if (!Page.IsPostBack)
            {
                ComboYear.SelectedIndex = 10;
            }
            string pValue = string.Format("[Период].[Год].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);

            //Обновляем таблицу только, если либо параметр периода был изменен, либо загружаемся в первый раз

            if (!Page.IsPostBack || !UserParams.PeriodYear.ValueIs(pValue))
            {
                UserParams.PeriodYear.Value = string.Format("[Период].[Год].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);
                UltraWebGridFFPR.DataBind();
            }
            ShowHideCharts(false);
        }        

        #region ФФПР
        DataTable dtMasterFFPR = new DataTable();
        DataTable dtDetailFFPR = new DataTable();
        DataTable dtChartFFPR1fondsall = new DataTable();
        DataTable dtChartFFPR1fonds = new DataTable();
        DataTable dtFFPRChart2People1 = new DataTable();
        DataTable dtFFPRChart2People2 = new DataTable();
        DataTable dtFondsAverage = new DataTable();
        DataSet tableDataSetFFPR = new DataSet();
        //имя текущего субъекта выбранного во втором бенде мастер-таблицы
        private string curStateAreaNameFFPR = string.Empty;

        private void ShowHideCharts(bool show)
        {
            UltraChartFFPR1.Visible = show;
            UltraChartFFPR2.Visible = show;
        }

        protected void UltraWebGridFFPR_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FFPRMasterTable");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtMasterFFPR);

            query = DataProvider.GetQueryText("FFPRDetailTable");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtDetailFFPR);

            tableDataSetFFPR.Tables.Add(dtMasterFFPR);
            tableDataSetFFPR.Tables.Add(dtDetailFFPR);

            tableDataSetFFPR.Relations.Add(dtMasterFFPR.Columns[0], dtDetailFFPR.Columns[1]);
            UltraWebGridFFPR.DataSource = tableDataSetFFPR.Tables[0].DefaultView;
        }

        protected void UltraChartFFPR1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FFPRChart1fondsAll");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChartFFPR1fondsall);
            query = DataProvider.GetQueryText("FFPRChart1fonds");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChartFFPR1fonds);

            UltraChartFFPR1.ChartType = ChartType.Composite;

            ChartArea area = new ChartArea();
            UltraChartFFPR1.CompositeChart.ChartAreas.Add(area);

            AxisItem axisX = new AxisItem();
            axisX.OrientationType = AxisNumber.X_Axis;
            axisX.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            axisX.DataType = AxisDataType.String;
            axisX.Labels.ItemFormat = AxisItemLabelFormat.ItemLabel;
            axisX.Labels.OrientationAngle = 210;
            axisX.Labels.Flip = true;
            axisX.Labels.Orientation = TextOrientation.Custom;
            axisX.Labels.Orientation = TextOrientation.VerticalLeftFacing;

            AxisItem axisX2 = new AxisItem();
            axisX2.OrientationType = AxisNumber.X2_Axis;
            axisX2.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            axisX2.DataType = AxisDataType.String;
            axisX2.Visible = false;

            AxisItem axisY = new AxisItem();
            axisY.OrientationType = AxisNumber.Y_Axis;
            axisY.DataType = AxisDataType.Numeric;
            axisY.Labels.ItemFormat = AxisItemLabelFormat.DataValue;

            AxisItem axisY2 = new AxisItem();
            axisY2.OrientationType = AxisNumber.Y2_Axis;
            axisY2.DataType = AxisDataType.Numeric;
            axisY2.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            axisY2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;

            area.Axes.Add(axisX);
            area.Axes.Add(axisX2);
            area.Axes.Add(axisY);
            area.Axes.Add(axisY2);

            ChartLayerAppearance layer1 = new ChartLayerAppearance();
            ChartLayerAppearance layer2 = new ChartLayerAppearance();

            layer1.ChartType = ChartType.AreaChart;
            layer2.ChartType = ChartType.LineChart;

            for (int i = 1; i < dtChartFFPR1fonds.Columns.Count; i++)
            {
                NumericSeries series1 = new NumericSeries();
                series1.Label = dtChartFFPR1fonds.Columns[i].ColumnName;
                series1.Data.LabelColumn = dtChartFFPR1fonds.Columns[0].ColumnName;
                series1.Data.ValueColumn = dtChartFFPR1fonds.Columns[i].ColumnName;
                series1.Data.DataSource = dtChartFFPR1fonds;
                //series1.DataBind();
                UltraChartFFPR1.CompositeChart.Series.Add(series1);
                layer1.Series.Add(series1);
            }

            for (int i = 1; i < dtChartFFPR1fondsall.Columns.Count; i++)
            {
                NumericSeries series2 = new NumericSeries();
                series2.Label = dtChartFFPR1fondsall.Columns[i].ColumnName;
                series2.Data.LabelColumn = dtChartFFPR1fondsall.Columns[0].ColumnName;
                series2.Data.ValueColumn = dtChartFFPR1fondsall.Columns[i].ColumnName;
                series2.Data.DataSource = dtChartFFPR1fondsall;
                //series2.DataBind();
                UltraChartFFPR1.CompositeChart.Series.Add(series2);
                layer2.Series.Add(series2);
            }
            layer1.ChartArea = area;
            layer1.AxisX = axisX;
            layer1.AxisY = axisY;
            layer1.SwapRowsAndColumns = true;
            

            layer2.ChartArea = area;
            layer2.AxisX = axisX2;
            layer2.AxisY = axisY2;
            layer2.SwapRowsAndColumns = true;

            UltraChartFFPR1.CompositeChart.ChartLayers.Add(layer1);
            UltraChartFFPR1.CompositeChart.ChartLayers.Add(layer2);           
        }

        protected void UltraChartFFPR2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FFPRChart2People1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtFFPRChart2People1); 
            
            query = DataProvider.GetQueryText("FFPRChart2People2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtFFPRChart2People2);        

            ChartArea area = new ChartArea();
            UltraChartFFPR2.CompositeChart.ChartAreas.Add(area);

            UltraChartFFPR2.ChartType = ChartType.Composite;

            AxisItem axisX = new AxisItem();
            axisX.OrientationType = AxisNumber.X_Axis;
            axisX.SetLabelAxisType = SetLabelAxisType.GroupBySeries;
            axisX.DataType = AxisDataType.String;
            axisX.Labels.OrientationAngle = 210;
            axisX.Labels.Flip = true;
            axisX.Labels.Orientation = TextOrientation.Custom;
            axisX.Labels.ItemFormat = AxisItemLabelFormat.ItemLabel;
            axisX.Labels.Orientation = TextOrientation.VerticalLeftFacing;

            AxisItem axisX2 = new AxisItem();
            axisX2.OrientationType = AxisNumber.X2_Axis;
            axisX2.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            axisX2.DataType = AxisDataType.String;
            axisX2.Visible = false;

            AxisItem axisY = new AxisItem();
            axisY.OrientationType = AxisNumber.Y_Axis;
            axisY.DataType = AxisDataType.Numeric;
            axisY.Labels.ItemFormat = AxisItemLabelFormat.DataValue;

            AxisItem axisY2 = new AxisItem();
            axisY2.OrientationType = AxisNumber.Y2_Axis;
            axisY2.DataType = AxisDataType.Numeric;
            axisY2.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            axisY2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;

            area.Axes.Add(axisX);
            area.Axes.Add(axisX2);
            area.Axes.Add(axisY);
            area.Axes.Add(axisY2);

            ChartLayerAppearance layer1 = new ChartLayerAppearance();
            ChartLayerAppearance layer2 = new ChartLayerAppearance();

            layer1.ChartType = ChartType.ColumnChart;
            layer2.ChartType = ChartType.LineChart;

            for (int i = 1; i < dtFFPRChart2People1.Columns.Count; i++)
            {
                NumericSeries series1 = new NumericSeries();
                series1.Label = dtFFPRChart2People1.Columns[i].ColumnName;
                series1.Data.LabelColumn = dtFFPRChart2People1.Columns[0].ColumnName;
                series1.Data.ValueColumn = dtFFPRChart2People1.Columns[i].ColumnName;
                series1.Data.DataSource = dtFFPRChart2People1;                
                UltraChartFFPR2.CompositeChart.Series.Add(series1);
                layer1.Series.Add(series1);
            }

            for (int i = 1; i < dtFFPRChart2People2.Columns.Count; i++)
            {
                NumericSeries series2 = new NumericSeries();
                series2.Label = dtFFPRChart2People2.Columns[i].ColumnName;
                series2.Data.LabelColumn = dtFFPRChart2People2.Columns[0].ColumnName;
                series2.Data.ValueColumn = dtFFPRChart2People2.Columns[i].ColumnName;
                series2.Data.DataSource = dtFFPRChart2People2;                
                UltraChartFFPR2.CompositeChart.Series.Add(series2);
                layer2.Series.Add(series2);
            }

            layer1.ChartArea = area;
            layer1.AxisX = axisX;
            layer1.AxisY = axisY;
            layer1.SwapRowsAndColumns = true;


            layer2.ChartArea = area;
            layer2.AxisX = axisX2;
            layer2.AxisY = axisY2;
            layer2.SwapRowsAndColumns = true;

            UltraChartFFPR2.CompositeChart.ChartLayers.Add(layer1);
            UltraChartFFPR2.CompositeChart.ChartLayers.Add(layer2);     
           
        }

        protected void UltraWebGridFFPR_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            // по бенду регионов пока делать ничего не будем.
            if (e.Row.Band.Index == 0)
                return;

            string stateArea = e.Row.Cells[1].Text;                        
            string stateAreaTemplate = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]", stateArea);
            UserParams.StateArea.Value = stateAreaTemplate;
            curStateAreaNameFFPR = e.Row.Cells[1].Text;
            UltraChartFFPR1.DataBind();
            UltraChartFFPR2.DataBind();
            ShowHideCharts(true);
        }

        protected void UltraWebGridFFPR_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            for (int i = 1; i < UltraWebGridFFPR.Bands[0].Columns.Count; i++)
            {
                UltraWebGridFFPR.Bands[0].Columns[i].Hidden = true;
            }
           
            for (int i = 3; i < UltraWebGridFFPR.Bands[1].Columns.Count; i = i + 2)
            {
                CRHelper.FormatNumberColumn(UltraWebGridFFPR.Bands[1].Columns[i], "P0");
                UltraWebGridFFPR.Bands[1].Columns[i].Width = 75;
            }            
            for (int i = 2; i < UltraWebGridFFPR.Bands[1].Columns.Count; i = i + 2)
            {
                CRHelper.FormatNumberColumn(UltraWebGridFFPR.Bands[1].Columns[i], "N2");
                UltraWebGridFFPR.Bands[1].Columns[i].Width = 100;
            }            

            e.Layout.Bands[0].Columns[0].Width = 250;

            UltraWebGridFFPR.Bands[1].Columns[1].Hidden = true;

            UltraWebGridFFPR.Bands[0].Columns[0].CellStyle.Wrap = true;
            UltraWebGridFFPR.Bands[1].Columns[0].CellStyle.Wrap = true;
            UltraWebGridFFPR.Bands[0].Columns[0].Width = 250;
            UltraWebGridFFPR.Bands[1].Columns[0].Width = 250;

            if (IsPostBack)
                return;

            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridColumn c in e.Layout.Bands[1].Columns)
            {
                c.Header.RowLayoutColumnInfo.OriginY = 1;
            }

            int multiHeaderPos = 1;
            string[] captions;

            for (int i = 2; i < e.Layout.Bands[1].Columns.Count; i = i + 2)
            {
                Infragistics.WebUI.UltraWebGrid.ColumnHeader ch = new Infragistics.WebUI.UltraWebGrid.ColumnHeader(true);
                captions = e.Layout.Bands[1].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];
                e.Layout.Bands[1].Columns[i].Header.Caption = captions[1];
                e.Layout.Bands[1].Columns[i + 1].Header.Caption = e.Layout.Bands[1].Columns[i + 1].Header.Caption.Split(';')[1];
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[1].HeaderLayout.Add(ch);
            }
            e.Layout.GroupByBox.Hidden = true;
        }

        protected void UltraChartFFPR1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (UltraChartFFPR1.CompositeChart.ChartLayers.Count == 0)
                return;

            string query = DataProvider.GetQueryText("FFPRAveragefonds");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtFondsAverage);

            ChartLayer layer = UltraChartFFPR1.CompositeChart.ChartLayers[0].ChartLayer;

            IAdvanceAxis xAxis = (IAdvanceAxis)layer.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)layer.Grid["Y"];            
          
            double avgBeforeFO = Convert.ToDouble(dtFondsAverage.Rows[0].ItemArray[1]);
            double avgAfterFO = Convert.ToDouble(dtFondsAverage.Rows[0].ItemArray[2]);
            double avgBeforeRF = Convert.ToDouble(dtFondsAverage.Rows[0].ItemArray[3]);
            double avgAfterRF = Convert.ToDouble(dtFondsAverage.Rows[0].ItemArray[4]);

            int textWidht = 200;
            int textHeight = 10;
            int lineLength = 150;

            Text text = new Text();
            text.PE.Fill = Color.Black;  
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(avgBeforeRF)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя БО по РФ до: {0:N4}", avgBeforeRF));
            e.SceneGraph.Add(text);

            Line line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(avgBeforeRF));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(avgBeforeRF));
            e.SceneGraph.Add(line);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(avgAfterRF)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя БО по РФ после: {0:N4}", avgAfterRF));
            e.SceneGraph.Add(text);

            line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(avgAfterRF));
            line.p2 = new Point(((int)xAxis.Map(0)) + (lineLength), (int)yAxis.Map(avgAfterRF));
            e.SceneGraph.Add(line);

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(avgBeforeFO)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя БО по ФО до: {0:N4}", avgBeforeFO));
            e.SceneGraph.Add(text);

            line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 3;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(avgBeforeFO));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(avgBeforeFO));
            e.SceneGraph.Add(line);             

            text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(avgAfterFO)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя БО по ФО после: {0:N4}", avgAfterFO));
            e.SceneGraph.Add(text);

            line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(avgAfterFO));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(avgAfterFO));
            e.SceneGraph.Add(line);                       
        }

        #endregion      

    }    
}
