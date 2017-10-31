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


using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0001._0230
{
	public partial class default1 : CustomReportPage
	{
        private string page_title = "Информационная служба, сводная таблица";
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
		protected override void Page_Load(object sender, EventArgs e)
		{
            base.Page_Load(sender,e);
            RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            Lastdate.Value= ELV(getLastDate("ОРГАНИЗАЦИЯ И СОДЕРЖАНИЕ МУНИЦИПАЛЬНОЙ ИНФОРМАЦИОННОЙ СЛУЖБЫ"));
            Label1.Text = page_title;
            Grid.DataBind();
            Grid.Width = 1210;
            Grid.DataBind();
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.Height = Grid.Rows.Count * 25;
		}
        private String getLastDate(String way_ly)
        {
            try
            {
                way_last_year.Value = way_ly;
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        private String ELV(String s)
        {
            int i = s.Length;
            string res = "";
            while (s[--i] != ']') ;
            while (s[--i] != '[')
            {
                res = s[i] + res;
            }
            return res;

        }
        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.9);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[1].Hidden = 1 == 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.44);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "###,##0.##");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].Text == "-")
            {
                e.Row.Delete();
            }
            else
            {
                if (e.Row.Cells[1].Text != "-")
                {
                    e.Row.Cells[0].Text = e.Row.Cells[0].Text + "," + e.Row.Cells[1].Text.ToLower();
                }
            }
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("GridSvod"), " ", dt);
            Grid.DataSource = dt;
        }
	}
}
