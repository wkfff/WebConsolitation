using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Controllers
{
    public class InvestPlanController : SchemeBoundController
    {
        private readonly IInvestPlanService investService;
        private readonly IAdditionalDataService additionalService;
        private readonly IProjectService projectService;

        public InvestPlanController(
                                    IInvestPlanService investService,
                                    IAdditionalDataService additionalService,
                                    IProjectService projectService)
        {
            this.investService = investService;
            this.additionalService = additionalService;
            this.projectService = projectService;
        }

        public ActionResult GetInvestTable(int refProjId, int projInvestType)
        {
            var data = investService.GetInvestsTable(refProjId, (InvProjInvestType)projInvestType);
            return new AjaxStoreResult(data, data.Rows.Count);
        }

        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveInvestTable(int refProjId, string storeChangedData)
        {
            var result = new AjaxFormResult();

            try
            {
                if (projectService.GetProjectStatus(refProjId) != InvProjStatus.Edit)
                {
                    throw new InvalidOperationException("Редактирование разрешено только для статуса проекта \"На редактировании\"");
                }

                StoreDataHandler dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
                ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

                foreach (var rowData in data.Deleted)
                {
                    int refIndicatorId = Convert.ToInt32(rowData["IndicatorId"]);
                    investService.DeleteFactData(refProjId, refIndicatorId);
                }

                foreach (var rowData in data.Created)
                {
                    int refIndicatorId = Convert.ToInt32(rowData["IndicatorId"]);
                    var years = GetYearColumns(rowData);
                    investService.CreateFactData(refProjId, refIndicatorId, years, rowData);
                }

                foreach (var rowData in data.Updated)
                {
                    int refIndicatorId = Convert.ToInt32(rowData["IndicatorId"]);
                    var years = GetYearColumns(rowData);
                    investService.UpdateFactData(refProjId, refIndicatorId, years, rowData);
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

        public ActionResult GetIndicatorList(int refTypeI)
        {
            var data = additionalService.GetIndicatorList(refTypeI);
            return new AjaxStoreResult(data, data.Count);
        }

        private List<int> GetYearColumns(Dictionary<string, string> row)
        {
            var columns = new List<string>(row.Keys);
            var yearColumns = columns.FindAll(f => f.Length > 4 && f.Substring(0, 4) == "Year");
            var result = new List<int>(yearColumns.Select(yearColumn => Convert.ToInt32(yearColumn.Substring(4))));
            return result;
        }
    }
}
