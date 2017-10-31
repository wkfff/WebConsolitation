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
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

/**
 *  Труд и заработная плата  
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0004_00
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
            SelectItemGrid = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("SelectItemGrid"));
            CurentPeriod = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentPeriod"));
            LastPeriod = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("LastPeriod"));
            CurentRegion = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentRegion"));
            try
            {
                G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForChartDynamics")));


            }
            catch { }
            

            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 0.99; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 0.99; };

            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef - 5);
            CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef - 5);
            G2.Width = G.Width;
            Label2.Width = C.Width;
            Label10.Width = Label2.Width;
            if ((BN == "IE")||(BN=="APPLEMAC-SAFARI"))
            {
                C.Height = CRHelper.GetChartHeight(304);
                G.Height = CRHelper.GetChartHeight(307);

                CC.Height = CRHelper.GetChartHeight(300);
                G2.Height = CRHelper.GetChartHeight(319);
            }
            if (BN=="FIREFOX")
            {  
                C.Height = CRHelper.GetChartHeight(304);
                G.Height = CRHelper.GetChartHeight(327);

                CC.Height = CRHelper.GetChartHeight(300);
                G2.Height = CRHelper.GetChartHeight(340);
            }

            PanelDynamicChart.AddLinkedRequestTrigger(G);
            WebAsyncRefreshPanel2.AddLinkedRequestTrigger(G2);
            norefresh = UserParams.CustomParam("r");
            norefresh2 = UserParams.CustomParam("r2");
        }
        double Coef = 1;
        protected override void Page_Load(object sender, EventArgs e)
        {
           
                base.Page_Load(sender, e);
                string s;
               // RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");
                CurentRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                if (!Page.IsPostBack)
                {



                    marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid1_mark_"), true);
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

                s = RegionSettingsHelper.Instance.GetPropertyValue("titlePage");
                Hederglobal.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
                Page.Title = Hederglobal.Text;
                Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("subTitlePage");
                s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");
                Label1.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);

                s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid2");
                Label8.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);


                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid1_mark_"), true);
                G.DataBind();
                SelectItemGrid.Value = GetString_(G.Rows[0].Cells[0].Text);
                G.Rows[0].Activated = 1 == 1;
                G.Rows[0].Activate();
                G.Rows[0].Selected = 1 == 1;

                string title = G.Rows[0].Cells[0].Text.Split(',')[0];
                for (int j = 1; j < G.Rows[0].Cells[0].Text.Split(',').Length - 1; j++)
                {
                    title = title + "," + G.Rows[0].Cells[0].Text.Split(',')[j];
                }

                marks.Value = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark_1"), title);
                C.DataBind();    


                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), G.Rows[0].Cells[0].Text.Split(',')[0],G.Rows[0].Cells[2].Text.ToLower());                
                G2.DataBind();
                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("grid2_mark_1");

                G2.Rows[0].Activated = 1 == 1;
                G2.Rows[0].Activate();
                G2.Rows[0].Selected = 1 == 1;
                marks.Value = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_1"), G2.Rows[0].Cells[0].Text.Split(',')[0]);
                if (RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_2") != "")
                {
                    marks.Value = marks.Value + "," + String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_2"), GetEqualValue(G2.Rows[0].Cells[0].Text.Split(',')[0]));
                }
                else
                {

                }
                CC.DataBind();
                title = G2.Rows[0].Cells[0].Text.Split(',')[0];
                for (int i=1;i<G2.Rows[0].Cells[0].Text.Split(',').Length-1;i++)
                {
                    title = title + ","+G2.Rows[0].Cells[0].Text.Split(',')[i];
                }
                Label10.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart2"), title, G2.Rows[0].Cells[2].Text.ToLower());
                G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                G2.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " + G.Rows[0].Cells[2].Text.ToLower(); 
                CC.Tooltips.FormatString = "<b><DATA_VALUE:### ##0></b>, " + G2.Rows[0].Cells[2].Text.ToLower();
                if (norefresh.Value != Year.SelectedValue)
                {
                    norefresh2.Value = "no";
                }
                else
                {
                    norefresh2.Value = "Yes";
                };
                
                norefresh.Value = Year.SelectedValue;
           


        }
        DataTable gdt;
        ArrayList GridItems;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            gdt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForGrid"));
            GridItems = new ArrayList();            
            for (int i = 0; i < gdt.Rows.Count; i++)
            {
                if ((string.IsNullOrEmpty(gdt.Rows[i].ItemArray[1].ToString())))
                { gdt.Rows[i].Delete(); i--; }
                else
                { GridItems.Add(gdt.Rows[i].ItemArray[0].ToString()); }
            }
            G.DataSource = gdt;
        }
        protected void C_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            try
            {
                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                }
                C.Axis.X.RangeMax = max + 20;
                C.Axis.X.RangeMin = min - 20;
                C.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                C.Axis.X.TickmarkStyle = AxisTickStyle.DataInterval;
                C.Axis.X.TickmarkInterval = (int)C.Axis.Y.RangeMax / 5;
                C.Axis.X.Extent = 25;
                C.Axis.Y.Extent = 90;
            }
            catch { C.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Automatic; }
            C.Transform3D.Scale = 77;
            C.Transform3D.Perspective = 70;
            C.DataSource = dt;
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {

            if (norefresh2.Value == "Yes")
            {
                ArrayList itemGrid = ForMarks.Getmarks("grid1_mark_");
                string title = e.Row.Cells[0].Text.Split(',')[0];
                for (int j = 1; j < e.Row.Cells[0].Text.Split(',').Length - 1; j++)
                {
                    title = title +","+e.Row.Cells[0].Text.Split(',')[j];
                }

                marks.Value =String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark_1"),title);
                SelectItemGrid.Value = GridItems[e.Row.Index].ToString();
                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), e.Row.Cells[0].Text.Split(',')[0], e.Row.Cells[2].Text.ToLower());
                C.DataBind();
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " +e.Row.Cells[2].Text.ToLower();
                C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef - 50);

                
            }
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            
                e.Row.Cells[0].Text += ", " + e.Row.Cells[2].Text.ToLower();//
          
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                if (BN == "IE") { Coef = 0.995; }
                if (BN == "FIREFOX") { Coef = 0.95; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 0.97; };

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.375*Coef);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.08 * Coef);
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            }
            catch { }
            try
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForFirstColumnsGrid");
                e.Layout.Bands[0].Columns[1].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForSecondColumnsGrid");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ##0.00");
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
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {

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
       

            try
            {
            }
            catch { }
         
        }
        protected void CC_DataBinding(object sender, EventArgs e)
        {            
            
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));

           
               /* double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                for (int i = 2; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                }
                CC.Axis.Y.RangeMax = max * 1.1;
                CC.Axis.Y.RangeMin = min * 0.9;
                CC.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;*/
            

            CC.DataSource = dt;
//Label10.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart2"), UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue, UserComboBox.getLastBlock(marks.Value));
        }
        protected void CR_DataBinding(object sender, EventArgs e)
        {
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart2_mark_"), true);
        }

        protected void G2_ActiveRowChange(object sender, RowEventArgs e)
        {
            CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + e.Row.Cells[0].Text + "]";            
            CC.DataBind();
        }

        protected void CC_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void G2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
            }
            e.Layout.Bands[0].Columns[0].Header.Caption = "Год";
        }

        ArrayList GridItems2;
        protected void G2_DataBinding(object sender, EventArgs e)
        {
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid2_mark_"), 1==1);
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForGrid2"));
            G2.DataSource = dt;
        }
        string GetEqualValue(string str)
        { 
        switch (str)
            {
                case "Магазины": { return "Норматив по магазинам"; break; }
                case "Хранилища": { return "Норматив по хранилищам"; break; }
                case "Холодильники": { return "Норматив по холодильникам"; break; }
                case "Склады": { return "Норматив по складам"; break; }
                case "Предприятия общепита": { return "Норматив по предприятиям общепита"; break; }
                default: return " ";
            }
        }
        protected void G2_ActiveRowChange1(object sender, RowEventArgs e)
        {            
            if (norefresh2.Value == "Yes")
            {
                ArrayList itemGrid = ForMarks.Getmarks("grid2_mark_");

                int i = 0;

                string title = e.Row.Cells[0].Text.Split(',')[0];
                for (int j = 1; j < e.Row.Cells[0].Text.Split(',').Length - 1; j++)
                {
                    title = title +","+e.Row.Cells[0].Text.Split(',')[j];
                }
                    marks.Value = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_1"), title);
                    if (RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_2") != "")
                    {
                        marks.Value =marks.Value+","+ String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_2"), GetEqualValue(title));
                    }
                    else 
                    { 
                    
                    }
                    CC.DataBind();
                    
                    Label10.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart2"),title , e.Row.Cells[2].Text.ToLower());
                    try
                    {
                        CC.Tooltips.FormatString = "<b><DATA_VALUE:### ##0></b>, " + G2.Rows[0].Cells[2].Text.ToLower();
                    }
                    catch { }
                

            }
        }
    }

}
