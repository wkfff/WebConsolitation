using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.Form1NApp7MonthPump
{
    /// <summary>
    /// УФК_0007_0007_Сводная ведомость по кассовым поступлениям в бюджеты (месячная)
    /// !!!!!!! переписать - привести в порядок, перенести функционал в закачку ставрополя !!!!!!
    /// </summary>
    public partial class Form1NApp7MonthPumpModule : DataPumpModuleBase
    {
        #region Поля

        #region Классификаторы

        // КД.УФК
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        // Местные бюджеты.УФК
        private IDbDataAdapter daLocBdgt;
        private DataSet dsLocBdgt;
        private IClassifier clsLocBdgt;
        private Dictionary<string, DataRow> locBdgtCache = null;
        private Dictionary<string, int> locBdgtOkatoCache = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Сводная ведомость по кассовым поступл месячн
        private IDbDataAdapter daUFK71N;
        private DataSet dsUFK71N;
        private IFactTable fctUFK71N;

        #endregion Факты

        private int totalRowsCount;
        private int processedRowsCount;
        private int pumpedRowsCount;
        private int skippedRowsCount;

        // ID записи в классификаторе "Местные бюджеты.УФК",
        // распространяется на весь файл (в некоторых текстовых отчётах)
        private int globalLocBdgtID = -1;

        private ExcelHelper excelHelper = null;
        private object excelObj = null;

        #endregion Поля

        #region Структуры, перечисления

        private enum ReportFormat
        {
            Excel,
            Text
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CodeStr");
            if ((this.Region == RegionName.Naur) || (this.Region == RegionName.AltayKrai))
                FillRowsCache(ref locBdgtCache, dsLocBdgt.Tables[0], new string[] { "Name" });
            else
                FillRowsCache(ref locBdgtCache, dsLocBdgt.Tables[0], new string[] { "Account" });
            FillRowsCache(ref locBdgtOkatoCache, dsLocBdgt.Tables[0], "Name", "Id");
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD);
            InitClsDataSet(ref daLocBdgt, ref dsLocBdgt, clsLocBdgt);

            InitFactDataSet(ref daUFK71N, ref dsUFK71N, fctUFK71N);

            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daLocBdgt, dsLocBdgt, clsLocBdgt);
            UpdateDataSet(daUFK71N, dsUFK71N, fctUFK71N);
        }

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_LOC_BDGT_UFK_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string F_D_UFK7_1N_GUID = "546a308b-9f09-46a0-a245-e6faa3bba1ea";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsLocBdgt = this.Scheme.Classifiers[D_LOC_BDGT_UFK_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctUFK71N = this.Scheme.FactTables[F_D_UFK7_1N_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK71N);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsLocBdgt);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        // уменьшаем дату на 1 месяц
        private int DecrementMonth(int refDate)
        {
            int year = refDate / 10000;
            int month = refDate % 10000 / 100;
            int day = refDate % 100;
            month--;
            if (month <= 0)
            {
                month = 12;
                year--;
            }
            return (year * 10000 + month * 100 + day);
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Replace(" ", string.Empty).Replace(".", ",").Trim();
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        #endregion Общие методы

        #region Закачка Txt отчетов

        private bool IsSkippedRow(string[] fields)
        {
            // не закачиваем источники финансирования
            switch (this.Region)
            {
                case RegionName.AltayKrai:
                    {
                        return (fields[0].Trim().ToUpper() == "IPSTBK_M");
                    }
                case RegionName.EAO:
                case RegionName.Novosibirsk:
                case RegionName.SamaraGO:
                case RegionName.Karelya:
                case RegionName.Yaroslavl:
                    return ((fields[0].Trim().ToUpper() == "IPSTBK_M") && (fields[2].Trim() != "31") && (fields[2].Trim() != "32"));
                case RegionName.Naur:
                    if (this.DataSource.Year >= 2010)
                        return ((fields[0].Trim().ToUpper() == "IPSTBK_M") && (fields[2].Trim() != "31") && (fields[2].Trim() != "32"));
                    return (fields[0].Trim().ToUpper() == "IKST");
            }
            return (fields[0].Trim().ToUpper() == "IPSTBK_M");
        }

        private int PumpKD(string[] fields)
        {
            string code = fields[3].Trim().PadLeft(1, '0');
            if ((this.Region == RegionName.Naur) && (this.DataSource.Year < 2010))
                code = fields[2].Trim().PadLeft(1, '0');
            return PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                new object[] { "CODESTR", code, "NAME", "Неуказанное наименование" });
        }

        private int PumpLocBdgt(string[] fields)
        {
            if (globalLocBdgtID != -1)
                return globalLocBdgtID;

            if ((this.DataSource.Year >= 2009) || (this.Region == RegionName.EAO))
            {
                return PumpOriginalRow(dsLocBdgt, clsLocBdgt,
                    new object[] { "ACCOUNT", "Неуказанный счет", "NAME", fields[1].Trim(), "OKATO", "0" });
            }

            string code = fields[1].Trim().PadLeft(1, '0');
            string name = fields[2].Trim();
            // Если такая запись уже присутствует, то запись не добавляем. НО! Если такая запись уже
            // присутствует, а наименование у нее «местный бюджет» (без учета регистра и пробелов).
            // А в той записи, с которой мы работаем – не «местный бюджет». То наименование заменяем нашим, смысловым
            if (string.Compare(name, "местный бюджет", true) == 0)
            {
                return PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, code,
                    new object[] { "ACCOUNT", code, "NAME", name });
            }
            else
            {
                return RepumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, code,
                    new object[] { "ACCOUNT", code, "NAME", name });
            }
        }

        // Получаем дату отчета
        private int GetDate(string[] fileContent)
        {
            if ((this.Region == RegionName.Yaroslavl) || (this.Region == RegionName.Novosibirsk) || (this.Region == RegionName.Karelya) ||
                (this.Region == RegionName.SamaraGO) || (this.Region == RegionName.AltayKrai) ||
                ((this.Region == RegionName.Naur) && (this.DataSource.Year >= 2010)))
            {
                string[] cells = fileContent[3].Split(new char[] { '|' });
                int year = Convert.ToInt32(cells[4].Split(new char[] { '.' })[2]);
                string month = cells[6].PadLeft(2, '0');
                if (month == "12")
                    year--;
                return CommonRoutines.ShortDateToNewDate(string.Format("00{0}{1}", month, year));
            }
            else if (this.Region == RegionName.Naur)
            {
                // для Наура дата находится на строке с маркером IK (4-я строка)
                // между 3 и 4 символом "|" в формате ДД.ММ.ГГГГ
                string[] cells = fileContent[3].Split(new char[] { '|' });
                return DecrementMonth(CommonRoutines.ShortDateToNewDate(cells[3].Trim()));
            }
            return (this.DataSource.Year * 10000 + this.DataSource.Month * 100);
        }

        private int GetLocBdgt(string[] fileContent)
        {
            if ((this.Region == RegionName.Yaroslavl) || (this.Region == RegionName.AltayKrai) || (this.Region == RegionName.Karelya) ||
                ((this.Region == RegionName.Naur) && (this.DataSource.Year >= 2010)) ||
                (this.Region == RegionName.SamaraGO))
            {
                string[] fields = fileContent[3].Split(new char[] { '|' });
                if (fields[9] == string.Empty)
                    return -1;
                return PumpOriginalRow(dsLocBdgt, clsLocBdgt,
                    new object[] { "ACCOUNT", "Неуказанный счет", "NAME", fields[9].Trim(), "OKATO", "0" });
            }
            return -1;
        }

        private decimal GetForPeriod(string[] fields)
        {
            if ((this.Region == RegionName.Naur) && (this.DataSource.Year < 2010))
                return CleanFactValue(fields[4]);
            return CleanFactValue(fields[5]);
        }

        private decimal GetFromBeginYear(string[] fields)
        {
            if ((this.Region == RegionName.Naur) && (this.DataSource.Year < 2010))
                return CleanFactValue(fields[5]);
            return CleanFactValue(fields[6]);
        }

        private int GetRefLocBdgtNaur(string[] fields)
        {
            string name = fields[2].Trim();
            return PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, name,
                new object[] { "ACCOUNT", "Неуказанный счет", "NAME", name, "OKATO", "0" });
        }

        private bool IsLocBdgtForNaur(string[] fields)
        {
            if ((this.Region != RegionName.Naur) || (this.DataSource.Year >= 2010))
                return false;
            return (fields[0].Trim().ToUpper() == "IKBUD");
        }

        private void PumpTxtFile(FileInfo file)
        {
            // Получаем все строки файла
            string[] fileContent = CommonRoutines.GetFileContent(file, Encoding.GetEncoding(1251));
            // Если в файле кроме заголовка ничего нет, то он пуст
            if (fileContent.GetLength(0) < 5)
            {
                throw new Exception(string.Format("файл {0} пуст", file.Name));
            }

            int refDate = GetDate(fileContent);
            CheckDataSourceByDate(refDate, true);

            globalLocBdgtID = GetLocBdgt(fileContent);

            int totalRows = fileContent.GetLength(0);
            totalRowsCount = totalRows;
            // Строки заголовка считаем пропущенными
            skippedRowsCount = 4;

            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);

            // Бежим по полученным стрoкам и качаем данные
            for (int curRow = 4; curRow < totalRows; curRow++)
            {
                processedRowsCount++;

                SetProgress(totalRows, curRow + 1,
                    string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                    string.Format("Строка {0} из {1}", curRow + 1, totalRows));

                try
                {
                    // Надо обрабатывать только те строки, в которых заполнены все шесть полей
                    string[] fields = fileContent[curRow].Split(new char[] { '|' });

                    if (IsLocBdgtForNaur(fields))
                    {
                        globalLocBdgtID = GetRefLocBdgtNaur(fields);
                        continue;
                    }

                    if (!IsSkippedRow(fields)) //метод написан через ****. олгика обратная заявленному названию
                    {
                        skippedRowsCount++;
                        continue;
                    }

                    int kdID = PumpKD(fields);
                    int locBdgtID = PumpLocBdgt(fields);

                    PumpRow(dsUFK71N.Tables[0], new object[] {
                        "FORPERIOD", GetForPeriod(fields),
                        "FROMBEGINYEAR", GetFromBeginYear(fields),
                        "RefYearDayUNV", refDate,
                        "REFKD", kdID,
                        "REFLOCBDGT", locBdgtID });

                    pumpedRowsCount++;
                }
                catch (Exception ex)
                {
                    WriteToTrace(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", curRow + 1, ex), TraceMessageKind.Error);
                    throw new Exception(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", curRow + 1, ex.Message), ex);
                }

                if (dsUFK71N.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daUFK71N, ref dsUFK71N);
                }
            }
        }

        #endregion Закачка Txt отчетов

        #region закачка xls отчетов

        /// <summary>
        /// Проверяет страницу отчета на соответствие нужному формату
        /// </summary>
        /// <param name="sheet">Страница</param>
        /// <returns>Соответствует или нет</returns>
        private bool CheckSheetFormat(object sheet, out int date)
        {
            date = 0;

            // Проверяем, чтобы в А1 было записано «Сводная ведомость»
            if (String.Compare(excelHelper.GetCell(sheet, "A1").Value, "Сводная ведомость", true) != 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Страница не соответствует формату - ячейка А1 не содержит \"Сводная ведомость\""));
                return false;
            }

            // Чтобы в А2 было записано «по кассовым поступлениям» и дальше без разницы, тоже может быть текст
            if (excelHelper.GetCell(sheet, "A1").Value.ToUpper().StartsWith("ПО КАССОВЫМ ПОСТУПЛЕНИЯМ"))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Страница не соответствует формату - ячейка А2 не содержит \"По кассовым поступлениям\""));
                return false;
            }

            // Чтобы в А3 было записано «(месячная)»
            if (String.Compare(excelHelper.GetCell(sheet, "A3").Value, "(месячная)", true) != 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Страница не соответствует формату - ячейка А3 не содержит \"(месячная)\""));
                return false;
            }

            // Дальше у нас есть две даты – одна в ячейке Н4, вторая в А4. Берем ту, которая в Н4. Там должна быть дата в формате ДД.ММ.ГГГГ.
            // Если не дата, то ругаемся и говорим, что не тот формат файла.
            try
            {
                date = CommonRoutines.ShortDateToNewDate(excelHelper.GetCell(sheet, "H4").Value);
            }
            catch
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Страница не соответствует формату - ячейка H4 содержит дату неверного формата"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Возвращает нижнюю границу отчета на странице
        /// </summary>
        /// <param name="sheet">Страница</param>
        /// <returns>Нижняя граница</returns>
        private int GetSheetBottomMargin(object sheet)
        {
            for (int i = 12; i <= 50000; i++)
            {
                if (excelHelper.GetCell(sheet, i, 1).Value.Trim().ToUpper().StartsWith("ИТОГО"))
                {
                    return i - 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Закачивает файл екселя
        /// </summary>
        /// <param name="file">Файл</param>
        private void PumpXLSFile(FileInfo file)
        {
            // Загружаем файл
            object workbook = excelHelper.GetWorkbook(excelObj, file.FullName, true);

            try
            {
                // Загружаем первую страницу
                int sheetIndex = 1;
                object sheet = excelHelper.GetSheet(workbook, sheetIndex);

                int date = 0;
                string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);

                // Обходим все страницы
                while (sheet != null)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, string.Format("Обработка страницы {0} отчета.", sheetIndex));

                    int bottomMargin = GetSheetBottomMargin(sheet);
                    if (bottomMargin < 0)
                    {
                        WriteToTrace(string.Format("Не найдена нижняя граница страницы {0} отчета.", sheetIndex), TraceMessageKind.Error);
                        throw new Exception(string.Format("Не найдена нижняя граница страницы {0} отчета.", sheetIndex));
                    }
                    else
                    {
                        totalRowsCount += bottomMargin - 11;

                        // Закачиваем данные страницы
                        if (CheckSheetFormat(sheet, out date))
                        {
                            if (!CheckDataSourceByDate(date, false))
                            {
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                                    string.Format("Страница {0} пропущена из-за несоответствия даты параметрам источника.", sheetIndex));

                                skippedRowsCount += bottomMargin - 11;

                                // Получаем следующую страницу
                                sheetIndex++;
                                sheet = excelHelper.GetSheet(workbook, sheetIndex);
                                continue;
                            }

                            // Обнуляем день
                            date = (date / 100) * 100;

                            // Во всех имеющихся образцах непосредственно данные начинаются со строки 12
                            for (int i = 12; i <= bottomMargin; i++)
                            {
                                processedRowsCount++;

                                SetProgress(bottomMargin, i,
                                    string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                                    string.Format("Строка {0} из {1}", i, bottomMargin));

                                try
                                {
                                    string code = excelHelper.GetCell(sheet, i, 2).Value.Trim().PadLeft(1, '0');
                                    int kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code, new object[] { "CODESTR", code });

                                    string name = excelHelper.GetCell(sheet, i, 1).Value.Trim();
                                    if (name == string.Empty)
                                    {
                                        name = constDefaultClsName;
                                    }
                                    int locBdgtID = PumpOriginalRow(dsLocBdgt, clsLocBdgt, new object[] { "ACCOUNT", 0, "NAME", name });

                                    PumpRow(dsUFK71N.Tables[0], new object[] {
                                        "FORPERIOD", excelHelper.GetCell(sheet, i, 3).Value.Trim().PadLeft(1, '0'),
                                        "FROMBEGINYEAR", excelHelper.GetCell(sheet, i, 7).Value.Trim().PadLeft(1, '0'),
                                        "RefYearDayUNV", date, "REFKD", kdID, "REFLOCBDGT", locBdgtID });
                                    pumpedRowsCount++;

                                    if (dsUFK71N.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                                    {
                                        UpdateData();
                                        ClearDataSet(daUFK71N, ref dsUFK71N);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteToTrace(string.Format(
                                        "При обработке строки {0} возникла ошибка ({1})", i, ex), TraceMessageKind.Error);
                                    throw new  Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", i, ex.Message), ex);
                                }
                            }

                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                                string.Format("Страница {0} отчета успешно обработана.", sheetIndex));
                        }
                        else
                        {
                            skippedRowsCount += bottomMargin - 11;
                        }

                        // Получаем следующую страницу
                        sheetIndex++;
                        sheet = excelHelper.GetSheet(workbook, sheetIndex);
                    }
                }
            }
            finally
            {
                excelHelper.CloseWorkBooks(excelObj);
            }
        }

        #endregion закачка xls отчетов

        #region закачка xls отчетов ставрополя

        private int GetDateRef(object sheet)
        {
            string date = string.Empty;
            switch (this.Region)
            {
                case RegionName.Stavropol:
                    date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, 8, 1).Value);
                    break;
                case RegionName.Tambov:
                case RegionName.Penza:
                    date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, 4, 8).Value);
                    break;
                case RegionName.EAO:
                    if (this.DataSource.Year >= 2009)
                        date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, 4, 7).Value);
                    else
                        date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, 4, 8).Value);
                    break;
                case RegionName.HMAO:
                case RegionName.Chechnya:
                case RegionName.AltayKrai:
                    date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, 4, 7).Value);
                    break;
            }
            int dateInt = CommonRoutines.ShortDateToNewDate(date);
            if (this.Region == RegionName.HMAO || this.Region == RegionName.EAO || this.Region == RegionName.Chechnya || this.Region == RegionName.AltayKrai)
                dateInt = CommonRoutines.DecrementDateWithLastDay(dateInt);
            return dateInt / 100 * 100;
        }

        private const string TOTAL_VALUE = "ИТОГО";
        private const string REGION_VALUE = "РБ";
        private bool IsTotalRow(string cellValue)
        {
            return (cellValue.StartsWith(TOTAL_VALUE) || cellValue.Contains(REGION_VALUE));
        }

        private const string TABLE_TITLE = "1";
        private bool IsTableTitle(string cellValue)
        {
            return (cellValue == TABLE_TITLE);
        }

        private const string REPORT_END = "НАЧАЛЬНИ";
        private const string REPORT_END_2 = "ВСЕГО";
        private const string REPORT_END_3 = "ИСПОЛНИТЕЛ";
        private bool IsReportEnd(string cellValue)
        {
            return (cellValue.StartsWith(REPORT_END) || cellValue.StartsWith(REPORT_END_2) ||
                cellValue.StartsWith(REPORT_END_3));
        }

        private int PumpKd(int curRow, object sheet)
        {
            string code = string.Empty;
            if (this.Region == RegionName.HMAO || this.Region == RegionName.Chechnya || this.Region == RegionName.AltayKrai ||
                (this.Region == RegionName.EAO && this.DataSource.Year == 2009))
                code = excelHelper.GetCell(sheet, curRow, 3).Value.Trim();
            else
                code = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
            string name = constDefaultClsName;
            return PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code, new object[] { "CodeStr", code, "Name", name });
        }

        private const string NULL_ACCOUNT = "Неуказанный счет";
        private int PumpLocBdgt(int curRow, object sheet, int locBdgtParentId)
        {
            string name = excelHelper.GetCell(sheet, curRow, 1).Value.Trim().TrimStart('0').PadLeft(1, '0');
            string okato = name;

            switch (this.Region)
            {
                case RegionName.Tambov:
                case RegionName.Penza:
                case RegionName.EAO:
                case RegionName.HMAO:
                case RegionName.Chechnya:
                case RegionName.AltayKrai:
                    okato = "0";
                    break;
            }

            object[] mapping = new object[] { "Okato", okato, "Name", name, "Account", NULL_ACCOUNT };
            if (locBdgtParentId != -1)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentId", locBdgtParentId });
            return PumpCachedRow(locBdgtOkatoCache, dsLocBdgt.Tables[0], clsLocBdgt, name, mapping);
        }

        private void PumpXlsRow(int curRow, object sheet, int refDate, int locBdgtParentId)
        {
            int refKd = PumpKd(curRow, sheet);
            int refLocBdgt = PumpLocBdgt(curRow, sheet, locBdgtParentId);
            decimal forPeriod = 0;
            decimal fromBeginYear = 0;
            switch (this.Region)
            {
                case RegionName.Stavropol:
                    fromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 4).Value.Trim().PadLeft(1, '0'));
                    forPeriod = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 3).Value.Trim().PadLeft(1, '0'));
                    break;
                case RegionName.Tambov:
                case RegionName.Penza:
                    fromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 7).Value.Trim().PadLeft(1, '0'));
                    forPeriod = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 3).Value.Trim().PadLeft(1, '0'));
                    break;
                case RegionName.EAO:
                    if (this.DataSource.Year >= 2009)
                    {
                        fromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 5).Value.Trim().PadLeft(1, '0'));
                        forPeriod = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 4).Value.Trim().PadLeft(1, '0'));
                    }
                    else
                    {
                        fromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 7).Value.Trim().PadLeft(1, '0'));
                        forPeriod = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 3).Value.Trim().PadLeft(1, '0'));
                    }
                    break;
                case RegionName.HMAO:
                case RegionName.Chechnya:
                case RegionName.AltayKrai:
                    fromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 5).Value.Trim().PadLeft(1, '0'));
                    forPeriod = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 4).Value.Trim().PadLeft(1, '0'));
                    break;
            }
            if ((forPeriod == 0) && (fromBeginYear == 0))
                return;
            object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefKD", refKd, "RefLocBdgt", refLocBdgt,
                "forPeriod", forPeriod, "FromBeginYear", fromBeginYear };
            PumpRow(dsUFK71N.Tables[0], mapping);
            if (dsUFK71N.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK71N, ref dsUFK71N);
            }
        }

        private int PumpParentLocBdgt(string value)
        {
            int index = value.IndexOf(REGION_VALUE);
            string name = value.Remove(0, index);
            return PumpRow(dsLocBdgt.Tables[0], clsLocBdgt, new object[] { "Okato", 0, "Name", name, "Account", NULL_ACCOUNT });
        }

        private void PumpXlsSheetData(string fileName, object sheet)
        {
            int refDate = GetDateRef(sheet);
            bool toPumpRow = false;
            int locBdgtParentId = -1;
            if (this.Region == RegionName.Stavropol)
                locBdgtParentId = PumpParentLocBdgt(excelHelper.GetCell(sheet, 7, 1).Value);
            for (int curRow = 1; ; curRow++)
                try
                {
                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.ToUpper().Trim();
                    if (cellValue == string.Empty)
                        continue;
                    if (IsTableTitle(cellValue))
                    {
                        toPumpRow = true;
                        continue;
                    }
                    if (IsTotalRow(cellValue))
                        continue;
                    if (IsReportEnd(cellValue))
                        return;
                    if (toPumpRow)
                        PumpXlsRow(curRow, sheet, refDate, locBdgtParentId);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} листа {1} файла {2} возникла ошибка ({3})",
                        curRow, excelHelper.GetSheetName(sheet), fileName, ex.Message), ex);
                }
        }

        private void PumpStavropolXlsFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int i = 1; i <= sheetCount; i++)
                {
                    object sheet = excelHelper.GetSheet(workbook, i);
                    PumpXlsSheetData(file.Name, sheet);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion закачка xls отчетов ставрополя

        #region Перекрытые методы закачки

        private void PumpFiles(DirectoryInfo dir, ReportFormat reportFormat)
        {
            FileInfo[] files = null;

            switch (reportFormat)
            {
                case ReportFormat.Excel:
                    // Находим все файлы
                    files = dir.GetFiles("*.xls", SearchOption.AllDirectories);

                    if (files.Length > 0)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
                        excelHelper = new ExcelHelper();
                        excelObj = excelHelper.OpenExcel(false);
                        excelHelper.AskToUpdateLinks = false;
                        excelHelper.DisplayAlerts = false;
                        excelHelper.EnableEvents = false;
                    }
                    break;

                case ReportFormat.Text:
                    // Находим все текстовые файлы
                    if (this.DataSource.Year >= 2009)
                    {
                        switch (this.Region)
                        {
                            case RegionName.AltayKrai:
                            case RegionName.EAO:
                            case RegionName.Kostroma:
                            case RegionName.Novosibirsk:
                            case RegionName.Orenburg:
                            case RegionName.SamaraGO:
                            case RegionName.Saratov:
                            case RegionName.Karelya:
                            case RegionName.Yaroslavl:
                                files = dir.GetFiles("*.ip*", SearchOption.AllDirectories);
                                break;
                            case RegionName.Naur:
                                if (this.DataSource.Year >= 2010)
                                    files = dir.GetFiles("*.ip*", SearchOption.AllDirectories);
                                else
                                    files = dir.GetFiles("*.ik*", SearchOption.AllDirectories);
                                break;
                            default:
                                files = dir.GetFiles("*.txt", SearchOption.AllDirectories);
                                break;
                        }
                    }
                    else
                    {
                        files = dir.GetFiles("*.txt", SearchOption.AllDirectories);
                    }
                    // В источнике может лежат только один файл
                    /*if (files.GetLength(0) > 1)
                    {
                        throw new Exception("в источнике находится более одного файла");
                    }*/
                    break;
            }

            try
            {
                // Обходим все найденные файлы, разбираем и закачиваем данные
                for (int i = 0; i < files.GetLength(0); i++)
                {
                    totalRowsCount = 0;
                    processedRowsCount = 0;
                    pumpedRowsCount = 0;
                    skippedRowsCount = 0;

                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeStartFilePumping, string.Format("Старт обработки файла {0}.", files[i].FullName));

                    try
                    {
                        switch (reportFormat)
                        {
                            case ReportFormat.Excel:
                                PumpXLSFile(files[i]);
                                break;
                            case ReportFormat.Text:
                                PumpTxtFile(files[i]);
                                break;
                        }

                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                                "Обработка файла {0} успешно завершена. Всего строк файла: {1}. Обработано строк: {2}, из которых закачано {3}, пропущено {4}.",
                                files[i].FullName, totalRowsCount, processedRowsCount, pumpedRowsCount, skippedRowsCount));
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                                "Обработка файла {0} завершена с ошибками: {1}. Всего строк файла: {2}. " +
                                "На момент возникновения ошибки обработано строк {3}, из которых закачано {4}, пропущено {5}.",
                                files[i].FullName, ex.Message, totalRowsCount, processedRowsCount, pumpedRowsCount, skippedRowsCount));
                        throw;
                    }
                }
            }
            finally
            {
                if (excelHelper != null)
                {
                    excelHelper.CloseExcel(ref excelObj);
                    excelHelper.Dispose();
                }
            }
        }

        private void ProcessStavropolFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpStavropolXlsFile), false);
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
        	switch (this.Region)
        	{
        		case RegionName.Stavropol:
        		case RegionName.Tambov:
        		case RegionName.Penza:
                    ProcessStavropolFiles(dir);
                    break;
        		case RegionName.EAO:
                    ProcessStavropolFiles(dir);
                    PumpFiles(dir, ReportFormat.Text);
        			break;
                case RegionName.HMAO:
                case RegionName.Chechnya:
                    if (this.DataSource.Year >= 2009)
                    {
                        ProcessStavropolFiles(dir);
                    }
                    else
                    {
                        PumpFiles(dir, ReportFormat.Text);
                        PumpFiles(dir, ReportFormat.Excel);
                    }
                    break;
                case RegionName.AltayKrai:
                case RegionName.Naur:
                    PumpFiles(dir, ReportFormat.Text);
                    if (this.Region == RegionName.AltayKrai)
                        ProcessStavropolFiles(dir);
                    break;
                default:
                    PumpFiles(dir, ReportFormat.Text);
                    PumpFiles(dir, ReportFormat.Excel);
                    break;
        	}
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Удаление данных

        protected override void DeleteEarlierPumpedData()
        {
            DeletePumpedData();
        }

        #endregion Удаление данных
    }
}