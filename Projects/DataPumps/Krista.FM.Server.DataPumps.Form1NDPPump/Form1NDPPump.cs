using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.Form1NDPPump
{
    /// <summary>
    /// УФК_0008_1Н_DP_Реестр перечисленных поступлений
    /// </summary>
    public partial class Form1NDPPumpModule : DataPumpModuleBase
    {
        #region Поля

        #region Классификаторы

        // КД.УФК
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null; // new Dictionary<string, int>(1000);
        private int nullKD;

        // Местные бюджеты.УФК
        private IDbDataAdapter daLocBdgt;
        private DataSet dsLocBdgt;
        private IClassifier clsLocBdgt;
        private Dictionary<string, DataRow> locBdgtCache = null; // new Dictionary<string, DataRow>(1000);
        private int nullLocBdgt;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_1Н_DP Реестр перечисленных поступлений
        private IDbDataAdapter daUFK8DP;
        private DataSet dsUFK8DP;
        private IFactTable fctUFK8DP;

        #endregion Факты

        private Formats1nParser formats1nParser = new Formats1nParser();

        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;

        private int totalRowsCount;
        private int processedRowsCount;
        private int pumpedRowsCount;
        private int skippedRowsCount;

        private List<int> deletedDate = new List<int>();

        #endregion Поля

        #region Константы

        // Количество записей для занесения в базу
        private const int constMaxQueryRecords = 10000;

        #endregion Константы

        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public Form1NDPPumpModule()
            : base()
        {

        }

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (formats1nParser != null)
                    formats1nParser.Dispose();
                if (deletedDate != null)
                    deletedDate.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация

        #region Закачка данных

        #region Работа с базой и кэшами

        /// <summary>
        /// Запрос данных из базы
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD);
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daLocBdgt, ref dsLocBdgt, clsLocBdgt);
            nullLocBdgt = clsLocBdgt.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daUFK8DP, ref dsUFK8DP, fctUFK8DP);

            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref locBdgtCache, dsLocBdgt.Tables[0], new string[] { "ACCOUNT" });
        }

        /// <summary>
        /// Внести изменения в базу
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daLocBdgt, dsLocBdgt, clsLocBdgt);
            UpdateDataSet(daUFK8DP, dsUFK8DP, fctUFK8DP);
        }

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_LOC_BDGT_UFK_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string F_D_UFK8_DP_GUID = "80862f23-2087-4493-bb1e-a78b8670d7bb";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsLocBdgt = this.Scheme.Classifiers[D_LOC_BDGT_UFK_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctUFK8DP = this.Scheme.FactTables[F_D_UFK8_DP_GUID] };
        }

        /// <summary>
        /// Функция выполнения завершающих действий этап
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK8DP);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsLocBdgt);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        private void PumpFiles(DirectoryInfo dir)
        {
            PumpTXTFiles(dir);

            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);

            // если есть, распаковываем архивы rar
            FileInfo[] rarFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo rarFile in rarFiles)
            {
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(rarFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    PumpFiles(tempDir);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
            }
        }

        #endregion Общие функции закачки

        #region Работа с Txt

        private int GetDateTxt(string value)
        {
            return CommonRoutines.ShortDateToNewDate(value);
        }

        private void PumpTxtRow(string[] fileData, int curRow, int refDate, int refLocBdgt)
        {
            int refKd = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, fileData[curRow], "CODESTR", null);
            double sum = CommonRoutines.ReduceDouble(fileData[curRow + 2]);

            PumpRow(dsUFK8DP.Tables[0], new object[] { 
                "FORPERIOD", sum, "RefYearDayUNV", refDate, "REFKD", refKd, "REFLOCBDGT", refLocBdgt });

            if (dsUFK8DP.Tables[0].Rows.Count >= constMaxQueryRecords)
            {
                UpdateData();
                ClearDataSet(daUFK8DP, ref dsUFK8DP);
            }
        }

        /// <summary>
        /// Закачивает данные текстового файла (для ЕАО)
        /// </summary>
        /// <param name="file">Файл</param>
        private void PumpTXTFileEAO(FileInfo file)
        {
            string[] fileData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());

            int refDate = -1;
            int refKd = nullKD;
            int refLocBdgt = nullLocBdgt;
            totalRowsCount = fileData.Length;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int i = 0; i < totalRowsCount; i++)
            {
                processedRowsCount++;
                SetProgress(totalRowsCount, i + 1,
                    string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                    string.Format("Строка {0} из {1}", i + 1, totalRowsCount));

                try
                {
                    string[] fields = fileData[i].Split(new char[] { '|' });
                    if (fields.GetLength(0) == 0)
                        continue;

                    if (fields[0].Trim().ToUpper() == "DPST")
                    {
                        refKd = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, fields[2], "CODESTR", null);
                        decimal forPeriod = CommonRoutines.ReduceDecimal(fields[4]);
                        PumpRow(dsUFK8DP.Tables[0], new object[] { "FORPERIOD", forPeriod,
                            "RefYearDayUNV", refDate, "REFKD", refKd, "REFLOCBDGT", refLocBdgt });
                        pumpedRowsCount++;
                        continue;
                    }

                    if (fields[0].Trim().ToUpper() == "DP")
                    {
                        refDate = CommonRoutines.ShortDateToNewDate(fields[3]);
                        CheckDataSourceByDate(refDate, true);
                        if (!deletedDate.Contains(refDate))
                        {
                            DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                            deletedDate.Add(refDate);
                        }
                        refLocBdgt = PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, fields[15],
                            new object[] { "ACCOUNT", fields[15], "NAME", fields[7], "OKATO", 0 });
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", i + 1, ex), TraceMessageKind.Error);
                    throw new Exception(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", i + 1, ex.Message), ex);
                }
            }
        }

        /// <summary>
        /// Закачивает данные текстового файла (для Ярославля)
        /// </summary>
        /// <param name="file">Файл</param>
        private const string DATE_ROW_MARK = "ДАТА";
        private const string ACCOUNT_ROW_MARK = "НОМЕР СЧЕТА";
        private const string BDGTNAME_ROW_MARK = "НАИМЕНОВАНИЕ БЮДЖЕТА";
        private const string START_ROW_MARK = "3";
        private const string TOTAL_ROW_MARK = "ИТОГО";
        private void PumpTXTFileYaroslavl(FileInfo file)
        {
            string[] fileData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());

            int refDate = 0;
            bool toPumpRow = false;
            int refLocBdgt = nullLocBdgt;
            string bdgtName = string.Empty;
            totalRowsCount = fileData.Length;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int i = 0; i < totalRowsCount; i++)
            {
                SetProgress(totalRowsCount, i + 1,
                    string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                    string.Format("Строка {0} из {1}", i + 1, totalRowsCount));

                try
                {
                    string value = fileData[i].Trim().ToUpper();
                    if (value == string.Empty)
                        continue;

                    if (value == TOTAL_ROW_MARK)
                    {
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        PumpTxtRow(fileData, i, refDate, refLocBdgt);
                        i += 2;
                        continue;
                    }

                    if (value == DATE_ROW_MARK)
                    {
                        refDate = GetDateTxt(fileData[i + 1]);
                        continue;
                    }

                    if (value == BDGTNAME_ROW_MARK)
                    {
                        bdgtName = fileData[i + 1].Trim();
                        continue;
                    }

                    if (value == ACCOUNT_ROW_MARK)
                    {
                        string account = fileData[i + 1].Trim();
                        refLocBdgt = PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, account,
                            new object[] { "OKATO", 0, "ACCOUNT", account, "NAME", bdgtName });
                        continue;
                    }

                    if (value == START_ROW_MARK)
                    {
                        toPumpRow = true;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", i + 1, ex), TraceMessageKind.Error);
                    throw new Exception(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", i + 1, ex.Message), ex);
                }
            }
        }

        /// <summary>
        /// Закачивает данные текстового файла
        /// </summary>
        /// <param name="file">Файл</param>
        private void PumpTXTFile(FileInfo file)
        {
            // Вытаскиваем данные из файла
            Format1nLinearData fileData = formats1nParser.ParseLinearFile(file, Format1n.DPR);
            if (fileData.Count == 0)
            {
                throw new Exception("нет данных для закачки");
            }

            // Закачиваем
            // Смотрим номер версии файла (FK|NUM_VER). Номер версии «2005.12» для нас родной. 
            // Если другой – выдаем предупреждение в протокол и пишем там номер версии который ожидали и найденный.
            if (fileData[0].BlockName != "FK")
            {
                throw new Exception("не найдена секция FK");
            }
            else if (fileData[0].BlockData["NUM_VER"] != "2005.12")
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, 
                    string.Format("Версия файла {0} - ожидается 2005.12.", fileData[0].BlockData["NUM_VER"]));
            }

            totalRowsCount = fileData.Count;

            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int locBdgtID = nullLocBdgt;
            int date = 0;
            
            for (int i = 1; i < fileData.Count; i++)
            {
                processedRowsCount++;

                SetProgress(fileData.Count, i + 1,
                    string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                    string.Format("Строка {0} из {1}", i + 1, fileData.Count));

                try
                {
                    if (String.Compare(fileData[i].BlockName, "DP", true) == 0)
                    {
                        // По дате реестра формируем ссылку на классификатор «Год-Месяц-День»
                        date = CommonRoutines.ShortDateToNewDate(fileData[i].BlockData["DP_DATE"]);
                        CheckDataSourceByDate(date, true);

                        if (!deletedDate.Contains(date))
                        {
                            DeleteData(string.Format("RefYearDayUNV = {0}", date), string.Format("Дата отчета: {0}.", date));
                            deletedDate.Add(date);
                        }

                        string code = fileData[i].BlockData["POL_ACC"];
                        string name = fileData[i].BlockData["NAME_FO"];

                        // Если такая запись уже присутствует, то запись не добавляем. НО! Если такая запись уже 
                        // присутствует, а наименование у нее «местный бюджет» (без уче-та регистра и пробелов). 
                        // А в той записи, с которой мы работаем – не «мест-ный бюджет». То наименование заменяем нашим, смысловым
                        if (String.Compare(name, "местный бюджет", true) == 0)
                        {
                            locBdgtID = PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, code,
                                new object[] { "ACCOUNT", code, "NAME", name });
                        }
                        else
                        {
                            locBdgtID = RepumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, code,
                                new object[] { "ACCOUNT", code, "NAME", name });
                        }

                        continue;
                    }

                    if (date > 0 && String.Compare(fileData[i].BlockName, "DPR", true) == 0)
                    {
                        int kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, fileData[i].BlockData["KOD_DOH"], "CODESTR", null);
                        double sum = CommonRoutines.ReduceDouble(fileData[i].BlockData["STR_SUM"]) / 100;

                        PumpRow(dsUFK8DP.Tables[0], new object[] { 
                            "FORPERIOD", sum, "RefYearDayUNV", date, "REFKD", kdID, "REFLOCBDGT", locBdgtID });

                        pumpedRowsCount++;
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", i + 1, ex), TraceMessageKind.Error);
                    throw new Exception(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", i + 1, ex.Message), ex);
                }

                if (dsUFK8DP.Tables[0].Rows.Count >= constMaxQueryRecords)
                {
                    UpdateData();
                    ClearDataSet(daUFK8DP, ref dsUFK8DP);
                }
            }
        }

        /// <summary>
        /// Закачивает текстовые файлы
        /// </summary>
        /// <param name="dir">Каталог с файлами</param>
        private void PumpTXTFiles(DirectoryInfo dir)
        {
            // Распаковываем архивы
            CommonRoutines.ExtractArchiveFiles(dir.FullName, dir.FullName, ArchivatorName.Arj, FilesExtractingOption.SeparateSubDirs);

            try
            {
                // Находим все текстовые файлы
                FileInfo[] files = dir.GetFiles("*.dp?", SearchOption.AllDirectories);

                deletedDate.Clear();

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
                        switch (this.Region)
                        {
                            case RegionName.EAO:
                                PumpTXTFileEAO(files[i]);
                                break;
                            case RegionName.Yaroslavl:
                                PumpTXTFileYaroslavl(files[i]);
                                break;
                            default:
                                PumpTXTFile(files[i]);
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
                                "Обработка файла {0} завершена с ошибками. Всего строк файла: {1}. " +
                                "На момент возникновения ошибки обработано строк {2}, из которых закачано {3}, пропущено {4}.",
                                files[i].FullName, totalRowsCount, processedRowsCount, pumpedRowsCount, skippedRowsCount), ex);
                        throw;
                    }
                }
            }
            finally
            {
                CommonRoutines.DeleteExtractedDirectories(dir);
            }
        }

        #endregion Работа с Txt

        #region Работа с Excel

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 1;
            while (emptyStrCount < 10)
            {
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim() == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
                curRow++;
            }
            return (curRow - 10);
        }

        private int GetDate(object sheet)
        {
            string value = excelHelper.GetCell(sheet, 3, 8).Value.Trim();
            return CommonRoutines.ShortDateToNewDate(value);
        }

        private int GetRefLocBdgt(object sheet)
        {
            switch (this.Region)
            {
                case RegionName.EAO:
                case RegionName.Yaroslavl:
                    string name = excelHelper.GetCell(sheet, 6, 2).Value.Trim();
                    string account = excelHelper.GetCell(sheet, 9, 8).Value.Trim();
                    return PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, account,
                        new object[] { "ACCOUNT", account, "NAME", name, "OKATO", 0 });
                default:
                    return nullLocBdgt;
            }
        }

        /// <summary>
        /// Закачка классификатора КД
        /// </summary>
        /// <param name="curRow">Номер текущей строки</param>
        /// <param name="sheet">Лист</param>
        /// <returns></returns>
        private int PumpKD(int curRow, object sheet)
        {
            string code = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
            
            object[] mapping = new object[] { "CODESTR", code };
            
            return PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, mapping, code, "ID");
        }

        /// <summary>
        /// Закачка строки Excel-файла
        /// </summary>
        /// <param name="curRow">Номер текущей строки</param>
        /// <param name="sheet">Лист</param>
        private void PumpXLSRow(int curRow, object sheet, int refDate, int refLocBdgt)
        {
            int refKD = PumpKD(curRow, sheet);
            decimal sum = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 5).Value.Trim());

            object[] mapping = new object[] { "FORPERIOD", sum, "RefYearDayUNV", refDate, "REFKD", refKD, "REFLOCBDGT", refLocBdgt };

            PumpRow(dsUFK8DP.Tables[0], mapping);
            if (dsUFK8DP.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK8DP, ref dsUFK8DP);
            }
        }

        /// <summary>
        /// Закачка листа Excel-файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="sheet">Лист</param>
        private const string TOTAL_ROW = "ИТОГО";
        private void PumpXLSSheetData(string fileName, object sheet)
        {
            int refDate = GetDate(sheet);
            int refLocBdgt = GetRefLocBdgt(sheet);
            
            if (!deletedDate.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDate.Add(refDate);
            }

            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            for (int curRow = 0; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow, string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.ToUpper().Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (cellValue.Contains(TOTAL_ROW))
                        toPumpRow = false;

                    if (toPumpRow)
                        PumpXLSRow(curRow, sheet, refDate, refLocBdgt);

                    if (cellValue == "1")
                    {
                        toPumpRow = true;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXLSFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int i = 1; i <= sheetCount; i++)
                {
                    object sheet = excelHelper.GetSheet(workbook, i);
                    rowsCount = GetRowsCount(sheet);
                    PumpXLSSheetData(file.Name, sheet);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        /// <summary>
        /// Закачка файлов
        /// </summary>
        /// <param name="dir">Каталог с файлами</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных
    }
}