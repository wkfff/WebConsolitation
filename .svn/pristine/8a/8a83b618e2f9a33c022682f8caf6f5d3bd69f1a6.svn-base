using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Krista.FM.Server.Dashboards.reports.SGM;
using System.Drawing;
using System.Data;
using System.Web;
using System;
using System.Configuration;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using System.Data.OleDb;

namespace Krista.FM.Server.Dashboards.SgmSupport
{
    // Область карты для вывода
    public enum MapKindEnum
    {
        SingleSubject = 0,
        SingleRegion = 1,
        AllRegions = 2
    }

    public class SGMDataRotator
    {
        // Имена таблиц класификаторов и данных
        public const string startYearInjections = "2009";
        // Имена таблиц класификаторов и данных
        public const string ClsPeopleGroup = "kont.dbf";
        public const string ClsArea = "area.dbf";
        public const string ClsDeseases = "dies.dbf";
        public const string ClsDeseasesFull = "dies_mem.dbf";
        public const string ClsAreaFull = "area_mem.dbf";

        public MapKindEnum mapKind;
        public Dictionary<string, string> regionSubstrSubjectIDs = new Dictionary<string, string>();
        public Collection<string> mapList = new Collection<string>();
        public Collection<string> fullMapList = new Collection<string>();

        public Collection<string> deseasesNames = new Collection<string>();
        public Collection<string> deseasesCodes = new Collection<string>();
        public Dictionary<string, string> deseasesRelation = new Dictionary<string, string>();

        public Collection<Color> deseasesColors = new Collection<Color>();
        public Dictionary<string, Color> deseasesColorRelation = new Dictionary<string, Color>();

        public Collection<string> deseasesLinks = new Collection<string>();
        public Dictionary<string, string> deseasesLinksRelation = new Dictionary<string, string>();

        public int formNumber = 1;
        public int factFormNumber;

        public const string allDeseasesCodeForm1 = "1,4,8,13,18,24,98,121,19,117,25,27,32,33,31,34,36,37,38,42,40,41,23,55,44,48,63,66,67,57,56,59,58,71,81,130,131,52,135";
        public const string allDeseasesCodeForm2 = "42,1,2,115,4,8,13,121,18,19,117,25,27,24,98,29,30,32,33,87,31,34,39,36,37,38,40,41,23,55,44,47,48,54,61,46,63,66,67,57,56,59,58,125,60,123,69,68,70,71,130,131,52,135";

        public double[] maxValues = new double[0];
        public double[] minValues = new double[0];

        public bool isSubjectReport;
        public string subjectCode = string.Empty;

        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMSQLTexts sqlTextClass = new SGMSQLTexts();

        public void FillRelationValues()
        {
            const string totalParam = "Общая сумма инф. заболеваний";
            deseasesRelation[totalParam] = allDeseasesCodeForm1;
            
            if (deseasesCodes.Count > 0)
            {
                deseasesCodes[0] = allDeseasesCodeForm1;
            }

            if (formNumber == 2)
            {
                if (deseasesCodes.Count > 0) deseasesCodes[0] = allDeseasesCodeForm2;
                deseasesRelation[totalParam] = allDeseasesCodeForm2;
            }
        }

        public void CheckFormNumber(int year, ref string months)
        {
            const char splitter = ',';
            formNumber = 2;
            var dtTemp = new DataTable();
            const string selectStr = "select count(dies) from {0}\\{1} where yr = {2}";
            ExecQuery(dtTemp, String.Format(selectStr, PathIllData, ClsDataTable, year));
            
            if (Convert.ToInt32(dtTemp.Rows[0][0]) == 0)
            {
                formNumber = 1;
                factFormNumber = 1;
                if (months.Length == 0 || months.Split(splitter).Length > 11) factFormNumber = 2;
                FillRelationValues();
                return;
            }

            formNumber = 1;
            if (months.Length == 0) formNumber = 2;
            if (months.Length > 0 && months.Split(splitter).Length > 11)
            {
                formNumber = 2;
                months = "0";
            }
            
            factFormNumber = formNumber;
            FillRelationValues();
        }

        public void CheckSubjectReport()
        {
            subjectCode = string.Empty;
            string[] parts = HttpContext.Current.Request.Url.ToString().Split('/');
            var urlParts = new Collection<string>(parts);
            isSubjectReport = urlParts.Contains("001");
            if (isSubjectReport) subjectCode = "001";
        }

        public string PathCommonData
        {
            get
            {
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["SGMBase"];
                
                if (setting == null)
                {
                    return String.Empty;
                }
                
                return String.Format(@"{0}", setting.ConnectionString);
            }
        }

        public string PathIllData
        {
            get
            {
                if (isSubjectReport)
                {
                    return string.Format(@"{0}\{1}\base2009", PathCommonData, subjectCode);
                }
                
                return string.Format(@"{0}\base2009", PathCommonData);
            }
        }

        public string PathClsData
        {
            get
            {
                if (isSubjectReport)
                {
                    return String.Format(@"{0}\{1}\spr1_2009", PathCommonData, subjectCode);
                }
                
                return String.Format(@"{0}\spr1_2009", PathCommonData);
            }
        }

        public string ClsDataTable
        {
            get
            {
                return String.Format(@"mill{0}.dbf", formNumber);
            }
        }

        public string ClsPopulation
        {
            get
            {
                return String.Format(@"nas{0}.dbf", formNumber);
            }
        }

        public string ClsPrivData
        {
            get
            {
                return String.Format(@"p_dop.dbf");
            }
        }

        public string ClsPrivYearData
        {
            get
            {
                return String.Format(@"c_val.dbf");
            }
        }

        public string GetDeseaseCodes(int index)
        {
            return deseasesCodes[index];
        }

        public void FillYearList(CustomMultiCombo ComboYear)
        {
            var excludedYears = new Collection<int>();
            FillYearListEx(ComboYear, excludedYears);
        }

        public Collection<string> FillYearListEx(CustomMultiCombo ComboYear, Collection<int> excludedYears)
        {
            var dtTemp = new DataTable();
            ExecQuery(dtTemp, GetYearSQLQueryText(true));
            DataRow lastRow = supportClass.GetLastRow(dtTemp);
            int lastCol = dtTemp.Columns.Count - 1;
            int startYear = Convert.ToInt32(lastRow[lastCol]);
            dtTemp = new DataTable();
            ExecQuery(dtTemp, GetYearSQLQueryText(false));
            lastRow = supportClass.GetLastRow(dtTemp);
            int endYear = Convert.ToInt32(lastRow[lastCol]);

            var listAllYears = new Collection<string>();
            for (int i = startYear; i < endYear + 1; i++)
            {
                listAllYears.Add(i.ToString());
            }

            var listYears = new Collection<string>();
            for (int i = startYear; i < endYear + 1; i++)
            {
                if (excludedYears.IndexOf(i) == -1) listYears.Add(i.ToString());
            }

            ComboYear.FillValues(listYears);
            ComboYear.SetСheckedState(listYears[listYears.Count - 1], true);

            return listAllYears;
        }

        public void FillMonthListEx(CustomMultiCombo ComboMonth, string year)
        {
            var dtTemp = new DataTable();
            ExecQuery(dtTemp, GetMonthSQLQueryText(year, true));
            DataRow lastRow = supportClass.GetLastRow(dtTemp);
            int lastCol = dtTemp.Columns.Count - 1;
            int startMonth = Convert.ToInt32(lastRow[lastCol]);
            dtTemp = new DataTable();
            ExecQuery(dtTemp, GetMonthSQLQueryText(year, false));
            lastRow = supportClass.GetLastRow(dtTemp);
            int endMonth = Convert.ToInt32(lastRow[lastCol]);
            ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());

            for (int i = startMonth; i < endMonth + 1; i++)
            {
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), true);
            }
        }

        public void FillDeseasesList(CustomMultiCombo ComboDeseases, int selectedIndex)
        {
            var valuesDictionary = new Dictionary<string, int>();
            deseasesCodes.Clear();
            deseasesCodes.Add(allDeseasesCodeForm1);
            //ОРВИ+Грипп
            deseasesCodes.Add("58,59");
            deseasesCodes.Add("59");
            deseasesCodes.Add("58");
            // Кишечные
            deseasesCodes.Add("1,20,4,8,13,18");
            deseasesCodes.Add("1");
            deseasesCodes.Add("4");
            deseasesCodes.Add("8");
            deseasesCodes.Add("13");
            deseasesCodes.Add("18");
            deseasesCodes.Add("20");
            //Воздушно-капельные
            deseasesCodes.Add("34,31,33,32,27,25,24");
            deseasesCodes.Add("24");
            deseasesCodes.Add("25");
            deseasesCodes.Add("27");
            deseasesCodes.Add("32");
            deseasesCodes.Add("33");
            deseasesCodes.Add("31");
            deseasesCodes.Add("34");
            //Социально-обусловленные            
            deseasesCodes.Add("63,57,56,67,66");
            deseasesCodes.Add("63");
            deseasesCodes.Add("66");
            deseasesCodes.Add("67");
            deseasesCodes.Add("57");
            deseasesCodes.Add("56");
            // Вирусные гепатиты парентеральные
            deseasesCodes.Add("21,94");
            deseasesCodes.Add("21");
            deseasesCodes.Add("94");
            // Природно-очаговые
            deseasesCodes.Add("55,23,41,40,42,36,48");
            deseasesCodes.Add("36");
            deseasesCodes.Add("42");
            deseasesCodes.Add("40");
            deseasesCodes.Add("41");
            deseasesCodes.Add("23");
            deseasesCodes.Add("55");
            deseasesCodes.Add("48");
            // Прочие инфекции
            deseasesCodes.Add("98,121,117,37,38,44,71,81, 2,3,115,116,29,30,87,39,47,54,61,46,125,60,123,69,68,70");
            deseasesCodes.Add("121");
            deseasesCodes.Add("117");
            deseasesCodes.Add("98");
            deseasesCodes.Add("37");
            deseasesCodes.Add("38");
            deseasesCodes.Add("44");
            deseasesCodes.Add("71");
            deseasesCodes.Add("81");
            deseasesCodes.Add("2");
            deseasesCodes.Add("3");
            deseasesCodes.Add("115");
            deseasesCodes.Add("116");
            deseasesCodes.Add("29");
            deseasesCodes.Add("30");
            deseasesCodes.Add("87");
            deseasesCodes.Add("39");
            deseasesCodes.Add("47");
            deseasesCodes.Add("54");
            deseasesCodes.Add("61");
            deseasesCodes.Add("46");
            deseasesCodes.Add("125");
            deseasesCodes.Add("60");
            deseasesCodes.Add("123");
            deseasesCodes.Add("69");
            deseasesCodes.Add("68");
            deseasesCodes.Add("70");
            deseasesCodes.Add("19");
            deseasesCodes.Add("130");
            deseasesCodes.Add("131");
            deseasesCodes.Add("52");
            deseasesCodes.Add("135");

            deseasesNames.Clear();
            deseasesNames.Add("Общая сумма инф. заболеваний");
            deseasesNames.Add("Грипп + ОРЗ");
            deseasesNames.Add("ОРЗ");
            deseasesNames.Add("Грипп");
            deseasesNames.Add("Кишечные инфекции");
            deseasesNames.Add("Брюшной тиф");
            deseasesNames.Add("Сальмонеллезы");
            deseasesNames.Add("Дизентерия");
            deseasesNames.Add("ОКИ установленной этиологии");
            deseasesNames.Add("ОКИ неустановленной этиологии");
            deseasesNames.Add("Острый ВГА");
            deseasesNames.Add("Воздушно-капельные инфекции");
            deseasesNames.Add("Полиомиелит острый");
            deseasesNames.Add("Дифтерия");
            deseasesNames.Add("Коклюш");
            deseasesNames.Add("Корь");
            deseasesNames.Add("Краснуха");
            deseasesNames.Add("Паротит эпидемический");
            deseasesNames.Add("Менингококковая инфекция");
            deseasesNames.Add("Социально-обусловленные инфекции");
            deseasesNames.Add("Туберкулез, активные формы");
            deseasesNames.Add("Сифилис");
            deseasesNames.Add("Гонококковая инфекция");
            deseasesNames.Add("Болезнь, вызванная вирусом иммунодефицита человека");
            deseasesNames.Add("Бессимптомный инфекционный статус, вызванный вирусом иммунодефицита человека");
            deseasesNames.Add("Вирусные гепатиты парентеральные");
            deseasesNames.Add("Острый ВГВ");
            deseasesNames.Add("Острый ВГС");
            deseasesNames.Add("Природно-очаговые инфекции");
            deseasesNames.Add("Туляремия");
            deseasesNames.Add("Геморрагическая лихорадка");
            deseasesNames.Add("Клещевой энцефалит");
            deseasesNames.Add("Болезнь Лайма");
            deseasesNames.Add("Псевдотуберкулез");
            deseasesNames.Add("Лептоспироз");
            deseasesNames.Add("Риккетсиозы");
            deseasesNames.Add("Прочие инфекции");
            deseasesNames.Add("Энтеровирусные инфекции");
            deseasesNames.Add("Хронические ВГ");
            deseasesNames.Add("Острые вялые параличи");
            deseasesNames.Add("Сибирская язва");
            deseasesNames.Add("Бруцеллез, впервые выявленный");
            deseasesNames.Add("Бешенство");
            deseasesNames.Add("Малярия");
            deseasesNames.Add("Трихинеллез");
            deseasesNames.Add("Паратифы А,B,C и неуточненный");
            deseasesNames.Add("Бакносители брюшного тифа и паратифов");
            deseasesNames.Add("Холера");
            deseasesNames.Add("Вибриононосители холеры");
            deseasesNames.Add("Скарлатина");
            deseasesNames.Add("Ветряная оспа");
            deseasesNames.Add("Синдром врожденной краснухи");
            deseasesNames.Add("Столбняк");
            deseasesNames.Add("Орнитоз");
            deseasesNames.Add("Листериоз");
            deseasesNames.Add("Легионеллез");
            deseasesNames.Add("Инфекционный мононуклеоз");
            deseasesNames.Add("Гемофильная инфекция");
            deseasesNames.Add("Цитомегаловирусная инфекция");
            deseasesNames.Add("Врожденная цитомегаловирусная инфекция");
            deseasesNames.Add("Микроспория");
            deseasesNames.Add("Чесотка");
            deseasesNames.Add("Трихофития");
            deseasesNames.Add("Острый ВГ");
            deseasesNames.Add("Лихорадка Западного Нила");
            deseasesNames.Add("Крымская  геморрагическая лихорадка");
            deseasesNames.Add("Сибирский клещевой тиф");
            deseasesNames.Add("Пневмония (внебольничная)");

            deseasesColors.Clear();
            deseasesColors.Add(Color.Red);
            //ОРВИ+Грипп
            deseasesColors.Add(Color.Honeydew);
            deseasesColors.Add(Color.SeaGreen);
            deseasesColors.Add(Color.PaleGreen);
            // Кишечные
            deseasesColors.Add(Color.PaleTurquoise);
            deseasesColors.Add(Color.Cyan);
            deseasesColors.Add(Color.DarkTurquoise);
            deseasesColors.Add(Color.CornflowerBlue);
            deseasesColors.Add(Color.LawnGreen);
            deseasesColors.Add(Color.Beige);
            deseasesColors.Add(Color.Moccasin);
            //Воздушно-капельны
            deseasesColors.Add(Color.SandyBrown);
            deseasesColors.Add(Color.LightCoral);
            deseasesColors.Add(Color.LightSalmon);
            deseasesColors.Add(Color.DarkKhaki);
            deseasesColors.Add(Color.Aquamarine);
            deseasesColors.Add(Color.CadetBlue);
            deseasesColors.Add(Color.SlateBlue);
            deseasesColors.Add(Color.Plum);
            //Социально-обусловые            
            deseasesColors.Add(Color.Crimson);
            deseasesColors.Add(Color.Magenta);
            deseasesColors.Add(Color.Thistle);
            deseasesColors.Add(Color.MediumPurple);
            deseasesColors.Add(Color.RoyalBlue);
            deseasesColors.Add(Color.Teal);
            // Вирусные гепатитрентеральные
            deseasesColors.Add(Color.MediumTurquoise);
            deseasesColors.Add(Color.MediumVioletRed);
            deseasesColors.Add(Color.AliceBlue);
            // Природно-очаговы
            deseasesColors.Add(Color.Beige);
            deseasesColors.Add(Color.MintCream);
            deseasesColors.Add(Color.MistyRose);
            deseasesColors.Add(Color.Moccasin);
            deseasesColors.Add(Color.NavajoWhite);
            deseasesColors.Add(Color.Olive);
            deseasesColors.Add(Color.Peru);
            deseasesColors.Add(Color.PowderBlue);
            // Прочие инфекции
            deseasesColors.Add(Color.Silver);
            deseasesColors.Add(Color.SaddleBrown);
            deseasesColors.Add(Color.SkyBlue);
            deseasesColors.Add(Color.Tan);
            deseasesColors.Add(Color.Yellow);
            deseasesColors.Add(Color.YellowGreen);
            deseasesColors.Add(Color.MediumOrchid);
            deseasesColors.Add(Color.MediumSeaGreen);
            deseasesColors.Add(Color.MediumSlateBlue);
            deseasesColors.Add(Color.MediumSpringGreen);
            deseasesColors.Add(Color.OliveDrab);
            deseasesColors.Add(Color.PapayaWhip);
            deseasesColors.Add(Color.PeachPuff);
            deseasesColors.Add(Color.Purple);
            deseasesColors.Add(Color.RosyBrown);
            deseasesColors.Add(Color.SlateGray);
            deseasesColors.Add(Color.LightYellow);
            deseasesColors.Add(Color.Lime);
            deseasesColors.Add(Color.MediumAquamarine);
            deseasesColors.Add(Color.Bisque);
            deseasesColors.Add(Color.OldLace);
            deseasesColors.Add(Color.PaleGoldenrod);
            deseasesColors.Add(Color.DodgerBlue);
            deseasesColors.Add(Color.ForestGreen);
            deseasesColors.Add(Color.Fuchsia);
            deseasesColors.Add(Color.Gold);
            deseasesColors.Add(Color.HotPink);
            deseasesColors.Add(Color.Khaki);
            deseasesColors.Add(Color.Lavender);
            deseasesColors.Add(Color.LightSeaGreen);
            deseasesColors.Add(Color.LightCyan);
            deseasesColors.Add(Color.LightSlateGray);

            deseasesLinks.Clear();
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Грипп;http://ru.wikipedia.org/wiki/Острая_респираторная_вирусная_инфекция");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Острая_респираторная_вирусная_инфекция");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Грипп");
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Брюшной_тиф");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Сальмонеллёзы");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Шигеллёз");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/МКБ-10:_Класс_I ;http://dic.academic.ru/dic.nsf/dic_microbiology/1845/Кишечные");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/МКБ-10:_Класс_I;http://dic.academic.ru/dic.nsf/dic_microbiology/1845/Кишечные");
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Полиомиелит");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Дифтерия");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Коклюш");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Корь");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Краснуха");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Паротит_эпидемический");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Менингококковая_инфекция");
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Туберкулёз");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Сифилис");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Гонорея");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Вирус_иммунодефицита_человека");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Вирус_иммунодефицита_человека");
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Туляремия");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Геморрагическая_лихорадка");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Клещевой_энцефалит");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Клещевой_бореллиоз");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Псевдотуберкулёз");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Лептоспироз");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Риккетсиозы");
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Энтеровирус;http://dic.academic.ru/dic.nsf/enc_medicine/35796/Энтеровирусные");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Вирусный_гепатит");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Полиомиелит");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Сибирская_язва");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Бруцеллёз");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Бешенство ");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Малярия");
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Трихинеллёз");
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add("http://ru.wikipedia.org/wiki/Вирусный_гепатит");
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);
            deseasesLinks.Add(String.Empty);

            deseasesRelation.Clear();
            for (int i = 0; i < deseasesNames.Count; i++)
            {
                deseasesRelation.Add(deseasesNames[i], deseasesCodes[i]);
            }

            deseasesColorRelation.Clear();
            for (int i = 0; i < deseasesColors.Count; i++)
            {
                deseasesColorRelation.Add(deseasesCodes[i], deseasesColors[i]);
            }

            deseasesLinksRelation.Clear();
            for (int i = 0; i < deseasesLinks.Count; i++)
            {
                deseasesLinksRelation.Add(deseasesCodes[i], deseasesLinks[i]);
            }

            for (int i = 0; i < deseasesNames.Count; i++)
            {
                int level = 1;
                if (deseasesCodes[i].Split(',').Length > 1) level = 0;
                valuesDictionary.Add(deseasesNames[i], level);
            }

            if (ComboDeseases != null)
            {
                ComboDeseases.FillDictionaryValues(valuesDictionary);
                if (selectedIndex != -1) ComboDeseases.SetСheckedState(deseasesNames[selectedIndex], true);
            }
        }

        /// <summary>
        /// Запрос выборки Min\Max года для заполнения списка параметров
        /// </summary>
        /// <param name="needMin">true- минимальны год, false-максимальный</param>
        /// <returns>текст запроса</returns>
        public string GetYearSQLQueryText(bool needMin)
        {
            string strFunction = (needMin) ? "Min" : "Max";
            return String.Format("select {0}(YR) from {1}\\{2} where yr > 0",
                strFunction, PathIllData, ClsDataTable);
        }

        /// <summary>
        /// Запрос выборки Min\Max месяцы для заполнения списка параметров
        /// </summary>
        /// <param name="year">год у которого тянем макс. месяц</param>
        /// <param name="needMin">true- минимальный месяц, false-максимальный</param>
        /// <returns>текст запроса</returns>
        public string GetMonthSQLQueryText(string year, bool needMin)
        {
            string strFunction = (needMin) ? "Min" : "Max";
            return String.Format("select {0}(Mon) from {1}\\{2} where yr in ({3})",
                strFunction, PathIllData, ClsDataTable, year);
        }

        /// <summary>
        ///  Строка подключения к БД
        /// </summary>
        /// <param name="path">Путь к БД</param>
        /// <returns></returns>
        public string GetConnectString(string path)
        {
            return String.Format("Provider='VFPOLEDB.1';Data Source={0} ;Mode='Read'", path);
        }

        /// <summary>
        /// Выполнятель запросов к БД
        /// </summary>
        /// <param name="dt">DataTable для загрузки данных(не чистим)</param>
        /// <param name="sql">Текст запроса к БД</param>
        public void ExecQuery(DataTable dt, string sql)
        {
            string Connectionstring = GetConnectString(PathCommonData);
            var con = new OleDbConnection(Connectionstring);
            con.Open();
            var cmd = new OleDbCommand(sql, con);
            var sda = new OleDbDataAdapter(cmd);
            
            if (dt == null)
            {
                dt = new DataTable();
            }
            
            sda.Fill(dt);
            con.Close();
        }

        public void ExecUpdate(string sql)
        {
            var conString = GetConnectString(PathCommonData);
            var con = new OleDbConnection(conString);
            con.Open();
            var updateCommand = new OleDbCommand(sql, con);
            updateCommand.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// Заполнить список территориями
        /// </summary>
        public void FillSGMMapList(CustomMultiCombo combo, DataTable dtRegions, bool addSubjects)
        {
            FillSGMMapListEx(combo, dtRegions, addSubjects, true);
        }

        public void FillSGMMapListOnlySubjects(CustomMultiCombo combo, DataTable dtRegions, bool addSubjects)
        {
            FillSGMMapListEx(combo, dtRegions, addSubjects, false);
        }

        public void FillSGMMapListEx(CustomMultiCombo combo, DataTable dtRegions, bool addSubjects, bool addRegions)
        {
            var valuesDictionary = new Dictionary<string, int>();
            const int subjectLevel = 1;
            mapList.Clear();
            fullMapList.Clear();
            valuesDictionary.Clear();
            DataRow[] rowRF = dtRegions.Select("(L_A1 = 1) and (COD <> 0) and (KOD = 999)");
            if (addRegions) valuesDictionary.Add(rowRF[0]["Name"].ToString(), 0);
            mapList.Add(rowRF[0]["Name"].ToString());

            var subjectAddList = new Collection<string>();

            DataRow[] rowsRegions = dtRegions.Select("(L_A1 = 1) and (COD <> 0) and (KOD <> 999)");
            foreach (DataRow rowRegion in rowsRegions)
            {
                if (!rowRegion["Name"].ToString().Contains("ЮЖН.+"))
                {
                    mapList.Add(rowRegion["Name"].ToString());
                    valuesDictionary.Add(mapList[mapList.Count - 1], 0);
                    if (addSubjects)
                    {
                        string[] subjectKeys = rowRegion["f_a2"].ToString().Split(',');
                        foreach (string subjectKey in subjectKeys)
                        {
                            if (subjectKey.Trim() != string.Empty)
                            {
                                DataRow[] subjectRow = dtRegions.Select(string.Format("(kod = {0}) and (L_A1 = 0)", subjectKey.Trim()));
                                if (subjectRow.Length > 0)
                                {
                                    if (!valuesDictionary.ContainsKey(subjectRow[0]["name"].ToString()))
                                    {
                                        valuesDictionary.Add(subjectRow[0]["name"].ToString(), subjectLevel);
                                        fullMapList.Add(subjectRow[0]["name"].ToString());
                                        subjectAddList.Add(subjectRow[0]["kod"].ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            DataRow[] rowsSubjects = dtRegions.Select("(L_A1 = 0) and (COD <> 0)");
            foreach (DataRow rowSubject in rowsSubjects)
            {
                string subjectKod = rowSubject["kod"].ToString();
                if (addSubjects && !subjectAddList.Contains(subjectKod))
                {
                    valuesDictionary.Add(rowSubject["name"].ToString(), 0);
                    fullMapList.Add(rowSubject["name"].ToString());
                }
            }

            if (combo != null)
            {
                combo.FillDictionaryValues(valuesDictionary);
                if (addSubjects) combo.SetСheckedState(fullMapList[0], true);
                if (addRegions) combo.SetСheckedState(mapList[0], true);
            }
        }


        public int GetLastMonth(string year)
        {
            var dtTemp = new DataTable();
            ExecQuery(dtTemp, GetMonthSQLQueryText(year, false));
            int endMonth = 0;
            DataRow lastRow = supportClass.GetLastRow(dtTemp);
            int lastColumn = dtTemp.Columns.Count - 1;

            if (dtTemp.Rows.Count > 0 && Convert.ToString(lastRow[lastColumn]).Length > 0)
            {
                endMonth = Convert.ToInt32(lastRow[lastColumn]);
            }

            return endMonth;
        }

        private int GetBoundYear(bool needMin)
        {
            var dtTemp = new DataTable();
            ExecQuery(dtTemp, GetYearSQLQueryText(needMin));
            DataRow lastRow = supportClass.GetLastRow(dtTemp);
            int firstYear = Convert.ToInt32(lastRow[dtTemp.Columns.Count - 1]);
            return firstYear;            
        }

        public int GetFirstYear()
        {
            return GetBoundYear(true);
        }

        public int GetLastYear()
        {
            return GetBoundYear(false);
        }

        public string GetMonthParamByYear(int year)
        {
            const char splitter = ',';
            int endMonth = GetLastMonth(year.ToString());
            string result = string.Empty;
            for (int i = 1; i < endMonth + 1; i++)
            {
                result = string.Format("{0}{1}{2}", result, splitter, i);
            }
            return result.Trim(splitter);
        }

        public string GetMonthParamIphone()
        {
            int endYear = GetLastYear();
            int endMonth = GetLastMonth(endYear.ToString());
            const char splitter = ',';

            string result = string.Empty;
            for (int i = 1; i < endMonth + 1; i++)
            {
                result = string.Format("{0}{1}{2}", result, splitter, i);
            }
            return result.Trim(splitter);
        }

        public string GetMonthParamString(CustomMultiCombo combo, string year)
        {
            string result = string.Empty;
            int endMonth = GetLastMonth(year);

            for (int i = 0; i < combo.SelectedValues.Count; i++)
            {
                if (CRHelper.MonthNum(combo.SelectedValues[i].ToLower()) <= endMonth)
                {
                    if (result.Length > 0)
                    {
                        result = String.Format("{0},", result);
                    }

                    result = String.Format("{0}{1}", result, CRHelper.MonthNum(combo.SelectedValues[i].ToLower()));
                }
            }
            return result;
        }

        public string GetMonthParamLabel(CustomMultiCombo combo, string year)
        {
            var usedMonth = new Collection<string>();
            int endMonth = GetLastMonth(year);
            
            if (endMonth == 12 && (combo.SelectedValues.Count == 0 || combo.SelectedValues.Count == endMonth))
            {
                return String.Empty;
            }

            for (int i = 0; i < combo.SelectedValues.Count; i++)
            {
                if (CRHelper.MonthNum(combo.SelectedValues[i].ToLower()) <= endMonth)
                {
                    usedMonth.Add(CRHelper.ToUpperFirstSymbol(combo.SelectedValues[i]));
                }
            }

            return supportClass.GetMonthLabel(combo, usedMonth);
        }

        public string GetDeseaseSqlList(CustomMultiCombo ComboDesease)
        {
            var strBuilder = new StringBuilder();
            for (int i = 0; i < ComboDesease.SelectedValues.Count; i++)
            {
                strBuilder.Append(deseasesRelation[ComboDesease.SelectedValues[i]]);
                if (i != ComboDesease.SelectedValues.Count - 1)
                {
                    strBuilder.Append(",");
                }
            }

            return strBuilder.ToString();
        }

        public void FillRegionsDictionary(DataTable dt)
        {
            regionSubstrSubjectIDs.Clear();
            foreach (DataRow dataRow in dt.Rows)
            {
                string value = Convert.ToString(dataRow[AREA.F_A2]).Trim().Trim(',').Trim();
                string regionName = Convert.ToString(dataRow[AREA.Name]);
                string regionChilds = value.Length > 0 ? value : Convert.ToString(dataRow[AREA.Kod]);
                regionSubstrSubjectIDs.Add(regionName, regionChilds);
            }
        }

        public string GetDeseaseName(string deseaseCode)
        {
            string sql = "select {2} from {0}\\{1} where {3} = {4}";
            sql = String.Format(sql, PathClsData, ClsDeseases, DIES.Name, DIES.Kod, deseaseCode);

            var dtTemp = new DataTable();
            ExecQuery(dtTemp, sql);
            
            if (dtTemp.Rows.Count > 0)
            {
                return Convert.ToString(dtTemp.Rows[0][0]).Trim();
            }

            return String.Empty;
        }

        public string GetFormHeader()
        {
            return String.Format(" <nobr>(по данным формы №{0})</nobr>", formNumber);
        }

        public string GetYearAppendix()
        {
            if (factFormNumber == 1)
            {
                return "а";
            }

            return String.Empty;
        }

        public void FillMaxAndMin(DataTable dt, int startIndex, bool calcRegions)
        {
            Array.Resize(ref maxValues, dt.Columns.Count);
            Array.Resize(ref minValues, dt.Columns.Count);

            for (int i = 0; i < minValues.Length; i++)
            {
                minValues[i] = Double.MaxValue;
                maxValues[i] = 0;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string regionName = Convert.ToString(dr[1]);
                bool isRegion = supportClass.GetFOShortName(regionName) == regionName;
                
                if (!calcRegions || isRegion)
                {
                    for (int j = startIndex; j < dt.Columns.Count; j++)
                    {
                        if (Convert.ToString(dr[j]).Length > 0)
                        {
                            maxValues[j] = Math.Max(maxValues[j], Convert.ToDouble(dr[j]));
                            minValues[j] = Math.Min(minValues[j], Convert.ToDouble(dr[j]));
                        }
                    }
                }
            }
        }

        public void FillMaxAndMin(DataTable dt, int startIndex)
        {
            FillMaxAndMin(dt, startIndex, false);
        }
    }
}
