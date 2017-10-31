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
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using System.Drawing;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0002_0001
{
    public partial class Default : Dashboards.CustomReportPage
    {
        private DataSet tableDataSet = new DataSet();       
        private DataTable dtSubjectsFOChart = new DataTable();
        private DataTable dtDynamicsChart = new DataTable();
        private DataTable dtIndicatorsMaster = new DataTable();
        private DataTable dtIndicatorsDetail = new DataTable();
        private DataTable dtNormativeValue = new DataTable();
        private DataTable dtNote = new DataTable();

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int currentWidth = (int)Session["width_size"] - 70;
            IndicatorsGrid.Width = (int)(currentWidth * 0.5);
            DynamicChart.Width = (int)(currentWidth * 0.5);
            SubjectsFOChart.Width = (int)(currentWidth * 0.5);            
            WebPanel.Width = (int)(IndicatorsGrid.Width.Value) - (int)(ComboYear.Width.Value) - (int)(ComboQuarter.Width.Value) - 12;
            Indicator.Width = (int)(IndicatorsGrid.Width.Value) - (int)(ComboYear.Width.Value) - (int)(ComboQuarter.Width.Value) - 20;
            lbInfo.Width = (int)(currentWidth * 0.5);

            int currentHeight = (int)Session["height_size"] - 230;
            IndicatorsGrid.Height = (int)(currentHeight * 0.6);
            DynamicChart.Height = (int)(currentHeight * 0.4);
            SubjectsFOChart.Height = (int)(currentHeight * 0.6 - 20);
            lbInfo.Height = (int)(currentHeight * 0.4);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {           
            base.Page_Load(sender, e);            
            if (!Page.IsPostBack)
            {
                ComboYear.SelectedIndex = 9;
                ComboQuarter.SelectedIndex = 3;
            }

            WebPanel.Expanded = false;

            string periodUN = string.Format("[Период].[Месяц].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);            
            switch (ComboQuarter.SelectedIndex)
            {
                case 0:
                case 1:
                    periodUN += string.Format(".[Полугодие 1].[Квартал {0}]", ComboQuarter.SelectedIndex + 1);
                    break;

                case 2:
                case 3:
                    periodUN += string.Format(".[Полугодие 2].[Квартал {0}]", ComboQuarter.SelectedIndex + 1);
                    break;
            }            
            
            UserParams.PeriodYearQuater.Value = periodUN;
            if (!IsPostBack)
                IndicatorsGrid.DataBind();
            ShowHideCharts(false);
            ShowNote();
        }

        private void ShowHideCharts(bool show)
        {
            DynamicChart.Visible = show;
            SubjectsFOChart.Visible = show;         
        }

        protected void IndicatorsGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("indicatorsMaster");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtIndicatorsMaster);

            query = DataProvider.GetQueryText("indicatorsDetail");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtIndicatorsDetail);

            tableDataSet = new DataSet();
            tableDataSet.Tables.Add(dtIndicatorsMaster);
            tableDataSet.Tables.Add(dtIndicatorsDetail);

            tableDataSet.Relations.Add(dtIndicatorsMaster.Columns[0], dtIndicatorsDetail.Columns[2]);

            IndicatorsGrid.DataSource = tableDataSet.Tables[0];
        }

        private void ShowNote()
        {
            string query = DataProvider.GetQueryText("note");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtNote);
            string info = string.Empty;
            foreach (DataColumn col in dtNote.Columns)
            {
                info += string.Format("<b>{0}</b>: {1}<br>", col.Caption, dtNote.Rows[0][col.Caption]);
            }          
            lbInfo.Text = info;
        }

        protected void DynamicChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("dynamicsChart");
            DataTable dtSourceTable = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtSourceTable);

            query = DataProvider.GetQueryText("normativeValue");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtNormativeValue);

            DataColumn col = new DataColumn("Квартал", typeof(string));
            dtDynamicsChart.Columns.Add(col);
            col = new DataColumn("Значение", typeof(double));
            dtDynamicsChart.Columns.Add(col);
            foreach (DataRow sourceRow in dtSourceTable.Rows)
            {
                DataRow row = dtDynamicsChart.NewRow();
                row[0] = string.Format("{0}{1}", sourceRow[2], sourceRow[0]);
                row[1] = Convert.ToDouble(sourceRow[1]);
                dtDynamicsChart.Rows.Add(row);
            }

            DynamicChart.DataSource = dtDynamicsChart;
            DynamicChart.Data.SwapRowsAndColumns = true;            
        }

        protected void SubjectsFOChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("subjectChart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesname",  dtSubjectsFOChart);

            query = DataProvider.GetQueryText("normativeValue");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtNormativeValue);
            
            SubjectsFOChart.DataSource = dtSubjectsFOChart;        
        }

        protected void IndicatorsGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Hidden = true;
            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[1].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[1].Columns[0].Width = 250;
            e.Layout.Bands[1].Columns[2].Hidden = true;
            e.Layout.Bands[1].Columns[3].Hidden = true;
            e.Layout.Bands[1].Columns[1].Header.Caption = "Значение индикатора";
            e.Layout.Bands[1].Columns[1].Header.Style.Wrap = true;
            CRHelper.FormatNumberColumn(IndicatorsGrid.Bands[1].Columns[1], "N2");
            e.Layout.Bands[1].Columns[1].Width = 90;
            e.Layout.GroupByBox.Hidden = true;

            string caption = string.Empty;
            foreach (UltraGridColumn col in e.Layout.Bands[1].Columns)
            {
                caption = col.Header.Caption;
            }
        }
        
        protected void DynamicChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double normativeValue = Convert.ToDouble(dtNormativeValue.Rows[0].ItemArray[0]);

            int textWidht = 200;
            int textHeight = 10;
            int lineLength = 250;

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(normativeValue)) - textHeight, textWidht, textHeight);
            text.SetTextString("Нормативное значение");
            e.SceneGraph.Add(text);

            Line line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(normativeValue));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(normativeValue));
            e.SceneGraph.Add(line);
        }

        protected void SubjectsFOChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double normativeValue = Convert.ToDouble(dtNormativeValue.Rows[0].ItemArray[0]);

            int textWidht = 200;
            int textHeight = 10;
            int lineLength = 250;

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)xAxis.Map(0), ((int)yAxis.Map(normativeValue)) - textHeight, textWidht, textHeight);
            text.SetTextString("Нормативное значение");
            e.SceneGraph.Add(text);

            Line line = new Line();
            line.PE.Fill = Color.Red;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.Map(0), (int)yAxis.Map(normativeValue));
            line.p2 = new Point(((int)xAxis.Map(0)) + lineLength, (int)yAxis.Map(normativeValue));
            e.SceneGraph.Add(line);
        }

        protected void IndicatorsGrid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            string stateArea = e.Row.Cells[2].Text;
            string region = e.Row.Cells[0].Text;
            string regionTemplate = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}].[{1}]", stateArea, region);
            string stateAreaTemplate = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]", stateArea);
            UserParams.Region.Value = regionTemplate;
            UserParams.StateArea.Value = stateAreaTemplate;
            DynamicChart.DataBind();
            SubjectsFOChart.DataBind();
            ShowHideCharts(true);
        }

        protected void SubmitButton_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            IndicatorsGrid.DataBind();
        }

        protected void IndicatorsGrid_InitializeRow(object sender, RowEventArgs e)
        {            
            if (e.Row.Band.Index == 0)
                return;            
            if (Convert.ToDouble(e.Row.Cells[3].Value) == 1)
            {
               // e.Row.Cells[1].Style.BackColor = Color.Pink;
                e.Row.Cells[1].Style.BackgroundImage = "~/images/BallRed.gif";
            }
            else if (Convert.ToDouble(e.Row.Cells[3].Value) == 0)
            {
                //e.Row.Cells[1].Style.BackColor = Color.LightGreen;
                e.Row.Cells[1].Style.BackgroundImage = "~/images/BallGreen.gif";
            }
        }
    }
}
