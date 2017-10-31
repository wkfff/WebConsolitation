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
using Krista.FM.Server.Dashboards.reports.MO.MO_0001;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001._0032._00
{
    public partial class _default : CustomReportPage
    {

        private CustomParam SelectItemGrid;
        private CustomParam CurentPeriod;
        private CustomParam CurentRegion;
        private CustomParam marks;
        private CustomParam norefresh;
        private CustomParam norefresh2;
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
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
        int lastYear = 2008;
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
                lastYear = int.Parse(cs.Axes[0].Positions[cs.Axes[0].Positions.Count - 1].Members[0].Caption);

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
            double Coef = 1;
            marks = UserParams.CustomParam("marks");
            SelectItemGrid = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("SelectItemGrid"));
            CurentPeriod = UserParams.CustomParam("last_year");
            CurentRegion = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentRegion"));
            norefresh = UserParams.CustomParam("r");
            norefresh2 = UserParams.CustomParam("r2");
        
            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 1; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1; };

            G.Width = CRHelper.GetGridWidth(screen_width * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForGrid"))-10);
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 * Coef - 15); //CRHelper.GetChartWidth(screen_width * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForChartDynamics"))-6);
            C.Width = CRHelper.GetGridWidth(screen_width * 1 * Coef - 30);
            if (BN == "IE")
            {
                G.Height = int.Parse(C.Height.Value.ToString()) - 31;
            }
            else 
            {
                if (BN == "FIREFOX")
                {
                    G.Height = int.Parse(C.Height.Value.ToString()) - 14;
                }
                else {
                    G.Height = int.Parse(C.Height.Value.ToString()) - 34;
                }
            }

            PanelDynamicChart.AddLinkedRequestTrigger(G);
            PanelDynamicChart.AddRefreshTarget(C);
            PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
        }
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string s;
            CurentRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            Page.Title = "Характеристика территории МО РФ";
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
            }
            CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + Year.SelectedValue + "]";
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid_mark_"), true);
            G.DataBind();
            G.Rows[0].Selected = 1 == 1;
            G.Rows[0].Activated = 1 == 1;
            G.Rows[0].Activate();
            SelectItemGrid.Value = GetString_(G.Rows[0].Cells[0].Text);
            C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b> " + G.Rows[0].Cells[2].Text.ToLower();
            Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), G.Rows[0].Cells[0].Text.Split(',')[0], G.Rows[0].Cells[2].Text.ToLower());
            marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("grid_mark_1");
            C.DataBind();
            UltraChart1.DataBind();
            Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleChart1");
            G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            s = RegionSettingsHelper.Instance.GetPropertyValue("titlePage");
            Hederglobal.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
            Page.Title = Hederglobal.Text;
            Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("PageSubTitleText");
            s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");
            Label1.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
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

        protected void C_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                min = double.Parse(dt.Rows[0].ItemArray[1].ToString());

                for (int i = 1; i < dt.Rows[0].ItemArray.Length - 1; i++)
                {
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                }
            C.Axis.Y.RangeMax = max + 0.5;
            C.Axis.Y.RangeMin = 0;
            C.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            C.DataSource = dt;
           /* TextPage.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("textPage"),
            Year.SelectedValue,
            UserComboBox.getLastBlock(marks.Value),
            UserComboBox.getLastBlock(CurentRegion.Value),
            dt.Rows[0].ItemArray[1].ToString(),
            (System.Decimal)(dt.Rows[0].ItemArray[2]) > (System.Decimal)(dt.Rows[0].ItemArray[1]) ? "больше чем в" : "меньше чем в",
            (int.Parse(Year.SelectedValue) - 1).ToString(),
            "2002",
            ((System.Decimal)(dt.Rows[0].ItemArray[1]) > (System.Decimal)(dt.Rows[0].ItemArray[dt.Rows[0].ItemArray.Length - 1]) ? "увеличилась" : ((System.Decimal)(dt.Rows[0].ItemArray[1]) < (System.Decimal)(dt.Rows[0].ItemArray[dt.Rows[0].ItemArray.Length - 1])? "уменьшилась":"неизменилось")),
            (-int.Parse(dt.Columns[1].Caption) + int.Parse(dt.Columns[dt.Columns.Count - 1].Caption)).ToString(),
            Math.Round(((System.Decimal)(dt.Rows[0].ItemArray[dt.Rows[0].ItemArray.Length - 1]) / (System.Decimal)(dt.Rows[0].ItemArray[1]) * 100)).ToString());*/
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (norefresh2.Value == "Yes")
                {
                    ArrayList itemGrid = ForMarks.Getmarks("grid_mark_");
                    int i = 0;
                    for (i = 0; itemGrid.Count > i; i++)
                    {
                        if (itemGrid[i].ToString().Contains(GetString_(e.Row.Cells[0].Text)))
                        {
                            break;
                        }
                    }
                    marks.Value = itemGrid[i].ToString();
                    SelectItemGrid.Value = GridItems[e.Row.Index].ToString();
                    string title = e.Row.Cells[0].Text.Split(',')[0];
                    for (int j = 1; j < e.Row.Cells[0].Text.Split(',').Length - 1; j++)
                    {
                        title += "," + e.Row.Cells[0].Text.Split(',')[j];
                    }
                    Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), title, e.Row.Cells[2].Text.ToLower());
                    C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b> " + e.Row.Cells[2].Text.ToLower();
                    C.DataBind();
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
            double Coef = 1;
            if (BN == "IE") { Coef = 0.999; }
            if (BN == "FIREFOX") { Coef = 0.96; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 0.97; };

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForFirstColumnsGrid")));
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForSecondColumnsGrid")));
            e.Layout.Bands[0].Columns[0].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForFirstColumnsGrid");
            e.Layout.Bands[0].Columns[1].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForSecondColumnsGrid");
            e.Layout.Bands[0].Columns[2].Hidden = 1 == 1;
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
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart1_mark_"), true);
            UltraChart1.DataSource = GetDSForChart("chart1");
        }

        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = lastYear;
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
    }
}
