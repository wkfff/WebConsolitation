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
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0009_Sahalin
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Мониторинг видов проведенных операций (по муниципальным образованиям)";
        private string sub_page_title = "Данные ежегодного мониторинга видов проведенных операций, {0}";
        private string chart_title = "Анализ динамики показателей в разрезе видов операций за {0} год, {1}";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedDate { get { return (UserParams.CustomParam("selectedDate")); } }
        private string style = "";
        private object[] edIsm;
        private GridHeaderLayout headerLayout;
        string[] operationType;
        Color[] colorLegendArray;
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
            ComboDate.Width = 200;
            comboRegion.Width = 350;
            if (IsSmallResolution)
            {
                Grid.Width = minScreenWidth;
                Chart.Width = minScreenWidth - 5;
            }
            else
            {
                Grid.Width = (int)(minScreenWidth - 15);
                Chart.Width = (int)(minScreenWidth - 20);
            }
            Label1.Text = page_title;
            Page.Title = page_title;
            CrossLink1.Text = "Мониторинг видов проведенных операций (по состоянию на выбранную дату)";
            CrossLink1.NavigateUrl = "~/reports/STAT_0003_0010/default.aspx";
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            Chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight*0.8);
            Chart.ChartType = ChartType.StackColumnChart;
            Chart.ColumnChart.NullHandling = NullHandling.DontPlot;
            Chart.Border.Thickness = 0;
            Chart.Axis.X.Extent = 20;
            Chart.Axis.X.LineThickness = 1;
            Chart.Axis.Y.Extent = 50;
            Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            Chart.Axis.X.Labels.SeriesLabels.Visible = true;
            Chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            Chart.Axis.X.Margin.Near.Value = 17;
            Chart.ColorModel.ModelStyle = ColorModels.PureRandom;

            Chart.Legend.Visible = true;
            Chart.Legend.Location = LegendLocation.Bottom;
            Chart.Legend.SpanPercentage = 15;
            Chart.Border.Thickness = 0;
            
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                ComboDate.Title = "Период";
                ComboDate.FillDictionaryValues(DateLoad("Date"));
                ComboDate.SelectLastNode();
                comboRegion.Title = "Территория";
                comboRegion.FillDictionaryValues(RegionsLoad("Regions"));
            }
            baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Дальневосточный федеральный округ].[Сахалинская область].[Сахалинская обл.].[" + comboRegion.SelectedValue + "]";
            selectedDate.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboDate.SelectedValue.Split(' ')[0] + "]";
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            if (Grid.DataSource != null)
            {
                Chart.DataBind(); 
                FormText();
            }
            else
            {

            }
            Label2.Text = String.Format(sub_page_title, comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г."));
            Label3.Text = String.Format(chart_title, ComboDate.SelectedValue.Split(' ')[0], comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г."));
            
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

        Dictionary<string, int> DateLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption+" год", 0);
            }
            return d;
        }

        protected string GenitiveTransform(double number, string s)
        {
            number = Math.Abs(number);
            if ((number % 10 == 1) && ((number % 100 == 1) || (number % 100 >= 21)))
            {
                if (s.EndsWith("ц"))
                {
                    return s + "у";
                }
                else
                {
                    return s + "а";
                }
            }
            else
            {
                if ((number % 10 <= 4) && (number % 10 != 0) && ((number % 100).ToString()[0] != '1'))
                {
                    if (s.EndsWith("ц"))
                    {
                        return s+"ы";
                    }
                    else
                    {
                        return s + "а";
                    } 
                }
                else
                {
                    if (s.EndsWith("ц"))
                    {
                        return s;
                    }
                    else
                    {
                        return s;
                    }
                }
            }
        }

        protected string GenitiveTransform1(double number, string s)
        {
            number = Math.Abs(number);
            if ((number % 10 == 1) && ((number %100 == 1) || (number % 100 >= 21)))
            {
                if (s.EndsWith("ц"))
                {
                    return s + "у";
                }
                else
                {
                    return s;
                }
            }
            else
            {
                if ((number % 10 <= 4) && (number % 10 != 0)&&((number%100).ToString()[0]!='1'))
                {
                    if (s.EndsWith("ц"))
                    {
                        return s+"ы";
                    }
                    else
                    {
                        return s + "а";
                    }
                }
                else
                {
                    if (s.EndsWith("ц"))
                    {
                        return s;
                    }
                    else
                    {
                        return s;
                    }
                }
            }
        }

        #region Обработчики грида
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатель", dt);
            if (dt.Rows.Count > 1)
            {
                edIsm = new object[dt.Rows.Count / 4];
                int edCol = 0;
                for (int i = 0; i < dt.Rows.Count; i += 4)
                {
                    if (dt.Rows[i][1].ToString().EndsWith("а"))
                    {
                        edIsm[edCol] = dt.Rows[i][1].ToString().Remove(dt.Rows[i][1].ToString().Length - 1);
                    }
                    else
                    {
                        edIsm[edCol] = dt.Rows[i][1];
                    }
                    edCol += 1;
                }
                edCol = -1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().EndsWith("Ед"))
                    {
                        dt.Rows.Remove(dt.Rows[i]);
                        edCol += 1;
                        i -= 1;
                    }
                    else
                    {
                        dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0] + ", " + edIsm[edCol].ToString().ToLower();
                    }
                }
                Grid.DataSource = dt;
            }
            else
            {
                Grid.DataSource = null;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double widthCoef = 0;
            if (IsSmallResolution)
            {
                widthCoef = 0.11;
            }
            else
            {
                widthCoef = 0.07;
            }
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.14);
            e.Layout.Bands[0].Columns[0].Header.Caption = "Показатель";
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            GridHeaderCell header = headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            e.Layout.Bands[0].Columns[1].Hidden = true;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * widthCoef);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                header = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position:30px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 40px"; }

            if ((e.Row.Index + 1) % 3 == 0)
            {

                e.Row.Cells[0].Text = "";
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
                    if (ComboDate.SelectedIndex != 0)
                    {
                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Снижение относительно " + ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (Grid.Rows[e.Row.Index - 1].Cells[0].Text.StartsWith("Умерло"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }
                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Рост относительно " + ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (Grid.Rows[e.Row.Index - 1].Cells[0].Text.StartsWith("Умерло"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }

                        if ((e.Row.Cells[i].Value != null))
                        {
                            e.Row.Cells[i].Title = "Темп прироста к " + ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0] + " году";
                            e.Row.Cells[i].Value = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }
                }


            }
        }
        #endregion

        #region Формирование динамического текста
        protected void FormText()
        {
            Label4.Text = "";
            string buf = "";
            string str1 = "Общее {0} в <b>{1} году</b> ";
            string str2 = " по сравнению с <b>{0} годом</b>";
            string str3 = "{0} на <b>{1}</b> {2} и";
            string str4 = " составило <b>{0}</b> {1}";

            for (int i = 1; i < Grid.Rows.Count; i += 3)
            {
                buf = "";
                if (ComboDate.SelectedIndex != 0)
                {
                    if (i != 1)
                    {
                        if (Grid.Rows[i].Cells[0].Text.Remove(8) == Grid.Rows[i - 3].Cells[0].Text.Remove(8))
                        {
                            buf += ", в том числе " + String.Format(str1, Grid.Rows[i].Cells[0].Text.Split(',')[0].ToLower(), ComboDate.SelectedValue.Split(' ')[0]).ToLower();
                        }
                        else
                        {
                            buf += String.Format(str1, Grid.Rows[i].Cells[0].Text.Split(',')[0].ToLower(), ComboDate.SelectedValue.Split(' ')[0]);
                        }
                    }
                    else
                    {
                        buf += String.Format(str1, Grid.Rows[i].Cells[0].Text.Split(',')[0].ToLower(), ComboDate.SelectedValue.Split(' ')[0]);
                    }
                    buf += String.Format(str2, ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0]);

                    if (Convert.ToDouble(Grid.Rows[i].Cells[1].Value) > 0)
                    {
                        if (Grid.Rows[i].Cells[0].Text.StartsWith("Умерло после"))
                        {
                            buf += String.Format(str3, "&nbsp;<img src='../../images/arrowRedUpBB.png'>выросло", Math.Abs(Convert.ToDouble(Grid.Rows[i].Cells[1].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[i].Cells[1].Value), Grid.Rows[i].Cells[0].Text.Split(',')[1]));
                        }
                        else
                        {
                            buf += String.Format(str3, "&nbsp;<img src='../../images/arrowGreenUpBB.png'>выросло", Math.Abs(Convert.ToDouble(Grid.Rows[i].Cells[1].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[i].Cells[1].Value), Grid.Rows[i].Cells[0].Text.Split(',')[1]));
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(Grid.Rows[i].Cells[1].Value) < 0)
                        {
                            if (Grid.Rows[i].Cells[0].Text.StartsWith("Умерло после"))
                            {
                                buf += String.Format(str3, "&nbsp;<img src='../../images/arrowGreenDownBB.png'>уменьшилось", Math.Abs(Convert.ToDouble(Grid.Rows[i].Cells[1].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[i].Cells[1].Value), Grid.Rows[i].Cells[0].Text.Split(',')[1]));
                            }
                            else
                            {
                                buf += String.Format(str3, "&nbsp;<img src='../../images/arrowGreenDownBB.png'>уменьшилось", Math.Abs(Convert.ToDouble(Grid.Rows[i].Cells[1].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[i].Cells[1].Value), Grid.Rows[i].Cells[0].Text.Split(',')[1]));
                            }
                        }
                        else
                        {
                            buf += " не изменилось";
                        }


                    }
                    buf += String.Format(str4, Grid.Rows[i - 1].Cells[1].Text, GenitiveTransform1(Convert.ToDouble(Grid.Rows[i - 1].Cells[1].Value), Grid.Rows[i].Cells[0].Text.Split(',')[1]));
                }
                else
                {
                    if (i != 1)
                    {
                        if (Grid.Rows[i].Cells[0].Text.Remove(8) == Grid.Rows[i - 3].Cells[0].Text.Remove(8))
                        {
                            buf += ", в том числе" + String.Format(str1, Grid.Rows[i].Cells[0].Text.Split(',')[0].ToLower(), ComboDate.SelectedValue.Split(' ')[0]).ToLower();
                            buf += String.Format(str4, Grid.Rows[i - 1].Cells[1].Text, GenitiveTransform1(Convert.ToDouble(Grid.Rows[i - 1].Cells[1].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                        }
                        else
                        {
                            buf += String.Format(str1, Grid.Rows[i].Cells[0].Text.Split(',')[0].ToLower(), Grid.Rows[i].Cells[0].Text.Split(',')[1]);
                            buf += String.Format(str4, Grid.Rows[i - 1].Cells[1].Text, GenitiveTransform1(Convert.ToDouble(Grid.Rows[i - 1].Cells[1].Value), Grid.Rows[i].Cells[0].Text.Split(',')[1]));
                        }
                    }
                    else
                    {
                        buf += String.Format(str1, Grid.Rows[i].Cells[0].Text.Split(',')[0].ToLower(), Grid.Rows[i].Cells[0].Text.Split(',')[1]);
                        buf += String.Format(str4, Grid.Rows[i - 1].Cells[1].Text, GenitiveTransform1(Convert.ToDouble(Grid.Rows[i - 1].Cells[1].Value), Grid.Rows[i].Cells[0].Text.Split(',')[1]));
                    }
                }
                Label4.Text += buf;
            }

            Label4.Text = Label4.Text.Replace("Общее", "<br>&nbsp;&nbsp;&nbsp;Общее").Replace("умерло", "число умерших");
            Label4.Text = Label4.Text.Remove(0, 4);
        }
        #endregion

        #region Обработчики диаграммы
        DataTable dtChart;
        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            DataTable resDt = new DataTable();
            
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Виды операций", dtChart);
            colorLegendArray = new Color[dtChart.Columns.Count - 1];
            resDt = dtChart.Copy();
            if (resDt.Columns.IndexOf("Умерло после проведенных операций1") > 0 && resDt.Columns.IndexOf("Умерло после проведенных операций1") < resDt.Columns.Count)
            {
                resDt.Columns.Remove("Умерло после проведенных операций1");
            }
            if (resDt.Columns.IndexOf("Умерло после проведенных операций с применением высоких медицинских технологий") > 0 && resDt.Columns.IndexOf("Умерло после проведенных операций с применением высоких медицинских технологий") < resDt.Columns.Count)
            {
                resDt.Columns.Remove("Умерло после проведенных операций с применением высоких медицинских технологий");
            }
            for (int i = 1; i < resDt.Columns.Count; i++)
            {
                string s = resDt.Columns[i].ColumnName;
                switch (s)
                {
                    case "Число проведенных операций1": { colorLegendArray[i - 1] = Color.Green; break; }
                    case "Число проведенных операций с применением высоких медицинских технологий": { colorLegendArray[i - 1] = Color.Yellow; break; }
                }
                if (resDt.Columns[i].ColumnName == "Число проведенных операций1")
                {
                    resDt.Columns[i].ColumnName = "Число проведенных операций без применения высоких медицинских технологий";
                    dtChart.Columns[i].ColumnName = "Число проведенных операций без применения высоких медицинских технологий";
                }
                else
                {
                    if (resDt.Columns[i].ColumnName == "Умерло после проведенных операций1")
                    {
                        resDt.Columns[i].ColumnName = "Умерло после проведенных операций без применения высоких медицинских технологий";
                        dtChart.Columns[i].ColumnName = "Умерло после проведенных операций без применения высоких медицинских технологий";
                    }
                }
            }
            int colOper = 0;
            operationType = new string[resDt.Rows.Count / 2];
            if (resDt.Rows.Count > 0)
            {

                if (resDt.Columns.Count >= 2)
                {
                    for (int i = 0; i < resDt.Rows.Count; i+=2)
                    {
                            operationType[colOper] = resDt.Rows[i][0].ToString().Split(';')[0];
                            colOper += 1;
                           /* if (dtChart.Columns[dtChart.Columns.Count - 1].ColumnName.StartsWith("Умерло после"))
                            {
                                dtChart.Rows[i][dtChart.Columns.Count - 1] = dtChart.Rows[i - 1][dtChart.Columns.Count - 1];
                                dtChart.Rows[i - 1][dtChart.Columns.Count - 1] = DBNull.Value;
                            }
                            if (dtChart.Columns[dtChart.Columns.Count - 2].ColumnName.StartsWith("Умерло после"))
                            {
                                dtChart.Rows[i][dtChart.Columns.Count - 2] = dtChart.Rows[i - 1][dtChart.Columns.Count - 2];
                                dtChart.Rows[i - 1][dtChart.Columns.Count - 2] = DBNull.Value;
                            }*/

                            resDt.Rows[i+1][0] = "Нет данных";
                        
                    }
                }
                else
                {
                    for (int i = 1; i < resDt.Rows.Count; i+=2)
                    {
                        operationType[colOper] = resDt.Rows[i][0].ToString().Split(';')[0];
                        colOper += 1;
                        resDt.Rows[i+1][0] = "Нет данных";
                    }
                }

                Label5.Text = "";
                Label6.Text = "";
                for (int i = 0; i < operationType.Length; i++)
                {
                    if (i < 8)
                    {
                        Label5.Text += (i + 1).ToString() + ". " + operationType[i] + "<br>";
                    }
                    else
                    {
                        Label6.Text += (i + 1).ToString() + ". " + operationType[i] + "<br>";
                    }
                }
               // Chart.DataSource = dtChart;
                 Chart.Series.Clear();
                 Chart.Data.SwapRowsAndColumns = true;
                 for (int i = 1; i < resDt.Columns.Count; i++)
                 {
                     NumericSeries series = CRHelper.GetNumericSeries(i, resDt);
                     Chart.Series.Add(series);
                 }
            }
            else
            {
                Chart.DataSource = null;
                Label5.Text = "Нет данных";
                Label6.Text = "";
            }
            Chart.Tooltips.FormatString = "<ITEM_LABEL>";
        }

        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

            return;
            int colOper1 = 1;
            int colBox = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Box)
                {
                    Infragistics.UltraChart.Core.Primitives.Box box = (Infragistics.UltraChart.Core.Primitives.Box)primitive;
                    if (box.DataPoint != null)
                    {
                        
                        if (box.DataPoint.Label == "Число проведенных операций без применения высоких медицинских технологий")
                        {
                            box.PE.Fill = Color.Green;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                            box.DataPoint.Label +=" "+dtChart.Rows[box.Row][box.Column+1].ToString();

                            if (dtChart.Columns.IndexOf("Умерло после проведенных операций1") >= 0 && dtChart.Columns.IndexOf("Умерло после проведенных операций1") < dtChart.Columns.Count)
                            {
                                double m = 1;
                                if (double.TryParse(dtChart.Rows[box.Row][dtChart.Columns.IndexOf("Умерло после проведенных операций1")].ToString(),out m) && m!= 0)
                                {
                                    box.DataPoint.Label += "<br>Умерло после проведенных операций без применения высоких медицинских технологий " + dtChart.Rows[box.Row][dtChart.Columns.IndexOf("Умерло после проведенных операций1")].ToString();
                                }
                            }
                        }
                        if (box.DataPoint.Label == "Число проведенных операций с применением высоких медицинских технологий")
                        {
                            box.PE.Fill = Color.Yellow;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                            box.DataPoint.Label +=" "+ dtChart.Rows[box.Row][box.Column + 1].ToString();
                            if (dtChart.Columns.IndexOf("Умерло после проведенных операций с применением высоких медицинских технологий") >= 0 && dtChart.Columns.IndexOf("Умерло после проведенных операций с применением высоких медицинских технологий") < dtChart.Columns.Count)
                            {
                                double m = 1;
                                if (double.TryParse(dtChart.Rows[box.Row][dtChart.Columns.IndexOf("Умерло после проведенных операций с применением высоких медицинских технологий")].ToString(),out m) && m!=0)
                                {
                                    box.DataPoint.Label += "<br>Умерло после проведенных операций с применением высоких медицинских технологий " + dtChart.Rows[box.Row][dtChart.Columns.IndexOf("Умерло после проведенных операций с применением высоких медицинских технологий")].ToString();
                                }
                            }
                        }
                        if (box.DataPoint.Label == "Умерло после проведенных операций без применения высоких медицинских технологий")
                        {
                            box.PE.Fill = Color.Red;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                        }
                        if (box.DataPoint.Label == "Умерло после проведенных операций с применением высоких медицинских технологий")
                        {
                            box.PE.Fill = Color.Gray;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                        }
                    }
                    else
                    {
                        if ((box.Column <= (colorLegendArray.Length - 1)) && (box.rect.Width < 60))
                        {
                            box.PE.Fill = colorLegendArray[colBox];
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                            colBox += 1;
                        }
                    }
                }
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    Infragistics.UltraChart.Core.Primitives.Text text = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                    if (text.GetTextString().Contains("За период"))
                    {
                        string s = text.GetTextString();
                        text.SetTextString(colOper1.ToString());
                        colOper1 += 1;
                    }
                    if (text.GetTextString().Contains("Нет данных"))
                    {
                        text.Visible = false;
                    }
                }
            }

        }
        #endregion

        #region Экспорт в Excel
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
            ReportExcelExporter1.Export(Chart, Label3.Text, sheet2, 2);
            sheet2.Rows[1].Cells[0].Value = Label3.Text;
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
            e.Workbook.Worksheets["Таблица"].Rows[3].Height = 900;
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 950;
            e.Workbook.Worksheets["Таблица"].Rows[4].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Font.Name = "Verdana";
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Font.Height = 200;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            Label4.Text = Label4.Text.Replace("<br>", "\n");
            ReportPDFExporter1.PageSubTitle = Label2.Text + "\n\n" + Label4.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Label4.Text = Label4.Text.Replace("\n", "<br>");
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            int width = int.Parse(Chart.Width.Value.ToString());
            Chart.Width = 1000;
            ReportPDFExporter1.HeaderCellHeight = 60;
            int width1 = int.Parse(Chart.Width.Value.ToString());
            Grid.Width = 800;
            Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            ReportPDFExporter1.Export(headerLayout, section1);
            Grid.Width = width1;
            ReportPDFExporter1.Export(Chart, Label3.Text, section2);
            Chart.Width = width;
        }
        #endregion

        
    }
}