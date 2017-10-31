using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Shared.Styles;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Infragistics.Documents.Excel;
using Graphics = System.Drawing.Graphics;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraChart.Core.Layers;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0004new.EO_0004LeninRegionNew
{
    public partial class default1 : CustomReportPage
    {
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedYear { get { return (UserParams.CustomParam("selectedYear")); } }
        private CustomParam lastYear { get { return (UserParams.CustomParam("lastYear")); } }
        private CustomParam Pokazatel { get { return (UserParams.CustomParam("Pokazatel")); } }
        private CustomParam currentWay { get { return (UserParams.CustomParam("currentWay")); } }
        private CustomParam chartDynMer { get { return (UserParams.CustomParam("chartDynMer")); } }
        string page_title = "Оценка состояния сферы «{0}»";
        string grid_title = "Состав индикатора сферы «{0}»";
        string chart1_title = "Темп прироста индикатора сферы «{0}» по отношению к предыдущему году, %";
        string chart1_title2 = "Темп прироста показателя «{0}» по отношению к предыдущему году, %";
        string chart2_title = "Структура индикатора сферы «{0}» в {1} г.";
        bool flag = true;
        int CellIndex = 0;
        object[] ColumnName;
        double[] values;
        int[] labelIndex;
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        string PokazatelName = "";
        string BN = "IE";
        int oldCellIndex = 0;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            if (BN == "IE")
            {
                UltraChart2.Height = CRHelper.GetChartHeight(504);
            }
            else 
            {
                if (BN == "FIREFOX")
                {
                    UltraChart2.Height = CRHelper.GetChartHeight(512);
                }
                else 
                {
                    UltraChart2.Height = CRHelper.GetChartHeight(500);
                
                }
            }
            WebAdyncRefreshPanel.AddLinkedRequestTrigger(Grid1);
            WebAdyncRefreshPanel.AddRefreshTarget(UltraChart1);
            WebAdyncRefreshPanel.AddRefreshTarget(UltraChart2);
            UltraChart1.Width = (int)((screen_width - 45) * 0.5);
            UltraChart2.Width = (int)((screen_width - 45) * 0.5);
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            
                base.Page_Load(sender, e);
                
                baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                if (!Page.IsPostBack)
                {
                    if (Pokazatel.Value == "")
                    {
                        Pokazatel.Value = "[Демография]";
                    }
                    PokazatelName = Pokazatel.Value.Remove(0, 1);
                    PokazatelName = PokazatelName.Remove(PokazatelName.Length - 1, 1);
                }
                //else
                {
                  //  PokazatelName = Area.SelectedValue;
                }
                if (!Page.IsPostBack)
                {
                    Area.FillDictionaryValues(AreasLoad("Areas"));
                    Area.Title = "Сфера";
                    Area.SetСheckedState(PokazatelName, true);
                    
                }
                else
                {

                    //if (Area.SelectedValue != Pokazatel.Value)
                    //{
                    //    Pokazatel.Value ="["+ Area.SelectedValue+"]";
                    //}
                    
                }
             
                PokazatelName = Area.SelectedValue;
                Session.Remove("Pokazatel");
                Session.Add("Pokazatel", "["+PokazatelName+"]");
                Pokazatel.Value = string.Format("[{0}]", PokazatelName);
                CRHelper.SaveToErrorLog(Pokazatel.Value);
                CRHelper.SaveToErrorLog(PokazatelName);
                
                flag = false;
                //PokazatelName = Pokazatel.Value.Remove(0, 1);
                //PokazatelName = PokazatelName.Remove(PokazatelName.Length - 1, 1);

                Grid1.DataBind();
                CellIndex = Grid1.Rows[0].Cells.Count - 1;
                Grid1.Width = 1210;
                Area.Width = 300;
                Grid1.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
            
                UltraChart2.ChartType = ChartType.PieChart3D;
                //selectedYear.Value = Grid1.Columns[Grid1.Columns.Count - 1].Key;
                currentWay.Value = "";
                chartDynMer.Value = "[Measures].[Оценка]";

                Label1.Text = flag.ToString();
                Label1.Text = String.Format(page_title, PokazatelName);
                Label2.Text = String.Format(grid_title, PokazatelName);
                Label3.Text = String.Format(chart1_title, PokazatelName);
                Label4.Text = String.Format(chart2_title, PokazatelName, selectedYear.Value);
                Page.Title = Label1.Text;
                HyperLink1.Text = "Оценка качества жизни населения";
                for (int i = 0; i < Grid1.Rows.Count; i += 3)
                {
                    for (int j = 1; j < Grid1.Rows[i].Cells.Count; j++)
                    {
                        if (j == 1)
                        {
                            Grid1.Rows[i + 1].Cells[j].Text = "&mdash;";
                            Grid1.Rows[i + 2].Cells[j].Text = "&mdash;";
                        }
                        if ((GetNumber(Grid1.Rows[i].Cells[j].Text) == 0))
                        {
                            if (i==0)
                            {
                                Grid1.Rows[i].Cells[j].Text = "&mdash;";
                                Grid1.Rows[i + 1].Cells[j].Text = "&mdash;";
                                Grid1.Rows[i + 2].Cells[j].Text = "&mdash;";
                            }
                            else
                            {
                                    Grid1.Rows[i].Cells[j].Text = "&mdash;";
                                    Grid1.Rows[i + 1].Cells[j].Text = "&mdash;";
                                    Grid1.Rows[i + 2].Cells[j].Text = "&mdash;";
                                    if (j != Grid1.Rows[i].Cells.Count - 1)
                                    {
                                        Grid1.Rows[i + 2].Cells[j + 1].Text = "&mdash;";
                                        Grid1.Rows[i + 1].Cells[j + 1].Text = "&mdash;";
                                    }
                            }
                        }
                        else
                        {
                        }
                    }
                }
            
                Grid1.Rows.Remove(Grid1.Rows[1]);
                Grid1.Rows[0].Cells[0].Style.Font.Bold = 1 == 1;
                Grid1.Rows[1].Cells[0].Style.Font.Bold = 1 == 1;
                Grid1.Rows[0].Cells[0].Text = "Индикатор сферы «" + PokazatelName + "»";
                Grid1.Rows[1].Cells[0].Text = "Индикатор сферы «" + PokazatelName + "»";
                for (int i = 1; i < Grid1.Columns.Count; i++)
                {
                    Grid1.Rows[0].Cells[i].Style.Font.Bold = 1 == 1;
                }
                Grid1.Height = Unit.Empty;
                Grid1.Rows[0].Activated = 1 == 1;
                Grid1.Rows[0].Cells[Grid1.Rows[0].Cells.Count - 1].Activate();
                Grid1.Rows[0].Cells[Grid1.Rows[0].Cells.Count - 1].Selected = 1 == 1;
                Grid1Manual_ActiveCellChange(Grid1.Rows[0].Cells.Count - 1, 0);
        }
        Dictionary<string, int> AreasLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }
        protected double GetNumber(string s)
        {
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    return double.Parse(s);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }

        #region Обработчики Диаграммы2
        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("UltraChart2"), "fg", dt);
            dt2.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
            dt2.Columns.Add(dt.Columns[2].ColumnName, dt.Columns[2].DataType);

            object[] o = new object[dt2.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((GetNumber(dt.Rows[i].ItemArray[1].ToString()) != 0)&&(GetNumber(dt.Rows[i].ItemArray[2].ToString()) != 0))
                {
                    o[0] = "";
                    string s = dt.Rows[i].ItemArray[0].ToString();
                    bool flag = false;
                    for (int j = 0; j < s.Length; j++)
                    {
                        o[0] = o[0].ToString() + s[j];
                        if (((j > 74) && (s[j] == ' ')) && (flag == false))
                        {
                            o[0] += "<br>";
                            flag = true;
                        }
                    }
                    o[1] = (GetNumber(dt.Rows[i].ItemArray[2].ToString()) / GetNumber(dt.Rows[i].ItemArray[1].ToString())) * GetNumber(dt.Rows[i].ItemArray[3].ToString());
                    dt2.Rows.Add(o);
                }
                else
                {

                }
                
            }
            UltraChart2.DataSource = dt2;
        }
        #endregion

        #region Обработчики грида
        DataTable GridMarks;
        protected void Grid1_DataBinding(object sender, EventArgs e)
        {
            Grid1.Rows.Clear();
            Grid1.Columns.Clear();
            Grid1.Bands.Clear();
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            GridMarks = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid2_1"), "Показатели", dt);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid2_Prop"), "dff", GridMarks);
            lastYear.Value = dt.Columns[dt.Columns.Count - 1].ColumnName;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                resDt.Columns.Add(dt.Columns[i].ColumnName);//,dt.Columns[i].DataType);

            }
            object[] o = new object[resDt.Columns.Count];
            object[] o1 = new object[resDt.Columns.Count];
            object[] o2 = new object[resDt.Columns.Count];
            o[0] = dt.Rows[0].ItemArray[0];
            o1[0] = dt.Rows[0].ItemArray[0];
            o2[0] = dt.Rows[0].ItemArray[0];
            o[1] = String.Format("{0:0.00;0;-}", GetNumber(dt.Rows[0].ItemArray[1].ToString()));
            o1[1] = 0;
            o2[1] = String.Format("{0:+0.00%;-0.00%;-}", 0); 

            for (int j = 2; j < dt.Columns.Count; j++)
            {
                o[j] = String.Format("{0:0.00;0;-}", GetNumber(dt.Rows[0].ItemArray[j].ToString()));
                o1[j] = String.Format("{0:0.00}", (GetNumber(dt.Rows[0].ItemArray[j].ToString()) - GetNumber(dt.Rows[0].ItemArray[j - 1].ToString())));//абсолютное отклонение
                o2[j] = String.Format("{0:+0.00%;-0.00%;0.00%}", (GetNumber(dt.Rows[0].ItemArray[j].ToString()) - 1));//темп прироста
            }
            resDt.Rows.Add(o);
            resDt.Rows.Add(o1);
            resDt.Rows.Add(o2);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                o = new object[resDt.Columns.Count];
                o1 = new object[resDt.Columns.Count];
                o2 = new object[resDt.Columns.Count];
                o[0] = dt.Rows[i].ItemArray[0].ToString().Split(';')[0] + ", " + GridMarks.Rows[i].ItemArray[4].ToString().ToLower() + "<br><br>Весовой коэффициент: " + GridMarks.Rows[i].ItemArray[3].ToString();
                o1[0] = dt.Rows[i].ItemArray[0].ToString().Split(';')[0] + ", " + GridMarks.Rows[i].ItemArray[4].ToString().ToLower() + "<br><br>Весовой коэффициент: " + GridMarks.Rows[i].ItemArray[3].ToString();
                o2[0] = dt.Rows[i].ItemArray[0].ToString().Split(';')[0] + ", " + GridMarks.Rows[i].ItemArray[4].ToString().ToLower() + "<br><br>Весовой коэффициент: " + GridMarks.Rows[i].ItemArray[3].ToString();
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    o[j] = String.Format("{0:0.00;0;0}", dt.Rows[i].ItemArray[j]);
                    o1[j] = String.Format("{0:0.00}", GetNumber(dt.Rows[i].ItemArray[j].ToString()) - GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()));//абсолютное отклонение
                    if (GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()) != 0)
                    {

                        o2[j] = String.Format("{0:+0.00%;-0.00%;0.00%}", (GetNumber(dt.Rows[i].ItemArray[j].ToString()) / GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()) - 1));//темп прироста
                    }
                    else
                    {
                        o2[j] = String.Format("{0:+0.00%;-0.00%;0.00%}", 0);
                    }

                }
                resDt.Rows.Add(o);
                resDt.Rows.Add(o1);
                resDt.Rows.Add(o2);

            }
            countRow = 0;

            Grid1.DataSource = resDt;

        }
        
        protected void Grid1_Click(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            Grid1.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
                CellIndex = e.Column.Index;
                try
                {
                    if (CellIndex >= 1)
                    {
                        Grid1Manual_ActiveCellChange(CellIndex, 0);
                        Grid1.Rows[0].Cells[CellIndex].Activate();
                    }
                    else
                    {
                        Grid1Manual_ActiveCellChange(1, 0);
                        Grid1.Rows[0].Cells[1].Activate();
                    }
                }
                catch { Grid1Manual_ActiveCellChange(1, 0); Grid1.Rows[0].Cells[1].Activate(); }


        }
        protected void Grid1Manual_ActiveCellChange(int CellIndex,int RowIndex)
        {
            try
            {

                 if (Grid1.Rows[RowIndex].Cells[0].Style.Font.Bold)
                 {
                     currentWay.Value = "";
                     chartDynMer.Value = "[Measures].[Оценка]";
                     Label3.Text = String.Format(chart1_title, PokazatelName);
                     UltraChart1.DataBind();
                 }
                 else
                 {
                     string s = Grid1.Rows[RowIndex].Cells[0].Text.Split('<')[0];
                     s = s.Remove(s.LastIndexOf(','));
                     currentWay.Value = ".[" + s + "]";
                     chartDynMer.Value = "[Measures].[Значение]";
                     Label3.Text = String.Format(chart1_title2, s);
                     UltraChart1.DataBind();
                 }
                 if (CellIndex != 0)
                 {
                     selectedYear.Value = Grid1.Columns[CellIndex].Key;
                     Label4.Text = String.Format(chart2_title, PokazatelName, selectedYear.Value);
                     UltraChart2.DataBind();
                 }
            }
            catch
            { }

        }
        protected void Grid1_ActiveRowChange(object sender, RowEventArgs e)
        {

        }
        protected void Grid1_ActiveCellChange(object sender, CellEventArgs e)
        {
                CellIndex = e.Cell.Column.Index;
                try
                {
                    if (CellIndex >= 1)
                    {
                        Grid1Manual_ActiveCellChange(CellIndex, e.Cell.Row.Index);
                        e.Cell.Activate();
                    }
                    else
                    {
                        Grid1Manual_ActiveCellChange(1, e.Cell.Row.Index);
                        Grid1.Rows[e.Cell.Row.Index].Cells[CellIndex].Activate();
                    }
                }
                catch
                {
                    Grid1Manual_ActiveCellChange(CellIndex, e.Cell.Row.Index);
                    Grid1.Rows[e.Cell.Row.Index].Cells[CellIndex].Activate();
                }
        }
        protected void Grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
           
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            double colWidth = 0.08;//0.62 / e.Layout.Bands[0].Columns.Count;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ##0.00");
                if (BN == "IE")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * colWidth);
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.4);
                }
                if (BN == "FIREFOX")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * colWidth);
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.4);
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * colWidth);
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.37);
                }
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
             //   e.Layout.Bands[0].Columns[i].Selected = 1 == 2;
            }
        }
        int countRow = 0;
        protected void Grid1_InitializeRow(object sender, RowEventArgs e)
        {

                if (((e.Row.Index + 2) % 3 == 0))
                {
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        double m = double.Parse(e.Row.Cells[i].Text);
                        if (m > 0)
                        {
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        if (m < 0)
                        {
                            e.Row.Cells[i].Title = "Падение относительно прошлого года";
                        }
                    }
                }
                else
                {
                    if (((e.Row.Index) % 3 != 0))
                    {
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            try
                            {
                                if (GridMarks.Rows[countRow].ItemArray[2].ToString() == "0")
                                {
                                    if (GetNumber(e.Row.Cells[i].Text.Split('%')[0]) < 0)
                                    {
                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position: left";
                                    }
                                    if (GetNumber(e.Row.Cells[i].Text.Split('%')[0]) > 0)
                                    {
                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position: left";
                                    }
                                }
                                else
                                {
                                    if (GetNumber(e.Row.Cells[i].Text.Split('%')[0]) < 0)
                                    {
                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position: left";
                                    }
                                    if (GetNumber(e.Row.Cells[i].Text.Split('%')[0]) > 0)
                                    {
                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position:  left";

                                    }
                                }
                            }
                            catch { }

                            e.Row.Cells[i].Title = "Темп прироста";

                        }
                        countRow += 1;

                    }
                    else
                    {
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].Style.Font.Bold = 1 == 1;

                        }

                    }
                }
        }
        #endregion

        #region Обработчики диаграммы1
        DataTable UltraChart1Table;
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1Table = new DataTable();
            if (currentWay.Value == "")//если выбран интегральный показатель
            {
                chartDynMer.Value = "[Measures].[Оценка]";
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("UltraChart1"), "dfd", dt);
                values=new double[dt.Columns.Count];
                labelIndex = new int[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }

                object[] o = new object[dt1.Columns.Count];
                o[0] = (GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100;
                for (int i = 1; i < dt1.Columns.Count; i++)
                {
                    o[i] = (GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100; //GetNumber(o[i - 1].ToString()) + (GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100;
                }
                dt1.Rows.Add(o);

                UltraChart1.DataSource = dt1;
                UltraChart1Table = dt1;
                double limit = 0;
                for (int i = 0; i < dt1.Columns.Count; i++)
                {
                    limit += Math.Abs(double.Parse(dt1.Rows[0].ItemArray[i].ToString()));
                }
                if (limit == 0)
                {
                    limit = 40;
                }
                UltraChart1.Axis.Y.RangeMax = limit*1.1;
                UltraChart1.Axis.Y.RangeMin = -limit*1.1;
                UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
            }
            else//если выбран подчиненный показателя
            {
                chartDynMer.Value = "[Measures].[Значение]";
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("UltraChart1"), "dfd", dt);

               /* if (dt.Columns.Count > 2)
                {
                    values = new double[dt.Columns.Count - 1];
                    labelIndex = new int[dt.Columns.Count - 1];
                }
                else
                {
                    values = new double[dt.Columns.Count];
                    labelIndex = new int[dt.Columns.Count];
                    dt1.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
                }*/
                values = new double[dt.Columns.Count - 1];
                labelIndex = new int[dt.Columns.Count - 1];
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }

                object[] o = new object[dt1.Columns.Count];
               /* if (dt.Columns.Count > 2)
                {
                    o[0] = (GetNumber(dt.Rows[0].ItemArray[1].ToString()) / GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100;
                }
                else
                {
                    o[0] = 0;
                }*/
                o[0] = (GetNumber(dt.Rows[0].ItemArray[1].ToString()) / GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100;
                for (int i = 1; i < dt1.Columns.Count; i++)
                {
                    if (GetNumber(dt.Rows[0].ItemArray[i].ToString()) != 0)
                    {
                        if (dt.Columns.Count > 2)
                        {
                            o[i] = (GetNumber(dt.Rows[0].ItemArray[i + 1].ToString()) / GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100; //GetNumber(o[i - 1].ToString()) + (GetNumber(dt.Rows[0].ItemArray[i + 1].ToString()) / GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100;
                        }
                        else 
                        {
                            o[i] = (GetNumber(dt.Rows[0].ItemArray[i].ToString()) / GetNumber(dt.Rows[0].ItemArray[i - 1].ToString()) - 1) * 100; //GetNumber(o[i - 1].ToString()) + (GetNumber(dt.Rows[0].ItemArray[i].ToString()) / GetNumber(dt.Rows[0].ItemArray[i-1].ToString()) - 1) * 100;
                        }
                    }
                    else
                    {
                        o[i] = 0;
                    }
                }
                dt1.Rows.Add(o);
                double limit = 0;
                 for (int i = 0; i < dt1.Columns.Count; i++)
                 {
                     limit += Math.Abs(double.Parse(dt1.Rows[0].ItemArray[i].ToString()));
                 }
                 if (limit == 0)
                 {
                     limit = 40; 
                 }
                 UltraChart1.Axis.Y.RangeMax = limit*1.1;
                 UltraChart1.Axis.Y.RangeMin = -limit*1.1;
                 UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                
                UltraChart1.DataSource = dt1;
                UltraChart1Table = dt1;
            }
        }
        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            
                int counterValue = 0;
                int oldHeight = 0, oldY = 0;
 
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                    {
                        Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                        if ((GetNumber(te.GetTextString()) > 1000))
                        {
                        }
                        else
                        {
                            if ((UltraChart1.DataSource != null)&&(GetNumber(te.GetTextString())<1000))
                            {
                                if (counterValue < UltraChart1Table.Rows[0].ItemArray.Length)
                                {
                                    labelIndex[counterValue] = i;
                                    values[counterValue] = GetNumber(UltraChart1Table.Rows[0].ItemArray[counterValue].ToString());
                                    string s = String.Format("{0:+0.00;-0.00;0}", GetNumber(UltraChart1Table.Rows[0].ItemArray[counterValue].ToString()));
                                    te.SetTextString(s+"%");

                                    LabelStyle ls=new LabelStyle();
                                    ls.Font=new System.Drawing.Font("Verdana",float.Parse("7,7"));
                                    ls.HorizontalAlign = StringAlignment.Center;
                                    te.SetLabelStyle(ls);
                                    counterValue += 1;
                                }
                            }
                        }

                    }
                }
                counterValue=0;
                int countScenGraph = e.SceneGraph.Count;
                int lineInitialPoint = 0;
                for (int i = 0; i < countScenGraph; i++)
                { 
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Box)
                    {
                        Infragistics.UltraChart.Core.Primitives.Box box = (Infragistics.UltraChart.Core.Primitives.Box)primitive;
                        if (box.DataPoint != null)
                        {
                            Infragistics.UltraChart.Core.Primitives.Primitive primitive1 = e.SceneGraph[labelIndex[counterValue]];
                            Infragistics.UltraChart.Core.Primitives.Text label = (Infragistics.UltraChart.Core.Primitives.Text)primitive1;

                            if (counterValue != 0)
                            {
                                if (values[counterValue-1] > 0)
                                {
                                    if (values[counterValue] > 0)
                                    {
                                        if (box.rect.Height == 0)
                                        {
                                            box.rect.Height = 2; 
                                        }
                                        box.rect.Y = oldY - box.rect.Height;
                                        oldY = box.rect.Y;
                                        
                                        oldHeight = box.rect.Height;
                                        label.bounds.Y = box.rect.Y - 9;
                                        
                                    }
                                    else
                                    {
                                        if (values[counterValue] < 0)
                                        {
                                            if (box.rect.Height == 0)
                                            {
                                                box.rect.Height = 2;
                                            }
                                            box.rect.Y = oldY;
                                            oldY = box.rect.Y;
                                            oldHeight = box.rect.Height;
                                            label.bounds.Y = box.rect.Y + box.rect.Height + 9;
                                            
                                        }
                                        else 
                                        {
                                            box.rect.Y = oldY;
                                            box.rect.Height = 1;
                                            oldY = box.rect.Y;
                                            oldHeight =0;
                                            label.bounds.Y = box.rect.Y + box.rect.Height + 9;
                                        }
                                    }
                                }
                                else
                                {
                                    if (values[counterValue] > 0)
                                    {
                                        if (box.rect.Height == 0)
                                        {
                                            box.rect.Height = 2;
                                        }
                                        box.rect.Y = oldY + oldHeight-box.rect.Height;
                                        oldY = box.rect.Y;
                                        oldHeight = box.rect.Height;
                                        label.bounds.Y = box.rect.Y - 10;
                                    }
                                    else
                                    {
                                        if (values[counterValue] < 0)
                                        {
                                            if (box.rect.Height == 0)
                                            {
                                                box.rect.Height = 2;
                                            }
                                            box.rect.Y = oldY + oldHeight;
                                            oldY = box.rect.Y;
                                            oldHeight = box.rect.Height;
                                            label.bounds.Y = box.rect.Y + box.rect.Height + 9;
                                        }
                                        else
                                        {
                                            box.rect.Y = oldY + oldHeight;
                                            box.rect.Height = 1;
                                            oldY = box.rect.Y;
                                            oldHeight = 0;
                                            label.bounds.Y = box.rect.Y + box.rect.Height + 9;
                                        }
                                    }
                                }
                                counterValue += 1;
                            }
                            else 
                            {
                                if (values[counterValue] > 0)
                                {
                                    label.bounds.Y = box.rect.Y - 15;
                                    lineInitialPoint = box.rect.Y + box.rect.Height;
                                    oldHeight = box.rect.Height;
                                }
                                else 
                                {
                                    if (values[counterValue] < 0)
                                    {
                                        label.bounds.Y = box.rect.Y + box.rect.Height + 9;
                                        lineInitialPoint = box.rect.Y;
                                        oldHeight = box.rect.Height;
                                    }
                                    else 
                                    {
                                        box.rect.Y = 200;
                                        box.rect.Height = 1;
                                        label.bounds.Y = box.rect.Y + box.rect.Height + 9;
                                        lineInitialPoint = box.rect.Y + box.rect.Height;
                                        oldHeight = 0;
                                    }
                                }
                                oldY = box.rect.Y;
                                
                                counterValue += 1;
                                Infragistics.UltraChart.Core.Primitives.Line line = new Infragistics.UltraChart.Core.Primitives.Line();
                                line.p1.X = box.rect.X;
                                line.p1.Y = lineInitialPoint;
                                line.p2.X = 700;
                                line.p2.Y = lineInitialPoint;
                                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                                line.PE.Stroke = Color.Gray;
                                line.PE.StrokeWidth = 2;
                                e.SceneGraph.Add(line);
                            }
                        }
                    }
                }
        }
        #endregion

        protected void UltraChart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        protected void Grid1_DblClick(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            Grid1.DisplayLayout.CellClickActionDefault = CellClickAction.Edit;
        }

 
        

    }
}
