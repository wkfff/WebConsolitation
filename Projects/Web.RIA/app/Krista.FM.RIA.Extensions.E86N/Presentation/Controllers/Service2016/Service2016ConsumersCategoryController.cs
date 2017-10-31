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
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Service2016
{
    public class Service2016ConsumersCategoryController : SchemeBoundController
    {
        private readonly INewRestService newRestService;

        private readonly Service2016ConsumersCategoryViewModel model = new Service2016ConsumersCategoryViewModel();

        public Service2016ConsumersCategoryController()
        {
            newRestService = Resolver.Get<INewRestService>();
        }

        public RestResult Read(int masterId)
        {
            return new RestResult
            {
                Success = true,
                Data = newRestService.GetItems<F_F_ServiceConsumersCategory>()
                        .Where(v => v.RefService.ID == masterId)
                        .Select(
                            v => new Service2016ConsumersCategoryViewModel
                            {
                                ID = v.ID,
                                Code = v.Code,
                                Name = v.Name
                            })
            };
        }

        [HttpPost]
        [Transaction]
        public RestResult Create(string data, int masterId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ValidateData(dataUpdate, masterId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var formData = JavaScriptDomainConverter<F_F_ServiceConsumersCategory>.DeserializeSingle(data);

                formData.RefService = newRestService.GetItem<D_Services_Service>(masterId);

                string msg = "Запись обновлена";
                if (formData.ID < 0)
                {
                    formData.ID = 0;
                    msg = "Новая запись добавлена";
                }
                else if (ServiceIsUse(masterId))
                {
                    return new RestResult { Success = false, Message = Service2016Controller.MessageIsUse(false) };
                }

                newRestService.Save(formData);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = newRestService.GetItems<F_F_ServiceConsumersCategory>()
                            .Where(v => v.ID == formData.ID)
                            .Select(
                                v => new Service2016ConsumersCategoryViewModel
                                {
                                    ID = v.ID,
                                    Name = v.Name,
                                    Code = v.Code
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
                if (ServiceIsUse(newRestService.GetItem<F_F_ServiceConsumersCategory>(id).RefService.ID))
                {
                    return new RestResult { Success = false, Message = Service2016Controller.MessageIsUse(false) };
                }

                newRestService.BeginTransaction();
                
                newRestService.Delete<F_F_ServiceConsumersCategory>(id);

                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (newRestService.HaveTransaction)
                {
                    newRestService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteAction: " + e.Message + " : " + e.ExpandException());

                if (newRestService.HaveTransaction)
                {
                    newRestService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg2 = "Категория потребителей \"{0}\": \"{1}\" уже заведена<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => model.Name))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.Name)));
            }

            if (record.CheckNull(() => model.Code))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.Code)));
            }

            var consCateg = newRestService.GetItems<F_F_ServiceConsumersCategory>().FirstOrDefault(
                x => !x.ID.Equals(record.GetValueToIntOrDefault(() => model.ID, -1))
                     && x.RefService.ID.Equals(masterId)
                     && x.Name.Equals(record.GetValue(() => model.Name))
                     && x.Code.Equals(record.GetValue(() => model.Code)));

            if (consCateg != null)
            {
                message.Append(Msg2.FormatWith(consCateg.Name, consCateg.Code));
            }

            return message.ToString();
        }

        private bool ServiceIsUse(int id)
        {
            return newRestService.GetItems<F_F_GosZadanie2016>().Any(v => v.RefService.ID == id);
        }
    }
}
