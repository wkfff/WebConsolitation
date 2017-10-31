using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Text;

using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region Вспомогательный класс для загрузки XML
    /// <summary>
    /// Вспомогательный класс для загрузки XML с валидацией по схеме. 
    /// Стандартный из Krista.FM.Common привязан к одной схеме и делает 
    /// много ненужных действий
    /// </summary>
    public struct XmlLoader
    {
        // накопитель ошибок
        private static StringBuilder errors;

        /// <summary>
        /// Метод обратного вызова для регистрации ошибок проверки по схеме
        /// </summary>
        /// <param name="sender">проверяющий объект</param>
        /// <param name="e">описатель события</param>
        private static void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (errors == null)
                return;
            // помещаем информацию о сообщении в буфер
            errors.AppendFormat("Line: {0,4}\tPos: {1,4}\t{2}: {3}",
                e.Exception.LineNumber, e.Exception.LinePosition,
                e.Severity.ToString(), e.Message + Environment.NewLine);
        }

        /// <summary>
        /// Проверка наличия файла
        /// </summary>
        /// <param name="filePath">полное имя файла</param>
        /// <param name="error">сообщение об ошибке</param>
        /// <returns>результат ошибки</returns>
        private static bool CheckFile(string filePath, out string error)
        {
            error = String.Empty;
            if (!File.Exists(filePath))
            {
                error = String.Format("Файл '{0}' не найден", filePath);
            }
            return String.IsNullOrEmpty(error);
        }

        /// <summary>
        /// Загрузить XML дкумент и проверить его на соответствие схеме
        /// </summary>
        /// <param name="schemaPath">путь к схеме</param>
        /// <param name="xmlPath">путь к документу</param>
        /// <param name="xmlData">DomDocument с данными документа (если документ загрузился и успешно проверен)</param>
        /// <param name="message">сообщения об ошибках (если проверка закончилась неудачно)</param>
        /// <returns>true - документ загрузился и успешно проверен,  false - документ некорректен или несоответсвует схеме</returns>
        public static bool LoadXmlValidated(string schemaPath, string xmlPath, out XmlDocument xmlData, out string message)
        {
            xmlData = null;
            // проверяем наличие файлов
            if ((!CheckFile(schemaPath, out message)) ||
                (!CheckFile(xmlPath, out message)))
                return false;
            // загружаем схему
            XmlSchema sch = null;
            using (FileStream fs = new FileStream(schemaPath, FileMode.Open, FileAccess.Read))
            {
                sch = XmlSchema.Read(fs, null);
            }
            // создаем и загружаем XML документ
            xmlData = new XmlDocument();
            try
            {
                xmlData.Load(xmlPath);
            }
            catch (XmlException exp)
            {
                message = String.Format("Невозможно загрузить XML документ: {0}", exp.Message);
                return false;
            }
            // на всякий случай устанавлиаваем namespace такой же как и у схемы
            XmlAttribute attr = xmlData.DocumentElement.SetAttributeNode("xmlns", "http://www.w3.org/2000/xmlns/");
            attr.Value = sch.TargetNamespace;
            // создаем накопитель ошибок
            errors = new StringBuilder();
            // создаем StrinReader для данных XML
            using (StringReader sr = new StringReader(xmlData.InnerXml))
            {
                // создаем XmlReader с настройками, необходимыми для валидации
                XmlReaderSettings stt = new XmlReaderSettings();
                stt.CheckCharacters = false;
                stt.Schemas.Add(sch);
                stt.ValidationType = ValidationType.Schema;
                stt.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                // читаем и проверяем все данные XML
                using (XmlReader rdr = XmlReader.Create(sr, stt))
                {
                    while (rdr.Read()) { }
                }
            }
            // формируем сообщение об ошибке
            message = errors.ToString();
            errors = null;
            // если оно не пустое - освобождаем XML документ и возвращаем false
            if (!String.IsNullOrEmpty(message))
            {
                XmlHelper.ClearDomDocument(ref xmlData);
                return false;
            }
            // иначе - true
            return true;
        }

    }
    #endregion

}