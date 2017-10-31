using System;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Controllers
{
    public class ProjectsListController : SchemeBoundController
    {
        private readonly IProjectService service;

        public ProjectsListController(IProjectService service)
        {
            this.service = service;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult GetProjectsTable(int refPartId, bool[] projStatusFilters)
        {
            var data = service.GetProjectsTable((InvProjPart)refPartId, projStatusFilters);
            return new RestResult { Success = true, Data = data };
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult DeleteProject(int id)
        {
            var result = new RestResult();
            try
            {
                service.DeleteProject(id);
                return new RestResult { Success = true, Message = "Проект удален!" };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Message = String.Format("Ошибка удаления: {0}", e.Message);
                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ChangePart(int projId)
        {
            var result = new AjaxFormResult();
            StringBuilder script = new StringBuilder();

            try
            {
                service.ChangeProjectPart(projId);

                // Обновляем обе вкладки
                script.AppendLine(@"
var tab1 = parent.RunningProjectsTab.getBody();
tab1.gpProjects.store.reload();
");
                script.AppendLine(@"
if(parent.ProposedProjectsTab.iframe != undefined){
    var tab2 = parent.ProposedProjectsTab.getBody();
    tab2.gpProjects.store.reload();
}");
                result.Success = true;
                result.ExtraParams["msg"] = "Проект перенесен.";
                result.Script = script.ToString();
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка изменения.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }
    }
}
