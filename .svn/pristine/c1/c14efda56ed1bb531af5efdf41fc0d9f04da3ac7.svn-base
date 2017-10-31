using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.FactTables.DebtBook;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Planning.Data;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// Московская область Отчет «Курсовая разница»
        /// </summary>
        public DataTable[] GetMOExchangeRateData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var valFields = CreateValuePair(ReportConsts.CurrencySumField);
            var startDate = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exRate1 = GetDecimal(reportParams[ReportConsts.ParamStartExchangeRate]);
            var exRate2 = GetDecimal(reportParams[ReportConsts.ParamExchangeRate]);
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            cdoGrnt.ignoreCurrencyCalc = true;
            cdoGrnt.planServiceDate = calcDate;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefOKV] = String.Format("<>{0}", ReportConsts.codeRUB);
            // Формирование колонок
            const string formulaDbt = "--[0](1<{0})[1](1<{1})[2](1<{1})";
            const string formulaPct = "--[5](1<{0})[4](1<{1})[2](1<{1})";
            // 00
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            // 01
            cdoGrnt.AddDetailColumn(String.Format(formulaDbt, maxDate, startDate), valFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);
            // 02
            cdoGrnt.AddDetailColumn(String.Format(formulaDbt, maxDate, calcDate), valFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);
            // 03
            cdoGrnt.AddDetailColumn(String.Format(formulaPct, maxDate, startDate), valFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // 04
            cdoGrnt.AddDetailColumn(String.Format(formulaPct, maxDate, calcDate), valFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // Заполнение данными
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.Num);
            dtTables[0] = cdoGrnt.FillData();
            var exRate = new UndercutExchangeUniUsdPrevRate { ParentValue = startDate };
            var rate1 = exRate.NewValue;
            var actualLoDate = Convert.ToDateTime(exRate.ActualDate).ToShortDateString();
            exRate.ParentValue = calcDate;
            var rate2 = exRate.NewValue;
            var actualHiDate = Convert.ToDateTime(exRate.ActualDate).ToShortDateString();
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = startDate;
            drCaption[2] = exRate1;
            drCaption[3] = exRate2;
            drCaption[4] = actualLoDate;
            drCaption[5] = actualHiDate;
            return dtTables;
        }

        /// <summary>
        /// МО - Карточка учета долга по гарантии
        /// </summary>
        public DataTable[] GetGarantDebtHistoryMOData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[11];
            var regNum = reportParams[ReportConsts.ParamRegNum];
            var regNumFilter = GetMOSelectedRegNumFilter(regNum);
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter[f_S_Guarantissued.Num] = regNumFilter;
            cdoGrnt.useSummaryRow = false;
            cdoGrnt.filterLastPlanService = false;
            // Формирование колонок
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKVName);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalPercent);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalPeriod);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            // служебные
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalDocExist);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RefOKV);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVFullName);
            cdoGrnt.AddDetailColumn(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}][{7}][{8}][{9}][{10}]",
                t_S_PlanAttractPrGrnt.key,
                t_S_PlanDebtPrGrnt.key,
                t_S_FactAttractPrGrnt.key,
                t_S_FactDebtPrGrnt.key,
                t_S_FactAttractGrnt.key,
                t_S_PlanServicePrGrnt.key,
                t_S_FactPercentPrGrnt.key,
                t_S_ChargePenaltyDebtPrGrnt.key,
                t_S_PrGrntChargePenaltyPercent.key,
                t_S_FactPenaltyDebtPrGrnt.key,
                t_S_FactPenaltyPercentPrGrnt.key));
            // Заполнение данными
            dtTables[0] = cdoGrnt.FillData();

            var rowGrnt = GetLastRow(dtTables[0]);

            if (rowGrnt != null)
            {
                rowGrnt[2] = String.Format("{0} \\ {1}", rowGrnt[12], rowGrnt[11]);
                var sumField = ReportConsts.SumField;
                var refOkv = Convert.ToInt32(rowGrnt[13]);
                var masterKey = Convert.ToInt32(rowGrnt[f_S_Guarantissued.id]);

                var planServiceDate1 = String.Empty;
                var planServiceDate2 = String.Empty;

                var filterEstimtDate = String.Format("{0} = {1}", cdoGrnt.GetParentRefName(), masterKey);
                var tblPlanService = DataTableUtils.FilterDataSet(
                    cdoGrnt.dtDetail[Convert.ToInt32(t_S_PlanServicePrGrnt.key)],
                    filterEstimtDate);
                tblPlanService = DataTableUtils.SortDataSet(tblPlanService, t_S_PlanServicePrGrnt.EstimtDate);

                if (tblPlanService.Rows.Count > 0)
                {
                    planServiceDate1 = Convert.ToString(GetFirstRow(tblPlanService)[t_S_PlanServicePrGrnt.EstimtDate]);
                    planServiceDate2 = Convert.ToString(GetLastRow(tblPlanService)[t_S_PlanServicePrGrnt.EstimtDate]);
                }

                if (Convert.ToInt32(rowGrnt[14]) == 0)
                {
                    refOkv = Convert.ToInt32(rowGrnt[15]);
                    rowGrnt[5] = rowGrnt[16];
                }

                if (refOkv != ReportConsts.codeRUB)
                {
                    // изменения по гарантиям
                    var objGrntAlter = new CommonDataObject();
                    objGrntAlter.InitObject(scheme);
                    objGrntAlter.ObjectKey = t_S_AlterationGrnt.internalKey;
                    objGrntAlter.useSummaryRow = false;
                    objGrntAlter.mainFilter[t_S_AlterationGrnt.RefGrnt] = Convert.ToString(rowGrnt[f_S_Guarantissued.id]);
                    objGrntAlter.AddDataColumn(t_S_AlterationGrnt.Sum);
                    objGrntAlter.AddDataColumn(t_S_AlterationGrnt.ChargeDate, ReportConsts.ftDateTime);
                    objGrntAlter.sortString = t_S_AlterationGrnt.ChargeDate;
                    var tblAlter = objGrntAlter.FillData();
                    var rowAlter = GetLastRow(tblAlter);
                    
                    if (rowAlter != null)
                    {
                        rowGrnt[f_S_Guarantissued.Sum] = rowAlter[t_S_AlterationGrnt.Sum];
                    }

                    sumField = ReportConsts.CurrencySumField;
                    var fltNonEmptySum = String.Format("{0} > 0", sumField);
                    // факт привлечения
                    var factAttrPrKey = Convert.ToInt32(t_S_FactAttractPrGrnt.key);
                    cdoGrnt.dtDetail[factAttrPrKey] = DataTableUtils.FilterDataSet(
                        cdoGrnt.dtDetail[factAttrPrKey], fltNonEmptySum);
                    // факт погашения
                    var factDebtPrKey = Convert.ToInt32(t_S_FactDebtPrGrnt.key);
                    cdoGrnt.dtDetail[factDebtPrKey] = DataTableUtils.FilterDataSet(
                        cdoGrnt.dtDetail[factDebtPrKey], fltNonEmptySum);
                    // факт исполнения
                    var tblFactAttrKey = Convert.ToInt32(t_S_FactAttractGrnt.key);
                    cdoGrnt.dtDetail[tblFactAttrKey] = DataTableUtils.FilterDataSet(
                        cdoGrnt.dtDetail[tblFactAttrKey], fltNonEmptySum);
                    // факт погашения процентов
                    var tblFactPctKey = Convert.ToInt32(t_S_FactPercentPrGrnt.key);
                    cdoGrnt.dtDetail[tblFactPctKey] = DataTableUtils.FilterDataSet(
                        cdoGrnt.dtDetail[tblFactPctKey], fltNonEmptySum);
                }

                var datesList = new Collection<string>();
                var loDate = DateTime.MinValue.ToShortDateString();
                var hiDate = calcDate;
                var refField = cdoGrnt.GetParentRefName();

                // 1. Формирование таблицы «Движение основного долга»

                // 1.1 Формирование таблицы «План»

                // список изменений по плану привлечения
                var detailIndex1 = Convert.ToInt32(t_S_PlanAttractPrGrnt.key);
                var tblDetail1 = cdoGrnt.dtDetail[detailIndex1];
                var dateField1 = t_S_PlanAttractPrGrnt.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                // список изменений по плану погашения
                var detailIndex2 = Convert.ToInt32(t_S_PlanDebtPrGrnt.key);
                var tblDetail2 = cdoGrnt.dtDetail[detailIndex2];
                var dateField2 = t_S_PlanDebtPrGrnt.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, maxDate, refField, masterKey);

                var tableDebtMoving = CreateReportCaptionTable(9);
                foreach (string t in datesList)
                {
                    var rowDebt = tableDebtMoving.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[0] = t;
                    rowDebt[3] = cdoGrnt.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 7, cdoGrnt.sumIncludedRows, t_S_PlanAttractPrGrnt.Note);
                    rowDebt[4] = cdoGrnt.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 7, cdoGrnt.sumIncludedRows, t_S_PlanDebtPrGrnt.Note);
                }

                dtTables[1] = tableDebtMoving;

                // 1.2 Формирование таблицы «Факт»

                datesList.Clear();
                // список изменений по факту привлечения
                detailIndex1 = Convert.ToInt32(t_S_FactAttractPrGrnt.key);
                tblDetail1 = cdoGrnt.dtDetail[detailIndex1];
                dateField1 = t_S_FactAttractPrGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, hiDate, refField, masterKey);
                // список изменений по факту погашения
                detailIndex2 = Convert.ToInt32(t_S_FactDebtPrGrnt.key);
                tblDetail2 = cdoGrnt.dtDetail[detailIndex2];
                dateField2 = t_S_FactDebtPrGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);
                // список изменений по факту исполнения принципалом
                var factFilter = String.Format("{0} = {1} or {0} = {2}", t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumMainDbt, ReportConsts.GrntTypeSumUndefined);
                var detailIndex3 = Convert.ToInt32(t_S_FactAttractGrnt.key);
                var tblDetail3 = DataTableUtils.FilterDataSet(cdoGrnt.dtDetail[detailIndex3], factFilter);
                var dateField3 = t_S_FactAttractGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail3, dateField3, loDate, hiDate, refField, masterKey);

                var tableDebtFact = CreateReportCaptionTable(13);
                foreach (string t in datesList)
                {
                    var rowDebt = tableDebtFact.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[0] = t;
                    rowDebt[3] = cdoGrnt.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    rowDebt[6] = cdoGrnt.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 09, cdoGrnt.sumIncludedRows, t_S_FactDebtPrGrnt.Note);
                    CombineCellValues(rowDebt, 11, cdoGrnt.sumIncludedRows, t_S_FactDebtPrGrnt.NumPayOrder);
                    rowDebt[7] = cdoGrnt.GetSumValue(tblDetail3, masterKey, dateField3, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 9, cdoGrnt.sumIncludedRows, t_S_FactAttractGrnt.Note);
                    CombineCellValues(rowDebt, 11, cdoGrnt.sumIncludedRows, t_S_FactAttractGrnt.NumPayOrder);
                }
                
                dtTables[2] = tableDebtFact;

                // 2. Формирование раздела «Проценты»

                // 2.1 Формирование таблицы «План»

                datesList.Clear();
                // список изменений по факту привлечения
                detailIndex1 = Convert.ToInt32(t_S_PlanServicePrGrnt.key);
                tblDetail1 = cdoGrnt.dtDetail[detailIndex1];
                dateField1 = t_S_PlanServicePrGrnt.StartDate;

                if (planServiceDate1.Length > 0)
                {
                    var filterStr = String.Format("{0} = '{1}'", t_S_PlanServicePrGrnt.EstimtDate, planServiceDate1);
                    tblDetail1 = DataTableUtils.FilterDataSet(tblDetail1, filterStr);
                }

                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                var tablePercentPlan = CreateReportCaptionTable(8);
                
                foreach (string t in datesList)
                {
                    var rowDebt = tablePercentPlan.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[3] = cdoGrnt.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 00, cdoGrnt.sumIncludedRows, t_S_PlanServicePrGrnt.StartDate, true);
                    CombineCellValues(rowDebt, 01, cdoGrnt.sumIncludedRows, t_S_PlanServicePrGrnt.EndDate, true);
                    CombineCellValues(rowDebt, 02, cdoGrnt.sumIncludedRows, t_S_PlanServicePrGrnt.DayCount, true);
                    CombineCellValues(rowDebt, 04, cdoGrnt.sumIncludedRows, t_S_PlanServicePrGrnt.PaymentDate, true);
                }

                dtTables[3] = tablePercentPlan;

                // 2.2 Формирование таблицы «Факт»

                datesList.Clear();
                // список изменений по факту привлечения
                detailIndex1 = Convert.ToInt32(t_S_PlanServicePrGrnt.key);
                tblDetail1 = cdoGrnt.dtDetail[detailIndex1];
                dateField1 = t_S_PlanServicePrGrnt.StartDate;

                if (planServiceDate2.Length > 0)
                {
                    var filterStr = String.Format("{0} = '{1}'", t_S_PlanServicePrGrnt.EstimtDate, planServiceDate2);
                    tblDetail1 = DataTableUtils.FilterDataSet(tblDetail1, filterStr);
                }

                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                var tablePercentAttr = CreateReportCaptionTable(5);
                foreach (string t in datesList)
                {
                    var rowDebt = tablePercentAttr.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[3] = cdoGrnt.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 00, cdoGrnt.sumIncludedRows, t_S_PlanServicePrGrnt.StartDate, true);
                    CombineCellValues(rowDebt, 01, cdoGrnt.sumIncludedRows, t_S_PlanServicePrGrnt.EndDate, true);
                    CombineCellValues(rowDebt, 02, cdoGrnt.sumIncludedRows, t_S_PlanServicePrGrnt.DayCount, true);
                }

                dtTables[4] = tablePercentAttr;

                datesList.Clear();

                // список изменений по факту погашения
                detailIndex2 = Convert.ToInt32(t_S_FactPercentPrGrnt.key);
                tblDetail2 = cdoGrnt.dtDetail[detailIndex2];
                dateField2 = t_S_FactPercentPrGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);
                // список изменений по факту исполнения принципалом
                factFilter = String.Format("{0} = {1}", t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
                detailIndex3 = Convert.ToInt32(t_S_FactAttractGrnt.key);
                tblDetail3 = DataTableUtils.FilterDataSet(cdoGrnt.dtDetail[detailIndex3], factFilter);
                dateField3 = t_S_FactAttractGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail3, dateField3, loDate, hiDate, refField, masterKey);

                var tablePercentDebt = CreateReportCaptionTable(8);
                foreach (string t in datesList)
                {
                    var rowDebt = tablePercentDebt.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    rowDebt[0] = t;
                    rowDebt[2] = cdoGrnt.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 04, cdoGrnt.sumIncludedRows, t_S_FactPercentPrGrnt.Note);
                    CombineCellValues(rowDebt, 06, cdoGrnt.sumIncludedRows, t_S_FactPercentPrGrnt.NumPayOrder);
                    rowDebt[3] = cdoGrnt.GetSumValue(tblDetail3, masterKey, dateField3, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 04, cdoGrnt.sumIncludedRows, t_S_FactAttractGrnt.Note);
                    CombineCellValues(rowDebt, 06, cdoGrnt.sumIncludedRows, t_S_FactAttractGrnt.NumPayOrder);
                }

                dtTables[5] = tablePercentDebt;

                // 2.3 Формирование таблицы «Комиссии»

                dtTables[6] = CreateReportCaptionTable(1);

                // 2.4 Формирование таблицы «Санкции»

                datesList.Clear();
                // список изменений по Начисление пени по основному долгу
                detailIndex1 = Convert.ToInt32(t_S_ChargePenaltyDebtPrGrnt.key);
                tblDetail1 = cdoGrnt.dtDetail[detailIndex1];
                dateField1 = t_S_ChargePenaltyDebtPrGrnt.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                // список изменений по Начисление пени по процентам
                detailIndex2 = Convert.ToInt32(t_S_PrGrntChargePenaltyPercent.key);
                tblDetail2 = cdoGrnt.dtDetail[detailIndex2];
                dateField2 = t_S_PrGrntChargePenaltyPercent.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, maxDate, refField, masterKey);

                var tablePeniCharge = CreateReportCaptionTable(6);
                foreach (string t in datesList)
                {
                    var rowDebt = tablePeniCharge.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    var chargeSumDbt = cdoGrnt.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 03, cdoGrnt.sumIncludedRows, t_S_ChargePenaltyDebtPrGrnt.StartDate, true);
                    var chargeSumPct = cdoGrnt.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 03, cdoGrnt.sumIncludedRows, t_S_PrGrntChargePenaltyPercent.StartDate, true);
                    rowDebt[4] = chargeSumDbt + chargeSumPct;
                }

                dtTables[7] = tablePeniCharge;

                datesList.Clear();
                // список изменений по Факт погашения пени по основному долгу
                detailIndex1 = Convert.ToInt32(t_S_FactPenaltyDebtPrGrnt.key);
                tblDetail1 = cdoGrnt.dtDetail[detailIndex1];
                dateField1 = t_S_FactPenaltyDebtPrGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, hiDate, refField, masterKey);
                // список изменений по Факт погашения пени по процентам
                detailIndex2 = Convert.ToInt32(t_S_FactPenaltyPercentPrGrnt.key);
                tblDetail2 = cdoGrnt.dtDetail[detailIndex2];
                dateField2 = t_S_FactPenaltyPercentPrGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);
                // список изменений по Факт исполнения по гарантии
                factFilter = String.Format("{0} = {1} or {0} = {2} or {0} = {3}",
                    t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPeniDbt, ReportConsts.GrntTypeSumPeniPct, ReportConsts.GrntTypeSumFine);
                detailIndex3 = Convert.ToInt32(t_S_FactAttractGrnt.key);
                tblDetail3 = DataTableUtils.FilterDataSet(cdoGrnt.dtDetail[detailIndex3], factFilter);
                dateField3 = t_S_FactAttractGrnt.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail3, dateField3, loDate, hiDate, refField, masterKey);

                var tablePeniFact = CreateReportCaptionTable(12);
                
                foreach (var t in datesList)
                {
                    var rowDebt = tablePeniFact.Rows.Add();
                    var changeDate = Convert.ToDateTime(t);
                    var factSumDbt = cdoGrnt.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 0, cdoGrnt.sumIncludedRows, t_S_FactPenaltyDebtPrGrnt.FactDate, true);
                    CombineCellValues(rowDebt, 7, cdoGrnt.sumIncludedRows, t_S_FactPenaltyDebtPrGrnt.Note);
                    CombineCellValues(rowDebt, 9, cdoGrnt.sumIncludedRows, t_S_FactPenaltyDebtPrGrnt.NumPayOrder);
                    var factSumPct = cdoGrnt.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 0, cdoGrnt.sumIncludedRows, t_S_FactPenaltyPercentPrGrnt.FactDate, true);
                    CombineCellValues(rowDebt, 7, cdoGrnt.sumIncludedRows, t_S_FactPenaltyPercentPrGrnt.Note);
                    CombineCellValues(rowDebt, 9, cdoGrnt.sumIncludedRows, t_S_FactPenaltyPercentPrGrnt.NumPayOrder);
                    rowDebt[5] = cdoGrnt.GetSumValue(tblDetail3, masterKey, dateField3, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 0, cdoGrnt.sumIncludedRows, t_S_FactAttractGrnt.FactDate, true);
                    CombineCellValues(rowDebt, 7, cdoGrnt.sumIncludedRows, t_S_FactAttractGrnt.Note);
                    CombineCellValues(rowDebt, 9, cdoGrnt.sumIncludedRows, t_S_FactAttractGrnt.NumPayOrder);
                    rowDebt[3] = factSumDbt + factSumPct;
                }

                dtTables[8] = tablePeniFact;

                dtTables[9] = CreateReportCaptionTable(1);
            }
            else
            {
                for (var i = 1; i < dtTables.Length - 1; i++)
                {
                    dtTables[0] = CreateReportCaptionTable(50);
                }
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = regNum;
            FillSignatureData(drCaption, 4, reportParams, ReportConsts.ParamExecutor1, ReportConsts.ParamExecutor2, ReportConsts.ParamExecutor3);
            return dtTables;
        }

        /// <summary>
        /// Саратов - Структура для таблицы результатов отчета "Программа гарантий"
        /// </summary>
        public DataTable[] GetGarantProgrammSaratovData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new GarantDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Guarantissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Guarantissued.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            cdo.mainFilter.Add(f_S_Guarantissued.EndDate, String.Format(">='{0}'", GetYearStart(year)));
            cdo.mainFilter.Add(f_S_Guarantissued.StartDate, String.Format("<='{0}'", GetYearEnd(year)));
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctPosition);
            cdo.AddDataColumn(f_S_Guarantissued.Purpose);
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+6;+7");
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+6;+7");
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+6;+7");
            cdo.AddDataColumn(f_S_Guarantissued.Sum);
            cdo.AddDetailColumn(String.Format("[1](1<={0})", maxDate));
            // Заполнение данными
            cdo.sortString = StrSortUp(TempFieldNames.OrgName);
            dtTables[0] = cdo.FillData();
            CorrectThousandSumValue(dtTables[0], 3);
            CorrectThousandSumValue(dtTables[0], 4);
            CorrectThousandSumValue(dtTables[0], 5);
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = reportParams[ReportConsts.ParamYear];
            return dtTables;
        }

        /// <summary>
        /// Московская обсласть - Государственные гарантии
        /// </summary>
        public DataTable[] GetMOGarantInfoData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var variantFilter = ReportConsts.FixedVariantsID;
            
            if (reportParams[ReportConsts.ParamVariantType] == "i1")
            {
                variantFilter = ReportConsts.ActiveVariantID;
            }

            var exchangeRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var sumFieldRub = CreateValuePair(ReportConsts.SumField);
            var sumFieldUsd = CreateValuePair(ReportConsts.CurrencySumField);
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.planServiceDate = calcDate;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = variantFilter;
            // 00
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctOrganization3, TempFieldNames.OrgName);
            // 01
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            // 02
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 03
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 04
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 05
            cdoGrnt.AddDetailColumn(String.Format("--[0](1<{0})[1](1<{0})[2](1<{0})", calcDate), sumFieldRub);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);
            // 06
            cdoGrnt.AddDetailColumn(String.Format("--[5](1<{1})[4](1<{0})[2](1<{0})", calcDate, maxDate), sumFieldRub);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // 07
            cdoGrnt.AddDetailTextColumn(String.Format("[3](1<{0})", maxDate), cdoGrnt.ParamOnlyDates, String.Empty);
            // 08
            cdoGrnt.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), String.Empty, String.Empty);
            // служебные
            // 09
            cdoGrnt.AddDetailColumn(String.Format("[0](1<{0})", maxDate), sumFieldUsd, true);
            // 10
            cdoGrnt.AddDetailColumn(String.Format("--[0](1<{0})[1](1<{0})[2](1<{0})", calcDate), sumFieldUsd, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);
            // 11
            cdoGrnt.AddDetailColumn(String.Format("--[5](1<{1})[4](1<{0})[2](1<{0})", calcDate, maxDate), sumFieldUsd, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // 12
            cdoGrnt.AddDetailTextColumn(String.Format("[5](1>{0})", calcDate), String.Empty, String.Empty);
            // 13
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum);
            // 14
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalCurrencySum);
            // 15
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            // 16
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalPercent);
            // 17
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, "5");
            // 18
            cdoGrnt.AddDetailTextColumn(String.Format("[5](1<{0})", maxDate), String.Empty, String.Empty);
            // 19
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 20
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            // 21
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.SortStatus);
            // 22
            cdoGrnt.AddDetailColumn(String.Format("[5](1<{0})", maxDate), sumFieldRub);
            // 23
            cdoGrnt.AddDetailTextColumn(String.Format("[5](1>{0})", calcDate), cdoGrnt.ParamFieldList, sumFieldUsd);
            // 24
            cdoGrnt.AddDetailTextColumn(String.Format("[5](1<{0})", maxDate), cdoGrnt.ParamFieldList, sumFieldUsd);
            cdoGrnt.columnParamList[23].Add(cdoGrnt.ParamIgnoreExchange, ReportConsts.strTrue);
            cdoGrnt.columnParamList[24].Add(cdoGrnt.ParamIgnoreExchange, ReportConsts.strTrue);
            // 25
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, "0");
            // 26
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            // 27
            cdoGrnt.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoGrnt.ParamOnlyDates, sumFieldRub);
            cdoGrnt.sortString = FormSortString(StrSortUp(TempFieldNames.SortStatus), StrSortUp(TempFieldNames.OrgName));
            var tableGrnt = cdoGrnt.FillData();

            for (var i = 0; i < tableGrnt.Rows.Count - 1; i++)
            {
                var rowData = tableGrnt.Rows[i];
                var refOkv = Convert.ToInt32(rowData[15]);

                // заполненность процентов
                if (rowData[2].ToString() == String.Empty)
                {
                    rowData[2] = rowData[16];
                }

                var documentSum = GetNumber(rowData[13]);

                // а гарантия то валютная
                rowData[4] = rowData[22];
                
                if (refOkv != ReportConsts.codeRUB)
                {
                    rowData[3] = exchangeRate * GetNumber(rowData[20]);
                    rowData[4] = "USDTEXTSTUB1";
                    rowData[5] = rowData[10];
                    rowData[6] = exchangeRate * GetNumber(rowData[11]);
                    documentSum = GetNumber(rowData[14]);
                }

                var planDebtRecordCnt = Convert.ToInt32(rowData[25]);
                
                if (planDebtRecordCnt == 0)
                {
                    rowData[5] = GetNumber(rowData[5]) + documentSum;
                }

                if (refOkv != ReportConsts.codeRUB)
                {
                    rowData[5] = exchangeRate * GetNumber(rowData[5]);
                }

                rowData[TempFieldNames.SortStatus] = 0;
                // для договоров с нулевым остатком не заполняем столбик Дата\исполнения обязательства, ибо уже исполнены
                if (GetNumber(rowData[5]) != 0)
                {
                    rowData[8] = String.Empty;
                    rowData[TempFieldNames.SortStatus] = 1;
                }

                int planPercentRecordCnt = Convert.ToInt32(rowData[17]);

                if (planPercentRecordCnt >= 1 && planPercentRecordCnt < 5)
                {
                    if (refOkv != ReportConsts.codeRUB)
                    {
                        rowData[7] = rowData[24];
                    }
                    else
                    {
                        rowData[7] = rowData[18];
                    }
                }
                else
                {
                    if (refOkv != ReportConsts.codeRUB)
                    {
                        rowData[7] = rowData[23];
                    }
                    else
                    {
                        rowData[7] = rowData[12];
                    }
                }
            }

            cdoGrnt.summaryColumnIndex.Add(3);
            tableGrnt = cdoGrnt.RecalcSummary(tableGrnt);
            GetLastRow(tableGrnt)[TempFieldNames.SortStatus] = 2;
            tableGrnt = ClearCloseCreditMO(cdoGrnt, tableGrnt, Convert.ToDateTime(calcDate), 5, 27, -1);
            dtTables[0] = cdoGrnt.SortDataSet(tableGrnt, cdoGrnt.sortString);

            var year = Convert.ToDateTime(calcDate).Year;
            var yearStart = GetYearStart(year);
            var yearEnd = GetYearEnd(year);

            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter[f_S_Guarantissued.StartDate] = String.Format(">='{0}' or c.{1}>='{2}'", yearStart, f_S_Guarantissued.StartDate, yearEnd);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = variantFilter;
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            cdoGrnt.summaryColumnIndex.Add(0);
            var tableSummaryAttr = cdoGrnt.FillData();
            
            for (var i = 0; i < tableSummaryAttr.Rows.Count - 1; i++)
            {
                var rowData = tableSummaryAttr.Rows[i];

                if (Convert.ToInt32(rowData[2]) != ReportConsts.codeRUB)
                {
                    rowData[0] = exchangeRate * GetNumber(rowData[1]);
                }
            }
            
            tableSummaryAttr = cdoGrnt.RecalcSummary(tableSummaryAttr);

            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = variantFilter;
            cdoGrnt.AddDetailColumn(String.Format("++[1](1>={0}1<={1})[4](1>={0}1<={1})[2](1>={0}1<={1})", yearStart, yearEnd), sumFieldRub);
            cdoGrnt.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearStart, yearEnd), sumFieldRub);
            cdoGrnt.AddDetailColumn(String.Format("+[1](1>={0}1<={1})[4](1>={0}1<={1})", yearStart, yearEnd), sumFieldRub);
            cdoGrnt.AddDetailColumn(String.Format("++[1](1>={0}1<={1})[4](1>={0}1<={1})[2](1>={0}1<={1})", yearStart, yearEnd), sumFieldUsd, true);
            cdoGrnt.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearStart, yearEnd), sumFieldUsd, true);
            cdoGrnt.AddDetailColumn(String.Format("+[1](1>={0}1<={1})[4](1>={0}1<={1})", yearStart, yearEnd), sumFieldUsd, true);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            var tableSummaryDetail = cdoGrnt.FillData();

            for (var i = 0; i < tableSummaryDetail.Rows.Count - 1; i++)
            {
                var rowData = tableSummaryDetail.Rows[i];
                
                if (Convert.ToInt32(rowData[6]) != ReportConsts.codeRUB)
                {
                    rowData[0] = exchangeRate * GetNumber(rowData[3]);
                    rowData[1] = exchangeRate * GetNumber(rowData[4]);
                    rowData[2] = exchangeRate * GetNumber(rowData[5]);
                }
            }

            tableSummaryDetail = cdoGrnt.RecalcSummary(tableSummaryDetail);

            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = year;
            drCaption[2] = exchangeRate;
            drCaption[6] = GetLastRow(tableSummaryAttr)[0];
            drCaption[7] = GetLastRow(tableSummaryDetail)[0];
            drCaption[8] = GetLastRow(tableSummaryDetail)[1];
            drCaption[9] = GetLastRow(tableSummaryDetail)[2];
            return dtTables;
        }

        /// <summary>
        /// Гарантии ДК МФРФ
        /// </summary>
        public DataTable[] GetDKMFRFSubjectGarantData(Dictionary<string, string> reportParams)
        {
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exchangeRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var rubFields = CreateValuePair(ReportConsts.SumField);
            var curFields = CreateValuePair(ReportConsts.CurrencySumField);
            var restSumFormula = String.Format("--[0](1<{1})[1](1<{0})[2](1<{0})", calcDate, maxDate);
            var percSumFormula = String.Format("--[5](1<{1})[4](1<{0})[2](1<{0})", calcDate, maxDate);
            var dtTables = new DataTable[4];
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            SetGarantFilter(cdoGrnt.mainFilter, calcDate, calcDate);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefOKV] = ReportConsts.codeRUBStr;
            cdoGrnt.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            cdoGrnt.planServiceDate = calcDate;
            // 00
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Occasion);
            // 01
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            // 02
            cdoGrnt.AddParamColumn(CalcColumnType.cctListDocs,
                FormFilterValue(t_S_ListContractGrnt.FunctionContract, ReportConsts.strFalse));
            // 03
            cdoGrnt.AddParamColumn(CalcColumnType.cctListDocs, CombineList(
                FormFilterValue(t_S_ListContractGrnt.BaseDoc, ReportConsts.strFalse),
                FormFilterValue(t_S_ListContractGrnt.FunctionContract, ReportConsts.strTrue),
                FormFilterValue(t_S_ListContractGrnt.RefViewContract, "5")));
            // 04
            cdoGrnt.AddCalcColumn(CalcColumnType.cctAlterationNumDocDate);
            // 05
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            // 06
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization2);
            // 07
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            // 08
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // 09
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDoc);
            // 10
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            // 11
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDemand);
            // 12
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DatePerformance);
            // 13
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 14
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+22;+24");
            // 15 - сортировочная
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            // 16
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+23;+25");
            // 17
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            // 18
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, t_S_FactAttractPrGrnt.key);
            // 19
            cdoGrnt.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoGrnt.ParamOnlyDates, rubFields);
            // 20
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum);
            // 21
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalCurrencySum);
            // 22
            cdoGrnt.AddDetailColumn(percSumFormula, rubFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // 23
            cdoGrnt.AddDetailColumn(percSumFormula, curFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // 24
            cdoGrnt.AddDetailColumn(restSumFormula, rubFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);
            // 25
            cdoGrnt.AddDetailColumn(restSumFormula, curFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);

            cdoGrnt.summaryColumnIndex.Add(13);
            cdoGrnt.summaryColumnIndex.Add(14);
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.StartDate);       
            dtTables[0] = cdoGrnt.FillData();
            cdoGrnt.mainFilter[f_S_Guarantissued.RefOKV] = FormNegFilterValue(ReportConsts.codeRUBStr);
            dtTables[1] = cdoGrnt.FillData();

            foreach (DataRow rowData in dtTables[0].Rows)
            {
                rowData[13] = 0;
            }

            // пересчет сумм для валютных и незаполненных
            foreach (DataRow rowData in dtTables[1].Rows)
            {
                if (rowData[17] == DBNull.Value)
                {
                    continue;
                }

                var refOKV = Convert.ToInt32(rowData[17]);
                // сумма по договору и гарантии
                var contractSum = GetNumber(rowData[20]);
                // заменяем на валютные
                if (refOKV != ReportConsts.codeRUB)
                {
                    contractSum = GetNumber(rowData[21]);
                }
                // если не заполнен Факт привлечения, берем значение из суммы по гарантируемому договору
                var factRecCount = Convert.ToInt32(rowData[18]);
                var emptyFactAttr = factRecCount == 0;
                if (emptyFactAttr)
                {
                    rowData[14] = GetNumber(rowData[14]) + contractSum;
                    rowData[16] = GetNumber(rowData[16]) + contractSum;
                }
                // для валютных пересчитываем по указанному в параметрах курсу
                if (refOKV != ReportConsts.codeRUB)
                {
                    rowData[14] = Math.Round(exchangeRate * GetNumber(rowData[16]), 2);
                }

                rowData[13] = 0;
            }

            dtTables[0] = ClearCloseCreditMO(cdoGrnt, dtTables[0], Convert.ToDateTime(calcDate), 14, 19, -1);
            dtTables[1] = ClearCloseCreditMO(cdoGrnt, dtTables[1], Convert.ToDateTime(calcDate), 14, 19, -1);
            dtTables[1] = cdoGrnt.RecalcSummary(dtTables[1]);

            // ДК
            var variantDate = String.Empty;
            var variantDK = GetDKVariantByDate(scheme, calcDate, ref variantDate);
            var subjectFilter = FormNegFilterValue(GetSubjectID(scheme, variantDate));
            var cdoDK = new CommonDataObject();
            cdoDK.InitObject(scheme);
            cdoDK.ObjectKey = f_S_SchBGuarantissued.internalKey;
            cdoDK.mainFilter[f_S_SchBGuarantissued.RefVariant] = variantDK;
            cdoDK.mainFilter[f_S_SchBGuarantissued.RefRegion] = subjectFilter;
            cdoDK.AddDataColumn(f_S_SchBGuarantissued.StaleGarantDebt);
            cdoDK.AddDataColumn(f_S_SchBGuarantissued.TotalDebt);
            cdoDK.summaryColumnIndex.Add(0);
            cdoDK.summaryColumnIndex.Add(1);
            var dtGarantSub = cdoDK.FillData();
            // гарантии поселений
            var cdoDKPos = new CommonDataObject();
            cdoDKPos.InitObject(scheme);
            cdoDKPos.ObjectKey = f_S_SchBGuarantissuedPos.internalKey;
            cdoDKPos.mainFilter[f_S_SchBGuarantissuedPos.RefVariant] = variantDK;
            cdoDKPos.mainFilter[f_S_SchBGuarantissuedPos.RefRegion] = subjectFilter;
            cdoDKPos.AddDataColumn(f_S_SchBGuarantissuedPos.StaleGarantDebt);
            cdoDKPos.AddDataColumn(f_S_SchBGuarantissuedPos.TotalDebt);
            cdoDKPos.summaryColumnIndex.Add(0);
            cdoDKPos.summaryColumnIndex.Add(1);
            var dtGarantPos = cdoDKPos.FillData();
            dtTables[2] = CreateReportCaptionTable(3, 1);
            var drLast = GetLastRow(dtTables[2]);
            drLast[0] = ReportConsts.RUB;
            drLast[1] = GetLastRowValue(dtGarantSub, 0) + GetLastRowValue(dtGarantPos, 0);
            drLast[2] = GetLastRowValue(dtGarantSub, 1) + GetLastRowValue(dtGarantPos, 1);
            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = exchangeRate;
            drCaption[2] = reportParams[ReportConsts.ParamExchangeDate];
            FillSignatureData(drCaption, 10, reportParams, ReportConsts.ParamExecutor1);
            return dtTables;
        }

        /// <summary>
        /// Изменение долговых обязательств за период
        /// </summary>
        public DataTable[] GetGarantChangesVologdaData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var startDate = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var endDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            SetGarantFilter(cdoGrnt.mainFilter, endDate, startDate);
            // 0
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGarantTypeNumStartDate);
            // 1
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // 2
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            // 3
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum);
            // 4
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            // 5
            cdoGrnt.AddDetailColumn("[0][1][3][4][5][12][9][8][11]");
            // 6 фильтровочная
            cdoGrnt.AddDataColumn(f_S_Guarantissued.id);
            // 7 сортировочная
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.RegNum);
            dtTables[0] = cdoGrnt.FillData();
            dtTables[1] = FillGarantChangesVologdaDetailData(cdoGrnt, dtTables[0], startDate, endDate);
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = startDate;
            drCaption[1] = endDate;
            return dtTables;
        }

        /// <summary>
        /// Карточка регистрации долгового обязательства
        /// </summary>
        public DataTable[] GetGarantCardVologdaData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[1];
            var startDate = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var endDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            SetGarantFilterDateDoc(cdoGrnt.mainFilter, startDate, endDate);
            // 0
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            // 1
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegDate);
            // 2
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGarantTypeNumStartDate);
            // 3
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3INNKPP);
            // 4
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganizationINNKPP);
            // 5
            cdoGrnt.AddDataColumn(f_S_Guarantissued.PrincipalDoc);
            // 6 
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Purpose);
            // 7
            cdoGrnt.AddCalcColumn(CalcColumnType.cctRegress);
            // 8
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            // 9
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 10
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DebtSum);
            // 11
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+9;-10");
            // 12
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            dtTables[0] = cdoGrnt.FillData();
            return dtTables;
        }

        /// <summary>
        /// Гарантии Самара
        /// </summary>
        public DataTable GetGarantSamaraData(Dictionary<string, string> reportParams, DataRow drCaption)
        {
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.exchangePrevDay = true;
            SetGarantFilter(cdoGrnt.mainFilter, calcDate, calcDate);
            cdoGrnt.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            const string rubFieldName = ReportConsts.SumField;
            const string usdFieldName = ReportConsts.CurrencySumField;
            var sumFieldRub = CombineList(rubFieldName, usdFieldName);
            var sumFieldUsd = CreateValuePair(usdFieldName);
            // 0
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            // 2
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Occasion);
            // 3
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate);
            // 4
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            // 5
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // 6
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            // 7
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDoc);
            // 8
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            // 9
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDemand);
            // 10
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCollateralType);
            // 11
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Recourse);
            // 12
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            // 13
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGarantCalcSum);
            // 14
            cdoGrnt.AddDataColumn(f_S_Guarantissued.PercentLiability);
            // 15
            cdoGrnt.AddDataColumn(f_S_Guarantissued.SanctionLiability);
            // 16
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 17
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+51;-52");
            // 18
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            // 19
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCalcSum);
            // 20
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            // 21
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCalcSum);
            // 22
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            // 23
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCalcSum);
            // 24
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            // 25
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCalcSum);
            // 26
            cdoGrnt.AddDetailColumn(String.Format("+[3](1<={0})[5](1<={0})", calcDate), sumFieldUsd, true);
            // 27
            cdoGrnt.AddDetailColumn(String.Format("+[3](1<={0})[5](1<={0})", calcDate), sumFieldRub);
            // 28
            cdoGrnt.AddDetailColumn(String.Format("[3](1<={0})", calcDate), sumFieldUsd, true);
            // 29
            cdoGrnt.AddDetailColumn(String.Format("[3](1<={0})", calcDate), sumFieldRub);
            // 30
            cdoGrnt.AddDetailColumn(String.Format("[5](1<={0})", calcDate), sumFieldUsd, true);
            // 31
            cdoGrnt.AddDetailColumn(String.Format("[5](1<={0})", calcDate), sumFieldRub);
            // 32
            cdoGrnt.AddDetailColumn(String.Format("+[8](1<={0})[11](0<={0})", calcDate), sumFieldUsd, true);
            // 33
            cdoGrnt.AddDetailColumn(String.Format("+[8](1<={0})[11](0<={0})", calcDate), sumFieldRub);
            // 34
            cdoGrnt.AddDetailColumn(String.Format("+[1](1<={0})[4](1<={0})", calcDate), sumFieldUsd, true);
            // 35
            cdoGrnt.AddDetailColumn(String.Format("+[1](1<={0})[4](1<={0})", calcDate), sumFieldRub);
            // 36
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={0})", calcDate), sumFieldUsd, true);
            // 37
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={0})", calcDate), sumFieldRub);
            // 38
            cdoGrnt.AddDetailColumn(String.Format("[4](1<={0})", calcDate), sumFieldUsd, true);
            // 39
            cdoGrnt.AddDetailColumn(String.Format("[4](1<={0})", calcDate), sumFieldRub);
            // 40
            cdoGrnt.AddDetailColumn(String.Format("[12](1<={0})", calcDate), sumFieldUsd, true);
            // 41
            cdoGrnt.AddDetailColumn(String.Format("[12](1<={0})", calcDate), sumFieldRub);
            // 42
            cdoGrnt.AddDetailColumn(String.Format("--+[3](1<={0})[5](1<={0})[1](1<={0})[4](1<={0})", calcDate)
                , sumFieldUsd, true);
            // 43
            cdoGrnt.AddDetailColumn(String.Format("--+[3](1<={0})[5](1<={0})[1](1<={0})[4](1<={0})", calcDate)
                , sumFieldRub);
            // 44
            cdoGrnt.AddDetailColumn(String.Format("-[3](1<={0})[1](1<={0})", calcDate), sumFieldUsd, true);
            // 45
            cdoGrnt.AddDetailColumn(String.Format("-[3](1<={0})[1](1<={0})", calcDate), sumFieldRub);
            // 46
            cdoGrnt.AddDetailColumn(String.Format("-[5](1<={0})[4](1<={0})", calcDate), sumFieldUsd, true);
            // 47
            cdoGrnt.AddDetailColumn(String.Format("-[5](1<={0})[4](1<={0})", calcDate), sumFieldRub);
            // 48
            cdoGrnt.AddDetailColumn(String.Format("--+[8](1<={0})[11](0<={0})[12](1<={0})[9](1<={0})", calcDate)
                , sumFieldUsd, true);
            // 49
            cdoGrnt.AddDetailColumn(String.Format("--+[8](1<={0})[11](0<={0})[12](1<={0})[9](1<={0})", calcDate)
                , sumFieldRub);
            // 50
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Note);
            // 51
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 52
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DebtSum);
            // 53
            cdoGrnt.AddDataColumn(f_S_Guarantissued.IsDepreciation);
            // 54
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={0})", calcDate));
            // 55
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+18;-36");
            // 56
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+19;-54");
            cdoGrnt.columnCondition.Add(12, FormNegativeFilterValue(f_S_Guarantissued.RefOKV, ReportConsts.codeRUBStr));
            cdoGrnt.columnCondition.Add(17, FormFilterValue(f_S_Guarantissued.RefOKV, ReportConsts.codeRUBStr));
            cdoGrnt.summaryColumnIndex.Add(12);
            cdoGrnt.summaryColumnIndex.Add(13);

            for (var i = 18; i < 26; i++)
            {
                cdoGrnt.summaryColumnIndex.Add(i);
            }

            var dtTable = cdoGrnt.FillData();
            
            foreach (DataRow dr in dtTable.Rows)
            {
                if (dr[f_S_Guarantissued.RefOKV] == DBNull.Value) continue;

                var okvType = Convert.ToInt32(dr[f_S_Guarantissued.RefOKV]);
                if (okvType != -1) dr[17] = DBNull.Value;
                for (var i = 0; i < 16; i++)
                {
                    if (okvType == -1) dr[18 + i * 2] = DBNull.Value;
                }

                if (!GetBoolValue(dr[53])) continue;

                for (var k = 0; k < 4; k++)
                {
                    dr[19 + k * 2] = dr[56];
                    if (okvType != -1) dr[18 + k * 2] = dr[55];
                }
            }

            dtTable = cdoGrnt.RecalcSummary(dtTable);
            FillExchangeValueSamara(drCaption, 6, cdoGrnt);
            // Приехали
            return dtTable;
        }
        
        /// <summary>
        /// Отчет по государственным гарантиям
        /// </summary>
        public DataTable[] GetGovGarantData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var monthStart = new DateTime(reportDate.Year, reportDate.Month, 1).ToShortDateString();
            decimal exchangeRate = 0;
            dtTables[0] = GetVologdaGarantData(calcDate, monthStart, ref exchangeRate, -1);
            dtTables[1] = GetVologdaGarantData(calcDate, monthStart, ref exchangeRate, 0);
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(4);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = monthStart;
            drCaption[1] = calcDate;
            drCaption[2] = exchangeRate;
            return dtTables;
        }

        private DataTable GetVologdaGarantData(string calcDate, string monthStart, ref decimal okvValue,
            int okvType)
        {
            var counter = 0;
            var sumFieldName = CreateValuePair(ReportConsts.SumField);
            var commissionFieldName = CreateValuePair(ReportConsts.Commission);
            var marginFieldName = CreateValuePair(ReportConsts.Margin);
            var okvFilter = ReportConsts.codeRUBStr;

            // Если договор валютный
            if (okvType != -1)
            {
                sumFieldName = CreateValuePair(ReportConsts.CurrencySumField);
                commissionFieldName = CreateValuePair(ReportConsts.CurrencyCommission);
                marginFieldName = CreateValuePair(ReportConsts.CurrencyMargin);
                okvFilter = FormNegFilterValue(ReportConsts.codeRUBStr);
            }
            
            const bool needConvert = false;
            var fieldNames = new Dictionary<string, string>
                                                        {
                                                            {sumFieldName, "2"},
                                                            {marginFieldName, "7"},
                                                            {commissionFieldName, "5"}
                                                        };
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefOKV] = okvFilter;
            // + 0
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            // + 1
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCreditTypeNumStartDate);
            // + 2
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // + 3
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNativeSum);
            // - 4
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            // - 5 Chapter.Month
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+6;+7;+8;+9;+10");
            // + 6 - mainDebt
            cdoGrnt.AddDetailColumn(String.Format("--+[3](1>={0}1<={1})[6](1>={0}1<={1})[0](1>={0}1<={1})[1](1>={0}1<={1})", monthStart, calcDate), sumFieldName, needConvert);
            cdoGrnt.columnParamList[counter++]["RefTypSum"] = "1";
            // + 7
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+17;-18");
            // + 8
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+19;-20");
            // + 9
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+21;-22");
            // + 10 - penalty
            cdoGrnt.AddDetailColumn(String.Format("-[6](1>={0}1<={1})[0](1>={0}1<={1})", monthStart, calcDate), 
                sumFieldName, needConvert);
            cdoGrnt.columnParamList[10]["RefTypSum"] = "3,4,6";
            // - 11 - Chapter.Date
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+12;+13;+13;+15;+16");
            // + 12 - main
            cdoGrnt.AddDetailColumn(String.Format("--[7](1<={0})[1](1<={0})[0](1<={0})",
                calcDate), sumFieldName, needConvert);
            cdoGrnt.columnParamList[counter++]["RefTypSum"] = "1";
            //+ 13 - percent
            cdoGrnt.AddDetailColumn(String.Format("--[5](1<={0})[1](1<={0})[4](1<={0})",
                calcDate), sumFieldName, needConvert);
            cdoGrnt.columnParamList[counter++]["RefTypSum"] = "2";
            // - 14 - margin
            cdoGrnt.AddDetailColumn(String.Format("-+-[6](1<={0})[0](1<={0})[5](1<={0})[4](1<={0})",
                calcDate), marginFieldName, needConvert);
            cdoGrnt.columnParamList[counter++]["RefTypSum"] = "7";
            // - 15 - commission
            cdoGrnt.AddDetailColumn(String.Format("-+-[6](1<={0})[0](1<={0})[5](1<={0})[4](1<={0})",
                calcDate), commissionFieldName, needConvert);
            cdoGrnt.columnParamList[counter++]["RefTypSum"] = "5";
            // - 16 - penalty
            cdoGrnt.AddDetailColumn(String.Format("++--[6](1<={0})[11](1<={0})[8](1<={0})[12](1<={0})[9](1<={0})",
                calcDate), commissionFieldName, needConvert);
            cdoGrnt.columnParamList[counter]["RefTypSum"] = "3,4,6";
            // служебные
            counter = 18;

            foreach (var fieldName in fieldNames.Keys)
            {
                cdoGrnt.AddDetailColumn(String.Format("-[6](1>={0}1<={1})[0](1>={0}1<={1})", monthStart, calcDate), sumFieldName, needConvert);
                cdoGrnt.AddDetailColumn(String.Format("-[5](1>={0}1<={1})[4](1>={0}1<={1})", monthStart, calcDate), fieldName, needConvert);
                cdoGrnt.columnParamList[counter]["RefTypSum"] = fieldNames[fieldName];
                counter += 2;
            }

            var dt = cdoGrnt.FillData();
            okvValue = GetUSDValue(cdoGrnt);
            return dt;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Программа государственных гарантий Ярославль"
        /// </summary>
        public DataTable[] GetGarantProgrammYarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            dtTables[0] = CreateReportCaptionTable(4);
            dtTables[1] = CreateReportCaptionTable(6);

            var cdo1 = new GarantDataObject();
            cdo1.InitObject(scheme);
            cdo1.mainFilter.Add(f_S_Guarantissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo1.AddDataColumn(f_S_Guarantissued.Purpose);
            cdo1.AddDataColumn(f_S_Guarantissued.Sum);
            cdo1.summaryColumnIndex.Add(1);
            cdo1.removeServiceFields = true;

            for (var i = 0; i < 3; i++)
            {
                cdo1.mainFilter[f_S_Guarantissued.StartDate] = String.Format(">='{0}' and {2} <= '{1}'",
                    GetYearStart(year + i), GetYearEnd(year + i), f_S_Guarantissued.StartDate);
                DataTable dt = CommonGroupDataSet(cdo1.FillData(), 0, true);
                if (i == 0)
                {
                    dtTables[0] = dt;
                }
                else
                {
                    dtTables[0] = CommonMergeDataSet(dtTables[0], dt, 0, true);
                }
            }

            var cdo2 = new GarantDataObject();
            cdo2.InitObject(scheme);
            cdo2.mainFilter.Add(f_S_Guarantissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo2.AddCalcColumn(CalcColumnType.cctOrganization);
            cdo2.AddDataColumn(f_S_Guarantissued.Sum);
            cdo2.summaryColumnIndex.Add(1);
            cdo2.removeServiceFields = true;
            
            for (var i = -1; i < 3; i++)
            {
                cdo2.mainFilter[f_S_Guarantissued.StartDate] = String.Format(">='{0}' and {2} <= '{1}'",
                    GetYearStart(year - 1), GetYearEnd(year + i), f_S_Guarantissued.StartDate);
                var dt = CommonGroupDataSet(cdo2.FillData(), 0, true);
                if (i == -1)
                {
                    dtTables[1] = dt;
                }
                else
                {
                    dtTables[1] = CommonMergeDataSet(dtTables[1], dt, 0, true);
                }
            }

            for (var i = 1; i < 5; i++)
            {
                CorrectThousandSumValue(dtTables[0], i);
                CorrectThousandSumValue(dtTables[1], i);
            }
            
            dtTables[2] = CreateReportCaptionTable(4, 1);
            var drCaption = GetLastRow(dtTables[2]);
            drCaption[0] = year + 0;
            drCaption[1] = year + 1;
            drCaption[2] = year + 2;
            drCaption[3] = year + 3;
            return dtTables;
        }

        /// <summary>
        /// Программа государственных гарантий области Вологда
        /// </summary>
        public DataTable[] GetGarantProgrammVologdaData(Dictionary<string, string> reportParams)
        {
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var shortCalcDateStart = GetYearStart(year);
            var shortCalcDateEnd = GetYearEnd(year);
            var sumPair = CreateValuePair(ReportConsts.SumField);
            var dtTables = new DataTable[3];
            var cdo = new GarantDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter[f_S_Guarantissued.RefVariant] = Combine(
                 ReportConsts.FixedVariantsID, reportParams[ReportConsts.ParamVariantID]);
            cdo.mainFilter[f_S_Guarantissued.DateDoc] = String.Format("<'{0}'", shortCalcDateStart);
            cdo.mainFilter[f_S_Guarantissued.EndDate] = String.Format(">='{0}' or {1} >= '{0}'",
                shortCalcDateStart, f_S_Guarantissued.RenewalDate);
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctPosition);
            cdo.AddDataColumn(f_S_Guarantissued.Purpose);
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            cdo.AddCalcColumn(CalcColumnType.cctRegress);
            cdo.AddDataColumn(f_S_Guarantissued.Note);
            // служебные
            cdo.AddDataColumn(f_S_Guarantissued.Sum);
            cdo.AddDataColumn(f_S_Guarantissued.CurrencySum);
            cdo.AddDetailColumn(String.Format("[3](1<={0}1>={1})", shortCalcDateEnd, shortCalcDateStart), sumPair, true);
            cdo.AddDetailColumn(String.Format("+[3](1<{0})[5](1<{0})", shortCalcDateStart), sumPair, true);
            cdo.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            cdo.AddCalcNamedColumn(CalcColumnType.cctCreditEndDate, TempFieldNames.CreditEndDate);
            // Заполнение данными
            cdo.sortString = StrSortUp(f_S_Guarantissued.StartDate);
            dtTables[0] = cdo.FillData();
            cdo.mainFilter[f_S_Guarantissued.DateDoc] = String.Format(">='{0}' and {2} <= '{1}'",
                shortCalcDateStart, shortCalcDateEnd, f_S_Guarantissued.DateDoc);
            dtTables[1] = cdo.FillData();
            var summary1 = CorrectGarantData(dtTables[0], year);
            var summary2 = CorrectGarantData(dtTables[1], year);
            var drParams = CreateReportParamsRow(dtTables);
            drParams[0] = reportParams[ReportConsts.ParamYear];
            const int colCount = 4;
            
            for (var i = 0; i < 4; i++)
            {
                drParams[i + 1 + colCount * 0] = summary1[i];
                drParams[i + 1 + colCount * 1] = summary2[i];
                drParams[i + 1 + colCount * 2] = summary1[i] + summary2[i];
            }
            
            dtTables[0].Rows.Remove(GetLastRow(dtTables[0]));
            dtTables[1].Rows.Remove(GetLastRow(dtTables[1]));
            return dtTables;
        }

        /// <summary>
        /// Приложение 4. Отчет о предоставленных государственных гарантиях и исполнении обязательств за отчетный период (Приложение 4)
        /// </summary>
        public DataTable[] GetStateDebtApplication4YarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);

            if (reportDate.Day == 1)
            {
                calcDate = reportDate.AddDays(-1).ToShortDateString();
            }

            var loPeriodDate = GetYearStart(calcDate);
            var hiPeriodDate = GetEndQuarter(calcDate);

            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.ActiveVariantID);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            cdoGrnt.AddDetailColumn(String.Format("[2](1<={1}1>={0})", loPeriodDate, hiPeriodDate), "RefTypSum", "1");
            cdoGrnt.AddDetailColumn(String.Format("[2](1<={1}1>={0})", loPeriodDate, hiPeriodDate), "RefTypSum", "2,3,4");
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={1}1>={0})", loPeriodDate, hiPeriodDate));
            cdoGrnt.AddDetailColumn(String.Format("[4](1<={1}1>={0})", loPeriodDate, hiPeriodDate));
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+2;-4;-5;-6;-7");
            dtTables[0] = cdoGrnt.FillData();
            
            foreach (var index in cdoGrnt.summaryColumnIndex)
            {
                CorrectThousandSumValue(dtTables[0], index);
            }

            GetLastRow(dtTables[0])[1] = "Итого";
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = loPeriodDate;
            drCaption[1] = hiPeriodDate;
            drCaption[3] = reportDate.ToShortDateString();
            drCaption[4] = Convert.ToDateTime(hiPeriodDate).Year;
            drCaption[5] = GetQuarterNum(hiPeriodDate);
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Муниц.гарантии за период"
        /// </summary>
        public DataTable[] GetGarantProgrammPeriodData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new GarantDataObject();
            cdo.InitObject(scheme);
            cdo.useSummaryRow = false;
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Guarantissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Guarantissued.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            // Список дат для колонок
            var shortCalcDateStart1 = GetYearStart(year + 1);
            var shortCalcDateEnd1 = GetYearEnd(year + 1);
            var shortCalcDateStart2 = GetYearStart(year + 2);
            var shortCalcDateEnd2 = GetYearEnd(year + 2);
            cdo.mainFilter.Add(f_S_Guarantissued.EndDate, String.Format(">='{0}'", shortCalcDateStart1));
            cdo.mainFilter.Add(f_S_Guarantissued.StartDate, String.Format("<='{0}'", shortCalcDateEnd1));
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctPosition);
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdo.AddDetailColumn(String.Format("[6](1<={0}1>={1})", shortCalcDateEnd1, shortCalcDateStart1));
            cdo.AddDetailColumn(String.Format("[6](1<={0}1>={1})", shortCalcDateEnd2, shortCalcDateStart2));
            cdo.AddCalcColumn(CalcColumnType.cctRegress2);
            cdo.AddCalcColumn(CalcColumnType.cctRegress2);
            cdo.AddDataColumn(f_S_Guarantissued.Purpose);
            cdo.AddDataColumn(f_S_Guarantissued.Note);
            cdo.AddDataColumn(f_S_Guarantissued.Note);
            cdo.AddDataColumn(f_S_Guarantissued.RefOrganizations);
            cdo.sortString = StrSortUp(TempFieldNames.OrgName);
            // Заполнение данными
            var dtTable1 = cdo.FillData();
            CorrectThousandSumValue(dtTable1, 2);
            CorrectThousandSumValue(dtTable1, 3);
            cdo.mainFilter[f_S_Guarantissued.EndDate] = String.Format(">='{0}'", shortCalcDateStart2);
            cdo.mainFilter[f_S_Guarantissued.StartDate] = String.Format("<='{0}'", shortCalcDateEnd2);
            var dtTable2 = cdo.FillData();
            CorrectThousandSumValue(dtTable2, 2);
            CorrectThousandSumValue(dtTable2, 3);
            var dtTable = dtTable1.Clone();
            foreach (DataRow drData in dtTable1.Rows)
            {
                drData[3] = 0;
                dtTable.ImportRow(drData);
            }
            
            foreach (DataRow drData in dtTable2.Rows)
            {
                var drSelect = dtTable.Select(String.Format("{0} = {1}", f_S_Guarantissued.id, drData[f_S_Guarantissued.id]));
                
                if (drSelect.Length == 0)
                {
                    drData[2] = 0;
                    dtTable.ImportRow(drData);
                }
                else
                {
                    drSelect[0][3] = drData[3];
                }
            }

            dtTable = DataTableUtils.SortDataSet(dtTable, cdo.sortString);
            dtTables[0] = dtTable;
            decimal sum1 = 0;
            decimal sum2 = 0;
            var rowNum = 1;

            foreach (DataRow drData in dtTable.Rows)
            {
                drData[0] = String.Format("{0}.", rowNum++);
                sum1 += Convert.ToDecimal(drData[2]);
                sum2 += Convert.ToDecimal(drData[3]);
            }

            var dtGrouped = dtTable.Clone();
            GroupProgrammPeriodOmskData(dtGrouped, dtTable, 9);
            dtTables[0] = dtGrouped;
            dtTables[1] = CreateReportCaptionTable(3);
            var drCaption = dtTables[1].Rows.Add();
            drCaption[0] = reportParams[ReportConsts.ParamYear];
            drCaption[1] = Math.Round(sum1, 2);
            drCaption[2] = Math.Round(sum2, 2);
            return dtTables;
        }

        private decimal[] GetGarantSum(CommonQueryParam procParams)
        {
            var result = new decimal[6];
            var cdoGarant = new GarantDataObject();
            cdoGarant.InitObject(scheme);
            cdoGarant.mainFilter.Add(f_S_Guarantissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, procParams.variantID));
            cdoGarant.mainFilter.Add(f_S_Guarantissued.SourceID, procParams.sourceID);
            cdoGarant.AddDetailColumn(String.Format("[0](1<{0})", procParams.yearStart));
            cdoGarant.AddDetailColumn(String.Format("[7](1<{0})", procParams.yearStart));
            cdoGarant.AddDetailColumn(String.Format("[1](1<{0})", procParams.yearStart));
            cdoGarant.AddDetailColumn(String.Format("[2](1<{0})", procParams.yearStart), "RefTypSum", "1");
            cdoGarant.AddDetailColumn(String.Format("[3](1<{0})", procParams.yearStart));
            var dtGarant = cdoGarant.FillData();
            var gFactAttrPr = GetLastRowValue(dtGarant, 0);
            var gPlanAttrPr = GetLastRowValue(dtGarant, 1);
            var gFactDebtPr = GetLastRowValue(dtGarant, 2);
            var gFactAttr = GetLastRowValue(dtGarant, 3);
            var gPlanDebtPr = GetLastRowValue(dtGarant, 4);
            result[0] = gFactAttrPr;
            result[1] = gPlanAttrPr;
            result[2] = gFactDebtPr;
            result[3] = gFactAttr;
            result[4] = gPlanDebtPr;
            result[5] = ConvertTo1000((2 * gFactAttr - gPlanAttrPr) - (2 * gFactDebtPr + 2 * gFactAttr - gPlanDebtPr));
            return result;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Программа гарантий"
        /// </summary>
        public DataTable[] GetGarantProgrammData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new GarantDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Guarantissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Guarantissued.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var shortCalcDateStart = GetYearStart(year);
            var shortCalcDateEnd = GetYearEnd(year);
            cdo.mainFilter.Add(f_S_Guarantissued.EndDate, String.Format(">='{0}'", shortCalcDateStart));
            cdo.mainFilter.Add(f_S_Guarantissued.StartDate, String.Format("<='{0}'", shortCalcDateEnd));
            // Формирование колонок
            cdo.AddCalcColumn(CalcColumnType.cctPosition);
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdo.AddDataColumn(f_S_Guarantissued.Sum);
            cdo.AddDetailColumn(String.Format("[3](1<={0}1>={1})", shortCalcDateEnd, shortCalcDateStart));
            cdo.AddCalcColumn(CalcColumnType.cctRegress);
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            cdo.AddDataColumn(f_S_Guarantissued.Purpose);
            cdo.AddDataColumn(f_S_Guarantissued.Note);
            cdo.AddDetailColumn(String.Format("[6](1<={0}1>={1})", shortCalcDateEnd, shortCalcDateStart));
            cdo.AddCalcColumn(CalcColumnType.cctRegress2);
            cdo.AddDataColumn(f_S_Guarantissued.Sum);
            // Заполнение данными
            cdo.sortString = StrSortUp(TempFieldNames.OrgName);
            cdo.summaryColumnIndex.Add(2);
            cdo.summaryColumnIndex.Add(10);
            dtTables[0] = cdo.FillData();
            CorrectThousandSumValue(dtTables[0], 2);
            CorrectThousandSumValue(dtTables[0], 3);
            CorrectThousandSumValue(dtTables[0], 8);
            dtTables[1] = CreateReportCaptionTable(5);
            var drCaption = dtTables[1].Rows.Add();
            drCaption[0] = reportParams[ReportConsts.ParamYear];
            drCaption[1] = Math.Round(GetLastRowValue(dtTables[0], 08), 2);
            drCaption[2] = Math.Round(GetLastRowValue(dtTables[0], 10), 2);
            drCaption[3] = Math.Round(GetLastRowValue(dtTables[0], 03), 2);
            drCaption[4] = Math.Round(GetLastRowValue(dtTables[0], 02), 2);
            var regionCode = GetOKTMOCode(cdo.scheme);
            
            if (regionCode == "52 701 000")
            {
                foreach (DataRow dr in dtTables[0].Rows)
                {
                    dr[0] = String.Format("{0}.", dr[0]);
                }
            }
            
            dtTables[0].Rows.RemoveAt(GetLastRowIndex(dtTables[0]));
            return dtTables;
        }
    }
    
    public class GuaranteeReportServer
    {
        // Запросы по гарантиям
        const string guranteeQuery = "select grnt.ID, org.Name, contract.Name, grnt.Num, grnt.StartDate, " +
            "grnt.Sum, grnt.CurrencySum, grnt.RefOKV, grnt.Purpose, org2.Name as Name2  from {0} grnt, {1} org, {2} contract, {1} org2 " +
            "where grnt.RefVariant = {3} and grnt.RefOKV = -1 and org.ID = grnt.RefOrganizationsPlan3 and contract.ID = grnt.RefTypeContract and org2.ID = grnt.RefOrganizations " +
            "order by grnt.RegNum";
        const string currencyGuranteeQuery = "select grnt.ID, org.Name, contract.Name, grnt.Num, grnt.StartDate, " +
            "grnt.Sum, grnt.CurrencySum, grnt.RefOKV, grnt.Purpose, org2.Name as Name2 from {0} grnt, {1} org, {2} contract, {1} org2 " +
            "where grnt.RefVariant = {3} and grnt.RefOKV <> -1 and org.ID = grnt.RefOrganizationsPlan3 and contract.ID = grnt.RefTypeContract and org2.ID = grnt.RefOrganizations " + 
            "order by grnt.RegNum";

        #region запросы по детали
        // План погашения основного долга
        const string planDebtQuery = "select Sum, CurrencySum, StartDate, EndDate from t_S_PlanDebtPrGrnt where RefGrnt = {0}";
        // Факт погашения основного долга
        const string factDebtQuery = "select Sum, CurrencySum, FactDate from t_S_FactDebtPrGrnt where RefGrnt = {0}";
        // План исполнения обязательств гарантом
        private const string planAttractQuery =
            "select Sum, CurrencySum, StartDate, EndDate, RefTypSum from t_S_PlanAttractGrnt where RefGrnt = {0}";
        // факт исполнения обязательств гарантом
        const string factAttractQuery = "select CurrencySum, Sum, FactDate, RefTypSum from t_S_FactAttractGrnt where RefGrnt = {0}";
        // план обслуживания долга
        const string planServiceQuery = "select Sum, CurrencySum, Margin, CurrencyMargin, Commission, CurrencyCommission, StartDate, EndDate from t_S_PlanServicePrGrnt where RefGrnt = {0}";
        // факт обслуживания долга
        const string factServiceQuery = "select Sum, CurrencySum, Margin, CurrencyMargin, Commission, CurrencyCommission, FactDate from t_S_FactPercentPrGrnt where RefGrnt = {0}";

        private const string planAttractPrQuery =
            "select Sum, CurrencySum, StartDate from t_S_PlanAttractPrGrnt where RefGrnt = {0}";

        private const string listContractQuery =
            "select NumberContract, DataContract from t_S_ListContractGrnt where RefGrnt = {0} and BaseDoc = 0";

        #endregion


        private readonly IScheme _scheme;
        public GuaranteeReportServer(IScheme scheme)
        {
            _scheme = scheme;
        }

        public DataTable[] GetGovernmentGuaranteeReportData(DateTime reportDate, int currentVariant)
        {
            var grntEntity = _scheme.RootPackage.FindEntityByName(f_S_Guarantissued.InternalKey);
            var orgEntity = _scheme.RootPackage.FindEntityByName(d_Organizations_Plan.internalKey);
            var contractEntity = _scheme.RootPackage.FindEntityByName(d_S_TypeContract.internalKey);

            var tables = new DataTable[3];

            #region получение данных
            using (var db = _scheme.SchemeDWH.DB)
            {
                // дата начала отчетного месяца
                var startDate = new DateTime(reportDate.Year, reportDate.Month, 1);
                // данные по валютным гарантиям
                var guaranteeData =
                    (DataTable)db.ExecQuery(String.Format(currencyGuranteeQuery, grntEntity.FullDBName, orgEntity.FullDBName, contractEntity.FullDBName, 0), QueryResultTypes.DataTable);
                tables[0] = GetGuaranteeData(guaranteeData, db, startDate, reportDate, "CurrencySum", "CurrencyMargin", "CurrencyCommission");
                // данные по гарантиям в рублях
                guaranteeData =
                    (DataTable)db.ExecQuery(String.Format(guranteeQuery, grntEntity.FullDBName, orgEntity.FullDBName, contractEntity.FullDBName, 0), QueryResultTypes.DataTable);
                tables[1] = GetGuaranteeData(guaranteeData, db, startDate, reportDate, "Sum", "Margin", "Commission");

                tables[2] = new DataTable();
                tables[2].Columns.Add("StartDate", typeof(string));
                tables[2].Columns.Add("ReportDate", typeof(string));
                tables[2].Columns.Add("CurrencyRate", typeof(decimal));
                var newRow = tables[2].NewRow();
                newRow[0] = startDate.ToShortDateString();
                newRow[1] = reportDate.ToShortDateString();
                var dt = (DataTable)
                    db.ExecQuery("select ExchangeRate, DateFixing from d_S_ExchangeRate where DateFixing <= ? and RefOKV = 131", QueryResultTypes.DataTable,
                    new System.Data.OleDb.OleDbParameter("FactDate", reportDate));
                decimal currencyRate = 0;
                
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Select(String.Empty, "DateFixing DESC")[0];
                    currencyRate = Convert.ToDecimal(row["ExchangeRate"]);
                }

                newRow[2] = currencyRate;
                tables[2].Rows.Add(newRow);
            }

            #endregion

            return tables;
        }

        private const string planDataMonthFilter = "EndDate >= '{0}' and EndDate <= '{1}'";
        private const string planDebtMonthFilter = "EndDate >= '{0}' and EndDate <= '{1}' and RefTypSum = 1";
        private const string planPercentMonthFilter = "EndDate >= '{0}' and EndDate <= '{1}' and RefTypSum = 2";
        private const string planPenaltyMonthFilter = "EndDate >= '{0}' and EndDate <= '{1}' and (RefTypSum = 3 or RefTypSum = 4 or RefTypSum = 6)";
        private const string planMarjaMonthFilter = "EndDate >= '{0}' and EndDate <= '{1}' and RefTypSum = 7";
        private const string planCommissionMonthFilter = "EndDate >= '{0}' and EndDate <= '{1}' and RefTypSum = 5";

        private const string planPenaltyFilter = "EndDate <= '{0}' and (RefTypSum = 3 or RefTypSum = 4 or RefTypSum = 6)";
        private const string planMarjaFilter = "EndDate <= '{0}' and RefTypSum = 7";
        private const string planCommissionFilter = "EndDate <= '{0}' and RefTypSum = 5";

        private const string dataMonthFilter = "FactDate >= '{0}' and FactDate <= '{1}'";
        private const string debtMonthFilter = "FactDate >= '{0}' and FactDate <= '{1}' and RefTypSum = 1";
        private const string percentMonthFilter = "FactDate >= '{0}' and FactDate <= '{1}' and RefTypSum = 2";
        private const string penaltyMonthFilter = "FactDate >= '{0}' and FactDate <= '{1}' and (RefTypSum = 3 or RefTypSum = 4 or RefTypSum = 6)";
        private const string marjaMonthFilter = "FactDate >= '{0}' and FactDate <= '{1}' and RefTypSum = 7";
        private const string commissionMonthFilter = "FactDate >= '{0}' and FactDate <= '{1}' and RefTypSum = 5";

        private const string dataFilter = "FactDate <= '{0}'";
        private const string debtFilter = "FactDate <= '{0}' and RefTypSum = 1";
        private const string percentFilter = "FactDate <= '{0}' and RefTypSum = 2";
        private const string penaltyFilter = "FactDate <= '{0}' and (RefTypSum = 3 or RefTypSum = 4 or RefTypSum = 6)";
        private const string marjaFilter = "FactDate <= '{0}' and RefTypSum = 7";
        private const string commissionFilter = "FactDate <= '{0}' and RefTypSum = 5";

        private static string GetGrntListContractText(DataTable dt)
        {
            return dt.Rows.Cast<DataRow>().Aggregate(String.Empty, (current, dr) => 
                String.Format("{0}, доп. соглашение №{1} от {2}", 
                    current, 
                    dr["NumberContract"], 
                    Convert.ToDateTime(dr["DataContract"]).ToShortDateString()));
        }

        private static DataTable GetGuaranteeData(DataTable dtGuarantee, IDatabase db,
            DateTime startDate, DateTime endDate, string sumColumnName, string marginColumnName, string commissionColumnName)
        {
            var dtResult = GetDataTable();
            var i = 1;

            foreach (DataRow row in dtGuarantee.Rows)
            {
                var reportRow = dtResult.NewRow();
                var guaranteeID = row["ID"];

                var dtListContract = (DataTable)db.ExecQuery(String.Format(listContractQuery, guaranteeID), QueryResultTypes.DataTable);
                reportRow[0] = i++;
                reportRow["DocName"] = String.Format("{0} {1} {2}{3}", row["Name1"], row["Num"], 
                    Convert.ToDateTime(row["StartDate"]).ToShortDateString(), GetGrntListContractText(dtListContract));
                reportRow["CreditorName"] = String.Format("{0}, {1}", row["Name"], row["Name2"]);
                reportRow["Purpose"] = row["Purpose"];
                reportRow["Sum"] = Convert.ToInt32(row["RefOKV"]) == -1 ? row["Sum"] : row["CurrencySum"];
                reportRow["Rur"] = Convert.ToInt32(row["RefOKV"]) == -1 ? "Рос.руб." : String.Empty;
                // идем по всем выбранным гарантиям и получаем данные из детали
                var dtPlanDebt = (DataTable)db.ExecQuery(String.Format(planDebtQuery, guaranteeID), QueryResultTypes.DataTable);
                var dtFactDebt = (DataTable)db.ExecQuery(String.Format(factDebtQuery, guaranteeID), QueryResultTypes.DataTable);
                var dtPlanAttract = (DataTable)db.ExecQuery(String.Format(planAttractQuery, guaranteeID), QueryResultTypes.DataTable);
                var dtFactAttract = (DataTable)db.ExecQuery(String.Format(factAttractQuery, guaranteeID), QueryResultTypes.DataTable);
                var dtPlanService = (DataTable)db.ExecQuery(String.Format(planServiceQuery, guaranteeID), QueryResultTypes.DataTable);
                var dtFactService = (DataTable)db.ExecQuery(String.Format(factServiceQuery, guaranteeID), QueryResultTypes.DataTable);
                var dtPlanAtractPr = (DataTable)db.ExecQuery(String.Format(planAttractPrQuery, guaranteeID), QueryResultTypes.DataTable);

                // основной долг. изменения
                decimal mainDebtMonthSum = 0;
                mainDebtMonthSum += GetRowsColumnSum(dtPlanDebt.Select(String.Format(planDataMonthFilter, startDate, endDate)),
                    sumColumnName);
                mainDebtMonthSum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planDebtMonthFilter, startDate, endDate)),
                    sumColumnName);
                mainDebtMonthSum -= GetRowsColumnSum(dtFactDebt.Select(String.Format(dataMonthFilter, startDate, endDate)),
                    sumColumnName);
                mainDebtMonthSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(debtMonthFilter, startDate, endDate)),
                    sumColumnName);

                // проценты. изменения
                decimal percentDebtMonthSum = 0;
                percentDebtMonthSum += GetRowsColumnSum(dtPlanService.Select(String.Format(planDataMonthFilter, startDate, endDate)),
                    sumColumnName);
                percentDebtMonthSum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planPercentMonthFilter, startDate, endDate)),
                    sumColumnName);
                percentDebtMonthSum -= GetRowsColumnSum(dtFactService.Select(String.Format(dataMonthFilter, startDate, endDate)),
                    sumColumnName);
                percentDebtMonthSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(percentMonthFilter, startDate, endDate)),
                    sumColumnName);
                // изменения по марже
                decimal marjaMonthSum = 0;
                marjaMonthSum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planMarjaMonthFilter, startDate, endDate)),
                    sumColumnName);
                marjaMonthSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(marjaMonthFilter, startDate, endDate)),
                    sumColumnName);
                marjaMonthSum += GetRowsColumnSum(dtPlanService.Select(String.Format(planDataMonthFilter, startDate, endDate)),
                    marginColumnName);
                marjaMonthSum -= GetRowsColumnSum(dtFactService.Select(String.Format(dataMonthFilter, startDate, endDate)),
                    marginColumnName);

                // изменения по комиссии
                decimal commissionMonthSum = 0;
                commissionMonthSum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planCommissionMonthFilter, startDate, endDate)),
                    sumColumnName);
                commissionMonthSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(commissionMonthFilter, startDate, endDate)),
                    sumColumnName);
                commissionMonthSum += GetRowsColumnSum(dtPlanService.Select(String.Format(planDataMonthFilter, startDate, endDate)),
                    commissionColumnName);
                commissionMonthSum -= GetRowsColumnSum(dtFactService.Select(String.Format(dataMonthFilter, startDate, endDate)),
                    commissionColumnName);

                // изменения по пени
                decimal penaltyMonthSum = 0;
                penaltyMonthSum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planPenaltyMonthFilter, startDate, endDate)),
                    sumColumnName);
                penaltyMonthSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(penaltyMonthFilter, startDate, endDate)),
                    sumColumnName);

                //Основной долг 
                var mainDebtSum = GetRowsColumnSum(dtPlanAtractPr.Select(String.Format("StartDate <= '{0}'", endDate)),
                    sumColumnName);
                mainDebtSum -= GetRowsColumnSum(dtFactDebt.Select(String.Format(dataFilter, endDate)),
                    sumColumnName);
                mainDebtSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(debtFilter, endDate)),
                    sumColumnName);
                // проценты
                var percentSum = GetRowsColumnSum(dtPlanService.Select(String.Empty), sumColumnName);
                percentSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(percentFilter, endDate)), sumColumnName);
                percentSum -= GetRowsColumnSum(dtFactService.Select(String.Format(dataFilter, endDate)), sumColumnName);
                // маржа
                decimal marjaSum = 0;
                marjaSum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planMarjaFilter, endDate)), sumColumnName);
                marjaSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(marjaFilter, endDate)), sumColumnName);
                marjaSum += GetRowsColumnSum(dtPlanService.Select(), marginColumnName);
                marjaSum -= GetRowsColumnSum(dtFactService.Select(String.Format(dataFilter, endDate)), marginColumnName);
                // комиссия
                decimal commissionSum = 0;
                commissionSum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planCommissionFilter, endDate)), sumColumnName);
                commissionSum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(commissionFilter, endDate)), sumColumnName);
                commissionSum += GetRowsColumnSum(dtPlanService.Select(), commissionColumnName);
                commissionSum -= GetRowsColumnSum(dtFactService.Select(String.Format(dataFilter, endDate)), commissionColumnName);
                // пени
                decimal penaltySum = 0;
                penaltySum += GetRowsColumnSum(dtPlanAttract.Select(String.Format(planPenaltyFilter, endDate)), sumColumnName);
                var queryResult = db.ExecQuery(String.Format("select Sum({0}) from t_S_ChargePenaltyDebtPrGrnt where RefGrnt = {1} and StartDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("StartDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum += Convert.ToDecimal(queryResult);
                queryResult = db.ExecQuery(String.Format("select Sum({0}) from t_S_PrGrntChargePenaltyPercent where RefGrnt = {1} and StartDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("StartDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum += Convert.ToDecimal(queryResult);
                queryResult = db.ExecQuery(String.Format("select Sum({0}) from t_S_FactPenaltyDebtPrGrnt where RefGrnt = {1} and FactDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("FactDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum -= Convert.ToDecimal(queryResult);
                queryResult = db.ExecQuery(String.Format("select Sum({0}) from t_S_FactPenaltyPercentPrGrnt where RefGrnt = {1} and FactDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("FactDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum -= Convert.ToDecimal(queryResult);
                penaltySum -= GetRowsColumnSum(dtFactAttract.Select(String.Format(penaltyFilter, endDate)), sumColumnName);

                var resultSum = mainDebtSum + percentSum + marjaSum + commissionSum + penaltySum;

                reportRow["MonthResult"] = mainDebtMonthSum + percentDebtMonthSum + penaltyMonthSum + marjaMonthSum + commissionMonthSum;
                reportRow["MonthDebt"] = mainDebtMonthSum;
                reportRow["MonthPercent"] = percentDebtMonthSum;
                reportRow["MonthMarja"] = marjaMonthSum;
                reportRow["MonthCommission"] = commissionMonthSum;
                reportRow["MonthPenalty"] = penaltyMonthSum;

                reportRow["Result"] = resultSum;
                reportRow["ResultDebt"] = mainDebtSum;
                reportRow["ResultPercent"] = percentSum;
                reportRow["ResultMarja"] = marjaSum;
                reportRow["ResultCommission"] = commissionSum;
                reportRow["ResultPenalty"] = penaltySum;

                dtResult.Rows.Add(reportRow);
            }

            return dtResult;
        }

        private static decimal GetRowsColumnSum(IEnumerable<DataRow> rows, string dataColumnName)
        {
            return rows.Sum(row => row.IsNull(dataColumnName) ? 0 : Convert.ToDecimal(row[dataColumnName]));
        }

        /// <summary>
        /// таблица со структурой отчета
        /// </summary>
        /// <returns></returns>
        private static DataTable GetDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("DocNum", typeof(int));
            dt.Columns.Add("DocName", typeof(string));
            dt.Columns.Add("CreditorName", typeof(string));
            dt.Columns.Add("Purpose", typeof(string));
            dt.Columns.Add("Sum", typeof(decimal));
            dt.Columns.Add("Rur", typeof(string));
            dt.Columns.Add("MonthResult", typeof(decimal));
            dt.Columns.Add("MonthDebt", typeof(decimal));
            dt.Columns.Add("MonthPercent", typeof(decimal));
            dt.Columns.Add("MonthMarja", typeof(decimal));
            dt.Columns.Add("MonthCommission", typeof(decimal));
            dt.Columns.Add("MonthPenalty", typeof(decimal));
            dt.Columns.Add("Result", typeof(decimal));
            dt.Columns.Add("ResultDebt", typeof(decimal));
            dt.Columns.Add("ResultPercent", typeof(decimal));
            dt.Columns.Add("ResultMarja", typeof(decimal));
            dt.Columns.Add("ResultCommission", typeof(decimal));
            dt.Columns.Add("ResultPenalty", typeof(decimal));
            return dt;
        }
    }
}