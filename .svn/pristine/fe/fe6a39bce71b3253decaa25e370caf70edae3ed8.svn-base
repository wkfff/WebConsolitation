using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Models;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsReglController : SchemeBoundController
    {
        private readonly IReglamentService reglRepository;
        private readonly IRepository<D_CD_Roles> roleRepository;
        private readonly IRepository<D_CD_Level> levelRepository;
        private readonly IRepository<D_CD_ReportKind> repKindRepository;
        private readonly IRepository<D_CD_Templates> templatesRepository;

        public ConsReglController(
            IReglamentService reglRepository,
            IRepository<D_CD_Roles> roleRepository,
            IRepository<D_CD_Level> levelRepository,
            IRepository<D_CD_ReportKind> repKindRepository,
            IRepository<D_CD_Templates> templatesRepository)
        {
            this.reglRepository = reglRepository;
            this.roleRepository = roleRepository;
            this.levelRepository = levelRepository;
            this.repKindRepository = repKindRepository;
            this.templatesRepository = templatesRepository;
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = false)]
        public RestResult Create(string data)
        {
            try
            {
                D_CD_Reglaments reglament = JavaScriptDomainConverter<D_CD_Reglaments>.DeserializeSingle(data);
                
                reglament.ID = 0;
                reglament.RefRole = roleRepository.Get(reglament.RefRole.ID);
                reglament.RefLevel = levelRepository.Get(reglament.RefLevel.ID);
                reglament.RefRepKind = repKindRepository.Get(reglament.RefRepKind.ID);
                reglament.RefTemplate = templatesRepository.Get(reglament.RefTemplate.ID);
                reglRepository.Save(reglament);
                reglRepository.DbContext.CommitChanges();

                return new RestResult
                {
                    Success = true,
                    Message = "Новая запись регламента добавлена",
                    Data = reglRepository.GetQueryOne(reglament.ID)
                };
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e);
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpGet]
        public RestResult Read()
        {
            try
            {
                return new RestResult { Success = true, Data = reglRepository.GetQueryAll() };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Update(int id, string data)
        {
            try
            {
                JsonObject fields = JSON.Deserialize<JsonObject>(data);

                var reglament = reglRepository.FindOne(id);

                reglament.Laboriousness = Convert.ToInt32(fields["Laboriousness"]);
                reglament.Workdays = Convert.ToBoolean(fields["Workdays"]);
                reglament.Note = Convert.ToString(fields["Note"]);
                reglament.RefRepKind = repKindRepository.Get(Convert.ToInt32(fields["RefRepKind"]));
                reglament.RefRole = roleRepository.Get(Convert.ToInt32(fields["RefRole"]));
                reglament.RefLevel = levelRepository.Get(Convert.ToInt32(fields["RefLevel"]));
                reglament.RefTemplate = templatesRepository.Get(Convert.ToInt32(fields["RefTemplate"]));

                reglRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись регламента обновлена" };
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e);
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Destroy(int id)
        {
            try
            {
                var reglament = reglRepository.FindOne(id);
                reglRepository.Delete(reglament);
                reglRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись регламента удалена" };
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e);
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpGet]
        public ActionResult LookupRole()
        {
            var data = roleRepository.GetAll();
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpGet]
        public ActionResult LookupLevel()
        {
            var data = levelRepository.GetAll();
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpGet]
        public ActionResult LookupRepKind()
        {
            var data = (from f in repKindRepository.GetAll() 
                       select new { f.ID, f.Name }).ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpGet]
        public ActionResult LookupTemplate()
        {
            var data = (from t in templatesRepository.GetAll()
                       where t.Status == (int)FormStatus.Active
                       select new { t.ID, Name = t.Name + "(версия " + t.FormVersion + ')' }).ToList();
            return new AjaxStoreResult(data, data.Count);
        }
    }
}
