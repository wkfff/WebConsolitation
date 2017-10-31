using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Helpers;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers.HMAO
{
    public class FO41HMAOEstimateController : SchemeBoundController
    {
        private readonly IFO41Extension extension;
        private readonly ILinqRepository<F_Marks_Privilege> factsEstimateRepository;
        private readonly IFactsService factsRepository;
        private readonly ICategoryTaxpayerService categoryRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly FO41EstimateCalculator calculator;
        private readonly IIndicatorService indicatorsRepository;
        private readonly FormulaService formulasRepository;
        private readonly IAppPrivilegeService requestRepository;

        private const int PercentCode = 744;

        public FO41HMAOEstimateController(
            IFO41Extension extension,
            ILinqRepository<F_Marks_Privilege> factsEstimateRepository,
            IFactsService factsRepository, 
            ICategoryTaxpayerService categoryRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository,
            FO41EstimateCalculator calculator, 
            IIndicatorService indicatorsRepository, 
            FormulaService formulasRepository,
            IAppPrivilegeService requestRepository)
        {
            this.extension = extension;
            this.periodRepository = periodRepository;
            this.factsRepository = factsRepository;
            this.formulasRepository = formulasRepository;
            this.requestRepository = requestRepository;
            this.indicatorsRepository = indicatorsRepository;
            this.factsEstimateRepository = factsEstimateRepository;
            this.categoryRepository = categoryRepository;
            this.categoryRepository = categoryRepository;
            this.calculator = calculator;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Transaction]
        public RestResult Read(int categoryId, int periodId, int taxTypeId)
        {
            try
            {
                IEnumerable<IndicatorsModel> data = factsRepository.GetEstimateDataHMAO(periodId, categoryId, taxTypeId);

                return new RestResult { Success = true, Data = data };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [Transaction]
        public AjaxStoreResult SaveResultData(int categoryId, int periodId, int taxTypeId)
        {
            var result = new AjaxStoreResult();

            try
            {
                // получили список значений показателей по включенным заявкам налогоплательщиков
                // отсортированный по показателям
                var data = factsRepository.GetFactsForOgvHMAO(categoryId, taxTypeId, periodId);

                // код источника
                var sourceId = extension.DataSource(periodId / 10000).ID;

                var cntOrgs = requestRepository.FindAll().Count(x =>
                   x.RefOrgCategory.ID == categoryId &&
                   x.RefYearDayUNV.ID == periodId &&
                   x.RefStateOrg.ID != 1);

                // новый факт
                var curIndicatorFacts = GetEmptyFact(data.First().RefMarks, categoryId, periodId, sourceId);

                foreach (var f in data)
                {
                    // если перешли к фактам по другому показателю, сохраняем накопленные факты, создаем новые
                    if (f.RefMarks.ID != curIndicatorFacts.RefMarks.ID)
                    {
                        // для показателя в процентах считаем среднее значение
                        if (curIndicatorFacts.RefMarks.RefOKEI.Code == PercentCode && cntOrgs != 0)
                        {
                            curIndicatorFacts.Fact /= cntOrgs;
                            curIndicatorFacts.PreviousFact /= cntOrgs;
                        }

                        factsEstimateRepository.Save(curIndicatorFacts);
                        curIndicatorFacts = GetEmptyFact(f.RefMarks, categoryId, periodId, sourceId);
                    }

                    curIndicatorFacts.Fact += f.Fact ?? 0;
                    curIndicatorFacts.PreviousFact += f.PreviousFact ?? 0;
                    curIndicatorFacts.Estimate += f.Estimate ?? 0;
                    curIndicatorFacts.Forecast += f.Forecast ?? 0;
                }

                // для показателя в процентах считаем среднее значение
                if (curIndicatorFacts.RefMarks.RefOKEI.Code == PercentCode && cntOrgs != 0)
                {
                    curIndicatorFacts.Fact /= cntOrgs;
                    curIndicatorFacts.PreviousFact /= cntOrgs;
                }

                factsEstimateRepository.Save(curIndicatorFacts);
            }
            catch (Exception e)
            {
                result.SaveResponse.Success = false;
                result.SaveResponse.Message = e.Message;
                result.ResponseFormat = StoreResponseFormat.Save;
                return result;
            }

            result.SaveResponse.Success = true;
            result.SaveResponse.Message = "Обобщенная форма сохранена";
            result.ResponseFormat = StoreResponseFormat.Save;
            return result;
        }

        [Transaction]
        public void CalculateIndicators(int categoryId, int periodId, int taxTypeId)
        {
            var marks = indicatorsRepository.FindAll().Where(f =>
                f.RefTypeMark.Code == 4 &&
                (f.RefTypeTax.ID == taxTypeId || f.RefTypeTax == null || f.RefTypeTax.ID == -1)).ToList();

            var sourceId = extension.DataSource(periodId / 10000).ID;
            var category = categoryRepository.GetQueryOne(categoryId);
            var period = periodRepository.Get(periodId);
            foreach (var mark in marks)
            {
                if (!formulasRepository.GetForIndicator(mark.ID, "Fact").IsNotNullOrEmpty())
                {
                    continue;
                }

                var record = new F_Marks_Privilege
                {
                    PreviousFact = null,
                    Fact = null,
                    Estimate = null,
                    Forecast = null,
                    SourceID = sourceId,
                    SourceKey = 0,
                    TaskID = 0,
                    PumpID = -1,
                    RefCategory = category,
                    RefMarks = mark,
                    RefYearDayUNV = period
                };

                calculator.Calc(new List<F_Marks_Privilege> { record }, categoryId, periodId, taxTypeId, false);
            }
        }

        private F_Marks_Privilege GetEmptyFact(D_Marks_Privilege indicator, int categoryId, int periodId, int sourceId)
        {
            var facts = factsEstimateRepository.FindAll().Where(
                    f => f.RefMarks.ID == indicator.ID
                    && f.RefCategory.ID == categoryId
                    && f.RefYearDayUNV.ID == periodId
                    && f.SourceID == sourceId).ToList();

           var fact = (facts.Count > 0)
                       ? facts.First()
                       : new F_Marks_Privilege
                       {
                           Fact = 0,
                           PreviousFact = 0,
                           Estimate = 0,
                           Forecast = 0,
                           RefCategory = categoryRepository.GetQueryOne(categoryId),
                           RefMarks = indicator,
                           RefYearDayUNV = periodRepository.Get(periodId),
                           SourceID = sourceId
                       };

           fact.Fact = 0;
           fact.PreviousFact = 0;
           fact.Estimate = 0;
           fact.Forecast = 0;

            return fact;
        }
    }
}
