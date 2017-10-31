using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Core;

/**
 *  Анализ средних розничных цен на отдельные товары и услуги в разрезе территорий РФ
 */
namespace Krista.FM.Server.Dashboards.reports.FSGS_0001_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtMap;
        private GridHeaderLayout headerLayout;

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

        private CustomParam selectedRegion;
        private CustomParam selectedFood;
        private CustomParam selectedDate;
        private CustomParam previousDate;
        private CustomParam yearDate;
        // те же, но в текстовом формате (для вывода на экран, чтобы не конвертировать)
        private static string selectedDateText;
        private static string previousDateText;
        private static string yearDateText;
        private static string region;
        private static string food;
        private static string unit;

        private static string rfName; // Сюда загрузим название Российской Федерации, а то у нас в сопоставимом классификаторе с двумя пробелами

        #endregion

        // --------------------------------------------------------------------

        // заголовок страницы
        private const string PageTitleCaption = "Анализ средних розничных цен на отдельные товары и услуги в разрезе территорий РФ";
        private const string PageSubTitleCaption = "Еженедельный мониторинг средних розничных цен на отдельные товары и услуги  в разрезе территорий РФ, {0}, по состоянию на {1}";

        //Старый заголова - private const string Chart1TitleCaption = "Распределение территорий по средней розничной цене на товар «{0}», по состоянию на {1}, рублей за {2}";
        private const string Chart1TitleCaption = "Распределение территорий по средней розничной цене на товар «{0}», {2}, по состоянию на {1}";

        private const string MapTitleCaption = "Средняя розничная цена на товар «{0}», рублей за {1}";

        private static Dictionary<string, string> dictRegion = new Dictionary<string, string>();
        private static Dictionary<string, string> dictDate = new Dictionary<string, string>();
        private static Dictionary<string, string> dictFood = new Dictionary<string, string>();
        private static Dictionary<string, string> dictUnits = new Dictionary<string, string>();

        // --------------------------------------------------------------------

        private bool RF
        {
            get { return UserComboBox.getLastBlock(selectedRegion.Value).ToUpper() == rfName.ToUpper(); }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            ComboDate.Title = "Выберите дату";
            ComboDate.Width = 275 / (IsSmallResolution ? 2 : 1);
            ComboCompareDate.Width = 350 / (IsSmallResolution ? 2 : 1);
            ComboDate.ParentSelect = true;
            ComboFood.Title = "Товар";
            ComboFood.Width = 400 / (IsSmallResolution ? 2 : 1);
            ComboFood.Height = 800;

            ComboRegion.Title = "Территория";
            ComboRegion.Width = 400 / (IsSmallResolution ? 2 : 1);
            ComboRegion.ParentSelect = true;

            #region Параметры

            selectedDate = UserParams.CustomParam("selected_date");
            previousDate = UserParams.CustomParam("previous_date");
            yearDate = UserParams.CustomParam("year_date");
            selectedFood = UserParams.CustomParam("selected_food");
            selectedRegion = UserParams.CustomParam("selected_region");

            #endregion

            #region Грид

            if (!IsSmallResolution)
            {
                UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 50);
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
                UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            }
            else
            {
                UltraChart1.Width = CRHelper.GetChartWidth(752);
            }
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart1.ColumnChart.SeriesSpacing = 0;
            UltraChart1.ColumnChart.ColumnSpacing = 0;

            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;

            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.Trimming = StringTrimming.None;

            UltraChart1.Axis.X.Labels.Layout.BehaviorCollection.Add(behavior);

            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.WrapText = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.Y.Extent = 20;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.X.Margin.Far.Value = 3;
            UltraChart1.Axis.X.Margin.Near.Value = 3;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<b><DATA_VALUE:N2></b>, рубль";
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            #region Карта

            if (!IsSmallResolution)
            {
                DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            }
            else
            {
                DundasMap.Width = CRHelper.GetChartWidth(750);
            }
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #endregion
        }

        string SclonUnit(string s)
        {
            string Result = s.ToLower().Replace("пачка", "рублей за пачку").Replace("коробка", "рублей за коробку").Replace("штука", "рублей за штуку").Replace("поездка", "рублей за поездку").Replace("месяц с человека", "рублей в месяц с человека");
            if (Result == s)
            {
                return "рублей за " + s;
            }
            return Result;
        }

        


        // --------------------------------------------------------------------
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                FillComboDate(ComboDate, "FSGS_0001_0003_list_of_dates");
                FillComboCompareDate(ComboCompareDate, "FSGS_0001_0003_list_of_dates");
                FillComboFood(ComboFood, "FSGS_0001_0003_list_of_food");
                FillComboRegion(ComboRegion, "FSGS_0001_0003_list_of_regions");
            }

            #region Анализ параметров

            ComboDate.SetSelectedNode(GetFirstChild(ComboDate.SelectedNode), true);
            ComboCompareDate.SetSelectedNode(GetFirstChild(ComboCompareDate.SelectedNode), true);

            GetDates(ComboDate, selectedDate, previousDate, yearDate);
            selectedDateText = MDXDateToShortDateString(selectedDate.Value);
            previousDateText = MDXDateToShortDateString(previousDate.Value);
            yearDateText = MDXDateToShortDateString(yearDate.Value);
            string value;
            food = ComboFood.SelectedValue;
            dictFood.TryGetValue(food, out value);
            selectedFood.Value = value;
            String FoName = RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);

            if (!Page.IsPostBack)
                ComboRegion.SetСheckedState(
                    //"Уральский федеральный округ",
                    FoName,
            true);
            region = ComboRegion.SelectedValue;
            dictRegion.TryGetValue(region, out value);
            selectedRegion.Value = value;
            dictUnits.TryGetValue(food, out value);
            unit = SclonUnit(value);
            #endregion

            PageTitle.Text = PageTitleCaption;
            Page.Title = PageTitle.Text;
            //.Replace("<b>","").Replace("</b>","")
            //PageSubTitle.Text = String.Format(PageSubTitleCaption, "<b>" + ComboRegion.SelectedValue + "</b>", "<b>" + ComboDate.SelectedValue + "</b>");
            PageSubTitle.Text = String.Format(PageSubTitleCaption, "<b>" + region + "</b>", "<b>" + GetLastNodeDate().Text + "</b>");

            #region Грид

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            #endregion

            #region Диаграмма

            UltraChart1.DataBind();

            #endregion

            #region Карта

            mapFolderName = RF ? "РФ" : RegionsNamingHelper.ShortName(region);

            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Value");
            DundasMap.ShapeFields["Value"].Type = typeof(double);
            DundasMap.ShapeFields["Value"].UniqueIdentifier = false;

            SetMapSettings();
            AddMapLayer(DundasMap, mapFolderName, mapFolderName, CRHelper.MapShapeType.Areas);
            FillMapData();

            #endregion

            UltraWebGrid.DisplayLayout.AllowSortingDefault = AllowSorting.No;

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

            return index;
        }

        #region Обработка дат

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
            //selectedDate.Value = StringToMDXDate(node.Text);
            Node prevNode = null;
            if (node.PrevNode != null)
            {
                prevNode = node.PrevNode;
            }
            else if (node.Parent.PrevNode != null)
            {
                prevNode = node.Parent.PrevNode.FirstNode;
            }
            else if (node.Parent.Parent.PrevNode != null)
            {
                prevNode = combo.GetLastChild(combo.GetLastChild(node.Parent.Parent));
            }
            if (prevNode != null)
            {
                previousDate.Value = StringToMDXDate(prevNode.Text);
            }
            else
            {
                previousDate.Value = StringToMDXDate(ReplaceMonth(node.Text));
            }


            Node yearNode = node.Parent.Parent.FirstNode.FirstNode;
            yearDate.Value = StringToMDXDate(yearNode.Text); 

            selectedDate.Value = StringToMDXDate(GetFirstChild(ComboDate.SelectedNode).Text);
            previousDate.Value = StringToMDXDate(GetFirstChild(ComboCompareDate.SelectedNode).Text);


            if (GetIndexNode(ComboDate.SelectedNode) < GetIndexNode(ComboCompareDate.SelectedNode))
            {
                string b = selectedDate.Value;
                selectedDate.Value = previousDate.Value;
                previousDate.Value = b;


                yearNode = ComboCompareDate.SelectedNode.Parent.Parent.FirstNode.FirstNode;
                yearDate.Value = StringToMDXDate(yearNode.Text);
            }



        }

        Node GetComapreNodeDate()
        {
            if (GetIndexNode(ComboDate.SelectedNode) < GetIndexNode(ComboCompareDate.SelectedNode))
            {
                return ComboDate.SelectedNode;
            }
            return ComboCompareDate.SelectedNode;
            
        }

        Node GetLastNodeDate()
        {
            if (GetIndexNode(ComboDate.SelectedNode) < GetIndexNode(ComboCompareDate.SelectedNode))
            {
                return ComboCompareDate.SelectedNode;
                
            }
            return ComboDate.SelectedNode;
        }


        #endregion

        Node GetFirstChild(Node n)
        {
            for (; n.Nodes.Count > 0; n = n.Nodes[0]) ;
            return n;
        }

        // --------------------------------------------------------------------

        #region Обработчики грида
        int maxRankInRF = 0, maxRankInFO = 0;

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FSGS_0001_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование территории", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                
                    dtGrid.Columns.RemoveAt(0);
                    for (int i = 1; i < dtGrid.Rows.Count; ++i)
                        maxRankInFO =
                            maxRankInFO > Convert.ToInt32(dtGrid.Rows[i]["Ранг по ФО 2"]) ?
                                maxRankInFO :
                                Convert.ToInt32(dtGrid.Rows[i]["Ранг по ФО 2"]);
                    
                    if (RF)
                    {
                        for (int i = 1; i < dtGrid.Rows.Count; ++i)
                        {
                            DataRow row = dtGrid.Rows[i];
                            maxRankInRF = 
                                maxRankInRF > Convert.ToInt32(row["Ранг по РФ"]) ? 
                                    maxRankInRF : 
                                    Convert.ToInt32(row["Ранг по РФ"]);
                        
                        }
                    }
                    else
                    {
                        maxRankInRF = Convert.ToInt32(dtGrid.Rows[0]["Ранг по РФ"]);
                    }
                    dtGrid.Rows[0]["Ранг по ФО 2"] = DBNull.Value;
                    dtGrid.Rows[0]["Ранг по РФ"] = DBNull.Value;
                    dtGrid.Rows[1]["Ранг по ФО 2"] = DBNull.Value;
                    UltraWebGrid.DataSource = dtGrid;
                    try
                    { }
                catch { }
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }

            String.Format(maxRankInFO.ToString());
        }

        protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(265);
            int columnWidth = 85;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(50);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(50);

            // Заголовки
            GridHeaderCell header;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            headerLayout.AddCell("Территория");
            header = headerLayout.AddCell("Розничная цена, рубль");

            header.AddCell(String.Format("{0}", yearDateText));
            header.AddCell(String.Format("{0}", previousDateText));
            header.AddCell(String.Format("{0}", selectedDateText));

            header = headerLayout.AddCell("Ранг по");
            header.AddCell("РФ");
            header.AddCell("ФО");

            header = headerLayout.AddCell("Динамика за период с начала года");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");
            
            header = headerLayout.AddCell("Динамика к отчетной дате");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            

            headerLayout.ApplyHeaderInfo();
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
            UltraGridRow row = e.Row;
            if ((row.Index == 0) || ((row.Index == 1) && (!RF)))
                foreach (UltraGridCell cell in row.Cells)
                    cell.Style.Font.Bold = true;
            double value;
            if (Double.TryParse(row.Cells[7].Text, out value) && value != 0)
            {
                if (value > 0)
                    SetImageFromCell(row.Cells[7], "ArrowRedUpBB.png");
                else
                    SetImageFromCell(row.Cells[7], "ArrowGreenDownBB.png");

            }
            if (Double.TryParse(row.Cells[9].Text, out value) && value != 0)
            {
                if (value > 0)
                    //row.Cells[9].Style.CssClass = "arrowUpRedBB.png";
                    SetImageFromCell(row.Cells[9], "ArrowRedUpBB.png");
                else
                    SetImageFromCell(row.Cells[9], "ArrowGreenDownBB.png");
                //row.Cells[9].Style.CssClass = "arrowDownGreenBB.png";
            }

            #region Маркировка звездами

            int rank;
            if (Int32.TryParse(row.Cells[4].Text, out rank) && (rank == 1) && ((row.Index > 1) || ((row.Index == 1) && RF)))
            {
                row.Cells[4].Style.BackgroundImage = "~/images/starYellowbb.png";
                row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                row.Cells[4].Title = "Самый низкий уровень цены";
            }
            if (Int32.TryParse(row.Cells[4].Text, out rank) && (rank == maxRankInRF) && ((row.Index > 1) || ((row.Index == 1) && RF)))
            {
                row.Cells[4].Style.BackgroundImage = "~/images/starGraybb.png";
                row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                row.Cells[4].Title = "Самый высокий уровень цены";
            }
            if (Int32.TryParse(row.Cells[5].Text, out rank) && (rank == 1) && ((row.Index > 1) || ((row.Index == 1) && RF)))
            {
                row.Cells[5].Style.BackgroundImage = "~/images/starYellowbb.png";
                row.Cells[5].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                row.Cells[5].Title = "Самый низкий уровень цены";
            }
            if (Int32.TryParse(row.Cells[5].Text, out rank) && (rank == maxRankInFO) && ((row.Index > 1) || ((row.Index == 1) && RF)))
            {
                row.Cells[5].Style.BackgroundImage = "~/images/starGraybb.png";
                row.Cells[5].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                row.Cells[5].Title = "Самый высокий уровень цены";
            }

            #endregion

            // Хинты
            row.Cells[8].Title = String.Format("Абсолютное отклонение по отношению к {0}", previousDateText);
            row.Cells[9].Title = String.Format("Темп прироста к {0}", previousDateText);
            row.Cells[6].Title = String.Format("Абсолютное отклонение по отношению к {0}", yearDateText);
            row.Cells[7].Title = String.Format("Темп прироста к {0}", yearDateText);
        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики диаграммы
        double rfPrice, foPrice;



        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            LabelChart1.Text = String.Format(Chart1TitleCaption, food, selectedDateText, unit);
            LabelMap.Text = string.Format("Средняя розничная цена на товар «{0}», {1}, по состоянию на {2} г.", food, unit, ComboDate.SelectedValue);
            GridHeader.Text = string.Format("Средние розничные цены на товар \"{0}\", {1}", food, unit);
            string query = DataProvider.GetQueryText("FSGS_0001_0003_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart1.Data.SwapRowsAndColumns = true;
                UltraChart1.DataSource = dtChart.DefaultView;

                // установка границ оси Y
                rfPrice = Convert.ToDouble(dtGrid.Rows[0]["Цена на выбранную дату"].ToString());
                foPrice = Convert.ToDouble(dtGrid.Rows[1]["Цена на выбранную дату"].ToString());
                double minPrice = Convert.ToDouble(dtChart.Rows[0]["Цена на выбранную дату"].ToString());
                double maxPrice = Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 1]["Цена на выбранную дату"].ToString());
                double minY = rfPrice < foPrice ? (rfPrice < minPrice ? rfPrice : minPrice) : (foPrice < minPrice ? foPrice : minPrice);
                double maxY = rfPrice > foPrice ? (rfPrice > maxPrice ? rfPrice : maxPrice) : (foPrice > maxPrice ? foPrice : maxPrice);

                UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                UltraChart1.Axis.Y.RangeMin = minY * 0.9;
                UltraChart1.Axis.Y.RangeMax = maxY * 1.1;

            }
            else
            {
                UltraChart1.DataSource = null;
            }
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            try
            {
                foreach (Primitive primitive in e.SceneGraph)
                {
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text)primitive;
                        axisText.bounds.Width = 30;
                    }
                }
                UltraChart1_AddLineWithTitle(rfPrice, "Российская Федерация", Color.Red, e, foPrice);
                if (!RF)
                    UltraChart1_AddLineWithTitle(foPrice, region, Color.Blue, e, rfPrice);
            }
            catch { }
        }
        bool xz = false;
        protected void UltraChart1_AddLineWithTitle(double value, string region, Color color, FillSceneGraphEventArgs e, double otherValue)
        {
            string formatString = "{0}: {1:N2}, рубль";
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];
            Point p1 = new Point((int)xAxis.MapMinimum, (int)yAxis.Map(value));
            Point p2 = new Point((int)xAxis.MapMaximum, (int)yAxis.Map(value));
            Line line = new Line(p1, p2);
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = color;
            line.PE.StrokeWidth = 2;
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.labelStyle.Orientation = TextOrientation.Horizontal;
            text.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);//new System.Drawing.Font("Verdana", 10, FontStyle.Italic);

            text.labelStyle.HorizontalAlign = StringAlignment.Near;
            text.labelStyle.VerticalAlign = StringAlignment.Near;

            Size size = new Size(500, 15);
            Point p;
            p = new Point(p1.X + 50, p1.Y - 20);
            if (Math.Abs((yAxis.Map(value) - yAxis.Map(otherValue))) < 25)
            {

                if (yAxis.Map(otherValue) == yAxis.Map(value))
                {
                    if (xz)
                    {
                        p = new Point(p1.X + 50, p1.Y + 3);
                    }
                    else
                    {
                        p = new Point(p1.X + 50, p1.Y - 20);
                    }
                    xz = true;
                }
                else
                    if (yAxis.Map(otherValue) > yAxis.Map(value))
                    {
                        p = new Point(p1.X + 50, p1.Y - 20);

                    }
                    else
                    {
                        p = new Point(p1.X + 50, p1.Y + 3);

                    }

            }


            if (Math.Abs(yAxis.Map(value) - yAxis.MapMaximum) < 20)
            {
                //p = new Point(p1.X + 50, p1.Y - 20);
                p = new Point(p1.X + 50, p1.Y + 3);
            }
            else
            {

            }

            text.bounds = new System.Drawing.Rectangle(p, size);

            //text.labelStyle.FontColor = Color.Black;

            text.SetTextString(String.Format(formatString, region, value));

            e.SceneGraph.Add(text);
        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики карты

        protected void SetMapSettings()
        {
            DundasMap.Visible = true;
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
            legend.Title = String.Format("Розничная цена на товар\n«{0}», рубль", food);
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
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

        protected void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        string GetCompareName(string s)
        {
            return s.ToLower().Replace("автономный округ", "ао").Replace("область", "обл.").Replace("республика", "р.");
        }

        protected Shape FindMapShape(MapControl map, string patternValue)
        {
            foreach (Shape shape in map.Shapes)
            {


                if ((GetCompareName(shape.Name) == GetCompareName(patternValue))
                    ||
                    shape.Name.ToLower() == patternValue.ToLower())
                {
                    return shape;
                }
            }
            return null;
        }


        protected void FillMapData()
        {
            string valueSeparator = IsMozilla ? ". " : "\n";
            string shapeHint;
            if (RF)
                shapeHint = "{0}" + valueSeparator + "{1:N2}, рубль" + valueSeparator + "Ранг по РФ: {2:N0}";
            else
                shapeHint = "{0}" + valueSeparator + "{1:N2}, рубль" + valueSeparator + "Ранг по ФО: {2:N0}";
            //LabelMap.Text = String.Format(MapTitleCaption1, food);
            string query = DataProvider.GetQueryText("FSGS_0001_0003_map");
            dtMap = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtMap);
            if (dtMap.Rows.Count == 0)
            {
                return;
            }
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = String.Format("{0}", shape.Name);
            }
            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                string subject = row["Территория"].ToString();
                string shortName = row["Краткое наименование"].ToString().Replace(" ФО", " федеральный округ"); ;
                int rank = Convert.ToInt32(row["Ранг"]);
                double value;
                if (!Double.TryParse(row["Цена на выбранную дату"].ToString(), out value))
                {
                    value = 0;
                }
                Shape shape = FindMapShape(DundasMap, shortName);
                if (shape != null)
                {
                    shape.Visible = true;
                    string shapeName = shape.Name;

                    shape["Name"] = subject;
                    shape["Value"] = value;
                    shape.TextVisibility = TextVisibility.Shown;
                    shape.Text = String.Format("{0}\n{1:N2}", shapeName.Replace(" ", "\n"), value);
                    shape.ToolTip = String.Format(shapeHint, subject, value, rank);
                }
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Заполнение словарей и выпадающих списков параметров

        protected void FillComboFood(CustomMultiCombo combo, string queryName)
        {
            DataTable dtFood = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Товар или услуга", dtFood);
            if (dtFood.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (DataRow row in dtFood.Rows)
            {
                string food = row["Товар или услуга"].ToString();
                AddPairToDictionary(dict, food, 0);
                AddPairToDictionary(dictFood, food, row["MDX имя"].ToString());
                AddPairToDictionary(dictUnits, food, row["Единица измерения"].ToString().ToLower().Replace("илограмм", "г."));
            }
            combo.FillDictionaryValues(dict);
        }

        protected void FillComboRegion(CustomMultiCombo combo, string queryName)
        {
            DataTable dtData = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtData);
            if (dtData.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (DataRow row in dtData.Rows)
            {
                string region = row["Территория"].ToString();
                string levelName = row["Уровень"].ToString();
                int level = levelName == "РФ" ? 0 : 1;
                if (levelName == "РФ")
                    rfName = UserComboBox.getLastBlock(row["MDX имя"].ToString());
                AddPairToDictionary(dict, region, level);
                AddPairToDictionary(dictRegion, region, row["MDX имя"].ToString());
            }
            combo.FillDictionaryValues(dict);
        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (DataRow row in dtDate.Rows)
            {
                string year = row["Год"].ToString();
                string month = row["Месяц"].ToString();
                string day = row["День"].ToString();
                AddPairToDictionary(dict, year + " год", 0);
                AddPairToDictionary(dict, month + " " + year + " года", 1);
                AddPairToDictionary(dict, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
                AddPairToDictionary(dictDate, day + " " +
                    CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) +
                    ' ' + year +
                    " года", row["MDX имя"].ToString());
            }
            combo.FillDictionaryValues(dict);
            combo.SelectLastNode();
        }

        protected void FillComboCompareDate(CustomMultiCombo combo, string queryName)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < dtDate.Rows.Count - 1; i++)
            {
                DataRow row = dtDate.Rows[i];

                string year = row["Год"].ToString();
                string month = row["Месяц"].ToString();
                string day = row["День"].ToString();
                AddPairToDictionary(dict, year + " год", 0);
                AddPairToDictionary(dict, month + " " + year + " года", 1);
                AddPairToDictionary(dict, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
                AddPairToDictionary(dictDate, day + " " +
                    CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) +
                    ' ' + year +
                    " года", row["MDX имя"].ToString());
            }
            combo.FillDictionaryValues(dict);
            combo.SelectLastNode();
            combo.Title = "Выберите дату для сравнения";

        }

        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        protected void AddPairToDictionary(Dictionary<string, string> dict, string key, string value)
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
            string template = "[Период].[Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
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
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "");

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(headerLayout, "\n" + GridHeader.Text, section1);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            ReportPDFExporter1.Export(
                UltraChart1,
                LabelChart1.Text.Replace("<b>", "").Replace("</b>", ""),
                section2);

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            ReportPDFExporter1.Export(DundasMap, LabelMap.Text.Replace("<b>", "").Replace("</b>", ""), section3);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "").Replace("&nbsp", "");

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

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet1.Rows[1].Height = 550;

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text.Replace("<b>", "").Replace("</b>", ""), sheet2, 1);
            sheet2.MergedCellsRegions.Clear();
            sheet2.MergedCellsRegions.Add(0, 0, 0, 18);
            sheet2.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet2.Rows[0].Height = 550;
            sheet2.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);

            sheet3.Rows[0].Cells[0].Value = LabelMap.Text.Replace("<b>", "").Replace("</b>", "");
            sheet3.Rows[0].Cells[0].CellFormat.Font.Name = sheet2.Rows[0].Cells[0].CellFormat.Font.Name;
            sheet3.Rows[0].Cells[0].CellFormat.Font.Height = sheet2.Rows[0].Cells[0].CellFormat.Font.Height;
            sheet3.Rows[0].Cells[0].CellFormat.Font.Bold = sheet2.Rows[0].Cells[0].CellFormat.Font.Bold;


            sheet1.Rows[2].Cells[0].Value = GridHeader.Text.Replace("<b>", "").Replace("</b>", "");
            sheet1.Rows[2].Cells[0].CellFormat.Font.Name = sheet2.Rows[0].Cells[0].CellFormat.Font.Name;
            sheet1.Rows[2].Cells[0].CellFormat.Font.Height = sheet2.Rows[0].Cells[0].CellFormat.Font.Height;
            sheet1.Rows[2].Cells[0].CellFormat.Font.Bold = sheet2.Rows[0].Cells[0].CellFormat.Font.Bold;

            ReportExcelExporter.MapExcelExport(sheet3.Rows[1].Cells[0], DundasMap);
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
