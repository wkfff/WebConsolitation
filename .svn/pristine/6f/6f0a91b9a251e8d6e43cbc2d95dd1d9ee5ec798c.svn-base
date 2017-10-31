using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// Базовый класс для закачек отчетов СКИФ
    /// </summary>
    public abstract partial class SKIFRepPumpModuleBase : CorrectedPumpModuleBase
    {
        #region Структуры, перечисления

        /// <summary>
        /// Формат отчетности СКИФ
        /// </summary>
        protected enum SKIFFormat
        {
            /// <summary>
            /// Ежемесячная отчетность
            /// </summary>
            MonthReports,

            /// <summary>
            /// Ежегодная отчетность
            /// </summary>
            YearReports,

            /// <summary>
            /// Неизвестно что
            /// </summary>
            Unknown
        }

        /// <summary>
        /// Вид обработки элемента с данными отчета
        /// </summary>
        private enum NodeProcessOption
        {
            /// <summary>
            /// Обрабатываются только указанные элементы
            /// </summary>
            Stated,

            /// <summary>
            /// Обрабатываются все элементы
            /// </summary>
            All,

            /// <summary>
            /// Обработка для блока "Задолженность"
            /// </summary>
            Arrears,

            /// <summary>
            /// Обработка для блока "Баланс"
            /// </summary>
            Balanc
        }

        #endregion Структуры, перечисления


        #region Поля

        private int xmlFilesCount = 0;
        private int dbfFilesCount = 0;
        private int xlsFilesCount = 0;
        private int filesCount = 0;

        private SKIFFormat skifFormat = SKIFFormat.Unknown;
        private XmlFormat xmlFormat;
        private int sumFactor = 1;

        private DirectoryInfo currentDir;
        private Database dbfDB = null;
        private CultureInfo currentCulture = CultureInfo.CurrentCulture;

        protected DBDataAccess dbDataAccess = new DBDataAccess();
         
        private List<string> warnedRegions = new List<string>(50);

        protected bool isPulseReports = false;
        protected bool isKitPatterns = false;
        // районы, данные по которым были закачаны
        // ключ - код|наименование района, значение - список форм
        protected Dictionary<string, List<XmlForm>> pumpedRegions = new Dictionary<string, List<XmlForm>>();

        #endregion Поля


        #region Константы

        private const int constMaxQueryRecords = 10000;
        // Название шаблона для ежемесячных отчётов
        private const string MonthPatternName = "FormDC_Skif_OTCH";

        #endregion Константы


        #region Инициализация

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbDataAccess != null) dbDataAccess.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Свойства класса

        /// <summary>
        /// Формат отчетности (ежемесячная или ежегодная)
        /// </summary>
        protected SKIFFormat SkifReportFormat
        {
            get
            {
                return skifFormat;
            }
            set
            {
                skifFormat = value;
            }
        }

        /// <summary>
        /// Формат отчета хмл
        /// </summary>
        protected XmlFormat XmlReportFormat
        {
            get 
            { 
                return xmlFormat; 
            }
            set 
            { 
                xmlFormat = value; 
            }
        }

        /// <summary>
        /// Множитель для сумм
        /// </summary>
        protected int SumFactor
        {
            get 
            { 
                return sumFactor; 
            }
            set 
            { 
                sumFactor = value; 
            }
        }

        /// <summary>
        /// Database, настроенный на источник с отчетами дбф
        /// </summary>
        protected Database DbfDB
        {
            get 
            { 
                return dbfDB; 
            }
            set 
            { 
                dbfDB = value; 
            }
        }

        #endregion Свойства класса


        #region Общие функции

        /// <summary>
        /// Проверяет код на соответствие списку исключений
        /// </summary>
        /// <param name="code">Код</param>
        /// <param name="codeExclusions">Список исключений (описание см. в PumpXMLReportBlock)</param>
        /// <returns>true - код входит в список исключений</returns>
        private bool CheckCodeExclusion(object code, string[] codeExclusions)
        {
            if (codeExclusions == null) return false;

            string[] exclusions = codeExclusions;
            if (codeExclusions.GetLength(0) > 1)
            {
                exclusions = GetFieldsValuesAsSubstring(codeExclusions, Convert.ToString(code), string.Empty);
            }

            bool result = false;

            int count = exclusions.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                string codePart = Convert.ToString(code);

                if (codeExclusions.GetLength(0) > 1)
                {
                    codePart = exclusions[i + 1];
                }
                string[] rules = Convert.ToString(exclusions[i]).Split(';');

                int rulesCount = rules.GetLength(0);
                for (int j = 0; j < rulesCount; j++)
                {
                    if (rules[j] == string.Empty) continue;

                    string positiveRule = rules[j].TrimStart('#').TrimStart('!');

                    // Префиксы правил:
                    // "!" - отрицание; "#" - превалирующее правило (если код ему не удовлетворяет, то он не будет пропущен 
                    // вне зависимости от других правил)

                    // "*code*" - исключаются коды, содержащие указанный
                    if (positiveRule.StartsWith("*") && positiveRule.EndsWith("*"))
                    {
                        result = InverseExclusionResult(rules[j],
                            codePart.Contains(positiveRule.Replace("*", string.Empty)));
                    }
                    // "code*" - исключаются коды, начинающиеся с указанного
                    else if (positiveRule.EndsWith("*"))
                    {
                        result = InverseExclusionResult(rules[j],
                            codePart.StartsWith(positiveRule.Replace("*", string.Empty)));
                    }
                    // "*code" - исключаются коды, заканчивающиеся на указанный
                    else if (positiveRule.StartsWith("*"))
                    {
                        result = InverseExclusionResult(rules[j],
                            codePart.EndsWith(positiveRule.Replace("*", string.Empty)));
                    }
                    // "code1..code2" - исключаются коды, входящие в диапазон code1..code2
                    else if (rules[j].Contains(".."))
                    { 
                        string[] values = positiveRule.Split(new string[] { ".." }, StringSplitOptions.None);

                        if (values[0] != string.Empty && values[1] != string.Empty)
                        {
                            if (code is string)
                            {
                                result = InverseExclusionResult(rules[j],
                                    codePart.CompareTo(values[0]) >= 0 && codePart.CompareTo(values[1]) <= 0);
                            }
                            else if (code is int)
                            {
                                result = InverseExclusionResult(rules[j],
                                    Convert.ToInt32(code) >= Convert.ToInt32(values[0]) &&
                                    Convert.ToInt32(code) <= Convert.ToInt32(values[1]));
                            }
                        }
                    }
                    // "<=code" - исключаются коды, меньшие или равные code;
                    else if (positiveRule.StartsWith("<="))
                    {
                        if (code is string)
                        {
                            result = InverseExclusionResult(rules[j],
                                codePart.CompareTo(positiveRule.Replace("<=", string.Empty)) < 0);
                        }
                        else if (code is int)
                        {
                            result = InverseExclusionResult(rules[j],
                                Convert.ToInt32(code) <= Convert.ToInt32(positiveRule.Replace("<=", string.Empty)));
                        }
                    }
                    // ">=code" - исключаются коды >= code;
                    else if (positiveRule.StartsWith(">="))
                    {
                        if (code is string)
                        {
                            result = InverseExclusionResult(rules[j],
                                codePart.CompareTo(positiveRule.Replace(">=", string.Empty)) > 0);
                        }
                        else if (code is int)
                        {
                            result = InverseExclusionResult(rules[j],
                                Convert.ToInt32(code) >= Convert.ToInt32(positiveRule.Replace(">=", string.Empty)));
                        }
                    }
                    // "code" - исключаются коды, равные указанному;
                    else
                    {
                        result = InverseExclusionResult(rules[j], codePart == positiveRule);
                    }

                    if (!result && rules[j].StartsWith("#")) return result;
                    if (result && rules[j].StartsWith("#")) result = false;

                    if (result)
                        break;
                }

                if (result) break;
            }

            return result;
        }

        /// <summary>
        /// Возвращает массив значений полей классификатора, являющихся подстроками значения элемента хмл
        /// </summary>
        /// <param name="attrValuesMapping">Список пар поле-количество_символов.
        /// Поле - имя поля. Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов, num..-1 - все оставшиеся символы, начиная с num))</param>
        /// <param name="clsCode">Код для поиска подстрок</param>
        /// <param name="defaultValue">Значение по умолчанию для пустых кодов</param>
        /// <returns>Массив значений подстрок</returns>
        private string[] GetFieldsValuesAsSubstring(string[] attrValuesMapping, string clsCode, string defaultValue)
        {
            if (attrValuesMapping == null) return null;

            int startIndex = 0;

            int count = attrValuesMapping.GetLength(0);
            string[] fieldsMapping = new string[count];

            for (int j = 0; j < count; j += 2)
            {
                // Копируем в массив результата название поля
                fieldsMapping[j] = attrValuesMapping[j];
                fieldsMapping[j + 1] = GetFieldValueAsSubstring(
                    attrValuesMapping[j + 1], clsCode, ref startIndex, defaultValue);
            }

            return fieldsMapping;
        }

        /// <summary>
        /// Возвращает массив значений полей классификатора, являющихся подстроками значения элемента хмл
        /// </summary>
        /// <param name="attrValuesMapping">Список количество_символов.
        /// Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num:mask" - mask определяет количество символов, до которого будет дополнено справа нулями полученное значение;
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        /// <param name="clsCode">Код для поиска подстрок</param>
        /// <param name="defaultValue">Значение по умолчанию для пустых кодов</param>
        /// <returns>Массив значений подстрок</returns>
        private string[] GetCodeValuesAsSubstring(string[] attrValuesMapping, string clsCode, string defaultValue)
        {
            if (attrValuesMapping == null || clsCode == string.Empty) return null;

            int startIndex = 0;

            int count = attrValuesMapping.GetLength(0);
            string[] fieldsMapping = new string[count];

            for (int j = 0; j < count; j++)
            {
                fieldsMapping[j] = GetFieldValueAsSubstring(attrValuesMapping[j], clsCode, ref startIndex, defaultValue);
            }

            return fieldsMapping;
        }

        /// <summary>
        /// Возвращает значение подстроки кода. Формат описания подстроки см. в GetFieldsValuesAsSubstring.attrValuesMapping
        /// </summary>
        /// <param name="attrSubstring">Значение массива GetFieldsValuesAsSubstring.attrValuesMapping</param>
        /// <param name="clsCode">Код</param>
        /// <param name="startIndex">Индекс текущего символа кода</param>
        /// <param name="defaultValue">Значение по умолчанию для пустых кодов</param>
        /// <returns>Значение подстроки</returns>
        private string GetFieldValueAsSubstring(string attrSubstring, string clsCode, ref int startIndex, string defaultValue)
        {
            string result = string.Empty;

            string[] mask = attrSubstring.Split(':');
            if (mask.GetLength(0) == 0) return defaultValue;

            string[] parts = mask[0].Split(';');

            int count = parts.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string[] intervals = parts[i].Split(new string[] { ".." }, StringSplitOptions.None);

                if (intervals.GetLength(0) == 0)
                {
                    continue;
                }
                else if (intervals.GetLength(0) == 1)
                {
                    if (intervals[0] == "-1")
                    {
                        result += clsCode.Substring(startIndex);
                    }
                    else
                    {
                        result += Convert.ToString(clsCode[Convert.ToInt32(intervals[0])]);
                        startIndex++;
                    }
                }
                else
                {
                    int lo = Convert.ToInt32(intervals[0]);
                    int hi = Convert.ToInt32(intervals[1]);

                    if (lo == -1)
                    {
                        result += clsCode.Substring(clsCode.Length - hi);
                    }
                    else if (hi == -1)
                    {
                        if (lo < clsCode.Length)
                        {
                            result += clsCode.Substring(lo);
                        }
                    }
                    else if (lo == 0)
                    {
                        result += clsCode.Substring(lo, hi);
                    }
                    else
                    {
                        if (hi >= clsCode.Length)
                        {
                            hi = clsCode.Length - 1;
                        }
                        result += clsCode.Substring(lo, hi - lo + 1);
                    }

                    startIndex = hi + 1;
                }
            }

            if (result == string.Empty) return defaultValue;

            if (mask.GetLength(0) == 2)
            {
                result = result.PadRight(mask[1].Length, '0');
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
        private bool CheckClsIDByCode(object[] fieldsMapping, Dictionary<string, int>[] codesMapping,
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
                    return false;
                }
                //result[i + 1] = fieldsMapping[i + 1];
            }

            return true;
        }

        /// <summary>
        /// Проверяет, чтобы коды пристуствовали во всех классификаторах
        /// </summary>
        /// <param name="codesMapping">Список пар код - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="clsValuesMapping">Массив значений классификаторов</param>
        /// <returns>true - Код найден во всех codesMapping, иначе в каком-то нет.</returns>
        private bool CheckClsIDByCode(Dictionary<string, int>[] codesMapping,
            object[] clsValuesMapping)
        {
            int count = clsValuesMapping.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string key = Convert.ToString(clsValuesMapping[i]);
                if (!codesMapping[i].ContainsKey(key))
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Формирует значение ключа для кэша
        /// </summary>
        /// <param name="row">Строка с записями</param>
        /// <param name="keyFields">Массив полей со значениями ключа</param>
        protected string GetComplexRegionCacheKey(DataRow row)
        {
            if (row == null)
                return string.Empty;
            return Convert.ToString(row["CODESTR"]).PadLeft(10, '0') +"|" + Convert.ToString(row["NAME"]);
        }

        /// <summary>
        /// Заполняет список кэша записей для районов
        /// </summary>
        /// <param name="cache">кэш районов</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyFields">Поля со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRegionsCache(ref Dictionary<string, int> cache, DataTable dt, string valueField)
        {
            if (dt == null)
                return;
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, int>(dt.Rows.Count);
            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState != DataRowState.Deleted)
                {
                    string key = GetComplexRegionCacheKey(row);
                    if (!cache.ContainsKey(key))
                        cache.Add(key, Convert.ToInt32(row[valueField]));
                }
            }
        }

        #endregion Общие функции
    

        #region Закачка данных

        /// <summary>
        /// Вызывает функции установки иерархии для всех классификаторов
        /// </summary>
        protected virtual void SetClsHierarchy()
        {

        }

        protected virtual void PumpXLSReports(DirectoryInfo dir)
        {

        }

        protected virtual void PumpTxtReports(DirectoryInfo dir)
        {

        }

        protected virtual void PumpFK2TxtReports(DirectoryInfo dir)
        {

        }

        protected virtual void PumpXlsNovosibReports(DirectoryInfo dir)
        {

        }

        protected virtual void PumpPulseReports(DirectoryInfo dir)
        {

        }

        protected virtual void PumpOviont(DirectoryInfo dir)
        {

        }

        /// <summary>
        /// Закачивает файлы из указанного каталога
        /// </summary>
        /// <param name="dir">Каталог</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            isPulseReports = false;
            isKitPatterns = false;
            PumpXMLReports(dir);
            // в новосибирске файлы dbf в формате пульса
            if (this.Region == RegionName.Novosibirsk)
            {
                isPulseReports = true;
                // могут быть экселевские файлы, тогда пульс качать не надо
                if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0)
                    PumpPulseReports(dir);
            }
            else
                PumpDBFReports(dir);
            isPulseReports = false;
            if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) != 0)
                switch (this.Region)
                {
                    case RegionName.Krasnodar:
                        PumpXLSReports(dir);
                        break;
                    case RegionName.Novosibirsk:
                        PumpXlsNovosibReports(dir);
                        break;
                }
            // формат фк - районы
            PumpTxtReports(dir);
            // формат фк - чиста в самом фк (пиздец)
            PumpFK2TxtReports(dir);
            // закачка базы овионта (как же все это заебало)
            PumpOviont(dir);

            toSetHierarchy = false;
            SetClsHierarchy();
            UpdateData();
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            xmlFilesCount = this.RootDir.GetFiles("*.xml", SearchOption.AllDirectories).GetLength(0);
            dbfFilesCount = this.RootDir.GetFiles("*.dbf", SearchOption.AllDirectories).GetLength(0);
            xlsFilesCount = this.RootDir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;
        }

        #endregion Закачка данных
    }
}