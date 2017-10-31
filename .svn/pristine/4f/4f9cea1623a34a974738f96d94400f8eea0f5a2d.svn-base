using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Collections.ObjectModel;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Documents.Excel;
using Color=System.Drawing.Color;
using Point=System.Drawing.Point;
using LinearGradientBrush = System.Drawing.Drawing2D.LinearGradientBrush;

namespace Krista.FM.Server.Dashboards.reports.FO_0003_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate;
        private DataTable dtDebt;
        private DataTable dtData;
        private DataTable dtChart1;

        private int firstYear = 2009;
        private int endYear = 2011;

        private CustomParam param;
        private CustomParam SelectedYear;
       

     protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = Unit.Empty;

            
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
         
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth);
            UltraChart1.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 100);

            #region Инициализация параметров
              if (param == null)
              {
                  param = UserParams.CustomParam("param");
              }
              if (SelectedYear == null)
              {
                  SelectedYear = UserParams.CustomParam("selectedYear");
              }


         #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExportButton.Visible = true;
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
       }
      
        protected override void Page_Load(Object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)   
            { //Инициализация элементов управления при первом обращении

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0003_0001_Date");
               // DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                
              param.Value ="[Measures].[Годовые назначения]";

        }
        
        SelectedYear.Value = ComboYear.SelectedValue;
        Label1.Text = "Оценка соблюдения норматива расходов на содержание органов государственной власти";
        if (DebtKindButtonList.SelectedIndex == 0)
        {
            Label2.Text = string.Format("Информация консолидированного бюджета субъекта по плану за {0} год",
                                        ComboYear.SelectedValue);
            param.Value ="[Measures].[Годовые назначения]";
        }
        else
        {
            Label2.Text = string.Format("Информация консолидированного бюджета субъекта по факту за {0} год",
                                        ComboYear.SelectedValue);
            param.Value = "[Measures].[Факт]";
        }
       Label3.Text = "Отклонение расходов на содержание органов государственной власти от установленного норматива";
       UltraWebGrid.Bands.Clear();
       UltraWebGrid.DataBind();
       UltraChart1.DataBind();
     
        
       }
       
        #region обработчик грида

     protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0003_0001_Grid");
            dtData = new DataTable();
            //DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtData);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtData);
             
           for (int i = 1; i < dtData.Columns.Count; i++)
            {
                
                if (dtData.Rows[6][i] != DBNull.Value)
                {
                    dtData.Rows[6][i] = Convert.ToDouble(dtData.Rows[6][i])*100;
                }
            }
            if (MoneyButtonList.SelectedIndex == 0)
            {
                for (int i = 1; i < dtData.Columns.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {

                        if (dtData.Rows[j][i] != DBNull.Value)
                        {
                            dtData.Rows[j][i] = Convert.ToDouble(dtData.Rows[j][i])/1000;
                        }
                    }
                    for (int j = 7; j < 9; j++)
                    {

                        if (dtData.Rows[j][i] != DBNull.Value)
                        {
                            dtData.Rows[j][i] = Convert.ToDouble(dtData.Rows[j][i])/1000;
                        }
                    }

                }

                dtData.Rows[0][0] = "Сумма налоговых и неналоговых доходов и дотации на выравнивание бюджетной обеспеченности, тыс. руб.";
                dtData.Rows[1][0] = "налоговые и неналоговые поступления, тыс. руб. ";
                dtData.Rows[2][0] = "дотации на выравнивание уровня бюджетной обеспеченности, тыс. руб.";
                dtData.Rows[3][0] = "Расходы на содержание органов государственной власти субъекта РФ (всего) за вычетом расходов направленных на выполнение полномочий РФ, тыс. руб.";
                dtData.Rows[4][0] = "Доля расходов на содержание органов государственной власти,%";
                dtData.Rows[5][0] = "Норматив расходов,%";
                dtData.Rows[6][0] = "Доля расходов на содержание органов государственной власти в нормативе";
                dtData.Rows[7][0] = "Сумма норматива, тыс. руб.";
                dtData.Rows[8][0] = "Отклонение от норматива, тыс. руб.";
            }
            else
            {
                for (int i = 1; i < dtData.Columns.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {

                        if (dtData.Rows[j][i] != DBNull.Value)
                        {
                            dtData.Rows[j][i] = Convert.ToDouble(dtData.Rows[j][i]) / 1000000;
                        }
                    }
                    for (int j = 7; j < 9; j++)
                    {

                        if (dtData.Rows[j][i] != DBNull.Value)
                        {
                            dtData.Rows[j][i] = Convert.ToDouble(dtData.Rows[j][i]) / 1000000;
                        }
                    }

                }

                dtData.Rows[0][0] = "Сумма налоговых и неналоговых доходов и дотации на выравнивание бюджетной обеспеченности, млн. руб.";
                dtData.Rows[1][0] = "налоговые и неналоговые поступления, млн. руб. ";
                dtData.Rows[2][0] = "дотации на выравнивание уровня бюджетной обеспеченности, млн. руб.";
                dtData.Rows[3][0] = "Расходы на содержание органов государственной власти субъекта РФ (всего) за вычетом расходов направленных на выполнение полномочий РФ, млн. руб.";
                dtData.Rows[4][0] = "Доля расходов на содержание органов государственной власти,%";
                dtData.Rows[5][0] = "Норматив расходов,%";
                dtData.Rows[6][0] = "Доля расходов на содержание органов государственной власти в нормативе";
                dtData.Rows[7][0] = "Сумма норматива, млн. руб.";
                dtData.Rows[8][0] = "Отклонение от норматива, млн. руб.";
            }
         if (dtData.Columns.Count > 2)
                {
                    UltraWebGrid.DataSource = dtData;
                }
            
        }

       protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(220);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
           // e.Layout.Bands[0].Columns[0].
             for (int i = 1; i < UltraWebGrid.Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(76);
                }
          
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {     
              int rowIndex = e.Row.Index;

              if (rowIndex == 6)
              {
                  for (int i = 1; i < e.Row.Cells.Count; i++)
                  {
                      if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          double value = Convert.ToDouble(e.Row.Cells[i].Value);
                          if (value  <= 1)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                              e.Row.Cells[i].Title = "Соблюдение установленного норматива";
                          }
                          else
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                              e.Row.Cells[i].Title = "Превышение установленного норматива";
                          }
                      }

                      e.Row.Cells[i].Style.CustomRules =
                       "background-repeat: no-repeat; background-position: left center; margin: 2px";
                  }
                  e.Row.Cells[0].Title = string.Format("Отношение доли расходов на содержание органов государственной власти к установленному нормативу");
              }
             
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                       {
                           if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                           {
                               double value = Convert.ToDouble(e.Row.Cells[i].Value);
                               e.Row.Cells[i].Value = string.Format("{0:N2}", value);
                               e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                           }
                       }
                
                
                if (rowIndex == 4)
                {
                    double max = double.MinValue;
                    int indMax = 1;
                    double min = double.MaxValue;
                    int indMin = 1;
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToDouble(e.Row.Cells[i].Value) > max)
                            {
                                max = Convert.ToDouble(e.Row.Cells[i].Value);
                                indMax = i;
                            }
                            if (Convert.ToDouble(e.Row.Cells[i].Value) < min)
                            {
                                min = Convert.ToDouble(e.Row.Cells[i].Value);
                                indMin = i;
                            }
                        }
                    }
                    e.Row.Cells[indMin].Style.BackgroundImage = "~/images/starYellowBB.png";
                    e.Row.Cells[indMax].Style.BackgroundImage = "~/images/starGrayBB.png";
                    e.Row.Cells[indMin].Title = "Минимальная доля расходов";
                    e.Row.Cells[indMax].Title = "Максимальная доля расходов";
                    e.Row.Cells[indMax].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    e.Row.Cells[indMin].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    e.Row.Cells[0].Title = "Отношение расходов на содержание органов государственной власти к сумме налоговых и неналоговых поступлений и дотаций на выравнивание бюджетной обеспеченности";
             
                }
                if (rowIndex == 5)
                {
                    e.Row.Cells[0].Title = "Норматив расходов на содержание органов государственной власти, установленный Постановлением Правительства РФ от 29 декабря 2007 г. N 990";
                }
               
                if (rowIndex == 7)
                {
                    e.Row.Cells[0].Title = "Сумма расходов на содержание органов государственной власти в рамках норматива";
                }
                if (rowIndex == 8)
                {
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            double value = Convert.ToDouble(e.Row.Cells[i].Value);
                            if (value >= 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                                e.Row.Cells[i].Title = "Резерв средств установленной суммы норматива";
                            }
                            else
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                                e.Row.Cells[i].Title = "Перерасход средств установленной суммы норматива";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules =
                         "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    }
                }
               if (rowIndex == 1 || rowIndex == 2)
               {
                   e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
                   
               }
               else
               {
                   e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                   e.Row.Cells[0].Style.Font.Size = 9;
               }

               e.Row.Cells[0].Style.Padding.Top = 5;
               e.Row.Cells[0].Style.Padding.Bottom = 5;

               foreach (UltraGridCell cell in e.Row.Cells)
               {
                   if (cell.Value != null && cell.Value.ToString() != string.Empty)
                   {
                       cell.Style.Padding.Right = 7;
                       decimal value;
                       if (decimal.TryParse(cell.Value.ToString(), out value))
                       {
                           if (value < 0)
                           {
                               cell.Style.ForeColor = Color.Red;
                           }
                       }
                   }
               }
      

        }

        #endregion

        #region обработчик диаграмм
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
          if (  DebtKindButtonList2.SelectedIndex==0 )
          {
              UltraChart1.ChartType = ChartType.StackColumnChart;
              
              UltraChart1.Border.Thickness = 0;
              UltraChart1.Axis.X.Extent = 100;
              UltraChart1.Axis.Y.Extent = 100;

              UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
              UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
              UltraChart1.Axis.X.Labels.Visible = false;
              UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
              UltraChart1.Data.SwapRowsAndColumns = true;

              UltraChart1.Legend.Visible = true;
              UltraChart1.Legend.Location = LegendLocation.Top;
              UltraChart1.Legend.SpanPercentage = 15;
              UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Height.Value)/2;

              System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);
              UltraChart1.Legend.Font = font;
              UltraChart1.Axis.X.Labels.SeriesLabels.Font = font;

              UltraChart1.TitleLeft.Visible = true;
              UltraChart1.TitleLeft.Text = "Тыс.руб";
              UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
              UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
             UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

              UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
              UltraChart1.ColorModel.Skin.ApplyRowWise = false;
              UltraChart1.ColorModel.Skin.PEs.Clear();
              for (int i = 1; i <= 3; i++)
              {
                  PaintElement pe = new PaintElement();
                  Color color = Color.White;
                  Color stopColor = Color.White;
                  PaintElementType peType = PaintElementType.Gradient;
                  GradientStyle peStyle = GradientStyle.ForwardDiagonal;
                  switch (i)
                  {
                      case 1:
                          {
                              color = Color.Yellow;
                              stopColor = Color.Goldenrod;
                              peType = PaintElementType.Gradient;
                              peStyle = GradientStyle.ForwardDiagonal;
                              break;
                          }
                      case 2:
                          {
                              color = Color.LimeGreen;
                              stopColor = Color.ForestGreen;
                              peType = PaintElementType.Gradient;
                              peStyle = GradientStyle.ForwardDiagonal;
                              break;
                          }
                      case 3:
                          {
                              color = Color.Red;
                              stopColor = Color.DarkRed;
                              peType = PaintElementType.Gradient;
                              peStyle = GradientStyle.ForwardDiagonal;

                              break;
                          }

                  }
                  pe.Fill = color;
                  pe.FillStopColor = stopColor;
                  pe.ElementType = peType;
                  pe.FillOpacity = 250;
                  pe.FillStopOpacity = 250;
                  pe.FillGradientStyle = peStyle;
                  UltraChart1.ColorModel.Skin.PEs.Add(pe);
              }
              dtChart1 = new DataTable();
              string query = DataProvider.GetQueryText("FO_0003_0001_Chart1");
            //DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart1);
              DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart1);
              for (int i=0; i<dtChart1.Rows.Count;i++)
              {
                  for(int j=1; j<dtChart1.Columns.Count; j++)
                  {
                      if (dtChart1.Rows[i][j] != DBNull.Value)
                      {
                          dtChart1.Rows[i][j] = Convert.ToDouble(dtChart1.Rows[i][j]) / 1000;
                      }
                  }
              }
              
              UltraChart1.Data.SwapRowsAndColumns = false;
              UltraChart1.Series.Clear();
              for (int i = 1; i < dtChart1.Columns.Count; i++)
              {
                  NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                  series.Label = dtChart1.Columns[i].ColumnName;
                  UltraChart1.Series.Add(series);
              }
           
            } 

            else
            {
                UltraChart1.ChartType = ChartType.ColumnChart;
                UltraChart1.ColumnChart.NullHandling = NullHandling.Zero;
                UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
                
                UltraChart1.Data.ZeroAligned = true;
                UltraChart1.Border.Thickness = 0;
                UltraChart1.Axis.X.Extent = 40;
                UltraChart1.Axis.Y.Extent = 40;

                UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                UltraChart1.Axis.X.Labels.Visible = false;
                UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
                UltraChart1.Data.SwapRowsAndColumns = true;

                UltraChart1.Legend.Visible = true;
                UltraChart1.Legend.Location = LegendLocation.Top;
                UltraChart1.Legend.SpanPercentage = 10;
                //UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Height.Value) / 2;

                System.Drawing.Font font = new System.Drawing.Font("Verdana",9);
                UltraChart1.Legend.Font = font;
                UltraChart1.Axis.X.Labels.SeriesLabels.Font = font;

                UltraChart1.TitleLeft.Visible = true;
                UltraChart1.TitleLeft.Text = "%";
                UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;

                UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
                UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
               
                dtChart1 = new DataTable();
                string query = DataProvider.GetQueryText("FO_0003_0001_Chart2");
               // DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart1);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart1);

                UltraChart1.DataSource = dtChart1;
            }
        }
        
        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            
           
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        double value = Convert.ToDouble(box.Value);
                        if (value <= 4.5)
                        {
                            box.DataPoint.Label = string.Format("Соблюдение норматива \n {2} {0} год \n {1:N2}% (норматив - 4,50%)",ComboYear.SelectedValue,value,box.Series.Label);
                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else
                        {
                            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                            int lineStart = (int)xAxis.MapMinimum;
                            int lineLength = (int)xAxis.MapMaximum;

                            Line line = new Line();
                            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                            line.PE.Stroke = Color.DarkGray;
                            line.PE.StrokeWidth = 1;
                            line.p1 = new Point(lineStart, (int)yAxis.Map(4.5));
                            line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(4.5));
                            e.SceneGraph.Add(line);
            
                            box.DataPoint.Label = string.Format("Превышение норматива \n {2} {0} год \n {1:N2}% (норматив - 4,50%)", ComboYear.SelectedValue,value,box.Series.Label);
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Maroon;
                        }
                    }
                    else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                    {
                        box.PE.ElementType = PaintElementType.CustomBrush;
                        LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Green, Color.Red, 45, false);
                        box.PE.CustomBrush = brush;
                    }
                   
                }
            }
        }
        double value;
        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label != null)
                        {
                           // int boxColumn = box.Column + 1;
                            int boxRow = box.Row+1;
                           
                            switch (box.DataPoint.Label)
                            {

                                case "Расходы на содержание органов государственной власти субъекта РФ":
                                    {
                                       if (dtData != null && dtData.Rows.Count != 0 && dtData.Rows[7][boxRow] != DBNull.Value)
                                        {
                                            
                                            box.DataPoint.Label =
                                                string.Format(
                                                    "Расходы \n {0} {1} года \n {2:N2} тыс.руб \n (сумма норматива - {3:N2} тыс.руб)",
                                                    box.Series.Label, ComboYear.SelectedValue, box.Value, dtData.Rows[7][boxRow]);
                                         value = Convert.ToDouble(box.Value); 
                                       }
                                        break;
                                        
                                    }

                                case "Резерв средств по установленной сумме норматива":
                                    {
                                        
                                        box.DataPoint.Label =
                                            string.Format("Резерв средств  \n{1} {0} года \n {2:N2} тыс.руб. ",ComboYear.SelectedValue,box.Series.Label,(Convert.ToDouble(box.Value) - value));
                                        break;
                                    }
                                case "Перерасход средств установленной суммы норматива":
                                    {
                                        box.DataPoint.Label =
                                            string.Format(
                                                "Перерасход средств \n {1} {0} года \n {2:N2} тыс.руб", ComboYear.SelectedValue, box.Series.Label, box.Value);

                                        break;
                                    }
                            }
                        }
                    } 
                }
            }
        }
        #endregion

        #region экспорт
     #region экспорт в PDF
        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
            {
                IText title = e.Section.AddText();
                System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
                title.Style.Font = new Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(Label1.Text);

                title = e.Section.AddText();
                font = new System.Drawing.Font("Verdana", 14);
                title.Style.Font = new Font(font);
                title.AddContent(Label2.Text);

                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = 120;
                }

               
            }
        private void PdfExporter_EndExport(Object sender, EndExportEventArgs e)
            {
                
                ITable table = e.Section.AddTable();
                ITableRow row = table.AddRow();
                ITableCell cell = row.AddCell();
                IText title = cell.AddText();
                System.Drawing.Font font = new System.Drawing.Font("Verdana", 15);
                title.Style.Font = new Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(Label3.Text);

                if (DebtKindButtonList2.SelectedIndex == 0)
                {
                    title = cell.AddText();
                    font = new System.Drawing.Font("Verdana", 14);
                    title.Style.Font = new Font(font);
                    title.AddContent("Абсолютные данные");
                }
                else
                {
                    title = cell.AddText();
                    font = new System.Drawing.Font("Verdana", 14);
                    title.Style.Font = new Font(font);
                    title.AddContent("Относительные данные");
                    
                }
                UltraChart1.Width = 1200;
                cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart1));
             }
        private void PdfExportButton_Click(Object sender, EventArgs e)
            {
                UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
        
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
               }
     #endregion
     #region экспорт в Excel
       private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text; 
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }
       private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }
          private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
          {
              e.CurrentWorksheet.Columns[0].Width = 200*37;
              
              for (int i = 1 ; i<UltraWebGrid.Columns.Count; i++)
              {
                e.CurrentWorksheet.Columns[i].Width = 120 * 37; 
              }

             for(int i=4 ; i<UltraWebGrid.Rows.Count+4; i++)
              {
                  e.CurrentWorksheet.Rows[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                  e.CurrentWorksheet.Rows[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                  e.CurrentWorksheet.Rows[i].Height = 20*37;
              } 
          
          }
          private void ExcelExportButton_Click(object sender, EventArgs e)
          {
              UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
              UltraGridExporter1.ExcelExporter.DownloadName = "reports.xls";
              UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
          }
    #endregion
         
#endregion

      }
}



