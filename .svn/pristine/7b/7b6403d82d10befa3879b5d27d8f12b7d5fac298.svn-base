using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0002_0004
{
    public partial class Default : CustomReportPage
    {
        #region Поля и свойства

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid, dtGridHiddenFields;
        private GridHeaderLayout headerLayout;
        private DataTable dtComments;

        private string Item;
        private string Code;

        /// <summary>
        /// Выбраны ли 
        /// федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return regionsCombo.SelectedIndex == 0; }
        }

        /// <summary>
        /// Месячная отчетность
        /// </summary>
        public bool IsMonthReprt
        {
            get { return RadioButtonList1.SelectedIndex == 0; }
        }
        
        #endregion

        #region Параметры запроса

        // выбранный варинат
        private CustomParam bkkuVariant;
        // выбранный варинат
        private CustomParam selectedIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);// - 675);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.30);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);// - 600);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.45);//818);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N0>";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.Axis.X.Extent = 160;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 15;
            UltraChart.Legend.Margins.Bottom = (int)(UltraChart.Height.Value / 2);
            UltraChart.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Margin.Near.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 2;
            UltraChart.Axis.Y.TickmarkStyle = AxisTickStyle.DataInterval;
            UltraChart.Axis.Y.TickmarkInterval = 1;

            EmptyAppearance item = new EmptyAppearance();
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            LineStyle style = new LineStyle();
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart.LineChart.EmptyStyles.Add(item);
            UltraChart.LineChart.NullHandling = NullHandling.InterpolateCustom;

            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InterpolateValues += new InterpolateValuesEventHandler(UltraChart_InterpolateValues);

            LineAppearance lineAppearance3 = new LineAppearance();
            lineAppearance3.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance3.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance3.Thickness = 3;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance3);

            LineAppearance lineAppearance1 = new LineAppearance();

            lineAppearance1.Thickness = 0;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance1);

            LineAppearance lineAppearance2 = new LineAppearance();
            lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance2.Thickness = 0;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance2);

            UltraChart.Data.SwapRowsAndColumns = true;

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            CrossLink1.Text = "Мониторинг&nbsp;соблюдения&nbsp;требований&nbsp;БК&nbsp;и&nbsp;КУ";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0002_0002/Default.aspx";
            CrossLink2.Text = "Индикаторы&nbsp;БК&nbsp;И&nbsp;КУ";
            CrossLink2.NavigateUrl = "~/reports/MFRF_0002_0003/Default.aspx";

            bkkuVariant = UserParams.CustomParam("bkku_Variant");
            selectedIndicator = UserParams.CustomParam("selected_indicator");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
            }

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddRefreshTarget(ChartCaption);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0002_0004_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                regionsCombo.Width = 300;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    regionsCombo.SetСheckedState(RegionSettings.Instance.Name, true);
                }

            }

            string firstyear;
            if (IsMonthReprt)
            {
                bkkuVariant.Value = "По данным месячной отчетности";
                firstyear = "2005";
            }
            else
            {
                bkkuVariant.Value = "По данным годовой отчетности";
                firstyear = "2006";
            }


            Page.Title = string.Format("Динамика индикаторов БК и КУ ({0}) ", regionsCombo.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("<br/>Данные Минфина РФ за период  {2} - {0} ({1})", IsMonthReprt ? "2009 гг." : "2011 гг.", bkkuVariant.Value.ToLower(), firstyear);
            //Динамика индикаторов БК и КУ (ДФО) Амурская область
            //Данные Минфина РФ за период c 2000 - 4 квартал 2009 года (по данным месячной отчетности)

            UserParams.PeriodYear.Value = "2008";
            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;


            UltraWebGrid.Bands.Clear();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
            UltraChart.DataBind();
            if (!Page.IsPostBack)
            {
                selectedIndicator.Value = UltraWebGrid.Rows[1].Cells[0].Value.ToString();
            }
            string patternValue = UserParams.StateArea.Value;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = selectedIndicator.Value;
                defaultRowIndex = 0;
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            Code = "";
            Item = "";
            if (row == null)
                return;
            if (row.Index < 1 &&
                chartWebAsyncPanel.IsAsyncPostBack)
            {
                //  Response.End();
                return;
            }

            selectedIndicator.Value = row.Cells[0].Value.ToString();
            if (row.Cells[row.Cells.Count - 1].Value.ToString() == "Группа, присвоенная субъекту РФ по доле МБТ")
                return;
            GetIndicatorComments(row.Cells[0].Value.ToString());

            if (Item == "тыс. руб")
            {
                UltraChart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
                UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
                UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N3>";
            }
            else if (Item == "доля")
            {
                UltraChart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
                UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N3>";
                UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N3>";
            }
            ChartCaption.Text = "Динамика значения индикатора " + Code.ToString();
            ChartComment.Text = row.Cells[0].Value.ToString() + " " + GetIndicatorComments(row.Cells[0].Value.ToString());

            UltraChart.DataBind();
        }
        
        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            if (IsMonthReprt)
            {
                string query = DataProvider.GetQueryText("MFRF_0002_0004_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Индикатор БК и КУ", dtGrid);

                query = DataProvider.GetQueryText("MFRF_0002_0004_compare_GridYear");
                DataTable dtGrid1 = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Индикатор БК и КУ", dtGrid1);

                string queryHiddenFields = DataProvider.GetQueryText("MFRF_0002_0004_compare_Grid_HiddenFields");
                dtGridHiddenFields = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(queryHiddenFields, "Индикатор БК и КУ", dtGridHiddenFields);
            }
            else
            {
                string query = DataProvider.GetQueryText("MFRF_0002_0004_compare_Grid1");
                dtGrid = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Индикатор БК и КУ", dtGrid);

                string queryHiddenFields = DataProvider.GetQueryText("MFRF_0002_0004_compare_Grid_HiddenFields1");
                dtGridHiddenFields = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(queryHiddenFields, "Индикатор БК и КУ", dtGridHiddenFields);
            }

            dtGridHiddenFields.Columns[4].DataType = typeof(string);

            string FoArrow = "";
            string RfArrow = "";
            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                if (dtGrid.Rows[i][0].ToString() != "Группа, присвоенная субъекту РФ по доле МБТ")
                {
                    int k = 4;
                    for (int j = 2; j < dtGrid.Columns.Count - 1; j = j + 2)
                    {
                        if (dtGridHiddenFields.Rows[i][j] == DBNull.Value)
                        {
                            dtGrid.Rows[i][j - 1] = DBNull.Value;
                        }
                        if ((dtGridHiddenFields.Rows[i][k - 3] != DBNull.Value) || (dtGridHiddenFields.Rows[i][k - 2] != DBNull.Value))
                        {
                            if (dtGrid.Rows[i][j] != DBNull.Value)
                            {
                                FoArrow = "";
                                RfArrow = "";
                            }
                            dtGridHiddenFields.Rows[i][k] = string.Format("{1} = {0:N3} {2}", dtGridHiddenFields.Rows[i][k - 3], RegionsNamingHelper.ShortName(regionsCombo.SelectedNodeParent.ToString()), FoArrow) + "<br/>" + string.Format("РФ = {0:N3} {1}", dtGridHiddenFields.Rows[i][k - 2], RfArrow);
                        }

                        k = k + 4;
                    }
                    k = 4;
                    for (int j = 1; j < dtGrid.Columns.Count - 1; j = j + 2)
                    {
                        dtGrid.Rows[0][j] = DBNull.Value;
                        dtGridHiddenFields.Rows[0][k] = DBNull.Value;
                        k = k + 4;
                    }
                    string comment = "";
                    string IndicatorWithCode = DataDictionariesHelper.GetIndicator(String.Format("[МФ РФ].[Сопоставимый индикаторы БККУ].[Все индикаторы].[{0}]", dtGrid.Rows[i][0].ToString()));
                    if (IndicatorWithCode == "")
                    {
                        IndicatorWithCode = dtGrid.Rows[i][0].ToString();
                    }
                    dtGrid.Rows[i][dtGrid.Columns.Count - 1] = string.Format("{0}<br/>{1}", IndicatorWithCode, comment);
                }
                else
                {

                    dtGrid.Rows[i][dtGrid.Columns.Count - 1] = dtGrid.Rows[i][0];

                }
            }
            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (dtGrid.Rows.Count < 15)
            {
                    UltraWebGrid.Height = Unit.Empty;
            }
        }
        
        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[i].Columns[0].Header.RowLayoutColumnInfo.OriginY = 1;
                    e.Layout.Bands[i].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[i].Columns[0].Header.RowLayoutColumnInfo.OriginY = 2;
                }
            }
            e.Layout.Bands[0].Columns[0].Width = 400;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            
            headerLayout.AddCell("Индикаторы БК и КУ", "Индикаторы соблюдения требованиям Бюджетного кодекса и качества управления бюджетами субъектов РФ");

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[i + 1].CellStyle.Padding.Right = 5;

                e.Layout.Grid.Columns[i].HeaderStyle.Height = 40;
                e.Layout.Grid.Columns[i + 1].HeaderStyle.Height = 40;
                e.Layout.Bands[0].Columns[i].Width = 85;
                e.Layout.Bands[0].Columns[i + 1].Width = 80;
            }

            int Year = 2004;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell;
                if (bkkuVariant.Value == "По данным месячной отчетности")
                {
                    if (captions[0] == "Квартал 1")
                    {
                        Year++;
                    }
                    cell = headerLayout.AddCell(captions[0] + " " + "(" + Year.ToString() + " год" + ")");
                }
                else
                {
                    cell = headerLayout.AddCell(captions[0] + " год");
                }
                cell.AddCell("Нормативное значение", "Нормативное значение, установленное Приказом Минфина  «О мониторинге БК и КУ»");
                cell.AddCell("Значение", "Значение индикатора");
            }

            headerLayout.ApplyHeaderInfo();
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int k = 3;
            for (int i = 2; i < e.Row.Cells.Count - 1; i = i + 2)
            {
                if ((dtGridHiddenFields.Rows[e.Row.Index][k].ToString() != "Cell") & (e.Row.Index > 2))
                {
                    object obj = dtGridHiddenFields.Rows[e.Row.Index][k];
                    if (obj != DBNull.Value)
                    {
                        int value = Convert.ToInt32(obj);
                        if (value == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие");
                        }
                        else if (value == 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие");
                        }

                    }
                }
                if (e.Row.Index != 0)
                {
                    GetIndicatorComments(e.Row.Cells[0].ToString());
                    string Format = "{0:N0}";
                    if (Item == "тыс. руб")
                        Format = "{0:N1}";
                    else if (Item == "доля")
                        Format = "{0:N3}";

                    double value_double = Convert.ToDouble(e.Row.Cells[i].Value);
                    e.Row.Cells[i].Value = string.Format(Format, value_double);

                    value_double = Convert.ToDouble(e.Row.Cells[i - 1].Value);
                    e.Row.Cells[i - 1].Value = string.Format(Format, value_double);
                }
                k = k + 4;
                e.Row.Cells[i].Style.CustomRules =
                       "background-repeat: no-repeat; background-position: right top; margin: 0px";
            }
        }
        
        #endregion

        #region Обработчики диаграмы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            if (selectedIndicator.Value == "Группа, присвоенная субъекту РФ по доле МБТ")
            {
                return;
            }
            DataTable dtChart = new DataTable();
            if (IsMonthReprt)
            {
                string queryName = "Chart_query";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
                queryName = "Chart_yearquery";
                query = DataProvider.GetQueryText(queryName);
                DataTable dtYearChart = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtYearChart);
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {

                    try
                    {
                        dtChart.Columns[i].ColumnName = dtYearChart.Rows[0][i].ToString();
                    }
                    catch
                    { }

                }
            }
            else
            {
                string queryName = "Chart_query1";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

                for (int i = 1; i < dtChart.Columns.Count; i++)
                {

                    dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName + " год";
                }
            }
            UltraChart.Data.SwapRowsAndColumns = false;
            UltraChart.DataSource = dtChart;

        }


        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
            for (int i = 0; i < e.NullValueIndices.Length; i++)
            {
                e.Values.SetValue(Double.MinValue, e.NullValueIndices[i]);
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int count = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet ps = primitive as PointSet;
                    for (int j = 0; j < ps.points.Length; j++)
                    {
                        if (ps.points[j].Row == 0)
                        {

                        }

                        if (ps.points[j].Row == 1)
                        {
                            ps.points[j].Visible = false;
                            DrawBox(e, ps.points[j].point, Color.Green);
                        }

                        if (ps.points[j].Row == 2)
                        {
                            ps.points[j].Visible = false;
                            DrawBox(e, ps.points[j].point, Color.Red);
                        }
                    }
                }

                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Polyline")
                {
                    primitive.PE.Fill = Color.Blue;
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (!(string.IsNullOrEmpty(box.Path)) && box.Path.EndsWith("Legend") &&
                        box.rect.Width == box.rect.Height)
                    {
                        Color color = Color.Aqua;
                        count++;

                        switch (count)
                        {
                            case 1:
                                {
                                    color = Color.Blue;
                                    break;
                                }
                            case 2:
                                {
                                    color = Color.Green;
                                    break;
                                }
                            case 3:
                                {
                                    color = Color.Red;
                                    break;
                                }
                        }

                        Box box1 = new Box(box.rect);
                        box1.PE.ElementType = PaintElementType.Gradient;
                        box1.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                        box1.PE.Fill = color;
                        box1.PE.FillStopColor = color;
                        box1.PE.Stroke = Color.Black;
                        box1.PE.StrokeWidth = 1;
                        box1.Row = 0;
                        box1.Column = 2;
                        box1.Value = 42;
                        box1.Layer = e.ChartCore.GetChartLayer();
                        box1.Chart = this.UltraChart.ChartType;
                        e.SceneGraph.Add(box1);

                    }
                }
            }
        }

        private void DrawBox(FillSceneGraphEventArgs e, Point p, Color color)
        {

            Box box = new Box(new Point(p.X - 6, p.Y - 6), 13, 13);

            box.PE.ElementType = PaintElementType.Gradient;
            box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
            box.PE.Fill = color;
            box.PE.FillStopColor = color;
            box.PE.Stroke = Color.Black;
            box.PE.StrokeWidth = 1;
            box.Row = 0;
            box.Column = 2;
            box.Value = 42;
            box.Layer = e.ChartCore.GetChartLayer();
            box.Chart = this.UltraChart.ChartType;
            e.SceneGraph.Add(box);
        }

        #endregion

        #region Export excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            sheet1.Rows[0].Cells[0].Value = Label1.Text;
            sheet1.Rows[1].Cells[0].Value = Label2.Text.Replace("<br/>", " ");
            sheet2.Rows[0].Cells[0].Value = Label1.Text;
            sheet2.Rows[1].Cells[0].Value = Label2.Text.Replace("<br/>", " ");


            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, sheet2, 4);

        }
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;

            ReportPDFExporter1.Export(headerLayout, Label2.Text.Replace("<br/>", " "), section1);

            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = font;
            title.AddContent(Label1.Text);

            MemoryStream imageStream = new MemoryStream();
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.SaveTo(imageStream, ImageFormat.Png);
            Bitmap bm2 = new Bitmap(imageStream);


            ReportPDFExporter1.Export(bm2, Label2.Text.Replace("<br/>", "\n"), section2);
        }
        #endregion

        private string GetIndicatorComments(string name)
        {
            if (dtComments == null || dtComments.Columns.Count == 0)
            {
                dtComments = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0002_0004_compare_comment");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtComments);
            }

            for (int i = 1; i < dtComments.Columns.Count; i++)
            {
                if (dtComments.Columns[i].Caption == name)
                {
                    Code = dtComments.Rows[0][i].ToString();
                    Item = dtComments.Rows[2][i].ToString();
                    return

                  String.Format("<br/>Условие: {0}  &nbsp;<br/>Единицы измерения: {1}",
                            dtComments.Rows[1][i],
                            dtComments.Rows[2][i]);
                }
            }
            return String.Empty;
        }
    }
}