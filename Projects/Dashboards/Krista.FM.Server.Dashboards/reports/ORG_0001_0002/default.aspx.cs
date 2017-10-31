using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;

/**
 *  Цены на нефтепродукты по Ханты-Мансийскому автономному округу – Югре детализация
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0001_0002
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private static DataTable dtDate;
		private static DataTable dtFuel;
		private static DataTable dtGrid;
		private static DataTable dtChart1;
		private static GridHeaderLayout headerLayout;
		private static DataTable dtRegions;

        private static string Istoch =
            "[Источники данных].[Источники данных].[Все источники данных].[СТАТ Отчетность - Департамент экономики]";
    //"[Источники данных].[Источники данных].[Источник].[СТАТ Отчетность - ДЭ Еженедельная]";

		#endregion

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

        private CustomParam DataIstock;

		private CustomParam fuel;
		private CustomParam date;
		private CustomParam price;
		private CustomParam selectedDate;
		private CustomParam compareDate;
		private CustomParam region;

		#endregion

		// --------------------------------------------------------------------

		private const string PageTitleCaption = "Анализ средних розничных и закупочных цен на нефтепродукты в разрезе муниципальных образований";
        private const string PageSubTitleCaption = "Ежедекадный мониторинг цен на нефтепродукты на основании данных мониторинга предприятий торговли муниципальных образований, Ханты-Мансийский автономный округ - Югра, по состоянию на {0}";
		private const string Chart1TitleCaption = "Уровень средних закупочных и средних розничных цен на «{0}» в разрезе муниципальных образований на {1} года, рублей за литр";

		// --------------------------------------------------------------------
				
		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			if (!IsSmallResolution)
			{
				ComboDate.Title = "Выберите дату";
				ComboDate.Width = 275;
				ComboDate.ShowSelectedValue = true;
				ComboDate.ParentSelect = true;
				ComboCompareDate.Title = "Выберите дату для сравнения";
				ComboCompareDate.Width = 375;
				ComboCompareDate.ShowSelectedValue = true;
				ComboCompareDate.ParentSelect = true;
				ComboFuel.Title = "Выберите вид топлива";
				ComboFuel.Width = 375;
				ComboFuel.ShowSelectedValue = true;
				ComboFuel.ParentSelect = true;
			}
			else
			{
				ComboDate.PanelHeaderTitle = "Выберите дату";
				ComboDate.Width = 200;
				ComboDate.ShowSelectedValue = false;
				ComboDate.ParentSelect = true;
				ComboCompareDate.PanelHeaderTitle = "Выберите дату для сравнения";
				ComboCompareDate.Width = 250;
				ComboCompareDate.ShowSelectedValue = false;
				ComboCompareDate.ParentSelect = true;
				ComboFuel.PanelHeaderTitle = "Выберите вид топлива";
				ComboFuel.Width = 200;
				ComboFuel.ShowSelectedValue = false;
				ComboFuel.ParentSelect = true;
			}

            DataIstock = UserParams.CustomParam("DataIstock");
            DataIstock.Value = Istoch;

			#region Настройка грида

			if (!IsSmallResolution)
			{
				UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			}
			else
			{
				UltraWebGrid.Width = CRHelper.GetGridWidth(750);
			}
			UltraWebGrid.Height = new Unit(300);
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

			#endregion

			#region Настройка диаграммы 1

			if (!IsSmallResolution)
			{
				UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
			}
			else
			{
				UltraChart1.Width = CRHelper.GetChartWidth(750);
			}
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

			UltraChart1.ChartType = ChartType.StackColumnChart;

			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 10;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 10;
			UltraChart1.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.Y.Margin.Near.Value = 10;
			UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.Y.Margin.Far.Value = 10;

			UltraChart1.Axis.X.Extent = 120;
			UltraChart1.Axis.X.Labels.Visible = false;
			UltraChart1.Axis.X.Labels.SeriesLabels.WrapText = true;
			UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
			UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			//UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 45;
			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 10;
			UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart1.Legend.Visible = true;

			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;

			Color color1 = Color.LimeGreen;
			Color color2 = Color.Firebrick;

			UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
			UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
			UltraChart1.ColorModel.Skin.ApplyRowWise = false;

			UltraChart1.Effects.Effects.Clear();
			GradientEffect effect = new GradientEffect();
			effect.Style = GradientStyle.ForwardDiagonal;
			effect.Coloring = GradientColoringStyle.Darken;
			effect.Enabled = true;
			UltraChart1.Effects.Enabled = true;
			UltraChart1.Effects.Effects.Add(effect);

			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			#endregion

			#region Ссылки

			HyperLink.Text = "Анализ&nbspсредних&nbspрозничных&nbspи средних&nbspзакупочных&nbspцен&nbspна&nbspнефтепродукты";
			HyperLink.NavigateUrl = "~/reports/ORG_0001_0003/Default.aspx";

			#endregion

			#region Экспорт

			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

			#endregion
		}

		// --------------------------------------------------------------------

        string UnPAck(string s)
        {
            return s.Replace("-", "[Период__День].[Период__День].[Данные всех периодов].[");
        }

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			region = UserParams.CustomParam("region");
			date = UserParams.CustomParam("date");
			price = UserParams.CustomParam("price");
			fuel = UserParams.CustomParam("fuel", true);
			selectedDate = UserParams.CustomParam("selectedDate", true);
			compareDate = UserParams.CustomParam("compareDate", true);
            UserParams.UnlockParams();
			if (!Page.IsPostBack)
			{
				FillComboDate(ComboDate, "ORG_0001_0002_list_of_dates", 0);
				if (!String.IsNullOrEmpty(selectedDate.Value))
				{
                    selectedDate.Value = UnPAck(selectedDate.Value);
                    
					ComboDate.SetСheckedState(MDXDateToDateString(selectedDate.Value), true);
				}
				FillComboDate(ComboCompareDate, "ORG_0001_0002_list_of_dates", 1);
				if (!String.IsNullOrEmpty(compareDate.Value))
				{
                    compareDate.Value = UnPAck(compareDate.Value);
					ComboCompareDate.SetСheckedState(MDXDateToDateString(compareDate.Value), true);
				}
				FillComboFuel(ComboFuel, "ORG_0001_0002_list_of_fuel", 0);
			}

			#region Анализ параметров

			fuel.Value = ComboFuel.SelectedValue;
			switch (ComboDate.SelectedNode.Level)
			{
				case 0:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.GetLastChild(ComboDate.SelectedNode).FirstNode.Text);
						break;
					}
				case 1:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.SelectedNode.FirstNode.Text);
						break;
					}
				case 2:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.SelectedNode.Text);
						break;
					}
			}
			switch (ComboCompareDate.SelectedNode.Level)
			{
				case 0:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.GetLastChild(ComboCompareDate.SelectedNode).FirstNode.Text);
						break;
					}
				case 1:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.SelectedNode.FirstNode.Text);
						break;
					}
				case 2:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.SelectedNode.Text);
						break;
					}
			}
			if (compareDate.Value == GetMaxMDXDate(selectedDate.Value, compareDate.Value))
			{
				string tmpDate = selectedDate.Value;
				selectedDate.Value = compareDate.Value;
				compareDate.Value = tmpDate;
			}

			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(PageSubTitleCaption, ComboDate.SelectedValue);
			//PageSubTitle.Text = String.Format(PageSubTitleCaption, MDXDateToShortDateString(selectedDate.Value));

			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.DataBind();
			UltraChart1.DataBind();

            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {   
                Row.Cells[0].RowSpan = 3;
            }

            GridHeader.Text = string.Format("Средние закупочные и розничные цены на товар «{0}», рублей за литр", ComboFuel.SelectedValue);
            GridHeader.Visible = true;
             
		}

		// --------------------------------------------------------------------

		#region Обработчики грида


        class SortDataRow : System.Collections.Generic.IComparer<TreeRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(TreeRow x, TreeRow y)
            {
                return -Compare_(x, y);
            }

            public int Compare_(TreeRow x, TreeRow y)
            {
                string Xname = x.ValueRow[0].ToString();
                string Yname = y.ValueRow[0].ToString();

                if (Xname == Yname)
                {
                    return 0;
                }

                if (Xname == "Ханты-Мансийский автономный округ")
                {
                    return 1;
                }

                if (Yname == "Ханты-Мансийский автономный округ")
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

        struct TreeRow
        {
            public DataRow ValueRow;
            public DataRow DeviationRow;
            public DataRow SpeedDeviationRow;
        }

        DataTable SortTable(DataTable Table)
        {
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<TreeRow> LR = new System.Collections.Generic.List<TreeRow>();
            
            for(int i = 0;i<Table.Rows.Count;i+=3)
            {
                TreeRow TR = new TreeRow();
                TR.ValueRow = Table.Rows[i];
                TR.DeviationRow = Table.Rows[i+1];
                TR.SpeedDeviationRow = Table.Rows[i + 2];
                LR.Add(TR);
            }

            LR.Sort(new SortDataRow());

            foreach (TreeRow Row in LR)
            {   
                TableSort.Rows.Add(Row.ValueRow.ItemArray);
                TableSort.Rows.Add(Row.DeviationRow.ItemArray);
                TableSort.Rows.Add(Row.SpeedDeviationRow.ItemArray);
            }
            return TableSort;
        }

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			dtRegions = new DataTable();
			string query = DataProvider.GetQueryText("ORG_0001_0002_regions");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Муниципальные образования", dtRegions);

			if (dtRegions != null)
			{
				dtGrid = new DataTable();
				dtGrid.Columns.Add("Муниципальное образование", typeof(string));
				dtGrid.Columns.Add("Средняя закупочная цена на дату для сравнения", typeof(string));
				dtGrid.Columns.Add("Средняя закупочная цена на выбранную дату", typeof(string));
				dtGrid.Columns.Add("Средняя розничная цена на дату для сравнения", typeof(string));
				dtGrid.Columns.Add("Средняя розничная цена на выбранную дату", typeof(string));
				dtGrid.Columns.Add("Разница между средней закупочной и средней розничной ценой на дату для сравнения", typeof(string));
				dtGrid.Columns.Add("Разница между средней закупочной и средней розничной ценой на выбранную дату", typeof(string));
                dtGrid.Columns.Add("Процент торговой надбавки на дату для сравнения", typeof(string));
                dtGrid.Columns.Add("Процент торговой надбавки на выбранную дату", typeof(string));

				for (int region_num = 0; region_num < dtRegions.Rows.Count; ++region_num)
				{
					region.Value = dtRegions.Rows[region_num][0].ToString();
					DataRow row1 = dtGrid.NewRow();
					DataRow row2 = dtGrid.NewRow();
					DataRow row3 = dtGrid.NewRow();
					row1[0] = region.Value;

					date.Value = compareDate.Value;
					price.Value = "Закупочная цена";
					query = DataProvider.GetQueryText("ORG_0001_0002_fuel_data");
					DataTable dtFuelData = new DataTable();
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по ГСМ", dtFuelData);
					row1[1] = MathMean(dtFuelData, 1);

					date.Value = selectedDate.Value;
					query = DataProvider.GetQueryText("ORG_0001_0002_fuel_data");
					dtFuelData = new DataTable();
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по ГСМ", dtFuelData);
					row1[2] = MathMean(dtFuelData, 1);
					row2[2] = Grow(row1[2], row1[1]);
					row3[2] = Minus(row1[2], row1[1]);

					date.Value = compareDate.Value;
					price.Value = "Розничная цена";
					query = DataProvider.GetQueryText("ORG_0001_0002_fuel_data");
					dtFuelData = new DataTable();
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по ГСМ", dtFuelData);
					row1[3] = MathMean(dtFuelData, 1);

					date.Value = selectedDate.Value;
					query = DataProvider.GetQueryText("ORG_0001_0002_fuel_data");
					dtFuelData = new DataTable();
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по ГСМ", dtFuelData);
					row1[4] = MathMean(dtFuelData, 1);
					row2[4] = Grow(row1[4], row1[3]);
					row3[4] = Minus(row1[4], row1[3]);

					row1[5] = Minus(row1[3], row1[1]);
					row1[6] = Minus(row1[4], row1[2]);
					row2[6] = Grow(row1[6], row1[5]);
					row3[6] = Minus(row1[6], row1[5]);

					row1[7] = Percent(row1[5], row1[1]);
					row1[8] = Percent(row1[6], row1[2]);
					row3[8] = Minus(row1[8], row1[7]);

					dtGrid.Rows.Add(row1);
					dtGrid.Rows.Add(row2);
					dtGrid.Rows.Add(row3);
				}
                
                

                UltraWebGrid.DataSource = SortTable(dtGrid);
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
			e.Layout.NullTextDefault = "-";
			
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Муниципальное образование");
			GridHeaderCell cell = headerLayout.AddCell("Средняя закупочная цена, рублей за литр");
			cell.AddCell(MDXDateToShortDateString(compareDate.Value));
			cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
			cell = headerLayout.AddCell("Средняя розничная цена, рублей за литр");
			cell.AddCell(MDXDateToShortDateString(compareDate.Value));
			cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
			cell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
			cell.AddCell(MDXDateToShortDateString(compareDate.Value));
			cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            cell = headerLayout.AddCell("Торговая надбавка, %");
			cell.AddCell(MDXDateToShortDateString(compareDate.Value));
			cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
			headerLayout.ApplyHeaderInfo();

			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				int columnWidth = 100;
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
			}
			e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.Gray;
			e.Layout.Bands[0].Columns[3].CellStyle.ForeColor = Color.Gray;
			e.Layout.Bands[0].Columns[5].CellStyle.ForeColor = Color.Gray;
			e.Layout.Bands[0].Columns[7].CellStyle.ForeColor = Color.Gray;
		}

		protected string GetCellFormatString(int row, int column)
		{
			if (row % 3 == 1 || column > UltraWebGrid.Columns.Count - 3)
				return "{0:P2}";
			else
				return "{0:N2}";
		} 

        private void SetStarChar(UltraGridCell Cell)
        {
            if ((Cell==null)||(Cell.Value == null))
                return;
            string NameRegion = Cell.Text;

            string[] StarRegions = new string[12] { "Ханты-Мансийский автономный округ", "Советск", "Сургутск", "Когал", "Ланге", "Мегион", "Нефтеюганск", "Нижневартовский-", "Нягань", "Сургут", "Пыть", "Югорск" };
            foreach (string R in StarRegions)
            {
                if (NameRegion.Contains(R))
                {
                    return;
                }
            }
            Cell.Text += "*";
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
			UltraWebGrid grid = sender as UltraWebGrid;
            //
            if (e.Row.Cells[0].Value != null)
            if ((e.Row.Cells[0].Text.Contains("Белоярский муниципальный район")) 
                && (ComboFuel.SelectedValue.Contains("Газ сжиженный углеводородный для заправки автотранспортных средств")))
            {
                e.Row.Cells[0].Style.BackgroundImage = "~/images/cornerGreen.gif";
                e.Row.Cells[0].Title = "Единица измерения: куб.метр";
                e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
            }


			foreach (UltraGridCell cell in e.Row.Cells)
			{
				if (cell.Column.Index > 0 && cell.Value != null)
				{   
					if (cell.Row.Index % 3 == 2)
						if (Convert.ToDouble(cell.GetText()) > 0.0001)
						{				
                            SetImageFromCell(grid.Rows[e.Row.Index - 2].Cells[cell.Column.Index], "ArrowRedUpBB.png");
						}
						else if (Convert.ToDouble(cell.GetText()) < -0.0001)
						{
                            SetImageFromCell(grid.Rows[e.Row.Index - 2].Cells[cell.Column.Index], "ArrowGreenDownBB.png");				
						}
					cell.Value = String.Format(GetCellFormatString(cell.Row.Index, cell.Column.Index), Convert.ToDouble(cell.GetText()));
				}
				SetCellHint(cell);
			}
            SetStarChar(e.Row.Cells[0]);
		}

		protected void SetCellHint(UltraGridCell cell)
		{
			int row = cell.Row.Index;
			int column = cell.Column.Index;
			if (row % 3 == 1 && (column == 2 || column == 4 || column == 6))
			{
                cell.Title = String.Format("Изменение в % к {0} года", MDXDateToShortDateString(compareDate.Value));
			}
			if (row % 3 == 2 && (column == 2 || column == 4 || column == 6 || column == 8))
			{
                cell.Title = String.Format("Изменение в руб. к {0} года", MDXDateToShortDateString(compareDate.Value));
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1


        class SortDataRowChart : System.Collections.Generic.IComparer<Row>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(Row x, Row y)
            {
                return -Compare_(x, y);
            }

            public int Compare_(Row x, Row y)
            {
                
                if ((x.ValueRow[1] == DBNull.Value)
                    &
                    (y.ValueRow[1] != DBNull.Value))
                {
                    return 1;
                }

                if ((x.ValueRow[1] != DBNull.Value)
                    &
                    (y.ValueRow[1] == DBNull.Value))
                {
                    return -1;
                }

                if (x.ValueRow[1] == y.ValueRow[1])
                {
                    return 0;
                }

                try
                {
                    double XValue = (double)x.ValueRow[1];
                    double YValue = (double)y.ValueRow[1];

                    if (XValue > YValue)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                catch 
                {
                    return 0;
                }
            }

            #endregion
        }

        struct Row
        {
            public DataRow ValueRow;
            public DataRow DeviationRow;
            public DataRow SpeedDeviationRow;
        }

        DataTable SortTableChart(DataTable Table)
        {
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<Row> LR = new System.Collections.Generic.List<Row>();

            for (int i = 0; i < Table.Rows.Count; i += 1)
            {
                Row TR = new Row(); 
                TR.ValueRow = Table.Rows[i];                
                LR.Add(TR);
            }
            try
            {
                LR.Sort(new SortDataRowChart());
            }catch{}

            foreach (Row Row in LR)
            {
                TableSort.Rows.Add(Row.ValueRow.ItemArray);
            }

            return TableSort;
        }

		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{            


			LabelChart1.Text = String.Format(Chart1TitleCaption, fuel.Value, MDXDateToShortDateString(selectedDate.Value));
			UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
			if (dtRegions != null)
			{
				dtChart1 = new DataTable();
				dtChart1.Columns.Add("Территория", typeof(string));
				dtChart1.Columns.Add("Закупочная цена", typeof(double));
				dtChart1.Columns.Add("Розничная цена", typeof(double));

				for (int i = 0; i < dtGrid.Rows.Count; i += 3)
				{
                    if (ComboFuel.SelectedValue.Contains("Газ") && (dtGrid.Rows[i][0].ToString().Contains("Белоярский муниципальный район")))
                    {
                        continue;
                    }   
                    DataRow row = dtChart1.NewRow();
					row[0] = dtGrid.Rows[i][0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
					row[1] = dtGrid.Rows[i][2];
					row[2] = dtGrid.Rows[i][2] != DBNull.Value ? dtGrid.Rows[i][6] : dtGrid.Rows[i][4];

					dtChart1.Rows.Add(row);
				}

                dtChart1 = SortTableChart(dtChart1);
                
                
				UltraChart1.Data.SwapRowsAndColumns = true;
				UltraChart1.Series.Clear();
				for (int i = 1; i < 3; i++)
				{
					NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
					series.Label = dtChart1.Columns[i].ColumnName;
					UltraChart1.Series.Add(series);
				}
			}
			else
			{
				UltraChart1.DataSource = null;
			}
		}

		protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				Box box;
				if (primitive is Box && primitive.DataPoint != null)
					if (primitive.DataPoint.Label == "Розничная цена")
					{
						box = primitive as Box;
						double value1, value2;
						if (Double.TryParse(dtChart1.Rows[box.Row][1].ToString(), out value1) && Double.TryParse(dtChart1.Rows[box.Row][2].ToString(), out value2))
						{
							box.DataPoint.Label = String.Format("Розничная цена\n<b>{0:N2}</b>, рублей за литр", value1 + value2);
						}
						if (dtChart1.Rows[box.Row][1] == DBNull.Value && Double.TryParse(dtChart1.Rows[box.Row][2].ToString(), out value2))
						{
							box.DataPoint.Label = String.Format("Розничная цена\n<b>{0:N2}</b>, рублей за литр", value2);
						}
					}
					else if (primitive.DataPoint.Label == "Закупочная цена")
					{
						box = primitive as Box;
						double value;
						if (Double.TryParse(dtChart1.Rows[box.Row][1].ToString(), out value))
						{
							box.DataPoint.Label = String.Format("Закупочная цена\n<b>{0:N2}</b>, рублей за литр", value);
						}
					}
			}
            Text Caption = new Text();
            Caption.SetTextString("Рублей за литр");
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -30;
            Caption.bounds.Y = 120;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
		}

		#endregion

		// --------------------------------------------------------------------

		#region Общие функции

		/// <summary>
		/// Арифметическое среднее
		/// </summary>
		/// <param name="table">Таблица</param>
		/// <param name="column">Колонка (индекс)</param>
		/// <returns>Арифметическое среднее</returns>
		protected object MathMean(DataTable table, int column)
		{
			if (table == null || table.Rows.Count == 0)
				return DBNull.Value;
			double result = 0.0;
			foreach (DataRow row in table.Rows)
			{
				result += Convert.ToDouble(row[column]);
			}
			result = result / table.Rows.Count;
			return result;
		}

		/// <summary>
		/// Геометрическое среднее
		/// </summary>
		/// <param name="table">Таблица</param>
		/// <param name="column">Колонка (индекс)</param>
		/// <returns>Геометричекое среднее</returns>
		protected object GeoMean(DataTable table, int column)
		{
			if (table == null || table.Rows.Count == 0)
				return DBNull.Value;
			double result = 1.0;
			foreach (DataRow row in table.Rows)
			{
				result *= Convert.ToDouble(row[column]);
			}
			result = Math.Pow(result, 1.0 / table.Rows.Count);
			return result;
		}

		/// <summary>
		/// Разбирает таблицу с данными. На выходе массив: арифметическое среднее по ХМАО, минимальное и максимальное значение среднего по МР
		/// </summary>
		/// <param name="dtFuelData">Таблица с данными</param>
		/// <param name="results">Массив с результатами</param>
		protected void ParseFuelDataMath(DataTable dtFuelData, object[] results)
		{
			double[] gmValues = new double[dtFuelData.Rows.Count];
			string[] gmRegions = new string[dtFuelData.Rows.Count];
			for (int i = 0; i < dtFuelData.Rows.Count; ++i)
			{
				DataRow row = dtFuelData.Rows[i];
				gmRegions[i] = row[0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
				int count = 0;
				for (int j = 1; j < row.ItemArray.Length; ++j)
				{
					object cell = row[j];
					if (cell != DBNull.Value)
					{
						gmValues[i] = (gmValues[i] == 0) ? Convert.ToDouble(cell) : gmValues[i] + Convert.ToDouble(cell);
						++count;
					}
				}
				gmValues[i] = gmValues[i] / count;
			}
			Array.Sort(gmValues, gmRegions);
			results[0] = 0.0;
			foreach (double value in gmValues)
			{
				results[0] = Convert.ToDouble(results[0]) + value;
			}
			results[0] = Convert.ToDouble(results[0]) / gmValues.Length;
			results[1] = gmRegions[0];
			results[2] = gmValues[0];
			results[3] = gmRegions[gmRegions.Length - 1];
			results[4] = gmValues[gmValues.Length - 1];
		}

		/// <summary>
		/// Разбирает таблицу с данными. На выходе массив: геометрическое среднее по ХМАО, минимальное и максимальное значение среднего по МР
		/// </summary>
		/// <param name="dtFuelData">Таблица с данными</param>
		/// <param name="results">Массив с результатами</param>
		protected void ParseFuelData(DataTable dtFuelData, object[] results)
		{
			double[] gmValues = new double[dtFuelData.Rows.Count];
			string[] gmRegions = new string[dtFuelData.Rows.Count];
			for (int i = 0; i < dtFuelData.Rows.Count; ++i)
			{
				DataRow row = dtFuelData.Rows[i];
				gmRegions[i] = row[0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
				int count = 0;
				for (int j = 1; j < row.ItemArray.Length; ++j)
				{
					object cell = row[j];
					if (cell != DBNull.Value)
					{
						gmValues[i] = (gmValues[i] == 0) ? Convert.ToDouble(cell) : gmValues[i] * Convert.ToDouble(cell);
						++count;
					}
				}
				gmValues[i] = Math.Pow(gmValues[i], 1.0 / count);
			}
			Array.Sort(gmValues, gmRegions);
			results[0] = 1.0;
			foreach (double value in gmValues)
			{
				results[0] = Convert.ToDouble(results[0]) * value;
			}
			results[0] = Math.Pow(Convert.ToDouble(results[0]), 1.0 / gmValues.Length);
			results[1] = gmRegions[0];
			results[2] = gmValues[0];
			results[3] = gmRegions[gmRegions.Length - 1];
			results[4] = gmValues[gmValues.Length - 1];
		}

		#endregion

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
		{
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			if (dtDate.Rows.Count == 0)
			{
				throw new Exception("Данные для построения отчета отсутствуют в кубе");
			}
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				AddPairToDictionary(dictDate, year + " год", 0);
				AddPairToDictionary(dictDate, month + " " + year + " года", 1);
				AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
			}
			combo.FillDictionaryValues(dictDate);
			combo.SelectLastNode();
		}

		protected void FillComboFuel(CustomMultiCombo combo, string queryName, int offset)
		{
			dtFuel = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtFuel);
			Dictionary<string, int> dictFuel = new Dictionary<string, int>();
			for (int row = 0; row < dtFuel.Rows.Count - offset; ++row)
			{
				string fuel_name = dtFuel.Rows[row][2].ToString();
				AddPairToDictionary(dictFuel, fuel_name, 0);
			}
			combo.FillDictionaryValues(dictFuel);
			if (!String.IsNullOrEmpty(fuel.Value))
			{
				//combo.SetСheckedState(fuel.Value, true);
			}
			else if (dtFuel != null)
			{
				//combo.SetСheckedState(dtFuel.Rows[0][2].ToString(), true);
			}
            if (fuel.Value != "")
            {
                combo.SetСheckedState(fuel.Value, true);
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

		public object Percent(object firstValue, object secondValue)
		{
			double value1, value2;
			if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2) && value2 != 0)
				return value1 / value2;
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

		public string GetMaxMDXDate(string firstDate, string secondDate)
		{
			if (Convert.ToInt32(FormatMDXDate(firstDate, "{0}{1:00}{2:00}")) > Convert.ToInt32(FormatMDXDate(secondDate, "{0}{1:00}{2:00}")))
			{
				return firstDate;
			}
			else
			{
				return secondDate;
			}
		}

		public string FormatMDXDate(string mdxDate, string formatString, int yearIndex, int monthIndex, int dayIndex)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[yearIndex]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[monthIndex]));
			int day = Convert.ToInt32(dateElements[dayIndex]);
			return String.Format(formatString, year, month, day);
		}

		public string FormatMDXDate(string mdxDate, string formatString)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[3]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[6]));
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			return String.Format(formatString, year, month, day);
		}

		public string StringToMDXDate(string str)
		{
			string template = "[Период__День].[Период__День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
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
			string template = "{0}.{1}.{2}";
			string day = dateElements[7].Replace("]", String.Empty);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3];
			return String.Format(template, day, month, year);
		}

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

		#endregion

		#region Экспорт в PDF

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();

			IText title = section1.AddText();
			Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.Style.Font.Bold = true;
			title.AddContent(PageTitle.Text);


			title = section1.AddText();
			font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(PageSubTitle.Text);

			UltraWebGrid grid = headerLayout.Grid;

            grid.Columns.Add("Безымяный столбик");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                }
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);

            GridHeaderCell с = headerLayout.AddCell("Муниципальное образование");
            с.AddCell(" ");
            с.AddCell(" ");
            GridHeaderCell cell = headerLayout.AddCell("Средняя закупочная цена, рублей за литр");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            cell = headerLayout.AddCell("Средняя розничная цена, рублей за литр");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            cell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            cell = headerLayout.AddCell("Процент торговой надбавки");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerLayout.ApplyHeaderInfo();

            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 180;
			
			ReportPDFExporter1.HeaderCellHeight = 60;
			ReportPDFExporter1.Export(headerLayout,GridHeader.Text, section1);

            if (ComboFuel.SelectedValue.Contains("Газ сжиженный"))
            {
                IText t = section1.AddText();
                t.AddContent("  ");

                ITable tabletext = section1.AddTable();

                ITableRow tr = tabletext.AddRow();

                ITableCell tc = tr.AddCell();
                tc.Width = new FixedWidth(30);



                IImage ima = tc.AddImage(new Infragistics.Documents.Reports.Graphics.Image(Server.MapPath("~/images/cornerGreen.gif")));

                IText text = tr.AddCell().AddText();

                text.AddContent(" - по Белоярскому району цены на газ сжиженный углеводородный для заправки автотранспортных средств указаны в куб. метрах");
            }

			UltraChart1.Width = LabelChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
			ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
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

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 30;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			ReportExcelExporter1.Export(headerLayout, sheet1, 4);

			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Height = 550;

			for (int i = 5; i < UltraWebGrid.Rows.Count + 5; ++i)
			{
				Font exportFont = new Font("Verdana", 9);
				sheet1.Rows[i].Height = 255;
				for (int j = 1; j < UltraWebGrid.Columns.Count; ++j)
				{
					sheet1.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Right;
					sheet1.Rows[i].Cells[j].CellFormat.Font.Name = exportFont.Name;
					sheet1.Rows[i].Cells[j].CellFormat.Font.Height = (int)exportFont.Size * 20;
				}
			}

			for (int i = 5; i < UltraWebGrid.Rows.Count + 5; i += 3)
			{
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
					sheet1.Rows[i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[1 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[1 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[2 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
				}
			}
            for (int i = 5; i < UltraWebGrid.Rows.Count + 5; i++)
            {
                try
                {
                    if (ComboFuel.SelectedValue.Contains("Газ сжиженный углеводор"))
                        if (sheet1.Rows[i].Cells[0].Value.ToString().Contains("Белоярский"))
                        {
                            sheet1.Rows[i].Cells[0].Comment = new WorksheetCellComment();
                            sheet1.Rows[i].Cells[0].Comment.Visible = true;
                            sheet1.Rows[i].Cells[0].Comment.Text = new FormattedString("Единица измерения: куб.метр");
                        }
                }
                catch { }
            }



			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);


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

		private void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.0;

            grid.Columns.Add("Безымяный столбик");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                } 
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);

            GridHeaderCell с = headerLayout.AddCell("Муниципальное образование");
            с.AddCell(" ");
            с.AddCell(" ");            
            GridHeaderCell cell = headerLayout.AddCell("Средняя закупочная цена, рублей за литр");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            cell = headerLayout.AddCell("Средняя розничная цена, рублей за литр");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            cell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            cell = headerLayout.AddCell("Процент торговой надбавки");
            cell.AddCell(MDXDateToShortDateString(compareDate.Value));
            cell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerLayout.ApplyHeaderInfo();

            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 180;

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
