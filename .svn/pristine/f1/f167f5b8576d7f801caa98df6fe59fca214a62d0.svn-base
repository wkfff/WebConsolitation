using System;
using System.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees
{
    public class GuaranteeReportServer
    {
        // Запросы по гарантиям
        const string guranteeQuery = "select grnt.ID, org.Name, contract.Name, grnt.Num, grnt.StartDate, " +
            "grnt.Sum, grnt.CurrencySum, grnt.RefOKV from {0} grnt, {1} org, {2} contract " +
            "where grnt.RefVariant = {3} and grnt.RefOKV = -1 and org.ID = grnt.RefOrganizationsPlan3 and contract.ID = grnt.RefTypeContract";
        const string currencyGuranteeQuery = "select grnt.ID, org.Name, contract.Name, grnt.Num, grnt.StartDate, " +
            "grnt.Sum, grnt.CurrencySum, grnt.RefOKV from {0} grnt, {1} org, {2} contract " +
            "where grnt.RefVariant = {3} and grnt.RefOKV <> -1 and org.ID = grnt.RefOrganizationsPlan3 and contract.ID = grnt.RefTypeContract";

        #region запросы по детали
        // План погашения основного долга
        const string planDebtQuery = "select Sum, CurrencySum, StartDate, EndDate from t_S_PlanDebtPrGrnt where RefGrnt = {0}";
        // Факт погашения основного долга
        const string factDebtQuery = "select Sum, CurrencySum, FactDate from t_S_FactDebtPrGrnt where RefGrnt = {0}" ;
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
        #endregion


        private readonly IScheme _scheme;
        public GuaranteeReportServer(IScheme scheme)
        {
            _scheme = scheme;
        }

        public DataTable[] GetGovernmentGuaranteeReportData(DateTime reportDate, int currentVariant)
        {
            IEntity grntEntity = _scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Guarantissued_Key);
            IEntity orgEntity = _scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
            IEntity contractEntity = _scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);

            DataTable[] tables = new DataTable[3];

            #region получение данных
            using (IDatabase db = _scheme.SchemeDWH.DB)
            {
                // дата начала отчетного месяца
                DateTime startDate = new DateTime(reportDate.Year, reportDate.Month, 1);
                // данные по валютным гарантиям
                DataTable guaranteeData =
                    (DataTable)db.ExecQuery(string.Format(currencyGuranteeQuery, grntEntity.FullDBName, orgEntity.FullDBName, contractEntity.FullDBName, 0), QueryResultTypes.DataTable);
                tables[0] = GetGuaranteeData(guaranteeData, db, startDate, reportDate, "CurrencySum", "CurrencyMargin", "CurrencyCommission");
                // данные по гарантиям в рублях
                guaranteeData =
                    (DataTable)db.ExecQuery(string.Format(guranteeQuery, grntEntity.FullDBName, orgEntity.FullDBName, contractEntity.FullDBName, 0), QueryResultTypes.DataTable);
                tables[1] = GetGuaranteeData(guaranteeData, db, startDate, reportDate, "Sum", "Margin", "Commission");

                tables[2] = new DataTable();
                tables[2].Columns.Add("StartDate", typeof(string));
                tables[2].Columns.Add("ReportDate", typeof(string));
                tables[2].Columns.Add("CurrencyRate", typeof(decimal));
                DataRow newRow = tables[2].NewRow();
                newRow[0] = startDate.ToShortDateString();
                newRow[1] = reportDate.ToShortDateString();
                DataTable dt = (DataTable)
                    db.ExecQuery("select ExchangeRate, DateFixing from d_S_ExchangeRate where DateFixing <= ? and RefOKV = 131", QueryResultTypes.DataTable,
                    new System.Data.OleDb.OleDbParameter("FactDate", reportDate));
                decimal currencyRate = 0;
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Select(string.Empty, "DateFixing DESC")[0];
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

        private const string planDataFilter = "EndDate <= '{0}'";
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

        private static DataTable GetGuaranteeData(DataTable dtGuarantee, IDatabase db,
            DateTime startDate, DateTime endDate, string sumColumnName, string marginColumnName, string commissionColumnName)
        {
            DataTable dtResult = GetDataTable();
            int i = 1;
            foreach (DataRow row in dtGuarantee.Rows)
            {
                DataRow reportRow = dtResult.NewRow();
                object guaranteeID = row["ID"];

                reportRow[0] = i++;
                reportRow["DocName"] = row["Name1"] + " " + row["Num"] + " " + Convert.ToDateTime(row["StartDate"]).ToShortDateString();
                reportRow["CreditorName"] = row["Name"];
                reportRow["Sum"] = Convert.ToInt32(row["RefOKV"]) == -1 ? row["Sum"] : row["CurrencySum"];
                reportRow["Rur"] = Convert.ToInt32(row["RefOKV"]) == -1 ? "Рос.руб." : string.Empty;
                // идем по всем выбранным гарантиям и получаем данные из детали
                DataTable dtPlanDebt = (DataTable)db.ExecQuery(string.Format(planDebtQuery, guaranteeID), QueryResultTypes.DataTable);
                DataTable dtFactDebt = (DataTable)db.ExecQuery(string.Format(factDebtQuery, guaranteeID), QueryResultTypes.DataTable);
                DataTable dtPlanAttract = (DataTable)db.ExecQuery(string.Format(planAttractQuery, guaranteeID), QueryResultTypes.DataTable);
                DataTable dtFactAttract = (DataTable)db.ExecQuery(string.Format(factAttractQuery, guaranteeID), QueryResultTypes.DataTable);
                DataTable dtPlanService = (DataTable)db.ExecQuery(string.Format(planServiceQuery, guaranteeID), QueryResultTypes.DataTable);
                DataTable dtFactService = (DataTable)db.ExecQuery(string.Format(factServiceQuery, guaranteeID), QueryResultTypes.DataTable);
                DataTable dtPlanAtractPr = (DataTable)db.ExecQuery(string.Format(planAttractPrQuery, guaranteeID), QueryResultTypes.DataTable);

                // основной долг. изменения
                decimal mainDebtMonthSum = 0;
                mainDebtMonthSum += GetRowsColumnSum(dtPlanDebt.Select(string.Format( planDataMonthFilter, startDate, endDate)),
                    sumColumnName);
                mainDebtMonthSum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planDebtMonthFilter, startDate, endDate)),
                    sumColumnName);
                mainDebtMonthSum -= GetRowsColumnSum(dtFactDebt.Select(string.Format(dataMonthFilter, startDate, endDate)),
                    sumColumnName);
                mainDebtMonthSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(debtMonthFilter, startDate, endDate)),
                    sumColumnName);

                // проценты. изменения
                decimal percentDebtMonthSum = 0;
                percentDebtMonthSum += GetRowsColumnSum(dtPlanService.Select(string.Format(planDataMonthFilter, startDate, endDate)),
                    sumColumnName);
                percentDebtMonthSum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planPercentMonthFilter, startDate, endDate)),
                    sumColumnName);
                percentDebtMonthSum -= GetRowsColumnSum(dtFactService.Select(string.Format(dataMonthFilter, startDate, endDate)),
                    sumColumnName);
                percentDebtMonthSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(percentMonthFilter, startDate, endDate)),
                    sumColumnName);
                // изменения по марже
                decimal marjaMonthSum = 0;
                marjaMonthSum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planMarjaMonthFilter, startDate, endDate)),
                    sumColumnName);
                marjaMonthSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(marjaMonthFilter, startDate, endDate)),
                    sumColumnName);
                marjaMonthSum += GetRowsColumnSum(dtPlanService.Select(string.Format(planDataMonthFilter, startDate, endDate)),
                    marginColumnName);
                marjaMonthSum -= GetRowsColumnSum(dtFactService.Select(string.Format(dataMonthFilter, startDate, endDate)),
                    marginColumnName);

                // изменения по комиссии
                decimal commissionMonthSum = 0;
                commissionMonthSum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planCommissionMonthFilter, startDate, endDate)),
                    sumColumnName);
                commissionMonthSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(commissionMonthFilter, startDate, endDate)),
                    sumColumnName);
                commissionMonthSum += GetRowsColumnSum(dtPlanService.Select(string.Format(planDataMonthFilter, startDate, endDate)),
                    commissionColumnName);
                commissionMonthSum -= GetRowsColumnSum(dtFactService.Select(string.Format(dataMonthFilter, startDate, endDate)),
                    commissionColumnName);

                // изменения по пени
                decimal penaltyMonthSum = 0;
                penaltyMonthSum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planPenaltyMonthFilter, startDate, endDate)),
                    sumColumnName);
                penaltyMonthSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(penaltyMonthFilter, startDate, endDate)),
                    sumColumnName);

                //Основной долг 
                decimal mainDebtSum = GetRowsColumnSum(dtPlanAtractPr.Select(string.Format("StartDate <= '{0}'", endDate)),
                    sumColumnName);
                mainDebtSum -= GetRowsColumnSum(dtFactDebt.Select(string.Format(dataFilter, endDate)),
                    sumColumnName);
                mainDebtSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(debtFilter, endDate)),
                    sumColumnName);
                // проценты
                decimal percentSum = GetRowsColumnSum(dtPlanService.Select(string.Empty), sumColumnName);
                percentSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(percentFilter, endDate)), sumColumnName);
                percentSum -= GetRowsColumnSum(dtFactService.Select(string.Format(dataFilter, endDate)), sumColumnName);
                // маржа
                decimal marjaSum = 0;
                marjaSum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planMarjaFilter, endDate)), sumColumnName);
                marjaSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(marjaFilter, endDate)), sumColumnName);
                marjaSum += GetRowsColumnSum(dtPlanService.Select(), marginColumnName);
                marjaSum -= GetRowsColumnSum(dtFactService.Select(string.Format(dataFilter, endDate)), marginColumnName);
                // комиссия
                decimal commissionSum = 0;
                commissionSum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planCommissionFilter, endDate)), sumColumnName);
                commissionSum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(commissionFilter, endDate)), sumColumnName);
                commissionSum += GetRowsColumnSum(dtPlanService.Select(), commissionColumnName);
                commissionSum -= GetRowsColumnSum(dtFactService.Select(string.Format(dataFilter, endDate)), commissionColumnName);
                // пени
                decimal penaltySum = 0;
                penaltySum += GetRowsColumnSum(dtPlanAttract.Select(string.Format(planPenaltyFilter, endDate)), sumColumnName);
                object queryResult = db.ExecQuery(string.Format("select Sum({0}) from t_S_ChargePenaltyDebtPrGrnt where RefGrnt = {1} and StartDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("StartDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum += Convert.ToDecimal(queryResult);
                queryResult = db.ExecQuery(string.Format("select Sum({0}) from t_S_PrGrntChargePenaltyPercent where RefGrnt = {1} and StartDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("StartDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum += Convert.ToDecimal(queryResult);
                queryResult = db.ExecQuery(string.Format("select Sum({0}) from t_S_FactPenaltyDebtPrGrnt where RefGrnt = {1} and FactDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("FactDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum -= Convert.ToDecimal(queryResult);
                queryResult = db.ExecQuery(string.Format("select Sum({0}) from t_S_FactPenaltyPercentPrGrnt where RefGrnt = {1} and FactDate <= ?", sumColumnName, row["ID"]),
                    QueryResultTypes.Scalar, new System.Data.OleDb.OleDbParameter("FactDate", endDate));
                if (!(queryResult is DBNull))
                    penaltySum -= Convert.ToDecimal(queryResult);
                penaltySum -= GetRowsColumnSum(dtFactAttract.Select(string.Format(penaltyFilter, endDate)), sumColumnName);

                decimal resultSum = mainDebtSum + percentSum + marjaSum + commissionSum + penaltySum;

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

        private static decimal GetRowsColumnSum(DataRow[] rows, string dataColumnName)
        {
            decimal sum = 0;
            foreach (DataRow row in rows)
            {
                sum += row.IsNull(dataColumnName) ? 0 : Convert.ToDecimal(row[dataColumnName]);
            }
            return sum;
        }

        /// <summary>
        /// таблица со структурой отчета
        /// </summary>
        /// <returns></returns>
        private static DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DocNum", typeof(int));
            dt.Columns.Add("DocName", typeof(string));
            dt.Columns.Add("CreditorName", typeof(string));
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
