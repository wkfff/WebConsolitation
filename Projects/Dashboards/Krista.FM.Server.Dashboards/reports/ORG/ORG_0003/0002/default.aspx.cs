using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Microsoft.AnalysisServices.AdomdClient;

/**
 *  Информация о ценах на нефтепродукты, реализуемые через 
 *  АЗС и другие хозяйствующие субъекты на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0002
{
	public partial class Default : CustomReportPage
	{
		// --------------------------------------------------------------------

		// заголовок страницы
		private static String page_title_caption = "Информация о ценах по нефтепродуктам на {0} ({1})";
		// подзаголовок страницы
		private static String page_subtitle_caption = "Еженедельный мониторинг розничных цен на нефтепродукты в разрезе продавцов и видов топлива";
		// заголовок для UltraChart по динамике розничной цены
		private static String chart_title_caption = "Динамика розничной цены по {0}, рублей";
		private static String chart_title_error = "Для отображения динамики цен необходимо выбрать производителя";
		// заголовок для UltraChart по уровню цен
		private static String grid_chart_caption = "Уровень цен на различные виды топлива в разрезе продавцов на {0}, рублей";
		// заголовки для первого уровня таблицы
		private static String[] grid_columns_b0 = { "Наименование товаров, услуг", "Средняя цена, рублей" };
		// заголовки для второго уровня таблицы
		private static String[] grid_columns_b1 = { "Продавец" };
		// ширина экрана в пикселях
		private Int32 screen_width { get { return (int)Session["width_size"]; } }
		// высота экрана в пикселях
		private Int32 screen_height { get { return (int)Session["height_size"]; } }
		// параметр для последней актуальной даты
		private CustomParam last_date { get { return UserParams.CustomParam("last_date"); } }
		// параметр для последней актуальной даты
		private CustomParam first_date { get { return UserParams.CustomParam("first_date"); } }
		// параметр для выбранной.текущей территории
		private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
		// заголовки таблицы
		private static String[] grid_columns = { "Организация", "Дизельное топливо", "Бензин АИ-76", "Бензин АИ-92 (93)", "Бензин АИ-95 (96)", "Бензин АИ-98" };
		// таблица склонения имен месяцев
		private static Hashtable month_names = monthNames();
		// параметр запроса для региона
		private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
		// имя столбца заголовков для UltraChart (нигде не отображается)
		private static String chart_table_name = "series_name";
		// сообщения об ошибке при некорректной загрузке данных для UltraChart
		private static String chart_error_message = "в настоящий момент данные отсутствуют";
		// таблица преобразования имен месяцев в номера
		private static Hashtable month_numbers = monthNumbers();
		// --------------------------------------------------------------------

		private static string getBrowser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			// насройка размеров для элементов странички
			int k = 1;
			if (getBrowser == "Firefox")
			{
				k = 2;
			}
			else if (getBrowser == "AppleMAC-Safari")
			{
				k = 4;
			}
			grid.Width = CRHelper.GetGridWidth(screen_width - 38 - k);
			chart.Width = CRHelper.GetChartWidth(screen_width - 55);
			chart.Height = CRHelper.GetChartHeight((screen_height - 200) * 1 / 2);
			grid_chart.Width = CRHelper.GetChartWidth(screen_width - 55);
			grid_chart.Height = CRHelper.GetChartHeight((screen_height - 200) * 4 / 10);
			
			CustomMultiComboDate.Width = 300;
			CustomMultiComboDate.Title = "Выберите период";
			CustomMultiComboDate.ParentSelect = true;

			grid_chart.Tooltips.FormatString = "<ITEM_LABEL>";
			chart.Tooltips.FormatString = "<SERIES_LABEL>\n Розничная цена на <ITEM_LABEL>\n<b><DATA_VALUE:N2></b> рублей";
			grid_chart.ColumnChart.NullHandling = NullHandling.DontPlot;
			chart.LineChart.NullHandling = NullHandling.DontPlot;
		}

		// --------------------------------------------------------------------


		public void FillCustomMultiComboByDates(CustomMultiCombo cmb)
		{
			DataTable dtDate;
			string query = String.Empty;
			dtDate = new DataTable();
			query = DataProvider.GetQueryText("list_of_dates");
			string prev_year = String.Empty;
			string prev_month = String.Empty;
			Dictionary<string, int> dict_date = new Dictionary<string, int>();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			first_date.Value = null;
			for (int i = 0; i < dtDate.Rows.Count; ++i)
			{
				string yearD = UserComboBox.getLastBlock(dtDate.Rows[i][0].ToString());
				string monthD = UserComboBox.getLastBlock(dtDate.Rows[i][3].ToString());
				string dayD = UserComboBox.getLastBlock(dtDate.Rows[i][4].ToString());
				if (yearD != prev_year)
				{
					//CRHelper.SaveToUserAgentLog(String.Format("{0} {1}", yearD, 0));
					dict_date.Add(yearD + " год", 0);
					dict_date.Add(monthD + " " + yearD + " года", 1);
					prev_year = yearD;
					prev_month = monthD;
				}
				else
				{
					if (monthD != prev_month)
					{
						//CRHelper.SaveToUserAgentLog(String.Format("{0} {1}", monthD + " " + yearD + " года", 1));
						dict_date.Add(monthD + " " + yearD + " года", 1);
						prev_month = monthD;
					}
				}
				//CRHelper.SaveToUserAgentLog(String.Format("{0} {1}", dayD + " " + monthD + " " + yearD + " года", 2));
				monthD = CRHelper.RusMonthGenitive(CRHelper.MonthNum(monthD));
				dict_date.Add(dayD + " " + monthD + " " + yearD + " года", 2);
				if (first_date.Value == null)
				{
					first_date.Value = StringToMDXDate(dayD + " " + monthD + " " + yearD + " года");
				}
			}
			cmb.FillDictionaryValues(dict_date);
			cmb.SelectLastNode();
		}
		
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
				baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

				refresh_panel.AddLinkedRequestTrigger(grid);
				refresh_panel.AddRefreshTarget(chart);
				refresh_panel.AddRefreshTarget(grid_chart);


				// установка параметра территории
				current_region.Value = baseRegion.Value;

				FillCustomMultiComboByDates(CustomMultiComboDate);
			}
			string selectedText;
			if (CustomMultiComboDate.SelectedNode.Level == 0)
			{
				selectedText = CustomMultiComboDate.GetLastChild(CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode)).Text;
			}
			else
			{
				if (CustomMultiComboDate.SelectedNode.Level == 1)
				{
					selectedText = CustomMultiComboDate.SelectedNode.FirstNode.Text;
				}
				else
				{
					selectedText = CustomMultiComboDate.SelectedValue;
				}
			}
			last_date.Value = StringToMDXDate(selectedText);
			grid_chart_lable.Text = String.Format(grid_chart_caption, selectedText);
			page_title.Text = String.Format(page_title_caption, selectedText, UserComboBox.getLastBlock(current_region.Value));
			Page.Title = page_title.Text;
			page_subtitle.Text = page_subtitle_caption;

			// заполнение UltraWebGrid данными
			grid.DataBind();
			grid.Rows[0].Selected = true;
			// раскраска UltraWebGrid в различные цвета
			setColorsOfGrid(grid);
			// заполнение диаграммы UltraChart
			grid_chart.DataBind();

			webGridActiveRowChange(0, true);
		}

		// --------------------------------------------------------------------

		/** <summary>
		 *  Создание таблицы имен для преобразования названий месяцев 
		 *  </summary>
		 */
		private static Hashtable monthNames()
		{
			Hashtable table = new Hashtable();
			String[] data = 
            {
                "Январь", "января",
                "Февраль", "февраля",
                "Март", "марта",
                "Апрель", "апреля",
                "Май", "мая",
                "Июнь", "июня",
                "Июль", "июля",
                "Август", "августа",
                "Сентябрь", "сентября",
                "Октябрь", "октября",
                "Ноябрь", "ноября",
                "Декабрь", "декабря",
            };
			for (int i = 0; i < data.Length; i += 2)
			{
				table.Add(data[i], data[i + 1]);
			}
			return table;
		}

		// --------------------------------------------------------------------

		/** <summary>
		 *  Преобразование члена измерения времени в строку формата ЧЧ.ММ.ГГГГ
		 * 
		 *  до преобразования - [Период].[День].[Данные всех периодов].[2008].[Полугодие 2].[Квартал 4].[Декабрь].[1]
		 *  после преобразования - 1 декабря 2008
		 *  </summary>
		 */
		public static String mdxTime2String(String str)
		{
			if (str == null) return null;
			String[] list = str.Split('.');
			for (int i = 0; i < list.Length; ++i)
			{
				list[i] = list[i].Replace("[", "");
				list[i] = list[i].Replace("]", "");
			}
			return (list.Length > 7) ?
				list[7] + " " + month_names[list[6]] + " " + list[3] :
				null;
		}


		public string StringToMDXDate(string str)
		{
			string[] dateElements = str.Split(' ');
			string template = "[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
			int year = Convert.ToInt32(dateElements[2]);
			string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
			int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
			int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
			int day = Convert.ToInt32(dateElements[0]);
			return String.Format(template, year, halfYear, quarter, month, day);
		}
		

		// --------------------------------------------------------------------

		protected void grid_DataBinding(object sender, EventArgs e)
		{
			DataTable grid_master1 = new DataTable();
			DataTable grid_master2 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("all_measures"), "_", grid_master1);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("avg_measures"), "_", grid_master2);
			grid_master1.Rows.Add(grid_master2.Rows[0].ItemArray);
			for (int i = 0; i < grid_master1.Columns.Count; ++i)
				grid_master1.Columns[i].Caption = grid_columns[i];
			grid.DataSource = grid_master1.DefaultView;
		}

		// --------------------------------------------------------------------

		protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			// насройка столбцов в UltraWebGrid 
			double tempWidth = grid.Width.Value - 2;
			e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.35) - 5;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			for (int i = 1; i < grid.Bands[0].Columns.Count; i++)
			{
				e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth - 20) * 0.65 / (grid.Bands[0].Columns.Count - 1)) - 5;
				// установка формата отображения данных в UltraWebGrid
				CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "N2");
			}
		}

		// --------------------------------------------------------------------

		protected void chart_DataBinding(object sender, EventArgs e)
		{
			if (grid.Rows[grid.Rows.Count - 1].Activated)
			{
				return;
			}
			DataTable chart_table = new DataTable();
			// формирование DataTable для диаграммы цен за три месяца по выбранному продавцу
			cellSet2DataTable(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart")), chart_table, true);
			// форматирование столбцовых заголовков
			foreach (DataColumn col in chart_table.Columns)
			{
				if (col.Ordinal > 0)
					col.ColumnName = mdxDateToString(col.ColumnName, true);
			}
			// настройка отображения UltraChart
			setUltraChartRanges(chart, chart_table);
			// установка источника данных для диаграммы цен по выбранному производителю
			chart.DataSource = chart_table == null ? null : chart_table.DefaultView;
		}

		// --------------------------------------------------------------------
		/** <summary>
		 *  Метод заполнения DataTable из CellSet
		 *  примечание: надо бы проверить может и не только для этой страницы пригодится
		 *  </summary>
		 */
		public static void cellSet2DataTable(CellSet chart_set, DataTable chart_table, bool columns_by_name)
		{
			// проверка на наличие данных в таблице CellSet
/*			if (chart_set.Cells.Count == 0)
			{
				throw new Exception("cell set is empty");
			}*/
			// заполнение заголовков столбцов
			chart_table.Columns.Add(chart_table_name);
			foreach (Position p in chart_set.Axes[0].Positions)
			{
				DataColumn dc = chart_table.Columns.Add(
					columns_by_name ? p.Members[0].ToString() : p.Members[0].Caption);
				dc.DataType = typeof(Decimal);
			}
			// заполнение заголовков для строк
			foreach (Position p in chart_set.Axes[1].Positions)
			{
				chart_table.Rows.Add(p.Members[0].Caption);
			}
			// заполнение таблицы данными
			int row_index = 0;
			object[] values = null;
			foreach (DataRow row in chart_table.Rows)
			{
				values = row.ItemArray;
				foreach (DataColumn col in chart_table.Columns)
				{
					if (col.Ordinal != 0)
					{
						values[col.Ordinal] = chart_set.Cells[col.Ordinal - 1, row_index].Value;
					}
				}
				row.ItemArray = values;
				++row_index;
			}
		}

		
		protected void grid_chart_DataBinding(object sender, EventArgs e)
		{
			DataTable chart_table = new DataTable();
			// заполнение таблицы для диаграммы уровня цен на топливо
			cellSet2DataTable(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid_chart")), chart_table, false);
			// настройка отображения UltraChart
			setUltraChartRanges(grid_chart, chart_table);
			grid_chart.DataSource = chart_table == null ? null : chart_table.DefaultView;
		}

		// --------------------------------------------------------------------

		protected void grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
		{
			// Обработчик отваливается после второго нажатия потому обработка идёт по ячейкам
			// смотреть grid_ActiveCellChange
			if (e.Row.Index != (grid.Rows.Count - 1))
			{
				webGridActiveRowChange(e.Row.Index, true);
			}
		}

		// --------------------------------------------------------------------

		/** <summary>
		 *  Настройка формата сообщения об ошибке для UltraChart
		 *  </summary>
		 */
		public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			e.Text = chart_error_message;
			e.LabelStyle.Font = new Font("Verdana", 20);
			e.LabelStyle.FontColor = Color.LightGray;
			e.LabelStyle.VerticalAlign = StringAlignment.Center;
			e.LabelStyle.HorizontalAlign = StringAlignment.Center;
		}

		// --------------------------------------------------------------------

		public static void setChartSettings(UltraChart chart)
		{
			chart.Legend.BackgroundColor = Color.Empty;
			chart.Legend.BorderColor = Color.Empty;
		}

		/** <summary>
		 *  Преобразование члена измерения времени в строку формата ЧЧ.ММ.ГГГГ
		 * 
		 *  до преобразования - [Период].[День].[Данные всех периодов].[2008].[Полугодие 2].[Квартал 4].[Декабрь].[1]
		 *  после преобразования - 01.12.2008
		 *  </summary>
		 */

		public string mdxDateToString(string mdxDate, bool IsShortDate)
		{
			string[] dateElements = mdxDate.Replace("]", String.Empty).Replace("[", String.Empty).Split('.');
			string day = dateElements[7];
			string monthNum = CRHelper.MonthNum(dateElements[6]).ToString();
			string month = CRHelper.RusMonthGenitive(CRHelper.MonthNum(dateElements[6]));
			string year = dateElements[3];
			if (IsShortDate)
			{
				if (day.Length == 1)
				{
					day = "0" + day;
				}
				if (monthNum.Length == 1)
				{
					monthNum = "0" + monthNum;
				}
				return (day + "." + monthNum + "." + year.Substring(2, 2));
			}
			else
			{
				return (day + " " + month + " " + year + " года");
			}
		}

		public static string mdxTime2String(String str, Boolean IsShortDate)
		{
			if (str == null) return null;
			string[] list = str.Split('.');
			for (int i = 0; i < list.Length; ++i)
			{
				list[i] = list[i].Replace("[", "");
				list[i] = list[i].Replace("]", "");
			}
			string result = (list.Length > 7) ?
				list[7] + "." + month_numbers[list[6]] + "." + list[3] :
				null;
			if (result.Length == 9)
			{
				result = "0" + result;
			}
			if (IsShortDate)
			{
				result = result.Substring(0, 6) + result.Substring(8);
			}
			return result;
		}
		
		protected void chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			setChartErrorFont(e);
		}

		// --------------------------------------------------------------------

		protected void grid_chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			setChartErrorFont(e);
		}

		// --------------------------------------------------------------------

		/** <summary>
		 *  Метод получения последней актуальной даты 
		 *  </summary>
		 */
		private String getLastDate()
		{
			CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
			return cs.Axes[1].Positions[0].Members[0].ToString();
		}

		// --------------------------------------------------------------------

		/** <summary>
		 *  Установка параметров для UltraChart (избавляемся от пустых ячеек)
		 *  </summary>
		 */
		public static void setUltraChartRanges(UltraChart chart, DataTable chart_table)
		{
			if (chart_table == null) return;

			double min = Double.PositiveInfinity;
			double max = Double.NegativeInfinity;
			// 1. вычисление максимального и минимального элементов в таблице
			// 2. заполнение пустых ячеек очень большими числами
			for (int i = 0; i < (chart_table.Rows.Count - 1); ++i)
			{
				for (int j = 1; j < chart_table.Columns.Count; ++j)
				{
					double value = 0;
					if (Double.TryParse(chart_table.Rows[i][j].ToString(), out value))
					{
						if (value > max)
						{
							max = value;
						}
						if (value < min)
						{
							min = value;
						}
					}
				}
			}
			// установка настроек для UltraChart
			chart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
			chart.Axis.Y.RangeMin = min * 0.9;
			chart.Axis.Y.RangeMax = max * 1.1;

/*			chart.Data.UseMinMax = true;
			chart.Data.MinValue = min;
			chart.Data.MaxValue = max;*/
		}

		// --------------------------------------------------------------------

		/** <summary>
		 *  Настройка цветов отображения для UltraWebGrid
		 *  </summary>
		 */
		public static void setColorsOfGrid(UltraWebGrid grid)
		{
			for (int j = 1; j < grid.Columns.Count; ++j)
			{
				double min = Double.PositiveInfinity;
				double max = Double.NegativeInfinity;
				string minColumnCells = String.Empty;
				string maxColumnCells = String.Empty;
				for (int i = 0; i < (grid.Rows.Count - 1); ++i)
				{
					double value = 0;
					if (Double.TryParse(grid.Rows[i].Cells[j].ToString(), out value))
					{
						if (value == min)
						{
							minColumnCells += " " + i;
						}
						if (value == max)
						{
							maxColumnCells += " " + i;
						}
						if (value > max)
						{
							max = value;
							maxColumnCells = i.ToString();
						}
						if (value < min)
						{
							min = value;
							minColumnCells = i.ToString();
						}
					}
				}
				if (!Double.IsInfinity(min))
				{
					string[] minCells = minColumnCells.Split(' ');
					for (int i = 0; i < minCells.Length; ++i)
					{
						grid.Rows[Convert.ToInt32(minCells[i])].Cells[j].Style.BackgroundImage = "~/images/starYellowBB.png";
						grid.Rows[Convert.ToInt32(minCells[i])].Cells[j].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
						grid.Rows[Convert.ToInt32(minCells[i])].Cells[j].Title = "Самый низкий уровень цены";
					}
				}
				if (!Double.IsInfinity(max))
				{
					string[] maxCells = maxColumnCells.Split(' ');
					for (int i = 0; i < maxCells.Length; ++i)
					{
						grid.Rows[Convert.ToInt32(maxCells[i])].Cells[j].Style.BackgroundImage = "~/images/starGrayBB.png";
						grid.Rows[Convert.ToInt32(maxCells[i])].Cells[j].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
						grid.Rows[Convert.ToInt32(maxCells[i])].Cells[j].Title = "Самый высокий уровень цены";

					}
				}
			}
		}

/*
		public static void setColorsOfGrid(UltraWebGrid grid)
		{
			Color[] colors = { Color.LightGreen, Color.LightCoral };
			for (int i = 1; i < grid.Columns.Count; ++i)
			{
				bool init = false;
				decimal min = 0, max = 0;
				int i_min = -1, i_max = -1;
				grid.Columns[i].Header.Style.Wrap = true;
				foreach (UltraGridRow row in grid.Rows)
				{
					if (row.Index == grid.Rows.Count - 1) continue;
					decimal value = Convert.ToDecimal(row.Cells[i].Value);
					if (value == 0)
					{
						row.Cells[i].Text = "-";
						continue;
					}
					if (min > value || !init)
					{
						min = value;
						i_min = row.Index;
					}
					if (max < value || !init)
					{
						max = value;
						i_max = row.Index;
					}
					init = true;
				}
				if (init)
				{
					grid.Rows[i_min].Cells[i].Style.BackColor = colors[0];
					grid.Rows[i_max].Cells[i].Style.BackColor = colors[1];
				}
			}
			grid.Rows[grid.Rows.Count - 1].Cells[0].Text = "Средняя цена";
			foreach (UltraGridColumn Col in grid.Columns)
			{
			}
		}
*/
		// --------------------------------------------------------------------

		/** <summary>
		 *  Устанавливает текущую строку в таблице
		 *  параметры:
		 *      row - настраиваемая строка;
		 *      active - устанавливать или нет свойство 'Activate' у строки.
		 *  </summary>
		 */
		private void webGridActiveRowChange(int index, bool active)
		{
			// получаем выбранную строку
			UltraGridRow row = grid.Rows[index];
			// устанавливаем ее активной, если необходимо
			if (active)
			{
				row.Activate();
				row.Activated = true;
			}
			String seller = grid.Rows[index].Cells[0].Value.ToString();
			// устанавливаем параметр текущего производителя/продавца
			UserParams.CustomParam("seller_name").Value = seller;
			chart.DataBind();
			// установка заголовка для диаграммы
			chart_lable.Text = String.Format(chart_title_caption, seller);
			chart_lable.Width = new Unit(chart.Width.Value - 50);
		}

		// --------------------------------------------------------------------

		protected void chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
		{
			int xOct = 0;
			int xNov = 0;
			Text decText = null;
			string year = mdxTime2String(last_date.Value);

			CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("prev_date"));

			string year1 = mdxTime2String(cs.Axes[1].Positions[1].Members[0].ToString());
			string year2 = mdxTime2String(cs.Axes[1].Positions[0].Members[0].ToString());

			foreach (Primitive primitive in e.SceneGraph)
			{
				if (primitive is Text)
				{
					Text text = primitive as Text;

					if (year2 == text.GetTextString())
					{
						xOct = text.bounds.X;
						continue;
					}
					if (year1 == text.GetTextString())
					{
						xNov = text.bounds.X;
						decText = new Text();
						decText.bounds = text.bounds;
						decText.labelStyle = text.labelStyle;
						continue;
					}
				}
				if (decText != null)
				{
					decText.bounds.X = xNov + (xNov - xOct);
					decText.SetTextString(year);
					e.SceneGraph.Add(decText);
					break;
				}
			}
		}

		
		// --------------------------------------------------------------------
		private static Hashtable monthNumbers()
		{
			Hashtable table = new Hashtable();
			string[] data = 
            {
                "Январь", "01",
                "Февраль", "02",
                "Март", "03",
                "Апрель", "04",
                "Май", "05",
                "Июнь", "06",
                "Июль", "07",
                "Август", "08",
                "Сентябрь", "09",
                "Октябрь", "10",
                "Ноябрь", "11",
                "Декабрь", "12",
            };
			for (int i = 0; i < data.Length; i += 2)
			{
				table.Add(data[i], data[i + 1]);
			}
			return table;
		}
	}
}