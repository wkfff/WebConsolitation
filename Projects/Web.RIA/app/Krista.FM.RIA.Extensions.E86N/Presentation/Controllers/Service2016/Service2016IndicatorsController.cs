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
    public class Service2016IndicatorsController : SchemeBoundController
    {
        private readonly INewRestService newRestService;

        private readonly Service2016IndicatorsViewModel model = new Service2016IndicatorsViewModel();

        public Service2016IndicatorsController()
        {
            newRestService = Resolver.Get<INewRestService>();
        }
        
        public RestResult Read(int masterId)
        {
            return new RestResult
            {
                Success = true,
                Data = newRestService.GetItems<F_F_ServiceIndicators>()
                        .Where(v => v.RefService.ID == masterId)
                        .Select(
                            v => new Service2016IndicatorsViewModel
                            {
                                ID = v.ID,
                                Code = v.Code,
                                Name = v.Name,
                                RefIndType = v.RefType.ID,
                                RefIndTypeName = v.RefType.Name,
                                RefOKEI = v.RefOKEI.ID,
                                RefOKEIName = v.RefOKEI.Name
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

                var formData = JavaScriptDomainConverter<F_F_ServiceIndicators>.DeserializeSingle(data);

                formData.RefService = newRestService.GetItem<D_Services_Service>(masterId);
                formData.RefOKEI = newRestService.GetItem<D_Org_OKEI>(formData.RefOKEI.ID);

                // Из-за неккоректной работы SetComboBoxEditor, произошло несогласование имени ссылки на тип показателя
                formData.RefType = newRestService.GetItem<FX_FX_CharacteristicType>(Convert.ToInt32(dataUpdate["RefIndType"]));

                string msg = "Запись обновлена";
                if (formData.ID < 0)
                {
                    formData.ID = 0;
                    msg = "Новая запись добавлена";
                }
                else if (Service2016Controller.ServiceIsUse(masterId))
                {
                    return new RestResult { Success = false, Message = Service2016Controller.MessageIsUse(false) };
                }

                newRestService.Save(formData);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = newRestService.GetItems<F_F_ServiceIndicators>()
                            .Where(v => v.ID == formData.ID)
                            .Select(
                                v => new Service2016IndicatorsViewModel
                                {
                                    ID = v.ID,
                                    Code = v.Code,
                                    Name = v.Name,
                                    RefIndType = v.RefType.ID,
                                    RefIndTypeName = v.RefType.Name,
                                    RefOKEI = v.RefOKEI.ID,
                                    RefOKEIName = v.RefOKEI.Name
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
                if (Service2016Controller.ServiceIsUse(newRestService.GetItem<F_F_ServiceIndicators>(id).RefService.ID))
                {
                    return new RestResult { Success = false, Message = Service2016Controller.MessageIsUse(false) };
                }

                newRestService.BeginTransaction();
                
                newRestService.Delete<F_F_ServiceIndicators>(id);

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
            const string Msg2 = "Показатель \"{0}\": \"{1}\": \"{2}\" уже заведен<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => model.Name))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.Name)));
            }

            if (record.CheckNull(() => model.RefIndType))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.RefIndTypeName)));
            }

            if (record.CheckNull(() => model.RefOKEI))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.RefOKEIName)));
            }

            var indicator = newRestService.GetItems<F_F_ServiceIndicators>().FirstOrDefault(
                x => !x.ID.Equals(record.GetValueToIntOrDefault(() => model.ID, -1))
                     && x.RefService.ID.Equals(masterId)
                     && x.Name.Equals(record.GetValue(() => model.Name))
                     && x.RefType.ID.Equals(record.GetValueToIntOrDefault(() => model.RefIndType, -1))
                     && x.RefOKEI.ID.Equals(record.GetValueToIntOrDefault(() => model.RefOKEI, -1)));

            if (indicator != null)
            {
                message.Append(Msg2.FormatWith(indicator.Name, indicator.RefType.Name, indicator.RefOKEI.Name));
            }

            return message.ToString();
        }
    }
}
