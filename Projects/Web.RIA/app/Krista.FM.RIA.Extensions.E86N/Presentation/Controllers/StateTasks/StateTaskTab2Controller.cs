using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Services.StateTaskService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTasks
{
    public class StateTaskTab2Controller : SchemeBoundController
    {
        private readonly IChangeLogService logService;

        private readonly INewRestService newRestService;

        public StateTaskTab2Controller()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public ActionResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_PNRZnach>()
                       where p.RefFactGZ.ID == masterId
                       select new
                           {
                               p.ID,
                               p.ActualValue,
                               p.ReportingYear,
                               p.ComingYear,
                               p.FirstPlanYear,
                               p.Protklp,
                               p.CurrentYear,
                               p.SecondPlanYear,
                               p.Info,
                               p.Source,
                               p.SourceInfFact,
                               RefIndicators = p.RefIndicators.ID,
                               RefIndicatorsName = p.RefIndicators.Name,
                               PnrOkei = p.RefIndicators.RefOKEI.Name,
                               PnrType = p.RefIndicators.RefCharacteristicType.Name
                           };

            return new AjaxStoreResult(data, data.Count());
        }

        [HttpPost]
        [Transaction]
        public RestResult Save(string data, int masterId)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate, masterId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_F_PNRZnach record = JavaScriptDomainConverter<F_F_PNRZnach>.DeserializeSingle(data);

                string msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = newRestService.GetItem<F_F_GosZadanie>(masterId);
                record.RefIndicators = newRestService.GetItem<D_Services_Indicators>(record.RefIndicators.ID);

                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParametr);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = from p in newRestService.GetItems<F_F_PNRZnach>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID,
                                       p.ActualValue,
                                       p.ReportingYear,
                                       p.ComingYear,
                                       p.FirstPlanYear,
                                       p.Protklp,
                                       p.CurrentYear,
                                       p.SecondPlanYear,
                                       p.Info,
                                       p.Source,
                                       p.SourceInfFact,
                                       RefIndicators = p.RefIndicators.ID,
                                       RefIndicatorsName = p.RefIndicators.Name,
                                       PnrOkei = p.RefIndicators.RefOKEI.Name,
                                       PnrType = p.RefIndicators.RefCharacteristicType.Name
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
            return newRestService.DeleteDocDetailAction<F_F_PNRZnach>(id, docId);
        }

        public AjaxStoreResult GetIndicators(int? limit, int? start, string query, int service)
        {
            var data = newRestService.GetItems<F_F_PNRysl>().Where(x => x.RefPerV.ID.Equals(service) &&
                                                                                    x.RefIndicators.Name.Contains(query))
                                                            .Select(p => new
                                                            {
                                                                p.RefIndicators.ID,
                                                                p.RefIndicators.Name,
                                                                RefOKEI = p.RefIndicators.RefOKEI.Name,
                                                                RefCharacteristicType = p.RefIndicators.RefCharacteristicType.Name
                                                            });

            return new AjaxStoreResult(data.Skip(start ?? 0).Take(limit ?? 10), data.Count());
        }

        protected string ValidateData(JsonObject record, int masterId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg1 = "Поле \"Причина отклонения\" не должно быть заполнено<br>";
            const string Msg2 = "Показатель\"{0}\" уже указан<br>";

            var message = string.Empty;
            var duplication = false;

            if (record.GetValue(() => IndicatorsOfServiceFields.RefIndicators)
                                .IsNullOrEmpty())
            {
                message += Msg.FormatWith(StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.RefIndicatorsName));
            }
            else
            {
                var indicator = record.GetValueToIntOrDefault(() => IndicatorsOfServiceFields.RefIndicators, -1);
                var id = record.GetValueToIntOrDefault(() => IndicatorsOfServiceFields.ID, -1);

                if (newRestService.GetItems<F_F_PNRZnach>().Any(x => (x.ID != id) && (x.RefFactGZ.ID == masterId) && (x.RefIndicators.ID == indicator)))
                {
                    duplication = true;
                    message += Msg2.FormatWith(newRestService.Load<D_Services_Indicators>(indicator).Name);
                }
            }

            // если дубль то не проверяем все остальное
            if (!duplication)
            {
                var service = newRestService.GetItem<F_F_GosZadanie>(masterId);
                if (service.RefVedPch.RefTipY.ID == D_Services_TipY.FX_FX_SERVICE)
                {
                    if (record.GetValue(() => IndicatorsOfServiceFields.ComingYear)
                        .Trim().IsNullOrEmpty())
                    {
                        message += Msg.FormatWith(StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.ComingYear));
                    }
                }

                // если фактическое значение указано
                if (record.GetValue(() => IndicatorsOfServiceFields.ActualValue)
                        .Trim().IsNotNullOrEmpty())
                {
                    // фактическое значение отличное от очередного года
                    if (record.GetValue(() => IndicatorsOfServiceFields.ActualValue)
                        .Trim() != record.GetValue(() => IndicatorsOfServiceFields.ComingYear)
                                        .Trim())
                    {
                        // если Причина отклонения не заполнено
                        if (record.GetValue(() => IndicatorsOfServiceFields.Protklp)
                                .Trim().IsNullOrEmpty())
                        {
                            message += "Фактическое значение отличается от планового." +
                                       Msg.FormatWith(StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.Protklp));
                        }
                    }
                    else
                    {
                        // если фактическое значение указано и оно идентично очередному году
                        // если Причина отклонения заполнено
                        if (record.GetValue(() => IndicatorsOfServiceFields.Protklp)
                            .Trim().IsNotNullOrEmpty())
                        {
                            message += Msg1;
                        }
                    }
                }
                else
                {
                    // если фактическое значение не указано
                    // если Причина отклонения заполнена
                    if (record.GetValue(() => IndicatorsOfServiceFields.Protklp)
                            .Trim().IsNotNullOrEmpty())
                    {
                        message += Msg1;
                    }
                }
            }

            return message;
        }
    }
}
