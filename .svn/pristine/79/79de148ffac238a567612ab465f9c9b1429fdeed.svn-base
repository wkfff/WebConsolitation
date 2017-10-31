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
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebNavigator;
using System.Globalization;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Reports.Report.Section;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0004new.EO_0004LeninRegionNew1
{
    public partial class default1 : CustomReportPage
    {
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedYear { get { return (UserParams.CustomParam("selectedYear")); } }
        private CustomParam lastYear { get { return (UserParams.CustomParam("lastYear")); } }
        private CustomParam Pokazatel { get { return (UserParams.CustomParam("Pokazatel", true)); } }
        private CustomParam currentWay { get { return (UserParams.CustomParam("currentWay")); } }
        private CustomParam chartDynMer { get { return (UserParams.CustomParam("chartDynMer")); } }
        private CustomParam dataBase { get { return (UserParams.CustomParam("dataBase")); } }
        private CustomParam pokPrefix { get { return (UserParams.CustomParam("pokPrefix")); } }
        string page_title = "Оценка основных сфер жизни населения";
        string page_sub_title = "Анализ состояния сферы «{0}», лежащей в основе расчета интегрального показателя уровня жизни населения, {1}";
        string grid_title = "Состав индикатора сферы «{0}»";
        string chart1_title = "Темп прироста индикатора сферы «{0}» по отношению к базовому году, %";
        string chart1_title2 = "Динамика показателя «{0}», {1}";
        string chart2_title = "Структура индикатора сферы «{0}» в {1} г.";
        bool pokazatelType = false;
        bool flag = true;
        int CellIndex = 0;
        double[] values;
        int[] labelIndex;
        private object[] ColumnNames;
        private GridHeaderLayout headerLayout;
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
                UltraChart2.Height = CRHelper.GetChartHeight(527);
            }
            else 
            {
                if (BN == "FIREFOX")
                {
                    UltraChart2.Height = CRHelper.GetChartHeight(531);
                }
                else 
                {
                    UltraChart2.Height = CRHelper.GetChartHeight(506);
                }
            }
            WebAdyncRefreshPanel.AddLinkedRequestTrigger(Grid1);
            WebAdyncRefreshPanel.AddRefreshTarget(UltraChart1);
            WebAdyncRefreshPanel.AddRefreshTarget(UltraChart2);
            UltraChart1.Width = (int)((screen_width - 45) * 0.5);
            UltraChart2.Width = (int)((screen_width - 45) * 0.5);
            UltraChart1.Border.Color = Color.Transparent;
            UltraChart2.Border.Color = Color.Transparent;
            ComboRegion.Width = 450;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (Pokazatel.Value == "")
                {
                    Pokazatel.Value = "[Демография]";
                }
                PokazatelName = Pokazatel.Value.Remove(0, 1);
                PokazatelName = PokazatelName.Remove(PokazatelName.Length - 1, 1);
                if (!Page.IsPostBack)
                {
                    ComboRegion.Title = "Территория";
                    ComboRegion.FillDictionaryValues(RegionsLoad("regions"));
                    ComboRegion.ParentSelect = true;
                    ComboRegion.SetСheckedState(ComboRegion.GetRootNodesName(0), true);
                    if (ComboRegion.SelectedNode.Level == 0)
                    {
                        baseRegion.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Ханты-Мансийский автономный округ].DATAMEMBER";
                        dataBase.Value = "[ЭО_КЖН_Ленинградская область]";
                        pokPrefix.Value = "[КЖН__Ленинградская область].[КЖН__Ленинградская область]";
                    }
                    else
                    {
                        baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра].[" + ComboRegion.SelectedValue + "]";
                        dataBase.Value = "[ЭО_КЖН_Ленинградская область МО]";

                        pokPrefix.Value = "[КЖН__Ленинградская область МО].[КЖН__Ленинградская область МО]";
                    }
                    Area.FillDictionaryValues(AreasLoad("Areas"));
                    Area.Title = "Сфера";
                    Area.SetСheckedState(PokazatelName, true);
                }
                else
                {
                    if (Area.SelectedValue != Pokazatel.Value)
                    {
                        Pokazatel.Value = "[" + Area.SelectedValue + "]";
                    }

                }
                flag = false;
                PokazatelName = Pokazatel.Value.Remove(0, 1);
                PokazatelName = PokazatelName.Remove(PokazatelName.Length - 1, 1);
                if (ComboRegion.SelectedNode.Level == 0)
                {
                    baseRegion.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Ханты-Мансийский автономный округ].DATAMEMBER";
                    dataBase.Value = "[ЭО_КЖН_Ленинградская область]";
                    pokPrefix.Value = "[КЖН__Ленинградская область].[КЖН__Ленинградская область]";
                }
                else
                {
                    baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра].[" + ComboRegion.SelectedValue + "]";
                    dataBase.Value = "[ЭО_КЖН_Ленинградская область МО]";

                    pokPrefix.Value = "[КЖН__Ленинградская область МО].[КЖН__Ленинградская область МО]";
                }
                headerLayout = new GridHeaderLayout(Grid1);
                Grid1.DataBind();
                CellIndex = Grid1.Rows[0].Cells.Count - 1;
                Grid1.Width = 1210;
                Area.Width = 300;
                Grid1.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
                currentWay.Value = "";
                chartDynMer.Value = "[Measures].[Оценка]";
                Label1.Text = page_title;
                Label2.Text = String.Format(grid_title, PokazatelName);
                Label3.Text = String.Format(chart1_title, PokazatelName);
                Label4.Text = String.Format(chart2_title, PokazatelName, selectedYear.Value);
                Label5.Text = String.Format(page_sub_title, PokazatelName, ComboRegion.SelectedValue);
                Page.Title = Label1.Text;
                for (int i = 0; i < Grid1.Rows.Count; i += 3)
                {
                    if (i > 1)
                    {
                        Grid1.Rows[i].Cells[0].RowSpan = 2;
                        Grid1.Rows[i].Cells[0].Style.BorderDetails.WidthTop = 1;
                        Grid1.Rows[i].Cells[0].Style.BorderDetails.WidthBottom = 0;
                        Grid1.Rows[i + 1].Cells[0].Style.BorderDetails.WidthBottom = 0;
                        Grid1.Rows[i + 2].Cells[0].Style.BorderDetails.WidthBottom = 1;
                    }

                    for (int j = 1; j < Grid1.Rows[i].Cells.Count; j++)
                    {
                        if (j == 1)
                        {
                            Grid1.Rows[i + 1].Cells[j].Text = "-";
                            Grid1.Rows[i + 2].Cells[j].Text = "-";
                        }
                        if ((GetNumber(Grid1.Rows[i].Cells[j].Text) == 0))
                        {
                            if (i == 0)
                            {
                                Grid1.Rows[i].Cells[j].Text = "-";
                                Grid1.Rows[i + 1].Cells[j].Text = "-";
                                Grid1.Rows[i + 2].Cells[j].Text = "-";
                            }
                            else
                            {
                                Grid1.Rows[i].Cells[j].Text = "-";
                                Grid1.Rows[i + 1].Cells[j].Text = "-";
                                Grid1.Rows[i + 2].Cells[j].Text = "-";
                                if (j != Grid1.Rows[i].Cells.Count - 1)
                                {
                                    Grid1.Rows[i + 2].Cells[j + 1].Text = "-";
                                    Grid1.Rows[i + 1].Cells[j + 1].Text = "-";
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                }
                Grid1.Rows.Remove(Grid1.Rows[1]);
                Grid1.Rows[0].Cells[0].RowSpan = 2;
                Grid1.Rows[1].Cells[0].Style.BorderDetails.WidthTop = 0;
                Grid1.Rows[1].Cells[0].Style.BorderDetails.WidthBottom = 1;
                Grid1.Rows[0].Cells[0].Style.Font.Bold = 1 == 1;
                Grid1.Rows[1].Cells[0].Style.Font.Bold = 1 == 1;
                Grid1.Rows[0].Cells[0].Text = "Индикатор сферы «" + PokazatelName + "»";
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
            catch (Exception e_)
            {
                CRHelper.SaveToErrorLog(e_.HelpLink + "\n" + e_.InnerException + "\n" + e_.Message + "\n" + e_.Source + "\n" + e_.StackTrace + "\n" + e_.TargetSite);
                Label1.Text = "";
                Label2.Text = "";
                Label3.Text = "";
                Label4.Text = "";
                Label5.Text = "";
            }
        }
        Dictionary<string, int> AreasLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Регионы", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(dt.Rows[0].ItemArray[0].ToString(), 0);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i].ItemArray[0].ToString(), 1);
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
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("UltraChart2"), "fg", dt);
            if (dt.Rows.Count != 0)
            {
                dt2.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
                dt2.Columns.Add(dt.Columns[2].ColumnName, dt.Columns[2].DataType);

                object[] o = new object[dt2.Columns.Count];
                bool flag1 = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((GetNumber(dt.Rows[i].ItemArray[1].ToString()) != 0) && (GetNumber(dt.Rows[i].ItemArray[2].ToString()) != 0))
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
                        if (Convert.ToDouble(dt.Rows[i].ItemArray[dt.Rows[i].ItemArray.Length - 1]) == 0)
                        {
                            o[1] = (GetNumber(dt.Rows[i].ItemArray[2].ToString()) / GetNumber(dt.Rows[i].ItemArray[1].ToString())) * GetNumber(dt.Rows[i].ItemArray[3].ToString());
                        }
                        else
                        {
                            o[1] = (GetNumber(dt.Rows[i].ItemArray[1].ToString()) / GetNumber(dt.Rows[i].ItemArray[2].ToString())) * GetNumber(dt.Rows[i].ItemArray[3].ToString());
                        }
                        if (Convert.ToDouble(o[1]) != 0)
                        {
                            flag1 = true;
                        }
                        dt2.Rows.Add(o);
                    }
                    else
                    {

                    }

                }
                if (flag1)
                {
                    UltraChart2.DataSource = dt2;
                }
                else
                {
                    UltraChart2.DataSource = null;
                }
            }
            else
            {
                UltraChart2.DataSource = null;
            
            }
        }

        protected void UltraChart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (UltraChart2.DataSource != null)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                    {
                        Infragistics.UltraChart.Core.Primitives.Text text = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                        if (text.Path.Contains("Legend"))
                        {
                            if (text.GetTextString().Contains("<br>"))
                            {
                                string s = text.GetTextString().Replace("<br>", "");
                                text.SetTextString(s);
                            }
                            text.bounds.Width = 540;
                        }
                    }
                }
            }
        }
        #endregion

        #region Обработчики грида
        DataTable GridMarks;
        protected void Grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable resDt = new DataTable();
                GridMarks = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid2_1"), "Показатели", dt);
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid2_Prop"), "dff", GridMarks);
                lastYear.Value = dt.Columns[dt.Columns.Count - 1].ColumnName;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    resDt.Columns.Add(dt.Columns[i].ColumnName);//,dt.Columns[i].DataType);

                }
                object[] o = new object[resDt.Columns.Count];
                object[] o1 = new object[resDt.Columns.Count];
                object[] o2 = new object[resDt.Columns.Count];
                o[0] = dt.Rows[0].ItemArray[0];
                o1[0] = "";
                o2[0] = "";
                o[1] = String.Format("{0:0.0000;0.0000;-}", GetNumber(dt.Rows[0].ItemArray[1].ToString()));
                o1[1] = 0;
                o2[1] = String.Format("{0:+0.00%;-0.00%;-}", 0);

                for (int j = 2; j < dt.Columns.Count; j++)
                {
                    o[j] = String.Format("{0:0.0000;0.00;-}", GetNumber(dt.Rows[0].ItemArray[j].ToString()));
                    double m = GetNumber(dt.Rows[0].ItemArray[j].ToString()) - GetNumber(dt.Rows[0].ItemArray[j - 1].ToString());
                    o1[j] = String.Format("{0:0.00}", m);//абсолютное отклонение
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
                    o[0] = dt.Rows[i].ItemArray[0].ToString().Split(';')[0] + ", " + GridMarks.Rows[i].ItemArray[4].ToString().ToLower();// + "\nВесовой коэффициент: " + GridMarks.Rows[i].ItemArray[3].ToString();
                    o1[0] = "";
                    //o2[0] = dt.Rows[i].ItemArray[0].ToString().Split(';')[0] + ", " + GridMarks.Rows[i].ItemArray[4].ToString().ToLower() + "\nВесовой коэффициент: " + GridMarks.Rows[i].ItemArray[3].ToString();
                    o2[0] = "Весовой коэффициент: " + GridMarks.Rows[i].ItemArray[3].ToString();
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        o[j] = String.Format("{0:0.00}", dt.Rows[i].ItemArray[j]);
                        o1[j] = String.Format("{0:0.00}", GetNumber(dt.Rows[i].ItemArray[j].ToString()) - GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()));//абсолютное отклонение
                        if (GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()) != 0)
                        {
                            if (GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()) < 0)
                            {
                                o2[j] = String.Format("{0:+0.00%;-0.00%;0.00%}", -(GetNumber(dt.Rows[i].ItemArray[j].ToString()) / GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()) - 1));//темп прироста
                            }
                            else 
                            {
                                o2[j] = String.Format("{0:+0.00%;-0.00%;0.00%}", (GetNumber(dt.Rows[i].ItemArray[j].ToString()) / GetNumber(dt.Rows[i].ItemArray[j - 1].ToString()) - 1));//темп прироста
                            }
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
            catch { }
        }
        
        protected void Grid1_Click(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
                CellIndex = e.Column.Index;
              /*  try
                {*/
                    if (CellIndex >= 1)
                    {
                        Grid1Manual_ActiveCellChange(CellIndex, e.Row.Index);
                    }
                    else
                    {
                        Grid1Manual_ActiveCellChange(1, e.Row.Index);
                    }
                //}
               /* catch { Grid1Manual_ActiveCellChange(1, 0); 
                }*/


        }
        protected void Grid1Manual_ActiveCellChange(int CellIndex,int RowIndex)
        {
             
             if ((RowIndex==0)||(RowIndex==1))
             {
                 currentWay.Value = "";
                 chartDynMer.Value = "[Measures].[Оценка]";
                 Label3.Text = String.Format(chart1_title, PokazatelName);
                 pokazatelType = false;
                 UltraChart1.DataBind();
             }
             else
             {
                 int k = RowIndex;
                 if (Grid1.Rows[RowIndex].Cells[0].Text.StartsWith("Весовой"))
                 {
                     k = RowIndex - 2;
                 }
                 if (Grid1.Rows[RowIndex].Cells[0].Text=="")
                 {
                     k = RowIndex - 1;
                 }

                 string s = Grid1.Rows[k].Cells[0].Text.Split('<')[0];
                 s = s.Remove(s.LastIndexOf(','));
                 currentWay.Value = ".[" + s + "]";
                 chartDynMer.Value = "[Measures].[Значение]";
                 string buf = Grid1.Rows[k].Cells[0].Text.Split('<')[0];
                 
                 Label3.Text = String.Format(chart1_title2, buf.Remove(buf.LastIndexOf(',')),buf.Split(',')[buf.Split(',').Length-1]);
                 UltraChart1.DataBind();
                 UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:0.00></b>," + Grid1.Rows[k].Cells[0].Text.Split('<')[0].Split(',')[Grid1.Rows[k].Cells[0].Text.Split('<')[0].Split(',').Length-1];                     
             }
             if (CellIndex != 0)
             {
                 selectedYear.Value = Grid1.Columns[CellIndex].Key;
                 Label4.Text = String.Format(chart2_title, PokazatelName, selectedYear.Value);
                 UltraChart2.DataBind();
             }
        }
        protected void Grid1_ActiveRowChange(object sender, RowEventArgs e)
        {

        }
        protected void Grid1_ActiveCellChange(object sender, CellEventArgs e)
        {
            CellIndex = e.Cell.Column.Index;
           /* try
            {*/
                if (CellIndex >= 1)
                {
                    Grid1Manual_ActiveCellChange(CellIndex, e.Cell.Row.Index);
                    e.Cell.Activate();
                }
                else
                {
                    Grid1Manual_ActiveCellChange(1, e.Cell.Row.Index);
                }
            //}
           /* catch
            {
                Grid1Manual_ActiveCellChange(CellIndex, e.Cell.Row.Index);
            }*/
            
        }
        protected void Grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
           // e.Layout.Bands[0].Columns[0].MergeCells = true;

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
            }
            GridHeaderCell header = headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                header = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
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
                        e.Row.Cells[i].Title = "Снижение относительно прошлого года";
                    }
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
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

                        e.Row.Cells[i].Title = "Темп прироста к предыдущему году";

                    }
                    countRow += 1;

                }
                else
                {
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Style.Font.Bold = 1 == 1;
                        e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
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
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("UltraChart1"), "dfd", dt);
                values = new double[dt.Columns.Count];
                labelIndex = new int[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                    UltraChart1Table.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }
                
                ColumnNames = new object[dt1.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ColumnNames[i] = dt.Columns[i].ColumnName;
                }
                object[] o = new object[dt1.Columns.Count];
                object[] o1 = new object[dt1.Columns.Count];
                o[0] = (GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100;
                o1[0] = (GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100;
                dt1.Columns[0].ColumnName = "Значение индикатора сферы <b>" +String.Format("{0:0.0000}",Convert.ToDouble(dt.Rows[0].ItemArray[0])) + "</b><br>" + "Темп прироста к предыдущему году <b>" + Math.Round(((GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100), 2).ToString() + "%</b><br>Темп прироста к базовому году <b>" + Math.Round(((GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100), 2).ToString() + "%</b>";
                
                for (int i = 1; i < dt1.Columns.Count; i++)
                {
                    o[i] = (GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100; //GetNumber(o[i - 1].ToString()) + (GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100;
                    double increase = 0;
                    for (int j = i; j >= 0; j--)
                    {
                        increase += (GetNumber(dt.Rows[0].ItemArray[j].ToString()) - 1) * 100;
                    }
                    
                    o1[i] = increase;
                    dt1.Columns[i].ColumnName = "Значение индикатора сферы <b>" + String.Format("{0:0.0000}",Convert.ToDouble(dt.Rows[0].ItemArray[i])) + "</b><br>" + "Темп прироста к предыдущему году <b>" + Math.Round(Convert.ToDouble(o[i]), 2).ToString() + "%</b><br>Темп прироста к базовому году <b>" + Math.Round(increase, 2).ToString() + "%</b>";
                    o[i] = increase;
                }
                dt1.Rows.Add(o);
                UltraChart1Table.Rows.Add(o1);
                UltraChart1.DataSource = dt1;
                double max = GetNumber(o[0].ToString());
                double min = GetNumber(o[0].ToString());
                for (int i = 1; i < dt1.Columns.Count; i++)
                {
                    if (max < GetNumber(o[i].ToString()))
                    {
                        max = GetNumber(o[i].ToString());
                    }
                    if (min > GetNumber(o[i].ToString()))
                    {
                        min = GetNumber(o[i].ToString());
                    }
                }
              
                
                #region Настройка диаграммы
                    UltraChart1.ChartType = ChartType.LineChart;
                    LineAppearance lineApp=new LineAppearance();
                    lineApp.Thickness=0;
                    lineApp.IconAppearance.Icon=SymbolIcon.Square;
                    lineApp.IconAppearance.IconSize=SymbolIconSize.Large;
                    lineApp.IconAppearance.PE.Fill=Color.Transparent;
                    lineApp.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
                    lineApp.IconAppearance.PE.Stroke = Color.Transparent;
                    lineApp.IconAppearance.PE.StrokeOpacity = 0;
                    lineApp.IconAppearance.PE.FillGradientStyle = GradientStyle.None;
                    lineApp.IconAppearance.PE.FillOpacity = 0;
                    lineApp.IconAppearance.PE.FillStopColor = Color.Transparent;
                    lineApp.IconAppearance.PE.FillStopOpacity = 0;
                    UltraChart1.LineChart.LineAppearances.Add(lineApp);
                    ChartTextAppearance textApp=new ChartTextAppearance();
                    textApp.Visible=true;
                    textApp.Column=-2;
                    textApp.Row=-2;
                    textApp.ItemFormatString = "<DATA_VALUE:0.00>%";
                    textApp.VerticalAlign = StringAlignment.Far;
                    UltraChart1.LineChart.ChartText.Add(textApp);
                    UltraChart1.Axis.Y.Labels.Visible = false;
                    UltraChart1.Axis.Y.LineThickness = 0;
                    UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
                    UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
                #endregion

                    UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                    UltraChart1.Axis.Y.RangeMax = max + 1;
                    UltraChart1.Axis.Y.RangeMin = min - 1;
            }
            else
            {
                chartDynMer.Value = "[Measures].[Значение]";
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("UltraChart1"), "dfd", dt);
                if (dt.Rows.Count < 1)
                {
                    UltraChart1.DataSource = null;
                }
                else
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt1.Columns.Add("_"+i.ToString()+String.Format("{0:0.##}", (decimal)GetNumber(dt.Rows[0].ItemArray[i].ToString())), dt.Columns[i].DataType);
                    }
                    ColumnNames = new object[dt1.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ColumnNames[i] = dt.Columns[i].ColumnName;
                    }
                    object[] o = new object[dt1.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        o[i] = dt.Rows[0].ItemArray[i];
                    }
                    dt1.Rows.Add(o);
                    UltraChart1.DataSource = dt1;
                    UltraChart1Table = dt1;
                    double max = GetNumber(dt.Rows[0].ItemArray[0].ToString()), min = GetNumber(dt.Rows[0].ItemArray[0].ToString());
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        if (GetNumber(dt.Rows[0].ItemArray[i].ToString()) < min)
                        {
                            min = GetNumber(dt.Rows[0].ItemArray[i].ToString());
                        }
                        if (GetNumber(dt.Rows[0].ItemArray[i].ToString()) > max)
                        {
                            max = GetNumber(dt.Rows[0].ItemArray[i].ToString());
                        }
                    }
                   
                    
                    #region Настройка диаграммы
                        UltraChart1.ChartType = ChartType.ColumnChart;
                        UltraChart1.ResetColumnChart();
                        UltraChart1.ColorModel.ModelStyle = ColorModels.LinearRange;
                        UltraChart1.ColorModel.ColorBegin = Color.RoyalBlue;
                        UltraChart1.ColorModel.ColorEnd = Color.RoyalBlue;
                        UltraChart1.ColorModel.Scaling = ColorScaling.None;
                        UltraChart1.Axis.Y.Labels.Visible = true;
                        UltraChart1.Axis.Y.LineThickness = 1;
                        UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0.##>";
                        ChartTextAppearance textApp = new ChartTextAppearance();
                        textApp.Visible = true;
                        textApp.Column = -2;
                        textApp.Row = -2;
                        textApp.VerticalAlign = StringAlignment.Far;
                        UltraChart1.ColumnChart.ChartText.Add(textApp);
                        textApp.ItemFormatString = "<DATA_VALUE:0.##>";
                    #endregion
                        UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                        UltraChart1.Axis.Y.RangeMax = max * 1.1;
                        UltraChart1.Axis.Y.RangeMin = min * 0.9;
                        UltraChart1.Axis.Y.TickmarkInterval = (max * 1.1 - min * 0.9) / 10;
                }
            }
        }
        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

            if (UltraChart1.DataSource != null)
            {
                if (currentWay.Value == "")
                {
                    int counterValue = 0;
                    int counterMarks = 0;
                    int countBox = 0;
                    int countScen = e.SceneGraph.Count;
                        for (int i = 0; i < countScen; i++)
                        {
                            Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];

                            if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                            {

                                Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                                try
                                {
                                    if ((te.Path == "Border.Title.Grid.X")||(te.GetTextString().StartsWith("Значение")))
                                    {
                                        te.SetTextString(ColumnNames[counterMarks].ToString());
                                        counterMarks += 1;
                                    }
                                    else
                                    {
                                            te.bounds.Y = te.bounds.Y - 15;
                                            counterValue += 1;
                                            Infragistics.UltraChart.Core.Primitives.Box box = new Infragistics.UltraChart.Core.Primitives.Box(new Rectangle(te.bounds.X - 9, te.bounds.Y + 1, 17, 17));
                                            PaintElement paintEl = new PaintElement();
                                            paintEl.ElementType = PaintElementType.SolidFill;
                                            paintEl.Fill = Color.Green;
                                            paintEl.StrokeOpacity = 0;
                                            box.PE = paintEl;
                                            e.SceneGraph.Add(box);
                                    }
                                }
                                catch
                                {
                                    te.SetTextString(ColumnNames[0].ToString());
                                    counterMarks += 1;
                                }
                            }
                        }
                    
                }
                else
                {
                    int counterMarks = 0;
                    int countScen = e.SceneGraph.Count;
                    for (int i = 0; i < countScen; i++)
                    {
                        Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];

                        if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                        {
                            Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                            if ((te.Path == "Border.Title.Grid.X"))
                            {
                                if (counterMarks == 0)
                                {
                                    te.SetTextString(ColumnNames[0].ToString());
                                    counterMarks += 1;
                                }
                                else
                                {
                                    te.SetTextString(ColumnNames[counterMarks].ToString());
                                    counterMarks += 1;
                                }
                            }
                            else 
                            {
                                if (te.GetTextString()[0] == '_')
                                {
                                    string s = te.GetTextString().Remove(0, 2);
                                    te.SetTextString(s);
                                }
                            }
                        }
                    }
                }

            }
        }

        protected void UltraChart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }
        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label5.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            int width = int.Parse(UltraChart1.Width.Value.ToString());
            UltraChart1.Width = 900;
            UltraChart2.Width = 900;
            ReportExcelExporter1.Export(UltraChart1, Label3.Text, sheet3, 2);
            ReportExcelExporter1.Export(UltraChart2, Label4.Text, sheet2, 2);
            UltraChart1.Width = width;
            UltraChart2.Width = width;
        }
         
        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 800;
            e.Workbook.Worksheets["Таблица"].Rows[7].Cells[Grid1.Columns.Count-1].CellFormat.Alignment = HorizontalCellAlignment.Right;
            e.Workbook.Worksheets["Таблица"].Rows[7].Cells[Grid1.Columns.Count-1].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            e.Workbook.Worksheets["Диаграмма 1"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма 2"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма 1"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label5.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            int width = int.Parse(UltraChart1.Width.Value.ToString());
            UltraChart1.Width = 1000;
            UltraChart2.Width = 1000;
            Grid1.Width = 800;

            Grid1.Rows[0].Cells[0].Style.BorderDetails.WidthBottom = 0;
            ReportPDFExporter1.Export(headerLayout, section1);
            Grid1.Rows[0].Cells[0].Style.BorderDetails.WidthBottom = 1;
            Grid1.Width = 1210;
            ReportPDFExporter1.Export(UltraChart2, Label4.Text, section2);
            ReportPDFExporter1.Export(UltraChart1, Label3.Text, section3);
            
            UltraChart1.Width = width;
            UltraChart2.Width = width;
        }

        #endregion
    }
}
