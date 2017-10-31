using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_018
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
		private static MemberAttributesDigest digestBorder;
		private CustomParam selectedBorder;
		private CustomParam selectedPeriod;
		private CustomParam paramDirection;
		private CustomParam paramMark;
		private CustomParam paramMark2;
		private CustomParam paramMarkPart;
		private CustomParam paramMarkTotal;

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
		private Dictionary<string, string[]> accuMany;
		private Dictionary<string, string[]> ablaMany;
		private Dictionary<string, string[]> prepMany;
		private Dictionary<string, string> geniOne;
		private Dictionary<string, string> prepOne;

		// делегаты
		delegate string MonthConverter(int monthNum);
		delegate string ListFormat(DataRow row, int index, int count, object[] param);


		public Default()
		{
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
				
				{"функционировал", 
					new[] {"функционировал", String.Empty, "о", "о"}},
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

			// винительный падеж
			accuMany = new Dictionary<string, string[]> // 1,2-4,5
			{
				{"партия", 
					new[] {"парти", "ю", "и", "й"}}
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

				{"Российско-норвежский", "российско-норвежском"},
				{"Российско-финский", "российско-финском"},
				{"Российско-эстонский", "российско-эстонском"},
				{"Российско-латвийский", "российско-латвийском"},
				{"Российско-литовский", "российско-литовском"},
				{"Российско-польский", "российско-польском"},
				{"Российско-украинский", "российско-украинском"},
				{"Российско-абхазский", "российско-абхазском"},
				{"Российско-южноосетинский", "российско-южноосетинском"},
				{"Российско-грузинский", "российско-грузинском"},
				{"Российско-азербайджанский", "российско-азербайджанском"},
				{"Российско-казахстанский", "российско-казахстанском"},
				{"Российско-монгольский", "российско-монгольском"},
				{"Российско-китайский", "российско-китайском"},
				{"Российско-корейский", "российско-корейском"},
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
				{TypeList.DashBR, new Dictionary<TypeListPlaces, string>
				    {
						{TypeListPlaces.One, "– #"},
						{TypeListPlaces.First, "– #,<br />"},
						{TypeListPlaces.Other, "– #,<br />"},
						{TypeListPlaces.PreLast, "– #,<br />"},
						{TypeListPlaces.Last, "– #"},
				    }
				},
			};

			#endregion

		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedBorder = UserParams.CustomParam("selected_border");
			selectedPeriod = UserParams.CustomParam("selected_period");
			paramDirection = UserParams.CustomParam("param_direction");
			paramMark = UserParams.CustomParam("param_mark");
			paramMark2 = UserParams.CustomParam("param_mark2");
			paramMarkPart = UserParams.CustomParam("param_mark_part");
			paramMarkTotal = UserParams.CustomParam("param_mark_total");

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

				// участок границы
				digestBorder = SKKHelper.DigestBorder_Init();
				comboBorder.ComboBorder_Init(digestBorder, false);
			}

			// export определяет, осуществляется ли обработка отчета для экспорта
			string eventTarget = Page.Request.Form.Get("__EVENTTARGET");
			export = eventTarget != null && (eventTarget.Contains("pdfExportButton") || eventTarget.Contains("docExportButton"));

			// изменение результатов запроса
			SKKHelper.ChangeSelectedPeriod(comboPeriodMonth);

			// сбор параметров 
			string selectedMonthUnique = SKKHelper.GetUniqueMonths(comboPeriodMonth.SelectedValues, comboPeriodYear.SelectedValue);
			
			// текстовики
			PageTitle.Text = String.Format(
				"Анализ сведений об осуществлении санитарно-карантинного контроля на {0} участке Государственной границы РФ",
				prepOne[comboBorder.SelectedValue]);
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Empty;
			txtPeriod.Text = "Отчетный период";

			// параметры для запроса
			selectedBorder.Value = digestBorder.GetMemberUniqueName(comboBorder.SelectedValue);
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
			bool isEmpty;

			// общие стили
			report.Style.Add("font-size", "10pt");

			// ШАПКА
			#region ШАПКА

			paramDirection.Value = "[Направления].[Направление].[Все направления]";

			report.Controls.Add(StyleMainTitle(ToHTML(Text(
				"Анализ сведений об осуществлении санитарно-карантинного контроля<br />" +
				"на #BORDER_PREPOSITIONAL# участке Государственной границы<br />Российской Федерации за #PERIOD_NOMINATIVE#"
				))));

			text = Text(
				"На #BORDER_PREPOSITIONAL# участке Государственной границы Российской Федерации за #PERIOD_NOMINATIVE# " +
				"на территории #SUBJ_COUNT#__#SUBJ_GENI# Российской Федерации #PP_FUNCTION_NOMI# #PP_COUNT#__#PP_NOMI# пропуска:<br />" +
				"#PP_TRANSP_ITEMS_BR_NOMI#.<br />"
				);
			text = ReplacePPGeneric(text, out isEmpty);
			text = ReplaceSubjectsGeneric(text, 0);
			div = StyleSimplePara(ToHTML(Paragraph(text)));

			if (!isEmpty)
			{
				report.Controls.Add(div);
			}
			else 
			{
				// если нет данных, заканчиваем формирование отчета
				report.Controls.Add(StyleCenter(ToHTML(Text("<br /><br /><br /><b>нет данных</b><br /><br />"))));
				return;
			}
			
			#endregion

			// ВВОЗ
			#region ВВОЗ

			paramDirection.Value = "[Направления].[Направление].[Все направления].[ Прибытие в РФ]";
			
			report.Controls.Add(StyleMainTitle(ToHTML(Text("ПРИБЫТИЕ В РОССИЮ"))));
			
			text = Text(
				"Санитарно-карантинный контроль осуществлялся в #PP_COUNT#__#PP_PREP# пропуска" +
				"$#PP_COUNT#?. Контроль прибывающего в РФ транспорта, людей и грузов не осуществлялся в #NPP_COUNT#__#NPP_PREP# пропуска$" +
				"$#PP_COUNT# * #NPP_COUNT#?:<br />#NPP_ITEMS_NOMI#$.<br />"
				);
			text = ReplacePPGeneric(text);
			text = ReplaceNPPGeneric(text);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

			// ТС
			#region ВВОЗ, ТС

			markSI = "ед.";
			markFormat = "N0";

			// ТС, досмотрено
			paramMark.Value = "[Показатели].[Показатель].[Количество досмотренных транспортных средств]";
			text = Text(
				"При прибытии в Россию досмотрено #MARK_COUNT#__#MARK_TRANSP1_NOMI# #MARK_TRANSP2_NOMI#" +
				"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска)$.<br />"
				);
			text = ReplaceMark(text, out isEmpty);
			text = ReplaceTransportMark(text, String.Empty);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				// ТС, приостановлено
				paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных транспортных средств]";
				text = Text(
					"Пропуск транспортных средств приостанавливался (временно запрещался) в #PP_COUNT#__#PP_PREP# пропуска, " +
					"всего приостановлен въезд #MARK_COUNT#__#MARK_TRANSP1_GENI# #MARK_TRANSP2_GENI#" +
					"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска):<br />" +
					"#PP_ITEMS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text);
				text = ReplacePPListMark(text);
				text = ReplaceMark(text);
				text = ReplaceTransportMark(text, String.Empty);
				report.Controls.Add(StyleSimple(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}

			#endregion

			// ЛИЦА
			#region ВВОЗ, ЛИЦА

			markSI = "человек";
			markFormat = "N0";

			// ЛИЦА, досмотрено
			paramMark.Value = "[Показатели].[Показатель].[Число лиц, досмотренных на наличие признаков инфекционных заболеваний]";
			text = Text(
				"При прибытии в Россию на наличие признаков инфекционных заболеваний было досмотрено #MARK_COUNT#__#MARK_PEOPLE_NOMI#" +
				"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска)$.<br />"
				);
			text = ReplaceMark(text, out isEmpty);
			text = ReplaceTransportMark(text, String.Empty);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				// ЛИЦА, приостановлено
				paramMark.Value = "[Показатели].[Показатель].[Выявлено больных и/или лиц с подозрением на инфекционные заболевания]";
				text = Text(
					"Больные/подозрительные на инфекционные заболевания выявлялись в #PP_COUNT#__#PP_PREP# пропуска, " +
					"всего #MARK_DETECTED_NOMI# #MARK_COUNT#__#MARK_PEOPLE_ILL_NOMI# и/или #MARK_PEOPLE_FACE_NOMI# " +
					"с подозрением на инфекционные заболевания" +
					"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска):<br />" +
					"#PP_ITEMS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text);
				text = ReplacePPListMark(text);
				text = ReplaceMark(text);
				text = ReplaceTransportMark(text, String.Empty);
				report.Controls.Add(StyleSimple(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}

			#endregion

			// ГРУЗЫ
			#region ВВОЗ, ГРУЗЫ

			markSI = "партия";
			markFormat = "N0";

			// ГРУЗЫ, досмотрено
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, на которые предъявлены документы должностным лицам органа Роспотребнадзора, осуществляющего СКК]";
			
			text = Text(
				"Контроль за ввозимыми грузами в #PERIOD_PREPOSITIONAL# на #BORDER_PREPOSITIONAL# участке границы РФ " +
				"осуществлялся в #PP_COUNT#__#PP_PREP# пропуска. Всего #MARK_VIEW_NOMI# #MARK_COUNT#__#MARK_GOODS_NOMI# грузов и товаров, " +
				"на которые предъявлялись документы должностным лицам Роспотребнадзора, осуществляющим санитарно-карантинный контроль.<br />"
				);
			text = ReplacePPMark(text, out isEmpty);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				text = Text(
					"Санитарно-карантинный контроль за ввозимыми грузами не осуществлялся в " +
					"#NPP_COUNT#__#NPP_PREP# пропуска$#NPP_COUNT#?:<br />#NPP_ITEMS_NOMI#$.<br />"
					);
				text = ReplaceNPPMark(text);
				report.Controls.Add(StyleSimple(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}
			
			// ГРУЗЫ, по группам
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			paramMark2.Value = "[Показатели].[Показатель].[Объем партий грузов, подлежащих СКК]";
			paramMarkPart.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			paramMarkTotal.Value = "[Показатели].[Показатель].[Количество партий грузов, на которые предъявлены документы должностным лицам органа Роспотребнадзора, осуществляющего СКК]";
			
			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				text = Text(
					"Из общего числа проконтроллированных партий грузов #GROUPS_PART_PRC# " +
					"(#GROUPS_TOTAL_COUNT#__#GROUPS_TOTAL_COUNT_NOMI# общим объемом #GROUPS_TOTAL_VOLUME#__тонн) составили грузы, " +
					"относящиеся к 1-11__группам Раздела__II Единого перечня товаров, " +
					"которые досматривались в #PP_COUNT#__#PP_PREP# пропуска$#PP_COUNT#?:<br />" +
					"#PP_ITEMS_GOODS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text, out isEmpty);
				text = ReplacePPListMarkDbl(text);
				text = ReplaceGroups(text, 0);
				text = ReplaceGroupsPrc(text);
				report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
				//isEmpty = isEmpty2;
			}

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				text = Text(
					"Наибольшее число досмотренных партий грузов относятся к следующим группам товаров " +
					"Раздела__II Единого перечня товаров: <br />#GROUPS_ITEMS#.<br />" +
					SKKHelper.AddParagraph() +
					"На долю партий досмотренных товаров остальных групп Раздела__II Единого перечня товаров " +
					"приходится в общей сложности #GROUPS_OTHER_PRC#."
					);
				text = ReplaceGroups(text, 4);
				report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

				// ГРУЗЫ, по группам, таблица
				report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
				div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
				div.Controls.Add(StyleItemTitle(ToHTML(Text(
					"Общее число досмотренных партий и общие объемы досмотренных и запрещенных<br />" +
					"к ввозу в РФ грузов 1-11 групп Раздела__II Единого перечня товаров<br />" +
					"на #BORDER_PREPOSITIONAL# участке Государственной границы РФ в #PERIOD_PREPOSITIONAL#"
				))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsBegin));
				div.Controls.Add(AddGridGroups(typeof (GridHelpGroupsInCommon)));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsEnd));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

				// ГРУЗЫ, по группам, диаграммы
				report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
				div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
				div.Controls.Add(StyleItemTitle(ToHTML(Text(
					"Число и объемы досмотренных партий товаров<br />1-11 групп Раздела__II Единого перечня товаров<br />" +
					"(ввоз в РФ, #BORDER_NOMINATIVE# участок, #PERIOD_NOMINATIVE#)"
				))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsBegin));
				div.Controls.Add(AddChartGroups(typeof (ChartHelpGroups)));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsEnd));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

				// ГРУЗЫ, приостановлено
				paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных партий грузов]";
				paramMark2.Value = "[Показатели].[Показатель].[Объем приостановленных партий грузов]";
				paramMarkPart.Value = "[Показатели].[Показатель].[Количество приостановленных партий грузов]";
				paramMarkTotal.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";

				text = Text(
					"Ввоз подконтрольных грузов запрещался в #PP_COUNT#__#PP_PREP# пропуска " +
					"и составил в общей сложности #GROUPS_TOTAL_COUNT#__#GROUPS_TOTAL_COUNT_ACCU# общим объемом #GROUPS_TOTAL_VOLUME#__тонн,__— " +
					"#GROUPS_PART_PRC# от общего количества проконтроллированных партий грузов$#PP_COUNT#?:<br />" +
					"#PP_ITEMS_GOODS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text, out isEmpty);
				text = ReplacePPListMarkDbl(text);
				text = ReplaceGroups(text, 0);
				text = ReplaceGroupsPrc(text);
				report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
				//isEmpty = isEmpty2;
			}

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				text = Text(
					"$#PP_COUNT#?Структура задержанных грузов:<br />#GROUPS_ITEMS#$.<br />"
					);
				text = ReplacePPMark(text);
				text = ReplaceGroups(text, 11);
				report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

				// ГРУЗЫ, структура, диаграмма
				paramMark.Value = "[Показатели].[Показатель].[Объем приостановленных партий грузов]";
				report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
				div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
				div.Controls.Add(StyleItemTitle(ToHTML(Text(
					"Распределение объемов запрещенных к пропуску грузов по видам товаров<br />" +
					"(ввоз в РФ, #BORDER_NOMINATIVE# участок, #PERIOD_NOMINATIVE#)"
				))));
				div.Controls.Add(AddChartPieGroupsVolume(typeof (ChartHelpGroupsVolume)));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}

			#endregion

			#endregion

			// ВЫВОЗ
			#region ВЫВОЗ

			paramDirection.Value = "[Направления].[Направление].[Все направления].[ Отбытие из РФ]";

			//div.Controls.Add(AddServiceRecipe(ServiceRecipe.NewPage));
			report.Controls.Add(StyleMainTitle(ToHTML(Text("ОТБЫТИЕ ИЗ РОССИИ"))));

			text = Text(
				"Санитарно-карантинный контроль осуществлялся в #PP_COUNT#__#PP_PREP# пропуска" +
				"$#PP_COUNT#?. Контроль транспорта, людей и грузов при отбытии из РФ не осуществлялся в #NPP_COUNT#__#NPP_PREP# пропуска$" +
				"$#PP_COUNT# * #NPP_COUNT#?:<br />#NPP_ITEMS_NOMI#$.<br />"
				);
			text = ReplacePPGeneric(text);
			text = ReplaceNPPGeneric(text);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));
			
			// ТС
			#region ВЫВОЗ, ТС

			markSI = "ед.";
			markFormat = "N0";

			// ТС, досмотрено
			paramMark.Value = "[Показатели].[Показатель].[Количество досмотренных транспортных средств]";
			text = Text(
				"При отбытии из России досмотрено #MARK_COUNT#__#MARK_TRANSP1_NOMI# #MARK_TRANSP2_NOMI#" +
				"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска)$.<br />"
				);
			text = ReplaceMark(text, out isEmpty);
			text = ReplaceTransportMark(text, String.Empty);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				// ТС, приостановлено
				paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных транспортных средств]";
				text = Text(
					"Пропуск транспортных средств приостанавливался (временно запрещался) в #PP_COUNT#__#PP_PREP# пропуска, " +
					"всего приостановлен въезд #MARK_COUNT#__#MARK_TRANSP1_GENI# #MARK_TRANSP2_GENI#" +
					"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска):<br />" +
					"#PP_ITEMS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text);
				text = ReplacePPListMark(text);
				text = ReplaceMark(text);
				text = ReplaceTransportMark(text, String.Empty);
				report.Controls.Add(StyleSimple(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}

			#endregion

			// ЛИЦА
			#region ВЫВОЗ, ЛИЦА

			markSI = "человек";
			markFormat = "N0";

			// ЛИЦА, досмотрено
			paramMark.Value = "[Показатели].[Показатель].[Число лиц, досмотренных на наличие признаков инфекционных заболеваний]";
			text = Text(
				"При выезде из России на наличие признаков инфекционных заболеваний было досмотрено #MARK_COUNT#__#MARK_PEOPLE_NOMI#" +
				"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска)$.<br />"
				);
			text = ReplaceMark(text, out isEmpty);
			text = ReplaceTransportMark(text, String.Empty);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				// ЛИЦА, приостановлено
				paramMark.Value = "[Показатели].[Показатель].[Выявлено больных и/или лиц с подозрением на инфекционные заболевания]";
				text = Text(
					"Больные/подозрительные на инфекционные заболевания выявлялись в #PP_COUNT#__#PP_PREP# пропуска, " +
					"всего #MARK_DETECTED_NOMI# #MARK_COUNT#__#MARK_PEOPLE_ILL_NOMI# и/или #MARK_PEOPLE_FACE_NOMI# " +
					"с подозрением на инфекционные заболевания" +
					"$#MARK_COUNT#? (#TRANSP_MAX1_PRC# в #TRANSP_MAX1_PREP# пунктах пропуска):<br />" +
					"#PP_ITEMS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text);
				text = ReplacePPListMark(text);
				text = ReplaceMark(text);
				text = ReplaceTransportMark(text, String.Empty);
				report.Controls.Add(StyleSimple(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}

			#endregion

			// ГРУЗЫ
			#region ВЫВОЗ, ГРУЗЫ

			markSI = "партия";
			markFormat = "N0";

			// ГРУЗЫ, досмотрено
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, на которые предъявлены документы должностным лицам органа Роспотребнадзора, осуществляющего СКК]";

			text = Text(
				"Контроль за вывозимыми грузами в #PERIOD_PREPOSITIONAL# на #BORDER_PREPOSITIONAL# участке границы РФ " +
				"осуществлялся в #PP_COUNT#__#PP_PREP# пропуска. Всего #MARK_VIEW_NOMI# #MARK_COUNT#__#MARK_GOODS_NOMI# грузов и товаров, " +
				"на которые предъявлялись документы должностным лицам Роспотребнадзора, осуществляющим санитарно-карантинный контроль.<br />"
				);
			text = ReplacePPMark(text, out isEmpty);
			text = ReplaceMark(text);
			report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));
			
			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				text = Text(
					"Санитарно-карантинный контроль за вывозимыми грузами не осуществлялся в " +
					"#NPP_COUNT#__#NPP_PREP# пропуска$#NPP_COUNT#?:<br />#NPP_ITEMS_NOMI#$.<br />"
					);
				text = ReplaceNPPMark(text);
				report.Controls.Add(StyleSimple(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}
			
			// ГРУЗЫ, по группам
			paramMark.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			paramMark2.Value = "[Показатели].[Показатель].[Объем партий грузов, подлежащих СКК]";
			paramMarkPart.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";
			paramMarkTotal.Value = "[Показатели].[Показатель].[Количество партий грузов, на которые предъявлены документы должностным лицам органа Роспотребнадзора, осуществляющего СКК]";
			
			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				text = Text(
					"Из общего числа проконтроллированных партий грузов #GROUPS_PART_PRC# " +
					"(#GROUPS_TOTAL_COUNT#__#GROUPS_TOTAL_COUNT_NOMI# общим объемом #GROUPS_TOTAL_VOLUME#__тонн) составили грузы, " +
					"относящиеся к 1-11__группам Раздела__II Единого перечня товаров, " +
					"которые досматривались в #PP_COUNT#__#PP_PREP# пропуска$#PP_COUNT#?:<br />" +
					"#PP_ITEMS_GOODS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text, out isEmpty);
				text = ReplacePPListMarkDbl(text);
				text = ReplaceGroups(text, 0);
				text = ReplaceGroupsPrc(text);
				report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
				//isEmpty = isEmpty2;
			}

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));

				text = Text(
					"Наибольшее число досмотренных партий грузов относятся к следующим группам товаров " +
					"Раздела__II Единого перечня товаров: <br />#GROUPS_ITEMS#.<br />" +
					SKKHelper.AddParagraph() +
					"На долю партий досмотренных товаров остальных групп Раздела__II Единого перечня товаров " +
					"приходится в общей сложности #GROUPS_OTHER_PRC#."
					);
				text = ReplaceGroups(text, 4);
				report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

				// ГРУЗЫ, по группам, таблица
				report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
				div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
				div.Controls.Add(StyleItemTitle(ToHTML(Text(
					"Общее число досмотренных партий и общие объемы досмотренных и запрещенных<br />" +
					"к вывозу из РФ грузов 1-11 групп Раздела__II Единого перечня товаров<br />" +
					"на #BORDER_PREPOSITIONAL# участке Государственной границы РФ в #PERIOD_PREPOSITIONAL#"
				))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsBegin));
				div.Controls.Add(AddGridGroups(typeof (GridHelpGroupsOutCommon)));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.GridGroupsEnd));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

				// ГРУЗЫ, по группам, диаграммы
				report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
				div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
				div.Controls.Add(StyleItemTitle(ToHTML(Text(
					"Число и объемы досмотренных партий товаров<br />1-11 групп Раздела__II Единого перечня товаров<br />" +
					"(вывоз из РФ, #BORDER_NOMINATIVE# участок, #PERIOD_NOMINATIVE#)"
				))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsBegin));
				div.Controls.Add(AddChartGroups(typeof (ChartHelpGroups)));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.ChartGroupsEnd));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

				// ГРУЗЫ, приостановлено
				paramMark.Value = "[Показатели].[Показатель].[Количество приостановленных партий грузов]";
				paramMark2.Value = "[Показатели].[Показатель].[Объем приостановленных партий грузов]";
				paramMarkPart.Value = "[Показатели].[Показатель].[Количество приостановленных партий грузов]";
				paramMarkTotal.Value = "[Показатели].[Показатель].[Количество партий грузов, подлежащих СКК]";

				text = Text(
					"Вывоз подконтрольных грузов запрещался в #PP_COUNT#__#PP_PREP# пропуска " +
					"и составил в общей сложности #GROUPS_TOTAL_COUNT#__#GROUPS_TOTAL_COUNT_ACCU# общим объемом #GROUPS_TOTAL_VOLUME#__тонн,__— " +
					"#GROUPS_PART_PRC# от общего количества проконтроллированных партий грузов$#PP_COUNT#?:<br />" +
					"#PP_ITEMS_GOODS_BR_NOMI#$.<br />"
					);
				text = ReplacePPMark(text, out isEmpty);
				text = ReplacePPListMarkDbl(text);
				text = ReplaceGroups(text, 0);
				text = ReplaceGroupsPrc(text);
				report.Controls.Add(StyleSimplePara(ToHTML(Paragraph(text))));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
				//isEmpty = isEmpty2;
			}

			if (!isEmpty)
			{
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"red\">"))));
			
				text = Text(
					String.Format("${0}#PP_COUNT#?Структура задержанных грузов:<br />#GROUPS_ITEMS#.<br />$", SKKHelper.AddParagraph())
					);
				text = ReplacePPMark(text);
				text = ReplaceGroups(text, 11);
				report.Controls.Add(StyleSimplePara(ToHTML(text)));

				// ГРУЗЫ, структура, диаграмма
				paramMark.Value = "[Показатели].[Показатель].[Объем приостановленных партий грузов]";
				report.Controls.Add(div = StyleCenter(ToHTML(Text(String.Empty))));
				div.Controls.Add(div = StyleItem(ToHTML(Text(String.Empty))));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceBegin));
				div.Controls.Add(StyleItemTitle(ToHTML(Text(
					"Распределение объемов запрещенных к пропуску грузов по видам товаров<br />" +
					"(вывоз из РФ, #BORDER_NOMINATIVE# участок, #PERIOD_NOMINATIVE#)"
				))));
				div.Controls.Add(AddChartPieGroupsVolume(typeof (ChartHelpGroupsVolume)));
				div.Controls.Add(AddServiceRecipe(ServiceRecipe.KeepingPlaceEnd));

				//if (isEmpty)
				//report.Controls.Add(StyleSimplePara(ToHTML(Text("<hr color=\"blue\">"))));
			}

			#endregion

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
			html.Style.Add("text-align", "justify");
			return html;
		}

		private static HtmlGenericControl StyleSimplePara(HtmlGenericControl html)
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
			return AddGridBase(gridHelperClass, "skk_018_mark_subjects");
		}

		/// <summary>
		/// создать таблицу по участкам границы
		/// </summary>
		private HtmlGenericControl AddGridBorders(Type gridHelperClass)
		{
			return AddGridBase(gridHelperClass, "skk_018_mark_borders");
		}

		/// <summary>
		/// создать таблицу с показателем (досмотрено/приостановлено)
		/// </summary>
		private HtmlGenericControl AddGridGroups(Type gridHelperClass)
		{
			return AddGridBase(gridHelperClass, "skk_018_groups_grid");
		}

		/// <summary>
		/// создать диаграмму
		/// </summary> 
		private HtmlGenericControl AddChart(Type chartHelperClass, string queryName)
		{
			countCharts++;
			ChartHelpBase chartHelper = (ChartHelpBase)Activator.CreateInstance(chartHelperClass);
			chartHelper.Init(1800 + countCharts, queryName);
			return chartHelper.GetItem();
		}

		/// <summary>
		/// создать диаграмму по ФО
		/// </summary>
		private HtmlGenericControl AddChartPieGroupsVolume(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_018_groups_chart_volume");
		}

		/// <summary>
		/// создать диаграмму по ФО
		/// </summary>
		private HtmlGenericControl AddChartDistricts(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_018_count_districts");
		}

		/// <summary>
		/// создать диаграмму по видам сообщения
		/// </summary>
		private HtmlGenericControl AddChartTransport(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_018_count_transport");
		}

		/// <summary>
		/// создать диаграмму по участкам границы
		/// </summary>
		private HtmlGenericControl AddChartBorders(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_018_count_borders");
		}

		/// <summary>
		/// создать диаграмму по группам товаров
		/// </summary>
		private HtmlGenericControl AddChartGroups(Type chartHelperClass)
		{
			return AddChart(chartHelperClass, "skk_018_groups_chart");
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
		/// ПП по виду сообщения
		/// </summary>
		private StringBuilder ReplacePPBase(StringBuilder text, string queryName, out bool isEmpty)
		{
			DataTable dTable = new Query(queryName).GetDataTable();
			int totalPP = 0;
			isEmpty = true;

			if (dTable.Rows.Count > 0)
			{
				totalPP = Convert.ToInt32(dTable.Rows[0][1]);
				isEmpty = totalPP == 0;

				dTable.Rows.RemoveAt(0);
				text = text
					.Replace("#PP_TRANSP_ITEMS_BR_PREP#", GetList(dTable, ListFormatDictManyBR, -1, new[] { prepMany }))
					.Replace("#PP_TRANSP_ITEMS_BR_NOMI#", GetList(dTable, ListFormatDictManyBR, -1, new[] { nomiMany }))
					.Replace("#PP_TRANSP_ITEMS_LINE_NOMI#", GetList(dTable, ListFormatDictMany, -1, new[] { nomiMany }))
					;
			}

			return text
				.Replace("#PP_COUNT#", totalPP.ToString("N0"))
				.Replace("#PP_FUNCTION_NOMI#", MultiNumEnding(nomiMany, totalPP, "функционировал"))
				.Replace("#PP_NOMI#", MultiNumEnding(nomiMany, totalPP, "пункт"))
				.Replace("#PP_PREP#", MultiNumEnding(prepMany, totalPP, "пункт"))
				;
		}

		/// <summary>
		/// ПП по виду сообщения
		/// </summary>
		private StringBuilder ReplacePPGeneric(StringBuilder text)
		{
			bool isEmpty;
			return ReplacePPBase(text, "skk_018_pp_count_generic", out isEmpty);
		}

		/// <summary>
		/// ПП по виду сообщения
		/// </summary>
		private StringBuilder ReplacePPGeneric(StringBuilder text, out bool isEmpty)
		{
			return ReplacePPBase(text, "skk_018_pp_count_generic", out isEmpty);
		}
		
		/// <summary>
		/// ПП по виду сообщения
		/// </summary>
		private StringBuilder ReplacePPMark(StringBuilder text)
		{
			bool isEmpty;
			return ReplacePPBase(text, "skk_018_pp_count_mark", out isEmpty);
		}

		/// <summary>
		/// ПП по виду сообщения
		/// </summary>
		private StringBuilder ReplacePPMark(StringBuilder text, out bool isEmpty)
		{
			return ReplacePPBase(text, "skk_018_pp_count_mark", out isEmpty);
		}

		/// <summary>
		/// ПП, перечисление
		/// </summary>
		private StringBuilder ReplacePPListMark(StringBuilder text)
		{
			DataTable table = new Query("skk_018_pp_list_mark").GetDataTable();
			
			if (table.Rows.Count > 0)
			{
				text = text
					.Replace("#PP_ITEMS_BR_NOMI#", GetList(table, ListFormatDashBR, -1, null))
					;
			}

			return text;
		}

		/// <summary>
		/// ПП, перечисление, два показателя
		/// </summary>
		private StringBuilder ReplacePPListMarkDbl(StringBuilder text)
		{
			DataTable table = new Query("skk_018_pp_list_mark_dbl").GetDataTable();

			if (table.Rows.Count > 0)
			{
				CRHelper.SaveToErrorLog("" + table.Rows.Count);
				text = text
					.Replace("#PP_ITEMS_GOODS_BR_NOMI#", GetList(table, ListFormatGoodsDashBR, -1, null))
					;
			}

			return text;
		}

		/// <summary>
		/// ПП, где не осуществлялся досмотр
		/// </summary>
		private StringBuilder ReplaceNPPBase(StringBuilder text, string queryName)
		{
			DataTable tableNpp = new Query(queryName).GetDataTable();
			int totalNpp = 0;
			
			if (tableNpp.Rows.Count > 0)
			{
				totalNpp = tableNpp.Rows.Count;
				text = text
					.Replace("#NPP_ITEMS_NOMI#", GetList(tableNpp, ListFormatSimpleExt, -1, null));
			}

			return text
				.Replace("#NPP_COUNT#", totalNpp.ToString("N0"))
				.Replace("#NPP_NOMI#", MultiNumEnding(nomiMany, totalNpp, "пункт"))
				.Replace("#NPP_PREP#", MultiNumEnding(prepMany, totalNpp, "пункт"))
				;
		}

		/// <summary>
		/// ПП, где не осуществлялся досмотр
		/// </summary>
		private StringBuilder ReplaceNPPGeneric(StringBuilder text)
		{
			return ReplaceNPPBase(text, "skk_018_npp_list_generic");
		}

		/// <summary>
		/// ПП, где не осуществлялся досмотр
		/// </summary>
		private StringBuilder ReplaceNPPMark(StringBuilder text)
		{
			return ReplaceNPPBase(text, "skk_018_npp_list_mark");
		}

		/// <summary>
		/// показатель
		/// </summary>
		private StringBuilder ReplaceMark(StringBuilder text)
		{
			bool isEmpty;
			return ReplaceMark(text, out isEmpty);
		}

		/// <summary>
		/// показатель
		/// </summary>
		private StringBuilder ReplaceMark(StringBuilder text, out bool isEmpty)
		{
			DataTable table = new Query("skk_018_mark_count").GetDataTable();
			double totalMarkRAW = 0;
			int totalMark = 0;
			isEmpty = true;

			if (table.Rows.Count > 0)
			{
				totalMarkRAW = Convert.ToDouble(CRHelper.DBValueConvertToDoubleOrZero(table.Rows[0][0]));
				totalMark = Convert.ToInt32(Math.Round(totalMarkRAW));
				isEmpty = totalMark == 0;
			}

			return text
				.Replace("#MARK_COUNT#", totalMarkRAW.ToString(markFormat))

				.Replace("#MARK_TRANSP1_NOMI#", MultiNumEnding(nomiMany, totalMark, "транспортное"))
				.Replace("#MARK_TRANSP2_NOMI#", MultiNumEnding(nomiMany, totalMark, "средство"))
				.Replace("#MARK_TRANSP1_GENI#", MultiNumEnding(geniMany, totalMark, "транспортное"))
				.Replace("#MARK_TRANSP2_GENI#", MultiNumEnding(geniMany, totalMark, "средство"))

				.Replace("#MARK_DETECTED_NOMI#", MultiNumEnding(nomiMany, totalMark, "выявлен"))
				.Replace("#MARK_PEOPLE_NOMI#", MultiNumEnding(nomiMany, totalMark, "человек"))
				.Replace("#MARK_PEOPLE_FACE_NOMI#", MultiNumEnding(nomiMany, totalMark, "лицо"))
				.Replace("#MARK_PEOPLE_ILL_NOMI#", MultiNumEnding(nomiMany, totalMark, "больной"))

				.Replace("#MARK_VIEW_NOMI#", MultiNumEnding(nomiMany, totalMark, "досмотрена"))
				.Replace("#MARK_GOODS_NOMI#", MultiNumEnding(nomiMany, totalMark, "партия"));
		}

		/// <summary>
		/// субъекты
		/// </summary>
		private StringBuilder ReplaceSubjectsGeneric(StringBuilder text, int count)
		{
			DataTable table = new Query("skk_018_subjects_count_generic").GetDataTable();
			int totalSubjects = 0;

			if (table.Rows.Count > 0)
			{
				totalSubjects = table.Rows.Count - 1;
				table.Rows.RemoveAt(0);
				text = text
					.Replace("#SUBJ_ITEMS#", GetList(table, ListFormatExt, count, new[] { geniOne }));
			}
			return text
				.Replace("#SUBJ_COUNT#", totalSubjects.ToString())
				.Replace("#SUBJ_GENI#", MultiNumEnding(geniMany, totalSubjects, "субъект"))
				.Replace("#SUBJ_PREP#", MultiNumEnding(prepMany, totalSubjects, "субъект"));
		}

		/// <summary>
		/// федеральные округа
		/// </summary>
		private StringBuilder ReplaceDistricts(StringBuilder text, int count, string extraListText)
		{
			DataTable table = new Query("skk_018_count_districts").GetDataTable();
			int totalDistricts = 0;

			if (table.Rows.Count > 0)
			{
				totalDistricts = table.Rows.Count - 1;
				table.Rows.RemoveAt(0);
				text = text
					.Replace("#DISTR_ITEMS#", GetList(table, ListFormatExtPrc, count, new[] { prepOne, (object)extraListText }));
			}
			return text
				.Replace("#DISTR_COUNT#", totalDistricts.ToString());
		}

		/// <summary>
		/// виды сообщения
		/// </summary>
		private StringBuilder ReplaceTransportMark(StringBuilder text, string extraListText)
		{
			DataTable table = new Query("skk_018_transport_count_mark").GetDataTable();

			if (table.Rows.Count > 1)
			{
				string max1Title = table.Rows[1][0].ToString();
				double max1Prc = Convert.ToDouble(table.Rows[1][2]);
				text = text
					.Replace("#TRANSP_MAX1_PREP#", MultiNumEnding(prepMany, COUNT_MANY, max1Title))
					.Replace("#TRANSP_MAX1_PRC#", max1Prc.ToString("P2"));

				if (table.Rows.Count > 2)
				{
					string max2Title = table.Rows[2][0].ToString();
					double max2Prc = Convert.ToDouble(table.Rows[2][2]);
					text = text
						.Replace("#TRANSP_MAX2_PREP#", MultiNumEnding(prepMany, COUNT_MANY, max2Title))
						.Replace("#TRANSP_MAX2_PRC#", max2Prc.ToString("P2"));
				}

				if (table.Rows.Count > 3)
				{
					string max3Title = table.Rows[3][0].ToString();
					double max3Prc = Convert.ToDouble(table.Rows[3][2]);
					text = text
						.Replace("#TRANSP_MAX3_PREP#", MultiNumEnding(prepMany, COUNT_MANY, max3Title))
						.Replace("#TRANSP_MAX3_PRC#", max3Prc.ToString("P2"));
				}

				table.Rows.RemoveAt(0);
				text = text.Replace("#TRANSP_ITEMS_PREP#", CRHelper.ToUpperFirstSymbol(GetList(table, ListFormatDashExt, -1, new[] { prepMany, (object)extraListText })));
				table.Rows.RemoveAt(0);
				text = text
					.Replace("#TRANSP_OTHER_PREP#", GetList(table, ListFormatDictManySimple, -1, new[] { prepMany }))
					.Replace("#TRANSP_OTHER_PRC#", (1 - max1Prc).ToString("P2"));
			}

			return text;
		}

		/// <summary>
		/// участки границы
		/// </summary>
		private StringBuilder ReplaceBorders(StringBuilder text, int count, string extraListText)
		{
			DataTable table = new Query("skk_018_count_borders").GetDataTable();
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
						Replace("#BORDERS_ITEMS#", GetList(table, ListFormatExtSI, count, new[] { prepOne, (object)extraListText })).
						Replace("#BORDERS_ITEMS_PRC#", GetList(table, ListFormatExtPrc, count, new[] { prepOne, (object)extraListText }));
				}
			}
			return text
				.Replace("#BORDERS_COUNT#", totalBorders.ToString("N0"))
				.Replace("#BORDERS_PREP#", MultiNumEnding(prepMany, totalBorders, "участок"))
				.Replace("#BORDERS_MARK_COUNT#", totalMarkRAW.ToString(markFormat))
				.Replace("#BORDERS_MARK_TRANSP1_NOMI#", MultiNumEnding(nomiMany, totalMark, "транспортное"))
				.Replace("#BORDERS_MARK_TRANSP2_NOMI#", MultiNumEnding(nomiMany, totalMark, "средство"))
				.Replace("#BORDERS_MARK_PEOPLE_NOMI#", MultiNumEnding(nomiMany, totalMark, "человек"))
				.Replace("#BORDERS_MARK_VIEW_NOMI#", MultiNumEnding(nomiMany, totalMark, "досмотрена"))
				.Replace("#BORDERS_MARK_GOODS_NOMI#", MultiNumEnding(nomiMany, totalMark, "партия"))
				.Replace("#BORDERS_ITEMS#", "на 0")
				.Replace("#BORDERS_ITEMS_PRC#", String.Empty);
		}

		/// <summary>
		/// доля товаров 1-11 групп
		/// </summary>
		private StringBuilder ReplaceGroupsPrc(StringBuilder text)
		{
			DataTable table = new Query("skk_018_groups_prc").GetDataTable();
			double prc = 0;

			if (table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
			{
				prc = Convert.ToDouble(table.Rows[0][0]);
			}
			return text
				.Replace("#GROUPS_PART_PRC#", prc.ToString("P2"))
				;
		}

		/// <summary>
		/// по группам товаров
		/// </summary>
		private StringBuilder ReplaceGroups(StringBuilder text, int count)
		{
			DataTable table = new Query("skk_018_groups").GetDataTable();
			double totalCount = 0;
			double totalVolume = 0;

			if (table.Rows.Count > 0
				&& table.Rows[0][1] != DBNull.Value
				&& table.Rows[0][2] != DBNull.Value)
			{
				totalCount = Convert.ToDouble(table.Rows[0][1]);
				totalVolume = Convert.ToDouble(table.Rows[0][2]);
				
				table.Rows.RemoveAt(0);
				
				decimal otherPrc = 1;
				for (int indexRow = 0; indexRow < Math.Min(count, table.Rows.Count); indexRow++)
				{
					otherPrc -= CRHelper.DBValueConvertToDecimalOrZero(table.Rows[indexRow][4]);
				}

				text = text
					.Replace("#GROUPS_ITEMS#", GetList(table, ListFormatGroupsPrcDashBR, count, new[] { nomiMany }))
					.Replace("#GROUPS_OTHER_PRC#", otherPrc.ToString("P2"))
					;
			}
			return text
				.Replace("#GROUPS_TOTAL_COUNT#", totalCount.ToString("N0"))
				.Replace("#GROUPS_TOTAL_COUNT_NOMI#", MultiNumEnding(nomiMany, Convert.ToInt32(totalCount), "партия"))
				.Replace("#GROUPS_TOTAL_COUNT_ACCU#", MultiNumEnding(accuMany, Convert.ToInt32(totalCount), "партия"))
				.Replace("#GROUPS_TOTAL_VOLUME#", totalVolume.ToString("N3"))
				;
		}

		#endregion

		#region генерация списков

		/// <summary>
		/// получить список: [1] dictS[0]
		/// </summary>
		private string ListFormatDictManyBR(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>)param[0];
			int value = Convert.ToInt32(row[1]);

			return
				SKKHelper.AddParagraph() +
				GetListItem(
					GetListFormatType(TypeList.DashBR, index, count),
					String.Format("{0}__{1}", value.ToString("N0"), MultiNumEnding(dict, value, row[0].ToString()))
				);
		}

		/// <summary>
		/// получить список: [1] dictS[0]
		/// </summary>
		private string ListFormatDictMany(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>)param[0];
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
		/// получить список: dictS[0] extra*{1}
		/// </summary>
		private string ListFormatDashExt(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>)param[0];
			string extra = (string)param[1];
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
		/// получить список: dict[0] ([1] si*)
		/// </summary>
		private string ListFormatExt(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string> dict = (Dictionary<string, string>)param[0];
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
		/// получить список: [0] - [1]
		/// </summary>
		private string ListFormatDashBR(DataRow row, int index, int count, object[] param)
		{
			int value = Convert.ToInt32(row[1]);

			return
				SKKHelper.AddParagraph() +
				GetListItem(
					GetListFormatType(TypeList.DashBR, index, count),
					String.Format("{0}__— {1}",
						row[0],
						value.ToString("N0")
					)
				);
		}

		/// <summary>
		/// получить список: [0] - [1] партий, [2] тоннн
		/// </summary>
		private string ListFormatGoodsDashBR(DataRow row, int index, int count, object[] param)
		{
			return
				SKKHelper.AddParagraph() +
				GetListItem(
					GetListFormatType(TypeList.DashBR, index, count),
					String.Format("{0}__— {1:N0}__{2}, {3:N3}__тонн",
						row[0],
						row[1],
						MultiNumEnding(nomiMany, CRHelper.DBValueConvertToInt32OrZero(row[1]), "партия"),
						row[2]
					)
				);
		}

		/// <summary>
		/// получить список: dict[0] ([1] si)
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
		/// получить список: dict[0] ([1] si - [2%] extra*{1})
		/// </summary>
		private string ListFormatExtPrc(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string> dict = (Dictionary<string, string>)param[0];
			string extraText = (string)param[1];
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
		/// получить список: [0]
		/// </summary>
		private string ListFormatSimple(DataRow row, int index, int count, object[] param)
		{
			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					row[0].ToString()
				);
		}

		/// <summary>
		/// получить список: [0] ([1])
		/// </summary>
		private string ListFormatSimpleExt(DataRow row, int index, int count, object[] param)
		{
			return
				SKKHelper.AddParagraph() +
				GetListItem(
					GetListFormatType(TypeList.DashBR, index, count),
					String.Format("{0} ({1})", row[0], row[1])
				);
		}
		
		/// <summary>
		/// получить список: dictS[0]
		/// </summary>
		private string ListFormatDictManySimple(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>)param[0];

			return
				GetListItem(
					GetListFormatType(TypeList.Simple, index, count),
					MultiNumEnding(dict, COUNT_MANY, row[0].ToString())
				);
		}
		
		/// <summary>
		/// получить список: [3/группы] - [1] dictS[0/партия] объемом [2] тонн
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

		/// <summary>
		/// получить список: [3/группы] - [1] dictS[0/партия] ([4]%) объемом [2] тонн
		/// </summary>
		private string ListFormatGroupsPrcDashBR(DataRow row, int index, int count, object[] param)
		{
			Dictionary<string, string[]> dict = (Dictionary<string, string[]>)param[0];
			int valueCount = Convert.ToInt32(row[1]);
			double valueVolume = Convert.ToDouble(row[2]);
			string valueExtra = row[3].ToString().Replace("группа", "группы");
			decimal valuePrc = CRHelper.DBValueConvertToDecimalOrZero(row[4]);

			return
				SKKHelper.AddParagraph() +
				GetListItem(
					GetListFormatType(TypeList.DashBR, index, count),
					String.Format("{0}____— {1:N0}__{2} ({3:P2}) объемом {4:N3}__тонн",
						valueExtra,
						valueCount,
						MultiNumEnding(dict, valueCount, "партия"),
						valuePrc,
						valueVolume
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
			if (index == count - 2)
				return lists[typeList][TypeListPlaces.PreLast];
			if (index == count - 1)
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
				return key + "_";

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
			text = text
				.Replace("__", "&nbsp;")
				.Replace("#PERIOD_NOMINATIVE#", periodNominative)
				.Replace("#PERIOD_PREPOSITIONAL#", periodPrepositional)
				.Replace("#BORDER_NOMINATIVE#", comboBorder.SelectedValue.ToLower())
				.Replace("#BORDER_PREPOSITIONAL#", prepOne[comboBorder.SelectedValue])
				.Replace("#CHART_NEXT#", countCharts.ToString())
				.Replace("#CHART_PREV#", (countCharts - 1).ToString())
				.Replace("#TABLE_NEXT#", countTables.ToString())
				.Replace("#TABLE_PREV#", (countTables - 1).ToString())
				;

			MatchCollection matches = Regex.Matches(text, @"\$([^\?]*)\?([^\$]*)\$");
			foreach (Match match in matches)
			{
				string raw_data = match.Value;
				string raw_condition = match.Groups[1].Value;
				string raw_text = match.Groups[2].Value;
				decimal ifValue;

				if(raw_condition.Contains("*"))
				{
					List<string> args = new List<string>(raw_condition.Split("*"));
					decimal arg1;
					decimal arg2;
					Decimal.TryParse(args[0].Trim(), out arg1);
					Decimal.TryParse(args[1].Trim(), out arg2);
					raw_condition = Convert.ToString(arg1 * arg2);
				}

				Decimal.TryParse(raw_condition, out ifValue);
				if (ifValue <= 0)
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
			/// Перечисление "-" с разделением на строки: - #,\n - #,\n - #\n, - #
			/// </summary>
			DashBR
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
