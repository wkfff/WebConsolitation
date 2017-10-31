using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core.Primitives;

/**
 *  Средние розничные цены на продукты питания
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0001_0005
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private static DataTable dtDate;
		private static DataTable dtChart1;
		private static string unit;

		#endregion

		private static bool IsSmallResolution
		{
			get { return CRHelper.GetScreenWidth < 900; }
		}

		private static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		#region Параметры запроса

		private CustomParam selectedDate;
		private CustomParam date;
		private CustomParam compareDate;
		private CustomParam selectedFood;

		#endregion

        // Массивы с продуктами, mdx именами продуктов
        string[] food = { "Говядина", "Свинина","Баранина", "Куры", "Рыба мороженная неразделанная", "Масло сливочное", "Масло подсолнечное",
						  "Молоко питьевое цельное пастеризованное (2,5-3,2 % жирн.)","Молоко питьевое цельное стерилизованное (2,5-3,2 % жирн.)", "Яйца куриные", "Сахар-песок", "Чай черный байховый",
						  "Соль поваренная пищевая", "Мука пшеничная (в/с, 1 сорт)", "Хлеб ржаной, ржано-пшеничный",
						  "Хлеб и булочные изделия из муки пшеничной 1 и 2 сортов", "Рис шлифованный", "Пшено", "Крупа гречневая", "Вермишель", "Картофель",
						  "Капуста", "Лук репчатый", "Морковь", "Яблоки"
                           };
        string[] foodMDX =
		{
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Говядина (кроме бескостного мяса)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Свинина (кроме бескостного мяса)]",
            "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Баранина (кроме бескостного мяса)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Мясо].[Куры (кроме куринных окорочков)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Рыба].[Рыба мороженная неразделенная]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло сливочное]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Масло подсолнечное]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Молоко питьевое цельное пастеризованное (2,5-3,2 % жирн.)]",
"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Молочная продукция].[Молоко питьевое цельное стерилизованное (2,5-3,2 % жирн.)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Яйца куриные]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Сахар, яйца].[Сахар-песок]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Чай, кофе].[Чай черный байховый]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Соль, пряности].[Соль поваренная пищевая]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Мука пшеничная (в/с, 1 сорт)]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб ржаной, ржано-пшеничный]",
			"[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Продовольственные товары].[Зерно, мука, хлеб].[Хлеб и булочные изделия из муки пшеничной 1 и 2 сортов]",
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
		
		// --------------------------------------------------------------------

        private const string PageTitleCaption = "Средние розничные цены в автономном округе";
		private const string PageSubTitleCaption = "Еженедельный мониторинг средних розничных цен на отдельные виды социально значимых продовольственных товаров первой необходимости, Ханты-Мансийский автономный округ – Югра, по состоянию на {0}";
		private const string Chart1TitleCaption = "Динамика средней розничной цены на товар «{0}» по ХМАО – Югре, рублей за {1}";

		// --------------------------------------------------------------------
		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			if (!IsSmallResolution)
			{
				ComboFood.Title = "Товар";
				ComboFood.Width = 400;
				ComboFood.ShowSelectedValue = true;
				ComboFood.ParentSelect = true;
				ComboDate.Title = "Выберите дату";
				ComboDate.Width = 275;
				ComboDate.ShowSelectedValue = true;
				ComboDate.ParentSelect = true;
				ComboCompareDate.Title = "Выберите дату для сравнения";
				ComboCompareDate.Width = 375;
				ComboCompareDate.ShowSelectedValue = true;
				ComboCompareDate.ParentSelect = true;
			}
			else
			{
				ComboFood.PanelHeaderTitle = "Товар";
				ComboFood.Width = 250;
				ComboFood.ShowSelectedValue = false;
				ComboFood.ParentSelect = true;
				ComboDate.PanelHeaderTitle = "Выберите дату";
				ComboDate.Width = 175;
				ComboDate.ShowSelectedValue = false;
				ComboDate.ParentSelect = true;
				ComboCompareDate.PanelHeaderTitle = "Выберите дату для сравнения";
				ComboCompareDate.Width = 225;
				ComboCompareDate.ShowSelectedValue = false;
				ComboCompareDate.ParentSelect = true;
			}

			#region Настройка картинки
			ImageFood.Width = 200;
			ImageArrowUp.Width = 100;
			ImageArrowUp.ImageUrl = String.Format("../../images/k_red_up.png", 0);
			ImageArrowDown.Width = 100;
			ImageArrowDown.ImageUrl = String.Format("../../images/k_green_down.png", 0);
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

			UltraChart1.ChartType = ChartType.AreaChart;

			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 10;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 10;

			UltraChart1.Axis.X.Extent = 60;
			UltraChart1.Axis.X.Labels.Visible = true;
			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.Tooltips.FormatString = "Розничная цена\nна <ITEM_LABEL> года\n<b><DATA_VALUE:N2></b>, рубль";

			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 10;
			UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart1.Legend.Visible = false;

			UltraChart1.AreaChart.LineAppearances.Clear();

			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			lineAppearance.Thickness = 5;

			UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart1.ColorModel.Skin.PEs.Clear();
			PaintElement pe = new PaintElement();
			Color color = Color.MediumSeaGreen;
			Color stopColor = Color.Green;
			PaintElementType peType = PaintElementType.Gradient;
			pe.Fill = color;
			pe.FillStopColor = stopColor;
			pe.ElementType = peType;
			pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
			pe.FillOpacity = (byte)150;
			pe.FillStopOpacity = (byte)150;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);

			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			#endregion

			#region Параметры

			selectedDate = UserParams.CustomParam("selected_date");
			date = UserParams.CustomParam("date");
			compareDate = UserParams.CustomParam("compare_date");
			selectedFood = UserParams.CustomParam("selected_food");

			#endregion
		}

		// --------------------------------------------------------------------

        int GetIndexForImage(int ComboIndex)
        {
            int add = 0;
            if (ComboIndex == 2)
            {
                ///Баранина, было множество споров, какая картинка должна быть, я прдлогал Малахова, НО, как сказили методолги, у заказчиков, оооочень тонкое чуство юмора, а такая шутка, грозит им колапсом давления черепной коробки, поэтому решили поставить картинку живого барана, с серьёзным и задумчивым взглядом как у Медведева, думаю заказчики оценят =)
                return 23;
            }
            if (ComboIndex == 8)
            {
                //Молоко
                return 24;
            }
            if (ComboIndex > 2)
            {
                add--;
            }
            if (ComboIndex > 8)
            {
                add--;
            }
            return add+ComboIndex;
        }


		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboFood(ComboFood, "ORG_0001_0005_list_of_food");
				FillComboDate(ComboDate, "ORG_0001_0005_list_of_dates", 0);
				FillComboDate(ComboCompareDate, "ORG_0001_0005_list_of_dates", 1);
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

			selectedFood.Value = foodMDX[ComboFood.SelectedIndex];
			ImageFood.ImageUrl = String.Format("../../images/Продукты/{0}.png", GetIndexForImage(ComboFood.SelectedIndex));

			LabelLink.Text = String.Format("<a href='../ORG_0001_0006/Default.aspx?paramlist=food={0};selectedDate={1}'>Подробнее...</a>", HttpContext.Current.Server.UrlEncode(ComboFood.SelectedValue), HttpContext.Current.Server.UrlEncode(selectedDate.Value));

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(PageSubTitleCaption, ComboDate.SelectedValue);

			FillData();
			UltraChart1.DataBind();
		}

		// --------------------------------------------------------------------

		protected string GetUnit()
		{
			DataTable dtUnit = new DataTable();
			string query = DataProvider.GetQueryText("ORG_0001_0005_unit");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtUnit);
			if (dtUnit == null)
				return "Единица измерения";
			else
				return dtUnit.Rows[0][1].ToString().Replace("илограмм","г.");
		}

		protected void FillData()
		{
			LabelFood.Text = ComboFood.SelectedValue;
			DataTable dtData1 = new DataTable();
			string query = DataProvider.GetQueryText("ORG_0001_0005_data_part1");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtData1);
			DataTable dtData2 = new DataTable();
			query = DataProvider.GetQueryText("ORG_0001_0005_data_part2");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtData2);
			ParseData(dtData1, dtData2);

            DataTable dtData3 = new DataTable();
            query = DataProvider.GetQueryText("HMAO-VAL-CUR");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtData3);

            DataTable dtData4 = new DataTable();
            query = DataProvider.GetQueryText("HMAO-VAL-PREV");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtData4);

            try
            { 
                decimal prev = (decimal)dtData4.Rows[0][1];
                decimal Cur = (decimal)dtData3.Rows[0][1];
                SetHmaoValue((double)(Cur / prev - 1) );
            }
            catch { }
            

		}

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{
            double PrevHMAO = 0;
            double CurHMAO = 0;

			LabelChart1.Text = String.Format(Chart1TitleCaption, ComboFood.SelectedValue, GetUnit().ToLower());
			if (dtDate != null)
			{
				dtChart1 = new DataTable();
				dtChart1.Columns.Add("Дата", typeof(string));
				dtChart1.Columns.Add("Средняя розничная цена", typeof(double));
				int start = dtDate.Rows.Count < 48 ? 0 : dtDate.Rows.Count - 48;
				for (int i = start; i < dtDate.Rows.Count; ++i)
				{
					string year = dtDate.Rows[i][0].ToString();
					string month = dtDate.Rows[i][3].ToString();
					string day = dtDate.Rows[i][4].ToString();
					date.Value = 
                        StringToMDXDate(day + " " + 
                        CRHelper.RusMonthGenitive(CRHelper.MonthNum(month))
                        + ' ' + year + " года");

					DataTable dtData2 = new DataTable();
                    string query = DataProvider.GetQueryText("ORG_0001_0005_chart_part2");
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtData2);
                    if (dtData2.Rows.Count > 0)
                    {
                        DataRow row = dtChart1.NewRow();
                        row[0] = MDXDateToShortDateString(date.Value);
                        row[1] = dtData2.Rows[0][1];

                        try
                        {
                            PrevHMAO = CurHMAO;

                            CurHMAO = (double)dtData2.Rows[0][1];
                        }
                        catch { }
                        
                        dtChart1.Rows.Add(row);
                    }
				}

				UltraChart1.Series.Clear();
				for (int i = 1; i < dtChart1.Columns.Count; i++)
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
            Text Caption = new Text();
            Caption.SetTextString(string.Format("рублей за {0}", GetUnit().ToLower()));
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -37;
            Caption.bounds.Y = 150;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
		}

		#endregion

		// --------------------------------------------------------------------

		#region Общие функции
		
		/// <summary>
		/// Разбирает таблицу с данными. Возвращает среднее геометрическое
		/// Сначала рассчитываются средние геометрические по строкам, а потом уже по ним рассчитывается по всей таблице
		/// </summary>
		/// <param name="dtData1">Таблица с данными</param>
		/// <param name="dtData2">Еще таблица с данными</param>
		protected double GetGeoMean(DataTable dtData1, DataTable dtData2)
		{
			double[] gmValues = new double[dtData1.Rows.Count + dtData2.Rows.Count];
			for (int i = 0; i < dtData1.Rows.Count; ++i)
			{
				DataRow row = dtData1.Rows[i];
				int count = 0;
				gmValues[i] = 1.0;
				for (int j = 1; j < row.ItemArray.Length; ++j)
				{
					object cell = row[j];
					double value;
					if (Double.TryParse(cell.ToString(), out value) && value != 0)
					{
						gmValues[i] *= value;
						++count;
					}
				}
				gmValues[i] = Math.Pow(gmValues[i], 1.0 / count);
			}
			for (int i = dtData1.Rows.Count; i < dtData1.Rows.Count + dtData2.Rows.Count; ++i)
			{
				DataRow row = dtData2.Rows[i - dtData1.Rows.Count];
				gmValues[i] = Convert.ToDouble(row[1]);
			}
			double result = 1.0;
			foreach (double value in gmValues)
			{
				result *= value;
			}
			result = Math.Pow(Convert.ToDouble(result), 1.0 / gmValues.Length);
			return result;
		}

        class ComparerSortRegionValue : System.Collections.Generic.IComparer<RegionValue>
        {

            #region Члены IComparer<RegionValue>

            public int Compare(RegionValue x, RegionValue y)
            {
                return -Compare_(x, y);
            }

            public int Compare_(RegionValue x, RegionValue y)
            {
                string Xname = x.NameRegion;
                string Yname = y.NameRegion;

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

        class RegionValue
        {
            public string NameRegion;
            public double Value;
            public RegionValue(string NameRegion,double Value)
            {
                this.NameRegion = NameRegion;
                this.Value = Value;
            }
        }

        struct RegionValueArray
        {
            public double[] regionValues;
            public string[] regionNames;
        }

        private RegionValueArray SortRegionValue(double[] regionValues, string[] regionNames)
        {
            List<RegionValue> SortList = new List<RegionValue>();
            int i = 0;
            for (; regionValues.Length > i; i++)
            {
                SortList.Add(new RegionValue(regionNames[i],regionValues[i]));
            }

            SortList.Sort(new  ComparerSortRegionValue());

            RegionValueArray arr = new RegionValueArray();
            arr.regionNames = new string[SortList.Count];
            arr.regionValues = new double[SortList.Count];

            i = 0;
            foreach (RegionValue RV in SortList)
            {
                arr.regionNames[i] = RV.NameRegion;
                arr.regionValues[i] = RV.Value;
                i++;
            }
            return arr;
            
        }

        private static bool IsStarRegion(string[] regionNames,int i)
        {
            string[] StarRegions = new string[13] { "Ханты-Мансийский автономный округ", "Советск", "Сургутск", "Когал", "Ланге", "Мегион", "Нефтеюганск", "Нижневартовский-", "Нягань", "Сургут", "Пыть", "Югорск", "Город Нижневар" };
            foreach (string s in StarRegions)
            {
                if (regionNames[i].Contains(s))
                {
                    return false;
                }
            }
            return true;
        }

        private static void SetStarForRegion(string[] regionNames)
        {
            for (int i = 0; i < regionNames.Length; i++)
            {
                if (IsStarRegion(regionNames, i))
                {
                    regionNames[i] += "*";
                }
            }
        }

        private void SetReginUpDouwnValue(string[] regionNames, double[] regionValues)
        {
            for (int i = 0; i < regionNames.Length; ++i)
            {
                if (regionValues[i] < 0)
                {
                    LabelDown.Text = ConcatenateStrings(LabelDown.Text, String.Format("{0} {1:P2}", regionNames[i], regionValues[i]), "<br/>");
                }
                if (regionValues[i] > 0)
                {
                    LabelUp.Text = ConcatenateStrings(LabelUp.Text, String.Format("{0} +{1:P2}", regionNames[i], regionValues[i]), "<br/>");
                }
            }
        }

        private void SetHmaoValue(double HMAO)
        {


            if (HMAO > 0)
                LabelHMAO.Text = String.Format("{0} +{1:P2}", "Ханты-Мансийский автономный округ - Югра", HMAO);
            else
                LabelHMAO.Text = String.Format("{0} {1:P2}", "Ханты-Мансийский автономный округ - Югра", HMAO);
        }

        private void SetLabelHmao()
        {
            if (String.IsNullOrEmpty(LabelDown.Text) && String.IsNullOrEmpty(LabelUp.Text))
            {
                LabelHMAO.Text = "Изменений средней розничной цены в ХМАО-Югре не было зарегистрировано";
            }
            if (String.IsNullOrEmpty(LabelDown.Text))
            {
                LabelDown.Text = "Изменений уровня цен в сторону снижения не наблюдалось";
            }
            if (String.IsNullOrEmpty(LabelUp.Text))
            {
                LabelUp.Text = "Изменений уровня цен в сторону увеличения не наблюдалось";
            }

        }
        private void ClearLabelUpDownHmao()
        {
            LabelUp.Text = String.Empty;
            LabelHMAO.Text = String.Empty;
            LabelDown.Text = String.Empty;
        }

        /// <summary>
        /// Разбирает таблицу с данными.
        /// </summary>
        /// <param name="dtData1">Таблица с данными</param>
        /// <param name="dtData2">Еще таблица с данными</param>
        protected void ParseData(DataTable dtData1, DataTable dtData2)
		{
			string[] regionNames = new string[dtData1.Rows.Count + dtData2.Rows.Count];
			double[] regionValues = new double[dtData1.Rows.Count + dtData2.Rows.Count];
			double selectedPriceHMAO = 1, comparePriceHMAO = 1;
			double HMAO;
			for (int row_num = 0; row_num < dtData1.Rows.Count; ++row_num)
			{
				DataRow row = dtData1.Rows[row_num];
				regionNames[row_num] = row[4].ToString();
				double selectedPrice = 1, comparePrice = 1;
				double value;
				int orgCount = (row.ItemArray.Length - 5) / 2;
				int selectedPow = 0, comparePow = 0; ;
				for (int i = 0; i < orgCount; ++i)
				{
					if (Double.TryParse(row[5 + i].ToString(), out value) 
                        && value != 0)
					{
						comparePrice *= value;
						++comparePow;
					}
					if (Double.TryParse(row[5 + orgCount + i].ToString(), out value) 
                        && value != 0)
					{
						selectedPrice *= value;
						++selectedPow;
					}
				}

				comparePrice = Math.Pow(comparePrice, 1.0 / comparePow);
				selectedPrice = Math.Pow(selectedPrice, 1.0 / selectedPow);
				regionValues[row_num] = selectedPrice / comparePrice - 1;
				comparePriceHMAO *= comparePrice;
				selectedPriceHMAO *= selectedPrice;
			}
			for (int row_num = dtData1.Rows.Count; row_num < dtData1.Rows.Count + dtData2.Rows.Count; ++row_num)
			{
				DataRow row = dtData2.Rows[row_num - dtData1.Rows.Count];
				regionNames[row_num] = row[0].ToString();
				double selectedPrice, comparePrice;
				if (!(Double.TryParse(row[2].ToString(), out comparePrice)
                    && Double.TryParse(row[1].ToString(), out selectedPrice)
                    && comparePrice != 0
                    && selectedPrice != 0))
				{
					selectedPrice = 1;
					comparePrice = 1;
				}

				regionValues[row_num] = selectedPrice / comparePrice - 1;
				comparePriceHMAO *= comparePrice;
				selectedPriceHMAO *= selectedPrice;
			}
			comparePriceHMAO = Math.Pow(comparePriceHMAO, 1.0 / (dtData1.Rows.Count + dtData2.Rows.Count));
			selectedPriceHMAO = Math.Pow(selectedPriceHMAO, 1.0 / (dtData1.Rows.Count + dtData2.Rows.Count));
			HMAO = selectedPriceHMAO / comparePriceHMAO - 1;
			Array.Sort(regionValues, regionNames);

            ClearLabelUpDownHmao();
            SetStarForRegion(regionNames);
            SetReginUpDouwnValue(regionNames, regionValues);
            //SetHmaoValue(HMAO);
            SetLabelHmao();
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

		protected void FillComboFood(CustomMultiCombo combo, string queryName)
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			foreach (string foodName in food)
				AddPairToDictionary(dict, foodName, 0);
			combo.FillDictionaryValues(dict);
		}

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки: преобразования и все такое

		public string ConcatenateStrings(string firstStr, string secondStr, string separator)
		{
			if (String.IsNullOrEmpty(firstStr))
				return secondStr;
			else if (String.IsNullOrEmpty(secondStr))
			{
				return firstStr;
			}
			else
				return firstStr + separator + secondStr;
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
	}
}
