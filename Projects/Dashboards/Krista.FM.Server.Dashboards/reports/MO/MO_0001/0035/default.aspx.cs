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
 *  Труд и заработная плата  
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0005
{
    public partial class Default : CustomReportPage
    {

        private CustomParam SelectItemGrid;
        private CustomParam CurentPeriod;
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
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            marks = UserParams.CustomParam("marks");
            SelectItemGrid = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("SelectItemGrid"));
            CurentPeriod = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentPeriod"));
            CurentRegion = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentRegion"));
            norefresh = UserParams.CustomParam("r");
            norefresh2 = UserParams.CustomParam("r2");
            PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
            double Coef = 1;
            try
            {
                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 1.00; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 1; };
                G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth *Coef* double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForChartDynamics")));
            }
            catch { }

            if (BN == "FIREFOX")
            {
                G.Height = CRHelper.GetGridHeight(349);
                LC.Height = CRHelper.GetGridHeight(328);
            }
            else
            {
                G.Height = CRHelper.GetGridHeight(330);
                LC.Height = CRHelper.GetGridHeight(328);
            }

            
            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 0.96; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 0.99; };


            
           
            //LC.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 5);
            
            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 1.015; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1.015; };
            if (BN == "APPLEMAC-SAFARI")
            {
                LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.48 * Coef);
                CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.48 * Coef);
                C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.48 * Coef);
            }
            if (BN=="IE")
            {
                LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef);
                CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef - 3);
                C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef - 10);
            }
            if (BN == "FIREFOX")
            {
                LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef-16);
                CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef -16);
                C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 * Coef-16);
            }
            
            G.Width = C.Width;
            LC.Width = C.Width;
            Label2.Width = C.Width;
            Label7.Width = C.Width;
            PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
            
            PanelDynamicChart.AddLinkedRequestTrigger(G);
        }
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
 
                base.Page_Load(sender, e);
                string s;
              //  RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
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


                }
                CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + Year.SelectedValue + "]";

                s = RegionSettingsHelper.Instance.GetPropertyValue("titlePage");
                Hederglobal.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
                Page.Title = Hederglobal.Text;
                Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("PageSubTitle");
                
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid1_mark_"), true);
                G.DataBind();
                s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");
                Label1.Text = string.Format(s,G.Rows[0].Cells[2].Text.ToLower());

                    SelectItemGrid.Value = GetString_(G.Rows[0].Cells[0].Text);

                    G.Rows[0].Activated = 1 == 1;
                    G.Rows[0].Activate();
                    G.Rows[0].Selected = 1 == 1;
                


                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), G.Rows[0].Cells[2].Text.ToLower(), Year.SelectedValue, SelectItemGrid.Value);
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, "+G.Rows[0].Cells[2].Text.ToLower();
                Label7.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart2"), "рубль", Year.SelectedValue, SelectItemGrid.Value);
                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("grid1_mark_1");


                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("grid1_mark_1") + ".[" + G.Rows[0].Cells[0].Text + "]";
                C.DataBind();
                CC.DataBind();
                                
                LC.DataBind();
                LC.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ##0.##></b>, "+G.Rows[0].Cells[2].Text.ToLower();
                G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
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
            //try
            //{ gdt.Rows[0].Delete(); }
            //catch { }

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
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            
                
                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                    min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                try
                {
                    for (int i = 2; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                        if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    }
                }
                catch { }
                C.Axis.Y.RangeMax = max + 1;
                C.Axis.Y.RangeMin = min - 1;
                C.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;

            C.DataSource = dt;  
           // C.DataSource = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {

            if (norefresh2.Value == "Yes")
            {
                ArrayList itemGrid = ForMarks.Getmarks("grid1_mark_");

                int i = 0;

                for (i = 0; itemGrid.Count > i; i++)
                {
                    if (GetString_(e.Row.Cells[0].Text) == UserComboBox.getLastBlock(itemGrid[i].ToString()))
                    {
                        break;
                    }

                }


                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("grid1_mark_1") + ".[" + e.Row.Cells[0].Text+ "]";
                SelectItemGrid.Value = GridItems[e.Row.Index].ToString();
                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), e.Row.Cells[2].Text.ToLower(), Year.SelectedValue, SelectItemGrid.Value);
                Label7.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart2"), "рубль", Year.SelectedValue, SelectItemGrid.Value);
                C.DataBind();
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + e.Row.Cells[2].Text.ToLower();
                CC.DataBind();
                CC.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, рубль";
            }
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                Double Coef = 1;
                if (BN == "IE") { Coef = 0.970; }
                if (BN == "FIREFOX") { Coef = 0.95; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 0.955; };

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.368*Coef-12);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.088*Coef);
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ##0");
            }
            catch { }
            try
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForFirstColumnsGrid");
                e.Layout.Bands[0].Columns[1].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForSecondColumnsGrid");
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
            
                DataTable dt = new DataTable();
                for (int i = 0; i < gdt.Columns.Count; i++)
                {
                    dt.Columns.Add(gdt.Columns[i].ColumnName,gdt.Columns[i].DataType);
                }
                object[] o=new object[dt.Columns.Count];
                for (int i = 0; i < gdt.Rows.Count; i++)
                {
                    o[0] = "";
                    string s = gdt.Rows[i].ItemArray[0].ToString();
                    for (int j = 0; j < s.Length; j++)
                    {
                        o[0] = o[0].ToString() + s[j];
                        if ((j % 74 == 0)&&(j!=0))
                        {
                            o[0] += "<br>";
                        }
                    }
                    for (int j = 1; j < gdt.Columns.Count; j++)
                    {
                        o[j] = gdt.Rows[i].ItemArray[j];
                    }
                    dt.Rows.Add(o);
                }
                    LC.DataSource = dt;


                Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleStructChart");
          
        }
        protected void CC_DataBinding(object sender, EventArgs e)
        {
           // marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart3_prefix") + ".[" + UserComboBox.getLastBlock(marks.Value) + "]";
            marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark_static") + "," + RegionSettingsHelper.Instance.GetPropertyValue("chart3_prefix") + ".[" + UserComboBox.getLastBlock(marks.Value) + "]";
            //CC.DataSource = 
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));                        
            //
            try
            {
            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            //
           
     
            try
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        if (dt.Rows[j].ItemArray[i].ToString() != "")
                        {
                            if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                            if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                        }
                    }
                }
            }
            catch { }
            CC.Axis.Y.RangeMax = max + 1;
            CC.Axis.Y.RangeMin = min - 1;
            CC.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
        }
        catch { }
            CC.DataSource = dt;//GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));                        
            
        }
        protected void CR_DataBinding(object sender, EventArgs e)
        {
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart4_mark_"), true);            
        }

        protected void G2_ActiveRowChange(object sender, RowEventArgs e)
        {
            CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + e.Row.Cells[0].Text + "]";
            LC.DataBind();
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

        protected void CC_ChartDataClicked1(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void C_DataBound(object sender, EventArgs e)
        {

        }

        protected void C_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }
    }
}
