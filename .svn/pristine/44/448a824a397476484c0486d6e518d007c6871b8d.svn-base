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

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // ФО 2 мес отч скиф - формат XLS
    // только у краснодара, 2005 год, 1 - 9 месяц

    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {

        #region поля

        private Dictionary<int, int> kdSourceKeyCache = null;
        private Dictionary<int, int> srcOutFinSourceKeyCache = null;
        private Dictionary<int, int> srcInFinSourceKeyCache = null;
        private Dictionary<int, int> fkrSourceKeyCache = null;
        // source key - code
        private Dictionary<int, int> fkrAuxSourceKeyCache = new Dictionary<int, int>();
        private Dictionary<int, int> ekrSourceKeyCache = null;
        private Dictionary<int, int> fkrBookSourceKeyCache = null;
        private Dictionary<int, int> ekrBookSourceKeyCache = null;
        private Dictionary<int, int> kvsrSourceKeyCache = null;
        private Dictionary<int, int> marksOutcomesSourceKeyCache = null;
        private Dictionary<int, int> marksInDebtSourceKeyCache = null;
        private Dictionary<int, int> marksOutDebtSourceKeyCache = null;
        private Dictionary<int, int> marksArrearsSourceKeyCache = null;

        private ExcelHelper excelHelper = null;
        private object excelObj = null;

        #endregion

        #region функции закачки классификаторов

        private void PumpPatternRow(string sheetName, string code, string name, string kl, string kst)
        {
            string sourceKey = kl + kst;
            switch (sheetName.ToUpper())
            {
                case "СПИСОКАТЕ":
                    // Районы.МесОтч
                    string key = code.PadLeft(10, '0') + "|" + name;
                    PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, key, new object[] { "CodeStr", code, "Name", name, 
                        "BudgetKind", "МНЦП", "BudgetName", "Муниципальные образования" });
                    break;
                case "ОТЧЕТ":
                    switch (Convert.ToInt32(kl))
                    {
                        // кд.месотч2005
                        case 1:
                            if (!ToPumpBlock(Block.bIncomes))
                                break;
                            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                                new object[] { "CodeStr", code, "Name", name, "SourceKey", sourceKey, "kl", kl, "kst", kst });
                            break;
                        // фкр.месотч
                        case 2:
                            if (!ToPumpBlock(Block.bOutcomes))
                                break;
                            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                                new object[] { "Code", code, "Name", name, "SourceKey", sourceKey });
                            break;
                        // КИВнутрФ.МесОтч_2005
                        case 4:
                            if (!ToPumpBlock(Block.bInnerFinSources))
                                break;
                            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                                new object[] { "CodeStr", code, "Name", name, "SourceKey", sourceKey, "kl", kl, "kst", kst });
                            break;
                        // КИВнешФ.МесОтч2005
                        case 5:
                            if (!ToPumpBlock(Block.bOuterFinSources))
                                break;
                            PumpCachedRow(srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, code,
                                new object[] { "CodeStr", code, "Name", name, "SourceKey", sourceKey, "kl", kl, "kst", kst });
                            break;
                    }
                    break;
                case "СПРАВОЧНАЯ_ТАБЛИЦА":
                    int intKL = Convert.ToInt32(kl);
                    string longCode = code.Split(',')[0] + code.Split(',')[1] + code.Split(',')[2];
                    if (intKL == 1)
                    {
                        if (!ToPumpBlock(Block.bIncomesRefs))
                            break;
                        // Администратор.МесОтч
                        PumpCachedRow(kvsrCache, dsKVSR.Tables[0], clsKVSR, code.Split(',')[1],
                            new object[] { "Code", code.Split(',')[1], "Name", name, "SourceKey", sourceKey, "kl", kl, "kst", kst });
                    }
                    else if ((intKL >= 2) && (intKL <= 16) && (code.Split(',')[0].Trim('0') == string.Empty))
                    {
                        if (!ToPumpBlock(Block.bOutcomesRefs))
                            break;
                        // ЭКР.МесОтчСправРасходы
                        PumpCachedRow(ekrBookCache, dsEKRBook.Tables[0], clsEKRBook, code.Split(',')[1].PadRight(6, '0'),
                            new object[] { "Code", code.Split(',')[1].PadRight(6, '0'), "Name", name, "SourceKey", sourceKey });
                    }
                    else if ((intKL >= 2) && (intKL <= 16) && (code.Split(',')[0].Trim('0') != string.Empty))
                    {
                        if (!ToPumpBlock(Block.bOutcomesRefs))
                            break;
                        // ФКР.МесОтчСправРасходы
                        PumpCachedRow(fkrBookCache, dsFKRBook.Tables[0], clsFKRBook, code.Split(',')[0].TrimStart('0'),
                            new object[] { "Code", code.Split(',')[0].TrimStart('0'), "Name", name, "SourceKey", sourceKey });
                        if (!fkrAuxSourceKeyCache.ContainsKey(Convert.ToInt32(sourceKey)))
                            fkrAuxSourceKeyCache.Add(Convert.ToInt32(sourceKey), Convert.ToInt32(code.Split(',')[0]));
                    }
                    else if ((intKL >= 17) && (intKL <= 75))
                    {
                        if (!ToPumpBlock(Block.bOutcomesRefsAdd))
                            break;
                        // Показатели.МесОтч_СпрРасходы
                        PumpCachedRow(marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes, longCode,
                            new object[] { "LongCode", longCode, "FKR", code.Split(',')[0], "EKR", code.Split(',')[1], 
                            "Name", name, "SourceKey", sourceKey, "kl", kl, "kst", kst });
                    }
                    else if ((intKL >= 76) && (intKL <= 124))
                    {
                        if (!ToPumpBlock(Block.bInnerFinSourcesRefs))
                            break;
                        // Показатели.МесОтч_СпрВнутрДолг
                        PumpRow(dsMarksInDebt.Tables[0], clsMarksInDebt,
                            new object[] { "LongCode", longCode, "Name", name, "SrcInFin", longCode.Substring(0, 20), 
                                 "GvrmInDebt", longCode.Substring(20, 3), "SourceKey", sourceKey, "kl", kl, "kst", kst });
                    }
                    else if ((intKL >= 125) && (intKL <= 135))
                    {
                        if (!ToPumpBlock(Block.bOuterFinSourcesRefs))
                            break;
                        // Показатели.МесОтч_СпрВнешДолг
                        PumpRow(dsMarksOutDebt.Tables[0], clsMarksOutDebt,
                            new object[] { "LongCode", longCode, "Name", name, "SrcOutFin", longCode.Substring(0, 20),  
                                 "GvrmOutDebt", longCode.Substring(20, 6), "SourceKey", sourceKey, "kl", kl, "kst", kst });
                    }
                    else if ((intKL >= 136) && (intKL <= 170))
                    {
                        if (!ToPumpBlock(Block.bArrearsRefs))
                            break;
                        // Показатели.МесОтч_СпрЗадолженность
                        PumpRow(dsMarksArrears.Tables[0], clsMarksArrears,
                            new object[] { "LongCode", longCode, "FKR", code.Split(',')[0], "EKR", code.Split(',')[1],  
                            "Name", name, "SourceKey", sourceKey, "kl", kl, "kst", kst });
                    }
                    // кривая хуета - в шаблоне типа нет нужной строчки по экр.справ расходы - 260000 и 262000 - добавляем вручную...
                    if (ToPumpBlock(Block.bOutcomesRefs))
                    {
                        if (intKL == 11)
                            PumpCachedRow(ekrBookCache, dsEKRBook.Tables[0], clsEKRBook, "260000",
                                new object[] { "Code", "260000", "Name", "Социальное обеспечение", "SourceKey", sourceKey });
                        if (intKL == 12)
                            PumpCachedRow(ekrBookCache, dsEKRBook.Tables[0], clsEKRBook, "262000",
                                new object[] { "Code", "262000", "Name", "Пособия по социальной помощи населению", "SourceKey", sourceKey });
                    }
                    break;
            }
        }

        private bool IsReportEnd(string value)
        {
            return value.ToUpper().StartsWith("РУКОВОДИТЕЛЬ");
        }

        private void PumpPatternSheetWithExcelHelper(object sheet, int curRow, Dictionary<string, int> columnsMapping, string sheetName)
        {
            for (; ; curRow++)
                try
                {
                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value;
                    if (IsReportEnd(cellValue))
                        break;
                    if ((sheetName == "СписокАТЕ") && (cellValue == string.Empty))
                        break;
                    string code = excelHelper.GetCell(sheet, curRow, columnsMapping["code"]).Value;
                    code = CommonRoutines.TrimLetters(code).Replace(" ", "");
                    if (code == string.Empty)
                        continue;
                    // для справочной таблицы код составляем из трех подкодов
                    if (sheetName == "СПРАВОЧНАЯ_ТАБЛИЦА")
                        code += "," + excelHelper.GetCell(sheet, curRow, 3).Value + "," + excelHelper.GetCell(sheet, curRow, 4).Value;
                    string name = excelHelper.GetCell(sheet, curRow, columnsMapping["name"]).Value;
                    string kl = string.Empty;
                    string kst = string.Empty;
                    if (columnsMapping["kl"] != -1)
                        kl = excelHelper.GetCell(sheet, curRow, columnsMapping["kl"]).Value.PadLeft(5, '0');
                    if (columnsMapping["kst"] != -1)
                        kst = excelHelper.GetCell(sheet, curRow, columnsMapping["kst"]).Value.PadLeft(5, '0');
                    PumpPatternRow(sheetName, code, name, kl, kst);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, sheetName, ex.Message), ex);
                }
        }

        private int GetStartRow(string sheetName)
        {
            switch (sheetName.ToUpper())
            {
                case "СПИСОКАТЕ":
                    return 4;
                case "ОТЧЕТ":
                    return 16;
                case "СПРАВОЧНАЯ_ТАБЛИЦА":
                    return 13;
            }
            return -1;
        }

        private Dictionary<string, int> GetColumnMapping(string sheetName)
        {
            Dictionary<string, int> columnMapping = new Dictionary<string, int>(4);
            switch (sheetName.ToUpper())
            {
                case "СПИСОКАТЕ":
                    columnMapping.Add("code", 1);
                    columnMapping.Add("name", 2);
                    columnMapping.Add("kl", -1);
                    columnMapping.Add("kst", -1);
                    break;
                case "ОТЧЕТ":
                    columnMapping.Add("code", 1);
                    columnMapping.Add("name", 2);
                    columnMapping.Add("kl", 3);
                    columnMapping.Add("kst", 4);
                    break;
                case "СПРАВОЧНАЯ_ТАБЛИЦА":
                    columnMapping.Add("code", 2);
                    columnMapping.Add("name", 1);
                    columnMapping.Add("kl", 4);
                    columnMapping.Add("kst", 5);
                    break;
            }
            return columnMapping;
        }

        private void PumpUnknownCLS()
        {
            PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, "0",
                new object[] { "Code", "0", "Name", "Неуказанное наименование" });
            PumpCachedRow(fkrBookCache, dsFKRBook.Tables[0], clsFKRBook, "0",
                new object[] { "Code", "0", "Name", "Неуказанное наименование" });
        }

        private void PumpPattern(FileInfo patternFile)
        {
            WriteToTrace("Обработка шаблона: " + patternFile.Name, TraceMessageKind.Information);
            excelObj = excelHelper.OpenExcel(false);
            object workbook = excelHelper.GetWorkbook(excelObj, patternFile.FullName, true);
            try
            {
                PumpUnknownCLS();

                object sheet = excelHelper.GetSheet(workbook, "СписокАТЕ");
                PumpPatternSheetWithExcelHelper(sheet, GetStartRow("СписокАТЕ"), GetColumnMapping("СписокАТЕ"), "СписокАТЕ");
                sheet = excelHelper.GetSheet(workbook, "ОТЧЕТ");
                PumpPatternSheetWithExcelHelper(sheet, GetStartRow("ОТЧЕТ"), GetColumnMapping("ОТЧЕТ"), "ОТЧЕТ");
                sheet = excelHelper.GetSheet(workbook, "СПРАВОЧНАЯ_ТАБЛИЦА");
                PumpPatternSheetWithExcelHelper(sheet, GetStartRow("СПРАВОЧНАЯ_ТАБЛИЦА"),
                    GetColumnMapping("СПРАВОЧНАЯ_ТАБЛИЦА"), "СПРАВОЧНАЯ_ТАБЛИЦА");

                UpdateData();
                WriteToTrace("Обработка шаблона завершена: " + patternFile.Name, TraceMessageKind.Information);
            }
            finally
            {
                excelHelper.SetDisplayAlert(excelObj, false);
                excelHelper.CloseWorkBooks(excelObj);
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion функции закачки классификаторов

        #region функции закачки фактов

        private int GetRefEKR(string sourceKey)
        {
            foreach (KeyValuePair<int, int> item in ekrBookSourceKeyCache)
            {
                string keyStr = item.Key.ToString().PadLeft(10, '0');
                if ((!keyStr.EndsWith("1")) || (keyStr.Substring(0, 5) != sourceKey.Substring(0, 5)))
                    continue;
                return item.Value;
            }
            return nullEKRBook;
        }

        private int GetRefFKR(string codeKey)
        {
            int code = FindCachedRow(fkrAuxSourceKeyCache, Convert.ToInt32(codeKey), 0);
            return FindCachedRow(fkrBookCache, code.ToString(), nullFKRBook);
        }

        private void PumpReportRow(string reportType, object[] mapping, int clsCode, int kl, int kst)
        {
            int refCls = -1;
            switch (reportType)
            {
                case "50":
                    switch (kl)
                    {
                        case 1:
                            // ФО_МесОтч_Доходы
                            if (!ToPumpBlock(Block.bIncomes))
                                break;
                            refCls = FindCachedRow(kdSourceKeyCache, clsCode, nullKD);
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefKD", refCls });
                            PumpRow(dsMonthRepIncomes.Tables[0], mapping);
                            break;
                        case 2:
                            if (!ToPumpBlock(Block.bOutcomes))
                                break;
                            // ФО_МесОтч_Расходы
                            refCls = FindCachedRow(fkrSourceKeyCache, clsCode, nullFKR);
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefFKR", refCls, 
                                "refEKR", FindCachedRow(ekrCache, "0", nullEKR) });
                            PumpRow(dsMonthRepOutcomes.Tables[0], mapping);
                            break;
                        case 3:
                            if (!ToPumpBlock(Block.bDefProf))
                                break;
                            // ФО_МесОтч_Дефицит Профицит
                            if (kst != 1)
                                break;
                            PumpRow(dsMonthRepDefProf.Tables[0], mapping);
                            break;
                        case 4:
                            if (!ToPumpBlock(Block.bInnerFinSources))
                                break;
                            // ФО_МесОтч_ИсточникВнутрФинансирования
                            refCls = FindCachedRow(srcInFinSourceKeyCache, clsCode, nullSrcInFin);
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefSIF", refCls });
                            PumpRow(dsMonthRepInFin.Tables[0], mapping);
                            break;
                        case 5:
                            if (!ToPumpBlock(Block.bOuterFinSources))
                                break;
                            // ФО_МесОтч_ИсточникВнешФинансирования
                            refCls = FindCachedRow(srcOutFinSourceKeyCache, clsCode, nullSrcOutFin);
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefSOF", refCls });
                            PumpRow(dsMonthRepOutFin.Tables[0], mapping);
                            break;
                    }
                    break;
                case "51":
                    if (kl == 1)
                    {
                        // ФО_МесОтч_СпрДоходы
                        if (!ToPumpBlock(Block.bIncomesRefs))
                            break;
                        refCls = FindCachedRow(kvsrSourceKeyCache, clsCode, nullKVSR);
                        mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefKVSR", refCls });
                        PumpRow(dsMonthRepIncomesBooks.Tables[0], mapping);
                    }
                    else if ((kl >= 2) && (kl <= 16))
                    {
                        // ФО_МесОтч_СпрРасходы
                        if (!ToPumpBlock(Block.bOutcomesRefs))
                            break;
                        string strClsCode = clsCode.ToString();
                        if (strClsCode.EndsWith("01"))
                        {
                            int refFKR = nullFKRBook; 
                            if ((kl == 11) || (kl == 12))
                                refFKR = GetRefFKR(strClsCode);
                            else
                                refFKR = FindCachedRow(fkrBookCache, "0", nullFKRBook);
                            refCls = FindCachedRow(ekrBookSourceKeyCache, clsCode, nullEKRBook);
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefEKR", refCls, "refFKR", refFKR });
                            PumpRow(dsMonthRepOutcomesBooks.Tables[0], mapping);
                        }
                        else
                        {
                            int refEKR = GetRefEKR(strClsCode.PadLeft(10, '0'));
                            int refFKR = GetRefFKR(strClsCode);
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefFKR", refFKR, "refEKR", refEKR });
                            PumpRow(dsMonthRepOutcomesBooks.Tables[0], mapping);
                        }
                    }
                    else if ((kl >= 17) && (kl <= 75))
                    {
                        if (!ToPumpBlock(Block.bOutcomesRefsAdd))
                            break;
                        // ФО_МесОтч_СпрРасходыДоп
                        refCls = FindCachedRow(marksOutcomesSourceKeyCache, clsCode, nullMarksOutcomes);
                        mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefMarksOutcomes", refCls });
                        PumpRow(dsMonthRepOutcomesBooksEx.Tables[0], mapping);
                    }
                    else if ((kl >= 76) && (kl <= 124))
                    {
                        if (!ToPumpBlock(Block.bInnerFinSourcesRefs))
                            break;
                        // ФО_МесОтч_СпрВнутрДолг
                        refCls = FindCachedRow(marksInDebtSourceKeyCache, clsCode, nullMarksInDebt);
                        mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefMarksInDebt", refCls });
                        PumpRow(dsMonthRepInDebtBooks.Tables[0], mapping);
                    }
                    else if ((kl >= 125) && (kl <= 135))
                    {
                        if (!ToPumpBlock(Block.bOuterFinSourcesRefs))
                            break;
                        // ФО_МесОтч_СпрВнешДолг
                        refCls = FindCachedRow(marksOutDebtSourceKeyCache, clsCode, nullMarksOutDebt);
                        mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefMarksOutDebt", refCls });
                        PumpRow(dsMonthRepOutDebtBooks.Tables[0], mapping);
                    }
                    else if ((kl >= 136) && (kl <= 170))
                    {
                        if (!ToPumpBlock(Block.bArrearsRefs))
                            break;
                        // ФО_МесОтч_СпрЗадолженность
                        refCls = FindCachedRow(marksArrearsSourceKeyCache, clsCode, nullMarksArrears);
                        mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefMarksArrears", refCls });
                        PumpRow(dsMonthRepArrearsBooks.Tables[0], mapping);
                    }
                    break;
            }
        }

        private const int yearPlanSumIndex = 3;
        private const int monthPlanSumIndex = 5;
        private const int factSumIndex = 7;
        private void GetSumsMapping(string reportType, ref string budgetLevel, double sum, ref object[] sumsMapping)
        {
            switch (reportType)
            {
                case "50":
                    switch (budgetLevel)
                    {
                        case "3":
                            budgetLevel = "2";
                            sumsMapping[yearPlanSumIndex] = sum;
                            break;
                        case "4":
                            budgetLevel = "3";
                            sumsMapping[yearPlanSumIndex] = sum;
                            break;
                        case "5":
                            budgetLevel = "7";
                            sumsMapping[yearPlanSumIndex] = sum;
                            break;
                        case "6":
                            budgetLevel = "2";
                            sumsMapping[monthPlanSumIndex] = sum;
                            break;
                        case "7":
                            budgetLevel = "3";
                            sumsMapping[monthPlanSumIndex] = sum;
                            break;
                        case "8":
                            budgetLevel = "7";
                            sumsMapping[monthPlanSumIndex] = sum;
                            break;
                        case "9":
                            budgetLevel = "2";
                            sumsMapping[factSumIndex] = sum;
                            break;
                        case "10":
                            budgetLevel = "3";
                            sumsMapping[factSumIndex] = sum;
                            break;
                        case "11":
                            budgetLevel = "7";
                            sumsMapping[factSumIndex] = sum;
                            break;
                    }
                    break;
                case "51":
                    switch (budgetLevel)
                    {
                        case "5":
                            budgetLevel = "2";
                            break;
                        case "6":
                            budgetLevel = "3";
                            break;
                        case "7":
                            budgetLevel = "7";
                            break;
                    }
                    sumsMapping = (object[])CommonRoutines.ConcatArrays(sumsMapping, new object[] { "FactReport", sum });
                    break;
            }
        }

        private void FillSourceKeyCaches()
        {
            FillRowsCache(ref kvsrSourceKeyCache, dsKVSR.Tables[0], "SourceKey");
            FillRowsCache(ref marksOutcomesSourceKeyCache, dsMarksOutcomes.Tables[0], "SourceKey");
            FillRowsCache(ref fkrSourceKeyCache, dsFKR.Tables[0], "SourceKey");
            FillRowsCache(ref ekrSourceKeyCache, dsEKR.Tables[0], "SourceKey");
            FillRowsCache(ref fkrBookSourceKeyCache, dsFKRBook.Tables[0], "SourceKey");
            FillRowsCache(ref ekrBookSourceKeyCache, dsEKRBook.Tables[0], "SourceKey");
            FillRowsCache(ref kdSourceKeyCache, dsKD.Tables[0], "SourceKey");
            FillRowsCache(ref srcInFinSourceKeyCache, dsSrcInFin.Tables[0], "SourceKey");
            FillRowsCache(ref srcOutFinSourceKeyCache, dsSrcOutFin.Tables[0], "SourceKey");
            FillRowsCache(ref marksInDebtSourceKeyCache, dsMarksInDebt.Tables[0], "SourceKey");
            FillRowsCache(ref marksOutDebtSourceKeyCache, dsMarksOutDebt.Tables[0], "SourceKey");
            FillRowsCache(ref marksArrearsSourceKeyCache, dsMarksArrears.Tables[0], "SourceKey");
        }

        private void ClearSourceKeyCaches()
        {
            kvsrSourceKeyCache.Clear();
            marksOutcomesSourceKeyCache.Clear();
            fkrSourceKeyCache.Clear();
            fkrAuxSourceKeyCache.Clear();
            ekrSourceKeyCache.Clear();
            fkrBookSourceKeyCache.Clear();
            ekrBookSourceKeyCache.Clear();
            kdSourceKeyCache.Clear();
            srcInFinSourceKeyCache.Clear();
            srcOutFinSourceKeyCache.Clear();
            marksInDebtSourceKeyCache.Clear();
            marksOutDebtSourceKeyCache.Clear();
            marksArrearsSourceKeyCache.Clear();
        }

        private int GetRegionRef(string regionCode)
        {
            foreach (KeyValuePair<string, int> item in regionCache)
                if (item.Key.ToString().Split('|')[0] == regionCode.PadLeft(10, '0'))
                    return item.Value;
            return nullRegions;
        }

        private void zeroSums(ref object[] sumsMapping)
        {
            for (int i = 0; i <= sumsMapping.GetLength(0) - 1; i += 2)
                sumsMapping[i + 1] = 0;
        }

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

        private void PumpReport(FileInfo reportFile, string regionCode)
        {
            WriteToTrace("Обработка отчета: " + reportFile.Name, TraceMessageKind.Information);
            
            int regionRef = GetRegionRef(regionCode);
            string refPeriod = this.DataSource.Year.ToString() + this.DataSource.Month.ToString().PadLeft(2, '0') + "00";
            object[] sumsMapping = new object[] { "QuarterPlanReport", 0, "YearPlanReport", 0, "MonthPlanReport", 0, "FactReport", 0 };
            string budgetLevel = string.Empty;

            excelObj = excelHelper.OpenExcel(false);
            object workbook = excelHelper.GetWorkbook(excelObj, reportFile.FullName, true);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                int rowsCount = GetRowsCount(sheet);
                for (int curRow = 1; curRow <= rowsCount; curRow++)
                    try
                    {
                        SetProgress(rowsCount, curRow, string.Format("Обработка отчета {0}", reportFile.Name),
                            string.Format("Строка {0} из {1}", curRow, rowsCount));
                        string reportType = excelHelper.GetCell(sheet, curRow, 1).Value;
                        if ((reportType != "50") && (reportType != "51"))
                            continue;
                        string kl = excelHelper.GetCell(sheet, curRow, 2).Value;
                        string kst = excelHelper.GetCell(sheet, curRow, 3).Value;
                        string nextKl = string.Empty;
                        string nextKst = string.Empty;
                        if (curRow != rowsCount - 1)
                        {
                            nextKl = excelHelper.GetCell(sheet, curRow + 1, 2).Value;
                            nextKst = excelHelper.GetCell(sheet, curRow + 1, 3).Value;
                        }
                        bool toPumpRow = ((nextKst == string.Empty) || (nextKl + nextKst != kl + kst));
                        if (excelHelper.GetCell(sheet, curRow, 4).Value.Trim('0') != string.Empty)
                        {
                            double sum = Convert.ToDouble(excelHelper.GetCell(sheet, curRow, 5).Value) * 1000;
                            budgetLevel = excelHelper.GetCell(sheet, curRow, 4).Value;
                            GetSumsMapping(reportType, ref budgetLevel, sum, ref sumsMapping);
                        }
                        if (toPumpRow)
                        {
                            object[] mapping = new object[] { "RefRegions", regionRef, "RefBdgtLevels", budgetLevel,
                            "RefYearDayUNV", refPeriod, "REFMEANSTYPE", 1};
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, sumsMapping);
                            int sourceKey = Convert.ToInt32(kl.PadLeft(5, '0') + kst.PadLeft(5, '0'));
                            PumpReportRow(reportType, mapping, sourceKey, Convert.ToInt32(kl), Convert.ToInt32(kst));
                            zeroSums(ref sumsMapping);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("При обработке строки {0} отчета '{1}' возникла ошибка ({2})",
                            curRow, reportFile.Name, ex.Message), ex);
                    }
                UpdateData();
                WriteToTrace("Обработка отчета завершена: " + reportFile.Name, TraceMessageKind.Information);
            }
            finally
            {
                excelHelper.SetDisplayAlert(excelObj, false);
                excelHelper.CloseWorkBooks(excelObj);
                excelHelper.CloseExcel(ref excelObj);
                GC.GetTotalMemory(true);
            }
        }

        private bool CheckReportFileName(string fileName, ref string regionCode)
        {
            fileName = fileName.Remove(fileName.Length - 4);
            if (this.DataSource.Month != Convert.ToInt32(fileName.Substring(2, 2)))
                return false;
            regionCode = fileName.Substring(4);
            return true;
        }

        private void PumpReports(DirectoryInfo dir, string patternFileName)
        {
            FillSourceKeyCaches();
            try
            {
                FileInfo[] filesList = dir.GetFiles("mo*.XLS", SearchOption.AllDirectories);
                for (int i = 0; i < filesList.GetLength(0); i++)
                {
                    if (filesList[i].Name == patternFileName)
                        continue;
                    string regionCode = string.Empty;
                    if (!CheckReportFileName(filesList[i].Name, ref regionCode))
                        continue;
                    PumpReport(filesList[i], regionCode);
                }
            }
            finally
            {
                ClearSourceKeyCaches();
            }
        }

        #endregion функции закачки фактов

        #region общая организация закачки

        private bool CheckXLSFilesPresence(DirectoryInfo dir)
        {
            return (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) != 0);
        }

        private bool CheckRegion()
        {
            return (this.Region == RegionName.Krasnodar);
        }

        private bool CheckSourceDate()
        {
            return ((this.DataSource.Year == 2005) && (this.DataSource.Month <= 9));
        }

        private FileInfo GetPatternFile(DirectoryInfo dir)
        {
            string fileName = "MO" + this.DataSource.Year.ToString() + CommonRoutines.MonthByNumber[this.DataSource.Month - 1] + ".xls";
            FileInfo[] patternFile = dir.GetFiles(fileName, SearchOption.AllDirectories);
            if (patternFile.GetLength(0) == 0)
                throw new PumpDataFailedException("Отсутствует справочная информация.");
            return patternFile[0];
        }

        protected override void PumpXLSReports(DirectoryInfo dir)
        {
            if (!CheckXLSFilesPresence(dir))
                return;
            if (!CheckRegion())
                throw new PumpDataFailedException("Закачка месячной отчетности СКИФ формата XLS предназначена только для Краснодарского края."); 
            if (!CheckSourceDate())
                throw new PumpDataFailedException("Закачка месячной отчетности СКИФ формата XLS предназначена только для 2005 года по сентябрь включительно.");
            FileInfo patternFile = GetPatternFile(dir);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                PumpPattern(patternFile);
                PumpReports(dir, patternFile.Name);
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        #endregion общая организация закачки

    }

}
