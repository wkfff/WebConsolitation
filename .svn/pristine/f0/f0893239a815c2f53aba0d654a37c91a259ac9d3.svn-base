using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIssued;
using Krista.FM.Client.Reports.Planning.Data;
using Krista.FM.Client.Reports.Common;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// Реестр предоставленных бюджетных кредитов
        /// </summary>
        public DataTable[] GetReestrBudCreditSaratovData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var startYearDate = GetYearStart(calcDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            // объект по приходам
            var cdo = new CreditIssuedDataObject();
            InitObjectParamsCI(cdo, calcDate, false);
            cdo.mainFilter[f_S_Creditissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdo.mainFilter[f_S_Creditissued.RefSStatusPlan] = FormNegFilterValue("4");
            cdo.valuesSeparator = ";";
            // 0
            cdo.AddDataColumn(f_S_Creditissued.RefOrganizations);
            // 1
            cdo.AddCalcColumn(CalcColumnType.cctPosition);
            // 2
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 3
            cdo.AddCalcColumn(CalcColumnType.cctNumStartDate);
            // 4
            cdo.AddDetailTextColumn(String.Format("[1](1<={0})", maxDate), cdo.ParamOnlyDates, "1");
            // 5
            cdo.AddDetailTextColumn(String.Format("[2](1<={0})", maxDate), cdo.ParamOnlyDates, "1");
            // 6
            cdo.AddDataColumn(f_S_Creditissued.CreditPercent);
            // 7
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+8;+9;+10;+12;+13;+14");
            // 8
            cdo.AddDetailColumn(String.Format("-[3](1<{0})[4](1<{0})", startYearDate));
            // 9
            cdo.AddDetailColumn(String.Format("-[2](1<{1})[7](1<{0})", startYearDate, maxDate));
            // 10
            cdo.AddDetailColumn(String.Format("--+[6](0<{0})[5](0<{0})[8](1<{0})[9](1<{0})", startYearDate));
            // 11
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+12;+13;+14");
            // 12
            cdo.AddDetailColumn(String.Format("-[1](1<{0})[4](1<{0})", startYearDate));
            // 13
            cdo.AddDetailColumn(String.Format("-[2](1<{0})[7](1<{0})", startYearDate));
            // 14
            cdo.AddDetailColumn(String.Format("--+[6](0<{0})[5](0<{0})[8](1<{0})[9](1<{0})", startYearDate));
            // 15
            cdo.AddDetailColumn(String.Format("[3](1>={0}1<{1})", startYearDate, calcDate));
            // 16
            cdo.AddDetailColumn(String.Format("[4](1>={0}1<{1})", startYearDate, calcDate));
            // 17
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 18
            cdo.AddDetailTextColumn(String.Format("[7](1>={0}1<{1})", startYearDate, calcDate), String.Empty, String.Empty);
            // 19
            cdo.AddDetailColumn(String.Format("+[6](0>={0}1<={1})[5](0>={0}1<={1})", startYearDate, calcDate));
            // 20
            cdo.AddDetailColumn(String.Format("+[8](1>={0}1<={1})[9](1>={0}1<={1})", startYearDate, calcDate));
            // 21
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+22;+23;+24;+26;+27;+28");
            // 22
            cdo.AddDetailColumn(String.Format("-[3](1<{0})[4](1<{0})", calcDate));
            // 23
            cdo.AddDetailColumn(String.Format("-[2](1<{1})[7](1<{0})", calcDate, maxDate));
            // 24
            cdo.AddDetailColumn(String.Format("--+[6](0<{0})[5](0<{0})[8](1<{0})[9](1<{0})", calcDate));
            // 25
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+26;+27;+28");
            // 26
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 27
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 28
            cdo.AddDetailColumn(String.Format("--+[6](0<={0})[5](0<={0})[8](1<={0})[9](1<={0})", calcDate));
            // служебные
            cdo.AddDataColumn(f_S_Creditissued.Purpose); // 29
            cdo.AddDataColumn(f_S_Creditissued.StartDate, ReportConsts.ftDateTime); // 30
            cdo.AddCalcColumn(CalcColumnType.cctUndefined); // 31 - номер цели
            cdo.AddCalcColumn(CalcColumnType.cctUndefined); // 32 - тип строки 0 - заголовок 1 - данные 2 - итог
            cdo.AddDetailColumn(String.Format("[2](1>={0}1<{1})", startYearDate, calcDate)); // 33
            cdo.AddDetailColumn(String.Format("[7](1>={0}1<{1})", startYearDate, calcDate)); // 34
            cdo.AddDetailColumn(String.Format("[2](1<{0})", maxDate)); // 35
            cdo.AddCalcColumn(CalcColumnType.cctNearestPercent); // 36

            cdo.sortString = StrSortUp(f_S_Creditissued.StartDate);
            var dtCredit = cdo.FillData();
           
            cdo.summaryColumnIndex.Add(17);
            cdo.summaryColumnIndex.Add(18);

            var dateYearStart = new DateTime(reportDate.Year, 1, 1);

            for (var i = 0; i < dtCredit.Rows.Count - 1; i++)
            {
                var rowData = dtCredit.Rows[i];

                if (rowData.IsNull(f_S_Creditissued.CreditPercent) || 
                    GetDecimal(rowData[f_S_Creditissued.CreditPercent]) == 0)
                {
                    rowData[f_S_Creditissued.CreditPercent] = rowData[36];
                }

                if (rowData.IsNull(f_S_Creditissued.StartDate))
                {
                    continue;
                }

                var startDate = Convert.ToDateTime(rowData[f_S_Creditissued.StartDate]);
                
                if (DateTime.Compare(startDate, dateYearStart) > 0)
                {
                    rowData[9] = 0;
                }

                if (reportDate.Year == startDate.Year)
                {
                    rowData[17] = rowData[35];
                }
            }

            dtCredit = cdo.RecalcSummary(dtCredit);

            // список различных целей
            IEnumerable<string> purposes = GetColumnValues(dtCredit, f_S_Creditissued.Purpose);
            // группируем по цели
            var dtResult = dtCredit.Clone();
            var dtPurpose = CreateReportCaptionTable(cdo.columnList.Count);
            var purposeNum = 1;
            const int stateColumn = 32;
            const int purposeColumn = 31;
            var resultSumArray = new decimal[cdo.columnList.Count];

            foreach (var purpose in purposes)
            {
                var drPurpose = dtPurpose.Rows.Add();
                drPurpose[0] = purpose;
                var drsSelect = dtCredit.Select(String.Format("{0} = '{1}'", f_S_Creditissued.Purpose, purpose), StrSortUp(TempFieldNames.OrgName));
                var orgs = GetColumnValues(drsSelect, f_S_Creditissued.RefOrganizations);
                var totalSumArray = new decimal[cdo.columnList.Count];

                foreach (var orgId in orgs)
                {
                    // выбираем строки по заемщику текущей цели
                    var drsOrgs = dtCredit.Select(String.Format("{0} = '{2}' and {1} = '{3}'",
                        f_S_Creditissued.RefOrganizations, f_S_Creditissued.Purpose, orgId, purpose));
                    // заголовок по организации
                    var drOrgCaption = dtResult.Rows.Add();
                    drOrgCaption[02] = drsOrgs[0][2];
                    drOrgCaption[stateColumn] = 0;
                    drOrgCaption[purposeColumn] = purposeNum;
                    // выводим строчки по организации
                    var sumArray = new decimal[cdo.columnList.Count];

                    foreach (var drOrg in drsOrgs)
                    {
                        foreach (var colIndex in cdo.summaryColumnIndex)
                        {
                            sumArray[colIndex] += GetNumber(drOrg[colIndex]); 
                        }

                        sumArray[18] += GetNumber(drOrg[34]);

                        dtResult.ImportRow(drOrg);
                        GetLastRow(dtResult)[stateColumn] = 1;
                        GetLastRow(dtResult)[purposeColumn] = purposeNum;
                    }

                    // итоговая строчка по организации
                    var drOrgResult = dtResult.Rows.Add();

                    foreach (var colIndex in cdo.summaryColumnIndex)
                    {
                        drOrgResult[colIndex] = sumArray[colIndex];
                        totalSumArray[colIndex] += sumArray[colIndex];
                    }

                    drOrgResult[2] = String.Format("Итого по {0}", drOrgCaption[02]);
                    drOrgResult[stateColumn] = 2;
                    drOrgResult[purposeColumn] = purposeNum;
                }

                foreach (var colIndex in cdo.summaryColumnIndex)
                {
                    drPurpose[colIndex] = totalSumArray[colIndex];
                    resultSumArray[colIndex] += totalSumArray[colIndex];
                }

                drPurpose[1] = purposeNum++;
            }

            dtTables[0] = dtResult;
            dtTables[1] = dtPurpose;
            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = GetDateText(startYearDate);
            drCaption[2] = GetDateText(calcDate);
            drCaption[3] = startYearDate;

            foreach (var colIndex in cdo.summaryColumnIndex)
            {
                if (colIndex > 3)
                {
                    drCaption[colIndex] = resultSumArray[colIndex];
                }
            }

            return dtTables;
        }

        /// <summary>
        /// Расчет процентов за пользование заемными средствами
        /// </summary>
        public DataTable[] GetAnalisysFinSupportVologdaData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var nextMonth = reportDate.AddMonths(1);
            var year = Convert.ToDateTime(calcDate).Year;
            var endYearDate = GetYearEnd(calcDate);
            var startYearDate = GetYearStart(calcDate);
            var startNextMonthDate = GetMonthStart(nextMonth);
            var endNextMonthDate = GetMonthEnd(nextMonth);
            // объект по приходам
            var cdo1 = new CreditIssuedDataObject();
            cdo1.InitObject(scheme);
            cdo1.mainFilter.Add(f_S_Creditissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            // 00
            cdo1.AddDataColumn(f_S_Creditissued.Sum);
            // 01
            cdo1.AddDetailColumn(String.Format("[0](0<{0})", calcDate));
            // 02
            cdo1.AddDetailColumn(String.Format("[0](0<{0})", startYearDate));
            // 03
            cdo1.AddDetailColumn(String.Format("[0](0<{0})", startYearDate));
            // 04
            cdo1.AddDetailColumn(String.Format("[0](0>={0}0<{1})", startYearDate, calcDate));
            // 05
            cdo1.AddDetailColumn(String.Format("[0](0>={0}0<={1})", calcDate, endYearDate));
            // 06
            cdo1.AddCalcColumn(CalcColumnType.cctUndefined);
            // 07
            cdo1.AddDetailColumn(String.Format("[0](0>={0}0<={1})", calcDate, endYearDate));
            // 08
            cdo1.AddDetailColumn(String.Format("[0](0>={0}0<={1})", calcDate, endYearDate));
            // 09
            cdo1.AddDetailColumn(String.Format("[0](0>={0}0<={1})", startNextMonthDate, endNextMonthDate));
            // 10
            cdo1.AddCalcColumn(CalcColumnType.cctUndefined);
            // 11
            cdo1.AddCalcColumn(CalcColumnType.cctUndefined);
            // фильтровочные
            cdo1.AddCalcNamedColumn(CalcColumnType.cctCreditYear, TempFieldNames.CreditStartYear);
            cdo1.AddDataColumn(f_S_Creditissued.RefVariant);
            cdo1.AddDataColumn(f_S_Creditissued.RefSTypeCredit);
            // объект по уходам
            var cdo2 = new CreditIssuedDataObject();
            cdo2.InitObject(scheme);
            cdo2.mainFilter.Add(f_S_Creditissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            // 00
            cdo2.AddDataColumn(f_S_Creditissued.Sum);
            // 01
            cdo2.AddCalcColumn(CalcColumnType.cctUndefined);
            // 02
            cdo2.AddDetailColumn(String.Format("[4](1<{0})", startYearDate));
            // 03
            cdo2.AddDetailColumn(String.Format("[4](0<{0})", calcDate));
            // 04
            cdo2.AddDetailColumn(String.Format("[4](0>={0}0<{1})", startYearDate, calcDate));
            // 05
            cdo2.AddDetailColumn(String.Format("[1](0>={0}0<={1})", calcDate, endYearDate));
            // 06
            cdo2.AddDetailColumn(String.Format("[1](0>={0}0<={1})", calcDate, endYearDate));
            // 07
            cdo2.AddDetailColumn(String.Format("[1](0>={0}0<={1})", calcDate, endYearDate));
            // 08
            cdo2.AddDetailColumn(String.Format("[1](0>={0}0<={1})", calcDate, endYearDate));
            // 09
            cdo2.AddDetailColumn(String.Format("[0](0>={0}0<={1})", startNextMonthDate, endNextMonthDate));
            // 10
            cdo1.AddCalcColumn(CalcColumnType.cctUndefined);
            // 11
            cdo1.AddCalcColumn(CalcColumnType.cctUndefined);
            // фильтровочные
            cdo2.AddCalcNamedColumn(CalcColumnType.cctCreditYear, TempFieldNames.CreditStartYear);
            cdo1.AddDataColumn(f_S_Creditissued.RefVariant);
            cdo1.AddDataColumn(f_S_Creditissued.RefSTypeCredit);
            // заполняим строчечки
            var dtDullData = new DataTable[5];
            SetColumnConditions(cdo1, reportDate.Year, 0);
            dtDullData[0] = cdo1.FillData();
            SetColumnConditions(cdo1, reportDate.Year, 1);
            dtDullData[1] = cdo1.FillData();
            SetColumnConditions(cdo2, reportDate.Year, 2);
            dtDullData[2] = cdo1.FillData();
            SetColumnConditions(cdo2, reportDate.Year, 3);
            dtDullData[3] = cdo1.FillData();
            SetColumnConditions(cdo2, reportDate.Year, 4);
            dtDullData[4] = cdo1.FillData();
            // пихнем их в общую таблицу итогов
            dtTables[0] = CreateReportCaptionTable(12, 5);
            
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 12; j++)
                {
                    dtTables[0].Rows[i][j] = GetLastRowValue(dtDullData[i], j);
                }
            }

            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = year;
            drCaption[2] = GetMonthText1(nextMonth.Month);
            return dtTables;
        }

        /// <summary>
        /// Справка-расчет начисленных процентов и пеней по бюджетным кредитам
        /// </summary>
        public DataTable[] GetCertificateCalcPercentPennyVologdaData(Dictionary<string, string> reportParams,
            bool checkRegionCode)
        {
            var dtTables = new DataTable[2];
            var cdo = new CreditIssuedDataObject();
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var year = InitObjectParamsCI(cdo, calcDate, false);
            cdo.mainFilter.Remove(f_S_Creditissued.StartDate);
            cdo.mainFilter.Remove(f_S_Creditissued.EndDate);
            cdo.mainFilter[f_S_Creditissued.RefRegions] = FormNegFilterValue(ReportConsts.UndefinedRegion);
            cdo.mainFilter[f_S_Creditissued.RefVariant] = ReportConsts.ActiveVariantID;
            // 04
            cdo.AddCalcNamedColumn(CalcColumnType.cctRegion, TempFieldNames.RegionName);
            // 05
            cdo.AddCalcColumn(CalcColumnType.cctPurposeActNumDate);
            cdo.AddDataColumn(f_S_Creditissued.EndDate);
            cdo.AddCalcColumn(CalcColumnType.cctCreditIssNumDocDate);
            cdo.AddDataColumn(f_S_Creditissued.StartDate);
            cdo.AddDataColumn(f_S_Creditissued.Sum);
            cdo.AddDataColumn(f_S_Creditissued.PercentRate);
            cdo.AddDataColumn(f_S_Creditissued.PenaltyPercentRate);
            cdo.AddDataColumn(f_S_Creditissued.PenaltyDebtRate);
            cdo.AddDetailColumn("[4][2][7][6][9][5][8]");
            cdo.AddDataColumn(f_S_Creditissued.Num);
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            cdo.AddDataColumn(f_S_Creditissued.RefRegions);
            cdo.useSummaryRow = false;
            cdo.sortString = StrSortUp(TempFieldNames.RegionName);
            var dtResult = cdo.FillData();
            var contractPos = 1;
            DateTime startDate, endDate, startPeriodDate, endPeriodDate;
            // Индексы колонок с итогами
            var summaryIndex = new Collection<int> {05, 07, 12, 14, 19, 21, 26, 28};
            // Теперь поднапряжемся и заполним детализации
            dtTables[0] = CreateReportCaptionTable(30);
            var monthNames = GetMonthRusNames();
            cdo.dtDetail[7] = cdo.SortDataSet(cdo.dtDetail[7], StrSortUp(t_S_FactPercentCO.FactDate));
            cdo.dtDetail[6] = cdo.SortDataSet(cdo.dtDetail[6], StrSortUp(t_S_COChargePenaltyPercent.EndPenaltyDate));
            cdo.dtDetail[9] = cdo.SortDataSet(cdo.dtDetail[9], StrSortUp(t_S_FactPenaltyPercentCO.FactDate));
            cdo.dtDetail[5] = cdo.SortDataSet(cdo.dtDetail[5], StrSortUp(t_S_ChargePenaltyDebtCO.StartDate));
            cdo.dtDetail[8] = cdo.SortDataSet(cdo.dtDetail[8], StrSortUp(t_S_FactPenaltyDebtCO.FactDate));
            var regionID = GetUserRegionCode(cdo.scheme);
            string[] subjectInfo;
            var reportRowCount = 0;

            foreach (DataRow drContract in dtResult.Rows)
            {
                var parentCode = -1;

                if (checkRegionCode)
                {
                    if (drContract[f_S_Creditissued.RefRegions] != DBNull.Value)
                    {
                        subjectInfo = GetParentTerritoryData(cdo.scheme,
                            Convert.ToInt32(drContract[f_S_Creditissued.RefRegions])).Split('=');
                        if (subjectInfo.Length > 0 && subjectInfo[2].Length > 0) 
                            parentCode = Convert.ToInt32(subjectInfo[2]);
                    }
                }

                if (parentCode == -1 || parentCode == regionID)
                {
                    reportRowCount++;
                    var summaryYear = new decimal[dtTables[0].Columns.Count];
                    var summaryFull = new decimal[summaryYear.Length];
                    // Выбираем месяцы года кредита, по которым нужно детализировать
                    startDate = Convert.ToDateTime(drContract[f_S_Creditissued.StartDate]);
                    endDate = Convert.ToDateTime(drContract[f_S_Creditissued.EndDate]);
                    startPeriodDate = startDate;
                    endPeriodDate = startDate;
                    // Для заголовков листа
                    var drRegionCaption = dtTables[0].Rows.Add();
                    // Заголовок.Район
                    drRegionCaption[0] = drContract[0];
                    // Заголовок.Валюта
                    drRegionCaption[1] = drContract[11];
                    // Заголовок.Район(Номер)
                    var regionCaption = String.Format("{0}({1})", drRegionCaption[0], drContract[f_S_Creditissued.id]);
                    regionCaption = SafeSubstrReplace(regionCaption, " район", String.Empty);
                    regionCaption = SafeSubstrReplace(regionCaption, " муниципальный", String.Empty);
                    regionCaption = SafeSubstrReplace(regionCaption, " поселение", String.Empty);
                    regionCaption = SafeSubstrReplace(regionCaption, " городское", String.Empty);
                    regionCaption = SafeSubstrReplace(regionCaption, " сельское", String.Empty);
                    regionCaption = SafeSubstrReplace(regionCaption, " округ", String.Empty);
                    regionCaption = SafeSubstrReplace(regionCaption, " городской", String.Empty);
                    drRegionCaption[2] = regionCaption;
                    SetContractOrderNum(drRegionCaption, contractPos);
                    var masterID = Convert.ToInt32(drContract[f_S_Creditissued.id]);
                    var manyYears = false;
                    var startPosition = dtTables[0].Rows.Count;
                    var detailList = new Dictionary<int, string>();
                    var rowList = new Collection<DataRow>[5];
                    var usedKeys = new Collection<int>[5];
                    var rowCollection = new Collection<DataRow>();
                    detailList.Add(7, t_S_FactPercentCO.FactDate);
                    detailList.Add(6, t_S_COChargePenaltyPercent.EndPenaltyDate);
                    detailList.Add(9, t_S_FactPenaltyPercentCO.FactDate);
                    detailList.Add(5, t_S_ChargePenaltyDebtCO.StartDate);
                    detailList.Add(8, t_S_FactPenaltyDebtCO.FactDate);
                    var counter = 0;

                    foreach (var key in detailList.Keys)
                    {
                        cdo.GetSumValue(cdo.dtDetail[key], masterID, detailList[key], ReportConsts.SumField,
                                startDate, endDate, true, true);
                        rowList[counter] = new Collection<DataRow>();
                        usedKeys[counter] = new Collection<int>();
                        
                        foreach (var drDetail in cdo.sumIncludedRows)
                        {
                            rowList[counter].Add(drDetail);
                        }
                        
                        counter++;
                    }

                    while (DateTime.Compare(endPeriodDate, endDate) < 0)
                    {
                        // Определяем период - 
                        endPeriodDate = startPeriodDate.AddDays(
                            DateTime.DaysInMonth(startPeriodDate.Year, startPeriodDate.Month) - startPeriodDate.Day);
                        
                        if (DateTime.Compare(endPeriodDate, endDate) > 0)
                        {
                            endPeriodDate = endDate;
                        }

                        // заполняем типо строку детализации
                        var isFirstRow = startPosition == dtTables[0].Rows.Count;
                        var drData = dtTables[0].Rows.Add();
                        SetContractOrderNum(drData, contractPos);
                        drData[00] = monthNames[startPeriodDate.Month - 1]; // Месяц 
                        
                        if (isFirstRow)
                        {
                            drData[01] = drContract[1]; // Цель 
                            drData[02] = drContract[2]; // Срок погашения 
                            drData[03] = drContract[3]; // Дата договора 
                            // ОД.Дата выдачи 
                            drData[04] = drContract[4];
                            // ОД.Сумма
                            drData[05] = drContract[5];
                            // Проценты.Ставка
                            drData[08] = "1/4";//drContract[6];
                            // Пени.Ставка
                            drData[15] = drContract[7];
                            // Пени ОД.Ставка
                            drData[22] = drContract[8];
                        }

                        // ОД.Факт погашения
                        drData[07] = cdo.GetSumValue(cdo.dtDetail[4], masterID, t_S_FactDebtCO.FactDate, 
                            ReportConsts.SumField, startPeriodDate, endPeriodDate, true, true);
                        // ОД.Дата погашения
                        drData[06] = GetCalcDetailDateList(cdo.sumIncludedRows, t_S_FactDebtCO.FactDate);
                        // Проценты.Период
                        drData[09] = CreatePeriodStr(startPeriodDate.ToShortDateString(), endPeriodDate.ToShortDateString());
                        // Проценты.Сумма начислений
                        drData[12] = cdo.GetSumValue(cdo.dtDetail[2], masterID, t_S_PlanServiceCO.EndDate, 
                            ReportConsts.SumField, startPeriodDate, endPeriodDate, true, true);
                        // Проценты.Кол-во дней
                        drData[10] = GetCalcDetailDateList(cdo.sumIncludedRows, t_S_PlanServiceCO.DayCount, false);
                        // Проценты.Дата начисления
                        drData[11] = GetCalcDetailDateList(cdo.sumIncludedRows, t_S_PlanServiceCO.EndDate);
                        // Проценты.Сумма погашений
                        drData[14] = CalcDetailSum(detailList, rowList, usedKeys, 0,
                            startPeriodDate, drData[12], ref rowCollection, false);
                        // Проценты.Дата погашения
                        drData[13] = GetCalcDetailDateList(rowCollection, t_S_FactPercentCO.FactDate);
                        // Пени.Сумма начислений
                        drData[19] = CalcDetailSum(detailList, rowList, usedKeys, 1,
                            startPeriodDate, drData[13], ref rowCollection, true);
                        // Пени.Период
                        drData[16] = CreatePeriodStr(
                            GetCalcDetailDateList(rowCollection, t_S_COChargePenaltyPercent.StartPenaltyDate),
                            GetCalcDetailDateList(rowCollection, t_S_COChargePenaltyPercent.EndPenaltyDate));
                        // Пени.Кол-во дней
                        drData[17] = GetCalcDetailDateList(rowCollection, t_S_COChargePenaltyPercent.LateDate, false);
                        // Пени.Дата погашения
                        drData[18] = GetCalcDetailDateList(rowCollection, t_S_COChargePenaltyPercent.StartDate);
                        // Пени.Сумма погашений
                        drData[21] = CalcDetailSum(detailList, rowList, usedKeys, 2,
                            startPeriodDate, drData[19], ref rowCollection, false);
                        // Пени.Дата погашения
                        drData[20] = GetCalcDetailDateList(rowCollection, t_S_FactPenaltyPercentCO.FactDate);
                        // ! Пени.Сумма начислений
                        drData[26] = CalcDetailSum(detailList, rowList, usedKeys, 3,
                            startPeriodDate, drData[06], ref rowCollection, true);
                        // ! Пени.Период
                        drData[23] = CreatePeriodStr(
                            GetCalcDetailDateList(rowCollection, t_S_ChargePenaltyDebtCO.StartPenaltyDate),
                            GetCalcDetailDateList(rowCollection, t_S_ChargePenaltyDebtCO.EndPenaltyDate));
                        // ! Пени.Кол-во дней
                        drData[24] = GetCalcDetailDateList(rowCollection, t_S_ChargePenaltyDebtCO.LateDate, false);
                        // ! Пени.Дата погашения
                        drData[25] = GetCalcDetailDateList(rowCollection, t_S_ChargePenaltyDebtCO.EndPenaltyDate);
                        // ! Пени.Сумма погашений
                        drData[28] = CalcDetailSum(detailList, rowList, usedKeys, 4,
                            startPeriodDate, drData[25], ref rowCollection, true);
                        // ! Пени.Дата погашения
                        drData[27] = GetCalcDetailDateList(rowCollection, t_S_FactPenaltyDebtCO.FactDate);
                        // Итоги
                        summaryYear = CalcSummary(drData, summaryYear, summaryIndex);
                        summaryFull = CalcSummary(drData, summaryFull, summaryIndex);
                        // Некст монс
                        startPeriodDate = Convert.ToDateTime(GetMonthStart(startPeriodDate)).AddMonths(1);
                        // год сменился, надо строку заголовок впихнуть
                        if (startPeriodDate.Year != endPeriodDate.Year && startDate.Year != endDate.Year
                            && DateTime.Compare(startPeriodDate, endDate) < 0)
                        {
                            // Если первый из многолетнего вывода, то заголовок первому году
                            if (!manyYears)
                            {
                                var drFirstYearCaption = dtTables[0].NewRow();
                                drFirstYearCaption[0] = endPeriodDate.Year;
                                dtTables[0].Rows.InsertAt(drFirstYearCaption, startPosition);
                            }

                            manyYears = true;
                            summaryYear = WriteSummary(dtTables[0], summaryYear, summaryIndex,
                                contractPos, String.Format("Итого по {0}г.", endPeriodDate.Year));
                            var drYearCaption = dtTables[0].Rows.Add();
                            drYearCaption[0] = startPeriodDate.Year;
                            SetContractOrderNum(drYearCaption, contractPos);
                        }
                    }
                    
                    // Если лет несколько, то итог перед основным еще и по году
                    if (manyYears)
                    {
                        WriteSummary(dtTables[0], summaryYear, summaryIndex, contractPos,
                            String.Format("Итого по {0}г.", endDate.Year));
                    }

                    WriteSummary(dtTables[0], summaryFull, summaryIndex, contractPos, "Итого");
                    contractPos++;
                }
            }

            // Заголовочная таблица
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = year;
            drCaption[1] = reportRowCount;
            // Приехали
            return dtTables;
        }

        /// <summary>
        /// Отчет по предоставлению бюджетных кредитов Саратов
        /// </summary>
        public DataTable[] GetCreditIssuedSaratovData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var cdo = new CreditIssuedDataObject();
            InitObjectCI(cdo, calcDate);
            var dateYearStart = GetYearStart(calcDate);
            cdo.mainFilter.Remove(f_S_Creditissued.RefSTypeCredit);
            cdo.mainFilter[f_S_Creditissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdo.mainFilter[f_S_Creditissued.RefSStatusPlan] = FormNegFilterValue("4");
            // 1
            cdo.AddCalcColumn(CalcColumnType.cctCreditIssNumDocDate);
            // 2
            cdo.AddDataColumn(f_S_Creditissued.EndDate);
            // 3
            cdo.AddDataColumn(f_S_Creditissued.CreditPercent);
            // 4
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+8;+9;+10");
            // 5
            cdo.AddDetailColumn(String.Format("-[3](1<{0})[4](1<{0})", dateYearStart));
            // 6
            cdo.AddDetailColumn(String.Format("-[2](1<{1})[7](1<{0})", dateYearStart, maxDate));
            // 7
            cdo.AddDetailColumn(String.Format("-+-[5](0<{0})[8](1<{0})[6](0<{0})[9](1<{0})", dateYearStart));
            // 8
            cdo.AddDetailColumn(String.Format("if[3](1<{2})<=[3](1>{2})?[0](1>={0}1<={1}),[3](1>={0}1<={1})", dateYearStart, calcDate, maxDate));
            // 9
            cdo.AddDetailColumn(String.Format("[4](1>={0}1<={1})", dateYearStart, calcDate));
            // 10
            cdo.AddDetailColumn(String.Format("[2](1>={0}1<={1})", dateYearStart, calcDate));
            // 11
            cdo.AddDetailColumn(String.Format("[7](1>={0}1<={1})", dateYearStart, calcDate));
            // 12
            cdo.AddDetailColumn(String.Format("+[5](0>={0}0<={1})[6](0>={0}0<={1})", dateYearStart, calcDate));
            // 13
            cdo.AddDetailColumn(String.Format("+[9](1>={0}1<={1})[8](1>={0}1<={1})", dateYearStart, calcDate));
            // 14
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+18;+19;+20");
            // 15
            cdo.AddDetailColumn(String.Format("-[3](1<{0})[4](1<{0})", calcDate));
            // 16
            cdo.AddDetailColumn(String.Format("-[2](1<{1})[7](1<{0})", calcDate, maxDate));
            // 17
            cdo.AddDetailColumn(String.Format("-+-[5](0<{0})[8](1<{0})[6](0<{0})[9](1<{0})", calcDate));
            // 18
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+22;+23;+26;+27");
            // 19
            cdo.AddDetailColumn(String.Format("if[4](1<{0})>[1](1<{0})?[4](1>{1}),-[1](1<{0})[4](1<{0})", calcDate, maxDate));
            // 20
            cdo.AddDetailColumn(String.Format("if[7](1<{0})>[2](1<{0})?[7](1>{1}),-[2](1<{0})[7](1<{0})", calcDate, maxDate));
            // 21
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+26;+27");
            // 22
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 23
            cdo.AddDetailColumn(String.Format("if[8](1<{0})>[5](0<{0})?[8](1>{1}),-[5](0<{0})[8](1<{0})", calcDate, maxDate));
            // 24
            cdo.AddDetailColumn(String.Format("if[9](1<{0})>[6](0<{0})?[9](1>{1}),-[6](0<{0})[9](1<{0})", calcDate, maxDate));
            // служебные
            // 25
            cdo.AddParamColumn(CalcColumnType.cctRecordCount, "3");
            // 26
            cdo.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 27
            cdo.AddDataColumn(f_S_Creditissued.StartDate);

            var reportDate = Convert.ToDateTime(calcDate);
            var dtResult = cdo.FillData();

            var dateReportStart = new DateTime(reportDate.Year, 1, 1);

            for (var i = 0; i < dtResult.Rows.Count; i++)
            {
                var rowData = dtResult.Rows[i];

                if (GetDecimal(rowData[f_S_Creditissued.CreditPercent]) == 0)
                {
                    rowData[f_S_Creditissued.CreditPercent] = rowData[29];
                }

                if (rowData.IsNull(f_S_Creditissued.StartDate))
                {
                    continue;
                }

                var startDate = Convert.ToDateTime(rowData[f_S_Creditissued.StartDate]);
                var isCurrentYearContract = DateTime.Compare(startDate, dateReportStart) >= 0 &&
                                            DateTime.Compare(startDate, reportDate) <= 0;

                if (isCurrentYearContract)
                {
                    rowData[9] = 0;
                }
                else
                {
                    rowData[13] = 0;
                }
            }

            dtTables[0] = CreateReportCaptionTable(30);
            
            var columnParams = new TerritoryColumnParam
                                   {
                                       columnKind = 5,
                                       dtResult = dtTables[0],
                                       dtContracts = dtResult,
                                       copyEachMonthRecords = false,
                                       monthIndex = 1,
                                       sumArray = new decimal[26],
                                       colIndex1 = 1,
                                       colIndex2 = 3,
                                       writeSummary = true
                                   };

            FillCommonTerritoryData(columnParams);
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = dateYearStart;
            return dtTables;
        }

        /// <summary>
        /// Информация о предоставлении бюджетных кредитов местным бюджетам и их погашении
        /// </summary>
        public DataTable[] GetMainDebtBudgetCreditData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new CreditIssuedDataObject();
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var year = InitObjectCI(cdo, calcDate);
            cdo.mainFilter.Remove(f_S_Creditissued.RefSTypeCredit);
            cdo.mainFilter[f_S_Creditissued.RefRegions] = FormNegFilterValue(ReportConsts.UndefinedRegion);
            // служебные для условий
            cdo.AddCalcNamedColumn(CalcColumnType.cctCreditYear, TempFieldNames.CreditStartYear);
            cdo.AddDataColumn(f_S_Creditissued.RefSTypeCredit);
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+7;+8"); // 04 - Выдано кредитов
            cdo.AddDetailColumn(String.Format("[3](1<{0})", calcDate)); // 05 - в т.ч. районам и городам
            cdo.AddDetailColumn(String.Format("[3](1<{0})", calcDate)); // 06 - в т.ч. поселениям
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+10;+14"); // 07 - Погашено кредитов
            cdo.AddDetailColumn(String.Format("[4](1<{0})", calcDate)); // 08 - в т.ч. районам и городам
            cdo.AddDetailColumn(String.Format("[4](1<{0})", calcDate)); // 09 - Из них прошлых лет
            cdo.AddDetailColumn(String.Format("[4](1<{0})", calcDate)); // 10 - Из них ЦК
            cdo.AddDetailColumn(String.Format("[4](1<{0})", calcDate)); // 11 - Из них текущего года
            cdo.AddDetailColumn(String.Format("[4](1<{0})", calcDate)); // 12 - В т.ч. поселения
            cdo.AddDetailColumn(String.Format("[4](1<={0})", calcDate)); // 13 - Из них прошлых лет
            cdo.AddDetailColumn(String.Format("[4](1<={0})", calcDate)); // 14 - Из них текущего года
            
            // кредиты от организаций, нужны только на колонке ЦК
            for (int i = 6; i < 17; i++)
            {
                var creditType = ReportConsts.CreditIssuedBudCode;
                
                if (i == 12)
                {
                    creditType = ReportConsts.CreditIssuedOrgCode;
                }

                cdo.columnCondition.Add(i, FormFilterValue(f_S_Creditissued.RefSTypeCredit, creditType));
            }
            
            // условия расчета сумм в колонках
            var filterYearPos = FormFilterValue(TempFieldNames.CreditStartYear, year.ToString());
            var filterYearNeg = FormNegativeFilterValue(TempFieldNames.CreditStartYear, year.ToString());
            var filterSettle = FormFilterValue(f_S_Creditissued.RefRegions, 
                GetTerritoryID(cdo.scheme, ReportTerritoryType.ttSettlement));
            var filterRegion = FormFilterValue(f_S_Creditissued.RefRegions, 
                GetTerritoryID(cdo.scheme, ReportTerritoryType.ttRegion));            
            cdo.columnCondition[07] = CombineList(cdo.columnCondition[07], filterRegion);
            cdo.columnCondition[08] = CombineList(cdo.columnCondition[08], filterSettle);
            cdo.columnCondition[10] = CombineList(cdo.columnCondition[10], filterRegion);
            cdo.columnCondition[12] = CombineList(cdo.columnCondition[12], filterRegion);
            cdo.columnCondition[14] = CombineList(cdo.columnCondition[14], filterSettle);
            cdo.columnCondition[11] = CombineList(cdo.columnCondition[11], filterYearNeg);
            cdo.columnCondition[13] = CombineList(cdo.columnCondition[13], filterYearPos);
            cdo.columnCondition[15] = CombineList(cdo.columnCondition[15], filterSettle, filterYearNeg);
            cdo.columnCondition[16] = CombineList(cdo.columnCondition[16], filterSettle, filterYearPos);
            cdo.useSummaryRow = false;
            var dtResult = cdo.FillData();
            dtResult.Columns.RemoveAt(4);
            dtResult.Columns.RemoveAt(4);
            dtResult = ConvertRegionNames(dtResult);
            // группировка и подсчет итогов
            dtTables[0] = CreateReportCaptionTable(12);
            var columnParams = new TerritoryColumnParam
                                   {
                                       columnKind = 4,
                                       dtResult = dtTables[0],
                                       dtContracts = dtResult,
                                       copyEachMonthRecords = false,
                                       monthIndex = 1,
                                       sumArray = new decimal[11],
                                       colIndex1 = 0,
                                       colIndex2 = 2,
                                       writeSummary = true
                                   };

            FillCommonTerritoryData(columnParams);
            // заголовочные параметры
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = Convert.ToDateTime(calcDate).Year - 1;
            drCaption[2] = Convert.ToDateTime(calcDate).Year;
            return dtTables;
        }

        /// <summary>
        /// Основная сумма задолженности с зачетом на отчетная дата
        /// </summary>
        public DataTable[] GetMainDebtWithTestData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var cdo = new CreditIssuedDataObject();
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            InitObjectCI(cdo, calcDate);
            cdo.mainFilter.Remove(f_S_Creditissued.StartDate);
            cdo.mainFilter.Remove(f_S_Creditissued.EndDate);
            cdo.mainFilter[f_S_Creditissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdo.mainFilter[f_S_Creditissued.RefRegions] = FormNegFilterValue(ReportConsts.UndefinedRegion);
            var prevMonth = GetMonthStart(reportDate);
            cdo.AddDetailColumn(String.Format("-[3](1<{0})[4](1<{0})", prevMonth));
            cdo.AddDetailColumn(String.Format("[4](1>={0}1<={1})", prevMonth, calcDate));
            cdo.AddDetailColumn(String.Format("[4](1>={0}1<={1})", prevMonth, calcDate));
            cdo.AddDetailColumn(String.Format("-[3](1<={0})[4](1<={0})", calcDate));
            cdo.AddCalcColumn(CalcColumnType.cctContractNum4);
            cdo.columnParamList[5][t_S_FactDebtCO.Offset] = "0";
            cdo.columnParamList[6][t_S_FactDebtCO.Offset] = "1";
            cdo.useSummaryRow = false;
            var dtResult = cdo.FillData();
            dtResult = ConvertRegionNames(dtResult);
            dtTables[0] = CreateReportCaptionTable(5 + dtResult.Rows.Count * 4);
            var columnParams = new TerritoryColumnParam
                                   {
                                       columnKind = 3,
                                       dtResult = dtTables[0],
                                       dtContracts = dtResult,
                                       copyEachMonthRecords = false,
                                       monthIndex = 1,
                                       sumArray = new decimal[dtResult.Rows.Count * 4 + 4],
                                       colIndex1 = 0,
                                       colIndex2 = 2,
                                       writeSummary = true
                                   };

            FillCommonTerritoryData(columnParams);
            dtTables[1] = CreateReportCaptionTable(1);
            
            foreach (DataRow drContract in dtResult.Rows)
            {
                var drContractName = dtTables[1].Rows.Add();
                drContractName[0] = drContract[8];
            }
            
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = prevMonth;
            drCaption[2] = 5 + dtResult.Rows.Count * 4;
            drCaption[3] = dtTables[0].Rows.Count;
            drCaption[4] = dtResult.Rows.Count;
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Справка по начисленным процентам за выданные кредиты по муниципальным образованиям области на 01 января отчетного года"
        /// </summary>
        public DataTable[] GetPlanServiceYearVologdaData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            var calcDate = GetYearStart(reportParams[ReportConsts.ParamEndDate]);
            var cdo = new CreditIssuedDataObject();
            var year = InitObjectCI(cdo, calcDate);
            cdo.mainFilter[f_S_Creditissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdo.mainFilter[f_S_Creditissued.RefRegions] = FormNegFilterValue(ReportConsts.UndefinedRegion);

            for (var i = 1; i < 13; i++)
            {
                var monthEnd = GetMonthEnd(year, i);
                cdo.AddDetailColumn(String.Format("[2](1<={0})", monthEnd));
            }

            var dtResult = cdo.FillData();
            dtResult = ConvertRegionNames(dtResult);
            dtTables[0] = CreateReportCaptionTable(13);
            var columnParams = new TerritoryColumnParam
                                   {
                                       sumArray = new decimal[12],
                                       columnKind = 2,
                                       dtResult = dtTables[0],
                                       dtContracts = dtResult,
                                       copyEachMonthRecords = false
                                   };

            for (var i = 1; i < 13; i++)
            {
                columnParams.monthIndex = i;
                columnParams.colIndex1 = 0;
                columnParams.colIndex2 = 2;
                columnParams.writeSummary = true;
                FillCommonTerritoryData(columnParams);
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = year;
            drCaption[1] = year + 1;
            var dtData = GetPercentCreditIssSubjectsData(reportParams);
            dtTables[2] = dtData[0];
            dtTables[3] = dtData[1];
            dtTables[4] = dtData[2];
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Справка по начисленным пеням за просроченные проценты и основную сумму кредита"
        /// </summary>
        public DataTable[] GetPeniPercentMainDebtData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var monthNum = GetMonthNum(calcDate);
            var cdo = new CreditIssuedDataObject();
            var year = InitObjectCI(cdo, calcDate);
            cdo.mainFilter.Remove(f_S_Creditissued.StartDate);
            cdo.mainFilter.Remove(f_S_Creditissued.EndDate);
            cdo.mainFilter[f_S_Creditissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdo.mainFilter[f_S_Creditissued.RefRegions] = FormNegFilterValue(ReportConsts.UndefinedRegion);

            for (var i = 1; i < 13; i++)
            {
                var monthStart = GetMonthStart(year, i);
                var monthEnd = GetMonthEnd(year, i);
                
                if (DateTime.Compare(Convert.ToDateTime(monthEnd), Convert.ToDateTime(calcDate)) > 0)
                {
                    monthEnd = Convert.ToDateTime(calcDate).ToShortDateString();
                }

                cdo.AddDetailColumn(String.Format("[6](0>={0}0<={1})", monthStart, monthEnd));
                cdo.AddDetailColumn(String.Format("[5](0>={0}0<={1})", monthStart, monthEnd));
            }

            cdo.AddDetailColumn(String.Format("[6](0>={0}0<={1})", GetYearStart(year), calcDate));
            cdo.AddDetailColumn(String.Format("[5](0>={0}0<={1})", GetYearStart(year), calcDate));
            cdo.AddDetailColumn(String.Format("+[5](0>={0}0<={1})[6](0>={0}0<={1})", GetYearStart(year), calcDate));
            var dtResult = cdo.FillData();
            dtResult = ConvertRegionNames(dtResult);
            dtTables[0] = CreateReportCaptionTable(6);
            
            var columnParams = new TerritoryColumnParam
                                   {
                                       columnKind = 0,
                                       dtResult = dtTables[0],
                                       dtContracts = dtResult,
                                       copyEachMonthRecords = true
                                   };

            for (var i = 1; i < 14; i++)
            {
                columnParams.sumArray = new decimal[2];
                columnParams.monthIndex = i;
                columnParams.colIndex1 = 0;
                columnParams.colIndex2 = 2;
                columnParams.writeSummary = true;
                FillCommonTerritoryData(columnParams);
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = monthNum;
            drCaption[2] = dtTables[0].Rows.Count / 13;
            drCaption[3] = year;
            drCaption[4] = GetMonthText1(monthNum - 0);
            drCaption[5] = GetMonthText2(monthNum - 1);
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Справка по начисленным процентам за выданные кредиты"
        /// </summary>
        public DataTable[] GetPercentCreditIssSubjectsData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var cdo = new CreditIssuedDataObject();
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var monthNum = GetMonthNum(calcDate);
            var year = InitObjectCI(cdo, calcDate);
            cdo.mainFilter[f_S_Creditissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdo.AddCalcColumn(CalcColumnType.cctContractNum4);

            for (var i = 1; i < 13; i++)
            {
                var monthStart = GetMonthStart(year, i);
                var monthEnd = GetMonthEnd(year, i);
                cdo.AddDetailColumn(String.Format("-[0](0<={0})[4](1<={0})", monthEnd));
                cdo.AddDetailColumn(String.Format("[2](1>={0}1<={1})", monthStart, monthEnd));
            }

            cdo.useSummaryRow = false;
            var dtResult = cdo.FillData();
            dtResult = ConvertRegionNames(dtResult);
            dtTables[0] = CreateReportCaptionTable(3 + dtResult.Rows.Count * 2);
            dtTables[1] = CreateReportCaptionTable(1);
            
            var columnParams = new TerritoryColumnParam
                                   {
                                       columnKind = 1,
                                       dtResult = dtTables[0],
                                       dtContracts = dtResult,
                                       copyEachMonthRecords = true
                                   };

            for (var i = 1; i < 13; i++)
            {
                columnParams.sumArray = new decimal[dtResult.Rows.Count * 2 + 2];
                columnParams.monthIndex = i;
                columnParams.colIndex1 = 0;
                columnParams.colIndex2 = 2;
                columnParams.writeSummary = true;
                FillCommonTerritoryData(columnParams);
            }

            foreach (DataRow drContract in dtResult.Rows)
            {
                var drContractName = dtTables[1].Rows.Add();
                drContractName[0] = drContract[4];
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = monthNum;
            drCaption[2] = 3 + dtResult.Rows.Count * 2;
            drCaption[3] = dtTables[0].Rows.Count / 12;
            drCaption[4] = dtResult.Rows.Count;
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Справка по начисленным пеням в отчетном месяце отчетного года за просроченные проценты по выданным кредитам  муниципальным образованиям области"
        /// </summary>
        public DataTable[] GetPeniPercentMainDebtCurMonthData(Dictionary<string, string> reportParams)
        {
            var dtReport = GetPeniPercentMainDebtData(reportParams);
            var monthNum = Convert.ToInt32(dtReport[1].Rows[0][1]);
            var monthRecCount = Convert.ToInt32(dtReport[1].Rows[0][2]);

            for (var i = 0; i < monthNum - 1; i++)
            {
                for (var j = 0; j < monthRecCount; j++)
                {
                    dtReport[0].Rows.RemoveAt(0);
                }
            }

            for (var i = monthNum + 1; i < 14; i++)
            {
                for (var j = 0; j < monthRecCount; j++)
                {
                    dtReport[0].Rows.RemoveAt(monthRecCount);
                }
            }

            return dtReport;
        }

        /// <summary> 
        /// Бюджетные кредиты предоставленные Самара
        /// </summary> 
        public DataTable[] GetBudgetCreditIssuedSamaraData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            
            for (var i = 0; i < 3; i++)
            {
                reportParams[ReportConsts.ParamYear] = Convert.ToString(year + i);
                dtTables[i] = GetBudgetCreditIssuedData(reportParams)[0];
            }

            return dtTables;
        }

        /// <summary> 
        /// Бюджетные кредиты предоставленные Омск
        /// </summary> 
        public DataTable[] GetBudgetCreditIssuedData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[1];
            var cdo = new CreditIssuedDataObject();
            cdo.InitObject(scheme);
            cdo.mainFilter.Add(f_S_Creditissued.RefSStatusPlan, "<>-1;<>1;<>2;<>3;<>4");
            cdo.mainFilter.Add(f_S_Creditissued.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdo.mainFilter.Add(f_S_Creditissued.RefSTypeCredit, ReportConsts.BudCreditIssuedCode);
            // Список дат для колонок
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var shortCalcDateStart = GetYearStart(year);
            var shortCalcDateEnd = GetYearEnd(year);
            // Формирование колонок
            cdo.AddDetailColumn(String.Format("[0](0<={0}0>={1})", shortCalcDateEnd, shortCalcDateStart));
            cdo.AddDetailColumn(String.Format("[1](1<={0}1>={1})", shortCalcDateEnd, shortCalcDateStart));
            // Заполнение данными
            var dtResult1 = cdo.FillData();
            var sum11 = GetLastRowValue(dtResult1, 0);
            var sum12 = GetLastRowValue(dtResult1, 1);
            cdo.mainFilter[f_S_Creditissued.RefSTypeCredit] = ReportConsts.OrgCreditIssuedCode;
            var dtResult2 = cdo.FillData();
            var sum21 = GetLastRowValue(dtResult2, 0);
            var sum22 = GetLastRowValue(dtResult2, 1);
            dtTables[0] = CreateReportTableDebtStructure();
            var drResult = dtTables[0].Rows.Add();
            drResult[0] = reportParams[ReportConsts.ParamYear];
            drResult[1] = ConvertTo1000(sum21);
            drResult[2] = ConvertTo1000(sum11);
            drResult[3] = ConvertTo1000(sum22);
            drResult[4] = ConvertTo1000(sum12);
            return dtTables;
        }
    }
}