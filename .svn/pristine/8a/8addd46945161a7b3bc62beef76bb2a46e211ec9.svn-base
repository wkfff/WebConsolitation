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

namespace Krista.FM.Server.Dashboards.reports.MFRF_0001_0001
{
    public partial class Default : Dashboards.CustumReportPage
    {
        private DataSet tableDataSet = new DataSet();
        private DataTable dtLevel0 = new DataTable();
        private DataTable dtLevel1 = new DataTable();
        private DataTable chartDT = new DataTable();

        //имя текущего субъекта выбранного во втором бенде мастер-таблицы
        private string curStateAreaName = string.Empty;

        protected void Page_Preload(object sender, EventArgs e)
        {
        	MainIndexRef.HRef = Convert.ToString(Session["MainIndexRef"]);

            int currentWidth = (int)Session["width_size"] - 40 ;
            paramPanel.Width = currentWidth - 170;
            chart.Width = currentWidth;
            MasterTable.Width = currentWidth;


            int currentHeight = (int)Session["height_size"] - 40;
            chart.Height = (int)(currentHeight * 0.5);
            MasterTable.Height = (int)(currentHeight * 0.25);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {

            base.Page_Load(sender, e);
            paramPanel.Expanded = false;
            curStateAreaName = string.Empty;

            if (!Page.IsPostBack)
            {
                MasterTable.DataBind();
            }

            ShowHideChart(false);
        }

        private void ShowHideChart(bool show)
        {
            chart.Visible = show;
            chartMessage.Visible = !show;

        }

        protected void MasterTable_DataBinding(object sender, EventArgs e)
        {

            string query = GetQueryText("table_level0");
            SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtLevel0);

            query = GetQueryText("table_level1");
            this.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtLevel1);

            tableDataSet.Tables.Add(dtLevel0);
            tableDataSet.Tables.Add(dtLevel1);

            tableDataSet.Relations.Add(dtLevel0.Columns[0], dtLevel1.Columns[1]);
            MasterTable.DataSource = tableDataSet.Tables[0].DefaultView;

        }

        protected void MasterTable_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            MasterTable.Bands[1].Columns[1].Hidden = true;
            MasterTable.Bands[0].Columns[0].CellStyle.Wrap = true;
            MasterTable.Bands[1].Columns[0].CellStyle.Wrap = true;

            MasterTable.Bands[0].Columns[0].Width = 250;
            MasterTable.Bands[1].Columns[0].Width = 250;

            


            for (int i = 1; i < MasterTable.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(MasterTable.Bands[0].Columns[i], "N2");                            
            }

            for (int i = 2; i < MasterTable.Bands[1].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(MasterTable.Bands[1].Columns[i], "N2");
            }

            
        }

        protected void chart_DataBinding(object sender, EventArgs e)
        {
            string query = GetQueryText("chart");
            SecondaryMASDataProvider.GetDataTableForChart(query, "series name", chartDT);
            chart.DataSource = chartDT;

            //настраиваем анотацию
            chart.Annotations.Visible = false;
            if (curStateAreaName != string.Empty)
            {
                int colnum = -1;
                for (int i = 0; i < chartDT.Columns.Count; i++)
                {
                    if (chartDT.Columns[i].Caption == curStateAreaName)
                    {
                        colnum = i;
                        break;
                    }
                }

                if (colnum != -1)
                {
                    chart.Annotations.Visible = true;
                    chart.Annotations[0].Location.Column = colnum;
                }
            }
        }

        protected void MasterTable_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            ShowHideChart(true);
            string areaTemplate = "[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]";
            string areaUName;
            if (e.Row.Band.Index == 1)
            {
                areaUName = string.Format(areaTemplate, e.Row.ParentRow.Cells[0].Text);
                curStateAreaName = e.Row.Cells[0].Text;
            }
            else
	        {
                areaUName = string.Format(areaTemplate, e.Row.Cells[0].Text);
                curStateAreaName = string.Empty;
	        }
            
            UserParams.StateArea.Value = areaUName;
            chart.DataBind();
        }
    }
}
