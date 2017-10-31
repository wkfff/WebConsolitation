using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Data;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.Service2016Model;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

using LinqKit;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Service2016
{
    public class Service2016Controller : SchemeBoundController
    {
        private const string MessageIsDuplicate = "Введенная запись уже существует в базе данных {0}<br>";

        private readonly IAuthService auth;

        private readonly INewRestService newRestService;

        private readonly AuthRepositiory<D_Services_Service> repository;

        private readonly Service2016ViewModel servicesModel = new Service2016ViewModel();
        
        public Service2016Controller()
        {
            auth = Resolver.Get<IAuthService>();
            newRestService = Resolver.Get<INewRestService>();

            repository = new AuthRepositiory<D_Services_Service>(
                newRestService.GetRepository<D_Services_Service>(),
                auth,
                ppoIdExpr => ppoIdExpr.RefYchr.RefOrgPPO,
                grbsIdExpr => grbsIdExpr.RefYchr.RefOrgGRBS.ID,
                orgIdExpr => 0);
        }

        public static string MessageIsUse(bool isDelete)
        {
            return string.Concat("Услуга используется в документе, ", isDelete ? "удаление " : "изменение ", "запрещено!");
        }

        public static bool ServiceIsUse(int id)
        {
            var newRestService = Resolver.Get<INewRestService>();
            return newRestService.GetItems<F_F_GosZadanie2016>().Any(v => v.RefService.ID == id);
        }

        public ActionResult Read([FiltersBinder] FilterConditions filters, bool editable, int limit = 10000, int start = 0)
        {
            var result = repository.FindAll().Where(x => x.IsEditable == editable).Select(
                v => new Service2016ViewModel
                         {
                             ID = v.ID,
                             Regrnumber = v.Regrnumber,
                             EffectiveFrom = v.EffectiveFrom,
                             EffectiveBefore = v.EffectiveBefore,
                             InstCode = v.InstCode,
                             NameCode = v.NameCode,
                             NameName = v.NameName,
                             SvcCnts1CodeVal = v.SvcCnts1CodeVal,
                             SvcCnts2CodeVal = v.SvcCnts2CodeVal,
                             SvcCnts3CodeVal = v.SvcCnts3CodeVal,
                             SvcCntsName1Val = v.SvcCntsName1Val,
                             SvcCntsName2Val = v.SvcCntsName2Val,
                             SvcCntsName3Val = v.SvcCntsName3Val,
                             SvcTerms1CodeVal = v.SvcTerms1CodeVal,
                             SvcTerms2CodeVal = v.SvcTerms2CodeVal,
                             SvcTermsName1Val = v.SvcTermsName1Val,
                             SvcTermsName2Val = v.SvcTermsName2Val,
                             GUID = v.GUID,
                             RefType = v.RefType.ID,
                             RefTypeCode = v.RefType.Code,
                             RefTypeName = v.RefType.Name,
                             RefPay = v.RefPay.ID,
                             RefPayCode = v.RefPay.Code,
                             RefPayName = v.RefPay.Name,
                             RefOKTMO = v.RefOKTMO.ID,
                             RefOKTMOCode = v.RefOKTMO.Code,
                             RefOKTMOName = v.RefOKTMO.Name,
                             RefYchr = v.RefYchr.ID,
                             RefYchrName = v.RefYchr.Name,
                             RefYchrShortName = v.RefYchr.ShortName,
                             RefYchrInn = v.RefYchr.INN,
                             RefActivityType = v.RefActivityType.ID,
                             RefActivityTypeCode = v.RefActivityType.Code,
                             RefActivityTypeName = v.RefActivityType.Name,
                             IsEditable = v.IsEditable.GetValueOrDefault(),
                             IsActual = v.IsActual,
                             BusinessStatus = v.BusinessStatus.Equals(D_Services_Service.Included),
                             FromPlaning = v.FromPlaning.GetValueOrDefault()
                         }).ToList();

            IEnumerable<Service2016ViewModel> filteredData = result;

            filters.Conditions
                .ForEach(
                    filter =>
                    {
                        if (filter.Name == servicesModel.NameOf(() => servicesModel.BusinessStatus))
                        {
                            filteredData = filteredData.Where(x => x.BusinessStatus == filter.ValueAsBoolean);
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefTypeName))
                        {
                            filteredData = filteredData.Where(x => x.RefTypeName.Equals(filter.ValueAsBoolean ? "Услуга" : "Работа"));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefPayName))
                        {
                            filteredData =
                                filteredData.Where(
                                    x => x.RefPayCode.Equals(filter.ValueAsBoolean ? FX_FX_ServicePayType2.CodeOfFree : FX_FX_ServicePayType2.CodeOfPayable));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.InstCode))
                        {
                            filteredData = filteredData.Where(x => x.InstCode.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefYchrName))
                        {
                            filteredData = filteredData.Where(
                                x =>
                                    x.RefYchrName.ContainsUppers(filter.Value) ||
                                    x.RefYchrShortName.ContainsUppers(filter.Value) ||
                                    x.RefYchrInn.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefOKTMOName))
                        {
                            filteredData = filteredData.Where(
                                x =>
                                    x.RefOKTMOName.ContainsUppers(filter.Value) ||
                                    x.RefOKTMOCode.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefActivityTypeCode))
                        {
                            filteredData = filteredData.Where(x => x.RefActivityTypeCode.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefActivityTypeName))
                        {
                            filteredData = filteredData.Where(x => x.RefActivityTypeName.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.Regrnumber))
                        {
                            filteredData = filteredData.Where(x => x.Regrnumber.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.NameCode))
                        {
                            filteredData = filteredData.Where(x => x.NameCode.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.NameName))
                        {
                            filteredData = filteredData.Where(x => x.NameName.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.SvcCntsName1Val))
                        {
                            filteredData = filteredData.Where(x => x.SvcCntsName1Val.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.SvcCntsName2Val))
                        {
                            filteredData = filteredData.Where(x => x.SvcCntsName2Val.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.SvcCntsName3Val))
                        {
                            filteredData = filteredData.Where(x => x.SvcCntsName3Val.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.SvcTermsName1Val))
                        {
                            filteredData = filteredData.Where(x => x.SvcTermsName1Val.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.SvcTermsName2Val))
                        {
                            filteredData = filteredData.Where(x => x.SvcTermsName2Val.ContainsUppers(filter.Value));
                        }
                        
                        if (filter.Name == servicesModel.NameOf(() => servicesModel.EffectiveFrom))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    filteredData = filteredData.Where(x => x.EffectiveFrom == filter.ValueAsDate);
                                    break;
                                case Comparison.Lt:
                                    filteredData = filteredData.Where(x => x.EffectiveFrom < filter.ValueAsDate);
                                    break;
                                case Comparison.Gt:
                                    filteredData = filteredData.Where(x => x.EffectiveFrom > filter.ValueAsDate);
                                    break;
                            }
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.EffectiveBefore))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    filteredData = filteredData.Where(x => x.EffectiveBefore == filter.ValueAsDate);
                                    break;
                                case Comparison.Lt:
                                    filteredData = filteredData.Where(x => x.EffectiveBefore < filter.ValueAsDate);
                                    break;
                                case Comparison.Gt:
                                    filteredData = filteredData.Where(x => x.EffectiveBefore > filter.ValueAsDate);
                                    break;
                            }
                        }
                        
                        if (filter.Name == servicesModel.NameOf(() => servicesModel.GUID))
                        {
                            filteredData = filteredData.Where(x => x.GUID.ContainsUppers(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.FromPlaning))
                        {
                            filteredData = filteredData.Where(x => x.FromPlaning == filter.ValueAsBoolean);
                        }
                    });

            var res = filteredData.ToList();

            return new AjaxStoreResult(res.Skip(start).Take(limit), res.Count);
        }
        
        [HttpPost]
        [Transaction]
        public RestResult ExcludeService(int id)
        {
            try
            {
                var service = repository.FindOne(id);
                service.BusinessStatus = D_Services_Service.Excluded;
                repository.Save(service);
                return new RestResult { Success = true, Message = "Услуга исключена: " + service.Regrnumber };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
       
        public RestResult CheckActuality(string regrnumber)
        {
            if (regrnumber == null)
            {
                return new RestResult { Success = false, Message = "Не удалось проверить актуальность.<br>Нужно заполнить реестровый номер." };
            }

            var actualCommonService = newRestService.GetItems<D_Services_Service>().FirstOrDefault(
                    x => x.Regrnumber.StartsWith(regrnumber.Substring(0, 42)) &&
                         x.Regrnumber != regrnumber &&
                         x.BusinessStatus == D_Services_Service.Included);
            if (actualCommonService != null)
            {
                var oldVersion = Convert.ToInt32(actualCommonService.Regrnumber.Substring(42, 3));
                var newVersion = Convert.ToInt32(regrnumber.Substring(42, 3));

                if (oldVersion >= newVersion)
                {
                    return new RestResult { Success = false, Message = "Имеется более новая услуга " };
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Услуга станет неактуальной " + actualCommonService.Regrnumber + ".<br>Применить изменения?",
                    Data = actualCommonService.ID
                };
            }

            return new RestResult { Success = true };
        }

        [HttpPost]
        [Transaction]
        public RestResult Create(string data)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ServiceValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                data = data.Replace("\"IsEditable\":\"\"", "\"IsEditable\":\"false\"");
                var viewModel = JavaScriptDomainConverter<Service2016ViewModel>.DeserializeSingle(data);

                if (repository.FindAll().Any(
                    p => (p.ID != viewModel.ID) &&
                         (p.Regrnumber == viewModel.Regrnumber) &&
                         (p.IsEditable == viewModel.IsEditable) &&
                         (p.BusinessStatus.Equals(D_Services_Service.Included) && viewModel.BusinessStatus.Equals(true)) &&
                         (p.RefType.ID == viewModel.RefType) &&
                         (p.NameName == viewModel.NameName) &&
                         (p.NameCode == viewModel.NameCode) &&
                         (p.RefYchr.ID == viewModel.RefYchr) &&
                         (p.SvcCntsName1Val == viewModel.SvcCntsName1Val) &&
                         (p.SvcCntsName2Val == viewModel.SvcCntsName2Val) &&
                         (p.SvcCntsName3Val == viewModel.SvcCntsName3Val) &&
                         (p.SvcTermsName1Val == viewModel.SvcTermsName1Val) &&
                         (p.SvcTermsName2Val == viewModel.SvcTermsName2Val) &&
                         (p.RefActivityType.ID == viewModel.RefActivityType)))
                {
                    return new RestResult { Success = false, Message = MessageIsDuplicate.FormatWith(viewModel.Regrnumber) };
                }

                D_Services_Service item;

                string msg = "Запись обновлена";
                if (viewModel.ID < 0)
                {
                    msg = "Новая запись добавлена";
                    item = new D_Services_Service
                    {
                        ID = 0,
                        BusinessStatus = D_Services_Service.Excluded,
                        GUID = Guid.NewGuid().ToString(),
                        IsEditable = true
                    };
                }
                else
                {
                    item = repository.Load(viewModel.ID);

                    if (!auth.IsAdmin())
                    {
                        if (ServiceIsUse(viewModel.ID) && !(
                            (item.Regrnumber == viewModel.Regrnumber) &&
                            (item.RefType.ID == viewModel.RefType) &&
                            (item.NameName == viewModel.NameName) &&
                            (item.NameCode == viewModel.NameCode) &&
                            (item.RefYchr.ID == viewModel.RefYchr) &&
                            (item.SvcCntsName1Val == viewModel.SvcCntsName1Val) &&
                            (item.SvcCntsName2Val == viewModel.SvcCntsName2Val) &&
                            (item.SvcCntsName3Val == viewModel.SvcCntsName3Val) &&
                            (item.SvcTermsName1Val == viewModel.SvcTermsName1Val) &&
                            (item.SvcTermsName2Val == viewModel.SvcTermsName2Val) &&
                            (item.RefActivityType.ID == viewModel.RefActivityType)))
                        {
                            return new RestResult { Success = false, Message = string.Concat(MessageIsUse(false), "Разрешено только изменять статус.") };
                        }
                    }
                }

                item.EffectiveFrom = viewModel.EffectiveFrom;
                item.EffectiveBefore = viewModel.EffectiveBefore;

                if (item.BusinessStatus.Equals(D_Services_Service.Included) != viewModel.BusinessStatus)
                {
                    if (viewModel.BusinessStatus)
                    {
                        item.EffectiveFrom = DateTime.Today;
                        item.EffectiveBefore = null;

                        validationError = ServiceValidateDetails(viewModel);
                        if (validationError.IsNotNullOrEmpty())
                        {
                            return new RestResult { Success = false, Message = validationError };
                        }
                    }
                    else
                    {
                        item.EffectiveBefore = DateTime.Today;
                    }
                }

                item.NameName = viewModel.NameName;
                item.NameCode = viewModel.NameCode;
                item.BusinessStatus = viewModel.BusinessStatus ? D_Services_Service.Included : D_Services_Service.Excluded;
                item.InstCode = viewModel.InstCode;
                item.Regrnumber = viewModel.Regrnumber;

                item.SvcCnts1CodeVal = viewModel.SvcCnts1CodeVal;
                item.SvcCnts2CodeVal = viewModel.SvcCnts2CodeVal;
                item.SvcCnts3CodeVal = viewModel.SvcCnts3CodeVal;
                item.SvcCntsName1Val = viewModel.SvcCntsName1Val;
                item.SvcCntsName2Val = viewModel.SvcCntsName2Val;
                item.SvcCntsName3Val = viewModel.SvcCntsName3Val;
                item.SvcTerms1CodeVal = viewModel.SvcTerms1CodeVal;
                item.SvcTerms2CodeVal = viewModel.SvcTerms2CodeVal;
                item.SvcTermsName1Val = viewModel.SvcTermsName1Val;
                item.SvcTermsName2Val = viewModel.SvcTermsName2Val;

                item.RefType = newRestService.GetItem<FX_FX_ServiceType>(viewModel.RefType);
                item.RefActivityType = newRestService.GetItem<D_Services_ActivityType>(viewModel.RefActivityType);
                item.RefPay = newRestService.GetItem<FX_FX_ServicePayType2>(viewModel.RefPay);
                item.RefYchr = newRestService.GetItem<D_Org_Structure>(viewModel.RefYchr);
                item.RefOKTMO = newRestService.GetItem<D_OKTMO_OKTMO>(viewModel.RefOKTMO);

                repository.Save(item);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = repository.FindAll()
                            .Where(v => v.ID == item.ID)
                            .Select(
                                v => new Service2016ViewModel
                                {
                                    ID = v.ID,
                                    Regrnumber = v.Regrnumber,
                                    EffectiveFrom = v.EffectiveFrom,
                                    EffectiveBefore = v.EffectiveBefore,
                                    InstCode = v.InstCode,
                                    NameCode = v.NameCode,
                                    NameName = v.NameName,
                                    SvcCnts1CodeVal = v.SvcCnts1CodeVal,
                                    SvcCnts2CodeVal = v.SvcCnts2CodeVal,
                                    SvcCnts3CodeVal = v.SvcCnts3CodeVal,
                                    SvcCntsName1Val = v.SvcCntsName1Val,
                                    SvcCntsName2Val = v.SvcCntsName2Val,
                                    SvcCntsName3Val = v.SvcCntsName3Val,
                                    SvcTerms1CodeVal = v.SvcTerms1CodeVal,
                                    SvcTerms2CodeVal = v.SvcTerms2CodeVal,
                                    SvcTermsName1Val = v.SvcTermsName1Val,
                                    SvcTermsName2Val = v.SvcTermsName2Val,
                                    GUID = v.GUID,
                                    RefType = v.RefType.ID,
                                    RefTypeCode = v.RefType.Code,
                                    RefTypeName = v.RefType.Name,
                                    RefPay = v.RefPay.ID,
                                    RefPayCode = v.RefPay.Code,
                                    RefPayName = v.RefPay.Name,
                                    RefOKTMO = v.RefOKTMO.ID,
                                    RefOKTMOCode = v.RefOKTMO.Code,
                                    RefOKTMOName = v.RefOKTMO.Name,
                                    RefYchr = v.RefYchr.ID,
                                    RefYchrName = v.RefYchr.Name,
                                    RefYchrShortName = v.RefYchr.ShortName,
                                    RefYchrInn = v.RefYchr.INN,
                                    RefActivityType = v.RefActivityType.ID,
                                    RefActivityTypeCode = v.RefActivityType.Code,
                                    RefActivityTypeName = v.RefActivityType.Name,
                                    IsEditable = v.IsEditable.GetValueOrDefault(),
                                    IsActual = v.IsActual,
                                    BusinessStatus = v.BusinessStatus.Equals(D_Services_Service.Included),
                                    FromPlaning = v.FromPlaning.GetValueOrDefault()
                                })
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Delete(int id)
        {
            try
            {
                if (ServiceIsUse(id))
                {
                    return new RestResult { Success = false, Message = MessageIsUse(true) };
                }

                // Удаление детализаций Услуги
                foreach (var detailId in newRestService.GetItems<F_F_ServiceInstitutionType>().Where(v => v.RefService.ID == id).Select(v => v.ID))
                {
                    newRestService.Delete<F_F_ServiceInstitutionType>(detailId);
                }

                foreach (var detailId in newRestService.GetItems<F_F_ServiceLegalAct>().Where(v => v.RefService.ID == id).Select(v => v.ID))
                {
                    newRestService.Delete<F_F_ServiceLegalAct>(detailId);
                }

                foreach (var detailId in newRestService.GetItems<F_F_ServiceOKPD>().Where(v => v.RefService.ID == id).Select(v => v.ID))
                {
                    newRestService.Delete<F_F_ServiceOKPD>(detailId);
                }

                foreach (var detailId in newRestService.GetItems<F_F_ServiceOKVED>().Where(v => v.RefService.ID == id).Select(v => v.ID))
                {
                    newRestService.Delete<F_F_ServiceOKVED>(detailId);
                }

                foreach (var detailId in newRestService.GetItems<F_F_ServiceConsumersCategory>().Where(v => v.RefService.ID == id).Select(v => v.ID))
                {
                    newRestService.Delete<F_F_ServiceConsumersCategory>(detailId);
                }

                foreach (var detailId in newRestService.GetItems<F_F_ServiceInstitutionsInfo>().Where(v => v.RefService.ID == id).Select(v => v.ID))
                {
                    newRestService.Delete<F_F_ServiceInstitutionsInfo>(detailId);
                }

                foreach (var detailId in newRestService.GetItems<F_F_ServiceIndicators>().Where(v => v.RefService.ID == id).Select(v => v.ID))
                {
                    newRestService.Delete<F_F_ServiceIndicators>(detailId);
                }

                // Удаление самой услуги
                repository.Delete(repository.FindOne(id));

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public string Copy(int id)
        {
            var target = repository.FindOne(id);
            var newItem = new D_Services_Service
            {
                ID = 0,
                RowType = target.RowType,
                RefType = target.RefType,
                RefPay = target.RefPay,
                NameCode = target.NameCode,
                NameName = target.NameName,
                Regrnumber = target.Regrnumber,
                RefActivityType = target.RefActivityType,
                EffectiveFrom = target.EffectiveFrom,
                EffectiveBefore = target.EffectiveBefore,
                InstCode = target.InstCode,
                RefYchr = target.RefYchr,
                RefOKTMO = target.RefOKTMO,
                SvcCnts1CodeVal = target.SvcCnts1CodeVal,
                SvcCnts2CodeVal = target.SvcCnts2CodeVal,
                SvcCnts3CodeVal = target.SvcCnts3CodeVal,
                SvcCntsName1Val = target.SvcCntsName1Val,
                SvcCntsName2Val = target.SvcCntsName2Val,
                SvcCntsName3Val = target.SvcCntsName3Val,
                SvcTerms1CodeVal = target.SvcTerms1CodeVal,
                SvcTerms2CodeVal = target.SvcTerms2CodeVal,
                SvcTermsName1Val = target.SvcTermsName1Val,
                SvcTermsName2Val = target.SvcTermsName2Val,
                GUID = Guid.NewGuid().ToString(),
                IsActual = target.IsActual,
                IsEditable = true,
                BusinessStatus = D_Services_Service.Excluded,
                FromPlaning = target.FromPlaning
            };
            repository.Save(newItem);

            foreach (var detail in newRestService.GetItems<F_F_ServiceInstitutionType>().Where(v => v.RefService.ID == id))
            {
                newRestService.Save(
                    new F_F_ServiceInstitutionType
                    {
                        ID = 0,
                        SourceID = detail.SourceID,
                        TaskID = detail.TaskID,
                        RefService = newItem,
                        Code = detail.Code,
                        Name = detail.Name,
                        GUID = Guid.NewGuid().ToString()
                    });
                newRestService.GetRepository<F_F_ServiceInstitutionType>().DbContext.CommitChanges();
            }

            foreach (var detail in newRestService.GetItems<F_F_ServiceLegalAct>().Where(v => v.RefService.ID == id))
            {
                newRestService.Save(
                    new F_F_ServiceLegalAct
                    {
                        ID = 0,
                        SourceID = detail.SourceID,
                        TaskID = detail.TaskID,
                        RefService = newItem,
                        Name = detail.Name,
                        ApprovedBy = detail.ApprovedBy,
                        ApprvdAt = detail.ApprvdAt,
                        EffectiveFrom = detail.EffectiveFrom,
                        DatetEnd = detail.DatetEnd,
                        Kind = detail.Kind,
                        MJregdate = detail.MJregdate,
                        MJnumber = detail.MJnumber,
                        LANumber = detail.LANumber
                    });
                newRestService.GetRepository<F_F_ServiceLegalAct>().DbContext.CommitChanges();
            }

            foreach (var detail in newRestService.GetItems<F_F_ServiceOKPD>().Where(v => v.RefService.ID == id))
            {
                newRestService.Save(
                    new F_F_ServiceOKPD
                    {
                        ID = 0,
                        SourceID = detail.SourceID,
                        TaskID = detail.TaskID,
                        RefService = newItem,
                        Name = detail.Name,
                        Code = detail.Code
                    });
                newRestService.GetRepository<F_F_ServiceOKPD>().DbContext.CommitChanges();
            }

            foreach (var detail in newRestService.GetItems<F_F_ServiceOKVED>().Where(v => v.RefService.ID == id))
            {
                newRestService.Save(
                    new F_F_ServiceOKVED
                    {
                        ID = 0,
                        SourceID = detail.SourceID,
                        TaskID = detail.TaskID,
                        RefService = newItem,
                        Name = detail.Name,
                        Code = detail.Code
                    });
                newRestService.GetRepository<F_F_ServiceOKVED>().DbContext.CommitChanges();
            }

            foreach (var detail in newRestService.GetItems<F_F_ServiceConsumersCategory>().Where(v => v.RefService.ID == id))
            {
                newRestService.Save(
                    new F_F_ServiceConsumersCategory
                    {
                        ID = 0,
                        SourceID = detail.SourceID,
                        TaskID = detail.TaskID,
                        RefService = newItem,
                        Name = detail.Name,
                        Code = detail.Code
                    });
                newRestService.GetRepository<F_F_ServiceConsumersCategory>().DbContext.CommitChanges();
            }

            foreach (var detail in newRestService.GetItems<F_F_ServiceInstitutionsInfo>().Where(v => v.RefService.ID == id))
            {
                newRestService.Save(
                    new F_F_ServiceInstitutionsInfo
                    {
                        ID = 0,
                        SourceID = detail.SourceID,
                        TaskID = detail.TaskID,
                        RefService = newItem,
                        Code = detail.Code,
                        RefStructure = detail.RefStructure,
                        OKVED = detail.OKVED,
                        DateReg = detail.DateReg,
                        OKOPF = detail.OKOPF,
                        InstKindCode = detail.InstKindCode,
                        InstKindName = detail.InstKindName
                    });
                newRestService.GetRepository<F_F_ServiceInstitutionsInfo>().DbContext.CommitChanges();
            }

            foreach (var detail in newRestService.GetItems<F_F_ServiceIndicators>().Where(v => v.RefService.ID == id))
            {
                newRestService.Save(
                    new F_F_ServiceIndicators
                    {
                        ID = 0,
                        SourceID = detail.SourceID,
                        TaskID = detail.TaskID,
                        RefService = newItem,
                        Name = detail.Name,
                        Code = detail.Code,
                        RefType = detail.RefType,
                        RefOKEI = detail.RefOKEI
                    });
                newRestService.GetRepository<F_F_ServiceIndicators>().DbContext.CommitChanges();
            }

            return target.NameCode;
        }

        public AjaxStoreResult GetFounder(int limit, int start, string query)
        {
            IQueryable<D_Org_Structure> data;

            if (auth.IsGrbsUser())
            {
                data = new[] { auth.Profile.RefUchr }.AsQueryable();
            }
            else
            {
                data = newRestService.GetItems<D_Org_Structure>()
                    .Where(
                        p => (p.RefTipYc.ID == 1) && // Федеральные
                             (p.INN.Contains(query) || p.KPP.Contains(query) || p.ShortName.Contains(query) || p.Name.Contains(query)));

                if (auth.IsPpoUser())
                {
                    data = data.Where(x => x.RefOrgGRBS.RefOrgPPO == auth.Profile.RefUchr.RefOrgPPO);
                }
            }

            var result = data.Select(
                p => new
                {
                    p.ID,
                    p.Name
                });

            return new AjaxStoreResult(result.Skip(start).Take(limit), result.Count());
        }

        public AjaxStoreResult GetOKTMO(int limit, int start, string query)
        {
            var data = newRestService.GetItems<D_OKTMO_OKTMO>()
                .Where(p => p.Name.Contains(query) || p.Code.Contains(query));

            var result = data.Select(
                p => new
                {
                    p.ID,
                    p.Name
                });

            return new AjaxStoreResult(result.Skip(start).Take(limit), result.Count());
        }

        public AjaxStoreResult GetActivityType(int limit, int start, string query)
        {
            var data = newRestService.GetItems<D_Services_ActivityType>()
                .Where(p => p.Name.Contains(query) || p.Code.Contains(query));

            var result = data.Select(
                p => new
                {
                    p.ID,
                    p.Code,
                    p.Name
                });

            return new AjaxStoreResult(result.Skip(start).Take(limit), result.Count());
        }
        
        private string ServiceValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = string.Empty;
            
            if (record.CheckNull(() => servicesModel.Regrnumber))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.Regrnumber));
            }

            if (record.CheckNull(() => servicesModel.EffectiveFrom))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.EffectiveFrom));
            }

            if (record.CheckNull(() => servicesModel.InstCode))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.InstCode));
            }

            if (record.CheckNull(() => servicesModel.NameCode))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.NameCode));
            }

            if (record.CheckNull(() => servicesModel.NameName))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.NameName));
            }

            if (record.CheckNull(() => servicesModel.RefTypeName))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.RefTypeName));
            }

            if (record.CheckNull(() => servicesModel.RefPayName))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.RefPayName));
            }

            if (record.CheckNull(() => servicesModel.RefOKTMOName))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.RefOKTMOName));
            }

            if (record.CheckNull(() => servicesModel.RefYchrName))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.RefYchrName));
            }

            if (record.CheckNull(() => servicesModel.RefActivityTypeName))
            {
                message += Msg.FormatWith(servicesModel.DescriptionOf(() => servicesModel.RefActivityTypeName));
            }

            return message;
        }

        private string ServiceValidateDetails(Service2016ViewModel formData)
        {
            const string ValidationError = "Детализация \"{0}\" должна быть заполнена.<br>";

            var message = new StringBuilder(string.Empty);

            if (!newRestService.GetItems<F_F_ServiceConsumersCategory>().Any(p => p.RefService.ID == formData.ID))
            {
                message.Append(ValidationError.FormatWith("Категории потребителей"));
            }

            if (!newRestService.GetItems<F_F_ServiceOKVED>().Any(p => p.RefService.ID == formData.ID))
            {
                message.Append(ValidationError.FormatWith("ОКВЭД"));
            }

            if (!newRestService.GetItems<F_F_ServiceInstitutionsInfo>().Any(p => p.RefService.ID == formData.ID))
            {
                message.Append(ValidationError.FormatWith("Информация об учреждениях"));
            }

            if (!newRestService.GetItems<F_F_ServiceIndicators>().Any(v => v.RefService.ID == formData.ID && v.RefType.ID == FX_FX_CharacteristicType.VolumeIndex))
            {
                message.Append(ValidationError.FormatWith("Показатели объема"));
            }

            return message.ToString();
        }
    }
}