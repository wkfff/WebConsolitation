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
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0001
{
    public partial class Default : CustomReportPage
    {
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int currentWidth = (int)Session["width_size"] - 50;
            uwgMaster.Width = (int)(currentWidth * 0.3 + 10);
            uwgSubjects.Width = (int)(currentWidth * 0.3 + 10);
            uwgOkved.Width = (int)(currentWidth * 0.3 + 10);
            lbOkvedTable.Width = (int)(currentWidth * 0.3 + 10);
            lbSubjectTable.Width = (int)(currentWidth * 0.3 + 10);
            DynamicChart.Width = (int)(currentWidth * 1);

            int currentHeight = (int)Session["height_size"] - 300;
            uwgSubjects.Height = (int)(currentHeight * 0.4 + 15);
            uwgOkved.Height = (int)(currentHeight * 0.4 + 15);
            uwgMaster.Height = (int)(currentHeight * 0.4 + 15);
            DynamicChart.Height = (int)(currentHeight * 0.6 - 20);            
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                uwgMaster.DataBind();
                this.UserParams.KDGroup.Value = uwgMaster.Rows[0].Cells[4].Text;
                TablesAndChartDataBinding(uwgMaster.Rows[0].Cells[0].Text);

                lbSubjectTable.Text = string.Format("Прирост недоимки по районам");
                lbOkvedTable.Text = string.Format("Прирост недоимки по ОКВЭД");
                lbChart.Text = string.Format("Динамика недоимки, переплаты, начислений и поступлений");
                CRHelper.SetConditionImageGridCells(uwgMaster, 3, 1, CompareAction.Greater, "~/images/ArrowUpRed.gif", "~/images/ArrowDownGreen.gif");
                CRHelper.SetConditionImageGridCells(uwgSubjects, 3, 1, CompareAction.Greater, "~/images/ArrowUpRed.gif", "~/images/ArrowDownGreen.gif");
                CRHelper.SetConditionImageGridCells(uwgOkved, 3, 1, CompareAction.Greater, "~/images/ArrowUpRed.gif", "~/images/ArrowDownGreen.gif");
            }
        }

        private void TablesAndChartDataBinding(string groupText)
        {
            uwgSubjects.DataBind();
            uwgOkved.DataBind();
            DynamicChart.DataBind();
            lbMaster.Text = groupText;
            lbChart.Text = string.Format("{0} - Динамика недоимки, переплаты, начислений и поступлений", groupText);            
        }

        DataTable dtMaster = new DataTable();
        DataTable dtSubjects = new DataTable();
        DataTable dtOkved = new DataTable();
        DataTable dtDynamicsChart = new DataTable();

        protected void uwgSubjects_DataBinding(object sender, EventArgs e)
        {   
            BindDataToGrid("table1", "Районы", dtSubjects, uwgSubjects);
        }

        protected void uwgOkved_DataBinding(object sender, EventArgs e)
        {
            BindDataToGrid("table2", "Раздел", dtOkved, uwgOkved);
        }

        protected void BindDataToGrid(string queryName, string seriesName, DataTable dtSource, UltraWebGrid uwGrid)
        {
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, seriesName, dtSource);
            foreach (DataRow item in dtSource.Rows)
            {                
                for (int i = 1; i <= 2; i++)
                {
                    double cellValue;
                    if (double.TryParse(item[i].ToString(), out cellValue))
                    {
                        item[i] = cellValue / 1000;
                    }
                }
            }            
            uwGrid.DataSource = dtSource;        
        }

        protected void uwgMaster_ActiveRowChange(object sender, RowEventArgs e)
        {
            this.UserParams.KDGroup.Value = e.Row.Cells[4].Text;
            TablesAndChartDataBinding(e.Row.Cells[0].Text);
        }

        protected void DynamicChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("chart");
            DataTable dtSourceTable = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtSourceTable);
            DataColumn col;
            for (int i = 0; i < dtSourceTable.Columns.Count - 1; i++)
            {
                col = new DataColumn(dtSourceTable.Columns[i].Caption, dtSourceTable.Columns[i].DataType);
                dtDynamicsChart.Columns.Add(col);
            }            
            foreach (DataRow sourceRow in dtSourceTable.Rows)
            {
                DataRow row = dtDynamicsChart.NewRow();
                row[0] = string.Format("{0} {1}", sourceRow[dtSourceTable.Columns.Count - 1], sourceRow[0]);
                for (int i = 1; i < dtDynamicsChart.Columns.Count; i++)
                {
                    double cellValue;
                    if (double.TryParse(sourceRow[i].ToString(), out cellValue))
                    {
                        row[i] = cellValue / 1000;
                    }
                }               
                dtDynamicsChart.Rows.Add(row);
            }
            DynamicChart.DataSource = dtDynamicsChart;
            //DynamicChart.Data.SwapRowsAndColumns = true;           
        }

        protected void uwgFns_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            if (e.Layout.Bands.Count == 0)
                return;
            if (e.Layout.Bands[0].Columns.Count == 0)
                return;

            if (Page.IsPostBack)
                return;
            foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.Header.Style.Wrap = true;
            }
            if (e.Layout.Bands[0].Columns.Count > 3)
            {
                UltraGridColumn col = e.Layout.Bands[0].Columns[0];
                col.Width = 110;
                col.CellStyle.Wrap = true;

                col = e.Layout.Bands[0].Columns[1];
                col.Width = 70;
                CRHelper.FormatNumberColumn(col, "N2");
                col.Header.Caption = string.Format("{0}, тыс. руб.", col.Header.Caption);

                col = e.Layout.Bands[0].Columns[2];
                col.Width = 70;
                CRHelper.FormatNumberColumn(col, "N2");
                col.Header.Caption = string.Format("{0}, тыс. руб.", col.Header.Caption);

                col = e.Layout.Bands[0].Columns[3];
                col.Width = 60;
                CRHelper.FormatNumberColumn(col, "P0");
            }
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count == 5)
            {
                e.Layout.Bands[0].Columns[4].Hidden = true;
                e.Layout.Bands[0].Columns[4].Width = 500;
            }
        }

        protected void uwgFNS_InitializeRow(object sender, RowEventArgs e)
        {
        }

        protected void uwgMaster_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("master");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dtMaster);

            // Переводим в тыс.руб.
            foreach (DataRow row in dtMaster.Rows)
            {
                double cellValue;
                if (double.TryParse(row[1].ToString(), out cellValue))
                {
                    row[1] = cellValue / 1000;
                }
                if (double.TryParse(row[2].ToString(), out cellValue))
                {
                    row[1] = cellValue / 1000;
                }                
            }
            uwgMaster.DataSource = dtMaster;
        }    
    }
}
