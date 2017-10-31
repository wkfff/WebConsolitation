using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers
{
    public abstract class RestDetailController<TDomain, TViewModel> : RestController<TDomain, TViewModel>
        where TDomain : DomainObject
        where TViewModel : ViewModelBase
    {
        protected RestDetailController(IRestService<TDomain, TViewModel> service) : base(service)
        {
        }

        public override ActionResult Index(int? parentId)
        {
            try
            {
                if (parentId == null)
                {
                    throw new InvalidDataException("Не указан родитель");
                }

                var data = Service.ConvertToView(Service.GetItems(Convert.ToInt32(parentId))).ToList();
                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "RestDetailController::Index: Ошибка построения списка записей: " + e.Message, Data = null };
            }
        }
    }
}
