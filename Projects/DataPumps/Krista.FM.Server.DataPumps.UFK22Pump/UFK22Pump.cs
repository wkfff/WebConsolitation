using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK22Pump
{

    // УФК - 0022 - Справка о перечисленных поступлениях в бюджет
    public class UFK22PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        // ОКАТО.УФК (d_OKATO_UFK)
        private IDbDataAdapter daOkato;
        private DataSet dsOkato;
        private IClassifier clsOkato;
        private Dictionary<string, int> cacheOkato = null;
        // Местные бюджеты.УФК (d_LocBdgt_UFK)
        private IDbDataAdapter daLocBdgt;
        private DataSet dsLocBdgt;
        private IClassifier clsLocBdgt;
        private Dictionary<string, int> cacheLocBdgt = null;
        // Период.Соответствие операционных дней (d_Date_ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<int, int> cachePeriod = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Справка о перечисленных поступлениях (f_D_UFK22)
        private IDbDataAdapter daUFK22;
        private DataSet dsUFK22;
        private IFactTable fctUFK22;

        #endregion Факты

        // id источника для классификаторов на год
        int clsSourceId = -1;
        private List<int> clsSourceIds = new List<int>();
        private List<int> deletedDateList = null;
        private Dictionary<int, List<string>> pumpedDateList = null;
        // параметры обработки
        private int year = -1;
        private int month = -1;

        private bool isTyva2011SeptemberFormat;
        private bool isMoskvaObl2011MayFormat;

        private bool finalOverturn = false;
        private bool fillForPeriod = false;
        private List<int> processedYears = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            clsSourceId = AddDataSource("УФК", "0022", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            clsSourceIds.Add(clsSourceId);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            InitDataSet(ref daKd, ref dsKd, clsKd, false, string.Format("SOURCEID = {0}", clsSourceId), string.Empty);
            InitDataSet(ref daOkato, ref dsOkato, clsOkato, false, string.Format("SOURCEID = {0}", clsSourceId), string.Empty);
            InitClsDataSet(ref daLocBdgt, ref dsLocBdgt, clsLocBdgt);
            InitFactDataSet(ref daUFK22, ref dsUFK22, fctUFK22);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheOkato, dsOkato.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheLocBdgt, dsLocBdgt.Tables[0], "Name", "Id");
            FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFODate", "RefFKDate");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daOkato, dsOkato, clsOkato);
            UpdateDataSet(daLocBdgt, dsLocBdgt, clsLocBdgt);
            UpdateDataSet(daUFK22, dsUFK22, fctUFK22);
        }

        private const string D_KD_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OKATO_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string D_LOCBDGT_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string D_PERIOD_GUID = "414c27e7-393c-4516-8b47-cf6df384569d";
        private const string F_D_UFK22_GUID = "48d60267-0547-4dcd-a7d4-6a442b15db00";
        protected override void InitDBObjects()
        {
            clsPeriod = this.Scheme.Classifiers[D_PERIOD_GUID];
            this.UsedClassifiers = new IClassifier[] { clsLocBdgt = this.Scheme.Classifiers[D_LOCBDGT_GUID] };

            this.AssociateClassifiersEx = new IClassifier[] {
                clsKd = this.Scheme.Classifiers[D_KD_GUID],
                clsOkato = this.Scheme.Classifiers[D_OKATO_GUID] };

            this.UsedFacts = new IFactTable[] { fctUFK22 = this.Scheme.FactTables[F_D_UFK22_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsPeriod);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsOkato);
            ClearDataSet(ref dsLocBdgt);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        private int PumpLocBudget(string name)
        {
            name = name.Trim();
            return PumpCachedRow(cacheLocBdgt, dsLocBdgt.Tables[0], clsLocBdgt, name,
                new object[] { "Account", "Неуказанный счет", "Name", name, "OKATO", 0 });
        }

        private int PumpKd(string code)
        {
            code = CommonRoutines.TrimLetters(code.Trim());
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code, new object[] {
                "Name", constDefaultClsName, "CodeStr", code, "SourceID", clsSourceId });
        }

        private int PumpOkato(string codeStr)
        {
            codeStr = CommonRoutines.TrimLetters(codeStr.Trim());
            long code = 0;
            if (codeStr != string.Empty)
                code = Convert.ToInt64(codeStr);
            return PumpCachedRow(cacheOkato, dsOkato.Tables[0], clsOkato, code.ToString(), new object[] {
                "CODE", code, "NAME", constDefaultClsName, "SourceId", clsSourceId });
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = CommonRoutines.TrimLetters(factValue.Trim()).Replace('.', ',');
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        private string GetFactField()
        {
            if (this.Region == RegionName.MoskvaObl)
            {
                // для Москвы данные до мая 2011 года качаются в поле "За период",
                // если не идет закачка заключительных оборотов
                if (!isMoskvaObl2011MayFormat && !finalOverturn)
                    return "ForPeriod";
            }
            // всё остальное качается в поле "С начала года"
            return "FromBeginYear";
        }

        private void PumpFactRow(string factValue, int refDate, int refMarks, int refLocBdgt, int refKd, int refOkato)
        {
            decimal fact = CleanFactValue(factValue);
            if (fact == 0)
            {
                if (this.Region != RegionName.MoskvaObl)
                    return;
                // для москвы при закачке с мая-2011 или заключительных оборотов нули качаем,
                // так как они нужны при расчете данных с нарастающим итогом на этапе обработки
                // во всех других случаях нули не качаем
                if (!isMoskvaObl2011MayFormat && !finalOverturn)
                    return;
            }

            object[] mapping = new object[] {
                GetFactField(), fact, "RefYearDayUNV", refDate, "RefMarks", refMarks,
                "RefLocBdgt", refLocBdgt, "RefOKATO", refOkato, "RefKD", refKd };

            PumpRow(dsUFK22.Tables[0], mapping);
            if (dsUFK22.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK22, ref dsUFK22);
            }
        }

        #endregion Общие методы

        #region Работа с Excel

        private int[] PUMPED_FACT_COLS = new int[] { 13, 18, 21, 29, 35 };
        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refLocBdgt)
        {
            int refKd = PumpKd(excelDoc.GetValue(curRow, 1));
            int refOkato = -1;
            if (isTyva2011SeptemberFormat)
                refOkato = PumpOkato(excelDoc.GetValue(curRow, 9));
            else
                refOkato = PumpOkato(excelDoc.GetValue(curRow, 3));

            for (int i = 4; i <= 8; i++)
            {
                string factValue = string.Empty;
                if (isTyva2011SeptemberFormat)
                    factValue = excelDoc.GetValue(curRow, PUMPED_FACT_COLS[i - 4]);
                else
                    factValue = excelDoc.GetValue(curRow, i).Trim();
                PumpFactRow(factValue, refDate, i - 3, refLocBdgt, refKd, refOkato);
            }
        }

        private const string FIRST_ROW_MARK = "ПО БК";
        private bool IsStartReport(string cellValue)
        {
            return (cellValue.ToUpper() == FIRST_ROW_MARK);
        }

        private const string LAST_ROW_MARK = "ИТОГО ПО КОДУ БК";
        private bool IsEndReport(string cellValue)
        {
            return (cellValue.ToUpper() == LAST_ROW_MARK);
        }

        private int GetRefDateFromString(string value, bool shortFormat)
        {
            int refDate = 0;
            if (shortFormat)
                refDate = Convert.ToInt32(CommonRoutines.ShortDateToNewDate(value.Trim().Replace(" ", string.Empty)));
            else
                refDate = Convert.ToInt32(CommonRoutines.LongDateToNewDate(value.Trim().Replace(" ", string.Empty)));
            if (finalOverturn)
                refDate = this.DataSource.Year * 10000 + 1232;
            if (!deletedDateList.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }
            return refDate;
        }

        private int GetXlsRefDate(ExcelHelper excelDoc)
        {
            string value = CommonRoutines.TrimLetters(excelDoc.GetValue("J4").Trim());
            return GetRefDateFromString(value, true);
        }

        private const string TABLEHEADER_CODE = "КОД";
        bool isTableHeader(string cellValue)
        {
            return (cellValue.ToUpper() == TABLEHEADER_CODE);
        }

        private void PumpXlsSheetData(ExcelHelper excelDoc, string fileName)
        {
            int countEmptyStrings = 0;
            bool toPump = false;
            bool dontPump = false;

            int refDate = 0;
            if (!isTyva2011SeptemberFormat)
                refDate = GetXlsRefDate(excelDoc);
            int refLocBdgt = 0;
            if (!isTyva2011SeptemberFormat)
                refLocBdgt = PumpLocBudget(excelDoc.GetValue("B6").Trim());

            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 0; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                    {
                        if (toPump && isTyva2011SeptemberFormat)
                        {
                            countEmptyStrings++;
                            if (countEmptyStrings >= 3)
                            {
                                toPump = false;
                                countEmptyStrings = 0;
                            }
                        }
                        continue;
                    }

                    if (isTyva2011SeptemberFormat && (isTableHeader(cellValue)))
                    {
                        refDate = GetRefDateFromString(excelDoc.GetValue("K"+(curRow-6).ToString()), false);
                        string bdgtName = excelDoc.GetValue("K"+(curRow-3).ToString()).Trim();
                        if (bdgtName == string.Empty)
                            dontPump = true;
                        else refLocBdgt = PumpLocBudget(excelDoc.GetValue("K" + (curRow - 3).ToString()));
                    }


                    if (IsEndReport(cellValue))
                        if (isTyva2011SeptemberFormat)
                            continue;
                        else
                            return;

                    if (toPump)
                    {
                        PumpXlsRow(excelDoc, curRow, refDate, refLocBdgt);
                        continue;
                    }

                    if (IsStartReport(cellValue))
                    {
                        if (dontPump)
                        {
                            dontPump = false;
                            continue;
                        }
                        toPump = true;
                        if (isTyva2011SeptemberFormat)
                            curRow += 4;
                        else curRow++;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка: {1}", curRow, ex.Message), ex);
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
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheetData(excelDoc, file.Name);
                }
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Ошибка при обработке файла {0}: ({1}). Файл будет пропущен.", file.FullName, ex.Message));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Работа с Txt

        private void PumpTxtRow(string[] rowValues, int refDate, int refLocBdgt)
        {
            int refOkato = PumpOkato(rowValues[4]);
            int refKd = PumpKd(rowValues[2]);

            if (this.Region == RegionName.Orenburg)
            {
                PumpFactRow(rowValues[7], refDate, 3, refLocBdgt, refKd, refOkato);
            }
            else
            {
                PumpFactRow(rowValues[5], refDate, 1, refLocBdgt, refKd, refOkato);
                PumpFactRow(rowValues[6], refDate, 2, refLocBdgt, refKd, refOkato);
                PumpFactRow(rowValues[7], refDate, 3, refLocBdgt, refKd, refOkato);
                PumpFactRow(rowValues[8], refDate, 4, refLocBdgt, refKd, refOkato);
                PumpFactRow(rowValues[9], refDate, 5, refLocBdgt, refKd, refOkato);
            }
        }

        private int GetRefDate(string []rowValues)
        {
            //String.Format("0.{0:D2}.{1}", this.DataSource.Month, this.DataSource.Year)
            int refDate;
            string value;

            //для алтайского края дату определяем по параметрам каталога. ссылка проставляется на месяц
            if (this.Region == RegionName.AltayKrai)
                refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            else
            {
                value = rowValues[3].Trim();
                if ((value == string.Empty) && (this.Region == RegionName.Krasnodar))
                    value = rowValues[2].Trim();
                refDate = CommonRoutines.ShortDateToNewDate(value);
            }            

            // дата на основе классификатора "Период.Соответствие операционных дней"
            if ((this.Region == RegionName.Krasnodar) && ((rowValues[3].Trim() == string.Empty) || refDate <= 20120322))
            {
                if (cachePeriod.ContainsKey(refDate))
                    refDate = cachePeriod[refDate];
            }

            if (this.Region == RegionName.MoskvaObl)
            {
                if ((refDate >= 20110500) && (refDate < 20110712))
                {
                    if (cachePeriod.ContainsKey(refDate))
                        refDate = cachePeriod[refDate];
                }
            }

            // закачка заключительных оборотов
            if (finalOverturn)
            {
                refDate = this.DataSource.Year * 10000 + 1232;
            }

            // для Оренбурга в дате убираем день
            if (this.Region == RegionName.Orenburg)
            {
                refDate = refDate - refDate % 100;
            }

            return refDate;
        }

        private const string SD_MARK = "SD";
        private void PumpSDRow(string[] reportData, string filename, ref int refDate, ref int refLocBdgt)
        {
            int curRow = 0;
            while (!reportData[curRow].Trim().ToUpper().StartsWith(SD_MARK))
                curRow++;
            string[] rowValues = reportData[curRow].Split('|');

            refDate = GetRefDate(rowValues);

            // удаление закачанных ранее данных
            if (!deletedDateList.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }

            if (!pumpedDateList.ContainsKey(refDate))
                pumpedDateList.Add(refDate, new List<string>());
            pumpedDateList[refDate].Add(filename);

            refLocBdgt = PumpLocBudget(rowValues[7]);
        }

        private const string SDST_MARK = "SDST";
        private void PumpTxtFile(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int refDate = 0;
            int refLocBdgt = 0;
            PumpSDRow(reportData, file.Name, ref refDate, ref refLocBdgt);

            int rowIndex = 0;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    string[] rowValues = row.Split('|');

                    if (rowValues[0].Trim().ToUpper() == SDST_MARK)
                        PumpTxtRow(rowValues, refDate, refLocBdgt);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #endregion Работа с Txt

        #region Работа с архивами

        private void PumpArchiveFile(FileInfo file, ArchivatorName archivatorName)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, archivatorName);
            try
            {
                ProcessAllFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private void PumpArjFile(FileInfo file)
        {
            PumpArchiveFile(file, ArchivatorName.Arj);
        }

        private void PumpRarFile(FileInfo file)
        {
            PumpArchiveFile(file, ArchivatorName.Rar);
        }

        private void PumpZipFile(FileInfo file)
        {
            PumpArchiveFile(file, ArchivatorName.Zip);
        }

        #endregion Работа с архивами

        #region Перекрытые методы закачки

        protected override void DirectClsHierarchySetting()
        {
            base.DirectClsHierarchySetting();
            foreach (int clsSourceId in clsSourceIds)
            {
                // кд.уфк
                DataSet ds = null;
                clsKd.DivideAndFormHierarchy(clsSourceId, this.PumpID, ref ds);
            }
        }

        private void SetFlags()
        {
            int refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            finalOverturn = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbFinalOverturn", "False"));
            fillForPeriod = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbFillForPeriod", "False"));
            isTyva2011SeptemberFormat = (this.Region == RegionName.Tyva) && (refDate >= 20110900);
            isMoskvaObl2011MayFormat = (this.Region == RegionName.MoskvaObl) && (refDate >= 20110500);
        }

        private void CheckPumpedDate()
        {
            string message = string.Empty;
            foreach (KeyValuePair<int, List<string>> pumpedDate in pumpedDateList)
            {
                if (pumpedDate.Value.Count > 1)
                    message += string.Format("\nс датой {0} файлы '{1}';", pumpedDate.Key, string.Join("', '", pumpedDate.Value.ToArray()));
            }
            if (message != string.Empty)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "В каталоге источника содержатся файлы с одинаковой датой: {0}\n" +
                    "Возможно некорректное вычисление показателя \"За период\".", message));
            }
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.ARJ", new ProcessFileDelegate(PumpArjFile), false);
            ProcessFilesTemplate(dir, "*.SD*", new ProcessFileDelegate(PumpTxtFile), false);
            ProcessFilesTemplate(dir, "*.XLS", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.RAR", new ProcessFileDelegate(PumpRarFile), false);
            ProcessFilesTemplate(dir, "*.ZIP", new ProcessFileDelegate(PumpZipFile), false);
        }

        private void CopySumsForJanuary()
        {
            // для москвы в первом месяце (в январе) каждого года, начиная с 2012,
            // ищем суммы на самый первый день в году и копируем у них суммы "С начала года" в поле "За период"
            // а то на этапе обработке при заполнении поля "За период" для записей первого дня в году этого не произойдет,
            // т.к. нет предшествующих сумм
            if ((this.Region == RegionName.MoskvaObl) && fillForPeriod)
            {
                if ((this.DataSource.Year >= 2012) && (this.DataSource.Month == 1))
                {
                    WriteToTrace("Перенос сумм из поля \"С начала года\" в поле \"За период\"", TraceMessageKind.Information);
                    string query = string.Format(
                        " update {0} set ForPeriod = FromBeginYear " +
                        " where refyeardayunv = ( " +
                        "   select min(refyeardayunv) from {0} " +
                        "   where sourceid = {1} ) ",
                        fctUFK22.FullDBName, this.SourceID);
                    this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });
                }
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            pumpedDateList = new Dictionary<int, List<string>>();
            try
            {
                SetFlags();
                if (finalOverturn && (this.DataSource.Month != 12))
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Источник SourceID = {0} будет пропущен, т.к. выполняется закачка " +
                        "заключительных оборотов.", this.SourceID));
                    return;
                }

                ProcessAllFiles(dir);
                UpdateData();

                CopySumsForJanuary();
                CheckPumpedDate();
            }
            finally
            {
                pumpedDateList.Clear();
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        #region Проставление ссылок на уровни бюджета

        private int GetRefBudgetLevels(string locBdgtName)
        {
            locBdgtName = locBdgtName.ToUpper();
            if (locBdgtName.Contains("РЕСПУБЛИКАНСКИЙ БЮДЖЕТ") ||
                (locBdgtName.Contains("БЮДЖЕТ") && locBdgtName.Contains("КРАЯ")))
                return 3;
            if (locBdgtName.Contains("ГОРОДСКОГО ПОСЕЛЕНИЯ") || locBdgtName.Contains("П.") ||
                locBdgtName.Contains("СЕЛЬСКОГО ПОСЕЛЕНИЯ") || locBdgtName.Contains("С.") ||
                locBdgtName.Contains("СЕЛЬСОВЕТ") || locBdgtName.Contains("ПОССОВЕТ"))
                return 6;
            if (locBdgtName.Contains("ГОРОД") || locBdgtName.Contains("Г.") ||
                locBdgtName.Contains("ОКРУГ") || locBdgtName.Contains("КУРОРТ"))
                return 15;
            if (locBdgtName.Contains("РАЙОН"))
                return 5;
            if (locBdgtName.Contains("ПЕНСИОН"))
                return 8;
            if (locBdgtName.Contains("СОЦИАЛЬНОГО СТРАХОВАНИЯ"))
                return 9;
            if ((locBdgtName.Contains("ФЕДЕРАЛ") && locBdgtName.Contains("МЕДИЦИНСК")) ||
                (locBdgtName.Contains("ФЕДЕРАЛ") && locBdgtName.Contains("ОМС")))
                return 10;
            if (locBdgtName.Contains("ФЕДЕРАЛ") && locBdgtName.Contains("БЮДЖ"))
                return 1;
            if ((locBdgtName.Contains("ТЕРРИТОРИАЛЬН") && locBdgtName.Contains("МЕДИЦИН")) ||
                (locBdgtName.Contains("ТЕРРИТОРИАЛЬН") && locBdgtName.Contains("ОМС")) ||
                (locBdgtName.Contains("РЕСПУБЛИК") && locBdgtName.Contains("МЕДИЦИН")) ||
                (locBdgtName.Contains("РЕСПУБЛИК") && locBdgtName.Contains("ОМС")))
                return 11;
            return 0;
        }

        private void SetRefBudgetLevels()
        {
            foreach (DataRow row in dsLocBdgt.Tables[0].Rows)
            {
                string name = row["Name"].ToString();
                row["RefBudgetLevels"] = GetRefBudgetLevels(name);
            }
        }

        #endregion

        #region Заполнение поля "За период"

        private bool IsFillForPeriod()
        {
            if (this.Region == RegionName.MoskvaObl)
            {
                int refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
                if (refDate < 20110500)
                    return false;
                return fillForPeriod;
            }
            return false;
        }

        private void GroupSumsForPeriod(ref Dictionary<string, List<DataRow>> cache, DataTable dt, string[] keyFields)
        {
            if (cache == null)
                cache = new Dictionary<string, List<DataRow>>();
            cache.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string key = GetComplexCacheKey(row, keyFields, "|");
                if (!cache.ContainsKey(key))
                    cache.Add(key, new List<DataRow>());
                cache[key].Add(row);
            }
        }

        private DataRow GetPrevRow(List<DataRow> cache, int refDate)
        {
            // нужно искать первую предыдущую дату, за которую есть данные, ищем например в пред 62 днях, чтобы наверняка
            DateTime curDate = new DateTime(refDate / 10000, refDate / 100 % 100, refDate % 100);
            for (int i = 1; i <= 62; i++)
            {
                DateTime prevDate = curDate.AddDays(-i);
                int prevDateInt = prevDate.Year * 10000 + prevDate.Month * 100 + prevDate.Day;
                foreach (DataRow row in cache)
                {
                    if (Convert.ToInt32(row["RefYearDayUNV"]) == prevDateInt)
                        return row;
                }
            }
            return null;
        }

        private void GetFuckingCacheForPeriod(ref Dictionary<string, decimal> cache, string[] keyFields, int leftDate, int rightDate)
        {
            string query = string.Format(" select RefOKATO, RefKD, RefMarks, Sum(ForPeriod) ForPeriod from f_D_UFK22 " +
                " where (RefYearDayUNV between {0} and {1}) Group By RefOKATO, RefKD, RefMarks ", leftDate, rightDate);
            DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    string key = GetComplexCacheKey(row, keyFields, "|");
                    decimal value = Convert.ToDecimal(row["ForPeriod"]);
                    if (value == 0)
                        continue;
                    cache.Add(key, value);
                }
            }
            finally
            {
                dt.Clear();
                dt = null;
            }
        }

        private void FillForPeriod()
        {
            if (!IsFillForPeriod())
                return;

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Начало заполнения поля \"За период\"");

            // группируем суммы в одинаковой разрезности за текущий год
            GroupTable(fctUFK22,
                new string[] { "RefOKATO", "RefKD", "RefMarks", "RefLocBdgt", "RefYearDayUNV" },
                new string[] { "FromBeginYear", "ForPeriod" },
                string.Format("RefYearDayUNV >= {0}0000 and RefYearDayUNV <= {0}9999", this.DataSource.Year));

            // вспомогательная структура для вычисления меры "переломного дня" (4 мая 2011  года - покруче чем 12 12 2012)
            // ключ - составной, набор клс (кроме даты), значение - сумма в этой разрезности за период [20110101;20110431]
            // суммы аггрегируются запросом и помещаются в кэш cacheForPeriodMayFuck 
            string[] keyFields = new string[] { "RefOKATO", "RefKD", "RefMarks" };
            Dictionary<string, decimal> cacheForPeriodMayFuck = new Dictionary<string, decimal>();
            if (this.DataSource.Year == 2011)
                GetFuckingCacheForPeriod(ref cacheForPeriodMayFuck, keyFields, 20110100, 20110431);

            DataSet dsFactsForPeriod = null;
            DataSet dsFacts = null;
            try
            {
                // выгребаем все записи с текущий и предыдущий месяцы
                InitDataSet(ref daUFK22, ref dsFactsForPeriod, fctUFK22, string.Format(
                    "RefYearDayUNV >= {0}{1}00 and RefYearDayUNV <= {0}{2}00", this.DataSource.Year,
                    (this.DataSource.Month - 1).ToString().PadLeft(2, '0'), (this.DataSource.Month + 1).ToString().PadLeft(2, '0')));

                // группируем записи в одинаковой разрезности
                Dictionary<string, List<DataRow>> cacheForPeriod = null;
                GroupSumsForPeriod(ref cacheForPeriod, dsFactsForPeriod.Tables[0], keyFields);

                InitDataSet(ref daUFK22, ref dsFacts, fctUFK22, string.Format("SourceID = {0}", this.SourceID));

                foreach (DataRow row in dsFacts.Tables[0].Rows)
                {
                    string key = GetComplexCacheKey(row, keyFields, "|");
                    int refDate = Convert.ToInt32(row["RefYearDayUNV"]);

                    // дата 04.05.2011 - "переломная" (бугога) - для нее заполнение поля "за период" специфичное
                    // за период 04.05.2011 = (Сумма с начала года 04.05.2011) – (Сумма(За период[01.01.2011, 30.04.2011]))
                    // сделаем мир гаже, нахуярим креатива
                    // P.S.: так как на этапе закачки дата проставляется на основе классификатора "Соответствие операционных дней"
                    // (то есть минус один рабочий день), то "переломной" датой будет 03.05.2011
                    if (refDate == 20110503)
                    {
                        decimal value = Convert.ToDecimal(row["FromBeginYear"]);
                        if (cacheForPeriodMayFuck.ContainsKey(key))
                            value -= cacheForPeriodMayFuck[key];
                        row["ForPeriod"] = value;
                        continue;
                    }

                    decimal valueForPeriod = Convert.ToDecimal(row["FromBeginYear"]);

                    if (cacheForPeriod.ContainsKey(key))
                    {
                        DataRow prevRow = GetPrevRow(cacheForPeriod[key], refDate);
                        if (prevRow != null)
                            valueForPeriod = Convert.ToDecimal(row["FromBeginYear"]) - Convert.ToDecimal(prevRow["FromBeginYear"]);
                    }

                    row["ForPeriod"] = valueForPeriod;
                }

                UpdateDataSet(daUFK22, dsFacts, fctUFK22);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Заполнение поля \"За период\" завершено");
            }
            finally
            {
                ClearDataSet(ref dsFactsForPeriod);
                ClearDataSet(ref dsFacts);
                cacheForPeriodMayFuck.Clear();
            }
        }

        private bool IsFillForPeriodFinalOverturn()
        {
            if (this.Region != RegionName.MoskvaObl)
                return false;
            // заполнение поля "За период" по заключительным оборотам необходимо выполнять только до 2010 год включительно
            if ((this.DataSource.Year >= 2011) || processedYears.Contains(this.DataSource.Year))
                return false;
            processedYears.Add(this.DataSource.Year);
            return finalOverturn;
        }

        // заполнение поля "За период" по заключительным оборотам
        private void FillForPeriodFinalOverturn()
        {
            if (!IsFillForPeriodFinalOverturn())
                return;

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Начало заполнения поля \"За период\" по заключительным оборотам");

            int year = this.DataSource.Year;

            // группируем суммы в одинаковой разрезности за текущий год
            GroupTable(fctUFK22,
                new string[] { "RefOKATO", "RefKD", "RefMarks", "RefLocBdgt", "RefYearDayUNV" },
                new string[] { "FromBeginYear", "ForPeriod" },
                string.Format("RefYearDayUNV >= {0}0000 and RefYearDayUNV <= {0}9999", year));

            // формируем вспомогательный кэш для вычисления поля за период
            // ключ - набор ссылок на классификаторы
            // значение - сгруппированная сумма "За период" за весь год
            string[] keyFields = new string[] { "RefOKATO", "RefKD", "RefMarks" };
            Dictionary<string, decimal> cacheForPeriod = new Dictionary<string, decimal>();
            GetFuckingCacheForPeriod(ref cacheForPeriod, keyFields, year * 10000 + 100, year * 10000 + 1231);

            DataSet dsFacts = null;
            try
            {
                // выбираем данные по заключительным оборотам за год, который обрабатываем
                InitDataSet(ref daUFK22, ref dsFacts, fctUFK22, string.Format("RefYearDayUNV = {0}", year * 10000 + 1232));

                foreach (DataRow row in dsFacts.Tables[0].Rows)
                {
                    string key = GetComplexCacheKey(row, keyFields, "|");
                    if (!cacheForPeriod.ContainsKey(key))
                        continue;
                    row["ForPeriod"] = Convert.ToDecimal(row["FromBeginYear"]) - cacheForPeriod[key];
                }

                UpdateDataSet(daUFK22, dsFacts, fctUFK22);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Заполнение поля \"За период\" по заключительным оборотам завершено");
            }
            finally
            {
                ClearDataSet(ref dsFacts);
                cacheForPeriod.Clear();
            }
        }

        #endregion

        protected override void ProcessDataSource()
        {
            SetRefBudgetLevels();
            FillForPeriod();
            FillForPeriodFinalOverturn();
            UpdateData();
        }

        private string GetProcessComment()
        {
            string comment = "Выполняется установка ссылок с классификатора «Местные бюджеты.УФК» на классификатор «Фиксированный.Уровни бюджета».";
            if (fillForPeriod)
                comment += "\nВыполняется вычисление показателя \"За период\"";
            if (finalOverturn)
                comment += "\nВыполняется вычисление показателя \"За период\" по заключительным оборотам";
            return comment;
        }

        private void SetProcessFlags()
        {
            finalOverturn = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbFinalOverturn", "False"));
            fillForPeriod = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbFillForPeriod", "False"));
        }

        protected override void DirectProcessData()
        {
            processedYears = new List<int>();
            try
            {
                SetProcessFlags();
                year = -1;
                month = -1;
                GetPumpParams(ref year, ref month);
                ProcessDataSourcesTemplate(year, month, GetProcessComment());
            }
            finally
            {
                processedYears.Clear();
            }
        }

        #endregion Обработка данных

        #region Сопоставление

        protected override int GetClsSourceID(int sourceID)
        {
            if (sourceID <= 0)
                return -1;
            IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
            IDataSource clsDs = FindDataSource(ParamKindTypes.Year, ds.SupplierCode, ds.DataCode, string.Empty, ds.Year, 0, string.Empty, 0, string.Empty);
            if (clsDs == null)
                return -1;
            return clsDs.ID;
        }

        #endregion Сопоставление

    }
}
