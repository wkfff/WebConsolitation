using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0002
{
    public partial class Default: CustomReportPage
    {
      #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;

        private int firstYear = 2007;
        private int endYear;

        private GridHeaderLayout headerLayout;

        public CustomParam LastHalf;
        public CustomParam LastQuater;
        public CustomParam Measures;
        
        public string curDate;
        public string lastDate;

      #endregion

      protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/1.1);
            
            #region инициализация параметров
             if (LastHalf ==null)
             {
                 LastHalf = UserParams.CustomParam("lastHalf");
             }

             if (LastQuater == null)
             {
                 LastQuater = UserParams.CustomParam("lastQuater");
             }

             if (Measures == null)
             {
                Measures = UserParams.CustomParam("measures");
             }
            
            #endregion

            #region  Настройка диаграмм
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);

            UltraChart1.ChartType = ChartType.BarChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X2.Visible = true;
            UltraChart1.Axis.X2.Extent = 30;
            UltraChart1.Axis.X.Extent = 40 ;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X2.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
            UltraChart1.Axis.Y.Extent = 350;
         
            UltraChart1.Data.SwapRowsAndColumns = true;
            

            UltraChart1.TitleBottom.Visible = true;
            UltraChart1.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleBottom.Text = "Человек";

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 5;
            UltraChart1.Legend.Font = font;

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
         
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                GradientStyle peStyle = GradientStyle.ForwardDiagonal;

                switch (i)
                {
                    case 2:
                        {
                            color = Color.Lime;
                            stopColor = Color.LimeGreen;
                            peType = PaintElementType.Gradient;
                            peStyle = GradientStyle.ForwardDiagonal;
                            pe.FillOpacity = 150;
                            pe.FillStopOpacity = 250;
                            break;
                        }
                    case 1:
                        {
                            color = Color.LimeGreen;
                            stopColor = Color.ForestGreen;
                            peType = PaintElementType.Gradient;
                            peStyle = GradientStyle.ForwardDiagonal;
                            pe.FillOpacity = 250;
                            pe.FillStopOpacity = 250;
                            break;
                        }
                   

                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = peStyle;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
         #endregion

            CrossLink1.Visible = true;
            CrossLink1.Text = "Численность&nbsp;гос.&nbsp;служащих";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Фактические&nbsp;затраты&nbsp;на&nbsp;содержание&nbsp;гос.&nbsp;служащих";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0005/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Фактические&nbsp;расходы&nbsp;на&nbsp;содержание&nbsp;гос.&nbsp;служащих";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0007/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Утвержденная&nbsp;штатная&nbsp;численность&nbsp;гос.&nbsp;служащих";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0006/Default.aspx";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0002_Data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string quart = dtDate.Rows[0][2].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                
                ComboPeriod.Title = "Год";
                ComboPeriod.Visible = true;
                ComboPeriod.Width = 100;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboPeriod.MultiSelect = false;
                ComboPeriod.SetСheckedState(endYear.ToString(), true);


                Collection <string> quarter = new Collection<string>();
                quarter.Add("Квартал 2");
                quarter.Add("Квартал 3");
                quarter.Add("Квартал 4");

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 200;
                ComboQuarter.FillValues(quarter);
                ComboQuarter.MultiSelect = false;
                ComboQuarter.SetСheckedState(quart, true);
                
            }

            Page.Title = "Численность работников государственных органов ";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;
            gridCaptionElement.Text = string.Empty;

            if (DebtKindButtonList2.SelectedIndex == 0)
             {
                 Measures.Value = "[Утверждено на дату]";
                 chart1ElementCaption.Text = "Утвержденная численность работников государственных органов в сравнении с прошлым отчетным периодом";
                 UltraChart1.Tooltips.FormatString = "Утверждено штатных единиц \n <DATA_VALUE:N0> чел. \n  по состоянию <ITEM_LABEL> г. ";
             }
            else
             {
                 Measures.Value = "[Фактически на дату]";
                 chart1ElementCaption.Text = "Фактическая численность работников государственных органов в сравнении с прошлым отчетным периодом";
                 UltraChart1.Tooltips.FormatString = "Фактически замещено штатных единиц \n <DATA_VALUE:N0> чел. \n  по состоянию <ITEM_LABEL> г.";
             }

             int year = Convert.ToInt32(ComboPeriod.SelectedValue);
             UserParams.PeriodYear.Value = year.ToString();
             UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}",CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex + 2));
             UserParams.PeriodQuater.Value = ComboQuarter.SelectedValue;

             curDate = string.Empty;
             lastDate = string.Empty;

             if (ComboQuarter.SelectedIndex == 0 ) // выбран второй квартал
             {
                 LastQuater.Value = "Квартал 4";
                 LastHalf.Value = "Полугодие 2";
                 UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboPeriod.SelectedValue)-1).ToString();
                 curDate = string.Format("01.07.{0}",ComboPeriod.SelectedValue);
                 lastDate = string.Format("01.01.{0}",ComboPeriod.SelectedValue);
             }
             if (ComboQuarter.SelectedIndex == 1 ) // выбран третий квартал
             {
                 LastQuater.Value = "Квартал 2";
                 LastHalf.Value = "Полугодие 1";
                 UserParams.PeriodLastYear.Value = ComboPeriod.SelectedValue;
                 curDate = string.Format("01.10.{0}", ComboPeriod.SelectedValue);
                 lastDate = string.Format("01.07.{0}", ComboPeriod.SelectedValue);
             }
             if (ComboQuarter.SelectedIndex == 2 ) // выбран четвертый квартал
             {
                 LastQuater.Value = "Квартал 3";
                 LastHalf.Value = "Полугодие 2";
                 UserParams.PeriodLastYear.Value = ComboPeriod.SelectedValue;
                 curDate = string.Format("01.01.{0}", Convert.ToInt32(ComboPeriod.SelectedValue)+1);
                 lastDate = string.Format("01.10.{0}", ComboPeriod.SelectedValue);
             }
           
            PageSubTitle.Text = string.Format("Информация по состоянию на {0} г.",curDate);
            
           
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
       }

        #region Обработчик грида

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0002_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"Показатели",dtGrid);

            for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++)
            {
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Костромской области", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Здравоохранение_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Образование_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Труд_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("ЗАГС_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("ГосЛесНадзор_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Лес_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Природа_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Животные_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("Все_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("высший исполнительный орган_",
                                                                                   "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace(
                    "Без высшего исполнительного органа_", "");
                dtGrid.Rows[indRow][0] =
                    dtGrid.Rows[indRow][0].ToString().Replace("кроме того Избирательная комиссия",
                                                              string.Format(
                                                                  "кроме того <br/> Избирательная комиссия"));
            }

            UltraWebGrid1.DataSource = dtGrid;
        }

      

        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(190);


            headerLayout.AddCell("Наименование государственного органа", "", 2);

            GridHeaderCell cell0 = headerLayout.AddCell(string.Format("Утверждено штатных единиц на {0} г., чел.", lastDate));
            cell0.AddCell("Всего", string.Format("Утвержденная численность работников гос. органа в штатном расписании на {0} г.", lastDate));
            cell0.AddCell("в том числе гос. гражданская служба", string.Format("Утвержденная численность работников государственной гражданской службы в штатном расписании на {0} г.",lastDate));
            GridHeaderCell cell1 = headerLayout.AddCell(string.Format("Утверждено штатных единиц на {0} г., чел.", curDate));
            cell1.AddCell("Всего", string.Format("Утвержденная численность работников гос. органа в штатном расписании на {0} г.", curDate));
            cell1.AddCell("в том числе гос. гражданская служба", string.Format("Утвержденная численность работников государственной гражданской службы в штатном расписании на {0} г.", curDate));
            GridHeaderCell cell2= headerLayout.AddCell("Темп роста");
            cell2.AddCell("Всего", "Темп роста утвержденной численности работников гос. органа к прошлому отчетному периоду ");
            cell2.AddCell("в том числе гос. гражданская служба", "Темп роста утвержденной численности работников государственной гражданской службы к прошлому отчетному периоду");

            GridHeaderCell cell3 = headerLayout.AddCell(string.Format("Фактически замещено единиц на {0} г., чел", lastDate));
            cell3.AddCell("Всего", string.Format("Фактическая численность работников гос. органа на {0} г.", lastDate));
            cell3.AddCell("в том числе гос. гражданская служба", string.Format("Фактическая численность работников государственной гражданской службы на {0} г.", lastDate));
            GridHeaderCell cell4 = headerLayout.AddCell(string.Format("Фактически замещено единиц на {0} г., чел", curDate));
            cell4.AddCell("Всего", string.Format("Фактическая численность работников гос. органа на {0} г.", curDate));
            cell4.AddCell("в том числе гос. гражданская служба", string.Format("Фактическая численность работников государственной гражданской службы на {0} г.", curDate));
            GridHeaderCell cell5 = headerLayout.AddCell("Темп роста");
            cell5.AddCell("Всего", "Темп роста фактической численности работников гос. органа к прошлому отчетному периоду ");
            cell5.AddCell("в том числе гос. гражданская служба", "Темп роста фактической численности работников государственной гражданской службы к прошлому отчетному периоду");

            headerLayout.ApplyHeaderInfo();
            

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                   e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                   e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80);
                }
       
            for (int i = 5; i < e.Layout.Bands[0].Columns.Count; i += 6)
                {
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                }
            
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
            e.Row.Cells[0].Style.Font.Size = 10;


            if (e.Row.Cells[0].Value.ToString() == "за счет средств областного бюджета" || e.Row.Cells[0].Value.ToString() == "за счет субвенций из федерального бюджета")
            {
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[0].Style.Font.Italic = true;
                e.Row.Cells[0].Style.Padding.Right = 10;
            }
            if (e.Row.Cells[0].Value.ToString() == "итого средний показатель (без высшего исполнительного органа), в том числе" || e.Row.Cells[0].Value.ToString() == "итого средний показатель (включая высший исполнительный орган), в том числе" || e.Row.Cells[0].Value.ToString() == "Итого средний показатель (без Избирательной комиссии), в том числе" || e.Row.Cells[0].Value.ToString() == "Итого средний показатель")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            
            for (int i=1; i<e.Row.Cells.Count;i++)
              {
                  if (i == 5 || i == 6 || i == 11 || i == 12)
                  {
                      if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                              e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";

                          }
                          else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                              e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                          }

                          e.Row.Cells[i].Style.CustomRules =
                              "background-repeat: no-repeat; background-position: left center; margin: 2px";
                      }
                  }
              }

             
              
          }

        #endregion 

        #region Обработчики диаграмм 
          /// <summary>
          /// Инвертирует следование строк в таблице
          /// </summary>
          /// <param name="dt">входная таблица</param>
          /// <returns>выходная таблица</returns>
          private static DataTable ReverseRowsDataTable(DataTable dt)
          {
              DataTable resDt = new DataTable();

              for (int i = 0; i < dt.Columns.Count; i++)
              {
                  DataColumn column = new DataColumn(dt.Columns[i].Caption, dt.Columns[i].DataType);
                  resDt.Columns.Add(column);
              }

              for (int i = dt.Rows.Count; i > 0; i--)
              {
                  DataRow row = resDt.NewRow();
                  row.ItemArray = dt.Rows[i - 1].ItemArray;
                  resDt.Rows.Add(row);
              }

              return resDt;
          }

          protected  void UltraChart1_DataBinding(Object sender, EventArgs e)
          {
              string query = DataProvider.GetQueryText("FO_0001_0002_Chart");
              dtChart = new DataTable();
              DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1" , dtChart);

              for (int i = 0; i < dtChart.Rows.Count; i++)
              {
                  dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString().Replace("Костромской области", "");
              }

              dtChart = ReverseRowsDataTable(dtChart);
              NumericSeries series1 = CRHelper.GetNumericSeries(2, dtChart);
              series1.Label = string.Format("на {0}", curDate); 
              UltraChart1.Series.Add(series1);

              NumericSeries series2 = CRHelper.GetNumericSeries(1, dtChart);
              series2.Label = string.Format("на {0}", lastDate); 
              UltraChart1.Series.Add(series2);
           }

    
        #endregion 

        #region экспорт

       #region экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
           
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 3);
            
        }

        #endregion

        #region Экспорт в Pdf
        
        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
           
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section2);
           

        }


        #endregion

      #endregion


       }
}