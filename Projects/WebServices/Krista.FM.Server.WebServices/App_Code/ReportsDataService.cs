using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.WebServices
{

/// <summary>
/// Сводное описание для ReportsDataService
/// </summary>
    public class ReportsDataService
    {
        public ReportsDataService()
        {
            //
            // TODO: добавьте логику конструктора
            //
        }

        #region месячные отчеты

        public ReportByMonth[] GetMonthReportsData(IScheme scheme, QueryByMonth monthQuery)
        {
            int currentMonth = monthQuery.Year * 10000 + monthQuery.Month * 100;
            int lastMonth = (monthQuery.Year - 1) * 10000 + monthQuery.Month * 100;
            List<ReportByMonth> reportList = new List<ReportByMonth>();

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                foreach (string code in monthQuery.ClassifierCodes)
                {
                    ReportByMonth report = new ReportByMonth();
                    string currentQuery = string.Empty;
                    string lastQuery = string.Empty;
                    GetMonthQuery(monthQuery, code, ref currentQuery, ref lastQuery);

                    DataTable currentData = (DataTable)db.ExecQuery(currentQuery, QueryResultTypes.DataTable,
                        new DbParameterDescriptor("p0", code), new DbParameterDescriptor("p1", currentMonth),
                        new DbParameterDescriptor("p2", monthQuery.BudgetLevel), new DbParameterDescriptor("p3", monthQuery.MunicipalCode));

                    DataTable latsData = (DataTable)db.ExecQuery(currentQuery, QueryResultTypes.DataTable,
                        new DbParameterDescriptor("p0", code), new DbParameterDescriptor("p1", lastMonth),
                        new DbParameterDescriptor("p2", monthQuery.BudgetLevel), new DbParameterDescriptor("p3", monthQuery.MunicipalCode));
                    report.classifierCode = code;
                    report.classifierType = monthQuery.ClassifierType;
                    report.factForMonth = currentData.Rows.Count > 0
                                              ? currentData.Rows[0].IsNull("FactReport")
                                                    ? 0
                                                    : Convert.ToDouble(currentData.Rows[0]["FactReport"])
                                              : 0;
                    report.planForYear = currentData.Rows.Count > 0
                                              ? currentData.Rows[0].IsNull("YearPlanReport")
                                                    ? 0
                                                    : Convert.ToDouble(currentData.Rows[0]["YearPlanReport"])
                                              : 0;

                    report.factForMonthPrevYear = latsData.Rows.Count > 0
                                              ? latsData.Rows[0].IsNull("FactReport")
                                                    ? 0
                                                    : Convert.ToDouble(latsData.Rows[0]["FactReport"])
                                              : 0;
                    report.implementPercent = report.planForYear == 0 ? 0 : report.factForMonth/report.planForYear;
                    report.increaseRate = report.factForMonthPrevYear == 0
                                              ? 0
                                              : report.factForMonth/report.factForMonthPrevYear;
                    reportList.Add(report);
                }
                return reportList.ToArray();
            }
        }

        private void GetMonthQuery(QueryByMonth monthQuery, string code, ref string currentQuery, ref string lastQuery)
        {
            switch (monthQuery.ClassifierType)
            {
                case 0:
                    currentQuery = @"select currentYear.FactReport, currentYear.YearPlanReport from 
                    f_F_MonthRepIncomes currentYear
                    where currentYear.RefKD in (select id from d_KD_MonthRep where RefKDBridge in (select id from b_KD_Bridge where CodeStr = ?))
                    and currentYear.RefYearDayUNV = ? and currentYear.RefBdgtLevels = ? 
                    and currentYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";

                    lastQuery = @"select lastYear.FactReport from 
                    f_F_MonthRepIncomes lastYear 
                    where lastYear.RefKD in (select id from d_KD_MonthRep where RefKDBridge in (select id from b_KD_Bridge where CodeStr = ?))
                    and lastYear.RefYearDayUNV = ? and lastYear.RefBdgtLevels = ? 
                    and lastYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";
                    break;
                case 1:
                    currentQuery = @"select currentYear.FactReport, currentYear.YearPlanReport, lastYear.FactReport from 
                    f_F_MonthRepOutcomes currentYear, f_F_MonthRepOutcomes lastYear 
                    where currentYear.RefFKR in (select id from d_R_MonthRep where RefBridgeFKR in (select id from b_FKR_Bridge where Code = ?))
                    and current.RefYearDayUNV = ? and current.RefBdgtLevels = ? 
                    and currentYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";

                    lastQuery = @"select lastYear.FactReport from 
                    f_F_MonthRepOutcomes lastYear 
                    where lastYear.RefFKR in (select id from d_R_MonthRep where RefBridgeFKR in (select id from b_FKR_Bridge where Code = ?))
                    and lastYear.RefYearDayUNV = ? and lastYear.RefBdgtLevels = ? 
                    and lastYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";
                    break;
                case 2:
                    if (code.Substring(0, 5) == "00002")
                    {
                        currentQuery = @"select currentYear.FactReport, currentYear.YearPlanReport, lastYear.FactReport from 
                    f_F_MonthRepOutFin currentYear, f_F_MonthRepOutFin lastYear 
                    where currentYear.RefSOF in (select id from d_SOF_MonthRep where RefKIFBridge in (select id from b_KIF_Bridge where CodeStr = ?))
                    and currentYear.RefYearDayUNV = ? and currentYear.RefBdgtLevels = ? 
                    and currentYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";

                        lastQuery = @"select lastYear.FactReport from 
                    f_F_MonthRepOutFin lastYear 
                    where lastYear.RefSOF in (select id from d_SOF_MonthRep where RefKIFBridge in (select id from b_KIF_Bridge where CodeStr = ?))
                    and lastYear.RefYearDayUNV = ? and lastYear.RefBdgtLevels = ? 
                    and lastYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";
                    }
                    else
                    {
                        currentQuery = @"select currentYear.FactReport, currentYear.YearPlanReport, lastYear.FactReport from 
                    f_F_MonthRepInFin currentYear, f_F_MonthRepInFin lastYear 
                    where currentYear.RefSIF in (select id from d_SIF_MonthRep where RefKIFBridge in (select id from b_KIF_Bridge where CodeStr = ?))
                    and currentYear.RefYearDayUNV = ? and currentYear.RefBdgtLevels = ? 
                    and currentYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";

                        lastQuery = @"select lastYear.FactReport from 
                    f_F_MonthRepInFin lastYear 
                    where lastYear.RefSIF in (select id from d_SIF_MonthRep where RefKIFBridge in (select id from b_KIF_Bridge where CodeStr = ?))
                    and lastYear.RefYearDayUNV = ? and lastYear.RefBdgtLevels = ? 
                    and lastYear.RefRegions in (select id from d_Regions_MonthRep where CodeStr = ?)";
                    }
                    
                    break;
            }
        }

        #endregion

        #region годовой отчет

        public ReportByYear GetYearReportData(IScheme scheme, QueryByYear yearQuery)
        {
            int currentYear = yearQuery.Year * 10000 + 001;
            ReportByYear report = new ReportByYear();
            List<BudgetIndicator> indicators = new List<BudgetIndicator>();
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                foreach (string code in yearQuery.ClassifierCodes)
                {
                    string dataQuery = string.Empty;
                    GetYearReportQuery(yearQuery, code, ref dataQuery);
                    DataTable dtData = (DataTable)db.ExecQuery(dataQuery, QueryResultTypes.DataTable,
                                 new DbParameterDescriptor("p0", code), 
                                 new DbParameterDescriptor("p1", currentYear),
                                 new DbParameterDescriptor("p2", yearQuery.MunicipalCode),
                                 new DbParameterDescriptor("p3", yearQuery.BudgetLevel));
                    if (dtData.Rows.Count > 0)
                    {
                        BudgetIndicator indicator = new BudgetIndicator();
                        indicator.classifierCode = code;
                        indicator.classifierType = yearQuery.ClassifierType;
                        indicator.factValue = dtData.Rows[0].IsNull("PerformedReport")
                                                  ? 0
                                                  : Convert.ToDouble(dtData.Rows[0]["PerformedReport"]);
                        indicator.planValue = dtData.Rows[0].IsNull("AssignedReport")
                                                  ? 0
                                                  : Convert.ToDouble(dtData.Rows[0]["AssignedReport"]);
                        indicators.Add(indicator);
                    }
                }
                report.BudgetStats = indicators.ToArray();

                DataTable dtBudgetTotal = (DataTable)db.ExecQuery(
                    @"select data.AssignedReport, data.PerformedReport from f_DP_FOYRDefProf data 
                    where data.RefYearDayUNV = ? and data.RefBdgtLevels = ? and
                    data.RefRegions in (select id from d_Regions_FOYR where CodeStr = ?)",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", currentYear),
                    new DbParameterDescriptor("p1", yearQuery.BudgetLevel),
                    new DbParameterDescriptor("p1", yearQuery.MunicipalCode));
                if (dtBudgetTotal.Rows.Count > 0)
                {
                    BudgetSummary summary = new BudgetSummary();
                    summary.planValue = dtBudgetTotal.Rows[0].IsNull("AssignedReport")
                                            ? 0
                                            : Convert.ToDouble(dtBudgetTotal.Rows[0]["AssignedReport"]);
                    summary.factValue = dtBudgetTotal.Rows[0].IsNull("PerformedReport")
                                            ? 0
                                            : Convert.ToDouble(dtBudgetTotal.Rows[0]["PerformedReport"]);
                    report.budgetTotal = summary;
                }
            }

            return report;
        }

        private void GetYearReportQuery(QueryByYear yearQuery, string code, ref string dataQuery)
        {
            switch (yearQuery.ClassifierType)
            {
                case 0:
                    dataQuery =
                        @"select data.AssignedReport, data.PerformedReport from 
                    f_D_FOYRIncomes data where 
                    data.RefKD in (select id from d_KD_FOYR where RefKDBridge in (select id from b_KD_Bridge where CodeStr = ?)) and
                    data.RefYearDayUNV = ? and data.RefRegions in (select id from d_Regions_FOYR where CodeStr = ?) and
                    data.RefBdgtLevels = ?";
                    break;
                case 1:
                    if (code.Length == 3)
                    {
                        // EKR   
                        dataQuery = @"select data.AssignedReport, data.PerformedReport from 
                    f_R_FOYROutcomes data where 
                    data.RefEKRFOYR in (select id from d_EKR_FOYR where RefEKRBridge in (select id from b_EKR_Bridge where Code = ?)) and
                    data.RefYearDayUNV = ? and data.RefRegions in (select id from d_Regions_FOYR where CodeStr = ?) and
                    data.RefBdgtLevels = ?";
                    }
                    else
                    {
                        // FKR
                        dataQuery = @"select data.AssignedReport, data.PerformedReport from 
                    f_D_FOYRIncomes data where 
                    data.RefFKR in (select id from d_FKR_FOYR where RefBridge in (select id from b_FKR_Bridge where Code = ?)) and
                    data.RefYearDayUNV = ? and data.RefRegions in (select id from d_Regions_FOYR where CodeStr = ?) and
                    data.RefBdgtLevels = ?";
                    }
                    break;
                case 2:
                    dataQuery =
                        @"select data.AssignedReport, data.PerformedReport from 
                    f_D_FOYRIncomes data where 
                    data.RefKIF2005 in (select id from d_KIF_FOYR2005 where RefBridge in (select id from b_KIF_Bridge where CodeStr = ?)) and
                    data.RefYearDayUNV = ? and data.RefRegions in (select id from d_Regions_FOYR where CodeStr = ?) and
                    data.RefBdgtLevels = ?";
                    break;
            }
        }

        #endregion
    }
}