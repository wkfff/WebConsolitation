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

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0002._001
{
    public partial class _default : CustomReportPage
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
            region = UserParams.CustomParam("region");
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth-20);
            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth-10);
            WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G);
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

                    //   SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("param1");
                    Dictionary<string, int> d = GetParam("params", RegionSettingsHelper.Instance.GetPropertyValue("param1"));
                    d.Remove("Значение не указано");
                    Type1.FillDictionaryValues(d);
                    Type2.FillDictionaryValues(GetParam("params", RegionSettingsHelper.Instance.GetPropertyValue("param2")));
                    Type3.FillDictionaryValues(GetParam("params", RegionSettingsHelper.Instance.GetPropertyValue("param3")));
                    Type1.SetСheckedState(RegionSettingsHelper.Instance.GetPropertyValue("param_default_1"), 1 == 1);
                    Type2.SetСheckedState(RegionSettingsHelper.Instance.GetPropertyValue("param_default_2"), 1 == 1);
                    Type3.SetСheckedState(RegionSettingsHelper.Instance.GetPropertyValue("param_default_3"), 1 == 1);
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

                Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);

                G.DataBind();
                TitleChart.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, G.Rows[0].Cells[0].Text);

                //Hederglobal.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitlePage"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);
                Page.Title = Hederglobal.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitlePage"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue);

                if (!Page.IsPostBack) { SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems_" + (1).ToString()); }
                C.DataBind();

                SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems_"), 1 == 1);

                

                DataTable dt = GetDSForChart("Ge");
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    G.Rows[i].Cells[0].Text += ", " + dt.Rows[0].ItemArray[i + 1].ToString();
                }

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
            G.Columns.Clear();
            G.Bands[0].HeaderLayout.Clear();
            G.Rows.Clear();
            G.Bands.Clear();

            SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems_"), 1 == 1);
            DataTable dt1 = GetDSForChart("G1");
            DataTable dt2 = GetDSForChart("G");
            DataTable dt = new DataTable();

            dt.Columns.Add("Показатель");
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt.Columns.Add(i.ToString() + ";Индикатор;" + dt1.Rows[i].ItemArray[0].ToString(), dt1.Columns[2].DataType);
              //  dt.Columns.Add(i.ToString() + "Индикатор", dt1.Columns[2].DataType);
                dt.Columns.Add(i.ToString() + "Ранг");

            }
            //   try
            {
                for (int i = 1; i < dt1.Columns.Count; i++)
                {
                    object[] o = new object[dt1.Rows.Count * 2 + 1];
                    o[0] = dt1.Columns[i].Caption;

                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {

                        //  try
                        {
                            //   Label1.Text += dt1.Rows[j].ItemArray[i].ToString() + dt2.Rows[j].ItemArray[i].ToString();
                            o[j * 2 + 1] = dt1.Rows[j].ItemArray[i];
                           // o[j * 3 + 2] = dt2.Rows[j].ItemArray[i];
                        }
                        // catch { }

                    }

                    dt.Rows.Add(o);

                }
            }
            //   catch { }



            //  DataTable Gdt = new DataTable();
            ////  try
            //  {
            //      //ArrayList GridRow = new ArrayList();
            //      SelectItemGrid.Value = ar[0].ToString();
            //      DataTable dt = GetDSForChart("G");
            //      Gdt.Columns.Add("Показатель");
            //      for (int i = 0; i < dt.Rows.Count; i++)
            //      {
            //          Gdt.Columns.Add(i.ToString()+"Значение;" + dt.Rows[i].ItemArray[0].ToString(), dt.Columns[1].DataType);
            //          Gdt.Columns.Add(i.ToString() + "Индикатор", dt.Columns[2].DataType);
            //          Gdt.Columns.Add(i.ToString()+"Ранг");

            //      }

            //      for (int i = 0; i < ar.Count; i++)
            //      {
            //          SelectItemGrid.Value = ar[i].ToString();
            //          dt = GetDSForChart("G");
            //          DataTable dt1 = GetDSForChart("G1");

            //          int max = 0, min = 0;
            //          for (int j = 0; j < dt.Rows.Count; j++)
            //          {
            //              try
            //              {
            //                  System.Decimal a = (System.Decimal)(dt.Rows[j].ItemArray[2]);
            //                  max = min=j;
            //                  break;
            //              }
            //              catch { }
            //          }

            //          Object[] o = new object[Gdt.Columns.Count];
            //          o[0] = SelectItemGrid.Value.Split('[',']')[SelectItemGrid.Value.Split('[',']').Length-2];

            //          for (int j = 0; j < dt.Rows.Count; j++)
            //          {
            //              try
            //              {

            //                  o[j * 3 + 1] = dt1.Rows[j].ItemArray[1];  }
            //              catch { }
            //                  try
            //                  {
            //                      o[j * 3 + 2] = dt.Rows[j].ItemArray[2];
            //                  }
            //                  catch { }

            //              if (o[j * 3 + 1].GetType() != o[j * 3 + 2].GetType())
            //              {
            //                  try
            //                  {
            //                      Decimal a = (System.Decimal)(o[j * 3 + 1]);
            //                  }
            //                  catch
            //                  {
            //                      o[j * 3 + 1] = 0;
            //                  }
            //                  try
            //                  {
            //                      Decimal a = (System.Decimal)(o[j * 3 + 2]);
            //                  }
            //                  catch
            //                  {
            //                      o[j * 3 + 2] = 0;
            //                  }
            //                  try
            //                  {
            //                      Decimal a = (System.Decimal)(o[j * 3 + 2]);
            //                  }
            //                  catch
            //                  {
            //                      o[j * 3 + 3] = "_";
            //                  }
            //              }

            //              try
            //              {
            //                  if ((System.Decimal)(dt.Rows[max].ItemArray[2]) < (System.Decimal)(dt.Rows[j].ItemArray[2]))
            //                  {
            //                      max = j;
            //                  }
            //                  if ((System.Decimal)(dt.Rows[min].ItemArray[2]) > (System.Decimal)(dt.Rows[j].ItemArray[2]))
            //                  {
            //                      min = j;
            //                  }
            //              }
            //              catch {  }

            //          }
            //          o[max * 3 + 3] = "+";
            //          o[min * 3 + 3] = "-";

            //          Gdt.Rows.Add(o);


            //      }
            //  }
            G.DataSource = dt;

        }
        bool[] hidenColumns = new bool[0];
        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            if (hidenColumns.Length == 0) { hidenColumns = new bool[e.Row.Cells.Count]; }
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
                if (e.Row.Cells[i * 2 + 2].Value.ToString() == (m.Length).ToString())
                {
                     e.Row.Cells[i * 2 + 2].Text = string.Format("<img src=\"../../../../images/starGray.png\">    ") + e.Row.Cells[i * 2 + 2].Text;
                }
                if (e.Row.Cells[i * 2 + 2].Value.ToString() == "1")
                {
                   e.Row.Cells[i * 2 + 2].Text = string.Format("<img src=\"../../../../images/starYellow.png\">    ") + e.Row.Cells[i * 2 + 2].Text;
                }
                if (e.Row.Cells[i * 2 + 2].Text == "_")
                {
                    //e.Row.Cells[i].Text = "v";
                }

            }
        }


        protected void ForCrossJoin(LayoutEventArgs e, int span)
        {
            e.Layout.Bands[0].Columns[0].Width = 400;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.000 0## ###");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80);
            }
            for (int i = 0; i < (G.Columns.Count - 1) / 2; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i * 2 + 1], "### ### ### #0.000 0### ###");
                e.Layout.Bands[0].Columns[i * 2 + 2].Width = CRHelper.GetColumnWidth(60);
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
                          //  e.Layout.Bands[0].HeaderLayout[i + 2].Caption = e.Layout.Bands[0].HeaderLayout[i + 2].Caption.Remove(0, 1);
                            e.Layout.Bands[0].HeaderLayout[i + 2].Style.Wrap = true;
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
            for (int i = 0; i < (G.Columns.Count - 1) / 2; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i * 2 + 1], "### ### ### ##0.000 0## ###");
            }

        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
            P4.Value = "--";
            C.DataSource = GetDSForChart("G2");
            P4.Value = "";
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems_" + (e.Row.Index + 1).ToString());
            TitleChart.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleChart"), Area.SelectedValue, Type1.SelectedValue, Type2.SelectedValue, Type3.SelectedValue, e.Row.Cells[0].Text);
            C.DataBind();
        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }
    }
}
