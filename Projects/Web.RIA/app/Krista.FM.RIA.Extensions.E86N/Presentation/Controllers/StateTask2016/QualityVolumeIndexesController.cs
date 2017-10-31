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
using Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016
{
    public class QualityVolumeIndexesController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public QualityVolumeIndexesController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public ActionResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_PNRZnach2016>()
                       where p.RefFactGZ.ID == masterId
                       select new QualityVolumeIndexesViewModel
                       {
                           ID = p.ID,
                           ActualValue = p.ActualValue,
                           ReportingYear = p.ReportingYear,
                           ComingYear = p.ComingYear,
                           FirstPlanYear = p.FirstPlanYear,
                           Protklp = p.Protklp,
                           CurrentYear = p.CurrentYear,
                           SecondPlanYear = p.SecondPlanYear,
                           RefIndicators = p.RefIndicators.ID,
                           RefIndicatorsName = p.RefIndicators.Name,
                           RefIndicatorsCode = p.RefIndicators.Code,
                           RefIndicatorsType = p.RefIndicators.RefType.ID,
                           RefIndicatorsTypeName = p.RefIndicators.RefType.Name,
                           RefIndicatorsTypeCode = p.RefIndicators.RefType.Code,
                           RefIndicatorsOKEI = p.RefIndicators.RefOKEI.ID,
                           RefIndicatorsOKEICode = p.RefIndicators.RefOKEI.Code,
                           RefIndicatorsOKEIName = p.RefIndicators.RefOKEI.Name,
                           Reject = p.Reject,
                           Deviation = p.Deviation,
                           RefReport = p.RefReport != null ? p.RefReport.ID : (int?)null,
                           RefReportName = p.RefReport != null ? p.RefReport.NameReport : null,
                           AveragePriceFact = p.AveragePriceFact
                       };

            return new AjaxStoreResult(data, data.Count());
        }

        public AjaxStoreResult GetIndicators(int? limit, int? start, string query, int service)
        {
            var data = newRestService.GetItems<F_F_ServiceIndicators>()
                .Where(v => v.RefService.ID == service && v.RefType.Name.Contains(query))
                .Select(
                    v => new
                    {
                        v.ID,
                        v.Name,
                        RefType = v.RefType.Name,
                        RefOKEI = v.RefOKEI.Name
                    });

            return new AjaxStoreResult(data.Skip(start ?? 0).Take(limit ?? 10), data.Count());
        }

        public AjaxStoreResult GetReports(int? limit, int? start, string query, int docId)
        {
            var data = newRestService.GetItems<F_F_Reports>()
                .Where(v => v.RefFactGZ.ID == docId && v.NameReport.Contains(query))
                .Select(
                    v => new
                    {
                        v.ID,
                        v.NameReport
                    });

            return new AjaxStoreResult(data.Skip(start ?? 0).Take(limit ?? 10), data.Count());
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

                var record = JavaScriptDomainConverter<F_F_PNRZnach2016>.DeserializeSingle(data);
                
                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                // недопустимо сохранять пробелы
                record.ActualValue = record.ActualValue.Trim();

                record.RefFactGZ = newRestService.GetItem<F_F_GosZadanie2016>(masterId);
                record.RefIndicators = newRestService.GetItem<F_F_ServiceIndicators>(record.RefIndicators.ID);
                record.RefReport = record.RefReport != null ? newRestService.GetItem<F_F_Reports>(record.RefReport.ID) : null;
                
                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParameter);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in newRestService.GetItems<F_F_PNRZnach2016>()
                           where p.ID == record.ID
                           select new QualityVolumeIndexesViewModel
                           {
                               ID = p.ID,
                               ActualValue = p.ActualValue,
                               ReportingYear = p.ReportingYear,
                               ComingYear = p.ComingYear,
                               FirstPlanYear = p.FirstPlanYear,
                               Protklp = p.Protklp,
                               CurrentYear = p.CurrentYear,
                               SecondPlanYear = p.SecondPlanYear,
                               RefIndicators = p.RefIndicators.ID,
                               RefIndicatorsName = p.RefIndicators.Name,
                               RefIndicatorsCode = p.RefIndicators.Code,
                               RefIndicatorsType = p.RefIndicators.RefType.ID,
                               RefIndicatorsTypeName = p.RefIndicators.RefType.Name,
                               RefIndicatorsTypeCode = p.RefIndicators.RefType.Code,
                               RefIndicatorsOKEI = p.RefIndicators.RefOKEI.ID,
                               RefIndicatorsOKEICode = p.RefIndicators.RefOKEI.Code,
                               RefIndicatorsOKEIName = p.RefIndicators.RefOKEI.Name,
                               Reject = p.Reject,
                               Deviation = p.Deviation,
                               RefReport = p.RefReport != null ? p.RefReport.ID : (int?)null,
                               RefReportName = p.RefReport != null ? p.RefReport.NameReport : null,
                               AveragePriceFact = p.AveragePriceFact
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
            try
            {
                newRestService.BeginTransaction();

                newRestService.GetItems<F_F_AveragePrice>().Where(x => x.RefVolumeIndex.ID == id).Each(newRestService.Delete);
                newRestService.Delete<F_F_PNRZnach2016>(id);
                
                logService.WriteDeleteDocDetail(newRestService.GetItem<F_F_ParameterDoc>(docId));

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
        
        protected string ValidateData(JsonObject record, int masterId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg1 = "Поле \"{0}\" не должно быть заполнено<br>";
            const string Msg2 = "Показатель\"{0}\" уже указан<br>";

            var model = new QualityVolumeIndexesViewModel();

            var message = string.Empty;
            var duplication = false;

            if (record.GetValue(() => model.RefIndicators)
                                .IsNullOrEmpty())
            {
                message += Msg.FormatWith(model.DescriptionOf(() => model.RefIndicatorsName));
            }
            else
            {
                var indicator = record.GetValueToIntOrDefault(() => model.RefIndicators, -1);
                var id = record.GetValueToIntOrDefault(() => model.ID, -1);

                if (newRestService.GetItems<F_F_PNRZnach2016>().Any(x => (x.ID != id) && (x.RefFactGZ.ID == masterId) &&
                    x.RefIndicators.ID == indicator))
                {
                    duplication = true;
                    message += Msg2.FormatWith(newRestService.Load<F_F_ServiceIndicators>(indicator).Name);
                }
            }

            // если дубль то не проверяем все остальное
            if (!duplication)
            {
                if (record.GetValue(() => model.ComingYear)
                   .Trim().IsNullOrEmpty())
                {
                    message += Msg.FormatWith(model.DescriptionOf(() => model.ComingYear));
                }

                var payable = newRestService.GetItem<F_F_GosZadanie2016>(masterId).RefService.RefPay.Code.Equals(FX_FX_ServicePayType2.CodeOfPayable);

                // Средняя цена, фактическое значение
                if (!record.GetValue(() => model.AveragePriceFact)
                    .Trim().IsNullOrEmpty() && !payable)
                {
                    message += Msg1.FormatWith(model.DescriptionOf(() => model.AveragePriceFact));
                }

                // если фактическое значение указано
                if (record.GetValue(() => model.ActualValue)
                        .Trim().IsNotNullOrEmpty())
                {
                    // фактическое значение отличное от очередного года
                    if (record.GetValue(() => model.ActualValue)
                        .Trim() != record.GetValue(() => model.ComingYear)
                                        .Trim())
                    {
                        // если Причина отклонения не заполнено
                        if (record.GetValue(() => model.Protklp)
                                .Trim().IsNullOrEmpty())
                        {
                            message += "Фактическое значение отличается от планового." +
                                       Msg.FormatWith(model.DescriptionOf(() => model.Protklp));
                        }
                        
                        // если Отклонение не заполнено
                        if (record.GetValue(() => model.Reject)
                            .Trim().IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(model.DescriptionOf(() => model.Reject));
                        }
                    }
                    else
                    {
                        // если фактическое значение указано и оно идентично очередному году
                        
                        // если Причина отклонения заполнено
                        if (record.GetValue(() => model.Protklp)
                            .Trim().IsNotNullOrEmpty())
                        {
                            message += Msg1.FormatWith(model.DescriptionOf(() => model.Protklp));
                        }

                        // если Отклонение заполнено
                        if (record.GetValue(() => model.Reject)
                        .Trim().IsNotNullOrEmpty())
                        {
                            message += Msg1.FormatWith(model.DescriptionOf(() => model.Reject));
                        }
                    }
                }
                else
                {
                    // если фактическое значение не указано
                    
                    // если Причина отклонения заполнена
                    if (record.GetValue(() => model.Protklp)
                            .Trim().IsNotNullOrEmpty())
                    {
                        message += Msg1.FormatWith(model.DescriptionOf(() => model.Protklp));
                    }

                    // если Отклонение заполнено
                    if (record.GetValue(() => model.Reject)
                        .Trim().IsNotNullOrEmpty())
                    {
                        message += Msg1.FormatWith(model.DescriptionOf(() => model.Reject));
                    }
                }
            }

            return message;
        }
    }
}
