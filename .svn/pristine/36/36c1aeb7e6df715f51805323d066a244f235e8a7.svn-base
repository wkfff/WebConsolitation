using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controllers
{
    public class EO15AIPDetailContractController : SchemeBoundController
    {
         private readonly IConstructionService constrRepository;
        private readonly ILinqRepository<F_ExcCosts_AIP> factsAipRepository;
        private readonly ILinqRepository<D_ExcCosts_WorkType> workTypeRepository;
        private readonly ILinqRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly ILinqRepository<D_ExcCosts_Finances> financesRepository;
        private readonly ILinqRepository<D_ExcCosts_StatusD> statusDRepository;
        private readonly ILinqRepository<D_ExcCosts_TypeCont> contTypeRepository;
        private readonly ILinqRepository<D_ExcCosts_AIPMark> markRepository;
        private readonly ILinqRepository<D_ExcCosts_Audits> auditRepository;

        public EO15AIPDetailContractController(
            IEO15ExcCostsAIPExtension extension,
            IConstructionService constrRepository,
            ILinqRepository<F_ExcCosts_AIP> factsAipRepository,
            ILinqRepository<D_ExcCosts_WorkType> workTypeRepository,
            ILinqRepository<FX_Date_YearDayUNV> periodRepository,
            ILinqRepository<D_ExcCosts_AIPMark> markRepository,
            ILinqRepository<D_ExcCosts_Audits> auditRepository,
            ILinqRepository<D_ExcCosts_Finances> financesRepository,
            ILinqRepository<D_ExcCosts_StatusD> statusDRepository,
            ILinqRepository<D_ExcCosts_TypeCont> contTypeRepository)
        {
            this.constrRepository = constrRepository;
            this.factsAipRepository = factsAipRepository;
            this.workTypeRepository = workTypeRepository;
            this.periodRepository = periodRepository;
            this.financesRepository = financesRepository;
            this.statusDRepository = statusDRepository;
            this.contTypeRepository = contTypeRepository;
            this.extension = extension;
            this.markRepository = markRepository;
            this.auditRepository = auditRepository;
        }

        public AjaxStoreResult Read(bool[] filter, int objectId)
        {
            try
            {
                var filters = new List<int>();
                var filtersCnt = filter.Count();
                for (var indexFilter = 0; indexFilter < filtersCnt; indexFilter++)
                {
                    if (!filter[indexFilter])
                    {
                        filters.Add(indexFilter + 1);
                    }
                }

                var curObject = constrRepository.GetOne(objectId);
                if (curObject.StartConstruction == null || curObject.EndConstruction == null)
                {
                    return new AjaxStoreResult
                    {
                        ResponseFormat = StoreResponseFormat.Load,
                        Data = new List<string>(),
                        Total = 0
                    };  
                }

                var data = new List<object>();
                
                var facts = factsAipRepository.FindAll().Where(x => 
                    !filters.Contains(x.RefStatusD.ID) &&
                    x.RefCObject.ID == objectId &&
                    x.SourceID == extension.DataSource.ID &&
                    x.RefContract.ID > 0).ToList();

                var typeContractPeriods = facts.Distinct(new EqualityComparerFactAIP());

                foreach (var tcp in typeContractPeriods)
                {
                    var recordFacts = facts.Where(x => 
                        x.RefPeriod.ID == tcp.RefPeriod.ID &&
                        x.RefTypeWork.ID == tcp.RefTypeWork.ID &&
                        x.RefContract.ID == tcp.RefContract.ID);
                    var factStartPrice = recordFacts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkStartPrice);
                    var factCurPrice = recordFacts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkCurPrice);
                    var factExpectPrice = recordFacts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkExpectPrice);
                    data.Add(new
                                 {
                                     TypeWorkId = tcp.RefTypeWork.ID,
                                     TypeWorkName = tcp.RefTypeWork.Name,
                                     PeriodId = tcp.RefPeriod.ID,
                                     Date = tcp.RefPeriod.ID,
                                     CObjectId = objectId,
                                     CObjectName = curObject.Name,
                                     ContractId = tcp.RefContract.ID,
                                     ContractName = tcp.RefContract.Property,
                                     PartnerId = tcp.RefContract.RefPartners.ID,
                                     PartnerName = tcp.RefContract.RefPartners.Name,
                                     StartPriceId = factStartPrice == null ? 0 : factStartPrice.ID,
                                     StartPrice = factStartPrice == null ? 0 : factStartPrice.Value,
                                     ExpectPriceId = factExpectPrice == null ? 0 : factExpectPrice.ID,
                                     ExpectPrice = factExpectPrice == null ? 0 : factExpectPrice.Value,
                                     PriceId = factCurPrice == null ? 0 : factCurPrice.ID,
                                     Price = factCurPrice == null ? 0 : factCurPrice.Value,
                                     StateId = tcp.RefContract == null ? -1 : tcp.RefContract.RefTypeCont.ID,
                                     StateName = tcp.RefContract == null ? String.Empty : tcp.RefContract.RefTypeCont.Name,
                                     StatusDId = tcp.RefStatusD.ID,
                                     StatusDName = tcp.RefStatusD.Name
                                 });
                }
                
                return new AjaxStoreResult
                {
                    ResponseFormat = StoreResponseFormat.Load,
                    Data = data,
                    Total = data.Count
                };
            }
            catch (Exception)
            {
                return new AjaxStoreResult
                {
                    ResponseFormat = StoreResponseFormat.Load,
                    Data = new List<string>(),
                    Total = 0
                };
            }
        }

        [Transaction]
        public ActionResult Save()
        {
            try
            {
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Deleted"))
                {
                    var table = dataSet["Deleted"];
                    foreach (var record in table)
                    {
                        try
                        {
                            var startPriceId = CommonService.GetIntFromRecord(record, "StartPriceId", "Начальная (максимальная) цена контракта по согласованию РСТ");
                            var fact = factsAipRepository.FindOne(startPriceId);
                            factsAipRepository.Delete(fact);
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            var priceId = CommonService.GetIntFromRecord(record, "PriceId", "Стоимость в текущих ценах по контракту");
                            var fact = factsAipRepository.FindOne(priceId);
                            factsAipRepository.Delete(fact);
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            var expectPriceId = CommonService.GetIntFromRecord(record, "ExpectPriceId", "Ожидаемая стоимость основных фондов");
                            var fact = factsAipRepository.FindOne(expectPriceId);
                            factsAipRepository.Delete(fact);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        var markStartPrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkStartPrice);
                        var markCurPrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkCurPrice);
                        var markExpectPrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkExpectPrice);

                        var factStartPrice = ChangeFact(record, "StartPriceId", "StartPrice", markStartPrice);
                        var factCurPrice = ChangeFact(record, "PriceId", "Price", markCurPrice);
                        var factExpectPrice = ChangeFact(record, "ExpectPriceId", "ExpectPrice", markExpectPrice);

                        var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                        var statusD = statusDRepository.FindOne(statusDId);

                        var factToUse = factStartPrice ?? factCurPrice ?? factExpectPrice;
                        if (factToUse == null)
                        {
                            throw new Exception("Начальная цена контакта, стоимость в текущих ценах и ожидаемая стоимость основных фондов не заданы.");
                        }

                        // изменяем контракт, контрагент
                        var contract = GetContract(record, factToUse);

                        if (factStartPrice != null)
                        {
                            SetPeriodContractTypeWork(record, factStartPrice, contract);
                            factStartPrice.RefStatusD = statusD;
                            factsAipRepository.Save(factStartPrice);
                        }

                        if (factCurPrice != null)
                        {
                            SetPeriodContractTypeWork(record, factCurPrice, contract);
                            factCurPrice.RefStatusD = statusD;
                            factsAipRepository.Save(factCurPrice);
                        }

                        if (factExpectPrice != null)
                        {
                            SetPeriodContractTypeWork(record, factExpectPrice, contract);
                            factExpectPrice.RefStatusD = statusD;
                            factsAipRepository.Save(factExpectPrice);
                        }
                    }
                }

                if (dataSet.ContainsKey("Created"))
                {
                    var markStartPrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkStartPrice);
                    var markCurPrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkCurPrice);
                    var markExpectPrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkExpectPrice);

                    var auditDefault = auditRepository.FindOne(-1);
                    var financesDefault = financesRepository.FindOne(-1);
                    var statusDDefault = statusDRepository.FindOne(-1);
                    var table = dataSet["Created"];
                    foreach (var record in table)
                    {
                        var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                        var statusD = statusDRepository.FindOne(statusDId);

                        // если нет - создать два факта и сохранить их*/
                        var id = CommonService.GetIntFromRecord(record, "CObjectId", "Объект строительства");
                        var factStartPrice = new F_ExcCosts_AIP
                                        {
                                            RefAIPMark = markStartPrice,
                                            RefAudits = auditDefault,
                                            RefCObject = constrRepository.GetOne(id),
                                            RefFinances = financesDefault,
                                            RefStatusD = statusDDefault,
                                            SourceID = extension.DataSource.ID
                                        };
                        
                        try
                        {
                            SaveValue(record, factStartPrice, "StartPrice");
                        }
                        catch (Exception)
                        {
                            factStartPrice = null;
                        }

                        var factCurPrice = new F_ExcCosts_AIP
                                        {
                                            RefAIPMark = markCurPrice,
                                            RefAudits = auditDefault,
                                            RefCObject = constrRepository.GetOne(id),
                                            RefFinances = financesDefault,
                                            RefStatusD = statusDDefault,
                                            SourceID = extension.DataSource.ID
                                        };

                        try
                        {
                            SaveValue(record, factCurPrice, "Price");
                        }
                        catch (Exception)
                        {
                            factCurPrice = null;
                        }

                        var factExpectPrice = new F_ExcCosts_AIP
                        {
                            RefAIPMark = markExpectPrice,
                            RefAudits = auditDefault,
                            RefCObject = constrRepository.GetOne(id),
                            RefFinances = financesDefault,
                            RefStatusD = statusDDefault,
                            SourceID = extension.DataSource.ID
                        };

                        try
                        {
                            SaveValue(record, factExpectPrice, "Price");
                        }
                        catch (Exception)
                        {
                            factExpectPrice = null;
                        }

                        var factToUse = factStartPrice ?? factCurPrice ?? factExpectPrice;
                        if (factToUse == null)
                        {
                            throw new Exception("Начальная цена контакта, стоимость в текущих ценах и ожидаемая стоимость основных фондов не заданы.");
                        }

                        // var factToUse = factStartPrice == null ? factCurPrice : (factStartPrice.RefContract == null ? factCurPrice : factStartPrice);
                        var contract = GetContract(record, factToUse);

                        // сохраняем здесь, потому что для показателя 6 могут быть дубликаты, и тогда факт5 сохранять не надо
                        if (factStartPrice != null)
                        {
                            SetPeriodContractTypeWork(record, factStartPrice, contract);
                            factStartPrice.RefStatusD = statusD;
                            factsAipRepository.Save(factStartPrice);
                        }

                        if (factCurPrice != null)
                        {
                            SetPeriodContractTypeWork(record, factCurPrice, contract);
                            factCurPrice.RefStatusD = statusD;
                            factsAipRepository.Save(factCurPrice);
                        }

                        if (factExpectPrice != null)
                        {
                            SetPeriodContractTypeWork(record, factExpectPrice, contract);
                            factExpectPrice.RefStatusD = statusD;
                            factsAipRepository.Save(factExpectPrice);
                        }
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Данные сохранены",
                    Data = dataSet.ToString()
                };
            }
            catch (Exception e)
            {
                return CommonService.ErrorResult(e.Message);
            }
        }

        private F_ExcCosts_AIP ChangeFact(IDictionary<string, object> record, string paramId, string paramValue, D_ExcCosts_AIPMark mark)
        {
            F_ExcCosts_AIP fact;
            try
            {
                var id = CommonService.GetIntFromRecord(record, paramId, paramId);
                fact = factsAipRepository.FindOne(id);
            }
            catch (Exception)
            {
                var auditDefault = auditRepository.FindOne(-1);
                var financesDefault = financesRepository.FindOne(-1);
                var statusDDefault = statusDRepository.FindOne(-1);
                var id = CommonService.GetIntFromRecord(record, "CObjectId", "Объект строительства");

                fact = new F_ExcCosts_AIP
                {
                    RefAIPMark = mark,
                    RefAudits = auditDefault,
                    RefCObject = constrRepository.GetOne(id),
                    RefFinances = financesDefault,
                    RefStatusD = statusDDefault,
                    SourceID = extension.DataSource.ID
                };
            }

            try 
            {
                SaveValue(record, fact, paramValue);
                return fact;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Изменить период, контракт, Вид работ
        /// </summary>
        /// <param name="record">Запись, которая изменяется</param>
        /// <param name="fact">Факт, который изменяется</param>
        /// <param name="contract">Объект контракта</param>
        private void SetPeriodContractTypeWork(IDictionary<string, object> record, F_ExcCosts_AIP fact, D_ExcCosts_Contract contract)
        {
            var typeWorkIdValue = CommonService.GetIntFromRecord(record, "TypeWorkId", "Вид работ");
            var workType = workTypeRepository.FindOne(typeWorkIdValue);

            // изменяем период
            object date;
            record.TryGetValue("Date", out date);
            if (date == null)
            {
                throw new Exception("Не определен отчетный год");
            }

            int yearValue;
            FX_Date_YearDayUNV period;

            if (Int32.TryParse(date.ToString(), out yearValue))
            {
                period = periodRepository.FindOne(yearValue);
                if (period == null)
                {
                    throw new Exception("Для заданного года в классификаторе не определен период. Обратитесь к администратору.");
                }
            }
            else
            {
                throw new Exception("Не определена отчетная дата");
            }

            fact.RefContract = contract;

            // проверить, есть ли в БД запись на выбранный период, контракт и Вид работ
            var existDublicate =
                factsAipRepository.FindAll().Count(
                    x =>
                    x.RefPeriod.ID == period.ID &&
                    x.RefContract.ID == fact.RefContract.ID &&
                    x.RefAIPMark.ID == fact.RefAIPMark.ID &&
                    x.RefTypeWork.ID == typeWorkIdValue &&
                    x.ID != fact.ID) > 0;
                        
            // если есть дубликат - вернуть ошибку
            if (existDublicate)
            {
                throw new Exception("Данные для периода '{0}', контракта '{1}' и типа работ '{2}' уже существуют".FormatWith(
                    date.ToString(), 
                    fact.RefContract.Property,
                    workType.Name));
            }

            fact.RefTypeWork = workType;
            fact.RefPeriod = period;
        }

        private D_ExcCosts_Contract GetContract(IDictionary<string, object> record, F_ExcCosts_AIP fact)
        {
            var partnerName = CommonService.GetStringFromRecord(record, "PartnerName", "Контрагент", false);
            var contractName = CommonService.GetStringFromRecord(record, "ContractName", "Контракт", false);

            var contract = fact.RefContract ?? new D_ExcCosts_Contract();
            
            contract.Property = contractName;

            if (contract.RefPartners == null)
            {
                contract.RefPartners = new D_ExcCosts_Partners();
            }

            contract.RefPartners.Name = partnerName;

            var stateId = CommonService.GetIntFromRecord(record, "StateId", "Статус контракта");
            var state = contTypeRepository.FindOne(stateId);
            contract.RefTypeCont = state;
            return contract;
        }

        private void SaveValue(IDictionary<string, object> record, F_ExcCosts_AIP fact, string paramName)
        {
            fact.Value = CommonService.GetDecimalFromRecord(record, paramName, paramName);
        }

        public class EqualityComparerFactAIP : IEqualityComparer<F_ExcCosts_AIP>
        {
            public bool Equals(F_ExcCosts_AIP x, F_ExcCosts_AIP y)
            {
                return x.RefTypeWork.ID == y.RefTypeWork.ID &&
                        x.RefContract.ID == y.RefContract.ID &&
                        x.RefPeriod.ID == y.RefPeriod.ID;
            }

            public int GetHashCode(F_ExcCosts_AIP obj)
            {
                return base.GetHashCode();
            }
        }
    }
}
