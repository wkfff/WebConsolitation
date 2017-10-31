using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Microsoft.AnalysisServices.AdomdClient;

/**
 *  Анализ цен на продукты питания.
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0004
{
	public partial class Default : CustomReportPage
	{
		// --------------------------------------------------------------------

		// заголовок страницы
		private static String page_title_caption = "Анализ розничных цен на основные продуты питания ({0})";
		// подзаголовок страницы
		private static String page_subtitle_caption = "Анализ динамики розничных цен на основные продукты питания в муниципальном образовании";
		// заголовок для Grid1
		private static String grid1_title_caption = "Динамика цен (темп роста) на продовольственные товары";
		private static String[] quarters = { "1", "1", "1", "2", "2", "2", "3", "3", "3", "4", "4", "4", "1", "1", "1", "2", "2", "2", "3", "3", "3", "4", "4", "4" };
		private static String[] halfs = { "1", "1", "1", "1", "1", "1", "2", "2", "2", "2", "2", "2", "1", "1", "1", "1", "1", "1", "2", "2", "2", "2", "2", "2" };
		private static String[] monthes = 
            {
                "Январь",   // 0
                "Февраль",
                "Март", 
                "Апрель",
                "Май", 
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь",
                "Ноябрь",
                "Декабрь",
                "Январь",   // 12
                "Февраль",
                "Март", 
                "Апрель",
                "Май",      // 16
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь",
                "Ноябрь",
                "Декабрь"
            };

		// параметр для выбранного ПЕРИОДА
		private CustomParam querisDate { get { return (UserParams.CustomParam("querisDate")); } }
		// параметр для последней актуальной даты
		private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
		// ширина экрана в пикселях
		private Int32 screen_width { get { return (int)Session["width_size"]; } }
		// параметр запроса для региона
		private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			// установка размера диаграммы
			web_grid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.75);
			web_grid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 1);

			CustomMultiComboDate.Width = 300;
			CustomMultiComboDate.Title = "Выберите период";
			CustomMultiComboMonthsCount.Width = 300;
			CustomMultiComboMonthsCount.Title = "Количество отображаемых месяцев";
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			DataTable dtDate;
			string query = String.Empty;
			if (!Page.IsPostBack)
			{   // опрерации которые должны выполняться при только первой загрузке страницы
				RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
				baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

				dtDate = new DataTable();
				query = DataProvider.GetQueryText("list_of_dates");
				string prev_year = String.Empty;
				DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
				Dictionary<string, int> dict_date = new Dictionary<string, int>();
				for (int i = 0; i < dtDate.Rows.Count; ++i)
				{
					string yearD = UserComboBox.getLastBlock(dtDate.Rows[i][0].ToString());
					string monthD = UserComboBox.getLastBlock(dtDate.Rows[i][3].ToString());
					if (yearD != prev_year)
					{
						dict_date.Add(yearD, 0);
						prev_year = yearD;
					}
					dict_date.Add(monthD + " " + yearD + " года", 1);
				}
				CustomMultiComboDate.FillDictionaryValues(dict_date);
				CustomMultiComboDate.SelectLastNode();
				CustomMultiComboMonthsCount.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(1, 12));
				CustomMultiComboMonthsCount.SetСheckedState("6", true);
			}
			page_title.Text = String.Format(page_title_caption, UserComboBox.getLastBlock(baseRegion.Value));
			Page.Title = page_title.Text;
			page_subtitle.Text = String.Format(page_subtitle_caption);
			string[] date_items = CustomMultiComboDate.SelectedValue.Split(' ');
			querisDate.Value = getRequest(date_items[1], date_items[0], CustomMultiComboMonthsCount.SelectedValue);
			web_grid1.DataBind();
			grid1_caption.Text = String.Format(grid1_title_caption, date_items[0].ToLower());

		}


		/** <summary>
		 *  Возвращает последний блок для MDX Member
		 *  например: getLastBlock("[Территории].[РФ].[Все территории]") возвращает 'Все территории'
		 *  </summary>
		 */
		public static String getYearFromMonth(String mdx_member)
		{
			if (mdx_member == null)
			{
				return null;
			}
			String[] list = mdx_member.Split('.');
			Int32 index = list.Length - 1;
			String total = list[index - 3];
			total = total.Replace("[", "");
			total = total.Replace("]", "");
			return total;
		}


		private int getMonthIndex(String month)
		{
			int monthIndex = 0;
			for (int i = 12; i != monthes.Length; i++)
			{
				if (month == monthes[i])
				{
					monthIndex = i;
					break;
				}
			}
			return monthIndex;
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

		private String getRequest(String year, String month, String count)
		{
			String[] years = new String[24];
			String request = null;
			int monthIndex = getMonthIndex(month);
			String template = "[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].FirstChild";
			for (int i = 0; i != 24; i++)
			{
				if (i > 11)
				{
					years[i] = year;
				}
				else
				{
					years[i] = (int.Parse(year) - 1).ToString();
				}
			}
			int monsCount = int.Parse(count);
			request = String.Format(template, years[monthIndex - monsCount],
											  halfs[monthIndex - monsCount],
											  quarters[monthIndex - monsCount],
											  monthes[monthIndex - monsCount]);
			for (int i = monsCount - 1; i >= 0; i--)
			{
				request = request + "," + String.Format(template, years[monthIndex - i],
																  halfs[monthIndex - i],
																  quarters[monthIndex - i],
																  monthes[monthIndex - i]);
			}
			return request;
		}

		protected void web_grid_DataBinding(object sender, EventArgs e)
		{
			DataTable grid_master = new DataTable();
			//SecondaryMASDataProvider.GetDataTableForPivotTable(GetQueryText("grid1"), grid_master);
			CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));

			string[] date_split = CustomMultiComboDate.SelectedValue.Split(' ');
			int monthIndex = getMonthIndex(date_split[0]);
			grid_master.Columns.Add("Продукты");
			int monthsCount = int.Parse(CustomMultiComboMonthsCount.SelectedValue) < cs.Axes[0].Positions.Count ?
				int.Parse(CustomMultiComboMonthsCount.SelectedValue) : cs.Axes[0].Positions.Count;
			string[] years = new string[24];
			for (int i = 0; i != 24; i++)
			{
				if (i > 11)
				{
					years[i] = date_split[1];
				}
				else
				{
					years[i] = (int.Parse(date_split[1]) - 1).ToString();
				}
			}
			for (int i = monthsCount - 1; i >= 0; i--)
			{
				grid_master.Columns.Add(years[monthIndex - i] + "<br>" + monthes[monthIndex - i]);
			}
			object[] values = new object[monthsCount + 1];
			foreach (Position pos in cs.Axes[1].Positions)
			{   // создание списка значений для строки UltraWebGrid
				values[0] = cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption;
				if (int.Parse(CustomMultiComboMonthsCount.SelectedValue) >= cs.Axes[0].Positions.Count)
				{
					values[1] = "100,00";
					for (int i = monthsCount - 1; i > 0; --i)
					{
						values[i + 1] = string.Format("{0:N2}",
							(Convert.ToDouble(cs.Cells[i, pos.Ordinal].Value) / Convert.ToDouble(cs.Cells[i - 1, pos.Ordinal].Value) * 100));
					}
				}
				else
				{
					for (int i = monthsCount; i > 0; --i)
					{
						values[i] = string.Format("{0:N2}",
							(Convert.ToDouble(cs.Cells[i, pos.Ordinal].Value) / Convert.ToDouble(cs.Cells[i - 1, pos.Ordinal].Value) * 100));
					}
				}
				// заполнение строки данными
				if ((Convert.ToDouble(values[values.Length - 1]) > 0) && (!Double.IsInfinity(Convert.ToDouble(values[values.Length - 1]))))
				{
					grid_master.Rows.Add(values);
				}
			}
			web_grid1.DataSource = grid_master.DefaultView;
		}

		protected void web_grid1_DataBinding(object sender, EventArgs e)
		{
			web_grid_DataBinding(sender, e);
		}

		protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			// настройка столбцов
			double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
			e.Layout.RowSelectorsDefault = RowSelectors.No;
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
			//e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.25) - 5;
			e.Layout.Bands[0].Columns[0].Width = (int)((screen_width) * 0.25) - 5;

			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
			{
				//e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.75 / (e.Layout.Bands[0].Columns.Count - 1)) - 5;
				e.Layout.Bands[0].Columns[i].Width = 80;
				e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
				//e.Layout.Bands[0].Columns[i].Header.Style.
				// установка формата отображения данных в UltraWebGrid
				CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "##.##");
			}
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
		}

		protected void web_grid1_InitializeRow(object sender, RowEventArgs e)
		{
			for (int i = 1; i < e.Row.Cells.Count; i++)
			{
				double cellValue = Convert.ToDouble(e.Row.Cells[i].Value);
				if ((cellValue > 0) & (!Double.IsInfinity(cellValue)))
				{
					if (cellValue != 100)
					{
						if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
						{
							e.Row.Cells[i].Style.CssClass = "ArrowDownGreen";
						}
						else
						{
							e.Row.Cells[i].Style.CssClass = "ArrowUpRed";
						}
					}
					else
					{
						e.Row.Cells[i].Style.CssClass = "RightYellow";
					}
					e.Row.Cells[i].Value = e.Row.Cells[i].Value.ToString() + "%";
				}
				else
				{
					e.Row.Cells[i].Value = "";
				}
			}
		}
	}

}

