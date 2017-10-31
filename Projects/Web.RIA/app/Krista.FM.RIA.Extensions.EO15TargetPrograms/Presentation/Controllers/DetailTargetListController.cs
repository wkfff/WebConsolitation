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
    public class DetailTargetListController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly ITargetService targetService;

        public DetailTargetListController(
                                   IProgramService programService,
                                   ITargetService targetService)
        {
            this.programService = programService;
            this.targetService = targetService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetTargetsTable(int programId)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var data = this.targetService.GetTargetsTable(programId);

            return new AjaxStoreResult { Data = data, Total = data.Count };
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveTargetsTable(int programId, string storeChangedData)
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
                    int targetId = Convert.ToInt32(rowData["ID"]);
                    targetService.Delete(targetId, programId);
                }

                foreach (var rowData in data.Created)
                {
                    string name = NullConverter(rowData["Name"]);
                    string note = NullConverter(rowData["Note"]);
                    targetService.Create(program, name, note);
                }

                foreach (var rowData in data.Updated)
                {
                    int targetId = Convert.ToInt32(rowData["ID"]);
                    string name = NullConverter(rowData["Name"]);
                    string note = NullConverter(rowData["Note"]);
                    targetService.Update(program, targetId, name, note);
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
