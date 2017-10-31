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


namespace Krista.FM.Server.Dashboards.reports.STAT_0003_00050
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Мониторинг естественного движения населения";
        private string sub_page_title = "Данные ежемесячного мониторинга естественного движения населения, {0}.";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam compareType { get { return (UserParams.CustomParam("compareType")); } }
        private CustomParam selectedDate { get { return (UserParams.CustomParam("selectedDate")); } }
        private CustomParam selectedYears { get { return (UserParams.CustomParam("selectedYears")); } }
        private CustomParam chartPok { get { return (UserParams.CustomParam("chartPok")); } }
        private string style = "";
        private string columnName = "";
        private CellSet Population;
        private GridHeaderLayout headerLayout;
        DataTable grid_desription;
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
           
            if (IsSmallResolution)
            { 
                Grid.Width = minScreenWidth;
                Chart.Width = minScreenWidth;
            }
            else
            {
                Grid.Width = (int)(minScreenWidth-15);
                Chart.Width = (int)(minScreenWidth-20);
            }
            Label1.Text = page_title;
            
            Page.Title = Label1.Text;
            if (IsSmallResolution)
            {
                comboDate.Width = 140;
                comboRegion.Width = 282;
                comboCompareType.Width = 220;
                comboDate.ShowSelectedValue = false;
                comboRegion.ShowSelectedValue = false;
                comboCompareType.ShowSelectedValue = false;
                CrossLink1.Text = "Мониторинг естественного движения населения";
                comboRegion.PanelHeaderTitle = "Территория";
                comboDate.PanelHeaderTitle = "Период";
                comboCompareType.PanelHeaderTitle = "Период для сравнения";
            }
            else
            {
                comboDate.Width = 180;
                comboRegion.Width = 350;
                comboCompareType.Width = 320;
                comboDate.ShowSelectedValue = true;
                comboRegion.ShowSelectedValue = true;
                comboCompareType.ShowSelectedValue = true;
                CrossLink1.Text = "Мониторинг естественного движения населения (по состоянию на выбранную дату)";
                comboRegion.Title = "Территория";
                comboDate.Title = "Период";
                comboCompareType.Title = "Период для сравнения";
                
            }
            
            CrossLink1.NavigateUrl = "~/reports/STAT_0003_0006/default.aspx";

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
                comboRegion.FillDictionaryValues(RegionsLoad("Regions"));
                comboDate.MultiSelect = true;
                comboDate.FillDictionaryValues(DateLoad("Date"));
                comboDate.SetСheckedState(comboDate.GetRootNodesName(comboDate.GetRootNodesCount() - 1), true);
                comboCompareType.FillDictionaryValues(CompareTypeLoad());
            }
            if (IsSmallResolution)
            {
                for (int i = 0; i < comboCompareType.GetRootNodesCount(); i++)
                {
                    comboCompareType.SetNodeStyle(comboCompareType.GetRootNodesName(i), "font-size:7pt;font-family:Verdana;padding-left:0px;");
                }
                for (int i = 0; i < comboRegion.GetRootNodesCount(); i++)
                {
                    comboRegion.SetNodeStyle(comboRegion.GetRootNodesName(i), "font-size:7pt;font-family:Verdana;padding-left:0px;");
                }
            }
            compareType.Value = comboCompareType.SelectedValue;
            chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число родившихся]";
            if (comboRegion.SelectedIndex == 0)
            {
                baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра].DATAMEMBER";
            }
            else
            {
                baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра].[" + comboRegion.SelectedValue + "],[Группировки__Население_Естественное движение].[Все группировки].[По городам и районам]";
            }
            selectedDate.Value = String.Empty;
            selectedYears.Value = String.Empty;
            foreach (Node node in comboDate.SelectedNodes)
            {
                if (node.Level == 1)
                {
                    selectedDate.Value = AddStringWithSeparator(selectedDate.Value, StringToMDXDate(node.Text), ",\n");
                    if (!selectedYears.Value.Contains(node.Text.Split(' ')[1]))
                    {
                        selectedYears.Value = AddStringWithSeparator(selectedYears.Value, StringToMDXYear(node.Text), ",\n");
                    }
                }
                else if (node.Level == 0)
                {
                    
                    foreach (Node monthNode in node.Nodes)
                    {
                        selectedDate.Value = AddStringWithSeparator(selectedDate.Value, StringToMDXDate(monthNode.Text), ",\n");
                        if (!selectedYears.Value.Contains(monthNode.Text.Split(' ')[1]))
                        {
                            selectedYears.Value = AddStringWithSeparator(selectedYears.Value, StringToMDXYear(monthNode.Text), ",\n");
                        }
                    }
                }
            }
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();

            if (Grid.Rows[Grid.Rows.Count - 3].Cells[0].Text.Contains("Естественный"))
            {
                Grid.Rows.Remove(Grid.Rows[Grid.Rows.Count - 1]);
            }
            Grid.Rows[Grid.Rows.Count - 1].Style.BorderDetails.WidthBottom = 1;
            
            if (comboRegion.SelectedIndex == 0)
            {
                Chart.Tooltips.FormatString = "Число родившихся, " + comboRegion.GetRootNodesName(0) + ", за <ITEM_LABEL> <SERIES_LABEL>г. <DATA_VALUE:##0> человек";
                //Label3.Text = "Число родившихся, " + comboRegion.GetRootNodesName(0) + ", человек";
            }
            else
            {
                Chart.Tooltips.FormatString = "Число родившихся, " + comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г.") + ", за <ITEM_LABEL> <SERIES_LABEL>г. <DATA_VALUE:##0> человек";
                
            }
            Label3.Text = "Число родившихся, " + comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г.") + ", человек"; ;
            SetSfereparam();
            Chart.DataBind();
            if (comboRegion.SelectedIndex!=0)
            {
                Label2.Text = String.Format(sub_page_title, comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г."));
            }
            else
            {
                Label2.Text = String.Format(sub_page_title, comboRegion.GetRootNodesName(0));
                
            }
          
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
            int year = Convert.ToInt32(dateElements[1]);
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

        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }
        Dictionary<string, int> DateLoad(string sql)
        {
            
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> dates = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                if ((cs.Axes[1].Positions[i].Members[0].LevelDepth == 1))
                {
                    if (cs.Axes[1].Positions[i+1].Members[0].LevelDepth!= 1)
                    {
                        
                            dates.Add(cs.Axes[1].Positions[i].Members[0].Caption + " год", 0);
                        
                    }
                }
                if ((cs.Axes[1].Positions[i].Members[0].LevelDepth == 4))
                {
                    if (IsSmallResolution)
                    {
                        dates.Add(cs.Axes[1].Positions[i].Members[0].Caption + " " + cs.Axes[1].Positions[i].Members[0].Parent.Parent.Parent.Caption, 1);
                    }
                    else
                    {
                        dates.Add(cs.Axes[1].Positions[i].Members[0].Caption + " " + cs.Axes[1].Positions[i].Members[0].Parent.Parent.Parent.Caption + " года", 1);
                    }

                }
            }

            return dates;
            
        }

        public string MDXDateToShortDateString(string mdxDateString)
        {
            if (mdxDateString=="")
            {
                return "";
            }
            string[] separator = { "].[" };
            string[] dateElements = mdxDateString.Split(separator, StringSplitOptions.None);
            string template = "{0} {1} года";
            string month = dateElements[6].Replace("]", String.Empty).ToLower();
            string year = dateElements[3];
            return String.Format(template,  month, year);
        }

        #region Обработчики грида
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            Grid.Columns.Clear();
            Grid.Bands[0].HeaderLayout.Clear();
            Grid.Bands.Clear();
            Population = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText("Grid2"));
            grid_desription = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid_Description"), "Даты", grid_desription);

            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid1"), "Показатель", dt);
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName + " " + grid_desription.Rows[0].ItemArray[i].ToString().Split('.')[3].Replace("[", String.Empty).Replace("]", String.Empty);
            }
            if ((comboCompareType.SelectedIndex == 2) && (selectedDate.Value.Contains(comboDate.GetRootNodesName(0).Split(' ')[0])))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0].ToString().EndsWith("прирост"))
                    {
                        for (int j = 1; j < dt.Rows[0].ItemArray.Length; j++)
                        {
                            if (dt.Columns[j].ColumnName.Contains(comboDate.GetRootNodesName(0).Split(' ')[0]))
                            {
                                row[j] = DBNull.Value;
                            }
                        }
                    }
                }
            }
            if ((comboCompareType.SelectedIndex == 0) && (selectedDate.Value.Contains(comboDate.GetRootNodesName(0).Split(' ')[0])))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0].ToString().EndsWith("прирост"))
                    {
                        if (dt.Columns[1].ColumnName.ToLower().StartsWith("январь"))
                        {
                            row[1] = DBNull.Value;
                        }
                    }
                }
            }
            Grid.DataSource = dt;
            string currentMonth = TransformMonth2(dt.Columns[dt.Columns.Count - 1].ColumnName.ToLower()) + " года";//CRHelper.RusMonthPrepositional(CRHelper.MonthNum(dt.Columns[dt.Columns.Count - 1].ColumnName.Split(' ')[0]))+" "+dt.Columns[dt.Columns.Count - 1].ColumnName.Split(' ')[1]+" года";

            string compareMonth = TransformMonth3(MDXDateToShortDateString(grid_desription.Rows[1].ItemArray[grid_desription.Rows[1].ItemArray.Length - 1].ToString()));

            if ((dt.Rows[1].ItemArray[dt.Rows[0].ItemArray.Length - 1] == DBNull.Value) || (grid_desription.Rows[1].ItemArray[grid_desription.Rows[1].ItemArray.Length - 1] == DBNull.Value))
            {
                Label4.Text = "&nbsp;&nbsp;&nbsp;Число рожденных в <b>" + currentMonth + "</b> составило <b>" + String.Format("{0:0}", GetNumber(dt.Rows[0].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString())) + "</b> человек.";
                Label4.Text += "<br>&nbsp;&nbsp;&nbsp;Число умерших в <b>" + currentMonth + "</b> составило <b>" + String.Format("{0:0}", GetNumber(dt.Rows[6].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString())) + "</b> человек.";
                Label4.Text += "<br>&nbsp;&nbsp;&nbsp;Естественный прирост населения в <b>" + currentMonth + "</b> составил <b>" + String.Format("{0:0}", Math.Abs((GetNumber(dt.Rows[12].ItemArray[dt.Rows[12].ItemArray.Length - 1].ToString())))) + "</b> человек.";
            }
            else
            {
                Label4.Text = "&nbsp;&nbsp;&nbsp;Число рожденных в <b>" + currentMonth + "</b>  по сравнению с <b>" + compareMonth + "</b>";// составило <b>" + String.Format("{0:0}", Convert.ToDouble(dt.Rows[0].ItemArray[dt.Rows[0].ItemArray.Length - 1])) + "</b> человек";
                if (GetNumber(dt.Rows[1].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString()) < 0)
                {
                    Label4.Text += " &nbsp;<img src='../../images/arrowRedDownBB.png'>уменьшилось на <b>" + String.Format("{0:0}", Math.Abs(GetNumber(dt.Rows[1].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString()))) + "</b> человек";
                }
                else
                {

                    if (GetNumber(dt.Rows[1].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString()) > 0)
                    {
                        Label4.Text += " &nbsp;<img src='../../images/arrowGreenUpBB.png'>увеличилось на <b>" + String.Format("{0:0}", (GetNumber(dt.Rows[1].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString()))) + "</b> человек";
                    }
                    else
                    {
                        Label4.Text += " не изменилось";
                    }
                }

                Label4.Text += " и составило <b>" + String.Format("{0:0}", GetNumber(dt.Rows[0].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString())) + "</b> человек.";


                Label4.Text += "<br>&nbsp;&nbsp;&nbsp;Число умерших в <b>" + currentMonth + "</b> по сравнению с <b>" + compareMonth + "</b>";

                if (GetNumber(dt.Rows[7].ItemArray[dt.Rows[7].ItemArray.Length - 1].ToString()) < 0)
                {
                    Label4.Text += " &nbsp;<img src='../../images/arrowGreenDownBB.png'> уменьшилось на <b>" + String.Format("{0:0}", Math.Abs(GetNumber(dt.Rows[7].ItemArray[dt.Rows[7].ItemArray.Length - 1].ToString()))) + "</b> человек";
                }
                else
                {
                    if (GetNumber(dt.Rows[7].ItemArray[dt.Rows[7].ItemArray.Length - 1].ToString()) > 0)
                    {
                        Label4.Text += " &nbsp;<img src='../../images/arrowRedUpBB.png'> увеличилось на <b>" + String.Format("{0:0}", (GetNumber(dt.Rows[7].ItemArray[dt.Rows[7].ItemArray.Length - 1].ToString()))) + "</b> человек";
                    }
                    else
                    {
                        Label4.Text += " не изменилось";
                    }
                }
                Label4.Text += " и составило <b>" + String.Format("{0:0}", GetNumber(dt.Rows[6].ItemArray[dt.Rows[0].ItemArray.Length - 1].ToString())) + "</b> человек.";


                Label4.Text += "<br>&nbsp;&nbsp;&nbsp;Естественный прирост населения в <b>" + currentMonth + "</b> по сравнению с <b>" + compareMonth + "</b>";


                if (GetNumber(dt.Rows[13].ItemArray[dt.Rows[13].ItemArray.Length - 1].ToString()) < 0)
                {
                    Label4.Text += "  &nbsp;<img src='../../images/arrowRedDownBB.png'>уменьшился на <b>" + String.Format("{0:0}", Math.Abs(GetNumber(dt.Rows[13].ItemArray[dt.Rows[13].ItemArray.Length - 1].ToString()))) + "</b> человек";// String.Format("{0:0}", (Convert.ToDouble(dt.Rows[1].ItemArray[dt.Rows[12].ItemArray.Length - 1]))) + 
                }
                else
                {
                    if (GetNumber(dt.Rows[13].ItemArray[dt.Rows[13].ItemArray.Length - 1].ToString()) > 0)
                    {
                        Label4.Text += " &nbsp;<img src='../../images/arrowGreenUpBB.png'>вырос на  <b>" + String.Format("{0:0}", (GetNumber(dt.Rows[13].ItemArray[dt.Rows[13].ItemArray.Length - 1].ToString()))) + "</b> человек";//String.Format("{0:0}", (Convert.ToDouble(dt.Rows[1].ItemArray[dt.Rows[12].ItemArray.Length - 1]))) +
                    }
                    else
                    {
                        Label4.Text += " не изменился";
                    }
                }
                Label4.Text += " и составил <b>" + String.Format("{0:0}", Math.Abs((GetNumber(dt.Rows[12].ItemArray[dt.Rows[12].ItemArray.Length - 1].ToString())))) + "</b> человек.";

            }
            if (Convert.ToDouble(dt.Rows[12].ItemArray[dt.Rows[12].ItemArray.Length - 1]) < 0)
            {
                Label4.Text = Label4.Text.Replace("прирост", "убыль").Replace("Естественный", "Естественная").Replace("уменьшился", "уменьшилась").Replace("вырос", "выросла").Replace("составил", "составила");
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double colWidth = 0;
            if (IsSmallResolution)
            {
                e.Layout.RowSelectorsDefault = RowSelectors.No;
                colWidth = minScreenWidth * 0.08;
            }
            else
            {
                e.Layout.RowSelectorsDefault = RowSelectors.Yes;
                colWidth = minScreenWidth * 0.067;
            }
            headerLayout.childCells.Clear();

            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.12);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key;
            }
            GridHeaderCell headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Key.Split(' ')[1] + " год");
            headerCell.AddCell(e.Layout.Bands[0].Columns[1].Key.Split(' ')[0].ToLower());

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (e.Layout.Bands[0].Columns[i].Key[e.Layout.Bands[0].Columns[i].Key.Length - 1] != e.Layout.Bands[0].Columns[i - 1].Key[e.Layout.Bands[0].Columns[i - 1].Key.Length - 1])
                {
                    string year = e.Layout.Bands[0].Columns[i].Key.Remove(0, e.Layout.Bands[0].Columns[i].Key.LastIndexOf(' '));
                    headerCell = headerLayout.AddCell(year + " год");
                }

                headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(' ')[0].ToLower());
            }
            headerLayout.ApplyHeaderInfo();
        }
        string compareText = "";
        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {

            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position: 15px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 30px"; }

            if ((e.Row.Index + 1) % 3 == 0)
            {

                Grid.Rows[e.Row.Index - 2].Cells[0].Text = Grid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0] + ", чел.";
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = "";
                e.Row.Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 2].Cells[0].RowSpan = 3;

                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.WidthBottom = 0;
                Grid.Rows[e.Row.Index].Cells[0].Style.BorderDetails.WidthBottom = 0;
                e.Row.Cells[0].Style.BackColor = Color.White;
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.WidthBottom = 0;
                    if (!Grid.Rows[e.Row.Index - 2].Cells[0].Text.Contains("Естественный"))
                    {
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.WidthBottom = 0;
                    }
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                    e.Row.Cells[i].Style.BackColor = Color.White;
                    if ((grid_desription.Rows[1][i].ToString() == "") && (grid_desription.Rows.Count >= 3))
                    {
                        compareText = MDXDateToShortDateString(grid_desription.Rows[1][i + 1].ToString());
                    }
                    else
                    {
                        compareText = MDXDateToShortDateString(grid_desription.Rows[1][i].ToString());
                    }

                    if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                    {
                        Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Снижение относительно " + TransformMonth(compareText);
                        if ((Grid.Rows[e.Row.Index - 2].Cells[0].Text.Contains("Число мертворожденных")) || (Grid.Rows[e.Row.Index - 2].Cells[0].Text.Contains("Число умерших")))
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        }
                        else
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        }
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;
                    }
                    if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                    {
                        Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Рост относительно " + TransformMonth(compareText);
                        if ((Grid.Rows[e.Row.Index - 2].Cells[0].Text.Contains("Число мертворожденных")) || (Grid.Rows[e.Row.Index - 2].Cells[0].Text.Contains("Число умерших")))
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        }
                        else
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        }
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;
                    }
                    Grid.Rows[e.Row.Index - 1].Cells[i].Value = String.Format("{0:N0}", Grid.Rows[e.Row.Index - 1].Cells[i].Value);
                    Grid.Rows[e.Row.Index - 2].Cells[i].Value = String.Format("{0:N0}", Grid.Rows[e.Row.Index - 2].Cells[i].Value);
                    e.Row.Cells[i].Value = String.Format("{0:0.##%}", e.Row.Cells[i].Value);
                    if (e.Row.Cells[i].Text != "")
                    {
                        e.Row.Cells[i].Title = "Темп прироста к " + TransformMonth1(compareText);
                    }
                }
            }
        }

        protected void calculateRank(UltraWebGrid Grid, int colNumber)
        {

            int m = 0;
            for (int i = 0; i < Grid.Rows.Count; i += 3)//подсчет ненулевых значений столбца
            {
                if (GetNumber(Grid.Rows[i].Cells[colNumber].Value.ToString()) != 0)
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
                for (int i = 0; i < Grid.Rows.Count; i += 3)
                {
                    if (GetNumber(Grid.Rows[i].Cells[colNumber].Value.ToString()) != 0)
                    {
                        rank[m] = GetNumber(Grid.Rows[i].Cells[colNumber].Value.ToString());
                        m += 1;
                    }
                    Grid.Rows[i].Cells[colNumber].Title = "";
                }
                int mark = 0;
                Array.Sort(rank);
                m = 1;
                if (colNumber != 5)
                {
                    m = 1;
                }
                else
                {
                    m = rank.Length;
                }
                for (int i = rank.Length - 1; i >= 0; i--)
                {
                    for (int j = 0; j < Grid.Rows.Count; j += 3)
                    {
                        if (rank[i] == Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Text))
                        {
                            if (Grid.Rows[j].Cells[colNumber].Title == "")
                            {
                                Grid.Rows[j].Cells[colNumber].Title = "Ранг по ХМАО-ЮГРЕ на " + Grid.Rows[Grid.Rows.Count - 3].Cells[0].Text + ": " + m.ToString();
                                if ((m) == 1)
                                {
                                    Grid.Rows[j].Cells[colNumber].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    Grid.Rows[j].Cells[colNumber].Style.CustomRules = style;
                                }
                                if (m == rank.Length)
                                {
                                    Grid.Rows[j].Cells[colNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    Grid.Rows[j].Cells[colNumber].Style.CustomRules = style;
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
                for (int j = 0; j < Grid.Rows.Count; j += 3)
                {
                    if (colNumber != 5)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) > max)
                        {
                            max = Convert.ToInt32(Grid.Rows[j].Cells[colNumber].Value);
                        }
                    }
                    else
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) < max)
                        {
                            max = Convert.ToInt32(Grid.Rows[j].Cells[colNumber].Value);
                        }
                    }
                }
                for (int j = 0; j < Grid.Rows.Count; j += 3)
                {
                    if (colNumber != 5)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) == max)
                        {
                            Grid.Rows[j].Cells[colNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                            Grid.Rows[j].Cells[colNumber].Style.CustomRules = style;
                        }
                    }
                    else
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) == max)
                        {
                            Grid.Rows[j].Cells[colNumber].Style.BackgroundImage = "~/images/starYellowBB.png";
                            Grid.Rows[j].Cells[colNumber].Style.CustomRules = style;
                        }
                    }
                }
            }
        }
        #endregion


        protected string TransformMonth(string date)
        {
            if (date == "")
            {
                return "";
            }
            else
            {
                string month = date.Split(' ')[0];
                date = date.Remove(0, month.Length);
                if ((month.Contains("март")) || (month.Contains("август")))
                {
                    return month + "а" + date;
                }
                else
                {
                    return month.Remove(month.Length - 1) + "я" + date;
                }
            }
        }

        protected string TransformMonth1(string date)
        {
            if (date == "")
            {
                return "";
            }
            else
            {
                string month = date.Split(' ')[0];
                date = date.Remove(0, month.Length);
                if ((month.Contains("март")) || (month.Contains("август")))
                {
                    return month + "у" + date;
                }
                else
                {
                    return month.Remove(month.Length - 1) + "ю" + date;
                }
            }
        }

        protected string TransformMonth2(string date)
        {
            if (date == "")
            {
                return "";
            }
            else
            {
                string month = date.Split(' ')[0];
                date = date.Remove(0, month.Length);
                if ((month.Contains("март")) || (month.Contains("август")))
                {
                    return month + "е" + date;
                }
                else
                {
                    return month.Remove(month.Length - 1) + "е" + date;
                }
            }
        }

        protected string TransformMonth3(string date)
        {
            if (date == "")
            {
                return "";
            }
            else
            {
                string month = date.Split(' ')[0];
                date = date.Remove(0, month.Length);
                if ((month.Contains("март")) || (month.Contains("август")))
                {
                    return month + "ом" + date;
                }
                else
                {
                    return month.Remove(month.Length - 1) + "ем" + date;
                }
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

        protected string StringToDate(string date)
        {
            int month = CRHelper.MonthNum(date.Split(' ')[0]);
            if (month < 10)
            {
                return ("0" + month.ToString() + "." + date.Split(' ')[1]);
            }
            else
            {
                return (month.ToString() + "." + date.Split(' ')[1]);
            }
        }


        #region Обработчики диаграммы
        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText("Chart1"));
            DataTable TransformedTable = new DataTable();
            TransformedTable.Columns.Add("Year");
            TransformedTable.Columns.Add("январь", typeof(decimal));
            TransformedTable.Columns.Add("февраль", typeof(decimal));
            TransformedTable.Columns.Add("март", typeof(decimal));
            TransformedTable.Columns.Add("апрель", typeof(decimal));
            TransformedTable.Columns.Add("май", typeof(decimal));
            TransformedTable.Columns.Add("июнь", typeof(decimal));
            TransformedTable.Columns.Add("июль", typeof(decimal));
            TransformedTable.Columns.Add("август", typeof(decimal));
            TransformedTable.Columns.Add("сентябрь", typeof(decimal));
            TransformedTable.Columns.Add("октябрь", typeof(decimal));
            TransformedTable.Columns.Add("ноябрь", typeof(decimal));
            TransformedTable.Columns.Add("декабрь", typeof(decimal));
            bool emptyFlag = true;
            double max = Convert.ToDouble(cs.Cells[0, 0].Value), min = Convert.ToDouble(cs.Cells[0, 0].Value);
            int k = 1;
            object[] o = new object[TransformedTable.Columns.Count];
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
                if ((i != 0) && (cs.Axes[1].Positions[i].Members[0].Parent.Parent.Parent.Caption != cs.Axes[1].Positions[i - 1].Members[0].Parent.Parent.Parent.Caption))
                {
                    TransformedTable.Rows.Add(o);
                    o = new object[TransformedTable.Columns.Count];
                    o[0] = cs.Axes[1].Positions[i].Members[0].Parent.Parent.Parent.Caption.ToLower();
                    k = 1;

                }
                else
                {
                    if (i == 0)
                    {
                        o[0] = cs.Axes[1].Positions[i].Members[0].Parent.Parent.Parent.Caption.ToLower();
                    }
                }
                if (cs.Cells[0, i].Value != null)
                {
                    emptyFlag = false;
                }
                o[k] = cs.Cells[0, i].Value;
                if (Convert.ToDouble(cs.Cells[0, i].Value) > max)
                {
                    max = Convert.ToDouble(cs.Cells[0, i].Value);
                }
                if (Convert.ToDouble(cs.Cells[0, i].Value) < min)
                {
                    min = Convert.ToDouble(cs.Cells[0, i].Value);
                }
                k += 1;

            }
            TransformedTable.Rows.Add(o);
            if (emptyFlag)
            {
                Chart.DataSource = null;
            }
            else
            {
                Chart.DataSource = TransformedTable;
            }
            Chart.Axis.Y.RangeType = AxisRangeType.Custom;
            Chart.Axis.Y.RangeMax = max + 1;
            Chart.Axis.Y.RangeMin = min - 1;
        }

        protected void Chart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }
        #endregion

        #region Добавление checkbox
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            string[] buttonNames = new string[5];
            buttonNames[0] = "Число родившихся";
            buttonNames[1] = "Число мертворожденных";
            buttonNames[2] = "Число умерших";
            buttonNames[3] = "Число умерших детей в возрасте до 1 года";
            buttonNames[4] = "Естественный прирост (убыль) населения";
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
          //  {
                region = "ХМАО-Югра";
          //  }
           // else
           // {
                region = comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г.");
           // }
            Label3.Text = rb.Text + ", " + region + ", человек";
            if (rb.Text == "Число родившихся")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число родившихся]";
            }
            if (rb.Text == "Число умерших")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число умерших]";
            }
            if (rb.Text == "Число мертворожденных")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число мертворожденных]";
            }
            if (rb.Text == "Число умерших детей в возрасте до 1 года")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Число умерших детей в возрасте до 1 года]";
            }
            if (rb.Text == "Естественный прирост (убыль) населения")
            {
                chartPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Естественный прирост населения]";
            }

            Chart.DataBind();
            if (comboRegion.SelectedIndex == 0)
            {
                Chart.Tooltips.FormatString = rb.Text + ", " + comboRegion.SelectedValue + ",  за <ITEM_LABEL> <SERIES_LABEL>г. <DATA_VALUE:##0> человек";
            }
            else
            {
                Chart.Tooltips.FormatString = rb.Text + ", " + comboRegion.SelectedValue.Replace("муниципальный район", "м-р").Replace("Город", "г.") + ", за <ITEM_LABEL> <SERIES_LABEL>г. <DATA_VALUE:##0> человек";
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
            Label4.Text = Label4.Text.Replace("\n", "<br>");
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            int width = int.Parse(Chart.Width.Value.ToString());
            Chart.Width = 1000;
            ReportPDFExporter1.HeaderCellHeight = 30;
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


    }
}
