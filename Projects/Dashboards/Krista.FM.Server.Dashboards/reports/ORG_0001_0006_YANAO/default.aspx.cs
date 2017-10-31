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

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0001_0006_YANAO
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
        string[] food = { "Говядина (кроме бескостного мяса)", 
                            "Свинина (кроме бескостного мяса)", 
                            "Куры (кроме куринных окорочков)", 
                            "Рыба замороженная неразделенная ",
                            "Масло сливочное", 
                            "Масло подсолнечное",
						  "Молоко питьевое (жирностью 2,5, 3,2 %)",
                          "Творог",
                          "Кефир",
                          "Сыр твердых сортов",
                          "Яйца куриные", 
                          "Сахар-песок",
                          "Соль поваренная пищевая",
                          "Чай черный байховый",
                          "Мука пшеничная ", 
                          "Хлеб ржано-пшеничный",
                          "Хлеб пшеничный из муки 1 сорта",
                          "Хлеб пшеничный из муки 2 сорта",
                          "Рис шлифованный",
                          "Пшено",
                          "Крупа гречневая - ядрица",
                          "Вермишель",
                          "Картофель",
						  "Капуста белокочанная свежая",
                          "Лук репчатый",
                          "Морковь", 
                          "Яблоки"};
        string[] foodMDX =
		{
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Говядина (кроме бескостного мяса)]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Свинина (кроме бескостного мяса)]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Куры (кроме куринных окорочков)]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Рыба].[Рыба мороженная неразделенная]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло сливочное]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло подсолнечное]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Молоко питьевое (жирностью 2,5, 3,2%)]",            

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Творог]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Кефир]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Сыр твердых сортов]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Яйца куриные]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Сахар-песок]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Соль, пряности].[Соль поваренная пищевая]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Чай, кофе].[Чай черный байховый]",			

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Мука пшеничная]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб ржано-пшеничный]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб пшеничный из муки 1 сорта]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб пшеничный из муки 2 сорта]",

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
        private const string PageSubTitleCaption = "Еженедельный мониторинг средних розничных цен на отдельные виды социально значимых продовольственных товаров первой необходимости в разрезе муниципальных образований, Ямало-Ненецкий автономный округ, по состоянию на {0}.";
        private const string MapTitleCaption1 = "Средняя розничная цена на товар «{0}», рублей за {1}, по состоянию на {2}";
        private const string MapTitleCaption2 = "Изменение цены на товар «{0}», рубль, по состоянию на {1}";
        private const string ChartTitleCaption = "Распределение территорий по средней розничной цене на товар «{0}», рублей за {1}, по состоянию на {2}";

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = "ЯНАО";

            ComboDate.Title = "Выберите дату";
            ComboDate.Width = 275;
            ComboDate.ParentSelect = true;
            ComboFood.Title = "Товар";
            ComboFood.Width = 400;

            ComboDateCompare.Title = "Выберите дату для сравнения";
            ComboDateCompare.Width = 375;
            ComboDateCompare.ParentSelect = true;    
        

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
            PanelMap.AddLinkedRequestTrigger(cbPrice);
            PanelMap.AddLinkedRequestTrigger(cbDelta);

            PanelMapCaption.AddRefreshTarget(LabelMap);
            PanelMapCaption.LinkedRefreshControlID = "PanelMap";

            #endregion

            #region Ссылки

            HyperLink.Text = "Анализ&nbsp;средних&nbsp;розничных цен&nbsp;на&nbsp;продукты&nbsp;питания";
            HyperLink.NavigateUrl = "~/reports/ORG_0001_0005_YANAO/Default.aspx";

            #endregion

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
            #endregion
        }

        Node GetFirstChild(Node n)
        {
            for (; n.Nodes.Count > 0; n = n.Nodes[0]) ;
            return n;
        }


        Node getRoot(Node n)
        {
            for (; n.Parent != null; n = n.Parent) ;

            for (; n.PrevNode != null; n = n.PrevNode) ;
            return n;
        }


        int GetIndexNode(Node n)
        {
            Node ParentNode = getRoot(n);

            int index = 0;

            for (; ParentNode != null; ParentNode = ParentNode.NextNode)
            {

                foreach (Node i in ParentNode.Nodes)
                {
                    foreach (Node j in i.Nodes)
                    {
                        index++;

                        if (j.Text == n.Text)
                        {
                            return index;
                        }
                    }
                }
            }

            return index;
        }

        Node GetCompareDateNode()
        {
            if (GetIndexNode(ComboDate.SelectedNode) < GetIndexNode(ComboDateCompare.SelectedNode))
            {
                return ComboDate.SelectedNode;
            }

            return ComboDateCompare.SelectedNode;
        }
         
        Node GetLastDateNode()
        {
            if (GetIndexNode(ComboDate.SelectedNode) < GetIndexNode(ComboDateCompare.SelectedNode))
            {
                return ComboDateCompare.SelectedNode;
            }
            return ComboDate.SelectedNode;
        }



        // --------------------------------------------------------------------

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                FillComboDate(ComboDateCompare, "ORG_0001_0006_list_of_dates");
                FillComboDate(ComboDate, "ORG_0001_0006_list_of_dates");
                FillComboFood(ComboFood, "ORG_0001_0006_list_of_food");

                cbPrice.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", cbDelta.ClientID));    
                cbDelta.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", cbPrice.ClientID));
            }

            #region Анализ параметров

            GetDates(ComboDate, selectedDate, previousDate, yearDate);
            selectedDateText = MDXDateToShortDateString(selectedDate.Value);
            selectedDateRF.Value = selectedDateText.Replace(" г.", "");

            previousDateText = MDXDateToShortDateString(previousDate.Value);
            previousDateRF.Value = previousDateText.Replace(" г.", "");

            yearDateText = MDXDateToShortDateString(yearDate.Value);
            yearDateRF.Value = yearDateText.Replace(" г.", "");
            selectedFood.Value = foodMDX[ComboFood.SelectedIndex];

            selectedFoodRF.Value = UserComboBox.getLastBlock(selectedFood.Value);
            #endregion

            #region Грид

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            DataBindGrid();
            UltraWebGrid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            UltraWebGrid.DisplayLayout.NullTextDefault = "Нет данных";

            ConfHeaderGrid();
            //UltraWebGrid.DataBind(); 
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
            AddMapLayer(DundasMap, mapFolderName, "Территории", CRHelper.MapShapeType.Areas);
            //AddMapLayer(DundasMap, mapFolderName, "Граница", CRHelper.MapShapeType.SublingAreas);
            FillMapData();

            #endregion

            GridHeader.Text = String.Format("Средняя розничная цена на товар «{0}», рублей за {1}", ComboFood.SelectedValue, GetUnit().ToLower());

            UltraWebGrid.Rows[0].Cells[4].Value = " ";
            UltraWebGrid.Rows[1].Cells[4].Value = " ";


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


        Node SetSelectLastNode(Node n)
        {
            if (n.Nodes.Count > 0)
            {
                return SetSelectLastNode(n.Nodes[0]);
            }
            return n;
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

            ComboDate.SetСheckedState(SetSelectLastNode(ComboDate.SelectedNode).Text,true);
            ComboDateCompare.SetСheckedState(SetSelectLastNode( ComboDateCompare.SelectedNode).Text,true);

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
            selectedDate.Value = StringToMDXDate(
                //node.Text
                  GetLastDateNode().Text
                );
            noMapData = false;
            Node prevNode = null;
            if (node.PrevNode != null)
            {
                prevNode = node.PrevNode;
            }
            else if (node.Parent.PrevNode != null)
            {
                prevNode = node.Parent.PrevNode.Nodes[node.Parent.PrevNode.Nodes.Count - 1];
            }
            else if (node.Parent.Parent.PrevNode != null)
            {
                prevNode = combo.GetLastChild(combo.GetLastChild(node.Parent.Parent));

            }
            if (prevNode != null)
            {
                previousDate.Value = StringToMDXDate( GetCompareDateNode().Text);
            }
            else
            {
                previousDate.Value = StringToMDXDate(ReplaceMonth(
                    GetCompareDateNode().Text
                    //node.Text
                    ));
                noMapData = true;
            }
            Node yearNode = GetLastDateNode().Parent.Parent.FirstNode.FirstNode;
            yearDate.Value = StringToMDXDate(yearNode.Text);

            selectedDate.Value = StringToMDXDate(GetLastDateNode().Text);
            previousDate.Value = StringToMDXDate(GetCompareDateNode().Text);

            PageTitle.Text = PageTitleCaption;
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(PageSubTitleCaption, GetLastDateNode().Text);

            //previousDate.Value = StringToMDXDate(ComboDateCompare.SelectedValue);
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

        void ClearemptyRow(DataTable Table)
        {
            List<DataRow> EmptyRow = new List<DataRow>();

            foreach (DataRow Row in Table.Rows)
            {
                bool NoEmptyRow = false;
                for (int i = 1; i < Table.Columns.Count; i++)
                {
                    if (Row[i] != DBNull.Value)
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

        private void DataBindGrid()
        {
            string query = DataProvider.GetQueryText("ORG_0001_0006_grid_part1");
            DataTable dtData1 = new DataTable();

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtData1);
            query = DataProvider.GetQueryText("ORG_0001_0006_grid_part2");
            DataTable dtData2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtData2);

            dtGrid = new DataTable();
            FillTableData(dtGrid, dtData1, dtData2);
            //ClearemptyRow(dtGrid);
            query = DataProvider.GetQueryText("RFTable");
            dtData2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtData2);

            DataRow RFRow = dtGrid.NewRow();
            dtGrid.Rows.Add(RFRow);

            RFRow[0] = "Российская Федерация";
            try
            {

                RFRow[2] = dtData2.Rows[0][1];
                RFRow[1] = dtData2.Rows[1][1];                
                RFRow[3] = dtData2.Rows[2][1];
            }
            catch { }

            try 
            {
                RFRow[5] = Minus(RFRow[3], RFRow[1]);
                RFRow[6] = Grow(RFRow[3], RFRow[1]);
                RFRow[7] = Minus(RFRow[3], RFRow[2]);
                RFRow[8] = Grow(RFRow[3], RFRow[2]); 
            }
            catch { }

            

            UltraWebGrid.DataSource = SortTable(dtGrid, "RN");
            //UltraWebGrid.DataSource = null;          

            
            UltraWebGrid.DataBind();
        }

        void ConfHeaderGrid()
        {
             UltraWebGrid.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(270);
            int columnWidth = 110;
            for (int i = 1; i < UltraWebGrid.Bands[0].Columns.Count; ++i)
            {
                UltraWebGrid.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                UltraWebGrid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                UltraWebGrid.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                UltraWebGrid.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
                CRHelper.FormatNumberColumn(UltraWebGrid.Bands[0].Columns[i], "N2");
            }
            CRHelper.FormatNumberColumn(UltraWebGrid.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(UltraWebGrid.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(UltraWebGrid.Bands[0].Columns[8], "P2");
            UltraWebGrid.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(50);

            // Заголовки
            GridHeaderCell header;
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.Wrap = true;
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            headerLayout.AddCell("Наименование МО");
            header = headerLayout.AddCell("Средняя розничная цена, рубль");
            header.AddCell(String.Format("{0}", yearDateText)); 
            header.AddCell(String.Format("{0}", previousDateText));
            header.AddCell(String.Format("{0}", selectedDateText));
            headerLayout.AddCell("Ранг");

            header = headerLayout.AddCell("Динамика за период с начала года");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");


            header = headerLayout.AddCell("Динамика к выбранному периоду");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            headerLayout.ApplyHeaderInfo();
        }


        private string SetStarChar(string RegionName)
        {
            return RegionName;
            string NameRegion = RegionName;
            return NameRegion;

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
            e.Row.Cells[5].Title = String.Format("Изменение в руб. к {0}", yearDateText);
            e.Row.Cells[6].Title = String.Format("Изменение в % к {0}", yearDateText);
            e.Row.Cells[7].Title = String.Format("Изменение в руб. к {0}", previousDateText);
            e.Row.Cells[8].Title = String.Format("Изменение в % к {0}", previousDateText);
            try
            {
                e.Row.Cells[0].Text = SetStarChar(e.Row.Cells[0].Text);
            }
            catch { }

            if (e.Row.Index < 2)
            {
                e.Row.Style.Font.Bold = true;

            }

            for (int i = 4; i < UltraWebGrid.Columns.Count; i++)
            {
                if (e.Row.Cells[i].Value == null)
                {
                    e.Row.Cells[i].Value = "-";
                }
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
                    if (Row.Cells[3].Value != null)
                    {
                        if (i > 1)
                            Ranger.AddItem(Row.Cells[3].ToString(), Convert.ToDecimal(Row.Cells[3].Value));
                    }
                    i++;

                }
                catch { }
            }

            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {
                try
                {
                    if (Row.Cells[3].Value != null)
                    {
                        int Rank = (int)Ranger.GetRang(Row.Cells[3].ToString());
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

                row[5] = Minus(row[3], row[1]);
                row[6] = Grow(row[3], row[1]);
                row[7] = Minus(row[3], row[2]);
                row[8] = Grow(row[3], row[2]);

                dtGrid.Rows.Add(row);
            }

            foreach (DataRow dataRow in dtData2.Rows)
            {
                DataRow row = dtGrid.NewRow();
                row[0] = dataRow[0];
                row[1] = dataRow[1];
                row[2] = dataRow[2];
                row[3] = dataRow[3];

                row[5] = Minus(row[3], row[1]);
                row[6] = Grow(row[3], row[1]);
                row[7] = Minus(row[3], row[2]);
                row[8] = Grow(row[3], row[2]);

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
        //---------------------------------------------------------------------

        class SortDataRowRegion : System.Collections.Generic.IComparer<DataRow>
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

                if (Xname == "Ямало-Ненецкий автономный округ")
                {
                    return 1;
                }

                if (Yname == "Ямало-Ненецкий автономный округ")
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

        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return Compare__(x, y);
            }

            public int Compare__(DataRow x, DataRow y)
            {
                try
                {
                    decimal X = (decimal)x[1];
                    decimal Y = (decimal)y[1];

                    if (X > Y)
                    {
                        return 1;
                    }

                    if (X < Y)
                    {
                        return -1;
                    }
                }
                catch { }

                return 0;

            }

            #endregion
        }


        // --------------------------------------------------------------------
        DataTable SortTable(DataTable Table, string type)
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

            if (type == "RN")
            {
                LR.Sort(new SortDataRowRegion());
            }
            else
            {
                LR.Sort(new SortDataRow());
            }

            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);
            }
            return TableSort;
        }

        //--------------------------------------------------------------------

        #region Обработчики диаграммы


        void InsertMedian()
        {

        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            
            DataTable TableChart = new DataTable();

            TableChart.Columns.Add("NameRegion");
            TableChart.Columns.Add("Value", typeof(decimal));

            int AvgINdex = 0;
            decimal sum = 0;

            double minValue = Double.PositiveInfinity; ;
            double maxValue = Double.NegativeInfinity;

            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {
                
                if (!(

                    Row.Cells[0].Text.Contains("Ямало-Ненецкий автономный округ") ||
                    Row.Cells[0].Text.Contains("Российская Федерация")))
                {
                    try
                    {
                        decimal val = decimal.Parse(Row.Cells[3].ToString());
                        string NameRegion =
                            Row.Cells[0].Text.Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
                        DataRow RowChart = TableChart.NewRow();
                        RowChart["NameRegion"] = NameRegion;
                        RowChart["Value"] = val;
                        TableChart.Rows.Add(RowChart);
                        AvgINdex++;
                        sum += (decimal)val;

                        minValue = (double)val < minValue ? (double)val : minValue;
                        maxValue = (double)val > maxValue ? (double)val : maxValue;
                    }
                    catch
                    { }
                }
            }

            DataRow AvgRow = TableChart.NewRow();
            AvgRow[0] = "Среднее";
            AvgRow[1] = sum / AvgINdex;
            TableChart.Rows.Add(AvgRow);

            int CountRow = TableChart.Rows.Count;

            TableChart = SortTable(TableChart, "NeRN");

            decimal MedianVal = (decimal)TableChart.Rows[CountRow / 2 - 1][1] / 2 + (decimal)TableChart.Rows[CountRow / 2][1] / 2;

            DataRow medianRow = TableChart.NewRow();
            medianRow[0] = "Медиана";
            medianRow[1] = MedianVal;

            TableChart.Rows.Add(medianRow);

            UltraChart1.DataSource = SortTable(TableChart, "NeRN");

            LabelChart1.Text = String.Format(ChartTitleCaption, ComboFood.SelectedValue, GetUnit().ToLower(), selectedDateText);

            double RFVal = (double)(sum / AvgINdex);
            if (UltraWebGrid.Rows[0].Cells[3].Value != null)
            {
                RFVal = (double)UltraWebGrid.Rows[0].Cells[3].Value;
                maxValue = maxValue > RFVal ? maxValue : RFVal;
                minValue = minValue < RFVal ? minValue : RFVal;
            }



            if (!Double.IsPositiveInfinity(minValue) && !Double.IsNegativeInfinity(maxValue))
            {
                UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                UltraChart1.Axis.Y.RangeMax = maxValue * 1.1;
                UltraChart1.Axis.Y.RangeMin = minValue / 1.1;
            }




            return;

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

                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                double RFVal = -666;
                if (UltraWebGrid.Rows[0].Cells[3].Value != null)
                {
                    RFVal = (double)UltraWebGrid.Rows[0].Cells[3].Value;
                }
                double XMAOVal = (double)UltraWebGrid.Rows[1].Cells[3].Value;

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
                            string.Format("Ямало-Ненецкий автономный округ -{0:### ### ##0.##}, рублей за {1}", XMAOVal, GetUnit().ToLower()),
                            e,
                            HMAOUp);
                }
            }
            catch { }
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
            if (noMapData && cbDelta.Checked)
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
            legend.Title = cbPrice.Checked ? "Средняя розничная\nцена на товар,\nрубль" : "Изменение цены\nк предыдущей дате,\nрубль";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            if (cbPrice.Checked)
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
        string GenShortNameRegion(string NameRegion)
        {
            //if (NameRegion[0] != 'Г')
            //{
            //    return NameRegion.Split(' ')[0] + " р-н";
            //}
            //else
            //{
            //    return NameRegion.Replace("Город", "г.");
            //}
            return NameRegion;
        }
         
        public Shape FindMapShape(MapControl map, string patternValue)
        {
            foreach (Shape shape in map.Shapes)
            {

                if (GenShortNameRegion(shape.Name.ToLower()).Replace(" ", "") == GenShortNameRegion(patternValue.ToLower()).Replace(" ", ""))
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
            if (cbPrice.Checked)
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
                int index = cbPrice.Checked ? 3 : 7;
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
                shape.Text = String.Format("{0}\n{1:N2}", shapeName, value);//.Replace(" ", "\n")
                if (cbPrice.Checked)
                {
                    shape.ToolTip = String.Format(shapeHint, subject, row.Cells[3].Value, row.Cells[4].Value);
                }
                else
                {
                    if (row.Cells[index].Value != null)
                    {
                        shape.ToolTip = String.Format(shapeHint, subject, MDXDateToShortDateString(previousDate.Value), value);
                    }
                    else
                    {
                        shape.ToolTip = String.Format("{0}" + valueSeparator + "В настоящий момент данные отсутствуют.", subject);
                        shape.Text = String.Format("{0}", shapeName.Replace(" ", "\n"));
                    }
                    shape.Color = value > 0 ? Color.Red : Color.Green;
                }
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Заполнение словарей и выпадающих списков параметров

        protected string GetUnit()
        {
            DataTable dtUnit = new DataTable();
            string query = DataProvider.GetQueryText("ORG_0001_0006_unit");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtUnit);
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
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < (combo == ComboDateCompare? dtDate.Rows.Count-1:dtDate.Rows.Count); ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                string day = dtDate.Rows[row][4].ToString();
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
                AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
            }
            combo.FillDictionaryValues(dictDate);
            
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
                return value1 / value2 - 1;
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
