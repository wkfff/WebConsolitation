using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ReverseController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastExtension extension;
        
        public ReverseController(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public ActionResult ShowExist(int id, int parentId)
        {
            var varid = id;
            string key = String.Format("reverseForm_{0}", varid);

            if (extension.Forms.ContainsKey(key))
            {
                extension.Forms.Remove(key);
            }
            
            extension.Forms.Add(key, new UserFormsControls(varid));
            
            var viewControl = Resolver.Get<ReverseView>();
            
            viewControl.Initialize(varid, parentId);

            var ufc = extension.Forms[key];

            ufc.DataService.Initialize();
            ufc.DataService.FillData(parentId);
            ufc.DataService.LoadData(parentId);

            viewControl.LoadSavedData(varid);

            return View(ViewRoot + "View.aspx", viewControl);
        }
    }
}
