using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_017
{
	public partial class Default : CustomReportPage
	{
		// константы

		/// <summary> 
		/// "Большое" число, соответствующее множественному числу
		/// </summary>
		private const int COUNT_MANY = 10;

		// переменные

		// параметры запроса
		private CustomParam selectedPeriod;
		private CustomParam paramWhere;
		private CustomParam paramMark;
		private CustomParam paramMark2;
		
		private string periodNominative;
		private string periodPrepositional;
		private string markFormat;
		private string markSI;

		private int countCharts;
		private int countTables;
		
		private bool export;

		// словарики
		private Dictionary<TypeList, Dictionary<TypeListPlaces, string>> lists;
		private Dictionary<string, string[]> nomiMany; 
		private Dictionary<string, string[]> geniMany;
		private Dictionary<string, string[]> ablaMany;
		private Dictionary<string, string[]> prepMany;
		private Dictionary<string, string> geniOne;
		private Dictionary<string, string> prepOne;

		// делегаты
		delegate string MonthConverter(int monthNum);
		delegate string ListFormat(DataRow row, int index, int count, object[] param);


		public Default()
		{
			Server.ScriptTimeout = 600; // 10 минут

			countCharts = 1;
			countTables = 1;
			
			#region Заполнение словарей

			// именительный падеж
			nomiMany = new Dictionary<string, string[]> // 1,2-4,5
			{
				{"ед.", 
					new[] {"ед.", String.Empty, String.Empty}},
				{"транспортное", 
					new[] {"транспортн", "ое", "ых"}},
				{"средство", 
					new[] {"средств", "о", "а", String.Empty}},
				{"человек", 
					new[] {"человек", String.Empty, "а", String.Empty}}, 
				{"лицо", 
					new[] {"лиц", "о", "а", String.Empty}},
				{"больной", 
					new[] {"больн", "ой", "ых"}},
				{"выявлен", 
					new[] {"выявлен", String.Empty, "о"}},
				{"досмотрена", 
					new[] {"досмотрен", "а", "о"}},
				{"запрещена", 
					new[] {"запрещен", "а", "ы", "о"}},
				{"партия", 
					new[] {"парти", "я", "и", "й"}},
				{"пункт", 
					new[] {"пункт", String.Empty, "а", "ов"}},
				{"тонн", // всегда тонн
					new[] {"тонн", String.Empty, String.Empty}},

				{"Автомобильный", 
					new[] {"автомобильн", "ый", "ых"}},
				{"Воздушный", 
					new[] {"воздушн", "ый", "ых"}},
				{"Железнодорожный", 
					new[] {"железнодорожн", "ый", "ых"}},
				{"Морской", 
					new[] {"морск", "ой", "их"}},
				{"Речной", 
					new[] {"речн", "ой", "ых"}},
				{"Смешанный", 
					new[] {"смешанн", "ый", "ых"}},
			};

			// родительный падеж
			geniMany = new Dictionary<string, string[]> // 1,2-4,5
			{
				{"субъект", 
					new[] {"субъект", "а", "ов"}},
				{"транспортное", 
					new[] {"транспортн", "ого", "ых"}},
				{"средство", 
					new[] {"средств", "а", String.Empty}},
			};

			// творительный падеж
			ablaMany = new Dictionary<string, string[]> // 1,2-4,5
			{
				{"управление", 
					new[] {"управлени", "ем", "ями"}},
			};

			// предложный падеж
			prepMany = new Dictionary<string, string[]> // 1,2-4,5
			{
				{"пункт", 
					new[] {"пункт", "е", "ах"}},
				{"участок", 
					new[] {"участк", "е", "ах"}},
				{"субъект", 
					new[] {"субъект", "е", "ах"}},
				
			    {"Автомобильный", 
					new[] {"автомобильн", "ом", "ых"}},
				{"Воздушный", 
					new[] {"воздушн", "ом", "ых"}},
				{"Железнодорожный", 
					new[] {"железнодорожн", "ом", "ых"}},
				{"Морской", 
					new[] {"морск", "ом", "их"}},
				{"Речной", 
					new[] {"речн", "ом", "ых"}},
				{"Смешанный", 
					new[] {"смешанн", "ом", "ых"}},
			};
			
			// родительный падеж
			geniOne = new Dictionary<string, string>
			{
				// на территории ХХХ
				{"г. Москва", "города Москвы"},
				{"г. Санкт-Петербург", "города Санкт-Петербурга"},
				{"Алтайский край", "Алтайского края"},
				{"Амурская область", "Амурской области"},
				{"Архангельская область", "Архангельской области"},
				{"Астраханская область", "Астраханской области"},
				{"Белгородская область", "Белгородской области"},
				{"Брянская область", "Брянской области"},
				{"Волгоградская область", "Волгоградской области"},
				{"Воронежская область", "Воронежской области"},
				{"Еврейская автономная область", "Еврейской автономной области"},
				{"Забайкальский край", "Забайкальского края"},
				{"Иркутская область", "Иркутской области"},
				{"Кабардино-Балкарская Республика", "Кабардино-Балкарской Республики"},
				{"Калининградская область", "Калининградской области"},
				{"Камчатский край", "Камчатского края"},
				{"Кемеровская область", "Кемеровской области"},
				{"Краснодарский край", "Краснодарского края"},
				{"Красноярский край", "Красноярского края"},
				{"Курганская область", "Курганской области"},
				{"Курская область", "Курской области"},
				{"Ленинградская область", "Ленинградской области"},
				{"Магаданская область", "Магаданской области"},
				{"Московская область", "Московской области"},
				{"Мурманская область", "Мурманской области"},
				{"Нижегородская область", "Нижегородской области"},
				{"Новосибирская область", "Новосибирской области"},
				{"Омская область", "Омской области"},
				{"Оренбургская область", "Оренбургской области"},
				{"Пермский край", "Пермского края"},
				{"Приморский край", "Приморского края"},
				{"Псковская область", "Псковской области"},
				{"Республика Алтай", "Республики Алтай"},
				{"Республика Башкортостан", "Республики Башкортостан"},
				{"Республика Бурятия", "Республики Бурятия"},
				{"Республика Дагестан", "Республики Дагестан"},
				{"Республика Калмыкия", "Республики Калмыкия"},
				{"Республика Карелия", "Республики Карелия"},
				{"Республика Коми", "Республики Коми"},
				{"Республика Саха (Якутия)", "Республики Саха (Якутия)"},
				{"Республика Северная Осетия - Алания", "Республикии Северная Осетия - Алания"},
				{"Республика Татарстан", "Республики Татарстан"},
				{"Республика Тыва", "Республики Тыва"},
				{"Республика Хакасия", "Республики Хакасия"},
				{"Ростовская область", "Ростовской области"},
				{"Самарская область", "Самарской области"},
				{"Саратовская область", "Саратовской области"},
				{"Сахалинская область", "Сахалинской области"},
				{"Свердловская область", "Свердловской области"},
				{"Ставропольский край", "Ставропольского края"},
				{"Тюменская область", "Тюменской области"},
				{"Ульяновская область", "Ульяновской области"},
				{"Хабаровский край", "Хабаровского края"},
				{"Ханты-Мансийский автономный округ", "Ханты-Мансийского автономного округа"},
				{"Челябинская область", "Челябинской области"},
				{"Чеченская Республика", "Чеченской Республики"},
				{"Чувашская Республика", "Чувашской Республики"},
				{"Чукотский автономный округ", "Чукотского автономного округа"},
				{"Ярославская область", "Ярославской области"},
			};

			// предложный падеж
			prepOne = new Dictionary<string, string>
			{
				{"Центральный ФО", "Центральном"},
				{"Северо-Западный ФО", "Северо-Западном"},
				{"Северо-Кавказский ФО", "Северо-Кавказском"},
				{"Приволжский ФО", "Приволжском"},
				{"Уральский ФО", "Уральском"},
				{"Сибирский ФО", "Сибирском"},
				{"Дальневосточный ФО", "Дальневосточном"},
				{"Южный ФО", "Южном"},

				{"Российско-норвежский", "на российско-норвежском"},
				{"Российско-финский", "на российско-финском"},
				{"Российско-эстонский", "на российско-эстонском"},
				{"Российско-латвийский", "на российско-латвийском"},
				{"Российско-литовский", "на российско-литовском"},
				{"Российско-польский", "на российско-польском"},
				{"Российско-украинский", "на российско-украинском"},
				{"Российско-абхазский", "на российско-абхазском"},
				{"Российско-южноосетинский", "на российско-южноосетинском"},
				{"Российско-грузинский", "на российско-грузинском"},
				{"Российско-азербайджанский", "на российско-азербайджанском"},
				{"Российско-казахстанский", "на российско-казахстанском"},
				{"Российско-монгольский", "на российско-монгольском"},
				{"Российско-китайский", "на российско-китайском"},
				{"Российско-корейский", "на российско-корейском"},
			};
			
			// Формат списков
			lists = new Dictionary<TypeList, Dictionary<TypeListPlaces, string>>
			{
				{TypeList.Simple, new Dictionary<TypeListPlaces, string>
				    {
						{TypeListPlaces.One, "#"},
						{TypeListPlaces.First, "#, "},
						{TypeListPlaces.Other, "#, "},
						{TypeListPlaces.PreLast, "# "},
						{TypeListPlaces.Last, "и #"},
				    }
				},
				{TypeList.InLine, new Dictionary<TypeListPlaces, string>
				    {
						{TypeListPlaces.One, "в #"},
						{TypeListPlaces.First, "в #, "},
						{TypeListPlaces.Other, "в #, "},
						{TypeListPlaces.PreLast, "в # "},
						{TypeListPlaces.Last, "и в #"},
				    }
				},
				{TypeList.InBR, new Dictionary<TypeListPlaces, string>
				    {
						{TypeListPlaces.One, "в #"},
						{TypeListPlaces.First, "в #,<br />"},
						{TypeListPlaces.Other, "в #,<br />"},
						{TypeListPlaces.PreLast, "в #<br />"},
						{TypeListPlaces.Last, "и в #"},
				    }
				},
			};

			#endregion

		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedPeriod = UserParams.CustomParam("selected_period");
			paramWhere = UserParams.CustomParam("where");
			paramMark = UserParams.CustomParam("mark");
			paramMark2 = UserParams.CustomParam("mark2");

			// Настройка экспорта
			ExporterPdf.PdfExportButton.Click += PdfExportButton_Click;
			ExporterDoc.DocExportButton.Click += DocExportButton_Click;
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			if (!Page.IsPostBack)
			{
				ParamDate paramDate = SKKHelper.ParamDate_Init();

				// отчетный период
				comboPeriodYear.ComboCurrentYear_Init(paramDate.GetFirstYear(), paramDate.GetLastYear(), paramDate.GetLastYear());
				comboPeriodMonth.ComboCurrentMonth_Init(paramDate.GetLastMonth());
			}

			// export определяет, осуществляется ли обработка отчета для экспорта
			string eventTarget = Page.Request.Form.Get("__EVENTTARGET");
			export = eventTarget != null && (eventTarget.Contains("pdfExportButton") || eventTarget.Contains("docExportButton"));

			// изменение результатов запроса
			SKKHelper.ChangeSelectedPeriod(comboPeriodMonth);

			// сбор параметров 
			string selectedMonthUnique = SKKHelper.GetUniqueMonths(comboPeriodMonth.SelectedValues, comboPeriodYear.SelectedValue);

			// текстовики
			PageTitle.Text = "Анализ результатов санитарно-карантинного контроля в пунктах пропуска на российском участке внешней границы Таможенного союза";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Empty;
			txtPeriod.Text = "Отчетный период";

			// параметры для запроса
			selectedPeriod.Value = selectedMonthUnique;
			
			// инициализация и запуск
			SetDataHelpers();
			
		}

		private void DocExportButton_Click(object sender, EventArgs e)
		{
			SKKExportDoc exporter = new SKKExportDoc(Page, report);
			InitExporter(exporter);
			exporter.Export();
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKExportPDF exporter = new SKKExportPDF(ExporterPdf.UltraWebGridDocumentExporter, report);
			InitExporter(exporter);
			exporter.Export();
		}
		
		private void InitExporter(SKKExportBase exporter)
		{
			exporter.HeaderHeight = 55;
			exporter.HeaderText = PageTitle.Text + ", " + PeriodNominative();
			exporter.Header1stTextLeft = "\"Мониторинг СКК\"";
			exporter.Header1stTextRight = DateTime.Now.ToString("dd MMMM yyyy, H:mm:ss");
			exporter.FooterHeight = 45;
			exporter.FooterText = HttpContext.Current.Request.Url.AbsoluteUri;

		}

		private void SetDataHelpers()
		{
			periodNominative = PeriodNominative();
			periodPrepositional = PeriodPrepositional();
			HtmlGenericControl div;
			StringBuilder text;

			// общие стили
			report.Style.Add("font-size", "10pt");

			// ШАПКА
			
			paramWhere.Value = "[Направления].[Направление].[Все направления]";
			
			report.Controls.Add(StyleMainTitle(ToHTML(Text(
				"Анализ сведений об осуществлении санитарно-карантинного контроля<br />"+
				"на российском участке внешней границы Таможенного союза<br />за #PERIOD_NOMINATIVE#"
				))));
			
			div = StyleSimple(ToHTML(Paragraph(ReplacePPGeneric(Text(
				"В #PERIOD_PREPOSITIONAL#, санитарно-карантинный контроль на российском участке внешней границы Таможенного союза " +
				"за прибытием и/или отбытием транспортных средств, пассажиров и грузов осуществлялся в общей сложности " +
				"в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				)))));
			
			if (div.InnerText.ToLower().Contains("нет данных"))
			{
				// если нет данных, то заканчиваем формирование отчета
				report.Controls.Add(StyleCenter(ToHTML(Text("<br /><br /><br /><b>нет данных</b><br /><br />"))));
				return;
			}
			report.Controls.Add(div);
			
			// ВВОЗ
			
			paramWhere.Value = "[Направления].[Направление].[Все направления].[ Прибытие в РФ]";
			
			report.Controls.Add(StyleMainTitle(ToHTML(Text("ПРИ ПРИБЫТИИ В РОССИЮ"))));
			report.Controls.Add(StyleSimple(ToHTML(Paragraph(ReplacePPGeneric(Text(
				"В #PERIOD_PREPOSITIONAL#, санитарно-карантинный контроль на российском участке внешней границы Таможенного союза " +
				"за прибытием/ввозом транспортных средств, пассажиров и грузов осуществлялся " +
				"в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				))))));
			
			#region ВВОЗ, ТС
			
			markSI = "ед.";
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Количество досмотренных транспортных средств]";
			paramMark2.Value = "[Показатели].[Показатель].[Количество приостановленных транспортных средств]";

			report.Controls.Add(StyleSubTitle(ToHTML(Paragraph(Text("1. Досмотр транспортных средств")))));
			
			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр транспортных средств при прибытии в Российскую Федерацию осуществлялся #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора " +
				"на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации, " +
				"в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				);
			text = ReplaceSubjects(text, 0);
			text = ReplacePRN(text);
			text = ReplacePPMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по субъектам
			text = Text(
				SKKHelper.AddParagraph() +
				"Во всех пунктах пропуска в #PERIOD_PREPOSITIONAL# было досмотрено при прибытии в Россию " +
				"#MARK_COUNT#__#MARK_TRANSP1_NOMI# #MARK_TRANSP2_NOMI#.<br />" +
				SKKHelper.AddParagraph() +
				"Наибольшее число транспортных средств досматривалось на территории #SUBJ_ITEMS#.<br />" +
				SKKHelper.AddParagraph() +
				"В таблице__#TABLE_NEXT# приведены сведения в разрезе субъектов Российской Федерации, " +
				"на территории которых досматривались транспортные средства при прибытии в Россию."
				);
			text = ReplaceMark(text);
			text = ReplaceSubjects(text, 4);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по субъектам, таблица			
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Досмотр транспортных средств<br />при прибытии в Российскую Федерацию в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongBegin));
			div.Controls.Add(AddGridMark(typeof(GridHelpMarkTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, по ФО
			text = Text(
				SKKHelper.AddParagraph() +
				"В разрезе федеральных округов больше всего досмотренных транспортных средств было в " +
				"#DISTR_ITEMS# федеральных округах (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceDistricts(text, 4, "от общего количества досмотренных транспортных средств в целом по Российской Федерации");
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ТС, по ФО, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа транспортных средств,<br />досмотренных на российском участке внешней границы " +
				"Таможенного союза,<br />по федеральным округам Российской Федерации<br />(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartDistricts(typeof(ChartHelpDistrictsTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, по виду сообщения
			text = Text(
				SKKHelper.AddParagraph() +
				"Наибольшее число транспортных средств досмотрено в #TRANSP_MAX1_PREP# пунктах пропуска__— #TRANSP_MAX1_PRC# " +
				"от общего количества транспортных средств, досмотренных в целом на российском участке внешней границы Таможенного союза. " +
				"На долю #TRANSP_OTHER_PREP# пунктов пропуска приходится в общей сложности #TRANSP_OTHER_PRC# (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceTransport(text, String.Empty);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по виду сообщения, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа транспортных средств,<br />досмотренных на российском участке внешней границы Таможенного союза,<br />"+
				"по видам международного сообщения в пунктах пропуска<br />(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartTransport(typeof(ChartHelpTransportTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр транспортных средств осуществлялся на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы России, где в общей сложности " +
				"было досмотрено #BORDERS_MARK_COUNT#__#BORDERS_MARK_TRANSP1_NOMI# #BORDERS_MARK_TRANSP2_NOMI#, при этом наибольшее количество__— " +
			    "#BORDERS_ITEMS_PRC# участках Государственной границы РФ (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 3, "в общем количестве транспортных средств, досмотренных на всех участках");
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа транспортных средств,<br />досмотренных на российском участке внешней границы Таможенного союза,<br />"+
				"по участкам Государственной границы Российской Федерации<br />(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, приостановлено
			paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных транспортных средств]";

			text = Text(
				SKKHelper.AddParagraph() + 
				"В #PERIOD_PREPOSITIONAL# был приостановлен (временно запрещен) въезд #MARK_COUNT#__#MARK_TRANSP1_GENI# #MARK_TRANSP2_GENI#. " +
				"Пропуск приостанавливался в #SUBJ_COUNT#__#SUBJ_PREP# Российской Федерации, в #PP_COUNT#__#PP_PREP# пропуска" +
				"$#PP_COUNT#?, из которых #PP_TRANSP_ITEMS_LINE_NOMI#$.<br />" +
				"$#PP_COUNT#?" + SKKHelper.AddParagraph() + "#TRANSP_ITEMS_PREP#.<br />$" +
				
				SKKHelper.AddParagraph() + 
				"Наибольшее число транспортных средств, пропуск которых был приостановлен, "+
				"зафиксировано #BORDERS_ITEMS#__#BORDERS_PREP# Государственной границы Российской Федерации (таблица__#TABLE_NEXT#).<br />"
				);
			text = ReplaceTransport(text, "пунктах пропуска приостановлен въезд #MARK_COUNT#__#MARK_TRANSP1_GENI# #MARK_TRANSP2_GENI#");
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceSubjects(text, 0);
			text = ReplacePPMark(text);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ТС, приостановлено, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Перечень пунктов пропуска, в которых приостанавливался<br />"+
				"(временно запрещался) пропуск транспортных средств<br />(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddGridBorders(typeof(GridHelpBordersCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			#endregion
			
			#region ВВОЗ, ЛИЦА
			
			markSI = "человек";
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Число лиц, досмотренных на наличие признаков инфекционных заболеваний]";
			paramMark2.Value = "[Показатели].[Показатель].[Выявлено больных и/или лиц с подозрением на инфекционные заболевания]";
			
			report.Controls.Add(StyleSubTitle(ToHTML(Paragraph(Text("2. Досмотр лиц на наличие признаков инфекционных заболеваний")))));

			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр лиц на наличие признаков инфекционных заболеваний осуществлялся #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора "+
				"на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации и проводился " +
				"в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				);
			text = ReplaceSubjects(text, 0);
			text = ReplacePRN(text);
			text = ReplacePPMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, по субъектам
			text = Text(
				SKKHelper.AddParagraph() +
				"В целом по Российской Федерации на наличие признаков инфекционных заболеваний было досмотрено " +
				"при прибытии в Российскую Федерацию #MARK_COUNT#__#MARK_PEOPLE_NOMI#.<br />"+
				SKKHelper.AddParagraph() +
				"Наибольшее число лиц было досмотрено на территории #SUBJ_ITEMS#.<br />" +
				SKKHelper.AddParagraph() +
				"В таблице__#TABLE_NEXT# приведены сведения в разрезе субъектов Российской Федерации, о числе досмотренных лиц " +
				"на наличие признаков инфекционных заболеваний."
				);
			text = ReplaceMark(text);
			text = ReplaceSubjects(text, 6);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ЛИЦА, по субъектам, таблица

			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число досмотренных лиц на наличие признаков инфекционных заболеваний<br />"+
				"при прибытии в Российскую Федерацию в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongBegin));
			div.Controls.Add(AddGridMark(typeof(GridHelpMarkPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ЛИЦА, по ФО
			text = Text(
				SKKHelper.AddParagraph() +
				"Наибольшее число прибывающих в Россию лиц досмотрено в #DISTR_ITEMS# федеральных округах (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceDistricts(text, 4, "от общего числа лиц, досмотренных на российском участке внешней границы Таможенного союза");
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ЛИЦА, по ФО, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа лиц, досмотренных на наличие признаков инфекционных<br />заболеваний, "+
				"по федеральным округам Российской Федерации<br />(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartDistricts(typeof(ChartHelpDistrictsPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ЛИЦА, по виду сообщения
			text = Text(
				SKKHelper.AddParagraph() +
				"В #TRANSP_MAX1_PREP# пунктах пропуска досмотрено #TRANSP_MAX1_PRC# от общего числа лиц, "+
				"досмотренных на наличие признаков инфекционных заболеваний на российском участке внешней границы Таможенного союза. "+
				"На долю #TRANSP_MAX2_PREP# пунктов пропуска приходится #TRANSP_MAX2_PRC#, #TRANSP_MAX3_PREP#__— #TRANSP_MAX3_PRC# (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceTransport(text, String.Empty);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, по виду сообщения, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа лиц, досмотренных на наличие признаков инфекционных<br />заболеваний " +
				"на российском участке внешней границы Таможенного союза,<br />по видам международного сообщения "+
				"в пунктах пропуска<br />(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartTransport(typeof(ChartHelpTransportPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ЛИЦА, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр лиц проводился на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы Российской Федерации, где за #PERIOD_NOMINATIVE# "+
				"на наличие признаков инфекционных заболеваний досмотрено #BORDERS_MARK_COUNT#__#BORDERS_MARK_PEOPLE_NOMI#. Наибольшее количество__— "+
				"#BORDERS_ITEMS_PRC# участках Государственной границы РФ (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 2, "в общем количестве лиц, досмотренных на всех участках");
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа лиц, досмотренных на наличие признаков инфекционных<br />"+
				"заболеваний на российском участке внешней границы Таможенного союза,<br />"+
				"по участкам Государственной границы Российской Федерации<br />(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ЛИЦА, приостановлено
			paramMark.Value = "[Показатели].[Показатель].[Выявлено больных и/или лиц с подозрением на инфекционные заболевания]";

			text = Text(
				SKKHelper.AddParagraph() +
				"В #PERIOD_PREPOSITIONAL# при прибытии в Российскую Федерацию #MARK_DETECTED_NOMI# #MARK_COUNT#__" +
				"#MARK_PEOPLE_ILL_NOMI# и/или #MARK_PEOPLE_FACE_NOMI# с подозрением на инфекционные заболевания.<br />" +
				
				SKKHelper.AddParagraph() + 
				"Больные/подозрительные на инфекционные заболевания выявлялись в #PP_COUNT#__#PP_PREP# пропуска" +
				"$#PP_COUNT#?, из которых #PP_TRANSP_ITEMS_LINE_NOMI#$.<br />" +
				"$#PP_COUNT#?" + SKKHelper.AddParagraph() + "#TRANSP_ITEMS_PREP#.<br />$" +

				SKKHelper.AddParagraph() +
				"Больные/подозрительные на инфекционные заболевания выявлены на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы, " +
				"на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации. Наибольшее число больных/подозрительных на " +
				"инфекционные заболевания выявлено #BORDERS_ITEMS#__#BORDERS_PREP# Государственной границы (таблица__#TABLE_NEXT#).<br />"
				);
			text = ReplaceTransport(text, "пунктах пропуска #MARK_DETECTED_NOMI# #MARK_COUNT#__#MARK_PEOPLE_ILL_NOMI# и/или #MARK_PEOPLE_FACE_NOMI# с подозрением на инфекционные заболевания");
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceSubjects(text, 0);
			text = ReplacePPMark(text);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, приостановлено, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число выявленных больных/лиц с подозрением на инфекционные заболевания<br />" +
				"(прибытие в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddGridBorders(typeof(GridHelpBordersCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			#endregion
			
			#region ВВОЗ, ГРУЗЫ
			
			markSI = "партия";
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, на которые предъявлены документы должностным лицам органа Роспотребнадзора, осуществляющего СКК]";
			paramMark2.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";

			report.Controls.Add(StyleSubTitle(ToHTML(Paragraph(Text("3. Досмотр грузов")))));

			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр грузов осуществлялся #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации " +
				"и проводился в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				);
			text = ReplaceSubjects(text, 0);
			text = ReplacePRN(text);
			text = ReplacePPMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по субъектам
			text = Text(
				SKKHelper.AddParagraph() +
				"При ввозе в Россию было #MARK_VIEW_NOMI# #MARK_COUNT#__#MARK_GOODS_NOMI# грузов и товаров, на которые предъявлялись документы " +
				"должностным лицам Роспотребнадзора, осуществляющим санитарно-карантинный контроль.<br />"+
				SKKHelper.AddParagraph() +
				"Наибольшее число партий грузов досмотрено на территории #SUBJ_ITEMS# (таблица__#TABLE_NEXT#).<br />"
				);
			text = ReplaceMark(text);
			text = ReplaceSubjects(text, 6);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по субъектам, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число досмотренных грузов товаров при ввозе в Российскую Федерацию<br />" +
				"в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongBegin));
			div.Controls.Add(AddGridMark(typeof(GridHelpMarkGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ, по ФО
			text = Text(
				SKKHelper.AddParagraph() +
				"Наибольшее число партий грузов досмотрено в #DISTR_ITEMS# федеральных округах (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceDistricts(text, 3, "в общем количестве досмотренных партий грузов на российском участке внешней границы Таможенного союза");
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по ФО, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа партий грузов, досмотренных<br />"+
 				"на российском участке внешней границы Таможенного союза,<br />"+
				"по федеральным округам Российской Федерации<br />(ввоз в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartDistricts(typeof(ChartHelpDistrictsGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ, по виду сообщения
			text = Text(
				SKKHelper.AddParagraph() +
				"Большинство партий грузов и товаров досмотрено в #TRANSP_MAX1_PREP#__пунктах пропуска__— #TRANSP_MAX1_PRC# " +
				"от общего числа партий грузов и товаров, досмотренных в #PERIOD_PREPOSITIONAL# в целом " +
				"на российском участке внешней границы Таможенного союза (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceTransport(text, String.Empty);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по виду сообщения, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа партий грузов, досмотренных<br />на российском участке " +
				"внешней границы Таможенного союза,<br />по видам международного сообщения " +
				"в пунктах пропуска<br />(ввоз в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartTransport(typeof(ChartHelpTransportGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"На #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы России в общей сложности " +
				"#BORDERS_MARK_VIEW_NOMI# #BORDERS_MARK_COUNT#__#BORDERS_MARK_GOODS_NOMI# грузов и товаров. " +
				"Наибольшее значение этого показателя зарегистрировано " +
				"#BORDERS_ITEMS_PRC# Государственной границы РФ (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 4, "в общем количестве партий грузов и товаров, досмотренных на всех участках");
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа партий грузов, досмотренных<br />на российском участке " +
				"внешней границы Таможенного союза,<br />по участкам Государственной границы Российской Федерации<br />" +
				"(ввоз в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ (группы), по участкам границы
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			text = Text(
				SKKHelper.AddParagraph() +
				"Подконтрольные грузы и товары, относящиеся к 1-11__группам Раздела__II Единого перечня товаров, досматривались " +
				"на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы, #BORDERS_MARK_VIEW_NOMI# #BORDERS_MARK_COUNT#__#BORDERS_MARK_GOODS_NOMI# грузов и товаров. " +
				"Наибольшее значение этого показателя зарегистрировано #BORDERS_ITEMS_PRC# участках Государственной границы РФ (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы), по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение партий грузов 1-11 групп товаров (в ед.),<br />" +
				"досмотренных на российском участке внешней границы Таможенного союза,<br />" +
				"по участкам Государственной границы Российской Федерации<br />(ввоз в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ (группы) тонны
			markSI = "тонн";
			markFormat = "N3";
			paramMark.Value = "[Показатели].[Показатель].[Объем партий грузов, подлежащих СКК]";

			// ГРУЗЫ (группы) тонны, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"На #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы было досмотрено #BORDERS_MARK_COUNT#__тонн грузов, " +
				"относящихся к 1-11__группам товаров Раздела__II Единого перечня товаров. Наибольшее значение этого показателя " +
				"зарегистрировано #BORDERS_ITEMS_PRC# участках Государственной границы РФ (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы) тонны, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение объема партий грузов 1-11 групп товаров (в тоннах),<br />" +
				"досмотренных на российском участке внешней границы Таможенного союза,<br />по участкам " +
				"Государственной границы Российской Федерации<br />(ввоз в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersGoodsTon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ (группы) по группам товаров
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			paramMark2.Value = "[Показатели].[Показатель].[Объем партий грузов, подлежащих СКК]";
			text = Text(
				SKKHelper.AddParagraph() +
				"Подконтрольные грузы и товары, относящиеся к 1-11__группам Раздела__II Единого перечня товаров, " +
				"досматривались #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации. " +
				"Сведения о доле подконтрольных партий грузов и товаров, в общем количестве досмотренных партий грузов, " +
				"так же отражены в таблице__#TABLE_PREV# (см. выше).<br />" +
				SKKHelper.AddParagraph() +
				"За #PERIOD_NOMINATIVE# всего #MARK_VIEW_NOMI# #GROUPS_TOTAL_COUNT#__#MARK_GOODS_NOMI# грузов и товаров, " +
				"относящихся к 1-11__группам товаров Раздела__II Единого перечня товаров, общим объемом #GROUPS_TOTAL_VOLUME#__тонн (таблица__#TABLE_NEXT#).<br />"
			    );			
			text = ReplacePRN(text);
			text = ReplaceSubjects(text, 0);			
			text = ReplaceMark(text);
			text = ReplaceGroups(text, 0);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы) по группам товаров, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Общее число досмотренных партий и общие объемы досмотренных и запрещенных<br />к ввозу в РФ " +
				"грузов 1-11 групп товаров в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsBegin));
			div.Controls.Add(AddGridGroups(typeof(GridHelpGroupsInCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ (группы) по группам товаров
			text = Text(
				SKKHelper.AddParagraph() +
				"Чаще всего досматривались грузы, товары #GROUPS_ITEMS# (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceGroups(text, 3);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы) по группам товаров, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число и объемы досмотренных партий товаров<br />1-11 групп Раздела__II Единого перечня товаров<br />" +
				"(ввоз в РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsBegin));
			div.Controls.Add(AddChartGroups(typeof(ChartHelpGroups)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ (группы), приостановлено
			paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных партий грузов]";
			paramMark2.Value = "[Показатели].[Показатель].[Объем приостановленных партий грузов]";
			text = Text(
				SKKHelper.AddParagraph() +
				"Число партий подконтрольных грузов и товаров, ввоз которых был запрещен, в целом на российском участке " +
				"внешней границы Таможенного союза составило #GROUPS_TOTAL_COUNT#__ед., " +
				"общим объемом #GROUPS_TOTAL_VOLUME#__тонн (таблица__#TABLE_PREV#).<br />"+
				
				SKKHelper.AddParagraph() +
				"Ввоз грузов и товаров запрещался на территории #SUBJ_COUNT#__#SUBJ_GENI# РФ, в #PP_COUNT#__#PP_PREP# пропуска" +
				"$#PP_COUNT#?, из которых #PP_TRANSP_ITEMS_LINE_NOMI# $ (таблица__#TABLE_NEXT#).<br />" +
				"$#PP_COUNT#?" + SKKHelper.AddParagraph() + "#TRANSP_ITEMS_PREP#.<br />$"
				);
			text = ReplaceGroups(text, 0);
			text = ReplaceSubjects(text, 0);
			text = ReplacePPMark(text);
			text = ReplaceTransport(text, "пунктах пропуска #MARK_DENY_NOMI# к ввозу #MARK_COUNT#__#MARK_GOODS_NOMI# грузов");
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы), приостановлено, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число партий подконтрольных грузов, запрещенных к ввозу в РФ<br />" +
				"в #PERIOD_PREPOSITIONAL#"
			    ))));
			div.Controls.Add(AddGridBorders(typeof(GridHelpBordersCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			#endregion
			
			// ВЫВОЗ
			
			paramWhere.Value = "[Направления].[Направление].[Все направления].[ Отбытие из РФ]";

			div.Controls.Add(AddServiceRecipe(ServiceRecipe.NewPage));
			report.Controls.Add(StyleMainTitle(ToHTML(Text("ПРИ ОТБЫТИИ ИЗ РОССИИ"))));
			report.Controls.Add(StyleSimple(ToHTML(Paragraph(ReplacePPGeneric(Text(
				"В #PERIOD_PREPOSITIONAL#, санитарно-карантинный контроль транспортных средств, пассажиров и грузов при их отбытии " +
				"из Российской Федерации осуществлялся в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				))))));

			#region ВЫВОЗ, ТС
			
			markSI = "ед.";
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Количество досмотренных транспортных средств]";
			paramMark2.Value = "[Показатели].[Показатель].[Количество приостановленных транспортных средств]";
			
			report.Controls.Add(StyleSubTitle(ToHTML(Paragraph(Text("1. Досмотр транспортных средств")))));

			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр транспортных средств при отбытии из России осуществлялся #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора " +
				"на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации, " +
				"в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				);
			text = ReplaceSubjects(text, 0);
			text = ReplacePRN(text);
			text = ReplacePPMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по субъектам
			text = Text(
				SKKHelper.AddParagraph() +
				"Во всех пунктах пропуска в #PERIOD_PREPOSITIONAL# было досмотрено при отбытии из России " +
				"#MARK_COUNT#__#MARK_TRANSP1_NOMI# #MARK_TRANSP2_NOMI#.<br />" +
				SKKHelper.AddParagraph() +
				"Наибольшее число транспортных средств досматривалось на территории #SUBJ_ITEMS#.<br />" +
				SKKHelper.AddParagraph() +
				"В таблице__#TABLE_NEXT# приведены сведения в разрезе субъектов Российской Федерации, " +
				"на территории которых досматривались транспортные средства при отбытии из России."
				);
			text = ReplaceMark(text);
			text = ReplaceSubjects(text, 4);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по субъектам, таблица			
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Досмотр транспортных средств<br />при отбытии из Российской Федерации в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongBegin));
			div.Controls.Add(AddGridMark(typeof(GridHelpMarkTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, по ФО
			text = Text(
				SKKHelper.AddParagraph() +
				"В разрезе федеральных округов наибольшее количество досмотренных транспортных средств наблюдается в " +
				"#DISTR_ITEMS# федеральных округах (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceDistricts(text, 4, "от общего количества досмотренных транспортных средств в целом по Российской Федерации");
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ТС, по ФО, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа транспортных средств,<br />досмотренных на российском участке внешней границы " +
				"Таможенного союза,<br />по федеральным округам Российской Федерации<br />(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartDistricts(typeof(ChartHelpDistrictsTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, по виду сообщения
			text = Text(
				SKKHelper.AddParagraph() +
				"Наибольшее число транспортных средств досмотрено в #TRANSP_MAX1_PREP# пунктах пропуска__— #TRANSP_MAX1_PRC# " +
				"от общего количества транспортных средств, досмотренных в целом на российском участке внешней границы Таможенного союза. " +
				"На долю #TRANSP_OTHER_PREP# пунктов пропуска приходится в общей сложности #TRANSP_OTHER_PRC# (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceTransport(text, String.Empty);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по виду сообщения, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа транспортных средств,<br />досмотренных на российском участке внешней границы Таможенного союза,<br />"+
				"по видам международного сообщения в пунктах пропуска<br />(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartTransport(typeof(ChartHelpTransportTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр транспортных средств осуществлялся на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы России, где в общей сложности " +
				"было досмотрено #BORDERS_MARK_COUNT#__#BORDERS_MARK_TRANSP1_NOMI# #BORDERS_MARK_TRANSP2_NOMI#, при этом наибольшее количество__— " +
				"#BORDERS_ITEMS_PRC# участках Государственной границы РФ (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 3, "в общем количестве транспортных средств, досмотренных на всех участках");
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ТС, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа транспортных средств,<br />досмотренных на российском участке внешней границы Таможенного союза,<br />"+
				"по участкам Государственной границы Российской Федерации<br />(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersTransport)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ТС, приостановлено
			paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных транспортных средств]";

			text = Text(
				SKKHelper.AddParagraph() + 
				"В #PERIOD_PREPOSITIONAL# был приостановлен (временно запрещен) выезд #MARK_COUNT#__#MARK_TRANSP1_GENI# #MARK_TRANSP2_GENI#. " +
				"Пропуск приостанавливался в #SUBJ_COUNT#__#SUBJ_PREP# Российской Федерации, в #PP_COUNT#__#PP_PREP# пропуска"+
				"$#PP_COUNT#?, из которых #PP_TRANSP_ITEMS_LINE_NOMI#$.<br />" +
				"$#PP_COUNT#?" + SKKHelper.AddParagraph() + "#TRANSP_ITEMS_PREP#.<br />$"+

				SKKHelper.AddParagraph() +
				"Наибольшее число транспортных средств, выезд которых был приостановлен, " +
				"зафиксировано #BORDERS_ITEMS#__#BORDERS_PREP# Государственной границы Российской Федерации (таблица__#TABLE_NEXT#).<br />"
				);
			text = ReplaceTransport(text, "пунктах пропуска приостановлен выезд #MARK_COUNT#__#MARK_TRANSP1_GENI# #MARK_TRANSP2_GENI#");
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceSubjects(text, 0);
			text = ReplacePPMark(text);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ТС, приостановлено, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Перечень пунктов пропуска, в которых приостанавливался<br />"+
				"(временно запрещался) выезд транспортных средств<br />(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddGridBorders(typeof(GridHelpBordersCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			#endregion

			#region ВЫВОЗ, ЛИЦА
			
			markSI = "человек";
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Число лиц, досмотренных на наличие признаков инфекционных заболеваний]";
			paramMark2.Value = "[Показатели].[Показатель].[Выявлено больных и/или лиц с подозрением на инфекционные заболевания]";
			
			report.Controls.Add(StyleSubTitle(ToHTML(Paragraph(Text("2. Досмотр лиц на наличие признаков инфекционных заболеваний")))));

			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр лиц на наличие признаков инфекционных заболеваний при отбытии из России " +
				"осуществлялся #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации " +
				"и проводился в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				);
			text = ReplaceSubjects(text, 0);
			text = ReplacePRN(text);
			text = ReplacePPMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, по субъектам
			text = Text(
				SKKHelper.AddParagraph() +
				"В целом по Российской Федерации на наличие признаков инфекционных заболеваний было досмотрено " +
				"при отбытии из Российской Федерации #MARK_COUNT#__#MARK_PEOPLE_NOMI#.<br />" +
				SKKHelper.AddParagraph() +
				"Наибольшее число лиц было досмотрено на территории #SUBJ_ITEMS#.<br />" +
				SKKHelper.AddParagraph() +
				"В таблице__#TABLE_NEXT# приведены сведения в разрезе субъектов РФ, о числе досмотренных лиц " +
				"на наличие признаков инфекционных заболеваний."
				);
			text = ReplaceMark(text);
			text = ReplaceSubjects(text, 6);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ЛИЦА, по субъектам, таблица

			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число досмотренных лиц на наличие признаков инфекционных заболеваний<br />"+
				"при отбытии из Российской Федерации в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongBegin));
			div.Controls.Add(AddGridMark(typeof(GridHelpMarkPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ЛИЦА, по ФО
			text = Text(
				SKKHelper.AddParagraph() +
				"При отбытии из России наибольшее число лиц досмотрено в #DISTR_ITEMS# федеральных округах (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceDistricts(text, 4, "от общего числа лиц, досмотренных на российском участке внешней границы Таможенного союза");
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ЛИЦА, по ФО, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа лиц, досмотренных на наличие признаков инфекционных<br />заболеваний, "+
				"по федеральным округам Российской Федерации<br />(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartDistricts(typeof(ChartHelpDistrictsPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ЛИЦА, по виду сообщения
			text = Text(
				SKKHelper.AddParagraph() +
				"В #TRANSP_MAX1_PREP# пунктах пропуска досмотрено #TRANSP_MAX1_PRC# от общего числа лиц, "+
				"досмотренных на наличие признаков инфекционных заболеваний на российском участке внешней границы Таможенного союза. "+
				"На долю #TRANSP_MAX2_PREP# пунктов пропуска приходится #TRANSP_MAX2_PRC#, #TRANSP_MAX3_PREP#__— " +
				"#TRANSP_MAX3_PRC# (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceTransport(text, String.Empty);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, по виду сообщения, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа лиц, досмотренных на наличие признаков инфекционных<br />заболеваний " +
				"на российском участке внешней границы Таможенного союза,<br />по видам международного сообщения "+
				"в пунктах пропуска<br />(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartTransport(typeof(ChartHelpTransportPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ЛИЦА, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр лиц проводился на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы Российской Федерации, где за #PERIOD_NOMINATIVE# "+
				"на наличие признаков инфекционных заболеваний досмотрено #BORDERS_MARK_COUNT#__#BORDERS_MARK_PEOPLE_NOMI#. Наибольшее количество__— "+
				"#BORDERS_ITEMS_PRC# участках Государственной границы РФ (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 2, "в общем количестве лиц, досмотренных на всех участках");
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа лиц, досмотренных на наличие признаков инфекционных<br />"+
				"заболеваний на российском участке внешней границы Таможенного союза,<br />"+
				"по участкам Государственной границы Российской Федерации<br />(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersPeople)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ЛИЦА, приостановлено
			paramMark.Value = "[Показатели].[Показатель].[Выявлено больных и/или лиц с подозрением на инфекционные заболевания]";

			text = Text(
				SKKHelper.AddParagraph() +
				"В #PERIOD_PREPOSITIONAL# при отбытии из Российской Федерации #MARK_DETECTED_NOMI# #MARK_COUNT#__" +
				"#MARK_PEOPLE_ILL_NOMI# и/или #MARK_PEOPLE_FACE_NOMI# с подозрением на инфекционные заболевания.<br />" +
				
				SKKHelper.AddParagraph() + 
				"Больные/подозрительные на инфекционные заболевания выявлялись в #PP_COUNT#__#PP_PREP# пропуска" +
				"$#PP_COUNT#?, из которых #PP_TRANSP_ITEMS_LINE_NOMI#$.<br />" +
				"$#PP_COUNT#?" + SKKHelper.AddParagraph() + "#TRANSP_ITEMS_PREP#.<br />$" +
				
				SKKHelper.AddParagraph() +
				"Больные/подозрительные на инфекционные заболевания выявлены на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы, " +
				"на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации. Наибольшее число больных/подозрительных на " +
				"инфекционные заболевания выявлено #BORDERS_ITEMS#__#BORDERS_PREP# Государственной границы (таблица__#TABLE_NEXT#).<br />"
				);
			text = ReplaceTransport(text, "пунктах пропуска #MARK_DETECTED_NOMI# #MARK_COUNT#__" +
			                              "#MARK_PEOPLE_ILL_NOMI# и/или #MARK_PEOPLE_FACE_NOMI# с подозрением на инфекционные заболевания");
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceSubjects(text, 0);
			text = ReplacePPMark(text);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ЛИЦА, приостановлено, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число выявленных больных/лиц с подозрением на инфекционные заболевания<br />" +
				"(отбытие из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddGridBorders(typeof(GridHelpBordersCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			#endregion
			
			#region ВЫВОЗ, ГРУЗЫ

			markSI = "партия";
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, на которые предъявлены документы должностным лицам органа Роспотребнадзора, осуществляющего СКК]";
			paramMark2.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";

			report.Controls.Add(StyleSubTitle(ToHTML(Paragraph(Text("3. Досмотр грузов")))));

			text = Text(
				SKKHelper.AddParagraph() +
				"Досмотр грузов при вывозе из России осуществлялся #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора " +
				"на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации и проводился " +
				"в #PP_COUNT#__#PP_PREP# пропуска:<br />#PP_TRANSP_ITEMS_BR_PREP#.<br />"
				);
			text = ReplaceSubjects(text, 0);
			text = ReplacePRN(text);
			text = ReplacePPMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по субъектам
			text = Text(
				SKKHelper.AddParagraph() +
				"При вывозе из России было #MARK_VIEW_NOMI# #MARK_COUNT#__#MARK_GOODS_NOMI# грузов и товаров, на которые предъявлялись " +
				"документы должностным лицам Роспотребнадзора, осуществляющим санитарно-карантинный контроль.<br />"+
				SKKHelper.AddParagraph() +
				"Наибольшее число партий грузов досмотрено на территории #SUBJ_ITEMS# (таблица__#TABLE_NEXT#).<br />"
				);
			text = ReplaceMark(text);
			text = ReplaceSubjects(text, 6);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по субъектам, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число досмотренных грузов товаров при вывозе из Российской Федерации<br />" +
				"в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongBegin));
			div.Controls.Add(AddGridMark(typeof(GridHelpMarkGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridLongEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ, по ФО
			text = Text(
				SKKHelper.AddParagraph() +
				"Наибольшее число партий грузов досмотрено в #DISTR_ITEMS# федеральных округах (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceDistricts(text, 3, "в общем количестве досмотренных партий грузов на российском участке внешней границы Таможенного союза");
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по ФО, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа партий грузов, досмотренных<br />"+
 				"на российском участке внешней границы Таможенного союза,<br />"+
				"по федеральным округам Российской Федерации<br />(вывоз из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartDistricts(typeof(ChartHelpDistrictsGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ, по виду сообщения
			text = Text(
				SKKHelper.AddParagraph() +
				"Большинство партий грузов и товаров досмотрено в #TRANSP_MAX1_PREP# пунктах пропуска__— #TRANSP_MAX1_PRC# " +
				"от общего числа партий грузов и товаров, досмотренных в #PERIOD_PREPOSITIONAL# в целом " +
				"на российском участке внешней границы Таможенного союза (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceTransport(text, String.Empty);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по виду сообщения, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа партий грузов, досмотренных<br />на российском участке " +
				"внешней границы Таможенного союза,<br />по видам международного сообщения " +
				"в пунктах пропуска<br />(вывоз из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartTransport(typeof(ChartHelpTransportGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"На #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы России в общей сложности " +
				"#BORDERS_MARK_VIEW_NOMI# #BORDERS_MARK_COUNT#__#BORDERS_MARK_GOODS_NOMI# грузов и товаров. " +
				"Наибольшее значение этого показателя зарегистрировано #BORDERS_ITEMS_PRC# участках Государственной границы РФ " +
				"(диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 4, "в общем количестве партий грузов и товаров, досмотренных на всех участках");
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение общего числа партий грузов, досмотренных<br />на российском участке " +
				"внешней границы Таможенного союза,<br />по участкам Государственной границы Российской Федерации<br />" +
				"(вывоз из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ (группы), по участкам границы
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			text = Text(
				SKKHelper.AddParagraph() +
				"Подконтрольные грузы и товары, относящиеся к 1-11__группам Раздела__II Единого перечня товаров, " +
				"досматривались на #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы, " +
				"#BORDERS_MARK_VIEW_NOMI# #BORDERS_MARK_COUNT#__#BORDERS_MARK_GOODS_NOMI# грузов и товаров" +
				"$#BORDERS_COUNT#?. Наибольшее значение этого показателя зарегистрировано " +
				"#BORDERS_ITEMS_PRC# участках Государственной границы РФ $(диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы), по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение партий грузов 1-11 групп товаров (в ед.),<br />" +
				"досмотренных на российском участке внешней границы Таможенного союза,<br />" +
				"по участкам Государственной границы Российской Федерации<br />(вывоз из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersGoods)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ГРУЗЫ (группы) тонны
			markSI = "тонн";
			markFormat = "N3";
			paramMark.Value = "[Показатели].[Показатель].[Объем партий грузов, подлежащих СКК]";
			
			// ГРУЗЫ (группы) тонны, по участкам границы
			text = Text(
				SKKHelper.AddParagraph() +
				"На #BORDERS_COUNT#__#BORDERS_PREP# Государственной границы было досмотрено #BORDERS_MARK_COUNT#__тонн грузов, " +
				"относящихся к 1-11__группам товаров Раздела__II Единого перечня товаров" +
				"$#BORDERS_COUNT#?. Наибольшее значение этого показателя зарегистрировано " +
				"#BORDERS_ITEMS_PRC# участках Государственной границы РФ $(диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceBorders(text, 2, String.Empty);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы) тонны, по участкам границы, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Распределение объема партий грузов 1-11 групп товаров (в тоннах),<br />" +
				"досмотренных на российском участке внешней границы Таможенного союза,<br />по участкам " +
				"Государственной границы Российской Федерации<br />(вывоз из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddChartBorders(typeof(ChartHelpBordersGoodsTon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ГРУЗЫ (группы) по группам товаров
			markFormat = "N0";
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			paramMark2.Value = "[Показатели].[Показатель].[Объем партий грузов, подлежащих СКК]";
			text = Text(
				SKKHelper.AddParagraph() +
				"Подконтрольные грузы и товары, относящиеся к 1-11__группам Раздела__II Единого перечня товаров, " +
				"досматривались #RPN_COUNT#__#RPN_ABLA# Роспотребнадзора на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации. " +
				"Сведения о доле подконтрольных партий грузов и товаров, в общем количестве досмотренных партий грузов, " +
				"так же отражены в таблице__#TABLE_PREV# (см. выше).<br />" +
				SKKHelper.AddParagraph() +
				"За #PERIOD_NOMINATIVE# всего #MARK_VIEW_NOMI# #GROUPS_TOTAL_COUNT#__#MARK_GOODS_NOMI# грузов и товаров, " +
				"относящихся к 1-11__группам товаров Раздела__II Единого перечня товаров, общим объемом " +
				"#GROUPS_TOTAL_VOLUME#__тонн (таблица__#TABLE_NEXT#).<br />"
				);
			text = ReplacePRN(text);
			text = ReplaceSubjects(text, 0);
			text = ReplaceMark(text);
			text = ReplaceGroups(text, 0);
			report.Controls.Add(StyleSimple(ToHTML(text)));
			
			// ГРУЗЫ (группы) по группам товаров, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Общее число досмотренных партий и общие объемы досмотренных и запрещенных<br />к вывозу из РФ " +
				"грузов 1-11 групп товаров в #PERIOD_PREPOSITIONAL#"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsBegin));
			div.Controls.Add(AddGridGroups(typeof(GridHelpGroupsOutCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			// ГРУЗЫ (группы) по группам товаров
			text = Text(
				SKKHelper.AddParagraph() +
				"Чаще всего досматривались грузы, товары #GROUPS_ITEMS# (диаграмма__#CHART_NEXT#).<br />"
				);
			text = ReplaceGroups(text, 3);
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы) по группам товаров, диаграмма
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма__#CHART_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число и объемы досмотренных партий товаров<br />1-11 групп Раздела__II Единого перечня товаров<br />" +
				"(вывоз из РФ, #PERIOD_NOMINATIVE#)"
				))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsBegin));
			div.Controls.Add(AddChartGroups(typeof(ChartHelpGroups)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsEnd));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

			// ГРУЗЫ (группы), приостановлено
			paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных партий грузов]";
			paramMark2.Value = "[Показатели].[Показатель].[Объем приостановленных партий грузов]";
			text = Text(
				SKKHelper.AddParagraph() +
				"Число партий подконтрольных грузов и товаров, вывоз которых был запрещен, в целом на российском участке " +
				"внешней границы Таможенного союза составило #GROUPS_TOTAL_COUNT#__ед., " +
				"общим объемом #GROUPS_TOTAL_VOLUME#__тонн (таблица__#TABLE_PREV#).<br />"+
				SKKHelper.AddParagraph() +
				"Вывоз грузов и товаров запрещался на территории #SUBJ_COUNT#__#SUBJ_GENI# РФ, в #PP_COUNT#__#PP_PREP# пропуска" +
				"$#PP_COUNT#?, из которых #PP_TRANSP_ITEMS_LINE_NOMI# $ (таблица__#TABLE_NEXT#).<br />" +
				"$#PP_COUNT#?" + SKKHelper.AddParagraph() + "#TRANSP_ITEMS_PREP#.<br />$"
				);
			text = ReplaceGroups(text, 0);
			text = ReplaceSubjects(text, 0);
			text = ReplacePPMark(text);
			text = ReplaceTransport(text, "пунктах пропуска #MARK_DENY_NOMI# к вывозу #MARK_COUNT#__#MARK_GOODS_NOMI# грузов");
			report.Controls.Add(StyleSimple(ToHTML(text)));

			// ГРУЗЫ (группы), приостановлено, таблица
			report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
			div.Controls.Add(StyleRight(ToHTML(Text("Таблица__#TABLE_NEXT#"))));
			div.Controls.Add(StyleItemTitle(ToHTML(Text(
				"Число партий подконтрольных грузов, запрещенных к вывозу из РФ<br />" +
				"в #PERIOD_PREPOSITIONAL#"
			    ))));
			div.Controls.Add(AddGridBorders(typeof(GridHelpBordersCommon)));
			div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));
			
			#endregion
			
		}

		#region методы стилизации

		private HtmlGenericControl ToHTML(StringBuilder text)
		{
			HtmlGenericControl item = new HtmlGenericControl("div");
			item.InnerHtml = ReplaceGeneral(text.ToString());
			return item;
		}

		private static HtmlGenericControl StyleSimple(HtmlGenericControl html)
		{
			html.Style.Add("margin", "5px 0 0 0"); 
			html.Style.Add("text-align", "justify");
			return html;
		}

		private static HtmlGenericControl StyleRight(HtmlGenericControl html)
		{
			html.Style.Add("text-align", "right");
			return html;
		}

		private static HtmlGenericControl StyleCenter(HtmlGenericControl html)
		{
			html.Style.Add("text-align", "center");
			return html;
		}

		private static HtmlGenericControl StyleMainTitle(HtmlGenericControl html)
		{
			html.Style.Add("margin", "10px 0");
			html.Style.Add("text-align", "center");
			html.Style.Add("font-size", "12pt");
			html.Style.Add("font-weight", "bold");
			return html;
		}

		private static HtmlGenericControl StyleSubTitle(HtmlGenericControl html)
		{
			html.Style.Add("margin", "5px 0");
			html.Style.Add("font-size", "10pt");
			html.Style.Add("font-weight", "bold");
			return html;
		}

		private static HtmlGenericControl StyleItemTitle(HtmlGenericControl html)
		{
			html.Style.Add("margin", "5px 0");
			html.Style.Add("font-size", "10pt");
			html.Style.Add("font-weight", "bold");
			return html;
		}

		private static HtmlGenericControl StyleItem(HtmlGenericControl html)
		{
			html.Style.Add("margin", "10px 0");
			html.Style.Add("display", "inline-block");
			html.Style.Add("zoom", "1");
			html.Style.Add("*display", "inline");
			return html;
		}

		#endregion

		#region добавление таблиц и диаграмм

		/// <summary>
		/// создать таблицу
		/// </summary>
		private HtmlGenericControl AddGridBase(Type gridHelperClass, string queryName)
		{
			countTables++;
			GridHelpBase gridHelper = (GridHelpBase)Activator.CreateInstance(gridHelperClass);
			gridHelper.ParamOneGrid = export;
			gridHelper.Init(Page, queryName);
			
			return gridHelper.GetItem();
		}

		/// <summary>
		/// создать таблицу с показателем (досмотрено/приостановлено)
		/// </summary>
		private HtmlGenericControl AddGridMark(Type gridHelperClass)
		{
			return AddGridBase(gridHelperClass, "skk_017_mark_subjects");
		}

		/// <summary>
		/// создать таблицу по участкам границы
		/// </summary>
		private HtmlGenericControl AddGridBorders(Type gridHelperClass)
		{
			return AddGridBase(gridHelperClass, "skk_017_mark_borders");
		}
		
		/// <summary>
		/// создать таблицу с показателем (досмотрено/приостановлено)
		/// </summary>
		private HtmlGenericControl AddGridGroups(Type gridHelperClass)
		{
			return AddGridBase(gridHelperClass, "skk_017_groups_grid");
		}

		/// <summary>
		/// создать диаграмму
		/// </summary> 
		private HtmlGenericControl AddChart(Type chartHelperClass, string queryName)
		{
			countCharts++;
			ChartHelpBase chartHelper = (ChartHelpBase)Activator.CreateInstance(chartHelperClass);
			chartHelper.Init(1700 + countCharts, queryName);
			return chartHelper.GetItem();
		}

		/// <summary>
		/// создать диаграмму по ФО
		/// </summary>
		private HtmlGenericControl AddChartDistricts(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_017_count_districts");
		}

		/// <summary>
		/// создать диаграмму по видам сообщения
		/// </summary>
		private HtmlGenericControl AddChartTransport(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_017_count_transport");
		}

		/// <summary>
		/// создать диаграмму по участкам границы
		/// </summary>
		private HtmlGenericControl AddChartBorders(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_017_count_borders");
		}
		
		/// <summary>
		/// создать диаграмму по группам товаров
		/// </summary>
		private HtmlGenericControl AddChartGroups(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_017_count_groups");
		}

		#endregion

		#region добавление служебного контента

		/// <summary>
		/// создать диаграмму по группам товаров
		/// </summary>
		private HtmlGenericControl AddServiceRecipe(ServiceRecipe note)
		{
			HtmlGenericControl item = new HtmlGenericControl("div");
			item.Style.Add("display", "none");
			item.InnerText = note.ToString();
			return item;
		}

		#endregion

		#region добавление текстов

		private static StringBuilder Text(string text)
		{
			return new StringBuilder(text);
		}

		private static StringBuilder Paragraph(StringBuilder text)
		{
			return text.Insert(0, SKKHelper.AddParagraph());
		}

		#endregion

		#region подстановка текстов

		/// <summary>
		/// кол-во ПП по виду сообщения
		/// </summary>
		private StringBuilder ReplacePPBase(StringBuilder text, string queryName)
		{
			DataTable dTable = new Query(queryName).GetDataTable();
			int totalPP = 0;

			if (dTable.Rows.Count > 0)
			{
				totalPP = Convert.ToInt32(dTable.Rows[0][1]);
				dTable.Rows.RemoveAt(0);
				text = text.
					Replace("#PP_TRANSP_ITEMS_BR_PREP#", GetList(dTable, ListFormatDictManyBR, -1, new[] { prepMany })).
					Replace("#PP_TRANSP_ITEMS_LINE_NOMI#", GetList(dTable, ListFormatDictMany, -1, new[] { nomiMany }));
			}

			return text.
				Replace("#PP_COUNT#", totalPP.ToString("N0")).
				Replace("#PP_PREP#", MultiNumEnding(prepMany, totalPP, "пункт")).
				Replace("#PP_NOMI#", MultiNumEnding(nomiMany, totalPP, "пункт"));
			
		}

		/// <summary>
		/// кол-во ПП по виду сообщения (общая - шапка, ввоз, вывоз)
		/// </summary>
		private StringBuilder ReplacePPGeneric(StringBuilder text)
		{
			return
				ReplacePPBase(text, "skk_017_count_pp_common");
		}

		/// <summary>
		/// кол-во ПП по виду сообщения (общая - показатели)
		/// </summary>
		private StringBuilder ReplacePPMark(StringBuilder text)
		{
			return
				ReplacePPBase(text, "skk_017_count_pp_simple");
		}

		/// <summary>
		/// кол-во управлений РПН
		/// </summary>
		private StringBuilder ReplacePRN(StringBuilder text)
		{
			DataTable tableRPN = new Query("skk_017_count_rpn").GetDataTable();
			int totalRpn = 0;

			if (tableRPN.Rows.Count > 0)
			{
				totalRpn = Convert.ToInt32(tableRPN.Rows[0][0]);
			}

			return text.
				Replace("#RPN_COUNT#", totalRpn.ToString("N0")).
				Replace("#RPN_ABLA#", MultiNumEnding(ablaMany, totalRpn, "управление"));
		}

		/// <summary>
		/// показатель
		/// </summary>
		private StringBuilder ReplaceMark(StringBuilder text)
		{
			DataTable table = new Query("skk_017_count_mark").GetDataTable();
			double totalMarkRAW = 0;
			int totalMark = 0;
			if (table.Rows.Count > 0)
			{
				totalMarkRAW = Convert.ToDouble(CRHelper.DBValueConvertToDoubleOrZero(table.Rows[0][0]));
				totalMark = Convert.ToInt32(Math.Round(totalMarkRAW));
			}

			return text.
				Replace("#MARK_COUNT#", totalMarkRAW.ToString(markFormat)).

				Replace("#MARK_TRANSP1_NOMI#", MultiNumEnding(nomiMany, totalMark, "транспортное")).
				Replace("#MARK_TRANSP2_NOMI#", MultiNumEnding(nomiMany, totalMark, "средство")).
				Replace("#MARK_TRANSP1_GENI#", MultiNumEnding(geniMany, totalMark, "транспортное")).
				Replace("#MARK_TRANSP2_GENI#", MultiNumEnding(geniMany, totalMark, "средство")).

				Replace("#MARK_DETECTED_NOMI#", MultiNumEnding(nomiMany, totalMark, "выявлен")).
				Replace("#MARK_PEOPLE_NOMI#", MultiNumEnding(nomiMany, totalMark, "человек")).
				Replace("#MARK_PEOPLE_FACE_NOMI#", MultiNumEnding(nomiMany, totalMark, "лицо")).
				Replace("#MARK_PEOPLE_ILL_NOMI#", MultiNumEnding(nomiMany, totalMark, "больной")).

				Replace("#MARK_VIEW_NOMI#", MultiNumEnding(nomiMany, totalMark, "досмотрена")).
				Replace("#MARK_GOODS_NOMI#", MultiNumEnding(nomiMany, totalMark, "партия"));
		}

		/// <summary>
		/// субъекты
		/// </summary>
		private StringBuilder ReplaceSubjects(StringBuilder text, int count)
		{
			DataTable table = new Query("skk_017_count_subjects").GetDataTable();
			int totalSubjects = 0;

			if (table.Rows.Count > 0)
			{
				totalSubjects = table.Rows.Count - 1;
				table.Rows.RemoveAt(0);
				text = text.
					Replace("#SUBJ_ITEMS#", GetList(table, ListFormatExt, count, new[] {geniOne}));
			}
			return text.
				Replace("#SUBJ_COUNT#", totalSubjects.ToString()).
				Replace("#SUBJ_GENI#", MultiNumEnding(geniMany, totalSubjects, "субъект")).
				Replace("#SUBJ_PREP#", MultiNumEnding(prepMany, totalSubjects, "субъект"));
		}
		
		/// <summary>
		/// федеральные округа
		/// </summary>
		private StringBuilder ReplaceDistricts(StringBuilder text, int count, string extraListText)
		{
			DataTable table = new Query("skk_017_count_districts").GetDataTable();
			int totalDistricts = 0;

			if (table.Rows.Count > 0)
			{
				totalDistricts = table.Rows.Count - 1;
				table.Rows.RemoveAt(0);
				text = text.
					Replace("#DISTR_ITEMS#", GetList(table, ListFormatExtPrc, count, new[] { prepOne, (object)extraListText }));
			}
			return text.
				Replace("#DISTR_COUNT#", totalDistricts.ToString());
		}

		/// <summary>
		/// виды сообщения
		/// </summary>
		private StringBuilder ReplaceTransport(StringBuilder text, string extraListText)
		{ 
			DataTable table = new Query("skk_017_count_transport").GetDataTable();
			
			if (table.Rows.Count > 1)
			{
				string max1Title = table.Rows[1][0].ToString();
				double max1Prc = Convert.ToDouble(table.Rows[1][2]);
				text = text.
					Replace("#TRANSP_MAX1_PREP#", MultiNumEnding(prepMany, COUNT_MANY, max1Title)).
					Replace("#TRANSP_MAX1_PRC#", max1Prc.ToString("P2"));

				if (table.Rows.Count > 2)
				{
					string max2Title = table.Rows[2][0].ToString();
					double max2Prc = Convert.ToDouble(table.Rows[2][2]);
					text = text.
						Replace("#TRANSP_MAX2_PREP#", MultiNumEnding(prepMany, COUNT_MANY, max2Title)).
						Replace("#TRANSP_MAX2_PRC#", max2Prc.ToString("P2"));
				}

				if (table.Rows.Count > 3)
				{
					string max3Title = table.Rows[3][0].ToString();
					double max3Prc = Convert.ToDouble(table.Rows[3][2]);
					text = text.
						Replace("#TRANSP_MAX3_PREP#", MultiNumEnding(prepMany, COUNT_MANY, max3Title)).
						Replace("#TRANSP_MAX3_PRC#", max3Prc.ToString("P2"));
				}

				table.Rows.RemoveAt(0);
				text = text.Replace("#TRANSP_ITEMS_PREP#", CRHelper.ToUpperFirstSymbol(GetList(table, ListFormatDashExt, -1, new[] { prepMany, (object)extraListText })));
				table.Rows.RemoveAt(0);
				text = text.
					Replace("#TRANSP_OTHER_PREP#", GetList(table, ListFormatDictManySimple, -1, new[] { prepMany })).
					Replace("#TRANSP_OTHER_PRC#", (1 - max1Prc).ToString("P2")); 
			}

			return text;
		}

		/// <summary>
		/// участки границы
		/// </summary>
		private StringBuilder ReplaceBorders(StringBuilder text, int count, string extraListText)
		{
			DataTable table = new Query("skk_017_count_borders").GetDataTable();
			double totalMarkRAW = 0;
			int totalMark = 0;
			int totalBorders = 0;
			
			if (table.Rows.Count > 0)
			{
				totalMarkRAW = Convert.ToDouble(table.Rows[0][1]);
				totalMark = Convert.ToInt32(totalMarkRAW);
				totalBorders = table.Rows.Count - 1;

				table.Rows.RemoveAt(0);
				if (table.Rows.Count > 0)
				{
					text = text.
						Replace("#BORDERS_ITEMS#", GetList(table, ListFormatExtSI, count, new[] {prepOne, (object) extraListText})).
						Replace("#BORDERS_ITEMS_PRC#", GetList(table, ListFormatExtPrc, count, new[] {prepOne, (object) extraListText}));
				}
			}
			return text.
				Replace("#BORDERS_COUNT#", totalBorders.ToString("N0")).
				Replace("#BORDERS_PREP#", MultiNumEnding(prepMany, totalBorders, "участок")).
				Replace("#BORDERS_MARK_COUNT#", totalMarkRAW.ToString(markFormat)).
				Replace("#BORDERS_MARK_TRANSP1_NOMI#", MultiNumEnding(nomiMany, totalMark, "транспортное")).
				Replace("#BORDERS_MARK_TRANSP2_NOMI#", MultiNumEnding(nomiMany, totalMark, "средство")).
				Replace("#BORDERS_MARK_PEOPLE_NOMI#", MultiNumEnding(nomiMany, totalMark, "человек")).
				Replace("#BORDERS_MARK_VIEW_NOMI#", MultiNumEnding(nomiMany, totalMark, "досмотрена")).
				Replace("#BORDERS_MARK_GOODS_NOMI#", MultiNumEnding(nomiMany, totalMark, "партия")).
				Replace("#BORDERS_ITEMS#", "на 0").
				Replace("#BORDERS_ITEMS_PRC#", String.Empty);
		}
		
		/// <summary>
		/// по группам товаров
		/// </summary>
		private StringBuilder ReplaceGroups(StringBuilder text, int count)
		{
			DataTable table = new Query("skk_017_count_groups").GetDataTable();
			double totalCount = 0;
			double totalVolume = 0;

			if (table.Rows.Count > 0 
				&& table.Rows[0][1] != DBNull.Value 
				&& table.Rows[0][2] != DBNull.Value)
			{
				totalCount = Convert.ToDouble(table.Rows[0][1]);
				totalVolume = Convert.ToDouble(table.Rows[0][2]);

				table.Rows.RemoveAt(0);
				text = text.
					Replace("#GROUPS_ITEMS#", GetList(table, ListFormatGroups, count, new[] { nomiMany }));
			}
			return text.
				Replace("#GROUPS_TOTAL_COUNT#", totalCount.ToString("N0")).
				Replace("#GROUPS_TOTAL_VOLUME#", totalVolume.ToString("N3"));
		}
		#endregion

		#region генерация списков

		/// <summary>
		/// получить список: {0} {1-dictS[0]}
		/// </summary>
		private string ListFormatDictManyBR(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>) param[0];
			int value = Convert.ToInt32(row[1]);

			return 
				SKKHelper.AddParagraph()+
				GetListItem(
					GetListFormatType(TypeList.InBR, index, count),
					String.Format("{0}__{1}", value.ToString("N0"), MultiNumEnding(dict, value, row[0].ToString()))
				);
		}

		/// <summary>
		/// получить список: {0} {1-dictS[0]}
		/// </summary>
		private string ListFormatDictMany(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>) param[0];
			int value = Convert.ToInt32(row[1]);

			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					String.Format(
						"{0}__{1}", 
						value.ToString("N0"), 
						MultiNumEnding(dict, value, row[0].ToString())
					)
				);
		}

		/// <summary>
		/// получить список: {0-dict[0]} extra*[1]
		/// </summary>
		private string ListFormatDashExt(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>) param[0];
			string extra = (string) param[1];
			int value = Convert.ToInt32(row[1]);

			string item;
			if (index == 0)
			{
				item = String.Format(
					"{0}__{1}",
					MultiNumEnding(dict, COUNT_MANY, row[0].ToString()),
					extra);
				item = item.
					Replace("#MARK_COUNT#", value.ToString(markFormat)).
					Replace("#MARK_TRANSP1_GENI#", MultiNumEnding(geniMany, value, "транспортное")).
					Replace("#MARK_TRANSP2_GENI#", MultiNumEnding(geniMany, value, "средство")).
					Replace("#MARK_DETECTED_NOMI#", MultiNumEnding(nomiMany, value, "выявлен")).
					Replace("#MARK_PEOPLE_NOMI#", MultiNumEnding(nomiMany, value, "человек")).
					Replace("#MARK_PEOPLE_FACE_NOMI#", MultiNumEnding(nomiMany, value, "лицо")).
					Replace("#MARK_PEOPLE_ILL_NOMI#", MultiNumEnding(nomiMany, value, "больной")).
					Replace("#MARK_DENY_NOMI#", MultiNumEnding(nomiMany, value, "запрещена")).
					Replace("#MARK_GOODS_NOMI#", MultiNumEnding(nomiMany, value, "партия"));
			}
			else
			{
				item = String.Format(
					"{0}__— {1}",
					MultiNumEnding(dict, COUNT_MANY, row[0].ToString()),
				    value.ToString(markFormat));
			}

			return
				GetListItem(
					GetListFormatType(TypeList.InLine, index, count),
					item
				);
		}

		/// <summary>
		/// получить список: {0-dict[0]} ({1} si*)
		/// </summary>
		private string ListFormatExt(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string> dict = (Dictionary<string, string>) param[0];
			int value = Convert.ToInt32(row[1]);

			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					String.Format("{0} ({1}{2})", 
						GetFromDict(dict, row[0].ToString()), 
						value.ToString(markFormat), 
						(index == 0 ? "__" + MultiNumEnding(nomiMany, value, markSI) : String.Empty))
				);
		}

		/// <summary>
		/// получить список: {0-dict[0]} ({1} si)
		/// </summary>
		private string ListFormatExtSI(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string> dict = (Dictionary<string, string>)param[0];
			int value = Convert.ToInt32(row[1]);

			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					String.Format("{0} ({1}__{2})",
						GetFromDict(dict, row[0].ToString()),
						value.ToString(markFormat),
						MultiNumEnding(nomiMany, value, markSI))
				);
		}
		
		/// <summary>
		/// получить список: {0-dict[0]} ({1} si - {2%} extra*[1])
		/// </summary>
		private string ListFormatExtPrc(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string> dict = (Dictionary<string, string>) param[0];
			string extraText = (string) param[1];
			double valueRAW = Convert.ToDouble(row[1]);
			int value = Convert.ToInt32(valueRAW);

			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					String.Format("{0} ({1}__{2}__— {3}{4})", 
						GetFromDict(dict, row[0].ToString()),
						valueRAW.ToString(markFormat),
						MultiNumEnding(nomiMany, value, markSI),
						Convert.ToDouble(row[2]).ToString("P2"),
						(index == 0 && !extraText.Equals(String.Empty))
							? " " + extraText 
							: String.Empty)
				);
		}

		/// <summary>
		/// получить список: {0-dictS[0]}
		/// </summary>
		private string ListFormatDictManySimple(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>) param[0];

			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					MultiNumEnding(dict, COUNT_MANY, row[0].ToString())
				);
		}

		/// <summary>
		/// получить список: {3} - {0} {1-dictS[0]} {2} объемом {3} тонн
		/// </summary>
		private string ListFormatGroups(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>)param[0];
			int valueCount = Convert.ToInt32(row[1]);
			double valueVolume = Convert.ToDouble(row[2]);
			string valueExtra = row[3].ToString().Replace("группа", "группы");

			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					String.Format("{0}____— {1}__{2} объемом {3}__тонн",
						valueExtra,
						valueCount.ToString("N0"),
						MultiNumEnding(dict, valueCount, "партия"),
						valueVolume.ToString("N3")
						)
				);
		}
		#endregion

		#region методы нижнего уровня

		/// <summary>
		/// Получить список по таблице и функции формата элемента
		/// </summary>
		private static string GetList(DataTable dTable, ListFormat listFormat, int count, object[] param)
		{
			StringBuilder list = new StringBuilder();
			if (dTable.Rows.Count < count || count == -1)
				count = dTable.Rows.Count;
			for (int i = 0; i < count; i++)
			{
				DataRow row = dTable.Rows[i];
				list.Append(listFormat(row, i, count, param));
			}
			return list.ToString();
		}

		/// <summary>
		/// Возвращает индекс в массиве формата элемента списка
		/// </summary>
		private string GetListFormatType(TypeList typeList, int index, int count)
		{
			if (count == 1) 
				return lists[typeList][TypeListPlaces.One];
			if (index == count-2) 
				return lists[typeList][TypeListPlaces.PreLast];
			if (index == count-1) 
				return lists[typeList][TypeListPlaces.Last];
			if (index == 0)
				return lists[typeList][TypeListPlaces.First];
			return lists[typeList][TypeListPlaces.Other];
		}

		/// <summary>
		/// Форматирование элемента списка
		/// </summary>
		private static string GetListItem(string listFormat, string item)
		{
			return
				listFormat.Replace("#", item);
		}

		/// <summary>
		/// Возвращает слово из словарика
		/// </summary>
		private static string GetFromDict(Dictionary<string, string> dict, string key)
		{
			if (dict.ContainsKey(key))
			{
				return dict[key];
			}
			return key;
		}

		/// <summary>
		/// Возвращает слово из словарика с правильным количественным окончанием
		/// </summary>
		private static string MultiNumEnding(Dictionary<string, string[]> dict, int num, string key)
		{
			string ending;
			string number = num.ToString();
			if (number.Length > 2)
				number = number.Substring(number.Length - 2);
			else if (number.Length == 1)
				number = "0" + number;

			if (!dict.ContainsKey(key))
				return key+"_";

			if (number[1] == '1' && number[0] != '1')
			{
				ending = dict[key][1];
			}
			else if (dict[key].Length < 4 ||
				(number[1] == '2' && number[0] != '1') ||
				(number[1] == '3' && number[0] != '1') ||
				(number[1] == '4' && number[0] != '1'))
			{
				ending = dict[key][2];
			}
			else
			{
				ending = dict[key][3];
			}

			return
				dict[key][0] + ending;
		}

		/// <summary>
		/// Период в именительном падеже
		/// </summary>
		/// <returns></returns>
		private string PeriodNominative()
		{
			return PeriodToString(CRHelper.RusMonth);
		}
		
		/// <summary>
		/// Период в предложном падеже
		/// </summary>
		private string PeriodPrepositional()
		{
			return PeriodToString(CRHelper.RusMonthPrepositional);
		}

		/// <summary>
		/// Базовый метод преобразования периода
		/// </summary>
		private string PeriodToString(MonthConverter convertFunc)
		{
			string str = String.Empty;
			int prev = -1;
			bool period = false;
			foreach (string month in comboPeriodMonth.SelectedValues)
			{
				int mon = CRHelper.MonthNum(month.ToLower());
				if (prev != mon - 1) 
				{
					if (period)
						str += "-" + convertFunc(prev);
					str += ((prev == -1) ? String.Empty : ", ") + convertFunc(mon);
					period = false;
				}
				else
				{
					period = true;
				}
				prev = mon;
			}
			if (period)
				str += "-" + convertFunc(prev);
			str = String.Format("{0} {1}&nbsp;г.", str, comboPeriodYear.SelectedValue);
			return str;
		}

		/// <summary>
		/// Замена основных переменных
		/// </summary>
		private string ReplaceGeneral(string text)
		{
			text = text.
				Replace("__", "&nbsp;").
				Replace("#PERIOD_NOMINATIVE#", periodNominative).
				Replace("#PERIOD_PREPOSITIONAL#", periodPrepositional).
				Replace("#CHART_NEXT#", countCharts.ToString()).
				Replace("#CHART_PREV#", (countCharts - 1).ToString()).
				Replace("#TABLE_NEXT#", countTables.ToString()).
				Replace("#TABLE_PREV#", (countTables - 1).ToString());

			MatchCollection matches = Regex.Matches(text, @"\$([^\?]*)\?([^\$]*)\$");
			foreach (Match match in matches)
			{
				string raw_data = match.Value;
				string raw_condition = match.Groups[1].Value;
				string raw_text = match.Groups[2].Value;
				double ifValue;
				
				Double.TryParse(raw_condition, out ifValue); 
				if ( ifValue <= 0)
				{
					raw_text = String.Empty;
				}
				
				text = text.Replace(raw_data, raw_text);
			}

			return 
				Regex.Replace(text, @"#[^#]*#", "<b style=\"color: red;\">&lt;нет данных&gt;</b>");
		}

		#endregion

		/// <summary>
		/// Типы списков
		/// </summary>
		enum TypeList
		{
			/// <summary>
			/// Простое перечисление: #, #, # и #
			/// </summary>
			Simple, 
			
			/// <summary>
			/// Перечисление "в": в #, в #, в # и в #
			/// </summary>
			InLine, 
			
			/// <summary>
			/// Перечисление "в" с разделением на строки: в #,\n в #,\n в #\n и в #
			/// </summary>
			InBR
		}

		/// <summary>
		/// Типы местоположений внутри списков
		/// </summary>
		enum TypeListPlaces
		{
			/// <summary>
			/// Единственный в списке
			/// </summary>
			One, 
			
			/// <summary>
			/// Первый в списке
			/// </summary>
			First, 
			
			/// <summary>
			/// Типичный в списке
			/// </summary>
			Other, 
			
			/// <summary>
			/// Предпоследний в списке
			/// </summary>
			PreLast, 
			
			/// <summary>
			/// Последний в списке
			/// </summary>
			Last
		}
		
	}
}
