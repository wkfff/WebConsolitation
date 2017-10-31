using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.SKK.STAT_0005_0001
{
	public partial class Default : CustomReportPage
	{
		private const int PARAGRAPH_LENGTH = 20;
		private const int PARAGRAPH_HEIGHT = 18;

		private Collection<string> reportSections;
		private Dictionary<string, string[]> nomiMany;
		private int chartsIndex = 0;

		// параметры запроса
		private CustomParam paramCount;
		

		public Default()
		{
			reportSections = 
				new Collection<string>
				{
					"Общие сведения",
					"Символы округа",
					"Карта-схема и состав округ",
					"Краткая историческая справка",
					"Природные богатства",
					"Климат и географическое положение",
					"Население округа",
					"Основные тенденции"
				};

			// именительный падеж
			nomiMany = new Dictionary<string, string[]> // 1,2-4,5
			{
				{"человек", 
					new[] {"человек", String.Empty, "а", String.Empty}}
			};
		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			paramCount = UserParams.CustomParam("count");

		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			if (!Page.IsPostBack)
			{
				// параметр - раздел отчета
				ComboSection.Title = "Выберите раздел";
				ComboSection.Width = 400;
				ComboSection.MultiSelect = false;
				ComboSection.ParentSelect = false;
				ComboSection.ShowSelectedValue = true;
				ComboSection.FillValues(reportSections);
				
			}
			
			// текстовики
			PageTitle.Text = "Паспорт субъекта Российской Федерации";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = "Общие сведения и социально-экономическое развитие Ханты-Мансийского автономного округа&nbsp;— Югры";
			
			// погнали!
			GenerateSection(ComboSection.SelectedIndex);

		}

		private void GenerateSection(int sectionID)
		{
			ReportText.Style.Add("font-size", "14px");
			ReportText.Style.Add("text-align", "justify");
			ReportText.Style.Add("padding-bottom", "10px");
			
			switch (sectionID)
			{
				case 0:
					GenerateSection_Common();
					break;
				case 1:
					GenerateSection_Symbol();
					break;
				case 2:
					GenerateSection_Map();
					break;
				case 3:
					GenerateSection_History();
					break;
				case 4:
					GenerateSection_Nature();
					break;
				case 5:
					GenerateSection_Climate();
					break;
				case 6:
					GenerateSection_Population();
					break;
				case 7:
					GenerateSection_Tendencies();
					break;
				default:
					GenerateSection_Common();
					break;
			}
			
		}

		/// <summary>
		/// раздел "Общие сведения"
		/// </summary>
		private void GenerateSection_Common()
		{
			paramCount.Value = "1";
			ReportText.Controls.Add(ToHTML(
				ReplaceSection_Population(Text(
				"<img src=\"images/gerb.jpg\" align=\"left\" width=\"150\" style=\"padding: 8px 5px 3px 0\">" +
				"@PARAGRAPH@" +
				"Ханты-Мансийский автономный округ__— Югра является субъектом Российской Федерации, " +
				"который находится в Уральском федеральном округе.<br />" +
				"@PARAGRAPH@" +
				"Ханты-Мансийскийский автономный округ образован 10__декабря 1930__года.<br />" +
				"@PARAGRAPH@" +
				"Административным центром округа является <b>г. Ханты-Мансийск</b>.<br />" +
				"@PARAGRAPH@" +
				"Площадь составляет 534,8__тыс. кв.__км.<br />" +
				"@PARAGRAPH@" +
				"Численность постоянного населения на 1__января @POPULATION_YEAR@__года__— @POPULATION_VALUE@__тыс.__человек.<br />" +
				"@PARAGRAPH@" +
				"Округ расположен в серединной части России. Он занимает центральную часть Западно-Сибирской равнины. " +
				"На севере округ граничит с Ямало-Ненецким автономным округом, на северо-западе__— с Республикой Коми, " +
				"на юго-западе со Свердловской областью, на юге__— с Тобольским и Уватским районами Тюменской области, " +
				"на юго-востоке и востоке__— с Томской областью и Красноярским краем.<br />" +
				"@PARAGRAPH@" +
				"Специфика экономики округа связана с открытием здесь богатейших нефтяных и газовых месторождений. " +
				"В отраслевой структуре промышленной продукции нефтегазодобывающая промышленность составляет 89,4%, " +
				"электроэнергетика__— 5,5%, машиностроение и металлообработка__— 2,4%, газоперерабатывающая__— 1,6%, " +
				"лесозаготовительная и деревообрабатывающая__— 0,24%, производство строительных материалов__— 0,24%, " +
				"пищевая__— 0,17%, нефтеперерабатывающая__— 0,1%.<br />" +
				"@PARAGRAPH@" +
				"Природные условия округа не благоприятствуют развитию сельского хозяйства. Поэтому большая часть " +
				"сельскохозяйственной и пищевой продукции завозится из других регионов России.<br />" +
				"@PARAGRAPH@" +
				"В Ханты-Мансийском автономном округе основная перевозка грузов приходится на водный и железнодорожный транспорт, " +
				"29% перевозится автомобильным транспортом и 2%__— авиационным. Общая протяженность железнодорожных путей 1__106__км. " +
				"Протяженность автомобильных дорог__— более 18__тыс.__км, из них с твердым покрытием__— более 13__тыс.__км.  " +
				"Протяженность судоходных водных путей составляет 5__608__км, из которых 3__736__км — боковые и малые реки. " +
				"Общая протяженность магистральных нефтепроводов на территории округа составляет 6__283__км, газопроводов__— 19__500__км.<br />" +
				"@PARAGRAPH@" +
				"Основные продукты экспорта: нефть, продукты ее переработки, топливо, древесина, изделия из нее и т.п. " +
				"Импорт округа составляют высокотехнологичное оборудование для предприятий ТЭК, изделия из черных металлов, " +
				"телекоммуникационное и компьютерное оборудование, автомобили и т.п.<br />"
			))));

		}

		/// <summary>
		/// раздел "Символ округа"
		/// </summary>
		private void GenerateSection_Symbol()
		{
			ReportText.Controls.Add(ToHTML(
				Text(
				"<img src=\"images/gerb.jpg\" align=\"left\" width=\"150\" style=\"padding: 8px 5px 3px 0\">" +
				"@PARAGRAPH@" +
				"<b>Герб Ханты-Мансийского автономного округа__— Югры</b> представляет собой серебряную эмблему, " +
				"расположенную на подкладе двух щитов, вписанных один в другой, и воспроизводящую стилизованный символ " +
				"\"Кат ухуп вой\" (двуглавая птица) в поле рассеченного лазоревого (синего, голубого) и зеленого щита. " +
				"Контур щита обведен золотом.<br />" +
				"@PARAGRAPH@" +
				"Фигурный щит вписан в прямой щит красного цвета, представляющий собой прямоугольник с фигурным заострением " +
				"в нижней части. Щит увенчан элементом белого цвета, выполненным в орнаментальном стиле обских угров, " +
				"и окаймлен кедровыми ветвями зеленого цвета, сплетенными в полукольцо. Девиз \"Югра\" начертан серебряными " +
				"литерами на лазоревой ленте, расположенной под щитом.<br />" +
				"<div style=\"clear:both;\"></div>" +

				"<img src=\"images/flag.jpg\" align=\"left\" width=\"150\" style=\"padding: 8px 5px 3px 0\">" +
				"@PARAGRAPH@" +
				"<b>Флаг Ханты-Мансийского автономного округа__— Югры</b> представляет собой прямоугольное полотнище, разделенное " +
				"по горизонтали на две равновеликие полосы (верхняя__— сине-голубая, нижняя__— зеленая), завершенное по вертикали " +
				"прямоугольной полосой белого цвета.<br />" +
				"@PARAGRAPH@" +
				"В левой верхней части полотна расположен элемент белого цвета из герба Ханты-Мансийского автономного округа__— Югры.<br />" +
				"<div style=\"clear:both;\"></div>"
			)));

		}

		/// <summary>
		/// раздел "Карта-схема и состав округа"
		/// </summary>
		private void GenerateSection_Map()
		{

			ReportText.Controls.Add(StyleCenter(ToHTML(
				Text("<img src=\"images/map2.gif\">"))));

			ReportText.Controls.Add(ToHTML(
				Text(
				"@PARAGRAPH@" +
				"В состав округа входят 106 муниципальных образований:<br />" +
				"@PARAGRAPH@@PARAGRAPH@" +
				"&#9679;&nbsp;<b>13 городских округов</b> (г.Когалым, г.Лангепас, г.Мегион, г.Нефтеюганск, г.Нижневартовск, г.Нягань, " +
				"г.Покачи, г.Пыть-Ях, г.Радужный, г.Сургут, г.Урай, г.Ханты-Мансийск, г.Югорск);<br />" +
				"@PARAGRAPH@@PARAGRAPH@" +
				"&#9679;&nbsp;<b>9 муниципальных районов</b> (Белоярский, Берёзовский, Кондинский, Нефтеюганский, Нижневартовский, " +
				"Октябрьский, Советский, Сургутский, Ханты-Мансийский);<br />" +
				"@PARAGRAPH@@PARAGRAPH@" +
				"&#9679;&nbsp;<b>26 городских поселений</b> (Белоярский, Березово, Игрим, Кондинское, Куминский, Луговой, Междуреченский, " +
				"Мортка, Пойковский, Излучинск, Новоаганск, Октябрьское, Андра, Приобье, Талинка, Зеленоборск, Таежный, " +
				"Агириш, Коммунистический, Пионерский, Советский, Малиновский, Белый Яр, Барсово, Федоровский, Лянтор);<br />" +
				"@PARAGRAPH@@PARAGRAPH@" +
				"&#9679;&nbsp;<b>58 сельских поселений</b> (Верхнеказымский, Казым, Лыхма, Полноват, Сорум, Сосновка, Саранпауль, Приполярный, " +
				"Светлый, Хулимсунт, Леуши, Мулымья, Шугур, Болчары, Половинка, Салым, Сентябрьский, Чеускино, Каркатеевы, " +
				"Куть-Ях, Лемпино, Усть-Юган, Сингапай, Аган, Ларьяк, Ваховск, Покур, Вата, Зайцева Речка, Карымкары, " +
				"Малый Атлым, Перегребное, Сергино, Шеркалы, Каменное, Унъюган, Алябьевский, Солнечный, Локосово, Русскинская, " +
				"Сытомино, Нижнесортымский, Лямина, Тундрино, Угут, Ульт-Ягун, Горноправдинск, Цингалы, Кедровый, Красноленинский, " +
				"Луговской, Согом, Нялинское, Кышик, Селиярово, Сибирский, Выкатной, Шапша).<br />"
			)));

		}

		/// <summary>
		/// раздел "Краткая историческая справка"
		/// </summary>
		private void GenerateSection_History()
		{

            ReportText.Controls.Add(ToHTML(
                Text(
				"<img src=\"images/istor1.gif\" align=\"right\" style=\"padding: 8px 0px 3px 5px\">" +
                "@PARAGRAPH@" +
                "Свидетельства жизни на югорской земле относятся к среднекаменному веку. Стоянки, поселения и могильники " +
                "того времени малочисленны и уникальны. Основная часть их расположена в бассейне Конды (Советский, Октябрьский районы) " +
                "и Северной Сосьвы (Березовский и Белоярский районы)__— поселения Смоляной Сор__1 у поселка Игрим и на реке Эсс около города Югорска. " +
                "Территория привлекала к себе и обитателей новокаменного века. Единственное древнейшее укрепленное поселение эпохи камня " +
                "лесной полосы Евразии, городище Амня, находится в Березовском районе. Поселения Хулюм-Сунт, Сартынья, Честый-Яг, Няксимволь__— " +
                "эти памятники остались от древних культур, относящихся к праугросамодийской языковой семье.<br />" +
                "@PARAGRAPH@" +
                "Эпоха раннего железа ознаменовалась бурным расцветом культур и оставила наиболее значительное количество прекрасных памятников. " +
                "Самые интересные из них представлены на всемирно известной Барсовой горе и на территории вблизи п. Сайгатино в Сургутском районе.<br />" +
				"@PARAGRAPH@" +
                "Средневековый период связан с распространением так называемых угорских княжеств и городков. Именно с этого времени началось " +
                "развитие культурно-исторических образований__— прямых предков ряда коренных народов Западной Сибири (ханты, манси, селькупов, " +
                "ненцев, тунгусов и т.д.) Эпоха оставила интереснейшие клады (наиболее изученный из которых__— Холмогорский), оригинальные по " +
                "своему архитектурному содержанию городища__— Барсов городок__2/1, Кучиминское, Ермаково__I,__II,__VI, уникальные могильники__— " +
                "Барсовский__V, Сайганики__— Барсовский__V, Сайгатинский__II, \"Барсов городок\".<br />" +
				"<img src=\"images/istor2.gif\" align=\"left\" style=\"padding: 8px 5px 3px 0\">" +
				"@PARAGRAPH@" +
				"С 1364__г. началось непосредственное освоение русскими восточных склонов Урала. В 1478__г. Югорская земля вошла в состав первого " +
                "русского государства.<br />" +
				"@PARAGRAPH@" +
				"В XVI__в. Московское правительство перешло к реализации плана строительства укрепленных городов на новых землях. Одним из первых " +
                "подобных городов стал Березов, основанный в 1593__г. на месте остяцкого городка \"Сугмут-Ваш\" или, как его еще называли вогулы, " +
                "\"Халумо-Сугмут\", что в переводе означает \"Березовый город\". Чуть позже был основан г.__Сургут, в 1595__г.__— Обдорский городок.<br />" +
				"@PARAGRAPH@" +
				"Появившиеся на Обском Севере городки стали служить местом торговли. На наиболее оживленных направлениях возникли особые станции " +
                "для перемены лошадей__— \"ямы\". В 1637__году были устроены два яма__— Демьянский и Самаровский (ныне г.Ханты-Мансийск).<br />" +
				"@PARAGRAPH@" +
 				"В целях установления новых порядков и хозяйственного освоения богатейшего по природным ресурсам края указом Петра__I в 1708__году " +
                "была учреждена Сибирская губерния (в нее вошли города Березов, Сургут). В 1775__г. указом Екатерины__II создана Тобольская губерния.<br />" +
				"@PARAGRAPH@" +
				"За историей края закрепилась слава места ссылки государственных преступников. В Березовском районе отбывали наказание князь " +
                "Дмитрий Ромодановский, в 1742__году__— граф Андрей Остерман, в 1798__году__— многочисленное семейство князей Долгоруковых. " +
                "В земле Березовской покоится прах сосланного в эти места князя Меншикова и его дочери Марии. После событий на Сенатской площади " +
                "здесь отбывали ссылку декабристы.<br />" +
				"@PARAGRAPH@" +
				"Административное управление и выполнение судебных функций у народностей Севера осуществлялось на основе устава Сперанского " +
                "\"Об управлении инородцев Сибири\", утвержденного в 1822__году.<br />" +
				"@PARAGRAPH@" +
				"Характер экономики Обь-Иртышского Севера на рубеже XIX-XX__вв. определялся как особенностями природно-климатических условий, " +
                "так и относительно низкой плотностью населения. Основным средством сообщения служил речной транспорт. Начавшееся в середине XIX__в. " +
                "движение пароходов становилось все более интенсивным. В 1859__году по Оби и Иртышу ходило 7__пароходов, в 1904__году__— 107, а в " +
                "1913__году__— уже 220.<br />" +
				"@PARAGRAPH@" +
				"В 1909__году в Самарово была проложена телеграфная линия, в 1913__году она достигла Березова и Сургута.<br />" +
				"@PARAGRAPH@" +
				"Промышленность Обь-Иртышского Севера была представлена несколькими полукустарными рыбоконсервными заведениями. Сельскохозяйственное " +
                "производство в северных условиях сводилось к овощеводству и животноводству. Главным занятием северян были рыбная ловля, " +
                "охота на зверей и птиц, сбор кедровых орехов, грибов и ягод.<br />" +
				"@PARAGRAPH@" +
				"В 1918__г. Тобольская губерния переименована в Тюменскую, губернский центр перенесен в г.__Тюмень. В 1923__г. упразднены губернии, " +
                "уезды, волости. Образована Уральская область, Тобольский округ и районы: Березовский, Сургутский, Самаровский, Кондинский.<br />" +
				"@PARAGRAPH@" +
				"10__декабря 1930__года Президиум ВЦИК принял постановление \"Об организации национальных объединений в районах расселения малых " +
                "народностей Севера\". Постановлением предусматривалось создание 8__национальных округов, в том числе и Остяко-Вогульского " +
                "(Ханты-Мансийского).<br />" +
				"@PARAGRAPH@" +
 				"В связи с упразднением Тобольского округа произошло уточнение в составе и границах Ханты-Мансийского и Ямало-Ненецкого " +
                "национальных округов. В составе Ханты-Мансийского округа образованы районы: Березовский (центр р.п. Берёзово), Микояновский " +
                "(центр Кондинское), Кондинский (центр Нахрачи), Самаровский (центр Самарово), Сургутский (центр р.п. Сургут), Ларьякский (центр Ларьяк).<br />" +
				"@PARAGRAPH@" +
				"Ханты-Мансийский национальный округ получил статус автономного в 1977__г.<br />" +
				"@PARAGRAPH@" +
				"В 1993__г. Ханты-Мансийский автономный округ получил статус полноправного субъекта Российской Федерации согласно ст.65 Конституции__РФ.<br />" +
				"@PARAGRAPH@" +
 				"В соответствии с Указом Президента Российской Федерации от 25__июля 2003__года  №__841 Ханты-Мансийский автономный округ переименован " +
                "в Ханты-Мансийский автономный округ__— Югра.<br />"
            )));

		}

		/// <summary>
		/// раздел "Природные богатства"
		/// </summary>
		private void GenerateSection_Nature()
		{
			ReportText.Controls.Add(ToHTML(
				Text(
				"<img src=\"images/prirod_bogat.jpg\" align=\"left\" style=\"padding: 8px 5px 3px 0\">" +
				"@PARAGRAPH@" +
				"Территория Югры располагает богатыми природными ресурсами. Леса, представляя огромную ценность, являются в то же время питомником и " +
				"хранителем пушного зверя и лесной птицы. Водные бассейны содержат запасы ценной рыбы, привлекают водоплавающую птицу. На протяжении " +
				"многих веков жизнь коренного населения зависела от рыбного и охотничьего промыслов.<br />" +
				"@PARAGRAPH@" +
				"В настоящее время округ является основным нефтегазоносным районом Российской Федерации. Наиболее крупные месторождения нефти и газа__— " +
				"Самотлорское, Федоровское, Мамонтовское, Приобское.<br />" + 
				"@PARAGRAPH@" +
				"В округе добывается россыпное золото, жильный кварц и коллекционное сырье. Открыты месторождения бурого и каменного угля. Обнаружены " +
				"залежи железных руд, меди, цинка, свинца, ниобия, тантала, проявления бокситов и др. Находятся в стадии подготовки к разработке " +
				"месторождения декоративного камня, кирпично-керамзитовых глин, песков строительных.<br />" +
				"@PARAGRAPH@" +
				"В пределах Урала на территории округа выявлены породы, обладающие высокими фильтрационными и сорбционными свойствами. К их числу " +
				"относятся цеолитсодержащие породы, вулканические образования и др.<br />" +
				"@PARAGRAPH@" +
				"Разведаны и утверждены эксплуатационные запасы минеральных (йодо-бромных) вод.<br />"+
				"<div style=\"clear:both;\"></div>"
			)));

		}

		/// <summary>
		/// раздел "Климат и географическое положение"
		/// </summary>
		private void GenerateSection_Climate()
		{
			ReportText.Controls.Add(ToHTML(
				Text(
				"<img src=\"images/klimat_2.jpg\" align=\"right\" style=\"padding: 8px 0 3px 5px\">" +
				"@PARAGRAPH@" +
				"Ханты-Мансийский автономный округ__— Югра расположен в центральной части Западно-Сибирской равнины, одной из крупнейших " +
				"равнин в мире. По территории округа с юга на север протекают две крупнейшие реки России__— Обь и Иртыш. Кроме того, " +
				"наиболее значительными реками округа являются притоки Оби: Вах, Аган, Тромъеган, Большой Юган, Лямин, Пим, Большой Салым, " +
				"Назым, Северная Сосьва, Казым;&nbsp; притоки Иртыша: Конда, Согом. Водный режим рек характеризуется продолжительным половодьем. " +
				"Зимой реки замерзают на длительный период — до 6 месяцев.<br />" +
				"@PARAGRAPH@" +
				"Округ расположен в пределах одной природной зоны__— лесной. Основную часть территории занимает сильно заболоченная тайга. " +
				"Среди болот и лесов расположено более 25__тысяч озёр. Они питаются, в основном, зимними и лишь отчасти летними осадками.<br />" +
				"<img src=\"images/klimat_1.jpg\" align=\"left\" style=\"padding: 8px 5px 3px 0\">" +
				"@PARAGRAPH@" +
				"Климат округа резко континентальный: суровая продолжительная зима с сильными ветрами и  метелями, весенними возвратами холодов, " +
				"поздними весенними и ранними осенними заморозками. При этом, вследствие обилия солнечного света и тепла, преобладает тёплое, " +
				"хотя и довольно короткое, лето. В округе наблюдается большая изменчивость погоды, и отмечаются частые ветры.<br />" +
				"@PARAGRAPH@" +
				"Территория Ханты-Мансийского автономного округа__— Югры, по сравнению с Европейской территорией России, отличается более низкими " +
				"температурами и большей суровостью климата, в том числе и летом. Весна__— наиболее короткий и сухой сезон в году с преобладанием " +
				"ясной и ветреной погоды. На большей части округа в апреле ещё лежит  снег. Последние заморозки отмечаются в конце мая__— начале июня. " +
				"Средняя продолжительность безморозного периода от 65__дней (в северных районах округа) до 115__дней (в южных районах округа).<br />" +
				"@PARAGRAPH@" +
				"Лето довольно жаркое, но короткое, со средней температурой июля +16&deg;+19&deg;. Абсолютный максимум температуры воздуха на " +
				"территории округа составляет +34&deg;+37&deg;. Переход к осени заметен по значительному понижению температуры воздуха. В конце августа " +
				"в северных районах уже отмечаются первые заморозки, а к середине сентября__— и на остальной территории округа. Зимний период с устойчивым " +
				"снежным покровом и морозами длится 5-6 месяцев. Число дней с оттепелью, в среднем, очень невелико. Январь и февраль отличаются " +
				"ясной и морозной погодой с сильным радиационным выхолаживанием и слабыми ветрами. Абсолютный минимум температуры воздуха для нашего " +
				"округа находится в пределах от -48&deg;&nbsp;до -60&deg;. Среднее годовое количество осадков по округу составляет 443-610мм. " +
				"Максимальное их количество выпадает в июле и августе.<br />" +
				"<div style=\"clear:both;\"></div>"
			)));

		}

		/// <summary>
		/// раздел "Население округа"
		/// </summary>
		private void GenerateSection_Population()
		{
			HtmlGenericControl div;

			paramCount.Value = "1";
			ReportText.Controls.Add(ToHTML(
				ReplaceSection_Population(Text(
				"@PARAGRAPH@" +
				"Численность населения на начало @POPULATION_YEAR@ года составляет @POPULATION_VALUE@__тысяч человек.<br />" +
				"@PARAGRAPH@" +
				"В связи с бурным развитием нефтегазодобывающей промышленности за последние годы население округа увеличивается. " +
				"Динамика численности населения представлена на диаграмме__1.<br />"
				))));

			// Диаграмма 1
			paramCount.Value = "5";
			ReportText.Controls.Add(
				div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(
				div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма 1"))));
			div.Controls.Add(AddChart(typeof(ChartHelperPopulation)));
			
			ReportText.Controls.Add(ToHTML(
				Text(
				"@PARAGRAPH@" +
				"Устойчивость демографического развития Югры достигается за счет двух факторов: молодой возрастной структуры " +
				"населения и сравнительно низким уровнем смертности в сравнении с другими регионами Российской Федерации " +
				"(В настоящее время определяющим фактором увеличения численности населения в автономном округе является " +
				"естественный прирост, его динамика представлена на диаграмме__2).<br />" 
				)));

			// Диаграмма 2
			ReportText.Controls.Add(
				div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(
				div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма 2"))));
			div.Controls.Add(AddChart(typeof(ChartHelperMovement)));

			ReportText.Controls.Add(ToHTML(
				ReplaceSection_Movement(Text(
				"@PARAGRAPH@" +
				"За @MOVEMENT_YEAR@__год число родившихся в округе составляет @MOVEMENT_BIRTH_VALUE@__@MOVEMENT_BIRTH_NOMI@, " +
				"число умерших__— @MOVEMENT_DEAD_VALUE@__@MOVEMENT_DEAD_NOMI@.<br />"
				))));

			ReportText.Controls.Add(ToHTML(
				ReplaceSection_Migration(Text(
				"@PARAGRAPH@" +
				"За @MIGRATION_YEAR@__год число прибывших в округ составляет  @MIGRATION_ARRIVAL_VALUE@__@MIGRATION_ARRIVAL_NOMI@, " +
				"число выбывших @MIGRATION_DEPART_VALUE@__@MIGRATION_DEPART_NOMI@. " +
				"Динамика миграционного прироста представлена на диаграмме__3.<br />"
				))));

			// Диаграмма 3
			ReportText.Controls.Add(
				div = StyleCenter(ToHTML(Text(String.Empty))));
			div.Controls.Add(
				div = StyleItem(ToHTML(Text(String.Empty))));
			div.Controls.Add(StyleRight(ToHTML(Text("Диаграмма 3"))));
			div.Controls.Add(AddChart(typeof(ChartHelperMigration)));

			ReportText.Controls.Add(ToHTML(
				Text(
				"@PARAGRAPH@" +
				"На территории округа проживают представители 123__национальностей, в том числе славянской, тюркской, финно-угорской групп. " +
				"По данным переписи 2002__года, в национальном составе населения округа преобладают: русские, украинцы, татары, башкиры.<br />"+
				"@PARAGRAPH@" +
				"Ханты-Мансийский автономный округ является исторической родиной коренного (аборигенного) населения, " +
				"которое представлено тремя небольшими по численности народностями. Это ханты, манси и лесные ненцы. " +
				"Общая их численность составляет около 1,5%."
				)));
			 
		}

		/// <summary>
		/// раздел "Основные тенденции"
		/// </summary>
		private void GenerateSection_Tendencies()
		{
			ReportText.Controls.Add(AddGrid(typeof(GridHelperTendencies)));
			ReportText.Controls.Add(
				new HyperLink
			    {
			        Text = "Мониторинг социально-экономических показателей",
					NavigateUrl = "~/reports/SEP_0005_0001/default.aspx"
			    });

		}

		#region методы подстановки

		/// <summary>
		/// численность населения
		/// </summary>
		private StringBuilder ReplaceSection_Population(StringBuilder text)
		{
			DataTable table = Helper.GetDataTable("STAT_0005_0001_population_count");
			if (table.Rows.Count > 0)
			{
				int year = Convert.ToInt32(table.Rows[0][0].ToString());
				double value = Convert.ToDouble(table.Rows[0][1].ToString());

				text = text.
					Replace("@POPULATION_YEAR@", year.ToString()).
					Replace("@POPULATION_VALUE@", value.ToString("N1"));
			}
			return text;
		}

		/// <summary>
		/// родившиеся, умершие
		/// </summary>
		private StringBuilder ReplaceSection_Movement(StringBuilder text)
		{
			DataTable table = Helper.GetDataTable("STAT_0005_0001_movement_count");
			if (table.Rows.Count > 0)
			{
				int year = Convert.ToInt32(table.Rows[0][0].ToString());
				text = text.
					Replace("@MOVEMENT_YEAR@", year.ToString());

				int valueBirth;
				if (Int32.TryParse(table.Rows[0][1].ToString(), out valueBirth))
				{
					text = text.
						Replace("@MOVEMENT_BIRTH_VALUE@", valueBirth.ToString("N0")).
						Replace("@MOVEMENT_BIRTH_NOMI@", MultiNumEnding(nomiMany, valueBirth, "человек"));
				}
				else
				{
					text = text.Replace("@MOVEMENT_BIRTH_NOMI@", String.Empty);
				}

				int valueDead;
				if (Int32.TryParse(table.Rows[0][2].ToString(), out valueDead))
				{
					text = text.
						Replace("@MOVEMENT_DEAD_VALUE@", valueDead.ToString("N0")).
						Replace("@MOVEMENT_DEAD_NOMI@", MultiNumEnding(nomiMany, valueDead, "человек"));
				}
				else
				{
					text = text.Replace("@MOVEMENT_DEAD_NOMI@", String.Empty);
				}

			}
			return text;
		}

		/// <summary>
		/// прибывшие, выбывшие
		/// </summary>
		private StringBuilder ReplaceSection_Migration(StringBuilder text)
		{
			DataTable table = Helper.GetDataTable("STAT_0005_0001_migration_count");
			if (table.Rows.Count > 0)
			{
				int year = Convert.ToInt32(table.Rows[0][0].ToString());
				text = text.
					Replace("@MIGRATION_YEAR@", year.ToString());

				int valueArrival;
				if (Int32.TryParse(table.Rows[0][1].ToString(), out valueArrival))
				{
					text = text.
						Replace("@MIGRATION_ARRIVAL_VALUE@", valueArrival.ToString("N0")).
						Replace("@MIGRATION_ARRIVAL_NOMI@", MultiNumEnding(nomiMany, valueArrival, "человек"));
				}
				else
				{
					text = text.Replace("@MIGRATION_ARRIVAL_NOMI@", String.Empty);
				}

				int valueDepart;
				if (Int32.TryParse(table.Rows[0][2].ToString(), out valueDepart))
				{
					text = text.
						Replace("@MIGRATION_DEPART_VALUE@", valueDepart.ToString("N0")).
						Replace("@MIGRATION_DEPART_NOMI@", MultiNumEnding(nomiMany, valueDepart, "человек"));
				}
				else
				{
					text = text.Replace("@MOVEMENT_DEAD_NOMI@", String.Empty);
				}
			}
			return text;
		}

		#endregion

		#region методы стилизации
		
		private static HtmlGenericControl StyleCenter(HtmlGenericControl html)
		{
			html.Style.Add("text-align", "center");
			return html;
		}
		
		private static HtmlGenericControl StyleRight(HtmlGenericControl html)
		{
			html.Style.Add("text-align", "right");
			return html;
		}

		private static HtmlGenericControl StyleItem(HtmlGenericControl html)
		{
			html.Style.Add("margin", "5px 0");
			html.Style.Add("display", "inline-block");
			html.Style.Add("zoom", "1");
			html.Style.Add("*display", "inline");
			return html;
		}

		#endregion

		#region Методы нижнего уровня

		/// <summary>
		/// создает диаграмму
		/// </summary>
		private HtmlGenericControl AddChart(Type chartHelperClass)
		{
			chartsIndex++;
			ChartHelperBase chartHelper = (ChartHelperBase)Activator.CreateInstance(chartHelperClass);
			chartHelper.Init(chartsIndex);
			return chartHelper.GetItem();
		}

		/// <summary>
		/// создает таблицу
		/// </summary>
		private HtmlGenericControl AddGrid(Type gridHelperClass)
		{
			GridHelperBase gridHelper = (GridHelperBase)Activator.CreateInstance(gridHelperClass);
			gridHelper.Init((UltraGridBrick)Page.LoadControl("../../Components/UltraGridBrick.ascx"));
			return gridHelper.GetItem();
		}

		/// <summary>
		/// перегоняет строку в StringBuilder
		/// </summary>
		private static StringBuilder Text(string text)
		{
			return new StringBuilder(text);
		}
		
		/// <summary>
		/// создает простой div с текстом
		/// </summary>
		private HtmlGenericControl ToHTML(StringBuilder text)
		{
			HtmlGenericControl item = new HtmlGenericControl("div");
			item.InnerHtml = ReplaceGeneral(text.ToString());
			return item;
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
		/// заменяет основные параметры
		/// </summary>
		private string ReplaceGeneral(string text)
		{
			text = text.
				Replace("__", "&nbsp;").
				Replace("@PARAGRAPH@", String.Format("<img src=\"../../images/empty.gif\" width={0} height={1}>", PARAGRAPH_LENGTH, PARAGRAPH_HEIGHT));

			return
				Regex.Replace(text, "@[^@]*@", "<b style=\"color: red;\">&lt;нет данных&gt;</b>");
		}

		#endregion
		
		#region Диаграммы

		/// <summary>
		/// Диаграмма численности населения
		/// </summary>
		public class ChartHelperPopulation : ChartHelperBase
		{
			protected override void SetStyle()
			{
				base.SetStyle();
				Chart.Axis.Y.RangeType = AxisRangeType.Custom;
				Chart.Axis.Y.RangeMin = 1000;
				Chart.Axis.Y.RangeMax = 1600;
				Chart.TitleLeft.Text = "тыс. человек";
				Chart.Tooltips.FormatString = 
					"&nbsp;Численность населения на 1 января&nbsp;\n" +
					"&nbsp;<ITEM_LABEL> года  — <b><DATA_VALUE:N1></b> тыс. человек&nbsp;";
				
			}

			protected override void SetData()
			{
				base.SetData("STAT_0005_0001_population_count");
			}
		}

		/// <summary>
		/// Диаграмма естественного прироста населения
		/// </summary>
		public class ChartHelperMovement : ChartHelperBase
		{
			protected override void SetStyle()
			{
				base.SetStyle();
				Chart.TitleLeft.Text = "тыс. человек";
				Chart.Tooltips.FormatString =
					"&nbsp;Естественный прирост населения&nbsp;\n" +
					"&nbsp;за <ITEM_LABEL> год  — <b><DATA_VALUE:N1></b> тыс. человек&nbsp;";

			}

			protected override void SetData()
			{
				base.SetData("STAT_0005_0001_movement_chart");
			}
		}

		/// <summary>
		/// Диаграмма миграционного прироста населения
		/// </summary>
		public class ChartHelperMigration : ChartHelperBase
		{
			protected override void SetStyle()
			{
				base.SetStyle();
				Chart.TitleLeft.Text = "человек";
				Chart.Tooltips.FormatString =
					"&nbsp;Миграционный прирост населения&nbsp;\n" +
					"&nbsp;за <ITEM_LABEL> год  — <b><DATA_VALUE:N0></b> человек&nbsp;";

			}

			protected override void SetData()
			{
				base.SetData("STAT_0005_0001_migration_chart");
			}
		}

		/// <summary>
		/// Базовый класс для диаграмм
		/// </summary>
		public abstract class ChartHelperBase
		{
			protected static readonly Font defaultFont = new Font("Verdana", 9);

			public UltraChart Chart { protected set; get; }
			
			public virtual void Init(int chartID)
			{
				Chart = new UltraChart();
				Chart.DeploymentScenario.FilePath = "../../TemporaryImages";
				Chart.DeploymentScenario.ImageURL = String.Format("../../TemporaryImages/Chart_STAT_0005_0001{0}#SEQNUM(100).png", chartID);
				
				SetStyle();
				SetData();
			}
			
			protected virtual void SetStyle()
			{
				Chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.45);
				Chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.35);

				Chart.ChartType = ChartType.ColumnChart;

				Chart.EnableViewState = false;

				Chart.Data.ZeroAligned = true;
				Chart.Border.Thickness = 0;
				Chart.Tooltips.Font.Name = "Verdana";
				Chart.Tooltips.Font.Size = 9;

				Chart.Axis.X.Visible = true;
				Chart.Axis.X.LineThickness = 1;
				Chart.Axis.X.LineColor = Color.DarkGray;
				Chart.Axis.X.Labels.Font = defaultFont;
				Chart.Axis.X.Labels.SeriesLabels.Visible = false;
				Chart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
				Chart.Axis.X.Extent = 30;
				
				Chart.Axis.Y.Visible = true;
				Chart.Axis.Y.LineThickness = 1;
				Chart.Axis.Y.LineColor = Color.DarkGray;
				Chart.Axis.Y.Labels.Font = defaultFont;
				Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
				Chart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
				Chart.Axis.Y.Extent = 40;
				
				Chart.TitleLeft.Visible = true;
				Chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
				Chart.TitleLeft.Font = defaultFont;

				Chart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
			}

			protected abstract void SetData();

			protected virtual void SetData(string queryName)
			{
				DataTable table = Helper.GetDataTable(queryName);

				Chart.Series.Clear();
				Chart.Series.Add(CRHelper.GetNumericSeries(1, table));
			}

			public HtmlGenericControl GetItem()
			{
				HtmlGenericControl item = new HtmlGenericControl("div");
				item.Controls.Add(Chart);
				return item;
			}

		}

		#endregion

		#region Таблицы

		public class GridHelperTendencies : GridHelperBase
		{
			public override void SetStyle()
			{
				base.SetStyle();
				Grid.AutoSizeStyle = GridAutoSizeStyle.Auto;
				Grid.RedNegativeColoring = false;
			}

			protected override void InitializeLayout(object sender, LayoutEventArgs e)
			{
				e.Layout.RowAlternateStyleDefault.CopyFrom(e.Layout.RowStyleDefault);

				base.InitializeLayout(sender, e);
			}

			public override void SetData()
			{
				base.SetData("STAT_0005_0001_tendencies_grid");

				int columnsCount = Grid.DataTable.Columns.Count;
				foreach (DataRow row in Grid.DataTable.Rows)
				{
					if (!row[columnsCount - 2].ToString().Equals(String.Empty))
					{
						row[0] += ", " + row[columnsCount - 2];
					}
				}
			}

			public override void SetDataStyle()
			{
				Band.Columns[0].CellStyle.Wrap = true;
				Band.Columns[0].Width = CRHelper.GetColumnWidth(205);
				Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

				const int hiddenColumns = 3;
				Band.HideColumns(hiddenColumns);

				for (int i = 1; i < Band.Columns.Count - hiddenColumns; i++)
				{
					string caption = Band.Columns[i].Header.Caption;
					
					int width = 100;
					if (caption.ToLower().Contains("%"))
					{
						width = 80;
					}
					Band.Columns[i].Width = CRHelper.GetColumnWidth(width);

					string format = Helper.GetColumnFormat(caption, "N2");
					CRHelper.FormatNumberColumn(Band.Columns[i], format);
				}

			}

			public override void SetDataRules()
			{
				GrowRateRule upDownRule;
				upDownRule = new GrowRateRule(5);
				upDownRule.Limit = 1;
				upDownRule.InverseColumnName = "ТипПоказателяОбратный";
				upDownRule.IncreaseText = String.Empty;
				upDownRule.DecreaseText = String.Empty;
				Grid.AddIndicatorRule(upDownRule);

				upDownRule = new GrowRateRule(7);
				upDownRule.Limit = 1;
				upDownRule.InverseColumnName = "ТипПоказателяОбратный"; 
				upDownRule.IncreaseText = String.Empty;
				upDownRule.DecreaseText = String.Empty;
				Grid.AddIndicatorRule(upDownRule);


			}

			public override void SetDataHeader()
			{
				GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
				GridHeaderCell header1;
				GridHeaderCell header2;

				int extraIndex = Grid.DataTable.Columns.Count - 1;
				string year = String.Empty;
				string curMonth = String.Empty;

				MatchCollection matches = Regex.Matches(Grid.DataTable.Rows[0][extraIndex].ToString(), @"\[([^\]]*)\]");
				if (matches.Count > 3)
				{
					year = matches[3].Groups[1].Value;
				}
				if (matches.Count > 6)
				{
					curMonth = matches[6].Groups[1].Value.ToLower();
				}

				headerLayout.AddCell("Показатели");
				header1 = headerLayout.AddCell("Значение");
				header1.AddCell(String.Format("{0} год", Convert.ToInt32(year)-1)).AddCell(curMonth);
				header2 = header1.AddCell(String.Format("{0} год", year));
				header2.AddCell("январь");
				header2.AddCell(curMonth);
				header1 = headerLayout.AddCell("Динамика к предыдущему<br />отчетному периоду");
				header1.AddCell("Абсолютное отклонение");
				header1.AddCell("Темп роста");
				header1 = headerLayout.AddCell("Динамика за период<br />с начала года");
				header1.AddCell("Абсолютное отклонение");
				header1.AddCell("Темп роста");

				Grid.GridHeaderLayout.ApplyHeaderInfo();
			}

		}

		public abstract class GridHelperBase
		{
			public UltraGridBrick Grid { set; get; }
			public UltraGridBand Band { set; get; }
			
			public int DeleteColumns { set; get; }

			public abstract void SetDataStyle();
			public abstract void SetDataRules();
			public abstract void SetDataHeader();
			
			protected GridHelperBase()
			{
				DeleteColumns = 0;
			}

			public virtual void Init(UltraGridBrick grid)
			{
				Grid = grid;
				Grid.Grid.InitializeLayout += InitializeLayout;

				SetStyle();
				SetData();
			}

			public virtual void SetStyle()
			{
				Grid.EnableViewState = false;
			}

			public abstract void SetData();
			public virtual void SetData(string queryName)
			{
				DataTable dtGrid = Helper.GetDataTable(queryName);
				if (dtGrid.Rows.Count > 0)
				{
					dtGrid.DeleteColumns(DeleteColumns);
					Grid.DataTable = dtGrid;
				}
			}

			protected virtual void InitializeLayout(object sender, LayoutEventArgs e)
			{
				if (e.Layout.Bands[0].Columns.Count == 0)
				{
					return;
				}

				Band = e.Layout.Bands[0];
				SetDataStyle();
				SetDataRules();
				SetDataHeader();
			}

			public HtmlGenericControl GetItem()
			{
				HtmlGenericControl item = new HtmlGenericControl("div");
				item.Controls.Add(Grid);
				return item;
			}
		}
		
		#endregion

	}

	/// <summary>
	/// Класс расширений
	/// </summary>
	public static class Helper
	{
		/// <summary>
		/// получает таблицу по идентификатору запроса
		/// </summary>
		public static DataTable GetDataTable(string queryName)
		{
			DataTable table = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", table);
			return table;
		}

		/// <summary>
		/// удалить колонки в начале таблицы
		/// </summary>
		public static void DeleteColumns(this DataTable table, int count)
		{
			for (int i = 0; i < count; i++)
			{
				table.Columns.RemoveAt(0);
			}
		}

		/// <summary>
		/// скрыть колонки в конце таблицы
		/// </summary>
		public static void HideColumns(this UltraGridBand band, int count)
		{
			int columnsCount = band.Columns.Count;

			for (int i = 0; i < count; i++)
			{
				band.Columns[columnsCount - 1].Hidden = true;
				columnsCount--;
			}
		}

		/// <summary>
		/// Определяет формат кололнки по метасимволам
		/// </summary>
		public static string GetColumnFormat(string columnName, string defaultFormat)
		{
			if (columnName.ToLower().Contains("%"))
			{
				return "P2";
			}
			if (columnName.ToLower().Contains("#"))
			{
				return "N0";
			}
			return defaultFormat;
		}

	}
}
