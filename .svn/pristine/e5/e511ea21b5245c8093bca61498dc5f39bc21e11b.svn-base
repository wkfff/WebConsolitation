using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.DDE
{
    public class DDEService
    {
        private readonly IScheme scheme;

        readonly FinSourcesRererencesUtils finSourcesRererencesUtils;

        private string oktmo;

        public DDEService(IScheme scheme)
        {
            this.scheme = scheme;
            finSourcesRererencesUtils = new FinSourcesRererencesUtils(scheme);
            oktmo = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString();
        }

        public DataTable CalculateDDE(DdeCalculationParams calculationParams, int sourceId)
        {
            IEntity ddeEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_ContentDebt);

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable dtDde = new DataTable();
                using (IDataUpdater du = ddeEntity.GetDataUpdater("1 = 2", null))
                {
                    du.Fill(ref dtDde);
                    DateTime startPeriod = DateTime.Today;
                    DateTime endPeriod = DateTime.Today;
                    if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter && calculationParams.SplitQuarterToMonths)
                    {
                        int firstMonthInQuarter = GetFirstMonth(calculationParams.Quarter);
                        decimal quarterIncome = 0;
                        decimal quarterCurrentCharge = 0;
                        decimal quarterChangeRemains = 0;
                        decimal quarterSafetyStock = 0;
                        decimal quarterPlanDebt = 0;
                        decimal quarterPlanService = 0;
                        decimal quarterContLiability = 0;
                        decimal quarterConsPlan = 0;
                        decimal quarterContentDebt = 0;
                        decimal quarterAvailableDebt = 0;
                        for (int i = firstMonthInQuarter; i <= firstMonthInQuarter + 2; i++)
                        {
                            startPeriod = new DateTime(calculationParams.Year, i, 1, 0, 0, 0);
                            endPeriod = new DateTime(calculationParams.Year, i, DateTime.DaysInMonth(calculationParams.Year, i), 0, 0, 0);
                            string period = calculationParams.Year + calculationParams.Month.ToString().PadLeft(2, '0') + "00";
                            DataRow newRow = dtDde.NewRow();
                            newRow.BeginEdit();
                            decimal income = GetIncome(calculationParams, i, db);
                            decimal currentCharge = GetCurrentCharge(calculationParams, i, db);
                            decimal changeRemains = calculationParams.ConsiderRestChange ? GetChangeRemains(calculationParams, i, db) : 0;
                            decimal safetyStock = GetSafetyStock(calculationParams, i, db);
                            decimal planDebt = GetPlanDebt(calculationParams, startPeriod, endPeriod, db);
                            decimal planService = GetPlanService(calculationParams, startPeriod, endPeriod, db);
                            decimal contLiability = GetContLiability(calculationParams, startPeriod, endPeriod, db);
                            decimal consPlan = planDebt + planService + contLiability;
                            decimal contentDebt = income - currentCharge + changeRemains + safetyStock;
                            decimal availableDebt = contentDebt - consPlan;

                            quarterIncome += income;
                            quarterCurrentCharge += currentCharge;
                            quarterChangeRemains += changeRemains;
                            quarterSafetyStock += safetyStock;
                            quarterPlanDebt += planDebt;
                            quarterPlanService += planService;
                            quarterContLiability += contLiability;
                            quarterConsPlan += consPlan;
                            quarterContentDebt += contentDebt;
                            quarterAvailableDebt += availableDebt;

                            newRow["SourceID"] = sourceId;
                            newRow["TaskID"] = -1;
                            newRow["Income"] = income;
                            newRow["CurrentCharge"] = currentCharge;
                            newRow["ChangeRemains"] = changeRemains;
                            newRow["SafetyStock"] = safetyStock;
                            newRow["PlanDebt"] = planDebt;
                            newRow["PlanService"] = planService;
                            newRow["ContLiability"] = contLiability;
                            newRow["ConsPlan"] = consPlan;
                            newRow["ContentDebt"] = contentDebt;
                            newRow["AvailableDebt"] = availableDebt;
                            newRow["RefBrwVariant"] = calculationParams.PlaningVariant;
                            newRow["CalcComment"] = "";
                            newRow["RefIncVariant"] = -1;
                            newRow["RefRVariant"] = -1;
                            newRow["RefYearDayUNV"] = period;
                            newRow["Caption"] = GetCaption(calculationParams, i);
                            newRow.EndEdit();
                            dtDde.Rows.Add(newRow);
                        }
                        string quarterPeriod = calculationParams.Year + "999" + calculationParams.Quarter;
                        DataRow quarterRow = dtDde.NewRow();
                        quarterRow.BeginEdit();
                        quarterRow["SourceID"] = sourceId;
                        quarterRow["TaskID"] = -1;
                        quarterRow["Income"] = quarterIncome;
                        quarterRow["CurrentCharge"] = quarterCurrentCharge;
                        quarterRow["ChangeRemains"] = quarterChangeRemains;
                        quarterRow["SafetyStock"] = quarterSafetyStock;
                        quarterRow["PlanDebt"] = quarterPlanDebt;
                        quarterRow["PlanService"] = quarterPlanService;
                        quarterRow["ContLiability"] = quarterContLiability;
                        quarterRow["ConsPlan"] = quarterConsPlan;
                        quarterRow["ContentDebt"] = quarterContentDebt;
                        quarterRow["AvailableDebt"] = quarterAvailableDebt;
                        quarterRow["RefBrwVariant"] = calculationParams.PlaningVariant;
                        quarterRow["CalcComment"] = "";
                        quarterRow["RefIncVariant"] = -1;
                        quarterRow["RefRVariant"] = -1;
                        quarterRow["RefYearDayUNV"] = quarterPeriod;
                        quarterRow["Caption"] = GetCaption(calculationParams, 0);
                        quarterRow.EndEdit();
                        dtDde.Rows.Add(quarterRow);
                    }
                    else
                    {
                        startPeriod = GetStartDate(calculationParams);
                        endPeriod = GetEndDate(calculationParams);
                        string period = string.Empty;
                        if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
                            period = calculationParams.Year + "0001";
                        if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
                            period = calculationParams.Year + calculationParams.Month.ToString().PadLeft(2, '0') + "00";
                        if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter)
                            period = calculationParams.Year + "999" + calculationParams.Quarter;
                        DataRow newRow = dtDde.NewRow();
                        newRow.BeginEdit();
                        decimal income = GetIncome(calculationParams, 0, db);
                        decimal currentCharge = GetCurrentCharge(calculationParams, 0, db);
                        decimal changeRemains = calculationParams.ConsiderRestChange ? GetChangeRemains(calculationParams, 0, db) : 0;
                        decimal safetyStock = GetSafetyStock(calculationParams, 0, db);
                        decimal planDebt = GetPlanDebt(calculationParams, startPeriod, endPeriod, db);
                        decimal planService = GetPlanService(calculationParams, startPeriod, endPeriod, db);
                        decimal contLiability = GetContLiability(calculationParams, startPeriod, endPeriod, db);
                        decimal consPlan = planDebt + planService + contLiability;
                        decimal contentDebt = income - currentCharge + changeRemains + safetyStock;
                        decimal availableDebt = contentDebt - consPlan;
                        newRow["SourceID"] = sourceId;
                        newRow["TaskID"] = -1;
                        newRow["Income"] = income;
                        newRow["CurrentCharge"] = currentCharge;
                        newRow["ChangeRemains"] = changeRemains;
                        newRow["SafetyStock"] = safetyStock;
                        newRow["PlanDebt"] = planDebt;
                        newRow["PlanService"] = planService;
                        newRow["ContLiability"] = contLiability;
                        newRow["ConsPlan"] = consPlan;
                        newRow["ContentDebt"] = contentDebt;
                        newRow["AvailableDebt"] = availableDebt;
                        newRow["RefBrwVariant"] = calculationParams.PlaningVariant;
                        newRow["CalcComment"] = "";
                        newRow["RefIncVariant"] = -1;
                        newRow["RefRVariant"] = -1;
                        newRow["RefYearDayUNV"] = period;
                        newRow["Caption"] = GetCaption(calculationParams, calculationParams.Month);
                        newRow.EndEdit();
                        dtDde.Rows.Add(newRow);
                    }
                }
                return dtDde;
            }
        }

        private string GetCaption(DdeCalculationParams calculationParams, int month)
        {
            if ((calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter && month > 0 && calculationParams.SplitQuarterToMonths) ||
                calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
            {
                return GetMonth(month) + " " + calculationParams.Year + " года";
            }

            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
                return calculationParams.Year + " год";

            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                calculationParams.SplitQuarterToMonths)
                return "Итого " + calculationParams.Quarter + " квартал " + calculationParams.Year + " года";

            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                !calculationParams.SplitQuarterToMonths)
                return calculationParams.Quarter + " квартал " + calculationParams.Year + " года";

            return string.Empty;
        }

        private string GetMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "Январь";
                case 2:
                    return "Февраль";
                case 3:
                    return "Март";
                case 4:
                    return "Апрель";
                case 5:
                    return "Май";
                case 6:
                    return "Июнь";
                case 7:
                    return "Июль";
                case 8:
                    return "Август";
                case 9:
                    return "Сентябрь";
                case 10:
                    return "Октябрь";
                case 11:
                    return "Ноябрь";
                case 12:
                    return "Декабрь";
            }
            return string.Empty;
        }

        private DateTime GetStartDate(DdeCalculationParams calculationParams)
        {
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
                return new DateTime(calculationParams.Year, 1, 1, 0, 0, 0);
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter)
            {
                return new DateTime(calculationParams.Year, GetFirstMonth(calculationParams.Quarter), 1, 0, 0, 0);
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
            {
                return new DateTime(calculationParams.Year, calculationParams.Month, 1, 0, 0, 0);
            }
            return DateTime.Today;
        }

        private DateTime GetEndDate(DdeCalculationParams calculationParams)
        {
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
                return new DateTime(calculationParams.Year, 12, DateTime.DaysInMonth(calculationParams.Year, 12), 0, 0, 0);
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter)
            {
                return new DateTime(calculationParams.Year, GetLastMonth(calculationParams.Quarter),
                    DateTime.DaysInMonth(calculationParams.Year, GetLastMonth(calculationParams.Quarter)), 0, 0, 0);
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
            {
                return new DateTime(calculationParams.Year, calculationParams.Month, DateTime.DaysInMonth(calculationParams.Year, calculationParams.Month), 0, 0, 0);
            }
            return DateTime.Today;
        }

        private int GetFirstMonth(int quarter)
        {
            switch (quarter)
            {
                case 1:
                    return 1;
                case 2:
                    return 4;
                case 3:
                    return 7;
                case 4:
                    return 10;
            }
            return -1;
        }

        private int GetLastMonth(int quarter)
        {
            switch (quarter)
            {
                case 1:
                    return 3;
                case 2:
                    return 6;
                case 3:
                    return 9;
                case 4:
                    return 12;
            }
            return -1;
        }

        private decimal GetIncome(DdeCalculationParams calculationParams, int month, IDatabase db)
        {
            object queryResult = null;
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery("select Sum(Income) from f_S_SrcDataCDbt where Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                !calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery("select Sum(Income) from f_S_SrcDataCDbt where Year = ? and Quarter = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Quarter),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
            {
                queryResult = db.ExecQuery("select Sum(Income) from f_S_SrcDataCDbt where Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
            {
                queryResult = db.ExecQuery("select Sum(Income) from f_S_SrcDataCDbt where Year = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (queryResult != null && queryResult != DBNull.Value)
                return Convert.ToDecimal(queryResult);
            return 0;
        }

        private decimal GetCurrentCharge(DdeCalculationParams calculationParams, int month, IDatabase db)
        {
            string query = calculationParams.ReduceCharge ?
                "select Sum(COALESCE(Charge, 0) - COALESCE(SrvcDbtCharge, 0) - COALESCE(BdgtInvCharge, 0)) from f_S_SrcDataCDbt where " :
                "select Sum(COALESCE(Charge, 0) - COALESCE(SrvcDbtCharge, 0)) from f_S_SrcDataCDbt where ";

            object queryResult = null;
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery(query + "Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                !calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery(query + "Year = ? and Quarter = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Quarter),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
            {
                queryResult = db.ExecQuery(query + "Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
            {
                queryResult = db.ExecQuery(query + "Year = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (queryResult != null && queryResult != DBNull.Value)
                return Convert.ToDecimal(queryResult);
            return 0;
        }

        private decimal GetChangeRemains(DdeCalculationParams calculationParams, int month, IDatabase db)
        {
            object queryResult = null;
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery("select Sum(ChangeRemains) from f_S_SrcDataCDbt where Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                !calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery("select Sum(ChangeRemains) from f_S_SrcDataCDbt where Year = ? and Quarter = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Quarter),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
            {
                queryResult = db.ExecQuery("select Sum(ChangeRemains) from f_S_SrcDataCDbt where Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
            {
                queryResult = db.ExecQuery("select Sum(ChangeRemains) from f_S_SrcDataCDbt where Year = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (queryResult != null && queryResult != DBNull.Value)
                return Convert.ToDecimal(queryResult);
            return 0;
        }

        private decimal GetSafetyStock(DdeCalculationParams calculationParams, int month, IDatabase db)
        {
            object queryResult = null;
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery("select Sum(SafetyStock) from f_S_SrcDataCDbt where Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter &&
                !calculationParams.SplitQuarterToMonths)
            {
                queryResult = db.ExecQuery("select Sum(SafetyStock) from f_S_SrcDataCDbt where Year = ? and Quarter = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Quarter),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
            {
                queryResult = db.ExecQuery("select Sum(SafetyStock) from f_S_SrcDataCDbt where Year = ? and Month = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.Month),
                             new DbParameterDescriptor("p2", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
            {
                queryResult = db.ExecQuery("select Sum(SafetyStock) from f_S_SrcDataCDbt where Year = ? and IsPlan = ?",
                             QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", calculationParams.Year),
                             new DbParameterDescriptor("p1", calculationParams.PlaningLevel == DdePlaningLevel.CurrentYear));
            }
            if (queryResult != null && queryResult != DBNull.Value)
                return Convert.ToDecimal(queryResult);
            return 0;
        }

        private decimal GetPlanDebt(DdeCalculationParams calculationParams, DateTime startPeriod, DateTime endPeriod, IDatabase db)
        {
            string currencyQuery =
                @"select Sum(CurrencySum) from t_S_PlanDebtCI where
                RefCreditInc in (select id from f_S_Creditincome where (RefVariant = 0 or RefVariant = ?) and (RefSStatusPlan = 0 or RefSStatusPlan = 5))
                and RefOKV in (select id from d_OKV_Currency where Code = {0}) and (EndDate between ? and ?)";

            string query =
                @"select Sum(sum) from t_S_PlanDebtCI where
                RefCreditInc in (select id from f_S_Creditincome where (RefVariant = 0 or RefVariant = ?) and (RefSStatusPlan = 0 or RefSStatusPlan = 5))
                and RefOKV in (select id from d_OKV_Currency where Code = 643) and (EndDate between ? and ?)";

            object rurResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                                              new DbParameterDescriptor("p1", startPeriod),
                                              new DbParameterDescriptor("p2", endPeriod));

            object usdResult = db.ExecQuery(string.Format(currencyQuery, 840), QueryResultTypes.Scalar,
                new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                                              new DbParameterDescriptor("p1", startPeriod),
                                              new DbParameterDescriptor("p2", endPeriod));

            object eurResult = db.ExecQuery(string.Format(currencyQuery, 978), QueryResultTypes.Scalar,
                new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                                              new DbParameterDescriptor("p1", startPeriod),
                                              new DbParameterDescriptor("p2", endPeriod));
            decimal result = 0;
            if (rurResult != null && rurResult != DBNull.Value)
                result += Convert.ToDecimal(rurResult);

            if (usdResult != null && usdResult != DBNull.Value)
                result += Convert.ToDecimal(usdResult) * calculationParams.UsdRate;

            if (eurResult != null && eurResult != DBNull.Value)
                result += Convert.ToDecimal(eurResult) * calculationParams.EurRate;

            query =
                    @"select Sum(sum) from t_S_CPPlanDebt where RefCap in
                    (select id from f_S_Capital where (RefVariant = 0 or RefVariant = ?) and RefStatusPlan = 1) and (EndDate between ? and ?)";
            rurResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                        new DbParameterDescriptor("p1", startPeriod),
                        new DbParameterDescriptor("p2", endPeriod));
            if (rurResult != null && rurResult != DBNull.Value)
                result += Convert.ToDecimal(rurResult);

            if (oktmo == "78 000 000")
            {
                string queryDiscont =
                    @"select Sum(fact.ChargeSum) from t_S_CPPlanDebt plans, t_S_CPFactService fact where 
                    plans.RefCap = fact.RefCap and
                    plans.RefCap in (select id from f_S_Capital where (RefVariant = 0 or RefVariant = ?) and RefStatusPlan = 1) and
                    (plans.EndDate between ? and ?) and fact.RefTypeSum = 9";

                rurResult = db.ExecQuery(queryDiscont, QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                            new DbParameterDescriptor("p1", startPeriod),
                            new DbParameterDescriptor("p2", endPeriod));
                if (rurResult != null && rurResult != DBNull.Value)
                    result -= Convert.ToDecimal(rurResult);
            }
            return result;
        }

        private decimal GetPlanService(DdeCalculationParams calculationParams, DateTime startPeriod, DateTime endPeriod, IDatabase db)
        {
            DataTable dt = (DataTable)db.ExecQuery(
                "select id from f_S_Creditincome where (RefVariant = 0 or RefVariant = ?) and (RefSStatusPlan = 0 or RefSStatusPlan = 5)",
                QueryResultTypes.DataTable, new DbParameterDescriptor("p0", calculationParams.PlaningVariant));

            decimal result = 0;
            object queryResult = null;
            object usdResult = null;
            object eurResult = null;

            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(db.ExecQuery("select count(distinct EstimtDate) from t_S_PlanServiceCI where RefCreditInc = ?", QueryResultTypes.Scalar,
                    new DbParameterDescriptor("p0", row["ID"]))) > 1 )
                {
                    string query = @"select Sum(sum) from t_S_PlanServiceCI where RefCreditInc = {0}
                        and EstimtDate = (select Max(EstimtDate) from t_S_PlanServiceCI where EstimtDate <= ?)
                        and (EndDate between ? and ?) and 
                        RefOKV in (select id from d_OKV_Currency where Code = 643)";

                    string currencyQuery = @"select Sum(CurrencySum) from t_S_PlanServiceCI where RefCreditInc = {0}
                        and EstimtDate = (select Max(EstimtDate) from t_S_PlanServiceCI where EstimtDate <= ?) 
                        and (EndDate between ? and ?) and 
                        RefOKV in (select id from d_OKV_Currency where Code = {1})";

                    queryResult = db.ExecQuery(string.Format(query, row["ID"]), QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", calculationParams.FormDate),
                         new DbParameterDescriptor("p1", startPeriod),
                         new DbParameterDescriptor("p2", endPeriod));
                    if (queryResult != null && queryResult != DBNull.Value)
                        result += Convert.ToDecimal(queryResult);

                    usdResult = db.ExecQuery(string.Format(currencyQuery, row["ID"], 840), QueryResultTypes.Scalar,
                                                    new DbParameterDescriptor("p0", calculationParams.FormDate),
                                                    new DbParameterDescriptor("p1", startPeriod),
                                                    new DbParameterDescriptor("p2", endPeriod));
                    if (usdResult != null && usdResult != DBNull.Value)
                    {
                        result += Convert.ToDecimal(usdResult) * calculationParams.UsdRate;
                    }

                    eurResult = db.ExecQuery(string.Format(currencyQuery, row["ID"], 978), QueryResultTypes.Scalar,
                                                    new DbParameterDescriptor("p0", calculationParams.FormDate),
                                                    new DbParameterDescriptor("p1", startPeriod),
                                                    new DbParameterDescriptor("p2", endPeriod));
                    if (eurResult != null && eurResult != DBNull.Value)
                    {
                        result += Convert.ToDecimal(eurResult) * calculationParams.EurRate;
                    }
                }
                else
                {
                    string query = @"select Sum(sum) from t_S_PlanServiceCI where RefCreditInc = {0}
                        and (EndDate between ? and ?) and 
                        RefOKV in (select id from d_OKV_Currency where Code = 643)";

                    string currencyQuery = @"select Sum(CurrencySum) from t_S_PlanServiceCI where RefCreditInc = {0}
                        and (EndDate between ? and ?) and 
                        RefOKV in (select id from d_OKV_Currency where Code = {1})";

                    queryResult = db.ExecQuery(string.Format(query, row["ID"]), QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", startPeriod),
                         new DbParameterDescriptor("p1", endPeriod));
                    if (queryResult != null && queryResult != DBNull.Value)
                        result += Convert.ToDecimal(queryResult);

                    usdResult = db.ExecQuery(string.Format(currencyQuery, row["ID"], 840), QueryResultTypes.Scalar,
                                                    new DbParameterDescriptor("p0", startPeriod),
                                                    new DbParameterDescriptor("p1", endPeriod));
                    if (usdResult != null && usdResult != DBNull.Value)
                    {
                        result += Convert.ToDecimal(usdResult) * calculationParams.UsdRate;
                    }

                    eurResult = db.ExecQuery(string.Format(currencyQuery, row["ID"], 978), QueryResultTypes.Scalar,
                                                    new DbParameterDescriptor("p0", startPeriod),
                                                    new DbParameterDescriptor("p1", endPeriod));
                    if (eurResult != null && eurResult != DBNull.Value)
                    {
                        result += Convert.ToDecimal(eurResult) * calculationParams.EurRate;
                    }

                }
            }

            string query1 =
                @"select Sum(sum) from t_S_CPPlanService where RefCap in
                    (select id from f_S_Capital where (RefVariant = 0 or RefVariant = ?) and RefStatusPlan = 1) and (EndDate between ? and ?)";

            queryResult = db.ExecQuery(query1, QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                         new DbParameterDescriptor("p1", startPeriod),
                         new DbParameterDescriptor("p2", endPeriod));
            if (queryResult != null && queryResult != DBNull.Value)
                result += Convert.ToDecimal(queryResult);

            if (oktmo == "78 000 000")
            {
                string queryDiscont =
                    @"select Sum(fact.ChargeSum) from t_S_CPPlanDebt plans, t_S_CPFactService fact where 
                    plans.RefCap = fact.RefCap and
                    plans.RefCap in (select id from f_S_Capital where (RefVariant = 0 or RefVariant = ?) and RefStatusPlan = 1) and
                    (plans.EndDate between ? and ?) and fact.RefTypeSum = 9";

                queryResult = db.ExecQuery(queryDiscont, QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                            new DbParameterDescriptor("p1", startPeriod),
                            new DbParameterDescriptor("p2", endPeriod));
                if (queryResult != null && queryResult != DBNull.Value)
                    result += Convert.ToDecimal(queryResult);
            }
            return result;
        }

        private decimal GetContLiability(DdeCalculationParams calculationParams, DateTime startPeriod, DateTime endPeriod, IDatabase db)
        {
            string query = @"select Sum(sum) from t_S_PlanAttractGrnt where RefGrnt in 
                (select id from f_S_Guarantissued where (RefVariant = 0 or RefVariant = ?) and (RefSStatusPlan = 0 or RefSStatusPlan = 5)) and
                (EndDate between ? and ?) and RefOKV in (select id from d_OKV_Currency where Code = 643)";

            string currencyQuery = @"select Sum(CurrencySum) from t_S_PlanAttractGrnt where RefGrnt in 
                (select id from f_S_Guarantissued where (RefVariant = 0 or RefVariant = ?) and (RefSStatusPlan = 0 or RefSStatusPlan = 5)) and
                (EndDate between ? and ?) and RefOKV in (select id from d_OKV_Currency where Code = {0})";

            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                         new DbParameterDescriptor("p1", startPeriod),
                         new DbParameterDescriptor("p2", endPeriod));
            decimal result = 0;
            if (queryResult != null && queryResult != DBNull.Value)
                result += Convert.ToDecimal(queryResult);

            object usdResult = db.ExecQuery(string.Format(currencyQuery, 840), QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                         new DbParameterDescriptor("p1", startPeriod),
                         new DbParameterDescriptor("p2", endPeriod));
            if (usdResult != null && usdResult != DBNull.Value)
            {
                result += Convert.ToDecimal(usdResult) * calculationParams.UsdRate;
            }

            object eurResult = db.ExecQuery(string.Format(currencyQuery, 978), QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", calculationParams.PlaningVariant),
                         new DbParameterDescriptor("p1", startPeriod),
                         new DbParameterDescriptor("p2", endPeriod));
            if (eurResult != null && eurResult != DBNull.Value)
            {
                result += Convert.ToDecimal(eurResult) * calculationParams.EurRate;
            }

            return result;
        }

        private string GetConstCode(string constName)
        {
            object[] constValues = finSourcesRererencesUtils.GetConstDataByName(constName);
            if (constValues == null)
            {
                //errors.Add(string.Format("Константа с идентификатором '{0}' не найдена в справочнике констант для ИФ", constName));
                return string.Empty;
            }
            return finSourcesRererencesUtils.GetConstDataByName(constName)[0].ToString();
        }
    }
}
