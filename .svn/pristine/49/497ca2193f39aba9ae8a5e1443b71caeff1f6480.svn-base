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
using Infragistics.UltraChart.Core;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common; 

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic; 

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.SEP_0001_ComplexSahalin
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Показатели для расчета комплексной оценки социально-экономического развития муниципальных образований";
        private string page_sub_title = "{0}, {1} для расчета комплексной оценки социально-экономического развития муниципальных образований, {2}";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion", true)); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod",true)); } }
        private CustomParam prevPeriod { get { return (UserParams.CustomParam("prevPeriod",true)); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok",true)); } }
        private CustomParam weightKoef { get { return (UserParams.CustomParam("weightKoef", true)); } }
        private GridHeaderLayout headerLayout;
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


        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;
        private bool onWall;
        private bool blackStyle;

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
          /*  blackStyle = false;
            if (Session["blackStyle"] != null)
            {
                blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            }

            CRHelper.SetPageTheme(this, blackStyle ? "MinfinBlackStyle" : "Minfin");*/
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Year.Width = 250;
            ComboPok.Width = 400;


            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            pageWidth = onWall ? int.Parse(RegionSettingsHelper.Instance.GetPropertyValue("WallResolutionWidth")) : (int)Session["width_size"];
            pageHeight = onWall ? int.Parse(RegionSettingsHelper.Instance.GetPropertyValue("WallResolutionHeight")) : (int)Session["height_size"];
            G.Width = CRHelper.GetGridWidth(pageWidth-50);
            G.Height = Unit.Empty;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #region Bigresolution
            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            primitiveSizeMultiplier = onWall ? 4 : 1;

            pageWidth = onWall ? pageWidth : (int)Session["width_size"];

            pageHeight = onWall ? pageHeight : (int)Session["height_size"];

            widthMultiplier = 1;
            if (Session["width_size"] != null && (int)Session["width_size"] != 0)
            {
                widthMultiplier = onWall ? 1.08 * pageWidth / (int)Session["width_size"] : 1;
            }
             
            Color fontColor = Color.Black; 
            //blackStyle ? Color.White : Color.Black;

            G.DisplayLayout.HeaderStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            G.DisplayLayout.RowStyleDefault.Font.Name = "Microsoft Sans Serif";
            G.DisplayLayout.HeaderStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            G.DisplayLayout.HeaderStyleDefault.BorderWidth = blackStyle ? 1 : onWall ? 3 : 1;

            G.DisplayLayout.RowStyleDefault.Font.Size = Convert.ToInt32((8)* fontSizeMultiplier);
            G.DisplayLayout.RowStyleDefault.Font.Name = "Microsoft Sans Serif";
            G.DisplayLayout.RowStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            G.DisplayLayout.RowStyleDefault.BorderWidth = 1;

            PageTitle.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            PageSubTitle.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);
            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);
            string bestStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string worseStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            if (onWall)
            {
                ComprehensiveDiv.Style.Add("width", pageWidth.ToString() + "px");
                ComprehensiveDiv.Style.Add("height", pageHeight.ToString() + "px");
                PageTitle.Font.Size = Convert.ToInt32(12.5 * fontSizeMultiplier);
                PageSubTitle.Font.Size = Convert.ToInt32(11.5 * fontSizeMultiplier);

            }
            else
            {
                PageTitle.Font.Size = Convert.ToInt32(12.5);
                PageSubTitle.Font.Size = Convert.ToInt32(11.3);
            }

            if (onWall && Page.Master is IMasterPage)
            {
                ((IMasterPage)Page.Master).SetHeaderVisible(false);
            }

            PopupInformer1.Visible = !onWall;
            Year.Visible = !onWall;
            ComboPok.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
            WallLink.Visible = !onWall;
            ReportExcelExporter1.Visible = !onWall;
            ReportPDFExporter1.Visible = !onWall;

            #endregion


            
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                Year.FillDictionaryValues(YearsLoad("years"));
                Year.Title = "Период";
                Year.ParentSelect = true;
                Year.SelectLastNode();
                ComboPok.Title = "Показатель";
                ComboPok.FillDictionaryValues(PokLoad("pok")); 
            }
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
           
            if (Year.SelectedNode.Level == 0)
            {
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "].[Полугодие 2].[Квартал 4]";
            }
            else
            {
                int half_num = CRHelper.HalfYearNumByQuarterNum(int.Parse(Year.SelectedValue.Split(' ')[1]));
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие " + half_num.ToString() + "].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
            } 
            if (!Page.IsPostBack) 
            { 
                if (selectedPok.Value == "")
                {
                    if (ComboPok.SelectedNode.Text == "Экономическая сфера, Базовые показатели")
                    {
                        selectedPok.Value = "[Показатели__Комплексная оценка].[Показатели__Комплексная оценка].[Все].[Комплексная оценка СЭР_Сахалин].[" + ComboPok.SelectedNode.Parent.Text + "].[" + ComboPok.SelectedNode.Text.Split(',')[0] + "].children,    [Показатели__Комплексная оценка].[Показатели__Комплексная оценка].[Все].[Комплексная оценка СЭР_Сахалин].[Базовые показатели].[Финансовая сфера].[Общий объем расходов местного бюджета, в части бюджетных инвестиций на увеличение стоимости основных средств]";
                    }
                    else
                    {
                        selectedPok.Value = "[Показатели__Комплексная оценка].[Показатели__Комплексная оценка].[Все].[Комплексная оценка СЭР_Сахалин].[" + ComboPok.SelectedNode.Parent.Text + "].[" + ComboPok.SelectedNode.Text.Split(',')[0] + "].children";
                    }
                }
                else
                {
                    ComboPok.SelectLastNode();
                }
            }
            else
            {
                if (ComboPok.SelectedNode.Text == "Экономическая сфера, Базовые показатели")
                {
                    selectedPok.Value = "[Показатели__Комплексная оценка].[Показатели__Комплексная оценка].[Все].[Комплексная оценка СЭР_Сахалин].[" + ComboPok.SelectedNode.Parent.Text + "].[" + ComboPok.SelectedNode.Text.Split(',')[0] + "].children,    [Показатели__Комплексная оценка].[Показатели__Комплексная оценка].[Все].[Комплексная оценка СЭР_Сахалин].[Базовые показатели].[Финансовая сфера].[Общий объем расходов местного бюджета, в части бюджетных инвестиций на увеличение стоимости основных средств]";
                }
                else
                {
                    selectedPok.Value = "[Показатели__Комплексная оценка].[Показатели__Комплексная оценка].[Все].[Комплексная оценка СЭР_Сахалин].[" + ComboPok.SelectedNode.Parent.Text + "].[" + ComboPok.SelectedNode.Text.Split(',')[0] + "].children";
                }
            }
            if (Year.SelectedNode.Level == 0)
            {
                prevPeriod.Value = (int.Parse(Year.SelectedNode.Text.Split(' ')[0])).ToString();
                weightKoef.Value = "Годовой";
            }
            else  
            {
                prevPeriod.Value = (int.Parse(Year.SelectedNode.Parent.Text.Split(' ')[0]) - 1).ToString();
                weightKoef.Value = "Квартальный";
            }
            headerLayout = new GridHeaderLayout(G);
            G.DataBind();
            if (selectedPok.Value.Split(']')[4].TrimStart('.').TrimStart('[') != "Базовые показатели" && G.DataSource != null)
            {
                G.Rows.Remove(G.Rows[0]);
            }
            if (G.DataSource == null)
            {
                EmptyReport.Visible = true;
                TableGrid.Visible = false;
            }
            else
            {
                EmptyReport.Visible = false;
                TableGrid.Visible = true;
            }
            PageTitle.Text = page_title;
            Page.Title = page_title;
            if (weightKoef.Value == "Годовой")
            {
                PageSubTitle.Text = String.Format(page_sub_title, selectedPok.Value.Split(']')[5].TrimStart('.').TrimStart('['), selectedPok.Value.Split(']')[4].TrimStart('.').TrimStart('[').ToLower(), selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[') + " год");
            }
            else
            {
                PageSubTitle.Text = String.Format(page_sub_title, selectedPok.Value.Split(']')[5].TrimStart('.').TrimStart('['), selectedPok.Value.Split(']')[4].TrimStart('.').TrimStart('[').ToLower(), UserComboBox.getLastBlock(selectedPeriod.Value).Split(' ')[1] + " квартал " + selectedPeriod.Value.Split(']')[3].TrimStart('.').TrimStart('[')+" год");
            }

            WallLink.Text = "Для&nbsp;видеостены";
            WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());//String.Format("default.aspx?paramlist=onWall=true");
            selectedPok.Value = "";
        }
        #region Заполнение combo
            Dictionary<string, int> YearsLoad(string sql)
            {
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "years", dt);

                Dictionary<string, int> d = new Dictionary<string, int>();
                d.Add(dt.Rows[0][2].ToString() + " год", 0);
                d.Add(dt.Rows[0][0].ToString() + " " + dt.Rows[0][2].ToString() + " года", 1);
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][2].ToString() != dt.Rows[i - 1][2].ToString())
                    {
                        d.Add(dt.Rows[i][2].ToString() + " год", 0);
                    }
                    d.Add(dt.Rows[i][0].ToString() + " " + dt.Rows[i][2].ToString() + " года", 1);
                }
                return d;
            }

            Dictionary<string, int> PokLoad(string sql)
            {
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "pok", dt);

                Dictionary<string, int> d = new Dictionary<string, int>();
                d.Add(dt.Rows[0][2].ToString(), 0);
                d.Add(dt.Rows[0][0].ToString() + ", " + dt.Rows[0][2].ToString(), 1);
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][2].ToString() != dt.Rows[i - 1][2].ToString())
                    {
                        d.Add(dt.Rows[i][2].ToString(), 0);
                    }
                    d.Add(dt.Rows[i][0].ToString() + ", " + dt.Rows[i][2].ToString(), 1);
                }
                return d;
            }
        #endregion

        #region Обработчики грида
        DataTable dtGrid;
            protected void G_DataBinding(object sender, EventArgs e)
            { 
                G.Columns.Clear();
                G.Bands.Clear();
                DataTable dt = new DataTable();
                dtGrid = new DataTable();
                string sql = "";
                if (selectedPok.Value.Split(']')[4].TrimStart('.').TrimStart('[') == "Базовые показатели")
                {
                    sql = "grid_Base";
                } 
                else
                {
                    sql = "grid_Initial";
                }
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Территория", dt);

                if (dt.Rows.Count > 1)
                {
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.Insert(dt.Columns[i].ColumnName.IndexOf(';'), ", " + dt.Rows[0][i].ToString().ToLower());
                    }
                    dt.Rows.Remove(dt.Rows[0]);
                    if (selectedPok.Value.Split(']')[4].TrimStart('.').TrimStart('[') == "Базовые показатели")
                    {
                        dt.Columns[1].ColumnName = dt.Columns[1].ColumnName.Insert(dt.Columns[1].ColumnName.IndexOf(','), ", " + prevPeriod.Value + " год");
                    }

                    dtGrid.Columns.Add("Территория", typeof(string));
                    for (int i=1;i<dt.Columns.Count;i++)
                    {
                        dtGrid.Columns.Add(dt.Columns[i].ColumnName, typeof(double));
                    }
                    object[] o = new object[dtGrid.Columns.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        o[0] = dt.Rows[i][0].ToString().Split(';')[0];
                        for (int j = 1; j < dt.Rows[i].ItemArray.Length; j++)
                        {
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                o[j] = Convert.ToDouble(dt.Rows[i][j]);
                            }
                        }
                        dtGrid.Rows.Add(o);
                    }

                    
                   
                    
                    G.DataSource = dtGrid;
                }
                else
                {
                    G.DataSource = null;
                }
            }

            protected void G_InitializeLayout(object sender, LayoutEventArgs e)
            {
                double width = 0;
                double width1 = 0;
                if (IsSmallResolution)
                {
                    width = 0.22;
                    width1 = 0.2;
                }
                else
                {
                    width = 0.18;
                    width1=0.14;
                }
                

                headerLayout.childCells.Clear();
                e.Layout.AllowSortingDefault = AllowSorting.No;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
                e.Layout.AllowSortingDefault = AllowSorting.No;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
                e.Layout.HeaderStyleDefault.Wrap = true;
                if (!onWall)
                {
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * width);
                }
                else
                {
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * (onWall ? 0.8 : 0.15));
                }
                e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                    if (!onWall)
                    {
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * width1);
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * (onWall ? 0.4 : 0.13));
                    }
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                    e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                    
                }
             
              /*  if (ComboPok.SelectedNode.Parent.Text == "Базовые показатели")
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N3");
                    for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                    }
                }
                else
                {
                    if (ComboPok.SelectedValue.StartsWith("Оценка"))
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                        for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                        }
                    }
                    else
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N3");
                        for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                        }
                    }
                }*/
                if (dtGrid.Rows.Count > 1)
                {
                    headerLayout.AddCell("Территория");
                    GridHeaderCell headerCell = null;
                    GridHeaderCell headerCell1 = null;
                    headerCell = headerLayout.AddCell(selectedPok.Value.Split(']')[4].TrimStart('.').TrimStart('[') + ", " + selectedPok.Value.Split(']')[5].TrimStart('.').TrimStart('['));
                    for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                    {

                        if (selectedPok.Value.Split(']')[4].TrimStart('.').TrimStart('[') != "Базовые показатели")
                        {
                            headerCell1 = headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[0]);
                            headerCell1.AddCell("Весовой коэффициент " + dtGrid.Rows[0][i].ToString());
                        }
                        else
                        {
                            headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[0]);
                        }
                    }
                }
                headerLayout.ApplyHeaderInfo();
            } 

            protected void G_InitializeRow(object sender, RowEventArgs e)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.BackColor = Color.White;
                }
                if (ComboPok.SelectedNode.Parent.Text == "Базовые показатели")
                {
                    if (e.Row.Cells[1].Value != null)
                    {
                        e.Row.Cells[1].Value = String.Format("{0:N3}", Convert.ToDouble(e.Row.Cells[1].Value));
                    }
                    for (int i = 2; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Value != null)
                        {
                            e.Row.Cells[i].Value = String.Format("{0:N0}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }
                }
                else
                {
                    if (ComboPok.SelectedValue.StartsWith("Оценка"))
                    {
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            if (e.Row.Cells[i].Value != null)
                            {
                                e.Row.Cells[i].Value = String.Format("{0:N2}", Convert.ToDouble(e.Row.Cells[i].Value));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            if (e.Row.Cells[i].Value != null)
                            {
                                e.Row.Cells[i].Value = String.Format("{0:N3}", Convert.ToDouble(e.Row.Cells[i].Value));
                            }
                        }
                    }
                }
            }
        #endregion
  
        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();
           
            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.TitleStartRow = 0;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
           
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 112;

            ReportPDFExporter1.Export(headerLayout, section1);
        }
        #endregion

    }
}