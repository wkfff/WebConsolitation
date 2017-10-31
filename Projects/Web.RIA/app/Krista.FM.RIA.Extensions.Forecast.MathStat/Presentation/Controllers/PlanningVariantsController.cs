using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
   public class PlanningVariantsController : SchemeBoundController
    {
        private readonly IForecastVariantsRepository variantsRepository;
        private readonly IForecastExtension extension;

        public PlanningVariantsController(IForecastVariantsRepository variantsRepository, IForecastExtension extension)
        {
            this.variantsRepository = variantsRepository;
            this.extension = extension;
        }
        
        public ActionResult Load()
        {
            //// where f.RefParam.ID == id
            var view = from f in variantsRepository.GetAllVariants()
                       select new
                       {
                           f.ID,
                           f.Name,
                           ////f.XMLString,
                           refParam = f.RefParam.ID,
                           refParamName = f.RefParam.Name,
                           luMethod = GetMethod(f.Method),
                           luStatus = GetStatus(f.Status),
                           luUser = Scheme.UsersManager.GetUserNameByID(f.UserID),
                           luRefDate = DataService.GetDate(f.RefDate.ID, f.Period)
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult GetDescription(int method)
        {
            AjaxResult ar = new AjaxResult();

            var mathGroup = extension.LoadedMathGroups.GetGroupByCode(FixedMathGroups.ComplexEquation);
            
            if (mathGroup.HasValue)
            {
                var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                if (mathMethod.HasValue)
                {
                    ar.Script = String.Format("textArea.setValue(\"{0}\");", mathMethod.Value.Description);
                    ar.Script += "textArea.setHeight(200);";
                }
            }

            return ar;
        }

        private string GetStatus(int id)
        {
            switch (id)
            {
                case 0: return "Новый расчет";
                case 1: return "Расчитанный вариант";
                case 2: return "Заблокированный";
            }

            return String.Empty;
        }

        private string GetMethod(int id)
        {
            int group = id / 100;
            int method = id % 100;
            
            string methodName = String.Empty;

            var mathGroup = extension.LoadedMathGroups.GetGroupByCode(group);

            if (mathGroup.HasValue)
            {
                var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                if (mathMethod.HasValue)
                {
                    methodName = mathMethod.Value.TextName;
                }
            }
            
            return methodName;
        }
    }
}
