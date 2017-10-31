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
using System.Drawing;
using Infragistics.UltraGauge.Resources;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.UltraChart.Core.Primitives;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using System.IO;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.Misc;
using Infragistics.Documents.Excel;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using Dundas.Maps.WebControl;
using System.Drawing.Imaging;


namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0001._0130
{
	public partial class default1 : CustomReportPage
	{
        string page_title = "Организация, содержание и развитие учреждений здравоохранения, сводный отчет";
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        // параметр для выбранного/текущего года
		protected override void Page_Load(object sender, EventArgs e)
		{
            base.Page_Load(sender,e);
            RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            last_year.Value = getLastDate("ОРГАНИЗАЦИЯ, СОДЕРЖАНИЕ И РАЗВИТИЕ УЧРЕЖДЕНИЙ ЗДРАВООХРАНЕНИЯ");
            Label1.Text = page_title;
            Grid.DataBind();
            Grid.Width = 1210;
            Grid.Height = Grid.Rows.Count * 25;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
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
                //return null;
            }
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("GridAll"), " ", dt);
            Grid.DataSource = dt;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 1.5);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[1].Hidden = 1 == 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.35);
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
	}
    
}
