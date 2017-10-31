using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTasks
{
    public class StateTaskTab3Controller : RestBaseController<F_F_NPACena>
    {
        private readonly IRepository<F_F_GosZadanie> gosZadanie;

        private readonly IChangeLogService logService;

        private readonly INewRestService newRestService;

        public StateTaskTab3Controller(
            ILinqRepository<F_F_NPACena> repository, 
            IRepository<F_F_GosZadanie> gosZadanie)
        {
            TableRepository = repository;
            this.gosZadanie = gosZadanie;
            logService = Resolver.Get<IChangeLogService>();
            newRestService = Resolver.Get<INewRestService>();
        }

        public ActionResult Read(int masterId)
        {
            var data = from p in TableRepository.FindAll()
                       where p.RefGZPr.ID == masterId
                       select new
                           {
                               p.ID, 
                               p.Name, 
                               p.DataNPAGZ, 
                               p.NumNPA, 
                               p.OrgUtvDoc, 
                               p.VidNPAGZ
                           };
            return new AjaxStoreResult(data, data.Count());
        }

        [HttpPost]
        [Transaction]
        public RestResult Create(string data, int masterId)
        {
            try
            {
                F_F_NPACena formData = JavaScriptDomainConverter<F_F_NPACena>.DeserializeSingle(data);

                formData.ID = 0;
                formData.SourceID = 0;
                formData.TaskID = 0;
                formData.RefGZPr = gosZadanie.Get(masterId);

                var validationError = ValidateData(formData);
                if (!string.IsNullOrEmpty(validationError))
                {
                    throw new InvalidDataException(validationError);
                }

                TableRepository.Save(formData);
                logService.WriteChangeDocDetail(formData.RefGZPr.RefParametr);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Новая запись добавлена", 
                        Data = from p in TableRepository.FindAll()
                               where p.ID == formData.ID
                               select new
                                   {
                                       p.ID, 
                                       p.Name, 
                                       p.DataNPAGZ, 
                                       p.NumNPA, 
                                       p.OrgUtvDoc, 
                                       p.VidNPAGZ
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public override void ConfigUpdates()
        {
            var rec = new F_F_NPACena
                {
                    Name = Convert.ToString(JoDataUpdate["Name"]), 
                    NumNPA = Convert.ToString(JoDataUpdate["NumNPA"]), 
                    OrgUtvDoc = Convert.ToString(JoDataUpdate["OrgUtvDoc"]), 
                    VidNPAGZ = Convert.ToString(JoDataUpdate["VidNPAGZ"]), 
                    DataNPAGZ = Convert.ToDateTime(JoDataUpdate["DataNPAGZ"])
                };

            var validationError = ValidateData(rec);
            if (!string.IsNullOrEmpty(validationError))
            {
                throw new InvalidDataException(validationError);
            }

            RecordDataUpdate.Name = rec.Name;
            RecordDataUpdate.NumNPA = rec.NumNPA;
            RecordDataUpdate.OrgUtvDoc = rec.OrgUtvDoc;
            RecordDataUpdate.VidNPAGZ = rec.VidNPAGZ;
            RecordDataUpdate.DataNPAGZ = rec.DataNPAGZ;
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public override RestResult Update(int id, string data)
        {
            logService.WriteChangeDocDetail(TableRepository.Load(id).RefGZPr.RefParametr);
            return base.Update(id, data);
        }

        [HttpDelete]
        public RestResult Delete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_NPACena>(id, docId);
        }

        protected string ValidateData(F_F_NPACena record)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(record.Name))
            {
                message += "Не указано наименование НПА <br/>";
            }

            if (string.IsNullOrEmpty(record.NumNPA))
            {
                message += "Не указан номер НПА <br/>";
            }

            if (string.IsNullOrEmpty(record.OrgUtvDoc))
            {
                message += "Не указан орган утвердивший НПА <br/>";
            }

            if (string.IsNullOrEmpty(record.VidNPAGZ))
            {
                message += "Не указан вид НПА <br/>";
            }

            if (record.DataNPAGZ == null)
            {
                message += "Не указана дата НПА <br/>";
            }

            if (record.DataNPAGZ > DateTime.Today)
            {
                message += "Некорректная дата НПА <br/>";
            }

            return message;
        }
    }
}
