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
 *  Анализ цен на ГСМ.
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0003
{
	public partial class Default : CustomReportPage
	{
		// --------------------------------------------------------------------

		// заголовок страницы
		private static string page_title_caption = "Анализ розничных цен на ГСМ ({0})";
		// подзаголовок страницы
		private static string page_subtitle_caption = "Анализ динамики розничных цен на горюче-смазочные материалы (ГСМ) в муниципальном образовании";
		// заголовок для Grid1
		private static string grid1_title_caption = "Стоимость 1 литра бензина АИ-95(96), рубль";
		// заголовок для Grid2
		private static string grid2_title_caption = "Стоимость 1 литра бензина АИ-92(93), рубль";
		// заголовок для Grid3
		private static string grid3_title_caption = "Стоимость 1 литра бензина АИ-80, рубль";
		// заголовок для Grid4
		private static string grid4_title_caption = "Стоимость 1 литра бензина АИ-76, рубль";

		private static string[] quarters = { "1", "1", "1", "2", "2", "2", "3", "3", "3", "4", "4", "4" };
		private static string[] halfs = { "1", "1", "1", "1", "1", "1", "2", "2", "2", "2", "2", "2" };
		private static string[] monthes = 
            {
                "Октябрь",
                "Ноябрь",
                "Декабрь",
                "Январь", 
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
                "Декабрь"
            };

		// параметр для выбранного ПЕРИОДА
		private CustomParam querisDate { get { return (UserParams.CustomParam("querisDate")); } }
		// ширина экрана в пикселях
		private Int32 screen_width { get { return (int)Session["width_size"]; } }
		// параметр запроса для региона
		private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			// установка размера диаграммы
			web_grid1.Width = (int)((screen_width - 55));
			web_grid2.Width = (int)((screen_width - 55));
			web_grid3.Width = (int)((screen_width - 55));
			web_grid4.Width = (int)((screen_width - 55));

			CustomMultiComboDate.Width = 300;
			CustomMultiComboDate.Title = "Выберите период";
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			DataTable dtDate;
			string query = String.Empty;
			base.Page_Load(sender, e);
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
			}
			page_title.Text = String.Format(page_title_caption, UserComboBox.getLastBlock(baseRegion.Value));
			Page.Title = page_title.Text;
			page_subtitle.Text = String.Format(page_subtitle_caption);
			string[] date_items;
			if (CustomMultiComboDate.SelectedNode.Level == 1)
			{
				date_items = CustomMultiComboDate.SelectedValue.Split(' ');
			}
			else
			{
				date_items = CustomMultiComboDate.GetLastChild(CustomMultiComboDate.SelectedNode).Text.Split(' ');
			}
			querisDate.Value = getRequest(date_items[1], date_items[0]);
			web_grid1.DataBind();
			grid1_caption.Text = grid1_title_caption;
			web_grid2.DataBind();
			grid2_caption.Text = grid2_title_caption;
			web_grid3.DataBind();
			grid3_caption.Text = grid3_title_caption;
			web_grid4.DataBind();
			grid4_caption.Text = grid4_title_caption;
		}


		/** <summary>
		 *  Возвращает последний блок для MDX Member
		 *  например: getLastBlock("[Территории].[РФ].[Все территории]") возвращает 'Все территории'
		 *  </summary>
		 */
		public static string getYearFromMonth(string mdx_member)
		{
			if (mdx_member == null) return null;
			string[] list = mdx_member.Split('.');
			int index = list.Length - 1;
			string total = list[index - 3];
			total = total.Replace("[", "");
			total = total.Replace("]", "");
			return total;
		}


		private int getMonthIndex(string month)
		{
			int monthIndex = 0;
			for (int i = 3; i != monthes.Length; i++)
				if (month == monthes[i])
				{
					monthIndex = i;
					break;
				}
			return monthIndex;
		}

		// --------------------------------------------------------------------

		/** <summary>
		 *  Метод получения последней актуальной даты 
		 *  </summary>
		 */
		private string getLastDate()
		{
			CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
			return cs.Axes[1].Positions[0].Members[0].ToString();
		}

		// --------------------------------------------------------------------

		private string getRequest(string year, string month)
		{
			string request = null;
			int monthIndex = getMonthIndex(month);
			string template = "[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].FirstChild";
			if (monthIndex < 6)
			{
				string prevYear = (int.Parse(year) - 1).ToString();
				switch (monthIndex - 3)
				{
					case 0:
						{
							request = String.Format(template, prevYear, "2", "4", "Октябрь") + "," +
									  String.Format(template, prevYear, "2", "4", "Ноябрь") + "," +
									  String.Format(template, prevYear, "2", "4", "Декабрь") + "," +
									  String.Format(template, year, "1", "1", "Январь");
							break;
						}
					case 1:
						{
							request = String.Format(template, prevYear, "2", "4", "Ноябрь") + "," +
									  String.Format(template, prevYear, "2", "4", "Декабрь") + "," +
									  String.Format(template, year, "1", "1", "Январь") + "," +
									  String.Format(template, year, "1", "1", "Февраль");
							break;
						}
					case 2:
						{
							request = String.Format(template, prevYear, "2", "4", "Декабрь") + "," +
									  String.Format(template, year, "1", "1", "Январь") + "," +
									  String.Format(template, year, "1", "1", "Февраль") + "," +
									  String.Format(template, year, "1", "1", "Март");
							break;
						}
				}

			}
			else
			{
				request = String.Format(template, year, halfs[monthIndex - 3 - 3], quarters[monthIndex - 3 - 3], monthes[monthIndex - 3]) + "," +
						  String.Format(template, year, halfs[monthIndex - 3 - 2], quarters[monthIndex - 3 - 2], monthes[monthIndex - 2]) + "," +
						  String.Format(template, year, halfs[monthIndex - 3 - 1], quarters[monthIndex - 3 - 1], monthes[monthIndex - 1]) + "," +
						  String.Format(template, year, halfs[monthIndex - 3], quarters[monthIndex - 3], monthes[monthIndex]);
			}
			return request;
		}

		protected void web_grid_DataBinding(object sender, EventArgs e, string gridNum)
		{
			DataTable grid_master = new DataTable();
			CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid" + gridNum));
			CellSet csavg = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("gridavg" + gridNum));
			int monthIndex = 0;
			int year = Convert.ToInt32(CustomMultiComboDate.SelectedValue.Split(' ')[1]);
			for (int i = 3; i != monthes.Length; i++)
				if (CustomMultiComboDate.SelectedValue.Split(' ')[0] == monthes[i])
					monthIndex = i;
			grid_master.Columns.Add("Организации");
			for (int i = 0; i != 4; i++)
			{
				if ((monthIndex - 3 + i) > 2)
				{
					grid_master.Columns.Add(year.ToString() + " " + monthes[monthIndex - 3 + i]);
				}
				else
				{
					grid_master.Columns.Add((year - 1).ToString() + " " + monthes[monthIndex - 3 + i]);
				}
			}
			if (monthIndex > 3)
			{
				grid_master.Columns.Add(year.ToString() + " " + monthes[monthIndex] + "/" + year.ToString() + " " + monthes[monthIndex - 3]);
			}
			else
			{
				grid_master.Columns.Add(year.ToString() + " " + monthes[monthIndex] + "/" + (year - 1).ToString() + " " + monthes[monthIndex - 3]);
			}
			if (monthIndex > 0)
			{
				grid_master.Columns.Add(year.ToString() + " " + monthes[monthIndex] + "/" + year.ToString() + " " + monthes[monthIndex - 1]);
			}
			else
			{
				grid_master.Columns.Add(year.ToString() + " " + monthes[monthIndex] + "/" + (year - 1).ToString() + " " + monthes[monthIndex - 1]);
			}

			object[] values1 = new object[7];
			values1[0] = "Среднее значение";
			for (int i = 0; i < csavg.Cells.Count; ++i)
			{
				values1[4 - i] = String.Format("{0:N2}", csavg.Cells[csavg.Cells.Count - i - 1, 0].FormattedValue);
			}
			if (csavg.Cells.Count == 4)
			{
				values1[5] = String.Format("{0:N2}", (Convert.ToDouble(csavg.Cells[3, 0].Value) / Convert.ToDouble(csavg.Cells[0, 0].Value) * 100));
			}
			if (csavg.Cells.Count > 1)
			{
				values1[6] = String.Format("{0:N2}", (Convert.ToDouble(csavg.Cells[csavg.Cells.Count - 1, 0].Value) / Convert.ToDouble(csavg.Cells[csavg.Cells.Count - 2, 0].Value) * 100));
			}
			grid_master.Rows.Add(values1);

			foreach (Position pos in cs.Axes[1].Positions)
			{   // создание списка значений для строки UltraWebGrid
				object[] values = new object[7];
				values[0] = cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption;
				for (int i = 0; i < cs.Axes[0].Positions.Count; ++i)
				{
					values[4 - i] =
						String.Format("{0:N2}", cs.Cells[cs.Axes[0].Positions.Count - i - 1, pos.Ordinal].Value);
				}
				if (cs.Axes[0].Positions.Count == 4)
				{
					values[5] = String.Format("{0:N2}",
						(Convert.ToDouble(cs.Cells[3, pos.Ordinal].Value) / Convert.ToDouble(cs.Cells[0, pos.Ordinal].Value) * 100));
				}
				if (cs.Axes[0].Positions.Count > 1)
				{
					values[6] = String.Format("{0:N2}",
						(Convert.ToDouble(cs.Cells[cs.Axes[0].Positions.Count - 1, pos.Ordinal].Value) / Convert.ToDouble(cs.Cells[cs.Axes[0].Positions.Count - 2, pos.Ordinal].Value) * 100));
				}
				// заполнение строки данными
				grid_master.Rows.Add(values);
			}
			switch (gridNum)
			{
				case "1":
					{
						web_grid1.DataSource = grid_master.DefaultView;
						break;
					}
				case "2":
					{
						web_grid2.DataSource = grid_master.DefaultView;
						break;
					}
				case "3":
					{
						web_grid3.DataSource = grid_master.DefaultView;
						break;
					}
				case "4":
					{
						web_grid4.DataSource = grid_master.DefaultView;
						break;
					}
			}
		}

		protected void web_grid1_DataBinding(object sender, EventArgs e)
		{
			web_grid_DataBinding(sender, e, "1");
		}

		protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			// настройка столбцов
			double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
			//e.Layout.RowSelectorStyleDefault.Width = 0;
			e.Layout.RowSelectorsDefault = RowSelectors.No;
			e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.3) - 5;
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;

			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
			{
				e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.7 / (e.Layout.Bands[0].Columns.Count - 1)) - 5;
				e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
				e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
				// установка формата отображения данных в UltraWebGrid
				CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "##.00");
			}
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			e.Layout.Bands[0].Columns[4].CellStyle.Font.Bold = true;
		}

		protected void web_grid1_InitializeRow(object sender, RowEventArgs e)
		{

			double cellValue = Convert.ToDouble(e.Row.Cells[5].Value);
			if ((cellValue > 0) & (cellValue < 1000))
			{
				if (cellValue != 100)
					if (Convert.ToDouble(e.Row.Cells[5].Value) < 100)
						e.Row.Cells[5].Style.CssClass = "ArrowDownGreen";
					else
						e.Row.Cells[5].Style.CssClass = "ArrowUpRed";
				e.Row.Cells[5].Value = e.Row.Cells[5].Value.ToString() + "%";
			}
			else
				e.Row.Cells[5].Value = "";

			cellValue = Convert.ToDouble(e.Row.Cells[6].Value);
			if ((cellValue > 0) & (cellValue < 1000))
			{
				if (cellValue != 100)
					if (Convert.ToDouble(e.Row.Cells[6].Value) < 100)
						e.Row.Cells[6].Style.CssClass = "ArrowDownGreen";
					else
						e.Row.Cells[6].Style.CssClass = "ArrowUpRed";
				e.Row.Cells[6].Value = e.Row.Cells[6].Value.ToString() + "%";
			}
			else
				e.Row.Cells[6].Value = "";
		}

		protected void web_grid2_DataBinding(object sender, EventArgs e)
		{
			web_grid_DataBinding(sender, e, "2");
		}

		protected void web_grid3_DataBinding(object sender, EventArgs e)
		{
			web_grid_DataBinding(sender, e, "3");
		}

		protected void web_grid4_DataBinding(object sender, EventArgs e)
		{
			web_grid_DataBinding(sender, e, "4");
		}
	}
}
