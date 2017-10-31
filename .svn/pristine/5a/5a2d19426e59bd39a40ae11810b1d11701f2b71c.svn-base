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
using Krista.FM.RIA.Extensions.E86N.Models.Service2016Model;
using Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016
{
    public class ConsumersCategoryController : SchemeBoundController
    {
        private readonly INewRestService newRestService;

        private readonly IChangeLogService logService;

        private readonly ConsumersCategoryViewModel consumerCategoryModel = new ConsumersCategoryViewModel();

        public ConsumersCategoryController()
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
                    Data = newRestService.GetItems<F_F_GZYslPotr2016>()
                               .Where(x => x.RefFactGZ.ID == masterId)
                               .Select(item => new ConsumersCategoryViewModel
                               {
                                   ID = item.ID,
                                   RefFactGZ = item.RefFactGZ.ID,
                                   RefConsumersCategory = item.RefConsumersCategory.ID,
                                   RefConsumersCategoryName = item.RefConsumersCategory.Name
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
        public ActionResult Create(string data, int masterId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ValidateData(dataUpdate, masterId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<F_F_GZYslPotr2016>.DeserializeSingle(data);

                var msg = "Запись обновлена";
                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = newRestService.GetItem<F_F_GosZadanie2016>(masterId);
                record.RefConsumersCategory = newRestService.GetItem<F_F_ServiceConsumersCategory>(record.RefConsumersCategory.ID);

                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParameter);
                
                var returnData = newRestService.GetItems<F_F_GZYslPotr2016>()
                    .Where(v => v.ID == record.ID)
                    .Select(
                        item => new ConsumersCategoryViewModel
                        {
                            ID = item.ID,
                            RefFactGZ = item.RefFactGZ.ID,
                            RefConsumersCategory = item.RefConsumersCategory.ID,
                            RefConsumersCategoryName = item.RefConsumersCategory.Name
                        });
                
                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = returnData
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
            return newRestService.DeleteDocDetailAction<F_F_GZYslPotr2016>(id, docId);
        }

        public AjaxStoreResult GetCategory(int? limit, int? start, string query, int service)
        {
            var data = newRestService.GetItems<F_F_ServiceConsumersCategory>()
                .Where(x => x.RefService.ID.Equals(service) && x.Name.Contains(query))
                .Select(
                    p =>
                        new Service2016ConsumersCategoryViewModel
                            {
                                ID = p.ID,
                                Code = p.Code,
                                Name = p.Name
                            });

            return new AjaxStoreResult(data.Skip(start ?? 0).Take(limit ?? 10), data.Count());
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg2 = "Категория \"{0}\" уже указана<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => consumerCategoryModel.RefConsumersCategory))
            {
                message.Append(Msg.FormatWith(consumerCategoryModel.DescriptionOf(() => consumerCategoryModel.RefConsumersCategoryName)));
            }
            else
            {
                var category = record.GetValueToIntOrDefault(() => consumerCategoryModel.RefConsumersCategory, -1);

                if (newRestService.GetItems<F_F_GZYslPotr2016>().Any(x => x.ID != record.GetValueToIntOrDefault(() => consumerCategoryModel.ID, -1)
                                                                    && x.RefFactGZ.ID == masterId
                                                                    && x.RefConsumersCategory.ID == category))
                {
                    message.Append(Msg2.FormatWith(newRestService.Load<F_F_ServiceConsumersCategory>(category).Name));
                }
            }

            return message.ToString();
        }
    }
}
