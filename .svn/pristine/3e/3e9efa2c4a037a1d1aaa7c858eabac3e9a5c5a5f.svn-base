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

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using Orientation = Infragistics.Documents.Excel.Orientation;

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;

using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Infragistics.WebUI.UltraWebNavigator;

using ContentAlignment = System.Drawing.ContentAlignment;
using FontStyle = System.Drawing.FontStyle;
using Graphics = System.Drawing.Graphics;
using IList = Infragistics.Documents.Reports.Report.List.IList;
using Symbol = Dundas.Maps.WebControl.Symbol;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Pen = System.Drawing.Pen;




namespace Krista.FM.Server.Dashboards.MO.MO_0001._0028._00
{
    public partial class _default : CustomReportPage
    {
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

        ArrayList al = new ArrayList();


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
                try
                {

                    if (ly != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                    {
                        ly = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                        //d.Add(ly, 0);
                        lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                        //d.Add(lm, 1);

                        //try
                        //{
                        //    d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                        //}
                        //catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }
                        AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7], 0);
                        AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13], 1);
                        ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                        

                        al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                    }
                    else
                        if (lm != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                        {
                            lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                            //d.Add(lm, 1);
                            AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13], 1);
                            ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);

                            al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                        }
                        else
                        {
                            //try
                            //{
                            //    d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            //}
                            //catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }

                            al.Add(cs.Axes[1].Positions[i].Members[0].UniqueName);
                            ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                        }
                }
                catch { }

            }


            ls2 = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName;
            //ls = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName.Split('[', ']')[15];

            return d;
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

        string ls2 = "";
        string ls = "";

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

        private CustomParam p1 { get { return (UserParams.CustomParam("p1")); } }
        private CustomParam p2 { get { return (UserParams.CustomParam("p2")); } }
        private CustomParam p3 { get { return (UserParams.CustomParam("p3")); } }
        private CustomParam p4 { get { return (UserParams.CustomParam("p4")); } }
        private CustomParam p5 { get { return (UserParams.CustomParam("p5")); } }
        string BE = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BE = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            G.Height = 600;

            BC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 1 - 25);
            BC.Height = 500;

            Mapo.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            Mapa.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);

            Mapa.Height = 800;
            Mapo.Height = 800;



            ////###########################################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //############################################################################

            Mapo.Meridians.Visible = false;
            Mapo.Parallels.Visible = false;
            Mapo.Viewport.EnablePanning = true;
        }
        #region xz
        public DateTime GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            return GetDateString(Year.GetNodeLastChild(Year.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                        }
                }
            }
            return DateTime.MinValue;
        }
        #endregion
        bool xnia = 1 == 2;
        bool xnia2 = 1 == 2;

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            
        }


        void Page_SetChart(Infragistics.WebUI.UltraWebChart.UltraChart UltraChart)
        {

            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            #region Настройка диаграммы

            UltraChart.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>%";

            //UltraChart.Tooltips.FormatString = "<b><ITEM_LABEL></b>  <DATA_VALUE_ITEM:N2>(<PERCENT_VALUE:#0.00>%)";


            for (int i = 1; i <= 6; i++) ;

            #endregion

        }
        string mapFolderName;
        protected override void Page_Load(object sender, EventArgs e)
        {
            {
                
                
                    //RegionSettingsHelper.Instance.SetWorkingRegion("Kostroma");
                    p4.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBaseDimension");
                    p5.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsMini2");
                    mapFolderName = "Кострома";
                //try
                {
                   base.Page_Load(sender, e);
                    p2.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    if (!Page.IsPostBack)
                    {
                            Year.MultiSelect = false;
                            Year.ShowSelectedValue = false;
                            Year.ParentSelect = 1 == 1;
                            Dictionary<string, int> d = GenDistonary("LD");
                            Year.FillDictionaryValues(d);
                            Year.SetСheckedState(ls, true);
                            Year.PanelHeaderTitle = "Выберите дату";
                            Year.Width = 200;
                        

                    }
                    try
                    {
                        Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                        Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                    }
                    catch { }
                    Year.ParentSelect = 1 == 1;


                    p3.Value = "";
                    ls = DelLastsChar(Year.SelectedValue,' ');


                    Infragistics.WebUI.UltraWebNavigator.Node n3 = Year.SelectedNode;
                    Infragistics.WebUI.UltraWebNavigator.Node n2 = Year.SelectedNode.Parent.Parent.Nodes[0].Nodes[0];
                    Infragistics.WebUI.UltraWebNavigator.Node n1 = Year.SelectedNode.Parent.Parent.Nodes[0].Nodes[0];

                    if (n3 == n2)
                    {

                        DateTime deti = CRHelper.PeriodDayFoDate(GenString(n1, 0));
                        deti = deti.AddDays(-7);

                        p1.Value = GenString(n1, -1) + "," + GenString(n3, 0);
                        xnia = 1 == 1;
                        xnia2 = 1 == 1;
                    }
                    else
                        if (n2 == n1)
                        {
                            p1.Value = GenString(n1, -1) + "," + GenString(n2, 0) + "," + GenString(n3, 0);
                            xnia = 1 == 1;
                            xnia2 = 1 == 2;
                        }
                        else
                        {
                            p1.Value = GenString(n1, 0) + "," + GenString(n2, 0) + "," + GenString(n3, 0);
                            xnia = 1 == 2;
                            xnia2 = 1 == 2;
                        }
                    try
                    { }
                    catch { }

                        G.DataBind();

                        G.Rows[0].Cells[0].Text = "Всего по АО";
                        G.Rows[0].Hidden = 1 == 1;
                        G.Rows[1].Hidden = 1 == 1;
                        G.Rows[2].Hidden = 1 == 1;

                    //Жёсткая константа... такая жесткая однако...
                        G.Rows[3].Cells[6].Value = 100*(System.Decimal)(G.Rows[3].Cells[4].Value)/906400;

                        RashetGrid();

                        

                        RasG2();
                        //G.Rows[3].Cells[4].Text = "ololo"; //906400 / (System.Decimal)(G.Rows[i].Cells[2].Value);
                        //.Text = "ololo"; //906400 / (System.Decimal)(G.Rows[i].Cells[2].Value);
                   

                    
                    p1.Value = "[Период].[Период].[Год].[" + DelLastsChar(Year.SelectedNode.Parent.Parent.Text,' ') +
                    "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text,' '))).ToString() + 
                      "].[Квартал " + CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text,' '))).ToString() + 
                    "].[" + DelLastsChar(Year.SelectedNode.Parent.Text,' ') + "].[" + ls + "]";
                    p2.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionComparableDimention");

                    n3 = Year.SelectedNode;
                    n2 = GetPredNode(n3);
                    n1 = GetPredNode(n2);
                    Infragistics.WebUI.UltraWebNavigator.Node n0 = GetPredNode(n1);

                    int x0 = 0;
                    int x1 = 0;
                    int x2 = 0;

                    if (n0 == n1)
                    {
                        x0 = 1;
                    }
                    if (n2 == n1)
                    {
                        x1 = -2;
                    }
                    if (n3 == n2)
                    {
                        x2 = -1;
                    }
                    p1.Value = GenString(n0, x0) + "," + GenString(n1, x1) + "," + GenString(n2, x2) + "," + GenString(n3, 0);
                    try
                    {
                    }     
                    catch { }
                    try
                    {
                    }
                    catch
                    { }

                    p1.Value = "[Период].[Период].[Год].[" + DelLastsChar(Year.SelectedNode.Parent.Parent.Text, ' ') +
                        "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text, ' ' ))).ToString() +
                        "].[Квартал " + CRHelper.QuarterNumByMonthNum((CRHelper.MonthNum(DelLastsChar(Year.SelectedNode.Parent.Text,' ')))).ToString() +
                        "].[" + DelLastsChar(Year.SelectedNode.Parent.Text,' ') + "].[" + ls + "]";
                    try
                    { }
                    catch { }
                        BC.DataBind();
                    
                    DateTime currDateTime = GetDateString(Year.GetSelectedNodePath(), Year.SelectedNode.Level);
                    //Мониторинг ситуации на рынке труда по области в целом по состоянию на {0:dd.MM.yyyy}

                    Label2.Text = string.Format("Уровень зарегистрированной безработицы по муниципальным образованиям на {0:dd.MM.yyyy}", currDateTime);
                    Label3.Text = string.Format("Коэффициент напряженности на рынке труда на {0:dd.MM.yyyy}", currDateTime);
                    Hederglobal.Text = string.Format("Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на {0:dd.MM.yyyy}", currDateTime);
                }//                catch
                {
                }
                Page.Title = string.Format(Hederglobal.Text, "");
                
                {
                    
                    SetMapa(Mapo, "mapa");
                    SetMapa(Mapa, "mapo");
                }
            }
        }

        #region grid

        string GenString(Infragistics.WebUI.UltraWebNavigator.Node n, int add)
        {            
            string l = DelLastsChar(n.Text,' ');
            try
            {

                l = (int.Parse(l) + add).ToString();

                n.Hidden = 1 == 2;
            }
            catch { }
            //Label1.Text = "[Период].[Период].[Год].[" + DelLastsChar(n.Parent.Parent.Text, ' ') + "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(n.Parent.Text, ' '))).ToString() + "].[Квартал " + CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(DelLastsChar(n.Parent.Text, ' '))).ToString() + "].[" + DelLastsChar(n.Parent.Text, ' ') + "].[" + l + "]"; ;
            return "[Период].[Период].[Год].[" + DelLastsChar(n.Parent.Parent.Text,' ') + "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(n.Parent.Text, ' '))).ToString() + "].[Квартал " + CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(DelLastsChar(n.Parent.Text,' '))).ToString() + "].[" + DelLastsChar(n.Parent.Text,' ') + "].[" + l + "]";
        }

        Infragistics.WebUI.UltraWebNavigator.Node GetPredNode(Infragistics.WebUI.UltraWebNavigator.Node n)
        {

            try
            {
                if (n.PrevNode != null)
                { n = n.PrevNode; }
                else
                {
                    n = n.Parent.PrevNode.Nodes[n.Parent.PrevNode.Nodes.Count - 1];
                }
            }
            catch
            {

            }

            return n;


        }


        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.DataSource = GetDSForChart("G");
        }
         
        protected void G_DataBinding1(object sender, EventArgs e)
        {
            G.Columns.Clear();
            G.Bands.Clear();

            DataTable dt = new DataTable();

            {

                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));
                dt.Columns.Add("xz");
                DataTable dt1 = GetDSForChart("G");

                for (int i = 0; i < cs.Axes[0].Positions.Count; i++)
                {

                    dt.Columns.Add(cs.Axes[0].Positions[i].Members[0].UniqueName + cs.Axes[0].Positions[i].Members[1].UniqueName, dt1.Columns[1].DataType);
                }

                object[] sum = dt1.Rows[dt1.Rows.Count - 1].ItemArray;
                for (int i = 1; i < dt1.Rows.Count - 1; i++)
                {
                    for (int j = 1; j < dt1.Rows[i].ItemArray.Length; j++)
                    {
                        try
                        {
                            if (((dt1.Rows[i].ItemArray[0].ToString() != "Костромская область") && (dt1.Rows[i].ItemArray[0].ToString() != "(Костромская область ДАННЫЕ)")))
                            {
                                sum[j] = (System.Decimal)(sum[j]) + (System.Decimal)(dt1.Rows[i].ItemArray[j]);
                            }
                        }
                        catch { }
                    }

                }
                try
                {
                    sum[7] = ((System.Decimal)(sum[4]) / (System.Decimal)(sum[1])) * 100;
                }
                catch { }
                try
                {
                    sum[8] = ((System.Decimal)(sum[5]) / (System.Decimal)(sum[2])) * 100;
                }
                catch { }
                try
                {
                    sum[9] = ((System.Decimal)(sum[6]) / (System.Decimal)(sum[3])) * 100;
                }
                catch { }
                try
                {
                    sum[16] = ((System.Decimal)(sum[10]) / (System.Decimal)(sum[13]));
                }
                catch { }
                try
                {
                    sum[17] = ((System.Decimal)(sum[11]) / (System.Decimal)(sum[14]));
                }
                catch { }
                try
                {
                    sum[18] = ((System.Decimal)(sum[12]) / (System.Decimal)(sum[15]));
                }
                catch { }
                dt.Rows.Add(sum);
                dt.Rows.Add(new object[dt1.Rows[0].ItemArray.Length]);
                dt.Rows.Add(new object[dt1.Rows[0].ItemArray.Length]);
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if ((dt1.Rows[i].ItemArray[0].ToString() != "Костромская область") && (dt1.Rows[i].ItemArray[0].ToString() != "(Костромская область ДАННЫЕ)"))
                    {
                        dt.Rows.Add(dt1.Rows[i].ItemArray);
                        dt.Rows.Add(new object[dt1.Rows[i].ItemArray.Length]);
                        dt.Rows.Add(new object[dt1.Rows[i].ItemArray.Length]);
                    }
                }
            }
            G.DataSource = dt;
        }

        protected bool fi(int index)
        {
            for (int i = 0; i < iskl.Length; i++)
            {
                if (index == iskl[i])
                {
                    return 2 == 1;
                }
            }
            return 1 == 1;
        }


        int[] iskl = new int[7];
        DataTable dte;
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            dte = GetDSForChart("Ge");
            string prs = "";
            ColumnHeader ch = new ColumnHeader();
            int counter = 0;
            int c2 = 0;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Территория";
            e.Layout.Bands[0].Columns[0].Width = 130;
            int c3 = 0;
            UltraGridColumn cp = e.Layout.Bands[0].Columns[0], cpp = e.Layout.Bands[0].Columns[1];
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.### ###");
                e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                {
                    if (prs != e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[7])
                    {
                        iskl[c2] = i;
                        c2++;
                        prs = e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[7];
                        ch.RowLayoutColumnInfo.SpanX = counter-1;
                        counter = 0;
                        ch = new ColumnHeader();
                        c3++;
                        ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[7] + (string.IsNullOrEmpty(dte.Rows[0].ItemArray[c3].ToString()) ? "" : ", ") + dte.Rows[0].ItemArray[c3].ToString().ToLower();
                        ch.RowLayoutColumnInfo.OriginY = 0;
                        ch.Style.Wrap = 1 == 1;
                        ch.RowLayoutColumnInfo.OriginX = e.Layout.Bands[0].HeaderLayout[i+1].RowLayoutColumnInfo.OriginX;
                        e.Layout.Bands[0].HeaderLayout.Add(ch);
                    }
                    e.Layout.Bands[0].Columns[i].Header.Caption = (e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[23].Length <= 1 ? "0" + e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[23] : e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[23]) + "." + (CRHelper.MonthNum(e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[21]).ToString().Length <= 1 ? ("0" + CRHelper.MonthNum(e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[21]).ToString()) : (CRHelper.MonthNum(e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[21]).ToString())) + "." + e.Layout.Bands[0].Columns[i].Header.Caption.Split('[', ']')[15];
                }
                counter++;
            }
            ch.RowLayoutColumnInfo.SpanX = counter;
            e.Layout.Bands[0].HeaderLayout[0].RowLayoutColumnInfo.SpanY = 2;

        }

        protected void RashetGrid()
        {
            G.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            G.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            G.Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 0; i < G.Rows.Count - 2; i++)
            {
                if (!string.IsNullOrEmpty(G.Rows[i].Cells[0].Text))
                {

                    G.Rows[i].Cells[0].RowSpan = 3;

                    for (int j = 1; j < G.Columns.Count; j++)
                    {
                        Decimal sd = System.Decimal.MaxValue;
                        {
                            
                            //{
                            //    
                            //}
                                if (fi(j))
                                {
                                    try
                                    {
                                        sd = Math.Round((((System.Decimal)(System.Decimal.Parse(G.Rows[i].Cells[j].Value.ToString())) - (System.Decimal)(System.Decimal.Parse(G.Rows[i].Cells[j - 1].Value.ToString())))), 2);
                                        sd = Math.Round(((System.Decimal)(G.Rows[i].Cells[j].Value) - (System.Decimal)(G.Rows[i].Cells[j - 1].Value)), 2);
                                    }
                                    catch
                                    {
                                    }
                                    G.Rows[i + 1].Cells[j].Value = ((sd == System.Decimal.MaxValue) ? (object)(null) : (object)(sd));

                                    int a = 2;
                                    ///  даже интересно, а следущая строчка как скомпилиться?
                                    G.Rows[i + 1].Cells[j].Title = sd > 0 ? sd != System.Decimal.MaxValue ? "Прирост к первой дате" : a * a != 4 ? "Я знаю, знаю. Пользователь меня никогда не увидит... эх.. но остаются надежда... вдруг случится сбой в программе, и наконец то я буду не просто строчкаой где то в памяти, меня увидит пользователь и я на всегда поселюсь между нервом его ушей..." : "" : sd < 0 ? "Падение относительно первой даты" : "";
                                    ;

                                }
                            
                        }
                        sd = sd == Decimal.MaxValue ? 0 : sd;
                        try
                        {
                            if (fi(j))
                            {
                                try
                                {
                                    G.Rows[i + 2].Cells[j].Value = ((( Math.Round((System.Double)(G.Rows[i].Cells[j].Value),2) /  Math.Round((System.Double)(G.Rows[i].Cells[j - 1].Value),2)) - 1.0) * 100.0).ToString("### ##0.00");
                                    

                                }
                                catch
                                {
                                    try
                                    {
                                        G.Rows[i + 2].Cells[j].Value = ((( Math.Round((System.Double)(System.Double.Parse(G.Rows[i].Cells[j].Value.ToString())),2) / Math.Round((System.Double)(System.Double.Parse(G.Rows[i].Cells[j - 1].Value.ToString())),2)) - 1.0) * 100.0).ToString("### ##0.00");
                                    
                                    }
                                    catch { }

                                }
                                if (G.Rows[i + 2].Cells[j].Value.ToString().Contains("бесконечность"))
                                {
                                    G.Rows[i + 2].Cells[j].Value = "-";
                                }
                                
                                G.Rows[i + 2].Cells[j].Title = string.IsNullOrEmpty(G.Rows[i + 2].Cells[j].Text) ? "" : "Темп прироста к первой дате";
                            }
                        }
                        catch { }
                        try
                        {
                            try
                            {
                                G.Rows[i].Cells[j].Value = Math.Round((System.Decimal)(G.Rows[i].Cells[j].Value), 2);
                            }
                            catch
                            {
                                G.Rows[i].Cells[j].Value = Math.Round((System.Decimal)(System.Decimal.Parse(G.Rows[i].Cells[j].Value.ToString())), 2);
                            }
                            if ((j >= iskl[0]) & (j < iskl[1]))
                            {
                                if ((System.Decimal)(sd) > 0)
                                {
                                    G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowGreenUpBB.png\"> ";
                                }
                                if ((System.Decimal)(sd) < 0)
                                {
                                    G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowRedDownBB.png\"> ";
                                }
                            }
                            else
                                if ((j > iskl[1]) & (j < iskl[2]))
                                {
                                    if ((System.Decimal)(sd) < 0)
                                    {
                                        G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowGreenDownBB.png\"> ";
                                    }
                                    if ((System.Decimal)(sd) > 0)
                                    {
                                        G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowRedUpBB.png\"> ";
                                    }
                                }
                                else
                                    if ((j > iskl[2]) & (j < iskl[3]))
                                    {
                                        if ((System.Decimal)(sd) < 0)
                                        {
                                            G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowGreenDownBB.png\"> ";
                                        }
                                        if ((System.Decimal)(sd) > 0)
                                        {
                                            G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowRedUpBB.png\"> ";
                                        }
                                    }
                                    else
                                        if ((j > iskl[3]) & (j < iskl[4]))
                                        {
                                            if ((System.Decimal)(sd) < 0)
                                            {
                                                G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowGreenDownBB.png\"> ";
                                            }
                                            if ((System.Decimal)(sd) > 0)
                                            {
                                                G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowRedUpBB.png\"> ";

                                            }
                                        }
                                        else
                                            if ((j > iskl[4]) & (j < iskl[5]))
                                            {
                                                if ((System.Decimal)(sd) > 0)
                                                {
                                                    G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowGreenUpBB.png\"> ";
                                                }
                                                if ((System.Decimal)(sd) < 0)
                                                {
                                                    G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowRedDownBB.png\"> ";
                                                }
                                            }
                                            else
                                                if ((j > iskl[5]) & (j < iskl[6]))
                                                {
                                                    if ((System.Decimal)(sd) > 0)
                                                    {
                                                        G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowRedUpBB.png\"> ";
                                                    }
                                                    if ((System.Decimal)(sd) < 0)
                                                    {
                                                        G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/arrowGreenDownBB.png\"> ";
                                                    }
                                                }
                                                else
                                                { 
                                                    if ((j > iskl[6]))
                                                    {
                                                        //if (j == 21)
                                                        {
                                                            if ((System.Decimal)(G.Rows[i].Cells[j].Value) >= 0)
                                                            {
                                                                G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/ballGreenBB.png\"> ";
                                                                G.Rows[i].Cells[j].Title = (System.Decimal)(G.Rows[i].Cells[j].Value) != 0 ? G.Rows[i].Cells[j].Title = "Профицит" : "";
                                                            }
                                                            if ((System.Decimal)(G.Rows[i].Cells[j].Value) < 0)
                                                            {
                                                                G.Rows[i + 1].Cells[j].Text += ";<img src=\"../../../../../images/ballRedBB.png\"> ";
                                                                G.Rows[i].Cells[j].Title = "Дефицит";
                                                            }
                                                        }
                                                    }
                                                }
                        }
                        catch { }
                    }

                }
            }
        }

        protected void RasG2()
        {
            for (int i = 1; i < G.Rows.Count; i += 3)
                for (int j = 1; j < G.Rows[i].Cells.Count; j++)
                {
                    try
                    {
                        if (G.Rows[i].Cells[j].Text.Split(';').Length == 2)
                        {
                            G.Rows[i - 1].Cells[j].Text = "<div style=\"FLOAT: left;\">" + G.Rows[i].Cells[j].Text.Split(';')[1] + "</div>" + "<b>" + G.Rows[i - 1].Cells[j].Text + "</b>";
                            G.Rows[i].Cells[j].Text = G.Rows[i].Cells[j].Text.Split(';')[0];
                            G.Rows[i + 1].Cells[j].Text = (G.Rows[i + 1].Cells[j].Text[0] == '-' ? G.Rows[i + 1].Cells[j].Text : "+" + G.Rows[i + 1].Cells[j].Text)+"%";// +G.Rows[i + 1].Cells[j].Text == "-" ? "" : "%";
                        }
                        else
                        {
                            G.Rows[i - 1].Cells[j].Text = "<b>" + G.Rows[i - 1].Cells[j].Text + "</b>";
                            G.Rows[i + 1].Cells[j].Text = G.Rows[i + 1].Cells[j].Text + "%";
                        }
                        G.Rows[i + 1].Cells[j].Text = G.Rows[i + 1].Cells[j].Text == "-%"?"-":G.Rows[i + 1].Cells[j].Text;


                        
                    }
                    catch { }
                }

                        

            for (int i = 1; i < G.Columns.Count; i++)
            {
                //Label1.Text = G.Columns.Count.ToString();   
                if (G.Columns.Count < 17)
                {
                    G.Columns[i].Width = BE == "IE" ? 140 : 148;
                }
                else
                {
                    G.Columns[i].Width = BE == "IE" ? 70 : 74;
                }
            }
            for (int i = 0; i < iskl.Length; i++)
            {
                
                try
                {
                    G.Columns[iskl[i]].Width = 0;
                    for (int j = 0; j    < G.Rows.Count; j++)
                    {
                        G.Rows[j].Cells[iskl[i]].Text = "";
                    }
                }
                catch { }
                G.Columns[iskl[i]].Header.Caption = "";
                G.Columns[iskl[i]].Width = 0;
                G.Columns[iskl[i]].Header.Style.Width = 0;
                G.Columns[iskl[i]].Hidden = 1==1;

                //G.Columns[iskl[i]].;                
            }

            for (int i = 0; i < G.Rows.Count; i++)
            {
                try
                {
                    G.Rows[i].Cells[20].Text = G.Rows[i].Cells[20].Text.Replace("<b>", "").Replace("</b>", "");
                }
                catch { }
            }
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {

        }


        /// <summary>
        /// антискобачки
        /// </summary>
        /// <param name="dt"></param>
        void DelSkoba(DataTable dt)
        {
            for (int i = 0; dt.Rows.Count > i; i++)
            {
                if (dt.Rows[i].ItemArray[0].ToString()[0] == '(')
                {
                    dt.Rows[i].Delete();
                    i--;
                }

            }
        }


        

        System.Decimal[] ar;
        /// <summary>
        /// Это не дроид
        /// </summary>
        System.Decimal[] ar2;

        DataTable CC1, CC2;
        #endregion

        #region хня
        protected void CL_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("LC");
            DelSkoba(dt);
            DataTable dt1 = new DataTable();
            dt1.Columns.Add(dt.Columns[0].Caption, dt.Columns[0].DataType);
            dt1.Columns.Add(dt.Columns[2].Caption, dt.Columns[2].DataType);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object[] o = new object[2];

                if (dt.Rows[i].ItemArray[2] != DBNull.Value)
                {
                    o[0] = dt.Rows[i].ItemArray[1];
                    o[1] = dt.Rows[i].ItemArray[2];

                    dt1.Rows.Add(o);
                }
            }
            //LC.DataSource = dt1;
            //LC.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:00.##></b> " + dte.Rows[0].ItemArray[1].ToString().ToLower();
        }

        protected void LC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        Type t;
        protected void RC_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("RC");
            DelSkoba(dt);

            DataTable dt1 = new DataTable();
            dt1.Columns.Add(dt.Columns[0].Caption, dt.Columns[0].DataType);
            dt1.Columns.Add(dt.Columns[2].Caption, dt.Columns[2].DataType);
            t = dt.Columns[2].DataType;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object[] o = new object[2];
                if (dt.Rows[i].ItemArray[2] != DBNull.Value)
                {
                    o[0] = dt.Rows[i].ItemArray[1];
                    o[1] = dt.Rows[i].ItemArray[2];

                    dt1.Rows.Add(o);
                }
            }

            //RC.DataSource = dt1;
            //RC.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:00.##></b> " + dte.Rows[0].ItemArray[2].ToString().ToLower();
        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("CC");
            DataTable dt1 = new DataTable();

            dt1.Columns.Add("Показатель");

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("CC"));

            for (int i = 2; i < dt.Columns.Count; i++)
            {
                string mounth = CRHelper.MonthNum(cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[13]).ToString();
                string year = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[7];
                string day = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[15];
                dt1.Columns.Add(((day.Length < 2) ? "0" + day : day) +
                    "." + ((mounth.Length < 2) ? "0" + mounth : mounth) +
                    "." + ((year.Length < 2) ? "0" + year : year), dt.Columns[i].DataType);
            }

            ar = new System.Decimal[dt.Columns.Count - 1];

            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                object[] o = new object[dt.Columns.Count - 1];
                o[0] = dt.Rows[i].ItemArray[1].ToString();
                for (int j = 2; j < dt.Rows[i].ItemArray.Length; j++)
                {
                    o[j - 1] = dt.Rows[i].ItemArray[j];
                    try
                    {
                        ar[j - 2] += (System.Decimal)(dt.Rows[i].ItemArray[j]);
                    }
                    catch { }
                }
                dt1.Rows.Add(o);
            }
            CC1 = dt1;
        }

        protected void BC_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("BC0");
            DataTable dt0 = GetDSForChart("BC1");

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("BC0"));

            DataTable dt1 = new DataTable();

            dt1.Columns.Add("xz");
            for (int i = 1; dt.Columns.Count > i; i++)
            {
                string mounth = CRHelper.MonthNum(cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[13]).ToString();
                string year = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[7];
                string day = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[15];
                dt1.Columns.Add(((day.Length < 2) ? "0" + day : day) +
                    "." + ((mounth.Length < 2) ? "0" + mounth : mounth) +
                    "." + ((year.Length < 2) ? "0" + year : year), dt.Columns[i].DataType);                
            }
            object[] sum1 = dt.Rows[0].ItemArray;
            object[] sum0 = dt0.Rows[0].ItemArray;

            sum1[0] = "Численность незанятых граждан, состоящих на учёте в органах службы занятости населения, " + dte.Rows[0].ItemArray[4].ToString().ToLower();
            sum0[0] = "Численность вакансий, " + dte.Rows[0].ItemArray[5].ToString().ToLower();
            try
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    for (int j = 1; j < dt.Rows[0].ItemArray.Length; j++)
                    {
                        try
                        {
                            sum1[j] = (System.Decimal)(sum1[j]) + (System.Decimal)(dt.Rows[i].ItemArray[j]);
                        }
                        catch { }
                    }
                }
            }
            catch { }

            try
            {
                for (int i = 1; i < dt0.Rows.Count; i++)
                {
                    for (int j = 1; j < dt0.Rows[0].ItemArray.Length; j++)
                    {
                        try
                        {
                            sum0[j] = (System.Decimal)(sum0[j]) + (System.Decimal)(dt0.Rows[i].ItemArray[j]);
                        }
                        catch { }
                    }
                }
            }
            catch { }
            System.Decimal max = 0;
            for (int i = 1; i < sum0.Length; i++)
            {
                try
                {
                    if (max < (System.Decimal)(sum0[i]))
                    {

                        max = (System.Decimal)(sum0[i]);
                    }
                }
                catch { }
            }
            for (int i = 1; i < sum1.Length; i++)
            {
                try
                {
                    if (max < (System.Decimal)(sum1[i]))
                    {
                        max = (System.Decimal)(sum1[i]);
                    }
                }
                catch { }
            }
            BC.Axis.Y.RangeType = AxisRangeType.Custom;
            BC.Axis.Y.RangeMin = 0;
            BC.Axis.Y.RangeMax = (System.Double)(max) * 1.1;
            dt1.Rows.Add(sum1);
            dt1.Rows.Add(sum0);            
            BC.Series.Clear();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                BC.Series.Add(CRHelper.GetNumericSeries(i, dt1));
            }
            BC.Data.SwapRowsAndColumns = 1 == 1;

            //Численность вакансий на 17.03.2010
            //2455 единица



        }

        protected void CC_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("CCC");
            DataTable dt1 = new DataTable();

            dt1.Columns.Add("Показатель");

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("CCC"));

            for (int i = 2; i < dt.Columns.Count; i++)
            {
                string mounth = CRHelper.MonthNum(cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[13]).ToString();
                string year = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[7];
                string day = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[15];
                dt1.Columns.Add(((day.Length < 2) ? "0" + day : day) +
                    "." + ((mounth.Length < 2) ? "0" + mounth : mounth) +
                    "." + ((year.Length < 2) ? "0" + year : year), dt.Columns[i].DataType);
            }

            ar2 = new System.Decimal[dt.Columns.Count - 1];

            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                object[] o = new object[dt.Columns.Count - 1];
                o[0] = dt.Rows[i].ItemArray[1].ToString();
                for (int j = 2; j < dt.Rows[i].ItemArray.Length; j++)
                {
                    o[j - 1] = dt.Rows[i].ItemArray[j];
                    try
                    {

                        ar[j - 2] += (System.Decimal)(dt.Rows[i].ItemArray[j]);
                    }
                    catch { }
                }
                dt1.Rows.Add(o);
            }
            for (int i = 1; i < 5; i++)
            {
                try
                {
                }
                catch { }
            }
            CC2 = dt1;
        }

        #endregion

        #region
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 0; i < G.Columns.Count; i++)
            {
                e.CurrentWorksheet.Rows[0].Cells[i].Value = null;
                e.CurrentWorksheet.Rows[1].Cells[i].CellFormat.FillPatternBackgroundColor = Color.Gray;
                e.CurrentWorksheet.Rows[1].Cells[i].CellFormat.FillPatternForegroundColor = Color.Gray;
                e.CurrentWorksheet.Rows[1].Cells[i].Value = G.Columns[i].Header.Caption;

                e.CurrentWorksheet.Rows[0].Cells[i].CellFormat.FillPatternBackgroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[0].Cells[i].CellFormat.FillPatternForegroundColor = Color.LightGray;

                e.CurrentWorksheet.Rows[0].Cells[i].CellFormat.TopBorderColor = Color.Gray;
                e.CurrentWorksheet.Rows[0].Cells[i].CellFormat.BottomBorderColor = Color.Gray;
                e.CurrentWorksheet.Rows[0].Cells[i].CellFormat.LeftBorderColor = Color.Gray;
                e.CurrentWorksheet.Rows[0].Cells[i].CellFormat.RightBorderColor = Color.Gray;

                e.CurrentWorksheet.Columns[i].Width = e.CurrentWorksheet.Columns[i].Width != 0 ? 8000 : 0;

                if (G.Columns[i].Width != 0)
                {
                    for (int j = 0; j < G.Rows.Count; j++)
                    {
                        
                        e.CurrentWorksheet.Rows[j + 2].Cells[i].Value = null;
                        string z = "";
                        try
                        { z = G.Rows[j].Cells[i].Text.Split('<', '>')[G.Rows[j].Cells[i].Text.Split('>', '<').Length - 3]; }
                        catch
                        { z = G.Rows[j].Cells[i].Text; }
                        e.CurrentWorksheet.Rows[j + 2].Cells[i].Value = z;

                        
                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.FillPatternBackgroundColor = Color.White;
                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.FillPatternForegroundColor = Color.White;

                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.TopBorderColor = Color.Gray;
                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.BottomBorderColor = Color.Gray;
                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.LeftBorderColor = Color.Gray;
                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.RightBorderColor = Color.Gray;

                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.Font.Name = "Arial";
                        e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.Font.Height = 180;

                        if (i > 1)
                        {
                            e.CurrentWorksheet.Rows[j + 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                        }

                    }
                }
                else
                {
                    e.CurrentWorksheet.Columns[i].Width = 0;
                }
                e.CurrentWorksheet.Rows[1].Cells[i].CellFormat.FillPatternBackgroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[1].Cells[i].CellFormat.FillPatternForegroundColor = Color.LightGray;

            }

            for (int i = 0; G.Columns.Count > i; i++)
            {
                e.CurrentWorksheet.Rows[G.Rows.Count + 1].Cells[i].CellFormat.SetFormatting(e.CurrentWorksheet.Rows[G.Rows.Count - 2].Cells[i].CellFormat);
            }
            if (!(G.Columns.Count < 17))
            {
                e.CurrentWorksheet.Rows[0].Cells[2].Value = "Численность экономически активного населения, " + dte.Rows[0].ItemArray[1].ToString();

                e.CurrentWorksheet.Rows[0].Cells[5].Value = "Численность безработных граждан, " + dte.Rows[0].ItemArray[2].ToString();

                e.CurrentWorksheet.Rows[0].Cells[8].Value = "Уровень зарегистрированной безработицы, процент";

                e.CurrentWorksheet.Rows[0].Cells[11].Value = "Численность незанятых граждан, состоящих на учёте в органах службы занятости населения, " + dte.Rows[0].ItemArray[4].ToString();

                e.CurrentWorksheet.Rows[0].Cells[14].Value = "Численность вакансий, " + dte.Rows[0].ItemArray[5].ToString();

                e.CurrentWorksheet.Rows[0].Cells[17].Value = "Коэффициент напряженности на рынке труда,  на 1 вакансию/человек";

                e.CurrentWorksheet.Rows[0].Cells[20].Value = "Дефицит (-), профицит (+) рабочих мест";
            }
            else
            {
                e.CurrentWorksheet.Rows[0].Cells[2].Value = "Численность экономически активного населения, " + dte.Rows[0].ItemArray[1].ToString();
                e.CurrentWorksheet.Rows[0].Cells[2].CellFormat.Font.Name = "Arial";
                e.CurrentWorksheet.Rows[0].Cells[2].CellFormat.Font.Height = 180;


                e.CurrentWorksheet.Rows[0].Cells[4].Value = "Численность безработных граждан, " + dte.Rows[0].ItemArray[2].ToString();
                e.CurrentWorksheet.Rows[0].Cells[4].CellFormat.Font.Name = "Arial";
                e.CurrentWorksheet.Rows[0].Cells[4].CellFormat.Font.Height = 180;


                e.CurrentWorksheet.Rows[0].Cells[6].Value = "Уровень зарегистрированной безработицы, процент";
                e.CurrentWorksheet.Rows[0].Cells[6].CellFormat.Font.Name = "Arial";
                e.CurrentWorksheet.Rows[0].Cells[6].CellFormat.Font.Height = 180;

                e.CurrentWorksheet.Rows[0].Cells[8].Value = "Численность незанятых граждан, состоящих на учёте в органах службы занятости населения, " + dte.Rows[0].ItemArray[4].ToString();
                e.CurrentWorksheet.Rows[0].Cells[8].CellFormat.Font.Name = "Arial";
                e.CurrentWorksheet.Rows[0].Cells[8].CellFormat.Font.Height = 180;


                e.CurrentWorksheet.Rows[0].Cells[10].Value = "Численность вакансий, " + dte.Rows[0].ItemArray[5].ToString();
                e.CurrentWorksheet.Rows[0].Cells[10].CellFormat.Font.Name = "Arial";
                e.CurrentWorksheet.Rows[0].Cells[10].CellFormat.Font.Height = 180;

                e.CurrentWorksheet.Rows[0].Cells[12].Value = "Коэффициент напряженности на рынке труда,  на 1 вакансию/человек";
                e.CurrentWorksheet.Rows[0].Cells[12].CellFormat.Font.Name = "Arial";
                e.CurrentWorksheet.Rows[0].Cells[12].CellFormat.Font.Height = 180;

                e.CurrentWorksheet.Rows[0].Cells[14].Value = "Дефицит (-), профицит (+) рабочих мест";
                e.CurrentWorksheet.Rows[0].Cells[14].CellFormat.Font.Name = "Arial";
                e.CurrentWorksheet.Rows[0].Cells[14].CellFormat.Font.Height = 180;

                e.CurrentWorksheet.Columns[2].Width = 16000; //Value = "Численность экономически активного населения, " + dte.Rows[0].ItemArray[1].ToString();

                e.CurrentWorksheet.Columns[4].Width = 16000;//"Численность безработных граждан, " + dte.Rows[0].ItemArray[2].ToString();

                e.CurrentWorksheet.Columns[6].Width = 16000;//"Уровень зарегистрированной безработицы, процент";

                e.CurrentWorksheet.Columns[8].Width = 16000;//"Численность незанятых граждан, состоящих на учёте в органах службы занятости населения, " + dte.Rows[0].ItemArray[4].ToString();

                e.CurrentWorksheet.Columns[10].Width = 16000;//"Численность вакансий, " + dte.Rows[0].ItemArray[5].ToString();

                e.CurrentWorksheet.Columns[12].Width = 16000;//"Коэффициент напряженности на рынке труда,  на 1 вакансию/человек";

                e.CurrentWorksheet.Columns[14].Width = 16000;//"Дефицит (-), профицит (+) рабочих мест";
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
            for (int j = 0; j < G.Rows.Count; j++)
            {
            }
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }
        #endregion

        #region поверх диограмм
        protected void BC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            decimal[] smas = new decimal[4];

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        try
                        {
                            box.DataPoint.Label = ((System.Decimal)(CC1.Rows[box.Row].ItemArray[box.Column]) / ar[box.Row]).ToString(); ///box.Row.ToString() + " " + box.Column.ToString();
                        }
                        catch { }
                    }
                }
            }
        }

        protected void LC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
        }

        protected void BC_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Polygon)
                {
                    Polygon box = (Polygon)primitive;
                    if (box.Series != null)
                    {
                        try
                        {
                            box.Series.Label = ((box.Row == 0) ? "Численность незанятых граждан, состоящих на учёте<br> в органах службы занятости населения" : "Численность вакансий") 
                                + "  на <ITEM_LABEL> <SERIES_LABEL><br> <b><DATA_VALUE:### ##0.##></b> " +
                                ((box.Row == 0) ? dte.Rows[0].ItemArray[4].ToString().ToLower() : dte.Rows[0].ItemArray[5].ToString().ToLower());
                        }
                        catch { }
                    }

                }
                {
                    try
                    {
                        primitive.DataPoint.Label += "xz";
                    }
                    catch { }
                }
            }

        }

        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            decimal[] smas = new decimal[4];

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        try
                        {
                            box.DataPoint.Label = ((System.Decimal)(CC2.Rows[box.Row].ItemArray[box.Column]) / ar2[box.Row]).ToString();                            
                        }
                        catch { }
                    }
                }
            }
        }
        #endregion

        //######################################################### глючит
        #region
        protected void SetMapa(MapControl DundasMap, string dataForMapa)
        {
            DundasMap.Shapes.Clear();
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);            
            AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
            AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.NavigationPanel.Visible = false;

            // добавляем легенду раскраски
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Visible = dataForMapa =="mapo";
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;

            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
        
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.Title = dataForMapa == "mapo" ?
                "Коэффициент напряженности,\n    человек на одну вакансию" 
              : "Уровень зарегистрированной безработицы";
            legend1.AutoFitMinFontSize = 7;
            
            DundasMap.Legends.Add(legend1);

            // добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("CompletePercent");
            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;         
        
            // добавляем поля
            DundasMap.SymbolFields.Add("CrimesBK");
            DundasMap.SymbolFields["CrimesBK"].Type = typeof(double);
            DundasMap.SymbolFields["CrimesBK"].UniqueIdentifier = false;
            DundasMap.SymbolFields.Add("CrimesKU");
            DundasMap.SymbolFields["CrimesKU"].Type = typeof(double);
            DundasMap.SymbolFields["CrimesKU"].UniqueIdentifier = false;        

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = dataForMapa == "mapa"?30:7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.White;
            rule.MiddleColor = Color.Pink;
            rule.ToColor = Color.Maroon;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = true;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = dataForMapa == "mapo" ? "#FROMVALUE{### ##0.0##} - #TOVALUE{### ##0.0##}" : "#FROMVALUE{### ##0.0##}% - #TOVALUE{### ##0.0##}%";
            DundasMap.ShapeRules.Add(rule);
            DundasMap.ColorSwatchPanel.Visible = 1 == 1;
            // заполняем карту данными

            DataTable dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(dataForMapa), "xz", dtGrid);


            if (dataForMapa == "mapa")
            {
                AddMapSymbols(DundasMap);

                AddMapLegends(DundasMap);
            };
            
                foreach (Shape shape in DundasMap.Shapes)
                {
                    string shapeName = shape.Name;
                    //if ((!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName)))
                    //{
                    //    shape.Visible = false;
                    //}
                }
                if (dtGrid == null || DundasMap == null)
                {
                    return;
                }

                Dundas.Maps.WebControl.Symbol symbol1 = new Dundas.Maps.WebControl.Symbol(); 
                for (int i = 0; i < DundasMap.Shapes.Count; i++)
                {
                    
                    for (int j = 0; j < dtGrid.Rows.Count; j++)
                    {
                        DundasMap.Shapes[i].TextVisibility = TextVisibility.Shown;
                        DundasMap.Shapes[i].Visible = 1 == 1;
                        try{
                            
                        if (DundasMap.Shapes[i].Name.Split('_')[0] == dtGrid.Rows[j].ItemArray[2].ToString().Replace(" ", "")
                            || 
                            (DundasMap.Shapes[i]["CODE"].ToString() == dtGrid.Rows[j].ItemArray[3].ToString())
                            )                            
                        {
                            try
                            {
                            
                                    DundasMap.Shapes[i]["Complete"] = Convert.ToDouble((System.Decimal)(dtGrid.Rows[j].ItemArray[dataForMapa=="mapa"?4:1]));                                
                                {
                                    string fString = dataForMapa != "mapo"                                       
                                      ? "\n{0}\n{2}" 
                                      : "\n{0}\n{1}";                       
                                    
                                    DundasMap.Shapes[i].Text = 
                                    (string.Format(fString, dtGrid.Rows[j].ItemArray[2].ToString()
                                    , Decimal.Round((System.Decimal)(dtGrid.Rows[j].ItemArray[1]), 2),
                                    dataForMapa == "mapa"?
                                    (System.Decimal)(dtGrid.Rows[j].ItemArray[4]):0));

                                    DundasMap.Shapes[i].ToolTip = dataForMapa != "mapo" ?
                                    string.Format("{1}\nЧисленность безработных граждан: {0} человек",
                                    Decimal.Round(Convert.ToDecimal(dtGrid.Rows[j].ItemArray[4]), 2), 
                                    dtGrid.Rows[j].ItemArray[0].ToString())
                                    : string.Format("{1}\nКоэффициент напряженности: {0} человек на одну вакансию"
                                    ,Decimal.Round(Convert.ToDecimal(dtGrid.Rows[j].ItemArray[1]), 2)
                                    ,dtGrid.Rows[j].ItemArray[0].ToString());

                                    //DundasMap.Shapes[i].to

                                    

                                    if (DundasMap.Shapes[i].Name[0] == 'г')
                                    {
                                        try
                                        {
                                            DundasMap.Shapes[i].TextAlignment = ContentAlignment.MiddleCenter;

                                            DundasMap.Shapes[i].Text =  DundasMap.Shapes[i].Name.Split('_')[0]
                                               + "\n\n\n" +
                                             Math.Round(Convert.ToDouble((System.Decimal)(dtGrid.Rows[j].ItemArray[dataForMapa == "mapa" ? 4 : 1])),2)
                                            + ((dataForMapa == "mapa")?" человек":"");
                                            if (dataForMapa == "mapo")
                                            {
                                                //DundasMap.Shapes[i].TextAlignment = ContentAlignment.MiddleCenter;
                                                //DundasMap.Shapes[i].Text = "\n"+DundasMap.Shapes[i].Text;
                                            }
                                        }
                                        catch { }
                                    }




                                }
                                if (dtGrid.Rows[j].ItemArray[1].ToString() == "")
                                {
                                    DundasMap.Shapes[i].Text = "";
                                }
                                if (dataForMapa == "mapa")
                                {
                                    {
                                        {
                                            symbol1 = new Dundas.Maps.WebControl.Symbol();
                                            symbol1.Name = DundasMap.Shapes[i].Name + DundasMap.Symbols.Count;
                                            symbol1.ParentShape = DundasMap.Shapes[i].Name;                                                                                        
                                            symbol1.Offset.Y = 17;
                                            symbol1.Offset.X = -5;
                                            symbol1["CrimesBK"] = Convert.ToDouble((System.Decimal)(dtGrid.Rows[j].ItemArray[1]));
                                            symbol1.Color = Color.Blue;//DarkViolet
                                            symbol1.MarkerStyle = MarkerStyle.Triangle;
                                            symbol1.ToolTip =
                                            string.Format("{1}\nУровень безработицы: {0}%  ",
                                            Decimal.Round(Convert.ToDecimal(dtGrid.Rows[j].ItemArray[1]), 2),
                                            dtGrid.Rows[j].ItemArray[0].ToString());

                                            symbol1.Text = Decimal.Round((System.Decimal)(dtGrid.Rows[j].ItemArray[1]), 2).ToString()+"%";
                                            symbol1.TextAlignment = Dundas.Maps.WebControl.TextAlignment.Right;

                                            DundasMap.Symbols.Add(symbol1);
                                            if (Convert.ToDouble((System.Decimal)(dtGrid.Rows[j].ItemArray[1])) < 2)
                                            {
                                                symbol1.Offset.Y+=6;
                                                symbol1.Offset.X = 0;
                                                
                                            }
                                            if ((DundasMap.Shapes[i].Name[0] == 'г')&& (DundasMap.Shapes[i].Name.Split('_').Length == 2))
                                            {
                                                symbol1.Text = "  " + Decimal.Round((System.Decimal)(dtGrid.Rows[j].ItemArray[1]), 2).ToString() + "%";
                                                symbol1.Offset.X = 0;
                                                symbol1.Offset.Y = -35;
                                                symbol1.TextAlignment = Dundas.Maps.WebControl.TextAlignment.Right;
                                            }



                                        }
                                    }

                                }

                            }
                            catch {
                             
                            
                            }
                        }
                        else
                        {

                        }
                        
                        }catch { }
                    }

                    if (((DundasMap.Shapes[i].Name[0] == 'г') && (DundasMap.Shapes[i].Name.Split('_').Length != 2)) 
                        || ((DundasMap.Shapes[i].Name[0] == 'п') && (DundasMap.Shapes[i].Name.Split('_').Length != 2)) 
                        || ((DundasMap.Shapes[i].Name[0] == 'с') && (DundasMap.Shapes[i].Name.Split('_').Length != 2)))
                    {
                        DundasMap.Shapes[i].Text = "";                        
                        try
                        {
                            symbol1.Offset.X = 0;
                            symbol1.Offset.Y = 0;
                            symbol1.Text = "";
                            symbol1.Visible = 1 == 2;
                        }
                        catch { }


                    }
                    else
                    {




                    }
                    if (DundasMap.Shapes[i].Text == "#NAME")
                    {
                        if (!((DundasMap.Shapes[i].Name[0] != 'г') & (DundasMap.Shapes[i].Name[0] != 'п') & (DundasMap.Shapes[i].Name[0] != 'с')))
                        {
                            DundasMap.Shapes[i].Text = DundasMap.Shapes[i].Name.Split('_')[0];
                            DundasMap.Shapes[i].TextAlignment = ContentAlignment.TopCenter;
                            DundasMap.Shapes[i].Text = DundasMap.Shapes[i].Text + "\n ";
                        }
                        else
                        {
                            DundasMap.Shapes[i].Text = DundasMap.Shapes[i].Name.Replace(' ', '\n');
                        }
                    }
                    if (CRHelper.MapShapeType.CalloutTowns.ToString() == DundasMap.Shapes[i].Layer)
                    {
                        DundasMap.Shapes[i].TextVisibility = TextVisibility.Shown;
                        DundasMap.Shapes[i].TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
                        DundasMap.Shapes[i].CentralPointOffset.Y = 10;
                    }

                }

                DundasMap.ColorSwatchPanel.Visible = dataForMapa =="mapa";
                DundasMap.ColorSwatchPanel.Title = "Численность безработных граждан, человек";
                DundasMap.ColorSwatchPanel.Size.Width = 400;
                DundasMap.ColorSwatchPanel.Dock = PanelDockStyle.Right;
                DundasMap.ColorSwatchPanel.DockAlignment = DockAlignment.Near;
        }

        private void AddMapColoringRules(MapControl DundasMap)
        {

        }

        private void AddMapSymbols(MapControl DundasMap)
        {
            // добавляем правила расстановки символов
            DundasMap.SymbolRules.Clear();

            SymbolRule symbolRule1 = new SymbolRule();
            symbolRule1.Name = "SymbolRuleBK";
            symbolRule1.Category = string.Empty;
            symbolRule1.DataGrouping = DataGrouping.EqualInterval;
            symbolRule1.FromValue = "1";
            symbolRule1.SymbolField = "CrimesBK";
            symbolRule1.ShowInLegend = "CrimesBKLegend";
            symbolRule1.LegendText = "#FROMVALUE{### ##0.00}% - #TOVALUE{### ##0.00}%";
            DundasMap.SymbolRules.Add(symbolRule1);

            // звезды для легенды
            for (int i = 1; i < 8; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbolBK" + i;
                predefined.MarkerStyle = MarkerStyle.Triangle;
                predefined.Width = 3 + (i * 3);
                predefined.Height = predefined.Width;
                predefined.Color = Color.Blue; 
                DundasMap.SymbolRules["SymbolRuleBK"].PredefinedSymbols.Add(predefined);
            }

            SymbolRule symbolRule2 = new SymbolRule();
            symbolRule2.Name = "SymbolRuleKU";
            symbolRule2.Category = string.Empty;
            symbolRule2.DataGrouping = DataGrouping.EqualInterval;
            symbolRule2.FromValue = "1";
            symbolRule2.SymbolField = "CrimesKU";
            symbolRule2.ShowInLegend = "CrimesKULegend";
            symbolRule2.LegendText = "#FROMVALUE{### ##0.00}% - #TOVALUE{### ##0.00}%";
            DundasMap.SymbolRules.Add(symbolRule2);

            // звезды для легенды
            for (int i = 1; i < 8; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbolKU" + i;
                predefined.MarkerStyle = MarkerStyle.Triangle;
                predefined.Width = 3 + (i * 3);
                predefined.Height = predefined.Width;
                predefined.Color = Color.Blue;
                DundasMap.SymbolRules["SymbolRuleKU"].PredefinedSymbols.Add(predefined);
            }
        }

        private void AddMapLegends(MapControl DundasMap)
        {
            DundasMap.Legends.Clear();

            // добавляем легенду
            Legend legend1 = new Legend("CrimesBKLegend");
            legend1.Visible = true;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.Title = "Уровень зарегистрированной\nбезработицы";
            legend1.AutoFitMinFontSize = 7;
            legend1.Dock = PanelDockStyle.Left;
            DundasMap.Legends.Add(legend1);

            Legend legend2 = new Legend("CrimesKULegend");
            legend2.Visible = false;
            legend2.BackColor = Color.White;
            legend2.BackSecondaryColor = Color.Gainsboro;
            legend2.BackGradientType = GradientType.DiagonalLeft;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Gray;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.Black;
            legend2.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = "Количество нарушений КУ";
            legend2.AutoFitMinFontSize = 7;
            legend2.Dock = PanelDockStyle.Left;
            DundasMap.Legends.Add(legend2);
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../../../../maps/{0}/{1}.shp", RegionSettingsHelper.Instance.GetRegionSetting("ShortName"),
                "Города"));
            
            map.LoadFromShapeFile(layerName, "NAME", true);
            
            layerName = Server.MapPath(string.Format("../../../../../maps/{0}/{1}.shp", RegionSettingsHelper.Instance.GetRegionSetting("ShortName"),
                "Выноски"));

            map.LoadFromShapeFile(layerName, "NAME", true);


            layerName = Server.MapPath(string.Format("../../../../../maps/{0}/{1}.shp", RegionSettingsHelper.Instance.GetRegionSetting("ShortName"),
    "Территор"));

            map.LoadFromShapeFile(layerName, "NAME", true);
            //Территор
            int oldShapesCount = map.Shapes.Count;
            //DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../../../maps/{0}/{1}.shp",
                //RegionSettingsHelper.Instance.GetRegionSetting("MapPathSubject"),
                //RegionSettingsHelper.Instance.GetRegionSetting("MapPathSubject")))
                //, "NAME", true);

            
        }

        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }
            return shapeName;
        }
        /// <summary>
        /// Является ли форма городом-выноской
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>true, если является</returns>
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
        }
        #endregion
        ///###################################################################
        ///


        #region Обработчики карты

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            //string subject = patternValue.Replace("область", "обл.");
            //subject = subject.Replace("автономный округ", "АО");

            //ArrayList shapeList = map.Shapes.Find(patternValue, true, false);
            ///if (shapeList.Count > 0)
            //{
            //    return (Shape)shapeList[0];
            //}
            for (int i = 0; i < map.Shapes.Count; i++)
            {
string s = map.Shapes[i]["CODE"].ToString();
                if (s == patternValue)
                {
                    return map.Shapes[i];
                }

            }
            

            return null;
        }
        DataTable dtMap1;
        public void FillMapData1(MapControl map)
        {

            map.ShapeFields.Clear();
            map.ShapeFields.Add("Name");
            map.ShapeFields["Name"].Type = typeof(string);
            map.ShapeFields["Name"].UniqueIdentifier = true;

            map.ShapeFields.Add("UnemploymentLevel");
            map.ShapeFields["UnemploymentLevel"].Type = typeof(double);
            map.ShapeFields["UnemploymentLevel"].UniqueIdentifier = false;

            map.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "UnemploymentLevel";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 30;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{### ##0.0##}% - #TOVALUE{### ##0.0##}%" ;//: "#FROMVALUE{### ##0.0##}% - #TOVALUE{### ##0.0##}%";
            map.ShapeRules.Add(rule);
            
            AddMapLayer(map, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);            
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("mapa");

            dtMap1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap1);

            map.Symbols.Clear();
            //Label1.Text += "7";
            foreach (DataRow row in dtMap1.Rows)
            {
                // заполняем карту данными

                //Label1.Text += "6";
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string regionName = row[3].ToString();

                    //if (RegionsNamingHelper.IsSubject(regionName))
                    {
                        Shape shape = FindMapShape(map, regionName);
                        
                        if (shape != null)
                        {
                        
                            double unemploymentLevel = Convert.ToDouble(row[1]) / 100;
                            double unemploymentPopulation = Convert.ToDouble(row[4]);

                            shape["Name"] = regionName;
                            shape["UnemploymentLevel"] = unemploymentLevel;
                            shape.ToolTip = string.Format("#NAME \nчисленность безработных: {0:N0} чел.\nуровень безработицы: #UNEMPLOYMENTLEVEL{{P3}}",
                                    unemploymentPopulation);
                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;
                            //if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\n{2:N0} чел.\n{1:P3}", shape.Name, unemploymentLevel, unemploymentPopulation);

                            shape.BorderWidth = 2;
                            shape.TextColor = Color.Black;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;

                            Symbol symbol = new Symbol();
                            symbol.Name = shape.Name + map.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["UnemploymentPopulation"] = unemploymentPopulation;
                            symbol.Offset.Y = -30;
                            symbol.Width = 40;
                            symbol.Height    = 40;
                            symbol.ToolTip = "xz";
                                
                            symbol.Color = Color.DarkViolet;
                            symbol.MarkerStyle = MarkerStyle.Star;
                            map.Symbols.Add(symbol);

                            //if (IsSmallResolution)
                            {
                                if (shape.Name.Contains("Курган"))
                                {
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                }
                                if (shape.Name.Contains("Челябинск"))
                                {
                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    shape.Offset.X = -10;
                                    symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Тюмен"))
                                {
                                    //                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    //                                    symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Свердловск"))
                                {
                                    shape.Offset.Y = -10;
                                    //symbol.Offset.Y = -10;
                                }
                            }
                        }
                    }
                }
            }
            Symbol smbl = new Symbol();

            smbl.MarkerStyle = MarkerStyle.Pentagon;

            smbl.Width = 200;
            smbl.Height = 200;
            //smbl.ParentShape = map.Shapes[0].Name;
            map.Symbols.Add(smbl);
        }
        DataTable dtMap2;
        public void FillMapData2(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("STAT_0001_0002_map2");

            dtMap2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap2);

            foreach (DataRow row in dtMap2.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();

                    //if (RegionsNamingHelper.IsSubject(regionName))
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            double tensionKoeff = Convert.ToDouble(row[1]);
                            double redundantCount = Convert.ToDouble(row[2]);
                            double vacancyCount = Convert.ToDouble(row[3]);
                            double totalCount = vacancyCount + redundantCount;

                            shape["Name"] = regionName;
                            shape["TensionKoeff"] = tensionKoeff;
                            shape.ToolTip = string.Format("#NAME \nчисло зарегистрированных безработных в расчёте на 1 вакансию: #TENSIONKOEFF{{N2}}\nчисло безработных: {0:N0} чел.\nчисло вакансий: {1:N0}",
                                    redundantCount, vacancyCount);
                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.TextColor = Color.Black;
                            shape.Offset.X = -15;
                            //if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\nвакансий: {2:N0}\n{1:N2}", shape.Name, tensionKoeff, vacancyCount);
                            shape.BorderWidth = 2;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;

                            Symbol symbol = new Symbol();
                            symbol.Name = shape.Name + map.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["vacancyCount"] = totalCount == 0 ? 0 : vacancyCount / totalCount * 100;
                            symbol["redundantCount"] = totalCount == 0 ? 0 : redundantCount / totalCount * 100;
                            symbol.Offset.Y = -40;
                            symbol.MarkerStyle = MarkerStyle.Circle;
                            map.Symbols.Add(symbol);

                            //if (IsSmallResolution)
                            {
                                if (shape.Name.Contains("Курган"))
                                {
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                    shape.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Челябинск"))
                                {
                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    symbol.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Тюмен"))
                                {
                                    //                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    //                                    symbol.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Свердловск"))
                                {
                                    shape.Offset.Y = -10;
                                    //symbol.Offset.Y = -10;
                                }
                            }
                        }
                    }
                }
            }
        }

        void DundasMap2_PostPaint(object sender, MapPaintEventArgs e)
        {
            Symbol symbol = e.MapElement as Symbol;
            if (symbol != null && symbol.Visible)
            {
                // Размер диаграммы
                int width = 30;
                int height = 30;

                // Get the symbol location in pixels.
                MapGraphics mg = e.Graphics;
                PointF p = symbol.GetCenterPointInContentPixels(mg);
                int x = (int)p.X - width / 2;
                int y = (int)p.Y - height / 2;
                symbol.Width = width;
                symbol.Height = height;

                int startAngle, sweepAngle1, sweepAngle2;

                // Делим углы соотвественно долям
                startAngle = 0;
                sweepAngle1 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["redundantCount"]));
                sweepAngle2 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["vacancyCount"]));

                // Поверх символа рисуем круговую диаграмму
                Graphics g = mg.Graphics;
                g.FillPie(new SolidBrush(Color.DarkViolet), x, y, width, height, startAngle, sweepAngle1);
                startAngle += sweepAngle1;
                g.FillPie(new SolidBrush(Color.Black), x, y, width, height, startAngle, sweepAngle2);

                g.DrawEllipse(new Pen(Color.Gray, 1), x, y, width, height);
            }
        }

        #endregion


    }

}

