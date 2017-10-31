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
    public class AveragePriceController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public AveragePriceController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }
        
        public RestResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_AveragePrice>()
                       where p.RefVolumeIndex.RefFactGZ.ID == masterId
                       select new AveragePriceViewModel
                       {
                           ID = p.ID,
                           RefVolumeIndex = p.RefVolumeIndex.ID,
                           RefVolumeIndexName = p.RefVolumeIndex.RefIndicators.Name,
                           ReportYearDec = p.ReportYearDec,
                           CurrentYearDec = p.CurrentYearDec,
                           NextYearDec = p.NextYearDec,
                           PlanFirstYearDec = p.PlanFirstYearDec,
                           PlanLastYearDec = p.PlanLastYearDec
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
                
                var validationError = ValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<F_F_AveragePrice>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }
                
                record.RefVolumeIndex = newRestService.GetItem<F_F_PNRZnach2016>(record.RefVolumeIndex.ID);

                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefVolumeIndex.RefFactGZ.RefParameter);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in newRestService.GetItems<F_F_AveragePrice>()
                           where p.ID == record.ID
                           select new AveragePriceViewModel
                           {
                               ID = p.ID,
                               RefVolumeIndex = p.RefVolumeIndex.ID,
                               RefVolumeIndexName = p.RefVolumeIndex.RefIndicators.Name,
                               ReportYearDec = p.ReportYearDec,
                               CurrentYearDec = p.CurrentYearDec,
                               NextYearDec = p.NextYearDec,
                               PlanFirstYearDec = p.PlanFirstYearDec,
                               PlanLastYearDec = p.PlanLastYearDec
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
            return newRestService.DeleteDocDetailAction<F_F_AveragePrice>(id, docId);
        }
        
        public AjaxStoreResult GetVolumeIndexes(int? limit, int? start, string query, int stateTask)
        {
            var data = newRestService.GetItems<F_F_PNRZnach2016>()
                .Where(
                    v => v.RefFactGZ.ID == stateTask
                         && v.RefIndicators.RefType.Code.Equals(FX_FX_CharacteristicType.VolumeIndex)
                         && v.RefIndicators.Name.Contains(query))
                .Select(
                    v => new
                        {
                            v.ID,
                            v.RefIndicators.Name
                        });

            return new AjaxStoreResult(data.Skip(start ?? 0).Take(limit ?? 10), data.Count());
        }

        private string ValidateData(JsonObject record)
        {
            const string RequireFiled = "Необходимо заполнить поле \"{0}\".<br>";
            const string Dublicate = "Показатель \"{0}\" уже указан.<br>";
            const string Empty = "Не заполнено ни одно значения.<br>";

            var message = new StringBuilder();
            var model = new AveragePriceViewModel();

            if (record.CheckNull(() => model.RefVolumeIndex))
            {
                message.Append(RequireFiled.FormatWith(model.DescriptionOf(() => model.RefVolumeIndexName)));
            }

            if (record.CheckNull(() => model.CurrentYearDec) && record.CheckNull(() => model.NextYearDec) &&
                record.CheckNull(() => model.PlanFirstYearDec) && record.CheckNull(() => model.PlanLastYearDec) &&
                record.CheckNull(() => model.ReportYearDec))
            {
                message.Append(Empty);
            }

            var id = record.GetValueToIntOrDefault(() => model.ID, -1);
            var volumeIndex = record.GetValueToIntOrDefault(() => model.RefVolumeIndex, -1);
            if (newRestService.GetItems<F_F_AveragePrice>().Any(x => x.ID != id && x.RefVolumeIndex.ID == volumeIndex))
            {
                message.Append(Dublicate.FormatWith(newRestService.Load<F_F_PNRZnach2016>(volumeIndex).RefIndicators.Name));
            }

            return message.ToString();
        }
    }
}
