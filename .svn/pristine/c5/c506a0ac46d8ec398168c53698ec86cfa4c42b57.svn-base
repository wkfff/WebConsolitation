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
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016
{
    public class ReportRequirementsController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public ReportRequirementsController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public RestResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_RequestAccount2016>()
                       where p.RefFactGZ.ID == masterId
                       select new ReportRequirementsViewModel
                       {
                           ID = p.ID,
                           PeriodicityTerm = p.PeriodicityTerm,
                           DeliveryTerm = p.DeliveryTerm,
                           OtherRequest = p.OtherRequest,
                           OtherIndicators = p.OtherIndicators,
                           ReportForm = p.ReportForm
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

                var record = JavaScriptDomainConverter<F_F_RequestAccount2016>.DeserializeSingle(data);

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
                    Data = from p in newRestService.GetItems<F_F_RequestAccount2016>()
                           where p.ID == record.ID
                           select new ReportRequirementsViewModel
                           {
                               ID = p.ID,
                               PeriodicityTerm = p.PeriodicityTerm,
                               DeliveryTerm = p.DeliveryTerm,
                               OtherRequest = p.OtherRequest,
                               OtherIndicators = p.OtherIndicators,
                               ReportForm = p.ReportForm
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
            return newRestService.DeleteDocDetailAction<F_F_RequestAccount2016>(id, masterId);
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string Empty = "Запись пуста<br>";
            const string EmptyField = "Поле \"{0}\" не заполнено<br>";
            const string DublicateMsg = "Запись уже заведена: \"{0}\"<br>";

            var message = new StringBuilder();
            var model = new ReportRequirementsViewModel();

            if (record.CheckNull(() => model.DeliveryTerm) && record.CheckNull(() => model.OtherRequest) &&
                record.CheckNull(() => model.ReportForm) && record.CheckNull(() => model.PeriodicityTerm) &&
                record.CheckNull(() => model.OtherIndicators))
            {
                message.Append(Empty);
            }

            if (record.CheckNull(() => model.PeriodicityTerm))
            {
                message.Append(EmptyField.FormatWith(UiBuilders.SchemeDescriptionOf(() => model.PeriodicityTerm)));
            }

            var id = record.GetValueToIntOrDefault(() => model.ID, -1);
            var deliveryTerm = record.GetValue(() => model.DeliveryTerm);
            var otherRequest = record.GetValue(() => model.OtherRequest);
            var reportForm = record.GetValue(() => model.ReportForm);
            var periodicityTerm = record.GetValue(() => model.PeriodicityTerm);
            var otherIndicators = record.GetValue(() => model.OtherIndicators);

            if (newRestService.GetItems<F_F_RequestAccount2016>().Any(
                x => x.ID != id && x.RefFactGZ.ID == masterId &&
                     x.DeliveryTerm == deliveryTerm &&
                     x.OtherRequest == otherRequest && x.ReportForm == reportForm &&
                     x.PeriodicityTerm == periodicityTerm && x.OtherIndicators == otherIndicators))
            {
                message.Append(DublicateMsg.FormatWith(deliveryTerm + " " + otherRequest));
            }

            return message.ToString();
        }
    }
}
