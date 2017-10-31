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

namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0006
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Мониторинг естественного движения населения (по состоянию на выбранную дату)";
        private string sub_page_title = "Данные ежемесячного мониторинга естественного движения населения, {0} (по состоянию на {1}).";
        private string map_title = "{0} в {1}, человек";
        private string mapFolderName = "";
        private string legendTitle = "";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam compareType { get { return (UserParams.CustomParam("compareType")); } }
        private CustomParam selectedDate { get { return (UserParams.CustomParam("selectedDate")); } }
        private CustomParam selectedYears { get { return (UserParams.CustomParam("selectedYears")); } }
        private CustomParam mapPok { get { return (UserParams.CustomParam("mapPok")); } }
        private string style = "";
        private CellSet Population;
        private GridHeaderLayout headerLayout;
        private string bestBorn = "";
        private string worstBorn = "";
        private string bestDeath = "";
        private string worstDeath = "";
        private string bestGrow= "";
        private string worstGrow = "";
        private string compareDate = "";
        private DataTable grid_desription;
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
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.Towns.ToString();
        }
        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Grid.Width = (int)((minScreenWidth) - 15);
            DundasMap.Width = (int)((minScreenWidth) - 15);
            DundasMap.Height = 800;
            Grid.Height = Unit.Empty;
            Label1.Text = page_title;
            Page.Title = Label1.Text;
            comboDate.Width = 180;
            comboCompareType.Width = 450;
            CrossLink1.Text = "Мониторинг естественного движения населения (по муниципальному образованию)";
            CrossLink1.NavigateUrl = "~/reports/STAT_0003_0005/default.aspx";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            Grid.DisplayLayout.FixedCellStyleDefault.BackColor = Color.White;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                comboDate.Title = "Период";
                comboDate.FillDictionaryValues(DateLoad("Date"));
                comboDate.SelectLastNode();
                comboCompareType.Title = "Период для сравнения";
                comboCompareType.FillDictionaryValues(CompareTypeLoad());
                
                legendTitle = "Число родившихся";
            }
            mapPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число родившихся]";
            mapFolderName = "Субъекты/ХМАОDeer";
            compareType.Value = comboCompareType.SelectedValue;
            selectedDate.Value = String.Empty;
            selectedYears.Value = String.Empty;
            selectedDate.Value = AddStringWithSeparator(selectedDate.Value, StringToMDXDate(comboDate.SelectedNode.Text), ",\n");
            selectedYears.Value = AddStringWithSeparator(selectedYears.Value, StringToMDXYear(comboDate.SelectedNode.Parent.Text), ",\n");
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            Grid.Rows[0].Hidden = true;
            Grid.Rows[1].Hidden = true;
            Grid.Rows[2].Hidden = true;
            FormText();
            SetSfereparam();
            SetMapSettings();
            Label3.Text = String.Format(map_title, "Число родившихся", TransformMonth2(comboDate.SelectedValue));
            Label2.Text = String.Format(sub_page_title,RegionSettingsHelper.Instance.Name, comboDate.SelectedValue.ToLower());
        }
        public string StringToMDXDate(string str)
        {
            string template = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[1]);
            string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[0]));
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            return String.Format(template, year, halfYear, quarter, month);
        }

        public string StringToMDXYear(string str)
        {
            string template = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[0]);
            return String.Format(template, year);
        }

        protected string AddStringWithSeparator(string firstString, string secondString, string separator)
        {
            if (String.IsNullOrEmpty(firstString))
            {
                return secondString;
            }
            else
            {
                return firstString + separator + secondString;
            }
        }

        public string GetYearFromMDXDate(string mdxDate)
        {
            string[] separator = { "].[" };
            string[] mdxDateElements = mdxDate.Split(separator, StringSplitOptions.None);
            if (mdxDateElements.Length == 8)
            {
                return mdxDateElements[3];
            }
            else
            {
                return "2010";
            }
        }

        Dictionary<string, int> CompareTypeLoad()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("к предыдущему периоду", 0);
            d.Add("на начало года", 0);
            d.Add("к аналогичному периоду прошлого года", 0);
            return d;
        }
        Dictionary<string, int> DateLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                if ((cs.Axes[1].Positions[i].Members[0].LevelDepth == 1))
                {
                    if (cs.Axes[1].Positions[i + 1].Members[0].LevelDepth != 1)
                    {
                        d.Add(cs.Axes[1].Positions[i].Members[0].Caption + " год", 0);
                    }
                }
                if ((cs.Axes[1].Positions[i].Members[0].LevelDepth == 4))
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption + " " + cs.Axes[1].Positions[i].Members[0].Parent.Parent.Parent.Caption + " года", 1);
                }
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
        public string MDXDateToShortDateString(string mdxDateString)
        {
            if (String.IsNullOrEmpty(mdxDateString))
            {
                return null;
            }
            string[] separator = { "].[" };
            string[] dateElements = mdxDateString.Split(separator, StringSplitOptions.None);
            string template = "{0} {1} года";
            string month = dateElements[6].Replace("]", String.Empty).ToLower();
            string year = dateElements[3];
            return String.Format(template, month, year);
        }

        #region Обработчики грида

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            grid_desription = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid_Description"), "Даты", grid_desription);
            if (grid_desription.Rows[1].ItemArray[1] == DBNull.Value)
            {
                compareDate = "";
            }
            else
            {
                compareDate = MDXDateToShortDateString(grid_desription.Rows[1].ItemArray[1].ToString());
            }
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid1"), "Регион", dt);
            if (compareDate=="")
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0].ToString().EndsWith("прирост") || row[0].ToString().EndsWith("темп прироста"))
                    {
                        for (int j = 1; j < dt.Rows[0].ItemArray.Length; j++)
                        {
                            row[j] = DBNull.Value;
                        }
                    }
                }
            }
            Grid.DataSource = dt;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.19);
            e.Layout.Bands[0].Columns[0].Header.Caption = "МР ГО";
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            GridHeaderCell header = headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.12);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;

                header = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "#0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "#0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "#0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "#0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "#0");
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position: 25px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 75px"; }
            string compareType = "";
            if (comboCompareType.SelectedIndex == 0)
            {
                compareType = "предыдущей даты";
            }
            else
            {
                compareType = "начала года";
            }
            if ((e.Row.Index - 1) % 3 == 0)
            {
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        if (Grid.Columns[i].Header.Caption.StartsWith("Число мертворожденных") || Grid.Columns[i].Header.Caption.StartsWith("Число умерших"))
                        {
                            Grid.Rows[e.Row.Index].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        }
                        else
                        {
                            Grid.Rows[e.Row.Index].Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        }
                        Grid.Rows[e.Row.Index].Cells[i].Style.CustomRules = style;
                        if (Grid.Rows[e.Row.Index].Cells[i].Value != null && compareDate != "")
                        {
                            Grid.Rows[e.Row.Index].Cells[i].Title = "Снижение относительно " + TransformMonth(compareDate);
                        }
                    }
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        if (Grid.Columns[i].Header.Caption.StartsWith("Число мертворожденных") || Grid.Columns[i].Header.Caption.StartsWith("Число умерших"))
                        {
                            Grid.Rows[e.Row.Index].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        }
                        else
                        {
                            Grid.Rows[e.Row.Index].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        }
                        Grid.Rows[e.Row.Index].Cells[i].Style.CustomRules = style;
                        if (Grid.Rows[e.Row.Index].Cells[i].Value != null && compareDate != "")
                        {
                            Grid.Rows[e.Row.Index].Cells[i].Title = "Рост относительно " + TransformMonth(compareDate);
                        }
                    }
                }
            }
            if ((e.Row.Index + 1) % 3 == 0)
            {
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = Grid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0];
                Grid.Rows[e.Row.Index].Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 2].Cells[0].Text = "";

                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index].Cells[0].Style.BackColor = Color.White;
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index].Cells[i].Style.BackColor = Color.White;
                    if (Grid.Columns[i].Header.Caption.StartsWith("Естественный"))
                    {
                        e.Row.Cells[i].Text = "";
                    }
                    else
                    {
                        if ((compareDate != "") && (e.Row.Cells[i].Value != null))
                        {
                            e.Row.Cells[i].Text = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value));
                            e.Row.Cells[i].Title = "Темп прироста к " + TransformMonth1(compareDate);
                        }
                    }
                }
            }
        }

        protected void calculateRank(out string best, out string worst, UltraWebGrid Grid, int colNumber)
        {
            worst = "";
            best = "";
            int m = 0;
            for (int i = 3; i < Grid.Rows.Count; i += 3)//подсчет ненулевых значений столбца
            {
                if (Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value) != 0)
                {
                    m += 1;
                }
            }
            if (m == 0)
            {
                /* for (int j = 0; j < Grid.Rows.Count; j++)
                 {
                     if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value) == 0)
                     {
                         Grid.Rows[j].Cells[colNumber].Text = "";
                     }
                 }*/
            }
            else
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 3; i < Grid.Rows.Count; i += 3)
                {
                    if (Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value) != 0)
                    {
                        rank[m] = Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value);
                        m += 1;
                    }
                    Grid.Rows[i + 2].Cells[colNumber].Text = "0";
                }
                Array.Sort(rank);
                m = 1;
                if (colNumber != 5)
                {
                    m = 1;
                    best = String.Format("{0:0.00}", rank[rank.Length - 1]) + ";";
                    worst = String.Format("{0:0.00}", rank[0]) + ";";
                }
                else
                {
                    m = rank.Length;
                    worst = String.Format("{0:0.00}", rank[rank.Length - 1]) + ";";
                    best = String.Format("{0:0.00}", rank[0]) + ";";
                }
                for (int i = rank.Length - 1; i >= 0; i--)
                {
                    for (int j = 3; j < Grid.Rows.Count; j += 3)
                    {
                        if (rank[i] == Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value))
                        {
                            if (Grid.Rows[j + 2].Cells[colNumber].Text == "0")
                            {
                                Grid.Rows[j + 2].Cells[colNumber].Title = "Ранг по ХМАО-ЮГРЕ на " + compareDate + ": " + m.ToString();
                                Grid.Rows[j + 2].Cells[colNumber].Text = m.ToString();
                                if ((m) == 1)
                                {
                                    best += Grid.Rows[j].Cells[0].Text + ", ";
                                    Grid.Rows[j + 2].Cells[colNumber].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    Grid.Rows[j + 2].Cells[colNumber].Style.CustomRules = style;
                                }
                                if (m == rank.Length)
                                {
                                    worst += Grid.Rows[j].Cells[0].Text + ", ";
                                    Grid.Rows[j + 2].Cells[colNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    Grid.Rows[j + 2].Cells[colNumber].Style.CustomRules = style;
                                }
                            }
                        }
                    }
                    if (colNumber != 5)
                    {
                        m += 1;
                    }
                    else
                    { m -= 1; }
                }
                double max = 0;
                for (int j = 3; j < Grid.Rows.Count; j += 3)
                {
                    if (colNumber != 5)
                    {
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value) > max)
                        {
                            max = Convert.ToInt32(Grid.Rows[j].Cells[colNumber].Value);
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value) < max)
                        {
                            max = Convert.ToInt32(Grid.Rows[j].Cells[colNumber].Value);
                        }
                    }
                }
                for (int j = 3; j < Grid.Rows.Count; j += 3)
                {
                    if (colNumber != 5)
                    {
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value) == max)
                        {
                            worst += Grid.Rows[j].Cells[0].Text + ", ";
                            Grid.Rows[j + 2].Cells[colNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                            Grid.Rows[j + 2].Cells[colNumber].Style.CustomRules = style;
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value) == max)
                        {
                            best += Grid.Rows[j].Cells[0].Text + ", ";
                            Grid.Rows[j + 2].Cells[colNumber].Style.BackgroundImage = "~/images/starYellowBB.png";
                            Grid.Rows[j + 2].Cells[colNumber].Style.CustomRules = style;
                        }
                    }
                }

            }
            best = best.Remove(best.Length - 2);
            worst = worst.Remove(worst.Length - 2);
        }
        #endregion
       
        #region Формирование динамического текста
        protected void FormText()
        {
            
                DataTable dt = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("text"), "Текст", dt);
                string str1 = "&nbsp;&nbsp;&nbsp;{0} человек в целом по ХМАО-Югре в <b>{1}</b> по сравнению с <b>{2}</b> ";
                string str2 = "{0} на <b>{1}</b> человек ";
                string str3 = "и составило <b>{0}</b> человек.<br>";
                Label4.Text = "";
                string buf = "";
                if ((dt.Rows[1].ItemArray[1] == DBNull.Value)||(compareDate==""))
                {
                    str1 = "&nbsp;&nbsp;&nbsp;{0} человек в целом по ХМАО-Югре в <b>{1}</b> составило <b>{2}</b> человек.<br>";
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        buf = "";
                        buf += String.Format(str1, dt.Columns[i].ColumnName, TransformMonth2(comboDate.SelectedValue.ToLower()), Math.Abs(Convert.ToDouble(dt.Rows[0].ItemArray[i])));
                        if (i == 3)
                        {
                            if (Convert.ToDouble(dt.Rows[0].ItemArray[i]) < 0)
                            {
                                buf = buf.Replace("прирост", "убыль").Replace("Естественный", "Естественная").Replace("уменьшилось", "уменьшилась").Replace("возросло", "выросла").Replace("составило", "составила");
                            }
                            else
                            {
                                buf = buf.Replace("уменьшилось", "уменьшился").Replace("возросло", "вырос").Replace("составило", "составил");
                            }
                        }
                        Label4.Text += buf;
                    }
                }
                else
                {
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        buf = "";
                        buf += String.Format(str1, dt.Columns[i].ColumnName, TransformMonth2(comboDate.SelectedValue.ToLower()), TransformMonth3(compareDate));
                        if (Convert.ToDouble(dt.Rows[1].ItemArray[i]) < 0)
                        {
                            if (dt.Columns[i].ColumnName.StartsWith("Число умерших"))
                            {
                                buf += String.Format(str2, "&nbsp;<img src='../../images/arrowGreenDownBB.png'>уменьшилось", String.Format("{0:0}", Math.Abs(Convert.ToDouble(dt.Rows[1].ItemArray[i]))));
                            }
                            else
                            {
                                buf += String.Format(str2, "&nbsp;<img src='../../images/arrowRedDownBB.png'>уменьшилось", String.Format("{0:0}", Math.Abs(Convert.ToDouble(dt.Rows[1].ItemArray[i]))));
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(dt.Rows[1].ItemArray[i]) > 0)
                            {
                                if (dt.Columns[i].ColumnName.StartsWith("Число умерших"))
                                {
                                    buf += String.Format(str2, "&nbsp;<img src='../../images/arrowRedUpBB.png'>возросло", String.Format("{0:0}", Math.Abs(Convert.ToDouble(dt.Rows[1].ItemArray[i]))));
                                }
                                else
                                {
                                    buf += String.Format(str2, "&nbsp;<img src='../../images/arrowGreenUpBB.png'>возросло", String.Format("{0:0}", Math.Abs(Convert.ToDouble(dt.Rows[1].ItemArray[i]))));
                                }
                            }
                            else
                            {
                                buf += "не изменилось";
                            }
                        }
                        buf += String.Format(str3, String.Format("{0:0}", Math.Abs(Convert.ToDouble(dt.Rows[0].ItemArray[i]))));
                        if (i == 3)
                        {
                            if (Convert.ToDouble(dt.Rows[0].ItemArray[i]) < 0)
                            {
                                buf = buf.Replace("прирост", "убыль").Replace("Естественный", "Естественная").Replace("уменьшилось", "уменьшилась").Replace("возросло", "выросла").Replace("составило", "составила");
                            }
                            else
                            {
                                buf = buf.Replace("уменьшилось", "уменьшился").Replace("возросло", "вырос").Replace("составило", "составил");
                            }
                        }
                        Label4.Text += buf;
                    }
                }
        }
        #endregion 

        protected string TransformMonth(string date)
        {
            
            string month = date.Split(' ')[0].ToLower();
            date = date.Remove(0, month.Length);
            if ((month.Contains("март")) || (month.Contains("август")))
            {
                return (month + "а" + date);
            }
            else
            {
                return (month.Remove(month.Length - 1) + "я" + date);
            }

        }

        protected string TransformMonth1(string date)
        {
            string month = date.Split(' ')[0].ToLower();
            date = date.Remove(0, month.Length);
            if ((month.Contains("март")) || (month.Contains("август")))
            {
                return (month + "у" + date);
            }
            else
            {
                return (month.Remove(month.Length - 1) + "ю" + date);
            }

        }

        protected string TransformMonth2(string date)
        {
            string month = date.Split(' ')[0].ToLower();
            date = date.Remove(0, month.Length);
            if ((month.Contains("март")) || (month.Contains("август")))
            {
                return (month + "е" + date);
            }
            else
            {
                return (month.Remove(month.Length - 1) + "е" + date);
            }

        }

        protected string TransformMonth3(string date)
        {
            string month = date.Split(' ')[0].ToLower();
            date = date.Remove(0, month.Length);
            if ((month.Contains("март")) || (month.Contains("август")))
            {
                return (month + "ом" + date);
            }
            else
            {
                return (month.Remove(month.Length - 1) + "ем" + date);
            }

        }

        #region Добавление checkbox
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            //DataTable dt = new DataTable();
            string[] buttonNames = new string[5];
            buttonNames[0] = "Число родившихся";
            buttonNames[1] = "Число мертворожденных";
            buttonNames[2] = "Число умерших";
            buttonNames[3] = "Число умерших детей в возрасте до 1 года";
            buttonNames[4] = "Естественный прирост (убыль) населения";
            //   DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("AreasChart"), "dfdf", dt);
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
            legendTitle = rb.Text;
            Label3.Text = String.Format(map_title, rb.Text, TransformMonth2(comboDate.SelectedValue));
            if (rb.Text == "Число родившихся")
            {
                mapPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число родившихся]";
            }
            if (rb.Text == "Число умерших")
            {
                mapPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число умерших]";
            }
            if (rb.Text == "Число мертворожденных")
            {
                mapPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число мертворожденных]";
            }
            if (rb.Text == "Число умерших детей в возрасте до 1 года")
            {
                mapPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число умерших детей в возрасте до 1 года]";
            }
            if (rb.Text == "Естественный прирост (убыль) населения")
            {
                mapPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Естественный прирост населения]";
            }
            SetMapSettings();
        }
        #endregion

        #region Обработчики карты
        Color GetColor(Double val)
        {
            if (val < 1)
            {
                if (revPok)
                {
                    return Color.Green;
                }
                else
                {
                    return Color.Red;
                }
            }

            if (val < 2)
            {
                if (revPok)
                {
                    return Color.FromArgb(128,192,0);
                }
                else
                {
                    return Color.Orange;
                }
            }
            if (val < 3)
            {

                    return Color.Yellow;
                
            }
            if (val < 4)
            {
                if (revPok)
                {
                    return Color.Orange;
                }
                else
                {
                    return Color.FromArgb(128, 192, 0);
                }
            }
            if (revPok)
            {
                return Color.Red;
            }
            else
            {
                return Color.Green;
            }

        }

        DataTable dtMap;
        bool smallLegend = false;
        bool revPok = false;
        public void SetMapSettings()
        {
            if (legendTitle == "")
            {
                legendTitle = "Число родившихся";
            }
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 98;
            DundasMap.ColorSwatchPanel.Visible = false;

            DundasMap.Legends.Clear();
            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.Dock = PanelDockStyle.Right;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = legendTitle + ", чел.";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            
            dtMap = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Map"), "Карта", dtMap);
            double[] values = new double[dtMap.Rows.Count];
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                values[i] = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
            }
            Array.Sort(values);
            if (legendTitle.StartsWith("Число мертворожденных") || legendTitle.Contains("Число умерших"))
            {
                revPok = true;
            }
            else
            {
                revPok = false;
            }
            if (Math.Abs(values[0] - values[values.Length - 1] )<= 4)
            {
                smallLegend = true;
                LegendItem item = new LegendItem();
                item.Text = String.Format("0");
                item.Color = GetColor(0);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("1");
                item.Color = GetColor(1);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("2");
                item.Color = GetColor(2);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("3");
                item.Color = GetColor(3);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("4");
                item.Color = GetColor(4);
                DundasMap.Legends["CostLegend"].Items.Add(item);
            }
            else
            {
                smallLegend = false;
                ShapeRule rule = new ShapeRule();
                rule.Name = "CostRule";
                rule.Category = String.Empty;
                rule.ShapeField = "Cost";
                rule.DataGrouping = DataGrouping.EqualDistribution;
                rule.ColorCount = 5;
                rule.ColoringMode = ColoringMode.ColorRange;
                if (revPok)
                {
                    rule.FromColor = Color.Green;
                    rule.MiddleColor = Color.Yellow;
                    rule.ToColor = Color.Red;
                }
                else
                {
                    rule.FromColor = Color.Red;
                    rule.MiddleColor = Color.Yellow;
                    rule.ToColor = Color.Green;
                }
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInLegend = "CostLegend";

                rule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
                DundasMap.ShapeRules.Add(rule);

            }
            
            // добавляем поля
            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");
            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

            // добавляем правила расстановки символов

            // звезды для легенды


            AddMapLayer(DundasMap, mapFolderName, "Граница", CRHelper.MapShapeType.Areas);
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.CalloutTowns);
            
            FillMapData();

        }

        public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
        {
            hasCallout = false;
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (shape.Name.ToLower() == patternValue.ToLower())
                {
                    shapeList.Add(shape);
                    if (IsCalloutTownShape(shape))
                    {
                        hasCallout = true;
                    }
                }
            }

            return shapeList;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Clear();
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        public void FillMapData()
        {
                bool hasCallout;
                string valueSeparator = IsMozilla ? ". " : "\n";
                string shapeHint = "";
                
                if (mapPok.Value.Contains("Число родившихся"))
                {
                    shapeHint = "Число родившихся {0}";
                }
                if (mapPok.Value.Contains("Число умерших"))
                {
                    shapeHint = "Число умерших {0}";
                }
                if (mapPok.Value.Contains("Число мертворожденных"))
                {
                    shapeHint = "Число мертворожденных {0}";
                }
                if (mapPok.Value.Contains("Число умерших детей в возрасте до 1 года"))
                {
                    shapeHint = "Число умерших детей в возрасте до 1 года {0}";
                }
                if (mapPok.Value.Contains("прирост"))
                {
                    shapeHint = "Естественный прирост (убыль) населения {0}";
                }

                Dictionary<string, int> colTown = new Dictionary<string, int>();
                int indTown = 1;
                for (int i = 0; i < dtMap.Rows.Count; i++)
                {
                    if (dtMap.Rows[i].ItemArray[0].ToString().StartsWith("Город"))
                    {
                        colTown.Add(dtMap.Rows[i].ItemArray[0].ToString().Replace(" муниципальный район", " р-н").Replace("Город ", "г. "), indTown);
                        indTown += 1;
                    }
                }

                for (int j = 0; j < dtMap.Rows.Count; j++)
                {

                    string subject = dtMap.Rows[j].ItemArray[0].ToString().Replace(" муниципальный район", " р-н").Replace("Город ", "г. ");
                    double value = Convert.ToDouble(dtMap.Rows[j].ItemArray[1].ToString());
                    ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
                    foreach (Shape shape in shapeList)
                    {
                        shape.Visible = true;
                        shape["Name"] = subject;
                        shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                        
                        if (subject.StartsWith("г."))
                        {
                            shape.ToolTip = String.Format(shapeHint, String.Format("{0:0}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]))) +", " + subject.Split('.')[1];
                            if (shape.Name.Contains("Нижневартовск") || shape.Name.Contains("Ханты-Мансийск") || shape.Name.Contains("Сургут") || shape.Name.Contains("Нефтеюганск"))
                            {
                                shape.CentralPointOffset.Y += 5000;
                            }
                            else
                            {
                                shape.CentralPointOffset.Y += 2100;
                            }
                            shape.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                        }
                        else
                        {
                            shape.ToolTip = String.Format(shapeHint, String.Format("{0:0}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])))+", "  + subject.Replace("р-н",String.Empty) +"МР";
                        }
                        shape.Text = subject + "\n"+String.Format("{0:0}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                        shape.Name = subject;
                        shape.TextVisibility = TextVisibility.Shown;
                        if (smallLegend)
                        {
                            shape.Color = GetColor(Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
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
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.Width = 900;
            DundasMap.Height = 600;
            sheet2.Rows[2].Cells[0].Value = Label3.Text;
            ReportExcelExporter.MapExcelExport(sheet2.Rows[3].Cells[0], DundasMap);
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            
            
           
           // Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            
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
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Font.Name = "Verdana";
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Font.Height = 200;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Label4.Text = Label4.Text.Replace("<br>", "\n");
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text + "\n\n" + Label4.Text+"\n\n";
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 80;
            Grid.Width = 800;
            ReportPDFExporter1.Export(headerLayout, section1);
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;


            section2.PageOrientation = PageOrientation.Landscape;
            section2.PagePaddings = new Paddings(20,30);
            section2.PageAlignment = Infragistics.Documents.Reports.Report.ContentAlignment.Center;
            IText title = section2.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font = font;
            title.AddContent(Label3.Text+"\n\n   ");
            DundasMap.Width = 900;
            DundasMap.Height = 600;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            section2.AddImage(img);
            Grid.Width = (int)((minScreenWidth) - 15);
        }
        void ultraWebGridDocumentExporter_RowExporting(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportingEventArgs e)
        {
            e.GridRow.Height = 500;
        }
        #endregion

    }
}
