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
    public class StateTask2016ReportsController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public StateTask2016ReportsController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }
        
        public RestResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_Reports>()
                       where p.RefFactGZ.ID == masterId
                       select new ReportsViewModel
                       {
                           ID = p.ID,
                           ReportGuid = p.ReportGuid,
                           NameReport = p.NameReport,
                           HeadName = p.HeadName,
                           HeadPosition = p.HeadPosition,
                           DateReport = p.DateReport
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

                var record = JavaScriptDomainConverter<F_F_Reports>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                    record.ReportGuid = Guid.NewGuid().ToString();
                }
                else if (ReportIsUse(record.ID))
                        {
                            return new RestResult { Success = false, Message = MessageIsUse(false) };
                        }

                record.RefFactGZ = newRestService.GetItem<F_F_ParameterDoc>(masterId);
                
                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in newRestService.GetItems<F_F_Reports>()
                           where p.ID == record.ID
                           select new ReportsViewModel
                           {
                               ID = p.ID,
                               ReportGuid = p.ReportGuid,
                               NameReport = p.NameReport,
                               HeadName = p.HeadName,
                               HeadPosition = p.HeadPosition,
                               DateReport = p.DateReport
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
            try
            {
                if (ReportIsUse(id))
                {
                    return new RestResult { Success = false, Message = MessageIsUse(true) };
                }

                newRestService.BeginTransaction();

                newRestService.Delete<F_F_Reports>(id);
                
                logService.WriteDeleteDocDetail(newRestService.GetItem<F_F_ParameterDoc>(masterId));

                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (newRestService.HaveTransaction)
                {
                    newRestService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteDocDetailAction: " + e.Message + " : " + e.ExpandException());

                if (newRestService.HaveTransaction)
                {
                    newRestService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        private static string MessageIsUse(bool isDelete)
        {
            return string.Concat("Отчет используется в показателях, ", isDelete ? "удаление " : "изменение ", "запрещено!");
        }

        private bool ReportIsUse(int id)
        {
            return newRestService.GetItems<F_F_PNRZnach2016>().Any(x => x.RefReport.ID == id);
        }
        
        private string ValidateData(JsonObject record, int masterId)
        {
            const string RequireFieldMsg = "Необходимо заполнить поле: {0}<br>";
            const string DublicateMsg = "Запись уже заведена: \"{0}\"<br>";

            var model = new ReportsViewModel();
            var message = new StringBuilder();

            if (record.CheckNull(() => model.NameReport))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.NameReport)));
            }

            DateTime? dateReport = null;
            if (record.CheckNull(() => model.DateReport))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.DateReport)));
            }
            else
            {
                dateReport = Convert.ToDateTime(record.GetValue(() => model.DateReport));
                if (dateReport > DateTime.Today)
                {
                    message.Append("Некорректная дата<br>");
                }
            }

            if (record.CheckNull(() => model.HeadName))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.HeadName)));
            }

            if (record.CheckNull(() => model.HeadPosition))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.HeadPosition)));
            }

            var id = record.GetValueToIntOrDefault(() => model.ID, -1);
            var nameReport = record.GetValue(() => model.NameReport);
            var headName = record.GetValue(() => model.HeadName);
            var headPosition = record.GetValue(() => model.HeadPosition);

            if (newRestService.GetItems<F_F_Reports>().Any(
                x => x.ID != id && x.RefFactGZ.ID == masterId &&
                     x.NameReport == nameReport &&
                     x.DateReport == dateReport &&
                     x.HeadName == headName &&
                     x.HeadPosition == headPosition))
            {
                message.Append(DublicateMsg.FormatWith(nameReport));
            }

            return message.ToString();
        }
    }
}
