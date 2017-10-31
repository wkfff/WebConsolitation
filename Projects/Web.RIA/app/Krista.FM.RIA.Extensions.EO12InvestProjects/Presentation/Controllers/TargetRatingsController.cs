using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Controllers
{
    public class TargetRatingsController : SchemeBoundController
    {
        private readonly ITargetRatingsService targetService;
        private readonly IAdditionalDataService additionalService;
        private readonly IProjectService projectService;

        public TargetRatingsController(
                                       ITargetRatingsService targetService,
                                       IAdditionalDataService additionalService,
                                       IProjectService projectService)
        {
            this.targetService = targetService;
            this.additionalService = additionalService;
            this.projectService = projectService;
        }

        public ActionResult GetQuarterList(int refProjId)
        {
            var data = targetService.GetQuarterList(refProjId);
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetIndicatorList()
        {
            var data = additionalService.GetIndicatorList((int)InvProjInvestType.TargetRatings);
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetRatingsTable(int refProjId, string yearQuarter)
        {
            var data = targetService.GetRatingsTable(refProjId, yearQuarter);
            return new AjaxStoreResult(data, data.Rows.Count);
        }

        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveRatingsTable(int refProjId, string yearQuarter, string storeChangedData)
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
                    CheckParametersOnUpdateDelete(rowData);
                    int refIndicatorId = Convert.ToInt32(rowData["IndicatorId"]);
                    targetService.DeleteFactData(refProjId, yearQuarter, refIndicatorId);
                }

                foreach (var rowData in data.Created)
                {
                    CheckParametersOnCreate(rowData);
                    int refIndicatorId = Convert.ToInt32(rowData["IndicatorId"]);
                    decimal value = ConvertToDecimal(rowData["Value"]);
                    targetService.CreateFactData(refProjId, yearQuarter, refIndicatorId, value);
                }

                foreach (var rowData in data.Updated)
                {
                    CheckParametersOnUpdateDelete(rowData); 
                    int refIndicatorId = Convert.ToInt32(rowData["IndicatorId"]);
                    decimal? value = String.IsNullOrEmpty(rowData["Value"]) ? null : (decimal?)ConvertToDecimal(rowData["Value"]);
                    targetService.UpdateFactData(refProjId, yearQuarter, refIndicatorId, value);
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

        private void CheckParametersOnCreate(Dictionary<string, string> rowData)
        {
            CheckParametersOnUpdateDelete(rowData);

            if (String.IsNullOrEmpty(rowData["Value"]))
            {
                throw new NoNullAllowedException("Не указано значение по показателю.");
            }
        }

        private void CheckParametersOnUpdateDelete(Dictionary<string, string> rowData)
        {
            if (String.IsNullOrEmpty(rowData["IndicatorId"]))
            {
                throw new NoNullAllowedException("Не указан показатель.");
            }
        }
    }
}
