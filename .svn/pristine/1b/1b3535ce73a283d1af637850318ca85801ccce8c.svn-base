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
//using Krista.FM.Server.Dashboards.reports.MO.MO_0001;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

/**
 *  Показатели сферы образования.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0007
{
    public partial class Default : CustomReportPage
    {

        private CustomParam SelectItemGrid;
        private CustomParam CurentPeriod;
        private CustomParam LastPeriod;
        private CustomParam CurentRegion;
        private CustomParam marks;
        private CustomParam norefresh;
        private CustomParam norefresh2;

        Dictionary<string, int> ForParamRegion()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForSubregion"));
            d.Add(RegionSettingsHelper.Instance.GetPropertyValue("nameRegionLong").Split(']', '[')[RegionSettingsHelper.Instance.GetPropertyValue("nameRegionLong").Split(']', '[').Length - 2], 1);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i].ItemArray[0].ToString(), 1);
            }
// string.Format("{0},{1}","1212","23121");
//RegionSettingsHelper.Instance.GetPropertyValue("teg1")
            return d;
        }

        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            if (!string.IsNullOrEmpty(sql))
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));


                for (int i = cs.Axes[0].Positions.Count - 1; i >= 0; i--)
                {
                    d.Add(cs.Axes[0].Positions[i].Members[0].Caption, 0);
                }
            }
            return d;
        }

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }

        private string GetString_(string s)
        {
            try
            {
                string res = "";
                int i = 0;
                for (i = s.Length - 1; s[i] != ','; i--) ;
                for (int j = 0; j < i; j++)
                {
                    res += s[j];
                }
                return res;
            }
            catch { return s; }


        }
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            marks = UserParams.CustomParam("marks");
            norefresh = UserParams.CustomParam("r");
            norefresh2 = UserParams.CustomParam("r2");
            SelectItemGrid = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("SelectItemGrid"));//CurrentPeriod
            CurentPeriod = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentPeriod"));
            LastPeriod = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("LastPeriod"));
            CurentRegion = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentRegion"));
            try
            {
                G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForChartDynamics")));
                
                //UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForGrid")));
            }
            catch { }

            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49);
            LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49);
            CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 - 5);
            if (BN == "FIREFOX")
            {
                RC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.98);
                G.Height = CRHelper.GetGridWidth(309);
                C.Height = CRHelper.GetChartHeight(255);
            }
            if (BN=="IE")
            {
                RC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.99);
                G.Height = CRHelper.GetGridWidth(296);
                C.Height = CRHelper.GetChartHeight(300 - 34);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                RC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.98);
                G.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49)-10;
                G.Height = CRHelper.GetGridWidth(309);
                C.Height = CRHelper.GetChartHeight(272);
            }

            PanelDynamicChart.AddLinkedRequestTrigger(G);
            PopupInformer1.HelpPageUrl=RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
            LC.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chart3_tooltips");
            CC.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chart4_tooltips");

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            
                base.Page_Load(sender, e);
                string s;
            //    RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");
                CurentRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                if (!Page.IsPostBack)
                {



                    marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid_mark_"), true);
                    SelectItemGrid.Value = "children";
                    Year.FillDictionaryValues(GenDistonary(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForParamPeriod")));

                    s = RegionSettingsHelper.Instance.GetPropertyValue("ParamPeriodValueDefault");
                    if (!string.IsNullOrEmpty(s))
                    {
                        Year.SetСheckedState(s, true);
                    }
                    else
                    {

                    }
                    LastPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + Year.SelectedValue + "]";  
                }
                CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + Year.SelectedValue + "]";
               // LastPeriod.Value = "[Период].[Год Квартал Месяц].[Год].["+Year.GetLastNode(0)+"]";
                s = RegionSettingsHelper.Instance.GetPropertyValue("titlePage");
                Hederglobal.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
                SubTitle.Text = RegionSettingsHelper.Instance.GetPropertyValue("subTitlePage");
                Page.Title = Hederglobal.Text;
                s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");
                Label1.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);


                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid_mark_"), true);
                G.DataBind();
                    SelectItemGrid.Value = GetString_(G.Rows[0].Cells[0].Text);
                    G.Rows[0].Selected = 1 == 1;
                    G.Rows[0].Activated = 1 == 1;
                    G.Rows[0].Activate();

                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), SelectItemGrid.Value,G.Rows[0].Cells[2].Text.ToLower() );
                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("grid_mark_1");
                C.DataBind();
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, "+G.Rows[0].Cells[2].Text.ToLower();
                
                RC.DataBind();
                RC.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chart2_tooltips");
                LC.DataBind();
                CC.DataBind();
                G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                //UltraChart1.DataBind();
                //L.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleChart1");
                if (norefresh.Value != Year.SelectedValue)
                {
                    norefresh2.Value = "no";
                }
                else
                {
                    norefresh2.Value = "Yes";
                };

                norefresh.Value = Year.SelectedValue;
                RC.BackColor = Color.White;
  


        }
        ArrayList GridItems;
        protected void G_DataBinding(object sender, EventArgs e)
        {
          
                DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForGrid"));
                GridItems = new ArrayList();


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((string.IsNullOrEmpty(dt.Rows[i].ItemArray[1].ToString())))
                    {/* dt.Rows[i].Delete(); i--;*/ }
                    else
                    { GridItems.Add(dt.Rows[i].ItemArray[0].ToString()); }
                }


                G.DataSource = dt;
            
        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
           
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics")), "tg", dt);
                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                double min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                for (int i = 2; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    if (max < double.Parse(dt.Rows[0].ItemArray[i].ToString()))
                    {
                        max = double.Parse(dt.Rows[0].ItemArray[i].ToString());
                    }
                    if (min > double.Parse(dt.Rows[0].ItemArray[i].ToString()))
                    {
                        min = double.Parse(dt.Rows[0].ItemArray[i].ToString());
                    }
                
                }
                C.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                C.Axis.Y.RangeMax = max * 1.1;
                C.Axis.Y.RangeMin = min * 0.9;
                    C.DataSource = dt;//GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
                if (norefresh2.Value == "Yes")
                {
                    marks.Value = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark"), GetString_(e.Row.Cells[0].Text));
                    SelectItemGrid.Value = GridItems[e.Row.Index].ToString();

                    Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"),SelectItemGrid.Value,e.Row.Cells[2].Text.ToLower());
                    C.DataBind();
                    C.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " + e.Row.Cells[2].Text.ToLower();
                }
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                e.Row.Cells[0].Text += ", " + e.Row.Cells[2].Text.ToLower();//
            }
            catch { }
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double Coef = 1;
            try
            {
                
                if (BN == "IE") { Coef = 0.98; }
                if (BN == "FIREFOX") { Coef = 0.95; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 0.97; };
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForFirstColumnsGrid")));
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForSecondColumnsGrid")));

            }
            catch { }
            try
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForFirstColumnsGrid");
                e.Layout.Bands[0].Columns[1].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForSecondColumnsGrid");
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            }
            catch { }


            try
            {
                e.Layout.Bands[0].Columns[2].Hidden = 1 == 1;
            }
            catch { }
        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = RegionSettingsHelper.Instance.GetPropertyValue("text_EmptyChart");

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
            //C.Visible = false;
            //PanelDynamicChart.Visible = false;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
         //   marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart1_mark_"), true);
         //   UltraChart1.DataSource = GetDSForChart("chart1");
        }
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

        protected void LC_DataBinding(object sender, EventArgs e)
        {

           
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart2_mark_"), true);

                if (RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_1") == "!!!")
                { Panel1.Visible = false; WebAsyncRefreshPanel1.Visible = false; }
                else
                {
                    Panel1.Visible = true; L.Visible = 1 == 1; WebAsyncRefreshPanel1.Visible = true;
                    L.Visible = 1 == 1;
                    L.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleChart2");
                    DataTable dt = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_pie"), "dfg", dt);
                    DataTable resDt = new DataTable();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        resDt.Columns.Add(dt.Columns[i].ColumnName,dt.Columns[i].DataType);
                    }
                    object[] o=new object[resDt.Columns.Count];
                    o[0] = "Выпускники 9-х классов";
                    for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        o[i] = dt.Rows[0].ItemArray[i];
                    }
                    resDt.Rows.Add(o);
                    o[0] = "Выпускники 11-х классов";
                    for (int i = 1; i < dt.Rows[1].ItemArray.Length; i++)
                    {
                        o[i] = dt.Rows[1].ItemArray[i];
                    }
                    resDt.Rows.Add(o);
                    LC.DataSource = resDt;
                }
        }
        protected void CC_DataBinding(object sender, EventArgs e)
        {
            
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart3_mark_"), true);

                if (RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark_1") == "!!!")
                { Panel2.Visible = false; WebAsyncRefreshPanel2.Visible = false; }
                else
                {
                    Panel2.Visible = true;
                    WebAsyncRefreshPanel2.Visible = true;
                    Label7.Visible = 1 == 1;
                    CC.DataSource = GetDSForChart("chart_pie");
                    Label7.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleChart3");
                }
        
        }
        protected void CR_DataBinding(object sender, EventArgs e)
        {
            
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart4_mark_"), true);
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_pie3"), "tg", dt);
               /* double max = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                double min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 1; j < dt.Rows[i].ItemArray.Length; j++)
                    {
                        if (max < double.Parse(dt.Rows[i].ItemArray[j].ToString()))
                        {
                            max = double.Parse(dt.Rows[i].ItemArray[j].ToString());
                        }
                        if (min > double.Parse(dt.Rows[i].ItemArray[j].ToString()))
                        {
                            min = double.Parse(dt.Rows[i].ItemArray[j].ToString());
                        }
                    }

                }
                RC.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                RC.Axis.Y.RangeMax = max+100;
                RC.Axis.Y.RangeMin = min-100;
                RC.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.DataInterval;
                RC.Axis.Y.TickmarkInterval = (double)((RC.Axis.Y.RangeMax - RC.Axis.Y.RangeMin) / 10);*/
                RC.DataSource = dt;//GetDSForChart("chart_pie3");
            
        }
    }

}

