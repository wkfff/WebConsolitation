using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core
{
    /// <summary>
    /// Воспомогательный класс, предоставляющий методы работы со словарями данных
    /// </summary>
    public static class DataDictionariesHelper
    {
        #region Уровни бюджетов

        /// <summary>
        /// Уровни бюджетов
        /// </summary>
        private static Dictionary<string, string> budgetLevelsTypes;

        /// <summary>
        /// Возвращает словарик уровней бюджетов
        /// </summary>
        public static Dictionary<string, string> BudgetLevelTypes
        {
            get
            {
                // если словарь пустой
                if (budgetLevelsTypes == null || budgetLevelsTypes.Count == 0)
                {
                    // заполняем его
                    FillBudgetLevelsTypes();
                }
                return budgetLevelsTypes;
            }
        }

        private static void FillBudgetLevelsTypes()
        {
            budgetLevelsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("budgetLevels", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Уровни бюджета", dt);

            // если не будет куба с расшеплением, то вернется пустой словарь
            foreach (DataRow row in dt.Rows)
            {
                budgetLevelsTypes.Add(row[0].ToString(), row[1].ToString());
            }
        }

        #endregion 

        #region Расходы ФКР

        /// <summary>
        /// Расходы ФКР
        /// </summary>
        private static Dictionary<string, string> outcomesFKRTypes;
        private static Dictionary<string, string> outcomesFKRLevels;

        /// <summary>
        /// Возвращает словарь наименований расходов ФКР
        /// </summary>
        public static Dictionary<string, string> OutcomesFKRTypes
        {
            get
            {
                // если словарь пустой
                if (outcomesFKRTypes == null || outcomesFKRTypes.Count == 0)
                {
                    // заполняем его
                    FillOutcomesFKR();
                }
                return outcomesFKRTypes;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней расходов ФКР
        /// </summary>
        public static Dictionary<string, string> OutcomesFKRLevels
        {
            get
            {
                // если словарь пустой
                if (outcomesFKRLevels == null || outcomesFKRLevels.Count == 0)
                {
                    // заполняем его
                    FillOutcomesFKR();
                }
                return outcomesFKRLevels;
            }
        }

        private static void FillOutcomesFKR()
        {
            outcomesFKRTypes = new Dictionary<string, string>();
            outcomesFKRLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("outcomesFKR", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Расходы РзПр", dt);

            foreach (DataRow row in dt.Rows)
            {
                outcomesFKRTypes.Add(row[0].ToString(), row[1].ToString());
                outcomesFKRLevels.Add(row[0].ToString(), row[2].ToString());
            }
        }

        #endregion 

        #region Расходы ФКР для ФО

        /// <summary>
        /// Расходы ФКР
        /// </summary>
        private static Dictionary<string, string> outcomesFOFKRTypes;
        private static Dictionary<string, string> outcomesFOFKRLevels;

        /// <summary>
        /// Возвращает словарь наименований расходов ФКР для ФО
        /// </summary>
        public static Dictionary<string, string> OutcomesFOFKRTypes
        {
            get
            {
                // если словарь пустой
                if (outcomesFOFKRTypes == null || outcomesFOFKRTypes.Count == 0)
                {
                    // заполняем его
                    FillOutcomesFOFKR();
                }
                return outcomesFOFKRTypes;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней расходов ФКР
        /// </summary>
        public static Dictionary<string, string> OutcomesFOFKRLevels
        {
            get
            {
                // если словарь пустой
                if (outcomesFOFKRLevels == null || outcomesFOFKRLevels.Count == 0)
                {
                    // заполняем его
                    FillOutcomesFOFKR();
                }
                return outcomesFOFKRLevels;
            }
        }

        private static void FillOutcomesFOFKR()
        {
            outcomesFOFKRTypes = new Dictionary<string, string>();
            outcomesFOFKRLevels = new Dictionary<string, string>();

            CustomParam.CustomParamFactory("fkr_dimension").Value = RegionSettingsHelper.Instance.FKRDimension;
            CustomParam.CustomParamFactory("fkr_all_level").Value = RegionSettingsHelper.Instance.FKRAllLevel;

            CustomParam.CustomParamFactory("rzpr_internal_circulation_extruding").Value = RegionSettingsHelper.Instance.RzPrInternalCircualtionExtruding;

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("outcomesFOFKR", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Расходы РзПр", dt);

            int multiplyValueCount = 4;
            foreach (DataRow row in dt.Rows)
            {
                string type = row[1].ToString();
                string level = row[2].ToString();

                type = RegionsNamingHelper.CheckMultiplyValue(type, multiplyValueCount);
                level = RegionsNamingHelper.CheckMultiplyValue(level, multiplyValueCount);

                outcomesFOFKRTypes.Add(GetDictionaryUniqueKey(outcomesFOFKRTypes, row[0].ToString()), type);
                outcomesFOFKRLevels.Add(GetDictionaryUniqueKey(outcomesFOFKRLevels, row[0].ToString()), level);
            }
        }

        #endregion 

        #region Расходы КОСГУ

        /// <summary>
        /// Расходы КОСГУ
        /// </summary>
        private static Dictionary<string, string> outcomesKOSGUTypes;
        private static Dictionary<string, string> outcomesKOSGULevels;

        /// <summary>
        /// Возвращает словарь наименований расходов КОСГУ
        /// </summary>
        public static Dictionary<string, string> OutcomesKOSGUTypes
        {
            get
            {
                // если словарь пустой
                if (outcomesKOSGUTypes == null || outcomesKOSGUTypes.Count == 0)
                {
                    // заполняем его
                    FillOutcomesKOSGU();
                }
                return outcomesKOSGUTypes;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней расходов КОСГУ
        /// </summary>
        public static Dictionary<string, string> OutcomesKOSGULevels
        {
            get
            {
                // если словарь пустой
                if (outcomesKOSGULevels == null || outcomesFKRLevels.Count == 0)
                {
                    // заполняем его
                    FillOutcomesKOSGU();
                }
                return outcomesKOSGULevels;
            }
        }

        private static void FillOutcomesKOSGU()
        {
            outcomesKOSGUTypes = new Dictionary<string, string>();
            outcomesKOSGULevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("outcomesKOSGU", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Расходы КОСГУ", dt);

            foreach (DataRow row in dt.Rows)
            {
                outcomesKOSGUTypes.Add(row[0].ToString(), row[1].ToString());
                outcomesKOSGULevels.Add(row[0].ToString(), row[2].ToString());
            }
        }

        #endregion 

        #region Краткие наименования ГРБС

        /// <summary>
        /// Краткие наименования ГРБС
        /// </summary>
        private static Dictionary<string, string> shortGRBSNames;
        /// <summary>
        /// Коды ГРБС
        /// </summary>
        private static Dictionary<string, string> shortGRBSCodes;

        /// <summary>
        /// Возвращает словарь кратких наименований ГРБС
        /// </summary>
        public static Dictionary<string, string> ShortGRBSNames
        {
            get
            {
                // если словарь пустой
                if (shortGRBSNames == null || shortGRBSNames.Count == 0)
                {
                    // заполняем его
                    FillShortGRBSNames();
                }
                return shortGRBSNames;
            }
        }

        /// <summary>
        /// Возвращает словарь кратких наименований ГРБС
        /// </summary>
        public static Dictionary<string, string> ShortGRBSCodes
        {
            get
            {
                // если словарь пустой
                if (shortGRBSCodes == null || shortGRBSCodes.Count == 0)
                {
                    // заполняем его
                    FillShortGRBSNames();
                }
                return shortGRBSCodes;
            }
        }

        private static void FillShortGRBSNames()
        {
            shortGRBSNames = new Dictionary<string, string>();
            shortGRBSCodes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("shortGRBSNames", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Краткие наименования ГРБС", dt);

            foreach (DataRow row in dt.Rows)
            {
                shortGRBSNames.Add(row[0].ToString(), row[1].ToString());
                shortGRBSCodes.Add(row[1].ToString(), row[2].ToString());
            }
        }

        public static string GetGRBSCodeByShortName(string shortName)
        {
            if (ShortGRBSCodes.ContainsKey(shortName))
            {
                return ShortGRBSCodes[shortName];
            }
            return shortName;
        }

        public static string GetGRBSCodeByFullName(string fullName)
        {
            string shortName = GetShortGRBSName(fullName);
            return GetGRBSCodeByShortName(shortName);
        }

        /// <summary>
        /// Получение краткого имени ГРБС по полному
        /// </summary>
        /// <param name="fullName">полное имя</param>
        /// <returns>краткое имя</returns>
        public static string GetShortGRBSName(string fullName)
        {
            if (ShortGRBSNames.ContainsKey(fullName))
            {
                return ShortGRBSNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// Получение полного имени ГРБС по краткому
        /// </summary>
        /// <param name="shortName">краткое имя</param>
        /// <returns>полное имя</returns>
        public static string GetFullGRBSName(string shortName)
        {
            if (ShortGRBSNames.ContainsValue(shortName))
            {
                foreach (string key in ShortGRBSNames.Keys)
                {
                    if (ShortGRBSNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region Непустые дни кассового плана

        /// <summary>
        /// Непустые дни кассового плана
        /// </summary>
        private static Dictionary<string, string> cashPlanNonEmptyDays;

        /// <summary>
        /// Возвращает словарь непустых дней кассового плана
        /// </summary>
        public static Dictionary<string, string> CashPlanNonEmptyDays
        {
            get
            {
                // если словарь пустой
                if (cashPlanNonEmptyDays == null || cashPlanNonEmptyDays.Count == 0)
                {
                    // заполняем его
                    FillCashPlanNonEmptyDays();
                }
                return cashPlanNonEmptyDays;
            }
        }

        private static void FillCashPlanNonEmptyDays()
        {
            cashPlanNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("cashPlanNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    cashPlanNonEmptyDays.Add(GetDictionaryUniqueKey(cashPlanNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    cashPlanNonEmptyDays.Add(GetDictionaryUniqueKey(cashPlanNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(cashPlanNonEmptyDays, row[4].ToString());
                cashPlanNonEmptyDays.Add(day, "Day");
            }
        }

        private static string GetDictionaryUniqueKey(Dictionary<string, string> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        #endregion 

        #region Краткие наименования КОСГУ

        /// <summary>
        /// Краткие наименования ГРБС
        /// </summary>
        private static Dictionary<string, string> shortKOSGUNames;

        /// <summary>
        /// Возвращает словарь кратких наименований КОСГУ
        /// </summary>
        public static Dictionary<string, string> ShortKOSGUNames
        {
            get
            {
                // если словарь пустой
                if (shortKOSGUNames == null || shortKOSGUNames.Count == 0)
                {
                    // заполняем его
                    FillShortKOSGUNames();
                }
                return shortKOSGUNames;
            }
        }

        private static void FillShortKOSGUNames()
        {
            shortKOSGUNames = new Dictionary<string, string>();
            shortKOSGUNames.Add("Заработная плата", "Заработная плата");
            shortKOSGUNames.Add("Прочие выплаты по заработной плате", "Прочие выплаты");
            shortKOSGUNames.Add("Начисления на выплаты по оплате труда", "Начисления по зарплате");
            shortKOSGUNames.Add("Услуги связи", "Услуги связи");
            shortKOSGUNames.Add("Транспортные услуги", "Транспортные услуги");
            shortKOSGUNames.Add("Арендная плата за пользование имуществом", "Аренда за имущество");
            shortKOSGUNames.Add("Работы, услуги по содержанию имущества", "Работы, услуги по содерж. имущества");
            shortKOSGUNames.Add("Прочие работы, услуги", "Прочие работы, услуги");
            shortKOSGUNames.Add("Обслуживание государственного (муниципального) долга", "Обслуживание гос. (мун) долга");
            shortKOSGUNames.Add("Безвозмездные перечисления организациям", "Безвозмездные перечисления орг-ям");
            shortKOSGUNames.Add("Социальное обеспечение", "Социальное обеспечение");
            shortKOSGUNames.Add("Прочие расходы", "Прочие расходы");
            shortKOSGUNames.Add("Увеличение стоимости основных средств", "Увеличение ст-ти осн. ср-в");
            shortKOSGUNames.Add("Увеличение стоимости нематериальных активов", "Увеличение ст-ти немат. активов");
            shortKOSGUNames.Add("Увеличение стоимости материальных запасов", "Увеличение ст-ти мат. запасов");
            shortKOSGUNames.Add("Увеличение стоимости акций и иных форм участия в капитале", "Увеличение ст-ти акций");
            shortKOSGUNames.Add("Увеличение стоимости непроизведенных активов", "Увеличение ст-ти непроизв. активов");
        }

        /// <summary>
        /// Получение краткого имени КОСГУ по полному
        /// </summary>
        /// <param name="fullName">полное имя</param>
        /// <returns>краткое имя</returns>
        public static string GetShortKOSGUName(string fullName)
        {
            if (ShortKOSGUNames.ContainsKey(fullName))
            {
                return ShortKOSGUNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// Получение полного имени КОСГУ по краткому
        /// </summary>
        /// <param name="shortName">краткое имя</param>
        /// <returns>полное имя</returns>
        public static string GetFullKOSGUName(string shortName)
        {
            if (ShortKOSGUNames.ContainsValue(shortName))
            {
                foreach (string key in ShortKOSGUNames.Keys)
                {
                    if (ShortKOSGUNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region Краткие наименования расходов и доходов бюджета

        /// <summary>
        /// Краткие наименования расходов и доходов бюджета
        /// </summary>
        private static Dictionary<string, string> shortBugetNames;

        /// <summary>
        /// Возвращает словарь кратких наименований расходов и доходов бюджета
        /// </summary>
        public static Dictionary<string, string> ShortBugetNames
        {
            get
            {
                // если словарь пустой
                if (shortBugetNames == null || shortBugetNames.Count == 0)
                {
                    // заполняем его
                    FillShortBugetNames();
                }
                return shortBugetNames;
            }
        }

        private static void FillShortBugetNames()
        {
            shortBugetNames = new Dictionary<string, string>();
            shortBugetNames.Add("НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ", "Налоговые и неналоговые доходы");
            shortBugetNames.Add("НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ", "Налоги на прибыль, доходы");
            shortBugetNames.Add("НАЛОГИ НА ТОВАРЫ (РАБОТЫ, УСЛУГИ), РЕАЛИЗУЕМЫЕ НА ТЕРРИТОРИИ РОССИЙСКОЙ ФЕДЕРАЦИИ", "Налоги на товары (работы, услуги)");
            shortBugetNames.Add("НАЛОГИ НА СОВОКУПНЫЙ ДОХОД", "Налоги на имущество");
            shortBugetNames.Add("НАЛОГИ НА ИМУЩЕСТВО", "Налоги на имущество");
            shortBugetNames.Add("НАЛОГИ, СБОРЫ И РЕГУЛЯРНЫЕ ПЛАТЕЖИ ЗА ПОЛЬЗОВАНИЕ ПРИРОДНЫМИ РЕСУРСАМИ", "Налоги и сборы за польз. природными ресурсами");
            shortBugetNames.Add("ГОСУДАРСТВЕННАЯ ПОШЛИНА", "Госуд. пошлина");
            shortBugetNames.Add("ЗАДОЛЖЕННОСТЬ И ПЕРЕРАСЧЕТЫ ПО ОТМЕНЕННЫМ НАЛОГАМ, СБОРАМ И ИНЫМ ОБЯЗАТЕЛЬНЫМ ПЛАТЕЖАМ", "Задолженность по отмененным налогам");
            shortBugetNames.Add("ДОХОДЫ ОТ ИСПОЛЬЗОВАНИЯ ИМУЩЕСТВА, НАХОДЯЩЕГОСЯ В ГОСУДАРСТВЕННОЙ И МУНИЦИПАЛЬНОЙ СОБСТВЕННОСТИ", "Доходы от использования госуд. и муницип. имущества");
            shortBugetNames.Add("ПЛАТЕЖИ ПРИ ПОЛЬЗОВАНИИ ПРИРОДНЫМИ РЕСУРСАМИ", "Платежи при польз. природными ресурсами");
            shortBugetNames.Add("ДОХОДЫ ОТ ОКАЗАНИЯ ПЛАТНЫХ УСЛУГ И КОМПЕНСАЦИИ ЗАТРАТ ГОСУДАРСТВА", "Доходы от оказания платных услуг");
            shortBugetNames.Add("ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ", "Доходы от продажи активов");
            shortBugetNames.Add("АДМИНИСТРАТИВНЫЕ ПЛАТЕЖИ И СБОРЫ", "Административные платежи и сборы");
            shortBugetNames.Add("ШТРАФЫ, САНКЦИИ, ВОЗМЕЩЕНИЕ УЩЕРБА", "Штрафы, санкции, возмещение ущерба");
            shortBugetNames.Add("ПРОЧИЕ НЕНАЛОГОВЫЕ ДОХОДЫ", "Прочие неналоговые доходы");
            shortBugetNames.Add("ДОХОДЫ БЮДЖЕТОВ БЮДЖЕТНОЙ СИСТЕМЫ РОССИЙСКОЙ ФЕДЕРАЦИИ ОТ ВОЗВРАТА ОСТАТКОВ СУБСИДИЙ И СУБВЕНЦИЙ ПРОШЛЫХ ЛЕТ", "Доходы от возврата остатков субсидий, субвенций прошлых лет");
            shortBugetNames.Add("ВОЗВРАТ ОСТАТКОВ СУБСИДИЙ И СУБВЕНЦИЙ ПРОШЛЫХ ЛЕТ", "Возврат остатков субсидий и субвенций прошлых лет");

            shortBugetNames.Add("ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ", "Общегосударств. вопросы");
            shortBugetNames.Add("НАЦИОНАЛЬНАЯ ОБОРОНА", "Национальная оборона");
            shortBugetNames.Add("НАЦИОНАЛЬНАЯ БЕЗОПАСНОСТЬ И ПРАВООХРАНИТЕЛЬНАЯ ДЕЯТЕЛЬНОСТЬ", "Национальная безопасность и правоохранит. деят.");
            shortBugetNames.Add("НАЦИОНАЛЬНАЯ ЭКОНОМИКА", "Национальная экономика");
            shortBugetNames.Add("ЖИЛИЩНО-КОММУНАЛЬНОЕ ХОЗЯЙСТВО", "ЖКХ");
            shortBugetNames.Add("ОХРАНА ОКРУЖАЮЩЕЙ СРЕДЫ", "Охрана окружающей среды");
            shortBugetNames.Add("ОБРАЗОВАНИЕ", "Образование");
            shortBugetNames.Add("КУЛЬТУРА, КИНЕМАТОГРАФИЯ, СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ", "Культура, кинематография, СМИ");
            shortBugetNames.Add("ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ", "Здравоохранение, физкультура и спорт");
            shortBugetNames.Add("СОЦИАЛЬНАЯ ПОЛИТИКА", "Социальная политика");
            shortBugetNames.Add("МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ", "Межбюджетные трансферты");
        }

        /// <summary>
        /// Получение краткого имени расходов и доходов бюджета по полному
        /// </summary>
        /// <param name="fullName">полное имя</param>
        /// <returns>краткое имя</returns>
        public static string GetShortBugetName(string fullName)
        {
            if (ShortBugetNames.ContainsKey(fullName))
            {
                return ShortBugetNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// Получение полного имени расходов и доходов бюджета по краткому
        /// </summary>
        /// <param name="shortName">краткое имя</param>
        /// <returns>полное имя</returns>
        public static string GetFullBugetName(string shortName)
        {
            if (ShortBugetNames.ContainsValue(shortName))
            {
                foreach (string key in ShortBugetNames.Keys)
                {
                    if (ShortBugetNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region Краткие наименования КД

        /// <summary>
        /// Краткие наименования КД
        /// </summary>
        private static Dictionary<string, string> shortKDNames;

        /// <summary>
        /// Возвращает словарь кратких наименований КД
        /// </summary>
        public static Dictionary<string, string> ShortKDNames
        {
            get
            {
                // если словарь пустой
                if (shortKDNames == null || shortKDNames.Count == 0)
                {
                    // заполняем его
                    FillShortKDNames();
                }
                return shortKDNames;
            }
        }

        private static void FillShortKDNames()
        {
            shortKDNames = new Dictionary<string, string>();
            shortKDNames.Add("Налог на прибыль организаций", "Налог на прибыль");
            shortKDNames.Add("Налог на доходы физических лиц", "НДФЛ");
            shortKDNames.Add("Налог на имущество физических лиц", "НИФЛ");
            shortKDNames.Add("Налог, взимаемый в связи с упрощенной системой", "УСН");
            shortKDNames.Add("Налог, взимаемый в связи с применением упрощенной системы налогообложения", "УСН");
            shortKDNames.Add("Единый налог на вмененный доход", "ЕНВД");
            shortKDNames.Add("Единый налог на вмененный доход для отдельных видов деятельности", "ЕНВД");
            shortKDNames.Add("Налог на имущество организаций", "НИО");
            shortKDNames.Add("Доходы от использования имущества", "Доходы от исп. имущества");
            shortKDNames.Add("Плата за использование лесов", "Плата за исп. лесов");
            shortKDNames.Add("Доходы от продажи имущества", "Доходы от прод. имущества");
            shortKDNames.Add("Налог на добычу полезных ископаемых", "НДПИ");
            shortKDNames.Add("Единый сельскохозяйственный налог", "ЕСХН");
            shortKDNames.Add("Государственная пошлина", "Гос.пошлина");
            shortKDNames.Add("Задолженность и перерасчеты по отмененным налогам, сборам и иным обязательным платежам", "Задолженность по отмененным налогам");
        }

        /// <summary>
        /// Получение краткого имени КД по полному
        /// </summary>
        /// <param name="fullName">полное имя</param>
        /// <returns>краткое имя</returns>
        public static string GetShortKDName(string fullName)
        {
            if (ShortKDNames.ContainsKey(fullName))
            {
                return ShortKDNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// Получение полного имени КД по краткому
        /// </summary>
        /// <param name="shortName">краткое имя</param>
        /// <returns>полное имя</returns>
        public static string GetFullKDName(string shortName)
        {
            if (ShortKDNames.ContainsValue(shortName))
            {
                foreach (string key in ShortKDNames.Keys)
                {
                    if (ShortKDNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region Непустые дни оперативной информации по исполнению бюджета субъекта

        /// <summary>
        /// Непустые дни оперативной информации
        /// </summary>
        private static Dictionary<string, string> hotInfoNonEmptyDays;

        /// <summary>
        /// Возвращает словарь непустых дней оперативной информации
        /// </summary>
        public static Dictionary<string, string> HotInfoNonEmptyDays
        {
            get
            {
                // если словарь пустой
                if (hotInfoNonEmptyDays == null || hotInfoNonEmptyDays.Count == 0)
                {
                    // заполняем его
                    FillHotInfoNonEmptyDays();
                }
                return hotInfoNonEmptyDays;
            }
        }

        private static void FillHotInfoNonEmptyDays()
        {
            hotInfoNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("hotInfoNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    hotInfoNonEmptyDays.Add(GetDictionaryUniqueKey(hotInfoNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    hotInfoNonEmptyDays.Add(GetDictionaryUniqueKey(hotInfoNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(hotInfoNonEmptyDays, row[4].ToString());
                hotInfoNonEmptyDays.Add(day, "Day");
            }
        }

        #endregion 

        #region Непустые месяцы оперативной информации по исполнению бюджета субъекта

        /// <summary>
        /// Непустые месяцы оперативной информации
        /// </summary>
        private static Dictionary<string, string> hotInfoNonEmptyMonths;

        /// <summary>
        /// Возвращает словарь непустых месяцев оперативной информации
        /// </summary>
        public static Dictionary<string, string> HotInfoNonEmptyMonths
        {
            get
            {
                // если словарь пустой
                if (hotInfoNonEmptyMonths == null || hotInfoNonEmptyMonths.Count == 0)
                {
                    // заполняем его
                    FillHotInfoNonEmptyMonths();
                }
                return hotInfoNonEmptyMonths;
            }
        }

        private static void FillHotInfoNonEmptyMonths()
        {
            hotInfoNonEmptyMonths = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("hotInfoNonEmptyMonths", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    hotInfoNonEmptyMonths.Add(GetDictionaryUniqueKey(hotInfoNonEmptyMonths, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    hotInfoNonEmptyMonths.Add(GetDictionaryUniqueKey(hotInfoNonEmptyMonths, curMonth), "Month");
                    month = curMonth;
                }
            }
        }

        #endregion 

        #region Непустые дни мониторинга рынка труда

        /// <summary>
        /// Непустые дни мониторинга рынка труда
        /// </summary>
        private static Dictionary<string, string> labourMarketNonEmptyDays;

        /// <summary>
        /// Возвращает словарь непустых дней мониторинга рынка труда
        /// </summary>
        public static Dictionary<string, string> LabourMarketNonEmptyDays
        {
            get
            {
                // если словарь пустой
                if (labourMarketNonEmptyDays == null || labourMarketNonEmptyDays.Count == 0)
                {
                    // заполняем его
                    FillLabourMarketNonEmptyDays();
                }
                return labourMarketNonEmptyDays;
            }
        }

        private static void FillLabourMarketNonEmptyDays()
        {
            labourMarketNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("labourMarketNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    labourMarketNonEmptyDays.Add(GetDictionaryUniqueKey(labourMarketNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    labourMarketNonEmptyDays.Add(GetDictionaryUniqueKey(labourMarketNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(labourMarketNonEmptyDays, row[4].ToString());
                labourMarketNonEmptyDays.Add(day, "Day");
            }
        }

        #endregion 
        private static Dictionary<string, string> creditsNonEmptyDays;
        public static Dictionary<string, string> CreditsNonEmptyDays
        {
            get
            {
                // если словарь пустой
                if (creditsNonEmptyDays == null || creditsNonEmptyDays.Count == 0)
                {
                    // заполняем его
                    FillCreditsNonEmptyDays();
                }
                return creditsNonEmptyDays;
            }
        }

        private static void FillCreditsNonEmptyDays()
        {
            creditsNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("creditsNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    creditsNonEmptyDays.Add(GetDictionaryUniqueKey(creditsNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    creditsNonEmptyDays.Add(GetDictionaryUniqueKey(creditsNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(creditsNonEmptyDays, row[4].ToString());
                creditsNonEmptyDays.Add(day, "Day");
            }
        }


        #region Номера показателей мониторинга рынка труда

       

        private static Dictionary<string, string> indicatorsTypes;
        
        public static Dictionary<string, string> IndicatorsTypes
        {
        get
            {
                // если словарь пустой
                if (indicatorsTypes == null || indicatorsTypes.Count == 0)
                {
                    // заполняем его
                    FillIndicatorsTypes();
                }
                return indicatorsTypes;
            }
        }

        private static Dictionary<string, string> okvedTypes;
        public static Dictionary<string, string> OKVEDTypes
        {
            get
            {
                // если словарь пустой
                if (okvedTypes == null || okvedTypes.Count == 0)
                {
                    // заполняем его
                    FillOKVEDTypes();
                }
                return okvedTypes;
            }
        }
        private static Dictionary<string, string> kindsTypes;

        public static Dictionary<string, string> KindsTypes
        {
            get
            {
                // если словарь пустой
                if (kindsTypes == null || KindsTypes.Count == 0)
                {
                    // заполняем его
                    FillKindsTypes();
                }
                return kindsTypes;
            }
        }

        /// <summary>
        /// Номера показателей мониторинга рынка труда
        /// </summary>
        private static Dictionary<string, string> labourMarketIndicatorNumbers;
        /// <summary>
        /// Возвращает словарь номеров показателей мониторинга рынка труда
        /// </summary>
        public static Dictionary<string, string> LabourMarketIndicatorNumbers
        {
            get
            {
                // если словарь пустой
                if (labourMarketIndicatorNumbers == null || labourMarketIndicatorNumbers.Count == 0)
                {
                    // заполняем его
                    FillLabourMarketIndicatorNumbers();
                }
                return labourMarketIndicatorNumbers;
            }
        }

        private static void FillLabourMarketIndicatorNumbers()
        {
            labourMarketIndicatorNumbers = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("labourMarketIndicatorNumbers", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(row[1].ToString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out value))
                    {
                        labourMarketIndicatorNumbers.Add(row[0].ToString(), string.Format("{0:N0}", value));
                    }
                    else
                    {
                        labourMarketIndicatorNumbers.Add(row[0].ToString(),row[1].ToString());
                    }
                }
            }
        }

        public static string GetLabourMarketIndicatorNumber(string indicatorName)
        {
            if (LabourMarketIndicatorNumbers.ContainsKey(indicatorName))
            {
                return LabourMarketIndicatorNumbers[indicatorName];
            }
            else if (indicatorName.Split('_').Length > 0)
            {
                string indName = indicatorName.Split('_')[0];
                if (LabourMarketIndicatorNumbers.ContainsKey(indName))
                {
                    return LabourMarketIndicatorNumbers[indName];
                }
            }

            return "0";
        }

        #endregion 

        #region Показатели мониторинга рынка труда

        /// <summary>
        /// Уровни бюджетов
        /// </summary>
        private static Dictionary<string, string> labourMarketIndicatorsTypes;

        /// <summary>
        /// Возвращает словарик уровней бюджетов
        /// </summary>
        public static Dictionary<string, string> LabourMarketIndicatorsTypes
        {
            get
            {
                // если словарь пустой
                if (labourMarketIndicatorsTypes == null || labourMarketIndicatorsTypes.Count == 0)
                {
                    // заполняем его
                    FillLabourMarketIndicatorsTypes();
                }
                return labourMarketIndicatorsTypes;
            }
        }

        private static void FillLabourMarketIndicatorsTypes()
        {
            labourMarketIndicatorsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("labourMarketIndicators", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);

            // если не будет куба с расшеплением, то вернется пустой словарь
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "Все" &&
                    row[0].ToString() != "Численность экономически активного населения")
                {
                    labourMarketIndicatorsTypes.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        private static void FillIndicatorsTypes()
        {
            indicatorsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("Indicators", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);

            // если не будет куба с расшеплением, то вернется пустой словарь
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "Все" &&
                    row[0].ToString() != "Численность экономически активного населения")
                {
                    indicatorsTypes.Add(String.Format("{0} {1}", row[1], row[0]), row[2].ToString());
                }
            }
        }

        private static void FillOKVEDTypes()
        {
            okvedTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("OKVEDList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);

            // если не будет куба с расшеплением, то вернется пустой словарь
            foreach (DataRow row in dt.Rows)
            {
                
                    okvedTypes.Add(String.Format("{0}", row[0]), row[1].ToString());
            
            }
        }

        public static string GetIndicator(string Indicator)
        { 
            if (IndicatorsTypes.ContainsValue(Indicator))
            {
                foreach (string key in IndicatorsTypes.Keys)
                {
                    if (IndicatorsTypes[key].Contains(Indicator))
                    {
                        return key;
                    }
                }
            }
            return "";
        }

        private static void FillKindsTypes()
        {
            kindsTypes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("Kinds", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Виды", dt);

            // если не будет куба с расшеплением, то вернется пустой словарь
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "Все" &&
                    row[0].ToString() != "Численность экономически активного населения")
                {
                    kindsTypes.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region Список субъектов федерального округа

        /// <summary>
        ///  Список субъектов федерального округа
        /// </summary>
        private static Dictionary<string, string> foSubjectList;

        /// <summary>
        /// Возвращает словарик субъектов федерального округа
        /// </summary>
        public static Dictionary<string, string> FOSubjectList
        {
            get
            {
                // если словарь пустой
                if (foSubjectList == null || foSubjectList.Count == 0)
                {
                    // заполняем его
                    FillFOSubjectList();
                }
                return foSubjectList;
            }
        }

        private static void FillFOSubjectList()
        {
            foSubjectList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FOSubjectList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъекты", dt);

            foreach (DataRow row in dt.Rows)
            {
                foSubjectList.Add(row[0].ToString(), row[1].ToString());
            }
        }

        #endregion 

        #region Администраторы МБТ с субсидиями

        /// <summary>
        /// Администраторы МБТ
        /// </summary>
        private static Dictionary<string, string> mbtAdministratorsDetailUniqNames;
        private static Dictionary<string, string> mbtAdministratorsDetailLevels;

        /// <summary>
        /// Возвращает словарь наименований администраторов МБТ
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsDetailUniqNames
        {
            get
            {
                // если словарь пустой
                if (mbtAdministratorsDetailUniqNames == null || mbtAdministratorsDetailUniqNames.Count == 0)
                {
                    // заполняем его
                    FillMBTAdministratorsDetail();
                }
                return mbtAdministratorsDetailUniqNames;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней администраторов МБТ
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsDetailLevels
        {
            get
            {
                // если словарь пустой
                if (mbtAdministratorsDetailLevels == null || mbtAdministratorsDetailLevels.Count == 0)
                {
                    // заполняем его
                    FillMBTAdministratorsDetail();
                }
                return mbtAdministratorsDetailLevels;
            }
        }

        private static void FillMBTAdministratorsDetail()
        {
            mbtAdministratorsDetailUniqNames = new Dictionary<string, string>();
            mbtAdministratorsDetailLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("MBTAdministratorDetailList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Администраторы МБТ", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string key = row[0].ToString();
                    if (key.ToLower().Contains("субсидии"))
                    {
                        key = "Субсидии";
                    }
                    else if (key.ToLower().Contains("дотации"))
                    {
                        key = "Дотации";
                    }
                    else if (key.ToLower().Contains("субвенции"))
                    {
                        key = "Субвенции";
                    }
                    else if (key.ToLower().Contains("иные"))
                    {
                        key = "Иные";
                    }
                    key = GetDictionaryUniqueKey(mbtAdministratorsDetailUniqNames, key);
                    mbtAdministratorsDetailUniqNames.Add(key, row[1].ToString());
                    mbtAdministratorsDetailLevels.Add(key, row[2].ToString());
                }
            }
        }

        #endregion 

        #region Администраторы МБТ без субсидий

        /// <summary>
        /// Администраторы МБТ
        /// </summary>
        private static Dictionary<string, string> mbtAdministratorsUniqNames;
        private static Dictionary<string, string> mbtAdministratorsLevels;

        /// <summary>
        /// Возвращает словарь наименований администраторов МБТ
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsUniqNames
        {
            get
            {
                // если словарь пустой
                if (mbtAdministratorsUniqNames == null || mbtAdministratorsUniqNames.Count == 0)
                {
                    // заполняем его
                    FillMBTAdministrators();
                }
                return mbtAdministratorsUniqNames;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней администраторов МБТ
        /// </summary>
        public static Dictionary<string, string> MBTAdministratorsLevels
        {
            get
            {
                // если словарь пустой
                if (mbtAdministratorsLevels == null || mbtAdministratorsLevels.Count == 0)
                {
                    // заполняем его
                    FillMBTAdministrators();
                }
                return mbtAdministratorsLevels;
            }
        }

        private static void FillMBTAdministrators()
        {
            mbtAdministratorsUniqNames = new Dictionary<string, string>();
            mbtAdministratorsLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("MBTAdministratorList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Администраторы МБТ", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string key = row[0].ToString();
                    key = GetDictionaryUniqueKey(mbtAdministratorsUniqNames, key);
                    mbtAdministratorsUniqNames.Add(key, row[1].ToString());
                    mbtAdministratorsLevels.Add(key, row[2].ToString());
                }
            }
        }

        #endregion 

        #region Полный список уровней бюджетов

        /// <summary>
        /// Номера уровней бюджета
        /// </summary>
        private static Dictionary<string, string> fullBudgetLevelNumbers;
        /// <summary>
        /// Уникальные имена уровней бюджета
        /// </summary>
        private static Dictionary<string, string> fullBudgetLevelUniqNames;
        /// <summary>
        /// Уникальные имена районов
        /// </summary>
        private static Dictionary<string, string> fullBudgetRegionUniqNames;

        /// <summary>
        /// Возвращает словарь номеров уровней бюджетов
        /// </summary>
        public static Dictionary<string, string> FullBudgetLevelNumbers
        {
            get
            {
                // если словарь пустой
                if (fullBudgetLevelNumbers == null || fullBudgetLevelNumbers.Count == 0)
                {
                    // заполняем его
                    FillFullBudgetLevels();
                }
                return fullBudgetLevelNumbers;
            }
        }

        /// <summary>
        /// Возвращает словарь уникальных имен уровней бюджетов
        /// </summary>
        public static Dictionary<string, string> FullBudgetLevelUniqNames
        {
            get
            {
                // если словарь пустой
                if (fullBudgetLevelUniqNames == null || fullBudgetLevelUniqNames.Count == 0)
                {
                    // заполняем его
                    FillFullBudgetLevels();
                }
                return fullBudgetLevelUniqNames;
            }
        }

        /// <summary>
        /// Возвращает словарь уникальных имен районов
        /// </summary>
        public static Dictionary<string, string> FullBudgetRegionUniqNames
        {
            get
            {
                // если словарь пустой
                if (fullBudgetRegionUniqNames == null || fullBudgetRegionUniqNames.Count == 0)
                {
                    // заполняем его
                    FillFullBudgetLevels();
                }
                return fullBudgetRegionUniqNames;
            }
        }

        private static void FillFullBudgetLevels()
        {
            fullBudgetLevelNumbers = new Dictionary<string, string>();
            fullBudgetLevelUniqNames = new Dictionary<string, string>();
            fullBudgetRegionUniqNames = new Dictionary<string, string>();

            CustomParam.CustomParamFactory("localSettlementLevelName").Value = RegionSettingsHelper.Instance.SettlementLevel;
            CustomParam.CustomParamFactory("localLevelName").Value = RegionSettingsHelper.Instance.RegionsLevel;
            CustomParam.CustomParamFactory("fns_district_budget_level").Value = RegionSettingsHelper.Instance.FNSDistrictBudgetLevel;

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("fullBudgetLevels", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Уровни бюджета", dt);

            // если не будет куба с расшеплением, то вернется пустой словарь
            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    string uniqKey = GetDictionaryUniqueKey(fullBudgetLevelNumbers, row[0].ToString());
                    fullBudgetLevelNumbers.Add(uniqKey, row[1].ToString());
                }
                if (row[2] != DBNull.Value)
                {
                    string uniqKey = GetDictionaryUniqueKey(fullBudgetLevelUniqNames, row[0].ToString());
                    fullBudgetLevelUniqNames.Add(uniqKey, row[2].ToString());
                }
                if (row[3] != DBNull.Value)
                {
                    string uniqKey = GetDictionaryUniqueKey(fullBudgetRegionUniqNames, row[0].ToString());
                    fullBudgetRegionUniqNames.Add(uniqKey, row[3].ToString());
                }
            }
        }

        #endregion 

        #region Краткие наименования ОКВЭД

        /// <summary>
        /// Краткие наименования ОКВЭД
        /// </summary>
        private static Dictionary<string, string> shortOKVDNames;

        /// <summary>
        /// Возвращает словарь кратких наименований ОКВЭД
        /// </summary>
        public static Dictionary<string, string> ShortOKVDNames
        {
            get
            {
                // если словарь пустой
                if (shortOKVDNames == null || shortOKVDNames.Count == 0)
                {
                    // заполняем его
                    FillShortOKVDNames();
                }
                return shortOKVDNames;
            }
        }

        private static void FillShortOKVDNames()
        {
            shortOKVDNames = new Dictionary<string, string>();
            shortOKVDNames.Add("Сельское хозяйство, охота и лесное хозяйство", "С/Х, охота, Л/Х");
            shortOKVDNames.Add("Добыча полезных ископаемых", "Добыча ПИ");
            shortOKVDNames.Add("Производство и распределение электроэнергии, газа и воды", "Э\\э, газ, вода");
            shortOKVDNames.Add("Оптовая и розничная торговля; ремонт автотранспортных средств, мотоциклов, бытовых изделий и предметов личного пользования", "Торговля, ремонт");
            shortOKVDNames.Add("Операции с недвижимым имуществом, аренда и предоставление услуг", "Недвижимость");
            shortOKVDNames.Add("Государственное управление и обеспечение военной безопасности; обязательное социальное обеспечение", "Гос. управление, без-сть; соц. страх."); 
            shortOKVDNames.Add("Государственное управление и обеспечение военной безопасности; социальное страхование", "Гос. управление, без-сть; соц. страх.");
            shortOKVDNames.Add("Здравоохранение и предоставление социальных услуг", "Здрав. и соц.услуги");
            shortOKVDNames.Add("Предоставление прочих коммунальных, социальных и персональных услуг, Деятельность домашних хозяйств, Деятельность экстерриториальных организаций", "Прочие отрасли");
            shortOKVDNames.Add("Предоставление прочих коммунальных, социальных и персональных услуг", "Прочие комм., соц. и перс.услуги");
        }

        /// <summary>
        /// Получение краткого имени ОКВЭД по полному
        /// </summary>
        /// <param name="fullName">полное имя</param>
        /// <returns>краткое имя</returns>
        public static string GetShortOKVDName(string fullName)
        {
            if (ShortOKVDNames.ContainsKey(fullName))
            {
                return ShortOKVDNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// Получение полного имени ОКВЭД по краткому
        /// </summary>
        /// <param name="shortName">краткое имя</param>
        /// <returns>полное имя</returns>
        public static string GetFullOKVDName(string shortName)
        {
            if (ShortOKVDNames.ContainsValue(shortName))
            {
                foreach (string key in ShortOKVDNames.Keys)
                {
                    if (ShortOKVDNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region Непустые дни для распределения МБТ

        /// <summary>
        /// Непустые дни для распределения МБТ
        /// </summary>
        private static Dictionary<string, string> mbtNonEmptyDays;

        /// <summary>
        /// Возвращает словарь непустых дней для распределения МБТ
        /// </summary>
        public static Dictionary<string, string> MBTNonEmptyDays
        {
            get
            {
                // если словарь пустой
                if (mbtNonEmptyDays == null || mbtNonEmptyDays.Count == 0)
                {
                    // заполняем его
                    FillMBTNonEmptyDays();
                }
                return mbtNonEmptyDays;
            }
        }

        private static void FillMBTNonEmptyDays()
        {
            mbtNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("mbtNonEmptyDays", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    mbtNonEmptyDays.Add(GetDictionaryUniqueKey(mbtNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    mbtNonEmptyDays.Add(GetDictionaryUniqueKey(mbtNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(mbtNonEmptyDays, row[4].ToString());
                mbtNonEmptyDays.Add(day, "Day");
            }
        }

        #endregion 

        #region Список вариантов фондов ФО

        /// <summary>
        /// Список вариантов фондов ФО
        /// </summary>
        private static Dictionary<string, string> foFondVariantList;

        /// <summary>
        /// Возвращает словарь со списоком вариантов МФРФ
        /// </summary>
        public static Dictionary<string, string> FOFondVariantList
        {
            get
            {
                // если словарь пустой
                if (foFondVariantList == null || foFondVariantList.Count == 0)
                {
                    // заполняем его
                    FillFOFondVariantList();
                }
                return foFondVariantList;
            }
        }

        private static void FillFOFondVariantList()
        {
            foFondVariantList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FOFondVariantList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    foFondVariantList.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region Список основных мероприятий

        /// <summary>
        /// Уникальные имена основных мероприятий
        /// </summary>
        private static Dictionary<string, string> mainActivityUniqNames;
        /// <summary>
        /// Уровни дополнительных мероприятий
        /// </summary>
        private static Dictionary<string, string> mainActivityLevels;

        /// <summary>
        /// Возвращает словарь уник.имен основных мероприятий
        /// </summary>
        public static Dictionary<string, string> MainActivityUniqNames
        {
            get
            {
                // если словарь пустой
                if (mainActivityUniqNames == null || mainActivityUniqNames.Count == 0)
                {
                    // заполняем его
                    FillMainActivityUniqNames();
                }
                return mainActivityUniqNames;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней основных мероприятий
        /// </summary>
        public static Dictionary<string, string> MainActivityLevels
        {
            get
            {
                // если словарь пустой
                if (mainActivityLevels == null || mainActivityLevels.Count == 0)
                {
                    // заполняем его
                    FillMainActivityUniqNames();
                }
                return mainActivityLevels;
            }
        }

        private static void FillMainActivityUniqNames()
        {
            mainActivityUniqNames = new Dictionary<string, string>();
            mainActivityLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("MainActivityList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Основные мероприятия", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string key = GetDictionaryUniqueKey(mainActivityUniqNames, row[0].ToString());
                    mainActivityUniqNames.Add(key, row[1].ToString());
                    mainActivityLevels.Add(key, row[2].ToString());
                }
            }
        }

        #endregion 

        #region Список дополнительных мероприятий
        
        /// <summary>
        /// Уникальные имена дополнительных мероприятий
        /// </summary>
        private static Dictionary<string, string> additionalActivityUniqNames;
        /// <summary>
        /// Уровни дополнительных мероприятий
        /// </summary>
        private static Dictionary<string, string> additionalActivityLevels;

        /// <summary>
        /// Возвращает словарь уник.имен дополнительных мероприятий
        /// </summary>
        public static Dictionary<string, string> AdditionalActivityUniqNames
        {
            get
            {
                // если словарь пустой
                if (additionalActivityUniqNames == null || additionalActivityUniqNames.Count == 0)
                {
                    // заполняем его
                    FillAdditionalActivityUniqNames();
                }
                return additionalActivityUniqNames;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней дополнительных мероприятий
        /// </summary>
        public static Dictionary<string, string> AdditionalActivityLevels
        {
            get
            {
                // если словарь пустой
                if (additionalActivityLevels == null || additionalActivityLevels.Count == 0)
                {
                    // заполняем его
                    FillAdditionalActivityUniqNames();
                }
                return additionalActivityLevels;
            }
        }

        private static void FillAdditionalActivityUniqNames()
        {
            additionalActivityUniqNames = new Dictionary<string, string>();
            additionalActivityLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("AdditionalActivityList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дополнительных мероприятия", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string key = row[0].ToString();
                    if (!additionalActivityUniqNames.ContainsKey(key))
                    {
                        additionalActivityUniqNames.Add(key, row[1].ToString());
                        additionalActivityLevels.Add(key, row[2].ToString());
                    }
                }
            }
        }

        #endregion 

        #region Список муниципальных районов для отчетов по МОФО

        private static Dictionary<string, string> mofoRegionsTypes;
        private static Dictionary<string, string> mofoRegionsUniqueNames;

        /// <summary>
        /// Возвращает словарик типов муниципальных районов для отчетов по МОФО
        /// </summary>
        public static Dictionary<string, string> MOFORegionsTypes
        {
            get
            {
                // если словарь пустой
                if (mofoRegionsTypes == null || mofoRegionsTypes.Count == 0)
                {
                    // заполняем его
                    FillMOFORegions();
                }
                return mofoRegionsTypes;
            }
        }

        /// <summary>
        /// Возвращает словарик уникальных имен муниципальных районов для отчетов по МОФО
        /// </summary>
        public static Dictionary<string, string> MOFORegionsUniqueNames
        {
            get
            {
                // если словарь пустой
                if (mofoRegionsUniqueNames == null || mofoRegionsUniqueNames.Count == 0)
                {
                    // заполняем его
                    FillMOFORegions();
                }
                return mofoRegionsUniqueNames;
            }
        }

        private static void FillMOFORegions()
        {
            mofoRegionsTypes = new Dictionary<string, string>();
            mofoRegionsUniqueNames = new Dictionary<string, string>();

            CustomParam.CustomParamFactory("settlement_level").Value = RegionSettingsHelper.Instance.SettlementLevel;
            CustomParam.CustomParamFactory("region_level").Value = RegionSettingsHelper.Instance.RegionsLevel;

            DataTable regionsDT = new DataTable();
            string query = DataProvider.GetQueryText("MOFORegionsList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "МО", regionsDT);

            for (int i = 0; i < regionsDT.Rows.Count; i++)
            {
                DataRow row = regionsDT.Rows[i];
                if (row[1] != DBNull.Value && row[2] != DBNull.Value && row[3] != DBNull.Value)
                {
                    string name = row[1].ToString();
                    string type = row[2].ToString();
                    string uniqName = row[3].ToString();

                    name = name.Replace("муниципальный район", "МР");
                    name = name.Replace("сельское поселение", "СП");
                    name = name.Replace("городское поселение", "ГП");
                    name = name.Replace("муниципальное образование", "МО");

                    string key = GetDictionaryUniqueKey(mofoRegionsUniqueNames, name);
                    mofoRegionsTypes.Add(key, type);
                    mofoRegionsUniqueNames.Add(key, uniqName);
                }

            }
        }

        #endregion

        #region Получение даты для численности постоянного населения

        public static string GetFederalPopulationDate()
        {
            string query = DataProvider.GetQueryText("FederalPopulationDate", HttpContext.Current.Request.PhysicalApplicationPath);
            DataTable dtPopulationDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtPopulationDate);

            string date = string.Empty;

            if (dtPopulationDate.Rows.Count > 0 && dtPopulationDate.Rows[0][1] != DBNull.Value &&
                    dtPopulationDate.Rows[0][1].ToString() != string.Empty)
            {
                date = string.Format("на 01.01.{0} г.", dtPopulationDate.Rows[0][1]);
            }

            return date;
        }

        #endregion

        #region Получение даты для численности постоянного населения текущего региона

        public static string GetRegionPopulationDate(int currentYear)
        {
            CustomParam.CustomParamFactory("current_year").Value = currentYear.ToString();
            CustomParam.CustomParamFactory("consolidate_region").Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            CustomParam.CustomParamFactory("population_cube").Value = RegionSettingsHelper.Instance.PopulationCube;
            CustomParam.CustomParamFactory("population_filter").Value = RegionSettingsHelper.Instance.PopulationFilter;
            CustomParam.CustomParamFactory("population_period_dimension").Value = RegionSettingsHelper.Instance.PopulationPeriodDimension;

            string query = DataProvider.GetQueryText("RegionPopulationDate", HttpContext.Current.Request.PhysicalApplicationPath);
            DataTable dtPopulationDate = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtPopulationDate);

            string date = String.Empty;

            if (dtPopulationDate.Rows.Count > 0 && dtPopulationDate.Rows[0][1] != DBNull.Value &&
                    dtPopulationDate.Rows[0][1].ToString() != String.Empty)
            {
                date = String.Format("на 01.01.{0} г.", dtPopulationDate.Rows[0][1]);
            }

            return date;
        }

        public static string GetOmskRegionPopulationDate(int currentYear)
        {
            CustomParam.CustomParamFactory("current_year").Value = currentYear.ToString();

            string query = DataProvider.GetQueryText("OmskRegionPopulationDate", HttpContext.Current.Request.PhysicalApplicationPath);
            DataTable dtPopulationDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtPopulationDate);

            string date = String.Empty;

            if (dtPopulationDate.Rows.Count > 0 && dtPopulationDate.Rows[0][1] != DBNull.Value &&
                    dtPopulationDate.Rows[0][1].ToString() != String.Empty)
            {
                date = String.Format("на 01.01.{0} г.", dtPopulationDate.Rows[0][1]);
            }

            return date;
        }

        #endregion

        #region Список показателей для оценки качества

        /// <summary>
        /// Список показателей для оценки качества
        /// </summary>
        private static Dictionary<string, string> qualityEvaluationIndicatorList;

        /// <summary>
        /// Возвращает словарь со списоком показателей для оценки качества
        /// </summary>
        public static Dictionary<string, string> QualityEvaluationIndicatorList
        {
            get
            {
                // если словарь пустой
                if (qualityEvaluationIndicatorList == null || qualityEvaluationIndicatorList.Count == 0)
                {
                    // заполняем его
                    FillQualityEvaluationIndicatorList();
                }
                return qualityEvaluationIndicatorList;
            }
        }

        private static void FillQualityEvaluationIndicatorList()
        {
            qualityEvaluationIndicatorList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("QualityEvaluationIndicatorList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    qualityEvaluationIndicatorList.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region Список показателей для значений качества

        /// <summary>
        /// Список показателей для оценки качества
        /// </summary>
        private static Dictionary<string, string> qualityValueIndicatorList;

        /// <summary>
        /// Возвращает словарь со списоком показателей для значений качества
        /// </summary>
        public static Dictionary<string, string> QualityValueIndicatorList
        {
            get
            {
                // если словарь пустой
                if (qualityValueIndicatorList == null || qualityValueIndicatorList.Count == 0)
                {
                    // заполняем его
                    FillQualityValueIndicatorList();
                }
                return qualityValueIndicatorList;
            }
        }

        private static void FillQualityValueIndicatorList()
        {
            qualityValueIndicatorList = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("QualityValueIndicatorList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    qualityValueIndicatorList.Add(row[0].ToString(), row[1].ToString());
                }
            }
        }

        #endregion 

        #region Краткие наименования ГРБС для показателей ФМ

        /// <summary>
        /// Краткие наименования ГРБС для показателей ФМ
        /// </summary>
        private static Dictionary<string, string> shortFMGRBSNames;

        /// <summary>
        /// Возвращает словарь кратких наименований ГРБС для показателей ФМ
        /// </summary>
        public static Dictionary<string, string> ShortFMGRBSNames
        {
            get
            {
                // если словарь пустой
                if (shortFMGRBSNames == null || shortFMGRBSNames.Count == 0)
                {
                    // заполняем его
                    FillShortFMGRBSNames();
                }
                return shortFMGRBSNames;
            }
        }

        private static void FillShortFMGRBSNames()
        {
            shortFMGRBSNames = new Dictionary<string, string>();
            shortFMGRBSNames.Add("Законодательное Собрание Омской области. Управление обеспечения", "Заксобрание");
            shortFMGRBSNames.Add("Законодательное Собрание Омской области", "Заксобрание");
            shortFMGRBSNames.Add("Управление делами Правительства Омской области", "Управделами");
            shortFMGRBSNames.Add("Представительство Правительства Омской области при Правительстве Российской Федерации", "Представительство");
            shortFMGRBSNames.Add("Контрольно-счетная палата Омской области", "КСП");
            shortFMGRBSNames.Add("Министерство государственно-правового развития Омской области", "МинГПР");
            shortFMGRBSNames.Add("Министерство здравоохранения Омской области", "Минздрав");
            shortFMGRBSNames.Add("Министерство имущественных отношений Омской области", "Минимущество");
            shortFMGRBSNames.Add("Министерство культуры Омской области", "Минкультуры");
            shortFMGRBSNames.Add("Министерство образования Омской области", "Минобр");
            shortFMGRBSNames.Add("Министерство по делам молодежи, физической культуры и спорта Омской области", "Минмолодежи");
            shortFMGRBSNames.Add("Министерство промышленной политики, транспорта и связи Омской области", "Минпром");
            shortFMGRBSNames.Add("Министерство сельского хозяйства и продовольствия Омской области", "Минсельхозпрод");
            shortFMGRBSNames.Add("Министерство строительства и жилищно-коммунального комплекса Омской области", "Минстрой");
            shortFMGRBSNames.Add("Министерство труда и социального развития Омской области", "Минтруд");
            shortFMGRBSNames.Add("Министерство экономики Омской области", "Минэконом");
            shortFMGRBSNames.Add("Главное организационно-кадровое управление Омской области", "ГОКУ");
            shortFMGRBSNames.Add("Главное управление по делам печати, телерадиовещания и средств массовых коммуникаций Омской области", "ГУ печати");
            shortFMGRBSNames.Add("Главное управление по делам гражданской обороны и чрезвычайным ситуациям Омской области", "ГУ по делам ГО и ЧС");
            shortFMGRBSNames.Add("Государственная жилищная инспекция Омской области", "Госжилинсп");
            shortFMGRBSNames.Add("Главное управление ветеринарии Омской области", "ГУ ветеринарии");
            shortFMGRBSNames.Add("Управление внутренних дел по Омской области", "УВД");
            shortFMGRBSNames.Add("Избирательная комиссия Омской области", "Избирком");
            shortFMGRBSNames.Add("Главное управление государственного строительного надзора и государственной экспертизы Омской области", "ГУ строит.надзор");
            shortFMGRBSNames.Add("Региональная энергетическая комиссия Омской области", "РЭК");
            shortFMGRBSNames.Add("Государственная инспекция по надзору за техническим состоянием самоходных машин и других видов техники при Министерстве сельского хозяйства и продовольствия Омской области", "Гостехнадзор");
            shortFMGRBSNames.Add("Главное управление государственной службы занятости населения Омской области", "ГУ занятости");
            shortFMGRBSNames.Add("Главное управление по земельным ресурсам Омской области", "ГУ по земресурсам");
            shortFMGRBSNames.Add("Уполномоченный Омской области по правам человека", "Уполномоченный");
            shortFMGRBSNames.Add("Главное управление информационных технологий и телекоммуникаций Омской области", "ГУИТиТ");

            shortFMGRBSNames.Add("Министерство финансов Омской области", "Минфин");
            shortFMGRBSNames.Add("Главное управление лесного хозяйства Омской области", "ГУ лесхоз");
            shortFMGRBSNames.Add("Главное управление жилищного контроля, государственного строительного надзора Омской области", "Госжилстройнадзор");

            shortFMGRBSNames.Add("Дума Ханты-Мансийского автономного округа - Югры", "Дума Югры");
            shortFMGRBSNames.Add("Правительство Ханты-Мансийского автономного округа – Югры", "Правительство Югры");
            shortFMGRBSNames.Add("Представительство Ханты - Мансийского автономного округа - Югры при Правительстве Российской Федерации и в субъектах Российской Федерации", "Представительство Югры");
            shortFMGRBSNames.Add("Представительство Ханты-Мансийского автономного округа – Югры в г. Тюмени", "Представительство Югры в г. Тюмени");
            shortFMGRBSNames.Add("Региональная служба по тарифам Ханты-Мансийского автономного округа - Югры", "РСТ Югры");
            shortFMGRBSNames.Add("Служба по контролю и надзору в сфере здравоохранения Ханты-Мансийского автономного округа - Югры", "Здравнадзор Югры");
            shortFMGRBSNames.Add("Управление ветеринарии Ханты-Мансийского автономного округа – Югры", "Ветуправление Югры");
            shortFMGRBSNames.Add("Служба государственного надзора за техническим состоянием самоходных машин и других видов техники Ханты-Мансийского автономного округа - Югры", "Гостехнадзор Югры");
            shortFMGRBSNames.Add("Главное управление Министерства Российской Федерации по делам гражданской обороны, чрезвычайным ситуациям и ликвидации последствий стихийных бедствий по Ханты-Мансийскому автономному округу – Югре", "ГУ по делам ГО и ЧС Югры");
            shortFMGRBSNames.Add("Департамент дорожного хозяйства и транспорта Ханты-Мансийского автономного округа - Югры", "Депдорхоз и транспорта Югры");
            shortFMGRBSNames.Add("Управление внутренних дел по Ханты-Мансийскому автономному округу - Югре", "УВД Югры");
            shortFMGRBSNames.Add("Департамент инвестиций, науки и технологий Ханты-Мансийского автономного округа – Югры", "Депинвест и технологий Югры");
            shortFMGRBSNames.Add("Департамент образования и молодежной политики Ханты- Мансийского автономного округа - Югры", "Депобразования и молодежи Югры");
            shortFMGRBSNames.Add("Департамент культуры Ханты-Мансийского автономного округа - Югры", "Депкультуры Югры");
            shortFMGRBSNames.Add("Департамент общественных связей Ханты-Мансийского автономного округа - Югры", "Департамент общественных связей Югры");
            shortFMGRBSNames.Add("Департамент здравоохранения Ханты-Мансийского автономного округа – Югры", "Депздрав Югры");
            shortFMGRBSNames.Add("Департамент физической культуры и спорта Ханты- Мансийского автономного округа - Югры", "Депспорт Югры");
            shortFMGRBSNames.Add("Комитет по молодежной политике Ханты-Мансийского автономного округа – Югры", "Комитет по молодежной политике Югры");
            shortFMGRBSNames.Add("Департамент социального развития Ханты-Мансийского автономного округа - Югры", "Депсоцразвития Югры");
            shortFMGRBSNames.Add("Департамент экологии Ханты- Мансийского автономного округа - Югры", "Депэкологии Югры");
            shortFMGRBSNames.Add("Департамент труда и занятости населения Ханты-Мансийского автономного округа - Югры", "Дептруда и занятости Югры");
            shortFMGRBSNames.Add("Дорожный департамент Ханты-Мансийского автономного округа – Югры", "Дорожный департамент Югры");
            shortFMGRBSNames.Add("Департамент гражданской защиты населения Ханты- Мансийского автономного округа - Югры", "Департамент гражданской защиты населения Югры");
            shortFMGRBSNames.Add("Департамент по вопросам юстиции Ханты-Мансийского автономного округа – Югры", "Депюстиции Югры");
            shortFMGRBSNames.Add("Представительство Ханты-Мансийского автономного округа – Югры в г. Санкт-Петербурге", "Представительство Югры в г. Санкт-Петербурге");
            shortFMGRBSNames.Add("Департамент природных ресурсов и несырьевого сектора экономики Ханты- Мансийского автономного округа - Югры", "Депприродресурс и несырьевого сектора экономики Югры");
            shortFMGRBSNames.Add("Служба по контролю и надзору в сфере образования Ханты- Мансийского автономного округа - Югры", "Обрнадзор Югры");
            shortFMGRBSNames.Add("Служба жилищного контроля и строительного надзора Ханты-Мансийского автономного округа - Югры", "Жилстройнадзор Югры");
            shortFMGRBSNames.Add("Департамент по управлению государственным имуществом Ханты-Мансийского автономного округа - Югры", "Депимущества Югры");
            shortFMGRBSNames.Add("Избирательная комиссия Ханты-Мансийского автономного округа - Югры", "Депимущества Югры");
            shortFMGRBSNames.Add("Комитет информационного мониторинга Ханты-Мансийского автономного округа – Югры", "Комитет инфмониторинга Югры");
            shortFMGRBSNames.Add("Департамент строительства Ханты-Мансийского автономного округа – Югры", "Депстрой Югры");
            shortFMGRBSNames.Add("Управление агропромышленного комплекса Ханты-Мансийского автономного округа – Югры", "Управление АПК Югры");
            shortFMGRBSNames.Add("Департамент строительства, энергетики и жилищно-  коммунального комплекса Ханты-Мансийского автономного округа - Югры", "Депстройэнергетики и ЖКК Югры");
            shortFMGRBSNames.Add("Департамент по вопросам малочисленных народов Севера Ханты-Мансийского автономного округа – Югры", "Депмалочислнародов Севера Югры");
            shortFMGRBSNames.Add("Департамент финансов Ханты-Мансийского автономного округа - Югры", "Депфин Югры");
            shortFMGRBSNames.Add("Департамент по недропользованию Ханты- Мансийского автономного округа - Югры", "Депнедра Югры");
            shortFMGRBSNames.Add("Служба государственной охраны объектов культурного наследия Ханты-Мансийского автономного округа - Югры", "Госкультохрана Югры");
            shortFMGRBSNames.Add("Служба по контролю и надзору в сфере охраны окружающей  среды, объектов животного мира и лесных отношений Ханты-Мансийского автономного округа - Югры", "Природнадзор Югры");
            shortFMGRBSNames.Add("Представительство Ханты-Мансийского автономного округа – Югры в г. Екатеринбурге", "Представительство Югры в г. Екатеринбурге");
            shortFMGRBSNames.Add("Служба по делам архивов Ханты-Мансийского автономного округа - Югры", "Архивная служба Югры");
            shortFMGRBSNames.Add("Департамент государственного заказа Ханты-Мансийского автономного округа - Югры", "Депгосзаказа Югры");
            shortFMGRBSNames.Add("Департамент информационных технологий Ханты-Мансийского автономного округа - Югры", "Депинформтехнологий Югры");
            shortFMGRBSNames.Add("Департамент внутренней политики Ханты-Мансийского автономного округа - Югры", "Депполитики Югры");
            shortFMGRBSNames.Add("Департамент управления делами Губернатора Ханты-Мансийского автономного округа - Югры", "Департамент управделами Югры");
            shortFMGRBSNames.Add("Департамент экономического развития Ханты-Мансийского автономного округа - Югры", "Депэкономики Югры");
            shortFMGRBSNames.Add("Департамент жилищной политики Ханты-Мансийского автономного округа - Югры", "Депжилполитики Югры");
            shortFMGRBSNames.Add("Департамент здравоохранения Ханты-Мансийского автономного округа - Югры", "Депздрав Югры");
            shortFMGRBSNames.Add("Ветеринарная служба Ханты-Мансийского автономного округа - Югры", "Ветслужба Югры");
            shortFMGRBSNames.Add("Правительство Ханты-Мансийского автономного округа - Югры", "Правительство Югры");
        }

        /// <summary>
        /// Получение краткого имени КД по полному
        /// </summary>
        /// <param name="fullName">полное имя</param>
        /// <returns>краткое имя</returns>
        public static string GetShortFMGRBSNames(string fullName)
        {
            if (ShortFMGRBSNames.ContainsKey(fullName))
            {
                return ShortFMGRBSNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// Получение полного имени КД по краткому
        /// </summary>
        /// <param name="shortName">краткое имя</param>
        /// <returns>полное имя</returns>
        public static string GetFullFMGRBSNames(string shortName)
        {
            if (ShortFMGRBSNames.ContainsValue(shortName))
            {
                foreach (string key in ShortFMGRBSNames.Keys)
                {
                    if (ShortFMGRBSNames[key] == shortName)
                    {
                        return key;
                    }
                }
            }
            return shortName;
        }

        #endregion 

        #region Список территорий для УрФО

        private static Dictionary<string, string> urfoRegionUniqueNames;
        private static Dictionary<string, string> urfoRegionLevels;

        /// <summary>
        /// Возвращает словарь уник.нэймов территорий для УрФО
        /// </summary>
        public static Dictionary<string, string> UrfoRegionUniqueName
        {
            get
            {
                // если словарь пустой
                if (urfoRegionUniqueNames == null || urfoRegionUniqueNames.Count == 0)
                {
                    // заполняем его
                    FillUrfoRegionList();
                }
                return urfoRegionUniqueNames;
            }
        }

        /// <summary>
        /// Возвращает словарь уровней территорий для УрФО
        /// </summary>
        public static Dictionary<string, string> UrfoRegionLevels
        {
            get
            {
                // если словарь пустой
                if (urfoRegionLevels == null || urfoRegionLevels.Count == 0)
                {
                    // заполняем его
                    FillUrfoRegionList();
                }
                return urfoRegionLevels;
            }
        }

        private static void FillUrfoRegionList()
        {
            urfoRegionUniqueNames = new Dictionary<string, string>();
            urfoRegionLevels = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("UrFORegionList", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территории УрФО", dt);

            foreach (DataRow row in dt.Rows)
            {
                urfoRegionUniqueNames.Add(GetDictionaryUniqueKey(urfoRegionUniqueNames, row[0].ToString()), row[1].ToString());
                urfoRegionLevels.Add(GetDictionaryUniqueKey(urfoRegionLevels, row[0].ToString()), row[2].ToString());
            }
        }

        #endregion 

        public static string GetShortRzPrName(string fullName)
        {
            string shortName = fullName;

            switch (fullName)
            {
                case "ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ":
                    {
                        return "Общегосуд.вопросы";
                    }
                case "НАЦИОНАЛЬНАЯ ОБОРОНА":
                    {
                        return "Национальная оборона";
                    }
                case "НАЦИОНАЛЬНАЯ БЕЗОПАСНОСТЬ И ПРАВООХРАНИТЕЛЬНАЯ ДЕЯТЕЛЬНОСТЬ":
                    {
                        return "Нац.безопасность и правоохранит.деят.";
                    }
                case "НАЦИОНАЛЬНАЯ ЭКОНОМИКА":
                    {
                        return "Национальная экономика";
                    }
                case "ЖИЛИЩНО-КОММУНАЛЬНОЕ ХОЗЯЙСТВО":
                    {
                        return "ЖКХ";
                    }
                case "ОХРАНА ОКРУЖАЮЩЕЙ СРЕДЫ":
                    {
                        return "Охрана окруж.среды";
                    }
                case "ОБРАЗОВАНИЕ":
                    {
                        return "Образование";
                    }
                case "КУЛЬТУРА, КИНЕМАТОГРАФИЯ":
                    {
                        return "Культура и кинематография";
                    }
                case "КУЛЬТУРА, КИНЕМАТОГРАФИЯ, СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ":
                    {
                        return "Культура,  кинематография, СМИ";
                    }
                case "СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ":
                    {
                        return "СМИ";
                    }
                case "ЗДРАВООХРАНЕНИЕ":
                    {
                        return "Здравоохранение";
                    }
                case "ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ":
                    {
                        return "Здрав., физ.культура и спорт";
                    }
                case "ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ":
                    {
                        return "Физическая культура и спорт";
                    }
                case "СОЦИАЛЬНАЯ ПОЛИТИКА":
                    {
                        return "Социальная политика";
                    }
                case "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ":
                    {
                        return "Межбюджетные трансферты";
                    }
                case "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ ОБЩЕГО ХАРАКТЕРА БЮДЖЕТАМ СУБЪЕКТОВ РОССИЙСКОЙ ФЕДЕРАЦИИ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ":
                    {
                        return "МБТ бюджетам суб.РФ и МО";
                    }
                case "ОБСЛУЖИВАНИЕ ГОСУДАРСТВЕННОГО И МУНИЦИПАЛЬНОГО ДОЛГА":
                    {
                        return "Обслуж.гос.и мун.долга";
                    }
            }
            return shortName;
        }
    }
}
