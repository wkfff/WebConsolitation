using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0001_0006
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate;
        private DataTable dtFood;
        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable medianDT;
        private GridHeaderLayout headerLayout;
        private bool noMapData;

        #endregion

        // имя папки с картами региона
        private string mapFolderName;

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        private static string Browser
        {
            get { return HttpContext.Current.Request.Browser.Browser; }
        }


        #region Параметры запроса

        // Переданный из основного отчета товар
        private CustomParam Food;
        // выбранный товар
        private CustomParam selectedFood;

        private CustomParam selectedFoodRF;
        // выбранная дата
        private CustomParam selectedDate;

        private CustomParam selectedDateRF;

        // предыдущий месяц дата
        private CustomParam previousDate;

        private CustomParam previousDateRF;
        // начало года
        private CustomParam yearDate;

        private CustomParam yearDateRF;
        // те же, но в текстовом формате (для вывода на экран, чтобы не конвертировать)
        private static string selectedDateText;
        private static string previousDateText;
        private static string yearDateText;

        #endregion

        // Массивы с продуктами, mdx именами продуктов
        string[] food = { "Говядина", "Свинина",
                          "Баранина", "Куры", "Рыба мороженная неразделанная", "Масло сливочное", "Масло подсолнечное",
						  "Молоко питьевое цельное пастеризованное (2,5-3,2 % жирн.)","Молоко питьевое цельное стерилизованное (2,5-3,2 % жирн.)", "Яйца куриные", "Сахар-песок", "Чай черный байховый",
						  "Соль поваренная пищевая", "Мука пшеничная (в/с, 1 сорт)", "Хлеб ржаной, ржано-пшеничный",
						  "Хлеб и булочные изделия из муки пшеничной 1 и 2 сортов", "Рис шлифованный", "Пшено", "Крупа гречневая", "Вермишель", "Картофель",
						  "Капуста", "Лук репчатый", "Морковь", "Яблоки" };
        string[] foodMDX =
		{
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Говядина (кроме бескостного мяса)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Свинина (кроме бескостного мяса)]",
            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Баранина (кроме бескостного мяса)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Куры (кроме куринных окорочков)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Рыба].[Рыба мороженная неразделенная]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло сливочное]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло подсолнечное]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Молоко питьевое цельное пастеризованное (2,5-3,2 % жирн.)]",
            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Молоко питьевое цельное стерилизованное (2,5-3,2 % жирн.)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Яйца куриные]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Сахар-песок]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Чай, кофе].[Чай черный байховый]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Соль, пряности].[Соль поваренная пищевая]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Мука пшеничная (в/с, 1 сорт)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб ржаной, ржано-пшеничный]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб и булочные изделия из муки пшеничной 1 и 2 сортов]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Рис шлифованный]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Пшено]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Крупа гречневая - ядрица]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Вермишель]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Картофель]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Капуста белокочанная свежая]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Лук репчатый]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Морковь]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Фрукты].[Яблоки]"
		};

        // --------------------------------------------------------------------

        // заголовок страницы
        private const string PageTitleCaption = "Анализ средних розничных цен на отдельные виды социально значимых продовольственных товаров первой необходимости в разрезе муниципальных образований.";
        private const string PageSubTitleCaption = "Еженедельный мониторинг средних розничных цен на отдельные виды социально значимых продовольственных товаров первой необходимости в разрезе муниципальных образований, Ханты-Мансийский автономный округ – Югра, по состоянию на {0}.";
        private const string MapTitleCaption1 = "Средняя розничная цена на товар «{0}», рублей за {1}, по состоянию на {2}";
        private const string MapTitleCaption2 = "Изменение цены на товар «{0}», рубль, по состоянию на {1}";
        private const string ChartTitleCaption = "Распределение территорий по средней розничной цене на товар «{0}», рублей за {1}, по состоянию на {2}";

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = "ХМАОDeer";

            ComboDate.Title = "Выберите дату";
            ComboDate.Width = 275;
            ComboDate.ParentSelect = true;
            ComboFood.Title = "Товар";
            ComboFood.Width = 400;

            #region Параметры

            Food = UserParams.CustomParam("food", true);
            selectedDate = UserParams.CustomParam("selected_date", true);
            selectedDateRF = UserParams.CustomParam("selected_dateRF", true);
            previousDate = UserParams.CustomParam("previous_date");
            previousDateRF = UserParams.CustomParam("previous_dateRF");
            yearDate = UserParams.CustomParam("year_date");
            yearDateRF = UserParams.CustomParam("year_dateRF");
            selectedFood = UserParams.CustomParam("selected_food");
            selectedFoodRF = UserParams.CustomParam("selected_foodRF");

            #endregion

            #region Грид

            if (!IsSmallResolution)
            {
                UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            }
            else
            {
                UltraWebGrid.Width = CRHelper.GetGridWidth(750);
            }
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #endregion

            #region Настройка диаграммы

            if (!IsSmallResolution)
            {
                UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            }
            else
            {
                UltraChart1.Width = CRHelper.GetChartWidth(752);
            }
            UltraChart1.Height = Unit.Empty;

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.ColumnChart.SeriesSpacing = 1;
            UltraChart1.ColumnChart.ColumnSpacing = 1;

            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\nРозничная цена: <b><DATA_VALUE:N2></b>, рубль";
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            #region Карта

            //mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            //mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            if (!IsSmallResolution)
            {
                DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            }
            else
            {
                DundasMap.Width = CRHelper.GetChartWidth(750);
            }
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

            #endregion

            #region Асинхронные панели

            PanelMap.AddRefreshTarget(DundasMap);
            //PanelMap.AddLinkedRequestTrigger(RadioButton1);
            //PanelMap.AddLinkedRequestTrigger(RadioButton2);
            //PanelMap.AddLinkedRequestTrigger(RadioButton3);

            

            PanelMapCaption.AddRefreshTarget(LabelMap);
            PanelMapCaption.LinkedRefreshControlID = "PanelMap";

            #endregion

            #region Ссылки

            HyperLink.Text = "Анализ&nbsp;средних&nbsp;розничных цен&nbsp;на&nbsp;продукты&nbsp;питания";
            HyperLink.NavigateUrl = "~/reports/ORG_0001_0005/Default.aspx";

            #endregion

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
            #endregion
        }

        // --------------------------------------------------------------------


        private string GenAllStrWithFood()
        {
            string res = "";

            for (int i = 0; i < food.Length; i++)
            {
                res += string.Format(@"member [{0}] as '(        [Measures].[Средняя цена],{1})'", food[i], foodMDX[i]);
            }
            return res;
        }

        private string GenAllStrSelectFood()
        {
            string res = "";

            foreach(string str in food)
            {
                res += "["+str+"]"+",";

            }
            return res.Remove(res.Length - 1);

        }

        DataTable GetDSFromQuery(string Query)
        {
                DataTable Table = new DataTable();
             DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(Query, "Filed", Table);
             return Table;
        }


        private string GetDefaultFood(string SelectDate)
        {
            SelectDate = "{" + SelectDate + "}";
            

            string QuertParamWith = GenAllStrWithFood();
            string QueryParamSelect = "{"+GenAllStrSelectFood()+"}";
            string WhereParam = @"([Источники данных].[Источники данных].[Все источники данных].[СТАТ Отчетность - Департамент экономики],
        [Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары],
        [Организации__Предприятия торговли].[Организации__Предприятия торговли].[Все организации],
        [Организации__Тип цены].[Организации__Тип цены].[Все типы цен].[Розничная цена],
        [Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра])";

            DataTable Tabe = GetDSFromQuery(string.Format("/*defaultparam*/ with {0} select non empty {1} on rows, {2} on columns from [ОРГАНИЗАЦИИ_Цены и тарифы] where {3}",
                QuertParamWith,
                QueryParamSelect,
                SelectDate,
                WhereParam).Replace("[Период].[День]", "[Период__День].[Период__День]"));
            return Tabe.Rows[0][0].ToString();
        }


        private void GenSDChart()
        {
            DataTable TableChart = new DataTable();
            TableChart.Columns.Add("Region");
            TableChart.Columns.Add("к предыдущему периоду", typeof(decimal));
            TableChart.Columns.Add("к началу года", typeof(decimal));

            int ColDev = 5+1;
            int ColFy = 7+1;

            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {

                if (row.Cells[ColDev].Value != null || row.Cells[ColFy].Value != null)
                {
                DataRow RowT = TableChart.NewRow();
                TableChart.Rows.Add(RowT);

                    RowT["Region"] = row.Cells[0];
                    RowT["к предыдущему периоду"] = GetValDB(row.Cells[ColDev].Value);
                    RowT["к началу года"] = GetValDB(row.Cells[ColFy].Value);

                    RowT[0] = RowT[0].ToString().Replace("ДАННЫЕ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Trim();
                    RowT[0] = RowT[0].ToString().Replace(" муниципальный район", " р-н");
                    RowT[0] = RowT[0].ToString().Replace("Город ", "Г. ");
                    RowT[0] = RowT[0].ToString().Replace("Ханты-Мансийский автономный округ - Югра", "ХМАО"); 
                }
            }

            SDChart.DataSource = TableChart;
            SDChart.DataBind();

            SDChart.ChartType = ChartType.LineChart;

            SDChart.Width = UltraChart1.Width;
            SDChart.Height = UltraChart1.Height;


            SDChart.Data.SwapRowsAndColumns = true;

            SDChart.Border.Color = Color.White;


            #region Настройка диаграммы 2

            if (!IsSmallResolution)
                SDChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            else
                SDChart.Width = CRHelper.GetChartWidth(750);
            SDChart.Height = (550);

            SDChart.ChartType = ChartType.LineChart;
            SDChart.LineChart.NullHandling = NullHandling.DontPlot;
            SDChart.Border.Thickness = 0;

            SDChart.Axis.X.Extent = 165;
            SDChart.Axis.X.Labels.WrapText = true;
            SDChart.Axis.Y.Extent = 50;
            SDChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            SDChart.Axis.X.Labels.Font = new Font("Arail", 10);

            SDChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            SDChart.ColorModel.Skin.ApplyRowWise = true;
            SDChart.ColorModel.Skin.PEs.Clear();

            for (int i = 0; i < 4; ++i)
            {
                PaintElement pe = new PaintElement();

                switch (i)
                {
                    case 0:
                        {
                            pe.Fill = Color.Green;
                            break;
                        }
                    case 1:
                        {
                            pe.Fill = Color.Blue;
                            break;
                        }
                    case 2:
                        {
                            pe.Fill = Color.Yellow;
                            break;
                        }
                    case 3:
                        {
                            pe.Fill = Color.Red;
                            break;
                        }
                }

                pe.FillStopColor = pe.Fill;
                pe.StrokeWidth = 0;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 200;
                pe.FillStopOpacity = 100;
                SDChart.ColorModel.Skin.PEs.Add(pe);

                pe.Stroke = Color.Black;
                pe.StrokeWidth = 0;

                LineAppearance lineAppearance = new LineAppearance();

                lineAppearance.IconAppearance.Icon = SymbolIcon.Square;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance.IconAppearance.PE = pe;

                SDChart.LineChart.LineAppearances.Add(lineAppearance);

                SDChart.LineChart.Thickness = 0;

                SDChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8); 
                SDChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            }

            SDChart.Tooltips.FormatString = @"<SERIES_LABEL><br>Темп роста <ITEM_LABEL><br><b><DATA_VALUE:N2></b>, %"; 

            SDChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            //SDChart.Tooltips.FormatString = "Помагите!!! я застрял в двумерном простарнстве!!! <br><ITEM_LABEL>\n<SERIES_LABEL>\n<b><DATA_VALUE:N2></b>, рубль";
                //"<ITEM_LABEL>\n<SERIES_LABEL>\n<b><DATA_VALUE:N2></b>, рубль"; 
            SDChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            SDChart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            SDChart.Axis.X.Margin.Near.Value = 20;
            SDChart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            SDChart.Axis.X.Margin.Far.Value = 20;
            SDChart.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
            SDChart.Axis.Y.Margin.Near.Value = 20;
            SDChart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            SDChart.Axis.Y.Margin.Far.Value = 20;

            SDChart.Legend.Location = LegendLocation.Top;
            SDChart.Legend.SpanPercentage = 13;
            SDChart.Legend.Font = new Font("Microsoft Sans Serif", 9);
            SDChart.Legend.Visible = true;

            //SDChart.FillSceneGraph += new FillSceneGraphEventHandler(SDChart_FillSceneGraph);

            #endregion

        }
        

        private object GetValDB(object p)
        {
            if (p == null) 
            {
                return DBNull.Value;
            }

            try
            {
                return decimal.Parse(p.ToString()) * 100 + 100;
            }
            catch 
            {
                return DBNull.Value;
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                FillComboDate(ComboDate, "ORG_0001_0006_list_of_dates");
            }

            #region Анализ параметров

            GetDates(ComboDate, selectedDate, previousDate, yearDate);

            if (!Page.IsPostBack)
            {

                FillComboFood(ComboFood, "ORG_0001_0006_list_of_food");
                ComboFood.SetСheckedState(GetDefaultFood(selectedDate.Value), true);
            }


            selectedDateText = MDXDateToShortDateString(selectedDate.Value);
            selectedDateRF.Value = selectedDateText.Replace(" г.", "");

            previousDateText = MDXDateToShortDateString(previousDate.Value);
            previousDateRF.Value = previousDateText.Replace(" г.", "");

            yearDateText = MDXDateToShortDateString(yearDate.Value);
            yearDateRF.Value = yearDateText.Replace(" г.", "");
            selectedFood.Value = foodMDX[ComboFood.SelectedIndex];

            selectedFoodRF.Value = getLastBlock(selectedFood.Value);
                //ComboFood.SelectedValue;
                //UserComboBox.getLastBlock(selectedFood.Value);
            #endregion

            #region Грид

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            calculateRank();

            #endregion

            #region Диаграмма

            UltraChart1.DataBind();

            #endregion

            #region Карта

            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Value");
            DundasMap.ShapeFields["Value"].Type = typeof(double);
            DundasMap.ShapeFields["Value"].UniqueIdentifier = false;

            SetMapSettings();
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            AddMapLayer(DundasMap, mapFolderName, "Граница", CRHelper.MapShapeType.SublingAreas);
            FillMapData();

            #endregion


            GenSDChart();

            try
            {
                GridHeader.Text = String.Format("Средняя розничная цена на товар «{0}», рублей за {1}", ComboFood.SelectedValue, GetUnit().ToLower());


                LabelChart2.Text = string.Format("Распределение территорий по темпам роста цены на товар «{0}», %, по состоянию на {1}", ComboFood.SelectedValue, MDXDateToShortDateString(selectedDate.Value));
            }
            catch { }
        }

        

        
        string getLastBlock(string s)
        {            
            string[] arrss = s.Split(new string[1] { "].["}, StringSplitOptions.None);
            return arrss[arrss.Length - 1].Replace("]","");
        }

        Node GetPrevNode(Node n)
        {
            if (n.PrevNode == null)
            {
                if (n.Parent.PrevNode == null)
                {
                    return n.Parent.Parent.Nodes[n.Parent.Parent.Nodes.Count - 1].Nodes[n.Parent.Parent.Nodes[n.Parent.Parent.Nodes.Count - 1].Nodes.Count - 1];
                }
                return n.Parent.PrevNode.Nodes[n.Parent.PrevNode.Nodes.Count - 1];
            }
            return n.PrevNode;
        }


        private Node GetLastChild(Node node)
        {
            return node.Nodes[node.Nodes.Count-1];
        }

        /// <summary>
        /// По комбобоксу с датами на основании выбранной даты определяет предыдущую дату и дату на начало года
        /// </summary>
        /// <param name="combo">Комбобокс</param>
        /// <param name="selectedDate">Параметр "Выбранная дата"</param>
        /// <param name="previousDate">Параметр "Предыдущая дата"</param>
        /// <param name="yearDate">Параметр "На начало года"</param>
        protected void GetDates(CustomMultiCombo combo, CustomParam selectedDate, CustomParam previousDate, CustomParam yearDate)
        {
            Node node = new Node();
            if (combo.SelectedNode.Level == 0)
            {
                node = combo.GetLastChild(combo.SelectedNode).FirstNode;
            }
            if (combo.SelectedNode.Level == 1)
            {
                node = combo.SelectedNode.FirstNode;
            }
            if (combo.SelectedNode.Level == 2)
            {
                node = combo.SelectedNode;
            }
            selectedDate.Value = StringToMDXDate(node.Text);
            noMapData = false;
            Node prevNode = null;
            if (node.PrevNode != null)
            {
                prevNode = node.PrevNode;
            }
            else 
            if (node.Parent.PrevNode != null)
            {
                prevNode = GetLastChild(node.Parent.PrevNode);                    
            }
            else if (node.Parent.Parent.PrevNode != null)
            {
                prevNode = GetLastChild(GetLastChild(node.Parent.Parent.PrevNode));
            }
            if (prevNode != null)
            {
                previousDate.Value = StringToMDXDate(prevNode.Text);
            }
            else
            {
                previousDate.Value = StringToMDXDate(ReplaceMonth(node.Text));
                noMapData = true;
            }
            Node yearNode = node.Parent.Parent.FirstNode.FirstNode;
            yearDate.Value = StringToMDXDate(yearNode.Text);

            PageTitle.Text = "Средние розничные цены на отдельные виды социально значимых продовольственных товаров первой необходимости в разрезе муниципальных образований автономного округа";
                //PageTitleCaption;
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(PageSubTitleCaption, node.Text);
        }

        

        // --------------------------------------------------------------------

        #region Обработчики грида

        class RankingField
        {
            class SortKeyVal : System.Collections.Generic.IComparer<KeyVal>
            {
                #region Члены IComparer<KeyVal>

                public int Compare(KeyVal x, KeyVal y)
                {
                    if (x.Val > y.Val)
                    {
                        return 1;
                    }
                    if (x.Val < y.Val)
                    {
                        return -1;
                    }
                    return 0;
                }

                #endregion
            }

            struct KeyVal
            {
                public string Key;
                public decimal Val;
            }

            List<KeyVal> Fields = new List<KeyVal>();

            public int Count
            {
                get { return Fields.Count; }
            }

            public void AddItem(string Key, decimal Val)
            {
                KeyVal NewFild = new KeyVal();
                NewFild.Key = Key;
                NewFild.Val = Val;
                Fields.Add(NewFild);
            }

            void ClearDoubleVal()
            {
                List<KeyVal> RemoveList = new List<KeyVal>();
                for (int i = 0; i < Fields.Count - 1; i++)
                {
                    for (int j = i + 1; j < Fields.Count; j++)
                    {
                        if (Fields[i].Key == Fields[j].Key)
                        {
                            //RemoveList.Add(Fields[j]);
                            Fields.Remove(Fields[j]);
                        }
                    }
                }

                foreach (KeyVal kv in RemoveList)
                {
                    Fields.Remove(kv);
                }
            }

            public object GetRang(string Key)
            {
                ClearDoubleVal();
                Fields.Sort(new SortKeyVal());

                int i = 0;
                foreach (KeyVal kv in Fields)
                {
                    i++;

                    if (kv.Key.Split(';')[0] == Key.Split(';')[0])
                    {
                        return i;
                    }
                }
                return DBNull.Value;
            }

        }


        #region Сортировалко >_<
        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return -Compare_(x, y);
            }

            public int Compare_(DataRow x, DataRow y)
            {
                string Xname = x[0].ToString();
                string Yname = y[0].ToString();

                if (Xname == Yname)
                {
                    return 0;
                }

                if (Xname == "Российская Федерация")
                {
                    return 1;
                }

                if (Yname == "Российская Федерация")
                {
                    return -1;
                }

                if (Xname.Contains("Ханты-Мансийский автономный округ"))
                {
                    return 1;
                }

                if (Yname.Contains("Ханты-Мансийский автономный округ"))
                {
                    return -1;
                }

                if (Xname.Contains("Город Ханты-Мансийск"))
                {
                    return 1;
                }

                if (Yname.Contains("Город Ханты-Мансийск"))
                {
                    return -1;
                }
                if ((Xname[0] == 'Г') && (Yname[0] != 'Г'))
                {
                    return 1;
                }

                if ((Xname[0] != 'Г') && (Yname[0] == 'Г'))
                {
                    return -1;
                }


                return Yname.CompareTo(Xname);
            }

            #endregion
        }

        DataTable SortTable(DataTable Table)
        {
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<DataRow> LR = new System.Collections.Generic.List<DataRow>();

            foreach (DataRow row in Table.Rows)
            {
                LR.Add(row);
            }

            LR.Sort(new SortDataRow());



            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);
            }
            return TableSort;
        }
        #endregion

        void ClearemptyRow(DataTable Table)
        {
            List<DataRow> EmptyRow = new List<DataRow>();

            // 
            foreach (DataRow Row in Table.Rows)
            //for (int j = 3; j < Table.Rows.Count;j++)
            {
                //DataRow Row = Table.Rows[j];
                if (Row[0].ToString().Contains("Ханты-Мансийский автономный округ"))
                {
                    continue;
                }
                bool NoEmptyRow = false;
                //for (int i = 1; i < Table.Columns.Count; i++)
                {
                    if (Row[1] != DBNull.Value)
                    {
                        NoEmptyRow = true;
                    }
                }
                if (!NoEmptyRow)
                {
                    EmptyRow.Add(Row);
                }
            }
            foreach (DataRow Row in EmptyRow)
            {
                Table.Rows.Remove(Row);
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("ORG_0001_0006_grid_part1");
            DataTable dtData1 = new DataTable();

            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtData1);
            query = DataProvider.GetQueryText("ORG_0001_0006_grid_part2");
            DataTable dtData2 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtData2);
            if (dtData1.Rows.Count > 0 || dtData2.Rows.Count > 0)
            {
                dtGrid = new DataTable();
                FillTableData(dtGrid, dtData1, dtData2);

                query = DataProvider.GetQueryText("RFTable");
                dtData2 = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtData2);
                ClearemptyRow(dtGrid);
                DataRow RFRow = dtGrid.NewRow();
                dtGrid.Rows.Add(RFRow);

                RFRow[0] = "Российская Федерация";
                try
                { 
                    RFRow[3] = dtData2.Rows[2][1];
                    RFRow[1] = dtData2.Rows[1][1];
                    RFRow[2] = dtData2.Rows[0][1];

                }
                catch { }

                try
                {
                    RFRow[7] = Minus(RFRow[1], RFRow[3]);
                    RFRow[8] = Grow(RFRow[1], RFRow[3]);
                    RFRow[5] = Minus(RFRow[1], RFRow[2]);
                    RFRow[6] = Grow(RFRow[1], RFRow[2]);
                }
                catch { }

                UltraWebGrid.DataSource = SortTable(dtGrid);
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }


        protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(270);
            int columnWidth = 110;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(50);

            // Заголовки
            GridHeaderCell header;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            headerLayout.AddCell("Наименование МО");
            header = headerLayout.AddCell("Средняя розничная цена, рубль");
            header.AddCell(String.Format("{0}", selectedDateText));
            header.AddCell(String.Format("{0}", previousDateText));
            header.AddCell(String.Format("{0}", yearDateText));
            headerLayout.AddCell("Ранг");

            header = headerLayout.AddCell("Динамика к предыдущему отчетному периоду");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            header = headerLayout.AddCell("Динамика за период с начала года");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            headerLayout.ApplyHeaderInfo();
        }

        private string SetStarChar(string RegionName)
        {
            string NameRegion = RegionName;

            string[] StarRegions = new string[14] { "Ханты-Мансийский автономный округ", "Советск", "Сургутск", "Когал", "Ланге", "Мегион", "Нефтеюганск", " Нижневартовск", "Нижневартовский-", "Нягань", "Сургут", "Пыть", "Югорск", "Российская" };
            foreach (string R in StarRegions)
            {
                if (NameRegion.Contains(R))
                {
                    return NameRegion;
                }
            }
            return NameRegion + "*";
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            //;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            double value;
            if (Double.TryParse(e.Row.Cells[6].Text, out value) && value != 0)
            {
                if (value > 0)
                    SetImageFromCell(e.Row.Cells[6], "ArrowRedUpBB.png");
                //e.Row.Cells[6].Style.CssClass = "ArrowUpRed";
                else
                    SetImageFromCell(e.Row.Cells[6], "ArrowGreenDownBB.png");
                //e.Row.Cells[6].Style.CssClass = "ArrowDownGreen";
            }
            if (Double.TryParse(e.Row.Cells[8].Text, out value) && value != 0)
            {
                if (value > 0)
                    SetImageFromCell(e.Row.Cells[8], "ArrowRedUpBB.png");
                //e.Row.Cells[8].Style.CssClass = "ArrowUpRed";
                else
                    SetImageFromCell(e.Row.Cells[8], "ArrowGreenDownBB.png");
                //e.Row.Cells[8].Style.CssClass = "ArrowDownGreen";
            }
            // Хинты
            e.Row.Cells[7].Title = String.Format("Изменение в руб. к {0}", yearDateText);
            e.Row.Cells[8].Title = String.Format("Изменение в % к {0}", yearDateText);
            e.Row.Cells[5].Title = String.Format("Изменение в руб. к {0}", previousDateText);
            e.Row.Cells[6].Title = String.Format("Изменение в % к {0}", previousDateText);
            try
            {
                e.Row.Cells[0].Text = SetStarChar(e.Row.Cells[0].Text);
            }
            catch { }

            if (e.Row.Index < 2)
            {
                e.Row.Style.Font.Bold = true;

            }
        }

        struct RegionValue
        {
            public string RegionName;
            public decimal Value;
        }

        class ComparerRegionValue : IComparer<RegionValue>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(RegionValue x, RegionValue y)
            {
                if (x.Value > y.Value)
                {
                    return 1;
                }
                if (x.Value < y.Value)
                {
                    return -1;
                }
                return 0;
            }

            #endregion
        }

        List<RegionValue> RVList;

        void BindListRang()
        {
            RVList = new List<RegionValue>();
            for (int i = 2; i < UltraWebGrid.Rows.Count; i++)
            {
                if (UltraWebGrid.Rows[i].Cells[3].Value != null)
                {
                    RegionValue rv = new RegionValue();
                    rv.RegionName = UltraWebGrid.Rows[i].Cells[0].Text;
                    rv.Value = decimal.Parse(UltraWebGrid.Rows[i].Cells[3].Text);
                    RVList.Add(rv);
                }
            }
            RVList.Sort(new ComparerRegionValue());
        }
        int GetRang(string RegionNAme)
        {
            int i = 0;
            foreach (RegionValue Rv in RVList)
            {
                i++;
                if (Rv.RegionName == RegionNAme)
                {
                    return i;
                }
            }
            return -1;
        }


        protected void calculateRank()
        {
            RankingField Ranger = new RankingField();
            int i = 0;
            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {
                try
                {
                    if (i > 1)
                        Ranger.AddItem(Row.Cells[1].ToString(), Convert.ToDecimal(Row.Cells[1].Value));
                    i++;
                }
                catch { }
            }

            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {
                try
                {
                    int Rank = (int)Ranger.GetRang(Row.Cells[1].ToString());
                    Row.Cells[4].Value = Rank;

                    if (Rank == 1)
                    {
                        Row.Cells[4].Style.BackgroundImage = "~/images/starYellowbb.png";
                        Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        Row.Cells[4].Title = "Самый низкий уровень цены";
                    }
                    if (Rank == Ranger.Count)
                    {
                        Row.Cells[4].Style.BackgroundImage = "~/images/starGraybb.png";
                        Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        Row.Cells[4].Title = "Самый высокий уровень цены";
                    }
                }
                catch { }

            }
        }

        protected void FillTableData(DataTable dtGrid, DataTable dtData1, DataTable dtData2)
        {
            dtGrid.Columns.Add("Название МО", typeof(string));
            dtGrid.Columns.Add("Цена на начало года", typeof(double));
            dtGrid.Columns.Add("Цена на предыдущую отчетную дату", typeof(double));
            dtGrid.Columns.Add("Цена на отчетную дату", typeof(double));
            dtGrid.Columns.Add("Ранг", typeof(int));
            dtGrid.Columns.Add("Прирост к началу года", typeof(double));
            dtGrid.Columns.Add("Темп прироста к началу года", typeof(double));
            dtGrid.Columns.Add("Прирост к предыдущей дате", typeof(double));
            dtGrid.Columns.Add("Темп прироста к предыдущей дате", typeof(double));

            foreach (DataRow dataRow in dtData1.Rows)
            {
                DataRow row = dtGrid.NewRow();
                row[0] = dataRow[0];
                int index1 = 1 + (dataRow.ItemArray.Length - 1) / 3;
                int index2 = index1 + (dataRow.ItemArray.Length - 1) / 3;
                int index3 = dataRow.ItemArray.Length;
                row[1] = GetGeoMean(dataRow, 1, index1);
                row[2] = GetGeoMean(dataRow, index1, index2);
                row[3] = GetGeoMean(dataRow, index2, index3);

                row[7] = Minus(row[1], row[3]);
                row[8] = Grow(row[1], row[3]);
                row[5] = Minus(row[1], row[2]);
                row[6] = Grow(row[1], row[2]);

                dtGrid.Rows.Add(row);
            }

            foreach (DataRow dataRow in dtData2.Rows)
            {
                DataRow row = dtGrid.NewRow();
                row[0] = dataRow[0];
                row[1] = dataRow[1];
                row[2] = dataRow[2];
                row[3] = dataRow[3];

                row[7] = Minus(row[1], row[3]);
                row[8] = Grow(row[1], row[3]);
                row[5] = Minus(row[1], row[2]);
                row[6] = Grow(row[1], row[2]);

                dtGrid.Rows.Add(row);
            }

        }

        protected object GetGeoMean(DataRow row, int startIndex, int finishIndex)
        {
            double result = 1.00;
            int pow = 0;
            for (int i = startIndex; i < finishIndex; ++i)
            {
                double value;
                if (Double.TryParse(row[i].ToString(), out value))
                {
                    result *= value;
                    ++pow;
                }
            }

            if (pow > 0)
            {
                result = Math.Pow(result, 1.0 / pow);
                return result;
            }
            else
            {
                return DBNull.Value;
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики диаграммы
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {


            LabelChart1.Text = String.Format(ChartTitleCaption, ComboFood.SelectedValue, GetUnit().ToLower(), selectedDateText);
            if (dtGrid == null)
            {
                UltraChart1.DataSource = null;
                return;
            }

            dtChart = new DataTable();
            dtChart.Columns.Add("МО", typeof(string));
            dtChart.Columns.Add("Средняя розничная цена", typeof(double));
            for (int i = 2; i <= UltraWebGrid.Rows.Count; ++i)
            {
                foreach (UltraGridRow gridRow in UltraWebGrid.Rows)
                {

                    if (Convert.ToInt32(gridRow.Cells[4].Value) == i - 1)
                    {
                        DataRow chartRow = dtChart.NewRow();
                        chartRow[0] = gridRow.Cells[0].Value;
                        chartRow[1] = gridRow.Cells[1].Value == null ? DBNull.Value : gridRow.Cells[1].Value;
                        dtChart.Rows.Add(chartRow);
                    }

                }
            }
            medianDT = new DataTable();
            double minValue = Double.PositiveInfinity; ;
            double maxValue = Double.NegativeInfinity;
            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("ДАННЫЕ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Trim();
                    row[0] = row[0].ToString().Replace(" муниципальный район", " р-н");
                    row[0] = row[0].ToString().Replace("Город ", "Г. ");
                }
            }
            if (dtChart.Rows.Count > 1)
            {
                double avgValue = 0;
                for (int i = 0; i < dtChart.Rows.Count; ++i)
                {
                    if (dtChart.Rows[i][1] != DBNull.Value)
                    {
                        double value = Convert.ToDouble(dtChart.Rows[i][1]);
                        avgValue += value;
                        minValue = value < minValue ? value : minValue;
                        maxValue = value > maxValue ? value : maxValue;
                    }
                }
                avgValue /= dtChart.Rows.Count;
                // рассчитываем медиану
                int medianIndex = MedianIndex(dtChart.Rows.Count);
                medianDT = dtChart.Clone();
                double medianValue = MedianValue(dtChart, 1);
                for (int i = 0; i < dtChart.Rows.Count - 1; i++)
                {

                    medianDT.ImportRow(dtChart.Rows[i]);

                    double value;
                    Double.TryParse(dtChart.Rows[i][1].ToString(), out value);
                    double nextValue;
                    Double.TryParse(dtChart.Rows[i + 1][1].ToString(), out nextValue);
                    if (((value <= avgValue) && (nextValue > avgValue)) && (i == medianIndex))
                    {
                        if (medianValue > avgValue)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                            row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                            medianDT.Rows.Add(row);
                        }
                        else
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                            medianDT.Rows.Add(row);
                            row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                        }
                    }
                    else
                    {
                        if ((value <= avgValue) && (nextValue > avgValue))
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                        }

                        if (i == medianIndex)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                            medianDT.Rows.Add(row);
                        }
                    }
                }
                medianDT.ImportRow(dtChart.Rows[dtChart.Rows.Count - 1]);


                double RFVal = -666;
                if (UltraWebGrid.Rows[0].Cells[3].Value != null)
                {
                    RFVal = (double)UltraWebGrid.Rows[0].Cells[3].Value;
                }
                if (UltraWebGrid.Rows[1].Cells[3].Value != null)
                {
                    double XMAOVal = (double)UltraWebGrid.Rows[1].Cells[3].Value;


                    maxValue = maxValue > XMAOVal ? maxValue : XMAOVal;

                    minValue = minValue < XMAOVal ? minValue : XMAOVal;
                }

                if (UltraWebGrid.Rows[0].Cells[3].Value != null)
                {
                    maxValue = maxValue > RFVal ? maxValue : RFVal;
                    minValue = minValue < RFVal ? minValue : RFVal;
                }

                if (!Double.IsPositiveInfinity(minValue) && !Double.IsNegativeInfinity(maxValue))
                {
                    UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                    UltraChart1.Axis.Y.RangeMax = maxValue * 1.1;
                    UltraChart1.Axis.Y.RangeMin = minValue / 1.1;
                }
            }

            if (medianDT != null)
            {
                 List<DataRow> DelRows = new List<DataRow>();
                 foreach (DataRow Row in medianDT.Rows)
                 {
                     if (Row[0].ToString().ToLower().Contains("медиана"))
                     {
                         DelRows.Add(Row);
                     }
                     if (Row[0].ToString().ToLower().Contains("среднее"))
                     {
                         DelRows.Add(Row);
                     }
                 }
                 foreach (DataRow row in DelRows)
                 {
                     medianDT.Rows.Remove(row);
                 }
            }

            UltraChart1.DataSource = (medianDT == null) ? null : medianDT.DefaultView;
        }

        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 2;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;

            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY - 15, 800, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY + 1, 800, 15);
            }
            textLabel.labelStyle.FontColor = Color.LightGray;
            textLabel.SetTextString(Label);
            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            try
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];

                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            if (box.Series != null && (box.Series.Label == "Среднее" || box.Series.Label == "Медиана"))
                            {
                                box.PE.Fill = Color.Orange;
                                box.PE.FillStopColor = Color.OrangeRed;
                            }
                        }
                    }
                }

                Text Caption = new Text();
                Caption.SetTextString(string.Format("рублей за {0}", GetUnit().ToLower()));
                Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
                Caption.labelStyle.FontColor = Color.Gray;
                Caption.bounds.X = -30;
                Caption.bounds.Y = 20;
                Caption.bounds.Width = 100;
                Caption.bounds.Height = 100;

                e.SceneGraph.Add(Caption);


            }
            catch { }

            double RFVal = -666;
            double XMAOVal = -666;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];


            try
            {
                XMAOVal = (double)UltraWebGrid.Rows[1].Cells[1].Value;
                if (UltraWebGrid.Rows[0].Cells[3].Value != null)
                {
                    RFVal = (double)UltraWebGrid.Rows[0].Cells[1].Value;
                }

                bool RFUp = true;
                bool HMAOUp = true;

                if (Math.Abs((int)yAxis.Map(RFVal) - (int)yAxis.Map(XMAOVal)) < 15)
                {
                    RFUp = RFVal > XMAOVal;
                    HMAOUp = RFVal < XMAOVal;
                }
                if (RFVal != -666)
                    if (UltraWebGrid.Rows[0].Cells[3].Value != null)
                    {
                        GenHorizontalLineAndLabel(
                                (int)xAxis.Map(xAxis.Minimum),
                                (int)yAxis.Map(RFVal),
                                (int)xAxis.Map(xAxis.Maximum),
                                (int)yAxis.Map(RFVal),
                                Color.Red,
                                string.Format("Российская Федерация  -{0:### ### ##0.##}, рублей за {1}", RFVal, GetUnit().ToLower()),
                                e,
                                RFUp);
                    }

                if (UltraWebGrid.Rows[1].Cells[3].Value != null)
                {
                    GenHorizontalLineAndLabel(
                            (int)xAxis.Map(xAxis.Minimum),
                            (int)yAxis.Map(XMAOVal),
                            (int)xAxis.Map(xAxis.Maximum),
                            (int)yAxis.Map(XMAOVal),
                            Color.Blue,
                            string.Format("Ханты-Мансийский автономный округ - Югра {0:### ### ##0.##}, рублей за {1}", XMAOVal, GetUnit().ToLower()),
                            e,
                            HMAOUp);
                }
            }
            catch {
                try
                {
                    RFVal = (double)UltraWebGrid.Rows[0].Cells[1].Value;
                    GenHorizontalLineAndLabel(
                                    (int)xAxis.Map(xAxis.Minimum),
                                    (int)yAxis.Map(RFVal),
                                    (int)xAxis.Map(xAxis.Maximum),
                                    (int)yAxis.Map(RFVal),
                                    Color.Red,
                                    string.Format("Российская Федерация  -{0:### ### ##0.##}, рублей за {1}", RFVal, GetUnit().ToLower()),
                                    e,
                                    true);
                }
                catch 
                {
                    try
                    {
                        XMAOVal = (double)UltraWebGrid.Rows[1].Cells[1].Value;
                        GenHorizontalLineAndLabel(
                            (int)xAxis.Map(xAxis.Minimum),
                            (int)yAxis.Map(XMAOVal),
                            (int)xAxis.Map(xAxis.Maximum),
                            (int)yAxis.Map(XMAOVal),
                            Color.Blue,
                            string.Format("Ханты-Мансийский автономный округ - Югра  -{0:### ### ##0.##}, рублей за {1}", XMAOVal, GetUnit().ToLower()),
                            e,
                            true);
                    }
                    catch { }
                }
            
            
            }

        }

        #endregion

        #region Расчет медианы

        private static bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }

        private static int MedianIndex(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            if (Even(length))
            {
                return length / 2 - 1;
            }
            else
            {
                return (length + 1) / 2 - 1;
            }
        }

        private static double MedianValue(DataTable dt, int medianValueColumn)
        {
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            if (Even(dt.Rows.Count))
            {
                double value1;
                double value2;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value1);
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count) + 1][medianValueColumn].ToString(),
                        out value2);
                return (value1 + value2) / 2;
            }
            else
            {
                double value;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value);
                return value;
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики карты

        public void SetMapSettings()
        {
            if (noMapData )//&& RadioButton2.Checked)
            {
                DundasMap.Visible = false;
                EmptyMapLabel.Visible = true;
                EmptyMapLabel.Text = "<br/>В настоящий момент данные отсутствуют<br/>";
            }
            else
            {
                DundasMap.Visible = true;
                EmptyMapLabel.Visible = false;
            }
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 100;

            // добавляем легенду
            Legend legend = new Legend("Legend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = RadioButton1.Checked ? "Средняя розничная\nцена на товар,\nрубль" : 
                RadioButton2.Checked?
                "Изменение цены\nк предыдущей дате,\nрубль":
                "Изменение цены\nк началу года,\nрубль";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            if (RadioButton1.Checked)
            {
                ShapeRule rule = new ShapeRule();
                rule.Name = "Rule";
                rule.Category = String.Empty;
                rule.ShapeField = "Value";
                rule.DataGrouping = DataGrouping.EqualInterval;
                rule.ColorCount = 5;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = Color.Green;
                rule.MiddleColor = Color.Yellow;
                rule.ToColor = Color.Red;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInColorSwatch = false;
                rule.ShowInLegend = "Legend";
                DundasMap.ShapeRules.Add(rule);
            }
            else
            {
                LegendItem item = new LegendItem();
                item.Text = "Рост цены";
                item.Color = Color.Red; ;
                legend.Items.Add(item);

                item = new LegendItem();
                item.Text = "Снижение цены";
                item.Color = Color.Green;
                legend.Items.Add(item);
            }
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            foreach (Shape shape in map.Shapes)
            {
                if (shape.Name.ToLower() == patternValue.ToLower())
                {
                    return shape;
                }
            }
            return null;
        }


        public void FillMapData()
        {
            string valueSeparator = IsMozilla ? ". " : "\n";
            string shapeHint;
            if (RadioButton1.Checked)
            {
                shapeHint = "{0}" + valueSeparator + "Розничная цена: {1:N2}, рубль" + valueSeparator + "Ранг: {2:N0}";
                LabelMap.Text = String.Format(MapTitleCaption1, ComboFood.SelectedValue, GetUnit().ToLower(), selectedDateText);
            }
            else
            {
                shapeHint = "{0}" + valueSeparator + "Прирост к {1}" + valueSeparator + "{2:N2}, рубль";
                LabelMap.Text = String.Format(MapTitleCaption2, ComboFood.SelectedValue, selectedDateText);
            }
            if (dtGrid == null || DundasMap == null)
            {
                return;
            }
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = String.Format("{0}", shape.Name);
            }
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                // заполняем карту данными
                string subject = row.Cells[0].Value.ToString().Replace("Город ", "г. ").Replace(" муниципальный район", " р-н");
                int index = RadioButton1.Checked ? 1 : (RadioButton2.Checked?5:7);
                double value;
                if (row.Cells[index].Value == null || !Double.TryParse(row.Cells[index].Value.ToString(), out value))
                {
                    value = 0;
                }
                Shape shape = FindMapShape(DundasMap, subject.Replace("*", ""));
                if (shape == null)
                {
                    continue;
                }
                shape.Visible = true;
                string shapeName = shape.Name;

                shape["Name"] = subject;
                shape["Value"] = value;
                shape.TextVisibility = TextVisibility.Shown;
                shape.Text = String.Format("{0}\n{1:N2}", shapeName.Replace(" ", "\n"), value);
                if (RadioButton1.Checked)
                {
                    shape.ToolTip = String.Format(shapeHint, subject, row.Cells[index].Value, row.Cells[4].Value);
                }
                else
                {
                    if (row.Cells[index].Value != null)
                    {

                        if (RadioButton2.Checked)
                        {

                            shape.ToolTip = String.Format(shapeHint, subject, MDXDateToShortDateString(previousDate.Value), value);
                        }
                        else
                        {
                            shape.ToolTip = String.Format(shapeHint, subject, MDXDateToShortDateString(yearDate.Value), value);
                        }
                        shape.Color = value > 0 ? Color.Red : Color.Green;
                    }
                    else
                    {
                        shape.ToolTip = String.Format("{0}" + valueSeparator + "В настоящий момент данные отсутствуют.", subject);
                        shape.Text = String.Format("{0}", shapeName.Replace(" ", "\n"));
                    }
                    
                }
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Заполнение словарей и выпадающих списков параметров

        protected string GetUnit()
        {
            string query = DataProvider.GetQueryText("ORG_0001_0006_unit");
            DataTable dtUnit = GetDSFromQuery(query);
            
            if (dtUnit == null)
                return "Единица измерения";
            else
                return dtUnit.Rows[0][1].ToString().Replace("илограмм", "г.");
        }

        protected void FillComboFood(CustomMultiCombo combo, string queryName)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (string foodName in food)
                AddPairToDictionary(dict, foodName, 0);
            combo.FillDictionaryValues(dict);
            if (!String.IsNullOrEmpty(Food.Value))
            {
                combo.SetСheckedState(Food.Value, true);
            }
        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName)
        {
            // Загрузка списка актуальных дат
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            // Закачку придется делать через словарь
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < dtDate.Rows.Count; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                string day = dtDate.Rows[row][4].ToString();
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
                AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
            }
            combo.FillDictionaryValues(dictDate);
            //if (!String.IsNullOrEmpty(selectedDate.Value))
            //{
            //    combo.SetСheckedState(MDXDateToDateString(selectedDate.Value), true);
            //}
            //else
            //{

            //}
            if (!Page.IsPostBack)
            {
                combo.SelectLastNode();
            }
        }

        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        #endregion

        #region Функции-полезняшки преобразования и все такое

        public string MDXDateToDateString(string MDXDateString)
        {
            string[] separator = { "].[" };
            string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
            string template = "{0} {1} {2} года";
            string day = dateElements[7].Replace("]", String.Empty);
            string month = CRHelper.RusMonthGenitive(CRHelper.MonthNum(dateElements[6]));
            string year = dateElements[3];
            return String.Format(template, day, month, year);
        }

        public object Plus(object firstValue, object secondValue)
        {
            double value1, value2;
            if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2))
                return value1 + value2;
            else
                return DBNull.Value;
        }

        public object Minus(object firstValue, object secondValue)
        {
            double value1, value2;
            if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2))
                return value1 - value2;
            else
                return DBNull.Value;
        }

        public object Grow(object firstValue, object secondValue)
        {
            double value1, value2;
            if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2) && value2 != 0)
                return value1 / value2 -1;
            else
                return DBNull.Value;
        }

        public string StringToMDXDate(string str)
        {
            string template = "[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[2]);
            string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            int day = Convert.ToInt32(dateElements[0]);
            return String.Format(template, year, halfYear, quarter, month, day);
        }

        public string StringToMDXFood(string foodGroup, string food)
        {
            string template = "[Организации].[Товары и услуги].[Все товары и услуги].[Продовольственные товары].[{0}].[{1}]";
            return String.Format(template, foodGroup, food);
        }

        public string MDXDateToShortDateString(string MDXDateString)
        {
            string[] separator = { "].[" };
            string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
            string template = "{0:00}.{1:00}.{2} г.";
            int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
            int month = CRHelper.MonthNum(dateElements[6]);
            string year = dateElements[3];
            return String.Format(template, day, month, year);
        }

        public bool isPreviousMonth(string firstMonth, string secondMonth)
        {
            int MonthNumDelta = CRHelper.MonthNum(firstMonth) - CRHelper.MonthNum(secondMonth);
            return ((MonthNumDelta == 1) || (MonthNumDelta == 11));
        }

        public string getMonthFromString(string date)
        {
            string[] dateElements = date.Split(' ');
            return dateElements[1];
        }

        public string getYearFromString(string date)
        {
            string[] dateElements = date.Split(' ');
            return dateElements[2];
        }

        public string getYearDate(string date)
        {
            string[] dateElements = date.Split(' ');
            return String.Format("11 января {0} года", dateElements[2]);
        }

        public string ReplaceMonth(string dateString)
        {
            string[] dateElements = dateString.Split(' ');
            int day = Convert.ToInt32(dateElements[0]);
            int monthIndex = CRHelper.MonthNum(dateElements[1]);
            int year = Convert.ToInt32(dateElements[2]);
            DateTime date = new DateTime(year, monthIndex, day);
            date = date.AddDays(-7);
            return String.Format("{0} {1} {2} года", date.Day, CRHelper.RusMonthGenitive(date.Month), date.Year);
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

            ISection section3 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 20;
            ReportPDFExporter1.Export(headerLayout, GridHeader.Text, section1);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));


            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            section3.PageSize = new PageSize(700, 900);
            ReportPDFExporter1.Export(DundasMap, LabelMap.Text, section3);
            section3.PageSize = new PageSize(700, 900);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");

            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 25;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;
            ReportExcelExporter1.TitleStartRow = 0;

            ReportExcelExporter1.Export(headerLayout, sheet1, 4);

            sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet1.Rows[1].Height = 550;






            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);

            sheet2.MergedCellsRegions.Clear();
            sheet2.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet2.Rows[0].Height = 550;

            sheet3.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            sheet3.PrintOptions.PaperSize = PaperSize.A4;
            sheet3.PrintOptions.BottomMargin = 0.25;
            sheet3.PrintOptions.TopMargin = 0.25;
            sheet3.PrintOptions.LeftMargin = 0.25;
            sheet3.PrintOptions.RightMargin = 0.25;
            sheet3.PrintOptions.ScalingType = ScalingType.FitToPages;
            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
            sheet3.Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
            sheet3.Rows[0].Cells[0].CellFormat.Font.Height = 220;
            sheet3.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet3.Rows[0].Cells[0].Value = LabelMap.Text;
            sheet3.Rows[0].Height = 550;
            ReportExcelExporter.MapExcelExport(sheet3.Rows[1].Cells[0], DundasMap);
            sheet1.Rows[3].Cells[0].Value = GridHeader.Text;
            sheet1.Rows[3].Cells[0].CellFormat.Font.Name = sheet1.Rows[1].Cells[0].CellFormat.Font.Name;
            sheet1.Rows[3].Cells[0].CellFormat.Font.Height = sheet1.Rows[1].Cells[0].CellFormat.Font.Height;
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

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.1;
            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }

        #endregion
    }
}
