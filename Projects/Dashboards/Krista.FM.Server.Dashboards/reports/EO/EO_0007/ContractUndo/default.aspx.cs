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

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebChart;

using System.Globalization;

using Infragistics.Documents.Reports.Report.Text;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Infragistics.WebUI.UltraWebNavigator;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using Orientation = Infragistics.Documents.Excel.Orientation;
using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;

namespace Krista.FM.Server.Dashboards.reports.EO.EO_0007.ContractUndo
{
    public partial class _default : CustomReportPage
    {

        private CustomParam WhippingAndGingerbread { get { return (UserParams.CustomParam("WhippingAndGingerbread")); } }
        private CustomParam GRBS { get { return (UserParams.CustomParam("GRBS")); } }
        private CustomParam Year { get { return (UserParams.CustomParam("Year")); } }
        private CustomParam MadHand { get { return (UserParams.CustomParam("MadHand")); } }

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }

        string AID(Dictionary<string, int> d, string str, int level)
        {
            string lev = "";
            for (; ; )
            {
                try
                {
                    d.Add(str + " " + lev, level);
                    break;
                }
                catch
                { }
                lev += " ";
            }
            return str + " " + lev;
        }

        Dictionary<string, int> ForParam(string query)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(query));
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            ls = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].Caption;
            return d;

        }
        string ls = "";

        Dictionary<string, int> ForParam_GRBS_PBS_XZ_MZ_i_Ibbbbbb(string query)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(query));
            //string year = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            d.Add("Все заказчики", 0);
            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                if (cs.Axes[1].Positions[i].Members[0].UniqueName[0] != '(')
                {

                    string[] ar = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']');
                    if (ar[ar.Length - 2] != "Несопоставленные данные") { AID(d, ar[ar.Length - 2], ar.Length < 10 ? 0 : 1); }
                }
                //AID(d,cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[',']').Length.ToString()+ " "+cs.Axes[1].Positions[i].Members[0].UniqueName, 0);
                //d.Add(ar[ar.Length - 1], ar.Length < 7 ? 0 : 1);
            }
            return d;
        }
        string DelLastsChar(string s, Char c)
        {
            for (int i = s.Length - 1; i > 0; i--)
            {
                if (s[i] == c)
                {
                    s = s.Remove(i, 1);
                }
                else
                {
                    break;
                }
            }
            return s;

        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 2 - 7 + 5);
            Children_G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 + 5);
            C.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 2 - 7 + 5);
            C.Height = 350;
            G.Height = 265;
            Children_G.Height = 275;
            C.ChartType = ChartType.PieChart;
            C.PieChart.OthersCategoryPercent = 0;
            C.Legend.SpanPercentage = 30;
            C.Legend.Visible = 1 == 1;
            C.Data.SwapRowsAndColumns = 1 == 2;
            _panel_children_G.AddLinkedRequestTrigger(G);
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);

                if (!Page.IsPostBack)
                {
                    a.FillDictionaryValues(ForParam("ld"));
                    b.ParentSelect = 1 == 1;
                    a.SetСheckedState(ls, 1 == 1);
                    b.FillDictionaryValues(ForParam_GRBS_PBS_XZ_MZ_i_Ibbbbbb("xm"));
                    
                    b.PanelHeaderTitle = "Заказчик";
                    b.Title = "Заказчик";

                    b.Width = 1020 - 337;

                    MadHand.Value = b.SelectedNode.Text;
                }
                string xZZZZZ = "";
                if (!CheckBox1.Checked)
                {
                    xZZZZZ = ".datamember";
                }
                if (b.SelectedValue != "Все заказчики")
                {
                    GRBS.Value = string.Format("[ГРБС__Сопоставимый].[ГРБС__Сопоставимый].[ГРБС].[{0}]{1}", b.SelectedNode.Level.ToString() == "0" ? DelLastsChar(b.SelectedNode.Text, ' ') : DelLastsChar(b.SelectedNode.Parent.Text, ' '), b.SelectedNode.Level.ToString() == "0" ? "" : ".[" + DelLastsChar(b.SelectedNode.Text, ' ') + "]") + xZZZZZ;
                }
                else
                {
                    GRBS.Value = "[ГРБС__Сопоставимый].[ГРБС__Сопоставимый].[ГРБС]";
                }
                Year.Value = DelLastsChar(a.SelectedValue, ' ');

                G.DataBind();

                G.Rows[0].Activate();
                G.Rows[0].Activated = 1 == 1;
                G.Rows[0].Selected = 1 == 1;
                //WhippingAndGingerbread.Value = cs.Axes[1].Positions[0].Members[0].UniqueName;
                WhippingAndGingerbread.Value = G.Rows[0].Cells[1].Text;
                object[] xzzz = new object[4];
                if (SumOther > 0)
                {

                    xzzz[0] = "";
                    xzzz[1] = "all";
                    xzzz[2] = "Остальные поставщики";
                    xzzz[3] = SumOther.ToString();
                    G.Rows.Add(new UltraGridRow(xzzz));
                }
                xzzz = new object[4];
                xzzz[0] = "";
                xzzz[1] = "ny_vashe_vse";
                xzzz[2] = "Общий итог";
                xzzz[3] = (SumNeOther + SumOther).ToString();
                G.Rows.Add(new UltraGridRow(xzzz));
                G.Rows[G.Rows.Count - 1].Cells[3].Style.Font.Bold = 1 == 1;
                G.Rows[G.Rows.Count - 1].Cells[2].Style.Font.Bold = 1 == 1;

                Children_G.DataBind();

                Children_G.Rows.Add();
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[0].Text = "Общий итог";
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[6].Text = "-";
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[0].ColSpan = 4;
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[0].Style.Font.Bold = 1 == 1;
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[5].Value = Sum5Column;
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[4].Value = Sum4Column;
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[4].Style.Font.Bold = 1 == 1;
                Children_G.Rows[Children_G.Rows.Count - 1].Cells[5].Style.Font.Bold = 1 == 1;

                C.DataBind();
                TGTL.Text = "Количество расторгнутых контрактов за " + a.SelectedValue + " год";
                CL.Text = "Соотношение исполненных и расторгнутых контрактов за " + a.SelectedValue + " год";
                BGTL.Text = "Фактическое исполнение расторгнутых контрактов за " + a.SelectedValue + " год";
                //Hederglobal.Text += "o)o";
                
                Page.Title = Hederglobal.Text;
            }
            catch {
                
            }
            Hederglobal.Text = "Сведения о расторгнутых контрактах в " + a.SelectedValue + " году (" + b.SelectedValue + ")";
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable cs_ = new DataTable();
            cs_.Columns.Add("№ п.п.");
            cs_.Columns.Add("Uniqe");
            cs_.Columns.Add("Поставщик");
            cs_.Columns.Add("Количество расторгнутых контрактов");
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("FatherGrid"));

            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
                cs_.Rows.Add((i + 1).ToString(), cs.Axes[1].Positions[i].Members[0].UniqueName, cs.Axes[1].Positions[i].Members[0].Caption, cs.Cells[i].Value.ToString());
            }

            if (cs_.Rows.Count > 0)
            {
                G.DataSource = cs_;
                _TEXT_.Text = "<font style=\"font-family:Arial;font-size:small\"><span style=\"font-size: 12pt;font-family: 'Times New Roman'; mso-fareast-font-family: 'Times New Roman'; mso-ansi-language: RU;mso-fareast-language: RU; mso-bidi-language: AR-SA\">Выделите строку для отображения данных о фактическом исполнении расторгнутых контрактов с конкретным поставщиком</span></font>";
                G.Height = 265;

            }
            else
            {
                    G.DataSource = null;
                    Children_G.Columns.Clear();
                    Children_G.Bands.Clear();
                    Children_G.Rows.Clear();
                    _TEXT_.Text = "";
                    G.Height = 350;
                
            }
            
            
        }

        protected void Children_G_DataBinding(object sender, EventArgs e)
        {   
            DataTable dt = new DataTable();
            DataTable Dt = GetDSForChart("ChildrenGrid");
            dt.Columns.Add("Реквизиты контракта");
            dt.Columns.Add("Поставщик");//Property
            dt.Columns.Add("Способ закупки");
            dt.Columns.Add("Дата прекращения действия");
            dt.Columns.Add("Стоимость контракта", Dt.Columns[1].DataType);
            dt.Columns.Add("Фактически оплачено", Dt.Columns[2].DataType);
            dt.Columns.Add("Процент исполнения");
            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                object[] o = new object[7];
                string[] O = Dt.Rows[i][0].ToString().Split(';');
                try
                {
                    o[0] = O[0];
                    o[1] = O[1];
                    o[2] = O[2];
                    o[3] = O[3];
                }
                catch { }
                try
                {
                    o[4] = Dt.Rows[i][1];
                    o[5] = Dt.Rows[i][2];
                    //BGBL.Text += Dt.Rows[i][1].GetType().ToString();
                    o[6] = ((System.Decimal)(100 * (System.Decimal)(Dt.Rows[i][2]) / (System.Decimal)(Dt.Rows[i][1]))).ToString("### ##0.00") + "%";
                }
                catch { }



                dt.Rows.Add(o);
            }
            Children_G.DataSource = G.Rows.Count>0 ?dt:null;//dt.Rows.Count>0?dt:Children_G.DataSource;
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid curG = (UltraWebGrid)(sender);
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;

            }

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            if (curG == G)
            {
                e.Layout.Bands[0].Columns[0].Width = 40;
                e.Layout.Bands[0].Columns[1].Hidden = 1 == 1;
                e.Layout.Bands[0].Columns[2].Width = 410;
                e.Layout.Bands[0].Columns[3].Width = 120;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.AllowSortingDefault = AllowSorting.No;

                e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
                e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                //e.Layout.NullTextDefault = "<font style='font-family:Arial;font-size:20pt;'><br><br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbspНет данных</font>";
                e.Layout.NoDataMessage = "<font style='font-family:Arial;font-size:16pt;'><br><br><br><br><br><br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbspНет данных</font>";

            }
            if (curG == Children_G)
            {
                e.Layout.Bands[0].Columns[0].Width = 82 * 3;
                e.Layout.Bands[0].Columns[1].Width = 80 * 3;
                e.Layout.Bands[0].Columns[2].Width = 95 * 3 + 5;
                e.Layout.Bands[0].Columns[3].Width = 50 * 2 + 10;
                e.Layout.Bands[0].Columns[4].Width = 50 * 2 + 10;
                e.Layout.Bands[0].Columns[5].Width = 50 * 2 + 10;
                e.Layout.Bands[0].Columns[6].Width = 37 * 2 + 10;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");

                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Left;

                e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Center;

                e.Layout.Bands[0].Columns[4].Header.Caption = "Стоимость контракта, руб.";
                e.Layout.Bands[0].Columns[5].Header.Caption = "Фактически оплачено, руб.";

                e.Layout.Bands[0].Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Center;

                //e.Layout.Bands[0].Columns[4].CellStyle.Font.Bold = 1 == 1;
                //e.Layout.Bands[0].Columns[5].CellStyle.Font.Bold = 1 == 1;
                //e.Layout.Bands[0].Columns[6].CellStyle.Font.Bold = 1 == 1;
                //e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                //e.Layout.Bands[0].Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.CellClickActionDefault = CellClickAction.NotSet;
                e.Layout.RowSelectorsDefault = RowSelectors.No;
                //e.Layout.NullTextDefault = "<font style='font-family:Arial;font-size:20pt;'><br><br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbspНет данных</font>";
                e.Layout.NoDataMessage = "<font style='font-family:Arial;font-size:16pt;'><br><br><br><br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbspНет данных</font>";



            }
        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
            C.DataSource = GetDSForChart("C_");
        }

        int SumOther = 0;
        int SumNeOther = 0;
        string pred = "";

        decimal Sum4Column = 0;
        decimal Sum5Column = 0;


        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            if ((UltraWebGrid)(sender) == G)
            {

                if ((e.Row.Index > 8) & (e.Row.Cells[3].Text != pred))
                {
                    e.Row.Hidden = 1 == 1;
                    SumOther += int.Parse(e.Row.Cells[3].Value.ToString());
                }
                else
                {
                    pred = e.Row.Cells[3].Text;
                    SumNeOther += int.Parse(e.Row.Cells[3].Value.ToString());
                }
            }
            else
            {
                Sum4Column += (System.Decimal)(e.Row.Cells[4].Value);
                Sum5Column += (System.Decimal)(e.Row.Cells[5].Value);
            }
        }

        protected void G_AddRow(object sender, RowEventArgs e)
        {

        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {


            //BGTL.Text += e.Row.Cells[1].Text + "|" + b.SelectedValue + "|" + G.Rows.Count.ToString()+"||<br>";
            if (e.Row.Index <= G.Rows.Count) 
            {
                
                WhippingAndGingerbread.Value = e.Row.Cells[1].Text;
            }
            else
            {
                WhippingAndGingerbread.Value = G.Rows[0].Cells[1].Text;
            }
            if (WhippingAndGingerbread.Value == "ny_vashe_vse")
            {
                
                {
                    WhippingAndGingerbread.Value = "{";
                    for (int i = 0; i < G.Rows.Count-2; i++)
                    {
             //           if (G.Rows[i].Hidden)
                        {
                            WhippingAndGingerbread.Value += G.Rows[i].Cells[1].Text + ",";
                        }


                    }
                    WhippingAndGingerbread.Value = WhippingAndGingerbread.Value.Remove(WhippingAndGingerbread.Value.Length - 1, 1) + "}";
                }
            }

            if (WhippingAndGingerbread.Value == "all")
            {
                WhippingAndGingerbread.Value = "{";
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    if (G.Rows[i].Hidden)
                    {
                        WhippingAndGingerbread.Value += G.Rows[i].Cells[1].Text + ",";
                    }


                }
                WhippingAndGingerbread.Value = WhippingAndGingerbread.Value.Remove(WhippingAndGingerbread.Value.Length - 1, 1) + "}";
            }
                

            Children_G.DataBind();
            Children_G.Rows.Add();
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[0].Text = "Общий итог";
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[6].Text = "-";
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[0].ColSpan = 4;
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[0].Style.Font.Bold = 1 == 1;
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[5].Value = Sum5Column;
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[4].Value = Sum4Column;
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[4].Style.Font.Bold = 1 == 1;
            Children_G.Rows[Children_G.Rows.Count - 1].Cells[5].Style.Font.Bold = 1 == 1;
            MadHand.Value = b.SelectedNode.Text;
        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }
    }
}
