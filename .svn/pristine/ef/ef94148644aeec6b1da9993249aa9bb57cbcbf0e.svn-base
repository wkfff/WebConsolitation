using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIssued;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;
using Krista.FM.Client.Reports.Planning.Data;
using Krista.FM.Client.Reports.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// Калмыкия - Сводный график погашения долговых обязательств
        /// </summary>
        public DataTable[] GetVaultGraphDebtKalmykiaData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var loYearBound = GetYearStart(reportDate.Year);
            var hiYearBound = GetYearEnd(reportDate.Year);
            var rowCaption = CreateReportParamsRow(tables);
            const string factAtrKey = t_S_FactAttractCI.key;
            const string planDbtKey = t_S_PlanDebtCI.key;
            const string factDbtKey = t_S_FactDebtCI.key;
            const string planPctKey = t_S_PlanServiceCI.key;
            const string factPctKey = t_S_FactPercentCI.key;
            // Кредиты
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.useSummaryRow = false;
            cdoCredit.onlyLastPlanService = true;
            cdoCredit.onlyLastPlanDebt = true;
            cdoCredit.planServiceDate = maxDate;
            cdoCredit.planDebtDate = maxDate;
            cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            // 00
            cdoCredit.AddCalcColumn(CalcColumnType.cctOrganization);
            // 01
            cdoCredit.AddDataColumn(f_S_Creditincome.Purpose);
            // 02
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            // 03
            cdoCredit.AddDataColumn(f_S_Creditincome.CreditPercent);
            // 04
            cdoCredit.AddDataColumn(f_S_Creditincome.StartDate);
            // 05
            cdoCredit.AddDataColumn(f_S_Creditincome.EndDate);
            // 06
            cdoCredit.AddDetailColumn(String.Format("-[{0}](1<{2})[{1}](1<{2})", factAtrKey, factDbtKey, calcDate));
            // 07
            cdoCredit.AddDetailColumn(String.Format("[{0}](1>={1}1<={2})", planDbtKey, loYearBound, hiYearBound));
            // 08
            cdoCredit.AddDetailColumn(String.Format("[{0}](1>={1}1<={2})", planPctKey, loYearBound, hiYearBound));

            for (var i = 1; i < 13; i++)
            {
                var loMonthBound = GetMonthStart(reportDate.Year, i);
                var hiMonthBound = GetMonthEnd(reportDate.Year, i);

                var formulaDbt = String.Format("[{0}](1>={1}1<={2})", planDbtKey, loMonthBound, hiMonthBound);
                var formulaPct = String.Format("[{0}](1>={1}1<={2})", planPctKey, loMonthBound, hiMonthBound);

                if (i < reportDate.Month)
                {
                    formulaDbt = String.Format("[{0}](1>={1}1<={2})", factDbtKey, loMonthBound, hiMonthBound);
                    formulaPct = String.Format("[{0}](1>={1}1<={2})", factPctKey, loMonthBound, hiMonthBound);
                }

                cdoCredit.AddDetailColumn(formulaDbt);
                cdoCredit.AddDetailColumn(formulaPct);
            }

            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            tables[0] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            tables[1] = cdoCredit.FillData();
            rowCaption[0] = calcDate;
            rowCaption[1] = reportDate.Year;
            return tables;
        }

        /// <summary>
        /// Калмыкия - График погашения долговых обязательств
        /// </summary>
        public DataTable[] GetGraphDebtKalmykiaData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[3];
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var rowCaption = CreateReportParamsRow(tables);
            // Кредиты
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.useSummaryRow = false;
            cdoCredit.onlyLastPlanService = true;
            cdoCredit.onlyLastPlanDebt = true;
            cdoCredit.planServiceDate = maxDate;
            cdoCredit.planDebtDate = maxDate;
            cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            // 00
            cdoCredit.AddCalcColumn(CalcColumnType.cctOrganization);
            // 01
            cdoCredit.AddDataColumn(f_S_Creditincome.Purpose);
            // 02
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            // 03
            cdoCredit.AddDataColumn(f_S_Creditincome.CreditPercent);
            // 04
            cdoCredit.AddDataColumn(f_S_Creditincome.StartDate);
            // 05
            cdoCredit.AddDataColumn(f_S_Creditincome.EndDate);
            // 06
            cdoCredit.AddParamColumn(CalcColumnType.cctRelation, "+7;+8;+9;+10");
            // 07-10
            for (var i = 0; i < 4; i++)
            {
                var curYear = year + i;
                var loYearBoud = GetYearStart(curYear);
                var hiYearBoud = GetYearEnd(curYear);
                var formulaPlan = String.Format("[{0}](1>={1}1<={2})", t_S_PlanDebtCI.key, loYearBoud, hiYearBoud);
                cdoCredit.AddDetailColumn(formulaPlan);
                rowCaption[10 + i] = curYear;
            }

            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            tables[0] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            tables[1] = cdoCredit.FillData();
            rowCaption[0] = year;
            return tables;
        }

        /// <summary>
        /// Московская область - Прогноз изменений объема долговых обязательств Московской области
        /// </summary>
        public DataTable[] GetMOForecastDebtVolumeChangeData(Dictionary<string, string> reportParams)
        {
            const int MarkColumn = 0;
            const int DataColumn = 1;
            const int FlagColumn = 2;
            const int DataIndex = 0;
            const int SummIndex = 50;
            const int MarkIndex = 100;
            const int YearCount = 4;
            var dtTables = new DataTable[5];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var year = reportDate.Year;
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exchgRate = Convert.ToDecimal(reportParams[ReportConsts.ParamExchangeRate]);
            decimal sumBalance = 0;

            if (reportParams[ReportConsts.ParamSum].Length > 0)
            {
                sumBalance = Convert.ToDecimal(reportParams[ReportConsts.ParamSum]);
            }

            const string crdFormulaDebt = "-[0](1<{0})[1](1<{0})";
            const string crdFormulaPlan = "[2](1>={0}1<={1})";

            const string capFormulaDebt = "-[0](1<{0})[1](1<{0})";
            const string capFormulaPlan = "[6](1>={0}1<={1})";

            const string grnFormulaDebt = "-+--[0](1<{1})[1](1<{0})[2](1<{0})[5](1<{0})[4](1<{0})";
            const string grnFormulaPlan = "[3](1>={0}1<={1})";

            var periodList = new Dictionary<string, string> { { calcDate, GetYearEnd(year) } };

            for (var i = 1; i < YearCount; i++)
            {
                periodList.Add(GetYearStart(year + i), GetYearEnd(year + i));                
            }

            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.onlyLastPlanService = true;
            cdoCredit.onlyLastPlanDebt = true;
            cdoCredit.planServiceDate = calcDate;
            cdoCredit.planDebtDate = calcDate;
            cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoCredit.AddDetailColumn(String.Format(crdFormulaDebt, calcDate));
            cdoCredit.AddDataColumn(f_S_Creditincome.Num);
            cdoCredit.AddDataColumn(f_S_Creditincome.RefSTypeCredit);

            foreach (var period in periodList)
            {
                var crdFormulaYearPlan = String.Format(crdFormulaPlan, period.Key, period.Value);
                cdoCredit.AddDetailColumn(crdFormulaYearPlan);
                cdoCredit.AddDetailTextColumn(crdFormulaYearPlan, cdoCredit.ParamOnlyDates, String.Empty);                
            }

            var tblCredit = cdoCredit.FillData();

            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCap.AddDetailColumn(String.Format(capFormulaDebt, calcDate));
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            cdoCap.AddDataColumn(f_S_Capital.RegNumber);

            foreach (var period in periodList)
            {
                var capFormulaYearPlan = String.Format(capFormulaPlan, period.Key, period.Value);
                cdoCap.AddDetailColumn(capFormulaYearPlan);
                cdoCap.AddDetailTextColumn(capFormulaYearPlan, cdoCap.ParamOnlyDates, String.Empty);
            }

            var tblCapital = cdoCap.FillData();

            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.planServiceDate = calcDate;
            cdoGrnt.fixedExchangeRate.Add(GetUsdCode(), exchgRate);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdoGrnt.AddDetailColumn(String.Format(grnFormulaDebt, calcDate, maxDate));
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, 
                Combine(ReportConsts.GrntTypeSumDbt, ReportConsts.GrntTypeSumPct));
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);

            foreach (var period in periodList)
            {
                var grnFormulaYearPlan = String.Format(grnFormulaPlan, period.Key, period.Value);
                cdoGrnt.AddDetailColumn(grnFormulaYearPlan);
                cdoGrnt.AddDetailTextColumn(grnFormulaYearPlan, cdoGrnt.ParamOnlyDates, String.Empty);
            }

            var tblGarant = cdoGrnt.FillData();

            // Предельные объемы долга

            var cdoLimit = new CommonDataObject();
            cdoLimit.InitObject(scheme);
            cdoLimit.useSummaryRow = false;
            cdoLimit.ObjectKey = f_S_DebtLimit.internalKey;
            cdoLimit.mainFilter[f_S_DebtLimit.StateDebtUpLim] = "> 0";
            cdoLimit.AddDataColumn(f_S_DebtLimit.StateDebtUpLim);
            cdoLimit.AddDataColumn(f_S_DebtLimit.AcceptDate, ReportConsts.ftDateTime);
            cdoLimit.sortString = StrSortUp(f_S_DebtLimit.AcceptDate);
            var tblLimit = cdoLimit.FillData();
            var rowLimit = GetLastRow(tblLimit);
            decimal sumLimit = 0;

            if (rowLimit != null)
            {
                sumLimit = Convert.ToDecimal(rowLimit[f_S_DebtLimit.StateDebtUpLim]);
            }
          
            var crdSummary = GetLastRow(tblCredit);
            var capSummary = GetLastRow(tblCapital);
            var grnSummary = GetLastRow(tblGarant);

            for (var i = 0; i < YearCount; i++)
            {
                var firstPlanDate = calcDate;
                var planIndex = 3 + i * 2;

                if (i > 0)
                {
                    firstPlanDate = GetYearStart(year + i);
                }

                var yearLoBound = Convert.ToDateTime(firstPlanDate);
                var yearLoBoundStr = yearLoBound.ToShortDateString();
                var yearHiBound = Convert.ToDateTime(GetYearEnd(year + i));
                var currentYear = Convert.ToDateTime(firstPlanDate).Year;

                var tblResult = CreateReportCaptionTable(3);
                dtTables[i] = tblResult;
                var startDebt = tblResult.Rows.Add();
                startDebt[MarkColumn] = String.Format("Долг на {0}", firstPlanDate);
                startDebt[DataColumn] = GetNumber(crdSummary[0]) + GetNumber(capSummary[0]) + GetNumber(grnSummary[0]);
                startDebt[FlagColumn] = SummIndex + 0;

                var markIncome = tblResult.Rows.Add();
                markIncome[MarkColumn] = String.Format("До конца {0} года", currentYear);
                markIncome[FlagColumn] = MarkIndex;

                var crdIncome = tblResult.Rows.Add();
                crdIncome[MarkColumn] = "Привлеч.ком.кред - всего";
                var attrSum = Convert.ToDecimal(crdSummary[planIndex]) + Convert.ToDecimal(capSummary[planIndex]);

                if (i == 0)
                {
                    attrSum += GetNumber(sumBalance);
                }

                crdIncome[DataColumn] = attrSum;
                crdIncome[FlagColumn] = SummIndex + 1;

                var fltPlan = String.Format("{0} > 0 and {1} > 0", tblCredit.Columns[planIndex], f_S_Creditincome.id);
                
                var rowsCrdInc = tblCredit.Select(fltPlan);
                var rowsCapInc = tblCapital.Select(fltPlan);
                var rowsGrnInc = tblGarant.Select(fltPlan);

                var capPlanDbtKey = Convert.ToInt32(t_S_CPPlanDebt.key);
                var tblCapDbtPlan = cdoCap.dtDetail[capPlanDbtKey];

                foreach (var dataRow in rowsCapInc)
                {
                    var key = Convert.ToInt32(dataRow[f_S_Capital.id]);
                    cdoCap.GetSumValue(tblCapDbtPlan, key, t_S_CPPlanDebt.EndDate, t_S_CPPlanDebt.Sum, yearLoBound, yearHiBound, true, true);

                    foreach (var row in cdoCap.sumIncludedRows)
                    {
                        var rowCapital = tblResult.Rows.Add();
                        rowCapital[MarkColumn] = String.Format("на амортизацию ({0})",
                            GetPrevDay(row[t_S_CPPlanDebt.EndDate]));
                        rowCapital[DataColumn] = row[t_S_CPPlanDebt.Sum];
                        rowCapital[FlagColumn] = DataIndex + 1;                        
                    }
                }

                var crdPlanDbtKey = Convert.ToInt32(t_S_PlanDebtCI.key);

                foreach (var dataRow in rowsCrdInc)
                {
                    var key = Convert.ToInt32(dataRow[f_S_Creditincome.id]);
                    var formula = String.Format(crdFormulaDebt, yearLoBoundStr);
                    cdoCredit.FilterDetailTables(key);
                    var dbtSum = cdoCredit.ParseFormula(key, ref formula);

                    if (Math.Abs(dbtSum) > 0)
                    {
                        var tblCrdDbtPlan = cdoCredit.dtDetail[crdPlanDbtKey];
                        cdoCredit.GetSumValue(tblCrdDbtPlan, key, t_S_PlanDebtCI.EndDate, t_S_PlanDebtCI.Sum, yearLoBound,
                                              yearHiBound, true, true);

                        foreach (var row in cdoCredit.sumIncludedRows)
                        {
                            var rowCredit = tblResult.Rows.Add();
                            rowCredit[MarkColumn] = String.Format(
                                "{1} на погашение бюджетного кредита ({0})",
                                GetPrevDay(row[t_S_PlanDebtCI.EndDate]),
                                dataRow[1]);
                            rowCredit[DataColumn] = row[t_S_PlanDebtCI.Sum];
                            rowCredit[FlagColumn] = DataIndex + 2;
                        }
                    }
                }

                if (i == 0 && sumBalance != 0)
                {
                    var rowBalance = tblResult.Rows.Add();
                    rowBalance[MarkColumn] = String.Format("На исполнение расходных обязательств {0} года.", 
                        currentYear);
                    rowBalance[DataColumn] = sumBalance;
                    rowBalance[FlagColumn] = SummIndex + 2;
                }

                var markOutcome = tblResult.Rows.Add();
                markOutcome[MarkColumn] = String.Format("Погашение до конца {0}  года", currentYear);
                markOutcome[FlagColumn] = MarkIndex + 1;

                foreach (var dataRow in rowsCapInc)
                {
                    var key = Convert.ToInt32(dataRow[f_S_Capital.id]);
                    cdoCap.GetSumValue(tblCapDbtPlan, key, t_S_CPPlanDebt.EndDate, t_S_CPPlanDebt.Sum, yearLoBound, yearHiBound, true, true);

                    foreach (var row in cdoCap.sumIncludedRows)
                    {
                        var rowCapital = tblResult.Rows.Add();
                        rowCapital[MarkColumn] = String.Format(
                            "Амортизация {0}({1:d})",
                            dataRow[1],
                            row[t_S_CPPlanDebt.EndDate]
                            );
                        rowCapital[DataColumn] = row[t_S_CPPlanDebt.Sum];
                        rowCapital[FlagColumn] = DataIndex + 3;
                    }
                }

                var rowCrdOutcome = tblResult.Rows.Add();
                rowCrdOutcome[MarkColumn] = "Кредиты";
                rowCrdOutcome[DataColumn] = crdSummary[planIndex];
                rowCrdOutcome[FlagColumn] = SummIndex + 3;

                foreach (var dataRow in rowsCrdInc)
                {
                    var key = Convert.ToInt32(dataRow[f_S_Creditincome.id]);
                    var formula = String.Format(crdFormulaDebt, yearLoBoundStr);
                    cdoCredit.FilterDetailTables(key);
                    var dbtSum = cdoCredit.ParseFormula(key, ref formula);

                    if (Math.Abs(dbtSum) > 0)
                    {
                        var tblCrdDbtPlan = cdoCredit.dtDetail[crdPlanDbtKey];
                        cdoCredit.GetSumValue(tblCrdDbtPlan, key, t_S_PlanDebtCI.EndDate, t_S_PlanDebtCI.Sum, yearLoBound,
                                              yearHiBound, true, true);

                        foreach (var row in cdoCredit.sumIncludedRows)
                        {
                            var rowCredit = tblResult.Rows.Add();
                            rowCredit[MarkColumn] = String.Format(
                                "{0} {1:d}",
                                dataRow[1],
                                row[t_S_PlanDebtCI.EndDate]);

                            rowCredit[DataColumn] = row[t_S_PlanDebtCI.Sum];
                            rowCredit[FlagColumn] = DataIndex + 4;
                        }
                    }
                }

                var rowGrnOutcome = tblResult.Rows.Add();
                rowGrnOutcome[MarkColumn] = "Гарантии";
                rowGrnOutcome[DataColumn] = grnSummary[planIndex];
                rowGrnOutcome[FlagColumn] = SummIndex + 4;

                var grnPlanDbtKey = Convert.ToInt32(t_S_PlanDebtPrGrnt.key);

                foreach (var dataRow in rowsGrnInc)
                {
                    var key = Convert.ToInt32(dataRow[f_S_Guarantissued.id]);
                    cdoGrnt.FilterDetailTables(key);
                    var tblGrnDbtPlan = cdoGrnt.dtDetail[grnPlanDbtKey];
                    cdoGrnt.GetSumValue(tblGrnDbtPlan, key, t_S_PlanDebtPrGrnt.EndDate, t_S_PlanDebtPrGrnt.Sum, yearLoBound, yearHiBound, true, true);

                    foreach (var row in cdoGrnt.sumIncludedRows)
                    {
                        var rowGarant = tblResult.Rows.Add();
                        rowGarant[MarkColumn] = String.Format("{0} ({1:d})", dataRow[1], row[t_S_PlanDebtPrGrnt.EndDate]);
                        rowGarant[DataColumn] = row[t_S_PlanDebtPrGrnt.Sum];
                        rowGarant[FlagColumn] = DataIndex + 5;
                    }
                }

                foreach (var dataRow in rowsGrnInc)
                {
                    var key = Convert.ToInt32(dataRow[f_S_Guarantissued.id]);
                    var formula = String.Format("[5](1<={0}1>={1})", yearLoBoundStr, yearHiBound);
                    cdoGrnt.FilterDetailTables(key);
                    var pctSum = cdoGrnt.ParseFormula(key, ref formula);
                    
                    if (Math.Abs(pctSum) > 0)
                    {
                        var rowGarant = tblResult.Rows.Add();
                        rowGarant[MarkColumn] = "процент бл";
                        rowGarant[DataColumn] = pctSum;
                        rowGarant[FlagColumn] = DataIndex + 6;
                    }
                }

                var rowOutcome = tblResult.Rows.Add();
                rowOutcome[MarkColumn] = String.Format("ИТОГО погашение в {0} году", currentYear);
                rowOutcome[DataColumn] = 
                    GetNumber(grnSummary[planIndex]) + 
                    GetNumber(crdSummary[planIndex]) + 
                    GetNumber(capSummary[planIndex]);
                rowOutcome[FlagColumn] = SummIndex + 5;

                if (i == 1)
                {
                    startDebt[DataColumn] = sumLimit;
                }

                DataTable tblPrev;
                DataRow rowPrev = null;

                if (i > 0)
                {
                    tblPrev = dtTables[i - 1];
                    var rowsPrev = tblPrev.Select(String.Format("{0}='{1}'", tblPrev.Columns[FlagColumn], SummIndex + 6));
                    
                    if (rowsPrev.Length > 0)
                    {
                        rowPrev = rowsPrev[0];

                        if (i > 1)
                        {
                            startDebt[DataColumn] = rowPrev[DataColumn];
                        }
                    }
                }

                var rowSummary = tblResult.Rows.Add();
                rowSummary[MarkColumn] = String.Format("Долг на {0}", GetYearStart(currentYear + 1));
                rowSummary[DataColumn] =
                    GetNumber(startDebt[DataColumn]) +
                    GetNumber(crdSummary[planIndex]) +
                    GetNumber(capSummary[planIndex]) -
                    GetNumber(rowOutcome[DataColumn]);
                rowSummary[FlagColumn] = SummIndex + 6;

                if (i <= 0 || rowPrev == null)
                {
                    continue;
                }

                var rowChange = tblResult.Rows.Add();
                rowChange[MarkColumn] = "Уменьшение долга";
                rowChange[DataColumn] = GetNumber(rowPrev[DataColumn]) - GetNumber(startDebt[DataColumn]);
                rowChange[FlagColumn] = SummIndex + 7;
            }

            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = exchgRate;
            drCaption[2] = sumBalance;
            drCaption[3] = year;
            drCaption[4] = YearCount;
            return dtTables;
        }

        /// <summary>
        /// Московская область - Расчет обслуживания
        /// </summary>
        public DataTable[] GetMOCalcServiceReportData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var year = reportDate.Year;
            var strYearEnd = GetYearEnd(calcDate);
            var strYearStart = GetYearStart(calcDate);
            var dateYearEnd = Convert.ToDateTime(strYearEnd);
            var dateYearStart = Convert.ToDateTime(strYearStart);
            var dayCount = dateYearEnd.DayOfYear;
            var fltVariant = ReportConsts.ActiveVariantID;
            var planVariant = reportParams[ReportConsts.ParamVariantID];

            if (planVariant != ReportConsts.UndefinedKey)
            {
                fltVariant = Combine(fltVariant, planVariant);
            }

            // Кредиты бюджетов
            var fltRest = String.Format("{0} = {1} or {2} <> 0",
                f_S_Creditincome.RefVariant,
                planVariant,
                TempFieldNames.SortStatus);
            var formulaRest = String.Format("-[0](1<{0})[1](1<{1})", maxDate, calcDate);

            var cdoBudCredit = new CreditDataObject();
            cdoBudCredit.InitObject(scheme);
            cdoBudCredit.useSummaryRow = false;
            cdoBudCredit.onlyLastPlanService = true;
            cdoBudCredit.planServiceDate = calcDate;
            cdoBudCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoBudCredit.mainFilter[f_S_Creditincome.RefVariant] = fltVariant;
            cdoBudCredit.AddCalcColumn(CalcColumnType.cctCreditNumContractDateOrg);
            cdoBudCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoBudCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoBudCredit.AddDataColumn(f_S_Creditincome.EndDate);
            cdoBudCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoBudCredit.AddDataColumn(f_S_Creditincome.Sum);
            cdoBudCredit.AddDataColumn(f_S_Creditincome.CreditPercent);
            cdoBudCredit.AddParamColumn(CalcColumnType.cctRelation, "+8;+9");
            cdoBudCredit.AddDetailColumn(String.Format("-[5](2<={0})[4](1<={1})", strYearEnd, calcDate));
            cdoBudCredit.AddDetailColumn(String.Format("[4](1<={0})", calcDate), CreateValuePair(t_S_FactPercentCI.ChargeSum), true);
            cdoBudCredit.SetColumnParam(cdoBudCredit.ParamSumValueType, "-");
            cdoBudCredit.AddDataColumn(f_S_Creditincome.RefVariant, ReportConsts.ftInt32);
            cdoBudCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoBudCredit.SetColumnNameParam(TempFieldNames.RowType);
            cdoBudCredit.AddDetailColumn(formulaRest);
            cdoBudCredit.SetColumnNameParam(TempFieldNames.SortStatus);
            cdoBudCredit.AddDetailColumn(String.Format("[5](2<={0})", strYearEnd));
            cdoBudCredit.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{1})", maxDate, strYearStart));
            cdoBudCredit.sortString = StrSortUp(f_S_Creditincome.RefVariant);
            var tblCreditBud = cdoBudCredit.FillData();

            tblCreditBud = DataTableUtils.FilterDataSet(tblCreditBud, fltRest);

            foreach (DataRow row in tblCreditBud.Rows)
            {
                var isPlanContract = Convert.ToString(row[f_S_Creditincome.RefVariant]) != ReportConsts.ActiveVariantID;
                row[1] = strYearStart;
                row[2] = strYearEnd;
                row[4] = dayCount;

                if (isPlanContract)
                {
                    row[7] = row[13]; 
                }
                else
                {
                    row[5] = row[14]; 
                }

                row[TempFieldNames.RowType] = isPlanContract;
            }

            dtTables[0] = tblCreditBud;

            // Комемрческие кредитыъ

            var cdoOrgCredit = new CreditDataObject();
            cdoOrgCredit.InitObject(scheme);
            cdoOrgCredit.useSummaryRow = false;
            cdoOrgCredit.onlyLastPlanService = true;
            cdoOrgCredit.planServiceDate = calcDate;
            cdoOrgCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            cdoOrgCredit.mainFilter[f_S_Creditincome.RefVariant] = fltVariant;
            cdoOrgCredit.AddCalcColumn(CalcColumnType.cctCreditNumContractDateOrg);
            cdoOrgCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoOrgCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoOrgCredit.AddDataColumn(f_S_Creditincome.EndDate);
            cdoOrgCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoOrgCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoOrgCredit.AddCalcColumn(CalcColumnType.cctNearestPercent);
            cdoOrgCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoOrgCredit.AddDataColumn(f_S_Creditincome.id);
            cdoOrgCredit.AddDetailColumn(String.Format("[{0}][{1}][{2}][{3}]",
                t_S_FactPercentCI.key,
                t_S_FactDebtCI.key,
                t_S_PlanServiceCI.key,
                t_S_FactAttractCI.key));
            cdoOrgCredit.AddDataColumn(f_S_Creditincome.RefVariant, ReportConsts.ftInt32);
            cdoOrgCredit.AddDetailColumn(formulaRest);
            cdoOrgCredit.SetColumnNameParam(TempFieldNames.SortStatus);
            cdoOrgCredit.AddDataColumn(f_S_Creditincome.Sum);
            cdoOrgCredit.AddDetailColumn(String.Format("[5](2>={0}2<={1})", strYearStart, strYearEnd));
            cdoOrgCredit.sortString = StrSortUp(f_S_Creditincome.RefVariant);
            var tblCreditOrg = cdoOrgCredit.FillData();
            tblCreditOrg = DataTableUtils.FilterDataSet(tblCreditOrg, fltRest);
            var tblCreditOrgResult = CreateReportCaptionTable(9);
            var tblDetailFact = cdoOrgCredit.dtDetail[Convert.ToInt32(t_S_FactDebtCI.key)];

            foreach (DataRow row in tblCreditOrg.Rows)
            {
                var isPlanContract = Convert.ToString(row[f_S_Creditincome.RefVariant]) != ReportConsts.ActiveVariantID;
                var key = Convert.ToInt32(row[f_S_Creditincome.id]);
                cdoOrgCredit.FilterDetailTables(key);
                var tblDetailPlan = cdoOrgCredit.dtDetail[Convert.ToInt32(t_S_PlanServiceCI.key)];
                cdoOrgCredit.GetSumValue(tblDetailPlan, key, t_S_PlanServiceCI.PaymentDate, t_S_PlanServiceCI.Sum, dateYearStart, dateYearEnd, true, true);

                var planLoDate = String.Empty;
                var planHiDate = String.Empty;
                var rows = cdoOrgCredit.sumIncludedRows;

                if (rows.Count > 0)
                {
                    planLoDate = Convert.ToString(rows[0][t_S_PlanServiceCI.StartDate]);
                    planHiDate = Convert.ToString(rows[rows.Count - 1][t_S_PlanServiceCI.PaymentDate]);
                }

                cdoOrgCredit.GetSumValue(tblDetailFact, key, t_S_FactDebtCI.FactDate, t_S_FactDebtCI.Sum, dateYearStart, dateYearEnd, true, true);

                var rowsFact = new Collection<DataRow>();

                foreach (var dataRow in rows)
                {
                    rowsFact.Add(dataRow);
                }

                var listRows = new Collection<DataRow>();

                for (var i = 0; i < Math.Max(1, rows.Count); i++)
                {
                    var rowResult = tblCreditOrgResult.Rows.Add();
                    listRows.Add(rowResult);
                }

                var rowCounter = 0;

                foreach (var rowResult in listRows)
                {
                    rowResult[0] = row[0];

                    if (rowCounter == 0)
                    {
                        rowResult[1] = planLoDate;

                        if (rowsFact.Count == 0)
                        {
                            rowResult[2] = planHiDate;
                        }
                        else
                        {
                            rowResult[2] = rowsFact[rowCounter][t_S_FactDebtCI.FactDate];
                        }
                    }
                    else
                    {
                        var rowPrev = listRows[rowCounter - 1];
                        rowResult[1] = Convert.ToDateTime(rowPrev[2]).AddDays(1).ToShortDateString();
                        rowResult[2] = rowsFact[rowCounter][t_S_FactDebtCI.FactDate];                     
                    }

                    DateTime loDate;
                    DateTime hiDate;

                    var hasLoDate = DateTime.TryParse(Convert.ToString(rowResult[1]), out loDate);
                    var hasHiDate = DateTime.TryParse(Convert.ToString(rowResult[2]), out hiDate);

                    rowResult[3] = row[3];

                    if (hasLoDate)
                    {
                        rowResult[1] = loDate.ToShortDateString();
                    }

                    if (hasHiDate)
                    {
                        rowResult[2] = hiDate.ToShortDateString();
                    }

                    if (hasHiDate && hasLoDate)
                    {
                        var ts = hiDate - loDate;
                        rowResult[4] = ts.Days + 1;
                        var formula = String.Format("-[0](1<{0})[1](1<{1})", maxDate, loDate.ToShortDateString());
                        rowResult[5] = cdoOrgCredit.ParseFormula(key, ref formula);
                    }

                    if (isPlanContract)
                    {
                        rowResult[5] = row[f_S_Creditincome.Sum];
                    }

                    rowResult[6] = row[6];
                    rowResult[7] = isPlanContract;
                    rowResult[8] = row[13];
                    rowCounter++;
                }

                row[1] = strYearStart;
                row[2] = strYearEnd;
                row[4] = dayCount;
            }

            dtTables[1] = tblCreditOrgResult;

            // ЦБ

            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.useSummaryRow = false;
            cdoCap.mainFilter[f_S_Capital.RefVariant] = fltVariant;
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddDataColumn(f_S_Capital.id);
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+7;+8");
            cdoCap.AddDetailColumn(String.Format("[7](1<={0})", calcDate));
            cdoCap.AddDetailColumn(String.Format("[3](1<={0})", calcDate), CreateValuePair(t_S_CPFactService.ChargeSum), true);
            cdoCap.SetColumnParam(cdoBudCredit.ParamSumValueType, "-");
            cdoCap.AddDataColumn(f_S_Capital.RefVariant, ReportConsts.ftInt32);
            cdoCap.sortString = FormSortString(
                StrSortUp(f_S_Capital.RefVariant),
                StrSortUp(f_S_Capital.OfficialNumber));
            var tblCapital = cdoCap.FillData();
            var tblCapitalResult = CreateReportCaptionTable(7);
            var tblPlanDetail = cdoCap.dtDetail[Convert.ToInt32(t_S_CPPlanService.key)];
            var tblFactDetail = cdoCap.dtDetail[Convert.ToInt32(t_S_CPFactService.key)];

            foreach (DataRow rowCapital in tblCapital.Rows)
            {
                var key = Convert.ToInt32(rowCapital[f_S_Capital.id]);
                cdoCap.GetSumValue(tblPlanDetail, key, t_S_CPPlanService.PaymentDate, t_S_CPPlanService.Sum, dateYearStart, dateYearEnd, true, true);

                var planRows = new Collection<DataRow>();

                foreach (var planRow in cdoCap.sumIncludedRows)
                {
                    planRows.Add(planRow);
                }

                cdoCap.GetSumValue(tblFactDetail, key, t_S_CPFactService.FactDate, t_S_CPFactService.ChargeSum, dateYearStart, dateYearEnd, true, true);
                var factCounter = 0;
                var factRows = cdoCap.sumIncludedRows;

                foreach (var rowDetail in planRows)
                {
                    var rowResult = tblCapitalResult.Rows.Add();
                    rowResult[0] = rowCapital[0];
                    rowResult[1] = rowDetail[t_S_CPPlanService.PaymentDate];
                    rowResult[2] = rowDetail[t_S_CPPlanService.Income];
                    rowResult[3] = rowDetail[t_S_CPPlanService.Quantity];

                    var planSum = GetNumber(rowDetail[t_S_CPPlanService.Sum]);
                    decimal factSum = 0;

                    if (factCounter < factRows.Count)
                    {
                        factSum = GetNumber(factRows[factCounter][t_S_CPFactService.ChargeSum]);
                        
                        if (factSum > 0)
                        {
                            factSum = 0;
                        }
                    }

                    rowResult[5] = planSum + factSum;
                    rowResult[6] = Convert.ToString(rowCapital[f_S_Capital.RefVariant]) != ReportConsts.ActiveVariantID;
                    factCounter++;
                }
            }

            dtTables[2] = tblCapitalResult;

            // Предельные объемы долга

            var cdoLimit = new CommonDataObject();
            var fltPeriodCur = String.Format("<='{0}'", strYearEnd);
            cdoLimit.InitObject(scheme);
            cdoLimit.useSummaryRow = false;
            cdoLimit.ObjectKey = f_S_DebtLimit.internalKey;
            cdoLimit.mainFilter[f_S_DebtLimit.AcceptDate] = fltPeriodCur;
            cdoLimit.mainFilter[f_S_DebtLimit.StateDebtService] = "> 0";
            cdoLimit.AddDataColumn(f_S_DebtLimit.DataComment);
            cdoLimit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoLimit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoLimit.AddDataColumn(f_S_DebtLimit.AcceptDate, ReportConsts.ftDateTime);
            cdoLimit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoLimit.AddDataColumn(f_S_DebtLimit.StateDebtService);
            cdoLimit.AddDataColumn(f_S_DebtLimit.RefYearDayUNV, ReportConsts.ftInt32);
            cdoLimit.sortString = StrSortUp(f_S_DebtLimit.AcceptDate);
            var tblLimit = cdoLimit.FillData();
            var fltPrvLimits = String.Format("{0}>='{1}' and {0}<'{2}'", 
                f_S_DebtLimit.AcceptDate, 
                GetYearStart(year - 1), 
                strYearStart);
            var tblPrevData = tblLimit.Select(fltPrvLimits, f_S_DebtLimit.AcceptDate);

            if (tblPrevData.Length > 0)
            {
                var rowLast = tblPrevData[tblPrevData.Length - 1];
                var fltPeriod = String.Format("{0}>='{1}'", f_S_DebtLimit.AcceptDate, strYearStart);
                tblLimit = DataTableUtils.FilterDataSet(tblLimit, fltPeriod);
                var prevRow = tblLimit.NewRow();
                prevRow.ItemArray = rowLast.ItemArray;
                tblLimit.Rows.InsertAt(prevRow, 0);
            }

            dtTables[3] = tblLimit;

            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = reportDate.Year;
            return dtTables;
        }

        public DataTable[] GetDebtorBookYarOblData(Dictionary<string, string> reportParams)
        {
            return GetDebtorBookCommonData(reportParams, SpreadDebtBookEnum.YaroslavlObl);
        }

        public DataTable[] GetDebtorBookSamaraOblData(Dictionary<string, string> reportParams)
        {
            return GetDebtorBookCommonData(reportParams, SpreadDebtBookEnum.SamaraObl);
        }

        /// <summary>
        /// Ярославль область - Долговая книга Ярославской области
        /// </summary>
        public DataTable[] GetDebtorBookCommonData(Dictionary<string, string> reportParams, SpreadDebtBookEnum region)
        {
            var tables = new DataTable[12];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var realReportDate = Convert.ToDateTime(calcDate);
            var dateYearStart = GetYearStart(calcDate);
            var year = Convert.ToDateTime(realReportDate).Year;
            var idValue = ReportConsts.UndefinedKey;

            if (reportParams.ContainsKey(ReportConsts.ParamStartDate))
            {
                idValue = GetMOSelectedRegNumFilter(reportParams[ReportConsts.ParamStartDate]);
            }

            if (realReportDate.Day == 1)
            {
                calcDate = realReportDate.AddDays(-1).ToShortDateString();
            }

            var rubFields = CreateValuePair(ReportConsts.SumField);
            var valFields = CreateValuePair(ReportConsts.CurrencySumField);
            var chargeFields = CreateValuePair(t_S_FactPercentCI.ChargeSum);

            var eurCode = GetEuroCode();

            // Ставка  рефинансирования Центрального Банка России
            var dataCB = new CommonDataObject();
            dataCB.InitObject(scheme);
            dataCB.useSummaryRow = false;
            dataCB.ObjectKey = d_S_JournalCB.internalKey;
            dataCB.AddDataColumn(d_S_JournalCB.InputDate, ReportConsts.ftDateTime);
            dataCB.AddDataColumn(d_S_JournalCB.PercentRate);
            var tblJournalCB = dataCB.FillData();

            // Кредиты
            var detailList = new Dictionary<int, string>
                                                     {
                                                         {00, t_S_FactAttractCI.FactDate},
                                                         {01, t_S_FactDebtCI.FactDate},
                                                         {
                                                             10,
                                                             CombineList(t_S_RateSwitchCI.EndDate,
                                                                         t_S_RateSwitchCI.RefTypeSum,
                                                                         ReportConsts.GrntTypeSumMainDbt,
                                                                         ReportConsts.GrntTypeSumPct)
                                                             },
                                                         {04, t_S_FactPercentCI.FactDate}
                                                     };

            const string selectDbtYearEx = "+-[0](1<{0})[1](1<{0})[10](1<{0})";
            const string selectPctYearEx = "+-[5](2<{0})[4](1<{0})[10](1<{0})";
            const string selectDbtYearValEx = "-[0](1<{0})[1](1<{0})";
            const string selectPctYearValEx = "-[5](2<{0})[4](1<{0})";
            const string selectDbtYearIn = "+-[0](1<={0})[1](1<={0})[10](1<={0})";
            const string selectDbtYearValIn = "-[0](1<={0})[1](1<={0})";

            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            cdoCI.useSummaryRow = false;
            cdoCI.ignoreZeroCurrencyRows = true;
            cdoCI.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdoCI.mainFilter.Add(f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr);

            if (idValue != ReportConsts.UndefinedKey)
            {
                cdoCI.mainFilter.Add(f_S_Creditincome.Num, idValue);
            }

            // колонки
            cdoCI.AddCalcColumn(CalcColumnType.cctContractType);
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditNumContractDateOrg);
            cdoCI.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdoCI.AddDataColumn(f_S_Creditincome.EndDate, ReportConsts.ftDateTime);
            cdoCI.AddCalcColumn(CalcColumnType.cctCollateralType);
            cdoCI.AddDataColumn(f_S_Creditincome.PenaltyDebtRate);
            // остатки на начало года
            // рублевые суммы
            cdoCI.AddDetailColumn(String.Format(selectDbtYearEx, dateYearStart), rubFields, true);
            cdoCI.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
            cdoCI.AddDetailColumn(String.Format(selectPctYearEx, dateYearStart), rubFields, true);
            cdoCI.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumPct);
            // валютные суммы
            cdoCI.AddDetailColumn(String.Format(selectDbtYearValEx, dateYearStart), valFields, true);
            cdoCI.AddDetailColumn(String.Format(selectPctYearValEx, dateYearStart), valFields, true);
            // служебные
            cdoCI.AddCalcColumn(CalcColumnType.cctPercentText);
            cdoCI.AddCalcNamedColumn(CalcColumnType.cctOKVName, TempFieldNames.OKVShortName);
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            cdoCI.AddDataColumn(f_S_Creditincome.Sum);
            cdoCI.AddDataColumn(f_S_Creditincome.Purpose);
            cdoCI.AddDetailColumn(String.Format("[{0}]", t_S_PlanDebtCI.key));

            cdoCI.sortString = FormSortString(
                StrSortUp(f_S_Creditincome.RefOKV), 
                StrSortDown(TempFieldNames.OrgName),
                StrSortDown(f_S_Creditincome.ContractDate),
                StrSortDown(f_S_Creditincome.EndDate));
            // Кредиты коммерческие
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            var tblOrgContracts = cdoCI.FillData();

            var orgCreditCount = SpreadChangesDataYar(
                cdoCI, 
                tblJournalCB,
                ReportDocumentType.CreditOrg,
                ref tables[0], 
                tblOrgContracts, 
                detailList, 
                dateYearStart,
                calcDate,
                region);

            // Кредиты бюджетов
            cdoCI.mainFilter.Remove(f_S_Creditincome.RefOKV);
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var tblBudContracts = cdoCI.FillData();

            var budCreditCount = SpreadChangesDataYar(
                cdoCI,
                tblJournalCB,
                ReportDocumentType.CreditBud,
                ref tables[1],
                tblBudContracts,
                detailList,
                dateYearStart,
                calcDate,
                region);

            var cdoCreditRubSummary = new CreditDataObject();
            cdoCreditRubSummary.InitObject(scheme);
            cdoCreditRubSummary.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCreditRubSummary.ignoreZeroCurrencyRows = true;
            // тут будут суммы на начало и конец года
            cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);

            // разбивка по месяцам
            for (var m = 1; m < 13; m++)
            {
                var monthLoBound = GetMonthStart(year, m);
                var monthHiBound = GetMonthEnd(year, m);
                // ОД на начало месяца
                cdoCreditRubSummary.AddDetailColumn(String.Format(selectDbtYearEx, monthLoBound), rubFields, true);
                cdoCreditRubSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
                cdoCreditRubSummary.AddDetailColumn(String.Format("[0](1>={0}1<={1})", monthLoBound, monthHiBound), rubFields, true);
                cdoCreditRubSummary.AddDetailColumn(String.Format("[1](1>={0}1<={1})", monthLoBound, monthHiBound), rubFields, true);
                cdoCreditRubSummary.AddDetailColumn(String.Format(selectDbtYearIn, monthHiBound), rubFields, true);
                cdoCreditRubSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
                cdoCreditRubSummary.AddDetailColumn(String.Format("[10](1>={0}1<={1})", monthLoBound, monthHiBound), rubFields, true);
                cdoCreditRubSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
                cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                // Проценты
                cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditRubSummary.AddDetailColumn(String.Format("[4](1>={0}1<={1})", monthLoBound, monthHiBound), chargeFields, true);
                cdoCreditRubSummary.AddDetailColumn(String.Format("[4](1>={0}1<={1})", monthLoBound, monthHiBound), rubFields, true);
                cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditRubSummary.AddDetailColumn(String.Format("[10](1>={0}1<={1})", monthLoBound, monthHiBound), rubFields, true);
                cdoCreditRubSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumPct);
                cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                // Пени
                cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditRubSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            }

            cdoCreditRubSummary.AddDataColumn(f_S_Creditincome.RefOKV);

            // итоговая таблица по коммерческим кредитам
            cdoCreditRubSummary.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            cdoCreditRubSummary.mainFilter[f_S_Creditincome.RefOKV] = ReportConsts.codeRUBStr;
            var tblOrgSummary = cdoCreditRubSummary.FillData();

            // итоговая таблица по рублевым бюджетным кредитам
            cdoCreditRubSummary.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoCreditRubSummary.mainFilter.Remove(f_S_Creditincome.RefOKV);
            var tblBudRubSummary = cdoCreditRubSummary.FillData();

            var cdoCreditValSummary = new CreditDataObject();
            cdoCreditValSummary.InitObject(scheme);
            cdoCreditValSummary.ignoreZeroCurrencyRows = true;
            cdoCreditValSummary.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoCreditValSummary.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCreditValSummary.mainFilter[f_S_Creditincome.RefOKV] = eurCode.ToString();
            // тут будут суммы на начало и конец года
            cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            // разбивка по месяцам
            for (var m = 1; m < 13; m++)
            {
                var monthLoBound = GetMonthStart(year, m);
                var monthHiBound = GetMonthEnd(year, m);
                // ОД на начало месяца
                cdoCreditValSummary.AddDetailColumn(String.Format(selectDbtYearValEx, monthLoBound), valFields, true);
                cdoCreditValSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
                cdoCreditValSummary.AddDetailColumn(String.Format("[0](1>={0}1<={1})", monthLoBound, monthHiBound), valFields, true);
                cdoCreditValSummary.AddDetailColumn(String.Format("[1](1>={0}1<={1})", monthLoBound, monthHiBound), valFields, true);
                cdoCreditValSummary.AddDetailColumn(String.Format(selectDbtYearValIn, monthHiBound), valFields, true);
                cdoCreditValSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
                cdoCreditValSummary.AddDetailColumn(String.Format("[10](1>={0}1<={1})", monthLoBound, monthHiBound), valFields, true);
                cdoCreditValSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
                cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                // Проценты
                cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditValSummary.AddDetailColumn(String.Format("[4](1>={0}1<={1})", monthLoBound, monthHiBound), chargeFields, true);
                cdoCreditValSummary.AddDetailColumn(String.Format("[4](1>={0}1<={1})", monthLoBound, monthHiBound), valFields, true);
                cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditValSummary.AddDetailColumn(String.Format("[10](1>={0}1<={1})", monthLoBound, monthHiBound), valFields, true);
                cdoCreditValSummary.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumPct);
                cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                // Пени
                cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCreditValSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            }

            // итоговая таблица по евровым бюджетным кредитам
            var tblBudEurSummary = cdoCreditValSummary.FillData();

            tables[4] = CalcYearSummary(tblOrgSummary);
            tables[5] = CalcYearSummary(tblBudRubSummary);
            tables[6] = CalcYearSummary(tblBudEurSummary);

            // ЦБ

            var chargeSumPair = CreateValuePair(t_S_CPFactService.ChargeSum);
            var factDbtCountField = CreateValuePair(t_S_CPFactDebt.Quantity);
            var factCapCountField = CreateValuePair(t_S_CPFactCapital.Quantity);

            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.useSummaryRow = false;
            cdoCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            cdoCap.mainFilter.Add(f_S_Capital.RefOKV, ReportConsts.codeRUBStr);

            if (idValue != ReportConsts.UndefinedKey)
            {
                cdoCap.mainFilter.Add(f_S_Capital.OfficialNumber, idValue);
            }

            // колонки для заголовка
            // 00
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            // 01
            cdoCap.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);
            // 02
            cdoCap.AddDataColumn(f_S_Capital.Underwriter);
            // 03
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge, ReportConsts.ftDateTime);
            // 04
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            // 05
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "-17;+16");
            // колонки для остатка
            // рублевые суммы
            // 06
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+14;-15;*4");
            // 07
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+10;-11");
            // валютные суммы
            // 08
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 9
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебные
            // 10
            cdoCap.AddDetailColumn(String.Format("[7](1<{0})", dateYearStart));
            // 11
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", dateYearStart));
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, "8");
            // 12
            cdoCap.AddCalcColumn(CalcColumnType.cctPercentText);
            // 13
            cdoCap.AddDetailColumn("[6]");
            // 14
            cdoCap.AddDetailColumn(String.Format("[0](1<{0})", dateYearStart), factCapCountField, true);
            // 15
            cdoCap.AddDetailColumn(String.Format("[1](1<{0})", dateYearStart), factDbtCountField, true);
            // 16
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", calcDate), chargeSumPair, false);
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");
            // 17
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", calcDate));
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");
            // 18
            cdoCap.AddDataColumn(f_S_Capital.Sum);
            // 19
            cdoCap.AddDataColumn(f_S_Capital.Purpose);

            cdoCap.sortString = StrSortDown(f_S_Capital.StartDate);

            var tblCapital = cdoCap.FillData();

            detailList = new Dictionary<int, string>
                             {
                                 {00, t_S_CPFactCapital.DateDoc},
                                 {01, t_S_CPFactDebt.DateDischarge},
                                 {03, CombineList(t_S_CPFactService.FactDate, t_S_CPFactService.RefTypeSum, "9", "8")}
                             };

            var capitalCount = SpreadChangesDataYar(
                cdoCap,
                tblJournalCB,
                ReportDocumentType.Capital,
                ref tables[2], 
                tblCapital, 
                detailList, 
                dateYearStart,
                calcDate,
                region);

            var cdoCapSummary = new CapitalDataObject();
            cdoCapSummary.InitObject(scheme);
            cdoCapSummary.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            cdoCapSummary.mainFilter.Add(f_S_Capital.RefOKV, ReportConsts.codeRUBStr);
            // суммы на начало и конец года
            cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCapSummary.AddDataColumn(f_S_Capital.Nominal);

            for (var m = 1; m < 13; m++)
            {
                var monthLoBound = GetMonthStart(year, m);
                var monthHiBound = GetMonthEnd(year, m);
                var serviceColumnIndex = 186 + (m - 1) * 6;

                // ОД
                cdoCapSummary.AddParamColumn(CalcColumnType.cctRelation, String.Format("+{0};*5", serviceColumnIndex + 0));
                cdoCapSummary.AddParamColumn(CalcColumnType.cctRelation, String.Format("+{0};*5", serviceColumnIndex + 4));
                cdoCapSummary.AddParamColumn(CalcColumnType.cctRelation, String.Format("+{0};*5", serviceColumnIndex + 5));
                cdoCapSummary.AddParamColumn(CalcColumnType.cctRelation, String.Format("+{0};*5", serviceColumnIndex + 2));
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                // Проценты
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCapSummary.AddDetailColumn(String.Format("[3](1>={0}1<={1})", monthLoBound, monthHiBound), CreateValuePair(t_S_CPFactService.ChargeSum), true);
                cdoCapSummary.SetColumnCondition(t_S_CPFactService.RefTypeSum, "8");
                cdoCapSummary.AddDetailColumn(String.Format("[3](1>={0}1<={1})", monthLoBound, monthHiBound));
                cdoCapSummary.SetColumnCondition(t_S_CPFactService.RefTypeSum, "8");
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                // пени
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCapSummary.AddCalcColumn(CalcColumnType.cctUndefined);
           }

            var quantityField = CreateValuePair(t_S_CPFactCapital.Quantity);
            for (var m = 1; m < 13; m++)
            {
                var monthLoBound = GetMonthStart(year, m);
                var monthHiBound = GetMonthEnd(year, m);
                // остаток на начало
                cdoCapSummary.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", monthLoBound), quantityField, true);
                cdoCapSummary.AddDetailColumn(String.Format("[3](1<{0})", monthLoBound), chargeSumPair, true);
                cdoCapSummary.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");
                // остаток на конец
                cdoCapSummary.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", monthHiBound), quantityField, true);
                cdoCapSummary.AddDetailColumn(String.Format("[3](1<={0})", monthHiBound), chargeSumPair, true);
                cdoCapSummary.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");

                cdoCapSummary.AddDetailColumn(String.Format("[0](1>={0}1<={1})", monthLoBound, monthHiBound), quantityField, true);
                cdoCapSummary.AddDetailColumn(String.Format("[1](1>={0}1<={1})", monthLoBound, monthHiBound), quantityField, true);
                cdoCapSummary.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");
            }

            var tblCapitalSummary = cdoCapSummary.FillData();
            tables[7] = CalcYearSummary(tblCapitalSummary);

            // гарантии (нереализовано)
            tables[03] = CreateReportCaptionTable(1);
            tables[09] = CreateReportCaptionTable(1);

            // суммы итоговых таблиц, не реализовано
            tables[08] = CreateReportCaptionTable(1);
            tables[10] = CreateReportCaptionTable(1);

            var cdoCapAssum = new CapitalDataObject();
            cdoCapAssum.InitObject(scheme);
            cdoCapAssum.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            cdoCapAssum.mainFilter.Add(f_S_Capital.RefOKV, ReportConsts.codeRUBStr);
            cdoCapAssum.AddParamColumn(CalcColumnType.cctRelation, "-2;+1");
            cdoCapAssum.AddDetailColumn(String.Format("[3](1<{0})", calcDate), chargeSumPair, true);
            cdoCapAssum.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");
            cdoCapAssum.AddDetailColumn(String.Format("[3](1<{0})", calcDate));
            cdoCapAssum.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");
            cdoCapAssum.AddDataColumn(f_S_Capital.id);

            var tblTotalAssum = cdoCapAssum.FillData();

            // заголовочное
            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = realReportDate.ToShortDateString();
            rowCaption[1] = orgCreditCount;
            rowCaption[2] = budCreditCount;
            rowCaption[3] = capitalCount;
            rowCaption[4] = 0;
            rowCaption[5] = Convert.ToDateTime(calcDate).Month;
            rowCaption[6] = GetLastRow(tblTotalAssum)[0];
            rowCaption[7] = calcDate;

            FillSignatureData(rowCaption, 10, reportParams, ReportConsts.ParamExecutor1);

            // это конец
            return tables;
        }

        /// <summary>
        /// Ярославль область - График погашения долговых обязательств
        /// </summary>
        public DataTable[] GetGraphDebtYaroslavlData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[2];
            tables[0] = CreateReportCaptionTable(12, 19);
            var reportDate = reportParams[ReportConsts.ParamEndDate];
            var calcDate = Convert.ToDateTime(reportDate);
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var selectedMonth = calcDate.Month;
            var exchangeRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var tblData = tables[0];
            var sumField = CreateValuePair(ReportConsts.SumField);
            var eurCode = GetEuroCode();
            var variantID = reportParams[ReportConsts.ParamVariantID];
            var variantFilter = Combine(variantID, ReportConsts.ActiveVariantID);

            var factMonthCount = 0;
            // Кредиты
            for (var i = 1; i < 13; i++)
            {
                var curMonthIndex = i - 1;
                var useFact = year < DateTime.Now.Year || year == DateTime.Now.Year && i < selectedMonth;
                
                if (useFact)
                {
                    factMonthCount++;
                }

                var monthBegin = GetMonthStart(year, i);
                var monthEnd = GetMonthEnd(year, i);

                var orgDetail = useFact ? 1 : 2;
                var budDetail = useFact ? 1 : 2;
                var capDetail = useFact ? 1 : 6;
                var orgDateField = useFact ? t_S_FactDebtCI.FactDate : t_S_PlanDebtCI.EndDate;
                var budDateField = useFact ? t_S_FactDebtCI.FactDate : t_S_PlanDebtCI.EndDate;
                var capDateField = useFact ? t_S_CPFactDebt.DateDischarge : t_S_CPPlanDebt.EndDate;

                var orgAttrDetail = useFact ? 0 : 3;
                var budAttrDetail = useFact ? 0 : 3;
                var capAttrDetail = useFact ? 0 : 5;
                var orgAttrDateField = useFact ? t_S_FactAttractCI.FactDate : t_S_PlanAttractCI.StartDate;
                var budAttrDateField = useFact ? t_S_FactAttractCI.FactDate : t_S_PlanAttractCI.StartDate;
                var capAttrDateField = useFact ? t_S_CPFactCapital.DateDoc : t_S_CPPlanCapital.StartDate;

                // Коммерческие кредиты
                var cdoCI = new CreditDataObject();
                cdoCI.InitObject(scheme);
                cdoCI.reportParams[ReportConsts.ParamHiDate] = reportDate;
                cdoCI.fixedExchangeRate.Add(eurCode, exchangeRate);
                cdoCI.mainFilter[f_S_Creditincome.RefVariant] = variantFilter;
                cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
                cdoCI.ignoreZeroCurrencyRows = true;
                cdoCI.AddDetailColumn(String.Format("[1](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCI.AddDetailColumn(String.Format("[2](1>={0}1<={1})", monthBegin, monthEnd));
                cdoCI.AddDetailColumn(String.Format("[4](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCI.AddDetailColumn(String.Format("[5](2>={0}2<={1})", monthBegin, monthEnd));
                cdoCI.AddDetailColumn(String.Format("[0](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCI.AddDetailColumn(String.Format("[3](0>={0}0<={1})", monthBegin, monthEnd));
                cdoCI.AddCalcColumn(CalcColumnType.cctNearestPercent);
                cdoCI.AddCalcColumn(CalcColumnType.cctOrganization);
                var tblOrgCredit = cdoCI.FillData();
                var rowOrgCreditSummary = GetLastRow(tblOrgCredit);
                var rowDebtOrgSum = tblData.Rows[00];
                var rowSrvcOrgSum = tblData.Rows[09];
                var rowAttrOrgSum = tblData.Rows[14];

                var orgCreditInfo = CombineDetailTextYar(
                    cdoCI, 
                    tblOrgCredit, 
                    ReportDocumentType.CreditOrg, 
                    false,
                    orgDetail, 
                    orgDateField, 
                    monthBegin, 
                    monthEnd,
                    exchangeRate);

                tblData.Rows[01][curMonthIndex] = orgCreditInfo[0];
                tblData.Rows[02][curMonthIndex] = orgCreditInfo[1];

                var orgCreditAttrInfo = CombineDetailTextYar(
                    cdoCI,
                    tblOrgCredit,
                    ReportDocumentType.CreditOrg,
                    false,
                    orgAttrDetail,
                    orgAttrDateField,
                    monthBegin,
                    monthEnd,
                    exchangeRate);

                tblData.Rows[15][curMonthIndex] = orgCreditAttrInfo[1];

                // Бюджетные кредиты
                cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
                var tblBudCredit = cdoCI.FillData();
                var rowBudCreditSummary = GetLastRow(tblBudCredit);
                var rowDebtBudSum = tblData.Rows[03];
                var rowSrvcBudSum = tblData.Rows[10];
                var rowAttrBudSum = tblData.Rows[16];

                var budCreditInfo = CombineDetailTextYar(
                    cdoCI, 
                    tblBudCredit, 
                    ReportDocumentType.CreditBud, 
                    false,
                    budDetail, 
                    budDateField,
                    monthBegin,
                    monthEnd,
                    exchangeRate);

                var budCreditAttrInfo = CombineDetailTextYar(
                    cdoCI,
                    tblBudCredit,
                    ReportDocumentType.CreditBud,
                    false,
                    budAttrDetail,
                    budAttrDateField,
                    monthBegin,
                    monthEnd,
                    exchangeRate);

                // ЦБ 
                var cdoCap = new CapitalDataObject();
                cdoCap.InitObject(scheme);
                cdoCap.reportParams[ReportConsts.ParamHiDate] = reportDate;
                cdoCap.fixedExchangeRate.Add(eurCode, exchangeRate);
                cdoCap.mainFilter[f_S_Capital.RefVariant] = variantFilter;
                cdoCap.AddDetailColumn(String.Format("[1](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+10;-11");
                cdoCap.AddDetailColumn(String.Format("[3](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCap.AddDetailColumn(String.Format("[7](1>={0}1<={1})", monthBegin, monthEnd));
                cdoCap.AddDetailColumn(String.Format("[0](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCap.AddDetailColumn(String.Format("[5](0>={0}0<={1})", monthBegin, monthEnd));
                cdoCap.AddCalcColumn(CalcColumnType.cctNearestPercent);
                cdoCap.AddCalcColumn(CalcColumnType.cctOrganization);
                cdoCap.AddDetailColumn(String.Format("[13](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCap.AddDetailColumn(String.Format("[14](1>={0}1<={1})", monthBegin, monthEnd), sumField, true);
                cdoCap.AddDetailColumn(String.Format("[6](1>={0}1<={1})", monthBegin, monthEnd));
                cdoCap.AddDetailColumn(String.Format("[3](1<{0})", reportDate), CreateValuePair(t_S_CPFactService.ChargeSum), true);
                cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, "9");
                var tblCap = cdoCap.FillData();

                foreach (DataRow rowCap in tblCap.Rows)
                {
                    var dbtSum = GetNumber(rowCap[10]);
                    rowCap[1] = 0;
                    
                    if (dbtSum != 0)
                    {
                        rowCap[1] = dbtSum;
                        
                        if (year <= DateTime.Now.Year)
                        {
                            rowCap[1] = dbtSum - GetNumber(rowCap[11]);
                            rowCap[3] = GetNumber(rowCap[3]) + GetNumber(rowCap[11]);
                        }
                    }
                }

                tblCap = cdoCap.RecalcSummary(tblCap);

                var rowCapSummary = GetLastRow(tblCap);
                var rowDebtCapSum = tblData.Rows[06];
                var rowSrvcCapSum = tblData.Rows[11];
                var rowAttrCapSum = tblData.Rows[12];

                for (var m = 0; m < 6; m++)
                {
                    CorrectThousandSumValue(tblCap, m);
                    CorrectThousandSumValue(tblBudCredit, m);
                    CorrectThousandSumValue(tblOrgCredit, m);
                }

                CorrectThousandSumValue(tblCap, 8);
                CorrectThousandSumValue(tblCap, 9);

                if (useFact)
                {
                    rowDebtOrgSum[curMonthIndex] = rowOrgCreditSummary[0];
                    rowSrvcOrgSum[curMonthIndex] = rowOrgCreditSummary[2];
                    rowAttrOrgSum[curMonthIndex] = rowOrgCreditSummary[4];

                    rowDebtBudSum[curMonthIndex] = rowBudCreditSummary[0];
                    rowSrvcBudSum[curMonthIndex] = rowBudCreditSummary[2];
                    rowAttrBudSum[curMonthIndex] = rowBudCreditSummary[4];

                    rowDebtCapSum[curMonthIndex] = rowCapSummary[0];
                    rowSrvcCapSum[curMonthIndex] = rowCapSummary[2];
                    rowAttrCapSum[curMonthIndex] = rowCapSummary[4];

                    tblData.Rows[18][curMonthIndex] = rowCapSummary[8];
                }
                else
                {
                    rowDebtOrgSum[curMonthIndex] = rowOrgCreditSummary[1];
                    rowSrvcOrgSum[curMonthIndex] = rowOrgCreditSummary[3];
                    rowAttrOrgSum[curMonthIndex] = rowOrgCreditSummary[5];

                    rowDebtBudSum[curMonthIndex] = rowBudCreditSummary[1];
                    rowSrvcBudSum[curMonthIndex] = rowBudCreditSummary[3];
                    rowAttrBudSum[curMonthIndex] = rowBudCreditSummary[5];

                    rowDebtCapSum[curMonthIndex] = rowCapSummary[1];
                    rowSrvcCapSum[curMonthIndex] = rowCapSummary[3];
                    rowAttrCapSum[curMonthIndex] = rowCapSummary[5];

                    tblData.Rows[18][curMonthIndex] = rowCapSummary[9];
                }

                if (GetNumber(rowDebtBudSum[curMonthIndex]) != 0)
                {
                    tblData.Rows[04][curMonthIndex] = budCreditInfo[0];
                    tblData.Rows[05][curMonthIndex] = budCreditInfo[1];
                }

                if (GetNumber(rowAttrBudSum[curMonthIndex]) != 0)
                {
                    tblData.Rows[17][curMonthIndex] = budCreditAttrInfo[1];
                }

                var capitalInfo = CombineDetailTextYar(
                    cdoCap, 
                    tblCap,
                    ReportDocumentType.Capital,
                    true,
                    capDetail,
                    capDateField, 
                    monthBegin, 
                    monthEnd,
                    exchangeRate);

                tblData.Rows[07][curMonthIndex] = capitalInfo[0];
                tblData.Rows[08][curMonthIndex] = capitalInfo[1];

                var capitalAttrInfo = CombineDetailTextYar(
                    cdoCap,
                    tblCap,
                    ReportDocumentType.Capital,
                    false,
                    capAttrDetail,
                    capAttrDateField,
                    monthBegin,
                    monthEnd,
                    exchangeRate);

                tblData.Rows[13][curMonthIndex] = capitalAttrInfo[1];
            }

            // заголовочное
            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = reportParams[ReportConsts.ParamYear];
            rowCaption[1] = GetMonthStart(year, selectedMonth);
            rowCaption[2] = DateTime.Now.ToShortDateString();
            rowCaption[3] = factMonthCount + 1;

            var variantCaption = String.Empty;
            var variantList = variantID.Split(',');

            variantCaption = variantList.Select(GetVariantInfo)
                .Where(rowVariant => rowVariant != null).Aggregate(variantCaption, (current, rowVariant) => 
                    Combine(current, rowVariant[d_Variant_Borrow.Name].ToString()));

            rowCaption[4] = variantCaption.Trim(',');
            rowCaption[5] = calcDate.ToShortDateString();
            rowCaption[6] = reportParams[ReportConsts.ParamDigitNumber];

            return tables;
        }

        /// <summary>
        /// Московская область - Сведения о договоре (расп.58)
        /// </summary>
        public DataTable[] GetMOContractInfoForm58Data(Dictionary<string, string> reportParams)
        {
            const string fictiveID = ReportConsts.UndefinedKey;
            var tablesResult = new DataTable[11];
            var regNumValue = reportParams[ReportConsts.ParamRegNum];
            var regNumFilter = GetMOSelectedRegNumFilter(regNumValue);
            var exchangeRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var sumFieldRub = CreateValuePair(ReportConsts.SumField);
            var sumFieldUsd = CreateValuePair(ReportConsts.CurrencySumField);
            // Гарантии
            const string formulaGrntFull = "---+[3](1<{0})[5](1<{0})[1](1<{1})[4](1<{1})[2](1<{1})";
            const string formulaGrntRest = "---+[3](1<{0})[5](1<{0})[1](1<{0})[4](1<{0})[2](1<{0})";
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            cdoGrnt.mainFilter[f_S_Guarantissued.Num] = regNumFilter;
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Occasion);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined); // оглавление
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DebtSum);
            cdoGrnt.AddDetailColumn(String.Format("[{0}](1<{1})", t_S_PlanServicePrGrnt.key, maxDate));
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalPercent);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV); // 11
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctRegress2);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDoc);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDemand);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DatePerformance);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalStartDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum);
            cdoGrnt.AddDetailColumn(String.Format(formulaGrntFull, maxDate, calcDate), sumFieldRub, true);
            cdoGrnt.AddDetailColumn(String.Format(formulaGrntRest, calcDate), sumFieldRub, true);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined); // оглавление
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined); // оглавление
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined); // оглавление
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебное
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalNum); // 32
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalStartDate); // 33
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalDoc); // 34
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum); // 35
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum); // 36
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum); // 37
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalCurrencySum); // 38
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, "3"); // 39
            cdoGrnt.AddDetailColumn(String.Format(formulaGrntFull, maxDate, calcDate), sumFieldUsd, true);
            cdoGrnt.AddDetailColumn(String.Format(formulaGrntRest, maxDate), sumFieldUsd, true);
            tablesResult[0] = cdoGrnt.FillData();

            // конвертация сумм по валюте договора и заполненности планов
            foreach (DataRow rowData in tablesResult[0].Rows)
            {
                var refOKV = Convert.ToInt32(rowData[11]);
                var contractSum = GetNumber(rowData[37]);
                
                if (refOKV != ReportConsts.codeRUB)
                {
                    contractSum = GetNumber(rowData[38]);
                }
                
                var emptyPlanDebt = GetNumber(rowData[39]) == 0;

                if (refOKV != ReportConsts.codeRUB)
                {
                    rowData[21] = rowData[40];
                    rowData[22] = rowData[41];
                }

                if (emptyPlanDebt)
                {
                    rowData[21] = GetNumber(rowData[21]) + contractSum;
                    rowData[22] = GetNumber(rowData[22]) + contractSum;
                }

                if (refOKV != ReportConsts.codeRUB)
                {
                    rowData[21] = exchangeRate * GetNumber(rowData[21]);
                    rowData[22] = exchangeRate * GetNumber(rowData[22]);
                }
            }

            var currentID = fictiveID;
            var rowGrnt = GetLastRow(tablesResult[0]);

            if (rowGrnt != null)
            {
                currentID = Convert.ToString(rowGrnt[f_S_Guarantissued.id]);
            }

            // Сведения о погашении основного долга
            var objGrntFactDebt = new GrntFactDebtPrDataObject();
            objGrntFactDebt.InitObject(scheme);
            objGrntFactDebt.useSummaryRow = false;
            objGrntFactDebt.mainFilter[t_S_FactDebtPrGrnt.RefGrnt] = currentID;
            objGrntFactDebt.AddDataColumn(t_S_FactDebtPrGrnt.FactDate, ReportConsts.ftDateTime);
            objGrntFactDebt.AddDataColumn(t_S_FactDebtPrGrnt.Sum);
            objGrntFactDebt.AddCalcColumn(CalcColumnType.cctUndefined);
            objGrntFactDebt.sortString = StrSortUp(t_S_FactDebtPrGrnt.FactDate);
            var tableGrntFactDebtPr = objGrntFactDebt.FillData();

            foreach (DataRow rowData in tableGrntFactDebtPr.Rows)
            {
                if (rowGrnt != null)
                {
                    rowData[2] = rowGrnt[6];
                }
            }

            var objGrntFactAttr = new GrntFactAttractDataObject();
            objGrntFactAttr.InitObject(scheme);
            objGrntFactAttr.useSummaryRow = false;
            objGrntFactAttr.mainFilter[t_S_FactAttractGrnt.RefTypSum] = "1";
            objGrntFactAttr.mainFilter[t_S_FactAttractGrnt.RefGrnt] = currentID;
            objGrntFactAttr.AddDataColumn(t_S_FactAttractGrnt.FactDate, ReportConsts.ftDateTime);
            objGrntFactAttr.AddDataColumn(t_S_FactAttractGrnt.Sum);
            objGrntFactAttr.AddCalcColumn(CalcColumnType.cctUndefined);
            objGrntFactAttr.sortString = StrSortUp(t_S_FactAttractGrnt.FactDate);
            var tableGrntFactAttrDbt = objGrntFactAttr.FillData();

            foreach (DataRow rowData in tableGrntFactAttrDbt.Rows)
            {
                rowData[2] = "Министерство финансов Московской области";
            }

            tablesResult[1] = DataTableUtils.MergeDataSet(tableGrntFactDebtPr, tableGrntFactAttrDbt);

            // Сведения о погашении процентов
            var objGrntFactPercent = new GrntFactPercentPrDataObject();
            objGrntFactPercent.InitObject(scheme);
            objGrntFactPercent.useSummaryRow = false;
            objGrntFactPercent.mainFilter[t_S_FactPercentPrGrnt.RefGrnt] = currentID;
            objGrntFactPercent.AddDataColumn(t_S_FactPercentPrGrnt.FactDate, ReportConsts.ftDateTime);
            objGrntFactPercent.AddDataColumn(t_S_FactPercentPrGrnt.Sum);
            objGrntFactPercent.AddCalcColumn(CalcColumnType.cctUndefined);
            objGrntFactPercent.sortString = StrSortUp(t_S_FactPercentPrGrnt.FactDate);
            var tableGrntFactPercentPr = objGrntFactPercent.FillData();

            foreach (DataRow rowData in tableGrntFactPercentPr.Rows)
            {
                if (rowGrnt != null)
                {
                    rowData[2] = rowGrnt[6];
                }
            }

            objGrntFactAttr.mainFilter[t_S_FactAttractGrnt.RefTypSum] = "2";
            DataTable tableGrntFactAttrPct = objGrntFactAttr.FillData();

            foreach (DataRow rowData in tableGrntFactAttrPct.Rows)
            {
                rowData[2] = "Министерство финансов Московской области";
            }

            tablesResult[2] = DataTableUtils.MergeDataSet(tableGrntFactPercentPr, tableGrntFactAttrPct);

            // изменения по гарантиям
            var objGrntAlter = new CommonDataObject();
            objGrntAlter.InitObject(scheme);
            objGrntAlter.ObjectKey = t_S_AlterationGrnt.internalKey;
            objGrntAlter.useSummaryRow = false;
            objGrntAlter.mainFilter[t_S_AlterationGrnt.RefGrnt] = currentID;
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.Charge);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.DocDate);
            objGrntAlter.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                t_S_AlterationGrnt.RefKindDocs,
                fx_S_KindDocs.internalKey,
                fx_S_KindDocs.Name,
                String.Empty);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.Num);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.ChargeDate);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.Sum);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.EndDate);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.CreditPercent);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.IntRepayUpTo);
            tablesResult[3] = objGrntAlter.FillData();

            // Ценные бумаги
            var formulaCapPlanDbt = String.Format("[6](1<{0})", maxDate);
            var formulaCapFactDbt = String.Format("[0](1<{0})", maxDate);
            var formulaCapFactSvc = String.Format("[3](1<{0})", maxDate);
            var formulaCapExtra = String.Format("[12](1<{0})", maxDate);
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter[f_S_Capital.OfficialNumber] = regNumFilter;
            cdoCap.useSummaryRow = false;
            cdoCap.AddDataColumn(f_S_Capital.RegNumber);
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            cdoCap.AddDataColumn(f_S_Capital.Name);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapKind);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapForm);
            cdoCap.AddDataColumn(f_S_Capital.NameNPA);
            cdoCap.AddCalcColumn(CalcColumnType.cctOKVName);
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            cdoCap.AddDataColumn(f_S_Capital.Count);
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+8;+52");
            cdoCap.AddDataColumn(f_S_Capital.StartDate);
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined); // заголовок
            cdoCap.AddDetailTextColumn(formulaCapPlanDbt, cdoCap.ParamFieldList, CreateValuePair(t_S_CPPlanDebt.PercentNom));
            cdoCap.AddDetailTextColumn(formulaCapPlanDbt, cdoCap.ParamOnlyValues, String.Empty);
            cdoCap.AddDetailTextColumn(formulaCapPlanDbt, cdoCap.ParamOnlyDates, String.Empty);
            cdoCap.AddDetailColumn(String.Format("[7](1<{0})", maxDate));
            cdoCap.AddDataColumn(f_S_Capital.Owner);
            cdoCap.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoCap.AddDataColumn(f_S_Capital.Depository);
            cdoCap.AddDataColumn(f_S_Capital.Trade);
            cdoCap.AddDataColumn(f_S_Capital.Underwriter);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined); // 23 заголовок
            cdoCap.AddDetailTextColumn(formulaCapFactDbt, cdoCap.ParamOnlyDates, String.Empty);
            cdoCap.AddDetailTextColumn(formulaCapFactDbt, cdoCap.ParamFieldList, CreateValuePair(t_S_CPFactCapital.Quantity));
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+8;*7");
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined); // заголовок
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined); // заголовок
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined); // заголовок
            cdoCap.AddDetailTextColumn(formulaCapFactSvc, cdoCap.ParamOnlyDates, String.Empty);
            cdoCap.AddDetailTextColumn(formulaCapFactSvc, cdoCap.ParamOnlyValues, String.Empty);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined); // заголовок
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            cdoCap.AddDetailColumn(String.Format("-[5](1<{0})[1](1<{0})", calcDate));
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", calcDate));
            cdoCap.AddDetailColumn(String.Format("-[7](1<{0})[3](1<{1})", maxDate, calcDate));
            // 48 служебные
            cdoCap.AddParamColumn(CalcColumnType.cctRecordCount, "6");
            cdoCap.AddParamColumn(CalcColumnType.cctRecordCount, "12");
            cdoCap.AddDetailTextColumn(formulaCapExtra, cdoCap.ParamOnlyDates, String.Empty);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddDetailColumn(formulaCapExtra, cdoCap.ParamFieldList, CreateValuePair(t_S_CPExtraIssue.Count));
            cdoCap.AddCalcColumn(CalcColumnType.cctCapCoupon);
            tablesResult[4] = cdoCap.FillData();

            foreach (DataRow rowData in tablesResult[4].Rows)
            {
                if (Convert.ToInt32(rowData[49]) > 0)
                {
                    rowData[05] = String.Format("{0} {1}", rowData[05], rowData[51]);
                    rowData[10] = String.Format("{0} {1}", rowData[10], rowData[50]);
                }
            }

            currentID = fictiveID;
            var rowCap = GetLastRow(tablesResult[4]);
            if (rowCap != null)
            {
                currentID = Convert.ToString(rowCap[f_S_Capital.id]);
            }

            // Сведения о купонном доходе по ценным бумагам
            var objCapPlanSvc = new CommonDataObject();
            objCapPlanSvc.InitObject(scheme);
            objCapPlanSvc.ObjectKey = t_S_CPPlanService.internalKey;
            objCapPlanSvc.useSummaryRow = false;
            objCapPlanSvc.mainFilter[t_S_CPPlanService.RefCap] = currentID;
            objCapPlanSvc.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapPlanSvc.AddDataColumn(t_S_CPPlanService.Income);
            objCapPlanSvc.AddDataColumn(t_S_CPPlanService.StartDate);
            objCapPlanSvc.AddDataColumn(t_S_CPPlanService.EndDate);
            tablesResult[5] = objCapPlanSvc.FillData();

            foreach (DataRow rowData in tablesResult[5].Rows)
            {
                if (rowCap != null)
                {
                    rowData[0] = rowCap[54];
                }
            }

            // Кредиты
            var formulaCIFactAttr = String.Format("[0](1<{0})", maxDate);
            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            cdoCI.useSummaryRow = false;
            cdoCI.mainFilter[f_S_Creditincome.Num] = regNumFilter;
            cdoCI.AddDataColumn(f_S_Creditincome.RegNum);
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCI.AddDataColumn(f_S_Creditincome.Num);
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate);
            cdoCI.AddDataColumn(f_S_Creditincome.Occasion);
            cdoCI.AddDataColumn(f_S_Creditincome.Purpose);
            cdoCI.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoCI.AddCalcColumn(CalcColumnType.cctOKVName);
            cdoCI.AddDataColumn(f_S_Creditincome.StartDate);
            cdoCI.AddDataColumn(f_S_Creditincome.EndDate);
            cdoCI.AddDataColumn(f_S_Creditincome.Sum);
            cdoCI.AddDataColumn(f_S_Creditincome.CreditPercent);
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditPeriodRate);
            cdoCI.AddDetailTextColumn(String.Format("[5](2<{0})", maxDate), cdoCI.ParamOnlyDates, String.Empty);
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined); // заголовок
            cdoCI.AddDetailTextColumn(formulaCIFactAttr, cdoCap.ParamOnlyDates, String.Empty);
            cdoCI.AddDetailTextColumn(formulaCIFactAttr, cdoCap.ParamOnlyValues, String.Empty);
            cdoCI.AddDataColumn(f_S_Creditincome.Note);
            cdoCI.AddDetailColumn(String.Format("+-[0](1<{0})[1](1<{0})[10](0<{0})", calcDate));
            cdoCI.AddDetailColumn(String.Format("+-[3](1<{0})[1](1<{0})[10](0<{0})", calcDate));
            tablesResult[6] = cdoCI.FillData();

            currentID = fictiveID;
            var rowCI = GetLastRow(tablesResult[6]);

            if (rowCI != null)
            {
                currentID = Convert.ToString(rowCI[f_S_Creditincome.id]);
            }

            // Сведения о процентных платежах по кредиту
            var objFactPctCI = new CommonDataObject();
            objFactPctCI.InitObject(scheme);
            objFactPctCI.ObjectKey = t_S_FactPercentCI.internalKey;
            objFactPctCI.useSummaryRow = false;
            objFactPctCI.mainFilter[t_S_FactPercentCI.RefCreditInc] = currentID;
            objFactPctCI.AddDataColumn(t_S_FactPercentCI.FactDate);
            objFactPctCI.AddDataColumn(t_S_FactPercentCI.Sum);
            tablesResult[7] = objFactPctCI.FillData();

            // Сведения о погашении кредита
            var objFactBdtCI = new CommonDataObject();
            objFactBdtCI.InitObject(scheme);
            objFactBdtCI.ObjectKey = t_S_FactDebtCI.internalKey;
            objFactBdtCI.useSummaryRow = false;
            objFactBdtCI.mainFilter[t_S_FactDebtCI.RefCreditInc] = currentID;
            objFactBdtCI.AddDataColumn(t_S_FactDebtCI.FactDate);
            objFactBdtCI.AddDataColumn(t_S_FactDebtCI.Sum);
            tablesResult[8] = objFactBdtCI.FillData();

            // Изменение
            var objCreditAlter = new CommonDataObject();
            objCreditAlter.InitObject(scheme);
            objCreditAlter.ObjectKey = t_S_AlterationCl.internalKey;
            objCreditAlter.useSummaryRow = false;
            objCreditAlter.mainFilter[t_S_AlterationCl.RefCreditInc] = currentID;
            objCreditAlter.AddCalcColumn(CalcColumnType.cctUndefined);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.Num);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.DocDate);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.Sum);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.CreditPercent);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.EndDate);
            tablesResult[9] = objCreditAlter.FillData();

            foreach (DataRow rowData in tablesResult[9].Rows)
            {
                rowData[0] = "Кредит";
            }

            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = regNumValue;
            rowCaption[1] = calcDate;
            rowCaption[2] = exchangeRate;
            return tablesResult;
        }

        /// <summary>
        /// Московская область - Сведения о договоре (долговые обязательства)
        /// </summary>
        public DataTable[] GetMOContractInfoData(Dictionary<string, string> reportParams)
        {
            const string fictiveID = ReportConsts.UndefinedKey;
            var tablesResult = new DataTable[8];
            var regNumValue = reportParams[ReportConsts.ParamRegNum];
            var regNumFilter = GetMOSelectedRegNumFilter(regNumValue);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            cdoGrnt.mainFilter[f_S_Guarantissued.Num] = regNumFilter;
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegDate);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctContractType);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCollateralType);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalDoc);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalNum);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalStartDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVFullName);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DebtSum);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalPercent);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalPeriod);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.FactDate);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Note);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            cdoGrnt.SetColumnNameParam(TempFieldNames.OKVShortName);
            tablesResult[0] = cdoGrnt.FillData();

            var currentID = fictiveID;
            var rowGrnt = GetLastRow(tablesResult[0]);

            if (rowGrnt != null)
            {
                currentID = Convert.ToString(rowGrnt[f_S_Guarantissued.id]);
                rowGrnt[0] = GetMORegNumDigits(rowGrnt[0]);
            }

            // изменения по гарантиям
            var objGrntAlter = new CommonDataObject();
            objGrntAlter.InitObject(scheme);
            objGrntAlter.ObjectKey = t_S_AlterationGrnt.internalKey;
            objGrntAlter.useSummaryRow = false;
            objGrntAlter.mainFilter[t_S_AlterationGrnt.RefGrnt] = currentID;
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.Charge);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.DocDate, ReportConsts.ftDateTime);
            objGrntAlter.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                t_S_AlterationGrnt.RefKindDocs,
                fx_S_KindDocs.internalKey,
                fx_S_KindDocs.Name,
                String.Empty);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.Num);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.ChargeDate, ReportConsts.ftDateTime);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.Sum);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.EndDate);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.CreditPercent);
            objGrntAlter.AddDataColumn(t_S_AlterationGrnt.IntRepayUpTo);
            objGrntAlter.sortString = StrSortUp(t_S_AlterationGrnt.DocDate);
            tablesResult[1] = objGrntAlter.FillData();
            objGrntAlter.sortString = StrSortUp(t_S_AlterationGrnt.ChargeDate);
            var tblAlter = objGrntAlter.FillData();

            foreach (DataRow rowAlter in tablesResult[1].Rows)
            {
                if (rowAlter[t_S_AlterationGrnt.IntRepayUpTo] != DBNull.Value)
                {
                    rowAlter[t_S_AlterationGrnt.IntRepayUpTo] =
                        String.Format("{0} числа", rowAlter[t_S_AlterationGrnt.IntRepayUpTo]);
                }
            }

            if (rowGrnt != null)
            {
                var refOkv = Convert.ToInt32(rowGrnt[TempFieldNames.OKVShortName]);

                if (refOkv != ReportConsts.codeRUB)
                {
                    var rowAlter = GetFirstRow(tblAlter);

                    if (rowAlter != null)
                    {
                        rowGrnt[f_S_Guarantissued.Sum] = rowAlter[t_S_AlterationGrnt.Sum];
                    }
                }
            }

            // ЦБ
            var paramValue = reportParams[ReportConsts.ParamRegNum];
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter[f_S_Capital.OfficialNumber] = regNumFilter;
            cdoCap.useSummaryRow = false;
            cdoCap.AddDataColumn(f_S_Capital.RegNumber);
            cdoCap.AddDataColumn(f_S_Capital.RegDate);
            cdoCap.AddDataColumn(f_S_Capital.RegNumber);
            cdoCap.AddDataColumn(f_S_Capital.Name);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapForm);
            cdoCap.AddCalcColumn(CalcColumnType.cctOKVFullName);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapKind);
            cdoCap.AddDataColumn(f_S_Capital.SeriesCapital);
            cdoCap.AddDataColumn(f_S_Capital.RegEmissionDate);
            cdoCap.AddDataColumn(f_S_Capital.RegEmissnNumber);
            cdoCap.AddDataColumn(f_S_Capital.Owner);
            cdoCap.AddDataColumn(f_S_Capital.GenAgent);
            cdoCap.AddDataColumn(f_S_Capital.Depository);
            cdoCap.AddDataColumn(f_S_Capital.Trade);
            cdoCap.AddDataColumn(f_S_Capital.NameNPA);
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            cdoCap.AddDataColumn(f_S_Capital.StartDate);
            cdoCap.AddDataColumn(f_S_Capital.Count);
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            cdoCap.AddDataColumn(f_S_Capital.Sum);
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
            cdoCap.AddCalcColumn(CalcColumnType.cctPercentGroupText);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapCouponPeriod);
            cdoCap.AddDataColumn(f_S_Capital.FactDate);
            cdoCap.AddDataColumn(f_S_Capital.Note);
            tablesResult[2] = cdoCap.FillData();
            
            currentID = fictiveID;
            var rowCap = GetLastRow(tablesResult[2]);
            var capCount = 0;
            var capCountCurrent = 0;
            var completeEmission = false;

            if (rowCap != null)
            {
                rowCap[0] = GetMORegNumDigits(rowCap[0]);
                currentID = Convert.ToString(rowCap[f_S_Capital.id]);
                capCount = Convert.ToInt32(rowCap[f_S_Capital.Count]);
            }

            // Сведения о размещении 
            var objCapFact = new CommonDataObject();
            objCapFact.InitObject(scheme);
            objCapFact.ObjectKey = t_S_CPFactCapital.internalKey;
            objCapFact.useSummaryRow = false;
            objCapFact.mainFilter[t_S_CPFactCapital.RefCap] = currentID;
            objCapFact.mainFilter[t_S_CPFactCapital.Secondary] = "=0";
            objCapFact.AddDataColumn(t_S_CPFactCapital.RegDate);
            objCapFact.AddDataColumn(t_S_CPFactCapital.DateDoc);
            objCapFact.AddDataColumn(t_S_CPFactCapital.Quantity);
            objCapFact.AddDataColumn(t_S_CPFactCapital.Price);
            objCapFact.AddDataColumn(t_S_CPFactCapital.Sum);
            objCapFact.AddDataColumn(t_S_CPFactCapital.Secondary);
            tablesResult[3] = objCapFact.FillData();

            foreach (DataRow rowFactCapital in tablesResult[3].Rows)
            {
                var isSecondary = GetBoolValue(rowFactCapital[t_S_CPFactCapital.Secondary]);

                if (!completeEmission && !isSecondary)
                {
                    capCountCurrent += Convert.ToInt32(rowFactCapital[t_S_CPFactCapital.Quantity]);
                    completeEmission = capCountCurrent >= capCount;
                }

                if (!completeEmission)
                {
                    rowFactCapital[t_S_CPFactCapital.DateDoc] = DBNull.Value;
                }
            }

            // Дополнительные выпуски
            var objCapExtra = new CommonDataObject();
            objCapExtra.InitObject(scheme);
            objCapExtra.ObjectKey = t_S_CPExtraIssue.internalKey;
            objCapExtra.useSummaryRow = false;
            objCapExtra.mainFilter[t_S_CPExtraIssue.RefCap] = currentID;

            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddDataColumn(t_S_CPExtraIssue.Occasion);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddDataColumn(t_S_CPExtraIssue.ExtraIssueDate);
            objCapExtra.AddDataColumn(t_S_CPExtraIssue.Count);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            objCapExtra.AddCalcColumn(CalcColumnType.cctUndefined);
            tablesResult[4] = objCapExtra.FillData();
            
            // Кредиты
            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            cdoCI.useSummaryRow = false;
            cdoCI.reportParams[ReportConsts.ParamHiDate] = maxDate;
            cdoCI.mainFilter[f_S_Creditincome.Num] = regNumFilter;
            cdoCI.AddDataColumn(f_S_Creditincome.RegNum);
            cdoCI.AddDataColumn(f_S_Creditincome.RegDate);
            cdoCI.AddDataColumn(f_S_Creditincome.RegNum);
            cdoCI.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCI.AddCalcColumn(CalcColumnType.cctContractType);
            cdoCI.AddDataColumn(f_S_Creditincome.Num);
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate);
            cdoCI.AddCalcColumn(CalcColumnType.cctOKVFullName);
            cdoCI.AddDataColumn(f_S_Creditincome.Sum);
            cdoCI.AddDataColumn(f_S_Creditincome.EndDate);
            cdoCI.AddDataColumn(f_S_Creditincome.CreditPercent);
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditPeriodRate);
            cdoCI.AddDataColumn(f_S_Creditincome.IntRepayUpTo);
            cdoCI.AddDataColumn(f_S_Creditincome.Commission);
            cdoCI.AddDataColumn(f_S_Creditincome.FactDate);
            cdoCI.AddDataColumn(f_S_Creditincome.Note);
            cdoCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", maxDate));
            cdoCI.AddParamColumn(CalcColumnType.cctNearestDown, t_S_FactDebtCI.key, t_S_FactDebtCI.FactDate); 

            tablesResult[5] = cdoCI.FillData();

            currentID = fictiveID;
            var rowCI = GetLastRow(tablesResult[5]);

            if (rowCI != null)
            {
                rowCI[0] = GetMORegNumDigits(rowCI[0]);
                currentID = Convert.ToString(rowCI[f_S_Creditincome.id]);

                if (rowCI[f_S_Creditincome.Commission] == DBNull.Value)
                {
                    rowCI[f_S_Creditincome.Commission] = 0;
                }

                if (Math.Abs(GetNumber(rowCI[18])) == 0 && rowCI[19] != DBNull.Value)
                {
                    rowCI[16] = Convert.ToDateTime(rowCI[19]).ToShortDateString();
                }

                if (rowCI[f_S_Creditincome.IntRepayUpTo] != DBNull.Value)
                {
                    rowCI[f_S_Creditincome.IntRepayUpTo] = String.Format("{0} числа", rowCI[f_S_Creditincome.IntRepayUpTo]);
                }
            }

            // Изменение
            var objCreditAlter = new CommonDataObject();
            objCreditAlter.InitObject(scheme);
            objCreditAlter.ObjectKey = t_S_AlterationCl.internalKey;
            objCreditAlter.useSummaryRow = false;
            objCreditAlter.mainFilter[t_S_AlterationCl.RefCreditInc] = currentID;
            objCreditAlter.AddDataColumn(t_S_AlterationCl.Charge);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.DocDate);
            objCreditAlter.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                t_S_AlterationCl.RefKindDocs,
                fx_S_KindDocs.internalKey,
                fx_S_KindDocs.Name,
                String.Empty);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.Num);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.ChargeDate);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.Sum);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.EndDate);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.CreditPercent);
            objCreditAlter.AddDataColumn(t_S_AlterationCl.IntRepayUpTo);
            tablesResult[6] = objCreditAlter.FillData();

            foreach (DataRow rowAlter in tablesResult[6].Rows)
            {
                if (rowAlter[t_S_AlterationCl.IntRepayUpTo] != DBNull.Value)
                {
                    rowAlter[t_S_AlterationCl.IntRepayUpTo] =
                        String.Format("{0} числа", rowAlter[t_S_AlterationCl.IntRepayUpTo]);
                }
            }

            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = regNumValue;
            rowCaption[1] = reportParams[ReportConsts.ParamEndDate];
            return tablesResult;
        }

        /// <summary>
        /// Московская область - Долговая нагрузка с обслуживанием
        /// </summary>
        public DataTable[] GetMODebtServiceLoadingData(Dictionary<string, string> reportParams)
        {
            return GetMODebtServiceLoadingData(reportParams, null);
        }

        /// <summary>
        /// Московская область - Долговая нагрузка с обслуживанием
        /// </summary>
        public DataTable[] GetMODebtServiceLoadingData(Dictionary<string, string> reportParams, List<int> lstYears)
        {
            var tablesResult = new DataTable[4];
            var tablesDbtData = GetMODebtLoadingData(reportParams, lstYears);
            var tablesSrvData = GetMOServiceSumData(reportParams, lstYears);
            tablesResult[0] = tablesDbtData[0];
            tablesResult[1] = tablesSrvData[0];
            tablesResult[2] = tablesDbtData[1];
            tablesResult[3] = tablesSrvData[1];
            return tablesResult;
        }

        /// <summary>
        /// Московская область - Прогноз кассовых выплат
        /// </summary>
        public DataTable[] GetMOCashPlanData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var month = Convert.ToInt32(reportParams[ReportConsts.ParamMonth].Remove(0, 1));
            var exchangeRate = GetDecimal(reportParams[ReportConsts.ParamExchangeRate]);
            // Кредиты
            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            cdoCI.useSummaryRow = false;
            cdoCI.onlyLastPlanService = true;
            cdoCI.planServiceDate = DateTime.Now.ToShortDateString();
            cdoCI.onlyLastPlanDebt = true;
            cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            // 00 - вид
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 01 - Обслуживание\источники
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 02 - Вид долгового обязательства
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 03 - Дата исполнения обязательств
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 04 - Обязательство
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 05 - Тип
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 06 - Бенефициар\контрагент
            cdoCI.AddCalcColumn(CalcColumnType.cctOrganization);
            // 07 - Объем обязательства (рубли)
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 08 - Дата исполнения обязательства - факт
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 09 - Объем исполнения обязательства (рубли) факт из бюджета МО
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 10 - Объем исполнения обязательства (рубли) факт принципалом
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 11 - Дата привлечения
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 12 - Кредитор
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 13 - Сумма (рубли)
            cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебные
            // 14
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            // 15
            cdoCI.AddCalcColumn(CalcColumnType.cctContractDateNum);
            // 16
            cdoCI.AddDetailColumn(String.Format("[{0}][{1}]", t_S_PlanDebtCI.key, t_S_PlanServiceCI.key));
            
            for (var i = 1; i < 13; i++)
            {
                var monthLo = GetMonthStart(year, i);
                var monthHi = GetMonthEnd(year, i);
                // 17
                cdoCI.AddDetailTextColumn(String.Format("[1](1>={0}1<={1})", monthLo, monthHi), cdoCI.ParamOnlyDates, String.Empty);
                // 18
                cdoCI.AddDetailTextColumn(String.Format("[1](1>={0}1<={1})", monthLo, monthHi), cdoCI.ParamOnlyValues, String.Empty);
                // 19
                cdoCI.AddDetailTextColumn(String.Format("[4](1>={0}1<={1})", monthLo, monthHi), cdoCI.ParamOnlyDates, String.Empty);
                // 20
                cdoCI.AddDetailTextColumn(String.Format("[4](1>={0}1<={1})", monthLo, monthHi), cdoCI.ParamOnlyValues, String.Empty);
            }

            cdoCI.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.SortStatus);
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            var tableOrg = cdoCI.FillData();
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var tableBud = cdoCI.FillData();
            var tblResultOrg = CorrectMOCashPlanData(cdoCI, tableOrg, ReportDocumentType.CreditOrg, year, exchangeRate);
            var tblResultBud = CorrectMOCashPlanData(cdoCI, tableBud, ReportDocumentType.CreditOrg, year, exchangeRate);
            // Гарантии
            var sumFieldRub = CreateValuePair(ReportConsts.SumField);
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdoGrnt.useSummaryRow = false;
            // 00 - вид
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 01 - Обслуживание\источники
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 02 - Вид долгового обязательства
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 03 - Дата исполнения обязательств
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 04 - Обязательство
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGarantTypeNumDate);
            // 05 - Тип
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 06 - Бенефициар\контрагент
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // 07 - Объем обязательства (рубли)
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 08 - Дата исполнения обязательства - факт
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 09 - Объем исполнения обязательства (рубли) факт из бюджета МО
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 10 - Объем исполнения обязательства (рубли) факт принципалом
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 11 - Дата привлечения
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 12 - Кредитор
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 13 - Сумма (рубли)
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебные
            // 14
            cdoGrnt.AddDetailColumn(String.Format("[{0}][{1}]", t_S_PlanDebtPrGrnt.key, t_S_PlanServicePrGrnt.key));
            for (var i = 1; i < 13; i++)
            {
                var monthLo = GetMonthStart(year, i);
                var monthHi = GetMonthEnd(year, i);
                // 15
                cdoGrnt.AddDetailColumn(String.Format("[1](1>={0}1<={1})", monthLo, monthHi), sumFieldRub, true);
                // 16
                cdoGrnt.AddDetailTextColumn(String.Format("[4](1>={0}1<={1})", monthLo, monthHi), cdoGrnt.ParamOnlyDates, String.Empty);
                // 17
                cdoGrnt.AddDetailColumn(String.Format("[4](1>={0}1<={1})", monthLo, monthHi), sumFieldRub, true);
                // 18
                cdoGrnt.AddDetailColumn(String.Format("[2](1>={0}1<={1})", monthLo, monthHi), sumFieldRub, true);
                // 19
                cdoGrnt.AddDetailTextColumn(String.Format("[1](1>={0}1<={1})", monthLo, monthHi), cdoGrnt.ParamOnlyDates, String.Empty);
                // 20
                cdoGrnt.AddDetailTextColumn(String.Format("[2](1>={0}1<={1})", monthLo, monthHi), cdoGrnt.ParamOnlyDates, String.Empty);
            }
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.SortStatus);
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctPrincipalOKV, TempFieldNames.OKVShortName);
            var tableGrnt = cdoGrnt.FillData();
            var tblResultGrnt = CorrectMOCashPlanData(cdoGrnt, tableGrnt, ReportDocumentType.Garant, year, exchangeRate);            
            
            // ЦБ
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCap.useSummaryRow = false;
            // 00 - вид
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 01 - Обслуживание\источники
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 02 - Вид долгового обязательства
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 03 - Дата исполнения обязательств
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 04 - Обязательство
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 05 - Тип
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 06 - Бенефициар\контрагент
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 07 - Объем обязательства (рубли)
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 08 - Дата исполнения обязательства - факт
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 09 - Объем исполнения обязательства (рубли) факт из бюджета МО
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 10 - Объем исполнения обязательства (рубли) факт принципалом
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 11 - Дата привлечения
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 12 - Кредитор
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 13 - Сумма (рубли)
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебные
            // 14
            cdoCap.AddDataColumn(f_S_Capital.StartDate);
            // 15
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            
            for (var i = 1; i < 13; i++)
            {
                var monthLo = GetMonthStart(year, i);
                var monthHi = GetMonthEnd(year, i);
                // 16
                cdoCap.AddDetailTextColumn(String.Format("[1](1>={0}1<={1})", monthLo, monthHi), cdoGrnt.ParamOnlyDates, String.Empty);
                // 17
                cdoCap.AddDetailTextColumn(String.Format("[1](1>={0}1<={1})", monthLo, monthHi), cdoGrnt.ParamOnlyValues, String.Empty);
                // 18
                cdoCap.AddDetailTextColumn(String.Format("[3](1>={0}1<={1})", monthLo, monthHi), cdoGrnt.ParamOnlyDates, String.Empty);
                // 19
                cdoCap.AddDetailTextColumn(String.Format("[3](1>={0}1<={1})", monthLo, monthHi), cdoGrnt.ParamOnlyValues, String.Empty);
            }

            cdoCap.AddDetailColumn(String.Format("[{0}][{1}]", t_S_CPPlanDebt.key, t_S_CPPlanService.key));
            cdoCap.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.SortStatus);
            var tableCap = cdoCap.FillData();
            var tblResultCap = CorrectMOCashPlanData(cdoCap, tableCap, ReportDocumentType.Capital, year, exchangeRate);

            dtTables[0] = CreateReportCaptionTable(80);
            const int mergeColumnsCount = 14;
            AppendRows(tblResultBud, dtTables[0], mergeColumnsCount);
            AppendRows(tblResultOrg, dtTables[0], mergeColumnsCount);
            AppendRows(tblResultGrnt, dtTables[0], mergeColumnsCount);
            AppendRows(tblResultCap, dtTables[0], mergeColumnsCount);

            foreach (DataRow rowData in dtTables[0].Rows)
            {
                rowData[mergeColumnsCount] = Convert.ToDateTime(rowData[3]).Month;
            }

            // сортировка по дате изменения
            dtTables[0] = cdoCI.SortDataSet(dtTables[0], StrSortUp(dtTables[0].Columns[3].ColumnName));

            dtTables[1] = CreateReportCaptionTable(1);

            DataRow drCaption = CreateReportParamsRow(dtTables);

            if (reportParams[ReportConsts.ParamPeriodType] == "i1")
            {
                drCaption[0] = GetYearStart(year);
                drCaption[1] = GetYearEnd(year);
            }
            else
            {
                drCaption[0] = GetMonthStart(year, month);
                drCaption[1] = GetMonthEnd(year, month);
            }
            drCaption[2] = exchangeRate;
            drCaption[3] = year;
            drCaption[4] = reportParams[ReportConsts.ParamPeriodType];
            drCaption[5] = month;
            drCaption[6] = GetMonthRusNames()[month - 1];
            return dtTables;
        }

        /// <summary>
        /// Московская область - Долговая нагрузка
        /// </summary>
        public DataTable[] GetMODebtLoadingData(Dictionary<string, string> reportParams, List<int> lstYears)
        {
            const int GroupSize = 5;
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var year1 = Convert.ToDateTime(calcDate).Year;

            // если первое число года, то данные по предыдущему
            if (calcDate == GetYearStart(year1))
            {
                year1--;
            }

            var year2 = year1;

            if (lstYears == null)
            {
                lstYears = new List<int> { year1 };
            }

            var exchangeRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var sumFieldRub = CreateValuePair(ReportConsts.SumField);
            var sumFieldUsd = CreateValuePair(ReportConsts.CurrencySumField);
            dtTables[0] = CreateReportCaptionTable(GroupSize * lstYears.Count, 12);

            for (var y = 0; y < lstYears.Count; y++)
            {
                var curYear = lstYears[y];
                // Кредиты
                var cdoCI = new CreditDataObject();
                cdoCI.InitObject(scheme);
                cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
                cdoCI.onlyLastPlanService = true;
                cdoCI.planServiceDate = calcDate;
                cdoCI.planDebtDate = calcDate;
                // ЦБ
                var cdoCap = new CapitalDataObject();
                cdoCap.InitObject(scheme);
                cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.ActiveVariantID;
                // Гарантии
                var cdoGrnt = new GarantDataObject();
                cdoGrnt.InitObject(scheme);
                cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.ActiveVariantID;

                cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV); // 0
                cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum); // 1
                cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum); // 2
                cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum); // 3
                cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalCurrencySum); // 4

                for (var i = 1; i < 13; i++)
                {
                    var monthStart = GetMonthStart(curYear, i);
                    var monthEnd = GetMonthEnd(curYear, i);

                    if (curYear < reportDate.Year || curYear == reportDate.Year && i < reportDate.Month)
                    {
                        const string crdFormula = "[1](1>={0}1<={1})";
                        const string capFormula = "[1](1>={0}1<={1})";
                        const string grnFormula = "+[1](1>={0}1<={1})[2](1>={0}1<={1})";

                        cdoCI.AddDetailColumn(String.Format(crdFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoCap.AddDetailColumn(String.Format(capFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldUsd, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldUsd, true);
                    }
                    else if (curYear == reportDate.Year && i == reportDate.Month)
                    {
                        const string crdFormula = "if[1](1>={0}1<={1})>[0](1>{3});[1](1>={0}1<={1});[2](1>{0}1<={2})";
                        const string capFormula = "if[1](1>={0}1<={1})>[0](1>{3});[1](1>={0}1<={1});[6](1>{0}1<={2})";
                        const string grnFormula = "if[1](1>={0}1<={1})>[0](1>{3});+[1](1>={0}1<={1})[2](1>={0}1<={1});+[3](1>{0}1<={2})[2](1>={0}1<={1})";

                        var formulaCrd = String.Format(crdFormula, monthStart, calcDate, monthEnd, maxDate);
                        var formulaCap = String.Format(capFormula, monthStart, calcDate, monthEnd, maxDate);
                        var formulaGrn = String.Format(grnFormula, monthStart, calcDate, monthEnd, maxDate);

                        cdoCI.AddDetailColumn(formulaCrd, sumFieldRub, true);
                        cdoCI.SetColumnParam(cdoCI.ParamPlanDate, monthEnd);
                        cdoCap.AddDetailColumn(formulaCap, sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(formulaGrn, sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(formulaGrn, sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(formulaGrn, sumFieldUsd, true);
                        cdoGrnt.AddDetailColumn(formulaGrn, sumFieldUsd, true);
                    }
                    else
                    {
                        const string crdFormula = "[2](1>={0}1<={1})";
                        const string capFormula = "[6](1>={0}1<={1})";
                        const string grnFormula = "[3](1>={0}1<={1})";

                        cdoCI.AddDetailColumn(String.Format(crdFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoCI.SetColumnParam(cdoCI.ParamPlanDate, monthEnd);
                        cdoCap.AddDetailColumn(String.Format(capFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldRub, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldUsd, true);
                        cdoGrnt.AddDetailColumn(String.Format(grnFormula, monthStart, monthEnd), sumFieldUsd, true);
                    }
                }

                cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
                var tblCreditOrg = cdoCI.FillData();
                cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
                var tblCreditBud = cdoCI.FillData();
                var tblCapital = cdoCap.FillData();
                var tblGrnt = cdoGrnt.FillData();

                // пересчет сумм для валютных и незаполненных
                for (var j = 0; j < tblGrnt.Rows.Count - 1; j++)
                {
                    var drData = tblGrnt.Rows[j];
                    var refOKV = Convert.ToInt32(drData[0]);

                    const int offset = 4;

                    for (var m = 1; m < 13; m++)
                    {
                        var newIndex = offset + (m - 1) * 4 + 1;

                        // для валютных пересчитываем по указанному в параметрах курсу
                        if (refOKV != ReportConsts.codeRUB)
                        {
                            drData[newIndex + 0] = exchangeRate * GetNumber(drData[newIndex + 2]);
                            drData[newIndex + 1] = exchangeRate * GetNumber(drData[newIndex + 3]);
                        }
                    }
                }

                tblGrnt = cdoGrnt.RecalcSummary(tblGrnt);

                var rowCreditOrg = GetLastRow(tblCreditOrg);
                var rowCreditBud = GetLastRow(tblCreditBud);
                var rowResultCap = GetLastRow(tblCapital);
                var rowResultGrnt = GetLastRow(tblGrnt);
                var tblResult = dtTables[0];
                var curIndex = GroupSize * y;

                for (var i = 0; i < 12; i++)
                {
                    tblResult.Rows[i][curIndex + 0] = rowResultCap[i];
                    tblResult.Rows[i][curIndex + 1] = rowCreditOrg[i];
                    tblResult.Rows[i][curIndex + 2] = rowCreditBud[i];
                    tblResult.Rows[i][curIndex + 3] = rowResultGrnt[i * 4 + 5];
                    tblResult.Rows[i][curIndex + 4] = (curYear < reportDate.Year || curYear == reportDate.Year && i < reportDate.Month);
                }
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = year1;
            drCaption[1] = year2;
            drCaption[2] = calcDate;
            return dtTables;
        }


        /// <summary>
        /// Московская область - Расходы на обслуживание долга
        /// </summary>
        public DataTable[] GetMOServiceSumData(Dictionary<string, string> reportParams, List<int>  lstYears)
        {
            const int GroupSize = 3;
            var dtTables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var year1 = Convert.ToDateTime(calcDate).Year;
            var maxDate = DateTime.MaxValue.ToShortDateString();

            // если первое число года, то данные по предыдущему
            if (calcDate == GetYearStart(year1))
            {
                year1--;
            }

            var year2 = year1;
            var sumFields = CreateValuePair(ReportConsts.SumField);

            if (lstYears == null)
            {
                lstYears = new List<int> { year1 };
            }

            dtTables[0] = CreateReportCaptionTable(GroupSize * lstYears.Count, 12);

            for (var y = 0; y < lstYears.Count; y++)
            {
                var curYear = lstYears[y];
                // Кредиты организаций
                var cdoCI = new CreditDataObject();
                cdoCI.InitObject(scheme);
                cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
                cdoCI.onlyLastPlanService = true;
                cdoCI.planServiceDate = calcDate;
                // ЦБ
                var cdoCap = new CapitalDataObject();
                cdoCap.InitObject(scheme);
                cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.ActiveVariantID;

                for (var i = 1; i < 13; i++)
                {
                    var monthStart = GetMonthStart(curYear, i);
                    var monthEnd = GetMonthEnd(curYear, i);

                    if (curYear < reportDate.Year || curYear == reportDate.Year && i < reportDate.Month)
                    {
                        cdoCI.AddDetailColumn(String.Format("[4](1>={0}1<={1})", monthStart, monthEnd), sumFields, true);
                        cdoCap.AddDetailColumn(String.Format("[3](1>={0}1<={1})", monthStart, monthEnd), sumFields, true);
                    }
                    else if (curYear == reportDate.Year && i == reportDate.Month)
                    {
                        const string crdFormula = "if[4](1>={0}1<={1})>[4](1>{3});[4](1>={0}1<={1});[5](1>={0}1<={2})";
                        const string capFormula = "if[3](1>={0}1<={1})>[3](1>{3});[3](1>={0}1<={1});[7](1>={0}1<={2})";

                        cdoCI.AddDetailColumn(String.Format(crdFormula, monthStart, calcDate, monthEnd, maxDate), sumFields, true);
                        cdoCap.AddDetailColumn(String.Format(capFormula, monthStart, calcDate, monthEnd, maxDate), sumFields, true);
                    }
                    else
                    {
                        cdoCI.AddDetailColumn(String.Format("[5](2>={0}2<={1})", monthStart, monthEnd), sumFields, true);
                        cdoCap.AddDetailColumn(String.Format("[7](1>={0}1<={1})", monthStart, monthEnd), sumFields, true);
                    }
                }

                cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
                cdoCap.AddDetailColumn(String.Format("-[7](1>={0})[7](0>={0})", calcDate), CreateValuePair(t_S_CPPlanService.Income), true);
                cdoCap.SetColumnNameParam(t_S_CPPlanService.Income);

                var tblCredit = cdoCI.FillData();
                var tblCapital = cdoCap.FillData();

                var summaryCap = GetLastRow(tblCapital);
                var summaryCrd = GetLastRow(tblCredit);
                var curIndex = GroupSize * y;
                var tblData = dtTables[0];

                for (var i = 0; i < 12; i++)
                {
                    tblData.Rows[i][curIndex + 0] = summaryCap[i];
                    tblData.Rows[i][curIndex + 1] = summaryCrd[i];
                    tblData.Rows[i][curIndex + 2] = (curYear < reportDate.Year || curYear == reportDate.Year && i < reportDate.Month);
                }

                tblCapital.Rows.Remove(summaryCap);

                var emptyRows = new List<DataRow>();

                foreach (DataRow rowCapital in tblCapital.Rows)
                {
                    rowCapital[f_S_Capital.OfficialNumber] = 
                        String.Format("Займ {0} (куп. доход {1} руб.)",
                            rowCapital[f_S_Capital.OfficialNumber],
                            rowCapital[t_S_CPPlanService.Income]);

                    var isNonEmpty = false;

                    for (var i = 0; i < 12; i++)
                    {
                        isNonEmpty = isNonEmpty || GetDecimal(rowCapital[i]) != 0;
                    }

                    if (!isNonEmpty)
                    {
                        emptyRows.Add(rowCapital);
                    }
                }

                foreach (var emptyRow in emptyRows)
                {
                    tblCapital.Rows.Remove(emptyRow);
                }

                dtTables[1] = tblCapital;
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = year1;
            drCaption[1] = year2;
            drCaption[2] = calcDate;
            return dtTables;
        }

        /// <summary>
        /// Долговая книга субъекта Саратов
        /// </summary>
        public DataTable[] GetSubjectDebtorBookSaratovData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[8];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var year = Convert.ToDateTime(calcDate).Year;
            var yearStart = GetYearStart(year);

            // Кредиты организаций

            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            SetCreditFilter(cdoCI.mainFilter, calcDate, yearStart);
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            cdoCI.reportParams[ReportConsts.ParamHiDate] = calcDate;
            cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCI.mainFilter[f_S_Creditincome.RefSStatusPlan] = FormNegFilterValue("4");
            // 00
            cdoCI.AddCalcColumn(CalcColumnType.cctPosition);
            // 01
            cdoCI.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 02
            cdoCI.AddCalcColumn(CalcColumnType.cctContractDateNum);
            // 03
            cdoCI.AddDataColumn(f_S_Creditincome.Purpose);
            // 04
            cdoCI.AddCalcColumn(CalcColumnType.cctCollateralType);
            // 05
            cdoCI.AddParamColumn(CalcColumnType.cctListDocs, String.Empty, String.Empty);
            // 06
            cdoCI.AddDataColumn(f_S_Creditincome.CreditPercent);
            // 07
            cdoCI.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 08
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // 09
            cdoCI.AddDataColumn(f_S_Creditincome.Sum);
            // 10
            cdoCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", yearStart));
            // 11
            cdoCI.AddDetailColumn(String.Format("-[2](1<{0})[1](1<{0})", yearStart));
            // 12
            cdoCI.AddDetailTextColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyDates, "1");
            // 13
            cdoCI.AddDetailTextColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyValues, String.Empty);
            // 14
            cdoCI.AddDetailTextColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyDates, "1");
            // 15
            cdoCI.AddDetailTextColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyValues, String.Empty);
            // 16
            cdoCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            // 17
            cdoCI.AddDetailColumn(String.Format("-[2](1<{0})[1](1<{0})", calcDate));
            // 18
            cdoCI.AddDetailColumn(String.Format("[4](1>={0}1<{1})", yearStart, calcDate));
            // 19
            cdoCI.AddDetailColumn(String.Format("[4](1>={0}1<{1})", yearStart, calcDate));
            // 20
            cdoCI.AddDetailColumn(String.Format("+[8](1>={0}1<{1})[9](1>={0}1<{1})", yearStart, calcDate));
            // 21
            cdoCI.AddDataColumn(f_S_Creditincome.Note);
            // 22
            cdoCI.AddDataColumn(f_S_Creditincome.StartDate, ReportConsts.ftDateTime);
            // служебные 23
            cdoCI.AddDataColumn(f_S_Creditincome.Occasion);
            // 24
            cdoCI.AddDetailColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate));
            // 25
            cdoCI.AddDetailColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate));

            cdoCI.sortString = FormSortString(
                StrSortUp(TempFieldNames.OrgName),
                StrSortUp(f_S_Creditincome.Occasion),
                StrSortUp(f_S_Creditincome.StartDate));

            cdoCI.summaryColumnIndex.Add(09);
            cdoCI.summaryColumnIndex.Add(13);
            cdoCI.summaryColumnIndex.Add(15);
            cdoCI.realSummaryIndex.Add(13, 24);
            cdoCI.realSummaryIndex.Add(15, 25);
            dtTables[2] = cdoCI.FillData();
            dtTables[2].Columns.RemoveAt(dtTables[2].Columns.Count - 1);

            // ГАРАНТИИ

            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            SetGarantFilter(cdoGrnt.mainFilter, calcDate, yearStart);
            cdoGrnt.reportParams[ReportConsts.ParamHiDate] = calcDate;

            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.ActiveVariantID;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefSStatusPlan] = FormNegFilterValue("4");

            // 00
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            // 01
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctOrganization3, TempFieldNames.OrgName);
            // 02
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            // 03 
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Occasion);
            // 04
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalDoc);
            // 05
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            // 06
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 07
            cdoGrnt.AddParamColumn(CalcColumnType.cctListDocs, String.Empty, String.Empty);
            // 08
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DatePerformance);
            // 09
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", yearStart));
            // 10
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 11
            cdoGrnt.AddDetailTextColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate), cdoGrnt.ParamOnlyDates, "1");
            // 12
            cdoGrnt.AddDetailTextColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate), cdoGrnt.ParamOnlyValues, String.Empty);
            // 13
            cdoGrnt.AddDetailTextColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate), cdoGrnt.ParamOnlyDates, "1");
            // 14
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+15;+16;+17;+18");
            // 15
            cdoGrnt.AddDetailColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate));
            // 16
            cdoGrnt.AddDetailColumn(String.Format("[2](1>={0}1<{1})", yearStart, calcDate));
            cdoGrnt.SetColumnCondition("RefTypSum", "1");
            // 17
            cdoGrnt.AddDetailColumn(String.Format("[4](1>={0}1<{1})", yearStart, calcDate));
            // 18
            cdoGrnt.AddDetailColumn(String.Format("+[9](1>={0}1<{1})[12](1>={0}1<{1})", yearStart, calcDate));
            // 19
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            // 20
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 21
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            // служебные 22
            cdoGrnt.AddDetailColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate));

            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            cdoGrnt.sortString = FormSortString(StrSortUp(TempFieldNames.OrgName), StrSortUp(f_S_Guarantissued.StartDate));
            cdoGrnt.summaryColumnIndex.Add(06);
            cdoGrnt.summaryColumnIndex.Add(12);
            cdoGrnt.realSummaryIndex.Add(12, 22);
            dtTables[3] = cdoGrnt.FillData();

            // БЮДЖЕТНЫЕ ССУДЫ

            var cdoBudCI = new CreditDataObject();
            cdoBudCI.InitObject(scheme);
            SetCreditFilter(cdoBudCI.mainFilter, calcDate, yearStart);
            cdoBudCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoBudCI.reportParams[ReportConsts.ParamHiDate] = calcDate;
            cdoBudCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            cdoBudCI.mainFilter[f_S_Creditincome.RefSStatusPlan] = FormNegFilterValue("4");
            // 00
            cdoBudCI.AddCalcColumn(CalcColumnType.cctPosition);
            // 01
            cdoBudCI.AddCalcColumn(CalcColumnType.cctUndefined);
            // 02
            cdoBudCI.AddCalcColumn(CalcColumnType.cctContractDateNum);
            // 03
            cdoBudCI.AddDataColumn(f_S_Creditincome.Purpose);
            // 04
            cdoBudCI.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // 05
            cdoBudCI.AddParamColumn(CalcColumnType.cctListDocs, String.Empty, String.Empty);
            // 06
            cdoBudCI.AddDataColumn(f_S_Creditincome.CreditPercent);
            // 07
            cdoBudCI.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 08
            cdoBudCI.AddDataColumn(f_S_Creditincome.Sum);
            // 09
            cdoBudCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", yearStart));
            // 10
            cdoBudCI.AddDetailColumn(String.Format("-[2](1<{0})[1](1<{0})", yearStart));
            // 11
            cdoBudCI.AddDetailTextColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyDates, "1");
            // 12
            cdoBudCI.AddDetailTextColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyValues, String.Empty);
            // 13
            cdoBudCI.AddDetailTextColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyDates, "1");
            // 14
            cdoBudCI.AddDetailTextColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyValues, String.Empty);
            // 15 - isForgiven = 1
            cdoBudCI.AddDetailTextColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyDates, "1");
            cdoBudCI.SetColumnCondition("IsForgiven", "1");
            // 16 - isForgiven = 1
            cdoBudCI.AddDetailTextColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate), cdoCI.ParamOnlyValues, String.Empty);
            cdoBudCI.SetColumnCondition("IsForgiven", "1");
            // 17
            cdoBudCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            // 18
            cdoBudCI.AddDetailColumn(String.Format("-[2](1<{0})[1](1<{0})", calcDate));
            // 19
            cdoBudCI.AddDetailColumn(String.Format("[4](1>={0}1<{1})", yearStart, calcDate));
            // 20
            cdoBudCI.AddDetailColumn(String.Format("[4](1>={0}1<{1})", yearStart, calcDate));
            // 21
            cdoBudCI.AddDetailColumn(String.Format("+[8](1>={0}1<{1})[9](1>={0}1<{1})", yearStart, calcDate));
            // 22
            cdoBudCI.AddDataColumn(f_S_Creditincome.StartDate, ReportConsts.ftDateTime);
            // служебные
            // 23
            cdoBudCI.AddDetailColumn(String.Format("[0](1>={0}1<{1})", yearStart, calcDate));
            // 24
            cdoBudCI.AddDetailColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate));
            // 25
            cdoBudCI.AddDetailColumn(String.Format("[1](1>={0}1<{1})", yearStart, calcDate));
            cdoBudCI.SetColumnCondition("IsForgiven", "1");

            cdoBudCI.summaryColumnIndex.Add(08);
            cdoBudCI.summaryColumnIndex.Add(12);
            cdoBudCI.summaryColumnIndex.Add(14);
            cdoBudCI.summaryColumnIndex.Add(16);
            cdoBudCI.realSummaryIndex.Add(12, 23);
            cdoBudCI.realSummaryIndex.Add(14, 24);
            cdoBudCI.realSummaryIndex.Add(16, 25);

            cdoBudCI.sortString = FormSortString(StrSortUp(f_S_Creditincome.StartDate));
            dtTables[1] = cdoBudCI.FillData();

            var reportDate = Convert.ToDateTime(calcDate);

            // пересчет просроченной задолженности
            dtTables[1] = ClearStaleRestSaratov(cdoBudCI, dtTables[1], reportDate, 18, 4);
            dtTables[2] = ClearStaleRestSaratov(cdoCI, dtTables[2], reportDate, 17, 8);

            dtTables[0] = CreateReportCaptionTable(1); // ЦБ
            dtTables[4] = CreateReportCaptionTable(1); // Иные
            dtTables[5] = CreateReportCaptionTable(12, 3); // Структура
            dtTables[6] = CreateReportCaptionTable(13, 1); // Обслуживание
            var rowServiceResult = GetLastRow(dtTables[6]);

            for (var i = 0; i < dtTables[1].Rows.Count - 1; i++)
                dtTables[1].Rows[i][1] = "Федеральный";

            var curYearStart = String.Format("{0}0100", DateTime.Now.Year);
            // Расходы на обслуживание
            var monthRepOut = new AbstractTable(f_F_MonthRepOutcomes.InternalKey);
            var clsEKR = new AbstractTable(d_EKR_MonthRep.InternalKey);
            var clsFKR = new AbstractTable(d_R_MonthRep.InternalKey);
            var clsRegions = new AbstractTable(d_Regions_MonthRep.InternalKey);
            var ufkServiceObj = new UFKDataObject();
            ufkServiceObj.InitObject(scheme);
            ufkServiceObj.useSummaryRow = false;
            ufkServiceObj.useEmptyRows = false;
            ufkServiceObj.AddDataEntity(monthRepOut);
            ufkServiceObj.AddFilter(monthRepOut, FormFilterValue(f_F_MonthRepOutcomes.RefBdgtLevels, "3"));
            ufkServiceObj.leadingInfo.leadTable = monthRepOut;
            ufkServiceObj.leadingInfo.leadFieldName = f_F_MonthRepOutcomes.RefYearDayUNV;
            ufkServiceObj.AddClsLink(clsEKR, monthRepOut, f_F_MonthRepOutcomes.RefEKR);
            ufkServiceObj.AddClsLink(clsFKR, monthRepOut, f_F_MonthRepOutcomes.RefFKR);
            ufkServiceObj.AddClsLink(clsRegions, monthRepOut, f_F_MonthRepOutcomes.RefRegions);
            ufkServiceObj.AddClsColumn(monthRepOut, f_F_MonthRepOutcomes.RefYearDayUNV);
            ufkServiceObj.AddDataColumn(monthRepOut, f_F_MonthRepOutcomes.YearPlanReport);
            ufkServiceObj.AddColumnFilter(clsEKR, FormFilterValue(d_EKR_MonthRep.Code, "'230'"));
            ufkServiceObj.AddColumnFilter(clsFKR, FormFilterValue(d_R_MonthRep.Code, "'13000000000000'"));
            ufkServiceObj.AddDataColumn(monthRepOut, f_F_MonthRepOutcomes.YearPlan);
            ufkServiceObj.AddColumnFilter(clsFKR, FormFilterValue(d_R_MonthRep.Code, "'96000000000000'"));
            ufkServiceObj.AddColumnFilter(clsRegions, FormFilterValue(d_Regions_MonthRep.BudgetKind, "'СБС'"));
            ufkServiceObj.AddDataColumn(monthRepOut, f_F_MonthRepOutcomes.YearPlanReport);
            ufkServiceObj.AddColumnFilter(clsFKR, FormFilterValue(d_R_MonthRep.Code, "'96000000000000'"));
            ufkServiceObj.AddColumnFilter(clsRegions, FormFilterValue(d_Regions_MonthRep.BudgetKind, "'СБС'"));
            ufkServiceObj.AddDataColumn(monthRepOut, f_F_MonthRepOutcomes.FactReport);
            ufkServiceObj.AddColumnFilter(clsFKR, FormFilterValue(d_R_MonthRep.Code, "'96000000000000'"));
            ufkServiceObj.AddColumnFilter(clsRegions, FormFilterValue(d_Regions_MonthRep.BudgetKind, "'СБС'"));
            var tblServiceFact = ufkServiceObj.FillData();
            tblServiceFact = DataTableUtils.SortDataSet(tblServiceFact, StrSortUp(tblServiceFact.Columns[0].ColumnName));
            var rowServiceLstData = GetLastRow(tblServiceFact);
            var rowServiceFstData = FindDataRow(tblServiceFact, curYearStart, tblServiceFact.Columns[0].ColumnName);

            if (rowServiceFstData != null)
            {
                rowServiceResult[0] = rowServiceFstData[1];
                rowServiceResult[2] = rowServiceFstData[2];
            }
            if (rowServiceLstData != null)
            {
                rowServiceResult[01] = rowServiceLstData[1];
                rowServiceResult[03] = rowServiceLstData[3];
                rowServiceResult[12] = rowServiceLstData[4];
            }

            // структура
            var monthRepIn = new AbstractTable(f_F_MonthRepInFin.InternalKey);
            var clsSIF = new AbstractTable(d_SIF_MonthRep.InternalKey);
            var ufkStructureObj = new UFKDataObject();
            ufkStructureObj.InitObject(scheme);
            ufkStructureObj.useSummaryRow = false;
            ufkStructureObj.useEmptyRows = false;
            ufkStructureObj.AddDataEntity(monthRepIn);
            ufkStructureObj.AddFilter(monthRepIn, FormFilterValue(f_F_MonthRepInFin.RefBdgtLevels, "3"));
            ufkStructureObj.AddFilter(clsRegions, FormFilterValue(d_Regions_MonthRep.BudgetKind, "'СБС'"));
            ufkStructureObj.leadingInfo.leadTable = monthRepIn;
            ufkStructureObj.leadingInfo.leadFieldName = f_F_MonthRepInFin.RefYearDayUNV;
            ufkStructureObj.AddClsLink(clsSIF, monthRepIn, f_F_MonthRepInFin.RefSIF);
            ufkStructureObj.AddClsLink(clsRegions, monthRepIn, f_F_MonthRepInFin.RefRegions);
            // 0
            ufkStructureObj.AddClsColumn(monthRepIn, f_F_MonthRepInFin.RefYearDayUNV);
            // 1 - комм. кредиты
            ufkStructureObj.AddDataColumn(monthRepIn, f_F_MonthRepInFin.YearPlanReport);
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KL, "205"));
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KST, "520"));
            // 2
            ufkStructureObj.AddDataColumn(monthRepIn, f_F_MonthRepInFin.YearPlanReport);
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KL, "315"));
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KST, "520"));
            // 3 - гарантии
            ufkStructureObj.AddDataColumn(monthRepIn, f_F_MonthRepInFin.YearPlanReport);
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KL, "1815"));
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KST, "520"));
            // 4
            ufkStructureObj.AddDataColumn(monthRepIn, f_F_MonthRepInFin.YearPlanReport);
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KL, "1685"));
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KST, "520"));
            // 5 - бюджетные кредиты
            ufkStructureObj.AddDataColumn(monthRepIn, f_F_MonthRepInFin.YearPlanReport);
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KL, "435"));
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KST, "520"));
            // 6
            ufkStructureObj.AddDataColumn(monthRepIn, f_F_MonthRepInFin.YearPlanReport);
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KL, "545"));
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.KST, "520"));
            // 7
            ufkStructureObj.AddDataColumn(monthRepIn, f_F_MonthRepInFin.YearPlanReport);
            ufkStructureObj.AddColumnFilter(clsSIF, FormFilterValue(d_SIF_MonthRep.CodeStr, "'00001020000000000800'"));
            var tblStructureFact = ufkStructureObj.FillData();
            tblStructureFact = DataTableUtils.SortDataSet(tblStructureFact, StrSortUp(tblStructureFact.Columns[0].ColumnName));
            var rowStructureLstData = GetLastRow(tblStructureFact);
            var rowStructureFstData = FindDataRow(tblStructureFact, curYearStart, tblStructureFact.Columns[0].ColumnName);
            var curTable = dtTables[5];

            if (rowStructureFstData != null)
            {
                for (var i = 0; i < 3; i++)
                {
                    curTable.Rows[i][2] = rowStructureFstData[i * 2 + 1];
                    curTable.Rows[i][5] = rowStructureFstData[i * 2 + 2];
                }
            }

            if (rowStructureLstData != null)
            {
                for (var i = 0; i < 3; i++)
                {
                    curTable.Rows[i][3] = rowStructureLstData[i * 2 + 1];
                    curTable.Rows[i][6] = rowStructureLstData[i * 2 + 2];
                }
                curTable.Rows[0][6] = rowStructureLstData[7];
            }

            // контроль
            var monthRepIncome = new AbstractTable(f_F_MonthRepIncomes.InternalKey);
            var clsKD = new AbstractTable(d_KD_MonthRep.InternalKey);
            var ufkControlObj = new UFKDataObject();
            ufkControlObj.InitObject(scheme);
            ufkControlObj.useSummaryRow = false;
            ufkControlObj.useEmptyRows = false;
            ufkControlObj.AddDataEntity(monthRepIncome);
            ufkControlObj.AddFilter(monthRepIncome, FormFilterValue(f_F_MonthRepIncomes.RefBdgtLevels, "3"));
            ufkControlObj.AddFilter(clsRegions, FormFilterValue(d_Regions_MonthRep.BudgetKind, "'СБС'"));
            ufkControlObj.leadingInfo.leadTable = monthRepIncome;
            ufkControlObj.leadingInfo.leadFieldName = f_F_MonthRepIncomes.RefYearDayUNV;
            ufkControlObj.AddClsLink(clsRegions, monthRepIncome, f_F_MonthRepIncomes.RefRegions);
            ufkControlObj.AddClsLink(clsKD, monthRepIncome, f_F_MonthRepIncomes.RefKD);
            ufkControlObj.AddClsColumn(monthRepIncome, f_F_MonthRepOutcomes.RefYearDayUNV);
            ufkControlObj.AddDataColumn(monthRepIncome, f_F_MonthRepIncomes.YearPlan);
            ufkControlObj.AddColumnFilter(clsKD, FormFilterValue(d_KD_MonthRep.KL, "10..12100"));
            ufkControlObj.AddColumnFilter(clsKD, FormFilterValue(d_KD_MonthRep.KL, "15120..18880"));
            ufkControlObj.AddDataColumn(monthRepIncome, f_F_MonthRepIncomes.YearPlanReport);
            ufkControlObj.AddColumnFilter(clsKD, FormFilterValue(d_KD_MonthRep.CodeStr, "'00010000000000000000'"));
            ufkControlObj.AddDataColumn(monthRepIncome, f_F_MonthRepIncomes.FactReport);
            ufkControlObj.AddColumnFilter(clsKD, FormFilterValue(d_KD_MonthRep.CodeStr, "'00010000000000000000'"));
            var tblControlFact = ufkControlObj.FillData();
            tblControlFact = DataTableUtils.SortDataSet(tblControlFact, StrSortUp(tblControlFact.Columns[0].ColumnName));
            var rowControlLstData = GetLastRow(tblControlFact);
            var rowControlFstData = FindDataRow(tblControlFact, GetUNVYearStart(year), tblControlFact.Columns[0].ColumnName);
            
            if (rowControlFstData != null)
            {
                rowServiceResult[4] = rowControlFstData[1];
            }

            if (rowControlLstData != null)
            {
                rowServiceResult[5] = rowControlLstData[2];
                rowServiceResult[6] = rowControlLstData[3];
            }

            var cdoLimit = new CommonDataObject();
            var nextYearStart = Convert.ToInt32(GetUNVYearStart(year + 1)) + 1;
            var nextYearEnd = GetUNVYearEnd(year + 1);
            cdoLimit.InitObject(scheme);
            cdoLimit.useSummaryRow = false;
            cdoLimit.ObjectKey = f_S_DebtLimit.internalKey;
            cdoLimit.mainFilter[f_S_DebtLimit.RefYearDayUNV] =
                String.Format(">={0} and {1}<={2}", nextYearStart, f_S_DebtLimit.RefYearDayUNV, nextYearEnd);
            cdoLimit.AddDataColumn(f_S_DebtLimit.StateDebtUpLim);
            cdoLimit.AddDataColumn(f_S_DebtLimit.StateDebtService);
            cdoLimit.AddDataColumn(f_S_DebtLimit.RefYearDayUNV, ReportConsts.ftInt32);
            cdoLimit.AddDataColumn(f_S_DebtLimit.AcceptDate, ReportConsts.ftDateTime);
            cdoLimit.sortString = FormSortString(
                StrSortUp(f_S_DebtLimit.RefYearDayUNV), 
                StrSortUp(f_S_DebtLimit.AcceptDate));
            var tblDebtLimit = cdoLimit.FillData();
            var actualLstLimit = GetLastRow(tblDebtLimit);
            var actualFstLimit = GetFirstRow(tblDebtLimit);

            if (actualLstLimit != null)
            {
                rowServiceResult[09] = actualLstLimit[0];
                var rowCount = tblDebtLimit.Rows.Count - 1;

                while (actualLstLimit[1] == DBNull.Value && rowCount >= 0)
                {
                    actualLstLimit = tblDebtLimit.Rows[rowCount--];
                }
                
                rowServiceResult[10] = actualLstLimit[1];
            }

            if (actualFstLimit != null)
            {
                rowServiceResult[07] = actualFstLimit[0];
                rowServiceResult[08] = actualFstLimit[1];
            }

            // заголовочные параметры
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = year;
            drCaption[2] = GetYearStart(year);
            return dtTables;
        }


        /// <summary>
        /// Выписка из долговой книги краткая
        /// </summary>
        public DataTable[] GetMOExtractDebtBookShortData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            var hiDate = GetParamDate(reportParams, ReportConsts.ParamReportDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exchangeRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var rubFields = CreateValuePair(ReportConsts.SumField);
            var curFields = CreateValuePair(ReportConsts.CurrencySumField);
            var reportDate = Convert.ToDateTime(hiDate);
            var planDebtLoBound = GetMonthStart(reportDate);
            
            if (reportDate.Day == 1)
            {
                planDebtLoBound = GetMonthStart(reportDate.AddMonths(-1));
            }

            // ЦБ
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.useSummaryRow = false;
            cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.FixedVariantsID;
            cdoCap.mainFilter[f_S_Capital.StartDate] = String.Format("<'{0}'", hiDate);
            cdoCap.sortString = StrSortUp(f_S_Capital.StartDate);
            cdoCap.valuesSeparator = ";";
            cdoCap.textAppendix = " R";
            // 0
            cdoCap.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoCap.AddCalcColumn(CalcColumnType.cctCapMOFoundation);
            // 2
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 3
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 4
            cdoCap.AddCalcColumn(CalcColumnType.cctOKVName);
            // 5
            cdoCap.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", hiDate));
            // служебные 
            // 6
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            // 7
            cdoCap.AddDetailTextColumn(String.Format("[6](1>{0})", planDebtLoBound), cdoCap.ParamFieldList, CreateValuePair(t_S_CPPlanDebt.PercentNom));
            // 8
            cdoCap.AddParamColumn(CalcColumnType.cctRecordCount, "6");
            // 9
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
            // 10
            cdoCap.AddDataColumn(f_S_Capital.ExstraIssue);
            // 11
            cdoCap.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoCap.ParamOnlyDates, rubFields);
            // 12
            cdoCap.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);

            var dtCap = cdoCap.FillData();
            var capNumbers = new Collection<string>();

            foreach (DataRow dr in dtCap.Rows)
            {
                var officialNum = dr[f_S_Capital.OfficialNumber].ToString();
                
                if (officialNum.Length > 0 && !capNumbers.Contains(officialNum))
                {
                    capNumbers.Add(officialNum);
                }

                dr[3] = dr[9];
                
                if (Convert.ToInt32(dr[8]) <= 1)
                {
                    continue;
                }

                var parts = dr[7].ToString().Split(';');
                var fullPercentText = parts.Aggregate(String.Empty, (current, part) => 
                    String.Format("{0}{1}% от номин.стоим.облигации; ", current, part));

                dr[3] = fullPercentText.Trim().Trim(';');
            }
            
            var dtCapResult = dtCap.Clone();
            var counter = 1;

            foreach (var officialNum in capNumbers)
            {
                var rowsCap = dtCap.Select(
                    String.Format("{0} = '{1}'", f_S_Capital.OfficialNumber, officialNum),
                    f_S_Capital.ExstraIssue);

                decimal sumValue = 0;
                var startDateValue = String.Empty;

                foreach (var rowCap in rowsCap)
                {
                    sumValue += GetNumber(rowCap[5]);

                    if (rowCap[12] != DBNull.Value)
                    {
                        startDateValue = String.Join(",", new [] {startDateValue,
                            Convert.ToDateTime(rowCap[12]).ToShortDateString()});
                    }
                }

                rowsCap[0][2] = startDateValue.Trim(',');
                rowsCap[0][5] = sumValue;

                dtCapResult.ImportRow(rowsCap[0]);
                var drResult = GetLastRow(dtCapResult);
                drResult[0] = counter++;
            }
            
            dtTables[0] = dtCapResult;
            dtTables[0] = ClearCloseCreditMO(cdoCap, dtTables[0], Convert.ToDateTime(hiDate), 5, 11, 0);

            // БЮДЖЕТНЫЕ КРЕДИТЫ
            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            cdoCI.useSummaryRow = false;
            cdoCI.onlyLastPlanService = true;
            cdoCI.planServiceDate = hiDate;
            cdoCI.planDebtDate = hiDate;
            cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.FixedVariantsID;
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoCI.mainFilter[f_S_Creditincome.ContractDate] = String.Format("<'{0}'", hiDate);
            cdoCI.sortString = StrSortUp(f_S_Creditincome.ContractDate);
            // 0
            cdoCI.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoCI.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 2
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            // 3
            cdoCI.AddDetailTextColumn(String.Format("[2](1>{0})", planDebtLoBound), String.Empty, String.Empty);
            // 4
            cdoCI.AddCalcColumn(CalcColumnType.cctOKVName);
            // 5
            cdoCI.AddDetailColumn(String.Format("+-[0](1<{0})[1](1<{0})[10](0<{0})", hiDate));
            // 6
            cdoCI.AddDataColumn(f_S_Creditincome.EndDate);
            // 7
            cdoCI.AddDataColumn(f_S_Creditincome.RenewalDate);
            // 8
            cdoCI.AddParamColumn(CalcColumnType.cctRecordCount, "2");
            // 9
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            // 10
            cdoCI.AddDataColumn(f_S_Creditincome.RefOrganizations);
            // 11
            cdoCI.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoCI.ParamOnlyDates, rubFields);
            dtTables[1] = cdoCI.FillData();
            
            foreach (DataRow rowData in dtTables[1].Rows)
            {
                rowData[1] = "Бюджетный кредит";

                if (Convert.ToInt32(rowData[8]) > 1)
                {
                    continue;
                }

                rowData[3] = rowData[f_S_Creditincome.EndDate];

                if (rowData[f_S_Creditincome.RenewalDate] != DBNull.Value)
                {
                    rowData[3] = rowData[f_S_Creditincome.RenewalDate];
                }
            }

            // КРЕДИТЫ ОРГАНИЗАЦИЙ
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtTables[2] = cdoCI.FillData();
            foreach (DataRow rowData in dtTables[2].Rows)
            {
                rowData[1] = "Кредит";
                var contractDate = Convert.ToDateTime(rowData[f_S_Creditincome.ContractDate]);
                var loBound = contractDate;
                var hiBound = contractDate.AddDays(5);

                if (DateTime.Compare(reportDate, loBound) >= 0 && DateTime.Compare(reportDate, hiBound) <= 0)
                {
                    rowData[1] = rowData[9];
                }

                if (Convert.ToInt32(rowData[8]) > 1)
                {
                    continue;
                }

                rowData[3] = rowData[f_S_Creditincome.EndDate];

                if (rowData[f_S_Creditincome.RenewalDate] != DBNull.Value)
                {
                    rowData[3] = rowData[f_S_Creditincome.RenewalDate];
                }
            }

            dtTables[1] = ClearCloseCreditMO(cdoCI, dtTables[1], reportDate, 5, 11, 0);
            dtTables[2] = ClearCloseCreditMO(cdoCI, dtTables[2], reportDate, 5, 11, 0);

            // ГАРАНТИИ

            const string mainDebtFormula = "---+[0](1<{0})[5](1<{1})[1](1<{0})[4](1<{0})[2](1<{0})";
            var mainDebtAttrCondition = Combine(ReportConsts.GrntTypeSumDbt, ReportConsts.GrntTypeSumPct);

            const string dbtGrntFormula = "--[0](1<{1})[1](1<{0})[2](1<{0})";
            const string pctGrntFormula = "---+[3](1<{1})[5](1<{1})[1](1<{0})[4](1<{0})[2](1<{0})";

            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.FixedVariantsID;
            cdoGrnt.mainFilter[f_S_Guarantissued.StartDate] = String.Format("<'{0}'", hiDate);
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.Num);
            cdoGrnt.textAppendix = " R";
            cdoGrnt.planServiceDate = hiDate;
            // 0
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // 2
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            // 3
            cdoGrnt.AddDetailTextColumn(String.Format("[3](1>{0})", hiDate), String.Empty, String.Empty);
            // 4
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            // 5
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебные
            // 6
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum);
            // 7
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalCurrencySum);
            // 8
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            // 9 
            cdoGrnt.AddDetailColumn(String.Format("[3](1<={0})", maxDate));
            // 10
            cdoGrnt.AddDetailColumn(String.Format(pctGrntFormula, hiDate, maxDate), rubFields, true);
            // 11
            cdoGrnt.AddDetailColumn(String.Format(dbtGrntFormula, hiDate, maxDate), rubFields, true);
            // 12
            cdoGrnt.AddDetailColumn(String.Format(pctGrntFormula, hiDate, maxDate), curFields, true);
            // 13
            cdoGrnt.AddDetailColumn(String.Format(dbtGrntFormula, hiDate, maxDate), curFields, true);
            // 14
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 15
            cdoGrnt.AddDataColumn(f_S_Guarantissued.CurrencySum);
            // 16
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            // 17
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RenewalDate);
            // 18
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, "0");
            // 19
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            // 20
            cdoGrnt.AddDetailColumn(String.Format(mainDebtFormula, hiDate, maxDate), rubFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, mainDebtAttrCondition);
            // 21
            cdoGrnt.AddDetailColumn(String.Format(mainDebtFormula, hiDate, maxDate), curFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, mainDebtAttrCondition);
            // 22
            cdoGrnt.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoGrnt.ParamOnlyDates, rubFields);

            dtTables[3] = cdoGrnt.FillData();
            
            foreach (DataRow rowData in dtTables[3].Rows)
            {
                rowData[1] = "Государственная гарантия Московской области";
                
                if (Convert.ToInt32(rowData[18]) > 1) continue;

                rowData[3] = rowData[f_S_Guarantissued.EndDate];
                if (rowData[f_S_Guarantissued.RenewalDate] != DBNull.Value)
                {
                    rowData[3] = rowData[f_S_Guarantissued.RenewalDate];
                }
            }

            foreach (DataRow rowData in dtTables[3].Rows)
            {
                rowData[5] = rowData[20];
                if (Convert.ToInt32(rowData[8]) != ReportConsts.codeRUB)
                {
                    rowData[5] = Math.Round(exchangeRate * GetNumber(rowData[21]), 2);
                }
            }

            dtTables[3] = ClearCloseCreditMO(cdoGrnt, dtTables[3], Convert.ToDateTime(hiDate), 5, 22, 0);

            for (var i = 0; i < 4; i++)
            {
                SetColumnValue(dtTables[i], 4, "руб.");
            }

            // заголовочные параметры
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = hiDate;
            drCaption[1] = GetDateText(hiDate);
            drCaption[2] = exchangeRate;
            drCaption[3] = GetDateText(reportParams[ReportConsts.ParamExchangeDate]);
            FillSignatureData(drCaption, 4, reportParams, ReportConsts.ParamExecutor1, ReportConsts.ParamExecutor2);
            return dtTables;
        }

        /// <summary>
        /// Выписка из долговой книги
        /// </summary>
        public DataTable[] GetMOExtractDebtBookData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            var hiDate = GetParamDate(reportParams, ReportConsts.ParamReportDate);
            var calcDate = Convert.ToDateTime(hiDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exchangeRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var rubFields = CreateValuePair(ReportConsts.SumField);
            var curFields = CreateValuePair(ReportConsts.CurrencySumField);
            var planDebtLoBound = GetMonthStart(calcDate);

            if (calcDate.Day == 1)
            {
                planDebtLoBound = GetMonthStart(calcDate.AddMonths(-1));
            }

            // ЦБ
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.useSummaryRow = false;
            cdoCap.reportParams[ReportConsts.ParamHiDate] = hiDate;
            cdoCap.textAppendix = " R";
            cdoCap.valuesSeparator = ";";
            cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.FixedVariantsID;
            cdoCap.mainFilter[f_S_Capital.StartDate] = String.Format("<'{0}'", hiDate);
            // 0
            cdoCap.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 2
            cdoCap.AddCalcColumn(CalcColumnType.cctCapMOFoundation);
            // 3
            cdoCap.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 4
            cdoCap.AddCalcColumn(CalcColumnType.cctOKVName);
            // 5
            cdoCap.AddDataColumn(f_S_Capital.Sum);
            // 6
            cdoCap.AddDetailColumn(String.Format("[7](1<{0})", maxDate));
            // 7
            cdoCap.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", hiDate));
            // 8
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+17;+18");
            // 9
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
            // служебные
            // 10
            cdoCap.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);
            // 11
            cdoCap.AddDataColumn(f_S_Capital.RefOKV);
            // 12
            cdoCap.AddDetailTextColumn(String.Format("[6](1>{0})", planDebtLoBound), cdoCap.ParamFieldList, CreateValuePair(t_S_CPPlanDebt.PercentNom));
            // 13
            cdoCap.AddParamColumn(CalcColumnType.cctRecordCount, "6");
            // 14
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            // 15
            cdoCap.AddDataColumn(f_S_Capital.ExstraIssue);
            // 16
            cdoCap.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoCap.ParamOnlyDates, rubFields);
            // 17
            cdoCap.AddDetailColumn(String.Format("-[7](1<{0})[3](1<{1})", maxDate, hiDate));
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, ReportConsts.CapTypeSumCoupon);
            // 18
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", hiDate), CreateValuePair(t_S_CPFactService.ChargeSum), true);

            cdoCap.sortString = StrSortUp(f_S_Capital.StartDate);
            var dtCap = cdoCap.FillData();
            var dtCapResult = dtCap.Clone();
            var capNumbers = new Collection<string>();

            foreach (DataRow rowData in dtCap.Rows)
            {
                var officialNum = rowData[f_S_Capital.OfficialNumber].ToString();

                if (officialNum.Length > 0 && !capNumbers.Contains(officialNum))
                {
                    capNumbers.Add(officialNum);
                }

                if (Convert.ToInt32(rowData[13]) <= 1)
                {
                    continue;
                }

                var parts = rowData[12].ToString().Split(';');
                var fullPercentText = parts.Aggregate(String.Empty, (current, part) => 
                    String.Format("{0}{1}; ", current, part));
                rowData[9] = fullPercentText.Trim().Trim(';');
            }

            var counter = 1;
            foreach (var officialNum in capNumbers)
            {
                var rowsCap = dtCap.Select(
                    String.Format("{0} = '{1}'", f_S_Capital.OfficialNumber, officialNum),
                    f_S_Capital.ExstraIssue);

                var sumValue = new decimal[4];

                foreach (var rowCap in rowsCap)
                {
                    sumValue[0] += GetNumber(rowCap[5]);
                    sumValue[1] += GetNumber(rowCap[6]);
                    sumValue[2] += GetNumber(rowCap[7]);
                    sumValue[3] += GetNumber(rowCap[8]);
                }

                var rowResult = rowsCap[0];
                rowResult[5] = sumValue[0];
                rowResult[6] = sumValue[1];
                rowResult[7] = sumValue[2];
                rowResult[8] = sumValue[3];
                dtCapResult.ImportRow(rowResult);
                var drResult = GetLastRow(dtCapResult);
                drResult[0] = counter++;
            }

            dtTables[0] = dtCapResult;
            dtTables[0] = ClearCloseCreditMO(cdoCap, dtTables[0], calcDate, 7, 16, 0);

            // БЮДЖЕТНЫЕ КРЕДИТЫ
            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            cdoCI.useSummaryRow = false;
            cdoCI.onlyLastPlanService = true;
            cdoCI.planServiceDate = hiDate;
            cdoCI.planDebtDate = hiDate;
            cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.FixedVariantsID;
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoCI.mainFilter[f_S_Creditincome.ContractDate] = String.Format("<'{0}'", hiDate);
            cdoCI.onlyLastPlanService = true;
            cdoCI.planServiceDate = hiDate;
            cdoCI.reportParams[ReportConsts.ParamHiDate] = hiDate;
            // 0
            cdoCI.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoCI.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 2
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            // 3
            cdoCI.AddCalcColumn(CalcColumnType.cctNearestPercent);
            // 4
            cdoCI.AddCalcColumn(CalcColumnType.cctOKVName);
            // 5
            cdoCI.AddDataColumn(f_S_Creditincome.Sum);
            // 6
            cdoCI.AddDetailColumn(String.Format("[5](1<{0})", maxDate));
            // 7
            cdoCI.AddDetailColumn(String.Format("+-[0](1<{0})[1](1<{0})[10](0<{0})", hiDate));
            // 8
            cdoCI.AddDetailColumn(String.Format("-[5](1<{0})[4](1<{1})", maxDate, hiDate));
            // 9
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // служебные
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            cdoCI.AddDataColumn(f_S_Capital.RefOKV);
            cdoCI.AddDetailTextColumn(String.Format("[2](1>{0})", planDebtLoBound), String.Empty, String.Empty);
            cdoCI.AddParamColumn(CalcColumnType.cctRecordCount, "2");
            cdoCI.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoCI.ParamOnlyDates, rubFields);
            cdoCI.AddDetailColumn(String.Format("[4](1<{0})", hiDate), CreateValuePair(t_S_FactPercentCI.ChargeSum), true);
            cdoCI.SetColumnParam(cdoCI.ParamSumValueType, "-");
            cdoCI.sortString = FormSortString(StrSortUp(TempFieldNames.OrgName), StrSortUp(f_S_Creditincome.ContractDate));
            dtTables[1] = cdoCI.FillData();

            foreach (DataRow rowData in dtTables[1].Rows)
            {
                rowData[1] = "Министерство финансов Российской Федерации";
                if (Convert.ToInt32(rowData[13]) > 1)
                {
                    rowData[9] = rowData[12];
                }

                // для бюджетных надо учитывать списания
                rowData[8] = GetNumber(rowData[8]) + GetNumber(rowData[15]);
            }

            // КРЕДИТЫ ОРГАНИЗАЦИЙ
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtTables[2] = cdoCI.FillData();

            foreach (DataRow rowData in dtTables[2].Rows)
            {
                if (Convert.ToInt32(rowData[13]) > 1)
                {
                    rowData[9] = rowData[12];
                }
            }

            dtTables[1] = ClearCloseCreditMO(cdoCI, dtTables[1], calcDate, 7, 14, 0);
            dtTables[2] = ClearCloseCreditMO(cdoCI, dtTables[2], calcDate, 7, 14, 0);

            // ГАРАНТИИ
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            cdoGrnt.textAppendix = " R";
            cdoGrnt.planServiceDate = hiDate;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.FixedVariantsID;
            cdoGrnt.mainFilter[f_S_Guarantissued.StartDate] = String.Format("<'{0}'", hiDate);
            // 0
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // 2
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGarantOrgTypeNumDate);
            // 3
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 4
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 5
            cdoGrnt.AddDetailColumn(String.Format("[0](1<{0})", maxDate), rubFields, true);
            // 6
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 7
            cdoGrnt.AddDetailColumn(String.Format("--[0](1<{1})[1](1<{0})[2](1<{0})", hiDate, maxDate), rubFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);
            // 8
            cdoGrnt.AddDetailColumn(String.Format("--[5](1<{0})[4](1<{1})[2](1<{1})", maxDate, hiDate), rubFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // 9
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // служебные
            // 10
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum);
            // 11
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalCurrencySum);
            // 12
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            // 13
            cdoGrnt.AddDetailColumn(String.Format("[0](1<{0})", maxDate), curFields, true);
            // 14
            cdoGrnt.AddDetailColumn(String.Format("--[0](1<{1})[1](1<{0})[2](1<{0})", hiDate, maxDate), curFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumDbt);
            // 15
            cdoGrnt.AddDetailColumn(String.Format("--[5](1<{0})[4](1<{1})[2](1<{1})", maxDate, hiDate), curFields, true);
            cdoGrnt.SetColumnCondition(t_S_FactAttractGrnt.RefTypSum, ReportConsts.GrntTypeSumPct);
            // 16
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            // 17
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, t_S_FactAttractPrGrnt.key);
            // 18
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            // 19
            cdoGrnt.AddDetailTextColumn(String.Format("[3](1>{0})", hiDate), cdoGrnt.ParamFieldList, rubFields);
            // 20
            cdoGrnt.AddDetailTextColumn(String.Format("[3](1<{0})", maxDate), cdoGrnt.ParamFieldList, rubFields);
            // 21
            cdoGrnt.AddDetailTextColumn(String.Format("[3](1>{0})", hiDate), cdoGrnt.ParamFieldList, curFields);
            // 22
            cdoGrnt.AddDetailTextColumn(String.Format("[3](1<{0})", maxDate), cdoGrnt.ParamFieldList, curFields);
            // 23
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, t_S_PlanDebtPrGrnt.key);
            // 24
            cdoGrnt.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoGrnt.ParamOnlyDates, rubFields);
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.Num);
            dtTables[3] = cdoGrnt.FillData();

            for (var i = 0; i < 4; i++)
            {
                SetColumnValue(dtTables[i], 4, "руб.");
            }

            // пересчет сумм для валютных и незаполненных
            foreach (DataRow rowData in dtTables[3].Rows)
            {
                var refOKV = Convert.ToInt32(rowData[12]);
                // сумма по договору и гарантии
                var contractSum = GetNumber(rowData[10]);
                // заменяем на валютные
                if (refOKV != ReportConsts.codeRUB)
                {
                    contractSum = GetNumber(rowData[11]);
                }
                // если не заполнен Факт привлечения, берем значение из суммы по гарантируемому договору
                var factRecCount = Convert.ToInt32(rowData[17]);
                var emptyFactAttr = factRecCount == 0;
                if (emptyFactAttr)
                {
                    rowData[07] = GetNumber(rowData[07]) + contractSum;
                    rowData[13] = GetNumber(rowData[13]) + contractSum;
                    rowData[15] = GetNumber(rowData[15]) + contractSum;
                }
                // для валютных пересчитываем по указанному в параметрах курсу
                if (refOKV != ReportConsts.codeRUB)
                {
                    rowData[5] = Math.Round(exchangeRate * GetNumber(rowData[13]), 2);
                    rowData[7] = Math.Round(exchangeRate * GetNumber(rowData[14]), 2);
                    rowData[8] = Math.Round(exchangeRate * GetNumber(rowData[15]), 2);
                }

                // хитрый алгорит заполнения текстовки плана выплаты гарантии
                var planRecCount = Convert.ToInt32(rowData[23]);

                if (planRecCount > 1)
                {
                    object percentValue;
                    if (planRecCount > 10)
                    {
                        percentValue = refOKV == ReportConsts.codeRUB ? rowData[19] : rowData[21];
                    }
                    else
                    {
                        percentValue = refOKV == ReportConsts.codeRUB ? rowData[20] : rowData[22];
                    }
                    rowData[9] = percentValue;
                }
            }

            dtTables[3] = ClearCloseCreditMO(cdoGrnt, dtTables[3], calcDate, 7, 24, 0);

            var cdoDebtLimit = new CommonDataObject();
            cdoDebtLimit.InitObject(scheme);
            cdoDebtLimit.useSummaryRow = false;
            cdoDebtLimit.ObjectKey = f_S_DebtLimit.internalKey;
            cdoDebtLimit.mainFilter[f_S_DebtLimit.RefYearDayUNV] = String.Format("={0}", 
                Convert.ToInt32(GetUNVYearStart(calcDate.Year + 1)) + 1);
            cdoDebtLimit.mainFilter[f_S_DebtLimit.AcceptDate] = String.Format("<='{0}'", calcDate.ToShortDateString());
            cdoDebtLimit.AddDataColumn(f_S_DebtLimit.StateDebtUpLim);
            cdoDebtLimit.AddDataColumn(f_S_DebtLimit.RefYearDayUNV, ReportConsts.ftInt32);
            cdoDebtLimit.AddDataColumn(f_S_DebtLimit.AcceptDate, ReportConsts.ftDateTime);
            cdoDebtLimit.sortString = StrSortUp(f_S_DebtLimit.AcceptDate);
            var tblDebtLimit = cdoDebtLimit.FillData();

            // заголовочные параметры
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = hiDate;
            drCaption[1] = GetDateText(hiDate);
            drCaption[2] = exchangeRate;
            drCaption[3] = GetParamDate(reportParams, ReportConsts.ParamExchangeDate);
            FillSignatureData(drCaption, 4, reportParams, ReportConsts.ParamExecutor1, ReportConsts.ParamExecutor2);

            foreach (DataRow rowDebtLimit in tblDebtLimit.Rows)
            {
                if (rowDebtLimit[f_S_DebtLimit.StateDebtUpLim] != DBNull.Value)
                {
                    drCaption[25] = rowDebtLimit[f_S_DebtLimit.StateDebtUpLim];
                }
            }

            return dtTables;
        }

        /// <summary>
        /// Сводная информация о долговых обязательствах - Московская обсласть
        /// </summary>
        public DataTable[] GetMOSummaryDebtInfoData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var loDate = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var hiDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exchangeRate1 = GetNumber(reportParams[ReportConsts.ParamStartExchangeRate]);
            var exchangeRate2 = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var rubFields = CreateValuePair(ReportConsts.SumField);
            var curFields = CreateValuePair(ReportConsts.CurrencySumField);
            dtTables[0] = CreateReportCaptionTable(2, 4);
            var yearStart = GetYearStart(DateTime.Now.Year);
            const string dateFilter = ">='{0}' and c.{1} = {2} or c.{1} = {3}";
            // ЦБ
            const string capDebtFormula = "-[0](1<={0})[1](1<={0})";
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCap.AddDetailColumn(String.Format(capDebtFormula, loDate));
            cdoCap.AddDetailColumn(String.Format(capDebtFormula, hiDate));
            var dtCapHi = cdoCap.FillData();
            cdoCap.mainFilter[f_S_Capital.RefVariant] = ReportConsts.FixedVariantsID;
            cdoCap.mainFilter[f_S_Capital.EndDate] = String.Format(dateFilter,
                yearStart, f_S_Capital.RefVariant, ReportConsts.ArchivVariantID, ReportConsts.ActiveVariantID);
            var dtCapLo = cdoCap.FillData();
            var drCapResult = dtTables[0].Rows[0];
            drCapResult[0] = GetLastRowValue(dtCapLo, 0);
            drCapResult[1] = GetLastRowValue(dtCapHi, 1);
            // Кредиты
            const string creditFormula = "+-[0](1<={0})[1](1<={0})[10](0<={0})";
            var cdoCI = new CreditDataObject();
            cdoCI.InitObject(scheme);
            cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.ActiveVariantID;
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoCI.AddDetailColumn(String.Format(creditFormula, loDate));
            cdoCI.AddDetailColumn(String.Format(creditFormula, hiDate));
            var dtBudHiCredit = cdoCI.FillData();
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            var dtOrgHiCredit = cdoCI.FillData();
            cdoCI.mainFilter[f_S_Creditincome.RefVariant] = ReportConsts.FixedVariantsID;
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdoCI.mainFilter[f_S_Creditincome.FactDate] = String.Format(dateFilter,
                yearStart, f_S_Creditincome.RefVariant, ReportConsts.ArchivVariantID, ReportConsts.ActiveVariantID);
            var dtBudLoCredit = cdoCI.FillData();
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            var dtOrgLoCredit = cdoCI.FillData();
            var drBudCreditResult = dtTables[0].Rows[1];
            var drOrgCreditResult = dtTables[0].Rows[2];
            drBudCreditResult[0] = GetLastRowValue(dtBudLoCredit, 0);
            drBudCreditResult[1] = GetLastRowValue(dtBudHiCredit, 1);
            drOrgCreditResult[0] = GetLastRowValue(dtOrgLoCredit, 0);
            drOrgCreditResult[1] = GetLastRowValue(dtOrgHiCredit, 1);
            // Гарантии
            const string garantMainDebt1 = "-+--[0](1<={0})[1](1<={0})[2](1<={0})[5](1<={1})[4](1<={0})";
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.planServiceDate = hiDate;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.ActiveVariantID;
            // 0
            cdoGrnt.AddDetailColumn(String.Format(garantMainDebt1, loDate, maxDate), rubFields, true);
            // 1
            cdoGrnt.AddDetailColumn(String.Format(garantMainDebt1, hiDate, maxDate), rubFields, true);
            // 2
            cdoGrnt.AddDetailColumn(String.Format(garantMainDebt1, loDate, maxDate), curFields, true);
            // 3
            cdoGrnt.AddDetailColumn(String.Format(garantMainDebt1, hiDate, maxDate), curFields, true);
            // 4
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalOKV);
            // 5
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalSum);
            // 6
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalCurrencySum);
            // 7
            cdoGrnt.AddParamColumn(CalcColumnType.cctRecordCount, "0");
            var dtHiGarant = cdoGrnt.FillData();
            dtHiGarant = FillGarantSumMO(dtHiGarant, 0, 2, exchangeRate2, 4, 5, 6, 7);
            dtHiGarant = FillGarantSumMO(dtHiGarant, 1, 3, exchangeRate2, 4, 5, 6, 7);
            dtHiGarant = cdoGrnt.RecalcSummary(dtHiGarant);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = ReportConsts.FixedVariantsID;
            cdoCI.mainFilter[f_S_Guarantissued.EndDate] = String.Format(dateFilter,
                yearStart, f_S_Guarantissued.RefVariant, ReportConsts.ArchivVariantID, ReportConsts.ActiveVariantID);
            var dtLoGarant = cdoGrnt.FillData();
            dtLoGarant = FillGarantSumMO(dtLoGarant, 0, 2, exchangeRate1, 4, 5, 6, 7);
            dtLoGarant = FillGarantSumMO(dtLoGarant, 1, 3, exchangeRate1, 4, 5, 6, 7);
            dtLoGarant = cdoGrnt.RecalcSummary(dtLoGarant);
            var drGarantLoData = GetLastRow(dtLoGarant);
            var drGarantHiData = GetLastRow(dtHiGarant);
            var drGarantResult = dtTables[0].Rows[3];
            drGarantResult[0] = drGarantLoData[0];
            drGarantResult[1] = drGarantHiData[1];
            CorrectBillionSumValue(dtTables[0], 0);
            CorrectBillionSumValue(dtTables[0], 1);
            // заголовочные параметры
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = loDate;
            drCaption[1] = hiDate;
            drCaption[2] = GetDateText(hiDate);
            return dtTables;
        }

        /// <summary>
        /// Выписка из ДК Саратов
        /// </summary>
        public DataTable[] GetExtractDKSaratovData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var cdo = new CreditDataObject();
            InitObjectParamsCI(cdo, calcDate, false);
            cdo.sortString = FormSortString(StrSortUp(TempFieldNames.OrgName), StrSortUp(f_S_Creditincome.StartDate));
            // Формирование колонок
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdo.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            cdo.AddCalcColumn(CalcColumnType.cctPercentValues);
            cdo.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdo.AddDetailColumn(String.Format("[0](1<{0})", calcDate));
            cdo.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            cdo.AddDataColumn(f_S_Creditincome.Purpose);
            cdo.AddDataColumn(f_S_Creditincome.StartDate, ReportConsts.ftDateTime);
            // Бюдгеты
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var dtBudCredit = cdo.FillData();
            // Организации
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            cdo.useSummaryRow = false;
            dtTables[0] = cdo.FillData();
            
            foreach (var index in cdo.summaryColumnIndex)
            {
                CorrectThousandSumValue(dtTables[0], index);
            }

            var minDate = DateTime.MinValue;
            var maxYear = 0;

            foreach (DataRow row in dtBudCredit.Rows)
            {
                if (row[3] != DBNull.Value)
                    maxYear = Math.Max(maxYear, Convert.ToDateTime(row[3]).Year);
                if (row[7] != DBNull.Value && minDate < Convert.ToDateTime(row[7]))
                    minDate = Convert.ToDateTime(row[7]);
            }

            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            SetGarantFilter(cdoGrnt.mainFilter, calcDate, calcDate);
            cdoGrnt.sortString = FormSortString(StrSortUp(TempFieldNames.OrgName), StrSortUp(f_S_Guarantissued.StartDate));
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            cdoGrnt.useSummaryRow = false;
            dtTables[1] = cdoGrnt.FillData();
            
            foreach (var index in cdoGrnt.summaryColumnIndex)
            {
                CorrectThousandSumValue(dtTables[1], index);
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = GetBookValue(cdo.scheme, d_Organizations_Plan.internalKey, reportParams[ReportConsts.ParamOrgID]);
            drCaption[2] = minDate.ToShortDateString();
            drCaption[3] = maxYear;
            drCaption[4] = ConvertTo1000(GetLastRowValue(dtBudCredit, 5));
            return dtTables;
        }

        /// <summary>
        /// Курсовая разница Самара
        /// </summary>
        public DataTable[] GetRateSwitchSamaraData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var dtData = new DataTable[4];
            var loDate = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var hiDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var dateYearStart = GetYearStart(hiDate);
            var sumFieldName = CreateValuePair(ReportConsts.CurrencySumField);
            // Заголовочная таблица
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(5);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = loDate;
            drCaption[1] = hiDate;
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.useSummaryRow = false;
            SetGarantFilter(cdoGrnt.mainFilter, hiDate, dateYearStart);
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+5;-4");
            cdoGrnt.AddExchangeColumn(loDate);
            cdoGrnt.AddExchangeColumn(hiDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            cdoGrnt.AddDetailColumn(String.Format("[5](1<{0})", loDate), sumFieldName, true);
            cdoGrnt.AddDataColumn(ReportConsts.CurrencySumField);
            dtData[3] = cdoGrnt.FillData();
            // ЦБ
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.useSummaryRow = false;
            SetCapitalFilter(cdoCap.mainFilter, hiDate, dateYearStart);
            cdoCap.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", loDate), sumFieldName, true);
            cdoCap.AddExchangeColumn(loDate);
            cdoCap.AddExchangeColumn(hiDate);
            cdoCap.AddCalcColumn(CalcColumnType.cctOKVName);
            dtData[0] = cdoCap.FillData();
            //Кредиты
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.useSummaryRow = false;
            SetCreditFilter(cdo.mainFilter, hiDate, dateYearStart);
            cdo.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", loDate), sumFieldName, true);
            cdo.AddExchangeColumn(loDate);
            cdo.AddExchangeColumn(hiDate);
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtData[2] = cdo.FillData();
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtData[1] = cdo.FillData();
            // Создаем структуру для подложки в отчет
            dtTables[0] = CreateReportCaptionTable(5);
            
            for (var i = 0; i < 4; i++)
            {
                dtTables[0].Rows.Add();
            }
            
            // Заполняем данные
            var totalSummary = new decimal[4];

            for (var i = 0; i < 4; i++)
            {
                var drsSelect = dtData[i].Select(String.Format("{0} <> {1}", f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr));
                var summary = new decimal[4];

                for (var j = 0; j < 4; j++)
                {
                    summary[j] = 0;
                }

                foreach (DataRow t in drsSelect)
                {
                    // Курсы для заголовка
                    drCaption[2] = t[3];

                    if (t[1] != DBNull.Value)
                    {
                        drCaption[3] = t[1].ToString().Split('(')[0];
                    }

                    if (t[2] != DBNull.Value)
                    {
                        drCaption[4] = t[2].ToString().Split('(')[0];
                    }

                    // Итоги по типу документов
                    summary[0] += Convert.ToDecimal(t[0]);

                    if (t[1] != DBNull.Value)
                    {
                        var exchangeRate1 = Convert.ToDecimal(t[1].ToString().Split('(')[0]);
                        summary[1] += exchangeRate1 * Convert.ToDecimal(t[0]);
                    }

                    if (t[2] != DBNull.Value)
                    {
                        var exchangeRate2 = Convert.ToDecimal(t[2].ToString().Split('(')[0]);
                        summary[2] += exchangeRate2 * Convert.ToDecimal(t[0]);
                    }

                    summary[3] += Convert.ToDecimal(summary[2]) - Convert.ToDecimal(summary[1]);
                }

                // Записываем в результат
                for (int j = 0; j < 4; j++)
                {
                    var drResult = dtTables[0].Rows[j];
                    totalSummary[j] += summary[j];
                    drResult[i + 1] = summary[j];
                    drResult[0] = totalSummary[j];
                }
            }
            return dtTables;
        }

        /// <summary>
        /// Свод Самара
        /// </summary>
        public DataTable[] GetVaultSamaraData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            dtTables[0] = CreateReportCaptionTable(8);
            var drCaption = dtTables[0].Rows.Add();
            var hiDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            dtTables[1] = GetCapitalSamaraData(reportParams, drCaption);
            dtTables[2] = GetGarantSamaraData(reportParams, drCaption);
            dtTables[3] = GetCreditDataSamara(hiDate, ReportConsts.BudCreditCode, drCaption);
            dtTables[4] = GetCreditDataSamara(hiDate, ReportConsts.OrgCreditCode, drCaption);
            // Заголовочная таблица
            drCaption[1] = GetLastRowValue(dtTables[1], 19);
            drCaption[2] = GetLastRowValue(dtTables[3], 22);
            drCaption[3] = GetLastRowValue(dtTables[4], 22);
            drCaption[4] = GetLastRowValue(dtTables[2], 25);
            drCaption[0] =
                Convert.ToDecimal(drCaption[1]) + Convert.ToDecimal(drCaption[2]) +
                Convert.ToDecimal(drCaption[3]) + Convert.ToDecimal(drCaption[4]);
            drCaption[5] = hiDate;
            return dtTables;
        }

        /// <summary>
        /// Программа государственных внутренних заимствований Самарской области
        /// </summary>
        public DataTable[] GetBorrowingProgrammSamaraData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            var dtData = new DataTable[2];
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var loDate = GetYearStart(year);
            var hiDate = GetYearEnd(year);
            var dateYearStart = GetYearStart(hiDate);
            // ЦБ RegNumber
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            SetCapitalFilter(cdoCap.mainFilter, hiDate, dateYearStart);
            cdoCap.mainFilter[f_S_Capital.RefVariant] =
                Combine(cdoCap.mainFilter[f_S_Capital.RefVariant], reportParams[ReportConsts.ParamVariantID]);
            cdoCap.AddDataColumn(f_S_Capital.RegNumber);
            cdoCap.AddDetailColumn(String.Format("[5](1>={0}1<={1})", loDate, hiDate));
            cdoCap.AddDetailColumn(String.Format("[6](1>={0}1<={1})", loDate, hiDate));
            dtData[1] = cdoCap.FillData();
            // Кредиты Purpose
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            SetCreditFilter(cdo.mainFilter, hiDate, dateYearStart);
            cdo.mainFilter[f_S_Creditincome.RefVariant] =
                Combine(cdo.mainFilter[f_S_Creditincome.RefVariant], reportParams[ReportConsts.ParamVariantID]);
            cdo.AddDataColumn(f_S_Creditincome.Purpose);
            cdo.AddDetailColumn(String.Format("[3](1>={0}1<={1})", loDate, hiDate));
            cdo.AddDetailColumn(String.Format("[2](1>={0}1<={1})", loDate, hiDate));
            cdo.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtData[0] = cdo.FillData();
            
            // Группируем по цели принятия
            for (var i = 0; i < 2; i++)
            {
                dtTables[i] = CreateReportCaptionTable(4);
                GroupProgrammSamaraData(dtTables[i], dtData[i], 0, i);
            }
            
            // Заголовочная таблица
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(3);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = year;
            var dr1 = dtTables[0].Rows[0];
            var dr2 = dtTables[1].Rows[0];
            drCaption[1] = Convert.ToDecimal(dr1[2]) + Convert.ToDecimal(dr2[2]);
            drCaption[2] = Convert.ToDecimal(dr1[3]) + Convert.ToDecimal(dr2[3]);
            // Это конец!
            return dtTables;
        }

        /// <summary>
        /// Свод Самара
        /// </summary>
        public DataTable[] GetVaultDKSamaraData(Dictionary<string, string> reportParams)
        {
            var sumFieldName = CreateValuePair(ReportConsts.CurrencySumField);
            var dtTables = new DataTable[7];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            // ЦБ
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.exchangePrevDay = true;
            cdoCap.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            SetCapitalFilter(cdoCap.mainFilter, calcDate, calcDate);
            // 0
            cdoCap.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoCap.AddCalcColumn(CalcColumnType.cctCapOffNumNPANameDateNum);
            // 2
            cdoCap.AddCalcColumn(CalcColumnType.cctCapLabelPurpose);
            // 3
            cdoCap.AddDataColumn(f_S_Capital.NameOrg);
            // 4
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
            // 5
            cdoCap.AddCalcColumn(CalcColumnType.cctPercentValues);
            // 6
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+8;%7");
            // 7
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            // 8
            cdoCap.AddDetailColumn(String.Format("[5](1<={0})", maxDate));
            // 9 
            cdoCap.AddDetailColumn(String.Format("+[0](1<={0})[7](1<={1})", calcDate, maxDate));
            // 10
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+12;%11");
            // 11
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            // 12
            cdoCap.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", calcDate));
            // 13
            cdoCap.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", calcDate));
            dtTables[1] = cdoCap.FillData();
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.exchangePrevDay = true;
            cdoGrnt.summaryColumnIndex.Add(5);
            cdoGrnt.summaryColumnIndex.Add(6);
            
            for (var i = 0; i < 4; i++)
            {
                cdoGrnt.summaryColumnIndex.Add(09 + i * 2);
                cdoGrnt.summaryColumnIndex.Add(10 + i * 2);
            }
            
            cdoGrnt.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            SetGarantFilter(cdoGrnt.mainFilter, calcDate, calcDate);
            // 0
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate2);
            // 2
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization);
            // 3
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            // 4
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            // 5
            cdoGrnt.AddDataColumn(ReportConsts.CurrencySumField);
            // 6
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCalcSum);
            // 7 
            cdoGrnt.AddDataColumn(f_S_Guarantissued.PercentLiability);
            // 8 
            cdoGrnt.AddDataColumn(f_S_Guarantissued.SanctionLiability);
            // 9
            cdoGrnt.AddDataColumn(ReportConsts.CurrencySumField);
            // 10
            cdoGrnt.AddDataColumn(ReportConsts.SumField);
            // 11
            cdoGrnt.AddDataColumn(ReportConsts.CurrencySumField);
            // 12
            cdoGrnt.AddDataColumn(ReportConsts.SumField);
            // 13
            cdoGrnt.AddDataColumn(ReportConsts.CurrencySumField);
            // 14
            cdoGrnt.AddDataColumn(ReportConsts.SumField);
            // 15
            cdoGrnt.AddDataColumn(ReportConsts.CurrencySumField);
            // 16 
            cdoGrnt.AddDataColumn(ReportConsts.SumField);
            // 17 - служебное
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={0})", calcDate));
            // 18 - служебное
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={0})", calcDate), sumFieldName, true);
            // 19  - служебное
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+5;-18");
            // 20  - служебное в миллионах
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+6;-17");
            // 21
            cdoGrnt.AddDataColumn(f_S_Guarantissued.IsDepreciation);
            dtTables[2] = cdoGrnt.FillData();
            foreach (DataRow dr in dtTables[2].Rows)
            {
                if (GetBoolValue(dr[21]))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        dr[09 + i * 2] = dr[19];
                        dr[10 + i * 2] = dr[20];
                    }
                }
            }
            CorrectBillionSumValue(dtTables[2], 06);
            for (int i = 0; i < 2; i++)
            {
                CorrectBillionSumValue(dtTables[2], 10 + i * 2);
            }
            dtTables[2] = cdoGrnt.RecalcSummary(dtTables[2]);
            // Кредиты минфина
            var cdo1 = new CreditDataObject();
            cdo1.InitObject(scheme);
            cdo1.exchangePrevDay = true;
            cdo1.summaryColumnIndex.Add(5);
            cdo1.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            SetCreditFilter(cdo1.mainFilter, calcDate, calcDate);
            // 0
            cdo1.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdo1.AddCalcColumn(CalcColumnType.cctNumContractDate);
            // 2
            cdo1.AddCalcColumn(CalcColumnType.cctOrgPurpose);
            // 3
            cdo1.AddDataColumn(f_S_Creditincome.EndDate);
            // 4
            cdo1.AddCalcColumn(CalcColumnType.cctPercentValues);
            // 5
            cdo1.AddDataColumn(ReportConsts.SumField);
            // 6 !
            cdo1.AddParamColumn(CalcColumnType.cctRelation, "+5;+9");
            // 7
            cdo1.AddDetailColumn(String.Format("[0](1<={0})", calcDate));
            // 8
            cdo1.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", calcDate));
            // 9 - служебное
            cdo1.AddDetailColumn(String.Format("[5](1<={0})", maxDate));
            cdo1.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            cdo1.mainFilter[f_S_Creditincome.RefOKV] = ReportConsts.codeRUBStr;
            dtTables[3] = cdo1.FillData();
            // Субзаймы минфина 
            var cdo2 = new CreditDataObject();
            cdo2.InitObject(scheme);
            cdo2.exchangePrevDay = true;
            cdo2.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            SetCreditFilter(cdo2.mainFilter, calcDate, calcDate);
            // 0
            cdo2.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdo2.AddCalcColumn(CalcColumnType.cctNumContractDate);
            // 2
            cdo2.AddCalcColumn(CalcColumnType.cctSubCreditCaption);
            // 3
            cdo2.AddDataColumn(f_S_Creditincome.EndDate);
            // 4
            cdo2.AddCalcColumn(CalcColumnType.cctPercentValues);
            // 5
            cdo2.AddDetailColumn(String.Format("[3](1<={0})", calcDate), sumFieldName, true);
            // 6
            cdo2.AddDetailColumn(String.Format("[3](1<={0})", calcDate));
            // 7
            cdo2.AddDetailColumn(String.Format("[0](1<={0})", calcDate), sumFieldName, true);
            // 8
            cdo2.AddDetailColumn(String.Format("[0](1<={0})", calcDate));
            // 9
            cdo2.AddDetailColumn(String.Format("[2](1<={0})", maxDate), sumFieldName, true);
            // 10
            cdo2.AddDetailColumn(String.Format("[2](1<={0})", maxDate));
            // 11
            cdo2.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={1})", maxDate, calcDate), sumFieldName, true);
            // 12
            cdo2.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={1})", maxDate, calcDate));
            // 13
            cdo2.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={1})", maxDate, calcDate), sumFieldName, true);
            // 14
            cdo2.AddDetailColumn(String.Format("-[2](1<={0})[1](1<={1})", maxDate, calcDate));
            cdo2.mainFilter[f_S_Creditincome.RefOKV] = FormNegFilterValue(ReportConsts.codeRUBStr);
            cdo2.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[4] = cdo2.FillData();
            for (int i = 0; i < 4; i++)
            {
                CorrectBillionSumValue(dtTables[4], 6 + i * 2);
            }
            dtTables[4].Columns.RemoveAt(4);
            dtTables[4].Columns.RemoveAt(1);
            cdo2.mainFilter.Remove(f_S_Creditincome.RefOKV);
            cdo2.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtTables[5] = cdo2.FillData();
            for (int i = 0; i < 5; i++)
            {
                CorrectBillionSumValue(dtTables[5], 6 + i * 2);
            }
            dtTables[0] = CreateReportCaptionTable(5);
            // Общие суммы
            DataRow drTotal = dtTables[0].Rows.Add();
            drTotal[1] = GetLastRowValue(dtTables[2], 20);
            drTotal[2] = GetLastRowValue(dtTables[5], 14);
            drTotal[3] = GetLastRowValue(dtTables[3], 8) + GetLastRowValue(dtTables[4], 12);
            drTotal[4] = GetLastRowValue(dtTables[1], 13);
            drTotal[0] =
                Convert.ToDecimal(drTotal[1]) + Convert.ToDecimal(drTotal[2]) +
                Convert.ToDecimal(drTotal[3]) + Convert.ToDecimal(drTotal[4]);
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(6);
            DataRow drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = calcDate;
            drCaption[1] = 0;
            const int codeUSD = ReportConsts.codeUSD;
            if (cdoGrnt.okvValues.ContainsKey(codeUSD)) drCaption[1] = GetUSDValue(cdoGrnt);
            if (cdo1.okvValues.ContainsKey(codeUSD)) drCaption[1] = GetUSDValue(cdo1);
            if (cdo2.okvValues.ContainsKey(codeUSD)) drCaption[1] = GetUSDValue(cdo2);
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Программа заимствований Ярославль"
        /// </summary>
        public DataTable[] GetBorrowingProgrammYarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[7];
            var year = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            decimal exchangeRate = 0;
            
            if (reportParams[ReportConsts.ParamExchangeRate].Length > 0)
            {
                exchangeRate = Convert.ToDecimal(reportParams[ReportConsts.ParamExchangeRate]);
            }

            string yearLo;
            string yearHi;
            var currencySumField = CreateValuePair(ReportConsts.CurrencySumField);
            var nowStr = DateTime.Now.ToShortDateString();
            // 1 таблица
            dtTables[0] = CreateReportCaptionTable(12, 2);
            var fixedNumbers = new Collection<string> { "'01-01-06/07-775'", "'01-01-06/04-170'" };
            var counter = 0;

            foreach (var fixedNum in fixedNumbers)
            {
                var cdo = new CreditDataObject();
                cdo.InitObject(scheme);
                cdo.useSummaryRow = false;
                cdo.mainFilter.Add(f_S_Creditincome.Num, fixedNum);
                
                for (var i = 0; i < 3; i++)
                {
                    yearLo = GetYearStart(year + i);
                    yearHi = GetYearEnd(year + i);
                    cdo.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearLo, yearHi), currencySumField, true);
                    cdo.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearLo, yearHi), currencySumField, true);
                }

                for (var i = 0; i < 3; i++)
                {
                    yearLo = GetYearStart(year + i);
                    yearHi = GetYearEnd(year + i);
                    cdo.AddDetailColumn(String.Format("[5](1>={0}1<={1})", yearLo, yearHi), currencySumField, true);
                    cdo.AddDetailColumn(String.Format("[5](1>={0}1<={1})", yearLo, yearHi), currencySumField, true);
                }

                var dtData = cdo.FillData();

                for (var i = 0; i < 12; i++)
                {
                    CorrectThousandSumValue(dtData, i);
                }

                foreach (DataRow dr in dtData.Rows)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        dr[1 + i * 2] = exchangeRate * Math.Round(Convert.ToDecimal(dr[1 + i * 2]), 1);
                    }
                }

                if (dtData.Rows.Count > 0)
                {
                    for (var i = 0; i < 12; i++)
                    {
                        dtTables[0].Rows[counter][i] = dtData.Rows[0][i];
                    }
                }
                counter++;
            }

            // 3 таблица
            dtTables[2] = CreateReportCaptionTable(3, 22);
            var cdoCredit = new CreditDataObject();
            
            // бюджетные
            for (var i = 0; i < 3; i++)
            {
                yearLo = GetYearStart(year + i);
                yearHi = GetYearEnd(year + i);
                cdoCredit.InitObject(scheme);
                cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = Combine(
                    ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
                cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
                cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "0,5";
                cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = ReportConsts.codeRUBStr;
                cdoCredit.AddDetailColumn(String.Format("[3](0>={0}0<={1})", yearLo, yearHi));
                cdoCredit.AddDetailColumn(String.Format("[3](0>={0}0<={1})", yearLo, yearHi), currencySumField, true);
                cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearLo, yearHi));
                cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearLo, yearHi), currencySumField, true);
                var dtData1 = cdoCredit.FillData();
                cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = FormNegFilterValue(ReportConsts.codeRUBStr);
                var dtData2 = cdoCredit.FillData();

                for (var j = 0; j < 4; j++)
                {
                    CorrectThousandSumValue(dtData1, j);
                    CorrectThousandSumValue(dtData2, j);
                }

                var sum1 = GetLastRowValue(dtData1, 0);
                var sum2 = exchangeRate * Math.Round(GetLastRowValue(dtData2, 1), 1);
                var sum3 = GetLastRowValue(dtData1, 2);
                var sum4 = exchangeRate * Math.Round(GetLastRowValue(dtData2, 3), 1);
                dtTables[2].Rows[1][i] = sum1 + sum2 - sum3 - sum4;
                dtTables[2].Rows[2][i] = sum1 + sum2;
                dtTables[2].Rows[3][i] = sum3 + sum4;
                dtTables[2].Rows[4][i] = sum3;
                dtTables[2].Rows[5][i] = sum4;
            }
            // коммерческие
            cdoCredit.InitObject(scheme);
            cdoCredit.removeServiceFields = true;
            cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "0,5";
            cdoCredit.AddCalcColumn(CalcColumnType.cctGenOrg);
            
            for (var i = 0; i < 3; i++)
            {
                yearLo = GetYearStart(year + i);
                yearHi = GetYearEnd(year + i);
                cdoCredit.AddDetailColumn(String.Format("[3](0>={0}0<={1})", yearLo, yearHi));
                cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearLo, yearHi));
            }

            var dtDataOrg = cdoCredit.FillData();
           
            for (var i = 0; i < 3; i++)
            {
                CorrectThousandSumValue(dtDataOrg, i * 2 + 1);
                CorrectThousandSumValue(dtDataOrg, i * 2 + 2);
                var sum1 = GetLastRowValue(dtDataOrg, 1 + i * 2);
                var sum2 = GetLastRowValue(dtDataOrg, 2 + i * 2);
                dtTables[2].Rows[6][i] = sum1 - sum2;
                dtTables[2].Rows[7][i] = sum1;
                dtTables[2].Rows[8][i] = sum2;
            }

            for (var i = 0; i < 3; i++)
            {
                dtDataOrg.Columns.RemoveAt(5 - i * 2);
            }

            dtTables[5] = CommonGroupDataSet(dtDataOrg, 0, false);
            // 9 строку пропустим чтобы туда организаций потом напихать
            // ЦБ
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.removeServiceFields = true;
            cdoCap.mainFilter[f_S_Capital.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            cdoCap.mainFilter[f_S_Capital.RefStatusPlan] = "1";

            for (var i = 0; i < 3; i++)
            {
                yearLo = GetYearStart(year + i);
                yearHi = GetYearEnd(year + i);
                cdoCap.AddDetailColumn(String.Format("[5](0>={0}0<={1})", yearLo, yearHi));
                cdoCap.AddDetailColumn(String.Format("[6](1>={0}1<={1})", yearLo, yearHi));
            }

            var dtCapData = cdoCap.FillData();
            for (var i = 0; i < 3; i++)
            {
                CorrectThousandSumValue(dtCapData, i * 2 + 0);
                CorrectThousandSumValue(dtCapData, i * 2 + 1);
                var sum1 = GetLastRowValue(dtCapData, 0 + i * 2);
                var sum2 = GetLastRowValue(dtCapData, 1 + i * 2);
                dtTables[2].Rows[10][i] = sum1 - sum2;
                dtTables[2].Rows[11][i] = sum1;
                dtTables[2].Rows[12][i] = sum2;
            }

            // 13 пустая строчка итого
            for (var i = 0; i < 3; i++)
            {
                dtTables[2].Rows[14][i] =
                    Convert.ToDecimal(dtTables[2].Rows[2][i]) + Convert.ToDecimal(dtTables[2].Rows[7][i])
                    + Convert.ToDecimal(dtTables[2].Rows[11][i]);
                dtTables[2].Rows[15][i] =
                    Convert.ToDecimal(dtTables[2].Rows[3][i]) + Convert.ToDecimal(dtTables[2].Rows[8][i])
                    + Convert.ToDecimal(dtTables[2].Rows[12][i]);
                dtTables[2].Rows[16][i] =
                    Convert.ToDecimal(dtTables[2].Rows[14][i]) - Convert.ToDecimal(dtTables[2].Rows[15][i]);
                dtTables[2].Rows[17][i] = dtTables[2].Rows[16][i];
            }

            // 18 одна пустая строчка итого
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.removeServiceFields = true;
            cdoGrnt.mainFilter[f_S_Guarantissued.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            cdoGrnt.mainFilter[f_S_Guarantissued.RefSStatusPlan] = "0";
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctCreditYear, TempFieldNames.CreditStartYear);
            
            for (var i = 0; i < 3; i++)
            {
                yearLo = GetYearStart(year + i);
                yearHi = GetYearEnd(year + i);
                cdoGrnt.AddDataColumn(ReportConsts.SumField);
                cdoGrnt.AddDetailColumn(String.Format("[6](0>={0}0<={1})", yearLo, yearHi));
                cdoGrnt.summaryColumnIndex.Add(i * 2 + 1);
                cdoGrnt.columnCondition.Add(i * 2 + 1, 
                    FormFilterValue(TempFieldNames.CreditStartYear, Convert.ToString(year + i)));
            }

            var dtGrntData = cdoGrnt.FillData();

            for (var i = 0; i < 3; i++)
            {
                CorrectThousandSumValue(dtGrntData, i * 2 + 1);
                CorrectThousandSumValue(dtGrntData, i * 2 + 2);
                var sum1 = GetLastRowValue(dtGrntData, 1 + i * 2);
                var sum2 = GetLastRowValue(dtGrntData, 2 + i * 2);
                dtTables[2].Rows[19][i] = sum1;
                dtTables[2].Rows[20][i] = sum2;
            }
            // 4 таблица
            // организации
            dtTables[3] = CreateReportCaptionTable(4, 22);
            cdoCredit.InitObject(scheme);
            cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "0,4,5";
            
            for (var i = 0; i < 4; i++)
            {
                cdoCredit.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", nowStr));
                cdoCredit.AddDetailColumn(String.Format("-[3](0>{0}0<{1})[2](1>{0}1<{1})", nowStr, GetYearStart(year + i)));
            }

            var dt3CreditOrg1 = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "0,5";
            var dt3CreditOrg2 = cdoCredit.FillData();

            for (var i = 0; i < 4; i++)
            {
                CorrectThousandSumValue(dt3CreditOrg1, i * 2 + 0);
                CorrectThousandSumValue(dt3CreditOrg1, i * 2 + 1);
                CorrectThousandSumValue(dt3CreditOrg2, i * 2 + 0);
                CorrectThousandSumValue(dt3CreditOrg2, i * 2 + 1);
                var sum1 = GetLastRowValue(dt3CreditOrg1, i * 2 + 0);
                var sum2 = GetLastRowValue(dt3CreditOrg2, i * 2 + 1);
                dtTables[3].Rows[0][i] = sum1 + sum2;
            }

            // цб
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter[f_S_Capital.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            
            for (var i = 0; i < 4; i++)
            {
                cdoCap.AddDetailColumn(String.Format("-[5](0<={0})[6](1<={0})", nowStr));
                cdoCap.AddDetailColumn(String.Format("-[5](0>{0}0<{1})[6](1>{0}1<{1})", nowStr, GetYearStart(year + i)));
            }

            cdoCap.mainFilter[f_S_Capital.RefStatusPlan] = "1,4";
            var dt3Capital1 = cdoCap.FillData();
            cdoCap.mainFilter[f_S_Capital.RefStatusPlan] = "1";
            var dt3Capital2 = cdoCap.FillData();

            for (var i = 0; i < 4; i++)
            {
                CorrectThousandSumValue(dt3Capital1, i * 2 + 0);
                CorrectThousandSumValue(dt3Capital2, i * 2 + 1);
                var sum1 = GetLastRowValue(dt3Capital1, i * 2 + 0);
                var sum2 = GetLastRowValue(dt3Capital2, i * 2 + 1);
                dtTables[3].Rows[1][i] = sum1 + sum2;
            }
            // бюджетные
            cdoCredit.InitObject(scheme);
            cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            
            for (var i = 0; i < 4; i++)
            {
                cdoCredit.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", nowStr));
                cdoCredit.AddDetailColumn(String.Format("-[3](0>{0}0<{1})[2](1>{0}1<{1})", nowStr, GetYearStart(year + i)));
                cdoCredit.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", nowStr), currencySumField, true);
                cdoCredit.AddDetailColumn(String.Format("-[3](0>{0}0<{1})[2](1>{0}1<{1})", nowStr, GetYearStart(year + i)), 
                    currencySumField, true);
            }

            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "0,4,5";
            cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = ReportConsts.codeRUBStr;
            var dt3CreditBudRub1 = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = FormNegFilterValue(ReportConsts.codeRUBStr);
            var dt3CreditBudVal1 = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "0,5";
            cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = ReportConsts.codeRUBStr;
            var dt3CreditBudRub2 = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = FormNegFilterValue(ReportConsts.codeRUBStr);
            var dt3CreditBudVal2 = cdoCredit.FillData();

            for (var i = 0; i < 4; i++)
            {
                CorrectThousandSumValue(dt3CreditBudRub1, i * 4 + 0);
                CorrectThousandSumValue(dt3CreditBudRub2, i * 4 + 1);
                CorrectThousandSumValue(dt3CreditBudVal1, i * 4 + 2);
                CorrectThousandSumValue(dt3CreditBudVal2, i * 4 + 3);
                // факты рублевые
                var sum11 = GetLastRowValue(dt3CreditBudRub1, 0 + i * 4);
                // факты валютные
                var sum12 = exchangeRate * Math.Round(GetLastRowValue(dt3CreditBudVal1, 2 + i * 4), 1);
                // планы рублевые
                var sum21 = GetLastRowValue(dt3CreditBudRub2, 1 + i * 4);
                // планы валютные
                var sum22 = exchangeRate * Math.Round(GetLastRowValue(dt3CreditBudVal2, 3 + i * 4), 1);
                dtTables[3].Rows[2][i] = sum11 + sum12 + sum21 + sum22;
                dtTables[3].Rows[3][i] = sum11 + sum21;
                dtTables[3].Rows[4][i] = sum12 + sum22;
            }

            var dtGrntPlan = GetGarantProgrammYarData(reportParams);

            for (var i = 0; i < 4; i++)
            {
                dtTables[3].Rows[5][i] =
                    Convert.ToDecimal(dtTables[3].Rows[2][i]) +
                    Convert.ToDecimal(dtTables[3].Rows[0][i]) +
                    Convert.ToDecimal(dtTables[3].Rows[1][i]);

                dtTables[3].Rows[6][i] = GetLastRow(dtGrntPlan[1])[i + 1];
                dtTables[3].Rows[7][i] = Convert.ToDecimal(dtTables[3].Rows[5][i]) +
                    Convert.ToDecimal(dtTables[3].Rows[6][i]);
            }

            // 5 таблица
            dtTables[4] = CreateReportCaptionTable(4, 4);
            var rowIndexes = new Collection<int> { 0, 2, 1, 6} ;

            for (var i = 0; i < rowIndexes.Count; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (Convert.ToDecimal(dtTables[3].Rows[7][j]) != 0)
                    {
                        dtTables[4].Rows[i][j] = 100 * Convert.ToDecimal(dtTables[3].Rows[rowIndexes[i]][j]) /
                            Convert.ToDecimal(dtTables[3].Rows[7][j]);
                    }
                }
            }
            
            // 2 таблица
            dtTables[1] = CreateReportCaptionTable(3, 6);
            // кредиты
            cdoCredit.InitObject(scheme);
            cdoCredit.mainFilter[f_S_Creditincome.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.AllCreditsCode;
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "0,5";
            
            for (var i = 0; i < 4; i++)
            {
                cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})",
                    GetYearStart(year + i), GetYearEnd(year + i)));
                cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})",
                    GetYearStart(year + i), GetYearEnd(year + i)), currencySumField, true);
            }

            cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = ReportConsts.codeRUBStr;
            var dt1CreditRub = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefOKV] = FormNegFilterValue(ReportConsts.codeRUBStr);
            var dt1CreditVal = cdoCredit.FillData();
            
            for (var i = 0; i < 4; i++)
            {
                CorrectThousandSumValue(dt1CreditRub, i);
                CorrectThousandSumValue(dt1CreditVal, i);
            }
            
            // цб
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter[f_S_Capital.RefVariant] = Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]);
            cdoCap.mainFilter[f_S_Capital.RefStatusPlan] = "1";
            
            for (var i = 0; i < 4; i++)
            {
                cdoCap.AddDetailColumn(String.Format("[7](1>={0}1<={1})",
                    GetYearStart(year + i), GetYearEnd(year + i)));
            }
            
            var dt1Capital = cdoCap.FillData();
            
            for (var i = 0; i < 4; i++)
            {
                CorrectThousandSumValue(dt1Capital, i);
            }
            
            for (var i = 0; i < 3; i++)
            {
                dtTables[1].Rows[0][i] = dtTables[3].Rows[7][i + 1];
                dtTables[1].Rows[1][i] = dtTables[3].Rows[6][i + 1];
                dtTables[1].Rows[2][i] = 13000000;
                var sum1 = GetLastRowValue(dt1Capital, i);
                var sum2 = GetLastRowValue(dt1CreditRub, i * 2 + 0);
                var sum3 = exchangeRate * Math.Round(GetLastRowValue(dt1CreditVal, i * 2 + 1), 1);
                dtTables[1].Rows[3][i] = sum1 + sum2 + sum3;
                dtTables[1].Rows[4][i] = dtTables[2].Rows[14][i];
                dtTables[1].Rows[5][i] = dtTables[2].Rows[19][i];
            }
            
            dtTables[1].Rows.InsertAt(dtTables[1].NewRow(), 2);
            dtTables[6] = CreateReportCaptionTable(5, 1);
            var drCaption = GetLastRow(dtTables[6]);
            drCaption[0] = year + 0;
            drCaption[1] = year + 1;
            drCaption[2] = year + 2;
            drCaption[3] = year + 3;
            drCaption[4] = exchangeRate;
            return dtTables;
        }

        /// <summary>
        /// Приложение 1. Отчет о состоянии государственного долга краткий
        /// </summary>
        public DataTable[] GetStateDebtApplication1YarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[6];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var year = reportDate.Year;
            var dateMonthPlanStr = GetMonthStart(reportDate);
            var dateMonthPlanEnd = GetMonthEnd(reportDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var dateMonthFactStr = GetMonthStart(reportDate.AddMonths(-1));
            var dateMonthFactEnd = GetMonthEnd(reportDate.AddMonths(-1));
            var checkList = new Collection<int> { 3, 4, 5, 6, 7, 20 };
            var rubFields = CreateValuePair(ReportConsts.SumField);
            var curFields = CreateValuePair(ReportConsts.CurrencySumField);
            // Кредиты
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.ignoreZeroCurrencyRows = true;
            cdoCredit.reportParams[ReportConsts.ParamHiDate] = calcDate;
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdoCredit.mainFilter.Add(f_S_Creditincome.StartDate, GetFilterStartDateYar(f_S_Creditincome.StartDate, calcDate));
            cdoCredit.mainFilter.Add(f_S_Creditincome.EndDate,
                GetFilterEndDateYar(f_S_Creditincome.EndDate, f_S_Creditincome.RenewalDate, calcDate));
            // 00
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditNumPercent);
            // 01
            cdoCredit.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            // 02
            cdoCredit.AddDataColumn(f_S_Creditincome.Sum);
            // 03
            cdoCredit.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate), rubFields, true);
            // 04
            cdoCredit.AddDetailColumn(String.Format("[2](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd));
            // 05
            cdoCredit.AddDetailColumn(String.Format("[5](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd));
            // 06
            cdoCredit.AddDetailColumn(String.Format("+[1](1<={1}1>={0})[4](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd), rubFields, true);
            // 07
            cdoCredit.AddDetailColumn(String.Format("+[8](1<={1}1>={0})[9](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd));
            // 08
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // 09
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.RowType);
            // 10
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 11
            cdoCredit.AddDataColumn(f_S_Creditincome.RefOrganizations);
            // служебные
            // 12
            cdoCredit.AddDataColumn(f_S_Creditincome.RefOKV, ReportConsts.ftInt32);
            // 13
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctOKVName, TempFieldNames.OKVShortName);
            // 14
            cdoCredit.AddDataColumn(f_S_Creditincome.CurrencySum);
            // 15
            cdoCredit.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate), curFields, true);
            // 16
            cdoCredit.AddDetailColumn(String.Format("[2](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd), curFields, true);
            // 17
            cdoCredit.AddDetailColumn(String.Format("[5](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd), curFields, true);
            // 18
            cdoCredit.AddDetailColumn(String.Format("+[1](1<={1}1>={0})[4](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd), curFields, true);
            // 19
            cdoCredit.AddDetailTextColumn(String.Format("[2](1>={0}1<={1})", calcDate, maxDate), cdoCredit.ParamOnlyDates, String.Empty);
            // 20
            cdoCredit.AddDetailColumn(String.Format("[5](1>{0})", calcDate));

            cdoCredit.sortString = FormSortString(
                StrSortUp(f_S_Creditincome.RefOKV), 
                StrSortUp(TempFieldNames.OrgName),
                StrSortUp(f_S_Creditincome.ContractDate));

            cdoCredit.summaryColumnIndex.Add(2);
            cdoCredit.summaryColumnIndex.Add(3);
            var tblOrgCredit = cdoCredit.FillData();
            tblOrgCredit = ClearEmptyRows(tblOrgCredit, f_S_Creditincome.id, checkList);
            tblOrgCredit = cdoCredit.RecalcSummary(tblOrgCredit);
            tblOrgCredit = CorrectDetailPeriodText(tblOrgCredit, 19, 8);
            dtTables[0] = tblOrgCredit;
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            var tblBudCredit = cdoCredit.FillData();

            foreach (DataRow rowCredit in tblBudCredit.Rows)
            {
                if (rowCredit[f_S_Creditincome.RefOKV] == DBNull.Value)
                {
                    continue;
                }

                var refOkv = Convert.ToInt32(rowCredit[f_S_Creditincome.RefOKV]);
                
                if (refOkv != ReportConsts.codeRUB)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        rowCredit[2 + i] = exRate * GetNumber(rowCredit[14 + i]);
                    }
                }
            }

            tblBudCredit = ClearEmptyRows(tblBudCredit, f_S_Creditincome.id, checkList);
            tblBudCredit = cdoCredit.RecalcSummary(tblBudCredit);
            tblBudCredit = CorrectDetailPeriodText(tblBudCredit, 19, 8);
            dtTables[2] = tblBudCredit;

            // ЦБ

            var capFactCapitalSumField = CreateValuePair(t_S_CPFactCapital.Quantity);
            var capFactDebtSumField = CreateValuePair(t_S_CPFactDebt.Quantity);

            var cdCap = new CapitalDataObject();
            cdCap.InitObject(scheme);
            cdCap.reportParams[ReportConsts.ParamHiDate] = calcDate;
            cdCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            // 00
            cdCap.AddCalcColumn(CalcColumnType.cctCapNumDateDiscount);
            // 01
            cdCap.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);
            // 02
            cdCap.AddDataColumn(f_S_Capital.Sum);
            // 03
            cdCap.AddParamColumn(CalcColumnType.cctRelation, "+15;-16;*13");
            // 04
            cdCap.AddDetailColumn(String.Format("[6](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd));
            // 05
            cdCap.AddDetailColumn(String.Format("[7](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd));
            // 06
            cdCap.AddParamColumn(CalcColumnType.cctRelation, "+18;*13;+17");
            // 07
            cdCap.AddDetailColumn(String.Format("+[9](1<={1}1>={0})[10](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd));
            // 08
            cdCap.AddDataColumn(f_S_Capital.DateDischarge);
            // СЛУЖЕБНЫЕ
            // 09
            cdCap.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.RowType);
            // 10
            cdCap.AddDataColumn(f_S_Capital.RefOrganizations);
            // 11
            cdCap.AddDataColumn(f_S_Capital.GenAgent);
            // 12
            cdCap.AddCalcColumn(CalcColumnType.cctPercentText);
            // 13
            cdCap.AddDataColumn(f_S_Capital.Nominal);
            // 14
            cdCap.AddDetailColumn(String.Format("[0](1<{0})", maxDate), capFactCapitalSumField, true);
            // 15
            cdCap.AddDetailColumn(String.Format("[0](1<{0})", calcDate), capFactCapitalSumField, true);
            // 16
            cdCap.AddDetailColumn(String.Format("[1](1<{0})", calcDate), capFactDebtSumField, true);
            // 17
            cdCap.AddDetailColumn(String.Format("[3](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd));
            cdCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, "8");
            // 18
            cdCap.AddDetailColumn(String.Format("[1](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd), capFactDebtSumField, true);
            // 19
            cdCap.AddDetailTextColumn(String.Format("[6](1>={0}1<={1})", calcDate, maxDate), cdCap.ParamOnlyDates, String.Empty);
            // 20
            cdCap.AddDetailColumn(String.Format("[7](1>{0})", calcDate));
            // это финиш, детка
            cdCap.sortString = FormSortString(StrSortUp(f_S_Capital.GenAgent), StrSortUp(f_S_Capital.StartDate));
            cdCap.summaryColumnIndex.Add(2);
            var tblCap = cdCap.FillData();
            tblCap = ClearEmptyRows(tblCap, f_S_Capital.id, checkList);
            tblCap = ClearNegativeCells(tblCap, checkList);
            tblCap = cdCap.RecalcSummary(tblCap);
            tblCap = CorrectDetailPeriodText(tblCap, 19, 8);
            dtTables[1] = tblCap;

            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.DateDoc, GetFilterStartDateYar(f_S_Guarantissued.DateDoc, calcDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.EndDate,
                GetFilterEndDateYar(f_S_Guarantissued.EndDate, f_S_Guarantissued.RenewalDate, calcDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.ActiveVariantID);
            // 00
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGrntNumRegPercent);
            // 01
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate);
            // 02
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 03
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            // 04
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd));
            // 05
            cdoGrnt.AddDetailColumn(String.Format("[4](1<={1}1>={0})", dateMonthPlanStr, dateMonthPlanEnd));
            // 06
            cdoGrnt.AddDetailColumn(String.Format("+[1](1<={1}1>={0})[4](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd));
            // 07
            cdoGrnt.AddDetailColumn(String.Format("+[12](1<={1}1>={0})[9](1<={1}1>={0})", dateMonthFactStr, dateMonthFactEnd));
            // 08
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // 09
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.RowType);
            // 10
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPercentText);
            // 11
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RefOrganizations);
            // 12
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 13
            cdoGrnt.AddDetailColumn(String.Format("[5](1>{0})", dateMonthPlanEnd));
            // 14
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 15
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 16
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 17
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 18
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 19
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 20
            cdoGrnt.AddDetailColumn(String.Format("[5](1>{0})", calcDate));

            cdoGrnt.summaryColumnIndex.Add(2);
            cdoGrnt.sortString = FormSortString(StrSortUp(TempFieldNames.OrgName), StrSortUp(f_S_Capital.StartDate));
            dtTables[3] = cdoGrnt.FillData();
            cdoGrnt.summaryColumnIndex.Remove(13);
            dtTables[3] = ClearEmptyRows(dtTables[3], f_S_Guarantissued.id, checkList);
            dtTables[3] = cdoGrnt.RecalcSummary(dtTables[3]);

            for (var k = 0; k < 4; k++)
            {
                foreach (var index in cdoGrnt.summaryColumnIndex)
                {
                    CorrectThousandSumValue(dtTables[k], index);
                }
            }

            dtTables[4] = dtTables[0].Clone();
            var dtSummary = FillSummaryDataSet(dtTables[0], dtTables[1], cdoGrnt.summaryColumnIndex);
            dtSummary = FillSummaryDataSet(dtSummary, dtTables[2], cdoGrnt.summaryColumnIndex);
            dtTables[4].ImportRow(dtSummary.Rows[0]);
            dtSummary = FillSummaryDataSet(dtSummary, dtTables[3], cdoGrnt.summaryColumnIndex);
            dtTables[4].ImportRow(dtSummary.Rows[0]);
            dtTables[0] = GroupOrgData(dtTables[0], cdoGrnt.summaryColumnIndex);
            dtTables[1] = GroupOrgData(dtTables[1], cdoGrnt.summaryColumnIndex, f_S_Capital.GenAgent, f_S_Capital.GenAgent);
            dtTables[2] = GroupOrgData(dtTables[2], cdoGrnt.summaryColumnIndex);
            dtTables[3] = GroupOrgData(dtTables[3], cdoGrnt.summaryColumnIndex);
            dtTables[2] = CombineCurrencySumYar(dtTables[2], 2, 14, 5);

            foreach (DataRow rowData in dtTables[2].Rows)
            {
                if (rowData[1] != DBNull.Value)
                {
                    rowData[1] = Convert.ToDateTime(rowData[1]).ToShortDateString();
                }
            }

            // заголовочное
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = year;
            drCaption[3] = exRate;

            var monthNames = GetMonthRusNames();
            var monthName1 = monthNames[reportDate.Month - 1];
            var monthName2 = monthNames[reportDate.AddMonths(-1).Month - 1];

            drCaption[4] = monthName1.ToLower();
            drCaption[5] = monthName2.ToLower();

            FillSignatureData(drCaption, 10, reportParams, ReportConsts.ParamExecutor1);

            return dtTables;
        }

        /// <summary>
        /// Приложение 2. Отчет о состоянии государственного долга за на дату
        /// </summary>
        public DataTable[] GetStateDebtApplication2YarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[6];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var reportDate = Convert.ToDateTime(calcDate);
            var year = reportDate.Year;
            var dateMonthPlanStr = GetMonthStart(reportDate);
            var dateMonthPlanEnd = GetMonthEnd(reportDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var exRate = GetNumber(reportParams[ReportConsts.ParamExchangeRate]);
            var dateMonthFactStr = GetMonthStart(reportDate.AddMonths(-1));
            var dateMonthFactEnd = GetMonthEnd(reportDate.AddMonths(-1));
            var checkList = new Collection<int> { 2, 3, 4, 5, 6, 11, 14, 15, 38 };
            var curFields = CreateValuePair(ReportConsts.CurrencySumField);
            var rubFields = CreateValuePair(ReportConsts.SumField);
            // Кредиты
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.ignoreZeroCurrencyRows = true;
            cdoCredit.reportParams[ReportConsts.ParamHiDate] = calcDate;
            cdoCredit.mainFilter.Add(f_S_Creditincome.StartDate, GetFilterStartDateYar(f_S_Creditincome.StartDate, calcDate));
            cdoCredit.mainFilter.Add(f_S_Creditincome.EndDate,
                GetFilterEndDateYar(f_S_Creditincome.EndDate, f_S_Creditincome.RenewalDate, calcDate));
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdoCredit.fixedExchangeRate.Add(GetEuroCode(), exRate);
            // 00
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditNumDatePercent);
            // 01
            cdoCredit.AddCalcColumn(CalcColumnType.cctCalcSum);
            // 02
            cdoCredit.AddDetailColumn(String.Format("[1](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd), rubFields, true);
            // 03
            cdoCredit.AddDetailColumn(String.Format("[4](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd));
            // 04
            cdoCredit.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateMonthPlanStr));
            // 05
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 06
            cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 07
            cdoCredit.AddDetailTextColumn(String.Format("[2](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd),
                cdoCredit.ParamOnlyDates, "1");
            // 08
            cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 09
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 10
            cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 11
            cdoCredit.AddDetailTextColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd),
                cdoCredit.ParamOnlyDates, "1");
            // 12
            cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 13
            cdoCredit.AddDetailColumn(String.Format("+[8](1>={0}1<={1})[9](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd));
            // 14
            cdoCredit.AddDetailColumn(String.Format("--+[6](1<{0})[7](1<{0})[8](1<{0})[9](1<{0})", dateMonthPlanStr));
            // 15
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // 16
            cdoCredit.AddDataColumn(f_S_Creditincome.RefOrganizations);
            // 17
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 18
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.RowType);
            // 19
            cdoCredit.AddCalcColumn(CalcColumnType.cctPercentText);
            // 20
            cdoCredit.AddDetailTextColumn(String.Format("[2](1>={0}1<={1})", calcDate, maxDate), cdoCredit.ParamOnlyDates, String.Empty);
            // 21
            cdoCredit.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            // 22
            cdoCredit.AddDataColumn(f_S_Creditincome.RefOKV, ReportConsts.ftInt32);
            // 23
            cdoCredit.AddDataColumn(f_S_Creditincome.CurrencySum);
            // 24
            cdoCredit.AddDetailColumn(String.Format("[1](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd), curFields, true);
            // 25
            cdoCredit.AddDetailColumn(String.Format("[4](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd), curFields, true);
            // 26
            cdoCredit.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateMonthPlanStr), curFields, true);
            // 27
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 28
            cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd), curFields, true);
            // 29
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 30
            cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd), curFields, true);
            // 31
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 32
            cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd), curFields, true);
            // 33
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 34
            cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd), curFields, true);
            // 35
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 36
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            // 37
            cdoCredit.AddDetailColumn(String.Format("[5](1>{0})", dateMonthPlanEnd), curFields, true);
            // 38
            cdoCredit.AddDetailColumn(String.Format("[5](1>{0})", calcDate));

            cdoCredit.summaryColumnIndex.Add(1);
            cdoCredit.sortString = FormSortString(
                StrSortUp(f_S_Creditincome.RefOKV), 
                StrSortUp(TempFieldNames.OrgName), 
                StrSortUp(f_S_Creditincome.ContractDate));
            dtTables[0] = cdoCredit.FillData();
            dtTables[0] = CorrectDetailPeriodText(dtTables[0], 20, 15);
            dtTables[0] = ClearEmptyRows(dtTables[0], f_S_Creditincome.id, checkList);
            dtTables[0] = ClearNegativeCells(dtTables[0], checkList);
            dtTables[0] = CorrectDetailDateText(dtTables[0], 07, 07);
            dtTables[0] = CorrectDetailDateText(dtTables[0], 11, 11);
            dtTables[0] = cdoCredit.RecalcSummary(dtTables[0]);
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[2] = cdoCredit.FillData();
            dtTables[2] = CorrectDetailPeriodText(dtTables[2], 20, 15);
            dtTables[2] = ClearEmptyRows(dtTables[2], f_S_Creditincome.id, checkList);
            dtTables[2] = ClearNegativeCells(dtTables[2], checkList);
            dtTables[2] = cdoCredit.RecalcSummary(dtTables[2]);
            // ЦБ
            var capFactCapitalSumField = CreateValuePair(t_S_CPFactCapital.Quantity);
            var capFactDebtSumField = CreateValuePair(t_S_CPFactDebt.Quantity);
            var cdCap = new CapitalDataObject();
            cdCap.InitObject(scheme);
            cdCap.reportParams[ReportConsts.ParamHiDate] = calcDate;
            cdCap.mainFilter.Add(f_S_Capital.StartDate, GetFilterStartDateYar(f_S_Capital.StartDate, calcDate));
            cdCap.mainFilter.Add(f_S_Capital.DateDischarge,
                GetFilterEndDateYar(f_S_Capital.DateDischarge, f_S_Capital.DateDischarge, calcDate));
            cdCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            // 00
            cdCap.AddCalcColumn(CalcColumnType.cctCapNumDateDiscount);
            // 01
            cdCap.AddDataColumn(f_S_Capital.Sum);
            // 02
            cdCap.AddParamColumn(CalcColumnType.cctRelation, "+22;*20");
            // 03
            cdCap.AddDetailColumn(String.Format("[3](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd));
            cdCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, "8");
            // 04
            cdCap.AddParamColumn(CalcColumnType.cctRelation, "+23;-24;*20");
            // 05
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 06
            cdCap.AddDetailColumn(String.Format("[6](1<={0}1>={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 07
            cdCap.AddDetailTextColumn(String.Format("[6](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd),
                cdCap.ParamOnlyDates, "1");
            // 08
            cdCap.AddDetailColumn(String.Format("[6](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 09
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 10
            cdCap.AddDetailColumn(String.Format("[7](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 11
            cdCap.AddDetailTextColumn(String.Format("[7](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd),
                cdCap.ParamOnlyDates, "1");
            // 12
            cdCap.AddDetailColumn(String.Format("[7](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 13
            cdCap.AddDetailColumn(String.Format("+[9](1>={0}1<={1})[10](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd));
            // 14
            cdCap.AddDetailColumn(String.Format("--+[8](1<{0})[11](0<{0})[9](1<{0})[10](1<{0})", dateMonthPlanStr));
            // 15
            cdCap.AddDataColumn(f_S_Capital.DateDischarge);
            // 16
            cdCap.AddDataColumn(f_S_Capital.RefOrganizations);
            // 17
            cdCap.AddDataColumn(f_S_Capital.GenAgent);
            // 18
            cdCap.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.RowType);
            // 19
            cdCap.AddCalcColumn(CalcColumnType.cctPercentText);
            // 20
            cdCap.AddDataColumn(f_S_Capital.Nominal);
            // 21
            cdCap.AddDetailColumn(String.Format("[0](1<{0})", maxDate), capFactCapitalSumField, true);
            // 22
            cdCap.AddDetailColumn(String.Format("[1](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd), capFactDebtSumField, true);
            // 23
            cdCap.AddDetailColumn(String.Format("[0](1<{0})", calcDate), capFactCapitalSumField, true);
            // 24
            cdCap.AddDetailColumn(String.Format("[1](1<{0})", calcDate), capFactDebtSumField, true);
            // 25
            cdCap.AddDetailColumn(String.Format("[6](1<={0}1>={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 26
            cdCap.AddDetailTextColumn(String.Format("[6](1>={0}1<={1})", calcDate, maxDate), cdCap.ParamOnlyDates, String.Empty);
            // 27
            cdCap.AddDetailColumn(String.Format("[7](1>{0})", dateMonthPlanEnd));
            // 28
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 29
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 30
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 31
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 32
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 33
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 34
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 35
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 36
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 37
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 38
            cdCap.AddDetailColumn(String.Format("[7](1>{0})", calcDate));
            cdCap.sortString = StrSortUp(f_S_Capital.GenAgent);
            dtTables[1] = cdCap.FillData();
            dtTables[1] = CorrectDetailPeriodText(dtTables[1], 26, 15);
            dtTables[1] = ClearEmptyRows(dtTables[1], f_S_Capital.id, checkList);
            dtTables[1] = ClearNegativeCells(dtTables[1], checkList);
            dtTables[1] = cdCap.RecalcSummary(dtTables[1]);
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.DateDoc, String.Format("<='{0}'", calcDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.EndDate,
                String.Format(">='{0}' or c.{1}>='{0}'", GetYearStart(calcDate), f_S_Guarantissued.RenewalDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.ActiveVariantID);
            // 00
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGrntNumRegPercent);
            // 01
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Sum);
            // 02
            cdoGrnt.AddDetailColumn(String.Format("[1](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd));
            // 03
            cdoGrnt.AddDetailColumn(String.Format("[4](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd));
            // 04
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateMonthPlanStr));
            // 05
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 06
            cdoGrnt.AddDetailColumn(String.Format("[3](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 07
            cdoGrnt.AddDetailTextColumn(String.Format("[3](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd),
                cdoGrnt.ParamOnlyDates, "1");
            // 08
            cdoGrnt.AddDetailColumn(String.Format("[3](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 09
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 10
            cdoGrnt.AddDetailColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 11
            cdoGrnt.AddDetailTextColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd),
                cdoGrnt.ParamOnlyDates, "1");
            // 12
            cdoGrnt.AddDetailColumn(String.Format("[5](1>={0}1<={1})", dateMonthPlanStr, dateMonthPlanEnd));
            // 13
            cdoGrnt.AddDetailColumn(String.Format("+[9](1>={0}1<={1})[12](1>={0}1<={1})", dateMonthFactStr, dateMonthFactEnd));
            // 14
            cdoGrnt.AddDetailColumn(String.Format("--+[8](1<{0})[11](0<{0})[9](1<{0})[12](1<{0})", dateMonthPlanStr));
            // 15
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            // 16
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RefOrganizations);
            // 17
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            // 18
            cdoGrnt.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.RowType);
            // 19
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPercentText);
            // 20
            cdoGrnt.AddDetailColumn(String.Format("[5](1>{0})", dateMonthPlanEnd));
            // 21
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 22
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 23
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 24
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 25
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 26
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 27
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 28
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 29
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 30
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 31
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 32
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 33
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 34
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 35
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 36
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 37
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            // 38
            cdoGrnt.AddDetailColumn(String.Format("[5](1>{0})", calcDate));

            cdoGrnt.summaryColumnIndex.Add(1);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPercentText);
            cdoGrnt.sortString = StrSortUp(TempFieldNames.OrgName);
            dtTables[3] = cdoGrnt.FillData();
            cdoGrnt.summaryColumnIndex.Remove(20);
            dtTables[3] = ClearEmptyRows(dtTables[3], f_S_Guarantissued.id, checkList);
            dtTables[3] = cdoGrnt.RecalcSummary(dtTables[3]);

            for (var k = 0; k < 4; k++)
            {
                foreach (var index in cdoGrnt.summaryColumnIndex)
                {
                    CorrectThousandSumValue(dtTables[k], index);
                }
            }
            
            dtTables[4] = dtTables[0].Clone();
            var dtSummary = FillSummaryDataSet(dtTables[0], dtTables[1], cdoGrnt.summaryColumnIndex);
            dtSummary = FillSummaryDataSet(dtSummary, dtTables[2], cdoGrnt.summaryColumnIndex);
            dtTables[4].ImportRow(dtSummary.Rows[0]);
            dtSummary = FillSummaryDataSet(dtSummary, dtTables[3], cdoGrnt.summaryColumnIndex);
            dtTables[4].ImportRow(dtSummary.Rows[0]);
            dtTables[0] = GroupOrgData(dtTables[0], cdoGrnt.summaryColumnIndex);
            dtTables[1] = GroupOrgData(dtTables[1], cdoGrnt.summaryColumnIndex, f_S_Capital.GenAgent, f_S_Capital.GenAgent);
            dtTables[2] = GroupOrgData(dtTables[2], cdoGrnt.summaryColumnIndex);
            dtTables[3] = GroupOrgData(dtTables[3], cdoGrnt.summaryColumnIndex);
            dtTables[2] = CombineCurrencySumYar(dtTables[2], 1, 23, 12);
            // заголовочное
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = year;
            drCaption[2] = GetMonthText1(reportDate.Month);
            drCaption[3] = GetMonthText1(reportDate.AddMonths(-1).Month);
            drCaption[4] = exRate;
            FillSignatureData(drCaption, 10, reportParams, ReportConsts.ParamExecutor1);
            return dtTables;
        }

        /// <summary>
        /// Приложение 3. Отчет о состоянии государственного долга за  отчётный период
        /// </summary>
        public DataTable[] GetStateDebtApplication3YarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var dtResult = new DataTable[4];
            var calcDate = GetEndQuarter(reportParams[ReportConsts.ParamEndDate]);
            var reportDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var sumDivider = (SumDividerEnum)Enum.Parse(typeof(SumDividerEnum), reportParams[ReportConsts.ParamSumModifier]);
            
            if (reportDate.Day == 1)
            {
                calcDate = reportDate.AddDays(-1).ToShortDateString();
            }

            var rubFields = CreateValuePair(ReportConsts.SumField);
            var loPeriodDate = GetYearStart(calcDate);
            var hiPeriodDate = GetEndQuarter(calcDate);
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.ignoreZeroCurrencyRows = true;
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdoCredit.AddDetailColumn(String.Format("+-[0](1<{0})[1](1<{0})[10](1<{0})", loPeriodDate), rubFields, true);
            cdoCredit.AddDetailColumn(String.Format("[0](1>={0}1<={1})", loPeriodDate, hiPeriodDate), rubFields, true);
            cdoCredit.AddDetailColumn(String.Format("[1](1>={0}1<={1})", loPeriodDate, hiPeriodDate), rubFields, true);
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCredit.AddDetailColumn(String.Format("[10](1>={0}1<={1})", loPeriodDate, hiPeriodDate), rubFields, true);
            cdoCredit.SetColumnCondition(t_S_RateSwitchCI.RefTypeSum, ReportConsts.GrntTypeSumMainDbt);
            // служебные
            dtResult[0] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtResult[1] = cdoCredit.FillData();
            // ЦБ
            var capFactCapitalSumField = CreateValuePair(t_S_CPFactCapital.Quantity);
            var capFactDebtSumField = CreateValuePair(t_S_CPFactDebt.Quantity);
            var cdCap = new CapitalDataObject();
            cdCap.InitObject(scheme);
            cdCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            cdCap.AddParamColumn(CalcColumnType.cctRelation, "+6;-7;*5");
            cdCap.AddParamColumn(CalcColumnType.cctRelation, "+10;*5");
            cdCap.AddParamColumn(CalcColumnType.cctRelation, "+9;*5");
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // служебные
            // 5
            cdCap.AddDataColumn(f_S_Capital.Nominal);
            // 6
            cdCap.AddDetailColumn(String.Format("[0](1<{0})", loPeriodDate), capFactCapitalSumField, true);
            // 7
            cdCap.AddDetailColumn(String.Format("[1](1<{0})", loPeriodDate), capFactDebtSumField, true);
            // 8
            cdCap.AddDetailColumn(String.Format("[3](1<={1}1>={0})", loPeriodDate, hiPeriodDate));
            cdCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, "8");
            // 9
            cdCap.AddDetailColumn(String.Format("[1](1>={0}1<={1})", loPeriodDate, hiPeriodDate), capFactDebtSumField, true);
            // 10
            cdCap.AddDetailColumn(String.Format("[0](1>={0}1<={1})", loPeriodDate, hiPeriodDate), capFactCapitalSumField, true);

            dtResult[2] = cdCap.FillData();
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.ActiveVariantID);
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", loPeriodDate));
            cdoGrnt.AddDetailColumn(String.Format("[0](1<={0}1>={1})", loPeriodDate, hiPeriodDate));
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={0}1>={1})", loPeriodDate, hiPeriodDate));
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            dtResult[3] = cdoGrnt.FillData();

            var powModifier = 0;

            if (sumDivider == SumDividerEnum.i2)
            {
                powModifier = 3;
            }

            if (sumDivider == SumDividerEnum.i3)
            {
                powModifier = 6;
            }

            if (sumDivider == SumDividerEnum.i4)
            {
                powModifier = 9;
            }

            var modifier = Math.Pow(10, powModifier);
            
            dtTables[0] = dtResult[0].Clone();
            for (var i = 0; i < 3; i++)
            {
                var rowResult = GetLastRow(dtResult[i]);

                for (var j = 0; j < 5; j++ )
                {
                    rowResult[j] = GetNumber(rowResult[j]) / (decimal)modifier;
                }

                dtTables[0].ImportRow(rowResult);
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = reportDate.ToShortDateString();
            drCaption[1] = loPeriodDate;
            drCaption[2] = GetPrevDay(hiPeriodDate);
            drCaption[3] = Convert.ToDateTime(calcDate).Year;
            drCaption[4] = GetQuarterNum(hiPeriodDate);

            var exRate = new UndercutExchangeUniEuroRate { ParentValue = loPeriodDate };
            var newLoRate = GetNumber(exRate.NewValue);
            var actualLoDate = Convert.ToDateTime(exRate.ActualDate).ToShortDateString();
            exRate.ParentValue = hiPeriodDate;
            var newHiRate = GetNumber(exRate.NewValue);
            var actualHiDate = Convert.ToDateTime(exRate.ActualDate).ToShortDateString();
            drCaption[5] = actualLoDate;
            drCaption[6] = actualHiDate;
            drCaption[7] = newLoRate;
            drCaption[8] = newHiRate;
            drCaption[9] = sumDivider;

            return dtTables;
        }

        /// <summary>
        /// Приложение 5. Отчет о состоянии государственного долга
        /// </summary>
        public DataTable[] GetStateDebtApplication5YarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var dtResult = new DataTable[4];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var year = Convert.ToDateTime(calcDate).Year;
            var dateCurYearStart = GetYearStart(calcDate);
            var datePrevYearStart = GetYearStart(year - 1);
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.mainFilter.Add(f_S_Creditincome.StartDate, GetFilterStartDateYar(f_S_Creditincome.StartDate, calcDate));
            cdoCredit.mainFilter.Add(f_S_Creditincome.EndDate,
                GetFilterEndDateYar(f_S_Creditincome.EndDate, f_S_Creditincome.RenewalDate, calcDate));
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.FixedVariantsID);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdoCredit.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateCurYearStart));
            cdoCredit.AddDetailColumn(String.Format("-[0](1<{0}1>{1})[1](1<{0}1>{1})", calcDate, dateCurYearStart));
            cdoCredit.AddDetailColumn(String.Format("-[0](1<{0}1>{1})[1](1<{0}1>{1})", dateCurYearStart, datePrevYearStart));
            dtResult[0] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtResult[2] = cdoCredit.FillData();
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.DateDoc, GetFilterStartDateYar(f_S_Guarantissued.DateDoc, calcDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.EndDate,
                GetFilterEndDateYar(f_S_Guarantissued.EndDate, f_S_Guarantissued.RenewalDate, calcDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.FixedVariantsID);
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateCurYearStart));
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0}1>{1})[1](1<{0}1>{1})", calcDate, dateCurYearStart));
            cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0}1>{1})[1](1<{0}1>{1})", dateCurYearStart, datePrevYearStart));
            dtResult[3] = cdoGrnt.FillData();
            var cdCap = new CapitalDataObject();
            cdCap.InitObject(scheme);
            cdCap.mainFilter.Add(f_S_Capital.StartDate, GetFilterStartDateYar(f_S_Capital.StartDate, calcDate));
            cdCap.mainFilter.Add(f_S_Capital.DateDischarge,
                GetFilterEndDateYar(f_S_Capital.DateDischarge, f_S_Capital.DateDischarge, calcDate));
            cdCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.FixedVariantsID);
            cdCap.AddDetailColumn(String.Format("-[5](1<{0})[1](1<{0})", dateCurYearStart));
            cdCap.AddDetailColumn(String.Format("-[5](1<{0}1>{1})[1](1<{0}1>{1})", calcDate, dateCurYearStart));
            cdCap.AddDetailColumn(String.Format("-[5](1<{0}1>{1})[1](1<{0}1>{1})", dateCurYearStart, datePrevYearStart));
            dtResult[1] = cdCap.FillData();
            
            foreach (DataTable t in dtResult)
            {
                foreach (var index in cdoCredit.summaryColumnIndex)
                {
                    CorrectThousandSumValue(t, index);
                }
            }

            dtTables[0] = dtResult[0].Clone();
            
            for (var k = 0; k < 4; k++)
            {
                dtTables[0].ImportRow(GetLastRow(dtResult[k]));
            }
            
            var drResult = dtTables[0].Rows.Add();
            
            for (var i = 0; i < 3; i++)
            {
                drResult[i] = 0;
                for (int k = 0; k < 4; k++)
                {
                    drResult[i] = Convert.ToDecimal(drResult[i]) + Convert.ToDecimal(dtTables[0].Rows[k][i]);
                }
            }

            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(3);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = calcDate;
            drCaption[1] = year;
            drCaption[2] = year - 1;
            return dtTables;
        }

        /// <summary>
        /// Отчета "Сверка расходов и доходов в проект бюджета"
        /// </summary>
        public DataTable[] GetCollationDRData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            var year = DateTime.Now.Year;
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.ignoreCurrencyCalc = true;
            cdoCredit.mainFilter.Add(f_S_Capital.RefVariant, Combine(
                ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdoCredit.AddCalcColumn(CalcColumnType.cctOrganizationRegion);
            cdoCredit.AddDataColumn(f_S_Creditincome.Num);
            cdoCredit.AddDataColumn(f_S_Creditincome.ContractDate);
            cdoCredit.AddDataColumn(f_S_Creditincome.RenewalDate);
            cdoCredit.AddDataColumn(f_S_Creditincome.Sum, ReportConsts.ftDecimal);
            
            for (var i = 0; i < 4; i++)
            {
                var yearStart = GetYearStart(year + i);
                var yearEnd = GetYearEnd(year + i);
                cdoCredit.AddDetailColumn(String.Format("[5](1>={0}1<={1})", yearStart, yearEnd));
            }

            cdoCredit.AddDataColumn(f_S_Creditincome.RefSStatusPlan, ReportConsts.ftInt32);
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctSortStatus, TempFieldNames.SortStatus);
            cdoCredit.sortString = StrSortUp(TempFieldNames.SortStatus);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            dtTables[0] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdoCredit.FillData();
            var cdoCreditIss = new CreditIssuedDataObject();
            cdoCreditIss.InitObject(scheme);
            cdoCreditIss.ignoreCurrencyCalc = true;
            cdoCreditIss.mainFilter.Add(f_S_Creditissued.RefVariant, Combine
                (ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdoCreditIss.AddCalcColumn(CalcColumnType.cctOrganizationRegion);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.Num);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.DocDate);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.RenewalDate);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.Sum, ReportConsts.ftDecimal);
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(4);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            
            for (var i = 0; i < 4; i++)
            {
                var yearStart = GetYearStart(year + i);
                var yearEnd = GetYearEnd(year + i);
                cdoCreditIss.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearStart, yearEnd));
                drCaption[i] = year + i;
            }

            cdoCreditIss.AddDataColumn(f_S_Creditissued.RefSStatusPlan, ReportConsts.ftInt32);
            cdoCreditIss.AddCalcNamedColumn(CalcColumnType.cctSortStatus, TempFieldNames.SortStatus);
            cdoCreditIss.sortString = StrSortUp(TempFieldNames.SortStatus);
            cdoCreditIss.mainFilter.Add(f_S_Creditissued.RefSTypeCredit, ReportConsts.CreditIssuedBudCode);
            dtTables[2] = cdoCreditIss.FillData();
            cdoCreditIss.mainFilter[f_S_Creditissued.RefSTypeCredit] = ReportConsts.CreditIssuedOrgCode;
            dtTables[3] = cdoCreditIss.FillData();
            return dtTables;
        }

        /// <summary>
        /// Отчета "Сверка ИФ в проект бюджета"
        /// </summary>
        public DataTable[] GetCollationIFData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[13];
            var year = DateTime.Now.Year;
            var today = DateTime.Now.ToShortDateString();
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.ignoreCurrencyCalc = true;
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefVariant, 
                Combine(ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSStatusPlan, "4");
            cdoCredit.AddCalcColumn(CalcColumnType.cctOrganizationRegion);
            cdoCredit.AddDataColumn(f_S_Creditincome.Num);
            cdoCredit.AddDataColumn(f_S_Creditincome.ContractDate);
            cdoCredit.AddDataColumn(f_S_Creditincome.RenewalDate);
            cdoCredit.AddDataColumn(f_S_Creditincome.Sum, ReportConsts.ftDecimal);
            var yearStart = GetYearStart(year);
            var yearEnd = GetYearEnd(year);
            cdoCredit.AddDetailColumn(String.Format("+-[0](1>{0}1<{1})[1](1>{0}1<{1})[10](0<{1})",
                DateTime.MinValue.ToShortDateString(), yearStart));
            cdoCredit.AddDetailColumn(String.Format("[0](1>={0}1<={1})", yearStart, yearEnd));
            cdoCredit.AddDetailColumn(String.Format("[3](0>{0}0<={1})", today, yearEnd));
            cdoCredit.AddDetailColumn(String.Format("+[0](1>={0}1<={1})[3](0>{2}0<={1})", yearStart, yearEnd, today));
            cdoCredit.AddDetailColumn(String.Format("[1](1>={0}1<={1})", yearStart, yearEnd));
            cdoCredit.AddDetailColumn(String.Format("[2](1>{0}1<={1})", today, yearEnd));
            cdoCredit.AddDetailColumn(String.Format("+[1](1>={0}1<={1})[2](1>{2}1<={1})", yearStart, yearEnd, today));
            cdoCredit.AddDetailColumn(String.Format("[10](0>={0}0<={1})", yearStart, yearEnd));
            cdoCredit.columnCondition.Add(7, FormNegativeFilterValue(f_S_Creditincome.RefSStatusPlan, "4"));
            cdoCredit.columnCondition.Add(10, FormFilterValue(f_S_Creditincome.RefSStatusPlan, "4"));
            
            for (var i = 1; i < 4; i++)
            {
                yearStart = GetYearStart(year + i);
                yearEnd = GetYearEnd(year + i);
                cdoCredit.AddDetailColumn(String.Format("[3](0>={0}0<={1})", yearStart, yearEnd));
                cdoCredit.AddDetailColumn(String.Format("[2](1>={0}1<={1})", yearStart, yearEnd));
                cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCredit.columnCondition.Add(10 + i * 3, FormNegativeFilterValue(f_S_Creditincome.RefSStatusPlan, "4"));
                cdoCredit.columnCondition.Add(11 + i * 3, FormNegativeFilterValue(f_S_Creditincome.RefSStatusPlan, "4"));
                cdoCredit.columnCondition.Add(12 + i * 3, FormNegativeFilterValue(f_S_Creditincome.RefSStatusPlan, "4"));
            }

            cdoCredit.AddDataColumn(f_S_Creditincome.RefSStatusPlan, ReportConsts.ftInt32);
            cdoCredit.AddCalcNamedColumn(CalcColumnType.cctSortStatus, TempFieldNames.SortStatus);
            cdoCredit.sortString = StrSortUp(TempFieldNames.SortStatus);
            cdoCredit.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            dtTables[0] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "<>-1;<>1;<>2;<>3;<>4";
            dtTables[1] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "4";
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[2] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSStatusPlan] = "<>-1;<>1;<>2;<>3;<>4";
            dtTables[3] = cdoCredit.FillData();
            var cdoCreditIss = new CreditIssuedDataObject();
            cdoCreditIss.InitObject(scheme);
            cdoCreditIss.ignoreCurrencyCalc = true;
            cdoCreditIss.mainFilter.Add(f_S_Creditissued.RefSStatusPlan, "4");
            cdoCreditIss.mainFilter.Add(f_S_Creditissued.RefVariant, 
                Combine(ReportConsts.ActiveVariantID, reportParams[ReportConsts.ParamVariantID]));
            cdoCreditIss.AddCalcColumn(CalcColumnType.cctOrganizationRegion);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.Num);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.DocDate);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.RenewalDate);
            cdoCreditIss.AddDataColumn(f_S_Creditissued.Sum, ReportConsts.ftDecimal);
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(4);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            yearStart = GetYearStart(year);
            cdoCreditIss.AddDetailColumn(String.Format("-[3](1>{0}1<{1})[4](1>{0}1<{1})",
                DateTime.MinValue.ToShortDateString(), yearStart));
            yearStart = GetYearStart(year);
            yearEnd = GetYearEnd(year);
            cdoCreditIss.AddDetailColumn(String.Format("[3](1>={0}1<={1})", yearStart, yearEnd));
            cdoCreditIss.AddDetailColumn(String.Format("[0](0>{0}0<={1})", today, yearEnd));
            cdoCreditIss.AddDetailColumn(String.Format("+[3](1>={0}1<={1})[0](0>{2}0<={1})", yearStart, yearEnd, today));
            cdoCreditIss.AddDetailColumn(String.Format("[4](1>={0}1<={1})", yearStart, yearEnd));
            cdoCreditIss.AddDetailColumn(String.Format("[1](1>{0}1<={1})", today, yearEnd));
            cdoCreditIss.AddDetailColumn(String.Format("+[4](1>={0}1<={1})[1](1>{2}1<={1})", yearStart, yearEnd, today));
            cdoCreditIss.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCreditIss.columnCondition.Add(07, FormNegativeFilterValue(f_S_Creditissued.RefSStatusPlan, "4"));
            cdoCreditIss.columnCondition.Add(10, FormNegativeFilterValue(f_S_Creditissued.RefSStatusPlan, "4"));
            drCaption[0] = year;
            
            for (var i = 1; i < 4; i++)
            {
                yearStart = GetYearStart(year + i);
                yearEnd = GetYearEnd(year + i);
                cdoCreditIss.AddDetailColumn(String.Format("[0](0>={0}0<={1})", yearStart, yearEnd));
                cdoCreditIss.AddDetailColumn(String.Format("[1](1>={0}1<={1})", yearStart, yearEnd));
                cdoCreditIss.AddCalcColumn(CalcColumnType.cctUndefined);
                drCaption[i] = year + i;
                cdoCreditIss.columnCondition.Add(10 + i * 3, FormNegativeFilterValue(f_S_Creditissued.RefSStatusPlan, "4"));
                cdoCreditIss.columnCondition.Add(11 + i * 3, FormNegativeFilterValue(f_S_Creditissued.RefSStatusPlan, "4"));
                cdoCreditIss.columnCondition.Add(12 + i * 3, FormNegativeFilterValue(f_S_Creditissued.RefSStatusPlan, "4"));
            }

            cdoCreditIss.AddDataColumn(f_S_Creditissued.RefSStatusPlan, ReportConsts.ftInt32);
            cdoCreditIss.AddCalcNamedColumn(CalcColumnType.cctSortStatus, TempFieldNames.SortStatus);
            cdoCreditIss.sortString = StrSortUp(TempFieldNames.SortStatus);
            cdoCreditIss.mainFilter.Add(f_S_Creditissued.RefSTypeCredit, ReportConsts.CreditIssuedBudCode);
            dtTables[4] = cdoCreditIss.FillData();
            cdoCreditIss.mainFilter[f_S_Creditissued.RefSStatusPlan] = "<>-1;<>1;<>2;<>3;<>4";
            dtTables[5] = cdoCreditIss.FillData();
            cdoCreditIss.mainFilter[f_S_Creditissued.RefSTypeCredit] = ReportConsts.CreditIssuedOrgCode;
            cdoCreditIss.mainFilter[f_S_Creditissued.RefSStatusPlan] = "4";
            dtTables[6] = cdoCreditIss.FillData();
            cdoCreditIss.mainFilter[f_S_Creditissued.RefSStatusPlan] = "<>-1;<>1;<>2;<>3;<>4";
            dtTables[7] = cdoCreditIss.FillData();
            
            for (var i = 0; i < 8; i++)
            {
                FillYearColumn(dtTables[i]);
            }
            
            cdoCredit.summaryColumnIndex.Add(12);
            cdoCredit.summaryColumnIndex.Add(15);
            cdoCredit.summaryColumnIndex.Add(18);
            cdoCredit.summaryColumnIndex.Add(21);
            cdoCreditIss.summaryColumnIndex.Add(12);
            cdoCreditIss.summaryColumnIndex.Add(15);
            cdoCreditIss.summaryColumnIndex.Add(18);
            cdoCreditIss.summaryColumnIndex.Add(21);
            dtTables[8] = FillSummaryDataSet(dtTables[0], dtTables[1], cdoCredit.summaryColumnIndex);
            dtTables[9] = FillSummaryDataSet(dtTables[2], dtTables[3], cdoCredit.summaryColumnIndex);
            dtTables[10] = FillSummaryDataSet(dtTables[4], dtTables[5], cdoCreditIss.summaryColumnIndex);
            dtTables[11] = FillSummaryDataSet(dtTables[6], dtTables[7], cdoCreditIss.summaryColumnIndex);
            return dtTables;
        }

        /// <summary>
        /// Отчета "Муниципальная задолженность"
        /// </summary>
        public DataTable[] GetMunicipalDebtYarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            var cdoCI = new CreditDataObject();
            var cdoGrnt = new GarantDataObject();
            var shortDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var isFirstDay = Convert.ToDateTime(shortDate).Day == 1;
            var date1Str = GetYearStart(shortDate);
            var nextMonth = Convert.ToDateTime(GetMonthStart(shortDate)).AddMonths(1);
            
            if (isFirstDay)
            {
                nextMonth = Convert.ToDateTime(GetMonthStart(shortDate));
                shortDate = Convert.ToDateTime(shortDate).AddDays(-1).ToShortDateString();
            }

            var nextMonthStart = nextMonth.ToShortDateString();
            var nextMonthEnd = GetMonthEnd(nextMonth);

            // Кредиты
            cdoCI.InitObject(scheme);
            cdoCI.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.FixedVariantsID);
            cdoCI.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdoCI.mainFilter.Add(f_S_Creditincome.StartDate, String.Format("<='{0}'", shortDate));
            cdoCI.mainFilter.Add(f_S_Creditincome.EndDate,
                String.Format(">='{0}' or c.{1}>='{0}'", date1Str, f_S_Creditincome.RenewalDate));
            cdoCI.AddCalcColumn(CalcColumnType.cctPosition);
            cdoCI.AddCalcColumn(CalcColumnType.cctContractNum2);
            cdoCI.AddCalcColumn(CalcColumnType.cctPercentTextMaxMin);
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdoCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", date1Str));
            cdoCI.AddDetailColumn(String.Format("[0](1<{0}1>={1})", shortDate, date1Str));
            cdoCI.AddDetailColumn(String.Format("[1](1<{0}1>={1})", shortDate, date1Str));
            cdoCI.AddParamColumn(CalcColumnType.cctAllOperationDates, "1");
            cdoCI.AddDetailColumn(String.Format("[4](1<{0}1>={1})", shortDate, date1Str));
            cdoCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", shortDate));
            cdoCI.AddDetailColumn(String.Format("[5](1<={0}1>={1})", nextMonthEnd, nextMonthStart));
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            cdoCI.sortString = StrSortUp(f_S_Creditincome.ContractDate);
            dtTables[0] = cdoCI.FillData();
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdoCI.FillData();
            // Гарантии
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.FixedVariantsID);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.DateDoc, String.Format("<='{0}'", shortDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.EndDate,
                String.Format(">='{0}' or c.{1}>='{0}'", date1Str, f_S_Guarantissued.RenewalDate));
            cdoGrnt.summaryColumnIndex = cdoCI.summaryColumnIndex;
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctContractNum3);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPercentTextMaxMin);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdoGrnt.AddDetailColumn(String.Format("+[1](1<{0})[4](1<{0})", date1Str));
            cdoGrnt.AddDetailColumn(String.Format("[0](1<{0}1>={1})", shortDate, date1Str));
            cdoGrnt.AddDetailColumn(String.Format("[1](1<{0}1>={1})", shortDate, date1Str));
            cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoGrnt.AddDetailColumn(String.Format("[4](1<{0}1>={1})", shortDate, date1Str));
            cdoGrnt.AddDetailColumn(String.Format("+[1](1<{0})[4](1<{0})", shortDate));
            cdoGrnt.AddDetailColumn(String.Format("[5](1<={0}1>={1})", nextMonthEnd, nextMonthStart));
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDoc, ReportConsts.ftDateTime);
            cdoGrnt.AddDataColumn(ReportConsts.SumField);
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.DateDoc);
            dtTables[2] = cdoGrnt.FillData();
            CorrectGarantSum(dtTables[2], date1Str, shortDate, cdoGrnt);
            
            if (isFirstDay)
            {
                shortDate = Convert.ToDateTime(shortDate).AddDays(1).ToShortDateString();
            }

            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(4);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = shortDate;
            drCaption[1] = date1Str;
            drCaption[2] = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]).Year;
            drCaption[3] = String.Format("{0}-{1}", nextMonthStart, nextMonthEnd);
            dtTables[3] = dtTables[0].Clone();
            dtTables[3].ImportRow(GetLastRow(dtTables[0]));
            dtTables[3].ImportRow(GetLastRow(dtTables[1]));
            dtTables[3].ImportRow(GetLastRow(dtTables[2]));
            var dtSummary = FillSummaryDataSet(dtTables[0], dtTables[1], cdoGrnt.summaryColumnIndex);
            dtTables[3].ImportRow(GetLastRow(dtSummary));
            dtSummary = FillSummaryDataSet(dtSummary, dtTables[2], cdoGrnt.summaryColumnIndex);
            dtTables[3].ImportRow(GetLastRow(dtSummary));
            var drGarantSummary = dtTables[3].Rows[2];
            drGarantSummary[5] = DBNull.Value;
            drGarantSummary[8] = DBNull.Value;
            drGarantSummary[10] = DBNull.Value;
            dtTables[0].Rows.RemoveAt(GetLastRowIndex(dtTables[0]));
            dtTables[1].Rows.RemoveAt(GetLastRowIndex(dtTables[1]));
            dtTables[2].Rows.RemoveAt(GetLastRowIndex(dtTables[2]));
            return dtTables;
        }

        /// <summary>
        /// Отчета "Программа гарантий"
        /// </summary>
        public DataTable[] GetDebtContractInformationData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[5];
            var cdoCI = new CreditDataObject();
            var cdoCap = new CapitalDataObject();
            var cdoGrnt = new GarantDataObject();
            var shortDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            cdoGrnt.InitObject(scheme);
            cdoGrnt.ignoreCurrencyCalc = true;
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.ActiveVariantID);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctNumStartDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganizations);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOKVName);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDoc);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.EndDate);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDemand);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DatePerformance);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCalcSum);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.StartDate);
            dtTables[2] = cdoGrnt.FillData();
            cdoCap.InitObject(scheme);
            cdoCap.ignoreCurrencyCalc = true;
            cdoCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapNum);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapNameKind);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapForm);
            cdoCap.AddCalcColumn(CalcColumnType.cctOKVName);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapRegDateNum);
            cdoCap.AddCalcColumn(CalcColumnType.cctCapNPANames);
            cdoCap.AddDataColumn(f_S_Capital.Owner);
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            cdoCap.AddCalcColumn(CalcColumnType.cctCalcSum);
            cdoCap.AddDataColumn(f_S_Capital.StartDate);
            cdoCap.AddDataColumn(f_S_Capital.EndDate);
            cdoCap.AddDetailColumn(String.Format("[0](1<{0})", shortDate));
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddDataColumn(f_S_Capital.Discount);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddDataColumn(f_S_Capital.Depository);
            cdoCap.AddDataColumn(f_S_Capital.Trade);
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCap.AddDetailColumn(String.Format("[3](1<={0})", shortDate));
            cdoCap.AddDetailColumn(String.Format("[3](1<={0})", shortDate));
            cdoCap.AddDetailColumn(String.Format("[4](1<={0})", shortDate));
            cdoCap.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", shortDate));
            cdoCap.AddDataColumn(f_S_Capital.RegEmissionDate, ReportConsts.ftDateTime);
            cdoCap.sortString = StrSortUp(f_S_Capital.RegEmissionDate);
            dtTables[0] = cdoCap.FillData();
            cdoCI.InitObject(scheme);
            cdoCI.ignoreCurrencyCalc = true;
            cdoCI.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdoCI.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdoCI.sortString = StrSortUp(f_S_Creditincome.ContractDate);
            cdoCI.AddCalcColumn(CalcColumnType.cctNumContractDate);
            cdoCI.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoCI.AddCalcColumn(CalcColumnType.cctOKVName);
            cdoCI.AddParamColumn(CalcColumnType.cctMinOperationDate, "0");
            cdoCI.AddCalcColumn(CalcColumnType.cctPercentText);
            cdoCI.AddParamColumn(CalcColumnType.cctAllOperationDates, "1");
            cdoCI.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", shortDate));
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            dtTables[1] = cdoCI.FillData();
            cdoCI.InitObject(scheme);
            cdoCI.ignoreCurrencyCalc = true;
            cdoCI.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
            cdoCI.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.BudCreditCode);
            cdoCI.sortString = StrSortUp(f_S_Creditincome.ContractDate);
            cdoCI.AddCalcColumn(CalcColumnType.cctNumContractDate);
            cdoCI.AddCalcColumn(CalcColumnType.cctContractType);
            cdoCI.AddDataColumn(f_S_Creditincome.Note);
            cdoCI.AddParamColumn(CalcColumnType.cctMinOperationDate, "0");
            cdoCI.AddCalcColumn(CalcColumnType.cctCreditEndDate);
            cdoCI.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", shortDate));
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            dtTables[3] = cdoCI.FillData();
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(3);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = shortDate;
            return dtTables;
        }

        /// <summary>
        /// Структура для таблицы результатов отчета "Долговая книга Омск"
        /// </summary>
        public DataTable[] GetDebtorBookOmskData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[4];
            var cdoCI = new CreditDataObject();
            var cdoCap = new CapitalDataObject();
            var cdoGrnt = new GarantDataObject();
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(6);
            // Список дат для колонок
            var dateList = new string[3];
            dateList[0] = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]).AddMonths(-1).ToShortDateString();
            dateList[1] = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            dateList[2] = dateList[1];
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = dateList[0];
            drCaption[1] = dateList[1];
            var regionCode = GetOKTMOCode(scheme);
            drCaption[2] = String.Format("Долговая книга по состоянию на {0}", dateList[1]);
            
            if (regionCode == "52 000 000")
            {
                drCaption[2] = String.Format("Государственная долговая книга Омской области по состоянию на {0} года", dateList[1]);
            }
            
            if (regionCode == "52 701 000")
            {
                drCaption[2] = String.Format("Муниципальная долговая книга муниципального образования городской округ город Омск Омской области по состоянию на {0} года", dateList[1]);
            }

            for (var i = 0; i < 3; i++)
            {
                var dtDetails = new DataTable[4, 3];
                cdoCI.InitObject(scheme);
                cdoCI.mainFilter.Add(f_S_Creditincome.RefSStatusPlan, FormNegFilterValue(ReportConsts.codeRUBStr));
                cdoCI.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.ActiveVariantID);
                cdoCI.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
                cdoCI.AddCalcColumn(CalcColumnType.cctContractDesc);
                cdoCI.AddCalcColumn(CalcColumnType.cctCollateralType);
                cdoCI.AddDataColumn(f_S_Creditincome.Num);
                cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCI.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCI.AddCalcColumn(CalcColumnType.cctOrganization);
                cdoCI.AddCalcColumn(CalcColumnType.cctCreditEndDate);
                
                if (i != 2)
                {
                    cdoCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateList[i]));
                    cdoCI.AddDetailColumn(String.Format("[10](1<={0})", dateList[i]));
                }
                else
                {
                    cdoCI.AddDetailColumn(String.Format("-[0](1<{0}1>={1})[1](1<{0}1>={1})", dateList[1], dateList[0]));
                    cdoCI.AddDetailColumn(String.Format("[10](1<={0}1>={1})", dateList[1], dateList[0]));
                }

                var dtResult2 = cdoCI.FillData();
                dtDetails[1, 0] = cdoCI.dtDetail[0];
                dtDetails[1, 1] = cdoCI.dtDetail[1];
                dtDetails[1, 2] = cdoCI.dtDetail[10];
                cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
                var dtResult3 = cdoCI.FillData();
                dtDetails[2, 0] = cdoCI.dtDetail[0];
                dtDetails[2, 1] = cdoCI.dtDetail[1];
                dtDetails[2, 2] = cdoCI.dtDetail[10];
                cdoCap.InitObject(scheme);
                cdoCap.mainFilter.Add(f_S_Capital.RefStatusPlan, "<>-1");
                cdoCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
                cdoCap.summaryColumnIndex = cdoCI.summaryColumnIndex;
                cdoCap.AddCalcColumn(CalcColumnType.cctContractDesc);
                cdoCap.AddCalcColumn(CalcColumnType.cctCollateralType);
                cdoCap.AddDataColumn(f_S_Capital.CodeCapital);
                cdoCap.AddDataColumn(f_S_Capital.SeriesCapital);
                cdoCap.AddDataColumn(f_S_Capital.StartDate);
                cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
                
                if (i != 2)
                {
                    cdoCap.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateList[i]));
                    cdoCap.AddDetailColumn(String.Format("[2](1<={0})", dateList[i]));
                }
                else
                {
                    cdoCap.AddDetailColumn(String.Format("-[0](1<{0}1>={1})[1](1<{0}1>={1})", dateList[1], dateList[0]));
                    cdoCap.AddDetailColumn(String.Format("[2](1<={0}1>={1})", dateList[1], dateList[0]));
                }

                var dtResult1 = cdoCap.FillData();
                dtDetails[0, 0] = cdoCap.dtDetail[0];
                dtDetails[0, 1] = cdoCap.dtDetail[1];
                dtDetails[0, 2] = cdoCap.dtDetail[2];
                cdoGrnt.InitObject(scheme);
                cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefSStatusPlan, "<>-1");
                cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.ActiveVariantID);
                cdoGrnt.summaryColumnIndex = cdoCI.summaryColumnIndex;
                cdoGrnt.AddCalcColumn(CalcColumnType.cctContractDesc);
                cdoGrnt.AddCalcColumn(CalcColumnType.cctCollateralType);
                cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
                cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoGrnt.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
                cdoGrnt.AddCalcColumn(CalcColumnType.cctCreditEndDate);
                
                if (i != 2)
                {
                    cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateList[i]));
                    cdoGrnt.AddDetailColumn(String.Format("[2](1<{0})", dateList[i]));
                }
                else
                {
                    cdoGrnt.AddDetailColumn(String.Format("-[0](1<{0}1>={1})[1](1<{0}1>={1})", dateList[1], dateList[0]));
                    cdoGrnt.AddDetailColumn(String.Format("[2](1<={0}1>={1})", dateList[1], dateList[0]));
                }

                var dtResult4 = cdoGrnt.FillData();
                dtDetails[3, 0] = cdoGrnt.dtDetail[0];
                dtDetails[3, 1] = cdoGrnt.dtDetail[1];
                dtDetails[3, 2] = cdoGrnt.dtDetail[2];
                dtTables[i] = CreateReportDebtorBookOmskStructure();
                decimal sum = 0;
                if (i == 2) sum = Convert.ToDecimal(drCaption[3]);
                sum += MergeDataTables(cdoCap.okvValues, dtTables[i], dtResult1, dtDetails, 0, dateList, i == 2, sum);
                sum += MergeDataTables(cdoCI.okvValues, dtTables[i], dtResult2, dtDetails, 1, dateList, i == 2, sum);
                sum += MergeDataTables(cdoCI.okvValues, dtTables[i], dtResult3, dtDetails, 2, dateList, i == 2, sum);
                sum += MergeDataTables(cdoGrnt.okvValues, dtTables[i], dtResult4, dtDetails, 3, dateList, i == 2, sum);
                drCaption[i + 3] = sum;
            }

            return dtTables;
        }

        /// <summary>
        /// Заполнитель для отчета "Конструктор отчетов по кредитам"
        /// </summary>
        public DataTable[] GetCreditConstructorData(Dictionary<string, string> reportParams,
            ConstructorType docType)
        {
            var dtTables = new DataTable[4];
            CommonDataObject cdo = new CreditDataObject();
            if (docType == ConstructorType.ctGarant) cdo = new GarantDataObject();
            if (docType == ConstructorType.ctCreditIssued) cdo = new CreditIssuedDataObject();
            var reportDate1 = String.Empty;
            var reportDate2 = String.Empty;
            cdo.InitObject(scheme);
            string fieldName;
            const int maxColumnCount = 250;
            if (docType == ConstructorType.ctCreditOrg)
            {
                cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            }
            
            if (docType == ConstructorType.ctCreditBud)
            {
                cdo.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.BudCreditCode);
            }

            dtTables[1] = CreateReportCaptionTable(maxColumnCount);
            var drVisibility = dtTables[1].Rows.Add();
            dtTables[2] = CreateReportCaptionTable(maxColumnCount);
            var drReportParams = dtTables[2].Rows.Add();
            dtTables[3] = CreateReportCaptionTable(maxColumnCount);
            var drCaptions = dtTables[3].Rows.Add();
            var counter = 0;
            cdo.AddCalcColumn(CalcColumnType.cctPosition);
            drVisibility[counter] = "1";
            counter++;
            var detailSplitNum = 1;
            var reportCaption = String.Empty;

            foreach (var key in reportParams.Keys)
            {
                var value = reportParams[key];
                var splitDetailCount = 0;
                
                if (key == "DocumentType")
                {
                    docType = (ConstructorType) Enum.Parse(typeof (ConstructorType), value);
                }

                if (key == "ReportCaption")
                {
                    reportCaption = value;
                }

                if (key == "ReportDate1")
                {
                    if (value == String.Empty) value = DateTime.MinValue.ToShortDateString();
                    reportDate1 = Convert.ToDateTime(value).ToShortDateString();
                }

                if (key == "ReportDate2")
                {
                    if (value == String.Empty) value = DateTime.MaxValue.ToShortDateString();
                    reportDate2 = Convert.ToDateTime(value).ToShortDateString();
                }

                if (key != "ReportCaption" && key != "DocumentType" && !key.StartsWith("ReportDate") &&
                    !key.EndsWith("Start") && !key.EndsWith("End") && !key.StartsWith("ConstructorType"))
                {
                    // фильтры таблицы фактов
                    if (key.StartsWith("Filter"))
                    {
                        fieldName = key.Remove(0, 6);
                        // Если не параметры периода, то запишем в фильтр значения
                        // Если не сцылочное значение
                        if (!key.Contains("Ref"))
                        {
                            // То либо дата
                            if (key.Contains("Date"))
                            {
                                var filterParamName = key.Remove(key.Length - 9);
                                fieldName = fieldName.Remove(fieldName.Length - 9, 9);
                                cdo.mainFilter.Add(fieldName, String.Empty);
                                
                                // Нижняя граница диапазона
                                if (value == "i2" || value == "i4")
                                {
                                    var value1 = GetConstructorDataValue(
                                        reportParams[filterParamName + "TypeStart"], 
                                        reportParams[filterParamName + "Start"], 
                                        reportDate1, 
                                        reportDate2);

                                    cdo.mainFilter[fieldName] = String.Format("{0}>='{1}';", cdo.mainFilter[fieldName], value1);
                                }
                                
                                // Верхняя граница диапазона
                                if (value == "i3" || value == "i4")
                                {
                                    var value2 = GetConstructorDataValue(
                                        reportParams[filterParamName + "TypeEnd"],
                                        reportParams[filterParamName + "End"],
                                        reportDate1,
                                        reportDate2);
                                    cdo.mainFilter[fieldName] = String.Format("{0}<='{1}';", cdo.mainFilter[fieldName], value2);
                                }

                                cdo.mainFilter[fieldName] = cdo.mainFilter[fieldName].TrimEnd(';');
                                if (cdo.mainFilter[fieldName] == String.Empty) cdo.mainFilter.Remove(fieldName);
                            }
                            else
                            {
                                // либо строка
                                if (value != String.Empty)
                                {
                                    value = String.Format("'{0}'", value);
                                    cdo.mainFilter.Add(fieldName, value);
                                }
                            }
                        }
                        else
                        {
                            if (value != ReportConsts.UndefinedKey)
                            {
                                cdo.mainFilter.Add(fieldName, value);
                            }
                        }
                    }
                    else
                    {
                        drVisibility[counter] = "0";
                    }
                    // видимость колонок из таблицы фактов
                    if (key.StartsWith("View"))
                    {
                        if (Convert.ToBoolean(value))
                        {
                            drVisibility[counter] = "1";
                            fieldName = key.Remove(0, 4);
                            if (!fieldName.Contains("Calc"))
                            {
                                // обычное невычисляемое поле
                                cdo.AddDataColumn(fieldName);
                                if (fieldName == ReportConsts.SumField || fieldName == "DebtSum") 
                                    cdo.summaryColumnIndex.Add(cdo.columnList.Count - 1);
                            }
                            else
                            {
                                // вычисляемое поле - справочник, номер договора и тп
                                fieldName = fieldName.Remove(0, 4);
                                if (fieldName == "OrgName")
                                    cdo.AddCalcColumn(CalcColumnType.cctOrganization);
                                if (fieldName == "ContractNum")
                                    cdo.AddCalcColumn(CalcColumnType.cctCreditTypeNumStartDate);
                                if (fieldName == "Org3Name")
                                    cdo.AddCalcColumn(CalcColumnType.cctOrganization3);
                                if (fieldName == "Percent")
                                    cdo.AddCalcColumn(CalcColumnType.cctPercentText);
                                if (fieldName == "RegionName")
                                    cdo.AddCalcColumn(CalcColumnType.cctRegion);
                                if (fieldName == "NumDocDate")
                                    cdo.AddCalcColumn(CalcColumnType.cctCreditIssNumDocDate);
                            }
                        }
                    }
                    // суммы по деталям
                    if (key.StartsWith("Detail") && (value != "i0"))
                    {
                        drVisibility[counter] = "1";
                        var detailNum = key[6].ToString();
                        var detailSubstrParam = key.Remove(key.Length - 9);
                        
                        if (key[7] >= '0' && key[7] <= '9')
                        {
                            detailNum = key.Substring(6, 2);
                        }
                        
                        var date1 = DateTime.MinValue.ToShortDateString();
                        var date2 = DateTime.MaxValue.ToShortDateString();
                        var columnCaption = String.Empty;
                        
                        // Нижняя граница диапазона
                        if (value == "i2" || value == "i4" || value == "i5")
                        {
                            var dateType = reportParams[detailSubstrParam + "TypeStart"];
                            var dateValue = reportParams[detailSubstrParam + "Start"];
                            date1 = GetConstructorDataValue(dateType, dateValue, reportDate1, reportDate2); 
                            columnCaption = String.Format("{0} c {1}", columnCaption, date1);
                        }
                        
                        // Верхняя граница диапазона
                        if (value == "i3" || value == "i4" || value == "i5")
                        {
                            var dateType = reportParams[detailSubstrParam + "TypeEnd"];
                            var dateValue = reportParams[detailSubstrParam + "End"];
                            date2 = GetConstructorDataValue(dateType, dateValue, reportDate1, reportDate2); 
                            columnCaption = String.Format("{0} по {1}", columnCaption, date2);
                        }
                        // пробегаемся по списку контрактов и определяем общий период для всех
                        if (value == "i5")
                        {
                            var dtMain = cdo.FillMainTableData();
                            var dateType1 = reportParams[detailSubstrParam + "TypeStart"];
                            var dateType2 = reportParams[detailSubstrParam + "TypeEnd"];
                            date1 = CompareConstructorDataValue(dtMain, dateType1, 0, date1);
                            date2 = CompareConstructorDataValue(dtMain, dateType2, 1, date2);
                            columnCaption = String.Format("с {0} по {1}", date1, date2);
                        }
                        
                        var dateFieldIndex = "1";
                        var endDateList = cdo.GetEndDates();

                        if (endDateList[Convert.ToInt32(detailNum)] == null)
                        {
                            dateFieldIndex = "0";
                        }

                        var formula = String.Format("[{0}]({3}>={1}{3}<={2})", detailNum, date1, date2, dateFieldIndex);
                        cdo.AddDetailColumn(formula);
                        drCaptions[counter] = columnCaption;
                        
                        // Детализации на каждый месяц из промежутка
                        if (value == "i5")
                        {
                            var endPeriodDate = Convert.ToDateTime(date2);
                            var loBound = Convert.ToDateTime(date1);
                            var hiBound = GetLastMonthDate(loBound, endPeriodDate);

                            while (DateTime.Compare(loBound, hiBound) <= 0)
                            {
                                counter++;
                                drVisibility[counter] = "1";
                                formula = String.Format("[{0}]({3}>={1}{3}<={2})", 
                                    detailNum,
                                    loBound.ToShortDateString(), 
                                    hiBound.ToShortDateString(), 
                                    dateFieldIndex);
                                cdo.AddDetailColumn(formula);
                                drCaptions[counter] = String.Format("{0} по {1}", 
                                    loBound.ToShortDateString(), 
                                    hiBound.ToShortDateString());
                                loBound = loBound.AddMonths(1);
                                loBound = Convert.ToDateTime(
                                    String.Format("01.{0}.{1}", loBound.Month, loBound.Year));
                                hiBound = GetLastMonthDate(loBound, endPeriodDate);
                                splitDetailCount++;
                            }
                        }
                    }

                    if (key.StartsWith("Detail"))
                    {
                        drReportParams[detailSplitNum] = splitDetailCount;
                        detailSplitNum++;
                    }
                    // суммы по деталям
                    if (key.StartsWith("Formula") && (value != "i0"))
                    {
                        drVisibility[counter] = "1";
                        var dateList = new string[8];
                        
                        for (var i = 0; i < 4; i++)
                        {
                            dateList[i * 2 + 0] = DateTime.MinValue.ToShortDateString();
                            dateList[i * 2 + 1] = DateTime.MaxValue.ToShortDateString();
                        }
                        
                        var paramCount = 2;
                        
                        if (key == "FormulaOverdatedDebt")
                        {
                            paramCount = 4;
                        }

                        var shortFormulaParam = key.Remove(key.Length - 9);

                        for (var i = 0; i < paramCount; i++)
                        {
                            dateList[i * 2 + 0] = GetConstructorDataValue(
                                reportParams[String.Format("{0}{1}TypeStart", i + 1, shortFormulaParam)],
                                reportParams[String.Format("{0}{1}Start", i + 1, shortFormulaParam)],
                                reportDate1,
                                reportDate2);

                            dateList[i * 2 + 1] = GetConstructorDataValue(
                                reportParams[String.Format("{0}{1}TypeEnd", i + 1, shortFormulaParam)],
                                reportParams[String.Format("{0}{1}End", i + 1, shortFormulaParam)],
                                reportDate1,
                                reportDate2);
                        }

                        string detailNum1, detailNum2;
                        
                        if (shortFormulaParam == "FormulaMainDebt")
                        {
                            detailNum1 = "0";
                            detailNum2 = "1";
                            if (value == "i2") detailNum1 = "3";
                            if (value == "i3") detailNum1 = "2";
                            if (docType == ConstructorType.ctGarant)
                            {
                                if (value == "i2") detailNum1 = "7";
                                if (value == "i3") detailNum1 = "3";
                            }
                            
                            if (docType == ConstructorType.ctCreditIssued)
                            {
                                detailNum1 = "3";
                                detailNum2 = "4";
                                if (value == "i2") detailNum1 = "0";
                                if (value == "i3") detailNum1 = "1";
                            }
                            
                            cdo.AddDetailColumn(String.Format("-[{0}](1>={2}1<={3})[{1}](1>={4}1<={5})",
                                    detailNum1, detailNum2, dateList[0], dateList[1], dateList[2], dateList[3]));
                        }

                        if (shortFormulaParam == "FormulaServiceDebt")
                        {
                            detailNum1 = "5";
                            detailNum2 = "4";                            
                            
                            if (docType == ConstructorType.ctCreditIssued)
                            {
                                detailNum1 = "2";
                                detailNum2 = "7";
                            }
                            
                            cdo.AddDetailColumn(String.Format("-[{0}](1>={2}1<={3})[{1}](1>={4}1<={5})",
                                    detailNum1, detailNum2, dateList[0], dateList[1], dateList[2], dateList[3]));
                        }
                        if (shortFormulaParam == "FormulaOverdatedDebt")
                        {
                            detailNum1 = "0";
                            detailNum2 = "1";
                            var detailNum3 = "5";
                            var detailNum4 = "4";

                            if (value == "i2") detailNum1 = "3";
                            if (value == "i3") detailNum1 = "2";
                            
                            if (docType == ConstructorType.ctGarant)
                            {
                                if (value == "i2") detailNum1 = "7";
                                if (value == "i3") detailNum1 = "3";
                            }
                            
                            if (docType == ConstructorType.ctCreditIssued)
                            {
                                detailNum1 = "3";
                                detailNum2 = "4";
                                detailNum3 = "2";
                                detailNum4 = "7";
                                if (value == "i2") detailNum1 = "0";
                                if (value == "i3") detailNum1 = "1";
                            }
                            
                            cdo.AddDetailColumn(String.Format("-+-[{0}](1>={4}1<={5})[{1}](1>={6}1<={7})[{2}](1>={8}1<={9})[{3}](1>={10}1<={11})",
                                    detailNum1, detailNum2, detailNum3, detailNum4,
                                    dateList[0], dateList[1], dateList[2], dateList[3],
                                    dateList[4], dateList[5], dateList[6], dateList[7]));
                        }
                    }

                    if (!key.StartsWith("Filter"))
                    {
                        counter++;
                    }
                }
            }

            dtTables[0] = cdo.FillData();
            drCaptions[maxColumnCount - 1] = reportCaption;
            drReportParams[0] = cdo.columnList.Count;
            return dtTables;
        }

        /// <summary>
        /// Общий класс заполнения таблиц для отчетов ДК Вологды
        /// </summary>
        public DataTable[] GetDebtObligationsData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[4];
            var loDate = GetParamDate(reportParams, ReportConsts.ParamStartDate);
            var hiDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var peniSumFieldName = CombineList(
                Combine(ReportConsts.Margin, ReportConsts.Commission), 
                Combine(ReportConsts.CurrencyMargin, ReportConsts.CurrencyCommission));
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPosition);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.RegNum);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctGarantTypeNumDate);
            cdoGrnt.AddDetailColumn(String.Format("[0](1<={1}1>={0})", loDate, hiDate));
            cdoGrnt.AddDetailColumn(String.Format("[1](1<={1}1>={0})", loDate, hiDate));
            cdoGrnt.AddDetailColumn(String.Format("[5](1<={1}1>={0})", loDate, hiDate));
            cdoGrnt.AddDetailColumn(String.Format("[4](1<={1}1>={0})", loDate, hiDate));
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+9;+10;+11");
            cdoGrnt.AddParamColumn(CalcColumnType.cctRelation, "+12;+13;+14");
            // служебные для вычислений
            cdoGrnt.AddDetailColumn(String.Format("[5](1<={1}1>={0})", loDate, hiDate), peniSumFieldName, false);
            cdoGrnt.AddDetailColumn(String.Format("[11](0<={1}0>={0})", loDate, hiDate));
            cdoGrnt.AddDetailColumn(String.Format("[8](1<={1}1>={0})", loDate, hiDate));
            cdoGrnt.AddDetailColumn(String.Format("[4](1<={1}1>={0})", loDate, hiDate), peniSumFieldName, false);
            cdoGrnt.AddDetailColumn(String.Format("[12](1<={1}1>={0})", loDate, hiDate));
            cdoGrnt.AddDetailColumn(String.Format("[9](1<={1}1>={0})", loDate, hiDate));
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.RegNum);
            dtTables[2] = cdoGrnt.FillData();
            // Кредиты
            var cdoCredit = new CreditDataObject();
            cdoCredit.InitObject(scheme);
            cdoCredit.AddCalcColumn(CalcColumnType.cctPosition);
            cdoCredit.AddDataColumn(f_S_Creditincome.RegNum);
            cdoCredit.AddCalcColumn(CalcColumnType.cctCreditTypeNumDate);
            cdoCredit.AddDetailColumn(String.Format("[0](1<={1}1>={0})", loDate, hiDate));
            cdoCredit.AddDetailColumn(String.Format("[1](1<={1}1>={0})", loDate, hiDate));
            cdoCredit.AddDetailColumn(String.Format("[5](1<={1}1>={0})", loDate, hiDate));
            cdoCredit.AddDetailColumn(String.Format("[4](1<={1}1>={0})", loDate, hiDate));
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCredit.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCredit.sortString = StrSortUp(f_S_Creditincome.RegNum);
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.OrgCreditCode;
            dtTables[0] = cdoCredit.FillData();
            cdoCredit.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtTables[1] = cdoCredit.FillData();
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(2);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = loDate;
            drCaption[1] = hiDate;
            return dtTables;
        }

        /// <summary>
        /// получение задолженности на определенный период времени по всем договорам
        /// </summary>
        /// <param name="reportParams"></param>
        public DataTable[] GetCreditOmskReportData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[3];
            // Параметры
            var variantFilter = ReportConsts.FixedVariantsID;
            
            if (reportParams[ReportConsts.ParamVariantType] == "i1")
            {
                variantFilter = ReportConsts.ActiveVariantID;
            }

            if (reportParams[ReportConsts.ParamVariantType] == "i3")
            {
                variantFilter = ReportConsts.ArchivVariantID;
            }

            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            // Полученные
            var sumFieldName = CreateValuePair(ReportConsts.CurrencySumField);
            var cdo = new CreditDataObject();
            cdo.InitObject(scheme);
            cdo.ignoreCurrencyCalc = true;
            cdo.mainFilter.Add(f_S_Creditincome.RefVariant, variantFilter);
            // Формирование колонок
            cdo.AddCalcNamedColumn(CalcColumnType.cctOrganization, TempFieldNames.OrgName);
            cdo.AddDataColumn(f_S_Creditincome.Num);
            cdo.AddDataColumn(f_S_Creditincome.ContractDate);
            cdo.AddDataColumn(f_S_Creditincome.GenDocInfo);
            cdo.AddDetailColumn(String.Format("[0](1<={0})", calcDate));
            cdo.AddDetailColumn(String.Format("[1](1<={0})", calcDate));
            cdo.AddDetailColumn(String.Format("+-[0](1<={0})[1](1<={0})[10](1<={0})", calcDate));
            cdo.AddDetailColumn(String.Format("-[0](1<={0})[1](1<={0})", calcDate), sumFieldName, true);
            cdo.AddCalcColumn(CalcColumnType.cctPercentText);
            cdo.AddDetailColumn(String.Format("[4](1<={0})", calcDate));
            cdo.AddDataColumn(f_S_Creditincome.EndDate);
            cdo.AddDataColumn(f_S_Creditincome.RenewalDate);
            cdo.AddCalcColumn(CalcColumnType.cctOKVName);
            // сортировочка
            cdo.AddCalcNamedColumn(CalcColumnType.cctCreditEndDate, TempFieldNames.SortEndDate);
            cdo.AddDataColumn(f_S_Creditincome.RefSTypeCredit);
            cdo.sortString = "RefSTypeCredit asc, RefOKV asc, SortEndDate asc";
            cdo.columnCondition.Add(7, FormNegativeFilterValue(f_S_Creditincome.RefOKV, ReportConsts.codeRUBStr));
            dtTables[0] = cdo.FillData();
            // Выданные
            var cdoIssued = new CreditIssuedDataObject();
            cdoIssued.InitObject(scheme);
            cdoIssued.ignoreCurrencyCalc = true;
            cdoIssued.mainFilter.Add(f_S_Creditissued.RefVariant, variantFilter);
            // Формирование колонок
            cdoIssued.AddCalcNamedColumn(CalcColumnType.cctOrganizationRegion, TempFieldNames.OrgName);
            cdoIssued.AddDataColumn(f_S_Creditissued.Num);
            cdoIssued.AddDataColumn(f_S_Creditissued.DocDate);
            cdoIssued.AddDataColumn(f_S_Creditissued.GenDocInfo);
            cdoIssued.AddDetailColumn(String.Format("[3](1<={0})", calcDate));
            cdoIssued.AddDetailColumn(String.Format("[4](1<={0})", calcDate));
            cdoIssued.AddDetailColumn(String.Format("-[3](1<={0})[4](1<={0})", calcDate));
            cdoIssued.AddDetailColumn(String.Format("-[3](1<={0})[4](1<={0})", calcDate));
            cdoIssued.AddCalcColumn(CalcColumnType.cctPercentText);
            cdoIssued.AddDetailColumn(String.Format("[7](1<={0})", calcDate));
            cdoIssued.AddDataColumn(f_S_Creditissued.EndDate);
            cdoIssued.AddDataColumn(f_S_Creditissued.RenewalDate);
            cdoIssued.AddCalcColumn(CalcColumnType.cctOKVName);
            // сортировочка
            cdoIssued.AddCalcNamedColumn(CalcColumnType.cctCreditEndDate, TempFieldNames.SortEndDate);
            cdoIssued.AddDataColumn(f_S_Creditissued.RefSTypeCredit);
            cdoIssued.sortString = "RefSTypeCredit asc, RefOKV asc, SortEndDate asc";
            cdoIssued.columnCondition.Add(7, FormNegativeFilterValue(f_S_Creditissued.RefOKV, ReportConsts.codeRUBStr));
            dtTables[1] = cdoIssued.FillData();
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(1);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = calcDate;
            return dtTables;
        }

        /// <summary>
        /// Заполнитель для "Отчет по долговой книге" Ярославль
        /// </summary>
        public DataTable[] GetDebtorBookDataYaroslavl(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[11];

            for (var i = 0; i < dtTables.Length; i++)
            {
                dtTables[i] = CreateReportCaptionTable(20);
            }

            var shortDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var dateYear0Start = GetYearStart(shortDate);
            var isFirstDay = Convert.ToDateTime(shortDate).Day == 1;
            // Заголовочные параметры
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(5);
            var drCaption = dtTables[dtTables.Length - 1].Rows.Add();
            drCaption[0] = shortDate;
            if (isFirstDay) shortDate = Convert.ToDateTime(shortDate).AddDays(-1).ToShortDateString();
            // Кредиты
            var cdoCI = new CreditDataObject();
            drCaption[1] = Convert.ToDateTime(shortDate).Month;
            drCaption[2] = GetOKTMOCode(scheme);
            cdoCI.InitObject(scheme);
            cdoCI.mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.FixedVariantsID);
            cdoCI.mainFilter.Add(f_S_Creditincome.RefSTypeCredit, ReportConsts.OrgCreditCode);
            cdoCI.mainFilter.Add(f_S_Creditincome.StartDate, String.Format("<='{0}'", shortDate));
            cdoCI.mainFilter.Add(f_S_Creditincome.EndDate,
                String.Format(">='{0}' or c.{1}>='{0}'", dateYear0Start, f_S_Creditincome.RenewalDate));
            cdoCI.AddCalcColumn(CalcColumnType.cctContractType);
            cdoCI.AddDataColumn(f_S_Creditincome.Num);
            cdoCI.AddDataColumn(f_S_Creditincome.ContractDate, ReportConsts.ftDateTime);
            cdoCI.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoCI.AddDataColumn(f_S_Creditincome.EndDate);
            cdoCI.AddCalcColumn(CalcColumnType.cctCollateralType);
            cdoCI.AddDataColumn(f_S_Creditincome.PenaltyDebtRate);
            cdoCI.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", dateYear0Start));
            cdoCI.AddCalcColumn(CalcColumnType.cctPercentText);
            cdoCI.AddDetailColumn(String.Format("[4](1<={0})", dateYear0Start));
            cdoCI.AddDetailColumn(String.Format("[5](1<={0})", dateYear0Start));
            cdoCI.AddDetailColumn(String.Format("[6](1<={0})", dateYear0Start));
            cdoCI.AddDetailColumn(String.Format("[8](1<={0})", dateYear0Start));
            cdoCI.useSummaryRow = false;
            cdoCI.sortString = StrSortUp(f_S_Creditincome.ContractDate);
            var dtContracts = cdoCI.FillData();
            FillDetailDataList(0, dtTables[0], dtTables[4], dtContracts, cdoCI, dateYear0Start, shortDate, true);
            cdoCI.mainFilter[f_S_Creditincome.RefSTypeCredit] = ReportConsts.BudCreditCode;
            dtContracts = cdoCI.FillData();
            FillDetailDataList(0, dtTables[1], dtTables[5], dtContracts, cdoCI, dateYear0Start, shortDate, false);
            // ЦБ
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.FixedVariantsID);
            cdoCap.mainFilter.Add(f_S_Capital.StartDate, String.Format("<='{0}'", shortDate));
            cdoCap.mainFilter.Add(f_S_Capital.DateDischarge, String.Format(">='{0}'", dateYear0Start));
            cdoCap.AddDataColumn(f_S_Capital.CodeCapital);
            cdoCap.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);
            cdoCap.AddCalcColumn(CalcColumnType.cctOrganization);
            cdoCap.AddDataColumn(f_S_Capital.EndDate);
            cdoCap.AddCalcColumn(CalcColumnType.cctCollateralType);
            cdoCap.AddDataColumn(f_S_Capital.Discount);
            cdoCap.AddDetailColumn(String.Format("-[5](1<{0})[1](1<{0})", dateYear0Start));
            cdoCap.AddCalcColumn(CalcColumnType.cctPercentText);
            cdoCap.AddDetailColumn(String.Format("[7](1<={0})", dateYear0Start));
            cdoCap.AddDetailColumn(String.Format("[3](1<={0})", dateYear0Start));
            cdoCap.AddDetailColumn(String.Format("[8](1<={0})", dateYear0Start));
            cdoCap.AddDetailColumn(String.Format("[9](1<={0})", dateYear0Start));
            cdoCap.sortString = StrSortUp(f_S_Capital.StartDate);
            cdoCap.useSummaryRow = false;
            dtContracts = cdoCap.FillData();
            FillDetailDataList(1, dtTables[2], dtTables[6], dtContracts, cdoCap, dateYear0Start, shortDate, false);
            // Гарантии
            var cdoGrnt = new GarantDataObject();
            cdoGrnt.InitObject(scheme);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.FixedVariantsID);
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.StartDate, String.Format("<='{0}'", shortDate));
            cdoGrnt.mainFilter.Add(f_S_Guarantissued.EndDate,
                String.Format(">='{0}' or c.{1}>='{0}'", dateYear0Start, f_S_Guarantissued.RenewalDate));
            cdoGrnt.AddCalcColumn(CalcColumnType.cctContractType);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.Num);
            cdoGrnt.AddDataColumn(f_S_Guarantissued.DateDoc);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalDoc);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalStartDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctOrganization3);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPrincipalEndDate);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctCollateralType);
            cdoGrnt.AddDetailColumn(String.Format("+[1](1<{0})[4](1<{0})", dateYear0Start));
            cdoGrnt.AddDataColumn(ReportConsts.SumField);
            cdoGrnt.AddCalcColumn(CalcColumnType.cctPercentText);
            cdoGrnt.AddDetailColumn(String.Format("[8](1<={0})", dateYear0Start));
            cdoGrnt.AddDetailColumn(String.Format("[9](1<={0})", dateYear0Start));
            cdoGrnt.AddDetailColumn(String.Format("[10](1<={0})", dateYear0Start));
            cdoGrnt.AddDataColumn(f_S_Guarantissued.StartDate, ReportConsts.ftDateTime);
            cdoGrnt.useSummaryRow = false;
            cdoGrnt.sortString = StrSortUp(f_S_Guarantissued.StartDate);
            dtContracts = cdoGrnt.FillData();
            FillDetailDataList(2, dtTables[3], dtTables[7], dtContracts, cdoGrnt, dateYear0Start, shortDate, false);
            AddRows(dtTables[8], 14, "0");
            AddRows(dtTables[9], 14, "0");
            FillSummaryData(dtTables[8], dtTables[4]);
            FillSummaryData(dtTables[8], dtTables[5]);
            FillSummaryData(dtTables[8], dtTables[6]);
            FillSummaryData(dtTables[9], dtTables[8]);
            FillSummaryData(dtTables[9], dtTables[7]);
            return dtTables;
        }
    }
}