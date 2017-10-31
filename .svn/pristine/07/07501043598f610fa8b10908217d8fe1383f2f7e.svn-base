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

using Infragistics.UltraChart.Core.Layers;

using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.MO.MO_0001._0028._01
{
    public partial class _default : CustomReportPage
    {
        int lastYear = 2008;

        ArrayList al = new ArrayList();

        string ls = "";


        /// <summary>
        /// очень удивлённый метод, он не ожидал что в нём будет такой кот
        /// </summary>
        /// <param name="^_^">няяяяя</param>
        /// <returns>существует вероятность тово что вернёт ответ на вопрос жизни, вселеной и всего такова, каторый даже мыши не знают, но очень хотят узнать</returns>
        int o_0(Node v_v)
        {
            //всякие разные кружочки
            int O_o = 0;

            //я удивляюсь
            Node O_O = v_v;

            //конец, удивлённо плачу
            Node Q_Q = null;

            for (; O_O != Q_Q; O_O = O_O.PrevNode) { O_o++; }

            //папа
            Node V_V = v_v.Parent;

            //дедушка оп отцовской линии
            Node W_W = V_V.Parent;

            //дяди
            Nodes v_V__V_V__V_v = W_W.Nodes;

            for (int _ = 0; _ < V_V.Index; _++)
            {
                //двоюродные братья
                Nodes v_v___v_v___v_v = v_V__V_V__V_v[_].Nodes;
                O_o += v_v___v_v___v_v.Count;
            }

            if (2 * 2 == 5)
            {
                ///мы нашли его!!! сообщите мышам!!!! СРОЧНО!!!!
                return 42;
            }
            return O_o;
        }

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }

        private CustomParam p1 { get { return (UserParams.CustomParam("p1")); } }
        private CustomParam p2 { get { return (UserParams.CustomParam("p2")); } }
        private CustomParam p3 { get { return (UserParams.CustomParam("p3")); } }
        private CustomParam p4 { get { return (UserParams.CustomParam("p4")); } }

        int vuschet = 2;
        string Lregion = "";

        #region это для параметра с териториями
        Dictionary<string, int> GenDForRegion(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            int i = 1;
            for (; i < cs.Axes[0].Positions.Count; i++)
            {
                d.Add(cs.Axes[0].Positions[i].Members[0].Caption, 0);
            }
            Lregion = cs.Axes[0].Positions[1].Members[0].Caption;
            return d;
        }
        #endregion

        #region Для этово, как там его, блин, аааа параметра ирархичного с датами!

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
                {
                }
                lev += " ";
            }
            return str + " " + lev;
        }

        Dictionary<string, int> GenDistonary1(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));

            string year = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            string poly = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[9];
            string qvart = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[11];
            string mounth = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];
            

            AID(d, year, 0);

            AID(d, poly, 1);

            AID(d, qvart, 2);

            AID(d, mounth, 3);


            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                if (year != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                {
                    year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                    AID(d, year, 0);
                    mounth = "";
                    qvart = "";
                    poly = "";
                }

                if (poly != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[9])
                {
                    poly = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[9];
                    AID(d, poly, 1);
                }

                if (qvart != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11])
                {
                    qvart = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11];
                    AID(d, qvart, 2);

                }

                if (mounth != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                {
                    mounth = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                    ls = AID(d, mounth, 3);
                }

            }


            string subS = "";

            ls2 = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName;
            //ls = //cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName.Split('[', ']')[13];

            return d;
        }
        string ls2 = "";
        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            string ly = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            string lm = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];


            //d.Add("Всего по области", 0);
            d.Add(ly, 0);
            d.Add(lm, 1);
            d.Add(cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[15], 2);
            al.Add(cs.Axes[1].Positions[0].Members[0].UniqueName);
            string subS = "";

            //красота

            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                try
                {

                    if (ly != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                    {
                        ly = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                        //d.Add(ly, 0);
                        lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                        //d.Add(lm, 1);
                        AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7], 0);
                        AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13], 1);
                        ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                        //try
                        //{
                        //    d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                        //}
                        //catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }

                        al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                    }
                    else
                        if (lm != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                        {
                            lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                            //d.Add(lm, 1);
                            AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13], 1);
                            ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            //try
                            //{
                            //    d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            //}
                            //catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }

                            

                            al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                        }
                        else
                        {
                            //try
                            //{
                            //    d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            //}
                            //catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }

                            ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);

                            al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                        }
                }
                catch { }

            }



            ls2 = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName;
            //ls = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName.Split('[', ']')[15];

            return d;
        }

        #endregion

        //string ls = "";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            TC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            TBC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);

            ////###########################################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //#############################################################################

            TC.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
            TBC.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
        }


        string GetAllChildrenInNode(Node n)
        {
            string res = "";
            if (n.Level != 3)
            {
                for (int i = 0; i < n.Nodes.Count; i++)
                {
                    res += GetAllChildrenInNode(n.Nodes[i]);
                }
            }
            else
            {
                res += GetDateInNode(n) + ".children,";
            }
            return res;
        }

        string GetDateInNode(Node n)
        {
            string res = "";

            if (n.Level != 0)
            {
                res = GetDateInNode(n.Parent) + ".[" + DelLastsChar(n.Text, ' ') + "]";
            }
            else
            {

                res = "[" + DelLastsChar(n.Text, ' ') + "]";
            }
            return res;
        }

        string BN = "";
        protected override void Page_Load(object sender, EventArgs e)
        { 
            //RegionSettingsHelper.Instance.SetWorkingRegion("Kostroma");
            base.Page_Load(sender, e);
            
                p4.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                if (!Page.IsPostBack)
                {
                    Dictionary<string, int> d = GenDistonary("LD");
                    Year.FillDictionaryValues(d);
                    Year.Width = 300;
                    Year.ShowSelectedValue = false;
                    Year.MultiSelect = false;
                    Year.ShowSelectedValue = false;
                    Year.ParentSelect = 1 == 1;
                    Year.PanelHeaderTitle = "Выберите период";
                    Year.SetСheckedState(ls, 1 == 1);
                    para.FillDictionaryValues(GenDForRegion("R"));
                    para.SetСheckedState(Lregion, 1 == 1);
                    para.Width = 500;
                    para.Title = "Территория";
                }

                try
                {
                    p2.Value = para.SelectedValue;

                    Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                    Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                }
                catch { }
                    
                    p1.Value = "[Период].[Период].[Год].[" + DelLastsChar(Year.SelectedNode.Parent.Parent.Text,' ') + "].[Полугодие " +

                    CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text, ' '))).ToString() + "].[Квартал " +
                    CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text, ' '))).ToString()
                    + "].[" +
                    DelLastsChar(Year.SelectedNode.Parent.Text, ' ')
                    + "].[" +
                    DelLastsChar(Year.SelectedNode.Text, ' ')
                    + "]";


                    if (!(o_0(Year.SelectedNode) <= 13))
                    {

                        p3.Value = "[Период].[Период].[Год].[" + DelLastsChar( Year.SelectedNode.Parent.Parent.Text,' ') + "].[Полугодие " +

CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Parent.Nodes[0].Text, ' '))).ToString() + "].[Квартал " +
CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Parent.Nodes[0].Text, ' '))).ToString()
+ "].[" +
DelLastsChar(Year.SelectedNode.Parent.Parent.Nodes[0].Text, ' ')
+ "].[" +
DelLastsChar(Year.SelectedNode.Parent.Parent.Nodes[0].Nodes[0].Text, ' ')
+ "],";
                    }
                    else
                    {
                        p3.Value = "";
                    }

                
                    G.DisplayLayout.NoDataMessage = "в настоящий момент данные отсутствуют";
                    G.DataBind();
                    Raschet(G);
                    SetImage(G);
                    try
                    { }
                catch { }
                
                TC.DataBind();
                TBC.DataBind();

                try
                { }
            catch { }

            Label3.Text = string.Format(Label3.Text, p2.Value);
            Label8.Text = string.Format(Label8.Text, p2.Value);

            Page.Title = Hederglobal.Text;

            G.Height = (G.Rows.Count != 0) ? (CRHelper.GetGridHeight(G.Rows.Count * 23.7)) : 100;
            Label1.Text = string.Format("Основные показатели ({0})", p2.Value);
            G.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            G.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;

            try
            {

                if (G.Columns.Count > 15)
                {
                    G.Columns[1].Width = 0;
                    G.Columns[1].Header.Caption = "";
                    for (int i = 3; i < G.Bands[0].HeaderLayout.Count; i++)
                    {
                        if (G.Columns[1].Header.RowLayoutColumnInfo.OriginX == G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX)
                        {
                            G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX++;
                            G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.SpanX--;
                        }
                    }
                    G.Columns[1].Header.Caption = "";

                    for (int i = 0; i < G.Rows.Count; i++)
                    {
                        G.Rows[i].Cells[1].Text = "";
                    }

                }
            }
            catch { }


            //RegionSettingsHelper.Instance.SetWorkingRegion("Gubkinski");
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Bands[0].Columns.Clear();
            G.Bands.Clear();
            G.Rows.Clear();
            DataTable dt = GetDSForChart("G");
            DataTable dt1 = new DataTable();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));

            DataTable dte = GetDSForChart("Ge");
            
            dt1.Columns.Add("Показатель");
            for (int i = 1; i < dt.Columns.Count; i++)
                try
                {
                    dt1.Columns.Add(dt.Columns[i].Caption + ";" + cs.Axes[0].Positions[i - 1].Members[0].UniqueName, dt.Columns[i].DataType);
                }
                catch {
                try
                {
                    dt1.Columns.Add(dt.Columns[i].Caption + ";" + cs.Axes[0].Positions[i - 1].Members[0].UniqueName+" ", dt.Columns[i].DataType);
                }
                catch { }
                
                }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    object[] o = dt.Rows[i].ItemArray;
                    o[0] = o[0].ToString() + ((string.IsNullOrEmpty(dte.Rows[i].ItemArray[1].ToString())) ? "" : ", " + dte.Rows[i].ItemArray[1].ToString().ToLower());
                    dt1.Rows.Add(o);
                    dt1.Rows.Add(new object[dt.Columns.Count]);
                    dt1.Rows.Add(new object[dt.Columns.Count]);
                }
                catch { }

            }

            if (dt.Columns.Count < 3)
            {              
            }
            empty = dt1.Columns.Count <= 1;
            G.DataSource = !empty ? dt1:null;
        }

        protected void G_DataBinding1(object sender, EventArgs e)
        {

        }

        protected void GenHeder()
        {

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            ColumnHeader ch = new ColumnHeader();
            ColumnHeader ch1 = new ColumnHeader();
            string pred = "";
            string pred1 = "";
            int span = 0;
            int span1 = 0;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.### ###");
                try
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 2;
                    e.Layout.Bands[0].Columns[i].Width = 70;
                    try
                    {
                        if (pred != e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1].Split('[', ']')[13])
                        {
                            ch.RowLayoutColumnInfo.SpanX = span + 1;
                            ch = new ColumnHeader();
                            ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1].Split('[', ']')[13];
                            ch.RowLayoutColumnInfo.OriginX = e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginX;
                            ch.RowLayoutColumnInfo.OriginY = 1;
                            pred = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1].Split('[', ']')[13];
                            span = 0;
                            e.Layout.Bands[0].HeaderLayout.Add(ch);
                        }
                        else
                        {
                            span++;
                        }

                        if (pred1 != e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1].Split('[', ']')[7])
                        {
                            ch1.RowLayoutColumnInfo.SpanX = 10;
                            ch1 = new ColumnHeader();
                            ch1.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1].Split('[', ']')[7];
                            ch1.RowLayoutColumnInfo.OriginX = e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginX;
                            ch1.RowLayoutColumnInfo.OriginY = 0;
                            pred1 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1].Split('[', ']')[7];
                            span1 = 0;
                            e.Layout.Bands[0].HeaderLayout.Add(ch1);
                        }
                        else
                        {
                            span1++;
                        }
                    }
                    catch { }
                }
                catch { }
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
            }
            ch.RowLayoutColumnInfo.SpanX = span + 1;
            ch1.RowLayoutColumnInfo.SpanX = span1 + 1;
            e.Layout.Bands[0].Columns[0].Width = 170;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 3;
            G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
        }

        #region
        protected void RashetGrid()
        {

        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {

        }

        protected void CL_DataBinding(object sender, EventArgs e)
        {
        }

        protected void LC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {

        }

        protected void RC_DataBinding(object sender, EventArgs e)
        {

        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {

        }

        protected void BC_DataBinding(object sender, EventArgs e)
        {

        }

        protected void CC_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }
        #endregion

        void Raschet(UltraWebGrid Gr)
        {
            for (int j = 0; j < Gr.Rows.Count - 2; j++)
            {

                if (!string.IsNullOrEmpty(Gr.Rows[j].Cells[0].Text))
                {
                    try
                    {
                        Gr.Rows[j].Cells[1].Value = System.Decimal.Round((System.Decimal)(Gr.Rows[j].Cells[1].Value), 2);
                        Gr.Rows[j].Cells[0].RowSpan = 3;
                        if (Gr.Columns[1].Width == 0)
                        {
                            Gr.Rows[j].Cells[1].Text = "";
                        }
                    }
                    catch { }
                    for (int i = 2; i < Gr.Rows[j].Cells.Count; i++)
                    {
                        try
                        {
                            Gr.Rows[j].Cells[i].Value = System.Decimal.Round((System.Decimal)(Gr.Rows[j].Cells[i].Value), 2);
                            Gr.Rows[j + 1].Cells[i].Value = System.Decimal.Round(((((System.Decimal)(Gr.Rows[j].Cells[i].Value) - (System.Decimal)(Gr.Rows[j].Cells[1].Value)))), 2);
                            Gr.Rows[j + 1].Cells[i].Title = (System.Decimal)(Gr.Rows[j + 1].Cells[i].Value) > 0 ? "Прирост к первой дате" : (System.Decimal)(Gr.Rows[j + 1].Cells[i].Value) < 0 ? "Падение относительно первой даты" : "";

                            Gr.Rows[j + 2].Cells[i].Value = System.Decimal.Round(((((System.Decimal)(Gr.Rows[j].Cells[i].Value) / (System.Decimal)(Gr.Rows[j].Cells[1].Value)) - 1) * 100), 2);
                            Gr.Rows[j + 2].Cells[i].Text =  ((System.Decimal)(Gr.Rows[j + 2].Cells[i].Value)).ToString("### ##0.00");
                            Gr.Rows[j + 2].Cells[i].Text = ((Gr.Rows[j + 2].Cells[i].Text[0] == '-') ? Gr.Rows[j + 2].Cells[i].Text : ((Gr.Rows[j + 2].Cells[i].Text[0] == '0') ? "" : "+") + Gr.Rows[j + 2].Cells[i].Text) + "%";                            
                            Gr.Rows[j + 2].Cells[i].Title = "Темп прироста к первой дате";

                        }
                        catch {Gr.Rows[j + 2].Cells[i].Text = "-";}
                    }
                    try
                    {
                        Gr.Rows[j].Cells[0].Value = System.Decimal.Round((System.Decimal)(Gr.Rows[j].Cells[0].Value), 2);
                    }
                    catch { }
                }
                else
                {
                }
            }
        }
        /// <summary>
        /// бред
        /// </summary>
        /// <param name="Gr"></param>
        void SetImage(UltraWebGrid Gr)
        {
            for (int i = 2; i < Gr.Rows[1].Cells.Count; i++)
            {
                int xz = 1;
                try
                {
                    if ((System.Decimal)(Gr.Rows[1].Cells[i].Value) > 0)
                    {
                        Gr.Rows[0].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenUpBB.png\"> " + "</div>" + Gr.Rows[0].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[1].Cells[i].Value) < 0)
                    {
                        Gr.Rows[0].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedDownBB.png\"> " + "</div>" + Gr.Rows[0].Cells[i];
                    }

                    if ((System.Decimal)(Gr.Rows[4].Cells[i].Value) > 0)
                    {
                        Gr.Rows[3].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedUpBB.png\"> " + "</div>" + Gr.Rows[3].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[4].Cells[i].Value) < 0)
                    {
                        Gr.Rows[3].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenDownBB.png\"> " + "</div>" + Gr.Rows[3].Cells[i];
                    }

                    if ((System.Decimal)(Gr.Rows[7].Cells[i].Value) > 0)
                    {
                        Gr.Rows[6].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedUpBB.png\"> " + "</div>" + Gr.Rows[6].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[7].Cells[i].Value) < 0)
                    {
                        Gr.Rows[6].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenDownBB.png\"> " + "</div>" + Gr.Rows[6].Cells[i];
                    }

                    if ((System.Decimal)(Gr.Rows[10].Cells[i].Value) > 0)
                    {
                        Gr.Rows[9].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedUpBB.png\"> " + "</div>" + Gr.Rows[9].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[10].Cells[i].Value) < 0)
                    {
                        Gr.Rows[9].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenDownBB.png\"> " + "</div>" + Gr.Rows[9].Cells[i];
                    }

                    if ((System.Decimal)(Gr.Rows[13].Cells[i].Value) > 0)
                    {
                        Gr.Rows[12].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenUpBB.png\"> " + "</div>" + Gr.Rows[12].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[13].Cells[i].Value) < 0)
                    {
                        Gr.Rows[12].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedDownBB.png\"> " + "</div>" + Gr.Rows[12].Cells[i];
                    }

                    if ((System.Decimal)(Gr.Rows[16].Cells[i].Value) > 0)
                    {
                        Gr.Rows[15].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedUpBB.png\"> " + "</div>" + Gr.Rows[15].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[16].Cells[i].Value) < 0)
                    {
                        Gr.Rows[15].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenDownBB.png\"> " + "</div>" + Gr.Rows[15].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[18].Cells[i].Value) < 0)
                    {
                        Gr.Rows[18].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img Title=\"Дефицит\" src=\"../../../../../images/ballRedBB.png\"> " + "</div>" + Gr.Rows[18].Cells[i];
                        Gr.Rows[18].Cells[i].Title = "Дефицит";    
                    }
                    if ((System.Decimal)(Gr.Rows[18].Cells[i].Value) > 0)
                    {
                        Gr.Rows[18].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img Title=\"Профицит\" src=\"../../../../../images/ballGreenBB.png\"> " + "</div>" + Gr.Rows[18].Cells[i];
                        Gr.Rows[18].Cells[i].Title = "Профицит";
                    }
                    if ((System.Decimal)(Gr.Rows[18].Cells[i].Value) == 0)
                    {
                        Gr.Rows[18].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/ballGreenBB.png\"> " + "</div>" + Gr.Rows[18].Cells[i];                        
                    }                    
                }
                catch { }

            }
        }

        void SetImage2(UltraWebGrid Gr)
        {
            for (int i = 2; i < Gr.Rows[1].Cells.Count; i++)
            {
                try
                {
                    if ((System.Decimal)(Gr.Rows[1].Cells[i].Value) > 0)
                    {
                        Gr.Rows[0].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedUpBB.png\"> " + "</div>" + Gr.Rows[0].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[1].Cells[i].Value) < 0)
                    {
                        Gr.Rows[0].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenDownBB.png\"> " + "</div>" + Gr.Rows[0].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[4].Cells[i].Value) > 0)
                    {
                        Gr.Rows[3].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowRedUpBB.png\"> " + "</div>" + Gr.Rows[3].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[4].Cells[i].Value) < 0)
                    {
                        Gr.Rows[3].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/arrowGreenDownBB.png\"> " + "</div>" + Gr.Rows[3].Cells[i];
                    }
                }
                catch { }
            }
           




        }

        protected void G_InitializeRow1(object sender, RowEventArgs e)
        {


        }


        System.Decimal max, min = 0;

        DataTable GetDtDate(string q)
        {

            DataTable dt = GetDSForChart(q);

            DataTable dt1 = new DataTable();
            try
            {
                dt1.Columns.Add(dt.Columns[0].Caption, dt.Columns[0].DataType);

                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(q));

                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    string mounth = CRHelper.MonthNum(cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[13]).ToString();
                    string year = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[7];
                    string day = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[15];
                    dt1.Columns.Add(((day.Length < 2) ? "0" + day : day) +
                        "." + ((mounth.Length < 2) ? "0" + mounth : mounth) +
                        "." + ((year.Length < 2) ? "0" + year : year), dt.Columns[i].DataType);
                }
                {
                    max = (System.Decimal)(dt.Rows[0].ItemArray[1]);
                    min = max;
                }
                
                if (dt.Rows.Count < 2)
                {
                    return null;
                }
                for (int i = (dt.Rows.Count - 52) <= 0 ? 0 : dt.Rows.Count - 52; i < dt.Rows.Count; i++)
                {
                    object[] o = dt.Rows[i].ItemArray;
                    try
                    {
                        for (int j = 1; j < o.Length; j++)
                        {
                            if ((System.Decimal)(o[j]) > max)
                            {
                                max = (System.Decimal)(o[j]);
                            }
                            if ((System.Decimal)(o[j]) < min)
                            {
                                min = (System.Decimal)(o[j]);
                            }
                        }
                    }
                    catch { }

                    dt1.Rows.Add(o);
                }
            }
            catch { }

            return dt1;
        }


        protected void TC_DataBinding(object sender, EventArgs e)
        {

            TC.DataSource = GetDtDate("TC");
            TC.Axis.Y.RangeType = AxisRangeType.Custom;
            TC.Axis.Y.RangeMax = Convert.ToDouble(max) * 1.1;
            TC.Axis.Y.RangeMin = Convert.ToDouble(min) * 0.9;
            TC.Tooltips.FormatString = "Уровень зарегистрированной безработицы на <ITEM_LABEL><br><DATA_VALUE:### ###.00> процентов";

            TC.ColorModel.ModelStyle = ColorModels.CustomSkin;

            TC.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Red;
                            
                            break;
                        }
                    case 2:
                        {
                            color = Color.Blue;
                            break;
                        }
                }
                pe.Fill = color;
                TC.ColorModel.Skin.PEs.Add(pe);
            }


        }

        protected void TBC_DataBinding(object sender, EventArgs e)
        {

            TBC.DataSource = GetDtDate("TBC");
            TBC.Axis.Y.RangeType = AxisRangeType.Custom;
            TBC.Axis.Y.RangeMax = Convert.ToDouble(max) * 1.1;
            TBC.Axis.Y.RangeMin = Convert.ToDouble(min) * 0.9;
            TBC.Tooltips.FormatString = "Коэффициент напряженности на <ITEM_LABEL><br><DATA_VALUE:### ###.00> человек на одну вакансию";
            TBC.ColorModel.ModelStyle = ColorModels.CustomSkin;

            TBC.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Red;

                            break;
                        }
                    case 2:
                        {
                            color = Color.Blue;
                            break;
                        }
                }
                pe.Fill = color;
                TBC.ColorModel.Skin.PEs.Add(pe);
            }
        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("GB");
            DataTable dt1 = new DataTable();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GB"));
            dt1.Columns.Add("Показатель");
            DataTable dte = GetDSForChart("GBe");
            for (int i = 1; i < dt.Columns.Count; i++)
                try
                {
                    dt1.Columns.Add(dt.Columns[i].Caption + ";" + cs.Axes[0].Positions[i - 1].Members[0].UniqueName, dt.Columns[i].DataType);
                }
                catch { }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    object[] o = dt.Rows[i].ItemArray;
                    try
                    {
                        o[0] = o[0].ToString() + ((string.IsNullOrEmpty(dte.Rows[i].ItemArray[1].ToString())) ? "" : ", " + dte.Rows[i].ItemArray[1].ToString().ToLower());
                    }
                    catch { }
                    dt1.Rows.Add(o);
                    dt1.Rows.Add(new object[dt.Columns.Count]);
                    dt1.Rows.Add(new object[dt.Columns.Count]);
                }
                catch { }
            }
        }

        protected void BC_DataBinding1(object sender, EventArgs e)
        {

        }

        void CompoChart(Infragistics.WebUI.UltraWebChart.UltraChart C)
        {
            C.ChartType = ChartType.Composite;
            ChartArea Area = new ChartArea();
            CompositeLegend lega = new CompositeLegend();
            lega.Bounds = new Rectangle(0, 93, 100, 7);
            lega.BoundsMeasureType = MeasureType.Percentage;
            lega.PE.ElementType = PaintElementType.CustomBrush;
            lega.PE.Fill = C.Legend.BackgroundColor;
            lega.LabelStyle.Font = C.Legend.Font;
            lega.LabelStyle.FontSizeBestFit = !(lega.LabelStyle.FontSizeBestFit);

            AxisItem Ax = new AxisItem();
            Ax.DataType = AxisDataType.String;
            Ax.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            Ax.OrientationType = AxisNumber.X_Axis;
            Ax.Labels.Visible = 1 == 1;
            Ax.Labels.ItemFormatString = "<ITEM_LABEL>";
            Ax.Extent = 30;
            Ax.Margin.Far.Value = 5;
            Ax.Margin.Near.Value = 5;

            AxisItem Ay = new AxisItem();

            Ay.Extent = 40;
            Ay.DataType = AxisDataType.Numeric;

            Ay.Labels.ItemFormatString = "<DATA_VALUE:00.0>";
            Ay.Labels.Visible = 1 == 1;
            Ay.Labels.HorizontalAlign = StringAlignment.Far;
            Ay.Labels.Layout.Padding = 5;
            Ay.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            Ay.OrientationType = AxisNumber.Y_Axis;

            AxisItem Ay1 = new AxisItem();
            Ay1.Extent = 40;
            Ay1.DataType = AxisDataType.Numeric;

            Ay1.Labels.ItemFormatString = "<DATA_VALUE:00.0>";
            Ay1.Labels.Visible = 1 == 1;
            Ay1.Labels.Layout.Padding = 5;
            Ay1.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            Ay1.OrientationType = AxisNumber.Y2_Axis;

            Ay.Margin.Far.Value = 10;
            Ay1.Margin.Far.Value = 10;

            Area.Axes.Add(Ax);
            Area.Axes.Add(Ay);
            Area.Axes.Add(Ay1);

            NumericSeries seriesC = GetXYSeriesBound("BC0");
            seriesC.Label = "Сумма задолженности";

            C.CompositeChart.Series.Add(seriesC);

            ChartLayerAppearance cla = new ChartLayerAppearance();
            SplineAreaChartAppearance SACA = new SplineAreaChartAppearance();
            ChartTextAppearance cta = new ChartTextAppearance();
            cta.Column = -2;
            cta.Row = -2;
            cta.Visible = 1 == 1;
            cta.VerticalAlign = StringAlignment.Far;
            SACA.ChartText.Add(cta);

            cla.ChartTypeAppearance = SACA;

            cla.ChartType = ChartType.SplineAreaChart;
            cla.ChartArea = Area;

            cla.AxisX = Ax;
            cla.AxisY = Ay;

            Ay.RangeMin = System.Decimal.ToDouble(Lmin);
            Ay.RangeMax = System.Decimal.ToDouble(Lmax);

            cla.Series.Add(seriesC);

            lega.ChartLayers.Add(cla);
            C.CompositeChart.ChartLayers.Add(cla);

            seriesC = GetXYSeriesBound("BC1");
            seriesC.Label = "Кол-во граждан, перед которыми имеется задолженность";

            C.CompositeChart.Series.Add(seriesC);

            cla = new ChartLayerAppearance();
            LineChartAppearance SACA0 = new LineChartAppearance();
            cta = new ChartTextAppearance();
            cta.Column = -2;
            cta.Row = -2;
            cta.Visible = 1 == 1;
            cta.VerticalAlign = StringAlignment.Far;
            SACA0.ChartText.Add(cta);
            SACA0.DrawStyle = LineDrawStyle.Solid;
            SACA0.Thickness = 8;

            cla.ChartTypeAppearance = SACA0;

            cla.ChartType = ChartType.LineChart;
            cla.ChartArea = Area;

            cla.AxisX = Ax;
            cla.AxisY = Ay1;

            Ay.RangeType = AxisRangeType.Custom;
            Ay1.RangeType = AxisRangeType.Custom;
            Ay1.RangeMin = System.Decimal.ToDouble(Lmin);
            Ay1.RangeMax = System.Decimal.ToDouble(Lmax);

            cla.Series.Add(seriesC);

            C.CompositeChart.ChartLayers.Add(cla);

            lega.ChartLayers.Add(cla);

            C.CompositeChart.Legends.Add(lega);

            C.CompositeChart.ChartAreas.Add(Area);
            C.Tooltips.FormatString = "<SERIES_LABEL>, <b><DATA_VALUE:00.##></b>";

            C.Border.Color = Color.Transparent;
            Area.Border.Color = Color.Transparent;

        }


        private NumericSeries GetXYSeriesBound(string xz)
        {
            NumericSeries series = new NumericSeries();
            if (xz == "BC0")
            { series.Label = "Сумма задолженности по выплате заработной платы"; }
            else
            { series.Label = "Кол-во граждан, перед которыми имеется задолженность"; }
            DataTable table = GetData(xz);
            series.Data.DataSource = table;
            series.Data.LabelColumn = "Label Column";
            series.Data.ValueColumn = "Value Column";
            return series;
        }

        System.Decimal Lmax = 0;
        System.Decimal Lmin = 0;

        private DataTable GetData(string xz)
        {
            Lmax = 0;
            Lmin = 0;
            DataTable table = new DataTable();
            table.Columns.Add("Label Column", typeof(string));
            table.Columns.Add("Value Column", typeof(double));
            DataTable dt1 = GetDSForChart(xz);

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(xz));

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                table.Rows.Add(new object[] { 
                    cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + "." + CRHelper.MonthNum(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13]).ToString() + "." + cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7]
                    , dt1.Rows[i].ItemArray[1] 
                });
                Lmax = Lmax < (System.Decimal)(dt1.Rows[i].ItemArray[1]) ? (System.Decimal)(dt1.Rows[i].ItemArray[1]) : Lmax;
                Lmin = Lmin > (System.Decimal)(dt1.Rows[i].ItemArray[1]) ? (System.Decimal)(dt1.Rows[i].ItemArray[1]) : Lmin;
            }
            if (dt1.Rows.Count < 2)
            {
                empty = 1 == 1;
            };
            return table;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        }

        int ExportGrid(UltraWebGrid G, EndExportEventArgs e, int offset)
        {
            for (int i = 0; i < G.Bands[0].HeaderLayout.Count; i++)
            {
                e.Workbook.Worksheets[0].Rows[offset + G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginY].Cells[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX].Value = G.Bands[0].HeaderLayout[i].Caption;
            }
            offset += 3;

            for (int j = 0; j < G.Columns.Count; j++)
            {
                e.CurrentWorksheet.Columns[j].Width = string.IsNullOrEmpty(G.Columns[j].Header.Caption) ? 0 : 100 * 80;
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    try
                    {
                        e.CurrentWorksheet.Rows[i + offset].Cells[j].Value = G.Rows[i].Cells[j].Text.Split('>')[G.Rows[i].Cells[j].Text.Split('>').Length - 1];
                    }
                    catch
                    {
                        e.CurrentWorksheet.Rows[i + offset].Cells[j].Value = G.Rows[i].Cells[j].Text;
                    }
                    try
                    {
                        e.CurrentWorksheet.Rows[i + offset].Cells[j].CellFormat.SetFormatting(e.CurrentWorksheet.Rows[3].Cells[j].CellFormat);
                    }
                    catch { }
                }
            }
            return offset + G.Rows.Count;
        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {

            for (int j = 0; j < G.Columns.Count; j++)
            {

                e.CurrentWorksheet.Rows[0].Cells[j].Value = null;
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    e.CurrentWorksheet.Rows[i + 1].Cells[j].Value = null;



                }
            } ExportGrid(G, e, 0);
            e.CurrentWorksheet.Columns[0].Width = 100 * 200;            
            if (G.Columns[1].Width == 0)
            {
                e.CurrentWorksheet.Columns[1].Width = 0;            
                //e.CurrentWorksheet.Rows[0].Cells[2].Value = e.CurrentWorksheet.Rows[0].Cells[1].Value;
            }
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = G.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = e.HeaderText.Replace("&quot;", "\"");
            if (col.Hidden)
            {
                offset++;
            }
        }




        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }

        protected void para_Load(object sender, EventArgs e)
        {

        }

        protected void TC_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void TBC_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        bool empty =true;
        protected void TBC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (!empty)
            {
                Text textLabel = new Text();
                textLabel.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
                textLabel.PE.Fill = Color.Black;
                textLabel.labelStyle.Font = new Font("Arial", 9);
                textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
                textLabel.labelStyle.VerticalAlign = StringAlignment.Near;
                textLabel.bounds = new Rectangle(0, 0, 300, 280);
                textLabel.SetTextString((sender == TC)
                    ? "                            Уровень зарегистрированной\n                                 безработицы, процент"
                    : "             Коэффициент напряженности на рынке \n                   труда, человек на одну вакансию");
                e.SceneGraph.Add(textLabel);
            }
        }
    }
}
