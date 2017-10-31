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
using Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016
{
    public class SupervisionProcedureController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public SupervisionProcedureController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public RestResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_OrderControl2016>()
                       where p.RefFactGZ.ID == masterId
                       select new SupervisionProcedureViewModel
                       {
                           ID = p.ID,
                           Form = p.Form,
                           Rate = p.Rate,
                           Supervisor = p.Supervisor
                       };

            return new RestResult { Success = true, Data = data };
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

                var record = JavaScriptDomainConverter<F_F_OrderControl2016>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = newRestService.GetItem<F_F_ParameterDoc>(masterId);

                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in newRestService.GetItems<F_F_OrderControl2016>()
                           where p.ID == record.ID
                           select new SupervisionProcedureViewModel
                           {
                               ID = p.ID,
                               Form = p.Form,
                               Rate = p.Rate,
                               Supervisor = p.Supervisor
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult Delete(int id, int masterId)
        {
            return newRestService.DeleteDocDetailAction<F_F_OrderControl2016>(id, masterId);
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string RequireFieldMsg = "Необходимо заполнить поле: {0}<br>";
            const string DublicateMsg = "Запись уже заведена: \"{0}\"<br>";

            var message = new StringBuilder();
            var model = new SupervisionProcedureViewModel();

            if (record.CheckNull(() => model.Form))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Form)));
            }

            if (record.CheckNull(() => model.Rate))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Rate)));
            }

            if (record.CheckNull(() => model.Supervisor))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Supervisor)));
            }
            
            var id = record.GetValueToIntOrDefault(() => model.ID, -1);
            var form = record.GetValue(() => model.Form);
            var rate = record.GetValue(() => model.Rate);
            var supervisor = record.GetValue(() => model.Supervisor);

            if (newRestService.GetItems<F_F_OrderControl2016>().Any(
                x => x.ID != id && x.RefFactGZ.ID == masterId &&
                     x.Form == form && x.Rate == rate && x.Supervisor == supervisor))
            {
                message.Append(DublicateMsg.FormatWith(form + " " + rate + " " + supervisor));
            }

            return message.ToString();
        }
    }
}
