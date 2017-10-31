using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    public class ReportProgramExecutingController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly ITargetRatingService targetRatingService;

        public ReportProgramExecutingController(
                                   IProgramService programService,
                                   ITargetRatingService targetRatingService)
        {
            this.programService = programService;
            this.targetRatingService = targetRatingService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetReportTable(int programId, int year)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var program = programService.GetProgram(programId);
            
            DataTable data = this.targetRatingService.GetReportTable(program, year);
            
            return new AjaxStoreResult { Data = data, Total = data.Rows.Count };
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
        public AjaxFormResult SaveReportTable(int programId, int year, string storeChangedData)
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

                StoreDataHandler dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
                ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

                foreach (var rowData in data.Deleted)
                {
                    throw new Exception("Нельзя удалять строки");
                }

                foreach (var rowData in data.Created)
                {
                    throw new Exception("Нельзя создавать строки");
                }

                foreach (var rowData in data.Updated)
                {
                    if (String.IsNullOrEmpty(rowData["ID"]))
                    {
                        throw new Exception("Не указано id показателя");
                    }

                    int rateId = Convert.ToInt32(rowData["ID"]);

                    Dictionary<int, decimal?> monthFactList = GetMonthFactList(rowData);

                    targetRatingService.SaveReportFactData(programId, rateId, year, monthFactList);
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
        
        private Dictionary<int, decimal?> GetMonthFactList(Dictionary<string, string> rowData)
        {
            const int MonthInYear = 12;
            var result = new Dictionary<int, decimal?>(MonthInYear);
            
            for (int month = 1; month <= MonthInYear; month++)
            {
                string columnName = String.Format("Month{0}", month);
                if (String.IsNullOrEmpty(rowData[columnName]))
                {
                    result.Add(month, null);
                }
                else
                {
                    result.Add(month, ConvertToDecimal(rowData[columnName]));   
                }
            }

            return result;
        }
    }
}
