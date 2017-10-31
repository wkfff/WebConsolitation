using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastExtension extension;
        
        public PlanningController(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public ActionResult ShowExist(int id)
        {
            var varid = id;
            string key = String.Format("planningForm_{0}", varid);

            if (extension.Forms.ContainsKey(key))
            {
                extension.Forms.Remove(key);
            }
            
            extension.Forms.Add(key, new UserFormsControls(varid));
            
            var viewControl = Resolver.Get<PlanningView>();
            
            viewControl.Initialize(varid);

            var ufc = extension.Forms[key];

            ufc.DataService.Initialize();
            ufc.DataService.FillData(varid);
            ufc.DataService.LoadData(varid);

            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult ShowNew(int id, int group, int method)
        {
            var paramid = id;
            string key = "planningForm_-1";
            ////string key = String.Format("planningForm_{0}", id);

            if (extension.Forms.ContainsKey(key))
            {
                extension.Forms.Remove(key);
            }

            extension.Forms.Add(key, new UserFormsControls(-1));
            
            var viewControl = Resolver.Get<PlanningView>();

            viewControl.ParamId = paramid;
            viewControl.Initialize(-1);
            
            viewControl.Group = group;
            
            var ufc = extension.Forms[key];

            string xmlString = String.Empty;

            if (group == FixedMathGroups.ComplexEquation)
            {
                viewControl.Method = method;

                var mathGroup = extension.LoadedMathGroups.GetGroupByCode(group);
                
                if (mathGroup.HasValue)
                {
                    var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                    if (mathMethod.HasValue)
                    {
                        xmlString = mathMethod.Value.XMLString;
                    }
                }
            }

            ufc.DataService.Initialize();
            if (xmlString == String.Empty)
            {
                ufc.DataService.NewData(paramid);
            }
            else
            {
                ufc.DataService.FillDataForNewComplexMethod(xmlString);
            }
            ////ufc.DataService.FillData(-1););

            return View(ViewRoot + "View.aspx", viewControl);
        }
    }
}
