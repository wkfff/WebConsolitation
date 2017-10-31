using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BudgetVaultPump
{
    // модуль закачки файлов XML
    // при случае переписать!!! (особенно работу с шаблонами...)
    public partial class BudgetVaultPumpModule : CorrectedPumpModuleBase
    {

        #region поля

        private XmlFormat xmlFormat;
        private XmlDocument patternDOM;
        private XmlForm[] xmlForms;
        private XmlForm xmlForm;
        private string[] forms;
        private int[] sections;
        private XmlNode patternNode;
        private XmlNode reportNode;
        private string codeAttrName;
        // районы, данные по которым были закачаны
        // ключ - код|наименование района, значение - (список форма+номер секции)
        protected Dictionary<string, List<string>> pumpedRegions = new Dictionary<string, List<string>>();

        #endregion поля

        #region Константы

        private const string PatternName = "Формы_Skif_PBUD";

        #endregion Константы

        #region Структуры, перечисления

        /// <summary>
        /// Формат файлов хмл
        /// </summary>
        protected enum XmlFormat
        {
            /// <summary>
            /// Формат 2004 г. и раньше
            /// </summary>
            Format2004,

            /// <summary>
            /// Формат 2005 г.
            /// </summary>
            Format2005,

            /// <summary>
            /// Skif3 (отчеты за 2005 год)
            /// </summary>
            Skif3
        }

        /// <summary>
        /// Формы отчетов хмл
        /// </summary>
        protected enum XmlForm
        {
            Form201, 
            Form202,
            Form203,
            Form204,
            Form207,
            Form204FK,
            UnknownForm
        }

        #endregion Структуры, перечисления

        #region Общие функции XML

        /// <summary>
        /// Возвращает элемент с данными отчета файла хмл
        /// </summary>
        /// <param name="xdReport">хмл-документ отчета</param>
        /// <returns>Элемент с данными отчета</returns>
        protected void GetReportNode(XmlDocument xdReport)
        {
            if (xmlFormat == XmlFormat.Format2004)
                reportNode = xdReport.SelectSingleNode("/Otchet");
            else
                reportNode = xdReport.SelectSingleNode("/RootXml/Report");
        }

        private bool CheckXMLFileName(string fileName)
        {
            try
            {
                if (fileName.StartsWith("350"))
                {
                    if (Convert.ToInt32(fileName.Split('_')[1]) != this.DataSource.Year)
                        throw new Exception("год в названии файла не соответствует источнику.");
                }
                else
                    if (Convert.ToInt32(fileName.Substring(fileName.ToUpper().IndexOf("ГОД") + 3, 4)) != this.DataSource.Year)
                        throw new Exception("год в названии файла не соответствует источнику.");
                return true;
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format("Ошибка при проверке названия файла {0}", fileName), ex);
                return false;
            }
        }

        private void GetPatternDOM(DirectoryInfo dir)
        {
            FileInfo[] filesList = dir.GetFiles(PatternName + "*.xml", SearchOption.AllDirectories);
            if (filesList.GetLength(0) == 0)
                return;
            if (filesList.GetLength(0) > 1)
                throw new PumpDataFailedException("В каталоге находится более одного шаблона.");
            xmlFilesCount--;
            patternDOM.Load(filesList[0].FullName);
        }

        private FileInfo[] GetReportList(FileInfo[] filesList)
        {
            FileInfo[] filesRepList = new FileInfo[0];
            for (int i = 0; i < filesList.GetLength(0); i++)
            {
                if (!File.Exists(filesList[i].FullName)) 
                    continue;
                if (filesList[i].Name.ToUpper().StartsWith(PatternName.ToUpper()))
                    continue;
                if (!CheckXMLFileName(filesList[i].Name))
                {
                    xmlFilesCount--;
                    continue;
                }
                filesRepList = (FileInfo[])CommonRoutines.RedimArray(filesRepList, filesRepList.GetLength(0) + 1);
                filesRepList[filesRepList.GetLength(0) - 1] = filesList[i];
            }
            return filesRepList;
        }

        /// <summary>
        /// Возвращает ссылку на классификатор (шаблон) узла данных отчета.
        /// </summary>
        /// <param name="dataNode">Узел данных</param>
        /// <param name="attrIndex">Индекс атрибута, содержащего ссылку (-1 - конкатенация всех атрибутов)</param>
        /// <returns>Ссылка</returns>
        private string GetClsfCode(XmlNode dataNode, int attrIndex)
        {
            string result = string.Empty;
            if (attrIndex >= 0)
            {
                if (attrIndex < dataNode.Attributes.Count)
                    result = dataNode.Attributes[attrIndex].Value;
            }
            else
                for (int i = 0; i < dataNode.Attributes.Count; i++)
                    result += dataNode.Attributes[i].Value;
            return result;
        }

        /// <summary>
        /// Проверяет, содержит ли файл отчета шаблон
        /// </summary>
        /// <param name="xmlDoc">Отчет</param>
        /// <returns>Содержит или нет</returns>
        private XmlNode GetPatternNode(XmlDocument xmlDoc)
        {
            return xmlDoc.SelectSingleNode("//RootXml/Task");
        }

        /// <summary>
        /// Закачивает данные районов
        /// </summary>
        /// <param name="xn">хмл-элемент с данными районов</param>
        /// <param name="regionsTable">Таблица районов</param>
        /// <param name="regionsCls">Объект классификатора</param>
        /// <param name="regionsCache">Кэш районов</param>
        private void PumpRegionsXML(DataTable regionsTable, IClassifier regionsCls, Dictionary<string, int> regionsCache, ref bool noRegForPump)
        {
            XmlNodeList xnlSources = reportNode.SelectNodes("//Source");
            for (int i = 0; i < xnlSources.Count; i++)
            {
                string regionCode = GetAttrValueByName(xnlSources[i].Attributes, "ObjectNmb", "Code").PadLeft(10, '0');
                string regionName = ConvertClsName(GetAttrValueByName(xnlSources[i].Attributes, "ObjectName", "Name"));
                string regionKey = regionCode + "|" + regionName;
                PumpCachedRow(regionsCache, regionsTable, regionsCls, regionCode, regionKey, "CODE", "ID",
                    new object[] { "NAME", regionName,
                                   "BUDGETKIND", GetAttrValueByName(xnlSources[i].Attributes, "ClassKey", "ClassCode"),
                                   "BUDGETNAME", GetAttrValueByName(xnlSources[i].Attributes, "ClassName") });
                if (!PumpRegionForPump(regionCode, regionKey, regionName))
                    noRegForPump = true;
            }
        }

        /// <summary>
        /// Преобразует форму хмл в строку
        /// </summary>
        /// <param name="xmlForm">Форма</param>
        private string XmlFormToString(XmlForm xmlForm)
        {
            switch (xmlForm)
            {
                case XmlForm.Form201: return "201";
                case XmlForm.Form202: return "202";
                case XmlForm.Form203: return "203";
                case XmlForm.Form204: return "204";
                case XmlForm.Form207: return "207";
                case XmlForm.Form204FK: return "204ФК";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Преобразует форму хмл в строку
        /// </summary>
        /// <param name="xmlForm">Форма</param>
        private XmlForm StringToXmlForm(string xmlForm)
        {
            switch (xmlForm.ToUpper())
            {
                case "201": return XmlForm.Form201;
                case "202": return XmlForm.Form202;
                case "203": return XmlForm.Form203;
                case "204": return XmlForm.Form204;
                case "207": return XmlForm.Form207;
                case "204ФК": return XmlForm.Form204FK;
                default: return XmlForm.UnknownForm;
            }
        }

        /// <summary>
        /// Преобразует массив номеров форм хмл в строку с разделением запятыми
        /// </summary>
        /// <param name="xmlForm">Массив форм</param>
        private string XmlFormsToString(XmlForm[] xmlForm)
        {
            string result = string.Empty;
            for (int i = 0; i < xmlForm.GetLength(0); i++)
                result += XmlFormToString(xmlForm[i]) + ", ";
            return result.Trim().Trim(',');
        }

        /// <summary>
        /// Преобразует форму хмл в строку
        /// </summary>
        /// <param name="xmlForm">Строка со списком форм</param>
        private XmlForm[] StringToXmlForms(string xmlForm)
        {
            string[] forms = xmlForm.Split(';');
            XmlForm[] result = new XmlForm[forms.GetLength(0)];
            for (int i = 0; i < forms.GetLength(0); i++)
                result[i] = StringToXmlForm(forms[i]);
            return result;
        }

        /// <summary>
        /// Ищет индекс атрибута по его названию
        /// </summary>
        /// <param name="xac">Коллекция атрибутов</param>
        /// <param name="attrName">Название атрибута</param>
        /// <returns>Индекс (-1 - не найден)</returns>
        private int GetAttributeIndex(XmlAttributeCollection xac, string attrName)
        {
            for (int i = 0; i < xac.Count; i++)
                if (xac[i].Name.ToUpper() == attrName.ToUpper()) 
                    return i;
            return -1;
        }

        /// <summary>
        /// Ищет атрибут в указанном элементе и возвращает его индекс
        /// </summary>
        /// <param name="xd">Документ</param>
        /// <param name="nodeName">Наименование элемента</param>
        /// <param name="attrName">Наименование атрибута</param>
        /// <returns>Индекс атрибута (-1 - не найден)</returns>
        private int GetTableAttributeIndex(XmlDocument xd, string nodeName, string attrName)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//Table[@Name = \"{0}\"]/TableFields", nodeName));
            if (xn == null) 
                return -1;
            return GetAttributeIndex(xn.Attributes, attrName);
        }

        /// <summary>
        /// Преобразует наименование классификатора к стандартному виду
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns>Наименование в стандартном виде</returns>
        private string ConvertClsName(string name)
        {
            string str = name.Replace("&quot;", "\"");
            if (string.IsNullOrEmpty(str))
                str = constDefaultClsName;
            return str;
        }

        #endregion Общие функции XML

        #region Закачка данных

        #region Функции закачки внешнего шаблона

        #region Вспомогательные функции

        /// <summary>
        /// Возвращает значения кода и наименования классификатора для старого шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        /// <param name="xnFormRow">Элемент с данными классификатора</param>
        /// <param name="itemNameInd">Индекс атрибута ItemName</param>
        /// <param name="clsfIDInd">Индекс атрибута ClsfID</param>
        /// <param name="clsfCodeInd">Индекс атрибута ClsfCode</param>
        /// <param name="indPagNoInd">Индекс атрибута IndPagNo</param>
        /// <param name="indRowNoInd">Индекс атрибута IndRowNo</param>
        /// <param name="code">Код</param>
        /// <param name="name">Наименование</param>
        /// <param name="kl">Код листа</param>
        /// <param name="kst">Код строки</param>
        /// <returns>Если что-то не найдено, то false</returns>
        private bool GetClsFieldsFromExternalXMLPattern(XmlNode xnFormRow, int itemNameInd, int clsfIDInd, int clsfCodeInd, int indPagNoInd, 
            int indRowNoInd, out string code, out string name, out int kl, out int kst)
        {
            code = string.Empty; name = string.Empty; kl = 0; kst = 0;
            string[] tableFields = xnFormRow.Attributes["Values"].Value.Split(new string[] { "," }, StringSplitOptions.None);
            // Теперь для каждого наименования получаем код и закачиваем
            XmlNode xnCls = patternDOM.SelectSingleNode(string.Format(
                "//Table[@Name = \"Skif_Classificator\" or @Name = \"MFClassificator\"]/RecordSet/" +
                "Row[starts-with(@Values, \"{0}\")]", tableFields[clsfIDInd]));
            if (xnCls == null)
                return false;
            string[] clsFields = xnCls.Attributes["Values"].Value.Split(',');
            code = clsFields[clsfCodeInd].Trim('\'');
            string[] nameField = xnFormRow.Attributes["Values"].Value.Split(new string[] { "'" }, StringSplitOptions.None);
            if (nameField.GetLength(0) >= 2)
                name = nameField[1].Trim();
            else
                name = constDefaultClsName;
            kl = Convert.ToInt32(tableFields[indPagNoInd]);
            kst = Convert.ToInt32(tableFields[indRowNoInd]);
            return true;
        }

        /// <summary>
        /// Возвращает индексы атрибутов для поиска данных классификаторов во внешнем шаблоне
        /// </summary>
        /// <param name="formIDInd">Индекс атрибута FormID</param>
        /// <param name="sectNoInd">Индекс атрибута SectNo</param>
        /// <param name="rowsSetIDInd">Индекс атрибута RowsSetID</param>
        /// <param name="itemNameInd">Индекс атрибута ItemName</param>
        /// <param name="clsfIDInd">Индекс атрибута ClsfID</param>
        /// <param name="clsfCodeInd">Индекс атрибута ClsfCode</param>
        /// <param name="indPagNoInd">Индекс атрибута IndPagNo</param>
        /// <param name="indRowNoInd">Индекс атрибута IndRowNo</param>
        private void GetClsAttributesIndexes(ref int formIDInd, ref int sectNoInd, ref int rowsSetIDInd, ref int itemNameInd, 
            ref int clsfIDInd, ref int clsfCodeInd, ref int indPagNoInd, ref int indRowNoInd)
        {
            // Получаем индексы нужных атрибутов
            formIDInd = GetTableAttributeIndex(patternDOM, "FormSection", "FormID");
            sectNoInd = GetTableAttributeIndex(patternDOM, "FormSection", "SectNo");
            rowsSetIDInd = GetTableAttributeIndex(patternDOM, "FormSection", "RowsSetID");
            itemNameInd = GetTableAttributeIndex(patternDOM, "FormRows", "ItemName");
            clsfIDInd = GetTableAttributeIndex(patternDOM, "FormRows", "ClsfID");
            indPagNoInd = GetTableAttributeIndex(patternDOM, "FormRows", "IndPagNo");
            indRowNoInd = GetTableAttributeIndex(patternDOM, "FormRows", "IndRowNo");
            clsfCodeInd = GetTableAttributeIndex(patternDOM, "MFClassificator", "ClsfCode");
            if (clsfCodeInd < 0)
                clsfCodeInd = GetTableAttributeIndex(patternDOM, "Skif_Classificator", "ClsfCode");
        }

        /// <summary>
        /// Создает ограничение для выборки данных с указанными значениями номера формы
        /// </summary>
        /// <param name="xmlForm">Массив форм</param>
        /// <returns>Ограничение</returns>
        private string GetConstrForExternalXMLPattern()
        {
            if (xmlForms == null) 
                return string.Empty;
            string result = string.Empty;
            Array.Sort(xmlForms);
            for (int i = 0; i < xmlForms.GetLength(0); i++)
                result += string.Format("starts-with(@Values, \"{0}\") or ", XmlFormToString(xmlForms[i]));
            if (result != string.Empty)
                result = "[" + result.Remove(result.Length - 4) + "]";
            return result;
        }

        /// <summary>
        /// Закачивает строку классификатора из шаблона, выгруженного в отдельный файл
        /// </summary>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="useCodeMapping">Если в списке полей указано поле кода, то его значение раскладывается по 
        /// указанному правилу, иначе берется найденное в шаблоне</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_строки_в_датасете для каждого классификатора</param>
        /// <param name="clsCode">Код классификатора из шаблона</param>
        /// <param name="clsName">Наименование классификатора из шаблона</param>
        /// <param name="kl">Код листа</param>
        /// <param name="kst">Код строки</param>
        /// <param name="fieldsMapping">Список пар нименование_поля - значение для таблицы классификатора</param>
        /// <param name="codeMasks">Массив масок подкодов кода классификатора. Если указан, то код классификатора
        /// будет собран из подкодов, полученных из attr2FieldMapping или attrValuesMapping, дополненных до
        /// указанной маски (useCodeMapping false)</param>
        private void PumpRowFromExternalXMLPattern(DataTable dt, IClassifier cls, 
            Dictionary<string, int> codesMapping, string clsCode, string clsName, int kl, int kst,
            string[] fieldsMapping, string clsCodeField, ClsProcessModifier clsProcessModifier)
        {
            // Определяем значение ключа кэша
            if (clsProcessModifier == ClsProcessModifier.CacheSubCode)
            {
                if (fieldsMapping[1].TrimStart('0') != string.Empty)
                    PumpCachedRow(codesMapping, dt, cls, fieldsMapping[1], clsCodeField, new object[] { "NAME", ConvertClsName(clsName), "KL", kl, "KST", kst });
            }
            else
                PumpCachedRow(codesMapping, dt, cls, clsCode, (object[])CommonRoutines.ConcatArrays(fieldsMapping,
                        new string[] { "NAME", ConvertClsName(clsName), "KL", kl.ToString(), "KST", kst.ToString() }));
        }

        /// <summary>
        /// Закачивает классификатор из шаблона, хранимого в отдельном файле
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        /// <param name="xmlForm">Номер формы</param>
        /// <param name="sectNo">Номер секции</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_строки_в_датасете для каждого классификатора</param>
        protected void PumpClsFromExternalXMLPattern(DataTable dt, IClassifier cls, Dictionary<string, int> clsCache, int clsCodeLenght)//
        {
            int formIDInd = -1; int sectNoInd = -1; int rowsSetIDInd = -1; int itemNameInd = -1;
            int clsfIDInd = -1; int indPagNoInd = -1; int indRowNoInd = -1; int clsfCodeInd = -1;
            GetClsAttributesIndexes(ref formIDInd, ref sectNoInd, ref rowsSetIDInd, ref itemNameInd, 
                ref clsfIDInd, ref clsfCodeInd, ref indPagNoInd, ref indRowNoInd);
            XmlNodeList xnlFormSectionRows = patternDOM.SelectNodes(string.Format("//Table[@Name = \"FormSection\"]/RecordSet/Row{0}", GetConstrForExternalXMLPattern()));
            if (xnlFormSectionRows.Count == 0) 
                return;
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);
            string clsCodeField = GetClsCodeField(cls);
            for (int i = 0; i < xnlFormSectionRows.Count; i++)
            {
                string[] tableFields = xnlFormSectionRows[i].Attributes["Values"].Value.Split(',');
                if (!CommonRoutines.CheckValueEntry(Convert.ToInt32(tableFields[sectNoInd]), sections))
                    continue;
                // Получаем элементы с наименованиями классификатора
                XmlNodeList xnlFormRows = patternDOM.SelectNodes(string.Format("//Table[@Name = \"FormRows\"]/RecordSet/Row[starts-with(@Values, \"{0}\")]",
                    tableFields[rowsSetIDInd]));
                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    string clsCode; string clsName; int kl; int kst;
                    if (!GetClsFieldsFromExternalXMLPattern(xnlFormRows[j], itemNameInd, clsfIDInd, clsfCodeInd, indPagNoInd, indRowNoInd,
                            out clsCode, out clsName, out kl, out kst))
                        continue;
                    if ((clsCodeLenght != -1) && (clsCode.Length != clsCodeLenght))
                        continue;
                    PumpCachedRow(clsCache, dt, cls, clsCode, new object[] { clsCodeField, clsCode, "NAME", ConvertClsName(clsName), "KL", kl, "KST", kst });
                    SetProgress(xnlFormRows.Count, j + 1, string.Format("Обработка шаблона. Данные {0}...", semantic),
                        string.Format("Строка {0} из {1}", j + 1, xnlFormRows.Count));
                }
            }
            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает классификатор из шаблона, хранимого в отдельном файле. 
        /// Позволяет задать правила формирования отдельных полей.
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        /// <param name="xmlForm">Номер формы</param>
        /// <param name="sectNo">Номер секции</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">Список пар поле-количество_символов.
        /// Поле - имя поля. Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов, num..-1 - все оставшиеся символы, начиная с num)</param>
        /// <param name="useCodeMapping">Если в списке полей указано поле кода, то его значение раскладывается по 
        /// указанному правилу, иначе берется найденное в шаблоне</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_строки_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Список исключений (описание см. в PumpXMLReportBlock)</param>
        /// <param name="indPagNo">Массив значений IndPagNo - ограничение для закачки. Описание см. в codeExclusions</param>
        /// <param name="clsProcessModifier">Модификатор обработки классификатора. Нужен для сообщения функции обработки 
        /// какой классификатор обрабатывается</param>
        /// <param name="codeMasks">Массив масок подкодов кода классификатора. Если указан, то код классификатора
        /// будет собран из подкодов, полученных из attr2FieldMapping или attrValuesMapping, дополненных до
        /// указанной маски (useCodeMapping false)</param>
        protected void PumpComplexClsFromExternalXMLPattern(DataTable dt, IClassifier cls, string[] attrValuesMapping, 
            Dictionary<string, int> codesMapping, string[] codeExclusions, ClsProcessModifier clsProcessModifier)
        {
            if (patternDOM.SelectNodes("//Table[@Name = \"FormSection\"]").Count == 0)
                throw new Exception("Ошибка при закачке шаблона: отсутствует или поврежден FormSection.");
            int formIDInd = -1; int sectNoInd = -1; int rowsSetIDInd = -1; int itemNameInd = -1;
            int clsfIDInd = -1; int indPagNoInd = -1; int indRowNoInd = -1; int clsfCodeInd = -1;
            GetClsAttributesIndexes(ref formIDInd, ref sectNoInd, ref rowsSetIDInd, ref itemNameInd,
                ref clsfIDInd, ref clsfCodeInd, ref indPagNoInd, ref indRowNoInd);
            XmlNodeList xnlFormSectionRows = patternDOM.SelectNodes(string.Format("//Table[@Name = \"FormSection\"]/RecordSet/Row{0}", GetConstrForExternalXMLPattern()));
            if (xnlFormSectionRows.Count == 0) 
                return;
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);
            string clsCodeField = GetClsCodeField(cls);
            // Список закачанных номеров секций - для предотвращения их повторного закачивания
            List<int> pumpedSectNo = new List<int>(30);
            // Обходим все строки FormSection и закачиваем соответствующие классификаторы
            for (int i = 0; i < xnlFormSectionRows.Count; i++)
            {
                string[] tableFields = xnlFormSectionRows[i].Attributes["Values"].Value.Split(',');
                int currSectNo = Convert.ToInt32(tableFields[sectNoInd]);
                if (pumpedSectNo.Contains(currSectNo))
                    continue;
                else
                    pumpedSectNo.Add(currSectNo);
                if (!CommonRoutines.CheckValueEntry(currSectNo, sections)) 
                    continue;
                // Получаем элементы с наименованиями классификатора
                XmlNodeList xnlFormRows = patternDOM.SelectNodes(string.Format("//Table[@Name = \"FormRows\"]/RecordSet/Row[starts-with(@Values, \"{0}\")]",
                    tableFields[rowsSetIDInd]));
                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    string clsCode;
                    string clsName;
                    int kl;
                    int kst;
                    if (!GetClsFieldsFromExternalXMLPattern(xnlFormRows[j], itemNameInd, clsfIDInd, clsfCodeInd, 
                            indPagNoInd, indRowNoInd, out clsCode, out clsName, out kl, out kst))
                        continue;
                    if (CheckCodeExclusion(clsCode, codeExclusions))
                        continue;
                    // Разбиваем код из хмл по полям классификатора
                    clsCode = clsCode.Replace(" ", "");
                    string[] fieldsMapping = GetFieldsValuesAsSubstring(attrValuesMapping, clsCode, "0");
                    // Закачиваем строку
                    PumpRowFromExternalXMLPattern(dt, cls, codesMapping, clsCode, clsName, kl, kst,
                        fieldsMapping, clsCodeField, clsProcessModifier);
                    SetProgress(xnlFormRows.Count, j + 1, string.Format("Обработка шаблона. Данные {0}...", semantic),
                        string.Format("Строка {0} из {1}", j + 1, xnlFormRows.Count));
                }
            }
            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
        }

        #endregion Вспомогательные функции

        #region Закачка блоков

        private void PumpIncomesClsFromExtPattern()
        {
            if (this.DataSource.Year >= 2005)
            {
                xmlForms = new XmlForm[] { XmlForm.Form204 };
                sections = new int[] { 1 };
                PumpClsFromExternalXMLPattern(dsKD.Tables[0], clsKD, kdCache, -1);
            }
            else
            {
                sections = new int[] { 1 };
                xmlForms = new XmlForm[] { XmlForm.Form204 };
                PumpClsFromExternalXMLPattern(dsKD.Tables[0], clsKD, kdCache, -1);
                xmlForms = new XmlForm[] { XmlForm.Form202 };
                PumpClsFromExternalXMLPattern(dsKD.Tables[0], clsKD, kdCache, 7);
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Расходы из внешнего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpOutcomesClsFromExtPattern()
        {
            if (this.DataSource.Year >= 2005)
                return;
            xmlForms = new XmlForm[] { XmlForm.Form204 };
            sections = CommonRoutines.ParseParamsString("3..25");
            PumpComplexClsFromExternalXMLPattern(dsKCSR.Tables[0], clsKCSR, new string[] { "CODE", "7..9:0000000" }, kcsrCache,
                new string[] { "!*000000000" }, ClsProcessModifier.CacheSubCode);
            PumpComplexClsFromExternalXMLPattern(dsKVR.Tables[0], clsKVR, new string[] { "CODE", "10..12" }, kvrCache,
                new string[] { "!*000000" }, ClsProcessModifier.CacheSubCode);
            PumpComplexClsFromExternalXMLPattern(dsEKR.Tables[0], clsEKR, new string[] { "CODESTR", "13..18" }, ekrCache,
                null, ClsProcessModifier.CacheSubCode);
            PumpComplexClsFromExternalXMLPattern(dsFKR.Tables[0], clsFKR, new string[] { "CODE", "0..4" }, fkrCache, new string[] { 
                    "!*000000000000000;7980*;0000*" }, ClsProcessModifier.CacheSubCode);
            xmlForms = new XmlForm[] { XmlForm.Form203 };
            sections = new int[] { 1 };
            PumpComplexClsFromExternalXMLPattern(dsEKR.Tables[0], clsEKR, new string[] { "CODESTR", "13..18" }, ekrCache,
                null, ClsProcessModifier.CacheSubCode);
            PumpComplexClsFromExternalXMLPattern(dsFKR.Tables[0], clsFKR, new string[] { "CODE", "0..4" }, fkrCache,
                new string[] { "!*000000000000000;7980*;0000*" }, ClsProcessModifier.CacheSubCode);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Источники финансирования из внешнего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpFinSourcesClsFromExtPattern()
        {
            if (this.DataSource.Year >= 2005)
            {
                xmlForms = new XmlForm[] { XmlForm.Form204 };
                sections = new int[] { 2 };
                PumpClsFromExternalXMLPattern(dsKIF2005.Tables[0], clsKIF2005, kifCache, -1);
            }
            else
            {
                xmlForms = new XmlForm[] { XmlForm.Form202 };
                sections = new int[] { 1 };
                PumpClsFromExternalXMLPattern(dsKIF2004.Tables[0], clsKIF2004, kifCache, 5);
                xmlForms = new XmlForm[] { XmlForm.Form204 };
                sections = new int[] { 2 };
                PumpClsFromExternalXMLPattern(dsKIF2004.Tables[0], clsKIF2004, kifCache, -1);
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Сеть Штаты Контингент из внешнего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpNetsClsFromExtPattern()
        {
            if (this.DataSource.Year >= 2005)
                return;
            xmlForms = new XmlForm[] { XmlForm.Form201 };
            sections = new int[] { 1 };
            PumpComplexClsFromExternalXMLPattern(dsKSHK.Tables[0], clsKSHK, new string[] { "CODE", "-1..3" }, kshkCache, null, ClsProcessModifier.CacheSubCode);
        }

        /// <summary>
        /// Закачивает шаблон старого формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        protected void PumpExternalXMLPattern()
        {
            if (xmlFormat == XmlFormat.Skif3)
                throw new Exception("Закачка из внешнего шаблона в формате Skif3 не поддерживается");
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromExtPattern();
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromExtPattern();
            if (ToPumpBlock(Block.bFinSources))
                PumpFinSourcesClsFromExtPattern();
            if (ToPumpBlock(Block.bNets))
                PumpNetsClsFromExtPattern();
        }

        #endregion Закачка блоков

        #endregion Функции закачки внешнего шаблона

        #region Функции закачки внутреннего шаблона

        #region Вспомогательные функции

        /// <summary>
        /// Формирует значение атрибута Code
        /// </summary>
        /// <param name="xmlForm">Форма</param>
        /// <param name="sectNo">Секция (-1 - не учитывать)</param>
        /// <returns>Значение атрибута</returns>
        private string GetReportCode(XmlForm xmlForm, int sectNo)
        {
            switch (xmlForm)
            {
                case XmlForm.Form201:
                    if (sectNo < 0)
                        return "@Code = \"201\" or ";
                    else
                        return string.Format("@Code = \"201{0:00}\" or ", sectNo);
                case XmlForm.Form202:
                    if (sectNo < 0)
                        return "@Code = \"202\" or ";
                    else
                        return string.Format("@Code = \"202{0:00}\" or ", sectNo);
                case XmlForm.Form203:
                    if (sectNo < 0)
                        return "@Code = \"203\" or ";
                    else
                        return string.Format("@Code = \"203{0:00}\" or ", sectNo);
                case XmlForm.Form204:
                    if (sectNo < 0)
                        return "@Code = \"204\" or ";
                    else
                        return string.Format("@Code = \"204{0:00}\" or ", sectNo);
                case XmlForm.Form207:
                    if (sectNo < 0)
                        return "@Code = \"207\" or ";
                    else
                        return string.Format("@Code = \"207{0:00}\" or ", sectNo);
                case XmlForm.Form204FK:
                    if (sectNo < 0)
                        return "@Code = \"204ФК\" or ";
                    else
                        return string.Format("@Code = \"204ФК{0:00}\" or ", sectNo);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Создает ограничение для выборки данных с указанными значениями номера формы и секции
        /// </summary>
        /// <param name="xmlForm">Массив форм</param>
        /// <param name="sectNo">Массив номеров секций</param>
        /// <returns>Ограничение</returns>
        private string GetConstrForInternalXMLPattern(XmlForm[] xmlForm, int[] sectNo)
        {
            if (xmlForm == null)
                return string.Empty;
            string result = string.Empty;
            for (int i = 0; i < xmlForm.GetLength(0); i++)
                if (sectNo != null && sectNo.GetLength(0) > 0)
                    for (int j = 0; j < sectNo.GetLength(0); j++)
                        result += GetReportCode(xmlForm[i], sectNo[j]);
                else
                    result += GetReportCode(xmlForm[i], -1);
            if (result != string.Empty)
                result = "[" + result.Remove(result.Length - 4) + "]";
            return result;
        }

        /// <summary>
        /// Собирает код классификатора по подкодам с учетом масок подкодов
        /// </summary>
        /// <param name="codeValues">Массив подкодов (имя_поля - код)</param>
        /// <param name="codeMasks">Массив масок</param>
        /// <returns>Код</returns>
        private string BuildCodeBySubCodesMask(string[] codeValues, int[] codeMasks)
        {
            string result = string.Empty;
            int count = codeValues.GetLength(0);
            for (int i = 0; i < count; i += 2)
                result += codeValues[i + 1].PadLeft(codeMasks[i / 2], '0');
            return result;
        }

        /// <summary>
        /// Записывает в кэш код классификатора
        /// </summary>
        /// <param name="dt">Таблица для поиска</param>
        /// <param name="codesMapping">Кэш</param>
        /// <param name="clsCode">Код классификатора</param>
        /// <param name="codeValues">Значения полей классификатора</param>
        private void WriteToCacheClsCode(DataTable dt, Dictionary<string, int> codesMapping, string clsCode,
            object[] codeValues)
        {
            if (codesMapping != null)
            {
                int id = FindRowID(dt, codeValues, -1);
                if (id != -1 && !codesMapping.ContainsKey(clsCode))
                    codesMapping.Add(clsCode, id);
            }
        }

        /// <summary>
        /// Закачивает строку из внутреннего шаблона
        /// </summary>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">Классификатор</param>
        /// <param name="useCodeMapping">Если в списке полей указано поле кода, то его значение раскладывается по 
        /// указанному правилу, иначе берется найденное в шаблоне</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="clsProcessModifier">Модификатор обработки классификатора. Нужен для сообщения функции обработки 
        /// какой классификатор обрабатывается</param>
        /// <param name="codeMasks">Массив масок подкодов кода классификатора. Если указан, то код классификатора
        /// будет собран из подкодов, полученных из attr2FieldMapping или attrValuesMapping, дополненных до
        /// указанной маски (useCodeMapping false)</param>
        /// <param name="clsCodeField">Имя поля кода классификатора</param>
        /// <param name="clsCode">Код</param>
        /// <param name="codeValues">Значения расщепленного кода</param>
        /// <param name="xn">Элемент с данными классификатора</param>
        /// <param name="kl">Код листа</param>
        private void PumpRowFromInternalXMLPattern(DataTable dt, IClassifier cls, bool useCodeMapping,
            Dictionary<string, int> codesMapping, ClsProcessModifier clsProcessModifier, int[] codeMasks,
            string clsCodeField, string clsCode, string[] codeValues, XmlNode xn, int kl)
        {
            string name = ConvertClsName(xn.Attributes["Name"].Value);
            string kst = xn.Attributes["Row"].Value;
            if (!useCodeMapping)
            {
                if (codeMasks == null)
                    PumpCachedRow(codesMapping, dt, cls, clsCode, (object[])CommonRoutines.ConcatArrays(codeValues, 
                        new string[] { clsCodeField, clsCode, "NAME", name, "KL", kl.ToString(), "KST", kst }));
                else
                {
                    string newClsCode = BuildCodeBySubCodesMask(codeValues, codeMasks);
                    PumpCachedRow(codesMapping, dt, cls, newClsCode, (object[])CommonRoutines.ConcatArrays(codeValues, 
                        new string[] { clsCodeField, newClsCode, "NAME", name, "KL", kl.ToString(), "KST", kst }));
                }
            }
            else
                // Определяем значение ключа кэша
                switch (clsProcessModifier)
                {
                    case ClsProcessModifier.CacheSubCode:
                    case ClsProcessModifier.EKR:
                    case ClsProcessModifier.FKR:
                        PumpCachedRow(codesMapping, dt, cls, codeValues[1], clsCodeField, new object[] { 
                            "NAME", name, "KL", kl, "KST", kst });
                        break;
                    default:
                        PumpCachedRow(codesMapping, dt, cls, clsCode, (object[])CommonRoutines.ConcatArrays(codeValues, 
                            new string[] { "NAME", name, "KL", kl.ToString(), "KST", kst }));
                        break;
                }
        }

        /// <summary>
        /// Закачивает классификатор из шаблона, выгруженного вместе с отчетом.
        /// Позволяет задать правила формирования отдельных полей.
        /// </summary>
        /// <param name="xnPattern">Элемент формы</param>
        /// <param name="xmlForm">Номер формы</param>
        /// <param name="sectNo">Номер секции</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attr2FieldMapping">Список пар поле-наименование_атрибута. Это означает, что в указанное поле
        /// будет закачано значение из атрибута с соответствующим наименованием. Может быть указано несколько 
        /// атрибутов через ; - будет закачано значение из первого атрибута, в котором оно есть.
        /// "attrName+attrName" - будет произведена конкатенация значений атрибутов.</param>
        /// <param name="attrValuesMapping">Список пар поле-количество_символов.
        /// Поле - имя поля. Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - 2 символа: num1, num2;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        /// <param name="useCodeMapping">Если в списке полей указано поле кода, то его значение раскладывается по 
        /// указанному правилу, иначе берется найденное в шаблоне</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Список исключений (описание см. в PumpXMLReportBlock)</param>
        /// <param name="indPagNo">Массив значений IndPagNo - ограничение для закачки. Описание см. в codeExclusions</param>
        /// <param name="clsProcessModifier">Модификатор обработки классификатора. Нужен для сообщения функции обработки 
        /// какой классификатор обрабатывается</param>
        /// <param name="codeMasks">Массив масок подкодов кода классификатора. Если указан, то код классификатора
        /// будет собран из подкодов, полученных из attr2FieldMapping или attrValuesMapping, дополненных до
        /// указанной маски (useCodeMapping false)</param>
        private void PumpComplexClsFromInternalXMLPattern(DataTable dt,
            IClassifier cls, string[] attr2FieldMapping, string[] attrValuesMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);
            string constr = GetConstrForInternalXMLPattern(xmlForms, sections);
            int totalNodes = patternNode.SelectNodes(string.Format("//FormTemplate{0}/FormRows/Rows/Row", constr)).Count;
            if (totalNodes == 0)
            {
                WriteToTrace(string.Format("Нет данных по классификатору {0}.", semantic), TraceMessageKind.Information);
                return;
            }
            int nodesCount = 0;
            // Получаем элементы с кодами и наименованиями классификатора
            XmlNodeList xnlFormTemplates = patternNode.SelectNodes(string.Format("//FormTemplate{0}", constr));
            string clsCodeField = GetClsCodeField(cls);
            // Список закачанных номеров секций - для предотвращения их повторного закачивания
            List<int> pumpedSectNo = new List<int>(30);
            for (int i = 0; i < xnlFormTemplates.Count; i++)
            {
                XmlNodeList xnlFormRows = xnlFormTemplates[i].SelectNodes("./FormRows/Rows/Row");
                int currSectNo = GetSectNoFromFormCode(xnlFormTemplates[i].Attributes["Code"].Value);
                if (pumpedSectNo.Contains(currSectNo))
                {
                    nodesCount += xnlFormRows.Count;
                    SetProgress(totalNodes, nodesCount, string.Format("Обработка шаблона. Данные {0}...", semantic),
                        string.Format("Запись {0} из {1}", nodesCount, totalNodes));
                    continue;
                }
                else
                    pumpedSectNo.Add(currSectNo);
                bool ToCheckKL = true;
                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    nodesCount++;
                    SetProgress(totalNodes, nodesCount, string.Format("Обработка шаблона. Данные {0}...", semantic),
                        string.Format("Запись {0} из {1}", nodesCount, totalNodes));
                    // Получаем код
                    // Этот код будет представлять собой конкатенацию атрибутов элемента хмл
                    string clsCode = GetClsfCode(xnlFormRows[j], -1);
                    // Формируем массив значений по атрибутам: массив пар значений поле-значение_атрибута
                    string[] codeValues = null;
                    switch (clsProcessModifier)
                    {
                        case ClsProcessModifier.SrcInFin:
                            if (attr2FieldMapping != null)
                                codeValues = GetFieldsValuesAtPos(attr2FieldMapping, xnlFormRows[j], true);
                            else if (attrValuesMapping != null)
                                codeValues = GetFieldsValuesAsSubstring(attrValuesMapping, clsCode, "0");
                            break;
                        default:
                            if (attr2FieldMapping != null)
                                codeValues = GetFieldsValuesAtPos(attr2FieldMapping, xnlFormRows[j], false);
                            else if (attrValuesMapping != null)
                                codeValues = GetFieldsValuesAsSubstring(attrValuesMapping, clsCode, "0");
                            break;
                    }
                    if (codeValues == null)
                        continue;
                    // Проверяем вхождение кода в список исключений
                    if (clsCode == string.Empty || CheckCodeExclusion(clsCode, codeExclusions))
                        continue;
                    XmlNode tmpNode = xnlFormRows[j].SelectSingleNode("./RowP");
                    int kl = Convert.ToInt32(tmpNode.Attributes["Page"].Value);
                    // Проверяем вхождение kl в список исключений
                    // не проверяем по KL для Показатели.МесОтч_СпрРасходы начиная с февраля 2007 
                    if ((ToCheckKL) && (indPagNo != null))
                        if (!CheckCodeExclusion(kl, indPagNo))
                            continue;
                    PumpRowFromInternalXMLPattern(dt, cls, useCodeMapping, codesMapping, clsProcessModifier,
                        codeMasks, clsCodeField, clsCode, codeValues, tmpNode, kl);
                }
            }
            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// Возвращает правило формирования поля для элемента данных НСИ
        /// </summary>
        /// <param name="xmlNode">Элемент НСИ</param>
        /// <param name="fieldMapping">Массив правил формирования полей</param>
        /// <returns>Правило для элемента НСИ</returns>
        private string[] GetFieldMappingForNSICatalog(XmlNode xmlNode, string[] fieldMapping)
        {
            string[] result = null;
            if (fieldMapping == null)
                return result;
            int count = fieldMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                if (xmlNode.Attributes["Code"].Value == fieldMapping[i])
                {
                    if (result == null)
                        result = new string[2];
                    else
                        Array.Resize(ref result, result.GetLength(0) + 2);
                    Array.Copy(fieldMapping[i + 1].Split(new char[] { ';' }, 2), 0, result, result.GetLength(0) - 2, 2);
                }
            }
            return result;
        }

        /// <summary>
        /// Закачивает классификатор из NSI шаблона, выгруженного вместе с отчетом.
        /// Позволяет задать правила формирования отдельных полей.
        /// </summary>
        /// <param name="xnPattern">Элемент шаблона</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrNames">Массив наименований атрибутов для выборки</param>
        /// <param name="attr2FieldMapping">Список пар наименование_атрибута-поле. Это означает, что в указанное поле
        /// будет закачано значение из атрибута с соответствующим наименованием. Формат поле:
        /// "fieldName;valueSubStr" - fieldName - наименование поля, формат valueSubStr:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - 2 символа: num1, num2;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Список исключений (описание см. в PumpXMLReportBlock)</param>
        protected void PumpClsFromInternalXMLNSIPattern(DataTable dt, IClassifier cls,
            string[] attrNames, string[] attr2FieldMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, bool skipCodeWithLetters)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);
            // Получаем элементы с кодами и наименованиями классификатора
            XmlNodeList xnlNSIData = patternNode.SelectNodes(string.Format("//NSI/Catalogs/Catalog{0}", GetXPathConstrByAttr("Code", attrNames)));
            for (int i = 0; i < xnlNSIData.Count; i++)
            {
                string[] fieldMapping = GetFieldMappingForNSICatalog(xnlNSIData[i], attr2FieldMapping);
                if (fieldMapping == null) 
                    continue;
                string[] exclusions = GetFieldMappingForNSICatalog(xnlNSIData[i], codeExclusions);
                XmlNodeList xnlCatalogItems = xnlNSIData[i].SelectNodes("./CatalogItem");
                for (int j = 0; j < xnlCatalogItems.Count; j++)
                {
                    string code = xnlCatalogItems[j].Attributes["Code"].Value;
                    if ((skipCodeWithLetters) && (CommonRoutines.TrimNumbers(code) != string.Empty))
                        continue;
                    if (exclusions != null)
                        if (CheckCodeExclusion(code, exclusions))
                            continue;
                    string[] fieldValues = GetFieldsValuesAsSubstring(fieldMapping, code, "0");
                    if (fieldValues == null) 
                        continue;
                    PumpCachedRow(codesMapping, dt, cls, fieldValues[1], fieldValues[0], new object[] { 
                        "NAME", ConvertClsName(xnlCatalogItems[j].Attributes["Name"].Value), "KL", "-1", "KST", "-1" });
                }
            }
            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// Возвращает значение атрибута по имени из списка
        /// </summary>
        /// <param name="xac">Коллекция атрибутов</param>
        /// <param name="attrName">Список имен атрибута</param>
        /// <returns>Значение атрибута</returns>
        private string GetAttrValueByName(XmlAttributeCollection xac, params string[] attrName)
        {
            int count = attrName.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                XmlNode xn = xac.GetNamedItem(attrName[i]);
                if (xn != null)
                    return xn.Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Возвращает массив значений полей классификатора, являющихся значениями атрибутов элемента
        /// </summary>
        /// <param name="fieldsMapping">Список пар поле-наименование_атрибута. Это означает, что в указанное поле
        /// будет закачано значение из атрибута с соответствующим наименованием. Может быть указано несколько 
        /// атрибутов через ; - будет закачано значение из первого атрибута, в котором оно есть.
        /// "attrName+attrName" - будет произведена конкатенация значений атрибутов.</param>
        /// <param name="xnlFormRow">Элемент хмл, содержащий атрибуты со значениями полей</param>
        /// <param name="skipEmptyAttr">Если встретится пустой атрибут, то вернется null</param>
        /// <returns>Массив значений полей</returns>
        private string[] GetFieldsValuesAtPos(string[] fieldsMapping, XmlNode xnlFormRow, bool skipEmptyAttr)
        {
            string[] codeValues = new string[fieldsMapping.GetLength(0)];
            int count = fieldsMapping.GetLength(0);
            for (int j = 0; j < count; j += 2)
            {
                codeValues[j] = fieldsMapping[j];
                string attrValue = string.Empty;
                attrValue = GetFieldValueAtPos(fieldsMapping[j + 1], xnlFormRow, false);
                if (attrValue == string.Empty)
                {
                    if (skipEmptyAttr)
                        return null;
                    else
                        codeValues[j + 1] = "0";
                }
                else
                    codeValues[j + 1] = attrValue;
            }
            return codeValues;
        }

        /// <summary>
        /// Возвращает значение заданного атрибута
        /// </summary>
        /// <param name="attr">Атрибут со значением классификатора. Может быть указано несколько 
        /// атрибутов через ; - будет закачано значение из первого атрибута, в котором оно есть.
        /// "attrName+attrName" - будет произведена конкатенация значений атрибутов.</param>
        /// <param name="xn">Элемент хмл с атрибутами</param>
        /// <param name="allowParent">true - провести поиск атрибутов сначала в родительском элементе</param>
        /// <returns>Значение</returns>
        private string GetFieldValueAtPos(string attr, XmlNode xn, bool allowParent)
        {
            string attrValue = string.Empty;
            XmlNode parent = xn.ParentNode;
            // Может быть указано несколько 
            // атрибутов через ; - будет закачано значение из первого атрибута, в котором оно есть.
            string[] attrNames = attr.Split(';');
            int count = attrNames.GetLength(0);
            for (int k = 0; k < count; k++)
            {
                // "attrName+attrName" - будет произведена конкатенация значений атрибутов
                string[] attrSummands = attrNames[k].Split('+');
                int summandsCount;
                if (allowParent && parent != null)
                {
                    summandsCount = attrSummands.GetLength(0);
                    for (int m = 0; m < summandsCount; m++)
                        attrValue += GetAttrValueByName(parent.Attributes, attrSummands[m]);
                }
                summandsCount = attrSummands.GetLength(0);
                for (int m = 0; m < summandsCount; m++)
                    attrValue += GetAttrValueByName(xn.Attributes, attrSummands[m]);
                if (attrValue != string.Empty)
                    break;
            }
            return attrValue;
        }

        /// <summary>
        /// Возвращает номер секции из кода формы
        /// </summary>
        /// <param name="formCode">Код формы</param>
        /// <returns>Номер секции</returns>
        private int GetSectNoFromFormCode(string formCode)
        {
            string str = CommonRoutines.RemoveLetters(formCode);
            if (str.Length >= 5)
                return Convert.ToInt32(str.Substring(str.Length - 2, 2));
            return -1;
        }

        #endregion Вспомогательные функции

        #region Закачка блоков

        /// <summary>
        /// Закачивает данные классификаторов блока Доходы из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpIncomesClsFromIntPattern()
        {
            if (this.DataSource.Year >= 2010)
                return;
            xmlForms = new XmlForm[] { XmlForm.Form204 };
            sections = new int[] { 1 };
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpComplexClsFromInternalXMLPattern(dsKD.Tables[0], clsKD, new string[] { "CODESTR", "АдмВД" }, null,
                        false, kdCache, new string[] { "!000*" }, null, ClsProcessModifier.Standard, null);
                    break;
                default:
                    PumpComplexClsFromInternalXMLPattern(dsKD.Tables[0], clsKD, new string[] { "CODESTR", "ВД" }, null,
                        false, kdCache, new string[] { "!000*" }, null, ClsProcessModifier.Standard, null);
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Расходы из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpOutcomesClsFromIntPattern()
        {
            xmlForms = new XmlForm[] { XmlForm.Form204 };
            sections = CommonRoutines.ParseParamsString("3..13");
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    if (this.DataSource.Year >= 2010)
                    {
                        PumpClsFromInternalXMLNSIPattern(dsFKR.Tables[0], clsFKR, new string[] { "РзПр" },
                            new string[] { "РзПр", "CODE;-1" }, fkrCache,
                            new string[] { "РзПр", "7900;0..4", "РзПр", "0000;0..4" }, true);
                        PumpClsFromInternalXMLNSIPattern(dsEKR.Tables[0], clsEKR, new string[] { "ЭКР" },
                            new string[] { "ЭКР", "CODE;-1" }, ekrCache,
                            new string[] { "ЭКР", "000;0..3" }, true);
                        PumpClsFromInternalXMLNSIPattern(dsKCSR.Tables[0], clsKCSR, new string[] { "ЦСР" },
                            new string[] { "ЦСР", "CODE;-1" }, kcsrCache,
                            new string[] { "ЦСР", "0000000;-1" }, true);
                        PumpClsFromInternalXMLNSIPattern(dsKVR.Tables[0], clsKVR, new string[] { "ВР" },
                            new string[] { "ВР", "CODE;-1" }, kvrCache,
                            new string[] { "ВР", "000;0..3" }, true);
                    }
                    else
                    {
                        PumpClsFromInternalXMLNSIPattern(dsKVR.Tables[0], clsKVR, new string[] { "РзПрЦСРВРПВРПСР" },
                            new string[] { "РзПрЦСРВРПВРПСР", "CODE;11..13" }, kvrCache,
                            new string[] { "РзПрЦСРВРПВРПСР", "!*000;-1", "РзПрЦСРВРПВРПСР", "000;11..13" }, true);
                        PumpClsFromInternalXMLNSIPattern(dsKCSR.Tables[0], clsKCSR, new string[] { "РзПрЦСРВРПВРПСР" },
                            new string[] { "РзПрЦСРВРПВРПСР", "CODE;4..10" }, kcsrCache,
                            new string[] { "РзПрЦСРВРПВРПСР", "!*000000;-1", "РзПрЦСРВРПВРПСР", "0000000;4..10" }, true);
                        PumpClsFromInternalXMLNSIPattern(dsFKR.Tables[0], clsFKR, new string[] { "РзПрЦСРВРПВРПСР" },
                            new string[] { "РзПрЦСРВРПВРПСР", "CODE;0..4" }, fkrCache,
                            new string[] { "РзПрЦСРВРПВРПСР", "!*0000000000000;-1", "РзПрЦСРВРПВРПСР", "7900;0..4" }, true);
                        PumpClsFromInternalXMLNSIPattern(dsEKR.Tables[0], clsEKR, new string[] { "РзПрЦСРВРПВРПСР" },
                            new string[] { "РзПрЦСРВРПВРПСР", "CODE;14..16" }, ekrCache,
                            new string[] { "РзПрЦСРВРПВРПСР", "000;14..16" }, true);
                    }
                    break;
                default:
                    PumpComplexClsFromInternalXMLPattern(dsKVR.Tables[0], clsKVR, new string[] { "CODE", "ВР" }, null, true, kvrCache,
                        new string[] { "000" }, null, ClsProcessModifier.CacheSubCode, null);
                    PumpComplexClsFromInternalXMLPattern(dsKCSR.Tables[0], clsKCSR, new string[] { "CODE", "ЦСР" }, null, true, kcsrCache,
                        new string[] { "0000000" }, null, ClsProcessModifier.CacheSubCode, null);
                    PumpComplexClsFromInternalXMLPattern(dsFKR.Tables[0], clsFKR, new string[] { "CODE", "РзПр" }, null, true, fkrCache,
                        new string[] { "7900*" }, null, ClsProcessModifier.CacheSubCode, null);
                    PumpComplexClsFromInternalXMLPattern(dsEKR.Tables[0], clsEKR, new string[] { "CODESTR", "ПСР" }, null, true, ekrCache,
                        new string[] {"000"}, null, ClsProcessModifier.CacheSubCode, null);
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Источники финансирования из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpFinSourcesClsFromIntPattern()
        {
            if (this.DataSource.Year >= 2010)
                return;
            xmlForms = new XmlForm[] { XmlForm.Form204 };
            sections = new int[] { 2 };
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpComplexClsFromInternalXMLPattern(dsKIF2005.Tables[0], clsKIF2005, new string[] { "CODESTR", "АдмКИВФ;АдмКИВнФ" }, 
                        null, false, kifCache, null, null, ClsProcessModifier.Standard, null);
                    break;
                default:
                    PumpComplexClsFromInternalXMLPattern(dsKIF2005.Tables[0], clsKIF2005, new string[] { "CODESTR", "КИВФ;КИВнФ;ИФ" }, 
                        null, false, kifCache, null, null, ClsProcessModifier.Standard, null);
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Сеть Штаты Контингент из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpNetsClsFromIntPattern()
        {
            xmlForms = new XmlForm[] { XmlForm.Form201 };
            sections = new int[] { 1 };
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpClsFromInternalXMLNSIPattern(dsKSHK.Tables[0], clsKSHK, new string[] { "РзПрЦСРВРСШ" },
                        new string[] { "РзПрЦСРВРСШ", "CODE;-1..3" }, kshkCache, null, false);
                    break;
                default:
                    PumpComplexClsFromInternalXMLPattern(dsKSHK.Tables[0], clsKSHK,
                        new string[] { "CODE", "СШ" }, null, true, kshkCache, null, null, ClsProcessModifier.CacheSubCode, null);
                    break;
            }
            if (this.DataSource.Year >= 2010)
            {
                xmlForms = new XmlForm[] { XmlForm.Form207 };
                sections = new int[] { 1 };
                PumpClsFromInternalXMLNSIPattern(dsKSHK.Tables[0], clsKSHK, new string[] { "РзПрЦСРВРСШ" },
                    new string[] { "РзПрЦСРВРСШ", "CODE;-1..3" }, kshkCache, null, false);
            }
        }

        /// <summary>
        /// Закачивает шаблон нового формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        private void PumpInternalXMLPattern()
        {
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromIntPattern();
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromIntPattern();
            if (ToPumpBlock(Block.bFinSources))
                PumpFinSourcesClsFromIntPattern();
            if (ToPumpBlock(Block.bNets))
                PumpNetsClsFromIntPattern();
        }

        #endregion Закачка блоков

        #endregion Функции закачки внутреннего шаблона

        #region Функции закачки блоков

        #region Вспомогательные функции

        private string GetXPathConstrByAttr(string attrName, object[] attrValues)
        {
            string result = string.Empty;
            int count = attrValues.GetLength(0);
            for (int i = 0; i < count; i++)
                result += string.Format(" or @{0} = \"{1}\"", attrName, attrValues[i].ToString());
            if (result != string.Empty)
                result = "[" + result.Remove(0, 4) + "]";
            return result;
        }

        private string GetXPathConstrByAttr(string attrName, XmlForm[] xmlForm)
        {
            string result = string.Empty;
            for (int i = 0; i < xmlForm.GetLength(0); i++)
                result += string.Format(" or @{0} = \"{1}\"", attrName, XmlFormToString(xmlForm[i]));
            if (result != string.Empty)
                result = "[" + result.Remove(0, 4) + "]";
            return result;
        }

        private string GetXPathConstrByAttr(string attrName, int[] attrValues)
        {
            string result = string.Empty;
            int count = attrValues.GetLength(0);
            for (int i = 0; i < count; i++)
                result += string.Format(" or @{0} = \"{1}\"", attrName, attrValues[i]);
            if (result != string.Empty)
                result = "[" + result.Remove(0, 4) + "]";
            return result;
        }

        /// <summary>
        /// Формирует код классификатора для поиска ссылок на классификаторы строки фактов
        /// </summary>
        /// <param name="clsCodeAttr">Атрибуты кода</param>
        /// <param name="xn">Элемент со строкой фактов</param>
        /// <returns>Код</returns>
        private string GetClsCodeFromFactNode(string clsCodeAttr, XmlNode xn)
        {
            string result = string.Empty;
            if (clsCodeAttr == string.Empty)
                result = GetClsfCode(xn, -1);
            else
                result = GetFieldValueAtPos(clsCodeAttr, xn, true);
            return result;
        }

        /// <summary>
        /// Выбирает данные отчета по указанным параметрам
        /// </summary>
        /// <param name="xn">Элемент с данными отчета</param>
        /// <param name="sectNo">Номера секций</param>
        /// <param name="xmlForm">Массив форм отчета</param>
        /// <returns>Элементы с данными отчета</returns>
        private XmlNodeList GetFactDataNodes(XmlNode xn, int[] sectNo, XmlForm[] xmlForm)
        {
            switch (xmlFormat)
            {
                case XmlFormat.Format2004:
                    return xn.SelectNodes(string.Format(".//Forma{0}/Section{1}/Data",
                        GetXPathConstrByAttr("FormNo", xmlForm), GetXPathConstrByAttr("SectNo", sectNo)));
                default:
                    return xn.SelectNodes(string.Format(".//Form{0}/Document/Data",
                        GetConstrForInternalXMLPattern(xmlForm, sectNo)));
            }
        }

        /// <summary>
        /// Возвращает количество элементов с данными
        /// </summary>
        /// <param name="xn">Корневой элемент (где искать)</param>
        /// <param name="formSection">Массив пар номер_формы-номер_секции. Каждая пара определяет, даыные из каких 
        /// секций и форм будут закачаны (соответствие секций формам). Номера форм должны быть перечислены через ";".
        /// Формат номер_секции:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в 
        /// варианте 2)</param>
        /// <returns>Количество элементов с данными</returns>
        private int GetTotalDataNodes(XmlNode xn, string[] formSection)
        {
            int result = 0;
            for (int i = 0; i < formSection.GetLength(0); i += 2)
                result += GetFactDataNodes(xn, CommonRoutines.ParseParamsString(formSection[i + 1]), StringToXmlForms(formSection[i])).Count;
            return result;
        }

        private double GetPxNodeValue(XmlNode xn, double defaultValue, string colNo)
        {
            if (colNo == string.Empty)
                return defaultValue;
            string[] colNoArray = colNo.Split(';');
            XmlNodeList px = xn.SelectNodes(string.Format("./Px{0}", GetXPathConstrByAttr("ColNo", colNoArray)));
            if (px.Count == 0)
                px = xn.SelectNodes(string.Format("./Px{0}", GetXPathConstrByAttr("Num", colNoArray)));
            for (int i = 0; i < px.Count; i++)
            {
                double d = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * sumFactor;
                if (d != 0)
                    return d;
            }
            return defaultValue;
        }

        private void PumpXMLNode(DataTable factTable, XmlNode dataNode, object[] clsValuesMapping, object[] colNo2ColumnMapping, object[] refsMapping)
        {
            bool zeroSums = true;
            int count = colNo2ColumnMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                double sum = GetPxNodeValue(dataNode, 0, colNo2ColumnMapping[i + 1].ToString());
                colNo2ColumnMapping[i + 1] = sum.ToString();
                if (sum != 0)
                    zeroSums = false;
            }
            if (!zeroSums)
                PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(colNo2ColumnMapping, clsValuesMapping, refsMapping));
        }

        private void ProcessForm201(DataTable factTable, XmlNode dataNode, string[] periodID, object[] clsValuesMapping, int regID)
        {
            if (this.DataSource.Year >= 2010)
            {
                object[] sumMapping = new object[] { "BEGYEAR", "1", "ENDYEAR", "4", "MIDYEAR", "7" };
                if (block == Block.bOutcomes)
                    sumMapping = new object[] { "AssignedReport", "1" };
                PumpXMLNode(factTable, dataNode, clsValuesMapping, sumMapping,
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 2 });

                sumMapping = new object[] { "BEGYEAR", "2", "ENDYEAR", "5", "MIDYEAR", "8" };
                if (block == Block.bOutcomes)
                    sumMapping = new object[] { "AssignedReport", "2" };
                PumpXMLNode(factTable, dataNode, clsValuesMapping, sumMapping,
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 3 });

                sumMapping = new object[] { "BEGYEAR", "3", "ENDYEAR", "6", "MIDYEAR", "9" };
                if (block == Block.bOutcomes)
                    sumMapping = new object[] { "AssignedReport", "3" };
                PumpXMLNode(factTable, dataNode, clsValuesMapping, sumMapping,
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 9 });
            }
            else
                PumpXMLNode(factTable, dataNode, clsValuesMapping,
                    new object[] { "BEGYEAR", "1", "ENDYEAR", "2", "MIDYEAR", "3" },
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID });
        }

        private void ProcessForm207(DataTable factTable, XmlNode dataNode, string[] periodID, object[] clsValuesMapping, int regID)
        {
            object[] sumMapping = new object[] { "BEGYEAR", "2", "ENDYEAR", "6", "MIDYEAR", "10" };
            if (block == Block.bOutcomes)
                sumMapping = new object[] { "AssignedReport", "2" };
            PumpXMLNode(factTable, dataNode, clsValuesMapping, sumMapping, 
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 4 });

            sumMapping = new object[] { "BEGYEAR", "3", "ENDYEAR", "7", "MIDYEAR", "11" };
            if (block == Block.bOutcomes)
                sumMapping = new object[] { "AssignedReport", "3" };
            PumpXMLNode(factTable, dataNode, clsValuesMapping, sumMapping, 
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 5 });

            sumMapping = new object[] { "BEGYEAR", "4", "ENDYEAR", "8", "MIDYEAR", "12" };
            if (block == Block.bOutcomes)
                sumMapping = new object[] { "AssignedReport", "4" };
            PumpXMLNode(factTable, dataNode, clsValuesMapping, sumMapping,
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 6 });
        }

        private void ProcessForm202(DataTable factTable, XmlNode dataNode, string[] periodID, object[] clsValuesMapping, int regID, string regionCode)
        {
            if (xmlFormat != XmlFormat.Format2004)
                return;
            int budgetLevel = 7;
            if (this.DataSource.Year <= 2004)
            {
                if ((regionCode.Length == 3) && (!regionCode.Contains("0")))
                    budgetLevel = 3;
                if ((regionCode.Length == 5) && (regionCode.EndsWith("00")))
                    budgetLevel = 3;
            }
            PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "2" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regID });
            PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "3" },
                new object[] { "REFYEARDAYUNV", periodID[1], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regID });
            PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "4" },
                new object[] { "REFYEARDAYUNV", periodID[2], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regID });
            PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "5" },
                new object[] { "REFYEARDAYUNV", periodID[3], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regID });
        }

        private void ProcessForm204(DataTable factTable, XmlNode dataNode, string[] periodID, object[] clsValuesMapping, int regID)
        {
            PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "1" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 2, "REFREGIONS", regID });
            PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "2" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 3, "REFREGIONS", regID });
            if (this.DataSource.Year >= 2007)
                PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "3" },
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 11, "REFREGIONS", regID });
            else
                PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "3" },
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 7, "REFREGIONS", regID });
            if (xmlFormat == XmlFormat.Skif3)
            {
                PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "4" },
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 4, "REFREGIONS", regID });
                PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "5" },
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 5, "REFREGIONS", regID });
                PumpXMLNode(factTable, dataNode, clsValuesMapping, new object[] { "ASSIGNEDREPORT", "6" },
                    new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 6, "REFREGIONS", regID });
            }
        }

        private void ProcessForm204FK(DataTable factTable, XmlNode dataNode, string[] periodID, object[] clsValuesMapping, int regID)
        {
            PumpXMLNode(factTable, dataNode, clsValuesMapping,
                new object[] { "AssignedReport", "1" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 2 });
            PumpXMLNode(factTable, dataNode, clsValuesMapping,
                new object[] { "AssignedReport", "2" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 3 });
            PumpXMLNode(factTable, dataNode, clsValuesMapping,
                new object[] { "AssignedReport", "3" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 9 });
            PumpXMLNode(factTable, dataNode, clsValuesMapping,
                new object[] { "AssignedReport", "4" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 4 });
            PumpXMLNode(factTable, dataNode, clsValuesMapping,
                new object[] { "AssignedReport", "5" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 5 });
            PumpXMLNode(factTable, dataNode, clsValuesMapping,
                new object[] { "AssignedReport", "6" },
                new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regID, "RefBdgtLevels", 6 });
        }

        private string[] GetXMLPeriodRefs()
        {
            string[] periodID = new string[] { };
            switch (xmlFormat)
            {
                case XmlFormat.Format2004:
                    switch (xmlForm)
                    {
                        case XmlForm.Form201:
                        case XmlForm.Form204:
                        case XmlForm.Form207:
                            periodID = new string[] { this.DataSource.Year.ToString() + "0001" };
                            break;
                        case XmlForm.Form202:
                        case XmlForm.Form203:
                            for (int j = 1; j <= 4; j += 1)
                            {
                                // формат квартала: ГГГГ999К
                                string quarter = this.DataSource.Year.ToString() + "999" + j.ToString();
                                periodID = (string[])CommonRoutines.ConcatArrays(periodID, new string[] { quarter });
                            }
                            break;
                    }
                    break;
                default:
                    periodID = new string[] { this.DataSource.Year.ToString() + "0001" };
                    break;
            }
            return periodID;
        }

        private void ProcessXMLNode(IDbDataAdapter da, DataTable factTable, XmlNode dataNode, int regionID,
            object[] clsValuesMapping, string[] periodID, string regionCode)
        {
            switch (xmlForm)
            {
                case XmlForm.Form201:
                    ProcessForm201(factTable, dataNode, periodID, clsValuesMapping, regionID);
                    break;
                case XmlForm.Form207:
                    ProcessForm207(factTable, dataNode, periodID, clsValuesMapping, regionID);
                    break;
                case XmlForm.Form202:
                case XmlForm.Form203:
                    ProcessForm202(factTable, dataNode, periodID, clsValuesMapping, regionID, regionCode);
                    break;
                case XmlForm.Form204:
                    ProcessForm204(factTable, dataNode, periodID, clsValuesMapping, regionID);
                    break;
                case XmlForm.Form204FK:
                    ProcessForm204FK(factTable, dataNode, periodID, clsValuesMapping, regionID);
                    break;
            }
        }

        private string[] GetXMLCacheKey(string code)
        {
            sumFactor = 1000;
            string[] cacheKeys = new string[] { };
            switch (block)
            {
                case Block.bIncomes:
                    cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys, new string[] { code, code });
                    break;
                case Block.bOutcomes:
                    if (this.DataSource.Year >= 2010)
                    {
                        if ((xmlForm == XmlForm.Form201) || (xmlForm == XmlForm.Form207))
                            cacheKeys = new string[] { "-1", "-1", code.TrimStart('0').PadLeft(1, '0'), "-1" };
                        else
                            cacheKeys = new string[] { code.Substring(14, 3).TrimStart('0').PadLeft(1, '0'),
                                code.Substring(4, 7).TrimStart('0').PadLeft(1, '0'), code.Substring(0, 4).TrimStart('0').PadLeft(1, '0'), 
                                code.Substring(11, 3).TrimStart('0').PadLeft(1, '0') };
                    }
                    else if (this.DataSource.Year >= 2005)
                        cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys,
                            new string[] { code.Substring(14, 3).TrimStart('0').PadLeft(1, '0'),
                            code.Substring(4, 7).TrimStart('0').PadLeft(1, '0'), code.Substring(0, 4).TrimStart('0').PadLeft(1, '0'),
                            code.Substring(11, 3).TrimStart('0').PadLeft(1, '0') });
                    else
                        cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys,
                            new string[] { code.Substring(13, 6).TrimStart('0').PadLeft(1, '0'), 
                                           code.Substring(7, 3).PadRight(7, '0').TrimStart('0').PadLeft(1, '0'), 
                                           code.Substring(0, 4).TrimStart('0').PadLeft(1, '0'),
                                           code.Substring(10, 3).TrimStart('0').PadLeft(1, '0') });
                    break;
                case Block.bFinSources:
                    cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys, new string[] { code, code });
                    break;
                case Block.bNets:
                    string kshk = code.Substring(code.Length - 3, 3);
                    if (this.DataSource.Year >= 2005)
                        cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys,
                        new string[] { code.Substring(0, 4).TrimStart('0').PadLeft(1, '0'), code.Substring(4, 7).TrimStart('0').PadLeft(1, '0'), 
                                   code.Substring(11, 3).TrimStart('0').PadLeft(1, '0'), code.Substring(0, code.Length - 3).PadLeft(1, '0'),
                                   kshk });
                    else
                        cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys,
                        new string[] { code.Substring(0, 4).TrimStart('0').PadLeft(1, '0'), code.Substring(7, 3).PadRight(7, '0').TrimStart('0').PadLeft(1, '0'), 
                                   code.Substring(10, 3).TrimStart('0').PadLeft(1, '0'), code.Substring(0, code.Length - 3).PadLeft(1, '0'),
                                   kshk });
                    // у сетей в случае ряда кодов ксшк домножать на 1000 не нужно
                    SetNetsSumFactor(Convert.ToInt32(kshk));
                    break;
                default:
                    code = code.TrimStart('0');
                    cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys, new string[] { code });
                    break;
            }
            return cacheKeys;
        }

        private void PumpXMLReportData(IDbDataAdapter da, DataTable factTable, DataTable[] clsTables,
            int[] nullRefsToCls, Dictionary<string, int>[] clsCaches, string[] codeExclusions, object[] clsValuesMapping)
        {
            int totalDataNodes = GetTotalDataNodes(reportNode, forms);
            WriteToTrace(string.Format("{0} элементов Data.", totalDataNodes), TraceMessageKind.Information);
            if (totalDataNodes == 0) 
                return;
            int nodesCount = 0;
            XmlNodeList xnlSources = reportNode.SelectNodes("//Source");
            for (int i = 0; i < xnlSources.Count; i++)
            {
                string regionCode = GetAttrValueByName(xnlSources[i].Attributes, "ObjectNmb", "Code").PadLeft(10, '0');
                string regionName = GetAttrValueByName(xnlSources[i].Attributes, "ObjectName", "Name");
                string regionKey = regionCode + "|" + regionName;
                int regionID = FindCachedRow(regionCache, regionKey, -1);
                if (regionID == -1)
                    continue;
                for (int f = 0; f < forms.GetLength(0); f += 2)
                {
                    xmlForms = StringToXmlForms(forms[f]);
                    int[] sectNoList = CommonRoutines.ParseParamsString(forms[f + 1]);
                    for (int j = 0; j < xmlForms.GetLength(0); j++)
                    {
                        if (xmlForms[j] == XmlForm.UnknownForm)
                            continue;
                        xmlForm = xmlForms[j];

                        // очередной ВЫСЕР постановки, убил бы нахуй в пизду.
                        // расходы, с 2010 года.
                        // из 201, 207 качаем, если нет форм 204фк,
                        // НО, бля пиздец нахуй 1) если нет 204ФК22, то из 201, 207 качаем только РзПр=9700
                        // 2) если нет 204ФК12, то из 201, 207 качаем все, кроме РзПр=9700
                        // ББПЕ!!!!!
                        string[] codeExclWithFormPrior = (string[])codeExclusions.Clone();
                        if ((block == Block.bOutcomes) && (this.DataSource.Year >= 2010))
                            if ((xmlForm == XmlForm.Form201) || (xmlForm == XmlForm.Form207))
                                if (pumpedRegions.ContainsKey(regionKey))
                                {
                                    // отсутствует 204фк22, из 201, 207 качаем только РзПр=9700
                                    if (pumpedRegions[regionKey].Contains("FORM204FK12") && 
                                        !pumpedRegions[regionKey].Contains("FORM204FK22"))
                                        codeExclWithFormPrior[0] += ";!9700";
                                    // отсутствует 204фк12, из 201, 207 качаем все, кроме РзПр=9700
                                    if (pumpedRegions[regionKey].Contains("FORM204FK22") &&
                                        !pumpedRegions[regionKey].Contains("FORM204FK12"))
                                        codeExclWithFormPrior[0] += ";9700";
                                    // если присутствуют обе формы 204ФК, то из 201, 207 качать не надо
                                    if (pumpedRegions[regionKey].Contains("FORM204FK22") &&
                                        pumpedRegions[regionKey].Contains("FORM204FK12"))
                                        continue;
                                }

                        periodID = GetXMLPeriodRefs();
                        XmlNodeList xnlData = GetFactDataNodes(xnlSources[i], sectNoList, new XmlForm[] { xmlForms[j] });
                        if (xnlData.Count == 0)
                            continue;

                        List<string> formsList = null;
                        string section = string.Empty;
                        if (sectNoList.GetLength(0) != 0)
                            section = sectNoList[0].ToString();
                        string formSectionStr = string.Format("{0}{1}", xmlForm.ToString().ToUpper(), section);
                        if (!pumpedRegions.ContainsKey(regionKey))
                        {
                            formsList = new List<string>();
                            pumpedRegions.Add(regionKey, formsList);
                        }
                        else
                            formsList = pumpedRegions[regionKey];
                        if (!formsList.Contains(formSectionStr))
                            formsList.Add(formSectionStr);

                        for (int k = 0; k < xnlData.Count; k++)
                        {
                            nodesCount++;
                            SetProgress(totalDataNodes, nodesCount, progressMsg, string.Format("{0}. Строка {1} из {2}", blockName, nodesCount, totalDataNodes));
                            string clsCode = GetClsCodeFromFactNode(codeAttrName, xnlData[k]);
                            clsCode = clsCode.Replace(" ", "");
                            if (CheckCodeExclusion(clsCode, codeExclWithFormPrior))
                                continue;
                            // проверяем только для формы 204
                            if ((forms[0] == "204") && (!CheckCode(clsCode)))
                                continue;
                            // у доходов из 202 формы код должен быть семизначным
                            if ((block == Block.bIncomes) && (forms[0] == "202") && (clsCode.Length != 7))
                                continue;
                            // у источников финансирования из 202 формы код должен быть пятизначным
                            if ((block == Block.bFinSources) && (forms[0] == "202") && (clsCode.Length != 5))
                                continue;
                            // приводим Х к одному виду - к английскому
                            if (block == Block.bNets)
                                clsCode = clsCode.ToUpper().Replace('Х', 'X');

                            if (clsCode != string.Empty)
                                FormCLSFromReport(clsCode);
                            string[] cacheKeys = GetXMLCacheKey(clsCode);
                            GetClsValuesMapping(clsTables, nullRefsToCls, clsCaches, ref clsValuesMapping, cacheKeys);
                            ProcessXMLNode(da, factTable, xnlData[k], regionID, clsValuesMapping, periodID, regionCode);
                            if (factTable.Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                            {
                                UpdateData();
                                ClearDataSet(da, factTable);
                            }
                        }
                    }
                }
            }
        }

        #endregion Вспомогательные функции

        #region Закачка блоков

        #region Доходы

        private void PumpSkif3Incomes()
        {
            forms = new string[] { "204", "1" };
            codeAttrName = "АдмВД";
            object[] ClsDefaultMapping = new object[] { "REFKDFOPROJ", nullKD };
            PumpXMLReportData(daIncomes, dsIncomes.Tables[0], new DataTable[] { dsKD.Tables[0] },
                new int[] { nullKD }, new Dictionary<string, int>[] { kdCache }, new string[] { }, ClsDefaultMapping);
        }

        private void Pump2005Incomes()
        {
            codeAttrName = "ВД";
            object[] ClsDefaultMapping = new object[] { "REFKDFOPROJ", nullKD };
            forms = new string[] { "202", "1" };
            PumpXMLReportData(daIncomes, dsIncomes.Tables[0], new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                new int[] { nullKD }, new Dictionary<string, int>[] { kdCache }, new string[] { }, ClsDefaultMapping);
            forms = new string[] { "204", "1" };
            PumpXMLReportData(daIncomes, dsIncomes.Tables[0], new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                new int[] { nullKD }, new Dictionary<string, int>[] { kdCache }, new string[] { }, ClsDefaultMapping);
        }

        private void Pump2004Incomes()
        {
            codeAttrName = "ClsfCode";
            object[] ClsDefaultMapping = new object[] { "REFKDFOPROJ", nullKD };
            forms = new string[] { "202", "1" };
            PumpXMLReportData(daIncomes, dsIncomes.Tables[0], new DataTable[] { dsKD.Tables[0] },
                new int[] { nullKD }, new Dictionary<string, int>[] { kdCache }, new string[] { }, ClsDefaultMapping);
            forms = new string[] { "204", "1" };
            PumpXMLReportData(daIncomes, dsIncomes.Tables[0], new DataTable[] { dsKD.Tables[0] },
                new int[] { nullKD }, new Dictionary<string, int>[] { kdCache }, new string[] { }, ClsDefaultMapping);
        }

        private void PumpIncomes()
        {
            if (this.DataSource.Year >= 2010)
                return;
            WriteToTrace("Старт закачки Блок \"Доходы\".", TraceMessageKind.Information);
            blockName = "Блок \"Доходы\"";
            block = Block.bIncomes;
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpSkif3Incomes();
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                        Pump2005Incomes();
                    else
                        Pump2004Incomes();
                    break;
            }
            UpdateData();
            ClearDataSet(daIncomes, ref dsIncomes);
            WriteToTrace("Закачка Блок \"Доходы\" закончена.", TraceMessageKind.Information);
        }

        #endregion Доходы

        #region расходы

        private void PumpSkif3Outcomes()
        {
            object[] ClsDefaultMapping = new object[] { "RefEKRFOProj", nullEKR, "REFKCSR", nullKCSR, "REFFKR", nullFKR, "REFKVR", nullKVR };

            if (this.DataSource.Year >= 2010)
            {
                forms = new string[] { "204ФК", "12", "204ФК", "22" };
                codeAttrName = "РзПр+ЦСР+ВР+ЭКР;РзПрЦСРВРПВРПСР";
                PumpXMLReportData(daOutcomes, dsOutcomes.Tables[0],
                    new DataTable[] { dsEKR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsKVR.Tables[0] },
                    new int[] { nullEKR, nullKCSR, nullFKR, nullKVR },
                    new Dictionary<string, int>[] { ekrCache, kcsrCache, fkrCache, kvrCache },
                    new string[] { "00000000000000000" }, ClsDefaultMapping);
                // качаем если нет 204ФК
                forms = new string[] { "201", "2", "207", "2" };
                codeAttrName = "РзПр";
                PumpXMLReportData(daOutcomes, dsOutcomes.Tables[0],
                    new DataTable[] { dsEKR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsKVR.Tables[0] },
                    new int[] { nullEKR, nullKCSR, nullFKR, nullKVR },
                    new Dictionary<string, int>[] { ekrCache, kcsrCache, fkrCache, kvrCache },
                    new string[] { "0000" }, ClsDefaultMapping);
            }
            else
            {
                forms = new string[] { "204", "3..13" };
                codeAttrName = "РзПрЦСРВРПВРПСР";
                PumpXMLReportData(daOutcomes, dsOutcomes.Tables[0],
                    new DataTable[] { dsEKR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsKVR.Tables[0] },
                    new int[] { nullEKR, nullKCSR, nullFKR, nullKVR },
                    new Dictionary<string, int>[] { ekrCache, kcsrCache, fkrCache, kvrCache },
                    new string[] { "79000000000000000" }, ClsDefaultMapping);
            }
            UpdateData();
        }

        private void Pump2004Outcomes()
        {
            codeAttrName = "ClsfCode";
            object[] ClsDefaultMapping = new object[] { "RefEKRFOProj", nullEKR, "REFKCSR", nullKCSR, "REFFKR", nullFKR, "REFKVR", nullKVR };
            forms = new string[] { "203", "1" };
            PumpXMLReportData(daOutcomes, dsOutcomes.Tables[0], 
                new DataTable[] { dsEKR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsKVR.Tables[0] },
                new int[] { nullEKR, nullKCSR, nullFKR, nullKVR }, 
                new Dictionary<string, int>[] { ekrCache, kcsrCache, fkrCache, kvrCache },
                new string[] { "7980000000000000000" }, ClsDefaultMapping);
            forms = new string[] { "204", "3..25" };
            PumpXMLReportData(daOutcomes, dsOutcomes.Tables[0],
                new DataTable[] { dsEKR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsKVR.Tables[0] },
                new int[] { nullEKR, nullKCSR, nullFKR, nullKVR }, 
                new Dictionary<string, int>[] { ekrCache, kcsrCache, fkrCache, kvrCache },
                new string[] { "7980000000000000000" }, ClsDefaultMapping);
        }

        private void Pump2005Outcomes()
        {
            forms = new string[] { "204", "3..13" };
            codeAttrName = "РзПр+ЦСР+ВР+ПСР";
            object[] ClsDefaultMapping = new object[] { "RefEKRFOProj", nullEKR, "REFKCSR", nullKCSR, "REFFKR", nullFKR, "REFKVR", nullKVR };
            PumpXMLReportData(daOutcomes, dsOutcomes.Tables[0],
                new DataTable[] { dsEKR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsKVR.Tables[0] },
                new int[] { nullEKR, nullKCSR, nullFKR, nullKVR }, 
                new Dictionary<string, int>[] { ekrCache, kcsrCache, fkrCache, kvrCache },
                new string[] { "7900*" }, ClsDefaultMapping);
        }

        private void PumpOutcomes()
        {
            WriteToTrace("Старт закачки Блок \"Расходы\".", TraceMessageKind.Information);
            blockName = "Блок \"Расходы\"";
            block = Block.bOutcomes;
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpSkif3Outcomes();
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                        Pump2005Outcomes();
                    else
                        Pump2004Outcomes();
                    break;
            }
            UpdateData();
            ClearDataSet(daOutcomes, ref dsOutcomes);
            WriteToTrace("Закачка Блок \"Расходы\" закончена.", TraceMessageKind.Information);
        }

        #endregion расходы

        #region дефицит профицит

        private void PumpSkif3DefProf()
        {
            forms = new string[] { "204", "13" };
            codeAttrName = "РзПрЦСРВРПВРПСР";
            object[] ClsDefaultMapping = new object[] { };
            PumpXMLReportData(daDefProf, dsDefProf.Tables[0], new DataTable[] { null }, new int[] { -1 }, new Dictionary<string, int>[] { null },
                new string[] { "!79000000000000000" }, ClsDefaultMapping);
        }

        private void Pump2004DefProf()
        {
            codeAttrName = "ClsfCode";
            object[] ClsDefaultMapping = new object[] { };
            forms = new string[] { "203", "1" };
            PumpXMLReportData(daDefProf, dsDefProf.Tables[0], new DataTable[] { null }, new int[] { -1 }, new Dictionary<string, int>[] { null },
                new string[] { "!7980000000000000000" }, ClsDefaultMapping);
            forms = new string[] { "204", "25" };
            PumpXMLReportData(daDefProf, dsDefProf.Tables[0], new DataTable[] { null }, new int[] { -1 }, new Dictionary<string, int>[] { null },
                new string[] { "!7980000000000000000" }, ClsDefaultMapping);
        }

        private void Pump2005DefProf()
        {
            forms = new string[] { "204", "13" };
            codeAttrName = "РзПр";
            object[] ClsDefaultMapping = new object[] { };
            PumpXMLReportData(daDefProf, dsDefProf.Tables[0], new DataTable[] { null }, new int[] { -1 }, new Dictionary<string, int>[] { null },
                new string[] { "!7900" }, ClsDefaultMapping);
        }

        private void PumpDefProf()
        {
            if (this.DataSource.Year >= 2010)
                return;
            WriteToTrace("Старт закачки Блок \"ДефицитПрофицит\".", TraceMessageKind.Information);
            blockName = "Блок \"ДефицитПрофицит\"";
            block = Block.bDefProf;
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpSkif3DefProf();
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                        Pump2005DefProf();
                    else
                        Pump2004DefProf();
                        break;
            }
            UpdateData();
            ClearDataSet(daDefProf, ref dsDefProf);
            WriteToTrace("Закачка Блок \"ДефицитПрофицит\" закончена.", TraceMessageKind.Information);
        }

        #endregion дефицит профицит

        #region источники финансирования

        private void PumpSkif3FinSources()
        {
            forms = new string[] { "204", "2" };
            codeAttrName = "АдмКИВФ;АдмКИВнФ";
            object[] ClsDefaultMapping = new object[] { "REFKIFFOPROJ2004", nullKIF2004, "REFKIF", nullKIF2005 };
            PumpXMLReportData(daFinSources, dsFinSources.Tables[0], new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
            new int[] { nullKIF2004, nullKIF2005 }, new Dictionary<string, int>[] { null, kifCache }, new string[] { }, ClsDefaultMapping);
        }

        private void Pump2005FinSources()
        {
            forms = new string[] { "204", "2" };
            codeAttrName = "АдмКИВФ;КИВФ;КИВнФ;ИФ";
            object[] ClsDefaultMapping = new object[] { "REFKIFFOPROJ2004", nullKIF2004, "REFKIF", nullKIF2005 };
            PumpXMLReportData(daFinSources, dsFinSources.Tables[0], new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
            new int[] { nullKIF2004, nullKIF2005 }, new Dictionary<string, int>[] { null, kifCache }, new string[] { }, ClsDefaultMapping);
        }

        private void Pump2004FinSources()
        {
            codeAttrName = "ClsfCode";
            object[] ClsDefaultMapping = new object[] { "REFKIFFOPROJ2004", nullKIF2004, "REFKIF", nullKIF2005 };
            forms = new string[] { "202", "1" };
            PumpXMLReportData(daFinSources, dsFinSources.Tables[0], new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
            new int[] { nullKIF2004, nullKIF2005 }, new Dictionary<string, int>[] { kifCache, null }, new string[] { }, ClsDefaultMapping);
            forms = new string[] { "204", "2" };
            PumpXMLReportData(daFinSources, dsFinSources.Tables[0], new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
            new int[] { nullKIF2004, nullKIF2005 }, new Dictionary<string, int>[] { kifCache, null }, new string[] { }, ClsDefaultMapping);
        }

        private void PumpFinSources()
        {
            if (this.DataSource.Year >= 2010)
                return;
            WriteToTrace("Старт закачки Блок \"Источники финансирования\".", TraceMessageKind.Information);
            blockName = "Блок \"Источники финансирования\"";
            block = Block.bFinSources;
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpSkif3FinSources();
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                        Pump2005FinSources();
                    else
                        Pump2004FinSources();
                    break;
            }
            UpdateData();
            ClearDataSet(daFinSources, ref dsFinSources);
            WriteToTrace("Закачка Блок \"Источники финансирования\" закончена.", TraceMessageKind.Information);
        }

        #endregion источники финансирования

        #region сети

        private void PumpSkif3Nets()
        {
            forms = new string[] { "201", string.Empty };
            if (this.DataSource.Year >= 2010)
                forms = new string[] { "201", "1", "207", "1" };
            codeAttrName = "РзПрЦСРВРСШ";
            object[] ClsDefaultMapping = new object[] { "REFFKR", nullFKR, "REFKCSR", nullKCSR, "REFKVR", nullKVR, "REFMARKS", nullSubKVR, "REFKSSHK", nullKSHK };
            PumpXMLReportData(daNets, dsNets.Tables[0], new DataTable[] { dsFKR.Tables[0], dsKCSR.Tables[0], dsKVR.Tables[0], dsSubKVR.Tables[0], dsKSHK.Tables[0] },
                new int[] { nullFKR, nullKCSR, nullKVR, nullSubKVR, nullKSHK },
                new Dictionary<string, int>[] { fkrCache, kcsrCache, kvrCache, subKVRCache, kshkCache },
                new string[] { }, ClsDefaultMapping);
        }

        private void Pump2004Nets()
        {
            forms = new string[] { "201", "1" };
            codeAttrName = "ClsfCode";
            object[] ClsDefaultMapping = new object[] { "REFFKR", nullFKR, "REFKCSR", nullKCSR, "REFKVR", nullKVR, "REFMARKS", nullSubKVR, "REFKSSHK", nullKSHK };
            PumpXMLReportData(daNets, dsNets.Tables[0], new DataTable[] { dsFKR.Tables[0], dsKCSR.Tables[0], dsKVR.Tables[0], dsSubKVR.Tables[0], dsKSHK.Tables[0] },
                new int[] { nullFKR, nullKCSR, nullKVR, nullSubKVR, nullKSHK },
                new Dictionary<string, int>[] { fkrCache, kcsrCache, kvrCache, subKVRCache, kshkCache },
                new string[] { }, ClsDefaultMapping);
        }

        private void Pump2005Nets()
        {
            forms = new string[] { "201", "1" };
            codeAttrName = "ClsfCode;РзПр+ЦСР+ВР+П_ВР_СШ+СШ";
            object[] ClsDefaultMapping = new object[] { "REFFKR", nullFKR, "REFKCSR", nullKCSR, "REFKVR", nullKVR, "REFMARKS", nullSubKVR, "REFKSSHK", nullKSHK };
            PumpXMLReportData(daNets, dsNets.Tables[0], new DataTable[] { dsFKR.Tables[0], dsKCSR.Tables[0], dsKVR.Tables[0], dsSubKVR.Tables[0], dsKSHK.Tables[0] },
                new int[] { nullFKR, nullKCSR, nullKVR, nullSubKVR, nullKSHK },
                new Dictionary<string, int>[] { fkrCache, kcsrCache, kvrCache, subKVRCache, kshkCache },
                new string[] { }, ClsDefaultMapping);
        }

        private void PumpNets()
        {
            // должен быть заполнен субквр
            if (subKVRCache.Count <= 1)
                return;
            WriteToTrace("Старт закачки Блок \"Сеть Штаты Контингент\".", TraceMessageKind.Information);
            blockName = "Блок \"Сеть Штаты Контингент\"";
            block = Block.bNets;
            switch (xmlFormat)
            {
                case XmlFormat.Skif3:
                    PumpSkif3Nets();
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                        Pump2005Nets();
                    else
                        Pump2004Nets();
                    break;
            }
            UpdateData();
            ClearDataSet(daNets, ref dsNets);
            WriteToTrace("Закачка Блок \"Сеть Штаты Контингент\" закончена.", TraceMessageKind.Information);
        }

        #endregion сети

        private void PumpXMLReport()
        {
            if (reportNode == null) 
                return;
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomes();
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomes();
            if (ToPumpBlock(Block.bDefProf))
                PumpDefProf();
            if (ToPumpBlock(Block.bFinSources))
                PumpFinSources();
            if (ToPumpBlock(Block.bNets))
                PumpNets();
        }

        #endregion Закачка блоков

        #endregion Функции закачки блоков

        #region Общая организация закачки

        private XmlDocument ConfigureXmlParams(FileInfo reportFile)
        {
            XmlDocument xdReport = new XmlDocument();
            xdReport.Load(reportFile.FullName);
            sumFactor = 1000;
            xmlFormat = XmlFormat.Skif3;
            if (this.DataSource.Year <= 2005)
            {
                if (xdReport.SelectSingleNode("/Otchet") != null)
                    xmlFormat = XmlFormat.Format2004;
                else if (xdReport.SelectSingleNode("/RootXml//Report") != null)
                    xmlFormat = XmlFormat.Format2005;
                else
                    throw new Exception("не найден элемент с данными отчета");
            }
            return xdReport;
        }

        private void PumpXMLPattern(FileInfo[] filesRepList)
        {
            bool patternIsPumped = false;
            // добавляем ссылку на "неуказанный код классификатора"  
            PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, "0", new object[] { "CODE", "0", "NAME", "Неуказанный код классификатора" });
            nullKCSR = FindCachedRow(kcsrCache, "0", nullKCSR);
            nullEKR = PumpRow(dsEKR.Tables[0], clsEKR, new object[] { "CODE", "0", "NAME", "Неуказанный код классификатора" });
            if (!ekrCache.ContainsKey("0"))
                ekrCache.Add("0", nullEKR);
            PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, "0", new object[] { "CODE", "0", "NAME", "Неуказанный код классификатора" });
            nullKVR = FindCachedRow(kvrCache, "0", nullKVR);
            nullSubKVR = PumpOriginalRow(clsSubKVR, dsSubKVR.Tables[0], new object[] { "CodeRprt", "0", "NAME", "Неуказанный код классификатора" }, null);
            // сначала ищем внутренний шаблон
            // внутренний шаблон может быть только в одном из файлов отчетов. Потому ищем во всех
            for (int i = 0; i < filesRepList.GetLength(0); i++)
            {
                patternNode = GetPatternNode(ConfigureXmlParams(filesRepList[i]));
                if (patternNode == null)
                    continue;
                WriteToTrace(string.Format("Старт обработки внутреннего шаблона {0}.", filesRepList[i].Name), TraceMessageKind.Information);
                PumpInternalXMLPattern();
                patternIsPumped = true;
                WriteToTrace(string.Format("Внутренний шаблон {0} обработан.", filesRepList[i].Name), TraceMessageKind.Information);
            }
            // обрабатываем внешний шаблон
            if (!patternIsPumped && patternDOM.DocumentElement != null)
            {
                WriteToTrace(string.Format("Старт обработки внешнего шаблона."), TraceMessageKind.Information);
                PumpExternalXMLPattern();
                patternIsPumped = true;
                WriteToTrace(string.Format("Внешний шаблон обработан."), TraceMessageKind.Information);
            }
            if (!patternIsPumped)
                throw new PumpDataFailedException("Ни в одном из файлов шаблон не найден.");
            UpdateData();
        }

        private void PumpXMLReports(FileInfo[] filesRepList)
        {
            // должен быть заполнен субквр
            if (subKVRCache.Count <= 1)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, "Не заполнен классификатор Показатели.ФО_Свод_СубКВР, форма 201 закачана не будет");
            bool noRegForPump = false;
            for (int i = 0; i < filesRepList.GetLength(0); i++)
            {
                filesCount++;
                progressMsg = string.Format("Обработка файла {0} ({1} из {2})...", filesRepList[i].Name, filesCount, xmlFilesCount);
                if (!File.Exists(filesRepList[i].FullName))
                    continue;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, string.Format("Старт закачки файла {0}.", filesRepList[i].FullName));
                try
                {
                    GetReportNode(ConfigureXmlParams(filesRepList[i]));
                    if (reportNode == null)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, "В файле отсутствуют данные для закачки.");
                        continue;
                    }
                    PumpRegionsXML(dsRegions.Tables[0], clsRegions, regionCache, ref noRegForPump);
                    PumpXMLReport();
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format("Файл {0} успешно закачан.", filesRepList[i].FullName));
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError,
                        string.Format("Закачка из файла {0} закончена с ошибками", filesRepList[i].FullName), ex);
                    continue;
                }
                finally
                {
                    CollectGarbage();
                }
            }
            if (noRegForPump)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Классификатор Районы.Служебный (SOURCEID {0}) имеет записи с неуказанным типом района. " +
                    "Необходимо установить значения поля \"ТипДокумента.СКИФ\" и запустить этап обработки.", regForPumpSourceID));
            UpdateData();
        }

        protected void PumpXMLReports(DirectoryInfo dir)
        {
            FileInfo[] filesList = dir.GetFiles("*.xml", SearchOption.AllDirectories);
            if (filesList.GetLength(0) == 0)
                return;
            patternDOM = new XmlDocument();
            GetPatternDOM(dir);
            FileInfo[] filesRepList;
            filesRepList = GetReportList(filesList);
            try
            {
                PumpXMLPattern(filesRepList);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref patternDOM);
            }
            PumpXMLReports(filesRepList);
        }

        #endregion Общая организация закачки отчетов XML

        #endregion Закачка данных

    }
}
