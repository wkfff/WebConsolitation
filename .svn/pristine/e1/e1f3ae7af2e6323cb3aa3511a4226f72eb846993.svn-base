using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Progress;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public class FactsPassportMOSaveService : IFactsPassportMOSaveService
    {
        private readonly IRepository<FX_BdgtLevels_SKIF> bdgtRepository;
        private readonly IFactsByMesOtchService factsByMesOtchService;
        private readonly IFO51Extension extension; 
        private readonly ILinqRepository<F_Marks_PassportMO> factsPassportRepository; 
        private readonly ILinqRepository<D_Marks_PassportMO> marksPassportRepository;
        private readonly IRepository<FX_MeansType_SKIF> meansTypeRepository; 
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly IRepository<D_Regions_Analysis> regionRepository;
        private readonly IRepository<FX_State_PassportMO> stateRepository; 
        private readonly IRepository<D_PMO_Variant> variantRepository;

        private readonly IProgressManager progressManager;
        
        private readonly string MarkIncome = "Доходы";
        private readonly string MarkCosts = "Расходы";
        private readonly string MarkCredit = "Кредиторская задолженность";
        private readonly string MarkSources = "Источники финансирования дефицита бюджета";
        private readonly string MarkReference = "Справочно";
        private int dataCreatedState;

        public FactsPassportMOSaveService(
            IFO51Extension extension,
            IProgressManager progressManager,
            ILinqRepository<D_Marks_PassportMO> marksPassportRepository,
            ILinqRepository<F_Marks_PassportMO> factsPassportRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository,
            IRepository<D_PMO_Variant> variantRepository,
            IRepository<D_Regions_Analysis> regionRepository,
            IRepository<FX_State_PassportMO> stateRepository,
            IRepository<FX_MeansType_SKIF> meansTypeRepository,
            IRepository<FX_BdgtLevels_SKIF> bdgtRepository,
            IFactsByMesOtchService factsByMesOtchService)
        {
            this.extension = extension;
            this.marksPassportRepository = marksPassportRepository;
            this.factsPassportRepository = factsPassportRepository;
            this.periodRepository = periodRepository;
            this.variantRepository = variantRepository;
            this.regionRepository = regionRepository;
            this.stateRepository = stateRepository;
            this.meansTypeRepository = meansTypeRepository;
            this.bdgtRepository = bdgtRepository;
            this.factsByMesOtchService = factsByMesOtchService;
            dataCreatedState = extension.UserGroup.Equals(FO51Extension.GroupMo) ? FO51Extension.StateEdit : FO51Extension.StateConsider;
            this.progressManager = progressManager;
        }

        private delegate void ParameterSetterDelegat(decimal? result, F_Marks_PassportMO fact);

        private delegate decimal? ParameterGetterDelegat(F_Marks_PassportMO fact);

        public void SaveUpdatedRecordsAndCalc(
            List<Dictionary<string, object>> updatedRecords, 
            D_Regions_Analysis region, 
            DataSources source, 
            int periodId, 
            int year, 
            int month, 
            int periodForYear, 
            int periodLastYear, 
            D_Marks_PassportMO parentMark)
        {
            var parentMarkName = parentMark.Name;

            // факты по показателю для района
            var periodForNextYear = periodForYear + 10000;
            var facts = factsPassportRepository.FindAll()
                .Where(x => x.RefPasRegions.ID == region.ID && x.RefPasPeriod.ID >= periodLastYear && x.RefPasPeriod.ID < periodForNextYear)
                .ToList();

            FX_BdgtLevels_SKIF defaultBdgt;
            if (extension.User.RefRegion != null)
            {
                var curRegion = regionRepository.Get(extension.User.RefRegion.Value);
                defaultBdgt = bdgtRepository.Get(curRegion.RefTerr.ID == 7 ? 4 : 5);
            }
            else
            {
                defaultBdgt = bdgtRepository.Get(4);
            }

            var defaultMeansType = meansTypeRepository.Get(0);
            var defaultMeansType1 = meansTypeRepository.Get(1);
            var variantsAll = variantRepository.GetAll().ToList();

            // Сохраняем измененные данные.
            SaveChangedRecords(updatedRecords, region, source, periodId, periodForYear, periodLastYear, month, parentMarkName, defaultMeansType, defaultBdgt, defaultMeansType1, facts, variantsAll);

            var marksAll = marksPassportRepository.FindAll()
                .Where(x => x.SourceID == source.ID && x.RefTypeMark.ID == 1).ToList();

            /*// Подтягиваем данные из месячной отчетности.
            if (parentMarkName.Equals(MarkCosts))
            {
                var period = periodRepository.Get(periodId);
                factsByMesOtchService.SaveMonthReportData(region, period, facts, marksAll, source.ID, defaultMeansType, defaultBdgt);
            }*/

            // Пересчитываем поля, которые подсчитываются автоматически.
            CalcFactForMark(parentMark, year, month, parentMarkName, region, facts, variantsAll, source.ID, defaultMeansType, defaultBdgt, marksAll);
        }

        public void SaveMesOtchReCalc(
            D_Regions_Analysis region,
            DataSources source,
            int periodId,
            int year,
            int month,
            int periodForYear,
            int periodLastYear, 
            D_Marks_PassportMO parentMark)
        {
            var parentMarkName = parentMark.Name;

            // факты по показателю для района
            var periodForNextYear = periodForYear + 10000;
            var facts = factsPassportRepository.FindAll().Where(x => 
                    x.SourceID == source.ID &&
                    x.RefPasRegions.ID == region.ID && 
                    (x.RefVar.ID == -1 || x.RefVar.ID == month) &&
                    x.RefPasPeriod.ID >= periodLastYear && 
                    x.RefPasPeriod.ID < periodForNextYear)
                .ToList();

            FX_BdgtLevels_SKIF defaultBdgt;
            if (extension.User.RefRegion != null)
            {
                var curRegion = regionRepository.Get(extension.User.RefRegion.Value);
                defaultBdgt = bdgtRepository.Get(curRegion.RefTerr.ID == 7 ? 4 : 5);
            }
            else
            {
                defaultBdgt = bdgtRepository.Get(4);
            }

            var defaultMeansType = meansTypeRepository.Get(0);
            var variantsAll = variantRepository.GetAll().ToList();

            var marksAll = marksPassportRepository.FindAll()
                .Where(x => x.SourceID == source.ID && x.RefTypeMark.ID == 1).ToList();

            // Подтягиваем данные из месячной отчетности.
            progressManager.SetCompleted("Получение данных месячной отчетности...", 0.3);
            if (parentMarkName.Equals(MarkCosts))
            {
                var period = periodRepository.Get(periodId);
                factsByMesOtchService.SaveMonthReportData(region, period, facts, marksAll, source.ID, defaultMeansType, defaultBdgt);
            }

            // пересчитываем поля, которые подсчитываются автоматически.
            progressManager.SetCompleted("Пересчет вычислимых значений...", 0.7);
            CalcFactForMark(parentMark, year, month, parentMarkName, region, facts, variantsAll, source.ID, defaultMeansType, defaultBdgt, marksAll);
        }

        /// <summary>
        /// Сохранение измененных записей
        /// </summary>
        private void SaveChangedRecords(
            IEnumerable<Dictionary<string, object>> updatedRecords,
            D_Regions_Analysis region,
            DataSources source,
            int periodId,
            int periodForYear,
            int periodLastYear,
            int month,
            string parentMarkName,
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt,
            FX_MeansType_SKIF defaultMeansType1,
            ICollection<F_Marks_PassportMO> facts,
            IEnumerable<D_PMO_Variant> variantsAll)
        {
            foreach (var record in updatedRecords)
            {
                // если факт по показателю существовал - изменяем его, иначе - создаем новый
                var markIdObj = record["ID"];
                if (markIdObj != null && Convert.ToInt32(markIdObj) > 0)
                {
                    var markId = Convert.ToInt32(markIdObj);
                    var mark = marksPassportRepository.FindOne(markId);
                    var isLeafObj = record["IsLeaf"];
                    if (isLeafObj != null)
                    {
                        var isLeaf = Convert.ToBoolean(isLeafObj);
                        if (isLeaf || mark.Code.Equals("3.8.2"))
                        {
                            if (parentMarkName.Equals(MarkIncome) ||
                                parentMarkName.Equals(MarkSources) ||
                                parentMarkName.Equals(MarkCosts))
                            {
                                if (month == 1)
                                {
                                    SaveParam(
                                        periodForYear,
                                        record,
                                        facts,
                                        mark,
                                        "OrigPlan",
                                        (value, fact) => fact.OrigPlan = value,
                                        region,
                                        defaultMeansType,
                                        source.ID,
                                        defaultMeansType,
                                        defaultBdgt);
                                    SaveParam(
                                        periodLastYear,
                                        record,
                                        facts,
                                        mark,
                                        "FactLastYear",
                                        (value, fact) => fact.FactLastYear = value,
                                        region,
                                        defaultMeansType1,
                                        source.ID,
                                        defaultMeansType,
                                        defaultBdgt);
                                    SaveParam(
                                        periodLastYear,
                                        record,
                                        facts,
                                        mark,
                                        "RefinPlan",
                                        (value, fact) => fact.RefinPlan = value,
                                        region,
                                        defaultMeansType1,
                                        source.ID,
                                        defaultMeansType,
                                        defaultBdgt);
                                }

                                SaveParam(
                                    periodId,
                                    record,
                                    facts,
                                    mark,
                                    "FactPeriod",
                                    (value, fact) => fact.FactPeriod = value,
                                    region,
                                    defaultMeansType,
                                    source.ID,
                                    defaultMeansType,
                                    defaultBdgt);

                                SaveParam(
                                    periodId,
                                    record,
                                    facts,
                                    mark,
                                    "PlanPeriod",
                                    (value, fact) => fact.PlanPeriod = value,
                                    region,
                                    defaultMeansType,
                                    source.ID,
                                    defaultMeansType,
                                    defaultBdgt);

                                SaveScoreMO(
                                    periodForYear,
                                    record,
                                    facts,
                                    mark,
                                    month,
                                    region,
                                    variantsAll,
                                    source.ID,
                                    defaultMeansType,
                                    defaultBdgt);
                            }
                            else if (parentMarkName.Equals(MarkCredit))
                            {
                                SaveParam(
                                    periodId,
                                    record,
                                    facts,
                                    mark,
                                    "FactPeriod",
                                    (value, fact) => fact.FactPeriod = value,
                                    region,
                                    defaultMeansType,
                                    source.ID,
                                    defaultMeansType,
                                    defaultBdgt);
                            }
                            else if (parentMarkName.Equals(MarkReference))
                            {
                                SaveParam(
                                    periodLastYear,
                                    record,
                                    facts,
                                    mark,
                                    "FactLastYear",
                                    (value, fact) => fact.FactLastYear = value,
                                    region,
                                    defaultMeansType1,
                                    source.ID,
                                    defaultMeansType,
                                    defaultBdgt);
                            }
                        }
                    }
                }
            }
        }

        private void AddChildValues(
           int year,
           int month,
           D_Marks_PassportMO childMark,
           F_Marks_PassportMO factYear,
           F_Marks_PassportMO factPrevYear,
           F_Marks_PassportMO factMonth,
           F_Marks_PassportMO[] factMonthVar,
           string markName,
           IEnumerable<F_Marks_PassportMO> factsAll)
        {
            var childFacts = factsAll.Where(x => x.RefPassportMO.ID == childMark.ID);

            // факт по дочернему показателю для текущего района на период - год
            var factChildYear = childFacts.FirstOrDefault(x => 
                x.RefPasPeriod.ID == (year * 10000) + 1 && 
                x.RefVar.Code == -1);

            // факт по дочернему показателю для текущего района на период - предыдущий год
            var factChildPrevYear = childFacts.FirstOrDefault(x =>
                    x.RefPasPeriod.ID == ((year - 1) * 10000) + 1 &&
                    x.RefVar.Code == -1);

            // факт по дочернему показателю для текущего района на период - месяц
            var factChildMonth = childFacts.FirstOrDefault(x =>
                    x.RefPasPeriod.ID == (year * 10000) + (month * 100) &&
                    x.RefVar.Code == -1);

            // Если январь
            var isMarkIncome = markName.Equals(MarkIncome);
            var isMarkSources = markName.Equals(MarkSources);
            var isMarkCosts = markName.Equals(MarkCosts);
            var isMarkReference = markName.Equals(MarkReference);
            var isMarkCredit = markName.Equals(MarkCredit);
            if (month == 1)
            {
                if (isMarkIncome || isMarkSources || isMarkCosts)
                {
                    // ПЛАН НА ГОД (ПЕРВОНАЧАЛЬНО УТВЕРЖДЕННЫЙ)
                    if (factChildYear != null && factYear != null)
                    {
                        factYear.OrigPlan += factChildYear.OrigPlan ?? 0;
                    }
                }

                if (factChildPrevYear != null && factPrevYear != null)
                {
                    if (isMarkIncome || isMarkSources || isMarkCosts || isMarkReference)
                    {
                        // ИСПОЛНЕНО ЗА ОТЧЕТНЫЙ ГОД
                        factPrevYear.FactLastYear += factChildPrevYear.FactLastYear ?? 0;
                    }

                    if (isMarkIncome || isMarkSources || isMarkCosts)
                    {
                        // УТОЧНЕННЫЙ ПЛАН НА ГОД (прошлый год)
                        factPrevYear.RefinPlan += factChildPrevYear.RefinPlan ?? 0;
                    }
                }
            }

            if (factChildMonth != null && factMonth != null)
            {
                if (isMarkCredit || isMarkIncome || isMarkSources || isMarkCosts)
                {
                    // ИСПОЛНЕНО ЗА ОТЧЕТНЫЙ МЕСЯЦ
                    factMonth.FactPeriod += factChildMonth.FactPeriod ?? 0;
                }

                if (isMarkCosts || isMarkIncome || isMarkSources)
                {
                    // УТОЧНЕННЫЙ ПЛАН НА ГОД ПО МЕСЯЧНОМУ ОТЧЕТУ
                    factMonth.PlanPeriod += factChildMonth.PlanPeriod ?? 0;
                }
            }

            if (isMarkIncome || isMarkSources || isMarkCosts)
            {
                // ОЦЕНКА ИСПОЛНЕНИЯ ФЕВРАЛЯ, МАРТА ...
                for (var i = month + 1; i < 13; i++)
                {
                    // факт по дочернему показателю для текущего района на период - i, вариант - month
                    var factChildMonthVariant = childFacts == null
                                                    ? null
                                                    : childFacts.FirstOrDefault(
                                                        x =>
                                                        x.RefPasPeriod.ID == (year * 10000) + (i * 100) &&
                                                        x.RefVar.Code == month);

                    if (factChildMonthVariant != null)
                    {
                        factMonthVar[i].ScoreMO += factChildMonthVariant.ScoreMO ?? 0;
                    }
                }
            }
        }

        private void CalcMark38(
            int year, 
            int month, 
            F_Marks_PassportMO fact38Year, 
            F_Marks_PassportMO fact38PrevYear,
            F_Marks_PassportMO fact38Period, 
            F_Marks_PassportMO[] fact38MonthVar, 
            int regionID, 
            IEnumerable<F_Marks_PassportMO> factsAll)
        {
            var factsAll382 = factsAll.Where(x =>
                       x.RefPasRegions.ID == regionID &&
                       x.RefPassportMO.Code.Equals("3.8.2"));
            var factsAll381 = factsAll.Where(x =>
                       x.RefPasRegions.ID == regionID &&
                       x.RefPassportMO.Code.Equals("3.8.1"));

            // факт по дочернему показателю для текущего района на период - отчетный год
            var fact382Year = factsAll382.FirstOrDefault(x =>
                       x.RefPasPeriod.ID == (year * 10000) + 1 &&
                       x.RefVar.Code == -1);
            var fact381Year = factsAll381.FirstOrDefault(x =>
                       x.RefPasPeriod.ID == (year * 10000) + 1 &&
                       x.RefVar.Code == -1);

            // факт по дочернему показателю для текущего района на период - прошлый год
            var fact382LastYear = factsAll382.FirstOrDefault(x =>
                       x.RefPasPeriod.ID == ((year - 1) * 10000) + 1 &&
                       x.RefVar.Code == -1);
            var fact381LastYear = factsAll381.FirstOrDefault(x =>
                       x.RefPasPeriod.ID == ((year - 1) * 10000) + 1 &&
                       x.RefVar.Code == -1);

            // факт по дочернему показателю для текущего района на отчетный период
            var fact382Period = factsAll382.FirstOrDefault(x =>
                       x.RefPasPeriod.ID == (year * 10000) + (month * 100) &&
                       x.RefVar.Code == -1);
            var fact381Period = factsAll381.FirstOrDefault(x =>
                       x.RefPasPeriod.ID == (year * 10000) + (month * 100) &&
                       x.RefVar.Code == -1);

            // Если январь
            if (month == 1)
            {
                // ПЛАН НА ГОД (ПЕРВОНАЧАЛЬНО УТВЕРЖДЕННЫЙ)
                CalcDifference(fact38Year, fact382Year, fact381Year, (value, fact) => fact.OrigPlan = value, fact => fact.OrigPlan);
                CalcDifference(fact38PrevYear, fact382LastYear, fact381LastYear, (value, fact) => fact.FactLastYear = value, fact => fact.FactLastYear);
                CalcDifference(fact38PrevYear, fact382LastYear, fact381LastYear, (value, fact) => fact.RefinPlan = value, fact => fact.RefinPlan);
            }

            CalcDifference(fact38Period, fact382Period, fact381Period, (value, fact) => fact.FactPeriod = value, fact => fact.FactPeriod);
            CalcDifference(fact38Period, fact382Period, fact381Period, (value, fact) => fact.PlanPeriod = value, fact => fact.PlanPeriod);

            // ОЦЕНКА ИСПОЛНЕНИЯ ФЕВРАЛЯ, МАРТА ...
            for (var i = month + 1; i < 13; i++)
            {
                // факт по дочернему показателю для текущего района на период - i, вариант - month
                var fact382MonthVariant = factsAll382.FirstOrDefault(x =>
                    x.RefVar.Code == month &&
                    x.RefPasPeriod.ID == (year * 10000) + (i * 100));

                var fact381MonthVariant = factsAll381.FirstOrDefault(x =>
                    x.RefVar.Code == month &&
                    x.RefPasPeriod.ID == (year * 10000) + (i * 100));

                CalcDifference(fact38MonthVar[i], fact382MonthVariant, fact381MonthVariant, (value, fact) => fact.ScoreMO = value, fact => fact.ScoreMO);
            }
        }

        private void CalcDifference(
            F_Marks_PassportMO fact38Year, 
            F_Marks_PassportMO fact382Year, 
            F_Marks_PassportMO fact381Year, 
            ParameterSetterDelegat setter, 
            ParameterGetterDelegat getter)
        {
            var mark382 = (fact382Year == null) ? 0 : getter(fact382Year) ?? 0;
            var mark381 = (fact381Year == null) ? 0 : getter(fact381Year) ?? 0;
            setter(mark381 - mark382, fact38Year);
        }

        private void CalcFactForMark(
            D_Marks_PassportMO mark,
            int year,
            int month,
            string rootMarkName,
            D_Regions_Analysis region,
            ICollection<F_Marks_PassportMO> factsAll,
            IEnumerable<D_PMO_Variant> variantsAll,
            int sourceId,
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt,
            IEnumerable<D_Marks_PassportMO> marksAll)
        {
            var childMarks = marksAll.Where(x => x.ParentID == mark.ID).ToList();
            if (childMarks.Count() <= 0)
            {
                return;
            }

            // для показателя mark.Code.Equals("3.8.2") сохранять только FactPeriod и FactLastPeriod
            if (mark.Code.Equals("3.8.2"))
            {
                Calc382(mark, factsAll, (year * 10000) + (month * 100), sourceId, region, defaultMeansType, defaultBdgt, (value, fact) => fact.FactPeriod = value, fact => fact.FactPeriod);
                if (month == 1)
                {
                    Calc382(mark, factsAll, ((year - 1) * 10000) + 1, sourceId, region, defaultMeansType, defaultBdgt, (value, fact) => fact.FactLastYear = value, fact => fact.FactLastYear);
                }

                return;
            }

            // факты по показателю mark для текущего района
            var facts = factsAll
                .Where(x => x.RefPassportMO.ID == mark.ID);

            // запись на текущий год
            var factYear = facts == null
                               ? null
                               : facts.FirstOrDefault(x =>
                                                      x.RefPasPeriod.ID == (year * 10000) + 1 &&
                                                      x.RefVar.Code == -1 &&
                                                      x.SourceID == sourceId)
                                 ?? CreateFactDefault(periodRepository.Get((year * 10000) + 1), mark, region, sourceId, defaultMeansType, defaultBdgt);

            // запись на предыдущий год
            var factPrevYear = (rootMarkName.Equals(MarkCosts) || rootMarkName.Equals(MarkReference) || rootMarkName.Equals(MarkIncome) || rootMarkName.Equals(MarkSources))
                                   ? (facts == null
                                          ? null
                                          : facts.FirstOrDefault(x =>
                                                                 x.RefPasPeriod.ID == ((year - 1) * 10000) + 1 &&
                                                                 x.SourceID == sourceId &&
                                                                 x.RefVar.Code == -1)
                                            ?? CreateFactDefault(periodRepository.Get(((year - 1) * 10000) + 1), mark, region, sourceId, defaultMeansType, defaultBdgt))
                                   : null;

            // запись на месяц
            var factMonth = (rootMarkName.Equals(MarkCosts) || rootMarkName.Equals(MarkCredit) || rootMarkName.Equals(MarkIncome) || rootMarkName.Equals(MarkSources))
                                ? (facts == null
                                       ? null
                                       : facts.FirstOrDefault(x =>
                                                              x.RefPasPeriod.ID == (year * 10000) + (month * 100) &&
                                                              x.SourceID == sourceId &&
                                                              x.RefVar.Code == -1)
                                         ?? CreateFactDefault(periodRepository.Get((year * 10000) + (month * 100)), mark, region, sourceId, defaultMeansType, defaultBdgt))
                                : null;

            // запись на месяц и вариант / квартал
            F_Marks_PassportMO[] factMonthVar = GetFactsMonthVar(mark, year, month, sourceId, region, defaultMeansType, defaultBdgt, facts, rootMarkName, variantsAll);

            // рассчет дочерних показателей
            foreach (var childMark in childMarks)
            {
                CalcFactForMark(childMark, year, month, rootMarkName, region, factsAll, variantsAll, sourceId, defaultMeansType, defaultBdgt, marksAll);
            }

            // Суммирование дочерних показателей (с исключением).
            GetChildSumValues(mark, childMarks, factsAll, year, month, region, rootMarkName, factMonthVar, factYear, factPrevYear, factMonth);

            // Сохранение полученных значений
            SaveAfterCalc(factsAll, month, factMonthVar, factYear, factPrevYear, factMonth);
        }

        private F_Marks_PassportMO[] GetFactsMonthVar(
            D_Marks_PassportMO mark, 
            int year, 
            int month, 
            int sourceId, 
            D_Regions_Analysis region, 
            FX_MeansType_SKIF defaultMeansType, 
            FX_BdgtLevels_SKIF defaultBdgt, 
            IEnumerable<F_Marks_PassportMO> facts, 
            string rootMarkName, 
            IEnumerable<D_PMO_Variant> variantsAll)
        {
            F_Marks_PassportMO[] factMonthVar = null;
            if (rootMarkName.Equals(MarkIncome) || rootMarkName.Equals(MarkSources) ||
                rootMarkName.Equals(MarkCosts))
            {
                // значения по месяцам и вариантам
                factMonthVar = new F_Marks_PassportMO[13];
                for (var i = month + 1; i < 13; i++)
                {
                    factMonthVar[i] = facts == null
                                          ? null
                                          : facts.FirstOrDefault(x =>
                                                                 x.RefPasPeriod.ID == (year * 10000) + (i * 100) &&
                                                                 x.SourceID == sourceId &&
                                                                 x.RefVar.Code == month)
                                            ?? CreateFactDefault(periodRepository.Get((year * 10000) + (i * 100)), mark, region, sourceId, defaultMeansType, defaultBdgt);
                    if (factMonthVar[i] != null)
                    {
                        factMonthVar[i].RefVar = variantsAll.FirstOrDefault(x => x.Code == month);
                    }
                }
            }

            return factMonthVar;
        }

        private void GetChildSumValues(
            D_Marks_PassportMO mark, 
            IEnumerable<D_Marks_PassportMO> childMarks, 
            IEnumerable<F_Marks_PassportMO> factsAll, 
            int year, 
            int month, 
            D_Regions_Analysis region,
            string rootMarkName,
            F_Marks_PassportMO[] factMonthVar, 
            F_Marks_PassportMO factYear, 
            F_Marks_PassportMO factPrevYear, 
            F_Marks_PassportMO factMonth)
        {
            if (month == 1)
            {
                if (factYear != null)
                {
                    factYear.OrigPlan = 0;
                }

                if (factPrevYear != null)
                {
                    factPrevYear.RefinPlan = 0;
                    factPrevYear.FactLastYear = 0;
                }
            }

            if (factMonth != null)
            {
                factMonth.FactPeriod = 0;
                factMonth.PlanPeriod = 0;
            }

            for (int i = month + 1; i < 13; i++)
            {
                if (factMonthVar != null)
                {
                    factMonthVar[i].ScoreMO = 0;
                }
            }

            if (mark.Code.Equals("3.8"))
            {
                CalcMark38(year, month, factYear, factPrevYear, factMonth, factMonthVar, region.ID, factsAll);
            }
            else
            {
                // Суммирование значений дочерних показателей
                foreach (var childMark in childMarks)
                {
                    AddChildValues(year, month, childMark, factYear, factPrevYear, factMonth, factMonthVar, rootMarkName, factsAll);
                }
            }
        }

        private void SaveAfterCalc(
            ICollection<F_Marks_PassportMO> factsAll, 
            int month, 
            F_Marks_PassportMO[] factMonthVar,
            F_Marks_PassportMO factYear,
            F_Marks_PassportMO factPrevYear, 
            F_Marks_PassportMO factMonth)
        {
            if (factYear != null)
            {
                factsPassportRepository.Save(factYear);
                if (!factsAll.Contains(factYear))
                {
                    factsAll.Add(factYear);
                }
            }

            if (factPrevYear != null && month == 1)
            {
                if (factPrevYear.ID <= 0)
                {
                    factsPassportRepository.Save(factPrevYear);
                    factsAll.Add(factPrevYear);
                }
                else
                {
                    factsPassportRepository.Save(factPrevYear);
                }
            }

            if (factMonth != null)
            {
                if (factMonth.ID <= 0)
                {
                    factsPassportRepository.Save(factMonth);
                    factsAll.Add(factMonth);
                }
                else
                {
                    factsPassportRepository.Save(factMonth);
                }
            }

            if (factMonthVar != null)
            {
                for (var i = month + 1; i < 13; i++)
                {
                    if (factMonthVar[i].ID <= 0)
                    {
                        factsPassportRepository.Save(factMonthVar[i]);
                        factsAll.Add(factMonthVar[i]);
                    }
                    else
                    {
                        factsPassportRepository.Save(factMonthVar[i]);
                    }
                }
            }
        }

        private void Calc382(
            D_Marks_PassportMO mark, 
            ICollection<F_Marks_PassportMO> factsAll, 
            int periodId, 
            int sourceId, 
            D_Regions_Analysis region, 
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt, 
            ParameterSetterDelegat setter, 
            ParameterGetterDelegat getter)
        {
            var childFacts = factsAll.Where(x => 
                x.RefPassportMO.ParentID == mark.ID && 
                x.RefPasPeriod.ID == periodId &&
                x.RefVar.Code == -1 &&
                x.SourceID == sourceId);

            var fact382 = factsAll.FirstOrDefault(x =>
                                                  x.RefPassportMO.ID == mark.ID &&
                                                  x.RefPasPeriod.ID == periodId &&
                                                  x.RefVar.Code == -1 &&
                                                  x.SourceID == sourceId)
                          ?? CreateFactDefault(periodRepository.Get(periodId), mark, region, sourceId, defaultMeansType, defaultBdgt);
            
            setter(0, fact382);

            foreach (var childFact in childFacts)
            {
                setter(getter(fact382) + (childFact == null ? 0 : getter(childFact)), fact382);        
            }

            // Сохранение полученных значений
            factsPassportRepository.Save(fact382);
            if (!factsAll.Contains(fact382))
            {
                factsAll.Add(fact382);
            }
        }

        private decimal ConvertToDecimal(string value)
        {
            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            if (ci != null)
            {
                ci.NumberFormat.NumberDecimalSeparator = ",";
            }

            var val = value.Replace(".", ",");
            var result = Decimal.Parse(val, ci);
            return result;
        }

        private F_Marks_PassportMO CreateFactDefault(
            FX_Date_YearDayUNV period,
            D_Marks_PassportMO mark,
            D_Regions_Analysis region,
            int sourceId,
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt)
        {
            return new F_Marks_PassportMO
            {
                PumpID = -1,
                TaskID = -1,
                SourceID = sourceId,
                RefPasMeans = defaultMeansType,
                RefPasBdgt = defaultBdgt,
                RefVar = variantRepository.Get(-1),
                RefStatePasMO = stateRepository.Get(dataCreatedState),
                RefPasPeriod = period,
                RefPasRegions = regionRepository.Get(region.ID),
                RefPassportMO = mark,
                ScoreMO = 0,
                OrigPlan = 0,
                FactPeriod = 0,
                FactLastYear = 0,
                RefinPlan = 0,
                PlanPeriod = 0
            };
        }
        
        private void SaveParam(
            int periodId,
            IDictionary<string, object> record,
            ICollection<F_Marks_PassportMO> facts,
            D_Marks_PassportMO mark,
            string paramName,
            ParameterSetterDelegat setter,
            D_Regions_Analysis region,
            FX_MeansType_SKIF meansTypeSkif,
            int sourceId,
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt)
        {
            var param = record[paramName];
            if (param == null)
            {
                return;
            }

            var fact = facts.FirstOrDefault(x =>
                x.RefPasPeriod.ID == periodId &&
                x.RefPassportMO.ID == mark.ID &&
                x.RefVar.Code == -1 &&
                x.SourceID == sourceId);
            var isNew = false;
            if (fact == null)
            {
                fact = CreateFactDefault(periodRepository.Get(periodId), mark, region, sourceId, defaultMeansType, defaultBdgt);
                isNew = true;
            }

            if (param.Equals(String.Empty))
            {
                setter(null, fact);
            }
            else
            {
                setter(ConvertToDecimal(param.ToString()), fact);
            }

            fact.RefPasMeans = meansTypeSkif;

            factsPassportRepository.Save(fact);
            if (isNew)
            {
                facts.Add(fact);
            }
        }

        private void SaveScoreMO(
            int periodForYear,
            IDictionary<string, object> record,
            ICollection<F_Marks_PassportMO> facts,
            D_Marks_PassportMO mark,
            int month,
            D_Regions_Analysis region,
            IEnumerable<D_PMO_Variant> variantsAll,
            int sourceId,
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt)
        {
            // Сохраняем значения по месяцам
            for (var i = month + 1; i <= 12; i++)
            {
                var scoreMOiObj = record["ScoreMO{0}".FormatWith(i)];
                if (scoreMOiObj != null)
                {
                    var periodEstimate = periodForYear + (i * 100) - 1;
                    var fact = facts.FirstOrDefault(x =>
                        x.RefPasPeriod.ID == periodEstimate &&
                        x.RefPassportMO.ID == mark.ID &&
                        x.RefVar.Code == month);
                    var isNew = false;
                    if (fact == null)
                    {
                        fact = CreateFactDefault(periodRepository.Get(periodEstimate), mark, region, sourceId, defaultMeansType, defaultBdgt);
                        fact.RefVar = variantsAll.FirstOrDefault(x => x.Code == month);
                        isNew = true;
                    }

                    if (scoreMOiObj.Equals(String.Empty))
                    {
                        fact.ScoreMO = null;
                    }
                    else
                    {
                        fact.ScoreMO = ConvertToDecimal(scoreMOiObj.ToString());
                    }

                    factsPassportRepository.Save(fact);
                    if (isNew)
                    {
                        facts.Add(fact);
                    }
                }
            }

            // список значений по месяцам для рассчета значений по кварталам.
            var estimateByMonth = new List<decimal>();
             
            // до отчетного периода включительно берем значения - исполнено за отч. месяц
            for (var i = 1; i <= month; i++)
            {
                var factMonth = facts.FirstOrDefault(
                    x =>
                    x.RefPasPeriod.ID == periodForYear - 1 + (i * 100) && x.RefVar.Code == -1 &&
                    x.RefPassportMO.ID == mark.ID);
                if (factMonth == null)
                {
                    estimateByMonth.Add(0);
                }
                else
                {
                    estimateByMonth.Add(factMonth.FactPeriod ?? 0);
                }
            }

            // после берем оценку
            for (var i = month + 1; i <= 12; i++)
            {
                    var periodEstimate = periodForYear + (i * 100) - 1;
                    var factMonth = facts.FirstOrDefault(x =>
                        x.RefPasPeriod.ID == periodEstimate &&
                        x.RefPassportMO.ID == mark.ID &&
                        x.RefVar.Code == month);
                    if (factMonth == null)
                    {
                        estimateByMonth.Add(0);
                    }
                    else
                    {
                        estimateByMonth.Add(factMonth.ScoreMO ?? 0);
                    }
            }
        }
    }
}
