using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.OrgGKH.Services
{
    public static class FactValueService
    {
        public delegate decimal? ParameterGetterDelegat(F_Org_GKH planData);

        public static decimal? GetASValue(
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans, 
            int code, 
            ParameterGetterDelegat getValue, 
            int periodId)
        {
            // Если d коде во втором разряде стоит .00., то должна подтягиваться сумма значений  показателей  
            // у которых код в первом разряде должен начинаться с той же цифры, что и у показателя А, 
            // во втором разряде не равно 00, в третьем разряде должно быть обязательно 00.
            var r1 = code / 10000;
            var r2 = (code / 100) % 100;
            var r3 = code % 100;
            decimal result = 0;

            if (r2 == 0)
            {
                var marks = marksGkh.Where(m =>
                    m.Code / 10000 == r1 &&
                    (m.Code / 100) % 100 != 0 &&
                    m.Code % 100 == 0);
                foreach (var mark in marks)
                {
                    var planData = orgPlans.FirstOrDefault(x =>
                        x.RefMarksGKH.ID == mark.ID &&
                        x.RefYearDayUNV.ID == periodId);
                    if (planData != null)
                    {
                        var res = getValue(planData);
                        if (res != null)
                        {
                            result += (decimal)res;
                        }
                    }
                }
            }
            else
            {
                if (r3 == 0)
                {
                    // Если у него во втором разряде стоит не .00., в третьем .00., 
                    // то должна подтягиваться сумма значений  показателей  
                    // у которых код должен начинаться в первом и втором разряде с теми же значениями, 
                    // что и у показателя А, 
                    // а в третьем разряде должно быть обязательно не равно 00.
                    var marks = marksGkh.Where(m =>
                        m.Code / 10000 == r1 &&
                        (m.Code / 100) % 100 == r2 &&
                        m.Code % 100 != 0);
                    foreach (var mark in marks)
                    {
                        if (orgPlans != null)
                        {
                            var planData = orgPlans.FirstOrDefault(x =>
                                x.RefMarksGKH.ID == mark.ID &&
                                x.RefYearDayUNV.ID == periodId);

                            if (planData != null)
                            {
                                var value = getValue(planData);
                                if (value != null)
                                {
                                    result += (decimal)value;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static decimal? GetADValue(
            int periodId,
            D_Org_MarksGKH mark,
            F_Org_GKH planData,
            ParameterGetterDelegat getAdditionalValue,
            IList<F_Org_GKH> orgPlans)
        {
            var additData = (planData != null && getAdditionalValue(planData) != null)
                ? getAdditionalValue(planData)
                : 0;

            // на январь берем только xOP на тек. месяц
            if (periodId % 10000 < 200)
            {
                return additData;
            }

            // берем значение x на пред. месяц и xOP на тек. месяц
            var plansBefore = orgPlans.Where(x => x.RefYearDayUNV.ID < periodId &&
                                                  x.RefYearDayUNV.ID > (periodId / 10000) * 10000 &&
                                                  x.RefYearDayUNV.ID % 100 == 0 &&
                                                  x.RefMarksGKH.ID == mark.ID);
            var mainData = plansBefore.Sum(plan => getAdditionalValue(plan) ?? 0);

            return mainData + additData;
        }
        
        public static decimal? GetPlanY(
            int periodId, 
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            // Значение по умолчанию выводим только для тех показателей, которые не подсчитываются автоматически, 
            // т.е. у них свойства «План на год (PrPlanY)» у каждого показателя 
            // в классификаторе  «Организации.Показатели ЖКХ(d_Org_MarksGKH)» не равно AS или AD, X.
            var symbolPlanY = mark.PrPlanY;
            if (symbolPlanY.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.Value, periodId);
            }

            if (symbolPlanY.Equals("AD") || symbolPlanY.Equals("X"))
            {
                // подсчитывается автоматически ??
                return null;
            }

            // значение по умолчанию
            return GetDefaultPlanY(periodId, ((periodId / 10000) * 10000) + 1, mark, planData, orgPlans);
        }

        public static decimal? GetPlanO(
            int periodId, 
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrPlanO.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.PlanO, periodId);
            }

            if (mark.PrAssigned.Equals("AD"))
            {
                return GetADValue(periodId, mark, planData, fact => fact.PlanOOP, orgPlans);
            }

            return planData == null ? null : planData.PlanO;
        }

        public static decimal? GetAssigned(
            int periodId, 
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrAssigned.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.Assigned, periodId);
            }

            if (mark.PrAssigned.Equals("AD"))
            {
                return GetADValue(periodId, mark, planData, fact => fact.AssignedOP, orgPlans);
            }

            return planData == null ? null : planData.Assigned;
        }

        public static decimal? GetPlanS(
            int periodId, 
            D_Org_MarksGKH mark,
            F_Org_GKH planData, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrPlanS.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.PlanS, periodId);
            }

            if (mark.PrPlanS.Equals("AD"))
            {
                return GetADValue(periodId, mark, planData, fact => fact.PlanSOP, orgPlans);
            }

            return planData == null ? null : planData.PlanS;
        }

        public static decimal? GetPerformed(
            int periodId, 
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrPerformed.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.Performed, periodId);
            }

            if (mark.PrPerformed.Equals("AD"))
            {
                return GetADValue(periodId, mark, planData, fact => fact.PerformedOP, orgPlans);
            }

            return planData == null ? null : planData.Performed;
        }

        public static decimal? GetPlanOOP(
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            int periodId, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrPlanOOP.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.PlanOOP, periodId);
            }

            return planData == null ? null : planData.PlanOOP;
        }

        public static decimal? GetAssignedOP(
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            int periodId, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrAssignedOP.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.AssignedOP, periodId);
            }

            return planData == null ? null : planData.AssignedOP;
        }

        public static decimal? GetPerformedOP(
            int periodId, 
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            IList<D_Org_MarksGKH> marksGkh,
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrPerformedOP.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.PerformedOP, periodId);
            }

            if (mark.PrPerformedOP.Equals("W"))
            {
                var ym = periodId / 100;

                // у которых месяц равен выбранному в параметре периоде
                var plans = orgPlans.Where(
                    x =>
                    x.RefYearDayUNV.ID / 100 == ym &&
                    x.RefYearDayUNV.ID % 100 != 0 &&
                    x.RefMarksGKH.ID == mark.ID);

                return (from plan in plans
                        where plan != null
                        select plan.PerformedOP
                            into res
                            where res != null
                            select (decimal)res)
                        .Sum();
            }

            return planData == null ? null : planData.PerformedOP;
        }

        public static decimal? GetPlanSOP(
            int periodId, 
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            IList<D_Org_MarksGKH> marksGkh, 
            IList<F_Org_GKH> orgPlans)
        {
            if (mark.PrPlanSOP.Equals("AS"))
            {
                return GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.PlanSOP, periodId);
            }

            if (mark.PrPlanSOP.Equals("W"))
            {
                var ym = periodId / 100;

                // у которых месяц равен выбранному в параметре периоде
                var plans = orgPlans.Where(
                    x =>
                    x.RefYearDayUNV.ID / 100 == ym &&
                    x.RefMarksGKH.ID == mark.ID);

                return (from plan in plans
                        where plan != null
                        select plan.PlanSOP
                            into res
                            where res != null
                            select (decimal)res)
                        .Sum();
            }

            return planData == null ? null : planData.PlanSOP;
        }

        /// <summary>
        /// Если значение пусто или 0, то выводим значение периода, выбранного в параметре -1, 
        /// т.е. на предыдущий месяц. 
        /// Но если выбран период – январь и пусто, то ничего не выводим
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="periodYear">Идентификатор периода годового</param>
        /// <param name="mark">Объект - показатель</param>
        /// <param name="planData">Объект - данные на заданный период</param>
        /// <param name="orgPlans">список планов для текущей организации, на выбранный период и источник</param>
        /// <returns>Значение "План на год" по умолчанию</returns>
        private static decimal? GetDefaultPlanY(
            int periodId, 
            int periodYear, 
            D_Org_MarksGKH mark, 
            F_Org_GKH planData, 
            IEnumerable<F_Org_GKH> orgPlans)
        {
            if ((planData == null || planData.Value == null) && (periodId % 10000 > 199))
            {
                var prevPlans = orgPlans.Where(x =>
                                               x.Value != null &&
                                               x.RefMarksGKH.ID == mark.ID &&
                                               x.RefYearDayUNV.ID < periodId &&
                                               x.RefYearDayUNV.ID > periodYear &&
                                               x.RefYearDayUNV.ID % 100 == 0)
                    .OrderBy(x => x.RefYearDayUNV.ID);
                var prevPlanData = prevPlans.LastOrDefault();

                return prevPlanData != null ? prevPlanData.Value : null;
            }

            return planData == null ? null : planData.Value;
        }
    }
}
