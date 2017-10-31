using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment=System.Drawing.ContentAlignment;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Symbol = Infragistics.UltraChart.Core.Primitives.Symbol;

namespace Krista.FM.Server.Dashboards.reports.FO_0047_0007
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        
        private int firstYear = 2010;
        private int endYear = 2011;
        private int serial = 0;
        private string query;
        private DateTime currentdate;
        private GridHeaderLayout headerLayout;

        private CustomParam measures;
        private CustomParam param;

        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2.0);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);

            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2.0);
           
            #region Настройка диаграмм

            UltraChart1.ChartType = ChartType.LineChart;
            UltraChart1.Axis.X.Extent = 100;
            UltraChart1.Axis.Y.Extent = 55;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "%";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL> \n <DATA_VALUE:P2> %";
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            LineAppearance lineAppearance1 = new LineAppearance();
            lineAppearance1.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance1.Thickness = 0;
            lineAppearance1.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            lineAppearance1.IconAppearance.PE.Fill = Color.Yellow;
            UltraChart1.LineChart.LineAppearances.Add(lineAppearance1);
      //       UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
       
             UltraChart2.ChartType = ChartType.LineChart;
             UltraChart2.Axis.X.Extent = 100;
             UltraChart2.Axis.Y.Extent = 55;
             UltraChart2.TitleLeft.Visible = true;
             UltraChart2.TitleLeft.Text = "%";
             UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
             UltraChart2.Border.Thickness = 0;
             UltraChart2.Tooltips.FormatString = "<SERIES_LABEL> \n <DATA_VALUE:P2> %";
             UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
             UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
            lineAppearance1 = new LineAppearance();
            lineAppearance1.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance1.Thickness = 0;
            lineAppearance1.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            lineAppearance1.IconAppearance.PE.Fill = Color.Yellow;
            UltraChart2.LineChart.LineAppearances.Add(lineAppearance1);

        //   UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);

          #endregion

          #region Инициализация параметров 
            measures = UserParams.CustomParam("measures");
            param = UserParams.CustomParam("param");
          #endregion

          GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
          ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
          ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.PointSet")
                {
                    PointSet ps = primitive as PointSet;

                    measures.Value = " Measures.[Отношение просроченной кредиторской задолженности к расходам бюджета]";
                    query = DataProvider.GetQueryText("FO_0047_0007_Max");
                    DataTable dtGridMax = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridMax);

                    ps.PE.ElementType = PaintElementType.Gradient;
                    ps.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                    ps.PE.Fill = Color.Yellow;
                    ps.PE.FillStopColor = Color.Yellow;

                    for (int j = 0; j < ps.points.Length; j++)
                    {
                      double value = Convert.ToDouble(ps.points[j].Value);
                      if (value == Convert.ToDouble(dtGridMax.Rows[0][1]) || value == Convert.ToDouble(dtGridMax.Rows[1][1]) || value == Convert.ToDouble(dtGridMax.Rows[2][1]) || value == Convert.ToDouble(dtGridMax.Rows[3][1]))
                        {
                            
                            ps.points[j].PE.ElementType = PaintElementType.Gradient;
                            ps.points[j].PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                            ps.points[j].PE.Fill = Color.Red;
                            ps.points[j].PE.FillStopColor = Color.Red;
                        }
                    }

                }

            }
            serial = 0;
        }

        void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Box)
                {
                    Box box = (Box) primitive;

                    
                    if (box.DataPoint != null)
                          {
                              if (box.DataPoint.Label != null)
                              {
                                box.DataPoint.Label = string.Format("{0}\n{1}\n{2}", box.Series.Label, box.DataPoint.Label, box.Value);
                              }
                          }  
                }


            }
         
        }
        
      protected override void Page_Load(object sender, EventArgs e)
       {
           base.Page_PreLoad(sender, e);

           if (!Page.IsPostBack)
           {
             
               dtDate = new DataTable();
               query = DataProvider.GetQueryText("FO_0047_0007_date");
               DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
               endYear = Convert.ToInt32(dtDate.Rows[0][0]);

               ComboYear.Title = "Год";
               ComboYear.Width = 100;
               ComboYear.MultiSelect = false;
               ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
               ComboYear.SetСheckedState(endYear.ToString(), true);

               Dictionary<string,int> quarters = new Dictionary<string, int>();
               quarters.Add("Квартал 2",0);
               quarters.Add("Квартал 3",0);
               quarters.Add("Квартал 4",0);

               ComboQuarter.Title = "Квартал";
               ComboQuarter.Width = 150;
               ComboQuarter.MultiSelect = false;
               ComboQuarter.FillDictionaryValues(quarters);
               ComboQuarter.SetСheckedState(dtDate.Rows[0][2].ToString(), true);

           }

           UserParams.PeriodYear.Value = ComboYear.SelectedValue;
           UserParams.PeriodHalfYear.Value =string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex+2));
           UserParams.PeriodQuater.Value = string.Format("Квартал {0}", ComboQuarter.SelectedIndex + 2);

           Page.Title = "Анализ устойчивости местных бюджетов Вологодской области";
           PageTitle.Text = Page.Title;

           int month = ComboQuarter.SelectedIndex == 0 ? 7 : ComboQuarter.SelectedIndex == 1 ? 10 : 1; 
           currentdate = new DateTime(ComboQuarter.SelectedIndex == 2 ? Convert.ToInt32(ComboYear.SelectedValue)+1 : Convert.ToInt32(ComboYear.SelectedValue), month , 1);
           PageSubTitle.Text = string.Format("Информация по состоянию на {0:dd.MM.yyyy} г., тыс. руб.", currentdate);
           
           if (ComboQuarter.SelectedIndex == 2 ) // квартал 4
           {
               param.Value = "[Measures].[Факт]";
           }
           else // квартал 2 или 3
           {
               param.Value = "[Measures].[Годовые назначения]";
           }


          headerLayout = new GridHeaderLayout(UltraWebGrid);
        UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();

           chart1ElementCaption.Text = "Распределение территорий по доле просроченной кредиторской задолженности в общем объеме расходов местного бюджета";
           chart2ElementCaption.Text = "Распределение территорий по доле кредиторской задолженности в общем объеме собственных доходов местного бюджета";
           UltraChart1.DataBind();
           UltraChart2.DataBind();

          }
        

        #region Обработчики грида

      protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0047_0007_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);
            if (dtGrid.Rows.Count>0)
            {

                UltraWebGrid.DataSource = dtGrid;
            }
        }
        
       protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            
            for (int i = 1; i < UltraWebGrid.Columns.Count; i++)
            {
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption.ToLower();
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], caption.Contains("отношение") ? "P2": "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            headerLayout.AddCell("Наименование МР(ГО)");
            headerLayout.AddCell("Просроченная кредиторская задолженность (бюджетные средства)", string.Format("Просроченная кредиторская задолженность (бюджетные средства) по состоянию на {0:dd.MM.yyyy} г.", currentdate));
            headerLayout.AddCell("Объем расходов бюджета", string.Format("Объем расходов бюджета по состоянию на {0: dd.MM.yyyy} г.", currentdate));
            headerLayout.AddCell("Отношение просроченной кредиторской задолженности к расходам бюджета", string.Format("Отношение просроченной кредиторской задолженности к расходам бюджета по состоянию на {0:dd.MM.yyyy} г.",currentdate));
            headerLayout.AddCell("Кредиторская задолженность (бюджетные средства)", string.Format("Кредиторская задолженность (бюджетные средства) по состоянию на {0:dd.MM.yyyy} г.", currentdate));
            headerLayout.AddCell("Собственные доходы бюджета", string.Format("Собственные доходы бюджета по состоянию на {0:dd.MM.yyyy} г.", currentdate));
            headerLayout.AddCell("Отношение кредиторской задолженности к собственным доходам бюджета", string.Format("Отношение кредиторской задолженности к собственным доходам бюджета по состоянию на {0:dd.MM.yyyy} г.",currentdate));
            headerLayout.ApplyHeaderInfo();
          }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            
            for (int i = 1; i < UltraWebGrid.Columns.Count; i ++)
             {
               
                if (e.Row.Cells[0].Value.ToString() == "Итого задолженность по МР(ГО)")
                {
                    e.Row.Style.Font.Bold = true;
                }

             }

            measures.Value = "Measures.[Отношение просроченной кредиторской задолженности к расходам бюджета] ";
            string query = DataProvider.GetQueryText("FO_0047_0007_Max");
            DataTable dtGridMax1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridMax1);

            measures.Value = "Measures.[Отношение кредиторской задолженности к собственным доходам бюджета] ";
            query = DataProvider.GetQueryText("FO_0047_0007_Max");
            DataTable dtGridMax2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridMax2);

           for (int rowNum = 0; rowNum<UltraWebGrid.Rows.Count; rowNum++)
            {
                int i = 3;
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(e.Row.Cells[i].Value);
                    if (value == Convert.ToDouble(dtGridMax1.Rows[0][1]) || value == Convert.ToDouble(dtGridMax1.Rows[1][1]) || value == Convert.ToDouble(dtGridMax1.Rows[2][1]) || value == Convert.ToDouble(dtGridMax1.Rows[3][1]))
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                        e.Row.Cells[i].Title = "Наиболее высокая доля просроченной кредиторской задолженности в расходах местного бюджета";
                    }
                }

                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                i = 6;
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(e.Row.Cells[i].Value);

                    if (value == Convert.ToDouble(dtGridMax2.Rows[0][1]) || value == Convert.ToDouble(dtGridMax2.Rows[1][1]) || value == Convert.ToDouble(dtGridMax2.Rows[2][1]) || value == Convert.ToDouble(dtGridMax2.Rows[3][1]))
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                        e.Row.Cells[i].Title = "Наиболее высокая доля кредиторской задолженности в собственных доходах местного бюджета";
                    }
                }
               e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
           }

        }
   #endregion 

        #region Обработчики диаграмм
        protected void UltraChart1_DataBinding(Object sender, EventArgs e)
        {
           query = DataProvider.GetQueryText("FO_0047_0007_Chart1");
           dtChart1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart1);
            if (dtChart1.Rows.Count>0)
            {
                for (int colNum = 1; colNum < dtChart1.Columns.Count; colNum++ )
                {
                    dtChart1.Columns[colNum].ColumnName = dtChart1.Columns[colNum].ColumnName.Replace("муниципальный район", "МР").Replace("муниципальное образование", "МО").Replace("\"", "'");
                }

                UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL> \n{0} год \n {1} \n <DATA_VALUE:P2>", ComboYear.SelectedValue, ComboQuarter.SelectedValue);
                UltraChart1.DataSource = dtChart1;
            }
           
        }

        protected void UltraChart2_DataBinding(Object sender, EventArgs e)
        {
          query = DataProvider.GetQueryText("FO_0047_0007_Chart2");
          dtChart2 = new DataTable();
          DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма2", dtChart2);
          if (dtChart2.Rows.Count > 0)
          {
              for (int colNum = 1; colNum < dtChart2.Columns.Count; colNum++)
              {
                  dtChart2.Columns[colNum].ColumnName = dtChart2.Columns[colNum].ColumnName.Replace("муниципальный район", "МР").Replace("муниципальное образование", "МО").Replace("\"", "'"); 
              }
              UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL> \n{0} год \n {1} \n <DATA_VALUE:P2>", ComboYear.SelectedValue, ComboQuarter.SelectedValue);
              UltraChart2.DataSource = dtChart2;
          }

        }
        #endregion
        
        #region Экспорт

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart2, chart2ElementCaption.Text, sheet3, 3);
        }

        #endregion
      
        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            UltraChart1.Width = 1000;
            ReportPDFExporter1.Export(UltraChart1,chart1ElementCaption.Text, section2);
            UltraChart2.Width = 1000;
            ReportPDFExporter1.Export(UltraChart2,chart2ElementCaption.Text, section3);
        }

        #endregion

        #endregion
    }
}
