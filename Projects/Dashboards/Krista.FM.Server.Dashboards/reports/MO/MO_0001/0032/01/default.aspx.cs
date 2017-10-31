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

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001._0032._01
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
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            double Coef = 1;
            if (BN == "IE") { Coef = 1.01; }
            if (BN == "FIREFOX") { Coef = 1.01; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1.02; };
            base.Page_PreLoad(sender, e);
            marks = UserParams.CustomParam("marks");
            SelectItemGrid = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("SelectItemGrid"));
            CurentPeriod = UserParams.CustomParam("last_year");
            norefresh = UserParams.CustomParam("r");
            norefresh2 = UserParams.CustomParam("r2");
            CurentRegion = UserParams.CustomParam(RegionSettingsHelper.Instance.GetPropertyValue("CurrentRegion"));
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForChartDynamics"))-10);
            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForGrid"))-20);
            Coef = 1;
            if (BN == "IE") { Coef = 1.01; }
            if (BN == "FIREFOX") { Coef = 1; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1.01; };
            C.Height = CRHelper.GetChartHeight(300);
            G.Height = CRHelper.GetGridHeight(300 * Coef);
            Coef = 1;
            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 1.001; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1.003; };
            UltraChart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * Coef * 1 - 40);
            PanelDynamicChart.AddLinkedRequestTrigger(G);
            PanelDynamicChart.AddRefreshTarget(C);
            PanelDynamicChart.AddRefreshTarget(Label2);
            WebAsyncRefreshPanel1.AddLinkedRequestTrigger(PanelDynamicChart);
            WebAsyncRefreshPanel1.AddRefreshTarget(UltraChart1);
            WebAsyncRefreshPanel1.AddRefreshTarget(Label5);
            PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
        }
        string lastdate = "";
        string lastdate2 = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string s;
            string regionss = RegionSettings.Instance.Id;
            CurentRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid_mark_"), true);
                SelectItemGrid.Value = "children";
                Year.FillDictionaryValues(GenDistonary(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForParamPeriod")));
                norefresh.Value = Year.SelectedValue;
                norefresh2.Value = "Yes";
                s = RegionSettingsHelper.Instance.GetPropertyValue("ParamPeriodValueDefault");
                if (!string.IsNullOrEmpty(s))
                {
                    Year.SetСheckedState(s, true);
                }
            }
            CurentPeriod.Value = "[Период].[Год Квартал Месяц].[Год].[" + Year.SelectedValue + "]";
            s = RegionSettingsHelper.Instance.GetPropertyValue("titlePage");
            Hederglobal.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
            Page.Title = Hederglobal.Text;
            Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("PageSubTitleText");
            s = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");
            Label1.Text = string.Format(s, UserComboBox.getLastBlock(CurentRegion.Value), Year.SelectedValue);
            marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid_mark_"), true);
            G.DataBind();
            Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), G.Rows[0].Cells[0].Text.Split(',')[0], G.Rows[0].Cells[2].Text.ToLower());
            SelectItemGrid.Value = GetString_(G.Rows[0].Cells[0].Text);
            ArrayList itemGrid = ForMarks.Getmarks("grid_mark_");
            int i = 0;
            for (i = 0; itemGrid.Count > i; i++)
            {
                if (GetString_(G.Rows[0].Cells[0].Text) == UserComboBox.getLastBlock(itemGrid[i].ToString()))
                {
                    break;
                }
            }
            marks.Value = itemGrid[i].ToString();
              s = RegionSettingsHelper.Instance.GetPropertyValue("ParamPeriodValueDefault");
            G.Rows[0].Selected = true;
            G.Rows[0].Activate();
            C.DataBind();
            UltraChart1.DataBind();
            C.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " + G.Rows[0].Cells[2].Text.ToLower();
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
            G.Rows[0].Activated = true;
            G.Rows[0].Selected = true;
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

        int DCLD = 2008;

        protected void C_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            DCLD = int.Parse(dt.Columns[dt.Columns.Count - 1].Caption);
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
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (norefresh2.Value == "Yes")
            {
                CRHelper.SaveToQueryLog("1");
                ArrayList itemGrid = ForMarks.Getmarks("grid_mark_");
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
                Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), e.Row.Cells[0].Text.Split(',')[0], e.Row.Cells[2].Text.ToLower());

                C.DataBind();
                UltraChart1.DataBind();
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
            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 0.978; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 0.99; };
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForFirstColumnsGrid")));
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * Coef * double.Parse(RegionSettingsHelper.Instance.GetPropertyValue("widthForSecondColumnsGrid")));
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForFirstColumnsGrid");
            e.Layout.Bands[0].Columns[1].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("nameForSecondColumnsGrid");
            e.Layout.Bands[0].Columns[2].Hidden = 1 == 1;
        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }
        int CLD = 2008;

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            WebAsyncRefreshPanel1.Visible = 1 == 1;
            Panel1.Visible = 1 == 1;
            string[] aS = marks.Value.Split('.');

            string[] res = { aS[aS.Length - 1] };
            marks.Value = marks.Value.Split(res, StringSplitOptions.RemoveEmptyEntries)[0] + caseParam(aS[aS.Length - 1]);

            Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart2"), marks.Value.Split('[', ']')[marks.Value.Split('[', ']').Length - 2]);

            DataTable dt = GetDSForChart(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics"));
            CellSet cs;
            cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(RegionSettingsHelper.Instance.GetPropertyValue("idInQueryForChartDynamics")));
            try
            {
                Label5.Text =Label5.Text+", "+ cs.Axes[1].Positions[0].Members[0].MemberProperties[0].Value.ToString().ToLower();
                UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " + cs.Axes[1].Positions[0].Members[0].MemberProperties[0].Value.ToString().ToLower();
                CLD = int.Parse(dt.Columns[dt.Columns.Count - 1].Caption);
                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                min = double.Parse(dt.Rows[0].ItemArray[1].ToString());

                for (int i = 2; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                }
                if ((marks.Value.Split('[', ']')[marks.Value.Split('[', ']').Length - 2] == "Коэффициент механического прироста") || (marks.Value.Split('[', ']')[marks.Value.Split('[', ']').Length - 2] == "Коэффициент естественного прироста"))
                {
                    if (min >= 0)
                    {
                        UltraChart1.Axis.Y.RangeMin = min;
                    }
                    else
                    {
                        UltraChart1.Axis.Y.RangeMin = min - 1;
                    }
                }
                else
                {
                    UltraChart1.Axis.Y.RangeMin = min - 1;
                }
                UltraChart1.Axis.Y.RangeMax = max + 1;
                UltraChart1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            }
            catch { }
            UltraChart1.DataSource = dt;
        }

        #region //Сопастовление параметров
        string caseParam(string s)
        {
            //s.Remove(0, 1);
            //s.Remove(s.Length - 1, 1);//= "[" + s + "]";
            if (s == "[Численность постоянного населения на начало года]")
            { s = "Среднегодовая численность населения"; }
            else
                if (s == "[Численность постоянного населения на конец года]")
                { s = "Среднегодовая численность населения"; }
                else
                    if (s == "[Число родившихся]")
                    { s = "Коэффициент рождаемости"; }
                    else
                        if (s == "[Число умерших]")
                        { s = "Коэффициент смертности"; }
                        else
                            if (s == "[Естественный прирост]")
                            { s = "Коэффициент естественного прироста"; }
                            else
                                if (s == "[Механический прирост]")
                                { s = "Коэффициент механического прироста"; }
                                else
                                    if (s == "[Число зарегистрированных браков]")
                                    { s = "Коэффициент брачности"; }
                                    else
                                        if (s == "[Число зарегистрированных разводов]")
                                        { s = "Коэффициент разводов"; }
                                        else
                                        {
                                            s = "Среднегодовая численность населения";
                                        }
            return "[" + s + "]";
        }
        #endregion

        protected void chart_avg_count_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                int xOct = 0;
                int xNov = 0;
                Text decText = null;
                int year = CLD;
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

        protected void C_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                int xOct = 0;
                int xNov = 0;
                Text decText = null;
                int year = DCLD;
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
