using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
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
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001._00421
{
    public partial class _default : CustomReportPage
    {
        private CustomParam last_date;
        private CustomParam current_region;
        private CustomParam selected_date;//год выбранный в гриде
        private CustomParam marks;
        private CustomParam chart1_mark;
        private CustomParam chart2_mark;
        private CustomParam chart3_mark;
        private CellSet CS;
        private CellSet CS1;
        private CellSet CS2;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            last_date = UserParams.CustomParam("last_date");
            current_region = UserParams.CustomParam("current_region");
            selected_date = UserParams.CustomParam("selected_date");
            marks = UserParams.CustomParam("marks");
            chart1_mark = UserParams.CustomParam("chart1_mark");
            chart2_mark = UserParams.CustomParam("chart2_mark");
            chart3_mark = UserParams.CustomParam("chart3_mark");
            Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("titlechart31");
            region.Title = "Территория РФ";
            ComboYear.Title = "Год";
            refreshPanel.AddLinkedRequestTrigger(ComboYear);
            refreshPanel.AddRefreshTarget(Label1);
            refreshPanel.AddRefreshTarget(Chart1);
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
               // RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                marks = SetMarks(marks, Getmarks("grid_mark_"), true);
                last_date.Value = UserComboBox.getLastBlock(getLastDate());
                if (!Page.IsPostBack)
                {
                    region.FillDictionaryValues(RegionsLoad("Regions"));
                    ComboYear.FillDictionaryValues(YearsLoad("years"));
                    ComboYear.SetСheckedState(last_date.Value, true);
                }

                selected_date.Value = ComboYear.SelectedValue;
                current_region.Value = getRegion();
                chart1_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart_mark_1");
                chart2_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart_mark_2");
                chart3_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart_mark_3");
                Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("titlechart21");
                Label3.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("pagetitle"), selected_date.Value, UserComboBox.getLastBlock(current_region.Value));
                Page.Title = Label3.Text;
                Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("titlechart11");
                Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("titlechart31");
                Chart2.DataBind();
                Chart1.DataBind();
                Chart3.DataBind();
                Label5.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titlechart2"), CS1.Axes[1].Positions[0].Members[0].MemberProperties[0].Value.ToString().ToLower());
                Chart1.Tooltips.FormatString = String.Format("<DATA_VALUE:00.##>, {0}", CS1.Axes[1].Positions[0].Members[0].MemberProperties[0].Value.ToString().ToLower());
                Label6.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titlechart1"),"%");
                Chart2.Tooltips.FormatString = String.Format("<DATA_VALUE:00.##>, {0}", CS1.Axes[1].Positions[0].Members[0].MemberProperties[0].Value.ToString().ToLower());
                Label7.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titlechart3"), CS2.Axes[1].Positions[0].Members[0].MemberProperties[0].Value.ToString().ToLower());
                Chart3.Tooltips.FormatString = String.Format("<DATA_VALUE:00.##>, {0}", CS2.Axes[1].Positions[0].Members[0].MemberProperties[0].Value.ToString().ToLower());
                
            }
            catch { }
        }
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

        protected String getLastDate()
        {
            try
            {
                CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return CS.Axes[1].Positions[0].Members[0].ToString();
            }
            catch
            {
                return null;
            }
        
        }
        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            if (cs.Axes[1].Positions.Count <= 1)
            {
                region.Visible = false;
            }
            else
            {
                region.Width = int.Parse(RegionSettingsHelper.Instance.GetPropertyValue("regionCombowidth"));
            }
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }

            return d;

        }
        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            /*if (cs.Axes[1].Positions.Count <= 1)
            {
                region.Visible = false;
                RefreshButton.Visible = false;
            }*/
            for (int i = (cs.Axes[1].Positions.Count - 1); i >=0; i--)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }

            return d;

        }
        public String getRegion()
        {
            if (region.Visible == true)
            {
                CellSet regionCellSet = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("Regions"));
                int k = region.SelectedIndex;
                return regionCellSet.Axes[1].Positions[k].Members[0].UniqueName;

            }
            else
            {
                return RegionSettingsHelper.Instance.RegionBaseDimension;
            }
        }
        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master = new DataTable();
                
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master);
                CS1 = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart2"));
                
                Chart2.DataSource = chart_master.DefaultView;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "_", chart_master);
                Chart1.DataSource = chart_master.DefaultView;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "_", chart_master);
                CS2 = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart3"));
                Chart3.DataSource = chart_master.DefaultView;
               // Chart3.TextRenderingHint=System.Drawing.Text.TextRenderingHint.;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

    }

}
