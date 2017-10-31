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
    public class DetailTaskListController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly ITargetService targetService;
        private readonly ITaskService taskService;

        public DetailTaskListController(
                                   IProgramService programService,
                                   ITargetService targetService,
                                   ITaskService taskService)
        {
            this.programService = programService;
            this.targetService = targetService;
            this.taskService = taskService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetTasksTable(int programId)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var data = this.taskService.GetTasksTable(programId);

            return new AjaxStoreResult { Data = data, Total = data.Count };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetTargetsListForLookup(int programId)
        {
            var data = this.targetService.GetTargetsTableForLookup(programId);
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveTasksTable(int programId, string storeChangedData)
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
                    int taskId = Convert.ToInt32(rowData["ID"]);
                    taskService.Delete(taskId, programId);
                }

                foreach (var rowData in data.Created)
                {
                    string taskName = NullConverter(rowData["TaskName"]);
                    string taskNote = NullConverter(rowData["TaskNote"]);
                    if (String.IsNullOrEmpty(rowData["TargetId"]))
                    {
                        throw new Exception("Не указано поле <Цель>");
                    }

                    int tagretId = Convert.ToInt32(rowData["TargetId"]);
                    
                    var target = targetService.GetTarget(tagretId);

                    taskService.Create(programId, target, taskName, taskNote);
                }

                foreach (var rowData in data.Updated)
                {
                    int taskId = Convert.ToInt32(rowData["ID"]);
                    string taskName = NullConverter(rowData["TaskName"]);
                    string taskNote = NullConverter(rowData["TaskNote"]);

                    if (String.IsNullOrEmpty(rowData["TargetId"]))
                    {
                        throw new Exception("Не указано поле <Цель>");
                    }

                    int tagretId = Convert.ToInt32(rowData["TargetId"]);
                    
                    var target = targetService.GetTarget(tagretId);

                    taskService.Update(programId, target, taskId, taskName, taskNote);
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
