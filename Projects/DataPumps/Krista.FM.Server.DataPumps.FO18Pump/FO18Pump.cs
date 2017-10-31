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

namespace Krista.FM.Server.DataPumps.FO18Pump
{
    // ФО_0018_КЦ СИСТЕМА
    public class FO18PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.КЦ Система (d.KD.KCSystema)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> cacheKD = null;

        // Получатели.КЦ Система (d.PBS.KCSystema)
        private IDbDataAdapter daPBS;
        private DataSet dsPBS;
        private IClassifier clsPBS;
        private Dictionary<string, int> cachePBS = null;

        // Расходы.КЦ Система (d.R.KCSystema)
        private IDbDataAdapter daR;
        private DataSet dsR;
        private IClassifier clsR;
        private Dictionary<string, int> cacheR = null;

        // ЭКР.КЦ Система (d.EKR.KCSystema)
        private IDbDataAdapter daEKR;
        private DataSet dsEKR;
        private IClassifier clsEKR;
        private Dictionary<string, int> cacheEKR = null;

        // Тип средств.КЦ Система (fx.MeansType.KCSystema)
        private IDbDataAdapter daMeansType;
        private DataSet dsMeansType;
        private IClassifier clsMeansType;
        private Dictionary<int, int> cacheMeansType = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.КЦ Система (f.D.KCSystema)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IFactTable fctIncomes;

        // Расходы.КЦ Система (f.R.KCSystema)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IFactTable fctOutcomes;

        #endregion Факты

        // тип отчета
        private ReportType reportType;

        // ссылка на фиксированный классификатор "Период.ГодКварталМесяц"
        private int refYearDayUNV;
        // ссылка на фиксированный классификатор "Период.Дата утверждение"
        private int refAssertionDate;

        // массив контрольных сумм
        decimal[] totalSums;

        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;
        private List<int> deletedDateList = null;

        #endregion Поля

        #region Перечисления

        /// <summary>
        /// Тип отчета
        /// </summary>
        private enum ReportType
        {
            // Доходы
            Incomes,
            // Расходы
            Outcomes,
            // Справочник
            Catalogue
        }

        #endregion Перечисления

        #region Константы

        private const string CONST_CATALOGUE_DIR_NAME = "СПРАВОЧНИКИ";
        private const string CONST_TOTAL_STR = "ВСЕГО";
        private const string CONST_NPP_STR = "№ П/П";

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daMeansType, ref dsMeansType, clsMeansType, true, string.Empty, string.Empty);
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daPBS, ref dsPBS, clsPBS, false, string.Empty);
            InitClsDataSet(ref daR, ref dsR, clsR, false, string.Empty);
            InitClsDataSet(ref daEKR, ref dsEKR, clsEKR, false, string.Empty);
            InitFactDataSet(ref daIncomes, ref dsIncomes, fctIncomes);
            InitFactDataSet(ref daOutcomes, ref dsOutcomes, fctOutcomes);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMeansType, dsMeansType.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheKD, dsKD.Tables[0], "CodeStr");
            FillRowsCache(ref cachePBS, dsPBS.Tables[0], "CodeStr");
            FillRowsCache(ref cacheR, dsR.Tables[0], "CodeStr");
            FillRowsCache(ref cacheEKR, dsEKR.Tables[0], "Code");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daPBS, dsPBS, clsPBS);
            UpdateDataSet(daR, dsR, clsR);
            UpdateDataSet(daEKR, dsEKR, clsEKR);
            UpdateDataSet(daIncomes, dsIncomes, fctIncomes);
            UpdateDataSet(daOutcomes, dsOutcomes, fctOutcomes);
        }

        private const string FX_MEANS_TYPE_GUID    = "18b199bb-ac62-4b46-a31a-aa19b89ae267";
        private const string D_KD_KC_SYSTEMA_GUID  = "a2a4532b-4ac2-49b0-a1ce-40f998e02f85";
        private const string D_PBS_KC_SYSTEMA_GUID = "bc626968-5ad7-4231-acd4-0749bc8726ee";
        private const string D_R_KC_SYSTEMA_GUID   = "4255d53f-b546-414b-8a9a-1c735c6fa3df";
        private const string D_EKR_KC_SYSTEMA_GUID = "800fe76d-323f-438e-abee-f1645b5bbafa";
        private const string F_D_KC_SYSTEMA_GUID   = "8579f1b0-a26a-4f05-96b9-80ddc22721f9";
        private const string F_R_KC_SYSTEMA_GUID   = "16b919b9-7d8a-4817-a7f0-a5c2a6790e93";
        protected override void InitDBObjects()
        {
            clsMeansType = this.Scheme.Classifiers[FX_MEANS_TYPE_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_KC_SYSTEMA_GUID],
                clsPBS = this.Scheme.Classifiers[D_PBS_KC_SYSTEMA_GUID],
                clsR = this.Scheme.Classifiers[D_R_KC_SYSTEMA_GUID],
                clsEKR = this.Scheme.Classifiers[D_EKR_KC_SYSTEMA_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomes = this.Scheme.FactTables[F_D_KC_SYSTEMA_GUID],
                fctOutcomes = this.Scheme.FactTables[F_R_KC_SYSTEMA_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomes);
            ClearDataSet(ref dsOutcomes);
            ClearDataSet(ref dsMeansType);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsPBS);
            ClearDataSet(ref dsR);
            ClearDataSet(ref dsEKR);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 15;
            while (emptyStrCount < 10)
            {
                if (excelHelper.GetCell(sheet, curRow, 2).Value.Trim() == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
                curRow++;
            }
            return (curRow - 10);
        }

        // определение верхней границы отчёта
        private bool IsReportStart(object sheet, int curRow)
        {
            // для отчётов по доходам начало со слова "ВСЕГО"
            if (reportType == ReportType.Incomes)
                return excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper().StartsWith(CONST_TOTAL_STR);

            // для отчётов по расходам начало со слова "ВСЕГО"
            if (reportType == ReportType.Outcomes)
                return excelHelper.GetCell(sheet, curRow, 3).Value.Trim().ToUpper().StartsWith(CONST_TOTAL_STR);

            // для справочников начало c "№ П/П"
            return excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper().StartsWith(CONST_NPP_STR);
        }

        // определение нижней границы отчёта
        private bool IsReportEnd(object sheet, int curRow)
        {
            // для отчётов по доходам конец отчёта определяется по слову "ВСЕГО"
            if (reportType == ReportType.Incomes)
                return excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper().StartsWith(CONST_TOTAL_STR);

            // для отчётов по расходам начала и для справочников конец отчёта - пустая строка
            for (int curCol = 1; curCol < 10; curCol++)
                if (excelHelper.GetCell(sheet, curRow, curCol).Value.Trim() != string.Empty)
                    return false;
            return true;
        }

        private int GetKdStartColumn()
        {
            if ((this.Region == RegionName.Omsk) || (this.Region == RegionName.OmskCity))
                return 4;
            else if (this.DataSource.Year >= 2009)
                return 4;
            else
                return 3;
        }

        // закачка классификатора "КД.КЦ_Система"
        private int PumpKD(object sheet, int curRow)
        {
            int kdStartColumn = GetKdStartColumn();
            string kdCode = string.Empty;
            for (int i = kdStartColumn; i <= kdStartColumn + 4; i++)
                kdCode += excelHelper.GetCell(sheet, curRow, i).Value.Trim();
            string kdName = string.Format("{0} ({1})", excelHelper.GetCell(sheet, curRow, 2).Value.Trim(), kdCode);
            return PumpCachedRow(cacheKD, dsKD.Tables[0], clsKD, kdCode, new object[] { "CodeStr", kdCode, "Name", kdName });
        }

        // закачка классификатора "Расходы.КЦ_Система"
        private int PumpR(string codeStr)
        {
            codeStr = codeStr.Replace(" ", string.Empty);
            return PumpCachedRow(cacheR, dsR.Tables[0], clsR,
                new object[] { "CodeStr", codeStr }, codeStr, "ID");
        }

        // закачка классификатора "ЭКР.КЦ_Система"
        private int PumpEKR(int code, string name)
        {
            return PumpCachedRow(cacheEKR, dsEKR.Tables[0], clsEKR,
                new object[] { "Code", code, "Name", name }, code.ToString(), "ID");
        }

        // поиск классификатора "Получатели.КЦ_Система" по коду
        private int GetPBS(string code)
        {
            return FindCachedRow(cachePBS, code, -1);
        }

        // определение типа отчёта по его содержимому
        private ReportType GetReportType(object sheet, string dirName)
        {
            if (dirName.ToUpper() == CONST_CATALOGUE_DIR_NAME)
            {
                return ReportType.Catalogue;
            }
            else
            {
                for (int curRow = 1; curRow <= rowsCount; curRow++)
                {
                    if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper().StartsWith(CONST_TOTAL_STR))
                        return ReportType.Incomes;
                    if (excelHelper.GetCell(sheet, curRow, 3).Value.Trim().ToUpper().StartsWith(CONST_TOTAL_STR))
                        return ReportType.Outcomes;
                }
                return ReportType.Incomes;
            }
        }

        private void CheckPBS()
        {
            // для отчётов по расходам классификатор "Получатели.КЦ Система" должен быть заполнен
            if (reportType == ReportType.Outcomes && cachePBS.Count < 1)
                throw new Exception("Не заполнен классификатор \"Получатели.КЦ Система\" - закачайте справочные таблицы");
        }

        // получить ссылку на Тип средств.КЦ Система по коду
        private int GetRefMeansType(int code)
        {
            return FindCachedRow(cacheMeansType, code, 0);
        }

        private void CheckOutcomesControlSums(object sheet, int curRow)
        {
            for (int i = 0; i <= 1; i++)
            {
                decimal cellValue = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, i + 4).Value.Trim().PadLeft(1, '0'));
                if (totalSums[i] != cellValue)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Сумма строк {0} не совпадает с контрольной {1} (столбец {2}).", totalSums[i], cellValue, i + 4));
            }
        }

        #region Омск

        private void PumpOmskXlsRow(object sheet, int curRow, int refDate, ref decimal[] totalSums)
        {
            int refKD = PumpKD(sheet, curRow);
            decimal planForYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 9).Value);
            totalSums[0] += planForYear;
            decimal planCorrForYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 10).Value);
            totalSums[1] += planCorrForYear;
            decimal planCorrFromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 11).Value);
            totalSums[2] += planCorrFromBeginYear;
            decimal factFromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 12).Value);
            totalSums[3] += factFromBeginYear;
            object[] mapping = new object[] { "RefKD", refKD, "RefYearDayUNV", refDate, "RefAssertionDate", refDate, "PlanForYear", planForYear, 
                "PlanCorrForYear", planCorrForYear, "PlanCorrFromBeginYear", planCorrFromBeginYear, "FactFromBeginYear", factFromBeginYear};
            PumpRow(dsIncomes.Tables[0], mapping);
            if (dsIncomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daIncomes, ref dsIncomes);
            }
        }

        private void CheckOmskControlSums(object sheet, int curRow)
        {
            for (int i = 0; i <= 3; i++)
            {
                decimal cellValue = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 9 + i).Value.Trim());
                if (totalSums[i] != cellValue)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "сумма строк {0} не совпадает с контрольной {1}. Столбец '{2}'", cellValue, totalSums[i], 9 + i));
            }
        }

        #endregion Омск

        #region Пенза

        private void PumpPenzaXlsSum(decimal sum, int refKD, int refDate, int refDateUNV)
        {
            object[] mapping = new object[] { "RefKD", refKD, "RefYearDayUNV", refDateUNV, 
                "RefAssertionDate", refDate, "PlanForPeriod", sum };
            PumpRow(dsIncomes.Tables[0], mapping);
            if (dsIncomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daIncomes, ref dsIncomes);
            }
        }

        // закачка строки из отчёта по доходам
        private void PumpPenzaXlsRow(object sheet, int curRow, int refDate)
        {
            int refKD = PumpKD(sheet, curRow);
            decimal totalSum = 0;
            int offset = this.DataSource.Year >= 2009 ? 1 : 0;
            for (int i = 1; i <= 12; i++)
            {
                int refDateUNV = this.DataSource.Year * 10000 + i * 100;
                decimal sum = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, i + offset + 8).Value.Trim().PadLeft(1, '0'));
                totalSum += sum;
                PumpPenzaXlsSum(sum, refKD, refDate, refDateUNV);
            }
            decimal reportTotalSum = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, offset + 8).Value.Trim().PadLeft(1, '0'));
            if (totalSum != reportTotalSum)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Сумма столбцов {0} не совпадает с контрольной {1} (строка {2})", totalSum, reportTotalSum, curRow));
        }

        // закачка одной записи в таблицу фактов (расходы)
        private void PumpPenzaOutcomesRow(object[] mapping)
        {
            PumpRow(dsOutcomes.Tables[0], mapping);
            if (dsOutcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daOutcomes, ref dsOutcomes);
            }
        }

        // закачка строки из отчёта по расходам
        private void PumpPenzaXlsRowOutcomes(object sheet, int curRow)
        {
            string value = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();

            int refR = PumpR(value.Substring(0, 22));
            int refPBS = GetPBS(value.Substring(22, 5));
            int refEKR = PumpEKR(Convert.ToInt32(value.Substring(27)), excelHelper.GetCell(sheet, curRow, 3).Value.Trim());

            object[] mapping = new object[] { "RefR", refR, "RefPBS", refPBS, "RefEKR", refEKR, 
                    "RefYearDayUNV", refYearDayUNV, "RefAssertionDate", refAssertionDate };

            decimal planForYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 4).Value.Trim().PadLeft(1, '0'));
            totalSums[0] += planForYear;
            decimal planCorrForYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 5).Value.Trim().PadLeft(1, '0'));
            totalSums[1] += planCorrForYear;

            PumpPenzaOutcomesRow((object[])CommonRoutines.ConcatArrays(mapping, new object[] {
                "PlanForYear", planForYear, "PlanCorrForYear", planCorrForYear, "RefMeansType", GetRefMeansType(5) }));

            decimal[] totalSumsPlanFact = new decimal[] { 0, 0 };
            for (int i = 1; i <= 4; i++)
            {
                object[] mappingPlanFact = new object[] { };

                value = excelHelper.GetCell(sheet, curRow, i + 6).Value.Trim();
                if (value != string.Empty)
                {
                    decimal planCorrFromBeginYear = Convert.ToDecimal(value);
                    totalSumsPlanFact[0] += planCorrFromBeginYear;
                    mappingPlanFact = new object[] { "PlanCorrFromBeginYear", planCorrFromBeginYear };
                }

                value = excelHelper.GetCell(sheet, curRow, i + 11).Value.Trim();
                if (value != string.Empty)
                {
                    decimal factFromBeginYear = Convert.ToDecimal(value);
                    totalSumsPlanFact[1] += factFromBeginYear;
                    mappingPlanFact = (object[])CommonRoutines.ConcatArrays(mappingPlanFact,
                        new object[] { "FactFromBeginYear", factFromBeginYear });
                }
                
                if (mappingPlanFact.Length > 0)
                    PumpPenzaOutcomesRow((object[])CommonRoutines.ConcatArrays(mapping,
                        mappingPlanFact, new object[] { "RefMeansType", GetRefMeansType(i) }));
            }

            for (int i = 1; i <= 2; i++)
            {
                decimal reportTotalSum = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, i * 5 + 1).Value.Trim().PadLeft(1, '0'));
                if (totalSumsPlanFact[i - 1] != reportTotalSum)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Сумма столбцов \"Объём финансирования\" {0} не совпадает с контрольной {1} (строка {2})",
                        totalSumsPlanFact[i - 1], reportTotalSum, curRow));
            }
        }

        // закачка строки из отчёта "Справочники"
        // по сути формирование классификатора «Получатели.КЦ Система» 
        private void PumpPenzaXlsRowCatalogue(object sheet, int curRow)
        {
            string codeStr = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
            string inn = excelHelper.GetCell(sheet, curRow, 3).Value.Trim();
            string kpp = excelHelper.GetCell(sheet, curRow, 4).Value.Trim();
            string name = excelHelper.GetCell(sheet, curRow, 5).Value.Trim();
            PumpCachedRow(cachePBS, dsPBS.Tables[0], clsPBS, codeStr,
                new object[] { "CodeStr", codeStr, "INN", inn, "KPP", kpp, "Name", name });
        }

        #endregion Пенза

        private void DeleteDateData(int refDate)
        {
            if (deletedDateList.Contains(refDate))
                return;
            DeleteData(string.Format("RefAssertionDate = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
            deletedDateList.Add(refDate);
        }

        // выбор номера строки отчёта с датой
        private int GetDateRow()
        {
            if ((this.Region == RegionName.Omsk) || (this.Region == RegionName.OmskCity))
                return 8;
            return 6;
        }

        // вычисление даты "ГодМесяцДень" для отчёта по доходам
        // "ГодКварталМесяц" и "Дата утверждения" для отчёта по расходам
        // для отчёта "Справочники" дата не нужна
        private void GetRefDate(object sheet)
        {
            if (reportType == ReportType.Incomes)
            {
                string date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, GetDateRow(), 1).Value);
                refAssertionDate = CommonRoutines.ShortDateToNewDate(date.Split(new char[] { ' ' })[0]);
                DeleteDateData(refAssertionDate);
            }
            else if (reportType == ReportType.Outcomes)
            {
                string dateRow = excelHelper.GetCell(sheet, 8, 1).Value.Trim().ToUpper();
                int position = dateRow.LastIndexOf("ПО");
                // получаем дату "ГодКварталМесяц" в формате xxxx0000, где xxxx - год из даты отчёта
                refYearDayUNV = CommonRoutines.ShortDateToNewDate(
                    CommonRoutines.TrimLetters(dateRow.Substring(0, position)));
                refYearDayUNV = refYearDayUNV - refYearDayUNV % 10000 + 1;
                // получаем дату утверждения
                refAssertionDate = CommonRoutines.ShortDateToNewDate(
                    CommonRoutines.TrimLetters(dateRow.Substring(position)));
                DeleteDateData(refAssertionDate);
            }
        }

        private void PumpXLSSheet(object sheet, FileInfo file)
        {
            GetRefDate(sheet);
            totalSums = new decimal[] { 0, 0, 0, 0 };
            // номер строки с контрольными суммами
            int numTotalSumRow = 0;
            bool toPumpRow = false;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    string value = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    // в отчётах по расходам почти все строки начинаются с пустой строки
                    // так что нужна ещё одна проверка
                    if (value == string.Empty && reportType != ReportType.Outcomes)
                        continue;

                    if (!toPumpRow && IsReportStart(sheet, curRow))
                    {
                        toPumpRow = true;
                        numTotalSumRow = curRow;
                        continue;
                    }

                    if (toPumpRow && IsReportEnd(sheet, curRow))
                    {
                        toPumpRow = false;
                        // конец данных - проверяем контрольные суммы
                        if ((this.Region == RegionName.Omsk) || (this.Region == RegionName.OmskCity))
                            CheckOmskControlSums(sheet, curRow);
                        if (reportType == ReportType.Outcomes)
                            CheckOutcomesControlSums(sheet, numTotalSumRow);
                        return;
                    }

                    if (toPumpRow)
                    {
                        switch (reportType)
                        { 
                            case ReportType.Outcomes:
                                PumpPenzaXlsRowOutcomes(sheet, curRow);
                                break;
                            case ReportType.Catalogue:
                                PumpPenzaXlsRowCatalogue(sheet, curRow);
                                break;
                            default:
                                if ((this.Region == RegionName.Omsk) || (this.Region == RegionName.OmskCity))
                                    PumpOmskXlsRow(sheet, curRow, refAssertionDate, ref totalSums);
                                else
                                    PumpPenzaXlsRow(sheet, curRow, refAssertionDate);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, file.FullName, ex.Message), ex);
                }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int i = 1; i <= sheetCount; i++)
                {
                    object sheet = excelHelper.GetSheet(workbook, i);
                    rowsCount = GetRowsCount(sheet);
                    reportType = GetReportType(sheet, file.Directory.Name);
                    CheckPBS();
                    PumpXLSSheet(sheet, file);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        private void PumpXLSFiles(DirectoryInfo dir)
        {
            // закачка отчётов из каталога "Справочники"
            DirectoryInfo[] subdir = dir.GetDirectories(CONST_CATALOGUE_DIR_NAME);
            if (subdir.Length > 0)
                ProcessFilesTemplate(subdir[0], "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
            // закачка отчётов из корня источника
            // вложенные каталоги не обходится
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false, SearchOption.TopDirectoryOnly);
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            deletedDateList = new List<int>();
            excelHelper = new ExcelHelper();
            try
            {
                if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0)
                    return;
                PumpXLSFiles(dir);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataFYTemplate();
        }
        
        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
