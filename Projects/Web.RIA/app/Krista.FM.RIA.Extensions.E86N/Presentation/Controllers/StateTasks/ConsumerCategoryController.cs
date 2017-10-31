using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models.StateTaskModel;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTasks
{
    public class ConsumerCategoryController : SchemeBoundController
    {
        private readonly INewRestService newRestService;

        private readonly IChangeLogService logService;

        private readonly ConsumerCategoryModel consumerCategoryModel = new ConsumerCategoryModel();

        public ConsumerCategoryController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }
        
        public ActionResult Read(int masterId)
        {
            try
            {
                return new RestResult
                    {
                        Success = true, 
                        Data = newRestService.GetItems<F_F_GZYslPotr>()
                               .Where(x => x.RefFactGZ.ID == masterId)
                               .Select(item => new ConsumerCategoryModel
                                            {
                                                ID = item.ID,
                                                RefFactGZ = item.RefFactGZ.ID,
                                                RefServicesCPotr = item.RefServicesCPotr.ID,
                                                RefServicesCPotrName = item.RefServicesCPotr.Name
                                            })
                    };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "Ошибка загрузки категорий потребителей ГЗ: " + e.Message, Data = null };
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Save(string data, int masterId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ValidateData(dataUpdate, masterId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_F_GZYslPotr record = JavaScriptDomainConverter<F_F_GZYslPotr>.DeserializeSingle(data);
               
                string msg = "Запись обновлена";
                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = newRestService.GetItem<F_F_GosZadanie>(masterId);
                record.RefServicesCPotr = newRestService.GetItem<D_Services_CPotr>(record.RefServicesCPotr.ID);

                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParametr);
                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = newRestService.GetItems<F_F_GZYslPotr>()
                            .Where(v => v.ID == record.ID)
                            .Select(item => new ConsumerCategoryModel
                                            {
                                                ID = item.ID,
                                                RefFactGZ = item.RefFactGZ.ID,
                                                RefServicesCPotr = item.RefServicesCPotr.ID,
                                                RefServicesCPotrName = item.RefServicesCPotr.Name
                                            })
                };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = e.Message, Data = null };
            }
        }
        
        [HttpDelete]
        public RestResult Delete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_GZYslPotr>(id, docId);
        }

        public AjaxStoreResult GetCategory(int? limit, int? start, string query, int service)
        {
            var data = newRestService.GetItems<F_F_PotrYs>().Where(x => x.RefVedPP.ID.Equals(service) &&
                                                                                    x.RefCPotr.Name.Contains(query))
                                                            .Select(p => new
                                                            {
                                                                p.RefCPotr.ID,
                                                                p.RefCPotr.Name
                                                            });

            return new AjaxStoreResult(data.Skip(start ?? 0).Take(limit ?? 10), data.Count());
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg2 = "Категория \"{0}\" уже указана<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => consumerCategoryModel.RefServicesCPotr))
            {
                message.Append(Msg.FormatWith(consumerCategoryModel.DescriptionOf(() => consumerCategoryModel.RefServicesCPotrName)));
            }
            else
            {
                var category = record.GetValueToIntOrDefault(() => consumerCategoryModel.RefServicesCPotr, -1);

                if (newRestService.GetItems<F_F_GZYslPotr>().Any(x => x.ID != record.GetValueToIntOrDefault(() => consumerCategoryModel.ID, -1)
                                                                    && x.RefFactGZ.ID == masterId
                                                                    && x.RefServicesCPotr.ID == category))
                {
                    message.Append(Msg2.FormatWith(newRestService.Load<D_Services_CPotr>(category).Name));
                }
            }

            return message.ToString();
        }
    }
}
