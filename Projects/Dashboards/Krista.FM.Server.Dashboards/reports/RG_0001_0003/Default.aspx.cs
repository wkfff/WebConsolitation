using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Web.UI.GridControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using RowEventArgs = Infragistics.WebUI.UltraWebGrid.RowEventArgs;


namespace Krista.FM.Server.Dashboards.reports.RG_0001_0003
{
    public partial class Default : CustomReportPage
    {
        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1200; }
        }

        #region Поля

        public string TEMPORARY_URL_PREFIX = "../../..";
        public string REPORT_ID = "RG_0001_0003";
        private string str;

        private DataTable GridDt = new DataTable();
        private DataTable dtDate;
        private DateTime currentDate;
        private string query;
        private int year;
        private int day;
        private string month;
        private string replSrt1;
        private string replSrt2;
        
        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        //года
        private CustomParam years;
        #endregion
        
      protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick1.AutoSizeStyle = GridAutoSizeStyle.None;
            GridBrick1.Height = Unit.Empty;
          GridBrick1.Width = IsSmallResolution ? 950 :Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            GridBrick1.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick1.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion
          
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            years = UserParams.CustomParam("years");
            #endregion
       
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                query = DataProvider.GetQueryText("RG_0001_0003_date");
                dtDate = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query,dtDate);
                year = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();
                day = Convert.ToInt32(dtDate.Rows[0][4]);

                DateTime date = new DateTime(year,CRHelper.MonthNum(month), day);
                CustomCalendar1.WebCalendar.SelectedDate = date;

            }

            currentDate = CustomCalendar1.WebCalendar.SelectedDate;
            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);

            year = CustomCalendar1.WebCalendar.SelectedDate.Year;
            day = CustomCalendar1.WebCalendar.SelectedDate.Day;
            month = CRHelper.RusMonthGenitive(CustomCalendar1.WebCalendar.SelectedDate.Month);

            string par = string.Empty;
            for (int i = year - 4; i <= year - 1; i++)
            {
                par += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}],", i);
            }
            years.Value = par;

            topBorder = 100 / 12 * currentDate.Month;
            bottomBorder = topBorder / 2.5;
           
            Page.Title = String.Format("Охват и доступность услуг дошкольного образования");
            Label2.Text = string.Format("в субъектах Российской Федерации входящих в состав Центрального Федерального округа по состоянию на {0} {1} {2} года", day, CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)), year );
            
            lbInfo.Text = string.Empty;
            UltraChart1.Visible = true;
            GridDataBind();
            BindInfoText();
        }

        private void BindInfoText()
        {
            string query = DataProvider.GetQueryText("RG_0001_0003_text1");
            DataTable dtInfo = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);
            if (dtInfo.Rows.Count>0)
            {
              lbInfo.Text = string.Format(@"&nbsp;&nbsp; По состоянию на <b> {0} {1} {2} </b> общая численность детей в возрасте от 1 до 7 лет составляет <b> {3:N2} тыс. человек</b>, численность детей данного возраста, охваченных услугами дошкольного образования, составляет <b>{4:N2} тыс.человек</b> (&nbsp;<img align = 'center' src='../../TemporaryImages/Chart_RG_0001_0003_{5}.png'> {6:N1}% от общей численности детей от 1 до 7 лет).", day, CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)), year, dtInfo.Rows[0][1], dtInfo.Rows[0][2], dtInfo.Rows[0][3].ToString().Replace(",", "_").Replace(".", "_"), dtInfo.Rows[0][3]);
              replSrt1 = string.Format("<img align = 'center' src='../../TemporaryImages/Chart_RG_0001_0003_{0}.png'>", dtInfo.Rows[0][3].ToString().Replace(",", "_").Replace(".", "_"));
            }

            query = DataProvider.GetQueryText("RG_0001_0003_text2");
            dtInfo = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);
            if (dtInfo.Rows.Count>0)
            {
                DataTable dtChart = new DataTable();
                dtChart.Columns.Add(new DataColumn("1", typeof(double)));

                int value = 0;
                if (dtInfo.Rows[0][2] != DBNull.Value && dtInfo.Rows[0][2].ToString() != string.Empty)
                {

                    UltraChart1.Width = 23;
                    UltraChart1.Height = 23;

                    UltraChart1.ChartType = ChartType.PieChart;
                    UltraChart1.Border.Thickness = 0;
                    UltraChart1.BackColor = Color.Transparent;
                    UltraChart1.BorderColor = Color.Transparent;
                    UltraChart1.PieChart.OthersCategoryPercent = 0;
                    UltraChart1.PieChart.OthersCategoryText = "Прочие";
                    UltraChart1.PieChart.Labels.Visible = false;
                    UltraChart1.PieChart.Labels.LeaderLinesVisible = false;
                    UltraChart1.PieChart.Labels.FontColor = Color.White;
                    UltraChart1.Tooltips.FormatString =
                        "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";

                    UltraChart1.Legend.Visible = false;
                    UltraChart1.PieChart.StartAngle = 270;
                    UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
                    UltraChart1.ColorModel.Skin.ApplyRowWise = true;
                    UltraChart1.ColorModel.Skin.PEs.Clear();
                    for (int i = 1; i <= 2; i++)
                    {
                        PaintElement pe = new PaintElement();
                        Color color = GetColor(i);
                        Color stopColor = GetColor(i);

                        pe.Fill = color;
                        pe.FillStopColor = stopColor;
                        pe.ElementType = PaintElementType.Gradient;
                        pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                        pe.FillOpacity = 150;
                        UltraChart1.ColorModel.Skin.PEs.Add(pe);
                    }

                    value = Convert.ToInt32(dtInfo.Rows[0][2]);
                    DataRow row = dtChart.NewRow();
                    row[0] = value;
                    dtChart.Rows.Add(row);

                    row = dtChart.NewRow();
                    row[0] = 0;
                    dtChart.Rows.Add(row);

                    row = dtChart.NewRow();
                    row[0] = 0;
                    dtChart.Rows.Add(row);

                    row = dtChart.NewRow();
                    row[0] = 100 - value;
                    dtChart.Rows.Add(row);

                   
                   // UltraChart1.DeploymentScenario.ImageURL = String.Format("../../TemporaryImages/Chart_RG_0001_0003_{0}.png",value.ToString().Replace(',', '_').Replace('.', '_'));
                    //для публикации
                   

                    UltraChart1.DataSource = dtChart;
                    UltraChart1.DataBind();
                    UltraChart1.SaveTo(Path.Combine(CRHelper.BasePath, String.Format("TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'))), ImageFormat.Png);
                  
                    //UltraChart1.SaveTo(String.Format("D:\\Users\\WWWRoot\\astrakhan.ifinmon.ru\\reports\\RG_0001_0003\\TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_')), ImageFormat.Png);
                    
                    StringWriter writer = new StringWriter();
                    HtmlTextWriter output = new HtmlTextWriter(writer);

                   // UltraChart1.RenderControl(output);
                }
                lbInfo.Text = String.Format(@"{0}  <br />&nbsp;&nbsp; Очередность в дошкольные образовательные   учреждения  в целом по ЦФО составляет <b>  {1:N0}  человек </b> (<img align = 'center' src='../../TemporaryImages/Chart_RG_0001_0003_{3}.png'> {2:N1}% от общей численности детей в возрасте от 1 до 7 лет).", lbInfo.Text, dtInfo.Rows[0][1], dtInfo.Rows[0][2], value.ToString().Replace(',', '_').Replace('.', '_'));
                replSrt2 = string.Format("<img align = 'center' src='../../TemporaryImages/Chart_RG_0001_0003_{0}.png'>", value.ToString().Replace(',', '_').Replace('.', '_'));
            }

            query = DataProvider.GetQueryText("RG_0001_0003_text3");
            DataTable dtText = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtText);
            str = string.Empty;
            int endYear;
            int firstYear;
            if (dtText.Rows.Count > 0 && dtInfo.Rows.Count > 0)
            {
                if (dtInfo.Rows[0][3] != DBNull.Value && dtInfo.Rows[0][3].ToString() != string.Empty)
                {
                    firstYear = Convert.ToInt32(dtInfo.Rows[0][3]);
                    if (dtInfo.Rows[0][4] != DBNull.Value && dtInfo.Rows[0][4].ToString() != string.Empty)
                    {
                        endYear = Convert.ToInt32(dtInfo.Rows[0][4]);
                        for (int i = firstYear; i <= endYear; i++)
                        {
                            if (i == firstYear)
                            {
                                str += string.Format("в {0} году планируется в ", i);
                            }
                            else
                            {
                                str += string.Format("&nbsp;в {0} году - ", i);
                            }
                            for (int j = 0; j < dtText.Rows.Count; j++)
                            {

                                if (i == Convert.ToInt32(dtText.Rows[j][1]))
                                {
                                    str += string.Format("{0}, ",
                                                         dtText.Rows[j][0].ToString().Replace("ая", "ой").Replace(
                                                             " область", ""));
                                }

                            }
                            str += "областях;";
                        }
                    }
                }


                string nullregions = string.Empty;
                query = DataProvider.GetQueryText("RG_0001_0003_text4");
                DataTable dtNull = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                   dtNull);

                if (dtNull.Rows.Count > 0)
                {
                    string list = string.Empty;
                    for (int i = 0; i < dtNull.Rows.Count; i++)
                    {
                        list += string.Format("{0}",
                                              dtNull.Rows[i][0].ToString().Replace("ая", "ой").Replace("область",
                                                                                                       "области").
                                                  Replace("г. Москва", "г.&nbsp;Москва"));
                    }
                    nullregions = string.Format("В {0} с {1} года очерёдность отсутствует.", list,
                                                Convert.ToInt32(CustomCalendar1.WebCalendar.SelectedDate.Year) - 1);
                }

                lbInfo.Text =
                    String.Format(
                        @"{0}  <br/>&nbsp;&nbsp; Предполагаемый год ликвидации очерёдности в регионах находится в диапазоне от <b> {1}</b> до <b> {2}</b> года. Ликвидировать  очерёдность {3}. {4}",
                        lbInfo.Text, dtInfo.Rows[0][3], dtInfo.Rows[0][4], str
                                                                               .TrimEnd(';').Replace(", областях",
                                                                                                     " областях"),
                        nullregions);

                lbInfo.Text = String.Format(@"{0}", lbInfo.Text);
            }
            UltraChart1.Visible = false;
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("RG_0001_0003_grid");
            GridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Субъект РФ", GridDt);

            if (GridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(GridDt.Columns.Count - 1);
                levelRule.AddFontLevel("1", new Font(GridBrick1.Grid.DisplayLayout.RowStyleDefault.Font.Name, 10, FontStyle.Bold));
                GridBrick1.AddIndicatorRule(levelRule);
                GridBrick1.DataTable = GridDt;
            }
        }

        double topBorder = 0;
        double bottomBorder = 0;

      void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            DataTable dtChart = new DataTable();
            dtChart.Columns.Add(new DataColumn("1", typeof(double)));

            if (e.Row.Cells[6].Value != null)
            {
                SetupChart();

                double value = Convert.ToDouble(e.Row.Cells[6].Value.ToString());
                
                DataRow row = dtChart.NewRow();
                row[0] = value;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 0;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 0;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 100 - value;
                dtChart.Rows.Add(row);

                UltraChart1.DeploymentScenario.ImageURL = String.Format("../../TemporaryImages/Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));

                UltraChart1.DataSource = dtChart;
                UltraChart1.DataBind();

                StringWriter writer = new StringWriter();
                HtmlTextWriter output = new HtmlTextWriter(writer);

               // UltraChart1.RenderControl(output);

                //две строки для публикации
                
               // UltraChart1.SaveTo(String.Format("D:\\Users\\WWWRoot\\astrakhan.ifinmon.ru\\reports\\RG_0001_0003\\TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_')), ImageFormat.Png);
                
                UltraChart1.SaveTo(Path.Combine(CRHelper.BasePath, String.Format("TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'))), ImageFormat.Png);
                e.Row.Cells[6].Style.BackgroundImage = String.Format("../../TemporaryImages/Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));
               
                //e.Row.Cells[6].Style.BackgroundImage = String.Format("~/TemporaryImages/Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));
                e.Row.Cells[6].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-left: 2px";

                e.Row.Cells[6].Value = String.Format("{1:N2}", e.Row.Cells[4].Value, e.Row.Cells[6].Value);
            }
          
        }

      private void SetupChart()
      {
          UltraChart1.Width = 23;
          UltraChart1.Height = 23;

          UltraChart1.ChartType = ChartType.PieChart;
          UltraChart1.Border.Thickness = 0;
          UltraChart1.BackColor = Color.Transparent;
          UltraChart1.BorderColor = Color.Transparent;
          UltraChart1.PieChart.OthersCategoryPercent = 0;
          UltraChart1.PieChart.OthersCategoryText = "Прочие";
          UltraChart1.PieChart.Labels.Visible = false;
          UltraChart1.PieChart.Labels.LeaderLinesVisible = false;
          UltraChart1.PieChart.Labels.FontColor = Color.White;
          UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";

          UltraChart1.Legend.Visible = false;
          UltraChart1.PieChart.StartAngle = 270;
          UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
          UltraChart1.ColorModel.Skin.ApplyRowWise = true;
          UltraChart1.ColorModel.Skin.PEs.Clear();
          for (int i = 1; i <= 2; i++)
          {
              PaintElement pe = new PaintElement();
              Color color = GetColor(i);
              Color stopColor = GetColor(i);

              pe.Fill = color;
              pe.FillStopColor = stopColor;
              pe.ElementType = PaintElementType.Gradient;
              pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
              pe.FillOpacity = 150;
              UltraChart1.ColorModel.Skin.PEs.Add(pe);
          }
      }

      private static Color GetColor(int i)
      {
          switch (i)
          {
              case 1:
                  {
                      return Color.ForestGreen;
                  }
              case 2:
                  {
                      return Color.Red;
                  }
              default:
                  {
                      return Color.White;
                  }
          }
      }
       
        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
           if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

           e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
           e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
           e.Layout.Bands[0].Columns[0].Width = IsSmallResolution ? CRHelper.GetColumnWidth(125) : CRHelper.GetColumnWidth(100);
           e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

           int columnCount = e.Layout.Bands[0].Columns.Count;

           e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

           for (int i = 1; i > columnCount; i++)
           {
               e.Layout.Bands[0].Columns[i].Width = IsSmallResolution ? CRHelper.GetColumnWidth(80) : CRHelper.GetColumnWidth(100);
               e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           }

           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N1");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N0");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "0000");
           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "0000");

           e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[7].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[8].CellStyle.HorizontalAlign = HorizontalAlign.Right;
           e.Layout.Bands[0].Columns[9].CellStyle.HorizontalAlign = HorizontalAlign.Right;

           e.Layout.Bands[0].Columns[1].Width = IsSmallResolution ? CRHelper.GetColumnWidth(80) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[2].Width = IsSmallResolution ? CRHelper.GetColumnWidth(80) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[3].Width = IsSmallResolution ? CRHelper.GetColumnWidth(80) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[4].Width = IsSmallResolution ? CRHelper.GetColumnWidth(80) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[5].Width = IsSmallResolution ? CRHelper.GetColumnWidth(80) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[6].Width = IsSmallResolution ? CRHelper.GetColumnWidth(90) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[7].Width = IsSmallResolution ? CRHelper.GetColumnWidth(70) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[8].Width = IsSmallResolution ? CRHelper.GetColumnWidth(70) : CRHelper.GetColumnWidth(80);
           e.Layout.Bands[0].Columns[9].Width = IsSmallResolution ? CRHelper.GetColumnWidth(110) : CRHelper.GetColumnWidth(100);
          
           
            GridHeaderLayout headerLayout = GridBrick1.GridHeaderLayout;
            headerLayout.AddCell("Субъект Российской Федерации");

            GridHeaderCell cell1 = headerLayout.AddCell("Численность детей, охваченных услугами дошкольного образования, всего человек");
            GridHeaderCell cell11 = cell1.AddCell("Справочно по данным Минобразования России");
            cell1.AddCell(string.Format("На {0:dd.MM.yyyy}", currentDate));
            for (int i = year - 4; i <= year - 1; i++)
            {
                cell11.AddCell(string.Format("в {0} году", i));
            }
            
            headerLayout.AddCell("Охват детей услугами дошкольного образования в возрасте от 1 до 7 лет, %");
            GridHeaderCell cell2 = headerLayout.AddCell("Численность детей дошкольного возраста, состоящих на учете для определения в государственные и муниципальные ДОУ (очередность от 1 до 7 лет), человек, из них:");
            cell2.AddCell("в возрасте от 1 до 7 лет","",2);
            cell2.AddCell("в возрасте от 3 до 7 лет","",2);
            headerLayout.AddCell("Предполагаемый год ликвидации очередности");
            
            headerLayout.ApplyHeaderInfo();
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = "Охват и доступность услуг дошкольного образования";
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick1.GridHeaderLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = "Охват и доступность услуг дошкольного образования";
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 80;

            if (IsSmallResolution)
            {
                GridBrick1.Grid.Columns[6].Width = CRHelper.GetColumnWidth(80);
                GridBrick1.Grid.Columns[7].Width = CRHelper.GetColumnWidth(70);
                GridBrick1.Grid.Columns[8].Width = CRHelper.GetColumnWidth(70);
                GridBrick1.Grid.Columns[9].Width = CRHelper.GetColumnWidth(80);
            }
            else
            {
                GridBrick1.Grid.Columns[9].Width = CRHelper.GetColumnWidth(80);
            }
            ReportPDFExporter1.Export(GridBrick1.GridHeaderLayout, section1);
            
            IText title = section2.AddText();
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbInfo.Text.Replace("&nbsp;", " ").Replace("<br/>", "").Replace("<b>", "").Replace("</b>", "").Replace(replSrt1, "").Replace(replSrt2, "").Replace("<br />", ""));
            
        }

        #endregion
    }
}