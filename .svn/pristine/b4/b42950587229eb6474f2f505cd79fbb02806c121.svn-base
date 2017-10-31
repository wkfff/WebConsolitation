using System;
using System.Data;
using System.Web.UI;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.UFK_0008_0001
{
    public partial class Default : CustomReportPage
    {
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            try
            {
                int currentWidth = (int) Session["width_size"] - 65;
                UltraWebGrid.Width = (int) (currentWidth);
                UltraChart1.Width = (int) (currentWidth);

                int currentHeight = (int) Session["height_size"] - 260;
                UltraWebGrid.Height = (int) (currentHeight*0.4 - 10);
                UltraChart1.Height = (int) (currentHeight*0.6);
            }
            catch
            {
                
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UltraWebGrid.DataBind();
                lbGrid.Text = "Темп роста доходов областного бюджета на 25 июля 2008г.";
                string group = UltraWebGrid.Rows[0].Cells[0].Text;
                UserParams.KDGroup.Value = string.Format("[Доходы].[Группы КД].[Все группы КД].[Итого доходов].[{0}]", group);
                lbChart1.Text = string.Format("Динамика за 2008 год по доходу: {0}", group);
                UltraChart1.DataBind();
            }
        }

        DataTable dtTable = new DataTable();
        DataTable dtChart1 = new DataTable();

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Группа", dtTable);

            DataRow row = dtTable.NewRow();            
            foreach (DataRow item in dtTable.Rows)
            {   
                // с первой по шестую переводим в тысячи
                for (int i = 1; i <= 6; i++)
                {
                    double cellValue;
                    if (double.TryParse(item[i].ToString(), out cellValue))
                    {
                        item[i] = cellValue / 1000;
                    }
                }
            }            
            UltraWebGrid.DataSource = dtTable;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("chart1CurrentFact");
            DataTable dtCurrentFact = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Name", dtCurrentFact);

            query = DataProvider.GetQueryText("chart1LastFact");
            DataTable dtLastFact = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Name", dtLastFact);

            query = DataProvider.GetQueryText("chart1CurrentPlan");
            DataTable dtCurrentPlan = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Name", dtCurrentPlan);
                        
            DataColumn col = new DataColumn(dtCurrentFact.Columns[0].Caption, dtCurrentFact.Columns[0].DataType);
            dtChart1.Columns.Add(col);
            col = new DataColumn("Факт текущего года", dtCurrentFact.Columns[1].DataType);
            dtChart1.Columns.Add(col);
            col = new DataColumn("Факт предыдущего года", dtLastFact.Columns[1].DataType);
            dtChart1.Columns.Add(col);
            col = new DataColumn("План на текущий год", dtCurrentPlan.Columns[1].DataType);
            dtChart1.Columns.Add(col);

            // Месяцев ведь двенадцать?
            for (int i = 0; i < 12; i++)
            {
                DataRow row = dtChart1.NewRow();
                row[0] = dtCurrentFact.Rows[i][0].ToString();
                double cellValue;                
                if (double.TryParse(dtCurrentFact.Rows[i][1].ToString(), out cellValue))
                {
                    row[1] = cellValue / 1000;
                }
                if (double.TryParse(dtLastFact.Rows[i][1].ToString(), out cellValue))
                {
                    row[2] = cellValue / 1000;
                }
                if (double.TryParse(dtCurrentPlan.Rows[i][1].ToString(), out cellValue))
                {
                    row[3] = cellValue / 1000;
                }
                dtChart1.Rows.Add(row);
            }
            UltraChart1.DataSource = dtChart1;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.Header.Style.Wrap = true;
            }

            UltraGridColumn col = e.Layout.Bands[0].Columns[0];
            col.CellStyle.Wrap = true;
                
            col = e.Layout.Bands[0].Columns[1];
            col.Header.Caption = "Факт на 25.07.2008, тыс. руб.";
            CRHelper.FormatNumberColumn(col, "N2");

            col = e.Layout.Bands[0].Columns[2];
            col.Header.Caption = "План на 2008г., тыс. руб.";
            CRHelper.FormatNumberColumn(col, "N2");

            col = e.Layout.Bands[0].Columns[3];
            col.Header.Caption = "План на август 2008г., тыс. руб.";
            CRHelper.FormatNumberColumn(col, "N2");

            col = e.Layout.Bands[0].Columns[4];
            col.Header.Caption = "Факт на 25.07.2007, тыс. руб.";
            CRHelper.FormatNumberColumn(col, "N2");

            col = e.Layout.Bands[0].Columns[5];
            col.Header.Caption = "Факт на 2007г., тыс. руб.";
            CRHelper.FormatNumberColumn(col, "N2");

            col = e.Layout.Bands[0].Columns[6];
            col.Header.Caption = "Факт на август 2008г., тыс. руб.";
            CRHelper.FormatNumberColumn(col, "N2");

            col = e.Layout.Bands[0].Columns[7];
            col.Header.Caption = "Темп роста плановый 2008г. к 2007г.";
            CRHelper.FormatNumberColumn(col, "P2");

            col = e.Layout.Bands[0].Columns[8];
            col.Header.Caption = "Темп роста плановый июль 2008 к июлю 2007";
            CRHelper.FormatNumberColumn(col, "P2");

            col = e.Layout.Bands[0].Columns[9];
            col.Header.Caption = "Темп роста фактический 25.07.2008 к 25.07.2007";
            CRHelper.FormatNumberColumn(col, "P2");

            e.Layout.Bands[0].Columns[0].Width = 200;
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            string group = e.Row.Cells[0].Text;
            UserParams.KDGroup.Value = string.Format("[Доходы].[Группы КД].[Все группы КД].[Итого доходов].[{0}]", group);
            lbChart1.Text = string.Format("Динамика темпа роста доходов областного бюджета за июль 2008 года по доходу: {0}", group);
            UltraChart1.DataBind();    
        }

        protected void UltraChart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = string.Empty;
            lbChart1.Text = "<br>";      
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {            
            if (Convert.ToDouble(e.Row.Cells[9].Value) < Convert.ToDouble(e.Row.Cells[8].Value))
            {
              //  e.Row.Cells[9].Style.BackColor = Color.Pink;
               e.Row.Cells[9].Style.BackgroundImage = "~/images/ArrowDownRed.gif";
            }
            else if (Convert.ToDouble(e.Row.Cells[9].Value) > Convert.ToDouble(e.Row.Cells[8].Value))
            {
              //  e.Row.Cells[9].Style.BackColor = Color.LightGreen;
                e.Row.Cells[9].Style.BackgroundImage = "~/images/ArrowUpGreen.gif";
            }
            e.Row.Cells[9].Style.CustomRules = "background-repeat: no-repeat; background-position: left; margin-left: 5px";
        }
       
    }
}
