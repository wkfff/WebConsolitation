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
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0001._029
{
    public partial class _default : CustomReportPage
    {
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "()-_-()");
            return dt;
        }

        private CustomParam chartMarks;
        private CustomParam gridMarks;

        static public class ForMarks
        {
            public static ArrayList Getmarks(string prefix)
                {
                ArrayList AL = new ArrayList();
                string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + "1");
                int i = 2;
                while (!string.IsNullOrEmpty(CurMarks))
                {
                    AL.Add(CurMarks.ToString());
                    CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());
                    i++;
                }
                return AL;
            }
            public static CustomParam SetMarks(CustomParam param, ArrayList AL, params bool[] clearParam)
            {
                if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
                int i;
                for (i = 0; i < AL.Count - 1; i++)
                {
                    param.Value += AL[i].ToString() + ",";
                }
                param.Value += AL[i].ToString();
                return param;
            }
        }

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            chartMarks = UserParams.CustomParam("mark");
            gridMarks = UserParams.CustomParam("G_Marks");
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);         
        }

        string realRegion = "Gubkinski";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            RegionSettingsHelper.Instance.SetWorkingRegion(realRegion);
            
            G.DataBind();

            //try{
                C1.DataBind(); 
                C2.DataBind();
                C3.DataBind();
                C4.DataBind();
                C5.DataBind();
                C6.DataBind();
                C7.DataBind();
                C8.DataBind();
            //}catch { }
        }

        Dictionary<string, int> param_3;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            ForMarks.SetMarks(gridMarks, ForMarks.Getmarks("grid1_mark_"), 1 == 1);
            G.DataSource = GetDSForChart("G");
        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            UltraChart C = (UltraChart)(sender);
            C.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PieChart;
            C.PieChart.OthersCategoryPercent = 0;
            C.PieChart.OthersCategoryText = "Прочие";

            //C.Legend.Visible = 2*2 == 4;
            //C.Legend.SpanPercentage = 20;
            //C.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                

            //C.Tooltips.FormatString = "<ITEM_LABEL>, <DATA_VALUE:00.##>";
            

            chartMarks.Value = RegionSettingsHelper.Instance.GetRegionSetting("grid1_mark_" + C.ID[1]);
            C.DataSource = GetDSForChart("C12345678");
        }

        protected void CC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 0;i<e.Layout.Bands[0].Columns.Count;i++)            
            {
                e.Layout.Bands[0].Columns[i].Width = 200;
            }
        }
    }

}
