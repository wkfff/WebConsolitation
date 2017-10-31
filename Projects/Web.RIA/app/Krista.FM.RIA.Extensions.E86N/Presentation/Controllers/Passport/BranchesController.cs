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
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Passport
{
    public class BranchesController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;
        
        public BranchesController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public ActionResult Read(int recId)
        {
            var data = from p in newRestService.GetItems<F_F_Filial>()
                       where p.RefPassport.ID == recId
                       select new
                        {
                            p.ID,
                            RefTipFi = p.RefTipFi.ID,
                            RefTipFiName = p.RefTipFi.Name,
                            p.Code,
                            p.Name,
                            p.Nameshot,
                            p.INN,
                            p.KPP
                        };
            return new AjaxStoreResult(data, data.Count());
        }

        [HttpPost]
        [Transaction]
        public RestResult Save(string data, int recId)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_F_Filial record =
                    JavaScriptDomainConverter<F_F_Filial>.DeserializeSingle(data);

                string msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefPassport = newRestService.GetItem<F_Org_Passport>(recId);
                record.RefTipFi = newRestService.GetItem<D_Org_TipFil>(record.RefTipFi.ID);
               
                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefPassport.RefParametr);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in newRestService.GetItems<F_F_Filial>()
                       where p.ID == record.ID
                       select new
                        {
                            p.ID,
                            RefTipFi = p.RefTipFi.ID,
                            RefTipFiName = p.RefTipFi.Name,
                            p.Code,
                            p.Name,
                            p.Nameshot,
                            p.INN,
                            p.KPP
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
            return newRestService.DeleteDocDetailAction<F_F_Filial>(id, docId);
        }

        protected string ValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            
            string message = string.Empty;

            if (Convert.ToString(record["RefTipFi"]).IsNullOrEmpty())
            {
                message += Msg.FormatWith("Тип филиала");
            }

            return message;
        }
    }
}
