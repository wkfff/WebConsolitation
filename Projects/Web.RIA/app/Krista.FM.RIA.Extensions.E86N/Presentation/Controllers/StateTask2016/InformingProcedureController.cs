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
    public class InformingProcedureController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public InformingProcedureController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public RestResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_InfoProcedure2016>()
                       where p.RefFactGZ.ID == masterId
                       select new InformingProcedureViewModel
                       {
                           ID = p.ID,
                           Content = p.Content,
                           Method = p.Method,
                           Rate = p.Rate
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

                var record = JavaScriptDomainConverter<F_F_InfoProcedure2016>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = newRestService.GetItem<F_F_GosZadanie2016>(masterId);

                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParameter);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in newRestService.GetItems<F_F_InfoProcedure2016>()
                           where p.ID == record.ID
                           select new InformingProcedureViewModel
                           {
                               ID = p.ID,
                               Content = p.Content,
                               Method = p.Method,
                               Rate = p.Rate
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult Delete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_InfoProcedure2016>(id, docId);
        }

        public string ValidateData(JsonObject record, int masterId)
        {
            const string RequireFieldMsg = "Необходимо заполнить поле: {0}<br>";
            const string DublicateMsg = "Запись уже заведена: \"{0}\"<br>";

            var message = new StringBuilder();
            var model = new InformingProcedureViewModel();

            if (record.CheckNull(() => model.Content))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Content)));
            }

            if (record.CheckNull(() => model.Rate))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Rate)));
            }

            if (record.CheckNull(() => model.Method))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Method)));
            }

            var id = record.GetValueToIntOrDefault(() => model.ID, -1);
            var content = record.GetValue(() => model.Content);
            var rate = record.GetValue(() => model.Rate);
            var method = record.GetValue(() => model.Method);

            if (newRestService.GetItems<F_F_InfoProcedure2016>().Any(
                x => x.ID != id && x.RefFactGZ.ID == masterId &&
                     x.Content == content && x.Rate == rate && x.Method == method))
            {
                message.Append(DublicateMsg.FormatWith(content + " " + rate + " " + method));
            }

            return message.ToString();
        }
    }
}
