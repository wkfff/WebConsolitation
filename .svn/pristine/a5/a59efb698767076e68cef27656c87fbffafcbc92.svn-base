using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebNavigator; 
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
 
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders; 
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using Infragistics.WebUI.UltraWebGrid;
using Dundas.Maps.WebControl;
using System.Globalization;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report;
using System.Text.RegularExpressions;
using Infragistics.WebUI.Misc;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Graphics = System.Drawing.Graphics;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using System.Drawing.Imaging;
 
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0007_Sahalin
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Мониторинг коэффициентов естественного движения населения (по муниципальным образованиям).";
        private string sub_page_title = "Данные ежегодного мониторинга коэффициентов естественного движения населения, «{0}»";
        private string columnName = "";
        private CustomParam chartPok { get { return (UserParams.CustomParam("chartPok")); } }
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam typeGroup { get { return (UserParams.CustomParam("typeGroup")); } }
        private GridHeaderLayout headerLayout;
        private int legendPercentage = 0;
        private string style = "";
        private object[] edIsm;
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            Grid.Height = Unit.Empty;
            comboRegion.Width = 350;
            if (IsSmallResolution)
            {
                Grid.Width = minScreenWidth;
                Chart.Width = minScreenWidth-5;
                legendPercentage = 18;
            }
            else
            {
                Grid.Width = (int)(minScreenWidth - 15);
                Chart.Width = (int)(minScreenWidth - 20);
                legendPercentage = 10;
            }

            CrossLink1.Text = "Мониторинг коэффициентов естественного движения населения (по состоянию на выбранную дату)";
            CrossLink1.NavigateUrl = "~/reports/STAT_0003_0008/default.aspx";
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                comboRegion.Title = "Территория";
                comboRegion.FillDictionaryValues(RegionsLoad("Regions"));
            }
            if (comboRegion.SelectedIndex == 0)
            {
                baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Дальневосточный федеральный округ].[Сахалинская область].DATAMEMBER";
                typeGroup.Value = "[Группировки__Население_Естественное движение].[Все группировки].[Без группировки]";

            }
            else
            {
                baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Дальневосточный федеральный округ].[Сахалинская область].[Сахалинская обл.].[" + comboRegion.SelectedValue + "]";
                typeGroup.Value = "[Группировки__Население_Естественное движение].[Все группировки].[По городам и районам]";
               
            }

            #region Настройка диаграммы 1

                Chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

                Chart.ChartType = ChartType.AreaChart;
                Chart.Border.Thickness = 0;
                Chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
                Chart.Axis.X.Extent = 60;
                Chart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
                Chart.Axis.X.Labels.FontColor = Color.Black;
                Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                Chart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
                Chart.Axis.Y.Labels.FontColor = Color.Black;
                Chart.Axis.Y.Extent = 40;

                Chart.Data.EmptyStyle.Text = " ";
                Chart.EmptyChartText = " ";

                Chart.AreaChart.NullHandling = NullHandling.DontPlot;

                Chart.AreaChart.LineAppearances.Clear();

                LineAppearance lineAppearance = new LineAppearance();
                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.Thickness = 5;
                Chart.AreaChart.LineAppearances.Add(lineAppearance);

                lineAppearance = new LineAppearance();
                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.Thickness = 3;
                Chart.AreaChart.LineAppearances.Add(lineAppearance);

                lineAppearance = new LineAppearance();
                lineAppearance.IconAppearance.Icon = SymbolIcon.None;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.Thickness = 5;
                lineAppearance.LineStyle.MidPointAnchors = false;
                Chart.AreaChart.LineAppearances.Add(lineAppearance);

                Chart.Legend.Visible = true;
                Chart.Legend.Location = LegendLocation.Top;
                Chart.Legend.SpanPercentage = legendPercentage;
                Chart.Legend.Font = new System.Drawing.Font("Verdana", 10);


                Chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
                Chart.ColorModel.Skin.PEs.Clear();
                int peCol = 0;
                if (comboRegion.SelectedIndex == 0)
                { peCol=2;}
                else
                { peCol=1;}
                for (int i = peCol; i <= 3; i++)
                {
                    PaintElement pe = new PaintElement();
                    Color color = Color.White;
                    Color stopColor = Color.White;
                    PaintElementType peType = PaintElementType.Gradient;
                    switch (i)
                    {
                        case 1:
                            {
                                if (comboRegion.SelectedIndex==0)
                                {
                                color = Color.Transparent;
                                stopColor = Color.Gray;
                                peType = PaintElementType.Hatch;
                                pe.Hatch = FillHatchStyle.ForwardDiagonal;
                            
                                }
                                else
                                {
                                 color = Color.MediumSeaGreen;
                                stopColor = Color.Green;
                                peType = PaintElementType.Gradient;
                            
                                }
                                break;
                            }
                        case 2:
                            {
                                color = Color.Transparent;
                                stopColor = Color.Gray;
                                peType = PaintElementType.Hatch;
                                pe.Hatch = FillHatchStyle.ForwardDiagonal;
                            
                                break;
                            }
                    }
                    pe.Fill = color;
                    pe.FillStopColor = stopColor;
                    pe.ElementType = peType;
                    pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                    pe.FillOpacity = (byte)150;
                    pe.FillStopOpacity = (byte)150;
                   /* if (i == 3)
                    {
                        pe.Stroke = Color.Red;
                        pe.StrokeWidth = 5;
                    }*/
                    Chart.ColorModel.Skin.PEs.Add(pe);
                }

            #endregion
            
            chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Коэффициент рождаемости]";
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            SetSfereparam();
            if (Grid.DataSource != null)
            { 
                Chart.DataBind();
                Chart.Tooltips.FormatString = "<SERIES_LABEL>,<br>  за <ITEM_LABEL>г. <DATA_VALUE:##0.00> " + edIsm[1].ToString().ToLower();
                FormText();
            }
            Label1.Text = page_title;
            Page.Title = page_title;
            Label2.Text = String.Format(sub_page_title, comboRegion.SelectedValue);//.Replace("муниципальный район", "м-р").Replace("Город", "г."));
            Label3.Text = "Динамика коэффициента рождаемости, " + edIsm[1].ToString().ToLower();
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }

        #region Добавление checkbox
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            string[] buttonNames = new string[4];
            buttonNames[0] = "Коэффициент рождаемости";
            buttonNames[1] = "Коэффициент смертности";
            buttonNames[2] = "Коэффициент естественного прироста населения";
            buttonNames[3] = "Младенческая смертность";
            //buttonNames[4] = "Перинатальная смертность";
            if (PlaceHolder1.Controls.Count != 0)
            {
                RadioButton r1 = (RadioButton)(PlaceHolder1.Controls[0]);
                if (r1.Text != buttonNames[0])
                {
                    PlaceHolder1.Controls.Clear();
                    for (int i = 0; i < buttonNames.Length; i++)
                    {
                        Random r = new Random();
                        ra = ra++;
                        RadioButton rb = new RadioButton();
                        rb.Style.Add("font-size", "10pt");
                        rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                        rb.Style.Add("font-family", "Verdana");
                        PlaceHolder1.Controls.Add(rb);
                        Label l = new Label();
                        l.Text = "<br>";
                        PlaceHolder1.Controls.Add(l);
                        rb.Text = buttonNames[i];
                        rb.GroupName = "sfere" + ra.ToString();
                        rb.ValidationGroup = rb.GroupName;
                        rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                        rb.AutoPostBack = 1 == 1;
                        rb.Checked = 1 == 2;
                    }
                    ((RadioButton)(PlaceHolder1.Controls[0])).Checked = true;
                }
            }
            else
            {
                PlaceHolder1.Controls.Clear();
                for (int i = 0; i < buttonNames.Length; i++)
                {
                    Random r = new Random();
                    ra = ra++;
                    RadioButton rb = new RadioButton();
                    rb.Style.Add("font-size", "10pt");

                    rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                    rb.Style.Add("font-family", "Verdana");
                    PlaceHolder1.Controls.Add(rb);
                    Label l = new Label();
                    l.Text = "<br>";
                    PlaceHolder1.Controls.Add(l);
                    rb.Text = buttonNames[i];
                    rb.GroupName = "sfere" + ra.ToString();
                    rb.ValidationGroup = rb.GroupName;
                    rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                    rb.AutoPostBack = 1 == 1;
                    rb.Checked = 1 == 2;
                }
                ((RadioButton)(PlaceHolder1.Controls[0])).Checked = true;
            }
        }

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)(sender);
            rb.Checked = 1 == 1;

            columnName = rb.Text;
            string region = "";
            if (comboRegion.SelectedIndex == 0)
            {
                region = RegionSettingsHelper.Instance.Name;
            }
            else
            {
                region = comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г.");
            }
            Chart.Legend.SpanPercentage = legendPercentage;
            if (rb.Text == "Коэффициент рождаемости")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Коэффициент рождаемости]";
                Label3.Text = "Динамика коэффициента рождаемости, " + edIsm[1].ToString().ToLower();
                Chart.Tooltips.FormatString = "<SERIES_LABEL>,<br>  за <ITEM_LABEL>г. <DATA_VALUE:##0.00>, " + edIsm[1].ToString().ToLower();
            }
            if (rb.Text == "Коэффициент смертности")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Коэффициент смертности]";

                Label3.Text = "Динамика коэффициента смертности, " + edIsm[2].ToString().ToLower();
                Chart.Tooltips.FormatString = "<SERIES_LABEL>,<br>  за <ITEM_LABEL>г. <DATA_VALUE:##0.00>, " + edIsm[2].ToString().ToLower();
            }
            if (rb.Text == "Коэффициент естественного прироста населения")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Коэффициент естественного прироста населения]";


                Chart.Legend.SpanPercentage = 18;
                Label3.Text = "Динамика коэффициента естественного прироста, " + edIsm[3].ToString().ToLower();
                Chart.Tooltips.FormatString = "<SERIES_LABEL>,<br>  за <ITEM_LABEL>г. <DATA_VALUE:##0.00>, " + edIsm[3].ToString().ToLower();
            }
            if (rb.Text == "Младенческая смертность")
            {
                chartPok.Value = "        [Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Младенческая смертность]";

                Label3.Text = "Динамика младенческой смертности, " + edIsm[4].ToString().ToLower();
                Chart.Tooltips.FormatString = "<SERIES_LABEL>,<br>  за <ITEM_LABEL>г. <DATA_VALUE:##0.00>, " + edIsm[4].ToString().ToLower();
            }
            //if (rb.Text == "Перинатальная смертность")
            //{
            //    chartPok.Value = "        [Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Перинатальная смертность]";


            //    Label3.Text = "Динамика перинатальной смертности, " + edIsm[5].ToString().ToLower();
            //    Chart.Tooltips.FormatString = "<SERIES_LABEL>,<br>  за <ITEM_LABEL>г. <DATA_VALUE:##0.00>, " + edIsm[5].ToString().ToLower();
            //}
            Chart.DataBind();
        }
        #endregion

        #region Обработчики грида
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Период", dt);
            edIsm = new object[dt.Rows[0].ItemArray.Length];
            edIsm = dt.Rows[0].ItemArray;
            dt.Rows.Remove(dt.Rows[0]);
            try
            {
                string ValueFirstYearName = dt.Rows[0]["Период"].ToString();
                string DeviationFirstYearName = dt.Rows[1]["Период"].ToString();

                if (DeviationFirstYearName.Split(';')[0] != ValueFirstYearName.Split(';')[0])
                {
                    dt.Rows.InsertAt(dt.NewRow(), 1);
                    dt.Rows.InsertAt(dt.NewRow(), 1);
                }
            }
            catch { }

            if (dt.Rows.Count < 1)
            {
                Grid.DataSource = null;
            }
            else
            {
                if (dt.Rows.Count > 3)
                {
                    for (int i = 1; i < dt.Rows[1].ItemArray.Length; i++)
                    {
                        dt.Rows[1][i] = DBNull.Value;
                    }

                }
                Grid.DataSource = dt;
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            //return; ;
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position:30px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 90px"; }

            if ((e.Row.Index + 1) % 3 == 0)
            {

                e.Row.Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = Grid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0];
                Grid.Rows[e.Row.Index - 2].Cells[0].Text = "";

                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                e.Row.Cells[0].Style.BackColor = Color.White;
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                    e.Row.Cells[i].Style.BackColor = Color.White;
                    if (e.Row.Index != 2)
                    {
                        if (Grid.Rows[e.Row.Index - 2].Cells[i].Value != null)
                        {
                            Grid.Rows[e.Row.Index - 2].Cells[i].Value = string.Format("{0:N2}", Decimal.Parse(Grid.Rows[e.Row.Index - 2].Cells[i].Value.ToString()));
                        }

                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                        {
                            DownDeviation(e, i);
                        } 
                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                        {
                            UpDeviation(e, i);
                        }

                        if ((e.Row.Cells[i].Value != null))
                        {
                            SpeedDeviation(e, i);
                        }
                    }
                    else
                    {
                        e.Row.Cells[i].Value = null;
                    }



                    if (Grid.Columns[i].Key.StartsWith("Коэффициент естественного"))
                    {
                        e.Row.Cells[i].Text = "";
                    }
                }

            }
        }

        private void SpeedDeviation(RowEventArgs e, int i)
        {
            e.Row.Cells[i].Title = "Темп прироста к " + Grid.Rows[e.Row.Index - 4].Cells[0].Text + " году";
            e.Row.Cells[i].Value = String.Format("{0:0.##%}", Convert.ToDouble(e.Row.Cells[i].Value));
        }

        private void UpDeviation(RowEventArgs e, int i)
        {
            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Рост относительно " + Grid.Rows[e.Row.Index - 4].Cells[0].Text + " года";
            if ((Grid.Columns[i].Key.Contains("смертности")) || (Grid.Columns[i].Key.Contains("смертность")))
            {
                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
            }
            else
            {
                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            }
            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;
            Grid.Rows[e.Row.Index - 1].Cells[i].Value = String.Format("{0:0.00}", Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value));
        }

        private void DownDeviation(RowEventArgs e, int i)
        {
            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Снижение относительно " + Grid.Rows[e.Row.Index - 4].Cells[0].Text + " года";
            if ((Grid.Columns[i].Key.Contains("смертности")) || (Grid.Columns[i].Key.Contains("смертность")))
            {
                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
            }
            else
            {
                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
            }
            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;
            Grid.Rows[e.Row.Index - 1].Cells[i].Value = String.Format("{0:0.00}", Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value));
        }


        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.09);
            e.Layout.Bands[0].Columns[0].Header.Caption = "Период";
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            GridHeaderCell header = headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.14);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key + ", " + edIsm[i].ToString().ToLower();

                header = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "#0.00");
            }
        }
        #endregion

        #region экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            int width = int.Parse(Chart.Width.Value.ToString());
            Chart.Width = 900;
            if (comboRegion.SelectedIndex == 0)
            {
                Chart.Legend.SpanPercentage = legendPercentage;
            }
            else
            {
                Chart.Legend.SpanPercentage = 18;
            }
            ReportExcelExporter1.Export(Chart, Label3.Text, sheet2, 2);
            Chart.Width = width;
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
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            Label4.Text = Label4.Text.Replace("<br>", "\n");
            ReportPDFExporter1.PageSubTitle = Label2.Text+"\n\n"+Label4.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Label4.Text = Label4.Text.Replace("\n", "<br>");
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            int width = int.Parse(Chart.Width.Value.ToString());
            Chart.Width = 1000;
            if (comboRegion.SelectedIndex == 0)
            {
                Chart.Legend.SpanPercentage = legendPercentage;
            }
            else
            {
                Chart.Legend.SpanPercentage = 18;
            }
            ReportPDFExporter1.HeaderCellHeight = 60;
            int width1 = int.Parse(Chart.Width.Value.ToString());
            Grid.Width = 800;
            Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            ReportPDFExporter1.Export(headerLayout, section1);
            Grid.Width = width1;
            ReportPDFExporter1.Export(Chart, Label3.Text, section2);
            Chart.Width = width;
        }
        void ultraWebGridDocumentExporter_RowExporting(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportingEventArgs e)
        {
            e.GridRow.Height = 500;
        }
        #endregion

        #region Формирование динамичсекого текста
        protected void FormText()
        {
            
            string str1 = "&nbsp;&nbsp;&nbsp;{0} в <b>{1}г.</b> ";
            string str2 = "по сравнению с <b>{0}г.</b> {1}";
            string str3 = " на <b>{0}</b> {1} и";
            string str4 = " составил <b>{0}</b> {1}.<br>";
            Label4.Text = "";
            string buf = "";
            for (int i = 1; i < 4; i++)
            {
                buf = "";
                if (Grid.Rows.Count <= 3)
                {
                    buf += String.Format(str1, Grid.Columns[i].Key, Grid.Rows[Grid.Rows.Count - 2].Cells[0].Text);
                    buf += String.Format(str4, String.Format("{0:0.00}", Convert.ToDouble(Grid.Rows[Grid.Rows.Count - 3].Cells[i].Value)), edIsm[i].ToString().ToLower());
                }
                else
                {
                    buf += String.Format(str1, Grid.Columns[i].Key, Grid.Rows[Grid.Rows.Count - 2].Cells[0].Text);
                    CRHelper.SaveToQueryLog(Grid.Rows[Grid.Rows.Count - 2].Cells[i].Value.ToString());

                    if (!string.IsNullOrEmpty(Grid.Rows[Grid.Rows.Count - 2].Cells[i].Value.ToString()))
                    if (Convert.ToDouble(Grid.Rows[Grid.Rows.Count - 2].Cells[i].Value.ToString().Replace("%","")) < 0)
                    {
                        if (Grid.Columns[i].Key.StartsWith("Коэффициент смертности"))
                        {
                            buf += String.Format(str2, Grid.Rows[Grid.Rows.Count - 5].Cells[0].Text, "&nbsp;<img src='../../images/arrowGreenDownBB.png'>уменьшился");
                        }
                        else
                        {
                            buf += String.Format(str2, Grid.Rows[Grid.Rows.Count - 5].Cells[0].Text, "&nbsp;<img src='../../images/arrowRedDownBB.png'>уменьшился");
                        }
                        buf += String.Format(str3, String.Format("{0:0.00}", Math.Abs(Convert.ToDouble(Grid.Rows[Grid.Rows.Count - 2].Cells[i].Value))),edIsm[i].ToString().ToLower());
                    }
                    else
                    {
                        if (Convert.ToDouble(Grid.Rows[Grid.Rows.Count - 2].Cells[i].Value.ToString().Replace("%", "")) > 0)
                        {
                            if (Grid.Columns[i].Key.StartsWith("Коэффициент смертности"))
                            {
                                buf += String.Format(str2, Grid.Rows[Grid.Rows.Count - 5].Cells[0].Text, "&nbsp;<img src='../../images/arrowRedUpBB.png'>вырос");
                            }
                            else
                            {
                                buf += String.Format(str2, Grid.Rows[Grid.Rows.Count - 5].Cells[0].Text, "&nbsp;<img src='../../images/arrowGreenUpBB.png'>вырос");
                            }
                            buf += String.Format(str3, String.Format("{0:0.00}", Math.Abs(Convert.ToDouble(Grid.Rows[Grid.Rows.Count - 2].Cells[i].Value.ToString().Replace("%", "")))), edIsm[i].ToString().ToLower());
                        }
                        else
                        { 
                            buf+=String.Format(str2,Grid.Rows[Grid.Rows.Count - 5].Cells[0].Text," не изменился");
                            buf += " и" ;
                        }
                    }
                    buf += String.Format(str4, String.Format("{0:0.00}", Convert.ToDouble(Grid.Rows[Grid.Rows.Count - 3].Cells[i].Value)), edIsm[i].ToString().ToLower());
                }
                Label4.Text += buf;
            }
        }
        #endregion

        #region Обработчики диаграммы
        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            if (columnName == "")
            {
                columnName = "Коэффициент рождаемости";
            }

            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Диаграмма", dt);
            if (dt.Rows.Count < 1)
            {
                Chart.DataSource = null;
            }
            else
            {
                if (comboRegion.SelectedIndex != 0)
                {
                    if (IsSmallResolution)
                    {
                        dt.Rows[0][0] = columnName + ",<br> " + comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г.");
                        dt.Rows[1][0] = columnName + ",<br> " + comboRegion.GetRootNodesName(0);
                    }
                    else
                    {
                        dt.Rows[0][0] = columnName + ", " + comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г.");
                        dt.Rows[1][0] = columnName + ", " + comboRegion.GetRootNodesName(0);
                    }
                    for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        if (dt.Rows[0][i] == DBNull.Value)
                        {
                            dt.Columns.Remove(dt.Columns[i]);
                            i -= 1;
                        }
                    }
                }
                else
                {
                    if (IsSmallResolution)
                    {
                        dt.Rows[0][0] = columnName + ",<br> " + comboRegion.GetRootNodesName(0);
                    }
                    else
                    {
                        dt.Rows[0][0] = columnName + ", " + comboRegion.GetRootNodesName(0);
                    }
                }

                Chart.DataSource = dt;
            }
        }
        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                    if (!string.IsNullOrEmpty(te.Path))
                    if (te.Path.Contains("Legend"))
                    {
                        te.SetTextString(te.GetTextString().Replace("<br>",String.Empty));
                    }
                }
            }
        }
        #endregion
    }
}