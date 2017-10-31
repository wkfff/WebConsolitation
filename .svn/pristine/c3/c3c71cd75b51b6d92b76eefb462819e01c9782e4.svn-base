using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;
using Krista.FM.Client.Reports.Planning.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {

        private static DataTable ClearStaleRestSaratov(CommonDataObject cdo,  DataTable tblData, DateTime calcDate, int restIndex, int dateIndex)
        {
            foreach (DataRow rowData in tblData.Rows)
            {
                DateTime closeDate;
                
                if (DateTime.TryParse(Convert.ToString(rowData[dateIndex]), out closeDate))
                {
                    if (DateTime.Compare(calcDate, closeDate) <= 0)
                    {
                        rowData[restIndex] = DBNull.Value;
                    }
                }
            }

            return cdo.RecalcSummary(tblData);
        }

        private static string GetFilterStartDateYar(string startFieldName, string calcDate)
        {
            return String.Format("<='{0}' or {1} is null", calcDate, startFieldName);
        }

        private static string GetFilterEndDateYar(string endFieldName, string renewalFieldName, string calcDate)
        {
            return String.Format(">='{0}' or c.{2}>='{0}' or ({1} is null and {2} is null) ", 
                GetYearStart(calcDate), endFieldName, renewalFieldName);
        }

        private static DataTable CorrectDetailDateText(DataTable tblData, int sourceIndex, int destIndex)
        {
            const string splitter = ",";
            foreach (DataRow rowCap in tblData.Rows)
            {
                if (rowCap[sourceIndex] == DBNull.Value)
                {
                    continue;
                }

                var planPayParts = Convert.ToString(rowCap[sourceIndex]).Split(splitter[0]);
                var fullText = (from datePart in planPayParts
                                where datePart.Length > 6
                                select datePart.Trim().Remove(datePart.Trim().Length - 5, 5))
                                .Aggregate(String.Empty, (current, strDayMonth) => 
                                    Combine(current, "до " + strDayMonth, splitter));

                rowCap[destIndex] = fullText.Trim(splitter[0]);
            }

            return tblData;
        }

        private static DataTable ClearCloseCreditMO(
            CommonDataObject cdo,
            DataTable tblCredit, 
            DateTime calcDate, 
            int restIndex, 
            int dateIndex, 
            int positionColumnIndex)
        {
            var deletedRows = new Collection<DataRow>();
            foreach (DataRow rowData in tblCredit.Rows)
            {
                var dateText = Convert.ToString(rowData[dateIndex]);

                if (cdo.textAppendix.Length > 0)
                {
                    dateText = dateText.Replace(cdo.textAppendix, String.Empty);
                }

                if (dateText.Length <= 0 || cdo.valuesSeparator.Length <= 0)
                {
                    continue;
                }

                var dateList = dateText.Split(cdo.valuesSeparator[0]);

                if (GetNumber(rowData[restIndex]) < (decimal)0.001 && dateList.Length > 0)
                {
                    deletedRows.Add(rowData);
                    var debtDate = Convert.ToDateTime(dateList[dateList.Length - 1]);
                    var nextMonth = debtDate.AddMonths(1);
                    if ((debtDate.Month == calcDate.Month && debtDate.Year == calcDate.Year) ||
                        (calcDate.Day == 1 && nextMonth.Month == calcDate.Month && nextMonth.Year == calcDate.Year))
                    {
                        deletedRows.Remove(rowData);
                    }
                }
            }

            foreach (var deletedRow in deletedRows)
            {
                tblCredit.Rows.Remove(deletedRow);
            }

            if (positionColumnIndex >= 0)
            {
                var rowCounter = 1;

                foreach (DataRow row in tblCredit.Rows)
                {
                    row[positionColumnIndex] = rowCounter++;
                }
            }

            return tblCredit;
        }

        private static DataTable FillGarantSumMO(DataTable dtGarant,
            int columnIndexRub,
            int columnIndexVal,
            decimal exRate, 
            int refOkvIndex, 
            int sumIndex, 
            int currencySumIndex,
            int factCounterIndex)
        {
            for (var i = 0; i < dtGarant.Rows.Count - 1; i++)
            {
                var drData = dtGarant.Rows[i];
                var refOKV = Convert.ToInt32(drData[refOkvIndex]);
                // сумма по гарантируемому договору
                var contractSum = GetNumber(drData[sumIndex]);

                if (refOKV != ReportConsts.codeRUB)
                {
                    contractSum = GetNumber(drData[currencySumIndex]);
                }

                // если не заполнен План погашения основного долга, берем значение из суммы по гарантируемому договору
                var emptyPlanDebt = GetNumber(drData[factCounterIndex]) == 0;

                if (emptyPlanDebt)
                {
                    drData[columnIndexRub] = GetNumber(drData[columnIndexRub]) + contractSum;
                    drData[columnIndexVal] = GetNumber(drData[columnIndexVal]) + contractSum;
                }

                // для валютных пересчитываем по указанному в параметрах курсу
                if (refOKV != ReportConsts.codeRUB)
                {
                    drData[columnIndexRub] = Math.Round(exRate * GetNumber(drData[columnIndexVal]));
                }
            }

            return dtGarant;
        }

        private DataTable CombineCurrencySumYar(DataTable tblData, int baseRubIndex, int baseValIndex, int fieldCount)
        {
            var tblClone = CreateReportCaptionTable(tblData.Columns.Count);

            foreach (DataRow rowCredit in tblData.Rows)
            {
                tblClone.ImportRow(rowCredit);
                var rowData = GetLastRow(tblClone);

                for (var i = 0; i < rowCredit.Table.Columns.Count; i++)
                {
                    rowData[i] = rowCredit[i];
                }

                if (rowCredit[f_S_Creditincome.RefOKV] == DBNull.Value)
                {
                    continue;
                }

                var refOkv = Convert.ToInt32(rowCredit[f_S_Creditincome.RefOKV]);

                if (refOkv == ReportConsts.codeRUB)
                {
                    continue;
                }

                for (var i = 0; i < fieldCount; i++)
                {
                    var rubSum = GetNumber(rowCredit[baseRubIndex + i]);
                    var valSum = GetNumber(rowCredit[baseValIndex + i]);
                            
                    if (Math.Abs(valSum) > 0)
                    {
                        rowData[baseRubIndex + i] = String.Format("{0:N2} {1:N2}EUR", rubSum, valSum);
                    }
                }
            }

            return tblClone;
        }

        public enum SpreadDebtBookEnum
        {
            YaroslavlObl,
            SamaraObl
        }

        private string GetYarDKEndContractDate(CommonDataObject cdo, int dtlIndex, int key, string fldDateName)
        {
            var endPlanDate = String.Empty;
            var tblPlanDebt = DataTableUtils.FilterDataSet(
                cdo.dtDetail[dtlIndex],
                String.Format("{0} = {1}", cdo.GetParentRefName(), key));

            tblPlanDebt = DataTableUtils.SortDataSet(tblPlanDebt, StrSortUp(fldDateName));

            if (tblPlanDebt.Rows.Count > 0)
            {
                var cellDate = GetLastRow(tblPlanDebt)[fldDateName];

                if (cellDate != DBNull.Value)
                {
                    endPlanDate = Convert.ToDateTime(cellDate).ToShortDateString();

                    if (tblPlanDebt.Rows.Count > 1)
                    {
                        cellDate = GetFirstRow(tblPlanDebt)[fldDateName];
                        var startPlanDate = String.Empty;

                        if (cellDate != DBNull.Value)
                        {
                            startPlanDate = Convert.ToDateTime(cellDate).ToShortDateString();
                        }

                        endPlanDate = String.Format("{0}-{1}", startPlanDate, endPlanDate);
                    }
                }
            }

            return endPlanDate;
        }

        private int SpreadChangesDataYar(
            CommonDataObject cdo,
            DataTable tblJournalCB,
            ReportDocumentType docType,
            ref DataTable tblResult,
            DataTable tblContract,
            Dictionary<int, string> detailList,
            string loDate,
            string hiDate,
            SpreadDebtBookEnum region = SpreadDebtBookEnum.YaroslavlObl)
        {
            const int counterServiceField = 100;
            const int payOrderField = counterServiceField - 5;
            tblResult = CreateReportCaptionTable(counterServiceField + 1);
            var dateList = new Collection<string>();
            var parentField = cdo.GetParentRefName();
            var contractCount = 0;

            var crdAttrKey = Convert.ToInt32(t_S_FactAttractCI.key);
            var crdDebtKey = Convert.ToInt32(t_S_FactDebtCI.key);
            var crdRateKey = Convert.ToInt32(t_S_RateSwitchCI.key);
            var crdPctFactKey = Convert.ToInt32(t_S_FactPercentCI.key);

            var capPlanAttrKey = Convert.ToInt32(t_S_CPFactCapital.key);
            var capFactKey = Convert.ToInt32(t_S_CPFactDebt.key);

            var capPctFactKey = Convert.ToInt32(t_S_CPFactService.key);

            const string rubField = ReportConsts.SumField;
            const string valField = ReportConsts.CurrencySumField;
            const string chargeField = t_S_FactPercentCI.ChargeSum;

            var capPlanDebt = Convert.ToInt32(t_S_CPPlanDebt.key);

            var emptyRows = new Collection<DataRow>();

            foreach (DataRow rowContract in tblContract.Rows)
            {
                var totalRubSum = new decimal[6];
                var totalValSum = new decimal[6];

                var masterKey = Convert.ToInt32(rowContract[f_S_Creditincome.id]);
                var refOkv = Convert.ToInt32(rowContract[f_S_Creditincome.RefOKV]);
                var isRubContract = refOkv == ReportConsts.codeRUB;
                var okvCode = String.Empty;

                if (!isRubContract)
                {
                    okvCode = Convert.ToString(rowContract[TempFieldNames.OKVShortName]);
                }

                dateList.Clear();

                foreach (var detailInfo in detailList)
                {
                    var detailIndex = detailInfo.Key;
                    var fields = detailInfo.Value.Split(';');
                    var tblDetail = cdo.dtDetail[detailIndex];

                    if (fields.Length > 1)
                    {
                        for (var f = 2; f < fields.Length; f++)
                        {
                            var filterStr = String.Format("{0} = {1}", fields[1], fields[f]);
                            tblDetail = DataTableUtils.FilterDataSet(tblDetail, filterStr);
                            dateList = FillDetailDateList(dateList, tblDetail, fields[0], loDate, hiDate, parentField,
                                                          masterKey);
                            tblDetail = cdo.dtDetail[detailIndex];
                        }
                    }
                    else
                    {
                        dateList = FillDetailDateList(dateList, tblDetail, fields[0], loDate, hiDate, parentField,
                                                      masterKey);
                    }
                }

                var hasNonZeroSum = isRubContract ? 
                    GetNumber(rowContract[6]) != 0 :
                    GetNumber(rowContract[8]) != 0;

                if (dateList.Count <= 0 && !hasNonZeroSum)
                {
                    continue;
                }

                contractCount++;
                var rowCaption = tblResult.Rows.Add();
                rowCaption[counterServiceField] = contractCount;

                if (docType == ReportDocumentType.CreditOrg || docType == ReportDocumentType.CreditBud)
                {
                    var endDate = String.Empty;

                    if (rowContract[3] != DBNull.Value)
                    {
                        endDate = Convert.ToDateTime(rowContract[3]).ToShortDateString();
                    }

                    var planEndDate = GetYarDKEndContractDate(
                        cdo,
                        Convert.ToInt32(t_S_PlanDebtCI.key),
                        masterKey,
                        t_S_PlanDebtCI.EndDate);

                    if (planEndDate.Length > 0)
                    {
                        endDate = planEndDate;
                    }

                    rowCaption[0] = String.Format("{0} {1} Дата погашения: {2} {3}",
                                                  rowContract[0],
                                                  rowContract[1],
                                                  endDate,
                                                  rowContract[4]);

                    if (region == SpreadDebtBookEnum.SamaraObl)
                    {
                        rowCaption[0] = String.Format("{0} {1} {2}",
                                                      rowCaption[0],
                                                      rowContract[f_S_Creditincome.Sum],
                                                      rowContract[f_S_Creditincome.Purpose]);
                    }
                }
                else
                {
                    var endPlanDate = String.Empty;

                    if (rowContract[3] != DBNull.Value)
                    {
                        endPlanDate = Convert.ToDateTime(rowContract[3]).ToShortDateString();
                    }

                    var planEndDate = GetYarDKEndContractDate(
                        cdo,
                        Convert.ToInt32(t_S_CPPlanDebt.key),
                        masterKey,
                        t_S_CPPlanDebt.EndDate);

                    if (planEndDate.Length > 0)
                    {
                        endPlanDate = planEndDate;
                    }

                    var contractDate = String.Empty;

                    if (rowContract[1] != DBNull.Value)
                    {
                        contractDate = Convert.ToDateTime(rowContract[1]).ToShortDateString();
                    }

                    rowCaption[0] = String.Format("Договор {0} от {1} {2} Дата погашения {3}",
                                                  rowContract[0],
                                                  contractDate,
                                                  rowContract[2],
                                                  endPlanDate);

                    if (region == SpreadDebtBookEnum.SamaraObl)
                    {
                        rowCaption[0] = String.Format("{0} {1} {2}",
                                                      rowCaption[0],
                                                      rowContract[f_S_Capital.Sum],
                                                      rowContract[f_S_Capital.Purpose]);
                    }
                }

                var rowYearStart = tblResult.Rows.Add();

                if (isRubContract)
                {
                    rowYearStart[1] = rowContract[6];
                }
                else
                {
                    var currencySum = GetNumber(rowContract[8]);
                    var rubSum = GetNumber(rowContract[6]);

                    if (currencySum != 0 && rubSum != 0)
                    {
                        rowYearStart[1] = String.Format("{0:N2} {1:N2} {2}", rubSum, currencySum, okvCode);
                    }
                }

                rowYearStart[4] = rowYearStart[1];
                rowYearStart[counterServiceField] = contractCount;

                for (var d = 0; d < dateList.Count; d++)
                {
                    var prevRow = GetLastRow(tblResult);
                    var rowChange = tblResult.Rows.Add();
                    var curDate = dateList[d];
                    rowChange[0] = curDate;
                    rowChange[counterServiceField] = contractCount;
                    var changeDate = Convert.ToDateTime(curDate);
                    // ставка
                    rowChange[06] = CalcActualPercent(cdo, masterKey, curDate);

                    if (docType == ReportDocumentType.CreditOrg)
                    {
                        var filterCBStr = String.Format("{0} <= '{1}'", d_S_JournalCB.InputDate, curDate);
                        var sortCBStr = StrSortDown(d_S_JournalCB.InputDate);
                        var tblCBPercent = DataTableUtils.FilterDataSet(tblJournalCB, filterCBStr);
                        tblCBPercent = DataTableUtils.SortDataSet(tblCBPercent, sortCBStr);
                        var rowCBPercent = GetFirstRow(tblCBPercent);

                        if (rowCBPercent != null)
                        {
                            var dayCount = 365;
                            if (DateTime.IsLeapYear(changeDate.Year))
                            {
                                dayCount++;
                            }

                            var percentPeniRate = GetNumber(rowCBPercent[d_S_JournalCB.PercentRate]);
                            var multiplier = GetNumber(rowContract[5]);

                            if (multiplier > 0 && multiplier < 1)
                            {
                                var divider = 1;

                                while ((decimal)1 / divider > multiplier)
                                {
                                    divider++;
                                }

                                multiplier = (decimal)1/(divider - 1);
                            }

                            rowChange[12] = Math.Round(dayCount*multiplier*percentPeniRate, 4);
                        }
                    }

                    if (docType == ReportDocumentType.CreditOrg || docType == ReportDocumentType.CreditBud)
                    {
                        var templateSum = "{0}";
                        const string templateRate = "{0}";

                        if (!isRubContract)
                        {
                            templateSum = "{0:N2} {1:N2} {2}";
                        }

                        var tblSourceRate = cdo.dtDetail[crdRateKey];
                        cdo.dtDetail[crdRateKey] = DataTableUtils.FilterDataSet(tblSourceRate,
                                                                                String.Format("{0} = 1",
                                                                                              t_S_RateSwitchCI.RefTypeSum));
                        // рублевые суммы ОД
                        var rubAttr = CalcDetailSumValue(cdo, crdAttrKey, masterKey, changeDate, rubField, isRubContract);
                        CombineCellValues(rowChange, payOrderField, cdo.sumIncludedRows, t_S_FactAttractCI.NumPayOrder);
                        var rubDebt = CalcDetailSumValue(cdo, crdDebtKey, masterKey, changeDate, rubField, isRubContract);
                        CombineCellValues(rowChange, payOrderField, cdo.sumIncludedRows, t_S_FactDebtCI.NumPayOrder);
                        var rubRate = CalcDetailSumValue(cdo, crdRateKey, masterKey, changeDate, rubField, isRubContract);
                        // валютные суммы ОД
                        var valAttr = CalcDetailSumValue(cdo, crdAttrKey, masterKey, changeDate, valField, isRubContract);
                        var valDebt = CalcDetailSumValue(cdo, crdDebtKey, masterKey, changeDate, valField, isRubContract);

                        // полный текст ОД
                        if (isRubContract && rubAttr != 0 || !isRubContract && valAttr != 0)
                        {
                            rowChange[02] = String.Format(templateSum, rubAttr, valAttr, okvCode);

                            totalRubSum[0] += rubAttr;
                            totalValSum[0] += valAttr;
                        }

                        if (isRubContract && rubDebt != 0 || !isRubContract && valDebt != 0)
                        {
                            rowChange[03] = String.Format(templateSum, rubDebt, valDebt, okvCode);

                            totalRubSum[1] += rubDebt;
                            totalValSum[1] += valDebt;
                        }

                        rowChange[05] = String.Format(templateRate, rubRate);
                        // рублевые суммы процентов
                        cdo.dtDetail[crdRateKey] =
                            DataTableUtils.FilterDataSet(tblSourceRate,
                                                         String.Format("{0} = 2", t_S_RateSwitchCI.RefTypeSum));

                        var rubPctAttr = CalcDetailSumValue(cdo, crdPctFactKey, masterKey, changeDate, chargeField,
                                                            isRubContract);
                        var rubPctDebt = CalcDetailSumValue(cdo, crdPctFactKey, masterKey, changeDate, rubField,
                                                            isRubContract);
                        var rubPctRate = CalcDetailSumValue(cdo, crdRateKey, masterKey, changeDate, rubField,
                                                            isRubContract);
                        // валютные суммы процентов
                        var valPctAttr = CalcDetailSumValue(cdo, crdPctFactKey, masterKey, changeDate, chargeField,
                                                            isRubContract);
                        var valPctDebt = CalcDetailSumValue(cdo, crdPctFactKey, masterKey, changeDate, valField,
                                                            isRubContract);

                        // полный текст процентов
                        if (isRubContract && rubPctAttr != 0 || !isRubContract && valPctAttr != 0)
                        {
                            rowChange[08] = String.Format(templateSum, rubPctAttr, valPctAttr, okvCode);

                            totalRubSum[2] += rubPctAttr;
                            totalValSum[2] += valPctAttr;
                        }

                        if (isRubContract && rubPctDebt != 0 || !isRubContract && valPctDebt != 0)
                        {
                            rowChange[09] = String.Format(templateSum, rubPctDebt, valPctDebt, okvCode);

                            totalRubSum[3] += rubPctDebt;
                            totalValSum[4] += valPctDebt;
                        }

                        rowChange[11] = String.Format(templateRate, rubPctRate);
                        cdo.dtDetail[crdRateKey] = tblSourceRate;
                    }
                    else
                    {
                        var nominal = GetNumber(rowContract[f_S_Capital.Nominal]);
                        // ОД
                        var tblDetailSource = cdo.dtDetail[capPctFactKey];
                        var sumCapAttr = CalcDetailSumValue(cdo, capPlanAttrKey, masterKey, changeDate,
                                                            t_S_CPFactCapital.Quantity, isRubContract);
                        CombineCellValues(rowChange, payOrderField, cdo.sumIncludedRows, t_S_CPFactCapital.NumPayOrder);

                        var sumCapFact = CalcDetailSumValue(cdo, capFactKey, masterKey, changeDate,
                                                            t_S_CPFactDebt.Quantity, isRubContract);
                        CombineCellValues(rowChange, payOrderField, cdo.sumIncludedRows, t_S_CPFactDebt.NumPayOrder);
                        rowChange[02] = nominal * sumCapAttr;
                        rowChange[03] = nominal * sumCapFact;
                        // Проценты
                        var filterCapPct = String.Format("{0} = 8", t_S_CPFactService.RefTypeSum);
                        cdo.dtDetail[capPctFactKey] = DataTableUtils.FilterDataSet(tblDetailSource, filterCapPct);
                        rowChange[08] = CalcDetailSumValue(cdo, capPctFactKey, masterKey, changeDate,
                                                           t_S_CPFactService.ChargeSum, isRubContract);
                        rowChange[09] = CalcDetailSumValue(cdo, capPctFactKey, masterKey, changeDate, rubField,
                                                           isRubContract);
                        cdo.dtDetail[capPctFactKey] = tblDetailSource;
                    }

                    rowChange[1] = prevRow[4];

                    if (!isRubContract)
                    {
                        var sumStart = ParseCurrencySum(rowChange[1]);
                        var sumAttr = ParseCurrencySum(rowChange[2]);
                        var sumDebt = ParseCurrencySum(rowChange[3]);
                        var sumRate = GetNumber(rowChange[5]);

                        var rubSumTotal = sumStart[0] + sumAttr[0] - sumDebt[0] + sumRate;
                        var valSumTotal = sumStart[1] + sumAttr[1] - sumDebt[1];

                        if (Math.Abs(valSumTotal) > 0)
                        {
                            rowChange[4] = String.Format("{0:N2} {1:N2} {2}", rubSumTotal, valSumTotal, okvCode);
                        }
                    }
                    else
                    {
                        rowChange[4] = GetNumber(rowChange[1]) + GetNumber(rowChange[2]) - GetNumber(rowChange[3]) +
                                       GetNumber(rowChange[5]);
                    }

                    // планы с фактами по процентам разошлись
                    var planPctSum = GetNumber(rowChange[8]);
                    var factPctSum = GetNumber(rowChange[9]);

                    if (factPctSum != planPctSum)
                    {
                        var rowFound = false;
                        var lastRowIndex = GetLastRowIndex(tblResult);
                        var curRowOffset = 1;

                        for (var k = d - 1; k >= 0; k--)
                        {
                            if (rowFound)
                            {
                                continue;
                            }

                            var rowFind = tblResult.Rows[lastRowIndex - curRowOffset];
                            var planSum = GetNumber(rowFind[8]);
                            var factSum = GetNumber(rowFind[9]);

                            if (factPctSum == 0)
                            {
                                rowFound = factSum == planPctSum && planSum == 0;
                                if (rowFound)
                                {
                                    rowFind[8] = planPctSum;
                                    rowChange[8] = 0;
                                }
                            }
                            else
                            {
                                rowFound = planSum == factPctSum && factSum == 0;
                                if (rowFound)
                                {
                                    rowFind[8] = 0;
                                    rowChange[8] = planSum;
                                }
                            }

                            curRowOffset++;
                        }
                    }
                }

                var rowSummary = tblResult.Rows.Add();
                rowSummary[counterServiceField - 0] = contractCount;
                rowSummary[counterServiceField - 1] = rowContract[5];

                // подчистим расхождение начисление и погашений по процентам
                var lastRowNum = GetLastRowIndex(tblResult);
                var curRowNum = 1;

                foreach (var t in dateList)
                {
                    var rowChange = tblResult.Rows[lastRowNum - curRowNum++];
                    var incPctSum = GetNumber(rowChange[8]);
                    var decPctSum = GetNumber(rowChange[9]);

                    if (Math.Abs(decPctSum) == 0)
                    {
                        rowChange[8] = 0;
                        totalRubSum[2] -= incPctSum;
                    }
                    else
                    {
                        rowChange[8] = decPctSum;
                        totalRubSum[2] += decPctSum;
                    }

                    if (CheckDebtorEmptySum(rowChange, isRubContract))
                    {
                        emptyRows.Add(rowChange);
                    }
                }

                if (!isRubContract)
                {
                    const string templateVal = "{0:N2} {1:N2} {2}";
                    var columnIndexes = new Dictionary<int, int> {{2, 0}, {3, 1}, {8, 2}, {9, 3}};

                    foreach (var pair in columnIndexes.Where(pair => totalValSum[pair.Value] != 0))
                    {
                        rowSummary[pair.Key] = String.Format(
                            templateVal,
                            totalRubSum[pair.Value],
                            totalValSum[pair.Value],
                            okvCode);
                    }
                }

                rowYearStart[4] = DBNull.Value;
                rowYearStart[6] = CalcActualPercent(cdo, masterKey, GetYearStart(loDate));
            }

            foreach (var emptyRow in emptyRows)
            {
                tblResult.Rows.Remove(emptyRow);
            }


            if (region == SpreadDebtBookEnum.SamaraObl)
            {
                var tblSamaraResult = CreateReportCaptionTable(tblResult.Columns.Count);

                foreach (DataRow rowData in tblResult.Rows)
                {
                    var rowResult = tblSamaraResult.Rows.Add();

                    for (var i = 1; i < counterServiceField; i++)
                    {
                        rowResult[i + 1] = rowData[i];
                    }

                    rowResult[0] = rowData[0];
                    
                    if (rowData[payOrderField] != DBNull.Value)
                    {
                        rowResult[1] = Convert.ToString(rowData[payOrderField]).Replace("\\", String.Empty);
                    }

                    rowResult[counterServiceField] = rowData[counterServiceField];
                    rowResult[counterServiceField - 1] = rowData[counterServiceField - 1];
                }

                tblResult = tblSamaraResult;
            }

            return contractCount;
        }

        private static bool CheckDebtorEmptySum(DataRow rowData, bool isRubContract)
        {
            var isEmptyRow =
                GetNumber(rowData[9]) == 0 &&
                GetNumber(rowData[8]) == 0 &&
                GetNumber(rowData[2]) == 0 &&
                GetNumber(rowData[3]) == 0 &&
                GetNumber(rowData[5]) == 0;

            if (!isRubContract)
            {
                var pctDebt = ParseCurrencySum(rowData[9]);
                var pctAttr = ParseCurrencySum(rowData[8]);
                var dbtAttr = ParseCurrencySum(rowData[2]);
                var dbtDebt = ParseCurrencySum(rowData[3]);

                isEmptyRow = GetNumber(pctDebt[1]) == 0 &&
                             GetNumber(pctAttr[1]) == 0 &&
                             GetNumber(dbtAttr[1]) == 0 &&
                             GetNumber(dbtDebt[1]) == 0 &&
                             GetNumber(rowData[5]) == 0;
            }

            return isEmptyRow;
        }


        private static decimal[] ParseCurrencySum(object cellValue)
        {
            var result = new decimal[] { 0, 0 };
            var textValue = Convert.ToString(cellValue);

            if (textValue.Length > 0)
            {
                var parts = textValue.Split(' ');
                
                if (parts.Length > 0)
                {
                    result[0] = GetNumber(parts[0]);
                }
                
                if (parts.Length > 1)
                {
                    result[1] = GetNumber(parts[1]);
                }
            }
            return result;
        }

        private static decimal CalcDetailSumValue(
            CommonDataObject cdo, 
            int detailIndex, 
            int masterId, 
            DateTime changeDate,
            string sumField, 
            bool isRubContract)
        {
            var tblDetail = cdo.dtDetail[detailIndex];
            var factDate = cdo.GetEndDates()[detailIndex];

            if (cdo is CreditDataObject && detailIndex == Convert.ToInt32(t_S_PlanServiceCI.key))
            {
                factDate = t_S_PlanServiceCI.PaymentDate;
            }

            var sumValue = cdo.GetSumValue(tblDetail, masterId, factDate, sumField, changeDate, changeDate, true, true);

            if (!isRubContract 
                && String.Compare(sumField, ReportConsts.SumField, StringComparison.CurrentCultureIgnoreCase) == 0 
                && tblDetail.Columns.Contains(ReportConsts.CurrencySumField))
            {
                sumValue = (from sumRow in cdo.sumIncludedRows
                            let sumValPart = GetNumber(sumRow[ReportConsts.CurrencySumField])
                            let sumRubPart = GetNumber(sumRow[sumField])
                            where Math.Abs(sumValPart) > 0
                            select sumRubPart).Sum();
            }

            return sumValue;
        }

        private static DataRow[] GetActualPercentRows(CommonDataObject cdo, int masterId, string changeDate)
        {
            var refParent = cdo.GetParentRefName();
            const string journalDate = t_S_JournalPercentCI.ChargeDate;
            var filterStr = String.Format("{0} = {1} and {2} <= '{3}'", refParent, masterId, journalDate, changeDate);
            var orderStr = StrSortDown(journalDate);
            return cdo.dtJournalPercent.Select(filterStr, orderStr);            
        }

        private static object CalcActualPercent(CommonDataObject cdo, int masterId, string changeDate)
        {
            var journalRecords = GetActualPercentRows(cdo, masterId, changeDate);
            return journalRecords.Length > 0 ? journalRecords[0][t_S_JournalPercentCI.CreditPercent] : null;
        }

        private static object CalcMinimalPercent(CommonDataObject cdo, int masterId, string changeDate)
        {
            var journalRecords = GetActualPercentRows(cdo, masterId, changeDate);

            return journalRecords.Length > 0 ? journalRecords[journalRecords.Length - 1][t_S_JournalPercentCI.CreditPercent] : null;
        }

        private DataTable CalcYearSummary(DataTable tblData)
        {
            var tblResult = CreateReportCaptionTable(16, 14);
            var rowYearStart = GetFirstRow(tblResult);
            var rowYearEnd = GetLastRow(tblResult);
            var summaryRow = GetLastRow(tblData);

            rowYearStart[01] = summaryRow[0];
            rowYearStart[07] = summaryRow[1];
            rowYearStart[14] = summaryRow[2];

            rowYearEnd[04] = summaryRow[4];
            rowYearEnd[10] = summaryRow[5];

            const int captionColumns = 6;
            const int groupColumns = 15;

            for (var i = 1; i < 13; i++)
            {
                var rowMonth = tblResult.Rows[i];
                var baseOffset = captionColumns + groupColumns * (i - 1);

                for (var k = 0; k < 15; k++)
                {
                    rowMonth[k + 1] = summaryRow[baseOffset + k];
                }

                rowMonth[8] = rowMonth[9];
            }

            return tblResult;
        }

        private static string[] CreateChangeListYar(
            CommonDataObject cdo, 
            ReportDocumentType docType,
            DataTable tblChanges, 
            bool nearestPercent,
            string templateSingle, 
            string templateMulti, 
            bool multiChanges,
            decimal exchangeRate)
        {
            var templateStr = templateSingle;

            if (multiChanges)
            {
                templateStr = templateMulti;
            }

            const string splitter = ";";
            var lastPctText = String.Empty;
            var changeList = String.Empty;
            var percentList = String.Empty;

            foreach (DataRow rowChangeInfo in tblChanges.Rows)
            {
                var sumValue = ConvertTo1000(GetNumber(rowChangeInfo[4]));

                if (Convert.ToInt32(rowChangeInfo[7]) != ReportConsts.codeRUB)
                {
                    sumValue = ConvertTo1000(exchangeRate * GetNumber(rowChangeInfo[8]));
                }

                changeList = Combine(changeList, String.Format(templateStr, rowChangeInfo[3], sumValue), splitter);
                var curDate = Convert.ToDateTime(rowChangeInfo[2]).ToShortDateString();

                var masterId = Convert.ToInt32(rowChangeInfo[6]);

                var pctCurText = CalcActualPercent(cdo, masterId, curDate);
                object pctMinText;

                if (docType == ReportDocumentType.Capital && !nearestPercent)
                {
                    pctCurText = CalcActualPercent(cdo, masterId, DateTime.MaxValue.ToShortDateString());
                    pctMinText = CalcMinimalPercent(cdo, masterId, DateTime.MaxValue.ToShortDateString());

                    if (GetNumber(pctMinText) == 0)
                    {
                        pctMinText = pctCurText;
                    }
                }
                else
                {
                    pctMinText = pctCurText;                    
                }

                if (GetNumber(pctCurText) == 0 && GetNumber(pctMinText) == 0)
                {
                    pctCurText = rowChangeInfo[5];
                    pctMinText = rowChangeInfo[5];
                }

                var pct1Text = String.Empty;
                var pct2Text = String.Empty;

                if (pctMinText != null)
                {
                    pct1Text = pctMinText.ToString().Trim('0').Trim('.').Trim(',');
                }
                
                if (pctCurText != null)
                {
                    pct2Text = pctCurText.ToString().Trim('0').Trim('.').Trim(',');
                }

                if (GetNumber(pctCurText) != GetNumber(pctMinText))
                {
                    var pctFullText = String.Format("{0}-{1}%", pct1Text, pct2Text);

                    if (lastPctText != pctFullText)
                    {
                        percentList = Combine(percentList, pctFullText, splitter);
                        lastPctText = pctFullText;
                    }
                }
                else
                {
                    if (GetNumber(pctCurText) != 0)
                    {
                        var pctFullText = String.Format("{0}%", pct2Text);

                        if (lastPctText != pctFullText)
                        {
                            percentList = Combine(percentList, pctFullText, splitter);
                            lastPctText = pctFullText;
                        }
                    }
                }

            }

            return new [] { changeList.Trim(';').Trim(), percentList.Trim(splitter[0]).Trim() };
        }

        private string[] CombineDetailTextYar(
            CommonDataObject cdo,
            DataTable tblContracts,
            ReportDocumentType docType,
            bool nearestPercent, 
            int detailIndex,
            string dateFieldName,
            string loDate,
            string hiDate,
            decimal exchangeRate)
        {
            var resultDateInfo = String.Empty;
            var resultPctInfo = String.Empty;
            const string templateSingleDate = " {0}";
            const string templateMultiDate = " {0} - {1:N2}";
            const string strSplitter = ";";
            const char chrSplitter = ';';
            var hasMultiChanges = false;
            var hasData = false;

            var orgList = new Collection<string>();
            var tblChangeInfo = CreateReportCaptionTable(10);

            for (var i = 0; i < tblContracts.Rows.Count - 1; i++)
            {
                var sourceRow = tblContracts.Rows[i];

                var selectStr = String.Format("{0}>='{3}' and {0}<='{4}' and {1} = {2}",
                                                 dateFieldName,
                                                 cdo.GetParentRefName(),
                                                 sourceRow[f_S_Creditincome.id],
                                                 loDate,
                                                 hiDate);

                var rowsSelect = cdo.dtDetail[detailIndex].Select(selectStr, StrSortUp(dateFieldName));

                hasMultiChanges = hasMultiChanges || rowsSelect.Length > 1 || hasData && rowsSelect.Length > 0;

                hasData = hasData || rowsSelect.Length > 0;

                foreach (var dataRow in rowsSelect)
                {
                    var destRow = tblChangeInfo.Rows.Add();
                    destRow[0] = sourceRow[f_S_Creditincome.id];

                    if (docType == ReportDocumentType.CreditOrg)
                    {
                        var orgName = Convert.ToString(sourceRow[7]);
                        destRow[1] = orgName;

                        if (!orgList.Contains(orgName))
                        {
                            orgList.Add(orgName);
                        }
                    }

                    destRow[2] = dataRow[dateFieldName];
                    destRow[3] = GetDateDayMonth(dataRow[dateFieldName]);
                    destRow[4] = GetNumber(dataRow[ReportConsts.SumField]);
                    destRow[5] = sourceRow[6];
                    destRow[6] = sourceRow[f_S_Creditincome.id];
                    destRow[7] = sourceRow[f_S_Creditincome.RefOKV];

                    if (dataRow.Table.Columns.Contains(ReportConsts.CurrencySumField))
                    {
                        destRow[8] = GetNumber(dataRow[ReportConsts.CurrencySumField]);
                    }
                }
            }

            if (docType == ReportDocumentType.CreditOrg)
            {
                var filterColumnName = tblChangeInfo.Columns[1].ColumnName;
                var sortColumnName = tblChangeInfo.Columns[2].ColumnName;

                foreach (var orgName in orgList)
                {
                    var selectStr = String.Format("{0} = '{1}'", filterColumnName, orgName);
                    var tblSingleOrgInfo = DataTableUtils.FilterDataSet(tblChangeInfo, selectStr);
                    tblSingleOrgInfo = DataTableUtils.SortDataSet(tblSingleOrgInfo, StrSortUp(sortColumnName));

                    var changeInfo = CreateChangeListYar(
                        cdo,
                        docType, 
                        tblSingleOrgInfo, 
                        nearestPercent,
                        templateSingleDate, 
                        templateMultiDate, 
                        hasMultiChanges,
                        exchangeRate);

                    resultDateInfo = Combine(resultDateInfo, String.Format(" {0} ({1})", orgName, changeInfo[0]), strSplitter);
                    resultPctInfo = Combine(resultPctInfo, changeInfo[1], strSplitter);
                }
            }
            else
            {
                var filterColumnName = tblChangeInfo.Columns[0].ColumnName;

                for (var i = 0; i < tblContracts.Rows.Count - 1; i++)
                {
                    var sourceRow = tblContracts.Rows[i];
                    var selectStr = String.Format("{0} = '{1}'", filterColumnName, sourceRow[f_S_Creditincome.id]);
                    var tblSingleContractInfo = DataTableUtils.FilterDataSet(tblChangeInfo, selectStr);

                    var changeInfo = CreateChangeListYar(
                        cdo, 
                        docType,
                        tblSingleContractInfo, 
                        nearestPercent,
                        templateSingleDate, 
                        templateMultiDate, 
                        hasMultiChanges,
                        exchangeRate);

                    if (changeInfo[0].Length > 0)
                    {
                        resultDateInfo = Combine(resultDateInfo, String.Format(" {0}", changeInfo[0]), strSplitter);
                    }

                    if (changeInfo[1].Length > 0)
                    {
                        resultPctInfo = Combine(resultPctInfo, changeInfo[1], strSplitter);
                    }
                }
            }

            return new [] { resultDateInfo.Trim(chrSplitter).Trim(), resultPctInfo.Trim(chrSplitter).Trim() };
        }

        private static void CombineCellValues(
            DataRow rowDest, 
            int destColumn, 
            IEnumerable<DataRow> rowsSrc, 
            string srcFieldName, 
            bool singleValue = false)
        {
            const char splitter = '\\';

            foreach (var rowData in rowsSrc)
            {
                var insertValue = rowData[srcFieldName];
                DateTime testValue;

                if (DateTime.TryParse(Convert.ToString(insertValue), out testValue))
                {
                    insertValue = testValue.ToShortDateString();
                }

                if (singleValue || rowDest[destColumn] == DBNull.Value)
                {
                    rowDest[destColumn] = insertValue;
                }
                else
                {
                    var comboStr = String.Format("{0} {1} {2}", rowDest[destColumn], splitter, insertValue);
                    rowDest[destColumn] = comboStr.Trim().Trim(splitter);
                }
            }
        }

        private static string GetSelectedContractId(Dictionary<string, string> reportParams)
        {
            var paramValue = reportParams[ReportConsts.ParamRegNum];
            
            if (paramValue.Length > 0 && paramValue != ReportConsts.UndefinedKey)
            {
                return paramValue.Remove(0, 1);
            }

            return paramValue;
        }

        private static string GetMOSelectedRegNumFilter(string regNum)
        {
            return String.Format("='{0}'", regNum);
        }

        private static object GetMORegNumDigits(object regNumValue)
        {
            if (regNumValue != DBNull.Value)
            {
                var regNumStr = Convert.ToString(regNumValue);

                if (regNumStr.Length > 3)
                {
                    return regNumStr.Substring(1, 3);
                }
            }
            return regNumValue;
        }

        private static DataTable GetMOPlanInfo(
            int monthNum, // месяц
            ReportDocumentType docType, // тип документа кредит\гарантия\цб
            DataTable tblResult, // таблица для сформированных строк данных
            DataRow rowData, // строчка шаблон данных, которую нужно подправить и добавить в таблицу результата
            IEnumerable<DataRow> rowsSelect, // строчки детализации, которые нужно править под шаблон
            string dbtSumField, // сумма по ОД
            string calcDate, // дата записи
            bool isPercent,
            decimal exchangeRate,
            bool isForeign)
        {
            foreach (DataRow rowDebt in rowsSelect)
            {
                tblResult.ImportRow(rowData);
                var rowCurrent = GetLastRow(tblResult);

                if (isPercent)
                {
                    rowCurrent[1] = 7;
                    rowCurrent[5] = "Проценты";
                    
                    if (docType == ReportDocumentType.Capital)
                    {
                        rowCurrent[5] = "Купон";
                    }
                }
                else
                {
                    rowCurrent[1] = 8;
                    rowCurrent[5] = "Основной долг";
                }

                decimal multiplier = isForeign ? exchangeRate : 1;
                rowCurrent[3] = calcDate;
                rowCurrent[7] = multiplier * GetDecimal(rowDebt[dbtSumField]);
                rowCurrent[TempFieldNames.SortStatus] = monthNum;
            }

            return tblResult;
        }

        private static DataTable CorrectMOCashPlanData(CommonDataObject cdo, DataTable tblContracts, ReportDocumentType docType, int year, decimal exchangeRate)
        {
            var maxDate = DateTime.MaxValue.ToShortDateString();
            const string templateSelect = "{0} = '{1}' and {2} = {3}";
            var detailDates = new Collection<string>();
            var tblResult = tblContracts.Clone();
            // кредиты
            var keyMainDbt = Convert.ToInt32(t_S_PlanDebtCI.key);
            var keyPercent = Convert.ToInt32(t_S_PlanServiceCI.key);
            var masterField = f_S_Creditincome.id;
            var dbtDateField = t_S_PlanDebtCI.EndDate;
            var dbtRefField = t_S_PlanDebtCI.RefCreditInc;
            var dbtSumField = t_S_PlanDebtCI.Sum;
            var pctDateField = t_S_PlanServiceCI.PaymentDate;
            var pctRefField = t_S_PlanServiceCI.RefCreditInc;
            // цб
            if (docType == ReportDocumentType.Capital)
            {
                masterField = f_S_Capital.id;
                keyMainDbt = Convert.ToInt32(t_S_CPPlanDebt.key);
                keyPercent = Convert.ToInt32(t_S_CPPlanService.key);
                dbtDateField = t_S_CPPlanDebt.EndDate;
                dbtRefField = t_S_CPPlanDebt.RefCap;
                dbtSumField = t_S_CPPlanDebt.Sum;
                pctDateField = t_S_CPPlanService.EndDate;
                pctRefField = t_S_CPPlanService.RefCap;
            }
            // гарантии
            if (docType == ReportDocumentType.Garant)
            {
                masterField = f_S_Guarantissued.id;
                keyMainDbt = Convert.ToInt32(t_S_PlanDebtPrGrnt.key);
                keyPercent = Convert.ToInt32(t_S_PlanServicePrGrnt.key);
                dbtDateField = t_S_PlanDebtPrGrnt.EndDate;
                dbtRefField = t_S_PlanDebtPrGrnt.RefGrnt;
                dbtSumField = t_S_PlanDebtPrGrnt.Sum;
                pctDateField = t_S_PlanServicePrGrnt.EndDate;
                pctRefField = t_S_PlanServicePrGrnt.RefGrnt;
            }

            foreach (DataRow rowData in tblContracts.Rows)
            {
                var masterRef = Convert.ToInt32(rowData[masterField]);

                if (docType == ReportDocumentType.Capital)
                {
                    var startCapDate = Convert.ToDateTime(rowData[14]);
                    rowData[4] = String.Format("Московский облигационный займ {0} № {1}", startCapDate.Year, rowData[15]);
                    rowData[6] = "инвесторы";
                }

                if (docType == ReportDocumentType.CreditBud || docType == ReportDocumentType.CreditOrg)
                {
                    rowData[4] = rowData[14];
                }

                for (var m = 0; m < 12; m++)
                {
                    var monthNum = m + 1;
                    var loDate = GetMonthStart(year, monthNum);
                    var hiDate = GetMonthEnd(year, monthNum);
                    var offset = m*4;

                    if (docType == ReportDocumentType.Garant)
                    {
                        offset = m * 6;
                    }

                    detailDates.Clear();
                    var planFilterDate = docType == ReportDocumentType.Garant ? maxDate : hiDate;
                    cdo.FilterDetailTables(masterRef, planFilterDate);
                    // список плановых изменений по ОД
                    detailDates = FillDetailDateList(detailDates, cdo.dtDetail[keyMainDbt], dbtDateField, loDate, hiDate,
                                                     dbtRefField, masterRef);
                    // список плановых изменений по процентам
                    detailDates = FillDetailDateList(detailDates, cdo.dtDetail[keyPercent], pctDateField, loDate, hiDate,
                                                     pctRefField, masterRef);

                    foreach (string t in detailDates)
                    {
                        var selectStr1 = String.Format(templateSelect, dbtDateField, t, dbtRefField,
                                                       rowData[masterField]);
                        var selectStr2 = String.Format(templateSelect, pctDateField, t, pctRefField,
                                                       rowData[masterField]);

                        var currentSumField = dbtSumField;
                        var isForeign = false;

                        if (docType == ReportDocumentType.Capital)
                        {
                            rowData[2] = 1;
                            rowData[8] = rowData[16 + offset];
                            rowData[9] = rowData[17 + offset];
                        }
                        else if (docType == ReportDocumentType.Garant)
                        {
                            rowData[2] = 5;
                            var part1 = Convert.ToString(rowData[19 + offset]);
                            var part2 = Convert.ToString(rowData[20 + offset]);
                            rowData[08] = String.Format("{0} {1}", part1, part2).Trim();
                            rowData[09] = rowData[18 + offset];
                            rowData[10] = rowData[15 + offset];
                            isForeign = Convert.ToInt32(rowData[TempFieldNames.OKVShortName]) != ReportConsts.codeRUB;

                            if (isForeign)
                            {
                                currentSumField = ReportConsts.CurrencySumField;
                            }
                        }
                        else
                        {
                            rowData[2] = 3;
                            if (docType == ReportDocumentType.CreditBud)
                            {
                                rowData[2] = 7;
                            }
                            rowData[8] = rowData[17 + offset];
                            rowData[9] = rowData[18 + offset];
                        }

                        var drsMainDebt = cdo.dtDetail[keyMainDbt].Select(selectStr1);
                        tblResult = GetMOPlanInfo(monthNum, docType, tblResult, rowData, drsMainDebt, currentSumField, t,
                                                  false, exchangeRate, isForeign);

                        if (docType == ReportDocumentType.Capital)
                        {
                            rowData[2] = 2;
                            rowData[8] = rowData[18 + offset];
                            rowData[9] = rowData[19 + offset];
                        }
                        else if (docType == ReportDocumentType.Garant)
                        {
                            rowData[2] = 6;
                            rowData[08] = rowData[16 + offset];
                            rowData[09] = null;
                            rowData[10] = rowData[17 + offset];
                        }
                        else
                        {
                            rowData[2] = 4;
                            if (docType == ReportDocumentType.CreditBud)
                            {
                                rowData[2] = 8;
                            }
                            rowData[8] = rowData[19 + offset];
                            rowData[9] = rowData[20 + offset];
                        }

                        var drsPercent = cdo.dtDetail[keyPercent].Select(selectStr2);
                        tblResult = GetMOPlanInfo(monthNum, docType, tblResult, rowData, drsPercent, currentSumField, t,
                                                  true, exchangeRate, isForeign);
                    }
                }
            }
            return tblResult;
        }

        private static void SetColumnValue(DataTable tblData, int colIndex, object value)
        {
            foreach (DataRow rowData in tblData.Rows)
            {
                rowData[colIndex] = value;
            }
        }

        // Для вологодских группировок
        private DataTable ConvertRegionNames(DataTable dtResult)
        {
            foreach (DataRow dr in dtResult.Rows)
            {
                var regionId = Convert.ToInt32(dr[0]);

                if (regionId != -1)
                {
                    var convertRegionStr = GetParentTerritoryData(scheme, regionId).Split('=');
                    dr[0] = convertRegionStr[0];
                    dr[2] = convertRegionStr[1];
                }
            }
            return dtResult;
        }

        private static string CompareConstructorDataValue(DataTable dtMain, string dateType, int dateKind, string defaultValue)
        {
            var resultDate = defaultValue;
            var dateFieldName = "StartDate";
            
            if (dateKind != 0)
            {
                dateFieldName = "EndDate";
            }

            foreach (DataRow drContract in dtMain.Rows)
            {
                if (drContract[dateFieldName] != DBNull.Value)
                {
                    var contractDate = Convert.ToDateTime(drContract[dateFieldName]);
                    var resultDateValue = Convert.ToDateTime(resultDate);
                    
                    if (resultDate == DateTime.MinValue.ToShortDateString() ||
                        resultDate == DateTime.MaxValue.ToShortDateString())
                    {
                        resultDateValue = contractDate;
                        resultDate = resultDateValue.ToShortDateString();
                    }

                    if ((dateType == "i8" && contractDate.CompareTo(resultDateValue) < 0) ||
                        (dateType == "i9" && contractDate.CompareTo(resultDateValue) > 0))
                    {
                        resultDate = contractDate.ToShortDateString();
                    }
                }
            }
            return resultDate;
        }

        private static string GetConstructorDataValue(string dateType, string calcDate, string commonDate1, string commonDate2)
        {
            var result = calcDate;
            if (dateType == "i1") result = DateTime.Now.ToShortDateString();
            if (dateType == "i2") result = GetMonthStart(DateTime.Now);
            if (dateType == "i3") result = GetMonthEnd(DateTime.Now);
            if (dateType == "i4") result = GetYearStart(DateTime.Now.Year);
            if (dateType == "i5") result = GetYearEnd(DateTime.Now.Year);
            if (dateType == "i6") result = commonDate1;
            if (dateType == "i7") result = commonDate2;
            if (dateType == "i8") result = DateTime.MinValue.ToShortDateString();
            if (dateType == "i9") result = DateTime.MaxValue.ToShortDateString();
            return Convert.ToDateTime(result).ToShortDateString();
        }

        private static void SetColumnConditions(CommonDataObject cdo, int year, int rowIndex)
        {
            cdo.columnCondition.Clear();
            var terrType = ReportTerritoryType.ttRegion;
            
            if (rowIndex == 1 || rowIndex == 3)
            {
                terrType = ReportTerritoryType.ttSettlement;
            }

            var territoryCodes = GetTerritoryID(cdo.scheme, terrType);

            // кредиты от организаций, нужны только на колонке ЦК
            var creditType = "4,3";

            for (var i = 0; i < 12; i++)
            {
                if (i > 0) creditType = "3";
                if (rowIndex < 4)
                {
                    cdo.columnCondition.Add(i, String.Format("RefSTypeCredit={0};RefRegions={1}",
                        creditType, territoryCodes));
                }
                else
                {
                    cdo.columnCondition.Add(i, "RefSTypeCredit=4");
                }
            }
            // условия расчета сумм в колонках
            cdo.columnCondition[0] = String.Format("{0};CreditStartYear={1}", cdo.columnCondition[0], year);
            cdo.columnCondition[1] = String.Format("{0};RefVariant=0", cdo.columnCondition[1]);
            cdo.columnCondition[2] = String.Format("{0};RefVariant=0", cdo.columnCondition[2]);
            cdo.columnCondition[3] = String.Format("{0};RefVariant=0", cdo.columnCondition[3]);
            cdo.columnCondition[4] = String.Format("{0};RefVariant=0", cdo.columnCondition[4]);

            cdo.columnParamList[2]["IsCentralCredit"] = "0";
            cdo.columnParamList[3]["IsCentralCredit"] = "1";
            cdo.columnParamList[4]["IsCentralCredit"] = "1";
            cdo.columnParamList[7]["IsCentralCredit"] = "0";
            cdo.columnParamList[8]["IsCentralCredit"] = "1";
        }

        // выбиратор строк из детали(detailIndex) нужного договора(parentId), 
        // которые попадают в период начиная с startDate по endDate
        private static IEnumerable<DataRow> GetDetailRows(CommonDataObject cdo, int detailIndex, string startDate, string endDate,
            int parentId)
        {
            DataTable dtPlanDetail;
            string dateFieldName;
            // если деталь с суммами
            if (detailIndex >= 0)
            {
                dtPlanDetail = cdo.dtDetail[detailIndex];
                dateFieldName = cdo.GetEndDates()[detailIndex];
            }
            else
            // если журнал процентов
            {
                dtPlanDetail = cdo.dtJournalPercent;
                dateFieldName = t_S_JournalPercentCI.ChargeDate;
            }
            
            // выбираем записи за нужный период по конкретному договору
            if (dtPlanDetail != null)
            {
                return dtPlanDetail.Select(String.Format("{0}>='{1}' and {0}<='{2}' and {3} = {4}",
                        dateFieldName, startDate, endDate, cdo.GetParentRefName(), parentId), StrSortUp(dateFieldName));
            }

            return null;
        }

        // расщепитель строк в соотвествии с платежами по основному долгу и изменениями процентной ставки
        // Для отчета "Расчет процентов за пользование заемными средствами"
        private DataTable SplitRowsCalcPercentVologda(CommonDataObject cdo, DataTable dtSource, string calcDate,
            ref int counter, ref decimal totalSum)
        {
            var dtResult = CreateReportCaptionTable(9);
            var contractCounter = 1;
            // погнали договора расщеплять
            var refOrgOld = -1;

            foreach (DataRow drSource in dtSource.Rows)
            {
                var parentId = Convert.ToInt32(drSource[f_S_Creditincome.id]);
                var refOrgNew = Convert.ToInt32(drSource[f_S_Creditincome.RefOrganizations]);
                // Ищем ближайший план уплаты процентов
                var planRow = cdo.GetNearestRow(5, calcDate, parentId, true);                
                
                if (planRow != null)
                {
                    DataRow drResult;
                    // Заголовок по организации
                    if (refOrgNew != refOrgOld)
                    {
                        drResult = dtResult.Rows.Add();
                        drResult[0] = "1"; // признак заголовочной строки
                        drResult[1] = ++contractCounter;
                        drResult[2] = drSource[1];
                    }
                    // Начинаем мучения по разделениям на периоды
                    var startPeriodDate = cdo.GetDateValue(planRow[t_S_PlanServiceCI.StartDate]);
                    var endPeriodDate = cdo.GetDateValue(planRow[t_S_PlanServiceCI.EndDate]);
                    var dateChangeList = new Collection<string>();
                    var drsJounal = GetDetailRows(cdo, -1, startPeriodDate, endPeriodDate, parentId);
                    var drsFactAttr = GetDetailRows(cdo, 0, startPeriodDate, endPeriodDate, parentId);
                    var drsFactDebt = GetDetailRows(cdo, 1, startPeriodDate, endPeriodDate, parentId);
                    // создаем общий список дат изменений по основному долгу и процентным ставкам за период
                    dateChangeList = FillDetailDateList(dateChangeList, drsJounal);
                    dateChangeList = FillDetailDateList(dateChangeList, drsFactAttr);
                    dateChangeList = FillDetailDateList(dateChangeList, drsFactDebt);
                    
                    if (dateChangeList.Count == 0)
                    {
                        // если делить период не надо, то все просто берем из главной таблицы и первой записи плана
                        drResult = dtResult.Rows.Add();
                        drResult[0] = "2"; // признак строки данных
                        drResult[1] = contractCounter;
                        drResult[2] = drSource[2];
                        drResult[3] = startPeriodDate;
                        drResult[4] = endPeriodDate;
                        drResult[5] = drSource[3];
                        drResult[6] = planRow[t_S_PlanServiceCI.CreditPercent];
                        drResult[7] = planRow[t_S_PlanServiceCI.DayCount];
                        drResult[8] = planRow[t_S_PlanServiceCI.Sum];
                        totalSum += Convert.ToDecimal(drResult[8]);
                    }
                    else
                    {
                        // но вот если меняли ставку внутри периода или производили уплаты по основному долгу...
                        var startDate = startPeriodDate;
                        
                        if (!dateChangeList.Contains(endPeriodDate))
                        {
                            dateChangeList.Add(endPeriodDate);
                        }

                        foreach (var endDate in dateChangeList)
                        {
                            drResult = dtResult.Rows.Add();
                            drResult[0] = "2";
                            drResult[1] = contractCounter;
                            drResult[2] = drSource[2]; // номер договра
                            // частичка общего периода
                            drResult[3] = startDate; 
                            drResult[4] = endDate;
                            var startDateValue = Convert.ToDateTime(startDate);
                            var endDateValue = Convert.ToDateTime(endDate);
                            // считаем суммы привлечения и погашения по частичке
                            var attrSum = cdo.GetSumValue(cdo.dtDetail[0], parentId, cdo.GetEndDates()[0], ReportConsts.SumField,
                                                          DateTime.MinValue, endDateValue, false, false);
                            var debtSum = cdo.GetSumValue(cdo.dtDetail[1], parentId, cdo.GetEndDates()[1], ReportConsts.SumField,
                                                          DateTime.MinValue, endDateValue, false, false);
                            drResult[5] = attrSum - debtSum;
                            var dateDiff = endDateValue - startDateValue;
                            // число дней в этой частичке
                            drResult[7] = dateDiff.Days + 1;
                            // последняя строка в журнале процентов до этой частички
                            var drLastPercent = cdo.GetNearestRow(-1, endDate, parentId, false);

                            if (drLastPercent != null) 
                            {
                                drResult[6] = drLastPercent[t_S_JournalPercentCI.CreditPercent];
                                // високосные года чтоб их(!не знаю что делать если начало и конец частички в разных годах)
                                decimal maxDayCount = 365;
                                if (DateTime.IsLeapYear(Convert.ToDateTime(endDate).Year)) maxDayCount += 1;
                                // считаем проценты по частичке
                                drResult[8] = Convert.ToDecimal(drResult[5]) * Convert.ToDecimal(drResult[6]) / 100 *
                                              Convert.ToDecimal(drResult[7]) / maxDayCount;

                                totalSum += Convert.ToDecimal(drResult[8]);
                            }
                            // следующая ччастичка
                            startDate = Convert.ToDateTime(endDate).AddDays(1).ToShortDateString();
                        }
                    }
                    
                    if (refOrgNew != refOrgOld)
                    {
                        refOrgOld = refOrgNew;
                    }
                }
            }
            counter = contractCounter;
            return dtResult;
        }

        private DataTable FillGarantChangesVologdaDetailData(CommonDataObject cdo, DataTable dtSource, string minDate, string maxDate)
        {
            var dt = CreateReportCaptionTable(10);
            var dateList = new Collection<string>();

            foreach (DataRow dr in dtSource.Rows)
            {
                // строим общий список дат детализаций по всем нужным деталям
                dateList.Clear();

                for (var i = 0; i < cdo.dtDetail.Length; i++)
                {
                    if (cdo.dtDetail[i] != null)
                    {
                        var drsSelect = cdo.dtDetail[i].Select(
                            String.Format("{0} >= '{1}' and {0} <= '{2}' and {3} = {4}",
                            cdo.GetEndDates()[i], minDate, maxDate, cdo.GetParentRefName(), dr[f_S_Guarantissued.id]));
                        dateList = FillDetailDateList(dateList, drsSelect);
                    }
                }

                // по каждой дате изменения раскидываем суммы по нужным столбцам
                foreach (var t in dateList)
                {
                    var drDetail = dt.Rows.Add();
                    var hasData = false;

                    for (var j = 0; j < cdo.dtDetail.Length; j++)
                    {
                        if (cdo.dtDetail[j] == null)
                        {
                            continue;
                        }

                        var sumFieldName = ReportConsts.SumField;
                        var marginFieldName = ReportConsts.Margin;
                        var commissionFieldName = ReportConsts.Commission;

                        if (Convert.ToInt32(dr[f_S_Guarantissued.RefOKV]) != -1)
                        {
                            sumFieldName = ReportConsts.CurrencySumField;
                            marginFieldName = ReportConsts.CurrencyMargin;
                            commissionFieldName = ReportConsts.CurrencyCommission;
                        }

                        var drsSelect = cdo.dtDetail[j].Select(
                            String.Format("{0} = '{1}' and {2} = {3}",
                                          cdo.GetEndDates()[j], t, cdo.GetParentRefName(), dr[f_S_Guarantissued.id]));

                        foreach (var rowSelect in drsSelect)
                        {
                            if (hasData)
                            {
                                drDetail = dt.Rows.Add();
                            }

                            hasData = true;
                            drDetail[0] = dr[f_S_Guarantissued.id];
                            var paymentNo = String.Empty;

                            if (drsSelect[0].Table.Columns.Contains("Payment"))
                            {
                                paymentNo = String.Format("№ {0} ", rowSelect["Payment"]);
                            }

                            drDetail[1] = String.Format("{0}от {1}",
                                                        paymentNo, cdo.GetDateValue(rowSelect[cdo.GetEndDates()[j]]));
                            var operationName = String.Empty;
                            decimal sumMult = 1;

                            if (j == 00)
                            {
                                operationName = "Привлечение основного долга";
                            }
                            if (j == 01)
                            {
                                operationName = "Гашение основного долга";
                                sumMult = -1;
                            }
                            if (j == 03)
                            {
                                operationName = "Начисление основного долга";
                            }
                            if (j == 04)
                            {
                                operationName = "Гашение процентов";
                                sumMult = -1;
                            }
                            if (j == 05)
                            {
                                operationName = "Начисление процентов";
                            }
                            if (j == 12)
                            {
                                operationName = "Гашение пени по основному долгу";
                                sumMult = -1;
                            }
                            if (j == 09)
                            {
                                operationName = "Гашение пени по процентам";
                                sumMult = -1;
                            }
                            if (j == 08)
                            {
                                operationName = "Начисление пени по основному долгу";
                            }
                            if (j == 11)
                            {
                                operationName = "Начисление пени по процентам";
                            }
                                
                            var resultColumnIndex = 3;
                                
                            if (j > 7)
                            {
                                resultColumnIndex = 7;
                            }

                            if (j == 4 || j == 5)
                            {
                                resultColumnIndex = 4;
                                if (rowSelect[sumFieldName] == DBNull.Value)
                                {
                                    if (rowSelect[marginFieldName] != DBNull.Value)
                                    {
                                        if (j == 04) operationName = "Гашение маржи";
                                        if (j == 05) operationName = "Начисление маржи";
                                        resultColumnIndex = 5;
                                        sumFieldName = marginFieldName;
                                    }
                                    else if (rowSelect[commissionFieldName] != DBNull.Value)
                                    {
                                        if (j == 04) operationName = "Гашение комиссии";
                                        if (j == 05) operationName = "Начисление комиссии";
                                        resultColumnIndex = 6;
                                        sumFieldName = commissionFieldName;
                                    }
                                }
                            }

                            drDetail[2] = operationName;
                            drDetail[resultColumnIndex] = sumMult * Convert.ToDecimal(rowSelect[sumFieldName]);
                        }
                    }
                }

                var drsDetail = dt.Select(String.Format("{0} = '{1}'", dt.Columns[0].ColumnName, dr[f_S_Guarantissued.id]));
                var drResult = dt.Rows.Add();
                drResult[0] = dr[f_S_Guarantissued.id];
                drResult[1] = "Итого";
                FillVologdaChangeSummaryRow(drResult, drsDetail);
            }

            var drsSummary = dt.Select(String.Format("{0} = 'Итого'", dt.Columns[1].ColumnName));
            var drTotal = dt.Rows.Add();
            FillVologdaChangeSummaryRow(drTotal, drsSummary);
            return dt;
        }

        private static void FillVologdaChangeSummaryRow(DataRow drSummary, IEnumerable<DataRow> drsSummary)
        {
            var totalSummary = new decimal[drSummary.Table.Columns.Count];

            foreach (var t in drsSummary)
            {
                for (var j = 3; j < 8; j++)
                {
                    if (t[j] != DBNull.Value && t[2].ToString() != "Начисление основного долга")
                    {
                        totalSummary[j] += Convert.ToDecimal(t[j]);
                    }
                }
            }

            for (var i = 3; i < 8; i++)
            {
                drSummary[i] = totalSummary[i];
            }
        }

        private static void FillExchangeValueSamara(DataRow drCaption, int outIndex, CommonDataObject cdo)
        {
            decimal exchangeValue = 0;
            var exchangeName = String.Empty;

            if (cdo.okvValues.ContainsKey(ReportConsts.codeUSD))
            {
                exchangeValue = cdo.okvValues[ReportConsts.codeUSD];
                exchangeName = "Доллар США";
            }

            if (cdo.okvValues.ContainsKey(ReportConsts.codeEUR))
            {
                exchangeValue = cdo.okvValues[ReportConsts.codeEUR];
                exchangeName = "ЕВРО";
            }

            if (exchangeName.Length > 0)
            {
                drCaption[outIndex + 0] = exchangeName;
                drCaption[outIndex + 1] = exchangeValue;
            }
        }

        // Группировка по колонке с номером groupColumnIndex
        private static void GroupProgrammSamaraData(DataTable dtResult, DataTable dtSource, int groupColumnIndex, int prefix)
        {
            var drSummary = dtResult.Rows.Add();
            var totalSummary = new decimal[2];
            var summary = new decimal[2];
            var foundKeys = new Collection<string>();
            totalSummary[0] = 0; totalSummary[1] = 0;
            var counter = 1;

            foreach (DataRow dr in dtSource.Rows)
            {
                var key = dr[groupColumnIndex].ToString();

                if (!foundKeys.Contains(key))
                {
                    var drsSelect = dtSource.Select(String.Format("{0} = '{1}'",
                        dtSource.Columns[groupColumnIndex].ColumnName, dr[groupColumnIndex]));
                    summary[0] = 0; summary[1] = 0;

                    foreach (DataRow t in drsSelect)
                    {
                        summary[0] += Convert.ToDecimal(t[1]);
                        summary[1] += Convert.ToDecimal(t[2]);
                    }

                    totalSummary[0] += summary[0];
                    totalSummary[1] += summary[1];

                    if (summary[0] != 0 || summary[1] != 0)
                    {
                        var drData = dtResult.Rows.Add();
                        drData[0] = String.Format("{0}.{1}", prefix + 1, counter);
                        drData[1] = dr[groupColumnIndex];
                        drData[2] = summary[0];
                        drData[3] = summary[1];
                    }

                    foundKeys.Add(key);
                    counter++;
                }
            }

            drSummary[2] = totalSummary[0];
            drSummary[3] = totalSummary[1];
        }

        private static decimal CalcDetailSum(Dictionary<int, string> detailList, Collection<DataRow>[] rowList,
            Collection<int>[] usedKeys, int index, DateTime startDate, object sumCell,
            ref Collection<DataRow> rowCollection, bool isPeni)
        {
            decimal result = 0;
            decimal sumValue = 0;
            var dateValue = String.Empty;
            rowCollection.Clear();

            if (sumCell.ToString() == String.Empty) return result;

            if (isPeni)
            {
                dateValue = sumCell.ToString();
            }
            else
            {
                sumValue = Convert.ToDecimal(sumCell);
            }

            var dateValues = dateValue.Split(',');
            var overflowValue = false;
            var indexConv = new Dictionary<int, int> { { 0, 7 }, { 1, 6 }, { 2, 9 }, { 3, 5 }, { 4, 8 } };

            foreach (var dr in rowList[index])
            {
                var id = Convert.ToInt32(dr["id"]);
                var detailDate = Convert.ToDateTime(dr[detailList[indexConv[index]]]);

                if (!overflowValue && !usedKeys[index].Contains(id))
                {
                    if (isPeni)
                    {
                        var partDate = dateValues.Aggregate(false, (current, t) => 
                            current || DateTime.Compare(Convert.ToDateTime(t), detailDate) == 0);

                        if (partDate)
                        {
                            result += Convert.ToDecimal(dr[ReportConsts.SumField]);
                            usedKeys[index].Add(id);
                            rowCollection.Add(dr);
                        }
                    }
                    else
                    {
                        if (DateTime.Compare(startDate, detailDate) <= 0)
                        {
                            result += Convert.ToDecimal(dr[ReportConsts.SumField]);
                            usedKeys[index].Add(id);

                            rowCollection.Add(dr);

                            if (result > sumValue)
                            {
                                dr[ReportConsts.SumField] = result - sumValue;
                                result = sumValue;
                                usedKeys[index].Remove(id);
                            }

                            if (result == sumValue)
                            {
                                overflowValue = true;
                            }
                        }
                    }
                }
            }

            if (result == 0)
            {
                rowCollection.Clear();
            }

            return result;
        }

        private static decimal[] CorrectGarantData(DataTable dtTable, int year)
        {
            const string formatCurrency = "эквивалент {0:N2} долларов США";
            const string formatRub = "{0:N2} тыс. руб";
            var sumArray = new decimal[4];

            foreach (DataRow dr in dtTable.Rows)
            {
                if (dr[f_S_Guarantissued.RefOKV] != DBNull.Value)
                {
                    if (Convert.ToInt32(dr[f_S_Guarantissued.RefOKV]) != -1)
                    {
                        var sumValue = Convert.ToDecimal(dr[8]);
                        sumArray[2] += sumValue;

                        dr[3] = String.Format(formatCurrency, sumValue);
                        if (Convert.ToDateTime(dr[12]).Year == year)
                        {
                            sumArray[3] += sumValue;
                            dr[4] = String.Format(formatCurrency, sumValue);
                        }
                    }
                    else
                    {
                        var sumValue = ConvertTo1000(Convert.ToDecimal(dr[7]));
                        dr[3] = String.Format(formatRub, sumValue);
                        sumArray[0] += sumValue;

                        if (Convert.ToDecimal(dr[9]) > 0)
                        {
                            sumValue = ConvertTo1000(Convert.ToDecimal(dr[10]));
                            dr[4] = String.Format(formatRub, sumValue);
                            sumArray[1] += sumValue;
                        }
                    }
                }
            }
            return sumArray;
        }

        private static string CreatePeriodStr(string date1, string date2)
        {
            var result = String.Empty;

            if (date1.Length > 0 && date2.Length > 0)
            {
                result = String.Format("с {0} по {1}", date1, date2);
            }

            return result;
        }

        private static decimal[] WriteSummary(DataTable dt, decimal[] summaryArray,
            IEnumerable<int> summaryIndex, int contractPos, string caption)
        {
            var drSummary = dt.Rows.Add();

            foreach (var columnIndex in summaryIndex)
            {
                drSummary[columnIndex] = summaryArray[columnIndex];
                summaryArray[columnIndex] = 0;
            }

            drSummary[0] = caption;
            
            if (caption != "Итого")
            {
                drSummary[5] = DBNull.Value;
            }

            SetContractOrderNum(drSummary, contractPos);
            return summaryArray;
        }

        private static decimal[] CalcSummary(DataRow dr, decimal[] summaryArray, IEnumerable<int> summaryIndex)
        {
            foreach (var columnIndex in summaryIndex)
            {
                if (dr[columnIndex] != DBNull.Value)
                {
                    if (columnIndex != 5)
                    {
                        summaryArray[columnIndex] += Convert.ToDecimal(dr[columnIndex]);
                    }
                    else
                    {
                        summaryArray[columnIndex] = Convert.ToDecimal(dr[columnIndex]);
                    }
                }
            }

            return summaryArray;
        }

        private static string GetCalcDetailDateList(IEnumerable<DataRow> rows, string dateFieldName, bool convertToDate = true)
        {
            var result = String.Empty;

            foreach (var dr in rows)
            {
                if (dr[dateFieldName] == DBNull.Value)
                {
                    continue;
                }

                var value = dr[dateFieldName].ToString();
                    
                if (convertToDate)
                {
                    value = Convert.ToDateTime(value).ToShortDateString();
                }

                result = String.Format("{0},{1}", result, value);
            }
            return result.TrimStart(',');
        }

        private static void SetContractOrderNum(DataRow dr, int rowNum)
        {
            dr[dr.Table.Columns.Count - 1] = rowNum;
        }

        private static void FillBorrowSumArray(DataTable dtData, ref decimal[,] summary,
            int dataRowIndex, int summaryRowIndex, int year)
        {
            for (var i = 0; i < 2; i++)
            {
                foreach (DataRow dr in dtData.Rows)
                {
                    var valueTotal = Convert.ToDecimal(dr[0]);
                    var valueFact = Convert.ToDecimal(dr[i * 2 + 1]);
                    var valuePlan = Convert.ToDecimal(dr[i * 2 + 2]);
                    decimal value = 0;

                    if (valueFact > 0)
                    {
                        value = valueFact;
                    }
                    else
                    {
                        if (valuePlan > 0)
                        {
                            value = valuePlan;
                        }
                        else
                        {
                            if (Convert.ToDateTime(dr["StartDate"]).Year == year)
                            {
                                value = valueTotal;
                            }
                        }
                    }

                    value = ConvertTo1000(value);
                    summary[dataRowIndex, i] += value;
                    summary[summaryRowIndex, i] += value;
                }
            }
        }

        private static DataTable GroupOrgData(
            DataTable dtTable, 
            IEnumerable<int> summaryColumnIndex,
            string fieldName = f_S_Guarantissued.RefOrganizations, 
            string fieldCaption = TempFieldNames.OrgName)
        {
            var dtOrgData = dtTable.Clone();
            var refOrgCurr = ReportConsts.UndefinedKey;
            var refOrgPrev = ReportConsts.UndefinedKey;
            var isFirst = true;
            var summary = new decimal[dtTable.Columns.Count];
            var cntInsert = 0;

            foreach (DataRow drData in dtTable.Rows)
            {
                if (drData[fieldName] != DBNull.Value)
                {
                    refOrgCurr = Convert.ToString(drData[fieldName]);

                    if (String.Compare(refOrgCurr, refOrgPrev, true) != 0)
                    {
                        if (!isFirst && cntInsert > 1)
                        {
                            var drSummary = dtOrgData.Rows.Add();
                            drSummary[TempFieldNames.RowType] = 3;

                            foreach (var index in summaryColumnIndex)
                            {
                                drSummary[index] = summary[index];
                            }

                            drSummary[0] = "Итого по организации";
                            cntInsert = 0;
                        }

                        var drOrgName = dtOrgData.Rows.Add();
                        drOrgName[0] = drData[fieldCaption];
                        drOrgName[TempFieldNames.RowType] = 1;

                        foreach (var index in summaryColumnIndex)
                        {
                            summary[index] = 0;
                        }
                    }
                }
                else
                {
                    if (dtTable.Rows.Count > 1 && cntInsert > 1)
                    {
                        var drSummary = dtOrgData.Rows.Add();
                        drSummary[TempFieldNames.RowType] = 3;

                        foreach (var index in summaryColumnIndex)
                        {
                            drSummary[index] = summary[index];
                        }

                        foreach (var index in summaryColumnIndex)
                        {
                            summary[index] = 0;
                        }

                        drSummary[0] = "Итого по организации";
                        cntInsert = 0;
                    }
                }

                drData[TempFieldNames.RowType] = 2;
                dtOrgData.ImportRow(drData);

                foreach (var index in summaryColumnIndex)
                {
                    summary[index] += GetNumber(drData[index]);
                }

                refOrgPrev = refOrgCurr;
                isFirst = false;
                cntInsert++;
            }

            return dtOrgData;
        }

        // Общий инициализатор параметров
        private int InitObjectParamsCI(CommonDataObject cdo, string calcDate, bool useYearStart)
        {
            var year = Convert.ToDateTime(calcDate).Year;
            var dateYearStart = GetYearStart(calcDate);
            
            if (!useYearStart)
            {
                dateYearStart = calcDate;
            }

            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Creditincome.StartDate, String.Format("<='{0}'", calcDate));
            cdo.mainFilter.Add(f_S_Creditincome.EndDate, String.Format(">='{0}' or c.{1}>='{0}'",
                dateYearStart, f_S_Creditincome.RenewalDate));
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.FixedVariantsID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.CreditIssuedBudCode);
            return year;
        }

        // Общий инициализатор параметров для вологодских отчетов
        private int InitObjectCI(CreditIssuedDataObject cdo, string calcDate)
        {
            var year = InitObjectParamsCI(cdo, calcDate, true);
            cdo.useSummaryRow = false;
            cdo.AddDataColumn(f_S_Creditincome.RefRegions);
            cdo.AddDataColumn(f_S_Creditincome.RefOrganizations);
            cdo.AddCalcColumn(CalcColumnType.cctRegion);
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            return year;
        }

        // Вологодская фича определения номера месяца по дате
        private static int GetMonthNum(string calcDate)
        {
            var monthNum = Convert.ToDateTime(calcDate).Month;
            
            if (Convert.ToDateTime(calcDate).Day == 1)
            {
                monthNum--;
            }

            return monthNum;
        }

        // Общий группирователь контрактов по территории
        private static void FillCommonTerritoryData(TerritoryColumnParam columnParam)
        {
            var dtTable = DataTableUtils.SortDataSet(columnParam.dtContracts,
                String.Format("{0} asc", columnParam.dtContracts.Columns[columnParam.colIndex2]));
            // Собираем список всех регионов по которым есть кредиты
            var regions = new Dictionary<int, string>();
            foreach (DataRow drContract in dtTable.Rows)
            {
                var regionID = Convert.ToInt32(drContract[columnParam.colIndex1]);

                if (regionID != -1 && !regions.ContainsKey(regionID))
                {
                    regions.Add(regionID, drContract[columnParam.colIndex2].ToString());
                }
            }

            var regionIndex = 0;
            DataRow drResult;

            foreach (var regionKey in regions.Keys)
            {
                if (columnParam.copyEachMonthRecords || columnParam.monthIndex == 1)
                {
                    drResult = columnParam.dtResult.Rows.Add();
                    if (columnParam.columnKind == 2 || columnParam.columnKind == 4)
                    {
                        for (var j = 0; j < columnParam.sumArray.Length; j++)
                        {
                            drResult[j + 1] = 0;
                        }
                    }
                }
                else
                {
                    drResult = columnParam.dtResult.Rows[regionIndex];
                }

                drResult[0] = regions[regionKey];
                var sum = new decimal[30];

                for (var j = 0; j < sum.Length; j++)
                {
                    sum[j] = 0;
                }

                if (columnParam.columnKind == 1)
                {
                    drResult[drResult.Table.Columns.Count - 1] = 0;
                    drResult[drResult.Table.Columns.Count - 2] = 0;
                }

                var i = 0;

                foreach (DataRow drContract in columnParam.dtContracts.Rows)
                {
                    if (Convert.ToInt32(drContract[columnParam.colIndex1]) == regionKey)
                    {
                        // Отчет по пеням
                        if (columnParam.columnKind == 0)
                        {
                            sum[0] += Convert.ToDecimal(drContract[2 + columnParam.monthIndex * 2]);
                            sum[1] += Convert.ToDecimal(drContract[3 + columnParam.monthIndex * 2]);
                        }

                        // Отчет по контрактам
                        if (columnParam.columnKind == 1)
                        {
                            sum[0] = Convert.ToDecimal(drContract[3 + columnParam.monthIndex * 2]);
                            sum[1] = Convert.ToDecimal(drContract[4 + columnParam.monthIndex * 2]);
                            drResult[i * 2 + 1] = sum[0];
                            drResult[i * 2 + 2] = sum[1];
                            drResult[drResult.Table.Columns.Count - 1] = Convert.ToDecimal(drResult[drResult.Table.Columns.Count - 1]) + sum[1];
                            drResult[drResult.Table.Columns.Count - 2] = Convert.ToDecimal(drResult[drResult.Table.Columns.Count - 2]) + sum[0];

                            columnParam.sumArray[i * 2 + 0] += sum[0];
                            columnParam.sumArray[i * 2 + 1] += sum[1];
                        }

                        // Помесячный план обслуживания
                        if (columnParam.columnKind == 2)
                        {
                            sum[0] = Convert.ToDecimal(drContract[3 + columnParam.monthIndex]);
                            drResult[columnParam.monthIndex] = Convert.ToDecimal(drResult[columnParam.monthIndex]) + sum[0];
                            columnParam.sumArray[columnParam.monthIndex - 1] += sum[0];
                        }

                        // Отчет по заимам
                        if (columnParam.columnKind == 3)
                        {
                            sum[0] += Convert.ToDecimal(drContract[4]);
                            sum[1] += Convert.ToDecimal(drContract[5]);
                            sum[2] += Convert.ToDecimal(drContract[6]);
                            sum[3] += Convert.ToDecimal(drContract[7]);

                            drResult[i * 4 + 1] = drContract[4];
                            drResult[i * 4 + 2] = drContract[5];
                            drResult[i * 4 + 3] = drContract[6];
                            drResult[i * 4 + 4] = drContract[7];
                            columnParam.sumArray[i * 4 + 0] += Convert.ToDecimal(drContract[4]);
                            columnParam.sumArray[i * 4 + 1] += Convert.ToDecimal(drContract[5]);
                            columnParam.sumArray[i * 4 + 2] += Convert.ToDecimal(drContract[6]);
                            columnParam.sumArray[i * 4 + 3] += Convert.ToDecimal(drContract[7]);
                        }

                        // Отчет по бюджетным займам
                        if (columnParam.columnKind == 4)
                        {
                            for (var kk = 0; kk < 11; kk++)
                            {
                                sum[kk] += Convert.ToDecimal(drContract[kk + 4]);
                                columnParam.sumArray[kk] += Convert.ToDecimal(drContract[kk + 4]);
                                drResult[kk + 1] = Convert.ToDecimal(drResult[kk + 1]) + Convert.ToDecimal(drContract[kk + 4]);
                            }
                        }

                        // Саратовский отчет по кредитам
                        if (columnParam.columnKind == 5)
                        {
                            var drData = columnParam.dtResult.Rows.Add();

                            for (var kk = 4; kk < 25; kk++)
                            {
                                drData[kk - 4] = drContract[kk];
                            }

                            for (var kk = 7; kk < 25; kk++)
                            {
                                columnParam.sumArray[kk - 5] += Convert.ToDecimal(drContract[kk]);
                                sum[kk - 4] += Convert.ToDecimal(drContract[kk]);
                            }
                        }
                    }

                    i++;
                }

                // Отчет по пеням
                if (columnParam.columnKind == 0)
                {
                    columnParam.sumArray[0] += sum[0];
                    columnParam.sumArray[1] += sum[1];

                    drResult[1] = sum[0];
                    drResult[2] = sum[1];
                    
                    if (columnParam.monthIndex == 13)
                    {
                        drResult[3] = sum[0] + sum[1];
                    }
                }
                // Отчет по контрактам
                if (columnParam.columnKind == 1)
                {
                    columnParam.sumArray[columnParam.sumArray.Length - 2] += sum[0];
                    columnParam.sumArray[columnParam.sumArray.Length - 1] += sum[1];
                }
                // Отчет по займам
                if (columnParam.columnKind == 3)
                {
                    columnParam.sumArray[columnParam.sumArray.Length - 4] += sum[0];
                    columnParam.sumArray[columnParam.sumArray.Length - 3] += sum[1];
                    columnParam.sumArray[columnParam.sumArray.Length - 2] += sum[2];
                    columnParam.sumArray[columnParam.sumArray.Length - 1] += sum[3];

                    drResult[drResult.Table.Columns.Count - 1] = sum[3];
                    drResult[drResult.Table.Columns.Count - 2] = sum[2];
                    drResult[drResult.Table.Columns.Count - 3] = sum[1];
                    drResult[drResult.Table.Columns.Count - 4] = sum[0];
                }
                // Саратовский отчет по кредитам
                if (columnParam.columnKind == 5)
                {
                    var drData = columnParam.dtResult.Rows.Add();
                    drData[0] = String.Format("Итого по '{0}'", drResult[0]);

                    for (var kk = 7; kk < 25; kk++)
                    {
                        drData[kk - 4] = sum[kk - 4];
                    }
                }

                regionIndex++;
            }

            if (columnParam.writeSummary)
            {
                if (columnParam.copyEachMonthRecords || columnParam.monthIndex == 1)
                {
                    drResult = columnParam.dtResult.Rows.Add();
                }
                else
                {
                    drResult = GetLastRow(columnParam.dtResult);
                }
                
                drResult[0] = "Всего:";

                for (var i = 0; i < columnParam.sumArray.Length; i++)
                {
                    drResult[i + 1] = columnParam.sumArray[i];
                }

                if (columnParam.monthIndex == 13)
                    drResult[3] = columnParam.sumArray[0] + columnParam.sumArray[1];
                
                // Саратовский отчет по кредитам
                if (columnParam.columnKind == 5)
                {
                    drResult[01] = DBNull.Value;
                    drResult[02] = DBNull.Value;
                }
            }
        }

        // Группировка по колонке с номером groupColumnIndex
        private static void GroupProgrammPeriodOmskData(DataTable dtResult, DataTable dtSource, int groupColumnIndex)
        {
            var totalSummary = new decimal[2];
            var summary = new decimal[2];
            var foundKeys = new Collection<string>();
            totalSummary[0] = 0; totalSummary[1] = 0;
            var realRowIndex = 1;

            foreach (DataRow dr in dtSource.Rows)
            {
                var key = dr[groupColumnIndex].ToString();
                if (!foundKeys.Contains(key))
                {
                    var drsSelect = dtSource.Select(String.Format("{0} = '{1}'",
                        dtSource.Columns[groupColumnIndex].ColumnName, dr[groupColumnIndex]));
                    summary[0] = 0; summary[1] = 0;
                    var firstRowIndex = 0;

                    for (var i = 0; i < drsSelect.Length; i++)
                    {
                        var val1 = Convert.ToDecimal(drsSelect[i][2]);
                        var val2 = Convert.ToDecimal(drsSelect[i][3]);
                        
                        if (Math.Abs(val1) > 0)
                        {
                            firstRowIndex = Math.Max(firstRowIndex, i);
                        }

                        summary[0] += val1;
                        summary[1] += val2;
                    }

                    totalSummary[0] += summary[0];
                    totalSummary[1] += summary[1];

                    if (summary[0] != 0 || summary[1] != 0)
                    {
                        var drData = dtResult.Rows.Add();
                        
                        for (var k = 0; k < dtSource.Columns.Count; k++)
                        {
                            drData[k] = drsSelect[firstRowIndex][k];
                        }

                        drData[0] = realRowIndex++;
                        drData[2] = summary[0];
                        drData[3] = summary[1];
                    }
                    foundKeys.Add(key);
                }
            }
        }

        private static void FillYearColumn(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr[8] = Convert.ToDecimal(dr[6]) + Convert.ToDecimal(dr[7]);
                dr[11] = Convert.ToDecimal(dr[9]) + Convert.ToDecimal(dr[10]);

                decimal rateSwitch = 0;
                if (dr[12] != DBNull.Value) rateSwitch = Convert.ToDecimal(dr[12]);

                dr[12] = rateSwitch + Convert.ToDecimal(dr[5]) + Convert.ToDecimal(dr[8]) - Convert.ToDecimal(dr[11]);
                dr[15] = Convert.ToDecimal(dr[12]) + Convert.ToDecimal(dr[13]) - Convert.ToDecimal(dr[14]);
                dr[18] = Convert.ToDecimal(dr[15]) + Convert.ToDecimal(dr[16]) - Convert.ToDecimal(dr[17]);
                dr[21] = Convert.ToDecimal(dr[18]) + Convert.ToDecimal(dr[19]) - Convert.ToDecimal(dr[20]);
            }
        }

        /// <summary>
        /// Структура для таблицы деталей отчета "Соответствие требованиям Бюджетного кодекса"
        /// </summary>
        private static DataTable CreateReportBKIndicatorsStructure()
        {
            var dt = new DataTable("Report");
            dt.Columns.Add("F00", typeof(Int32));
            dt.Columns.Add("F01", typeof(Decimal));
            dt.Columns.Add("F02", typeof(Decimal));
            dt.Columns.Add("F03", typeof(Decimal));
            dt.Columns.Add("F04", typeof(Decimal));
            return dt;
        }

        private decimal GetBKSumValue(IEntity planEntity, Dictionary<string, string> queryParams, Dictionary<string, string> refParams)
        {
            var conditionValue = queryParams.Keys.Aggregate(String.Empty, (current, key) => 
                CreditSQLObject.CombineFilter(current, key, queryParams[key]));

            foreach (var key in queryParams.Keys)
            {
                if (key.StartsWith("Code"))
                {
                    var valueSrc = key;
                    var codeIndex = String.Empty;
                    
                    if (valueSrc[5].ToString() != "0")
                    {
                        codeIndex = valueSrc[5].ToString();
                    }

                    var valueDst = String.Format("cls{0}.Code{1}", valueSrc[4], codeIndex);
                    conditionValue = conditionValue.Replace(String.Format("c.{0}", valueSrc), valueDst);
                }
            }

            var fromPart = String.Empty;
            var wherePart = String.Empty;
            var i = 1;

            foreach (var key in refParams.Keys)
            {
                fromPart = String.Format("{0}, {1} cls{2}", fromPart, key, i);
                wherePart = String.Format("{0} and cls{2}.id = c.{1}", wherePart, refParams[key], i++);
            }

            var sumFieldName = "Forecast";
            
            if (refParams.Count > 1)
            {
                sumFieldName = "Summa";
            }

            var selectStr =
                String.Format("select c.{4} from {0} c {1} where {2} {3}",
                planEntity.FullDBName, fromPart, conditionValue, wherePart, sumFieldName);

            var dbHelper = new ReportDBHelper(scheme);
            var dtSum = dbHelper.GetTableData(selectStr);
            decimal sum = dtSum.Rows.Cast<DataRow>().Sum(dr => Convert.ToDecimal(dr[0]));
            return ConvertTo1000(sum);
        }

        private static void SetCodeParams(Dictionary<string, string> queryParams, string paramStr)
        {
            var deletedKeys = new Collection<string>();

            foreach (var key in queryParams.Keys)
            {
                if (key.StartsWith("Code")) deletedKeys.Add(key);
            }

            foreach (var key in deletedKeys)
            {
                queryParams.Remove(key);
            }

            var paramList = paramStr.Split(';');

            foreach (string t in paramList)
            {
                if (t.Length == 0)
                {
                    continue;
                }

                var key = t.Split('=')[0];
                var value = t.Split('=')[1];
                queryParams.Add(key, value);
            }
        }

        private static void CorrectGarantSum(DataTable dtGarant, string yearStart, string calcDate, CommonDataObject cdo)
        {
            var drResult = GetLastRow(dtGarant);
            drResult[4] = 0;
            drResult[5] = 0;
            drResult[6] = 0;
            drResult[8] = 0;
            drResult[9] = 0;
            drResult[10] = 0;

            for (var i = 0; i < GetLastRowIndex(dtGarant); i++)
            {
                var drGarant = dtGarant.Rows[i];
                var masterKey = Convert.ToInt32(drGarant[f_S_Guarantissued.id]);
                drGarant[4] = Convert.ToDecimal(drGarant[ReportConsts.SumField]) - Convert.ToDecimal(drGarant[4])
                     - cdo.GetSumValue(cdo.dtDetail[4], masterKey, t_S_FactPercentPrGrnt.FactDate, "Margin,Commission", DateTime.MinValue, Convert.ToDateTime(yearStart), true, false);
                drGarant[6] = Convert.ToDecimal(drGarant[6])
                    + cdo.GetSumValue(cdo.dtDetail[4], masterKey, t_S_FactPercentPrGrnt.FactDate, "Sum,Margin,Commission", Convert.ToDateTime(yearStart), Convert.ToDateTime(calcDate), true, false);

                drGarant[9] = Convert.ToDecimal(drGarant[ReportConsts.SumField]) - Convert.ToDecimal(drGarant[9])
                     - cdo.GetSumValue(cdo.dtDetail[4], masterKey, t_S_FactPercentPrGrnt.FactDate, "Margin,Commission", DateTime.MinValue, Convert.ToDateTime(calcDate), true, false);

                drResult[4] = Convert.ToDecimal(drResult[4]) + Convert.ToDecimal(drGarant[4]);
                drResult[6] = Convert.ToDecimal(drResult[6]) + Convert.ToDecimal(drGarant[6]);
                drResult[9] = Convert.ToDecimal(drResult[9]) + Convert.ToDecimal(drGarant[9]);

                drGarant[5] = DBNull.Value;
                drGarant[8] = DBNull.Value;
                drGarant[10] = DBNull.Value;
            }
        }

        /// <summary>
        /// Структура для таблицы деталей отчета "Долговая книга Омск"
        /// </summary>
        private static DataTable CreateReportDebtorBookOmskStructure()
        {
            var dt = new DataTable("Report");
            dt.Columns.Add("F01", typeof(String));
            dt.Columns.Add("F02", typeof(String));
            dt.Columns.Add("F03", typeof(String));
            dt.Columns.Add("F04", typeof(String));
            dt.Columns.Add("F05", typeof(DateTime));
            dt.Columns.Add("F06", typeof(String));
            dt.Columns.Add("F07", typeof(DateTime));
            dt.Columns.Add("F08", typeof(String));
            dt.Columns.Add("F09", typeof(DateTime));
            dt.Columns.Add("F10", typeof(Decimal));
            dt.Columns.Add("F11", typeof(Decimal));
            dt.Columns.Add("F12", typeof(Decimal));
            dt.Columns.Add("F13", typeof(Decimal));
            dt.Columns.Add("F14", typeof(Decimal));
            dt.Columns.Add("F15", typeof(Decimal));
            return dt;
        }

        /// <summary>
        /// Сливание общего списка дат изменений
        /// </summary>
        private static Collection<string> FillDetailDateList(Collection<string> datesList, IEnumerable<DataRow> dtDetail)
        {
            var result = datesList;
            foreach (var row in dtDetail)
            {
                var dateStr = Convert.ToDateTime(row[GetDetailDateFieldName(row.Table)]).ToShortDateString();
                if (dateStr.Length > 0 && result.IndexOf(dateStr) == -1)
                {
                    if (result.Count == 0)
                    {
                        result.Add(dateStr);
                    }
                    else
                    {
                        var indexCollection = result.TakeWhile(t =>
                            Convert.ToDateTime(t) <= Convert.ToDateTime(dateStr)).Count();
                        result.Insert(indexCollection, dateStr);
                    }
                }
            }
            return result;
        }

        private static decimal MergeDataTables(Dictionary<int, decimal> okvValues, DataTable dtResult, DataTable dtSource, DataTable[,] dtDetail, int index, string[] dateBounds, bool needDetail, decimal startSum)
        {
            decimal sum = 0;
            for (var i = 0; i < GetLastRowIndex(dtSource); i++)
            {
                var drResult = dtResult.NewRow();
                drResult[0] = dtSource.Rows[i][0];
                drResult[1] = dtSource.Rows[i][1];
                drResult[2] = dtSource.Rows[i][2];
                drResult[3] = dtSource.Rows[i][3];
                drResult[4] = dtSource.Rows[i][4];
                drResult[5] = dtSource.Rows[i][5];
                drResult[6] = dtSource.Rows[i][6];
                drResult[13] = dtSource.Rows[i][7];
                
                if (dtSource.Rows[i][7] != DBNull.Value && !needDetail)
                {
                    sum += Convert.ToDecimal(dtSource.Rows[i][7]);
                }

                if (needDetail)
                {
                    var contractID = dtSource.Rows[i]["ID"].ToString();
                    const string selectStrSingleDate = "{0} = '{1}' and {2} = {3}";
                    const string selectStrDateBounds = "{0} = {1} and {2} >= '{3}' and {2} <= '{4}'";
                    var sumFieldName = ReportConsts.SumField;
                    var currencyType = Convert.ToInt32(dtSource.Rows[i]["RefOKV"]);
                    
                    if (currencyType != -1)
                    {
                        sumFieldName = ReportConsts.CurrencySumField;
                    }

                    var parentColumnName = "RefCreditInc";
                    switch (index)
                    {
                        case 0:
                            parentColumnName = "RefCap";
                            break;
                        case 3:
                            parentColumnName = "RefGrnt";
                            break;
                    }

                    var dateList = new Collection<string>();

                    if (dtDetail[index, 0] != null)
                    {
                        var dt1 = dtDetail[index, 0].Select(
                            String.Format(selectStrDateBounds,
                                parentColumnName,
                                contractID,
                                GetDetailDateFieldName(dtDetail[index, 0]),
                                dateBounds[0],
                                dateBounds[1]));
                        dateList = FillDetailDateList(dateList, dt1);
                    }
                    
                    if (dtDetail[index, 1] != null)
                    {
                        var dt2 = dtDetail[index, 1].Select(
                            String.Format(
                                selectStrDateBounds,
                                parentColumnName,
                                contractID,
                                GetDetailDateFieldName(dtDetail[index, 1]),
                                dateBounds[0],
                                dateBounds[1]));
                        dateList = FillDetailDateList(dateList, dt2);
                    }

                    if (dtDetail[index, 2] != null)
                    {
                        var dt3 = dtDetail[index, 2].Select(
                            String.Format(
                                selectStrDateBounds,
                                parentColumnName,
                                contractID,
                                GetDetailDateFieldName(dtDetail[index, 2]),
                                dateBounds[0],
                                dateBounds[1]));
                        dateList = FillDetailDateList(dateList, dt3);
                    }
                    
                    if (dateList.Count > 0)
                    {
                        dtResult.Rows.Add(drResult);
                    }

                    foreach (string t in dateList)
                    {
                        decimal sum1 = 0;
                        decimal sum2 = 0;
                        decimal sum3 = 0;

                        if (dtDetail[index, 0] != null)
                        {
                            var detail1 = dtDetail[index, 0].Select(
                                String.Format(selectStrSingleDate, GetDetailDateFieldName(dtDetail[index, 0]), t, parentColumnName, contractID));
                            sum1 += detail1.Sum(row => Convert.ToDecimal(row[sumFieldName]));
                        }

                        if (dtDetail[index, 1] != null)
                        {
                            var detail2 = dtDetail[index, 1].Select(
                                String.Format(selectStrSingleDate, GetDetailDateFieldName(dtDetail[index, 1]), t, parentColumnName, contractID));
                            sum2 += detail2.Sum(row => Convert.ToDecimal(row[sumFieldName]));
                        }

                        if (dtDetail[index, 2] != null)
                        {
                            var detail3 = dtDetail[index, 2].Select(
                                String.Format(selectStrSingleDate, GetDetailDateFieldName(dtDetail[index, 2]), t, parentColumnName, contractID));
                            sum3 = detail3.Sum(row => Convert.ToDecimal(row[ReportConsts.SumField]));
                        }

                        drResult = dtResult.Rows.Add();
                        drResult[08] = t;

                        if (currencyType != -1)
                        {
                            sum1 = okvValues[currencyType] * sum1;
                            sum2 = okvValues[currencyType] * sum2;
                        }

                        drResult[09] = sum1;
                        
                        if (index == 3)
                        {
                            drResult[11] = sum2;
                        }
                        else
                        {
                            drResult[10] = sum2;
                        }

                        if (index != 3)
                        {
                            drResult[12] = sum3;
                        }

                        sum += sum1 - sum2;
                        startSum += sum1 - sum2;
                        drResult[13] = startSum;
                    }
                }
                else
                {
                    dtResult.Rows.Add(drResult);
                }
            }
            return sum;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Структура долга"
        /// </summary>
        private static DataTable CreateReportTableDebtStructure()
        {
            var dt = new DataTable("ReportData");
            dt.Columns.Add("F00", typeof(String));
            dt.Columns.Add("F01", typeof(Decimal));
            dt.Columns.Add("F02", typeof(Decimal));
            dt.Columns.Add("F03", typeof(Decimal));
            dt.Columns.Add("F04", typeof(Decimal));
            dt.Columns.Add("F05", typeof(Decimal));
            dt.Columns.Add("F06", typeof(Decimal));
            dt.Columns.Add("F07", typeof(Decimal));
            dt.Columns.Add("F08", typeof(Decimal));
            return dt;
        }

        private static string GetDetailDateFieldName(DataTable dtDetail)
        {
            var dateFieldName = String.Empty;
            if (dtDetail.Columns.Contains("FactDate")) dateFieldName = "FactDate";
            else if (dtDetail.Columns.Contains("DateCharge")) dateFieldName = "DateCharge";
            else if (dtDetail.Columns.Contains("EndDate")) dateFieldName = "EndDate";
            else if (dtDetail.Columns.Contains("DateDoc")) dateFieldName = "DateDoc";
            else if (dtDetail.Columns.Contains("ChargeDate")) dateFieldName = "ChargeDate";
            else if (dtDetail.Columns.Contains("DateCharge")) dateFieldName = "DateCharge";
            else if (dtDetail.Columns.Contains("DateDischarge")) dateFieldName = "DateDischarge";
            else if (dtDetail.Columns.Contains("StartDate")) dateFieldName = "StartDate";

            return dateFieldName;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Источники финансирования дефицита бюджета"
        /// </summary>
        private static DataTable CreateReportTableBudgetSourceDeficit()
        {
            var dt = new DataTable("ReportData");
            dt.Columns.Add("F01", typeof(String));
            dt.Columns.Add("F02", typeof(String));
            dt.Columns.Add("F03", typeof(String));
            dt.Columns.Add("F04", typeof(String));
            dt.Columns.Add("F05", typeof(String));
            dt.Columns.Add("F06", typeof(String));
            dt.Columns.Add("F07", typeof(String));
            dt.Columns.Add("F08", typeof(String));
            dt.Columns.Add("F09", typeof(String));
            dt.Columns.Add("F10", typeof(Decimal));
            dt.Columns.Add("F11", typeof(Int32));
            dt.Columns.Add("F12", typeof(Int32));
            dt.Columns.Add("F13", typeof(String));
            return dt;
        }

        private static void AddDeficitRecord(DataRow drResult, DataRow drSource, decimal sum, bool updateSum)
        {
            var value = drSource["CodeStr"].ToString();
            var part1 = value.Substring(00, 3);
            var part2 = value.Substring(03, 2);
            var part3 = value.Substring(05, 2);
            var part4 = value.Substring(07, 2);
            var part5 = value.Substring(09, 2);
            var part6 = value.Substring(11, 2);
            var part7 = value.Substring(13, 4);
            var part8 = value.Substring(17, 3);

            drResult[0] = part1;
            drResult[1] = part2;
            drResult[2] = part3;
            drResult[3] = part4;
            drResult[4] = part5;
            drResult[5] = part6;
            drResult[6] = part7;
            drResult[7] = part8;

            drResult[8] = drSource["Name"];
            drResult[9] = 0;
            
            if (updateSum)
            {
                drResult[9] = sum;
            }
            
            drResult[10] = drSource["ID"];
            drResult[11] = drSource["ParentID"];
            drResult[12] = drSource["CodeStr"];
        }

        private static bool CheckPeriod(object periodValue, int year)
        {
            return year == Convert.ToInt32(periodValue) / 10000;
        }

        private static DateTime GetLastMonthDate(DateTime startMonthDate, DateTime endPeriodDate)
        {
            var hiBound = Convert.ToDateTime(String.Format("{0}.{1}.{2}",
                DateTime.DaysInMonth(startMonthDate.Year, startMonthDate.Month), startMonthDate.Month, startMonthDate.Year));
            
            // Если даже одного полного месяца не попадает в период
            if (DateTime.Compare(hiBound, endPeriodDate) > 0)
            {
                hiBound = endPeriodDate;
            }

            return hiBound;
        }

        private static DataTable FillSummaryDataSet(DataTable dt1, DataTable dt2, IEnumerable<int> colIndexes)
        {
            var dtTemp = dt1.Clone();
            dtTemp.Rows.Add();

            foreach (var colIndex in colIndexes)
            {
                dtTemp.Rows[0][colIndex] = GetLastRowValue(dt1, colIndex) + GetLastRowValue(dt2, colIndex);
            }

            dtTemp.AcceptChanges();
            return dtTemp;
        }

        /// <summary>
        /// Сливание общего списка дат изменений
        /// </summary>
        private static Collection<string> FillDetailDateList(Collection<string> datesList, DataTable dtDetail,
            string dateFieldName, string startDate, string endDate, string refFieldName, int masterKey)
        {
            var result = datesList;
            var drsSelect = dtDetail.Select(String.Format("{0} = {1}", refFieldName, masterKey));

            foreach (var row in drsSelect)
            {
                if (row[dateFieldName] != DBNull.Value)
                {
                    var dateStr = Convert.ToDateTime(row[dateFieldName]).ToShortDateString();

                    if (Convert.ToDateTime(dateStr) >= Convert.ToDateTime(startDate) && Convert.ToDateTime(dateStr) <= Convert.ToDateTime(endDate))
                    {
                        if (dateStr.Length > 0 && result.IndexOf(dateStr) == -1)
                        {
                            if (result.Count == 0)
                            {
                                result.Add(dateStr);
                            }
                            else
                            {
                                var indexCollection = result.TakeWhile(t => 
                                    Convert.ToDateTime(t) <= Convert.ToDateTime(dateStr)).Count();
                                result.Insert(indexCollection, dateStr);
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static string GetDetailValue(DataTable dtPercent, int masterKey, string refFieldName,
            string startDate, string endDate, string dateFieldName, string resultFieldName)
        {
            var result = String.Empty;
            var drSelect = dtPercent.Select(
                String.Format("{0} = {1} and {4} >= '{2}' and {4} <= '{3}'",
                    refFieldName, masterKey, startDate, endDate, dateFieldName),
                String.Format("{0} desc", dateFieldName));

            if (drSelect.Length > 0)
            {
                result = drSelect[0][resultFieldName].ToString();
            }

            return result;
        }

        private static void FillDetailDataList(int contractType, DataTable dtResult, DataTable dtSummary, DataTable dtContracts,
            CommonDataObject cdo, string startDate, string endDate, bool isIncome)
        {
            var detailKeys = new Dictionary<int, string>();
            var datesList = new Collection<string>();
            var monthCount = Convert.ToDateTime(endDate).Month;
            var yearStart = GetYearStart(endDate);
            int headerColumnCount;

            if (contractType == 0)
            {
                detailKeys.Add(0, "FactDate");
                detailKeys.Add(1, "FactDate");
                detailKeys.Add(5, "EndDate");
                detailKeys.Add(4, "FactDate");
                detailKeys.Add(6, "StartDate");
                detailKeys.Add(8, "FactDate");
                headerColumnCount = 7;
            }
            else if (contractType == 1)
            {
                detailKeys.Add(5, "EndDate");
                detailKeys.Add(1, "DateDischarge");
                detailKeys.Add(7, "EndDate");
                detailKeys.Add(3, "FactDate");
                detailKeys.Add(8, "StartDate");
                detailKeys.Add(9, "FactDate");
                headerColumnCount = 6;
            }
            else
            {
                detailKeys.Add(1, "FactDate");
                detailKeys.Add(4, "FactDate");
                detailKeys.Add(8, "StartDate");
                detailKeys.Add(9, "FactDate");
                headerColumnCount = 8;
            }

            var sumArray = new decimal[15];
            var prevArray = new decimal[3];
            var totalSumArray = new decimal[20];
            var flagStartSumArray = new bool[20];
            var monthSumArray = new decimal[14, 20];
            var minDate = DateTime.MinValue;

            foreach (DataRow drContract in dtContracts.Rows)
            {
                var masterKey = -1;
                
                if (drContract["ID"] != DBNull.Value)
                {
                    masterKey = Convert.ToInt32(drContract["ID"]);
                }

                datesList.Clear();

                datesList = detailKeys.Keys.Aggregate(datesList, (current, key) => 
                    FillDetailDateList(current, cdo.dtDetail[key], detailKeys[key], startDate, endDate, cdo.GetParentRefName(), masterKey));

                datesList = FillDetailDateList(datesList, cdo.dtJournalPercent, "ChargeDate",
                    startDate, endDate, cdo.GetParentRefName(), masterKey);
                var usedMonth = new Collection<int>();

                foreach (var dateStr in datesList)
                {
                    if (!usedMonth.Contains(Convert.ToDateTime(dateStr).Month))
                        usedMonth.Add(Convert.ToDateTime(dateStr).Month);
                }

                var fictiveDates = new Collection<string>();

                for (int i = 1; i <= monthCount; i++)
                {
                    if (!usedMonth.Contains(i))
                    {
                        fictiveDates.Add(
                            Convert.ToDateTime(String.Format("01.{0}.{1}", i,
                                                             Convert.ToDateTime(cdo.reportParams["EndDate"]).Year)).
                                ToShortDateString());
                    }
                }

                var realFictiveDates = new Collection<string>();

                foreach (var key in fictiveDates)
                {
                    realFictiveDates.Add(key);
                }

                datesList = new Collection<string>(fictiveDates);

                datesList = detailKeys.Keys.Aggregate(datesList, (current, key) => 
                    FillDetailDateList(current, cdo.dtDetail[key], detailKeys[key], startDate, endDate, cdo.GetParentRefName(), masterKey));

                datesList = FillDetailDateList(datesList, cdo.dtJournalPercent, "ChargeDate",
                    startDate, endDate, cdo.GetParentRefName(), masterKey);

                if (datesList.Count > 0)
                {
                    var drMaster = dtResult.Rows.Add();

                    if (contractType == 0)
                    {
                        drMaster[0] = String.Format("{0} {1} от {2} Кредитор: {3} Дата погашения: {4} Вид обеспечения: {5}",
                            drContract[0], drContract[1], Convert.ToDateTime(drContract[2]).ToShortDateString(),
                            drContract[3], drContract[4], drContract[5]);
                    }
                    else if (contractType == 1)
                    {
                        drMaster[0] = String.Format("Выпуск: {0} от {1} {2} Дата погашения: {3} Вид обеспечения: {4}",
                            drContract[0], Convert.ToDateTime(drContract[1]).ToShortDateString(),
                            drContract[2], drContract[3], drContract[4]);
                    }
                    else
                    {
                        drMaster[0] = String.Format("{0} {1} от {2} № {3} от {4} Кредитор: {5} Дата погашения: {6} Вид обеспечения: {7}",
                            drContract[0], drContract[1], Convert.ToDateTime(drContract[2]).ToShortDateString(),
                            drContract[3], drContract[4], drContract[5], drContract[6], drContract[7]);
                    }

                    for (var i = 0; i < totalSumArray.Length; i++)
                    {
                        totalSumArray[i] = 0;
                        flagStartSumArray[i] = false;
                    }

                    drMaster[drMaster.Table.Columns.Count - 1] = "1";
                    drMaster = dtResult.Rows.Add();
                    drMaster[0] = "На начало года";

                    if (contractType == 2)
                    {
                        drContract[headerColumnCount] = Convert.ToDecimal(drContract[ReportConsts.SumField])
                            - Convert.ToDecimal(drContract[headerColumnCount])
                            - cdo.GetSumValue(
                                cdo.dtDetail[4], masterKey, detailKeys[4],
                                "Margin,Commission", minDate, Convert.ToDateTime(yearStart), true, false);
                    }
                    
                    drMaster[1] = drContract[headerColumnCount];
                    drMaster[drMaster.Table.Columns.Count - 1] = "2";
                    monthSumArray[0, 1] += Convert.ToDecimal(drContract[headerColumnCount]);
                    prevArray[0] = 0; prevArray[1] = 0; prevArray[2] = 0;

                    foreach (var dateStr in datesList)
                    {
                        var maxDate = Convert.ToDateTime(dateStr);

                        for (var k = 0; k < 3; k++)
                        {
                            int detailIndex1;
                            int detailIndex2;

                            if (contractType == 0)
                            {
                                if (k == 0)
                                {
                                    detailIndex1 = 0;
                                    detailIndex2 = 1;
                                }
                                else if (k == 1)
                                {
                                    detailIndex1 = 5;
                                    detailIndex2 = 4;
                                }
                                else
                                {
                                    detailIndex1 = 6;
                                    detailIndex2 = 8;
                                }
                            }
                            else if (contractType == 1)
                            {
                                if (k == 0)
                                {
                                    detailIndex1 = 5;
                                    detailIndex2 = 1;
                                }
                                else if (k == 1)
                                {
                                    detailIndex1 = 7;
                                    detailIndex2 = 3;
                                }
                                else
                                {
                                    detailIndex1 = 8;
                                    detailIndex2 = 9;
                                }
                            }
                            else
                            {
                                if (k == 0)
                                {
                                    detailIndex1 = 1;
                                    detailIndex2 = 4;
                                }
                                else if (k == 1)
                                {
                                    detailIndex1 = 1;
                                    detailIndex2 = 4;
                                }
                                else
                                {
                                    detailIndex1 = 8;
                                    detailIndex2 = 9;
                                }
                            }

                            const string sumFieldName = ReportConsts.SumField;
                            sumArray[k * 5 + 0] = cdo.GetSumValue(
                                cdo.dtDetail[detailIndex1], masterKey, detailKeys[detailIndex1],
                                sumFieldName, minDate, maxDate, true, false);
                            sumArray[k * 5 + 1] = cdo.GetSumValue(
                                cdo.dtDetail[detailIndex2], masterKey, detailKeys[detailIndex2],
                                sumFieldName, minDate, maxDate, true, false);
                            sumArray[k * 5 + 2] = cdo.GetSumValue(
                                cdo.dtDetail[detailIndex2], masterKey, detailKeys[detailIndex2],
                                sumFieldName, minDate, maxDate, true, true);
                            sumArray[k * 5 + 3] = cdo.GetSumValue(
                                cdo.dtDetail[detailIndex1], masterKey, detailKeys[detailIndex1],
                                sumFieldName, maxDate, maxDate, true, true);
                            sumArray[k * 5 + 4] = cdo.GetSumValue(
                                cdo.dtDetail[detailIndex2], masterKey, detailKeys[detailIndex2],
                                sumFieldName, maxDate, maxDate, true, true);

                            if (contractType == 2)
                            {
                                if (k == 0)
                                {
                                    sumArray[k * 5 + 0] = Convert.ToDecimal(drContract[ReportConsts.SumField]);
                                    sumArray[k * 5 + 1] =
                                         cdo.GetSumValue(
                                            cdo.dtDetail[detailIndex1], masterKey, detailKeys[detailIndex1],
                                            ReportConsts.SumField, minDate, maxDate, true, false)
                                    + cdo.GetSumValue(
                                        cdo.dtDetail[detailIndex2], masterKey, detailKeys[detailIndex2],
                                        "Sum,Margin,Commission", minDate, maxDate, true, false);
                                    sumArray[k * 5 + 2] = Convert.ToDecimal(drContract[ReportConsts.SumField]);
                                    sumArray[k * 5 + 3] =
                                         -cdo.GetSumValue(
                                            cdo.dtDetail[detailIndex1], masterKey, detailKeys[detailIndex1],
                                            ReportConsts.SumField, maxDate, maxDate, true, true)
                                    - cdo.GetSumValue(
                                        cdo.dtDetail[detailIndex2], masterKey, detailKeys[detailIndex2],
                                        "Sum,Margin,Commission", maxDate, maxDate, true, true);
                                    sumArray[k * 5 + 4] = 0;
                                }
                                else
                                {
                                    sumArray[k * 5 + 0] = 0;
                                    sumArray[k * 5 + 1] = 0;
                                    sumArray[k * 5 + 2] = 0;
                                    sumArray[k * 5 + 3] = 0;
                                    sumArray[k * 5 + 4] = 0;
                                }
                            }

                            if (!flagStartSumArray[maxDate.Month])
                            {
                                monthSumArray[maxDate.Month, k * 6 + 1] +=
                                    sumArray[k * 5 + 0] - sumArray[k * 5 + 1];
                            }

                            monthSumArray[maxDate.Month, k * 6 + 2] += sumArray[k * 5 + 3];
                            monthSumArray[maxDate.Month, k * 6 + 3] += sumArray[k * 5 + 4];

                            var endSum = sumArray[k * 5 + 0] - sumArray[k * 5 + 1] + sumArray[k * 5 + 3] - sumArray[k * 5 + 4];

                            if (!flagStartSumArray[maxDate.Month])
                            {
                                monthSumArray[maxDate.Month, k * 6 + 4] += endSum;
                            }
                            else
                            {
                                monthSumArray[maxDate.Month, k * 6 + 4] +=
                                     endSum - prevArray[k];
                            }

                            prevArray[k] = endSum;
                            flagStartSumArray[maxDate.Month] = flagStartSumArray[maxDate.Month] || k == 2;
                        }

                        drMaster = dtResult.Rows.Add();
                        drMaster[0] = dateStr;

                        for (var k = 0; k < 3; k++)
                        {
                            drMaster[k * 6 + 1] = sumArray[k * 5 + 0] - sumArray[k * 5 + 1];
                            totalSumArray[k * 6 + 1] = sumArray[k * 5 + 0] - sumArray[k * 5 + 1];

                            drMaster[k * 6 + 2] = sumArray[k * 5 + 3];
                            totalSumArray[k * 6 + 2] += sumArray[k * 5 + 3];

                            drMaster[k * 6 + 3] = sumArray[k * 5 + 4];
                            totalSumArray[k * 6 + 3] += sumArray[k * 5 + 4];

                            drMaster[k * 6 + 4] = sumArray[k * 5 + 0] - sumArray[k * 5 + 1] + sumArray[k * 5 + 3] - sumArray[k * 5 + 4];
                            totalSumArray[k * 6 + 4] = sumArray[k * 5 + 0] - sumArray[k * 5 + 1] + sumArray[k * 5 + 3] - sumArray[k * 5 + 4];
                        }

                        if (contractType == 0 || contractType == 2)
                        {
                            drMaster[6] = GetDetailValue(cdo.dtJournalPercent, masterKey,
                                cdo.GetParentRefName(), DateTime.MinValue.ToShortDateString(),
                                maxDate.ToShortDateString(), "ChargeDate", "CreditPercent");
                        }
                        else
                        {
                            drMaster[6] = drContract["Discount"];
                        }

                        if (contractType == 0)
                        {
                            if (!isIncome)
                            {
                                drMaster[12] = GetDetailValue(cdo.dtJournalPercent, masterKey,
                                    cdo.GetParentRefName(), DateTime.MinValue.ToShortDateString(),
                                    maxDate.ToShortDateString(), "ChargeDate", "PenaltyDebt");

                                if (drMaster[12].ToString() == String.Empty)
                                    drMaster[12] = drContract["PenaltyDebtRate"];
                            }
                            else
                            {
                                drMaster[12] = GetDetailValue(cdo.dtDetail[6], masterKey,
                                    cdo.GetParentRefName(), DateTime.MinValue.ToShortDateString(),
                                    maxDate.ToShortDateString(), "StartDate", "Penalty");
                            }
                        }
                        else if (contractType == 1)
                        {
                            drMaster[12] = GetDetailValue(cdo.dtJournalPercent, masterKey,
                                cdo.GetParentRefName(), DateTime.MinValue.ToShortDateString(),
                                maxDate.ToShortDateString(), "ChargeDate", "PenaltyCapital");
                        }

                        drMaster[5] = 0;
                        drMaster[11] = 0;
                        drMaster[17] = 0;

                        drMaster[drMaster.Table.Columns.Count - 1] = "2";
                    }

                    if (contractType == 0)
                    {
                        var deletedRows = new Collection<int>();
                        var prvMonth = -1;
                        var summaryRowNum = -1;
                        var outFactPercent = true;

                        for (var k = 0; k < datesList.Count; k++)
                        {
                            var curMonth = Convert.ToDateTime(datesList[datesList.Count - k - 1]).Month;
                            var rowNum = dtResult.Rows.Count - 1 - k;

                            if (curMonth == prvMonth)
                            {
                                if (outFactPercent && Math.Abs(Convert.ToDecimal(dtResult.Rows[summaryRowNum][09])) > 0)
                                    outFactPercent = false;
                                var checkValues = Convert.ToDecimal(dtResult.Rows[rowNum][02]) > 0;
                                checkValues = checkValues || Convert.ToDecimal(dtResult.Rows[rowNum][03]) > 0;
                                checkValues = checkValues || Convert.ToDecimal(dtResult.Rows[rowNum][14]) > 0;
                                checkValues = checkValues || Convert.ToDecimal(dtResult.Rows[rowNum][15]) > 0;

                                dtResult.Rows[summaryRowNum][07] = Convert.ToDecimal(dtResult.Rows[rowNum][07]);
                                dtResult.Rows[summaryRowNum][08] =
                                    Convert.ToDecimal(dtResult.Rows[summaryRowNum][08]) + Convert.ToDecimal(dtResult.Rows[rowNum][08]);
                                dtResult.Rows[summaryRowNum][09] =
                                    Convert.ToDecimal(dtResult.Rows[summaryRowNum][09]) + Convert.ToDecimal(dtResult.Rows[rowNum][09]);
                                dtResult.Rows[summaryRowNum][10] = Convert.ToDecimal(dtResult.Rows[summaryRowNum][07]) +
                                    Convert.ToDecimal(dtResult.Rows[summaryRowNum][08]) - Convert.ToDecimal(dtResult.Rows[summaryRowNum][09]);

                                if (outFactPercent && Math.Abs(Convert.ToDecimal(dtResult.Rows[rowNum][09])) > 0)
                                {
                                    dtResult.Rows[summaryRowNum][0] = dtResult.Rows[rowNum][0];
                                    outFactPercent = false;
                                }

                                var checkPercent = Convert.ToDecimal(dtResult.Rows[rowNum][08]) > 0;
                                checkPercent = checkPercent || Convert.ToDecimal(dtResult.Rows[rowNum][09]) > 0;

                                dtResult.Rows[rowNum][08] = 0;
                                dtResult.Rows[rowNum][09] = 0;
                                dtResult.Rows[rowNum][10] = dtResult.Rows[rowNum][07];

                                if (!checkValues && checkPercent)
                                {
                                    deletedRows.Add(rowNum);
                                }
                            }
                            else
                            {
                                summaryRowNum = rowNum;
                                outFactPercent = true;
                            }

                            if (realFictiveDates.Contains(datesList[datesList.Count - k - 1]) && !deletedRows.Contains(rowNum))
                                deletedRows.Add(rowNum);

                            prvMonth = curMonth;
                        }

                        foreach (var rowIndex in deletedRows)
                        {
                            dtResult.Rows.RemoveAt(rowIndex);
                        }
                    }
                    else
                    {
                        var deletedRows = new Collection<int>();
                        for (var k = 0; k < datesList.Count; k++)
                        {
                            var rowNum = dtResult.Rows.Count - 1 - k;
                            if (realFictiveDates.Contains(datesList[datesList.Count - k - 1]))
                            {
                                deletedRows.Add(rowNum);
                            }
                        }

                        foreach (var rowIndex in deletedRows)
                        {
                            dtResult.Rows.RemoveAt(rowIndex);
                        }
                    }


                    if (datesList.Count == realFictiveDates.Count)
                    {
                        drMaster = dtResult.Rows.Add();
                        drMaster[drMaster.Table.Columns.Count - 1] = "2";
                    }

                    drMaster = dtResult.Rows.Add();
                    drMaster[0] = "Итого:";
                    drMaster[1] = "X";

                    for (var k = 2; k < totalSumArray.Length; k++)
                    {
                        drMaster[k] = totalSumArray[k];
                    }
                    drMaster[7] = "X";
                    drMaster[13] = "X";

                    drMaster[05] = 0;
                    drMaster[06] = DBNull.Value;
                    drMaster[11] = 0;
                    drMaster[12] = DBNull.Value;
                    drMaster[17] = 0;

                    drMaster[drMaster.Table.Columns.Count - 1] = "3";
                }
            }

            var drSummary = dtSummary.Rows.Add();
            drSummary[0] = "На начало года:";
            drSummary[1] = monthSumArray[0, 1];

            for (var i = 0; i < totalSumArray.Length; i++)
            {
                totalSumArray[i] = 0;
            }

            var prevMonth = new decimal[3];

            for (var i = 1; i < 13; i++)
            {
                drSummary = dtSummary.Rows.Add();
                if (i == 1)
                {
                    prevMonth[0] = monthSumArray[i, 1];
                    prevMonth[1] = monthSumArray[i, 7];
                    prevMonth[2] = monthSumArray[i, 13];
                }

                for (var k = 0; k < 3; k++)
                {
                    drSummary[k * 6 + 1] = prevMonth[k];
                    drSummary[k * 6 + 2] = monthSumArray[i, k * 6 + 2];
                    drSummary[k * 6 + 3] = monthSumArray[i, k * 6 + 3];
                    drSummary[k * 6 + 4] = prevMonth[k] + monthSumArray[i, k * 6 + 2] - monthSumArray[i, k * 6 + 3];
                    drSummary[k * 6 + 5] = 0;

                    if (i == monthCount) totalSumArray[k * 6 + 1] += Convert.ToDecimal(drSummary[k * 6 + 1]);
                    totalSumArray[k * 6 + 2] += monthSumArray[i, k * 6 + 2];
                    totalSumArray[k * 6 + 3] += monthSumArray[i, k * 6 + 3];
                    if (i == monthCount) totalSumArray[k * 6 + 4] += Convert.ToDecimal(drSummary[k * 6 + 4]);

                    prevMonth[k] = Convert.ToDecimal(drSummary[k * 6 + 4]);
                }
            }

            drSummary = dtSummary.Rows.Add();
            drSummary[0] = "Итого:";

            for (var k = 0; k < 3; k++)
            {
                if (k <= monthCount) drSummary[k * 6 + 1] = totalSumArray[k * 6 + 1];
                drSummary[k * 6 + 2] = totalSumArray[k * 6 + 2];
                drSummary[k * 6 + 3] = totalSumArray[k * 6 + 3];
                
                if (k <= monthCount)
                {
                    drSummary[k*6 + 4] = totalSumArray[k*6 + 4];
                }

                drSummary[k * 6 + 5] = 0;
            }

            drSummary[1] = "X";
            drSummary[7] = "X";
            drSummary[13] = "X";

            drSummary[06] = DBNull.Value;
            drSummary[12] = DBNull.Value;
        }

        private static void FillSummaryData(DataTable dt1, DataTable dt2)
        {
            for (var i = 1; i < dt1.Columns.Count; i++)
            {
                for (var j = 0; j < dt1.Rows.Count; j++)
                {
                    if (dt2.Rows[j][i] != DBNull.Value)
                    {
                        if (dt2.Rows[j][i].ToString() != "X")
                        {
                            dt1.Rows[j][i] =
                                Convert.ToDecimal(dt1.Rows[j][i]) +
                                Convert.ToDecimal(dt2.Rows[j][i]);
                        }
                    }
                }
            }
        }

        private static void AddRows(DataTable dtResult, int rowCount, string filler)
        {
            for (var i = 0; i < rowCount; i++)
            {
                var drAdd = dtResult.Rows.Add();

                for (var j = 0; j < dtResult.Columns.Count; j++)
                {
                    drAdd[j] = filler;
                }
            }
        }
    }
}