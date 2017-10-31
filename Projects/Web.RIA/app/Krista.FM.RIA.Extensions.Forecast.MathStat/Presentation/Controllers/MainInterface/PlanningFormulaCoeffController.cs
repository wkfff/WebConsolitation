using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningFormulaCoeffController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastExtension extension;
        private readonly IForecastMethodsRepository methodsRepository;

        public PlanningFormulaCoeffController(IForecastExtension extension, IForecastMethodsRepository methodsRepository)
        {
            this.extension = extension;
            this.methodsRepository = methodsRepository;
        }

        public ActionResult Show(int id)
        {
            var viewControl = Resolver.Get<PlanningFormulaCoeffView>();

            viewControl.Initialize(id);

            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult ChangeFormula(int group, int method)
        {
            string imgpath = String.Format("/PlanningFormulaCoeff/FormulaImage/{0}", (group * 100) + method);

            /* switch (group)
            {
                case MathMethods.FirstOrderRegression:
                    if (FirstOrderRegression.GetMethod(method).HasValue)
                    {
                        imgpath = FirstOrderRegression.GetMethod(method).Value.FormulaImgPath;
                    }

                    break;
                case MathMethods.SecondOrderRegression:
                    if (SecondOrderRegression.GetMethod(method).HasValue)
                    {
                        imgpath = SecondOrderRegression.GetMethod(method).Value.FormulaImgPath;
                    }

                    break;
                case MathMethods.ARMAMethod:
                    break;
            }*/

            var mathGroup = extension.LoadedMathGroups.GetGroupByCode(group);

            var txtAreascript = String.Empty;

            if (mathGroup.HasValue)
            {
                var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                if (mathMethod.HasValue)
                {
                    txtAreascript = String.Format("textArea.setValue(\"{0}\");", mathMethod.Value.Description);
                }
            }

            string imgScript = String.Format("imgFormula.setImageUrl('{0}');", imgpath);
            
            return new AjaxResult { Script = String.Concat(txtAreascript, imgScript) };
        }

        public ActionResult FormulaImage(int id)
        {
            int groupCode = id / 100;
            int methodCode = id % 100;

            byte[] fileContents = new byte[] { };

            /*var tempData = (from t in methodsRepository.GetAllMethods()
                                  where t.Code == code
                                  select t.ImageFile).ToList();*/
            
            var group = extension.LoadedMathGroups.GetGroupByCode(groupCode);

            bool found = false;
            if (group.HasValue)
            {
                var method = group.Value.Methods.GetMethodByCode(methodCode);

                if (method.HasValue)
                {
                    fileContents = method.Value.FormulaImg;
                    found = true;
                }
            }

            if (!found)
            {
                fileContents = Resources.Resource.EmptyImage;
            }

            if ((fileContents == null) || (fileContents.Length == 0))
            {
                fileContents = Resources.Resource.EmptyImage;
            }

            FileContentResult fcr = new FileContentResult(fileContents, "image/png");
            
            return fcr;
        }

        public ActionResult LoadProgCoeffs(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            var dicCoeff = ufc.DataService.GetCoeff().ToList();

            return new AjaxStoreResult(dicCoeff, dicCoeff.Count);
        }
    }
}
