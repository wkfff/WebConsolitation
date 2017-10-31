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
using Krista.FM.Server.DataPumps.Common;
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
        /// Тип файла отчета дбф
        /// </summary>
        protected enum DBFReportKind
        {
            /// <summary>
            /// Шаблон
            /// </summary>
            Pattern,

            /// <summary>
            /// Отчет района
            /// </summary>
            Region,

            /// <summary>
            /// Консолидированный отчет
            /// </summary>
            Consolidated,

            /// <summary>
            /// Консолидированный отчет для отправки в МинФин
            /// </summary>
            ConsolidatedMF
        }

        #endregion Структуры, перечисления


        #region Общие функции DBF

        /// <summary>
        /// Возвращает номер формы файла дбф
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        /// <returns>Номер формы</returns>
        protected int GetFileFormNo(FileInfo file)
        {
            return Convert.ToInt32(file.Name.Substring(file.Name.Length - file.Extension.Length - 2, 2));
        }

        /// <summary>
        /// Проверяет строку на наличие нулей в полях сумм
        /// </summary>
        /// <param name="row">Строка</param>
        /// <returns>Если все суммы = 0, то true</returns>
        private bool CheckZeroSums(DataRow row)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (GetDoubleCellValue(row, string.Format("P{0}", i), 0) != 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Формирует строку ограничения по списку кодов листа
        /// </summary>
        /// <param name="kl">Массив кодов листа.
        /// Формат элементов массива: "code" - выбираются коды, равные указанному;
        /// "code1;code2" - выбираются указанные коды;
        /// "code1..code2" - выбираются коды из указанного диапазона</param>
        /// <param name="fieldName">Поле, по которому строится ограничение</param>
        /// <returns>Строка ограничения</returns>
        private string GetConstrByKlList(string kl, string fieldName)
        {
            if (kl == string.Empty) return string.Empty;

            string result = string.Empty;
            string[] values = kl.Split(';');

            int count = values.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string[] intervals = values[i].Split(new string[] { ".." }, StringSplitOptions.None);
                if (intervals.GetLength(0) == 1)
                {
                    result += string.Format(" or (KL = {0})", intervals[0]);
                }
                else
                {
                    result += string.Format(" or (KL >= {0} and KL <= {1})", intervals[0], intervals[1]);
                }
            }

            if (result != string.Empty) result = result.Remove(0, 3).Trim();

            return result;
        }

        /// <summary>
        /// Определяет тип отчета дбф
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        /// <returns>Тип отчета</returns>
        protected DBFReportKind GetDBFReportKind(string fileName)
        {
            if (fileName.ToUpper().StartsWith("SS")) return DBFReportKind.Pattern;
            else if (fileName.ToUpper().StartsWith("SV")) return DBFReportKind.Consolidated;
            else if (fileName.StartsWith("650") || fileName.StartsWith("651")) return DBFReportKind.ConsolidatedMF;
            else return DBFReportKind.Region;
        }

        /// <summary>
        /// Проверяет название xml-файла на соответствие источнику и т.п.
        /// </summary>
        /// <param name="fileName">Название файла</param>
        private bool CheckDBFFileName(string fileName)
        {
            string str = fileName.ToUpper();

            if (str.StartsWith("MBUD") ||
                str.StartsWith("MDK") ||
                str.StartsWith("MES") ||
                str.StartsWith("MFORM") ||
                str.StartsWith("MOKRUG") ||
                str.StartsWith("VK")) return false;

            try
            {
                switch (GetDBFReportKind(fileName))
                {
                    case DBFReportKind.Consolidated:
                        switch (this.SkifReportFormat)
                        {
                            case SKIFFormat.MonthReports:
                                // Для консолидированных отчетов проверяем год и месяц
                                if (Convert.ToInt32(fileName.Substring(2, fileName.Length - 10)) != this.DataSource.Month)
                                {
                                    throw new Exception("месяц в названии файла не соответствует источнику.");
                                }
                                if (Convert.ToInt32(fileName.Substring(fileName.Length - 8, 2)) != this.DataSource.Year % 100)
                                {
                                    throw new Exception("год в названии файла не соответствует источнику.");
                                }
                                break;

                            case SKIFFormat.YearReports:
                                // Для консолидированных отчетов проверяем год и месяц
                                if (Convert.ToInt32(fileName.Substring(2, fileName.Length - 10)) != this.DataSource.Month)
                                {
                                    throw new Exception("год в названии файла не соответствует источнику.");
                                }
                                break;
                        }
                        break;

                    case DBFReportKind.ConsolidatedMF:
                        break;

                    case DBFReportKind.Pattern:
                        // Для шаблона проверяем только год
                        if (Convert.ToInt32(fileName.Substring(2, 2)) != this.DataSource.Year % 100)
                        {
                            throw new Exception("год в названии файла не соответствует источнику.");
                        }
                        break;

                    case DBFReportKind.Region:
                        switch (this.SkifReportFormat)
                        {
                            case SKIFFormat.MonthReports:
                                // Для отчетов районов проверяем год и месяц
                                if (Convert.ToInt32(fileName.Substring(0, fileName.Length - 8)) != this.DataSource.Month)
                                {
                                    throw new Exception("месяц в названии файла не соответствует источнику.");
                                }
                                if (Convert.ToInt32(fileName.Substring(fileName.Length - 8, 2)) != this.DataSource.Year % 100)
                                {
                                    throw new Exception("год в названии файла не соответствует источнику.");
                                }
                                break;

                            case SKIFFormat.YearReports:
                                // Для отчетов районов проверяем год
                                if (Convert.ToInt32(fileName.Substring(0, fileName.Length - 8)) != this.DataSource.Month)
                                {
                                    throw new Exception("год в названии файла не соответствует источнику.");
                                }
                                break;
                        }
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Ошибка при проверке названия файла {0}", fileName), ex);
                return false;
            }
        }

        /// <summary>
        /// Загружает dbf-файлы
        /// </summary>
        /// <param name="filesList">Список файлов</param>
        /// <param name="filesRepList">Список файлов отчетов</param>
        /// <param name="filesPtrnList">Список шаблонов</param>
        private void LoadDBFFiles(FileInfo[] filesList, out FileInfo[] filesRepList, out FileInfo[] filesPtrnList)
        {
            filesRepList = new FileInfo[0];
            filesPtrnList = new FileInfo[0];

            for (int i = 0; i < filesList.GetLength(0); i++)
            {
                if (!File.Exists(filesList[i].FullName)) continue;
                if (!CheckDBFFileName(filesList[i].Name))
                {
                    dbfFilesCount--;
                    continue;
                }

                switch (GetDBFReportKind(filesList[i].Name))
                {
                    case DBFReportKind.Pattern:
                        filesPtrnList =
                            (FileInfo[])CommonRoutines.RedimArray(filesPtrnList, filesPtrnList.GetLength(0) + 1);
                        filesPtrnList[filesPtrnList.GetLength(0) - 1] = filesList[i];
                        dbfFilesCount--;
                        break;

                    case DBFReportKind.Consolidated:
                    case DBFReportKind.Region:
                        // Формируем список файлов отчетов
                        filesRepList =
                            (FileInfo[])CommonRoutines.RedimArray(filesRepList, filesRepList.GetLength(0) + 1);
                        filesRepList[filesRepList.GetLength(0) - 1] = filesList[i];
                        break;
                }
            }
        }

        #endregion Общие функции DBF


        #region Функции закачки шаблона

        /// <summary>
        /// Возвращает код КБК для поиска по кэшу классификаторов
        /// </summary>
        /// <param name="code">Исходный код</param>
        /// <param name="kl">Код листа</param>
        /// <param name="kst">Код строки</param>
        /// <returns>Код для поиска</returns>
        protected string GetCacheCode(string code, int kl, int kst)
        {
            return code;// + Convert.ToString(kl).PadLeft(10, '0');// + Convert.ToString(kst).PadLeft(10, '0');
        }

        /// <summary>
        /// Закачивает классификатор из шаблона
        /// </summary>
        /// <param name="fileFormNo">Номер формы из названия файла</param>
        /// <param name="kl">Поле KL</param>
        /// <param name="kst">Поле KST</param>
        /// <param name="kbk">Поле KBK</param>
        /// <param name="n2">Поле N2</param>
        /// <returns>ИД классификатора</returns>
        protected virtual int PumpClsFromPattern(int fileFormNo, int kl, int kst, string kbk, string n2)
        {
            return -1;
        }

        /// <summary>
        /// Закачивает шаблоны дбф
        /// </summary>
        /// <param name="filesPtrnList">Список файлов шаблонов</param>
        private void PumpDBFPattern(FileInfo[] filesPtrnList)
        {
            if (filesPtrnList.GetLength(0) == 0)
            {
                throw new PumpDataFailedException("Отсутствует шаблон.");
            }

            for (int i = 0; i < filesPtrnList.GetLength(0); i++)
            {
                if (!CommonRoutines.CheckValueEntry(GetFileFormNo(filesPtrnList[i]), GetAllFormNo())) continue;

                WriteToTrace(string.Format("Старт обработки шаблона {0}.", filesPtrnList[i].Name), TraceMessageKind.Information);

                IDbDataAdapter da = null;
                DataSet ds = new DataSet();
                InitDataSet(this.DbfDB, ref da, ref ds, string.Format("SELECT * FROM {0}", filesPtrnList[i].Name));

                // Определяем форму шаблона
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    int kl = GetIntCellValue(ds.Tables[0].Rows[j], "KL", 0);
                    int kst = GetIntCellValue(ds.Tables[0].Rows[j], "KST", 0);
                    string kbk = GetStringCellValue(ds.Tables[0].Rows[j], "KBK", "0");
                    string n2 = GetStringCellValue(ds.Tables[0].Rows[j], "N2", constDefaultClsName).Trim();

                    PumpClsFromPattern(GetFileFormNo(filesPtrnList[i]), kl, kst, kbk, n2);

                    SetProgress(ds.Tables[0].Rows.Count, j + 1,
                        string.Format("Обработка шаблона {0}...", filesPtrnList[i].Name),
                        string.Format("Строка {0} из {1}", j + 1, ds.Tables[0].Rows.Count));
                }

                WriteToTrace(string.Format("Шаблон {0} обработан.", filesPtrnList[i].Name), TraceMessageKind.Information);
            }

            UpdateData();
        }

        #endregion Функции закачки шаблона


        #region Функции закачки данных блока

        /// <summary>
        /// Ищет код классификатора. Если не найден - закачивает с name = ???
        /// </summary>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="clsTable">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="fileFormNo">Номер формы файла</param>
        /// <param name="kl">Поле KL</param>
        /// <param name="kst">Поле KST</param>
        /// <param name="kbk">Поле KBK</param>
        /// <param name="nullRefToCls">Ссылка из таблицы фактов на классификаторы "Неизвестные данные"</param>
        /// <param name="clsValuesMapping">Массив значений ссылок на классификаторы</param>
        /// <param name="isMark">true - используется классификатор показателя. Код для поиска кода классификатора
        /// будет формироваться конкатенацией KL и KBK</param>
        /// <returns>ИД классификатора</returns>
        private void FindClsID(Dictionary<string, int>[] codesMapping, DataTable[] clsTable, IClassifier[] cls,
            int fileFormNo, int kl, int kst, string kbk, int[] nullRefToCls, object[] clsValuesMapping, bool isMark)
        {
            string code = kbk;
            if (isMark) code = kbk + kst.ToString();

            // Ищем код
            if (codesMapping != null)
            {
                int count = clsValuesMapping.GetLength(0);
                for (int i = 0; i < count; i += 2)
                {
                    int id = FindCachedRow(codesMapping[i / 2], code, nullRefToCls[i / 2]);
                    // Если код не найден, добавляем классификатор
                    int patternClsID = -1;
                    if (id == nullRefToCls[i / 2])
                    {
                        patternClsID = PumpClsFromPattern(fileFormNo, kl, kst, kbk, constDefaultClsName);
                        if (patternClsID == -1)
                            patternClsID = nullRefToCls[i / 2];
                        clsValuesMapping[i + 1] = patternClsID;
                    }
                    else
                    {
                        clsValuesMapping[i + 1] = id;
                    }
                }
            }
            else
            {
                int count = clsValuesMapping.GetLength(0);
                for (int i = 0; i < count; i += 2)
                {
                    clsValuesMapping[i + 1] = FindRowID(clsTable[i / 2],
                        new object[] { GetClsCodeField(cls[i / 2]), code }, nullRefToCls[i / 2]);
                }
            }
        }

        /// <summary>
        /// Ищет код классификатора. Если не найден - закачивает с name = ???
        /// </summary>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="clsTable">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="fileFormNo">Номер формы файла</param>
        /// <param name="kl">Поле KL</param>
        /// <param name="kst">Поле KST</param>
        /// <param name="kbk">Поле KBK</param>
        /// <param name="clsCode">Код классификатора. Если заполнено используется при добавлении записи.</param>
        /// <param name="nullRefToCls">Ссылка из таблицы фактов на классификаторы "Неизвестные данные"</param>
        /// <param name="clsValuesMapping">Массив значений ссылок на классификаторы</param>
        /// <param name="isMark">true - используется классификатор показателя. Код для поиска кода классификатора
        /// будет формироваться конкатенацией KL и KBK</param>
        /// <returns>ИД классификатора</returns>
        private int FindClsID(Dictionary<string, int> codesMapping, DataTable clsTable, IClassifier cls,
            int fileFormNo, int kl, int kst, string kbk, string clsCode, int nullRefToCls, object[] clsValuesMapping, 
            bool isMark)
        {
            int clsID = nullRefToCls;
            string code = kbk;
            string kbkEx = GetCacheCode(kbk, kl, kst);

            if (isMark)
            {
                code = kbk + kst.ToString();
            }

            // Ищем код
            if (codesMapping != null)
            {
                clsID = FindCachedRow(codesMapping, kbkEx, nullRefToCls);
            }
            else
            {
                clsID = FindRowID(clsTable, new object[] { GetClsCodeField(cls), code }, nullRefToCls);
            }

            // Если код не найден, добавляем классификатор
            if (clsID == nullRefToCls)
            {
                int patternClsID = -1;
                if (clsCode == string.Empty)
                {
                    patternClsID = PumpClsFromPattern(fileFormNo, kl, kst, kbk, constDefaultClsName);
                    if (patternClsID != -1)
                        clsID = patternClsID;
                }
                else
                {
                    clsID = PumpCachedRow(codesMapping, clsTable, cls, kbkEx,
                        new object[] { GetClsCodeField(cls), clsCode, "NAME", constDefaultClsName, "KL", kl, "KST", kst });
                }
            }

            return clsID;
        }

        /// <summary>
        /// Формирует массив ссылок на классификаторы
        /// </summary>
        /// <param name="fileFormNo">Номер формы файла</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="nullRefsToCls">Ссылки из таблицы фактов на классификаторы "Неизвестные данные"</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="isMark">true - используется классификатор показателя. Код для поиска кода классификатора
        /// будет формироваться конкатенацией KL и KBK</param>
        /// <param name="clsValuesMapping">Массив значений классификаторов</param>
        /// <param name="row">Строка дбф-файла</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="kbk2ClsMapping"> Список количество_символов объединенного кода классификатора, откуда будут браться 
        /// значения для указанных выше классификаторов.
        /// Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num:mask" - mask определяет количество символов, до которого будет дополнено нулями справа полученное значение;
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        private bool GetClsValuesMapping(int fileFormNo, DataTable[] clsTable, IClassifier[] cls, int[] nullRefsToCls, 
            Dictionary<string, int>[] codesMapping, bool isMark, int yearIndex, object[] clsValuesMapping, 
            DataRow row, BlockProcessModifier blockProcessModifier, string[] kbk2ClsMapping)
        {
            string kbk = Convert.ToString(row["KBK"]).Trim();
            if (kbk.StartsWith("LIST")) return false;
            
            int kl = Convert.ToInt32(row["KL"]);
            int kst = Convert.ToInt32(row["KST"]);
            string kbkEx = GetCacheCode(kbk, kl, kst);

            try
            {
                // Получаем ИД классификатора
                switch (blockProcessModifier)
                {
                    case BlockProcessModifier.MROutcomesBooksEx:
                        // Код классификатора должен присутствовать во всех таблицах
                        return CheckClsIDByCode(clsValuesMapping, codesMapping, kbkEx);

                    case BlockProcessModifier.YROutcomes:
                    case BlockProcessModifier.MROutcomesBooks:
                        // Формируем ссылки на классификаторы по коду
                        string[] codeValues = GetCodeValuesAsSubstring(kbk2ClsMapping, kbk, "0");

                        int count = codeValues.GetLength(0);
                        for (int i = 0; i < count; i++)
                        {
                            if (clsTable[i] == null) continue;

                            if (blockProcessModifier == BlockProcessModifier.MROutcomesBooks)
                            {
                                clsValuesMapping[i * 2 + 1] = PumpCachedRow(codesMapping[i], clsTable[i], cls[i],
                                    codeValues[i], "CODE", new string[] { "NAME", constDefaultClsName, "KL", kl.ToString(), "KST", kst.ToString() });
                            }
                            else
                            {
                                clsValuesMapping[i * 2 + 1] = FindClsID(
                                    codesMapping[i], clsTable[i], cls[i], fileFormNo, kl, kst, codeValues[i],
                                    codeValues[i], nullRefsToCls[i], clsValuesMapping, false);
                            }
                        }

                        if (blockProcessModifier == BlockProcessModifier.MROutcomesBooks)
                        {
                            return CheckClsIDByCode(codesMapping, codeValues);
                        }

                        break;

                    case BlockProcessModifier.YREmbezzles:
                        break;

                    default:
                        if (yearIndex >= 0)
                        {
                            if (codesMapping == null)
                            {
                                if (clsValuesMapping.GetLength(0) != 0)
                                    clsValuesMapping[yearIndex * 2 + 1] = FindClsID(null, clsTable[yearIndex], cls[yearIndex], fileFormNo,
                                        kl, kst, kbk, string.Empty, nullRefsToCls[yearIndex], clsValuesMapping, isMark);
                            }
                            else
                            {
                                clsValuesMapping[yearIndex * 2 + 1] =
                                    FindClsID(codesMapping[0], clsTable[yearIndex], cls[yearIndex], fileFormNo,
                                        kl, kst, kbk, string.Empty, nullRefsToCls[yearIndex], clsValuesMapping, isMark);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка при формировании кодов классификации по КБК {0}", kbk), ex);
            }

            return true;
        }

        /// <summary>
        /// Создает массив значений ссылок на классификаторы в зависимости от блока
        /// </summary>
        /// <param name="formNo">Номер формы</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <returns>Массив значений</returns>
        private object[] PrepareIndividualCodesMappingDBF(int formNo, BlockProcessModifier blockProcessModifier)
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
                case BlockProcessModifier.YRDefProf:
                case BlockProcessModifier.YROutcomes:
                case BlockProcessModifier.YRIncomes:
                case BlockProcessModifier.YRSrcFin:
                    Array.Resize(ref result, 2);
                    result[0] = "REFMEANSTYPE";
                    result[1] = 1;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Возваращает значение ячейки строки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="field">Название поля. Может содержать значения через ; - тогда будет закачан значение того поля, где есть значение;
        /// пустая строка эквивалентна нулевому значению поля.</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение поля</returns>
        private double GetDBFRowCellValue(DataRow row, string field, double defaultValue)
        {
            if (field == string.Empty) return defaultValue;

            string[] fieldArray = field.Split(';');
            int count = fieldArray.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                double sum = GetDoubleCellValue(row, fieldArray[i], 0);
                if (sum != 0)
                {
                    return sum;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Закачивает строку отчета районов
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="date">Дата источника</param>
        /// <param name="row">Строка дбф</param>
        /// <param name="fieldValuesMapping">Массив пар вида имя_поля_факты-имя_поля_дбф.
        /// имя_поля_дбф может содержать значения через ; - тогда будет закачан значение того поля, где есть значение;
        /// пустая строка эквивалентна нулевому значению поля.</param>
        /// <param name="clsValuesMapping">Массив значений классификаторов</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="budgetLevel">Уровень бюджета</param>
        private void PumpDBFRow(DataTable factTable, IFactTable fct, string date, DataRow row,
            string[] fieldValuesMapping, object[] clsValuesMapping, int regionID, int budgetLevel)
        {
            object[] fieldsMapping = new object[fieldValuesMapping.GetLength(0)];

            bool zeroSums = true;
            int count = fieldValuesMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                fieldsMapping[i] = fieldValuesMapping[i];
                double sum = GetDBFRowCellValue(row, fieldValuesMapping[i + 1], 0) * this.SumFactor;
                fieldsMapping[i + 1] = sum;

                if (sum != 0)
                {
                    zeroSums = false;
                }
            }

            if (!zeroSums)
            {
                switch (this.SkifReportFormat)
                {
                    case SKIFFormat.MonthReports:
                        PumpRow(factTable,
                            (object[])CommonRoutines.ConcatArrays(fieldsMapping, clsValuesMapping, new object[] { 
                                "RefYearDayUNV", date, "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID }));
                        break;

                    case SKIFFormat.YearReports:
                        PumpRow(factTable,
                            (object[])CommonRoutines.ConcatArrays(fieldsMapping, clsValuesMapping, new object[] { 
                                "REFYEAR", date, "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID }));
                        break;
                }
            }
        }

        /// <summary>
        /// Закачивает строку отчета районов
        /// </summary>
        /// <param name="fileFormNo">Номер формы файла</param>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="date">Дата источника</param>
        /// <param name="row">Строка дбф</param>
        /// <param name="clsValuesMapping">Массив значений классификаторов</param>
        private void ProcessDBFRow(int fileFormNo, DBFReportKind repKind, DataTable factTable, IFactTable fct, 
            string date, DataRow row, object[] clsValuesMapping, 
            Dictionary<string, int> regionCache, int nullRegions, Dictionary<string, int> regions4PumpSkifCache)
        {
            int kvb = GetIntCellValue(row, "KVB", 0);
            int org = GetIntCellValue(row, "ORG", 0);
            int kst = GetIntCellValue(row, "KST", 0);
            int budgetLevel = 3;
            int regionID = -1;

            switch (repKind)
            {
                case DBFReportKind.Region:
                    string code = GetDBFRegionCode(kvb.ToString(), org.ToString());
                    string name = string.Empty;
                    foreach (KeyValuePair<string, int> item in regionCache)
                        if (item.Key.ToString().Split('|')[0] == code)
                        {
                            name = item.Key.ToString().Split('|')[1];
                            break;
                        }
                    string regKey = code + "|" + name;
                    if (regions4PumpSkifCache != null)
                    {
                        if (!regions4PumpSkifCache.ContainsKey(regKey))
                            return;
                        switch (regions4PumpSkifCache[regKey])
                        {
                            case 2:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                regionID = FindCachedRow(regionCache, regKey, nullRegions);
                                if ((kvb == 1 || kvb == 2) && org == 900)
                                    budgetLevel = 3;
                                else
                                    budgetLevel = 7;
                                break;
                            case 3:
                                regionID = FindCachedRow(regionCache, regKey, nullRegions);
                                break;
                            default:
                                return;
                        }
                    }
                    break;
                default:
                    regionID = FindCachedRow(regionCache, "0000000001|Консолидированный отчет субъекта", nullRegions);
                    budgetLevel = 2;
                    break;
            }

            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    switch (fileFormNo)
                    {
                        case 50:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", "P1", "QUARTERPLANREPORT", "P4",
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P7" },
                                clsValuesMapping, regionID, 2);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", "P2", "QUARTERPLANREPORT", "P5",
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P8" },
                                clsValuesMapping, regionID, 3);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", "P3", "QUARTERPLANREPORT", "P6",
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P9" },
                                clsValuesMapping, regionID, 7);
                            break;

                        case 51:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty,
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P1" },
                                clsValuesMapping, regionID, 2);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty,
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P2" },
                                clsValuesMapping, regionID, 3);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty,
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P3" },
                                clsValuesMapping, regionID, 7);
                            break;
                    }
                    break;

                case SKIFFormat.YearReports:
                    switch (fileFormNo)
                    {
                        case 2:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "P2" }, clsValuesMapping, regionID, 2);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "P3" }, clsValuesMapping, regionID, 3);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "P4" }, clsValuesMapping, regionID, 7);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", "P1", "PERFORMEDREPORT", string.Empty }, clsValuesMapping, regionID, budgetLevel);
                            break;

                        case 3:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "FACTBEGINYEAR", "P1", "FACTENDYEAR", "P2", "BUDGETMIDYEAR", "P4", "FACTMIDYEAR", "P3" },
                                clsValuesMapping, regionID, budgetLevel);
                            break;

                        case 8:
                            for (int i = 1; i <= 9; i++)
                            {
                                PumpDBFRow(factTable, fct, date, row, new string[] {
                                    "FACT", string.Format("P{0}", i) }, (object[])CommonRoutines.ConcatArrays(
                                        clsValuesMapping, new object[] { "REFMARKS", kst, "REFMEANSTYPE", i }), 
                                    regionID, budgetLevel);
                            }
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Формирует код района отчетов дбф по KVB и ORG
        /// </summary>
        /// <param name="kvb">KVB</param>
        /// <param name="org">ORG</param>
        /// <returns>Код района</returns>
        private string GetDBFRegionCode(string kvb, string org)
        {
            return kvb.PadLeft(5, '0') + org.PadLeft(5, '0');
        }

        /// <summary>
        /// Формирует код района отчетов дбф по KVB и ORG
        /// </summary>
        /// <param name="kvb">KVB</param>
        /// <param name="org">ORG</param>
        /// <returns>Код района</returns>
        private string GetDBFRegionCode(int kvb, int org)
        {
            return GetDBFRegionCode(kvb.ToString(), org.ToString());
        }

        private bool PumpRegionForPump(string code, string key, string name, DataTable regions4PumpTable, 
            IClassifier regions4PumpCls, Dictionary<string, int> regions4PumpCache)
        {
            if (regions4PumpCache == null)
                return true;
            if (!regions4PumpCache.ContainsKey(key))
            {
                PumpCachedRow(regions4PumpCache, regions4PumpTable, regions4PumpCls, code, key, "CODESTR", "REFDOCTYPE",
                    new object[] { "NAME", name, "REFDOCTYPE", 1, "SOURCEID", GetRegions4PumpSourceID() });
                return false;
            }
            return true;
        }

        /// <summary>
        /// Закачивает район из MOKRUG.DBF
        /// </summary>
        protected void PumpRegionsDBF(DataTable dt, IClassifier cls, Dictionary<string, int> regionCache, int nullRegions,
            DataTable regions4PumpTable, IClassifier regions4PumpCls, Dictionary<string, int> regions4PumpCache)
        {
            ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);

            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            InitDataSet(this.DbfDB, ref da, ref ds,
                "select mo.KVB, mo.ORG, mo.N2 as MO_N2, mb.N2 as MB_N2 " +
                "from MOKRUG mo left join MBUD mb on (mo.KVB = mb.KVB)");
            bool noRegForPump = false;
            string code; string name; string regKey;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                // Отработка районов
                string kvb = Convert.ToString(ds.Tables[0].Rows[i]["KVB"]);
                string org = Convert.ToString(ds.Tables[0].Rows[i]["ORG"]);
                code = GetDBFRegionCode(kvb, org);
                name = GetStringCellValue(ds.Tables[0].Rows[i], "MO_N2", "Неуказанный район");
                regKey = code + "|" + name;
                if ((regions4PumpCache != null) && (regions4PumpCache.ContainsKey(regKey)))
                {
                    switch (regions4PumpCache[regKey])
                    {
                        case 2:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 3:
                            PumpCachedRow(regionCache, dt, cls, regKey, new object[] { 
                                "CODESTR", code, "NAME", name, "BUDGETKIND", kvb, "BUDGETNAME", ds.Tables[0].Rows[i]["MB_N2"] });
                            break;
                    }
                }
                if (!PumpRegionForPump(code, regKey, name, regions4PumpTable, regions4PumpCls, regions4PumpCache))
                    noRegForPump = true;
            }
            // консолидированный 
            code = "0000000001";
            name = "Консолидированный отчет субъекта";
            regKey = code + "|" + name;
            PumpCachedRow(regionCache, dt, cls, regKey, 
                new object[] { "CODESTR", code, "NAME", name, "BUDGETKIND", "К", "BUDGETNAME", constDefaultClsName });
            if (!PumpRegionForPump(code, regKey, name, regions4PumpTable, regions4PumpCls, regions4PumpCache))
                noRegForPump = true;
            if (noRegForPump)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Классификатор Районы.Служебный (SOURCEID {0}) имеет записи с неуказанным типом района. " +
                    "Необходимо установить значения поля \"ТипДокумента.СКИФ\" и запустить этап обработки.", GetRegions4PumpSourceID()));
            ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);
        }

        /// <summary>
        /// Закачивает данные отчета
        /// </summary>
        /// <param name="blockName">Название блока (для прогресса)</param>
        /// <param name="dbfTable">Таблица ДБФ</param>
        /// <param name="file">Файл отчета</param>
        /// <param name="fileFormNo">Номер формы файла</param>
        /// <param name="kl">Массив кодов листа.
        /// Формат элементов массива: "code" - выбираются коды, равные указанному;
        /// "code1;code2" - выбираются указанные коды;
        /// "code1..code2" - выбираются коды из указанного диапазона</param>
        /// <param name="da">ДатаАдаптер таблицы фактов</param>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="clsYears">Года, определяющие в данные какого года в какой классификатор из clsTable качать</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="nullRefsToCls">Ссылки из таблицы фактов на классификаторы "Неизвестные данные"</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="codeExclusions">Исключения кода классификатора (указанные коды не закачиваются).
        /// Массив пар Rule - CodePart, где CodePart - число символов кода, к которому будет применено правило 
        /// исключения Rule. Если присутствует только Rule, т.е. массив содержит 1 элемент, то это правило будет
        /// применено ко всему коду. Если код удовлетворяет хотя бы одному из правил, то он будет исключен.
        /// Формат элементов массива: 
        /// "rule1;rule2" - несколько правил, описывающих исключения, могут быть перечислены через точку с запятой;
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
        /// <param name="isMark">true - используется классификатор показателя. Код для поиска кода классификатора
        /// будет формироваться конкатенацией KL и KBK</param>
        /// <param name="multipleCls">true - таблица фактов имеет несколько ссылок на классификаторы, 
        /// классификаторы не зависят от года.</param>
        /// <param name="sumFieldForCorrect">Массив полей сумм для коррекции в таблице фактов</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="hierarchyMapping">Коллекция соответствия ИД подчиненной и родительской записей 
        /// для каждого классификатора</param>
        /// <param name="kbk2ClsMapping"> Список количество_символов объединенного кода классификатора, откуда будут браться 
        /// значения для указанных выше классификаторов.
        /// Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num:mask" - mask определяет количество символов, до которого будет дополнено нулями справа полученное значение;
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        protected void PumpDBFReportData(string blockName, DataTable dbfTable, FileInfo file, int fileFormNo,
            string kl, IDbDataAdapter da, DataTable factTable, IFactTable fct, 
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls, string progressMsg,
            Dictionary<string, int>[] codesMapping, string[] codeExclusions, bool isMark, BlockProcessModifier blockProcessModifier,
            Dictionary<string, int> regionCache, int nullRegions, string[] kbk2ClsMapping, Dictionary<string, int> regions4PumpSkifCache)
        {
            if (fileFormNo != GetFileFormNo(file) || clsTable.GetLength(0) != cls.GetLength(0)) return;

            string date = string.Empty;
            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    date = string.Format("{0}{1:00}00", this.DataSource.Year, this.DataSource.Month);
                    break;

                case SKIFFormat.YearReports:
                    date = this.DataSource.Year.ToString();
                    break;
            }

            // Определяем индекс года текущего источника в массиве всех лет.
            // Нужно для того, чтобы определить, что брать из других массивов.
            int yearIndex = GetYearIndexByYear(clsYears);

            // Формируем массив значений ссылок на классификаторы
            object[] clsValuesMapping = new object[factRefsToCls.GetLength(0) * 2];
            int count = factRefsToCls.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                clsValuesMapping[i * 2] = factRefsToCls[i];
                clsValuesMapping[i * 2 + 1] = nullRefsToCls[i];
            }

            DataRow[] rows = dbfTable.Select(GetConstrByKlList(kl, "KL"));
            int rowsCount = rows.GetLength(0);

            DBFReportKind repKind = GetDBFReportKind(file.Name);

            object[] individualCodesMapping =
                PrepareIndividualCodesMappingDBF(fileFormNo, blockProcessModifier);

            // Закачиваем данные в зависимости от формы отчета
            for (int i = 0; i < rowsCount; i++)
            {
                SetProgress(rowsCount, i + 1, progressMsg,
                    string.Format("{0}. Строка {1} из {2}", blockName, i + 1, rowsCount));

                if (CheckCodeExclusion(rows[i]["KBK"], codeExclusions) ||
                    !GetClsValuesMapping(fileFormNo, clsTable, cls, nullRefsToCls, codesMapping,
                        isMark, yearIndex, clsValuesMapping, rows[i], blockProcessModifier, kbk2ClsMapping))
                {
                    continue;
                }

                ProcessDBFRow(fileFormNo, repKind, factTable, fct, date, rows[i],
                    (object[])CommonRoutines.ConcatArrays(clsValuesMapping, individualCodesMapping),
                    regionCache, nullRegions, regions4PumpSkifCache);

                if (factTable.Rows.Count >= constMaxQueryRecords)
                {
                    UpdateData();
                    ClearDataSet(da, factTable);
                }
            }
        }

        #endregion Функции закачки данных блока


        #region Общая организация закачки отчетов DBF

        /// <summary>
        /// Проверяет все файлы в каталоге на соответствие источнику, наличие шаблонов и т.п.
        /// </summary>
        /// <param name="dir">Каталог</param>
        private void CheckDBFFilesInDir(DirectoryInfo dir)
        {
            if (dir.GetFiles("MBUD.DBF", SearchOption.AllDirectories).GetLength(0) == 0)
                throw new PumpDataFailedException("Отсутствует справочник видов бюджетов.");

            if (dir.GetFiles("MOKRUG.DBF", SearchOption.AllDirectories).GetLength(0) == 0)
                throw new PumpDataFailedException("Отсутствует справочник территориальных образований.");
        }

        /// <summary>
        /// Закачивает отчет формата DBF
        /// </summary>
        /// <param name="file">Файл отчета</param>
        protected virtual void PumpDBFReport(FileInfo file, string progressMsg)
        {

        }

        /// <summary>
        /// Возвращает все формы файлов, разрешенные к закачке
        /// </summary>
        /// <returns>Массив номеров форм</returns>
        protected virtual int[] GetAllFormNo()
        {
            return null;
        }

        /// <summary>
        /// Устанавливает значение множителя для сумм в зависимости от даты
        /// </summary>
        private void ConfigureDbfParams()
        {
            sumFactor = 1;

            if (this.DataSource.Year < 2005)
            {
                // Для закачки отчетов до 2005 переводить в рубли.
                sumFactor = 1000;
            }
        }

        /// <summary>
        /// Переустанавливает соединение с источником, используя указанный драйвер
        /// </summary>
        /// <param name="driver">Драйвер</param>
        private void ReconnectToDbfDataSource(ODBCDriverName driver)
        {
            // Коннектимся к источнику
            dbDataAccess.ConnectToDataSource(ref this.dbfDB, this.currentDir.FullName, driver);
        }

        /// <summary>
        /// Закачивает отчеты формата DBF
        /// </summary>
        /// <param name="dir">Каталог с отчетами</param>
        private void PumpDBFReports(DirectoryInfo dir)
        {
            this.currentDir = dir;

            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports: if (this.DataSource.Year < 2002 || this.DataSource.Year > 2004) return;
                    break;

                case SKIFFormat.YearReports: if (this.DataSource.Year < 2000 || this.DataSource.Year > 2004) return;
                    break;

                default: return;
            }

            FileInfo[] filesList = dir.GetFiles("*.DBF", SearchOption.AllDirectories);
            if (filesList.GetLength(0) == 0) return;

            ConfigureDbfParams();

            // Проверяем файлы в каталоге
            CheckDBFFilesInDir(dir);

            try
            {
                FileInfo[] filesRepList;
                FileInfo[] filesPtrnList;
                LoadDBFFiles(filesList, out filesRepList, out filesPtrnList);

                ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);

                // Закачиваем шаблон
                PumpDBFPattern(filesPtrnList);

                ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);

                // Обрабатываем файлы
                for (int i = 0; i < filesRepList.GetLength(0); i++)
                {
                    filesCount++;
                    string progressMsg = string.Format("Обработка файла {0} ({1} из {2})...",
                        filesRepList[i].Name, filesCount, dbfFilesCount);

                    if (!filesRepList[i].Exists) continue;

                    if (!CommonRoutines.CheckValueEntry(GetFileFormNo(filesRepList[i]), GetAllFormNo())) continue;

                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, 
                        string.Format("Старт закачки файла {0}.", filesRepList[i].FullName));

                    try
                    {
                        PumpDBFReport(filesRepList[i], progressMsg);

                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                            string.Format("Файл {0} успешно закачан.", filesRepList[i].FullName));
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, 
                            string.Format("Закачка из файла {0} закончена с ошибками", filesRepList[i].FullName), ex);
                        throw;
                    }
                    finally
                    {
                        CollectGarbage();
                    }

                    // Сохранение данных
                    UpdateData();
                }
            }
            finally
            {
                if (this.DbfDB != null)
                {
                    this.DbfDB.Close();
                    this.DbfDB = null;
                }
            }
        }

        #endregion Общая организация закачки отчетов DBF
    }
}