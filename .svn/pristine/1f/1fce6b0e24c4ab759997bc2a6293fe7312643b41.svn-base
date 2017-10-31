using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS14Pump
{

    // ФНС - 0014 - Форма 5-НДПИ
    public class FNS14PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 НДПИ (d_Marks_FNS5NDPI)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        private int nullMarks;
        // Районы.ФНС (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, DataRow> cacheRegions = null;
        private Dictionary<string, string> cacheRegionsNames = null;
        private Dictionary<string, DataRow> cacheRegionsFirstRow = null;
        private int nullRegions;
        // ЕдИзмер.ОКЕИ (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 НДПИ_Сводный (f_D_FNS5NDPITotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 НДПИ_Районы (f_D_FNS5NDPIRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        // параметры обработки
        private int year = -1;
        private int month = -1;
        // итоговая сумма
        private decimal[] totalSums;
        private decimal sumMultiplier = 1;

        // номер строки с наименованиями и кодами районов
        private int regionsRow = 0;
        private bool noSvodReports = false;
        private bool isRegionsSpecial = false;
        private bool isTyvaRegion2009 = false;

        private bool hasTitleSheet = false;

        #endregion Поля

        #region Структуры, перечисления

        // Тип отчета
        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5NDPI_GUID = "8bb7b783-1703-4b4d-bb75-9876e219fc58";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5NDPI_TOTAL_GUID = "56715e2c-42f1-4b53-9cd7-5abb50cbf8e8";
        private const string F_D_FNS_5NDPI_REGIONS_GUID = "4568c3e3-5755-4f45-8327-b4449923e737";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5NDPI_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5NDPI_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5NDPI_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsUnits);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        // обнуление итоговой суммы
        private void SetNullTotalSum()
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                totalSums[i] = 0;
            }
        }

        // проверка контрольной суммы
        private void CheckTotalSum(decimal totalSum, decimal controlSum, string comment)
        {
            if (totalSum != controlSum)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F} {2}",
                    controlSum, totalSum, comment));
            }
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Trim().ToUpper().Trim('X').Trim('Х').PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        private void SetSumMultiplier(int marksCode)
        {
            sumMultiplier = 1000;
            if (this.DataSource.Year >= 2007)
            {
                int code = marksCode / 1000;
                if ((code == 2) || (code == 3) || (code == 4))
                    sumMultiplier = 1000;
                else
                    sumMultiplier = 1;
            }
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 НДПИ' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isTyvaRegion2009 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year >= 2009);
            isRegionsSpecial = (reportType == ReportType.Region) &&
                ((this.Region == RegionName.AltayKrai) || (this.Region == RegionName.Kostroma));
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            SetFlags();
            CheckMarks();
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
        }

        private void PumpXlsFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            // reportType = ReportType.Str;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            // ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        private void CheckDirectories(DirectoryInfo dir)
        {
            noSvodReports = (this.Region == RegionName.Tyva) && (this.DataSource.Year >= 2009);
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                if (!noSvodReports)
                    throw new Exception(string.Format("Отсутствует каталог \"{0}\"", constSvodDirName));
            }
            if (str.GetLength(0) == 0)
                dir.CreateSubdirectory(constStrDirName);
            if (reg.GetLength(0) == 0)
                dir.CreateSubdirectory(constRegDirName);
            // Каталоги Строки и Районы для одного месяца не могут быть заполнены одновременно
            if ((str.GetLength(0) > 0 && str[0].GetFiles().GetLength(0) > 0) &&
                (reg.GetLength(0) > 0 && reg[0].GetFiles().GetLength(0) > 0))
                throw new Exception("Каталоги \"Строки\" и \"Районы\" для одного месяца не могут быть заполнены одновременно");
        }

        #endregion Общие функции закачки

        #region Работа с Excel

        private void CheckXlsTotalSums(ExcelHelper excelDoc, int curRow)
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, i + 3));
                string comment = string.Format("по столбцу {0}", i + 3);
                CheckTotalSum(totalSums[i], controlSum, comment);
            }
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            if (regionName == string.Empty)
                regionName = regionCode;
            // если у регионов наименования одинаковые, а коды разные,
            // то к наименованию необходимо приписывать код в скобках
            if (!cacheRegionsNames.ContainsKey(regionCode))
            {
                // проверка: встречалось ли такое наименование, но с другим кодом
                if (cacheRegionsNames.ContainsValue(regionName))
                {
                    // если да, то необходимо изменить наименование у первой попавшейся записи с таким же наименованием
                    if (cacheRegionsFirstRow.ContainsKey(regionName))
                    {
                        DataRow firstRow = cacheRegionsFirstRow[regionName];
                        firstRow["Name"] = string.Format("{0} ({1})", firstRow["Name"], firstRow["Code"]);
                        cacheRegionsFirstRow.Remove(regionName);
                    }
                    regionName = string.Format("{0} ({1})", regionName, regionCode);
                }
                cacheRegionsNames.Add(regionCode, regionName);
            }
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            DataRow regionRow = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regionKey, mapping, false);
            // запоминаем регионы с уникальными наименованиями
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private void PumpFactRow(decimal factValue, int refDate, int refRegions, int refMarks, int sumIndex)
        {
            if (factValue == 0)
                return;

            totalSums[sumIndex] += factValue;
            factValue *= sumMultiplier;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] { "ValueReport", factValue, "RefYearDayUNV", refDate, "RefMarks", refMarks };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "ValueReport", factValue, "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private int GetRowOKATO(ExcelHelper excelDoc, int curRow)
        {
            while (!excelDoc.GetValue(curRow, 1).Trim().ToUpper().StartsWith(MARK_REGION))
                curRow++;
            return curRow;
        }

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            string regionName = string.Empty;
            string regionCode = string.Empty;
            string cellValue = excelDoc.GetValue(curRow, 1).Trim();
            if (this.Region == RegionName.Altay)
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 1).Trim());
                regionName = excelDoc.GetValue(curRow - 2, 1).Trim();
            }
            else if (cellValue.Trim().ToUpper().StartsWith(MARK_TAX_DEP))
            {
                regionCode = CommonRoutines.TrimLetters(cellValue);
                regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
            }
            else
            {
                regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
                if ((regionCode == string.Empty) && (this.Region == RegionName.Orenburg) && (this.DataSource.Year >= 2008))
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(GetRowOKATO(excelDoc, curRow + 3), 3).Trim());
            }
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsRegionsTyva(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow < rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                if (cellValue == REGION_ROW)
                {
                    string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    string regionName = excelDoc.GetValue(curRow, 5).Trim();
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow, string name)
        {
            int code = -1;
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if (value != string.Empty)
                code = Convert.ToInt32(value);
            SetSumMultiplier(code);
            object[] mapping = new object[] { "NAME", name, "CODE", code };
            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions, string markName)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, markName);
            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
            PumpFactRow(factValue, refDate, refRegions, refMarks, 0);
        }

        #region Специальный формат по районам

        private int FindRegion(string regionCode, string regionName)
        {
            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            regionKey = string.Format("{0}|{1} ({0})", regionCode, regionName); ;
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            return nullRegions;
        }

        private const string SUF = "[SUF]";
        private int PumpXlsRegionsSpecial(ExcelHelper excelDoc, int curRow)
        {
            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(curRow, curCol).Trim();
                if ((cellValue == string.Empty) || (cellValue.ToUpper() == SUF))
                {
                    return (curCol - 3);
                }
                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(curRow - 1, curCol).Trim();
                PumpRegion(regionCode, regionName);
            }
        }

        private void PumpXlsRowRegionsSpecial(ExcelHelper excelDoc, int curRow, int refDate, string markName)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, markName);

            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(regionsRow, curCol).Trim();
                if ((cellValue == string.Empty) || (cellValue.ToUpper() == SUF))
                    return;

                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(regionsRow - 1, curCol).Trim();
                int refRegions = FindRegion(regionCode, regionName);

                decimal valueReport = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                PumpFactRow(valueReport, refDate, refRegions, refMarks, curCol - 3);
            }
        }

        #endregion Специальный формат по районам

        private const string REGION_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private const string MARK_REGION = "КОД ОКАТО";
        private const string MARK_TAX_DEP = "НАЛОГОВЫЙ ОРГАН";
        private bool IsRegionRow(ExcelHelper excelDoc, int curRow)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion2009 || isRegionsSpecial)
                return false;
            string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            if (cellValue.StartsWith(REGION_ROW))
                return excelDoc.GetValue(curRow + 2, 1).Trim().ToUpper().StartsWith(MARK_REGION);
            return cellValue.StartsWith(MARK_TAX_DEP);
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            if (reportType == ReportType.Region)
                switch (this.Region)
                {
                    case RegionName.EAO:
                    case RegionName.Orenburg:
                        return (cellValue.ToUpper() == AUX_TABLE_MARK_REGION);
                }
            return false;
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        private const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        // получаем полное наименование показателя,
        // т.к. оно может находиться на нескольких строках
        private string GetXlsMarksName(ExcelHelper excelDoc, ref int curRow)
        {
            List<string> marksName = new List<string>();
            while (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
            {
                marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
                curRow++;
            }
            marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
            return string.Join(" ", marksName.ToArray());
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {

            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            int indexStartSection = 0;

            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsRegionRow(excelDoc, curRow))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        curRow += 2;
                        continue;
                    }

                    if (IsSectionEnd(cellValue) && toPumpRow)
                    {
                        CheckXlsTotalSums(excelDoc, curRow);
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        string markName = GetXlsMarksName(excelDoc, ref curRow);
                        if (isRegionsSpecial)
                            PumpXlsRowRegionsSpecial(excelDoc, curRow, refDate, markName);
                        else
                            PumpXlsRow(excelDoc, curRow, refDate, refRegions, markName);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        //в сводных отчетах если нет титульного, то пропускаем первую секцию А
                        if (!hasTitleSheet && (indexStartSection < 1))
                        {
                            indexStartSection++;
                            continue;
                        }
                        
                        // в некоторых отчетах райнов перед основными разделами добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1).Trim()))
                            continue;

                        int columnsCount = 1;
                        if (isRegionsSpecial)
                        {
                            regionsRow = curRow;
                            columnsCount = PumpXlsRegionsSpecial(excelDoc, curRow);
                        }
                        totalSums = new decimal[columnsCount];
                        SetNullTotalSum();
                        toPumpRow = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                         curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int refRegions = nullRegions;
                int wsCount = excelDoc.GetWorksheetsCount();
                hasTitleSheet = false;//нужно для самары ГО, чтобы считывать с определнной секции

                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    string worksheetName = excelDoc.GetWorksheetName().Trim().ToUpper();


                    if (isTyvaRegion2009)
                    {
                        if (worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ"))
                            refRegions = PumpXlsRegionsTyva(excelDoc);
                    }
                    else if ((this.Region == RegionName.AltayKrai) && (this.DataSource.Year <= 2008))
                    {
                        if (((worksheetName == "СВОД") && (reportType == ReportType.Svod)) ||
                            ((worksheetName == "ПО МО") && (reportType == ReportType.Region)))
                            PumpXlsSheetData(file.Name, excelDoc, refDate, nullRegions);
                    }
                    else 
                    {
                        if ((this.Region == RegionName.SamaraGO) && worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ") && (this.DataSource.Year >= 2008) && (this.reportType == ReportType.Svod))
                            hasTitleSheet = true;
                        else PumpXlsSheetData(file.Name, excelDoc, refDate, refRegions); 
                    }
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            cacheRegionsNames = new Dictionary<string, string>();
            cacheRegionsFirstRow = new Dictionary<string, DataRow>();
            try
            {
                CheckDirectories(dir);
                PumpXlsFiles(dir);
                UpdateData();
            }
            finally
            {
                cacheRegionsFirstRow.Clear();
                cacheRegionsNames.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private const string KILO_TONS_UNIT_NAME = "Тысяча тонн";
        private const string KILOGRAM_UNIT_NAME = "Килограмм";
        private const string MILLION_CUBE_METER_UNIT_NAME = "Миллион кубических метров";
        private const string ROUBLE_UNIT_NAME = "Рубль";
        private const string THOUSAND_CUBE_METER_UNIT_NAME = "Тысяча кубических метров";
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            if (marksCode >= 2140 && marksCode <= 4023)
                return ROUBLE_UNIT_NAME;
            if ((marksCode >= 1110 && marksCode <= 1116) ||
                (marksCode >= 1140 && marksCode <= 1233))
                return KILO_TONS_UNIT_NAME;
            if (marksCode >= 1380 && marksCode <= 1411)
                return KILOGRAM_UNIT_NAME;
            if (marksCode >= 6010 && marksCode <= 6014)
                return UNIT_UNIT_NAME;
            if (marksCode >= 1120 && marksCode <= 1123)
                return MILLION_CUBE_METER_UNIT_NAME;
            if (marksCode == 1130)
                return THOUSAND_CUBE_METER_UNIT_NAME;
            return string.Empty;
        }

        private int GetRefUnits(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
            {
                string unitName = GetUnitName(marksCode);
                return FindCachedRow(cacheUnits, unitName, nullUnits);
            }
            return -1;
        }

        protected void SetRefUnits()
        {
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
                row["RefUnits"] = refUnits;
            }
        }

        private void SetClsHierarchy()
        {
            string d_Marks_FNS14_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2009)
                d_Marks_FNS14_HierarchyFileName = const_d_Marks_FNS14_HierarchyFile2009;
            else
                d_Marks_FNS14_HierarchyFileName = const_d_Marks_FNS14_HierarchyFile2007;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS14_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy()
        {
            F1NMSumCorrectionConfig f1nmSumCorrectionConfig = new F1NMSumCorrectionConfig();
            f1nmSumCorrectionConfig.EarnedField = "Value";
            f1nmSumCorrectionConfig.EarnedReportField = "ValueReport";
            f1nmSumCorrectionConfig.InpaymentsField = string.Empty;
            f1nmSumCorrectionConfig.InpaymentsReportField = string.Empty;
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByHierarchy();
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Выполняется корректировка сумм фактов по иерархии классификаторов");
        }

        #endregion Обработка данных

    }
}
