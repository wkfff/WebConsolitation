using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Passport
{
    public class FoundersController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public FoundersController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        public RestResult Read(int parentID)
        {
            var data = from p in newRestService.GetItems<F_F_Founder>()
                       where p.RefPassport.ID == parentID
                       select new
                           {
                               p.ID,
                               RefPassport = p.RefPassport.ID,
                               RefYchred = p.RefYchred.ID,
                               RefYchredName = p.RefYchred.Name,
                               p.formative,
                               p.stateTask,
                               p.supervisoryBoard,
                               p.manageProperty,
                               p.financeSupply
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult Save(string data, int parentID)
        {
            try
            {
                F_F_Founder record = JavaScriptDomainConverter<F_F_Founder>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefPassport = newRestService.GetItem<F_Org_Passport>(parentID);
                record.RefYchred = newRestService.GetItem<D_Org_OrgYchr>(record.RefYchred.ID);
                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefPassport.RefParametr);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in newRestService.GetItems<F_F_Founder>()
                           where p.ID == record.ID
                           select new
                           {
                               p.ID,
                               RefPassport = p.RefPassport.ID,
                               RefYchred = p.RefYchred.ID,
                               RefYchredName = p.RefYchred.Name,
                               p.formative,
                               p.stateTask,
                               p.supervisoryBoard,
                               p.manageProperty,
                               p.financeSupply
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
            return newRestService.DeleteDocDetailAction<F_F_Founder>(id, docId);
        }
    }
}