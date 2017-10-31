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

namespace Krista.FM.Server.Dashboards.reports.PMO_0001_00060
{
    public partial class Default : CustomReportPage
    {

        private CustomParam SelectItemGrid;
        private CustomParam CurentPeriod;
        private CustomParam SelectedPeriod;
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
            SelectedPeriod = UserParams.CustomParam("SelectedPeriod"); ;
            CurentRegion = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentRegion"));
            PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
            double Coef = 1;
            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 1; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1; };
            if (BN == "FIREFOX")
            {
                G.Height = int.Parse(C.Height.Value.ToString()) + 29;
            }
            else
            {
                if (BN == "IE")
                {
                    
                    G.Height =int.Parse(C.Height.Value.ToString())+8;
                }
                else {
                    G.Height = int.Parse(C.Height.Value.ToString()) + 8;
                }
            }
            G.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 8);
            LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 15);
            CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 15);
            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 15);
            Label2.Width = Label5.Width = Label7.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 15);
            Label2.Width = C.Width;
            Label5.Width = LC.Width;
            Label7.Width = CC.Width;
            PanelDynamicChart.AddLinkedRequestTrigger(G);
            G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            norefresh = UserParams.CustomParam("r");
            norefresh2 = UserParams.CustomParam("r2");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
          
                base.Page_Load(sender, e);
                string s;
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
                s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");
                Label1.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid1_mark_"), true);
                G.DataBind();
                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), G.Rows[0].Cells[0].Text.Split(',')[0], G.Rows[0].Cells[2].Text.ToLower());
                marks.Value = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark"), G.Rows[0].Cells[0].Text.Remove(G.Rows[0].Cells[0].Text.LastIndexOf(',')));
                C.DataBind();
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + G.Rows[0].Cells[2].Text.ToLower();
                SelectedPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + Year.SelectedValue + "]";
                SelectItemGrid.Value = GetString_(G.Rows[0].Cells[0].Text);
                G.Rows[0].Selected = 1 == 1;
                G.Rows[0].Activate();
                G.Rows[0].Activated = 1 == 1;
                LC.DataBind();
                CC.DataBind();
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
        ArrayList GridItems;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForGrid"));
            GridItems = new ArrayList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((string.IsNullOrEmpty(dt.Rows[i].ItemArray[1].ToString())))
                { dt.Rows[i].Delete(); i--; }
                else
                { GridItems.Add(dt.Rows[i].ItemArray[0].ToString()); }
            }
            G.DataSource = dt;
        }

        ArrayList GridItems2;
        protected void C_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics")), "dfd", dt);
            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            double min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
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
            C.Axis.Y.RangeType = AxisRangeType.Custom;
            C.Axis.Y.RangeMax = max*1.1;
            C.Axis.Y.RangeMin = min*0.9;
            C.Axis.Y.TickmarkInterval = (double)(C.Axis.Y.RangeMax / 5);
            C.DataSource = dt;
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
                marks.Value = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark"),e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(',')));
                SelectItemGrid.Value = GridItems[e.Row.Index].ToString();
                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), e.Row.Cells[0].Text.Split(',')[0], e.Row.Cells[2].Text.ToLower());
                C.DataBind();
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + e.Row.Cells[2].Text.ToLower();
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
            try
            {
                double Coef = 1;
                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 0.97; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 0.98; };

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForFirstColumnsGrid")));
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForSecondColumnsGrid")));
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
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
        DataTable LCdt;
        protected void LC_DataBinding(object sender, EventArgs e)
        {
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart2_mark_"), true);
            LCdt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_pie"), "dd", LCdt);
            LCdt.Columns.Remove(LCdt.Columns[2]);
            LC.DataSource = LCdt;
            Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart2_title"), SelectedPeriod.Value.Split('[', ']')[SelectedPeriod.Value.Split('[', ']').Length - 2]);
        }
        DataTable CCdt;
        protected void CC_DataBinding(object sender, EventArgs e)
        {
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart3_mark_"), true);
            CCdt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_pie"), "dfd", CCdt);
            DataTable resDt = new DataTable();
            for (int i = 0; i < CCdt.Columns.Count-1; i++)
            {
                resDt.Columns.Add(CCdt.Columns[i].ColumnName, CCdt.Columns[i].DataType);
            }
            object[] o = new object[resDt.Columns.Count];
            for (int i = 0; i < CCdt.Rows.Count; i++)
            { 
                o[0]=CCdt.Rows[i].ItemArray[0].ToString()+", <b>"+String.Format("{0:### ### ##0.##}",double.Parse(CCdt.Rows[i].ItemArray[1].ToString()))+"</b>, "+CCdt.Rows[i].ItemArray[2].ToString().ToLower();
                o[1] = CCdt.Rows[i].ItemArray[1].ToString();
                resDt.Rows.Add(o);
            }
            CC.DataSource = resDt;
            Label7.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart3_title"), SelectedPeriod.Value.Split('[', ']')[SelectedPeriod.Value.Split('[', ']').Length - 2]);
        }
        protected void CR_DataBinding(object sender, EventArgs e)
        {
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart4_mark_"), true);
        }

        protected void G2_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (norefresh2.Value == "Yes")
            {
                SelectedPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + e.Row.Cells[0].Text + "]";
                LC.DataBind();
                CC.DataBind();
            }
        }

        protected void G2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double Coef = 1;
            if (BN == "IE") { Coef = 0.985; }
            if (BN == "FIREFOX") { Coef = 1; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1.03; };
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.099 * Coef);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.1 * Coef);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.1 * Coef);
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
            }
            e.Layout.Bands[0].Columns[0].Header.Caption = "Год";
        }

        protected void CC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                for (int i = 0; i < CCdt.Rows.Count; i++)
                {

                    Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(29, 210 + i * 20 - i, 390, 10), CCdt.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                    e.SceneGraph.Add(textLabel);
                   
                }
            }
            catch { }
        }
    }
}
