using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS28Pump
{

    // ФНС - 0028 - Форма 5-ФЛ
    public class FNS28PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 ФЛ (d_Marks_FNS5FL)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, DataRow> cacheMarks = null;
        private Dictionary<int, string> cacheMarksNames = null;
        private Dictionary<string, DataRow> cacheMarksFirstRow = null;
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

        // Доходы.ФНС_5 ФЛ_Сводный (f_D_FNS5FLTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 ФЛ_Районы (f_D_FNS5FLRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        private ReportForm reportForm;
        private int sectionIndex = -1;
        private int year = -1;
        private int month = -1;

        private bool isTyvaRegion = false;
        private bool isKalmykyaStrReport = false;

        // контрольная сумма
        private decimal[] totalSums = new decimal[1];

        private bool noSvodReports = false;

        #endregion Поля

        #region Структуры, перечисления

        // Тип отчета
        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        // Форма отчета
        private enum ReportForm
        {
            MN,
            NIO
        }

        #endregion Структуры, перечисления

        #region Константы

        // наименования разделов для отчетов
        private string[] sectionNames = new string[3] {
            // разделы для формы НИО
            "Раздел I. Сведения о суммах налога на имущество организаций, не поступивших в бюджет в связи с предоставлением налоговых льгот",
            // разделы для формы МН
            "Раздел I. Сведения о суммах земельного налога, не поступивших в бюджет в связи с предоставлением налоговых льгот",
            "Раздел II. Сведения о суммах налога на имущество физических лиц, не поступивших в бюджет в связи с предоставлением налоговых льгот"
        };

        private int[] marksParentIds = new int[3];

        #endregion

        #region Закачка данных

        #region Работа с базой и кэшами

        // получить ID записей разделов классификатора показателей
        private void SetMarksParentIds()
        {
            for (int index = 0; index < sectionNames.Length; index++)
            {
                DataRow[] markSectionRow = dsMarks.Tables[0].Select(
                    string.Format("NAME = '{0}'", sectionNames[index]));

                if ((markSectionRow != null) && (markSectionRow.GetLength(0) > 0))
                {
                    marksParentIds[index] = Convert.ToInt32(markSectionRow[0]["ID"]);
                }
                else
                {
                    marksParentIds[index] = PumpRow(dsMarks.Tables[0], clsMarks,
                        new object[] { "CODE", 0, "NAME", sectionNames[index] });
                }
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], new string[] { "CODE", "NAME" }, "|");
//            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

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
            SetMarksParentIds();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5FL_GUID = "57815ecb-25a4-4163-a00c-c5eb958e7012";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5FL_TOTAL_GUID = "bb5e8911-e613-4a64-bddd-aafe8d8db417";
        private const string F_D_FNS_5FL_REGIONS_GUID = "1fa6848b-2211-44c4-81de-e1c1f01dde22";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5FL_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5FL_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5FL_REGIONS_GUID] };
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

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Trim().ToUpper().Trim('X').Trim('Х').PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            switch (reportForm)
            {
                case ReportForm.NIO:
                    if (cellValue.Contains("РАЗДЕЛ I"))
                        return 0;
                    break;
                case ReportForm.MN:
                    if (cellValue.Contains("РАЗДЕЛ II"))
                        return 2;
                    if (cellValue.Contains("РАЗДЕЛ I"))
                        return 1;
                    break;
            }
            return -1;
        }

        private int GetSectionIndexTyva(string worksheetName)
        {
            worksheetName = worksheetName.Trim().ToUpper();
            switch (reportForm)
            {
                case ReportForm.NIO:
                    if (worksheetName == "ЛИСТ1")
                        return 0;
                    break;
                case ReportForm.MN:
                    if (worksheetName == "ЛИСТ1")
                        return 1;
                    if (worksheetName == "ЛИСТ2")
                        return 2;
                    break;
            }
            return -1;
        }

        private void SetReportForm(string filename)
        {
            filename = filename.Trim().ToUpper();
            if (filename.Contains("МН") || filename.Contains("MN"))
                reportForm = ReportForm.MN;
            if (filename.Contains("НИО") || filename.Contains("NIO"))
                reportForm = ReportForm.NIO;
        }

        private int GetReportDate()
        {
            return (this.DataSource.Year * 10000 + 1);
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 ФЛ' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isTyvaRegion = (this.Region == RegionName.Tyva) && (reportType == ReportType.Region);
            isKalmykyaStrReport = (this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str);
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            SetFlags();
            CheckMarks();
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.rar", new ProcessFileDelegate(PumpRarFile), false);
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            reportType = ReportType.Str;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        private void CheckDirectories(DirectoryInfo dir)
        {
            noSvodReports = ((this.Region == RegionName.Kalmykya) && (this.DataSource.Year == 2010));
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

        private void PumpFactRow(decimal factValue, int refDate, int refMarks, int refRegions)
        {
            if (factValue == 0)
                return;

            totalSums[0] += factValue;

            factValue *= 1000;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] {
                    "ValueReport", factValue, "RefMarks", refMarks, "RefYearDayUNV", refDate };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] {
                    "ValueReport", factValue, "RefMarks", refMarks, "RefYearDayUNV", refDate, "RefRegions", refRegions };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private int PumpRegion(string regionCode, string regionName)
        {
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
            string regionKey = string.Format("{0}|{1}", regionCode, (this.Region != RegionName.Kalmykya)? regionName : "");
            DataRow regionRow = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regionKey, mapping, false);
            // запоминаем регионы с уникальными наименованиями
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            string regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
            string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsRegionsTyva(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                if (cellValue == REGION_START_ROW)
                {
                    string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    string regionName = excelDoc.GetValue(curRow, 5).Trim();
                    if ((regionCode == string.Empty) || (regionName == string.Empty))
                    {
                        regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow - 2, 4).Trim());
                        regionName = constDefaultClsName;
                    }
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        //закачка показателя с проверкой уникальности
        private int PumpMarks(int code, string name, string privilegeCode)
        {
            // если наименования одинаковые, а коды разные,
            // то к наименованию необходимо приписывать код в скобках
            if (!cacheMarksNames.ContainsKey(code))
            {
                // проверка: встречалось ли такое наименование, но с другим кодом
                if (cacheMarksNames.ContainsValue(name))
                {
                    // если да, то необходимо изменить наименование у первой попавшейся записи с таким же наименованием
                    if (cacheMarksFirstRow.ContainsKey(name))
                    {
                        DataRow firstRow = cacheMarksFirstRow[name];
                        firstRow["Name"] = string.Format("{0} ({1})", firstRow["Name"], firstRow["Code"]);
                        cacheMarksFirstRow.Remove(name);
                    }
                    name = string.Format("{0} ({1})", name, code);
                }
                cacheMarksNames.Add(code, name);
            }
            int markCode = 0;
            if ((this.DataSource.Year >= 2010) && (reportForm == ReportForm.NIO))
                markCode = Convert.ToInt32(String.Format("{0}0", code));
            else markCode = code;

            object[] mapping = new object[] { "Name", name, "Code", code, "ParentID", marksParentIds[sectionIndex], "MarkCode", markCode };
            if (privilegeCode != string.Empty)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "PrivilegeCode", privilegeCode });
            string key = string.Format("{0}|{1}", code, name);
            DataRow row = PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, key, mapping, false);
            // запоминаем показатели с уникальными наименованиями
            if (!cacheMarksFirstRow.ContainsKey(name))
                cacheMarksFirstRow.Add(name, row);
            return Convert.ToInt32(row["ID"]);
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow, string name)
        {
            int code = Convert.ToInt32(excelDoc.GetValue(curRow, 3).Trim());
            string privilegeCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 2).Trim()).Trim();

            return PumpMarks(code, name, privilegeCode);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions, string marksName)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, marksName);
            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
            PumpFactRow(factValue, refDate, refMarks, refRegions);
        }

        private const string SECTION_START_ROW = "НАИМЕНОВАНИЕ НАЛОГОВЫХ ЛЬГОТ";
        private bool IsSectionStart(string cellValue)
        {
            if ((this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str))
                return (cellValue.ToUpper() == "А");
            return (cellValue.ToUpper() == SECTION_START_ROW);
        }

        public const string REGION_START_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private bool IsRegionStart(string cellValue)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion)
                return false;
            return (cellValue.ToUpper() == REGION_START_ROW);
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        public const string TOTAL_ROW1 = "ВСЕГО";
        private bool IsSectionEnd(string cellValue)
        {
            if (this.Region == RegionName.Kalmykya)
                return (cellValue.ToUpper() == TOTAL_ROW1);
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private string GetXlsMarksName(ExcelHelper excelDoc, ref int curRow)
        {
            List<string> marksName = new List<string>();
            string marksCode = string.Empty;
            do
            {
                marksCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 3).Trim()).Trim();
                marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
                curRow++;
            } while (marksCode == string.Empty);
            curRow--;
            return string.Join(" ", marksName.ToArray());
        }

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

        // проверка контрольной суммы
        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            int columnCount = 1;

            int columnStart = 4;
            if ((this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str))
                columnStart = 3;

            for (int i = 0; i < columnCount; i++)
            {
                string comment = string.Format("по столбцу '{0}' раздела '{1}'", i + columnStart, sectionNames[sectionIndex]);
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, i + columnStart));
                CheckTotalSum(totalSums[i], controlSum, comment);
            }
        }

        private int GetRefMarks(int marksCode)
        {
            if (cacheMarksNames.ContainsKey(marksCode))
                return Convert.ToInt32(cacheMarks[string.Format("{0}|{1}", marksCode, cacheMarksNames[marksCode])]["ID"]);
            return nullMarks;
        }

        private void PumpXlsRowStrKalmykya(ExcelHelper excelDoc, int curRow, int refDate, int refMarks)
        {
            string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 2)).Trim();
            string regionName = excelDoc.GetValue(curRow, 1).Trim();
            int refRegions = PumpRegion(regionCode, regionName);
            
            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
            PumpFactRow(factValue, refDate, refMarks, refRegions);
        }

        private void PumpXlsSheetData(FileInfo file, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            sectionIndex = -1;
            if (isTyvaRegion)
                sectionIndex = GetSectionIndexTyva(excelDoc.GetWorksheetName());
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();

            string cutRow;
            int refMarks = nullMarks;
            bool skipSection = false;

            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if ((cellValue == string.Empty) || IsSectionEnd(cellValue))
                    {
                        toPumpRow = false;

                        if ((IsSectionEnd(cellValue)) && toPumpRow)
                        {
                            CheckXlsTotalSum(excelDoc, curRow);
                        }

                        continue;
                    }

                    if (cellValue.ToUpper().StartsWith("РАЗРЕЗ") && (cellValue.ToUpper().Contains("СТРОК")))
                    {
                        cutRow = excelDoc.GetValue(curRow + 1, 1).Trim();
                        if (cutRow.ToUpper().Contains("КОНТРОЛЬНАЯ") && cutRow.ToUpper().Contains("СУММА"))
                        {
                            skipSection = true;
                            continue;
                        }

                        int marksCode = Convert.ToInt32(cutRow.Split(new char[] { '-' })[0].Trim().PadLeft(3, '0'));
                        string marksName = cutRow.Substring(cutRow.IndexOf('-')+1).Trim();

                        if (this.DataSource.Year == 2009)
                            refMarks = GetRefMarks(marksCode);
                        else
                        {
                            refMarks = PumpMarks(marksCode, marksName, String.Empty);
                        }
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        if (skipSection)
                        {
                            skipSection = false;
                            continue;
                        }
                        if ((this.reportType != ReportType.Str) && (this.Region == RegionName.Kalmykya))
                            curRow++;
                        SetNullTotalSum();
                        toPumpRow = true;
                        continue;
                    }

                    if (toPumpRow && (cellValue == "А"))
                    {
                        continue;
                    }

                    if (IsRegionStart(cellValue))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        continue;
                    }

                    if (cellValue.ToUpper().StartsWith("РАЗДЕЛ") && (!toPumpRow))
                    {
                        sectionIndex = GetSectionIndex(cellValue);
                        continue;
                    }

                    if (toPumpRow)
                    {
                        if ((reportType == ReportType.Str) && (this.Region == RegionName.Kalmykya))
                            PumpXlsRowStrKalmykya(excelDoc, curRow, refDate, refMarks);
                        else
                        {
                            string markName = GetXlsMarksName(excelDoc, ref curRow);
                            PumpXlsRow(excelDoc, curRow, refDate, refRegions, markName);
                        }
                        continue;
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

        private bool IsTitleSheet(string worksheetName)
        {
            worksheetName = worksheetName.ToUpper();
            return (isTyvaRegion && worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ"));
        }

        private void PumpXlsFile(FileInfo file)
        {
            SetReportForm(file.Name);
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int refRegions = nullRegions;
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (IsTitleSheet(excelDoc.GetWorksheetName()))
                        refRegions = PumpXlsRegionsTyva(excelDoc);
                    else
                        PumpXlsSheetData(file, excelDoc, refDate, refRegions);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Работа с Rar

        private void PumpRarFile(FileInfo file)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
            try
            {
                ProcessAllFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        #endregion Работа с Rar

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            cacheRegionsNames = new Dictionary<string, string>();
            cacheRegionsFirstRow = new Dictionary<string, DataRow>();

            cacheMarksNames = new Dictionary<int, string>();
            cacheMarksFirstRow = new Dictionary<string, DataRow>();

            try
            {
                CheckDirectories(dir);
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
                cacheRegionsFirstRow.Clear();
                cacheRegionsNames.Clear();

                cacheMarksFirstRow.Clear();
                cacheMarksNames.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion  Закачка данных

        #region Обработка данных

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private void SetRefUnits()
        {
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                row["RefUnits"] = FindCachedRow(cacheUnits, ROUBLE_UNIT_NAME, nullUnits);
            }
        }

        private void SetClsHierarchy()
        {
            if (this.DataSource.Year >= 2010)
                SetClsHierarchy(clsMarks, ref dsMarks, "MarkCode", const_d_Marks_FNS28_HierarchyFile2010, ClsHierarchyMode.Special);
            else SetClsHierarchy(clsMarks, ref dsMarks, "CODE", const_d_Marks_FNS28_HierarchyFile2009, ClsHierarchyMode.Special);
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
