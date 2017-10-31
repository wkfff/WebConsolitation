using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
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
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0007
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

		#endregion

		// имя папки с картами региона
		private string mapFolderName;

		private static bool IsMozilla
		{
			get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
		}

		private static string getBrowser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}


		#region Параметры запроса

		// выбранный товар
		private CustomParam selectedFood;
		// выбранная дата
		private CustomParam selectedDate;
		// предыдущий месяц дата
		private CustomParam previousDate;
		// начало года
		private CustomParam yearDate;
		// те же, но в текстовом формате (для вывода на экран, чтобы не конвертировать)
		private static string selectedDateText;
		private static string previousDateText;
		private static string yearDateText;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Анализ розничных цен на социально значимые продовольственные товары (в разрезе муниципальных образований)";
		private const string PageSubTitleCaption = "Ежемесячный мониторинг средних розничных цен на социально значимые продовольственные товары, по состоянию на {0}";
		// заголовок для UltraChart
		private const string ChartTitleCaption = "Распределение территорий по розничной цене на товар \"{0}\", рублей за {1}";
		private const string MapTitleCaption = "Розничная цена на товар \"{0}\", рублей за {1}";
		// Единицы измерения
		private static Dictionary<string, string> dictUnits;


		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			mapFolderName = "Вологда";

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			ComboFood.Title = "Товар";
			ComboFood.Width = 500;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

			#region Настройка диаграммы
			UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
			UltraChart.Height = Unit.Empty;

			UltraChart.ChartType = ChartType.ColumnChart;
			UltraChart.Border.Thickness = 0;

			UltraChart.ColumnChart.SeriesSpacing = 1;
			UltraChart.ColumnChart.ColumnSpacing = 1;

			UltraChart.Axis.X.Extent = 120;
			UltraChart.Axis.X.Labels.Visible = false;
			UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
			UltraChart.Axis.Y.Extent = 20;
			UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\nРозничная цена: <b><DATA_VALUE:N2></b> рублей";
			UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
			UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			#endregion

			#region Параметры
			if (selectedDate == null)
			{
				selectedDate = UserParams.CustomParam("selected_date");
			}
			if (previousDate == null)
			{
				previousDate = UserParams.CustomParam("previous_date");
			}
			if (yearDate == null)
			{
				yearDate = UserParams.CustomParam("year_date");
			}
			if (selectedFood == null)
			{
				selectedFood = UserParams.CustomParam("selected_food");
			}
			#endregion

			#region Карта

			//mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
			//mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

			DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
			DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

			#endregion

			#region Экспорт
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				fillComboDate("ORG_0003_0007_list_of_dates");
				fillComboFood("ORG_0003_0007_list_of_food");
			}

			#region Анализ параметров

			GetDates(ComboDate, selectedDate, previousDate, yearDate);
			selectedDateText = MDXDateToShortDateString(selectedDate.Value);
			previousDateText = MDXDateToShortDateString(previousDate.Value);
			yearDateText = MDXDateToShortDateString(yearDate.Value);
			if (ComboFood.SelectedNode.Level == 0)
			{
				selectedFood.Value = StringToMDXFood(ComboFood.SelectedValue, ComboFood.SelectedNode.FirstNode.Text);
			}
			if (ComboFood.SelectedNode.Level == 1)
			{
				selectedFood.Value = StringToMDXFood(ComboFood.SelectedNode.Parent.Text, ComboFood.SelectedValue);
			}

			#endregion

			PageTitle.Text = PageTitleCaption;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, MDXDateToShortDateString(selectedDate.Value));
			Page.Title = PageTitle.Text;
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			UltraWebGrid.DataBind();
			calculateRank();

			UltraChart.DataBind();

			#region Карта
			DundasMap.Shapes.Clear();
			DundasMap.ShapeFields.Add("Name");
			DundasMap.ShapeFields["Name"].Type = typeof(string);
			DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
			DundasMap.ShapeFields.Add("Cost");
			DundasMap.ShapeFields["Cost"].Type = typeof(double);
			DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

			SetMapSettings();
			AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
			//AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
			AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
			FillMapData();
			#endregion
		}

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
			Node prevNode = null;
			if (node.Parent.PrevNode != null)
			{
				prevNode = node.Parent.PrevNode.FirstNode;
				if (!isPreviousMonth(getMonthFromString(node.Text), getMonthFromString(prevNode.Text)))
				{
					prevNode = null;
				}
			}
			if (prevNode != null)
			{
				previousDate.Value = StringToMDXDate(prevNode.Text);
			}
			else
			{
				previousDate.Value = StringToMDXDate(replaceMonth(node.Text));
			}
			Node yearNode = node.Parent.Parent.FirstNode.FirstNode;
			if (CRHelper.MonthNum(getMonthFromString(yearNode.Text)) == 1)
			{
				yearDate.Value = StringToMDXDate(yearNode.Text);
			}
			else
			{
				yearDate.Value = StringToMDXDate(getYearDate(node.Text));
			}
		}

		// --------------------------------------------------------------------
		#region Обработчики карты

		public void SetMapSettings()
		{
			DundasMap.Meridians.Visible = false;
			DundasMap.Parallels.Visible = false;
			DundasMap.ZoomPanel.Visible = true;
			DundasMap.NavigationPanel.Visible = true;
			DundasMap.Viewport.EnablePanning = true;
			DundasMap.Viewport.Zoom = 100;
			//DundasMap.Viewport.Zoom = (float)mapZoomValue;

			// добавляем легенду
			Legend legend = new Legend("CostLegend");
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
			string unit;
			dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
			legend.Title = String.Format("Средняя розничная\nцена на товар,\nрублей за {0}", unit.ToLower());
			legend.AutoFitMinFontSize = 7;
			DundasMap.Legends.Clear();
			DundasMap.Legends.Add(legend);

			// добавляем правила раскраски
			DundasMap.ShapeRules.Clear();
			ShapeRule rule = new ShapeRule();
			rule.Name = "CostRule";
			rule.Category = String.Empty;
			rule.ShapeField = "Cost";
			rule.DataGrouping = DataGrouping.EqualDistribution;
			rule.ColorCount = 7;
			rule.ColoringMode = ColoringMode.ColorRange;
			rule.FromColor = Color.Green;
			rule.MiddleColor = Color.Yellow;
			rule.ToColor = Color.Red;
			rule.BorderColor = Color.FromArgb(50, Color.Black);
			rule.GradientType = GradientType.None;
			rule.HatchStyle = MapHatchStyle.None;
			rule.ShowInColorSwatch = false;
			rule.ShowInLegend = "CostLegend";
			DundasMap.ShapeRules.Add(rule);
		}

		/// <summary>
		/// Является ли форма городом-выноской
		/// </summary>
		/// <param name="shape">форма</param>
		/// <returns>true, если является</returns>
		public static bool IsCalloutTownShape(Shape shape)
		{
			return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
		}

		/// <summary>
		/// Получение имени формы (с выделением имени из города-выноски)
		/// </summary>
		/// <param name="shape">форма</param>
		/// <returns>имя формы</returns>
		public static string GetShapeName(Shape shape)
		{
			string shapeName = shape.Name;
			if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
			{
				shapeName = shape.Name.Split('_')[0];
			}

			return shapeName;
		}

		private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
		{
			string layerName = Server.MapPath(string.Format("../../../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
			int oldShapesCount = map.Shapes.Count;

			map.LoadFromShapeFile(layerName, "Name", true);
			map.Layers.Add(shapeType.ToString());

			for (int i = oldShapesCount; i < map.Shapes.Count; i++)
			{
				Shape shape = map.Shapes[i];
				shape.Layer = shapeType.ToString();
			}
		}

		/// <summary>
		/// Поиск формы карты
		/// </summary>
		/// <param name="map">карта</param>
		/// <param name="patternValue">искомое имя формы</param>
		/// <returns>найденные формы</returns>
		public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
		{
			hasCallout = false;
			ArrayList shapeList = new ArrayList();
			foreach (Shape shape in map.Shapes)
			{
				if (GetShapeName(shape).ToLower() == patternValue.ToLower())
				{
					shapeList.Add(shape);
					if (IsCalloutTownShape(shape))
					{
						hasCallout = true;
					}
				}
			}

			return shapeList;
		}


		public void FillMapData()
		{
			bool hasCallout;
			string valueSeparator = IsMozilla ? ". " : "\n";
			string shapeHint = "{0}" + valueSeparator + "Розничная цена: {1:N2} рублей" + valueSeparator + "Ранг: {2}";
			string unit;
			dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
			labelMap.Text = String.Format(MapTitleCaption, ComboFood.SelectedValue, unit.ToLower());
			bool Vologda = false;
			bool Cherepovets = false;
			if (dtGrid == null || DundasMap == null)
			{
				return;
			}
			foreach (Shape shape in DundasMap.Shapes)
			{
				shape.Text = String.Format("{0}", GetShapeName(shape).Replace(" ", "\n"));
			}
			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				// заполняем карту данными
				string subject = row.Cells[0].Value.ToString();
				if (subject == "Город Вологда")
				{
					subject = "муниципальное образование \"Город Вологда\"";
					Vologda = true;
				}
				if (subject == "Город Череповец")
				{
					Cherepovets = true;
				}
				double value = Convert.ToDouble(row.Cells[3].Value.ToString());
				ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
				foreach (Shape shape in shapeList)
				{
					shape.Visible = true;
					string shapeName = GetShapeName(shape);
					if (subject.IndexOf("Город Вологда") != -1)
					{
						subject = "Город Вологда";
						shapeName = "Город Вологда";
					}

					if (IsCalloutTownShape(shape))
					{
						shape["Name"] = subject;
						shape["Cost"] = Convert.ToDouble(row.Cells[3].Value.ToString());
						shape.ToolTip = String.Format(shapeHint, subject, row.Cells[3].Value, row.Cells[4].Value);
						shape.TextVisibility = TextVisibility.Shown;
						shape.Text = String.Format("{0}\n{1:N2}", shapeName.Replace(" ", "\n"), value);
						shape.TextVisibility = TextVisibility.Shown;
						shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
					}
					else
					{
						if (!hasCallout)
						{
							shape["Name"] = subject;
							shape["Cost"] = Convert.ToDouble(row.Cells[3].Value.ToString());
							shape.ToolTip = String.Format(shapeHint, subject, row.Cells[3].Value, row.Cells[4].Value);
							shape.TextVisibility = TextVisibility.Shown;
							shape.Text = String.Format("{0}\n{1:N2}", shapeName.Replace(" ", "\n"), value);
						}
					}
				}
			}
			if (!Vologda)
			{
				ArrayList shapeList = FindMapShape(DundasMap, "муниципальное образование \"Город Вологда\"", out hasCallout);
				foreach (Shape shape in shapeList)
				{
					if (IsCalloutTownShape(shape))
					{
						shape.Visible = true;
						shape.Text = "Город Вологда";
						shape.TextVisibility = TextVisibility.Shown;
						shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
					}
				}
			}
			if (!Cherepovets)
			{
				ArrayList shapeList = FindMapShape(DundasMap, "Город Череповец", out hasCallout);
				foreach (Shape shape in shapeList)
				{
					if (IsCalloutTownShape(shape))
					{
						shape.Visible = true;
						shape.Text = "Город Череповец";
						shape.TextVisibility = TextVisibility.Shown;
						shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------
		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ORG_0003_0007_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				dtGrid.Columns.Add("Ранг", typeof(Int32));
				dtGrid.Columns.Add("Абсолютное отклонение по отношению к предыдущему периоду", typeof(Double));
				dtGrid.Columns.Add("Темп прироста по отношению к предыдущему периоду", typeof(Double));
				dtGrid.Columns.Add("Абсолютное отклонение по отношению к началу года", typeof(Double));
				dtGrid.Columns.Add("Темп прироста по отношению к началу года", typeof(Double));
				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				UltraWebGrid.Height = UltraChart.Height;
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
			int columnWidth = Convert.ToInt32((Convert.ToInt32(e.Layout.Grid.Width.Value) - CRHelper.GetColumnWidth(300)) / 8);
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
			e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(columnWidth - 60);
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Наименование МО");
			headerLayout.AddCell(String.Format("Розничная цена на {0}, рубль", yearDateText));
			headerLayout.AddCell(String.Format("Розничная цена на {0}, рубль", previousDateText));
			headerLayout.AddCell(String.Format("Розничная цена на {0}, рубль", selectedDateText));
			headerLayout.AddCell("Ранг");

			GridHeaderCell header = headerLayout.AddCell("Динамика к предыдущему отчетному периоду");
			header.AddCell("Абсолютное отклонение, рубль");
			header.AddCell("Темп прироста, %");

			header = headerLayout.AddCell("Динамика за период с начала года");
			header.AddCell("Абсолютное отклонение, рубль");
			header.AddCell("Темп прироста, %");

			headerLayout.ApplyHeaderInfo();
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			double value;
			if (!Double.TryParse(e.Row.Cells[3].ToString(), out value))
			{
				e.Row.Delete();
			}
			double prevValue, yearValue;
			if (Double.TryParse(e.Row.Cells[2].ToString(), out prevValue))
			{
				e.Row.Cells[5].Value = value - prevValue;
				e.Row.Cells[6].Value = String.Format("{0:P2}", (value - prevValue) / prevValue);
				if (value != prevValue)
				{
					if (value < prevValue)
					{
						e.Row.Cells[6].Style.CssClass = "ArrowDownGreen";
					}
					else
					{
						e.Row.Cells[6].Style.CssClass = "ArrowUpRed";
					}
				}
			}
			else
			{
				e.Row.Cells[2].Value = "-";
				e.Row.Cells[5].Value = "-";
				e.Row.Cells[6].Value = "-";
			}
			if (Double.TryParse(e.Row.Cells[1].ToString(), out yearValue))
			{
				e.Row.Cells[7].Value = value - yearValue;
				e.Row.Cells[8].Value = String.Format("{0:P2}", (value - yearValue) / yearValue);
				if (value != yearValue)
				{
					if (value < yearValue)
					{
						e.Row.Cells[8].Style.CssClass = "ArrowDownGreen";
					}
					else
					{
						e.Row.Cells[8].Style.CssClass = "ArrowUpRed";
					}
				}
			}
			else
			{
				e.Row.Cells[1].Value = "-";
				e.Row.Cells[7].Value = "-";
				e.Row.Cells[8].Value = "-";
			}
			// Хинты
			e.Row.Cells[5].Title = String.Format("Абсолютное отклонение по отношению к {0}", previousDateText);
			e.Row.Cells[6].Title = String.Format("Темп прироста к {0}", previousDateText);
			e.Row.Cells[7].Title = String.Format("Абсолютное отклонение по отношению к {0}", yearDateText);
			e.Row.Cells[8].Title = String.Format("Темп прироста к {0}", yearDateText);
		}

		protected void calculateRank()
		{
			double[] rank = new double[UltraWebGrid.Rows.Count];
			for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
			{
				rank[i] = Convert.ToDouble(UltraWebGrid.Rows[i].Cells[3].Value);
			}
			Array.Sort(rank);
			int rank_value;
			for (int i = 0; i < rank.Length; ++i)
			{
				for (int j = 0; j < UltraWebGrid.Rows.Count; ++j)
				{
					if (rank[i] == Convert.ToDouble(UltraWebGrid.Rows[j].Cells[3].Value))
					{
						if (!Int32.TryParse(UltraWebGrid.Rows[j].Cells[4].ToString(), out rank_value))
						{
							UltraWebGrid.Rows[j].Cells[4].Value = i + 1;
						}
						if (i == 0)
						{
							UltraWebGrid.Rows[j].Cells[4].Style.BackgroundImage = "~/images/starYellowbb.png";
							UltraWebGrid.Rows[j].Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							UltraWebGrid.Rows[j].Cells[4].Title = "Самый низкий уровень цены";
						}
						if (i == rank.Length - 1)
						{
							UltraWebGrid.Rows[j].Cells[4].Style.BackgroundImage = "~/images/starGraybb.png";
							UltraWebGrid.Rows[j].Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							UltraWebGrid.Rows[j].Cells[4].Title = "Самый высокий уровень цены";
						}
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы

		protected void UltraChart_DataBinding(object sender, EventArgs e)
		{
			string unit;
			dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
			labelChart.Text = String.Format(ChartTitleCaption, ComboFood.SelectedValue, unit.ToLower());
			string query = DataProvider.GetQueryText("ORG_0003_0007_chart");
			dtChart = new DataTable();
			medianDT = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart);
			double minValue = Double.PositiveInfinity; ;
			double maxValue = Double.NegativeInfinity;
			foreach (DataRow row in dtChart.Rows)
			{
				if (row[0] != DBNull.Value)
				{
					row[0] = row[0].ToString().Replace(" муниципальный район", " р-н");
					row[0] = row[0].ToString().Replace("Город ", "Г. ");
				}
			}
			if (dtChart.Rows.Count > 1)
			{
				double avgValue = 0;
				for (int i = 0; i < dtChart.Rows.Count; ++i)
				{
					double value = Convert.ToDouble(dtChart.Rows[i][1]);
					avgValue += value;
					minValue = value < minValue ? value : minValue;
					maxValue = value > maxValue ? value : maxValue;
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

				if (!Double.IsPositiveInfinity(minValue) && !Double.IsNegativeInfinity(maxValue))
				{
					UltraChart.Axis.Y.RangeType = AxisRangeType.Custom;
					UltraChart.Axis.Y.RangeMax = maxValue * 1.1;
					UltraChart.Axis.Y.RangeMin = minValue / 1.1;
				}
			}
			UltraChart.DataSource = (medianDT == null) ? null : medianDT.DefaultView;
		}

		protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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
		}
		
		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void fillComboFood(string queryName)
		{
			// Загрузка списка продуктов
			dtFood = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtFood);
			// Закачку придется делать через словарь
			Dictionary<string, int> dictFood = new Dictionary<string, int>();
			dictUnits = new Dictionary<string, string>();
			for (int row = 0; row < dtFood.Rows.Count; ++row)
			{
				string food_group = dtFood.Rows[row][1].ToString();
				string food = dtFood.Rows[row][2].ToString();
				string unit = dtFood.Rows[row][4].ToString();
				addPairToDictionary(dictFood, food_group, 0);
				addPairToDictionary(dictFood, food, 1);
				dictUnits.Add(food, unit);
			}
			ComboFood.FillDictionaryValues(dictFood);
		}

		protected void fillComboDate(string queryName)
		{
			// Загрузка списка актуальных дат
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			// Закачку придется делать через словарь
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				addPairToDictionary(dictDate, year + " год", 0);
				addPairToDictionary(dictDate, month + " " + year + " года", 1);
				addPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
			}
			ComboDate.FillDictionaryValues(dictDate);
			ComboDate.SelectLastNode();
		}

		protected void addPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

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
			string template = "{0}.{1}.{2} г.";
			string day = dateElements[7].Substring(0, 1);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
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

		public string replaceMonth(string date)
		{
			string[] dateElements = date.Split(' ');
			int monthIndex = CRHelper.MonthNum(dateElements[1]);
			int year = Convert.ToInt32(dateElements[2]);
			string newMonth = String.Empty;
			if (monthIndex == 1)
			{
				newMonth = "декабря";
				--year;
			}
			else
			{
				newMonth = CRHelper.RusMonthGenitive(monthIndex - 1);
			}
			return String.Format("1 {0} {1} года", newMonth, year);
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

		#region Экспорт в PDF

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();
			ISection section3 = report.AddSection();

			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			ReportPDFExporter1.HeaderCellHeight = 70;
			ReportPDFExporter1.Export(headerLayout, section1);
			ReportPDFExporter1.Export(UltraChart, labelChart.Text, section2);

			section3.PagePaddings = section2.PagePaddings;
			section3.PageMargins = section2.PageMargins;
			section3.PageBorders = section2.PageBorders;
			section3.PageSize = new PageSize(section2.PageSize.Height, section2.PageSize.Width);
			section3.PageOrientation = PageOrientation.Landscape;
			IText title = section3.AddText();
			Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(labelMap.Text);

			DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			DundasMap.ZoomPanel.Visible = false;
			DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
			section3.AddImage(img);
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
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 10);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			ReportExcelExporter1.Export(headerLayout, sheet1, 3);

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
			ReportExcelExporter1.Export(UltraChart, labelChart.Text, sheet2, 1);

			sheet3.Rows[0].Cells[0].Value = labelMap.Text;
			sheet3.Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
			sheet3.Rows[0].Cells[0].CellFormat.Font.Height = 220;

			DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
			ReportExcelExporter.MapExcelExport(sheet3.Rows[1].Cells[0], DundasMap);
			sheet1.MergedCellsRegions.Clear();
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;

			sheet1.MergedCellsRegions.Add(3, 0, 4, 0);
			sheet1.MergedCellsRegions.Add(3, 1, 4, 1);
			sheet1.MergedCellsRegions.Add(3, 2, 4, 2);
			sheet1.MergedCellsRegions.Add(3, 3, 4, 3);
			sheet1.MergedCellsRegions.Add(3, 4, 4, 4);
			sheet1.MergedCellsRegions.Add(3, 5, 3, 6);
			sheet1.MergedCellsRegions.Add(3, 7, 3, 8);
			sheet2.MergedCellsRegions.Clear();
			sheet2.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet3.MergedCellsRegions.Clear();
			sheet3.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
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
