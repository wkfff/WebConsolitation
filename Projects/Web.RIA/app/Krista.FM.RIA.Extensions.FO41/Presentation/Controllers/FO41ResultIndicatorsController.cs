using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Helpers;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41ResultIndicatorsController : SchemeBoundController
    {
        private readonly FactsService factsRepository;

        private readonly IFO41Extension extension;

        private readonly AppFromOGVService appFromOGVRepository;

        private readonly IndicatorService indicatorsRepository;

        private readonly FormulaService formulasRepository;

        private readonly FO41EstimateCalculator calculator;

        private readonly ILinqRepository<F_Marks_Privilege> factsEstimateRepository;

        private readonly ILinqRepository<D_Marks_NormPrivilege> normRepository; 

        public FO41ResultIndicatorsController(
            IFO41Extension extension, 
            FactsService factsRepository, 
            AppFromOGVService appFromOGVRepository,  
            IndicatorService indicatorsRepository,
            FormulaService formulasRepository,
            FO41EstimateCalculator calculator,
            ILinqRepository<F_Marks_Privilege> factsEstimateRepository,
            ILinqRepository<D_Marks_NormPrivilege> normRepository)
        {
            this.extension = extension;
            this.factsRepository = factsRepository;
            this.appFromOGVRepository = appFromOGVRepository;
            this.indicatorsRepository = indicatorsRepository;
            this.formulasRepository = formulasRepository;
            this.calculator = calculator;
            this.factsEstimateRepository = factsEstimateRepository;
            this.normRepository = normRepository;
        }

        public static string GetOkeiName(int id, string designation)
        {
            switch (id)
            {
                case -1:
                    return string.Empty;
                case 58:
                    return "тыс. руб.";
            }

            return designation;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Transaction]
        public RestResult Read(int appFromOGV)
        {
            try
            {
                if (appFromOGVRepository != null)
                {
                    var applicationFromOGV = appFromOGVRepository.Get(appFromOGV);
                    var year = applicationFromOGV.RefYearDayUNV.ID / 10000;
                    var sourceId = extension.DataSource(year).ID;
                    var categoryId = applicationFromOGV.RefOrgCategory.ID;
                    var facts = factsEstimateRepository.FindAll()
                        .Where(f => 
                               f.RefCategory.ID == categoryId && 
                               f.RefYearDayUNV.ID == applicationFromOGV.RefYearDayUNV.ID && 
                               f.SourceID == sourceId && 
                               f.RefMarks.RefTypeMark.ID == 4)
                        .OrderBy(f => f.RefMarks.Code).ToList();
                    
                    var data = from f in facts
                               select new IndicatorsModel
                                          {
                                              PreviousFact = f.PreviousFact,
                                              Fact = f.Fact,
                                              Forecast = f.Forecast,
                                              Estimate = f.Estimate,
                                              RefName = f.RefMarks.Name,
                                              RefNumberString = f.RefMarks.NumberString,
                                              RowType = f.RefMarks.RowType,
                                              RefMarks = f.RefMarks.ID,
                                              OKEI = f.RefMarks.RefOKEI.Code,
                                              OKEIName = GetOkeiName(f.RefMarks.RefOKEI.ID, f.RefMarks.RefOKEI.Designation),
                                              IsFormula = f.RefMarks.Formula != null && !f.RefMarks.Formula.Equals(string.Empty),
                                              Symbol = f.RefMarks.Symbol,
                                              TempID = f.ID > 0 ? f.ID : -f.RefMarks.ID,
                                          };

                    return new RestResult { Success = true, Data = data };
                }
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }

            return new RestResult { Success = false, Message = "Заявка на оценку номер {0} не найдена".FormatWith(appFromOGV) };
        }

        [Transaction]
        public AjaxStoreResult SaveResultData(int appFromOGVId)
        {
            var result = new AjaxStoreResult();

            try
            {
                // получили список значений показателей по включенным заявкам налогоплательщиков
                // отсортированный по показателям
                var data = factsRepository.GetFactsForOGV(appFromOGVId);
                var app = appFromOGVRepository.Get(appFromOGVId);

                // код источника
                var sourceId = extension.DataSource(app.RefYearDayUNV.ID / 10000).ID;
                
                // новый факт
                var curIndicatorFacts = GetFact(app, data.First().RefMarks, sourceId);

                foreach (var f in data)
                {
                    // если перешли к фактам по другому показателю, сохраняем накопленные факты, создаем новые
                    if (f.RefMarks.ID != curIndicatorFacts.RefMarks.ID)
                    {
                        factsEstimateRepository.Save(curIndicatorFacts);
                        var oldFacts = factsEstimateRepository.FindAll().Where(
                            oldFact => oldFact.RefMarks.ID == f.RefMarks.ID
                            && oldFact.RefCategory.ID == app.RefOrgCategory.ID
                            && oldFact.RefYearDayUNV.ID == app.RefYearDayUNV.ID
                            && oldFact.SourceID == sourceId).ToList();
                        foreach (var oldFact in oldFacts)
                        {
                            factsEstimateRepository.Delete(oldFact);
                        }
                        
                        curIndicatorFacts = GetFact(app, f.RefMarks, sourceId);
                    }

                    curIndicatorFacts.Fact += f.Fact ?? 0;
                    curIndicatorFacts.PreviousFact += f.PreviousFact ?? 0;
                    curIndicatorFacts.Estimate += f.Estimate ?? 0;
                    curIndicatorFacts.Forecast += f.Forecast ?? 0;
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
        public RestResult SaveEstimate(string data)
        {
            try
            {
                var dataSet = JSON.Deserialize<JsonObject>(data);

                var factId = Convert.ToInt32(dataSet["TempID"]);
                var record = factsEstimateRepository.FindOne(factId);
                if (!dataSet.Keys.Contains("PreviousFact"))
                {
                    record.PreviousFact = null;
                }
                else
                {
                    var value = dataSet["PreviousFact"];
                    if (value == null || String.IsNullOrEmpty(value.ToString()))
                    {
                        record.PreviousFact = null;
                    }
                    else
                    {
                        record.PreviousFact = Convert.ToDecimal(value);
                    }
                }

                if (!dataSet.Keys.Contains("Fact"))
                {
                    record.PreviousFact = null;
                }
                else
                {
                    var value = dataSet["Fact"];
                    if (value == null || String.IsNullOrEmpty(value.ToString()))
                    {
                        record.Fact = null;
                    }
                    else
                    {
                        record.Fact = Convert.ToDecimal(value);
                    }
                }

                if (!dataSet.Keys.Contains("Estimate"))
                {
                    record.PreviousFact = null;
                }
                else
                {
                    var value = dataSet["Estimate"];
                    if (value == null || String.IsNullOrEmpty(value.ToString()))
                    {
                        record.Estimate = null;
                    }
                    else
                    {
                        record.Estimate = Convert.ToDecimal(value);
                    }
                }

                if (!dataSet.Keys.Contains("Forecast"))
                {
                    record.PreviousFact = null;
                }
                else
                {
                    var value = dataSet["Forecast"];
                    if (value == null || String.IsNullOrEmpty(value.ToString()))
                    {
                        record.Forecast = null;
                    }
                    else
                    {
                        record.Forecast = Convert.ToDecimal(value);
                    }
                }

                factsEstimateRepository.Save(record);

                var okeiName = GetOkeiName(record.RefMarks.RefOKEI.ID, record.RefMarks.RefOKEI.Designation);
                return new RestResult
                           {
                               Success = true,
                               Message = "Показатели оценки сохранены",
                               Data = new
                                          {
                                              record.ID,
                                              TempID = record.ID > 0 ? record.ID : -record.RefMarks.ID,
                                              record.Fact,
                                              record.PreviousFact,
                                              record.RefMarks.RowType,
                                              record.Estimate,
                                              record.Forecast,
                                              RefMarks = record.RefMarks.ID,
                                              record.RefMarks.Symbol,
                                              RefName = record.RefMarks.Name,
                                              RefNumberString = record.RefMarks.NumberString,
                                              OKEI = record.RefMarks.RefOKEI.Code,
                                              OKEIName = okeiName,
                                              IsFormula = record.RefMarks.Formula != null && !record.RefMarks.Formula.Equals(string.Empty),
                                          }
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [Transaction]
        public void SaveNorm(int year, int? prev, int? fact, int? estimate, int? forecast)
        {
            var norm = normRepository.FindAll().FirstOrDefault(f => f.Year == year && f.Symbol.Equals("MIN"));
            if (norm == null)
            {
                norm = new D_Marks_NormPrivilege
                           {
                               Symbol = "MIN",
                               Name = "Величина прожиточного минимума в расчете на душу населения",
                               RowType = 0,
                               PreviousFact = prev == null ? 0 : (decimal)prev,
                               Fact = fact == null ? 0 : (decimal)fact,
                               Estimate = estimate == null ? 0 : (decimal)estimate,
                               Forecast = forecast == null ? 0 : (decimal)forecast,
                               Year = year
                           };
            }
            else
            {
                if (prev != null)
                {
                    norm.PreviousFact = (decimal)prev;
                }

                if (fact != null)
                {
                    norm.Fact = (decimal)fact;
                }

                if (estimate != null)
                {
                    norm.Estimate = (decimal)estimate;
                }

                if (forecast != null)
                {
                    norm.Forecast = (decimal)forecast;
                }
            }

            normRepository.Save(norm);
        }

        [Transaction]
        public void CalculateIndicators(int appFromOGVId)
        {
            if (appFromOGVId <= -1)
            {
                return;
            }

            var marks = indicatorsRepository.FindAll().Where(f => f.RefTypeMark.Code == 4);
            var appFromOGV = appFromOGVRepository.Get(appFromOGVId);
            if (appFromOGV == null)
            {
                return;
            }

            var applicationYear = appFromOGV.RefYearDayUNV.ID / 10000;

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
                                     SourceID = extension.DataSource(applicationYear).ID,
                                     SourceKey = 0,
                                     TaskID = 0,
                                     PumpID = -1,
                                     RefCategory = appFromOGV.RefOrgCategory,
                                     RefMarks = mark,
                                     RefYearDayUNV = appFromOGV.RefYearDayUNV
                                 };

                calculator.Calc(new List<F_Marks_Privilege> { record }, appFromOGV, false);
            }
        }
        
        private F_Marks_Privilege GetFact(D_Application_FromOGV app, D_Marks_Privilege indicator, int sourceId)
        {
            var facts = factsEstimateRepository.FindAll().Where(
                    f => f.RefMarks.ID == indicator.ID
                    && f.RefCategory.ID == app.RefOrgCategory.ID
                    && f.RefYearDayUNV.ID == app.RefYearDayUNV.ID
                    && f.SourceID == sourceId).ToList();

            return (facts.Count > 0)
                       ? facts.First()
                       : new F_Marks_Privilege
                       {
                           Fact = 0,
                           PreviousFact = 0,
                           Estimate = 0,
                           Forecast = 0,
                           RefCategory = app.RefOrgCategory,
                           RefMarks = indicator,
                           RefYearDayUNV = app.RefYearDayUNV,
                           SourceID = sourceId
                       };
        }
    }
}
