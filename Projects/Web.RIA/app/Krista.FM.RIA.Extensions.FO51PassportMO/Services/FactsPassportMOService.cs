using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public class FactsPassportMOService : IFactsPassportMOService
    {
        private readonly ILinqRepository<F_Marks_PassportMO> factsRepository;
        private readonly IRepository<D_PMO_Variant> variantRepository;

        public FactsPassportMOService(
            ILinqRepository<F_Marks_PassportMO> factsRepository, 
            IRepository<D_PMO_Variant> variantRepository)
        {
            this.factsRepository = factsRepository;
            this.variantRepository = variantRepository;
        }

        /// <summary>
        /// Возвращает список показателей для вкладки формы сбора.
        /// </summary>
        /// <param name="markId">Идентификатор показателя вкладки.</param>
        public static List<MarkPassportMO> GetMarks(int markId)
        {
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                throw new NotImplementedException();
            }

            var queryString = new StringBuilder(@"
with Marks as (
    select p.*, 0 as mLevel 
        from D_Marks_PassportMO p where p.ParentID = :ParamMarkId 
    union all
    select c.*, mLevel + 1
        from D_Marks_PassportMO as c, Marks 
        where Marks.ID = c.ParentID
)
select
m.ID as ID,
m.ID as MarkId,
m.mLevel as Level,
case when exists (select null from D_Marks_PassportMO child where child.ParentID = m.ID)
            then 0
            else 1
        end as IsLeaf
from Marks as m
".FormatWith(markId));
            var data = NHibernateSession.Current.CreateSQLQuery(queryString.ToString())
                .AddEntity(typeof(MarkPassportMO))
                .SetInt32("ParamMarkId", markId);

            var list = data.List<MarkPassportMO>();

            return (List<MarkPassportMO>)list;
        }

        /// <summary>
        /// Возвращает данные для формы сбора (отдельной вкладки).
        /// </summary>
        /// <param name="markId">Идентификатор показателя вкладки.</param>
        /// <param name="periodId">Идентификатор периода.</param>
        /// <param name="regionId">Идентификатор региона (или константу (тип территории) для "МР", "ГО" или "МР и ГО".</param>
        /// <param name="sourceId">Идентификатор источника.</param>
        /// <param name="isFictRegion">Является ли фиктивным районом (неуказанное мо).</param>
        public List<MarksPassportMOFact> GetPassportMOFacts(
            int markId,
            int periodId,
            int regionId,
            int sourceId,
            bool isFictRegion)
        {
            var data = GetFacts(markId, regionId, sourceId, periodId, isFictRegion);
            var resultFactsOrdered = data.OrderBy(x => x.MarkCode).ToList();

            /* Для показателя "2. Содержание органов местного самоуправления (раздел. 01-12)" с кодом 2.1.1.999
             * уровень в дереве меняем на 2*/
            var mark211999 = resultFactsOrdered.FirstOrDefault(x => x.MarkCode.Equals("2.1.1.999"));
            if (mark211999 != null)
            {
                mark211999.Level = 2;
            }

            return resultFactsOrdered;
        }

        /// <summary>
        /// Возвращает идентификатор состояния данных по всей форме.
        /// </summary>
        /// <param name="periodId">Идентификатор периода.</param>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <param name="sourceId">Идентификатор источника.</param>
        public int GetStateId(int periodId, int regionId, int sourceId)
        {
            var month = (periodId / 100) % 100;
            var year = periodId / 10000;

            var yearPeriodId = (year * 10000) + 1;
            var prevYearPeriodId = ((year - 1) * 10000) + 1;
            var fact = month == 1
                    ? factsRepository.FindAll()
                    .FirstOrDefault(x =>
                        x.RefPasRegions.ID == regionId &&
                        x.SourceID == sourceId &&
                        ((x.RefPasPeriod.ID == periodId && x.RefVar.Code == -1) ||
                         (x.RefPasPeriod.ID / 10000 == year && x.RefVar.Code == month) ||
                         (x.RefPasPeriod.ID == yearPeriodId && x.RefVar.Code == -1) ||
                         (x.RefPasPeriod.ID == prevYearPeriodId && x.RefVar.Code == -1)))
                   : factsRepository.FindAll()
                    .FirstOrDefault(x =>
                        x.RefPasRegions.ID == regionId &&
                        x.SourceID == sourceId &&
                        ((x.RefPasPeriod.ID == periodId && x.RefVar.Code == -1) ||
                         (x.RefPasPeriod.ID / 10000 == year && x.RefVar.Code == month)));

            return fact != null ? fact.RefStatePasMO.ID : 1;
        }

        /// <summary>
        /// Возвращает факты Фин.паспортаМО по набору параметров.
        /// </summary>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <param name="sourceId">Идентификатор источника.</param>
        /// <param name="markId">Идентификатор показателя.</param>
        /// <param name="periodId">Идентификатор периода.</param>
        /// <param name="variantCode">Код варианта.</param>
        public IEnumerable<F_Marks_PassportMO> FindFacts(
            int regionId,
            int sourceId,
            int markId,
            int periodId,
            int variantCode)
        {
            switch (regionId)
            {
                case FO51Extension.RegionsMR:
                case FO51Extension.RegionsGO:
                    return factsRepository.FindAll().Where(x =>
                                                           x.RefPasRegions.RefTerr.ID == -regionId &&
                                                           x.SourceID == sourceId &&
                                                           x.RefPassportMO.ID == markId &&
                                                           x.RefPasPeriod.ID == periodId &&
                                                           x.RefVar.Code == variantCode);
                case FO51Extension.RegionsAll:
                    return factsRepository.FindAll().Where(x =>
                                                           (x.RefPasRegions.RefTerr.ID == -FO51Extension.RegionsMR ||
                                                                x.RefPasRegions.RefTerr.ID == -FO51Extension.RegionsGO ||
                                                                (x.RefPasRegions.RefBridgeRegions != null &&
                                                                x.RefPasRegions.RefBridgeRegions.ID ==
                                                                FO51Extension.RegionFictID)) &&
                                                           x.SourceID == sourceId &&
                                                           x.RefPassportMO.ID == markId &&
                                                           x.RefPasPeriod.ID == periodId &&
                                                           x.RefVar.Code == variantCode);
                default:
                    return factsRepository.FindAll().Where(x =>
                                                    x.RefPasRegions.ID == regionId &&
                                                    x.SourceID == sourceId &&
                                                    x.RefPassportMO.ID == markId &&
                                                    x.RefPasPeriod.ID == periodId &&
                                                    x.RefVar.Code == variantCode);
            }
        }

        /// <summary>
        /// Возвращает первый факт Фин.паспортаМО по набору параметров.
        /// </summary>
        /// <param name="regionId">Идентификатор региона.</param>
        /// <param name="sourceId">Идентификатор источника.</param>
        /// <param name="markId">Идентификатор показателя.</param>
        /// <param name="periodId">Идентификатор периода.</param>
        /// <param name="variantCode">Код варианта.</param>
        public F_Marks_PassportMO FindFirstOrDefault(
            int regionId,
            int sourceId,
            int markId,
            int periodId,
            int variantCode)
        {
            return FindFacts(regionId, sourceId, markId, periodId, variantCode).FirstOrDefault();
        }

        public void SaveStateForFacts(int periodId, int stateId, int regionId, int sourceId)
        {
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                throw new NotImplementedException();
            }

            var periodMonth = (periodId / 100) % 100;
            var year = periodId / 10000;
            var yearPeriodId = (year * 10000) + 1;
            var prevYearPeriodId = ((year - 1) * 10000) + 1;
            var variantMinusOne = variantRepository.GetAll().FirstOrDefault(x => x.Code == -1);
            if (variantMinusOne == null)
            {
                throw new Exception("Вариант 'Неизвестные данные' отсутствует.");
            }

            var queryString =
                new StringBuilder(
                    @"
            UPDATE f_Marks_PassportMO 
SET RefStatePasMO = :newState
WHERE  
RefPasRegions = :regionId and
SourceID = :sourceId and 
((RefPasPeriod = :periodId and RefVar = :minusOneID) or
 (RefPasPeriod / 10000 = :periodsyear and RefVar = :monthId) ");
            if (periodMonth == 1)
            {
                queryString.Append(
                    @" or
(RefPasPeriod = :yearPeriodId and RefVar = :minusOneID) or
 (RefPasPeriod = :prevYearPeriodId and RefVar = :minusOneID)");
            }

            queryString.Append(")");

            var data = NHibernateSession.Current.CreateSQLQuery(queryString.ToString())
                .SetInt32("newState", stateId)
                .SetInt32("regionId", regionId)
                .SetInt32("sourceId", sourceId)
                .SetInt32("periodId", periodId)
                .SetInt32("minusOneID", variantMinusOne.ID)
                .SetInt32("periodsyear", year)
                .SetInt32("monthId", periodMonth);

            if (periodMonth == 1)
            {
                data.SetInt32("yearPeriodId", yearPeriodId)
                    .SetInt32("prevYearPeriodId", prevYearPeriodId);
            }

            data.ExecuteUpdate();
        }

        /// <summary>
        /// Возвращает текст SQL-запроса - выбор фактов по всем колонкам формы сбора в зависимости от территории.
        /// </summary>
        /// <param name="regionId">Идентификатор региона.</param>
        private string GetAllColumnsFactsSQL(int regionId)
        {
            string regionCondition;
            switch (regionId)
            {
                case FO51Extension.RegionsMR:
                case FO51Extension.RegionsGO:
                    regionCondition = "regions.RefTerr = -:RegionId";
                    return GetSumColumnsFactsSQL().FormatWith(regionCondition);
                case FO51Extension.RegionsAll:
                    regionCondition = "(regions.RefTerr = 4 or regions.RefTerr = 7 or regions.RefBridgeRegions = {0})".FormatWith(FO51Extension.RegionFictID);
                    return GetSumColumnsFactsSQL().FormatWith(regionCondition);
                default:
                    return GetColumnsFactsSQL();
            }
        }

        /// <summary>
        /// Возвращает текст SQL-запроса - выбор фактов для колонки.
        /// </summary>
        /// <param name="columnName">Имя поля т.ф.</param>
        /// <param name="periodParamName">Наименование параметра периода.</param>
        private string GetColumnFactsSQL(string columnName, string periodParamName)
        {
            return
                @"
{0}Facts (Val, RefMark) as (
    select SUM(isnull({0},0)), RefPassportMO from F_Marks_PassportMO
    where RefPasRegions = :RegionId 
        and SourceID = :SourceId
        and RefPasPeriod = :{1}
        and RefVar = -1
    group by RefPassportMO
)".FormatWith(columnName, periodParamName);
        }

        /// <summary>
        /// Возвращает текст SQL-запроса - выбор фактов по всем колонкам формы сбора.
        /// </summary>
        private string GetColumnsFactsSQL()
        {
            var sb = new StringBuilder()
                .Append(GetColumnFactsSQL("OrigPlan", "YearPeriodId"))
                .Append(",")
                .Append(GetColumnFactsSQL("FactPeriod", "PeriodId"))
                .Append(",")
                .Append(GetColumnFactsSQL("FactLastYear", "PrevYearPeriodId"))
                .Append(",")
                .Append(GetColumnFactsSQL("PlanPeriod", "PeriodId"))
                .Append(",")
                .Append(GetColumnFactsSQL("RefinPlan", "PrevYearPeriodId"));
                
            for (var month = 1; month <= 12; month++)
            {
                sb.Append(",")
                    .Append(GetScoreMOSQL(month));
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает текст SQL-запроса - выбор фактов для колонки, данные суммируются (по всем МР, ГО или МР и ГО).
        /// </summary>
        /// <param name="columnName">Имя поля т.ф.</param>
        /// <param name="periodParamName">Наименование параметра периода.</param>
        private string GetColumnSumFactsSQL(string columnName, string periodParamName)
        {
            return
                @"
{0}Facts (Val, RefMark) as (
 select sum(isnull(facts.{0}, 0)), facts.RefPassportMO
 from F_Marks_PassportMO as facts
    inner join D_Regions_Analysis regions on regions.ID = facts.RefPasRegions
    where
        {{0}} and
        facts.SourceID = :SourceId
        and facts.RefPasPeriod = :{1}
        and facts.RefVar = -1
   group by facts.RefPassportMO
)".FormatWith(columnName, periodParamName);
        }

        /// <summary>
        /// Возвращает текст SQL-запроса - выбор фактов по всем колонкам формы сбора, данные суммируются (по всем МР, ГО или МР и ГО).
        /// </summary>
        private string GetSumColumnsFactsSQL()
        {
            var sb = new StringBuilder()
                .Append(GetColumnSumFactsSQL("OrigPlan", "YearPeriodId"))
                .Append(",")
                .Append(GetColumnSumFactsSQL("FactPeriod", "PeriodId"))
                .Append(",")
                .Append(GetColumnSumFactsSQL("FactLastYear", "PrevYearPeriodId"))
                .Append(",")
                .Append(GetColumnSumFactsSQL("PlanPeriod", "PeriodId"))
                .Append(",")
                .Append(GetColumnSumFactsSQL("RefinPlan", "PrevYearPeriodId"));
            for (var month = 1; month <= 12; month++)
            {
                sb.Append(",")
                    .Append(GetScoreMOSumSQL(month));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает список записей для формы сбора.
        /// </summary>
        /// <param name="markId">Идентификатор показателя.</param>
        /// <param name="regionId">Идентификатор района.</param>
        /// <param name="sourceId">Идентификатор источника.</param>
        /// <param name="periodId">Идентификатор периода.</param>
        /// <param name="isFictRegion">Является ли регион фиктивным (неуказанное мо).</param>
        private List<MarksPassportMOFact> GetFacts(int markId, int regionId, int sourceId, int periodId, bool isFictRegion)
        {
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                throw new NotImplementedException();
            }

            var queryString = new StringBuilder(@"
with Marks as (
    select p.*, 0 as mLevel 
        from D_Marks_PassportMO p where p.ParentID = :MarkId 
    union all
    select c.*, mLevel + 1 
        from D_Marks_PassportMO as c, Marks 
        where Marks.id = c.ParentID
), ")
    .Append(GetAllColumnsFactsSQL(regionId))
    .Append(
@"
select 
    m.ID as ID, 
    m.Name as MarkName, 
    m.Code as MarkCode, 
    f1.Val as OrigPlan,
    f101.Val as ScoreMO1,
    f102.Val as ScoreMO2,
    f103.Val as ScoreMO3,
    f104.Val as ScoreMO4,
    f105.Val as ScoreMO5,
    f106.Val as ScoreMO6,
    f107.Val as ScoreMO7,
    f108.Val as ScoreMO8,
    f109.Val as ScoreMO9,
    f110.Val as ScoreMO10,
    f111.Val as ScoreMO11,
    f112.Val as ScoreMO12,
    0 as ScoreMOQuarter1,
    0 as ScoreMOQuarter2,
    0 as ScoreMOQuarter3,
    0 as ScoreMOQuarter4,
    f2.Val as FactPeriod, 
    f3.Val as FactLastYear, 
    f4.Val as PlanPeriod, 
    f5.Val as RefinPlan, 
    m.mLevel as Level,
    case when exists (select null from D_Marks_PassportMO child where child.ParentID = m.ID)
            then 0
            else 1
        end as IsLeaf,
    1 as State,
    case when (m.FactPeriod = 1 and :IsFictRegion = 0) then 0 else 1 end as Editable3456
from Marks m 
    left outer join OrigPlanFacts f1 on (f1.RefMark = m.ID)
    left outer join FactPeriodFacts f2 on (f2.RefMark = m.ID)
    left outer join FactLastYearFacts f3 on (f3.RefMark = m.ID)
    left outer join PlanPeriodFacts f4 on (f4.RefMark = m.ID)
    left outer join RefinPlanFacts f5 on (f5.RefMark = m.ID)
    left outer join ScoreMO1Facts f101 on (f101.RefMark = m.ID)
    left outer join ScoreMO2Facts f102 on (f102.RefMark = m.ID)
    left outer join ScoreMO3Facts f103 on (f103.RefMark = m.ID)
    left outer join ScoreMO4Facts f104 on (f104.RefMark = m.ID)
    left outer join ScoreMO5Facts f105 on (f105.RefMark = m.ID)
    left outer join ScoreMO6Facts f106 on (f106.RefMark = m.ID)
    left outer join ScoreMO7Facts f107 on (f107.RefMark = m.ID)
    left outer join ScoreMO8Facts f108 on (f108.RefMark = m.ID)
    left outer join ScoreMO9Facts f109 on (f109.RefMark = m.ID)
    left outer join ScoreMO10Facts f110 on (f110.RefMark = m.ID)
    left outer join ScoreMO11Facts f111 on (f111.RefMark = m.ID)
    left outer join ScoreMO12Facts f112 on (f112.RefMark = m.ID)
");

            var periodMonth = (periodId / 100) % 100;
            var year = periodId / 10000;
            var yearPeriodId = (year * 10000) + 1;
            var prevYearPeriodId = ((year - 1) * 10000) + 1;
            var data = NHibernateSession.Current.CreateSQLQuery(queryString.ToString())
                .AddEntity(typeof(MarksPassportMOFact))
                .SetInt32("MarkId", markId)
                .SetInt32("IsFictRegion", isFictRegion ? 1 : 0)
                .SetInt32("SourceId", sourceId)
                .SetInt32("YearPeriodId", yearPeriodId)
                .SetInt32("PeriodId", periodId)
                .SetInt32("PrevYearPeriodId", prevYearPeriodId);

            if (regionId != FO51Extension.RegionsAll)
            {
                data.SetInt32("RegionId", regionId);
            }

            for (var month = 1; month <= 12; month++)
            {
                data.SetInt32("ScoreMO{0}PeriodId".FormatWith(month), (year * 10000) + (month * 100))
                .SetInt32("ScoreMO{0}VariantId".FormatWith(month), periodMonth);
            }

            var list = data.List<MarksPassportMOFact>();

            return (List<MarksPassportMOFact>)list;
        }

        /// <summary>
        /// Возвращает текст SQL-запроса - выбор фактов по мере ScoreMO (оценка исполнения по месяцу).
        /// </summary>
        /// <param name="month">Месяц, на который берется оценка.</param>
        private string GetScoreMOSQL(int month)
        {
            return @"
ScoreMO{0}Facts (Val, RefMark) as (
    select sum(isnull(ScoreMO,0)), RefPassportMO from F_Marks_PassportMO
    where RefPasRegions = :RegionId
        and SourceID = :SourceId
        and RefPasPeriod = :ScoreMO{0}PeriodId
        and RefPasMeans = 0
        and RefVar = :ScoreMO{0}VariantId
    group by RefPassportMO
)".FormatWith(month);
        }

        /// <summary>
        /// Возвращает текст SQL-запроса - выбор фактов по мере ScoreMO (оценка исполнения по месяцу),
        /// данные суммируются (по всем МР, ГО или МР и ГО).
        /// </summary>
        /// <param name="month">Месяц, на который берется оценка.</param>
        private string GetScoreMOSumSQL(int month)
        {
            return @"
ScoreMO{0}Facts (Val, RefMark) as (
    select sum(isnull(ScoreMO,0)), RefPassportMO from F_Marks_PassportMO as f
    inner join D_Regions_Analysis regions on regions.ID = f.RefPasRegions
    where {{0}}
        and f.SourceID = :SourceId
        and f.RefPasPeriod = :ScoreMO{0}PeriodId
        and f.RefVar = :ScoreMO{0}VariantId
        and RefPasMeans = 0
    group by f.RefPassportMO
)".FormatWith(month);
        }
    }
}
