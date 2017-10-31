using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps
{

    /// <summary>
    /// Базовый класс для закачек отчетов СКИФ
    /// </summary>
    public abstract partial class SKIFRepPumpModuleBase : CorrectedPumpModuleBase
    {

        public int regForPumpSourceID;
        public int yearSourceID;
        public bool noRegForPump = false;
        public bool forcePumpForm127 = false;
        public double sumMultiplier = 1;

        protected Dictionary<int, string> kvrAuxCache = new Dictionary<int, string>();
        protected Dictionary<int, string> kcsrAuxCache = new Dictionary<int, string>();
        protected Dictionary<int, string> fkrAuxCache = new Dictionary<int, string>();

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
            /// Формат 2005 г. (до октября 2005)
            /// </summary>
            Format2005,

            /// <summary>
            /// Формат октябрь 2005+
            /// </summary>
            October2005,

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
            // МесОтч
            Form649,
            Form650,
            Form651,
            Form414,
            Form428,
            Form428v,
            Form128,
            Form128v,
            Form487,
            Form117,
            Form628r,
            Form127,
            Form127v,
            Form127g,
            Form159,
            Form169,
            Form159V,
            Form169V,
            Form469,
            Form459,
            Form469V,
            Form459V,
            // ГодОтч
            Form428g,
            Form428Vg,
            Form623,
            Form625,
            Form624,
            Form630,
            Form12001,
            Form13001,
            Form43001,
            Form12002,
            Form13002,
            Form43002,

            UnknownForm
        }

        #endregion Структуры, перечисления

        #region Общие функции XML

        /// <summary>
        /// Возвращает элемент с данными отчета файла хмл
        /// </summary>
        /// <param name="xdReport">хмл-документ отчета</param>
        /// <returns>Элемент с данными отчета</returns>
        protected XmlNode GetReportNode(XmlDocument xdReport)
        {
            XmlNode xnReport = xdReport.SelectSingleNode("/Otchet");

            if (xnReport == null)
            {
                xnReport = xdReport.SelectSingleNode("/RootXml/Report");
            }

            return xnReport;
        }

        /// <summary>
        /// Загружает хмл-файлы
        /// </summary>
        /// <param name="filesList">Список файлов</param>
        /// <param name="filesRepList">Список файлов отчетов</param>
        /// <param name="xdPattern">Загруженный шаблон</param>
        private void LoadXMLFiles(FileInfo[] filesList, out FileInfo[] filesRepList, out XmlDocument xdPattern)
        {
            filesRepList = new FileInfo[0];
            xdPattern = null;
            for (int i = 0; i < filesList.GetLength(0); i++)
            {
                if (!File.Exists(filesList[i].FullName))
                    continue;
                // пропускаем наши хмл с классификаторами (нужны в закачке ткст отчетов формата фк)
                if (filesList[i].Name.ToUpper().StartsWith("ПОКАЗАТЕЛИ_"))
                    continue;
                // Ищем шаблон
                if (filesList[i].Name.ToUpper().StartsWith(MonthPatternName.ToUpper()))
                {
                    if (xdPattern != null)
                        throw new PumpDataFailedException("В каталоге находится более одного шаблона.");
                    xmlFilesCount--;
                    // Если нашли - загружаем
                    xdPattern = new XmlDocument();
                    xdPattern.Load(filesList[i].FullName);
                }
                else
                {
                    // Формируем список файлов отчетов
                    filesRepList = (FileInfo[])CommonRoutines.RedimArray(filesRepList, filesRepList.GetLength(0) + 1);
                    filesRepList[filesRepList.GetLength(0) - 1] = filesList[i];
                }
            }
        }

        /// <summary>
        /// Проверяет правило исключения на инверсию - если впереди стоит !, то результат инвертируется
        /// </summary>
        /// <param name="codeExclusion">Правило исключения</param>
        /// <param name="result">Результат</param>
        private bool InverseExclusionResult(string codeExclusion, bool result)
        {
            if (codeExclusion.TrimStart('#').StartsWith("!"))
            {
                return !result;
            }

            return result;
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
                if (attrIndex < dataNode.Attributes.Count) result = dataNode.Attributes[attrIndex].Value;
            }
            else
            {
                for (int i = 0; i < dataNode.Attributes.Count; i++)
                {
                    result += dataNode.Attributes[i].Value;
                }
            }

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

        // Возвращает ИД источника на Год
        protected int GetYearSourceID()
        {
            return AddDataSource("ФО", "0002", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        /// <summary>
        /// Возвращает ИД источника для Районы.Служебный
        /// </summary>
        /// <returns>ИД источника для Районы.Служебный</returns>
        protected int GetRegions4PumpSourceID()
        {
            return AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        private bool PumpRegion4Pump(string code, string key, string name,
            DataTable regions4PumpTable, IClassifier regions4PumpCls, Dictionary<string, int> regions4PumpCache)
        {
            if (regions4PumpTable == null)
                return true;
            if (!regions4PumpCache.ContainsKey(key))
            {
                PumpCachedRow(regions4PumpCache, regions4PumpTable, regions4PumpCls, code, key, "CODESTR", "REFDOCTYPE",
                    new object[] { "NAME", name, "REFDOCTYPE", 1, "SOURCEID", regForPumpSourceID });
                return false;
            }
            return true;
        }

        protected bool PumpRegionsXML(XmlNode xn, DataTable regionsTable, IClassifier regionsCls,
            Dictionary<string, int> regionsCache, DataTable regions4PumpTable, IClassifier regions4PumpCls,
            Dictionary<string, int> regions4PumpCache)
        {
            XmlNodeList xnlSources = xn.SelectNodes("//Source");
            for (int i = 0; i < xnlSources.Count; i++)
            {
                string regionCode = GetAttrValueByName(xnlSources[i].Attributes, "ObjectNmb", "Code").PadLeft(10, '0');
                string regionName = GetAttrValueByName(xnlSources[i].Attributes, "Name", "ObjectName");
                string regionKey = regionCode + "|" + regionName;
                PumpCachedRow(regionsCache, regionsTable, regionsCls, regionCode, regionKey, "CODESTR", "ID",
                    new object[] { "NAME", ConvertClsName(GetAttrValueByName(xnlSources[i].Attributes, "ObjectName", "Name")),
                        "BUDGETKIND", GetAttrValueByName(xnlSources[i].Attributes, "ClassKey", "ClassCode"),
                        "BUDGETNAME", GetAttrValueByName(xnlSources[i].Attributes, "ClassName") });
                if (!PumpRegion4Pump(regionCode, regionKey, regionName, regions4PumpTable, regions4PumpCls, regions4PumpCache))
                    noRegForPump = true;
            }
            return true;
        }

        /// <summary>
        /// Преобразует форму хмл в строку
        /// </summary>
        /// <param name="xmlForm">Форма</param>
        private string XmlFormToString(XmlForm xmlForm)
        {
            switch (xmlForm)
            {
                case XmlForm.Form128: return "128";
                case XmlForm.Form128v: return "128v";
                case XmlForm.Form414: return "414";
                case XmlForm.Form428: return "428";
                case XmlForm.Form428g: return "428g";
                case XmlForm.Form428v: return "428v";
                case XmlForm.Form428Vg: return "428Vg";
                case XmlForm.Form649: return "649";
                case XmlForm.Form650: return "650";
                case XmlForm.Form651: return "651";
                case XmlForm.Form623: return "623";
                case XmlForm.Form625: return "625";
                case XmlForm.Form624: return "624";
                case XmlForm.Form630: return "630";
                case XmlForm.Form487: return "487";
                case XmlForm.Form117: return "117";
                case XmlForm.Form127: return "127";
                case XmlForm.Form127v: return "127v";
                case XmlForm.Form127g: return "127g";
                case XmlForm.Form159: return "159";
                case XmlForm.Form169: return "169";
                case XmlForm.Form159V: return "159V";
                case XmlForm.Form169V: return "169V";
                case XmlForm.Form469: return "469";
                case XmlForm.Form459: return "459";
                case XmlForm.Form469V: return "469V";
                case XmlForm.Form459V: return "459V";
                case XmlForm.Form12001: return "12001";
                case XmlForm.Form13001: return "13001";
                case XmlForm.Form43001: return "43001";
                case XmlForm.Form12002: return "12002";
                case XmlForm.Form13002: return "13002";
                case XmlForm.Form43002: return "43002";
                default: return string.Empty;
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
            {
                result += XmlFormToString(xmlForm[i]) + ", ";
            }

            return result.Trim().Trim(',');
        }

        /// <summary>
        /// Преобразует форму хмл в строку
        /// </summary>
        /// <param name="xmlForm">Форма</param>
        private XmlForm StringToXmlForm(string xmlForm)
        {
            switch (xmlForm.ToUpper())
            {
                case "128": return XmlForm.Form128;
                case "128V": return XmlForm.Form128v;
                case "414": return XmlForm.Form414;
                case "428": return XmlForm.Form428;
                case "428G": return XmlForm.Form428g;
                case "428V": return XmlForm.Form428v;
                case "428VG": return XmlForm.Form428Vg;
                case "649": return XmlForm.Form649;
                case "650": return XmlForm.Form650;
                case "651": return XmlForm.Form651;
                case "623": return XmlForm.Form623;
                case "625": return XmlForm.Form625;
                case "624": return XmlForm.Form624;
                case "630": return XmlForm.Form630;
                case "487": return XmlForm.Form487;
                case "117": return XmlForm.Form117;
                case "628R": return XmlForm.Form628r;
                case "127": return XmlForm.Form127;
                case "127V": return XmlForm.Form127v;
                case "127G": return XmlForm.Form127g;
                case "159": return XmlForm.Form159;
                case "169": return XmlForm.Form169;
                case "159V": return XmlForm.Form159V;
                case "169V": return XmlForm.Form169V;
                case "469": return XmlForm.Form469;
                case "459": return XmlForm.Form459;
                case "469V": return XmlForm.Form469V;
                case "459V": return XmlForm.Form459V;
                case "12001": return XmlForm.Form12001;
                case "13001": return XmlForm.Form13001;
                case "43001": return XmlForm.Form43001;
                case "12002": return XmlForm.Form12002;
                case "13002": return XmlForm.Form13002;
                case "43002": return XmlForm.Form43002;
                default: return XmlForm.UnknownForm;
            }
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
            {
                result[i] = StringToXmlForm(forms[i]);
            }

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
            {
                if (xac[i].Name.ToUpper() == attrName.ToUpper()) return i;
            }
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
            if (xn == null) return -1;
            return GetAttributeIndex(xn.Attributes, attrName);
        }

        /// <summary>
        /// Определяет форму отчета хмл
        /// </summary>
        /// <param name="formNo">Номер формы, указанный в хмл</param>
        /// <returns>Номер формы</returns>
        private XmlForm GetXmlForm(string formNo)
        {
            string str = formNo.ToUpper();
            switch (this.XmlReportFormat)
            {
                case XmlFormat.October2005:
                    if (CommonRoutines.TrimNumbers(str) == string.Empty)
                    {
                        if (str.StartsWith("414"))
                            return XmlForm.Form414;
                        else if (str.StartsWith("428"))
                            return XmlForm.Form428;
                    }
                    else
                        if (str.StartsWith("428V"))
                            return XmlForm.Form428v;
                    break;
                default:
                    if (CommonRoutines.TrimNumbers(str) == string.Empty)
                    {
                        if (str.StartsWith("649"))
                            return XmlForm.Form649;
                        else if (str.StartsWith("650"))
                            return XmlForm.Form650;
                        else if (str.StartsWith("651"))
                            return XmlForm.Form651;
                        else if (str.StartsWith("623"))
                            return XmlForm.Form623;
                        else if (str.StartsWith("625"))
                            return XmlForm.Form625;
                        else if (str.StartsWith("624"))
                            return XmlForm.Form624;
                        else if (str.StartsWith("630"))
                            return XmlForm.Form630;
                        else if (str.StartsWith("487"))
                            return XmlForm.Form487;
                    }
                    break;
            }
            return XmlForm.UnknownForm;
        }

        /// <summary>
        /// Преборазует наименование классификатора к стандартному виду
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


        #region Функции закачки внешнего шаблона XML

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
        private bool GetClsFieldsFromExternalPatternXML(XmlDocument xdPattern, XmlNode xnFormRow, int itemNameInd,
            int clsfIDInd, int clsfCodeInd, int indPagNoInd, int indRowNoInd, out string code, out string name,
            out int kl, out int kst)
        {
            code = string.Empty;
            name = string.Empty;
            kl = 0;
            kst = 0;

            string[] tableFields =
                xnFormRow.Attributes["Values"].Value.Split(new string[] { "," }, StringSplitOptions.None);

            // Теперь для каждого наименования получаем код и закачиваем
            XmlNode xnCls = xdPattern.SelectSingleNode(string.Format(
                "//Table[@Name = \"Skif_Classificator\" or @Name = \"MFClassificator\"]/RecordSet/" +
                "Row[starts-with(@Values, \"{0}\")]", tableFields[clsfIDInd]));
            if (xnCls == null)
            {
                return false;
            }

            string[] clsFields = xnCls.Attributes["Values"].Value.Split(',');

            code = clsFields[clsfCodeInd].Trim('\'');
            string[] nameField = xnFormRow.Attributes["Values"].Value.Split(new string[] { "'" }, StringSplitOptions.None);

            if (nameField.GetLength(0) >= 2)
            {
                name = nameField[1].Trim();
            }
            else
            {
                name = constDefaultClsName;
            }

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
        private void GetClsAttributesIndexes(XmlDocument xdPattern, ref int formIDInd, ref int sectNoInd,
            ref int rowsSetIDInd, ref int itemNameInd, ref int clsfIDInd, ref int clsfCodeInd, ref int indPagNoInd,
            ref int indRowNoInd)
        {
            // Получаем индексы нужных атрибутов
            formIDInd = GetTableAttributeIndex(xdPattern, "FormSection", "FormID");
            sectNoInd = GetTableAttributeIndex(xdPattern, "FormSection", "SectNo");
            rowsSetIDInd = GetTableAttributeIndex(xdPattern, "FormSection", "RowsSetID");
            itemNameInd = GetTableAttributeIndex(xdPattern, "FormRows", "ItemName");
            clsfIDInd = GetTableAttributeIndex(xdPattern, "FormRows", "ClsfID");
            indPagNoInd = GetTableAttributeIndex(xdPattern, "FormRows", "IndPagNo");
            indRowNoInd = GetTableAttributeIndex(xdPattern, "FormRows", "IndRowNo");
            clsfCodeInd = GetTableAttributeIndex(xdPattern, "MFClassificator", "ClsfCode");
            if (clsfCodeInd < 0)
            {
                clsfCodeInd = GetTableAttributeIndex(xdPattern, "Skif_Classificator", "ClsfCode");
            }
        }

        /// <summary>
        /// Создает ограничение для выборки данных с указанными значениями номера формы
        /// </summary>
        /// <param name="xmlForm">Массив форм</param>
        /// <returns>Ограничение</returns>
        private string GetConstrForExternalPattern(XmlForm[] xmlForm)
        {
            if (xmlForm == null) return string.Empty;

            string result = string.Empty;

            bool disable428v = false;
            Array.Sort(xmlForm);

            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                if (xmlForm[i] == XmlForm.Form428)
                {
                    disable428v = true;
                }

                if (xmlForm[i] == XmlForm.Form428v && disable428v) continue;

                result += string.Format("starts-with(@Values, \"{0}\") or ", XmlFormToString(xmlForm[i]));
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(result.Length - 4) + "]";
            }

            return result;
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
        protected void PumpClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, Dictionary<string, int> codesMapping)
        {
            int formIDInd = -1;
            int sectNoInd = -1;
            int rowsSetIDInd = -1;
            int itemNameInd = -1;
            int clsfIDInd = -1;
            int indPagNoInd = -1;
            int indRowNoInd = -1;
            int clsfCodeInd = -1;

            GetClsAttributesIndexes(xdPattern, ref formIDInd, ref sectNoInd, ref rowsSetIDInd, ref itemNameInd,
                ref clsfIDInd, ref clsfCodeInd, ref indPagNoInd, ref indRowNoInd);

            XmlNodeList xnlFormSectionRows = xdPattern.SelectNodes(string.Format(
                "//Table[@Name = \"FormSection\"]/RecordSet/Row{0}", GetConstrForExternalPattern(xmlForm)));
            if (xnlFormSectionRows.Count == 0) return;

            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);

            string clsCodeField = GetClsCodeField(cls);

            // Обходим все строки FormSection и закачиваем соответствующие классификаторы
            for (int i = 0; i < xnlFormSectionRows.Count; i++)
            {
                string[] tableFields = xnlFormSectionRows[i].Attributes["Values"].Value.Split(',');
                if (!CommonRoutines.CheckValueEntry(Convert.ToInt32(tableFields[sectNoInd]), sectNo)) continue;

                // Получаем элементы с наименованиями классификатора
                XmlNodeList xnlFormRows = xdPattern.SelectNodes(string.Format(
                    "//Table[@Name = \"FormRows\"]/RecordSet/Row[starts-with(@Values, \"{0}\")]",
                    tableFields[rowsSetIDInd]));

                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    string clsCode;
                    string clsName;
                    int kl;
                    int kst;

                    if (!GetClsFieldsFromExternalPatternXML(
                        xdPattern, xnlFormRows[j], itemNameInd, clsfIDInd, clsfCodeInd, indPagNoInd, indRowNoInd,
                            out clsCode, out clsName, out kl, out kst))
                    {
                        continue;
                    }

                    PumpCachedRow(codesMapping, dt, cls, clsCode, new object[] {
                        clsCodeField, clsCode, "NAME", ConvertClsName(clsName), "KL", kl, "KST", kst });

                    SetProgress(xnlFormRows.Count, j + 1, string.Format("Обработка шаблона. Данные {0}...", semantic),
                        string.Format("Строка {0} из {1}", j + 1, xnlFormRows.Count));
                }
            }

            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает классификатор из шаблона, хранимого в отдельном файле
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        /// <param name="xmlForm">Номер формы</param>
        /// <param name="sectNo">Номер секции. Формат строки:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в
        /// варианте 2)</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_строки_в_датасете для каждого классификатора</param>
        protected void PumpClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, string sectNo, DataTable dt,
            IClassifier cls, Dictionary<string, int> codesMapping)
        {
            PumpClsFromExternalPatternXML(xdPattern, xmlForm, CommonRoutines.ParseParamsString(sectNo), dt, cls, codesMapping);
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
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            if (xdPattern.SelectNodes("//Table[@Name = \"FormSection\"]").Count == 0)
            {
                throw new Exception("Ошибка при закачке шаблона: отсутствует или поврежден FormSection.");
            }

            int formIDInd = -1;
            int sectNoInd = -1;
            int rowsSetIDInd = -1;
            int itemNameInd = -1;
            int clsfIDInd = -1;
            int indPagNoInd = -1;
            int indRowNoInd = -1;
            int clsfCodeInd = -1;

            GetClsAttributesIndexes(xdPattern, ref formIDInd, ref sectNoInd, ref rowsSetIDInd, ref itemNameInd,
                ref clsfIDInd, ref clsfCodeInd, ref indPagNoInd, ref indRowNoInd);

            XmlNodeList xnlFormSectionRows = xdPattern.SelectNodes(string.Format(
                "//Table[@Name = \"FormSection\"]/RecordSet/Row{0}", GetConstrForExternalPattern(xmlForm)));
            if (xnlFormSectionRows.Count == 0) return;

            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);

            string clsCodeField = GetClsCodeField(cls);
            // Список закачанных номеров секций - для предотвращения их повторного закачивания
            List<int> pumpedSectNo = new List<int>(10);

            // Обходим все строки FormSection и закачиваем соответствующие классификаторы
            for (int i = 0; i < xnlFormSectionRows.Count; i++)
            {
                string[] tableFields = xnlFormSectionRows[i].Attributes["Values"].Value.Split(',');

                int currSectNo = Convert.ToInt32(tableFields[sectNoInd]);
                if (pumpedSectNo.Contains(currSectNo))
                {
                    continue;
                }
                else
                {
                    pumpedSectNo.Add(currSectNo);
                }

                if (!CommonRoutines.CheckValueEntry(currSectNo, sectNo)) continue;

                // Получаем элементы с наименованиями классификатора
                XmlNodeList xnlFormRows = xdPattern.SelectNodes(string.Format(
                    "//Table[@Name = \"FormRows\"]/RecordSet/Row[starts-with(@Values, \"{0}\")]",
                    tableFields[rowsSetIDInd]));

                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    string clsCode;
                    string clsName;
                    int kl;
                    int kst;

                    if (!GetClsFieldsFromExternalPatternXML(
                        xdPattern, xnlFormRows[j], itemNameInd, clsfIDInd, clsfCodeInd, indPagNoInd, indRowNoInd,
                            out clsCode, out clsName, out kl, out kst))
                    {
                        continue;
                    }

                    if (CheckCodeExclusion(clsCode, codeExclusions) ||
                        (indPagNo != null && !CheckCodeExclusion(kl, indPagNo)))
                    {
                        continue;
                    }

                    // Разбиваем код из хмл по полям классификатора
                    string[] fieldsMapping = GetFieldsValuesAsSubstring(attrValuesMapping, clsCode, "0");

                    if (clsProcessModifier == ClsProcessModifier.FKRBook)
                        fieldsMapping[1] = fieldsMapping[1].TrimStart('0').PadLeft(1, '0');

                    // Классификатор ЭКР формируется по значениям, где значения класификатора ФКР равны 0.
                    // Классификатор ФКР формируется по значениям, где значения класификатора ФКР не равны 0.
                    if ((clsProcessModifier == ClsProcessModifier.EKRBook &&
                        Convert.ToInt32(GetFieldsValuesAsSubstring(new string[] { "CODE", "0..3" }, clsCode, "0")[1]) != 0) ||
                        (clsProcessModifier == ClsProcessModifier.FKRBook && Convert.ToInt32(fieldsMapping[1]) == 0))
                    {
                        // Для ЭКР все равно запоминаем этот код в кэше
                        if (clsProcessModifier == ClsProcessModifier.EKRBook)
                        {
                            if (codesMapping != null)
                            {
                                int id = FindRowID(dt, fieldsMapping, -1);
                                if (id != -1)
                                {
                                    if (!codesMapping.ContainsKey(clsCode))
                                    {
                                        codesMapping.Add(clsCode, id);
                                    }
                                }
                            }
                        }

                        continue;
                    }

                    switch (clsProcessModifier)
                    {
                        case ClsProcessModifier.FKRBook:
                        case ClsProcessModifier.EKRBook:
                            clsCode = fieldsMapping[1];
                            break;
                        case ClsProcessModifier.MarksOutcomes:
                            clsCode = string.Concat(fieldsMapping[1].ToString(), fieldsMapping[3].ToString(), kst);
                            fieldsMapping = (string[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] { "LongCode", clsCode});
                            break;
                    }
                    // справ внутр и внешн долг
                    if ((fieldsMapping[0] == "SRCINFIN") || (fieldsMapping[0] == "SRCOUTFIN"))
                    {
                        clsCode = string.Concat(fieldsMapping[1].ToString(), fieldsMapping[3].ToString(), kst);
                        fieldsMapping = (string[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] { "LongCode", clsCode });
                    }
                    // справ задолженности
                    if ((fieldsMapping[0] == "FKR") && (clsProcessModifier == ClsProcessModifier.Special))
                    {
                        clsCode = string.Concat(fieldsMapping[1].ToString(), fieldsMapping[3].ToString(), kst);
                        fieldsMapping = (string[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] { "LongCode", clsCode });
                    }

                    // Закачиваем строку
                    PumpRowFromExternalPattern(dt, cls, useCodeMapping, codesMapping, clsCode, clsName, kl, kst,
                        fieldsMapping, codeMasks, clsCodeField, clsProcessModifier);

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
        /// <param name="sectNo">Номер секции. Формат строки:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в
        /// варианте 2)</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">Список пар поле-количество_символов.
        /// Поле - имя поля. Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
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
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, string sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForm, CommonRoutines.ParseParamsString(sectNo), dt,
                cls, attrValuesMapping, useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier,
                codeMasks);
        }

        /// <summary>
        /// Закачивает классификатор из шаблона, хранимого в отдельном файле.
        /// Позволяет задать правила формирования отдельных полей.
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        /// <param name="xmlForm">Номер формы</param>
        /// <param name="sectNo">Номер секции. Формат строки:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в
        /// варианте 2)</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">Список пар поле-количество_символов.
        /// Поле - имя поля. Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
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
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, string sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForm, sectNo, dt, cls, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
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
        /// -1..num - последние num символов)</param>
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
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForm, sectNo, dt, cls, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
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
        private void PumpRowFromExternalPattern(DataTable dt, IClassifier cls, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string clsCode, string clsName, int kl, int kst,
            string[] fieldsMapping, int[] codeMasks, string clsCodeField, ClsProcessModifier clsProcessModifier)
        {
            if (useCodeMapping)
            {
                // Определяем значение ключа кэша
                if (clsProcessModifier == ClsProcessModifier.CacheSubCode)
                {
                    PumpCachedRow(codesMapping, dt, cls, fieldsMapping[1], clsCodeField,
                        new object[] { "NAME", ConvertClsName(clsName), "KL", kl, "KST", kst });
                }
                else
                {
                    PumpCachedRow(codesMapping, dt, cls, clsCode,
                        (object[])CommonRoutines.ConcatArrays(fieldsMapping,
                            new string[] { "NAME", ConvertClsName(clsName), "KL", kl.ToString(), "KST", kst.ToString() }));
                }
            }
            else
            {
                if (codeMasks == null)
                {
                    PumpCachedRow(codesMapping, dt, cls, clsCode,
                        (object[])CommonRoutines.ConcatArrays(fieldsMapping,
                            new string[] { clsCodeField, clsCode, "NAME", ConvertClsName(clsName), "KL", kl.ToString(), "KST", kst.ToString() }));
                }
                else
                {
                    string newClsCode = BuildCodeBySubCodesMask(fieldsMapping, codeMasks);
                    PumpCachedRow(codesMapping, dt, cls, newClsCode,
                        (object[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] {
                            clsCodeField, newClsCode, "NAME", ConvertClsName(clsName), "KL", kl.ToString(), "KST", kst.ToString() }));
                }
            }
        }

        #endregion Функции закачки внешнего шаблона XML


        #region Функции закачки внутреннего шаблона XML

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
                case XmlForm.Form128v:
                    if (sectNo < 0)
                    {
                        return "@Code = \"128v\" or @Code = \"128V\" or ";
                    }
                    else
                    {
                        return string.Format("@Code = \"128{0:00}v\" or @Code = \"128{0:00}V\" or ", sectNo);
                    }

                case XmlForm.Form428g:
                    if (sectNo < 0)
                    {
                        return "@Code = \"428g\" or ";
                    }
                    else
                    {
                        return string.Format("@Code = \"428{0:00}g\" or ", sectNo);
                    }

                case XmlForm.Form428Vg:
                    if (sectNo < 0)
                    {
                        return "@Code = \"428Vg\" or ";
                    }
                    else
                    {
                        return string.Format("@Code = \"428{0:00}Vg\" or ", sectNo);
                    }

                case XmlForm.Form628r:
                    return string.Format("@Code = \"628{0:00}r\" or ", sectNo);

                case XmlForm.Form127g:
                    return string.Format("@Code = \"127g{0:00}\" or @Code = \"127G{0:00}\" or ", sectNo);
                case XmlForm.Form127v:
                    return string.Format("@Code = \"127{0:00}v\" or @Code = \"127{0:00}V\" or ", sectNo);

                default:
                    if (sectNo < 0)
                    {
                        return string.Format("@Code = \"{0}\" or ", XmlFormToString(xmlForm));
                    }
                    else
                    {
                        return string.Format("@Code = \"{0}{1:00}\" or ", XmlFormToString(xmlForm), sectNo);
                    }
            }
        }

        /// <summary>
        /// Создает ограничение для выборки данных с указанными значениями номера формы и секции
        /// </summary>
        /// <param name="xmlForm">Массив форм</param>
        /// <param name="sectNo">Массив номеров секций</param>
        /// <returns>Ограничение</returns>
        private string GetConstrForInternalPattern(XmlForm[] xmlForm, int[] sectNo)
        {
            if (xmlForm == null)
                return string.Empty;
            string result = string.Empty;
            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                if (sectNo != null && sectNo.GetLength(0) > 0)
                    for (int j = 0; j < sectNo.GetLength(0); j++)
                    {
                        result += GetReportCode(xmlForm[i], sectNo[j]);
                    }
                else
                    result += GetReportCode(xmlForm[i], -1);
            }
            if (result != string.Empty)
                result = "[" + result.Remove(result.Length - 4) + "]";
            return result;
        }

        /// <summary>
        /// Закачивает классификатор из шаблона, выгруженного вместе с отчетом
        /// </summary>
        /// <param name="xnPattern">Элемент формы</param>
        /// <param name="xmlForm">Номер формы</param>
        /// <param name="sectNo">Номер секции</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_строки_в_датасете для каждого классификатора</param>
        /// <param name="formHierarchy">Установить иерархию после закачки</param>
        protected void PumpClsFromInternalPatternXML(XmlNode xnPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, Dictionary<string, int> codesMapping)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);

            int totalNodes = xnPattern.SelectNodes(string.Format(
                "//FormTemplate{0}/FormRows/Rows/Row", GetConstrForInternalPattern(xmlForm, sectNo))).Count;
            if (totalNodes == 0)
            {
                WriteToTrace(string.Format("Нет данных по классификатору {0}.", semantic), TraceMessageKind.Information);
                return;
            }
            int nodesCount = 0;

            // Получаем элементы с кодами и наименованиями классификатора
            XmlNodeList xnlFormTemplates = xnPattern.SelectNodes(string.Format(
                "//FormTemplate{0}/FormRows/Rows/Row", GetConstrForInternalPattern(xmlForm, sectNo)));

            string clsCodeField = GetClsCodeField(cls);
            // Список закачанных номеров секций - для предотвращения их повторного закачивания
            List<int> pumpedSectNo = new List<int>(10);

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
                {
                    pumpedSectNo.Add(currSectNo);
                }

                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    nodesCount++;
                    SetProgress(totalNodes, nodesCount, string.Format("Обработка шаблона. Данные {0}...", semantic),
                        string.Format("Запись {0} из {1}", nodesCount, totalNodes));

                    // Получаем код
                    string clsCode = GetClsfCode(xnlFormRows[j], -1);
                    if (clsCode == string.Empty)
                    {
                        continue;
                    }

                    XmlNode tmpNode = xnlFormRows[j].SelectSingleNode("./RowP");
                    PumpCachedRow(codesMapping, dt, cls, clsCode, new object[] {
                        clsCodeField, clsCode,
                        "NAME", ConvertClsName(tmpNode.Attributes["Name"].Value),
                        "KL", tmpNode.Attributes["Page"].Value,
                        "KST", tmpNode.Attributes["Row"].Value });

                }
            }

            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
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
            {
                result += codeValues[i + 1].PadLeft(codeMasks[i / 2], '0');
            }

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
                {
                    codesMapping.Add(clsCode, id);
                }
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
        private void PumpRowFromInternalPattern(DataTable dt, IClassifier cls, bool useCodeMapping,
            Dictionary<string, int> codesMapping, ClsProcessModifier clsProcessModifier, int[] codeMasks,
            string clsCodeField, string clsCode, string[] codeValues, XmlNode xn, int kl)
        {
            string name = ConvertClsName(xn.Attributes["Name"].Value);
            string kst = xn.Attributes["Row"].Value;
            if (this.DataSource.Year >= 2010)
            {
                switch (clsProcessModifier)
                {
                    case ClsProcessModifier.MarksOutcomes:
                    case ClsProcessModifier.Arrears:
                    case ClsProcessModifier.Excess:
                        kst = clsCode.Substring(0, 5);
                        break;
                }
            }

            // ваще непонятный кусок кода - просто ибануццо
            //if (clsProcessModifier == ClsProcessModifier.EKRBook && (kl == 11 || kl == 12))
            //    name = string.Empty;

            // букавки в коде приводим к английской
            clsCode = clsCode.Replace('А', 'A');
            clsCode = clsCode.Replace('а', 'a');
            codeValues[1] = codeValues[1].Replace('А', 'A');
            codeValues[1] = codeValues[1].Replace('а', 'a');

            if (!useCodeMapping)
            {
                if (codeMasks == null)
                {
                    PumpCachedRow(codesMapping, dt, cls, clsCode,
                        (object[])CommonRoutines.ConcatArrays(codeValues, new string[] {
                            clsCodeField, clsCode, "NAME", name, "KL", kl.ToString(), "KST", kst }));
                }
                else
                {
                    string newClsCode = BuildCodeBySubCodesMask(codeValues, codeMasks);
                    PumpCachedRow(codesMapping, dt, cls, newClsCode,
                        (object[])CommonRoutines.ConcatArrays(codeValues, new string[] {
                            clsCodeField, newClsCode, "NAME", name, "KL", kl.ToString(), "KST", kst }));
                }
            }
            else
            {
                // Определяем значение ключа кэша
                switch (clsProcessModifier)
                {
                    case ClsProcessModifier.CacheSubCode:
                    case ClsProcessModifier.EKR:
                    case ClsProcessModifier.EKRBook:
                    case ClsProcessModifier.FKR:
                    case ClsProcessModifier.FKRBook:
                        PumpCachedRow(codesMapping, dt, cls, codeValues[1], clsCodeField, new object[] {
                            "NAME", name, "KL", kl, "KST", kst });
                        break;

                    default:
                        PumpCachedRow(codesMapping, dt, cls, clsCode,
                            (object[])CommonRoutines.ConcatArrays(codeValues, new string[] {
                                "NAME", name, "KL", kl.ToString(), "KST", kst }));
                        break;
                }
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
        private void PumpComplexClsFromInternalPatternXML(XmlNode xnPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, string[] attr2FieldMapping, string[] attrValuesMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);
            string constr = GetConstrForInternalPattern(xmlForm, sectNo);
            int totalNodes = xnPattern.SelectNodes(string.Format("//FormTemplate{0}/FormRows/Rows/Row", constr)).Count;
            if (totalNodes == 0)
            {
                WriteToTrace(string.Format("Нет данных по классификатору {0}.", semantic), TraceMessageKind.Information);
                return;
            }
            int nodesCount = 0;
            // Получаем элементы с кодами и наименованиями классификатора
            XmlNodeList xnlFormTemplates = xnPattern.SelectNodes(string.Format("//FormTemplate{0}", constr));
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
                bool ToPumpArrearsRow = false;
                bool ToPumpOutcomesBooksRow = true;
                bool ToPumpInDebtBooksRow = false;
                bool ToPumpOutDebtBooksRow = false;
                bool toPumpExcessBooksRow = false;

                if (this.DataSource.Year >= 2010)
                    ToPumpOutcomesBooksRow = false;

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
                        case ClsProcessModifier.SrcOutFin:
                            // В классификатор источников внешнего финансирования КИВнФ.МесОтч_2005 должны попадать
                            // данные по заполненному полю АдмКИВнФ.
                            // В классификатор источников внутреннего финансирования КИВФ.МесОтч_2005 должны попадать
                            // данные по заполненному полю АдмКИВФ.
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
                    // у кд добиваем код до 20
                    if (((clsProcessModifier == ClsProcessModifier.Standard) ||
                         (clsProcessModifier == ClsProcessModifier.SrcInFin) ||
                         (clsProcessModifier == ClsProcessModifier.SrcOutFin)) &&
                        (this.DataSource.Year >= 2005))
                    {
                        codeValues[1] = codeValues[1].ToString().PadLeft(20, '0');
                        clsCode = clsCode.PadLeft(20, '0');
                    }

                    // у фкр добиваем код до 14
                    if (clsProcessModifier == ClsProcessModifier.FKR)
                    {
                        codeValues[1] = codeValues[1].ToString() + "0000000000";
                    }

                    int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
                    if (this.DataSource.Year >= 2008)
                    {
                        decimal clsCodeInt = 0;
                        if (this.DataSource.Year <= 2010)
                        {
                            if ((clsProcessModifier == ClsProcessModifier.Arrears) || (clsProcessModifier == ClsProcessModifier.MarksInDebt) ||
                                (clsProcessModifier == ClsProcessModifier.MarksOutDebt) || (clsProcessModifier == ClsProcessModifier.MarksOutcomes) ||
                                (clsProcessModifier == ClsProcessModifier.Excess))
                                clsCodeInt = Convert.ToDecimal(clsCode.TrimStart('0').PadLeft(1, '0'));
                        }
                        switch (clsProcessModifier)
                        {
                            case ClsProcessModifier.Arrears:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 года - [1090000000000000000000000, 1210000000000000000000225)
                                    if (clsCode == "1090000000000000000000000")
                                        ToPumpArrearsRow = true;
                                    if (clsCode == "1210000000000000000000225")
                                        ToPumpArrearsRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 года - [1000000000000000000000000, 1130000000000000000000225)
                                    if (clsCode == "1000000000000000000000000")
                                        ToPumpArrearsRow = true;
                                    if (clsCode == "1130000000000000000000225")
                                        ToPumpArrearsRow = false;
                                }
                                else
                                {
                                    if (currSectNo == 2)
                                    {
                                        // с 2009 года - Показатели.МесОтч_СпрЗадолженность качаем после 0000000000000000000010100 (включая)
                                        if (this.DataSource.Year >= 2009)
                                        {
                                            if (clsCode == "0000000000000000000010100")
                                                ToPumpArrearsRow = true;
                                            // c апреля 2009 года - Показатели.МесОтч_СпрЗадолженность  [0000000000000000000010100, 0000000000000000022511400)
                                            if (sourceDate >= 200904)
                                            {
                                                if (clsCode == "0000000000000000022511400")
                                                    ToPumpArrearsRow = false;
                                            }
                                        }
                                        // с 2008 года - Показатели.МесОтч_СпрЗадолженность качаем после 000000000000000000008600 (включая)
                                        else if (clsCode == "000000000000000000008600")
                                            ToPumpArrearsRow = true;
                                    }
                                }
                                if (!ToPumpArrearsRow)
                                    continue;
                                break;
                            case ClsProcessModifier.MarksInDebt:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 года -  [1050000000000000000000000, 1060000000000000000000000)
                                    // [1070000000000000000000000, 1080000000000000000000000)  + 1240000000000000000000000
                                    if ((clsCode == "1050000000000000000000000") || (clsCode == "1070000000000000000000000"))
                                        ToPumpInDebtBooksRow = true;
                                    if ((clsCode == "1060000000000000000000000") || (clsCode == "1080000000000000000000000") || (clsCode == "1300000000000000000000000"))
                                        ToPumpInDebtBooksRow = false;
                                    if (clsCode == "1240000000000000000000000")
                                        ToPumpInDebtBooksRow = true;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 года -  [0960000000000000000000000, 0970000000000000000000000)
                                    // [0980000000000000000000000, 0990000000000000000000000)  + 1160000000000000000000000
                                    if ((clsCode == "0960000000000000000000000") || (clsCode == "0980000000000000000000000"))
                                        ToPumpInDebtBooksRow = true;
                                    if ((clsCode == "0970000000000000000000000") || (clsCode == "0990000000000000000000000"))
                                        ToPumpInDebtBooksRow = false;
                                    if (clsCode == "1160000000000000000000000")
                                        ToPumpInDebtBooksRow = true;
                                }
                                else if (this.DataSource.Year >= 2009)
                                {
                                    // c 2009 года Показатели.МесОтч_СпрВнутрДолг качаем [0000000000000000000009700,0000000000000000000009800)
                                    // [0000000000000000000009900, 0000000000000000000010000)
                                    ToPumpInDebtBooksRow = (((clsCodeInt >= 9700) && (clsCodeInt < 9800)) ||
                                                            ((clsCodeInt >= 9900) && (clsCodeInt < 10000)));
                                }
                                else
                                {
                                    // c 2008 года Показатели.МесОтч_СпрВнутрДолг качаем [000000000000000000008200,000000000000000000008300)
                                    // [000000000000000000008400, 000000000000000000008500)
                                    ToPumpInDebtBooksRow = (((clsCodeInt >= 8200) && (clsCodeInt < 8300)) ||
                                                            ((clsCodeInt >= 8400) && (clsCodeInt < 8500)));
                                }
                                if (!ToPumpInDebtBooksRow)
                                    continue;
                                break;
                            case ClsProcessModifier.MarksOutDebt:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 года -  [1060000000000000000000000, 1070000000000000000000000)
                                    if (clsCode == "1060000000000000000000000")
                                        ToPumpOutDebtBooksRow = true;
                                    if (clsCode == "1070000000000000000000000")
                                        ToPumpOutDebtBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 года -  [0970000000000000000000000, 0980000000000000000000000)
                                    if (clsCode == "0970000000000000000000000")
                                        ToPumpOutDebtBooksRow = true;
                                    if (clsCode == "0980000000000000000000000")
                                        ToPumpOutDebtBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 год - Показатели.МесОтч_СпрВнешнДолг качаем [0000000000000000000009800, 0000000000000000000009900)
                                    ToPumpOutDebtBooksRow = ((clsCodeInt >= 9800) && (clsCodeInt < 9900));
                                }
                                else
                                {
                                    // 2008 год - Показатели.МесОтч_СпрВнешнДолг качаем [000000000000000000008300, 000000000000000000008400)
                                    ToPumpOutDebtBooksRow = ((clsCodeInt >= 8300) && (clsCodeInt < 8400));
                                }
                                if (!ToPumpOutDebtBooksRow)
                                    continue;
                                break;
                            case ClsProcessModifier.MarksOutcomes:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2012)
                                {
                                    // c 2012 года - Показатели.МесОтч_СпрРасходы
                                    // [0010000000000000000000000, 1050000000000000000000000)
                                    // [1210000000000000000000225, 1240000000000000000000000)
                                    // [1300000000000000000000225, 1499900000000000000000000)
                                    if ((clsCode == "0010000000000000000000000") || (clsCode == "1210000000000000000000225") || (clsCode == "1300000000000000000000000"))
                                        ToPumpOutcomesBooksRow = true;
                                    if ((clsCode == "1050000000000000000000000") || (clsCode == "1240000000000000000000000") || (clsCode == "1499900000000000000000000"))
                                        ToPumpOutcomesBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 года - Показатели.МесОтч_СпрРасходы
                                    // [0010000000000000000000000, 1050000000000000000000000)
                                    // [1210000000000000000000225, 1240000000000000000000000)
                                    // [1300000000000000000000225, 1360200000000000000000000)
                                    if ((clsCode == "0010000000000000000000000") || (clsCode == "1210000000000000000000225") || (clsCode == "1300000000000000000000000"))
                                        ToPumpOutcomesBooksRow = true;
                                    if ((clsCode == "1050000000000000000000000") || (clsCode == "1240000000000000000000000") || (clsCode == "1360200000000000000000000"))
                                        ToPumpOutcomesBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 года - Показатели.МесОтч_СпрРасходы [0010000000000000000000000, 0960000000000000000000000)
                                    // [1130000000000000000000225, 1160000000000000000000000)
                                    if ((clsCode == "0010000000000000000000000") || (clsCode == "1130000000000000000000225"))
                                        ToPumpOutcomesBooksRow = true;
                                    if ((clsCode == "0960000000000000000000000") || (clsCode == "1160000000000000000000000"))
                                        ToPumpOutcomesBooksRow = false;
                                }
                                else if (currSectNo == 2)
                                {
                                    // c 2009 года - Показатели.МесОтч_СпрРасходы из 2 секции до СПРТАБСтр = 0000000000000000000009700 (не включаем)
                                    if (this.DataSource.Year >= 2009)
                                    {
                                        if (clsCode == "0000000000000000000009700")
                                            ToPumpOutcomesBooksRow = false;
                                        // c апреля 2009 года - Показатели.МесОтч_СпрРасходы  (<;10100, 0000000000000000022511400;>]
                                        if (sourceDate >= 200904)
                                        {
                                            if (clsCode == "0000000000000000022511400")
                                                ToPumpOutcomesBooksRow = true;
                                        }
                                    }
                                    // c 2008 года - Показатели.МесОтч_СпрРасходы из 2 секции до СПРТАБСтрока = 000000000000000000008200 (не включая)
                                    else
                                    {
                                        if (clsCode == "000000000000000000008200")
                                            ToPumpOutcomesBooksRow = false;
                                    }
                                }
                                if (!ToPumpOutcomesBooksRow)
                                    continue;
                                break;
                            case ClsProcessModifier.Excess:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // 2011 - [1080000000000000000000000, 1090000000000000000000000)
                                    if (clsCode == "1080000000000000000000000")
                                        toPumpExcessBooksRow = true;
                                    if (clsCode == "1090000000000000000000000")
                                        toPumpExcessBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // 2010 - [0990000000000000000000000, 1000000000000000000000000) 
                                    if (clsCode == "0990000000000000000000000")
                                        toPumpExcessBooksRow = true;
                                    if (clsCode == "1000000000000000000000000")
                                        toPumpExcessBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2009)
                                {
                                    // 2008 - Показатели.МесОтч_СпрОстатки качаем [0000000000000000000010000, 0000000000000000000010100) 
                                    toPumpExcessBooksRow = ((clsCodeInt >= 10000) && (clsCodeInt < 10100));
                                }
                                else
                                {
                                    // 2008 - Показатели.МесОтч_СпрОстатки качаем [000000000000000000008500, 000000000000000000008600) 
                                    toPumpExcessBooksRow = ((clsCodeInt >= 8500) && (clsCodeInt < 8600));
                                }
                                if (!toPumpExcessBooksRow)
                                    continue;
                                break;
                        }
                    }
                    else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                    {
                        switch (clsProcessModifier)
                        {
                            case ClsProcessModifier.Arrears:
                                ToCheckKL = false;
                                // Показатели.МесОтч_СпрЗадолженность качаем после строки РзПрПСРЭКРСтрока = 000000000000000000007000 (включая)
                                if (currSectNo == 4)
                                {
                                    if (clsCode == "000000000000000000007000")
                                        ToPumpArrearsRow = true;
                                    if (!ToPumpArrearsRow)
                                        continue;
                                }
                                break;
                            case ClsProcessModifier.MarksOutcomes:
                                ToCheckKL = false;
                                // Показатели.МесОтч_СпрРасходы качаем из 1 секции все а из 4 - до строки РзПрПСРЭКРСтрока = 000000000000000000007000 (не включая)
                                if (currSectNo == 4)
                                {
                                    if (clsCode == "000000000000000000007000")
                                        ToPumpOutcomesBooksRow = false;
                                    if (!ToPumpOutcomesBooksRow)
                                        continue;
                                }
                                break;
                        }
                    }
                    XmlNode tmpNode = xnlFormRows[j].SelectSingleNode("./RowP");
                    int kl = Convert.ToInt32(tmpNode.Attributes["Page"].Value);
                    string kst = tmpNode.Attributes["Row"].Value.PadLeft(4, '0');
                    // с 2009 года kst нужно доводить до 5 символов
                    if (this.DataSource.Year >= 2009)
                        kst = kst.PadLeft(5, '0');
                    switch (clsProcessModifier)
                    {
                        case ClsProcessModifier.MarksOutDebt:
                            if (this.DataSource.Year >= 2010)
                            {
                                kst = clsCode.Substring(0, 5);
                                codeValues[1] = string.Empty.PadLeft(11, '0');
                                codeValues[3] = "000000";
                                // Код =  Строка (KST) + Источник внутреннего финансирования + ГосВнутрДолг
                                clsCode = kst + codeValues[1] + codeValues[3];
                            } 
                            else if (this.DataSource.Year == 2004)
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                            {
                                codeValues[1] = string.Empty.PadLeft(20, '0');
                                codeValues[3] = "000000";
                                // Код = Источник внешнего финансирования+ГосВнешДолг+Строка (KST)
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            }
                            break;
                        case ClsProcessModifier.MarksInDebt:
                            if (this.DataSource.Year >= 2010)
                            {
                                kst = clsCode.Substring(0, 5);
                                codeValues[1] = string.Empty.PadLeft(14, '0');
                                codeValues[3] = "000";
                                // Код =  Строка (KST) + Источник внутреннего финансирования + ГосВнутрДолг
                                clsCode = kst + codeValues[1] + codeValues[3];
                            }
                            else if (this.DataSource.Year == 2004)
                            {
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            }
                            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                            {
                                codeValues[1] = string.Empty.PadLeft(20, '0');
                                codeValues[3] = "000";
                                // Код = Источник внутреннего финансирования + ГосВнутрДолг + Строка (KST)
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            }
                            break;
                        case ClsProcessModifier.MarksOutcomes:
                        case ClsProcessModifier.Arrears:
                        case ClsProcessModifier.Excess:
                            if (this.DataSource.Year >= 2010)
                            {
                                kst = clsCode.Substring(0, 5);
                                // Код = KST+ФКР+КЦСР+КВР+ЭКР
                                clsCode = kst + codeValues[1] + codeValues[5] + codeValues[7] + codeValues[3];
                            }
                            else
                            {
                                if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)

                                    // Код = ФКР+КЦСР+КВР+ЭКР+KST
                                    clsCode = codeValues[1] + codeValues[5] + codeValues[7] + codeValues[3] + kst;
                                else
                                    // Код = ФКР+ЭКР+Kl
                                    clsCode = codeValues[1] + codeValues[3] + kl.ToString().PadLeft(3, '0');
                            }
                            break;
                    }

                    // Проверяем вхождение кода в список исключений
                    if (clsCode == string.Empty || CheckCodeExclusion(clsCode, codeExclusions))
                        continue;
                    // Проверяем вхождение kl в список исключений
                    // не проверяем по KL для Показатели.МесОтч_СпрРасходы начиная с февраля 2007 
                    if ((ToCheckKL) && (indPagNo != null))
                        if (!CheckCodeExclusion(kl, indPagNo))
                            continue;
                    PumpRowFromInternalPattern(dt, cls, useCodeMapping, codesMapping, clsProcessModifier, 
                        codeMasks, clsCodeField, clsCode, codeValues, tmpNode, kl);
                }
            }
            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
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
        /// <param name="useCodeMapping">Если в списке полей указано поле кода, то его значение раскладывается по 
        /// указанному правилу, иначе берется найденное в шаблоне</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Список исключений кода классификатора (указанные коды не закачиваются).
        /// Формат элементов списка: "code" - исключаются коды, равные указанному;
        /// "code*" - исключаются коды, начинающиеся с указанного;
        /// "code1;code2" - исключаются коды, входящие в диапазон code1..code2;
        /// "code;" - исключаются коды >= code;
        /// ";code" - исключаются коды, меньшие или равные code;
        /// "*code* - исключаются коды, содержащие code"</param>
        /// <param name="indPagNo">Массив значений IndPagNo - ограничение для закачки. Описание см. в codeExclusions</param>
        /// <param name="clsProcessModifier">Модификатор обработки классификатора. Нужен для сообщения функции обработки какой классификатор 
        /// обрабатывается</param>
        protected void PumpComplexClsFromInternalPatternXML(XmlNode xnPattern, XmlForm[] xmlForm, int[] sectNo,
            DataTable dt, IClassifier cls, string[] attr2FieldMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sectNo, dt, cls, attr2FieldMapping, null,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
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
        /// <param name="clsProcessModifier">Модификатор обработки классификатора. Нужен для сообщения функции обработки какой классификатор 
        /// обрабатывается</param>
        protected void PumpComplexClsFromInternalPatternXML(DataTable dt, XmlNode xnPattern, XmlForm[] xmlForm,
            int[] sectNo, IClassifier cls, string[] attrValuesMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sectNo, dt, cls, null, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
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
        /// <param name="clsProcessModifier">Модификатор обработки классификатора. Нужен для сообщения функции обработки какой классификатор 
        /// обрабатывается</param>
        protected void PumpComplexClsFromInternalPatternXML(DataTable dt, XmlNode xnPattern, XmlForm[] xmlForm,
            string sectNo, IClassifier cls, string[] attrValuesMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, CommonRoutines.ParseParamsString(sectNo), dt,
                cls, null, attrValuesMapping, useCodeMapping, codesMapping, codeExclusions, indPagNo,
                clsProcessModifier, null);
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
        /// <param name="attrValuesMapping">Список пар поле-количество_символов.
        /// Пример: "field1", 3, "field2", 2. Это значит, что первые 3 символа будут записаны в field1, 
        /// следующие 2 - в field2. Если поле = null, то символы пропускаются, если
        /// количество_символов = -1, то берутся все символы до конца строки если это последняя пара,
        /// иначе значения последующих пар будут браться с конца</param>
        /// <param name="useCodeMapping">Если в списке полей указано поле кода, то его значение раскладывается по 
        /// указанному правилу, иначе берется найденное в шаблоне</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Список исключений (описание см. в PumpXMLReportBlock)</param>
        /// <param name="indPagNo">Массив значений IndPagNo - ограничение для закачки. Описание см. в codeExclusions</param>
        /// <param name="clsProcessModifier">Модификатор обработки классификатора. Нужен для сообщения функции обработки какой классификатор 
        /// обрабатывается</param>
        /// <param name="codeMasks">Массив масок подкодов кода классификатора. Если указан, то код классификатора
        /// будет собран из подкодов, полученных из attr2FieldMapping или attrValuesMapping, дополненных до
        /// указанной маски (useCodeMapping false)</param>
        protected void PumpComplexClsFromInternalPatternXML(DataTable dt, XmlNode xnPattern, XmlForm[] xmlForm, 
            int[] sectNo, IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, 
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo, 
            ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sectNo, dt, cls, null, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, codeMasks);
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

            int count = fieldMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                if (xmlNode.Attributes["Code"].Value == fieldMapping[i])
                {
                    if (result == null)
                    {
                        result = new string[2];
                    }
                    else
                    {
                        Array.Resize(ref result, result.GetLength(0) + 2);
                    }

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
        protected void PumpClsFromInternalNSIPatternXML(XmlNode xnPattern, DataTable dt, IClassifier cls,
            string[] attrNames, string[] attr2FieldMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, bool skipCodeWithLetters)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("Закачка данных классификатора {0}...", semantic), TraceMessageKind.Information);

            // Получаем элементы с кодами и наименованиями классификатора
            XmlNodeList xnlNSIData = xnPattern.SelectNodes(string.Format(
                "//NSI/Catalogs/Catalog{0}", GetXPathConstrByAttr("Code", attrNames)));

            for (int i = 0; i < xnlNSIData.Count; i++)
            {
                string[] fieldMapping = GetFieldMappingForNSICatalog(xnlNSIData[i], attr2FieldMapping);
                if (fieldMapping == null) continue;

                string[] exclusions = GetFieldMappingForNSICatalog(xnlNSIData[i], codeExclusions);

                XmlNodeList xnlCatalogItems = xnlNSIData[i].SelectNodes("./CatalogItem");
                for (int j = 0; j < xnlCatalogItems.Count; j++)
                {
                    string code = xnlCatalogItems[j].Attributes["Code"].Value;
                    if ((skipCodeWithLetters) && (CommonRoutines.TrimNumbers(code) != string.Empty))
                        continue;
                    if (this.SkifReportFormat == SKIFFormat.MonthReports)
                        if (CheckCodeExclusion(code, codeExclusions))
                            continue;
                    if (exclusions != null)
                    {
                        if (CheckCodeExclusion(code, exclusions))
                        {
                            continue;
                        }
                    }

                    string[] fieldValues = GetFieldsValuesAsSubstring(fieldMapping, code, "0");
                    if (fieldValues == null) continue;

                    // для фкр к коду прибавляем 10 нулей
                    if (cls.ObjectKey == "0299a09f-9d23-4e6c-b39a-930cbe219c3a")
                        fieldValues[1] += "0000000000";

                    PumpCachedRow(codesMapping, dt, cls, fieldValues[1], fieldValues[0], new object[] { 
                        "NAME", ConvertClsName(xnlCatalogItems[j].Attributes["Name"].Value), "KL", "-1", "KST", "-1" });
                }
            }

            WriteToTrace(string.Format("Данные классификатора {0} закачаны.", semantic), TraceMessageKind.Information);
        }

        protected void PumpClsFromInternalNSIPatternXML(XmlNode xnPattern, DataTable dt, IClassifier cls,
            string[] attrNames, string[] attr2FieldMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions)
        {
            PumpClsFromInternalNSIPatternXML(xnPattern, dt, cls, attrNames, attr2FieldMapping, codesMapping, codeExclusions, true);
        }

        /// <summary>
        /// Возвращает значение атрибута по имени из списка
        /// </summary>
        /// <param name="xac">Коллекция атрибутов</param>
        /// <param name="attrName">Список имен атрибута</param>
        /// <returns>Значение атрибута</returns>
        protected string GetAttrValueByName(XmlAttributeCollection xac, params string[] attrName)
        {
            int count = attrName.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                XmlNode xn = xac.GetNamedItem(attrName[i]);
                if (xn != null)
                {
                    return xn.Value;
                }
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
                    {
                        return null;
                    }
                    else
                    {
                        codeValues[j + 1] = "0";
                    }
                }
                else
                {
                    codeValues[j + 1] = attrValue;
                }
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
                    {
                        attrValue += GetAttrValueByName(parent.Attributes, attrSummands[m]);
                    }
                }

                summandsCount = attrSummands.GetLength(0);
                for (int m = 0; m < summandsCount; m++)
                {
                    attrValue += GetAttrValueByName(xn.Attributes, attrSummands[m]);
                }

                if (attrValue != string.Empty)
                {
                    break;
                }
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
            {
                return Convert.ToInt32(str.Substring(str.Length - 2, 2));
            }

            return -1;
        }

        #endregion Функции закачки внутреннего шаблона XML


        #region Функции закачки данных блока XML

        /// <summary>
        /// Закачивает строку отчета
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">Объект таблицы фактов</param>
        /// <param name="date">Дата</param>
        /// <param name="clsValuesMapping">Массив значений ссылок на классификаторы</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="columnValues">Значения столбцов сумм</param>
        private void PumpReportRow(DataTable factTable, IFactTable fct, string date, object[] clsValuesMapping,
            int regionID, object[] columnValues, int budgetLevel, BlockProcessModifier blockProcessModifier)
        {
            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    if (blockProcessModifier == BlockProcessModifier.MRArrears)
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(columnValues, clsValuesMapping,
                            new object[] { "RefYearDayUNV", date, "RefRegion", regionID }));
                    }
                    else
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(columnValues, clsValuesMapping,
                            new object[] { "RefYearDayUNV", date, "REFREGIONS", regionID, "REFBDGTLEVELS", budgetLevel }));
                    }
                    break;

                case SKIFFormat.YearReports:
                    date = string.Format("{0}0001", this.DataSource.Year);
                    if (blockProcessModifier == BlockProcessModifier.YRBalanc)
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(columnValues, clsValuesMapping,
                            new object[] { "REFYEARDAYUNV", date, "REFREGION", regionID }));
                    }
                    else
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays( columnValues, clsValuesMapping,
                            new object[] { "REFYEARDAYUNV", date, "REFREGIONS", regionID, "REFBDGTLEVELS", budgetLevel }));
                    }
                    break;
            }
        }

        // Закачивает все элементы Рх
        private void PumpPxNodeAll(XmlNode dataNode, DataTable factTable, IFactTable fct,
            string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID,
            BlockProcessModifier blockProcessModifier, int budgetLevel)
        {
            XmlNodeList px = dataNode.SelectNodes("./Px");
            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0) * 2];
            string clsCode = GetClsfCode(dataNode, -1);
            for (int i = 0; i < px.Count; i++)
            {
                double sum = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;

                if (sum != 0)
                {
                    int count = colNo2ColumnMapping.GetLength(0);
                    for (int j = 0; j < count; j++)
                    {
                        columnValues[j] = colNo2ColumnMapping[j];
                        columnValues[j + 1] = sum;
                    }

                    switch (blockProcessModifier)
                    {
                        case BlockProcessModifier.YREmbezzles:
                            if (i >= 9) return;
                            clsValuesMapping[1] = clsCode;
                            clsValuesMapping[3] = GetAttrValueByName(px[i].Attributes, "Num", "ColNo");
                            break;
                    }

                    PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
                }
            }
        }

        // Закачивает все элементы Рх для блока "Задолженность"
        private void PumpPxNodeArrears(XmlNode dataNode, DataTable factTable, IFactTable fct,
            string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID,
            BlockProcessModifier blockProcessModifier, int budgetLevel)
        {
            XmlNodeList px = dataNode.SelectNodes("./Px");
            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0) * 2];
            for (int i = 0; i < px.Count; i++)
            {
                int num = Convert.ToInt32(GetAttrValueByName(px[i].Attributes, "Num"));
                columnValues[(num - 1) * 2] = colNo2ColumnMapping[num - 1];
                if (num <= 2)
                {
                    columnValues[(num - 1) * 2 + 1] = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;
                }
                else
                {
                    string strValue = px[i].Attributes["Value"].Value;
                    if (strValue.Length > 4000)
                        strValue = strValue.Substring(0, 4000);
                    columnValues[(num - 1) * 2 + 1] = strValue;
                }
            }
            PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
        }

        #region Баланс (Формы 120, 130, 430)

        private int GetMeansTypeBalanc(XmlForm xmlForm, int num)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form13001))
            {
                if (new int[] { 4, 8 }.Contains(num))
                    return 0;
                if (new int[] { 1, 5 }.Contains(num))
                    return 1;
                if (new int[] { 2, 6 }.Contains(num))
                    return 2;
                if (new int[] { 3, 7 }.Contains(num))
                    return 3;
            }
            else if ((xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002))
            {
                if (new int[] { 3, 6 }.Contains(num))
                    return 0;
                if (new int[] { 1, 4 }.Contains(num))
                    return 1;
                if (new int[] { 2, 5 }.Contains(num))
                    return 2;
            }
            else if ((xmlForm == XmlForm.Form43001) && (this.DataSource.Year >= 2011))
            {
                if (new int[] { 1, 4, 5, 8, 21, 24, 25, 28 }.Contains(num))
                    return 0;
                if (new int[] { 2, 6, 9, 11, 13, 15, 17, 19, 22, 26, 29, 31, 33, 35, 37, 39 }.Contains(num))
                    return 1;
                if (new int[] { 3, 7, 10, 12, 14, 16, 18, 20, 23, 27, 30, 32, 34, 36, 38, 40 }.Contains(num))
                    return 2;
            }
            else if (((xmlForm == XmlForm.Form43001) && (this.DataSource.Year < 2011)) ||
                (xmlForm == XmlForm.Form43002))
            {
                if (new int[] { 1, 4, 19, 22 }.Contains(num))
                    return 0;
                if (new int[] { 2, 5, 7, 9, 11, 13, 15, 17, 20, 23, 25, 27, 29, 31, 33, 35 }.Contains(num))
                    return 1;
                if (new int[] { 3, 6, 8, 10, 12, 14, 16, 18, 21, 24, 26, 28, 30, 32, 34, 36 }.Contains(num))
                    return 2;
            }
            return -1;
        }

        private int GetBudgetLevelBalanc(XmlForm xmlForm, int num, int vbAttrValue)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form12002) ||
                (xmlForm == XmlForm.Form13001) || (xmlForm == XmlForm.Form13002))
            {
                switch (vbAttrValue)
                {
                    case 0: return 1;
                    case 2: return 3;
                    case 3: return 11;
                    case 4: return 4;
                    case 5: return 5;
                    case 10: return 6;
                    case 9: return 8;
                }
            }
            else if ((xmlForm == XmlForm.Form43001) && (this.DataSource.Year >= 2011))
            {
                    if (new int[] { 1, 2, 3, 4, 21, 22, 23, 24 }.Contains(num))
                        return 1;
                    if (new int[] { 5, 6, 7, 8, 25, 26, 27, 28 }.Contains(num))
                        return 2;
                    if (new int[] { 9, 10, 29, 30 }.Contains(num))
                        return 3;
                    if (new int[] { 11, 12, 31, 32 }.Contains(num))
                        return 11;
                    if (new int[] { 13, 14, 33, 34 }.Contains(num))
                        return 4;
                    if (new int[] { 15, 16, 35, 36 }.Contains(num))
                        return 5;
                    if (new int[] { 17, 18, 37, 38 }.Contains(num))
                        return 6;
                    if (new int[] { 19, 20, 39, 40 }.Contains(num))
                        return 8;
            }
            else if (((xmlForm == XmlForm.Form43001) && (this.DataSource.Year < 2011)) ||
                (xmlForm == XmlForm.Form43002))
            {
                if (new int[] { 1, 2, 3, 19, 20, 21 }.Contains(num))
                    return 1;
                if (new int[] { 4, 5, 6, 22, 23, 24 }.Contains(num))
                    return 2;
                if (new int[] { 7, 8, 25, 26 }.Contains(num))
                    return 3;
                if (new int[] { 9, 10, 27, 28 }.Contains(num))
                    return 11;
                if (new int[] { 11, 12, 29, 30 }.Contains(num))
                    return 4;
                if (new int[] { 13, 14, 31, 32 }.Contains(num))
                    return 5;
                if (new int[] { 15, 16, 33, 34 }.Contains(num))
                    return 6;
                if (new int[] { 17, 18, 35, 36 }.Contains(num))
                    return 8;
            }
            return -1;
        }

        private int GetColumnIndexBalanc(XmlForm xmlForm, int num)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form13001))
            {
                if (num >= 5)
                    return 1;
                return 0;
            }
            else if ((xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002))
            {
                if (num >= 4)
                    return 1;
                return 0;
            }
            else if ((xmlForm == XmlForm.Form43001) && (this.DataSource.Year >= 2011))
            {
                if ((num == 4) || (num == 8))
                    return 2;
                if ((num == 24) || (num == 28))
                    return 3;
                if (num < 21)
                    return 0;
                return 1;
            }
            else if (((xmlForm == XmlForm.Form43001) && (this.DataSource.Year < 2011)) ||
                (xmlForm == XmlForm.Form43002))
            {
                if (num >= 19)
                    return 1;
                return 0;
            }
            return 0;
        }

        // Закачивает все элементы Px для блока "Баланс"
        private void PumpPxNodeBalanc(XmlForm xmlForm, XmlNode dataNode, DataTable factTable, IFactTable fct,
            string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID,
            BlockProcessModifier blockProcessModifier, int budgetLevel, int vbAttrValue)
        {
            XmlNodeList px = dataNode.SelectNodes("./Px");
            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0) * 2];
            for (int i = 0; i < px.Count; i++)
            {
                int num = Convert.ToInt32(GetAttrValueByName(px[i].Attributes, "Num"));
                clsValuesMapping = (object[])CommonRoutines.ConcatArrays(clsValuesMapping, new object[] {
                    "RefBdgtLev", GetBudgetLevelBalanc(xmlForm, num, vbAttrValue),
                    "RefMeansType", GetMeansTypeBalanc(xmlForm, num) });

                columnValues[1] = DBNull.Value;
                columnValues[3] = DBNull.Value;
                columnValues[5] = DBNull.Value;
                columnValues[7] = DBNull.Value;

                double factValue = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;
                int columnIndex = GetColumnIndexBalanc(xmlForm, num);
                columnValues[columnIndex * 2] = colNo2ColumnMapping[columnIndex];
                columnValues[columnIndex * 2 + 1] = factValue;

                PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
            }
        }

        #endregion

        /// <summary>
        /// Возвращает значение атрибута элемента по имени из списка
        /// </summary>
        /// <param name="xn">Список элементов</param>
        /// <param name="defaultValue">Значение атрибута, если он не найден или равен 0</param>
        /// <param name="colNo">Массив значений ColNo.
        /// ColNo может содержать значения через ; - тогда будет закачан тот атрибут, где есть значение;
        /// пустая строка эквивалентна нулевому значению атрибута, -1 - все атрибуты</param>
        /// <param name="usedColNo">ColNo, из которого было взято значение</param>
        /// <returns>Значение атрибута</returns>
        private double GetPxNodeValue(XmlNode xn, double defaultValue, string colNo, out int usedColNo)
        {
            usedColNo = -1;
            if (colNo == string.Empty)
            {
                return defaultValue;
            }

            string[] colNoArray = colNo.Split(';');
            XmlNodeList px = xn.SelectNodes(string.Format("./Px{0}", GetXPathConstrByAttr("ColNo", colNoArray)));
            if (px.Count == 0)
            {
                px = xn.SelectNodes(string.Format("./Px{0}", GetXPathConstrByAttr("Num", colNoArray)));
            }

            for (int i = 0; i < px.Count; i++)
            {
                double d = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;
                if (d != 0)
                {
                    usedColNo = Convert.ToInt32(colNoArray[i]);
                    return d;
                }
            }

            return defaultValue;
        }

        // Закачивает только указанные элементы
        private void PumpPxNodeStated(List<XmlForm> xmlForm, XmlNode dataNode, DataTable factTable, IFactTable fct, string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier, int budgetLevel, int vbAttrValue)
        {
            if (xmlForm != null && (xmlForm.Contains(XmlForm.Form128) || xmlForm.Contains(XmlForm.Form128v)))
            {
                // Для этих форм берем значение атрибута ВБ родительского элемента
                if (XmlHelper.GetIntAttrValue(dataNode.ParentNode, "ВБ", -1) != vbAttrValue)
                    return;
            }

            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0)];

            // Закачиваем указанные элементы Рх
            bool zeroSums = true;
            int usedColNo = -1;
            int count = colNo2ColumnMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                double sum = GetPxNodeValue(dataNode, 0, colNo2ColumnMapping[i + 1], out usedColNo) * sumMultiplier;

                if (sum != 0)
                {
                    zeroSums = false;
                }

                columnValues[i] = colNo2ColumnMapping[i];
                if (sum == 0)
                    columnValues[i + 1] = DBNull.Value;
                else
                    columnValues[i + 1] = sum;
            }

            if (!zeroSums)
            {
                PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
            }
        }

        /// <summary>
        /// Закачивает один элемент данных
        /// </summary>
        /// <param name="factTable">Таблица фактов, куда качать</param>
        /// <param name="fct">IFactTable таблицы фактов</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="date">Дата</param>
        /// <param name="clsValuesMapping">Массив значений классфикаторов</param>
        /// <param name="colNo2ColumnMapping">Массив пар имя_поля-ColNo.
        /// ColNo может содержать значения через ; - тогда будет закачан тот атрибут, где есть значение;
        /// пустая строка эквивалентна нулевому значению атрибута.
        /// Если в параметре nodeProcessOption указана обработка всех элементов, то массив должен содержать
        /// только имена полей.</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков.</param>
        /// <param name="nodeProcessOption">Вид обработки элемента с данными отчета.</param>
        /// <param name="budgetLevel">Уровень бюджета</param>
        /// <param name="xmlForm">форма</param>
        /// <param name="vbAttrValue">Значение атрибута ВБ элемента Documents, из которого берутся данные по
        /// указанному уровню бюджета</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel, List<XmlForm> xmlForm, int vbAttrValue)
        {
            if (colNo2ColumnMapping == null) return;

            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRSrcOutFin:
                    // Для блока "Источники внешнего финансирования" закачиваются только те значения,
                    // где есть значение в поле АдмКИВнФ
                    if (GetAttrValueByName(dataNode.Attributes, "АдмКИВнФ", "КИВнФ", "ClsfCode") == string.Empty)
                        return;
                    break;

                case BlockProcessModifier.MRSrcInFin:
                    // Для блока "Источники внутреннего финансирования" закачиваются только те значения,
                    // где есть значение в поле АдмКИВФ
                    if (GetAttrValueByName(dataNode.Attributes, "АдмКИВФ", "КИВФ", "ClsfCode") == string.Empty)
                        return;
                    break;
            }

            switch (nodeProcessOption)
            {
                case NodeProcessOption.All:
                    PumpPxNodeAll(dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel);
                    break;

                case NodeProcessOption.Arrears:
                    PumpPxNodeArrears(dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel);
                    break;

                case NodeProcessOption.Balanc:
                    PumpPxNodeBalanc(xmlForm[0], dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel, vbAttrValue);
                    break;

                case NodeProcessOption.Stated:
                    PumpPxNodeStated(xmlForm, dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel, vbAttrValue);
                    break;
            }
        }

        /// <summary>
        /// Закачивает один элемент данных
        /// </summary>
        /// <param name="factTable">Таблица фактов, куда качать</param>
        /// <param name="fct">IFactTable таблицы фактов</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="date">Дата</param>
        /// <param name="clsValuesMapping">Массив значений классфикаторов</param>
        /// <param name="colNo2ColumnMapping">Массив пар имя_поля-ColNo.
        /// ColNo может содержать значения через ; - тогда будет закачан тот атрибут, где есть значение;
        /// пустая строка эквивалентна нулевому значению атрибута.
        /// Если в параметре nodeProcessOption указана обработка всех элементов, то массив должен содержать
        /// только имена полей.</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков.</param>
        /// <param name="nodeProcessOption">Вид обработки элемента с данными отчета.</param>
        /// <param name="budgetLevel">Уровень бюджета</param>
        /// <param name="vbAttrValue">Значение атрибута ВБ элемента Documents, из которого берутся данные по
        /// указанному уровню бюджета</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel, int vbAttrValue)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, colNo2ColumnMapping, regionID,
                blockProcessModifier, nodeProcessOption, budgetLevel, null, vbAttrValue);
        }

        /// <summary>
        /// Закачивает один элемент данных
        /// </summary>
        /// <param name="factTable">Таблица фактов, куда качать</param>
        /// <param name="fct">IFactTable таблицы фактов</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="date">Дата</param>
        /// <param name="clsValuesMapping">Массив значений классфикаторов</param>
        /// <param name="colNo2ColumnMapping">Массив пар имя_поля-ColNo.
        /// ColNo может содержать значения через ; - тогда будет закачан тот атрибут, где есть значение;
        /// пустая строка эквивалентна нулевому значению атрибута.
        /// Если в параметре nodeProcessOption указана обработка всех элементов, то массив должен содержать
        /// только имена полей.</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="nodeProcessOption">Вид обработки элемента с данными отчета.</param>
        /// <param name="budgetLevel">Уровень бюджета</param>
        /// <param name="xmlForm">форма</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel, List<XmlForm> xmlForm)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, colNo2ColumnMapping, regionID,
                blockProcessModifier, nodeProcessOption, budgetLevel, null, -1);
        }

        /// <summary>
        /// Закачивает один элемент данных
        /// </summary>
        /// <param name="factTable">Таблица фактов, куда качать</param>
        /// <param name="fct">IFactTable таблицы фактов</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="date">Дата</param>
        /// <param name="clsValuesMapping">Массив значений классфикаторов</param>
        /// <param name="colNo2ColumnMapping">Массив пар имя_поля-ColNo.
        /// ColNo может содержать значения через ; - тогда будет закачан тот атрибут, где есть значение;
        /// пустая строка эквивалентна нулевому значению атрибута.
        /// Если в параметре nodeProcessOption указана обработка всех элементов, то массив должен содержать
        /// только имена полей.</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="nodeProcessOption">Вид обработки элемента с данными отчета.</param>
        /// <param name="budgetLevel">Уровень бюджета</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, colNo2ColumnMapping, regionID, 
                blockProcessModifier, nodeProcessOption, budgetLevel, null);
        }

        /// <summary>
        /// Закачивает данные формы 414
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="regionCode">Код района</param>
        /// <param name="date">Дата</param>
        /// <param name="regionCode">Исходный код района</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm414(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "1",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "2",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "3",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 7);
        }

        private void ProcessForm487Refs2009(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "1", "FACTREPORT", "13", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "2", "FACTREPORT", "14", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 12);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "3", "FACTREPORT", "15", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "4", "FACTREPORT", "16", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 13);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "5", "FACTREPORT", "17", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "6", "FACTREPORT", "18", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 17);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "7", "FACTREPORT", "19", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 4);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "8", "FACTREPORT", "20", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 14);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "9", "FACTREPORT", "21", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "10", "FACTREPORT", "22", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 15);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "11", "FACTREPORT", "23", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "12", "FACTREPORT", "24", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 16);
        }

        /// <summary>
        /// Закачивает данные формы 487
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="regionCode">Код района</param>
        /// <param name="date">Дата</param>
        /// <param name="regionCode">Исходный код района</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm487(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            bool isRefsData = ((blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx) ||
                               (blockProcessModifier == BlockProcessModifier.MRCommonBooks) ||
                               (blockProcessModifier == BlockProcessModifier.MRExcessBooks));
            // c 2009 года - изменения в закачке справ форм
            if ((this.DataSource.Year >= 2009) && (isRefsData))
            {
                ProcessForm487Refs2009(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                return;
            }

            string assignedSumColumn = string.Empty;
            if (isRefsData)
                assignedSumColumn = "1";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "7", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);

            if (isRefsData)
                assignedSumColumn = "2";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "8",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            if (isRefsData)
                assignedSumColumn = "3";
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "9",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            else
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "9",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);

            if (isRefsData)
                assignedSumColumn = "4";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "10",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);

            if (isRefsData)
                assignedSumColumn = "5";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "11",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);

            if (isRefsData)
                assignedSumColumn = "6";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "12",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
        }

        /// <summary>
        /// Закачивает данные формы 623
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="regionCode">Код района</param>
        /// <param name="date">Дата</param>
        /// <param name="regionCode">Исходный код района</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm623(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "2" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "3" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "4" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 7);

            switch (regionCode.Length)
            {
                case 2:
                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                        new string[] { "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", string.Empty },
                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                    break;

                case 5:
                    if (regionCode[2] == '0')
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", string.Empty },
                            regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                    }
                    if (regionCode.EndsWith("900") || regionCode.EndsWith("100"))
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", string.Empty },
                            regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                    }
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные формы 625
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="regionCode">Код района</param>
        /// <param name="date">Дата</param>
        /// <param name="regionCode">Исходный код района</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm625(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            date = string.Format("{0}0000", this.DataSource.Year);
            if (this.DataSource.Year >= 2010)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "1", "EndYearRep", "4", "BudMidYRep", "7", "FMidYRep", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "2", "EndYearRep", "5", "BudMidYRep", "8", "FMidYRep", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "3", "EndYearRep", "6", "BudMidYRep", "9", "FMidYRep", "12" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 9);
            }
            else
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "1", "EndYearRep", "2", "BudMidYRep", "3", "FMidYRep", "4" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            }
        }

        private void ProcessForm624(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            date = string.Format("{0}0000", this.DataSource.Year);
            int budLevel = 2;
            if (this.DataSource.Year >= 2010)
                budLevel = 4;
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "BegYearRep", "2", "EndYearRep", "6", "BudMidYRep", "10", "FMidYRep", "14" },
                regID, blockProcessModifier, NodeProcessOption.Stated, budLevel);
            if (this.DataSource.Year >= 2010)
                budLevel = 5;
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "BegYearRep", "3", "EndYearRep", "7", "BudMidYRep", "11", "FMidYRep", "15" },
                regID, blockProcessModifier, NodeProcessOption.Stated, budLevel);
            if (this.DataSource.Year >= 2010)
                budLevel = 6;
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "BegYearRep", "4", "EndYearRep", "8", "BudMidYRep", "12", "FMidYRep", "16" },
                regID, blockProcessModifier, NodeProcessOption.Stated, budLevel);
        }

        /// <summary>
        /// Закачивает данные формы 630
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="regionCode">Код района</param>
        /// <param name="date">Дата</param>
        /// <param name="regionCode">Исходный код района</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm630(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            switch (regionCode.Length)
            {
                case 2:
                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                        new string[] { "FACT" }, regID, blockProcessModifier, NodeProcessOption.All, 2);
                    break;

                case 5:
                    if (regionCode[2] == '0')
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "FACT" }, regID, blockProcessModifier, NodeProcessOption.All, 7);
                    }
                    if (regionCode.EndsWith("900") || regionCode.EndsWith("100"))
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "FACT" }, regID, blockProcessModifier, NodeProcessOption.All, 3);
                    }
                    break;

                default: return;
            }
        }

        /// <summary>
        /// Закачивает данные формы 128
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="regionCode">Код района</param>
        /// <param name="date">Дата</param>
        /// <param name="regionCode">Исходный код района</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков.</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm128(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            string monthNum = string.Empty;
            string factNum = string.Empty;
            string spreadYearNum = string.Empty;
            string spreadMonthNum = string.Empty;

            if (blockProcessModifier == BlockProcessModifier.MRDefProf ||
                blockProcessModifier == BlockProcessModifier.MROutcomes)
            {
                monthNum = "2";
                factNum = "3";
                spreadYearNum = "4";
                spreadMonthNum = "5";
            }
            else if (blockProcessModifier == BlockProcessModifier.MRIncomes || 
                blockProcessModifier == BlockProcessModifier.MRSrcInFin ||
                blockProcessModifier == BlockProcessModifier.MRSrcOutFin)
            {
                factNum = "2";
                spreadYearNum = "3";
            }

            List<XmlForm> xmlForm = new List<XmlForm>(2);
            xmlForm.Add(XmlForm.Form128);
            xmlForm.Add(XmlForm.Form128v);

            if (this.skifFormat == SKIFFormat.MonthReports)
            {

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3, xmlForm, 2);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4, xmlForm, 4);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5, xmlForm, 5);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6, xmlForm, 10);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8, xmlForm, 9);

                if (this.DataSource.Year * 100 + this.DataSource.Month >= 200704)
                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                        regID, blockProcessModifier, NodeProcessOption.Stated, 11, xmlForm, 3);
            }
            else
            {
                string performedNum = "2";
                if ((blockProcessModifier == BlockProcessModifier.YROutcomes) ||
                    (blockProcessModifier == BlockProcessModifier.YRDefProf))
                    performedNum = "3";

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3, xmlForm, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7, xmlForm, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4, xmlForm, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5, xmlForm, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6, xmlForm, 10);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8, xmlForm, 9);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2, xmlForm, 0);
            }
        }

        #region форма 117

        private string[] GetSumsMappingForm117()
        {
            if (this.SkifReportFormat == SKIFFormat.MonthReports)
                return new string[] { "YEARPLANREPORT", "1", "FACTREPORT", "2", "SPREADFACTYEARPLANREPORT", "3" };
            else
                return new string[] { "AssignedReport", "1", "PerformedReport", "2" };
        }

        private int GetBudgetLevelForm117(int vb)
        {
            switch (vb)
            {
                case 0:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 11;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 9:
                    return 8;
                case 10:
                    return 6;
                default:
                    return 1;
            }
        }

        private void ProcessForm117(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            int vb = XmlHelper.GetIntAttrValue(dataNode.ParentNode, "ВБ", -1);
            int budgetLevel = GetBudgetLevelForm117(vb);
            string[] sumsMapping = GetSumsMappingForm117();
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, sumsMapping,
                regID, blockProcessModifier, NodeProcessOption.Stated, budgetLevel);
        }

        #endregion форма 117

        private void ProcessForm628(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            int budgetLevel = 1;
            for (int i = 1; i <= 8; i++)
            {
                int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
                if ((sourceDate >= 200704) && (i == 4))
                    continue;
                string quarterPlanReport = Convert.ToString(i + 8);
                string monthPlanReport = Convert.ToString(i + 16);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", quarterPlanReport, 
                    "MONTHPLANREPORT", monthPlanReport, "ASSIGNEDREPORT", string.Empty, "FACTREPORT", string.Empty, 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, budgetLevel);
                budgetLevel++;
            }
        }

        #region форма 428

        private void ProcessForm428Account(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "1" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "2" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "3" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 4);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "4" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "5" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "6" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 8);
        }

        private void ProcessForm428_2011(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "1", "FACTREPORT", "11", "ExcSumPRep", "2", "ExcSumFRep", "12" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 1);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "3", "FACTREPORT", "13", "ExcSumPRep", "4", "ExcSumFRep", "14" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "5", "FACTREPORT", "15" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "6", "FACTREPORT", "16" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "7", "FACTREPORT", "17" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 4);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "8", "FACTREPORT", "18" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "9", "FACTREPORT", "19" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "10", "FACTREPORT", "20" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 8);
        }

        /// <summary>
        /// Закачивает данные формы 428 (МесОтч)
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="xmlForm">Код формы</param>
        /// <param name="date">Дата</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm428MR(DataTable factTable, IFactTable fct, XmlNode dataNode, XmlForm xmlForm,
            string date, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            if (blockProcessModifier == BlockProcessModifier.MRAccount)
            {
                ProcessForm428Account(factTable, fct, dataNode, date,
                    clsValuesMapping, blockProcessModifier, regID);
                return;
            }

            if (this.DataSource.Year >= 2011)
            {
                ProcessForm428_2011(factTable, fct, dataNode, date,
                    clsValuesMapping, blockProcessModifier, regID);
                return;
            }

            if ((xmlForm == XmlForm.Form428v) && (this.DataSource.Year >= 2008))
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "YEARPLAN", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "4",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "2", "YEARPLAN", "2", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "5",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "3", "YEARPLAN", "3", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "6",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                return;
            }

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "1", "YEARPLAN", "1", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "9",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 1);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "2", "YEARPLAN", "2", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "10",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "3", "YEARPLAN", "3", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "11",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200704)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "4", "YEARPLAN", "4", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "12",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 11);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "5", "YEARPLAN", "5", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "13",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "6", "YEARPLAN", "6", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "14",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "7", "YEARPLAN", "7", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "15",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            }
            else
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "4", "YEARPLAN", "4", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "12",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "5", "YEARPLAN", "5", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "13",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "6", "YEARPLAN", "6", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "14",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "7", "YEARPLAN", "7", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "15",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7);
            }
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "8", "YEARPLAN", "8", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "16",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 8);
        }

        /// <summary>
        /// Закачивает данные формы 428 (ГодОтч)
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="xmlForm">Код формы</param>
        /// <param name="date">Дата</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков</param>
        /// <param name="regID">ИД района</param>
        private void ProcessForm428YR(DataTable factTable, IFactTable fct, XmlNode dataNode, XmlForm xmlForm, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            if ((this.DataSource.Year >= 2011) && (xmlForm == XmlForm.Form428g))
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "11", "ASSIGNED", "1", "PERFORMED", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "13", "ASSIGNED", "3", "PERFORMED", "13" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "15", "ASSIGNED", "5", "PERFORMED", "15" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "6", "PERFORMEDREPORT", "16", "ASSIGNED", "6", "PERFORMED", "16" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 11);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "7", "PERFORMEDREPORT", "17", "ASSIGNED", "7", "PERFORMED", "17" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "8", "PERFORMEDREPORT", "18", "ASSIGNED", "8", "PERFORMED", "18" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "9", "PERFORMEDREPORT", "19", "ASSIGNED", "9", "PERFORMED", "19" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "10", "PERFORMEDREPORT", "20", "ASSIGNED", "10", "PERFORMED", "20" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "EXCSUMPREP", "2", "EXCSUMFREP", "12", "EXCSUMP", "2", "EXCSUMF", "12" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "EXCSUMPREP", "4", "EXCSUMFREP", "14", "EXCSUMP", "4", "EXCSUMF", "14" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            }
            else if ((this.DataSource.Year >= 2007) && (xmlForm == XmlForm.Form428g))
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "9", "ASSIGNED", "1", "PERFORMED", "9" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "10", "ASSIGNED", "2", "PERFORMED", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "11", "ASSIGNED", "3", "PERFORMED", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "13", "ASSIGNED", "5", "PERFORMED", "13" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "6", "PERFORMEDREPORT", "14", "ASSIGNED", "6", "PERFORMED", "14" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "7", "PERFORMEDREPORT", "15", "ASSIGNED", "7", "PERFORMED", "15" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "8", "PERFORMEDREPORT", "16", "ASSIGNED", "8", "PERFORMED", "16" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
            }
            else if (this.DataSource.Year >= 2007)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "4", "ASSIGNED", "1", "PERFORMED", "4" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "5", "ASSIGNED", "2", "PERFORMED", "5" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "6", "ASSIGNED", "3", "PERFORMED", "6" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            }
            else if (this.DataSource.Year >= 2006)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "10", "ASSIGNED", "1", "PERFORMED", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "11", "ASSIGNED", "2", "PERFORMED", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "12", "ASSIGNED", "3", "PERFORMED", "12" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "14", "ASSIGNED", "5", "PERFORMED", "14" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "6", "PERFORMEDREPORT", "15", "ASSIGNED", "6", "PERFORMED", "15" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "7", "PERFORMEDREPORT", "16", "ASSIGNED", "7", "PERFORMED", "16" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "8", "PERFORMEDREPORT", "17", "ASSIGNED", "8", "PERFORMED", "17" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "9", "PERFORMEDREPORT", "18", "ASSIGNED", "9", "PERFORMED", "18" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
            }
            else
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "6", "ASSIGNED", "1", "PERFORMED", "6" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "7", "ASSIGNED", "2", "PERFORMED", "7" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "8", "ASSIGNED", "3", "PERFORMED", "8" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "4", "PERFORMEDREPORT", "9", "ASSIGNED", "4", "PERFORMED", "9" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "10", "ASSIGNED", "5", "PERFORMED", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
            }
        }

        #endregion форма 428

        #region форма 127

        private string[] GetSumsMappingForm127(BlockProcessModifier blockProcessModifier)
        {
            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRIncomes:
                case BlockProcessModifier.MRSrcInFin:
                case BlockProcessModifier.MRSrcOutFin:
                    return new string[] { "YearPlanReport", "1", "FactReport", "5", "SpreadFactYearPlanReport", "6" };
                case BlockProcessModifier.MROutcomes:
                case BlockProcessModifier.MRDefProf:
                    return new string[] { "YearPlanReport", "1", "MonthPlanReport", "2", "FactReport", "6", 
                        "SpreadFactYearPlanReport", "7", "SpreadFactMonthPlanReport", "8" };

                case BlockProcessModifier.YRIncomes:
                case BlockProcessModifier.YRSrcFin:
                    return new string[] { "AssignedReport", "1", "PerformedReport", "5" };
                case BlockProcessModifier.YROutcomes:
                case BlockProcessModifier.YRDefProf:
                    return new string[] { "AssignedReport", "1", "PerformedReport", "6" };
                default:
                    return new string[] { };
            }
        }

        private int GetBudgetLevelForm127(int vb)
        {
            switch (vb)
            {
                case 0:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 7;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 9:
                    return 8;
                case 10:
                    return 6;
                case 33:
                    return 11;
                default:
                    return 1;
            }
        }

        private void ProcessForm127(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            int vb = XmlHelper.GetIntAttrValue(dataNode.ParentNode, "ВБ", -1);
            int budgetLevel = GetBudgetLevelForm127(vb);
            string[] sumsMapping = GetSumsMappingForm127(blockProcessModifier);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, sumsMapping,
                regID, blockProcessModifier, NodeProcessOption.Stated, budgetLevel);
        }

        #endregion форма 127

        /// <summary>
        /// Закачивает элементы с данными по отчетам
        /// </summary>
        /// <param name="da">ДатаАдаптер таблицы фактов</param>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">Элемент с данными по классификатору</param>
        /// <param name="xmlForm">Номер формы</param>
        /// <param name="regionCode">Код района</param>
        /// <param name="date">Дата</param>
        /// <param name="regionCode">Исходный код района</param>
        /// <param name="clsValuesMapping">Список пар поле-значение классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="regID">ИД района</param>
        private void ProcessPxNodes(IDbDataAdapter da, DataTable factTable, IFactTable fct, XmlNode dataNode,
            XmlForm xmlForm, int regionID, string date, string regionCode, object[] clsValuesMapping,
            BlockProcessModifier blockProcessModifier, int nullRegions, int vbAttrValue)
        {
            int regID = regionID;
            if (regID < 0)
            {
                regID = nullRegions;
            }

            switch (this.SkifReportFormat)
            {
                #region SKIFFormat.MonthReports

                case SKIFFormat.MonthReports:
                    switch (this.XmlReportFormat)
                    {
                        #region XmlFormat.Format2004
                        case XmlFormat.Format2004:
                            switch (xmlForm)
                            {
                                case XmlForm.Form649:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "1", "QUARTERPLANREPORT", "5", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "2", "QUARTERPLANREPORT", "6", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "3", "QUARTERPLANREPORT", "7", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;

                                case XmlForm.Form650:
                                case XmlForm.Form651:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "1",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "2",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "3",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;
                            }
                            break;
                        #endregion XmlFormat.Format2004

                        #region XmlFormat.Format2005
                        case XmlFormat.Format2005:
                            switch (xmlForm)
                            {
                                case XmlForm.Form649:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "1", "QUARTERPLANREPORT", "4", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "2", "QUARTERPLANREPORT", "5", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "3", "QUARTERPLANREPORT", "6", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;

                                case XmlForm.Form650:
                                case XmlForm.Form651:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "1",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "2",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "3",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.October2005
                        case XmlFormat.October2005:
                            switch (xmlForm)
                            {
                                case XmlForm.Form414:
                                    ProcessForm414(factTable, fct, dataNode, date, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form428:
                                case XmlForm.Form428v:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "5",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "2", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "6",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "3", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "7",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 8);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "4", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "8",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.Skif3
                        case XmlFormat.Skif3:
                            switch (xmlForm)
                            {
                                case XmlForm.Form128:
                                case XmlForm.Form128v:
                                    ProcessForm128(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form414:
                                    ProcessForm414(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form487:
                                    ProcessForm487(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form428:
                                case XmlForm.Form428v:
                                    ProcessForm428MR(factTable, fct, dataNode, xmlForm, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form117:
                                    ProcessForm117(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form628r:
                                    ProcessForm628(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form127:
                                case XmlForm.Form127g:
                                case XmlForm.Form127v:
                                    ProcessForm127(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form159:
                                case XmlForm.Form169:
                                case XmlForm.Form159V:
                                case XmlForm.Form169V:
                                case XmlForm.Form469:
                                case XmlForm.Form459:
                                case XmlForm.Form469V:
                                case XmlForm.Form459V:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                                        "Arrears", "OutArrears", "Contractor", "Occasion" }, regID, blockProcessModifier,
                                        NodeProcessOption.Arrears, -1);
                                    break;
                            }
                            break;
                        #endregion
                    }
                    break;

                #endregion SKIFFormat.MonthReports

                #region SKIFFormat.YearReports

                case SKIFFormat.YearReports:
                    switch (this.XmlReportFormat)
                    {
                        #region XmlFormat.Format2004
                        case XmlFormat.Format2004:
                            switch (xmlForm)
                            {
                                case XmlForm.Form623:
                                    ProcessForm623(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form625:
                                    ProcessForm625(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form630:
                                    ProcessForm630(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.Format2005
                        case XmlFormat.Format2005:
                            switch (xmlForm)
                            {
                                case XmlForm.Form623:
                                    ProcessForm623(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form625:
                                    ProcessForm625(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form630:
                                    ProcessForm630(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.October2005
                        case XmlFormat.October2005:
                            break;
                        #endregion

                        #region XmlFormat.Skif3
                        case XmlFormat.Skif3:
                            switch (xmlForm)
                            {
                                case XmlForm.Form128:
                                case XmlForm.Form128v:
                                    ProcessForm128(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form428g:
                                case XmlForm.Form428Vg:
                                    ProcessForm428YR(factTable, fct, dataNode, xmlForm, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form625:
                                    ProcessForm625(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form624:
                                    ProcessForm624(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form127:
                                case XmlForm.Form127g:
                                case XmlForm.Form127v:
                                    ProcessForm127(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form117:
                                    ProcessForm117(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form12001:
                                case XmlForm.Form13001:
                                case XmlForm.Form43001:
                                case XmlForm.Form12002:
                                case XmlForm.Form13002:
                                case XmlForm.Form43002:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                                        new string[] { "BegYear", "EndYear", "ExceptBegYear", "ExceptEndYear" },
                                        regID, blockProcessModifier, NodeProcessOption.Balanc, -1,
                                        new List<XmlForm>(new XmlForm[] { xmlForm }), vbAttrValue);
                                    break;
                            }
                            break;
                        #endregion
                    }
                    break;

                #endregion SKIFFormat.YearReports
            }
        }

        /// <summary>
        /// Возвращает ограничение для выборки из хмл по атрибуту
        /// </summary>
        /// <param name="attrName">Наименование атрибута</param>
        /// <param name="attrValues">Список с означениями атрибута</param>
        /// <returns>Ограничение</returns>
        protected string GetXPathConstrByAttr(string attrName, string[] attrValues)
        {
            string result = string.Empty;

            int count = attrValues.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result += string.Format(" or @{0} = \"{1}\"", attrName, attrValues[i]);
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(0, 4) + "]";
            }

            return result;
        }

        /// <summary>
        /// Возвращает ограничение для выборки из хмл по атрибуту
        /// </summary>
        /// <param name="attrName">Наименование атрибута</param>
        /// <param name="attrValues">Список с означениями атрибута</param>
        /// <returns>Ограничение</returns>
        protected string GetXPathConstrByAttr(string attrName, int[] attrValues)
        {
            string result = string.Empty;

            int count = attrValues.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result += string.Format(" or @{0} = \"{1}\"", attrName, attrValues[i]);
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(0, 4) + "]";
            }

            return result;
        }

        /// <summary>
        /// Возвращает ограничение для выборки из хмл по атрибуту
        /// </summary>
        /// <param name="attrName">Наименование атрибута</param>
        /// <param name="xmlForm">Массив номеров форм</param>
        /// <returns>Ограничение</returns>
        protected string GetXPathConstrByAttr(string attrName, XmlForm[] xmlForm)
        {
            string result = string.Empty;

            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                result += string.Format(" or @{0} = \"{1}\"", attrName, XmlFormToString(xmlForm[i]));
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(0, 4) + "]";
            }

            return result;
        }

        /// <summary>
        /// Возвращает ограничение для выборки из хмл по FormNo.
        /// </summary>
        /// <param name="attrName">Наименование атрибута</param>
        /// <param name="attrValues">Список с означениями атрибута</param>
        /// <returns>Ограничение</returns>
        private string GetXPathConstrByFormNo(int sectNo, XmlForm[] xmlForm, XmlFormat xmlFormat)
        {
            string result = string.Empty;

            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                switch (xmlFormat)
                {
                    case XmlFormat.Format2004:
                        result += string.Format(" or @FormNo = \"{0}\"", XmlFormToString(xmlForm[i]));
                        break;

                    case XmlFormat.Format2005:
                    case XmlFormat.October2005:
                        result += string.Format(
                            " or @Code = \"{0}{1}\"",
                            XmlFormToString(xmlForm[i]), sectNo.ToString().PadLeft(2, '0'));
                        break;
                }
            }

            if (result != string.Empty)
            {
                // Удаляем первый or
                result = "[" + result.Remove(0, 3).Trim() + "]";

                switch (xmlFormat)
                {
                    case XmlFormat.Format2004:
                        result = string.Format("//Forma{0}/Section[@SectNo = \"{1}\"]", result, sectNo);
                        break;

                    case XmlFormat.Format2005:
                    case XmlFormat.October2005:
                        result = string.Format("//Form{0}", result);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Создает массив значений ссылок на классификаторы в зависимости от блока
        /// </summary>
        /// <param name="formNo">Номер формы</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <returns>Массив значений</returns>
        private object[] PrepareIndividualCodesMappingXML(XmlForm xmlForm, BlockProcessModifier blockProcessModifier)
        {
            object[] result = new object[0];

            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRStandard:
                case BlockProcessModifier.MRDefProf:
                case BlockProcessModifier.MRIncomes:
                case BlockProcessModifier.MROutcomes:
                case BlockProcessModifier.MRSrcInFin:
                case BlockProcessModifier.MRSrcOutFin:
                case BlockProcessModifier.MRAccount:
                case BlockProcessModifier.YRDefProf:
                case BlockProcessModifier.YROutcomes:
                case BlockProcessModifier.YRIncomes:
                case BlockProcessModifier.YRSrcFin:
                    Array.Resize(ref result, 2);
                    result[0] = "REFMEANSTYPE";

                    switch (xmlForm)
                    {
                        case XmlForm.Form414:
                        case XmlForm.Form487:
                            break;
                        case XmlForm.Form128:
                        case XmlForm.Form428:
                        case XmlForm.Form649:
                        case XmlForm.Form650:
                        case XmlForm.Form428g:
                        case XmlForm.Form623:
                        case XmlForm.Form117:
                        case XmlForm.Form628r:
                        case XmlForm.Form127:
                        case XmlForm.Form127g:
                            result[1] = 1;
                            break;
                        case XmlForm.Form128v:
                        case XmlForm.Form428v:
                        case XmlForm.Form428Vg:
                        case XmlForm.Form127v:
                            result[1] = 2;
                            break;
                        case XmlForm.Form651:
                            break;
                    }

                    break;
                case BlockProcessModifier.MRArrears:
                    Array.Resize(ref result, 4);
                    result[0] = "RefMeansType";
                    result[2] = "RefType";

                    switch (xmlForm)
                    {
                        case XmlForm.Form159:
                        case XmlForm.Form459:
                            result[1] = 1;
                            result[3] = 1;
                            break;
                        case XmlForm.Form159V:
                        case XmlForm.Form459V:
                            result[1] = 2;
                            result[3] = 1;
                            break;
                        case XmlForm.Form169:
                        case XmlForm.Form469:
                            result[1] = 1;
                            result[3] = 0;
                            break;
                        case XmlForm.Form169V:
                        case XmlForm.Form469V:
                            result[1] = 2;
                            result[3] = 0;
                            break;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Формирует код классификатора для поиска ссылок на классификаторы строки фактов
        /// </summary>
        /// <param name="clsCodeAttr">Атрибуты кода</param>
        /// <param name="xn">Элемент со строкой фактов</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <returns>Код</returns>
        private string GetClsCodeFromFactNode(string clsCodeAttr, XmlNode xn, BlockProcessModifier blockProcessModifier)
        {
            string result = string.Empty;

            if (clsCodeAttr == string.Empty)
            {
                result = GetClsfCode(xn, -1);
            }
            else
            {
                result = GetFieldValueAtPos(clsCodeAttr, xn, true);
            }

            switch (blockProcessModifier)
            {
                case BlockProcessModifier.YREmbezzles: return result.TrimStart('0');
            }

            return result;
        }

        /// <summary>
        /// Выбирает данные отчета по указанным параметрам
        /// </summary>
        /// <param name="xn">Элемент с данными отчета</param>
        /// <param name="sectNo">Номера секций</param>
        /// <param name="xmlForm">Массив форм отчета</param>
        /// <returns>Элементы с данными отчета</returns>
        private XmlNodeList GetFactDocumentNodes(XmlNode xn, int[] sectNo, XmlForm[] xmlForm)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                    return xn.SelectNodes(string.Format(
                        ".//Forma{0}/Section{1}",
                        GetXPathConstrByAttr("FormNo", xmlForm), GetXPathConstrByAttr("SectNo", sectNo)));

                default:
                    return xn.SelectNodes(string.Format(
                        ".//Form{0}/Document", GetConstrForInternalPattern(xmlForm, sectNo)));
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
            {
                XmlNodeList documents = GetFactDocumentNodes(xn,
                    CommonRoutines.ParseParamsString(formSection[i + 1]), StringToXmlForms(formSection[i]));
                foreach (XmlNode document in documents)
                {
                    result += document.SelectNodes("Data").Count;
                }
            }

            return result;
        }

        /// <summary>
        /// Проверяет, чтобы код пристуствовал во всех классификаторах
        /// </summary>
        /// <param name="fieldsMapping">Список пар поле_кода_классификатора-значение</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="clsCode">Код классификатора из хмл</param>
        /// <returns>true - Код найден во всех codesMapping, иначе в каком-то нет.</returns>
        private void FindClsIDByCode(object[] fieldsMapping, Dictionary<string, int>[] codesMapping,
            string clsCode)
        {
            int count = fieldsMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                if (codesMapping[i / 2].ContainsKey(clsCode))
                {
                    fieldsMapping[i + 1] = codesMapping[i / 2][clsCode];
                }
                else
                {
                    throw new Exception(string.Format("Код {0} не обнаружен в справочнике классификатора", clsCode));
                }
            }
        }

        /// <summary>
        /// Проверяет, чтобы код пристуствовал во всех классификаторах. Если его где-то нет, то ставится ссылка
        /// на "Неуказанный классификатор"
        /// </summary>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов</param>
        /// <param name="factRefsToCls">Массив ссылок на классификаторы из фактов</param>
        /// <param name="codesMapping">Кэши классификаторов</param>
        /// <param name="clsValuesMapping">Массив поле-значение ссылки на классификатор</param>
        /// <param name="clsCode">Код классификатора</param>
        private void CheckClsIDByCodeWithUnknown(DataTable[] clsTable, IClassifier[] cls, string[] factRefsToCls,
            Dictionary<string, int>[] codesMapping, object[] clsValuesMapping, string clsCode)
        {
            int count = factRefsToCls.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (!codesMapping[i].ContainsKey(clsCode))
                {
                    if (codesMapping[i].ContainsKey("0"))
                    {
                        clsValuesMapping[i * 2 + 1] = codesMapping[i]["0"];
                    }
                    else
                    {
                        clsValuesMapping[i * 2 + 1] = PumpCachedRow(
                            codesMapping[i], clsTable[i], cls[i], "0", new object[] { 
                                GetClsCodeField(cls[i]), "0", "NAME", "Неуказанный классификатор",
                                "KL", -1, "KST", -1 });
                    }
                }
                else
                {
                    clsValuesMapping[i * 2 + 1] = codesMapping[i][clsCode];
                }
            }
        }

        /// <summary>
        /// Формирует массив значений ИД классификаторов. Описание параметров см. в PumpXMLReportBlock.
        /// </summary>
        /// <returns>Данные подлежат закачке или нет</returns>
        private bool GetClsIDForFact(DataTable[] clsTable, IClassifier[] cls, string[] factRefsToCls,
            int[] nullRefsToCls, Dictionary<string, int>[] codesMapping, BlockProcessModifier blockProcessModifier,
            int yearIndex, object[] clsValuesMapping, string clsCodeField, string clsCode, string[] attr2ClsMapping)
        {
            if (clsTable == null || cls == null || factRefsToCls == null || nullRefsToCls == null)
                return true;
            int clsID;
            try
            {
                if ((blockProcessModifier == BlockProcessModifier.MROutcomes) && (this.DataSource.Year >= 2005))
                    clsCode = clsCode.PadLeft(20, '0');
                clsCode = clsCode.ToUpper().Replace('X', '0').Replace('Х', '0');
                switch (blockProcessModifier)
                {
                    case BlockProcessModifier.MROutcomes:
                    case BlockProcessModifier.MRIncomesBooks:
                    case BlockProcessModifier.MROutcomesBooks:
                    case BlockProcessModifier.YROutcomes:
                        // Получаем значения кодов классификаторов
                        string[] codeValues = GetCodeValuesAsSubstring(attr2ClsMapping, clsCode, "0");
                        if (codeValues == null)
                        {
                            codeValues = new string[clsTable.GetLength(0)];
                            CommonRoutines.InitArray(codeValues, clsCode);
                        }

                        // для фкр к коду приписываем 10 нулей
                        if (cls[0].ObjectKey == "0299a09f-9d23-4e6c-b39a-930cbe219c3a")
                            codeValues[0] += "0000000000";

                        // Формируем ссылки на классификаторы по полученным кодам
                        int count = codeValues.GetLength(0);
                        for (int i = 0; i < count; i++)
                        {
                            if (clsTable[i] == null)
                                continue;

                            string code = codeValues[i];
                            if (this.DataSource.Year >= 2005)
                                code = code.TrimStart('0').PadLeft(1, '0');
                            // букавки в коде приводим к английской
                            code = code.Replace('А', 'A');
                            code = code.Replace('а', 'a');
                            clsValuesMapping[i * 2 + 1] = PumpCachedRow(codesMapping[i], clsTable[i], cls[i], code,
                                new object[] { GetClsCodeField(cls[i]), code, "NAME", constDefaultClsName, "KL", -1, "KST", -1 });
                        }
                        break;
                    case BlockProcessModifier.MROutcomesBooksEx:
                    case BlockProcessModifier.MRCommonBooks:
                    case BlockProcessModifier.MRExcessBooks:
                        clsID = nullRefsToCls[yearIndex];
                        if (codesMapping != null)
                        {
                            foreach (KeyValuePair<string, int> item in codesMapping[0])
                            {
                                if (item.Key.TrimStart('0') == clsCode.TrimStart('0'))
                                {
                                    clsID = item.Value;
                                    break;
                                }
                            }
                        }
                        else
                            clsID = FindRowID(clsTable[yearIndex],
                                new object[] { clsCodeField, clsCode }, nullRefsToCls[yearIndex]);
                        clsValuesMapping[yearIndex * 2 + 1] = clsID;
                        break;
                    case BlockProcessModifier.YREmbezzles:
                        break;

                    default:
                        if (yearIndex >= 0)
                        {
                            if (codesMapping != null)
                                clsID = FindCachedRow(codesMapping[0], clsCode, nullRefsToCls[yearIndex]);
                            else
                                clsID = FindRowID(clsTable[yearIndex],
                                    new object[] { clsCodeField, clsCode }, nullRefsToCls[yearIndex]);
                            clsValuesMapping[yearIndex * 2 + 1] = clsID;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка при формировании кодов классификации по {0}", clsCode), ex);
            }
            return true;
        }

        /// <summary>
        /// Возвращает массив исключений кодов для указанной формы
        /// </summary>
        /// <param name="codeExclusions">Массив исключений</param>
        /// <param name="xmlForm">Форма</param>
        /// <returns>Массив исключений для формы</returns>
        private string[] GetCodesExclusions4XmlForm(Dictionary<XmlForm, string[]> codeExclusions, XmlForm xmlForm)
        {
            if (codeExclusions.ContainsKey(xmlForm))
            {
                return codeExclusions[xmlForm];
            }
            else if (codeExclusions.ContainsKey(XmlForm.UnknownForm))
            {
                return codeExclusions[XmlForm.UnknownForm];
            }

            return new string[0];
        }

        /// <summary>
        /// Ищет указанную форму в массиве форм
        /// </summary>
        /// <param name="xmlForm">Форма</param>
        /// <param name="xmlForms">Массив форм</param>
        /// <returns>Есть или нет</returns>
        private bool FindXmlForm(XmlForm xmlForm, XmlForm[] xmlForms)
        {
            foreach (XmlForm form in xmlForms)
            {
                if (form == xmlForm)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Для района проверяет, есть ли данные по указанным формам и секциям и определяет, что качать, а что - нет
        /// </summary>
        /// <param name="xn">Узел с данными отчета по району</param>
        /// <param name="formSection">Массив пар номер_формы-номер_секции. Каждая пара определяет, даыные из каких 
        /// секций и форм будут закачаны (соответствие секций формам). Номера форм должны быть перечислены через ";".
        /// Формат номер_секции:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в 
        /// варианте 2)</param>
        /// <returns>Скорректированное значение formSection</returns>
        private string[] CheckFormForPump(XmlNode xn, string[] formSection)
        {
            string[] result = new string[formSection.GetLength(0)];
            Array.Copy(formSection, result, formSection.GetLength(0));

            int count128 = 0;
            int count428 = 0;

            // Есть регионы, которые формируют собственные отчет МО, субъекта и внебюджетных фондов по 128 форме, 
            // а консолидированный отчет по 428.
            // В xml-файле консолидированный отчет может быть представлен одновременно в 128 и 428 формах, 
            // только в 128 форме или только в 428 форме.

            for (int i = 0; i < result.GetLength(0); i += 2)
            {
                XmlForm[] xmlForms = StringToXmlForms(result[i]);
                for (int j = 0; j < xmlForms.GetLength(0); j++)
                {
                    int count = GetTotalDataNodes(xn, new string[] { result[i], result[i + 1] });

                    switch (xmlForms[j])
                    {
                        case XmlForm.Form128:
                        case XmlForm.Form128v:
                            count128 += count;
                            break;

                        case XmlForm.Form428:
                        case XmlForm.Form428g:
                        case XmlForm.Form428Vg:
                        case XmlForm.Form428v:
                            count428 += count;
                            break;
                    }
                }
            }

            for (int i = 0; i < result.GetLength(0); i += 2)
            {
                // нет данных по форме 128
                if (count428 == 0)
                {
                    result[i] = result[i].Replace("428v", string.Empty);
                    result[i] = result[i].Replace("428", string.Empty);
                }
                else
                    // в 428 данные есть, из 128 качать не нужно
                    count128 = 0;
                // нет данных по форме 128
                if (count128 == 0)
                {
                    result[i] = result[i].Replace("128v", string.Empty);
                    result[i] = result[i].Replace("128", string.Empty);
                }

                result[i] = result[i].Replace(";;", ";").Trim(';');
            }

            return result;
        }

        /// <summary>
        /// получить строковый тип документа
        /// </summary>
        /// <param name="docType"> тип документа </param>
        /// <returns></returns>
        public string GetSKIFDocType(int docType)
        {
            switch (docType)
            {
                case 0:
                    return "Значение не указано";
                case 1:
                    return "Неуказанный тип отчетности";
                case 2:
                    return "Собственная отчетность муниципальных образований";
                case 3:
                    return "Консолидированная отчетность и отчетность внебюджетных территориальных фондов";
                case 4:
                    return "Отчетность главных распорядителей средств бюджета";
                case 5:
                    return "Собственный отчет по бюджету субъекта";
                case 9:
                    return "Консолидированный отчет муниципальных образований";
                case 10:
                    return "Консолидированный отчет муниципального района";
                case 12:
                    return "Отчет внебюджетного фонда";
                case 20:
                    return "Данные в разрезе муниципальных образований";
                case 21:
                    return "Данные в разрезе муниципальных образований и поселений";
                default: 
                    return string.Empty;
            }
        }

        private void FormCLSFromReport(string code, BlockProcessModifier blockProcessModifier, Dictionary<string, int> cache,
            DataTable[] clsTable, IClassifier[] cls)
        {
            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRIncomes:
                    if (this.DataSource.Year >= 2005)
                        PumpCachedRow(cache, clsTable[0], cls[0], code, 
                            new object[] { "CODESTR", code, "NAME", "Неуказанное наименование", "KL", 0, "KST", 0 });
                    break;
                case BlockProcessModifier.MRSrcInFin:
                    if (this.DataSource.Year >= 2005)
                        PumpCachedRow(cache, clsTable[0], cls[0], code, 
                            new object[] { "CODESTR", code, "NAME", "Неуказанное наименование", "KL", 0, "KST", 0 });
                    break;
                case BlockProcessModifier.MRSrcOutFin:
                    if (this.DataSource.Year >= 2005)
                        PumpCachedRow(cache, clsTable[0], cls[0], code, 
                            new object[] { "CODESTR", code, "NAME", "Неуказанное наименование", "KL", 0, "KST", 0 });
                    break;
            }
        }

        // если данные по этому району уже закачаны, причем из более приоритетной формы - данные по этой форме пропускаем (return false)
        private bool CheckFormPriority(string regionKey, XmlForm form)
        {
            switch (form.ToString().ToUpper())
            {
                case "FORM127":
                case "FORM127G":
                case "FORM127V":
                    // 127 форма - самая приоритетная, качаем всегда
                    if (!forcePumpForm127)
                        return true;
                    else
                        // если принудительная закачка 127 формы, то качаем, если по району не закачаны никакие данные
                        return (!pumpedRegions.ContainsKey(regionKey) || (pumpedRegions[regionKey].Contains(XmlForm.Form127) ||
                                                                          pumpedRegions[regionKey].Contains(XmlForm.Form127v) ||
                                                                          pumpedRegions[regionKey].Contains(XmlForm.Form127g)));
                case "FORM628R":
                case "FORM428":
                case "FORM428V":
                case "FORM428G":
                case "FORM428VG":
                case "FORM128":
                case "FORM128V":
                    // из этих форм качаем, если не было в 127
                    if (!pumpedRegions.ContainsKey(regionKey))
                        return true;
                    return (!pumpedRegions[regionKey].Contains(XmlForm.Form127) && !pumpedRegions[regionKey].Contains(XmlForm.Form127v) &&
                            !pumpedRegions[regionKey].Contains(XmlForm.Form127g));
                case "FORM117":
                    // из 117 качаем, если данных не было в пред формах (127, 128, 428, 628)
                    return (!pumpedRegions.ContainsKey(regionKey) || (pumpedRegions[regionKey].Contains(XmlForm.Form117)));
            }
            // остальные формы в проверке на приоритетность не нуждаются, качаем
            return true;
        }

        private int CleanIntValue(string value)
        {
            value = CommonRoutines.TrimLetters(value.Trim()).Trim();
            return Convert.ToInt32(value.PadLeft(1, '0'));
        }

        // закачка данных из одного документа
        private void PumpXMLData(XmlNodeList xnlData, IDbDataAdapter da, DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, string[] factRefsToCls, int[] nullRefsToCls,
            Dictionary<string, int>[] codesMapping, string[] codeExcl, BlockProcessModifier blockProcessModifier,
            string clsCodeAttr, int nullRegions, string[] attr2ClsMapping, string date, int yearIndex,
            object[] clsValuesMapping, string clsCodeField, string regionCode, string regionKey, int regionID,
            XmlForm xmlForm, object[] individualCodesMapping, int vbAttrValue)
        {
            List<XmlForm> formsList = null;
            if (!pumpedRegions.ContainsKey(regionKey))
            {
                formsList = new List<XmlForm>();
                pumpedRegions.Add(regionKey, formsList);
            }
            else
                formsList = pumpedRegions[regionKey];
            if (!formsList.Contains(xmlForm))
                formsList.Add(xmlForm);

            bool toPumpOutcomesRefs = true;
            bool toPumpInDebtRefs = false;
            bool toPumpOutDebtRefs = false;
            bool toPumpArrearsRefs = false;
            bool toPumpExcessRefs = false;

            if (this.DataSource.Year >= 2010)
                toPumpOutcomesRefs = false;

            int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
            for (int k = 0; k < xnlData.Count; k++)
            {
                string clsCode = GetClsCodeFromFactNode(clsCodeAttr, xnlData[k], blockProcessModifier);

                #region
                if (this.DataSource.Year >= 2008)
                {
                    decimal clsCodeInt = 0;
                    if (this.DataSource.Year < 2010)
                    {
                        if ((blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx) ||
                            (blockProcessModifier == BlockProcessModifier.MRCommonBooks) ||
                            (blockProcessModifier == BlockProcessModifier.MRExcessBooks))
                            clsCodeInt = Convert.ToDecimal(clsCode.TrimStart('0').PadLeft(1, '0'));
                    }
                    switch (blockProcessModifier)
                    {
                        case BlockProcessModifier.MROutcomesBooksEx:
                            //if ((this.DataSource.Year < 2010) && (sectNoList[0] != 2))
                            //   break;
                            if (this.DataSource.Year >= 2009)
                            {
                                // 2009 - закачиваем до кода 0000000000000000000009700
                                if (clsCode == "0000000000000000000009700")
                                    toPumpOutcomesRefs = false;
                                // c апреля 2009 года - Показатели.МесОтч_СпрРасходы  (<;10100, 0000000000000000022511400;>]
                                if (sourceDate >= 200904)
                                {
                                    if (clsCode == "0000000000000000022511400")
                                        toPumpOutcomesRefs = true;
                                }
                            }
                            // 2008 - закачиваем до кода 000000000000000000008200
                            else
                            {
                                if (clsCode == "000000000000000000008200")
                                    toPumpOutcomesRefs = false;
                            }
                            //   if (!toPumpOutcomesRefs)
                            //      continue;
                            break;
                        case BlockProcessModifier.MRCommonBooks:
                        case BlockProcessModifier.MRExcessBooks:
                            if (factRefsToCls[0] == "REFMARKSINDEBT")
                            {
                                if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 - закачиваем коды [0000000000000000000009700,0000000000000000000009800)
                                    // [0000000000000000000009900, 0000000000000000000010000)
                                    toPumpInDebtRefs = (((clsCodeInt >= 9700) && (clsCodeInt < 9800)) ||
                                                        ((clsCodeInt >= 9900) && (clsCodeInt < 10000)));
                                }
                                else
                                {
                                    // 2008 - закачиваем коды [000000000000000000008200,000000000000000000008300)
                                    // [000000000000000000008400, 000000000000000000008500)
                                    toPumpInDebtRefs = (((clsCodeInt >= 8200) && (clsCodeInt < 8300)) ||
                                                        ((clsCodeInt >= 8400) && (clsCodeInt < 8500)));
                                }
                                //        if (!toPumpInDebtRefs)
                                //           continue;
                            }
                            else if (factRefsToCls[0] == "REFMARKSOUTDEBT")
                            {
                                if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 - качаем коды [0000000000000000000009800, 0000000000000000000009900)
                                    toPumpOutDebtRefs = ((clsCodeInt >= 9800) && (clsCodeInt < 9900));
                                }
                                else
                                {
                                    // 2008 - качаем коды [000000000000000000008300, 000000000000000000008400)
                                    toPumpOutDebtRefs = ((clsCodeInt >= 8300) && (clsCodeInt < 8400));
                                }
                                //        if (!toPumpOutDebtRefs)
                                //           continue;
                            }
                            else if (factRefsToCls[0] == "REFMARKSARREARS")
                            {
                                // 2009 - качаем после 0000000000000000000010100 (включая)
                                if (this.DataSource.Year >= 2009)
                                {
                                    if (clsCode == "0000000000000000000010100")
                                        toPumpArrearsRefs = true;
                                    // c апреля 2009 года - Показатели.МесОтч_СпрЗадолженность  [0000000000000000000010100, 22511400)
                                    if (sourceDate >= 200904)
                                    {
                                        if (clsCode == "0000000000000000022511400")
                                            toPumpArrearsRefs = false;
                                    }
                                }
                                else
                                    // 2008 - качаем после 000000000000000000008600 (включая)
                                    if (clsCode == "000000000000000000008600")
                                        toPumpArrearsRefs = true;
                                //  if (!toPumpArrearsRefs)
                                //     continue;
                            }
                            else if (factRefsToCls[0] == "RefMarks")
                            {
                                if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 - качаем коды [0000000000000000000010000, 0000000000000000000010100)
                                    toPumpExcessRefs = ((clsCodeInt >= 10000) && (clsCodeInt < 10100));
                                }
                                else
                                {
                                    // 2008 - качаем коды [000000000000000000008500, 000000000000000000008600)
                                    toPumpExcessRefs = ((clsCodeInt >= 8500) && (clsCodeInt < 8600));
                                }
                                //     if (!toPumpExcessRefs)
                                //        continue;
                            }
                            break;
                    }
                }
                else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                {
                    if (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx)
                    {
                        // закачиваем до кода 000000000000000000007000
                        if (clsCode == "000000000000000000007000")
                            toPumpOutcomesRefs = false;
                        if (!toPumpOutcomesRefs)
                            continue;
                    }
                    else if (factRefsToCls != null)
                    {
                        if (factRefsToCls[0] == "REFMARKSARREARS")
                        {
                            // качаем после 000000000000000000007000 (включая)
                            if (clsCode == "000000000000000000007000")
                                toPumpArrearsRefs = true;
                            if (!toPumpArrearsRefs)
                                continue;
                        }
                    }
                }
                #endregion

                // для 117 формы - для блоков финансирования, если нет указанного атрибута - пропускаем запись
                switch (blockProcessModifier)
                {
                    case BlockProcessModifier.MRSrcOutFin:
                    case BlockProcessModifier.MRSrcInFin:
                        if ((xmlForm == XmlForm.Form117) && (clsCode == string.Empty))
                            continue;
                        break;
                }

                switch (xmlForm)
                {
                    case XmlForm.Form128:
                    case XmlForm.Form128v:
                        if (this.skifFormat == SKIFFormat.MonthReports)
                        {
                            // Часть кода классификаторов находится в родительском элементе
                            clsCode = GetClsCodeFromFactNode(
                                clsCodeAttr, xnlData[k].ParentNode, blockProcessModifier).Remove(0, 2) + clsCode;
                        }
                        break;
                }

                if (this.DataSource.Year < 2005)
                    if ((blockProcessModifier == BlockProcessModifier.MROutcomesBooks) ||
                        (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx))
                    {
                        string psr = clsCode.Substring(clsCode.Length - 6);
                        string psrCodes = "110100,110200,110310,110330,110700,130100,130200,130300,240110";
                        if (blockProcessModifier == BlockProcessModifier.MROutcomesBooks)
                            if (!psrCodes.Contains(psr))
                                continue;
                        if (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx)
                            if (psrCodes.Contains(psr))
                                continue;
                    }

                if (CheckCodeExclusion(clsCode, codeExcl))
                    continue;

                sumMultiplier = 1;
                #region формируем код классификатора из отчета
                if (clsCode != string.Empty)
                    switch (blockProcessModifier)
                    {
                        case BlockProcessModifier.MROutcomesBooksEx:
                        case BlockProcessModifier.MRExcessBooks:
                            if (this.DataSource.Year >= 2010)
                                clsCode = clsCode.Remove(5, 3);
                            break;
                        case BlockProcessModifier.MRCommonBooks:
                            if (this.DataSource.Year >= 2010)
                            {
                                if ((factRefsToCls[0] == "REFMARKSARREARS") ||
                                (factRefsToCls[0] == "REFMARKSINDEBT") ||
                                (factRefsToCls[0] == "REFMARKSOUTDEBT"))
                                    clsCode = clsCode.Remove(5, 3);
                            }
                            break;
                        case BlockProcessModifier.MRIncomes:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            // букавки в коде приводим к английской
                            clsCode = clsCode.Replace('А', 'A');
                            clsCode = clsCode.Replace('а', 'a');
                            FormCLSFromReport(clsCode, blockProcessModifier, codesMapping[0], clsTable, cls);
                            break;
                        case BlockProcessModifier.MRSrcInFin:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            // букавки в коде приводим к английской
                            clsCode = clsCode.Replace('А', 'A');
                            clsCode = clsCode.Replace('а', 'a');
                            if (GetAttrValueByName(xnlData[k].Attributes, "АдмКИВФ", "КИВФ", "ClsfCode") != string.Empty)
                                FormCLSFromReport(clsCode, blockProcessModifier, codesMapping[0], clsTable, cls);
                            break;
                        case BlockProcessModifier.MRSrcOutFin:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            // букавки в коде приводим к английской
                            clsCode = clsCode.Replace('А', 'A');
                            clsCode = clsCode.Replace('а', 'a');
                            if (GetAttrValueByName(xnlData[k].Attributes, "АдмКИВнФ", "КИВнФ", "ClsfCode") != string.Empty)
                                FormCLSFromReport(clsCode, blockProcessModifier, codesMapping[0], clsTable, cls);
                            break;
                        case BlockProcessModifier.YRSrcFin:
                        case BlockProcessModifier.YRIncomes:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            break;
                        case BlockProcessModifier.YRNet:
                            // фкр
                            int auxCode = Convert.ToInt32(clsCode.Substring(0, 4));
                            clsValuesMapping[1] = PumpCachedRow(codesMapping[0], clsTable[0], cls[0], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // кцср
                            auxCode = Convert.ToInt32(clsCode.Substring(4, 7));
                            clsValuesMapping[3] = PumpCachedRow(codesMapping[1], clsTable[1], cls[1], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // квр
                            auxCode = Convert.ToInt32(clsCode.Substring(11, 3));
                            clsValuesMapping[5] = PumpCachedRow(codesMapping[2], clsTable[2], cls[2], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // ксшк
                            auxCode = Convert.ToInt32(clsCode.Substring(16, 3));
                            if ((auxCode == 850) || ((auxCode >= 500) && (auxCode < 700)))
                                sumMultiplier = 1000;
                            else
                                sumMultiplier = 1;

                            clsValuesMapping[7] = PumpCachedRow(codesMapping[3], clsTable[3], cls[3], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // субквр - код = 16 первых символов изначального кода
                            string subKvrCode = clsCode.Substring(0, 16);
                            // меняем английские х на русские
                            subKvrCode = subKvrCode.ToUpper().Replace('X', 'Х');
                            clsValuesMapping[9] = FindCachedRow(codesMapping[4], subKvrCode, nullRefsToCls[4]);
                            if (Convert.ToInt32(clsValuesMapping[9]) == nullRefsToCls[4])
                            {
                                // меняем русские х на английские
                                subKvrCode = subKvrCode.ToUpper().Replace('Х', 'X');
                                clsValuesMapping[9] = FindCachedRow(codesMapping[4], subKvrCode, nullRefsToCls[4]);
                            }
                            break;
                        case BlockProcessModifier.MRArrears:
                            clsValuesMapping[1] = FindCachedRow(codesMapping[0], clsCode, nullRefsToCls[0]);
                            break;
                        case BlockProcessModifier.YRBalanc:
                            if ((xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002) || (xmlForm == XmlForm.Form43002))
                            {
                                // для блока "Баланс_Справка" если в классификаторе нет записи с соответствующим кодом,
                                // то добавляем его в таблицу классификатора с неуказанным наименованием
                                clsCode = Convert.ToInt32(clsCode.Trim()).ToString();
                                if (!codesMapping[0].ContainsKey(clsCode))
                                {
                                    clsValuesMapping[1] = PumpCachedRow(codesMapping[0], clsTable[0], cls[0],
                                        clsCode, new object[] { "Code", clsCode, "Name", constDefaultClsName });
                                }
                            }
                            else
                            {
                                clsValuesMapping[1] = FindCachedRow(codesMapping[0], clsCode, nullRefsToCls[0]);
                            }
                            break;
                    }
                #endregion

                #region получаем ссылки на фкр и экр
                if (((xmlForm == XmlForm.Form117) || ((xmlForm == XmlForm.Form127) ||
                     (xmlForm == XmlForm.Form127g) || (xmlForm == XmlForm.Form127v))) &&
                    (blockProcessModifier == BlockProcessModifier.MROutcomes))
                {
                    int fkr = CleanIntValue(xnlData[k].ParentNode.Attributes["РзПр"].Value);
                    int kcsr = CleanIntValue(xnlData[k].ParentNode.Attributes["ЦСР"].Value);
                    int kvr = CleanIntValue(xnlData[k].ParentNode.Attributes["ВР"].Value);
                    string name = constDefaultClsName;
                    if (kvr != 0)
                    {
                        if (kvrAuxCache.ContainsKey(kvr))
                            name = kvrAuxCache[kvr];
                    }
                    else if (kcsr != 0)
                    {
                        if (kcsrAuxCache.ContainsKey(kcsr))
                            name = kcsrAuxCache[kcsr];
                    }
                    else if (fkr != 0)
                    {
                        if (fkrAuxCache.ContainsKey(fkr))
                            name = fkrAuxCache[fkr];
                    }
                    if (name.Trim() == string.Empty)
                        name = constDefaultClsName;

                    string fkrCode = string.Format("{0}{1}{2}", fkr, kcsr.ToString().PadLeft(7, '0'),
                        kvr.ToString().PadLeft(3, '0'));
                    clsValuesMapping[1] = PumpCachedRow(codesMapping[0], clsTable[0], cls[0], fkrCode,
                        new object[] { "Code", fkrCode, "Name", name });

                    string ekr = clsCode;
                    clsValuesMapping[3] = PumpCachedRow(codesMapping[1], clsTable[1], cls[1], ekr,
                        new object[] { "Code", ekr, "Name", constDefaultClsName });
                    if (xmlForm != XmlForm.Form117)
                    {
                        string kvsr = xnlData[k].ParentNode.Attributes["Адм"].Value.TrimStart('0').PadLeft(1, '0');
                        clsValuesMapping[5] = FindCachedRow(codesMapping[2], kvsr, nullRefsToCls[2]);
                    }
                }
                else
                {
                    if (blockProcessModifier != BlockProcessModifier.YRNet)
                        if (!GetClsIDForFact(clsTable, cls, factRefsToCls, nullRefsToCls, codesMapping,
                            blockProcessModifier, yearIndex, clsValuesMapping, clsCodeField, clsCode, attr2ClsMapping))
                            continue;
                }
                #endregion

                // для задолженностей и справ расходы доп, неопределенных ссылок не должно быть
                if ((blockProcessModifier == BlockProcessModifier.MRCommonBooks) ||
                    (blockProcessModifier == BlockProcessModifier.MRExcessBooks) ||
                    (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx))
                    if (Convert.ToInt32(clsValuesMapping[1]) == nullRefsToCls[0])
                        continue;

                // Закачиваем данные по этому классификатору
                ProcessPxNodes(da, factTable, fct, xnlData[k], xmlForm, regionID, date, regionCode,
                    (object[])CommonRoutines.ConcatArrays(clsValuesMapping, individualCodesMapping),
                    blockProcessModifier, nullRegions, vbAttrValue);

                // Сохраняем данные, если нужно
                if (factTable.Rows.Count >= constMaxQueryRecords)
                {
                    UpdateData();
                    ClearDataSet(da, factTable);
                }
            }
        }

        private string GetReportDate()
        {
            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    return string.Format("{0}{1:00}00", this.DataSource.Year, this.DataSource.Month);

                case SKIFFormat.YearReports:
                    return string.Format("{0}", this.DataSource.Year);
            }
            return string.Empty;
        }

        private int GetArrearsYear(XmlNode document)
        {
            string year = GetAttrValueByName(document.Attributes, "Год");
            return Convert.ToInt32(year.Trim().PadLeft(1, '0'));
        }

        private int GetBalancVB(XmlNode document, XmlForm xmlForm)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form13001) ||
                (xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002))
            {
                return Convert.ToInt32(GetAttrValueByName(document.Attributes, "ВБ").Trim().PadLeft(1, '0'));
            }
            return -1;
        }

        /// <summary>
        /// Закачивает данные блока отчета
        /// </summary>
        /// <param name="blockName">Название блока (для прогресса)</param>
        /// <param name="xn">Узел отчета</param>
        /// <param name="formSection">Массив пар номер_формы-номер_секции. Каждая пара определяет, даыные из каких
        /// секций и форм будут закачаны (соответствие секций формам). Номера форм должны быть перечислены через ";".
        /// Формат номер_секции:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в
        /// варианте 2)</param>
        /// <param name="da">ДатаАдаптер таблицы фактов</param>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="consDa">ДатаАдаптер таблицы фактов конс. отчетов</param>
        /// <param name="consFactTable">Таблица фактов конс. отчетов</param>
        /// <param name="consFct">IFactTable конс. отчетов</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="clsYears">Года, определяющие в данные какого года в какой классификатор из clsTable качать</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="nullRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable, равные null</param>
        /// <param name="progressMsg">Сообщение прогресса</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Исключения кода классификатора (указанные коды не закачиваются).
        /// Массив пар Rule - CodePart, где CodePart - число символов кода, к которому будет применено правило
        /// исключения Rule. Если присутствует только Rule, т.е. массив содержит 1 элемент, то это правило будет
        /// применено ко всему коду. Если код удовлетворяет хотя бы одному из правил, то он будет исключен.
        /// Формат элементов массива:
        /// "rule1;rule2" - несколько правил, описывающих исключения, могут быть перечислены через точку с запятой, объединяются по И;
        /// Формат чисел см. в PumpComplexClsFromInternalPatternXML.AttrValuesMapping.
        /// Формат правил:
        /// "code" - исключаются коды, равные указанному;
        /// "code*" - исключаются коды, начинающиеся с указанного;
        /// "*code" - исключаются коды, заканчивающиеся на указанный;
        /// "code1..code2" - исключаются коды, входящие в диапазон code1..code2;
        /// ">=code" - исключаются коды >= code;
        /// "code" - исключаются коды, меньшие или равные code (перед code знак меньше-равно нужно ставить, а
        /// здесь комментарии не позволяют :).
        /// Префиксы правил:
        /// "!" - отрицание; "#" - превалирующее правило (если код ему не удовлетворяет, то он не будет пропущен
        /// вне зависимости от других правил; это правило должно быть первым)</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков.</param>
        /// <param name="clsCodeAttr">Наименование атрибута с кодом классификатора. Пустая строка - конкатенация
        /// всех атрибутов, "attrName+attrName" - будет произведена конкатенация значений указанных атрибутов.</param>
        /// <param name="regionsCache">Кэш классификатора Районы</param>
        /// <param name="nullRegions">ИД ссылки на неизвестные данные классификатора Районы</param>
        /// <param name="attr2ClsMapping"> Список количество_символов объединенного кода классификатора, откуда будут браться
        /// значения для указанных выше классификаторов.
        /// Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        /// <param name="regions4PumpSkifCache">Кэш классификатора Районы.Служебный</param>
        /// <param name="regionsUnitCache">Кэш классификатора Районы для конс. отчетов</param>
        /// <param name="nullRegionsUnit">ИД ссылки на неизвестные данные классификатора Районы для конс. отчетов</param>
        protected void PumpXMLReportBlock(string blockName, XmlNode xn, string[] formSection, IDbDataAdapter da,
            DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls,
            string progressMsg, Dictionary<string, int>[] codesMapping, Dictionary<XmlForm, string[]> codeExclusions,
            BlockProcessModifier blockProcessModifier, string clsCodeAttr, Dictionary<string, int> regionsCache,
            int nullRegions, string[] attr2ClsMapping, Dictionary<string, int> regions4PumpSkifCache)
        {
            #region Узнаем общее количество элементов данных
            int totalDataNodes = GetTotalDataNodes(xn, formSection);
            WriteToTrace(string.Format("{0} элементов Data.", totalDataNodes), TraceMessageKind.Information);
            if (totalDataNodes == 0)
                return;
            #endregion

            // формируем дату
            string date = GetReportDate();

            // Определяем индекс года текущего источника в массиве всех лет.
            // Нужно для того, чтобы определить, что брать из других массивов.
            int yearIndex = GetYearIndexByYear(clsYears);

            #region Формируем массив значений ссылок на классификаторы и инициализируем его ссылками на Неизвестные данные
            object[] clsValuesMapping = null;
            if (factRefsToCls != null)
            {
                clsValuesMapping = new object[factRefsToCls.GetLength(0) * 2];
                for (int i = 0; i < factRefsToCls.GetLength(0); i++)
                {
                    clsValuesMapping[i * 2] = factRefsToCls[i];
                    clsValuesMapping[i * 2 + 1] = nullRefsToCls[i];
                }
            }
            #endregion

            // Определяем название поля кода классификатора
            string clsCodeField = string.Empty;
            if (yearIndex >= 0 && cls != null)
            {
                clsCodeField = GetClsCodeField(cls[yearIndex]);
            }

            XmlNodeList xnlSources = xn.SelectNodes("//Source");

            for (int i = 0; i < xnlSources.Count; i++)
            {
                string regionCode = GetAttrValueByName(xnlSources[i].Attributes, "ObjectNmb", "Code").PadLeft(10, '0');
                string regionName = GetAttrValueByName(xnlSources[i].Attributes, new string[] { "Name", "ObjectName" });
                string regionKey = regionCode + "|" + regionName;
                string classCode = GetAttrValueByName(xnlSources[i].Attributes, "ClassKey", "ClassCode").ToUpper().Trim();
                int regionID = FindCachedRow(regionsCache, regionKey, nullRegions);

                string[] correctedFormSection = (string[])formSection.Clone();
                //if (this.SkifReportFormat == SKIFFormat.MonthReports)
                correctedFormSection = CheckFormForPump(xnlSources[i], formSection);

                for (int f = 0; f < correctedFormSection.GetLength(0); f += 2)
                {
                    // Массив форм отчета
                    XmlForm[] xmlForm = StringToXmlForms(correctedFormSection[f]);
                    // Массив секций форм
                    int[] sectNoList = CommonRoutines.ParseParamsString(correctedFormSection[f + 1]);

                    for (int j = 0; j < xmlForm.GetLength(0); j++)
                    {
                        if (xmlForm[j] == XmlForm.UnknownForm)
                            continue;

                        #region кусочек - полный пиздец
                        // так как постановка изъябнулась по полной - или так или ваще логику закачки переписывать
                        // связано с приоритетами в зависимости от класс кодов...
                        if (xmlForm[j].ToString().ToUpper().StartsWith("FORM127"))
                        {
                            bool isFirstGroupClassCodes =
                                ((classCode == "ГР") || (classCode == "ГР1") || (classCode == "ГРБС") ||
                                (classCode == "ДМС") || (classCode == "БЮДЖ") || (classCode == "ГРБСКК") ||
                                (classCode == "ДЕЗ (ЖКХ)") || (classCode == "МНЦП СМЕТА") || (classCode == "ДЕПАРТАМЕНТ ФИНАНСОВ") ||
                                (classCode == "МИВКЛМК") || (classCode == "ОБРАЗОВАН"));
                            if (!forcePumpForm127)
                            {
                                // первая закачка 127 формы - качаем только у класс кодов "первой группы"
                                if (!isFirstGroupClassCodes)
                                    continue;
                            }
                            else
                            {
                                // вторая закачка 127 формы - качаем только у класс кодов "второй группы"
                                if (isFirstGroupClassCodes)
                                    continue;
                            }
                        }
                        #endregion

                        if (!CheckFormPriority(regionKey, xmlForm[j]))
                            continue;

                        object[] individualCodesMapping = PrepareIndividualCodesMappingXML(xmlForm[j], blockProcessModifier);

                        string[] codeExcl = GetCodesExclusions4XmlForm(codeExclusions, xmlForm[j]);

                        // Получаем данные XML
                        XmlNodeList documents = GetFactDocumentNodes(xnlSources[i], sectNoList, new XmlForm[] { xmlForm[j] });
                        foreach (XmlNode document in documents)
                        {
                            XmlNodeList xnlData = document.SelectNodes("Data");
                            if (xnlData.Count == 0)
                                continue;

                            #region получаем год возникновения (для MRArrears)
                            if (blockProcessModifier == BlockProcessModifier.MRArrears)
                            {
                                individualCodesMapping = (object[])CommonRoutines.ConcatArrays(individualCodesMapping,
                                    new object[] { "Year", GetArrearsYear(document) });
                            }
                            #endregion
                            #region получаем значение атрибута ВБ (для YRBalanc)
                            int vbAttrValue = -1;
                            if (blockProcessModifier == BlockProcessModifier.YRBalanc)
                            {
                                vbAttrValue = GetBalancVB(document, xmlForm[j]);
                            }
                            #endregion

                            PumpXMLData(xnlData, da, factTable, fct, clsTable, cls, factRefsToCls, nullRefsToCls,
                                codesMapping, codeExcl, blockProcessModifier, clsCodeAttr, nullRegions, attr2ClsMapping,
                                date, yearIndex, clsValuesMapping, clsCodeField, regionCode, regionKey,
                                regionID, xmlForm[j], individualCodesMapping, vbAttrValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Закачивает данные блока отчета
        /// </summary>
        /// <param name="blockName">Название блока (для прогресса)</param>
        /// <param name="xn">Узел отчета</param>
        /// <param name="formSection">Массив пар номер_формы-номер_секции. Каждая пара определяет, даыные из каких
        /// секций и форм будут закачаны (соответствие секций формам). Номера форм должны быть перечислены через ";".
        /// Формат номер_секции:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в
        /// варианте 2)</param>
        /// <param name="da">ДатаАдаптер таблицы фактов</param>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="consDa">ДатаАдаптер таблицы фактов конс. отчетов</param>
        /// <param name="consFactTable">Таблица фактов конс. отчетов</param>
        /// <param name="consFct">IFactTable конс. отчетов</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="clsYears">Года, определяющие в данные какого года в какой классификатор из clsTable качать</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="nullRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable, равные null</param>
        /// <param name="progressMsg">Сообщение прогресса</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Исключения кода классификатора (указанные коды не закачиваются).
        /// Массив пар Rule - CodePart, где CodePart - число символов кода, к которому будет применено правило
        /// исключения Rule. Если присутствует только Rule, т.е. массив содержит 1 элемент, то это правило будет
        /// применено ко всему коду. Если код удовлетворяет хотя бы одному из правил, то он будет исключен.
        /// Формат элементов массива:
        /// "rule1;rule2" - несколько правил, описывающих исключения, могут быть перечислены через точку с запятой, объединяются по И;
        /// Формат чисел см. в PumpComplexClsFromInternalPatternXML.AttrValuesMapping.
        /// Формат правил:
        /// "code" - исключаются коды, равные указанному;
        /// "code*" - исключаются коды, начинающиеся с указанного;
        /// "*code" - исключаются коды, заканчивающиеся на указанный;
        /// "code1..code2" - исключаются коды, входящие в диапазон code1..code2;
        /// ">=code" - исключаются коды >= code;
        /// "code" - исключаются коды, меньшие или равные code (перед code знак меньше-равно нужно ставить, а
        /// здесь комментарии не позволяют :).
        /// Префиксы правил:
        /// "!" - отрицание; "#" - превалирующее правило (если код ему не удовлетворяет, то он не будет пропущен
        /// вне зависимости от других правил; это правило должно быть первым)</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков.</param>
        /// <param name="clsCodeAttr">Наименование атрибута с кодом классификатора. Пустая строка - конкатенация
        /// всех атрибутов, "attrName+attrName" - будет произведена конкатенация значений указанных атрибутов.</param>
        /// <param name="regionsCache">Кэш классификатора Районы</param>
        /// <param name="nullRegions">ИД ссылки на неизвестные данные классификатора Районы</param>
        /// <param name="attr2ClsMapping"> Список количество_символов объединенного кода классификатора, откуда будут браться
        /// значения для указанных выше классификаторов.
        /// Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        /// <param name="regions4PumpSkifCache">Кэш классификатора Районы.Служебный</param>
        /// <param name="regionsUnitCache">Кэш классификатора Районы для конс. отчетов</param>
        /// <param name="nullRegionsUnit">ИД ссылки на неизвестные данные классификатора Районы для конс. отчетов</param>
        protected void PumpXMLReportBlock(string blockName, XmlNode xn, string[] formSection, IDbDataAdapter da,
            DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls,
            string progressMsg, Dictionary<string, int>[] codesMapping, string[] codeExclusions,
            BlockProcessModifier blockProcessModifier, string clsCodeAttr, Dictionary<string, int> regionsCache,
            int nullRegions, string[] attr2ClsMapping, Dictionary<string, int> regions4PumpSkifCache)
        {
            Dictionary<XmlForm, string[]> codesExcl = new Dictionary<XmlForm,string[]>(1);
            codesExcl.Add(XmlForm.UnknownForm, codeExclusions);

            PumpXMLReportBlock(blockName, xn, formSection, da, factTable, fct,
                clsTable, cls, clsYears, factRefsToCls, nullRefsToCls, progressMsg, codesMapping, codesExcl,
                blockProcessModifier, clsCodeAttr, regionsCache, nullRegions, attr2ClsMapping, regions4PumpSkifCache);
        }

        /// <summary>
        /// Закачивает данные блока отчета
        /// </summary>
        /// <param name="blockName">Название блока (для прогресса)</param>
        /// <param name="xn">Узел отчета</param>
        /// <param name="formSection">Массив пар номер_формы-номер_секции. Каждая пара определяет, даыные из каких
        /// секций и форм будут закачаны (соответствие секций формам). Номера форм должны быть перечислены через ";".
        /// Формат номер_секции:
        /// "sectNo" - закачиваются данные указанной секции;
        /// "sectNo1;sectNo2;..." - закачиваются данные sectNo1 и sectNo2;
        /// "sectNo1..sectNo2" - закачиваются данные секций с sectNo1 по sectNo2 (возможно использование в
        /// варианте 2)</param>
        /// <param name="da">ДатаАдаптер таблицы фактов</param>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="consDa">ДатаАдаптер таблицы фактов конс. отчетов</param>
        /// <param name="consFactTable">Таблица фактов конс. отчетов</param>
        /// <param name="consFct">IFactTable конс. отчетов</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="clsYears">Года, определяющие в данные какого года в какой классификатор из clsTable качать</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="nullRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable, равные null</param>
        /// <param name="progressMsg">Сообщение прогресса</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Исключения кода классификатора (указанные коды не закачиваются).
        /// Массив пар Rule - CodePart, где CodePart - число символов кода, к которому будет применено правило
        /// исключения Rule. Если присутствует только Rule, т.е. массив содержит 1 элемент, то это правило будет
        /// применено ко всему коду. Если код удовлетворяет хотя бы одному из правил, то он будет исключен.
        /// Формат элементов массива:
        /// "rule1;rule2" - несколько правил, описывающих исключения, могут быть перечислены через точку с запятой, объединяются по И;
        /// Формат чисел см. в PumpComplexClsFromInternalPatternXML.AttrValuesMapping.
        /// Формат правил:
        /// "code" - исключаются коды, равные указанному;
        /// "code*" - исключаются коды, начинающиеся с указанного;
        /// "*code" - исключаются коды, заканчивающиеся на указанный;
        /// "code1..code2" - исключаются коды, входящие в диапазон code1..code2;
        /// ">=code" - исключаются коды >= code;
        /// "code" - исключаются коды, меньшие или равные code (перед code знак меньше-равно нужно ставить, а
        /// здесь комментарии не позволяют :).
        /// Префиксы правил:
        /// "!" - отрицание; "#" - превалирующее правило (если код ему не удовлетворяет, то он не будет пропущен
        /// вне зависимости от других правил; это правило должно быть первым)</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных
        /// операций для какого-либо из блоков.</param>
        /// <param name="clsCodeAttr">Наименование атрибута с кодом классификатора. Пустая строка - конкатенация
        /// всех атрибутов, "attrName+attrName" - будет произведена конкатенация значений указанных атрибутов.</param>
        /// <param name="regionsCache">Кэш классификатора Районы</param>
        /// <param name="nullRegions">ИД ссылки на неизвестные данные классификатора Районы</param>
        /// <param name="attr2ClsMapping"> Список количество_символов объединенного кода классификатора, откуда будут браться
        /// значения для указанных выше классификаторов.
        /// Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        /// <param name="regions4PumpSkifCache">Кэш классификатора Районы.Служебный</param>
        /// <param name="regionsUnitCache">Кэш классификатора Районы для конс. отчетов</param>
        /// <param name="nullRegionsUnit">ИД ссылки на неизвестные данные классификатора Районы для конс. отчетов</param>
        protected void PumpXMLReportBlock(string blockName, XmlNode xn, string[] formSection, IDbDataAdapter da,
            DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls,
            string progressMsg, Dictionary<string, int>[] codesMapping, BlockProcessModifier blockProcessModifier,
            string clsCodeAttr, Dictionary<string, int> regionsCache, int nullRegions, string[] attr2ClsMapping,
            Dictionary<string, int> regions4PumpSkifCache)
        {
            PumpXMLReportBlock(blockName, xn, formSection, da, factTable, fct,
                clsTable, cls, clsYears, factRefsToCls, nullRefsToCls, progressMsg, codesMapping, new string[0],
                blockProcessModifier, clsCodeAttr, regionsCache, nullRegions, attr2ClsMapping, regions4PumpSkifCache);
        }

        #endregion Функции закачки данных блока XML


        #region Общая организация закачки отчетов XML

        /// <summary>
        /// Закачивает шаблон старого формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        protected virtual void PumpExternalXMLPattern(XmlDocument xdPattern)
        {

        }

        /// <summary>
        /// Закачивает шаблон нового формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        protected virtual void PumpInternalXMLPattern(XmlNode xnPattern)
        {

        }

        protected virtual void PumpKITPattern(DirectoryInfo dir)
        {

        }

        /// <summary>
        /// Закачивает шаблон.
        /// </summary>
        /// <param name="xdPattern">Документ с загруженным шаблоном (для старого формата)</param>
        /// <param name="filesRepList">Список файлов отчета (для поиска шаблона нового формата)</param>
        private void PumpXMLPattern(XmlDocument xdPattern, FileInfo[] filesRepList, DirectoryInfo dir)
        {
            // Обработка нового шаблона
            XmlNode xn = null;
            bool patternIsPumped = false;

            // Шаблон может быть только в одном из файлов отчетов. Потому ищем во всех
            for (int i = 0; i < filesRepList.GetLength(0); i++)
            {
                xn = GetPatternNode(ConfigureXmlParams(filesRepList[i]));

                if (xn != null)
                {
                    WriteToTrace(string.Format("Старт обработки шаблона {0}.", filesRepList[i].Name), TraceMessageKind.Information);
                    PumpInternalXMLPattern(xn);
                    WriteToTrace(string.Format("Шаблон {0} обработан.", filesRepList[i].Name), TraceMessageKind.Information);

                    patternIsPumped = true;
                }
            }

            // Обработка старого шаблона
            if (!patternIsPumped && xdPattern != null)
            {
                WriteToTrace(string.Format("Старт обработки внешнего шаблона."), TraceMessageKind.Information);
                PumpExternalXMLPattern(xdPattern);
                patternIsPumped = true;
                WriteToTrace(string.Format("Шаблон обработан."), TraceMessageKind.Information);
            }
            else if (!patternIsPumped)
            {
                if (dir.GetFiles("*.kit", SearchOption.AllDirectories).GetLength(0) != 0)
                {
                    WriteToTrace(string.Format("Старт обработки kit шаблонов."), TraceMessageKind.Information);
                    PumpKITPattern(dir);
                    WriteToTrace(string.Format("Шаблоны kit обработаны."), TraceMessageKind.Information);
                }
            }
            else if (!patternIsPumped && xdPattern == null)
            {
                throw new PumpDataFailedException("Ни в одном из файлов шаблон не найден.");
            }

            UpdateData();
        }

        /// <summary>
        /// Закачивает районы из отчета формата хмл
        /// </summary>
        /// <param name="xnReport">Элемент с данными отчета</param>
        protected virtual bool PumpRegionsFromXMLReport(XmlNode xnReport)
        {
            return false;
        }

        /// <summary>
        /// Закачивает отчет формата хмл
        /// </summary>
        /// <param name="xnReport">Элемент с данными отчета</param>
        protected virtual void PumpXMLReport(XmlNode xnReport, string progressMsg)
        {

        }

        /// <summary>
        /// Устанавливает значение множителя для сумм и номера форм в зависимости от даты
        /// </summary>
        private XmlDocument ConfigureXmlParams(FileInfo reportFile)
        {
            XmlDocument xdReport = new XmlDocument();
            try
            {
                xdReport.Load(reportFile.FullName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("При загрузке отчета произошла ошибка, xml имеет некорректный формат: {0}", ex.Message), ex);
            }
            sumFactor = 1;
            xmlFormat = XmlFormat.Skif3;

            if (((this.DataSource.Year < 2005) || (this.DataSource.Year == 2005 && this.DataSource.Month <= 9)) &&
                reportFile.Name.ToUpper().StartsWith("ПЕР"))
            {
                // Для закачки до отчетов до октября 2005 переводить в рубли.
                sumFactor = 1000;

                if (xdReport.SelectSingleNode("/Otchet") != null)
                {
                    xmlFormat = XmlFormat.Format2004;
                }
                else if (xdReport.SelectSingleNode("/RootXml//Report") != null)
                {
                    xmlFormat = XmlFormat.Format2005;
                }
                else
                {
                    throw new Exception("не найден элемент с данными отчета");
                }
            }
            else if (this.DataSource.Year < 2006 && reportFile.Name.ToUpper().StartsWith("ПЕР"))
            {
                xmlFormat = XmlFormat.October2005;
            }

            return xdReport;
        }

        /// <summary>
        /// Закачивает отчеты формата XML
        /// </summary>
        /// <param name="dir">Каталог закачки</param>
        protected void PumpXMLReports(DirectoryInfo dir)
        {
            FileInfo[] filesList = dir.GetFiles("*.xml", SearchOption.AllDirectories);
            if (filesList.GetLength(0) == 0)
                return;
            XmlDocument xdPattern = null;
            // Загрузка файлов в XML
            // Список файлов отчетов
            FileInfo[] filesRepList;
            LoadXMLFiles(filesList, out filesRepList, out xdPattern);

            // Закачиваем шаблон
            PumpXMLPattern(xdPattern, filesRepList, dir);

            bool pumpReports = true;

            // Закачка данных
            for (int i = 0; i < filesRepList.GetLength(0); i++)
            {
                filesCount++;
                string progressMsg = string.Format("Обработка файла {0} ({1} из {2})...",
                    filesRepList[i].Name, filesCount, xmlFilesCount);

                if (!File.Exists(filesRepList[i].FullName))
                    continue;

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                    string.Format("Старт закачки файла {0}.", filesRepList[i].FullName));

                try
                {
                    // Выбираем элемент с отчетами
                    XmlNode xnReport = GetReportNode(ConfigureXmlParams(filesRepList[i]));
                    if (xnReport == null)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                            "В файле отсутствуют данные для закачки.");
                        continue;
                    }

                    // Закачиваем районы
                    if (!PumpRegionsFromXMLReport(xnReport))
                        pumpReports = false;

                    // Закачиваем данные отчета
                    if (pumpReports)
                    {
                        PumpXMLReport(xnReport, progressMsg);

                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                            string.Format("Файл {0} успешно закачан.", filesRepList[i].FullName));
                    }

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
                    xdPattern = null;

                    CollectGarbage();
                }
            }

            if (noRegForPump)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Классификатор Районы.Служебный (SOURCEID {0}) имеет записи с неуказанным типом района. " +
                    "Необходимо установить значения поля \"ТипДокумента.СКИФ\" и запустить этап обработки.", regForPumpSourceID));

            // Сохранение данных
            UpdateData();
        }

        #endregion Общая организация закачки отчетов XML
    }
}