using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core
{
    /// <summary>
    /// Воспомогательный класс, предоставляющий методы работы с наименованиями регионов.
    /// </summary>
    // Регионы одни на все приложение, можно сделать статическими.
    public static class RegionsNamingHelper
    {
        /// <summary>
        /// Словарь сокращенных наименований.
        /// </summary>
        private static Dictionary<string, string> shortRegionsNames;

        private static DataTable regionsFoDictionary;
        private static Collection<string> foNames;

        public static Collection<string> FoNames
        {
            get
            {
                if (foNames == null || foNames.Count == 0)
                {
                    // заполняем словарик
                    FillRegionsFoDictionary();
                }
                return foNames;
            }
        }
       
        public static DataTable RegionsFoDictionary
        {
            get
            {
                if (regionsFoDictionary == null || regionsFoDictionary.Rows.Count == 0)
                {
                    // заполняем словарик
                    FillRegionsFoDictionary();
                }
                return regionsFoDictionary;
            }
        }

        private static void FillRegionsFoDictionary()
        {
            regionsFoDictionary = new DataTable();
            string query = DataProvider.GetQueryText("regionsFoDictionary", HttpContext.Current.Request.PhysicalApplicationPath);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Регион", regionsFoDictionary);
            foNames = new Collection<string>();
            foreach (DataRow row in regionsFoDictionary.Rows)
            {
                if (!foNames.Contains(row[1].ToString()))
                {
                    foNames.Add(row[1].ToString());
                }
            }
        }

        public static string GetFoBySubject(string subject)
        {
            DataRow[] rows = RegionsFoDictionary.Select(String.Format("Регион='{0}'", subject));
            if (rows.Length > 0)
            {
                return rows[0][1].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Возвращает словарик сокращенных наименований, если запрос первый, то инициирует
        /// </summary>
        public static Dictionary<string, string> ShortRegionsNames
        {
            get
            {
                // если короткие имена регионов еще не получены
                if (shortRegionsNames == null || shortRegionsNames.Count == 0)
                {
                    // заполняем словарик
                    FillShortRegionsNames();
                }
                return shortRegionsNames;
            }
        }

        private static void FillShortRegionsNames()
        {
            shortRegionsNames = new Dictionary<string, string>();
            string query = DataProvider.GetQueryText("shortRegionsNames", HttpContext.Current.Request.PhysicalApplicationPath);
            DataTable dt = new DataTable();

            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Регион", dt);
            foreach (DataRow row in dt.Rows)
            {
                // пока нет нормального запроса с именами ФО, будем делать глупо.
                string key = row[0].ToString();
                key = key.Trim('(');
                key = key.Replace(" ДАННЫЕ)", string.Empty);
                shortRegionsNames.Add(key, row[1].ToString());
            }
        }

        /// <summary>
        /// Заменяет имена регионов в дататейбле на сокращенные.
        /// </summary>
        /// <param name="dt">Исходная таблица.</param>
        /// <param name="namesColNum">Номер колонки с именами.</param>
        public static void ReplaceRegionNames(DataTable dt, int namesColNum)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (ShortRegionsNames.ContainsKey(row[namesColNum].ToString()))
                {
                    row[namesColNum] = ShortRegionsNames[row[namesColNum].ToString()];
                }
            }
        }

        /// <summary>
        /// Возвращает короткое наименование региона или ФО по полному.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string ShortName(string fullName)
        {
            if (fullName == null)
            {
                return String.Empty;
            }

            if (fullName == "Российская Федерация")
            {
                return "РФ";
            }
            if (ShortRegionsNames.ContainsKey(fullName))
            {
                return ShortRegionsNames[fullName];
            }
            return fullName;
        }

        /// <summary>
        /// Возвращает полное имя по короткому наименованию
        /// </summary>
        /// <param name="ShortName">короткое имя</param>
        /// <returns>полное имя</returns>
        public static string FullName(string ShortName)
        {
            if (ShortRegionsNames.ContainsValue(ShortName))
            {
                foreach (string key in ShortRegionsNames.Keys)
                {
                    if (ShortRegionsNames[key] == ShortName)
                    {
                        return key;
                    }
                }
            }
            return ShortName;
        }

        /// <summary>
        /// Является ли имя наменованием РФ
        /// </summary>
        /// <param name="subject">имя</param>
        /// <returns>true - если является</returns>
        public static bool IsRF(string subject)
        {
            return subject.Contains("Федерация");
        }

        /// <summary>
        /// Является ли имя наменованием федерального округа
        /// </summary>
        /// <param name="subject">имя</param>
        /// <returns>true - если является</returns>
        public static bool IsFO(string subject)
        {
            return FoNames.Contains(subject);
        }

        /// <summary>
        /// Является ли имя наменованием субъекта
        /// </summary>
        /// <param name="subject">имя</param>
        /// <returns>true - если является</returns>
        public static bool IsSubject(string subject)
        {
            return (!IsFO(subject) && !IsRF(subject));
        }

        #region Локальные бюджеты

        private static string localBudgetTypesCacheKey
        {
            get
            {
                return RegionSettingsHelper.Instance.GetCacheKey("localBudgetTypes");
            }
        }

        private static string localBudgetUniqueNamesCacheKey
        {
            get
            {
                return RegionSettingsHelper.Instance.GetCacheKey("localBudgetUniqueNames");
            }
        }

        private static string localRegionLevelUniqNameCacheKey
        {
            get
            {
                return RegionSettingsHelper.Instance.GetCacheKey("localRegionLevelUniqName");
            }
        }

        private static string localCityLevelUniqNameCacheKey
        {
            get
            {
                return RegionSettingsHelper.Instance.GetCacheKey("localCityLevelUniqName");
            }
        }

        private static string localBudgetCodesCacheKey
        {
            get
            {
                return RegionSettingsHelper.Instance.GetCacheKey("localBudgetCodes");
            }
        }

        private static bool LocalRegionLevelUniqNamesEmpty()
        {
            return HttpContext.Current.Cache[localRegionLevelUniqNameCacheKey] == null;
        }

        private static bool LocalCityLevelUniqNamesEmpty()
        {
            return HttpContext.Current.Cache[localCityLevelUniqNameCacheKey] == null;
        }

        private static bool LocalBudgetUniqueNamesEmpty()
        {
            return HttpContext.Current.Cache[localBudgetUniqueNamesCacheKey] == null ||
                   ((Dictionary<string, string>)HttpContext.Current.Cache[localBudgetUniqueNamesCacheKey]).Count == 0;
        }

        private static bool LocalBudgetTypesEmpty()
        {
            return HttpContext.Current.Cache[localBudgetTypesCacheKey] == null ||
                   ((Dictionary<string, string>)HttpContext.Current.Cache[localBudgetTypesCacheKey]).Count == 0;
        }

        private static bool LocalBudgetCodesEmpty()
        {
            return HttpContext.Current.Cache[localBudgetCodesCacheKey] == null ||
                   ((Dictionary<string, string>)HttpContext.Current.Cache[localBudgetCodesCacheKey]).Count == 0;
        }

        /// <summary>
        /// Уникальное имя уровня районов бюджета
        /// </summary>
        public static string LocalRegionLevelUniqName
        {
            get
            {
                // если словарь пустой
                if (LocalRegionLevelUniqNamesEmpty())
                {
                    // заполняем его
                    FillLocalBudgetTypes();
                }
                return HttpContext.Current.Cache[localRegionLevelUniqNameCacheKey].ToString();
            }
        }

        /// <summary>
        /// Уникальное имя уровня городов бюджета
        /// </summary>
        public static string LocalCityLevelUniqName
        {
            get
            {
                // если словарь пустой
                if (LocalCityLevelUniqNamesEmpty())
                {
                    // заполняем его
                    FillLocalBudgetTypes();
                }
                return HttpContext.Current.Cache[localCityLevelUniqNameCacheKey].ToString();
            }
        }

        /// <summary>
        /// Возвращает словарик типов местных бюджетов
        /// </summary>
        public static Dictionary<string, string> LocalBudgetTypes
        {
            get
            {
                // если словарь пустой
                if (LocalBudgetTypesEmpty())
                {
                    // заполняем его
                    FillLocalBudgetTypes();
                }
                return (Dictionary<string, string>)HttpContext.Current.Cache[localBudgetTypesCacheKey];
            }
        }

        /// <summary>
        /// Возвращает словарик уникальных имен местных бюджетов
        /// </summary>
        public static Dictionary<string, string> LocalBudgetUniqueNames
        {
            get
            {
                // если словарь пустой
                if (LocalBudgetUniqueNamesEmpty())
                {
                    // заполняем его
                    FillLocalBudgetTypes();
                }
                return (Dictionary<string, string>)HttpContext.Current.Cache[localBudgetUniqueNamesCacheKey];
            }
        }

        /// <summary>
        /// Возвращает словарик кодов карты местных бюджетов
        /// </summary>
        public static Dictionary<string, string> LocalBudgetCodes
        {
            get
            {
                // если словарь пустой
                if (LocalBudgetCodesEmpty())
                {
                    // заполняем его
                    FillLocalBudgetTypes();
                }
                return (Dictionary<string, string>)HttpContext.Current.Cache[localBudgetCodesCacheKey];
            }
        }

        private static void FillLocalBudgetTypes()
        {
            Dictionary<string, string> localBudgetTypes = new Dictionary<string, string>();
            Dictionary<string, string> localBudgetUniqueNames = new Dictionary<string, string>();
            Dictionary<string, string> localBudgetCodes = new Dictionary<string, string>();

            DataTable dt = new DataTable();

            CustomParam.CustomParamFactory("localLevelName").Value = RegionSettingsHelper.Instance.RegionsLevel;
            CustomParam.CustomParamFactory("regionDimesion").Value = RegionSettingsHelper.Instance.RegionDimension;
            CustomParam.CustomParamFactory("ownSubjectBudgetName").Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            string query = DataProvider.GetQueryText("localBudgets", HttpContext.Current.Request.PhysicalApplicationPath);
                        
            //query = query.Replace("<@localLevelName@>", Singleton<MdxQueryRenameService>.Instance.Rename(RegionSettingsHelper.Instance.RegionsLevel));
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dt);
            // значения дублируются почему-то именно 3 раза (уже 4)
            int multiplyValueCount = 4;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                string type = (row[1] == DBNull.Value) ? string.Empty : row[1].ToString();
                string uniqName = (row[2] == DBNull.Value) ? string.Empty : row[2].ToString();
                string code = (row[3] == DBNull.Value) ? string.Empty : row[3].ToString();

                type = CheckMultiplyValue(type, multiplyValueCount);
                uniqName = CheckMultiplyValue(uniqName, multiplyValueCount);
                code = CheckMultiplyValue(code, multiplyValueCount);
                
                switch (i)
                {
                    case 0:
                        {
                            HttpContext.Current.Cache.Insert(localRegionLevelUniqNameCacheKey, uniqName);
                            break;
                        }
                    case 1:
                        {
                            HttpContext.Current.Cache.Insert(localCityLevelUniqNameCacheKey, uniqName);
                            break;
                        }
                    default:
                        {
                            localBudgetTypes.Add(row[0].ToString(), type);
                            localBudgetUniqueNames.Add(row[0].ToString(), uniqName);
                            localBudgetCodes.Add(row[0].ToString(), code);
                            break;
                        }
                }
            }
            HttpContext.Current.Cache.Insert(localBudgetTypesCacheKey, localBudgetTypes);
            HttpContext.Current.Cache.Insert(localBudgetUniqueNamesCacheKey, localBudgetUniqueNames);
            HttpContext.Current.Cache.Insert(localBudgetCodesCacheKey, localBudgetCodes);
        }

        /// <summary>
        /// Проверяет повторямость подстроки в строке
        /// </summary>
        /// <param name="value">исходная строка</param>
        /// <param name="multiIndex">предполагамое число повторений в строке</param>
        /// <returns>повторяющаяся часть строки</returns>
        public static string CheckMultiplyValue(string value, int multiIndex)
        {
            int length = value.Length;
            if (length == 0 || length % multiIndex != 0)
            {
                return value;
            }

            int partIndex = length / multiIndex;
            string etalon = value.Substring(0, partIndex);
            for (int i = 1; i < multiIndex; i++)
            {
                string s = value.Substring(i * partIndex, partIndex);
                if (!String.Equals(s, etalon))
                {
                    return value;
                }
            }

            return etalon;
        }

        #endregion

        #region Локальные бюджеты с поселениями

        private static Dictionary<string, string> localSettlementTypes;
        private static Dictionary<string, string> localSettlementUniqueNames;
        public static DataTable localSettlementDT;

        /// <summary>
        /// Возвращает словарик типов местных бюджетов
        /// </summary>
        public static Dictionary<string, string> LocalSettlementTypes
        {
            get
            {
                // если словарь пустой
                if (localSettlementTypes == null || localSettlementTypes.Count == 0)
                {
                    // заполняем его
                    FillSettlementTypes();
                }
                return localSettlementTypes;
            }
        }

        /// <summary>
        /// Возвращает словарик уникальных имен местных бюджетов
        /// </summary>
        public static Dictionary<string, string> LocalSettlementUniqueNames
        {
            get
            {
                // если словарь пустой
                if (localSettlementUniqueNames == null || localSettlementUniqueNames.Count == 0)
                {
                    // заполняем его
                    FillSettlementTypes();
                }
                return localSettlementUniqueNames;
            }
        }

        private static void FillSettlementTypes()
        {   
            localSettlementTypes = new Dictionary<string, string>();
            localSettlementUniqueNames = new Dictionary<string, string>();

            localSettlementDT = new DataTable();
            bool level6 = RegionSettingsHelper.Instance.SettlementLevel.Contains("уровень 6");
            CustomParam.CustomParamFactory("localSettlementLevelName").Value = RegionSettingsHelper.Instance.SettlementLevel;
            CustomParam.CustomParamFactory("localRegionLevelName").Value = RegionSettingsHelper.Instance.RegionsLevel;
            CustomParam.CustomParamFactory("consolidateLevelName").Value = level6 ? RegionSettingsHelper.Instance.RegionsConsolidateLevel : string.Format("{0}.[Все районы]", RegionSettingsHelper.Instance.RegionDimension);
            CustomParam.CustomParamFactory("regionDimesion").Value = RegionSettingsHelper.Instance.RegionDimension;

            string query = DataProvider.GetQueryText(RegionSettingsHelper.Instance.SettlementListQueryName, HttpContext.Current.Request.PhysicalApplicationPath);

            //query = query.Replace("<@localSettlementLevelName@>", Singleton<MdxQueryRenameService>.Instance.Rename(RegionSettingsHelper.Instance.SettlementLevel));
           // query = query.Replace("<@consolidateLevelName@>", Singleton<MdxQueryRenameService>.Instance.Rename(level6 ? RegionSettingsHelper.Instance.RegionsConsolidateLevel : "[Районы].[Сопоставимый].[Все районы]"));
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", localSettlementDT);
            // значения дублируются почему-то именно 4 раза
            int multiplyValueCount = 4;

            for (int i = 0; i < localSettlementDT.Rows.Count; i++)
            {
                DataRow row = localSettlementDT.Rows[i];
                string name = (row[0] == DBNull.Value) ? string.Empty : row[0].ToString();
                string type = (row[1] == DBNull.Value) ? string.Empty : row[1].ToString();
                string uniqName = (row[2] == DBNull.Value) ? string.Empty : row[2].ToString();

                type = CheckMultiplyValue(type, multiplyValueCount);
                uniqName = CheckMultiplyValue(uniqName, multiplyValueCount);

                string key = GetDictionaryUniqueKey(localSettlementTypes, row[0].ToString());
                localSettlementTypes.Add(key, type);

                key = GetDictionaryUniqueKey(localSettlementUniqueNames, row[0].ToString());
                localSettlementUniqueNames.Add(key, uniqName);
            }
        }

        private static string GetDictionaryUniqueKey(Dictionary<string,string> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        #endregion
    }
}
