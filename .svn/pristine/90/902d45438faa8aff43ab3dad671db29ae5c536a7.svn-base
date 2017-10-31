using System;
using System.Collections.Generic;
using System.Linq;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public class ControlService : IControlService
    {
        private readonly IFO51Extension extension;
        private readonly ILinqRepository<D_Marks_PassportMO> marksPassportRepository;
        private readonly ILinqRepository<F_F_MonthRepOutcomes> monthOutcomesRepository;
        private readonly ILinqRepository<F_F_MonthRepIncomes> monthRepIncomesRepository;
        private readonly ILinqRepository<F_F_MonthRepInFin> monthRepInFinREpository;
        private readonly ILinqRepository<F_F_MonthRepDefProf> monthRepDefProfRepository;
        private readonly ILinqRepository<D_EKR_MonthRep> ekrMonthRepRepository;
        private readonly ILinqRepository<DataSources> sourcesRepository;
        private readonly IFactsPassportMOService factsService;

        public ControlService(
            IFO51Extension extension,
            ILinqRepository<D_Marks_PassportMO> marksPassportRepository,
            IFactsPassportMOService factsService,
            ILinqRepository<DataSources> sourcesRepository,
            ILinqRepository<F_F_MonthRepOutcomes> monthOutcomesRepository,
            ILinqRepository<F_F_MonthRepIncomes> monthRepIncomesRepository,
            ILinqRepository<F_F_MonthRepInFin> monthRepInFinREpository,
            ILinqRepository<F_F_MonthRepDefProf> monthRepDefProfRepository,
            ILinqRepository<D_EKR_MonthRep> ekrMonthRepRepository)
        {
            this.monthOutcomesRepository = monthOutcomesRepository;
            this.monthRepIncomesRepository = monthRepIncomesRepository;
            this.monthRepInFinREpository = monthRepInFinREpository;
            this.monthRepDefProfRepository = monthRepDefProfRepository;
            this.ekrMonthRepRepository = ekrMonthRepRepository;
            this.extension = extension;
            this.marksPassportRepository = marksPassportRepository;
            this.factsService = factsService;
            this.sourcesRepository = sourcesRepository;
        }

        public List<FO51ControlModel> GetDefects(int periodId, D_Regions_Analysis region)
        {
            var year = periodId / 10000;
            var month = (periodId / 100) % 100;
            var source = extension.DataSourcesFO51.FirstOrDefault(x => x.Year.Equals(year.ToString()));
            if (source == null)
            {
                throw new Exception("Источник ФО 51 Финансовый паспорт МО на {0} год не найден. Обратитесь к администратору"
                                         .FormatWith(year));
            }

            var regionBridgeId = region.RefBridgeRegions.ID;

            var marksTop = marksPassportRepository.FindAll()
              .Where(x => x.RefTypeMark.ID == 1 && x.SourceID == source.ID).OrderBy(x => x.Code);
            var markIncome = marksTop.FirstOrDefault(x => x.Code.Equals("1"));
            var markCosts = marksTop.FirstOrDefault(x => x.Code.Equals("2"));
            var markSources = marksTop.FirstOrDefault(x => x.Code.Equals("3"));

            var modelIncome = GetModelByMark(markIncome, region.ID, source.ID, periodId);
            var modelCosts = GetModelByMark(markCosts, region.ID, source.ID, periodId);
            var modelSources = GetModelByMark(markSources, region.ID, source.ID, periodId);
            var deficitFactPeriod = (modelIncome.PassportMOFactPeriod ?? 0) - 
                (modelCosts.PassportMOFactPeriod ?? 0);
            var deficitPlanPeriod = (modelIncome.PassportMOPlanPeriod ?? 0) - 
                (modelCosts.PassportMOPlanPeriod ?? 0);
            var modelDeficit = new FO51ControlModel
                                   {
                                       MarkId = 0,
                                       MarkName = "Дефицит/Профицит",
                                       PassportMOFactPeriod = deficitFactPeriod,
                                       PassportMOPlanPeriod = deficitPlanPeriod,
                                   };

            // TODO ИСТОЧНИК!
            var sourcesFO2 = sourcesRepository.FindAll().FirstOrDefault(x =>
                   x.SupplierCode.Equals("ФО") &&
                   x.DataCode == 2 &&
                   x.Year.Equals(year.ToString()) &&
                   x.Month.Equals(month.ToString()));

            var sourcesFO2Prev = sourcesRepository.FindAll().FirstOrDefault(x =>
                    x.SupplierCode.Equals("ФО") &&
                    x.DataCode == 2 &&
                    x.Year.Equals(year.ToString()) &&
                    x.Month.Equals((month - 1).ToString()));

            int? sourcePrevId;
            if (sourcesFO2Prev == null)
            {
                sourcePrevId = null;
            }
            else
            {
                sourcePrevId = sourcesFO2Prev.ID;
            }

            if (sourcesFO2 != null && (sourcesFO2Prev != null || month == 1))
            {
                GetMesOtchCosts(month, regionBridgeId, periodId, modelCosts, sourcesFO2.ID, sourcePrevId);
                GetMesOtchIncome(month, regionBridgeId, periodId, modelIncome, sourcesFO2.ID, sourcePrevId);
                GetMesOtchSources(month, regionBridgeId, periodId, modelSources, sourcesFO2.ID, sourcePrevId);
                GetMesOtchDefProf(month, regionBridgeId, periodId, modelDeficit, sourcesFO2.ID, sourcePrevId);
            }

            CalcDefect(modelIncome);
            CalcDefect(modelCosts);
            CalcDefect(modelSources);
            CalcDefect(modelDeficit);
           
            return new List<FO51ControlModel> { modelIncome, modelCosts, modelSources, modelDeficit };
        }

        private void CalcDefect(FO51ControlModel model)
        {
            model.DefectFactPeriod = model.PassportMOFactPeriod - model.MesOtchFactPeriod;
            model.DefectPlanPeriod = model.PassportMOPlanPeriod - model.MesOtchPlanPeriod;
        }

        private FO51ControlModel GetModelByMark(D_Marks_PassportMO markIncome, int regionId, int sourceId, int periodId)
        {
            var factIncome = factsService.FindFirstOrDefault(regionId, sourceId, markIncome.ID, periodId, -1);

            return new FO51ControlModel
                       {
                           MarkId = markIncome.ID,
                           MarkName = markIncome.Name,
                           PassportMOFactPeriod = factIncome == null ? null : factIncome.FactPeriod,
                           PassportMOPlanPeriod = factIncome == null ? null : factIncome.PlanPeriod,
                       };
        }

        private void GetMesOtchDefProf(int month, int regionBridgeId, int periodId, FO51ControlModel model, int sourcesFO2Id, int? sourcesFO2PrevId)
        {
            // Уровень бюджета СКИФ («RefBdgtLevels»): элементы «Бюджет гор.округа», «Бюджет района» и «Бюджет поселения».
            var monthOutcomesCur = monthRepDefProfRepository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == periodId) &&
                       x.SourceID == sourcesFO2Id)
                .ToList();

            var monthOutcomesPrev = monthRepDefProfRepository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == periodId - 100) &&
                       x.SourceID == sourcesFO2PrevId)
                .ToList();

            if (model.MesOtchFactPeriod == null)
            {
                model.MesOtchFactPeriod = 0;
            }

            if (model.MesOtchPlanPeriod == null)
            {
                model.MesOtchPlanPeriod = 0;
            }

            // TODO Code
            var mesOtchData = monthOutcomesCur.Where(x => x.RefYearDayUNV.ID == periodId);
            var mesOtchPrevData = month == 1 ? null : monthOutcomesPrev.Where(x => x.RefYearDayUNV.ID == periodId - 100);

            if (mesOtchData.Count() > 0)
            {
                foreach (var mesOtch in mesOtchData)
                {
                    model.MesOtchFactPeriod += mesOtch.Fact ?? 0;
                    model.MesOtchPlanPeriod += mesOtch.YearPlan ?? 0;
                }
            }

            if (mesOtchPrevData != null)
            {
                foreach (var mesOtch in mesOtchPrevData)
                {
                    model.MesOtchFactPeriod -= mesOtch.Fact ?? 0;
                }
            }
        }

        private void GetMesOtchIncome(int month, int regionBridgeId, int periodId, FO51ControlModel model, int sourcesFO2Id, int? sourcesFO2PrevId)
        {
            // Уровень бюджета СКИФ («RefBdgtLevels»): элементы «Бюджет гор.округа», «Бюджет района» и «Бюджет поселения».
            var monthOutcomesCur = monthRepIncomesRepository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == periodId) &&
                       x.SourceID == sourcesFO2Id)
                .ToList();

            var monthOutcomesPrev = monthRepIncomesRepository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == periodId - 100) &&
                       x.SourceID == sourcesFO2PrevId)
                .ToList();

            if (model.MesOtchFactPeriod == null)
            {
                model.MesOtchFactPeriod = 0;
            }

            if (model.MesOtchPlanPeriod == null)
            {
                model.MesOtchPlanPeriod = 0;
            }

            // TODO Code
            var mesOtchData = monthOutcomesCur.Where(x => x.RefYearDayUNV.ID == periodId && x.RefKD.CodeStr.Equals("00085900000000000000"));
            var mesOtchPrevData = month == 1 ? null : monthOutcomesPrev.Where(x => x.RefYearDayUNV.ID == periodId - 100 && x.RefKD.CodeStr.Equals("00085900000000000000"));

            if (mesOtchData.Count() > 0)
            {
                foreach (var mesOtch in mesOtchData)
                {
                    model.MesOtchFactPeriod += mesOtch.Fact ?? 0;
                    model.MesOtchPlanPeriod += mesOtch.YearPlan ?? 0;
                }
            }

            if (mesOtchPrevData != null)
            {
                foreach (var mesOtch in mesOtchPrevData)
                {
                    model.MesOtchFactPeriod -= mesOtch.Fact ?? 0;
                }
            }
        }

        private void GetMesOtchSources(int month, int regionBridgeId, int periodId, FO51ControlModel model, int sourcesFO2Id, int? sourcesFO2PrevId)
        {
            // Уровень бюджета СКИФ («RefBdgtLevels»): элементы «Бюджет гор.округа», «Бюджет района» и «Бюджет поселения».
            var monthOutcomesCur = monthRepInFinREpository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == periodId) &&
                       x.SourceID == sourcesFO2Id)
                .ToList();

            var monthOutcomesPrev = monthRepInFinREpository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == periodId - 100) &&
                       x.SourceID == sourcesFO2PrevId)
                .ToList();

            if (model.MesOtchFactPeriod == null)
            {
                model.MesOtchFactPeriod = 0;
            }

            if (model.MesOtchPlanPeriod == null)
            {
                model.MesOtchPlanPeriod = 0;
            }

            // TODO Code
            var mesOtchData = monthOutcomesCur.Where(x => x.RefYearDayUNV.ID == periodId && x.RefSIF.CodeStr.Equals("00090000000000000000"));
            var mesOtchPrevData = month == 1 ? null : monthOutcomesPrev.Where(x => x.RefYearDayUNV.ID == periodId - 100 && x.RefSIF.CodeStr.Equals("00090000000000000000"));

            if (mesOtchData.Count() > 0)
            {
                foreach (var mesOtch in mesOtchData)
                {
                    model.MesOtchFactPeriod += mesOtch.Fact ?? 0;
                    model.MesOtchPlanPeriod += mesOtch.YearPlan ?? 0;
                }
            }

            if (mesOtchPrevData != null)
            {
                foreach (var mesOtch in mesOtchPrevData)
                {
                    model.MesOtchFactPeriod -= mesOtch.Fact ?? 0;
                }
            }
        }

        private void GetMesOtchCosts(int month, int regionBridgeId, int periodId, FO51ControlModel model, int sourcesFO2Id, int? sourcesFO2PrevId)
        {
            // Уровень бюджета СКИФ («RefBdgtLevels»): элементы «Консолидированный бюджет субьекта».
            var monthOutcomesCur = monthOutcomesRepository.FindAll().Where(x =>
                       x.RefBdgtLevels.ID == 2 &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == periodId) &&
                       x.SourceID == sourcesFO2Id)
                .ToList();

            List<F_F_MonthRepOutcomes> monthOutcomesPrev;
            if (sourcesFO2PrevId == null)
            {
                monthOutcomesPrev = null;
            }
            else
            {
                monthOutcomesPrev =
                    monthOutcomesRepository.FindAll().Where(
                        x =>
                        x.RefBdgtLevels.ID == 2 &&
                        x.RefRegions.RefRegionsBridge != null && x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                        (x.RefYearDayUNV.ID == periodId - 100) && x.SourceID == sourcesFO2PrevId).ToList();
            }

            var ekrAll = ekrMonthRepRepository.FindAll().ToList();
            AddMesOtchFactCosts(month, periodId, 200, ekrAll, monthOutcomesCur, monthOutcomesPrev, model, sourcesFO2Id, sourcesFO2PrevId);
            AddMesOtchFactCosts(month, periodId, 300, ekrAll, monthOutcomesCur, monthOutcomesPrev, model, sourcesFO2Id, sourcesFO2PrevId);
            AddMesOtchFactCosts(month, periodId, 500, ekrAll, monthOutcomesCur, monthOutcomesPrev, model, sourcesFO2Id, sourcesFO2PrevId);
        }

        private void AddMesOtchFactCosts(
            int month, 
            int periodId,
            int mesOtchCode,
            List<D_EKR_MonthRep> ekrAll,
            IEnumerable<F_F_MonthRepOutcomes> monthOutcomesCur,
            IEnumerable<F_F_MonthRepOutcomes> monthOutcomesPrev,
            FO51ControlModel model,
            int sourceId, 
            int? prevSourceId)
        {
            if (model.MesOtchFactPeriod == null)
            {
                model.MesOtchFactPeriod = 0;
            }

            if (model.MesOtchPlanPeriod == null)
            {
                model.MesOtchPlanPeriod = 0;
            }

            var listMarkIds = GetListMarkIds(ekrAll, mesOtchCode, sourceId);
            var listPrevMarkIds = prevSourceId == null   || month == 1 ? null : GetListMarkIds(ekrAll, mesOtchCode, (int)prevSourceId);

            var mesOtchData = monthOutcomesCur.Where(x => x.RefYearDayUNV.ID == periodId && listMarkIds.Contains(x.RefEKR.ID));
            var mesOtchPrevData = (month == 1 || monthOutcomesPrev == null || listPrevMarkIds == null) 
                ? null
                : monthOutcomesPrev.Where(x => x.RefYearDayUNV.ID == periodId - 100 && listPrevMarkIds.Contains(x.RefEKR.ID));

            if (mesOtchData.Count() > 0)
            {
                foreach (var mesOtch in mesOtchData)
                {
                    model.MesOtchFactPeriod += mesOtch.Fact ?? 0;
                    model.MesOtchPlanPeriod += mesOtch.YearPlan ?? 0;
                }
            }

            if (mesOtchPrevData != null)
            {
                foreach (var mesOtch in mesOtchPrevData)
                {
                    model.MesOtchFactPeriod -= mesOtch.Fact ?? 0;
                }
            }
        }

        private List<int> GetListMarkIds(List<D_EKR_MonthRep> ekrAll, int mesOtchCode, int sourceId)
        {
            var mark = ekrMonthRepRepository.FindAll().FirstOrDefault(x => x.Code == mesOtchCode && x.SourceID == sourceId);
            var listResultMarks = new List<D_EKR_MonthRep>();
            var listParentMarks = new List<D_EKR_MonthRep>();
            listParentMarks.Add(mark);

            while (listParentMarks.Count > 0)
            {
                var newParentMarks = new List<D_EKR_MonthRep>();
                foreach (var parentMark in listParentMarks)
                {
                    var childs = ekrAll.Where(x => x.ParentID == parentMark.ID);
                    if (childs.Count() > 0)
                    {
                        newParentMarks.AddRange(childs);
                    }
                    else
                    {
                        listResultMarks.Add(parentMark);
                    }
                }

                listParentMarks.Clear();
                listParentMarks = newParentMarks;
            }

            return (from m in listResultMarks select m.ID).ToList();
        }
    }
}
