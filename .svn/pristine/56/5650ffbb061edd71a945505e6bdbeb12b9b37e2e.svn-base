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
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers.HMAO
{
    [ControllerSessionState(ControllerSessionState.Required)]
    public class FO41HMAOIndicatorsController : SchemeBoundController
    {
        private readonly IFactsService factsRepository;
        private readonly IAppPrivilegeService requestRepository;
        private readonly IIndicatorService indicatorsRepository;
        private readonly IRepository<T_Marks_Detail> detailMarksRepository;
        private readonly IFO41Extension extension;
        private const int PercentCode = 744;

        /// <summary>
        /// Показатели с детализацией (коды)
        /// 1. (код 53) Из нее среднегодовая (средняя) стоимость не облагаемого налогом имущества в связи с реализацией инвестиционных проектов
        /// 2. (код 512) Сумма льготы, установленной ст. 381 НК РФ, по льготам,предоставляемым в соответствии со ст. 7 НК РФ международными договорами РФ и в соответствии с п. 7 ст. 346.35 НК РФ инвесторам по СРП
        /// 3. (код 513) Сумма льготы, установленной ст. 4 Закона автономного округа от 29.11.2010 №190 - оз, в том числе в соответствии с
        /// </summary>
        private static readonly int[] MarksWithDetailCodes = new[] { 53, 512, 513 };

        public FO41HMAOIndicatorsController(
            IFO41Extension extension,
            IFactsService factsRepository, 
            IAppPrivilegeService requestRepository, 
            IIndicatorService indicatorsRepository, 
            IRepository<T_Marks_Detail> detailMarksRepository)
        {
            this.extension = extension;
            this.factsRepository = factsRepository;
            this.requestRepository = requestRepository;
            this.indicatorsRepository = indicatorsRepository;
            this.detailMarksRepository = detailMarksRepository;
        }
        
        public static string GetOkeiName(int id, string designation)
        {
            switch (id)
            {
                case -1:
                case 0:
                    return string.Empty;
                case 65:
                    return "тыс. руб.";
            }

            return designation;
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
        public RestResult Read(int applicationId, int taxTypeId)
        {
            try
            {
                var data = from f in factsRepository.GetFactsForOrganizationHMAO(applicationId, taxTypeId)
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
                                          OKEIName = GetOkeiName(f.RefMarks.RefOKEI.ID, f.RefMarks.RefOKEI.Designation),
                                          IsFormula = false, 
                                          PrevFactFormula = string.Empty,
                                          FactFormula = string.Empty,
                                          EstimateFormula = string.Empty,
                                          ForecastFormula = string.Empty,
                                          HasDetail = MarksWithDetailCodes.Contains(f.RefMarks.Code),
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

        [AcceptVerbs(HttpVerbs.Get)]
        [Transaction]
        public RestResult ReadByTax(int taxTypeId, int periodId)
        {
            var data = factsRepository.GetResultFormByTax(taxTypeId, periodId);
            return new RestResult { Success = true, Data = data };
        }

        /// <summary>
        /// Возвращает суммарные значения показателей для заявок от налогоплательщиков, включенных в заявку от ОГВ
        /// </summary>
        /// <returns>Показатели для ОГВ</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        [Transaction]
        public RestResult ReadByCategory(int categoryId, int taxTypeId, int periodID)
        {
            try
            {
                var indicators = new List<IndicatorsModel>();

                // получили список значений показателей по включенным заявкам налогоплательщиков
                // отсортированный по показателям
                var data = factsRepository.GetFactsForOgvHMAO(categoryId, taxTypeId, periodID);

                var cntOrgs = requestRepository.FindAll().Count(x => 
                    x.RefOrgCategory.ID == categoryId &&
                    x.RefYearDayUNV.ID == periodID && 
                    x.RefStateOrg.ID != 1);

                if (data.Count() > 0)
                {
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
                            // для показателя в процентах считаем среднее значение
                            if (curIndicatorFacts.OKEI == PercentCode && cntOrgs != 0)
                            {
                                curIndicatorFacts.Fact /= cntOrgs;
                                curIndicatorFacts.PreviousFact /= cntOrgs;
                            }

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

                        if (f.RefApplication.RefStateOrg != null && f.RefApplication.RefStateOrg.ID != 1)
                        {
                            // Суммируем значения по заявкам
                            curIndicatorFacts.Fact += f.Fact ?? 0;
                            curIndicatorFacts.PreviousFact += f.PreviousFact ?? 0;
                        }

                        curIndicatorFacts.RowType = f.RefMarks.RowType;
                        curIndicatorFacts.RefMarks = f.RefMarks.ID;
                        curIndicatorFacts.Symbol = f.RefMarks.Symbol;
                        curIndicatorFacts.RefName = f.RefMarks.Name;
                        curIndicatorFacts.RefMarksCode = f.RefMarks.Code;
                        curIndicatorFacts.RefNumberString = f.RefMarks.NumberString;
                        curIndicatorFacts.OKEI = f.RefMarks.RefOKEI != null ? f.RefMarks.RefOKEI.Code : 0;
                        curIndicatorFacts.OKEIName = GetOkeiName(f.RefMarks.RefOKEI != null ? f.RefMarks.RefOKEI.ID : 0, f.RefMarks.RefOKEI != null ? f.RefMarks.RefOKEI.Designation : String.Empty);
                    }

                    // для показателя в процентах считаем среднее значение
                    if (curIndicatorFacts.OKEI == PercentCode && cntOrgs != 0)
                    {
                        curIndicatorFacts.Fact /= cntOrgs;
                        curIndicatorFacts.PreviousFact /= cntOrgs;
                    }

                    indicators.Add(curIndicatorFacts);
                }

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
                else
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
                    else
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

        /// <summary>
        /// Обработчик на открытие формы для добавления новой организации
        /// </summary>
        /// <returns>Представление с формой добавления новой  организации</returns>
        public ActionResult ViewDetails(int markId, int categoryId, int periodId)
        {
            var mark = indicatorsRepository.GetQueryOne(markId);
            var viewDetailsForm = new HMAOMarkDetailsView(extension, mark.Name, markId, categoryId, periodId, mark.RefOKEI == null ? 0 : mark.RefOKEI.Code);

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
                record = id > 0 ? factsRepository.FindOne(id) : new F_Marks_DataPrivilege();
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

            record.SourceID = extension.DataSource(applicationYear).ID;
            record.SourceKey = 0;
            record.TaskID = 0;
            record.PumpID = -1;
            record.RefApplication = requestRepository.Get(applicationId);
            record.RefMarks = indicatorsRepository.GetQueryOne(Convert.ToInt32(dataSet["RefMarks"]));

            factsRepository.Save(record);

            // Если справочная информация - сохранять ее для всех заявок данной организации в текущем периоде
            if (record.RefMarks.RefTypeMark.ID == 5)
            {
                var orgId = record.RefApplication.RefOrgPrivilege.ID;
                var periodId = record.RefApplication.RefYearDayUNV.ID;
                var markId = record.RefMarks.ID;
                var facts = factsRepository.FindAll().Where(x =>
                                               x.RefApplication.RefOrgPrivilege.ID == orgId &&
                                               x.RefApplication.RefYearDayUNV.ID == periodId &&
                                               x.RefMarks.ID == markId).ToList();
                foreach (var fact in facts)
                {
                    fact.Fact = record.Fact;
                    fact.PreviousFact = fact.PreviousFact;
                    factsRepository.Save(fact);
                }
            }

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
            object value;

            if (!dataSet.ContainsKey("PreviousFact"))
            {
                record.PreviousFact = null;
            }
            else
            {
                value = dataSet["PreviousFact"];
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    record.PreviousFact = null;
                }
                else
                {
                    record.PreviousFact = Convert.ToDecimal(value);
                }
            }

            if (!dataSet.ContainsKey("Fact"))
            {
                record.Fact = null;
            }
            else
            {
                value = dataSet["Fact"];
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    record.Fact = null;
                }
                else
                {
                    record.Fact = Convert.ToDecimal(value);
                }
            }

            if (!dataSet.ContainsKey("Estimate"))
            {
                record.Estimate = null;
            }
            else
            {
                value = dataSet["Estimate"];
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    record.Estimate = null;
                }
                else
                {
                    record.Estimate = Convert.ToDecimal(value);
                }
            }

            if (!dataSet.ContainsKey("Forecast"))
            {
                record.Forecast = null;
            }
            else
            {
                value = dataSet["Forecast"];
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

            value = dataSet["TempID"];
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                var id = Convert.ToInt32(value);
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
    }
}
