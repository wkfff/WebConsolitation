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
using Krista.FM.Server.Dashboards.Common;
using Microsoft.AnalysisServices.AdomdClient;
using System.Drawing;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.UFK_0017_0001
{
    public partial class Default_targer : CustomReportPage
    {
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int currentWidth = (int)Session["width_size"] - 65;
            UltraWebGrid.Width = (int)(currentWidth * 0.5);
            UltraChart1.Width = (int)(currentWidth * 0.5);
            UltraChart2.Width = (int)(currentWidth * 0.5);
            lbChart1.Width = (int)(currentWidth * 0.5);
            lbChart2.Width = (int)(currentWidth * 0.5);

            int currentHeight = (int)Session["height_size"] - 300;
            UltraWebGrid.Height = (int)(currentHeight);
            UltraChart1.Height = (int)(currentHeight  * 0.5 - ((int)lbChart1.Height.Value) + 25);
            UltraChart2.Height = (int)(currentHeight  * 0.5 - ((int)lbChart1.Height.Value) + 25);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DateTime lastDay;
                int periodDetailMode;

                if (UserParams.PeriodDayFO.Value != string.Empty)
                {
                    lastDay = CRHelper.PeriodDayFoDate(UserParams.PeriodDayFO.Value);
                    periodDetailMode = CRHelper.PeriodDayFoDetailLevel(UserParams.PeriodDayFO.Value);
                }
                else
                {
                    lastDay = new DateTime(2008, 8, 12);
                }
                date.Value = lastDay;                
            }

            string pValue = CRHelper.PeriodMemberUName("[Период].[День].[Данные всех периодов]", (DateTime)date.Value, 5);
            
            if (!Page.IsPostBack || !UserParams.PeriodDayFO.ValueIs(pValue))
            {
                DateTime choosenDate = (DateTime)date.Value;
                lbTitle.Text = string.Format("Остаток целевых средств федерального бюджета на  {0} {1}",
                    choosenDate.Day, CRHelper.RusMonthGenitive(choosenDate.Month));
                UserParams.KDGroup.Value = "[Межбюджетные трансферты].[УФК_Сопоставимый].[Все]";
                lbChart1.Text = string.Format("Движение остатка целевых средств федерального бюджета за {0}г.", ((DateTime)date.Value).Year);
                lbChart2.Text = string.Format("Движение остатка целевых средств федерального бюджета за {0}",
                    CRHelper.RusMonth(((DateTime)(date.Value)).Month));
                UserParams.PeriodDayFO.Value = pValue;
                UserParams.PeriodYear.Value = CRHelper.PeriodMemberUName("[Период].[День].[Данные всех периодов]", (DateTime)date.Value, 1);
                UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName("[Период].[День].[Данные всех периодов]", (DateTime)date.Value, 4); 
                
                UltraWebGrid.DataBind();
                UltraChart1.DataBind();
                UltraChart2.DataBind();
            }
        }

        DataTable dtTable = new DataTable();
        DataTable dtChart1 = new DataTable();
        DataTable dtChart2 = new DataTable();

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("tableTarget");
            CellSet cls = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(query);
            DataTable dtSource = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(cls, dtSource, "Группа");

            DataColumn col = new DataColumn("Код", typeof(System.String));
            dtTable.Columns.Add(col);
            foreach(DataColumn column in dtSource.Columns)
            {
                col = new DataColumn(column.Caption, column.DataType);
                dtTable.Columns.Add(col);
            }                        
            foreach (DataRow item in dtSource.Rows)
            {
                DataRow row = dtTable.NewRow();                
                for (int i = 1; i < dtSource.Columns.Count - 1; i++)
                {
                    row[1] = item[0].ToString();
                    double cellValue;
                    if (double.TryParse(item[i].ToString(), out cellValue))
                    {
                        row[i + 1] = cellValue;
                    }
                    row[dtSource.Columns.Count] = item[dtSource.Columns.Count - 1].ToString();
                }
                dtTable.Rows.Add(row);
            }
            if (cls != null)
            {                
                for (int i = 0; i < cls.Axes[1].Positions.Count; i++)
                {
                    try
                    {
                        dtTable.Rows[i][0] = cls.Axes[1].Positions[i].Members[0].MemberProperties[0].Value.ToString();
                    }
                    catch { }
                }
            }
            UltraWebGrid.DataSource = dtTable;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
       {
            string query = DataProvider.GetQueryText("chart1Target");            
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Name", dtChart1);
            UltraChart1.DataSource = dtChart1;
           // UltraChart1.Data.SwapRowsAndColumns = true;            
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("chart2Target");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Name", dtChart2);
            UltraChart2.DataSource = dtChart2;
            UltraChart2.Data.SwapRowsAndColumns = true;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.Header.Style.Wrap = true;
            }            
            UltraGridColumn col = e.Layout.Bands[0].Columns[0];
            col.CellStyle.Wrap = true;
            col.Width = 50;

            col = e.Layout.Bands[0].Columns[1];
            col.CellStyle.Wrap = true;
            col.Width = 200;

            col = e.Layout.Bands[0].Columns[8];
            col.Hidden = true;

            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.Header.RowLayoutColumnInfo.OriginY = 1;
            }

            int multiHeaderPos = 2;
            string[] captions;

            Infragistics.WebUI.UltraWebGrid.ColumnHeader ch = new Infragistics.WebUI.UltraWebGrid.ColumnHeader(true);
            captions = e.Layout.Bands[0].Columns[2].Header.Caption.Split(';');
            ch.Caption = captions[0];
            e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("За {0:dd.MM.yyyy}", (DateTime)date.Value);            
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;            
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            multiHeaderPos += 1;
            ch.RowLayoutColumnInfo.SpanX = 1;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
            e.Layout.Bands[0].Columns[3].Hidden = true;
            
            for (int i = 4; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {

                ch = new Infragistics.WebUI.UltraWebGrid.ColumnHeader(true);
                captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];
                e.Layout.Bands[0].Columns[i].Header.Caption = string.Format("За {0:dd.MM.yyyy}", (DateTime)date.Value);
                e.Layout.Bands[0].Columns[i + 1].Header.Caption = "С начала года";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N2");
                e.Layout.Bands[0].Columns[i + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }            
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            UserParams.KDGroup.Value = 
                e.Row.Cells[8].Text == string.Empty ? "[Межбюджетные трансферты].[УФК_Сопоставимый].[Все]" : e.Row.Cells[8].Text;                         
            string filterName = e.Row.Cells[8].Text == "Итого субсидий и субвенций" ? string.Empty : string.Format("{0}", e.Row.Cells[1].Text);
            lbChart1.Text = string.Format("Движение остатка целевых средств федерального бюджета {0}г.", ((DateTime)date.Value).Year);
            lbChart2.Text = string.Format("Движение остатка целевых средств федерального бюджета за {0}", 
                CRHelper.RusMonth(((DateTime)(date.Value)).Month));
            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        protected void chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.Font = new Font("Verdana", 8);
            e.Text = "Нет данных";
            e.LabelStyle.VerticalAlign = StringAlignment.Near;
        }
    }
}
