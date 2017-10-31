using System;
using System.Collections.Generic;
using System.Linq;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public class FactsByMesOtchService : IFactsByMesOtchService
    {
        private readonly ILinqRepository<F_F_MonthRepOutcomes> factsMonthRepRepository;
        private readonly ILinqRepository<DataSources> sourcesRepository;
        private readonly ILinqRepository<F_Marks_PassportMO> factsPassportRepository;
        private readonly IRepository<D_Regions_Analysis> regionRepository;
        private readonly IRepository<FX_State_PassportMO> stateRepository;
        private readonly IRepository<D_PMO_Variant> variantRepository;
        private readonly ILinqRepository<D_EKR_MonthRep> ekrMonthRepRepository;
        private readonly ILinqRepository<F_F_MonthRepInFin> monthRepInFinRepository;
        private int dataCreatedState;

        public FactsByMesOtchService(
            IFO51Extension extension,
            ILinqRepository<F_Marks_PassportMO> factsPassportRepository,
            ILinqRepository<F_F_MonthRepOutcomes> factsMonthRepRepository,
            ILinqRepository<DataSources> sourcesRepository, 
            IRepository<D_Regions_Analysis> regionRepository, 
            IRepository<FX_State_PassportMO> stateRepository, 
            IRepository<D_PMO_Variant> variantRepository,
            ILinqRepository<D_EKR_MonthRep> ekrMonthRepRepository,
            ILinqRepository<F_F_MonthRepInFin> monthRepInFinRepository)
        {
            this.factsPassportRepository = factsPassportRepository;
            this.variantRepository = variantRepository;
            this.ekrMonthRepRepository = ekrMonthRepRepository;
            this.monthRepInFinRepository = monthRepInFinRepository;
            this.stateRepository = stateRepository;
            this.regionRepository = regionRepository;
            this.factsMonthRepRepository = factsMonthRepRepository;
            this.sourcesRepository = sourcesRepository;
            dataCreatedState = extension.UserGroup.Equals(FO51Extension.GroupMo) ? FO51Extension.StateEdit : FO51Extension.StateConsider;
        }

        public void SaveMonthReportData(D_Regions_Analysis region, FX_Date_YearDayUNV period, IList<F_Marks_PassportMO> facts, IEnumerable<D_Marks_PassportMO> marksAll, int sourceId, FX_MeansType_SKIF defaultMeansType, FX_BdgtLevels_SKIF defaultBdgt)
        {
            var regionBridgeId = region.RefBridgeRegions.ID;
            var year = period.ID / 10000;
            var month = (period.ID / 100) % 100;
            var sourcesFO2 = sourcesRepository.FindAll().FirstOrDefault(x =>
                    x.SupplierCode.Equals("ФО") &&
                    x.DataCode == 2 &&
                    x.Year.Equals(year.ToString()) &&
                    x.Month.Equals(month.ToString()));
            if (sourcesFO2 == null)
            {
                throw new Exception("Источник ФО 2 'Ежемесячный отчет' на {0} год не найден. Обратитесь к администратору".FormatWith(year));
            }

            DataSources sourcesFO2Prev;
            if (month > 1)
            {
                sourcesFO2Prev = sourcesRepository.FindAll().FirstOrDefault(x =>
                                                                               x.SupplierCode.Equals("ФО")
                                                                            && x.DataCode == 2
                                                                            && x.Year.Equals(year.ToString())
                                                                            && x.Month.Equals((month - 1).ToString()));
            }
            else
            {
                sourcesFO2Prev = sourcesRepository.FindAll().FirstOrDefault(x =>
                                                                               x.SupplierCode.Equals("ФО")
                                                                            && x.DataCode == 2
                                                                            && x.Year.Equals((year - 1).ToString())
                                                                            && x.Month.Equals("12"));
            }

            if (sourcesFO2Prev == null)
            {
                throw new Exception("Источник ФО 2 'Ежемесячный отчет' на {0} год не найден. Обратитесь к администратору".FormatWith(year - 1));
            }

            /* В разделе «Расходы», при выборе данных из МесОтч, 
             * для показателя «Расходы на обслуживание долга (КОСГУ 230)» 
             * необходимо исключать данные по уровню бюджета «Бюджет поселения» 
             * (классификатор «Уровни бюджета.СКИФ» (fx_BdgtLevels_SKIF)), 
             * если в таблице фактов «ФО_МесОтч_ИсточникВнутрФинансирования» (f_F_MonthRepInFin)
             * есть данные по показателю «Погашение бюджетами поселений кредитов 
             * от других бюджетов бюджетной системы Российской Федерации 
             * в валюте Российской Федерации» (классификатор «КИВнутрФ.МесОтч» (d. SIF.MonthRep))
             * (назовем его markRepaymentOfLoansCode)
             * на уровне «Бюджет поселения» (6). */
            var markRepaymentOfLoansCode = "00001030000100000810";
            var useLevelBudgetSettlement = !monthRepInFinRepository.FindAll().Any(x =>
                x.RefBdgtLevels.ID == 6 &&
                x.RefRegions.RefRegionsBridge != null &&
                x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                (x.RefYearDayUNV.ID == period.ID) &&
                x.RefSIF.CodeStr.Equals(markRepaymentOfLoansCode) &&
                x.SourceID == sourcesFO2.ID);

            // Уровень бюджета СКИФ («RefBdgtLevels»): элементы «Бюджет гор.округа», «Бюджет района» и, если нужно «Бюджет поселения».
            var monthOutcomesCurSkifAll = factsMonthRepRepository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == period.ID) &&
                       x.SourceID == sourcesFO2.ID)
                .ToList();

            var monthOutcomesPrevSkifAll = factsMonthRepRepository.FindAll().Where(x =>
                (x.RefBdgtLevels.ID == 4 || x.RefBdgtLevels.ID == 5 || x.RefBdgtLevels.ID == 6) &&
                       x.RefRegions.RefRegionsBridge != null &&
                       x.RefRegions.RefRegionsBridge.ID == regionBridgeId &&
                       (x.RefYearDayUNV.ID == period.ID - 100) &&
                       x.SourceID == sourcesFO2Prev.ID)
                .ToList();

            var monthOutcomesCurSkifGoReg = useLevelBudgetSettlement 
                ? monthOutcomesCurSkifAll 
                : monthOutcomesCurSkifAll.Where(x => x.RefBdgtLevels.ID != 6).ToList();

            var monthOutcomesPrevSkifGoReg = useLevelBudgetSettlement
                ? monthOutcomesPrevSkifAll
                : monthOutcomesPrevSkifAll.Where(x => x.RefBdgtLevels.ID != 6).ToList();

            var comporisonMarks = new[]
            {
                /* Оплата коммунальных услуг (КОСГУ 223) (код 2.1.3) 
                 * Коммунальные услуги (код 2.2.3)*/
                new ComporisonModel(223, "2.1.3"),
                /* Прочие выплаты (КОСГУ 212) (код 2.2.1)
                 * Прочие выплаты (код 2.1.2)*/
                new ComporisonModel(212, "2.2.1"),
                /* Расходы на обслуживание долга (КОСГУ 230) (код 2.2.2)
                 * Обслуживание государственного (муниципального) долга (код 2.3.0) */
                new ComporisonModel(230, "2.2.2"),
                /* связь (КОСГУ 221) (код 2.2.3.1)
                 * услуги связи (код 2.2.1) */
                new ComporisonModel(221, "2.2.3.1"),
                /* транспортные услуги (ЭКР 222) (код 2.2.3.2)
                 * транспортные услуги ( код 2.2.2) */
                new ComporisonModel(222, "2.2.3.2"),
                /* арендная плата за пользование имуществом (КОСГУ 224) (код 2.2.3.3)
                 * арендная плата за пользование имуществом (код 2.2.4) */
                new ComporisonModel(224, "2.2.3.3"),
                /* Безвозмездные перечисления государственным и муниципальным организациям (КОСГУ 241) (код 2.3.3.2)
                 * Безвозмездные перечисления государственным и муниципальным организациям (код 2.4.1) */
                new ComporisonModel(241, "2.3.3.2"),
                /* Перечисления международным организациям (КОСГУ 253) (код 2.3.3.5)
                 * Перечисления международным организациям (код 2.5.3) */
                new ComporisonModel(253, "2.3.3.5"),
                /* Увеличение стоимости акций и иных форм участия в капитале (КОСГУ 530) (код 2.3.3.6)
                 * Увеличение стоимости акций и иных форм участия в капитале (код 5.3.0) */
                new ComporisonModel(530, "2.3.3.6"),
                /* Справочно: Перечисления другим бюджетам бюджетной системы РФ (КОСГУ 251) (код 2.999)
                 * Перечисления другим бюджетам бюджетной системы Российской Федерации (код 2.5.1) */
                new ComporisonModel(251, "2.999"),
            };

            for (var i = 0; i < comporisonMarks.Count(); i++)
            {
                SaveFactByMesOtch(
                    region,
                    period,
                    monthOutcomesCurSkifAll,
                    monthOutcomesPrevSkifAll,
                    monthOutcomesCurSkifGoReg,
                    monthOutcomesPrevSkifGoReg,
                    facts,
                    marksAll,
                    sourceId,
                    defaultMeansType,
                    defaultBdgt,
                    comporisonMarks[i].MesOtchMarkCode,
                    comporisonMarks[i].PassportMarkCode);
            }
        }

        private void SaveFactByMesOtch(
            D_Regions_Analysis region,
            FX_Date_YearDayUNV period,
            IEnumerable<F_F_MonthRepOutcomes> monthOutcomesCurAll,
            IEnumerable<F_F_MonthRepOutcomes> monthOutcomesPrevAll,
            IEnumerable<F_F_MonthRepOutcomes> monthOutcomesCurGoReg,
            IEnumerable<F_F_MonthRepOutcomes> monthOutcomesPrevGoReg,
            IList<F_Marks_PassportMO> facts,
            IEnumerable<D_Marks_PassportMO> marksAll,
            int sourceId,
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt,
            int mesOtchCode,
            string markCode)
        {
            var month = (period.ID / 100) % 100;
            IEnumerable<F_F_MonthRepOutcomes> mesOtchDataForFactPeriod;
            IEnumerable<F_F_MonthRepOutcomes> mesOtchPrevDataForFactPeriod;
            if (mesOtchCode == 230)
            {
                GetMesOtchData(period, month, mesOtchCode, monthOutcomesCurGoReg, monthOutcomesPrevGoReg, out mesOtchDataForFactPeriod, out mesOtchPrevDataForFactPeriod);
            }
            else
            {
                GetMesOtchData(period, month, mesOtchCode, monthOutcomesCurAll, monthOutcomesPrevAll, out mesOtchDataForFactPeriod, out mesOtchPrevDataForFactPeriod);
            }

            IEnumerable<F_F_MonthRepOutcomes> mesOtchDataForPlanPeriod;
            IEnumerable<F_F_MonthRepOutcomes> mesOtchPrevDataForPlanPeriod;
            GetMesOtchData(period, month, mesOtchCode, monthOutcomesCurAll, monthOutcomesPrevAll, out mesOtchDataForPlanPeriod, out mesOtchPrevDataForPlanPeriod);

            var isNew = false;
            var factPassportMO = facts.FirstOrDefault(x =>
                                                      x.RefPasPeriod.ID == period.ID &&
                                                      x.RefVar.Code == -1 &&
                                                      x.RefPassportMO.Code == markCode);
            if (factPassportMO == null)
            {
                isNew = true;
                factPassportMO = CreateFactDefault(
                    period,
                    marksAll.FirstOrDefault(x => x.Code == markCode),
                    region,
                    sourceId,
                    defaultMeansType,
                    defaultBdgt);
            }

            factPassportMO.FactPeriod = 0;
            factPassportMO.PlanPeriod = 0;

            // Уточненный план на год
            if (mesOtchDataForPlanPeriod != null && mesOtchDataForPlanPeriod.Count() > 0)
            {
                foreach (var mesOtch in mesOtchDataForPlanPeriod)
                {
                    factPassportMO.PlanPeriod += mesOtch.YearPlan ?? 0;
                }
            }

            // Исполнено за отчетный месяц
            if (mesOtchDataForFactPeriod != null && mesOtchDataForFactPeriod.Count() > 0)
            {
                foreach (var mesOtch in mesOtchDataForFactPeriod)
                {
                    factPassportMO.FactPeriod += mesOtch.Fact ?? 0;
                }
            } 
            
            if (mesOtchPrevDataForFactPeriod != null && month > 1)
            {
                foreach (var mesOtch in mesOtchPrevDataForFactPeriod)
                {
                    factPassportMO.FactPeriod -= mesOtch.Fact ?? 0;
                }
            }

            factsPassportRepository.Save(factPassportMO);
            if (isNew)
            {
                facts.Add(factPassportMO);
            }
        }

        private void GetMesOtchData(FX_Date_YearDayUNV period, int month, int mesOtchCode, IEnumerable<F_F_MonthRepOutcomes> monthOutcomesCurAll, IEnumerable<F_F_MonthRepOutcomes> monthOutcomesPrevAll, out IEnumerable<F_F_MonthRepOutcomes> mesOtchData, out IEnumerable<F_F_MonthRepOutcomes> mesOtchPrevData)
        {
            mesOtchData = null;
            mesOtchPrevData = null;

            if (mesOtchCode == 230)
            {
                /* Расходы на обслуживание долга (КОСГУ 230) - 
                * берем в месОтч подчиненные "Обслуживание государственного (муниципального) долга (код 2.3.0)"
                * - то есть по показателям с кодами 231 и 232 
                */
                var mark230 = ekrMonthRepRepository.FindAll().FirstOrDefault(x => x.Code == mesOtchCode);
                if (mark230 != null)
                {
                    mesOtchData = monthOutcomesCurAll.Where(x => x.RefYearDayUNV.ID == period.ID && (x.RefEKR.Code == 231 || x.RefEKR.Code == 232));
                    mesOtchPrevData = month == 1
                                          ? null
                                          : monthOutcomesPrevAll.Where(x => x.RefYearDayUNV.ID == period.ID - 100 && (x.RefEKR.Code == 231 || x.RefEKR.Code == 232));
                }
            }
            else
            {
                mesOtchData = monthOutcomesCurAll.Where(x => x.RefYearDayUNV.ID == period.ID && x.RefEKR.Code == mesOtchCode);
                mesOtchPrevData = month == 1
                                      ? null
                                      : monthOutcomesPrevAll.Where(x => x.RefYearDayUNV.ID == period.ID - 100 && x.RefEKR.Code == mesOtchCode);
            }
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

        public class ComporisonModel
        {
            public ComporisonModel(int mesOtchMarkCode, string passportMarkCode)
            {
                MesOtchMarkCode = mesOtchMarkCode;
                PassportMarkCode = passportMarkCode;
            }

            public int MesOtchMarkCode { get; set; }

            public string PassportMarkCode { get; set; }
        }
    }
}
