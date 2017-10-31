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

using ContentAlignment = System.Drawing.ContentAlignment;
using FontStyle = System.Drawing.FontStyle;
using Graphics = System.Drawing.Graphics;
using IList = Infragistics.Documents.Reports.Report.List.IList;
using Symbol = Dundas.Maps.WebControl.Symbol;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Pen = System.Drawing.Pen;

namespace Krista.FM.Server.Dashboards.reports.EO.EO_007.ContractProductionGroup
{
    public partial class _default : CustomReportPage
    {

            
        private CustomParam SelectYear { get { return (UserParams.CustomParam("SelectYear")); } }
        private CustomParam SelectQard { get { return (UserParams.CustomParam("SelectQard")); } }
        private CustomParam SelectHalf { get { return (UserParams.CustomParam("SelectHalf")); } }

        private CustomParam TypeProduct { get { return (UserParams.CustomParam("TypeProduct")); } }

        private CustomParam SelectYear2 { get { return (UserParams.CustomParam("SelectYear2")); } }
        private CustomParam SelectQard2 { get { return (UserParams.CustomParam("SelectQard2")); } }
        private CustomParam SelectHalf2 { get { return (UserParams.CustomParam("SelectHalf2")); } }

        private CustomParam GRBS { get { return (UserParams.CustomParam("GRBS")); } }
        private CustomParam FD_ { get { return (UserParams.CustomParam("FD")); } }

        private CustomParam Region { get { return (UserParams.CustomParam("Region")); } } 

        string GRBS_s = ",[ГРБС__Сопоставимый].[ГРБС__Сопоставимый].[ГРБС].[{0}]";
        string PBS_s = ",[ГРБС__Сопоставимый].[ГРБС__Сопоставимый].[ПБС].[{0}]";         

        string xz = "";
        string xz2 = "";
        string LD = "",FD = "";

        Dictionary<string, int> ForParam(string query)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(query));
            string year = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            
            //FD = AID(d, year, 0);  
            
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
                if (cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7] != year)
                {
                    year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                    AID(d, year, 0);
                }
                try
                {
                    xz = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11], 1);
                }
                catch { }
            }




            return d;
        }
        Dictionary<string, int> ForParamRegion2(string query)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Квартал 1",0);
            d.Add("Квартал 2", 0);
            d.Add("Квартал 3", 0);
            d.Add("Квартал 4", 0);
            return d;
        }

        Dictionary<string, int> ForParamRegion(string query)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(query));

            FD = AID(d, cs.Axes[1].Positions[0].Members[0].Caption, 0);
            d = new Dictionary<string, int>();
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
               xz2 =  AID(d, cs.Axes[1].Positions[i].Members[0].Caption,0);
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
                {}
                lev += " ";
            }
            return str + " " + lev;
        }

        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            string ly = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            string lm = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];

            d.Add(ly, 0);
            d.Add(lm, 1);
            d.Add(cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[15], 2);

            string subS = "";
            //красота 

            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                try
                {

                    if (ly != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                    {
                        ly = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                        d.Add(ly, 0);
                        lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                        d.Add(lm, 1);

                        try
                        {
                            d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                        }
                        catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }                       
                    }
                    else
                        if (lm != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                        {
                            lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                            d.Add(lm, 1);

                            try
                            {
                                d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            }
                            catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }
                        }
                        else
                        {
                            try
                            {
                                d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            }
                            catch { d.Add(cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15] + " ", 2); }
                        }
                }
                catch { }

            }

            return d;
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
            };
            return table;
        }


        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }

        string AddZero(string s)
        {
            try
            {
                int i;
                for (i = s.Length - 1; s[i] != ','; i--) ;
                if (s.Length - i == 2)
                {
                    return s + "0";
                }

            }
            catch
            {
                return s + ",00";
            }
            return s;

        }
        string AddSpace(string s, char cg)
        {
            int i;
            try
            {
                for (i = 0; s[i] != cg; i++) ;

                int j = 0;

                for (j = i - 3; j > 0; j -= 3)
                {
                    try
                    {
                        s = s.Insert(j, " ");
                    }
                    catch { }
                }
            }
            catch
            {
            }
            return s;
        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            G.Width = CustomReportConst.minScreenWidth-40;

            C1.Width = (CRHelper.GetChartWidth(CustomReportConst.minScreenWidth) / 2)-20;
            C2.Width = (CRHelper.GetChartWidth(CustomReportConst.minScreenWidth) / 2)-20;

            C1.Height = 400;
            C2.Height = 400;

            C3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth)-40;
            C4.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth)-40;
            C5.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth)-40;

            confChart(C3);
            confChart(C4);
            confChart(C5);

            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;

            ////###########################################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //############################################################################

            WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G);
            WebAsyncRefreshPanel2.LinkedRefreshControlID = WebAsyncRefreshPanel1.ID;
            WebAsyncRefreshPanel3.LinkedRefreshControlID = WebAsyncRefreshPanel1.ID;
            WebAsyncRefreshPanel4.LinkedRefreshControlID = WebAsyncRefreshPanel1.ID;
            Webasyncrefreshpanel5.LinkedRefreshControlID = WebAsyncRefreshPanel1.ID;

            C1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            C2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            C3.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            C4.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            C5.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);


            WebAsyncRefreshPanel1.AddRefreshTarget(Label1);
            WebAsyncRefreshPanel1.AddRefreshTarget(Label2);
            WebAsyncRefreshPanel1.AddRefreshTarget(Label3);
            WebAsyncRefreshPanel1.AddRefreshTarget(Label4);
            WebAsyncRefreshPanel1.AddRefreshTarget(Label5);
            WebAsyncRefreshPanel1.AddRefreshTarget(Label6);
            GRBS.Value = "";            
        }

        void confChart(UltraChart CC)
        {
            CC.ChartType = ChartType.StackBarChart;
            CC.StackChart.StackStyle = StackStyle.Complete;

            CC.Tooltips.FormatString = "<ITEM_LABEL>\nЗначение <DATA_VALUE:N2> тыс.руб.\nДоля <PERCENT_VALUE:N2>%";//"<SERIES_LABEL>\n<DATA_VALUE_X:N3> тыс.чел.\n<DATA_VALUE_Y:N2> руб./чел.";// "<SERIES_LABEL> <ITEM_LABEL> <DATA_VALUE:### ##0.##><!---->, <PERCENT_VALUE_ITEM:P2>, <PERCENT_VALUE>";
                        
            CC.Legend.Visible = true;
            
            CC.Axis.X.Extent = 30;
            CC.Axis.Y.Extent = 40;

            CC.Axis.X.Labels.Visible = true;
            CC.Axis.Y.Labels.Visible = false;

            CC.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            CC.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>%";

            CC.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            CC.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            CC.Axis.Y.Labels.SeriesLabels.VerticalAlign = StringAlignment.Near;
            CC.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            CC.Axis.Y.Extent = 100;

            CC.Legend.Location = LegendLocation.Right;

            CC.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;

            CC.Legend.Location = LegendLocation.Bottom;
            CC.Legend.SpanPercentage = 37;

            CC.Height = 400;

            CC.ColorModel.ModelStyle = ColorModels.PureRandom;
        }




        bool Qart = false;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            //RegionSettingsHelper.Instance.SetWorkingRegion("PRTEST4");
            try{                
                if (!(Page.IsPostBack))
                {
                    Qart = 1==1;
                    try
                    { }
                    catch { }  
                    Qard.FillDictionaryValues(ForParamRegion2(""));
                        
                    Region_.FillDictionaryValues(ForParamRegion("region"));
                        
                    Year.FillDictionaryValues(ForParamRegion("LastDate1"));
                        
                    Year.SetСheckedState(xz2, 1 == 1);                        
                    Year.Width = 100;
                    Qard.Width = 200;

                    FD_.Value = FD;
                    CheckBox1.Checked = 1 == 2;
                    
                }
                if (!Qart)
                {
                    //Year.SetСheckedState(Year.SelectedNode.Parent.Nodes[0].Text, 1 == 1);
                }

                

                Region.Value = DelLastsChar(Region_.SelectedValue, ' ');
                           
                SelectYear.Value = DelLastsChar(Year.SelectedValue, ' ');
                SelectYear2.Value = (int.Parse(SelectYear.Value) - 1).ToString();
                
                SelectQard.Value = DelLastsChar(Qard.SelectedValue,' ');
                                
                if ((Qard.SelectedValue == "Квартал 1") || (Qard.SelectedValue == "Квартал 2"))
                { 
                    SelectHalf.Value = "1";             
                }
                else
                {
                    SelectHalf.Value = "2";                    
                }

                LTLC.Text = "Структура заказа по видам продукции за " + Qard.SelectedValue[Qard.SelectedValue.Length-1] + " квартал " + Year.SelectedValue + " года";
                LTRC.Text = "Структурная динамика заказа по видам закупаемой продукции";                                
                Page.Title = Hederglobal.Text;
                
                G.DataBind();
                UltraGridRow GrRows = G.Rows[0];           

                GrRows.Cells[0].Text = "Общий итог";
                
                G.Rows.Remove(GrRows);
                for (int i = 0; i < GrRows.Cells.Count; i++)
                {
                    GrRows.Cells[i].Style.Font.Bold = 1 == 1;   
                }
                G.Rows.Insert(G.Rows.Count, GrRows);

                //if (CheckBox1.Checked)
                //{
                //    int isPostBold = 0;

                //    for (int i = 0; G.Rows.Count > i; i++)
                //    {
                //        //String.Format(G.Rows[i].Cells[0].Text);
                //        if (G.Rows[i].Cells[0].Style.Font.Bold)
                //        {
                //            isPostBold = i;
                //        }
                //        if (G.Rows[i].Cells[0].Text[0] == '[')
                //        {
                //            //String.Format(G.Rows[i].Cells[0].Text + "---------------------------");
                //            for (int j = 1; j < G.Columns.Count; j++)
                //            {
                //                G.Rows[isPostBold].Cells[j].Value = G.Rows[i].Cells[j].Value;

                //            }
                //        }
                //    }
                //}

                    C1.DataBind();
                C2.DataBind();

                C3.DataBind();

                C4.DataBind();
                C5.DataBind();

                customer = GRBS.Value == "" ? "Все заказчики" : UserComboBox.getLastBlock(GRBS.Value.Replace(".datamember", ""));///////ОлОлО
                Label2.Text = "(" + customer + ")";
                Label3.Text = "(" + customer + ")";
                Label4.Text = "(" + customer + ")";
                Label5.Text = "(" + customer + ")";
                Label6.Text = "(" + customer + ")";
            }catch { }
        }
        string customer;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    G.Bands.Clear();
                    G.Columns.Clear();
                    G.Rows.Clear();
                }
                catch { }
                DataTable newDT = GetDSForChart("Grid");
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("Grid"));
                for (int i = 1; i < newDT.Columns.Count - 1; i++)
                {
                    newDT.Columns[i].Caption = newDT.Columns[i].Caption + ";" + cs.Axes[0].Positions[i].Members[0].UniqueName;
                }
                DataRow dr = null;
                for (int i = 0; i < newDT.Rows.Count; i++)
                {
                    newDT.Rows[i][0] = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('.').Length.ToString() + ";" + cs.Axes[1].Positions[i].Members[0].Caption;
                    bool empty = false;

                    string s = "";
                    if (cs.Axes[1].Positions[i].Members[0].UniqueName.Split('.').Length.ToString() == "4")
                    {
                        dr = newDT.Rows[i];
                        for (int j = 1; j < newDT.Columns.Count; j++)
                        {
                           // newDT.Rows[i][j] = DBNull.Value;
                        }
                    }
                    else { }
                    try
                    {

                        char c = newDT.Rows[i][0].ToString().Split(';')[1].ToString()[0];
                        if ((c == '(') && (dr != null))
                        {
                            if (newDT.Rows[i][0].ToString().Split(';')[1].Replace("(", "").Replace(" ДАННЫЕ)", "") == dr.ItemArray[0].ToString().Split(';')[1])
                            {
                                //dr.ItemArray = newDT.Rows[i].ItemArray;
                                dr[0] = dr[0].ToString().Replace("(", "").Replace(" ДАННЫЕ)", "") + ";-";
                                empty = 1 == 1;
                                dr = null;
                            }
                            else
                            {
                                //dr[0] = dr[0].ToString().Replace("(", "").Replace(" ДАННЫЕ)", "") + ";-";
                            }
                        }
                    }
                    catch { }
                    for (int j = 1; j < newDT.Columns.Count; j++)
                    {
                        try
                        {
                           // newDT.Rows[i][j] = (System.Decimal)(newDT.Rows[i][j]) / 1000;
                        }
                        catch { }
                    }
                }
                ////Label1.Text = newDT.Columns.Count.ToString();
                G.DataSource =  newDT.Rows.Count>2? newDT :null;
                //Label1.Text = string.Format(//Label1.Text);
            }
            //ws
            catch {G.DataSource = null;}
        }


        ColumnHeader AddTopHeder(HeadersCollection HC, int x,int y,int spanX,int spanY)        
        {
            ColumnHeader ch = new ColumnHeader();
            ch.RowLayoutColumnInfo.OriginX = x;
            ch.RowLayoutColumnInfo.OriginY = y;
            ch.RowLayoutColumnInfo.SpanX = spanX;
            ch.RowLayoutColumnInfo.SpanY = spanY;
            ch.Style.HorizontalAlign = HorizontalAlign.Center;
            ch.Style.VerticalAlign = VerticalAlign.Middle;
            
            HC.Add(ch);
            return ch;

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {/**/
            bool xz_ = 1 == 2;
            try
            {
                e.Layout.NullTextDefault = "<font style='font-family:Arial;font-size:20pt;'><br><br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbspНет данных</font>";
                e.Layout.NoDataMessage = "<font style='font-family:Arial;font-size:20pt;'><br><br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbspНет данных</font>";

                e.Layout.AllowSortingDefault = AllowSorting.No;
                
                
                string pred = e.Layout.Bands[0].HeaderLayout[1].Caption.Split(';')[0];
                string first = pred.Split(' ')[1]+" "+pred.Split(' ')[0]+" " + e.Layout.Bands[0].HeaderLayout[1].Caption.Split(';')[2].Split('[', ']')[7]+" года";
                bool showAll = (e.Layout.Bands[0].HeaderLayout[1].Caption.Split(';')[2].Split('[', ']')[7] == DelLastsChar(FD_.Value, ' '));
                
                int count = e.Layout.Bands[0].HeaderLayout.Count;
                for (int i = 1; i < count; i++)
                {
                    e.Layout.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginY = 1;
                    e.Layout.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.SpanY = 1;
                    e.Layout.Bands[0].HeaderLayout[i].Style.HorizontalAlign = HorizontalAlign.Center;
                    e.Layout.Bands[0].HeaderLayout[i].Style.VerticalAlign = VerticalAlign.Middle;
                    if (e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[0] != pred)
                    {
                        pred = e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[0];
                        AddTopHeder(e.Layout.Bands[0].HeaderLayout,
                            e.Layout.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX, 0, 3, 1).Caption = pred.Split(' ')[1] 
                            + " " + pred.Split(' ')[0] + " " + e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[2].Split('[', ']')[7] + " года";
                        xz_ = 1 == 1;
                    }
                    e.Layout.Bands[0].HeaderLayout[i].Caption = string.Format(e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[1]+"\n тыс. руб.");
                    e.Layout.Bands[0].HeaderLayout[i].Style.Wrap = 1 == 1;
                    
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth( 76 );///(CustomReportConst.minScreenWidth-225)/(e.Layout.Bands[0].Columns.Count-3);
                }
                //* (4.0/((e.Layout.Bands[0].Columns.Count-4)/3.0))
               
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
                e.Layout.Bands[0].Columns[0].Header.Caption = "Наименование заказчика";
                e.Layout.Bands[0].HeaderLayout[0].Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].Width = 200 + CRHelper.GetColumnWidth(76*( 16-e.Layout.Bands[0].Columns.Count));
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            }
            catch { }
            try
            {
                e.Layout.Bands[0].Columns[1].Width = 0;
                e.Layout.Bands[0].Columns[2].Width = 0;
                e.Layout.Bands[0].Columns[3].Width = 0;
                e.Layout.Bands[0].Columns[1].Header.Caption = "";
                e.Layout.Bands[0].Columns[2].Header.Caption = "";
                e.Layout.Bands[0].Columns[3].Header.Caption = "";
                
            }
            catch { }
            e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
//e.Layout.ClientSideEvents.RowSelectorClickHandler

            
        }
        int indexLastBold = -1;
        int indexLastBold_ = -1;

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Text[0] == '4')
            {
                e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                e.Row.Cells[0].Style.Margin.Left = 1;
                if (indexLastBold_ == -1)
                {
                    
                    indexLastBold_ = e.Row.Index;
                }
                else
                {
                    if (!CheckBox1.Checked)
                    for (int xz = 1; xz < e.Row.Cells.Count; xz++)
                    {
                        G.Rows[indexLastBold_].Cells[xz].Value = "";
                    } 
                }
                indexLastBold = e.Row.Index;
                
            }
            
            if (e.Row.Cells[0].Text[e.Row.Cells[0].Text.Length-1] == '-')
            {
                e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                e.Row.Cells[0].Style.Margin.Left = 1;

                if (indexLastBold_ == -1)
                {
                    
                    indexLastBold_ = e.Row.Index;
                }
                else
                {
                    if (!CheckBox1.Checked)
                    for (int xz = 1; xz < e.Row.Cells.Count; xz++)
                    {
                        G.Rows[indexLastBold_].Cells[xz].Value = "";
                    }
                }
                indexLastBold = e.Row.Index;
                
            }
            else
            {
                e.Row.Cells[0].Style.Margin.Left = 4;
            }
                for (int j = e.Row.Cells.Count - 1; j > 0; j--)
                {
                    string s = "";
                    try
                    {
                            ;
                        if (string.IsNullOrEmpty(e.Row.Cells[j].Text))
                        {
                            s = "";
                        }
                        else
                        {
                            System.Decimal dec = (System.Decimal)(e.Row.Cells[j].Value);
                            s = dec.ToString("### ### ##0.##");
                        }
                        e.Row.Cells[j].Title = ((System.Decimal)(e.Row.Cells[j].Value) > (System.Decimal)(e.Row.Cells[j - 3].Value) ?
                        "Выше на " + ((System.Decimal)(e.Row.Cells[j].Value) - (System.Decimal)(e.Row.Cells[j - 3].Value)).ToString("### ##0.##") + " тыс. руб. по сравнению с предыдущим кварталом"
                        :
                        "Ниже на " + ((System.Decimal)(e.Row.Cells[j - 3].Value) - (System.Decimal)(e.Row.Cells[j].Value)).ToString("### ##0.##") + " тыс. руб. по сравнению с предыдущим кварталом");
                        e.Row.Cells[j].Text = ((System.Decimal)(e.Row.Cells[j].Value) > (System.Decimal)(e.Row.Cells[j - 3].Value) ? ("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenUpBB.png\">") : ("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenDownBB.png\">")) + s;
                    

                        //Ниже / выше на * тыс. руб.. по сравнению с предыдущим кварталом
                    }
                    catch {e.Row.Cells[j].Text = s;}
                    
                    if (e.Row.Cells[j].Column.Width == 0)
                    {
                        e.Row.Cells[j].Text = "";
                    }
                    e.Row.Cells[j].Value = e.Row.Cells[j].Text+ "&nbsp";
                }
            e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[1];
            if (e.Row.Cells[0].Text[0] == '(')
            {
                 e.Row.Hidden = 1 == 1;
                 e.Row.Cells[0].Text = "[[[[[";

                 if (!CheckBox1.Checked)
                 {
                     for (int xz = 1; xz < e.Row.Cells.Count; xz++)
                     {
                         G.Rows[indexLastBold].Cells[xz].Value =e.Row.Cells[xz].Value;
                     }
                 }
                 //indexLastBold = -1;
            }    
            if (e.Row.Cells[0].Text == "Несопаставленные данные")
            {
                e.Row.Hidden = 1 == 1;
            }
        }

        protected void C1_DataBinding(object sender, EventArgs e)
        {
            //////Label1.Text += xz3.ToString();            
            try
            {
                DataTable dt = GetDSForChart("C1");
                C1.PieChart.OthersCategoryPercent = 0;
                C1.PieChart.OthersCategoryText = "Прочие";
                bool empty = 1 == 1;
                //////Label1.Text = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        ////Label1.Text += dt.Rows[i][1].ToString();
                        if ((System.Decimal)(dt.Rows[i][1]) != 0)
                        {
                            empty = false;
                            break;
                        }
                        //dt.Rows[i][1] = (System.Decimal)(dt.Rows[i][1]);
                    }
                    catch {
                        //dt.Rows[i].Delete();
                    //    i--;
                    }
                }
                C1.DataSource = (empty) ? null : dt;
                //C1.Visible = (!(xz3));
            }
            catch 
            {
                C1.DataSource = null;
            }
        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {
            ////Label1.Text += xz3.ToString();
            
            if (!xz3)
            {
                try
                {
                    DataTable dt = GetDSForChart("C2");
                    CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("C2"));
                    for (int i = 1; i < dt.Columns.Count; i++)
                    { dt.Columns[i].ColumnName = cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[7] + "\n" + cs.Axes[0].Positions[i - 1].Members[0].UniqueName.Split('[', ']')[11]; }
                    C2.Series.Clear();

                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            try
                            {
                                dt.Rows[j][i] = (System.Decimal)(dt.Rows[j][i]);
                            }
                            catch { }
                        }
                    }
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        NumericSeries series = CRHelper.GetNumericSeries(i, dt);
                            
                        C2.Series.Add(series);
                    }
                    C2.Data.SwapRowsAndColumns = 2 == 1;
                }
                catch
                {
                    C2.DataSource = null;
                }
                }
                else
                {
                    C2.DataSource = null;
                    C2.Series.Clear();                    
                }
        }
        DataTable C3_dt;
        protected void C3_DataBinding(object sender, EventArgs e)
        {            
            if (!xz3)
            {
                try
                {
                    TypeProduct.Value = "Товары";
                    DataTable dt = GetDSForChart("C3");
                    C3_dt = dt;

                    CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("C3"));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][0] = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11] + "\n" + cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7] + " года"; ;
                    }
         
                    C3.Series.Clear();                    
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        NumericSeries series = CRHelper.GetNumericSeries(i, dt);
                        C3.Series.Add(series);
                    }
                }
                catch
                {
                    C3.DataSource = null;
                }
                }
                else
                {
                    C3.DataSource = null;
                    C3.Series.Clear();
                    //C3.Visible = 1 == 2;
                }
        }
        DataTable C4_dt;
        protected void C4_DataBinding(object sender, EventArgs e)
        {         
            if (!xz3)
                try
                {
                    TypeProduct.Value = "Услуги";
                    DataTable dt = GetDSForChart("C3");
                    C4_dt = dt;
                    CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("C3"));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][0] = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11] + "\n" + cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7] + " года";
                    }   
                    C4.Series.Clear();
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            try
                            {
                                dt.Rows[j][i] = (System.Decimal)(dt.Rows[j][i]);
                            }
                            catch { }
                        }
                    }
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        NumericSeries series = CRHelper.GetNumericSeries(i, dt);
                        C4.Series.Add(series);
                    }
                }
                catch
                {
                    C4.DataSource = null;
                }
               else 
                {
                    C4.DataSource = null;
                    C4.Series.Clear();
         
                }
        }

        DataTable C5_dt;
        protected void C5_DataBinding(object sender, EventArgs e)
        {
         
            if (!xz3)
            try
            {
                TypeProduct.Value = "Работы";
                DataTable dt = GetDSForChart("C3");

                C5_dt = dt;

                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("C3"));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][0] = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11] + "\n" + cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7] + " года";
                }
                C5.Series.Clear();
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        try
                        {
                            dt.Rows[j][i] = (System.Decimal)(dt.Rows[j][i]);
                        }
                        catch { }
                    }
                }
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dt);
                    C5.Series.Add(series);
                }
            }
            catch 
            {
                C5.DataSource = null;
            }
            else
            {
                //C5.Visible = 1 == 2;
                C5.DataSource = null;
                C5.Series.Clear();
            }
        }


        #region
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = //Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int j = 0; j < G.Columns.Count + 5; j++)            
            {

                e.CurrentWorksheet.Columns[j].Width = 10000;
                for (int i = 0; i < G.Rows.Count + 5; i++)    
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].Value = null;   
                }
            }

            for (int i = 0; i < G.Rows.Count; i++)
            {
                for (int j = 0; j < G.Columns.Count; j++)
                {
                    try
                    {
                        e.CurrentWorksheet.Rows[i + 2].Cells[j].Value =
                            G.Rows[i].Cells[j].Value.ToString().Replace("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenUpBB.png\">", "")
                            .Replace("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenDownBB.png\">", "");
                    }
                    catch { }
                }
            }

            for (int i = 0; G.Bands[0].HeaderLayout.Count>i; i++)
            {
                e.CurrentWorksheet.Rows[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginY].Cells[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX].Value = G.Bands[0].HeaderLayout[i].Caption;
                e.CurrentWorksheet.Rows[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginY].Cells[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX].CellFormat.FillPatternBackgroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginY].Cells[G.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.OriginX].CellFormat.FillPatternForegroundColor = Color.LightGray;
            }
            e.CurrentWorksheet.Columns[1].Width = 0;
            e.CurrentWorksheet.Columns[2].Width = 0;
            e.CurrentWorksheet.Columns[3].Width = 0;
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
        bool xz3 = 1 == 2;
        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
         

            C2.Data.SwapRowsAndColumns = 2 == 1;

            bool b = 1 == 1;
         
            for (int i = 2; i < e.Row.Cells.Count; i++)
            {
                string s = e.Row.Cells[i].Text;
                if (s == e.Row.Cells[1].Text)
                {   
                    ////Label1.Text += "(0"+s+"0)";
                }
                else
                {
                    b = false;
                    ////Label1.Text += "(#"+s+"#)";
                };
            }

            xz3 = b;

            ////Label1.Text += "(*"+b.ToString()+"*)";
            if (e.Row.Index != (G.Rows.Count - 1))
            {
                //спать хочу
                string xZZZZ = "";
                if (!CheckBox1.Checked)
                {
                    xZZZZ = ".datamember";  
                }
                GRBS.Value = string.Format((e.Row.Cells[0].Style.Font.Bold ? GRBS_s : PBS_s), e.Row.Cells[0].Text)+xZZZZ;
            }
            else
            {
                GRBS.Value = "";
            }
            try
            {
                //C1.DataBind();
                //C1.DataSource = b ? null : C1.DataSource;
            }
            catch { }
            try
            {
                C2.DataBind();
                C2.DataSource = b ? null : C2.DataSource;
            }catch{}
            try{
                C3.DataBind();
                C3.DataSource = b ? null : C3.DataSource;
            }catch{}
            try
            {
                C4.DataBind();
                C4.DataSource = b ? null : C4.DataSource;
            }catch{}
            try{
                C5.DataBind();
                C5.DataSource = b ? null : C5.DataSource;
            }   
            catch { }
            ////Label1.Text = b.ToString();

            customer = GRBS.Value == "" ? "Все заказчики" : UserComboBox.getLastBlock(GRBS.Value.Replace(".datamember",""));
            
            //Label2.Text = "(" + customer + ")";
            Label3.Text = "(" + customer + ")";
            Label4.Text = "(" + customer + ")";
            Label5.Text = "(" + customer + ")";
            Label6.Text = "(" + customer + ")";
        }

        protected void C1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {

        }

        protected void C_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e,DataTable Dt)
        {
            int textWidth = 800;
            int textHeight = 12;
            Text textLabel = new Text();
            for (int i = 1; i < Dt.Columns.Count; i++)
            {
                try
                {
                    textLabel = new Text();
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(24, 250 + i * 19, textWidth, textHeight);
                    string s = Dt.Columns[i].Caption;
                    textLabel.SetTextString(s.Replace("<br>", ""));
                    e.SceneGraph.Add(textLabel);
                }
                catch { }
            }
            e.SceneGraph.Add(textLabel);
        }

        protected void C3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            //if (C3 == (UltraChart)(sender)) { C_FillSceneGraph(sender, e, C3_dt); }
            //if (C4 == (UltraChart)(sender)) { C_FillSceneGraph(sender, e, C4_dt); }
            //if (C5 == (UltraChart)(sender)) { C_FillSceneGraph(sender, e, C5_dt); }
        }

        protected void G_Init(object sender, EventArgs e)
        {

        }



    }
}
