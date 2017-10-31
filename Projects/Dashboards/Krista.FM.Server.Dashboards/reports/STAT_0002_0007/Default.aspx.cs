using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0007
{
    public partial class Default : CustomReportPage
    {
        #region поля

        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private DataTable dtData;
        private DataTable dtOperative;
        

        private CustomParam index;
        private CustomParam Date;
        private CustomParam Source;

        private string year;
        private int operativeRow;
        private int numRow = 4;

       #endregion

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

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 1750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        private UltraChart UltraChart2;
        private Label chart2ElementCaption;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = IsSmallResolution ? 870 : CRHelper.GetGridWidth(MinScreenWidth-210);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(MinScreenHeight / 2);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraChart1.Width = CRHelper.GetChartWidth(MinScreenWidth / 2 -30);
            UltraChart1.Height = CRHelper.GetChartHeight(MinScreenHeight / 2);

            UltraChart2 = new UltraChart();
            chart2ElementCaption = new Label();
            chart2ElementCaption.CssClass = "ElementTitle";

            UltraChart2.Width = CRHelper.GetChartWidth(MinScreenWidth / 2 - 30);
            UltraChart2.Height = CRHelper.GetChartHeight(MinScreenHeight / 2);

            #region инициализация параметров запроса
 
            if (index == null)
            {
                index = UserParams.CustomParam("index");
            }
            if (Date==null)
            {
                Date = UserParams.CustomParam("date");
            }

            if (Source == null)
            {
                Source = UserParams.CustomParam("source");
            }

            #endregion


            #region Настройка диаграмм

            UltraChart1.ChartType = ChartType.PieChart; // круговая диаграмма
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 130;
            UltraChart1.Axis.Y.Extent = 130;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:#0.00>%\n<DATA_VALUE:N0>  человек";

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.Margins.Right = 0;
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 10);
            System.Drawing.Font font1 = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Legend.Font = IsSmallResolution ? font : font1;
            UltraChart1.PieChart.Labels.Font = IsSmallResolution ? font : font1;
            UltraChart1.Legend.SpanPercentage = IsSmallResolution ? 35: 22;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;

                switch (i)
                {
                    case 1:
                        {
                            // Курганская
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            // Свердловская
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            // Тюменская
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            // ХМАО
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            // Челябинская
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            // ЯНАО
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }

                }
                pe.Fill = color;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
           
            UltraChart2.ChartType = ChartType.PieChart;
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Extent = 130;
            UltraChart2.Axis.Y.Extent = 130;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:#0.00>%\n<DATA_VALUE:N0> человек ";
           
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
           
            UltraChart2.Legend.Font = IsSmallResolution ? font : font1;
            UltraChart2.PieChart.Labels.Font = IsSmallResolution ? font : font1;
            UltraChart2.Legend.SpanPercentage = IsSmallResolution ? 22 : 22;
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
           
            UltraChart2.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart2_ChartDrawItem);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart2.DeploymentScenario.FilePath = "../../TemporaryImages/";
            UltraChart2.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png";

          //  UltraChart2.Axis.Y.LineThickness = 1;

            
            GradientEffect gradientEffect = new GradientEffect();
            gradientEffect.Style = GradientStyle.ForwardDiagonal;
            gradientEffect.Coloring = GradientColoringStyle.Darken;
            UltraChart2.Effects.Effects.Add(gradientEffect);
           
            #endregion

            UltraGridExporter1.Visible = true;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            //UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);

            if (IsSmallResolution)
            {
                HorizontalTD.Visible = false;
                VerticalTD.Visible = true;
                VerticalChartTD.Controls.Add(chart2ElementCaption);
                VerticalChartTD.Controls.Add(UltraChart2);
            }
            else
            {
                VerticalTD.Visible = false;
                HorizontalTD.Visible = true;
                HorizontalChartTD.Controls.Add(chart2ElementCaption);
                HorizontalChartTD.Controls.Add(UltraChart2);
            }
        }

    
        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
              Primitive primitive = e.SceneGraph[i];
                if (primitive is Wedge)
                {
                    Wedge box = (Wedge)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label != null)
                        {
                            box.DataPoint.Label = RegionsNamingHelper.ShortName(box.DataPoint.Label);
                        }
                    }

                }
            }
        }
        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart1.Legend.Location == LegendLocation.Top) || (UltraChart1.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart1.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChart1.Legend.SpanPercentage * (int)UltraChart1.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }
        void UltraChart2_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart2.Legend.Location == LegendLocation.Top) || (UltraChart2.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart2.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChart2.Legend.SpanPercentage * (int)UltraChart2.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart2.Legend.Margins.Left + UltraChart2.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                FillComboIndex();
                ComboIndex.Title = "Показатель";
                ComboIndex.Width = 630;
                ComboIndex.Visible = true;
                ComboIndex.SetСheckedState("Общая численность безработных (по методологии МОТ)", true);
            }
            year = string.Empty;
            string month = string.Empty;
            string data = string.Empty;
            chart1ElementCaption.Text = string.Empty;
            chart2ElementCaption.Text = string.Empty;

            Page.Title = "Анализ вклада субъектов РФ по численности безработных граждан в ситуацию в УрФО и в РФ";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = "Анализ вклада по численности безработных граждан в ситуацию в УрФО и в РФ субъектов Российской Федерации, входящих в Уральский федеральный округ";
            
            if (ComboIndex.SelectedIndex == 0)
            {
                dtData = new DataTable();
                string query = DataProvider.GetQueryText("STAT_0002_0007_DataTotalNumber");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtData);
            }
            else
            {
                dtData = new DataTable();
                string query = DataProvider.GetQueryText("STAT_0002_0007_DataCount");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtData);
            }
            
            if (dtData.Rows.Count > 0)
            {
                month = dtData.Rows[0][3].ToString().ToLower();
                year = dtData.Rows[0][0].ToString();
                CRHelper.SaveToErrorLog(string.Format("{0}", year));
                data = dtData.Rows[0][4].ToString();
                Date.Value = data; 
                if (ComboIndex.SelectedIndex == 0)
                {
                    chart1ElementCaption.Text = string.Format("Вклад субъектов РФ по числу безработных зарегистрированных в службе занятости в УрФО за {0} {1} года", month, year);
                    chart2ElementCaption.Text = string.Format("Вклад УрФО по числу безработных зарегистрированных в службе занятости в РФ за {1} {0} года",year, month);
                    gridCaptionElement.Text = "Динамика вклада субъектов РФ  по общей численности безработных (по методологии МОТ), человек";
                    index.Value =
                    "[Труд].[Трудовые ресурсы].[Все показатели].[Общая численность безработных (на конец месяца)]";
                }
                else
                {
                    index.Value = "[Труд].[Трудовые ресурсы].[Все показатели].[Число безработных, зарегистрированных в службе занятости (на конец месяца)]";
                    gridCaptionElement.Text = "Динамика вклада субъектов РФ по числу безработных зарегистрированных в службе занятости, человек";
                }

            }
          /*  if (ComboIndex.SelectedIndex==0)
            {
                
               // Source.Value ="IIF (IsEmpty (([Measures].[Нарастающий итог],[Территории].[Сопоставимый].CurrentMember,[Труд].[Трудовые ресурсы].CurrentMember,[Период].[Год Квартал Месяц].CurrentMember,[Источники данных].[Все источники данных].[СТАТ Отчетность - ФСГС по Свердловской области] )),([Measures].[Нарастающий итог],[Источники данных].[Все источники данных].[СТАТ Отчетность - ФСГС]  ),([Measures].[Нарастающий итог],[Источники данных].[Все источники данных].[СТАТ Отчетность - ФСГС по Свердловской области] )  )";
            }
            else
            {
                
               
              //  Source.Value = "([Measures].[Нарастающий итог] + LookupCube (\"[СТАТ_Мониторинг ситуации на рынке труда]\",\"  ([Источники данных].[Все источники данных].[СТАТ Мониторинг ситуации на рынке труда - 2009],\" + MemberToStr  ([Территории].[Сопоставимый].CurrentMember  ) + \",[Период].[Период].[Данные всех периодов].[\" + [Период].[Год Квартал Месяц].CurrentMember.Parent.Parent.Parent.Name + \"].[\" + [Период].[Год Квартал Месяц].CurrentMember.Parent.Parent.Name + \"].[\" + [Период].[Год Квартал Месяц].CurrentMember.Parent.Name + \"].[\" + [Период].[Год Квартал Месяц].CurrentMember.Name +\"],[Показатели].[Рынок труда].[Все].[Общая численность зарегистрированных безработных граждан]  )\"  )   )";
               // Source.Value ="IIF (IsEmpty (([Measures].[Нарастающий итог],[Территории].[Сопоставимый].CurrentMember,[Труд].[Трудовые ресурсы].CurrentMember,[Период].[Год Квартал Месяц].CurrentMember,[Источники данных].[Все источники данных].[СТАТ Отчетность - ФСГС по Свердловской области] )),([Measures].[Нарастающий итог],[Источники данных].[Все источники данных].[СТАТ Отчетность - ФСГС]  ),([Measures].[Нарастающий итог],[Источники данных].[Все источники данных].[СТАТ Отчетность - ФСГС по Свердловской области] )  )";
            }
            
            */
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        private void FillComboIndex()
        {
            Dictionary<string, int> index = new Dictionary<string, int>();
            index.Add("Общая численность безработных (по методологии МОТ)", 0);
            index.Add("Число безработных зарегистрированных в службе занятости", 0);
            ComboIndex.FillDictionaryValues(index);
        }

    #region Обработчик грида

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0007_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            for (int i = 0; i < dtGrid.Rows.Count;i++)
            {
                string[] caption = dtGrid.Rows[i][0].ToString().Split(';');
                dtGrid.Rows[i][0] = caption[0];
            }
            //
            if (ComboIndex.SelectedIndex == 1)

            {
               query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_Operative"));
                DataTable dtRF = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtRF);

                query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_unjobedOperative"));
                dtOperative = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtOperative);

                string period = dtGrid.Rows[dtGrid.Rows.Count - 4][dtGrid.Columns.Count - 1].ToString();
                DateTime date = CRHelper.PeriodDayFoDate(period);

                int operativeRowNum = 0;
                int rowNum = 0;
                int num = 4;
                for (rowNum = 0; rowNum < dtGrid.Rows.Count; rowNum += 4)
                {
                    if (dtGrid.Rows[rowNum][0].ToString()[0] == '2')
                    {
                        year = dtGrid.Rows[rowNum][0].ToString();
                    }
                    // если строка есть, но какая то колонка пустая
                    if (dtGrid.Rows[rowNum][0].ToString()[0] != '2' && (
                        (dtGrid.Rows[rowNum]["Курганская область"] == DBNull.Value) || dtGrid.Rows[rowNum]["Свердловская область"] == DBNull.Value ||
                        dtGrid.Rows[rowNum]["Тюменская область"] == DBNull.Value || dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"] == DBNull.Value
                        || dtGrid.Rows[rowNum]["Челябинская область"] == DBNull.Value || dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"] == DBNull.Value
                        || dtGrid.Rows[rowNum]["УрФО"] == DBNull.Value || dtGrid.Rows[rowNum]["РФед"] == DBNull.Value))
                    {
                        string checkingPeriod =
                            dtGrid.Rows[rowNum][dtGrid.Columns.Count - 1].ToString().Replace(
                                "[Период].[Год Квартал Месяц]", "[Период].[Период]");
                    

                        while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                               (operativeRowNum < dtOperative.Rows.Count - 1 &&
                                dtOperative.Rows[operativeRowNum][1].ToString() ==
                                dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                        {
                           operativeRowNum++;
                           
                        }
                        
                        if (dtGrid.Rows[rowNum][0].ToString() == "Январь")
                        {
                            num = 8;
                        }

                        if (dtGrid.Rows[rowNum]["УрФО"] == DBNull.Value)
                        {
                            dtGrid.Rows[rowNum]["УрФО"] = dtOperative.Rows[operativeRowNum][8];
                            if (dtGrid.Rows[rowNum - num]["УрФО"] != DBNull.Value)
                            {
                                dtGrid.Rows[rowNum + 1]["УрФО"] = Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"])/
                                                                  Convert.ToDouble(dtGrid.Rows[rowNum - num]["УрФО"]);
                               
                            }
                            if (dtGrid.Rows[rowNum]["РФед"]!= DBNull.Value)
                            {
                                dtGrid.Rows[rowNum + 2]["УрФО"] = Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"])/
                                                                  Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                            }

                        }

                        if (dtGrid.Rows[rowNum]["РФед"] == DBNull.Value)
                        {
                           operativeRow=0;

                           while (checkingPeriod != dtRF.Rows[operativeRow][1].ToString() ||
                              (operativeRow < dtRF.Rows.Count - 1 &&
                               dtRF.Rows[operativeRowNum][1].ToString() ==
                               dtRF.Rows[operativeRowNum + 1][1].ToString()))
                            {
                                operativeRow++;
                            }

                            dtGrid.Rows[rowNum]["РФед"] = dtOperative.Rows[operativeRow][2];
                            dtGrid.Rows[rowNum + 1]["РФед"] = Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]) /
                                                              Convert.ToDouble(dtGrid.Rows[rowNum - num]["РФед"]);

                        }

                         if (dtGrid.Rows[rowNum]["Курганская область"] == DBNull.Value)
                          {
                              dtGrid.Rows[rowNum]["Курганская область"] = dtOperative.Rows[operativeRowNum][2];

                              if (dtGrid.Rows[rowNum - num]["Курганская область"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 1]["Курганская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - num]["Курганская область"]);
                                 
                              }

                              if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 2]["Курганская область"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                              }
                              dtGrid.Rows[rowNum + 3]["Курганская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                           
                          }


                          if (dtGrid.Rows[rowNum]["Свердловская область"] == DBNull.Value)
                          {
                              dtGrid.Rows[rowNum]["Свердловская область"] = dtOperative.Rows[operativeRowNum][3];
                              if (dtGrid.Rows[rowNum - num]["Свердловская область"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 1]["Свердловская область"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum - num]["Свердловская область"]);
                              }
                              if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 2]["Свердловская область"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                              }
                              dtGrid.Rows[rowNum + 3]["Свердловская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);

                          }


                          if (dtGrid.Rows[rowNum]["Тюменская область"] == DBNull.Value)
                          {
                              dtGrid.Rows[rowNum]["Тюменская область"] = dtOperative.Rows[operativeRowNum][4];

                              if (dtGrid.Rows[rowNum - num]["Тюменская область"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 1]["Тюменская область"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum - num]["Тюменская область"]);
                              }

                              if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 2]["Тюменская область"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                              }

                              dtGrid.Rows[rowNum + 3]["Тюменская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                          }
                         
                          if (dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"] == DBNull.Value)
                          {
                              dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"] = dtOperative.Rows[operativeRowNum][5];
                              if (dtGrid.Rows[rowNum - num]["Ханты-Мансийский автономный округ"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 1]["Ханты-Мансийский автономный округ"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum - num]["Ханты-Мансийский автономный округ"]);
                              }
                              if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 2]["Ханты-Мансийский автономный округ"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                              }
                              dtGrid.Rows[rowNum + 3]["Ханты-Мансийский автономный округ"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"]) / Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                          }

                          if (dtGrid.Rows[rowNum]["Челябинская область"] == DBNull.Value)
                          {
                              dtGrid.Rows[rowNum]["Челябинская область"] = dtOperative.Rows[operativeRowNum][6];
                              if (dtGrid.Rows[rowNum - num]["Челябинская область"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 1]["Челябинская область"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum - num]["Челябинская область"]);
                              }
                              if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 2]["Челябинская область"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                              }
                              dtGrid.Rows[rowNum + 3]["Челябинская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                          }

                          if (dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"] == DBNull.Value)
                          {
                              dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"] = dtOperative.Rows[operativeRowNum][7];
                              if (dtGrid.Rows[rowNum - num]["Ямало-Ненецкий автономный округ"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 1]["Ямало-Ненецкий автономный округ"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum - num]["Ямало-Ненецкий автономный округ"]);
                              }
                              if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                              {
                                  dtGrid.Rows[rowNum + 2]["Ямало-Ненецкий автономный округ"] =
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"])/
                                      Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                              }
                              dtGrid.Rows[rowNum + 3]["Ямало-Ненецкий автономный округ"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"]) / Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);

                          }
                     }
                    
                   
                    
                }
                if (operativeRowNum == 0)
                {
                    string checkingPeriod = dtGrid.Rows[dtGrid.Rows.Count - 4][dtGrid.Columns.Count - 1].ToString().Replace(
                            "[Период].[Год Квартал Месяц]", "[Период].[Период]");

                    CRHelper.SaveToErrorLog(checkingPeriod);

                    while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                           (operativeRowNum < dtOperative.Rows.Count - 1 &&
                            dtOperative.Rows[operativeRowNum][1].ToString() ==
                            dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                    {
                        operativeRowNum++;
                    }

                }
                
                for (; operativeRowNum < dtOperative.Rows.Count; operativeRowNum++)
                {
                    string periodOperative = dtOperative.Rows[operativeRowNum][1].ToString();
                    DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                   
                   if (dateOperative > date && dateOperative.Month > date.Month )
                    {
                        if (operativeRowNum < dtOperative.Rows.Count - 1)
                        {
                            DateTime dateOperativeNext = CRHelper.PeriodDayFoDate(dtOperative.Rows[operativeRowNum + 1][1].ToString());

                           if (dateOperativeNext.Month.ToString() =="1" && dateOperative.Month.ToString() == "12")
                           {
                               for (int i = 1; i < 5; i++)
                               {
                                   DataRow row = dtGrid.NewRow();
                                   row[0] = dateOperativeNext.Year.ToString();
                                   year = dateOperativeNext.Year.ToString();
                                   CRHelper.SaveToErrorLog(string.Format("+{0}",year));
                                   row[1] = 1;
                                   row[2] = 1;
                                   row[3] = 1;
                                   row[4] = 1;
                                   row[5] = 1;
                                   row[6] = 1;
                                   row[7] = 1;
                                   row[8] = 1;

                                   dtGrid.Rows.Add(row);

                               }
                                rowNum += 4;
                                numRow = 8;
                                
                           }
                            
                            if (dateOperativeNext.Month > dateOperative.Month)
                           {
                                DataRow row = dtGrid.NewRow();
                                row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                                row["Курганская область"] = dtOperative.Rows[operativeRowNum][2];
                                row["Свердловская область"] = dtOperative.Rows[operativeRowNum][3];
                                row["Тюменская область"] = dtOperative.Rows[operativeRowNum][4];
                                row["Ханты-Мансийский автономный округ"] = dtOperative.Rows[operativeRowNum][5];
                                row["Челябинская область"] = dtOperative.Rows[operativeRowNum][6];
                                row["Ямало-Ненецкий автономный округ"] = dtOperative.Rows[operativeRowNum][7];
                                row["УрФО"] = dtOperative.Rows[operativeRowNum][8];

                                for (; operativeRow < dtRF.Rows.Count; operativeRow++)
                                {
                                    string periodRF = dtRF.Rows[operativeRow][1].ToString();
                                    DateTime dateRF = CRHelper.PeriodDayFoDate(periodRF);
                                   if (dateRF > date && (dateRF.Month > date.Month ))
                                    {
                                     
                                       if (operativeRow < dtRF.Rows.Count - 1)
                                        {
                                            DateTime dateRFNext =
                                                CRHelper.PeriodDayFoDate(
                                                    dtRF.Rows[operativeRow + 1][1].ToString());

                                            if (dateRFNext.Month > dateRF.Month)
                                            {
                                                row["РФед"] = dtRF.Rows[operativeRow][2];
                                            }
                                        }
                                    }
                                }

                                dtGrid.Rows.Add(row);

                                DataRow row1 = dtGrid.NewRow();
                                row1[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                                row1["Курганская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum-numRow]["Курганская область"]);
                                row1["Свердловская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Свердловская область"]);
                                row1["Тюменская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Тюменская область"]);
                                row1["Ханты-Мансийский автономный округ"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Ханты-Мансийский автономный округ"]);
                                row1["Челябинская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Челябинская область"]);
                                row1["Ямало-Ненецкий автономный округ"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Ямало-Ненецкий автономный округ"]);
                                row1["УрФО"] = Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["УрФО"]);
                                if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value && dtGrid.Rows[rowNum]["РФед"].ToString() != string.Empty && dtGrid.Rows[rowNum - numRow]["РФед"] != DBNull.Value && dtGrid.Rows[rowNum - numRow]["РФед"].ToString() != string.Empty )
                                {
                                    row1["РФед"] = Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"])/
                                                   Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["РФед"]);
                                }
                                 
                               dtGrid.Rows.Add(row1);

                                row = dtGrid.NewRow();
                                row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                                if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                                {
                                    row["Курганская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                    row["Свердловская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                    row["Тюменская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                    row["Ханты-Мансийский автономный округ"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                    row["Челябинская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                    row["Ямало-Ненецкий автономный округ"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                    row["УрФО"] = Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"])/
                                                  Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                }
                                dtGrid.Rows.Add(row);

                               
                                row = dtGrid.NewRow();
                                row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                                if (dtGrid.Rows[rowNum]["УрФО"] != DBNull.Value)
                                {
                                    row["Курганская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                    row["Свердловская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                    row["Тюменская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                    row["Ханты-Мансийский автономный округ"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                    row["Челябинская область"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                    row["Ямало-Ненецкий автономный округ"] =
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"])/
                                        Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                }

                                dtGrid.Rows.Add(row);
                                rowNum += 4;
                                numRow = 4;
                           }
                        }
                        else
                        {
                            DataRow row = dtGrid.NewRow();
                            row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                            row["Курганская область"] = dtOperative.Rows[operativeRowNum][2];
                            row["Свердловская область"] = dtOperative.Rows[operativeRowNum][3];
                            row["Тюменская область"] = dtOperative.Rows[operativeRowNum][4];
                            row["Ханты-Мансийский автономный округ"] = dtOperative.Rows[operativeRowNum][5];
                            row["Челябинская область"] = dtOperative.Rows[operativeRowNum][6];
                            row["Ямало-Ненецкий автономный округ"] = dtOperative.Rows[operativeRowNum][7];
                            row["УрФО"] = dtOperative.Rows[operativeRowNum][8];
                            row["РФед"] = dtRF.Rows[dtRF.Rows.Count-1][2];
                            
                           
                            dtGrid.Rows.Add(row);

                            DataRow row1 = dtGrid.NewRow();
                            row1[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                            row1["Курганская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Курганская область"]);
                            row1["Свердловская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Свердловская область"]);
                            row1["Тюменская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Тюменская область"]);
                            row1["Ханты-Мансийский автономный округ"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Ханты-Мансийский автономный округ"]);
                            row1["Челябинская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Челябинская область"]);
                            row1["Ямало-Ненецкий автономный округ"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["Ямало-Ненецкий автономный округ"]);
                            row1["УрФО"] = Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]) / Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["УрФО"]);
                            if (dtGrid.Rows[rowNum]["РФед"] != DBNull.Value && dtGrid.Rows[rowNum]["РФед"].ToString() != string.Empty && dtGrid.Rows[rowNum - numRow]["РФед"] != DBNull.Value && dtGrid.Rows[rowNum - numRow]["РФед"].ToString() != string.Empty)
                            {
                                row1["РФед"] = Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"])/
                                               Convert.ToDouble(dtGrid.Rows[rowNum - numRow]["РФед"]);
                            }
                            dtGrid.Rows.Add(row1);

                            row = dtGrid.NewRow();
                            row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                            if ( dtGrid.Rows[rowNum]["РФед"] != DBNull.Value)
                            {
                                row["Курганская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"])/
                                                            Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                row["Свердловская область"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                row["Тюменская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"])/
                                                           Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                row["Ханты-Мансийский автономный округ"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                row["Челябинская область"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                row["Ямало-Ненецкий автономный округ"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                                row["УрФО"] = Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"])/
                                              Convert.ToDouble(dtGrid.Rows[rowNum]["РФед"]);
                            }
                            dtGrid.Rows.Add(row);


                            row = dtGrid.NewRow();
                            row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                            if (dtGrid.Rows[rowNum]["УрФО"] != DBNull.Value)
                            {
                                row["Курганская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Курганская область"])/
                                                            Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                row["Свердловская область"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Свердловская область"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                row["Тюменская область"] = Convert.ToDouble(dtGrid.Rows[rowNum]["Тюменская область"])/
                                                           Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                row["Ханты-Мансийский автономный округ"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Ханты-Мансийский автономный округ"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                row["Челябинская область"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Челябинская область"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                                row["Ямало-Ненецкий автономный округ"] =
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["Ямало-Ненецкий автономный округ"])/
                                    Convert.ToDouble(dtGrid.Rows[rowNum]["УрФО"]);
                            }
                            dtGrid.Rows.Add(row);
                            
                        
                        }
                   }
                   
                }
              
                dtGrid.Columns.RemoveAt(dtGrid.Columns.Count - 1);
                dtGrid.AcceptChanges();
            }
            else
            {
                dtGrid.Columns.RemoveAt(dtGrid.Columns.Count - 1);
                dtGrid.AcceptChanges();
            }
            //
            if (dtGrid.Rows.Count > 0)
                {

                    UltraWebGrid1.DataSource = dtGrid;
                }
        }

        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
           e.Layout.GroupByBox.Hidden = true;
           e.Layout.Bands[0].HeaderStyle.Wrap = true;
           e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
           
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = false;
            e.Layout.Bands[0].Columns[0].Width = IsSmallResolution ? CRHelper.GetColumnWidth(75) : CRHelper.GetColumnWidth(120);

            for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = IsSmallResolution ? CRHelper.GetColumnWidth(90) :CRHelper.GetColumnWidth(101);
            }

            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 8,"РФ","");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 7, "УрФО","");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 1, "Курганская область","");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 2, "Свердловская область","");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 3, "Тюменская область","");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 4, "Ханты-Мансийский автономный округ","");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 5, "Челябинская область","");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 6, "Ямало-Ненецкий автономный округ","");
             
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
            int rowIndex = e.Row.Index;
             e.Row.Cells[0].Style.BackColor = Color.WhiteSmoke;
            

           int year;
            if (Int32.TryParse(e.Row.Cells[0].Value.ToString(), out year))
            {
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Value = "";
                }
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count;

            }
          
            int k = ((rowIndex+1) % 4);
            int p = (rowIndex%4);

           switch (k)
            {
                case 0:
                    {
                        e.Row.Cells[0].Style.BorderDetails.WidthTop = 0;
                        e.Row.Cells[0].Style.BorderDetails.WidthBottom = 1;
                        break;
                    }
                case 1:
                case 2:
                case 3:
                    {
                        e.Row.Cells[0].Style.BorderDetails.WidthTop = 0;
                        e.Row.Cells[0].Style.BorderDetails.WidthBottom = 0;
                        break;
                    }

            }



            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[i].Style.Padding.Right = 10;
                
                switch (p)
                {
                    case 0:
                        {
                            if (!Int32.TryParse(e.Row.Cells[0].Value.ToString(), out year))
                            {
                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                                {
                                    int value = Convert.ToInt32(e.Row.Cells[i].Value);
                                    e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                    e.Row.Cells[i].Title = string.Format("{0} человек", e.Row.Cells[i].Value);
                                }
                            }
                            break;
                        }
                    case 1:
                        {
                           
                            if (!Int32.TryParse(e.Row.Cells[0].Value.ToString(), out year))
                            {
                                e.Row.Cells[0].Value = " ";
                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                                {
                                    object value = e.Row.Cells[i].Value;
                                    // CRHelper.SaveToErrorLog(value.ToString()); 
                                    double doubleValue = Double.Parse(value.ToString(), NumberStyles.Any);

                                    if (100*doubleValue > 100)
                                    {

                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                        e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";
                                        e.Row.Cells[i].Style.CustomRules =
                                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                    }
                                    else if (100*doubleValue < 100)
                                    {
                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                        e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                                        e.Row.Cells[i].Style.CustomRules =
                                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                    }
                                }

                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                                {
                                    double dValue = Convert.ToDouble(e.Row.Cells[i].Value);
                                    e.Row.Cells[i].Value = string.Format("{0:P2}", dValue);
                                }
                            }
                            break;
                           }
                    case 2:
                        {  
                            if (!Int32.TryParse(e.Row.Cells[0].Value.ToString(), out year))
                            {
                                e.Row.Cells[0].Value = " ";
                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                                {
                                    double dValue = Convert.ToDouble(e.Row.Cells[i].Value);
                                    e.Row.Cells[i].Value = string.Format("{0:P2}", dValue);
                                    e.Row.Cells[i].Title = "Доля в РФ";

                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            
                            if (!Int32.TryParse(e.Row.Cells[0].Value.ToString(), out year))
                            {
                                e.Row.Cells[0].Value = " ";
                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                                { 

                                    double dValue = Convert.ToDouble(e.Row.Cells[i].Value);
                                    e.Row.Cells[i].Value = string.Format("{0:P2}", dValue);
                                    e.Row.Cells[i].Title = "Доля в УрФО";
                                  
                                }
                            }
                            break;
                        }
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (rowIndex % 4 == 0)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                  
                }
                
            }

            if (Int32.TryParse(e.Row.Cells[0].Value.ToString(), out year))
            {
                switch (p)
                {   case 0:
                    {
                         e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                         e.Row.Cells[0].Style.Padding.Left = IsSmallResolution ? 20 : 40;
                         e.Row.Cells[0].Style.BorderDetails.WidthLeft = 1;
                         e.Row.Cells[0].Style.BorderDetails.WidthRight = 1;
                         e.Row.Cells[0].Style.BorderDetails.WidthTop = 1;
                         e.Row.Cells[0].Style.BorderDetails.WidthBottom = 1;
                          break;
                    }
                    case 1:
                    case 2:
                    case 3:
                        {
                           
                            e.Row.Hidden = true;
                            break;
                        }
                   
                }
            }
    
        }
    #endregion

        #region обработчик диаграмм

        protected void UltraChart1_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0007_Chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart1);

          if (ComboIndex.SelectedIndex == 1)
            {
                query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_unjobedOperative"));
                dtOperative = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtOperative);

               
              int operativeRowNum = 0;
              string checkingPeriod = dtChart1.Rows[0][dtChart1.Columns.Count - 1].ToString().Replace(
                                "[Период].[Год Квартал Месяц]", "[Период].[Период]");
              while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                               (operativeRowNum < dtOperative.Rows.Count - 1 &&
                                dtOperative.Rows[operativeRowNum][1].ToString() ==
                                dtOperative.Rows[operativeRowNum + 1][1].ToString()))
              {
                 operativeRowNum++;
              }

              if (operativeRowNum != dtOperative.Rows.Count - 1)
              {
                  int j = 2;
                  for (int i = 0; i < dtChart1.Rows.Count; i++)
                  {

                      dtChart1.Rows[i][1] = dtOperative.Rows[dtOperative.Rows.Count - 1][j];
                      j++;
                  }

                  string periodOperative = dtOperative.Rows[dtOperative.Rows.Count - 1][1].ToString();
                  DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                  string period = CRHelper.RusMonth(dateOperative.Month);
                  chart1ElementCaption.Text =
                      string.Format(
                          "Вклад субъектов РФ по числу безработных зарегистрированных в службе занятости в УрФО за {0} {1} года",
                          period, year);
              }
                dtChart1.Columns.RemoveAt(dtChart1.Columns.Count - 1);
                dtChart1.AcceptChanges();
            }
            else
            {
                dtChart1.Columns.RemoveAt(dtChart1.Columns.Count - 1);
                dtChart1.AcceptChanges();
            }

            UltraChart1.DataSource = dtChart1;
          /*  UltraChart1.Series.Clear();
            for (int i = 1; i < dtChart1.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                series.Label = dtChart1.Columns[i].ColumnName;
                UltraChart1.Series.Add(series);
            }*/
        }

        protected void UltraChart2_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0007_Chart");
            dtChart2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма2", dtChart2);
           
            query = DataProvider.GetQueryText("STAT_0002_0007_Chart2");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма", dtChart);
            
            dtChart2.Rows[1][0] = dtChart.Rows[1][1];
            dtChart2.AcceptChanges();

            if (ComboIndex.SelectedIndex == 1)
            {
                query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_unjobedOperative"));
                dtOperative = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtOperative);


               int operativeRowNum = 0;
                string checkingPeriod = dtChart2.Rows[0][dtChart2.Columns.Count - 1].ToString().Replace(
                                  "[Период].[Год Квартал Месяц]", "[Период].[Период]");
                while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                                 (operativeRowNum < dtOperative.Rows.Count - 1 &&
                                  dtOperative.Rows[operativeRowNum][1].ToString() ==
                                  dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                {
                    operativeRowNum++;
                }

               if (operativeRowNum != dtOperative.Rows.Count - 1)
                {
                   /*int rowOperative = dtOperative.Rows.Count - 1;
                   while (Convert.ToDouble(dtOperative.Rows[rowOperative][8]) == Convert.ToDouble(dtOperative.Rows[rowOperative][9])||
                        ( ( rowOperative  < dtOperative.Rows.Count - 1 &&
                                  dtOperative.Rows[rowOperative ][1].ToString() ==
                                  dtOperative.Rows[rowOperative + 1][1].ToString())) )
                   {
                     rowOperative--;
                   }
                  */
                    query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_Operative"));
                    DataTable dtRF = new DataTable();
                    DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtRF);

                    dtChart2.Rows[0][1] = Convert.ToDouble(dtRF.Rows[dtRF.Rows.Count-1][2]) - Convert.ToDouble(dtOperative.Rows[dtOperative.Rows.Count - 1 ][8]);
                    dtChart2.Rows[1][1] = dtOperative.Rows[dtOperative.Rows.Count - 1 ][8];


                    string periodOperative = dtOperative.Rows[dtOperative.Rows.Count - 1][1].ToString();
                    DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                    string period = CRHelper.RusMonth(dateOperative.Month);
                    chart2ElementCaption.Text =
                           string.Format(
                               "Вклад УрФО по числу безработных зарегистрированных в службе занятости в РФ за {1} {0} года",
                                year, period);
                
                }
                dtChart2.Columns.RemoveAt(dtChart2.Columns.Count - 1);
                dtChart2.AcceptChanges();
            }
            else
            {
                dtChart2.Columns.RemoveAt(dtChart2.Columns.Count - 1);
                dtChart2.AcceptChanges();
            }

             UltraChart2.DataSource = dtChart2;
        /*   UltraChart2.Series.Clear();
            for (int i = 1; i < dtChart2.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
                series.Label = dtChart2.Columns[i].ColumnName;
                UltraChart2.Series.Add(series);
            }
         */ 
        }
      #endregion
        #region экспорт
        #region Экспорт в Excel
        private void ExcelExporter_BeginExport(Object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }
        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Caption;
        }

        private void ExcelExporter_EndExport(Object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;
            e.CurrentWorksheet.Rows[3].Height = 17 * 37;
            e.CurrentWorksheet.Rows[4].Height = 17 * 37;
            for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0";
            }


            for (int i = 2; i < UltraWebGrid1.Rows.Count + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Height = 17 * 37;
            }
        }

        private void ExcelExportButton_Click(Object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        #endregion

        #region Экспорт в Pdf
       
        private void SetExportGridParams(UltraWebGrid grid)
         {
            foreach (UltraGridRow row in grid.Rows)
            {
                int rowIndex = row.Index;
                for (int i = 0; i < grid.Columns.Count;i++)
                {
                    if ((rowIndex > 1) && ((rowIndex + 1) % 4 == 0))
                    {
                        row.Cells[i].Style.BorderDetails.WidthTop = 0;
                        row.Cells[i].Style.BorderDetails.WidthBottom = 1;
                    }
                    else
                    {
                        row.Cells[i].Style.BorderDetails.WidthTop = 0;
                        row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                    }

                }
            }

           for (int i=0; i< grid.Rows.Count; i+=4)
           {
               grid.Rows[i + 1].Cells[0].Value = "";
               grid.Rows[i + 2].Cells[0].Value = "";
               grid.Rows[i + 3].Cells[0].Value = "";
           }
         }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            SetExportGridParams(UltraWebGrid1);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }

        private void PdfExporter_BeginExport(Object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = false;
            title.AddContent(PageSubTitle.Text);

            title = e.Section.AddText();
            title.AddContent("                 ");

            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            UltraChart1.Width = 430;
            UltraChart2.Width = 350;

            cell.Width = new FixedWidth((float)UltraChart1.Width.Value-40);
            cell.AddText().AddContent(chart1ElementCaption.Text);

            cell = row.AddCell();
            cell.Width = new FixedWidth((float)UltraChart2.Width.Value - 50);
            cell.AddText().AddContent(chart2ElementCaption.Text);

            row = table.AddRow();
            cell = row.AddCell();
          
            cell.Width = new FixedWidth((float)UltraChart1.Width.Value);
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart1));

            cell = row.AddCell();
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart2));

            e.Section.AddPageBreak();
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = 120;
            }

        }
        #endregion
        #endregion



    }
}