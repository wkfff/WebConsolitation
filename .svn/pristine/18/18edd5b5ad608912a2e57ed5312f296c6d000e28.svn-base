using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.Form16Pump
{
    // УФК_0001_ФОРМА 16 (из за ебнутой постановки по новосибирску и сжатых сроков модуль изгажен пездец как... надо переписать нормально!!!)
    // но при такой постановке кроме подобной поебени ничего и не получится
    public class Form16PumpModule : TextRepPumpModuleBase
    {
        #region Поля

        #region Классификаторы

        // Районы.УФК Форма 16 (d_Regions_Form16)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;
        // КД.УФК Форма 16 (d_KD_Form16)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> kdCache = null;
        // Районы.Служебный для закачки (d_Regions_ForPump)
        private IDbDataAdapter daRegionForPump;
        private DataSet dsRegionForPump;
        private IClassifier clsRegionForPump;
        private Dictionary<string, string> regionForPumpCache = null;
        // Местные бюджеты.УФК (d_LocBdgt_UFK)
        private IDbDataAdapter daLocBdgt;
        private DataSet dsLocBdgt;
        private IClassifier clsLocBdgt;
        private Dictionary<string, int> locBdgtCache = null;
        private Dictionary<int, int> locBdgtCacheById = null;

        #endregion Классификаторы

        #region Факты

        // Факт.Форма 16 (f_F_IncomesForm16)
        private IDbDataAdapter daIncomesForm16;
        private DataSet dsIncomesForm16;
        private IFactTable fctIncomesForm16;
        // Факт.Форма 16_Поселения (f_F_IncomesForm16Det)
        private IDbDataAdapter daIncomesForm16Det;
        private DataSet dsIncomesForm16Det;
        private IFactTable fctIncomesForm16Det;

        #endregion Факты

        private Database dbfDB = null;
        private DBDataAccess dbDataAccess = new DBDataAccess();
        private decimal[] controlSums = null;
        private bool disintAll = false;
        private NovosibFileType novosibFileType;
        private string archName = string.Empty;
        private int year = -1;
        private int month = -1;
        private int regForPumpSourceID;
        private List<string> regionForPumpCodes = null;
        private bool pumpOnlyRegion = false;
        private List<int> deletedDateList = null;
        private bool pumpOnlyNOColumns = false;
        private List<int> pumpedRegions = new List<int>();
        // находится ли дата в ячейке Z4
        private bool isZDateCell = false;
        private bool isNewKarelyaXlsFormat = false;
        private bool isDetailData = false;

        #endregion Поля

        #region Константы

        // типы закачиваемых новосибирских файлов
        protected enum NovosibFileType
        {
            Obl, 
            Okato, 
            NNDDMM
        }

        #endregion Константы

        #region Инициализация

        public Form16PumpModule() : base()
		{

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbDataAccess != null) 
                    dbDataAccess.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion Инициализация

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref kdCache, dsKd.Tables[0], "CodeStr");
            FillRowsCache(ref locBdgtCache, dsLocBdgt.Tables[0], "Name");
            FillRowsCache(ref regionCache, dsRegions.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref regionForPumpCache, dsRegionForPump.Tables[0], "OKATO", "Name");
        }

        private void GetRegionsForPumpSourceID()
        {
            regForPumpSourceID = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            InitClsDataSet(ref daKd, ref dsKd, clsKd, false, string.Empty);
            InitClsDataSet(ref daLocBdgt, ref dsLocBdgt, clsLocBdgt, false, string.Empty);
            GetRegionsForPumpSourceID();
            InitDataSet(ref daRegionForPump, ref dsRegionForPump, clsRegionForPump, false, 
                string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);
            InitFactDataSet(ref daIncomesForm16, ref dsIncomesForm16, fctIncomesForm16);
            InitFactDataSet(ref daIncomesForm16Det, ref dsIncomesForm16Det, fctIncomesForm16Det);
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daLocBdgt, dsLocBdgt, clsLocBdgt);
            UpdateDataSet(daRegionForPump, dsRegionForPump, clsRegionForPump);
            UpdateDataSet(daIncomesForm16, dsIncomesForm16, fctIncomesForm16);
            UpdateDataSet(daIncomesForm16Det, dsIncomesForm16Det, fctIncomesForm16Det);
        }

        private const string D_REGIONS_FOR_PUMP_GUID = "e9d2898d-fc2d-4626-834a-ed1ac98a1673";
        private const string D_REGIONS_FORM16_GUID = "c8983298-5943-4275-8eac-7a614577a5ae";
        private const string D_KD_FORM16_GUID = "649d4b3b-3988-4065-94f3-f34d15780d26";
        private const string D_LOC_BDGT_FORM16_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string F_F_INCOMES_FORM16_GUID = "06b693e5-85a8-4b86-a9c1-e4bf5460b051";
        private const string F_F_INCOMES_FORM16_DET_GUID = "e9b7e515-1a99-487f-91f9-9f16422b20ba";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FORM16_GUID],
                clsKd = this.Scheme.Classifiers[D_KD_FORM16_GUID],
                clsLocBdgt = this.Scheme.Classifiers[D_LOC_BDGT_FORM16_GUID] };

            this.HierarchyClassifiers = (IClassifier[])CommonRoutines.ConcatArrays(
                this.UsedClassifiers, new IClassifier[] {
                    clsRegionForPump = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_GUID] } );

            this.UsedFacts = new IFactTable[] {
                fctIncomesForm16 = this.Scheme.FactTables[F_F_INCOMES_FORM16_GUID], 
                fctIncomesForm16Det = this.Scheme.FactTables[F_F_INCOMES_FORM16_DET_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsLocBdgt);
            ClearDataSet(ref dsRegionForPump);
            ClearDataSet(ref dsIncomesForm16);
            ClearDataSet(ref dsIncomesForm16Det);
            if (dbfDB != null)
                dbfDB.Close();
        }

        #endregion Работа с базой и кэшами

        #region Общие функции

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Replace(" ", string.Empty).Replace(".", ",").Trim();
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        private void AddFactRow(decimal fromBeginYear, decimal forCurrMonth, object[] mapping)
        {
            if (fromBeginYear == 0 && forCurrMonth == 0)
                return;
            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "FROMBEGINYEAR", fromBeginYear, 
                "FrmBgnYearReport", fromBeginYear, "FORCURRMONTH", forCurrMonth, "FrCrrMonthReport", forCurrMonth });
            if (isDetailData)
            {
                PumpRow(dsIncomesForm16Det.Tables[0], mapping);
                if (dsIncomesForm16Det.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesForm16Det, ref dsIncomesForm16Det);
                }
            }
            else
            {
                PumpRow(dsIncomesForm16.Tables[0], mapping);
                if (dsIncomesForm16.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesForm16, ref dsIncomesForm16);
                }
            }
        }

        private void PumpFactRow(DataRow row, object[] mapping)
        {
            decimal field1 = GetDecimalCellValue(row, "S_INCO", 0);
            decimal field2 = GetDecimalCellValue(row, "S_INCOM", 0);
            decimal field3 = GetDecimalCellValue(row, "S_RETU", 0);
            decimal field4 = GetDecimalCellValue(row, "S_RET5", 0);
            decimal field5 = GetDecimalCellValue(row, "S_RETUM", 0);
            decimal field6 = GetDecimalCellValue(row, "S_RET5M", 0);
            decimal field7 = GetDecimalCellValue(row, "S_ALL", 0);
            decimal field8 = GetDecimalCellValue(row, "S_ALLM", 0);
            decimal field9 = GetDecimalCellValue(row, "S_TOF", 0);
            decimal field10 = GetDecimalCellValue(row, "S_TOFM", 0);
            decimal field11 = GetDecimalCellValue(row, "S_TOM1", 0);
            decimal field12 = GetDecimalCellValue(row, "S_TOM1M", 0);
            decimal field13 = GetDecimalCellValue(row, "S_TOM", 0);
            decimal field14 = GetDecimalCellValue(row, "S_TOMM", 0);
            decimal field15 = GetDecimalCellValue(row, "S_P", 0);
            decimal field16 = GetDecimalCellValue(row, "S_PM", 0);
            decimal field17 = GetDecimalCellValue(row, "S_S", 0);
            decimal field18 = GetDecimalCellValue(row, "S_SM", 0);
            decimal field19 = GetDecimalCellValue(row, "S_M", 0);
            decimal field20 = GetDecimalCellValue(row, "S_MM", 0);
            decimal field21 = GetDecimalCellValue(row, "S_T", 0);
            decimal field22 = GetDecimalCellValue(row, "S_TM", 0);
            decimal field23 = GetDecimalCellValue(row, "S_L", 0);
            decimal field24 = GetDecimalCellValue(row, "S_LM", 0);
            decimal field25 = GetDecimalCellValue(row, "S_RESTE", 0);
            decimal field26 = GetDecimalCellValue(row, "_S_P", 0);
            decimal field27 = GetDecimalCellValue(row, "_S_PM", 0);

            AddFactRow(field1, field2,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 1 }));
            AddFactRow(field3 + field4, field5 + field6, 
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 2 }));
            AddFactRow(field3, field5, 
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 3 }));
            AddFactRow(field4, field6,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 4 }));
            AddFactRow(field7, field8,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5 }));
            AddFactRow(field9, field10, 
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 6 }));
            AddFactRow(field11, field12,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 7 }));
            AddFactRow(field13, field14,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 8 }));
            AddFactRow(field15 + field17 + field19 + field21 + field26,
                field16 + field18 + field20 + field22 + field27,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 9 }));
            AddFactRow(field15, field16,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 10 }));
            AddFactRow(field17, field18,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 11 }));
            AddFactRow(field19, field20,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 12 }));
            AddFactRow(field21, field22,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 13 }));
            AddFactRow(field23, field24,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 14 }));
            AddFactRow(field25, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 15 }));
        }

        private int GetReportDate(FileInfo file)
        {
            try
            {
                return Convert.ToInt32(string.Format(
                    "{0}{1}{2}",
                    this.DataSource.Year,
                    file.Name.Substring(5, file.Name.Length - file.Extension.Length - 7).PadLeft(2, '0'),
                    file.Name.Substring(file.Name.Length - file.Extension.Length - 2, 2)));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка при получении даты из имени файла ({0}). " +
                    "Возможно, название файла не соответствует формату", ex.Message), ex);
            }
        }

        private int GetRegionCode(string filename)
        {
            return Convert.ToInt32(CommonRoutines.TrimLetters(filename));
        }

        #endregion Общие функции

        #region Работа с Txt

        private void PumpTxtFiles(DirectoryInfo dir)
        {
            if (dir.GetFiles("F16*.TXT", SearchOption.AllDirectories).GetLength(0) +
                dir.GetFiles("G16*.TXT", SearchOption.AllDirectories).GetLength(0) == 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Файлы TXT не найдены.");
                return;
            }
            int skippedReports = 0;
            int processedReports = 0;
            int date = 0;
            string processedFiles = "<Нет данных>";

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных текстовых отчетов.");

            try
            {
                this.CallTXTSorcerer(xmlSettingsForm16, dir.FullName);

                int totalRecs = GetTotalRecs();
                int rowsCount = 0;

                // Закачиваем полученные данные
                // Первая таблица датасета - служебная, ее не берем
                for (int i = 1; i < this.ResultDataSet.Tables.Count; i++)
                {
                    DataTable dt = this.ResultDataSet.Tables[i];
                    if (dt.Rows.Count == 0) continue;

                    int fileIndex = Convert.ToInt32(dt.Rows[0][this.FileIndexFieldName]);
                    processedFiles = GetStringCellValue(this.ResultDataSet.Tables[0].Rows[fileIndex], "FILES", "<Нет данных>");
                    WriteToTrace(string.Format("Старт обработки файла(ов) {0}...", processedFiles), TraceMessageKind.Information);

                    // Дата справки
                    string str = this.FixedParameters[fileIndex]["ReportDate"].Value;

                    if (str != string.Empty)
                    {
                        date = Convert.ToInt32(str);
                    }
                    else
                    {
                        date = GetReportDate(this.RepFilesLists[0][fileIndex]);
                    }

                    if (date / 10000 != this.DataSource.Year)
                    {
                        skippedReports++;
                        continue;
                    }

                    // Удаляем ранее закачанные данные
                    DeleteData(string.Format("RefYearDayUNV = {0}", date), string.Format("Дата отчета: {0}.", date));

                    // Район
                    int regionID = this.PumpOriginalRow(dsRegions, clsRegions,
                        new object[] { "CODE", GetRegionCode(this.RepFilesLists[0][fileIndex].Name), "NAME", string.Format(
                            "{0} {1}", 
                            this.FixedParameters[fileIndex]["TaxOrgan"].Value,
                            this.FixedParameters[fileIndex]["Organization"].Value).Trim() });

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string kd = Convert.ToString(dt.Rows[i]["KD"]);
                        int kdRef = PumpCachedRow(kdCache, dsKd.Tables[0], clsKd, kd, new object[] { "CODESTR", kd });
                        object[] mapping = new object[] { "REFKD", kdRef, "RefYearDayUNV", date, "REFREGIONS", regionID };
                        PumpFactRow(dt.Rows[j], mapping);

                        rowsCount++;
                        this.SetProgress(totalRecs, rowsCount,
                            string.Format("Источник {0}. Обработка данных...", dir.FullName),
                            string.Format("Строка {0} из {1}", rowsCount, totalRecs));
                    }

                    processedReports++;

                    WriteToTrace(string.Format("Обработка файла(ов) {0} завершена.", processedFiles), TraceMessageKind.Information);
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                        "Закачка данных текстовых отчетов закончена. Обработано отчетов: {0}, " +
                        "из них пропущено из-за несоответствия даты источнику: {1} отчетов .",
                        processedReports, skippedReports));
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                        "Закачка данных текстовых отчетов закончена с ошибками: {0}. \n" +
                        "На момент возникновения ошибки достигнуты следующие результаты. " +
                        "Обработано отчетов: {1}, " +
                        "из них пропущено из-за несоответствия даты источнику: {2} отчетов . " +
                        "Ошибка возникла при обрабатке файлов {3}.",
                        ex.Message, processedReports, skippedReports, processedFiles));
                throw;
            }
        }

        #endregion Работа с Txt

        #region Работа с Dbf

        private void PumpDbfFiles(DirectoryInfo dir)
        {
            FileInfo[] dbfFiles = dir.GetFiles("F16*.dbf", SearchOption.AllDirectories);
            if (dbfFiles.GetLength(0) == 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Файлы DBF не найдены.");
                return;
            }

            IDbDataAdapter da = null;
            DataSet ds = null;
            IDbDataAdapter daFStat = null;
            DataSet dsFStat = null;
            int skippedReports = 0;
            int processedReports = 0;
            int date = 0;


            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов DBF.");

            try
            {
                if (dbfFiles.GetLength(0) > 0)
                {
                    // Подключаемся к источнику
                    dbDataAccess.ConnectToDataSource(ref dbfDB, dir.FullName, ODBCDriverName.Microsoft_dBase_Driver);
                }
                else
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        "Закачка данных отчетов DBF закончена: отсутствуют данные для закачки.");
                    return;
                }

                // Дата отчета
                if (dir.GetFiles("F_STAT.DBF", SearchOption.AllDirectories).GetLength(0) == 0)
                {
                    throw new Exception("Не найден файл F_STAT.DBF.");
                }
                InitDataSet(dbfDB, ref daFStat, ref dsFStat, "SELECT * FROM F_STAT.DBF");

                for (int i = 0; i < dbfFiles.GetLength(0); i++)
                {
                    WriteToTrace(string.Format("Старт обработки файла {0}...", dbfFiles[i].Name), TraceMessageKind.Information);
                    this.SetProgress(dbfFiles.GetLength(0), i + 1,
                        string.Format("Файл {0}. Обработка данных...", dbfFiles[i].Name),
                        string.Format("{0} из {1}", i + 1, dbfFiles.GetLength(0)));

                    if (!dbfFiles[i].Exists) continue;

                    // Проверка даты
                    DataRow[] rows = dsFStat.Tables[0].Select(string.Format(
                        "N_FORM = '{0}'",
                        dbfFiles[i].Name.Remove(dbfFiles[i].Name.Length - dbfFiles[i].Extension.Length)));
                    if (rows.GetLength(0) > 0)
                    {
                        date = CommonRoutines.ShortDateToNewDate(Convert.ToDateTime(rows[0]["MDATE"]).ToShortDateString());
                    }
                    else
                    {
                        date = GetReportDate(dbfFiles[i]);
                    }

                    if (date > 0 && date / 10000 != this.DataSource.Year)
                    {
                        skippedReports++;
                        continue;
                    }

                    // Удаляем ранее закачанные данные
                    DeleteData(string.Format("RefYearDayUNV = {0}", date), string.Format("Дата отчета: {0}.", date));

                    InitDataSet(dbfDB, ref da, ref ds, string.Format("SELECT * FROM {0}", dbfFiles[i].Name));
                    DataTable dt = ds.Tables[0];

                    // Район
                    int regionID = this.PumpOriginalRow(dsRegions, clsRegions,
                        new object[] { "CODE", GetRegionCode(dbfFiles[i].Name), "NAME", constDefaultClsName });

                    int totalRecs = dt.Rows.Count;

                    for (int j = 0; j < totalRecs; j++)
                    {
                        string kd = Convert.ToString(dt.Rows[j]["AP"]) + Convert.ToString(dt.Rows[j]["C_PRIV"]);
                        int kdRef = PumpCachedRow(kdCache, dsKd.Tables[0], clsKd, kd, new object[] { "CODESTR", kd });
                        object[] mapping = new object[] { "REFKD", kdRef, "RefYearDayUNV", date, "REFREGIONS", regionID };
                        PumpFactRow(dt.Rows[j], mapping);
                    }

                    processedReports++;

                    WriteToTrace(string.Format("Обработка файла {0} завершена.", dbfFiles[i].Name), TraceMessageKind.Information);
                }

                if (skippedReports == dbfFiles.GetLength(0))
                    throw new Exception("из-за несоответствия дат не закачан ни один отчет");

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                        "Закачка данных отчетов DBF закончена. Обработано отчетов: {0}, " +
                        "из них пропущено из-за несоответствия даты источнику: {1} отчетов.",
                        processedReports, skippedReports));
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                        "Закачка данных отчетов DBF закончена с ошибками: {0}. \n" +
                        "На момент возникновения ошибки достигнуты следующие результаты. " +
                        "Обработано отчетов: {1}, " +
                        "из них пропущено из-за несоответствия даты источнику: {2} отчетов.",
                        ex.Message, processedReports, skippedReports));
                throw;
            }
        }

        #endregion Работа с Dbf

        #region Работа с Xls

        private int GetXlsReportDate(ExcelHelper excelDoc)
        {
            int refDate = -1;
            switch (this.Region)
            {
                case RegionName.Novosibirsk:
                    refDate = CommonRoutines.ShortDateToNewDate(excelDoc.GetValue("X4").Trim());
                    // если нет, то в ячейке Z4 - очередная песда постановки
                    if (refDate == -1)
                        refDate = CommonRoutines.ShortDateToNewDate(excelDoc.GetValue("Z4").Trim());
                    // заключительные обороты
                    if (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbFinalOverturn", "False")))
                        refDate = this.DataSource.Year * 10000 + 12 * 100 + 32;
                    break;
                case RegionName.Tambov:
                    refDate = CommonRoutines.ShortDateToNewDate(excelDoc.GetValue("Z4").Trim());
                    break;
                default:
                    if ((this.Region == RegionName.Karelya) && (this.DataSource.Year >= 2011))
                    {
                        // дата хранится в виде "01.08.2011 0:00:00", время нужно отбросить
                        string cellText = excelDoc.GetValue("AN7").Trim().Split(new char[] { ' ' })[0];
                        refDate = CommonRoutines.ShortDateToNewDate(cellText);
                    }
                    else
                    {
                        string cellText = string.Empty;
                        if ((this.Region == RegionName.Karelya) && (this.DataSource.Year >= 2009))
                            cellText = excelDoc.GetValue("D4").Trim();
                        else
                            cellText = excelDoc.GetValue("A4").Trim();
                        refDate = Convert.ToInt32(CommonRoutines.LongDateToNewDate(cellText));
                    }
                    refDate = CommonRoutines.DecrementDateWithLastDay(refDate);
                    break;
            }
            if ((refDate != -1) && !deletedDateList.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }
            return refDate;
        }

        private int GetRegionCodeBySheetName(string sheetName)
        {
            return Convert.ToInt32(CommonRoutines.TrimLetters(sheetName));
        }

        private int GetNovosibRegionCode(string fileName)
        {
            int code = -1;
            switch (novosibFileType)
            {
                case NovosibFileType.NNDDMM:
                    int archCode = Convert.ToInt32(archName.Split('_')[0]);
                    code = Convert.ToInt32(fileName.Substring(0, 2));
                    if (code != archCode)
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                            String.Format("Код района архива {0} (имя архива: {1}) не совпадает с кодом файла {2} (имя файла: {3}).",
                                archCode, archName, code, fileName));
                    break;
                case NovosibFileType.Obl:
                    code = Convert.ToInt32(fileName.Substring(0, 2));
                    break;
                case NovosibFileType.Okato:
                    code = Convert.ToInt32(CommonRoutines.TrimLetters(fileName));
                    break;
            }
            return code;
        }

        private int GetXlsRefRegion(string filename, ExcelHelper excelDoc)
        {
            int code = -1;
            string name = string.Empty;
            object[] mapping = null;
            switch (this.Region)
            {
                case RegionName.Novosibirsk:
                    code = GetNovosibRegionCode(filename);
                    if ((!pumpOnlyRegion) && (!pumpedRegions.Contains(code)))
                        pumpedRegions.Add(code);
                    name = FindCachedRow(regionForPumpCache, code.ToString(), string.Empty);
                    if (name == string.Empty)
                    {
                        // имя в ячейке D5
                        name = excelDoc.GetValue(5, 4).Trim();
                        if (novosibFileType != NovosibFileType.Okato)
                            name += string.Format("({0})", code);
                    }
                    int parentId = -1;
                    // закачиваем в служебные районы
                    if (!regionForPumpCache.ContainsKey(code.ToString()))
                    {
                        if (novosibFileType != NovosibFileType.Obl)
                        {
                            // ищем родительскую запись - NN из имени архива
                            DataRow[] region = dsRegionForPump.Tables[0].Select(string.Format("OKATO = {0}", archName.Substring(0, 2)));
                            if (region.GetLength(0) != 0)
                            {
                                parentId = Convert.ToInt32(region[0]["Id"]);
                                if (parentId != -1)
                                    mapping = new object[] { "ParentId", parentId };
                            }
                        }
                        mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "OKATO", code, "Name", name, "SourceId", regForPumpSourceID });
                        PumpRow(clsRegionForPump, dsRegionForPump.Tables[0], mapping, true);
                        regionForPumpCache.Add(code.ToString(), name);
                        if (!regionForPumpCodes.Contains(code.ToString()))
                            regionForPumpCodes.Add(code.ToString());
                    }
                    parentId = -1;
                    mapping = null;
                    if (novosibFileType != NovosibFileType.Obl)
                    {
                        // ищем родительскую запись - NN из имени архива
                        DataRow[] region = dsRegions.Tables[0].Select(string.Format("Code = {0}", archName.Substring(0, 2)));
                        if (region.GetLength(0) != 0)
                        {
                            parentId = Convert.ToInt32(region[0]["Id"]);
                            if (parentId != -1)
                                mapping = new object[] { "ParentId", parentId };
                        }
                    }
                    break;
                case RegionName.Tambov:
                    code = 0;
                    break;
                default:
                    if ((isNewKarelyaXlsFormat) && (this.DataSource.Year < 2008))
                        // код района получаем из наименования листа
                        code = GetRegionCodeBySheetName(excelDoc.GetWorksheetName());
                    else
                        // код района получаем из имени файла
                        code = GetRegionCode(filename);
                    break;
            }
            if (name == string.Empty)
                // имя в ячейке D5
                name = excelDoc.GetValue(5, 4).Trim();
            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "Code", code, "Name", name });
            string regionKey = string.Format("{0}|{1}", code, name);
            // новосибирск закачка из архива NN_DDMM.arj если уже есть район с таким кодом - добавлять не нужно
            if ((this.Region == RegionName.Novosibirsk) && (novosibFileType == NovosibFileType.NNDDMM))
            {
                DataRow[] regionRow = dsRegions.Tables[0].Select(string.Format("Code = {0}", code));
                if (regionRow.GetLength(0) != 0)
                    return Convert.ToInt32(regionRow[0]["Id"]);
            }
            // в карелии с 2008 года если уже есть район с таким наименованием, но с другим кодом - добавлять не нужно
            if ((this.Region == RegionName.Tambov) ||
                ((this.Region == RegionName.Karelya) && (this.DataSource.Year >= 2008)))
            {
                DataRow[] regionRow = dsRegions.Tables[0].Select(string.Format("Name = '{0}'", name));
                if (regionRow.GetLength(0) != 0)
                    return Convert.ToInt32(regionRow[0]["Id"]);
            }
            return PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, mapping, regionKey, "ID");
        }

        private void CheckRowControlSum(decimal sum1, decimal sum2, int curRow, string sheetName, string fieldName)
        {
            if (sum1 != sum2)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма ('{0}') на листе {1} по строке {2} ({3}) не совпадает ({4})",
                    fieldName, sheetName, curRow, sum1, sum2));
            }
        }

        private void CheckControlSum(Dictionary<string, string> dataRow, string sheetName)
        {
            int i = 0;
            foreach (string value in dataRow.Values)
            {
                decimal totalSum = CleanFactValue(value);
                if (controlSums[i] != totalSum)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Сумма строк на листе {0} в столбце {1} ({2}) не совпадает с контрольной ({3})",
                        sheetName, i + 2, controlSums[i], totalSum));
                }
                i++;
            }
        }

        private object[] GetRefMapping(int kdRef, int dateRef, int regionRef)
        {
            object[] mapping = new object[] { "REFKD", kdRef, "RefYearDayUNV", dateRef, "REFREGIONS", regionRef };
            if ((isNewKarelyaXlsFormat) || (this.Region == RegionName.Tambov))
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "Ghost", 4 });
            return mapping;
        }

        private void SetXlsFlags(ExcelHelper excelDoc, int firstRow)
        {
            isZDateCell = false;
            isNewKarelyaXlsFormat = false;
            if (this.Region == RegionName.Karelya)
            {
                isNewKarelyaXlsFormat = (this.DataSource.Year >= 2007) &&
                    (excelDoc.GetValue(firstRow - 1, 21).Trim() != string.Empty);
            }
            else if (this.Region == RegionName.Novosibirsk)
            {
                int tempDateRef = CommonRoutines.ShortDateToNewDate(excelDoc.GetValue(4, 23).Trim());
                isZDateCell = (tempDateRef == -1);
            }
            else if (this.Region == RegionName.Tambov)
            {
                isZDateCell = true;
            }
        }

        private int GetXlsFirstRow(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                if (excelDoc.GetValue(curRow, 1).Trim() == "1")
                    return (curRow + 1);
            }
            return 1;
        }

        private bool IsSkipRow(string cellValue)
        {
            return (cellValue.Trim().ToUpper() == TABLE_TOTAL_BK);
        }

        private const string TABLE_END_TEXT = "ИТОГО";
        private const string TABLE_END_MARK = "ВОЗВРАЩЕНО ИЗ БЮДЖЕТА";
        private const string TABLE_TOTAL_BK = "ИТОГО ПО КОДУ БК";
        private bool IsLastRow(string cellValue)
        {
            cellValue = cellValue.Trim().ToUpper();
            return (cellValue == string.Empty) || cellValue.StartsWith(TABLE_END_TEXT) ||
                cellValue.StartsWith(TABLE_END_MARK);
        }

        #region Массивы пар "поле-столбец"

        private object[] XLS_MAPPING_2006 = new object[] {
            "KD", 1, "S_INCO", 2, "S_INCOM", 3, "S_RETU", 4, "S_RET5", 5, "S_RETUM", 6, "S_RET5M", 7,
            "S_ALL", 8, "S_ALLM", 9, "S_TOF", 10, "S_TOFM", 11, "S_TOM1", 12, "S_TOM1M", 13, "S_TOM", 14,
            "S_TOMM", 15, "_S_P", 16, "_S_PM", 17, "S_L", 18, "S_LM", 19, "S_RESTE", 20 };
        private object[] XLS_MAPPING_2007 = new object[] {
            "KD", 1, "B", 2, "C", 3, "D", 4, "E", 5, "F", 6, "G", 7, "H", 8, "I", 9, "J", 10,
            "K", 11, "L", 12, "M", 13, "N", 14, "O", 15, "P", 16, "Q", 17, "R", 18, "S", 19,
            "T", 20, "U", 21, "V", 22, "W", 23, "X", 24 };
        private object[] XLS_MAPPING_2009_SHEET_1 = new object[] {
            "KD", 1, "C", 3, "D", 4, "E", 5, "F", 6, "G", 7, "H", 8, "I", 9, "J", 10,
            "K", 11, "L", 12, "M", 13, "N", 14 };
        private object[] XLS_MAPPING_2009_SHEET_2 = new object[] {
            "KD2", 1, "C2", 3, "D2", 4, "E2", 5, "F2", 6, "G2", 7,
            "H2", 8, "I2", 9, "J2", 10, "K2", 11, "L2", 12, "M2", 13 };
        private object[] XLS_MAPPING_2011 = new object[] {
            "KD", 1, "OKATO", 9, "M", 13, "R", 18, "U", 21, "AC", 29, "AI", 35 };

        private object[] GetXlsMapping()
        {
            if ((isNewKarelyaXlsFormat) || (this.Region == RegionName.Novosibirsk) || (this.Region == RegionName.Tambov))
            {
                object[] xlsMapping = XLS_MAPPING_2007;
                if ((isZDateCell) || ((this.Region == RegionName.Karelya) && (this.DataSource.Year >= 2008)))
                    xlsMapping = (object[])CommonRoutines.ConcatArrays(xlsMapping, new object[] { "Y", 25, "Z", 26 });
                return xlsMapping;
            }
            return XLS_MAPPING_2006;
        }

        private Dictionary<string, string> GetXlsDataRow(ExcelHelper excelDoc, int curRow, object[] mapping)
        {
            Dictionary<string, string> dataRow = new Dictionary<string, string>();
            int count = mapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                dataRow.Add(mapping[i].ToString(), excelDoc.GetValue(curRow, Convert.ToInt32(mapping[i + 1])));
            }
            return dataRow;
        }

        private Dictionary<string, string> GetXlsDataRow2Sheets(ExcelHelper excelDoc, int[] curRows)
        {
            Dictionary<string, string> dataRow = new Dictionary<string, string>();
            excelDoc.SetWorksheet(1);
            int count = XLS_MAPPING_2009_SHEET_1.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                dataRow.Add(XLS_MAPPING_2009_SHEET_1[i].ToString(), excelDoc.GetValue(curRows[0],
                    Convert.ToInt32(XLS_MAPPING_2009_SHEET_1[i + 1])));
            }
            excelDoc.SetWorksheet(2);
            count = XLS_MAPPING_2009_SHEET_2.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                dataRow.Add(XLS_MAPPING_2009_SHEET_2[i].ToString(), excelDoc.GetValue(curRows[1],
                    Convert.ToInt32(XLS_MAPPING_2009_SHEET_2[i + 1])));
            }
            return dataRow;
        }

        #endregion Массивы пар "поле-столбец"

        #region 2006

        private void PumpXlsFactRow2006(Dictionary<string, string> row, object[] mapping)
        {
            decimal field1 = CleanFactValue(row["S_INCO"]);
            decimal field2 = CleanFactValue(row["S_INCOM"]);
            decimal field3 = CleanFactValue(row["S_RETU"]);
            decimal field4 = CleanFactValue(row["S_RET5"]);
            decimal field5 = CleanFactValue(row["S_RETUM"]);
            decimal field6 = CleanFactValue(row["S_RET5M"]);
            decimal field7 = CleanFactValue(row["S_ALL"]);
            decimal field8 = CleanFactValue(row["S_ALLM"]);
            decimal field9 = CleanFactValue(row["S_TOF"]);
            decimal field10 = CleanFactValue(row["S_TOFM"]);
            decimal field11 = CleanFactValue(row["S_TOM1"]);
            decimal field12 = CleanFactValue(row["S_TOM1M"]);
            decimal field13 = CleanFactValue(row["S_TOM"]);
            decimal field14 = CleanFactValue(row["S_TOMM"]);
            decimal field15 = CleanFactValue(row["S_P"]);
            decimal field16 = CleanFactValue(row["S_PM"]);
            decimal field17 = CleanFactValue(row["S_S"]);
            decimal field18 = CleanFactValue(row["S_SM"]);
            decimal field19 = CleanFactValue(row["S_M"]);
            decimal field20 = CleanFactValue(row["S_MM"]);
            decimal field21 = CleanFactValue(row["S_T"]);
            decimal field22 = CleanFactValue(row["S_TM"]);
            decimal field23 = CleanFactValue(row["S_L"]);
            decimal field24 = CleanFactValue(row["S_LM"]);
            decimal field25 = CleanFactValue(row["S_RESTE"]);
            decimal field26 = CleanFactValue(row["_S_P"]);
            decimal field27 = CleanFactValue(row["_S_PM"]);

            AddFactRow(field1, field2,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 1 }));
            AddFactRow(field3 + field4, field5 + field6,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 2 }));
            AddFactRow(field3, field5,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 3 }));
            AddFactRow(field4, field6,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 4 }));
            AddFactRow(field7, field8,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5 }));
            AddFactRow(field9, field10,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 6 }));
            AddFactRow(field11, field12,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 7 }));
            AddFactRow(field13, field14,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 8 }));
            AddFactRow(field15 + field17 + field19 + field21 + field26,
                field16 + field18 + field20 + field22 + field27,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 9 }));
            AddFactRow(field15, field16,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 10 }));
            AddFactRow(field17, field18,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 11 }));
            AddFactRow(field19, field20,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 12 }));
            AddFactRow(field21, field22,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 13 }));
            AddFactRow(field23, field24,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 14 }));
            AddFactRow(field25, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 15 }));
        }

        #endregion 2006

        #region 2007

        private int GetNOBudgetLevel(string filename)
        {
            switch (this.Region)
            {
                case RegionName.Novosibirsk:
                    if (novosibFileType == NovosibFileType.Obl)
                        return 14;
                    if ((filename.ToUpper().StartsWith("MP")) || (filename[2].ToString().ToUpper() == "M"))
                        return 6;
                    else if ((filename.ToUpper().StartsWith("MR")) || (filename[2].ToString().ToUpper() == "R"))
                        return 5;
                    return 14;
                case RegionName.Tambov:
                    return 14;
                default:
                    if (this.DataSource.Year < 2008)
                        return 15;
                    return 14;
            }
        }

        private int GetPQBudgetLevel()
        {
            switch (this.Region)
            {
                case RegionName.Novosibirsk:
                case RegionName.Tambov:
                    return 8;
                default:
                    if (this.DataSource.Year < 2008)
                        return 5;
                    return 8;
            }
        }

        private int GetRSBudgetLevel()
        {
            switch (this.Region)
            {
                case RegionName.Novosibirsk:
                case RegionName.Tambov:
                    return 9;
                default:
                    if (this.DataSource.Year < 2008)
                        return 6;
                    return 9;
            }
        }

        private int GetTUBudgetLevel()
        {
            switch (this.Region)
            {
                case RegionName.Novosibirsk:
                case RegionName.Tambov:
                    return 10;
                default:
                    if (this.DataSource.Year < 2008)
                        return 7;
                    return 10;
            }
        }

        private int GetVWBudgetLevel()
        {
            switch (this.Region)
            {
                case RegionName.Novosibirsk:
                case RegionName.Tambov:
                    return 11;
                default:
                    if (this.DataSource.Year < 2008)
                        return 12;
                    return 11;
            }
        }

        private void PumpXlsFactRow2007(Dictionary<string, string> row, object[] mapping, int rowIndex,
            string sheetName, string filename)
        {
            decimal sumB = CleanFactValue(row["B"]);
            decimal sumC = CleanFactValue(row["C"]);
            decimal sumD = CleanFactValue(row["D"]);
            decimal sumE = CleanFactValue(row["E"]);
            decimal sumF = CleanFactValue(row["F"]);
            decimal sumG = CleanFactValue(row["G"]);
            decimal sumH = CleanFactValue(row["H"]);
            decimal sumI = CleanFactValue(row["I"]);
            decimal sumJ = CleanFactValue(row["J"]);
            decimal sumK = CleanFactValue(row["K"]);
            decimal sumL = CleanFactValue(row["L"]);
            decimal sumM = CleanFactValue(row["M"]);
            decimal sumN = CleanFactValue(row["N"]);
            decimal sumO = CleanFactValue(row["O"]);
            decimal sumP = CleanFactValue(row["P"]);
            decimal sumQ = CleanFactValue(row["Q"]);
            decimal sumR = CleanFactValue(row["R"]);
            decimal sumS = CleanFactValue(row["S"]);
            decimal sumT = CleanFactValue(row["T"]);
            decimal sumU = CleanFactValue(row["U"]);
            decimal sumV = CleanFactValue(row["V"]);
            decimal sumW = CleanFactValue(row["W"]);
            decimal sumX = CleanFactValue(row["X"]);
            decimal sumY = 0;
            decimal sumZ = 0;
            if ((isZDateCell) || (isNewKarelyaXlsFormat))
            {
                sumY = CleanFactValue(row["Y"]);
                sumZ = CleanFactValue(row["Z"]);
            }

            if ((isNewKarelyaXlsFormat) || (this.Region == RegionName.Tambov))
            {
                CheckRowControlSum(sumH, sumJ + sumL + sumN + sumP + sumR + sumT + sumV + sumX,
                    rowIndex, sheetName, "С начала года");
                CheckRowControlSum(sumI, sumK + sumM + sumO + sumQ + sumS + sumU + sumW + sumY, 
                    rowIndex, sheetName, "За текущий месяц");

                int i = 0;
                foreach (string value in row.Values)
                {
                    controlSums[i] = CleanFactValue(value);
                    i++;
                }
            }

            if (!pumpOnlyNOColumns)
            {
                AddFactRow(sumB, sumC,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 1, "RefBdgtLevels", 0, "Ghost", 4 }));
                AddFactRow(sumD, sumF,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 3, "RefBdgtLevels", 0, "Ghost", 4 }));
                AddFactRow(sumE, sumG,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 4, "RefBdgtLevels", 0, "Ghost", 4 }));
                AddFactRow(sumJ, sumK,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 1, "Ghost", 4 }));
                AddFactRow(sumL, sumM,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 3, "Ghost", 4 }));

                if (this.Region != RegionName.Novosibirsk)
                {
                    AddFactRow(sumN, sumO,
                        (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", GetNOBudgetLevel(filename), "Ghost", 4 }));
                }

                AddFactRow(sumP, sumQ,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", GetPQBudgetLevel(), "Ghost", 4 }));
                AddFactRow(sumR, sumS,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", GetRSBudgetLevel(), "Ghost", 4 }));
                AddFactRow(sumT, sumU,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", GetTUBudgetLevel(), "Ghost", 4 }));
                AddFactRow(sumV, sumW,
                    (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", GetVWBudgetLevel(), "Ghost", 4 }));

                if ((isZDateCell) || (isNewKarelyaXlsFormat))
                {
                    AddFactRow(sumX, sumY,
                        (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 12, "Ghost", 4 }));
                    AddFactRow(sumZ, 0,
                        (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 15, "RefBdgtLevels", 0, "Ghost", 4 }));
                }
                else
                {
                    AddFactRow(sumX, 0,
                        (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 15, "RefBdgtLevels", 0, "Ghost", 4 }));
                }
            }
            else
            {
                // признак 1 - только при закачке файлов NN_DDMM.xls из архива *obl* - иначе 4
                if (novosibFileType == NovosibFileType.Obl)
                {
                    AddFactRow(sumN, sumO,
                        (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", GetNOBudgetLevel(filename), "Ghost", 1 }));
                }
                else
                {
                    AddFactRow(sumN, sumO,
                        (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", GetNOBudgetLevel(filename), "Ghost", 4 }));
                }
            }
        }

        #endregion 2007

        #region Карелия-2009

        private void PumpXlsFactRow2009(Dictionary<string, string> row, object[] mapping, int curRow, string sheetName)
        {
            decimal sumC = CleanFactValue(row["C"]);
            decimal sumD = CleanFactValue(row["D"]);
            decimal sumE = CleanFactValue(row["E"]);
            decimal sumF = CleanFactValue(row["F"]);
            decimal sumG = CleanFactValue(row["G"]);
            decimal sumH = CleanFactValue(row["H"]);
            decimal sumI = CleanFactValue(row["I"]);
            decimal sumJ = CleanFactValue(row["J"]);
            decimal sumK = CleanFactValue(row["K"]);
            decimal sumL = CleanFactValue(row["L"]);
            decimal sumM = CleanFactValue(row["M"]);
            decimal sumN = CleanFactValue(row["N"]);

            decimal sumC2 = CleanFactValue(row["C2"]);
            decimal sumD2 = CleanFactValue(row["D2"]);
            decimal sumE2 = CleanFactValue(row["E2"]);
            decimal sumF2 = CleanFactValue(row["F2"]);
            decimal sumG2 = CleanFactValue(row["G2"]);
            decimal sumH2 = CleanFactValue(row["H2"]);
            decimal sumI2 = CleanFactValue(row["I2"]);
            decimal sumJ2 = CleanFactValue(row["J2"]);
            decimal sumK2 = CleanFactValue(row["K2"]);
            decimal sumL2 = CleanFactValue(row["L2"]);
            decimal sumM2 = CleanFactValue(row["M2"]);

            CheckRowControlSum(sumG, sumI + sumK + sumM + sumC2 + sumE2 + sumG2 + sumI2 + sumK2,
                curRow, sheetName, "С начала года");
            CheckRowControlSum(sumH, sumJ + sumL + sumN + sumD2 + sumF2 + sumH2 + sumJ2 + sumL2,
                curRow, sheetName, "За текущий месяц");

            AddFactRow(sumC, sumD,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 1, "RefBdgtLevels", 0, "Ghost", 4 }));
            AddFactRow(sumE, sumF,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 2, "RefBdgtLevels", 0, "Ghost", 4 }));
            AddFactRow(sumI, sumJ,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 1, "Ghost", 4 }));
            AddFactRow(sumK, sumL,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 3, "Ghost", 4 }));
            AddFactRow(sumM, sumN,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 14, "Ghost", 4 }));
            AddFactRow(sumC2, sumD2,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 8, "Ghost", 4 }));
            AddFactRow(sumE2, sumF2,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 9, "Ghost", 4 }));
            AddFactRow(sumG2, sumH2,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 10, "Ghost", 4 }));
            AddFactRow(sumI2, sumJ2,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 11, "Ghost", 4 }));
            AddFactRow(sumK2, sumL2,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 12, "Ghost", 4 }));
            AddFactRow(sumM2, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 15, "RefBdgtLevels", 0, "Ghost", 4 }));
        }

        private void PumpXlsSheetsData(string filename, ExcelHelper excelDoc, int sheetsCount)
        {
            int[] curRows = new int[sheetsCount];
            for (int i = 0; i < sheetsCount; i++)
            {
                excelDoc.SetWorksheet(i + 1);
                curRows[i] = GetXlsFirstRow(excelDoc);
            }

            excelDoc.SetWorksheet(1);
            SetXlsFlags(excelDoc, curRows[0]);

            int refDate = GetXlsReportDate(excelDoc);
            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Не удалось получить дату с листа {0} отчета {1}, данные отчета закачаны не будут.",
                    excelDoc.GetWorksheetName(), filename));
                return;
            }

            int refRegion = GetXlsRefRegion(filename, excelDoc);
            if (pumpOnlyRegion)
                return;

            int rowsCount = excelDoc.GetRowsCount();
            string sheetName = excelDoc.GetWorksheetName();
            for (; curRows[0] <= rowsCount; curRows[0]++)
                try
                {
                    SetProgress(rowsCount, curRows[0],
                        string.Format("Обработка файла {0}...", filename),
                        string.Format("Строка {0} из {1}", curRows[0], rowsCount));

                    Dictionary<string, string> dataRow = GetXlsDataRow2Sheets(excelDoc, curRows);

                    string kdCode = dataRow["KD"].Trim();
                    if (IsLastRow(kdCode))
                        return;

                    int refKd = PumpCachedRow(kdCache, dsKd.Tables[0], clsKd, kdCode, new object[] { "CODESTR", kdCode });
                    object[] mapping = GetRefMapping(refKd, refDate, refRegion);
                    PumpXlsFactRow2009(dataRow, mapping, curRows[0], sheetName);

                    curRows[1]++;
                }
                catch (Exception ex)
                {
                    excelDoc.SetWorksheet(1);
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                        "При обработке строки {0} листа {1} отчета {2} возникла ошибка ({3})",
                        curRows[0], excelDoc.GetWorksheetName(), filename, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #endregion Карелия-2009

        #region Карелия-2011

        private int PumpXlsKd(string code)
        {
            code = CommonRoutines.TrimLetters(code.Trim());
            return PumpCachedRow(kdCache, dsKd.Tables[0], clsKd, code, new object[] { "CODESTR", code });
        }

        private int PumpXlsLocBdgt(string name)
        {
            object[] mapping = new object[] { "Account", "Неуказанный счет", "Name", name, "ОКАТО", 0 };
            return PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, mapping, name, "ID");
        }

        private int PumpXlsRegion(string code)
        {
            code = CommonRoutines.TrimLetters(code.Trim());
            string name = constDefaultClsName;
            if (code == string.Empty)
            {
                code = "0";
                name = "Нераспределенные доходы";
            }
            object[] mapping = new object[] { "Code", code, "Name", name };
            string regionKey = string.Format("{0}|{1}", code, name);
            return PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, mapping, regionKey, "ID");
        }

        private void PumpXlsFactRow2011(Dictionary<string, string> row, object[] mapping)
        {
            decimal sumM = CleanFactValue(row["M"]);
            decimal sumR = CleanFactValue(row["R"]);
            decimal sumU = CleanFactValue(row["U"]);
            decimal sumAC = CleanFactValue(row["AC"]);
            decimal sumAI = CleanFactValue(row["AI"]);

            AddFactRow(sumM, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 1, "RefBdgtLevels", 0, "Ghost", 4 }));
            AddFactRow(sumR, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 2, "RefBdgtLevels", 0, "Ghost", 4 }));
            AddFactRow(sumU, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 5, "RefBdgtLevels", 0, "Ghost", 4 }));
            AddFactRow(sumAC, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 15, "RefBdgtLevels", 0, "Ghost", 4 }));
            AddFactRow(sumAI, 0,
                (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "REFDATAMARKSFORM16", 16, "RefBdgtLevels", 0, "Ghost", 4 }));
        }

        private const string BUDGET_NAME = "НАИМЕНОВАНИЕ БЮДЖЕТА";
        private void PumpXlsSheetData2011(string filename, ExcelHelper excelDoc)
        {
            int refDate = GetXlsReportDate(excelDoc);
            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Не удалось получить дату с листа {0} отчета {1}, данные отчета закачаны не будут.",
                    excelDoc.GetWorksheetName(), filename));
                return;
            }

            bool toPump = false;
            int refLocBdgt = -1;
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", filename),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    Dictionary<string, string> dataRow = GetXlsDataRow(excelDoc, curRow, XLS_MAPPING_2011);

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();

                    if (cellValue.ToUpper() == BUDGET_NAME)
                    {
                        refLocBdgt = PumpXlsLocBdgt(excelDoc.GetValue(curRow, 11).Trim());
                        continue;
                    }

                    if (IsSkipRow(cellValue))
                        continue;

                    if (IsLastRow(cellValue))
                    {
                        toPump = false;
                    }

                    if (toPump)
                    {
                        int refKd = PumpXlsKd(dataRow["KD"].Trim());
                        int refRegion = PumpXlsRegion(dataRow["OKATO"].Trim());
                        object[] mapping = new object[] { "REFKD", refKd, "RefYearDayUNV", refDate,
                            "REFREGIONS", refRegion, "REFLOCALBDGT", refLocBdgt };
                        PumpXlsFactRow2011(dataRow, mapping);
                    }

                    if (cellValue == "1")
                    {
                        toPump = true;
                    }
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                        "При обработке строки {0} листа {1} отчета {2} возникла ошибка ({3})",
                        curRow, excelDoc.GetWorksheetName(), filename, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #endregion Карелия-2011

        private void PumpXlsSheetData(string filename, ExcelHelper excelDoc)
        {
            int firstRow = GetXlsFirstRow(excelDoc);
            SetXlsFlags(excelDoc, firstRow);

            int refDate = GetXlsReportDate(excelDoc);
            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Не удалось получить дату с листа {0} отчета {1}, данные отчета закачаны не будут.",
                    excelDoc.GetWorksheetName(), filename));
                return;
            }

            int refRegion = GetXlsRefRegion(filename, excelDoc);
            if (pumpOnlyRegion)
                return;

            object[] xlsMapping = GetXlsMapping();
            int rowsCount = excelDoc.GetRowsCount();
            string sheetName = excelDoc.GetWorksheetName();
            for (int curRow = firstRow; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", filename),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    Dictionary<string, string> dataRow = GetXlsDataRow(excelDoc, curRow, xlsMapping);

                    string kdCode = dataRow["KD"].Trim();
                    if (kdCode.ToUpper().StartsWith(TABLE_END_TEXT) && isNewKarelyaXlsFormat)
                    {
                        CheckControlSum(dataRow, sheetName);
                        continue;
                    }

                    if (IsLastRow(kdCode))
                        return;

                    int refKd = PumpCachedRow(kdCache, dsKd.Tables[0], clsKd, kdCode, new object[] { "CODESTR", kdCode });
                    object[] mapping = GetRefMapping(refKd, refDate, refRegion);
                    if ((isNewKarelyaXlsFormat) || (this.Region == RegionName.Novosibirsk) || (this.Region == RegionName.Tambov))
                        PumpXlsFactRow2007(dataRow, mapping, curRow, sheetName, filename);
                    else
                        PumpXlsFactRow2006(dataRow, mapping);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                        "При обработке строки {0} листа {1} отчета {2} возникла ошибка ({3})",
                        curRow, sheetName, filename, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #region Обработка файлов

        private const string AUXILIARY_FILE_NAME = "INFORM";
        private const string COMMON_FILE_NAME = "SPR_";
        private bool IsSkipXlsFile(string filename)
        {
            filename = filename.ToUpper();
            // пропускаем служебные файлы - новосибирск
            if (this.Region == RegionName.Novosibirsk)
                return filename.Contains(AUXILIARY_FILE_NAME);
            // пропускаем сводные файлы - карелия, с 2008 года
            if (this.Region == RegionName.Karelya)
                return ((this.DataSource.Year >= 2008) && (this.DataSource.Year < 2011) &&
                    (filename.StartsWith(COMMON_FILE_NAME)));
            return false;
        }

        private void PumpXlsFile(FileInfo file)
        {
            if (IsSkipXlsFile(file.Name))
                return;

            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);

                if ((this.Region == RegionName.Karelya) && (this.DataSource.Year >= 2009) && (this.DataSource.Year < 2011))
                {
                    PumpXlsSheetsData(file.Name, excelDoc, 2);
                }
                else
                {
                    int wsCount = excelDoc.GetWorksheetsCount();
                    for (int index = 1; index <= wsCount; index++)
                    {
                        // у новосиба обрабатываем только первый лист
                        if ((this.Region == RegionName.Novosibirsk) && (index > 1))
                            continue;
                        excelDoc.SetWorksheet(index);
                        controlSums = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        if ((this.Region == RegionName.Karelya) && (this.DataSource.Year >= 2011))
                            PumpXlsSheetData2011(file.Name, excelDoc);
                        else
                            PumpXlsSheetData(file.Name, excelDoc);
                    }
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #region Новосибирск

        private void ProcessNN_DDMMXLSFiles(DirectoryInfo dir, string archNumber, bool sameNN)
        {
            // закачиваем файлы NNm_DDMM.xls, NNr_DDMM.xls - колонки N O
            pumpOnlyNOColumns = true;
            bool isPumped = false;
            FileInfo[] nnmFiles = dir.GetFiles("*m_*.xls");
            foreach (FileInfo nnmFile in nnmFiles)
                if (((sameNN) && (archNumber == nnmFile.Name.Substring(0, 2))) ||
                    ((!sameNN) && (archNumber != nnmFile.Name.Substring(0, 2))))
                {
                    ProcessXlsFilesTemplate(dir, nnmFile.Name);
                    isPumped = true;
                }
            FileInfo[] nnrFiles = dir.GetFiles("*r_*.xls");
            foreach (FileInfo nnrFile in nnrFiles)
                if (((sameNN) && (archNumber == nnrFile.Name.Substring(0, 2))) ||
                    ((!sameNN) && (archNumber != nnrFile.Name.Substring(0, 2))))
                {
                    ProcessXlsFilesTemplate(dir, nnrFile.Name);
                    isPumped = true;
                }
            // если файлов NNm_DDMM.xls, NNr_DDMM.xls нет, то качаем колонки N O из NN_DDMM.xls
            FileInfo[] nnFiles = null;
            if (!isPumped)
            {
                nnFiles = dir.GetFiles("*_*.xls");
                foreach (FileInfo nnFile in nnFiles)
                {
                    if ((nnFile.Name.Contains("m")) || (nnFile.Name.Contains("r")))
                        continue;
                    string codeStr = nnFile.Name.Substring(0, 2);
                    // первые два символа должны быть цифры
                    if (CommonRoutines.TrimNumbers(codeStr) != "")
                        continue;
                    if (((sameNN) && (archNumber == codeStr)) || ((!sameNN) && (archNumber != codeStr)))
                        ProcessXlsFilesTemplate(dir, nnFile.Name);
                }
            }
            // закачиваем столбцы, кроме N O из  NN_DDMM.xls
            pumpOnlyNOColumns = false;
            nnFiles = dir.GetFiles("*_*.xls");
            isPumped = false;
            foreach (FileInfo nnFile in nnFiles)
            {
                if ((nnFile.Name.Contains("m")) || (nnFile.Name.Contains("r")))
                    continue;
                string codeStr = nnFile.Name.Substring(0, 2);
                // первые два символа должны быть цифры
                if (CommonRoutines.TrimNumbers(codeStr) != "")
                    continue;
                if (((sameNN) && (archNumber == codeStr)) || ((!sameNN) && (archNumber != codeStr)))
                {
                    isPumped = true;
                    ProcessXlsFilesTemplate(dir, nnFile.Name);
                }
            }
            if (isPumped)
                return;
            // закачиваем столбцы, кроме N O из  NNm_DDMM.xls
            foreach (FileInfo nnmFile in nnmFiles)
                if (((sameNN) && (archNumber == nnmFile.Name.Substring(0, 2))) ||
                    ((!sameNN) && (archNumber != nnmFile.Name.Substring(0, 2))))
                {
                    isPumped = true;
                    ProcessXlsFilesTemplate(dir, nnmFile.Name);
                }
            if (isPumped)
                return;
            // закачиваем столбцы, кроме N O из  NNr_DDMM.xls
            foreach (FileInfo nnrFile in nnrFiles)
                if (((sameNN) && (archNumber == nnrFile.Name.Substring(0, 2))) ||
                    ((!sameNN) && (archNumber != nnrFile.Name.Substring(0, 2))))
                {
                    isPumped = true;
                    ProcessXlsFilesTemplate(dir, nnrFile.Name);
                }
        }

        private void ProcessNN_DDMMArchFiles(FileInfo archFile, ArchivatorName archiveName)
        {
            WriteToTrace(string.Format("Обработка архива {0}", archFile.Name), TraceMessageKind.Warning);
            isDetailData = false;
            DirectoryInfo tempDir = GetTempDir(archFile.FullName, archiveName);
            try
            {
                archName = archFile.Name;
                string archNumber = archFile.Name.Substring(0, 2);
                int archRegionCode = Convert.ToInt32(archNumber);
                if (!pumpedRegions.Contains(archRegionCode))
                    pumpedRegions.Add(archRegionCode);
                // сначала нужно закачать районы из файлов с совпадающим с архивом номером (блять!!!!!! пишу хуйню!)
                pumpOnlyRegion = true;
                novosibFileType = NovosibFileType.NNDDMM;
                // обрабатываем файлы NNm_DDMM.xls, NNr_DDMM.xls, где NN совпадает с NN архива
                WriteToTrace("закачка районов из отчетов", TraceMessageKind.Warning);
                ProcessNN_DDMMXLSFiles(tempDir, archNumber, true);
                UpdateData();
                pumpOnlyRegion = false;

                // закачка поселений
                // ищем файлы mpOkato, mrOkato, srOkato
                FileInfo[] mFiles = tempDir.GetFiles("m*.xls");
                FileInfo[] srFiles = tempDir.GetFiles("sr*.xls");
                if ((mFiles.GetLength(0) != 0) || (srFiles.GetLength(0) != 0))
                {
                    WriteToTrace("закачка данных поселений", TraceMessageKind.Warning);
                    isDetailData = true;
                    novosibFileType = NovosibFileType.Okato;
                    // закачиваем файлы mpOkato, mrOkato - колонки N O
                    pumpOnlyNOColumns = true;
                    ProcessXlsFilesTemplate(tempDir, "m*.xls");
                    // закачиваем столбцы, кроме N O из srOKATO
                    pumpOnlyNOColumns = false;
                    if (srFiles.GetLength(0) != 0)
                        ProcessXlsFilesTemplate(tempDir, "sr*.xls");
                    else
                    {
                        // закачиваем столбцы, кроме N O из mpOKATO
                        FileInfo[] mpFiles = tempDir.GetFiles("mp*.xls");
                        if (mpFiles.GetLength(0) != 0)
                            ProcessXlsFilesTemplate(tempDir, "mp*.xls");
                        else
                        {
                            // закачиваем столбцы, кроме N O из mrOKATO
                            FileInfo[] mrFiles = tempDir.GetFiles("mr*.xls");
                            if (mrFiles.GetLength(0) != 0)
                                ProcessXlsFilesTemplate(tempDir, "mr*.xls");
                        }
                    }
                    UpdateData();
                    isDetailData = false;
                }

                // закачка данных в основную таблицу
                WriteToTrace("закачка основных данных", TraceMessageKind.Warning);
                novosibFileType = NovosibFileType.NNDDMM;
                // обрабатываем файлы NNm_DDMM.xls, NNr_DDMM.xls, где NN совпадает с NN архива
                ProcessNN_DDMMXLSFiles(tempDir, archNumber, true);
                // обрабатываем файлы NNm_DDMM.xls, NNr_DDMM.xls, где NN не совпадает с NN архива
                ProcessNN_DDMMXLSFiles(tempDir, archNumber, false);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private void ProcessDDMM_oblArchFiles(FileInfo archFile, ArchivatorName archiveName)
        {
            WriteToTrace(string.Format("Обработка архива {0}", archFile.Name), TraceMessageKind.Warning);
            novosibFileType = NovosibFileType.Obl;
            // закачиваем файлы NN_DDMM.xls
            DirectoryInfo tempDir = GetTempDir(archFile.FullName, archiveName);
            try
            {
                archName = archFile.Name;
                FileInfo[] files = tempDir.GetFiles("*.xls");
                foreach (FileInfo file in files)
                {
                    // пропускаем служебные файлы
                    if (file.Name.ToUpper().Contains(AUXILIARY_FILE_NAME))
                        continue;
                    string regCodeStr = file.Name.Substring(0, 2);
                    // должны быть две цифры, остальные файлы пропускаем
                    if (CommonRoutines.TrimNumbers(regCodeStr) != "")
                        continue;
                    int regionCode = Convert.ToInt32(regCodeStr);
                    // закачиваем только районы, которые не были закачаны ранее
                    if (pumpedRegions.Contains(regionCode))
                        continue;
                    pumpOnlyNOColumns = false;
                    ProcessXlsFilesTemplate(tempDir, file.Name);
                    pumpOnlyNOColumns = true;
                    ProcessXlsFilesTemplate(tempDir, file.Name);
                }
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private void ShowRegionForPumpCodes()
        {
            if (regionForPumpCodes.Count == 0)
                return;
            string codes = string.Empty;
            foreach (string code in regionForPumpCodes)
                codes += string.Format("{0},", code);
            codes = codes.Remove(codes.Length - 1);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                string.Format("Добавлены новые записи в классификатор Районы.Служебный для закачки. " +
                    "Заполните в этом классификаторе поле 'Тип территории' для новых записей и запустите этап обработки еще раз. " +
                    "Всего кодов: {0}, Список кодов: {1}", regionForPumpCodes.Count, codes));
        }

        private string[] REPORTDAYS = new string[] { "01", "05", "10", "15", "20", "25" };
        private string[] REPORTMONTHS = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        private void ProcessXlsFilesNovosibirsk(DirectoryInfo dir, string archExt, ArchivatorName archiveName)
        {
            string fileDay = string.Empty;
            string fileMonth = string.Empty;
            foreach (string reportMonth in REPORTMONTHS)
                foreach (string reportDay in REPORTDAYS)
                {
                    // ищем файлы NN_DDMM.arj
                    string mask = string.Format("*_*.{0}", archExt);
                    FileInfo[] archFiles = dir.GetFiles(mask, SearchOption.AllDirectories);
                    foreach (FileInfo archFile in archFiles)
                    {
                        if ((archFile.Name.Contains("_obl")) || (archFile.Name.Split('_').GetLength(0) != 2))
                            continue;
                        // берем файлы только за нужную дату (ключ - день месяц)
                        fileDay = archFile.Name.Split('_')[1].Substring(0, 2);
                        fileMonth = archFile.Name.Split('_')[1].Substring(2, 2);
                        if ((fileDay != reportDay) || (fileMonth != reportMonth))
                            continue;
                        ProcessNN_DDMMArchFiles(archFile, archiveName);
                        UpdateData();
                    }
                    isDetailData = false;
                    // ищем файлы DDMM_obl.arj
                    // из них качаем районы, которые не были закачаны из NN_DDMM.arj
                    mask = string.Format("{0}*_obl.{1}", reportDay, archExt);
                    archFiles = dir.GetFiles(mask, SearchOption.AllDirectories);
                    foreach (FileInfo archFile in archFiles)
                    {
                        // берем файлы только за нужную дату (ключ - день месяц)
                        fileDay = archFile.Name.Substring(0, 2);
                        fileMonth = archFile.Name.Substring(2, 2);
                        if ((fileDay != reportDay) || (fileMonth != reportMonth))
                            continue;
                        ProcessDDMM_oblArchFiles(archFile, archiveName);
                    }
                    pumpedRegions.Clear();
                }
        }

        private void ProcessAllFilesNovosibirsk(DirectoryInfo dir)
        {
            regionForPumpCodes = new List<string>();
            try
            {
                // закачиваем файлы в корне
                WriteToTrace("закачка xls", TraceMessageKind.Warning);
                isDetailData = false;
                pumpOnlyRegion = false;
                pumpOnlyNOColumns = false;
                ProcessXlsFilesTemplate(dir, "*.xls");
                pumpOnlyNOColumns = true;
                ProcessXlsFilesTemplate(dir, "*.xls");
                UpdateData();
                // закачиваем ебнутые архивы
                WriteToTrace("закачка arj", TraceMessageKind.Warning);
                ProcessXlsFilesNovosibirsk(dir, "arj", ArchivatorName.Arj);
                UpdateData();
                //WriteToTrace("закачка rar", TraceMessageKind.Warning);
                //ProcessNovosibXLSFiles(dir, "rar", ArchivatorName.Rar);
                //UpdateData();
                ShowRegionForPumpCodes();
            }
            finally
            {
                regionForPumpCodes.Clear();
            }
        }

        #endregion Новосибирск

        private void ProcessXlsFilesTemplate(DirectoryInfo dir, string mask)
        {
            ProcessFilesTemplate(dir, mask, new ProcessFileDelegate(PumpXlsFile), false);
        }

        private bool GetRarFiles(DirectoryInfo dir, ref FileInfo[] acrhFiles)
        {
            acrhFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            return (acrhFiles.GetLength(0) != 0);
        }

        private DirectoryInfo GetTempDir(string fileName, ArchivatorName archName)
        {
            string tempDirPath = CommonRoutines.ExtractArchiveFileToTempDir(fileName,
                archName, FilesExtractingOption.SingleDirectory);
            return new DirectoryInfo(tempDirPath);
        }

        private void ProcessXlsFiles(DirectoryInfo dir)
        {
            pumpOnlyNOColumns = false;
            FileInfo[] archFiles = null;
            DirectoryInfo tempDir = dir;
            if (GetRarFiles(dir, ref archFiles))
                tempDir = GetTempDir(archFiles[0].FullName, ArchivatorName.Rar);
            ProcessXlsFilesTemplate(tempDir, "*.xls");
            if (tempDir != dir)
                CommonRoutines.DeleteDirectory(tempDir);
        }

        private void PumpXlsFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            isDetailData = false;
            try
            {
                // в новосибирске состав отчетов и порядок их разбора настолько ЕБНУТЫЙ!!!, что приходится разбирать их отдельно
                if (this.Region == RegionName.Novosibirsk)
                    ProcessAllFilesNovosibirsk(dir);
                else
                    ProcessXlsFiles(dir);
                UpdateData();
            }
            finally
            {
                deletedDateList.Clear();
            }
        }

        #endregion Обработка файлов

        #endregion Работа с Xls

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            PumpDbfFiles(dir);
            PumpTxtFiles(dir);
            PumpXlsFiles(dir);
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        #region Новосибирск

        private void QueryDataForProcessNovosibirsk()
        {
            QueryData();
            string dateConstraint = string.Empty;
            int dateRefMin = -1;
            int dateRefMax = -1;
            // заключительные обороты
            if (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbFinalOverturn", "False")))
                dateRefMin = dateRefMin = this.DataSource.Year * 10000 + 12 * 100 + 32;
            else if ((year > 0) && (month >= 0))
            {
                dateRefMin = year * 10000 + (month) * 100;
                if (month == 0)
                    month = 12;
                dateRefMax = year * 10000 + (month) * 100 + CommonRoutines.GetDaysInMonth(month, year);
            }
            if (dateRefMin != -1)
                dateConstraint = string.Format(" and RefYearDayUNV >= {0} and RefYearDayUNV <= {1}", dateRefMin, dateRefMax);
            InitDataSet(ref daIncomesForm16, ref dsIncomesForm16, fctIncomesForm16, false,
                string.Format("SOURCEID = {0}{1}", this.SourceID, dateConstraint), string.Empty);
        }

        protected override void AfterProcessDataAction()
        {
            UpdateMessagesDS();
        }

        private void SetTerrType()
        {
            DataTable dt = dsRegions.Tables[0];
            int count = dt.Rows.Count;
            regionForPumpCache.Clear();
            FillRowsCache(ref regionForPumpCache, dsRegionForPump.Tables[0], new string[] { "OKATO", "Name" }, "|", "RefTerrType");
            for (int i = 0; i <= count - 1; i++)
            {
                DataRow row = dt.Rows[i];
                string code = row["Code"].ToString();
                string name = row["Name"].ToString();
                string regkey = string.Format("{0}|{1}", code, name);
                if (!regionForPumpCache.ContainsKey(regkey))
                    continue;
                string terrType = regionForPumpCache[regkey];
                row["RefTerrType"] = terrType;
            }
        }

        private void SetBudgetLevelByTerrType()
        {
            DataTable dt = dsIncomesForm16.Tables[0];
            int count = dt.Rows.Count;
            for (int i = 0; i <= count - 1; i++)
            {
                DataRow row = dt.Rows[i];
                int budgetLevel = Convert.ToInt32(row["RefBdgtLevels"]);
                int regionId = Convert.ToInt32(row["REFREGIONS"]);
                DataRow[] region = dsRegions.Tables[0].Select(string.Format("ID = {0}", regionId));
                int terrType = Convert.ToInt32(region[0]["RefTerrType"]);
                if ((terrType == 7) && ((budgetLevel == 14) || (budgetLevel == 5) || (budgetLevel == 6)))
                    row["REFBDGTLEVELS"] = "15";
                if ((terrType == 4) && (budgetLevel == 14))
                    row["REFBDGTLEVELS"] = "4";
            }
        }

        private int[] GetDisintBudgetLevel(int terrType)
        {
            switch (terrType)
            {
                case 4:
                case 5:
                case 6:
                    return new int[] { 5, 6 };
                case 7:
                    return new int[] { 15 };
                default:
                    return null;
            }
        }

        private void DisintegrateRow(DataRow sourceRow, DataRow disintRow, string[] fieldsForDisint, string[] sourceSumsField, int terrType)
        {
            DataRow row = null;
            bool zeroSums = true;
            bool isDisinted = false;
            int count = fieldsForDisint.GetLength(0);
            int[] disintBudgetlevel = GetDisintBudgetLevel(terrType);
            if (disintBudgetlevel == null)
                return;
            double mr_percent = Convert.ToDouble(disintRow["MR_PERCENT"]);
            double stad_percent = Convert.ToDouble(disintRow["STAD_PERCENT"]);
            double percentSum = mr_percent + stad_percent;
            if (percentSum == 0)
                return;
            for (int j = 0; j <= disintBudgetlevel.GetLength(0) - 1; j++)
            {
                zeroSums = true;
                for (int i = 0; i < count; i++)
                {
                    if (row == null)
                    {
                        row = dsIncomesForm16.Tables[0].NewRow();
                        CopyRowToRow(sourceRow, row);
                    }
                    double d = Convert.ToDouble(sourceRow[sourceSumsField[i]]);
                    switch (disintBudgetlevel[j])
                    {
                        case 5: d *= mr_percent / percentSum;
                            break;
                        case 6: d *= stad_percent / percentSum;
                            break;
                        case 15:
                            // у го сумму не меняем
                            //Convert.ToDouble(disintRow["GO_PERCENT"]) / 100;
                            break;
                    }
                    if (d != 0)
                        zeroSums = false;
                    row[fieldsForDisint[i]] = d;
                }
                if (!zeroSums)
                {
                    row["RefBdgtLevels"] = disintBudgetlevel[j];
                    row["Ghost"] = "3";
                    row["ID"] = GetGeneratorNextValue(fctIncomesForm16);
                    dsIncomesForm16.Tables[0].Rows.Add(row);
                    isDisinted = true;
                }
                row = null;
            }
            if (isDisinted)
            {
                sourceRow["Ghost"] = "2";
                for (int i = 0; i < count; i++)
                    sourceRow[fieldsForDisint[i]] = "0";
            }
        }

        private void DisintegrateRegionData(string regionId, int terrType)
        {
            // Кэш правил расщепления: ключ - год, значение - список правил (ключ - код КД, значение - строка правила)
            FillDisintRulesCache();
            DataTable dt = dsIncomesForm16.Tables[0];
            DataRow[] rows = dt.Select(String.Format("RefRegions = {0}", regionId));
            int ghost = -1;
            DataRow row = null;
            int count = rows.GetLength(0);
            if (disintAll)
            {
                // удаляем записи с признаком 3; обнуляем суммы для записей с признаком 2, меняем у них признак на 1
                for (int i = 0; i <= count - 1; i++)
                {
                    row = rows[i];
                    ghost = Convert.ToInt32(row["Ghost"]);
                    if (ghost == 3)
                        row.Delete();
                    else if (ghost == 2)
                    {
                        row["FROMBEGINYEAR"] = "0";
                        row["FORCURRMONTH"] = "0";
                        row["GHOST"] = "1";
                    }
                }
            }
            UpdateData();
            rows = dt.Select(String.Format("RefRegions = {0}", regionId));
            count = rows.GetLength(0);
            for (int i = 0; i <= count - 1; i++)
            {
                row = rows[i];
                ghost = Convert.ToInt32(row["Ghost"]);
                if (ghost >= 3)
                    continue;
                if ((ghost == 2) && (!disintAll))
                    continue;
                string refKd = row["RefKD"].ToString();
                DataRow[] kdRows = dsKd.Tables[0].Select(String.Format("ID = {0}", refKd));
                string kd = kdRows[0]["CodeStr"].ToString();
                string date = row["RefYearDayUNV"].ToString();
                int year = Convert.ToInt32(date.Substring(0, 4));
                DataRow disintRow = FindDisintRule(disintRulesCache, year, kd);
                if (disintRow == null)
                {
                    // Не найдено не одного расщепления - пишем в протокол некритическую ошибку
                    WriteRecInMessagesDS(date, year, this.SourceID, kd);
                    continue;
                }
                DisintegrateRow(row, disintRow, new string[] { "FROMBEGINYEAR", "FORCURRMONTH" },
                    new string[] { "FrmBgnYearReport", "FrCrrMonthReport" }, terrType);
            }
        }

        private void DisintegrateData()
        {
            DataTable dt = dsRegions.Tables[0];
            int count = dt.Rows.Count;
            for (int i = 0; i <= count - 1; i++)
            {
                DataRow row = dt.Rows[i];
                string name = row["Name"].ToString();
                string code = row["Code"].ToString();
                if (code == "-1")
                    continue;
                int terrType = Convert.ToInt32(row["RefTerrType"]);
                if (terrType == 0)
                {
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning,
                        string.Format("Район '{0}' (код {1}) имеет неуказанный тип территории и данные не будут расщеплены по уровням бюджетов. ", name, code));
                    continue;
                }
                DisintegrateRegionData(row["Id"].ToString(), terrType);
            }
        }

        #endregion Новосибирск

        #region Карелия-2011

        private void QueryDataForProcessKarelya()
        {
            InitClsDataSet(ref daLocBdgt, ref dsLocBdgt, clsLocBdgt, false, string.Empty);
            FillRowsCache(ref locBdgtCacheById, dsLocBdgt.Tables[0], "Id", "RefBudgetLevels");
            InitDataSet(ref daIncomesForm16, ref dsIncomesForm16, fctIncomesForm16, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
        }

        private void UpdateProcessedDataKarelya()
        {
            UpdateDataSet(daIncomesForm16, dsIncomesForm16, fctIncomesForm16);
        }

        private void ProcessFinalizingKarelya()
        {
            ClearDataSet(ref dsLocBdgt);
            ClearDataSet(ref dsIncomesForm16);
        }

        private void SetBudgetLevelByLocBdgt()
        {
            DataTable dt = dsIncomesForm16.Tables[0];
            int count = dt.Rows.Count;
            for (int i = 0; i <= count - 1; i++)
            {
                DataRow row = dt.Rows[i];
                int refLocBdgt = Convert.ToInt32(row["RefLocalBdgt"]);
                if (locBdgtCacheById.ContainsKey(refLocBdgt))
                {
                    row["RefBdgtLevels"] = locBdgtCacheById[refLocBdgt];
                }
            }
        }

        #endregion Карелия-2011

        #region Перекрытые методы обработки

        protected override void QueryDataForProcess()
        {
            if (this.Region == RegionName.Novosibirsk)
                QueryDataForProcessNovosibirsk();
            else if (this.Region == RegionName.Karelya)
                QueryDataForProcessKarelya();
        }

        protected override void UpdateProcessedData()
        {
            if (this.Region == RegionName.Novosibirsk)
                UpdateData();
            else if (this.Region == RegionName.Karelya)
                UpdateProcessedDataKarelya();
        }

        protected override void ProcessFinalizing()
        {
            if (this.Region == RegionName.Novosibirsk)
                PumpFinalizing();
            else if (this.Region == RegionName.Karelya)
                ProcessFinalizingKarelya();
        }

        protected override void ProcessDataSource()
        {
            if (this.Region == RegionName.Novosibirsk)
            {
                // проставление типа территории в соответствии с типом территории классификатора районы.служебный для закачки
                SetTerrType();
                UpdateData();
                // проставление уровня бюджета в соответствии с типом территории
                SetBudgetLevelByTerrType();
                UpdateData();
                PrepareMessagesDS();
                DisintegrateData();
                UpdateData();
            }
            else if (this.Region == RegionName.Karelya)
            {
                if (this.DataSource.Year >= 2011)
                    SetBudgetLevelByLocBdgt();
            }
        }

        protected override void DirectProcessData()
        {
            if ((this.Region != RegionName.Novosibirsk) && (this.Region != RegionName.Karelya))
                return;
            year = -1;
            month = -1;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                GetDisintParams(ref year, ref month, ref disintAll);
            if (this.Region == RegionName.Novosibirsk)
                ProcessDataSourcesTemplate(year, month, "Расщепление сумм фактов по нормативам отчислений и установка типа территории.");
            else if (this.Region == RegionName.Karelya)
                ProcessDataSourcesTemplate(year, month, "Проставление ссылок на классификатор 'Фиксированный.Уровни бюджета'.");
        }

        #endregion Перекрытые методы обработки

        #endregion Обработка данных

    }
}
