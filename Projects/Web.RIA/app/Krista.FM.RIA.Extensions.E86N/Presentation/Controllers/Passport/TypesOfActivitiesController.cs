using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Passport
{
    public class TypesOfActivitiesController : RestBaseController<F_F_OKVEDY>
    {
        private readonly OKVEDYViewModel okvedyViewModel = new OKVEDYViewModel();

        private readonly IChangeLogService logService;

        private readonly IRepository<D_OKVED_OKVED> lookUp1Repository;

        private readonly IRepository<D_Org_PrOKVED> lookUp2Repository;

        private readonly IRepository<F_Org_Passport> passportRepository;

        private readonly INewRestService newRestService;

        public TypesOfActivitiesController(
            ILinqRepository<F_F_OKVEDY> repository, 
            IRepository<D_OKVED_OKVED> lookUp1Repository, 
            IRepository<D_Org_PrOKVED> lookUp2Repository, 
            IRepository<F_Org_Passport> passportRepository)
        {
            TableRepository = repository;

            this.lookUp1Repository = lookUp1Repository;
            this.lookUp2Repository = lookUp2Repository;
            this.passportRepository = passportRepository;
            logService = Resolver.Get<IChangeLogService>();
            newRestService = Resolver.Get<INewRestService>();
        }

        [HttpPost]
        [Transaction]
        public RestResult Create(string data, int recId)
        {
            try
            {
                JsonObject inData = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(inData);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_F_OKVEDY formData = JavaScriptDomainConverter<F_F_OKVEDY>.DeserializeSingle(data);

                formData.ID = 0;
                formData.SourceID = 0;
                formData.TaskID = 0;
                formData.RefPassport = passportRepository.Get(recId);
                formData.RefOKVED = lookUp1Repository.Get(formData.RefOKVED.ID);
                formData.RefPrOkved = lookUp2Repository.Get(formData.RefPrOkved.ID);

                TableRepository.Save(formData);
                TableRepository.DbContext.CommitChanges();

                logService.WriteChangeDocDetail(formData.RefPassport.RefParametr);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Новая запись добавлена", 
                        Data = from p in TableRepository.FindAll()
                               where p.ID == formData.ID
                               select new
                                   {
                                       p.ID, 
                                       RefOkved = p.RefOKVED.ID, 
                                       RefOkvedName = string.Format("{0};{1}", p.RefOKVED.Code, p.RefOKVED.Name), 
                                       RefPrOkved = p.RefPrOkved.ID, 
                                       RefPrOkvedName = p.RefPrOkved.Name, 
                                       p.Name
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public ActionResult Read(int recId)
        {
            var data = from p in TableRepository.FindAll()
                       where p.RefPassport.ID == recId
                       select new OKVEDYViewModel
                           {
                               ID = p.ID, 
                               Name = p.Name, 
                               RefOkved = p.RefOKVED.ID, 
                               RefOkvedName = string.Format("{0};{1}", p.RefOKVED.Code, p.RefOKVED.Name), 
                               RefPrOkved = p.RefPrOkved.ID, 
                               RefPrOkvedName = p.RefPrOkved.Name
                           };
            return new AjaxStoreResult(data, data.Count());
        }

        public override void ConfigUpdates()
        {
            string validationError = ValidateData(JoDataUpdate);
            if (validationError.IsNotNullOrEmpty())
            {
                throw new InvalidDataException(validationError);
            }

            RecordDataUpdate.RefOKVED = lookUp1Repository.Get(Convert.ToInt32(JoDataUpdate["RefOkved"]));
            RecordDataUpdate.RefPrOkved = lookUp2Repository.Get(Convert.ToInt32(JoDataUpdate["RefPrOkved"]));
            RecordDataUpdate.Name = Convert.ToString(JoDataUpdate["Name"]);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public override RestResult Update(int id, string data)
        {
            logService.WriteChangeDocDetail(TableRepository.Load(id).RefPassport.RefParametr);
            return base.Update(id, data);
        }

        [HttpDelete]
        public RestResult Delete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_OKVEDY>(id, docId);
        }

        protected string ValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            string message = string.Empty;

            if (Convert.ToString(record["RefOkved"]).IsNullOrEmpty())
            {
                message += Msg.FormatWith("ОКВЭД");
            }

            if (Convert.ToString(record["RefPrOkved"]).IsNullOrEmpty())
            {
                message += Msg.FormatWith("Признак вида деятельности");
            }

            if (record.CheckNull(() => okvedyViewModel.Name))
            {
                message += Msg.FormatWith(okvedyViewModel.DescriptionOf(() => okvedyViewModel.Name));
            }

            return message;
        }
    }
}
