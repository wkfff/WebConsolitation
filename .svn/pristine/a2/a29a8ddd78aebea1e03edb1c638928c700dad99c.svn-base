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

namespace Krista.FM.Server.Dashboards.reports.FNS_0004_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private DataTable dtSrPercent;
        private DataTable dtFinance;

        private int firstYear = 2005;
        private int endYear = 2012;

        private double max = double.MinValue;
        private double min = double.MaxValue;

        private string param;
        private string query;
       // private string month;
        private double st;

        Collection<double> MAX = new Collection<double>();
        Collection<double> MIN = new Collection<double>();
        Collection<string> header = new Collection<string>();
       
        private CustomParam SelectYear;
        private CustomParam LastYear;
        private CustomParam SelectFin;
        private CustomParam selectedFO;
        private CustomParam CurrentDate;

        private GridHeaderLayout headerLayout;

        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }
      /// <summary>
        /// Выбраны ли все федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);

            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2);
           
            #region Инициализация параматров
            if (SelectYear == null)
            {
                SelectYear = UserParams.CustomParam("select_year");
            }
            if (LastYear == null)
            {
                LastYear = UserParams.CustomParam("last_year");
            }

            if (SelectFin == null)
            {
                SelectFin = UserParams.CustomParam("Select_Fin");
            }

            if (selectedFO == null)
            {
                selectedFO = UserParams.CustomParam("selected_fo");
            }

            if (CurrentDate == null)
            {
                CurrentDate = UserParams.CustomParam("cur_date");
            }

            #endregion

            #region Настройка диаграмм

            UltraChart1.ChartType = ChartType.StackColumnChart;

            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 60;
            UltraChart1.Axis.Y.Extent = 100;
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> \n <SERIES_LABEL> \n <DATA_VALUE:N2> тыс.руб.";
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.Y.Labels.Font = font;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = font;
          
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 16;
            UltraChart1.Legend.Font = font;
            UltraChart1.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart1.Width.Value / 3);
            
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "Тыс.руб";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                GradientStyle peStyle = GradientStyle.Horizontal;
                
                switch (i)
                {
                    case 1:
                        {
                            peType = PaintElementType.Gradient;
                            peStyle = GradientStyle.ForwardDiagonal;
                            color = Color.Yellow;
                            stopColor = Color.Goldenrod;
                         
                            break;
                        }
                    case 2:
                        {
                            peType = PaintElementType.Gradient;
                            peStyle = GradientStyle.ForwardDiagonal;
                            color = Color.Red;
                            stopColor = Color.DarkRed;
                           
                            break;
                        }
                    case 3:
                        {
                            peType = PaintElementType.Gradient;
                            peStyle = GradientStyle.ForwardDiagonal;
                            color = Color.LimeGreen;
                            stopColor = Color.ForestGreen;
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
           
            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Extent = 40;
            UltraChart2.Axis.Y.Extent = 100;

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> \n <SERIES_LABEL> \n <DATA_VALUE:N0>";
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart2.Axis.Y.Labels.Font = font;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = font;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Data.SwapRowsAndColumns = true;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "Тыс.руб.";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 10;
            UltraChart2.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart2.Width.Value / 3);
            
            UltraChart2.Legend.Font = font;
          

            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
         
            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

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
                        if (box.Series != null)
                        {
                            box.DataPoint.Label = RegionsNamingHelper.FullName(box.DataPoint.Label);
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
               FillComboFinance();
               ComboFin.Title = "Доходы";
               ComboFin.Visible = true;
               ComboFin.Width = 325;
               ComboFin.SetСheckedState("Федеральные налоги и сборы", true);
               ComboFin.ParentSelect = true;

               dtDate = new DataTable();
               query = DataProvider.GetQueryText("FNS_0004_0001_date");
               DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
              // endYear = Convert.ToInt32(dtDate.Rows[0][0]);

               ComboYear.Title = "Год";
               ComboYear.Width = 100;
               ComboYear.MultiSelect = false;
               ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
               ComboYear.SetСheckedState(endYear.ToString(), true);

               ComboMonth.Title = "Месяц";
               ComboMonth.Width = 130;
               ComboMonth.MultiSelect = false;
               ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
               ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

               ComboFO.Title = "ФО";
               ComboFO.Width = 300;
               ComboFO.MultiSelect = false;
               ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
               ComboFO.SetСheckedState("Все федеральные округа", true);

               UserParams.Subject.Value = string.Empty;

               if (!string.IsNullOrEmpty(UserParams.Region.Value))
               {
                   ComboFO.SetСheckedState(UserParams.Region.Value, true);
               }
               else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
               {
                   ComboFO.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
               }

               chart2SubTitle.Text = string.Empty;
           }

           SelectYear.Value = ComboYear.SelectedValue;
      
           UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
           UserParams.PeriodHalfYear.Value =
               string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
           UserParams.PeriodQuater.Value =
               string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

           DebtKindButtonList.Width = 1200;

           Page.Title = "Оценка потерь бюджетной системы от недополученных налоговых доходов";
           Label1.Text = Page.Title;

           dtDate = new DataTable();
           query = DataProvider.GetQueryText("FNS_0004_0001_CurrentMonth");
           DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
        
           if (ComboFO.SelectedIndex == 0)
           {

               if ((CRHelper.MonthNum(ComboMonth.SelectedValue) + 1) < 10)
               {
                   chart1ElementCaption.Text =
                       string.Format(
                           "Прирост задолженности по уплате налоговых обязательств на 01.0{0}.{1} г. с начала года в разрезе федеральных округов",
                           (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1), ComboYear.SelectedValue);

               }
               else
               {
                   if (CRHelper.MonthNum(ComboMonth.SelectedValue) == 12)
                   {
                       chart1ElementCaption.Text =
                           string.Format(
                               "Прирост задолженности по уплате налоговых обязательств на 01.01.{0} г. с начала года в разрезе федеральных округов",
                               Convert.ToInt32(ComboYear.SelectedValue) + 1);

                   }
                   else
                   {


                       chart1ElementCaption.Text =
                           string.Format(
                               "Прирост задолженности по уплате налоговых обязательств на 01.{0}.{1} г. с начала года в разрезе федеральных округов",
                               (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1), ComboYear.SelectedValue);

                   }
               }
           }
           else
           {
               if ((CRHelper.MonthNum(ComboMonth.SelectedValue) + 1) < 10)
               {
                   chart1ElementCaption.Text =
                       string.Format(
                           "Прирост задолженности по уплате налоговых обязательств на 01.0{0}.{1} г. с начала года в разрезе субъектов {2}",
                           (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1), ComboYear.SelectedValue, RegionsNamingHelper.ShortName(ComboFO.SelectedValue));

               }
               else
               {
                   if (CRHelper.MonthNum(ComboMonth.SelectedValue) == 12)
                   {
                       chart1ElementCaption.Text =
                           string.Format(
                               "Прирост задолженности по уплате налоговых обязательств на 01.01.{0} г. с начала года в разрезе субъектов {1}",
                               Convert.ToInt32(ComboYear.SelectedValue) + 1, RegionsNamingHelper.ShortName(ComboFO.SelectedValue));

                   }
                   else
                   {


                       chart1ElementCaption.Text =
                           string.Format(
                               "Прирост задолженности по уплате налоговых обязательств на 01.{0}.{1} г. с начала года в разрезе субъектов {2}",
                               (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1), ComboYear.SelectedValue,RegionsNamingHelper.ShortName(ComboFO.SelectedValue));

                   }
               }
           }

           switch (ComboFin.SelectedValue)
            {
                case "Федеральные налоги и сборы":
                    {
                        SelectFin.Value = ComboFin.SelectedValue;
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }

                case "Налог на прибыль организаций":
                    {
                        SelectFin.Value = string.Format("Федеральные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }
                case "Налог на добавленную стоимость":
                    {
                        SelectFin.Value = string.Format("Федеральные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }
                case "Платежи за пользование природными ресурсами":
                    {
                        SelectFin.Value = string.Format("Федеральные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }
                case "Остальные федеральные налоги и сборы":
                    {
                        SelectFin.Value = string.Format("Федеральные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }
                case "Единый социальный налог":
                    {
                        SelectFin.Value = string.Format("Федеральные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Slicer4]";
                        
                        break;
                    }
                case "Платежи в ГВФ":
                    {
                        SelectFin.Value = "Федеральные налоги и сборы].[Платежи в Государственные внебюджетные фонды";
                        UserParams.Filter.Value = "[Slicer5]";
                        break;
                    }
                case "Страховые взносы на обязательное ПС":
                    {
                        SelectFin.Value = "Федеральные налоги и сборы].[Страховые взносы на обязательное пенсионное страхование в РФ, зачисляемые в пенсионный фонд РФ";
                        UserParams.Filter.Value = "[Slicer6]";
                        break;
                    }

                case "Региональные налоги и сборы":
                    {
                        SelectFin.Value = ComboFin.SelectedValue;
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }
                case "Налог на имущество организаций":
                    {
                        SelectFin.Value = string.Format("Региональные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Все показатели].[Раздел II.I. СПРАВОЧНО]";
                        break;
                    }
                case "Транспортный налог":
                    {
                        SelectFin.Value = string.Format("Региональные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Все показатели].[Раздел II.I. СПРАВОЧНО]";
                        break;
                    }

                case "Местные налоги и сборы":
                    {
                        SelectFin.Value = ComboFin.SelectedValue;
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }
                case "Земельный налог":
                    {
                        SelectFin.Value = string.Format("Местные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Все показатели].[Раздел II.I. СПРАВОЧНО]";
                        break;
                    }

                case "Налог на имущество физических лиц":
                    {
                        SelectFin.Value = string.Format("Местные налоги и сборы].[{0}", ComboFin.SelectedValue);
                        UserParams.Filter.Value = "[Все показатели].[Раздел II.I. СПРАВОЧНО]";
                        
                        break;
                    }

                case "Налоги со специальным налоговым режимом":
                    {
                        SelectFin.Value = ComboFin.SelectedValue;
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }
                default:
                    {
                        SelectFin.Value = ComboFin.SelectedValue;
                        UserParams.Filter.Value = "[Slicer]";
                        break;
                    }

            }
           
            LastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) -1).ToString();
          
            selectedFO.Value = RFSelected ? " " : string.Format(".[{0}]", ComboFO.SelectedValue);

            if (!Page.IsPostBack || !(UserParams.Filter.ValueIs(UserParams.Filter.Value)))
            {
                UserParams.Filter.Value = "[Slicer]";
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UserParams.Subject.Value = "]";

            chart2ElementCaption.Text = string.Format("Помесячная динамика задолженности по налоговым обязательствам за {0} год", ComboYear.SelectedValue);
            UltraChart1.DataBind();
            UltraChart2.DataBind();

          }

        private void FillComboFinance()
        {
            dtFinance = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0004_0001_Finance");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"Доходы",dtFinance);
          
            Dictionary<string, int> regions = new Dictionary<string, int>();
            regions.Add("Федеральные налоги и сборы", 0);
            regions.Add("Налог на прибыль организаций", 1);
            regions.Add("Налог на добавленную стоимость", 1);
            regions.Add("Платежи за пользование природными ресурсами", 1);
            regions.Add("Остальные федеральные налоги и сборы", 1);
            regions.Add("Единый социальный налог", 1);
            regions.Add("Платежи в ГВФ", 1);
            regions.Add("Страховые взносы на обязательное ПС", 1);

            regions.Add("Региональные налоги и сборы", 0);
            regions.Add("Налог на имущество организаций", 1);
            regions.Add("Транспортный налог", 1);

            regions.Add("Местные налоги и сборы", 0);
            regions.Add("Земельный налог", 1);
            regions.Add("Налог на имущество физических лиц", 1);

            regions.Add("Налоги со специальным налоговым режимом", 0);

          ComboFin.FillDictionaryValues(regions);
        }

        #region Обработчики грида

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            chart2ElementCaption.Text = string.Format("Помесячная динамика задолженности по налоговым обязательствам за {0} год", ComboYear.SelectedValue);

            if (RegionsNamingHelper.IsRF(subject))
            {
                UserParams.Subject.Value = "]";
                UserParams.SubjectFO.Value = "]";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                UserParams.Region.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}]", UserParams.Region.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }
            else
            {
                UserParams.Region.Value = RegionsNamingHelper.FullName(row.Cells[1].Text);
                UserParams.StateArea.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }

           UltraChart2.DataBind();
        }

       
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FNS_0004_0001_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);
        
             if (dtGrid.Columns.Count > 3)

            {
                if (DebtKindButtonList.SelectedIndex == 0) // Выбрана ставка софинансирования на отчетную дату
                {
                    query = DataProvider.GetQueryText(string.Format("FNS_0004_0001_percent_{0}", ComboYear.SelectedValue));
                    DataTable dtPercent = new DataTable();
                    DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Процент", dtPercent);
                   
                    for (int i=1; i<dtPercent.Columns.Count;i++)
                    {
                        string[] caption = dtPercent.Columns[i].ColumnName.Split(';');
                       
                        if (caption[0] == ComboMonth.SelectedValue)
                        {
                           for (int j=0; j<dtGrid.Rows.Count; j++)
                            {
                                st = Convert.ToDouble(dtPercent.Rows[0][i]) * 100;
                                if (dtGrid.Rows[j][6] != DBNull.Value && dtGrid.Rows[j][6].ToString() != string.Empty)
                                {
                                    dtGrid.Rows[j][6] = Convert.ToDouble(dtGrid.Rows[j][6])/12 * Convert.ToDouble(dtPercent.Rows[0][i]) * CRHelper.MonthNum(ComboMonth.SelectedValue);
                                    st = Convert.ToDouble(dtPercent.Rows[0][i]) * 100;
                                   
                                }
                                
                            }
                        }
                    }

                    dtGrid.AcceptChanges();
                    
                }
                else // выбрана средняя хронометрическая ставка
                {
                    query = DataProvider.GetQueryText(string.Format("FNS_0004_0001_SrPercent_{0}", ComboYear.SelectedValue));
                    DataTable dtSrPercent = new DataTable();
                    DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Процент", dtSrPercent);


                    for (int i = 1; i < dtSrPercent.Columns.Count; i++)
                    {
                        string[] caption = dtSrPercent.Columns[i].ColumnName.Split(';');

                        if (caption[0] == ComboMonth.SelectedValue)
                        {
                            for (int j = 0; j < dtGrid.Rows.Count; j++)
                            {
                                st = Convert.ToDouble(dtSrPercent.Rows[0][i]);
                                if (dtGrid.Rows[j][6] != DBNull.Value && dtGrid.Rows[j][6].ToString() != string.Empty)
                                {
                                    dtGrid.Rows[j][6] = Convert.ToDouble(dtGrid.Rows[j][6]) / 12 * Convert.ToDouble(dtSrPercent.Rows[0][i]) * CRHelper.MonthNum(ComboMonth.SelectedValue)/100;
                                    st = Convert.ToDouble(dtSrPercent.Rows[0][i]);
                                }

                            }
                        }
                    }

                   /* if (ComboYear.SelectedIndex == 0)
                    {
                        for (int i = 0; i < dtGrid.Rows.Count; i++)
                        { if (dtGrid.Rows[i][6] != DBNull.Value &&
                                        dtGrid.Rows[i][6].ToString() != string.Empty)
                            {
                            dtGrid.Rows[i][6] = Convert.ToDouble(dtGrid.Rows[i][6])* (CRHelper.MonthNum(month))/12 * 0.1298;
                            st = 0.1298 * 100; 
                            }
                        }
                    }
                    else if (ComboYear.SelectedIndex == 1)
                    {
                        for (int i = 0; i < dtGrid.Rows.Count; i++)
                        {
                            if (dtGrid.Rows[i][6] != DBNull.Value &&
                                       dtGrid.Rows[i][6].ToString() != string.Empty)
                            {
                                dtGrid.Rows[i][6] = Convert.ToDouble(dtGrid.Rows[i][6]) * (CRHelper.MonthNum(month)) / 12 * 0.1164;
                                st = 0.1164*100;
                            }
                        }
                    }
                    else if (ComboYear.SelectedIndex == 2)
                    {
                        for (int i = 0; i < dtGrid.Rows.Count; i++)
                        {
                            if (dtGrid.Rows[i][6] != DBNull.Value &&
                                       dtGrid.Rows[i][6].ToString() != string.Empty)
                            {
                                dtGrid.Rows[i][6] = Convert.ToDouble(dtGrid.Rows[i][6]) * (CRHelper.MonthNum(month)) / 12 * 0.1026;
                                st = 0.1026*100;
                            }
                        }
                    }
                    else if (ComboYear.SelectedIndex == 3)
                    {
                        for (int i = 0; i < dtGrid.Rows.Count; i++)
                        {
                            if (dtGrid.Rows[i][6] != DBNull.Value &&
                                       dtGrid.Rows[i][6].ToString() != string.Empty)
                            {
                                dtGrid.Rows[i][6] = Convert.ToDouble(dtGrid.Rows[i][6]) * (CRHelper.MonthNum(month)) / 12 * 0.1087;
                                st = 0.1087*100;
                            }
                        }
                    }
                    else if (ComboYear.SelectedIndex == 4)
                    {
                        for (int i = 0; i < dtGrid.Rows.Count; i++)
                        {
                            if (dtGrid.Rows[i][6] != DBNull.Value &&
                                        dtGrid.Rows[i][6].ToString() != string.Empty)
                            {
                                dtGrid.Rows[i][6] = Convert.ToDouble(dtGrid.Rows[i][6]) * (CRHelper.MonthNum(month)) / 12 * 0.1137;
                                st = 0.1137*100;
                            }
                        }
                    }
                    else if (ComboYear.SelectedIndex == 5)
                    {
                        query =
                            DataProvider.GetQueryText(string.Format("FNS_0004_0001_SrPercent_{0}",
                                                                    ComboYear.SelectedValue));
                        DataTable dtSrPercent = new DataTable();
                        DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "СреднийПроцент",
                                                                                           dtSrPercent);

                        for (int i = 1; i < dtSrPercent.Columns.Count; i++)
                        {
                            string[] caption = dtSrPercent.Columns[i].ColumnName.Split(';');

                            if (caption[0] == month)
                            {
                                for (int j = 0; j < dtGrid.Rows.Count; j++)
                                {
                                    if (dtGrid.Rows[j][6] != DBNull.Value &&
                                        dtGrid.Rows[j][6].ToString() != string.Empty)
                                    {
                                        dtGrid.Rows[j][6] = Convert.ToDouble(dtGrid.Rows[j][6])/12*
                                                            Convert.ToDouble(dtSrPercent.Rows[0][i])*
                                                            CRHelper.MonthNum(month)/100;
                                        st = Convert.ToDouble(dtSrPercent.Rows[0][i]);
                                    }
                                }
                            }
                        }
                    }
                    */

                    dtGrid.AcceptChanges();
                  
                }

              }

              if ((CRHelper.MonthNum(ComboMonth.SelectedValue)+1) < 10)
              {
                  if (DebtKindButtonList.SelectedIndex == 0)
                  {
                      Label2.Text = string.Format("{2}, данные по {0} по состоянию на 01.0{1}.{3}, ставка рефинансирования {4}%", RFSelected ? "Российской Федерации" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboFin.SelectedValue, ComboYear.SelectedValue, st);
                  }
                  else
                  {
                      Label2.Text = string.Format("{2}, данные по {0} по состоянию на 01.0{1}.{3}, средняя хронометрическая ставка {4}%", RFSelected ? "Российской Федерации" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboFin.SelectedValue, ComboYear.SelectedValue, st);
                  }
              }
              else
              {
                  if (CRHelper.MonthNum(ComboMonth.SelectedValue) == 12)
                  {
                      if (DebtKindButtonList.SelectedIndex == 0)
                      {
                          Label2.Text = string.Format("{2}, данные по {0} по состоянию на 01.01.{3},ставка рефинансирования {4}%", RFSelected ? "Российской Федерации" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), ComboFin.SelectedValue,Convert.ToInt32(ComboYear.SelectedValue)+1, st);
                      }
                      else
                      {
                          Label2.Text = string.Format("{2}, данные по {0} по состоянию на 01.01.{3},средняя хронометрическая ставка {4}%", RFSelected ? "Российской Федерации" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), ComboFin.SelectedValue, Convert.ToInt32(ComboYear.SelectedValue)+1, st);
                      }
                  }
                  else
                  {
                      if (DebtKindButtonList.SelectedIndex == 0)
                      {
                          Label2.Text = string.Format("{2}, данные по {0} по состоянию на 01.{1}.{3},ставка рефинансирования {4}%", RFSelected ? "Российской Федерации" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboFin.SelectedValue, ComboYear.SelectedValue, st);
                      }
                      else
                      {
                          Label2.Text = string.Format("{2}, данные по {0} по состоянию на 01.{1}.{3},средняя хронометрическая ставка {4}%", RFSelected ? "Российской Федерации" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboFin.SelectedValue, ComboYear.SelectedValue, st);
                      }
                  }
              }
            
        
            foreach (DataRow row in dtGrid.Rows)
                {
                 for (int i = 2; i < dtGrid.Columns.Count; i++)
                 {
                     if (i == 2 || i == 3 || i==4 ||i == 6)
                     {
                         if (row[i] != DBNull.Value)
                         {
                             row[i] = Convert.ToDouble(row[i]) / 1000;
                         }
                     }
                 }
                }
          
            if (dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns[1].ColumnName = "ФО";
            }
         
            if (dtGrid.Rows.Count > 2)
            {
                UltraWebGrid.DataSource = dtGrid;
               
                  for (int j = 0; j < dtGrid.Rows.Count; j++)
                    {
                        if (RegionsNamingHelper.IsSubject(dtGrid.Rows[j][0].ToString()))// если это субъект
                        {
                            if (dtGrid.Rows[j][6] != DBNull.Value)
                            {

                                if (Convert.ToDouble(dtGrid.Rows[j][6]) >= max)
                                {
                                    max = Convert.ToDouble(dtGrid.Rows[j][6]);
                                }
                                if (Convert.ToDouble(dtGrid.Rows[j][6]) < min)
                                {
                                    min = Convert.ToDouble(dtGrid.Rows[j][6]);
                                }
                            }
                        }
                    }
                    MAX.Add(max);
                    MIN.Add(min);
      
            }
        }
        
        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 2; i < UltraWebGrid.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(40);

            DateTime currentDate = ComboMonth.SelectedValue == "Декабрь" ? new DateTime(Convert.ToInt32(ComboYear.SelectedValue)+1, 1, 1) : new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, 1);
            headerLayout.AddCell("Субъект", "Субъект РФ");
            headerLayout.AddCell("ФО", "Федеральный округ");
            headerLayout.AddCell(string.Format("Сумма задолженности на 01.01.{0},<br/> тыс. руб.", ComboYear.SelectedValue), "Сумма задолженности по налогам и сборам, пеням и налоговым санкциям на начало года, тыс.руб.");
            headerLayout.AddCell(string.Format("Сумма задолженности на {0:dd.MM.yyyy},<br/> тыс. руб.", currentDate),"Сумма задолженности по налогам и сборам, пеням и налоговым санкциям на конец отчетного периода, тыс.руб.");
            headerLayout.AddCell(string.Format("Прирост задолженности,<br/> тыс. руб."), "Прирост задолженности по налогам и сборам, пеням и налоговым санкциям с начала года, тыс. руб.");
            headerLayout.AddCell(string.Format("Прирост задолженности,<br/> %"), "Прирост задолженности с начала года в процентах");
            headerLayout.AddCell(string.Format("Сумма потерь от недополученных доходов,<br/> тыс. руб."), "Сумма потерь от несвоевременного получения доходов нарастающим итогом с начала года, тыс.руб.");
            headerLayout.ApplyHeaderInfo();
          }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int k = 0;
                   for (int i = 2; i < UltraWebGrid.Columns.Count; i ++)
                    {
                        if (i == 5)
                        {
                            if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                            {
                               double value = Convert.ToDouble(e.Row.Cells[i].Value);
                                
                                if ( value > 0)
                                {

                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                    e.Row.Cells[i].Title = "Прирост задолженности с начала года";
                                    e.Row.Cells[i].Style.CustomRules =
                                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                                else if (value <= 0)
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                    e.Row.Cells[i].Title = "Снижение задолженности с начала года";
                                    e.Row.Cells[i].Style.CustomRules =
                                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                            }

                        }

                        if (i == 6)
                        {
                            if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                            {
                                if (Convert.ToDouble(e.Row.Cells[i].Value) == MIN[k])
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    e.Row.Cells[i].Title = "Самая маленькая сумма потерь";
                                }
                                else if (Convert.ToDouble(e.Row.Cells[i].Value) == MAX[k])
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    e.Row.Cells[i].Title = "Самая большая сумма потерь";

                                }
                                e.Row.Cells[i].Style.CustomRules =
                                    "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            }
                            k++;
                        }
                      
                    }
           foreach (UltraGridCell cell in e.Row.Cells)
                {
                    if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                    {
                        if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                        {
                            cell.Style.Font.Bold = true;
                        }
                    }
                }


                for (int i = 4; i < 6; i++)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        decimal value;
                        if (decimal.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                        {
                            if (value > 0)
                            {
                                e.Row.Cells[i].Style.ForeColor = Color.Red;
                            }
                        }
                    }
                }
            if (e.Row.Cells[3].Value == null)
            {
                e.Row.Cells[3].Value = "Нет данных";
            }
        }
   #endregion 

        #region Обработчики диаграмм
        protected void UltraChart1_DataBinding(Object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex == 0)
            {
                query = DataProvider.GetQueryText("FNS_0004_0001_Chart1_AllFO");
                dtChart1 = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart1);
                RegionsNamingHelper.ReplaceRegionNames(dtChart1, 0);
                
            }
            else
            {

                query = DataProvider.GetQueryText("FNS_0004_0001_Chart1_CurrentFO");
                dtChart1 = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart1);
                RegionsNamingHelper.ReplaceRegionNames(dtChart1, 0);
                UltraChart1.Data.SwapRowsAndColumns = true;
            }

            UltraChart1.Data.SwapRowsAndColumns = true;

            UltraChart1.Series.Clear();
            for (int i = 1; i < dtChart1.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                series.Label = dtChart1.Columns[i].ColumnName;
                UltraChart1.Series.Add(series);
            }
            //  UltraChart1.DataSource = dtChart1;
        }

        protected void UltraChart2_DataBinding(Object sender, EventArgs e)
        {
            query = DataProvider.GetQueryText("FNS_0004_0001_Chart2");
            dtChart2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма2", dtChart2);
           
          /*  if (DebtKindButtonList.SelectedIndex == 0) // Выбрана ставка софинансирования на отчетную дату
            {
                query = DataProvider.GetQueryText(string.Format("FNS_0004_0001_percent_{0}", ComboYear.SelectedValue));
                DataTable dtPercent = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Процент", dtPercent);

                for (int i = 1; i < dtChart2.Columns.Count; i++)
                {
                    string[] caption = dtChart2.Columns[i].ColumnName.Split(';');
                                        
                    for (int j = 1; j < dtPercent.Columns.Count; j++)
                    {
                        string[] capmonth = dtPercent.Columns[j].ColumnName.Split(';');
                        if (caption[0] == capmonth[0])
                        {
                            if (dtChart2.Rows[0][i] != DBNull.Value && dtChart2.Rows[0][i].ToString() != string.Empty)
                            {        
                                double percent = Convert.ToDouble(dtPercent.Rows[0][j]);
                                double value = Convert.ToDouble(dtChart2.Rows[0][i]);
                                dtChart2.Rows[0][i] = Convert.ToDouble(percent * value);
                               
                            }
                        }
                    }
                }

            }
            else
            {
                query =DataProvider.GetQueryText(string.Format("FNS_0004_0001_SrPercent_{0}",
                                                            ComboYear.SelectedValue));
                DataTable dtSrPercent = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "СреднийПроцент",
                                                                                   dtSrPercent);

                for (int i = 1; i < dtChart2.Columns.Count; i++)
                {
                    string[] caption = dtChart2.Columns[i].ColumnName.Split(';');
                    for (int j = 1; j < dtSrPercent.Columns.Count; j++)
                    {
                        string[] capmonth = dtSrPercent.Columns[j].ColumnName.Split(';');
                        if (caption[0] == capmonth[0])
                        {
                            if (dtChart2.Rows[0][i] != DBNull.Value && dtChart2.Rows[0][i].ToString() != string.Empty)
                            {
                                double percent = Convert.ToDouble(dtSrPercent.Rows[0][j]);
                                double value = Convert.ToDouble(dtChart2.Rows[0][i]);
                                dtChart2.Rows[0][i] = Convert.ToDouble(percent * value)/100;
                               
                            }
                        }
                    }
                }

            }*/

            
          
            for (int i=1; i<dtChart2.Columns.Count;i++)
            {
                string[] caption = dtChart2.Columns[i].Caption.Split(';');
                int number = CRHelper.MonthNum(caption[0]);
                if ((number+1)>=10)
                {
                    if ((number + 1) == 13)
                    {
                        dtChart2.Columns[i].ColumnName = string.Format("На 01.01.{0}",Convert.ToInt32(ComboYear.SelectedValue)+1);
                    }
                    else
                    {
                        dtChart2.Columns[i].ColumnName = string.Format("На 01.{0}.{1}", number + 1,ComboYear.SelectedValue);
                    }
                    
                }
                else
                {
                    dtChart2.Columns[i].ColumnName = string.Format("На 01.0{0}.{1}", number+1, ComboYear.SelectedValue);
                }

            }
            UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL> \n <SERIES_LABEL> \n {0} \n <DATA_VALUE:N2> тыс.руб", ComboFin.SelectedValue);
            dtChart2.AcceptChanges();
            UltraChart2.DataSource = dtChart2;

        }
        #endregion


        #region Экспорт

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

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
            ReportPDFExporter1.PageTitle = Label1.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout,Label2.Text, section1);
            UltraChart1.Legend.Margins.Right = 5;
            UltraChart2.Legend.Margins.Right = 5;
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chart1ElementCaption.Text);
            UltraChart1.Width = 1000;
            ReportPDFExporter1.Export(UltraChart1, Label2.Text, section2);
            title = section3.AddText();
            font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chart2ElementCaption.Text);
            UltraChart2.Width = 1000;
            ReportPDFExporter1.Export(UltraChart2, Label2.Text, section3);
        }

        #endregion

        #endregion
    }
}
