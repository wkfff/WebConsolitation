using System;
using System.Collections.Generic;
using System.Security;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Models;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Controllers
{
    public class ReportEffectivityEstimateController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly IEstimateService estimateService;

        public ReportEffectivityEstimateController(
                                   IProgramService programService,
                                   IEstimateService estimateService)
        {
            this.programService = programService;
            this.estimateService = estimateService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetReportTable(int programId, int year, ProgramStage stage)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var program = programService.GetProgram(programId);
            
            var data = this.estimateService.GetReportTable(program, year, stage);
            
            return new AjaxStoreResult { Data = data, Total = data.Count };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCriteriasListForLookup(int criteriaId)
        {
            // var data = new List<object> { new { ID = 1, Name = "Показатель третьего уровня", Value = 2 } };
            var data = this.estimateService.GetSubcriteriasList(criteriaId);
            return new AjaxStoreResult { Data = data, Total = data.Count };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetYearList(int programId)
        {
            var years = programService.GetProgram(programId).GetYears();

            var data = new List<object>(years.Count);
            foreach (int year in years)
            {
                data.Add(new { ID = year, Value = String.Format("Год {0}", year) });
            }
            
            return new AjaxStoreResult { Data = data, Total = data.Count };
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveReportTable(int programId, int year, object data)
        {
            var result = new AjaxFormResult();

            try
            {
                var program = programService.GetProgram(programId);
                var ownerPermissions = new PermissionSettings(User, program);
                bool editable = ownerPermissions.CanEditDetail;
                if (!editable)
                {
                    throw new SecurityException("Недостаточно привилегий");
                }

                var changedData = JavaScriptDomainConverter<EstimateModel>.Deserialize(Convert.ToString(((string[])data)[0]));

                foreach (var rowData in changedData.Updated)
                {
                    // Сохраняем только изменения со второго уровня вложенности
                    if (rowData.Level == 1)
                    {
                        if (rowData.SelectedId == null)
                        {
                            throw new Exception("Не выбран подкритерий");
                        }

                        this.estimateService.SaveReportFactData(program, year, (int)rowData.SelectedId, rowData.Comment);
                    }
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
    }
}
