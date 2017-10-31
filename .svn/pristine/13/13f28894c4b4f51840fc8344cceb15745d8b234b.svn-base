using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core
{
    public static class RegionNames
    {
        public static string Novosib
        {
            get
            {
                return "Novosib";
            }
        }

        public static string Yaroslavl
        {
            get
            {
                return "Yaroslavl";
            }
        }

        public static string Alaniya
        {
            get
            {
                return "Alaniya";
            }
        }

        public static string Chechnya
        {
            get
            {
                return "Chechnya";
            }
        }

        public static string Penza
        {
            get
            {
                return "Penza";
            }
        }

        public static string Kursk
        {
            get
            {
                return "Kursk";
            }
        }

        public static string Omsk
        {
            get
            {
                return "Omsk";
            }
        }

        public static string Krasnodar
        {
            get
            {
                return "Krasnodar";
            }
        }

        public static string Kostroma
        {
            get
            {
                return "Kostroma";
            }
        }

        public static string Samara
        {
            get
            {
                return "Samara";
            }
        }

        public static string Moris
        {
            get
            {
                return "Moris";
            }
        }

        public static string Vologda
        {
            get
            {
                return "Vologda";
            }
        }

        public static string Astrakhan
        {
            get
            {
                return "Astrakhan";
            }
        }

        public static string HMAO
        {
            get
            {
                return "HMAO";
            }
        }

        public static string Saratov
        {
            get
            {
                return "Saratov";
            }
        }

        public static string Sakhalin
        {
            get
            {
                return "Sakhalin";
            }
        }

        public static string Altai
        {
            get
            {
                return "Altai";
            }
        }

        public static string Yamal
        {
            get
            {
                return "Yamal";
            }
        }

        public static bool IsRegionName(string name)
        {
            return name.ToLower() == Novosib.ToLower() || name.ToLower() == Yaroslavl.ToLower() ||
                   name.ToLower() == Alaniya.ToLower() || name.ToLower() == Chechnya.ToLower() ||
                   name.ToLower() == Penza.ToLower() || name.ToLower() == Kursk.ToLower() ||
                   name.ToLower() == Omsk.ToLower() || name.ToLower() == Krasnodar.ToLower() ||
                   name.ToLower() == Kostroma.ToLower() || name.ToLower() == Samara.ToLower() ||
                   name.ToLower() == Moris.ToLower() || name.ToLower() == Vologda.ToLower() ||
                   name.ToLower() == Astrakhan.ToLower() || name.ToLower() == HMAO.ToLower() ||
                   name.ToLower() == Saratov.ToLower() || name.ToLower() == Sakhalin.ToLower() ||
                   name.ToLower() == Altai.ToLower() || name.ToLower() == Yamal.ToLower();
        }
    }

    public class RegionSettingsHelper
    {
        #region Свойства

        /// <summary>
        /// Имя региона в именительном падеже
        /// </summary>
        public string Name
        {
            get
            {
                return GetCacheValue("Name");
            }
        }

        /// <summary>
        /// Сокращенное имя региона
        /// </summary>
        public string ShortName
        {
            get
            {
                return GetCacheValue("ShortName");
            }
        }

        /// <summary>
        /// Имя региона в родительном падеже
        /// </summary>
        public string RegionNameGenitive
        {
            get
            {
                return GetCacheValue("RegionNameGenitive");
            }
        }

        /// <summary>
        /// Путь к файлу карты 
        /// </summary>
        public string FileMapName
        {
            get
            {
                return GetCacheValue("FileMapName");
            }
        }

        /// <summary>
        /// Строка MDX запроса региона
        /// </summary>
        public string RegionBaseDimension
        {
            get
            {
                return GetCacheValue("RegionBaseDimension");
            }
        }

        /// <summary>
        /// Доходы всего
        /// </summary>
        public string IncomeTotal
        {
            get
            {
                return GetCacheValue("IncomeTotal");
            }
        }

        /// <summary>
        /// Расходы ФКР всего
        /// </summary>
        public string OutcomeFKRTotal
        {
            get
            {
                return GetCacheValue("OutcomeFKRTotal");
            }
        }

        /// <summary>
        /// Консолидированный уровень бюджета
        /// </summary>
        public string RegionsConsolidateLevel
        {
            get
            {
                return GetCacheValue("RegionsConsolidateLevel");
            }
        }

        /// <summary>
        /// Уровень собственного бюджета субъекта
        /// </summary>
        public string RegionsOwnBudgetLevel
        {
            get
            {
                return GetCacheValue("RegionsOwnBudgetLevel");
            }
        }

        /// <summary>
        /// Уровень местных бюджетов субъекта
        /// </summary>
        public string RegionsLocalBudgetLevel
        {
            get
            {
                return GetCacheValue("RegionsLocalBudgetLevel");
            }
        }

        /// <summary>
        /// Уровень бюджета субъекта
        /// </summary>
        public string RegionsSubjectLevel
        {
            get
            {
                return GetCacheValue("RegionsSubjectLevel");
            }
        }

        /// <summary>
        /// Уровень районов
        /// </summary>
        public string RegionsLevel 
        {
            get
            {
                return GetCacheValue("RegionsLevel");
            }
        }

        /// <summary>
        /// Запрос для получения списка поселений
        /// </summary>
        public string SettlementListQueryName
        {
            get
            {
                return GetCacheValue("SettlementListQueryName");
            }
        }

        /// <summary>
        /// Строка подключения
        /// </summary>
        private string ConnectionString
        {
            get
            {
                return GetCacheValue("ConnectionString");
            }
        }
        
        private string regionID = String.Empty;

        /// <summary>
        /// Идентификатор региона. По умолчанию установленный в конфиге.
        /// </summary>
        private string RegionID
        {
            get
            {
                if (String.IsNullOrEmpty(regionID))
                {
                    return RegionSettings.Instance.Id;
                }
                else
                {
                    return regionID;
                }
            }
        }
        
        /// <summary>
        /// Уровень поселений
        /// </summary>
        public string SettlementLevel
        {
            get
            {
                return GetCacheValue("SettlementLevel");
            }
        }

        /// <summary>
        /// Измерение периода
        /// </summary>
        public string PeriodDimension
        {
            get
            {
                return GetCacheValue("PeriodDimension");
            }
        }

        /// <summary>
        /// Тип кассового плана
        /// </summary>
        public string CashPlanType
        {
            get
            {
                return GetCacheValue("CashPlanType");
            }
        }

        /// <summary>
        /// Наименование остатка кассового плана
        /// </summary>
        public string CashPlanBalance
        {
            get
            {
                return GetCacheValue("CashPlanBalance");
            }
        }

        /// <summary>
        /// Численность постоянного населения
        /// </summary>
        public string PopulationMeasure
        {
            get
            {
                return GetCacheValue("PopulationMeasure");
            }
        }

        /// <summary>
        /// Прогнозирование численности
        /// </summary>
        public bool PopulationMeasurePlanning
        {
            get
            {
                return Convert.ToBoolean(GetCacheValue("PopulationMeasurePlanning"));
            }
        }

        /// <summary>
        /// Измерение Районов
        /// </summary>
        public string RegionDimension
        {
            get
            {
                return GetCacheValue("RegionDimension");
            }
        }

        /// <summary>
        /// Измерение Расходов ФКР
        /// </summary>
        public string FKRDimension
        {
            get
            {
                return GetCacheValue("FKRDimension");
            }
        }

        /// <summary>
        /// Уровень All измерения расходов ФКР
        /// </summary>
        public string FKRAllLevel
        {
            get
            {
                return GetCacheValue("FKRAllLevel");
            }
        }

        /// <summary>
        /// Уровень Раздел измерения расходов ФКР
        /// </summary>
        public string FKRSectionLevel
        {
            get
            {
                return GetCacheValue("FKRSectionLevel");
            }
        }

        /// <summary>
        /// Уровень Подраздел измерения расходов ФКР
        /// </summary>
        public string FKRSubSectionLevel
        {
            get
            {
                return GetCacheValue("FKRSubSectionLevel");
            }
        }

        /// <summary>
        /// Измерение Расходов ЭКР (КОСГУ)
        /// </summary>
        public string EKRDimension
        {
            get
            {
                return GetCacheValue("EKRDimension");
            }
        }

        /// <summary>
        /// Наименование корневого элемента доходов КД.Сопоставимый
        /// </summary>
        public string IncomesKDRootName
        {
            get
            {
                return GetCacheValue("IncomesKDRootName");
            }
        }

        /// <summary>
        /// Наименование элемента доходов КД.Сопоставимый "Налоги на социальные нужды"
        /// </summary>
        public string IncomesKDSocialNeedsTax
        {
            get
            {
                return GetCacheValue("IncomesKDSocialNeedsTax");
            }
        }

        /// <summary>
        /// Наименование элемента доходов КД.Сопоставимый "ВОЗВРАТ ОСТАТКОВ СУБСИДИЙ, СУБВЕНЦИЙ И ИНЫХ МЕЖБЮДЖЕТНЫХ ТРАНСФЕРТОВ..."
        /// </summary>
        public string IncomesKDReturnOfRemains
        {
            get
            {
                return GetCacheValue("IncomesKDReturnOfRemains");
            }
        }
        
        /// <summary>
        /// Наименование элемента доходов с соотвествующим кодом
        /// </summary>
        public string IncomesKD11800000000000000
        {
            get
            {
                return GetCacheValue("IncomesKD11800000000000000");
            }
        }

        /// <summary>
        /// Наименование элемента доходов с соотвествующим кодом
        /// </summary>
        public string IncomesKD11402000000000000
        {
            get
            {
                return GetCacheValue("IncomesKD11402000000000000");
            }
        }

        /// <summary>
        /// Наименование элемента доходов с соотвествующим кодом
        /// </summary>
        public string IncomesKD11403000000000410
        {
            get
            {
                return GetCacheValue("IncomesKD11403000000000410");
            }
        }

        /// <summary>
        /// Наименование элемента доходов с соотвествующим кодом
        /// </summary>
        public string IncomesKD11403000000000440
        {
            get
            {
                return GetCacheValue("IncomesKD11403000000000440");
            }
        }

        /// <summary>
        /// Наименование элемента доходов с соотвествующим кодом
        /// </summary>
        public string IncomesKD11406000000000430
        {
            get
            {
                return GetCacheValue("IncomesKD11406000000000430");
            }
        }

        /// <summary>
        /// ПРОЧИЕ НЕНАЛОГОВЫЕ ДОХОДЫ
        /// </summary>
        public string IncomesKD11700000000000000
        {
            get
            {
                return GetCacheValue("IncomesKD11700000000000000");
            }
        }

        /// <summary>
        /// ДОХОДЫ ОТ ПРЕДПРИНИМАТЕЛЬСКОЙ И ИНОЙ ПРИНОСЯЩЕЙ ДОХОД ДЕЯТЕЛЬНОСТИ
        /// </summary>
        public string IncomesKD30000000000000000
        {
            get
            {
                return GetCacheValue("IncomesKD30000000000000000");
            }
        }

        /// <summary>
        /// ГОСУДАРСТВЕННАЯ ПОШЛИНА
        /// </summary>
        public string IncomesKD10800000000000000
        {
            get
            {
                return GetCacheValue("IncomesKD10800000000000000");
            }
        }

        /// <summary>
        /// Уровень "Группа" в КД.Сопоставимый
        /// </summary>
        public string IncomesKDGroupLevel
        {
            get
            {
                return GetCacheValue("IncomesKDGroupLevel");
            }
        }

        /// <summary>
        /// Уровень "Все коды доходов" в КД.Сопоставимый
        /// </summary>
        public string IncomesKDAllLevel
        {
            get
            {
                return GetCacheValue("IncomesKDAllLevel");
            }
        }
        
        /// <summary>
        /// Уровень "Статья" в КД.Сопоставимый
        /// </summary>
        public string IncomesKDSectionLevel
        {
            get
            {
                return GetCacheValue("IncomesKDSectionLevel");
            }
        }

        /// <summary>
        /// Уровень "Подстатья" в КД.Сопоставимый
        /// </summary>
        public string IncomesKDSubSectionLevel
        {
            get
            {
                return GetCacheValue("IncomesKDSubSectionLevel");
            }
        }

        /// <summary>
        /// Измерение КД.Сопоставимый
        /// </summary>
        public string IncomesKDDimension
        {
            get
            {
                return GetCacheValue("IncomesKDDimension");
            }
        }

        /// <summary>
        /// Элемент "Государственное управление и обеспечение военной безопасности.." в ОКВЭД.Сопоставимый
        /// </summary>
        public string FNSOKVEDGovernment
        {
            get
            {
                return GetCacheValue("FNSOKVEDGovernment");
            }
        }

        /// <summary>
        /// Элемент "Деятельность домашних хозяйств" в ОКВЭД.Сопоставимый
        /// </summary>
        public string FNSOKVEDHousehold
        {
            get
            {
                return GetCacheValue("FNSOKVEDHousehold");
            }
        }

        /// <summary>
        /// Использовать куб ФНС_28н_с расщеплением
        /// </summary>
        public string FNS28nSplitting
        {
            get
            {
                return GetCacheValue("FNS28nSplitting");
            }
        }

        /// <summary>
        /// Элемент "Культура, кинематография и средства массовой информации" в РзПр.Сопоставимый
        /// </summary>
        public string FOFKRCulture
        {
            get
            {
                return GetCacheValue("FOFKRCulture");
            }
        }

        /// <summary>
        /// Элемент "ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ" в РзПр.Сопоставимый
        /// </summary>
        public string FOFKRHelthCare
        {
            get
            {
                return GetCacheValue("FOFKRHelthCare");
            }
        }

        /// <summary>
        /// Наименование элемента "Собственный бюджета субъекта" для отображения в отчете
        /// </summary>
        public string OwnSubjectBudgetName
        {
            get
            {
                return GetCacheValue("OwnSubjectBudgetName");
            }
        }

        /// <summary>
        /// Уровень бюджета для ГО в отчетах ФНС
        /// </summary>
        public string FNSDistrictBudgetLevel
        {
            get
            {
                return GetCacheValue("FNSDistrictBudgetLevel");
            }
        }

        /// <summary>
        /// Удалять из списка КД внутренние обороты
        /// </summary>
        public string KDInternalCircualtionExtruding
        {
            get
            {
                return GetCacheValue("KDInternalCircualtionExtruding");
            }
        }

        /// <summary>
        /// Удалять из списка РзПр внутренние обороты
        /// </summary>
        public string RzPrInternalCircualtionExtruding
        {
            get
            {
                return GetCacheValue("RzPrInternalCircualtionExtruding");
            }
        }

        /// <summary>
        /// Использовать авторизованный список локальных бюджетов
        /// </summary>
        public string AuthorizedLocalBudgetList
        {
            get
            {
                return GetCacheValue("AuthorizedLocalBudgetList");
            }
        }

        /// <summary>
        /// Куб для численности населения
        /// </summary>
        public string PopulationCube
        {
            get
            {
                return GetCacheValue("PopulationCube");
            }
        }

        /// <summary>
        /// Фильтр для выборки численности населения
        /// </summary>
        public string PopulationFilter
        {
            get
            {
                return GetCacheValue("PopulationFilter");
            }
        }

        /// <summary>
        /// Измерение периода для выборки численности населения
        /// </summary>
        public string PopulationPeriodDimension
        {
            get
            {
                return GetCacheValue("PopulationPeriodDimension");
            }
        }

        /// <summary>
        /// Делитель значений численности населения
        /// </summary>
        public string PopulationValueDivider
        {
            get
            {
                return GetCacheValue("PopulationValueDivider");
            }
        }

        #endregion

        internal string GetCacheKey(string name)
        {
            string fileFullName = GetReportConfigFullName();
            if (File.Exists(fileFullName))
            {
                return string.Format("rs_{0}_{1}_{2}", name, RegionID, HttpContext.Current.Request.PhysicalPath);
            }
            else
            {
                return string.Format("rs_{0}_{1}", name, RegionID);
            }
        }

        /// <summary>
        /// Устанавливает рабочий регион. Меняет PrimaryMASDataProvider.
        /// </summary>
        /// <param name="region"></param>
        public void SetWorkingRegion(string region)
        {
            regionID = region;
            DataProvidersFactory.SetCustomPrimaryMASDataProvider(ConnectionString);
        }

        /// <summary>
        /// Возвращет значение свойства с произвольным именем из файла настроек.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        /// <returns>Значение свойства.</returns>
        public string GetPropertyValue(string propertyName)
        {
            return GetCacheValue(propertyName);
        }

        /// <summary>
        /// Получить значение свойства из кэша
        /// </summary>
        /// <param name="name">имя свойства</param>
        /// <returns>значение</returns>
        private string GetCacheValue(string name)
        {
            string cacheKey = GetCacheKey(name);
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                // если в кэше нет, то извлекаем значение заново
                return GetRegionSetting(name);
            }
            // иначе берем текущее значение из кэша
            return HttpContext.Current.Cache[cacheKey].ToString();
        }

        /// <summary>
        /// Получить значение свойства региональных настроек
        /// </summary>
        /// <param name="propertyName">свойство</param>
        /// <returns>значение свойства</returns>
        public string GetRegionSetting(string propertyName)
        {
            string fileFullName = GetReportConfigFullName();
            if (File.Exists(fileFullName))
            {
                string property = GetPropertyFromXml(fileFullName, propertyName);
                if (!String.IsNullOrEmpty(property))
                {
                    return property;
                }
            }
            fileFullName = HttpContext.Current.Server.MapPath("~/RegionSettingsStore.xml");
            return GetPropertyFromXml(fileFullName, propertyName);
        }

        /// <summary>
        /// Формирует имя файла конфига для текущего отчета.
        /// </summary>
        /// <returns></returns>
        public static string GetReportConfigFullName()
        {
            string filePath = HttpContext.Current.Request.PhysicalPath;
            string directoryName = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            fileName = fileName + ".settings.xml";
            return Path.Combine(directoryName, fileName);
        }

        private string GetPropertyFromXml(string fileName, string propertyName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            if (xmlDoc.ChildNodes.Count > 0)
            {
                XmlNode root = xmlDoc.ChildNodes[0];
                foreach (XmlNode node in root.ChildNodes)
                {
                    string id = XmlHelper.GetStringAttrValue(node, "id", string.Empty);
                    if (id.ToLower() == RegionID.ToLower())
                    {
                        foreach (XmlNode propNode in node.ChildNodes)
                        {
                            if (propNode.Name == propertyName && propNode.InnerText != string.Empty)
                            {
                                // помещаем значение свойства в кэш
                                CacheDependency dependency = new CacheDependency(fileName);
                                string cacheKey = GetCacheKey(propertyName);
                                HttpContext.Current.Cache.Insert(cacheKey, propNode.InnerText, dependency);
                                return propNode.InnerText;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        private RegionSettingsHelper()
        {
            
        }

        /// <summary>
        /// Одиночка в контексте отдельной сессии.
        /// </summary>
        public static RegionSettingsHelper Instance
        {
            get
            {
                if (HttpContext.Current.Session["RegionSettingsHelper"] == null)
                {
                    HttpContext.Current.Session["RegionSettingsHelper"] = new RegionSettingsHelper();
                }
                return (RegionSettingsHelper) HttpContext.Current.Session["RegionSettingsHelper"];
            }
        }
    }
}
