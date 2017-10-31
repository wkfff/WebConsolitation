using System;
using System.Collections.Generic;
using System.Linq;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public class FactsCheckDefectsService : IFactsCheckDefectsService
    {
        private readonly ILinqRepository<F_F_MonthRepOutcomes> factsMonthRepRepository;
        private readonly IFO51Extension extension;
        private readonly ILinqRepository<D_Marks_PassportMO> marksPassportRepository;
        private readonly ILinqRepository<F_Marks_PassportMO> factsPassportRepository;
        private readonly ILinqRepository<D_EKR_MonthRep> ekrMonthRepRepository;
        private readonly List<DataSources> sourcesFO2;
        private readonly decimal limit = (decimal)0.01;

        public FactsCheckDefectsService(
            IFO51Extension extension,
            ILinqRepository<D_Marks_PassportMO> marksPassportRepository,
            ILinqRepository<F_Marks_PassportMO> factsPassportRepository,
            ILinqRepository<D_EKR_MonthRep> ekrMonthRepRepository,
            ILinqRepository<F_F_MonthRepOutcomes> factsMonthRepRepository,
            ILinqRepository<DataSources> sourcesRepository)
        {
            this.factsMonthRepRepository = factsMonthRepRepository;
            sourcesFO2 = sourcesRepository.FindAll()
                .Where(x => x.SupplierCode.Equals("ФО") && x.DataCode == 2).ToList();
            this.extension = extension;
            this.marksPassportRepository = marksPassportRepository;
            this.factsPassportRepository = factsPassportRepository;
            this.ekrMonthRepRepository = ekrMonthRepRepository;
        }

        public DefectsListModel GetDefects(
            int periodId, 
            D_Regions_Analysis region, 
            bool exitIfDefect)
        {
            var defects = new DefectsListModel
                              {
                                  ErrorMsg = String.Empty,
                                  IsCorrect = true,
                                  DefectsList = new List<DefectModel>()
                              };

            var year = (periodId / 10000).ToString();
            var source = extension.DataSourcesFO51.FirstOrDefault(x => x.Year.Equals(year));
            if (source == null)
            {
                defects.IsCorrect = false;
                defects.ErrorMsg = "Источник ФО 51 Финансовый паспорт МО на {0} год не найден. Обратитесь к администратору"
                    .FormatWith(year);
                return defects;
            }

            // Получаем значения по показателям, которые нужно проверить
            var controlFacts = new List<F_Marks_PassportMO>();
            var res = GetControlFacts(periodId, controlFacts, region, source.ID);
            if (res != null)
            {
                defects.ErrorMsg = "{0} {1}".FormatWith(defects.ErrorMsg, res);
            }

            // Получаем значения месячной отчетности
            var factsMonthReps = new List<MonthOutcomesModel>();
            var factsPrevMonthReps = new List<MonthOutcomesModel>();
            res = GetControlRepMonthFacts(periodId, factsMonthReps, factsPrevMonthReps, region);
            if (res != null)
            {
                defects.ErrorMsg = "{0} {1}".FormatWith(defects.ErrorMsg, res);
            }

            // производим сверку
            var resultData = CheckControlFactsGetDefects(
                periodId,
                controlFacts, 
                factsMonthReps,
                factsPrevMonthReps, 
                exitIfDefect);

            if (resultData.ErrorMsg.IsNotEmpty())
            {
                defects.ErrorMsg = "{0} {1}".FormatWith(resultData.ErrorMsg, defects.ErrorMsg);
            }

            defects.DefectsList = resultData.DefectsList;
            defects.IsCorrect = defects.IsCorrect && resultData.IsCorrect;
            return defects;
        }

        public bool ReportDataExists(int periodId, D_Regions_Analysis region)
        {
            // Получаем значения месячной отчетности
            try
            {
                var regionBridgeId = region.RefBridgeRegions.ID;
                var monthOutcomes = factsMonthRepRepository.FindAll()
                    .Where(x =>
                           (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                           x.RefRegions.RefRegionsBridge != null &&
                           x.RefRegions.RefRegionsBridge.ID == regionBridgeId);

                var year = periodId / 10000;
                var month = (periodId / 100) % 100;
                var source = sourcesFO2.FirstOrDefault(x =>
                                                       x.Year.Equals(year.ToString()) &&
                                                       x.Month.Equals(month.ToString()));
                if (source == null)
                {
                    return false;
                }
                
                var sourceId = source.ID;
                var cntReportData = monthOutcomes.Count(x =>
                                                        x.RefEKR.SourceID == sourceId &&
                                                        x.RefYearDayUNV.ID == periodId);
                if (cntReportData > 0)
                {
                    return true;
                }

                periodId -= 100;
                year = periodId / 10000;
                month = (periodId / 100) % 100;
                source = sourcesFO2.FirstOrDefault(x =>
                                                   x.Year.Equals(year.ToString()) &&
                                                   x.Month.Equals(month.ToString()));
                if (source == null)
                {
                    return false;
                }

                sourceId = source.ID;

                cntReportData = monthOutcomes.Count(x =>
                                                    x.RefEKR.SourceID == sourceId &&
                                                    x.RefYearDayUNV.ID == periodId);
                return cntReportData > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private DefectsListModel CheckControlFactsGetDefects(
            int periodId,
            IList<F_Marks_PassportMO> controlFacts,
            IList<MonthOutcomesModel> factsMonthReps, 
            IList<MonthOutcomesModel> factsPrevMonthReps,
            bool exitIfDefect)
        {
            var defects = new DefectsListModel
                              {
                                  IsCorrect = true,
                                  ErrorMsg = String.Empty,
                                  DefectsList = new List<DefectModel>()
                              };

            try
            {
                var controlFact2111 = controlFacts[0];
                var controlFact2121 = controlFacts[1];
                var controlFact2112 = controlFacts[2];
                var controlFact2122 = controlFacts[3];
                var controlFact214 = controlFacts[4];
                var controlFact216 = controlFacts[5];
                var controlFact231 = controlFacts[6];
                var controlFact2333 = controlFacts[7];

                var factsMonthReps211 = factsMonthReps[0];
                var factsMonthReps213 = factsMonthReps[1];
                var factsMonthReps260 = factsMonthReps[2];
                var factsMonthReps340 = factsMonthReps[3];
                var factsMonthReps310 = factsMonthReps[4];
                var factsMonthReps320 = factsMonthReps[5];
                var factsMonthReps330 = factsMonthReps[6];
                var factsMonthReps242 = factsMonthReps[7];

                var factsPrevMonthReps211 = factsPrevMonthReps[0];
                var factsPrevMonthReps213 = factsPrevMonthReps[1];
                var factsPrevMonthReps260 = factsPrevMonthReps[2];
                var factsPrevMonthReps340 = factsPrevMonthReps[3];
                var factsPrevMonthReps310 = factsPrevMonthReps[4];
                var factsPrevMonthReps320 = factsPrevMonthReps[5];
                var factsPrevMonthReps330 = factsPrevMonthReps[6];
                var factsPrevMonthReps242 = factsPrevMonthReps[7];

                bool hasDefect;
                
                bool localMonthDataEmpty;

                var month = (periodId / 100) % 100;

                // зарплата
                var defect = GetDefectSumFact(
                    month,
                    defects.DefectsList.Count,
                    controlFact2121,
                    controlFact2111,
                    factsMonthReps211,
                    factsPrevMonthReps211,
                    out hasDefect,
                    out localMonthDataEmpty);

                defects.ErrorMsg = localMonthDataEmpty ? "Ежемесячный отчет не заполнен." : String.Empty;

                if (hasDefect)
                {
                    defects.IsCorrect = false;
                    if (exitIfDefect)
                    {
                        return defects;
                    }
                }
                
                defects.DefectsList.Add(defect);

                // начисления
                defect = GetDefectSumFact(
                    month,
                    defects.DefectsList.Count,
                    controlFact2122,
                    controlFact2112,
                    factsMonthReps213,
                    factsPrevMonthReps213,
                    out hasDefect,
                    out localMonthDataEmpty);

                defects.ErrorMsg = localMonthDataEmpty ? "Ежемесячный отчет не заполнен." : defects.ErrorMsg;
                
                if (hasDefect)
                {
                    defects.IsCorrect = false;
                    if (exitIfDefect)
                    {
                        return defects;
                    }
                }

                defects.DefectsList.Add(defect);

                // Социальное обеспечение
                defect = GetDefect(
                    month,
                    defects.DefectsList.Count,
                    controlFact214,
                    factsMonthReps260,
                    factsPrevMonthReps260,
                    out hasDefect,
                    out localMonthDataEmpty);

                defects.ErrorMsg = localMonthDataEmpty ? "Ежемесячный отчет не заполнен." : defects.ErrorMsg;

                if (hasDefect)
                {
                    defects.IsCorrect = false;
                    if (exitIfDefect)
                    {
                        return defects;
                    }
                }

                defects.DefectsList.Add(defect);

                // Увеличение стоимости нематериальных запасов
                defect = GetDefect(
                    month,
                    defects.DefectsList.Count,
                    controlFact216,
                    factsMonthReps340,
                    factsPrevMonthReps340,
                    out hasDefect,
                    out localMonthDataEmpty);

                defects.ErrorMsg = localMonthDataEmpty ? "Ежемесячный отчет не заполнен." : defects.ErrorMsg;

                if (hasDefect)
                {
                    defects.IsCorrect = false;
                    if (exitIfDefect)
                    {
                        return defects;
                    }
                }

                defects.DefectsList.Add(defect);

                // Капитальные вложения 
                defect = GetDefectSumRep(
                    month,
                    defects.DefectsList.Count,
                    controlFact231,
                    factsMonthReps310,
                    factsPrevMonthReps310,
                    factsMonthReps320,
                    factsPrevMonthReps320,
                    factsMonthReps330,
                    factsPrevMonthReps330,
                    out hasDefect,
                    out localMonthDataEmpty);

                defects.ErrorMsg = localMonthDataEmpty ? "Ежемесячный отчет не заполнен." : defects.ErrorMsg;

                if (hasDefect)
                {
                    defects.IsCorrect = false;
                    if (exitIfDefect)
                    {
                        return defects;
                    }
                }

                defects.DefectsList.Add(defect);

                // Увеличение стоимости нематериальных запасов
                defect = GetDefect(
                    month,
                    defects.DefectsList.Count,
                    controlFact2333,
                    factsMonthReps242,
                    factsPrevMonthReps242,
                    out hasDefect,
                    out localMonthDataEmpty);

                defects.ErrorMsg = localMonthDataEmpty ? "Ежемесячный отчет не заполнен." : defects.ErrorMsg;

                if (hasDefect)
                {
                    defects.IsCorrect = false;
                    if (exitIfDefect)
                    {
                        return defects;
                    }
                }

                defects.DefectsList.Add(defect);
            }
            catch (Exception)
            {
                defects.IsCorrect = false;
            }

            return defects;
        }

        // Рассчет нарушения (суммирование по данным МО)
        private DefectModel GetDefectSumFact(
            int month,
            int id, 
            F_Marks_PassportMO controlFact2121, 
            F_Marks_PassportMO controlFact2111, 
            MonthOutcomesModel factsMonthReps211, 
            MonthOutcomesModel factsPrevMonthReps211,
            out bool hasDefect,
            out bool monthDataEmpty)
        {
            var defect = new DefectModel
                             {
                                 ID = id,
                                 Name = controlFact2121.RefPassportMO.Name
                             };

            var fact = (controlFact2111.FactPeriod ?? 0) + (controlFact2121.FactPeriod ?? 0);

            decimal otch;
            if (factsMonthReps211.Fact == null)
            {
                monthDataEmpty = true;
                otch = 0;
            }
            else
            {
                monthDataEmpty = false;
                otch = (decimal)factsMonthReps211.Fact - (month == 1 ? 0 : (factsPrevMonthReps211.Fact ?? 0));
            }

            defect.FactPeriodMO = fact;
            defect.FactPeriodMonthReport = otch;
            hasDefect = Math.Abs(fact - otch) >= limit;
            defect.FactPeriodDefect = hasDefect ? fact - otch : 0;

            fact = (controlFact2111.PlanPeriod ?? 0) + (controlFact2121.PlanPeriod ?? 0);
            otch = factsMonthReps211.YearPlan ?? 0;
            defect.PlanPeriodMO = fact;
            defect.PlanPriodMonthReport = otch;
            hasDefect = Math.Abs(fact - otch) >= limit;
            defect.PlanPeriodDefect = hasDefect ? fact - otch : 0;

            return defect;
        }

        // Рассчет нарушения (нет суммирования)
        private DefectModel GetDefect(
            int month,
            int id, 
            F_Marks_PassportMO controlFact, 
            MonthOutcomesModel factsMonthReps, 
            MonthOutcomesModel factsPrevMonthReps,
            out bool hasDefect,
            out bool monthDataEmpty)
        {
            var defect = new DefectModel
            {
                ID = id,
                Name = controlFact.RefPassportMO.Name
            };

            var fact = controlFact.FactPeriod ?? 0;

            decimal otch;
            if (factsMonthReps.Fact == null)
            {
                monthDataEmpty = true;
                otch = 0;
            }
            else
            {
                monthDataEmpty = false;
                otch = (decimal)factsMonthReps.Fact - (month == 1 ? 0 : (factsPrevMonthReps.Fact ?? 0));
            }

            defect.FactPeriodMO = fact;
            defect.FactPeriodMonthReport = otch;
            hasDefect = Math.Abs(fact - otch) >= limit;
            defect.FactPeriodDefect = hasDefect ? fact - otch : 0;
            
            fact = controlFact.PlanPeriod ?? 0;
            otch = factsMonthReps.YearPlan ?? 0;
            defect.PlanPeriodMO = fact;
            defect.PlanPriodMonthReport = otch;
            hasDefect = Math.Abs(fact - otch) >= limit;
            defect.PlanPeriodDefect = hasDefect ? fact - otch : 0;

            return defect;
        }

        // Рассчет нарушения (суммирование по отчетным данным)
        private DefectModel GetDefectSumRep(
            int month,
            int id, 
            F_Marks_PassportMO controlFact, 
            MonthOutcomesModel factsMonthReps1, 
            MonthOutcomesModel factsPrevMonthReps1,
            MonthOutcomesModel factsMonthReps2, 
            MonthOutcomesModel factsPrevMonthReps2,
            MonthOutcomesModel factsMonthReps3,
            MonthOutcomesModel factsPrevMonthReps3,
            out bool hasDefect,
            out bool monthDataEmpty)
        {
            var defect = new DefectModel
            {
                ID = id,
                Name = controlFact.RefPassportMO.Name
            };

            var fact = controlFact.FactPeriod ?? 0;

            decimal otch;
            if (factsMonthReps1.Fact == null && factsMonthReps2.Fact == null && factsMonthReps3.Fact == null)
            {
                monthDataEmpty = true;
                otch = 0;
            }
            else
            {
                monthDataEmpty = false;
                otch = (factsMonthReps1.Fact ?? 0) + (factsMonthReps2.Fact ?? 0) + (factsMonthReps3.Fact ?? 0)
                       - (month == 1 
                                ? 0
                                : ((factsPrevMonthReps1.Fact ?? 0) + 
                                    (factsPrevMonthReps2.Fact ?? 0) +
                                    (factsPrevMonthReps3.Fact ?? 0)));
            }

            defect.FactPeriodMO = fact;
            defect.FactPeriodMonthReport = otch;
            hasDefect = Math.Abs(fact - otch) >= limit;
            defect.FactPeriodDefect = hasDefect ? fact - otch : 0;

            fact = controlFact.PlanPeriod ?? 0;
            otch = (factsMonthReps1.YearPlan ?? 0) + (factsMonthReps2.YearPlan ?? 0) + (factsMonthReps3.YearPlan ?? 0);
            defect.PlanPeriodMO = fact;
            defect.PlanPriodMonthReport = otch;
            hasDefect = Math.Abs(fact - otch) >= limit;
            defect.PlanPeriodDefect = hasDefect ? fact - otch : 0;

            return defect;
        }

        private string GetControlRepMonthFacts(
            int periodId, 
            ICollection<MonthOutcomesModel> factsMonthReps, 
            ICollection<MonthOutcomesModel> factsPrevMonthReps, 
            D_Regions_Analysis region)
        {
            var regionBridgeId = region.RefBridgeRegions.ID;

            var monthOutcomesPeriod = factsMonthRepRepository.FindAll().Where(x =>
                                    (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                                    x.RefRegions.RefRegionsBridge.ID == regionBridgeId && x.RefYearDayUNV.ID == periodId).ToList();

            string errorMsg;
            GetMonthOutcomes(periodId, factsMonthReps, monthOutcomesPeriod, out errorMsg);
            var year = periodId / 10000;
            var month = (periodId / 100) % 100;
            var day = periodId % 100;

            if (month == 1)
            {
                month = 12;
                year--;
            }
            else
            {
                month--;
            }

            string errorPrevMsg;
            var prevPeriodId = (year * 10000) + (month * 100) + day;
            monthOutcomesPeriod = factsMonthRepRepository.FindAll().Where(x =>
                                    (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                                    x.RefRegions.RefRegionsBridge.ID == regionBridgeId && x.RefYearDayUNV.ID == prevPeriodId).ToList();
            GetMonthOutcomes(prevPeriodId, factsPrevMonthReps, monthOutcomesPeriod, out errorPrevMsg);
            
            return errorMsg;
        }

        private void GetMonthOutcomes(
            int periodId, 
            ICollection<MonthOutcomesModel> factsMonthReps, 
            IEnumerable<F_F_MonthRepOutcomes> monthOutcomes,
            out string errorMsg)
        {
            errorMsg = null;
            var yearStr = (periodId / 10000).ToString();
            var monthStr = ((periodId / 100) % 100).ToString();
            var source = sourcesFO2.FirstOrDefault(x => x.Year.Equals(yearStr) && x.Month.Equals(monthStr));
            var sourceId = (source == null) ? -1 : source.ID;
            var marksMonthRepsSource = ekrMonthRepRepository.FindAll().Where(x => x.SourceID == sourceId);
            var marksMonthReps = new List<D_EKR_MonthRep>
            {
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 211),
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 213),
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 260),
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 340),
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 310),
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 320),
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 330),
                marksMonthRepsSource.FirstOrDefault(x => x.Code == 242)
            };

            foreach (var mark in marksMonthReps)
            {
                if (mark == null)
                {
                    factsMonthReps.Add(new MonthOutcomesModel());
                }
                else
                {
                    var markId = mark.ID;
                    var factRepOrign = monthOutcomes.Where(x => x.RefEKR.ID == markId).ToList();
                    
                    var factRep = new MonthOutcomesModel
                                      {
                                          Fact = 0,
                                          YearPlan = 0,
                                          RefEKRID = markId,
                                          PeriodId = periodId
                                      };
                    if (mark.Code != 260)
                    {
                        foreach (var fact in factRepOrign)
                        {
                            factRep.Fact += fact.Fact ?? 0;
                            factRep.YearPlan += fact.YearPlan ?? 0;
                        }
                    }
                    else
                    {
                        var childMarkss = ekrMonthRepRepository.FindAll().Where(x => x.ParentID == markId);
                        foreach (var childMark in childMarkss)
                        {
                            var childMarkId = childMark.ID;
                            var factRepOrignChild = monthOutcomes.Where(x => x.RefEKR.ID == childMarkId).ToList();
                            foreach (var childFact in factRepOrignChild)
                            {
                                factRep.Fact += childFact.Fact ?? 0;
                                factRep.YearPlan += childFact.YearPlan ?? 0;
                            }
                        }
                    }

                    factsMonthReps.Add(factRep);
                }
            }
        }

        private string GetControlFacts(int periodId, List<F_Marks_PassportMO> controlFacts, D_Regions_Analysis region, int sourceId)
        {
            var controlMarksNames = new List<string> { "2.1.1.1", "2.1.2.1", "2.1.1.2", "2.1.2.2", "2.1.4", "2.1.6", "2.3.1", "2.3.3.3" };

            var controlMarks = new List<D_Marks_PassportMO>();

            foreach (var markName in controlMarksNames)
            {
                var mark = marksPassportRepository.FindAll().FirstOrDefault(x => x.Code.Equals(markName) && x.SourceID == sourceId);
                if (mark == null)
                {
                    return "Показатель {0} не найден".FormatWith(markName);
                }

                controlMarks.Add(mark);
                var fact = factsPassportRepository.FindAll().FirstOrDefault(x =>
                                            x.RefPassportMO.ID == mark.ID &&
                                            x.RefPasRegions.ID == region.ID &&
                                            x.RefPasPeriod.ID == periodId &&
                                            x.RefVar.ID == -1) 
                          ?? new F_Marks_PassportMO { RefPassportMO = mark,  };

                controlFacts.Add(fact);
            }

            return null;
        }

        public class DefectModel
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public decimal? FactPeriodMO { get; set; }

            public decimal? FactPeriodMonthReport { get; set; }

            public decimal? FactPeriodDefect { get; set; }

            public decimal? PlanPeriodMO { get; set; }

            public decimal? PlanPriodMonthReport { get; set; }

            public decimal? PlanPeriodDefect { get; set; }
        }

        public class DefectsListModel
        {
            public List<DefectModel> DefectsList { get; set; }

            public bool IsCorrect { get; set; }

            public string ErrorMsg { get; set; }
        }

        private class MonthOutcomesModel
        {
            public decimal? Fact { get; set; }

            public decimal? YearPlan { get; set; }

            public int RefEKRID { get; set; }

            public int PeriodId { get; set; }
        }
    }
}
