using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;

/**
 *  Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0004
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtDate;
		private DataTable dtMap1;
		private DataTable dtMap2;
		private DataTable dtGrid;
		private DataTable dtChart;
		private DataTable medianDT;
		private GridHeaderLayout headerLayout;

		#endregion

		// имя папки с картами региона
        private const string mapFolderName = "ХМАО";

		private static bool IsMozilla
		{
			get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
		}

		private static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		#region Параметры запроса

		private CustomParam selectedDate;
		private CustomParam compareDate;
		private CustomParam selectedYear;
		private CustomParam compareYear;

		#endregion
		// --------------------------------------------------------------------

		private const string PageTitleCaption = "Задолженность по заработной плате";
		private const string PageSubTitleCaption = "Данные еженедельного мониторинга задолженности по заработной плате в ХМАО-Югре по состоянию на {0}";
		private const string Map1TitleCaption = "Задолженность по оплате труда по муниципальным образованиям на {0}";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			ComboCompareDate.Title = "Выберите дату для сравнения";
			ComboCompareDate.Width = 400;
			ComboCompareDate.ParentSelect = true;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

			#region Параметры
			selectedDate = UserParams.CustomParam("selected_date");
			compareDate = UserParams.CustomParam("compare_date");
			selectedYear = UserParams.CustomParam("selected_year");
			compareYear = UserParams.CustomParam("compare_year");
			#endregion

			#region Ссылки
			CrossLink1.Text = "Задолженность&nbspпо&nbspоплате&nbspтруда&nbsp(по&nbspмуниципальным&nbspобразованиям ХМАО-Югры)";
			CrossLink1.NavigateUrl = "~/reports/STAT_0003_0003/Default.aspx";
			CrossLink2.Text = "Мониторинг&nbspситуации&nbspна&nbspрынке&nbspтруда&nbspв&nbspсубъектах&nbspРФ, входящих&nbspв&nbspУрФО";
			CrossLink2.NavigateUrl = "http://urfo.ifinmon.ru/reports/STAT_0001_0002/Default.aspx";
			CrossLink3.Text = "Мониторинг&nbspситуации&nbspна&nbspрынке&nbspтруда&nbspв&nbspсубъектах&nbspРФ, входящих&nbspвУрФО&nbsp(по&nbspтерритории)";
			CrossLink3.NavigateUrl = "http://urfo.ifinmon.ru/reports/STAT_0001_0003/Default.aspx";
			#endregion

			#region Карта

			DundasMap1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
			DundasMap1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);
			LabelMap1.Width = DundasMap1.Width;

			DundasMap1.Meridians.Visible = false;
			DundasMap1.Parallels.Visible = false;
			DundasMap1.ZoomPanel.Visible = true;
			DundasMap1.NavigationPanel.Visible = true;
			DundasMap1.Viewport.EnablePanning = true;
			DundasMap1.Viewport.Zoom = 100;
			DundasMap1.ColorSwatchPanel.Visible = false;

			DundasMap1.Legends.Clear();
			// добавляем легенду
			Legend legend = new Legend("CostLegend");
			legend.Visible = true;
			legend.BackColor = Color.White;
			legend.Dock = PanelDockStyle.Right;
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
			legend.Title = "Сумма задолженности по выплате заработной платы,\nтысяч рублей";
			legend.AutoFitMinFontSize = 7;

			// добавляем легенду с символами
			Legend legend2 = new Legend("SymbolLegend");
			legend2.Visible = true;
			legend2.Dock = PanelDockStyle.Right;
			legend2.BackColor = Color.White;
			legend2.BackSecondaryColor = Color.Gainsboro;
			legend2.BackGradientType = GradientType.DiagonalLeft;
			legend2.BackHatchStyle = MapHatchStyle.None;
			legend2.BorderColor = Color.Gray;
			legend2.BorderWidth = 1;
			legend2.BorderStyle = MapDashStyle.Solid;
			legend2.BackShadowOffset = 4;
			legend2.TextColor = Color.Black;
			legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
			legend2.AutoFitText = true;
			legend2.Title = "Количество граждан,\nперед которыми имеется задолженность";
			legend2.AutoFitMinFontSize = 7;

			DundasMap1.Legends.Add(legend2);
			DundasMap1.Legends.Add(legend);

			// добавляем правила раскраски
			DundasMap1.ShapeRules.Clear();
			ShapeRule rule = new ShapeRule();
			rule.Name = "CostRule";
			rule.Category = String.Empty;
			rule.ShapeField = "Cost";
			rule.DataGrouping = DataGrouping.EqualDistribution;
			rule.ColorCount = 5;
			rule.ColoringMode = ColoringMode.ColorRange;
			rule.FromColor = Color.Green;
			rule.MiddleColor = Color.Yellow;
			rule.ToColor = Color.Red;
			rule.BorderColor = Color.FromArgb(50, Color.Black);
			rule.GradientType = GradientType.None;
			rule.HatchStyle = MapHatchStyle.None;
			rule.ShowInLegend = "CostLegend";
			rule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
			DundasMap1.ShapeRules.Add(rule);

			// добавляем поля
			DundasMap1.Shapes.Clear();
			DundasMap1.ShapeFields.Add("Name");
			DundasMap1.ShapeFields["Name"].Type = typeof(string);
			DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
			DundasMap1.ShapeFields.Add("Cost");
			DundasMap1.ShapeFields["Cost"].Type = typeof(double);
			DundasMap1.ShapeFields["Cost"].UniqueIdentifier = false;

			// добавляем поля для символов
			DundasMap1.SymbolFields.Add("UnemploymentPopulation");
			DundasMap1.SymbolFields["UnemploymentPopulation"].Type = typeof(double);
			DundasMap1.SymbolFields["UnemploymentPopulation"].UniqueIdentifier = false;

			// добавляем правила расстановки символов
			DundasMap1.SymbolRules.Clear();
			SymbolRule symbolRule = new SymbolRule();
			symbolRule.Name = "SymbolRule";
			symbolRule.Category = string.Empty;
			symbolRule.DataGrouping = DataGrouping.EqualInterval;
			symbolRule.SymbolField = "UnemploymentPopulation";
			symbolRule.ShowInLegend = "SymbolLegend";
			symbolRule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
			DundasMap1.SymbolRules.Add(symbolRule);

			// звезды для легенды
			for (int i = 1; i < 4; i++)
			{
				PredefinedSymbol predefined = new PredefinedSymbol();
				predefined.Name = "PredefinedSymbol" + i;
				predefined.MarkerStyle = MarkerStyle.Triangle;
				predefined.Width = 5 + (i * 5);
				predefined.Height = predefined.Width;
				predefined.Color = Color.DarkViolet;
				DundasMap1.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
			}

			AddMapLayer(DundasMap1, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
			AddMapLayer(DundasMap1, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);

			#endregion

			#region Экспорт
			
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			//ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			
			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate(ComboDate, "STAT_0003_0004_list_of_dates", 0);
				FillComboDate(ComboCompareDate, "STAT_0003_0004_list_of_dates", 1);
			}
			#region Анализ параметров
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
			selectedYear.Value = GetYearFromMDXDate(selectedDate.Value);
			compareYear.Value = GetYearFromMDXDate(compareDate.Value);
			if (selectedYear.Value == compareYear.Value)
			{
				compareYear.Value = Convert.ToString(Convert.ToUInt32(selectedYear.Value) - 1);
			}

			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, MDXDateToShortDateString(selectedDate.Value));
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);

			UltraWebGrid.DataBind();
			FillMap1Data();
		}

		// --------------------------------------------------------------------
		#region Обработчики карты 1

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

		public void FillMap1Data()
		{
			string query = DataProvider.GetQueryText("STAT_0003_0004_map1");
			dtMap1 = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtMap1);
			bool hasCallout;
			string valueSeparator = IsMozilla ? ". " : "\n";
			string shapeHint = "{0}\nКоличество граждан, перед которыми имеется задолженность: {1:N0} чел.\nСумма задолженности по выплате заработной платы: {2:N3} тысяч рублей";
			LabelMap1.Text = String.Format(Map1TitleCaption, MDXDateToShortDateString(selectedDate.Value));
			if (dtMap1 == null || DundasMap1 == null || dtMap1.Columns.Count < 5)
			{
				return;
			}
			foreach (Shape shape in DundasMap1.Shapes)
			{
				shape.Text = String.Format("{0}", GetShapeName(shape).Replace(" ", "\n"));
				if (shape.Text.Substring(0, 2) != "г.")
				{
					shape.Text = shape.Text + " р-н";
					shape.TextVisibility = TextVisibility.Shown;
				}
			}
			foreach (DataRow row in dtMap1.Rows)
			{
				// заполняем карту данными
				string subject = row[0].ToString().Replace(" муниципальный район", String.Empty).Replace("Город ", "г.");
				double value = Convert.ToDouble(row[4].ToString());
				double unemploymentPopulation = Convert.ToDouble(row[3].ToString());
				ArrayList shapeList = FindMapShape(DundasMap1, subject, out hasCallout);
				foreach (Shape shape in shapeList)
				{
					shape.Visible = true;
					string shapeName = GetShapeName(shape);
					shape["Name"] = subject;
					shape["Cost"] = Convert.ToDouble(row[4].ToString());
					if (IsCalloutTownShape(shape))
					{
						shape.ToolTip = String.Format(shapeHint, row[0].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
						shape.TextVisibility = TextVisibility.Shown;
						shape.Text = String.Format("{0}\n{1:N0} чел.\n{2:N3} т.р.", row[1].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
						shape.TextVisibility = TextVisibility.Shown;
						shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
					}
					else
					{
						if (!hasCallout)
						{
							shape.ToolTip = String.Format(shapeHint, row[0].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
							shape.TextVisibility = TextVisibility.Shown;
							shape.Text = String.Format("{0}\n{1:N0} чел.\n{2:N3} т.р.", row[1].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
							Symbol symbol = new Symbol();
							symbol.Name = shape.Name + DundasMap1.Symbols.Count;
							symbol.ParentShape = shape.Name;
							symbol["UnemploymentPopulation"] = unemploymentPopulation;
							symbol.Color = Color.DarkViolet;
							symbol.MarkerStyle = MarkerStyle.Triangle;
							symbol.Offset.Y = -30;
							symbol.Layer = CRHelper.MapShapeType.Areas.ToString();
							DundasMap1.Symbols.Add(symbol);
						}
						else
						{
							shape.Visible = false;
						}
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------
		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("STAT_0003_0004_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				UltraWebGrid.DataSource = dtGrid;
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
			e.Layout.NullTextDefault = "-";
			e.Layout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
			e.Layout.Bands[0].Columns[0].MergeCells = true;
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			int columnWidth = Convert.ToInt32((Convert.ToInt32(e.Layout.Grid.Width.Value) - CRHelper.GetColumnWidth(300)) / 8);
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Территория");
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				GridHeaderCell headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
				headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
			}
			headerLayout.ApplyHeaderInfo();
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			e.Row.Cells[0].Value = e.Row.Cells[0].GetText().Split(';')[0];
			for (int i = 1; i < e.Row.Cells.Count; ++i)
			{
				UltraGridCell cell = e.Row.Cells[i];
				if (e.Row.Index % 3 != 0)
				{
					cell.Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				if (e.Row.Index % 3 != 2)
				{
					cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				if (e.Row.Index % 3 == 1)
				{
					cell.Title = "Темп прироста к " + MDXDateToShortDateString(compareDate.Value);
					if (cell.Value != null && Convert.ToDouble(cell.Value) != 0)
					{
						double value = Convert.ToDouble(cell.Value);
						if (value < 0)
						{
							cell.Style.BackgroundImage = "~/images/ArrowGreenDownBB.png";
							cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
						}
						else
						{
							cell.Style.BackgroundImage = "~/images/ArrowRedUpBB.png";
							cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
						}
					}
				}
				if (e.Row.Index % 3 == 2)
				{
					if (cell.Value != null && Convert.ToDouble(cell.Value) != 0)
					{
						double value = Convert.ToDouble(cell.Value);
						if (value > 0)
						{
							cell.Title = "Прирост к " + MDXDateToShortDateString(compareDate.Value);
						}
						else
						{
							cell.Title = "Снижение относительно " + MDXDateToShortDateString(compareDate.Value);
						}
					}
				}
				switch (e.Row.Index % 3)
				{
					case 0:
						{
							if (i == 4 || i == 7)
							{
								cell.Value = String.Format("<b>{0:N2}</b>", cell.Value);
							}
							else
							{
								cell.Value = String.Format("<b>{0:N0}</b>", cell.Value);
							}
							break;
						}
					case 1:
						{
							cell.Value = String.Format("{0:P2}", cell.Value);
							break;
						}
					case 2:
						{
							cell.Value = String.Format("{0:N0}", cell.Value);
							break;
						}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
		{
			// Загрузка списка актуальных дат
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			// Закачку придется делать через словарь
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

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

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
			string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
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

		public string GetYearFromMDXDate(string mdxDate)
		{
			string[] separator = { "].[" };
			string[] mdxDateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (mdxDateElements.Length == 8)
			{
				return mdxDateElements[3];
			}
			else
			{
				return "2010";
			}
		}

		#endregion

		#region Экспорт в PDF
		
		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();

			foreach (UltraGridRow row in UltraWebGrid.Rows)
				foreach (UltraGridCell cell in row.Cells)
				{
					if (cell.Value != null)
					{
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
					}
				}

			IText title = section1.AddText();
			Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.Style.Font.Bold = true;
			title.AddContent(PageTitle.Text);

			title = section1.AddText();
			font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(PageSubTitle.Text);

			foreach (UltraGridRow row in headerLayout.Grid.Rows)
			{
				if (row.Index % 3 != 0)
				{
					row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
				if (row.Index % 3 != 2)
				{
					row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
			}

			ReportPDFExporter1.HeaderCellHeight = 60;
			ReportPDFExporter1.Export(headerLayout, section1);

			section2.PageOrientation = PageOrientation.Landscape;
			title = section2.AddText();
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(LabelMap1.Text);
			DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
			DundasMap1.ZoomPanel.Visible = false;
			DundasMap1.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap1);
			section2.AddImage(img);
		}
		
		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			int startRow = 5;
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Карта");
			Worksheet sheet3 = workbook.Worksheets.Add("Потом удалю");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 80;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;
			
			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				foreach (UltraGridCell cell in row.Cells)
				{
					if (cell.Value != null)
					{
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
					}
				}
			}

			ReportExcelExporter1.Export(headerLayout, sheet1, startRow - 2);
			
			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				sheet1.Rows[startRow + row.Index].Height = 255;
			}
			sheet1.Rows[startRow - 1].Height = 255;
			
			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Height = 1020;
			for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
					sheet1.Rows[startRow + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[startRow + 1 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[startRow + 1 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[startRow + 2 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
				}
			}
			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			sheet2.Rows[0].Cells[0].Value = LabelMap1.Text;
			DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
			DundasMap1.Height = Unit.Pixel((int)(CustomReportConst.minScreenHeight * 0.7));
			ReportExcelExporter.MapExcelExport(sheet2.Rows[1].Cells[0], DundasMap1);

			// Два дня убил - чтобы применились ручные изменения - необходимо чтобы был вызван базовый метод Export
			// Поскольку экспортировать больше нечего - экспортируем еще раз грид и грохаем лист
			ReportExcelExporter1.Export(headerLayout, sheet3, 2);
			workbook.Worksheets.Remove(sheet3);
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.0;
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
