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
    public class DetailTargetRatingsListController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly ITaskService taskService;
        private readonly IAdditionalService additionalService;
        private readonly ITargetRatingService ratingService;

        public DetailTargetRatingsListController(
                                   IProgramService programService,
                                   ITaskService taskService,
                                   IAdditionalService additionalService,
                                   ITargetRatingService ratingService)
        {
            this.programService = programService;
            this.taskService = taskService;
            this.additionalService = additionalService;
            this.ratingService = ratingService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetRatingsTable(int programId)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var program = programService.GetProgram(programId);
            
            //// TODO: проверка security

            var data = this.ratingService.GetRatingsListTable(program);
            
            return new AjaxStoreResult { Data = data, Total = data.Rows.Count };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetTasksListForLookup(int programId)
        {
            var data = this.taskService.GetTasksTableForLookup(programId);
            return new AjaxStoreResult(data, data.Count);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllUnitListForLookup()
        {
            var data = this.additionalService.GetAllUnitListForLookup();
            return new AjaxStoreResult(data, data.Count);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllRateTypeListForLookup()
        {
            var data = this.ratingService.GetAllRateTypeListForLookup();
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveRatingsTable(int programId, string storeChangedData)
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

                var yearColumns = program.GetYearsWithPreviousAndFollowing();
                
                StoreDataHandler dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
                ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

                foreach (var rowData in data.Deleted)
                {
                    if (String.IsNullOrEmpty(rowData["ID"]))
                    {
                        throw new Exception("Не указан показатель для удаления мероприятие");
                    }

                    int rateId = Convert.ToInt32(rowData["ID"]);
                    ratingService.DeleteRateWithFactData(programId, rateId);
                }

                foreach (var rowData in data.Created)
                {
                    string rateName = rowData["RateName"];
                    var task = GetTask(rowData["TaskId"]);
                    var unit = GetUnit(rowData["UnitId"]);
                    var rateType = GetRateType(rowData["RateTypeId"]);
                    
                    ratingService.CreateRateWithFactData(programId, task, rateName, rateType, unit, GetYearData(rowData, yearColumns));
                }

                foreach (var rowData in data.Updated)
                {
                    if (String.IsNullOrEmpty(rowData["ID"]))
                    {
                        throw new Exception("Не указан изменяемый показатель");
                    }

                    int rateId = Convert.ToInt32(rowData["ID"]);
                    string rateName = rowData["RateName"];
                    var task = GetTask(rowData["TaskId"]);
                    var unit = GetUnit(rowData["UnitId"]);
                    var rateType = GetRateType(rowData["RateTypeId"]);

                    ratingService.UpdateRateWithFactData(programId, rateId, rateName, rateType, unit, task, GetYearData(rowData, yearColumns));
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

        private D_ExcCosts_Tasks GetTask(string taskIdStr)
        {
            if (String.IsNullOrEmpty(taskIdStr))
            {
                throw new Exception("Не указана задача");
            }

            int taskId = Convert.ToInt32(taskIdStr);
            return taskService.GetTask(taskId);
        }
        
        private D_Units_OKEI GetUnit(string unitIdStr)
        {
            if (String.IsNullOrEmpty(unitIdStr))
            {
                throw new Exception("Не указана единица измерения показателя");
            }

            int unitId = Convert.ToInt32(unitIdStr);
            return additionalService.GetUnit(unitId);
        }

        private FX_ExcCosts_TypeMark GetRateType(string rateIdStr)
        {
            if (String.IsNullOrEmpty(rateIdStr))
            {
                throw new Exception("Не указан тип показателя");
            }

            int rateId = Convert.ToInt32(rateIdStr);
            return ratingService.GetRateType(rateId);
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
