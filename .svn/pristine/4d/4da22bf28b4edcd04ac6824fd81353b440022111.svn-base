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

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001._0028._03
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

            for (; O_O != Q_Q;O_O = O_O.PrevNode){O_o++;}

            //папа
            Node V_V = v_v.Parent;

            //дедушка оп отцовской линии
            Node W_W = V_V.Parent;

            //дяди
            Nodes v_V__V_V__V_v = W_W.Nodes;

            for (int _ = 0;_<V_V.Index;_++)
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
            d.Add(RegionSettingsHelper.Instance.GetPropertyValue("ShortName"), 0);
            Lregion = RegionSettingsHelper.Instance.GetPropertyValue("ShortName");
            int i = 1;
            for (; i < cs.Axes[0].Positions.Count; i++)
            {
                d.Add(cs.Axes[0].Positions[i].Members[0].Caption, 0);
                //Lregion = cs.Axes[0].Positions[0].Members[0].Caption;
            }
            
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
                    CRHelper.SaveToErrorLog("-" + ls + "-");
                }

            }


            string subS = "";

            ls2 = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName;
            //ls = //cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName.Split('[', ']')[13];

            return d;
        }
        string ls2 = "";

        #endregion

        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            string ly = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            string lm = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];
                        
            d.Add(ly, 0);
            d.Add(lm, 1);
            d.Add(cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[15], 2);
            al.Add(cs.Axes[1].Positions[0].Members[0].UniqueName);
            string subS = "";

            //красота

            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                //try                {

                    if (ly != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                    {
                        ly = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];                        
                        AID(d, ly, 0);
                        lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                        
                        AID(d, lm, 1);
                        ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);                        

                        al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                    }
                    else
                        if (lm != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                        {
                            lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                            AID(d, lm, 1);
                            ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);                        
                            al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                        }
                        else
                        {
                            ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);                        
                        }
                //}                catch { }

            }


            //ls2 = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName;            

            return d;
        }        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            BG.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);         
            BC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);            
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);         
            
            BC.Height = 400;
            UltraChart1.Height = 400;
            ////###########################################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //############################################################################
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

        string BN;
        protected override void Page_Load(object sender, EventArgs e)
        {
            
            //RegionSettingsHelper.Instance.SetWorkingRegion("Kostroma");
            base.Page_Load(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;            
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();            
            try            {
            p4.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                if (!Page.IsPostBack)
                {
                    Dictionary<string, int> d = GenDistonary("LD");
                    Year.FillDictionaryValues(d);
                    Year.Width = 300;
                    Year.ParentSelect = 1 == 1;
                    Year.MultiSelect = false;
                    Year.ShowSelectedValue = false;
                    Year.PanelHeaderTitle = "Выберите период";
                    Year.SetСheckedState(ls, 1 == 1);

                    para.FillDictionaryValues(GenDForRegion("R"));
                    para.SetСheckedState(Lregion, 1 == 1);
                    para.Width = 500;
                    para.Title = "Территория";
                }
                try
                {

                    if (para.SelectedValue == RegionSettingsHelper.Instance.GetPropertyValue("ShortName"))
                    {
                        p2.Value = "datamember";
                    }
                    else
                    {
                        p2.Value = "["+para.SelectedValue+"]";
                    }
                    
                    Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                    Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                }
                catch { }


                p1.Value = "[Период].[Период].[Год].[" + Year.SelectedNode.Parent.Parent.Text + "].[Полугодие " +

                    CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text, ' '))).ToString() + "].[Квартал " +
                    CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text, ' '))).ToString()
                    + "].[" +
                    DelLastsChar(Year.SelectedNode.Parent.Text, ' ')
                    + "].[" +
                    DelLastsChar(Year.SelectedNode.Text, ' ')
                    + "]";

                if (!(o_0(Year.SelectedNode) <= 13))
                {

                    p3.Value = "[Период].[Период].[Год].[" + Year.SelectedNode.Parent.Parent.Text + "].[Полугодие " +

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



                //p2.Value = para.SelectedValue;
                if (para.SelectedValue == RegionSettingsHelper.Instance.GetPropertyValue("ShortName"))
                {
                    p2.Value = "datamember";
                }
                else
                {
                    p2.Value = "[" + para.SelectedValue + "]";
                }
                try
                {
                    BG.DataBind();
                    Raschet(BG);
                    SetImage2(BG);                   
                }
                catch { }
            

            Label3.Text = string.Format("Основные показатели ({0})", para.SelectedValue);
            Label8.Text = string.Format("Динамика количества граждан, перед которыми имеется задолженность ({0})", para.SelectedValue);
            Label1.Text = string.Format("Динамика суммы задолженности по выплате заработной платы ({0})", para.SelectedValue);

            Page.Title = Hederglobal.Text;

            BC.DataBind();
            UltraChart1.DataBind();

            double coef = 1;
            if (BN == "IE")
            {
                coef = 1;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                coef = 1;
            }
            if (BN == "FIREFOX")
            {
                coef = 1.3;
            }
            try
            {
                BG.Height = CRHelper.GetGridHeight((BG.Rows.Count + 3) * 18 * coef);
                BG.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                BG.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
                BG.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            }
            catch { }
        }        catch { } 
            }

        protected void G_DataBinding(object sender, EventArgs e)
        {
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
            e.Layout.Bands[0].Columns[0].Width = 190;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 3;
            if (BG.Columns.Count > 15)
            {
                BG.Columns[1].Width = 0;

                for (int i = 3; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                {
                    if (BG.Columns[1].Header.RowLayoutColumnInfo.OriginX == e.Layout.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX)
                    {
                        e.Layout.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX++;
                        e.Layout.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.SpanX--;
                    }
                }

            }
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
                    }
                    catch { }
                    for (int i = 2; i < Gr.Rows[j].Cells.Count; i++)
                    {

                        System.Decimal pred = 0;

                        for (int k = i - 1; k > 0; k--)
                        {

                            try
                            {
                                pred = (System.Decimal)(Gr.Rows[j].Cells[k].Value);

                            }
                            catch { }
                            break;

                        }

                        try
                        {
                            Gr.Rows[j].Cells[i].Value = System.Decimal.Round((System.Decimal)(Gr.Rows[j].Cells[i].Value), 2);
                            Gr.Rows[j + 1].Cells[i].Value = System.Decimal.Round(((((System.Decimal)(Gr.Rows[j].Cells[i].Value) - (System.Decimal)(Gr.Rows[j].Cells[i-1].Value)))), 2);
                            Gr.Rows[j + 1].Cells[i].Title = (System.Decimal)(Gr.Rows[j + 1].Cells[i].Value) > 0 ? "Прирост к первой дате" : (System.Decimal)(Gr.Rows[j + 1].Cells[i].Value) < 0 ? "Падение относительно первой даты" : "";

                            Gr.Rows[j + 2].Cells[i].Value = System.Decimal.Round(((((System.Decimal)(Gr.Rows[j].Cells[i].Value) / (System.Decimal)(Gr.Rows[j].Cells[i-1].Value)) - 1) * 100), 2).ToString("### ##0.00");

                            Gr.Rows[j + 2].Cells[i].Text = ((Gr.Rows[j + 2].Cells[i].Text[0] == '-') ? Gr.Rows[j + 2].Cells[i].Text : ((Gr.Rows[j + 2].Cells[i].Text[0] == '0') ? "" : "+") + Gr.Rows[j + 2].Cells[i].Text) + "%";
                        }
                        catch
                        {

                        }
                        Gr.Rows[j + 2].Cells[i].Title = "Темп прироста к первой дате";
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
                        Gr.Rows[18].Cells[i].Text = "<div style=\"FLOAT: left;\">" + "<img src=\"../../../../../images/ballRedBB.png\"> " + "</div>" + Gr.Rows[18].Cells[i];
                    }
                    if ((System.Decimal)(Gr.Rows[18].Cells[i].Value) > 0)
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
            if (int.Parse(BG.Columns[1].Header.Caption) > int.Parse(BG.Columns[2].Header.Caption))
            {
                BG.Columns[1].Header.Caption
                    = "";
                for (int i = 0; i < BG.Rows.Count; i++)
                {
                    BG.Rows[i].Cells[1].Text = "";
                }
            }
        }

        protected void G_InitializeRow1(object sender, RowEventArgs e)
        {}

        DataTable GetDtDate(string q)
        {

            DataTable dt = GetDSForChart(q);

            DataTable dt1 = new DataTable();
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

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt1.Rows.Add(dt.Rows[i].ItemArray);
            }

            return dt1;
        }


        protected void TC_DataBinding(object sender, EventArgs e)
        {
        }

        protected void TBC_DataBinding(object sender, EventArgs e)
        {
        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            BG.Bands.Clear(); 
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
            BG.DataSource = dt1;
        }

        protected void BC_DataBinding1(object sender, EventArgs e)
        {
            BC.ColorModel.ModelStyle = ColorModels.CustomSkin;
            BC.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Transparent;
                            stopColor = Color.Indigo;
                            peType = PaintElementType.Gradient;                            
                            break;
                        }
                    case 2:
                        {
                            color = Color.MediumSeaGreen;
                            stopColor = Color.Indigo;
                            peType = PaintElementType.Gradient;
                            break;
                        }
                }

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = i == 1 ? (byte)250 : (byte)250;
                pe.FillStopOpacity = i == 1 ? (byte)250 : (byte)250;
                BC.ColorModel.Skin.PEs.Add(pe);
            }


            CompoChart(BC,2);
             
        }

        void CompoChart(Infragistics.WebUI.UltraWebChart.UltraChart C,int row)
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
            lega.Visible = 1 == 2;
            AxisItem Ax = new AxisItem();
            Ax.DataType = AxisDataType.String;
            Ax.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            Ax.OrientationType = AxisNumber.X_Axis;
            Ax.Labels.Visible = 1 == 1;
            Ax.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            Ax.Labels.ItemFormatString = "<ITEM_LABEL>";
            Ax.Labels.Font = new Font("Arial", 8);
            Ax.Labels.FontColor = Color.Gray;
            Ax.Extent = 85;
            Ax.Margin.Far.Value = 0;
            Ax.Margin.Near.Value = 0;
            Ax.LineThickness = 1;

            AxisItem Ay = new AxisItem();
            Ay.Extent = 0;
            Ay.DataType = AxisDataType.Numeric;
            Ay.Labels.ItemFormatString = C == UltraChart1 ? "<DATA_VALUE:### ### #00>" : "<DATA_VALUE:00.##>";
            Ay.Labels.HorizontalAlign = StringAlignment.Far;
            Ay.Labels.Layout.Padding = 0;
            Ay.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            Ay.LineThickness = 1;
            Ay.OrientationType = AxisNumber.Y2_Axis;
            Ay.Margin.Far.Value = 0;

            AxisItem Ay1 = new AxisItem();
            Ay1.Extent =80;
            Ay1.DataType = AxisDataType.Numeric;
            Ay1.Labels.ItemFormatString = C != UltraChart1 ? "<DATA_VALUE:### ### #00>" : "<DATA_VALUE:00.##>";
            Ay1.Labels.HorizontalAlign = StringAlignment.Far;
            Ay1.Labels.Layout.Padding = 0;
            Ay1.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            Ay1.LineThickness = 1;
            Ay1.OrientationType = AxisNumber.Y_Axis;
            Ay1.Margin.Far.Value = 0;
            Ay1.Visible = 1 == 1;
            Ay.Visible = 1 == 2;

            Area.Axes.Add(Ax);
            Area.Axes.Add(Ay);
            Area.Axes.Add(Ay1);

            NumericSeries seriesC = GetXYSeriesBound("BC0", row);

            Ay.RangeType = AxisRangeType.Custom;
            Ay.RangeMin = Math.Round(System.Decimal.ToDouble(Lmin) * 0.0);
            Ay.RangeMax = System.Decimal.ToDouble(Lmax) *1.1;

            seriesC.Label = C==UltraChart1 ? "Сумма задолженности по выплате заработной платы, тысяча рублей":"Кол-во граждан, перед которыми имеется задолженность, человек" ;

            C.CompositeChart.Series.Add(seriesC);

            ChartLayerAppearance cla = new ChartLayerAppearance();
            AreaChartAppearance SACA = new AreaChartAppearance();
            ChartTextAppearance cta = new ChartTextAppearance();
            cta.Column = -2;
            cta.Row = -2;
            cta.Visible = 1 == 2;
            cta.VerticalAlign = StringAlignment.Far;
            SACA.ChartText.Add(cta);
            cla.ChartTypeAppearance = SACA;
            cla.ChartType = ChartType.AreaChart;
            cla.ChartArea = Area;            
            cla.AxisX = Ax;
            cla.AxisY = Ay;
            cla.Series.Add(seriesC);
            
            SACA.NullHandling = NullHandling.DontPlot;
            SACA.LineAppearances.Clear();
            SACA.LineAppearances.Add(new LineAppearance());
            SACA.LineAppearances[0].IconAppearance.Icon = SymbolIcon.Circle;
            SACA.LineAppearances[0].IconAppearance.IconSize = SymbolIconSize.Small;
            SACA.LineAppearances[0].Thickness = 5;
            
            lega.ChartLayers.Add(cla);
            C.CompositeChart.ChartLayers.Add(cla);

            seriesC = GetXYSeriesBound("BC0", row);
            seriesC.Label = "Кол-во граждан, перед которыми имеется задолженность, человек";
            seriesC.PEs.Add(new PaintElement(Color.Blue));

            C.CompositeChart.Series.Add(seriesC);
            cla = new ChartLayerAppearance();
            LineChartAppearance SACA0 = new LineChartAppearance();

            cta = new ChartTextAppearance();
            cta.Column = -2;
            cta.Row = -2;
            cta.Visible = 1 == 1;
            cta.VerticalAlign = StringAlignment.Far;
            SACA0.NullHandling = NullHandling.DontPlot;
            SACA0.ChartText.Add(cta);
            SACA0.DrawStyle = LineDrawStyle.Solid;
            SACA0.Thickness = 1;

            cla.ChartTypeAppearance = SACA0;

            cla.ChartType = ChartType.LineChart;
            
            cla.ChartArea = Area;

            cla.AxisX = Ax;
            cla.AxisY = Ay1;

            Ay1.RangeType = AxisRangeType.Custom;
            Ay1.RangeMin = Math.Round(System.Decimal.ToDouble(Lmin) * 0.0);
            Ay1.RangeMax = System.Decimal.ToDouble(Lmax) *1.05;
            
            C.CompositeChart.Legends.Add(lega);            
            
            C.CompositeChart.ChartAreas.Add(Area);
            C.Tooltips.FormatString = "<SERIES_LABEL> <!--<DATA_VALUE:### ##0.##> <ITEM_LABEL>-->";

            C.Border.Color = Color.Transparent;
            Area.Border.Color = Color.Transparent;

            C.TitleLeft.Text = "Число безработных,\n чел/на 1 вакансию";
            C.TitleLeft.HorizontalAlign = StringAlignment.Center;
            C.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            C.TitleLeft.Extent = 0;
            C.TitleLeft.Margins.Bottom = 5;
            C.TitleLeft.Font = new Font("Verdana", 10);
            C.TitleLeft.FontColor = Color.Black;
        }

        DataTable dt1 = new DataTable();
        private NumericSeries GetXYSeriesBound(string xz, int zx)
        {            
            NumericSeries series = new NumericSeries();
            if (xz == "BC0")
            { series.Label = "<ITEM_VALUE>, Сумма задолженности по выплате заработной платы, тысяча рублей"; }
            else
            {
                series.Label = "<ITEM_VALUE>, Кол-во граждан, перед которыми имеется задолженность, человек";
            }
            DataTable table = GetData(xz, zx);
            dt1 = GetDSForChart(xz);

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(xz));

            Lmax = (System.Decimal)(dt1.Rows[0].ItemArray[zx]);
            Lmin = Lmax;
            for (int i = (dt1.Rows.Count - 52) <= 0 ? 0 : dt1.Rows.Count - 52; i < dt1.Rows.Count; i++)
            {
                string mounth = CRHelper.MonthNum(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13]).ToString();
                string year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                string day = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15];
                string da = ((day.Length < 2) ? "0" + day : day) +
                    "." + ((mounth.Length < 2) ? "0" + mounth : mounth) +
                    "." + ((year.Length < 2) ? "0" + year : year);
                try
                {

                    series.Points.Add(new NumericDataPoint(Convert.ToDouble((System.Decimal)(dt1.Rows[i].ItemArray[zx])), da, 1 == 2));
                }
                catch
                {
                    try
                    {
                        series.Points.Add(new NumericDataPoint(double.NaN, da, 1 == 1));
                    }
                    catch { }
                }

                try
                {
                    Lmax = Lmax < (System.Decimal)(dt1.Rows[i].ItemArray[zx]) ? (System.Decimal)(dt1.Rows[i].ItemArray[zx]) : Lmax;
                    Lmin = Lmin > (System.Decimal)(dt1.Rows[i].ItemArray[zx]) ? (System.Decimal)(dt1.Rows[i].ItemArray[zx]) : Lmin;
                }
                catch { }
            }
            return series;
        }

        System.Decimal Lmax = 0;
        System.Decimal Lmin = 0;

        private DataTable GetData(string xz, int zx)
        {
            Lmax = 0;
            Lmin = 0;
            DataTable table = new DataTable();
            table.Columns.Add("Label Column", typeof(string));
            table.Columns.Add("Value Column", typeof(double));
            DataTable dt1 = GetDSForChart(xz);
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(xz));
            Lmax = (System.Decimal)(dt1.Rows[0].ItemArray[zx]);
            Lmin = Lmax;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string mounth = CRHelper.MonthNum(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13]).ToString();
                string year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                string day = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15];
                string da = ((day.Length < 2) ? "0" + day : day) +
                    "." + ((mounth.Length < 2) ? "0" + mounth : mounth) +
                    "." + ((year.Length < 2) ? "0" + year : year);
                table.Rows.Add(new object[] { da, dt1.Rows[i].ItemArray[zx] });
                try
                {
                    Lmax = Lmax < (System.Decimal)(dt1.Rows[i].ItemArray[zx]) ? (System.Decimal)(dt1.Rows[i].ItemArray[zx]) : Lmax;
                    Lmin = Lmin > (System.Decimal)(dt1.Rows[i].ItemArray[zx]) ? (System.Decimal)(dt1.Rows[i].ItemArray[zx]) : Lmin;
                }
                catch { }
            }
            return table;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        }

        int ExportGrid(UltraWebGrid G, EndExportEventArgs e, int offset)
        {
            for (int i = 0; i < BG.Bands[0].HeaderLayout.Count; i++)
            {
                e.Workbook.Worksheets[0].Rows[offset + BG.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginY].Cells[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX].Value = BG.Bands[0].HeaderLayout[i].Caption;
            }
            offset += 3;

            if (string.IsNullOrEmpty(G.Columns[1].Header.Caption))
            {
                e.CurrentWorksheet.Rows[0].Cells[2].Value = "2010";
            }

            for (int j = 0; j < BG.Columns.Count; j++)
            {
                e.CurrentWorksheet.Columns[j].Width = string.IsNullOrEmpty(BG.Columns[j].Header.Caption) ? 0 : 100 * 40;                
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    try
                    {
                        e.CurrentWorksheet.Rows[i + offset].Cells[j].Value = BG.Rows[i].Cells[j].Text.Split('>')[BG.Rows[i].Cells[j].Text.Split('>').Length - 1];
                    }
                    catch
                    {
                        e.CurrentWorksheet.Rows[i + offset].Cells[j].Value = BG.Rows[i].Cells[j].Text;
                    }
                    try
                    {
                        e.CurrentWorksheet.Rows[i + offset].Cells[j].CellFormat.SetFormatting(e.CurrentWorksheet.Rows[3].Cells[j].CellFormat);
                    }
                    catch { }
                }
            }
            e.CurrentWorksheet.Columns[0].Width = 100 * 80;            
            return offset + G.Rows.Count;
        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[2].Cells[0].Value = "Показатель";

            for (int j = 0; j < BG.Columns.Count; j++)
            {
                e.CurrentWorksheet.Rows[0].Cells[j].Value = null;
                for (int i = 0; i < BG.Rows.Count; i++)
                {
                    e.CurrentWorksheet.Rows[i + 1].Cells[j].Value = null;




                }
            } ExportGrid(BG, e, 0);
            e.CurrentWorksheet.Columns[0].Width = 150 * 100;
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
        }
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(BG);
        }

        protected void TBC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        { 
            //
        }

        protected void BC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if ((primitive is Polygon)&&(sender != BC))
                {
                    Polygon box = (Polygon)primitive;
                    if (box.Series != null)
                    {

                        box.Series.Label = "Сумма задолженности по выплате заработной платы на <ITEM_LABEL><br> <b><DATA_VALUE:### ##0.##></b>, тысяча рублей";
                        try
                        {

                        }
                        catch { }
                    }
                }else
                if (primitive is Polygon)
                {
                    Polygon box = (Polygon)primitive;
                    if (box.Series != null)
                    {
                        box.Series.Label = "Кол-во граждан, перед которыми имеется задолженность на <ITEM_LABEL><br> <b><DATA_VALUE:### ##0.##></b>, человек";                        
                        try
                        {

                        }
                        catch { }
                    }

                }
            }
            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            textLabel.PE.Fill = Color.Black;
            textLabel.labelStyle.Font = new Font("Arial", 9);
            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;
            textLabel.bounds = new Rectangle(20, 0, 300, 280);
            textLabel.SetTextString((sender != BC) ? "        Cумма задолженности по выплате\n        заработной платы, тысяча рублей" : "               Количество граждан, перед\n которыми имеется задолженность, человек");
            e.SceneGraph.Add(textLabel);
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Transparent;
                            stopColor = Color.Red;
                            peType = PaintElementType.Gradient;                            
                            break;
                        }
                    case 2:
                        {
                            color = Color.MediumSeaGreen;
                            stopColor = Color.Tomato;
                            peType = PaintElementType.Gradient;
                            break;
                        }
                }
            
            pe.Fill = color;
            pe.FillStopColor = stopColor;
            pe.ElementType = peType;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = i == 1 ? (byte)250 : (byte)250;
            pe.FillStopOpacity = i == 1 ? (byte)250 : (byte)250;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);
        }

            CompoChart(UltraChart1,1);
        }

        protected void UltraChart1_Init(object sender, EventArgs e)
        {

        }
    }
}
