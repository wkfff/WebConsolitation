using System;
using System.Data;
using System.Xml.Linq;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
    /// <summary>
    /// Класс для формирования файлов-выгрузок в ЕМИСС
    /// </summary>
    public static class XmlWriter
    {
        private static XNamespace generic;
        private static Factor factor;
        private static DataTable table;


        /// <summary>
        /// Формирование элемента "Данные показателя"
        /// </summary>
        /// <param name="rootElement"> корневой узел </param>
        private static void CreateDataSetElement(XElement rootElement)
        {
            var dataSetElement = new XElement("DataSet");
            rootElement.Add(dataSetElement);
           
            foreach (DataRow row in table.Rows)
            {
                var genericSeriesKeyElement = new XElement(generic + "SeriesKey");
                genericSeriesKeyElement.Add(new XElement(generic + "Value",
                                                         new XAttribute("concept", "OKSM"),
                                                         new XAttribute("value", row["Territory"])));
                for (var i = 1; i <= (table.Columns.Count - 3)/2; i++)
                {
                    if (!ReferenceEquals(row[String.Format("Справочник_{0}", i)], ""))
                    {
                        genericSeriesKeyElement.Add(new XElement(generic + "Value",
                                     new XAttribute("concept", row[String.Format("Справочник_{0}", i)]),
                                     new XAttribute("value", row[String.Format("ЭлементСправочника_{0}", i)])));  
                    }
                }
                var genericSeriesElement = new XElement(generic + "Series", 
                                                        genericSeriesKeyElement,
                                                        new XElement(generic + "Attributes",
                                                                     new XElement(generic + "Value",
                                                                                  new XAttribute(
                                                                                      "concept", "EI"),
                                                                                  new XAttribute("value",
                                                                                                 factor.Unit)),
                                                                     new XElement(generic + "Value",
                                                                                  new XAttribute(
                                                                                      "concept", "PERIOD"),
                                                                                  new XAttribute("value",
                                                                                                 factor.FrequencyWorks))),
                                                        new XElement(generic + "Obs",
                                                                     new XElement(generic + "Time", factor.WorkYear),
                                                                     new XElement(generic + "ObsValue",
                                                                                  new XAttribute("value",
                                                                                                 row["Значение"]))));
                dataSetElement.Add(genericSeriesElement);
            }
        }

        /// <summary>
        /// Создание корневого узла с описанием пространств имен
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>корневой узел</returns>
        private static XElement CreateRootElement(XDocument doc)
        {
            generic = "http://www.SDMX.org/resources/SDMXML/schemas/v1_0/generic";
            var genericData = new XElement("GenericData",
                                           new XAttribute(XNamespace.Xmlns + "generic",
                                                          "http://www.SDMX.org/resources/SDMXML/schemas/v1_0/generic"));
            doc.Add(genericData);
            return genericData;
        }

        /// <summary>
        /// Создание XML файла-выгрузки показателя
        /// </summary>
        /// <param name="tableData">таблица данных показателя</param>
        /// <param name="factorData">метаданные показателя</param>
        public static void CreateXmlDocument(DataTable tableData, Factor factorData)
        {
            table = tableData;
            factor = factorData;
            var doc = new XDocument();
            var rootNode = CreateRootElement(doc);
            CreateDataSetElement(rootNode);
            doc.Save(@"C:\Result.xml");
        }
    }
}
