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
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0002._003
{
    public partial class _default :CustomReportPage
    {

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }

        CustomParam SelectItemGrid;
        CustomParam P1;
        CustomParam P2;
        CustomParam P3;
        CustomParam region;
        CustomParam P4;

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            SelectItemGrid = UserParams.CustomParam("Param");
            P1 = UserParams.CustomParam("p1");
            P2 = UserParams.CustomParam("p2");
            P3 = UserParams.CustomParam("p3");
            P4 = UserParams.CustomParam("p4");
            G.Height = 230;
            region = UserParams.CustomParam("region");
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);
            G2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            C2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);
            G3.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            C3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);
            G4.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            C4.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);
            WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G);
            pa2.AddLinkedRequestTrigger(G2);
            pa3.AddLinkedRequestTrigger(G3);
            pa4.AddLinkedRequestTrigger(G4);
        }

        protected Dictionary<string, int> GetParam(string q, string param)
        {
            Dictionary<string, int> areas = new Dictionary<string, int>();
            SelectItemGrid.Value = param;
            DataTable dt = GetDSForChart(q);

            for (int i = dt.Rows.Count - 1; i > -1; i--)
            {
                areas.Add(dt.Rows[i].ItemArray[0].ToString(), 0);
            }

            return areas;

        }


        string baseRegion;
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                //RegionSettingsHelper.Instance.SetWorkingRegion("Gubkinski");
                baseRegion = RegionSettingsHelper.Instance.RegionBaseDimension;
                //Label1.Text = baseRegion;

                if (!Page.IsPostBack)
                {

                    Dictionary<string, int> areas = new Dictionary<string, int>();
                    areas.Add("МP", 0);
                    areas.Add("ГО", 0);
                    Area.FillDictionaryValues(areas);
                    Area.SetСheckedState("ГО", true);

                    //SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("param1");
                    Dictionary<string, int> d = GetParam("params", RegionSettingsHelper.Instance.GetPropertyValue("param1"));
                    d.Remove("Значение не указано");
                    Type1.FillDictionaryValues(d);
                    Type2.FillDictionaryValues(GetParam("params", RegionSettingsHelper.Instance.GetPropertyValue("param2")));
                    Type3.FillDictionaryValues(GetParam("params", RegionSettingsHelper.Instance.GetPropertyValue("param3")));
                    Type1.SetСheckedState(RegionSettingsHelper.Instance.GetPropertyValue("param_default_1"), 1 == 1);
                    Type2.SetСheckedState(RegionSettingsHelper.Instance.GetPropertyValue("param_default_2"), 1 == 1);
                    Type3.SetСheckedState(RegionSettingsHelper.Instance.GetPropertyValue("param_default_3"), 1 == 1);
                    P4.Value = "[Measures].[Значение]";
                }
                P1.Value = RegionSettingsHelper.Instance.GetPropertyValue("param1") + ".[" + Type1.SelectedValue + "]";
                P2.Value = RegionSettingsHelper.Instance.GetPropertyValue("param2") + ".[" + Type2.SelectedValue + "]";
                P3.Value = RegionSettingsHelper.Instance.GetPropertyValue("param3") + ".[" + Type3.SelectedValue + "]";
                
                if (Area.SelectedIndex == 0)
                {
                    region = ForMarks.SetMarks(region, ForMarks.Getmarks("region_MO_"), 1 == 1);
                }
                else
                {
                    region = ForMarks.SetMarks(region, ForMarks.Getmarks("region_GO_"), 1 == 1);
                }

               // Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);

                L1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid1"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);
                L2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid2"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);
                L3.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid3"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);
                L4.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid4"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);

                G.DataBind();
                G2.DataBind();
                G3.DataBind();
                G4.DataBind();
                TitleChart.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, G.Rows[0].Cells[0].Text);

                Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart2"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, G2.Rows[0].Cells[0].Text);

                Label8.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart3"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, G3.Rows[0].Cells[0].Text);

                Label11.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart4"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, G4.Rows[0].Cells[0].Text);

                Page.Title = Hederglobal.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitlePage"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);

                //if (!Page.IsPostBack) 
                { SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems_" + (1).ToString()); }
                C.DataBind();
                
                SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems_"), 1 == 1);
                DataTable dt = GetDSForChart("Ge");
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    G.Rows[i].Cells[0].Text += ", " + dt.Rows[0].ItemArray[i + 1].ToString().ToLower();
                }
                //if (!Page.IsPostBack) 
                { SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems1_" + (1).ToString()); }
                C2.DataBind();

                //if (!Page.IsPostBack) 
                { SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems2_" + (1).ToString()); }
                C3.DataBind();

                //if (!Page.IsPostBack) 
                { SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems3_" + (1).ToString()); }
                C4.DataBind();
            }
            catch { }


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

        protected void G_DataBinding(object sender, System.EventArgs e)
        {
            try
            {
                SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems_"), 1 == 1);
                DataTable dt1 = GetDSForChart("G1");
                DataTable dt2 = GetDSForChart("G");
                DataTable dt = new DataTable();

                dt.Columns.Add("Показатель");
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    dt.Columns.Add(i.ToString() + ";Значение;" + dt1.Rows[i].ItemArray[0].ToString(), dt1.Columns[2].DataType);
                    // dt.Columns.Add(i.ToString() + "Индикатор", dt1.Columns[2].DataType);
                    dt.Columns.Add(i.ToString() + "Ранг");

                }
                object[] o;
                //   try
                {
                    int i = 0;
                    for (i = 1; i < dt1.Columns.Count - 1; i++)
                    {
                        o = new object[dt1.Rows.Count * 2 + 1];
                        o[0] = dt1.Columns[i].Caption;

                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {                            
                            {
                            
                                o[j * 2 + 1] = dt1.Rows[j].ItemArray[i];
                            
                            }
                        }

                        dt.Rows.Add(o);

                    }
                    o = new object[dt1.Rows.Count * 2 + 1];
                    o[0] = dt1.Columns[i].Caption;

                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {

                        //  try
                        {
                            //   Label1.Text += dt1.Rows[j].ItemArray[i].ToString() + dt2.Rows[j].ItemArray[i].ToString();
                            //o[j * 2 + 1] = dt1.Rows[j].ItemArray[i];
                            o[j * 2 + 1] = dt2.Rows[j].ItemArray[i];
                        }
                        // catch { }

                    }

                    dt.Rows.Add(o);

                }
            G.DataSource = dt;
            }
            catch {  }

            

        }

        protected DataTable GetDt(UltraWebGrid Gr)
        {

            try
            {
                Gr.Columns.Clear();
                Gr.Bands[0].HeaderLayout.Clear();
                Gr.Rows.Clear();
                Gr.Bands.Clear();


                DataTable dt1 = GetDSForChart("G1");
                DataTable dt2 = GetDSForChart("G");
                DataTable dt = new DataTable();

                dt.Columns.Add("Показатель");
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    dt.Columns.Add(i.ToString() + ";Значение;" + dt1.Rows[i].ItemArray[0].ToString(), dt1.Columns[2].DataType);
                    // dt.Columns.Add(i.ToString() + "Индикатор", dt1.Columns[2].DataType);
                    dt.Columns.Add(i.ToString() + "Ранг");

                }
                //   try
                object[] o;
                //   try
                {
                    int i = 0;
                    for (i = 1; i < dt1.Columns.Count - 1; i++)
                    {
                        o = new object[dt1.Rows.Count * 2 + 1];
                        o[0] = dt1.Columns[i].Caption;

                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {

                            //  try
                            {
                                //   Label1.Text += dt1.Rows[j].ItemArray[i].ToString() + dt2.Rows[j].ItemArray[i].ToString();
                                o[j * 2 + 1] = dt1.Rows[j].ItemArray[i];
                                //   o[j * 3 + 2] = dt2.Rows[j].ItemArray[i];
                            }
                            // catch { }

                        }

                        dt.Rows.Add(o);

                    }
                    o = new object[dt1.Rows.Count * 2 + 1];
                    o[0] = dt1.Columns[i].Caption;

                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {

                        //  try
                        {
                            //   Label1.Text += dt1.Rows[j].ItemArray[i].ToString() + dt2.Rows[j].ItemArray[i].ToString();
                            //o[j * 2 + 1] = dt1.Rows[j].ItemArray[i];
                            o[j * 2 + 1] = dt2.Rows[j].ItemArray[i];
                        }
                        // catch { }

                    }

                    dt.Rows.Add(o);

                } 
            return dt;
            }
            catch {  }
            return null;
        }


        bool[] hidenColumns = new bool[0];
        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            if (hidenColumns.Length == 0) { hidenColumns = new bool[e.Row.Cells.Count]; }

            int max = 0;
            System.Decimal maxV = 0;
            int[] m = new int[(e.Row.Cells.Count - 1) / 2];
            for (int i = 0; i < m.Length; i++)
            {
                m[i] = i * 2 + 1;
            }

            for (int i = 0; i < m.Length; i++)
                for (int j = i; j < m.Length; j++)
                {
                    if ((System.Decimal)(e.Row.Cells[m[j]].Value) > (System.Decimal)(e.Row.Cells[m[i]].Value))
                    {
                        int b = m[i];
                        m[i] = m[j];
                        m[j] = b;

                    }
                }

            for (int i = 0; i < m.Length; i++)
            {
                e.Row.Cells[m[i] + 1].Text = (i+1).ToString();
            }




            for (int i = 0; i < m.Length; i++)
            {
                if (e.Row.Cells[i * 2 + 2].Value.ToString() == "1")
                {
                    e.Row.Cells[i * 2 + 2].Text = string.Format("<img src=\"../../../../images/starYellow.png\">     ") + e.Row.Cells[i * 2 + 2].Text;
                }
                if (e.Row.Cells[i * 2 + 2].Value.ToString() == (m.Length).ToString())
                {
                    e.Row.Cells[i * 2 + 2].Text = string.Format("<img src=\"../../../../images/starGray.png\">     ") + e.Row.Cells[i * 2 + 2].Text;
                }
                if (e.Row.Cells[i * 2 + 2].Text == "_")
                {
                    
                }

            }
        }


        protected void ForCrossJoin(LayoutEventArgs e, int span)
        {
            e.Layout.Bands[0].Columns[0].Width = 400;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.### ### ###");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
            }

            for (int i = 0; i < (G.Columns.Count - 1) / 2; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i * 2 + 1], "### ### ### ##0.000 0## ###");
            }

            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                ColumnHeader colHead;
                for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                {
                    colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                    colHead.RowLayoutColumnInfo.OriginY = 1;

                }
                int dva = span;// :[)
                // if (!Load1) { e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].HeaderLayout[0]); }
                int c = e.Layout.Bands[0].HeaderLayout.Count;
                try
                {
                    for (int i = 1; i < c; i += dva)
                    {
                        ColumnHeader ch = new ColumnHeader(true);
                        //CH = ch;
                        ch.Caption = e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[2];
                        try
                        {
                            e.Layout.Bands[0].HeaderLayout[i].Caption = e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[1];
                            e.Layout.Bands[0].HeaderLayout[i].Style.Wrap = true;
                            e.Layout.Bands[0].HeaderLayout[i + 1].Caption = e.Layout.Bands[0].HeaderLayout[i + 1].Caption.Remove(0, 1);
                           // e.Layout.Bands[0].HeaderLayout[i + 2].Caption = e.Layout.Bands[0].HeaderLayout[i + 2].Caption.Remove(0, 1);
                           // e.Layout.Bands[0].HeaderLayout[i + 2].Style.Wrap = true;
                        }
                        catch
                        {
                        }


                        ch.RowLayoutColumnInfo.OriginX = i;//Позицыя по х относительно всех колумав

                        ch.RowLayoutColumnInfo.OriginY = 0;// по у
                        ch.RowLayoutColumnInfo.SpanX = dva;//Скока ячей резервировать
                        e.Layout.Bands[0].HeaderLayout.Add(ch);



                    }
                }
                catch
                {//Baanzzzaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiiiiii!
                }
            }

        }



        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            ForCrossJoin(e, 2);

        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
         //   P4.Value = "--";
            C.DataSource = GetDSForChart("G2");
         //   P4.Value = "";
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems_" + (e.Row.Index + 1).ToString());
            TitleChart.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart1"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, e.Row.Cells[0].Text);

            if (e.Row.Index == G.Rows.Count - 1)
            {
                P4.Value = "[Measures].[Индикатор]";
            }
            else
            {
                P4.Value = "[Measures].[Значение]";
            }
            C.DataBind();
        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {
         //   P4.Value = "--";
            C2.DataSource = GetDSForChart("G2");
         //   P4.Value = "";
        }

        protected void G2_DataBinding(object sender, EventArgs e)
        {
            SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems1_"), 1 == 1);
            G2.DataSource = GetDt(G2);
        }

        protected void G2_ActiveRowChange(object sender, RowEventArgs e)
        {
            SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems1_" + (e.Row.Index + 1).ToString());
            Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart2"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, e.Row.Cells[0].Text);
            if (e.Row.Index == G2.Rows.Count - 1)
            {
                P4.Value = "[Measures].[Индикатор]";
            }
            else
            {
                P4.Value = "[Measures].[Значение]";
            }
            C2.DataBind();
        }

        protected void G3_DataBinding(object sender, EventArgs e)
        {
            SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems2_"), 1 == 1);
            G3.DataSource = GetDt(G3);
        }

        protected void G3_ActiveRowChange(object sender, RowEventArgs e)
        {
            SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems2_" + (e.Row.Index + 1).ToString());
            Label8.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart3"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, e.Row.Cells[0].Text);
            if (e.Row.Index == G3.Rows.Count - 1)
            {
                P4.Value = "[Measures].[Индикатор]";
            }
            else
            {
                P4.Value = "[Measures].[Значение]";
            }
            C3.DataBind();
        }

        protected void C3_DataBinding(object sender, EventArgs e)
        {
          //  P4.Value = "--";
            C3.DataSource = GetDSForChart("G2");
          //  P4.Value = "";
        }

        protected void G4_DataBinding(object sender, EventArgs e)
        {
            SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems3_"), 1 == 1);
            G4.DataSource = GetDt(G4);
        }

        protected void G4_ActiveRowChange(object sender, RowEventArgs e)
        {
            SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems3_" + (e.Row.Index + 1).ToString());
            Label11.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart4"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, e.Row.Cells[0].Text);
            if (e.Row.Index == G4.Rows.Count - 1)
            {
                P4.Value = "[Measures].[Индикатор]";
            }
            else
            {
                P4.Value = "[Measures].[Значение]";
            }
            C4.DataBind();
        }

        protected void C4_DataBinding(object sender, EventArgs e)
        {
        //    P4.Value = "--";
            C4.DataSource = GetDSForChart("G2");
         //   P4.Value = "";
        }
    }
}
