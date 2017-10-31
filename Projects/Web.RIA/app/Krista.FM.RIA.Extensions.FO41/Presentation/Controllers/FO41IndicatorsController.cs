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
using Krista.FM.RIA.Extensions.FO41.Presentation.Views;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41IndicatorsController : SchemeBoundController
    {
        private readonly FactsService factsRepository;
        private readonly FormulaService formulasRepository;
        private readonly AppPrivilegeService requestRepository;
        private readonly IIndicatorService indicatorsRepository;
        private readonly IRepository<T_Marks_Detail> detailMarksRepository;
        private readonly FO41Calculator calculator;
        private readonly IFO41Extension extension;

        public FO41IndicatorsController(
            IFO41Extension extension,
            FactsService factsRepository, 
            FormulaService formulasRepository, 
            AppPrivilegeService requestRepository, 
            IndicatorService indicatorsRepository, 
            IRepository<T_Marks_Detail> detailMarksRepository,
            FO41Calculator calculator)
        {
            this.extension = extension;
            this.factsRepository = factsRepository;
            this.formulasRepository = formulasRepository;
            this.requestRepository = requestRepository;
            this.indicatorsRepository = indicatorsRepository;
            this.detailMarksRepository = detailMarksRepository;
            this.calculator = calculator;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Create(int applicationId, string data)
        {
            try
            {
                var dataSet = JSON.Deserialize<JsonObject>(data);
                return Create(applicationId, dataSet);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Transaction]
        public RestResult Read(int applicationId)
        {
            // если заявка существующая, рассчитываем показатели по формулам
            CalculateIndicators(applicationId);

            try
            {
                var data = from f in factsRepository.GetFactsForOrganization(applicationId)
                           select new IndicatorsModel
                                      {
                                          TempID = f.ID > 0 ? f.ID : -f.RefMarks.ID,
                                          Id = f.ID,
                                          RefApplication = f.RefApplication.ID,
                                          Fact = f.Fact,
                                          PreviousFact = f.PreviousFact,
                                          RowType = f.RefMarks.RowType,
                                          Estimate = f.Estimate,
                                          Forecast = f.Forecast,
                                          RefMarks = f.RefMarks.ID,
                                          RefMarksCode = f.RefMarks.Code,
                                          Symbol = f.RefMarks.Symbol,
                                          RefName = f.RefMarks.Name,
                                          RefNumberString = f.RefMarks.NumberString,
                                          OKEI = f.RefMarks.RefOKEI.Code,
                                          IsFormula =
                                          f.RefMarks.Formula != null && !f.RefMarks.Formula.Equals(string.Empty),
                                          PrevFactFormula = 
                                            formulasRepository.GetForIndicator(f.RefMarks.ID, "PreviousFact"),
                                          FactFormula = formulasRepository.GetForIndicator(f.RefMarks.ID, "Fact"),
                                          EstimateFormula = 
                                            formulasRepository.GetForIndicator(f.RefMarks.ID, "Estimate"),
                                          ForecastFormula = 
                                            formulasRepository.GetForIndicator(f.RefMarks.ID, "Forecast"),
                                          HasDetail = f.RefMarks.ID == 82,
                                          DetailMark = 0
                                    };

                var detailData = from f in factsRepository.GetDetaiFactsForOrg(applicationId)
                                 select new IndicatorsModel
                                 {
                                     TempID = f.ID > 0 ? f.ID : -f.RefMarks.ID,
                                     Id = f.ID,
                                     RefApplication = f.RefApplicOrg.ID,
                                     Fact = f.Fact,
                                     PreviousFact = f.PreviousFact,
                                     RowType = f.RefMarks.RowType,
                                     Estimate = f.Estimate,
                                     Forecast = f.Forecast,
                                     RefMarks = f.RefMarks.ID,
                                     RefMarksCode = f.RefMarks.Code,
                                     Symbol = f.RefMarks.Symbol,
                                     RefName = "- {0}".FormatWith(f.Name),
                                     RefNumberString = string.Empty,
                                     OKEI = f.RefMarks.RefOKEI.Code,
                                     IsFormula = false,
                                     HasDetail = false,
                                     DetailMark = f.RefMarks.ID
                                 };

                var resultData = data.ToList();

                foreach (var detailFact in detailData)
                {
                    if (detailFact != null)
                    {
                        var index = resultData.FindLastIndex(f => f.RefMarksCode <= detailFact.RefMarksCode);
                        resultData.Insert(index + 1, detailFact);
                    }
                }
                
                return new RestResult { Success = true, Data = resultData };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Возвращает суммарные значения показателей для заявок от налогоплательщиков, включенных в заявку от ОГВ
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от ОГВ</param>
        /// <returns>Показатели для ОГВ</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        [Transaction]
        public RestResult ReadEstimate(int applicationId)
        {
            try
            {
                var indicators = new List<IndicatorsModel>();

                // получили список значений показателей по включенным заявкам налогоплательщиков
                // отсортированный по показателям
                var data = factsRepository.GetFactsForOGV(applicationId);

                // код показателя
                var curIndicatorFacts = new IndicatorsModel
                                                {
                                                    Fact = 0,
                                                    PreviousFact = 0,
                                                    Estimate = 0,
                                                    Forecast = 0,
                                                    RefMarks = data.First().RefMarks.ID
                                                };
                foreach (var f in data)
                {
                    // если перешли к фактам по другому показателю, сохраняем накопленные факты, создаем новые
                    if (f.RefMarks.ID != curIndicatorFacts.RefMarks)
                    {
                        indicators.Add(curIndicatorFacts);
                        curIndicatorFacts = new IndicatorsModel
                                                {
                                                    Fact = 0,
                                                    PreviousFact = 0,
                                                    Estimate = 0,
                                                    Forecast = 0
                                                };
                    }

                    curIndicatorFacts.TempID = f.ID;
                    curIndicatorFacts.RefApplication = f.RefApplication.ID;
                    curIndicatorFacts.Fact += f.Fact ?? 0;
                    curIndicatorFacts.PreviousFact += f.PreviousFact ?? 0;
                    curIndicatorFacts.RowType = f.RefMarks.RowType;
                    curIndicatorFacts.Estimate += f.Estimate ?? 0;
                    curIndicatorFacts.Forecast += f.Forecast ?? 0;
                    curIndicatorFacts.RefMarks = f.RefMarks.ID;
                    curIndicatorFacts.Symbol = f.RefMarks.Symbol;
                    curIndicatorFacts.RefName = f.RefMarks.Name;
                    curIndicatorFacts.RefNumberString = f.RefMarks.NumberString;
                    curIndicatorFacts.OKEI = f.RefMarks.RefOKEI != null ? f.RefMarks.RefOKEI.Code : 0;
                    curIndicatorFacts.IsFormula =
                        f.RefMarks.Formula != null && !f.RefMarks.Formula.Equals(string.Empty);
                    curIndicatorFacts.PrevFactFormula = 
                        formulasRepository.GetForIndicator(f.RefMarks.ID, "PreviousFact");
                    curIndicatorFacts.FactFormula = formulasRepository.GetForIndicator(f.RefMarks.ID, "Fact");
                    curIndicatorFacts.EstimateFormula = 
                        formulasRepository.GetForIndicator(f.RefMarks.ID, "Estimate");
                    curIndicatorFacts.ForecastFormula = 
                        formulasRepository.GetForIndicator(f.RefMarks.ID, "Forecast");
                }

                indicators.Add(curIndicatorFacts);

                return new RestResult { Success = true, Data = indicators };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [Transaction]
        public ActionResult Save(int applicationId)
        {
            try
            {
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        Create(applicationId, record);
                    }
                }

                if (dataSet.ContainsKey("Deleted"))
                {
                    var table = dataSet["Deleted"];
                    foreach (var record in table)
                    {
                        object idObj;
                        record.TryGetValue("TempID", out idObj);
                        if (idObj != null)
                        {
                            var id = Int32.Parse(idObj.ToString());
                            if (id > 0)
                            {
                                detailMarksRepository.Delete(detailMarksRepository.Get(id));
                            }
                        }
                    }
                }

                if (dataSet.ContainsKey("Created"))
                {
                    var table = dataSet["Created"];
                    foreach (var record in table)
                    {
                        Create(applicationId, record);
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Показатели обновлены",
                    Data = dataSet.ToString()
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public ActionResult ViewDetails(int markId, int applicOGVId)
        {
            var mark = indicatorsRepository.GetQueryOne(markId);
            var viewDetailsForm = new MarkDetailsView(extension, mark.Name, markId, applicOGVId, mark.RefOKEI == null ? 0 : mark.RefOKEI.Code);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", viewDetailsForm);
        }

        private RestResult Create(int applicationId, Dictionary<string, object> dataSet)
        {
            var value = dataSet["DetailMark"];

            if (value != null && Convert.ToInt32(value) > 0)
            {
                return CreateDetailMark(applicationId, dataSet, Convert.ToInt32(value));
            }

            F_Marks_DataPrivilege record;
            value = dataSet["TempID"];
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                var id = Convert.ToInt32(value);
                record = id > 0 ? factsRepository.Get(id) : new F_Marks_DataPrivilege();
            }
            else
            {
                record = new F_Marks_DataPrivilege();
            }

            value = dataSet["PreviousFact"];
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                record.PreviousFact = null;
            }
            else
            {
                record.PreviousFact = Convert.ToDecimal(value);
            }

            value = dataSet["Fact"];
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                record.Fact = null;
            }
            else
            {
                record.Fact = Convert.ToDecimal(value);
            }

            value = dataSet["Estimate"];
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                record.Estimate = null;
            }
            else
            {
                record.Estimate = Convert.ToDecimal(value);
            }

            value = dataSet["Forecast"];
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                record.Forecast = null;
            }
            else
            {
                record.Forecast = Convert.ToDecimal(value);
            }

            var applicationYear = requestRepository.GetYear(applicationId);

            var source = extension.DataSource(applicationYear);
            record.SourceID = source == null ? -1 : source.ID;
            record.SourceKey = 0;
            record.TaskID = 0;
            record.PumpID = -1;
            record.RefApplication = requestRepository.Get(applicationId);
            record.RefMarks = indicatorsRepository.GetQueryOne(Convert.ToInt32(dataSet["RefMarks"]));

            factsRepository.Save(record);

            return new RestResult
            {
                Success = true,
                Message = "Показатели добавлены",
                Data = new
                {
                    Id = record.ID
                }
            };
        }

        private RestResult CreateDetailMark(int applicationId, Dictionary<string, object> dataSet, int markId)
        {
            var record = new T_Marks_Detail();

            if (dataSet.ContainsKey("PreviousFact"))
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

            if (dataSet.ContainsKey("Fact"))
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

            if (dataSet.ContainsKey("Estimate"))
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

            if (dataSet.ContainsKey("Forecast"))
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

            record.Name = dataSet["RefName"].ToString();

            record.RefApplicOrg = requestRepository.Get(applicationId);
            record.RefMarks = indicatorsRepository.GetQueryOne(markId);

            var tempID = dataSet["TempID"];
            if (tempID != null && !String.IsNullOrEmpty(tempID.ToString()))
            {
                var id = Convert.ToInt32(tempID);
                if (id > 0)
                {
                    record.ID = id;
                }
            }

            detailMarksRepository.Save(record);

            return new RestResult
            {
                Success = true,
                Message = "Показатели деатлизации добавлены",
                Data = new
                {
                    Id = record.ID,
                    TempID = record.ID > 0 ? record.ID : -record.RefMarks.ID,
                    RefApplication = record.RefApplicOrg.ID,
                    record.Fact,
                    record.PreviousFact,
                    record.RefMarks.RowType,
                    record.Estimate,
                    record.Forecast,
                    RefMarks = record.RefMarks.ID,
                    record.RefMarks.Name,
                    record.RefMarks.Symbol,
                    RefName = record.Name,
                    RefNumberString = string.Empty,
                    OKEI = record.RefMarks.RefOKEI.Code,
                    IsFormula = false,
                    DetailMark = markId
                }
            };
        }

        private void CalculateIndicators(int applicationId)
        {
            if (applicationId > -1)
            {
                var marks = indicatorsRepository.FindAll().Where(f => f.RefTypeMark.Code == 1).ToList();
                var applicationYear = requestRepository.GetYear(applicationId);

                foreach (var mark in marks)
                {
                    if (!formulasRepository.GetForIndicator(mark.ID, "Fact").IsNotNullOrEmpty())
                    {
                        continue;
                    }

                    var record = new F_Marks_DataPrivilege
                    {
                        PreviousFact = null,
                        Fact = null,
                        Estimate = null,
                        Forecast = null,
                        SourceID = extension.DataSource(applicationYear).ID,
                        SourceKey = 0,
                        TaskID = 0,
                        PumpID = -1,
                        RefApplication = requestRepository.Get(applicationId),
                        RefMarks = mark
                    };

                    calculator.Calc(new List<F_Marks_DataPrivilege> { record }, record.RefApplication.ID, false);
                }
            }
        }
    }
}
