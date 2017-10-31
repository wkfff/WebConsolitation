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

namespace Krista.FM.Server.Dashboards.reports.FO.FO_0002._0001
{
    public partial class _default : CustomReportPage
    {
        int lastYear = 2008;

        ArrayList al = new ArrayList();

        string ls = "";

        public DataTable GetDSForChart(string sql) 
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }

        private CustomParam p1 { get { return (UserParams.CustomParam("p1")); } }
        private CustomParam p2 { get { return (UserParams.CustomParam("p2")); } }

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

        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            ////.Text = "xz";
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            ////.Text = "xz1";
            ////.Text = cs.Axes[1].Positions[0].Members[0].UniqueName;

            string year = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            ////.Text = year+" ";
            string poly = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[9];
            ////.Text += poly+" ";//lm + cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[15 - vuschet];
            string qvart = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[11];
            ////.Text += qvart+" ";
            string mounth = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];
            ////.Text += mounth;

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

        #endregion

        string mounth;
        Dictionary<string, int> GenPeriod(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> res = new Dictionary<string, int>();
            string s = cs.Axes[0].Positions[0].Members[0].UniqueName;
            string year = s.Split('[', ']')[s.Split('[', ']').Length - 8];
            mounth = s.Split('[', ']')[s.Split('[', ']').Length - 2];
            res.Add(year, 0);
            for (int i = 0; i < cs.Axes[0].Positions.Count; i++)
            {

                s = cs.Axes[0].Positions[i].Members[0].UniqueName;
                if (s.Split('[', ']')[s.Split('[', ']').Length - 8] != year)
                {
                    year = s.Split('[', ']')[s.Split('[', ']').Length - 8];
                    try
                    {
                        AID(res, year, 0);
                    }
                    catch { }
                }
                mounth = s.Split('[', ']')[s.Split('[', ']').Length - 2];
                try
                {
                    mounth = AID(res, mounth, 1);
                }
                catch { }
            }


            return res;
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
        Dictionary<string, int> GenLevel()
        {
            Dictionary<string, int> res = new Dictionary<string, int>();

           ArrayList al =  ForMarks.Getmarks("paramLevel_");
           for (int i = 0; i < al.Count; i++)
           {
               res.Add(al[i].ToString(), 0);
           }
            return res;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            C1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 3 - 5);
            C2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 3 - 5);
            C3.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 3 - 5);
            ConfChart(C1);
            ConfChart(C3);
            ConfChart(C2);
            ////###########################################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //############################################################################
        }
        #region v ecsel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 0;i<5;i++)
                for (int j = 0; j < 5; j++)
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].Value = null;
                }
            try
            {
                e.CurrentWorksheet.Rows[0].Cells[0].Value = "";
                e.CurrentWorksheet.Rows[1].Cells[0].Value = "Доходы бюджета";
                e.CurrentWorksheet.Rows[2].Cells[0].Value = "Расходы бюджета";
                e.CurrentWorksheet.Rows[3].Cells[0].Value = "Дефицит (-), профицит (+)";
                e.CurrentWorksheet.Rows[0].Cells[1].Value = "Назначено, рублей";
                e.CurrentWorksheet.Rows[0].Cells[2].Value = "Исполнено, рублей";
                e.CurrentWorksheet.Rows[0].Cells[3].Value = "Исполнено (%)";
                e.CurrentWorksheet.Rows[0].Cells[4].Value = "Исполнено за текущий месяц, рублей";

                e.CurrentWorksheet.Rows[1].Cells[1].Value = G.Rows[1 - 1].Cells[1].Text;
                e.CurrentWorksheet.Rows[1].Cells[2].Value = G.Rows[1 - 1].Cells[2].Text;
                e.CurrentWorksheet.Rows[1].Cells[3].Value = G.Rows[1 - 1].Cells[3].Text.Split('>')[G.Rows[0].Cells[3].Text.Split('>').Length - 2].Split('<')[0];
                e.CurrentWorksheet.Rows[1].Cells[4].Value = G.Rows[1 - 1].Cells[4].Text;

                e.CurrentWorksheet.Rows[2].Cells[1].Value = G.Rows[2 - 1].Cells[1].Text;
                e.CurrentWorksheet.Rows[2].Cells[2].Value = G.Rows[2 - 1].Cells[2].Text;
                e.CurrentWorksheet.Rows[2].Cells[3].Value = G.Rows[2 - 1].Cells[3].Text.Split('>')[G.Rows[1].Cells[3].Text.Split('>').Length - 2].Split('<')[0];
                e.CurrentWorksheet.Columns[3].Width = 100 * 50;

                e.CurrentWorksheet.Rows[2].Cells[4].Value = G.Rows[2 - 1].Cells[4].Text;

                e.CurrentWorksheet.Rows[3].Cells[1].Value = G.Rows[3 - 1].Cells[1].Text;
                e.CurrentWorksheet.Rows[3].Cells[2].Value = G.Rows[3 - 1].Cells[2].Text;
            }
            catch { }
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
        }




        private void ExcelExportButton_Click(object sender, EventArgs e)
        {           
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }

        #endregion

        CellSet cs1;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            //RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");

            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(GenPeriod("LD"));
                ComboYear.SetСheckedState(mounth, 1 == 1);
                ComboYear.ParentSelect = 1 == 2;
                
                ComboLevel.FillDictionaryValues(GenLevel());
                ComboYear.Width = 200;
                ComboLevel.Width = 300;
                ComboYear.ShowSelectedValue = 1 == 2;                
            }
            try
            {
                p1.Value = "[Период__Период].[Период__Период].[Год].[" + DelLastsChar(ComboYear.SelectedNode.Parent.Text, ' ') + "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(ComboYear.SelectedNode.Text, ' '))).ToString() + "].[Квартал " + CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(DelLastsChar(ComboYear.SelectedNode.Text, ' '))).ToString() + "].[" + DelLastsChar(ComboYear.SelectedNode.Text, ' ') + "]";
                p2.Value = ComboLevel.SelectedValue;
                G.DataBind();

            }
            catch { }

            o_O____grid(G);
            C1.DataBind();
            C2.DataBind();
            C3.DataBind();

            Page.Title = PageTitle.Text = "Ежемесячный отчет об исполнении бюджета муниципального образования за " + ComboYear.SelectedValue.ToLower() + " " + ComboYear.SelectedNode.Parent.Text + " года";



            cs1 = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("C1"));
            //RegionSettingsHelper.Instance.SetWorkingRegion("Gubkinski");
        }

        object[] getO(object[] o)
        {
            for (int i = 0; o.Length > i; i++)
            {
                if (o[i] == DBNull.Value)
                {
                    o[i] = System.Decimal.Zero;
                }
            }
            return o;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("G0");
            DataTable dtRes = new DataTable();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dtRes.Columns.Add(dt.Columns[i].Caption, dt.Columns[i].DataType);
            }

            dtRes.Rows.Add(getO(dt.Rows[0].ItemArray));
            dtRes.Rows.Add(getO(GetDSForChart("G1").Rows[0].ItemArray));
            dtRes.Rows.Add(getO(GetDSForChart("G2").Rows[0].ItemArray));
            G.DataSource = dtRes;
        }

        protected void G_DataBound(object sender, EventArgs e)
        {

        }

        protected void Chart_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        DataTable GlobalDtRes;
        protected void Chart_DataBinding(string q)
        {
            try
            {                
                DataTable dt = GetDSForChart(q);
                System.Decimal sum = 0;             
                for (int i = 0; i < dt.Rows.Count; i++)
                {                    
                    sum += (System.Decimal)(dt.Rows[i].ItemArray[1]);
                }                
                DataTable resDt = new DataTable();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    resDt.Columns.Add(dt.Columns[i].Caption, dt.Columns[i].DataType);
                }                
                string othterString = "";
                System.Decimal otherSum = 0;
                object[] o;
                for (int i = 0; i < dt.Rows.Count; i++)
                {                    
                    if (((System.Decimal)(dt.Rows[i].ItemArray[1]) / sum) > (System.Decimal)(0.01))
                    {
                        
                        o = new object[2];
                        string res = dt.Rows[i].ItemArray[0].ToString().Remove(1) + dt.Rows[i].ItemArray[0].ToString().Remove(0, 1).ToLower();

                        for (int j = 1; j < res.Length / 30; j++)
                        {
                            int k;
                            for (k = j * 30; k < res.Length; k++)
                            {
                                if (res[k] == ' ')
                                {
                                    break;
                                }
                            }

                            res = !(k == res.Length) ? res.Insert(k, "<br>") : res;
                        }

                        o[0] = res;
                        o[1] = dt.Rows[i].ItemArray[1];                        
                        resDt.Rows.Add(o);
                    }
                    else
                    {
                        //string.Format("{0:### ### ##0.##",dt.Rows[i].ItemArray[1])
                        string newO = dt.Rows[i].ItemArray[0].ToString().Remove(1).ToUpper() + dt.Rows[i].ItemArray[0].ToString().Remove(0, 1).ToLower()
                            + ", <b>" + string.Format("{0:### ### ##0.##}", System.Decimal.Parse(dt.Rows[i].ItemArray[1].ToString())) + "</b> рублей (" + Math.Round((System.Decimal)(dt.Rows[i].ItemArray[1]) * 100 / sum, 2).ToString() + "%)" + "<br/>";
                        for (int j = 1; j < newO.Length/50; j++)
                        {
                            int k;                            
                            for (k = j * 50; k < newO.Length; k++)
                            {
                                if (newO[k] == ' ')
                                {
                                    break;
                                }
                            }

                            newO = !(k ==  newO.Length)?newO.Insert(k, "<br>"):newO;
                        }

                            othterString += newO;
                        otherSum += (System.Decimal)(dt.Rows[i].ItemArray[1]);
                    }
                }                
                o = new object[2];
                o[0] = othterString + " <b>Всего прочих</b> ";
                o[1] = otherSum;
                resDt.Rows.Add(o);
                GlobalDtRes = resDt;
            }
            catch { }
        }
        DataTable c2dt;
        protected void C2_DataBinding(object sender, EventArgs e)
        {
            Chart_DataBinding("C2");
            C2.DataSource = GlobalDtRes;
            c2dt = GlobalDtRes;            
        }
        DataTable c3dt;
        protected void C3_DataBinding(object sender, EventArgs e)
        {
            Chart_DataBinding("C3");
            C3.DataSource = GlobalDtRes;
            c3dt = GlobalDtRes;            
        }

        protected void o_O____grid(UltraWebGrid G)
        {
            G.Rows[0].Cells[0].Text = String.Format("<a target=\"_blank\" href = \"{0}\">Доходы бюджета</a>", RegionSettingsHelper.Instance.GetPropertyValue("url_1"));
            G.Rows[1].Cells[0].Text = String.Format("<a target=\"_blank\" href = \"{0}\">Расходы бюджета</a>", RegionSettingsHelper.Instance.GetPropertyValue("url_2"));
            G.Rows[2].Cells[0].Text = "Дефицит (-), профицит (+)";
            G.Rows[2].Cells[3].Text = "";
            G.Rows[2].Cells[4].Text = "";
            G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            try
            {
                System.Double V1 = System.Double.Parse(G.Rows[0].Cells[3].Value.ToString()) * 100;                
                Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)Ga1.Gauges[0]).Scales[0];
                
                scale.Markers[0].Value = V1>100?100:V1;                
                Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement be = 
                (Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);                
                be.ColorStops.Clear();                
                Color col ;
                if (V1>80)
                {
                     col = Color.Green;
                }
                else
                {
                    if ( V1<50)
                    {
                         col = Color.Red;
                    }
                    else
                    {
                        col = Color.Yellow;
                    }

                }
                be.ColorStops.Add(col,0);                
                be.ColorStops.Add(col,1);                
                Random r11 = new Random();                
                string r1 = V1.ToString();                
                Ga1.DeploymentScenario.FilePath = "../../../../TemporaryImages";                
                Ga1.DeploymentScenario.ImageURL = "../../../../TemporaryImages/" + r1 + ".png";                
                G.Rows[0].Cells[3].Text = "<div style=\"FLOAT: left;\">" + "<img src =" + '"' + "../../../../TemporaryImages/" + r1 + ".png" + '"' + "/></div>" + "<div style=\"position: relative; top: 5px\">" + Math.Round(V1, 2).ToString() + "%</div>";
            }
            catch { }
            try
            {
                System.Double V1 = System.Double.Parse(G.Rows[1].Cells[3].Value.ToString()) * 100;
                Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)Ga2.Gauges[0]).Scales[1];
                scale.Markers[0].Value = V1 > 100 ? 100 : V1;                
                Random r11 = new Random(1);
                Color col;
                Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement be =
(Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);   
                if (V1 > 80)
                {
                    col = Color.Green;
                }
                else
                {
                    if (V1 < 50)
                    {
                        col = Color.Red;
                    }
                    else
                    {
                        col = Color.Yellow;
                    }

                }
                be.ColorStops.Clear();    
                be.ColorStops.Add(col, 0);
                be.ColorStops.Add(col, 1);    
                string r1 = V1.ToString();
                Ga2.DeploymentScenario.FilePath = "../../../../TemporaryImages";
                Ga2.DeploymentScenario.ImageURL = "../../../../TemporaryImages/" + r1 + ".png";
                G.Columns[3].CellStyle.VerticalAlign = VerticalAlign.Middle;
                G.Rows[1].Cells[3].Text = "<div style=\"FLOAT: left;\">" + "<img src =" + '"' + "../../../../TemporaryImages/" + r1 + ".png" + '"' + "/></div>" + "<div style=\"position: relative; top: 5px\">" + Math.Round(V1, 2).ToString() + "%</div>";
            }
            catch { }
            G.Height = 100;
        }


        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = 120;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ### ### ##0.00");
            }
            e.Layout.Bands[0].Columns[3].Width = 165;
            e.Layout.Bands[0].Columns[4].Width = 165;
            e.Layout.Bands[0].Columns[0].Width = 160;
            e.Layout.Bands[0].Columns[0].Header.Caption = "";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Назначено, рублей";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Исполнено, рублей";
            e.Layout.Bands[0].Columns[3].Header.Caption = "Исполнено (%)";
            e.Layout.Bands[0].Columns[4].Header.Caption = "Исполнено за текущий месяц, рублей";

        }

        protected void ConfChart(UltraChart C)
        {
            C.Legend.SpanPercentage = 45;
            C.Height = 480;
            C.Border.Color = Color.Transparent;
            C.PieChart.OthersCategoryPercent = 0;
            C.PieChart.OthersCategoryText = "Прочие";
            C.ColorModel.ModelStyle = ColorModels.PureRandom;
            C.InvalidDataReceived += C1_InvalidDataReceived;
            offset = 275  ;

        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                try
                {
                    if (e.Row.Cells[i].Value.ToString() == "0")
                    {
                        e.Row.Cells[i].Text = "";
                    }
                    else
                    {
                        if (i != 3) { e.Row.Cells[i].Text = AddZero(AddSpace(e.Row.Cells[i].Text, ',')); };
                    }
                }
                catch { try { e.Row.Cells[i].Text = ""; } catch { } }



            }
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

        protected void C1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        protected void C1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int textWidth = 400;
            int textHeight = 12;            
            Text textLabel;
            for (int i = 0; i <  c1dt.Rows.Count-1; i++)
            {
                try
                {
                    textLabel = new Text();
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(24, offset + i * 19, textWidth, textHeight);
                    string s = c1dt.Rows[i].ItemArray[0].ToString();
                    if (s.Length > 64)
                    {
                        s = s.Remove(60) + "...";
                        
                    }
                    textLabel.SetTextString(s.Replace("<br>", ""));
                    e.SceneGraph.Add(textLabel);
                }
                catch { }
            }
            textLabel = new Text();
            textLabel.PE.Fill = Color.Black;
            textLabel.bounds = new Rectangle(24, offset + (c1dt.Rows.Count - 1) * 19, textWidth, textHeight);
            textLabel.SetTextString("Прочие");
            e.SceneGraph.Add(textLabel);
        }
        DataTable c1dt;
        protected void C1_DataBinding(object sender, EventArgs e)
        {
            Chart_DataBinding("C1");
            C1.DataSource = GlobalDtRes;
            c1dt = GlobalDtRes;
        }        
        protected void C2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int textWidth = 400;
            int textHeight = 12;            
            Text textLabel;
            for (int i = 0; i < c2dt.Rows.Count - 1; i++)
            {
                try
                {
                    textLabel = new Text();
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(24, offset + i * 19, textWidth, textHeight);
                    string s = c2dt.Rows[i].ItemArray[0].ToString();
                    if (s.Length > 64)
                    {
                        s = s.Remove(60) + "...";
                    }
                    textLabel.SetTextString(s.Replace("<br>", ""));
                    e.SceneGraph.Add(textLabel);
                }
                catch { }
            }
            textLabel = new Text();
            textLabel.PE.Fill = Color.Black;
            textLabel.bounds = new Rectangle(24, offset + (c2dt.Rows.Count - 1) * 19, textWidth, textHeight);
            textLabel.SetTextString("Прочие");
            e.SceneGraph.Add(textLabel);
        }

        protected void C3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {   
            int textWidth = 400;
            int textHeight = 12;
            Text textLabel;
            for (int i = 0; i < c3dt.Rows.Count - 1; i++)
            {
                try
                {
                    textLabel = new Text();
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(24, offset + i * 19, textWidth, textHeight);
                    string s = c3dt.Rows[i].ItemArray[0].ToString();
                    if (s.Length>64)
                    {
                        s = s.Remove(60)+"...";
                    }
                    textLabel.SetTextString(s.Replace("<br>",""));
                    e.SceneGraph.Add(textLabel);
                }
                catch { }
            }
            textLabel = new Text();
            textLabel.PE.Fill = Color.Black;
            textLabel.bounds = new Rectangle(24, offset + (c3dt.Rows.Count - 1) * 19, textWidth, textHeight);
            textLabel.SetTextString("Прочие");
            e.SceneGraph.Add(textLabel);
        }

        protected void ComboYear_Load(object sender, EventArgs e)
        {

        }

    }
}
