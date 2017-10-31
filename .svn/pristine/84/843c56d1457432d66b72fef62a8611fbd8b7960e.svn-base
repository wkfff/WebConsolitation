using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ReverseVarController : SchemeBoundController
    {
        private readonly IForecastReverseRepository reverseVarRepository;
        private readonly IForecastVariantsRepository variantsRepository;
        ////private readonly IForecastExtension extension;

        public ReverseVarController(IForecastReverseRepository reverseVarRepository, IForecastVariantsRepository variantsRepository/*, IForecastExtension extension*/)
        {
            this.reverseVarRepository = reverseVarRepository;
            this.variantsRepository = variantsRepository;
            ////this.extension = extension;
        }

        public ActionResult Load()
        {
            var view = from f in reverseVarRepository.GetAllVariants()
                       select new
                       {
                           f.ID,
                           f.Name,
                           RefID = f.RefPVar.ID
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult ComboVarsLoad()
        {
            var view = from f in variantsRepository.GetAllVariants()
                       where (f.Status == 1) && (f.Method / 100 == FixedMathGroups.MultiRegression)
                       select new
                       {
                           Value = f.ID,
                           Text = f.Name,
                           Group = f.RefParam.Name,
                           Year = DataService.GetDate(f.RefDate.ID, f.Period)
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult InsertNewReverse(string name, int parentId)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "failure";

            D_Forecast_PVars parentVariants = (from f in variantsRepository.GetAllVariants()
                       where f.ID == parentId
                       select f).First();

            D_Forecast_RevPlan reversePlanning = new D_Forecast_RevPlan
            {
                Name = name,
                RefPVar = parentVariants
            };

            reverseVarRepository.Save(reversePlanning);

            reverseVarRepository.DbContext.CommitChanges();

            if (reversePlanning.ID > -1)
            {
                ar.Result = "success";

                ar.Script = @"

parent.MdiTab.addTab({{ 
    title: 'Обратное прогнозирование', 
    url: '/Reverse/ShowExist/{0}?parentId={1}',
    passParentSize: false  
}});".FormatWith(reversePlanning.ID, parentId);
            }

            return ar;
        }
    }
}
