using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Microsoft.AnalysisServices.AdomdClient;

/**
 *  Розничные цены на продовольственные товары первой необходимости по данным на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0001
{
	public partial class Default : CustomReportPage
	{
		// --------------------------------------------------------------------

		// заголовок страницы
		private static String page_title_caption = "Розничные цены на основные продовольственные товары по данным на {0} <nobr>({1})</nobr>";
		private static String page_sub_title_caption = "Еженедельный мониторинг розничных цен на основные продовольственные товары";
		// заголовок для UltraChart
		private static String chart_title_caption = "Динамика розничной цены на товар \"{0}\", рублей";
		private static String dinamic_report_text = "";
		// заголовки столбцов для UltraWebGrid
		private static String[] grid_columns = { "Наименование продукта", "Ед. изм.", "Цена, рублей", "unique_name" };
		// таблица преобразования имен товаров
		private static Hashtable grid_names = foodstuffNames();
		// таблица склонения имен месяцев
		private static Hashtable month_names = monthNames();
		// таблица преобразования имен месяцев в номера
		private static Hashtable month_numbers = monthNumbers();
		// таблица преобразования номеров месяцев в имена
		private static Hashtable month_names_by_numbers = monthNamesByNumbers();
		// имя столбца заголовков для UltraChart (нигде не отображается)
		private static String chart_table_name = "series_name";
		// сообщения об ошибке при некорректной загрузке данных для UltraChart
		private static String chart_error_message = "в настоящий момент данные отсутствуют";
		// ширина экрана в пикселях
		private int screen_width { get { return (int)Session["width_size"]; } }
		// высота экрана в пикселях
		private int screen_height { get { return (int)Session["height_size"]; } }
		// выбранная строка грида
		private static int selected_grid_row = 0;
		// параметр для выбранной.текущей территории
		private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
		// Количество выбираемых дат для одного товара
		private CustomParam count_grid { get { return (UserParams.CustomParam("count_grid")); } }
		// Количество выбираемых дат для одного товара
		private CustomParam count_chart { get { return (UserParams.CustomParam("count_chart")); } }
		// параметр для последней актуальной даты
		private CustomParam on_date { get { return (UserParams.CustomParam("on_date")); } }
		// параметр для последней актуальной даты
		private CustomParam last_date { get { return (UserParams.CustomParam("last_date")); } }
		// параметр для первой актуальной даты
		private CustomParam first_date { get { return (UserParams.CustomParam("first_date")); } }
		// параметр для набора первых дат месяцев
		private CustomParam dates { get { return (UserParams.CustomParam("dates")); } }
		// параметр для набора первых дат месяцев для диаграммы
		private CustomParam dates_chart { get { return (UserParams.CustomParam("dates_chart")); } }
		// параметр запроса для региона
		private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
		// словарь - дата - mdx дата
		private static Dictionary<string, string> dates_dict = new Dictionary<string, string>();
		// словарь - дата - месяц
		private static Dictionary<string, string> months_dict = new Dictionary<string, string>();
		// словарь - месяц - первый день месяца с данными
		private static String[] months = { };
		private static String[] days = { };

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			// установка размера диаграммы
			web_grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.345);
			web_grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 1);
			warp_web_grid.Width = web_grid.Width;
			chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.345);
			chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.985);
			chart.Tooltips.FormatString = "Розничная цена на <ITEM_LABEL>\n<b><DATA_VALUE:N2></b> рублей";
		}

		// --------------------------------------------------------------------
		private int GetDotsNumber(string s)
		{
			int result = 0;
			for (int i = 0; i < s.Length; ++i)
			{
				if (s[i] == '.')
				{
					++result;
				}
			}
			return result;
		}


		protected override void Page_Load(object sender, EventArgs e)
		{

			base.Page_Load(sender, e);
			// Настраиваем обновление грида в зависимости от состояния чек-бокса
			//warp_web_grid.AddLinkedRequestTrigger(cbOnMonthBegin.ClientID);
			// Переменные, использующиес при формировании списка дат
			string year = "", month = "", last_value = "";
			string month_to_dict = "";
			bool new_month = true;
			string tmp_value = "";
			if (!Page.IsPostBack)
			{   // опрерации которые должны выполняться при только первой загрузке страницы
				RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
				baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
				// установка параметра территории
				current_region.Value = baseRegion.Value;

				#region Формирование списка актуальных дат
				// Будем формировать список дат
				first_date.Value = null;
				CellSet date_set = null;
				// Выполнение запроса
				date_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("get_dates"));
				if (date_set.Cells.Count == 0)
				{
					throw new Exception("Данные по актуальным датам не найдены");
				}
				Dictionary<string, int> date_dictionary = new Dictionary<string, int>();
				// Цикл по всем полученным значениям
				Array.Resize(ref months, 0);
				Array.Resize(ref days, 0);
				for (int i = 1; i < date_set.Axes[1].Positions.Count; ++i)
				{
					// Полное значение числа в формате
					// [Период].[День].[Данные всех периодов].[Год].[Полугодие].[Квартал].[Месяц].[День]
					string s = date_set.Axes[1].Positions[i].Members[0].UniqueName;
					string UniqueName = s;
					// Отрезаем [Период].[День].[Данные всех периодов].
					s = s.Replace("[", String.Empty).Replace("]", String.Empty);
					string[] dateElements = s.Split('.');
					// Тут отбраковываем полугодия и кварталы
					if ((dateElements.Length == 4) || (dateElements.Length > 6))
					{
						int level = 0;
						// Формируем красивую строчку с описанием даты
						// Если год, то такой-то год
						if (dateElements.Length == 4)
						{
							level = 0;
							year = dateElements[3];
							s = year + " год";
						}
						else
						{
							// Если месяц, то такой-то месяц такого-то года
							if (dateElements.Length == 7)
							{
								level = 1;
								month = dateElements[6];
								month_to_dict = UniqueName;
								s = month + " " + year + " года";
								new_month = true;
							}
							// Если не год и не месяц, то такой-то день такого-то месяца такого-то года
							else
							{
								if (new_month)
								{
									int new_size = months.Length + 1;
									Array.Resize(ref months, new_size);
									Array.Resize(ref days, new_size);
									months[new_size - 1] = month_to_dict;
									days[new_size - 1] = UniqueName;
									new_month = false;
								}
								if (first_date.Value == null)
								{
									first_date.Value = date_set.Axes[1].Positions[i].Members[0].ToString();
								}
								s = dateElements[7] + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(dateElements[6])) + " " + year + " года";
								level = 2;
								if (!months_dict.ContainsKey(UniqueName))
								{
									months_dict.Add(UniqueName, month_to_dict);
								}
								if (!dates_dict.ContainsKey(s))
								{
									dates_dict.Add(s, UniqueName);
								}
							}
						}
						//int value;
						//if (!date_dictionary.TryGetValue(s, out value))
						//{
							date_dictionary.Add(s, level);
						//}
						// Запоминаем значения, чтобы потом выделить последнее
						last_value = s;
					}
				}
				// Запихиваем полученный список в CustomMultiCombo
				CustomMultiComboDate.FillDictionaryValues(date_dictionary);
				// Выделяем по умолчанию последнюю дату
				CustomMultiComboDate.SetСheckedState(last_value, true);
				dates_dict.TryGetValue(last_value, out tmp_value);
				last_date.Value = tmp_value;
				#endregion

				warp_chart.AddLinkedRequestTrigger(web_grid);
			}
			// установка параметра территории
			current_region.Value = baseRegion.Value;

			// установка заголовка для страницы
			dates_dict.TryGetValue(CustomMultiComboDate.SelectedValue, out tmp_value);
			on_date.Value = tmp_value;
			string string_date = String.Empty;
			if (!cbOnMonthBegin.Checked)
			{
				if (CustomMultiComboDate.SelectedNode.Level == 0)
				{
					string_date = CustomMultiComboDate.GetLastChild(CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode)).Text;
					page_title.Text = String.Format(
						page_title_caption, CustomMultiComboDate.GetLastChild(CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode)).Text,
						UserComboBox.getLastBlock(current_region.Value));
				}
				else
				{
					if (CustomMultiComboDate.SelectedNode.Level == 1)
					{
						string_date = CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode).Text;
						page_title.Text = String.Format(
							page_title_caption, CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode).Text,
							UserComboBox.getLastBlock(current_region.Value));
					}
					else
					{
						string_date = CustomMultiComboDate.SelectedValue;
						page_title.Text = String.Format(
							page_title_caption, CustomMultiComboDate.SelectedValue,
							UserComboBox.getLastBlock(current_region.Value));
					}
				}
			}
			else
			{
				// Надо в заголовок правильную дату вывести
				if (CustomMultiComboDate.SelectedNode.Level == 0)
				{
					// если выбран год, то находим первую актуальную дату последнего месяца в этом году
					string_date = CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode).FirstNode.Text;
					page_title.Text = String.Format(
						page_title_caption, CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode).FirstNode.Text,
						UserComboBox.getLastBlock(current_region.Value));
				}
				else
				{
					if (CustomMultiComboDate.SelectedNode.Level == 1)
					{
						// если месяц, то находим последнюю актуальную дату в выбранном месяце
						string_date = CustomMultiComboDate.SelectedNode.FirstNode.Text;
						page_title.Text = String.Format(
							page_title_caption, CustomMultiComboDate.SelectedNode.FirstNode.Text,
							UserComboBox.getLastBlock(current_region.Value));
					}
					else
					{
						// если день, то мы его уже нашли :-)
						string_date = CustomMultiComboDate.SelectedNode.Parent.FirstNode.Text;
						page_title.Text = String.Format(
							page_title_caption, CustomMultiComboDate.SelectedNode.Parent.FirstNode.Text,
							UserComboBox.getLastBlock(current_region.Value));
					}
				}
			}
			page_sub_title.Text = page_sub_title_caption;
			// заполнение UltraWebGrid данными
			web_grid.DataBind();
			SetDynamicText(string_date);
			// В первом и втором столбце объединяем по три ячейки (на самом деле просто делаем
			// ячейки из этих столбцов в три строки размером)
			for (int i = 0; i < web_grid.Rows.Count; i += 3)
			{
				web_grid.Rows[i].Cells[0].RowSpan = 3;
				web_grid.Rows[i].Cells[1].RowSpan = 3;
			}
			// Маркировка звездами
			MarkByStars();

			// установка активной строки в UltraWebGrid
			if (!warp_web_grid.IsAsyncPostBack)
			{
				selected_grid_row = 0;
			}
			webGridActiveRowChange(selected_grid_row);
			//web_grid.Rows[selected_grid_row].Activated = true;
			//web_grid.Rows[selected_grid_row].Selected = true;
		}

		// --------------------------------------------------------------------

		protected void web_grid_DataBinding(object sender, EventArgs e)
		{
			DataTable grid_table = new DataTable();
			CellSet grid_set = null;
			// Загрузка таблицы цен и товаров в CellSet
			// Количество выбираемых столбцов на 3 больше нужного количества для
			// того, чтобы и в первом видимом столбце можно было поставить маркировку стрелками -
			// прирост или падение цены
			#region Обработка параметра даты
			string tmp1, tmp2;
			if (CustomMultiComboDate.SelectedNode.Level == 0)
			{
				// если выбран год, то находим последнюю актуальную дату в этом году
				tmp1 = CustomMultiComboDate.GetLastChild(CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode)).Text;
			}
			else
			{
				if (CustomMultiComboDate.SelectedNode.Level == 1)
				{
					// если месяц, то находим последнюю актуальную дату в выбранном месяце
					tmp1 = CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode).Text;
				}
				else
				{
					// если день, то мы его уже нашли :-)
					tmp1 = CustomMultiComboDate.SelectedValue;
				}
			}
			// Получаем дату в формате mdx
			dates_dict.TryGetValue(tmp1, out tmp2);
			// устанавливаем параметр для запроса
			on_date.Value = tmp2;
			#endregion
			count_grid.Value = "15";
			if (!cbOnMonthBegin.Checked)
			{
				// этот запрос выполняется, если не установлена галочка "На начало месяца"
				grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("new_grid"));
			}
			else
			{
				#region Формирование и выполнение запроса для грида по первым датам месяцев
				string s_month;
				int i_month;
				// Получаем месяц, в котором находится выбранное число
				months_dict.TryGetValue(on_date.Value, out s_month);
				// Теперь находим выбранный месяц в паре массивов ("месяц" - "первая актуальная дата")
				for (i_month = 0; (i_month < months.Length) & (months[i_month] != s_month); ++i_month)
				{
				}
				int i;
				string s_dates;
				// непосредственное формирование текста запроса
				if ((i_month - Convert.ToInt32(count_grid.Value.ToString()) + 1) < 0)
				{
					// Этот вариант, если месяц слишком близок к началу сбора данных, чтобы вывести 12 штук
					s_dates = days[0];
					for (i = 1; i <= i_month; ++i)
					{
						s_dates += ", " + days[i];
					}
				}
				else
				{
					// А здесь данных хватает на полный набор
					s_dates = days[i_month - Convert.ToInt32(count_grid.Value.ToString()) + 1];
					for (i = 1; i < (Convert.ToInt32(count_grid.Value.ToString())); ++i)
					{
						s_dates += ", " + days[i_month - Convert.ToInt32(count_grid.Value.ToString()) + 1 + i];
					}
				}
				dates.Value = s_dates;
				grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("new_grid_month"));
				#endregion

				#region Формирование запроса для диаграммы

				if ((i_month - Convert.ToInt32(count_chart.Value.ToString()) + 4) < 0)
				{
					s_dates = days[0];
					for (i = 1; i <= i_month; ++i)
					{
						s_dates += ", " + days[i];
					}
				}
				else
				{
					s_dates = days[i_month - Convert.ToInt32(count_chart.Value.ToString()) + 4];
					for (i = 1; i < (Convert.ToInt32(count_chart.Value.ToString()) - 3); ++i)
					{
						s_dates += ", " + days[i_month - Convert.ToInt32(count_chart.Value.ToString()) + 4 + i];
					}
				}
				dates_chart.Value = s_dates;
				#endregion

			}

			// Добавление столбцов в таблицу и заполнение данных в DataTable
			#region Заголовки столбцов грида
			grid_table.Columns.Add("Наименование продукта");
			grid_table.Columns.Add("Ед. изм.");
			int start;
			if (grid_set.Axes[0].Positions.Count <= Convert.ToInt32(count_grid.Value.ToString()))
			{
				start = 0;
			}
			else
			{
				start = grid_set.Axes[0].Positions.Count - Convert.ToInt32(count_grid.Value.ToString());
			}
			for (int i = start; i < grid_set.Axes[0].Positions.Count; ++i)
			{
				grid_table.Columns.Add(mdxTime2String(grid_set.Axes[0].Positions[i].Members[1].UniqueName, false));
			}
			grid_table.Columns.Add("unique_name");
			#endregion

			#region Заполнение грида данными
			// для каждого товара заполняется три строки - цена, изменение цены и изменение цены в процентах
			foreach (Position pos1 in grid_set.Axes[1].Positions)
			{
				object[] values = new object[grid_table.Columns.Count];
				object[] values1 = new object[grid_table.Columns.Count];
				object[] values2 = new object[grid_table.Columns.Count];
				// наименование товара
				values[0] = grid_set.Axes[1].Positions[pos1.Ordinal].Members[0].Caption;
				values1[0] = grid_set.Axes[1].Positions[pos1.Ordinal].Members[0].Caption;
				values2[0] = grid_set.Axes[1].Positions[pos1.Ordinal].Members[0].Caption;
				// единицы измерения
				values[1] = pos1.Members[0].MemberProperties[0].Value.ToString();
				values1[1] = pos1.Members[0].MemberProperties[0].Value.ToString();
				values2[1] = pos1.Members[0].MemberProperties[0].Value.ToString();
				// это оригинальное наименование - нужно для синхронизации с диаграммой
				values[grid_table.Columns.Count - 1] = pos1.Members[0].ToString();
				values1[grid_table.Columns.Count - 1] = pos1.Members[0].ToString();
				values2[grid_table.Columns.Count - 1] = pos1.Members[0].ToString();
				int i = 2, startIndex4CheckOnEmptyValues = 0;
				bool IsEmptyRow = true;
				startIndex4CheckOnEmptyValues = grid_set.Axes[0].Positions.Count < 12 ? 0 : grid_set.Axes[0].Positions.Count - 12;
				double prev_value = 0, value = 0, tmp_value;
				if (grid_set.Axes[0].Positions.Count <= Convert.ToInt32(count_grid.Value.ToString()))
				{
					start = 0;
				}
				else
				{
					start = grid_set.Axes[0].Positions.Count - Convert.ToInt32(count_grid.Value.ToString());
				}
				for (int j = start; j < grid_set.Axes[0].Positions.Count; ++j)
				{   // создание списка значений для строки UltraWebGrid
					// проверяем, не пустое ли значение
					if (grid_set.Cells[j, pos1.Ordinal].Value == null)
					{
						values[i] = "- ";
						values1[i] = "- ";
						values2[i] = "- ";
					}
					else
					{
						values[i] = Convert.ToDouble(grid_set.Cells[j, pos1.Ordinal].Value.ToString()).ToString("0.00 ");
						value = Convert.ToDouble(grid_set.Cells[j, pos1.Ordinal].Value.ToString());
					}
					// нулевую цену (запрос и такие возвращает) тоже считаем пустой
					if (value == 0)
					{
						values[i] = "- ";
						values1[i] = "- ";
						values2[i] = "- ";
					}
					// если уже есть предыдущее значение цены и текущее не ноль, то можем посчитать изменения
					if ((prev_value != 0) & (value != 0))
					{
						tmp_value = value - prev_value;
						values1[i] = tmp_value.ToString("0.00 ");
						tmp_value = (((value / prev_value) - 1) * 100);
						values2[i] = tmp_value.ToString("0.00") + "% ";
					}
					else
					{
						values1[i] = "- ";
						values2[i] = "- ";
					}
					// напоследок не нулевое текущее значение назначаем предыдущим
					if (value != 0)
					{
						prev_value = value;
						value = 0;
					}
					if (i >= (startIndex4CheckOnEmptyValues + 2))
					{
						IsEmptyRow &= (values[i].ToString().Trim() == "-");
						/*						if (pos1.Ordinal == 0)
												{
													CRHelper.SaveToUserAgentLog(values[i].ToString().Trim());
												}*/
					}
					++i;
				}
				// заполнение строки данными
				if (!IsEmptyRow)
				{
					grid_table.Rows.Add(values);
					grid_table.Rows.Add(values1);
					grid_table.Rows.Add(values2);
				}
			}
			#endregion
			// установка источника данных для UltraWebGrid
			web_grid.DataSource = grid_table.DefaultView;
		}

		// --------------------------------------------------------------------
		#region Инициализация грида (размеры и т.д. и т.п.)
		protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			// Опеределение браузера
			double coef = 1;
			string BN;
			System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
			BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
			if (BN == "FIREFOX")
			{
				coef = 1.04;
			}
			if (BN == "IE")
			{
				coef = 1.0;
			}

			// настройка размеров
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
			e.Layout.SelectTypeRowDefault = SelectType.Single;
			double tempWidth = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth).Value / 17;
			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns[0].Width = new Unit(tempWidth * 2.7 * coef);
			e.Layout.Bands[0].Columns[1].Width = new Unit(tempWidth * 0.85 * coef);
			for (int i = 2; i < (web_grid.Bands[0].Columns.Count - 1); ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = new Unit(tempWidth * coef - 1);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
			}

			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
			// скрываем ненужные столбцы
			e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
			for (int i = 0; i < (e.Layout.Bands[0].Columns.Count - Convert.ToInt32(count_grid.Value.ToString())); ++i)
			{
				e.Layout.Bands[0].Columns[2 + i].Hidden = true;
			}
		}
		#endregion

		// --------------------------------------------------------------------

		protected void chart_DataBinding(object sender, EventArgs e)
		{
			DataTable chart_table = new DataTable();
			CellSet chart_set = null;
			// Построение диаграммы. То что ниже закомментировано отвечало за построение ее на разные даты и по месяцам
			// может потом пригодиться
			count_chart.Value = "52";
			if (!cbOnMonthBegin.Checked)
			{
				chart_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("new_chart"));
			}
			else
			{
				chart_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("new_chart_month"));
			}
			// заполнение таблицы DataTable данными из CellSet
			cellSet2DataTable(chart_set, chart_table, true);
			// корректировка заголовков столбцов в DataTable
			foreach (DataColumn col in chart_table.Columns)
			{
				if (col.Ordinal > 0)
				{
					col.ColumnName = mdxTime2String(col.ColumnName, true);
				}
			}
			// установка источника данных для UltraChart
			chart.DataSource = (chart_table == null) ? null : chart_table.DefaultView;
			// определение максимального и минимального значений по оси ординат для UltraChart
			ArrayList list = new ArrayList(chart_table.Rows[0].ItemArray);
			list.RemoveAt(0);
			list.Sort();
			double min = Convert.ToDouble(list[0].ToString());
			double max = Convert.ToDouble(list[list.Count - 1].ToString());
			// настройка параметров оси ординат
			chart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
			chart.Axis.Y.RangeMin = min - (max - min) / 5;
			chart.Axis.Y.RangeMax = max;
		}

		// --------------------------------------------------------------------

		protected void web_grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
		{
			webGridActiveRowChange(e.Row.Index);
		}

		// --------------------------------------------------------------------

		protected void chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			setChartErrorFont(e);
		}

		// --------------------------------------------------------------------
		// --------------------------------------------------------------------

		/** <summary>
		 *  Устанавливает текущую строку в таблице 
		 *  </summary>
		 */
		private void webGridActiveRowChange(int index)
		{
			// установка заголовка для UltraChart
			chart_title.Text = String.Format(chart_title_caption, web_grid.Rows[index].Cells[0].Text);
			// установка параметра выбранного продукта
			UserParams.CustomParam("food_stuff").Value =
				String.Format("{0}", web_grid.Rows[index].Cells[web_grid.Columns.Count - 1].Text);
			// заполнение UltraChart данными
			chart.DataBind();
			selected_grid_row = index;
			/*for (int i = 0; i < web_grid.Rows.Count; ++i)
			{
				if (web_grid.Rows[i].Selected)
				{
					web_grid.Rows[i].Selected = false;
				}
			}*/
			web_grid.Rows[index].Selected = true;
			web_grid.DisplayLayout.ActiveRow = web_grid.Rows[index];
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
			if (chart_set.Cells.Count == 0)
			{
				throw new Exception("cell set is empty");
			}
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
			Int32 row_index = 0;
			Object[] values = null;
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

		// --------------------------------------------------------------------

		/** <summary>
		 *  Создание таблицы имен для продуктов питания 
		 *  </summary>
		 */
		private static Hashtable foodstuffNames()
		{
			Hashtable table = new Hashtable();
			String[] data = 
            {
                "В отчете", "В МДХ запросе",
                "Говядина (на кости)",  "Говядина на кости",
                "Куры (тушка 1 кат. отечественного производства)",  "Кура-тушка 1 категории",
                "Колбаса варёная (русская, любительская, докторская, молочная)", "Колбаса вареная типа в/c (Русская, Любительская)",
                "Масло сливочное (отечественного производства, без наполнителей)", "Масло сливочное коровье (жир не менее 72%)",
                "Масло подсолнечное (отечественного производства)", "Масло подсолнечное, рафинированное отечественное",
                "Молоко (3,2%, 2,5% жирности)", "Молоко цельное разл. (3,2 % жирности.)",
                "Творог (без наполнителей)", "Творог 5% жирности",
                "Кефир (без наполнителей)", "Кефир",
                "Сыр твёрдых сортов (отечественного производства)", "Сыр \"Российский\", твердый",
                "Яйца", "Яйцо куриное столовое диетическое",
                "Хлеб ржано-пшеничный", "Хлеб ржано-пшеничный",
                "Картофель", "Картофель свежий продовольственный 1кл",
                "Мука", "Мука пшеничная в/с",
                "Сахар", "Сахарный песок весовой",
                "Соль", "Соль поваренная, пищевая",
            };
			for (int i = 0; i < data.Length; i += 2)
			{
				table.Add(data[i + 1], data[i]);
			}
			return table;
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

		private static Hashtable monthNamesByNumbers()
		{
			Hashtable table = new Hashtable();
			String[] data = 
            {
                "01", "Январь",
                "02", "Февраль",
                "03", "Март",
                "04", "Апрель",
                "05", "Май",
                "06", "Июнь",
                "07", "Июль",
                "08", "Август",
                "09", "Сентябрь",
                "10", "Октябрь",
                "11", "Ноябрь",
                "12", "Декабрь",
            };
			for (int i = 0; i < data.Length; i += 2)
			{
				table.Add(data[i], data[i + 1]);
			}
			return table;
		}

		private static Hashtable monthNumbers()
		{
			Hashtable table = new Hashtable();
			String[] data = 
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

		// --------------------------------------------------------------------

		/** <summary>
		 *  Преобразование члена измерения времени в строку формата ЧЧ.ММ.ГГГГ
		 * 
		 *  до преобразования - [Период].[День].[Данные всех периодов].[2008].[Полугодие 2].[Квартал 4].[Декабрь].[1]
		 *  после преобразования - 01.12.2008
		 *  </summary>
		 */
		public static String mdxTime2String(String str, Boolean IsShortDate)
		{
			if (str == null) return null;
			String[] list = str.Split('.');
			for (int i = 0; i < list.Length; ++i)
			{
				list[i] = list[i].Replace("[", "");
				list[i] = list[i].Replace("]", "");
			}
			String result = (list.Length > 7) ?
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

		#region Рисование разноцветных стрелочек в зависмости от прироста
		protected void web_grid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
		{
			// Делаем только для строчек со значениями прироста
			if (decimal.Remainder(e.Row.Index, 3) == 0)
			{
				double prev_value = 0, value = 0;
				// Пробегаем по всем значениям в строке
				for (int i = 2; i < e.Row.Cells.Count - 1; ++i)
				{
					// проверки, чтобы исключения не лезли
					if (e.Row.Cells[i].Value != null)
					{
						if (e.Row.Cells[i].Value.ToString() != "- ")
						{
							// если есть значение
							if (Convert.ToDouble(e.Row.Cells[i].Value.ToString().Trim()) != 0)
							{
								value = Convert.ToDouble(e.Row.Cells[i].Value.ToString().Trim());
								e.Row.Cells[i].Style.Font.Bold = true;
								if (prev_value != 0)
								{
									if (prev_value < value)
									{
										// если предыдущее значение меньше, то стрелка красная
										e.Row.Cells[i].Style.CssClass = "ArrowUpRed";
									}
									else
									{
										if (prev_value > value)
										{
											// если больше, то зеленая
											e.Row.Cells[i].Style.CssClass = "ArrowDownGreen";
										}
									}
								}
								prev_value = value;
							}
						}
					}
				}
			}
			else
			{
				if (decimal.Remainder(e.Row.Index, 3) == 1)
				{
					// Пробегаем по всем значениям в строке
					for (int i = 2; i < e.Row.Cells.Count - 1; ++i)
					{
						if (e.Row.Cells[i].Value.ToString() != "- ")
						{
							if (Convert.ToDouble(e.Row.Cells[i].Value.ToString().Trim()) > 0)
							{
								e.Row.Cells[i].Title = "Прирост к предыдущей дате";
							}
							else
							{
								if (Convert.ToDouble(e.Row.Cells[i].Value.ToString().Trim()) < 0)
								{
									e.Row.Cells[i].Title = "Падение относительно предыдущей даты";
								}
							}
						}
					}
				}
				if (decimal.Remainder(e.Row.Index, 3) == 2)
				{
					// Пробегаем по всем значениям в строке
					for (int i = 2; i < e.Row.Cells.Count - 1; ++i)
					{
						if (e.Row.Cells[i].Value.ToString() != "- ")
						{
							e.Row.Cells[i].Title = "Темп прироста к предыдущей дате";
						}
					}
				}
			}
		}
		#endregion

		#region Маркировка ячеек звездочками
		private void MarkByStars()
		{
			// Маркировка звездами
			// пробегаем по гриду и ищем минимальное (отрицательное) и максимальное значения прироста цены
			// если находим - рисуем соответствующую звездочку
			for (int i = 3; i < web_grid.Columns.Count - 1; ++i)
			{
				int max_row = -1, min_row = -1;
				double max_value = 0, min_value = 0;
				for (int j = 2; j < web_grid.Rows.Count; j += 3)
				{
					if (web_grid.Rows[j].Cells[i].Value.ToString() != "- ")
					{
						string s_value = web_grid.Rows[j].Cells[i].Value.ToString();
						double value = Convert.ToDouble(s_value.Substring(0, s_value.Length - 2).Trim());
						if (value < min_value)
						{
							min_value = value;
							min_row = j;
						}
						if (value > max_value)
						{
							max_value = value;
							max_row = j;
						}
					}
				}
				if (max_row != -1)
				{
					web_grid.Rows[max_row].Cells[i].Style.BackgroundImage = "~/images/starGraybb.png";
					web_grid.Rows[max_row].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
					web_grid.Rows[max_row].Cells[i].Title = "Наибольший темп прироста";
				}
				if (min_row != -1)
				{
					web_grid.Rows[min_row].Cells[i].Style.BackgroundImage = "~/images/starYellowbb.png";
					web_grid.Rows[min_row].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
					web_grid.Rows[min_row].Cells[i].Title = "Наибольший темп снижения";
				}
			}
		}
		#endregion

		#region Формирование динамического текста
		private void SetDynamicText(string string_date)
		{
			string head = String.Empty, percentsMore2 = String.Empty, costDown = String.Empty, maxCostUp = String.Empty, maxCostDown = String.Empty;
			double max_value = 0, min_value = 0;
			for (int j = 2; j < web_grid.Rows.Count; j += 3)
			{
				string string_value = web_grid.Rows[j].Cells[web_grid.Columns.Count - 2].Value.ToString().Trim();
				double num_value;
				if (Double.TryParse(string_value.Replace("%", String.Empty), out num_value))
				{
					if (num_value != 0)
					{
						if (num_value > 0)
						{
							if (num_value > max_value)
							{
								max_value = num_value;
								maxCostUp = web_grid.Rows[j].Cells[0].Value.ToString().Trim();
							}
							if (num_value > 2)
							{
								percentsMore2 = percentsMore2 == String.Empty ? web_grid.Rows[j].Cells[0].Value.ToString().Trim() :
									percentsMore2 + ", " + web_grid.Rows[j].Cells[0].Value.ToString().Trim();
							}
						}
						else
						{
							if (num_value < min_value)
							{
								min_value = num_value;
								maxCostDown = web_grid.Rows[j].Cells[0].Value.ToString().Trim();
							}
							costDown = costDown == String.Empty ? web_grid.Rows[j].Cells[0].Value.ToString().Trim() :
									costDown + ", " + web_grid.Rows[j].Cells[0].Value.ToString().Trim();
						}
					}
				}
			}
			if ((percentsMore2 == String.Empty) & (costDown == String.Empty))
			{
				head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> розничные цены на продовольственные товары изменились незначительно.",
					string_date);
			}
			else
			{
				head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> наблюдалось изменение розничных цен на продовольственные товары:",
					string_date);
			}
			lbDynamicText.Text = head;
			if (percentsMore2 != String.Empty)
			{
				lbDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- увеличение цен более чем на 2% на товары: <b>" + percentsMore2 + "</b>";
			}
			if (costDown != String.Empty)
			{
				lbDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- снижение цен на товары: <b>" + costDown + "</b>";
			}
			if ((maxCostUp != String.Empty) | (maxCostDown != String.Empty))
			{
				if ((maxCostUp != String.Empty) & (maxCostDown != String.Empty))
				{
					lbDynamicText.Text += String.Format(
						"<br/>&nbsp;&nbsp;&nbsp;Наибольший темп прироста цены наблюдался на товар <b>«{0}»</b> - на <b>{1}%</b>; наибольший темп снижения зарегистрирован на товар <b>«{2}»</b> - на <b>{3}%</b>.",
						maxCostUp, max_value, maxCostDown, min_value);
				}
				else
				{
					if (maxCostUp != String.Empty)
					{
						lbDynamicText.Text += String.Format(
							"<br/>&nbsp;&nbsp;&nbsp;Наибольший темп прироста цены наблюдался на товар <b>«{0}» - на <b>{1}%.", maxCostUp, max_value);
					}
					else
					{
						lbDynamicText.Text += String.Format(
							"<br/>&nbsp;&nbsp;&nbsp;Наибольший темп снижения зарегистрирован на товар <b>«{0}» - <b>на {1}%.", maxCostDown, min_value);
					}
				}
			}
			lbDynamicText.Width = Unit.Empty;
			lbDynamicText.Height = Unit.Empty;
		}
		#endregion

		protected void web_grid_Click(object sender, ClickEventArgs e)
		{
//			selected_grid_row = e.Row.Index;
		}

		// --------------------------------------------------------------------
	}
}
