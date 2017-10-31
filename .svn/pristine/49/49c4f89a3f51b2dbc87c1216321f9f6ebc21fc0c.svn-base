using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;
using System.Text.RegularExpressions;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Core.Primitives;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.FSGS_0001_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private static DataTable dtGrid;
        private static DataTable dtChart1;
        private static GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный район/город
        private CustomParam selectedRegion;
        // выбранный продукт
        private CustomParam selectedFood;
        // первая актуальная дата
        private CustomParam firstDate;
        // выбранная дата
        private CustomParam selectedDate;
        //Кеп?
        private CustomParam selectedFirstYearDate;


        private CustomParam selectedFirstYearDateCompare;
        //
        private CustomParam selectedCompareDate;

        // территории для диаграммы
        private CustomParam regionsForChart;

        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;
        private bool onWall;
        private bool blackStyle;

        #endregion

        // --------------------------------------------------------------------

        // заголовок страницы
        private const string PageTitleCaption = "Анализ средних розничных цен на отдельные товары и услуги";
        private const string PageSubTitleCaption = "Еженедельный мониторинг средних розничных цен на отдельные товары и услуги, {0}, по состоянию на {1}";
        // заголовок для UltraChart 
        private const string Chart1TitleCaption = "Динамика средней розничной цены на товар (услугу) «{0}», рубль";

        private static string unitName = String.Empty;
        private static Dictionary<string, string> dictRegions = new Dictionary<string, string>();

        private static string food = String.Empty;

        // --------------------------------------------------------------------

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!IsSmallResolution)
            {
                ComboDate.Width = 350;
                ComboRegion.Width = 400;
                ComboDateCompare.Width = 400;
            }
            else
            {
                ComboDate.Width = 250;
                ComboRegion.Width = 350;
                ComboDateCompare.Width = 250;
            }
            ComboDate.Title = "Выберите дату";
            ComboDate.ParentSelect = true;
            ComboRegion.Title = "Территория";
            ComboRegion.ParentSelect = true;

            #region Грид

            if (!IsSmallResolution)
                UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            else
                UltraWebGrid.Width = CRHelper.GetGridWidth(750);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.4 + 10);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

            #endregion

            #region Настройка диаграммы 1

            if (!IsSmallResolution)
                UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            else
                UltraChart1.Width = CRHelper.GetChartWidth(750);
            UltraChart1.Height = 400; //CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50000001);
            LabelChart1.Width = UltraChart1.Width;

            UltraChart1.ChartType = ChartType.LineChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Axis.X.Extent = 60;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.LineChart.NullHandling = NullHandling.InterpolateSimple;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\nцена на <SERIES_LABEL> года\n<b><DATA_VALUE:N2></b>, рубль";
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Near.Value = 20;
            UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Far.Value = 20;

            UltraChart1.Legend.Location = LegendLocation.Bottom;
            if (!IsSmallResolution)
                UltraChart1.Legend.SpanPercentage = 10;
            else
                UltraChart1.Legend.SpanPercentage = 20;
            UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
            UltraChart1.Legend.Visible = true;

            #endregion

            #region Параметры

            selectedDate = UserParams.CustomParam("selected_date");
            selectedFirstYearDate = UserParams.CustomParam("selectedFirstYearDate");
            selectedFirstYearDateCompare = UserParams.CustomParam("selectedFirstYearDateCompare");
            selectedRegion = UserParams.CustomParam("selected_region");
            selectedFood = UserParams.CustomParam("selected_food");
            firstDate = UserParams.CustomParam("first_date");

            regionsForChart = UserParams.CustomParam("regions_for_chart");

            selectedCompareDate = UserParams.CustomParam("selectedCompareDate");

            #endregion

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
            #endregion

            //WallLink.Text = "Для&nbsp;видеостены";
            //WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());

        }

        // --------------------------------------------------------------------


        Node GetRootFormNode(Node node)
        {
            for (; node.Parent != null; node = node.Parent) ;
            return node;
        }
        Node GetFirstChildFromNode(Node node)
        {
            for (; node.Nodes.Count > 0; node = node.Nodes[0]) ;
            return node;
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
            String.Format(n.Text + " - " + index.ToString());

            return index;
        }

        Node GetLastNode()
        {
            return ComboDate.SelectedNode;
        }
        Node GetCompareNode()
        {
            return ComboDateCompare.SelectedNode;
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                FillComboDate("FSGS_0001_0001_list_of_dates");
                FillComboCompare("FSGS_0001_0001_list_of_dates");
                FillComboRegion("FSGS_0001_0001_list_of_regions");

                PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);
                PanelCharts.AddRefreshTarget(UltraChart1);
            }

            #region Анализ параметров

            //selectedDateText = MDXDateToShortDateString(selectedDate.Value);
            //selectedRegionText = ComboRegion.SelectedValue;
            string mdxRegionName;

            ComboDate.SetSelectedNode(GetFirstChild(ComboDate.SelectedNode), true);
            ComboDateCompare.SetSelectedNode(GetFirstChild(ComboDateCompare.SelectedNode), true);

            dictRegions.TryGetValue(ComboRegion.SelectedValue, out mdxRegionName);
            selectedRegion.Value = mdxRegionName;

            Node node = getDateSelectNode(ComboDate.SelectedNode);
            Node nodeCompare = getDateSelectNode(ComboDateCompare.SelectedNode);
            selectedDate.Value = StringToMDXDate(node.Text);

            selectedFirstYearDate.Value =
                StringToMDXDate(GetFirstChildFromNode(GetRootFormNode(node)).Text);

            selectedFirstYearDateCompare.Value =
                StringToMDXDate(GetFirstChildFromNode(GetRootFormNode(node).PrevNode).Text);


            selectedCompareDate.Value = ComboDateCompare.SelectedValue;
                
                //StringToMDXDate(nodeCompare.Text);

            //if (GetIndexNode(ComboDate.SelectedNode) <
            //    GetIndexNode(ComboDateCompare.SelectedNode))
            //{
            //    string b = selectedDate.Value;
            //    selectedDate.Value = selectedCompareDate.Value;
            //    selectedCompareDate.Value = b;
            //}
             
            #endregion

            PageTitle.Text = String.Format(PageTitleCaption, GetLastBlock(selectedRegion.Value), MDXDateToShortDateString(selectedDate.Value));
            Page.Title = PageTitle.Text;

            PageSubTitle.Text = String.Format(PageSubTitleCaption, "<b>" + ComboRegion.SelectedValue + "</b>", "<b>" + 
                //omboDate.SelectedValue 
                GetLastNode().Text
                + 
                "</b>");



            #region Настройка цветов для диаграммы

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            Color[] colors = new Color[ComboRegion.SelectedNode.Level + 1];
            switch (ComboRegion.SelectedNode.Level)
            {
                case 0:
                    {
                        colors[0] = Color.Red;
                        break;
                    };
                case 1:
                    {
                        colors[0] = Color.Blue;
                        colors[1] = Color.Red;
                        break;
                    };
                case 2:
                    {
                        colors[0] = Color.Green;
                        colors[1] = Color.Blue;
                        colors[2] = Color.Red;
                        break;
                    };
            }

            for (int i = 0; i <= ComboRegion.SelectedNode.Level; ++i)
            {
                PaintElement pe = new PaintElement();
                pe.Fill = colors[i];

                UltraChart1.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance = new LineAppearance();

                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.IconAppearance.PE = pe;

                UltraChart1.LineChart.LineAppearances.Add(lineAppearance);
            }

            #endregion

            if (!PanelCharts.IsAsyncPostBack)
            {
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                UltraWebGrid_FillSceneGraph();
                UltraWebGrid_MarkByStars();
                for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
                {
                    UltraWebGrid.Rows[i].Cells[1].RowSpan = 3;
                }
                string patternValue = selectedFood.Value;
                if (UltraWebGrid.Rows.Count > 0)
                {
                    UltraGridRow row =
                        CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, 0);
                    UltraWebGrid_ChangeRow(row);
                }
            }

            SetDynamicText(MDXDateToString(selectedDate.Value));

            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            primitiveSizeMultiplier = onWall ? 4 : 1;
            pageWidth = onWall ? 5600 : (int)Session["width_size"];
            pageHeight = onWall ? 2100 : (int)Session["height_size"];

            widthMultiplier = 1;
            if (Session["width_size"] != null && (int)Session["width_size"] != 0)
            {
                widthMultiplier = onWall ? 1.08 * 5600 / (int)Session["width_size"] : 1;
            }

            UltraChart1.Width = pageWidth - 50;
            UltraChart1.Height = 500;//pageHeight - 50;
        }

        private Node getDateSelectNode(Node node)
        {
            if (node.Level == 0)
            {
                node = ComboDate.GetLastChild(ComboDate.GetLastChild(node));
            }
            if (node.Level == 1)
            {
                node = ComboDate.GetLastChild(node);
            }
            return node;
        }

        // --------------------------------------------------------------------

        #region Обработчики грида

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            UltraWebGrid_ChangeRow(e.Row);
        }

        protected void UltraWebGrid_ChangeRow(UltraGridRow row)
        {
            if (row == null)
                return;
            selectedFood.Value = row.Cells[row.Cells.Count - 1].GetText();
            UltraChart1.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FSGS_0001_0001_grid");
            dtGrid = new DataTable();            

            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(
                query,
                "Наименование продукта", 
                dtGrid);
            

            if (dtGrid.Rows.Count > 0)
            {
                for (int i = 0; i < dtGrid.Columns.Count; ++i)
                {
                    try
                    {
                        dtGrid.Columns[i].Caption = MDXDateToShortDateString(dtGrid.Rows[0][i].ToString());
                    }
                    catch { }
                }
                dtGrid.Rows.RemoveAt(0);
                for (int i = 1; i < dtGrid.Rows.Count; ++i)
                {
                    if (dtGrid.Rows[i][dtGrid.Columns.Count - 1] == DBNull.Value)
                    {
                        dtGrid.Rows[i][dtGrid.Columns.Count - 1] = dtGrid.Rows[i - 1][dtGrid.Columns.Count - 1];
                    }
                }
                for (int row = dtGrid.Rows.Count - 3; row >= 0; row -= 3)
                {
                    bool rowHasData = false;
                    for (int column = 2; column < dtGrid.Columns.Count - 1; ++column)
                    {
                        double value;
                        rowHasData = rowHasData || Double.TryParse(dtGrid.Rows[row][column].ToString(), out value);
                    }
                    if (!rowHasData)
                    {
                        dtGrid.Rows.RemoveAt(row);
                        dtGrid.Rows.RemoveAt(row);
                        dtGrid.Rows.RemoveAt(row);
                    }
                }



                UltraWebGrid.DataSource = ClearDobleFirstDate(dtGrid);
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private DataTable ClearDobleFirstDate(DataTable dtGrid)
        {
            string firstdate = MDXDateToShortDateString(StringToMDXDate(GetFirstChildFromNode(GetFirstYear(GetLastNode())).Text));

            bool b = false;

            for (int i = 200; i < dtGrid.Columns.Count; i++)
            {
                if (dtGrid.Columns[i].Caption.Contains(firstdate))
                {
                    if (b)
                    {
                        dtGrid.Columns.Remove(dtGrid.Columns[2]);
                        return dtGrid;
                    }
                    b = true;

                }
            }
            return dtGrid;

        }





        Node GetFirstYear(Node n)
        {
            for (; n.Parent != null; n = n.Parent) ;
            return n;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];
            double k = 0.95;
            if (Browser == "Firefox")
            {
                k = 0.95;
            }
            else if (Browser == "AppleMAC-Safari")
            {
                k = 0.9;
            }
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            //e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
            e.Layout.NullTextDefault = "-";
            e.Layout.RowSelectorStyleDefault.Width = 20;
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[1].CellStyle.Wrap = true;
            double columnWidth = CRHelper.GetColumnWidth(70);
            band.Columns[0].Width = CRHelper.GetColumnWidth(145);
            for (int i = 1; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                band.Columns[i].CellStyle.Padding.Right = 5;
                band.Columns[i].CellStyle.Padding.Left = 5;
            }
            band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            // Заголовки
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            headerLayout.AddCell("Наименование продукта");
            headerLayout.AddCell("Единица измерения");
            //headerLayout.AddCell("На начало года ("+dtGrid.Columns[2].Caption.Split(';')[0]+")");


            string firstdate = MDXDateToShortDateString(StringToMDXDate(GetFirstChildFromNode(GetFirstYear( GetLastNode())).Text));
            String.Format(firstdate);

            //String.Format(firstdate);

            for (int i = 2; i < dtGrid.Columns.Count - 1; ++i)
            {
                string[] captions = dtGrid.Columns[i].Caption.Split(';');

                if (captions[0].Contains(firstdate))
                {
                    headerLayout.AddCell(string.Format("На начало года ({0})", captions[0]));
                }
                else
                {
                    headerLayout.AddCell(captions[0]);
                }
            }


            //headerLayout.AddCell("MDX имя");
            headerLayout.ApplyHeaderInfo();

            band.Columns[0].MergeCells = true;

            band.Columns[band.Columns.Count - 1].Hidden = true;
            if (band.Columns.Count > 15)
            {
                band.Columns[3].Hidden = true;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;
            e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[0];
            if (e.Row.Cells[1].Value == null)
                e.Row.Cells[1].Value = grid.Rows[e.Row.Index - 1].Cells[1].Value;
            string cellFormat = e.Row.Index % 3 == 2 ? "{0:P2}" : "{0:N2}";
            for (int i = 2; i < e.Row.Cells.Count - 1; ++i)
            {
                UltraWebGrid_SetCellHint(sender as UltraWebGrid, e.Row.Cells[i]);
                double value;
                if (Double.TryParse(e.Row.Cells[i].GetText(), out value))
                {
                    e.Row.Cells[i].Text = String.Format(cellFormat, Convert.ToDouble(e.Row.Cells[i].GetText()));
                }
            }
        }

        protected void UltraWebGrid_MarkByStars()
        {
            for (int columnIndex = 2; columnIndex < UltraWebGrid.Columns.Count - 1; ++columnIndex)
            {
                string maxValueRows = String.Empty;
                string minValueRows = String.Empty;
                double maxValue = Double.NegativeInfinity;
                double minValue = Double.PositiveInfinity;
                int rowIndex = 0;
                for (rowIndex = 2; rowIndex < dtGrid.Rows.Count; rowIndex += 3)
                {
                    DataRow row = dtGrid.Rows[rowIndex];
                    double value;
                    if (Double.TryParse(row[columnIndex].ToString(), out value))
                    {
                        if (value != 0)
                        {
                            if (value > 0)
                            {
                                if (value == maxValue)
                                {
                                    maxValueRows = maxValueRows == String.Empty ? rowIndex.ToString() : maxValueRows + " " + rowIndex.ToString();
                                }
                                if (value > maxValue)
                                {
                                    maxValue = value;
                                    maxValueRows = rowIndex.ToString();
                                }
                            }
                            if (value < 0)
                            {
                                if (value == minValue)
                                {
                                    minValueRows = minValueRows == String.Empty ? rowIndex.ToString() : minValueRows + " " + rowIndex.ToString();
                                }
                                if (value < minValue)
                                {
                                    minValue = value;
                                    minValueRows = rowIndex.ToString();
                                }
                            }
                        }
                    }
                }
                string[] rows = null;
                if (!String.IsNullOrEmpty(maxValueRows))
                {
                    rows = maxValueRows.Split(' ');
                    foreach (string row in rows)
                    {
                        rowIndex = Convert.ToInt32(row);
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starGraybb.png";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый высокий уровень тарифа";
                    }
                }
                if (!String.IsNullOrEmpty(minValueRows))
                {
                    rows = minValueRows.Split(' ');
                    foreach (string row in rows)
                    {
                        rowIndex = Convert.ToInt32(row);
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starYellowbb.png";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый низкий уровень тарифа";
                    }
                }
            }
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            //;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;
        }

        protected void UltraWebGrid_FillSceneGraph()
        {
            for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
            {

                for (int j = 2; j < UltraWebGrid.Columns.Count - 1; ++j)
                {
                    double value;
                    if (Double.TryParse(UltraWebGrid.Rows[i + 1].Cells[j].Text, out value))
                    {
                        if (value > 0)
                            SetImageFromCell(UltraWebGrid.Rows[i].Cells[j], "ArrowRedUpBB.png");
                        else
                            if (value < 0)
                                SetImageFromCell(UltraWebGrid.Rows[i].Cells[j], "ArrowGreenDownBB.png");
                    }
                }
            }

        }

        protected void UltraWebGrid_SetCellHint(UltraWebGrid grid, UltraGridCell cell)
        {
            string prevDate =
                //= MDXDateToString(selectedCompareDate.Value);
            dtGrid.Columns[cell.Column.Index - 1].Caption;

            bool cur_year = cell.Column.Header.Caption.Contains(ComboDate.SelectedNode.Parent.Parent.Text.Replace(" год", ""));


            if (cell.Column.Index > 2)
            {
                if (cell.Row.Index % 3 == 1)
                {
                    cell.Title = String.Format("Прирост к {0} г.", 
                        ComboDateCompare.SelectedValue.Contains("к началу") ? 
                            (cur_year?
                                MDXDateToString(selectedFirstYearDate.Value) : 
                                MDXDateToString(selectedFirstYearDateCompare.Value)
                                ):
                            prevDate);
                }
                else if (cell.Row.Index % 3 == 2)
                {
                    cell.Title = String.Format("Темп прироста к {0} г.", 
                        ComboDateCompare.SelectedValue.Contains("к началу") ?
                        (cur_year ?
                                MDXDateToString(selectedFirstYearDate.Value) :
                                MDXDateToString(selectedFirstYearDateCompare.Value)
                                ) : 
                        prevDate);
                }
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики диаграммы 1

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            LabelChart1.Text = String.Format(Chart1TitleCaption, GetLastBlock(selectedFood.Value));
            // Обработка параметра-территории
            string region;
            Node node = ComboRegion.SelectedNode;
            regionsForChart.Value = String.Empty;
            DataTable dtChart = new DataTable();
            dtChart.Columns.Add("Дата", typeof(string));
            while (node != null)
            {
                dtChart.Columns.Add(node.Text, typeof(double));
                dictRegions.TryGetValue(node.Text, out region);
                regionsForChart.Value = String.IsNullOrEmpty(regionsForChart.Value) ? region : regionsForChart.Value + ",\n" + region;
                node = node.Parent;
            }
            string query = DataProvider.GetQueryText("FSGS_0001_0001_chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtChart1);
            if (dtChart1 == null || dtChart1.Rows.Count == 0)
            {
                UltraChart1.DataSource = null;
                return;
            }

            for (int i = 1; i < dtChart1.Rows.Count; ++i)
            {
                try
                {
                    DataRow row = dtChart1.Rows[i];
                    DataRow newRow = dtChart.NewRow();
                    newRow[0] = MDXDateToShortDateString(row[1].ToString());
                    for (int j = 1; j < dtChart.Columns.Count; ++j)
                        newRow[j] = row[j + 1];
                    dtChart.Rows.Add(newRow);
                }
                catch { }
            }


            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.Data.SwapRowsAndColumns = true;

            for (; dtChart.Rows.Count > 32; dtChart.Rows[0].Delete()) ;

            UltraChart1.DataSource = dtChart.DefaultView;
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Text Caption = new Text();
            Caption.SetTextString("Рубль");
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -40;
            Caption.bounds.Y = 120;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
        }
        #endregion

        // --------------------------------------------------------------------

        #region Заполнение словарей и выпадающих списков параметров

        protected void FillComboRegion(string queryName)
        {
            DataTable dtRegion = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtRegion);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            if (dtRegion == null || dtRegion.Rows.Count == 0)
                throw new Exception("Нет данных для построения отчета!");
            foreach (DataRow row in dtRegion.Rows)
            {
                string levelName = row[5].ToString();
                int level = levelName == "РФ" ? 0 : levelName == "Федеральный округ" ? 1 : 2;
                AddPairToDictionary(dict, row[3].ToString(), level);
                AddPairToDictionary(dictRegions, row[3].ToString(), row[4].ToString() + ".DataMember");
            }
            ComboRegion.FillDictionaryValues(dict);
            String RegionName = RegionSettingsHelper.Instance.Name.Replace("Ямало-Ненецкий Автономный округ", "Ямало-Ненецкий автономный округ");

            if (dict.ContainsKey(RegionName))
                ComboRegion.SetСheckedState(RegionName, true);

            String.Format(RegionName);
        }

        protected void FillComboDate(string queryName)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            if (dtDate == null || dtDate.Rows.Count == 0)
                throw new Exception("Нет данных для построения отчета!");
            for (int row = 0; row < dtDate.Rows.Count; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                string day = dtDate.Rows[row][4].ToString();

                if (day.Contains("Данные месяца"))
                {
                    continue;
                }
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
                AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
                if (String.IsNullOrEmpty(firstDate.Value))
                {
                    firstDate.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
                }
            }
            ComboDate.FillDictionaryValues(dictDate);
            ComboDate.SelectLastNode();
        }

        protected void FillComboCompare(string queryName)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            dict.Add("к началу года", 0);
            dict.Add("к предыдущему периоду", 0);

            ComboDateCompare.FillDictionaryValues(dict);

            //ComboDateCompare.tit
            ComboDateCompare.Title = "Период для сравнения";
                
            return;

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            if (dtDate == null || dtDate.Rows.Count == 0)
                throw new Exception("Нет данных для построения отчета!");
            for (int row = 0; row < dtDate.Rows.Count - 1; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                string day = dtDate.Rows[row][4].ToString();

                if (day.Contains("Данные месяца"))
                {
                    continue;
                }
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
                AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
                if (String.IsNullOrEmpty(firstDate.Value))
                {
                    firstDate.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
                }
            }


            ComboDateCompare.FillDictionaryValues(dictDate);
            ComboDateCompare.SelectLastNode();

            ComboDateCompare.Title = "Выберите дату для сравнения";


        }


        string[] lastAdededPair = new string[2];
        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                lastAdededPair[0] = key;
                lastAdededPair[1] = value.ToString();
                dict.Add(key, value);
            }
        }

        protected void AddPairToDictionary(Dictionary<string, string> dict, string key, string value)
        {
            if (!dict.ContainsKey(key))
            {
                lastAdededPair[0] = key;
                lastAdededPair[1] = value.ToString(); ;
                dict.Add(key, value);
            }
        }

        #endregion

        #region Функции-полезняшки преобразования и все такое

        private static string Browser
        {
            get { return HttpContext.Current.Request.Browser.Browser; }
        }

        public string MDXDateToString(string MDXDateString)
        {
            string[] separator = { "].[" };
            string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
            string template = "{0} {1} {2} года";
            string day = dateElements[7].Replace("]", String.Empty);
            string month = CRHelper.RusMonthGenitive(CRHelper.MonthNum(dateElements[6].ToString()));
            string year = dateElements[3];
            return String.Format(template, day, month, year);
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

        public string MDXDateToShortDateString(string MDXDateString)
        {
            string[] separator = { "].[" };
            string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
            if (dateElements.Length < 8)
            {
                return MDXDateString;
            }
            string template = "{0:00}.{1:00}.{2}";
            int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
            int month = CRHelper.MonthNum(dateElements[6]);
            int year = Convert.ToInt32(dateElements[3]);
            return String.Format(template, day, month, year);
        }

        public string MDXDateToShortDateString1(string MDXDateString)
        {
            string[] separator = { "].[" };
            string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
            string template = "{0:00}.{1:00}.{2}";
            int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
            int month = CRHelper.MonthNum(dateElements[6]);
            int year = Convert.ToInt32(dateElements[3].Substring(2, 2));
            return String.Format(template, day, month, year);
        }

        public string GetLastBlock(string mdxString)
        {
            if (String.IsNullOrEmpty(mdxString))
            {
                return String.Empty;
            }
            string[] separator = { "].[" };
            string[] stringElements = mdxString.Split(separator, StringSplitOptions.None);
            return stringElements[stringElements.Length - 1].Replace("]", String.Empty);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraWebGrid grid = headerLayout.Grid;

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = (PageSubTitle.Text + "\n\n" +
                Regex.Replace(LabelDynamicText.Text.Replace("<br/>", "\n"), "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", String.Empty) + "\n").Replace("<b>", "").Replace("</b>", "");

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 70;

            foreach (UltraGridRow row in headerLayout.Grid.Rows)
            {
                if (row.Index % 3 != 0)
                {
                    row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
                    row.Cells[1].Style.BorderDetails.StyleTop = BorderStyle.None;
                }
                else
                {
                    row.Cells[0].Value = null;
                    row.Cells[1].Value = null;
                }
                if (row.Index % 3 != 2)
                {
                    row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    row.Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
                }
                else
                {
                    row.Cells[0].Value = null;
                    row.Cells[1].Value = null;
                }
            }

            headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("MDX имя"));
            grid.Columns.Remove(grid.Columns.FromKey("Уникальное имя"));

            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraWebGrid grid = headerLayout.Grid;

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "");

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма по времени");

            sheet2.PrintOptions.ScalingType = ScalingType.FitToPages;


            SetExportGridParams(grid);

            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 0;

            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.IsActiveRow())
                {
                    row.Activated = false;
                    row.Selected = false;
                }
            }

            headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("MDX имя"));
            grid.Columns.Remove(grid.Columns.FromKey("Уникальное имя"));

            ReportExcelExporter1.Export(headerLayout, sheet1, 7);

            sheet1.MergedCellsRegions.Clear();

            // Вывод динамичского текста
            sheet1.MergedCellsRegions.Add(3, 0, 3, 13);
            sheet1.MergedCellsRegions.Add(4, 0, 4, 13);
            sheet1.MergedCellsRegions.Add(5, 0, 5, 13);
            string[] separator1 = { "<br/>" };
            string[] text = LabelDynamicText.Text.Split(separator1, StringSplitOptions.None);
            for (int i = 0; i < text.Length; ++i)
            {
                sheet1.Rows[3 + i].Cells[0].Value = Regex.Replace(text[i], "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", String.Empty);
                sheet1.Rows[3 + i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
            {
                sheet1.Rows[8 + i].Height = 255;
                if (i % 3 == 0)
                {
                    sheet1.MergedCellsRegions.Add(8 + i, 0, 10 + i, 0);
                    sheet1.MergedCellsRegions.Add(8 + i, 1, 10 + i, 1);
                }
            }
            sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
            sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
            sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            //e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.4;
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



        #region Формирование динамического текста

        string GetCompareText(string s)
        {
            return (s.Contains("началу")) ? "началом года" : "предыдущим периодом";            
        }

        private void SetDynamicText(string string_date)
        {
            Dictionary<string, double> dictDown = new Dictionary<string, double>();
            Dictionary<string, double> dictUp = new Dictionary<string, double>();
            bool hasData = false;
            int columnIndex = UltraWebGrid.Columns.Count - 2;
            string head = String.Empty, percentsMore2 = String.Empty, costDown = String.Empty;
            for (int j = 2; j < UltraWebGrid.Rows.Count; j += 3)
            {
                if (UltraWebGrid.Rows[j].Cells[columnIndex].Value != null)
                {
                    double num_value;
                    if (Double.TryParse(UltraWebGrid.Rows[j].Cells[columnIndex].Text.Replace("%", String.Empty), out num_value))
                    {
                        num_value /= 100;
                        string name = "«" + GetLastBlock(UltraWebGrid.Rows[j].Cells[UltraWebGrid.Columns.Count - 1].GetText()) + "»";
                        hasData = true;
                        if (num_value != 0)
                        {
                            if (num_value > 0)
                            {
                                if (num_value > 0.02)
                                {
                                    dictUp.Add(name, num_value);
                                }
                            }
                            else
                            {
                                if (num_value < -0.02)
                                {
                                    dictDown.Add(name, num_value);
                                }
                            }
                        }
                    }
                }
            }
            if ((dictUp.Count == 0) && (dictDown.Count == 0))
            {
                if (hasData)
                {



                    head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> по сравнению с <b>{1}</b> не наблюдалось изменение розничных цен на основные товары (услуги)",
                        string_date,
                    //ComboDateCompare.SelectedValue
                    GetCompareText(GetCompareNode().Text)
                        );
                }
                else
                {
                    head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> по сравнению с <b>{1}</b>  данные об изменении розничных цен на товары (услуги) отсутствуют.",
                        string_date,
                        GetCompareText(GetCompareNode().Text));
                }
            }
            else
            {
                head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> по сравнению с <b>{1}</b> наблюдалось изменение розничных цен на товары (услуги):",
                    string_date,GetCompareText(GetCompareNode().Text));
            }
            LabelDynamicText.Text = head;
            if (dictUp.Count != 0)
            {
                string[] names = new string[dictUp.Count];
                double[] values = new double[dictUp.Count];
                dictUp.Keys.CopyTo(names, 0);
                dictUp.Values.CopyTo(values, 0);
                Array.Sort(values, names);
                percentsMore2 = String.Format("{0} (на <b>{1:P2}</b>)", names[names.Length - 1], values[names.Length - 1]);
                for (int i = names.Length - 2; i >= 0; --i)
                {
                    percentsMore2 = String.Format("{0}, {1} (на <b>{2:P2}</b>)", percentsMore2, names[i], values[i]);
                }
                LabelDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- увеличение цен более чем на 2% на товары (услуги): " + percentsMore2 + ".";
            }
            if (dictDown.Count != 0)
            {
                string[] names = new string[dictDown.Count];
                double[] values = new double[dictDown.Count];
                dictDown.Keys.CopyTo(names, 0);
                dictDown.Values.CopyTo(values, 0);
                Array.Sort(values, names);
                percentsMore2 = String.Format("{0} (на <b>{1:P2}</b>)", names[0], values[0]);
                for (int i = 1; i < names.Length; ++i)
                {
                    percentsMore2 = String.Format("{0}, {1} (на <b>{2:P2}</b>)", percentsMore2, names[i], values[i]);
                }
                LabelDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- снижение цен более чем на 2% на товары (услуги): " + percentsMore2 + ".";
            } 
            LabelDynamicText.Width = UltraWebGrid.Width;
            LabelDynamicText.Height = Unit.Empty;
        }
        #endregion
    }
}
