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
    public class Service2016LegalActController : SchemeBoundController
    {
        private readonly INewRestService newRestService;

        private readonly Service2016LegalActViewModel model = new Service2016LegalActViewModel();

        public Service2016LegalActController()
        {
            newRestService = Resolver.Get<INewRestService>();
        }

        public RestResult Read(int masterId)
        {
            return new RestResult
            {
                Success = true,
                Data = newRestService.GetItems<F_F_ServiceLegalAct>()
                        .Where(v => v.RefService.ID == masterId)
                        .Select(
                            v => new Service2016LegalActViewModel
                            {
                                ID = v.ID,
                                Name = v.Name,
                                EffectiveFrom = v.EffectiveFrom,
                                ApprovedBy = v.ApprovedBy,
                                ApprvdAt = v.ApprvdAt,
                                DatetEnd = v.DatetEnd,
                                Kind = v.Kind,
                                LANumber = v.LANumber,
                                MJnumber = v.MJnumber,
                                MJregdate = v.MJregdate
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

                var formData = JavaScriptDomainConverter<F_F_ServiceLegalAct>.DeserializeSingle(data);
                formData.RefService = newRestService.GetItem<D_Services_Service>(masterId);

                string msg = "Запись обновлена";
                if (formData.ID < 0)
                {
                    formData.ID = 0;
                    msg = "Новая запись добавлена";
                }

                newRestService.Save(formData);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = newRestService.GetItems<F_F_ServiceLegalAct>()
                            .Where(v => v.ID == formData.ID)
                            .Select(
                                v => new Service2016LegalActViewModel
                                {
                                    ID = v.ID,
                                    Name = v.Name,
                                    EffectiveFrom = v.EffectiveFrom,
                                    ApprovedBy = v.ApprovedBy,
                                    ApprvdAt = v.ApprvdAt,
                                    DatetEnd = v.DatetEnd,
                                    Kind = v.Kind,
                                    LANumber = v.LANumber,
                                    MJnumber = v.MJnumber,
                                    MJregdate = v.MJregdate
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
            return newRestService.DeleteAction<F_F_ServiceLegalAct>(id);
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg2 = "НПА \"{0}\" уже заведен<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => model.Name))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.Name)));
            }

            var la = newRestService.GetItems<F_F_ServiceLegalAct>().FirstOrDefault(
                x => !x.ID.Equals(record.GetValueToIntOrDefault(() => model.ID, -1))
                     && x.RefService.ID.Equals(masterId)
                     && x.Name.Equals(record.GetValue(() => model.Name)));

            if (la != null)
            {
                message.Append(Msg2.FormatWith(la.Name));
            }

            return message.ToString();
        }
    }
}
