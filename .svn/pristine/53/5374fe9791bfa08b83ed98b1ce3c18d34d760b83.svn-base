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
    public class PriceEnactmentController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public PriceEnactmentController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }
        
        public ActionResult Read(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_NPACena2016>()
                       where p.RefFactGZ.ID == masterId
                       select new PriceEnactmentViewModel
                       {
                           ID = p.ID,
                           Name = p.Name,
                           DataNPAGZ = p.DataNPAGZ,
                           NumNPA = p.NumNPA,
                           OrgUtvDoc = p.OrgUtvDoc,
                           VidNPAGZ = p.VidNPAGZ
                       };
            return new AjaxStoreResult(data, data.Count());
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

                var record = JavaScriptDomainConverter<F_F_NPACena2016>.DeserializeSingle(data);

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
                    Data = from p in newRestService.GetItems<F_F_NPACena2016>()
                           where p.ID == record.ID
                           select new PriceEnactmentViewModel
                           {
                               ID = p.ID,
                               Name = p.Name,
                               DataNPAGZ = p.DataNPAGZ,
                               NumNPA = p.NumNPA,
                               OrgUtvDoc = p.OrgUtvDoc,
                               VidNPAGZ = p.VidNPAGZ
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult Delete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_NPACena2016>(id, docId);
        }

        private string ValidateData(JsonObject record, int masterId)
        {
            const string RequireFieldMsg = "Необходимо заполнить поле: {0}<br>";
            const string DublicateMsg = "Запись уже заведена: \"{0}\"<br>";

            var model = new PriceEnactmentViewModel();
            var message = new StringBuilder();

            if (record.CheckNull(() => model.Name))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.Name)));
            }

            if (record.CheckNull(() => model.NumNPA))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.NumNPA)));
            }

            if (record.CheckNull(() => model.OrgUtvDoc))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.OrgUtvDoc)));
            }

            if (record.CheckNull(() => model.VidNPAGZ))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.VidNPAGZ)));
            }

            DateTime? dateNpaGz = null;
            if (record.CheckNull(() => model.DataNPAGZ))
            {
                message.Append(RequireFieldMsg.FormatWith(model.DescriptionOf(() => model.DataNPAGZ)));
            }
            else
            {
                dateNpaGz = Convert.ToDateTime(record.GetValue(() => model.DataNPAGZ));
                if (dateNpaGz > DateTime.Today)
                {
                    message.Append("Некорректная дата НПА<br>");
                }
            }

            var id = record.GetValueToIntOrDefault(() => model.ID, -1);
            var name = record.GetValue(() => model.Name);
            var numNPA = record.GetValue(() => model.NumNPA);
            var orgUtvDoc = record.GetValue(() => model.OrgUtvDoc);
            var vidNpaGz = record.GetValue(() => model.VidNPAGZ);

            if (newRestService.GetItems<F_F_NPACena2016>().Any(
                x => x.ID != id && x.RefFactGZ.ID == masterId &&
                     x.Name == name &&
                     x.NumNPA == numNPA &&
                     x.OrgUtvDoc == orgUtvDoc &&
                     x.VidNPAGZ == vidNpaGz &&
                     x.DataNPAGZ == dateNpaGz))
            {
                message.Append(DublicateMsg.FormatWith(name));
            }

            return message.ToString();
        }
    }
}
