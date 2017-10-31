using System;
using System.Data;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0032_0001
{
    public partial class Default : CustomReportPage
    {
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int currentWidth = (int)Session["width_size"] - 70;
            UltraWebGrid.Width = (int)(currentWidth * 0.5);
            UltraChartIncoming.Width = (int)(currentWidth * 0.5);
            UltraChartDynamics.Width = (int)(currentWidth + 9);

            int currentHeight = (int)Session["height_size"] - 280;
            UltraWebGrid.Height = (int)(currentHeight * 0.6);
            UltraChartIncoming.Height = (int)(currentHeight * 0.6 + 40);
            UltraChartDynamics.Height = (int)(currentHeight * 0.4);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UserParams.KDGroup.Value = "[Доходы].[Группы КД].[Slicer]";
                UltraWebGrid.DataBind();
                UltraChartIncoming.DataBind();
                UltraChartDynamics.DataBind();
                lbGrid.Text = "Доходы областного бюджета на 26 июля 2008";
                lbChart1.Text = "Структура собственных доходов областного бюджета";
                lbChart2.Text = string.Format("Динамика исполнения областного бюджета за июль 2008 года по доходу: {0}", allIncomeRowName);                
            }
        }

        DataTable dtTable = new DataTable();
        DataTable dtChartIncoming = new DataTable();
        DataTable dtChartDynamics = new DataTable();      
  
        private const string incomeRowName = "Итого доходов";
        private const string withoutPayIncomeRowName = "Безвозмездные поступления";
        private const string allIncomeRowName = "Всего доходов";

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Группа", dtTable);

            DataRow row = dtTable.NewRow();
            row[0] = allIncomeRowName;
            double allPlan = 0;
            double allFact = 0;
            foreach (DataRow item in dtTable.Rows)
            {
                double cellValue;
                for (int i = 1; i < dtTable.Columns.Count - 2; i++)
                {
                    if (double.TryParse(item[i].ToString(), out cellValue))
                    {
                        item[i] = cellValue / 1000;
                    }
                }                

                if (item[0].ToString() ==  incomeRowName || item[0].ToString() == withoutPayIncomeRowName)
                {
                    if (double.TryParse(item[1].ToString(), out cellValue))
                    {
                        allPlan += cellValue;
                    }
                    if (double.TryParse(item[2].ToString(), out cellValue))
                    {
                        allFact += cellValue;
                    }
                }                
            }            
            row[1] = allPlan;
            row[2] = allFact;
            row[4] = allFact / allPlan;
            row[5] = "[Доходы].[Группы КД].[Slicer]";
            dtTable.Rows.Add(row);
            UltraWebGrid.DataSource = dtTable;
        }

        protected void UltraChartIncoming_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("chartIncoming");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Name", dtChartIncoming);
            foreach (DataRow item in dtChartIncoming.Rows)
            {
                double cellValue;
                if (double.TryParse(item[1].ToString(), out cellValue))
                {
                    item[1] = cellValue / 1000;
                }                
            }
            UltraChartIncoming.DataSource = dtChartIncoming;
        }

        protected void UltraChartDynamics_DataBinding(object sender, EventArgs e)
        {   
            string query = DataProvider.GetQueryText("chartDynamics");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Name", dtChartDynamics);
            foreach (DataRow item in dtChartDynamics.Rows)
            {
                double cellValue;
                if (double.TryParse(item[1].ToString(), out cellValue))
                {
                    item[1] = cellValue / 1000;
                }                
            }
            UltraChartDynamics.DataSource = dtChartDynamics;
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            string group = e.Row.Cells[0].Text;
            UserParams.KDGroup.Value = e.Row.Cells[e.Row.Cells.Count - 1].Text;                
            lbChart2.Text = string.Format("Динамика исполнения областного бюджета за июль 2008 года по доходу: {0}", group);
            UltraChartDynamics.DataBind();           
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            if (e.Layout.Bands[0].Columns.Count == 0)
                return;
            UltraGridColumn col = e.Layout.Bands[0].Columns[0];
            col.CellStyle.Wrap = true;
            col.Width = 150;
            col.Header.Style.Wrap = true;            

            col = e.Layout.Bands[0].Columns[1];            
            col.Width = 100;
            col.Header.Style.Wrap = true;
            CRHelper.FormatNumberColumn(col, "N2");
            col.Header.Caption = "План на июль, тыс. руб.";

            col = e.Layout.Bands[0].Columns[2];
            col.Width = 100;
            col.Header.Style.Wrap = true;
            CRHelper.FormatNumberColumn(col, "N2");
            col.Header.Caption = "Поступило всего за июль, тыс. руб.";

            col = e.Layout.Bands[0].Columns[3];
            col.Width = 75;
            col.Header.Style.Wrap = true;
            CRHelper.FormatNumberColumn(col, "N2");
            col.Header.Caption = "в том числе 26 июля, тыс. руб.";

            col = e.Layout.Bands[0].Columns[4];
            col.Width = 75;
            col.Header.Style.Wrap = true;
            CRHelper.FormatNumberColumn(col, "P2");
            col.Header.Caption = "% исполнения за июль";

            col = e.Layout.Bands[0].Columns[5];
            col.Hidden = true;
        }

        protected void UltraChartDynamics_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = string.Empty;
            lbChart2.Text = "<br>";
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = e.Row.Cells[0].Value.ToString();
            if (rowName == incomeRowName || 
                rowName == withoutPayIncomeRowName ||
                rowName == allIncomeRowName)
            {
                foreach (UltraGridCell cell in  e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }                
            }
        }
    }
}
