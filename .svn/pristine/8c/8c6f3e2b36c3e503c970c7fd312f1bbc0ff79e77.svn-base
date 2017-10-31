using System;
using System.Collections.Generic;
using System.Security;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Controllers
{
    public class DetailActionListController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly ITaskService taskService;
        private readonly IActionService actionService;
        private readonly IAdditionalService additionalService;

        public DetailActionListController(
                                   IProgramService programService,
                                   ITaskService taskService,
                                   IActionService actionService,
                                   IAdditionalService additionalService)
        {
            this.programService = programService;
            this.taskService = taskService;
            this.actionService = actionService;
            this.additionalService = additionalService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetActionsTable(int programId)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var data = this.actionService.GetActionsTable(programId);

            return new AjaxStoreResult { Data = data, Total = data.Count };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetTasksListForLookup(int programId)
        {
            var data = this.taskService.GetTasksTableForLookup(programId);
            return new AjaxStoreResult(data, data.Count);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetOwnersListForLookup()
        {
            var data = this.additionalService.GetAllOwnersList();
            
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveActionsTable(int programId, string storeChangedData)
        {
            var result = new AjaxFormResult();

            try
            {
                var program = programService.GetProgram(programId);
                var editable = new PermissionSettings(User, program).CanEditDetail;
                if (!editable)
                {
                    throw new SecurityException("Недостаточно привилегий");
                }

                StoreDataHandler dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
                ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

                foreach (var rowData in data.Deleted)
                {
                    int actionId = Convert.ToInt32(rowData["ID"]);
                    actionService.Delete(actionId, programId);
                }

                foreach (var rowData in data.Created)
                {
                    string actionName = NullConverter(rowData["ActionName"]);
                    string actionNote = NullConverter(rowData["ActionNote"]);
                    string actionResult = NullConverter(rowData["ActionResults"]);
                    
                    if (String.IsNullOrEmpty(rowData["TaskId"]))
                    {
                        throw new Exception("Не указано поле <Задача>");
                    }

                    int taskId = Convert.ToInt32(rowData["TaskId"]);
                    var task = taskService.GetTask(taskId);

                    if (String.IsNullOrEmpty(rowData["ActionOwnerId"]))
                    {
                        throw new Exception("Не указано поле <Заказчик>");
                    }
                    
                    int ownerId = Convert.ToInt32(rowData["ActionOwnerId"]);
                    var owner = additionalService.GetCreator(ownerId);

                    actionService.Create(programId, task, actionName, actionNote, actionResult, owner);
                }

                foreach (var rowData in data.Updated)
                {
                    int actionId = Convert.ToInt32(rowData["ID"]);
                    string actionName = NullConverter(rowData["ActionName"]);
                    string actionNote = NullConverter(rowData["ActionNote"]);
                    string actionResult = NullConverter(rowData["ActionResults"]);

                    if (String.IsNullOrEmpty(rowData["TaskId"]))
                    {
                        throw new Exception("Не указано поле <Задача>");
                    }
                    
                    int taskId = Convert.ToInt32(rowData["TaskId"]);
                    var task = taskService.GetTask(taskId);

                    if (String.IsNullOrEmpty(rowData["ActionOwnerId"]))
                    {
                        throw new Exception("Не указано поле <Заказчик>");
                    }
                    
                    int ownerId = Convert.ToInt32(rowData["ActionOwnerId"]);
                    var owner = additionalService.GetCreator(ownerId);

                    actionService.Update(programId, actionId, task, actionName, actionNote, actionResult, owner);
                }

                result.Success = true;
                result.Script = null;
                result.ExtraParams["msg"] = "Данные сохранены.";
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка сохранения.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }

        private string NullConverter(string str)
        {
            return String.IsNullOrEmpty(str) ? null : str;
        }
    }
}
