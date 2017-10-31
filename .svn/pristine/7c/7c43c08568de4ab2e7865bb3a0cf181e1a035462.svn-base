using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Controllers
{
    public class DetailFinancesListController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly IActionService actionService;
        private readonly IFinanceService financeService;

        public DetailFinancesListController(
                                   IProgramService programService,
                                   IActionService actionService,
                                   IFinanceService financeService)
        {
            this.programService = programService;
            this.actionService = actionService;
            this.financeService = financeService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetFinancesTable(int programId)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var program = programService.GetProgram(programId);
            
            //// TODO: проверка security

            var data = this.financeService.GetFinanceListTable(program);
            
            return new AjaxStoreResult { Data = data, Total = data.Rows.Count };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetActionsListForLookup(int programId)
        {
            var data = this.actionService.GetActionsTableForLookup(programId);
            return new AjaxStoreResult(data, data.Count);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetFinSourcesListForLookup()
        {
            var data = this.financeService.GetAllFinSourcesListForLookup();

            return new AjaxStoreResult(data, data.Count);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveFinancesTable(int programId, string storeChangedData)
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

                var yearColumns = program.GetYears();

                StoreDataHandler dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
                ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

                foreach (var rowData in data.Deleted)
                {
                    D_ExcCosts_Events action = GetAction(rowData["ActionIdOld"]);
                    D_ExcCosts_Finances finSource = GetFinSource(rowData["FinSourceIdOld"]);
                    financeService.DeleteFactData(programId, action, finSource);
                }

                foreach (var rowData in data.Created)
                {
                    D_ExcCosts_Events action = GetAction(rowData["ActionId"]);
                    D_ExcCosts_Finances finSource = GetFinSource(rowData["FinSourceId"]);

                    financeService.CreateFactData(programId, action, finSource, GetYearData(rowData, yearColumns));
                }

                foreach (var rowData in data.Updated)
                {
                    if ((rowData["ActionId"] != rowData["ActionIdOld"])
                         || rowData["FinSourceId"] != rowData["FinSourceIdOld"])
                    {
                        throw new Exception("Нельзя изменять существующие параметры!");
                    }

                    D_ExcCosts_Events action = GetAction(rowData["ActionId"]);
                    D_ExcCosts_Finances finSource = GetFinSource(rowData["FinSourceId"]);

                    financeService.UpdateFactData(programId, action, finSource, GetYearData(rowData, yearColumns));
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

        private static decimal ConvertToDecimal(string value)
        {
            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";
            string val = value.Replace(".", ",");
            decimal result = Decimal.Parse(val, ci);
            return result;
        }

        private D_ExcCosts_Events GetAction(string actionIdStr)
        {
            if (String.IsNullOrEmpty(actionIdStr))
            {
                throw new Exception("Не указано мероприятие");
            }

            int actionId = Convert.ToInt32(actionIdStr);
            return actionService.GetAction(actionId);
        }

        private D_ExcCosts_Finances GetFinSource(string finSourceIdStr)
        {
            if (String.IsNullOrEmpty(finSourceIdStr))
            {
                throw new Exception("Не указан источник финансирования!");
            }

            int finSourceId = Convert.ToInt32(finSourceIdStr);
            return financeService.GetFinSource(finSourceId);
        }

        private Dictionary<int, decimal?> GetYearData(Dictionary<string, string> rowData, IList<int> yearsList)
        {
            var result = new Dictionary<int, decimal?>();
            foreach (int year in yearsList)
            {
                var columnName = String.Format("Year{0}", year);
                if (String.IsNullOrEmpty(rowData[columnName]))
                {
                    result.Add(year, null);
                }
                else
                {
                    result.Add(year, ConvertToDecimal(rowData[columnName]));
                }
            }

            return result;
        }
    }
}
