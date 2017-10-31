using System;
using System.IO;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;

namespace Krista.FM.Server.Providers.Planing
{
    internal partial class PlaningProvider
    {
        private object syncObj = new object();

        #region private methods

        private string cacheFolder
        {
            get
            {
                if (providerId == "0")
                    return scheme.BaseDirectory + "\\Cache\\";
                else
                    return scheme.BaseDirectory + "\\Cache" + providerId + "\\";
            }
        }

        /// <summary>
        /// сохранение метаданных в кэш
        /// </summary>
        /// <param name="metadataDocument"> документ с метаданными </param>
        private void SaveMetadata(XmlDocument metadataDocument)
        {
            if (metadataDocument == null)
                return;
            try
            {
                lock (syncObj)
                {
                    string fileName = cacheFolder + "Metadata.xml";
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    XmlHelper.Save(metadataDocument, fileName);
                }
            }
            catch (Exception e)
            {
                throw new ServerException("При сохранении метаданных в серверный кэш произошла ошибка.", e);
            }
        }

        /// <summary>
        /// удалить из кэша устаревшие (критерий - дата последнего процессинга) измерения 
        /// </summary>
        /// <param name="metadataDocument"> документ с метаданными </param>
        private void DeleteOldDimensions(XmlDocument metadataDocument)
        {
            // проходим по всем измерениям в кэше и сверяем LastProcessedDate с метаданными 
            string[] fileNames = Directory.GetFiles(cacheFolder, "*.xml", 0);
            bool error = false;
            foreach (string fileName in fileNames)
            {
                if (fileName == cacheFolder + "Metadata.xml")
                    continue;
                // выделяем краткое имя файла
                string fileShortName = fileName.Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                fileShortName = fileShortName.Remove(fileShortName.Length - 4);
                string dimensionName = string.Empty;
                string hierachyName = string.Empty;
                string dimensionDate = string.Empty;
                string metadataDimensionDate = string.Empty;
                bool IsFileOurs = true;
                try
                {
                    GetFileKeys(fileShortName, ref dimensionName, ref hierachyName, ref dimensionDate);
                    metadataDimensionDate = GetMetadataDimensionDate(metadataDocument, dimensionName, hierachyName);
                }
                catch
                {
                    // файл не наш или измерения ваще уже нет
                    IsFileOurs = false;
                }
                // если файл лажовый, мочим его
                if (!IsFileOurs)
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                        error = true;
                    }
                    continue;
                }
                // сравниваем даты
                // формат даты "dd.mm.yyyy hh:mm:ss"
                // если дата измерения меньше соответственной даты в метаданных, то мочим из кэша измерение
                if (CompareDates(dimensionDate, metadataDimensionDate) < 0)
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                        error = true;
                    }
                }
            }
            if (error)
            {
                throw new ServerException("При удалении устаревших измерений из серверного кэша произошла ошибка.");
            }
        }

        /// <summary>
        ///  загрузить измерение из кэша
        /// </summary>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierachyName"> имя иерархии </param>
        /// <returns> загруженное измерение </returns>
        private string LoadDimension(string dimensionName, string hierachyName)
        {
            string fileName = EncryptString(dimensionName) + "@" + EncryptString(hierachyName) + "@*.xml";
            lock (syncObj)
            {
                string[] fileList = Directory.GetFiles(cacheFolder, fileName, 0);
                if (fileList.Length == 0)
                    return string.Empty;
                return File.ReadAllText(fileList[0], Encoding.GetEncoding(1251));
            }
        }

        /// <summary>
        /// сохранить измерение в кэш
        /// </summary>
        /// <param name="dimensionDocument"> документ с данными измерения </param>
        private void SaveDimension(XmlDocument dimensionDocument)
        {
            if (dimensionDocument == null)
                return;
            try
            {
                string fileName = GetFileName(dimensionDocument);
                if (fileName == string.Empty)
                    return;
                fileName = cacheFolder + fileName + ".xml";

                lock (syncObj)
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    XmlHelper.Save(dimensionDocument, fileName);
                }
            }
            catch (Exception e)
            {
                throw new ServerException("При сохранении измерения в серверный кэш произошла ошибка.", e);
            }
        }

        /// <summary>
        /// Удалет измерение из кэша, если оно там есть
        /// </summary>
        /// <param name="dimName">Имя измерения</param>
        /// <param name="hierName">Имя иерархии</param>
        private void DeleteDimensionByName(string dimName, string hierName)
        {
            string fileName = EncryptString(dimName) + "@" + EncryptString(hierName) + "@*.xml";
            string[] fileList = Directory.GetFiles(cacheFolder, fileName, 0);
            if (fileList.Length == 0)
                return; //нету измерения, на выход
            try
            {
                File.Delete(fileList[0]);
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Не удалось удалить измерение {0} из кэша: {1}", fileList[0], e.Message);
            } //если не получилось удалить файл, то ничего делать не будем (пока).            
        }

        /// <summary>
        /// загрузить метаданные из кэша в DOM
        /// </summary>
        /// <returns> загруженные метаданные в виде DOM </returns>
        private XmlDocument LoadMetadataDOM()
        {
            //Берем метаданные из кэша
            string metadataStr = LoadMetadata();
            if (metadataStr == string.Empty)
                return null; //если метаданных нет, тогда ничего и не делаем

            XmlDocument metadataDOM = new XmlDocument();
            metadataDOM.LoadXml(metadataStr);
            if (metadataDOM.DocumentElement == null)
                return null;
            else
                return metadataDOM;
        }

        /// <summary>
        /// загрузить файл метаданных из кэша в строку. На время загрузки устанавливает блокировку
        /// </summary>
        /// <returns> загруженные метаданные в виде строки </returns>
        private string LoadMetadata()
        {
            string fileName = cacheFolder + "Metadata.xml";
            if (!File.Exists(fileName))
                return string.Empty;
            lock (syncObj)
            {
                return File.ReadAllText(fileName, Encoding.GetEncoding(1251));
            }
        }

        private string GetTempFileNameForCellsetData()
        {
            return String.Format(
                "{0}{1}_({2})_{3}.xml",
                cacheFolder,
                "CellSetData_",
                new Random(Environment.TickCount).Next(),
                DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss")
            );
        }



        /// <summary>
        /// очистка кэша
        /// </summary>
        public void ClearCache()
        {
            bool error = false;
            lock (syncObj)
            {
                string[] filesList = Directory.GetFiles(cacheFolder);
                foreach (string fileName in filesList)
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                        error = true;
                    }
                }
            }
            if (error)
            {
                throw new ServerException("При очищении серверного кэша произошла ошибка.");
            }
        }

        /// <summary>
        /// кодирует строку
        /// </summary>
        /// <param name="sourceString"> исходная строка </param>
        /// <returns> закодированная строка </returns>
        private static string EncryptString(string sourceString)
        {
            sourceString = sourceString.Replace("/", "&2f;");
            sourceString = sourceString.Replace("\\", "&5c;");
            sourceString = sourceString.Replace("|", "&7c;");
            sourceString = sourceString.Replace(":", "&3a;");
            sourceString = sourceString.Replace("*", "&2a;");
            sourceString = sourceString.Replace("?", "&3f;");
            sourceString = sourceString.Replace("\"", "&22;");
            sourceString = sourceString.Replace("<", "&3c;");
            sourceString = sourceString.Replace(">", "&3e;");
            return sourceString;
        }

        /// <summary>
        /// декодирует строку
        /// </summary>
        /// <param name="sourceString"> исходная строка </param>
        /// <returns> декодированная строка </returns>
        private static string DecryptString(string sourceString)
        {
            sourceString = sourceString.Replace("&2f;", "/");
            sourceString = sourceString.Replace("&5c;", "\\");
            sourceString = sourceString.Replace("&7c;", "|");
            sourceString = sourceString.Replace("&3a;", ":");
            sourceString = sourceString.Replace("&2a;", "*");
            sourceString = sourceString.Replace("&3f;", "?");
            sourceString = sourceString.Replace("&22;", "\"");
            sourceString = sourceString.Replace("&3c;", "<");
            sourceString = sourceString.Replace("&3e;", ">");
            return sourceString;
        }

        /// <summary>
        /// получить закодированное имя файла по ключам (измерение, иерархия, дата обработки), находящихся в документе
        /// </summary>
        /// <param name="domDocument"> исходный документ </param>
        /// <returns> закодированное имя файла </returns>
        private static string GetFileName(XmlDocument domDocument)
        {
            if (domDocument == null)
                return string.Empty;
            // получаем атрибуты из документа (имя измерения, имя иерархии, дата процессинга)
            XmlNode dimensionRoot = domDocument.SelectSingleNode("function_result/Dimension");
            string dimensionName = dimensionRoot.Attributes.GetNamedItem("name").Value;
            string date = dimensionRoot.Attributes.GetNamedItem("processing_date").Value;
            XmlNode hierachyRoot = domDocument.SelectSingleNode("function_result/Hierarchy");
            string hierachyName = hierachyRoot.Attributes.GetNamedItem("name").Value;
            // формат имени файла: закодированное имя измерения_закодированное имя иерархии_дата процессинга
            return EncryptString(dimensionName) + "@" + EncryptString(hierachyName) + "@" + EncryptString(date);
        }

        /// <summary>
        /// получить атрибуты файла (имя измерения, имя иерархии, дата процессинга)
        /// </summary>
        /// <param name="fileName"> имя файла кэша </param>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierarchyName"> имя иерархии </param>
        /// <param name="date"> дата процессинга </param>
        private static void GetFileKeys(string fileName, ref string dimensionName,
            ref string hierarchyName, ref string date)
        {
            string[] fileAttributes = fileName.Split('@');
            dimensionName = DecryptString(fileAttributes[0]);
            hierarchyName = DecryptString(fileAttributes[1]);
            date = DecryptString(fileAttributes[2]);
        }

        /// <summary>
        /// получить дату измерения из метаданных
        /// </summary>
        /// <param name="metadataDocument"> документ с метаданными </param>
        /// <param name="dimensionName"> имя измерения </param>
        /// <param name="hierachyName"> имя иерархии </param>
        /// <returns></returns>
        private static string GetMetadataDimensionDate(XmlDocument metadataDocument, string dimensionName, string hierachyName)
        {
            string xPath = "function_result/SharedDimensions/Dimension[@name='" + dimensionName +
                           "']/Hierarchy[@name='" + hierachyName + "']";
            XmlNode hierachyElement = metadataDocument.SelectSingleNode(xPath);
            return hierachyElement.Attributes.GetNamedItem("processing_date").Value;
        }

        /// <summary>
        /// сравнивает даты (формат даты "dd.mm.yyyy hh:mm:ss")
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns> -1 - date1 меньше date2; 0 даты равны; 1 - date1 больше date2 </returns>
        private static int CompareDates(string date1, string date2)
        {
            // обрабатываем частные случаи 
            if ((date1 == "null") && (date2 != "null"))
                return -1;
            if (date1 == date2)
                return 0;
            if ((date1 != "null") && (date2 == "null"))
                return 1;
            // сравниваем соответственные параметры дат
            // формат даты "dd.mm.yyyy hh:mm:ss"
            string[] dateParams1 = date1.Split('.');
            string[] dateParams2 = date2.Split('.');
            // получаем день
            int day1 = Convert.ToInt32(dateParams1[0]);
            int day2 = Convert.ToInt32(dateParams2[0]);
            // получаем месяц
            int month1 = Convert.ToInt32(dateParams1[1]);
            int month2 = Convert.ToInt32(dateParams2[1]);
            // получаем год
            dateParams1 = dateParams1[2].Split(' ');
            dateParams2 = dateParams2[2].Split(' ');
            int year1 = Convert.ToInt32(dateParams1[0]);
            int year2 = Convert.ToInt32(dateParams2[0]);
            // получаем час
            dateParams1 = dateParams1[1].Split(':');
            dateParams2 = dateParams2[1].Split(':');
            int hour1 = Convert.ToInt32(dateParams1[0]);
            int hour2 = Convert.ToInt32(dateParams2[0]);
            // получаем минуту
            int minute1 = Convert.ToInt32(dateParams1[1]);
            int minute2 = Convert.ToInt32(dateParams2[1]);
            // получаем секунду
            int second1 = Convert.ToInt32(dateParams1[2]);
            int second2 = Convert.ToInt32(dateParams2[2]);
            // сравниваем соответственные даты
            // сравниваем год
            if (year1 < year2)
                return -1;
            if (year1 > year2)
                return 1;
            // сравниваем месяц
            if (month1 < month2)
                return -1;
            if (month1 > month2)
                return 1;
            // сравниваем день
            if (day1 < day2)
                return -1;
            if (day1 > day2)
                return 1;
            // сравниваем час
            if (hour1 < hour2)
                return -1;
            if (hour1 > hour2)
                return 1;
            // сравниваем минуту
            if (minute1 < minute2)
                return -1;
            if (minute1 > minute2)
                return 1;
            // сравниваем секунду
            if (second1 < second2)
                return -1;
            if (second1 > second2)
                return 1;
            // даты равны
            return 0;
        }
    }
        #endregion
}
