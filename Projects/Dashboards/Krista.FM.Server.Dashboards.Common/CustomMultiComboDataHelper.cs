using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Xml;

namespace Krista.FM.Server.Dashboards.Common
{
    public static class CustomMultiComboDataHelper
    {
        /// <summary>
        /// Заполнить список годами
        /// </summary>
        /// <param name="lowValue">нижний предел</param>
        /// <param name="highValue">верхний предел</param>
        public static Dictionary<string, int> FillYearValues(int lowValue, int highValue)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            for (int i = lowValue; i <= highValue; i++)
            {
                valuesDictionary.Add(i.ToString(), 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список месяцами
        /// </summary>
        public static Dictionary<string, int> FillMonthValues()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            for (int i = 1; i <= 12; i++)
            {
                valuesDictionary.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список годами с месяцами
        /// </summary>
        /// <param name="lowValue">нижний предел</param>
        /// <param name="highValue">верхний предел</param>
        public static Dictionary<string, int> FillYearWithMonthsValues(int lowValue, int highValue)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            for (int i = lowValue; i <= highValue; i++)
            {
                valuesDictionary.Add(i.ToString(), 0);
                for (int j = 1; j <= 12; j++)
                {
                    string month = GetDictionaryUniqueKey(valuesDictionary, CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(j)));
                    valuesDictionary.Add(month, 1);
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список субъектами
        /// </summary>
        public static Dictionary<string, int> FillSubjectNames(Collection<string> subjectCollection)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            for (int i = 0; i < subjectCollection.Count; i++)
            {
                valuesDictionary.Add(subjectCollection[i], 0);
            }
            return valuesDictionary;
        }

         /// <summary>
        /// Заполнить список федеральными округами
        /// </summary>
        public static Dictionary<string, int> FillFONames(Collection<string> foCollection)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Все федеральные округа", 0);
            for (int i = 0; i < foCollection.Count; i++)
            {
                valuesDictionary.Add(foCollection[i], 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список видами доходов
        /// </summary>
        public static Dictionary<string, int> FillKDNames(Dictionary<string, string> kdDictionary)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            foreach (string key in kdDictionary.Keys)
            {
                valuesDictionary.Add(key.TrimEnd('_'), 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список видами расходов ФКР
        /// </summary>
        public static Dictionary<string, int> FillFKRNames(Dictionary<string, string> outcomesFKRTypes, Dictionary<string, string> outcomesFKRLevels)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in outcomesFKRTypes.Keys)
            {
                string levelName = outcomesFKRLevels[key];
                int level = 0;
                switch (levelName)
                {
                    case "(All)":
                        {
                            level = 0;
                            break;
                        }
                    case "Раздел":
                        {
                            level = 0;
                            break;
                        }
                    case "Подраздел":
                        {
                            level = 1;
                            break;
                        }
                }
                valuesDictionary.Add(key.TrimEnd('_'), level);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список видами расходов ФКР для ФО
        /// </summary>
        public static Dictionary<string, int> FillFOFKRNames(Dictionary<string, string> outcomesFOFKRTypes, Dictionary<string, string> outcomesFOFKRLevels, bool allLevel)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in outcomesFOFKRTypes.Keys)
            {
                string levelName = outcomesFOFKRLevels[key];
                int level = 0;
                switch (levelName)
                {
                    case "(All)":
                        {
                            level = 0;
                            break;
                        }
                    case "Раздел":
                        {
                            level = 0;
                            break;
                        }
                    case "Подраздел":
                        {
                            level = 1;
                            break;
                        }
                }
                valuesDictionary.Add(key.TrimEnd('_'), level);
            }

            if (allLevel)
            {
                valuesDictionary.Add("Расходы бюджета - Итого", 0);
            }

            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список видами расходов КОСГУ
        /// </summary>
        public static Dictionary<string, int> FillKOSGUNames(Dictionary<string, string> outcomesKOSGUTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            foreach (string key in outcomesKOSGUTypes.Keys)
            {
                valuesDictionary.Add(key.TrimEnd('_'), 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список местными бюджетами
        /// </summary>
        public static Dictionary<string, int> FillLocalBudgets(Dictionary<string, string> localBudgetTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            bool regionIncluded = false;
            bool townIncluded = false;

            foreach (string key in localBudgetTypes.Keys)
            {
                string type = localBudgetTypes[key];

                if (type.Contains("КБС") || type.Contains("СБС") || type.Contains("МБ"))
                {
                    valuesDictionary.Add(key, 0);
                }
                if (type.Contains("МР"))
                {
                    if (!regionIncluded)
                    {
                        valuesDictionary.Add("Муниципальные районы", 0);
                        regionIncluded = true;
                    }
                    valuesDictionary.Add(key, 1);
                }
                if (type.Contains("ГО"))
                {
                    if (!townIncluded)
                    {
                        valuesDictionary.Add("Городские округа", 0);
                        townIncluded = true;
                    }
                    valuesDictionary.Add(key, 1);
                }
            }

            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список уровнями СКИФ
        /// </summary>
        public static Dictionary<string, int> FillSKIFLevels()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Консолидированный бюджет субъекта", 0);
            valuesDictionary.Add("Собственный бюджет субъекта", 0);
            valuesDictionary.Add("Местные бюджеты", 0);
            valuesDictionary.Add("Городские округа", 1);
            valuesDictionary.Add("Муниципальные районы", 1);
            valuesDictionary.Add("Собственные бюджеты районов", 2);
            valuesDictionary.Add("Городские и сельские поселения", 2);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить полный список доходами КД (с элементами "в том числе")
        /// </summary>
        public static Dictionary<string, int> FillFullKDIncludingList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Налоговые доходы ", 0);
                valuesDictionary.Add("Налог на прибыль ", 1);
                valuesDictionary.Add("НДФЛ ", 1);
                valuesDictionary.Add("Налоги на имущество ", 1);
                    valuesDictionary.Add("Налог на имущество физ.лиц ", 2);
                    valuesDictionary.Add("Налог на имущество организаций ", 2);
                    valuesDictionary.Add("Транспортный налог ", 2);
                        valuesDictionary.Add("Транспортный налог с организаций ", 3);
                        valuesDictionary.Add("Транспортный налог с физ.лиц ", 3);
                    valuesDictionary.Add("Земельный налог ", 2);
                valuesDictionary.Add("Акцизы ", 1);
                    valuesDictionary.Add("Акцизы на нефтепродукты ", 2);
                    valuesDictionary.Add("Акцизы на алкоголь ", 2);
                valuesDictionary.Add("НДПИ ", 1);
                valuesDictionary.Add("Налоги на совокупный доход ", 1);
                    valuesDictionary.Add("УСН ", 2);
                    valuesDictionary.Add("ЕНВД ", 2);
                    valuesDictionary.Add("ЕСХН ", 2);
                valuesDictionary.Add("Гос.пошлина ", 1);
                valuesDictionary.Add("Задолженность по отмененным налогам ", 1);
            valuesDictionary.Add("Неналоговые доходы ", 0);
                valuesDictionary.Add("Доходы от использования имущества ", 1);
                    valuesDictionary.Add("Дивиденды по акциям ", 2);
                    valuesDictionary.Add("Арендная плата за земли ", 2);
                    valuesDictionary.Add("Доходы от сдачи в аренду имущества ", 2);
                    valuesDictionary.Add("Платежи от ГУПов и МУПов ", 2);
                valuesDictionary.Add("Платежи при пользовании природными ресурсами ", 1);
                    valuesDictionary.Add("Плата за негативное воздействие на окруж.среду ", 2);
                    valuesDictionary.Add("Платежи за пользование лесным фондом ", 2);
                    valuesDictionary.Add("Платежи при пользовании недрами ", 2);
                valuesDictionary.Add("Доходы от оказания платных услуг ", 1);
                valuesDictionary.Add("Доходы от продажи активов ", 1);
                    valuesDictionary.Add("Доходы от продажи активов (кроме зем.участков) ", 2);
                    valuesDictionary.Add("Доходы от продажи зем. участков ", 2);
                valuesDictionary.Add("Административные платежи и сборы ", 1);
                valuesDictionary.Add("Штрафы ", 1);
                valuesDictionary.Add("Прочие ", 1);
                valuesDictionary.Add("Доходы бюджетов от возврата остатков МБТ прошлых лет ", 1);
                valuesDictionary.Add("Возврат остатков МБТ прошлых лет ", 1);
            valuesDictionary.Add("Доходы от приносящей доход деятельности ", 0);
            valuesDictionary.Add("Налоговые и неналоговые доходы ", 0);
            valuesDictionary.Add("Безвозмездные поступления ", 0);
            valuesDictionary.Add("Доходы бюджета - Итого ", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить полный список доходами КД (с элементами "в том числе") для множественного выбора
        /// </summary>
        public static Dictionary<string, int> FillFullKDIncludingMultipleList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Налоговые доходы ", 0);
            valuesDictionary.Add("Налог на прибыль ", 1);
            valuesDictionary.Add("НДФЛ ", 1);
            valuesDictionary.Add("Налоги на имущество ", 1);
            valuesDictionary.Add("Налог на имущество физ.лиц ", 2);
            valuesDictionary.Add("Налог на имущество организаций ", 2);
            valuesDictionary.Add("Транспортный налог ", 2);
            valuesDictionary.Add("Транспортный налог с организации ", 3);
            valuesDictionary.Add("Транспортный налог с физ.лиц ", 3);
            valuesDictionary.Add("Налог на игорный бизнес ", 2);
            valuesDictionary.Add("Земельный налог ", 2);
            valuesDictionary.Add("Акцизы ", 1);
            valuesDictionary.Add("Акцизы на нефтепродукты ", 2);
            valuesDictionary.Add("Акцизы на алкоголь ", 2);
            valuesDictionary.Add("НДПИ ", 1);
            valuesDictionary.Add("Налоги на совокупный доход ", 1);
            valuesDictionary.Add("УСН ", 2);
            valuesDictionary.Add("ЕНВД ", 2);
            valuesDictionary.Add("ЕСХН ", 2);
            valuesDictionary.Add("Гос.пошлина ", 1);
            valuesDictionary.Add("Задолженность по отмененным налогам ", 1);
            valuesDictionary.Add("Неналоговые доходы ", 0);
            valuesDictionary.Add("Доходы от использования имущества ", 1);
            valuesDictionary.Add("Дивиденды по акциям ", 2);
            valuesDictionary.Add("Арендная плата за земли ", 2);
            valuesDictionary.Add("Доходы от сдачи в аренду имущества ", 2);
            valuesDictionary.Add("Платежи от ГУПов и МУПов ", 2);
            valuesDictionary.Add("Платежи при пользовании природными ресурсами ", 1);
            valuesDictionary.Add("Плата за негативное воздействие на окруж.среду ", 2);
            valuesDictionary.Add("Платежи за пользование лесным фондом ", 2);
            valuesDictionary.Add("Доходы от оказания платных услуг ", 1);
            valuesDictionary.Add("Доходы от продажи активов ", 1);
            valuesDictionary.Add("Доходы от продажи активов (кроме зем.участков) ", 2);
            valuesDictionary.Add("Доходы от продажи зем. участков ", 2);
            valuesDictionary.Add("Штрафы ", 1);
            valuesDictionary.Add("Доходы от приносящей доход деятельности ", 0);
            valuesDictionary.Add("Безвозмездные поступления ", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список налогами КД 
        /// </summary>

        public static Dictionary<string, int> FillTaxesKDIncludingList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Налог на прибыль ", 0);
            valuesDictionary.Add("НДФЛ ", 0);
            valuesDictionary.Add("Налоги на имущество ", 0);
            valuesDictionary.Add("Налог на имущество физ.лиц ", 1);
            valuesDictionary.Add("Налог на имущество организаций ", 1);
            valuesDictionary.Add("Транспортный налог ", 1);
            valuesDictionary.Add("Земельный налог ", 1);
            valuesDictionary.Add("Акцизы ", 0);
            valuesDictionary.Add("Акцизы на нефтепродукты ", 1);
            valuesDictionary.Add("Акцизы на алкоголь ", 1);
            valuesDictionary.Add("НДПИ ", 0);
            valuesDictionary.Add("Налоги на совокупный доход ", 0);
            valuesDictionary.Add("УСН ", 1);
            valuesDictionary.Add("ЕНВД ", 1);
            valuesDictionary.Add("ЕСХН ", 1);
            valuesDictionary.Add("Гос.пошлина ", 0);
            valuesDictionary.Add("Задолженность по отмененным налогам ", 0);
         
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить полный список доходами КД для отчетов ФНС (с элементами "в том числе")
        /// </summary>
        public static Dictionary<string, int> FillFullFNSKDIncludingList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Налоговые доходы ", 0);
            valuesDictionary.Add("Налог на прибыль ", 1);
            valuesDictionary.Add("НДФЛ ", 1);
            valuesDictionary.Add("Налоги на имущество ", 1);
            valuesDictionary.Add("Налог на имущество физ.лиц ", 2);
            valuesDictionary.Add("Налог на имущество организаций ", 2);
            valuesDictionary.Add("Транспортный налог ", 2);
            valuesDictionary.Add("Земельный налог ", 2);
            valuesDictionary.Add("Акцизы ", 1);
            valuesDictionary.Add("Акцизы на нефтепродукты ", 2);
            valuesDictionary.Add("Акцизы на алкогольную продукцию ", 2);
            valuesDictionary.Add("НДПИ ", 1);
            valuesDictionary.Add("Налоги на совокупный доход ", 1);
            valuesDictionary.Add("УСН ", 2);
            valuesDictionary.Add("ЕНВД ", 2);
            valuesDictionary.Add("ЕСХН ", 2);
            valuesDictionary.Add("Гос.пошлина ", 1);
            valuesDictionary.Add("Задолженность по отмененным налогам ", 1);
            valuesDictionary.Add("Неналоговые доходы ", 0);
            valuesDictionary.Add("Платежи при пользовании природными ресурсами ", 1);
            valuesDictionary.Add("Плата за негативное воздействие на окруж.среду ", 2);
            valuesDictionary.Add("Платежи за пользование лесным фондом ", 2);
            valuesDictionary.Add("Штрафы ", 1);
            valuesDictionary.Add("Налоговые и неналоговые доходы ", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить короткий список доходами КД (с элементами "в том числе")
        /// </summary>
        public static Dictionary<string, int> FillShortKDIncludingList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Налоговые доходы ", 0);
            valuesDictionary.Add("Налог на прибыль ", 1);
            valuesDictionary.Add("НДФЛ ", 1);
            valuesDictionary.Add("Налоги на имущество ", 1);
            valuesDictionary.Add("Акцизы ", 1);
            valuesDictionary.Add("НДПИ ", 1);
            valuesDictionary.Add("Налоги на совокупный доход ", 1);
            valuesDictionary.Add("Неналоговые доходы ", 0);
            valuesDictionary.Add("Доходы от приносящей доход деятельности ", 0);
            valuesDictionary.Add("Налоговые и неналоговые доходы ", 0);
            valuesDictionary.Add("Безвозмездные поступления ", 0);
            valuesDictionary.Add("Доходы ВСЕГО ", 0);
            return valuesDictionary;
        }

        public static Dictionary<string, int> FillFNS65nList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Неуказанное значение", 0);
            valuesDictionary.Add("Начислено - всего", 0);
            valuesDictionary.Add("Поступило - всего", 0);
            valuesDictionary.Add("Возмещено", 0);
            valuesDictionary.Add("Общая сумма задолженности - всего", 0);
            valuesDictionary.Add("Недоимка по налогу", 0);
            valuesDictionary.Add("Неурегулированная задолженность по пени", 0);
            valuesDictionary.Add("Неурегулированная задолженность по налоговым санкциям", 0);
            valuesDictionary.Add("Сумма непогашенной отсрочки (рассрочки)", 0);
            valuesDictionary.Add("Остаток непогашенной реструктурированной задолженности", 0);
            valuesDictionary.Add("Остаток непогашенной задолженности, приостановленной к взысканию", 0);
            valuesDictionary.Add("Переплата", 0);
            valuesDictionary.Add("Поступило - итого", 0);
            return valuesDictionary;
        }

        public static Dictionary<string, int> FillIndicators(Dictionary<string, string> IndicatorsTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            
            foreach (string key in IndicatorsTypes.Keys)
            { 
                valuesDictionary.Add(key, 0); 
            }
            return valuesDictionary;
        }

        public static Dictionary<string, int> FillOKVED(Dictionary<string, string> OKVEDTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in OKVEDTypes.Keys)
            {
                CRHelper.SaveToErrorLog(key);
                valuesDictionary.Add(key, 0);
            }
            return valuesDictionary;
        }

        public static Dictionary<string, int> FillKinds(Dictionary<string, string> KindsTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in KindsTypes.Keys)
            {
                valuesDictionary.Add(key, 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список видами задолженности
        /// </summary>
        public static Dictionary<string, int> FillDebtsList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Просроченная кредиторская задолженность, всего ", 0);
            valuesDictionary.Add("По заработной плате ", 1);
            valuesDictionary.Add("По начислениям на выплаты по оплате труда ", 1);
            valuesDictionary.Add("По услугам связи ", 1);
            valuesDictionary.Add("По транспортным услугам ", 1);
            valuesDictionary.Add("По коммунальным услугам ", 1);
            valuesDictionary.Add("По работам, услугам по содерж.имущества ", 1);
            valuesDictionary.Add("По прочим работам, услугам ", 1);
            valuesDictionary.Add("По безвозмездным перечислениям гос. и мун. организациям ", 1);
            valuesDictionary.Add("По безвозмездным перечислениям  организациям (без гос. и мун.) ", 1);
            valuesDictionary.Add("По пособиям по социальной помощи населению ", 1);
            valuesDictionary.Add("Прочие расходы ", 1);
            valuesDictionary.Add("Основные средства ", 1);
            valuesDictionary.Add("Материальные запасы ", 1);
//            valuesDictionary.Add("Прочая кредиторская задолженность ", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список кодами территорий
        /// </summary>
        public static Dictionary<string, int> FillRegionCodeList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Все ", 0);
            valuesDictionary.Add("кон ", 0);
            valuesDictionary.Add("МР ", 0);
            valuesDictionary.Add("ГО ", 0);
            valuesDictionary.Add("ГСП ", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список типами территорий
        /// </summary>
        public static Dictionary<string, int> FillRegionTypeList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Все ", 0);
            valuesDictionary.Add("МР ", 0);
            valuesDictionary.Add("ГО ", 0);
            valuesDictionary.Add("ГП ", 0);
            valuesDictionary.Add("СП ", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список вариантами фондов ФО
        /// </summary>
        public static Dictionary<string, int> FillFOFondVariantList(Dictionary<string, string> variantList)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in variantList.Keys)
            {
                valuesDictionary.Add(key, 0);
            }
            return valuesDictionary;
        }  

        /// <summary>
        /// Заполнить список территориями
        /// </summary>
        public static Dictionary<string, int> FillTerritories(Dictionary<string, string> localBudgetTypes)
        {
            return FillTerritories(localBudgetTypes, true, true);
        }

        /// <summary>
        /// Заполнить список территориями
        /// </summary>
        /// <param name="allLevel">нужен ли пункт "Все территории"</param>     
        public static Dictionary<string, int> FillTerritories(Dictionary<string, string> localBudgetTypes, bool allLevel)
        {
            return FillTerritories(localBudgetTypes, allLevel, true);
        }

        /// <summary>
        /// Заполнить список территориями
        /// </summary>
        /// <param name="allLevel">нужен ли пункт "Все территории"</param>        
        /// <param name="districtLevel">нужны ли городские округа</param>
        public static Dictionary<string, int> FillTerritories(Dictionary<string, string> localBudgetTypes, bool allLevel, bool districtLevel)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            if (allLevel)
            {
                valuesDictionary.Add("Все территории", 0);
            }
            valuesDictionary.Add("Муниципальные районы", 0);
            foreach (string key in localBudgetTypes.Keys)
            {
                if (localBudgetTypes[key].Contains("МР"))
                {
                    valuesDictionary.Add(key, 1);
                }
            }

            if (districtLevel)
            {
                valuesDictionary.Add("Городские округа", 0);
                foreach (string key in localBudgetTypes.Keys)
                {
                    if (localBudgetTypes[key].Contains("ГО"))
                    {
                        valuesDictionary.Add(key, 1);
                    }
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список кварталами
        /// </summary>
        public static Dictionary<string, int> FillQuaters()
        {
            return FillQuaters(false);
        }

        /// <summary>
        /// Заполнить список кварталами
        /// </summary>
        /// <param name="allQuarters">нужен ли пункт "Весь год"</param>
        public static Dictionary<string, int> FillQuaters(bool allQuarters)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            if (allQuarters)
            {
                valuesDictionary.Add("Весь год", 0);
            }
            for (int i = 1; i <= 4; i++)
            {
                valuesDictionary.Add(String.Format("Квартал {0}", i), 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список кварталами для отчетов МОФО
        /// </summary>
        public static Dictionary<string, int> FillMOFOQuaters()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("1 квартал", 0);
            valuesDictionary.Add("Полугодие", 0);
            valuesDictionary.Add("9 месяцев", 0);
            valuesDictionary.Add("Итоговый за год", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список датами на начало кварталов
        /// </summary>
        public static Dictionary<string, int> FillDateQuarters()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("по состоянию на 01.04", 0);
            valuesDictionary.Add("по состоянию на 01.07", 0);
            valuesDictionary.Add("по состоянию на 01.10", 0);
            valuesDictionary.Add("по итогам года", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список кварталами
        /// </summary>
        public static Dictionary<string, int> FillMonitoringQuarters()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("мониторинг за 1 квартал", 0);
            valuesDictionary.Add("мониторинг за 2 квартал", 0);
            valuesDictionary.Add("мониторинг за 3 квартал", 0);
            valuesDictionary.Add("мониторинг за 4 квартал (по итогам года)", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список кварталами для оценки качества
        /// </summary>
        public static Dictionary<string, int> FillEvaluaitonQuarters()
        {
            return FillEvaluaitonQuarters(false);
        }

        /// <summary>
        /// Заполнить список кварталами для оценки качества
        /// </summary>
        public static Dictionary<string, int> FillEvaluaitonQuarters(bool includeFirstQuarter)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            if (includeFirstQuarter)
            {
                valuesDictionary.Add("Оценка качеcтва на 01.04", 0);
            }
            valuesDictionary.Add("Оценка качеcтва на 01.07", 0);
            valuesDictionary.Add("Оценка качества на 01.10", 0);
            valuesDictionary.Add("Оценка качества по итогам года", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список уровнями бюджета
        /// </summary>
        public static Dictionary<string, int> FillBudgetLevels(Dictionary<string, string> budgetLevelTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Все уровни", 0);
            foreach (string key in budgetLevelTypes.Keys)
            {
                valuesDictionary.Add(key, 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполняет списком регионов.
        /// </summary>
        public static Dictionary<string, int> FillRegions(Collection<string> foNames, DataTable regionsFoDictionary)
        {
            return FillRegions(foNames, regionsFoDictionary, false);
        }

        /// <summary>
        /// Заполняет списком регионов.
        /// </summary>
        public static Dictionary<string, int> FillRegions(Collection<string> foNames, DataTable regionsFoDictionary, bool rfLevel)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            if (rfLevel)
            {
                valuesDictionary.Add("Российская  Федерация", 0);
            }
            for (int i = 0; i < foNames.Count; i++)
            {
                valuesDictionary.Add(foNames[i], 0);

                DataRow[] rows = regionsFoDictionary.Select(
                        string.Format("FK like '{0}'", foNames[i]));
                for (int j = 0; j < rows.Length; j++)
                {
                    valuesDictionary.Add(rows[j][0].ToString(), 1);
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список территориями c поселениями
        /// </summary>
        /// <param name="allLevel">нужен ли пункт "Все территории"</param>        
        public static Dictionary<string, int> FillSettlements(Dictionary<string, string> localSettlementTypes, bool allLevel)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            if (allLevel)
            {
                valuesDictionary.Add("Все территории", 0);
            }
            valuesDictionary.Add("Муниципальные районы", 0);
            bool goExists = false;
            bool grGroup = false;

            foreach (string key in localSettlementTypes.Keys)
            {
                string type = localSettlementTypes[key];

                if (type.Contains("МР") || type.Contains("ГО") || type.Contains("ГР"))
                {
                    if (!goExists && type.Contains("ГО"))
                    {
                        valuesDictionary.Add("Городские округа ", 0);
                        goExists = true;
                    }
                    if (type.Contains("ГР"))
                    {
                        valuesDictionary.Add(key, 2);
                        grGroup = true;
                    }
                    else
                    {
                        valuesDictionary.Add(key, 1);
                        grGroup = false;
                    }
                }
                else if (type.Contains("СП") || type.Contains("ГП") || type.Contains("РЦ"))
                {
                    if (grGroup)
                    {
                        valuesDictionary.Add(key, 3);
                    }
                    else
                    {
                        valuesDictionary.Add(key, 2);
                    }
                }
            }

            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список муниципальных районов для отчетов по МОФО
        /// </summary>
        public static Dictionary<string, int> FillMOFORegionsList(Dictionary<string, string> mofoRegionsTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
                       
            bool goExists = false;
            bool mrExists = false;

            valuesDictionary.Add("Местные бюджеты – Всего", 0);
            valuesDictionary.Add("Собственные бюджеты МР - Всего", 0);
            valuesDictionary.Add("Бюджеты ГО – Всего", 0);
            valuesDictionary.Add("Бюджеты поселений – Всего", 0);
            valuesDictionary.Add("Бюджеты сельских поселений", 1);
            valuesDictionary.Add("Бюджеты городских поселений", 1);
            valuesDictionary.Add("Местные бюджеты – с разбивкой", 0);

            foreach (string key in mofoRegionsTypes.Keys)
            {
                string type = mofoRegionsTypes[key];

//                if (type.Contains("СБ"))
//                {
//                    valuesDictionary.Add(key, 0);
//                }
                if (type.Contains("МР") || type.Contains("ГО"))
                {
                    if (!goExists && type.Contains("ГО"))
                    {
                        valuesDictionary.Add("Городские округа ", 1);
                        goExists = true;
                    }
                    if (!mrExists && type.Contains("МР"))
                    {
                        valuesDictionary.Add("Консолид.бюджеты районов", 1);
                        mrExists = true;
                    }
                    valuesDictionary.Add(key, 2);
                }
                else if (type.Contains("СП") || type.Contains("ГП"))
                {
                    valuesDictionary.Add(key, 3);
                }
            }

            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список непустыми днями кассового плана
        /// </summary>
        public static Dictionary<string, int> FillCashPlanNonEmptyDays(Dictionary<string, string> cashPlanDays)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in cashPlanDays.Keys)
            {
                string type = cashPlanDays[key];
                switch (type)
                {
                    case "Year":
                        {
                            valuesDictionary.Add(key, 0);
                            break;
                        }
                    case "Month":
                        {
                            valuesDictionary.Add(key, 1);
                            break;
                        }
                    case "Day":
                        {
                            valuesDictionary.Add(key, 2);
                            break;
                        }
                }
            }

            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список бюджетов кассового плана (для Костромы)
        /// </summary>
        public static Dictionary<string, int> FillCashPlanBudgetList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Областной бюджет ", 0);
            valuesDictionary.Add("г.Кострома ", 0);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список непустыми днями оперативной информации
        /// </summary>
        public static Dictionary<string, int> FillHotInfoNonEmptyDays(Dictionary<string, string> hotInfoDays)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in hotInfoDays.Keys)
            {
                string type = hotInfoDays[key];
                switch (type)
                {
                    case "Year":
                        {
                            valuesDictionary.Add(key, 0);
                            break;
                        }
                    case "Month":
                        {
                            valuesDictionary.Add(key, 1);
                            break;
                        }
                    case "Day":
                        {
                            valuesDictionary.Add(key, 2);
                            break;
                        }
                }
            }

            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список непустыми днями мониторинга рынка труда
        /// </summary>
        public static Dictionary<string, int> FillLabourMarketNonEmptyDays(Dictionary<string, string> hotInfoDays)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in hotInfoDays.Keys)
            {
                string type = hotInfoDays[key];
                switch (type)
                {
                    case "Year":
                        {
                            valuesDictionary.Add(key, 0);
                            break;
                        }
                    case "Month":
                        {
                            valuesDictionary.Add(key, 1);
                            break;
                        }
                    case "Day":
                        {
                            valuesDictionary.Add(key, 2);
                            break;
                        }
                }
            }

            return valuesDictionary;
        }

        public static Dictionary<string, int> FillCreditsNonEmptyDays(Dictionary<string, string> hotInfoDays)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in hotInfoDays.Keys)
            {
                string type = hotInfoDays[key];
                switch (type)
                {
                    case "Year":
                        {
                            valuesDictionary.Add(key, 0);
                            break;
                        }
                    case "Month":
                        {
                            valuesDictionary.Add(key, 1);
                            break;
                        }
                    case "Day":
                        {
                            valuesDictionary.Add(key, 2);
                            break;
                        }
                }
            }

            return valuesDictionary;
        }


        /// <summary>
        /// Заполнить список непустыми днями для распределения МБТ
        /// </summary>
        public static Dictionary<string, int> FillMBTNonEmptyDays(Dictionary<string, string> hotInfoDays)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in hotInfoDays.Keys)
            {
                string type = hotInfoDays[key];
                switch (type)
                {
                    case "Year":
                        {
                            valuesDictionary.Add(key, 0);
                            break;
                        }
                    case "Month":
                        {
                            valuesDictionary.Add(key, 1);
                            break;
                        }
                    case "Day":
                        {
                            valuesDictionary.Add(key, 2);
                            break;
                        }
                }
            }

            return valuesDictionary;
        }
//         public static XmlNode FillSettlements(DataTable dt, bool allLevel)
//         {
//             XmlDocument xmlDoc = new XmlDocument();
//             XmlNode root = xmlDoc.DocumentElement;
//             XmlNode allTerritories = xmlDoc.AppendChild(root);
//             XmlNode regions = xmlDoc.AppendChild(root);
//             XmlNode towns = xmlDoc.AppendChild(root);
// 
//             for (int i = 0; i < dt.Rows.Count; i++)
//             {
//                 DataRow row = dt.Rows[i];
//                 string name = (row[0] == DBNull.Value) ? string.Empty : row[0].ToString();
//                 string type = (row[1] == DBNull.Value) ? string.Empty : row[1].ToString();
//                 string uniqName = (row[2] == DBNull.Value) ? string.Empty : row[2].ToString();
//
//                 if (type == "ГО")
//                 {
//                     XmlNode town = xmlDoc.AppendChild(root);
//                     XmlAttribute nameAttribute = town.Attributes["name"];
//                     nameAttribute.Value = name;
//                     XmlAttribute nameType = town.Attributes["type"];
//                     nameType.Value = type;
//                     XmlAttribute nameUniqName = town.Attributes["uniqName"];
//                     nameUniqName.Value = uniqName;
//                 }
//             }
//         }

        /// <summary>
        /// Заполнить список показателями мониторинга рынка труда
        /// </summary>
        public static Dictionary<string, int> FillLabourMarketIndicators(Dictionary<string, string> labourMarketIndicatorsTypes)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in labourMarketIndicatorsTypes.Keys)
            {
                if (labourMarketIndicatorsTypes[key].Contains("из них обратившихся в службу занятости"))
                {
                    valuesDictionary.Add(key, 1);
                }
                else
                {
                    valuesDictionary.Add(key, 0);
                }
            }
            return valuesDictionary;
        }

        public static Dictionary<string, int> FillFOSubjectList(Dictionary<string, string> foSubjectList, bool allLevel)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            int level = 0;
            if (allLevel)
            {
                valuesDictionary.Add("Уральский федеральный округ", level);
                //level++;
            }
            
            foreach (string key in foSubjectList.Keys)
            {
                valuesDictionary.Add(key, level);
            }
            return valuesDictionary;
        }

//        /// <summary>
//        /// Заполнить список показателями по исполнению бюджетов муниципальных образований
//        /// </summary>
//        public static Dictionary<string, int> FillMOIndicatorList()
//        {
//            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
//            valuesDictionary.Add("Доходы бюджета ", 0);
//            valuesDictionary.Add("Налоговые и неналоговые доходы ", 1);
//            valuesDictionary.Add("Налог на доходы физических лиц ", 2);
//            valuesDictionary.Add("Безвозмездные поступления ", 1);
//            valuesDictionary.Add("Дотации ", 2);
//            valuesDictionary.Add("Субвенции ", 2);
//            valuesDictionary.Add("Субсидии ", 2);
//            valuesDictionary.Add("Иные ", 2);
//            valuesDictionary.Add("Расходы бюджета ", 0);
//            valuesDictionary.Add("Заработная плата и начисления на выплаты по оплате труда ", 1);
//            valuesDictionary.Add("Коммунальные услуги ", 1);
//            valuesDictionary.Add("Увеличение стоимости основных средств ", 1);
//            valuesDictionary.Add("Результат исполнения бюджета (дефицит/профицит) ", 0);
//            valuesDictionary.Add("Кредиты кредитных организаций ", 0);
//            valuesDictionary.Add("Бюджетные кредиты от других бюджетов бюджетной системы ", 0);
//            valuesDictionary.Add("Муниципальный долг ", 0);
//            valuesDictionary.Add("Остатки бюджетных средств ", 0);
//            valuesDictionary.Add("имеющих целевое назначение ", 1);
//            valuesDictionary.Add("имеющие нецелевое назначение ", 1);
//            valuesDictionary.Add("Просроченная кредиторская задолженность ", 0);
//            valuesDictionary.Add("по заработной плате ", 1);
//            valuesDictionary.Add("по начислениям на выплаты по оплате труда ", 1);
//            valuesDictionary.Add("по оплате коммунальных услуг ", 1);
//            valuesDictionary.Add("по услугам связи ", 1);
//            valuesDictionary.Add("по транспортным услугам ", 1);
//            valuesDictionary.Add("по работам, услугам по содержанию имущества ", 1);
//            valuesDictionary.Add("по прочим работам, услугам ", 1);
//            valuesDictionary.Add("по безвозмездным перечислениям гос. и мун-м организациям ", 1);
//            valuesDictionary.Add("по пособиям по соц. помощи населению ", 1);
//            valuesDictionary.Add("по прочим расходам ", 1);
//            valuesDictionary.Add("по оплате договоров на приобретение объектов, относящихся к основным средствам ", 1);
//            valuesDictionary.Add("по оплате договоров на приобретение сырья и материалов на оказание гос. (мун-х) услуг ", 1);
//            return valuesDictionary;
//        }

        /// <summary>
        /// Заполнить список показателями по исполнению бюджетов муниципальных образований
        /// </summary>
        public static Dictionary<string, int> FillMOIndicatorList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Доходы бюджета ", 0);
             valuesDictionary.Add("Налоговые и неналоговые доходы ", 1);
              valuesDictionary.Add("Налог на доходы физических лиц ", 2);
              valuesDictionary.Add("Налоги на совокупный доход ", 2);
              valuesDictionary.Add("Земельный налог ", 2);
              valuesDictionary.Add("Транспортный налог ", 2);
              valuesDictionary.Add("Налог на имущество физических лиц ", 2);
              valuesDictionary.Add("Прочие налоговые доходы ", 2);
              valuesDictionary.Add("Неналоговые доходы ", 2);
             valuesDictionary.Add("Безвозмездные поступления ", 1);
              valuesDictionary.Add("Дотации ", 2);
               valuesDictionary.Add("на выравнивание бюдж. обеспеченности ", 3);
              valuesDictionary.Add("Субвенции ", 2);
              valuesDictionary.Add("Субсидии ", 2);
              valuesDictionary.Add("Иные межбюджетные трансферты ", 2);

            valuesDictionary.Add("Расходы бюджета ", 0);

             valuesDictionary.Add("В том числе по разделам ", 1);
              valuesDictionary.Add("Общегосударственные вопросы ", 2);
              valuesDictionary.Add("Нац.оборона ", 2);
              valuesDictionary.Add("Нац.безопасность и правоохр. деятельность ", 2);
              valuesDictionary.Add("Национальная экономика ", 2);
              valuesDictionary.Add("ЖКХ ", 2);
              valuesDictionary.Add("Охрана окр.среды ", 2);
              valuesDictionary.Add("Образование ", 2);
              valuesDictionary.Add("Культура, кинематография ", 2);
              valuesDictionary.Add("Здравоохранение ", 2);
              valuesDictionary.Add("Социальная политика ", 2);
              valuesDictionary.Add("Физическая культура и спорт ", 2);
              valuesDictionary.Add("СМИ ", 2);
              valuesDictionary.Add("Обслуживание гос. и мун. долга ", 2);
              valuesDictionary.Add("Межбюджетн.трансферты общего характера бюджетам субъектов РФ и МО ", 2);
              valuesDictionary.Add("Межбюджетные трансферты ", 2);

             valuesDictionary.Add("В том числе по КОСГУ ", 1);
              valuesDictionary.Add("Заработная плата и начисления на выплаты по оплате труда ", 2);
               valuesDictionary.Add("Заработная плата ", 3);
               valuesDictionary.Add("Прочие выплаты ", 3);
               valuesDictionary.Add("Начисления на выплаты по оплате труда ", 3);
              valuesDictionary.Add("Оплата работ, услуг ", 2);
               valuesDictionary.Add("Услуги связи ", 3);
               valuesDictionary.Add("Транспортные услуги ", 3);
               valuesDictionary.Add("Коммунальные услуги ", 3);
               valuesDictionary.Add("Арендная плата за пользование имуществом ", 3);
               valuesDictionary.Add("Работы, услуги по содержанию имущества ", 3);
               valuesDictionary.Add("Прочие работы, услуги ", 3);
              valuesDictionary.Add("Обслуживание государственного (муниципального) долга ", 2);
               valuesDictionary.Add("Обслуживание внутреннего долга ", 3);
              valuesDictionary.Add("Безвозмездные перечисления организациям ", 2);
               valuesDictionary.Add("Безвозм. перечисления гос. и мун. организациям ", 3);
               valuesDictionary.Add("Безвозм. перечисления организациям, за исключением гос. и мун. организаций ", 3);
              valuesDictionary.Add("Безвозмездные перечисления бюджетам ", 2);
              valuesDictionary.Add("Социальное обеспечение ", 2);
               valuesDictionary.Add("Пособия по соц. помощи населению ", 3);
               valuesDictionary.Add("Пенсии, пособия, выплачиваемые организациями сектора гос. управления ", 3);
              valuesDictionary.Add("Прочие расходы ", 2);
              valuesDictionary.Add("Увеличение стоимости основных средств ", 2);
              valuesDictionary.Add("Увеличение стоимости нематериальных активов ", 2);
              valuesDictionary.Add("Увеличение стоимости материальных запасов ", 2);
              
            valuesDictionary.Add("Результат исполнения бюджета (дефицит/профицит) ", 0);

            valuesDictionary.Add("Кредиты кредитных организаций ", 0);
            valuesDictionary.Add("Бюджетные кредиты от других бюджетов бюджетной системы ", 0);
            valuesDictionary.Add("Муниципальный долг ", 0);
            valuesDictionary.Add("Остатки бюджетных средств ", 0);
            valuesDictionary.Add("имеющих целевое назначение ", 1);
            valuesDictionary.Add("имеющие нецелевое назначение ", 1);

            valuesDictionary.Add("Просроченная кредиторская задолженность ", 0);
            valuesDictionary.Add("по заработной плате ", 1);
            valuesDictionary.Add("по начислениям на выплаты по оплате труда ", 1);
            valuesDictionary.Add("по оплате коммунальных услуг ", 1);
            valuesDictionary.Add("по услугам связи ", 1);
            valuesDictionary.Add("по транспортным услугам ", 1);
            valuesDictionary.Add("по работам, услугам по содержанию имущества ", 1);
            valuesDictionary.Add("по прочим работам, услугам ", 1);
            valuesDictionary.Add("по безвозм. перечислениям гос. и мун-м организациям ", 1);
            valuesDictionary.Add("по безвозм. перечислениям организациям, за исключением гос. и мун. организаций ", 1);
            valuesDictionary.Add("по пособиям по соц. помощи населению ", 1);
            valuesDictionary.Add("по прочим расходам ", 1);
            valuesDictionary.Add("по оплате договоров на приобретение объектов, относящихся к основным средствам ", 1);
            valuesDictionary.Add("по оплате договоров на приобретение сырья и материалов на оказание гос. (мун-х) услуг ", 1);
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список администраторами МБТ
        /// </summary>
        public static Dictionary<string, int> FillMBTAdministratorList(Dictionary<string, string> mbtAdministratorUniqNames, Dictionary<string, string> mbtAdministratorLevels)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in mbtAdministratorUniqNames.Keys)
            {
                if (mbtAdministratorLevels.ContainsKey(key))
                {
                    string levelName = mbtAdministratorLevels[key];
                    int level = 0;
                    switch (levelName)
                    {
                        case "Расходы уровень 1":
                            {
                                level = 0;
                                break;
                            }
                        case "Расходы уровень 3":
                            {
                                level = 1;
                                break;
                            }
                    }
                    valuesDictionary.Add(key.TrimEnd('_'), level);
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполняет параметр полным списком уровней бюджетов с поселениями
        /// </summary>
        public static Dictionary<string, int> FillFullBudgetLevels(Dictionary<string, string> fullBudgetLevelNumbers,
            Dictionary<string, string> fullBudgetLevelUniqNames, Dictionary<string, string> fullBudgetRegionUniqNames)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in fullBudgetLevelNumbers.Keys)
            {
                string levelNumber = fullBudgetLevelNumbers[key];
                if (levelNumber != string.Empty)
                {
                    int level = Convert.ToInt32(levelNumber);
                    valuesDictionary.Add(key.TrimEnd('_'), level);
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполняет параметр списком групп МО для отчетов БККУ
        /// </summary>
        public static Dictionary<string, int> FillBKKUGroupMO()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Все группы ", 0);
            //valuesDictionary.Add("со значением 0 ", 1);
            valuesDictionary.Add("со значением 1 ", 1);
            valuesDictionary.Add("со значением 2 ", 1);
            valuesDictionary.Add("со значением 3 ", 1);
            return valuesDictionary;
        }
        
        /// <summary>
        /// Заполнить список дополнительными мероприятиями
        /// </summary>
        public static Dictionary<string, int> FillAdditionalActivityMainLevels(Dictionary<string, string> additionalActivityUniqueNames, Dictionary<string, string> additionalActivityLevels)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            valuesDictionary.Add("Все показатели", 0);
            foreach (string key in additionalActivityUniqueNames.Keys)
            {
                if (additionalActivityLevels.ContainsKey(key))
                {
                    string level = additionalActivityLevels[key];
                    switch(level)
                    {
                        case "Показатели уровень 1":
                            {
                                valuesDictionary.Add(key, 0);
                                break;
                            }
                        case "Показатели уровень 2":
                            {
                                valuesDictionary.Add(key, 1);
                                break;
                            }
                    }
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список дополнительными мероприятиями
        /// </summary>
        public static Dictionary<string, int> FillAdditionalActivityDetailLevels(Dictionary<string, string> additionalActivityUniqueNames, Dictionary<string, string> additionalActivityLevels)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            valuesDictionary.Add("Все мероприятия", 0);
            foreach (string key in additionalActivityUniqueNames.Keys)
            {
                if (additionalActivityLevels.ContainsKey(key))
                {
                    string level = additionalActivityLevels[key];
                    switch (level)
                        {
                            case "Показатели уровень 3":
                                {
                                    valuesDictionary.Add(key, 0);
                                    break;
                                }
                            case "Показатели уровень 05":
                                {
                                    valuesDictionary.Add(key, 1);
                                    break;
                                }
                        }
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список основными мероприятиями
        /// </summary>
        public static Dictionary<string, int> FillMainActivityList(Dictionary<string, string> mainActivityUniqueNames, Dictionary<string, string> mainActivityLevels)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            valuesDictionary.Add("Все мероприятия", 0);
            foreach (string key in mainActivityUniqueNames.Keys)
            {
                if (mainActivityLevels.ContainsKey(key))
                {
                    string level = mainActivityLevels[key];
                    switch (level)
                    {
                        case "Мероприятия уровень 2":
                            {
                                valuesDictionary.Add(key, 0);
                                break;
                            }
                        case "Мероприятия уровень 3":
                            {
                                valuesDictionary.Add(key, 1);
                                break;
                            }
                        case "Мероприятия уровень 05":
                            {
                                valuesDictionary.Add(key, 2);
                                break;
                            }
                    }
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить список территориями для УрФО
        /// </summary>
        public static Dictionary<string, int> FillUrFORegionList(Dictionary<string, string> urfoRegionUniqueNames, Dictionary<string, string> urfoRegionLevels)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            valuesDictionary.Add("Уральский федеральный округ", 0);
            foreach (string key in urfoRegionUniqueNames.Keys)
            {
                if (urfoRegionLevels.ContainsKey(key))
                {
                    string level = urfoRegionLevels[key];
                    switch (level)
                    {
                        case "Районы":
                            {
                                valuesDictionary.Add(key, 0);
                                break;
                            }
                        case "Районы уровень 04":
                            {
                                valuesDictionary.Add(key, 1);
                                break;
                            }
                    }
                }
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить списоком показателей для оценки качества
        /// </summary>
        public static Dictionary<string, int> FillQualityEvaluationIndicatorList(Dictionary<string, string> qualityEvaluationIndicatorList)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            foreach (string key in qualityEvaluationIndicatorList.Keys)
            {
                valuesDictionary.Add(key, 0);
            }
            return valuesDictionary;
        }

        /// <summary>
        /// Заполнить списоком показателей для значений качества
        /// </summary>
        public static Dictionary<string, int> FillQualityValueIndicatorList(Dictionary<string, string> qualityValueIndicatorList)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            foreach (string key in qualityValueIndicatorList.Keys)
            {
                valuesDictionary.Add(key, 0);
            }
            return valuesDictionary;
        }


        private static string GetDictionaryUniqueKey(Dictionary<string, int> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        /// <summary>
        /// Заполнить список мемберами
        /// </summary>
        public static Dictionary<string, int> FillMemberUniqueNameList(Dictionary<string, string> memberUniqueNames, Dictionary<string, string> memberLevels)
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            foreach (string key in memberUniqueNames.Keys)
            {
                if (memberLevels.ContainsKey(key) && memberLevels[key] != String.Empty)
                {
                    int level = Convert.ToInt32(memberLevels[key]);
                    valuesDictionary.Add(key, level);
                }
            }
            return valuesDictionary;
        }
    }
}