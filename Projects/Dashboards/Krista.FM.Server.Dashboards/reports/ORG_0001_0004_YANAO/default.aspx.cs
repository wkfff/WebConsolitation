using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Preferences.Printing;
using Infragistics.Documents.Reports.Report.Text;

/**
 *  Анализ максимальных и минимальных розничных цен на продукты питания
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0001_0004_YANAO
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private static DataTable dtDate;
		private static DataTable dtGrid;
		private static DataTable dtFoodTypes;
		private static GridHeaderLayout headerLayout;

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

		private CustomParam selectedDate;
		private CustomParam compareDate;
		private CustomParam currentDate;
		private CustomParam currentFood;
		private CustomParam currentPrice;

		#endregion

        // Массивы с продуктами, mdx именами продуктов
        string[] food = { "Говядина (кроме бескостного мяса)", 
                            "Свинина (кроме бескостного мяса)", 
                            "Куры (кроме куринных окорочков)", 
                            "Рыба замороженная неразделенная ",
                            "Масло сливочное", 
                            "Масло подсолнечное",
						  "Молоко питьевое (жирностью 2,5, 3,2 %)",
                          "Творог",
                          "Кефир",
                          "Сыр твердых сортов",
                          "Яйца куриные", 
                          "Сахар-песок",
                          "Соль поваренная пищевая",
                          "Чай черный байховый",
                          "Мука пшеничная ", 
                          "Хлеб ржано-пшеничный",
                          "Хлеб пшеничный из муки 1 сорта",
                          "Хлеб пшеничный из муки 2 сорта",
                            "Рис шлифованный",
                          "Пшено",
                          "Крупа гречневая - ядрица",
                          "Вермишель",
                          "Картофель",
						  "Капуста белокочанная свежая",
                          "Лук репчатый",
                          "Морковь", 
                          "Яблоки"};
        string[] foodMDX =
		{
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Говядина (кроме бескостного мяса)]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Свинина (кроме бескостного мяса)]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Куры (кроме куринных окорочков)]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Рыба].[Рыба мороженная неразделенная]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло сливочное]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло подсолнечное]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Молоко питьевое (жирностью 2,5, 3,2%)]",            

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Творог]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Кефир]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Сыр твердых сортов]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Яйца куриные]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Сахар-песок]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Соль, пряности].[Соль поваренная пищевая]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Чай, кофе].[Чай черный байховый]",			

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Мука пшеничная]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб ржано-пшеничный]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб пшеничный из муки 1 сорта]",

            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб пшеничный из муки 2 сорта]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Рис шлифованный]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Пшено]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Крупа гречневая - ядрица]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Крупы].[Вермишель]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Картофель]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Капуста белокочанная свежая]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Лук репчатый]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Овощи].[Морковь]",

			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Фрукты].[Яблоки]"
		};
        

        private const string PageTitleCaption = "Анализ максимальных и минимальных розничных цен на отдельные виды социально значимых продовольственных товаров первой необходимости";
        private const string PageSubTitleCaption = "Анализ максимальных и минимальных розничных цен на продукты питания на основании данных еженедельного мониторинга предприятий торговли, Ямало-Ненецкий автономный округ, по состоянию на {0}";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 275;
			ComboDate.ParentSelect = true;
			ComboCompareDate.Title = "Выберите дату для сравнения";
			ComboCompareDate.Width = 375;
			ComboCompareDate.ParentSelect = true;

			#region Настройка грида

			if (!IsSmallResolution)
			{
				UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
				UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6);
			}
			else
			{
				UltraWebGrid.Width = CRHelper.GetGridWidth(775);
				UltraWebGrid.Height = CRHelper.GetGridHeight(400);
			}
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

			#endregion

			#region Параметры

			selectedDate = UserParams.CustomParam("selected_date");
			compareDate = UserParams.CustomParam("compare_date");
			currentDate = UserParams.CustomParam("date");
			currentFood = UserParams.CustomParam("food");
			currentPrice = UserParams.CustomParam("price");

			#endregion

			#region Ссылки

			HyperLink.Text = "Анализ&nbsp;средних&nbsp;розничных цен&nbsp;на&nbsp;продукты&nbsp;питания";
			HyperLink.NavigateUrl = "~/reports/ORG_0001_0005_YANAO/Default.aspx";

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
				FillComboDate(ComboDate, "ORG_0001_0004_list_of_dates", 0);
				FillComboDate(ComboCompareDate, "ORG_0001_0004_list_of_dates", 1);
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

			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			//PageSubTitle.Text = String.Format(PageSubTitleCaption, MDXDateToShortDateString(selectedDate.Value));
            PageSubTitle.Text = String.Format(PageSubTitleCaption, ComboDate.SelectedValue);
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.DisplayLayout.Bands.Clear();
			UltraWebGrid.DataBind();
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			UltraWebGrid grid = sender as UltraWebGrid;
			string query;
			dtGrid = new DataTable();
			dtGrid.Columns.Add("Продукт", typeof(string));
			dtGrid.Columns.Add("Минимальная цена на дату для сравнения, рубль", typeof(string));
			dtGrid.Columns.Add("Максимальная цена на дату для сравнения, рубль", typeof(string));
			dtGrid.Columns.Add("Минимальная цена на выбранную дату, рубль", typeof(string));
			dtGrid.Columns.Add("Максимальная цена на выбранную дату, рубль", typeof(string));

			currentPrice.Value = "Розничная цена";
			for (int i = 0; i < food.Length; ++i)
			{
				DataRow row1 = dtGrid.NewRow();
				DataRow row2 = dtGrid.NewRow();
				currentFood.Value = foodMDX[i];
				row1[0] = food[i] + ", " + GetFoodUnit();
				currentDate.Value = compareDate.Value;
				DataTable dtFoodData = new DataTable();
				query = DataProvider.GetQueryText("ORG_0001_0004_data_part1");
				DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по продукту", dtFoodData);
				DataTable dtCities = new DataTable();
				query = DataProvider.GetQueryText("ORG_0001_0004_data_part2");
				DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Город", dtCities);
				object[] results = new object[5];
				//ParseData(dtFoodData, dtCities, results);
                dtCities = new DataTable();

                query = DataProvider.GetQueryText("GridPartMin");
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Город", dtCities);
                try
                {
                    row1[1] = dtCities.Rows[0][1];

                    row2[1] = dtCities.Rows[0][0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. "); ; //results[1];

                    query = DataProvider.GetQueryText("GridPartMax");
                    dtCities = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Город", dtCities);

                    row1[2] = dtCities.Rows[0][1];//results[4];
                    row2[2] = dtCities.Rows[0][0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. "); ;// results[3];
                }
                catch { }
				currentDate.Value = selectedDate.Value;
				dtFoodData = new DataTable();
				query = DataProvider.GetQueryText("ORG_0001_0004_data_part1");
				DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по продукту", dtFoodData);
				dtCities = new DataTable();
				query = DataProvider.GetQueryText("ORG_0001_0004_data_part2");
				DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Город", dtCities);
				results = new object[5];
                //ParseData(dtFoodData, dtCities, results);
                //row1[3] = results[2];
                //row2[3] = results[1];
                //row1[4] = results[4];
                //row2[4] = results[3];
                query = DataProvider.GetQueryText("GridPartMin");
                dtCities = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Город", dtCities);
                try
                {
                    row1[3] = dtCities.Rows[0][1];

                    row2[3] = dtCities.Rows[0][0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. "); ; //results[1];

                    query = DataProvider.GetQueryText("GridPartMax");
                    dtCities = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Город", dtCities);

                    row1[4] = dtCities.Rows[0][1];//results[4];
                    row2[4] = dtCities.Rows[0][0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. "); ;// results[3];
                }
                catch { }

                

				bool emptyRow = true;
				double value;
				for (int j = 1; j < row1.ItemArray.Length; ++j)
					emptyRow = emptyRow && !Double.TryParse(row1[j].ToString(), out value);
				if (!emptyRow)
				{
					dtGrid.Rows.Add(row1);
					dtGrid.Rows.Add(row2);
				}
			}
			grid.DataSource = dtGrid.DefaultView;
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
			
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Продукт");
			GridHeaderCell headerCell = headerLayout.AddCell(MDXDateToShortDateString(compareDate.Value));
			headerCell.AddCell("Минимальная цена, рубль");
			headerCell.AddCell("Максимальная цена, рубль");
			headerCell = headerLayout.AddCell(MDXDateToShortDateString(selectedDate.Value));
			headerCell.AddCell("Минимальная цена, рубль");
			headerCell.AddCell("Максимальная цена, рубль");

			e.Layout.Bands[0].Columns[0].Width = IsSmallResolution ? 150 : CRHelper.GetColumnWidth(250);
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = IsSmallResolution ? 125 : CRHelper.GetColumnWidth(200);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
			}
			headerLayout.ApplyHeaderInfo();
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			UltraWebGrid grid = sender as UltraWebGrid;
			UltraGridRow row = e.Row;
			if (row.Index % 2 == 0)
			{
				foreach (UltraGridCell cell in row.Cells)
				{
					double value;
					if (Double.TryParse(cell.Text, out value))
					{
						cell.Text = String.Format("<b>{0:N2}</b>", value);
					}
					cell.Style.HorizontalAlign = HorizontalAlign.Left;
					if (cell.Column.Index > 0)
					{
						cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
					}
					// Индикация, пока закоментировано. Вдруг захотят...
					/*UltraGridCell compareCell = row.Cells[cell.Column.Index - 2];
					if (cell.Column.Index > 2 && Double.TryParse(cell.Text, out value) && Double.TryParse(compareCell.Text, out value))
					{
						if (Convert.ToDouble(cell.Text) - Convert.ToDouble(compareCell.Text) > 0.0001)
						{
							cell.Style.Padding.Left = 15;
							cell.Style.CssClass = "ArrowUpRed";
						}
						else if (Convert.ToDouble(cell.Text) - Convert.ToDouble(compareCell.Text) < -0.0001)
						{
							cell.Style.Padding.Left = 15;
							cell.Style.CssClass = "ArrowDownGreen";
						}
					}*/
				}
			}
			else
			{
				grid.Rows[row.Index - 1].Cells[0].RowSpan = 2;
				foreach (UltraGridCell cell in row.Cells)
				{
					if (cell.Column.Index > 0)
					{
						cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                        try
                        {
                            cell.Text = SetStarChar(cell.Text);
                        }
                        catch { }
					}
				}
			}
		}
        private string SetStarChar(string RegionName)
        {
            string NameRegion = RegionName;
            return NameRegion;
            string[] StarRegions = new string[14] { "Ханты-Мансийский автономный округ", "Советск", "Сургутск", "Когал", "Ланге", "Мегион", "Нефтеюганск", " Нижневартовск", "Нижневартовский-", "Нягань", "Сургут", "Пыть", "Югорск", "Российская" };
            foreach (string R in StarRegions)
            {
                if (NameRegion.Contains(R))
                {
                    return NameRegion;
                }
            }
            
            return NameRegion + "*";
        }

		#endregion

		#region Общие функции

		protected string GetFoodUnit()
		{
			DataTable dtUnit = new DataTable();
			string query = DataProvider.GetQueryText("ORG_0001_0004_unit");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Продукт", dtUnit);
			if (dtUnit == null || dtUnit.Rows.Count == 0)
                return "килограмм".Replace("илограмм", "г.");
            return CRHelper.ToLowerFirstSymbol(dtUnit.Rows[0][0].ToString()).Replace("илограмм", "г.");
		}

		/// <summary>
		/// Разбирает таблицу с данными. На выходе массив: геометрическое среднее по ХМАО, минимальное и максимальное значение среднего по МР
		/// </summary>
		/// <param name="dtData">Таблица с данными</param>
		/// <param name="results">Массив с результатами</param>
		protected void ParseData(DataTable dtData, DataTable dtCities, object[] results)
		{
			if ((dtData == null || dtData.Rows.Count == 0) && (dtCities == null || dtCities.Rows.Count == 0))
			{
				for (int i = 0; i < results.Length; ++i)
					results[i] = DBNull.Value;
				return;
			}
			double[] gmValues = new double[dtData.Rows.Count + dtCities.Rows.Count];
			string[] gmRegions = new string[dtData.Rows.Count + dtCities.Rows.Count];
			for (int i = 0; i < dtData.Rows.Count; ++i)
			{
				DataRow row = dtData.Rows[i];
				gmRegions[i] = row[0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
				int count = 0;
				for (int j = 1; j < row.ItemArray.Length; ++j)
				{
					object cell = row[j];
					if (cell != DBNull.Value)
					{
						gmValues[i] = (gmValues[i] == 0) ? 
                            Convert.ToDouble(cell) : 
                            gmValues[i] *    Convert.ToDouble(cell);
						++count;
					}
				}
				gmValues[i] = Math.Pow(gmValues[i], 1.0 / count);
			}
			for (int i = dtData.Rows.Count; i < dtData.Rows.Count + dtCities.Rows.Count; ++i)
			{
				DataRow row = dtCities.Rows[i - dtData.Rows.Count];
				gmRegions[i] = row[0].ToString();
				gmValues[i] = Convert.ToDouble(row[1]);
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
			report.Preferences.Printing.PaperOrientation = PaperOrientation.Portrait;
            report.Preferences.Printing.PaperSize = Infragistics.Documents.Reports.Report.Preferences.Printing.PaperSize.A4;
			report.Preferences.Printing.FitToMargins = true;
			ISection section1 = report.AddSection();

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
			foreach (UltraGridRow row in grid.Rows)
				foreach (UltraGridCell cell in row.Cells)
					if (cell.Value != null)
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
			for (int i = 0; i < grid.Rows.Count; i += 2)
			{
				UltraGridCell cell1 = grid.Rows[i].Cells[0];
				UltraGridCell cell2 = grid.Rows[i + 1].Cells[0];

				cell1.Style.BorderDetails.StyleBottom = BorderStyle.None;
				cell2.Style.BorderDetails.StyleTop = BorderStyle.None;
			}

			ReportPDFExporter1.HeaderCellHeight = 20;
			ReportPDFExporter1.Export(headerLayout, section1);
		}

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			UltraWebGrid grid = headerLayout.Grid;
			foreach (UltraGridRow row in grid.Rows)
				foreach (UltraGridCell cell in row.Cells)
					if (cell.Value != null)
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
	
			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Так надо, потом удалю");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 30;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			ReportExcelExporter1.Export(headerLayout, sheet1, 3);
			
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Height = 550;

			for (int i = 5; i < headerLayout.Grid.Rows.Count + 5; ++i)
			{
				sheet1.Rows[i].Height = 255;
				if (i % 2 == 1)
				{
					sheet1.MergedCellsRegions.Add(i, 0, i + 1, 0);
				}
				foreach (WorksheetCell cell in sheet1.Rows[i].Cells)
				{
					if (cell.ColumnIndex > 0)
						if (i % 2 == 1)
							cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
						else
							cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;

				}
			}

			GridHeaderLayout emptyGridLayout = new GridHeaderLayout(emptyExportGrid);
			ReportExcelExporter1.Export(emptyGridLayout, sheet2, 0);
			workbook.Worksheets.Remove(sheet2);
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Portrait;
			e.CurrentWorksheet.PrintOptions.PaperSize = Infragistics.Documents.Excel.PaperSize.A4;
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
