using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Data;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.ServiceRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Models.ServisesModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

using LinqKit;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public sealed class ServicesController : SchemeBoundController
    {
        private const string MessageIsDuplicate = "Введенная запись уже существует в базе данных.";

        private readonly IAuthService auth;

        private readonly INewRestService newRestService;

        private readonly AuthRepositiory<D_Services_VedPer> repository;

        private readonly ServicesViewModel servicesModel = new ServicesViewModel();

        public ServicesController(IAuthService auth)
        {
            this.auth = auth;
            newRestService = Resolver.Get<INewRestService>();
            repository = new AuthRepositiory<D_Services_VedPer>(
                newRestService.GetRepository<D_Services_VedPer>(), 
                auth, 
                ppoIdExpr => ppoIdExpr.RefOrgPPO, 
                grbsIdExpr => grbsIdExpr.RefGRBSs.ID, 
                orgIdExpr => 0);
        }

        public ActionResult Read([FiltersBinder] FilterConditions filters, int limit = 10000, int start = 0)
        {
            var result = repository.FindAll().Select(
                v => new ServicesViewModel
                    {
                        ID = v.ID, 
                        BusinessStatus = v.BusinessStatus.Equals("801"), 
                        Code = v.Code, 
                        Name = v.Name, 
                        RefPl = v.RefPl.ID, 
                        RefPlName = v.RefPl.Name, 
                        RefGRBSs = v.RefGRBSs.ID, 
                        RefGRBSsName = v.RefGRBSs.Name, 
                        RefTipY = v.RefTipY.ID, 
                        RefTipYName = v.RefTipY.Name, 
                        RefOrgPPO = v.RefOrgPPO.ID, 
                        RefOrgPPOName = v.RefOrgPPO.Name, 
                        RefSferaD = v.RefSferaD.ID, 
                        RefSferaDName = v.RefSferaD.Name, 
                        DataVkluch = v.DataVkluch, 
                        DataIskluch = v.DataIskluch
                    });

            filters.Conditions
                .ForEach(
                    filter =>
                    {
                        if (filter.Name == servicesModel.NameOf(() => servicesModel.BusinessStatus))
                        {
                            result = result.Where(x => x.BusinessStatus == filter.ValueAsBoolean);
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefTipYName))
                        {
                            result = result.Where(x => x.RefTipYName.Equals(filter.ValueAsBoolean ? "Услуга" : "Работа"));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefPlName))
                        {
                            result = result.Where(x => filter.ValuesList.Contains(x.RefPlName));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.Name))
                        {
                            result = result.Where(x => x.Name.Contains(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefOrgPPOName))
                        {
                            result = result.Where(x => x.RefOrgPPOName.Contains(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefGRBSsName))
                        {
                            result = result.Where(x => x.RefGRBSsName.Contains(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.RefSferaDName))
                        {
                            result = result.Where(x => x.RefSferaDName.Contains(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.Code))
                        {
                            result = result.Where(x => x.Code.Contains(filter.Value));
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.DataVkluch))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    result = result.Where(x => x.DataVkluch == filter.ValueAsDate);
                                    break;
                                case Comparison.Lt:
                                    result = result.Where(x => x.DataVkluch < filter.ValueAsDate);
                                    break;
                                case Comparison.Gt:
                                    result = result.Where(x => x.DataVkluch > filter.ValueAsDate);
                                    break;
                            }
                        }

                        if (filter.Name == servicesModel.NameOf(() => servicesModel.DataIskluch))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    result = result.Where(x => x.DataIskluch == filter.ValueAsDate);
                                    break;
                                case Comparison.Lt:
                                    result = result.Where(x => x.DataIskluch < filter.ValueAsDate);
                                    break;
                                case Comparison.Gt:
                                    result = result.Where(x => x.DataIskluch > filter.ValueAsDate);
                                    break;
                            }
                        }
                    });

            return new AjaxStoreResult(result.Skip(start).Take(limit), result.Count());
        }

        [HttpPost]
        [Transaction]
        public string Copy(int id)
        {
            var target = repository.FindOne(id);
            var newItem = new D_Services_VedPer
                {
                    ID = 0, 
                    BusinessStatus = "866", 
                    Code = target.Code, 
                    DataIskluch = target.DataIskluch, 
                    DataVkluch = target.DataVkluch, 
                    Name = target.Name, 
                    Note = target.Note, 
                    RefPl = target.RefPl, 
                    RefGRBSs = target.RefGRBSs, 
                    RefTipY = target.RefTipY, 
                    RefOrgPPO = target.RefOrgPPO,
                    RefSferaD = target.RefSferaD,
                    RowType = target.RowType
/*
                    NumberLaw = target.NumberLaw, 
                    NumberService = target.NumberService, 
                    NameLow = target.NameLow, 
                    DateLaw = target.DateLaw, 
                    Founder = target.Founder, 
                    AuthorLaw = target.AuthorLaw, 
                    ParentID = target.ParentID, 
                    RefYchred = target.RefYchred, 
                    TypeLow = target.TypeLow, 
*/
                };
            repository.Save(newItem);

            var detal1 = newRestService.GetItems<F_F_PNRysl>()
                .Where(v => v.RefPerV.ID == id);
            foreach (var t in detal1)
            {
                newRestService.Save(new F_F_PNRysl { ID = 0, RefIndicators = t.RefIndicators, RefPerV = newItem, SourceID = t.SourceID, TaskID = t.TaskID });
                newRestService.GetRepository<F_F_PNRysl>().DbContext.CommitChanges();
            }

            var detal2 = newRestService.GetItems<F_F_PotrYs>()
                .Where(v => v.RefVedPP.ID == id);
            foreach (var t in detal2)
            {
                newRestService.Save(new F_F_PotrYs { ID = 0, RefCPotr = t.RefCPotr, RefVedPP = newItem, SourceID = t.SourceID, TaskID = t.TaskID });
                newRestService.GetRepository<F_F_PNRysl>().DbContext.CommitChanges();
            }

            var detal3 = newRestService.GetItems<F_F_VedPerProvider>()
                .Where(v => v.RefService.ID == id);
            foreach (var t in detal3)
            {
                newRestService.Save(new F_F_VedPerProvider { ID = 0, RefProvider = t.RefProvider, RefService = newItem, SourceID = t.SourceID, TaskID = t.TaskID });
                newRestService.GetRepository<F_F_PNRysl>().DbContext.CommitChanges();
            }

            return target.Code;
        }

        [HttpPost]
        [Transaction]
        public RestResult Save(string data)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ServiceValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    return new RestResult { Success = false, Message = validationError };
                }

                var viewModel = JavaScriptDomainConverter<ServicesViewModel>.DeserializeSingle(data);

                if (repository.FindAll().Any(
                    p => (p.ID != viewModel.ID) &&
                         (p.RefTipY.ID == viewModel.RefTipY) &&
                         (p.Name == viewModel.Name) &&
                         (p.RefPl.ID == viewModel.RefPl) &&
                         (p.RefGRBSs.ID == viewModel.RefGRBSs) &&
                         (p.RefOrgPPO.ID == viewModel.RefOrgPPO)))
                {
                    return new RestResult { Success = false, Message = MessageIsDuplicate };
                }

                D_Services_VedPer item;
                var msg = "Запись обновлена";
                if (viewModel.ID < 0)
                {
                    item = new D_Services_VedPer { ID = 0, BusinessStatus = "866" };
                    msg = "Новая запись добавлена";
                }
                else
                {
                    item = repository.Load(viewModel.ID);

                    if (!auth.IsAdmin())
                    {
                        if (ServiceIsUse(viewModel.ID) && !(
                                                               item.RefTipY.ID == viewModel.RefTipY &&
                                                               item.Code == viewModel.Code &&
                                                               item.Name == viewModel.Name &&
                                                               item.RefPl.ID == viewModel.RefPl &&
                                                               item.RefGRBSs.ID == viewModel.RefGRBSs &&
                                                               item.RefOrgPPO.ID == viewModel.RefOrgPPO &&
                                                               item.RefSferaD.ID == viewModel.RefSferaD))
                        {
                            return new RestResult { Success = false, Message = string.Concat(MessageIsUse(false), "Разрешено только изменять статус.") };
                        }
                    }
                }

                item.DataIskluch = viewModel.DataIskluch;
                item.DataVkluch = viewModel.DataVkluch;

                if (item.BusinessStatus.Equals("801") != viewModel.BusinessStatus)
                {
                    if (viewModel.BusinessStatus)
                    {
                        item.DataVkluch = DateTime.Today;
                        item.DataIskluch = null;

                        validationError = ServiceValidateDetails(viewModel);
                        if (validationError.IsNotNullOrEmpty())
                        {
                            return new RestResult { Success = false, Message = validationError };
                        }
                    }
                    else
                    {
                        item.DataIskluch = DateTime.Today;
                    }
                }

                item.Name = viewModel.Name;
                item.BusinessStatus = viewModel.BusinessStatus ? "801" : "866";
                item.Code = viewModel.Code.PadLeft(20, '0');
                item.RefPl = newRestService.GetItem<D_Services_Platnost>(viewModel.RefPl);
                item.RefTipY = newRestService.GetItem<D_Services_TipY>(viewModel.RefTipY);
                item.RefSferaD = newRestService.GetItem<D_Services_SferaD>(viewModel.RefSferaD);

                var profile = auth.Profile;

                if (auth.IsAdmin())
                {
                    item.RefOrgPPO = newRestService.GetItem<D_Org_PPO>(viewModel.RefOrgPPO);
                    item.RefGRBSs = newRestService.GetItem<D_Org_GRBS>(viewModel.RefGRBSs);
                }
                else if (auth.IsPpoUser())
                {
                    item.RefOrgPPO = newRestService.GetItem<D_Org_PPO>(profile.RefUchr.RefOrgPPO.ID);
                    item.RefGRBSs = newRestService.GetItem<D_Org_GRBS>(viewModel.RefGRBSs);
                    if (item.RefGRBSs.RefOrgPPO.ID != profile.RefUchr.RefOrgPPO.ID)
                    {
                        item.RefGRBSs = profile.RefUchr.RefOrgGRBS;
                    }
                }
                else if (auth.IsGrbsUser())
                {
                    item.RefOrgPPO = newRestService.GetItem<D_Org_PPO>(profile.RefUchr.RefOrgPPO.ID);
                    item.RefGRBSs = newRestService.GetItem<D_Org_GRBS>(profile.RefUchr.RefOrgGRBS.ID);
                }

                repository.Save(item);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = repository.FindAll()
                            .Where(v => v.ID == item.ID)
                            .Select(
                                v => new ServicesViewModel
                                    {
                                        ID = v.ID, 
                                        BusinessStatus = v.BusinessStatus.Equals("801"), 
                                        Code = v.Code, 
                                        Name = v.Name, 
                                        RefPl = v.RefPl.ID, 
                                        RefPlName = v.RefPl.Name, 
                                        RefGRBSs = v.RefGRBSs.ID, 
                                        RefGRBSsName = v.RefGRBSs.Name, 
                                        RefTipY = v.RefTipY.ID, 
                                        RefTipYName = v.RefTipY.Name, 
                                        RefOrgPPO = v.RefOrgPPO.ID, 
                                        RefOrgPPOName = v.RefOrgPPO.Name, 
                                        RefSferaD = v.RefSferaD.ID, 
                                        RefSferaDName = v.RefSferaD.Name, 
                                        DataVkluch = v.DataVkluch, 
                                        DataIskluch = v.DataIskluch
                                    })
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult Delete(int id)
        {
            try
            {
                if (ServiceIsUse(id))
                {
                    return new RestResult { Success = false, Message = MessageIsUse(true) };
                }

                newRestService.BeginTransaction();

                newRestService.GetItems<F_F_PNRysl>()
                    .Where(v => v.RefPerV.ID == id)
                    .Select(v => new { v.ID }).Each(model => newRestService.Delete<F_F_PNRysl>(model.ID));

                newRestService.GetItems<F_F_PotrYs>()
                    .Where(v => v.RefVedPP.ID == id)
                    .Select(v => new { v.ID }).Each(model => newRestService.Delete<F_F_PotrYs>(model.ID));
                
                newRestService.GetItems<F_F_VedPerProvider>()
                    .Where(v => v.RefService.ID == id)
                    .Select(v => new { v.ID }).Each(model => newRestService.Delete<F_F_VedPerProvider>(model.ID));
                
                repository.Delete(repository.FindOne(id));

                if (newRestService.HaveTransaction)
                {
                    newRestService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                if (newRestService.HaveTransaction)
                {
                    newRestService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        #region Категории потребителей

        public RestResult ConsumerRead(int masterId)
        {
            return new RestResult
                {
                    Success = true, 
                    Data = newRestService.GetItems<F_F_PotrYs>()
                        .Where(v => v.RefVedPP.ID == masterId)
                        .Select(
                            v => new ServiceRegisterConsumerViewModel
                                {
                                    ID = v.ID, 
                                    RefConsumer = v.RefCPotr.ID, 
                                    RefConsumerName = v.RefCPotr.Name
                                })
                };
        }

        [HttpPost]
        [Transaction]
        public RestResult ConsumerSave(string data, int masterId)
        {
            try
            {
                var viewModel = JavaScriptDomainConverter<ServiceRegisterConsumerViewModel>.DeserializeSingle(data);

                if (newRestService.GetItems<F_F_PotrYs>().Any(p => p.ID != viewModel.ID && p.RefCPotr.ID == viewModel.RefConsumer && p.RefVedPP.ID == masterId))
                {
                    return new RestResult { Success = false, Message = MessageIsDuplicate };
                }

                var msg = "Запись обновлена";
                if (viewModel.ID < 0)
                {
                    viewModel.ID = 0;
                    msg = "Новая запись добавлена";
                }
                else
                {
                    if (ServiceIsUse(masterId))
                    {
                        return new RestResult { Success = false, Message = MessageIsUse(false) };
                    }
                }

                var item = new F_F_PotrYs
                    {
                        ID = viewModel.ID, 
                        RefVedPP = repository.Load(masterId), 
                        RefCPotr = newRestService.GetItem<D_Services_CPotr>(viewModel.RefConsumer)
                    };
                newRestService.Save(item);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = newRestService.GetItems<F_F_PotrYs>()
                            .Where(v => v.ID == item.ID)
                            .Select(
                                v => new ServiceRegisterConsumerViewModel
                                    {
                                        ID = v.ID, 
                                        RefConsumer = v.RefCPotr.ID, 
                                        RefConsumerName = v.RefCPotr.Name
                                    })
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult ConsumerDelete(int id)
        {
            if (ServiceIsUse(newRestService.GetItems<F_F_PotrYs>().First(p => p.ID == id).RefVedPP.ID))
            {
                return new RestResult { Success = false, Message = "Категория потребителей используется в документе. Удаление запрещено!" };
            }

            return newRestService.DeleteAction<F_F_PotrYs>(id);
        }

        #endregion

        #region Показатели объема

        public RestResult CharacteristicRead(int masterId)
        {
            return new RestResult
                {
                    Success = true, 
                    Data = newRestService.GetItems<F_F_PNRysl>()
                        .Where(v => v.RefPerV.ID == masterId)
                        .Select(
                            v => new ServiceRegisterCharacteristicViewModel
                                {
                                    ID = v.ID, 
                                    RefOKEIName = v.RefIndicators.RefOKEI.Name, 
                                    RefTypeName = v.RefIndicators.RefCharacteristicType.Name, 
                                    RefIndicators = v.RefIndicators.ID, 
                                    RefIndicatorsName = v.RefIndicators.Name
                                })
                };
        }

        [HttpPost]
        [Transaction]
        public RestResult CharacteristicSave(string data, int masterId)
        {
            try
            {
                var viewModel = JavaScriptDomainConverter<ServiceRegisterCharacteristicViewModel>.DeserializeSingle(data);

                if (newRestService.GetItems<F_F_PNRysl>().Any(
                    v => (v.RefIndicators.ID == viewModel.RefIndicators)
                         && (v.RefPerV.ID == masterId)
                         && (v.ID != viewModel.ID)))
                {
                    return new RestResult { Success = false, Message = MessageIsDuplicate };
                }

                F_F_PNRysl item;
                var msg = "Запись обновлена";
                if (viewModel.ID < 0)
                {
                    viewModel.ID = 0;
                    item = new F_F_PNRysl();
                    msg = "Новая запись добавлена";
                }
                else
                {
                    item = newRestService.Load<F_F_PNRysl>(viewModel.ID);
                }

                item.ID = viewModel.ID;
                item.RefPerV = repository.Load(masterId);
                item.RefIndicators = newRestService.GetItem<D_Services_Indicators>(viewModel.RefIndicators);

                if (item.ID == 0)
                {
                    newRestService.Save(item);
                }
                else
                {
                    newRestService.CommitChanges();
                }

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = newRestService.GetItems<F_F_PNRysl>()
                            .Where(v => v.ID == item.ID)
                            .Select(
                                v => new ServiceRegisterCharacteristicViewModel
                                    {
                                        ID = v.ID, 
                                        RefOKEIName = v.RefIndicators.RefOKEI.Name, 
                                        RefTypeName = v.RefIndicators.RefCharacteristicType.Name, 
                                        RefIndicators = v.RefIndicators.ID, 
                                        RefIndicatorsName = v.RefIndicators.Name
                                    })
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult CharacteristicDelete(int id)
        {
            if (ServiceIsUse(newRestService.GetItems<F_F_PNRysl>().First(p => p.ID == id).RefPerV.ID))
            {
                return new RestResult { Success = false, Message = "Показатель используется в документе. Удаление запрещено!" };
            }

            return newRestService.DeleteAction<F_F_PNRysl>(id);
        }

        public AjaxStoreResult GetIndicators(int limit, int start, string query /*, int service*/)
        {
            /*var serviceEntity = repository.Load(service);*/

            // todo пляски с датами открытия и закрытия!!!!
            var data = newRestService.GetItems<D_Services_Indicators>().Where(
                x =>
                x.Name.Contains(query) /*&&

                                                                                    // если у услуги нет дат - выбираем показатель
                                                                                    ((!serviceEntity.DataVkluch.HasValue && !serviceEntity.DataIskluch.HasValue)

                                                                                    // если у показателя нет дат - выбираем показатель
                                                                                    || (!x.OpenDate.HasValue && !x.CloseDate.HasValue)

                                                                                    // если присутствуют все даты и 
                                                                                    // если дата включения услуги <= даты исключения показателя и
                                                                                    // дата исключения услуги >= даты включения показателя - выбираем показатель
                                                                                    || (serviceEntity.DataVkluch <= x.CloseDate && serviceEntity.DataIskluch >= x.OpenDate)

                                                                                    // если присутствуют дата включения услуги и дата исключения показателя и
                                                                                    // если дата включения услуги <= даты исключения показателя - выбираем показатель
                                                                                    || (serviceEntity.DataVkluch <= x.CloseDate)

                                                                                    // если присутствуют дата исключения услуги и дата включения показателя и
                                                                                    // если дата исключения услуги больше даты включения показателя - выбираем показатель
                                                                                    || (serviceEntity.DataIskluch >= x.OpenDate)

                                                                                    // если присутствуют только даты включения услуги и показателя - выбираем показатель
                                                                                    || (serviceEntity.DataVkluch.HasValue && x.OpenDate.HasValue)

                                                                                    // если присутствуют только даты исключения услуги и показателя - выбираем показатель
                                                                                     || (serviceEntity.DataIskluch.HasValue && x.CloseDate.HasValue))*/)
                .Select(
                    p => new
                        {
                            p.ID, 
                            p.Name, 
                            RefOKEI = p.RefOKEI.Name, 
                            RefOKEIID = p.RefOKEI.ID, 
                            RefCharacteristicType = p.RefCharacteristicType.Name, 
                            RefTypeID = p.RefCharacteristicType.ID
                        });

            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }

        #endregion

        #region Поставщики услуг

        public RestResult ProviderRead(int masterId)
        {
            return new RestResult
                {
                    Success = true, 
                    Data = newRestService.GetItems<F_F_VedPerProvider>()
                        .Where(v => v.RefService.ID == masterId)
                        .Select(
                            v => new ServiceRegisterProviderViewModel
                                {
                                    ID = v.ID, 
                                    RefProvider = v.RefProvider.ID, 
                                    RefProviderName = v.RefProvider.Name
                                })
                };
        }

        [HttpPost]
        [Transaction]
        public RestResult ProviderSave(string data, int masterId)
        {
            try
            {
                var item = JavaScriptDomainConverter<F_F_VedPerProvider>.DeserializeSingle(data);
                item.RefService = repository.Load(masterId);
                item.RefProvider = newRestService.GetItem<D_Org_Structure>(item.RefProvider.ID);

                if (newRestService.GetItems<F_F_VedPerProvider>().Any(p => p.ID != item.ID && p.RefService.ID == item.RefService.ID && p.RefProvider.ID == item.RefProvider.ID))
                {
                    return new RestResult { Success = false, Message = MessageIsDuplicate };
                }

                var msg = "Запись обновлена";
                if (item.ID < 0)
                {
                    item.ID = 0;
                    msg = "Новая запись добавлена";
                }

                newRestService.Save(item);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = newRestService.GetItems<F_F_VedPerProvider>()
                            .Where(v => v.ID == item.ID)
                            .Select(
                                v => new ServiceRegisterProviderViewModel
                                    {
                                        ID = v.ID, 
                                        RefProvider = v.RefProvider.ID, 
                                        RefProviderName = v.RefProvider.Name
                                    })
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult ProviderDelete(int id)
        {
            return newRestService.DeleteAction<F_F_VedPerProvider>(id);
        }

        public AjaxStoreResult GetProvider(int limit, int start, string query)
        {
            var data = newRestService.GetItems<D_Org_Structure>()
                .Where(
                    p => (p.RefTipYc.ID == FX_Org_TipYch.BudgetaryID || p.RefTipYc.ID == FX_Org_TipYch.AutonomousID || p.RefTipYc.ID == FX_Org_TipYch.GovernmentID) &&
                         (p.INN.Contains(query) || p.KPP.Contains(query) || p.ShortName.Contains(query) || p.Name.Contains(query)));
            if (auth.IsGrbsUser())
            {
                data = data.Where(p => p.RefOrgGRBS.ID == auth.ProfileOrg.RefOrgGRBS.ID);
            }

            if (auth.IsPpoUser())
            {
                data = data.Where(p => p.RefOrgPPO.ID == auth.ProfileOrg.RefOrgPPO.ID);
            }

            var rez = data.Select(
                p => new
                    {
                        p.ID, 
                        p.Name, 
                        p.INN, 
                        p.KPP, 
                        p.ShortName
                    });
            return new AjaxStoreResult(rez.Skip(start).Take(limit), rez.Count());
        }

        #endregion
        private static string MessageIsUse(bool isDelete)
        {
            return string.Concat("Услуга используется в документе. ", isDelete ? "удаление " : "изменение ", "запрещено!");
        }

        private bool ServiceIsUse(int id)
        {
            return newRestService.GetItems<F_F_GosZadanie>().Any(v => v.RefVedPch.ID == id) || newRestService.GetItems<F_ResultWork_ShowService>().Any(v => v.RefVedPch.ID == id);
        }

        private string ServiceValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => servicesModel.Code))
            {
                message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.Code)));
            }

            if (record.CheckNull(() => servicesModel.Name))
            {
                message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.Name)));
            }

            if (record.CheckNull(() => servicesModel.RefPl))
            {
                message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.RefPlName)));
            }

            if (record.CheckNull(() => servicesModel.RefTipY))
            {
                message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.RefTipYName)));
            }

            if (record.CheckNull(() => servicesModel.RefSferaD))
            {
                message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.RefSferaDName)));
            }

            if (auth.Profile == null || auth.IsAdmin())
            {
                if (record.CheckNull(() => servicesModel.RefGRBSs))
                {
                    message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.RefGRBSsName)));
                }

                if (record.CheckNull(() => servicesModel.RefOrgPPO))
                {
                    message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.RefOrgPPOName)));
                }
            }

            if (auth.IsPpoUser())
            {
                if (record.CheckNull(() => servicesModel.RefGRBSs))
                {
                    message.Append(Msg.FormatWith(UiBuilders.DescriptionOf(() => servicesModel.RefGRBSsName)));
                }
            }

            return message.ToString();
        }

        private string ServiceValidateDetails(ServicesViewModel formData)
        {
            const string ValidationError = "Детализация \"{0}\" должна быть заполнена.<br>";
            var message = new StringBuilder(string.Empty);

            if (!newRestService.GetItems<F_F_VedPerProvider>().Any(p => p.RefService.ID == formData.ID))
            {
                message.Append(ValidationError.FormatWith("Поставщики услуг"));
            }

            if (formData.RefTipY == 1)
            {
                if (!newRestService.GetItems<F_F_PotrYs>().Any(v => v.RefVedPP.ID == formData.ID))
                {
                    message.Append(ValidationError.FormatWith("Категории потребителей"));
                }

                if (!newRestService.GetItems<F_F_PNRysl>().Any(v => v.RefPerV.ID == formData.ID))
                {
                    message.Append(ValidationError.FormatWith("Показатели объема и качества услуг/работ"));
                }

                if (!newRestService.GetItems<F_F_PNRysl>().Any(v => v.RefPerV.ID == formData.ID && v.RefIndicators.RefCharacteristicType.ID == FX_FX_CharacteristicType.VolumeIndex) ||
                    !newRestService.GetItems<F_F_PNRysl>().Any(v => v.RefPerV.ID == formData.ID && v.RefIndicators.RefCharacteristicType.ID == FX_FX_CharacteristicType.QualityIndex))
                {
                    message.Append("Должен присутствовать как минимум одни показатель обьема и один показатель качества");
                }
            }

            return message.ToString();
        }
    }
}
