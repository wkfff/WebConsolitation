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
    public class RenderEnactmentController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public RenderEnactmentController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }
        
        public RestResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_NPARenderOrder2016>()
                       where p.RefFactGZ.ID == masterId
                       select new RenderEnactmentViewModel
                       {
                           ID = p.ID,
                           RenderEnact = p.RenderEnact,
                           DateNpa = p.DateNpa,
                           TypeNpa = p.TypeNpa,
                           NumberNpa = p.NumberNpa,
                           Author = p.Author
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

                var record = JavaScriptDomainConverter<F_F_NPARenderOrder2016>.DeserializeSingle(data);

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
                    Data = from p in newRestService.GetItems<F_F_NPARenderOrder2016>()
                           where p.ID == record.ID
                           select new RenderEnactmentViewModel
                           {
                               ID = p.ID,
                               RenderEnact = p.RenderEnact,
                               DateNpa = p.DateNpa,
                               TypeNpa = p.TypeNpa,
                               NumberNpa = p.NumberNpa,
                               Author = p.Author
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
            return newRestService.DeleteDocDetailAction<F_F_NPARenderOrder2016>(id, docId);
        }

        public string ValidateData(JsonObject record, int masterId)
        {
            const string RequireFieldMsg = "Необходимо заполнить поле: {0}<br>";
            const string DublicateMsg = "Запись уже заведена: \"{0}\"<br>";

            var model = new RenderEnactmentViewModel();
            var message = new StringBuilder();

            if (record.CheckNull(() => model.RenderEnact))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.RenderEnact)));
            }

            DateTime? dateNpa = null;
            if (record.CheckNull(() => model.DateNpa))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.DateNpa)));
            }
            else
            {
                dateNpa = Convert.ToDateTime(record.GetValue(() => model.DateNpa));
                if (dateNpa > DateTime.Today)
                {
                    message.Append("Некорректная дата НПА<br>");
                }
            }

            if (record.CheckNull(() => model.TypeNpa))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.TypeNpa)));
            }

            if (record.CheckNull(() => model.NumberNpa))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.NumberNpa)));
            }

            if (record.CheckNull(() => model.Author))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Author)));
            }

            var id = record.GetValueToIntOrDefault(() => model.ID, -1);
            var renderEnact = record.GetValue(() => model.RenderEnact);
            var typeNpa = record.GetValue(() => model.TypeNpa);
            var numberNpa = record.GetValue(() => model.NumberNpa);
            var author = record.GetValue(() => model.Author);

            if (newRestService.GetItems<F_F_NPARenderOrder2016>().Any(
                x => x.ID != id && x.RefFactGZ.ID == masterId &&
                     x.RenderEnact == renderEnact &&
                     x.TypeNpa == typeNpa &&
                     x.NumberNpa == numberNpa &&
                     x.DateNpa == dateNpa &&
                     x.Author == author))
            {
                message.Append(DublicateMsg.FormatWith(renderEnact));
            }

            return message.ToString();
        }
    }
}
