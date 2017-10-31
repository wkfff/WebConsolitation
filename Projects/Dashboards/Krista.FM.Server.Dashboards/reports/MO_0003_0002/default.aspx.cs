using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using System.Configuration;
using System.Collections;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebNavigator;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Components;
using Dundas.Maps.WebControl;
using System.Globalization;
using Infragistics.Documents.Reports.Report.Text;
using System.Text.RegularExpressions;
using Infragistics.WebUI.Misc;
using Infragistics.UltraChart.Core.Primitives;
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
namespace Krista.FM.Server.Dashboards.reports.MO_0003_0002
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Сравнение муниципальных образований по показателям социально-экономического развития";
        private string page_sub_title = "Анализ социально-экономического положения территории по показателю  «{0}» в разрезе МО субъекта РФ  за {1}.";
        private string chart_title = "Распределение МО по показателю «{0}», {1}";
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
        private CustomParam Period { get { return (UserParams.CustomParam("Period")); } }
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private string style = "";
        private DataTable dtGrid;
        private DataTable dtChart;
        private GridHeaderLayout headerLayout;
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            PageTitle.Text = page_title;
            Page.Title = page_title;
            UltraWebGrid.Width = (int)((minScreenWidth) - 15);
            UltraWebGrid.Height = Unit.Empty;
            Chart.Width = (int)((minScreenWidth) - 15);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            HyperLink1.NavigateUrl = "~/reports/MO_0003_0001/default.aspx";
            HyperLink1.Text = "Паспорт&nbsp;муниципального&nbsp;образования";

            
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Выберите период";
                ComboYear.Width = 250;
                // ComboYear.SelectLastNode();
                ComboYear.ParentSelect = true;
                FillComboYear();
                ComboIndicators.Title = "Показатель";
                ComboIndicators.Width = 600;
                ComboIndicators.ShowSelectedValue = true;
                ComboIndicators.ParentSelect = false;
                FillComboIndicators();
                gridTable.Visible = false;
                gridTable0.Visible = false;
                PageSubTitle.Text = "";
            }
            else
            {
                gridTable.Visible = true;
                gridTable0.Visible = true;
                PageSubTitle.Text = String.Format(page_sub_title,ComboIndicators.SelectedValue,ComboYear.SelectedNode.Text);
            }
           
                
                if (ComboIndicators.SelectedNode.Level == 1)
                {
                    selectedPok.Value = "[Паспорт МО__МР и Административный центр].[Паспорт МО__МР и Административный центр].[Все показатели].[" + ComboIndicators.SelectedNode.Parent.Text + "].[" + ComboIndicators.SelectedValue + "]";
                }
                else
                {
                    if (ComboIndicators.SelectedNode.Level == 2)
                    {
                        selectedPok.Value = "[Паспорт МО__МР и Административный центр].[Паспорт МО__МР и Административный центр].[Все показатели].[" + ComboIndicators.SelectedNode.Parent.Parent.Text + "].[" + ComboIndicators.SelectedNode.Parent.Text + "].[" + ComboIndicators.SelectedNode.Text + "]";
                    }
                    else
                    {
                        selectedPok.Value = "[Паспорт МО__МР и Административный центр].[Паспорт МО__МР и Административный центр].[Все показатели].[" + ComboIndicators.SelectedNode.Parent.Parent.Parent.Text + "].[" + ComboIndicators.SelectedNode.Parent.Parent.Text + "].["  + ComboIndicators.SelectedNode.Parent.Text + "].[" + ComboIndicators.SelectedNode.Text + "]";
                    }
                }
                if (ComboYear.SelectedNode.Level == 0)
                {
                    selectedPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboYear.SelectedNode.Text.Split(' ')[0] + "]";
                    Period.Value = "Год";
                }
                else
                {
                    Period.Value = "Квартал";
                    if (ComboYear.SelectedValue.StartsWith("Квартал 1") || ComboYear.SelectedValue.StartsWith("Квартал 2"))
                    {
                        selectedPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboYear.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие 1].[" + ComboYear.SelectedValue.Split(' ')[0] + " " + ComboYear.SelectedValue.Split(' ')[1] + "]";
                    }
                    else
                    {
                        selectedPeriod.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboYear.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие 2].[" + ComboYear.SelectedValue.Split(' ')[0] + " " + ComboYear.SelectedValue.Split(' ')[1] + "]";
                    }
                }
                if (IsSmallResolution)
                { style = "background-repeat: no-repeat;background-position: 10px"; }
                else
                { style = "background-repeat: no-repeat;background-position: 40px"; }
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Columns.Clear();
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                if (UltraWebGrid.DataSource != null )
                {
                    
                    calculateRank(UltraWebGrid, 1);
                    Chart.DataBind();
                    Label1.Text = String.Format(chart_title, ComboIndicators.SelectedValue, dtGrid.Rows[0].ItemArray[dtGrid.Rows[0].ItemArray.Length - 1].ToString().ToLower());
                    gridTable.Visible = true;
                    gridTable0.Visible = true;
                    EmptyReport.Visible = false;
                    EmptyReport.Text = "Нет данных<br>";
                    
                }
                if (
                        ComboYear.SelectedNode.Level == 0 &&
                    (ComboIndicators.SelectedValue == "Фонд заработной платы всех работников организаций" ||
                    ComboIndicators.SelectedValue == "Среднемесячная заработная плата работников организаций" ||
                    ComboIndicators.SelectedValue == "Среднемесячная заработная плата работников местных администраций (исполнительно-распорядительных органов муниципальных образований)" ||
                    ComboIndicators.SelectedValue == "Официально зарегистрированные безработные, получающие пособие по безработице" ||
                    ComboIndicators.SelectedValue == "Поголовье крупного рогатого скота в сельскохозяйственных организациях (без субъектов малого предпринимательства с численностью до 60 человек)" ||
                    ComboIndicators.SelectedValue == "Поголовье коров в сельскохозяйственных организациях" ||
                    ComboIndicators.SelectedValue == "Поголовье свиней в сельскохозяйственных организациях" ||
                    ComboIndicators.SelectedValue == "Поголовье птиц в сельскохозяйственных организациях") || UltraWebGrid.DataSource == null 
                        )
                {
                    if (Page.IsPostBack)
                    {
                        EmptyReport.Visible = true;
                        EmptyReport.Text = "Нет данных<br>";
                        gridTable.Visible = false;
                        gridTable0.Visible = false;
                    }
                }
        
        }

        private void FillComboIndicators()
        {

            Dictionary<string, int> indicatorLevelDictionary = new Dictionary<string, int>();
            DataTable dtIndicators = new DataTable();
            string query = DataProvider.GetQueryText("MO_0003_0001_indicators");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtIndicators);
            for (int i = 0; i < dtIndicators.Rows.Count; i++)
            {
                int level = 0;

                switch (dtIndicators.Rows[i]["Уровень"].ToString())
                {
                    case "Уровень 1":
                        {
                            level = 0;
                            break;
                        }
                    case "Уровень 2": 
                        {
                            level = 1;
                            break;
                        }
                    case "Уровень 3":
                        {
                            level = 2;
                            break;
                        }
                    case "Уровень 4":
                        {
                            level = 3;
                            break;
                        }
                }
               
                    indicatorLevelDictionary.Add(dtIndicators.Rows[i]["Показатель"].ToString(), level);
                
            }
            if (!Page.IsPostBack)
            {
                ComboIndicators.FillDictionaryValues(indicatorLevelDictionary);
            }
        }

        private void FillComboYear()
        {
            
         //   string query = DataProvider.GetQueryText("MO_0003_0002_date");
            Microsoft.AnalysisServices.AdomdClient.CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText("MO_0003_0002_date"));
            Dictionary<string, int> yearLevelDictionary = new Dictionary<string, int>();

            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                if (cs.Axes[1].Positions[i].Members[0].LevelDepth == 1)
                {
                    yearLevelDictionary.Add(cs.Axes[1].Positions[i].Members[0].Caption+" год", 0);
                }
                if (cs.Axes[1].Positions[i].Members[0].LevelDepth == 3)
                {
                    yearLevelDictionary.Add(cs.Axes[1].Positions[i].Members[0].Caption+" "+cs.Axes[1].Positions[i].Members[0].Parent.Parent.Caption + " года", 1);
                }
            }

                
            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(yearLevelDictionary);
            }
           
        }




        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("MO_0003_0002_grid"), "Территория", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }
        protected void calculateRank(UltraWebGrid Grid, int colNumber)
        {
         /*    string style = "";
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position: 15px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 50px"; }*/
            int m = 0;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                {
                    m += 1;
                }
            }

            if (m!=0)
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 0; i < Grid.Rows.Count; i ++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                    {
                        rank[m] = Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value);
                        m += 1;
                        Grid.Rows[i].Cells[Grid.Columns.IndexOf("Ранг")].Text = "0";
                    }
                    else
                    {
                        Grid.Rows[i].Cells[Grid.Columns.IndexOf("Ранг")].Text = String.Empty;
                    }
                    
                } 
                Array.Sort(rank);

                if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                {
                    m = 1;
                }
                else
                {
                    m = rank.Length;
                }
                  
               
                for (int i = rank.Length - 1; i >= 0; i--)
                {
                    
                    for (int j =0; j < Grid.Rows.Count; j ++)
                    {
                        if (rank[i] == GetNumber(Grid.Rows[j].Cells[colNumber].Text))
                        {
                            if (Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Text == "0")
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Text = String.Format("{0:0}", m);
                                if ((m) == 1)
                                {
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.CustomRules = style;
                                }
                                if (m == rank.Length)
                                {

                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.CustomRules = style;
                                }
                            }
                        }
                    }
                    if (i != 0)
                    {
                        if (rank[i] != rank[i - 1])
                        {
                            if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                            {
                                m += 1;
                            }
                            else
                            { m -= 1; }
                        }
                    }
                    else
                    {
                        if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                        { 
                            m += 1;
                        }
                        else
                        { m -= 1; }
                    }
                    
                }
                double max = GetNumber(Grid.Rows[0].Cells[colNumber].Text);
                for (int j = 0; j < Grid.Rows.Count; j ++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[j].Cells[colNumber].Value) == true && Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) < max)
                        {
                            max = GetNumber(Grid.Rows[j].Cells[colNumber].Text);
                        }
                    }
                }
                for (int j = 0; j < Grid.Rows.Count; j ++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[j].Cells[colNumber].Value) == true && Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colNumber].Text) == max)
                        {
                            if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starGrayBB.png";
                            }
                            else
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starYellowBB.png";
                            }
                            Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.CustomRules = style;
                        }
                    }
                }
            }
          /*  int m = 0;
            for (int i = 0; i < Grid.Rows.Count; i ++)//подсчет ненулевых значений столбца
            {
                if (Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value) != 0)
                {
                    m += 1;
                }
            }
            if (m == 0)
            {
            }
            else
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 0; i < Grid.Rows.Count; i ++)
                {
                    if (Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value) != 0)
                    {
                        rank[m] = Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value);
                        m += 1;
                    }
                }
                Array.Sort(rank);
                if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                {
                    m = 1;
                }
                else
                {
                    m = rank.Length;
                }
                for (int i = rank.Length - 1; i >= 0; i--)
                {
                    for (int j = 0; j < Grid.Rows.Count; j ++)
                    {
                        if (rank[i] == Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value))
                        {
                            if (Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Text == "0")
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Text = m.ToString();
                                if (m == 1)
                                {
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.CustomRules = style;
                                }
                                if (m == rank.Length)
                                {
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.CustomRules = style;
                                }
                            }
                        }
                    }
                    if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                    {
                        m += 1;
                    }
                    else
                    { m -= 1; }
                } 
                double max = Convert.ToDouble(Grid.Rows[0].Cells[colNumber].Value);
                double min = Convert.ToDouble(Grid.Rows[0].Cells[colNumber].Value);
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                   
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value) > max)
                        {
                            max = Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value);
                        }
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value) < min )
                        {
                           min= Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Value);
                        }
                    
                }
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                    
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Text) == max)
                        {
                            if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starYellowBB.png";
                            }
                            else
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starGrayBB.png";
                            }
                           
                            Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.CustomRules = style;
                        }
                        if (Convert.ToDouble(Grid.Rows[j].Cells[colNumber].Text) == min)
                        {
                            if (Grid.Rows[0].Cells[Grid.Columns.Count - 2].Text != "1")
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starGrayBB.png";
                            }
                            else
                            {
                                Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.BackgroundImage = "~/images/starYellowBB.png";
                            }
                           
                            Grid.Rows[j].Cells[Grid.Columns.IndexOf("Ранг")].Style.CustomRules = style;
                        }
                    }
                
            }*/
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

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (UltraWebGrid.DataSource!=null)
            {
                e.Layout.GroupByBox.Hidden = true;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
                e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
                e.Layout.AllowSortingDefault = AllowSorting.No;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
                {
                    int widthColumn = CRHelper.GetColumnWidth(150);
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.12);
                }
                headerLayout.childCells.Clear();
                int widthColumn1 = CRHelper.GetColumnWidth(minScreenWidth * 0.4);
                e.Layout.Bands[0].Columns[0].Width = widthColumn1;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                headerLayout.AddCell("Территория");

                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 1].Hidden = true;
                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 2].Hidden = true;

                headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Header.Caption+", "+dtGrid.Rows[0].ItemArray[dtGrid.Rows[0].ItemArray.Length-1].ToString().ToLower());
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Grid.Columns[1], "N2");
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                for (int i = 2; i < e.Layout.Bands[0].Grid.Columns.Count - 2; i++)
                {
                    headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Grid.Columns[i], "N2");
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                }
                headerLayout.ApplyHeaderInfo();
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.BackColor = Color.White;
                }
                if ((Convert.ToDouble(e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Value) < 1) && (Convert.ToDouble(e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Value) > 0))
                {
                    if (e.Row.Cells[UltraWebGrid.Columns.IndexOf("Примечание")].Text == "1")
                    {
                        e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                    }
                    else
                    {
                        e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    }
                    e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Style.CustomRules = style;
                }
                if (Convert.ToDouble(e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Value) > 1)
                {
                    if (e.Row.Cells[UltraWebGrid.Columns.IndexOf("Примечание")].Text == "1")
                    {
                        e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                    }
                    else
                    {
                        e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    }
                    e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Style.CustomRules = style;
                }
                if (e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Value != null)
                {
                    e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Text = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[UltraWebGrid.Columns.IndexOf("Темп роста цепной")].Value));
                }
            }
            catch { }
        }

        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("MO_0003_0002_chart"), "МО", dtChart);
            if (dtChart.Rows.Count >1)
            {
                dt.Columns.Add("МО", typeof(string));
                dt.Columns.Add("Значение", typeof(double));
                object[] o = new object[dt.Columns.Count];
                for (int i = 1; i < dtChart.Rows.Count; i++)
                {
                    o[0] = dtChart.Rows[i][0].ToString().Replace("муниципальный район","МО");
                    o[1] = dtChart.Rows[i][1];
                    dt.Rows.Add(o);
                }
                foreach (DataRow row in dt.Rows)
                {
                    row[0] = row[0].ToString().Replace("муниципальный район", "МО");
                }
                Chart.DataSource = dt;
                Chart.Tooltips.FormatString = "<SERIES_LABEL><br><b><DATA_VALUE:0.##></b>, " + dtGrid.Rows[0].ItemArray[dtGrid.Rows[0].ItemArray.Length - 1].ToString().ToLower();
            }
            else
            {
                Chart.DataSource = null;
            }
        }

        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (Chart.DataSource!=null)
            {
                if (dtChart.Rows[0].ItemArray[2].ToString() == "-1")
                {

                    IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                    IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                    if (xAxis == null || yAxis == null)
                        return;

                    int xMin = (int)xAxis.MapMinimum;
                    int xMax = (int)xAxis.MapMaximum;
                    double urfoAverage;
                    if (double.TryParse(dtChart.Rows[0].ItemArray[1].ToString(), out urfoAverage))
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
                        text.bounds = new Rectangle(xMin - 46, fmY, 780, 15);
                        text.SetTextString("Ханты-Мансийский автономный округ - "+String.Format("{0:0.##}",urfoAverage));//String.Format(dtChart12.Rows[0].ItemArray[0].ToString(),""));
                        e.SceneGraph.Add(text);
                    }
                }
            }
        }

        protected void Chart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
                ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text.Insert(PageSubTitle.Text.IndexOf(' ', 60), "<br>");
                
                Workbook workbook = new Workbook();
                Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
                Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
                ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
                ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
                ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
                ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
                ReportExcelExporter1.TitleStartRow = 3;
                Chart.Width = 900;
                ReportExcelExporter1.Export(headerLayout, sheet1, 6);
                Label1.Text = Label1.Text.Insert(Label1.Text.IndexOf(' ', 20), "<br>");
                ReportExcelExporter1.Export(Chart, Label1.Text, sheet2, 3);
            }
            catch { }

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
           // e.Workbook.Worksheets["Диаграмма"].MergedCellsRegions.Clear();
          //  e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 1000;
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Justify;
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Font.Name = "Verdana";
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Font.Height = 200;
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Height = 1500;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 80;
            ReportPDFExporter1.Export(headerLayout, section1);
            Chart.Width = 900;
            ReportPDFExporter1.Export(Chart, Label1.Text, section2);
        }
        void ultraWebGridDocumentExporter_RowExporting(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportingEventArgs e)
        {
            e.GridRow.Height = 500;
        }
        #endregion
    }
}