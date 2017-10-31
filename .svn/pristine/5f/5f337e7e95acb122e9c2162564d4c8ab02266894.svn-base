using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningRegController : SchemeBoundController
    {
        private readonly IForecastExtension extension;
        private readonly IForecastRegulatorsValueRepository regValuesRepository;
        private readonly IRepository<D_Forecast_VarScenCond> varScenCond;

        public PlanningRegController(
                    IForecastExtension extension, 
                    IForecastRegulatorsValueRepository regValuesRepository,
                    IRepository<D_Forecast_VarScenCond> varScenCond)
        {
            this.extension = extension;
            this.regValuesRepository = regValuesRepository;
            this.varScenCond = varScenCond;
        }
        
        public ActionResult LoadRegs(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datReg = ufc.DataService.GetRegulatorData();
            
            return new AjaxStoreResult(datReg, datReg.Rows.Count);
        }

        public ActionResult LoadFVar(int regId)
        {
            var list = from f in regValuesRepository.GetAllValues()
                       where (f.RefRegs.ID == regId) && (f.PType > 0)
                       select new
                       {
                           Value = f.RefVSC.ID,
                           Text = f.RefVSC.Symbol
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult ChangeFVar(string key, int paramId, int newVal)
        {
            var ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            List<ForecastStruct> lstForecastStruct = ufc.DataService.GetForecastList();

            foreach (var forecastStruct in lstForecastStruct)
            {
                List<ForecastRegulator> usedRegs = forecastStruct.UsedRegs;

                for (int i = 0; i < usedRegs.Count; i++)
                {
                    ForecastRegulator regulator = usedRegs[i];
                    if (regulator.RegId == paramId)
                    {
                        regulator.FVar = newVal;
                        usedRegs[i] = regulator;
                    }
                }
            }

            DataTable datReg = ufc.DataService.GetRegulatorData();
            datReg.PrimaryKey = new DataColumn[] { datReg.Columns["id"] };

            DataRow row = datReg.Rows.Find(paramId);

            var code = (from f in varScenCond.GetAll()
                       where f.ID == newVal
                       select f.Symbol).First();

            row["fvarcode"] = code;

            ufc.DataService.LoadRegulators();

            ar.Result = "success";

            return ar;
        }
    }
}
