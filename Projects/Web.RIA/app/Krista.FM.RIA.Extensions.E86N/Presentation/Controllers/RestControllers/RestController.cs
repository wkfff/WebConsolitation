using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers
{
    public abstract class RestController<TDomain, TViewModel> : SchemeBoundController, IRestController
        where TDomain : DomainObject
        where TViewModel : ViewModelBase
    {
        public readonly IRestService<TDomain, TViewModel> Service;

        protected RestController(IRestService<TDomain, TViewModel> service)
        {
            Service = service;
        }

        public virtual ActionResult Index(int? parentId)
        {
            var data = Service.ConvertToView(Service.GetItems(null)).ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public virtual ActionResult Read(int itemId)
        {
            var data = new List<TViewModel> { Service.ConvertToView(Service.GetItem(itemId)) };
            return new AjaxStoreResult(data, data.Count);
        }

        [Transaction(RollbackOnModelStateError = true)]
        public virtual ActionResult Create(string data)
        {
            try
            {
                var item = Service.Save(Service.DecodeJson(JsonUtils.FromJsonRaw(data)));
                return new RestResult { Success = true, Message = "Запись создана.", Data = Service.ConvertToView(item) };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e); //// Для сообщения методу-обертке о необходимости отката транзакции при ошибке.
                return new RestResult { Success = false, Message = "RestController::Create: Ошибка создания записи: " + e.Message, Data = null };
            }
        }

        [Transaction(RollbackOnModelStateError = true), ValidateInput(false)]
        public virtual ActionResult Update(string data)
        {
            try
            {
                var item = Service.Save(Service.DecodeJson(JsonUtils.FromJsonRaw(data)));
                return new RestResult { Success = true, Message = "Запись изменена.", Data = Service.ConvertToView(item) };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e); //// Для сообщения методу-обертке о необходимости отката транзакции при ошибке.
                return new RestResult { Success = false, Message = "RestController::Update: Ошибка изменения записи: " + e.Message, Data = null };
            }
        }

        [Transaction(RollbackOnModelStateError = true)]
        public virtual ActionResult Delete(int id)
        {
            try
            {
                Service.Delete(id);
                return new RestResult { Success = true, Message = "Запись удалена.", Data = null };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "RestController::Delete: Ошибка удаления записи: " + e.Message, Data = null };
            }
        }
    }
}
