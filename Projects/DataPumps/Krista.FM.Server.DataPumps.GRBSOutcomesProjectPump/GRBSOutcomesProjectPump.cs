using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.GRBSOutcomesProjectPump
{
    /// <summary>
    /// АДМИН_0003_Проект расходов
    /// </summary>
    public class GRBSOutcomesProjectPumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ЭКР.Планирование (d.EKR.PlanOutcomes)
        private IDbDataAdapter daEKR;
        private DataSet dsEKR;
        private IClassifier clsEKR;
        private Dictionary<int, int> ekrCache = null;
        private int nullEKR;
        // Администратор.Планирование (d.KVSR.Plan)
        private IDbDataAdapter daKVSR;
        private DataSet dsKVSR;
        private IClassifier clsKVSR;
        private Dictionary<int, DataRow> kvsrRowCache = null;
        private int nullKVSR;
        // ФКР.Планирование (d.FKR.PlanOutcomes)
        private IDbDataAdapter daFKR;
        private DataSet dsFKR;
        private IClassifier clsFKR;
        private Dictionary<int, int> fkrCache = null;
        private Dictionary<int, DataRow> fkrRowCache = null;
        private int nullFKR;
        // КЦСР.Планирование (d.KCSR.PlanOutcomes)
        private IDbDataAdapter daKCSR;
        private DataSet dsKCSR;
        private IClassifier clsKCSR;
        private Dictionary<int, int> kcsrCache = null;
        private Dictionary<int, DataRow> kcsrRowCache = null;
        private int nullKCSR;
        // КВР.Планирование (d.KVR.PlanOutcomes)
        private IDbDataAdapter daKVR;
        private DataSet dsKVR;
        private IClassifier clsKVR;
        private Dictionary<int, int> kvrCache = null;
        private Dictionary<int, DataRow> kvrRowCache = null;
        private int nullKVR;
        // Вариант.Проект доходов (d.Variant.PlanOutcomes)
        private IDbDataAdapter daVariant;
        private DataSet dsVariant;
        private IClassifier clsVariant;
        // ВидОбязат.Планирование (d.KindDebt.PlanOutcomes)
        private IDbDataAdapter daKindDebt;
        private DataSet dsKindDebt;
        private IClassifier clsKindDebt;
        private Dictionary<int, int> kindDebtCache = null;
        private Dictionary<int, int> kindDebt2Columns = null;
        private int nullKindDebt;
        // Расходы.АДМИН_Проект расходов (d.R.ADMProjectOutcome)
        private IDbDataAdapter daOutcomesCls;
        private DataSet dsOutcomesCls;
        private IClassifier clsOutcomesCls;
        private Dictionary<string, int> outcomesClsCache = null;
        private int nullOutcomesCls;

        #endregion Классификаторы

        #region Факты

        // Расходы.АДМИН_Результат расходов (f.R.ADMProjectOutcomes)
        private IDbDataAdapter daADMProjectOutcomes;
        private DataSet dsADMProjectOutcomes;
        private IFactTable fctADMProjectOutcomes;

        #endregion Факты

        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;
        private decimal[] kvrTotalSums;
        private decimal[] ekrTotalSums;
        private decimal[] totalSums;
        private int kvrCode = 0;
        private int kvrID;
        private int lastSumColumnIndex = -1;
        private bool totalPresence = false;

        #endregion Поля

        #region Константы

        private const int constFirstSumColumnIndex = 7;
        private const int constNextYearFirstSumColumnIndex = 9;
        private const int constLastSumColumnIndex2008 = 19;
        private const int constLastSumColumnIndex2009 = 17;
        private const int constMaxEmptyStrings = 100;

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private int GetKindDebtID(int code)
        {
            if (kindDebtCache.ContainsKey(code))
                return kindDebtCache[code];
            else
                return kindDebtCache[-1];
        }

        private void FillKindDebt2Columns()
        {
            // Проверяем наличие заполненного классификатора «ВидОбязательств.Планирование» по источнику 
            // «АДМИН\0003 Проект расходов – Год».
            if (kindDebtCache.Count == 0)
                throw new Exception(string.Concat("Классификатор ВидОбязательств.Планирование не заполнен. Заполните классификатор по ",
                    string.Format("источнику {0} и повторите закачку", GetShortSourcePathBySourceID(this.SourceID))));

            if (kindDebt2Columns != null)
                kindDebt2Columns.Clear();

            kindDebt2Columns = new Dictionary<int, int>();
            if (this.DataSource.Year >= 2010)
            {
                kindDebt2Columns.Add(6, GetKindDebtID(1000));
                kindDebt2Columns.Add(7, GetKindDebtID(2000));
                kindDebt2Columns.Add(8, GetKindDebtID(3000));
                kindDebt2Columns.Add(9, GetKindDebtID(4110));
                kindDebt2Columns.Add(10, GetKindDebtID(4120));
                kindDebt2Columns.Add(11, GetKindDebtID(4140));
                kindDebt2Columns.Add(12, GetKindDebtID(4210));
                kindDebt2Columns.Add(13, GetKindDebtID(4220));
                kindDebt2Columns.Add(14, GetKindDebtID(4240));
                kindDebt2Columns.Add(15, GetKindDebtID(4250));
                kindDebt2Columns.Add(16, GetKindDebtID(4300));
            }
            else if (this.DataSource.Year < 2009)
            {
                kindDebt2Columns.Add(6, GetKindDebtID(1000));
                kindDebt2Columns.Add(7, GetKindDebtID(2000));
                kindDebt2Columns.Add(8, GetKindDebtID(3110));
                kindDebt2Columns.Add(9, GetKindDebtID(3120));
                kindDebt2Columns.Add(10, GetKindDebtID(3130));
                kindDebt2Columns.Add(11, GetKindDebtID(3140));
                kindDebt2Columns.Add(12, GetKindDebtID(3210));
                kindDebt2Columns.Add(13, GetKindDebtID(3220));
                kindDebt2Columns.Add(14, GetKindDebtID(3230));
                kindDebt2Columns.Add(15, GetKindDebtID(3240));
                kindDebt2Columns.Add(16, GetKindDebtID(3250));
                kindDebt2Columns.Add(17, GetKindDebtID(3300));
                kindDebt2Columns.Add(18, GetKindDebtID(3400));
            }
            else
            {
                kindDebt2Columns.Add(6, GetKindDebtID(1000));
                kindDebt2Columns.Add(7, GetKindDebtID(2000));
                kindDebt2Columns.Add(8, GetKindDebtID(3110));
                kindDebt2Columns.Add(9, GetKindDebtID(3120));
                kindDebt2Columns.Add(10, GetKindDebtID(3140));
                kindDebt2Columns.Add(11, GetKindDebtID(3210));
                kindDebt2Columns.Add(12, GetKindDebtID(3220));
                kindDebt2Columns.Add(13, GetKindDebtID(3240));
                kindDebt2Columns.Add(14, GetKindDebtID(3250));
                kindDebt2Columns.Add(15, GetKindDebtID(3300));
                kindDebt2Columns.Add(16, GetKindDebtID(3400));
            }
        }

        private void InitClsNullValues()
        {
            nullEKR = clsEKR.UpdateFixedRows(this.DB, this.SourceID);
            nullKVSR = clsKVSR.UpdateFixedRows(this.DB, this.SourceID);
            nullKCSR = clsKCSR.UpdateFixedRows(this.DB, this.SourceID);
            nullKVR = clsKVR.UpdateFixedRows(this.DB, this.SourceID);
            nullKindDebt = clsKindDebt.UpdateFixedRows(this.DB, this.SourceID);
            nullFKR = clsFKR.UpdateFixedRows(this.DB, this.SourceID);
            nullOutcomesCls = clsOutcomesCls.UpdateFixedRows(this.DB, this.SourceID);
        }

        protected override void QueryData()
        {
            InitDataSet(ref daVariant, ref dsVariant, clsVariant, string.Empty);

            InitClsDataSet(ref daEKR, ref dsEKR, clsEKR);
            InitClsDataSet(ref daFKR, ref dsFKR, clsFKR);
            InitClsDataSet(ref daKCSR, ref dsKCSR, clsKCSR);
            InitClsDataSet(ref daKindDebt, ref dsKindDebt, clsKindDebt);
            InitClsDataSet(ref daKVR, ref dsKVR, clsKVR);
            InitClsDataSet(ref daKVSR, ref dsKVSR, clsKVSR);
            InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls);

            InitFactDataSet(ref daADMProjectOutcomes, ref dsADMProjectOutcomes, fctADMProjectOutcomes);

            FillClsCache();

            InitClsNullValues();
        }

        private void FillClsCache()
        {
            FillRowsCache(ref fkrCache, dsFKR.Tables[0], "CODE");
            FillRowsCache(ref kcsrCache, dsKCSR.Tables[0], "CODE");
            FillRowsCache(ref ekrCache, dsEKR.Tables[0], "CODE");
            FillRowsCache(ref kvrCache, dsKVR.Tables[0], "CODE");
            FillRowsCache(ref kindDebtCache, dsKindDebt.Tables[0], "CODE");
            FillRowsCache(ref outcomesClsCache, dsOutcomesCls.Tables[0], new string[] { "CODE", "ParentId" }, "|", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daEKR, dsEKR, clsEKR);
            UpdateDataSet(daFKR, dsFKR, clsFKR);
            UpdateDataSet(daKCSR, dsKCSR, clsKCSR);
            UpdateDataSet(daKindDebt, dsKindDebt, clsKindDebt);
            UpdateDataSet(daKVR, dsKVR, clsKVR);
            UpdateDataSet(daKVSR, dsKVSR, clsKVSR);
            UpdateDataSet(daVariant, dsVariant, clsVariant);
            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);

            UpdateDataSet(daADMProjectOutcomes, dsADMProjectOutcomes, fctADMProjectOutcomes);
        }

        private const string D_VARIANT_PLAN_OUTCOMES_GUID = "e8cb8e78-f486-46c1-800f-284eb791d95a";
        private const string D_KINDDEBT_PLAN_OUTCOMES_GUID = "70323b90-c17e-4ccf-b76c-caedfe33b2ff";
        private const string D_FKR_PLAN_OUTCOMES_GUID = "8b0f5b42-2672-4648-9e1a-4ac0a5e5e3f8";
        private const string D_EKR_PLAN_OUTCOMES_GUID = "0c63afa9-b543-4c0c-98a9-9c290cd7ccb0";
        private const string D_KCSR_PLAN_OUTCOMES_GUID = "c651ca3c-5833-44fa-a34d-6228a8dda34a";
        private const string D_KVR_PLAN_OUTCOMES_GUID = "c2c0fa0c-6ea5-4f19-ae04-2eb12ead32f7";
        private const string D_KVSR_PLAN_OUTCOMES_GUID = "dd69b4e1-f257-49ce-b553-442d094ae39a";
        private const string D_OUTCOMES_CLS_GUID = "7eba777a-6b61-45b3-b1d9-eedacad0288f";
        private const string F_R_ADM_PROJECT_OUTCOMES_GUID = "05c8ad57-628b-404d-b0f7-f6bd576f03dd";
        protected override void InitDBObjects()
        {
            clsVariant = this.Scheme.Classifiers[D_VARIANT_PLAN_OUTCOMES_GUID];
            clsKindDebt = this.Scheme.Classifiers[D_KINDDEBT_PLAN_OUTCOMES_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsOutcomesCls = this.Scheme.Classifiers[D_OUTCOMES_CLS_GUID] };

            this.AssociateClassifiers = new IClassifier[] {
                clsFKR = this.Scheme.Classifiers[D_FKR_PLAN_OUTCOMES_GUID],
                clsEKR = this.Scheme.Classifiers[D_EKR_PLAN_OUTCOMES_GUID],
                clsKCSR = this.Scheme.Classifiers[D_KCSR_PLAN_OUTCOMES_GUID],
                clsKVR = this.Scheme.Classifiers[D_KVR_PLAN_OUTCOMES_GUID],
                clsKVSR = this.Scheme.Classifiers[D_KVSR_PLAN_OUTCOMES_GUID], 
                clsOutcomesCls };

            this.HierarchyClassifiers = new IClassifier[] { clsOutcomesCls, clsFKR, clsEKR, clsKCSR, clsKVR, clsKVSR };

            this.UsedFacts = new IFactTable[] {
                fctADMProjectOutcomes = this.Scheme.FactTables[F_R_ADM_PROJECT_OUTCOMES_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsOutcomesCls);
            ClearDataSet(ref dsEKR);
            ClearDataSet(ref dsFKR);
            ClearDataSet(ref dsKCSR);
            ClearDataSet(ref dsKindDebt);
            ClearDataSet(ref dsKVR);
            ClearDataSet(ref dsKVSR);
            ClearDataSet(ref dsVariant);
            ClearDataSet(ref dsADMProjectOutcomes);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

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

        private const string REPORT_END_MARK = "ИТОГО ПО";
        private void GetReportDataMargins(object sheet, ref int firstRow, ref int lastRow)
        {
            for (lastRow = 1; lastRow <= rowsCount; lastRow++)
            {
                string cellValue = excelHelper.GetCell(sheet, lastRow, 1).Value.Trim();
                if (cellValue.ToUpper().StartsWith(REPORT_END_MARK))
                    break;
                if (string.Compare(cellValue, "1") == 0)
                    firstRow = lastRow + 1;
            }
        }

        /// <summary>
        /// Сравнивает итоговые суммы по ЭКР и КВР
        /// </summary>
        private void CompareTotalSums(int kvrCode)
        {
            for (int i = constFirstSumColumnIndex; i <= lastSumColumnIndex; i++)
            {
                decimal sum = Math.Round(kvrTotalSums[i - 1] * 1000, 2);
                if (Math.Round(ekrTotalSums[i - 1], 2) != sum)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Сумма столбца {0} {1:F} не сходится с итоговой {2:F} по КВР {3}",
                        i, ekrTotalSums[i - 1], sum, kvrCode));
                }
            }
        }

        private int PumpKVSR(FileInfo file, object sheet, int row, ref string kvsrName)
        {
            int kvsrID = nullKVSR;
            kvsrName = file.Name.Split('.')[0];

            // Из первой строки качаем КВСР
            int kvsrCode = 0;
            string value = excelHelper.GetCell(sheet, row, 2).Value.Trim();
            if (value != string.Empty)
                kvsrCode = Convert.ToInt32(value.PadLeft(1, '0'));
            kvsrID = PumpOriginalRow(dsKVSR, clsKVSR,
                new object[] { "CODE", kvsrCode, "NAME", kvsrName }, new object[] { "CODE", kvsrCode });

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                string.Format("Данные будут записаны на {0} {1}.", kvsrCode, kvsrName));

            return kvsrID;
        }

        private int PumpVariant(ref string variantName)
        {
            // Классификатор «Вариант.Проект Расходов»
            variantName = GetParamValueByName(this.ProgramConfig, "edVariant", string.Empty);
            if (string.IsNullOrEmpty(variantName))
            {
                throw new Exception("Параметр вариант должен быть заполнен");
            }
            return PumpOriginalRow(dsVariant, clsVariant, new object[] { 
                    "CODE", 0, "NAME", variantName, "VARIANTCOMMENT", this.PumpRegistryElement.Name,
                    "VARIANTCOMPLETED", 0 });
        }

        private int PumpFKR(object sheet, int curRow)
        {
            // Классификатор ФКР.Планирование берем из ячейки 3 «Раздел, подраздел» файла 
            int fkrCode = 0;
            if (excelHelper.GetCell(sheet, curRow, 3).Value != string.Empty)
                fkrCode = Convert.ToInt32(excelHelper.GetCell(sheet, curRow, 3).Value);
            return PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, fkrCode, "CODE",
                new object[] { "NAME", constDefaultClsName });
        }

        private int PumpKCSR(object sheet, int curRow)
        {
            // Классификатор КЦСР.Планирование
            int kcsrCode = 0;
            if (excelHelper.GetCell(sheet, curRow, 4).Value != string.Empty)
                kcsrCode = Convert.ToInt32(excelHelper.GetCell(sheet, curRow, 4).Value);
            return PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, kcsrCode, "CODE",
                new object[] { "NAME", constDefaultClsName });
        }

        private void PumpXLSRow(object sheet, int curRow, int kvsrID, int variantID)
        {
            string name = excelHelper.GetCell(sheet, curRow, 1).Value;
            // Если это последняя стока отчета с итоговыми суммами, сверяем суммы отчета с итоговыми
            if (name.ToUpper().StartsWith(REPORT_END_MARK))
            {
                CompareTotalSums(kvrCode);
                for (int curCol = constFirstSumColumnIndex; curCol <= lastSumColumnIndex; curCol++)
                {
                    decimal sum = 0.0M;
                    string sumStr = excelHelper.GetCell(sheet, curRow, curCol).Value.Trim();
                    if (sumStr.Contains("!"))
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                            string.Format("Некорректная сумма: строка {0}, столбец {1}", curRow, curCol));
                        sumStr = string.Empty;
                    }
                    if (sumStr != string.Empty)
                        sum = Convert.ToDecimal(sumStr);
                    sum = sum * 1000;
                    totalSums[curCol - 1] = totalSums[curCol - 1] * 1000;
                    if (totalSums[curCol - 1] != sum)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                            "Сумма по всем КВР столбца {0} {1:F} не сходится с итоговой {2:F}", curCol, totalSums[curCol - 1], sum));
                    }
                }
                return;
            }

            int fkrID = PumpFKR(sheet, curRow);
            int kcsrID = PumpKCSR(sheet, curRow);
            string cellValue = excelHelper.GetCell(sheet, curRow, 6).Value.Replace(" ", string.Empty);
            if (cellValue == string.Empty)
                return;
            int tmpInt = Convert.ToInt32(cellValue.PadLeft(1, '0'));

            if (tmpInt == 0)
            {
                totalPresence = true;
                // Если это не первая строка отчета, сравниваем итоговые суммы с полученными
                if (kvrID != nullKVR)
                    CompareTotalSums(kvrCode);

                kvrTotalSums = new decimal[19];
                ekrTotalSums = new decimal[19];

                // Запоминаем итоговые суммы по ЭКР = 0
                decimal sum = 0.0M;
                for (int curCol = constFirstSumColumnIndex; curCol <= lastSumColumnIndex; curCol++)
                {
                    string sumStr = excelHelper.GetCell(sheet, curRow, curCol).Value.Trim();
                    if (sumStr.Contains("!"))
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, 
                            string.Format("Некорректная сумма: строка {0}, столбец {1}", curRow, curCol));
                        continue;
                    }
                    sum = Convert.ToDecimal(sumStr.PadLeft(1, '0'));
                    kvrTotalSums[curCol - 1] += sum;
                    totalSums[curCol - 1] += sum;
                }
            }
            else
            {
                // КВР.Планирование
                kvrCode = Convert.ToInt32(excelHelper.GetCell(sheet, curRow, 5).Value.PadLeft(1, '0'));
                kvrID = PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, kvrCode, "CODE",
                    new object[] { "NAME", constDefaultClsName });
                // ЭКР.Планирование
                int ekrID = PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, tmpInt, "CODE",
                    new object[] { "NAME", name });

                // Обходим столбцы сумм и закачиваем их
                for (int curCol = constFirstSumColumnIndex; curCol <= lastSumColumnIndex; curCol++)
                {
                    decimal sum = 0.0M;
                    string sumStr = excelHelper.GetCell(sheet, curRow, curCol).Value.Trim();
                    if (sumStr.Contains("!"))
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                            string.Format("Некорректная сумма: строка {0}, столбец {1}", curRow, curCol));
                        continue;
                    }
                    if (sumStr != string.Empty)
                        sum = Convert.ToDecimal(sumStr) * 1000;
                    ekrTotalSums[curCol - 1] += sum;
                    if (sum == 0)
                        continue;
                    int year = this.DataSource.Year;
                    if (this.DataSource.Year >= 2010)
                    {
                        if (curCol < constNextYearFirstSumColumnIndex + 1)
                            year = this.DataSource.Year - 1;
                    }
                    else
                    {
                        if (curCol < constNextYearFirstSumColumnIndex)
                            year = this.DataSource.Year - 1;
                    }
                    year *= 10000;
                    PumpRow(dsADMProjectOutcomes.Tables[0], new object[] { 
                                "VALUE", sum, "RefYearDayUNV", year, "REFKVSR", kvsrID, "REFKVR", kvrID, 
                                "REFKCSR", kcsrID, "REFFKR", fkrID, "REFEKR", ekrID, "REFVARIANT", variantID, 
                                "REFKINDDEBT", kindDebt2Columns[curCol - 1] });
                }

                // Сохраняем данные
                if (dsADMProjectOutcomes.Tables[0].Rows.Count > MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daADMProjectOutcomes, ref dsADMProjectOutcomes);
                }
            }
        }

        private void PumpXlsSheet(object sheet, FileInfo file)
        {
            // Находим первую и последнюю строки отчета
            int firstRowIndex = 1;
            int lastRowIndex = 1;
            GetReportDataMargins(sheet, ref firstRowIndex, ref lastRowIndex);

            string kvsrName = string.Empty;
            int kvsrID = PumpKVSR(file, sheet, firstRowIndex, ref kvsrName);
            string variantName = string.Empty;
            int variantID = PumpVariant(ref variantName);

            // Массивы для проверки итоговых сумм
            kvrTotalSums = new decimal[19];
            ekrTotalSums = new decimal[19];
            totalSums = new decimal[19];
            kvrID = nullKVR;
            totalPresence = false;

            for (int curRow = firstRowIndex; curRow <= lastRowIndex; curRow++)
                try
                {
                    PumpXLSRow(sheet, curRow, kvsrID, variantID);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, file.FullName, ex.Message), ex);
                }

            if (!totalPresence)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    "Проверьте правильность формирования файла: отсутствуют итоговые значения по КВР, то есть строки, где ЭКР равно 0000000");
        }

        // на листе должно быть "ПРОЕКТ РАСХОДОВ ОБЛАСТНОГО БЮДЖЕТА" - иначе лист не закачиваем 
        private const string REPORT_MARK = "ПРОЕКТ РАСХОДОВ ОБЛАСТНОГО БЮДЖЕТА";
        private bool CheckSheet(object sheet)
        {
            for (int curRow = 0; curRow <= rowsCount; curRow++)
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper().StartsWith(REPORT_MARK))
                    return true;
            return false;
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
                    if (!CheckSheet(sheet))
                        continue;
                    PumpXlsSheet(sheet, file);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }
        
        #endregion Работа с экселем

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            // Заполняем коллекцию соответствия колонок отчета ИД классификатора ВидОбязательств.Планирование
            FillKindDebt2Columns();
            if (this.DataSource.Year >= 2009)
                lastSumColumnIndex = constLastSumColumnIndex2009;
            else
                lastSumColumnIndex = constLastSumColumnIndex2008;

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile));
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        #region заполнение классификатора Расходы

        private const string NPA = "АДМИН_Проект расходов - {0}";
        private int PumpOutcomesClsRow(string kvsrCode, string fkrCode, string kcsrCode,
            string kvrCode, string name, int parentId)
        {
            string budgetOutcomesClsCode = string.Format("{0}{1}{2}{3}", kvsrCode, fkrCode, kcsrCode, kvrCode).TrimStart('0').PadLeft(1, '0');
            string section = fkrCode.Substring(0, 2);
            string subSection = fkrCode.Substring(2, 2);
            string npa = string.Format(NPA, this.DataSource.Year);
            object[] mapping = new object[] { "NormativeAct", npa, "Code", budgetOutcomesClsCode, "KVSR", kvsrCode, 
                "FKR", fkrCode, "KCSR", kcsrCode, "KVR", kvrCode, "Name", name, "Section", section, "Subsection", subSection };
            string cacheKey = string.Format("{0}|", budgetOutcomesClsCode);
            if (parentId != -1)
            {
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentId", parentId });
                cacheKey += parentId.ToString();
            }
            return PumpCachedRow(outcomesClsCache, dsOutcomesCls.Tables[0], clsOutcomesCls, cacheKey, mapping);
        }

        private string GetClsNameByCode(int code, DataTable dt)
        {
            DataRow[] rows = dt.Select(string.Format("Code={0}", code));
            if (rows.GetLength(0) == 0)
                return constDefaultClsName;
            else
                return rows[0]["Name"].ToString();
        }

        private const string NULL_FKR_CODE = "0000";
        private const string NULL_KCSR_CODE = "0000000";
        private const string NULL_KVR_CODE = "000";
        private int PumpOutcomesClsRows(DataRow factRow)
        {
            DataRow clsRow = FindCachedRow(kvsrRowCache, Convert.ToInt32(factRow["RefKVSR"]));
            string kvsrCode = clsRow["Code"].ToString().PadLeft(3, '0');
            string kvsrName = clsRow["Name"].ToString();
            clsRow = FindCachedRow(fkrRowCache, Convert.ToInt32(factRow["RefFKR"]));
            string fkrCode = clsRow["Code"].ToString().PadLeft(4, '0');
            string fkrName = clsRow["Name"].ToString();
            clsRow = FindCachedRow(kcsrRowCache, Convert.ToInt32(factRow["RefKCSR"]));
            string kcsrCode = clsRow["Code"].ToString().PadLeft(7, '0');
            string kcsrName = clsRow["Name"].ToString();
            clsRow = FindCachedRow(kvrRowCache, Convert.ToInt32(factRow["RefKVR"]));
            string kvrCode = clsRow["Code"].ToString().PadLeft(3, '0');
            string kvrName = clsRow["Name"].ToString();

            // 1 - создаем запись с нулевыми кодами кроме квср
            int parentId = PumpOutcomesClsRow(kvsrCode, NULL_FKR_CODE, NULL_KCSR_CODE, NULL_KVR_CODE, kvsrName, -1);
            // 2 - последние два кода фкр заменяем на нули, добавляем запись
            string parentFkrCode = string.Format("{0}{1}", fkrCode.Substring(0, 2), "00");
            string parentFkrName = GetClsNameByCode(Convert.ToInt32(parentFkrCode), dsFKR.Tables[0]);
            parentId = PumpOutcomesClsRow(kvsrCode, parentFkrCode, NULL_KCSR_CODE, NULL_KVR_CODE, parentFkrName, parentId);
            // 3 - добавляем запись с заполненным квср и фкр
            parentId = PumpOutcomesClsRow(kvsrCode, fkrCode, NULL_KCSR_CODE, NULL_KVR_CODE, fkrName, parentId);
            // 4 - последние 4 кода кцср заменяем на нули, добавляем запись
            string parentKcsrCode = string.Format("{0}{1}", kcsrCode.Substring(0, 3), "0000");
            string parentKcsrName = GetClsNameByCode(Convert.ToInt32(parentKcsrCode), dsKCSR.Tables[0]);
            parentId = PumpOutcomesClsRow(kvsrCode, fkrCode, parentKcsrCode, NULL_KVR_CODE, parentKcsrName, parentId);
            // 5 - последние 2 кода кцср заменяем на нули, добавляем запись
            parentKcsrCode = string.Format("{0}{1}", kcsrCode.Substring(0, 5), "00");
            parentKcsrName = GetClsNameByCode(Convert.ToInt32(parentKcsrCode), dsKCSR.Tables[0]);
            parentId = PumpOutcomesClsRow(kvsrCode, fkrCode, parentKcsrCode, NULL_KVR_CODE, parentKcsrName, parentId);
            // 6 - добавляем запись с заполненным квср фкр и кцср
            parentId = PumpOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, NULL_KVR_CODE, kcsrName, parentId);
            // 7 - добавляем запись с всеми заполненными кодами, на нее ставится ссылка факта
            return PumpOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, kvrCode, kvrName, parentId);
        }

        private void FillOutcomesCls()
        {
            foreach (DataRow row in dsADMProjectOutcomes.Tables[0].Rows)
                row["RefRADMProj"] = PumpOutcomesClsRows(row);
        }

        #endregion заполнение классификатора Расходы

        private void FillRowsCacheForProcess()
        {
            FillRowsCache(ref outcomesClsCache, dsOutcomesCls.Tables[0], new string[] { "CODE", "ParentId" }, "|", "Id");
            FillRowsCache(ref kvsrRowCache, dsKVSR.Tables[0], "ID");
            FillRowsCache(ref fkrRowCache, dsFKR.Tables[0], "ID");
            FillRowsCache(ref kcsrRowCache, dsKCSR.Tables[0], "ID");
            FillRowsCache(ref kvrRowCache, dsKVR.Tables[0], "ID");
        }

        private void InitFactForProcess()
        {
            string constraint = string.Format("SOURCEID = {0}", this.SourceID);
            InitDataSet(ref daADMProjectOutcomes, ref dsADMProjectOutcomes, fctADMProjectOutcomes, false, constraint, string.Empty);
        }

        protected override void QueryDataForProcess()
        {
            QueryData();
            InitFactForProcess();
            FillRowsCacheForProcess();
        }

        protected override void ProcessFinalizing()
        {
            PumpFinalizing();
        }

        protected override void UpdateProcessedData()
        {
            UpdateData();
        }

        protected override void ProcessDataSource()
        {
            FillOutcomesCls();
            UpdateProcessedData();
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = 0;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, 0, "Формирование единого расходного классификатора 'Расходы.АДМИН_Проект расходов'");
        }

        #endregion Обработка данных

    }
}