using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.DebtBook;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Planning.Data;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// МО - Планы обслуживания
        /// </summary>
        public DataTable[] GetMOMOPlanServicesData(Dictionary<string, string> reportParams)
        {
            const int resultColumnCount = 6;
            const string fieldSplitter = "=";
            var dtTables = new DataTable[2];
            var regNum = reportParams[ReportConsts.ParamRegNum];
            var regNumFilter = GetMOSelectedRegNumFilter(regNum);
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.mainFilter[f_S_Creditincome.Num] = regNumFilter;
            cdoCredit.useSummaryRow = false;
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditNumContractDateOrg);
            cdoCredit.AddDataColumn(f_S_Creditincome.id);
            var tblCredit = cdoCredit.FillData();
            var rowCredit = GetLastRow(tblCredit);
            var rowCaption = CreateReportParamsRow(dtTables);
            rowCaption[1] = 0;
            rowCaption[2] = 0;
            var tblResult = CreateReportCaptionTable(resultColumnCount + 1);

            if (rowCredit != null)
            {
                var masterKey = Convert.ToString(rowCredit[f_S_Creditincome.id]);
                var masterRef = cdoCredit.GetParentRefName();

                // Изменение
                var alterKeys = new Collection<int>();
                var objCreditAlter = new CommonDataObject();
                objCreditAlter.InitObject(scheme);
                objCreditAlter.ObjectKey = t_S_AlterationCl.internalKey;
                objCreditAlter.useSummaryRow = false;
                objCreditAlter.mainFilter[masterRef] = masterKey;
                objCreditAlter.AddDataColumn(t_S_AlterationCl.ID);
                objCreditAlter.AddParamColumn(
                    CalcColumnType.cctCommonBookValue,
                    t_S_AlterationCl.RefKindDocs,
                    fx_S_KindDocs.internalKey,
                    fx_S_KindDocs.Name,
                    String.Empty);
                objCreditAlter.AddDataColumn(t_S_AlterationCl.Num);
                objCreditAlter.AddDataColumn(t_S_AlterationCl.DocDate, ReportConsts.ftDateTime);
                var tblAlter = objCreditAlter.FillData();

                // Выбираем все планы
                var dataObj = new CommonDataObject();
                dataObj.InitObject(scheme);
                dataObj.useSummaryRow = false;
                dataObj.ObjectKey = t_S_PlanServiceCI.internalKey;
                dataObj.mainFilter[masterRef] = masterKey;
                dataObj.AddDataColumn(t_S_PlanServiceCI.StartDate, ReportConsts.ftDateTime);
                dataObj.AddDataColumn(t_S_PlanServiceCI.EndDate);
                dataObj.AddDataColumn(t_S_PlanServiceCI.DebtSum);
                dataObj.AddDataColumn(t_S_PlanServiceCI.DayCount);
                dataObj.AddDataColumn(t_S_PlanServiceCI.CreditPercent);
                dataObj.AddDataColumn(t_S_PlanServiceCI.Sum);
                // служебные
                dataObj.AddDataColumn(t_S_PlanServiceCI.EstimtDate, ReportConsts.ftDateTime);
                dataObj.AddDataColumn(t_S_PlanServiceCI.CalcDate, ReportConsts.ftDateTime);
                dataObj.AddDataColumn(t_S_PlanServiceCI.CalcComment);
                dataObj.sortString = FormSortString(
                    StrSortUp(t_S_PlanServiceCI.EstimtDate), 
                    StrSortUp(t_S_PlanServiceCI.CalcDate));
                var tblPlanService = dataObj.FillData();

                var planVariantList = new Collection<string>();

                foreach (DataRow rowPlan in tblPlanService.Rows)
                {
                    var planVariant = Combine(
                        rowPlan[t_S_PlanServiceCI.EstimtDate],
                        rowPlan[t_S_PlanServiceCI.CalcComment], 
                        fieldSplitter);
                    
                    if (!planVariantList.Contains(planVariant))
                    {
                        planVariantList.Add(planVariant);
                    }
                }

                rowCaption[0] = rowCredit[0];
                rowCaption[1] = planVariantList.Count;
                var planInfoOffset = 10;
                var variantCounter = 1;
                var maxDepth = 0;
                var lastPlanDate = DateTime.MinValue.ToShortDateString();

                foreach (var planVariant in planVariantList)
                {
                    string[] planVariantParams = planVariant.Split(fieldSplitter[0]);
                    string planDate = DateTime.MaxValue.ToShortDateString();

                    if (planVariantParams[0].Length > 0)
                    {
                        planDate = Convert.ToDateTime(planVariantParams[0]).ToShortDateString();
                    }

                    var alterRows = tblAlter.Select(
                        String.Format("{0} <= '{1}' and {0} > '{2}'", t_S_AlterationCl.DocDate, planDate, lastPlanDate),
                        StrSortDown(t_S_AlterationCl.DocDate));
                    DataRow selectedAlter = null;
                    var alterIndex = planInfoOffset + planVariantList.Count;
                    rowCaption[alterIndex] = String.Empty;
                    lastPlanDate = planDate;

                    foreach (var alterRow in alterRows)
                    {
                        var alterKey = Convert.ToInt32(alterRow[t_S_AlterationCl.ID]);
                        if (alterKeys.Contains(alterKey) || selectedAlter != null) continue;
                        selectedAlter = alterRow;
                        rowCaption[alterIndex] = String.Format("{0} №{1} от {2:d}", 
                            alterRow[1], alterRow[2], alterRow[3]);
                    }

                    var filterPlan = String.Format("{0} ='{1}' and {2} = '{3}'",
                        t_S_PlanServiceCI.EstimtDate,
                        planDate,
                        t_S_PlanServiceCI.CalcComment,
                        planVariantParams[1]);

                    if (planVariantParams[0].Length == 0)
                    {
                        filterPlan = String.Format("{0} is null", t_S_PlanServiceCI.EstimtDate);
                    }

                    var orderStr = FormSortString(
                        StrSortUp(t_S_PlanServiceCI.EstimtDate),
                        StrSortUp(t_S_PlanServiceCI.CalcDate),
                        StrSortUp(t_S_PlanServiceCI.StartDate));

                    rowCaption[planInfoOffset++] = planVariantParams[1];
                    var tblCurrentPlan = DataTableUtils.FilterDataSet(tblPlanService, filterPlan);
                    tblCurrentPlan = DataTableUtils.SortDataSet(tblCurrentPlan, orderStr);

                    decimal sum = 0;
                    decimal sumDebt = 0;

                    foreach (DataRow rowPlan in tblCurrentPlan.Rows)
                    {
                        var rowResult = tblResult.Rows.Add();
                        rowResult[0] = Convert.ToDateTime(rowPlan[0]).ToShortDateString();

                        for (int i = 1; i < resultColumnCount; i++)
                        {
                            rowResult[i] = rowPlan[i];
                        }

                        sum += GetNumber(rowPlan[5]);
                        sumDebt += GetNumber(rowPlan[2]);

                        rowResult[resultColumnCount] = variantCounter;
                    }

                    var rowSummary = tblResult.Rows.Add();
                    rowSummary[2] = sumDebt;
                    rowSummary[5] = sum;
                    rowSummary[resultColumnCount] = variantCounter;

                    maxDepth = Math.Max(maxDepth, tblCurrentPlan.Rows.Count + 1);
                    variantCounter++;
                }

                rowCaption[2] = maxDepth;
            }

            dtTables[0] = tblResult;
            return dtTables;
        }

        /// <summary>
        /// МО - Карточка учета долга по кредиту
        /// </summary>
        public DataTable[] GetCreditDebtHistoryMOData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[11];
            const string templatePlanFilter = "{0} = '{1}'";
            const string templateDebtFilter = "{0} = '{1}'";
            var regNum = reportParams[ReportConsts.ParamRegNum];
            var regNumFilter = GetMOSelectedRegNumFilter(regNum);
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();

            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.mainFilter[f_S_Creditincome.Num] = regNumFilter;
            cdoCredit.useSummaryRow = false;
            cdoCredit.onlyLastPlanDebt = false;
            // Формирование колонок
            cdoCredit.AddDataColumn(f_S_Creditincome.RegNum);
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCredit.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoCredit.AddDataColumn(f_S_Creditincome.Num);
            cdoCredit.AddDataColumn(f_S_Creditincome.ContractDate);
            cdoCredit.AddCalcColumn(CalcColumnType.cctOKVFullName);
            cdoCredit.AddCalcColumn(CalcColumnType.cctPercentTextSplitter);
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditPeriodRate);
            cdoCredit.AddDataColumn(f_S_Creditincome.Commission);
            cdoCredit.AddDataColumn(f_S_Creditincome.Sum);
            cdoCredit.AddDataColumn(f_S_Creditincome.EndDate);
            // служебные
            cdoCredit.AddDataColumn(f_S_Creditincome.CurrencySum);
            cdoCredit.AddDataColumn(f_S_Creditincome.RefOKV);
            cdoCredit.AddDataColumn(f_S_Creditincome.id); 
            cdoCredit.AddDetailColumn(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}]",
                t_S_PlanAttractCI.key,
                t_S_PlanDebtCI.key,
                t_S_FactAttractCI.key,
                t_S_FactDebtCI.key,
                t_S_PlanServiceCI.key,
                t_S_FactPercentCI.key,
                t_S_FactPenaltyPercentCI.key));
            // Заполнение данными
            dtTables[0] = cdoCredit.FillData();

            var rowCredit = GetLastRow(dtTables[0]);

            if (rowCredit != null)
            {
                var planIndex1 = reportParams[ReportConsts.ParamCreditPlanFilter1];
                var planIndex2 = reportParams[ReportConsts.ParamCreditPlanFilter2];
                var planDebtVariant = String.Empty;
                var planDebtList = new ParamPlanDebt().GetValuesList(rowCredit[f_S_Creditincome.id]);

                if (planDebtList.Count > 0)
                {
                    planDebtVariant = planDebtList[0];
                }

                var sumField = ReportConsts.SumField;
                if (Convert.ToInt32(rowCredit[12]) != ReportConsts.codeRUB)
                {
                    rowCredit[9] = rowCredit[11];
                    sumField = ReportConsts.CurrencySumField;
                }

                var datesList = new Collection<string>();
                var loDate = DateTime.MinValue.ToShortDateString();
                var hiDate = calcDate;
                var masterKey = Convert.ToInt32(rowCredit[f_S_Creditincome.id]);
                var refField = cdoCredit.GetParentRefName();

                // 1. Формирование таблицы «Движение основного долга»

                // 1.1 Формирование таблицы «План»

                // список изменений по плану привлечения
                var detailIndex1 = Convert.ToInt32(t_S_PlanAttractCI.key);
                var tblDetail1 = cdoCredit.dtDetail[detailIndex1];
                var dateField1 = t_S_PlanAttractCI.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                // список изменений по плану погашения
                var detailIndex2 = Convert.ToInt32(t_S_PlanDebtCI.key);
                var tblDetail2 = cdoCredit.dtDetail[detailIndex2];
                var dateField2 = t_S_PlanDebtCI.EndDate;

                if (planDebtVariant.Length > 0)
                {
                    var filterStr = String.Format(
                        templateDebtFilter,
                        t_S_PlanServiceCI.EstimtDate,
                        planDebtVariant);
                    tblDetail2 = DataTableUtils.FilterDataSet(tblDetail2, filterStr);
                }

                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, maxDate, refField, masterKey);

                var tableDebtMoving = CreateReportCaptionTable(10);
                foreach (string t in datesList)
                {
                    var rowDebt = tableDebtMoving.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[0] = t;
                    rowDebt[3] = cdoCredit.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 8, cdoCredit.sumIncludedRows, t_S_PlanAttractCI.Note);
                    rowDebt[5] = cdoCredit.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 8, cdoCredit.sumIncludedRows, t_S_PlanDebtCI.Note);
                }

                dtTables[1] = tableDebtMoving;

                // 1.2 Формирование таблицы «Факт»

                datesList.Clear();
                // список изменений по факту привлечения
                detailIndex1 = Convert.ToInt32(t_S_FactAttractCI.key);
                tblDetail1 = cdoCredit.dtDetail[detailIndex1];
                dateField1 = t_S_FactAttractCI.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, hiDate, refField, masterKey);
                // список изменений по факту погашения
                detailIndex2 = Convert.ToInt32(t_S_FactDebtCI.key);
                tblDetail2 = cdoCredit.dtDetail[detailIndex2];
                dateField2 = t_S_FactDebtCI.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);

                var tableDebtFact = CreateReportCaptionTable(11);
                foreach (var t in datesList)
                {
                    var rowDebt = tableDebtFact.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[0] = t;
                    rowDebt[3] = cdoCredit.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 7, cdoCredit.sumIncludedRows, t_S_FactAttractCI.Note);
                    CombineCellValues(rowDebt, 9, cdoCredit.sumIncludedRows, t_S_FactAttractCI.NumPayOrder);
                    rowDebt[5] = cdoCredit.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 7, cdoCredit.sumIncludedRows, t_S_FactDebtCI.Note);
                    CombineCellValues(rowDebt, 9, cdoCredit.sumIncludedRows, t_S_FactDebtCI.NumPayOrder);
                }

                dtTables[2] = tableDebtFact;

                // 2. Формирование раздела «Проценты»

                // 2.1 Формирование таблицы «План»

                datesList.Clear();
                var detailIndex3 = Convert.ToInt32(t_S_PlanServiceCI.key);
                var tblDetail3 = cdoCredit.dtDetail[detailIndex3];

                if (planIndex1.Length > 0)
                {
                    var filterStr = String.Format(templatePlanFilter, t_S_PlanServiceCI.EstimtDate, planIndex1);
                    tblDetail3 = DataTableUtils.FilterDataSet(tblDetail3, filterStr);
                }

                const string dateField3 = t_S_PlanServiceCI.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail3, dateField3, loDate, maxDate, refField, masterKey);
                var tablePercentPlan = CreateReportCaptionTable(9);
                foreach (string t in datesList)
                {
                    var rowDebt = tablePercentPlan.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[4] = cdoCredit.GetSumValue(tblDetail3, masterKey, dateField3, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 0, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.StartDate, true);
                    CombineCellValues(rowDebt, 1, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.EndDate, true);
                    CombineCellValues(rowDebt, 2, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.DayCount, true);
                    CombineCellValues(rowDebt, 6, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.PaymentDate, true);
                    CombineCellValues(rowDebt, 7, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.Note);
                }

                dtTables[3] = tablePercentPlan;

                // 2.2 Формирование таблицы «Факт»
                // список изменений по факту привлечения
                datesList.Clear();
                detailIndex1 = Convert.ToInt32(t_S_PlanServiceCI.key);
                tblDetail1 = cdoCredit.dtDetail[detailIndex1];

                if (planIndex2.Length > 0)
                {
                    var filterStr = String.Format(templatePlanFilter, t_S_PlanServiceCI.EstimtDate, planIndex2);
                    tblDetail1 = DataTableUtils.FilterDataSet(tblDetail1, filterStr);
                }

                dateField1 = t_S_PlanServiceCI.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                var tablePercentAttr = CreateReportCaptionTable(5);
                foreach (string t in datesList)
                {
                    var rowDebt = tablePercentAttr.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[3] = cdoCredit.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 0, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.StartDate, true);
                    CombineCellValues(rowDebt, 1, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.EndDate, true);
                    CombineCellValues(rowDebt, 2, cdoCredit.sumIncludedRows, t_S_PlanServiceCI.DayCount, true);
                }

                dtTables[4] = tablePercentAttr;

                // список изменений по факту погашения
                datesList.Clear();
                detailIndex2 = Convert.ToInt32(t_S_FactPercentCI.key);
                tblDetail2 = cdoCredit.dtDetail[detailIndex2];
                dateField2 = t_S_FactPercentCI.FactDate;
                tblDetail2 = DataTableUtils.FilterDataSet(tblDetail2, String.Format("{0} is not null and {1} is not null", t_S_FactPercentCI.FactDate, t_S_FactPercentCI.Sum));
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);

                var tablePercentDebt = CreateReportCaptionTable(6);
                foreach (string t in datesList)
                {
                    var rowDebt = tablePercentDebt.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[0] = t;
                    rowDebt[1] = cdoCredit.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 02, cdoCredit.sumIncludedRows, t_S_FactPercentCI.Note);
                    CombineCellValues(rowDebt, 04, cdoCredit.sumIncludedRows, t_S_FactPercentCI.NumPayOrder);
                }

                dtTables[5] = tablePercentDebt;

                // 2.3 Формирование таблицы «Комиссии»

                dtTables[6] = CreateReportCaptionTable(1);

                // 2.4 Формирование таблицы «Санкции»

                // список изменений по факту погашения
                datesList.Clear();
                detailIndex1 = Convert.ToInt32(t_S_FactPenaltyPercentCI.key);
                tblDetail1 = cdoCredit.dtDetail[detailIndex1];
                dateField1 = t_S_FactPenaltyPercentCI.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, hiDate, refField, masterKey);

                var tablePeniPercent = CreateReportCaptionTable(14);
                foreach (var t in datesList)
                {
                    var rowPeni = tablePeniPercent.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowPeni[0] = "Пени";
                    rowPeni[2] = t;
                    rowPeni[4] = cdoCredit.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    rowPeni[6] = t;
                    rowPeni[7] = rowPeni[4];
                    CombineCellValues(rowPeni, 08, cdoCredit.sumIncludedRows, t_S_FactPenaltyPercentCI.Note);
                    CombineCellValues(rowPeni, 11, cdoCredit.sumIncludedRows, t_S_FactPenaltyPercentCI.NumPayOrder);
                }

                dtTables[7] = tablePeniPercent;
                dtTables[8] = CreateReportCaptionTable(1);
                dtTables[9] = CreateReportCaptionTable(1);
            }
            else
            {
                for (var i = 1; i < dtTables.Length - 1; i++)
                {
                    dtTables[i] = CreateReportCaptionTable(50);
                }
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = regNum;
            FillSignatureData(drCaption, 4, reportParams, ReportConsts.ParamExecutor1, ReportConsts.ParamExecutor2, ReportConsts.ParamExecutor3);
            return dtTables;
        }

        /// <summary>
        /// Московская обсласть - Кредиты и бюджетные ссуды
        /// </summary>
        public DataTable[] GetMOCreditInfoData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var variantFilter = ReportConsts.FixedVariantsID;
            
            if (reportParams[ReportConsts.ParamVariantType] == "i1")
            {
                variantFilter = ReportConsts.ActiveVariantID;
            }
            
            var year = Convert.ToDateTime(calcDate).Year;
            // Данные по ИФ 
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.onlyLastPlanService = true;
            cdo.onlyLastPlanDebt = true;
            cdo.planServiceDate = calcDate;
            cdo.planDebtDate = calcDate;
            cdo.mainFilter[f_S_Creditincome.RefVariant] = variantFilter;
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.AllCreditsCode;
            cdo.onlyLastPlanService = true;
            cdo.planServiceDate = calcDate;
            // 00
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 01
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 02
            cdo.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 03
            cdo.AddDataColumn(f_S_Creditincome.Sum);
            // 04
            cdo.AddDetailColumn(String.Format("+-[0](1<={0})[1](1<={0})[10](1<={0})", calcDate));
            // 05
            cdo.AddDetailTextColumn(String.Format("[0](1<={0})", calcDate), cdo.ParamOnlyDates, "1");
            // 06
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 07
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебные
            // 08
            cdo.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            // 09
            cdo.AddCalcColumn(CalcColumnType.cctContractDateNum);
            // 10
            cdo.AddDetailTextColumn(String.Format("[2](1>{0})", calcDate), String.Empty, String.Empty);
            // 11
            cdo.AddDetailTextColumn(String.Format("[1](1<={0})", calcDate), cdo.ParamOnlyDates, String.Empty);
            // 12
            cdo.AddParamColumn(CalcColumnType.cctRecordCount, "2");
            // 13
            cdo.AddDetailTextColumn(String.Format("[2](1<={0})", maxDate), cdo.ParamOnlyDates, String.Empty);
            // 14
            cdo.AddDataColumn(f_S_Creditincome.EndDate);
            // 15
            cdo.AddDataColumn(f_S_Creditincome.RenewalDate);
            // 16
            cdo.AddDataColumn(f_S_Creditincome.RefSTypeCredit);
            // 17
            cdo.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.SortStatus);
            // 18
            cdo.AddDetailColumn(String.Format("[2](1<={0})", DateTime.MaxValue.ToShortDateString()));
            cdo.sortString = FormSortString(
                StrSortUp(TempFieldNames.SortStatus),
                StrSortDown(f_S_Creditincome.RefSTypeCredit),
                StrSortUp(TempFieldNames.OrgName));
            cdo.summaryColumnIndex.Add(3);
            dtTables[0] = cdo.FillData();

            decimal activeContractSum1 = 0;
            decimal activeContractSum2 = 0;
            for (var i = 0; i < dtTables[0].Rows.Count - 1; i++)
            {
                var rowData = dtTables[0].Rows[i];
                // сперва сортировка по 0 остатку
                rowData[TempFieldNames.SortStatus] = 1;
                if (GetNumber(rowData[4]) == 0)
                {
                    rowData[TempFieldNames.SortStatus] = 0;
                    rowData[7] = rowData[11];
                }
                else
                {
                    activeContractSum1 += GetNumber(rowData[3]);
                    activeContractSum2 += GetNumber(rowData[4]);
                }

                var attractDates = rowData[5].ToString();
                if (attractDates != String.Empty)
                {
                    rowData[5] = attractDates.Split(',')[0].Trim();
                }

                // колонка данных контракта
                rowData[1] = String.Format("Бюджетный кредит {0}", rowData[9]);
                if (Convert.ToInt32(rowData[f_S_Creditincome.RefSTypeCredit]) == 0)
                {
                    rowData[1] = rowData[8];
                }

                if (Convert.ToString(rowData[10]).Length > 0)
                {
                    rowData[6] = rowData[10];
                }
                else
                {
                    rowData[6] = rowData[f_S_Creditincome.RenewalDate] == DBNull.Value ? 
                        rowData[f_S_Creditincome.EndDate] :
                        rowData[f_S_Creditincome.RenewalDate];
                }
            }

            GetLastRow(dtTables[0])[TempFieldNames.SortStatus] = 2;
            dtTables[0] = cdo.SortDataSet(dtTables[0], cdo.sortString);

            // Данные по ИФ 
            var yearStart = GetYearStart(year);
            var yearEnd = GetYearEnd(year);
            cdo.InitObject(scheme);
            cdo.onlyLastPlanService = true;
            cdo.onlyLastPlanDebt = true;
            cdo.planServiceDate = calcDate;
            cdo.planDebtDate = calcDate;
            cdo.mainFilter[f_S_Creditincome.RefVariant] = variantFilter;
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.AllCreditsCode;
            cdo.AddDataColumn(f_S_Creditincome.Sum);
            cdo.AddDetailColumn(String.Format("+-[0](1<={0})[1](1<={0})[10](1<={0})", calcDate));
            cdo.AddDetailColumn(String.Format("[0](1<={0})", calcDate));
            cdo.AddDetailColumn(String.Format("[1](1<={0})", calcDate));
            cdo.AddDetailColumn(String.Format("[0](1>={0}1<={1})", yearStart, yearEnd));
            cdo.AddDetailColumn(String.Format("[1](1>={0}1<={1})", yearStart, yearEnd));
            cdo.summaryColumnIndex.Add(0);
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            var dtCredotOrg = cdo.FillData();
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var dtCredotBud = cdo.FillData();

            dtTables[1] = CreateReportCaptionTable(2, 2);
            var drBudCredit = dtTables[1].Rows[0];
            drBudCredit[0] = GetLastRow(dtCredotBud)[0];
            drBudCredit[1] = GetLastRow(dtCredotBud)[1];
            var drOrgCredit = dtTables[1].Rows[1];
            drOrgCredit[0] = GetLastRow(dtCredotOrg)[0];
            drOrgCredit[1] = GetLastRow(dtCredotOrg)[1];

            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = Convert.ToDateTime(calcDate).Year;
            var drOrgAttract = GetLastRow(dtCredotOrg);
            var drBudAttract = GetLastRow(dtCredotBud);
            drCaption[4] = drBudAttract[4];
            drCaption[5] = drBudAttract[5];
            drCaption[6] = drOrgAttract[4];
            drCaption[7] = drOrgAttract[5];

            drCaption[8] = activeContractSum1;
            drCaption[9] = activeContractSum2;

            return dtTables;
        }


        /// <summary>
        /// Кредиты бюджетов ДК МФРФ 
        /// </summary>
        public DataTable[] GetDKMFRFSubjectCreditBudData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            // Данные по ИФ 
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.onlyLastPlanService = true;
            cdo.planServiceDate = calcDate;
            cdo.onlyLastPlanDebt = true;
            cdo.planDebtDate = calcDate;
            SetCreditFilter(cdo.mainFilter, calcDate, calcDate);
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            // 00
            cdo.AddDataColumn(f_S_Creditincome.Occasion);
            // 01
            cdo.AddCalcColumn(CalcColumnType.cctContractDateNum);
            // 02
            cdo.AddCalcColumn(CalcColumnType.cctBorrowKind);
            // 03
            cdo.AddParamColumn(CalcColumnType.cctListDocs,
                FormFilterValue(t_S_ListContractCl.FunctionContract, ReportConsts.strFalse));
            // 04
            cdo.AddParamColumn(CalcColumnType.cctListDocs,
                FormFilterValue(t_S_ListContractCl.RefViewContract, "7"));
            // 05
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            // 06
            cdo.AddCalcColumn(CalcColumnType.cctAlterationNumDocDate);
            // 07
            cdo.AddParamColumn(CalcColumnType.cctListDocs,
                FormFilterValue(t_S_ListContractCl.RefViewContract, "10"));
            // 08
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            // 09
            cdo.AddDataColumn(f_S_Creditincome.StartDate, ReportConsts.ftDateTime);
            // 10
            cdo.AddDetailTextColumn(String.Format("[2](1<={0})", maxDate), cdo.ParamOnlyDates, String.Empty);
            // 11
            cdo.AddDetailColumn(String.Format("[0](1>{0})", maxDate)); 
            // 12
            cdo.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            // 13
            cdo.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdo.ParamOnlyDates, String.Empty);
            cdo.sortString = FormSortString(StrSortUp(f_S_Creditincome.StartDate));
            dtTables[0] = cdo.FillData();
            dtTables[0] = ClearCloseCreditMO(cdo, dtTables[0], Convert.ToDateTime(calcDate), 12, 13, -1);
            // Данные по ДК
            // кредиты субъекта
            var variantDate = String.Empty;
            var variantDK = GetDKVariantByDate(scheme, calcDate, ref variantDate);
            var subjectFilter = FormNegFilterValue(GetSubjectID(scheme, variantDate));
            var cdoDK = new CommonDataObject();
            cdoDK.InitObject(scheme);
            cdoDK.ObjectKey = f_S_SchBCreditincome.internalKey;
            cdoDK.mainFilter[f_S_SchBCreditincome.RefVariant] = variantDK;
            cdoDK.mainFilter[f_S_SchBCreditincome.RefTypeCredit] = ReportConsts.DKBudCreditCode;
            cdoDK.mainFilter[f_S_SchBCreditincome.RefRegion] = subjectFilter;
            cdoDK.AddDataColumn(f_S_SchBCreditincome.StaleDebt);
            cdoDK.AddDataColumn(f_S_SchBCreditincome.CapitalDebt);
            cdoDK.summaryColumnIndex.Add(0);
            cdoDK.summaryColumnIndex.Add(1);
            var dtCreditSub = cdoDK.FillData();
            // кредиты поселений
            var cdoDKPos = new CommonDataObject();
            cdoDKPos.InitObject(scheme);
            cdoDKPos.ObjectKey = f_S_SchBCreditincomePos.internalKey;
            cdoDKPos.mainFilter[f_S_SchBCreditincomePos.RefVariant] = variantDK;
            cdoDKPos.mainFilter[f_S_SchBCreditincomePos.RefTypeCredit] = ReportConsts.DKBudCreditCode;
            cdoDKPos.mainFilter[f_S_SchBCreditincomePos.RefRegion] = subjectFilter;
            cdoDKPos.AddDataColumn(f_S_SchBCreditincomePos.StaleDebt);
            cdoDKPos.AddDataColumn(f_S_SchBCreditincomePos.CapitalDebt);
            cdoDKPos.summaryColumnIndex.Add(0);
            cdoDKPos.summaryColumnIndex.Add(1);
            var dtCreditPos = cdoDKPos.FillData();
            dtTables[1] = CreateReportCaptionTable(4, 1);
            var drLast = GetLastRow(dtTables[1]);
            drLast[0] = "Кредит";
            drLast[1] = ReportConsts.RUB;
            drLast[2] = GetLastRowValue(dtCreditSub, 0) + GetLastRowValue(dtCreditPos, 0);
            drLast[3] = GetLastRowValue(dtCreditSub, 1) + GetLastRowValue(dtCreditPos, 1);
            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            FillSignatureData(drCaption, 10, reportParams, ReportConsts.ParamExecutor1);
            return dtTables;
        }

        /// <summary>
        /// Кредиты организаций ДК МФРФ 
        /// </summary>
        public DataTable[] GetDKMFRFSubjectCreditOrgData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            // Данные по ИФ
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.planServiceDate = calcDate;
            cdo.onlyLastPlanService = true;
            cdo.planDebtDate = calcDate;
            cdo.onlyLastPlanDebt = true;
            SetCreditFilter(cdo.mainFilter, calcDate, calcDate);
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            // 00
            cdo.AddDataColumn(f_S_Creditincome.Occasion);
            // 01
            cdo.AddCalcColumn(CalcColumnType.cctContractDateNum);
            // 02
            cdo.AddParamColumn(CalcColumnType.cctListDocs, CombineList(
                    FormFilterValue(t_S_ListContractCl.BaseDoc, ReportConsts.strFalse),
                    FormFilterValue(t_S_ListContractCl.FunctionContract, ReportConsts.strFalse),
                    FormFilterValue(t_S_ListContractCl.RefViewContract, "1")));              
            // 03
            cdo.AddParamColumn(CalcColumnType.cctListDocs, CombineList(
                    FormFilterValue(t_S_ListContractCl.BaseDoc, ReportConsts.strFalse),
                    FormFilterValue(t_S_ListContractCl.FunctionContract, ReportConsts.strFalse),
                    FormFilterValue(t_S_ListContractCl.RefViewContract, "2")));
            // 04
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            // 05
            cdo.AddCalcColumn(CalcColumnType.cctAlterationNumDocDate);
            // 06
            cdo.AddParamColumn(CalcColumnType.cctListDocs,
                FormFilterValue(t_S_ListContractCl.RefViewContract, "3"));
            // 07
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            // 08
            cdo.AddDataColumn(f_S_Creditincome.StartDate, ReportConsts.ftDateTime);
            // 09
            cdo.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 10
            cdo.AddDataColumn(f_S_Creditincome.EndDate);
            // 11
            cdo.AddDetailColumn(String.Format("[0](1>{0})", maxDate)); 
            // 12
            cdo.AddDetailColumn(String.Format("[0](1>{0})", maxDate)); 
            // 13
            cdo.AddDetailColumn(String.Format("[0](1>{0})", maxDate)); 
            // 14
            cdo.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            // 15
            cdo.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdo.ParamOnlyDates, String.Empty);
            cdo.sortString = FormSortString(StrSortUp(f_S_Creditincome.StartDate));
            dtTables[0] = cdo.FillData();
            dtTables[0] = ClearCloseCreditMO(cdo, dtTables[0], Convert.ToDateTime(calcDate), 14, 15, -1);   
            // Данные по ДК
            // кредиты субъекта
            var variantDate = String.Empty;
            var variantDK = GetDKVariantByDate(scheme, calcDate, ref variantDate);
            var subjectFilter = FormNegFilterValue(GetSubjectID(scheme, variantDate));
            var cdoDK = new CommonDataObject();
            cdoDK.InitObject(scheme);
            cdoDK.ObjectKey = f_S_SchBCreditincome.internalKey;
            cdoDK.mainFilter[f_S_SchBCreditincome.RefVariant] = variantDK;
            cdoDK.mainFilter[f_S_SchBCreditincome.RefTypeCredit] = ReportConsts.DKOrgCreditCode;
            cdoDK.mainFilter[f_S_SchBCreditincome.RefRegion] = subjectFilter;
            cdoDK.AddDataColumn(f_S_SchBCreditincome.StaleDebt);
            cdoDK.AddDataColumn(f_S_SchBCreditincome.CapitalDebt);
            cdoDK.summaryColumnIndex.Add(0);
            cdoDK.summaryColumnIndex.Add(1);
            var dtCreditSub = cdoDK.FillData();
            // кредиты поселений
            var cdoDKPos = new CommonDataObject();
            cdoDKPos.InitObject(scheme);
            cdoDKPos.ObjectKey = f_S_SchBCreditincomePos.internalKey;
            cdoDKPos.mainFilter[f_S_SchBCreditincomePos.RefVariant] = variantDK;
            cdoDKPos.mainFilter[f_S_SchBCreditincomePos.RefTypeCredit] = ReportConsts.DKOrgCreditCode;
            cdoDKPos.mainFilter[f_S_SchBCreditincomePos.RefRegion] = subjectFilter;
            cdoDKPos.AddDataColumn(f_S_SchBCreditincomePos.StaleDebt);
            cdoDKPos.AddDataColumn(f_S_SchBCreditincomePos.CapitalDebt);
            cdoDKPos.summaryColumnIndex.Add(0);
            cdoDKPos.summaryColumnIndex.Add(1);
            var dtCreditPos = cdoDKPos.FillData();
            dtTables[1] = CreateReportCaptionTable(3, 1);
            var drLast = GetLastRow(dtTables[1]);
            drLast[0] = ReportConsts.RUB;
            drLast[1] = GetLastRowValue(dtCreditSub, 0) + GetLastRowValue(dtCreditPos, 0);
            drLast[2] = GetLastRowValue(dtCreditSub, 1) + GetLastRowValue(dtCreditPos, 1);
            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            FillSignatureData(drCaption, 10, reportParams, ReportConsts.ParamExecutor1);
            return dtTables;
        }

        /// <summary>
        /// Расчет процентов за пользование заемными средствами
        /// </summary>
        public DataTable[] GetCreditCalcPercentVologdaData(Dictionary<string, string> reportParams)
        {
            var orgCount = 0;
            decimal totalSum = 0;
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            // Кредиты
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.useSummaryRow = false;
            SetCreditFilter(cdo.mainFilter, calcDate, calcDate);
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            // 0
            cdo.AddDataColumn(f_S_Creditincome.RefOrganizations);
            // 1
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);         
            // 2
            cdo.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            // 3 - сумма договора
            cdo.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", calcDate));
            // 4 - нужные детали для разбивки строк
            cdo.AddDetailColumn("[0][1][5]");
            // 5 - сортировочная
            cdo.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            // 6 - сортировочная
            cdo.AddDataColumn(f_S_Creditincome.RegNum);
            // 7 - подцепим журнал процентов
            cdo.AddCalcColumn(CalcColumnType.cctPercentText);
            cdo.sortString = FormSortString(StrSortUp(f_S_Creditincome.RefOrganizations), StrSortUp(f_S_Creditincome.RegNum));
            dtTables[0] = cdo.FillData();
            dtTables[0] = SplitRowsCalcPercentVologda(cdo, dtTables[0], calcDate, ref orgCount, ref totalSum);
            // заголовочные параметры
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = Convert.ToDateTime(calcDate).Year;
            drCaption[2] = orgCount;
            drCaption[3] = totalSum;
            return dtTables;
        }

        /// <summary>
        /// Программа государственных внутренних заимствований Вологодской области на отчетный год
        /// </summary>
        public DataTable[] GetBorrowingVologdaData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, Combine( 
                ReportConsts.FixedVariantsID, reportParams[ReportConsts.ParamVariantID]));
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var yearStart = GetYearStart(year);
            var yearEnd = GetYearEnd(year);
            // Формирование колонок
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+1;-2");
            cdo.AddDetailColumn(String.Format("[3](0<={0}0>={1})", yearEnd, yearStart));
            cdo.AddDetailColumn(String.Format("[2](1<={0}1>={1})", yearEnd, yearStart));
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var dtResult1 = cdo.FillData();
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            var dtResult2 = cdo.FillData();
            dtTables[0] = CreateReportCaptionTable(7);
            var drData = dtTables[0].Rows.Add();
            
            for (int i = 0; i < 3; i++)
            {
                drData[i + 0] = ConvertTo1000(GetLastRowValue(dtResult1, i));
                drData[i + 3] = ConvertTo1000(GetLastRowValue(dtResult2, i));
            }
            
            drData[6] = ConvertTo1000(GetLastRowValue(dtResult1, 0) + GetLastRowValue(dtResult2, 0));
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = year;
            return dtTables;
        }

        private DataTable GetCreditDataSamara(string calcDate, string creditType, DataRow drCaption)
        {
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var currencySum = CreateValuePair(ReportConsts.CurrencySumField);
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.reportParams.Add(f_S_Creditincome.EndDate, calcDate);
            cdo.exchangePrevDay = true;
            SetCreditFilter(cdo.mainFilter, calcDate, calcDate);
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, creditType);
            cdo.columnCondition.Add(13, FormNegativeFilterValue(f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr));
            cdo.columnCondition.Add(15, FormNegativeFilterValue(f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr));
            cdo.columnCondition.Add(17, FormNegativeFilterValue(f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr));
            cdo.columnCondition.Add(19, FormNegativeFilterValue(f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr));
            // 0
            cdo.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdo.AddDataColumn(f_S_Creditincome.RegNum);
            // 2
            cdo.AddDataColumn(f_S_Creditincome.Occasion);
            // 3
            cdo.AddCalcColumn(CalcColumnType.cctNumContractDate);
            // 4
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            // 5
            cdo.AddDataColumn(f_S_Creditincome.Purpose);
            // 6
            cdo.AddCalcColumn(CalcColumnType.cctCollateralType);
            // 7
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            // 8
            cdo.AddDataColumn(f_S_Creditincome.StartDate);
            // 9
            cdo.AddDataColumn(f_S_Creditincome.EndDate);
            // 10
            cdo.AddDataColumn(f_S_Creditincome.CurrencySum);
            // 11
            cdo.AddCalcColumn(CalcColumnType.cctCalcSum);
            // 12
            cdo.AddCalcColumn(CalcColumnType.cctPercentValues);
            // 13 
            cdo.AddDetailColumn(String.Format("[5](1<={0})", maxDate), currencySum, true);
            // 14
            cdo.AddDetailColumn(String.Format("[5](1<={0})", maxDate));
            // 15
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+10;+13");
            // 16
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+11;+14");
            // 17 (CurrencySum)
            cdo.AddDetailColumn(String.Format("[0](1<={0})", calcDate), currencySum, true);
            // 18
            cdo.AddDetailColumn(String.Format("[0](1<={0})", calcDate));
            // 19 
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+17;+13");
            // 20
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+18;+14");
            // 21 (CurrencySum)
            cdo.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", calcDate), currencySum, true);
            // 22
            cdo.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", calcDate));
            // 23 (CurrencySum)
            cdo.AddDetailColumn(String.Format("-+-[0](1<={0})[1](1<={0})[5](1<={0})[4](1<={0})", calcDate),
                currencySum, true);
            // 24
            cdo.AddDetailColumn(String.Format("-+-[0](1<={0})[1](1<={0})[5](1<={0})[4](1<={0})", calcDate));
            // 25 (CurrencySum)
            cdo.AddDetailColumn(String.Format("+[2](1<={0})[5](1<={0})", calcDate), currencySum, true);
            // 26
            cdo.AddDetailColumn(String.Format("+[2](1<={0})[5](1<={0})", calcDate));
            // 27 (CurrencySum)
            cdo.AddDetailColumn(String.Format("[2](1<={0})", calcDate), currencySum, true);
            // 28
            cdo.AddDetailColumn(String.Format("[2](1<={0})", calcDate));
            // 29 (CurrencySum)
            cdo.AddDetailColumn(String.Format("[5](1<={0})", calcDate), currencySum, true);
            // 30
            cdo.AddDetailColumn(String.Format("[5](1<={0})", calcDate));
            // 31 (CurrencySum)
            cdo.AddDetailColumn(String.Format("+[6](1<={0})[7](1<={0})", calcDate), currencySum, true);
            // 32
            cdo.AddDetailColumn(String.Format("+[6](1<={0})[7](1<={0})", calcDate));
            // 33 (CurrencySum)
            cdo.AddDetailColumn(String.Format("+[1](1<={0})[4](1<={0})", calcDate), currencySum, true);
            // 34
            cdo.AddDetailColumn(String.Format("+[1](1<={0})[4](1<={0})", calcDate));
            // 35 (CurrencySum)
            cdo.AddDetailColumn(String.Format("[1](1<={0})", calcDate), currencySum, true);
            // 36
            cdo.AddDetailColumn(String.Format("[1](1<={0})", calcDate));
            // 37 (CurrencySum)
            cdo.AddDetailColumn(String.Format("[4](1<={0})", calcDate), currencySum, true);
            // 38
            cdo.AddDetailColumn(String.Format("[4](1<={0})", calcDate));
            // 39 (CurrencySum)
            cdo.AddDetailColumn(String.Format("+[8](1<={0})[9](1<={0})", calcDate), currencySum, true);
            // 40
            cdo.AddDetailColumn(String.Format("[8](1<={0})[9](1<={0})", calcDate));
            // 41 (CurrencySum)
            cdo.AddDetailColumn(String.Format("--+[2](1<={0})[5](1<={0})[1](1<={0})[4](1<={0})", calcDate),
                currencySum, true);
            // 42
            cdo.AddDetailColumn(String.Format("--+[2](1<={0})[5](1<={0})[1](1<={0})[4](1<={0})", calcDate));
            // 43 (CurrencySum)
            cdo.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={0})", calcDate), currencySum, true);
            // 44 
            cdo.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={0})", calcDate));
            // 45 (CurrencySum)
            cdo.AddDetailColumn(String.Format("-[5](1<={0})[4](1<={0})", calcDate), currencySum, true);
            // 46
            cdo.AddDetailColumn(String.Format("-[5](1<={0})[4](1<={0})", calcDate));
            // 47 (CurrencySum)
            cdo.AddDetailColumn(String.Format("--+[6](1<={0})[7](1<={0})[8](1<={0})[9](1<={0})", calcDate),
                currencySum, true);
            // 48
            cdo.AddDetailColumn(String.Format("--+[6](1<={0})[7](1<={0})[8](1<={0})[9](1<={0})", calcDate));
            // 49
            cdo.AddDataColumn(f_S_Creditincome.Note);
            // 50 - служебное (CurrencySum)
            cdo.AddDetailColumn(String.Format("[2](1<={0})", maxDate), currencySum, true);
            // 51 - служебное
            cdo.AddDetailColumn(String.Format("[2](1<={0})", maxDate));
            // 52 - служебное (19)
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+50;+13");
            // 53 - служебное (20)
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+51;+14");
            // 54 - служебное (CurrencySum) (21)
            cdo.AddDetailColumn(String.Format("-[2](1<={1})[1](1<={0})", calcDate, maxDate), currencySum, true);
            // 55 - служебное (22)
            cdo.AddDetailColumn(String.Format("-[2](1<={1})[1](1<={0})", calcDate, maxDate));
            // 56 - служебное (CurrencySum) (23)
            cdo.AddDetailColumn(String.Format("-+-[2](1<={1})[1](1<={0})[5](1<={0})[4](1<={0})", calcDate, maxDate),
                currencySum, true);
            // 57 - служебное (24)
            cdo.AddDetailColumn(String.Format("-+-[2](1<={1})[1](1<={0})[5](1<={0})[4](1<={0})", calcDate, maxDate));
            cdo.summaryColumnIndex.Add(10);
            cdo.summaryColumnIndex.Add(11);
            var dtTable = cdo.FillData();

            foreach (DataRow dr in dtTable.Rows)
            {
                if (dr[f_S_Creditincome.RefOKV] == DBNull.Value) continue;

                var okvType = Convert.ToInt32(dr[f_S_Creditincome.RefOKV]);

                if (okvType == -1)
                {
                    dr[10] = DBNull.Value;
                }

                for (var i = 0; i < 18; i++)
                {
                    if (okvType == -1) dr[13 + i * 2] = DBNull.Value;
                }

                if (okvType == -1) continue;

                for (var j = 0; j < 6; j++)
                {
                    dr[19 + j] = dr[52 + j];
                }
            }

            FillExchangeValueSamara(drCaption, 6, cdo);
            // Приехали
            return cdo.RecalcSummary(dtTable);
        }

        /// <summary>
        /// Планирование расходов на обслуживание долга
        /// </summary>
        public DataTable[] GetPlanDebtSaratovData(Dictionary<string, string> reportParams)
        {
            const int columnGroupCount = 6;
            var dtTables = new DataTable[2];
            var drCaption = CreateReportParamsRow(dtTables);
            var startDate = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var endDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.useSummaryRow = false;
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = FormNegFilterValue("4");
            cdoCredit.mainFilter.Add(f_S_Creditincome.EndDate, String.Format(">'{0}' or c.{1}>'{0}'",
                                                                             startDate, f_S_Creditincome.RenewalDate));
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSTypeCredit,
                                     Combine(ReportConsts.OrgCreditCode, ReportConsts.BudCreditCode));
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdoCredit.AddCalcColumn(CalcColumnType.cctNumContractDate);
            cdoCredit.AddDataColumn(f_S_Creditincome.Sum);
            cdoCredit.AddDataColumn(f_S_Creditincome.RefOrganizations);
            cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})", startDate, endDate));
            cdoCredit.AddDataColumn(f_S_Creditincome.RefSTypeCredit);
            cdoCredit.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            cdoCredit.AddDetailColumn(String.Format("[4]", startDate, endDate));

            cdoCredit.sortString = FormSortString(
                StrSortUp(f_S_Creditincome.RefSTypeCredit),
                StrSortUp(TempFieldNames.OrgName),
                StrSortUp(f_S_Creditincome.ContractDate));

            var dtMaster = cdoCredit.FillData();
            var dtPlanDetail = cdoCredit.dtDetail[5];
            var dtFactDetail = cdoCredit.dtDetail[4];
            int masterKey;
            var dateList = new List<string>();
            string dateKey;

            foreach (DataRow dr in dtMaster.Rows)
            {
                masterKey = Convert.ToInt32(dr[f_S_Creditincome.id]);

                // суммы по планам
                cdoCredit.GetSumValue(dtPlanDetail, masterKey, t_S_PlanServiceCI.EndDate, ReportConsts.SumField,
                                      Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), true, true);

                foreach (var drDetail in cdoCredit.sumIncludedRows)
                {
                    var endDetailDate = Convert.ToDateTime(drDetail[t_S_PlanServiceCI.EndDate]);
                    dateKey = String.Format("{0}{1}", endDetailDate.Year, endDetailDate.Month.ToString().PadLeft(2, '0'));

                    if (!dateList.Contains(dateKey))
                    {
                        dateList.Add(dateKey);
                    }
                }

                // суммы по фактам
                cdoCredit.GetSumValue(dtFactDetail, masterKey, t_S_FactPercentCI.FactDate, ReportConsts.SumField,
                                      Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), true, true);

                foreach (var drDetail in cdoCredit.sumIncludedRows)
                {
                    var endDetailDate = Convert.ToDateTime(drDetail[t_S_FactPercentCI.FactDate]);
                    dateKey = String.Format("{0}{1}", endDetailDate.Year, endDetailDate.Month.ToString().PadLeft(2, '0'));

                    if (!dateList.Contains(dateKey))
                    {
                        dateList.Add(dateKey);
                    }
                }
            }

            dateList.Sort();

            var counter = 0;
            var monthList = GetMonthRusNames();

            foreach (var date in dateList)
            {
                var month = date.Substring(4, date.Length - 4);
                var year = date.Substring(0, 4);
                drCaption[10 + counter++] = String.Format("{0} {1} года", monthList[Convert.ToInt32(month) - 1], year);
            }

            var columnCount = 4 + columnGroupCount*dateList.Count;
            dtTables[0] = CreateReportCaptionTable(columnCount + 1);
            var summary = new decimal[columnCount];
            var totalSum = new decimal[columnCount];
            var creditTypeSum = new decimal[columnCount];
            var refOrgPrev = -666;
            var refTypePrev = -666;
            var rowCount = 0;
            var rowTypeCount = 0;
            var orgName = String.Empty;

            for (var i = 0; i < columnCount; i++)
            {
                summary[i] = 0;
            }

            foreach (DataRow dr in dtMaster.Rows)
            {
                var refOrgCurr = Convert.ToInt32(dr[f_S_Creditincome.RefOrganizations]);
                var refTypeCurr = Convert.ToInt32(dr[f_S_Creditincome.RefSTypeCredit]);

                if (refOrgCurr != refOrgPrev && rowCount > 0)
                {
                    var drSummary = dtTables[0].Rows.Add();
                    drSummary[2] = summary[2];
                    drSummary[columnCount - 1] = summary[columnCount - 1];

                    for (var i = 0; i < dateList.Count; i++)
                    {
                        drSummary[i*columnGroupCount + 7] = summary[i*columnGroupCount + 7];
                    }

                    drSummary[0] = String.Format("Итого по {0}", orgName);
                    drSummary[columnCount] = 1;

                    for (var i = 0; i < columnCount; i++)
                    {
                        summary[i] = 0;
                    }

                    rowCount = 0;
                }

                if (refTypeCurr != refTypePrev && rowTypeCount > 0)
                {
                    var drSummary = dtTables[0].Rows.Add();

                    drSummary[2] = creditTypeSum[2];
                    drSummary[columnCount - 1] = creditTypeSum[columnCount - 1];

                    for (var i = 0; i < dateList.Count; i++)
                    {
                        drSummary[i*columnGroupCount + 7] = creditTypeSum[i*columnGroupCount + 7];
                    }

                    var typeCaption = "бюджетным кредитам";

                    if (Convert.ToString(refTypeCurr) == ReportConsts.BudCreditCode)
                    {
                        typeCaption = "кредитам организаций";
                    }

                    drSummary[0] = String.Format("Итого по {0}", typeCaption);
                    drSummary[columnCount] = 1;

                    for (var i = 0; i < columnCount; i++)
                    {
                        creditTypeSum[i] = 0;
                    }
                }

                orgName = dr[0].ToString();
                masterKey = Convert.ToInt32(dr[f_S_Creditincome.id]);
                var drResult = dtTables[0].Rows.Add();
                drResult[columnCount] = 0;
                drResult[0] = dr[0];
                drResult[1] = dr[1];
                drResult[2] = dr[2];
                drResult[columnCount - 1] = dr[4];
                summary[2] += Convert.ToDecimal(dr[2]);
                totalSum[2] += Convert.ToDecimal(dr[2]);
                creditTypeSum[2] += Convert.ToDecimal(dr[2]);
                summary[columnCount - 1] += Convert.ToDecimal(dr[4]);
                totalSum[columnCount - 1] += Convert.ToDecimal(dr[4]);
                creditTypeSum[columnCount - 1] += Convert.ToDecimal(dr[4]);

                // суммы по фактам
                cdoCredit.GetSumValue(dtFactDetail, masterKey, t_S_FactPercentCI.FactDate, ReportConsts.SumField,
                                      Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), true, true);

                var lstExistMonth = new List<string>();

                foreach (var drDetail in cdoCredit.sumIncludedRows)
                {
                    var endDetailDate = Convert.ToDateTime(drDetail[t_S_FactPercentCI.FactDate]);
                    dateKey = Combine(endDetailDate.Year, endDetailDate.Month.ToString().PadLeft(2, '0'), String.Empty);
                    lstExistMonth.Add(dateKey);
                    var colIndex = dateList.IndexOf(dateKey)*columnGroupCount + 3;
                    drResult[colIndex + 5] = cdoCredit.GetDateValue(drDetail[t_S_FactPercentCI.FactDate]);
                    drResult[colIndex + 4] = drDetail[t_S_FactPercentCI.Sum];
                    var sumValue = Convert.ToDecimal(drDetail[ReportConsts.SumField]);
                    summary[colIndex + 4] += sumValue;
                    totalSum[colIndex + 4] += sumValue;
                    creditTypeSum[colIndex + 4] += sumValue;
                }

                // суммы по планам
                cdoCredit.GetSumValue(dtPlanDetail, masterKey, t_S_PlanServiceCI.EndDate, ReportConsts.SumField,
                                      Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), true, true);

                foreach (var drDetail in cdoCredit.sumIncludedRows)
                {
                    var endDetailDate = Convert.ToDateTime(drDetail[t_S_PlanServiceCI.EndDate]);
                    dateKey = Combine(endDetailDate.Year, endDetailDate.Month.ToString().PadLeft(2, '0'), String.Empty);

                    if (lstExistMonth.Contains(dateKey))
                    {
                        continue;
                    }

                    var colIndex = dateList.IndexOf(dateKey)*columnGroupCount + 3;
                    drResult[colIndex + 0] = cdoCredit.GetDateValue(drDetail[t_S_PlanServiceCI.StartDate]);
                    drResult[colIndex + 1] = cdoCredit.GetDateValue(drDetail[t_S_PlanServiceCI.EndDate]);
                    drResult[colIndex + 2] = drDetail[t_S_PlanServiceCI.DayCount];
                    drResult[colIndex + 3] = drDetail[t_S_PlanServiceCI.CreditPercent];
                    drResult[colIndex + 4] = drDetail[t_S_PlanServiceCI.Sum];
                    drResult[colIndex + 5] = cdoCredit.GetDateValue(drDetail[t_S_PlanServiceCI.EndDate]);
                    var sumValue = Convert.ToDecimal(drDetail[ReportConsts.SumField]);
                    summary[colIndex + 4] += sumValue;
                    totalSum[colIndex + 4] += sumValue;
                    creditTypeSum[colIndex + 4] += sumValue;
                }

                refOrgPrev = refOrgCurr;
                refTypePrev = refTypeCurr;
                rowCount++;
                rowTypeCount++;
            }

            if (rowCount > 0)
            {
                var drSummary = dtTables[0].Rows.Add();
                drSummary[2] = summary[2];
                drSummary[columnCount - 1] = summary[columnCount - 1];

                for (var i = 0; i < dateList.Count; i++)
                {
                    drSummary[i*columnGroupCount + 7] = summary[i*columnGroupCount + 7];
                }

                drSummary[columnCount] = 1;
                drSummary[0] = String.Format("Итого по {0}", orgName);

                drSummary = dtTables[0].Rows.Add();

                drSummary[2] = creditTypeSum[2];
                drSummary[columnCount - 1] = creditTypeSum[columnCount - 1];

                for (var i = 0; i < dateList.Count; i++)
                {
                    drSummary[i*columnGroupCount + 7] = creditTypeSum[i*columnGroupCount + 7];
                }

                var typeCaption = "бюджетным кредитам";

                if (Convert.ToString(refTypePrev) == ReportConsts.OrgCreditCode)
                {
                    typeCaption = "кредитам организаций";
                }

                drSummary[0] = String.Format("Итого по {0}", typeCaption);
                drSummary[columnCount] = 1;

                for (var i = 0; i < columnCount; i++)
                {
                    creditTypeSum[i] = 0;
                }
            }

            if (dtMaster.Rows.Count > 0)
            {
                var drSummary = dtTables[0].Rows.Add();
                drSummary[0] = "ВСЕГО";
                drSummary[2] = totalSum[2];
                drSummary[columnCount - 1] = totalSum[columnCount - 1];
                drSummary[columnCount] = 2;

                for (var i = 0; i < dateList.Count; i++)
                {
                    drSummary[i*columnGroupCount + 7] = totalSum[i*columnGroupCount + 7];
                }
            }

            drCaption[0] = startDate;
            drCaption[1] = endDate;
            drCaption[2] = dateList.Count;
            return dtTables;
        }

        /// <summary>
        /// Программа государственных внутренних заимствований области
        /// </summary>
        public DataTable[] GetBorrowingSaratovData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.useSummaryRow = false;
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, 
                Combine(ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var yearStart = GetYearStart(year);
            var yearEnd = GetYearEnd(year);
            // Формирование колонок
            cdo.AddDataColumn(f_S_Creditincome.Sum);
            cdo.AddDetailColumn(String.Format("[0](1<={0}1>={1})", yearEnd, yearStart));
            cdo.AddDetailColumn(String.Format("[3](0<={0}0>={1})", yearEnd, yearStart));
            cdo.AddDetailColumn(String.Format("[1](1<={0}1>={1})", yearEnd, yearStart));
            cdo.AddDetailColumn(String.Format("[2](0<={0}0>={1})", yearEnd, yearStart));
            cdo.AddDataColumn(f_S_Creditincome.StartDate);
            cdo.summaryColumnIndex.Add(0);
            // Заполнение данными
            var dtResult1 = cdo.FillData();
            // Заполнение данными
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var dtResult2 = cdo.FillData();
            dtTables[0] = CreateReportCaptionTable(2);
            var summary = new decimal[3, 2];
            FillBorrowSumArray(dtResult1, ref summary, 0, 2, year);
            FillBorrowSumArray(dtResult2, ref summary, 1, 2, year);
            
            for (int i = 0; i < 3; i++)
            {
                var drData = dtTables[0].Rows.Add();
                drData[0] = summary[i, 0];
                drData[1] = summary[i, 1];
            }

            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(1);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = year;

            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Муниц.заимствования за период"
        /// </summary>
        public DataTable[] GetBorrowingPeriodData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[1];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Creditincome.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            // Список дат для колонок
            var shortCalcDateStart1 = GetYearStart(year + 1);
            var shortCalcDateEnd1 = GetYearEnd(year + 1);
            var shortCalcDateStart2 = GetYearStart(year + 2);
            var shortCalcDateEnd2 = GetYearEnd(year + 2);
            var nowStr = DateTime.Now.ToShortDateString();
            // Формирование колонок
            cdo.AddDetailColumn(String.Format("[3](0<={0}0>={1})", shortCalcDateEnd1, shortCalcDateStart1));
            cdo.AddDetailColumn(String.Format("[2](1<={0}1>={1})", shortCalcDateEnd1, shortCalcDateStart1));
            cdo.AddDetailColumn(String.Format("[3](0<={0}0>={1})", shortCalcDateEnd2, shortCalcDateStart2));
            cdo.AddDetailColumn(String.Format("[2](1<={0}1>={1})", shortCalcDateEnd2, shortCalcDateStart2));
            cdo.AddDetailColumn(String.Format("-[3](0<={0})[2](1<={0})", shortCalcDateEnd1));
            cdo.AddDetailColumn(String.Format("-[3](0<={0})[2](1<={0})", shortCalcDateEnd2));
            cdo.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", nowStr));
            cdo.AddDetailColumn(String.Format("[3](0>{0}0<{1})", nowStr, shortCalcDateStart1));
            cdo.AddDetailColumn(String.Format("[2](1>{0}1<{1})", nowStr, shortCalcDateStart1));
            cdo.AddDetailColumn(String.Format("[3](0>{0}0<{1})", nowStr, shortCalcDateStart2));
            cdo.AddDetailColumn(String.Format("[2](1>{0}1<{1})", nowStr, shortCalcDateStart2));
            cdo.columnCondition.Add(07, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(08, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(09, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(10, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            // Заполнение данными
            var dtResult = cdo.FillData();
            var sum11 = GetLastRowValue(dtResult, 6);
            var sum22 = GetLastRowValue(dtResult, 7);
            var sum33 = GetLastRowValue(dtResult, 8);
            var totalSum1 = sum11 + sum22 - sum33;
            sum22 = GetLastRowValue(dtResult, 9);
            sum33 = GetLastRowValue(dtResult, 10);
            var totalSum2 = sum11 + sum22 - sum33;
            var sum1 = GetLastRowValue(dtResult, 0);
            var sum2 = GetLastRowValue(dtResult, 1);
            var sum3 = GetLastRowValue(dtResult, 2);
            var sum4 = GetLastRowValue(dtResult, 3);
            dtTables[0] = CreateReportTableDebtStructure();
            var drResult = dtTables[0].Rows.Add();
            drResult[0] = reportParams[ReportConsts.ParamYear];
            drResult[1] = ConvertTo1000(sum1);
            drResult[2] = ConvertTo1000(sum3);
            drResult[3] = ConvertTo1000(sum2);
            drResult[4] = ConvertTo1000(sum4);
            drResult[5] = ConvertTo1000(totalSum1);
            drResult[6] = ConvertTo1000(totalSum2);
            return dtTables;
        }

        private decimal[] GetCreditSum(CommonQueryParam procParams)
        {
            var result = new decimal[5];
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefVariant, Combine(
                ReportConsts.ActiveVariantID, procParams.variantID));
            cdoCredit.mainFilter.Add(f_S_Creditincome.SourceID, procParams.sourceID);
            cdoCredit.AddDetailColumn(String.Format("[0](1<{0})", procParams.yearStart));
            cdoCredit.AddDetailColumn(String.Format("[3](0<{0})", procParams.yearStart));
            cdoCredit.AddDetailColumn(String.Format("[1](1<{0})", procParams.yearStart));
            cdoCredit.AddDetailColumn(String.Format("[2](1<{0})", procParams.yearStart));
            var dtCredit = cdoCredit.FillData();
            var kFactAttr = GetLastRowValue(dtCredit, 0);
            var kPlanAttr = GetLastRowValue(dtCredit, 1);
            var kFactDebt = GetLastRowValue(dtCredit, 2);
            var kPlanDebt = GetLastRowValue(dtCredit, 3);
            result[0] = kFactAttr;
            result[1] = kPlanAttr;
            result[2] = kFactDebt;
            result[3] = kPlanDebt;
            result[4] = ConvertTo1000(2 * kFactAttr - kPlanAttr - (2 * kFactDebt - kPlanDebt));
            return result;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Заимствования"
        /// </summary>
        public DataTable[] GetBorrowingData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, Combine( 
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdo.mainFilter.Add(f_S_Creditincome.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var shortCalcDateStart = GetYearStart(year);
            var shortCalcDateEnd = GetYearEnd(year);
            var nowStr = DateTime.Now.ToShortDateString();
            // Формирование колонок
            cdo.AddDetailColumn(String.Format("[3](0<={0}0>={1})", shortCalcDateEnd, shortCalcDateStart));
            cdo.AddDetailColumn(String.Format("[2](1<={0}1>={1})", shortCalcDateEnd, shortCalcDateStart));
            cdo.AddDetailColumn(String.Format("-[3](0<={0})[2](1<={0})", GetYearEnd(year)));
            cdo.AddDetailColumn(String.Format("-[3](0<={0})[2](1<={0})", GetYearEnd(year - 1)));
            cdo.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", nowStr));
            cdo.AddDetailColumn(String.Format("[3](0>{0}0<{1})", nowStr, shortCalcDateStart));
            cdo.AddDetailColumn(String.Format("[2](1>{0}1<{1})", nowStr, shortCalcDateStart));
            cdo.columnCondition.Add(5, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(6, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            // Заполнение данными
            var dtResult1 = cdo.FillData();
            var sum1 = GetLastRowValue(dtResult1, 4);
            var sum2 = GetLastRowValue(dtResult1, 5);
            var sum3 = GetLastRowValue(dtResult1, 6);
            var totalSum1 = sum1 + sum2 - sum3;
            // Заполнение данными
            var sum11 = GetLastRowValue(dtResult1, 0);
            var sum12 = GetLastRowValue(dtResult1, 1);
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var dtResult2 = cdo.FillData();
            var sum21 = GetLastRowValue(dtResult2, 0);
            var sum22 = GetLastRowValue(dtResult2, 1);
            dtTables[0] = CreateReportTableDebtStructure();
            dtTables[1] = cdo.dtExchange;
            var drResult = dtTables[0].Rows.Add();
            drResult[0] = reportParams[ReportConsts.ParamYear];
            drResult[1] = ConvertTo1000(sum21);
            drResult[2] = ConvertTo1000(sum11);
            drResult[3] = ConvertTo1000(sum22);
            drResult[4] = ConvertTo1000(sum12);
            drResult[5] = ConvertTo1000(totalSum1);
            drResult[6] = ConvertTo1000(totalSum1);
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Структура муниц.долга на период"
        /// </summary>
        public DataTable[] GetDebtStructurePeriodData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[1];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Creditincome.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var shortCalcDate1 = GetYearStart(year + 2);
            var shortCalcDate2 = GetYearStart(year + 3);
            var nowStr = DateTime.Now.ToShortDateString();
            // Формирование колонок
            cdo.AddDetailColumn(String.Format("+-[0](1<={0})[1](1<={0})[10](0<{1})", nowStr, shortCalcDate1));
            cdo.AddDetailColumn(String.Format("[3](0>{0}0<{1})", nowStr, shortCalcDate1));
            cdo.AddDetailColumn(String.Format("[2](1>{0}1<{1})", nowStr, shortCalcDate1));
            cdo.AddDetailColumn(String.Format("+-[0](1<={0})[1](1<={0})[10](0<{1})", nowStr, shortCalcDate2));
            cdo.AddDetailColumn(String.Format("[3](0>{0}0<{1})", nowStr, shortCalcDate2));
            cdo.AddDetailColumn(String.Format("[2](1>{0}1<{1})", nowStr, shortCalcDate2));
            cdo.columnCondition.Add(1, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(2, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(4, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(5, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            // Заполнение данными
            var dtResult1 = cdo.FillData();
            var sum1 = GetLastRowValue(dtResult1, 0);
            var sum2 = GetLastRowValue(dtResult1, 1);
            var sum3 = GetLastRowValue(dtResult1, 2);
            var totalSum1 = sum1 + sum2 - sum3;
            sum1 = GetLastRowValue(dtResult1, 3);
            sum2 = GetLastRowValue(dtResult1, 4);
            sum3 = GetLastRowValue(dtResult1, 5);
            var totalSum2 = sum1 + sum2 - sum3;
            dtTables[0] = CreateReportTableDebtStructure();
            var drResult = dtTables[0].Rows.Add();
            drResult[0] = reportParams[ReportConsts.ParamYear];
            drResult[1] = ConvertTo1000(totalSum1);
            drResult[2] = ConvertTo1000(totalSum2);
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Структура долга"
        /// </summary>
        public DataTable[] GetDebtStructureData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Creditincome.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            // Список дат для колонок
            var shortCalcDate = GetYearStart(Convert.ToInt32(reportParams[ReportConsts.ParamYear]));
            var nowStr = DateTime.Now.ToShortDateString();
            // Формирование колонок
            cdo.AddDetailColumn(String.Format("+-[0](1<={0})[1](1<={0})[10](0<{1})", nowStr, shortCalcDate));
            cdo.AddDetailColumn(String.Format("[3](0>{0}0<{1})", nowStr, shortCalcDate));
            cdo.AddDetailColumn(String.Format("[2](1>{0}1<{1})", nowStr, shortCalcDate));
            cdo.columnCondition.Add(1, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            cdo.columnCondition.Add(2, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "0,5"));
            // Заполнение данными
            var dtResult1 = cdo.FillData();
            var sum1 = GetLastRowValue(dtResult1, 0);
            var sum2 = GetLastRowValue(dtResult1, 1);
            var sum3 = GetLastRowValue(dtResult1, 2);
            var totalSum1 = sum1 + sum2 - sum3;
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var dtResult2 = cdo.FillData();
            sum1 = GetLastRowValue(dtResult2, 0);
            sum2 = GetLastRowValue(dtResult2, 1);
            sum3 = GetLastRowValue(dtResult2, 2);
            var totalSum2 = sum1 + sum2 - sum3;
            dtTables[0] = CreateReportTableDebtStructure();
            dtTables[1] = cdo.dtExchange;
            var drResult = dtTables[0].Rows.Add();
            drResult[0] = reportParams[ReportConsts.ParamYear];
            drResult[1] = ConvertTo1000(totalSum1);
            drResult[2] = ConvertTo1000(totalSum2);
            drResult[3] = ConvertTo1000(totalSum1 + totalSum2);
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Обязательства бюджета с кредитными линиями"
        /// </summary>
        public DataTable[] GetReportBudgetCreditLinesObligationsData(DateTime endDate)
        {
            var dtTables = new DataTable[3];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.AllCreditsCode);
            // Обрезаем время даты отчета
            var shortCalcDate = endDate.ToShortDateString();
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            cdo.AddDataColumn(f_S_Creditincome.Num);
            cdo.AddDataColumn(f_S_Creditincome.ContractDate);
            cdo.AddDataColumn(f_S_Creditincome.StartDate);
            cdo.AddDataColumn(f_S_Creditincome.Sum);
            cdo.AddDetailColumn(String.Format("[1](1<={0})", shortCalcDate));
            cdo.AddDataColumn(f_S_Creditincome.CurrencyBorrow);
            cdo.AddCalcColumn(CalcColumnType.cctPercentText);
            cdo.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdo.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdo.AddCalcColumn(CalcColumnType.cctCurrentRest);
            // Фишка - добавляем колонки и с их помощью делаем сортировку
            cdo.AddDataColumn(f_S_Creditincome.StartDate);
            // Накладываем условия на заполнение полей
            cdo.columnCondition.Add(0, FormFilterValue(f_S_Creditincome.RefSExtension, "-1,0,1,3,4,5,6"));
            cdo.columnCondition.Add(2, FormFilterValue(f_S_Creditincome.RefSExtension, "-1,0,1,3,4,5,6"));
            cdo.columnCondition.Add(6, FormFilterValue(f_S_Creditincome.RefSExtension, "2"));
            cdo.columnCondition.Add(8, FormFilterValue(f_S_Creditincome.RefSExtension, "2"));
            cdo.columnCondition.Add(9, FormFilterValue(f_S_Creditincome.RefSExtension, "-1,0,1,3,4,5,6"));
            // Заполнение данными
            cdo.hierarchicalSort = true;
            cdo.sortString = FormSortString(StrSortUp(f_S_Creditincome.StartDate));
            dtTables[0] = cdo.FillData();
            // Таблица заголовок
            dtTables[1] = CreateReportCaptionTable(3);
            var drCaption = dtTables[1].Rows.Add();
            drCaption[0] = shortCalcDate;
            var regionCode = GetOKTMOCode(cdo.scheme);
            drCaption[1] = String.Empty;
            
            switch (regionCode)
            {
                case "52 000 000":
                    drCaption[1] = "Омской области";
                    break;
                case "52 701 000":
                    drCaption[1] = "г.Омска";
                    break;
            }
            
            // Курсы валют
            dtTables[2] = cdo.dtExchange;
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Изменение задолженности по процентам"
        /// </summary>
        public DataTable[] GetMainDebtDifferenceData(DateTime endDate)
        {
            var dtTables = new DataTable[5];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            // Список дат для колонок
            var years = new string[1, 2];
            var shortCalcDate = endDate.ToShortDateString();

            for (var i = 0; i < 1; i++)
            {
                years[i, 0] = GetYearStart(endDate.Year + i);
                years[i, 1] = GetYearEnd(endDate.Year + i);
            }
            
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctContractNum);
            cdo.AddDetailColumn(String.Format("-[2](1<{0})[1](1<{0})", years[0, 0]));
            cdo.AddDetailColumn(String.Format("[2](1<={1}1>={0})", years[0, 0], shortCalcDate));
            cdo.AddDetailColumn(String.Format("[1](1<={1}1>={0})", years[0, 0], shortCalcDate));
            cdo.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={0})", shortCalcDate));
            cdo.AddExchangeColumn(shortCalcDate);
            // Заполнение данными
            cdo.sortString = FormSortString(StrSortUp(f_S_Creditincome.RefOKV), StrSortUp(TempFieldNames.SortCreditEndDate));
            dtTables[0] = cdo.FillData();
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdo.FillData();
            // Таблица заголовок
            dtTables[2] = CreateReportCaptionTable(3);
            var drCaption = dtTables[2].Rows.Add();
            drCaption[0] = shortCalcDate;
            dtTables[3] = cdo.dtExchange;
            dtTables[4] = FillSummaryDataSet(dtTables[0], dtTables[1], cdo.summaryColumnIndex);
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Изменение задолженности по процентам"
        /// </summary>
        public DataTable[] GetPercentDebtDifferenceData(DateTime endDate)
        {
            var dtTables = new DataTable[5];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            // Список дат для колонок
            var years = new string[1, 2];
            var shortCalcDate = endDate.ToShortDateString();

            for (var i = 0; i < 1; i++)
            {
                years[i, 0] = GetYearStart(endDate.Year + i);
                years[i, 1] = GetYearEnd(endDate.Year + i);
            }
            
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctContractNum);
            cdo.AddDetailColumn(String.Format("-[5](1<{0})[4](1<{0})", years[0, 0]));
            cdo.AddDetailColumn(String.Format("[5](1<={1}1>={0})", years[0, 0], shortCalcDate));
            cdo.AddDetailColumn(String.Format("[4](1<={1}1>={0})", years[0, 0], shortCalcDate));
            cdo.AddDetailColumn(String.Format("-[5](1<={0})[4](1<={0})", shortCalcDate));
            cdo.AddExchangeColumn(shortCalcDate);
            // Заполнение данными
            cdo.sortString = FormSortString(StrSortUp(f_S_Creditincome.RefOKV), StrSortUp(TempFieldNames.SortCreditEndDate));
            dtTables[0] = cdo.FillData();
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdo.FillData();
            // Таблица заголовок
            dtTables[2] = CreateReportCaptionTable(3);
            var drCaption = dtTables[2].Rows.Add();
            drCaption[0] = shortCalcDate;
            dtTables[3] = cdo.dtExchange;
            dtTables[4] = FillSummaryDataSet(dtTables[0], dtTables[1], cdo.summaryColumnIndex);
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Гашение основного долга"
        /// </summary>
        public DataTable[] GetExtinguishingMainDebtData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[4];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            // Список дат для колонок
            var years = new string[3, 2];
            var endDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var shortCalcDate = endDate.ToShortDateString();

            for (var i = 0; i < 3; i++)
            {
                years[i, 0] = GetYearStart(endDate.Year + i);
                years[i, 1] = GetYearEnd(endDate.Year + i);
            }
            
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            cdo.AddDataColumn(f_S_Creditincome.Num);
            cdo.AddDataColumn(f_S_Creditincome.ContractDate);
            cdo.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdo.AddDetailColumn(String.Format("+-[0](1<{0})[1](1<{0})[10](0<{0})", years[0, 0]));
            cdo.AddDetailColumn(String.Format("[1](1<{0})", shortCalcDate));
            cdo.AddDetailColumn(String.Format("[2](1>={0}1<={1})", shortCalcDate, years[0, 1]));
            cdo.AddExchangeColumn(shortCalcDate);
            cdo.AddDetailColumn(String.Format("+--+[0](1<={0})[3](1>={0}1<{1})[1](1<={0})[2](1>={0}1<{1})[10](0<{1})",
                    shortCalcDate, years[1, 0]));
            cdo.AddDetailColumn(String.Format("[2](1>={0}1<={1})", years[1, 0], years[1, 1]));
            cdo.AddExchangeColumn(shortCalcDate);
            cdo.AddDetailColumn(String.Format("+--+[0](1<={0})[3](1>={0}1<{1})[1](1<={0})[2](1>={0}1<{1})[10](0<{1})",
                    shortCalcDate, years[2, 0]));
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            // Заполнение данными
            cdo.sortString = FormSortString(StrSortUp(f_S_Creditincome.RefOKV), StrSortUp(TempFieldNames.SortCreditEndDate));
            dtTables[0] = cdo.FillData();
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdo.FillData();
            // Таблица заголовок
            dtTables[2] = CreateReportCaptionTable(5);
            var drCaption = dtTables[2].Rows.Add();
            drCaption[0] = shortCalcDate;
            drCaption[1] = endDate.Year;
            drCaption[2] = endDate.Year + 1;
            drCaption[3] = dtTables[0].Select(String.Format("{0} <> {1}", f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr)).Length > 0;
            drCaption[4] = dtTables[1].Select(String.Format("{0} <> {1}", f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr)).Length > 0;
            dtTables[3] = cdo.dtExchange;
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Расходы на обслуживание долга за период"
        /// </summary>
        public DataTable[] GetChargesServicePeriodData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[4];
            var calcDateStart = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var calcDateEnd = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.reportParams.Add(ReportConsts.ParamLoDate, calcDateStart);
            cdo.reportParams.Add(ReportConsts.ParamHiDate, calcDateEnd);
            cdo.mainFilter.Add(f_S_Creditincome.StartDate,
                String.Format("<='{0}' and c.{3} >='{1}' or c.{2}>='{0}' and c.{2}<='{1}' or c.{3}>='{0}' and c.{3}<='{1}'",
                calcDateStart, calcDateEnd, f_S_Creditincome.StartDate, f_S_Creditincome.EndDate));
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSExtension, "-1,0,1,2");
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            cdo.AddDataColumn(f_S_Creditincome.Num);
            cdo.AddDataColumn(f_S_Creditincome.ContractDate);
            cdo.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdo.AddDataColumn(f_S_Creditincome.Sum);
            cdo.AddCalcColumn(CalcColumnType.cctReportPeriod);
            cdo.AddCalcColumn(CalcColumnType.cctPercentText);
            cdo.AddDetailColumn(String.Format("[5](1>={0}1<={1})", calcDateStart, calcDateEnd));
            cdo.AddCalcNamedColumn(CalcColumnType.cctCreditEndDate, TempFieldNames.SortEndDate);
            cdo.sortString = FormSortString(StrSortDown(f_S_Creditincome.RefOKV), StrSortUp(TempFieldNames.SortEndDate));
            // Организации
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtTables[0] = cdo.FillData();
            // Бюдгеты
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdo.FillData();
            dtTables[2] = cdo.dtExchange;
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(2);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = calcDateStart;
            drCaption[1] = calcDateEnd;
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Сроки гашения кредитов"
        /// </summary>
        public DataTable[] GetCreditExtinguishingData()
        {
            var dtTables = new DataTable[5];
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSExtension, "-1,0,1,2");
            // Формирование колонок
            cdo.AddCalcNamedColumn(CalcColumnType.cctCreditEndDate, TempFieldNames.SortEndDate);
            cdo.AddCalcColumn(CalcColumnType.cctCalcSum);
            cdo.AddDetailColumn(String.Format("if[0](1<{0})>[2](1<={1});[0](1<{2});-[2](1<={1})[0](1<{0})",
                GetYearStart(DateTime.Now.Year), GetYearEnd(DateTime.Now.Year), DateTime.MinValue.ToShortDateString()));
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            cdo.AddCalcColumn(CalcColumnType.cctNumDateOKV);
            cdo.sortString = FormSortString(StrSortDown(f_S_Creditincome.RefOKV), StrSortUp(TempFieldNames.SortEndDate));
            // Организации
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtTables[0] = cdo.FillData();
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdo.FillData();
            dtTables[2] = dtTables[0].Clone();
            dtTables[2].ImportRow(GetLastRow(dtTables[0]));
            dtTables[2].ImportRow(GetLastRow(dtTables[1]));
            var dtSummary = FillSummaryDataSet(dtTables[0], dtTables[1], cdo.summaryColumnIndex);
            dtTables[2].ImportRow(GetLastRow(dtSummary));
            dtTables[0].Rows.RemoveAt(GetLastRowIndex(dtTables[0]));
            dtTables[1].Rows.RemoveAt(GetLastRowIndex(dtTables[1]));
            dtTables[3] = cdo.dtExchange;
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(1);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = DateTime.Now.Year;
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Расходы на обслуживание долга"
        /// </summary>
        public DataTable[] GetChargesDebtDataOmsk(Dictionary<string, string> reportParams)
        {
            var yearStart = new string[4];
            var yearEnd = new string[4];
            var dtTables = new DataTable[4];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var year = Convert.ToDateTime(calcDate).Year;
            
            for (var i = 0; i < yearStart.Length; i++)
            {
                yearStart[i] = GetYearStart(year + i);
                yearEnd[i] = GetYearEnd(year + i);
            }

            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdo.mainFilter.Add(f_S_Creditincome.RefSExtension, "-1,0,1,2");
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctOrganization);
            cdo.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdo.AddDataColumn(f_S_Creditincome.Num);
            cdo.AddDataColumn(f_S_Creditincome.ContractDate);
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            cdo.AddCalcColumn(CalcColumnType.cctPercentText);
            cdo.AddDetailColumn(String.Format("+-[0](1<{0})[1](1<{0})[10](1<{0})", yearStart[0]));
            cdo.AddDetailColumn(String.Format("[4](1>={0}1<={1})", yearStart[0], yearEnd[0]));
            cdo.AddDetailColumn(String.Format("+-[0](1<={0})[1](1<={0})[10](1<={0})", calcDate));
            cdo.AddDetailColumn(String.Format("[5](1>={0}1<={1})", calcDate, yearEnd[0]));
            
            for (var i = 0; i < 3; i++)
            {
                cdo.AddDetailColumn(String.Format("+--+[0](1<{0})[3](1>={0}1<={1})[1](1<{1})[2](1>={0}1<={1})[10](1<{1})", calcDate, yearStart[1 + i]));
                cdo.AddDetailColumn(String.Format("[5](1>={0}1<={1})", yearStart[1 + i], yearEnd[1 + i]));
            }

            cdo.AddDetailColumn(String.Format("+--+[0](1<{0})[3](1>={0}1<{1})[1](1<{1})[2](1>={0}1<{1})[10](1<{1})", calcDate, yearEnd[3]));
            cdo.AddCalcNamedColumn(CalcColumnType.cctCreditEndDate, TempFieldNames.SortEndDate);
            cdo.sortString = FormSortString(StrSortDown(f_S_Creditincome.RefOKV), StrSortUp(TempFieldNames.SortEndDate));
            // Организации
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtTables[0] = cdo.FillData();
            // Бюдгеты
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdo.FillData();
            dtTables[2] = cdo.dtExchange;
            dtTables[dtTables.Length - 1] = FillSummaryDataSet(dtTables[0], dtTables[1], cdo.summaryColumnIndex);
            var drCaption = dtTables[dtTables.Length - 1].Rows[0];
            drCaption[0] = calcDate;
            drCaption[1] = year;
            return dtTables;
        }

        /// <summary>
        /// получение задолженности на определенный период времени по всем договорам
        /// </summary>
        public DataTable[] GetReportData(DateTime calculateDate, string creditSource, string filter)
        {
            var dtTables = new DataTable[1];
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var calcDate = calculateDate.ToShortDateString();
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.AddDataColumn(f_S_Creditincome.id);
            cdoCredit.AddDataColumn(f_S_Creditincome.Num);
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditTypeNumStartDate);
            cdoCredit.AddParamColumn(CalcColumnType.cctRelation, "+10");
            cdoCredit.AddParamColumn(CalcColumnType.cctRelation, "+11;+12;-13");
            cdoCredit.AddParamColumn(CalcColumnType.cctRelation, "+6;+7;+8;-9");
            // 06 planDebt - factDebt (calcDate)
            cdoCredit.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={0})", calcDate));
            // 07 planPercent - factPercent (cclcDate)
            cdoCredit.AddDetailColumn(String.Format("-[5](1<={0})[4](1<={0})", calcDate));
            // 08 planChargeDebt + planChargePercent (cclcDate)
            cdoCredit.AddDetailColumn(String.Format("+[6](1<={0})[7](1<={0})", calcDate));
            // 09 factChargeDebt + factChargePercent (cclcDate)
            cdoCredit.AddDetailColumn(String.Format("+[9](1<={0})[8](1<={0})", calcDate));
            // 10 planDebt - factDebt (maxDate)
            cdoCredit.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={0})", maxDate));
            // 11 planPercent - factPercent (maxDate)
            cdoCredit.AddDetailColumn(String.Format("-[5](1<={0})[4](1<={0})", maxDate));
            // 12 planChargeDebt + planChargePercent (maxDate)
            cdoCredit.AddDetailColumn(String.Format("+[6](1<={0})[7](1<={0})", maxDate));
            // 13 factChargeDebt + factChargePercent (maxDate)
            cdoCredit.AddDetailColumn(String.Format("+[9](1<={0})[8](1<={0})", maxDate));
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = filter;
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = ReportConsts.StatusAccepted;
            var dtReport = cdoCredit.FillData();
            var drCaption = dtReport.NewRow();
            drCaption[1] = String.Format(
                "Фактическая задолженность по кредитам, полученным областью от {1} на {0} в руб. и коп.",
                calcDate, creditSource);
            drCaption[3] = GetLastRowValue(dtReport, 3);
            drCaption[4] = GetLastRowValue(dtReport, 4);
            drCaption[5] = GetLastRowValue(dtReport, 5);
            dtReport.Rows.InsertAt(drCaption, 0);
            dtReport.Rows.RemoveAt(GetLastRowIndex(dtReport));
            dtTables[0] = dtReport;
            return dtTables;
        }

        public DataTable[] GetBudgetReportData(DateTime calculateDate)
        {
            return GetReportData(calculateDate, "других бюджетов", ReportConsts.BudCreditCode);
        }

        public DataTable[] GetOrganizationReportData(DateTime calculateDate)
        {
            return GetReportData(calculateDate, "кредитных организаций", ReportConsts.OrgCreditCode);
        }
    }
}