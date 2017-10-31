using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.Redemption
{
    class RedeptionService
    {
        private IScheme Scheme
        {
            get; set;
        }

        public RedeptionService(IScheme scheme)
        {
            Scheme = scheme;
        }

        /// <summary>
        /// расчет значений 
        /// </summary>
        /// <param name="redemptionDate"></param>
        /// <param name="capitalId"></param>
        /// <param name="value"></param>
        /// <param name="calculatedValue"></param>
        /// <returns></returns>
        public decimal Calculate(DateTime redemptionDate, object capitalId, decimal value, CalculatedValue calculatedValue)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                DataTable dtPlanCervice = (DataTable)db.ExecQuery(
                    "select EndDate, Income from t_S_CPPlanService where RefCap = ? and EndDate > ?", QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", capitalId),
                    new DbParameterDescriptor("p1", redemptionDate));

                DataTable dtPlanDebt = (DataTable) db.ExecQuery(
                    "select PercentNom, EndDate from t_S_CPPlanDebt where RefCap = ? and EndDate > ?", QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", capitalId),
                    new DbParameterDescriptor("p1", redemptionDate));

                switch (calculatedValue)
                {
                    case CalculatedValue.YTM:
                        return CalculateYTM(dtPlanCervice, dtPlanDebt, redemptionDate, value);
                    case CalculatedValue.CurrPrice:
                        return CalculateCurrentRurCost(dtPlanCervice, dtPlanDebt, redemptionDate, value);
                }
            }
            return 0;
        }

        private decimal CalculateCurrentRurCost(DataTable dtPlanCervice, DataTable dtPlanDebt, DateTime redemptionDate, decimal ytm)
        {
            decimal rurCost = 0;
            int t = 0;
            foreach (DataRow row in dtPlanCervice.Rows)
            {
                DateTime endDate = Convert.ToDateTime(row[0]);
                decimal c = Convert.ToDecimal(row[1]);
                t = (endDate - redemptionDate).Days;
                double x1 = Convert.ToDouble(1 + (ytm / 100));
                decimal dc = Decimal.Divide(t, 365);
                double x2 = Convert.ToDouble(dc);
                double denominator = Math.Exp(x2 * Math.Log(x1));
                rurCost += c / Convert.ToDecimal(denominator);
            }

            foreach (DataRow row in dtPlanDebt.Rows)
            {
                decimal n = Convert.ToDecimal(row[0]) * 10;
                DateTime endDate = Convert.ToDateTime(row[1]);
                t = (endDate - redemptionDate).Days;
                double x1 = Convert.ToDouble(1 + (ytm / 100));
                decimal dc = Decimal.Divide(t, 365);
                double x2 = Convert.ToDouble(dc);
                double denominator = Math.Exp(x2 * Math.Log(x1));
                rurCost += (n /
                    Convert.ToDecimal(denominator));
            }

            return rurCost;
        }

        private decimal CalculateYTM(DataTable dtPlanCervice, DataTable dtPlanDebt, DateTime redemptionDate, decimal pRur)
        {
            if (dtPlanCervice.Rows.Count == 0 || dtPlanDebt.Rows.Count == 0)
                return 0;

            decimal interval = 100;
            decimal Y = 0;
            decimal P = pRur;
            decimal pY = CalculateCurrentRurCost(dtPlanCervice, dtPlanDebt, redemptionDate, Y);
            decimal dif = P - pY;
            decimal sigma = new decimal(0.0001);
            while (Math.Abs(dif) > sigma)
            {
                if (dif > 0)
                {
                    interval = interval / 2;
                    Y = Y - interval;
                }
                else
                {
                    Y = Y + interval;
                }
                pY = CalculateCurrentRurCost(dtPlanCervice, dtPlanDebt, redemptionDate, Y);
                dif = P - pY;
            }
            return Y;
        }

        public bool CheckDetails(long capitalId, string capitalIdentent, ref string message)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                var sb = new StringBuilder();
                object rowsCount = db.ExecQuery("select count(id) from t_S_CPJournalPercent where RefCap = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", capitalId));
                if (Convert.ToInt32(rowsCount) == 0)
                {
                    sb.AppendLine(string.Format("Для облигационного займа {0} не заполнена детализация 'Журнал ставок процентов'",
                        capitalIdentent));
                }

                rowsCount = db.ExecQuery("select count(id) from t_S_CPPlanService where RefCap = ?",
                    QueryResultTypes.Scalar, new DbParameterDescriptor("p0", capitalId));
                if (Convert.ToInt32(rowsCount) == 0)
                {
                    sb.AppendLine(string.Format("Для облигационного займа {0} не заполнена детализация 'План выплаты дохода'",
                        capitalIdentent));
                }

                rowsCount = db.ExecQuery("select count(id) from t_S_CPPlanDebt where RefCap = ?",
                    QueryResultTypes.Scalar, new DbParameterDescriptor("p0", capitalId));
                if (Convert.ToInt32(rowsCount) == 0)
                {
                    sb.AppendLine(string.Format("Для облигационного займа {0} не заполнена детализация 'План погашения номинальной стоимости'",
                        capitalIdentent));
                }
                message = sb.ToString();
                return string.IsNullOrEmpty(message);
            }
        }
    }
}
