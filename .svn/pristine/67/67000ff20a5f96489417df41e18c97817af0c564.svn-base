using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
    public static  class XmlWorker
    {
        private readonly static XElement document;
        private static string code;
        private static XElement factorElement;


        public static string Code
        {
            get { return code; }
            set { code = value;
                SetFactor();
            }
        }

        static XmlWorker()
        {
            var filePath = HttpContext.Current.Server.MapPath("~/reports/MinSport/");
            var dirInfo = new DirectoryInfo(filePath);

            foreach (FileInfo f in dirInfo.GetFiles("FactorsList.xml"))
            {
                document = XElement.Load(f.FullName);
                if (document != null)
                {
                    return;
                }
            }
            throw new Exception("Не удалось найти файл настроек");
        }

        private static void SetFactor()
        {
            var listFactors = from t in document.Elements("FactorList").Elements("Factor")
                              where (string)t.Attribute("code") == code
                              select t;
            factorElement = listFactors.ElementAt(0);
        }

        private static string GetFactorName()
        {
            return (string)factorElement.Element("Name");
        }

        private static string GetFactorCubeName()
        {
            return (string)factorElement.Element("CubeName");
        }

        private static string GetFactorServiceCubeName()
        {
            return (string)factorElement.Element("ServiceCubeName");
        }

        private static bool GetFactorTerritoryFeatures(string nameAttr)
        {
            return ((factorElement.Element("TerritoryFeatures").Attribute(nameAttr).Value == "1"));
        }

        private static string GetFactorFrequencyWorks()
        {
            return (string)factorElement.Element("FrequencyWorks");
        }

        private static string GetFactorTimeProvision()
        {
            return (string)factorElement.Element("TimeProvision");
        }

        private static string GetFactorUnit()
        {
            return (string) factorElement.Element("Unit");
        }

        private static string GetFactorDepthTimeSet()
        {
            return (string) factorElement.Element("DepthTimeSet");
        }

        /// <summary>
        /// Получаю список кодов справочников, используемых в классифкаторе
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFactorUseHandBooks()
        {
            var useHandBooks = factorElement.Elements("UseHandBooks").Elements("HandBook");
            return useHandBooks.Select(elementHandBook => (string) elementHandBook.Attribute("code")).ToList();
        }

        /// <summary>
        /// Получаю раскладку справочника (0 = строки, 1 = столбцы)
        /// </summary>
        public static int GetHandBookLayout(string codehandBook)
        {
            var useHandBookLayout = from t in factorElement.Elements("UseHandBooks").Elements("HandBook")
                                    where (string)t.Attribute("code") == codehandBook
                               select (int)t.Attribute("layout");
            return useHandBookLayout.FirstOrDefault();
        }


        /// <summary>
        /// Получаю перечень "пересекающихся" справочников для показателя
        /// </summary>
        /// <returns>список объектов CrossHandBooks</returns>
        public static List<CrossHandBooks> GetFactorCrossHandBooks()
        {
            var crossHandBooks = factorElement.Elements("CrossHandBooks").Elements("Cross");
            return crossHandBooks.Select(elementCross => new CrossHandBooks((string) elementCross.Attribute("handBookOnRows"), (string) elementCross.Attribute("handBookOnColumns"))).ToList();
        }

        /// <summary>
        /// Получить данные о показателе
        /// </summary>
        public static void GetFactor(Factor factor)
        {
            factor.Code = code;
            factor.Name = GetFactorName();
            factor.CubeName = GetFactorCubeName();
            factor.ServiceCubeName = GetFactorServiceCubeName();
            factor.ClsFeatureRF = GetFactorTerritoryFeatures("RF");
            factor.ClsFeatureFO = GetFactorTerritoryFeatures("FO");
            factor.ClsFeatureSubject = GetFactorTerritoryFeatures("Subject");
            factor.FrequencyWorks = GetFactorFrequencyWorks();
            factor.Unit = GetFactorUnit();
            factor.TimeProvision = GetFactorTimeProvision();
            factor.DepthTimeSet = GetFactorDepthTimeSet();
        }

        /// <summary>
        /// Получение списка элементов для заданного справочника
        /// </summary>
        /// <param name="handBookCode">код справочника</param>
        /// <returns>список элементов справочника</returns>
        public static List<string> GetHandBookElements(string handBookCode)
        {
            var listHandBooks = from t in document.Elements("HandBookList").Elements("HandBook")
                                where
                                    (string) t.Attribute("code") == handBookCode
                                select t;

            var listFilteredHandBooks = from t in listHandBooks.Elements("UseInFactors").Elements("Factor")
                                        where
                                            (string) t.Attribute("code") ==
                                            code
                                        select t;
            var listCodes =
                from t in listFilteredHandBooks.Ancestors("HandBook").Elements("Elements").Elements("Element")
                where (string)t.Attribute("total") == "0"
                select (string)t.Attribute("code");
            return listCodes.ToList();
        }

        /// <summary>
        /// Получение списка элементов "всего" для заданного показателя
        /// </summary>
        /// <param name="layout">раскладка</param>
        /// <returns>список элементов "всего"</returns>
        public static List<string> GetFactorTotalElements(int layout)
        {
            var listTotalElements = from t in factorElement.Elements("TotalCodes").Elements("Element")
                                    where (int) t.Attribute("layout") == layout
                                    select (string) t.Attribute("code");
            return listTotalElements.ToList();
        }

        /// <summary>
        /// Проверка на наличие кода "всего" по строкам
        /// </summary>
        /// <returns>есть или нет код "всего" по строкам</returns>
        public static bool CheckTotalElementsOnRows()
        {
            var listTotalElementsOnRows = from t in factorElement.Elements("TotalCodes").Elements("Element")
                                          where (int)t.Attribute("layout") == 0
                                          select t;
            return (listTotalElementsOnRows.Count() > 0);
        }
        
        /// <summary>
        ////проверка на наличие кода "всего" по столбцам
        /// </summary>
        /// <returns>есть или нет код "всего" по столбцам</returns>
        public static bool CheckTotalElementsOnColumns()
        {
            var listTotalElementsOnColumns = from t in factorElement.Elements("TotalCodes").Elements("Element")
                                          where (int)t.Attribute("layout") == 1
                                          select t;
            return (listTotalElementsOnColumns.Count() > 0);
        }

        /// <summary>
        /// Получение списка(словаря) показателей
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string,string> GetFactorsDictionary()
        {
            var dictionaryFactorsQuery = from t in document.Elements("FactorList").Elements("Factor") select new {code = (string)t.Attribute("code"), name = (string)t.Attribute("code") + " - " +  (string)t.Element("Name")};
            var dictionaryFactors = dictionaryFactorsQuery.ToDictionary(element => element.name, element => element.code);
            return dictionaryFactors;
        }

        /// <summary>
        /// Получаю ЕМИСС код элемента справочника
        /// </summary>
        /// <param name="handBookCode">код справочника</param>
        /// <param name="elementCode">код элемента справочника</param>
        /// <returns>список ЕМИСС кодов</returns>
        public static string GetHandBookECode(string handBookCode, string elementCode)
        {
            var listHandBooks = from t in document.Elements("HandBookList").Elements("HandBook")
                                where
                                    (string)t.Attribute("code") == handBookCode
                                select t;

            var listFilteredHandBooks = from t in listHandBooks.Elements("UseInFactors").Elements("Factor")
                                        where
                                            (string)t.Attribute("code") ==
                                            code
                                        select t;
            var listCodes =
                from t in listFilteredHandBooks.Ancestors("HandBook").Elements("Elements").Elements("Element")
                where (string)t.Attribute("code") == elementCode
                select (string)t.Attribute("eCode");
            return listCodes.ToList().ElementAt(0);
        }

        /// <summary>
        /// Получаю родительский код ЕМИСС
        /// </summary>
        public static string GetHandBookParentECode(string handBookCode, string elementCode)
        {
            var listHandBooks = from t in document.Elements("HandBookList").Elements("HandBook")
                                where
                                    (string)t.Attribute("code") == handBookCode
                                select t;

            var listFilteredHandBooks = from t in listHandBooks.Elements("UseInFactors").Elements("Factor")
                                        where
                                            (string)t.Attribute("code") ==
                                            code
                                        select t;
            var listCodes =
                from t in listFilteredHandBooks.Ancestors("HandBook").Elements("Elements").Elements("Element")
                where (string)t.Attribute("code") == elementCode
                select (string)t.Attribute("parent");
            return listCodes.ToList().ElementAt(0);
        }

        /// <summary>
        /// Получаю ЕМИСС код для элемента справочника "всего"
        /// </summary>
        /// <param name="handBookCode">код справочника</param>
        /// <returns>ЕМИСС код элемента "всего"</returns>
        public static string GetHandBookTotalCode(string handBookCode)
        {
            var listHandBooks = from t in document.Elements("HandBookList").Elements("HandBook")
                                where
                                    (string)t.Attribute("code") == handBookCode
                                select t;

            var listFilteredHandBooks = from t in listHandBooks.Elements("UseInFactors").Elements("Factor")
                                        where
                                            (string)t.Attribute("code") ==
                                            code
                                        select t;
            var listCodes =
                from t in listFilteredHandBooks.Ancestors("HandBook").Elements("Elements").Elements("Element")
                where (string)t.Attribute("total") == "1"
                select (string)t.Attribute("eCode");
            return (listCodes.Count() > 0) ? listCodes.ToList().First() : "Нет кода всего";
        }

        /// <summary>
        /// Получаю код родителя для справочника
        /// </summary>
        public static string GetHandBookParent(string handBookCode)
        {
            var listHandBooks = from t in document.Elements("HandBookList").Elements("HandBook")
                                where
                                    (string)t.Attribute("code") == handBookCode
                                select t;

            var listFilteredHandBooks = from t in listHandBooks.Elements("UseInFactors").Elements("Factor")
                                        where
                                            (string)t.Attribute("code") ==
                                            code
                                        select t;
            var listCodes =
                from t in listFilteredHandBooks.Ancestors("HandBook")
                select (string)t.Attribute("parent");
            return (listCodes.Count() > 0) ? listCodes.ToList().First() : null;
        }

        /// <summary>
        /// Чтение имени ответственного лица
        /// </summary>
        /// <returns>Имя ответственного лица</returns>
        public static string GetResponsibleName()
        {
            var xElement = document.Element("ServiceInfo");
            if (xElement == null)
            {
                return "Имя ответственного лица не найдено";
            }
            var name = (string) xElement.Element("ResponsibleName");
                return name;
        }

        /// <summary>
        /// Чтение телефона ответственного лица
        /// </summary>
        /// <returns>Телефон ответственного дица</returns>
        public static string GetResponsibleMobile()
        {
            var xElement = document.Element("ServiceInfo");
            if (xElement == null)
            {
                return "Телефон ответственного лица не найден";
            }
            var name = (string)xElement.Element("ResponsibleMobile");
                return name;
        }

        /// <summary>
        /// Чтение кода ведомства
        /// </summary>
        /// <returns>Код ведомства</returns>
        public static string GetDepartmentCode()
        {
            var xElement = document.Element("ServiceInfo");
            if (xElement == null)
            {
                return "Код ведомства не найден";
            }
            var name = (string)xElement.Element("DepartmentCode");
                return name;
        }

        public static bool IsDebugMode()
        {
            var xElement = document.Element("SettingsUI");

            if (xElement != null)
            {
                var mode = (bool)xElement.Element("DebugMode");
                return mode;
            }
            return false;
        }
    }
}
