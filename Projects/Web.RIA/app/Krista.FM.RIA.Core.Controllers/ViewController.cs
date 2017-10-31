using System.Collections.Generic;
using System.Web.Mvc;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;

using XBuilderFactory = Krista.FM.RIA.Core.Gui.XBuilderFactory;
using XControl = Krista.FM.RIA.Core.Gui.XControl;

namespace Krista.FM.RIA.Core.Controllers
{
    [ControllerSessionState(ControllerSessionState.Default)]
    public class ViewController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";
        private readonly ViewService viewService;
        private readonly IParametersService parametersService;

        public ViewController(ViewService viewService, IParametersService parametersService)
        {
            this.viewService = viewService;
            this.parametersService = parametersService;
        }

        public ActionResult Show(
            string viewId, 
            [ViewParamsBinder]Dictionary<string, string> parameters)
        {
            Gui.View view = viewService.GetView(viewId);

            if (view.Url.IsNotNullOrEmpty())
            {
                return Redirect(view.Url);
            }

            XControl control = new XBuilderFactory(Scheme, parametersService)
                .Create(view);
            return View(ViewRoot + "View.aspx", control.Create(parameters));
        }
    }
}
