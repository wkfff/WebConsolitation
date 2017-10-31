using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
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


namespace Krista.FM.Server.Dashboards.reports.FO_0004_0001
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
  
          
        private int firstYear = 2008;
        private string month;
        private DateTime currentDate;

        public CustomParam Measures;
        public CustomParam Budget;
        public CustomParam VariantYear;
        public CustomParam VariantLastYear;

        private GridHeaderLayout headerLayout;
        private static MemberAttributesDigest budDigest;
        

      #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -20);
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth /2-11);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth/2-11);
            UltraChart2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);


           #region инициализация параметров
             

             if (Measures == null)
             {
                 Measures = UserParams.CustomParam("measures");
             }
             if (Budget == null)
             {
                 Budget = UserParams.CustomParam("budget");
             }
            if (VariantYear == null)
            {
                VariantYear = UserParams.CustomParam("variant");
            }
            if (VariantLastYear == null)
            {
                VariantLastYear = UserParams.CustomParam("variant_last");
            }

            #endregion

            #region  Настройка диаграмм
             UltraChart1.ChartType = ChartType.PieChart; // круговая диаграмма
             UltraChart1.Border.Thickness = 0;
             UltraChart1.Axis.X.Extent = 130;
             UltraChart1.Axis.Y.Extent = 130;
             UltraChart1.Legend.Visible = true;
             UltraChart1.Legend.Location = LegendLocation.Bottom;
             UltraChart1.Legend.Margins.Right = 0;
             System.Drawing.Font font = new System.Drawing.Font("Verdana", 10);
//           UltraChart1.Legend.Font = font;
             UltraChart1.PieChart.Labels.Font =  font;
             UltraChart1.Legend.SpanPercentage = 25;
             UltraChart1.Data.SwapRowsAndColumns = false;
             UltraChart1.PieChart.OthersCategoryPercent = 0;
             UltraChart1.PieChart.StartAngle = 130;

             UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
             UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

             UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
             UltraChart1.ColorModel.Skin.ApplyRowWise = true;
             UltraChart1.ColorModel.Skin.PEs.Clear();

             for (int i = 1; i <= 6; i++)
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
                             color = Color.Maroon;
                             color = Color.Red;
                             break;
                         }
                     case 2:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.YellowGreen;
                             stopColor = Color.GreenYellow;
                             break;
                         }
                     case 3:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.YellowGreen;
                             stopColor = Color.Yellow;
                             break;
                         }
                     case 4:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.White;
                             stopColor = Color.Blue;
                             break;
                         }
                     case 5:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.Gold;
                             stopColor = Color.DarkOrange;
                             break;
                         }
                     case 6:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.GreenYellow;
                             stopColor = Color.Green;
                             break;
                         }

                 }
                 pe.Fill = color;
                 pe.FillStopColor = stopColor;
                 pe.ElementType = peType;
                 pe.FillOpacity = 180;
                 pe.FillStopOpacity = 250;
                 pe.FillGradientStyle = peStyle;
                 UltraChart1.ColorModel.Skin.PEs.Add(pe);
             }
            
             UltraChart2.ChartType = ChartType.StackColumnChart;
             UltraChart2.Border.Thickness = 0;
             UltraChart2.Axis.X.Extent = 10;
             UltraChart2.Axis.Y.Extent = 30;
             UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
             UltraChart2.Axis.X.Labels.Visible = false;
             UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
             UltraChart2.Axis.Y.Labels.Font = font;
             UltraChart2.Axis.X.Labels.SeriesLabels.Visible = false;
             UltraChart2.Data.SwapRowsAndColumns = true;
             UltraChart2.Legend.Visible = true;
             UltraChart2.Legend.Location = LegendLocation.Right;
             UltraChart2.Legend.SpanPercentage = 50;
           //  UltraChart2.Legend.Font = font;
             UltraChart2.Legend.Margins.Bottom = Convert.ToInt32(UltraChart2.Width.Value / 2);

             UltraChart2.TitleLeft.Visible = true;
             UltraChart2.TitleLeft.Text = "%";
             UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;

             UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
             UltraChart2.ColorModel.ModelStyle = ColorModels.LinearRange;
             UltraChart2.ColorModel.ColorBegin = Color.DarkOrange;
             UltraChart2.ColorModel.ColorEnd = Color.Gold;
             
            /* UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
             UltraChart2.ColorModel.Skin.ApplyRowWise = false;
             UltraChart2.ColorModel.Skin.PEs.Clear();
             for (int i = 1; i <= 4; i++)
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
                             color = Color.Orange;
                             stopColor = Color.OrangeRed;
                             pe.FillOpacity = 100;
                           

                             break;
                         }
                     case 2:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.Orange;
                             stopColor = Color.Orange;
                             pe.FillOpacity = 150;
                             pe.FillStopOpacity = 250;

                             break;
                         }
                     case 3:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.Orange;
                             stopColor = Color.Orange;
                             pe.FillOpacity = 200;
                             pe.FillStopOpacity = 250;
                             break;
                         }

                     case 4:
                         {
                             peType = PaintElementType.Gradient;
                             peStyle = GradientStyle.ForwardDiagonal;
                             color = Color.White;
                             stopColor = Color.Orange;
                             pe.FillOpacity = 100;
                             pe.FillStopOpacity = 200;
                             break;
                         }

                 }
                 pe.Fill = color;
                 pe.FillStopColor = stopColor;
                 pe.ElementType = peType;
                 pe.FillStopOpacity = 250;
                 pe.FillGradientStyle = peStyle;
                 UltraChart2.ColorModel.Skin.PEs.Add(pe);
             }
         */
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
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
    
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0004_0001_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.MultiSelect = false;
                ComboMonth.SetСheckedState(month,true);

                ComboBudget.Title = "Бюджет";
                ComboBudget.Width = 350;
                budDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "SaratovListBudget");
                ComboBudget.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budDigest.UniqueNames, budDigest.MemberLevels));
                ComboBudget.MultiSelect = false;
                ComboBudget.SetСheckedState("Консолидированнный бюджет субъекта",true);

                Dictionary<string, int> variant = new Dictionary<string, int>();
                variant.Add("Основной",0);
                variant.Add("Дополнительный", 0);
                variant.Add("К первому чтению", 0);
                variant.Add("Ко второмй чтению", 0);
                variant.Add("К третьему чтению", 0);
                variant.Add("Уточненный на первый квартал", 0);
                variant.Add("Уточненный на первое полугодие", 0);
                variant.Add("Уточненный на девять месяцев", 0);
               
                ComboVariantYear.Title = "Вариант расходов тек.год.";
                ComboVariantYear.Visible = false;
                ComboVariantYear.Width = 200;
                ComboVariantYear.FillDictionaryValues(variant);
                ComboVariantYear.MultiSelect = false;
                ComboVariantYear.SetСheckedState("Основной",true);

                ComboVariantLastYear.Title = "Вариант расходов пред.год.";
                ComboVariantLastYear.Visible = false;
                ComboVariantLastYear.Width = 200;
                ComboVariantLastYear.FillDictionaryValues(variant);
                ComboVariantLastYear.MultiSelect = false;
                ComboVariantLastYear.SetСheckedState("Основной", true);
                
            }

            Page.Title = "Структура расходов бюджетов";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;
            Measures.Value = "[Measures].[Факт]";
            Budget.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Конс.бюджет субъекта]";
            string subTitle = string.Empty;
            
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);
           
            if (DebtKindButtonList1.SelectedIndex == 0) // факт
            {
                Measures.Value = "[Measures].[Факт]";
                ComboVariantLastYear.Visible = false;
                ComboVariantYear.Visible = false;
                subTitle = "фактические данные";
                PageSubTitle.Text = string.Format("Приводятся {0} на {1:dd.MM.yyyy} г., {2}", subTitle, currentDate, ComboBudget.SelectedValue.ToLower());

            }
            else
            {
                if (DebtKindButtonList1.SelectedIndex == 1) // план
                {    ComboVariantLastYear.Visible = false;
                     ComboVariantYear.Visible = false;
                     subTitle = "годовые назначения";
                     PageSubTitle.Text = string.Format("Приводятся {0} на {1:dd.MM.yyyy} г., {2}", subTitle, currentDate, ComboBudget.SelectedValue.ToLower());
                     Measures.Value = " [Measures].[Годовые назначения]";
                }
                else
                {
                    if (DebtKindButtonList1.SelectedIndex == 2) // проект бюджета
                    {
                        ComboVariantLastYear.Visible = true;
                        ComboVariantYear.Visible = true;
                        Measures.Value = "[Measures].[Прогноз]";
                        subTitle = "проектные данные";
                        PageSubTitle.Text = string.Format("Приводятся {0} на {1} г., {2}", subTitle, ComboYear.SelectedValue, ComboBudget.SelectedValue.ToLower());
                       
                        switch (ComboVariantYear.SelectedValue)
                        {
                            case "Основной":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Основной]";
                                    break;
                                }
                            case "Дополнительный":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Дополнительный]";
                                    break;
                                }
                            case "К первому чтению":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[К первому чтению]";
                                    break;
                                }
                            case "Ко второму чтению":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Ко второму чтению]";
                                    break;
                                }
                            case "К третьему чтению":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[К третьему чтению]";
                                    break;
                                }
                            case "Уточненный на первый квартал":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Уточненный на первый квартал]";
                                    break;
                                }
                            case "Уточненный на первое полугодие":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Уточненный на первое полугодие]";
                                    break;
                                }
                            case "Уточненный на девять месяцев":
                                {
                                    VariantYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Уточненный на девять месяцев]";
                                    break;
                                }
                        }
                        switch (ComboVariantLastYear.SelectedValue)
                        {
                            case "Основной":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Основной]";
                                    break;
                                }
                            case "Дополнительный":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Дополнительный]";
                                    break;
                                }
                            case "К первому чтению":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[К первому чтению]";
                                    break;
                                }
                            case "Ко второму чтению":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Ко второму чтению]";
                                    break;
                                }
                            case "К третьему чтению":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[К третьему чтению]";
                                    break;
                                }
                            case "Уточненный на первый квартал":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Уточненный на первый квартал]";
                                    break;
                                }
                            case "Уточненный на первое полугодие":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Уточненный на первое полугодие]";
                                    break;
                                }
                            case "Уточненный на девять месяцев":
                                {
                                    VariantLastYear.Value = "[Вариант__Проект расходов].[Вариант__Проект расходов].[Все варианты].[Уточненный на девять месяцев]";
                                    break;
                                }
                        }

                    }
                }
            }
          
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            Budget.Value = budDigest.GetMemberUniqueName(ComboBudget.SelectedValue);

            chart1ElementCaption.Text = "Структура расходов бюджетов";
            chart2ElementCaption.Text = "Структура расходов социальной сферы";
            
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
          
        }

        #region Обработчик грида

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            double value;
            if (DebtKindButtonList2.SelectedIndex == 0)
            {
                value = 1000;
            }
            else
            {
                value = 1000000;
            }

            if (DebtKindButtonList1.SelectedIndex == 0 || DebtKindButtonList1.SelectedIndex == 1)
            {
                string query = DataProvider.GetQueryText("FO_0004_0001_Grid_planfact");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "РзПр", dtGrid);
            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0004_0001_Grid_proect");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "РзПр", dtGrid); 
            }
                     

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                if (dtGrid.Rows[i][1] != DBNull.Value && dtGrid.Rows[i][1].ToString() != string.Empty)
                {
                    dtGrid.Rows[i][1] = Convert.ToDouble(dtGrid.Rows[i][1]) / value;
                }
                if (dtGrid.Rows[i][3] != DBNull.Value && dtGrid.Rows[i][3].ToString() != string.Empty)
                {
                    dtGrid.Rows[i][3] = Convert.ToDouble(dtGrid.Rows[i][3]) / value;
                }
            }

            dtGrid.AcceptChanges();
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(490);
            string edIzm;
            if (DebtKindButtonList2.SelectedIndex == 0)
            {
                edIzm = "тыс.руб.";
            }
            else
            {
                edIzm = "млн.руб.";
            }
            headerLayout.AddCell("Наименование расходов");

            if (DebtKindButtonList1.SelectedIndex == 0)
            {
                headerLayout.AddCell(string.Format("Исполнено, {0}", edIzm), "Кассовый расход нарастающим итогом с начала года");
                headerLayout.AddCell("Доля, %", "Доля расхода в общей сумме расходов бюджета");
                headerLayout.AddCell(string.Format("Исполнено (прошлый год), {0}", edIzm), "Кассовый расход за аналогичный период предыдущего года");
                headerLayout.AddCell("Темп роста, %", "Темп роста  фактических расходов к аналогичному периоду предыдущего года");

            }
            else if (DebtKindButtonList1.SelectedIndex == 1)
            {
                headerLayout.AddCell(string.Format("Бюджетные назначения {0}", edIzm), "Плановые назначения на текущий год");
                headerLayout.AddCell("Доля, %", "Доля расхода в общей сумме расходов бюджета");
                headerLayout.AddCell(string.Format("Бюджетные назначения (прошлый год), {0}", edIzm), "Плановые назначения на прошлый год");
                headerLayout.AddCell("Темп роста, %", "Темп роста  плановых расходов к  аналогичному периоду предыдущего года");
            }
            else
            {
                headerLayout.AddCell(string.Format("Проект {0}", edIzm), "Проект расходов на текущий год");
                headerLayout.AddCell("Доля, %", "Доля расхода в общей сумме расходов бюджета");
                headerLayout.AddCell(string.Format("Проект (прошлый год), {0}", edIzm), "Проект расходов прошлый год");
                headerLayout.AddCell("Темп роста, %", "Темп роста  расходов к  аналогичному периоду предыдущего года");
            }

            headerLayout.ApplyHeaderInfo();
         
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130);
                    if ((i % 2) == 0)
                    {
                       CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                    }
                    else
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                    }
                }

        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            int i = 4; // темп роста
                  if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                  {
                      if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                          e.Row.Cells[i].Title = "Рост расходов";
                      }
                      else if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                          e.Row.Cells[i].Title = "Снижение расходов";
                      }

                      e.Row.Cells[i].Style.CustomRules =
                          "background-repeat: no-repeat; background-position: left center; margin: 2px";
                  }
            
          }

        #endregion 

        #region Обработчики диаграмм 
         
          protected  void UltraChart1_DataBinding(Object sender, EventArgs e)
          {
              double value;
              if (DebtKindButtonList2.SelectedIndex == 0)
              {
                  value = 1000;
                  UltraChart1.Tooltips.FormatString = string.Format("Доля в общей сумме расходов бюджета \n <ITEM_LABEL>\n<PERCENT_VALUE:#0.00>%\n<DATA_VALUE:N2> тыс.руб.");
              }
              else
              {
                  value = 1000000;
                  UltraChart1.Tooltips.FormatString = string.Format("Доля в общей сумме расходов бюджета \n <ITEM_LABEL>\n<PERCENT_VALUE:#0.00>%\n<DATA_VALUE:N2> млн.руб.");
              }
              if (DebtKindButtonList1.SelectedIndex == 0 || DebtKindButtonList1.SelectedIndex == 1)
              {
                  string query = DataProvider.GetQueryText("FO_0004_0001_Chart1_planfact");
                  dtChart1 = new DataTable();
                  DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart1);
              }
              else
              {
                  string query = DataProvider.GetQueryText("FO_0004_0001_Chart1_proect");
                  dtChart1 = new DataTable();
                  DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart1);
              }
              for (int i = 0; i < dtChart1.Rows.Count; i++)
              {
                  if (dtChart1.Rows[i][1] != DBNull.Value && dtChart1.Rows[i][1].ToString() != string.Empty)
                  {
                      dtChart1.Rows[i][1] = Convert.ToDouble(dtChart1.Rows[i][1]) / value;
                  }
                  
              }
             
              UltraChart1.DataSource = dtChart1;
          }


        protected void UltraChart2_DataBinding(Object sender, EventArgs e)
        {
           if (DebtKindButtonList1.SelectedIndex == 0 || DebtKindButtonList1.SelectedIndex == 1)
            {
                string query = DataProvider.GetQueryText("FO_0004_0001_Chart2_planfact");
                dtChart2 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма2", dtChart2);
               
            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0004_0001_Chart2_proect");
                dtChart2 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма2", dtChart2);
            }

            dtChart2.Rows[0][0] = "Социальная политика";
            dtChart2.Rows[1][0] = "Образование";
            dtChart2.Rows[2][0] = "Культура, кинематография, СМИ";
            dtChart2.Rows[3][0] = "Здравоохранение, спорт";

            UltraChart2.Tooltips.FormatString = string.Format("Доля в общей сумме расходов бюджета \n <SERIES_LABEL> \n <DATA_VALUE:P2>");
            UltraChart2.DataSource = dtChart2;

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
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
        
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart2, chart2ElementCaption.Text, sheet3, 3);
         
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
            ISection section3 = report.AddSection();
          
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
          
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart2, chart2ElementCaption.Text, section3);
          

        }


        #endregion

      #endregion


       }
}