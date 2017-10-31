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
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Shared.Events;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0004new.EO_0004LeninRegionNew
{
    public partial class _default : CustomReportPage
    {
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam curYear { get { return (UserParams.CustomParam("curYear")); } }
        private string page_title = "Оценка качества жизни населения в {0} году";
        string page_title2 = "Оценка качества жизни населения субъекта РФ по основным направлениям: демография, образование здравоохранение, социальная защита, культура и отдых, жилищные условия, экономика";
        private string grid_title = "Интегральный показатель уровня жизни населения и его составляющие";
        private string chart1_title = "Структура интегрального показателя";
        private string chart2_title = "Темп прироста интегрального показателя по отношению к предыдущему году, %";
        private int offsetX = 0;
        private int startOffsetX = 0;
        private Dictionary<string, int> widths = new Dictionary<string, int>();
        private Dictionary<string, int> kindNumbers = new Dictionary<string, int>();
        private int widthAll = 0;
        private double workersAll = 0;
        private object[] ColumnNanes;
        private int screen_width { get { return (int)Session["width_size"]; } }
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }
        string BN = "IE";

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
          /*  StringBuilder scriptString = new StringBuilder();
            
            scriptString.Append(@"<head><script type='text/javascript' language='JavaScript'>
            document.ready = function () 
        { el = getElementByID('3'); 
        alert(el.id);
   }
            </script></head>");
            writer.Write(scriptString.ToString());*/
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            Grid.Width = (int)((screen_width) - 10); ;
            Label6.Width = 1210;
            Chart1.Width = 600;
            Chart1.Height = 416;
            Chart2.Width = 600;
            Chart2.Height = 400;
            Label4.Width = 500;
            
            #region Настройка диаграммы

              
                Chart1.Border.Thickness = 0;
                Chart1.Axis.Y.Extent = 20;
                Chart1.Axis.X.Extent = 35;
                Chart1.Tooltips.FormatString = "<ITEM_LABEL>";
                Chart1.Legend.Visible = true;
                Chart1.Legend.Location = LegendLocation.Bottom;
                Chart1.Legend.SpanPercentage = 22;
                Chart1.Axis.X.Labels.SeriesLabels.Visible = false;
                Chart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
                Chart1.Axis.X.Labels.FontColor = Color.Black;
                Chart1.Data.ZeroAligned = true;
                Chart1.FillSceneGraph += new FillSceneGraphEventHandler(Chart1_FillSceneGraph);
                Chart1.ColorModel.ModelStyle = ColorModels.PureRandom;
                Chart1.ChartDrawItem += new ChartDrawItemEventHandler(Chart1_ChartDrawItem);
                Chart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                System.Drawing.Font font = new System.Drawing.Font("verdana", 10f);
                Chart1.Axis.Y.Labels.Font = font;
                Chart1.Axis.X.Labels.Font = font;
            #endregion
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender,e);
            //RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.FillDictionaryValues(YearsLoad("Years"));
                ComboYear.SelectLastNode();
                
            }
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            Grid.Height = Unit.Empty;
            /*if (BN != "FIREFOX")
            {
                Grid.Height = 202;
            }
            else
            {
                Grid.Height = 500;
            }*/
            Chart2.Axis.X.Labels.ItemFormatString=ComboYear.GetLastNode(0).Text;
            curYear.Value = ComboYear.SelectedValue;
            Label1.Text = String.Format(page_title,curYear.Value);
            Label2.Text = grid_title;
            Label3.Text = chart1_title;
            Label4.Text = chart2_title;
            Label5.Text = page_title2;
            Grid.DataBind();
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.Width = 1200;

            for (int i = 1; i < Grid.Rows[2].Cells.Count; i++)
            {
                Grid.Rows[2].Cells[i].Text = "<font font-family='Verdana'>Весовой коэффициент<br>" + Grid.Rows[2].Cells[i].Text + "<br> <a  href='../01/default1.aspx?paramlist=Pokazatel=[" + Grid.Columns[i].Header.Caption + "]'>Подробнее...</a></font>";
            }
            for (int i = 0; i < Grid.Rows.Count-1; i++)
            {
                Grid.Rows[i].Height = 65;
            }
            Grid.DisplayLayout.CellClickActionDefault = CellClickAction.Edit;
            Chart1.DataBind();
            Chart2.DataBind();
            Grid.DisplayLayout.RowSelectorStyleDefault.Width = 0;
        }
        Dictionary<string, int> YearsLoad(string sql)
        {
            DataTable dt = new DataTable();
            //DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Года", dt);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Года", dt);
            //CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            bool flag = false;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                flag = false;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        double m = double.Parse(dt.Rows[j].ItemArray[i].ToString());
                    }
                    catch 
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    dt.Columns.Remove(dt.Columns[i]);
                    i -= 1;
                }
            }
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                d.Add(dt.Columns[i].ColumnName, 0);
            }
                return d;
        }
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
         //   DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), " ", dt);
          //  DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid1"), " ", dt1);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), " ", dt);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid1"), " ", dt1);
            object[] o = new object[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                o[i] = dt.Rows[0].ItemArray[i];
            }
            dt.Rows.Add(o);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                o[i] = dt1.Rows[0].ItemArray[i];
            }
            dt.Rows.Add(o);
            Grid.DataSource = dt;
            double d=(double.Parse(dt.Rows[0].ItemArray[0].ToString())-1)*100;
            //Формирование динамичсекого текста
            Label6.Text = "&nbsp;&nbsp;&nbsp;В <b>" + ComboYear.SelectedValue + "</b> году <b>интегральный показатель уровня жизни населения</b> составил <b>" + String.Format("{0: 0.00}", double.Parse(dt.Rows[0].ItemArray[0].ToString())) + "</b>.<br>&nbsp;&nbsp;&nbsp;Это свидетельствует ";
            string AdvancedString = "";
            if (double.Parse(dt.Rows[0].ItemArray[0].ToString()) > 1)
            {
                Label6.Text = Label6.Text + "<b>об улучшении</b> общего социально-экономического состояния территории за отчетный год <b>на " + Math.Round(d, 2) /*String.Format("{0:# ##.00}", d).Remove(1, 1)*/ + "%. ";
                AdvancedString = "</b><br>&nbsp;&nbsp;&nbsp;На изменение качества жизни населения повлияло";
            }
            if (double.Parse(dt.Rows[0].ItemArray[0].ToString()) < 1)
            {
                Label6.Text = Label6.Text + "<b>об ухудшении</b> общего социально-экономического состояния территории за отчетный год <b>на " + Math.Round(d, 2) /*String.Format("{0:# ##.00}", d).Remove(1, 1)*/ + "%. ";
                AdvancedString = "</b><br>&nbsp;&nbsp;&nbsp;На изменение качества жизни населения повлияло";
            }
            if (double.Parse(dt.Rows[0].ItemArray[0].ToString()) == 1)
            {
                Label6.Text = Label6.Text + "о том, что общая социально-экономическая ситуация территории за отчетный год <b>не изменилась. ";
                AdvancedString = "</b><br>&nbsp;&nbsp;&nbsp;В то же время наблюдалось";
            }
            string s = AdvancedString + " ухудшение состояния сфер: <b>";
            int k = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < 1)
                {
                    k += 1;
                    s = s + dt.Columns[i].ColumnName.ToLower() + ", ";
                }

            }
            if (k == 0)
            {
                s = AdvancedString + " улучшение состояния сфер: <b>";
            }
            else
            {
                Label6.Text = Label6.Text + s + "</b>";
                s = "</b>а также улучшение состояния сфер: <b>";
            }
            k = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > 1)
                {
                    k += 1;
                    s = s + dt.Columns[i].ColumnName.ToLower() + ", ";
                }
            }
            if (k == 0)
            {
                Label6.Text = Label6.Text.Remove(Label6.Text.Length-2, 2);
                Label6.Text += "</b>.";
            }
            else
            {
                Label6.Text = Label6.Text + s;
                Label6.Text = Label6.Text.Remove(Label6.Text.Length-2, 2);
                Label6.Text += "</b>.";
            }

        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 1)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)//
                {
                    if (double.Parse(e.Row.Cells[i].Value.ToString()) > 1)
                    {
                        Grid.Rows[0].Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position: center;";
                        Grid.Rows[0].Cells[i].Style.BackgroundImage = "~/images/k_green.png";
                        Grid.Rows[1].Cells[i].Style.BackColor = Color.White;
                        Grid.Rows[0].Cells[i].Style.BackColor = Color.White;
                        Grid.Rows[1].Cells[i].Style.Font.Bold = true;
                        Grid.Rows[1].Cells[i].Style.Font.Name = "Verdana";
                        Grid.Rows[1].Cells[i].Style.Font.Size = 16;
                        Grid.Rows[1].Cells[i].Text = Grid.Rows[0].Cells[i].Text;
                        Grid.Rows[1].Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                        Grid.Rows[0].Cells[i].Text = "";
                    }
                    else
                    {
                        if (double.Parse(e.Row.Cells[i].Value.ToString()) < 1)
                        {
                            Grid.Rows[1].Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position: center;";
                            Grid.Rows[1].Cells[i].Style.BackgroundImage = "~/images/k_red.png";
                            Grid.Rows[0].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[1].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[0].Cells[i].Style.Font.Bold = true;
                            Grid.Rows[0].Cells[i].Style.Font.Name = "Verdana";
                            Grid.Rows[0].Cells[i].Style.Font.Size = 16;
                            Grid.Rows[0].Cells[i].Text = Grid.Rows[1].Cells[i].Text;
                            Grid.Rows[0].Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                            Grid.Rows[1].Cells[i].Text = "";
                        }
                        else
                        {
                            Grid.Rows[0].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[1].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[0].Cells[i].Style.Font.Bold = true;
                            Grid.Rows[0].Cells[i].Style.Font.Name = "Verdana";
                            Grid.Rows[0].Cells[i].Style.Font.Size = 16;
                            Grid.Rows[0].Cells[i].Text = Grid.Rows[1].Cells[i].Text;
                            Grid.Rows[0].Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                            Grid.Rows[1].Cells[i].Text = "";
                        }
                    }
                }

            }
            for (int i = 0; i < e.Row.Cells.Count; i++)//
            {
                e.Row.Cells[i].Style.Font.Name = "Verdana";
            }

            
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
        DataTable Chart2DataTable;
        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            //DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), " ", dt);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), " ", dt);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }
            object[] o=new object[dt1.Columns.Count];
            ColumnNanes=new object[dt1.Columns.Count-1];

             o[0]=dt.Rows[0].ItemArray[0];
             o[1]=(GetNumber(dt.Rows[0].ItemArray[1].ToString())-1)*100;
             ColumnNanes[0] = dt.Columns[1].ColumnName;
             dt1.Columns[1].ColumnName = "Темп прироста <b>" + Math.Round(double.Parse(o[1].ToString()), 2).ToString() + "</b>% за " + dt.Columns[1].ColumnName + " год";
             if (dt1.Columns.Count >= 3)
             {
                 for (int i = 2; i < dt1.Columns.Count; i++)
                 {
                     o[i] = GetNumber(o[i - 1].ToString()) + (GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100;
                     ColumnNanes[i - 1] = dt.Columns[i].ColumnName;
                     
                     dt1.Columns[i].ColumnName = "Темп прироста <b>" + Math.Round((GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100,2).ToString() + "</b>% за "+dt.Columns[i].ColumnName+" год";
                 }
             }
             dt1.Rows.Add(o);
             double max = GetNumber(o[1].ToString());
             double min = GetNumber(o[1].ToString());
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
            Chart2.Axis.Y.RangeType = AxisRangeType.Custom;
            Chart2.Axis.Y.RangeMax = max+1;
            Chart2.Axis.Y.RangeMin = min-1;
            Chart2.DataSource = dt1;
            Chart2DataTable = dt;
        }
        void Chart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                        
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            widthAll += box.rect.Width - 2;
                        }
                    }
                }
                offsetX =0;
                int j = 1;
                int labelCount = 1;
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            if (offsetX == 0)
                            {
                                offsetX = box.rect.X+10;
                                startOffsetX = box.rect.X;
                            }
                                box.rect.X = offsetX;
                            
                                double width = double.Parse(dtChart13.Rows[0].ItemArray[j].ToString()) *500;
                                widths.Add(box.DataPoint.Label, (int)width);
                                box.rect.Width = (int)width;
                                j += 1;
                                offsetX += box.rect.Width + 2;
                           
                        }
                    }
                   
                }
                offsetX = 0;
                int counter = 1;
                int counter2 = 1;
                try
                {
                    for (int i = 0; i < e.SceneGraph.Count; i++)
                    {
                        Primitive primitive = e.SceneGraph[i];
                        if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                        {

                            Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                            if (widths.ContainsKey(te.GetTextString()))
                                {
                                    Primitive primitive2 = e.SceneGraph[i + 18];
                                    Infragistics.UltraChart.Core.Primitives.Text te2 = (Infragistics.UltraChart.Core.Primitives.Text)primitive2;
                                    if (offsetX == 0)
                                    {
                                        offsetX = startOffsetX;
                                    }
                                    LabelStyle labStyle = new LabelStyle();
                                    labStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                                    te.SetLabelStyle(labStyle);
                                    te.bounds.X = offsetX - 8 + widths[te.GetTextString()] / 2 + 14;
                                    te2.bounds.X = offsetX - 8 + widths[te.GetTextString()] / 2 + 17;
                                    te.bounds.Y = 265;
                                    offsetX += widths[te.GetTextString()] + 2;
                                    te.bounds.Width = 20;
                                    te.SetTextString(counter.ToString());
                                    counter += 1;
                                }
                                else
                                {
                                    if (te.Path=="Border.Title.Legend")
                                    {
                                        string s = orderedTable.Columns[counter2].ColumnName;
                                        s = counter2.ToString() + " " + s;
                                        te.SetTextString(s);
                                        counter2 += 1;
                                    }
                                }

                        }
                    }
                }
                catch { }
                Line lineLegend = new Line(new Point(234,387),new Point(246,387)); //создание дополнительной строчки в легенде
                lineLegend.lineStyle.DrawStyle = LineDrawStyle.Solid;
                lineLegend.PE.Stroke = Color.Red;
                lineLegend.PE.StrokeWidth = 2;
                e.SceneGraph.Add(lineLegend);

                Text textLegend =new Text();
                textLegend.labelStyle.Font = new System.Drawing.Font("Verdana",(float)(7.8));
                textLegend.SetTextString("Уровень интегрального показателя");
                textLegend.bounds.X = 250;
                textLegend.bounds.Y = 387;
                e.SceneGraph.Add(textLegend);


               /* Text titleBottom = new Text();
                titleBottom.labelStyle.Font = new System.Drawing.Font("Arial", (float)(7.8));
                titleBottom.SetTextString("вес в интегральном показателе");
                titleBottom.bounds.X = 405;
                titleBottom.bounds.Y =314;
                e.SceneGraph.Add(titleBottom);*/

                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

                int xMin = (int)xAxis.MapMinimum;
                int xMax = (int)xAxis.MapMaximum;
                double urfoAverage;
                if (double.TryParse(dtChart12.Rows[0].ItemArray[0].ToString(), out urfoAverage))
                {
                    int fmY = (int)yAxis.Map(urfoAverage);
                    Line line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Solid;
                    line.PE.Stroke = Color.Red;
                    line.PE.StrokeWidth = 2;
                    line.p1 = new Point(xMin, fmY);
                    line.p2 = new Point(xMax, fmY);
                    e.SceneGraph.Add(line);

                    Text text = new Text();
                    text.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                    text.PE.Fill = Color.Black;
                    text.bounds = new Rectangle(xMin-38, fmY - 2, 780, 15);
                    //text.SetTextString(
                    // String.Format("Средняя з/п {0}: {1:N2} руб.",
                    //           RegionsNamingHelper.ShortName(ComboRegion.SelectedValue), urfoAverage));
                    text.SetTextString(String.Format("{0:# ##0.00}", double.Parse(dtChart12.Rows[0].ItemArray[0].ToString())));//String.Format(dtChart12.Rows[0].ItemArray[0].ToString(),""));
                    e.SceneGraph.Add(text);
                }

                if (double.TryParse("1", out urfoAverage))
                {
                    int fmY = (int)yAxis.Map(urfoAverage);
                    Line line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Dot;
                    line.PE.Stroke = Color.Gray;
                    line.PE.StrokeWidth = 2;
                    line.p1 = new Point(xMin, fmY);
                    line.p2 = new Point(xMax, fmY);
                    e.SceneGraph.Add(line);

                    Text text = new Text();
                    text.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                    
                    text.PE.Fill = Color.Black;
                    text.bounds = new Rectangle(xMin-15, fmY - 9, 780, 15);
                    //text.SetTextString(
                    // String.Format("Средняя з/п {0}: {1:N2} руб.",
                    //           RegionsNamingHelper.ShortName(ComboRegion.SelectedValue), urfoAverage));
                    text.SetTextString("1");
                    
                    e.SceneGraph.Add(text);
                }
                Label1.Text = widths.Keys.ToString();

        }


        void Chart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
           /* Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                widthLegendLabel = (int)Chart1.Width.Value + 20;

                widthLegendLabel -= Chart1.Legend.Margins.Left + Chart1.Legend.Margins.Right;
                text.bounds.Width = widthLegendLabel;
                if (kindNumbers.ContainsKey(text.GetTextString()))
                {
                    int kindNumber = kindNumbers[text.GetTextString()];
                    if (IsSmallResolution && text.GetTextString().Contains("<br>"))
                    {
                        text.SetTextString(
                            "Оптовая и розничная торговля; ремонт автотранспортных средств, бытовых изделий");
                    }
                  //  text.SetTextString(String.Format(" {0}. {1} ({2:N2} руб.)", kindNumber, text.GetTextString(), dtChart.Rows[0][kindNumber - 1]));
                }
            }*/
        }
        DataTable dtChartAverage;
        DataTable dtChart;
        DataTable dtChart12;
        DataTable dtChart13;
        DataTable orderedTable;
        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Chart1"));
            dtChart = new DataTable();
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dt);

            query = DataProvider.GetQueryText(String.Format("Chart1_2"));
            dtChart12 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart12);
            orderedTable = new DataTable();
            dtChart13 = new DataTable();
            query = DataProvider.GetQueryText(String.Format("Chart1_3"));
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", orderedTable);
            for (int i = 0; i < orderedTable.Columns.Count; i++)
            {
                dtChart13.Columns.Add(i.ToString(), orderedTable.Columns[0].DataType);
            }
            object[] rowsObj=new object[dtChart13.Columns.Count];
            for (int i = 0; i < orderedTable.Rows.Count; i++)
            {
                for (int j = 0; j < orderedTable.Rows[i].ItemArray.Length; j++)
                {
                    rowsObj[j] = orderedTable.Rows[i].ItemArray[j];
                }
                dtChart13.Rows.Add(rowsObj);
            }

            int counter=1;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dtChart.Columns.Add(counter.ToString(), dt.Columns[i].DataType);
                    counter += 1;
                }
            object[] o=new object[dtChart.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //o[0] = dt.Rows[0].ItemArray[0].ToString() + "<br>оценка " + dt.Rows[0].ItemArray[1].ToString() + "<br>вес" + dtChart13.Rows[i].ItemArray[2].ToString();
                o[i] = dt.Rows[0].ItemArray[i];
                dtChart.Columns[i].ColumnName = dt.Columns[i].ColumnName + "<br>оценка <b>" + String.Format("{0:# ##0.00}", decimal.Parse(dt.Rows[0].ItemArray[i].ToString())) + "</b><br>вес <b>" + String.Format("{0:# ##0.00}", decimal.Parse(dtChart13.Rows[0].ItemArray[i + 1].ToString()))+"</b>";
               // o[i] = dt.Columns[i].ColumnName + "оценка" + String.Format("{0:# ##0.00}", decimal.Parse(dt.Rows[0].ItemArray[i].ToString())) + "вес " + String.Format("{0:# ##0.00}", decimal.Parse(dtChart13.Rows[0].ItemArray[i + 1].ToString()));
            }
            dtChart.Rows.Add(o);
                Chart1.DataSource = dtChart;
        }

        protected void Chart2_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Header.Caption = e.Layout.Bands[0].Columns[0].Key.ToUpper();
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                if (BN == "IE")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.113);
                }
                if (BN=="FIREFOX")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.113);
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.1145);
                }
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i],"### ##0.00");
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
            }
            

        }

        protected void Chart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int counterValue=1;
            int counterMarks = 0;
            int countScen = e.SceneGraph.Count;
            for (int i = 0; i < countScen; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    
                    Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                    
                        if ((te.Path == "Border.Title.Grid.X"))
                        {
                            if (counterMarks == 0)
                            {
                                te.SetTextString("2006");
                                counterMarks += 1;
                            }
                            else
                            {
                                te.SetTextString(ColumnNanes[counterMarks].ToString());
                                counterMarks += 1;
                            }
                        }
                        else {

                        if ((Chart2.DataSource != null) && (ComboYear.SelectedIndex!=0))
                        {
                            if (counterValue < Chart2DataTable.Rows[0].ItemArray.Length)
                            {
                                te.bounds.Y = te.bounds.Y - 10;
                                Box box = new Box(new Rectangle(te.bounds.X - 9, te.bounds.Y + 1, 17, 17));
                                PaintElement paintEl = new PaintElement();
                                paintEl.ElementType = PaintElementType.SolidFill;
                                if (GetNumber(te.GetTextString()) <= 0)
                                {
                                    paintEl.Fill = Color.Red;
                                }
                                else
                                {
                                    paintEl.Fill = Color.Green;
                                }
                                paintEl.StrokeOpacity = 0;
                                box.PE = paintEl;
                                e.SceneGraph.Add(box);
                                double pers = (GetNumber(Chart2DataTable.Rows[0].ItemArray[counterValue].ToString()) - 1) * 100;
                                te.SetTextString(String.Format("{0:# ##0.00}", pers));
                                counterValue += 1;

                            }
                        }
                        else
                        {
                            if ((i == 15) && (Chart2.DataSource != null))
                            {
                                te.bounds.Y = te.bounds.Y - 10;
                                Box box = new Box(new Rectangle(te.bounds.X - 9, te.bounds.Y + 1, 17, 17));
                                PaintElement paintEl = new PaintElement();
                                paintEl.ElementType = PaintElementType.SolidFill;
                                if (GetNumber(te.GetTextString()) <= 0)
                                {
                                    paintEl.Fill = Color.Red;
                                }
                                else
                                {
                                    paintEl.Fill = Color.Green;
                                }
                                paintEl.StrokeOpacity = 0;
                                box.PE = paintEl;
                                e.SceneGraph.Add(box);
                                double pers = (GetNumber(Chart2DataTable.Rows[0].ItemArray[1].ToString()) - 1) * 100;
                                te.SetTextString(String.Format("{0:# ##0.00}", pers));
                                counterValue += 1;
                            }
                            if (i < 15)
                            {
                                te.SetTextString("2006");
                            }
                        
                        }
                    }

                    
                }

            }
        }




    }
}
