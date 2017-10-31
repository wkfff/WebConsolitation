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
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;
using NHibernate.Linq;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для представления "Показатели объема и качества услуг\работ справочник"
    /// </summary>
    public class IndicatorsServiceController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IndicatorsServiceModel model = new IndicatorsServiceModel();

        public IndicatorsServiceController(INewRestService newRestService)
        {
            this.newRestService = newRestService;
        }

        public AjaxStoreResult Read([FiltersBinder] FilterConditions filters, int limit, int start)
        {
            var data = newRestService.GetItems<D_Services_Indicators>().Select(
                x =>
                new IndicatorsServiceModel
                    {
                        ID = x.ID,
                        Name = x.Name,
                        RefCharacteristicType = x.RefCharacteristicType.ID,
                        RefCharacteristicTypeName = x.RefCharacteristicType.Name,
                        RefOKEI = x.RefOKEI.ID,
                        RefOKEIName = x.RefOKEI.Name
                    }).ToList();

            filters.Conditions
                .ForEach(
                    filter =>
                    {
                        if (filter.Name == model.NameOf(() => model.Name))
                        {
                            data = data.Where(x => x.Name.Contains(filter.Value)).ToList();
                        }

                        if (filter.Name == model.NameOf(() => model.RefCharacteristicTypeName))
                        {
                            data = data.Where(x => x.RefCharacteristicTypeName.Contains(filter.Value)).ToList();
                        }

                        if (filter.Name == model.NameOf(() => model.RefOKEIName))
                        {
                            data = data.Where(x => x.RefOKEIName.Contains(filter.Value)).ToList();
                        }
                    });

            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }

        [HttpPost]
        [Transaction]
        public RestResult Save(string data)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<D_Services_Indicators>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefCharacteristicType = newRestService.GetItem<FX_FX_CharacteristicType>(record.RefCharacteristicType.ID);
                record.RefOKEI = newRestService.GetItem<D_Org_OKEI>(record.RefOKEI.ID);

                newRestService.Save(record);
               
                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = newRestService.GetItems<D_Services_Indicators>()
                            .Where(p => p.ID == record.ID)
                            .Select(
                                x => new IndicatorsServiceModel
                                {
                                    ID = x.ID,
                                    Name = x.Name,
                                    RefCharacteristicType = x.RefCharacteristicType.ID,
                                    RefCharacteristicTypeName = x.RefCharacteristicType.Name,
                                    RefOKEI = x.RefOKEI.ID,
                                    RefOKEIName = x.RefOKEI.Name
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
            return newRestService.DeleteAction<D_Services_Indicators>(id);
        }

        public string ValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg2 = "Показатель\"{0}\": \"{1}\": \"{2}\" уже заведен<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => model.Name))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.Name)));
            }

            if (record.CheckNull(() => model.RefCharacteristicType))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.RefCharacteristicTypeName)));
            }

            if (record.CheckNull(() => model.RefOKEI))
            {
                message.Append(Msg.FormatWith(model.DescriptionOf(() => model.RefOKEIName)));
            }

            var indicator = newRestService.GetItems<D_Services_Indicators>().FirstOrDefault(
                x => !x.ID.Equals(record.GetValueToIntOrDefault(() => model.ID, -1))
                     && x.Name.Equals(record.GetValue(() => model.Name))
                     && x.RefOKEI.ID.Equals(record.GetValueToIntOrDefault(() => model.RefOKEI, -1))
                     && x.RefCharacteristicType.ID.Equals(record.GetValueToIntOrDefault(() => model.RefCharacteristicType, -1)));

            if (indicator != null)
            {
                message.Append(Msg2.FormatWith(indicator.Name, indicator.RefCharacteristicType.Name, indicator.RefOKEI.Name));
            }

            return message.ToString();
        }
    }
}
