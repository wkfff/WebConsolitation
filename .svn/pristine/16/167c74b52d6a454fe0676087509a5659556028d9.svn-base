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

using Infragistics.WebUI.UltraWebChart;

using Infragistics.UltraChart.Core.Primitives;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001._0043
{
    public partial class _default : CustomReportPage
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
            SelectItemGrid = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("SelectItemGrid"));//CurrentPeriod
            CurentPeriod = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentPeriod"));
            CurentRegion = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentRegion"));
            norefresh = UserParams.CustomParam("r");
            norefresh2 = UserParams.CustomParam("r2");
            try
            {
                G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth *1.01* double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForChartDynamics")));

                //UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForGrid")));
            }
            catch { }

            double Coef = 1;
            try
            {
                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 1; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 1; };

                G.Height = 250;
                C.Height = CRHelper.GetChartHeight(278);

                G2.Height = 300;
                LC.Height = CRHelper.GetChartHeight(311);
                CC.Height = 300;

                if (BN == "IE") { Coef = 1.008; }
                if (BN == "FIREFOX") { Coef = 1.005; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 1.01; };

                LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 15);
                CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 15);

                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 1; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 1; };

                G2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.505);
                C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 18);

                UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 15);
                UltraChart1.Height = 300;

                //RC.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 1);

                PanelDynamicChart.AddLinkedRequestTrigger(G);
                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G2);
            }
            catch { }

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
//#Page_PreLoad(sender, e);
                
                base.Page_Load(sender, e);



                string s;
                //RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
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
                Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("pageSubTitle");
                Page.Title = Hederglobal.Text;
                s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");
                Label1.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);

                Label8.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleGrid2"), UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);

                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid1_mark_"), true);
                G.DataBind();

                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid2_mark_"), true);
                G2.DataBind();
                    SelectItemGrid.Value = GetString_(G.Rows[0].Cells[0].Text);

                    G.Rows[0].Selected = 1 == 1;
                    G.Rows[0].Activate();
                    G.Rows[0].Activated = 1 == 1;

                    G2.Rows[0].Selected = 1 == 1;
                    G2.Rows[0].Activate();
                    G2.Rows[0].Activated = 1 == 1;

                


                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue, SelectItemGrid.Value);
                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("grid1_mark_1");
                C.DataBind();
                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), G.Rows[0].Cells[0].Text.Split(',')[0], G.Rows[0].Cells[2].Text.ToLower());
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, "+G.Rows[0].Cells[2].Text.ToLower();
                //CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + G2.Rows[G2.Rows.Count - 1].Cells[0].Text + "]";
                SelectItemGrid.Value = GetString_(G2.Rows[0].Cells[0].Text);
                marks.Value = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark_1"),SelectItemGrid.Value.Split(',')[0]);
                
                LC.DataBind();
                LC.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, "+G2.Rows[0].Cells[2].Text.ToLower();
                CC.DataBind();
                CC.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ### ##0.##></b>, "+G.Rows[0].Cells[2].Text.ToLower();
                Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart2_title"), G2.Rows[0].Cells[0].Text.Split(',')[0], G2.Rows[0].Cells[2].Text.ToLower());
                UltraChart1.DataBind();
                try
                {
                    Label7.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("CenterTitleChart1"), UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
                    Label10.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("CenterTitleChart2"), UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
                    G.Rows[0].Cells[0].Style.Font.Bold = 1 == 1;
                    G.Rows[0].Cells[1].Style.Font.Bold = 1 == 1;
                }
                catch { }
                UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>,"+Label10.Text.Split(',')[1];

                G2.DisplayLayout.AllowSortingDefault = AllowSorting.No;
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
                
                //if (!Page.IsPostBack) { Page.Response.Redirect("default.aspx"); }
                //UltraChart1.DataBind();
                //Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleChart1");
            }
            catch { }


        }

        ArrayList GridItems;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForGrid"));
                GridItems = new ArrayList();
                //try
                //{ dt.Rows[0].Delete(); }
                //catch { }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((string.IsNullOrEmpty(dt.Rows[i].ItemArray[1].ToString())))
                    { dt.Rows[i].Delete(); i--; }
                    else
                    { GridItems.Add(dt.Rows[i].ItemArray[0].ToString()); }
                }


                G.DataSource = dt;
            }
            catch
            { }
        }

        ArrayList GridItems2;
        protected void G2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForGrid"));
                GridItems2 = new ArrayList();
                //try
                //{ dt.Rows[0].Delete(); }
                //catch { }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((string.IsNullOrEmpty(dt.Rows[i].ItemArray[1].ToString())))
                    { dt.Rows[i].Delete(); i--; }
                    else
                    { GridItems2.Add(dt.Rows[i].ItemArray[0].ToString()); }
                }


                G2.DataSource = dt;
            }
            catch { }
        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
            try
            {
                C.DataSource = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            }
            catch { }
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
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


                    marks.Value = itemGrid[i].ToString();
                    SelectItemGrid.Value = GridItems[e.Row.Index].ToString();
                    if (e.Row.Index == 0)
                    {
                        Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), e.Row.Cells[0].Text.Split(',')[0], e.Row.Cells[2].Text.ToLower());
                    }
                    else
                    {
                        Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart1"), e.Row.Cells[0].Text.Split(',')[0], e.Row.Cells[2].Text.ToLower());
                    }
                    C.DataBind();
                    C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + e.Row.Cells[2].Text.ToLower();
                }
            }
            catch { }
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
            try
            {
                double Coef = 1;
                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 0.98; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 0.99; };

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForFirstColumnsGrid")));
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForSecondColumnsGrid")));

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

        int lcd = 2008;
        protected void LC_DataBinding(object sender, EventArgs e)
        {



            try
            {
                DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));// C.DataSource = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                try
                {
                    for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                        if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    }
                }
                catch { }

                
                LC.Axis.Y.RangeType = AxisRangeType.Custom;
                LC.Axis.Y.RangeMax = max + 2;
                LC.Axis.Y.RangeMin = min - 2;
                LC.Axis.Y.TickmarkStyle = AxisTickStyle.DataInterval;
                LC.Axis.Y.TickmarkInterval = (double)((LC.Axis.Y.RangeMax - LC.Axis.Y.RangeMax) / 10);
                LC.DataSource = dt;
                lcd = int.Parse(dt.Columns[dt.Columns.Count - 1].Caption);
            }
            catch { }
            //Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart2_title"), );
        }
        protected void CC_DataBinding(object sender, EventArgs e)
        {
            try
            {
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart3_mark_"), true);
            }
            catch { }
            //if (string.IsNullOrEmpty(RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark_"))) { Panel2.Visible = false; WebAsyncRefreshPanel2.Visible = false; } else { Panel2.Visible = true; WebAsyncRefreshPanel2.Visible = true; }
            CC.DataSource = GetDSForChart("chart_pie");
            Label7.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart3_title"), CurentPeriod.Value.Split('[', ']')[CurentPeriod.Value.Split('[', ']').Length - 2]);
        }
        protected void CR_DataBinding(object sender, EventArgs e)
        {
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart4_mark_"), true);
            //RC.DataSource = GetDSForChart("chart_pie3");
            //Label9.Text = RegionSettingsHelper.Instance.GetPropertyValue("Chart4_title");
        }

        protected void G2_ActiveRowChange(object sender, RowEventArgs e)
        {
           // CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + e.Row.Cells[0].Text + "]";
            try
            {
                if (norefresh2.Value == "Yes")
                {
                    ArrayList itemGrid = ForMarks.Getmarks("grid2_mark_");

                    int i = 0;

                    for (i = 0; itemGrid.Count > i; i++)
                    {
                        if (GetString_(e.Row.Cells[0].Text) == UserComboBox.getLastBlock(itemGrid[i].ToString()))
                        {
                            break;
                        }

                    }



                    marks.Value = itemGrid[i].ToString();
                    SelectItemGrid.Value = UserComboBox.getLastBlock(itemGrid[i].ToString());//GridItems[e.Row.Index].ToString();

                    //Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue, SelectItemGrid.Value);
                    LC.DataBind();
                    CC.DataBind();
                    Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart2_title"), e.Row.Cells[0].Text.Split(',')[0], e.Row.Cells[2].Text.ToLower());
                    LC.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + e.Row.Cells[2].Text.ToLower();
                }
            }
            catch { }
//Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue(
        }

        protected void CC_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void G2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double Coef = 1;
            if (BN == "IE") { Coef = 0.985; }
            if (BN == "FIREFOX") { Coef = 1; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1.01; };

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.39 * Coef);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.1 * Coef);
            //e.Layout.Bands[0].Columns[2].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.1 * Coef);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[0].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForFirstColumnsGrid");
            e.Layout.Bands[0].Columns[1].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForSecondColumnsGrid");
            e.Layout.Bands[0].Columns[2].Hidden = 1 == 1;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;


            }

        }
        int UC1;
        protected void UltraChart1_DataBinding1(object sender, EventArgs e)
        {
            try
            {
                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart4_mark_1");
           
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
    min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            try
            {
                for (int i = 1; i < dt.Rows[0].ItemArray.Length - 1; i++)
                {
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                }
            }
            catch { }
            UltraChart1.Axis.Y.RangeMax = max + 2;
            UltraChart1.Axis.Y.RangeMin = min - 2;
            UltraChart1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            //C.DataSource = dt;

            
            UltraChart1.DataSource = dt;// UltraChart1.DataSource;
            UC1 = int.Parse(dt.Columns[dt.Columns.Count - 1].Caption);

            Label7.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart3_title"), CurentPeriod.Value.Split('[', ']')[CurentPeriod.Value.Split('[', ']').Length - 2]);
        }
        catch { }
        }

        protected void chart_avg_count_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                int xOct = 0;
                int xNov = 0;
                Text decText = null;
                int year = UC1;
                String year1 = (year - 1).ToString();
                String year2 = (year - 2).ToString();

                foreach (Primitive primitive in e.SceneGraph)
                {
                    if (primitive is Text)
                    {
                        Text text = primitive as Text;

                        if (year2 == text.GetTextString())
                        {
                            xOct = text.bounds.X;
                            continue;
                        }
                        if (year1 == text.GetTextString())
                        {
                            xNov = text.bounds.X;
                            decText = new Text();
                            decText.bounds = text.bounds;
                            decText.labelStyle = text.labelStyle;
                            continue;
                        }
                    }
                    if (decText != null)
                    {
                        decText.bounds.X = xNov + (xNov - xOct);
                        decText.SetTextString(year.ToString());
                        e.SceneGraph.Add(decText);
                        break;
                    }
                }
            }
            catch { }
        }

        protected void LC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                int xOct = 0;
                int xNov = 0;
                Text decText = null;
                int year = lcd;
                String year1 = (year - 1).ToString();
                String year2 = (year - 2).ToString();

                foreach (Primitive primitive in e.SceneGraph)
                {
                    if (primitive is Text)
                    {
                        Text text = primitive as Text;

                        if (year2 == text.GetTextString())
                        {
                            xOct = text.bounds.X;
                            continue;
                        }
                        if (year1 == text.GetTextString())
                        {
                            xNov = text.bounds.X;
                            decText = new Text();
                            decText.bounds = text.bounds;
                            decText.labelStyle = text.labelStyle;
                            continue;
                        }
                    }
                    if (decText != null)
                    {
                        decText.bounds.X = xNov + (xNov - xOct);
                        decText.SetTextString(year.ToString());
                        e.SceneGraph.Add(decText);
                        break;
                    }
                }
            }
            catch { }
        }
    }
}
